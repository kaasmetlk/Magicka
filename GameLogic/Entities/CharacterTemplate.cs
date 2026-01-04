// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.CharacterTemplate
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Entities.Abilities;
using Magicka.GameLogic.Entities.Buffs;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead.Lights;
using System;
using System.Collections.Generic;
using XNAnimation;

#nullable disable
namespace Magicka.GameLogic.Entities;

public class CharacterTemplate
{
  private static string endlessLoopFix = "";
  private static Dictionary<int, CharacterTemplate> mCachedTemplates = new Dictionary<int, CharacterTemplate>();
  private string mDisplayID;
  private int mDisplayIDHash;
  private string mID;
  private int mIDHash;
  private BloodType mBlood;
  private bool mIsEthereal;
  private bool mLooksEthereal;
  private bool mCanSeeInvisible;
  private bool mFearless;
  private bool mUncharmable;
  private bool mNonslippery;
  private bool mHasFairy;
  private KeyValuePair<int, Banks>[] mAttachedSounds;
  private Character.PointLightHolder[] mPointLightHolder;
  private GibReference[] mGibs;
  private Factions mFaction;
  private Attachment[] mEquipment;
  private float mMaxHitpoints;
  private bool mUndying;
  private float mUndieTime;
  private float mUndieHitPoints;
  private int mHitTolerance;
  private float mKnockdownTolerance;
  private int mScoreValue;
  private int mRegeneration;
  private float mMaxPanic;
  private float mZapModifier;
  private float mTurnSpeed;
  private float mLength;
  private float mRadius;
  private float mMass;
  private float mSpeed;
  private int mNumberOfHealthBars;
  private float mBleedRate;
  private float mStunTime;
  private Resistance[] mResistances;
  private KeyValuePair<int, int>[] mAttachedEffects;
  private ModelProperties[] mModels;
  private SkinnedModelBoneCollection mSkeleton;
  private AnimationClipAction[][] mAnimationClips;
  private static VertexDeclaration sSkeletonVertexDeclaration;
  private VertexBuffer mSkeletonVertices;
  private int mSkeletonPrimitiveCount;
  private BindJoint mRightHandJoint;
  private BindJoint mRightKneeJoint;
  private BindJoint mLeftHandJoint;
  private BindJoint mLeftKneeJoint;
  private BindJoint mMouthJoint;
  private BindJoint mHipJoint;
  private ConditionCollection mEventConditions;
  private Banks mSummonElementBank;
  private int mSummonElementCueID;
  private string mSummonElementCueString;
  private float mAlertRadius;
  private float mGroupChase;
  private float mGroupSeparation;
  private float mGroupCohesion;
  private float mGroupAlignment;
  private float mGroupWander;
  private float mFriendlyAvoidance;
  private float mEnemyAvoidance;
  private float mSightAvoidance;
  private float mDangerAvoidance;
  private float mBreakFreeStrength;
  private float mAngerWeight;
  private float mDistanceWeight;
  private float mHealthWeight;
  private bool mFlocking;
  private Ability[] mAbilities;
  private MovementProperties mMoveAbilities;
  private Dictionary<byte, Animations[]> mMoveAnimations;
  private List<AuraStorage> mAuras = new List<AuraStorage>();
  private List<BuffStorage> mBuffs = new List<BuffStorage>();

  public static CharacterTemplate GetCachedTemplate(int iID)
  {
    try
    {
      return CharacterTemplate.mCachedTemplates[iID];
    }
    catch (Exception ex)
    {
      return (CharacterTemplate) null;
    }
  }

  public static void ClearCache()
  {
    foreach (CharacterTemplate characterTemplate in CharacterTemplate.mCachedTemplates.Values)
    {
      for (int index1 = 0; index1 < characterTemplate.Gibs.Length; ++index1)
      {
        Model mModel = characterTemplate.Gibs[index1].mModel;
        for (int index2 = 0; index2 < mModel.Meshes.Count; ++index2)
        {
          ModelMesh mesh = mModel.Meshes[index2];
          for (int index3 = 0; index3 < mesh.MeshParts.Count; ++index3)
          {
            if (!mesh.MeshParts[index3].Effect.IsDisposed)
              mesh.MeshParts[index3].Effect.Dispose();
            if (!mesh.MeshParts[index3].VertexDeclaration.IsDisposed)
              mesh.MeshParts[index3].VertexDeclaration.Dispose();
          }
          mesh.VertexBuffer.Dispose();
          mesh.IndexBuffer.Dispose();
        }
      }
    }
    CharacterTemplate.mCachedTemplates.Clear();
  }

  private CharacterTemplate()
  {
  }

  public static CharacterTemplate Read(ContentReader iInput)
  {
    if (string.Compare(iInput.AssetName, CharacterTemplate.endlessLoopFix) == 0)
      return (CharacterTemplate) null;
    CharacterTemplate.endlessLoopFix = iInput.AssetName;
    CharacterTemplate characterTemplate = new CharacterTemplate()
    {
      mID = iInput.ReadString().ToLowerInvariant()
    };
    characterTemplate.mIDHash = characterTemplate.mID.GetHashCodeCustom();
    characterTemplate.mDisplayID = iInput.ReadString().ToLowerInvariant();
    characterTemplate.mDisplayIDHash = characterTemplate.mDisplayID.GetHashCodeCustom();
    characterTemplate.mFaction = (Factions) iInput.ReadInt32();
    characterTemplate.mBlood = (BloodType) iInput.ReadInt32();
    characterTemplate.mIsEthereal = iInput.ReadBoolean();
    characterTemplate.mLooksEthereal = iInput.ReadBoolean();
    characterTemplate.mFearless = iInput.ReadBoolean();
    characterTemplate.mUncharmable = iInput.ReadBoolean();
    characterTemplate.mNonslippery = iInput.ReadBoolean();
    characterTemplate.mHasFairy = iInput.ReadBoolean();
    characterTemplate.mCanSeeInvisible = iInput.ReadBoolean();
    int num1 = iInput.ReadInt32();
    characterTemplate.mAttachedSounds = new KeyValuePair<int, Banks>[4];
    for (int index = 0; index < num1; ++index)
    {
      if (index < 4)
      {
        string lowerInvariant = iInput.ReadString().ToLowerInvariant();
        Banks banks = (Banks) iInput.ReadInt32();
        int hashCodeCustom = lowerInvariant.GetHashCodeCustom();
        characterTemplate.mAttachedSounds[index] = new KeyValuePair<int, Banks>(hashCodeCustom, banks);
      }
    }
    int length1 = iInput.ReadInt32();
    characterTemplate.mGibs = new GibReference[length1];
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    for (int index = 0; index < length1; ++index)
    {
      GibReference gibReference;
      lock (graphicsDevice)
        gibReference.mModel = iInput.ReadExternalReference<Model>();
      gibReference.mMass = iInput.ReadSingle();
      gibReference.mScale = iInput.ReadSingle();
      characterTemplate.mGibs[index] = gibReference;
    }
    int num2 = iInput.ReadInt32();
    characterTemplate.mPointLightHolder = new Character.PointLightHolder[4];
    for (int index = 0; index < num2; ++index)
    {
      if (index >= 4)
        throw new Exception("Character may Only have Four Lights!");
      characterTemplate.mPointLightHolder[index] = new Character.PointLightHolder();
      characterTemplate.PointLightHolder[index].ContainsLight = true;
      characterTemplate.PointLightHolder[index].JointName = iInput.ReadString();
      characterTemplate.mPointLightHolder[index].Radius = iInput.ReadSingle();
      characterTemplate.mPointLightHolder[index].DiffuseColor = iInput.ReadVector3();
      characterTemplate.mPointLightHolder[index].AmbientColor = iInput.ReadVector3();
      characterTemplate.mPointLightHolder[index].SpecularAmount = iInput.ReadSingle();
      characterTemplate.mPointLightHolder[index].VariationType = (LightVariationType) iInput.ReadByte();
      characterTemplate.mPointLightHolder[index].VariationAmount = iInput.ReadSingle();
      characterTemplate.mPointLightHolder[index].VariationSpeed = iInput.ReadSingle();
    }
    characterTemplate.mMaxHitpoints = iInput.ReadSingle();
    characterTemplate.mNumberOfHealthBars = iInput.ReadInt32();
    if (characterTemplate.mNumberOfHealthBars <= 0)
      characterTemplate.mNumberOfHealthBars = 1;
    characterTemplate.mUndying = iInput.ReadBoolean();
    characterTemplate.mUndieTime = iInput.ReadSingle();
    characterTemplate.mUndieHitPoints = iInput.ReadSingle();
    characterTemplate.mHitTolerance = iInput.ReadInt32();
    characterTemplate.mKnockdownTolerance = iInput.ReadSingle();
    characterTemplate.mScoreValue = iInput.ReadInt32();
    characterTemplate.mRegeneration = iInput.ReadInt32();
    characterTemplate.mMaxPanic = iInput.ReadSingle();
    characterTemplate.mZapModifier = iInput.ReadSingle();
    characterTemplate.mLength = Math.Max(iInput.ReadSingle(), 0.01f);
    characterTemplate.mRadius = iInput.ReadSingle();
    characterTemplate.mMass = iInput.ReadSingle();
    characterTemplate.mSpeed = iInput.ReadSingle();
    characterTemplate.mTurnSpeed = iInput.ReadSingle();
    characterTemplate.mBleedRate = iInput.ReadSingle();
    characterTemplate.mStunTime = iInput.ReadSingle();
    characterTemplate.mSummonElementBank = (Banks) iInput.ReadInt32();
    characterTemplate.mSummonElementCueString = iInput.ReadString();
    characterTemplate.mSummonElementCueID = characterTemplate.mSummonElementCueString.GetHashCodeCustom();
    int num3 = iInput.ReadInt32();
    characterTemplate.mResistances = new Resistance[11];
    for (int iIndex = 0; iIndex < characterTemplate.mResistances.Length; ++iIndex)
    {
      characterTemplate.mResistances[iIndex].ResistanceAgainst = Defines.ElementFromIndex(iIndex);
      characterTemplate.mResistances[iIndex].Multiplier = 1f;
      characterTemplate.mResistances[iIndex].Modifier = 0.0f;
      characterTemplate.mResistances[iIndex].StatusResistance = false;
    }
    for (int index1 = 0; index1 < num3; ++index1)
    {
      Elements iElement = (Elements) iInput.ReadInt32();
      int index2 = Defines.ElementIndex(iElement);
      characterTemplate.mResistances[index2].ResistanceAgainst = iElement;
      characterTemplate.mResistances[index2].Multiplier = iInput.ReadSingle();
      characterTemplate.mResistances[index2].Modifier = iInput.ReadSingle();
      characterTemplate.mResistances[index2].StatusResistance = iInput.ReadBoolean();
    }
    if ((characterTemplate.mFaction & Factions.FRIENDLY) == Factions.NONE)
    {
      float num4 = (float) (1.0 - ((double) Magicka.Game.Instance.PlayerCount - 1.0) * 0.006659999955445528);
      for (int index = 0; index < characterTemplate.mResistances.Length; ++index)
      {
        if ((double) characterTemplate.mResistances[index].Multiplier < 0.0)
          characterTemplate.mResistances[index].Multiplier /= num4;
        else
          characterTemplate.mResistances[index].Multiplier *= num4;
        if ((double) characterTemplate.mResistances[index].Modifier < 0.0)
          characterTemplate.mResistances[index].Modifier /= num4;
        else
          characterTemplate.mResistances[index].Modifier *= num4;
      }
    }
    int length2 = iInput.ReadInt32();
    characterTemplate.mModels = new ModelProperties[length2];
    SkinnedModel skinnedModel;
    lock (graphicsDevice)
    {
      for (int index = 0; index < length2; ++index)
      {
        ModelProperties modelProperties;
        modelProperties.Model = iInput.ReadExternalReference<SkinnedModel>();
        modelProperties.Scale = iInput.ReadSingle();
        Matrix.CreateScale(modelProperties.Scale, out modelProperties.Transform);
        modelProperties.Tint = iInput.ReadVector3();
        characterTemplate.mModels[index] = modelProperties;
      }
      skinnedModel = iInput.ReadExternalReference<SkinnedModel>();
    }
    characterTemplate.mSkeleton = skinnedModel.SkeletonBones;
    int length3 = iInput.ReadInt32();
    characterTemplate.mAttachedEffects = new KeyValuePair<int, int>[length3];
    for (int index3 = 0; index3 < length3; ++index3)
    {
      string lowerInvariant = iInput.ReadString().ToLowerInvariant();
      int hashCodeCustom = iInput.ReadString().ToLowerInvariant().GetHashCodeCustom();
      int key = -1;
      for (int index4 = 0; index4 < skinnedModel.SkeletonBones.Count; ++index4)
      {
        SkinnedModelBone skeletonBone = skinnedModel.SkeletonBones[index4];
        if (skeletonBone.Name.Equals(lowerInvariant, StringComparison.OrdinalIgnoreCase))
          key = (int) skeletonBone.Index;
      }
      characterTemplate.mAttachedEffects[index3] = key >= 0 ? new KeyValuePair<int, int>(key, hashCodeCustom) : throw new Exception($"Bone \"{lowerInvariant}\" not found!");
    }
    characterTemplate.mAnimationClips = new AnimationClipAction[27][];
    for (int index5 = 0; index5 < characterTemplate.mAnimationClips.Length; ++index5)
    {
      int num5 = iInput.ReadInt32();
      if (num5 > 0)
      {
        characterTemplate.mAnimationClips[index5] = new AnimationClipAction[231];
        for (int index6 = 0; index6 < num5; ++index6)
        {
          Animations iAnimation = (Animations) Enum.Parse(typeof (Animations), iInput.ReadString(), true);
          characterTemplate.mAnimationClips[index5][(int) iAnimation] = new AnimationClipAction(iAnimation, iInput, skinnedModel.AnimationClips, characterTemplate.mSkeleton);
        }
      }
    }
    characterTemplate.mEquipment = new Attachment[8];
    int num6 = iInput.ReadInt32();
    for (int index = 0; index < characterTemplate.mEquipment.Length; ++index)
      characterTemplate.mEquipment[index] = new Attachment((PlayState) null, (Character) null);
    for (int index7 = 0; index7 < num6; ++index7)
    {
      int index8 = iInput.ReadInt32();
      characterTemplate.mEquipment[index8] = new Attachment(iInput, characterTemplate.mSkeleton);
    }
    for (int index9 = 0; index9 < skinnedModel.SkeletonBones.Count; ++index9)
    {
      SkinnedModelBone skeletonBone = skinnedModel.SkeletonBones[index9];
      if (skeletonBone.Name.Equals("Mouth", StringComparison.OrdinalIgnoreCase))
      {
        characterTemplate.mMouthJoint.mIndex = (int) skeletonBone.Index;
        characterTemplate.mMouthJoint.mBindPose = Matrix.CreateRotationY(3.14159274f) * Matrix.Invert(skeletonBone.InverseBindPoseTransform);
      }
      else if (skeletonBone.Name.Equals("RightAttach", StringComparison.OrdinalIgnoreCase))
      {
        characterTemplate.mRightHandJoint.mIndex = (int) skeletonBone.Index;
        characterTemplate.mRightHandJoint.mBindPose = Matrix.CreateRotationY(3.14159274f) * Matrix.Invert(skeletonBone.InverseBindPoseTransform);
      }
      else if (skeletonBone.Name.Equals("RightLeg", StringComparison.OrdinalIgnoreCase))
      {
        characterTemplate.mRightKneeJoint.mIndex = (int) skeletonBone.Index;
        characterTemplate.mRightKneeJoint.mBindPose = Matrix.CreateRotationY(3.14159274f) * Matrix.Invert(skeletonBone.InverseBindPoseTransform);
      }
      else if (skeletonBone.Name.Equals("LeftAttach", StringComparison.OrdinalIgnoreCase))
      {
        characterTemplate.mLeftHandJoint.mIndex = (int) skeletonBone.Index;
        characterTemplate.mLeftHandJoint.mBindPose = Matrix.CreateRotationY(3.14159274f) * Matrix.Invert(skeletonBone.InverseBindPoseTransform);
      }
      else if (skeletonBone.Name.Equals("LeftLeg", StringComparison.OrdinalIgnoreCase))
      {
        characterTemplate.mLeftKneeJoint.mIndex = (int) skeletonBone.Index;
        characterTemplate.mLeftKneeJoint.mBindPose = Matrix.CreateRotationY(3.14159274f) * Matrix.Invert(skeletonBone.InverseBindPoseTransform);
      }
      else if (skeletonBone.Index == (ushort) 1)
      {
        characterTemplate.mHipJoint.mIndex = (int) skeletonBone.Index;
        characterTemplate.mHipJoint.mBindPose = Matrix.CreateRotationY(3.14159274f) * Matrix.Invert(skeletonBone.InverseBindPoseTransform);
      }
      for (int index10 = 0; index10 < characterTemplate.PointLightHolder.Length; ++index10)
      {
        if (characterTemplate.PointLightHolder != null && skeletonBone.Name.Equals(characterTemplate.PointLightHolder[index10].JointName, StringComparison.OrdinalIgnoreCase))
        {
          characterTemplate.PointLightHolder[index10].Joint.mIndex = index9;
          characterTemplate.PointLightHolder[index10].Joint.mBindPose = Matrix.CreateRotationY(3.14159274f) * Matrix.Invert(skeletonBone.InverseBindPoseTransform);
        }
      }
    }
    characterTemplate.mEventConditions = new ConditionCollection(iInput);
    characterTemplate.mAlertRadius = iInput.ReadSingle();
    characterTemplate.mGroupChase = iInput.ReadSingle();
    characterTemplate.mGroupSeparation = iInput.ReadSingle();
    characterTemplate.mGroupCohesion = iInput.ReadSingle();
    characterTemplate.mGroupAlignment = iInput.ReadSingle();
    characterTemplate.mGroupWander = iInput.ReadSingle();
    characterTemplate.mFriendlyAvoidance = iInput.ReadSingle();
    characterTemplate.mEnemyAvoidance = iInput.ReadSingle();
    characterTemplate.mSightAvoidance = iInput.ReadSingle();
    characterTemplate.mDangerAvoidance = iInput.ReadSingle();
    characterTemplate.mAngerWeight = iInput.ReadSingle();
    characterTemplate.mDistanceWeight = iInput.ReadSingle();
    characterTemplate.mHealthWeight = iInput.ReadSingle();
    characterTemplate.mFlocking = iInput.ReadBoolean();
    characterTemplate.mBreakFreeStrength = iInput.ReadSingle();
    characterTemplate.mAbilities = new Ability[iInput.ReadInt32()];
    for (int iIndex = 0; iIndex < characterTemplate.mAbilities.Length; ++iIndex)
      characterTemplate.mAbilities[iIndex] = Ability.Read(iIndex, iInput, characterTemplate.mAnimationClips);
    int capacity = iInput.ReadInt32();
    characterTemplate.mMoveAnimations = new Dictionary<byte, Animations[]>(capacity);
    characterTemplate.mMoveAbilities = MovementProperties.Default;
    for (int index11 = 0; index11 < capacity; ++index11)
    {
      MovementProperties key = (MovementProperties) iInput.ReadByte();
      Animations[] animationsArray = new Animations[iInput.ReadInt32()];
      for (int index12 = 0; index12 < animationsArray.Length; ++index12)
        animationsArray[index12] = (Animations) Enum.Parse(typeof (Animations), iInput.ReadString(), true);
      characterTemplate.mMoveAbilities |= key;
      characterTemplate.mMoveAnimations.Add((byte) key, animationsArray);
    }
    if (CharacterTemplate.sSkeletonVertexDeclaration == null)
    {
      lock (graphicsDevice)
        CharacterTemplate.sSkeletonVertexDeclaration = new VertexDeclaration(graphicsDevice, CharacterTemplate.VertexPositionIndexWeight.VertexElements);
    }
    CharacterTemplate.GenerateSkeletonModel(characterTemplate.mModels[0].Model.SkeletonBones, 0.125f * characterTemplate.mRadius / characterTemplate.mModels[0].Scale, out characterTemplate.mSkeletonVertices, out characterTemplate.mSkeletonPrimitiveCount);
    int num7 = iInput.ReadInt32();
    for (int index = 0; index < num7; ++index)
    {
      BuffStorage buffStorage = new BuffStorage(iInput);
      characterTemplate.mBuffs.Add(buffStorage);
    }
    int num8 = iInput.ReadInt32();
    for (int index = 0; index < num8; ++index)
    {
      AuraStorage auraStorage = new AuraStorage(iInput);
      if ((double) auraStorage.TTL <= 0.0)
        auraStorage.TTL = float.PositiveInfinity;
      characterTemplate.mAuras.Add(auraStorage);
    }
    CharacterTemplate.mCachedTemplates[characterTemplate.mIDHash] = characterTemplate;
    return characterTemplate;
  }

  public int Regeneration => this.mRegeneration;

  public int DisplayName => this.mDisplayIDHash;

  public int ID => this.mIDHash;

  public string Name => this.mID;

  public bool IsEthereal => this.mIsEthereal;

  public bool LooksEthereal => this.mLooksEthereal;

  public bool CanSeeInvisible => this.mCanSeeInvisible;

  public float ZapModifier => this.mZapModifier;

  public float TurnSpeed => this.mTurnSpeed;

  public BloodType Blood => this.mBlood;

  public GibReference[] Gibs => this.mGibs;

  public Attachment[] Equipment => this.mEquipment;

  public float MaxPanic => this.mMaxPanic;

  public float MaxHitpoints => this.mMaxHitpoints;

  public int NumberOfHealthBars => this.mNumberOfHealthBars;

  public bool Undying => this.mUndying;

  public float UndieTime => this.mUndieTime;

  public float UndieHitPoints => this.mUndieHitPoints;

  public int HitTolerance => this.mHitTolerance;

  public float KnockdownTolerance => this.mKnockdownTolerance;

  public int ScoreValue => this.mScoreValue;

  public float Length => this.mLength;

  public float Radius => this.mRadius;

  public float Mass => this.mMass;

  public float Speed => this.mSpeed;

  public int SummonElementCue => this.mSummonElementCueID;

  public Banks SummonElementBank => this.mSummonElementBank;

  public float BleedRate => this.mBleedRate;

  public float StunTime => this.mStunTime;

  public ModelProperties[] Models => this.mModels;

  public SkinnedModelBoneCollection Skeleton => this.mSkeleton;

  public VertexDeclaration SkeletonVertexDeclaration
  {
    get => CharacterTemplate.sSkeletonVertexDeclaration;
  }

  public VertexBuffer SkeletonVertices => this.mSkeletonVertices;

  public int SkeletonPrimitiveCount => this.mSkeletonPrimitiveCount;

  public int SkeletonVertexStride => 48 /*0x30*/;

  public AnimationClipAction[][] AnimationClips => this.mAnimationClips;

  public Resistance[] Resistances => this.mResistances;

  public BindJoint HipJoint => this.mHipJoint;

  public BindJoint LeftHandJoint => this.mLeftHandJoint;

  public BindJoint LeftKneeJoint => this.mLeftKneeJoint;

  public BindJoint RightHandJoint => this.mRightHandJoint;

  public BindJoint RightKneeJoint => this.mRightKneeJoint;

  public BindJoint MouthJoint => this.mMouthJoint;

  public KeyValuePair<int, Banks>[] AttachedSounds => this.mAttachedSounds;

  public KeyValuePair<int, int>[] AttachedEffects => this.mAttachedEffects;

  public Character.PointLightHolder[] PointLightHolder => this.mPointLightHolder;

  public ConditionCollection EventConditions => this.mEventConditions;

  public List<AuraStorage> Auras => this.mAuras;

  public List<BuffStorage> Buffs => this.mBuffs;

  public bool IsFearless => this.mFearless;

  public bool IsUncharmable => this.mUncharmable;

  public bool IsNonslippery => this.mNonslippery;

  public bool HasFairy => this.mHasFairy;

  public float AlertRadius => this.mAlertRadius;

  public float GroupChase => this.mGroupChase;

  public float GroupSeparation => this.mGroupSeparation;

  public float GroupCohesion => this.mGroupCohesion;

  public float GroupAlignment => this.mGroupAlignment;

  public float GroupWander => this.mGroupWander;

  public float FriendlyAvoidance => this.mFriendlyAvoidance;

  public float EnemyAvoidance => this.mEnemyAvoidance;

  public float SightAvoidance => this.mSightAvoidance;

  public float DangerAvoidance => this.mDangerAvoidance;

  public float AngerWeight => this.mAngerWeight;

  public float DistanceWeight => this.mDistanceWeight;

  public float HealthWeight => this.mHealthWeight;

  public bool Flocking => this.mFlocking;

  public float BreakFreeStrength => this.mBreakFreeStrength;

  public Factions Faction => this.mFaction;

  public Ability[] Abilities => this.mAbilities;

  public MovementProperties MoveAbilities => this.mMoveAbilities;

  public Dictionary<byte, Animations[]> MoveAnimations => this.mMoveAnimations;

  private static void GenerateSkeletonModel(
    SkinnedModelBoneCollection iSkeleton,
    float iScale,
    out VertexBuffer oVertexBuffer,
    out int oPrimitiveCount)
  {
    List<CharacterTemplate.VertexPositionIndexWeight> iVertices = new List<CharacterTemplate.VertexPositionIndexWeight>();
    for (int index = 1; index < iSkeleton.Count; ++index)
    {
      SkinnedModelBone parent = iSkeleton[index];
      Matrix result = parent.InverseBindPoseTransform;
      Matrix.Invert(ref result, out result);
      float num = 0.0f;
      while (parent.Parent != null)
      {
        parent = parent.Parent;
        ++num;
      }
      SkinnedModelBone iChild = iSkeleton[index];
      float iRadius = (float) Math.Pow(0.8, (double) num - 1.0) * iScale;
      CharacterTemplate.GenerateBall(iVertices, result, (float) index, iRadius);
      if (index > 1)
        CharacterTemplate.GeneratePyramid(iVertices, iChild, iRadius);
    }
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    if (iVertices.Count == 0)
    {
      oVertexBuffer = (VertexBuffer) null;
      oPrimitiveCount = 0;
    }
    else
    {
      lock (graphicsDevice)
      {
        oVertexBuffer = new VertexBuffer(graphicsDevice, iVertices.Count * 48 /*0x30*/, BufferUsage.WriteOnly);
        oVertexBuffer.SetData<CharacterTemplate.VertexPositionIndexWeight>(iVertices.ToArray());
      }
      oPrimitiveCount = iVertices.Count - 1;
    }
  }

  private static void GenerateBall(
    List<CharacterTemplate.VertexPositionIndexWeight> iVertices,
    Matrix iTransform,
    float iBoneIndex,
    float iRadius)
  {
    Vector3 result1 = new Vector3();
    result1.Z = iRadius;
    CharacterTemplate.VertexPositionIndexWeight positionIndexWeight = new CharacterTemplate.VertexPositionIndexWeight();
    positionIndexWeight.Color.PackedValue = 0U;
    positionIndexWeight.Indices = iBoneIndex;
    Vector3.Transform(ref result1, ref iTransform, out positionIndexWeight.Position);
    iVertices.Add(positionIndexWeight);
    positionIndexWeight.Color.PackedValue = uint.MaxValue;
    iVertices.Add(positionIndexWeight);
    for (int index1 = 0; index1 < 12; ++index1)
    {
      float num1 = 1f;
      if (index1 >= 4 & index1 != 7 & index1 != 11)
        num1 = -1f;
      float num2 = num1 / 2f;
      Quaternion result2;
      if (index1 % 3 == 0)
        Quaternion.CreateFromYawPitchRoll(0.0f, num2 * -1.57079637f, 0.0f, out result2);
      else if (index1 % 3 == 1)
        Quaternion.CreateFromYawPitchRoll(0.0f, 0.0f, num2 * -1.57079637f, out result2);
      else
        Quaternion.CreateFromYawPitchRoll(num2 * 1.57079637f, 0.0f, 0.0f, out result2);
      for (int index2 = 0; index2 < 2; ++index2)
      {
        Vector3.Transform(ref result1, ref result2, out result1);
        Vector3.Transform(ref result1, ref iTransform, out positionIndexWeight.Position);
        iVertices.Add(positionIndexWeight);
      }
    }
    positionIndexWeight.Color.PackedValue = 0U;
    iVertices.Add(positionIndexWeight);
  }

  private static void GeneratePyramid(
    List<CharacterTemplate.VertexPositionIndexWeight> iVertices,
    SkinnedModelBone iChild,
    float iRadius)
  {
    Matrix result1 = iChild.InverseBindPoseTransform;
    Matrix.Invert(ref result1, out result1);
    Matrix result2 = iChild.Parent.InverseBindPoseTransform;
    Matrix.Invert(ref result2, out result2);
    CharacterTemplate.VertexPositionIndexWeight positionIndexWeight1 = new CharacterTemplate.VertexPositionIndexWeight();
    positionIndexWeight1.Offset.Z = -iRadius;
    positionIndexWeight1.Position = result1.Translation;
    positionIndexWeight1.Indices = (float) iChild.Index;
    positionIndexWeight1.NextPosition = result2.Translation;
    positionIndexWeight1.NextIndices = (float) iChild.Parent.Index;
    positionIndexWeight1.Color = Color.White;
    CharacterTemplate.VertexPositionIndexWeight positionIndexWeight2 = new CharacterTemplate.VertexPositionIndexWeight();
    positionIndexWeight2.Position = result2.Translation;
    positionIndexWeight2.Indices = (float) iChild.Parent.Index;
    positionIndexWeight2.NextPosition = result1.Translation;
    positionIndexWeight2.NextIndices = (float) iChild.Index;
    positionIndexWeight2.Offset.Z = (float) (-(double) iRadius / 0.800000011920929);
    positionIndexWeight2.Offset.X = iRadius / 0.8f;
    Quaternion result3;
    Quaternion.CreateFromYawPitchRoll(0.0f, 0.0f, 1.57079637f, out result3);
    for (int index = 0; index < 4; ++index)
    {
      positionIndexWeight2.Color.PackedValue = 0U;
      iVertices.Add(positionIndexWeight2);
      positionIndexWeight2.Color.PackedValue = uint.MaxValue;
      iVertices.Add(positionIndexWeight2);
      Vector3.Transform(ref positionIndexWeight2.Offset, ref result3, out positionIndexWeight2.Offset);
      iVertices.Add(positionIndexWeight2);
      positionIndexWeight1.Color.PackedValue = uint.MaxValue;
      iVertices.Add(positionIndexWeight1);
      positionIndexWeight1.Color.PackedValue = 0U;
      iVertices.Add(positionIndexWeight1);
    }
  }

  private struct VertexPositionIndexWeight
  {
    public const int SIZEINBYTES = 48 /*0x30*/;
    public Vector3 Position;
    public float Indices;
    public Vector3 NextPosition;
    public float NextIndices;
    public Vector3 Offset;
    public Color Color;
    public static readonly VertexElement[] VertexElements = new VertexElement[6]
    {
      new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
      new VertexElement((short) 0, (short) 12, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.BlendIndices, (byte) 0),
      new VertexElement((short) 0, (short) 16 /*0x10*/, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 1),
      new VertexElement((short) 0, (short) 28, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.BlendIndices, (byte) 1),
      new VertexElement((short) 0, (short) 32 /*0x20*/, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Normal, (byte) 0),
      new VertexElement((short) 0, (short) 44, VertexElementFormat.Color, VertexElementMethod.Default, VertexElementUsage.Color, (byte) 0)
    };
  }
}
