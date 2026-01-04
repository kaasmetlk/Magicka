// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.PhysicsEntityTemplate
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead.Effects;
using System;
using System.Collections.Generic;
using XNAnimation;

#nullable disable
namespace Magicka.GameLogic.Entities;

public class PhysicsEntityTemplate
{
  private string mPath;
  private bool mMovable;
  private bool mPushable;
  private bool mSolid;
  private float mMass;
  private bool mCanHaveStatus;
  private int mMaxHitPoints;
  private int mHitSound;
  private Banks mHitSoundBank;
  private int mHitEffect;
  private int mGibTrailEffect;
  private ConditionCollection mConditions;
  private List<Vector3> mMeshVertices;
  private List<TriangleVertexIndices> mMeshIndices;
  private PhysicsEntityTemplate.BoxInfo mBox;
  private PhysicsEntity.VisualEffectStorage[] mEffects;
  private Model mModel;
  private Resistance[] mResistances;
  private GibReference[] mGibs;
  private ModelProperties[] mModels;
  private SkinnedModelBoneCollection mSkeleton;
  private AnimationClipAction[][] mAnimationClips;
  private KeyValuePair<int, int>[] mAttachedEffects;
  private static VertexDeclaration sSkeletonVertexDeclaration;
  private VertexBuffer mSkeletonVertices;
  private int mSkeletonPrimitiveCount;
  public float mRadius = 1f;
  private string mID;
  private int mIDHash;

  public int ID => this.mIDHash;

  private PhysicsEntityTemplate()
  {
  }

  public static PhysicsEntityTemplate Read(ContentReader iInput)
  {
    PhysicsEntityTemplate physicsEntityTemplate = new PhysicsEntityTemplate();
    string assetName = iInput.AssetName;
    int num1 = assetName.IndexOf("content", StringComparison.OrdinalIgnoreCase);
    string str1 = assetName.Substring(num1 + "content".Length + 1);
    physicsEntityTemplate.mPath = str1.ToLowerInvariant();
    physicsEntityTemplate.mMovable = iInput.ReadBoolean();
    physicsEntityTemplate.mPushable = iInput.ReadBoolean();
    physicsEntityTemplate.mSolid = iInput.ReadBoolean();
    physicsEntityTemplate.mMass = iInput.ReadSingle();
    physicsEntityTemplate.mMaxHitPoints = iInput.ReadInt32();
    physicsEntityTemplate.mCanHaveStatus = iInput.ReadBoolean();
    GraphicsDevice graphicsDevice = (iInput.ContentManager.ServiceProvider.GetService(typeof (IGraphicsDeviceManager)) as GraphicsDeviceManager).GraphicsDevice;
    int num2 = iInput.ReadInt32();
    physicsEntityTemplate.mResistances = new Resistance[11];
    for (int iIndex = 0; iIndex < physicsEntityTemplate.mResistances.Length; ++iIndex)
    {
      physicsEntityTemplate.mResistances[iIndex].ResistanceAgainst = Defines.ElementFromIndex(iIndex);
      physicsEntityTemplate.mResistances[iIndex].Multiplier = 1f;
      physicsEntityTemplate.mResistances[iIndex].Modifier = 0.0f;
    }
    for (int index1 = 0; index1 < num2; ++index1)
    {
      Elements iElement = (Elements) iInput.ReadInt32();
      int index2 = Defines.ElementIndex(iElement);
      physicsEntityTemplate.mResistances[index2].ResistanceAgainst = iElement;
      physicsEntityTemplate.mResistances[index2].Multiplier = iInput.ReadSingle();
      physicsEntityTemplate.mResistances[index2].Modifier = iInput.ReadSingle();
    }
    int length1 = iInput.ReadInt32();
    physicsEntityTemplate.mGibs = new GibReference[length1];
    for (int index = 0; index < length1; ++index)
    {
      GibReference gibReference;
      lock (graphicsDevice)
        gibReference.mModel = iInput.ReadExternalReference<Model>();
      gibReference.mMass = iInput.ReadSingle();
      gibReference.mScale = iInput.ReadSingle();
      physicsEntityTemplate.mGibs[index] = gibReference;
    }
    string str2 = iInput.ReadString();
    physicsEntityTemplate.mHitEffect = string.IsNullOrEmpty(str2) ? 0 : str2.ToLowerInvariant().GetHashCodeCustom();
    string str3 = iInput.ReadString();
    if (string.IsNullOrEmpty(str3))
    {
      physicsEntityTemplate.mHitSoundBank = Banks.Weapons;
      physicsEntityTemplate.mHitSound = 0;
    }
    else
    {
      string[] strArray = str3.Split('/');
      if (strArray != null && strArray.Length > 1)
      {
        physicsEntityTemplate.mHitSoundBank = (Banks) Enum.Parse(typeof (Banks), strArray[0], true);
        physicsEntityTemplate.mHitSound = strArray[1].ToLowerInvariant().GetHashCodeCustom();
      }
      else
      {
        physicsEntityTemplate.mHitSoundBank = Banks.Weapons;
        physicsEntityTemplate.mHitSound = str3.ToLowerInvariant().GetHashCodeCustom();
      }
    }
    string str4 = iInput.ReadString();
    physicsEntityTemplate.mGibTrailEffect = string.IsNullOrEmpty(str4) ? 0 : str4.ToLowerInvariant().GetHashCodeCustom();
    lock (graphicsDevice)
      physicsEntityTemplate.mModel = iInput.ReadObject<Model>();
    if (iInput.ReadBoolean())
    {
      physicsEntityTemplate.mMeshVertices = iInput.ReadObject<List<Vector3>>();
      int capacity = iInput.ReadInt32();
      physicsEntityTemplate.mMeshIndices = new List<TriangleVertexIndices>(capacity);
      for (int index = 0; index < capacity; ++index)
      {
        TriangleVertexIndices triangleVertexIndices;
        triangleVertexIndices.I0 = iInput.ReadInt32();
        triangleVertexIndices.I1 = iInput.ReadInt32();
        triangleVertexIndices.I2 = iInput.ReadInt32();
        physicsEntityTemplate.mMeshIndices.Add(triangleVertexIndices);
      }
    }
    int num3 = iInput.ReadInt32();
    if (num3 > 0)
    {
      iInput.ReadString();
      physicsEntityTemplate.mBox.Positon = iInput.ReadVector3();
      physicsEntityTemplate.mBox.Sides = iInput.ReadVector3();
      physicsEntityTemplate.mBox.Orientation = iInput.ReadQuaternion();
      if (num3 > 1)
      {
        for (int index = 1; index < num3; ++index)
        {
          iInput.ReadString();
          iInput.ReadVector3();
          iInput.ReadVector3();
          iInput.ReadQuaternion();
        }
      }
    }
    int length2 = iInput.ReadInt32();
    physicsEntityTemplate.mEffects = new PhysicsEntity.VisualEffectStorage[length2];
    for (int index = 0; index < length2; ++index)
    {
      physicsEntityTemplate.mEffects[index].EffectHash = iInput.ReadString().GetHashCodeCustom();
      physicsEntityTemplate.mEffects[index].Transform = iInput.ReadMatrix();
    }
    physicsEntityTemplate.Conditions = iInput.ReadInt32() <= 0 ? new ConditionCollection(iInput) : throw new NotImplementedException("Lights");
    for (int index3 = 0; index3 < physicsEntityTemplate.Conditions.Count; ++index3)
    {
      EventCollection condition = physicsEntityTemplate.Conditions[index3];
      if (condition != null && (condition.Condition.EventConditionType & (EventConditionType.Hit | EventConditionType.Collision)) != (EventConditionType) 0)
      {
        for (int index4 = 0; index4 < condition.Count; ++index4)
        {
          EventStorage eventStorage = condition[index3];
          if (eventStorage.EventType == EventType.Damage)
          {
            eventStorage.DamageEvent.Damage.Magnitude *= 0.25f;
            condition[index3] = eventStorage;
          }
        }
      }
    }
    physicsEntityTemplate.mID = iInput.ReadString().ToLowerInvariant();
    physicsEntityTemplate.mIDHash = physicsEntityTemplate.mID.GetHashCodeCustom();
    bool flag;
    try
    {
      flag = iInput.ReadBoolean();
    }
    catch
    {
      flag = false;
    }
    if (flag)
    {
      physicsEntityTemplate.mRadius = iInput.ReadSingle();
      SkinnedModel skinnedModel = (SkinnedModel) null;
      int length3 = iInput.ReadInt32();
      if (length3 == 0)
      {
        physicsEntityTemplate.mModels = (ModelProperties[]) null;
      }
      else
      {
        physicsEntityTemplate.mModels = new ModelProperties[length3];
        lock (graphicsDevice)
        {
          for (int index = 0; index < length3; ++index)
          {
            ModelProperties modelProperties;
            modelProperties.Model = iInput.ReadExternalReference<SkinnedModel>();
            modelProperties.Scale = iInput.ReadSingle();
            Matrix.CreateScale(modelProperties.Scale, out modelProperties.Transform);
            modelProperties.Tint = iInput.ReadVector3();
            physicsEntityTemplate.mModels[index] = modelProperties;
          }
        }
      }
      if (iInput.ReadBoolean())
      {
        lock (graphicsDevice)
          skinnedModel = iInput.ReadExternalReference<SkinnedModel>();
      }
      physicsEntityTemplate.mSkeleton = skinnedModel == null || skinnedModel.SkeletonBones == null ? (SkinnedModelBoneCollection) null : skinnedModel.SkeletonBones;
      int length4 = iInput.ReadInt32();
      if (length4 == 0)
      {
        physicsEntityTemplate.mAttachedEffects = (KeyValuePair<int, int>[]) null;
      }
      else
      {
        physicsEntityTemplate.mAttachedEffects = new KeyValuePair<int, int>[length4];
        for (int index5 = 0; index5 < length4; ++index5)
        {
          string lowerInvariant = iInput.ReadString().ToLowerInvariant();
          int hashCodeCustom = iInput.ReadString().ToLowerInvariant().GetHashCodeCustom();
          int key = -1;
          if (skinnedModel != null)
          {
            for (int index6 = 0; index6 < skinnedModel.SkeletonBones.Count; ++index6)
            {
              SkinnedModelBone skeletonBone = skinnedModel.SkeletonBones[index6];
              if (skeletonBone.Name.Equals(lowerInvariant, StringComparison.OrdinalIgnoreCase))
                key = (int) skeletonBone.Index;
            }
          }
          physicsEntityTemplate.mAttachedEffects[index5] = key >= 0 ? new KeyValuePair<int, int>(key, hashCodeCustom) : throw new Exception($"Bone \"{lowerInvariant}\" not found!");
        }
      }
      physicsEntityTemplate.mAnimationClips = new AnimationClipAction[27][];
      for (int index7 = 0; index7 < physicsEntityTemplate.mAnimationClips.Length; ++index7)
      {
        int num4 = iInput.ReadInt32();
        if (num4 > 0)
        {
          physicsEntityTemplate.mAnimationClips[index7] = new AnimationClipAction[231];
          for (int index8 = 0; index8 < num4; ++index8)
          {
            Animations iAnimation = (Animations) Enum.Parse(typeof (Animations), iInput.ReadString(), true);
            physicsEntityTemplate.mAnimationClips[index7][(int) iAnimation] = new AnimationClipAction(iAnimation, iInput, skinnedModel.AnimationClips, physicsEntityTemplate.mSkeleton);
          }
        }
      }
      if (PhysicsEntityTemplate.sSkeletonVertexDeclaration == null)
      {
        lock (graphicsDevice)
          PhysicsEntityTemplate.sSkeletonVertexDeclaration = new VertexDeclaration(graphicsDevice, PhysicsEntityTemplate.VertexPositionIndexWeight.VertexElements);
      }
      if (physicsEntityTemplate.mModels != null && physicsEntityTemplate.mModels.Length > 0)
        PhysicsEntityTemplate.GenerateSkeletonModel(physicsEntityTemplate.mModels[0].Model.SkeletonBones, 0.125f * physicsEntityTemplate.mRadius / physicsEntityTemplate.mModels[0].Scale, out physicsEntityTemplate.mSkeletonVertices, out physicsEntityTemplate.mSkeletonPrimitiveCount);
    }
    return physicsEntityTemplate;
  }

  public string Path => this.mPath;

  public bool Movable => this.mMovable;

  public bool Pushable => this.mPushable;

  public bool Solid => this.mSolid;

  public float Mass => this.mMass;

  public bool CanHaveStatus => this.mCanHaveStatus;

  public int MaxHitpoints => this.mMaxHitPoints;

  public GibReference[] Gibs => this.mGibs;

  public PhysicsEntity.VisualEffectStorage[] Effects => this.mEffects;

  public Resistance[] Resistances => this.mResistances;

  public PhysicsEntityTemplate.BoxInfo Box => this.mBox;

  public int HitSound => this.mHitSound;

  public int HitEffect => this.mHitEffect;

  public int GibTrailEffect => this.mGibTrailEffect;

  public int VertexCount
  {
    get => this.mModel.Meshes.Count > 0 ? this.mModel.Meshes[0].MeshParts[0].NumVertices : 0;
  }

  public int VertexStride
  {
    get => this.mModel.Meshes.Count > 0 ? this.mModel.Meshes[0].MeshParts[0].VertexStride : 0;
  }

  public int PrimitiveCount
  {
    get => this.mModel.Meshes.Count > 0 ? this.mModel.Meshes[0].MeshParts[0].PrimitiveCount : 0;
  }

  public VertexBuffer Vertices
  {
    get => this.mModel.Meshes.Count > 0 ? this.mModel.Meshes[0].VertexBuffer : (VertexBuffer) null;
  }

  public VertexDeclaration VertexDeclaration
  {
    get
    {
      return this.mModel.Meshes.Count > 0 ? this.mModel.Meshes[0].MeshParts[0].VertexDeclaration : (VertexDeclaration) null;
    }
  }

  public IndexBuffer Indices
  {
    get => this.mModel.Meshes.Count > 0 ? this.mModel.Meshes[0].IndexBuffer : (IndexBuffer) null;
  }

  public ConditionCollection Conditions
  {
    get => this.mConditions;
    set => this.mConditions = value;
  }

  public RenderDeferredMaterial Material
  {
    get
    {
      RenderDeferredMaterial material = new RenderDeferredMaterial();
      if (this.mModel.Meshes.Count > 0)
        material.FetchFromEffect(this.mModel.Meshes[0].MeshParts[0].Effect as RenderDeferredEffect);
      return material;
    }
  }

  public ModelProperties[] Models => this.mModels;

  public SkinnedModelBoneCollection Skeleton => this.mSkeleton;

  public VertexDeclaration SkeletonVertexDeclaration
  {
    get => PhysicsEntityTemplate.sSkeletonVertexDeclaration;
  }

  public VertexBuffer SkeletonVertices => this.mSkeletonVertices;

  public int SkeletonPrimitiveCount => this.mSkeletonPrimitiveCount;

  public int SkeletonVertexStride => 48 /*0x30*/;

  public AnimationClipAction[][] AnimationClips => this.mAnimationClips;

  private static void GenerateSkeletonModel(
    SkinnedModelBoneCollection iSkeleton,
    float iScale,
    out VertexBuffer oVertexBuffer,
    out int oPrimitiveCount)
  {
    List<PhysicsEntityTemplate.VertexPositionIndexWeight> iVertices = new List<PhysicsEntityTemplate.VertexPositionIndexWeight>();
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
      PhysicsEntityTemplate.GenerateBall(iVertices, result, (float) index, iRadius);
      if (index > 1)
        PhysicsEntityTemplate.GeneratePyramid(iVertices, iChild, iRadius);
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
        oVertexBuffer.SetData<PhysicsEntityTemplate.VertexPositionIndexWeight>(iVertices.ToArray());
      }
      oPrimitiveCount = iVertices.Count - 1;
    }
  }

  private static void GenerateBall(
    List<PhysicsEntityTemplate.VertexPositionIndexWeight> iVertices,
    Matrix iTransform,
    float iBoneIndex,
    float iRadius)
  {
    Vector3 result1 = new Vector3();
    result1.Z = iRadius;
    PhysicsEntityTemplate.VertexPositionIndexWeight positionIndexWeight = new PhysicsEntityTemplate.VertexPositionIndexWeight();
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
    List<PhysicsEntityTemplate.VertexPositionIndexWeight> iVertices,
    SkinnedModelBone iChild,
    float iRadius)
  {
    Matrix result1 = iChild.InverseBindPoseTransform;
    Matrix.Invert(ref result1, out result1);
    Matrix result2 = iChild.Parent.InverseBindPoseTransform;
    Matrix.Invert(ref result2, out result2);
    PhysicsEntityTemplate.VertexPositionIndexWeight positionIndexWeight1 = new PhysicsEntityTemplate.VertexPositionIndexWeight();
    positionIndexWeight1.Offset.Z = -iRadius;
    positionIndexWeight1.Position = result1.Translation;
    positionIndexWeight1.Indices = (float) iChild.Index;
    positionIndexWeight1.NextPosition = result2.Translation;
    positionIndexWeight1.NextIndices = (float) iChild.Parent.Index;
    positionIndexWeight1.Color = Color.White;
    PhysicsEntityTemplate.VertexPositionIndexWeight positionIndexWeight2 = new PhysicsEntityTemplate.VertexPositionIndexWeight();
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

  public struct BoxInfo
  {
    public Vector3 Sides;
    public Vector3 Positon;
    public Quaternion Orientation;
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
