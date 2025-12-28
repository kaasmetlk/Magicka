using System;
using Magicka.GameLogic.Entities.Bosses;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x0200001F RID: 31
	public class BossMessage : Action
	{
		// Token: 0x060000E5 RID: 229 RVA: 0x00006FA0 File Offset: 0x000051A0
		public BossMessage(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00006FAA File Offset: 0x000051AA
		protected override void Execute()
		{
			BossFight.Instance.PassMessage(this.mMessage);
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x00006FBC File Offset: 0x000051BC
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060000E8 RID: 232 RVA: 0x00006FC4 File Offset: 0x000051C4
		// (set) Token: 0x060000E9 RID: 233 RVA: 0x00006FCB File Offset: 0x000051CB
		public string Message
		{
			get
			{
				return "";
			}
			set
			{
				this.mMessage = (BossMessages)Enum.Parse(typeof(BossMessages), value, true);
			}
		}

		// Token: 0x040000A4 RID: 164
		private BossMessages mMessage;
	}
}
