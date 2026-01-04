// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Bosses.Grimnir2
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Levels.Triggers;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;
using System.Collections.Generic;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

#nullable disable
namespace Magicka.GameLogic.Entities.Bosses;

internal class Grimnir2 : BossStatusEffected, IBossSpellCaster, IBoss
{
  private const float NETWORK_UPDATE_PERIOD = 0.0333333351f;
  private const float MAXHITPOINTS = 35000f;
  private const float CAPSULE_RADIUS = 0.75f;
  private const float CAPSULE_LENGTH = 1.5f;
  private const int GRIMNIRID = 0;
  private const int LEFTSPIRITID = 1;
  private const int RIGHTSPIRITID = 2;
  private const float SPIRIT_TIME = 25f;
  private float mLastNetworkUpdate;
  protected float mNetworkUpdateTimer;
  private static readonly int ASSATUR_HEAL_EFFECT = "assatur_life".GetHashCodeCustom();
  private static readonly KeyValuePair<int, Banks> ASSATUR_HEAL_SOUND = new KeyValuePair<int, Banks>("spell_life_self".GetHashCodeCustom(), Banks.Spells);
  private static readonly Random RANDOM = new Random();
  private static readonly int ASSATUR_TALKS_TRIGGER = "assatur_talks".GetHashCodeCustom();
  private static readonly int ASSATUR_APPEARS_TRIGGER = "assatur_appears".GetHashCodeCustom();
  private static readonly int CAST_TORNADO_EFFECT = "grimnir_tornado".GetHashCodeCustom();
  private static readonly int CAST_THUNDER_EFFECT = "grimnir_thunder".GetHashCodeCustom();
  private static readonly int CAST_CONFLAG_EFFECT = "grimnir_conflagration".GetHashCodeCustom();
  private static readonly int CAST_RAIN_EFFECT = "grimnir_rain".GetHashCodeCustom();
  private static readonly int CAST_SPELL_EFFECT = "grimnir_spell".GetHashCodeCustom();
  private static readonly int BLOOD_EFFECT = "gore_splash_regular".GetHashCodeCustom();
  private static readonly int GENERIC_MAGICK = "magick_generic".GetHashCodeCustom();
  private VisualEffectReference mCastEffect;
  private Grimnir2.Animations mLeftCastAnimation;
  private Grimnir2.Animations mRightCastAnimation;
  private static readonly Grimnir2.SpellData[] SPIRITSPELLS = new Grimnir2.SpellData[2];
  private static readonly Grimnir2.SpellData[] GRIMNIRSPELLS = new Grimnir2.SpellData[3];
  private AnimationClip[] mAsaClips;
  private AnimationController mAsaController;
  private Grimnir2.RenderData[] mAsaRenderData;
  private Grimnir2.AdditiveRenderData[] mAdditiveAsaRenderData;
  private AnimationClip[] mClips;
  private AnimationController mController;
  private AnimationController mLeftSpiritController;
  private AnimationController mRightSpiritController;
  private Grimnir2.RenderData[] mRenderData;
  private Grimnir2.AdditiveRenderData[] mAdditiveRenderData;
  private Matrix mOrientation;
  private IBossState<Grimnir2> mCurrentState;
  private float mSpectralAlpha;
  private float mSpectralTargetAlpha;
  private Matrix mLeftSpecOrientation;
  private Matrix mRightSpecOrientation;
  private Grimnir2.ISpiritState mLeftSpiritState;
  private Grimnir2.ISpiritState mRightSpiritState;
  private int mCastAttachIndex;
  private int mWeaponAttachIndex;
  private bool mAssaturHeal;
  private MagickType mLastMagick = MagickType.MeteorS;
  private float mGrimnirIdleTimer;
  private float mGrimnirSpellPower;
  private CastType mGrimnirCastType;
  private SpellEffect mGrimnirSpellEffect;
  private int mGrimnirLastSpell;
  private float mLeftSpiritIdleTimer;
  private bool mLeftSpiritHasCastSpell;
  private float mLeftSpiritSpellPower;
  private CastType mLeftSpiritCastType;
  private SpellEffect mLeftSpiritSpellEffect;
  private int mLeftSpiritLastSpell;
  private float mRightSpiritIdleTimer;
  private bool mRightSpiritHasCastSpell;
  private float mRightSpiritSpellPower;
  private CastType mRightSpiritCastType;
  private SpellEffect mRightSpiritSpellEffect;
  private int mRightSpiritLastSpell;
  private Magicka.GameLogic.Entities.Character mTarget;
  private Vector3 mTargetPosition;
  private float mLeftFloatCounter;
  private float mRightFloatCounter;
  private float mSpiritTimer;
  private float mTargetSwapTimer;
  private PlayState mPlayState;
  private BossSpellCasterZone mGrimnirBody;
  private BossSpellCasterZone mLeftSpiritBody;
  private BossSpellCasterZone mRightSpiritBody;
  private TextureCube mIceCubeMap;
  private TextureCube mIceCubeNormalMap;
  private Matrix YROT = Matrix.CreateRotationY(3.14159274f);
  private float mAssaturAlpha;
  private Matrix mAssaturOrientation;
  private bool mUseBothSpirits;
  private float mDamageFlashTimer;
  private BoundingSphere mGrimnirBoundingSphere;
  private BoundingSphere mSpiritBoundingSphere;
  private BoundingSphere mAsaBoundingSphere;
  private SkinnedModelDeferredAdvancedMaterial mGrimnirMaterial;
  private bool mCalledAppearTrigger;
  private bool mLoopFight;
  private bool mCorporeal;
  private GibReference[] mGrimnirGibs;
  private Matrix mCliffOrientation;

  static Grimnir2()
  {
    Grimnir2.SpellData spellData;
    spellData.SPELL = new Spell();
    spellData.SPELL.Element = Elements.Lightning | Elements.Arcane;
    spellData.SPELL.ArcaneMagnitude = 1f;
    spellData.SPELL.LightningMagnitude = 3f;
    spellData.CASTTYPE = CastType.Force;
    spellData.SPELLPOWER = 1f;
    Grimnir2.SPIRITSPELLS[0] = spellData;
    spellData.SPELL = new Spell();
    spellData.SPELL.Element = Elements.Earth | Elements.Fire;
    spellData.SPELL.EarthMagnitude = 2f;
    spellData.SPELL.FireMagnitude = 2f;
    spellData.CASTTYPE = CastType.Force;
    spellData.SPELLPOWER = 0.1f;
    Grimnir2.SPIRITSPELLS[1] = spellData;
    spellData.SPELL = new Spell();
    spellData.SPELL.Element = Elements.Ice;
    spellData.SPELL.IceMagnitude = 5f;
    spellData.CASTTYPE = CastType.Area;
    spellData.SPELLPOWER = 1f;
    Grimnir2.GRIMNIRSPELLS[0] = spellData;
    spellData.SPELL = new Spell();
    spellData.SPELL.Element = Elements.Earth | Elements.Shield;
    spellData.SPELL.ShieldMagnitude = 1f;
    spellData.SPELL.EarthMagnitude = 4f;
    spellData.CASTTYPE = CastType.Force;
    spellData.SPELLPOWER = 1f;
    Grimnir2.GRIMNIRSPELLS[1] = spellData;
    spellData.SPELL = new Spell();
    spellData.SPELL.Element = Elements.Shield;
    spellData.SPELL.ShieldMagnitude = 1f;
    spellData.CASTTYPE = CastType.Force;
    spellData.SPELLPOWER = 1f;
    Grimnir2.GRIMNIRSPELLS[2] = spellData;
  }

  public Grimnir2(PlayState iPlayState)
  {
    this.mPlayState = iPlayState;
    this.mGrimnirGibs = new GibReference[6];
    SkinnedModel skinnedModel1;
    SkinnedModel skinnedModel2;
    SkinnedModel skinnedModel3;
    SkinnedModel skinnedModel4;
    lock (this.mPlayState.Scene.GraphicsDevice)
    {
      skinnedModel1 = this.mPlayState.Content.Load<SkinnedModel>("Models/Bosses/grimnir/grimnir_mesh");
      skinnedModel2 = this.mPlayState.Content.Load<SkinnedModel>("Models/Bosses/grimnir/grimnir_animation");
      skinnedModel3 = this.mPlayState.Content.Load<SkinnedModel>("Models/Bosses/grimnir/grimnir_mesh");
      skinnedModel4 = this.mPlayState.Content.Load<SkinnedModel>("Models/Bosses/assatur/assatur");
      this.mIceCubeMap = this.mPlayState.Content.Load<TextureCube>("EffectTextures/iceCube");
      this.mIceCubeNormalMap = this.mPlayState.Content.Load<TextureCube>("EffectTextures/iceCube_NRM");
    }
    for (int index = 0; index < skinnedModel2.SkeletonBones.Count; ++index)
    {
      SkinnedModelBone skeletonBone = skinnedModel2.SkeletonBones[index];
      if (skeletonBone.Name.Equals("leftattach", StringComparison.OrdinalIgnoreCase))
        this.mCastAttachIndex = (int) skeletonBone.Index;
      else if (skeletonBone.Name.Equals("rightattach", StringComparison.OrdinalIgnoreCase))
        this.mWeaponAttachIndex = (int) skeletonBone.Index;
    }
    this.mAsaController = new AnimationController();
    this.mAsaController.Skeleton = skinnedModel4.SkeletonBones;
    this.mAsaClips = new AnimationClip[3];
    this.mAsaClips[0] = skinnedModel4.AnimationClips["idle"];
    this.mAsaClips[1] = skinnedModel4.AnimationClips["cast_self"];
    this.mAsaClips[2] = skinnedModel4.AnimationClips["cast_ground"];
    this.mController = new AnimationController();
    this.mController.Skeleton = skinnedModel2.SkeletonBones;
    this.mClips = new AnimationClip[15];
    this.mClips[0] = skinnedModel2.AnimationClips["die"];
    this.mClips[1] = skinnedModel2.AnimationClips["hanging"];
    this.mClips[2] = skinnedModel2.AnimationClips["taunt"];
    this.mClips[3] = skinnedModel2.AnimationClips["talk0"];
    this.mClips[4] = skinnedModel2.AnimationClips["talk1"];
    this.mClips[5] = skinnedModel2.AnimationClips["talk2"];
    this.mClips[6] = skinnedModel2.AnimationClips["idle"];
    this.mClips[8] = skinnedModel2.AnimationClips["spirit_cast_projectile"];
    this.mClips[9] = skinnedModel2.AnimationClips["spirit_cast_railgun"];
    this.mClips[7] = skinnedModel2.AnimationClips["spirit_idle"];
    this.mClips[10] = skinnedModel2.AnimationClips["spirit_die"];
    this.mClips[11] = skinnedModel2.AnimationClips["cast_magick_global"];
    this.mClips[12] = skinnedModel2.AnimationClips["cast_magick_direct"];
    this.mClips[13] = skinnedModel2.AnimationClips["summon_spirits"];
    this.mClips[14] = skinnedModel2.AnimationClips["cast_shield"];
    this.mLeftSpiritController = new AnimationController();
    this.mLeftSpiritController.Skeleton = skinnedModel3.SkeletonBones;
    this.mRightSpiritController = new AnimationController();
    this.mRightSpiritController.Skeleton = skinnedModel3.SkeletonBones;
    Capsule capsule = new Capsule(new Vector3(0.0f, -0.375f, 0.0f), Matrix.CreateRotationX(-1.57079637f), 0.75f, 1.5f);
    this.mGrimnirBody = new BossSpellCasterZone(this.mPlayState, (IBossSpellCaster) this, this.mController, this.mCastAttachIndex, this.mWeaponAttachIndex, 0, 1f, new Primitive[1]
    {
      (Primitive) capsule
    });
    this.mGrimnirBody.Body.CollisionSkin.callbackFn += new CollisionCallbackFn(this.OnCollision);
    this.mLeftSpiritBody = new BossSpellCasterZone(this.mPlayState, (IBossSpellCaster) this, this.mLeftSpiritController, this.mCastAttachIndex, this.mWeaponAttachIndex, 1, 1.5f, new Primitive[1]
    {
      (Primitive) capsule
    });
    this.mLeftSpiritBody.Body.CollisionSkin.callbackFn += new CollisionCallbackFn(this.OnCollision);
    this.mLeftSpiritBody.IsEthereal = true;
    this.mRightSpiritBody = new BossSpellCasterZone(this.mPlayState, (IBossSpellCaster) this, this.mRightSpiritController, this.mCastAttachIndex, this.mWeaponAttachIndex, 2, 1.5f, new Primitive[1]
    {
      (Primitive) capsule
    });
    this.mRightSpiritBody.Body.CollisionSkin.callbackFn += new CollisionCallbackFn(this.OnCollision);
    this.mRightSpiritBody.IsEthereal = true;
    Helper.SkinnedModelDeferredMaterialFromBasicEffect(skinnedModel1.Model.Meshes[0].MeshParts[0].Effect as SkinnedModelBasicEffect, out this.mGrimnirMaterial);
    SkinnedModelDeferredAdvancedMaterial oMaterial1;
    Helper.SkinnedModelDeferredMaterialFromBasicEffect(skinnedModel3.Model.Meshes[0].MeshParts[0].Effect as SkinnedModelBasicEffect, out oMaterial1);
    SkinnedModelDeferredAdvancedMaterial oMaterial2;
    Helper.SkinnedModelDeferredMaterialFromBasicEffect(skinnedModel4.Model.Meshes[0].MeshParts[0].Effect as SkinnedModelBasicEffect, out oMaterial2);
    this.mGrimnirBoundingSphere = skinnedModel1.Model.Meshes[0].BoundingSphere;
    this.mSpiritBoundingSphere = new BoundingSphere(Vector3.Zero, 30f);
    this.mAsaBoundingSphere = skinnedModel4.Model.Meshes[0].BoundingSphere;
    this.mAsaRenderData = new Grimnir2.RenderData[3];
    this.mAdditiveAsaRenderData = new Grimnir2.AdditiveRenderData[3];
    this.mRenderData = new Grimnir2.RenderData[3];
    this.mAdditiveRenderData = new Grimnir2.AdditiveRenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      this.mRenderData[index] = new Grimnir2.RenderData();
      this.mRenderData[index].SetMesh(skinnedModel1.Model.Meshes[0].VertexBuffer, skinnedModel1.Model.Meshes[0].IndexBuffer, skinnedModel1.Model.Meshes[0].MeshParts[0], 0, 3, 4);
      this.mRenderData[index].mMaterial = this.mGrimnirMaterial;
      this.mAdditiveRenderData[index] = new Grimnir2.AdditiveRenderData();
      this.mAdditiveRenderData[index].SetMesh(skinnedModel3.Model.Meshes[0].VertexBuffer, skinnedModel3.Model.Meshes[0].IndexBuffer, skinnedModel3.Model.Meshes[0].MeshParts[0], 2);
      this.mAdditiveRenderData[index].mMaterial = oMaterial1;
      this.mAsaRenderData[index] = new Grimnir2.RenderData();
      this.mAsaRenderData[index].SetMesh(skinnedModel4.Model.Meshes[0].VertexBuffer, skinnedModel4.Model.Meshes[0].IndexBuffer, skinnedModel4.Model.Meshes[0].MeshParts[0], 0, 3, 4);
      this.mAsaRenderData[index].mMaterial = oMaterial2;
      this.mAdditiveAsaRenderData[index] = new Grimnir2.AdditiveRenderData();
      this.mAdditiveAsaRenderData[index].SetMesh(skinnedModel4.Model.Meshes[0].VertexBuffer, skinnedModel4.Model.Meshes[0].IndexBuffer, skinnedModel4.Model.Meshes[0].MeshParts[0], 2);
      this.mAdditiveAsaRenderData[index].mMaterial = oMaterial2;
    }
    for (int iIndex = 0; iIndex < this.mResistances.Length; ++iIndex)
    {
      Elements elements = Spell.ElementFromIndex(iIndex);
      this.mResistances[iIndex].Multiplier = 1f;
      this.mResistances[iIndex].ResistanceAgainst = elements;
      this.mResistances[iIndex].Modifier = 0.0f;
    }
    this.mLoopFight = false;
  }

  public bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
  {
    return false;
  }

  public void Initialize(ref Matrix iOrientation, int iUniqueID)
  {
    this.Initialize(ref iOrientation);
  }

  public void Initialize(ref Matrix iOrientation)
  {
    for (int index = 0; index < this.mStatusEffects.Length; ++index)
      this.mStatusEffects[index].Stop();
    this.mOrientation = iOrientation;
    this.mGrimnirBoundingSphere.Center = this.mOrientation.Translation;
    this.mCalledAppearTrigger = false;
    this.mMaxHitPoints = 35000f;
    this.mHitPoints = 35000f;
    EffectManager.Instance.Stop(ref this.mCastEffect);
    this.mGrimnirBody.Initialize("#boss_n07".GetHashCodeCustom());
    this.mGrimnirBody.IsEthereal = false;
    this.mPlayState.EntityManager.AddEntity((Magicka.GameLogic.Entities.Entity) this.mGrimnirBody);
    this.mLeftSpiritBody.Initialize();
    this.mPlayState.EntityManager.AddEntity((Magicka.GameLogic.Entities.Entity) this.mLeftSpiritBody);
    this.mRightSpiritBody.Initialize();
    this.mPlayState.EntityManager.AddEntity((Magicka.GameLogic.Entities.Entity) this.mRightSpiritBody);
    this.mAsaController.StartClip(this.mAsaClips[0], true);
    this.mAssaturAlpha = 0.0f;
    this.mAssaturOrientation = iOrientation;
    Vector3 result = iOrientation.Backward;
    Vector3.Multiply(ref result, 5f, out result);
    result.Y -= 6f;
    this.mAssaturOrientation.Translation = iOrientation.Translation + result;
    this.mCurrentState = (IBossState<Grimnir2>) Grimnir2.IntroState.Instance;
    this.mCurrentState.OnEnter(this);
    this.mLeftSpiritState = (Grimnir2.ISpiritState) Grimnir2.SpecIntroState.Instance;
    this.mLeftSpiritState.OnEnter(this.mLeftSpiritBody, this);
    this.mRightSpiritState = (Grimnir2.ISpiritState) Grimnir2.SpecIntroState.Instance;
    this.mRightSpiritState.OnEnter(this.mRightSpiritBody, this);
    Player[] players = Magicka.Game.Instance.Players;
    for (int index = 0; index < players.Length; ++index)
    {
      if (players[index].Playing && players[index].Avatar != null)
      {
        this.mTarget = (Magicka.GameLogic.Entities.Character) players[index].Avatar;
        break;
      }
    }
    this.mTargetPosition = this.mTarget.Position;
    this.mTargetSwapTimer = 30f;
    this.mLeftFloatCounter = 0.0f;
    this.mHitPoints = 35000f;
    this.mAssaturHeal = false;
    this.mCorporeal = false;
    this.mSpectralAlpha = 0.0f;
    this.mRightFloatCounter = 0.0f;
    this.mLeftFloatCounter = 0.0f;
    this.mLeftSpiritBody.IsEthereal = true;
    this.mRightSpiritBody.IsEthereal = true;
    this.mSpiritTimer = 25.1f;
    this.mLoopFight = true;
    foreach (AnimatedLevelPart animatedLevelPart in this.mPlayState.Level.CurrentScene.LevelModel.AnimatedLevelParts.Values)
    {
      if (animatedLevelPart.ID == "cliff".GetHashCodeCustom())
      {
        this.mCliffOrientation = animatedLevelPart.CollisionSkin.NewTransform.Orientation;
        this.mCliffOrientation.Translation = animatedLevelPart.CollisionSkin.NewPosition;
        break;
      }
    }
  }

  public unsafe void UpdateBoss(DataChannel iDataChannel, float iDeltaTime, bool iFightStarted)
  {
    if (NetworkManager.Instance.State == NetworkState.Server)
    {
      this.mNetworkUpdateTimer -= iDeltaTime;
      if ((double) this.mNetworkUpdateTimer <= 0.0)
      {
        this.mNetworkUpdateTimer = 0.0333333351f;
        this.NetworkUpdate();
      }
    }
    this.mTargetSwapTimer -= iDeltaTime;
    this.mCurrentState.OnUpdate(iDeltaTime, this);
    this.mTimeSinceLastDamage += iDeltaTime;
    if (iFightStarted && this.mCurrentState is Grimnir2.IntroState)
    {
      this.ChangeState(Grimnir2.States.Idle);
      this.ChangeSpiritState(Grimnir2.SpiritStates.Idle, 1);
      this.ChangeSpiritState(Grimnir2.SpiritStates.Idle, 2);
    }
    Vector3 translation = this.mOrientation.Translation;
    translation.Y += 1.5f;
    Matrix result1 = this.mOrientation with
    {
      Translation = new Vector3()
    };
    this.mGrimnirBody.Body.MoveTo(translation, result1);
    result1 = this.mOrientation;
    foreach (AnimatedLevelPart animatedLevelPart in this.mPlayState.Level.CurrentScene.LevelModel.AnimatedLevelParts.Values)
    {
      if (animatedLevelPart.ID == "cliff".GetHashCodeCustom() && (double) animatedLevelPart.Time > 0.0)
      {
        Matrix orientation = animatedLevelPart.CollisionSkin.NewTransform.Orientation with
        {
          Translation = animatedLevelPart.CollisionSkin.NewPosition
        };
        Matrix result2;
        Matrix.Invert(ref this.mCliffOrientation, out result2);
        Matrix result3;
        Matrix.Multiply(ref orientation, ref result2, out result3);
        Matrix result4;
        Matrix.Multiply(ref result3, ref result1, out result4);
        result1 = result4;
        break;
      }
    }
    this.UpdateDamage(iDeltaTime);
    this.UpdateStatusEffects(iDeltaTime);
    this.mController.Speed = 1f;
    if (this.HasStatus(StatusEffects.Frozen))
    {
      this.mRenderData[(int) iDataChannel].mMaterial.Bloat = 0.1f;
      this.mRenderData[(int) iDataChannel].mMaterial.EmissiveAmount = 3f;
      this.mRenderData[(int) iDataChannel].mMaterial.SpecularBias = 0.8f;
      this.mRenderData[(int) iDataChannel].mMaterial.SpecularPower = 20f;
      this.mRenderData[(int) iDataChannel].mMaterial.CubeMapRotation = Matrix.Identity;
      this.mRenderData[(int) iDataChannel].mMaterial.CubeMap = this.mIceCubeMap;
      this.mRenderData[(int) iDataChannel].mMaterial.CubeNormalMap = this.mIceCubeNormalMap;
      this.mRenderData[(int) iDataChannel].mMaterial.CubeMapColor = Vector4.One;
      this.mRenderData[(int) iDataChannel].mMaterial.CubeMapEnabled = true;
      this.mRenderData[(int) iDataChannel].mMaterial.CubeNormalMapEnabled = true;
      this.mRenderData[(int) iDataChannel].mMaterial.CubeMapColor.W = 1f - (float) Math.Pow(0.20000000298023224, (double) this.StatusMagnitude(StatusEffects.Frozen));
      this.mController.Speed = 0.0f;
    }
    else
    {
      if (this.HasStatus(StatusEffects.Cold))
        this.mController.Speed *= 0.2f;
      this.mRenderData[(int) iDataChannel].mMaterial.Bloat = 0.0f;
      this.mRenderData[(int) iDataChannel].mMaterial.EmissiveAmount = this.mGrimnirMaterial.EmissiveAmount;
      this.mRenderData[(int) iDataChannel].mMaterial.SpecularBias = this.mGrimnirMaterial.SpecularBias;
      this.mRenderData[(int) iDataChannel].mMaterial.SpecularPower = this.mGrimnirMaterial.SpecularPower;
      this.mRenderData[(int) iDataChannel].mMaterial.CubeMapEnabled = false;
      this.mRenderData[(int) iDataChannel].mMaterial.CubeNormalMapEnabled = false;
    }
    this.mController.Update(iDeltaTime, ref result1, true);
    this.mController.SkinnedBoneTransforms.CopyTo((Array) this.mRenderData[(int) iDataChannel].mBones, 0);
    this.mDamageFlashTimer = Math.Max(this.mDamageFlashTimer - iDeltaTime, 0.0f);
    this.mRenderData[(int) iDataChannel].Flash = this.mDamageFlashTimer * 10f;
    this.mRenderData[(int) iDataChannel].mBoundingSphere = this.mGrimnirBoundingSphere;
    this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) this.mRenderData[(int) iDataChannel]);
    if (NetworkManager.Instance.State != NetworkState.Client && ((double) this.mTargetSwapTimer <= 0.0 || this.mTarget == null || this.mTarget.Dead))
    {
      this.mTargetSwapTimer = 30f;
      int num = Grimnir2.RANDOM.Next(Magicka.Game.Instance.PlayerCount);
      Player[] players = Magicka.Game.Instance.Players;
      for (int index = 0; index < players.Length; ++index)
      {
        if (players[(index + num) % 4].Playing && players[(index + num) % 4].Avatar != null)
        {
          this.mTarget = (Magicka.GameLogic.Entities.Character) players[(index + num) % 4].Avatar;
          if (NetworkManager.Instance.State == NetworkState.Server)
          {
            Grimnir2.ChangeTargetMessage changeTargetMessage;
            changeTargetMessage.Target = this.mTarget.Handle;
            BossFight.Instance.SendMessage<Grimnir2.ChangeTargetMessage>((IBoss) this, (ushort) 7, (void*) &changeTargetMessage, true);
            break;
          }
          break;
        }
      }
    }
    if (this.mGrimnirSpellEffect != null)
    {
      if (this.mGrimnirSpellEffect.CastType == CastType.Weapon)
        this.mGrimnirSpellEffect.AnimationEnd((ISpellCaster) this.mGrimnirBody);
      if (!this.mGrimnirSpellEffect.CastUpdate(iDeltaTime, (ISpellCaster) this.mGrimnirBody, out float _))
      {
        this.mGrimnirSpellEffect.DeInitialize((ISpellCaster) this.mGrimnirBody);
        this.mGrimnirSpellEffect = (SpellEffect) null;
      }
    }
    if (this.mLeftSpiritSpellEffect != null)
    {
      if (this.mLeftSpiritSpellEffect.CastType == CastType.Weapon)
        this.mLeftSpiritSpellEffect.AnimationEnd((ISpellCaster) this.mLeftSpiritBody);
      if (!this.mLeftSpiritSpellEffect.CastUpdate(iDeltaTime, (ISpellCaster) this.mLeftSpiritBody, out float _))
      {
        this.mLeftSpiritSpellEffect.DeInitialize((ISpellCaster) this.mGrimnirBody);
        this.mLeftSpiritSpellEffect = (SpellEffect) null;
      }
    }
    if (this.mRightSpiritSpellEffect != null)
    {
      if (this.mRightSpiritSpellEffect.CastType == CastType.Weapon)
        this.mRightSpiritSpellEffect.AnimationEnd((ISpellCaster) this.mRightSpiritBody);
      if (!this.mRightSpiritSpellEffect.CastUpdate(iDeltaTime, (ISpellCaster) this.mRightSpiritBody, out float _))
      {
        this.mRightSpiritSpellEffect.DeInitialize((ISpellCaster) this.mGrimnirBody);
        this.mRightSpiritSpellEffect = (SpellEffect) null;
      }
    }
    this.mTargetPosition.X += (float) (((double) this.mTarget.Position.X - (double) this.mTargetPosition.X) * (double) iDeltaTime * 2.0);
    this.mTargetPosition.Y += (float) (((double) this.mTarget.Position.Y - (double) this.mTargetPosition.Y) * (double) iDeltaTime * 2.0);
    this.mTargetPosition.Z += (float) (((double) this.mTarget.Position.Z - (double) this.mTargetPosition.Z) * (double) iDeltaTime * 2.0);
    if (!this.mAssaturHeal)
    {
      this.mSpiritTimer += iDeltaTime;
      this.mRightFloatCounter += iDeltaTime * 0.05f;
      this.mLeftFloatCounter += iDeltaTime * 0.075f;
    }
    Vector3 result5;
    if ((double) this.mSpiritTimer <= 25.0)
    {
      this.mSpiritBoundingSphere.Radius = 40f;
      this.mSpiritBoundingSphere.Center = this.mOrientation.Translation;
      this.mAdditiveRenderData[(int) iDataChannel].mBoundingSphere = this.mSpiritBoundingSphere;
      if ((double) this.mSpiritTimer > 25.0)
      {
        this.ChangeSpiritState(Grimnir2.SpiritStates.Idle, 1);
        if (this.mUseBothSpirits)
          this.ChangeSpiritState(Grimnir2.SpiritStates.Idle, 2);
      }
      this.mLeftSpiritState.OnUpdate(iDeltaTime, this.mLeftSpiritBody, this);
      this.mRightSpiritState.OnUpdate(iDeltaTime, this.mRightSpiritBody, this);
      float num1 = (float) Math.Pow(Math.Sin((double) this.mLeftFloatCounter), 2.0);
      float num2 = (float) Math.Pow(Math.Sin((double) this.mRightFloatCounter), 2.0);
      float num3 = Math.Min(this.mSpiritTimer * 0.25f, 1f);
      Vector3 result6 = this.mOrientation.Forward;
      Vector3.Multiply(ref result6, num3 * (float) (10.0 + (double) num2 * 0.25), out result6);
      Matrix result7;
      Matrix.CreateRotationY((float) (3.1415927410125732 + (double) Math.Abs(num2) * (double) num3 * 1.5707963705062866 + 0.62831854820251465 * (double) num3), out result7);
      Matrix.Multiply(ref result7, ref this.mOrientation, out this.mLeftSpecOrientation);
      result5 = this.mLeftSpecOrientation.Translation;
      Vector3.Transform(ref result6, ref result7, out result6);
      Vector3.Add(ref result5, ref result6, out result5);
      result5.Y = this.mTargetPosition.Y - 1.5f * num3;
      Matrix.Multiply(ref this.YROT, ref this.mLeftSpecOrientation, out this.mLeftSpecOrientation);
      Vector3 result8 = this.mLeftSpecOrientation.Backward;
      Vector3.Multiply(ref result8, num3 * 10f, out result8);
      Vector3 result9 = this.mOrientation.Forward;
      Vector3.Multiply(ref result9, num3 * 10f, out result9);
      Vector3.Add(ref result9, ref result5, out result5);
      this.mLeftSpecOrientation.Translation = result5;
      Vector3 up = Vector3.Up;
      Matrix.CreateConstrainedBillboard(ref result5, ref this.mTargetPosition, ref up, new Vector3?(), new Vector3?(), out result1);
      result1.Translation = new Vector3();
      result5.Y += 0.75f;
      this.mLeftSpiritBody.Body.MoveTo(result5, result1);
      result5.Y -= 0.75f;
      result1.Translation = result5;
      this.mLeftSpiritBody.IsEthereal = true;
      this.mLeftSpiritController.Update(iDeltaTime, ref result1, true);
      this.mLeftSpiritController.SkinnedBoneTransforms.CopyTo((Array) this.mAdditiveRenderData[(int) iDataChannel].mLeftSpecBones, 0);
      this.mAdditiveRenderData[(int) iDataChannel].RenderLeftSpectral = true;
      if (!this.mUseBothSpirits)
      {
        this.mAdditiveRenderData[(int) iDataChannel].RenderRightSpectral = false;
      }
      else
      {
        result6 = this.mOrientation.Forward;
        Vector3.Multiply(ref result6, num3 * (float) (10.0 + (double) num1 * 0.5), out result6);
        Matrix.CreateRotationY((float) (3.1415927410125732 - (double) Math.Abs(num1) * (double) num3 * 1.5707963705062866 - 0.62831854820251465 * (double) num3), out result7);
        Matrix.Multiply(ref result7, ref this.mOrientation, out this.mRightSpecOrientation);
        result5 = this.mRightSpecOrientation.Translation;
        Vector3.Transform(ref result6, ref result7, out result6);
        Vector3.Add(ref result5, ref result6, out result5);
        result5.Y = this.mTargetPosition.Y - 1.5f * num3;
        Matrix.Multiply(ref this.YROT, ref this.mRightSpecOrientation, out this.mRightSpecOrientation);
        Vector3 result10 = this.mRightSpecOrientation.Backward;
        Vector3.Multiply(ref result10, num3 * 10f, out result10);
        result9 = this.mOrientation.Forward;
        Vector3.Multiply(ref result9, num3 * 10f, out result9);
        Vector3.Add(ref result9, ref result5, out result5);
        this.mRightSpecOrientation.Translation = result5;
        Matrix.CreateConstrainedBillboard(ref result5, ref this.mTargetPosition, ref up, new Vector3?(), new Vector3?(), out result1);
        result1.Translation = new Vector3();
        result5.Y += 0.75f;
        this.mRightSpiritBody.Body.MoveTo(result5, result1);
        result5.Y -= 0.75f;
        result1.Translation = result5;
        this.mRightSpiritBody.IsEthereal = true;
        this.mRightSpiritController.Update(iDeltaTime, ref result1, true);
        this.mRightSpiritController.SkinnedBoneTransforms.CopyTo((Array) this.mAdditiveRenderData[(int) iDataChannel].mRightSpecBones, 0);
        this.mAdditiveRenderData[(int) iDataChannel].RenderRightSpectral = true;
      }
      this.mSpectralAlpha += (this.mSpectralTargetAlpha - this.mSpectralAlpha) * iDeltaTime;
      float num4 = Math.Min(Math.Min(this.mSpiritTimer, 1f) * Math.Max(25f - this.mSpiritTimer, 0.0f), 1f) * this.mSpectralAlpha;
      this.mAdditiveRenderData[(int) iDataChannel].mMaterial.Alpha = num4;
      this.mAdditiveRenderData[(int) iDataChannel].mMaterial.Colorize = Grimnir2.AdditiveRenderData.ColdColor;
      this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, (IRenderableAdditiveObject) this.mAdditiveRenderData[(int) iDataChannel]);
    }
    else
    {
      if (this.mLeftSpiritSpellEffect != null)
      {
        this.mLeftSpiritSpellEffect.Stop((ISpellCaster) this.mLeftSpiritBody);
        this.mLeftSpiritSpellEffect.DeInitialize((ISpellCaster) this.mLeftSpiritBody);
        this.mLeftSpiritSpellEffect = (SpellEffect) null;
      }
      if (this.mRightSpiritSpellEffect != null)
      {
        this.mRightSpiritSpellEffect.Stop((ISpellCaster) this.mRightSpiritBody);
        this.mRightSpiritSpellEffect.DeInitialize((ISpellCaster) this.mRightSpiritBody);
        this.mRightSpiritSpellEffect = (SpellEffect) null;
      }
    }
    if (this.mAssaturHeal)
    {
      if (this.mCurrentState != Grimnir2.HealState.Instance)
      {
        this.mCalledAppearTrigger = false;
        this.ChangeState(Grimnir2.States.Heal);
        this.ChangeSpiritState(Grimnir2.SpiritStates.Heal, 1);
        this.ChangeSpiritState(Grimnir2.SpiritStates.Heal, 2);
      }
      this.mAssaturAlpha = Math.Min(this.mAssaturAlpha + iDeltaTime * 0.25f, 1f);
      if ((double) this.mAssaturAlpha >= 1.0)
      {
        if (this.mLoopFight && !this.mCalledAppearTrigger)
        {
          this.mPlayState.Level.CurrentScene.ExecuteTrigger(Grimnir2.ASSATUR_APPEARS_TRIGGER, (Magicka.GameLogic.Entities.Character) null, false);
          this.mCalledAppearTrigger = true;
        }
        if (this.mAsaController.AnimationClip == this.mAsaClips[0] && !this.mAsaController.CrossFadeEnabled)
          this.mAsaController.CrossFade(this.mAsaClips[1], 0.5f, true);
      }
    }
    else
      this.mAssaturAlpha = Math.Max(this.mAssaturAlpha - iDeltaTime * 2f, 0.0f);
    result1 = this.mAssaturOrientation;
    result5 = this.mAssaturOrientation.Translation;
    result5.Y += (float) Math.Sin((double) this.mRightFloatCounter) * 0.5f;
    result1.Translation = result5;
    this.mAsaBoundingSphere.Center = result1.Translation;
    this.mAsaController.Update(iDeltaTime, ref result1, true);
    if (this.mCorporeal)
    {
      this.mAsaController.SkinnedBoneTransforms.CopyTo((Array) this.mAsaRenderData[(int) iDataChannel].mBones, 0);
      this.mAsaRenderData[(int) iDataChannel].mBoundingSphere = this.mAsaBoundingSphere;
      this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) this.mAsaRenderData[(int) iDataChannel]);
    }
    else
    {
      this.mAdditiveAsaRenderData[(int) iDataChannel].mBoundingSphere = this.mAsaBoundingSphere;
      this.mAdditiveAsaRenderData[(int) iDataChannel].mMaterial.Alpha = this.mAssaturAlpha * 0.25f;
      this.mAdditiveAsaRenderData[(int) iDataChannel].RenderLeftSpectral = true;
      this.mAdditiveAsaRenderData[(int) iDataChannel].RenderRightSpectral = false;
      this.mAdditiveAsaRenderData[(int) iDataChannel].mMaterial.Colorize = Grimnir2.AdditiveRenderData.ColdColor;
      this.mAsaController.SkinnedBoneTransforms.CopyTo((Array) this.mAdditiveAsaRenderData[(int) iDataChannel].mLeftSpecBones, 0);
      this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, (IRenderableAdditiveObject) this.mAdditiveAsaRenderData[(int) iDataChannel]);
    }
  }

  private IBossState<Grimnir2> GetState(Grimnir2.States iState)
  {
    switch (iState)
    {
      case Grimnir2.States.Idle:
        return (IBossState<Grimnir2>) Grimnir2.IdleState.Instance;
      case Grimnir2.States.Spirit:
        return (IBossState<Grimnir2>) Grimnir2.SpiritState.Instance;
      case Grimnir2.States.Magick:
        return (IBossState<Grimnir2>) Grimnir2.CastMagickState.Instance;
      case Grimnir2.States.Spell:
        return (IBossState<Grimnir2>) Grimnir2.CastSpellState.Instance;
      case Grimnir2.States.Heal:
        return (IBossState<Grimnir2>) Grimnir2.HealState.Instance;
      case Grimnir2.States.Die:
        return (IBossState<Grimnir2>) Grimnir2.DieState.Instance;
      default:
        return (IBossState<Grimnir2>) null;
    }
  }

  protected unsafe void ChangeState(Grimnir2.States iState)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    IBossState<Grimnir2> state = this.GetState(iState);
    if (state == null)
      return;
    if (NetworkManager.Instance.State == NetworkState.Server)
    {
      Grimnir2.ChangeStateMessage changeStateMessage;
      changeStateMessage.NewState = iState;
      BossFight.Instance.SendMessage<Grimnir2.ChangeStateMessage>((IBoss) this, (ushort) 2, (void*) &changeStateMessage, true);
    }
    this.mCurrentState.OnExit(this);
    this.mCurrentState = state;
    this.mCurrentState.OnEnter(this);
  }

  private Grimnir2.ISpiritState GetSpiritState(Grimnir2.SpiritStates iState)
  {
    switch (iState)
    {
      case Grimnir2.SpiritStates.Intro:
        return (Grimnir2.ISpiritState) Grimnir2.SpecIntroState.Instance;
      case Grimnir2.SpiritStates.Idle:
        return (Grimnir2.ISpiritState) Grimnir2.SpecIdleState.Instance;
      case Grimnir2.SpiritStates.Cast:
        return (Grimnir2.ISpiritState) Grimnir2.SpecCastState.Instance;
      case Grimnir2.SpiritStates.Heal:
        return (Grimnir2.ISpiritState) Grimnir2.SpecHealState.Instance;
      case Grimnir2.SpiritStates.Die:
        return (Grimnir2.ISpiritState) Grimnir2.SpecDieState.Instance;
      default:
        return (Grimnir2.ISpiritState) null;
    }
  }

  protected unsafe void ChangeSpiritState(Grimnir2.SpiritStates iState, int iIndex)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    Grimnir2.ISpiritState spiritState = this.GetSpiritState(iState);
    if (spiritState == null)
      return;
    if (NetworkManager.Instance.State == NetworkState.Server)
    {
      Grimnir2.ChangeSpiritStateMessage spiritStateMessage;
      spiritStateMessage.NewState = iState;
      spiritStateMessage.SpiritIndex = iIndex;
      BossFight.Instance.SendMessage<Grimnir2.ChangeSpiritStateMessage>((IBoss) this, (ushort) 3, (void*) &spiritStateMessage, true);
    }
    switch (iIndex)
    {
      case 2:
        this.mRightSpiritState.OnExit(this.mRightSpiritBody, this);
        this.mRightSpiritState = spiritState;
        this.mRightSpiritState.OnEnter(this.mRightSpiritBody, this);
        break;
      default:
        this.mLeftSpiritState.OnExit(this.mLeftSpiritBody, this);
        this.mLeftSpiritState = spiritState;
        this.mLeftSpiritState.OnEnter(this.mLeftSpiritBody, this);
        break;
    }
  }

  public void Nullify()
  {
    if ((double) this.mSpiritTimer > 25.0)
      return;
    this.mSpiritTimer = 24f;
    this.ChangeSpiritState(Grimnir2.SpiritStates.Idle, 1);
    if (!this.mUseBothSpirits)
      return;
    this.ChangeSpiritState(Grimnir2.SpiritStates.Idle, 2);
  }

  public unsafe void Corporealize()
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    if (NetworkManager.Instance.State == NetworkState.Server)
      BossFight.Instance.SendMessage<Grimnir2.CorpMessage>((IBoss) this, (ushort) 8, (void*) &new Grimnir2.CorpMessage(), true);
    if ((double) this.mSpiritTimer <= 25.0)
      this.mSpiritTimer = 24f;
    if (!this.mAssaturHeal)
      return;
    if (this.mPlayState.Level.CurrentScene.Triggers.TryGetValue(Grimnir2.ASSATUR_TALKS_TRIGGER, out Trigger _))
    {
      this.mPlayState.Level.CurrentScene.ExecuteTrigger(Grimnir2.ASSATUR_TALKS_TRIGGER, (Magicka.GameLogic.Entities.Character) null, false);
      this.ChangeState(Grimnir2.States.Die);
      this.ChangeSpiritState(Grimnir2.SpiritStates.Die, 1);
      this.ChangeSpiritState(Grimnir2.SpiritStates.Die, 2);
      this.mAssaturHeal = false;
      this.mCorporeal = true;
      this.mAsaController.CrossFade(this.mAsaClips[0], 1f, true);
      new Magick() { MagickType = MagickType.Nullify }.Effect.Execute((ISpellCaster) this.mGrimnirBody, this.mPlayState);
    }
    else
      this.mCorporeal = true;
  }

  protected override DamageResult Damage(
    Magicka.GameLogic.Damage iDamage,
    Magicka.GameLogic.Entities.Entity iAttacker,
    Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
    return base.Damage(iDamage, iAttacker, iAttackPosition, iFeatures);
  }

  public DamageResult Damage(
    int iPartIndex,
    Magicka.GameLogic.Damage iDamage,
    Magicka.GameLogic.Entities.Entity iAttacker,
    ref Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
    if (iPartIndex != 0 || this.mCurrentState is Grimnir2.DieState || this.mAssaturHeal)
      return DamageResult.None;
    DamageResult damageResult = this.Damage(iDamage, iAttacker, iAttackPosition, iFeatures);
    if ((damageResult & DamageResult.Hit) == DamageResult.Hit | (damageResult & DamageResult.Damaged) == DamageResult.Damaged)
    {
      this.mTimeSinceLastDamage = 0.0f;
      this.mDamageFlashTimer = 0.1f;
    }
    if ((double) this.mHitPoints < 3500.0)
    {
      this.mAssaturHeal = true;
      this.mHitPoints = 3500f;
    }
    return damageResult;
  }

  public void Damage(int iPartIndex, float iDamage, Elements iElement)
  {
    if (iPartIndex != 0)
      return;
    this.Damage(iDamage, iElement);
    if ((double) this.mHitPoints < 3500.0)
    {
      this.mAssaturHeal = true;
      this.mHitPoints = 3500f;
    }
    this.mTimeSinceLastStatusDamage = 0.0f;
    if ((double) this.mDamageFlashTimer > 0.0)
      return;
    this.mDamageFlashTimer = 0.1f;
  }

  public void AddSelfShield(int iIndex, Spell iSpell)
  {
  }

  public void RemoveSelfShield(int iIndex, Magicka.GameLogic.Entities.Character.SelfShieldType iType)
  {
  }

  CastType IBossSpellCaster.CastType(int iIndex)
  {
    switch (iIndex)
    {
      case 1:
        return this.mLeftSpiritCastType;
      case 2:
        return this.mRightSpiritCastType;
      default:
        return this.mGrimnirCastType;
    }
  }

  float IBossSpellCaster.SpellPower(int iIndex)
  {
    switch (iIndex)
    {
      case 1:
        return this.mLeftSpiritSpellPower;
      case 2:
        return this.mRightSpiritSpellPower;
      default:
        return this.mGrimnirSpellPower;
    }
  }

  void IBossSpellCaster.SpellPower(int iIndex, float iSpellPower)
  {
    switch (iIndex)
    {
      case 1:
        this.mLeftSpiritSpellPower = iSpellPower;
        break;
      case 2:
        this.mRightSpiritSpellPower = iSpellPower;
        break;
      default:
        this.mGrimnirSpellPower = iSpellPower;
        break;
    }
  }

  SpellEffect IBossSpellCaster.CurrentSpell(int iIndex)
  {
    switch (iIndex)
    {
      case 1:
        return this.mLeftSpiritSpellEffect;
      case 2:
        return this.mRightSpiritSpellEffect;
      default:
        return this.mGrimnirSpellEffect;
    }
  }

  void IBossSpellCaster.CurrentSpell(int iIndex, SpellEffect iEffect)
  {
    switch (iIndex)
    {
      case 1:
        this.mLeftSpiritSpellEffect = iEffect;
        break;
      case 2:
        this.mRightSpiritSpellEffect = iEffect;
        break;
      default:
        this.mGrimnirSpellEffect = iEffect;
        break;
    }
  }

  public void DeInitialize()
  {
  }

  public bool AddImpulseVelocity(
    Vector3 iDirection,
    float iElevation,
    float iMassPower,
    float iDistance)
  {
    return false;
  }

  public override bool Dead => false;

  public float MaxHitPoints => 35000f;

  public float HitPoints => this.mHitPoints;

  public void SetSlow(int iIndex)
  {
  }

  public void GetRandomPositionOnCollisionSkin(int iIndex, out Vector3 oPosition)
  {
    oPosition = new Vector3();
  }

  public bool HasStatus(int iIndex, StatusEffects iStatus) => false;

  public float StatusMagnitude(int iIndex, StatusEffects iStatus) => 0.0f;

  public unsafe void ScriptMessage(BossMessages iMessage)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    if (NetworkManager.Instance.State == NetworkState.Server)
    {
      Grimnir2.ScriptMessageMessage scriptMessageMessage;
      scriptMessageMessage.Message = iMessage;
      BossFight.Instance.SendMessage<Grimnir2.ScriptMessageMessage>((IBoss) this, (ushort) 1, (void*) &scriptMessageMessage, true);
    }
    switch (iMessage)
    {
      case BossMessages.CloneDone:
        this.mLoopFight = true;
        this.mCalledAppearTrigger = true;
        break;
      case BossMessages.GrimnirHurt:
        this.mAssaturHeal = true;
        this.mLoopFight = false;
        this.mAssaturAlpha = 1f;
        this.mHitPoints = 3500f;
        break;
      case BossMessages.FutureVladDone:
        if (this.mCorporeal)
          break;
        this.mLoopFight = true;
        this.mCalledAppearTrigger = false;
        this.mUseBothSpirits = true;
        this.mHitPoints = 35000f;
        this.mAsaController.CrossFade(this.mAsaClips[0], 0.5f, true);
        this.mAssaturHeal = false;
        Matrix mOrientation = this.mOrientation;
        Vector3 translation = this.mOrientation.Translation;
        translation.Y += 1.5f;
        mOrientation.Translation = translation;
        EffectManager.Instance.StartEffect(Grimnir2.ASSATUR_HEAL_EFFECT, ref mOrientation, out VisualEffectReference _);
        AudioManager.Instance.PlayCue(Grimnir2.ASSATUR_HEAL_SOUND.Value, Grimnir2.ASSATUR_HEAL_SOUND.Key, this.mGrimnirBody.AudioEmitter);
        break;
      case BossMessages.KillGrimnir:
        this.mController.CrossFade(this.mClips[0], 0.25f, false);
        break;
      case BossMessages.Assatur_Cut:
        this.mAsaController.CrossFade(this.mAsaClips[2], 0.25f, false);
        break;
    }
  }

  protected override BossDamageZone Entity => (BossDamageZone) this.mGrimnirBody;

  protected override float Radius => this.mGrimnirBody.Radius;

  protected override float Length => 2f;

  protected override int BloodEffect => Grimnir2.BLOOD_EFFECT;

  protected override Vector3 NotifierTextPostion
  {
    get
    {
      Vector3 result = this.mOrientation.Translation;
      Vector3 vector3 = new Vector3(0.0f, 4f, 0.0f);
      Vector3.Add(ref result, ref vector3, out result);
      return result;
    }
  }

  private unsafe void NetworkUpdate()
  {
    if (!(NetworkManager.Instance.Interface is NetworkServer networkServer))
      return;
    Grimnir2.UpdateMessage updateMessage1 = new Grimnir2.UpdateMessage();
    updateMessage1.Animation = (byte) 0;
    while ((int) updateMessage1.Animation < this.mClips.Length && this.mController.AnimationClip != this.mClips[(int) updateMessage1.Animation])
      ++updateMessage1.Animation;
    updateMessage1.AnimationTime = this.mController.Time;
    updateMessage1.Hitpoints = this.mHitPoints;
    updateMessage1.LeftOrientation = this.mLeftSpecOrientation;
    updateMessage1.RightOrientation = this.mRightSpecOrientation;
    updateMessage1.LeftFloatCounter = this.mLeftFloatCounter;
    updateMessage1.RightFloatCounter = this.mRightFloatCounter;
    updateMessage1.SpiritTimer = this.mSpiritTimer;
    for (int index = 0; index < networkServer.Connections; ++index)
    {
      float num = networkServer.GetLatency(index) * 0.5f;
      Grimnir2.UpdateMessage updateMessage2 = updateMessage1;
      updateMessage2.AnimationTime += num;
      updateMessage2.SpiritTimer += num;
      BossFight.Instance.SendMessage<Grimnir2.UpdateMessage>((IBoss) this, (ushort) 0, (void*) &updateMessage1, false, index);
    }
  }

  public unsafe void NetworkUpdate(ref BossUpdateMessage iMsg)
  {
    if (iMsg.Type == (ushort) 0)
    {
      if ((double) iMsg.TimeStamp < (double) this.mLastNetworkUpdate)
        return;
      this.mLastNetworkUpdate = (float) iMsg.TimeStamp;
      Grimnir2.UpdateMessage updateMessage;
      BossUpdateMessage.ConvertTo(ref iMsg, (void*) &updateMessage);
      if (this.mController.AnimationClip != this.mClips[(int) updateMessage.Animation])
        this.mController.StartClip(this.mClips[(int) updateMessage.Animation], false);
      this.mLeftFloatCounter = updateMessage.LeftFloatCounter;
      this.mRightFloatCounter = updateMessage.RightFloatCounter;
      this.mLeftSpecOrientation = updateMessage.LeftOrientation;
      this.mRightSpecOrientation = updateMessage.RightOrientation;
      this.mSpiritTimer = updateMessage.SpiritTimer;
      this.mController.Time = updateMessage.AnimationTime;
      this.mHitPoints = updateMessage.Hitpoints;
    }
    else if (iMsg.Type == (ushort) 6)
    {
      Grimnir2.CastMagickMessage castMagickMessage;
      BossUpdateMessage.ConvertTo(ref iMsg, (void*) &castMagickMessage);
      new Magick() { MagickType = castMagickMessage.Magick }.Effect.Execute((ISpellCaster) this.mGrimnirBody, this.mPlayState);
    }
    else if (iMsg.Type == (ushort) 5)
    {
      Grimnir2.CastSpellMessage castSpellMessage;
      BossUpdateMessage.ConvertTo(ref iMsg, (void*) &castSpellMessage);
      if (castSpellMessage.SpiritCast)
      {
        Grimnir2.SpellData spellData = Grimnir2.SPIRITSPELLS[castSpellMessage.SpellIndex];
        if (castSpellMessage.SpiritIndex == 1)
        {
          this.mLeftSpiritCastType = spellData.CASTTYPE;
          this.mLeftSpiritSpellPower = spellData.SPELLPOWER;
          spellData.SPELL.Cast(true, (ISpellCaster) this.mLeftSpiritBody, this.mLeftSpiritCastType);
        }
        else
        {
          if (castSpellMessage.SpiritIndex != 2)
            return;
          this.mRightSpiritCastType = spellData.CASTTYPE;
          this.mRightSpiritSpellPower = spellData.SPELLPOWER;
          spellData.SPELL.Cast(true, (ISpellCaster) this.mRightSpiritBody, this.mRightSpiritCastType);
        }
      }
      else
      {
        Grimnir2.SpellData spellData = Grimnir2.GRIMNIRSPELLS[castSpellMessage.SpellIndex];
        this.mGrimnirCastType = spellData.CASTTYPE;
        this.mGrimnirSpellPower = spellData.SPELLPOWER;
        spellData.SPELL.Cast(true, (ISpellCaster) this.mGrimnirBody, this.mGrimnirCastType);
      }
    }
    else if (iMsg.Type == (ushort) 8)
    {
      Grimnir2.CorpMessage corpMessage;
      BossUpdateMessage.ConvertTo(ref iMsg, (void*) &corpMessage);
      if ((double) this.mSpiritTimer <= 25.0)
        this.mSpiritTimer = 24f;
      if (!this.mAssaturHeal)
        return;
      this.mAssaturHeal = false;
      this.mCorporeal = true;
      this.mAsaController.CrossFade(this.mAsaClips[0], 1f, true);
      new Magick() { MagickType = MagickType.Nullify }.Effect.Execute((ISpellCaster) this.mGrimnirBody, this.mPlayState);
    }
    else if (iMsg.Type == (ushort) 1)
    {
      Grimnir2.ScriptMessageMessage scriptMessageMessage;
      BossUpdateMessage.ConvertTo(ref iMsg, (void*) &scriptMessageMessage);
      switch (scriptMessageMessage.Message)
      {
        case BossMessages.CloneDone:
          this.mLoopFight = true;
          this.mCalledAppearTrigger = true;
          break;
        case BossMessages.GrimnirHurt:
          this.mAssaturHeal = true;
          this.mLoopFight = false;
          this.mAssaturAlpha = 1f;
          this.mHitPoints = 3500f;
          break;
        case BossMessages.FutureVladDone:
          if (this.mCorporeal)
            break;
          this.mLoopFight = true;
          this.mCalledAppearTrigger = false;
          this.mUseBothSpirits = true;
          this.mHitPoints = 35000f;
          this.mAsaController.CrossFade(this.mAsaClips[0], 0.5f, true);
          this.mAssaturHeal = false;
          Matrix mOrientation = this.mOrientation;
          Vector3 translation = this.mOrientation.Translation;
          translation.Y += 1.5f;
          mOrientation.Translation = translation;
          EffectManager.Instance.StartEffect(Grimnir2.ASSATUR_HEAL_EFFECT, ref mOrientation, out VisualEffectReference _);
          AudioManager.Instance.PlayCue(Grimnir2.ASSATUR_HEAL_SOUND.Value, Grimnir2.ASSATUR_HEAL_SOUND.Key, this.mGrimnirBody.AudioEmitter);
          break;
        case BossMessages.KillGrimnir:
          this.mController.CrossFade(this.mClips[0], 0.25f, false);
          break;
        case BossMessages.Assatur_Cut:
          this.mAsaController.CrossFade(this.mAsaClips[2], 0.25f, false);
          break;
        default:
          throw new Exception($"Incorrect message, {(object) scriptMessageMessage.Message} passed to {this.GetType().Name}");
      }
    }
    else if (iMsg.Type == (ushort) 7)
    {
      Grimnir2.ChangeTargetMessage changeTargetMessage;
      BossUpdateMessage.ConvertTo(ref iMsg, (void*) &changeTargetMessage);
      if (changeTargetMessage.Target == ushort.MaxValue)
        this.mTarget = (Magicka.GameLogic.Entities.Character) null;
      else
        this.mTarget = Magicka.GameLogic.Entities.Entity.GetFromHandle((int) changeTargetMessage.Target) as Magicka.GameLogic.Entities.Character;
    }
    else if (iMsg.Type == (ushort) 2)
    {
      Grimnir2.ChangeStateMessage changeStateMessage;
      BossUpdateMessage.ConvertTo(ref iMsg, (void*) &changeStateMessage);
      this.mCurrentState.OnExit(this);
      this.mCurrentState = this.GetState(changeStateMessage.NewState);
      this.mCurrentState.OnEnter(this);
    }
    else
    {
      if (iMsg.Type != (ushort) 3)
        return;
      Grimnir2.ChangeSpiritStateMessage spiritStateMessage;
      BossUpdateMessage.ConvertTo(ref iMsg, (void*) &spiritStateMessage);
      switch (spiritStateMessage.SpiritIndex)
      {
        case 2:
          this.mRightSpiritState.OnExit(this.mRightSpiritBody, this);
          this.mRightSpiritState = this.GetSpiritState(spiritStateMessage.NewState);
          this.mRightSpiritState.OnEnter(this.mRightSpiritBody, this);
          break;
        default:
          this.mLeftSpiritState.OnExit(this.mLeftSpiritBody, this);
          this.mLeftSpiritState = this.GetSpiritState(spiritStateMessage.NewState);
          this.mLeftSpiritState.OnEnter(this.mLeftSpiritBody, this);
          break;
      }
    }
  }

  public void NetworkInitialize(ref BossInitializeMessage iMsg)
  {
    throw new NotImplementedException();
  }

  public BossEnum GetBossType() => BossEnum.Grimnir2;

  public bool NetworkInitialized => true;

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

  protected class RenderData : 
    RenderableObject<SkinnedModelDeferredEffect, SkinnedModelDeferredAdvancedMaterial>
  {
    public float Flash;
    public Matrix[] mBones;

    public RenderData() => this.mBones = new Matrix[80 /*0x50*/];

    public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      SkinnedModelDeferredEffect modelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
      modelDeferredEffect.OverrideColor = new Vector4(1f, 1f, 1f, this.Flash);
      modelDeferredEffect.Bones = this.mBones;
      base.Draw(iEffect, iViewFrustum);
      modelDeferredEffect.OverrideColor = Vector4.Zero;
    }

    public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      (iEffect as SkinnedModelDeferredEffect).Bones = this.mBones;
      base.DrawShadow(iEffect, iViewFrustum);
    }
  }

  protected class AdditiveRenderData : 
    RenderableAdditiveObject<SkinnedModelDeferredEffect, SkinnedModelDeferredAdvancedMaterial>
  {
    public static readonly Vector4 ColdColor = new Vector4(1f, 1.6f, 2f, 1f);
    public Matrix[] mLeftSpecBones;
    public Matrix[] mRightSpecBones;
    public bool RenderLeftSpectral;
    public bool RenderRightSpectral;

    public AdditiveRenderData()
    {
      this.mRightSpecBones = new Matrix[80 /*0x50*/];
      this.mLeftSpecBones = new Matrix[80 /*0x50*/];
    }

    public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      if (this.RenderLeftSpectral)
      {
        SkinnedModelDeferredEffect modelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
        modelDeferredEffect.Bones = this.mLeftSpecBones;
        modelDeferredEffect.OverrideColor = new Vector4(1f, 1f, 1f, 0.0f);
        base.Draw(iEffect, iViewFrustum);
        modelDeferredEffect.Colorize = Vector4.Zero;
      }
      if (!this.RenderRightSpectral)
        return;
      SkinnedModelDeferredEffect modelDeferredEffect1 = iEffect as SkinnedModelDeferredEffect;
      modelDeferredEffect1.Bones = this.mRightSpecBones;
      modelDeferredEffect1.OverrideColor = new Vector4(1f, 1f, 1f, 0.0f);
      base.Draw(iEffect, iViewFrustum);
      modelDeferredEffect1.Colorize = Vector4.Zero;
    }
  }

  protected class IntroState : IBossState<Grimnir2>
  {
    private static Grimnir2.IntroState sSingelton;
    private static volatile object sSingeltonLock = new object();

    public static Grimnir2.IntroState Instance
    {
      get
      {
        if (Grimnir2.IntroState.sSingelton == null)
        {
          lock (Grimnir2.IntroState.sSingeltonLock)
          {
            if (Grimnir2.IntroState.sSingelton == null)
              Grimnir2.IntroState.sSingelton = new Grimnir2.IntroState();
          }
        }
        return Grimnir2.IntroState.sSingelton;
      }
    }

    public void OnEnter(Grimnir2 iOwner) => iOwner.mController.StartClip(iOwner.mClips[6], false);

    public void OnUpdate(float iDeltaTime, Grimnir2 iOwner)
    {
    }

    public void OnExit(Grimnir2 iOwner)
    {
    }
  }

  protected class IdleState : IBossState<Grimnir2>
  {
    private static Grimnir2.IdleState sSingelton;
    private static volatile object sSingeltonLock = new object();

    public static Grimnir2.IdleState Instance
    {
      get
      {
        if (Grimnir2.IdleState.sSingelton == null)
        {
          lock (Grimnir2.IdleState.sSingeltonLock)
          {
            if (Grimnir2.IdleState.sSingelton == null)
              Grimnir2.IdleState.sSingelton = new Grimnir2.IdleState();
          }
        }
        return Grimnir2.IdleState.sSingelton;
      }
    }

    private IdleState()
    {
    }

    public void OnEnter(Grimnir2 iOwner)
    {
      iOwner.mController.CrossFade(iOwner.mClips[6], 0.5f, true);
      iOwner.mGrimnirIdleTimer = (float) (2.0 + Grimnir2.RANDOM.NextDouble() * 2.0);
    }

    public void OnUpdate(float iDeltaTime, Grimnir2 iOwner)
    {
      if ((double) iOwner.mTimeSinceLastDamage < 1.0)
        iOwner.mGrimnirIdleTimer = 0.0f;
      if ((double) iOwner.mGrimnirIdleTimer <= 0.0)
      {
        float num = Math.Max((float) (((double) iOwner.mSpiritTimer - 25.0) / 25.0), 0.0f);
        if (Grimnir2.RANDOM.NextDouble() <= (double) num)
          iOwner.ChangeState(Grimnir2.States.Spirit);
        else if (Grimnir2.RANDOM.NextDouble() < (double) Math.Min(iOwner.mTimeSinceLastDamage, 0.75f))
          iOwner.ChangeState(Grimnir2.States.Spell);
        else
          iOwner.ChangeState(Grimnir2.States.Magick);
      }
      iOwner.mGrimnirIdleTimer -= iDeltaTime;
    }

    public void OnExit(Grimnir2 iOwner)
    {
    }
  }

  protected class SpiritState : IBossState<Grimnir2>
  {
    private static Grimnir2.SpiritState sSingelton;
    private static volatile object sSingeltonLock = new object();

    public static Grimnir2.SpiritState Instance
    {
      get
      {
        if (Grimnir2.SpiritState.sSingelton == null)
        {
          lock (Grimnir2.SpiritState.sSingeltonLock)
          {
            if (Grimnir2.SpiritState.sSingelton == null)
              Grimnir2.SpiritState.sSingelton = new Grimnir2.SpiritState();
          }
        }
        return Grimnir2.SpiritState.sSingelton;
      }
    }

    public void OnEnter(Grimnir2 iOwner)
    {
      iOwner.mController.CrossFade(iOwner.mClips[13], 0.25f, false);
    }

    public void OnUpdate(float iDeltaTime, Grimnir2 iOwner)
    {
      if (!iOwner.mController.HasFinished || iOwner.mController.CrossFadeEnabled)
        return;
      iOwner.mSpiritTimer = 0.0f;
      iOwner.mRightFloatCounter = 0.0f;
      iOwner.mLeftFloatCounter = 0.0f;
      iOwner.ChangeState(Grimnir2.States.Idle);
    }

    public void OnExit(Grimnir2 iOwner)
    {
    }
  }

  protected class CastMagickState : IBossState<Grimnir2>
  {
    private static Grimnir2.CastMagickState sSingelton;
    private static volatile object sSingeltonLock = new object();

    public static Grimnir2.CastMagickState Instance
    {
      get
      {
        if (Grimnir2.CastMagickState.sSingelton == null)
        {
          lock (Grimnir2.CastMagickState.sSingeltonLock)
          {
            if (Grimnir2.CastMagickState.sSingelton == null)
              Grimnir2.CastMagickState.sSingelton = new Grimnir2.CastMagickState();
          }
        }
        return Grimnir2.CastMagickState.sSingelton;
      }
    }

    private CastMagickState()
    {
    }

    public void OnEnter(Grimnir2 iOwner)
    {
      iOwner.mGrimnirIdleTimer = 2f;
      if (NetworkManager.Instance.State == NetworkState.Client)
        return;
      float tornadoWeight = this.GetTornadoWeight(iOwner.mLastMagick, iOwner.mPlayState);
      float rainWeight = this.GetRainWeight(iOwner.mLastMagick);
      float conflagrationWeight = this.GetConflagrationWeight(iOwner.mLastMagick);
      float thunderBweight = this.GetThunderBWeight(iOwner.mLastMagick);
      float num1 = tornadoWeight + rainWeight + conflagrationWeight + thunderBweight;
      float num2 = tornadoWeight / num1;
      float num3 = rainWeight / num1;
      float num4 = conflagrationWeight / num1;
      float num5 = thunderBweight / num1;
      Magick magick = new Magick();
      Matrix mOrientation = iOwner.mOrientation;
      Vector3 translation = mOrientation.Translation;
      translation.Y += iOwner.Radius * 2f;
      translation.Z += iOwner.Radius * 0.5f;
      mOrientation.Translation = translation;
      if ((double) num2 > (double) num3 && (double) num2 > (double) num4 && (double) num2 > (double) num5)
      {
        iOwner.mController.CrossFade(iOwner.mClips[11], 0.2f, true);
        magick.MagickType = MagickType.Tornado;
        EffectManager.Instance.StartEffect(Grimnir2.GENERIC_MAGICK, ref mOrientation, out iOwner.mCastEffect);
      }
      else if ((double) num3 > (double) num4 && (double) num3 > (double) num5)
      {
        iOwner.mController.CrossFade(iOwner.mClips[11], 0.2f, true);
        magick.MagickType = MagickType.Rain;
        EffectManager.Instance.StartEffect(Grimnir2.GENERIC_MAGICK, ref mOrientation, out iOwner.mCastEffect);
      }
      else if ((double) num4 > (double) num5)
      {
        iOwner.mController.CrossFade(iOwner.mClips[12], 0.25f, true);
        magick.MagickType = MagickType.Conflagration;
        EffectManager.Instance.StartEffect(Grimnir2.GENERIC_MAGICK, ref mOrientation, out iOwner.mCastEffect);
      }
      else
      {
        iOwner.mController.CrossFade(iOwner.mClips[12], 0.2f, true);
        magick.MagickType = MagickType.ThunderB;
        EffectManager.Instance.StartEffect(Grimnir2.GENERIC_MAGICK, ref mOrientation, out iOwner.mCastEffect);
      }
      iOwner.mLastMagick = magick.MagickType;
    }

    public void OnUpdate(float iDeltaTime, Grimnir2 iOwner)
    {
      if (NetworkManager.Instance.State == NetworkState.Client)
        return;
      if ((double) iOwner.mGrimnirIdleTimer <= 0.0)
        iOwner.ChangeState(Grimnir2.States.Idle);
      iOwner.mGrimnirIdleTimer -= iDeltaTime;
    }

    public unsafe void OnExit(Grimnir2 iOwner)
    {
      EffectManager.Instance.Stop(ref iOwner.mCastEffect);
      if (NetworkManager.Instance.State == NetworkState.Client)
        return;
      Magick magick = new Magick();
      magick.MagickType = iOwner.mLastMagick;
      if (NetworkManager.Instance.State == NetworkState.Server)
      {
        Grimnir2.CastMagickMessage castMagickMessage;
        castMagickMessage.Magick = magick.MagickType;
        BossFight.Instance.SendMessage<Grimnir2.CastMagickMessage>((IBoss) iOwner, (ushort) 6, (void*) &castMagickMessage, true);
      }
      magick.Effect.Execute((ISpellCaster) iOwner.mGrimnirBody, iOwner.mPlayState);
    }

    private float GetTornadoWeight(MagickType iLastMagick, PlayState iPlayState)
    {
      if (iLastMagick == MagickType.Blizzard)
        return 0.0f;
      float tornadoWeight = (float) Grimnir2.RANDOM.NextDouble();
      StaticList<Magicka.GameLogic.Entities.Entity> entities = iPlayState.EntityManager.Entities;
      for (int iIndex = 0; iIndex < entities.Count; ++iIndex)
      {
        if (entities[iIndex] is TornadoEntity)
          return 0.0f;
      }
      return tornadoWeight;
    }

    private float GetRainWeight(MagickType iLastMagick)
    {
      if (iLastMagick == MagickType.Blizzard)
        return 0.0f;
      float rainWeight = Rain.Instance.IsDead ? (float) Grimnir2.RANDOM.NextDouble() : 0.0f;
      Player[] players = Magicka.Game.Instance.Players;
      for (int index = 0; index < players.Length; ++index)
      {
        if (players[index].Playing && players[index].Avatar != null && players[index].Avatar.HasStatus(StatusEffects.Wet))
          return 0.0f;
      }
      return rainWeight;
    }

    private float GetConflagrationWeight(MagickType iLastMagick)
    {
      float conflagrationWeight = 0.0f;
      if (iLastMagick == MagickType.Blizzard || SpellManager.Instance.IsEffectActive(typeof (Conflagration)))
        return 0.0f;
      if (Blizzard.Instance.IsDead)
        conflagrationWeight = (float) Grimnir2.RANDOM.NextDouble();
      return conflagrationWeight;
    }

    private float GetThunderBWeight(MagickType iLastMagick)
    {
      if (iLastMagick == MagickType.Blizzard)
        return 0.0f;
      float thunderBweight = 0.0f;
      Player[] players = Magicka.Game.Instance.Players;
      for (int index = 0; index < players.Length; ++index)
      {
        if (players[index].Playing && players[index].Avatar != null && players[index].Avatar.HasStatus(StatusEffects.Wet))
          thunderBweight += 1f / (float) Magicka.Game.Instance.PlayerCount;
      }
      return thunderBweight;
    }
  }

  protected class CastSpellState : IBossState<Grimnir2>
  {
    private static Grimnir2.CastSpellState sSingelton;
    private static volatile object sSingeltonLock = new object();

    public static Grimnir2.CastSpellState Instance
    {
      get
      {
        if (Grimnir2.CastSpellState.sSingelton == null)
        {
          lock (Grimnir2.CastSpellState.sSingeltonLock)
          {
            if (Grimnir2.CastSpellState.sSingelton == null)
              Grimnir2.CastSpellState.sSingelton = new Grimnir2.CastSpellState();
          }
        }
        return Grimnir2.CastSpellState.sSingelton;
      }
    }

    private CastSpellState()
    {
    }

    public void OnEnter(Grimnir2 iOwner)
    {
      iOwner.mController.CrossFade(iOwner.mClips[14], 0.25f, false);
      if (NetworkManager.Instance.State == NetworkState.Client)
        return;
      float num = float.MaxValue;
      Player[] players = Magicka.Game.Instance.Players;
      for (int index = 0; index < players.Length; ++index)
      {
        if (players[index].Playing && players[index].Avatar != null)
        {
          Vector3 translation = iOwner.mOrientation.Translation;
          Vector3 position = players[index].Avatar.Position;
          float result;
          Vector3.DistanceSquared(ref translation, ref position, out result);
          if ((double) result < (double) num)
            num = result;
        }
      }
      if ((double) iOwner.mTimeSinceLastDamage < 1.0)
        iOwner.mGrimnirLastSpell = Grimnir2.RANDOM.NextDouble() > 0.5 ? 1 : 2;
      else if ((double) num < 25.0)
        iOwner.mGrimnirLastSpell = 0;
      else if (Grimnir2.RANDOM.NextDouble() > 0.5)
        iOwner.mGrimnirLastSpell = 2;
      else
        iOwner.mGrimnirLastSpell = 1;
    }

    public unsafe void OnUpdate(float iDeltaTime, Grimnir2 iOwner)
    {
      if (NetworkManager.Instance.State == NetworkState.Client || !iOwner.mController.HasFinished || iOwner.mController.CrossFadeEnabled)
        return;
      Grimnir2.SpellData spellData = Grimnir2.GRIMNIRSPELLS[iOwner.mGrimnirLastSpell];
      if (NetworkManager.Instance.State == NetworkState.Server)
      {
        Grimnir2.CastSpellMessage castSpellMessage;
        castSpellMessage.SpiritCast = false;
        castSpellMessage.SpiritIndex = 0;
        castSpellMessage.SpellIndex = iOwner.mGrimnirLastSpell;
        BossFight.Instance.SendMessage<Grimnir2.CastSpellMessage>((IBoss) iOwner, (ushort) 5, (void*) &castSpellMessage, true);
      }
      iOwner.mGrimnirCastType = spellData.CASTTYPE;
      iOwner.mGrimnirSpellPower = spellData.SPELLPOWER;
      spellData.SPELL.Cast(true, (ISpellCaster) iOwner.mGrimnirBody, iOwner.mGrimnirCastType);
      iOwner.ChangeState(Grimnir2.States.Idle);
    }

    public void OnExit(Grimnir2 iOwner)
    {
      if (iOwner.mGrimnirSpellEffect == null)
        return;
      iOwner.mGrimnirSpellEffect.DeInitialize((ISpellCaster) iOwner.mGrimnirBody);
    }
  }

  protected class HealState : IBossState<Grimnir2>
  {
    private static Grimnir2.HealState sSingelton;
    private static volatile object sSingeltonLock = new object();

    public static Grimnir2.HealState Instance
    {
      get
      {
        if (Grimnir2.HealState.sSingelton == null)
        {
          lock (Grimnir2.HealState.sSingeltonLock)
          {
            if (Grimnir2.HealState.sSingelton == null)
              Grimnir2.HealState.sSingelton = new Grimnir2.HealState();
          }
        }
        return Grimnir2.HealState.sSingelton;
      }
    }

    private HealState()
    {
    }

    public void OnEnter(Grimnir2 iOwner)
    {
      iOwner.mController.CrossFade(iOwner.mClips[6], 0.25f, true);
      if (NetworkManager.Instance.State == NetworkState.Client)
        return;
      Vector3 iBias = new Vector3(0.0f, 4f, -7f);
      iOwner.mPlayState.Camera.SetBias(ref iBias, 3f);
    }

    public void OnUpdate(float iDeltaTime, Grimnir2 iOwner)
    {
      if (iOwner.mAssaturHeal)
        return;
      iOwner.ChangeState(Grimnir2.States.Idle);
    }

    public void OnExit(Grimnir2 iOwner)
    {
    }
  }

  protected class DieState : IBossState<Grimnir2>
  {
    private static Grimnir2.DieState sSingelton;
    private static volatile object sSingeltonLock = new object();

    public static Grimnir2.DieState Instance
    {
      get
      {
        if (Grimnir2.DieState.sSingelton == null)
        {
          lock (Grimnir2.DieState.sSingeltonLock)
          {
            if (Grimnir2.DieState.sSingelton == null)
              Grimnir2.DieState.sSingelton = new Grimnir2.DieState();
          }
        }
        return Grimnir2.DieState.sSingelton;
      }
    }

    private DieState()
    {
    }

    public void OnEnter(Grimnir2 iOwner)
    {
      iOwner.mGrimnirBody.IsEthereal = true;
      iOwner.mHitPoints = 0.0f;
      iOwner.mController.CrossFade(iOwner.mClips[6], 0.25f, false);
    }

    public void OnUpdate(float iDeltaTime, Grimnir2 iOwner)
    {
    }

    public void OnExit(Grimnir2 iOwner)
    {
    }
  }

  public interface ISpiritState
  {
    void OnEnter(BossSpellCasterZone iZone, Grimnir2 iOwner);

    void OnUpdate(float iDeltaTime, BossSpellCasterZone iZone, Grimnir2 iOwner);

    void OnExit(BossSpellCasterZone iZone, Grimnir2 iOwner);
  }

  protected class SpecIntroState : Grimnir2.ISpiritState
  {
    private static Grimnir2.SpecIntroState sSingelton;
    private static volatile object sSingeltonLock = new object();

    public static Grimnir2.SpecIntroState Instance
    {
      get
      {
        if (Grimnir2.SpecIntroState.sSingelton == null)
        {
          lock (Grimnir2.SpecIntroState.sSingeltonLock)
          {
            if (Grimnir2.SpecIntroState.sSingelton == null)
              Grimnir2.SpecIntroState.sSingelton = new Grimnir2.SpecIntroState();
          }
        }
        return Grimnir2.SpecIntroState.sSingelton;
      }
    }

    private SpecIntroState()
    {
    }

    public void OnEnter(BossSpellCasterZone iZone, Grimnir2 iOwner)
    {
      iOwner.mSpectralTargetAlpha = 0.0f;
      iOwner.mLeftSpiritController.StartClip(iOwner.mClips[7], true);
      iOwner.mRightSpiritController.StartClip(iOwner.mClips[7], true);
    }

    public void OnUpdate(float iDeltaTime, BossSpellCasterZone iZone, Grimnir2 iOwner)
    {
    }

    public void OnExit(BossSpellCasterZone iZone, Grimnir2 iOwner)
    {
    }
  }

  protected class SpecIdleState : Grimnir2.ISpiritState
  {
    private static Grimnir2.SpecIdleState sSingelton;
    private static volatile object sSingeltonLock = new object();

    public static Grimnir2.SpecIdleState Instance
    {
      get
      {
        if (Grimnir2.SpecIdleState.sSingelton == null)
        {
          lock (Grimnir2.SpecIdleState.sSingeltonLock)
          {
            if (Grimnir2.SpecIdleState.sSingelton == null)
              Grimnir2.SpecIdleState.sSingelton = new Grimnir2.SpecIdleState();
          }
        }
        return Grimnir2.SpecIdleState.sSingelton;
      }
    }

    private SpecIdleState()
    {
    }

    public void OnEnter(BossSpellCasterZone iZone, Grimnir2 iOwner)
    {
      if (iZone.Index == 1)
        iOwner.mLeftSpiritIdleTimer = (float) (Grimnir2.RANDOM.NextDouble() * 2.0 + 2.0);
      else
        iOwner.mRightSpiritIdleTimer = (float) (Grimnir2.RANDOM.NextDouble() * 2.0 + 2.0);
      iOwner.mSpectralTargetAlpha = 1f;
      iZone.AnimationController.CrossFade(iOwner.mClips[7], 0.25f, true);
    }

    public void OnUpdate(float iDeltaTime, BossSpellCasterZone iZone, Grimnir2 iOwner)
    {
      if (iZone.Index == 1)
      {
        iOwner.mLeftSpiritIdleTimer -= iDeltaTime;
        if ((double) iOwner.mLeftSpiritIdleTimer > 0.0 || iOwner.mLeftSpiritSpellEffect != null)
          return;
        iOwner.ChangeSpiritState(Grimnir2.SpiritStates.Cast, 1);
      }
      else
      {
        iOwner.mRightSpiritIdleTimer -= iDeltaTime;
        if ((double) iOwner.mRightSpiritIdleTimer > 0.0 || iOwner.mRightSpiritSpellEffect != null)
          return;
        iOwner.ChangeSpiritState(Grimnir2.SpiritStates.Cast, 2);
      }
    }

    public void OnExit(BossSpellCasterZone iZone, Grimnir2 iOwner)
    {
    }
  }

  protected class SpecCastState : Grimnir2.ISpiritState
  {
    private static Grimnir2.SpecCastState sSingelton;
    private static volatile object sSingeltonLock = new object();

    public static Grimnir2.SpecCastState Instance
    {
      get
      {
        if (Grimnir2.SpecCastState.sSingelton == null)
        {
          lock (Grimnir2.SpecCastState.sSingeltonLock)
          {
            if (Grimnir2.SpecCastState.sSingelton == null)
              Grimnir2.SpecCastState.sSingelton = new Grimnir2.SpecCastState();
          }
        }
        return Grimnir2.SpecCastState.sSingelton;
      }
    }

    private SpecCastState()
    {
    }

    public void OnEnter(BossSpellCasterZone iZone, Grimnir2 iOwner)
    {
      if (iZone.Index == 1)
      {
        int num;
        do
        {
          num = Grimnir2.RANDOM.Next(Grimnir2.SPIRITSPELLS.Length);
        }
        while (num == iOwner.mLeftSpiritLastSpell);
        iOwner.mLeftSpiritHasCastSpell = false;
        iOwner.mLeftSpiritLastSpell = num;
        iOwner.mLeftCastAnimation = num < 1 ? Grimnir2.Animations.spirit_cast_railgrun : Grimnir2.Animations.spirit_cast_projectile;
        iZone.AnimationController.CrossFade(iOwner.mClips[(int) iOwner.mLeftCastAnimation], 0.15f, false);
      }
      else
      {
        int num;
        do
        {
          num = Grimnir2.RANDOM.Next(Grimnir2.SPIRITSPELLS.Length);
        }
        while (num == iOwner.mRightSpiritLastSpell);
        iOwner.mRightSpiritHasCastSpell = false;
        iOwner.mRightSpiritLastSpell = num;
        iOwner.mRightCastAnimation = num < 1 ? Grimnir2.Animations.spirit_cast_railgrun : Grimnir2.Animations.spirit_cast_projectile;
        iZone.AnimationController.CrossFade(iOwner.mClips[(int) iOwner.mRightCastAnimation], 0.15f, false);
      }
    }

    public unsafe void OnUpdate(float iDeltaTime, BossSpellCasterZone iZone, Grimnir2 iOwner)
    {
      if (NetworkManager.Instance.State == NetworkState.Client)
        return;
      float num = iZone.AnimationController.Time / iZone.AnimationController.AnimationClip.Duration;
      if (iZone.AnimationController.CrossFadeEnabled)
        return;
      switch (iZone.Index)
      {
        case 2:
          if (!iOwner.mRightSpiritHasCastSpell && (iOwner.mRightCastAnimation == Grimnir2.Animations.spirit_cast_projectile && (double) num >= 0.60000002384185791 || iOwner.mRightCastAnimation == Grimnir2.Animations.spirit_cast_railgrun && (double) num >= 0.15000000596046448))
          {
            Grimnir2.SpellData spellData = Grimnir2.SPIRITSPELLS[iOwner.mRightSpiritLastSpell];
            if (NetworkManager.Instance.State == NetworkState.Server)
            {
              Grimnir2.CastSpellMessage castSpellMessage;
              castSpellMessage.SpiritCast = true;
              castSpellMessage.SpiritIndex = 2;
              castSpellMessage.SpellIndex = iOwner.mRightSpiritLastSpell;
              BossFight.Instance.SendMessage<Grimnir2.CastSpellMessage>((IBoss) iOwner, (ushort) 5, (void*) &castSpellMessage, true);
            }
            iOwner.mRightSpiritCastType = spellData.CASTTYPE;
            iOwner.mRightSpiritSpellPower = spellData.SPELLPOWER;
            iOwner.mRightSpiritHasCastSpell = true;
            spellData.SPELL.Cast(true, (ISpellCaster) iZone, iOwner.mRightSpiritCastType);
            break;
          }
          break;
        default:
          if (!iOwner.mLeftSpiritHasCastSpell && (iOwner.mLeftCastAnimation == Grimnir2.Animations.spirit_cast_projectile && (double) num >= 0.60000002384185791 || iOwner.mLeftCastAnimation == Grimnir2.Animations.spirit_cast_railgrun && (double) num >= 0.15000000596046448))
          {
            Grimnir2.SpellData spellData = Grimnir2.SPIRITSPELLS[iOwner.mLeftSpiritLastSpell];
            if (NetworkManager.Instance.State == NetworkState.Server)
            {
              Grimnir2.CastSpellMessage castSpellMessage;
              castSpellMessage.SpiritCast = true;
              castSpellMessage.SpiritIndex = 1;
              castSpellMessage.SpellIndex = iOwner.mLeftSpiritLastSpell;
              BossFight.Instance.SendMessage<Grimnir2.CastSpellMessage>((IBoss) iOwner, (ushort) 5, (void*) &castSpellMessage, true);
            }
            iOwner.mLeftSpiritCastType = spellData.CASTTYPE;
            iOwner.mLeftSpiritSpellPower = spellData.SPELLPOWER;
            iOwner.mLeftSpiritHasCastSpell = true;
            spellData.SPELL.Cast(true, (ISpellCaster) iZone, iOwner.mLeftSpiritCastType);
            break;
          }
          break;
      }
      if (!iZone.AnimationController.HasFinished)
        return;
      iOwner.ChangeSpiritState(Grimnir2.SpiritStates.Idle, iZone.Index);
    }

    public void OnExit(BossSpellCasterZone iZone, Grimnir2 iOwner)
    {
      switch (iZone.Index)
      {
        case 2:
          if (iOwner.mRightSpiritSpellEffect == null)
            break;
          iOwner.mRightSpiritSpellEffect.Stop((ISpellCaster) iZone);
          break;
        default:
          if (iOwner.mLeftSpiritSpellEffect == null)
            break;
          iOwner.mLeftSpiritSpellEffect.Stop((ISpellCaster) iZone);
          break;
      }
    }
  }

  protected class SpecHealState : Grimnir2.ISpiritState
  {
    private static Grimnir2.SpecHealState sSingelton;
    private static volatile object sSingeltonLock = new object();

    public static Grimnir2.SpecHealState Instance
    {
      get
      {
        if (Grimnir2.SpecHealState.sSingelton == null)
        {
          lock (Grimnir2.SpecHealState.sSingeltonLock)
          {
            if (Grimnir2.SpecHealState.sSingelton == null)
              Grimnir2.SpecHealState.sSingelton = new Grimnir2.SpecHealState();
          }
        }
        return Grimnir2.SpecHealState.sSingelton;
      }
    }

    private SpecHealState()
    {
    }

    public void OnEnter(BossSpellCasterZone iZone, Grimnir2 iOwner)
    {
      iZone.AnimationController.CrossFade(iOwner.mClips[11], 0.25f, true);
      if (iZone.Index == 1)
        iOwner.mLeftSpiritIdleTimer = 0.25f;
      else
        iOwner.mRightSpiritIdleTimer = 0.25f;
    }

    public void OnUpdate(float iDeltaTime, BossSpellCasterZone iZone, Grimnir2 iOwner)
    {
      if ((iZone.Index != 1 ? (double) (iOwner.mRightSpiritIdleTimer -= iDeltaTime) : (double) (iOwner.mLeftSpiritIdleTimer -= iDeltaTime)) <= 0.0)
      {
        iOwner.mSpectralTargetAlpha = Grimnir2.RANDOM.NextDouble() > 0.0 ? 0.0f : 1f;
        if (iZone.Index == 1)
          iOwner.mLeftSpiritIdleTimer = 0.25f;
        else
          iOwner.mRightSpiritIdleTimer = 0.25f;
      }
      if (iOwner.mAssaturHeal)
        return;
      iOwner.ChangeSpiritState(Grimnir2.SpiritStates.Idle, iZone.Index);
    }

    public void OnExit(BossSpellCasterZone iZone, Grimnir2 iOwner)
    {
      iOwner.mSpectralTargetAlpha = 1f;
    }
  }

  protected class SpecDieState : Grimnir2.ISpiritState
  {
    private static Grimnir2.SpecDieState sSingelton;
    private static volatile object sSingeltonLock = new object();

    public static Grimnir2.SpecDieState Instance
    {
      get
      {
        if (Grimnir2.SpecDieState.sSingelton == null)
        {
          lock (Grimnir2.SpecDieState.sSingeltonLock)
          {
            if (Grimnir2.SpecDieState.sSingelton == null)
              Grimnir2.SpecDieState.sSingelton = new Grimnir2.SpecDieState();
          }
        }
        return Grimnir2.SpecDieState.sSingelton;
      }
    }

    private SpecDieState()
    {
    }

    public void OnEnter(BossSpellCasterZone iZone, Grimnir2 iOwner)
    {
      iOwner.mSpectralTargetAlpha = 0.0f;
      iZone.AnimationController.CrossFade(iOwner.mClips[10], 0.15f, true);
    }

    public void OnUpdate(float iDeltaTime, BossSpellCasterZone iZone, Grimnir2 iOwner)
    {
    }

    public void OnExit(BossSpellCasterZone iZone, Grimnir2 iOwner)
    {
    }
  }

  private enum MessageType : ushort
  {
    Update,
    Script,
    ChangeState,
    ChangeSpiritState,
    GrimnirSpell,
    CastSpell,
    CastMagick,
    ChangeTarget,
    Corporealize,
  }

  internal struct UpdateMessage
  {
    public const ushort TYPE = 0;
    public Matrix LeftOrientation;
    public Matrix RightOrientation;
    public float LeftFloatCounter;
    public float RightFloatCounter;
    public float SpiritTimer;
    public byte Animation;
    public float AnimationTime;
    public float Hitpoints;
  }

  internal struct ScriptMessageMessage
  {
    public const ushort TYPE = 1;
    public BossMessages Message;
  }

  internal struct CorpMessage
  {
    public const ushort TYPE = 8;
    public bool Dummy;
  }

  internal struct ChangeStateMessage
  {
    public const ushort TYPE = 2;
    public Grimnir2.States NewState;
  }

  internal struct CastMagickMessage
  {
    public const ushort TYPE = 6;
    public MagickType Magick;
  }

  internal struct CastSpellMessage
  {
    public const ushort TYPE = 5;
    public bool SpiritCast;
    public int SpiritIndex;
    public int SpellIndex;
  }

  internal struct ChangeSpiritStateMessage
  {
    public const ushort TYPE = 3;
    public Grimnir2.SpiritStates NewState;
    public int SpiritIndex;
  }

  internal struct ChangeTargetMessage
  {
    public const ushort TYPE = 7;
    public ushort Target;
  }

  public enum States
  {
    Idle,
    Spirit,
    Magick,
    Spell,
    Heal,
    Die,
  }

  public enum SpiritStates
  {
    Intro,
    Idle,
    Cast,
    Heal,
    Die,
  }

  public struct SpellData
  {
    public Spell SPELL;
    public CastType CASTTYPE;
    public float SPELLPOWER;
  }

  private enum AsaAnimation
  {
    idle,
    heal,
    cut,
    NrOfAnimations,
  }

  private enum Animations
  {
    die,
    hanging,
    taunt,
    talk0,
    talk1,
    talk2,
    idle,
    spirit_idle,
    spirit_cast_projectile,
    spirit_cast_railgrun,
    spirit_die,
    cast_magick_global,
    cast_magick_direct,
    cast_spirit,
    cast_shield,
    NrOfAnimations,
  }
}
