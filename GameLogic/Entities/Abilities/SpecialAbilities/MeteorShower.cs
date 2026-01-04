// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.MeteorShower
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class MeteorShower : SpecialAbility, IAbilityEffect
{
  private const float TIME_BETWEEN_METEORS = 0.4f;
  private const float LIFE_TIME = 17.5f;
  private static MeteorShower mSingelton;
  private static volatile object mSingeltonLock = new object();
  public static readonly ConditionCollection CONDITIONS;
  public static readonly Model MODEL;
  private static readonly int SOUND_RUMBLE = "magick_meteor_rumble".GetHashCodeCustom();
  private static readonly int SOUND_PREBLAST = "magick_meteor_preblast".GetHashCodeCustom();
  private float mMeteorSpawnTimer;
  private float mTTL;
  private GameScene mScene;
  private PlayState mPlayState;
  private ISpellCaster mOwner;
  private Vector3 mPosition;
  private Cue mRumble;

  public static MeteorShower Instance
  {
    get
    {
      if (MeteorShower.mSingelton == null)
      {
        lock (MeteorShower.mSingeltonLock)
        {
          if (MeteorShower.mSingelton == null)
            MeteorShower.mSingelton = new MeteorShower();
        }
      }
      return MeteorShower.mSingelton;
    }
  }

  private MeteorShower()
    : base(Magicka.Animations.cast_magick_global, "#magick_meteors".GetHashCodeCustom())
  {
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    if (iPlayState.Level.CurrentScene.Indoors)
    {
      AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL);
      return false;
    }
    if ((double) this.mTTL > 0.0)
    {
      this.mPosition = iPosition;
      this.mTTL = 17.5f;
      return true;
    }
    this.mPosition = iPosition;
    this.mOwner = (ISpellCaster) null;
    this.mPlayState = iPlayState;
    return this.Execute();
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    if (iPlayState.Level.CurrentScene.Indoors)
    {
      AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL);
      return false;
    }
    if ((double) this.mTTL > 0.0)
    {
      this.mTTL = 17.5f;
      return true;
    }
    this.mOwner = iOwner;
    this.mPlayState = iPlayState;
    return this.Execute();
  }

  private bool Execute()
  {
    this.mScene = this.mPlayState.Level.CurrentScene;
    this.mTTL = 17.5f;
    this.mScene.LightTargetIntensity = 0.4f;
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    if (this.mRumble != null && !this.mRumble.IsPlaying)
      this.mRumble = AudioManager.Instance.PlayCue(Banks.Spells, MeteorShower.SOUND_RUMBLE);
    return true;
  }

  public bool IsDead => (double) this.mTTL <= 0.0;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mTTL -= iDeltaTime;
    if (NetworkManager.Instance.State == NetworkState.Client || (double) this.mTTL <= 2.5)
      return;
    this.mMeteorSpawnTimer -= iDeltaTime;
    if ((double) this.mMeteorSpawnTimer > 0.0)
      return;
    AudioManager.Instance.PlayCue(Banks.Spells, "magick_meteor_preblast".GetHashCodeCustom());
    Vector3 vector3 = this.mOwner != null ? this.mOwner.Position : this.mPosition;
    this.mMeteorSpawnTimer += (float) (0.40000000596046448 + SpecialAbility.RANDOM.NextDouble() * 0.40000000596046448);
    int num1 = SpecialAbility.RANDOM.Next(2) + 1;
    for (int index = 0; index < num1; ++index)
    {
      Vector3 iPosition = vector3;
      float num2 = (float) Math.Sqrt(SpecialAbility.RANDOM.NextDouble());
      float num3 = (float) (SpecialAbility.RANDOM.NextDouble() * 6.2831854820251465);
      float num4 = num2 * (float) Math.Cos((double) num3);
      float num5 = num2 * (float) Math.Sin((double) num3);
      iPosition.X += 20f * num4;
      iPosition.Z += 20f * num5;
      iPosition.Y += 146f;
      Vector3 iVelocity = new Vector3(0.01f, -20f, -0.01f);
      Spell iSpell = new Spell();
      iSpell.Element = Elements.Earth | Elements.Fire;
      iSpell.EarthMagnitude = 5f;
      iSpell.FireMagnitude = 5f;
      MissileEntity iMissile = this.mOwner == null ? MissileEntity.GetInstance(this.mPlayState) : this.mOwner.GetMissileInstance();
      ProjectileSpell.SpawnMissile(ref iMissile, this.mOwner, 0.0f, ref iPosition, ref iVelocity, ref iSpell, 4f, 1);
      AudioManager.Instance.PlayCue(Banks.Spells, MeteorShower.SOUND_PREBLAST, iMissile.AudioEmitter);
      if (NetworkManager.Instance.State == NetworkState.Server)
        NetworkManager.Instance.Interface.SendMessage<SpawnMissileMessage>(ref new SpawnMissileMessage()
        {
          Type = SpawnMissileMessage.MissileType.Spell,
          Handle = iMissile.Handle,
          Item = (ushort) 0,
          Owner = this.mOwner.Handle,
          Position = iPosition,
          Velocity = iVelocity,
          Spell = iSpell,
          Homing = 0.0f,
          Splash = 4f
        });
    }
  }

  public static void SpawnMissile(
    MissileEntity iMissile,
    Entity iOwner,
    ref Vector3 iPosition,
    ref Vector3 iVelocity,
    float iRadius)
  {
    iMissile.Initialize(iOwner, iRadius, ref iPosition, ref iVelocity, MeteorShower.MODEL, MeteorShower.CONDITIONS, true);
    iMissile.PlayState.EntityManager.AddEntity((Entity) iMissile);
  }

  public void OnRemove()
  {
    this.mTTL = 0.0f;
    this.mScene.LightTargetIntensity = 1f;
    if (this.mRumble == null || this.mRumble.IsStopping)
      return;
    this.mRumble.Stop(AudioStopOptions.AsAuthored);
  }
}
