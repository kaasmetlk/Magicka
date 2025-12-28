using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

namespace Magicka.GameLogic.Entities.AnimationActions
{
	// Token: 0x020001F8 RID: 504
	internal class DetachItem : AnimationAction
	{
		// Token: 0x0600108F RID: 4239 RVA: 0x00068301 File Offset: 0x00066501
		public DetachItem(ContentReader iInput, SkinnedModelBoneCollection iSkeleton) : base(iInput, iSkeleton)
		{
			this.mItem = iInput.ReadInt32();
			this.mVelocity = iInput.ReadVector3();
		}

		// Token: 0x06001090 RID: 4240 RVA: 0x00068324 File Offset: 0x00066524
		protected override void InternalExecute(Character iOwner, bool iFirstExecution)
		{
			if (iFirstExecution && iOwner.Equipment[this.mItem].Item != null)
			{
				Matrix orientation = iOwner.Body.Orientation;
				Vector3 velocity;
				Vector3.Transform(ref this.mVelocity, ref orientation, out velocity);
				iOwner.Equipment[this.mItem].Release(iOwner.PlayState);
				iOwner.Equipment[this.mItem].Item.AnimationDetach();
				iOwner.Equipment[this.mItem].Item.Body.Velocity = velocity;
			}
		}

		// Token: 0x17000430 RID: 1072
		// (get) Token: 0x06001091 RID: 4241 RVA: 0x000683AF File Offset: 0x000665AF
		public override bool UsesBones
		{
			get
			{
				return false;
			}
		}

		// Token: 0x04000F31 RID: 3889
		private int mItem;

		// Token: 0x04000F32 RID: 3890
		private Vector3 mVelocity;
	}
}
