// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.SprayEntity
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using JigLibX.Physics;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.GameStates;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities;

public class SprayEntity : Entity
{
  private const float DECAYTIME = 1f;
  private static List<SprayEntity> sCache;
  private static readonly int SplashEffect = "web_hit".GetHashCodeCustom();
  private SprayEntity.RenderData[] mRenderData;
  private Entity mOwner;
  private float mDecayTimer;
  private SprayEntity mChild;
  private float mDragStrength;

  public static SprayEntity GetInstance()
  {
    SprayEntity instance = SprayEntity.sCache[0];
    SprayEntity.sCache.RemoveAt(0);
    SprayEntity.sCache.Add(instance);
    return instance;
  }

  public static void InitializeCache(PlayState iPlayState, int iNr)
  {
    SprayEntity.sCache = new List<SprayEntity>(iNr);
    for (int index = 0; index < iNr; ++index)
      SprayEntity.sCache.Add(new SprayEntity(iPlayState));
  }

  public static SprayEntity GetSpecificInstance(ushort iHandle)
  {
    SprayEntity fromHandle = Entity.GetFromHandle((int) iHandle) as SprayEntity;
    SprayEntity.sCache.Remove(fromHandle);
    SprayEntity.sCache.Add(fromHandle);
    return fromHandle;
  }

  public SprayEntity(PlayState iPlayState)
    : base(iPlayState)
  {
    this.mBody = new Body();
    this.mCollision = new CollisionSkin(this.mBody);
    this.mCollision.AddPrimitive((Primitive) new Capsule(Vector3.Zero, Matrix.Identity, 0.5f, 0.0f), 1, new MaterialProperties(0.333f, 0.8f, 0.8f));
    this.mBody.CollisionSkin = this.mCollision;
    this.mBody.Tag = (object) this;
    this.mBody.Immovable = false;
    this.mCollision.callbackFn += new CollisionCallbackFn(this.OnCollision);
    VertexPositionNormalTexture[] data = new VertexPositionNormalTexture[4];
    data[0].TextureCoordinate = new Vector2(1f, 1f);
    data[0].Position = new Vector3(0.5f, 0.0f, 0.0f);
    data[0].Normal = new Vector3(0.0f, 1f, 0.0f);
    data[1].TextureCoordinate = new Vector2(0.0f, 1f);
    data[1].Position = new Vector3(-0.5f, 0.0f, 0.0f);
    data[1].Normal = new Vector3(0.0f, 1f, 0.0f);
    data[2].TextureCoordinate = new Vector2(0.0f, 0.0f);
    data[2].Position = new Vector3(-0.5f, 0.0f, 1f);
    data[2].Normal = new Vector3(0.0f, 1f, 0.0f);
    data[3].TextureCoordinate = new Vector2(1f, 0.0f);
    data[3].Position = new Vector3(0.5f, 0.0f, 1f);
    data[3].Normal = new Vector3(0.0f, 1f, 0.0f);
    VertexBuffer iVertexBuffer;
    VertexDeclaration iDeclaration;
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      iVertexBuffer = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, data.Length * VertexPositionNormalTexture.SizeInBytes, BufferUsage.WriteOnly);
      iVertexBuffer.SetData<VertexPositionNormalTexture>(data);
      iDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, VertexPositionNormalTexture.VertexElements);
      iVertexBuffer.Name = "SprayEntityBuffer";
    }
    RenderDeferredMaterial deferredMaterial = new RenderDeferredMaterial();
    lock (Magicka.Game.Instance.GraphicsDevice)
      deferredMaterial.DiffuseTexture0 = this.mPlayState.Content.Load<Texture2D>("EffectTextures/Spray_web");
    deferredMaterial.VertexColorEnabled = false;
    this.mRenderData = new SprayEntity.RenderData[3];
    for (int index = 0; index < this.mRenderData.Length; ++index)
    {
      this.mRenderData[index] = new SprayEntity.RenderData(iVertexBuffer, iDeclaration);
      this.mRenderData[index].mMaterial = deferredMaterial;
    }
  }

  protected bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
  {
    if (iSkin1.Owner != null)
    {
      if (iSkin1.Owner.Tag is BossCollisionZone || iSkin1.Owner.Tag is SprayEntity)
        return false;
      if (iSkin1.Owner.Tag is Shield)
      {
        this.mBody.Immovable = true;
        return true;
      }
      if (iSkin1.Owner == null || !(iSkin1.Owner.Tag is Character) || (iSkin1.Owner.Tag as Character).IsEntangled || !(iSkin1.Owner.Tag is Avatar) || this.mBody.Immovable)
        return false;
      (iSkin1.Owner.Tag as Character).Entangle(12f);
      this.mBody.Immovable = true;
      this.mBody.Velocity = new Vector3();
      return false;
    }
    if (!(iSkin1.Tag is LevelModel))
      return true;
    this.mBody.Immovable = true;
    return true;
  }

  public SprayEntity Child
  {
    get => this.mChild;
    set => this.mChild = value == this ? (SprayEntity) null : value;
  }

  public void Initialize(Entity iOwner, Vector3 iPosition, Vector3 iDirection, float iVelocity)
  {
    this.Initialize(iOwner, (SprayEntity) null, iPosition, iDirection, iVelocity);
  }

  public void Initialize(
    Entity iOwner,
    SprayEntity iChild,
    Vector3 iPosition,
    Vector3 iDirection,
    float iVelocity)
  {
    this.Initialize();
    Vector3 zero = Vector3.Zero;
    Vector3 up = Vector3.Up;
    Matrix result;
    Matrix.CreateWorld(ref zero, ref iDirection, ref up, out result);
    Vector3.Add(ref iPosition, ref iDirection, out iPosition);
    this.mBody.MoveTo(iPosition, result);
    Vector3.Multiply(ref iDirection, iVelocity, out iDirection);
    this.mBody.Velocity = iDirection;
    this.mBody.Immovable = false;
    this.mBody.ApplyGravity = false;
    this.mOwner = iOwner;
    this.mChild = iChild;
    this.mDead = false;
    this.mDecayTimer = 0.0f;
    this.mDragStrength = 0.0f;
    (this.mCollision.GetPrimitiveLocal(0) as Capsule).Length = 0.0f;
    (this.mCollision.GetPrimitiveNewWorld(0) as Capsule).Length = 0.0f;
    (this.mCollision.GetPrimitiveOldWorld(0) as Capsule).Length = 0.0f;
    if (iOwner == null)
      return;
    this.mBody.CollisionSkin.NonCollidables.Add(iOwner.Body.CollisionSkin);
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    base.Update(iDataChannel, iDeltaTime);
    this.mDragStrength += iDeltaTime;
    Vector3 velocity = this.mBody.Velocity;
    if ((double) velocity.LengthSquared() > 1.4012984643248171E-45)
    {
      velocity.Y -= iDeltaTime * 10f;
      this.mBody.Velocity = velocity;
    }
    this.mBody.AngularVelocity = Vector3.Zero;
    if (this.mChild != null && this.mChild.Dead)
      this.mChild = this.mChild.Child == null ? (SprayEntity) null : this.mChild.Child;
    if (this.mBody.Immovable)
    {
      this.mDecayTimer += iDeltaTime;
      if ((double) this.mDecayTimer >= 1.0)
        this.mDead = true;
    }
    Transform identity = Transform.Identity;
    if (this.mChild != null || this.mOwner != null)
    {
      Matrix result1 = Matrix.Identity;
      Vector3 position1 = this.Position;
      Vector3 result2 = Vector3.Zero;
      if (this.mChild != null)
        result2 = this.mChild.Position;
      else if (this.mOwner != null)
        result2 = this.mOwner.Position;
      Vector3 result3 = Vector3.Zero;
      Vector3 result4 = Vector3.Up;
      Vector3 zero = Vector3.Zero;
      Vector3 result5 = Vector3.Right;
      Vector3.Multiply(ref result5, 0.01f, out result5);
      Vector3.Add(ref result2, ref result5, out result2);
      Vector3.Subtract(ref position1, ref result2, out result3);
      float zScale = result3.Length();
      result3.Normalize();
      Matrix.CreateWorld(ref zero, ref result3, ref result4, out identity.Orientation);
      (this.mCollision.GetPrimitiveLocal(0) as Capsule).Length = zScale;
      (this.mCollision.GetPrimitiveNewWorld(0) as Capsule).Length = zScale;
      (this.mCollision.GetPrimitiveOldWorld(0) as Capsule).Length = zScale;
      this.mCollision.UpdateWorldBoundingBox();
      Vector3 position2 = this.mPlayState.Camera.Position;
      Vector3 result6;
      Vector3.Subtract(ref position2, ref position1, out result6);
      Vector3 result7;
      Vector3.Cross(ref result3, ref result6, out result7);
      Vector3.Normalize(ref result7, out result7);
      Vector3.Cross(ref result7, ref result3, out result4);
      Vector3.Normalize(ref result4, out result4);
      Matrix result8;
      Matrix.CreateScale(1f, 1f, zScale, out result8);
      result1.Forward = result3;
      result1.Up = result4;
      result1.Right = result7;
      Matrix.Multiply(ref result8, ref result1, out result1);
      result1.Translation = position1;
      this.mRenderData[(int) iDataChannel].mMaterial.Alpha = (float) System.Math.Pow(1.0 - (double) this.mDecayTimer, 0.2);
      this.mRenderData[(int) iDataChannel].mSize = zScale;
      this.mRenderData[(int) iDataChannel].mTransform = result1;
      this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) this.mRenderData[(int) iDataChannel]);
    }
    this.mBody.SetOrientation(identity.Orientation);
  }

  public override void Deinitialize()
  {
    base.Deinitialize();
    this.mBody.CollisionSkin.NonCollidables.Clear();
  }

  public override bool Dead => this.mDead;

  public override bool Removable => this.mDead;

  public override void Kill() => this.mDead = true;

  protected override void INetworkUpdate(ref EntityUpdateMessage iMsg)
  {
    base.INetworkUpdate(ref iMsg);
    this.mBody.Velocity = iMsg.Velocity;
  }

  protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
  {
    oMsg = new EntityUpdateMessage();
    oMsg.Features |= EntityFeatures.Position;
    oMsg.Position = this.Position;
    Vector3 result = this.mBody.Velocity;
    Vector3.Multiply(ref result, iPrediction, out result);
    Vector3.Add(ref result, ref oMsg.Position, out oMsg.Position);
    oMsg.Features |= EntityFeatures.Velocity;
    oMsg.Velocity = this.mBody.Velocity;
  }

  protected class RenderData : IRenderableObject
  {
    private int mVerticesHash;
    private VertexBuffer mVertexBuffer;
    private VertexDeclaration mVertexDeclaration;
    public Matrix mTransform;
    public RenderDeferredMaterial mMaterial;
    public float mScroll;
    public float mSize;

    public RenderData(VertexBuffer iVertexBuffer, VertexDeclaration iDeclaration)
    {
      this.mVerticesHash = iVertexBuffer.GetHashCode();
      this.mVertexBuffer = iVertexBuffer;
      this.mVertexDeclaration = iDeclaration;
    }

    public int Effect => RenderDeferredEffect.TYPEHASH;

    public int Technique => 0;

    public VertexBuffer Vertices => this.mVertexBuffer;

    public int VerticesHashCode => this.mVerticesHash;

    public int VertexStride => VertexPositionNormalTexture.SizeInBytes;

    public IndexBuffer Indices => (IndexBuffer) null;

    public VertexDeclaration VertexDeclaration => this.mVertexDeclaration;

    public bool Cull(BoundingFrustum iViewFrustum) => false;

    public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      RenderDeferredEffect iEffect1 = iEffect as RenderDeferredEffect;
      this.mMaterial.AssignToEffect(iEffect1);
      iEffect1.EmissiveAmount0 = 1f;
      iEffect1.World = this.mTransform;
      iEffect1.GraphicsDevice.RenderState.CullMode = CullMode.None;
      iEffect1.DiffuseColor0 = Vector3.One;
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
      iEffect1.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
    }

    public int DepthTechnique => 4;

    public int ShadowTechnique => 5;

    public void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      RenderDeferredEffect iEffect1 = iEffect as RenderDeferredEffect;
      this.mMaterial.AssignToEffect(iEffect1);
      iEffect1.World = this.mTransform;
      iEffect1.GraphicsDevice.RenderState.CullMode = CullMode.None;
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
      iEffect1.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
    }
  }
}
