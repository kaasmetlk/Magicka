using System;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

namespace Magicka.GameLogic.Entities.AnimationActions
{
	// Token: 0x02000251 RID: 593
	public class BreakFree : AnimationAction
	{
		// Token: 0x0600124D RID: 4685 RVA: 0x000702C1 File Offset: 0x0006E4C1
		public BreakFree(ContentReader iInput, SkinnedModelBoneCollection iSkeleton) : base(iInput, iSkeleton)
		{
			this.mMagnitude = iInput.ReadSingle();
			this.mWeapon = iInput.ReadInt32();
		}

		// Token: 0x0600124E RID: 4686 RVA: 0x000702E3 File Offset: 0x0006E4E3
		protected override void InternalExecute(Character iOwner, bool iFirstExecution)
		{
			if (iFirstExecution)
			{
				if (iOwner.IsEntangled)
				{
					iOwner.DamageEntanglement(this.mMagnitude, Elements.Earth);
					return;
				}
				if (iOwner.IsGripped)
				{
					iOwner.BreakFree();
				}
			}
		}

		// Token: 0x170004B5 RID: 1205
		// (get) Token: 0x0600124F RID: 4687 RVA: 0x0007030C File Offset: 0x0006E50C
		public override bool UsesBones
		{
			get
			{
				return false;
			}
		}

		// Token: 0x04001109 RID: 4361
		private float mMagnitude;

		// Token: 0x0400110A RID: 4362
		private int mWeapon;
	}
}
