using System;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x0200023E RID: 574
	public class StopSound : Action
	{
		// Token: 0x060011A0 RID: 4512 RVA: 0x0006C229 File Offset: 0x0006A429
		public StopSound(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x060011A1 RID: 4513 RVA: 0x0006C233 File Offset: 0x0006A433
		protected override void Execute()
		{
			base.GameScene.StopSound(this.mID, this.mInstant);
		}

		// Token: 0x060011A2 RID: 4514 RVA: 0x0006C24C File Offset: 0x0006A44C
		public override void QuickExecute()
		{
		}

		// Token: 0x17000483 RID: 1155
		// (get) Token: 0x060011A3 RID: 4515 RVA: 0x0006C24E File Offset: 0x0006A44E
		// (set) Token: 0x060011A4 RID: 4516 RVA: 0x0006C256 File Offset: 0x0006A456
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

		// Token: 0x17000484 RID: 1156
		// (get) Token: 0x060011A5 RID: 4517 RVA: 0x0006C270 File Offset: 0x0006A470
		// (set) Token: 0x060011A6 RID: 4518 RVA: 0x0006C278 File Offset: 0x0006A478
		public bool Instant
		{
			get
			{
				return this.mInstant;
			}
			set
			{
				this.mInstant = value;
			}
		}

		// Token: 0x04001076 RID: 4214
		private string mIDStr;

		// Token: 0x04001077 RID: 4215
		private int mID;

		// Token: 0x04001078 RID: 4216
		private bool mInstant;
	}
}
