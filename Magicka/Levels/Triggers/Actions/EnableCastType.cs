using System;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x0200001E RID: 30
	public class EnableCastType : Action
	{
		// Token: 0x060000E0 RID: 224 RVA: 0x00006F6B File Offset: 0x0000516B
		public EnableCastType(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x00006F75 File Offset: 0x00005175
		protected override void Execute()
		{
			TutorialManager.Instance.EnableCastType(this.mCastType);
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x00006F87 File Offset: 0x00005187
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060000E3 RID: 227 RVA: 0x00006F8F File Offset: 0x0000518F
		// (set) Token: 0x060000E4 RID: 228 RVA: 0x00006F97 File Offset: 0x00005197
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

		// Token: 0x040000A3 RID: 163
		private CastType mCastType;
	}
}
