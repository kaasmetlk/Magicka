using System;
using System.Collections.Generic;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

namespace Magicka.GameLogic.Entities.AnimationActions
{
	// Token: 0x020003FE RID: 1022
	public class PlayEffect : AnimationAction
	{
		// Token: 0x06001F7C RID: 8060 RVA: 0x000DDBC0 File Offset: 0x000DBDC0
		public PlayEffect(ContentReader iInput, SkinnedModelBoneCollection iSkeleton) : base(iInput, iSkeleton)
		{
			string value = iInput.ReadString();
			for (int i = 0; i < iSkeleton.Count; i++)
			{
				SkinnedModelBone skinnedModelBone = iSkeleton[i];
				if (skinnedModelBone.Name.Equals(value, StringComparison.InvariantCultureIgnoreCase))
				{
					this.mBoneBindPose = skinnedModelBone.InverseBindPoseTransform;
					this.mBoneBindPose *= Matrix.CreateRotationY(3.1415927f);
					Matrix.Invert(ref this.mBoneBindPose, out this.mBoneBindPose);
					this.mBoneIndex = i;
					break;
				}
			}
			this.mAttach = iInput.ReadBoolean();
			string iString = iInput.ReadString().ToLowerInvariant();
			this.mEffectHash = iString.GetHashCodeCustom();
			if (this.mStartTime < this.mEndTime)
			{
				this.mMemory = new Dictionary<int, VisualEffectReference>(8);
			}
			iInput.ReadSingle();
		}

		// Token: 0x06001F7D RID: 8061 RVA: 0x000DDC8C File Offset: 0x000DBE8C
		protected override void InternalExecute(Character iOwner, bool iFirstExecution)
		{
			Matrix boneOrientation = iOwner.GetBoneOrientation(this.mBoneIndex, ref this.mBoneBindPose);
			Vector3 translation = boneOrientation.Translation;
			Vector3 forward = boneOrientation.Forward;
			forward.Normalize();
			VisualEffectReference value;
			if (this.mMemory == null || !this.mMemory.TryGetValue((int)iOwner.Handle, out value))
			{
				if (iFirstExecution)
				{
					EffectManager.Instance.StartEffect(this.mEffectHash, ref translation, ref forward, out value);
					if (this.mMemory != null)
					{
						this.mMemory[(int)iOwner.Handle] = value;
					}
				}
				return;
			}
			if (EffectManager.Instance.UpdatePositionDirection(ref value, ref translation, ref forward))
			{
				this.mMemory[(int)iOwner.Handle] = value;
				return;
			}
			this.mMemory.Remove((int)iOwner.Handle);
		}

		// Token: 0x06001F7E RID: 8062 RVA: 0x000DDD4C File Offset: 0x000DBF4C
		public override void Kill(Character iOwner)
		{
			base.Kill(iOwner);
			VisualEffectReference visualEffectReference;
			if (this.mMemory != null && this.mMemory.TryGetValue((int)iOwner.Handle, out visualEffectReference))
			{
				EffectManager.Instance.Stop(ref visualEffectReference);
				this.mMemory.Remove((int)iOwner.Handle);
			}
		}

		// Token: 0x170007BA RID: 1978
		// (get) Token: 0x06001F7F RID: 8063 RVA: 0x000DDD9B File Offset: 0x000DBF9B
		public override bool UsesBones
		{
			get
			{
				return true;
			}
		}

		// Token: 0x040021E6 RID: 8678
		private int mBoneIndex;

		// Token: 0x040021E7 RID: 8679
		private Matrix mBoneBindPose;

		// Token: 0x040021E8 RID: 8680
		private bool mAttach;

		// Token: 0x040021E9 RID: 8681
		private int mEffectHash;

		// Token: 0x040021EA RID: 8682
		private Dictionary<int, VisualEffectReference> mMemory;
	}
}
