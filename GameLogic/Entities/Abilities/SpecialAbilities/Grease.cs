// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Grease
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PolygonHead;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class Grease : SpecialAbility, IAbilityEffect
{
  public const float GREASESPRAY_TTL = 0.5f;
  public const int NR_OF_FIELDS = 6;
  public const float NR_OF_FIELDS_F = 6f;
  private static List<Grease> sCache;
  public static readonly int EFFECT = "magick_grease_spray".GetHashCodeCustom();
  public static readonly int SOUNDHASH = "magick_grease".GetHashCodeCustom();
  private Magicka.GameLogic.Entities.Character mOwner;
  private PlayState mPlayState;
  private VisualEffectReference mEffect;
  private Cue mCue;
  private float mTTL;

  public static Grease GetInstance()
  {
    if (Grease.sCache.Count <= 0)
      return new Grease();
    Grease instance = Grease.sCache[Grease.sCache.Count - 1];
    Grease.sCache.RemoveAt(Grease.sCache.Count - 1);
    return instance;
  }

  public static void InitializeCache(int iNr, PlayState iPlayState)
  {
    Grease.sCache = new List<Grease>(iNr);
    for (int index = 0; index < iNr; ++index)
      Grease.sCache.Add(new Grease());
  }

  public Grease(Magicka.Animations iAnimation)
    : base(iAnimation, "#magick_grease".GetHashCodeCustom())
  {
  }

  private Grease()
    : base(Magicka.Animations.cast_magick_sweep, "#magick_grease".GetHashCodeCustom())
  {
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    throw new Exception("Grease can not be cast without an owner!");
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    this.mOwner = iOwner as Magicka.GameLogic.Entities.Character;
    if (this.mOwner == null)
    {
      this.OnRemove();
      return false;
    }
    this.mPlayState = iPlayState;
    this.mTTL = 0.5f;
    this.mCue = AudioManager.Instance.PlayCue(Banks.Spells, Grease.SOUNDHASH, this.mOwner.AudioEmitter);
    Vector3 translation = iOwner.CastSource.Translation;
    Vector3 direction = iOwner.Direction;
    EffectManager.Instance.StartEffect(Grease.EFFECT, ref translation, ref direction, out this.mEffect);
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    return true;
  }

  public bool IsDead => (double) this.mTTL <= 0.0;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mOwner.TurnSpeed = 0.0f;
    float mTtl = this.mTTL;
    this.mTTL -= iDeltaTime;
    Vector3 position1 = this.mOwner.Position;
    Vector3 result1 = this.mOwner.Direction;
    if (NetworkManager.Instance.State != NetworkState.Client)
    {
      Quaternion result2;
      Quaternion.CreateFromYawPitchRoll((float) (-((double) this.mTTL / 0.5 * 6.0 - 2.5) / 2.5 * 0.78539818525314331), 0.0f, 0.0f, out result2);
      float num1 = 6f;
      Segment segment;
      segment.Origin = position1;
      Vector3.Transform(ref result1, ref result2, out result1);
      Vector3.Multiply(ref result1, num1, out segment.Delta);
      GameScene currentScene = this.mOwner.PlayState.Level.CurrentScene;
      List<Shield> shields = this.mOwner.PlayState.EntityManager.Shields;
      float scaleFactor1;
      Vector3 iPosition;
      Vector3 iNormal;
      for (int index = 0; index < shields.Count; ++index)
      {
        if (shields[index].Body.CollisionSkin.SegmentIntersect(out scaleFactor1, out iPosition, out iNormal, segment))
        {
          num1 *= scaleFactor1;
          Vector3.Multiply(ref segment.Delta, scaleFactor1, out segment.Delta);
        }
      }
      if (currentScene.SegmentIntersect(out scaleFactor1, out iPosition, out iNormal, segment))
      {
        num1 *= scaleFactor1;
        Vector3.Multiply(ref segment.Delta, scaleFactor1, out segment.Delta);
      }
      List<Entity> entities = this.mPlayState.EntityManager.GetEntities(position1, num1, true);
      entities.Remove((Entity) this.mOwner);
      for (int index = 0; index < entities.Count; ++index)
      {
        if (entities[index] is Magicka.GameLogic.Entities.Character character && !character.HasStatus(StatusEffects.Greased) && character.ArcIntersect(out Vector3 _, segment.Origin, result1, num1, 0.17453292f, 5f))
        {
          int num2 = (int) character.AddStatusEffect(new StatusEffect(StatusEffects.Greased, 0.0f, 1f, 1f, 1f));
        }
      }
      Vector3 translation = this.mOwner.CastSource.Translation;
      EffectManager.Instance.UpdatePositionDirection(ref this.mEffect, ref translation, ref result1);
      this.mPlayState.EntityManager.ReturnEntityList(entities);
      float scaleFactor2 = num1 - 1.5f;
      float num3 = (float) Math.Floor((double) mTtl / 0.5 * 6.0);
      float num4 = (float) Math.Floor((double) this.mTTL / 0.5 * 6.0);
      if (!((double) num4 < (double) num3 & (double) num4 >= 0.0 & (double) scaleFactor2 > 0.0))
        return;
      Vector3 position2 = this.mOwner.Position;
      result1 = this.mOwner.Direction;
      Quaternion.CreateFromYawPitchRoll((float) (-((double) num4 - 2.5) / 2.5 * 0.78539818525314331), 0.0f, 0.0f, out result2);
      Vector3.Transform(ref result1, ref result2, out result1);
      Vector3.Multiply(ref result1, scaleFactor2, out segment.Delta);
      Vector3.Add(ref segment.Origin, ref segment.Delta, out segment.Origin);
      segment.Origin.Y += 2f;
      segment.Delta.X = 0.0f;
      segment.Delta.Y = -4f;
      segment.Delta.Z = 0.0f;
      AnimatedLevelPart oAnimatedLevelPart;
      if (!currentScene.SegmentIntersect(out scaleFactor1, out iPosition, out iNormal, out oAnimatedLevelPart, segment))
        return;
      Grease.GreaseField instance = Grease.GreaseField.GetInstance(this.mPlayState);
      instance.Initialize((ISpellCaster) this.mOwner, oAnimatedLevelPart, ref iPosition, ref iNormal);
      this.mPlayState.EntityManager.AddEntity((Entity) instance);
      if (NetworkManager.Instance.State != NetworkState.Server)
        return;
      NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
      {
        Handle = instance.Handle,
        Position = iPosition,
        Direction = iNormal,
        Arg = oAnimatedLevelPart == null ? (int) ushort.MaxValue : (int) oAnimatedLevelPart.Handle,
        Id = (int) this.mOwner.Handle,
        ActionType = TriggerActionType.SpawnGrease
      });
    }
    else
    {
      Vector3 translation = this.mOwner.CastSource.Translation;
      EffectManager.Instance.UpdatePositionDirection(ref this.mEffect, ref translation, ref result1);
    }
  }

  public void OnRemove()
  {
    EffectManager.Instance.Stop(ref this.mEffect);
    if (this.mCue.IsPlaying)
      this.mCue.Stop(AudioStopOptions.AsAuthored);
    Grease.sCache.Add(this);
  }

  public class GreaseField : Entity, IDamageable
  {
    public const float GREASEFIELD_TTL = 60f;
    public const float BURN_RATE = 3f;
    public const float GREASE_RADIUS = 1.5f;
    private static List<Grease.GreaseField> sCache;
    public static readonly int BURNING_EFFECT = "magick_grease_burning".GetHashCodeCustom();
    public static readonly int PARTICLE_EFFECT = "magick_grease_particle".GetHashCodeCustom();
    private static Grease.GreaseField sHitListOwner;
    private static HitList sHitlist = new HitList(128 /*0x80*/);
    private float mTTL;
    private float mTemperature;
    private bool mBurning;
    private Damage mBurnDamage;
    private float mRestingTimer;
    private ISpellCaster mOwner;
    private VisualEffectReference mBurnEffect;
    private VisualEffectReference mParticleEffect;
    private new PlayState mPlayState;
    private DecalManager.DecalReference mDecalReference;
    private double mTimeStamp;

    public static Grease.GreaseField GetInstance(PlayState iPlayState)
    {
      Grease.GreaseField instance;
      lock (Grease.GreaseField.sCache)
      {
        instance = Grease.GreaseField.sCache[0];
        Grease.GreaseField.sCache.RemoveAt(0);
        Grease.GreaseField.sCache.Add(instance);
      }
      return instance;
    }

    public static Grease.GreaseField GetSpecificInstance(ushort iHandle)
    {
      Grease.GreaseField fromHandle;
      lock (Grease.GreaseField.sCache)
      {
        fromHandle = Entity.GetFromHandle((int) iHandle) as Grease.GreaseField;
        Grease.GreaseField.sCache.Remove(fromHandle);
        Grease.GreaseField.sCache.Add(fromHandle);
      }
      return fromHandle;
    }

    public static void InitializeCache(int iNr, PlayState iPlayState)
    {
      Grease.GreaseField.sCache = new List<Grease.GreaseField>(iNr);
      for (int index = 0; index < iNr; ++index)
        Grease.GreaseField.sCache.Add(new Grease.GreaseField(iPlayState));
    }

    private GreaseField(PlayState iPlayState)
      : base(iPlayState)
    {
      this.mPlayState = iPlayState;
      this.mBody = new Body();
      this.mCollision = new CollisionSkin(this.mBody);
      this.mCollision.AddPrimitive((Primitive) new Sphere(new Vector3(), 1.5f), 1, new MaterialProperties());
      this.mCollision.callbackFn += new CollisionCallbackFn(this.OnCollision);
      this.mBody.CollisionSkin = this.mCollision;
      this.mBody.Immovable = true;
      this.mBody.Tag = (object) this;
      this.mAudioEmitter.Forward = Vector3.Forward;
      this.mAudioEmitter.Up = Vector3.Up;
      this.mBurnDamage = new Damage();
      this.mBurnDamage.AttackProperty = AttackProperties.Status;
      this.mBurnDamage.Element = Elements.Fire;
      this.mBurnDamage.Amount = Defines.SPELL_DAMAGE_FIRE * 4f;
      this.mBurnDamage.Magnitude = 1f;
      this.mRadius = 1.5f;
    }

    public void Initialize(
      ISpellCaster iOwner,
      AnimatedLevelPart iAnimation,
      ref Vector3 iPosition,
      ref Vector3 iNormal)
    {
      EffectManager.Instance.Stop(ref this.mBurnEffect);
      EffectManager.Instance.Stop(ref this.mParticleEffect);
      float oTTL;
      DecalManager.Instance.GetDecalTTL(ref this.mDecalReference, out oTTL);
      if ((double) oTTL > 1.0)
        DecalManager.Instance.SetDecalTTL(ref this.mDecalReference, 1f);
      this.mOwner = iOwner;
      this.mPlayState = iOwner.PlayState;
      this.mTTL = 60f;
      this.mTemperature = 0.0f;
      this.mBurning = false;
      this.mTimeStamp = iOwner.PlayState.PlayTime;
      this.mDead = false;
      this.mBody.MoveTo(iPosition, Matrix.Identity);
      this.mBody.CollisionSkin.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
      iAnimation?.AddEntity((Entity) this);
      Vector2 iScale = new Vector2();
      iScale.X = iScale.Y = 3f;
      DecalManager.Instance.AddAlphaBlendedDecal(Decal.Grease, iAnimation, ref iScale, ref iPosition, new Vector3?(), ref iNormal, this.mTTL, 1f, out this.mDecalReference);
      this.Initialize();
      this.mAudioEmitter.Position = iPosition;
      Vector3 forward = Vector3.Forward;
      EffectManager.Instance.StartEffect(Grease.GreaseField.PARTICLE_EFFECT, ref iPosition, ref forward, out this.mParticleEffect);
    }

    public bool Resting => (double) this.mRestingTimer < 0.0;

    public override void Deinitialize()
    {
      if (Grease.GreaseField.sHitListOwner == this)
        Grease.GreaseField.sHitListOwner = (Grease.GreaseField) null;
      EffectManager.Instance.Stop(ref this.mBurnEffect);
      EffectManager.Instance.Stop(ref this.mParticleEffect);
      float oTTL;
      DecalManager.Instance.GetDecalTTL(ref this.mDecalReference, out oTTL);
      if ((double) oTTL > 1.0)
        DecalManager.Instance.SetDecalTTL(ref this.mDecalReference, 1f);
      this.mOwner = (ISpellCaster) null;
      base.Deinitialize();
    }

    public override void Update(DataChannel iDataChannel, float iDeltaTime)
    {
      if (this.mBurning)
      {
        this.mTemperature = Math.Min(1f, this.mTemperature + iDeltaTime * 0.5f);
        this.mTTL -= iDeltaTime * 3f;
        List<Entity> entities = this.PlayState.EntityManager.GetEntities(this.Position, 1.5f, false);
        entities.Remove((Entity) this);
        for (int index = 0; index < entities.Count; ++index)
        {
          if (entities[index] is IDamageable t && !Grease.GreaseField.sHitlist.ContainsKey(t.Handle))
          {
            Grease.GreaseField.sHitlist.Add(t.Handle);
            int num = (int) t.Damage(this.mBurnDamage, this.mOwner as Entity, this.mTimeStamp, this.Position);
          }
        }
        this.PlayState.EntityManager.ReturnEntityList(entities);
      }
      else
        this.mTTL -= iDeltaTime;
      DecalManager.Instance.SetDecalTTL(ref this.mDecalReference, this.mTTL);
      this.mDead = (double) this.mTTL < 0.0;
      if (Grease.GreaseField.sHitListOwner == null)
        Grease.GreaseField.sHitListOwner = this;
      if (Grease.GreaseField.sHitListOwner == this)
        Grease.GreaseField.sHitlist.Update(iDeltaTime);
      base.Update(iDataChannel, iDeltaTime);
      if ((double) this.mBody.Velocity.LengthSquared() > 9.9999999747524271E-07)
        this.mRestingTimer = 1f;
      else
        this.mRestingTimer -= iDeltaTime;
    }

    public bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
    {
      if (iSkin1.Owner != null && iSkin1.Owner.Tag is Magicka.GameLogic.Entities.Character tag && !tag.IsNonslippery)
        tag.CharacterBody.IsOnGrease = true;
      return false;
    }

    public float ResistanceAgainst(Elements iElement) => 0.0f;

    public override bool Dead => this.mDead;

    public override bool Removable => this.mDead;

    public override void Kill()
    {
      this.mTTL = 0.0f;
      this.mDead = true;
    }

    public float HitPoints => this.mTTL;

    public float MaxHitPoints => 60f;

    public bool SegmentIntersect(out Vector3 oPosition, Segment iSeg, float iSegmentRadius)
    {
      return this.mCollision.SegmentIntersect(out float _, out oPosition, out Vector3 _, iSeg);
    }

    public DamageResult InternalDamage(
      DamageCollection5 iDamages,
      Entity iAttacker,
      double iTimeStamp,
      Vector3 iAttackPosition,
      Defines.DamageFeatures iFeatures)
    {
      return DamageResult.None | this.InternalDamage(iDamages.A, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.B, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.C, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.D, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.E, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
    }

    public DamageResult InternalDamage(
      Damage iDamage,
      Entity iAttacker,
      double iTimeStamp,
      Vector3 iAttackPosition,
      Defines.DamageFeatures iFeatures)
    {
      if ((iDamage.Element & Elements.Fire) == Elements.Fire)
      {
        this.mTemperature = Math.Min(1f, this.mTemperature + iDamage.Magnitude);
        if (!this.mBurning & (double) this.mTemperature >= 1.0)
        {
          this.mBurning = true;
          Vector3 position = this.Position;
          EffectManager.Instance.StartEffect(Grease.GreaseField.BURNING_EFFECT, ref position, ref new Vector3()
          {
            Z = -1f
          }, out this.mBurnEffect);
        }
      }
      else if (SpellManager.InclusiveOpposites(iDamage.Element, Elements.Fire))
      {
        this.mTemperature = Math.Max(0.0f, this.mTemperature - iDamage.Magnitude);
        if (this.mBurning & (double) this.mTemperature <= 0.0)
        {
          this.mBurning = false;
          EffectManager.Instance.Stop(ref this.mBurnEffect);
        }
      }
      return DamageResult.None;
    }

    internal void Burn(float iTemperature)
    {
      this.mTemperature = Math.Min(1f, this.mTemperature + iTemperature);
      if (!(!this.mBurning & (double) this.mTemperature >= 1.0))
        return;
      this.mBurning = true;
      this.mTemperature = iTemperature;
      Vector3 position = this.Position;
      EffectManager.Instance.StartEffect(Grease.GreaseField.BURNING_EFFECT, ref position, ref new Vector3()
      {
        Z = -1f
      }, out this.mBurnEffect);
    }

    public void OverKill()
    {
      this.mTTL = 0.0f;
      this.mDead = true;
    }

    protected override void INetworkUpdate(ref EntityUpdateMessage iMsg)
    {
      base.INetworkUpdate(ref iMsg);
      bool genericBool = iMsg.GenericBool;
      if (!this.mBurning && genericBool)
      {
        Vector3 position = this.Position;
        EffectManager.Instance.StartEffect(Grease.GreaseField.BURNING_EFFECT, ref position, ref new Vector3()
        {
          Z = -1f
        }, out this.mBurnEffect);
      }
      if ((iMsg.Features & EntityFeatures.GenericFloat) == EntityFeatures.GenericFloat)
      {
        this.mTemperature = iMsg.GenericFloat;
      }
      else
      {
        this.mTemperature = 0.0f;
        EffectManager.Instance.Stop(ref this.mBurnEffect);
      }
      this.mBurning = genericBool;
    }

    protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
    {
      oMsg = new EntityUpdateMessage();
      if (!this.Resting)
      {
        oMsg.Features |= EntityFeatures.Position;
        oMsg.Position = this.Position;
      }
      oMsg.Features |= EntityFeatures.GenericBool;
      oMsg.GenericBool = this.mBurning;
      if (!this.mBurning || (double) this.mTemperature <= 0.0)
        return;
      oMsg.Features |= EntityFeatures.GenericFloat;
      oMsg.GenericFloat = this.mTemperature;
    }

    public void Electrocute(IDamageable iTarget, float iMultiplyer)
    {
    }

    internal override float GetDanger() => !this.mBurning ? 0.5f : this.mTemperature * 10f;
  }
}
