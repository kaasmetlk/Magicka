using System;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

namespace Magicka.GameLogic.Entities.AnimationActions
{
	// Token: 0x020004D7 RID: 1239
	internal class Tongue : AnimationAction
	{
		// Token: 0x060024DE RID: 9438 RVA: 0x0010A657 File Offset: 0x00108857
		public Tongue(ContentReader iInput, SkinnedModelBoneCollection iSkeleton) : base(iInput, iSkeleton)
		{
			this.mMaxLength = iInput.ReadSingle();
		}

		// Token: 0x060024DF RID: 9439 RVA: 0x0010A670 File Offset: 0x00108870
		protected override void InternalExecute(Character iOwner, bool iFirstExecution)
		{
			if (iFirstExecution)
			{
				FrogTongue instance = FrogTongue.GetInstance(iOwner.PlayState);
				instance.Initialize(iOwner, iOwner.GetMouthAttachOrientation().Forward, this.mMaxLength * this.mMaxLength);
				iOwner.PlayState.EntityManager.AddEntity(instance);
			}
		}

		// Token: 0x170008A1 RID: 2209
		// (get) Token: 0x060024E0 RID: 9440 RVA: 0x0010A6BF File Offset: 0x001088BF
		public override bool UsesBones
		{
			get
			{
				return true;
			}
		}

		// Token: 0x04002841 RID: 10305
		private float mMaxLength;
	}
}
