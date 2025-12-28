using System;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

namespace Magicka.GameLogic.Entities.AnimationActions
{
	// Token: 0x020001F7 RID: 503
	public class Immortal : AnimationAction
	{
		// Token: 0x0600108B RID: 4235 RVA: 0x000682B4 File Offset: 0x000664B4
		public Immortal(ContentReader iInput, SkinnedModelBoneCollection iSkeleton) : base(iInput, iSkeleton)
		{
			this.mCollide = iInput.ReadBoolean();
		}

		// Token: 0x0600108C RID: 4236 RVA: 0x000682CA File Offset: 0x000664CA
		protected override void InternalExecute(Character iOwner, bool iFirstExecution)
		{
			iOwner.SetImmortalTime(3f);
			if (!this.mCollide)
			{
				iOwner.CollisionIgnoreTime = 0.15f;
			}
		}

		// Token: 0x0600108D RID: 4237 RVA: 0x000682EA File Offset: 0x000664EA
		public override void Kill(Character iOwner)
		{
			iOwner.SetImmortalTime(0f);
			base.Kill(iOwner);
		}

		// Token: 0x1700042F RID: 1071
		// (get) Token: 0x0600108E RID: 4238 RVA: 0x000682FE File Offset: 0x000664FE
		public override bool UsesBones
		{
			get
			{
				return false;
			}
		}

		// Token: 0x04000F30 RID: 3888
		private bool mCollide;
	}
}
