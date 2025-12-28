using System;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

namespace Magicka.GameLogic.Entities.AnimationActions
{
	// Token: 0x02000585 RID: 1413
	internal class WeaponVisibility : AnimationAction
	{
		// Token: 0x06002A3F RID: 10815 RVA: 0x0014BF33 File Offset: 0x0014A133
		public WeaponVisibility(ContentReader iInput, SkinnedModelBoneCollection iSkeleton) : base(iInput, iSkeleton)
		{
			this.mWeapon = iInput.ReadInt32();
			this.mVisible = iInput.ReadBoolean();
		}

		// Token: 0x06002A40 RID: 10816 RVA: 0x0014BF55 File Offset: 0x0014A155
		protected override void InternalExecute(Character iOwner, bool iFirstExecution)
		{
			iOwner.Equipment[this.mWeapon].Item.Visible = this.mVisible;
		}

		// Token: 0x170009EF RID: 2543
		// (get) Token: 0x06002A41 RID: 10817 RVA: 0x0014BF74 File Offset: 0x0014A174
		public override bool UsesBones
		{
			get
			{
				return false;
			}
		}

		// Token: 0x04002DA2 RID: 11682
		private int mWeapon;

		// Token: 0x04002DA3 RID: 11683
		private bool mVisible;
	}
}
