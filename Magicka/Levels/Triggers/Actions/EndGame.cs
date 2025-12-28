using System;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x020000AB RID: 171
	internal class EndGame : Action
	{
		// Token: 0x060004F6 RID: 1270 RVA: 0x0001C416 File Offset: 0x0001A616
		public EndGame(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x060004F7 RID: 1271 RVA: 0x0001C427 File Offset: 0x0001A627
		protected override void Execute()
		{
			base.GameScene.PlayState.Endgame(this.mType, this.mFreezeGame, this.mPhony, 0f);
		}

		// Token: 0x060004F8 RID: 1272 RVA: 0x0001C450 File Offset: 0x0001A650
		public override void QuickExecute()
		{
		}

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x060004F9 RID: 1273 RVA: 0x0001C452 File Offset: 0x0001A652
		// (set) Token: 0x060004FA RID: 1274 RVA: 0x0001C45A File Offset: 0x0001A65A
		public EndGameCondition Type
		{
			get
			{
				return this.mType;
			}
			set
			{
				this.mType = value;
			}
		}

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x060004FB RID: 1275 RVA: 0x0001C463 File Offset: 0x0001A663
		// (set) Token: 0x060004FC RID: 1276 RVA: 0x0001C46B File Offset: 0x0001A66B
		public bool FreezeGame
		{
			get
			{
				return this.mFreezeGame;
			}
			set
			{
				this.mFreezeGame = value;
			}
		}

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x060004FD RID: 1277 RVA: 0x0001C474 File Offset: 0x0001A674
		// (set) Token: 0x060004FE RID: 1278 RVA: 0x0001C47C File Offset: 0x0001A67C
		public bool Phony
		{
			get
			{
				return this.mPhony;
			}
			set
			{
				this.mPhony = value;
			}
		}

		// Token: 0x0400039C RID: 924
		private EndGameCondition mType;

		// Token: 0x0400039D RID: 925
		private bool mFreezeGame = true;

		// Token: 0x0400039E RID: 926
		private bool mPhony;
	}
}
