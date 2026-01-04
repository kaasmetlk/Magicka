// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Confuse
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

internal class Confuse : SpecialAbility, IAbilityEffect, ITargetAbility
{
  private const float DEFAULT_TTL = 15f;
  private const float RADIUS = 14f;
  private static List<Confuse> sCache;
  private static List<Confuse> sActiveCaches;
  private static readonly int SOUND_EFFECT = "boss_fafnir_confuse".GetHashCodeCustom();
  private static readonly int EFFECT_ID = "confused".GetHashCodeCustom();
  private float mTTL;
  private Magicka.GameLogic.Entities.Character mTarget;
  private Controller mControllerTarget;
  private VisualEffectReference mEffect;

  public static Confuse GetInstance()
  {
    if (Confuse.sCache.Count > 0)
    {
      Confuse instance = Confuse.sCache[Confuse.sCache.Count - 1];
      Confuse.sCache.RemoveAt(Confuse.sCache.Count - 1);
      Confuse.sActiveCaches.Add(instance);
      return instance;
    }
    Confuse instance1 = new Confuse();
    Confuse.sActiveCaches.Add(instance1);
    return instance1;
  }

  public static void InitializeCache(int iNr)
  {
    Confuse.sCache = new List<Confuse>(iNr);
    Confuse.sActiveCaches = new List<Confuse>(iNr);
    for (int index = 0; index < iNr; ++index)
      Confuse.sCache.Add(new Confuse());
  }

  public float TTL
  {
    get => this.mTTL;
    set => this.mTTL = value;
  }

  private Confuse()
    : base(Magicka.Animations.cast_magick_direct, "#magick_confuse".GetHashCodeCustom())
  {
    this.mTTL = 15f;
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    base.Execute(iPosition, iPlayState);
    IDamageable closestIdamageable = iPlayState.EntityManager.GetClosestIDamageable((IDamageable) null, iPosition, 2f, false);
    if (closestIdamageable == null)
    {
      this.OnRemove();
      return false;
    }
    for (int index = 0; index < Confuse.sActiveCaches.Count; ++index)
    {
      if (Confuse.sActiveCaches[index].mTarget == closestIdamageable)
      {
        Confuse.sActiveCaches[index].mTTL = 15f;
        this.OnRemove();
        return true;
      }
    }
    return this.Execute((ISpellCaster) null, closestIdamageable as Entity, iPlayState);
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    Vector3 position1 = iOwner.Position;
    Vector3 direction = iOwner.Direction;
    List<Entity> entities = iPlayState.EntityManager.GetEntities(position1, 14f, true);
    float num1 = float.MaxValue;
    Magicka.GameLogic.Entities.Character iTarget = (Magicka.GameLogic.Entities.Character) null;
    for (int index = 0; index < entities.Count; ++index)
    {
      if (entities[index] is Magicka.GameLogic.Entities.Character && entities[index] != iOwner)
      {
        Vector3 position2 = entities[index].Position;
        Vector3 result;
        Vector3.Subtract(ref position2, ref position1, out result);
        result.Y = 0.0f;
        result.Normalize();
        float num2 = MagickaMath.Angle(ref direction, ref result);
        if ((double) num2 < (double) num1)
        {
          num1 = num2;
          iTarget = entities[index] as Magicka.GameLogic.Entities.Character;
        }
      }
    }
    iPlayState.EntityManager.ReturnEntityList(entities);
    if (iTarget != null)
      return this.Execute(iOwner, (Entity) iTarget, iPlayState);
    AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL, iOwner.AudioEmitter);
    this.OnRemove();
    return false;
  }

  public bool Execute(ISpellCaster iOwner, Entity iTarget, PlayState iPlayState)
  {
    if (!(iTarget is Magicka.GameLogic.Entities.Character))
      return false;
    for (int index = 0; index < Confuse.sActiveCaches.Count; ++index)
    {
      if (Confuse.sActiveCaches[index].mTarget == iTarget)
      {
        Confuse.sActiveCaches[index].mTTL = 15f;
        AudioManager.Instance.PlayCue(Banks.Characters, Confuse.SOUND_EFFECT, iOwner.AudioEmitter);
        Confuse.sCache.Add(this);
        return true;
      }
    }
    this.mTarget = iTarget as Magicka.GameLogic.Entities.Character;
    if (this.mTarget is Avatar && (this.mTarget as Avatar).Player != null && !((this.mTarget as Avatar).Player.Gamer is NetworkGamer))
    {
      this.mControllerTarget = (this.mTarget as Avatar).Player.Controller;
      this.mControllerTarget.Invert(true);
    }
    else if (this.mTarget is NonPlayerCharacter)
    {
      if (NetworkManager.Instance.State == NetworkState.Server)
        NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
        {
          Handle = this.mTarget.Handle,
          Bool0 = true,
          ActionType = TriggerActionType.Confuse
        });
      (this.mTarget as NonPlayerCharacter).Confuse(Factions.NONE);
    }
    this.mTTL = 15f;
    AudioManager.Instance.PlayCue(Banks.Characters, Confuse.SOUND_EFFECT, iTarget.AudioEmitter);
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    return true;
  }

  public bool IsDead => (double) this.mTTL <= 0.0;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if (this.mTarget == null || (double) this.mTarget.HitPoints <= 0.0)
    {
      this.mTTL = 0.0f;
    }
    else
    {
      this.mTTL -= iDeltaTime;
      Matrix identity = Matrix.Identity with
      {
        Translation = this.mTarget.Position
      };
      if (EffectManager.Instance.UpdateOrientation(ref this.mEffect, ref identity))
        return;
      EffectManager.Instance.StartEffect(Confuse.EFFECT_ID, ref identity, out this.mEffect);
    }
  }

  public void OnRemove()
  {
    EffectManager.Instance.Stop(ref this.mEffect);
    if (this.mControllerTarget != null)
      this.mControllerTarget.Invert(false);
    if ((!(this.mTarget is Avatar) || (this.mTarget as Avatar).Player == null || (this.mTarget as Avatar).Player.Gamer is NetworkGamer) && this.mTarget is NonPlayerCharacter)
    {
      if (NetworkManager.Instance.State == NetworkState.Server)
        NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
        {
          Handle = this.mTarget.Handle,
          Bool0 = false,
          ActionType = TriggerActionType.Confuse
        });
      (this.mTarget as NonPlayerCharacter).Confuse(this.mTarget.Template.Faction);
    }
    this.mControllerTarget = (Controller) null;
    this.mTarget = (Magicka.GameLogic.Entities.Character) null;
    this.mTTL = 0.0f;
    Confuse.sActiveCaches.Remove(this);
    Confuse.sCache.Add(this);
  }
}
