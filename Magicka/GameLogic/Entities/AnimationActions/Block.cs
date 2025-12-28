using System;
using Magicka.GameLogic.Entities.Abilities;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

namespace Magicka.GameLogic.Entities.AnimationActions
{
	// Token: 0x02000314 RID: 788
	internal class Block : AnimationAction
	{
		// Token: 0x0600183D RID: 6205 RVA: 0x000A0818 File Offset: 0x0009EA18
		public Block(ContentReader iInput, SkinnedModelBoneCollection iSkeleton) : base(iInput, iSkeleton)
		{
			this.mWeapon = iInput.ReadInt32();
		}

		// Token: 0x0600183E RID: 6206 RVA: 0x000A0830 File Offset: 0x0009EA30
		protected override void InternalExecute(Character iOwner, bool iFirstExecution)
		{
			iOwner.BlockItem = this.mWeapon;
			NonPlayerCharacter nonPlayerCharacter = iOwner as NonPlayerCharacter;
			if (nonPlayerCharacter != null && nonPlayerCharacter.AI.BusyAbility is Block)
			{
				nonPlayerCharacter.AI.BusyAbility = null;
			}
		}

		// Token: 0x0600183F RID: 6207 RVA: 0x000A0871 File Offset: 0x0009EA71
		public override void Kill(Character iOwner)
		{
			base.Kill(iOwner);
			iOwner.BlockItem = -1;
		}

		// Token: 0x17000618 RID: 1560
		// (get) Token: 0x06001840 RID: 6208 RVA: 0x000A0881 File Offset: 0x0009EA81
		public override bool UsesBones
		{
			get
			{
				return false;
			}
		}

		// Token: 0x040019FD RID: 6653
		private int mWeapon;
	}
}
