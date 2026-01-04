// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Bosses.Fafnir
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Gamers;
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
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

#nullable disable
namespace Magicka.GameLogic.Entities.Bosses;

public class Fafnir : BossStatusEffected, IBoss
{
  private const int BEGIN_IDX = 0;
  private const int END_IDX = 1;
  private const int WINGS_IDX_OFFSET = 2;
  private const float MAXHITPOINTS = 35000f;
  private const float MAXHITPOINTS_DIVISOR = 2.85714286E-05f;
  private const float WAKEUP_PROXIMITY_SQR = 400f;
  private const float NETWORK_UPDATE_PERIOD = 0.0333333351f;
  private const float MIN_NECK_ANGLE = 0.0f;
  private const float MAX_NECK_ANGLE = 3.14159274f;
  private static readonly int[] LEVEL_PARTS = new int[25]
  {
    "hole_debris0".GetHashCodeCustom(),
    "hole_debris1".GetHashCodeCustom(),
    "hole_debris2".GetHashCodeCustom(),
    "hole_debris3".GetHashCodeCustom(),
    "hole_debris4".GetHashCodeCustom(),
    "hole_debris5".GetHashCodeCustom(),
    "hole_debris6".GetHashCodeCustom(),
    "hole_debris7".GetHashCodeCustom(),
    "hole_debris8".GetHashCodeCustom(),
    "hole_debris9".GetHashCodeCustom(),
    "hole_debris10".GetHashCodeCustom(),
    "hole_debris11".GetHashCodeCustom(),
    "hole_debris12".GetHashCodeCustom(),
    "hole_debris14".GetHashCodeCustom(),
    "hole_debris15".GetHashCodeCustom(),
    "hole_debris16".GetHashCodeCustom(),
    "hole_debris17".GetHashCodeCustom(),
    "hole_debris18".GetHashCodeCustom(),
    "hole_debris19".GetHashCodeCustom(),
    "hole_debris20".GetHashCodeCustom(),
    "hole_debris21".GetHashCodeCustom(),
    "hole_debris22".GetHashCodeCustom(),
    "hole_debris23".GetHashCodeCustom(),
    "hole_debris24".GetHashCodeCustom(),
    "hole_debris25".GetHashCodeCustom()
  };
  private static readonly int[] LAVA_EFFECTS = new int[25]
  {
    "effect_lava0".GetHashCodeCustom(),
    "effect_lava1".GetHashCodeCustom(),
    "effect_lava2".GetHashCodeCustom(),
    "effect_lava3".GetHashCodeCustom(),
    "effect_lava4".GetHashCodeCustom(),
    "effect_lava5".GetHashCodeCustom(),
    "effect_lava6".GetHashCodeCustom(),
    "effect_lava7".GetHashCodeCustom(),
    "effect_lava8".GetHashCodeCustom(),
    "effect_lava9".GetHashCodeCustom(),
    "effect_lava10".GetHashCodeCustom(),
    "effect_lava11".GetHashCodeCustom(),
    "effect_lava12".GetHashCodeCustom(),
    "effect_lava14".GetHashCodeCustom(),
    "effect_lava15".GetHashCodeCustom(),
    "effect_lava16".GetHashCodeCustom(),
    "effect_lava17".GetHashCodeCustom(),
    "effect_lava18".GetHashCodeCustom(),
    "effect_lava19".GetHashCodeCustom(),
    "effect_lava20".GetHashCodeCustom(),
    "effect_lava21".GetHashCodeCustom(),
    "effect_lava22".GetHashCodeCustom(),
    "effect_lava23".GetHashCodeCustom(),
    "effect_lava24".GetHashCodeCustom(),
    "effect_lava25".GetHashCodeCustom()
  };
  private static readonly float[][] ANIMATION_TIMES = new float[12][]
  {
    new float[2],
    new float[2]{ 0.06666667f, 0.933333337f },
    new float[2]{ 0.144444451f, 0.8888889f },
    new float[1]{ 0.514285743f },
    new float[2]{ 0.6944444f, 1f },
    new float[2]{ 0.422222227f, 0.822222233f },
    new float[2]{ 5f / 16f, 0.75f },
    new float[0],
    new float[1]{ 0.466666669f },
    new float[0],
    new float[2]{ 15f / 32f, 1f },
    new float[4]{ 0.47f, 0.51f, 0.67f, 0.67f }
  };
  private static readonly int WAKE_TRIGGER = "fafnir_awake".GetHashCodeCustom();
  private static readonly int DEFEATED_TRIGGER = "fafnir_defeated".GetHashCodeCustom();
  private static readonly int LAVAQUAKE_SOUND = "magick_meteor_rumble".GetHashCodeCustom();
  private static readonly int DIALOG_INTRO = "fafnirintro".GetHashCodeCustom();
  private static readonly int DIALOG_OUTRO = "fafnirdefeat".GetHashCodeCustom();
  private static readonly int BLOOD_BLACK_EFFECT = "gore_splash_black".GetHashCodeCustom();
  private static readonly int FIRE_SPRAY_EFFECT = "fafnir_fire_spray".GetHashCodeCustom();
  private static readonly int FIRE_BREATH_EFFECT = "fafnir_fire_breath".GetHashCodeCustom();
  private float mLastNetworkUpdate;
  protected float mNetworkUpdateTimer;
  private AudioEmitter mLanceAudioEmitter = new AudioEmitter();
  private static readonly int[] SOUNDS = new int[6]
  {
    "boss_fafnir_attack".GetHashCodeCustom(),
    "boss_fafnir_breakfloor".GetHashCodeCustom(),
    "boss_fafnir_confuse".GetHashCodeCustom(),
    "boss_fafnir_death".GetHashCodeCustom(),
    "boss_fafnir_deathray".GetHashCodeCustom(),
    "boss_fafnir_pain".GetHashCodeCustom()
  };
  private Fafnir.SleepState mSleepState;
  private Fafnir.IntroState mIntroState;
  private Fafnir.IdleState mIdleState;
  private Fafnir.DecisionState mDecisionState;
  private Fafnir.DefeatedState mDefeatedState;
  private Fafnir.ConfuseState mConfuseState;
  private Fafnir.WingState mWingState;
  private Fafnir.TailState mTailState;
  private Fafnir.FireballState mFireballState;
  private Fafnir.FirelanceHighState mFirelanceHighState;
  private Fafnir.FirelanceLowState mFirelanceLowState;
  private Fafnir.CeilingState mCeilingState;
  private Fafnir.EarthQuakeState mEarthQuakeState;
  private float mEarthQuakeThreshold;
  private int mNrOfEarthquakes;
  private float mConfusedTimer;
  private HitList mHitList;
  private Fafnir.RenderData[] mRenderData;
  private Fafnir.FireRenderData[] mFireRenderData;
  private AnimationController mAnimationController;
  private AnimationClip[] mAnimationClips;
  private Matrix mOrientation;
  private IBossState<Fafnir> mCurrentState;
  private IBossState<Fafnir> mPreviousState;
  private PlayState mPlayState;
  private bool mDead;
  private float mAimForTargetWeight;
  private Vector3 mAimTargetPosition;
  private bool mAimForTarget;
  private Cue mLavaCue;
  private int mNrOfNeckBones = 6;
  private int mNrOfTailBones = 11;
  private int[] mNeckIndices;
  private Matrix[] mNeckBindPoses;
  private int mHeadIndex;
  private Matrix mHeadBindPose;
  private int mMouthIndex;
  private Matrix mMouthBindPose;
  private int mRightEyeIndex;
  private Matrix mRightEyeBindPose;
  private int mLeftEyeIndex;
  private Matrix mLeftEyeBindPose;
  private int mSpineUpperIndex;
  private Matrix mSpineUpperBindPose;
  private int mSpineMidIndex;
  private Matrix mSpineMidBindPose;
  private int mSpineBaseIndex;
  private Matrix mSpineBaseBindPose;
  private int mRightHipIndex;
  private Matrix mRightHipBindPose;
  private int mRightHeelIndex;
  private Matrix mRightHeelBindPose;
  private int mRightToeIndex;
  private Matrix mRightToeBindPose;
  private int mRightShoulderIndex;
  private Matrix mRightShoulderBindPose;
  private int mRightWristIndex;
  private Matrix mRightWristBindPose;
  private int[] mTailIndices;
  private Matrix[] mTailBindPoses;
  private BoundingSphere mBoundingSphere;
  private bool mDrawFireLance;
  private Matrix mRightEyeOrientation;
  private Matrix mLeftEyeOrientation;
  private Matrix mMouthOrientation;
  private Vector3 mSpineBasePosition;
  private BossDamageZone mHeadZone;
  private BossDamageZone mBodyZone;
  private BossDamageZone mRightWristZone;
  private BossDamageZone mRightHeelZone;
  private BossDamageZone mTailZone;
  private Player[] mPlayers;
  private Magicka.GameLogic.Entities.Character mTarget;
  private float mLanceTime;
  private Model mDebrisModel;
  private float mLanceScale;
  private Segment mLanceSegment;
  private static readonly Random sRandom = new Random();
  private static readonly Magicka.GameLogic.Damage sTailDamage = new Magicka.GameLogic.Damage(AttackProperties.Knockback, Elements.Earth, 200f, 5f);
  private float mDamageFlashTimer;
  private Matrix mRenderRotationOffset = Matrix.CreateRotationY(1.57079637f);

  public Fafnir(PlayState iPlayState)
  {
    this.mPlayState = iPlayState;
    this.mHitList = new HitList(16 /*0x10*/);
    SkinnedModel skinnedModel1;
    SkinnedModel skinnedModel2;
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      skinnedModel1 = this.mPlayState.Content.Load<SkinnedModel>("Models/Bosses/Fafnir/Fafnir_mesh");
      skinnedModel2 = this.mPlayState.Content.Load<SkinnedModel>("Models/Bosses/Fafnir/Fafnir_animation");
      this.mDebrisModel = this.mPlayState.Content.Load<Model>("Models/Bosses/Fafnir/Ceiling_Debri");
    }
    this.mTailIndices = new int[this.mNrOfTailBones];
    this.mTailBindPoses = new Matrix[this.mNrOfTailBones];
    this.mNeckIndices = new int[this.mNrOfNeckBones + 1];
    this.mNeckBindPoses = new Matrix[this.mNrOfNeckBones + 1];
    Matrix result1;
    Matrix.CreateRotationY(3.14159274f, out result1);
    for (int index1 = 0; index1 < skinnedModel1.SkeletonBones.Count; ++index1)
    {
      SkinnedModelBone skeletonBone = skinnedModel1.SkeletonBones[index1];
      if (skeletonBone.Name.Equals("Head", StringComparison.OrdinalIgnoreCase))
      {
        this.mHeadIndex = (int) skeletonBone.Index;
        this.mHeadBindPose = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mHeadBindPose, ref result1, out this.mHeadBindPose);
        Matrix.Invert(ref this.mHeadBindPose, out this.mHeadBindPose);
        this.mNeckIndices[this.mNrOfNeckBones] = (int) skeletonBone.Index;
        this.mNeckBindPoses[this.mNrOfNeckBones] = this.mHeadBindPose;
      }
      else if (skeletonBone.Name.Equals("Mouth", StringComparison.OrdinalIgnoreCase))
      {
        this.mMouthIndex = (int) skeletonBone.Index;
        this.mMouthBindPose = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mMouthBindPose, ref result1, out this.mMouthBindPose);
        Matrix.Invert(ref this.mMouthBindPose, out this.mMouthBindPose);
      }
      else if (skeletonBone.Name.Equals("RightEye", StringComparison.OrdinalIgnoreCase))
      {
        this.mRightEyeIndex = (int) skeletonBone.Index;
        this.mRightEyeBindPose = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mRightEyeBindPose, ref result1, out this.mRightEyeBindPose);
        Matrix.Invert(ref this.mRightEyeBindPose, out this.mRightEyeBindPose);
      }
      else if (skeletonBone.Name.Equals("LeftEye", StringComparison.OrdinalIgnoreCase))
      {
        this.mLeftEyeIndex = (int) skeletonBone.Index;
        this.mLeftEyeBindPose = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mLeftEyeBindPose, ref result1, out this.mLeftEyeBindPose);
        Matrix.Invert(ref this.mLeftEyeBindPose, out this.mLeftEyeBindPose);
      }
      else if (skeletonBone.Name.Equals("SpineUpper", StringComparison.OrdinalIgnoreCase))
      {
        this.mSpineUpperIndex = (int) skeletonBone.Index;
        this.mSpineUpperBindPose = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mSpineUpperBindPose, ref result1, out this.mSpineUpperBindPose);
        Matrix.Invert(ref this.mSpineUpperBindPose, out this.mSpineUpperBindPose);
      }
      else if (skeletonBone.Name.Equals("SpineMid", StringComparison.OrdinalIgnoreCase))
      {
        this.mSpineMidIndex = (int) skeletonBone.Index;
        this.mSpineMidBindPose = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mSpineMidBindPose, ref result1, out this.mSpineMidBindPose);
        Matrix.Invert(ref this.mSpineMidBindPose, out this.mSpineMidBindPose);
      }
      else if (skeletonBone.Name.Equals("SpineBase", StringComparison.OrdinalIgnoreCase))
      {
        this.mSpineBaseIndex = (int) skeletonBone.Index;
        this.mSpineBaseBindPose = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mSpineBaseBindPose, ref result1, out this.mSpineBaseBindPose);
        Matrix.Invert(ref this.mSpineBaseBindPose, out this.mSpineBaseBindPose);
      }
      else if (skeletonBone.Name.Equals("RightHip", StringComparison.OrdinalIgnoreCase))
      {
        this.mRightHipIndex = (int) skeletonBone.Index;
        this.mRightHipBindPose = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mRightHipBindPose, ref result1, out this.mRightHipBindPose);
        Matrix.Invert(ref this.mRightHipBindPose, out this.mRightHipBindPose);
      }
      else if (skeletonBone.Name.Equals("RightHeel", StringComparison.OrdinalIgnoreCase))
      {
        this.mRightHeelIndex = (int) skeletonBone.Index;
        this.mRightHeelBindPose = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mRightHeelBindPose, ref result1, out this.mRightHeelBindPose);
        Matrix.Invert(ref this.mRightHeelBindPose, out this.mRightHeelBindPose);
      }
      else if (skeletonBone.Name.Equals("RightIndextoeEnd", StringComparison.OrdinalIgnoreCase))
      {
        this.mRightToeIndex = (int) skeletonBone.Index;
        this.mRightToeBindPose = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mRightToeBindPose, ref result1, out this.mRightToeBindPose);
        Matrix.Invert(ref this.mRightToeBindPose, out this.mRightToeBindPose);
      }
      else if (skeletonBone.Name.Equals("RightShoulder", StringComparison.OrdinalIgnoreCase))
      {
        this.mRightShoulderIndex = (int) skeletonBone.Index;
        this.mRightShoulderBindPose = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mRightShoulderBindPose, ref result1, out this.mRightShoulderBindPose);
        Matrix.Invert(ref this.mRightShoulderBindPose, out this.mRightShoulderBindPose);
      }
      else if (skeletonBone.Name.Equals("RightWrist", StringComparison.OrdinalIgnoreCase))
      {
        this.mRightWristIndex = (int) skeletonBone.Index;
        this.mRightWristBindPose = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mRightWristBindPose, ref result1, out this.mRightWristBindPose);
        Matrix.Invert(ref this.mRightWristBindPose, out this.mRightWristBindPose);
      }
      else
      {
        for (int index2 = 0; index2 < this.mTailIndices.Length; ++index2)
        {
          string str = $"Tail{index2 + 1}";
          if (skeletonBone.Name.Equals(str, StringComparison.OrdinalIgnoreCase))
          {
            this.mTailIndices[index2] = (int) skeletonBone.Index;
            this.mTailBindPoses[index2] = skeletonBone.InverseBindPoseTransform;
            Matrix.Multiply(ref this.mTailBindPoses[index2], ref result1, out this.mTailBindPoses[index2]);
            Matrix.Invert(ref this.mTailBindPoses[index2], out this.mTailBindPoses[index2]);
            break;
          }
        }
        for (int index3 = 0; index3 < this.mNrOfNeckBones; ++index3)
        {
          string str = $"Neck{index3 + 1}";
          if (skeletonBone.Name.Equals(str, StringComparison.OrdinalIgnoreCase))
          {
            this.mNeckIndices[index3] = (int) skeletonBone.Index;
            this.mNeckBindPoses[index3] = skeletonBone.InverseBindPoseTransform;
            Matrix.Multiply(ref this.mNeckBindPoses[index3], ref result1, out this.mNeckBindPoses[index3]);
            Matrix.Invert(ref this.mNeckBindPoses[index3], out this.mNeckBindPoses[index3]);
            break;
          }
        }
      }
    }
    this.mAnimationController = new AnimationController();
    this.mAnimationController.Skeleton = skinnedModel1.SkeletonBones;
    this.mAnimationClips = new AnimationClip[12];
    this.mAnimationClips[8] = skinnedModel2.AnimationClips["intro"];
    this.mAnimationClips[7] = skinnedModel2.AnimationClips["idle"];
    this.mAnimationClips[1] = skinnedModel2.AnimationClips["ceiling"];
    this.mAnimationClips[2] = skinnedModel2.AnimationClips["charm"];
    this.mAnimationClips[3] = skinnedModel2.AnimationClips["defeated"];
    this.mAnimationClips[5] = skinnedModel2.AnimationClips["fire_high"];
    this.mAnimationClips[6] = skinnedModel2.AnimationClips["fire_low"];
    this.mAnimationClips[4] = skinnedModel2.AnimationClips["fireball"];
    this.mAnimationClips[9] = skinnedModel2.AnimationClips["sleep"];
    this.mAnimationClips[10] = skinnedModel2.AnimationClips["tailwhip"];
    this.mAnimationClips[11] = skinnedModel2.AnimationClips["wings"];
    Matrix result2;
    Matrix.CreateRotationZ(-3.14159274f, out result2);
    this.mHeadZone = new BossDamageZone(this.mPlayState, (IBoss) this, 0, 3f, (Primitive) new Capsule(Vector3.Forward, result2, 1f, 3f));
    this.mBodyZone = new BossDamageZone(this.mPlayState, (IBoss) this, 1, 3.2f, (Primitive) new Capsule(Vector3.Zero, Matrix.Identity, 3.2f, Vector3.Distance(this.mSpineBaseBindPose.Translation, this.mSpineUpperBindPose.Translation)));
    this.mBodyZone.Body.CollisionSkin.callbackFn += new CollisionCallbackFn(this.OnCollision);
    this.mRightHeelZone = new BossDamageZone(this.mPlayState, (IBoss) this, 2, 2f, (Primitive) new Capsule(Vector3.Zero, result1, 1f, 2f));
    this.mRightHeelZone.Body.CollisionSkin.callbackFn += new CollisionCallbackFn(this.OnCollision);
    this.mRightWristZone = new BossDamageZone(this.mPlayState, (IBoss) this, 2, 2f, (Primitive) new Capsule(Vector3.Zero, result1, 1f, 2f));
    this.mRightWristZone.Body.CollisionSkin.callbackFn += new CollisionCallbackFn(this.OnCollision);
    Primitive[] primitiveArray = new Primitive[this.mNrOfTailBones];
    for (int index = 0; index < primitiveArray.Length - 1; ++index)
    {
      float radius = System.Math.Max((float) (primitiveArray.Length - index) * 0.15f, 0.333f);
      float length = System.Math.Max(Vector3.Distance(this.mTailBindPoses[index].Translation, this.mTailBindPoses[index + 1].Translation) - radius, 0.0f);
      primitiveArray[index] = (Primitive) new Capsule(Vector3.Zero, Matrix.Identity, radius, length);
    }
    primitiveArray[this.mNrOfTailBones - 1] = (Primitive) new Capsule(Vector3.Zero, Matrix.Identity, 0.333f, 0.333f);
    this.mTailZone = new BossDamageZone(this.mPlayState, (IBoss) this, 3, 15f, primitiveArray);
    this.mTailZone.Body.CollisionSkin.callbackFn += new CollisionCallbackFn(this.OnTailCollision);
    ModelMesh mesh = skinnedModel1.Model.Meshes[0];
    ModelMeshPart meshPart = mesh.MeshParts[0];
    this.mBoundingSphere = mesh.BoundingSphere;
    SkinnedModelDeferredBasicMaterial oMaterial;
    Helper.SkinnedModelDeferredMaterialFromBasicEffect(meshPart.Effect as SkinnedModelBasicEffect, out oMaterial);
    this.mRenderData = new Fafnir.RenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      this.mRenderData[index] = new Fafnir.RenderData();
      this.mRenderData[index].SetMesh(mesh.VertexBuffer, mesh.IndexBuffer, meshPart, 0, 3, 4);
      this.mRenderData[index].mMaterial = oMaterial;
    }
    this.mSleepState = new Fafnir.SleepState();
    this.mDecisionState = new Fafnir.DecisionState();
    this.mIntroState = new Fafnir.IntroState();
    this.mIdleState = new Fafnir.IdleState();
    this.mDefeatedState = new Fafnir.DefeatedState();
    this.mConfuseState = new Fafnir.ConfuseState();
    this.mWingState = new Fafnir.WingState();
    this.mTailState = new Fafnir.TailState();
    this.mFireballState = new Fafnir.FireballState();
    this.mFirelanceHighState = new Fafnir.FirelanceHighState();
    this.mFirelanceLowState = new Fafnir.FirelanceLowState();
    this.mCeilingState = new Fafnir.CeilingState();
    this.mEarthQuakeState = new Fafnir.EarthQuakeState();
    Texture2D texture2D = this.mPlayState.Content.Load<Texture2D>("EffectTextures/FireLance02");
    VertexPositionColorTexture[] data = new VertexPositionColorTexture[6];
    float x = 0.5f;
    float y = 1f;
    data[0].TextureCoordinate = new Vector2(0.0f, y);
    data[0].Position = new Vector3(-0.5f, 0.0f, 1f);
    data[0].Color = Color.White;
    data[1].TextureCoordinate = new Vector2(0.0f, 0.0f);
    data[1].Position = new Vector3(-0.5f, 0.0f, 0.0f);
    data[1].Color = Color.White;
    data[2].TextureCoordinate = new Vector2(x, 0.0f);
    data[2].Position = new Vector3(0.5f, 0.0f, 0.0f);
    data[2].Color = Color.White;
    data[3].TextureCoordinate = new Vector2(x, 0.0f);
    data[3].Position = new Vector3(0.5f, 0.0f, 0.0f);
    data[3].Color = Color.White;
    data[4].TextureCoordinate = new Vector2(x, y);
    data[4].Position = new Vector3(0.5f, 0.0f, 1f);
    data[4].Color = Color.White;
    data[5].TextureCoordinate = new Vector2(0.0f, y);
    data[5].Position = new Vector3(-0.5f, 0.0f, 1f);
    data[5].Color = Color.White;
    VertexBuffer iVertexBuffer;
    VertexDeclaration iDeclaration;
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      iVertexBuffer = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, data.Length * VertexPositionColorTexture.SizeInBytes, BufferUsage.WriteOnly);
      iVertexBuffer.SetData<VertexPositionColorTexture>(data);
      iDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, VertexPositionColorTexture.VertexElements);
    }
    iVertexBuffer.Name = "FireLanceBuffer";
    AdditiveMaterial additiveMaterial = new AdditiveMaterial();
    lock (Magicka.Game.Instance.GraphicsDevice)
      additiveMaterial.Texture = texture2D;
    additiveMaterial.TextureEnabled = true;
    additiveMaterial.VertexColorEnabled = false;
    additiveMaterial.ColorTint = new Vector4(1f, 1f, 1f, 1f);
    this.mFireRenderData = new Fafnir.FireRenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      this.mFireRenderData[index] = new Fafnir.FireRenderData(iVertexBuffer, iDeclaration);
      this.mFireRenderData[index].mMaterial = additiveMaterial;
    }
  }

  protected bool OnTailCollision(
    CollisionSkin iSkin0,
    int iPrim0,
    CollisionSkin iSkin1,
    int iPrim1)
  {
    if (this.Dead && iSkin1.Owner != null)
      return false;
    if (iSkin1.Owner != null && this.mCurrentState is Fafnir.TailState)
    {
      float num1 = this.mAnimationController.Time / this.mAnimationController.AnimationClip.Duration;
      if ((double) num1 >= (double) Fafnir.ANIMATION_TIMES[10][0] && (double) num1 <= (double) Fafnir.ANIMATION_TIMES[10][1] && !this.mAnimationController.CrossFadeEnabled && iSkin1.Owner.Tag is IDamageable tag && !this.mHitList.ContainsKey(tag.Handle))
      {
        tag.Position.Z -= 4f;
        Vector3 position = (iSkin0.GetPrimitiveNewWorld(iPrim0) as Capsule).Position;
        this.mHitList.Add(tag.Handle, 1f);
        int num2 = (int) tag.Damage(Fafnir.sTailDamage, (Magicka.GameLogic.Entities.Entity) this.mTailZone, 0.0, position);
      }
    }
    return true;
  }

  protected bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
  {
    return true;
  }

  public void Initialize(ref Matrix iOrientation, int iUniqueID)
  {
    this.Initialize(ref iOrientation);
  }

  public void Initialize(ref Matrix iOrientation)
  {
    this.mOrientation = iOrientation;
    this.mBoundingSphere.Center = this.mOrientation.Translation;
    Vector3 oPos;
    if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out Vector3 _, new Segment()
    {
      Origin = iOrientation.Translation,
      Delta = {
        Y = -10f
      }
    }))
    {
      oPos.Y -= 0.333f;
      this.mOrientation.Translation = oPos;
    }
    this.mHeadZone.Initialize("#boss_n14".GetHashCodeCustom());
    this.mHeadZone.Body.CollisionSkin.NonCollidables.Add(this.mBodyZone.Body.CollisionSkin);
    this.mHeadZone.Body.CollisionSkin.NonCollidables.Add(this.mRightHeelZone.Body.CollisionSkin);
    this.mHeadZone.Body.CollisionSkin.NonCollidables.Add(this.mRightWristZone.Body.CollisionSkin);
    this.mHeadZone.Body.CollisionSkin.NonCollidables.Add(this.mTailZone.Body.CollisionSkin);
    this.mHeadZone.Body.CollisionSkin.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
    this.mPlayState.EntityManager.AddEntity((Magicka.GameLogic.Entities.Entity) this.mHeadZone);
    this.mBodyZone.Initialize();
    this.mBodyZone.Body.CollisionSkin.NonCollidables.Add(this.mRightHeelZone.Body.CollisionSkin);
    this.mBodyZone.Body.CollisionSkin.NonCollidables.Add(this.mRightWristZone.Body.CollisionSkin);
    this.mBodyZone.Body.CollisionSkin.NonCollidables.Add(this.mTailZone.Body.CollisionSkin);
    this.mBodyZone.Body.CollisionSkin.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
    this.mPlayState.EntityManager.AddEntity((Magicka.GameLogic.Entities.Entity) this.mBodyZone);
    this.mRightWristZone.Initialize();
    this.mRightWristZone.Body.CollisionSkin.NonCollidables.Add(this.mRightHeelZone.Body.CollisionSkin);
    this.mRightWristZone.Body.CollisionSkin.NonCollidables.Add(this.mTailZone.Body.CollisionSkin);
    this.mRightWristZone.Body.CollisionSkin.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
    this.mPlayState.EntityManager.AddEntity((Magicka.GameLogic.Entities.Entity) this.mRightWristZone);
    this.mRightHeelZone.Initialize();
    this.mRightHeelZone.Body.CollisionSkin.NonCollidables.Add(this.mTailZone.Body.CollisionSkin);
    this.mRightHeelZone.Body.CollisionSkin.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
    this.mPlayState.EntityManager.AddEntity((Magicka.GameLogic.Entities.Entity) this.mRightHeelZone);
    this.mTailZone.Initialize();
    this.mTailZone.Body.CollisionSkin.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
    this.mPlayState.EntityManager.AddEntity((Magicka.GameLogic.Entities.Entity) this.mTailZone);
    this.mCurrentState = (IBossState<Fafnir>) this.mSleepState;
    this.mCurrentState.OnEnter(this);
    this.mMaxHitPoints = 35000f;
    this.mHitPoints = this.mMaxHitPoints;
    this.mDead = false;
    this.mAimForTarget = false;
    this.mLanceTime = 0.0f;
    this.mDrawFireLance = false;
    this.mPlayers = Magicka.Game.Instance.Players;
    this.mHitList.Clear();
    for (int index = 0; index < this.mStatusEffects.Length; ++index)
    {
      this.mStatusEffects[index].Stop();
      this.mStatusEffects[index] = new StatusEffect();
    }
    this.mCurrentStatusEffects = StatusEffects.None;
    for (int iIndex = 0; iIndex < this.mResistances.Length; ++iIndex)
    {
      this.mResistances[iIndex].ResistanceAgainst = Spell.ElementFromIndex(iIndex);
      switch (this.mResistances[iIndex].ResistanceAgainst)
      {
        case Elements.Water:
          this.mResistances[iIndex].Modifier = 0.0f;
          this.mResistances[iIndex].Multiplier = 0.0f;
          break;
        case Elements.Cold:
          this.mResistances[iIndex].Modifier = 10f;
          this.mResistances[iIndex].Multiplier = 2f;
          break;
        case Elements.Fire:
          this.mResistances[iIndex].Modifier = 0.0f;
          this.mResistances[iIndex].Multiplier = -3f;
          break;
        default:
          this.mResistances[iIndex].Modifier = 0.0f;
          this.mResistances[iIndex].Multiplier = 1f;
          break;
      }
    }
    for (int index = 0; index < Fafnir.LEVEL_PARTS.Length; ++index)
    {
      AnimatedLevelPart animatedLevelPart = this.mPlayState.Level.CurrentScene.LevelModel.GetAnimatedLevelPart(Fafnir.LEVEL_PARTS[index]);
      animatedLevelPart.Play(true, -1f, -1f, 1f, false, false);
      animatedLevelPart.Stop(true);
    }
    this.mNrOfEarthquakes = 0;
    this.SetEarthQuakeThreshold();
    this.mDamageFlashTimer = 0.0f;
  }

  public void UpdateBoss(DataChannel iDataChannel, float iDeltaTime, bool iFightStarted)
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
    if (iFightStarted && this.mCurrentState is Fafnir.SleepState)
      this.ChangeState(Fafnir.State.Intro);
    else if ((double) this.mHitPoints <= 1.0 && this.mCurrentState != this.mDefeatedState)
      this.ChangeState(Fafnir.State.Defeated);
    if (this.mConfuseState.mInverted)
    {
      this.mConfusedTimer -= iDeltaTime;
      if ((double) this.mConfusedTimer <= 0.0)
      {
        Player[] players = Magicka.Game.Instance.Players;
        for (int index = 0; index < players.Length; ++index)
        {
          if (players[index].Playing && !(players[index].Gamer is NetworkGamer))
            players[index].Controller.Invert(false);
        }
        this.mConfuseState.mInverted = false;
      }
    }
    Matrix result1 = Matrix.Identity;
    Matrix.Multiply(ref result1, ref this.mRenderRotationOffset, out result1);
    Matrix.Multiply(ref result1, ref this.mOrientation, out result1);
    this.mAnimationController.Update(iDeltaTime, ref result1, true);
    this.mAimForTargetWeight = !this.mAimForTarget ? System.Math.Max(this.mAimForTargetWeight - iDeltaTime, 0.0f) : System.Math.Min(this.mAimForTargetWeight + iDeltaTime, 1f);
    if (this.mTarget != null)
    {
      Vector3 position = this.mTarget.Position;
      this.mAimTargetPosition.X += (position.X - this.mAimTargetPosition.X) * iDeltaTime;
      this.mAimTargetPosition.Y += (position.Y - this.mAimTargetPosition.Y) * iDeltaTime;
      this.mAimTargetPosition.Z += (position.Z - this.mAimTargetPosition.Z) * iDeltaTime;
    }
    Matrix result2;
    if ((double) this.mAimForTargetWeight > 1.4012984643248171E-45)
    {
      Matrix.CreateRotationY(3.14159274f, out Matrix _);
      Vector3 aimTargetPosition = this.mAimTargetPosition;
      for (int index = 0; index < this.mNeckIndices.Length; ++index)
      {
        result2 = this.mMouthBindPose;
        Matrix.Multiply(ref result2, ref this.mAnimationController.SkinnedBoneTransforms[this.mMouthIndex], out result2);
        Vector3 translation1 = result2.Translation;
        Vector3 forward = result2.Forward;
        Vector3 result3;
        Vector3.Subtract(ref aimTargetPosition, ref translation1, out result3);
        Vector3 vector2 = result3 with { Y = 0.0f };
        vector2.Normalize();
        Vector3 vector1 = forward with { Y = 0.0f };
        vector1.Normalize();
        float result4;
        Vector3.Dot(ref vector1, ref vector2, out result4);
        result4 = MathHelper.Clamp(result4, -1f, 1f);
        Vector3 result5;
        Vector3.Cross(ref vector1, ref vector2, out result5);
        int mNeckIndex = this.mNeckIndices[index];
        Matrix result6;
        Quaternion result7;
        if ((double) result5.LengthSquared() > 9.9999999747524271E-07)
        {
          float angle = (float) System.Math.Acos((double) result4) * this.mAimForTargetWeight / (float) (this.mNeckIndices.Length - index);
          result6 = this.mAnimationController.Skeleton[mNeckIndex].InverseBindPoseTransform;
          Matrix.Invert(ref result6, out result6);
          Matrix.Multiply(ref result6, ref this.mAnimationController.SkinnedBoneTransforms[mNeckIndex], out result6);
          Matrix.Invert(ref result6, out result6);
          Vector3.TransformNormal(ref result5, ref result6, out result5);
          result5.Normalize();
          Quaternion.CreateFromAxisAngle(ref result5, angle, out result7);
          Quaternion.Concatenate(ref result7, ref this.mAnimationController.LocalBonePoses[mNeckIndex].Orientation, out this.mAnimationController.LocalBonePoses[mNeckIndex].Orientation);
          this.mAnimationController.UpdateAbsoluteBoneTransformsFrom(mNeckIndex);
        }
        if (index == this.mNeckIndices.Length - 1)
        {
          result2 = this.mMouthBindPose;
          Matrix.Multiply(ref result2, ref this.mAnimationController.SkinnedBoneTransforms[this.mMouthIndex], out result2);
          Vector3 translation2 = result2.Translation;
          forward = result2.Forward;
          Vector3.Subtract(ref aimTargetPosition, ref translation2, out result3);
          result3.Normalize();
          Vector3.Cross(ref forward, ref result3, out result5);
          if ((double) result5.LengthSquared() > 9.9999999747524271E-07)
          {
            Vector3.Dot(ref forward, ref result3, out result4);
            result4 = MathHelper.Clamp(result4, -1f, 1f);
            float angle = (float) System.Math.Acos((double) result4) * this.mAimForTargetWeight;
            result6 = this.mAnimationController.Skeleton[mNeckIndex].InverseBindPoseTransform;
            Matrix.Invert(ref result6, out result6);
            Matrix.Multiply(ref result6, ref this.mAnimationController.SkinnedBoneTransforms[mNeckIndex], out result6);
            Matrix.Invert(ref result6, out result6);
            Vector3.TransformNormal(ref result5, ref result6, out result5);
            result5.Normalize();
            Quaternion.CreateFromAxisAngle(ref result5, angle, out result7);
            Quaternion.Concatenate(ref result7, ref this.mAnimationController.LocalBonePoses[mNeckIndex].Orientation, out this.mAnimationController.LocalBonePoses[mNeckIndex].Orientation);
            this.mAnimationController.UpdateAbsoluteBoneTransformsFrom(mNeckIndex);
          }
        }
      }
    }
    result2 = this.mRightEyeBindPose;
    Matrix.Multiply(ref result2, ref this.mAnimationController.SkinnedBoneTransforms[this.mRightEyeIndex], out this.mRightEyeOrientation);
    result2 = this.mLeftEyeBindPose;
    Matrix.Multiply(ref result2, ref this.mAnimationController.SkinnedBoneTransforms[this.mLeftEyeIndex], out this.mLeftEyeOrientation);
    result2 = this.mMouthBindPose;
    Matrix.Multiply(ref result2, ref this.mAnimationController.SkinnedBoneTransforms[this.mMouthIndex], out this.mMouthOrientation);
    Array.Copy((Array) this.mAnimationController.SkinnedBoneTransforms, 0, (Array) this.mRenderData[(int) iDataChannel].mSkeleton, 0, this.mAnimationController.Skeleton.Count);
    this.mDamageFlashTimer = System.Math.Max(this.mDamageFlashTimer - iDeltaTime, 0.0f);
    this.mRenderData[(int) iDataChannel].mBoundingSphere = this.mBoundingSphere;
    this.mRenderData[(int) iDataChannel].mDamage = (float) (1.0 - (double) this.mHitPoints * 2.8571428629220463E-05);
    this.mRenderData[(int) iDataChannel].Flash = this.mDamageFlashTimer * 10f;
    this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) this.mRenderData[(int) iDataChannel]);
    this.mHitList.Update(iDeltaTime);
    Transform identity = Transform.Identity;
    Vector3 result8 = Vector3.Up;
    Vector3 zero = Vector3.Zero;
    Vector3 result9;
    for (int prim = 0; prim < this.mNrOfTailBones - 1; ++prim)
    {
      Vector3 result10 = this.mTailBindPoses[prim].Translation;
      Vector3.Transform(ref result10, ref this.mAnimationController.SkinnedBoneTransforms[this.mTailIndices[prim]], out result10);
      if (prim == 0)
        this.mSpineBasePosition = result10;
      Vector3 result11 = this.mTailBindPoses[prim + 1].Translation;
      Vector3.Transform(ref result11, ref this.mAnimationController.SkinnedBoneTransforms[this.mTailIndices[prim + 1]], out result11);
      Vector3.Subtract(ref result10, ref result11, out result9);
      result9.Normalize();
      Matrix.CreateWorld(ref zero, ref result9, ref result8, out identity.Orientation);
      identity.Position = result10;
      this.mTailZone.Body.CollisionSkin.GetPrimitiveLocal(prim).SetTransform(ref identity);
      this.mTailZone.Body.CollisionSkin.GetPrimitiveNewWorld(prim).SetTransform(ref identity);
      this.mTailZone.Body.CollisionSkin.GetPrimitiveOldWorld(prim).SetTransform(ref identity);
    }
    Vector3 result12 = this.mTailBindPoses[this.mNrOfTailBones - 1].Translation;
    Vector3.Transform(ref result12, ref this.mAnimationController.SkinnedBoneTransforms[this.mTailIndices[this.mNrOfTailBones - 2]], out result12);
    Vector3 result13 = this.mTailBindPoses[this.mNrOfTailBones - 2].Translation;
    Vector3.Transform(ref result13, ref this.mAnimationController.SkinnedBoneTransforms[this.mTailIndices[this.mNrOfTailBones - 1]], out result13);
    Vector3.Subtract(ref result12, ref result13, out result9);
    result9.Normalize();
    Matrix.CreateWorld(ref zero, ref result9, ref result8, out identity.Orientation);
    identity.Position = result12;
    this.mTailZone.Body.CollisionSkin.GetPrimitiveLocal(this.mNrOfTailBones - 1).SetTransform(ref identity);
    this.mTailZone.Body.CollisionSkin.GetPrimitiveOldWorld(this.mNrOfTailBones - 1).SetTransform(ref identity);
    this.mTailZone.Body.CollisionSkin.GetPrimitiveNewWorld(this.mNrOfTailBones - 1).SetTransform(ref identity);
    this.mTailZone.Body.CollisionSkin.UpdateWorldBoundingBox();
    result2 = this.mMouthOrientation;
    Vector3 translation3 = this.mMouthOrientation.Translation;
    result2.Translation = new Vector3();
    this.mHeadZone.SetOrientation(ref translation3, ref result2);
    result2 = this.mRightHeelBindPose;
    Matrix.Multiply(ref result2, ref this.mAnimationController.SkinnedBoneTransforms[this.mRightHeelIndex], out result2);
    result12 = result2.Translation;
    result2 = this.mRightToeBindPose;
    Matrix.Multiply(ref result2, ref this.mAnimationController.SkinnedBoneTransforms[this.mRightToeIndex], out result2);
    result13 = result2.Translation;
    Vector3.Subtract(ref result13, ref result12, out result9);
    result9.Normalize();
    Matrix.CreateWorld(ref zero, ref result9, ref result8, out result2);
    this.mRightHeelZone.SetOrientation(ref result12, ref result2);
    result2 = this.mRightWristBindPose;
    Matrix.Multiply(ref result2, ref this.mAnimationController.SkinnedBoneTransforms[this.mRightWristIndex], out result2);
    Vector3 translation4 = result2.Translation;
    result2.Translation = new Vector3();
    this.mRightWristZone.SetOrientation(ref translation4, ref result2);
    result2 = this.mSpineBaseBindPose;
    translation4 = result2.Translation;
    --translation4.Y;
    result2.Translation = translation4;
    Matrix.Multiply(ref result2, ref this.mAnimationController.SkinnedBoneTransforms[this.mSpineBaseIndex], out result2);
    translation4 = result2.Translation;
    result2.Translation = new Vector3();
    this.mBodyZone.SetOrientation(ref translation4, ref result2);
    this.mCurrentState.OnUpdate(iDeltaTime, this);
    if (this.mDrawFireLance)
    {
      this.mLanceTime -= iDeltaTime;
      Vector3 translation5 = this.mMouthOrientation.Translation;
      this.mLanceSegment.Origin = translation5;
      float result14 = 30f * this.mLanceScale;
      result9 = this.mHeadZone.Body.Orientation.Forward;
      Vector3.Multiply(ref result9, result14, out this.mLanceSegment.Delta);
      Vector3 vector3 = this.mAimTargetPosition;
      bool flag = false;
      List<Shield> shields = this.mPlayState.EntityManager.Shields;
      for (int index = 0; index < shields.Count; ++index)
      {
        if (shields[index].SegmentIntersect(out vector3, this.mLanceSegment, 0.5f))
        {
          flag = true;
          Vector3.Subtract(ref vector3, ref this.mLanceSegment.Origin, out this.mLanceSegment.Delta);
          result14 = this.mLanceSegment.Delta.Length();
          break;
        }
      }
      if (!flag)
      {
        float frac;
        Vector3 pos;
        Vector3 normal;
        if (this.mPlayState.Level.CurrentScene.CollisionSkin.SegmentIntersect(out frac, out pos, out normal, this.mLanceSegment))
        {
          vector3 = pos;
          Vector3.Subtract(ref vector3, ref this.mLanceSegment.Origin, out this.mLanceSegment.Delta);
          Vector3.Distance(ref translation5, ref vector3, out result14);
        }
        else
        {
          foreach (AnimatedLevelPart animatedLevelPart in this.mPlayState.Level.CurrentScene.LevelModel.AnimatedLevelParts.Values)
          {
            if (animatedLevelPart.CollisionSkin.SegmentIntersect(out frac, out pos, out normal, this.mLanceSegment))
            {
              vector3 = pos;
              Vector3.Subtract(ref vector3, ref this.mLanceSegment.Origin, out this.mLanceSegment.Delta);
              Vector3.Distance(ref translation5, ref vector3, out result14);
              flag = true;
              break;
            }
          }
          if (!flag)
            Vector3.Add(ref this.mLanceSegment.Delta, ref translation5, out vector3);
        }
      }
      this.mLanceAudioEmitter.Position = vector3;
      this.mLanceAudioEmitter.Up = Vector3.Up;
      this.mLanceAudioEmitter.Forward = Vector3.UnitZ;
      Vector3 position = this.mPlayState.Camera.Position;
      Vector3 result15;
      Vector3.Subtract(ref position, ref translation5, out result15);
      Vector3.Negate(ref result9, out result9);
      Vector3 result16;
      Vector3.Cross(ref result9, ref result15, out result16);
      Vector3.Normalize(ref result16, out result16);
      Vector3.Cross(ref result16, ref result9, out result8);
      Vector3.Normalize(ref result8, out result8);
      Matrix result17;
      Matrix.CreateScale(1f, 1f, result14, out result17);
      result1.Forward = result9;
      result1.Up = result8;
      result1.Right = result16;
      Matrix.Multiply(ref result17, ref result1, out result1);
      result1.Translation = translation5;
      this.mLanceScale = System.Math.Max(System.Math.Min(this.mLanceScale, 1f), 0.0f);
      this.mFireRenderData[(int) iDataChannel].mTransform = result1;
      this.mFireRenderData[(int) iDataChannel].mScroll = this.mLanceTime;
      this.mFireRenderData[(int) iDataChannel].mSize = result14;
      this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, (IRenderableAdditiveObject) this.mFireRenderData[(int) iDataChannel]);
    }
    this.UpdateStatusEffects(iDeltaTime);
    this.UpdateDamage(iDeltaTime);
  }

  private IBossState<Fafnir> GetState(Fafnir.State iState)
  {
    switch (iState)
    {
      case Fafnir.State.Sleep:
        return (IBossState<Fafnir>) this.mSleepState;
      case Fafnir.State.Intro:
        return (IBossState<Fafnir>) this.mIntroState;
      case Fafnir.State.Idle:
        return (IBossState<Fafnir>) this.mIdleState;
      case Fafnir.State.Decision:
        return (IBossState<Fafnir>) this.mDecisionState;
      case Fafnir.State.Tail:
        return (IBossState<Fafnir>) this.mTailState;
      case Fafnir.State.Wing:
        return (IBossState<Fafnir>) this.mWingState;
      case Fafnir.State.Confuse:
        return (IBossState<Fafnir>) this.mConfuseState;
      case Fafnir.State.Fireball:
        return (IBossState<Fafnir>) this.mFireballState;
      case Fafnir.State.LanceHigh:
        return (IBossState<Fafnir>) this.mFirelanceHighState;
      case Fafnir.State.LanceLow:
        return (IBossState<Fafnir>) this.mFirelanceLowState;
      case Fafnir.State.Ceiling:
        return (IBossState<Fafnir>) this.mCeilingState;
      case Fafnir.State.EarthQuake:
        return (IBossState<Fafnir>) this.mEarthQuakeState;
      case Fafnir.State.Defeated:
        return (IBossState<Fafnir>) this.mDefeatedState;
      default:
        return (IBossState<Fafnir>) null;
    }
  }

  public unsafe void ChangeState(Fafnir.State iState)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    IBossState<Fafnir> state = this.GetState(iState);
    if (state == null)
      return;
    if (NetworkManager.Instance.State == NetworkState.Server)
    {
      Fafnir.ChangeStateMessage changeStateMessage;
      changeStateMessage.NewState = iState;
      BossFight.Instance.SendMessage<Fafnir.ChangeStateMessage>((IBoss) this, (ushort) 1, (void*) &changeStateMessage, true);
    }
    this.mCurrentState.OnExit(this);
    this.mPreviousState = this.mCurrentState;
    this.mCurrentState = state;
    this.mCurrentState.OnEnter(this);
  }

  private unsafe void SelectTarget()
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    int num = Fafnir.sRandom.Next(4);
    for (int index = 0; index < this.mPlayers.Length; ++index)
    {
      if (this.mPlayers[(index + num) % 4].Playing)
      {
        Player mPlayer = this.mPlayers[(index + num) % 4];
        if (mPlayer.Avatar != null && !mPlayer.Avatar.Dead)
        {
          this.mTarget = (Magicka.GameLogic.Entities.Character) mPlayer.Avatar;
          break;
        }
      }
    }
    if (this.mTarget != null)
      this.mAimTargetPosition = this.mTarget.Position;
    if (NetworkManager.Instance.State != NetworkState.Server)
      return;
    Fafnir.ChangeTargetMessage changeTargetMessage;
    changeTargetMessage.Target = this.mTarget == null ? ushort.MaxValue : this.mTarget.Handle;
    BossFight.Instance.SendMessage<Fafnir.ChangeTargetMessage>((IBoss) this, (ushort) 2, (void*) &changeTargetMessage, true);
  }

  protected void SetEarthQuakeThreshold()
  {
    this.mEarthQuakeThreshold = (float) ((double) this.MaxHitPoints * (double) (5 - (1 + this.mNrOfEarthquakes)) * 0.20000000298023224);
    this.mEarthQuakeThreshold += 2f;
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

  public override bool Dead => this.mDead;

  public float MaxHitPoints => 35000f;

  public float HitPoints => this.mHitPoints;

  public DamageResult Damage(
    int iPartIndex,
    Magicka.GameLogic.Damage iDamage,
    Magicka.GameLogic.Entities.Entity iAttacker,
    ref Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
    if (this.mCurrentState is Fafnir.SleepState || this.mCurrentState is Fafnir.IntroState || iAttacker is BossDamageZone || this.Dead | iPartIndex != 1)
      return DamageResult.Deflected;
    DamageResult damageResult = this.Damage(iDamage, iAttacker, iAttackPosition, iFeatures);
    if ((damageResult & DamageResult.Hit) == DamageResult.Hit | (damageResult & DamageResult.Damaged) == DamageResult.Damaged)
      this.mDamageFlashTimer = 0.1f;
    if ((double) this.mHitPoints < 0.0)
      this.mHitPoints = 1f;
    return damageResult;
  }

  public void Damage(int iPartIndex, float iDamage, Elements iElement)
  {
    this.Damage(iDamage, iElement);
  }

  public void SetSlow(int iIndex) => throw new NotImplementedException();

  public void GetRandomPositionOnCollisionSkin(int iIndex, out Vector3 oPosition)
  {
    throw new NotImplementedException();
  }

  public bool HasStatus(int iIndex, StatusEffects iStatus) => this.HasStatus(iStatus);

  public float StatusMagnitude(int iIndex, StatusEffects iStatus) => this.StatusMagnitude(iStatus);

  protected override int BloodEffect => Fafnir.BLOOD_BLACK_EFFECT;

  protected override BossDamageZone Entity => this.mHeadZone;

  protected override float Radius
  {
    get => (this.mBodyZone.Body.CollisionSkin.GetPrimitiveLocal(0) as Capsule).Radius;
  }

  protected override float Length
  {
    get => (this.mBodyZone.Body.CollisionSkin.GetPrimitiveLocal(0) as Capsule).Length;
  }

  protected override Vector3 NotifierTextPostion
  {
    get
    {
      Vector3 result = this.mHeadZone.Position;
      Vector3 vector3 = new Vector3(0.0f, 3f, 0.0f);
      Vector3.Add(ref vector3, ref result, out result);
      return result;
    }
  }

  public void ScriptMessage(BossMessages iMessage)
  {
    if (iMessage != BossMessages.FafnirFight)
      return;
    this.ChangeState(Fafnir.State.Decision);
  }

  private unsafe void NetworkUpdate()
  {
    if (!(NetworkManager.Instance.Interface is NetworkServer networkServer))
      return;
    Fafnir.UpdateMessage updateMessage = new Fafnir.UpdateMessage();
    updateMessage.Animation = (byte) 0;
    while ((int) updateMessage.Animation < this.mAnimationClips.Length && this.mAnimationController.AnimationClip != this.mAnimationClips[(int) updateMessage.Animation])
      ++updateMessage.Animation;
    updateMessage.AnimationTime = this.mAnimationController.Time;
    updateMessage.Hitpoints = this.mHitPoints;
    updateMessage.AimForTarget = this.mAimForTarget;
    updateMessage.AimForTargetWeight = this.mAimForTargetWeight;
    updateMessage.AimTargetPosition = this.mAimTargetPosition;
    for (int index = 0; index < networkServer.Connections; ++index)
    {
      float num = networkServer.GetLatency(index) * 0.5f;
      updateMessage.AnimationTime += num;
      BossFight.Instance.SendMessage<Fafnir.UpdateMessage>((IBoss) this, (ushort) 0, (void*) &updateMessage, false, index);
    }
  }

  public unsafe void NetworkUpdate(ref BossUpdateMessage iMsg)
  {
    if (iMsg.Type == (ushort) 0)
    {
      if ((double) iMsg.TimeStamp < (double) this.mLastNetworkUpdate)
        return;
      this.mLastNetworkUpdate = (float) iMsg.TimeStamp;
      Fafnir.UpdateMessage updateMessage;
      BossUpdateMessage.ConvertTo(ref iMsg, (void*) &updateMessage);
      if (this.mAnimationController.AnimationClip != this.mAnimationClips[(int) updateMessage.Animation])
        this.mAnimationController.StartClip(this.mAnimationClips[(int) updateMessage.Animation], false);
      this.mAnimationController.Time = updateMessage.AnimationTime;
      this.mHitPoints = updateMessage.Hitpoints;
      this.mAimForTarget = updateMessage.AimForTarget;
      this.mAimForTargetWeight = updateMessage.AimForTargetWeight;
      this.mAimTargetPosition = updateMessage.AimTargetPosition;
    }
    else if (iMsg.Type == (ushort) 2)
    {
      Fafnir.ChangeTargetMessage changeTargetMessage;
      BossUpdateMessage.ConvertTo(ref iMsg, (void*) &changeTargetMessage);
      if (changeTargetMessage.Target == ushort.MaxValue)
      {
        this.mTarget = (Magicka.GameLogic.Entities.Character) null;
      }
      else
      {
        this.mTarget = Magicka.GameLogic.Entities.Entity.GetFromHandle((int) changeTargetMessage.Target) as Magicka.GameLogic.Entities.Character;
        this.mAimTargetPosition = this.mTarget.Position;
      }
    }
    else
    {
      if (iMsg.Type != (ushort) 1)
        return;
      Fafnir.ChangeStateMessage changeStateMessage;
      BossUpdateMessage.ConvertTo(ref iMsg, (void*) &changeStateMessage);
      IBossState<Fafnir> state = this.GetState(changeStateMessage.NewState);
      if (state == null)
        return;
      this.mCurrentState.OnExit(this);
      this.mPreviousState = this.mCurrentState;
      this.mCurrentState = state;
      this.mCurrentState.OnEnter(this);
    }
  }

  public void NetworkInitialize(ref BossInitializeMessage iMsg)
  {
    throw new NotImplementedException();
  }

  BossEnum IBoss.GetBossType() => BossEnum.Fafnir;

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

  private enum MessageType : ushort
  {
    Update,
    ChangeState,
    ChangeTarget,
  }

  internal struct UpdateMessage
  {
    public const ushort TYPE = 0;
    public float AimForTargetWeight;
    public Vector3 AimTargetPosition;
    public bool AimForTarget;
    public byte Animation;
    public float AnimationTime;
    public float Hitpoints;
  }

  internal struct ChangeStateMessage
  {
    public const ushort TYPE = 1;
    public Fafnir.State NewState;
  }

  internal struct ChangeTargetMessage
  {
    public const ushort TYPE = 2;
    public ushort Target;
  }

  private enum Sounds : byte
  {
    Attack,
    Breakfloor,
    Confuse,
    Death,
    Deathray,
    Pain,
  }

  private enum Animations
  {
    Invalid,
    ceiling,
    charm,
    defeated,
    fireball,
    fire_high,
    fire_low,
    idle,
    intro,
    sleep,
    tailwhip,
    wings,
    NrOfAnimations,
  }

  protected class RenderData : 
    RenderableObject<SkinnedModelDeferredEffect, SkinnedModelDeferredBasicMaterial>
  {
    public float Flash;
    public float mDamage;
    public Matrix[] mSkeleton;

    public RenderData() => this.mSkeleton = new Matrix[80 /*0x50*/];

    public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      SkinnedModelDeferredEffect modelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
      modelDeferredEffect.OverrideColor = new Vector4(1f, 1f, 1f, this.Flash);
      modelDeferredEffect.Damage = this.mDamage;
      modelDeferredEffect.ProjectionMapEnabled = false;
      this.mMaterial.Damage = this.mDamage;
      modelDeferredEffect.Bones = this.mSkeleton;
      base.Draw(iEffect, iViewFrustum);
      modelDeferredEffect.OverrideColor = Vector4.Zero;
    }

    public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      (iEffect as SkinnedModelDeferredEffect).Bones = this.mSkeleton;
      base.DrawShadow(iEffect, iViewFrustum);
    }
  }

  protected class FireRenderData : IRenderableAdditiveObject
  {
    private int mVerticesHash;
    private VertexBuffer mVertexBuffer;
    private VertexDeclaration mVertexDeclaration;
    public Matrix mTransform;
    public AdditiveMaterial mMaterial;
    public float mScroll;
    public float mSize;
    private Matrix mRotation;

    public FireRenderData(VertexBuffer iVertexBuffer, VertexDeclaration iDeclaration)
    {
      this.mRotation = Matrix.CreateRotationY(3.14159274f);
      this.mVerticesHash = iVertexBuffer.GetHashCode();
      this.mVertexBuffer = iVertexBuffer;
      this.mVertexDeclaration = iDeclaration;
    }

    public int Effect => AdditiveEffect.TYPEHASH;

    public int Technique => 0;

    public VertexBuffer Vertices => this.mVertexBuffer;

    public int VerticesHashCode => this.mVerticesHash;

    public int VertexStride => VertexPositionColorTexture.SizeInBytes;

    public IndexBuffer Indices => (IndexBuffer) null;

    public VertexDeclaration VertexDeclaration => this.mVertexDeclaration;

    public bool Cull(BoundingFrustum iViewFrustum) => false;

    public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      AdditiveEffect iEffect1 = iEffect as AdditiveEffect;
      this.mMaterial.AssignToEffect(iEffect1);
      iEffect1.TextureEnabled = true;
      Matrix mTransform1 = this.mTransform;
      mTransform1.M11 *= 2f;
      mTransform1.M12 *= 2f;
      mTransform1.M13 *= 2f;
      mTransform1.M31 *= 16f;
      mTransform1.M32 *= 16f;
      mTransform1.M33 *= 16f;
      iEffect1.World = mTransform1;
      iEffect1.TextureScale = new Vector2(1f, this.mSize);
      iEffect1.TextureOffset = new Vector2(0.0f, this.mScroll * 10f);
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 2);
      Matrix mTransform2 = this.mTransform;
      mTransform2.M11 *= 2f;
      mTransform2.M12 *= 2f;
      mTransform2.M13 *= 2f;
      mTransform2.M31 *= 16f;
      mTransform2.M32 *= 16f;
      mTransform2.M33 *= 16f;
      iEffect1.World = mTransform2;
      iEffect1.TextureScale = new Vector2(1f, this.mSize);
      iEffect1.TextureOffset = new Vector2(0.5f, this.mScroll * 4f);
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 2);
    }
  }

  public enum State : byte
  {
    Sleep,
    Intro,
    Idle,
    Decision,
    Tail,
    Wing,
    Confuse,
    Fireball,
    LanceHigh,
    LanceLow,
    Ceiling,
    EarthQuake,
    Defeated,
  }

  public interface IFafnirState : IBossState<Fafnir>
  {
    float GetWeight(Fafnir iOwner, float iHealth, float iPlayerWeight);
  }

  public class SleepState : IBossState<Fafnir>
  {
    public void OnEnter(Fafnir iOwner)
    {
      iOwner.mAnimationController.StartClip(iOwner.mAnimationClips[9], true);
    }

    public void OnUpdate(float iDeltaTime, Fafnir iOwner)
    {
    }

    public void OnExit(Fafnir iOwner)
    {
    }
  }

  public class IntroState : IBossState<Fafnir>
  {
    public void OnEnter(Fafnir iOwner)
    {
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[8], 0.5f, false);
    }

    public void OnUpdate(float iDeltaTime, Fafnir iOwner)
    {
      if (iOwner.mAnimationController.IsLooping || !iOwner.mAnimationController.HasFinished || iOwner.mAnimationController.CrossFadeEnabled)
        return;
      iOwner.mPlayState.Level.CurrentScene.ExecuteTrigger(Fafnir.WAKE_TRIGGER, (Magicka.GameLogic.Entities.Character) null, false);
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[7], 0.5f, true);
    }

    public void OnExit(Fafnir iOwner) => iOwner.mAimForTarget = false;
  }

  public class IdleState : IBossState<Fafnir>
  {
    public void OnEnter(Fafnir iOwner)
    {
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[7], 2f, true);
      iOwner.mAimForTarget = false;
    }

    public void OnUpdate(float iDeltaTime, Fafnir iOwner)
    {
      for (int index = 0; index < iOwner.mPlayers.Length; ++index)
      {
        if (iOwner.mPlayers[index].Playing && iOwner.mPlayers[index].Avatar != null && !iOwner.mPlayers[index].Avatar.Dead)
        {
          iOwner.ChangeState(Fafnir.State.Decision);
          break;
        }
      }
    }

    public void OnExit(Fafnir iOwner)
    {
    }
  }

  public class DecisionState : IBossState<Fafnir>
  {
    private float mTimer;

    public void OnEnter(Fafnir iOwner)
    {
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[7], 2f, false);
      this.mTimer = (float) Fafnir.sRandom.NextDouble() * (float) (1.0 - (double) iOwner.mHitPoints * 2.8571428629220463E-05);
      iOwner.SelectTarget();
    }

    public void OnUpdate(float iDeltaTime, Fafnir iOwner)
    {
      this.mTimer -= iDeltaTime;
      if ((double) this.mTimer > 0.0 && (iOwner.mAnimationController.CrossFadeEnabled || !iOwner.mAnimationController.HasFinished))
        return;
      Player[] mPlayers = iOwner.mPlayers;
      bool flag = true;
      for (int index = 0; index < mPlayers.Length; ++index)
      {
        if (mPlayers[index].Playing && mPlayers[index].Avatar != null && !mPlayers[index].Avatar.Dead)
          flag = false;
      }
      if (flag)
        iOwner.ChangeState(Fafnir.State.Idle);
      else if ((double) iOwner.mHitPoints <= (double) iOwner.mEarthQuakeThreshold)
      {
        iOwner.ChangeState(Fafnir.State.EarthQuake);
      }
      else
      {
        float iHealth = iOwner.mHitPoints * 2.85714286E-05f;
        float num1 = 0.0f;
        for (int index = 0; index < mPlayers.Length; ++index)
        {
          if (mPlayers[index].Playing && mPlayers[index].Avatar != null && !mPlayers[index].Avatar.Dead)
            ++num1;
        }
        float iPlayerWeight = 1f / num1;
        float weight1 = iOwner.mWingState.GetWeight(iOwner, iHealth, iPlayerWeight);
        float weight2 = iOwner.mConfuseState.GetWeight(iOwner, iHealth, iPlayerWeight);
        float weight3 = iOwner.mCeilingState.GetWeight(iOwner, iHealth, iPlayerWeight);
        float weight4 = iOwner.mTailState.GetWeight(iOwner, iHealth, iPlayerWeight);
        float weight5 = iOwner.mFireballState.GetWeight(iOwner, iHealth, iPlayerWeight);
        float weight6 = iOwner.mFirelanceHighState.GetWeight(iOwner, iHealth, iPlayerWeight);
        float weight7 = iOwner.mFirelanceLowState.GetWeight(iOwner, iHealth, iPlayerWeight);
        float num2 = weight1 + weight2 + weight3 + weight4 + weight5 + weight6 + weight7;
        float num3 = weight1 / num2;
        float num4 = weight2 / num2;
        float num5 = weight3 / num2;
        float num6 = weight4 / num2;
        float num7 = weight5 / num2;
        float num8 = weight6 / num2;
        float num9 = weight7 / num2;
        if ((double) num3 >= (double) num4 && (double) num3 >= (double) num5 && (double) num3 >= (double) num7 && (double) num3 >= (double) num8 && (double) num3 >= (double) num6 && (double) num3 >= (double) num9)
          iOwner.ChangeState(Fafnir.State.Wing);
        else if ((double) num4 >= (double) num5 && (double) num4 >= (double) num7 && (double) num4 >= (double) num8 && (double) num4 >= (double) num6 && (double) num4 >= (double) num9)
          iOwner.ChangeState(Fafnir.State.Confuse);
        else if ((double) num6 >= (double) num5 && (double) num6 >= (double) num7 && (double) num6 >= (double) num8 && (double) num6 >= (double) num9)
          iOwner.ChangeState(Fafnir.State.Tail);
        else if ((double) num5 >= (double) num7 && (double) num5 >= (double) num8 && (double) num5 >= (double) num9)
          iOwner.ChangeState(Fafnir.State.Ceiling);
        else if ((double) num8 >= (double) num7 && (double) num8 >= (double) num9)
          iOwner.ChangeState(Fafnir.State.LanceHigh);
        else if ((double) num9 >= (double) num7)
          iOwner.ChangeState(Fafnir.State.LanceLow);
        else if (iOwner.mPreviousState != iOwner.mFireballState)
        {
          iOwner.ChangeState(Fafnir.State.Fireball);
        }
        else
        {
          Fafnir.State iState;
          IBossState<Fafnir> bossState;
          do
          {
            switch (Fafnir.sRandom.Next(6))
            {
              case 1:
                iState = Fafnir.State.Ceiling;
                bossState = (IBossState<Fafnir>) iOwner.mCeilingState;
                break;
              case 2:
                iState = Fafnir.State.Tail;
                bossState = (IBossState<Fafnir>) iOwner.mTailState;
                break;
              case 3:
                iState = Fafnir.State.LanceLow;
                bossState = (IBossState<Fafnir>) iOwner.mFirelanceLowState;
                break;
              case 4:
                iState = Fafnir.State.LanceHigh;
                bossState = (IBossState<Fafnir>) iOwner.mFirelanceHighState;
                break;
              case 5:
                iState = Fafnir.State.Confuse;
                bossState = (IBossState<Fafnir>) iOwner.mConfuseState;
                break;
              default:
                iState = Fafnir.State.Wing;
                bossState = (IBossState<Fafnir>) iOwner.mWingState;
                break;
            }
          }
          while (bossState == iOwner.mPreviousState);
          iOwner.ChangeState(iState);
        }
      }
    }

    public void OnExit(Fafnir iOwner) => iOwner.mAimForTarget = false;
  }

  public class TailState : Fafnir.IFafnirState, IBossState<Fafnir>
  {
    private static float WEIGHT_RANGE_SQR = 25f;
    private bool mPlayedSound;

    public void OnEnter(Fafnir iOwner)
    {
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[10], 0.5f, false);
      iOwner.mAimForTarget = false;
      this.mPlayedSound = false;
    }

    public void OnUpdate(float iDeltaTime, Fafnir iOwner)
    {
      if (iOwner.mAnimationController.CrossFadeEnabled)
        return;
      float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
      if (iOwner.mAnimationController.HasFinished)
        iOwner.ChangeState(Fafnir.State.Decision);
      if (this.mPlayedSound || (double) num <= 0.5)
        return;
      this.mPlayedSound = true;
      AudioManager.Instance.PlayCue(Banks.Characters, Fafnir.SOUNDS[0], iOwner.mHeadZone.AudioEmitter);
    }

    public void OnExit(Fafnir iOwner)
    {
    }

    public float GetWeight(Fafnir iOwner, float iHealth, float iPlayerWeight)
    {
      float weight = 0.0f;
      if (iOwner.mPreviousState != this)
      {
        Vector3 result1 = iOwner.mTailBindPoses[6].Translation;
        Matrix skinnedBoneTransform = iOwner.mAnimationController.SkinnedBoneTransforms[iOwner.mTailIndices[6]];
        Vector3.Transform(ref result1, ref skinnedBoneTransform, out result1);
        result1.Y = 0.0f;
        Player[] mPlayers = iOwner.mPlayers;
        for (int index = 0; index < mPlayers.Length; ++index)
        {
          if (mPlayers[index].Playing && mPlayers[index].Avatar != null && !mPlayers[index].Avatar.Dead)
          {
            Vector3 position = mPlayers[index].Avatar.Position with
            {
              Y = 0.0f
            };
            float result2;
            Vector3.DistanceSquared(ref result1, ref position, out result2);
            if ((double) result2 <= (double) Fafnir.TailState.WEIGHT_RANGE_SQR)
              weight += iPlayerWeight;
          }
        }
      }
      return weight;
    }
  }

  public class WingState : Fafnir.IFafnirState, IBossState<Fafnir>
  {
    private static readonly Magicka.GameLogic.Damage sPush = new Magicka.GameLogic.Damage(AttackProperties.Pushed, Elements.Earth, 150f, 1f);
    private bool mPlayedFirstSound;
    private bool mPlayedSecondSound;

    public void OnEnter(Fafnir iOwner)
    {
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[11], 0.5f, false);
      iOwner.mAimForTarget = false;
      this.mPlayedFirstSound = false;
      this.mPlayedSecondSound = false;
    }

    public void OnUpdate(float iDeltaTime, Fafnir iOwner)
    {
      float num1 = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
      if (iOwner.mAnimationController.HasFinished && !iOwner.mAnimationController.CrossFadeEnabled)
      {
        iOwner.ChangeState(Fafnir.State.Decision);
      }
      else
      {
        if ((iOwner.mAnimationController.CrossFadeEnabled || (double) num1 < (double) Fafnir.ANIMATION_TIMES[11][0] || (double) num1 > (double) Fafnir.ANIMATION_TIMES[11][1]) && ((double) num1 < (double) Fafnir.ANIMATION_TIMES[11][2] || (double) num1 > (double) Fafnir.ANIMATION_TIMES[11][3]))
          return;
        if ((double) num1 <= (double) Fafnir.ANIMATION_TIMES[11][1] && !this.mPlayedFirstSound)
        {
          AudioManager.Instance.PlayCue(Banks.Characters, Fafnir.SOUNDS[0], iOwner.mHeadZone.AudioEmitter);
          this.mPlayedFirstSound = true;
        }
        else if ((double) num1 >= (double) Fafnir.ANIMATION_TIMES[11][2] && !this.mPlayedSecondSound)
        {
          AudioManager.Instance.PlayCue(Banks.Characters, Fafnir.SOUNDS[0], iOwner.mHeadZone.AudioEmitter);
          this.mPlayedSecondSound = true;
        }
        Vector3 spineBasePosition = iOwner.mSpineBasePosition;
        List<Magicka.GameLogic.Entities.Entity> entities = iOwner.mPlayState.EntityManager.GetEntities(spineBasePosition, 30f, true);
        entities.Remove((Magicka.GameLogic.Entities.Entity) iOwner.mHeadZone);
        entities.Remove((Magicka.GameLogic.Entities.Entity) iOwner.mBodyZone);
        entities.Remove((Magicka.GameLogic.Entities.Entity) iOwner.mRightHeelZone);
        entities.Remove((Magicka.GameLogic.Entities.Entity) iOwner.mRightWristZone);
        entities.Remove((Magicka.GameLogic.Entities.Entity) iOwner.mTailZone);
        Vector3 backward = Vector3.Backward;
        for (int index = 0; index < entities.Count; ++index)
        {
          if (entities[index] is IDamageable && !iOwner.mHitList.ContainsKey(entities[index].Handle))
          {
            Vector3 position1 = entities[index].Position;
            Vector3 result1;
            Vector3.Subtract(ref position1, ref spineBasePosition, out result1);
            result1.Y = 0.0f;
            result1.Normalize();
            float result2;
            Vector3.Dot(ref result1, ref backward, out result2);
            if ((double) result2 > 0.5)
            {
              Vector3 position2 = entities[index].Position;
              position2.Z -= 2f;
              int num2 = (int) (entities[index] as IDamageable).Damage(Fafnir.WingState.sPush, (Magicka.GameLogic.Entities.Entity) iOwner.mHeadZone, 0.0, position2);
              iOwner.mHitList.Add(entities[index].Handle, 0.06125f);
            }
          }
        }
        iOwner.mPlayState.EntityManager.ReturnEntityList(entities);
      }
    }

    public void OnExit(Fafnir iOwner)
    {
    }

    public float GetWeight(Fafnir iOwner, float iHealth, float iPlayerWeight)
    {
      float weight = 0.0f;
      if (iOwner.mPreviousState != this)
      {
        Vector3 spineBasePosition = iOwner.mSpineBasePosition;
        Vector3 backward = Vector3.Backward;
        Player[] mPlayers = iOwner.mPlayers;
        for (int index = 0; index < mPlayers.Length; ++index)
        {
          if (mPlayers[index].Playing && mPlayers[index].Avatar != null && !mPlayers[index].Avatar.Dead)
          {
            Vector3 position = mPlayers[index].Avatar.Position;
            Vector3 result1;
            Vector3.Subtract(ref position, ref spineBasePosition, out result1);
            result1.Y = 0.0f;
            result1.Normalize();
            float result2;
            Vector3.Distance(ref position, ref spineBasePosition, out result2);
            float result3;
            Vector3.Dot(ref result1, ref backward, out result3);
            if ((double) result3 > 0.5 && (double) result2 < 10.0)
              weight += (float) (0.5 * (double) iPlayerWeight + 0.75 * (double) iPlayerWeight * (double) result3);
          }
        }
      }
      return weight;
    }
  }

  public class ConfuseState : Fafnir.IFafnirState, IBossState<Fafnir>
  {
    private VisualEffectReference mLeftCharmEffect;
    private VisualEffectReference mRightCharmEffect;
    private static readonly int CHARM_EFFECT = "fafnir_charm".GetHashCodeCustom();
    public bool mInverted;

    public void OnEnter(Fafnir iOwner)
    {
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[2], 0.5f, false);
      AudioManager.Instance.PlayCue(Banks.Characters, Fafnir.SOUNDS[2], iOwner.mHeadZone.AudioEmitter);
      iOwner.mAimForTarget = true;
      EffectManager.Instance.StartEffect(Fafnir.ConfuseState.CHARM_EFFECT, ref iOwner.mRightEyeOrientation, out this.mRightCharmEffect);
      EffectManager.Instance.StartEffect(Fafnir.ConfuseState.CHARM_EFFECT, ref iOwner.mLeftEyeOrientation, out this.mLeftCharmEffect);
    }

    public void OnUpdate(float iDeltaTime, Fafnir iOwner)
    {
      EffectManager.Instance.UpdateOrientation(ref this.mLeftCharmEffect, ref iOwner.mLeftEyeOrientation);
      EffectManager.Instance.UpdateOrientation(ref this.mRightCharmEffect, ref iOwner.mRightEyeOrientation);
      if (iOwner.mAnimationController.CrossFadeEnabled)
        return;
      float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
      if (!this.mInverted && (double) num >= (double) Fafnir.ANIMATION_TIMES[2][0] && (double) num <= (double) Fafnir.ANIMATION_TIMES[2][1])
      {
        Player[] players = Magicka.Game.Instance.Players;
        for (int index = 0; index < players.Length; ++index)
        {
          if (players[index].Playing && players[index].Avatar != null && !players[index].Avatar.Dead && !(players[index].Gamer is NetworkGamer))
          {
            players[index].Controller.Invert(true);
            this.mInverted = true;
          }
        }
        iOwner.mConfusedTimer = 20f;
      }
      else
      {
        if (!iOwner.mAnimationController.HasFinished)
          return;
        iOwner.ChangeState(Fafnir.State.Decision);
      }
    }

    public void OnExit(Fafnir iOwner)
    {
      iOwner.mAimForTarget = false;
      EffectManager.Instance.Stop(ref this.mLeftCharmEffect);
      EffectManager.Instance.Stop(ref this.mRightCharmEffect);
    }

    public float GetWeight(Fafnir iOwner, float iHealth, float iPlayerWeight)
    {
      float weight = 0.0f;
      if ((double) iHealth <= 0.60000002384185791 && iOwner.mPreviousState != this && !this.mInverted)
        weight = (float) Fafnir.sRandom.NextDouble();
      return weight;
    }
  }

  public class FireballState : Fafnir.IFafnirState, IBossState<Fafnir>
  {
    public static readonly int FIREBALL_TRAIL_EFFECT = "fafnir_fireball_trail".GetHashCodeCustom();
    public static readonly int FIREBALL_HIT_EFFECT = "magick_meteor_explosion".GetHashCodeCustom();
    public static readonly int FIREBALL_SPLASH_EFFECT = "fafnir_fireball_splash".GetHashCodeCustom();
    public static readonly int FIREBALL_TRAIL_SOUND = "spell_fire_projectile".GetHashCodeCustom();
    public static readonly int FIREBALL_HIT_SOUND = "magick_meteor_blast".GetHashCodeCustom();
    private bool mFireballShot;
    private ConditionCollection mFireballConditions;

    public FireballState()
    {
      float iRadius = 6f;
      DamageCollection5 damageCollection5 = new DamageCollection5();
      damageCollection5.AddDamage(new Magicka.GameLogic.Damage(AttackProperties.Damage, Elements.Fire, 300f, 1.5f));
      damageCollection5.AddDamage(new Magicka.GameLogic.Damage(AttackProperties.Knockback, Elements.Fire, 300f, 5f));
      damageCollection5.AddDamage(new Magicka.GameLogic.Damage(AttackProperties.Status, Elements.Fire, 200f, 3f));
      this.mFireballConditions = new ConditionCollection();
      this.mFireballConditions[0].Condition.EventConditionType = EventConditionType.Default;
      this.mFireballConditions[0].Condition.Repeat = true;
      this.mFireballConditions[0].Add(new EventStorage(new PlayEffectEvent(Fafnir.FireballState.FIREBALL_TRAIL_EFFECT, true)));
      this.mFireballConditions[0].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Fafnir.FireballState.FIREBALL_TRAIL_SOUND, true)));
      this.mFireballConditions[1].Condition.EventConditionType = EventConditionType.Hit;
      this.mFireballConditions[1].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Fafnir.FireballState.FIREBALL_HIT_SOUND)));
      this.mFireballConditions[1].Add(new EventStorage(new PlayEffectEvent(Fafnir.FireballState.FIREBALL_HIT_EFFECT)));
      this.mFireballConditions[1].Add(new EventStorage(new PlayEffectEvent(Fafnir.FireballState.FIREBALL_SPLASH_EFFECT)));
      this.mFireballConditions[1].Add(new EventStorage(new RemoveEvent()));
      this.mFireballConditions[1].Add(new EventStorage(new SplashEvent(damageCollection5.A, iRadius)));
      this.mFireballConditions[1].Add(new EventStorage(new SplashEvent(damageCollection5.B, iRadius)));
      this.mFireballConditions[1].Add(new EventStorage(new SplashEvent(damageCollection5.C, iRadius)));
      this.mFireballConditions[2].Condition.EventConditionType = EventConditionType.Collision;
      this.mFireballConditions[2].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Fafnir.FireballState.FIREBALL_HIT_SOUND)));
      this.mFireballConditions[2].Add(new EventStorage(new PlayEffectEvent(Fafnir.FireballState.FIREBALL_HIT_EFFECT)));
      this.mFireballConditions[2].Add(new EventStorage(new PlayEffectEvent(Fafnir.FireballState.FIREBALL_SPLASH_EFFECT)));
      this.mFireballConditions[2].Add(new EventStorage(new RemoveEvent()));
      this.mFireballConditions[2].Add(new EventStorage(new SplashEvent(damageCollection5.A, iRadius)));
      this.mFireballConditions[2].Add(new EventStorage(new SplashEvent(damageCollection5.B, iRadius)));
      this.mFireballConditions[2].Add(new EventStorage(new SplashEvent(damageCollection5.C, iRadius)));
      this.mFireballConditions[3].Condition.EventConditionType = EventConditionType.Timer;
      this.mFireballConditions[3].Add(new EventStorage(new RemoveEvent()));
      this.mFireballConditions[3].Condition.Time = 8f;
    }

    public void OnEnter(Fafnir iOwner)
    {
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[4], 0.4f, false);
      this.mFireballShot = false;
      iOwner.mAimForTarget = true;
      iOwner.SelectTarget();
    }

    public void OnUpdate(float iDeltaTime, Fafnir iOwner)
    {
      if (iOwner.mTarget == null || iOwner.mTarget.Dead)
      {
        iOwner.ChangeState(Fafnir.State.Decision);
      }
      else
      {
        float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
        if ((double) num >= (double) Fafnir.ANIMATION_TIMES[4][0] && (double) num <= (double) Fafnir.ANIMATION_TIMES[4][1] && !iOwner.mAnimationController.CrossFadeEnabled && !this.mFireballShot)
        {
          AudioManager.Instance.PlayCue(Banks.Characters, Fafnir.SOUNDS[0], iOwner.mHeadZone.AudioEmitter);
          MissileEntity instance = MissileEntity.GetInstance(iOwner.mPlayState);
          Vector3 result1 = iOwner.mHeadZone.Position;
          Vector3 result2 = Vector3.UnitZ;
          Matrix orientation = iOwner.mHeadZone.Body.Orientation;
          Vector3.Transform(ref result2, ref orientation, out result2);
          Vector3 result3 = iOwner.mHeadZone.Body.Orientation.Forward;
          Vector3.Add(ref result1, ref result3, out result1);
          Vector3.Add(ref result1, ref result3, out result1);
          Vector3.Multiply(ref result3, 30f, out result3);
          instance.Initialize((Magicka.GameLogic.Entities.Entity) iOwner.mHeadZone, 0.25f, ref result1, ref result3, (Model) null, this.mFireballConditions, false);
          instance.Body.ApplyGravity = false;
          iOwner.mPlayState.EntityManager.AddEntity((Magicka.GameLogic.Entities.Entity) instance);
          this.mFireballShot = true;
        }
        else
        {
          if (!iOwner.mAnimationController.HasFinished || iOwner.mAnimationController.CrossFadeEnabled)
            return;
          iOwner.ChangeState(Fafnir.State.Decision);
        }
      }
    }

    public void OnExit(Fafnir iOwner) => iOwner.mAimForTarget = false;

    public float GetWeight(Fafnir iOwner, float iHealth, float iPlayerWeight)
    {
      float weight = 0.0f;
      if (iOwner.mPreviousState != this)
        weight = (float) Fafnir.sRandom.NextDouble();
      return weight;
    }
  }

  public class FirelanceHighState : Fafnir.IFafnirState, IBossState<Fafnir>
  {
    private static readonly int LANCE_GROUND_SPLASH = "fafnir_lance_ground_splash".GetHashCodeCustom();
    private static readonly int LANCE_LAVA_SPLASH = "fafnir_lance_lava_splash".GetHashCodeCustom();
    private static readonly int LANCE_HIT = "fafnir_lance_hit".GetHashCodeCustom();
    private static readonly int LANCE_WELD = "fafnir_lance_weld".GetHashCodeCustom();
    private Cue mLanceCue;
    private float mDamageTimer;
    private float mLanceTime;
    private float mTotalLanceTime;
    private DamageCollection5 mDamage;
    private VisualEffectReference mFireBreath;
    private VisualEffectReference mWeldEffect;

    public FirelanceHighState()
    {
      this.mDamage = new DamageCollection5();
      this.mDamage.AddDamage(new Magicka.GameLogic.Damage(AttackProperties.Damage, Elements.Fire, 200f, 1f));
      this.mDamage.AddDamage(new Magicka.GameLogic.Damage(AttackProperties.Status, Elements.Fire, 200f, 3f));
      this.mDamage.AddDamage(new Magicka.GameLogic.Damage(AttackProperties.Pushed, Elements.Fire, 300f, 3f));
    }

    public void OnEnter(Fafnir iOwner)
    {
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[5], 0.5f, false);
      iOwner.mAimForTarget = false;
      iOwner.mDrawFireLance = false;
      iOwner.mLanceScale = 0.0f;
      this.mLanceTime = 0.0f;
      this.mDamageTimer = 0.5f;
      float duration = iOwner.mAnimationClips[5].Duration;
      this.mTotalLanceTime = (Fafnir.ANIMATION_TIMES[5][1] - Fafnir.ANIMATION_TIMES[5][0]) * duration;
    }

    public void OnUpdate(float iDeltaTime, Fafnir iOwner)
    {
      if (iOwner.mTarget == null || iOwner.mTarget.Dead)
      {
        iOwner.ChangeState(Fafnir.State.Decision);
      }
      else
      {
        float num1 = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
        if ((double) num1 >= (double) Fafnir.ANIMATION_TIMES[5][0] && (double) num1 <= (double) Fafnir.ANIMATION_TIMES[5][1] && !iOwner.mAnimationController.CrossFadeEnabled)
        {
          if (this.mLanceCue == null)
            this.mLanceCue = AudioManager.Instance.PlayCue(Banks.Characters, Fafnir.SOUNDS[4], iOwner.mLanceAudioEmitter);
          this.mLanceTime += iDeltaTime;
          this.mDamageTimer -= iDeltaTime;
          iOwner.mAimForTarget = true;
          float val1 = (float) (2.0 - 32.0 * System.Math.Pow((double) (this.mLanceTime / this.mTotalLanceTime) - 0.5, 4.0));
          iOwner.mLanceScale = System.Math.Min(val1, 1f);
          iOwner.mDrawFireLance = true;
          Vector3 delta = iOwner.mLanceSegment.Delta;
          delta.Normalize();
          if (!EffectManager.Instance.UpdatePositionDirection(ref this.mFireBreath, ref iOwner.mLanceSegment.Origin, ref delta))
            EffectManager.Instance.StartEffect(Fafnir.FIRE_SPRAY_EFFECT, ref iOwner.mLanceSegment.Origin, ref delta, out this.mFireBreath);
          Vector3 right = Vector3.Right;
          Vector3 result;
          Vector3.Add(ref iOwner.mLanceSegment.Origin, ref iOwner.mLanceSegment.Delta, out result);
          if (!EffectManager.Instance.UpdatePositionDirection(ref this.mWeldEffect, ref result, ref right))
            EffectManager.Instance.StartEffect(Fafnir.FirelanceHighState.LANCE_WELD, ref result, ref right, out this.mWeldEffect);
          if ((double) this.mDamageTimer <= 0.0)
          {
            this.mDamageTimer = 0.25f;
            int num2 = (int) Helper.CircleDamage(iOwner.mPlayState, (Magicka.GameLogic.Entities.Entity) iOwner.mHeadZone, 0.0, (Magicka.GameLogic.Entities.Entity) iOwner.mTailZone, ref result, 2f, ref this.mDamage);
          }
        }
        else if ((double) num1 > (double) Fafnir.ANIMATION_TIMES[5][1])
        {
          if (this.mLanceCue != null && !this.mLanceCue.IsStopping)
          {
            this.mLanceCue.Stop(AudioStopOptions.AsAuthored);
            this.mLanceCue = (Cue) null;
          }
          iOwner.mDrawFireLance = false;
          EffectManager.Instance.Stop(ref this.mFireBreath);
          EffectManager.Instance.Stop(ref this.mWeldEffect);
        }
        if (!iOwner.mAnimationController.HasFinished || iOwner.mAnimationController.CrossFadeEnabled)
          return;
        iOwner.ChangeState(Fafnir.State.Decision);
      }
    }

    public void OnExit(Fafnir iOwner)
    {
      if (this.mLanceCue != null && !this.mLanceCue.IsStopping)
      {
        this.mLanceCue.Stop(AudioStopOptions.AsAuthored);
        this.mLanceCue = (Cue) null;
      }
      iOwner.mDrawFireLance = false;
      EffectManager.Instance.Stop(ref this.mFireBreath);
    }

    public float GetWeight(Fafnir iOwner, float iHealth, float iPlayerWeight)
    {
      float weight = 0.0f;
      if (iOwner.mPreviousState != this)
        weight = (float) Fafnir.sRandom.NextDouble();
      return weight;
    }
  }

  public class FirelanceLowState : Fafnir.IFafnirState, IBossState<Fafnir>
  {
    private const float MIN_HEAD_RANGE_SQR = 9f;
    private const float MAX_HEAD_RANGE_SQR = 73f;
    private const float HEAD_RANGE_SQR = 64f;
    private float mDamageTimer;
    private float mTotalLanceTime;
    private DamageCollection5 mDamage;
    private VisualEffectReference mFireBreath;

    public FirelanceLowState()
    {
      this.mDamage = new DamageCollection5();
      this.mDamage.AddDamage(new Magicka.GameLogic.Damage(AttackProperties.Damage, Elements.Fire, 200f, 1f));
      this.mDamage.AddDamage(new Magicka.GameLogic.Damage(AttackProperties.Status, Elements.Fire, 200f, 3f));
      this.mDamage.AddDamage(new Magicka.GameLogic.Damage(AttackProperties.Pushed, Elements.Fire, 300f, 3f));
    }

    public void OnEnter(Fafnir iOwner)
    {
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[6], 0.5f, false);
      iOwner.mDrawFireLance = false;
      iOwner.mAimForTarget = false;
      this.mDamageTimer = 0.5f;
      this.mFireBreath.ID = -1;
      float duration = iOwner.mAnimationClips[6].Duration;
      this.mTotalLanceTime = (Fafnir.ANIMATION_TIMES[6][1] - Fafnir.ANIMATION_TIMES[6][0]) * duration;
    }

    public void OnUpdate(float iDeltaTime, Fafnir iOwner)
    {
      if (iOwner.mTarget == null || iOwner.mTarget.Dead)
      {
        iOwner.ChangeState(Fafnir.State.Decision);
      }
      else
      {
        float num1 = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
        if (iOwner.mAnimationController.CrossFadeEnabled)
          return;
        if ((double) num1 >= (double) Fafnir.ANIMATION_TIMES[6][0] && (double) num1 <= (double) Fafnir.ANIMATION_TIMES[6][1] && !iOwner.mAnimationController.CrossFadeEnabled)
        {
          this.mDamageTimer -= iDeltaTime;
          Vector3 forward = iOwner.mHeadZone.Body.Orientation.Forward;
          forward.Normalize();
          Vector3 result = iOwner.mMouthOrientation.Translation;
          Vector3.Add(ref result, ref forward, out result);
          if (!EffectManager.Instance.UpdatePositionDirection(ref this.mFireBreath, ref result, ref forward))
            EffectManager.Instance.StartEffect(Fafnir.FIRE_BREATH_EFFECT, ref result, ref forward, out this.mFireBreath);
          if ((double) this.mDamageTimer <= 0.0)
          {
            int num2 = (int) Helper.ArcDamage(iOwner.mPlayState, (Magicka.GameLogic.Entities.Entity) iOwner.mHeadZone, 0.0, (Magicka.GameLogic.Entities.Entity) iOwner.mHeadZone, ref result, ref forward, 20f, 0.7853982f, ref this.mDamage);
            this.mDamageTimer += 0.25f;
          }
        }
        else if (this.mFireBreath.ID != -1 && (double) num1 > (double) Fafnir.ANIMATION_TIMES[6][1])
        {
          iOwner.mDrawFireLance = false;
          EffectManager.Instance.Stop(ref this.mFireBreath);
        }
        if (!iOwner.mAnimationController.HasFinished)
          return;
        iOwner.ChangeState(Fafnir.State.Decision);
      }
    }

    public void OnExit(Fafnir iOwner)
    {
      iOwner.mDrawFireLance = false;
      EffectManager.Instance.Stop(ref this.mFireBreath);
    }

    public float GetWeight(Fafnir iOwner, float iHealth, float iPlayerWeight)
    {
      float val2 = 0.0f;
      if (iOwner.mPreviousState != this)
      {
        Vector2 vector2_1 = new Vector2(iOwner.mHeadZone.Position.X, iOwner.mHeadZone.Position.Z);
        Player[] mPlayers = iOwner.mPlayers;
        for (int index = 0; index < mPlayers.Length; ++index)
        {
          if (mPlayers[index].Playing && mPlayers[index].Avatar != null && !mPlayers[index].Avatar.Dead)
          {
            Vector2 vector2_2 = new Vector2(mPlayers[index].Avatar.Position.X, mPlayers[index].Avatar.Position.Z);
            float result;
            Vector2.DistanceSquared(ref vector2_1, ref vector2_2, out result);
            result = System.Math.Max(9f, result);
            if ((double) result <= 73.0)
            {
              result -= 9f;
              val2 = System.Math.Max((float) ((64.0 - (double) result) / 64.0), val2) + 0.2f;
            }
          }
        }
      }
      return val2;
    }
  }

  public class CeilingState : Fafnir.IFafnirState, IBossState<Fafnir>
  {
    private static readonly int CEILING_AREA = "trigger_area_ceiling_debri".GetHashCodeCustom();
    private static readonly int DEBRIS_EFFECT = "fafnir_debri_hit".GetHashCodeCustom();
    private static readonly int DEBRIS_SOUND = "spell_earth_hit".GetHashCodeCustom();
    private static readonly int DEBRI_TRAIL_EFFECT = "fafnir_debri_trail".GetHashCodeCustom();
    private VisualEffectReference mFireBreath;
    private ConditionCollection mDebrisConditionCollection;
    private float mTimer;
    private float mLanceTime;
    private float mTotalLanceTime;

    public CeilingState()
    {
      float iRadius = 3f;
      Magicka.GameLogic.Damage iDamage1 = new Magicka.GameLogic.Damage(AttackProperties.Pushed, Elements.Earth, 250f, 5f);
      Magicka.GameLogic.Damage iDamage2 = new Magicka.GameLogic.Damage(AttackProperties.Damage, Elements.Earth, 300f, 1f);
      this.mDebrisConditionCollection = new ConditionCollection();
      this.mDebrisConditionCollection[0].Condition.EventConditionType = EventConditionType.Default;
      this.mDebrisConditionCollection[0].Condition.Repeat = true;
      this.mDebrisConditionCollection[0].Add(new EventStorage(new PlayEffectEvent(Fafnir.CeilingState.DEBRI_TRAIL_EFFECT, true)));
      this.mDebrisConditionCollection[1].Condition.EventConditionType = EventConditionType.Hit;
      this.mDebrisConditionCollection[1].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Fafnir.CeilingState.DEBRIS_SOUND)));
      this.mDebrisConditionCollection[1].Add(new EventStorage(new PlayEffectEvent(Fafnir.CeilingState.DEBRIS_EFFECT)));
      this.mDebrisConditionCollection[1].Add(new EventStorage(new RemoveEvent()));
      this.mDebrisConditionCollection[1].Add(new EventStorage(new SplashEvent(iDamage1, iRadius)));
      this.mDebrisConditionCollection[1].Add(new EventStorage(new SplashEvent(iDamage2, iRadius)));
      this.mDebrisConditionCollection[1].Add(new EventStorage(new CameraShakeEvent(0.5f, 1f, true)));
      this.mDebrisConditionCollection[2].Condition.EventConditionType = EventConditionType.Collision;
      this.mDebrisConditionCollection[2].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Fafnir.CeilingState.DEBRIS_SOUND)));
      this.mDebrisConditionCollection[2].Add(new EventStorage(new PlayEffectEvent(Fafnir.CeilingState.DEBRIS_EFFECT)));
      this.mDebrisConditionCollection[2].Add(new EventStorage(new RemoveEvent()));
      this.mDebrisConditionCollection[2].Add(new EventStorage(new SplashEvent(iDamage1, iRadius)));
      this.mDebrisConditionCollection[2].Add(new EventStorage(new SplashEvent(iDamage2, iRadius)));
      this.mDebrisConditionCollection[2].Add(new EventStorage(new CameraShakeEvent(0.5f, 1f)));
      this.mDebrisConditionCollection[3].Condition.EventConditionType = EventConditionType.Timer;
      this.mDebrisConditionCollection[3].Add(new EventStorage(new RemoveEvent()));
      this.mDebrisConditionCollection[3].Condition.Time = 8f;
    }

    public void OnEnter(Fafnir iOwner)
    {
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[1], 0.25f, false);
      this.mTimer = 0.0f;
      iOwner.mAimForTarget = false;
      iOwner.mDrawFireLance = false;
      iOwner.mLanceScale = 0.0f;
      float duration = iOwner.mAnimationClips[1].Duration;
      this.mTotalLanceTime = (Fafnir.ANIMATION_TIMES[1][1] - Fafnir.ANIMATION_TIMES[1][0]) * duration;
    }

    public void OnUpdate(float iDeltaTime, Fafnir iOwner)
    {
      if (iOwner.mTarget == null || iOwner.mTarget.Dead)
      {
        iOwner.ChangeState(Fafnir.State.Decision);
      }
      else
      {
        float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
        if ((double) num >= (double) Fafnir.ANIMATION_TIMES[1][0] && (double) num <= (double) Fafnir.ANIMATION_TIMES[1][1] && !iOwner.mAnimationController.CrossFadeEnabled)
        {
          iOwner.mPlayState.Camera.CameraShake(0.5f, iDeltaTime);
          this.mLanceTime += iDeltaTime;
          float val1 = (float) (2.0 - 32.0 * System.Math.Pow((double) (this.mLanceTime / this.mTotalLanceTime) - 0.5, 4.0));
          iOwner.mLanceScale = System.Math.Min(val1, 1f);
          iOwner.mDrawFireLance = true;
          Vector3 delta = iOwner.mLanceSegment.Delta;
          delta.Normalize();
          if (!EffectManager.Instance.UpdatePositionDirection(ref this.mFireBreath, ref iOwner.mLanceSegment.Origin, ref delta))
            EffectManager.Instance.StartEffect(Fafnir.FIRE_SPRAY_EFFECT, ref iOwner.mLanceSegment.Origin, ref delta, out this.mFireBreath);
          this.mTimer += iDeltaTime;
          if ((double) this.mTimer > 0.5)
          {
            this.mTimer -= 0.5f;
            Vector3 position = iOwner.mTarget.Position;
            position.Y += 50f;
            position.X += (float) ((Fafnir.sRandom.NextDouble() - 0.5) * 6.0);
            position.Z += (float) ((Fafnir.sRandom.NextDouble() - 0.5) * 6.0);
            Vector3 iVelocity = new Vector3(0.1f, -22f, 0.1f);
            MissileEntity instance = MissileEntity.GetInstance(iOwner.mPlayState);
            instance.Initialize((Magicka.GameLogic.Entities.Entity) iOwner.mHeadZone, 1f, ref position, ref iVelocity, iOwner.mDebrisModel, this.mDebrisConditionCollection, false);
            instance.FacingVelocity = false;
            Vector3 angImpulse = new Vector3((float) ((Fafnir.sRandom.NextDouble() - 0.5) * (double) instance.Body.Mass * 0.5), (float) ((Fafnir.sRandom.NextDouble() - 0.5) * (double) instance.Body.Mass * 0.5), (float) ((Fafnir.sRandom.NextDouble() - 0.5) * (double) instance.Body.Mass * 0.5));
            instance.Body.ApplyBodyAngImpulse(angImpulse);
            iOwner.mPlayState.EntityManager.AddEntity((Magicka.GameLogic.Entities.Entity) instance);
          }
        }
        else if ((double) num > (double) Fafnir.ANIMATION_TIMES[1][1])
        {
          iOwner.mDrawFireLance = false;
          EffectManager.Instance.Stop(ref this.mFireBreath);
        }
        if (!iOwner.mAnimationController.HasFinished || iOwner.mAnimationController.CrossFadeEnabled)
          return;
        iOwner.ChangeState(Fafnir.State.Decision);
      }
    }

    public void OnExit(Fafnir iOwner)
    {
      iOwner.mDrawFireLance = false;
      EffectManager.Instance.Stop(ref this.mFireBreath);
    }

    public float GetWeight(Fafnir iOwner, float iHealth, float iPlayerWeight)
    {
      float weight = 0.0f;
      if ((double) iHealth <= 0.40000000596046448 && iOwner.mPreviousState != this)
        weight = (float) Fafnir.sRandom.NextDouble();
      return weight;
    }
  }

  public class EarthQuakeState : IBossState<Fafnir>
  {
    private static readonly int[][] FLOOR_GROUPS = new int[4][]
    {
      new int[6]{ 2, 6, 10, 14, 19, 23 },
      new int[7]{ 0, 5, 9, 13, 18, 20, 15 },
      new int[6]{ 3, 4, 7, 12, 16 /*0x10*/, 22 },
      new int[6]{ 1, 11, 8, 24, 17, 21 }
    };
    private static int EFFECT = "fafnir_earthquake".GetHashCodeCustom();
    private VisualEffectReference mEffect;
    private bool mStartedAnimations;

    public void OnEnter(Fafnir iOwner)
    {
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[2], 0.4f, false);
      this.mStartedAnimations = false;
    }

    public void OnUpdate(float iDeltaTime, Fafnir iOwner)
    {
      float num1 = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
      Matrix orientation = iOwner.mHeadZone.Body.Orientation with
      {
        Translation = iOwner.mHeadZone.Body.Position
      };
      EffectManager.Instance.UpdateOrientation(ref this.mEffect, ref orientation);
      if (iOwner.mAnimationController.CrossFadeEnabled)
        return;
      if ((double) num1 >= 0.5 && !this.mStartedAnimations)
      {
        do
        {
          int index = 0;
          int num2 = 0;
          while (index < Fafnir.EarthQuakeState.FLOOR_GROUPS[iOwner.mNrOfEarthquakes].Length)
          {
            int iId1 = Fafnir.LEVEL_PARTS[Fafnir.EarthQuakeState.FLOOR_GROUPS[iOwner.mNrOfEarthquakes][index]];
            iOwner.mPlayState.Level.CurrentScene.LevelModel.GetAnimatedLevelPart(iId1).Play(true, -1f, -1f, 1f, false, false);
            int iId2 = Fafnir.LAVA_EFFECTS[Fafnir.EarthQuakeState.FLOOR_GROUPS[iOwner.mNrOfEarthquakes][index]];
            iOwner.mPlayState.Level.CurrentScene.StartEffect(iId2);
            ++index;
            ++num2;
          }
          ++iOwner.mNrOfEarthquakes;
          iOwner.SetEarthQuakeThreshold();
        }
        while ((double) iOwner.mHitPoints <= (double) iOwner.mEarthQuakeThreshold);
        iOwner.mLavaCue = AudioManager.Instance.PlayCue(Banks.Spells, Fafnir.LAVAQUAKE_SOUND);
        AudioManager.Instance.PlayCue(Banks.Characters, Fafnir.SOUNDS[1]);
        this.mStartedAnimations = true;
        EffectManager.Instance.StartEffect(Fafnir.EarthQuakeState.EFFECT, ref orientation, out this.mEffect);
        iOwner.mPlayState.Camera.CameraShake(1.5f, 2.75f);
      }
      else
      {
        if (!iOwner.mAnimationController.HasFinished)
          return;
        iOwner.ChangeState(Fafnir.State.Decision);
      }
    }

    public void OnExit(Fafnir iOwner)
    {
      if (iOwner.mLavaCue != null && !iOwner.mLavaCue.IsStopping)
        iOwner.mLavaCue.Stop(AudioStopOptions.AsAuthored);
      EffectManager.Instance.Stop(ref this.mEffect);
    }
  }

  public class DefeatedState : IBossState<Fafnir>
  {
    public void OnEnter(Fafnir iOwner)
    {
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[3], 0.35f, false);
      AudioManager.Instance.PlayCue(Banks.Characters, Fafnir.SOUNDS[3], iOwner.mHeadZone.AudioEmitter);
      iOwner.mAimForTarget = false;
      iOwner.mAimForTargetWeight = 0.0f;
    }

    public void OnUpdate(float iDeltaTime, Fafnir iOwner)
    {
      if (iOwner.mAnimationController.IsLooping || !iOwner.mAnimationController.HasFinished || iOwner.mAnimationController.CrossFadeEnabled)
        return;
      iOwner.mPlayState.Level.CurrentScene.ExecuteTrigger(Fafnir.DEFEATED_TRIGGER, (Magicka.GameLogic.Entities.Character) null, false);
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[7], 1f, true);
    }

    public void OnExit(Fafnir iOwner)
    {
    }
  }
}
