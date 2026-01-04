// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Barrier
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
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

#nullable disable
namespace Magicka.GameLogic.Entities;

public class Barrier : Entity, IDamageable
{
  private const float ELEMENTALRADIUS = 0.75f;
  private const float RADIUS = 1.25f;
  public const float LENGTH = 0.1f;
  protected static List<Barrier> mCache;
  protected static readonly int[] BARRIER_EFFECTS;
  public static readonly int EXPLOSION_EFFECT = "mine_explosion".GetHashCodeCustom();
  protected static readonly int[] BARRIER_GEYSER_EFFECTS = new int[11]
  {
    0,
    "barrier_geyser_water".GetHashCodeCustom(),
    "barrier_geyser_cold".GetHashCodeCustom(),
    "barrier_geyser_fire".GetHashCodeCustom(),
    "barrier_ice_tesla".GetHashCodeCustom(),
    0,
    0,
    0,
    0,
    "barrier_geyser_steam".GetHashCodeCustom(),
    "barrier_geyser_poison".GetHashCodeCustom()
  };
  public static readonly int[] BARRIER_SOUND_HASH = new int[10]
  {
    "spell_earth_barrier".GetHashCodeCustom(),
    "spell_water_barrier".GetHashCodeCustom(),
    "spell_cold_barrier".GetHashCodeCustom(),
    "spell_fire_barrier".GetHashCodeCustom(),
    "spell_lightning_barrier".GetHashCodeCustom(),
    "spell_arcane_barrier".GetHashCodeCustom(),
    "spell_life_barrier".GetHashCodeCustom(),
    "spell_shield_barrier".GetHashCodeCustom(),
    "spell_ice_barrier".GetHashCodeCustom(),
    "spell_steam_barrier".GetHashCodeCustom()
  };
  public static readonly int Earth_Barrier_Spawn_Effect_Hash = "barrier_earth_spawn".GetHashCodeCustom();
  public static readonly int Earth_Barrier_Death_Effect_Hash = "barrier_earth_death".GetHashCodeCustom();
  public static readonly int Ice_Barrier_Spawn_Effect_Hash = "barrier_ice_spawn".GetHashCodeCustom();
  public static readonly int Ice_Barrier_Death_Effect_Hash = "barrier_ice_death".GetHashCodeCustom();
  public static readonly int Earth_Barrier_Sound_Hash = "spell_earth_barrier".GetHashCodeCustom();
  public static readonly int Earth_Barrier_Break_Sound_Hash = "spell_earth_barrier_break".GetHashCodeCustom();
  public static readonly int Ice_Barrier_Sound_Hash = "spell_ice_barrier".GetHashCodeCustom();
  public static readonly int Ice_Barrier_Break_Sound_Hash = "spell_ice_barrier_break".GetHashCodeCustom();
  public static readonly int Ice_Earth_Barrier_Sound_Hash = "spell_ice_earth_barrier".GetHashCodeCustom();
  public static readonly int Ice_Earth_Barrier_Break_Sound_Hash = "spell_ice_earth_barrier_break".GetHashCodeCustom();
  public static readonly int ICE_HIT_EFFECT = "barrier_ice_deflect".GetHashCodeCustom();
  public static readonly int EARTH_HIT_EFFECT = "barrier_earth_deflect".GetHashCodeCustom();
  protected static SkinnedModel[] sIceBarrierModels;
  protected static AnimationClip[] sIceAppearClips;
  protected static SkinnedModel[] sIceBarrierGeyserModels;
  protected static AnimationClip[] sIceGeyserAppearClips;
  protected static SkinnedModel[] sIceBarrierTeslaModels;
  protected static AnimationClip[] sIceTeslaAppearClips;
  protected static SkinnedModel[] sIceBarrierEarthModels;
  protected static AnimationClip[] sIceEarthAppearClips;
  protected static SkinnedModel[] sEarthBarrierModels;
  protected static AnimationClip[] sEarthAppearClips;
  protected static SkinnedModel[] sEarthVulcanoBarrierModels;
  protected static AnimationClip[] sEarthVulcanoAppearClips;
  protected static Model sRunesArcane;
  protected static Model sRunesLife;
  private static BindJoint sGeyserAttach;
  private static BindJoint sTeslaAttach;
  protected static BindJoint sVulcanoAttach;
  protected BindJoint mEffectAttach;
  protected bool mAddedEffect;
  private Model mRuneModel;
  protected Barrier.HitListWithBarriers mHitList;
  private Barrier.RenderData[] mIceRenderData;
  protected Barrier.RenderData[] mEarthRenderData;
  private RenderableAdditiveObject<AdditiveEffect, AdditiveMaterial>[] mRuneRenderData;
  protected Barrier.DrawMethod mDrawMethod;
  protected Spell mSpell;
  protected DamageCollection5 mDamage;
  protected int mArmour;
  protected float mScale;
  protected float mHitPoints;
  protected float mMaxHitPoints;
  protected float mDamageTimer;
  protected float mNextBarrierTTL;
  protected Vector3 mNextBarrierDir;
  protected Quaternion mNextBarrierRotation;
  protected float mNextBarrierRange;
  protected float mDistanceBetweenBarriers;
  protected float mNormalizedDamage;
  protected float mNormalizedDamageTarget;
  protected bool mDamageSelf;
  protected Barrier.BarrierType mBarrierType;
  protected static Random mRandom = new Random();
  private AnimationController mIceAnimationController;
  protected AnimationController mEarthAnimationController;
  protected float mVolume;
  protected Magicka.GameLogic.Entities.Resistance[] mResistances;
  protected Vector3 mDirection;
  protected ISpellCaster mOwner;
  protected Cue mSoundCue;
  protected double mTimeStamp;
  protected float mEffectTTL;
  private float mIceBarrierTTL;
  private float mIceEarthTTL;
  private bool mDrawIce;
  private bool mKillIce;
  protected float mRuneRotation;
  protected bool mInitilizeDamage;
  protected float mRestingMovementTimer = 1f;
  protected VisualEffectReference mEffect;
  private VisualEffectReference mSpawnDeathEffectReference;
  protected StatusEffect[] mStatusEffects = new StatusEffect[9];
  protected StatusEffects mCurrentStatusEffects;

  public static void InitializeCache(int iNrOfBarriers, PlayState iPlayState)
  {
    Barrier.mCache = new List<Barrier>(iNrOfBarriers);
    for (int index = 0; index < iNrOfBarriers; ++index)
      Barrier.mCache.Add(new Barrier(iPlayState));
  }

  public static Barrier GetFromCache(PlayState iPlayState)
  {
    if (Barrier.mCache.Count <= 0)
      return new Barrier(iPlayState);
    Barrier fromCache = Barrier.mCache[Barrier.mCache.Count - 1];
    Barrier.mCache.RemoveAt(Barrier.mCache.Count - 1);
    return fromCache;
  }

  public static void ReturnToCache(Barrier iBarrier)
  {
    if (Barrier.mCache.Contains(iBarrier))
      return;
    Barrier.mCache.Add(iBarrier);
  }

  static Barrier()
  {
    Barrier.BARRIER_EFFECTS = new int[10];
    for (int iIndex = 0; iIndex < Barrier.BARRIER_EFFECTS.Length; ++iIndex)
    {
      switch (Spell.ElementFromIndex(iIndex))
      {
        case Elements.Earth:
          Barrier.BARRIER_EFFECTS[iIndex] = "barrier_earth1_b".GetHashCodeCustom();
          break;
        case Elements.Water:
          Barrier.BARRIER_EFFECTS[iIndex] = "barrier_water1_b".GetHashCodeCustom();
          break;
        case Elements.Cold:
          Barrier.BARRIER_EFFECTS[iIndex] = "barrier_cold1_b".GetHashCodeCustom();
          break;
        case Elements.Fire:
          Barrier.BARRIER_EFFECTS[iIndex] = "barrier_fire1_b".GetHashCodeCustom();
          break;
        case Elements.Lightning:
          Barrier.BARRIER_EFFECTS[iIndex] = "barrier_lightning1_b".GetHashCodeCustom();
          break;
        case Elements.Arcane:
          Barrier.BARRIER_EFFECTS[iIndex] = "barrier_arcane1_b".GetHashCodeCustom();
          break;
        case Elements.Life:
          Barrier.BARRIER_EFFECTS[iIndex] = "barrier_life1_b".GetHashCodeCustom();
          break;
        case Elements.Shield:
          Barrier.BARRIER_EFFECTS[iIndex] = 0;
          break;
        case Elements.Ice:
          Barrier.BARRIER_EFFECTS[iIndex] = "barrier_ice1_b".GetHashCodeCustom();
          break;
        case Elements.Steam:
          Barrier.BARRIER_EFFECTS[iIndex] = "barrier_steam1_b".GetHashCodeCustom();
          break;
      }
    }
  }

  public static float GetRadius(bool iPhysical) => iPhysical ? 1.25f : 0.75f;

  protected Barrier(PlayState iPlayState)
    : base(iPlayState)
  {
    if (Barrier.sIceBarrierModels == null)
    {
      Barrier.sIceBarrierModels = new SkinnedModel[1];
      SkinnedModel skinnedModel1;
      lock (Magicka.Game.Instance.GraphicsDevice)
      {
        Barrier.sIceBarrierModels[0] = Magicka.Game.Instance.Content.Load<SkinnedModel>("Models/Effects/iceBarrier0_mesh");
        skinnedModel1 = Magicka.Game.Instance.Content.Load<SkinnedModel>("Models/Effects/iceBarrier_animation");
      }
      Barrier.sIceAppearClips = new AnimationClip[skinnedModel1.AnimationClips.Count];
      int num1 = 0;
      foreach (AnimationClip animationClip in skinnedModel1.AnimationClips.Values)
        Barrier.sIceAppearClips[num1++] = animationClip;
      Barrier.sIceBarrierGeyserModels = new SkinnedModel[1];
      SkinnedModel skinnedModel2;
      lock (Magicka.Game.Instance.GraphicsDevice)
      {
        Barrier.sIceBarrierGeyserModels[0] = Magicka.Game.Instance.Content.Load<SkinnedModel>("Models/Effects/icebarrier_geyser");
        foreach (SkinnedModelBone skeletonBone in (ReadOnlyCollection<SkinnedModelBone>) Barrier.sIceBarrierGeyserModels[0].SkeletonBones)
        {
          if (skeletonBone.Name.Equals("effect0", StringComparison.OrdinalIgnoreCase))
          {
            Barrier.sGeyserAttach.mIndex = (int) skeletonBone.Index;
            Barrier.sGeyserAttach.mBindPose = Matrix.CreateRotationY(3.14159274f) * Matrix.Invert(skeletonBone.InverseBindPoseTransform);
          }
        }
        skinnedModel2 = Magicka.Game.Instance.Content.Load<SkinnedModel>("Models/Effects/iceBarrier_geyser");
      }
      Barrier.sIceGeyserAppearClips = new AnimationClip[skinnedModel2.AnimationClips.Count];
      int num2 = 0;
      foreach (AnimationClip animationClip in skinnedModel2.AnimationClips.Values)
        Barrier.sIceGeyserAppearClips[num2++] = animationClip;
      Barrier.sIceBarrierTeslaModels = new SkinnedModel[1];
      SkinnedModel skinnedModel3;
      lock (Magicka.Game.Instance.GraphicsDevice)
      {
        Barrier.sIceBarrierTeslaModels[0] = Magicka.Game.Instance.Content.Load<SkinnedModel>("Models/Effects/icebarrier_tesla");
        foreach (SkinnedModelBone skeletonBone in (ReadOnlyCollection<SkinnedModelBone>) Barrier.sIceBarrierTeslaModels[0].SkeletonBones)
        {
          if (skeletonBone.Name.Equals("effect0", StringComparison.OrdinalIgnoreCase))
          {
            Barrier.sTeslaAttach.mIndex = (int) skeletonBone.Index;
            Barrier.sTeslaAttach.mBindPose = Matrix.CreateRotationY(3.14159274f) * Matrix.Invert(skeletonBone.InverseBindPoseTransform);
          }
        }
        skinnedModel3 = Magicka.Game.Instance.Content.Load<SkinnedModel>("Models/Effects/iceBarrier_tesla");
      }
      Barrier.sIceTeslaAppearClips = new AnimationClip[skinnedModel3.AnimationClips.Count];
      int num3 = 0;
      foreach (AnimationClip animationClip in skinnedModel3.AnimationClips.Values)
        Barrier.sIceTeslaAppearClips[num3++] = animationClip;
      Barrier.sIceBarrierEarthModels = new SkinnedModel[1];
      SkinnedModel skinnedModel4;
      lock (Magicka.Game.Instance.GraphicsDevice)
      {
        Barrier.sIceBarrierEarthModels[0] = Magicka.Game.Instance.Content.Load<SkinnedModel>("Models/Effects/iceBarrier_Earth");
        skinnedModel4 = Magicka.Game.Instance.Content.Load<SkinnedModel>("Models/Effects/iceBarrier_Earth");
      }
      Barrier.sIceEarthAppearClips = new AnimationClip[skinnedModel4.AnimationClips.Count];
      int num4 = 0;
      foreach (AnimationClip animationClip in skinnedModel4.AnimationClips.Values)
        Barrier.sIceEarthAppearClips[num4++] = animationClip;
      SkinnedModel skinnedModel5;
      lock (Magicka.Game.Instance.GraphicsDevice)
      {
        Barrier.sEarthBarrierModels = new SkinnedModel[5];
        Barrier.sEarthBarrierModels[0] = Magicka.Game.Instance.Content.Load<SkinnedModel>("Models/Effects/earthBarrier0_mesh");
        Barrier.sEarthBarrierModels[1] = Magicka.Game.Instance.Content.Load<SkinnedModel>("Models/Effects/earthBarrier1_mesh");
        Barrier.sEarthBarrierModels[2] = Magicka.Game.Instance.Content.Load<SkinnedModel>("Models/Effects/earthBarrier2_mesh");
        Barrier.sEarthBarrierModels[3] = Magicka.Game.Instance.Content.Load<SkinnedModel>("Models/Effects/earthBarrier3_mesh");
        Barrier.sEarthBarrierModels[4] = Magicka.Game.Instance.Content.Load<SkinnedModel>("Models/Effects/earthBarrier4_mesh");
        skinnedModel5 = Magicka.Game.Instance.Content.Load<SkinnedModel>("Models/Effects/earthBarrier_animation");
      }
      Barrier.sEarthAppearClips = new AnimationClip[skinnedModel5.AnimationClips.Count];
      int num5 = 0;
      foreach (AnimationClip animationClip in skinnedModel5.AnimationClips.Values)
        Barrier.sEarthAppearClips[num5++] = animationClip;
      SkinnedModel skinnedModel6;
      lock (Magicka.Game.Instance.GraphicsDevice)
      {
        Barrier.sEarthVulcanoBarrierModels = new SkinnedModel[1];
        Barrier.sEarthVulcanoBarrierModels[0] = Magicka.Game.Instance.Content.Load<SkinnedModel>("Models/Effects/earthBarrier_volcano");
        foreach (SkinnedModelBone skeletonBone in (ReadOnlyCollection<SkinnedModelBone>) Barrier.sEarthVulcanoBarrierModels[0].SkeletonBones)
        {
          if (skeletonBone.Name.Equals("effect0", StringComparison.OrdinalIgnoreCase))
          {
            Barrier.sVulcanoAttach.mIndex = (int) skeletonBone.Index;
            Barrier.sVulcanoAttach.mBindPose = Matrix.CreateRotationY(3.14159274f) * Matrix.Invert(skeletonBone.InverseBindPoseTransform);
          }
        }
        skinnedModel6 = Magicka.Game.Instance.Content.Load<SkinnedModel>("Models/Effects/earthBarrier_volcano");
      }
      Barrier.sEarthVulcanoAppearClips = new AnimationClip[skinnedModel6.AnimationClips.Count];
      int num6 = 0;
      foreach (AnimationClip animationClip in skinnedModel6.AnimationClips.Values)
        Barrier.sEarthVulcanoAppearClips[num6++] = animationClip;
      lock (Magicka.Game.Instance.GraphicsDevice)
        Barrier.sRunesArcane = Magicka.Game.Instance.Content.Load<Model>("Models/Effects/runesArcane");
      lock (Magicka.Game.Instance.GraphicsDevice)
        Barrier.sRunesLife = Magicka.Game.Instance.Content.Load<Model>("Models/Effects/runesLife");
    }
    this.mIceAnimationController = new AnimationController();
    this.mIceAnimationController.ClipSpeed = 2f;
    this.mEarthAnimationController = new AnimationController();
    this.mEarthAnimationController.ClipSpeed = 2f;
    this.mBody = new Body();
    this.mCollision = new CollisionSkin(this.mBody);
    this.mCollision.AddPrimitive((Primitive) new Capsule(Vector3.Down, Matrix.CreateRotationX(1.57079637f), Barrier.GetRadius(true), 0.1f), 1, new MaterialProperties(0.2f, 0.8f, 0.8f));
    this.mCollision.AddPrimitive((Primitive) new Sphere(new Vector3(), Barrier.GetRadius(true)), 1, new MaterialProperties(0.2f, 0.8f, 0.8f));
    this.mCollision.callbackFn += new CollisionCallbackFn(this.OnCollision);
    this.mCollision.postCollisionCallbackFn += new PostCollisionCallbackFn(this.PostCollision);
    this.mBody.CollisionSkin = this.mCollision;
    this.mBody.ApplyGravity = false;
    this.mBody.AllowFreezing = false;
    this.mBody.Tag = (object) this;
    this.mInitilizeDamage = true;
    this.mResistances = new Magicka.GameLogic.Entities.Resistance[Defines.DamageTypeIndex(AttackProperties.NumberOfTypes)];
    for (int iIndex = 0; iIndex < this.mResistances.Length; ++iIndex)
    {
      this.mResistances[iIndex].Multiplier = 1f;
      this.mResistances[iIndex].Modifier = 0.0f;
      this.mResistances[iIndex].ResistanceAgainst = Defines.ElementFromIndex(iIndex);
    }
    this.mIceRenderData = new Barrier.RenderData[3];
    this.mEarthRenderData = new Barrier.RenderData[3];
    this.mRuneRenderData = new RenderableAdditiveObject<AdditiveEffect, AdditiveMaterial>[3];
    for (int index = 0; index < 3; ++index)
    {
      this.mIceRenderData[index] = new Barrier.RenderData();
      this.mEarthRenderData[index] = new Barrier.RenderData();
      this.mRuneRenderData[index] = new RenderableAdditiveObject<AdditiveEffect, AdditiveMaterial>();
    }
  }

  protected virtual bool OnCollision(
    CollisionSkin iSkin0,
    int iPrim0,
    CollisionSkin iSkin1,
    int iPrim1)
  {
    if (iSkin1.Owner == null || iSkin1.Owner.Tag is Barrier)
      return false;
    if (iSkin1.Owner.Tag is IDamageable tag && !tag.Dead && (!(tag is Character character) || !character.IsEthereal) && (double) this.mEffectTTL > 0.0 | !this.Solid && !this.mHitList.HitList.Contains(tag))
    {
      if ((this.mSpell.Element & Elements.Lightning) != Elements.None)
      {
        Vector3 result = this.mEffectAttach.mBindPose.Translation;
        if (!this.Solid)
        {
          result = this.Position;
          ++result.Y;
        }
        else
          Vector3.Transform(ref result, ref this.mIceAnimationController.SkinnedBoneTransforms[this.mEffectAttach.mIndex], out result);
        LightningBolt.GetLightning().Cast(this.mOwner, result, tag as Entity, this.mHitList.HitList, Vector3.One, 1f, 1f, ref this.mDamage, this.mPlayState);
        if (!this.mHitList.HitList.ContainsKey(tag.Handle))
          this.mHitList.HitList.Add(tag.Handle, 0.25f);
      }
      else
      {
        int num = (int) tag.Damage(this.mDamage, this.Owner as Entity, this.mTimeStamp, this.Position);
        this.mHitList.HitList.Add(tag.Handle, 0.25f);
      }
    }
    return this.mBarrierType == Barrier.BarrierType.SOLID & iPrim0 == 0;
  }

  private void PostCollision(ref CollisionInfo iInfo)
  {
    if (iInfo.SkinInfo.Skin0 == this.mCollision)
      iInfo.SkinInfo.IgnoreSkin0 = true;
    else
      iInfo.SkinInfo.IgnoreSkin1 = true;
  }

  public override Matrix GetOrientation()
  {
    Vector3 result1 = this.mBody.Position;
    Matrix orientation = this.mBody.Orientation;
    Vector3 result2 = orientation.Down;
    Vector3.Multiply(ref result2, 0.05f + Barrier.GetRadius(this.Solid), out result2);
    Vector3.Add(ref result1, ref result2, out result1);
    orientation.Translation = result1;
    return orientation;
  }

  public void Initialize(
    ISpellCaster iOwner,
    Vector3 iPosition,
    Vector3 iDirection,
    float iScale,
    float iRange,
    Vector3 iNextDirection,
    Quaternion iNextRotation,
    float iDistanceBetweenBarriers,
    ref Spell iSpell,
    ref DamageCollection5 iDamage,
    Barrier.HitListWithBarriers iHitList,
    AnimatedLevelPart iAnimation)
  {
    if (this.mSoundCue != null && !this.mSoundCue.IsStopping)
      this.mSoundCue.Stop(AudioStopOptions.AsAuthored);
    this.mEarthAnimationController.Stop();
    this.mIceAnimationController.Stop();
    EffectManager.Instance.Stop(ref this.mEffect);
    if ((this.mDrawMethod & Barrier.DrawMethod.PARTICLEWALL) == Barrier.DrawMethod.PARTICLEWALL)
      this.mDrawMethod &= ~Barrier.DrawMethod.PARTICLEWALL;
    if (this.mHitList != null)
    {
      this.mHitList.Owners.Remove(this);
      if (this.mHitList.Owners.Count == 0)
        this.mHitList.Destroy();
    }
    this.mHitList = (Barrier.HitListWithBarriers) null;
    iAnimation?.AddEntity((Entity) this);
    this.mSpell = iSpell;
    this.mHitList = iHitList;
    this.mHitList.Owners.Add(this);
    this.mDirection = iDirection;
    this.mIceAnimationController.PlaybackMode = PlaybackMode.Forward;
    this.mEarthAnimationController.PlaybackMode = PlaybackMode.Forward;
    this.mNextBarrierTTL = 0.075f;
    Vector3.Transform(ref iNextDirection, ref iNextRotation, out this.mNextBarrierDir);
    this.mNextBarrierRotation = iNextRotation;
    this.mNextBarrierRange = iRange - iDistanceBetweenBarriers;
    this.mDistanceBetweenBarriers = iDistanceBetweenBarriers;
    this.mScale = iScale;
    this.mRuneRotation = 0.0f;
    this.mNormalizedDamage = 0.0f;
    this.mNormalizedDamageTarget = 0.0f;
    this.mOwner = iOwner;
    this.mTimeStamp = iOwner == null ? this.PlayState.PlayTime : iOwner.PlayState.PlayTime;
    this.mDamageTimer = 0.0f;
    this.mDrawMethod = Barrier.DrawMethod.NONE;
    this.mDamageSelf = true;
    this.mBarrierType = (this.mSpell.Element & ~Elements.Shield & Elements.PhysicalElements) == Elements.None ? Barrier.BarrierType.ELEMENTAL : Barrier.BarrierType.SOLID;
    DamageCollection5 oDamages = new DamageCollection5();
    if (this.mBarrierType == Barrier.BarrierType.SOLID)
    {
      this.mRadius = Barrier.GetRadius(true) * this.mScale;
      if ((iSpell.Element & Elements.PhysicalElements) == Elements.PhysicalElements)
        this.mSoundCue = AudioManager.Instance.GetCue(Banks.Spells, Barrier.Ice_Earth_Barrier_Sound_Hash);
      else if ((iSpell.Element & Elements.Ice) == Elements.Ice)
        this.mSoundCue = AudioManager.Instance.GetCue(Banks.Spells, Barrier.Ice_Barrier_Sound_Hash);
      else if ((iSpell.Element & Elements.Earth) == Elements.Earth)
        this.mSoundCue = AudioManager.Instance.GetCue(Banks.Spells, Barrier.Earth_Barrier_Sound_Hash);
      this.mAddedEffect = false;
      this.mRuneModel = (iSpell.Element & Elements.Life) != Elements.Life ? ((iSpell.Element & Elements.Arcane) != Elements.Arcane ? (Model) null : Barrier.sRunesArcane) : Barrier.sRunesLife;
      if (this.mRuneModel != null)
      {
        Spell spell = iSpell with
        {
          EarthMagnitude = 0.0f,
          IceMagnitude = 0.0f,
          ShieldMagnitude = 0.0f
        };
        spell.Element &= ~(Elements.PhysicalElements | Elements.Shield);
        Vector3 color = spell.GetColor();
        Vector4 vector4 = new Vector4();
        vector4.X = color.X;
        vector4.Y = color.Y;
        vector4.Z = color.Z;
        vector4.W = 1f;
        for (int index = 0; index < this.mRuneRenderData.Length; ++index)
        {
          this.mRuneRenderData[index].SetMesh(this.mRuneModel.Meshes[0], this.mRuneModel.Meshes[0].MeshParts[0], 0);
          this.mRuneRenderData[index].mMaterial.ColorTint = vector4;
        }
      }
      this.mSpell.CalculateDamage(SpellType.Projectile, CastType.Area, out oDamages);
      if ((iSpell.Element & Elements.Ice) == Elements.Ice)
      {
        this.mDrawIce = true;
        this.mKillIce = false;
        Vector3 forward = this.Body.Orientation.Forward;
        EffectManager.Instance.StartEffect(Barrier.Ice_Barrier_Spawn_Effect_Hash, ref iPosition, ref forward, out this.mSpawnDeathEffectReference);
        SkinnedModel skinnedModel = Barrier.sIceBarrierModels[Barrier.mRandom.Next(Barrier.sIceBarrierModels.Length)];
        AnimationClip[] animationClipArray = Barrier.sIceAppearClips;
        this.mEffectTTL = iSpell.IceMagnitude * 1f;
        if ((iSpell.Element & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Lightning | Elements.Steam | Elements.Poison)) != Elements.None && (iSpell.Element & Elements.Beams) == Elements.None)
        {
          this.mAddedEffect = true;
          skinnedModel = Barrier.sIceBarrierGeyserModels[Barrier.mRandom.Next(Barrier.sIceBarrierGeyserModels.Length)];
          animationClipArray = Barrier.sIceGeyserAppearClips;
          this.mEffectAttach = Barrier.sGeyserAttach;
          if ((iSpell.Element & Elements.Lightning) == Elements.Lightning)
          {
            this.mEffectAttach = Barrier.sTeslaAttach;
            skinnedModel = Barrier.sIceBarrierTeslaModels[Barrier.mRandom.Next(Barrier.sIceBarrierTeslaModels.Length)];
            animationClipArray = Barrier.sIceTeslaAppearClips;
            this.mEffectTTL = iSpell.LightningMagnitude * 1f;
          }
        }
        if ((iSpell.Element & Elements.Earth) != Elements.None)
        {
          skinnedModel = Barrier.sIceBarrierEarthModels[Barrier.mRandom.Next(Barrier.sIceBarrierEarthModels.Length)];
          animationClipArray = Barrier.sIceEarthAppearClips;
          this.mAddedEffect = false;
          this.mIceEarthTTL = iSpell.IceMagnitude * 1f;
        }
        this.mIceBarrierTTL = this.mEffectTTL;
        ModelMesh mesh = skinnedModel.Model.Meshes[0];
        ModelMeshPart meshPart = mesh.MeshParts[0];
        this.mIceAnimationController.Skeleton = skinnedModel.SkeletonBones;
        if (this.mAddedEffect)
        {
          Elements iElement1 = Elements.None;
          float num = -1f;
          for (int iIndex = 0; iIndex < 11; ++iIndex)
          {
            Elements iElement2 = Defines.ElementFromIndex(iIndex);
            if ((iElement2 & Elements.Lightning) == Elements.Lightning && (this.mSpell.Element & Elements.Lightning) == Elements.Lightning)
            {
              iElement1 = iElement2;
              num = this.mSpell[iElement1];
              iIndex = 11;
            }
            if ((iElement2 & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Lightning | Elements.Steam)) != Elements.None && (double) this.mSpell[iElement2] > (double) num)
            {
              iElement1 = iElement2;
              num = this.mSpell[iElement1];
            }
          }
          Matrix identity = Matrix.Identity with
          {
            Translation = iPosition
          };
          EffectManager.Instance.StartEffect(Barrier.BARRIER_GEYSER_EFFECTS[Defines.ElementIndex(iElement1)], ref identity, out this.mEffect);
          if ((iElement1 & Elements.Cold) == Elements.Cold)
            this.mEffectTTL = this.mSpell.ColdMagnitude * 1f;
          if ((iElement1 & Elements.Fire) == Elements.Fire)
            this.mEffectTTL = this.mSpell.FireMagnitude * 1f;
          if ((iElement1 & Elements.Poison) == Elements.Poison)
            this.mEffectTTL = this.mSpell.PoisonMagnitude * 1f;
          if ((iElement1 & Elements.Steam) == Elements.Steam)
            this.mEffectTTL = this.mSpell.SteamMagnitude * 1f;
          if ((iElement1 & Elements.Water) == Elements.Water)
            this.mEffectTTL = this.mSpell.WaterMagnitude * 1f;
        }
        float num1 = MathHelper.Clamp((float) (((double) iSpell.IceMagnitude - 1.0) * 0.33333298563957214), 0.0f, 1f);
        for (int index = 0; index < 3; ++index)
        {
          Barrier.RenderData renderData = this.mIceRenderData[index];
          renderData.SetMesh(mesh.VertexBuffer, mesh.IndexBuffer, meshPart);
          renderData.mSkinnedModelMaterial.OverlayAlpha = num1;
        }
        this.mIceAnimationController.StartClip(animationClipArray[Barrier.mRandom.Next(animationClipArray.Length)], false);
        this.mDrawMethod |= Barrier.DrawMethod.NORMAL;
      }
      if ((iSpell.Element & Elements.Earth) == Elements.Earth)
      {
        this.mSpell.CalculateDamage(SpellType.Projectile, CastType.Area, out oDamages);
        Vector3 forward = this.Body.Orientation.Forward;
        EffectManager.Instance.StartEffect(Barrier.Earth_Barrier_Spawn_Effect_Hash, ref iPosition, ref forward, out this.mSpawnDeathEffectReference);
        SkinnedModel skinnedModel = Barrier.sEarthBarrierModels[Barrier.mRandom.Next(Barrier.sIceBarrierModels.Length)];
        AnimationClip[] animationClipArray = Barrier.sEarthAppearClips;
        if ((iSpell.Element & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Steam | Elements.Poison)) != Elements.None && (iSpell.Element & Elements.Beams) == Elements.None)
        {
          this.mAddedEffect = true;
          this.mEffectAttach = Barrier.sVulcanoAttach;
          skinnedModel = Barrier.sEarthVulcanoBarrierModels[Barrier.mRandom.Next(Barrier.sEarthVulcanoBarrierModels.Length)];
          animationClipArray = Barrier.sEarthVulcanoAppearClips;
          Elements iElement3 = Elements.None;
          float num = -1f;
          for (int iIndex = 0; iIndex < 11; ++iIndex)
          {
            Elements iElement4 = Defines.ElementFromIndex(iIndex);
            if ((iElement4 & Elements.Lightning) == Elements.Lightning && (this.mSpell.Element & Elements.Lightning) == Elements.Lightning)
            {
              iElement3 = iElement4;
              num = this.mSpell[iElement3];
              iIndex = 11;
            }
            else if ((iElement4 & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Lightning | Elements.Steam)) != Elements.None && (double) this.mSpell[iElement4] > (double) num)
            {
              iElement3 = iElement4;
              num = this.mSpell[iElement3];
            }
          }
          if ((iElement3 & Elements.Cold) == Elements.Cold)
            this.mEffectTTL = this.mSpell.ColdMagnitude * 5f;
          if ((iElement3 & Elements.Fire) == Elements.Fire)
            this.mEffectTTL = this.mSpell.FireMagnitude * 5f;
          if ((iElement3 & Elements.Poison) == Elements.Poison)
            this.mEffectTTL = this.mSpell.PoisonMagnitude * 5f;
          if ((iElement3 & Elements.Steam) == Elements.Steam)
            this.mEffectTTL = this.mSpell.SteamMagnitude * 5f;
          if ((iElement3 & Elements.Water) == Elements.Water)
            this.mEffectTTL = this.mSpell.WaterMagnitude * 5f;
          Matrix identity = Matrix.Identity with
          {
            Translation = iPosition
          };
          EffectManager.Instance.StartEffect(Barrier.BARRIER_GEYSER_EFFECTS[Defines.ElementIndex(iElement3)], ref identity, out this.mEffect);
        }
        ModelMesh mesh = skinnedModel.Model.Meshes[0];
        ModelMeshPart meshPart = mesh.MeshParts[0];
        this.mEarthAnimationController.Skeleton = skinnedModel.SkeletonBones;
        float num2 = MathHelper.Clamp((float) (((double) iSpell.EarthMagnitude - 1.0) * 0.33333298563957214), 0.0f, 1f);
        for (int index = 0; index < 3; ++index)
        {
          Barrier.RenderData renderData = this.mEarthRenderData[index];
          renderData.SetMesh(mesh.VertexBuffer, mesh.IndexBuffer, meshPart);
          renderData.mSkinnedModelMaterial.OverlayAlpha = num2;
        }
        this.mEarthAnimationController.StartClip(animationClipArray[Barrier.mRandom.Next(animationClipArray.Length)], false);
        this.mDrawMethod |= Barrier.DrawMethod.NORMAL;
      }
    }
    else
    {
      Elements iElement5 = Elements.None;
      float num = -1f;
      for (int iIndex = 0; iIndex < 11; ++iIndex)
      {
        Elements iElement6 = Defines.ElementFromIndex(iIndex);
        if ((iElement6 & Elements.Steam) == Elements.Steam && (this.mSpell.Element & Elements.Lightning) == Elements.Lightning && (this.mSpell.Element & Elements.Steam) == Elements.Steam)
        {
          iElement5 = iElement6;
          num = this.mSpell[iElement5];
          iIndex = 11;
        }
        if ((iElement6 & Elements.Lightning) == Elements.Lightning && (this.mSpell.Element & Elements.Steam) == Elements.None && (this.mSpell.Element & Elements.Lightning) == Elements.Lightning)
        {
          iElement5 = iElement6;
          num = this.mSpell[iElement5];
          iIndex = 11;
        }
        if ((iElement6 & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Lightning | Elements.Steam)) != Elements.None && (double) this.mSpell[iElement6] > (double) num)
        {
          iElement5 = iElement6;
          num = this.mSpell[iElement5];
        }
      }
      Vector3 up = Vector3.Up;
      Matrix identity = Matrix.Identity with
      {
        Forward = iDirection,
        Up = up
      };
      Vector3 result;
      Vector3.Cross(ref iDirection, ref up, out result);
      identity.Right = result;
      MagickaMath.UniformMatrixScale(ref identity, this.mScale);
      identity.Translation = iPosition;
      this.mDrawMethod |= Barrier.DrawMethod.PARTICLEWALL;
      this.mSoundCue = AudioManager.Instance.GetCue(Banks.Spells, Barrier.BARRIER_SOUND_HASH[Spell.ElementIndex(iElement5)]);
      if (this.mBarrierType == Barrier.BarrierType.ELEMENTAL)
      {
        this.mRadius = Barrier.GetRadius(false) * this.mScale;
        EffectManager.Instance.StartEffect(Barrier.BARRIER_EFFECTS[Defines.ElementIndex(iElement5)], ref identity, out this.mEffect);
      }
    }
    Matrix orientation;
    MagickaMath.MakeOrientationMatrix(ref iDirection, out orientation);
    Vector3 pos = iPosition;
    pos.Y += (float) ((double) Barrier.GetRadius(this.Solid) * (double) this.mScale + 0.05000000074505806);
    Matrix scale = Matrix.CreateScale(this.mScale);
    Matrix result1;
    Matrix.Multiply(ref scale, ref orientation, out result1);
    this.mBody.MoveTo(pos, result1);
    this.mBody.CollisionSkin.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
    (this.mCollision.GetPrimitiveLocal(0) as Capsule).Radius = this.mRadius;
    (this.mCollision.GetPrimitiveNewWorld(0) as Capsule).Radius = this.mRadius;
    (this.mCollision.GetPrimitiveOldWorld(0) as Capsule).Radius = this.mRadius;
    float num3 = this.Solid ? this.mRadius * 2f : this.mRadius;
    (this.mCollision.GetPrimitiveLocal(1) as Sphere).Radius = num3;
    (this.mCollision.GetPrimitiveLocal(1) as Sphere).Position = new Vector3(0.0f, -this.mRadius, 0.0f);
    (this.mCollision.GetPrimitiveNewWorld(1) as Sphere).Radius = num3;
    (this.mCollision.GetPrimitiveNewWorld(1) as Sphere).Position = new Vector3(0.0f, -this.mRadius, 0.0f);
    (this.mCollision.GetPrimitiveOldWorld(1) as Sphere).Radius = num3;
    (this.mCollision.GetPrimitiveOldWorld(1) as Sphere).Position = new Vector3(0.0f, -this.mRadius, 0.0f);
    this.mVolume = (this.mCollision.GetPrimitiveLocal(0) as Capsule).GetVolume();
    List<Entity> entities1 = this.mPlayState.EntityManager.GetEntities(iPosition, (float) ((double) iScale * (double) Barrier.GetRadius(this.Solid) * 0.800000011920929), false);
    for (int index = 0; index < entities1.Count; ++index)
    {
      Barrier barrier = entities1[index] as Barrier;
      SpellMine spellMine = entities1[index] as SpellMine;
      if (barrier != null && this.mHitList != barrier.HitList)
        barrier.Kill();
      else
        spellMine?.Detonate();
    }
    this.mPlayState.EntityManager.ReturnEntityList(entities1);
    if (this.mInitilizeDamage)
    {
      List<Entity> entities2 = this.mPlayState.EntityManager.GetEntities(iPosition, (float) ((double) iScale * (double) Barrier.GetRadius(this.Solid) * 1.25), false);
      for (int index = 0; index < entities2.Count; ++index)
      {
        IDamageable t = entities2[index] as IDamageable;
        if (t != this && t != null && t != this.mOwner && !this.HitList.HitList.ContainsKey(t.Handle) && (!(t is Barrier) || (t as Barrier).HitList != this.HitList))
        {
          int num4 = (int) t.Damage(oDamages, iOwner as Entity, this.mTimeStamp, iPosition);
          this.mHitList.HitList.Add(t.Handle, 0.25f);
        }
      }
      this.mPlayState.EntityManager.ReturnEntityList(entities2);
    }
    this.mDamage = iDamage;
    this.mHitPoints = !this.Solid ? iSpell.TotalMagnitude() * 100f : (float) ((1.0 + (double) iSpell[Elements.Earth]) * 500.0);
    this.mArmour = (int) ((double) iSpell[Elements.Ice] * 50.0);
    this.mMaxHitPoints = this.mHitPoints;
    (this.mBody.CollisionSkin.GetPrimitiveLocal(0) as Capsule).Orientation = Matrix.CreateRotationX(-1.57079637f);
    this.Initialize();
    if (this.mSoundCue == null)
      return;
    this.mSoundCue.Apply3D(this.mPlayState.Camera.Listener, this.AudioEmitter);
    this.mSoundCue.Play();
  }

  public override void Deinitialize()
  {
    if (this.mSoundCue != null && !this.mSoundCue.IsStopping)
      this.mSoundCue.Stop(AudioStopOptions.AsAuthored);
    this.mEarthAnimationController.Stop();
    this.mIceAnimationController.Stop();
    EffectManager.Instance.Stop(ref this.mEffect);
    if ((this.mDrawMethod & Barrier.DrawMethod.PARTICLEWALL) == Barrier.DrawMethod.PARTICLEWALL)
      this.mDrawMethod &= ~Barrier.DrawMethod.PARTICLEWALL;
    if (this.mHitList != null)
    {
      this.mHitList.Owners.Remove(this);
      if (this.mHitList.Owners.Count == 0)
        this.mHitList.Destroy();
    }
    this.mHitList = (Barrier.HitListWithBarriers) null;
    base.Deinitialize();
    Barrier.ReturnToCache(this);
  }

  protected bool RestingMovement => (double) this.mRestingMovementTimer < 0.0;

  protected void SpawnNextBarrier()
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    Vector3 vector3 = this.Position + this.mNextBarrierDir;
    Segment iSeg1 = new Segment();
    iSeg1.Delta.Y = -1.5f;
    iSeg1.Origin = vector3;
    iSeg1.Origin.Y += (float) (0.75 - (0.05000000074505806 + (double) this.Radius));
    List<Shield> shields = this.mPlayState.EntityManager.Shields;
    Segment iSeg2 = new Segment();
    iSeg2.Origin = this.Position;
    Vector3.Subtract(ref iSeg1.Origin, ref iSeg2.Origin, out iSeg2.Delta);
    bool flag = false;
    for (int index = 0; index < shields.Count; ++index)
    {
      if (shields[index].SegmentIntersect(out Vector3 _, iSeg2, 1f))
        flag = true;
    }
    Vector3 oPos;
    AnimatedLevelPart oAnimatedLevelPart;
    if (flag || !this.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out Vector3 _, out oAnimatedLevelPart, iSeg1))
      return;
    Vector3 result = this.mBody.Orientation.Forward;
    Vector3.Transform(ref result, ref this.mNextBarrierRotation, out result);
    List<Entity> entities = this.mPlayState.EntityManager.GetEntities(oPos, 1.5f * this.mRadius, false);
    for (int index = 0; index < entities.Count; ++index)
    {
      if (entities[index] is Barrier && (entities[index] as Barrier).HitList != this.mHitList)
        entities[index].Kill();
      else if (entities[index] is SpellMine)
        (entities[index] as SpellMine).Detonate();
    }
    this.mPlayState.EntityManager.ReturnEntityList(entities);
    Barrier fromCache = Barrier.GetFromCache(this.mPlayState);
    if (NetworkManager.Instance.State == NetworkState.Server)
    {
      SpawnBarrierMessage iMessage;
      iMessage.Handle = fromCache.Handle;
      iMessage.OwnerHandle = this.Owner.Handle;
      iMessage.AnimationHandle = oAnimatedLevelPart == null ? ushort.MaxValue : oAnimatedLevelPart.Handle;
      iMessage.Position = oPos;
      iMessage.Direction = result;
      iMessage.Scale = this.mScale;
      iMessage.Spell = this.mSpell;
      iMessage.Damage = this.mDamage;
      iMessage.HitlistHandle = this.mHitList.Handle;
      NetworkManager.Instance.Interface.SendMessage<SpawnBarrierMessage>(ref iMessage);
    }
    fromCache.Initialize(this.Owner, oPos, result, this.mScale, this.mNextBarrierRange, this.mNextBarrierDir, this.mNextBarrierRotation, this.mDistanceBetweenBarriers, ref this.mSpell, ref this.mDamage, this.mHitList, oAnimatedLevelPart);
    this.mPlayState.EntityManager.AddEntity((Entity) fromCache);
  }

  public float ResistanceAgainst(Elements iElement)
  {
    float num1 = 0.0f;
    float num2 = 0.0f;
    for (int iIndex = 0; iIndex < this.mResistances.Length; ++iIndex)
    {
      Elements elements = Defines.ElementFromIndex(iIndex);
      if ((iElement & elements) != Elements.None)
      {
        float multiplier = this.mResistances[iIndex].Multiplier;
        float modifier = this.mResistances[iIndex].Modifier;
        if (this.HasStatus(StatusEffects.Frozen) && (iElement & Elements.Earth) != Elements.None)
          modifier -= 350f;
        if (this.HasStatus(StatusEffects.Greased) && (iElement & Elements.Fire) != Elements.None)
          multiplier *= 2f;
        num1 += modifier;
        num2 += multiplier;
      }
    }
    return 1f - MathHelper.Clamp(num1 / 300f + num2, -1f, 1f);
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    base.Update(iDataChannel, iDeltaTime);
    if (this.mSoundCue != null)
      this.mSoundCue.Apply3D(this.mPlayState.Camera.Listener, this.AudioEmitter);
    this.mEffectTTL -= iDeltaTime;
    this.mIceBarrierTTL -= iDeltaTime;
    this.mIceEarthTTL -= iDeltaTime;
    this.mRuneRotation -= iDeltaTime * 0.25f;
    if ((double) this.mIceBarrierTTL < 0.0 && (this.mSpell.Element & Elements.PhysicalElements) == Elements.Ice)
      this.mHitPoints = 0.0f;
    if ((double) this.mIceEarthTTL < 0.0 && !this.mKillIce && (this.mSpell.Element & Elements.Ice) == Elements.Ice && (this.mSpell.Element & Elements.Earth) == Elements.Earth)
    {
      this.mKillIce = true;
      this.RunIceEarthDeathAnimation();
    }
    if ((double) this.mIceEarthTTL < 0.0 && !this.mIceAnimationController.IsPlaying && this.mKillIce && this.mDrawIce)
      this.mDrawIce = false;
    if ((double) this.mNextBarrierRange > 1.4012984643248171E-45)
    {
      this.mNextBarrierTTL -= iDeltaTime;
      if ((double) this.mNextBarrierTTL < 0.0)
      {
        this.SpawnNextBarrier();
        this.mNextBarrierRange = 0.0f;
      }
    }
    if (this.mHitList.Owners[0] == this)
      this.mHitList.HitList.Update(iDeltaTime);
    if (this.mDamageSelf)
    {
      this.mDamageTimer -= iDeltaTime;
      if ((double) this.mDamageTimer < 0.0)
      {
        this.mHitPoints -= 10f;
        this.mDamageTimer += 0.2f;
        if (this.Dead && this.mBarrierType == Barrier.BarrierType.SOLID)
          this.RunDeathAnimation();
      }
    }
    if ((double) this.mBody.Velocity.LengthSquared() > 9.9999999747524271E-07)
      this.mRestingMovementTimer = 1f;
    else
      this.mRestingMovementTimer -= iDeltaTime;
    if ((this.mDrawMethod & Barrier.DrawMethod.PARTICLEWALL) == Barrier.DrawMethod.PARTICLEWALL)
    {
      Vector3 up = Vector3.Up;
      Matrix identity = Matrix.Identity with
      {
        Forward = this.mDirection,
        Up = up
      };
      Vector3 result;
      Vector3.Cross(ref this.mDirection, ref up, out result);
      identity.Right = result;
      MagickaMath.UniformMatrixScale(ref identity, this.mScale);
      identity.Translation = this.Position;
      EffectManager.Instance.UpdateOrientation(ref this.mEffect, ref identity);
    }
    this.mNormalizedDamageTarget = (float) (1.0 - (double) this.mHitPoints / (double) this.mMaxHitPoints);
    this.mNormalizedDamage = this.mNormalizedDamageTarget + (this.mNormalizedDamage - this.mNormalizedDamageTarget) * (float) Math.Pow(0.15, (double) iDeltaTime);
    if ((this.mDrawMethod & Barrier.DrawMethod.NORMAL) != Barrier.DrawMethod.NONE)
    {
      Matrix orientation = this.GetOrientation();
      if ((this.mSpell.Element & Elements.Ice) != Elements.None)
        this.mIceAnimationController.Update(iDeltaTime, ref orientation, true);
      if ((this.mSpell.Element & Elements.Earth) != Elements.None)
        this.mEarthAnimationController.Update(iDeltaTime, ref orientation, true);
    }
    if (this.mAddedEffect)
    {
      if ((double) this.mEffectTTL > 0.0)
      {
        Matrix result;
        if ((this.mSpell.Element & Elements.Ice) == Elements.Ice)
          Matrix.Multiply(ref this.mEffectAttach.mBindPose, ref this.mIceAnimationController.SkinnedBoneTransforms[this.mEffectAttach.mIndex], out result);
        else
          Matrix.Multiply(ref this.mEffectAttach.mBindPose, ref this.mEarthAnimationController.SkinnedBoneTransforms[this.mEffectAttach.mIndex], out result);
        EffectManager.Instance.UpdateOrientation(ref this.mEffect, ref result);
      }
      else
      {
        EffectManager.Instance.Stop(ref this.mEffect);
        this.mAddedEffect = false;
      }
    }
    if ((this.mDrawMethod & Barrier.DrawMethod.NORMAL) != Barrier.DrawMethod.NORMAL)
      return;
    if ((this.mSpell.Element & Elements.Ice) == Elements.Ice && this.mDrawIce)
    {
      Barrier.RenderData iObject = this.mIceRenderData[(int) iDataChannel];
      iObject.mBoundingSphere.Center = this.Position;
      iObject.mBoundingSphere.Radius = this.mRadius * 3f;
      iObject.mDamage = this.mNormalizedDamage;
      Array.Copy((Array) this.mIceAnimationController.SkinnedBoneTransforms, (Array) iObject.mBones, this.mIceAnimationController.Skeleton.Count);
      this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) iObject);
    }
    if ((this.mSpell.Element & Elements.Earth) == Elements.Earth)
    {
      Barrier.RenderData iObject = this.mEarthRenderData[(int) iDataChannel];
      iObject.mBoundingSphere.Center = this.Position;
      iObject.mBoundingSphere.Radius = this.mRadius * 3f;
      iObject.mDamage = this.mNormalizedDamage;
      Array.Copy((Array) this.mEarthAnimationController.SkinnedBoneTransforms, (Array) iObject.mBones, this.mEarthAnimationController.Skeleton.Count);
      this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) iObject);
    }
    if (this.mRuneModel == null)
      return;
    RenderableAdditiveObject<AdditiveEffect, AdditiveMaterial> iObject1 = this.mRuneRenderData[(int) iDataChannel];
    iObject1.mBoundingSphere.Center = this.Position;
    iObject1.mBoundingSphere.Radius = this.mRadius * 3f;
    Matrix.CreateRotationY(this.mRuneRotation, out iObject1.mMaterial.WorldTransform);
    iObject1.mMaterial.WorldTransform.Translation = this.Position;
    iObject1.mMaterial.WorldTransform.M42 -= (float) ((double) Barrier.GetRadius(this.Solid) * (double) this.mScale + 0.05000000074505806);
    this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, (IRenderableAdditiveObject) iObject1);
  }

  public override Vector3 CalcImpulseVelocity(
    Vector3 iDirection,
    float iElevation,
    float iMassPower,
    float iDistance)
  {
    return new Vector3();
  }

  public override bool Dead => (double) this.mHitPoints <= 0.0;

  public override bool Removable
  {
    get
    {
      return this.Dead && !this.mIceAnimationController.IsPlaying && !this.mEarthAnimationController.IsPlaying;
    }
  }

  public ISpellCaster Owner => this.mOwner;

  public virtual bool HasStatus(StatusEffects iStatus)
  {
    return (this.mCurrentStatusEffects & iStatus) == iStatus;
  }

  public virtual float StatusMagnitude(StatusEffects iStatus)
  {
    int index = StatusEffect.StatusIndex(iStatus);
    return !this.mStatusEffects[index].Dead ? this.mStatusEffects[index].Magnitude : 0.0f;
  }

  public override Vector3 Position => this.mBody.Position;

  public float HitPoints => this.mHitPoints;

  public float MaxHitPoints => this.mMaxHitPoints;

  public bool Solid => this.mBarrierType == Barrier.BarrierType.SOLID;

  public Barrier.HitListWithBarriers HitList => this.mHitList;

  public void GetRandomPositionOnCollisionSkin(out Vector3 oPosition)
  {
    Vector3 position = this.Position;
    if (this.mCollision == null)
      oPosition = position;
    Vector3 vector3 = new Vector3();
    float iAngle1 = MagickaMath.RandomBetween(-3.14159274f, 3.14159274f);
    float iAngle2 = MagickaMath.RandomBetween(-1.57079637f, 1.57079637f);
    float oSin1;
    float oCos1;
    MathApproximation.FastSinCos(iAngle1, out oSin1, out oCos1);
    float oSin2;
    float oCos2;
    MathApproximation.FastSinCos(iAngle2, out oSin2, out oCos2);
    float num1 = oCos1 * oCos2;
    float num2 = oSin1 * oCos2;
    float radius = (this.mCollision.GetPrimitiveLocal(0) as Capsule).Radius;
    float num3 = (this.mCollision.GetPrimitiveLocal(0) as Capsule).Radius * 2f + (this.mCollision.GetPrimitiveLocal(0) as Capsule).Length;
    vector3.X = num1 * radius;
    vector3.Z = num2 * radius;
    vector3.Y = (float) ((double) oSin2 * (double) num3 * 0.5);
    vector3.X += position.X;
    vector3.Y += position.Y;
    vector3.Z += position.Z;
    oPosition = vector3;
  }

  public virtual float Volume => this.mVolume;

  public Magicka.GameLogic.Entities.Resistance[] Resistance => this.mResistances;

  public void SetSlow()
  {
  }

  public void Damage(float iDamage, Elements iElement)
  {
    this.mHitPoints -= iDamage;
    if (!this.Dead || this.mBarrierType != Barrier.BarrierType.SOLID)
      return;
    this.RunDeathAnimation();
  }

  public StatusEffect[] GetStatusEffects() => this.mStatusEffects;

  public virtual DamageResult InternalDamage(
    DamageCollection5 iDamages,
    Entity iAttacker,
    double iTimeStamp,
    Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
    return DamageResult.None | this.InternalDamage(iDamages.A, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.B, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.C, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.D, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.E, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
  }

  public DamageResult InternalDamage(
    Magicka.GameLogic.Damage iDamage,
    Entity iAttacker,
    double iTimeStamp,
    Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
    DamageResult damageResult = DamageResult.None;
    Elements element = iDamage.Element;
    if ((element & Elements.Arcane) == Elements.Arcane)
      element ^= Elements.Arcane;
    switch (this.mBarrierType)
    {
      case Barrier.BarrierType.SOLID:
        if ((double) iDamage.Amount > 0.0)
        {
          if ((this.mSpell.Element & Elements.Cold) == Elements.Cold && ((element & Elements.Fire) == Elements.Fire || (element & Elements.Steam) == Elements.Steam))
          {
            this.mEffectTTL = 0.0f;
            break;
          }
          if ((this.mSpell.Element & Elements.Steam) == Elements.Steam && (element & Elements.Cold) == Elements.Cold)
          {
            this.mEffectTTL = 0.0f;
            break;
          }
          if ((this.mSpell.Element & Elements.Water) == Elements.Water && (element & Elements.Fire) == Elements.Fire)
          {
            this.mEffectTTL = 0.0f;
            break;
          }
          if ((this.mSpell.Element & Elements.Fire) == Elements.Fire && (element & Elements.Water) == Elements.Water)
          {
            this.mEffectTTL = 0.0f;
            break;
          }
          iDamage.Amount = (double) this.mSpell[Elements.Ice] <= 0.0 || (element & Elements.Fire) != Elements.Fire ? Math.Max(iDamage.Amount - (float) this.mArmour, 0.0f) : Math.Max(iDamage.Amount, 0.0f);
          if ((double) iDamage.Amount == 0.0)
            damageResult |= DamageResult.Deflected;
          if ((double) iDamage.Amount > 0.0)
          {
            VisualEffectReference oRef;
            if ((iDamage.Element & Elements.Earth) == Elements.Earth && (iDamage.AttackProperty & AttackProperties.Damage) == AttackProperties.Damage)
            {
              if ((double) this.mSpell.IceMagnitude > 0.0)
              {
                Vector3 position = this.Position;
                Vector3 result;
                Vector3.Subtract(ref iAttackPosition, ref position, out result);
                result.Normalize();
                EffectManager.Instance.StartEffect(Barrier.ICE_HIT_EFFECT, ref position, ref result, out oRef);
              }
              else
              {
                Vector3 position = this.Position;
                Vector3 result;
                Vector3.Subtract(ref iAttackPosition, ref position, out result);
                result.Normalize();
                EffectManager.Instance.StartEffect(Barrier.EARTH_HIT_EFFECT, ref position, ref result, out oRef);
              }
            }
            damageResult |= DamageResult.Damaged;
          }
        }
        if (Defines.FeatureDamage(iFeatures) && (double) iDamage.Amount * (double) iDamage.Magnitude > 0.0)
        {
          this.mHitPoints -= iDamage.Amount;
          break;
        }
        break;
      case Barrier.BarrierType.ELEMENTAL:
        float num1 = 0.0f;
        float num2 = 0.0f;
        for (int iIndex = 0; iIndex < 11; ++iIndex)
        {
          Elements iB = Defines.ElementFromIndex(iIndex);
          if ((this.mSpell.Element & iB) == iB && (iB & Elements.Shield) == Elements.None)
          {
            ++num2;
            if (SpellManager.InclusiveOpposites(element, iB))
              ++num1;
          }
        }
        if (Defines.FeatureDamage(iFeatures) && (double) iDamage.Amount * (double) iDamage.Magnitude > 0.0)
        {
          this.mHitPoints -= (float) (int) ((double) num1 / (double) num2 * (double) iDamage.Amount * 5.0);
          break;
        }
        break;
    }
    if (this.mBarrierType == Barrier.BarrierType.SOLID)
    {
      if (this.Dead)
        this.RunDeathAnimation();
      if ((double) this.mHitPoints - (double) iDamage.Amount < 0.0)
      {
        damageResult |= DamageResult.Killed;
        this.DeathVisuals();
      }
    }
    return damageResult;
  }

  public void Electrocute(IDamageable iTarget, float iMultiplyer)
  {
    if (this.mHitList.HitList.ContainsKey(iTarget.Handle))
      return;
    this.mHitList.HitList.Add(iTarget.Handle);
    DamageCollection5 mDamage = this.mDamage;
    mDamage.MultiplyMagnitude(iMultiplyer);
    if ((mDamage.A.Element & Elements.Lightning) != Elements.None)
    {
      int num1 = (int) iTarget.Damage(mDamage.A, this.Owner as Entity, this.mTimeStamp, this.Position);
    }
    if ((mDamage.B.Element & Elements.Lightning) != Elements.None)
    {
      int num2 = (int) iTarget.Damage(mDamage.B, this.Owner as Entity, this.mTimeStamp, this.Position);
    }
    if ((mDamage.C.Element & Elements.Lightning) != Elements.None)
    {
      int num3 = (int) iTarget.Damage(mDamage.C, this.Owner as Entity, this.mTimeStamp, this.Position);
    }
    if ((mDamage.D.Element & Elements.Lightning) != Elements.None)
    {
      int num4 = (int) iTarget.Damage(mDamage.D, this.Owner as Entity, this.mTimeStamp, this.Position);
    }
    if ((mDamage.E.Element & Elements.Lightning) == Elements.None)
      return;
    int num5 = (int) iTarget.Damage(mDamage.E, this.Owner as Entity, this.mTimeStamp, this.Position);
  }

  public override void Kill()
  {
    this.mHitPoints = 0.0f;
    if (this.mBarrierType != Barrier.BarrierType.SOLID)
      return;
    this.RunDeathAnimation();
  }

  public void Detonate()
  {
    this.mIceAnimationController.Stop();
    this.mEarthAnimationController.Stop();
    Vector3 position = this.Position;
    Vector3 forward = this.mBody.Orientation.Forward;
    int num = (int) Blast.FullBlast(this.mPlayState, this.mOwner as Entity, this.mTimeStamp, (Entity) this, 5f, position, this.mDamage);
    EffectManager.Instance.StartEffect(Barrier.EXPLOSION_EFFECT, ref position, ref forward, out VisualEffectReference _);
    Elements elements1 = this.mSpell.Element & ~Elements.Shield;
    for (int iIndex = 0; iIndex < 11; ++iIndex)
    {
      Elements elements2 = Defines.ElementFromIndex(iIndex);
      if ((elements2 & elements1) == elements2)
        AudioManager.Instance.PlayCue(Banks.Spells, Defines.SOUNDS_AREA[iIndex], this.AudioEmitter);
    }
    this.mDead = true;
  }

  private void DeathVisuals()
  {
    if ((this.mSpell.Element & Elements.Ice) == Elements.Ice && (this.mSpell.Element & Elements.Earth) == Elements.Earth && !this.mKillIce)
      AudioManager.Instance.PlayCue(Banks.Spells, Barrier.Ice_Earth_Barrier_Break_Sound_Hash, this.AudioEmitter);
    else if ((this.mSpell.Element & Elements.Ice) == Elements.Ice && !this.mKillIce)
      AudioManager.Instance.PlayCue(Banks.Spells, Barrier.Ice_Barrier_Break_Sound_Hash, this.AudioEmitter);
    else if ((this.mSpell.Element & Elements.Earth) == Elements.Earth)
      AudioManager.Instance.PlayCue(Banks.Spells, Barrier.Earth_Barrier_Break_Sound_Hash, this.AudioEmitter);
    if ((this.mSpell.Element & Elements.Ice) == Elements.Ice && !this.mKillIce)
    {
      EffectManager.Instance.Stop(ref this.mSpawnDeathEffectReference);
      Vector3 position = this.Body.Position;
      position.Y -= 0.5f * this.Capsule.Length + this.Capsule.Radius;
      Vector3 forward = this.Body.Orientation.Forward;
      EffectManager.Instance.StartEffect(Barrier.Ice_Barrier_Death_Effect_Hash, ref position, ref forward, out this.mSpawnDeathEffectReference);
    }
    if ((this.mSpell.Element & Elements.Earth) != Elements.Earth)
      return;
    EffectManager.Instance.Stop(ref this.mSpawnDeathEffectReference);
    Vector3 position1 = this.Body.Position;
    position1.Y -= 0.5f * this.Capsule.Length + this.Capsule.Radius;
    Vector3 forward1 = this.Body.Orientation.Forward;
    EffectManager.Instance.StartEffect(Barrier.Earth_Barrier_Death_Effect_Hash, ref position1, ref forward1, out this.mSpawnDeathEffectReference);
  }

  protected void RunDeathAnimation()
  {
    if (this.mDead)
      return;
    this.mDead = true;
    if ((this.mSpell.Element & Elements.Beams) != Elements.None)
    {
      this.Detonate();
    }
    else
    {
      if (this.mIceAnimationController.IsPlaying || this.mEarthAnimationController.IsPlaying)
        return;
      this.mIceAnimationController.PlaybackMode = PlaybackMode.Backward;
      this.mEarthAnimationController.PlaybackMode = PlaybackMode.Backward;
      if ((this.mSpell.Element & Elements.Ice) == Elements.Ice && (this.mSpell.Element & Elements.Earth) == Elements.Earth && !this.mKillIce)
        AudioManager.Instance.PlayCue(Banks.Spells, Barrier.Ice_Earth_Barrier_Break_Sound_Hash, this.AudioEmitter);
      else if ((this.mSpell.Element & Elements.Ice) == Elements.Ice && !this.mKillIce)
        AudioManager.Instance.PlayCue(Banks.Spells, Barrier.Ice_Barrier_Break_Sound_Hash, this.AudioEmitter);
      else if ((this.mSpell.Element & Elements.Earth) == Elements.Earth)
        AudioManager.Instance.PlayCue(Banks.Spells, Barrier.Earth_Barrier_Break_Sound_Hash, this.AudioEmitter);
      if ((this.mSpell.Element & Elements.Ice) == Elements.Ice && !this.mKillIce)
      {
        EffectManager.Instance.Stop(ref this.mSpawnDeathEffectReference);
        Vector3 position = this.Body.Position;
        position.Y -= 0.5f * this.Capsule.Length + this.Capsule.Radius;
        Vector3 forward = this.Body.Orientation.Forward;
        EffectManager.Instance.StartEffect(Barrier.Ice_Barrier_Death_Effect_Hash, ref position, ref forward, out this.mSpawnDeathEffectReference);
        this.mIceAnimationController.StartClip(Barrier.sIceAppearClips[0], false);
      }
      if ((this.mSpell.Element & Elements.Earth) != Elements.Earth)
        return;
      EffectManager.Instance.Stop(ref this.mSpawnDeathEffectReference);
      Vector3 position1 = this.Body.Position;
      position1.Y -= 0.5f * this.Capsule.Length + this.Capsule.Radius;
      Vector3 forward1 = this.Body.Orientation.Forward;
      EffectManager.Instance.StartEffect(Barrier.Earth_Barrier_Death_Effect_Hash, ref position1, ref forward1, out this.mSpawnDeathEffectReference);
      this.mEarthAnimationController.StartClip(Barrier.sEarthAppearClips[0], false);
    }
  }

  public void RunIceEarthDeathAnimation()
  {
    if (this.mIceAnimationController.IsPlaying)
      return;
    this.mIceAnimationController.PlaybackMode = PlaybackMode.Backward;
    AudioManager.Instance.PlayCue(Banks.Spells, Barrier.Ice_Barrier_Break_Sound_Hash, this.AudioEmitter);
    EffectManager.Instance.Stop(ref this.mSpawnDeathEffectReference);
    Vector3 position = this.Body.Position;
    position.Y -= 0.5f * this.Capsule.Length + this.Capsule.Radius;
    Vector3 forward = this.Body.Orientation.Forward;
    EffectManager.Instance.StartEffect(Barrier.Ice_Barrier_Death_Effect_Hash, ref position, ref forward, out this.mSpawnDeathEffectReference);
    this.mIceAnimationController.StartClip(Barrier.sIceAppearClips[0], false);
  }

  public Capsule Capsule => this.mCollision.GetPrimitiveNewWorld(0) as Capsule;

  public bool SegmentIntersect(out Vector3 oPosition, Segment iSeg, float iSegmentRadius)
  {
    Segment seg0 = new Segment();
    seg0.Origin = this.Position;
    seg0.Origin.Y -= this.Capsule.Length * 0.5f;
    seg0.Delta = this.Capsule.Orientation.Backward;
    Vector3.Multiply(ref seg0.Delta, this.Capsule.Length, out seg0.Delta);
    float t0;
    float t1;
    float num1 = Distance.SegmentSegmentDistanceSq(out t0, out t1, seg0, iSeg);
    float num2 = iSegmentRadius + this.Capsule.Radius;
    float num3 = num2 * num2;
    if ((double) num1 > (double) num3)
    {
      oPosition = new Vector3();
      return false;
    }
    Vector3 point;
    seg0.GetPoint(t0, out point);
    Vector3 vector3;
    iSeg.GetPoint(t1, out vector3);
    Vector3.Subtract(ref vector3, ref point, out vector3);
    vector3.Normalize();
    Vector3.Multiply(ref vector3, this.Capsule.Radius, out vector3);
    Vector3.Add(ref point, ref vector3, out oPosition);
    return true;
  }

  public bool ArcIntersect(
    out Vector3 oPosition,
    ref Vector3 iOrigin,
    ref Vector3 iDirection,
    float iRange,
    float iAngle,
    float iHeightDifference)
  {
    iOrigin.Y = 0.0f;
    iDirection.Y = 0.0f;
    Vector3 position1 = this.Position with { Y = 0.0f };
    Vector3 result1;
    Vector3.Subtract(ref iOrigin, ref position1, out result1);
    float num1 = result1.Length();
    float radius = this.Capsule.Radius;
    if ((double) num1 - (double) radius > (double) iRange)
    {
      oPosition = new Vector3();
      return false;
    }
    Vector3.Divide(ref result1, num1, out result1);
    float result2;
    Vector3.Dot(ref result1, ref iDirection, out result2);
    float num2 = (float) Math.Acos((double) -result2);
    float num3 = -2f * num1 * num1;
    float num4 = (float) Math.Acos(((double) radius * (double) radius + (double) num3) / (double) num3);
    if ((double) num2 - (double) num4 < (double) iAngle)
    {
      Vector3.Multiply(ref result1, radius, out result1);
      Vector3 position2 = this.Position;
      Vector3.Add(ref position2, ref result1, out oPosition);
      return true;
    }
    oPosition = new Vector3();
    return false;
  }

  public void OverKill() => this.mHitPoints = -this.mMaxHitPoints;

  protected override unsafe void INetworkUpdate(ref EntityUpdateMessage iMsg)
  {
    base.INetworkUpdate(ref iMsg);
    this.mHitPoints = iMsg.HitPoints;
    fixed (float* numPtr = iMsg.StatusEffectMagnitude)
    {
      for (int index = 0; index < 9; ++index)
        this.mStatusEffects[index].Magnitude = numPtr[index];
    }
  }

  protected override unsafe void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
  {
    oMsg = new EntityUpdateMessage();
    if (!this.RestingMovement)
    {
      oMsg.Features |= EntityFeatures.Position;
      oMsg.Position = this.Position;
    }
    oMsg.Features |= EntityFeatures.Damageable;
    oMsg.HitPoints = this.mHitPoints;
    oMsg.Features |= EntityFeatures.StatusEffected;
    oMsg.StatusEffects = this.mCurrentStatusEffects;
    fixed (float* numPtr = oMsg.StatusEffectMagnitude)
    {
      for (int index = 0; index < 9; ++index)
        numPtr[index] = this.mStatusEffects[index].Magnitude;
    }
  }

  internal override float GetDanger() => this.mDamage.GetTotalMagnitude();

  protected class RenderData : IRenderableObject
  {
    protected VertexDeclaration mVertexDeclaration;
    protected int mBaseVertex;
    protected int mNumVertices;
    protected int mPrimitiveCount;
    protected int mStartIndex;
    protected int mStreamOffset;
    protected int mVertexStride;
    public float mDamage;
    protected VertexBuffer mVertexBuffer;
    protected IndexBuffer mIndexBuffer;
    public Matrix[] mBones;
    public BoundingSphere mBoundingSphere;
    public SkinnedModelDeferredBasicMaterial mSkinnedModelMaterial;
    public int mVerticesHash;

    public RenderData()
    {
      this.mBones = new Matrix[80 /*0x50*/];
      this.mDamage = 0.0f;
    }

    public int Effect => SkinnedModelDeferredEffect.TYPEHASH;

    public int DepthTechnique => 3;

    public int Technique => 0;

    public int ShadowTechnique => 4;

    public VertexBuffer Vertices => this.mVertexBuffer;

    public IndexBuffer Indices => this.mIndexBuffer;

    public VertexDeclaration VertexDeclaration => this.mVertexDeclaration;

    public int VertexStride => this.mVertexStride;

    public Texture2D Texture => this.mSkinnedModelMaterial.DiffuseMap0;

    public int VerticesHashCode => this.mVerticesHash;

    public bool Cull(BoundingFrustum iViewFrustum)
    {
      return this.mBoundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
    }

    public virtual void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      SkinnedModelDeferredEffect iEffect1 = iEffect as SkinnedModelDeferredEffect;
      this.mSkinnedModelMaterial.AssignToEffect(iEffect1);
      iEffect1.Damage = this.mDamage;
      iEffect1.Bones = this.mBones;
      iEffect1.CubeMapEnabled = false;
      iEffect1.CubeNormalMapEnabled = false;
      iEffect1.ProjectionMapEnabled = false;
      iEffect1.SpecularBias = 0.0f;
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
    }

    public virtual void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      SkinnedModelDeferredEffect iEffect1 = iEffect as SkinnedModelDeferredEffect;
      this.mSkinnedModelMaterial.AssignOpacityToEffect(iEffect1);
      iEffect1.Bones = this.mBones;
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
    }

    public virtual void SetMesh(
      VertexBuffer iVertices,
      IndexBuffer iIndices,
      ModelMeshPart iMeshPart)
    {
      this.mVertexBuffer = iVertices;
      this.mVerticesHash = iVertices.GetHashCode();
      this.mIndexBuffer = iIndices;
      this.mVertexDeclaration = iMeshPart.VertexDeclaration;
      this.mBaseVertex = iMeshPart.BaseVertex;
      this.mNumVertices = iMeshPart.NumVertices;
      this.mPrimitiveCount = iMeshPart.PrimitiveCount;
      this.mStartIndex = iMeshPart.StartIndex;
      this.mStreamOffset = iMeshPart.StreamOffset;
      this.mVertexStride = iMeshPart.VertexStride;
      Helper.SkinnedModelDeferredMaterialFromBasicEffect(iMeshPart.Effect as SkinnedModelBasicEffect, out this.mSkinnedModelMaterial);
    }
  }

  public class HitListWithBarriers
  {
    private static List<Barrier.HitListWithBarriers> sInstances;
    private static List<Barrier.HitListWithBarriers> sHitListCache;
    private ushort mHandle;
    private Magicka.GameLogic.HitList mHitList = new Magicka.GameLogic.HitList(64 /*0x40*/);
    private List<Barrier> mOwners = new List<Barrier>(32 /*0x20*/);

    public static void InitializeCache(int iNrOfBarriers)
    {
      Barrier.HitListWithBarriers.sInstances = new List<Barrier.HitListWithBarriers>(64 /*0x40*/);
      Barrier.HitListWithBarriers.sHitListCache = new List<Barrier.HitListWithBarriers>(iNrOfBarriers);
      for (int index = 0; index < iNrOfBarriers; ++index)
        Barrier.HitListWithBarriers.sHitListCache.Add(new Barrier.HitListWithBarriers());
    }

    private HitListWithBarriers()
    {
      this.mHandle = (ushort) Barrier.HitListWithBarriers.sInstances.Count;
      Barrier.HitListWithBarriers.sInstances.Add(this);
    }

    public static Barrier.HitListWithBarriers GetFromCache()
    {
      lock (Barrier.HitListWithBarriers.sHitListCache)
      {
        if (Barrier.HitListWithBarriers.sHitListCache.Count == 0)
          return new Barrier.HitListWithBarriers();
      }
      int index = Barrier.HitListWithBarriers.sHitListCache.Count - 1;
      Barrier.HitListWithBarriers fromCache = Barrier.HitListWithBarriers.sHitListCache[index];
      Barrier.HitListWithBarriers.sHitListCache.RemoveAt(index);
      return fromCache;
    }

    public static Barrier.HitListWithBarriers GetByHandle(ushort iHandle)
    {
      Barrier.HitListWithBarriers sInstance = Barrier.HitListWithBarriers.sInstances[(int) iHandle];
      Barrier.HitListWithBarriers.sHitListCache.Remove(sInstance);
      return sInstance;
    }

    public void Destroy()
    {
      if (Barrier.HitListWithBarriers.sHitListCache.Contains(this))
        return;
      Barrier.HitListWithBarriers.sHitListCache.Add(this);
    }

    public ushort Handle => this.mHandle;

    public List<Barrier> Owners => this.mOwners;

    public Magicka.GameLogic.HitList HitList => this.mHitList;
  }

  [Flags]
  protected enum DrawMethod
  {
    NONE = 0,
    NORMAL = 1,
    PARTICLEWALL = 2,
    PARTICLEFOUNTAIN = 4,
  }

  protected enum BarrierType
  {
    SOLID,
    ELEMENTAL,
  }
}
