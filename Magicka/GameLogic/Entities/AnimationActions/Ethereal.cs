using System;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

namespace Magicka.GameLogic.Entities.AnimationActions
{
	// Token: 0x020005AB RID: 1451
	internal class Ethereal : AnimationAction
	{
		// Token: 0x06002B74 RID: 11124 RVA: 0x00156F43 File Offset: 0x00155143
		public Ethereal(ContentReader iInput, SkinnedModelBoneCollection iSkeleton) : base(iInput, iSkeleton)
		{
			this.mEthereal = iInput.ReadBoolean();
			this.mAlpha = iInput.ReadSingle();
			this.mSpeed = iInput.ReadSingle();
		}

		// Token: 0x06002B75 RID: 11125 RVA: 0x00156F71 File Offset: 0x00155171
		protected override void InternalExecute(Character iOwner, bool iFirstExecution)
		{
			if (iFirstExecution)
			{
				if (this.mSpeed == 0f)
				{
					iOwner.EtherealAlpha = this.mAlpha;
					return;
				}
				iOwner.Ethereal(this.mEthereal, this.mAlpha, this.mSpeed);
			}
		}

		// Token: 0x17000A32 RID: 2610
		// (get) Token: 0x06002B76 RID: 11126 RVA: 0x00156FA8 File Offset: 0x001551A8
		public override bool UsesBones
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06002B77 RID: 11127 RVA: 0x00156FAB File Offset: 0x001551AB
		public override void Kill(Character iOwner)
		{
			base.Kill(iOwner);
		}

		// Token: 0x04002F13 RID: 12051
		private bool mEthereal;

		// Token: 0x04002F14 RID: 12052
		private float mAlpha;

		// Token: 0x04002F15 RID: 12053
		private float mSpeed;
	}
}
