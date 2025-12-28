using System;
using System.Xml;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x020003E8 RID: 1000
	internal class EndCutscene : Action
	{
		// Token: 0x06001EAB RID: 7851 RVA: 0x000D616C File Offset: 0x000D436C
		public EndCutscene(Trigger iTrigger, GameScene iScene, XmlNode iNode) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06001EAC RID: 7852 RVA: 0x000D6176 File Offset: 0x000D4376
		protected override void Execute()
		{
			this.mScene.PlayState.EndCutscene(this.mSkipBarMove);
		}

		// Token: 0x06001EAD RID: 7853 RVA: 0x000D618E File Offset: 0x000D438E
		public override void QuickExecute()
		{
			this.mScene.PlayState.EndCutscene(true);
		}

		// Token: 0x17000781 RID: 1921
		// (get) Token: 0x06001EAE RID: 7854 RVA: 0x000D61A1 File Offset: 0x000D43A1
		// (set) Token: 0x06001EAF RID: 7855 RVA: 0x000D61A9 File Offset: 0x000D43A9
		public bool SkipBarMove
		{
			get
			{
				return this.mSkipBarMove;
			}
			set
			{
				this.mSkipBarMove = value;
			}
		}

		// Token: 0x040020F9 RID: 8441
		private bool mSkipBarMove;
	}
}
