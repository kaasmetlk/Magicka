using System;
using System.IO;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.Entities.Items
{
	// Token: 0x02000343 RID: 835
	public abstract class Pickable : Entity
	{
		// Token: 0x0600197E RID: 6526 RVA: 0x000ABF48 File Offset: 0x000AA148
		public Pickable(PlayState iPlayState) : base(iPlayState)
		{
			if (iPlayState != null)
			{
				this.mRenderData = new Pickable.RenderData[3];
				this.mHighlightRenderData = new Pickable.HighlightRenderData[3];
				for (int i = 0; i < 3; i++)
				{
					this.mRenderData[i] = new Pickable.RenderData();
					this.mHighlightRenderData[i] = new Pickable.HighlightRenderData();
				}
				this.mBody = new Body();
				this.mCollision = new CollisionSkin(this.mBody);
				this.mCollision.AddPrimitive(new Box(default(Vector3), Matrix.Identity, new Vector3(1f)), 1, new MaterialProperties(0f, 1f, 1f));
				this.mCollision.callbackFn += this.OnCollision;
				this.mBody.CollisionSkin = this.mCollision;
				this.mBody.Immovable = false;
				this.mBody.Tag = this;
				this.mPreviousOwner = null;
			}
		}

		// Token: 0x1700064F RID: 1615
		// (get) Token: 0x0600197F RID: 6527 RVA: 0x000AC048 File Offset: 0x000AA248
		// (set) Token: 0x06001980 RID: 6528 RVA: 0x000AC050 File Offset: 0x000AA250
		public Model Model
		{
			get
			{
				return this.mModel;
			}
			set
			{
				this.mModel = value;
				if (this.mRenderData != null)
				{
					for (int i = 0; i < 3; i++)
					{
						this.mRenderData[i].SetMeshDirty();
						this.mHighlightRenderData[i].SetMeshDirty();
					}
				}
				if (this.mModel != null)
				{
					this.mMesh = this.mModel.Meshes[0];
					this.mMeshPart = this.mMesh.MeshParts[0];
					return;
				}
				this.mMesh = null;
				this.mMeshPart = null;
			}
		}

		// Token: 0x06001981 RID: 6529 RVA: 0x000AC0D7 File Offset: 0x000AA2D7
		private bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
		{
			return iSkin1.Owner == null || !(iSkin1.Owner.Tag is Character);
		}

		// Token: 0x06001982 RID: 6530 RVA: 0x000AC0F6 File Offset: 0x000AA2F6
		public override void Deinitialize()
		{
			if (this.mBody != null)
			{
				this.mBody.DisableBody();
			}
		}

		// Token: 0x06001983 RID: 6531 RVA: 0x000AC10B File Offset: 0x000AA30B
		public void Highlight()
		{
			this.mHighlighted = true;
		}

		// Token: 0x06001984 RID: 6532 RVA: 0x000AC114 File Offset: 0x000AA314
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			base.Update(iDataChannel, iDeltaTime);
			if (this.mModel != null)
			{
				Matrix orientation = this.mBody.Orientation;
				orientation.Translation = this.mBody.Position;
				Pickable.RenderData renderData = this.mRenderData[(int)iDataChannel];
				if (renderData.MeshDirty)
				{
					renderData.SetMesh(this.mMesh.VertexBuffer, this.mMesh.IndexBuffer, this.mMeshPart, RenderDeferredEffect.TYPEHASH);
				}
				renderData.mTransform = orientation;
				renderData.mBoundingSphere = this.mMesh.BoundingSphere;
				Vector3.Transform(ref renderData.mBoundingSphere.Center, ref orientation, out renderData.mBoundingSphere.Center);
				if ((this.mVisible & !this.mIsInvisible) && !this.HideModel)
				{
					this.mPlayState.Scene.AddRenderableObject(iDataChannel, renderData);
				}
				if (this.mHighlighted)
				{
					Pickable.HighlightRenderData highlightRenderData = this.mHighlightRenderData[(int)iDataChannel];
					if (highlightRenderData.MeshDirty)
					{
						highlightRenderData.SetMesh(this.mMesh.VertexBuffer, this.mMesh.IndexBuffer, this.mMeshPart, RenderDeferredEffect.TYPEHASH);
					}
					highlightRenderData.mBoundingSphere = this.mRenderData[(int)iDataChannel].mBoundingSphere;
					highlightRenderData.mTransform = orientation;
					this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, highlightRenderData);
				}
			}
			this.mHighlighted = false;
		}

		// Token: 0x17000650 RID: 1616
		// (get) Token: 0x06001985 RID: 6533 RVA: 0x000AC25E File Offset: 0x000AA45E
		protected virtual bool HideModel
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000651 RID: 1617
		// (get) Token: 0x06001986 RID: 6534 RVA: 0x000AC261 File Offset: 0x000AA461
		public int DisplayName
		{
			get
			{
				return this.mDisplayName;
			}
		}

		// Token: 0x17000652 RID: 1618
		// (get) Token: 0x06001987 RID: 6535 RVA: 0x000AC269 File Offset: 0x000AA469
		public int Description
		{
			get
			{
				return this.mDescription;
			}
		}

		// Token: 0x17000653 RID: 1619
		// (get) Token: 0x06001988 RID: 6536 RVA: 0x000AC271 File Offset: 0x000AA471
		// (set) Token: 0x06001989 RID: 6537 RVA: 0x000AC279 File Offset: 0x000AA479
		public bool IsInvisible
		{
			get
			{
				return this.mIsInvisible;
			}
			set
			{
				this.mIsInvisible = value;
			}
		}

		// Token: 0x17000654 RID: 1620
		// (get) Token: 0x0600198A RID: 6538 RVA: 0x000AC282 File Offset: 0x000AA482
		// (set) Token: 0x0600198B RID: 6539 RVA: 0x000AC28A File Offset: 0x000AA48A
		public bool Visible
		{
			get
			{
				return this.mVisible;
			}
			set
			{
				this.mVisible = value;
			}
		}

		// Token: 0x17000655 RID: 1621
		// (get) Token: 0x0600198C RID: 6540 RVA: 0x000AC293 File Offset: 0x000AA493
		public bool IsPickable
		{
			get
			{
				return this.mPickable;
			}
		}

		// Token: 0x17000656 RID: 1622
		// (get) Token: 0x0600198D RID: 6541 RVA: 0x000AC29B File Offset: 0x000AA49B
		// (set) Token: 0x0600198E RID: 6542 RVA: 0x000AC2A3 File Offset: 0x000AA4A3
		public int OnPickup
		{
			get
			{
				return this.mOnPickupTrigger;
			}
			set
			{
				this.mOnPickupTrigger = value;
			}
		}

		// Token: 0x17000657 RID: 1623
		// (get) Token: 0x0600198F RID: 6543 RVA: 0x000AC2AC File Offset: 0x000AA4AC
		// (set) Token: 0x06001990 RID: 6544 RVA: 0x000AC2E8 File Offset: 0x000AA4E8
		public Matrix Transform
		{
			get
			{
				if (this.mBody == null)
				{
					return Matrix.Identity;
				}
				Matrix orientation = this.mBody.Orientation;
				orientation.Translation = this.mBody.Position;
				return orientation;
			}
			set
			{
				Matrix orientation = value;
				orientation.Translation = default(Vector3);
				this.mBody.Orientation = orientation;
				this.mBody.Velocity = value.Translation - this.mBody.Position;
				this.mBody.Position = value.Translation;
			}
		}

		// Token: 0x17000658 RID: 1624
		// (get) Token: 0x06001991 RID: 6545 RVA: 0x000AC347 File Offset: 0x000AA547
		// (set) Token: 0x06001992 RID: 6546 RVA: 0x000AC34F File Offset: 0x000AA54F
		public Entity PreviousOwner
		{
			get
			{
				return this.mPreviousOwner;
			}
			set
			{
				this.mPreviousOwner = value;
			}
		}

		// Token: 0x17000659 RID: 1625
		// (get) Token: 0x06001993 RID: 6547
		public abstract bool Permanent { get; }

		// Token: 0x04001BB9 RID: 7097
		protected Pickable.HighlightRenderData[] mHighlightRenderData;

		// Token: 0x04001BBA RID: 7098
		protected Pickable.RenderData[] mRenderData;

		// Token: 0x04001BBB RID: 7099
		protected string mName;

		// Token: 0x04001BBC RID: 7100
		protected int mDisplayName;

		// Token: 0x04001BBD RID: 7101
		protected int mDescription;

		// Token: 0x04001BBE RID: 7102
		protected int mType;

		// Token: 0x04001BBF RID: 7103
		private Model mModel;

		// Token: 0x04001BC0 RID: 7104
		protected ModelMesh mMesh;

		// Token: 0x04001BC1 RID: 7105
		protected ModelMeshPart mMeshPart;

		// Token: 0x04001BC2 RID: 7106
		protected BoundingBox mBoundingBox;

		// Token: 0x04001BC3 RID: 7107
		protected bool mPickable;

		// Token: 0x04001BC4 RID: 7108
		protected int mOnPickupTrigger;

		// Token: 0x04001BC5 RID: 7109
		protected bool mVisible = true;

		// Token: 0x04001BC6 RID: 7110
		protected bool mIsInvisible;

		// Token: 0x04001BC7 RID: 7111
		protected bool mHighlighted;

		// Token: 0x04001BC8 RID: 7112
		protected Entity mPreviousOwner;

		// Token: 0x02000344 RID: 836
		public struct State
		{
			// Token: 0x06001994 RID: 6548 RVA: 0x000AC358 File Offset: 0x000AA558
			public State(BinaryReader iReader)
			{
				this = default(Pickable.State);
				this.mEntity = null;
				this.Type = (Pickable.State.PickableType)iReader.ReadByte();
				if (this.Type != Pickable.State.PickableType.Invalid)
				{
					this.mPosition.X = iReader.ReadSingle();
					this.mPosition.Y = iReader.ReadSingle();
					this.mPosition.Z = iReader.ReadSingle();
					this.mOrientation.X = iReader.ReadSingle();
					this.mOrientation.Y = iReader.ReadSingle();
					this.mOrientation.Z = iReader.ReadSingle();
					this.mOrientation.W = iReader.ReadSingle();
					this.mImmovable = iReader.ReadBoolean();
					if (this.Type == Pickable.State.PickableType.BookOfMagick)
					{
						this.mMagick = (MagickType)iReader.ReadInt32();
					}
					else if (this.Type == Pickable.State.PickableType.Item)
					{
						this.mItemName = iReader.ReadString();
					}
					this.mUniqueID = iReader.ReadInt32();
				}
			}

			// Token: 0x06001995 RID: 6549 RVA: 0x000AC448 File Offset: 0x000AA648
			public State(Pickable iPickable)
			{
				this.mEntity = iPickable;
				this.Type = Pickable.State.PickableType.Invalid;
				this.mItemName = null;
				this.mImmovable = true;
				this.mMagick = MagickType.None;
				BookOfMagick bookOfMagick = iPickable as BookOfMagick;
				Item item = iPickable as Item;
				if (bookOfMagick != null)
				{
					this.Type = Pickable.State.PickableType.BookOfMagick;
					this.mMagick = bookOfMagick.Magick;
				}
				else if (item != null)
				{
					this.Type = Pickable.State.PickableType.Item;
					this.mItemName = item.Name;
				}
				this.mImmovable = iPickable.Body.Immovable;
				Vector3 vector;
				iPickable.Body.TransformMatrix.Decompose(out vector, out this.mOrientation, out this.mPosition);
				this.mUniqueID = iPickable.UniqueID;
			}

			// Token: 0x06001996 RID: 6550 RVA: 0x000AC4F4 File Offset: 0x000AA6F4
			public Pickable Restore(PlayState iPlayState)
			{
				Matrix matrix;
				Matrix.CreateFromQuaternion(ref this.mOrientation, out matrix);
				if (this.Type == Pickable.State.PickableType.Item)
				{
					if (this.mEntity == null)
					{
						this.mEntity = new Item(iPlayState, null);
					}
					Item item = this.mEntity as Item;
					if (item != null)
					{
						Item item2;
						try
						{
							item2 = item.PlayState.Content.Load<Item>("data/items/wizard/" + this.mItemName);
						}
						catch
						{
							item2 = item.PlayState.Content.Load<Item>("data/items/npc/" + this.mItemName);
						}
						item2.Copy(item);
						item.Body.MoveTo(this.mPosition, matrix);
						item.Body.Immovable = this.mImmovable;
					}
				}
				else if (this.Type == Pickable.State.PickableType.BookOfMagick)
				{
					if (this.mEntity == null)
					{
						this.mEntity = new BookOfMagick(iPlayState);
					}
					BookOfMagick bookOfMagick = this.mEntity as BookOfMagick;
					if (bookOfMagick != null)
					{
						bookOfMagick.Initialize(this.mPosition, matrix, this.mMagick, this.mImmovable, default(Vector3), 0f, this.mUniqueID);
					}
				}
				return this.mEntity;
			}

			// Token: 0x06001997 RID: 6551 RVA: 0x000AC624 File Offset: 0x000AA824
			public void Write(BinaryWriter iWriter)
			{
				iWriter.Write((byte)this.Type);
				if (this.Type != Pickable.State.PickableType.Invalid)
				{
					iWriter.Write(this.mPosition.X);
					iWriter.Write(this.mPosition.Y);
					iWriter.Write(this.mPosition.Z);
					iWriter.Write(this.mOrientation.X);
					iWriter.Write(this.mOrientation.Y);
					iWriter.Write(this.mOrientation.Z);
					iWriter.Write(this.mOrientation.W);
					iWriter.Write(this.mImmovable);
					if (this.Type == Pickable.State.PickableType.BookOfMagick)
					{
						iWriter.Write((int)this.mMagick);
					}
					else if (this.Type == Pickable.State.PickableType.Item)
					{
						iWriter.Write(this.mItemName);
					}
					iWriter.Write(this.mUniqueID);
				}
			}

			// Token: 0x04001BC9 RID: 7113
			private Pickable mEntity;

			// Token: 0x04001BCA RID: 7114
			public readonly Pickable.State.PickableType Type;

			// Token: 0x04001BCB RID: 7115
			private Vector3 mPosition;

			// Token: 0x04001BCC RID: 7116
			private Quaternion mOrientation;

			// Token: 0x04001BCD RID: 7117
			private bool mImmovable;

			// Token: 0x04001BCE RID: 7118
			private MagickType mMagick;

			// Token: 0x04001BCF RID: 7119
			private string mItemName;

			// Token: 0x04001BD0 RID: 7120
			private int mUniqueID;

			// Token: 0x02000345 RID: 837
			public enum PickableType : byte
			{
				// Token: 0x04001BD2 RID: 7122
				Invalid,
				// Token: 0x04001BD3 RID: 7123
				BookOfMagick,
				// Token: 0x04001BD4 RID: 7124
				Item
			}
		}

		// Token: 0x02000346 RID: 838
		protected class RenderData : IRenderableObject
		{
			// Token: 0x1700065A RID: 1626
			// (get) Token: 0x06001998 RID: 6552 RVA: 0x000AC703 File Offset: 0x000AA903
			public int Effect
			{
				get
				{
					return this.mEffect;
				}
			}

			// Token: 0x1700065B RID: 1627
			// (get) Token: 0x06001999 RID: 6553 RVA: 0x000AC70B File Offset: 0x000AA90B
			public int DepthTechnique
			{
				get
				{
					return 4;
				}
			}

			// Token: 0x1700065C RID: 1628
			// (get) Token: 0x0600199A RID: 6554 RVA: 0x000AC70E File Offset: 0x000AA90E
			public int Technique
			{
				get
				{
					return 0;
				}
			}

			// Token: 0x1700065D RID: 1629
			// (get) Token: 0x0600199B RID: 6555 RVA: 0x000AC711 File Offset: 0x000AA911
			public int ShadowTechnique
			{
				get
				{
					return 5;
				}
			}

			// Token: 0x1700065E RID: 1630
			// (get) Token: 0x0600199C RID: 6556 RVA: 0x000AC714 File Offset: 0x000AA914
			public VertexBuffer Vertices
			{
				get
				{
					return this.mVertexBuffer;
				}
			}

			// Token: 0x1700065F RID: 1631
			// (get) Token: 0x0600199D RID: 6557 RVA: 0x000AC71C File Offset: 0x000AA91C
			public IndexBuffer Indices
			{
				get
				{
					return this.mIndexBuffer;
				}
			}

			// Token: 0x17000660 RID: 1632
			// (get) Token: 0x0600199E RID: 6558 RVA: 0x000AC724 File Offset: 0x000AA924
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return this.mVertexDeclaration;
				}
			}

			// Token: 0x17000661 RID: 1633
			// (get) Token: 0x0600199F RID: 6559 RVA: 0x000AC72C File Offset: 0x000AA92C
			public int VertexStride
			{
				get
				{
					return this.mVertexStride;
				}
			}

			// Token: 0x17000662 RID: 1634
			// (get) Token: 0x060019A0 RID: 6560 RVA: 0x000AC734 File Offset: 0x000AA934
			public int VerticesHashCode
			{
				get
				{
					return this.mVerticesHash;
				}
			}

			// Token: 0x17000663 RID: 1635
			// (get) Token: 0x060019A1 RID: 6561 RVA: 0x000AC73C File Offset: 0x000AA93C
			public bool MeshDirty
			{
				get
				{
					return this.mMeshDirty;
				}
			}

			// Token: 0x060019A2 RID: 6562 RVA: 0x000AC744 File Offset: 0x000AA944
			public bool Cull(BoundingFrustum iViewFrustum)
			{
				BoundingSphere boundingSphere = this.mBoundingSphere;
				return boundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
			}

			// Token: 0x060019A3 RID: 6563 RVA: 0x000AC764 File Offset: 0x000AA964
			public virtual void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				RenderDeferredEffect renderDeferredEffect = iEffect as RenderDeferredEffect;
				this.mMaterial.AssignToEffect(renderDeferredEffect);
				renderDeferredEffect.World = this.mTransform;
				renderDeferredEffect.CommitChanges();
				renderDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
			}

			// Token: 0x060019A4 RID: 6564 RVA: 0x000AC7BC File Offset: 0x000AA9BC
			public void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				RenderDeferredEffect renderDeferredEffect = iEffect as RenderDeferredEffect;
				this.mMaterial.AssignToEffect(renderDeferredEffect);
				renderDeferredEffect.World = this.mTransform;
				renderDeferredEffect.CommitChanges();
				renderDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
			}

			// Token: 0x060019A5 RID: 6565 RVA: 0x000AC813 File Offset: 0x000AAA13
			public void SetMeshDirty()
			{
				this.mMeshDirty = true;
			}

			// Token: 0x060019A6 RID: 6566 RVA: 0x000AC81C File Offset: 0x000AAA1C
			public void SetMesh(VertexBuffer iVertices, IndexBuffer iIndices, ModelMeshPart iMeshPart, int iEffectHash)
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

			// Token: 0x04001BD5 RID: 7125
			protected int mEffect;

			// Token: 0x04001BD6 RID: 7126
			protected VertexDeclaration mVertexDeclaration;

			// Token: 0x04001BD7 RID: 7127
			protected int mBaseVertex;

			// Token: 0x04001BD8 RID: 7128
			protected int mNumVertices;

			// Token: 0x04001BD9 RID: 7129
			protected int mPrimitiveCount;

			// Token: 0x04001BDA RID: 7130
			protected int mStartIndex;

			// Token: 0x04001BDB RID: 7131
			protected int mStreamOffset;

			// Token: 0x04001BDC RID: 7132
			protected int mVertexStride;

			// Token: 0x04001BDD RID: 7133
			protected VertexBuffer mVertexBuffer;

			// Token: 0x04001BDE RID: 7134
			protected int mVerticesHash;

			// Token: 0x04001BDF RID: 7135
			protected IndexBuffer mIndexBuffer;

			// Token: 0x04001BE0 RID: 7136
			protected bool mMeshDirty;

			// Token: 0x04001BE1 RID: 7137
			protected RenderDeferredMaterial mMaterial;

			// Token: 0x04001BE2 RID: 7138
			public BoundingSphere mBoundingSphere;

			// Token: 0x04001BE3 RID: 7139
			public Matrix mTransform;
		}

		// Token: 0x02000347 RID: 839
		protected class HighlightRenderData : IRenderableAdditiveObject
		{
			// Token: 0x17000664 RID: 1636
			// (get) Token: 0x060019A8 RID: 6568 RVA: 0x000AC8C4 File Offset: 0x000AAAC4
			public int Effect
			{
				get
				{
					return this.mEffect;
				}
			}

			// Token: 0x17000665 RID: 1637
			// (get) Token: 0x060019A9 RID: 6569 RVA: 0x000AC8CC File Offset: 0x000AAACC
			public int Technique
			{
				get
				{
					return 6;
				}
			}

			// Token: 0x17000666 RID: 1638
			// (get) Token: 0x060019AA RID: 6570 RVA: 0x000AC8CF File Offset: 0x000AAACF
			public VertexBuffer Vertices
			{
				get
				{
					return this.mVertexBuffer;
				}
			}

			// Token: 0x17000667 RID: 1639
			// (get) Token: 0x060019AB RID: 6571 RVA: 0x000AC8D7 File Offset: 0x000AAAD7
			public int VerticesHashCode
			{
				get
				{
					return this.mVerticesHash;
				}
			}

			// Token: 0x17000668 RID: 1640
			// (get) Token: 0x060019AC RID: 6572 RVA: 0x000AC8DF File Offset: 0x000AAADF
			public int VertexStride
			{
				get
				{
					return this.mVertexStride;
				}
			}

			// Token: 0x17000669 RID: 1641
			// (get) Token: 0x060019AD RID: 6573 RVA: 0x000AC8E7 File Offset: 0x000AAAE7
			public IndexBuffer Indices
			{
				get
				{
					return this.mIndexBuffer;
				}
			}

			// Token: 0x1700066A RID: 1642
			// (get) Token: 0x060019AE RID: 6574 RVA: 0x000AC8EF File Offset: 0x000AAAEF
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return this.mVertexDeclaration;
				}
			}

			// Token: 0x060019AF RID: 6575 RVA: 0x000AC8F8 File Offset: 0x000AAAF8
			public bool Cull(BoundingFrustum iViewFrustum)
			{
				BoundingSphere boundingSphere = this.mBoundingSphere;
				return boundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
			}

			// Token: 0x060019B0 RID: 6576 RVA: 0x000AC918 File Offset: 0x000AAB18
			public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				RenderDeferredEffect renderDeferredEffect = iEffect as RenderDeferredEffect;
				this.mMaterial.AssignToEffect(renderDeferredEffect);
				renderDeferredEffect.DiffuseColor0 = new Vector3(1f);
				renderDeferredEffect.FresnelPower = 2f;
				renderDeferredEffect.World = this.mTransform;
				renderDeferredEffect.CommitChanges();
				renderDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
				renderDeferredEffect.DiffuseColor0 = Vector3.One;
			}

			// Token: 0x1700066B RID: 1643
			// (get) Token: 0x060019B1 RID: 6577 RVA: 0x000AC995 File Offset: 0x000AAB95
			public bool MeshDirty
			{
				get
				{
					return this.mMeshDirty;
				}
			}

			// Token: 0x060019B2 RID: 6578 RVA: 0x000AC99D File Offset: 0x000AAB9D
			public void SetMeshDirty()
			{
				this.mMeshDirty = true;
			}

			// Token: 0x060019B3 RID: 6579 RVA: 0x000AC9A8 File Offset: 0x000AABA8
			public void SetMesh(VertexBuffer iVertices, IndexBuffer iIndices, ModelMeshPart iMeshPart, int iEffectHash)
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

			// Token: 0x04001BE4 RID: 7140
			protected VertexDeclaration mVertexDeclaration;

			// Token: 0x04001BE5 RID: 7141
			protected int mBaseVertex;

			// Token: 0x04001BE6 RID: 7142
			protected int mNumVertices;

			// Token: 0x04001BE7 RID: 7143
			protected int mPrimitiveCount;

			// Token: 0x04001BE8 RID: 7144
			protected int mStartIndex;

			// Token: 0x04001BE9 RID: 7145
			protected int mStreamOffset;

			// Token: 0x04001BEA RID: 7146
			protected int mVertexStride;

			// Token: 0x04001BEB RID: 7147
			protected VertexBuffer mVertexBuffer;

			// Token: 0x04001BEC RID: 7148
			protected int mVerticesHash;

			// Token: 0x04001BED RID: 7149
			protected IndexBuffer mIndexBuffer;

			// Token: 0x04001BEE RID: 7150
			protected int mEffect;

			// Token: 0x04001BEF RID: 7151
			protected RenderDeferredMaterial mMaterial;

			// Token: 0x04001BF0 RID: 7152
			public BoundingSphere mBoundingSphere;

			// Token: 0x04001BF1 RID: 7153
			public Matrix mTransform;

			// Token: 0x04001BF2 RID: 7154
			protected bool mMeshDirty = true;
		}
	}
}
