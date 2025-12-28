using System;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

namespace Magicka.GameLogic.Entities.AnimationActions
{
	// Token: 0x0200053E RID: 1342
	internal class RemoveStatus : AnimationAction
	{
		// Token: 0x060027F2 RID: 10226 RVA: 0x00123E1B File Offset: 0x0012201B
		public RemoveStatus(ContentReader iInput, SkinnedModelBoneCollection iSkeleton) : base(iInput, iSkeleton)
		{
			this.mStatus = (StatusEffects)Enum.Parse(typeof(StatusEffects), iInput.ReadString(), true);
		}

		// Token: 0x060027F3 RID: 10227 RVA: 0x00123E46 File Offset: 0x00122046
		protected override void InternalExecute(Character iOwner, bool iFirstExecution)
		{
			if (iFirstExecution)
			{
				iOwner.StopStatusEffects(this.mStatus);
			}
		}

		// Token: 0x17000956 RID: 2390
		// (get) Token: 0x060027F4 RID: 10228 RVA: 0x00123E57 File Offset: 0x00122057
		public override bool UsesBones
		{
			get
			{
				return false;
			}
		}

		// Token: 0x04002B83 RID: 11139
		private StatusEffects mStatus;
	}
}
