// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Gib
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using JigLibX.Physics;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities;

public class Gib : Entity
{
  private static List<Gib> GibCache;
  public static readonly int[] GORE_GIB_SMALL_EFFECTS = new int[6]
  {
    "gore_gib_regular_small".GetHashCodeCustom(),
    "gore_gib_green_small".GetHashCodeCustom(),
    "gore_gib_black_small".GetHashCodeCustom(),
    "gore_gib_wood_small".GetHashCodeCustom(),
    "gore_gib_insect_small".GetHashCodeCustom(),
    "".GetHashCodeCustom()
  };
  public static readonly int[] GORE_GIB_MEDIUM_EFFECTS = new int[6]
  {
    "gore_gib_regular_medium".GetHashCodeCustom(),
    "gore_gib_green_medium".GetHashCodeCustom(),
    "gore_gib_black_medium".GetHashCodeCustom(),
    "gore_gib_wood_medium".GetHashCodeCustom(),
    "gore_gib_insect_medium".GetHashCodeCustom(),
    "".GetHashCodeCustom()
  };
  public static readonly int[] GORE_GIB_LARGE_EFFECTS = new int[6]
  {
    "gore_gib_regular_large".GetHashCodeCustom(),
    "gore_gib_green_large".GetHashCodeCustom(),
    "gore_gib_black_large".GetHashCodeCustom(),
    "gore_gib_wood_large".GetHashCodeCustom(),
    "gore_gib_insect_large".GetHashCodeCustom(),
    "".GetHashCodeCustom()
  };
  public static readonly int[] GORE_GIB_TRAIL_EFFECTS = new int[6]
  {
    "gore_gib_regular_trail".GetHashCodeCustom(),
    "gore_gib_green_trail".GetHashCodeCustom(),
    "gore_gib_black_trail".GetHashCodeCustom(),
    "gore_gib_wood_trail".GetHashCodeCustom(),
    "gore_gib_insect_trail".GetHashCodeCustom(),
    "".GetHashCodeCustom()
  };
  public static readonly int[] GORE_SPLASH_EFFECTS = new int[6]
  {
    "gore_splash_regular".GetHashCodeCustom(),
    "gore_splash_green".GetHashCodeCustom(),
    "gore_splash_black".GetHashCodeCustom(),
    "gore_splash_wood".GetHashCodeCustom(),
    "gore_splash_insect".GetHashCodeCustom(),
    "".GetHashCodeCustom()
  };
  private Gib.RenderData[] mRenderData;
  private Model mModel;
  private ModelMesh mMesh;
  private ModelMeshPart mMeshPart;
  private float mTTL;
  private float mScale = 1f;
  private BloodType mBloodType;
  private bool mFrozen;
  private VisualEffectReference mBloodEffect;
  private VisualEffectReference mTrailEffect;

  public static void InitializeCache(int size, PlayState p)
  {
    Gib.GibCache = new List<Gib>(size);
    for (int index = 0; index < size; ++index)
      Gib.GibCache.Add(new Gib(p));
  }

  public static Gib GetFromCache()
  {
    if (Gib.GibCache.Count <= 0)
      return (Gib) null;
    Gib fromCache = Gib.GibCache[Gib.GibCache.Count - 1];
    Gib.GibCache.RemoveAt(Gib.GibCache.Count - 1);
    return fromCache;
  }

  private static void ReturnGib(Gib iGib)
  {
    if (Gib.GibCache.Contains(iGib))
      throw new Exception("FAIL");
    Gib.GibCache.Add(iGib);
  }

  public Gib(PlayState iPlayState)
    : base(iPlayState)
  {
    this.mRenderData = new Gib.RenderData[3];
    for (int index = 0; index < 3; ++index)
      this.mRenderData[index] = new Gib.RenderData();
    this.mBody = new Body();
    this.mCollision = new CollisionSkin(this.mBody);
    this.mCollision.AddPrimitive((Primitive) new Box(new Vector3(0.5f), Matrix.Identity, new Vector3(1f)), 1, new MaterialProperties(0.0f, 0.8f, 0.8f));
    this.mBody.CollisionSkin = this.mCollision;
    this.mCollision.callbackFn += new CollisionCallbackFn(this.OnCollision);
    this.mBody.Immovable = false;
    this.mBody.Tag = (object) this;
  }

  public void Initialize(
    Model iModel,
    float iMass,
    float iScale,
    Vector3 iPosition,
    Vector3 iVelocity,
    float iTTL,
    Entity iOwner,
    BloodType iBloodType,
    int iTrailEffect,
    bool iFrozen)
  {
    this.mScale = iScale;
    this.mBloodType = iBloodType;
    this.mFrozen = iFrozen;
    this.Model = iModel;
    this.mBody.Mass = iMass;
    this.mTTL = iTTL;
    this.mRadius = this.mMesh.BoundingSphere.Radius;
    Vector3 vector3_1 = new Vector3(this.mMesh.BoundingSphere.Radius);
    Vector3 vector3_2 = -this.mMesh.BoundingSphere.Center;
    (this.mCollision.GetPrimitiveLocal(0) as Box).SideLengths = vector3_1;
    (this.mCollision.GetPrimitiveLocal(0) as Box).Position = vector3_2;
    (this.mCollision.GetPrimitiveNewWorld(0) as Box).SideLengths = vector3_1;
    (this.mCollision.GetPrimitiveNewWorld(0) as Box).Position = vector3_2;
    (this.mCollision.GetPrimitiveOldWorld(0) as Box).SideLengths = vector3_1;
    (this.mCollision.GetPrimitiveOldWorld(0) as Box).Position = vector3_2;
    Vector3 vector3_3 = this.SetMass(iMass);
    Transform transform = new Transform();
    Vector3.Negate(ref vector3_3, out transform.Position);
    transform.Orientation = Matrix.Identity;
    this.mCollision.ApplyLocalTransform(transform);
    this.mBody.MoveTo(iPosition, Matrix.Identity);
    this.mBody.AllowFreezing = false;
    this.mCollision.NonCollidables.Clear();
    if (iOwner != null)
      this.mCollision.NonCollidables.Add(iOwner.Body.CollisionSkin);
    this.mBody.Velocity = iVelocity;
    this.mBody.EnableBody();
    for (int index = 0; index < 3; ++index)
      this.mRenderData[index].mFrozen = this.mFrozen;
    if (this.mFrozen || iTrailEffect == 0 || iTrailEffect == Gib.GORE_GIB_TRAIL_EFFECTS[5])
      return;
    Matrix orientation = this.mBody.Orientation;
    EffectManager.Instance.StartEffect(iTrailEffect, ref orientation, out this.mTrailEffect);
  }

  public Model Model
  {
    get => this.mModel;
    set
    {
      this.mModel = value;
      if (this.mRenderData != null)
      {
        for (int index = 0; index < 3; ++index)
          this.mRenderData[index].SetMeshDirty();
      }
      if (this.mModel != null)
      {
        this.mMesh = this.mModel.Meshes[0];
        this.mMeshPart = this.mMesh.MeshParts[0];
        if (this.mModel.Meshes[0].VertexBuffer.IsDisposed)
          throw new Exception("Vertexbuffern is disposed!");
      }
      else
      {
        this.mMesh = (ModelMesh) null;
        this.mMeshPart = (ModelMeshPart) null;
      }
    }
  }

  public float Mass
  {
    get => this.mBody.Mass;
    set => this.mBody.Mass = value;
  }

  public float TTL
  {
    get => this.mTTL;
    set => this.mTTL = value;
  }

  public override bool Removable => (double) this.mTTL <= 0.0;

  public override Vector3 CalcImpulseVelocity(
    Vector3 iDirection,
    float iElevation,
    float iMassPower,
    float iDistance)
  {
    return !this.mBody.IsBodyEnabled ? new Vector3() : base.CalcImpulseVelocity(iDirection, iElevation, iMassPower, iDistance);
  }

  public override bool Dead => (double) this.mTTL < 0.0;

  private bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
  {
    if (iSkin1.Owner != null && (iSkin1.Owner.Tag is Character || iSkin1.Owner.Tag is Gib))
      return false;
    if (!this.mFrozen & this.mBloodType != BloodType.none && iSkin1.Tag is LevelModel && (double) this.mBody.Velocity.Y < -10.0 && !EffectManager.Instance.IsActive(ref this.mBloodEffect))
    {
      Vector3 position = this.mBody.Position;
      Vector3 backward = this.mBody.Orientation.Backward;
      EffectManager.Instance.StartEffect(Gib.GORE_GIB_SMALL_EFFECTS[(int) this.mBloodType], ref position, ref backward, out this.mBloodEffect);
    }
    return true;
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mTTL -= iDeltaTime;
    if ((double) this.mTTL < 1.0)
    {
      this.mBody.AllowFreezing = true;
      this.mBody.SetInactive();
      Vector3 position = this.Position;
      position.Y -= (float) ((double) iDeltaTime * (double) this.mRadius * 2.0);
      this.mBody.Position = position;
      if ((double) this.mTTL <= 0.0 && this.mBody.IsBodyEnabled && !this.mBody.IsActive)
        this.mBody.DisableBody();
    }
    base.Update(iDataChannel, iDeltaTime);
    if (this.mModel == null)
      return;
    Matrix orientation = this.mBody.Orientation with
    {
      Translation = this.mBody.Position
    };
    if (!this.mFrozen)
      EffectManager.Instance.UpdateOrientation(ref this.mTrailEffect, ref orientation);
    Gib.RenderData iObject = this.mRenderData[(int) iDataChannel];
    if (iObject.MeshDirty)
      iObject.SetMesh(this.mMesh.VertexBuffer, this.mMesh.IndexBuffer, this.mMeshPart, RenderDeferredEffect.TYPEHASH);
    iObject.mTransform = orientation;
    MagickaMath.UniformMatrixScale(ref iObject.mTransform, this.mScale);
    iObject.mBoundingSphere = this.mMesh.BoundingSphere;
    Vector3.Transform(ref iObject.mBoundingSphere.Center, ref orientation, out iObject.mBoundingSphere.Center);
    this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) iObject);
  }

  public override void Deinitialize()
  {
    Gib.ReturnGib(this);
    base.Deinitialize();
  }

  public override void Kill() => this.mTTL = 0.0f;

  protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
  {
    oMsg = new EntityUpdateMessage();
  }

  protected class RenderData : IRenderableObject
  {
    private int mEffect;
    private VertexDeclaration mVertexDeclaration;
    private int mBaseVertex;
    private int mNumVertices;
    private int mPrimitiveCount;
    private int mStartIndex;
    private int mStreamOffset;
    private int mVertexStride;
    private VertexBuffer mVertexBuffer;
    private IndexBuffer mIndexBuffer;
    public int mVerticesHash;
    private bool mMeshDirty;
    private RenderDeferredMaterial mMaterial;
    public BoundingSphere mBoundingSphere;
    public Matrix mTransform;
    public bool mFrozen;
    public float mBloat;
    private static readonly Vector3 ICE_COLOR = new Vector3(0.5f, 1f, 1.5f);
    private static Texture2D sIceGibTexture;

    public RenderData()
    {
      if (Gib.RenderData.sIceGibTexture != null)
        return;
      Gib.RenderData.sIceGibTexture = Magicka.Game.Instance.Content.Load<Texture2D>("EffectTextures/iceGib");
    }

    public int Effect => this.mEffect;

    public int DepthTechnique => 4;

    public int Technique => 0;

    public int ShadowTechnique => 5;

    public VertexBuffer Vertices => this.mVertexBuffer;

    public IndexBuffer Indices => this.mIndexBuffer;

    public VertexDeclaration VertexDeclaration => this.mVertexDeclaration;

    public int VertexStride => this.mVertexStride;

    public int VerticesHashCode => this.mVerticesHash;

    public bool MeshDirty => this.mMeshDirty;

    public bool Cull(BoundingFrustum iViewFrustum)
    {
      return this.mBoundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
    }

    public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      RenderDeferredEffect iEffect1 = iEffect as RenderDeferredEffect;
      this.mMaterial.AssignToEffect(iEffect1);
      iEffect1.GraphicsDevice.RenderState.DepthBias = -0.0001f;
      iEffect1.Bloat = this.mBloat;
      if (this.mFrozen)
      {
        iEffect1.DiffuseTexture0 = Gib.RenderData.sIceGibTexture;
        iEffect1.EmissiveAmount0 += 0.1f;
      }
      iEffect1.World = this.mTransform;
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
      iEffect1.Bloat = 0.0f;
      iEffect1.GraphicsDevice.RenderState.DepthBias = 0.0f;
    }

    public void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      RenderDeferredEffect iEffect1 = iEffect as RenderDeferredEffect;
      this.mMaterial.AssignOpacityToEffect(iEffect1);
      iEffect1.Bloat = this.mBloat;
      iEffect1.World = this.mTransform;
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
      iEffect1.Bloat = 0.0f;
    }

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
      if (this.mVertexBuffer.IsDisposed)
        throw new Exception("What the hell are we gonna do?");
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
