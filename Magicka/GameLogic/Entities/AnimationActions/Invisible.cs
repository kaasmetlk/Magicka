using System;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

namespace Magicka.GameLogic.Entities.AnimationActions
{
	// Token: 0x02000252 RID: 594
	internal class Invisible : AnimationAction
	{
		// Token: 0x06001250 RID: 4688 RVA: 0x0007030F File Offset: 0x0006E50F
		public Invisible(ContentReader iInput, SkinnedModelBoneCollection iSkeleton) : base(iInput, iSkeleton)
		{
			this.mNoEffect = iInput.ReadBoolean();
		}

		// Token: 0x06001251 RID: 4689 RVA: 0x00070325 File Offset: 0x0006E525
		protected override void InternalExecute(Character iOwner, bool iFirstExecution)
		{
			if (iFirstExecution)
			{
				if (this.mNoEffect)
				{
					iOwner.DoNotRender = true;
					return;
				}
				iOwner.SetInvisible(float.MaxValue);
			}
		}

		// Token: 0x170004B6 RID: 1206
		// (get) Token: 0x06001252 RID: 4690 RVA: 0x00070345 File Offset: 0x0006E545
		public override bool UsesBones
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06001253 RID: 4691 RVA: 0x00070348 File Offset: 0x0006E548
		public override void Kill(Character iOwner)
		{
			if (this.mNoEffect)
			{
				iOwner.DoNotRender = false;
			}
			else
			{
				iOwner.SetInvisible(0f);
			}
			base.Kill(iOwner);
		}

		// Token: 0x0400110B RID: 4363
		private bool mNoEffect;
	}
}
