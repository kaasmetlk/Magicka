// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.EtherealClone
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.AI;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.GameStates;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class EtherealClone : SpecialAbility
{
  private readonly int SPREAD = 12;
  protected static readonly int HUMAN_ADVENTURER = "dungeons_human_adventurer_clone".GetHashCodeCustom();
  protected ISpellCaster mOwner;
  protected PlayState mPlayState;
  public Magicka.Animations IdleAnimation;
  public Magicka.Animations SpawnAnimation;
  private static EtherealClone sSingelton;
  private static volatile object sSingeltonLock = new object();

  internal static void InitializeCache(PlayState iPlayState)
  {
    iPlayState.Content.Load<CharacterTemplate>("data/characters/dungeons_human_adventurer_clone");
  }

  public static EtherealClone Instance
  {
    get
    {
      if (EtherealClone.sSingelton == null)
      {
        lock (EtherealClone.sSingeltonLock)
        {
          if (EtherealClone.sSingelton == null)
            EtherealClone.sSingelton = new EtherealClone();
        }
      }
      return EtherealClone.sSingelton;
    }
  }

  private EtherealClone()
    : base(Magicka.Animations.cast_magick_direct, "#Ent_Dungeons_Slime_Medium".GetHashCodeCustom())
  {
  }

  public EtherealClone(Magicka.Animations iAnimation)
    : base(iAnimation, "#Ent_Dungeons_Slime_Medium".GetHashCodeCustom())
  {
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    throw new Exception("Ethereal clone cannot be spawned without a parent (iOwner)!");
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    if (NetworkManager.Instance.State == NetworkState.Client)
      return false;
    this.mOwner = iOwner;
    this.mPlayState = iPlayState;
    CharacterTemplate template;
    int iTemplateId;
    uint iAmount;
    switch ((iOwner as Character).Name)
    {
      case "dungeons_human_adventurer":
        template = CharacterTemplate.GetCachedTemplate(EtherealClone.HUMAN_ADVENTURER);
        iTemplateId = EtherealClone.HUMAN_ADVENTURER;
        iAmount = 1U;
        break;
      default:
        template = (CharacterTemplate) null;
        iTemplateId = -1;
        iAmount = 0U;
        break;
    }
    if (template == null)
      return false;
    this.SpawnClone(template, iTemplateId, iAmount);
    return true;
  }

  protected void SpawnClone(CharacterTemplate template, int iTemplateId, uint iAmount)
  {
    Vector3 position = this.mOwner.Position;
    Vector3 direction = this.mOwner.Direction;
    for (int index = 0; (long) index < (long) iAmount; ++index)
    {
      NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(this.mOwner.PlayState);
      Vector3 vector3;
      vector3.X = (float) (MagickaMath.Random.NextDouble() - 0.5) * (float) this.SPREAD;
      vector3.Y = position.Y;
      vector3.Z = (float) (MagickaMath.Random.NextDouble() - 0.5) * (float) this.SPREAD;
      Vector3 iPoint = position + vector3;
      Vector3 oPoint;
      double nearestPosition = (double) this.mPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref iPoint, out oPoint, MovementProperties.Default);
      if (NetworkManager.Instance.State == NetworkState.Server)
        NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
        {
          ActionType = TriggerActionType.SpawnNPC,
          Handle = instance.Handle,
          Template = iTemplateId,
          Id = 0,
          Position = oPoint,
          Direction = direction,
          Bool0 = false,
          Point0 = 0,
          Point1 = 0,
          Point2 = (int) this.SpawnAnimation,
          Point3 = (int) this.IdleAnimation
        });
      instance.Initialize(template, oPoint, 0);
      if (this.mOwner is Character)
        instance.Faction = (this.mOwner as Character).Faction;
      if (this.mOwner.PlayState.Level.CurrentScene.RuleSet != null && this.mOwner.PlayState.Level.CurrentScene.RuleSet is SurvivalRuleset)
        (this.mOwner.PlayState.Level.CurrentScene.RuleSet as SurvivalRuleset).AddedCharacter(instance, true);
      instance.HitPoints = instance.MaxHitPoints;
      instance.AI.SetOrder(Order.Attack, ReactTo.Proximity, Order.Attack, 0, 0, 0, (AIEvent[]) null);
      if (this.SpawnAnimation != Magicka.Animations.None && this.SpawnAnimation != Magicka.Animations.idle && this.SpawnAnimation != Magicka.Animations.idle_agg)
      {
        instance.SpawnAnimation = this.SpawnAnimation;
        instance.ChangeState((BaseState) RessurectionState.Instance);
      }
      if (this.IdleAnimation != Magicka.Animations.None)
        instance.SpecialIdleAnimation = this.IdleAnimation;
      this.mOwner.PlayState.EntityManager.AddEntity((Entity) instance);
    }
  }
}
