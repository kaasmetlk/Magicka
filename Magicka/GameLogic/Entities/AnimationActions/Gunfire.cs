using System;
using Magicka.GameLogic.Entities.Items;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

namespace Magicka.GameLogic.Entities.AnimationActions
{
	// Token: 0x02000361 RID: 865
	internal class Gunfire : AnimationAction
	{
		// Token: 0x06001A61 RID: 6753 RVA: 0x000B3897 File Offset: 0x000B1A97
		public Gunfire(ContentReader iInput, SkinnedModelBoneCollection iSkeleton) : base(iInput, iSkeleton)
		{
			this.mWeapon = iInput.ReadInt32();
			this.mAccuracy = iInput.ReadSingle();
		}

		// Token: 0x06001A62 RID: 6754 RVA: 0x000B38BC File Offset: 0x000B1ABC
		protected override void InternalExecute(Character iOwner, bool iFirstExecution)
		{
			Item item = iOwner.Equipment[this.mWeapon].Item;
			if (iFirstExecution)
			{
				item.ClearHitlist();
				item.ExecuteGun(this.mAccuracy);
			}
		}

		// Token: 0x1700069A RID: 1690
		// (get) Token: 0x06001A63 RID: 6755 RVA: 0x000B38F1 File Offset: 0x000B1AF1
		public override bool UsesBones
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06001A64 RID: 6756 RVA: 0x000B38F4 File Offset: 0x000B1AF4
		public override void Kill(Character iOwner)
		{
			base.Kill(iOwner);
			Item item = iOwner.Equipment[this.mWeapon].Item;
			item.StopGunfire();
		}

		// Token: 0x04001CC4 RID: 7364
		private int mWeapon;

		// Token: 0x04001CC5 RID: 7365
		private float mAccuracy;
	}
}
