using System;
using Magicka.GameLogic.Entities.Items;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

namespace Magicka.GameLogic.Entities.AnimationActions
{
	// Token: 0x0200057F RID: 1407
	public class DealDamage : AnimationAction
	{
		// Token: 0x06002A1C RID: 10780 RVA: 0x0014B5AE File Offset: 0x001497AE
		public DealDamage(ContentReader iInput, SkinnedModelBoneCollection iSkeleton) : base(iInput, iSkeleton)
		{
			this.mWeapon = iInput.ReadInt32();
			this.mTarget = (DealDamage.Targets)iInput.ReadByte();
		}

		// Token: 0x06002A1D RID: 10781 RVA: 0x0014B5D0 File Offset: 0x001497D0
		protected override void InternalExecute(Character iOwner, bool iFirstExecution)
		{
			Item item = iOwner.Equipment[this.mWeapon].Item;
			if (iFirstExecution)
			{
				item.ClearHitlist();
				EventCondition eventCondition = default(EventCondition);
				eventCondition.EventConditionType = EventConditionType.Default;
				item.MeleeConditions.ExecuteAll(item, null, ref eventCondition);
			}
			item.Execute(this.mTarget);
		}

		// Token: 0x170009E9 RID: 2537
		// (get) Token: 0x06002A1E RID: 10782 RVA: 0x0014B625 File Offset: 0x00149825
		public override bool UsesBones
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06002A1F RID: 10783 RVA: 0x0014B628 File Offset: 0x00149828
		public override void Kill(Character iOwner)
		{
			base.Kill(iOwner);
			Item item = iOwner.Equipment[this.mWeapon].Item;
			item.StopExecute();
		}

		// Token: 0x04002D88 RID: 11656
		private int mWeapon;

		// Token: 0x04002D89 RID: 11657
		private DealDamage.Targets mTarget;

		// Token: 0x02000580 RID: 1408
		[Flags]
		public enum Targets : byte
		{
			// Token: 0x04002D8B RID: 11659
			None = 0,
			// Token: 0x04002D8C RID: 11660
			Target = 0,
			// Token: 0x04002D8D RID: 11661
			Friendly = 1,
			// Token: 0x04002D8E RID: 11662
			Enemy = 2,
			// Token: 0x04002D8F RID: 11663
			NonCharacters = 4,
			// Token: 0x04002D90 RID: 11664
			All = 255
		}
	}
}
