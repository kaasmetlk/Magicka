using System;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.Entities.Items;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

namespace Magicka.GameLogic.Entities.AnimationActions
{
	// Token: 0x02000175 RID: 373
	public class SpecialAbility : AnimationAction
	{
		// Token: 0x06000B62 RID: 2914 RVA: 0x00044197 File Offset: 0x00042397
		public SpecialAbility(ContentReader iInput, SkinnedModelBoneCollection iSkeleton) : base(iInput, iSkeleton)
		{
			this.mWeapon = iInput.ReadInt32();
			if (this.mWeapon < 0)
			{
				this.mAbility = SpecialAbility.Read(iInput);
			}
		}

		// Token: 0x06000B63 RID: 2915 RVA: 0x000441C4 File Offset: 0x000423C4
		protected override void InternalExecute(Character iOwner, bool iFirstExecution)
		{
			if (iFirstExecution)
			{
				if (this.mWeapon < 0)
				{
					this.mAbility.Execute(iOwner, iOwner.PlayState);
					return;
				}
				Item item = iOwner.Equipment[this.mWeapon].Item;
				item.ExecuteSpecialAbility();
			}
		}

		// Token: 0x170002B7 RID: 695
		// (get) Token: 0x06000B64 RID: 2916 RVA: 0x0004420A File Offset: 0x0004240A
		public override bool UsesBones
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06000B65 RID: 2917 RVA: 0x0004420D File Offset: 0x0004240D
		public override void Kill(Character iOwner)
		{
			base.Kill(iOwner);
		}

		// Token: 0x04000A57 RID: 2647
		private int mWeapon;

		// Token: 0x04000A58 RID: 2648
		private SpecialAbility mAbility;
	}
}
