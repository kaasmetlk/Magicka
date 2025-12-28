using System;
using Magicka.GameLogic.GameStates;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

namespace Magicka.GameLogic.Entities.Items
{
	// Token: 0x02000649 RID: 1609
	public class Attachment
	{
		// Token: 0x060030FD RID: 12541 RVA: 0x001933E3 File Offset: 0x001915E3
		public Attachment(PlayState iPlayState, Character iOwner)
		{
			this.mAttachIndex = -1;
			this.mBindBose = default(Matrix);
			this.mItem = new Item(iPlayState, iOwner);
		}

		// Token: 0x060030FE RID: 12542 RVA: 0x0019340C File Offset: 0x0019160C
		public Attachment(ContentReader iInput, SkinnedModelBoneCollection iSkeleton)
		{
			string text = iInput.ReadString();
			SkinnedModelBone skinnedModelBone = null;
			for (int i = 0; i < iSkeleton.Count; i++)
			{
				if (iSkeleton[i].Name.Equals(text, StringComparison.OrdinalIgnoreCase))
				{
					skinnedModelBone = iSkeleton[i];
					this.mAttachIndex = i;
					break;
				}
			}
			if (skinnedModelBone == null)
			{
				throw new Exception("Invalid attach point: " + text);
			}
			this.mBindBose = skinnedModelBone.InverseBindPoseTransform;
			Matrix.Invert(ref this.mBindBose, out this.mBindBose);
			Vector3 vector = iInput.ReadVector3();
			Matrix matrix;
			Matrix.CreateRotationX(vector.X, out matrix);
			Matrix matrix2;
			Matrix.CreateRotationY(vector.Y + 3.1415927f, out matrix2);
			Matrix matrix3;
			Matrix.CreateRotationX(vector.Z, out matrix3);
			Matrix matrix4;
			Matrix.Multiply(ref matrix, ref matrix2, out matrix4);
			Matrix.Multiply(ref matrix4, ref matrix3, out matrix4);
			Matrix.Multiply(ref matrix4, ref this.mBindBose, out this.mBindBose);
			this.mItem = iInput.ReadExternalReference<Item>();
		}

		// Token: 0x060030FF RID: 12543 RVA: 0x001934FC File Offset: 0x001916FC
		public void TransformBindPose(ref Matrix iTransform)
		{
			Matrix.Multiply(ref iTransform, ref this.mBindBose, out this.mBindBose);
		}

		// Token: 0x17000B80 RID: 2944
		// (get) Token: 0x06003100 RID: 12544 RVA: 0x00193510 File Offset: 0x00191710
		// (set) Token: 0x06003101 RID: 12545 RVA: 0x00193518 File Offset: 0x00191718
		public int AttachIndex
		{
			get
			{
				return this.mAttachIndex;
			}
			set
			{
				this.mAttachIndex = value;
			}
		}

		// Token: 0x17000B81 RID: 2945
		// (get) Token: 0x06003102 RID: 12546 RVA: 0x00193521 File Offset: 0x00191721
		// (set) Token: 0x06003103 RID: 12547 RVA: 0x00193529 File Offset: 0x00191729
		public Matrix BindBose
		{
			get
			{
				return this.mBindBose;
			}
			set
			{
				this.mBindBose = value;
			}
		}

		// Token: 0x17000B82 RID: 2946
		// (get) Token: 0x06003104 RID: 12548 RVA: 0x00193532 File Offset: 0x00191732
		// (set) Token: 0x06003105 RID: 12549 RVA: 0x0019353A File Offset: 0x0019173A
		public Item Item
		{
			get
			{
				return this.mItem;
			}
			set
			{
				this.mItem = value;
			}
		}

		// Token: 0x06003106 RID: 12550 RVA: 0x00193543 File Offset: 0x00191743
		public void CopyToInstance(Attachment iTarget)
		{
			iTarget.mAttachIndex = this.mAttachIndex;
			iTarget.mBindBose = this.mBindBose;
			this.mItem.Copy(iTarget.mItem);
		}

		// Token: 0x06003107 RID: 12551 RVA: 0x00193570 File Offset: 0x00191770
		internal void Set(Item iItem, SkinnedModelBone iBone, Vector3? iRotation)
		{
			this.mAttachIndex = (int)iBone.Index;
			this.mBindBose = iBone.InverseBindPoseTransform;
			Matrix.Invert(ref this.mBindBose, out this.mBindBose);
			Matrix matrix4;
			if (iRotation != null)
			{
				Matrix matrix;
				Matrix.CreateRotationX(iRotation.Value.X, out matrix);
				Matrix matrix2;
				Matrix.CreateRotationY(iRotation.Value.Y + 3.1415927f, out matrix2);
				Matrix matrix3;
				Matrix.CreateRotationX(iRotation.Value.Z, out matrix3);
				Matrix.Multiply(ref matrix, ref matrix2, out matrix4);
				Matrix.Multiply(ref matrix4, ref matrix3, out matrix4);
			}
			else
			{
				Matrix.CreateRotationY(3.1415927f, out matrix4);
			}
			Matrix.Multiply(ref matrix4, ref this.mBindBose, out this.mBindBose);
			this.mItem.Deinitialize();
			iItem.Copy(this.mItem);
		}

		// Token: 0x06003108 RID: 12552 RVA: 0x0019363C File Offset: 0x0019183C
		public void Release(PlayState iPlayState)
		{
			if (this.mItem.Attached && this.mItem.Model != null)
			{
				Item item = null;
				Vector3 vector;
				Quaternion quaternion;
				Vector3 translation;
				this.Item.Transform.Decompose(out vector, out quaternion, out translation);
				Matrix transform;
				Matrix.CreateFromQuaternion(ref quaternion, out transform);
				transform.Translation = translation;
				this.mItem.Transform = transform;
				if (this.mItem.IsPickable)
				{
					if (NetworkManager.Instance.State != NetworkState.Client)
					{
						item = Item.GetPickableIntstance();
						this.mItem.Copy(item);
						item.Transform = this.mItem.Transform;
					}
					this.mItem.Detach();
				}
				else
				{
					item = this.mItem;
				}
				if (item != null)
				{
					item.Detach();
					if (!item.Dead)
					{
						iPlayState.EntityManager.AddEntity(item);
						item.Body.EnableBody();
						item.Body.SetActive();
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
							triggerActionMessage.ActionType = TriggerActionType.SpawnItem;
							triggerActionMessage.Handle = item.Handle;
							triggerActionMessage.Template = item.Type;
							triggerActionMessage.Position = item.Position;
							triggerActionMessage.Direction = item.Body.Velocity;
							triggerActionMessage.Bool0 = true;
							triggerActionMessage.Point0 = 0;
							Matrix orientation = item.Body.Orientation;
							Quaternion.CreateFromRotationMatrix(ref orientation, out triggerActionMessage.Orientation);
							NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
						}
					}
				}
			}
		}

		// Token: 0x06003109 RID: 12553 RVA: 0x001937C3 File Offset: 0x001919C3
		public void ReAttach(Character iOwner)
		{
			this.mItem.Deinitialize();
		}

		// Token: 0x0400352A RID: 13610
		private int mAttachIndex;

		// Token: 0x0400352B RID: 13611
		private Matrix mBindBose;

		// Token: 0x0400352C RID: 13612
		private Item mItem;
	}
}
