using System;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x020004F7 RID: 1271
	public class SetVolume : Action
	{
		// Token: 0x06002596 RID: 9622 RVA: 0x0011143B File Offset: 0x0010F63B
		public SetVolume(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06002597 RID: 9623 RVA: 0x00111445 File Offset: 0x0010F645
		protected override void Execute()
		{
			base.GameScene.ChangeAmbientSoundVolume(this.mID, this.mVolume);
		}

		// Token: 0x06002598 RID: 9624 RVA: 0x0011145E File Offset: 0x0010F65E
		public override void QuickExecute()
		{
		}

		// Token: 0x170008BE RID: 2238
		// (get) Token: 0x06002599 RID: 9625 RVA: 0x00111460 File Offset: 0x0010F660
		// (set) Token: 0x0600259A RID: 9626 RVA: 0x00111468 File Offset: 0x0010F668
		public string ID
		{
			get
			{
				return this.mIDStr;
			}
			set
			{
				this.mIDStr = value;
				this.mID = this.mIDStr.GetHashCodeCustom();
			}
		}

		// Token: 0x170008BF RID: 2239
		// (get) Token: 0x0600259B RID: 9627 RVA: 0x00111482 File Offset: 0x0010F682
		// (set) Token: 0x0600259C RID: 9628 RVA: 0x0011148A File Offset: 0x0010F68A
		public float Volume
		{
			get
			{
				return this.mVolume;
			}
			set
			{
				this.mVolume = value;
			}
		}

		// Token: 0x04002900 RID: 10496
		private string mIDStr;

		// Token: 0x04002901 RID: 10497
		private int mID;

		// Token: 0x04002902 RID: 10498
		private float mVolume;
	}
}
