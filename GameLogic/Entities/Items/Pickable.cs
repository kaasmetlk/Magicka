// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Items.Pickable
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System.IO;

#nullable disable
namespace Magicka.GameLogic.Entities.Items;

public abstract class Pickable : Entity
{
  protected Pickable.HighlightRenderData[] mHighlightRenderData;
  protected Pickable.RenderData[] mRenderData;
  protected string mName;
  protected int mDisplayName;
  protected int mDescription;
  protected int mType;
  private Model mModel;
  protected ModelMesh mMesh;
  protected ModelMeshPart mMeshPart;
  protected BoundingBox mBoundingBox;
  protected bool mPickable;
  protected int mOnPickupTrigger;
  protected bool mVisible = true;
  protected bool mIsInvisible;
  protected bool mHighlighted;
  protected Entity mPreviousOwner;

  public Pickable(PlayState iPlayState)
    : base(iPlayState)
  {
    if (iPlayState == null)
      return;
    this.mRenderData = new Pickable.RenderData[3];
    this.mHighlightRenderData = new Pickable.HighlightRenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      this.mRenderData[index] = new Pickable.RenderData();
      this.mHighlightRenderData[index] = new Pickable.HighlightRenderData();
    }
    this.mBody = new Body();
    this.mCollision = new CollisionSkin(this.mBody);
    this.mCollision.AddPrimitive((Primitive) new Box(new Vector3(), Matrix.Identity, new Vector3(1f)), 1, new MaterialProperties(0.0f, 1f, 1f));
    this.mCollision.callbackFn += new CollisionCallbackFn(this.OnCollision);
    this.mBody.CollisionSkin = this.mCollision;
    this.mBody.Immovable = false;
    this.mBody.Tag = (object) this;
    this.mPreviousOwner = (Entity) null;
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
        {
          this.mRenderData[index].SetMeshDirty();
          this.mHighlightRenderData[index].SetMeshDirty();
        }
      }
      if (this.mModel != null)
      {
        this.mMesh = this.mModel.Meshes[0];
        this.mMeshPart = this.mMesh.MeshParts[0];
      }
      else
      {
        this.mMesh = (ModelMesh) null;
        this.mMeshPart = (ModelMeshPart) null;
      }
    }
  }

  private bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
  {
    return iSkin1.Owner == null || !(iSkin1.Owner.Tag is Magicka.GameLogic.Entities.Character);
  }

  public override void Deinitialize()
  {
    if (this.mBody == null)
      return;
    this.mBody.DisableBody();
  }

  public void Highlight() => this.mHighlighted = true;

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    base.Update(iDataChannel, iDeltaTime);
    if (this.mModel != null)
    {
      Matrix orientation = this.mBody.Orientation with
      {
        Translation = this.mBody.Position
      };
      Pickable.RenderData iObject1 = this.mRenderData[(int) iDataChannel];
      if (iObject1.MeshDirty)
        iObject1.SetMesh(this.mMesh.VertexBuffer, this.mMesh.IndexBuffer, this.mMeshPart, RenderDeferredEffect.TYPEHASH);
      iObject1.mTransform = orientation;
      iObject1.mBoundingSphere = this.mMesh.BoundingSphere;
      Vector3.Transform(ref iObject1.mBoundingSphere.Center, ref orientation, out iObject1.mBoundingSphere.Center);
      if (this.mVisible & !this.mIsInvisible && !this.HideModel)
        this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) iObject1);
      if (this.mHighlighted)
      {
        Pickable.HighlightRenderData iObject2 = this.mHighlightRenderData[(int) iDataChannel];
        if (iObject2.MeshDirty)
          iObject2.SetMesh(this.mMesh.VertexBuffer, this.mMesh.IndexBuffer, this.mMeshPart, RenderDeferredEffect.TYPEHASH);
        iObject2.mBoundingSphere = this.mRenderData[(int) iDataChannel].mBoundingSphere;
        iObject2.mTransform = orientation;
        this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, (IRenderableAdditiveObject) iObject2);
      }
    }
    this.mHighlighted = false;
  }

  protected virtual bool HideModel => false;

  public int DisplayName => this.mDisplayName;

  public int Description => this.mDescription;

  public bool IsInvisible
  {
    get => this.mIsInvisible;
    set => this.mIsInvisible = value;
  }

  public bool Visible
  {
    get => this.mVisible;
    set => this.mVisible = value;
  }

  public bool IsPickable => this.mPickable;

  public int OnPickup
  {
    get => this.mOnPickupTrigger;
    set => this.mOnPickupTrigger = value;
  }

  public Matrix Transform
  {
    get
    {
      if (this.mBody == null)
        return Matrix.Identity;
      return this.mBody.Orientation with
      {
        Translation = this.mBody.Position
      };
    }
    set
    {
      this.mBody.Orientation = value with
      {
        Translation = new Vector3()
      };
      this.mBody.Velocity = value.Translation - this.mBody.Position;
      this.mBody.Position = value.Translation;
    }
  }

  public Entity PreviousOwner
  {
    get => this.mPreviousOwner;
    set => this.mPreviousOwner = value;
  }

  public abstract bool Permanent { get; }

  public struct State
  {
    private Pickable mEntity;
    public readonly Pickable.State.PickableType Type;
    private Vector3 mPosition;
    private Quaternion mOrientation;
    private bool mImmovable;
    private MagickType mMagick;
    private string mItemName;
    private int mUniqueID;

    public State(BinaryReader iReader)
      : this()
    {
      this.mEntity = (Pickable) null;
      this.Type = (Pickable.State.PickableType) iReader.ReadByte();
      if (this.Type == Pickable.State.PickableType.Invalid)
        return;
      this.mPosition.X = iReader.ReadSingle();
      this.mPosition.Y = iReader.ReadSingle();
      this.mPosition.Z = iReader.ReadSingle();
      this.mOrientation.X = iReader.ReadSingle();
      this.mOrientation.Y = iReader.ReadSingle();
      this.mOrientation.Z = iReader.ReadSingle();
      this.mOrientation.W = iReader.ReadSingle();
      this.mImmovable = iReader.ReadBoolean();
      if (this.Type == Pickable.State.PickableType.BookOfMagick)
        this.mMagick = (MagickType) iReader.ReadInt32();
      else if (this.Type == Pickable.State.PickableType.Item)
        this.mItemName = iReader.ReadString();
      this.mUniqueID = iReader.ReadInt32();
    }

    public State(Pickable iPickable)
    {
      this.mEntity = iPickable;
      this.Type = Pickable.State.PickableType.Invalid;
      this.mItemName = (string) null;
      this.mImmovable = true;
      this.mMagick = MagickType.None;
      BookOfMagick bookOfMagick = iPickable as BookOfMagick;
      Item obj = iPickable as Item;
      if (bookOfMagick != null)
      {
        this.Type = Pickable.State.PickableType.BookOfMagick;
        this.mMagick = bookOfMagick.Magick;
      }
      else if (obj != null)
      {
        this.Type = Pickable.State.PickableType.Item;
        this.mItemName = obj.Name;
      }
      this.mImmovable = iPickable.Body.Immovable;
      iPickable.Body.TransformMatrix.Decompose(out Vector3 _, out this.mOrientation, out this.mPosition);
      this.mUniqueID = iPickable.UniqueID;
    }

    public Pickable Restore(PlayState iPlayState)
    {
      Matrix result;
      Matrix.CreateFromQuaternion(ref this.mOrientation, out result);
      if (this.Type == Pickable.State.PickableType.Item)
      {
        if (this.mEntity == null)
          this.mEntity = (Pickable) new Item(iPlayState, (Magicka.GameLogic.Entities.Character) null);
        if (this.mEntity is Item mEntity)
        {
          Item obj;
          try
          {
            obj = mEntity.PlayState.Content.Load<Item>("data/items/wizard/" + this.mItemName);
          }
          catch
          {
            obj = mEntity.PlayState.Content.Load<Item>("data/items/npc/" + this.mItemName);
          }
          obj.Copy(mEntity);
          mEntity.Body.MoveTo(this.mPosition, result);
          mEntity.Body.Immovable = this.mImmovable;
        }
      }
      else if (this.Type == Pickable.State.PickableType.BookOfMagick)
      {
        if (this.mEntity == null)
          this.mEntity = (Pickable) new BookOfMagick(iPlayState);
        if (this.mEntity is BookOfMagick mEntity)
        {
          Vector3 mPosition = this.mPosition;
          Matrix iOrientation = result;
          int mMagick = (int) this.mMagick;
          int num = this.mImmovable ? 1 : 0;
          Vector3 iVelocity = new Vector3();
          int mUniqueId = this.mUniqueID;
          mEntity.Initialize(mPosition, iOrientation, (MagickType) mMagick, num != 0, iVelocity, 0.0f, mUniqueId);
        }
      }
      return this.mEntity;
    }

    public void Write(BinaryWriter iWriter)
    {
      iWriter.Write((byte) this.Type);
      if (this.Type == Pickable.State.PickableType.Invalid)
        return;
      iWriter.Write(this.mPosition.X);
      iWriter.Write(this.mPosition.Y);
      iWriter.Write(this.mPosition.Z);
      iWriter.Write(this.mOrientation.X);
      iWriter.Write(this.mOrientation.Y);
      iWriter.Write(this.mOrientation.Z);
      iWriter.Write(this.mOrientation.W);
      iWriter.Write(this.mImmovable);
      if (this.Type == Pickable.State.PickableType.BookOfMagick)
        iWriter.Write((int) this.mMagick);
      else if (this.Type == Pickable.State.PickableType.Item)
        iWriter.Write(this.mItemName);
      iWriter.Write(this.mUniqueID);
    }

    public enum PickableType : byte
    {
      Invalid,
      BookOfMagick,
      Item,
    }
  }

  protected class RenderData : IRenderableObject
  {
    protected int mEffect;
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
    protected bool mMeshDirty;
    protected RenderDeferredMaterial mMaterial;
    public BoundingSphere mBoundingSphere;
    public Matrix mTransform;

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

    public virtual void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      RenderDeferredEffect iEffect1 = iEffect as RenderDeferredEffect;
      this.mMaterial.AssignToEffect(iEffect1);
      iEffect1.World = this.mTransform;
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
    }

    public void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      RenderDeferredEffect iEffect1 = iEffect as RenderDeferredEffect;
      this.mMaterial.AssignToEffect(iEffect1);
      iEffect1.World = this.mTransform;
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
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
      RenderDeferredEffect iEffect1 = iEffect as RenderDeferredEffect;
      this.mMaterial.AssignToEffect(iEffect1);
      iEffect1.DiffuseColor0 = new Vector3(1f);
      iEffect1.FresnelPower = 2f;
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
