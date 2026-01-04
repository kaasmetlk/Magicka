// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.AnimatedLevelPart
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using JigLibX.Physics;
using Magicka.GameLogic.Entities;
using Magicka.Graphics;
using Magicka.Network;
using Magicka.PathFinding;
using Magicka.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using PolygonHead.Lights;
using System;
using System.Collections.Generic;
using System.IO;
using XNAnimation;
using XNAnimation.Controllers;

#nullable disable
namespace Magicka.Levels;

public class AnimatedLevelPart : IDisposable
{
  private static List<AnimatedLevelPart> mInstances = new List<AnimatedLevelPart>();
  private RenderableAdditiveObject<AdditiveEffect, AdditiveMaterial>[][] mAdditiveRenderData;
  private AnimatedLevelPart.DeferredRenderData[][] mDefaultRenderData;
  private AnimatedLevelPart.HighlightRenderData[][] mHighlightRenderData;
  private Dictionary<int, Locator> mLocators = new Dictionary<int, Locator>();
  private Liquid[] mLiquids;
  private BoundingSphere mBoundingSphere;
  private string mName;
  private int mID;
  private ushort mHandle;
  private Model mModel;
  private AnimationChannel mAnimation;
  private float mAnimationDuration;
  private float mTime;
  private CollisionMaterials mCollisionMaterial;
  private int[] mLights;
  private Matrix[] mLightPositions;
  private LevelModel.VisualEffectStorage[] mEffects;
  private CollisionSkin mCollisionSkin;
  private AnimatedNavMesh mNavMesh;
  private Dictionary<int, AnimatedLevelPart> mChildren;
  private LevelModel mLevel;
  private float mSpeed = 1f;
  private float mStart;
  private float mEnd;
  private bool mLooping;
  private bool mPlaying;
  private float mRestingTimer = -1f;
  private bool mAffectShields = true;
  private float mHighlighted = -1f;
  private Dictionary<string, KeyValuePair<bool, bool>> mMeshSettings;
  private Matrix mOldTransform;
  private SortedList<ushort, float> mCollidingEntities = new SortedList<ushort, float>(32 /*0x20*/);
  private List<DecalManager.DecalReference> mDecals = new List<DecalManager.DecalReference>(64 /*0x40*/);

  public static AnimatedLevelPart GetFromHandle(int iHandle)
  {
    return iHandle >= AnimatedLevelPart.mInstances.Count ? (AnimatedLevelPart) null : AnimatedLevelPart.mInstances[iHandle];
  }

  public static void ClearHandles() => AnimatedLevelPart.mInstances.Clear();

  public AnimatedLevelPart(ContentReader iInput, LevelModel iLevel)
  {
    lock (AnimatedLevelPart.mInstances)
    {
      this.mHandle = (ushort) AnimatedLevelPart.mInstances.Count;
      AnimatedLevelPart.mInstances.Add(this);
    }
    this.mLevel = iLevel;
    this.mName = iInput.ReadString().ToLowerInvariant();
    this.mID = this.mName.GetHashCodeCustom();
    this.mAffectShields = iInput.ReadBoolean();
    this.mModel = iInput.ReadObject<Model>();
    int capacity1 = iInput.ReadInt32();
    this.mMeshSettings = new Dictionary<string, KeyValuePair<bool, bool>>(capacity1);
    for (int index = 0; index < capacity1; ++index)
      this.mMeshSettings.Add(iInput.ReadString(), new KeyValuePair<bool, bool>(iInput.ReadBoolean(), iInput.ReadBoolean()));
    int length1 = iInput.ReadInt32();
    this.mLiquids = new Liquid[length1];
    for (int index = 0; index < length1; ++index)
    {
      this.mLiquids[index] = Liquid.Read(iInput, iLevel, this);
      if (this.mLiquids[index].CollisionSkin != null)
        this.mLiquids[index].CollisionSkin.postCollisionCallbackFn += new PostCollisionCallbackFn(this.PostCollision);
    }
    int num = iInput.ReadInt32();
    for (int index = 0; index < num; ++index)
    {
      string str = iInput.ReadString();
      this.mLocators.Add(str.GetHashCodeCustom(), new Locator(str, iInput));
    }
    this.mAnimationDuration = iInput.ReadSingle();
    this.mStart = 0.0f;
    this.mEnd = this.mAnimationDuration;
    this.mAnimation = AnimationChannel.Read(iInput);
    int length2 = iInput.ReadInt32();
    this.mEffects = new LevelModel.VisualEffectStorage[length2];
    Vector3 up = Vector3.Up;
    for (int index = 0; index < length2; ++index)
    {
      string lowerInvariant1 = iInput.ReadString().ToLowerInvariant();
      LevelModel.VisualEffectStorage visualEffectStorage;
      visualEffectStorage.ID = lowerInvariant1.GetHashCodeCustom();
      Vector3 position = iInput.ReadVector3();
      Vector3 forward = iInput.ReadVector3();
      visualEffectStorage.Range = iInput.ReadSingle();
      string lowerInvariant2 = iInput.ReadString().ToLowerInvariant();
      visualEffectStorage.Effect = lowerInvariant2.GetHashCodeCustom();
      Matrix.CreateWorld(ref position, ref forward, ref up, out visualEffectStorage.Transform);
      this.mEffects[index] = visualEffectStorage;
    }
    int length3 = iInput.ReadInt32();
    this.mLights = new int[length3];
    this.mLightPositions = new Matrix[length3];
    for (int index = 0; index < length3; ++index)
    {
      this.mLights[index] = iInput.ReadString().GetHashCodeCustom();
      this.mLightPositions[index] = iInput.ReadMatrix();
    }
    if (iInput.ReadBoolean())
    {
      this.mCollisionMaterial = (CollisionMaterials) iInput.ReadByte();
      List<Vector3> vertices = iInput.ReadObject<List<Vector3>>();
      int capacity2 = iInput.ReadInt32();
      List<TriangleVertexIndices> triangleVertexIndices1 = new List<TriangleVertexIndices>(capacity2);
      for (int index = 0; index < capacity2; ++index)
      {
        TriangleVertexIndices triangleVertexIndices2;
        triangleVertexIndices2.I0 = iInput.ReadInt32();
        triangleVertexIndices2.I1 = iInput.ReadInt32();
        triangleVertexIndices2.I2 = iInput.ReadInt32();
        triangleVertexIndices1.Add(triangleVertexIndices2);
      }
      TriangleMesh prim = new TriangleMesh();
      prim.CreateMesh(vertices, triangleVertexIndices1, 16 /*0x10*/, 2f);
      this.mCollisionSkin = new CollisionSkin((Body) null);
      this.mCollisionSkin.AddPrimitive((Primitive) prim, 1, new MaterialProperties(0.1f, 1f, 1f));
      this.mCollisionSkin.ApplyLocalTransform(Transform.Identity);
      this.mCollisionSkin.Tag = (object) iLevel;
      this.mCollisionSkin.postCollisionCallbackFn += new PostCollisionCallbackFn(this.PostCollision);
    }
    if (iInput.ReadBoolean())
      this.mNavMesh = new AnimatedNavMesh(iInput);
    int capacity3 = iInput.ReadInt32();
    this.mChildren = new Dictionary<int, AnimatedLevelPart>(capacity3);
    for (int index = 0; index < capacity3; ++index)
    {
      AnimatedLevelPart animatedLevelPart = new AnimatedLevelPart(iInput, iLevel);
      this.mChildren.Add(animatedLevelPart.mName.GetHashCodeCustom(), animatedLevelPart);
    }
  }

  public CollisionMaterials CollisionMaterial => this.mCollisionMaterial;

  public ushort Handle => this.mHandle;

  private void PostCollision(ref CollisionInfo iInfo)
  {
    if (!this.mPlaying)
      return;
    CollisionSkin collisionSkin = iInfo.SkinInfo.Skin0;
    float num = iInfo.DirToBody0.Y;
    if (collisionSkin == this.mCollisionSkin)
    {
      collisionSkin = iInfo.SkinInfo.Skin1;
      num = -num;
    }
    if (collisionSkin.Owner == null || (!(collisionSkin.Owner.Tag is Entity tag) || !(tag is Shield)) && (double) num <= 0.699999988079071)
      return;
    float val2;
    if (!this.mCollidingEntities.TryGetValue(tag.Handle, out val2))
      val2 = 0.0f;
    this.mCollidingEntities[tag.Handle] = System.Math.Max(0.25f, val2);
    collisionSkin.Owner.SetActive();
  }

  public void Initialize(ref Matrix iParent)
  {
    this.GetTransform(out this.mOldTransform);
    Matrix.Multiply(ref iParent, ref this.mOldTransform, out this.mOldTransform);
    if (this.mCollisionSkin != null)
    {
      Transform transform;
      transform.Orientation = this.mOldTransform;
      transform.Orientation.M41 = 0.0f;
      transform.Orientation.M42 = 0.0f;
      transform.Orientation.M43 = 0.0f;
      transform.Position = this.mOldTransform.Translation;
      this.mCollisionSkin.SetTransform(ref transform, ref transform);
    }
    this.mAdditiveRenderData = new RenderableAdditiveObject<AdditiveEffect, AdditiveMaterial>[3][];
    this.mDefaultRenderData = new AnimatedLevelPart.DeferredRenderData[3][];
    this.mHighlightRenderData = new AnimatedLevelPart.HighlightRenderData[3][];
    int length1 = 0;
    int length2 = 0;
    int num = 0;
    this.mHighlighted = -1f;
    if (this.mNavMesh != null)
    {
      this.mNavMesh.UpdateTransform(ref this.mOldTransform);
      this.mLevel.NavMesh.AnimatedParts.Add(this.mNavMesh);
    }
    for (int index1 = 0; index1 < this.mModel.Meshes.Count; ++index1)
    {
      ModelMesh mesh = this.mModel.Meshes[index1];
      for (int index2 = 0; index2 < mesh.MeshParts.Count; ++index2)
      {
        ModelMeshPart meshPart = mesh.MeshParts[index2];
        if (meshPart.Effect is AdditiveEffect)
          ++length1;
        else if (meshPart.Effect is RenderDeferredEffect)
        {
          ++length2;
        }
        else
        {
          if (!(meshPart.Effect is RenderDeferredLiquidEffect))
            throw new Exception("Invalid effect type!");
          ++num;
        }
      }
      this.mBoundingSphere = index1 != 0 ? BoundingSphere.CreateMerged(this.mBoundingSphere, mesh.BoundingSphere) : mesh.BoundingSphere;
    }
    for (int index3 = 0; index3 < 3; ++index3)
    {
      RenderableAdditiveObject<AdditiveEffect, AdditiveMaterial>[] renderableAdditiveObjectArray = new RenderableAdditiveObject<AdditiveEffect, AdditiveMaterial>[length1];
      AnimatedLevelPart.DeferredRenderData[] deferredRenderDataArray = new AnimatedLevelPart.DeferredRenderData[length2];
      AnimatedLevelPart.HighlightRenderData[] highlightRenderDataArray = new AnimatedLevelPart.HighlightRenderData[length2];
      this.mAdditiveRenderData[index3] = renderableAdditiveObjectArray;
      this.mDefaultRenderData[index3] = deferredRenderDataArray;
      this.mHighlightRenderData[index3] = highlightRenderDataArray;
      int index4 = 0;
      int index5 = 0;
      for (int index6 = 0; index6 < this.mModel.Meshes.Count; ++index6)
      {
        ModelMesh mesh = this.mModel.Meshes[index6];
        KeyValuePair<bool, bool> mMeshSetting = this.mMeshSettings[mesh.Name];
        for (int index7 = 0; index7 < mesh.MeshParts.Count; ++index7)
        {
          ModelMeshPart meshPart = mesh.MeshParts[index7];
          AdditiveEffect effect1 = meshPart.Effect as AdditiveEffect;
          RenderDeferredEffect effect2 = meshPart.Effect as RenderDeferredEffect;
          if (effect1 != null)
          {
            renderableAdditiveObjectArray[index4] = new RenderableAdditiveObject<AdditiveEffect, AdditiveMaterial>();
            renderableAdditiveObjectArray[index4++].SetMesh(mesh, meshPart, 0);
          }
          else if (effect2 != null)
          {
            deferredRenderDataArray[index5] = new AnimatedLevelPart.DeferredRenderData(mMeshSetting.Key, mMeshSetting.Value);
            highlightRenderDataArray[index5] = new AnimatedLevelPart.HighlightRenderData(mMeshSetting.Key);
            highlightRenderDataArray[index5].SetMesh(mesh.VertexBuffer, mesh.IndexBuffer, meshPart, RenderDeferredEffect.TYPEHASH);
            VertexDeclaration vertexDeclaration = meshPart.VertexDeclaration;
            RenderDeferredEffect.Technique iTechnique = effect2.ReflectionMap == null ? (effect2.DiffuseTexture1 == null ? RenderDeferredEffect.Technique.SingleLayer : RenderDeferredEffect.Technique.DualLayer) : (effect2.DiffuseTexture1 == null ? RenderDeferredEffect.Technique.SingleLayerReflection : RenderDeferredEffect.Technique.DualLayerReflection);
            deferredRenderDataArray[index5++].SetMesh(mesh, meshPart, vertexDeclaration, 4, (int) iTechnique, 5);
          }
        }
      }
    }
    foreach (AnimatedLevelPart animatedLevelPart in this.mChildren.Values)
      animatedLevelPart.Initialize(ref this.mOldTransform);
  }

  public void Update(
    DataChannel iDataChannel,
    float iDeltaTime,
    ref Matrix iParent,
    GameScene iScene)
  {
    if (this.mPlaying)
    {
      this.mRestingTimer = 1f;
      float mTime = this.mTime;
      this.mTime += iDeltaTime * this.mSpeed;
      if ((double) this.mSpeed > 0.0)
      {
        if ((double) mTime <= (double) this.mEnd && (double) this.mTime > (double) this.mEnd)
        {
          if (this.mLooping)
          {
            this.mTime -= this.mEnd - this.mStart;
          }
          else
          {
            this.mTime = this.mEnd;
            this.mPlaying = false;
          }
        }
      }
      else if ((double) mTime >= (double) this.mEnd && (double) this.mTime < (double) this.mEnd)
      {
        if (this.mLooping)
        {
          this.mTime += this.mEnd - this.mStart;
        }
        else
        {
          this.mTime = this.mEnd;
          this.mPlaying = false;
        }
      }
    }
    Matrix matrix;
    this.GetTransform(out matrix);
    Matrix.Multiply(ref matrix, ref iParent, out matrix);
    for (int index = 0; index < this.mLights.Length; ++index)
    {
      Matrix result;
      Matrix.Multiply(ref this.mLightPositions[index], ref matrix, out result);
      Light light = this.mLevel.Lights[this.mLights[index]];
      PointLight pointLight = light as PointLight;
      SpotLight spotLight = light as SpotLight;
      DirectionalLight directionalLight = light as DirectionalLight;
      if (pointLight != null)
        pointLight.Position = result.Translation;
      else if (spotLight != null)
      {
        spotLight.Position = result.Translation;
        spotLight.Direction = result.Forward;
      }
      else if (directionalLight != null)
        directionalLight.LightDirection = result.Forward;
    }
    Transform transform;
    if (this.mCollisionSkin != null)
    {
      transform = this.mCollisionSkin.NewTransform;
      this.mCollisionSkin.SetOldTransform(ref transform);
      transform.Orientation = matrix;
      transform.Orientation.M41 = 0.0f;
      transform.Orientation.M42 = 0.0f;
      transform.Orientation.M43 = 0.0f;
      transform.Position = matrix.Translation;
      this.mCollisionSkin.SetNewTransform(ref transform);
      this.mCollisionSkin.UpdateWorldBoundingBox();
    }
    Matrix result1;
    Matrix.Invert(ref this.mOldTransform, out result1);
    Matrix.Multiply(ref result1, ref matrix, out result1);
    for (int index = 0; index < this.mCollidingEntities.Count; ++index)
    {
      ushort key = this.mCollidingEntities.Keys[index];
      Entity fromHandle = Entity.GetFromHandle((int) key);
      float num = this.mCollidingEntities[key] - iDeltaTime;
      this.mCollidingEntities[key] = num;
      if ((double) num <= 0.0 || fromHandle.Removable || !this.mAffectShields && fromHandle is Shield)
      {
        this.mCollidingEntities.RemoveAt(index);
        --index;
      }
      Body body = fromHandle.Body;
      transform = body.Transform;
      transform.Orientation.Translation = transform.Position;
      Matrix.Multiply(ref transform.Orientation, ref result1, out transform.Orientation);
      transform.Position = transform.Orientation.Translation;
      transform.Orientation.Translation = new Vector3();
      if (body is CharacterBody characterBody)
      {
        Vector3 result2 = characterBody.DesiredDirection;
        Vector3.TransformNormal(ref result2, ref result1, out result2);
        characterBody.DesiredDirection = result2;
      }
      body.Transform = transform;
    }
    for (int index = 0; index < this.mDecals.Count; ++index)
    {
      DecalManager.DecalReference mDecal = this.mDecals[index];
      if (!DecalManager.Instance.TransformDecal(ref mDecal, ref result1))
        this.mDecals.RemoveAt(index--);
      else
        this.mDecals[index] = mDecal;
    }
    if (this.mNavMesh != null & this.mOldTransform != matrix)
      this.mNavMesh.UpdateTransform(ref matrix);
    this.mOldTransform = matrix;
    if (iDataChannel != DataChannel.None)
    {
      RenderableAdditiveObject<AdditiveEffect, AdditiveMaterial>[] renderableAdditiveObjectArray = this.mAdditiveRenderData[(int) iDataChannel];
      RenderableObject<RenderDeferredEffect, RenderDeferredMaterial>[] renderableObjectArray = (RenderableObject<RenderDeferredEffect, RenderDeferredMaterial>[]) this.mDefaultRenderData[(int) iDataChannel];
      AnimatedLevelPart.HighlightRenderData[] highlightRenderDataArray = this.mHighlightRenderData[(int) iDataChannel];
      for (int index = 0; index < renderableAdditiveObjectArray.Length; ++index)
      {
        RenderableAdditiveObject<AdditiveEffect, AdditiveMaterial> iObject = renderableAdditiveObjectArray[index];
        iObject.mBoundingSphere.Radius = this.mBoundingSphere.Radius;
        Vector3.Transform(ref this.mBoundingSphere.Center, ref matrix, out iObject.mBoundingSphere.Center);
        iObject.mMaterial.WorldTransform = matrix;
        iScene.PlayState.Scene.AddRenderableAdditiveObject(iDataChannel, (IRenderableAdditiveObject) iObject);
      }
      for (int index = 0; index < renderableObjectArray.Length; ++index)
      {
        RenderableObject<RenderDeferredEffect, RenderDeferredMaterial> iObject = renderableObjectArray[index];
        iObject.mBoundingSphere.Radius = this.mBoundingSphere.Radius;
        Vector3.Transform(ref this.mBoundingSphere.Center, ref matrix, out iObject.mBoundingSphere.Center);
        iObject.mMaterial.WorldTransform = matrix;
        iScene.PlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) iObject);
      }
      if ((double) this.mHighlighted >= 0.0)
      {
        for (int index = 0; index < highlightRenderDataArray.Length; ++index)
        {
          AnimatedLevelPart.HighlightRenderData iObject = highlightRenderDataArray[index];
          iObject.mBoundingSphere.Radius = this.mBoundingSphere.Radius;
          Vector3.Transform(ref this.mBoundingSphere.Center, ref matrix, out iObject.mBoundingSphere.Center);
          iObject.mTransform = matrix;
          iScene.PlayState.Scene.AddRenderableAdditiveObject(iDataChannel, (IRenderableAdditiveObject) iObject);
        }
      }
      if (this.mLiquids.Length > 0)
      {
        Matrix result3;
        Matrix.Invert(ref matrix, out result3);
        for (int index = 0; index < this.mLiquids.Length; ++index)
          this.mLiquids[index].Update(iDataChannel, iDeltaTime, iScene.PlayState.Scene, ref matrix, ref result3);
      }
    }
    foreach (AnimatedLevelPart animatedLevelPart in this.mChildren.Values)
      animatedLevelPart.Update(iDataChannel, iDeltaTime, ref matrix, iScene);
    this.mHighlighted -= iDeltaTime;
  }

  public bool IsTouchingEntity(ushort iHandle, bool iCheckChildren)
  {
    float num;
    if (this.mCollidingEntities.TryGetValue(iHandle, out num))
      return (double) num > 0.0;
    if (iCheckChildren)
    {
      foreach (AnimatedLevelPart animatedLevelPart in this.mChildren.Values)
      {
        if (animatedLevelPart.IsTouchingEntity(iHandle, iCheckChildren))
          return true;
      }
    }
    return false;
  }

  private void GetTransform(out Matrix oTransform)
  {
    int keyframeIndexByTime = this.mAnimation.GetKeyframeIndexByTime(this.mTime);
    int index = (keyframeIndexByTime + 1) % this.mAnimation.Count;
    AnimationChannelKeyframe animationChannelKeyframe1 = this.mAnimation[keyframeIndexByTime];
    AnimationChannelKeyframe animationChannelKeyframe2 = this.mAnimation[index];
    float num = index != 0 ? animationChannelKeyframe2.Time - animationChannelKeyframe1.Time : 0.0f;
    if ((double) num > 0.0)
    {
      float amount = (this.mTime - animationChannelKeyframe1.Time) / num;
      Pose resultPose;
      Pose.Interpolate(ref animationChannelKeyframe1.Pose, ref animationChannelKeyframe2.Pose, amount, InterpolationMode.Linear, InterpolationMode.Linear, InterpolationMode.Linear, out resultPose);
      resultPose.GetMatrix(out oTransform);
    }
    else
      animationChannelKeyframe1.Pose.GetMatrix(out oTransform);
  }

  public void Highlight(float iTTL) => this.mHighlighted = iTTL;

  public void RegisterCollisionSkin()
  {
    if (this.mCollisionSkin != null && !PhysicsManager.Instance.Simulator.CollisionSystem.CollisionSkins.Contains(this.mCollisionSkin))
      PhysicsManager.Instance.Simulator.CollisionSystem.AddCollisionSkin(this.mCollisionSkin);
    for (int index = 0; index < this.mLiquids.Length; ++index)
      this.mLiquids[index].Initialize();
    foreach (AnimatedLevelPart animatedLevelPart in this.mChildren.Values)
      animatedLevelPart.RegisterCollisionSkin();
  }

  public Matrix AbsoluteTransform => this.mOldTransform;

  public CollisionSkin CollisionSkin => this.mCollisionSkin;

  public AnimatedLevelPart GetChild(int iId) => this.mChildren[iId];

  public string Name => this.mName;

  public int ID => this.mID;

  public float AnimationDuration => this.mAnimationDuration;

  public float Time => this.mTime;

  public void Play(
    bool iAllChildren,
    float iStart,
    float iEnd,
    float iSpeed,
    bool iLoop,
    bool iResume)
  {
    if (iAllChildren)
    {
      foreach (AnimatedLevelPart animatedLevelPart in this.mChildren.Values)
        animatedLevelPart.Play(true, iStart, iEnd, iSpeed, iLoop, false);
    }
    this.mSpeed = iSpeed;
    if ((double) iStart < 0.0)
      iStart = 0.0f;
    if ((double) iEnd < 0.0)
      iEnd = this.mAnimationDuration;
    if ((double) iSpeed < 0.0)
    {
      this.mStart = System.Math.Max(iStart, iEnd);
      this.mEnd = System.Math.Min(iStart, iEnd);
    }
    else
    {
      this.mStart = System.Math.Min(iStart, iEnd);
      this.mEnd = System.Math.Max(iStart, iEnd);
    }
    if (iResume)
    {
      if ((double) iSpeed >= 0.0)
      {
        if ((double) this.mTime < (double) this.mStart)
          this.mTime = this.mStart;
        else if ((double) this.mTime > (double) this.mEnd)
          this.mTime = this.mEnd;
      }
      else if ((double) this.mTime > (double) this.mStart)
        this.mTime = this.mStart;
      else if ((double) this.mTime < (double) this.mEnd)
        this.mTime = this.mEnd;
    }
    else
      this.mTime = this.mStart;
    this.mLooping = iLoop;
    this.mPlaying = true;
  }

  public void Stop(bool iAllChildren)
  {
    if (NetworkManager.Instance.State == NetworkState.Server && NetworkManager.Instance.Interface is NetworkServer networkServer)
      networkServer.SendUdpMessage<AnimatedLevelPartUpdateMessage>(ref new AnimatedLevelPartUpdateMessage()
      {
        Playing = false,
        AnimationTime = this.mTime
      });
    if (iAllChildren)
    {
      foreach (AnimatedLevelPart animatedLevelPart in this.mChildren.Values)
        animatedLevelPart.Stop(true);
    }
    this.mPlaying = false;
  }

  internal void Resume(bool iChildren, float iLength, float iSpeed, bool? iLooping)
  {
    if (iChildren)
    {
      foreach (AnimatedLevelPart animatedLevelPart in this.mChildren.Values)
        animatedLevelPart.Resume(true, iLength, iSpeed, iLooping);
    }
    this.mSpeed = iSpeed;
    if (iLooping.HasValue)
      this.mLooping = iLooping.Value;
    if ((double) iLength > 1.4012984643248171E-45)
    {
      this.mEnd = (double) iSpeed >= 0.0 ? this.mTime + iLength : this.mTime - iLength;
    }
    else
    {
      if ((double) iLength < -1.4012984643248171E-45)
        throw new Exception("Negative length is not an accepted value");
      this.mEnd = this.mAnimationDuration;
    }
    this.mPlaying = true;
  }

  internal void AddStateTo(
    Dictionary<int, AnimatedLevelPart.AnimationState> iAnimationStates)
  {
    AnimatedLevelPart.AnimationState animationState;
    animationState.Start = this.mStart;
    animationState.End = this.mEnd;
    animationState.Loop = this.mLooping;
    animationState.Playing = this.mPlaying;
    animationState.Time = this.mTime;
    iAnimationStates.Add(this.mID, animationState);
    foreach (AnimatedLevelPart animatedLevelPart in this.mChildren.Values)
      animatedLevelPart.AddStateTo(iAnimationStates);
  }

  internal void RestoreStateFrom(
    Dictionary<int, AnimatedLevelPart.AnimationState> iAnimationStates)
  {
    AnimatedLevelPart.AnimationState animationState;
    if (iAnimationStates.TryGetValue(this.mID, out animationState))
    {
      this.mStart = animationState.Start;
      this.mEnd = animationState.End;
      this.mLooping = animationState.Loop;
      this.mPlaying = animationState.Playing;
      this.mTime = animationState.Time;
    }
    foreach (AnimatedLevelPart animatedLevelPart in this.mChildren.Values)
      animatedLevelPart.RestoreStateFrom(iAnimationStates);
  }

  internal void NetworkUpdate(NetworkServer iServer)
  {
    if ((double) this.mRestingTimer > 0.0)
    {
      AnimatedLevelPartUpdateMessage iMessage = new AnimatedLevelPartUpdateMessage();
      iMessage.Handle = this.mHandle;
      iMessage.Playing = this.mPlaying;
      for (int index = 0; index < iServer.Connections; ++index)
      {
        float num = this.mPlaying ? iServer.GetLatency(index) * 0.5f : 0.0f;
        iMessage.AnimationTime = this.mTime + num;
        iServer.SendUdpMessage<AnimatedLevelPartUpdateMessage>(ref iMessage, index);
      }
    }
    foreach (AnimatedLevelPart animatedLevelPart in this.mChildren.Values)
      animatedLevelPart.NetworkUpdate(iServer);
  }

  internal void NetworkUpdate(ref AnimatedLevelPartUpdateMessage iMsg)
  {
    this.mTime = iMsg.AnimationTime;
    this.mPlaying = iMsg.Playing;
  }

  internal void AddDecal(ref DecalManager.DecalReference iDecal) => this.mDecals.Add(iDecal);

  internal void AddEntity(Entity iEntity)
  {
    this.mCollidingEntities[iEntity.Handle] = float.MaxValue;
  }

  internal void RemoveEntity(Entity iEntity) => this.mCollidingEntities.Remove(iEntity.Handle);

  internal bool SegmentIntersect(
    out float oFrac,
    out Vector3 oPos,
    out Vector3 oNrm,
    out AnimatedLevelPart oAnim,
    Segment iSeg)
  {
    oFrac = float.MaxValue;
    oPos = new Vector3();
    oNrm = new Vector3();
    oAnim = (AnimatedLevelPart) null;
    float num;
    Vector3 vector3_1;
    Vector3 vector3_2;
    if (this.mCollisionSkin != null && this.mCollisionSkin.SegmentIntersect(out num, out vector3_1, out vector3_2, iSeg))
    {
      oFrac = num;
      oPos = vector3_1;
      oNrm = vector3_2;
      oAnim = this;
    }
    foreach (AnimatedLevelPart animatedLevelPart in this.mChildren.Values)
    {
      AnimatedLevelPart oAnim1;
      if (animatedLevelPart.SegmentIntersect(out num, out vector3_1, out vector3_2, out oAnim1, iSeg) && (double) num < (double) oFrac)
      {
        oFrac = num;
        oPos = vector3_1;
        oNrm = vector3_2;
        oAnim = oAnim1;
      }
    }
    return (double) oFrac <= 1.0;
  }

  public void Dispose()
  {
    foreach (ModelMesh mesh in this.mModel.Meshes)
    {
      mesh.VertexBuffer.Dispose();
      mesh.IndexBuffer.Dispose();
      foreach (ModelMeshPart meshPart in mesh.MeshParts)
      {
        meshPart.Effect.Dispose();
        meshPart.VertexDeclaration.Dispose();
      }
    }
    foreach (AnimatedLevelPart animatedLevelPart in this.mChildren.Values)
      animatedLevelPart.Dispose();
    this.mLevel = (LevelModel) null;
  }

  internal bool TryGetLocator(int iId, ref Matrix iParentTransform, out Locator oLocator)
  {
    Matrix mOldTransform = this.mOldTransform;
    if (this.mLocators.TryGetValue(iId, out oLocator))
    {
      Matrix.Multiply(ref oLocator.Transform, ref mOldTransform, out oLocator.Transform);
      return true;
    }
    foreach (AnimatedLevelPart animatedLevelPart in this.mChildren.Values)
    {
      if (animatedLevelPart.TryGetLocator(iId, ref mOldTransform, out oLocator))
        return true;
    }
    return false;
  }

  internal void GetLiquids(List<Liquid> iLiquids)
  {
    iLiquids.AddRange((IEnumerable<Liquid>) this.mLiquids);
    foreach (AnimatedLevelPart animatedLevelPart in this.mChildren.Values)
      animatedLevelPart.GetLiquids(iLiquids);
  }

  internal void GetAllEffects(SortedList<int, GameScene.EffectStorage> iEffects)
  {
    for (int index = 0; index < this.mEffects.Length; ++index)
    {
      GameScene.EffectStorage effectStorage;
      effectStorage.Effect = EffectManager.Instance.GetEffect(this.mEffects[index].Effect);
      effectStorage.Transform = this.mEffects[index].Transform;
      effectStorage.Animation = this;
      effectStorage.Range = this.mEffects[index].Range;
      Matrix result;
      Matrix.Multiply(ref effectStorage.Transform, ref this.mOldTransform, out result);
      effectStorage.Effect.Start(ref result);
      iEffects.Add(this.mEffects[index].ID, effectStorage);
    }
    foreach (AnimatedLevelPart animatedLevelPart in this.mChildren.Values)
      animatedLevelPart.GetAllEffects(iEffects);
  }

  public struct AnimationState
  {
    public float Start;
    public float End;
    public bool Loop;
    public bool Playing;
    public float Time;

    public AnimationState(BinaryReader iReader)
    {
      this.Start = iReader.ReadSingle();
      this.End = iReader.ReadSingle();
      this.Loop = iReader.ReadBoolean();
      this.Playing = iReader.ReadBoolean();
      this.Time = iReader.ReadSingle();
    }

    public void Write(BinaryWriter iWriter)
    {
      iWriter.Write(this.Start);
      iWriter.Write(this.End);
      iWriter.Write(this.Loop);
      iWriter.Write(this.Playing);
      iWriter.Write(this.Time);
    }
  }

  protected class DeferredRenderData : RenderableObject<RenderDeferredEffect, RenderDeferredMaterial>
  {
    private bool mVisible;
    private bool mCastShadows;

    public DeferredRenderData(bool iVisible, bool iCastShadows)
    {
      this.mVisible = iVisible;
      this.mCastShadows = iCastShadows;
    }

    public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      if (!this.mVisible)
        return;
      iEffect.GraphicsDevice.RenderState.ReferenceStencil = 1;
      base.Draw(iEffect, iViewFrustum);
      iEffect.GraphicsDevice.RenderState.ReferenceStencil = 0;
    }

    public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      if (iEffect.CurrentTechnique == iEffect.Techniques[this.ShadowTechnique] && !this.mCastShadows || iEffect.CurrentTechnique == iEffect.Techniques[this.DepthTechnique] && !this.mVisible)
        return;
      base.DrawShadow(iEffect, iViewFrustum);
    }

    public void SetMesh(
      ModelMesh iMesh,
      ModelMeshPart iPart,
      VertexDeclaration iVertexDeclaration,
      int iDepthTechnique,
      int iTechnique,
      int iShadowTechnique)
    {
      this.SetMesh(iMesh, iPart, iDepthTechnique, iTechnique, iShadowTechnique);
      this.mVertexDeclaration = iVertexDeclaration;
    }
  }

  protected class HighlightRenderData : IRenderableAdditiveObject
  {
    protected VertexDeclaration mVertexDeclaration;
    protected int mBaseVertex;
    protected int mNumVertices;
    protected int mPrimitiveCount;
    protected int mStartIndex;
    protected int mStreamOffset;
    protected int mVertexStride;
    protected VertexBuffer mVertexBuffer;
    protected int mVerticesHash;
    protected IndexBuffer mIndexBuffer;
    protected int mEffect;
    protected RenderDeferredMaterial mMaterial;
    public BoundingSphere mBoundingSphere;
    public Matrix mTransform;
    protected bool mMeshDirty = true;
    protected bool mVisible;

    public HighlightRenderData(bool iVisible) => this.mVisible = iVisible;

    public int Effect => this.mEffect;

    public int Technique => 6;

    public VertexBuffer Vertices => this.mVertexBuffer;

    public int VerticesHashCode => this.mVerticesHash;

    public int VertexStride => this.mVertexStride;

    public IndexBuffer Indices => this.mIndexBuffer;

    public VertexDeclaration VertexDeclaration => this.mVertexDeclaration;

    public bool Cull(BoundingFrustum iViewFrustum)
    {
      return this.mBoundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
    }

    public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      if (!this.mVisible)
        return;
      RenderDeferredEffect iEffect1 = iEffect as RenderDeferredEffect;
      this.mMaterial.AssignToEffect(iEffect1);
      iEffect1.DiffuseColor0 = new Vector3(1f);
      iEffect1.FresnelPower = 1f;
      iEffect1.World = this.mTransform;
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
      iEffect1.DiffuseColor0 = Vector3.One;
    }

    public bool MeshDirty => this.mMeshDirty;

    public void SetMeshDirty() => this.mMeshDirty = true;

    public void SetMesh(
      VertexBuffer iVertices,
      IndexBuffer iIndices,
      ModelMeshPart iMeshPart,
      int iEffectHash)
    {
      this.mMeshDirty = false;
      this.mVertexBuffer = iVertices;
      this.mVerticesHash = iVertices.GetHashCode();
      this.mIndexBuffer = iIndices;
      this.mEffect = iEffectHash;
      this.mVertexDeclaration = iMeshPart.VertexDeclaration;
      this.mBaseVertex = iMeshPart.BaseVertex;
      this.mNumVertices = iMeshPart.NumVertices;
      this.mPrimitiveCount = iMeshPart.PrimitiveCount;
      this.mStartIndex = iMeshPart.StartIndex;
      this.mStreamOffset = iMeshPart.StreamOffset;
      this.mVertexStride = iMeshPart.VertexStride;
      this.mMaterial.FetchFromEffect(iMeshPart.Effect as RenderDeferredEffect);
    }
  }
}
