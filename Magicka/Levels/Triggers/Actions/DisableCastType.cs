using System;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000393 RID: 915
	public class DisableCastType : Action
	{
		// Token: 0x06001C0B RID: 7179 RVA: 0x000BF8D7 File Offset: 0x000BDAD7
		public DisableCastType(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06001C0C RID: 7180 RVA: 0x000BF8E1 File Offset: 0x000BDAE1
		protected override void Execute()
		{
			TutorialManager.Instance.DisableCastType(this.mCastType);
		}

		// Token: 0x06001C0D RID: 7181 RVA: 0x000BF8F3 File Offset: 0x000BDAF3
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x170006EA RID: 1770
		// (get) Token: 0x06001C0E RID: 7182 RVA: 0x000BF8FB File Offset: 0x000BDAFB
		// (set) Token: 0x06001C0F RID: 7183 RVA: 0x000BF903 File Offset: 0x000BDB03
		public CastType Type
		{
			get
			{
				return this.mCastType;
			}
			set
			{
				this.mCastType = value;
			}
		}

		// Token: 0x04001E48 RID: 7752
		private CastType mCastType;
	}
}
