using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

namespace Magicka.GameLogic.Entities.AnimationActions
{
	// Token: 0x0200066E RID: 1646
	internal class SetItemAttach : AnimationAction
	{
		// Token: 0x060031B9 RID: 12729 RVA: 0x00198B00 File Offset: 0x00196D00
		public SetItemAttach(ContentReader iInput, SkinnedModelBoneCollection iSkeleton) : base(iInput, iSkeleton)
		{
			this.mItem = iInput.ReadInt32();
			string value = iInput.ReadString();
			if (!string.IsNullOrEmpty(value))
			{
				foreach (SkinnedModelBone skinnedModelBone in iSkeleton)
				{
					if (skinnedModelBone.Name.Equals(value, StringComparison.OrdinalIgnoreCase))
					{
						this.mJoint.mIndex = (int)skinnedModelBone.Index;
						this.mJoint.mBindPose = skinnedModelBone.InverseBindPoseTransform;
						Matrix.Invert(ref this.mJoint.mBindPose, out this.mJoint.mBindPose);
						Matrix.Multiply(ref SetItemAttach.ROTATE_Y, ref this.mJoint.mBindPose, out this.mJoint.mBindPose);
						break;
					}
				}
			}
		}

		// Token: 0x060031BA RID: 12730 RVA: 0x00198BD8 File Offset: 0x00196DD8
		protected override void InternalExecute(Character iOwner, bool iFirstExecution)
		{
			if (iFirstExecution)
			{
				iOwner.Equipment[this.mItem].AttachIndex = this.mJoint.mIndex;
				iOwner.Equipment[this.mItem].BindBose = this.mJoint.mBindPose;
			}
		}

		// Token: 0x17000BA0 RID: 2976
		// (get) Token: 0x060031BB RID: 12731 RVA: 0x00198C17 File Offset: 0x00196E17
		public override bool UsesBones
		{
			get
			{
				return true;
			}
		}

		// Token: 0x04003625 RID: 13861
		private int mItem;

		// Token: 0x04003626 RID: 13862
		private BindJoint mJoint;

		// Token: 0x04003627 RID: 13863
		private static Matrix ROTATE_Y = Matrix.CreateRotationY(3.1415927f);
	}
}
