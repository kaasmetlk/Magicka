using System;
using Magicka.GameLogic.Entities.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

namespace Magicka.GameLogic.Entities.AnimationActions
{
	// Token: 0x02000250 RID: 592
	public class SpawnMissile : AnimationAction
	{
		// Token: 0x06001249 RID: 4681 RVA: 0x00070220 File Offset: 0x0006E420
		public SpawnMissile(ContentReader iInput, SkinnedModelBoneCollection iSkeleton) : base(iInput, iSkeleton)
		{
			this.mVelocity = null;
			this.mWeapon = iInput.ReadInt32();
			Vector3 value = iInput.ReadVector3();
			if (value.LengthSquared() > 1E-06f)
			{
				this.mVelocity = new Vector3?(value);
			}
			this.mItemAligned = iInput.ReadBoolean();
		}

		// Token: 0x0600124A RID: 4682 RVA: 0x0007027C File Offset: 0x0006E47C
		protected override void InternalExecute(Character iOwner, bool iFirstExecution)
		{
			Item item = iOwner.Equipment[this.mWeapon].Item;
			if (iFirstExecution)
			{
				MissileEntity missileEntity = null;
				item.ExecuteRanged(ref missileEntity, this.mVelocity, this.mItemAligned);
			}
		}

		// Token: 0x170004B4 RID: 1204
		// (get) Token: 0x0600124B RID: 4683 RVA: 0x000702B5 File Offset: 0x0006E4B5
		public override bool UsesBones
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600124C RID: 4684 RVA: 0x000702B8 File Offset: 0x0006E4B8
		public override void Kill(Character iOwner)
		{
			base.Kill(iOwner);
		}

		// Token: 0x04001106 RID: 4358
		private int mWeapon;

		// Token: 0x04001107 RID: 4359
		private Vector3? mVelocity;

		// Token: 0x04001108 RID: 4360
		private bool mItemAligned;
	}
}
