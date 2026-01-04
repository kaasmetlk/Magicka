// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.EarthQuake
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PolygonHead;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

internal class EarthQuake : SpecialAbility, IAbilityEffect
{
  private const float TTL = 16f;
  private const float HIT_TTL = 0.5f;
  private static EarthQuake sSingelton;
  private static volatile object sSingeltonLock = new object();
  private static readonly int MAGICK_EFFECT = "magick_earthquake".GetHashCodeCustom();
  private static readonly int MAGICK_EFFECT_HIT = "magick_earthquake_hit".GetHashCodeCustom();
  private static readonly int MAGICK_SOUND = "magick_earthquake".GetHashCodeCustom();
  private float mTTL;
  private Vector3 mEpicenter;
  private ISpellCaster mOwner;
  private PlayState mPlayState;
  private Cue mQuakeCue;
  private AudioEmitter mQuakeEmitter = new AudioEmitter();
  private VisualEffectReference mQuakeEffect;
  private float mQuakeTTL;
  private Vector3 mQuakePosition;
  private Damage mDamage;
  private float mHitTimer;
  private HitList mHitList = new HitList(128 /*0x80*/);

  public static EarthQuake Instance
  {
    get
    {
      if (EarthQuake.sSingelton == null)
      {
        lock (EarthQuake.sSingeltonLock)
        {
          if (EarthQuake.sSingelton == null)
            EarthQuake.sSingelton = new EarthQuake();
        }
      }
      return EarthQuake.sSingelton;
    }
  }

  private EarthQuake()
    : base(Magicka.Animations.cast_magick_global, "#magick_earthquake".GetHashCodeCustom())
  {
    this.mDamage = new Damage(AttackProperties.Knockdown, Elements.Earth, 1000f, 1f);
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    if (this.Execute(iOwner.Position, iPlayState))
    {
      this.mOwner = iOwner;
      return true;
    }
    AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL, iOwner.AudioEmitter);
    return false;
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    this.mOwner = (ISpellCaster) null;
    this.mPlayState = iPlayState;
    this.mTTL = 16f;
    this.mHitTimer = 0.5f;
    Vector3 oPos;
    if (!this.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out Vector3 _, new Segment()
    {
      Origin = iPosition,
      Delta = {
        Y = -20f
      }
    }))
      return false;
    this.mEpicenter = oPos;
    if (!SpellManager.Instance.IsEffectActive(typeof (EarthQuake)))
      SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    return base.Execute(iPosition, iPlayState);
  }

  public bool IsDead => (double) this.mTTL <= 0.0;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mTTL -= iDeltaTime;
    this.mHitTimer -= iDeltaTime;
    this.mQuakeTTL -= iDeltaTime;
    this.mHitList.Update(iDeltaTime);
    if (NetworkManager.Instance.State != NetworkState.Client && (double) this.mQuakeTTL <= 0.0)
    {
      float num1 = (float) Math.Sqrt(SpecialAbility.RANDOM.NextDouble());
      float num2 = (float) SpecialAbility.RANDOM.NextDouble() * 6.28318548f;
      float num3 = num1 * (float) Math.Cos((double) num2);
      float num4 = num1 * (float) Math.Sin((double) num2);
      Vector3 iPosition;
      iPosition.X = this.mEpicenter.X + num3 * 15f;
      iPosition.Z = this.mEpicenter.Z + num4 * 15f;
      iPosition.Y = this.mEpicenter.Y + 0.5f;
      float iTTL = (float) SpecialAbility.RANDOM.NextDouble() + 1f;
      this.NewQuake(ref iPosition, iTTL);
      if (NetworkManager.Instance.State == NetworkState.Server)
        NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
        {
          Handle = this.mOwner.Handle,
          Position = this.mQuakePosition,
          ActionType = TriggerActionType.EarthQuake,
          Time = iTTL
        });
    }
    if ((double) this.mHitTimer > 0.0)
      return;
    this.Quake(ref this.mQuakePosition, 10f);
    this.mHitTimer = 0.5f;
  }

  public void OnRemove()
  {
    if (this.mQuakeCue != null && this.mQuakeCue.IsPlaying)
      this.mQuakeCue.Stop(AudioStopOptions.AsAuthored);
    if (!EffectManager.Instance.IsActive(ref this.mQuakeEffect))
      return;
    EffectManager.Instance.Stop(ref this.mQuakeEffect);
  }

  public void NewQuake(ref Vector3 iPosition, float iTTL)
  {
    this.mQuakePosition = iPosition;
    this.mQuakeTTL = iTTL;
    this.mPlayState.Camera.CameraShake(this.mQuakePosition, 0.5f, this.mQuakeTTL);
  }

  private void Quake(ref Vector3 iPosition, float iRadius)
  {
    List<Entity> entities = this.mPlayState.EntityManager.GetEntities(iPosition, iRadius, false, false);
    iPosition.Y -= 5f;
    for (int index = 0; index < entities.Count; ++index)
    {
      if (!this.mHitList.ContainsKey(entities[index].Handle))
      {
        if (entities[index] is IDamageable)
        {
          IDamageable t = entities[index] as IDamageable;
          if (entities[index] is Magicka.GameLogic.Entities.Character)
          {
            Magicka.GameLogic.Entities.Character character = entities[index] as Magicka.GameLogic.Entities.Character;
            if (!character.IsLevitating && (double) character.CharacterBody.Mass < 3000.0 && !character.IsImmortal && !character.IsEthereal && !character.IsInAEvent && character.CharacterBody.IsTouchingGround)
            {
              Vector3 iImpulse = new Vector3(0.0f, 6f, 0.0f);
              character.CharacterBody.AddImpulseVelocity(ref iImpulse);
            }
            else
              continue;
          }
          this.mDamage.Amount = t.Body.Mass;
          Vector3 position = t.Position;
          position.X += (float) SpecialAbility.RANDOM.NextDouble() - 0.5f;
          position.Z += (float) SpecialAbility.RANDOM.NextDouble() - 0.5f;
          int num = (int) t.Damage(this.mDamage, this.mOwner as Entity, this.mTimeStamp, position);
        }
        else
        {
          Entity entity = entities[index];
          Vector3 velocity = entity.Body.Velocity;
          velocity.Y += 5f;
          velocity.X += (float) SpecialAbility.RANDOM.NextDouble() - 0.5f;
          velocity.Z += (float) SpecialAbility.RANDOM.NextDouble() - 0.5f;
          entity.Body.Velocity = velocity;
        }
        this.mHitList.Add(entities[index]);
      }
    }
    this.mPlayState.EntityManager.ReturnEntityList(entities);
    if (this.mQuakeCue != null && this.mQuakeCue.IsPlaying)
      this.mQuakeCue.Stop(AudioStopOptions.AsAuthored);
    if (EffectManager.Instance.IsActive(ref this.mQuakeEffect))
      EffectManager.Instance.Stop(ref this.mQuakeEffect);
    this.mQuakeCue = AudioManager.Instance.PlayCue(Banks.Additional, EarthQuake.MAGICK_SOUND, this.mQuakeEmitter);
    this.mQuakeEmitter.Position = iPosition;
    this.mQuakeEmitter.Up = Vector3.Up;
    this.mQuakeEmitter.Forward = Vector3.Right;
    EffectManager.Instance.StartEffect(EarthQuake.MAGICK_EFFECT, ref new Matrix()
    {
      M11 = iRadius / 5f,
      M22 = 1f,
      M33 = iRadius / 5f,
      M44 = 1f,
      Translation = iPosition
    }, out this.mQuakeEffect);
  }
}
