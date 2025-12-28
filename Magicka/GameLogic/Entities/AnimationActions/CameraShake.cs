using System;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

namespace Magicka.GameLogic.Entities.AnimationActions
{
	// Token: 0x020003CC RID: 972
	internal class CameraShake : AnimationAction
	{
		// Token: 0x06001DC8 RID: 7624 RVA: 0x000D1F0C File Offset: 0x000D010C
		public CameraShake(ContentReader iInput, SkinnedModelBoneCollection iSkeleton) : base(iInput, iSkeleton)
		{
			iInput.ReadString();
			this.mDuration = iInput.ReadSingle();
			this.mMagnitude = iInput.ReadSingle();
		}

		// Token: 0x06001DC9 RID: 7625 RVA: 0x000D1F35 File Offset: 0x000D0135
		protected override void InternalExecute(Character iOwner, bool iFirstExecution)
		{
			if (iFirstExecution)
			{
				iOwner.PlayState.Camera.CameraShake(iOwner.Position, this.mMagnitude, this.mDuration);
			}
		}

		// Token: 0x17000756 RID: 1878
		// (get) Token: 0x06001DCA RID: 7626 RVA: 0x000D1F5C File Offset: 0x000D015C
		public override bool UsesBones
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0400204F RID: 8271
		private float mDuration;

		// Token: 0x04002050 RID: 8272
		private float mMagnitude;
	}
}
