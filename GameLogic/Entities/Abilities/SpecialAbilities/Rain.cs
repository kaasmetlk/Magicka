// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Rain
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PolygonHead;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class Rain : SpecialAbility, IAbilityEffect
{
  private const float MAGICK_TTL = 17.5f;
  private static Rain mSingelton;
  private static volatile object mSingeltonLock = new object();
  private float mTTL;
  private float mWetTimer;
  private VisualEffectReference mEffect;
  public static readonly int AMBIENCE = "magick_rain".GetHashCodeCustom();
  public static readonly int EFFECT = "magick_rain".GetHashCodeCustom();
  private PlayState mPlayState;
  private GameScene mScene;
  private Damage mDamage;
  private Cue mAmbience;
  private ISpellCaster mCaster;
  private bool mDoDamage;

  public static Rain Instance
  {
    get
    {
      if (Rain.mSingelton == null)
      {
        lock (Rain.mSingeltonLock)
        {
          if (Rain.mSingelton == null)
            Rain.mSingelton = new Rain();
        }
      }
      return Rain.mSingelton;
    }
  }

  private Rain()
    : base(Magicka.Animations.cast_magick_global, "#magick_rain".GetHashCodeCustom())
  {
    this.mDamage.AttackProperty = AttackProperties.Status;
    this.mDamage.Element = Elements.Water;
  }

  public Rain(Magicka.Animations iAnimation)
    : base(iAnimation, "#magick_rain".GetHashCodeCustom())
  {
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    this.mCaster = (ISpellCaster) null;
    this.mTimeStamp = 0.0;
    this.mPlayState = iPlayState;
    this.mDoDamage = NetworkManager.Instance.State != NetworkState.Client;
    return this.Execute();
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    this.mCaster = iOwner;
    this.mTimeStamp = iOwner.PlayState.PlayTime;
    this.mPlayState = iPlayState;
    Avatar avatar = iOwner as Avatar;
    this.mDoDamage = NetworkManager.Instance.State != NetworkState.Client ? NetworkManager.Instance.State != NetworkState.Server || avatar == null || !(avatar.Player.Gamer is NetworkGamer) : avatar != null && !(avatar.Player.Gamer is NetworkGamer);
    return this.Execute();
  }

  private bool Execute()
  {
    if (this.mPlayState.Level.CurrentScene.Indoors)
      return false;
    Vector3 result = this.mPlayState.Camera.Position;
    Vector3 cameraoffset = MagickCamera.CAMERAOFFSET;
    Vector3 iDirection = new Vector3();
    iDirection.Z = -1f;
    Vector3.Subtract(ref result, ref cameraoffset, out result);
    if (!EffectManager.Instance.IsActive(ref this.mEffect))
      EffectManager.Instance.StartEffect(Rain.EFFECT, ref result, ref iDirection, out this.mEffect);
    this.mScene = this.mPlayState.Level.CurrentScene;
    if (this.mAmbience == null || !this.mAmbience.IsPlaying)
      this.mAmbience = AudioManager.Instance.PlayCue(Banks.Spells, Rain.AMBIENCE);
    this.mTTL = 17.5f;
    this.mScene.LightTargetIntensity = 0.333f;
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    if (this.mCaster is Avatar)
      this.mPlayState.IncrementBlizzardRainCount();
    return true;
  }

  public bool IsDead => (double) this.mTTL <= 0.0;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mTTL -= iDeltaTime;
    this.mWetTimer -= iDeltaTime;
    Vector3 result = this.mPlayState.Camera.Position;
    Vector3 cameraoffset = MagickCamera.CAMERAOFFSET;
    Vector3 iDirection = new Vector3();
    iDirection.Z = -1f;
    Vector3.Subtract(ref result, ref cameraoffset, out result);
    EffectManager.Instance.UpdatePositionDirection(ref this.mEffect, ref result, ref iDirection);
    if (!this.mDoDamage)
      return;
    this.mDamage.Magnitude = 0.25f;
    if ((double) this.mWetTimer > 0.0)
      return;
    EntityManager entityManager = this.mPlayState.EntityManager;
    foreach (Entity entity in entityManager.Entities)
    {
      IDamageable t = entity as IDamageable;
      Shield oShield = (Shield) null;
      if (!(t == null | entity is MissileEntity) && (double) t.ResistanceAgainst(Elements.Water) != 1.0 && !entityManager.IsProtectedByShield(entity, out oShield))
      {
        int num = (int) t.Damage(this.mDamage, this.mCaster as Entity, this.mTimeStamp, new Vector3());
      }
    }
    this.mWetTimer = 0.25f;
  }

  public void OnRemove()
  {
    if (this.mAmbience != null && !this.mAmbience.IsStopping)
      this.mAmbience.Stop(AudioStopOptions.AsAuthored);
    EffectManager.Instance.Stop(ref this.mEffect);
    this.mScene.LightTargetIntensity = 1f;
  }
}
