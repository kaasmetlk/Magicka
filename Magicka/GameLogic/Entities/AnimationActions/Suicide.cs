using System;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

namespace Magicka.GameLogic.Entities.AnimationActions
{
	// Token: 0x020001F6 RID: 502
	internal class Suicide : AnimationAction
	{
		// Token: 0x06001088 RID: 4232 RVA: 0x0006827E File Offset: 0x0006647E
		public Suicide(ContentReader iInput, SkinnedModelBoneCollection iSkeleton) : base(iInput, iSkeleton)
		{
			this.mOverkill = iInput.ReadBoolean();
		}

		// Token: 0x06001089 RID: 4233 RVA: 0x00068294 File Offset: 0x00066494
		protected override void InternalExecute(Character iOwner, bool iFirstExecution)
		{
			if (this.mOverkill)
			{
				iOwner.OverKill();
				return;
			}
			iOwner.Kill();
			iOwner.Die();
		}

		// Token: 0x1700042E RID: 1070
		// (get) Token: 0x0600108A RID: 4234 RVA: 0x000682B1 File Offset: 0x000664B1
		public override bool UsesBones
		{
			get
			{
				return false;
			}
		}

		// Token: 0x04000F2F RID: 3887
		private bool mOverkill;
	}
}
