// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.GreaseLump
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class GreaseLump : SpecialAbility, IAbilityEffect
{
  private static List<GreaseLump> sCache;
  public static readonly int EFFECT = "dungeons_grease_ball_impact".GetHashCodeCustom();
  public static readonly int SOUNDHASH = "magick_grease".GetHashCodeCustom();
  private static Model sModel;
  private MissileEntity mMissile;
  private Magicka.GameLogic.Entities.Character mOwner;
  public PlayState mPlayState;
  private VisualEffectReference mEffect;
  private Cue mCue;
  private float mTTL;
  private float mInterval;
  private static readonly int NUM_OF_FIELDS = 0;
  private static readonly int SOUND = "dungeon_slime_cube_death_big".GetHashCodeCustom();
  private static readonly int PULSE = "dungeons_grease_ball".GetHashCodeCustom();

  public static GreaseLump GetInstance()
  {
    if (GreaseLump.sCache.Count <= 0)
      return new GreaseLump();
    GreaseLump instance = GreaseLump.sCache[GreaseLump.sCache.Count - 1];
    GreaseLump.sCache.RemoveAt(GreaseLump.sCache.Count - 1);
    return instance;
  }

  public static void InitializeCache(int iNr, PlayState iPlayState)
  {
    GreaseLump.sCache = new List<GreaseLump>(iNr);
    for (int index = 0; index < iNr; ++index)
      GreaseLump.sCache.Add(new GreaseLump());
  }

  public GreaseLump(Magicka.Animations iAnimation)
    : base(iAnimation, "#magick_grease".GetHashCodeCustom())
  {
    if (GreaseLump.sModel != null)
      return;
    lock (Magicka.Game.Instance.GraphicsDevice)
      GreaseLump.sModel = Magicka.Game.Instance.Content.Load<Model>("Models/Missiles/JudgementSprayMissile");
  }

  private GreaseLump()
    : base(Magicka.Animations.cast_magick_sweep, "#magick_grease".GetHashCodeCustom())
  {
    if (GreaseLump.sModel != null)
      return;
    lock (Magicka.Game.Instance.GraphicsDevice)
      GreaseLump.sModel = Magicka.Game.Instance.Content.Load<Model>("Models/Missiles/JudgementSprayMissile");
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    throw new Exception("Grease can not be cast without an owner!");
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    NetworkState state = NetworkManager.Instance.State;
    if ((state == NetworkState.Client || iOwner is Avatar && (iOwner as Avatar).Player.Gamer is NetworkGamer) && (state != NetworkState.Client || !(iOwner is Avatar) || (iOwner as Avatar).Player.Gamer is NetworkGamer))
      return false;
    base.Execute(iOwner, iPlayState);
    this.mTTL = 1.2f;
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    this.mOwner = iOwner as Magicka.GameLogic.Entities.Character;
    this.mPlayState = iPlayState;
    Vector3 translation = iOwner.CastSource.Translation;
    if (iOwner is Magicka.GameLogic.Entities.Character)
      translation = (iOwner as Magicka.GameLogic.Entities.Character).GetLeftAttachOrientation().Translation;
    Vector3 result = iOwner.Direction with { Y = 0.5f };
    result.Normalize();
    Vector3.Multiply(ref result, 15f, out result);
    this.mMissile = iOwner.GetMissileInstance();
    GreaseLump.SpawnLump(ref this.mMissile, iOwner, ref translation, ref result);
    this.mMissile.FacingVelocity = false;
    if (NetworkManager.Instance.State != NetworkState.Offline)
      NetworkManager.Instance.Interface.SendMessage<SpawnMissileMessage>(ref new SpawnMissileMessage()
      {
        Type = SpawnMissileMessage.MissileType.GreaseLump,
        Handle = this.mMissile.Handle,
        Item = (ushort) 0,
        Owner = iOwner.Handle,
        Position = translation,
        Velocity = result
      });
    return true;
  }

  internal static void SpawnLump(
    ref MissileEntity iMissile,
    ISpellCaster iOwner,
    ref Vector3 iPosition,
    ref Vector3 iVelocity)
  {
    ConditionCollection iConditions;
    lock (ProjectileSpell.sCachedConditions)
      iConditions = ProjectileSpell.sCachedConditions.Dequeue();
    iConditions.Clear();
    Damage iDamage = new Damage(AttackProperties.Damage, Elements.Earth, 100f, 1f);
    iConditions[0].Condition.EventConditionType = EventConditionType.Collision;
    iConditions[0].Condition.Threshold = 0.0f;
    iConditions[0].Add(new EventStorage(new PlayEffectEvent(GreaseLump.EFFECT, false, true)));
    iConditions[0].Add(new EventStorage(new PlaySoundEvent(Banks.Additional, GreaseLump.SOUND, false)));
    iConditions[0].Add(new EventStorage(new SplashEvent(iDamage, 3f)));
    iConditions[0].Add(new EventStorage(new CallbackEvent(new CallbackEvent.ItemCallbackFn(GreaseLump.OnCollision))));
    iConditions[0].Add(new EventStorage(new RemoveEvent()));
    iConditions[1].Condition.EventConditionType = EventConditionType.Hit;
    iConditions[1].Add(new EventStorage(new PlayEffectEvent(GreaseLump.EFFECT, false, true)));
    iConditions[1].Add(new EventStorage(new PlaySoundEvent(Banks.Additional, GreaseLump.SOUND, false)));
    iConditions[1].Add(new EventStorage(new SplashEvent(iDamage, 3f)));
    iConditions[1].Add(new EventStorage(new CallbackEvent(new CallbackEvent.ItemCallbackFn(GreaseLump.OnCollision))));
    iConditions[1].Add(new EventStorage(new RemoveEvent()));
    iConditions[3].Condition.EventConditionType = EventConditionType.Damaged;
    iConditions[3].Condition.Elements = Elements.Fire;
    iConditions[3].Condition.Hitpoints = 20f;
    iConditions[3].Add(new EventStorage(new PlayEffectEvent(GreaseLump.EFFECT, false, true)));
    iConditions[3].Add(new EventStorage(new PlaySoundEvent(Banks.Additional, GreaseLump.SOUND, false)));
    iConditions[3].Add(new EventStorage(new SplashEvent(iDamage, 3f)));
    iConditions[3].Add(new EventStorage(new CallbackEvent(new CallbackEvent.ItemCallbackFn(GreaseLump.OnCollision))));
    iConditions[3].Add(new EventStorage(new RemoveEvent()));
    iConditions[2].Condition.EventConditionType = EventConditionType.Default;
    iConditions[2].Condition.Repeat = true;
    iConditions[2].Add(new EventStorage(new PlayEffectEvent(GreaseLump.PULSE, true)));
    iMissile.Initialize(iOwner as Entity, GreaseLump.sModel.Meshes[0].BoundingSphere.Radius * 0.75f, ref iPosition, ref iVelocity, GreaseLump.sModel, iConditions, false);
    iMissile.Body.AngularVelocity = new Vector3(0.0f, 0.0f, 2f * iMissile.Body.Mass);
    iMissile.Danger = 10f;
    iOwner.PlayState.EntityManager.AddEntity((Entity) iMissile);
    lock (ProjectileSpell.sCachedConditions)
      ProjectileSpell.sCachedConditions.Enqueue(iConditions);
  }

  public bool IsDead => (double) this.mTTL <= 0.0;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mTTL -= iDeltaTime;
    this.mInterval -= iDeltaTime;
    if ((double) this.mInterval >= 0.0)
      return;
    this.mInterval = 0.15f;
    Vector3 position = this.mMissile.Position with
    {
      Y = 0.0f
    };
    Vector3 up = Vector3.Up;
    AnimatedLevelPart iAnimation = (AnimatedLevelPart) null;
    Grease.GreaseField instance = Grease.GreaseField.GetInstance(this.mPlayState);
    instance.Initialize((ISpellCaster) this.mOwner, iAnimation, ref position, ref up);
    this.mPlayState.EntityManager.AddEntity((Entity) instance);
    if (NetworkManager.Instance.State != NetworkState.Server)
      return;
    NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
    {
      Handle = instance.Handle,
      Position = position,
      Direction = up,
      Arg = iAnimation == null ? (int) ushort.MaxValue : (int) iAnimation.Handle,
      Id = (int) this.mOwner.Handle,
      ActionType = TriggerActionType.SpawnGrease
    });
  }

  public void OnRemove()
  {
    this.mTTL = 0.0f;
    EffectManager.Instance.Stop(ref this.mEffect);
    GreaseLump.sCache.Add(this);
  }

  public static void OnCollision(Entity iItem, Entity iTarget)
  {
    if (iTarget is Magicka.GameLogic.Entities.Character character)
    {
      int num = (int) character.AddStatusEffect(new StatusEffect(StatusEffects.Greased, 10f, 1f, 1f, 1f));
    }
    Vector3 result1 = iItem.Direction;
    Vector3 up = Vector3.Up;
    AnimatedLevelPart iAnimation = (AnimatedLevelPart) null;
    for (int index = 0; index < GreaseLump.NUM_OF_FIELDS; ++index)
    {
      Quaternion result2;
      Quaternion.CreateFromYawPitchRoll(3.14159274f / (float) GreaseLump.NUM_OF_FIELDS * (float) index, 0.0f, 0.0f, out result2);
      Vector3.Transform(ref result1, ref result2, out result1);
      float scaleFactor = (float) (1.0 + MagickaMath.Random.NextDouble() * 3.0);
      Vector3 result3;
      Vector3.Multiply(ref result1, scaleFactor, out result3);
      result3 = (iItem.Position + result3) with { Y = 0.0f };
      Grease.GreaseField instance = Grease.GreaseField.GetInstance(iItem.PlayState);
      instance.Initialize((iItem as MissileEntity).Owner as ISpellCaster, iAnimation, ref result3, ref up);
      iItem.PlayState.EntityManager.AddEntity((Entity) instance);
      if (NetworkManager.Instance.State == NetworkState.Server)
        NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
        {
          Handle = instance.Handle,
          Position = result3,
          Direction = result1,
          Arg = iAnimation == null ? (int) ushort.MaxValue : (int) iAnimation.Handle,
          Id = (int) (iItem as MissileEntity).Owner.Handle,
          ActionType = TriggerActionType.SpawnGrease
        });
    }
  }
}
