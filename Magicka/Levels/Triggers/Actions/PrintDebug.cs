using System;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000244 RID: 580
	public class PrintDebug : Action
	{
		// Token: 0x170004A2 RID: 1186
		// (get) Token: 0x060011F1 RID: 4593 RVA: 0x0006CD82 File Offset: 0x0006AF82
		// (set) Token: 0x060011F2 RID: 4594 RVA: 0x0006CD8A File Offset: 0x0006AF8A
		public string EntityPos
		{
			get
			{
				return this.mEntityID;
			}
			set
			{
				this.mEntityID = value;
			}
		}

		// Token: 0x170004A3 RID: 1187
		// (get) Token: 0x060011F3 RID: 4595 RVA: 0x0006CD93 File Offset: 0x0006AF93
		// (set) Token: 0x060011F4 RID: 4596 RVA: 0x0006CD9B File Offset: 0x0006AF9B
		public bool Break
		{
			get
			{
				return this.mBreak;
			}
			set
			{
				this.mBreak = value;
			}
		}

		// Token: 0x170004A4 RID: 1188
		// (get) Token: 0x060011F5 RID: 4597 RVA: 0x0006CDA4 File Offset: 0x0006AFA4
		// (set) Token: 0x060011F6 RID: 4598 RVA: 0x0006CDAC File Offset: 0x0006AFAC
		public string CounterName
		{
			get
			{
				return this.mCounterName;
			}
			set
			{
				this.mCounterName = value;
			}
		}

		// Token: 0x170004A5 RID: 1189
		// (get) Token: 0x060011F7 RID: 4599 RVA: 0x0006CDB5 File Offset: 0x0006AFB5
		// (set) Token: 0x060011F8 RID: 4600 RVA: 0x0006CDBD File Offset: 0x0006AFBD
		public string Message
		{
			get
			{
				return this.mMessage;
			}
			set
			{
				this.mMessage = value;
			}
		}

		// Token: 0x060011F9 RID: 4601 RVA: 0x0006CDC6 File Offset: 0x0006AFC6
		public PrintDebug(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x060011FA RID: 4602 RVA: 0x0006CDE6 File Offset: 0x0006AFE6
		public override void Initialize()
		{
		}

		// Token: 0x060011FB RID: 4603 RVA: 0x0006CDE8 File Offset: 0x0006AFE8
		public override void QuickExecute()
		{
		}

		// Token: 0x060011FC RID: 4604 RVA: 0x0006CDEA File Offset: 0x0006AFEA
		private string CounterValue()
		{
			return "";
		}

		// Token: 0x060011FD RID: 4605 RVA: 0x0006CDF1 File Offset: 0x0006AFF1
		private string EntityPosition()
		{
			return "";
		}

		// Token: 0x060011FE RID: 4606 RVA: 0x0006CDF8 File Offset: 0x0006AFF8
		protected override void Execute()
		{
		}

		// Token: 0x040010AC RID: 4268
		private string mMessage = "";

		// Token: 0x040010AD RID: 4269
		private bool mBreak;

		// Token: 0x040010AE RID: 4270
		private string mCounterName = "";

		// Token: 0x040010AF RID: 4271
		private int mCounterID;

		// Token: 0x040010B0 RID: 4272
		private string mEntityID;

		// Token: 0x040010B1 RID: 4273
		private int mEntityIDHash;

		// Token: 0x040010B2 RID: 4274
		private static readonly string PLAYER1 = "player1";

		// Token: 0x040010B3 RID: 4275
		private static readonly string PLAYER2 = "player2";

		// Token: 0x040010B4 RID: 4276
		private static readonly string PLAYER3 = "player3";

		// Token: 0x040010B5 RID: 4277
		private static readonly string PLAYER4 = "player4";
	}
}
