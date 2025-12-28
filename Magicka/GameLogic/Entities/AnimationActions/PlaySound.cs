using System;
using Magicka.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

namespace Magicka.GameLogic.Entities.AnimationActions
{
	// Token: 0x0200057E RID: 1406
	public class PlaySound : AnimationAction
	{
		// Token: 0x06002A19 RID: 10777 RVA: 0x0014B517 File Offset: 0x00149717
		public PlaySound(ContentReader iInput, SkinnedModelBoneCollection iSkeleton) : base(iInput, iSkeleton)
		{
			this.mSound = iInput.ReadString();
			this.mBank = (Banks)iInput.ReadInt32();
			this.mSoundHash = this.mSound.GetHashCodeCustom();
		}

		// Token: 0x06002A1A RID: 10778 RVA: 0x0014B54C File Offset: 0x0014974C
		protected override void InternalExecute(Character iOwner, bool iFirstExecution)
		{
			if (iFirstExecution)
			{
				iOwner.AudioEmitter.Position = iOwner.Position;
				iOwner.AudioEmitter.Up = Vector3.Up;
				iOwner.AudioEmitter.Forward = iOwner.Direction;
				AudioManager.Instance.PlayCue(this.mBank, this.mSoundHash, iOwner.AudioEmitter);
			}
		}

		// Token: 0x170009E8 RID: 2536
		// (get) Token: 0x06002A1B RID: 10779 RVA: 0x0014B5AB File Offset: 0x001497AB
		public override bool UsesBones
		{
			get
			{
				return false;
			}
		}

		// Token: 0x04002D85 RID: 11653
		private string mSound;

		// Token: 0x04002D86 RID: 11654
		private Banks mBank;

		// Token: 0x04002D87 RID: 11655
		private int mSoundHash;
	}
}
