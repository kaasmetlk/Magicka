using System;
using Magicka.Graphics;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000392 RID: 914
	public class Flash : Action
	{
		// Token: 0x06001C06 RID: 7174 RVA: 0x000BF887 File Offset: 0x000BDA87
		public Flash(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06001C07 RID: 7175 RVA: 0x000BF89C File Offset: 0x000BDA9C
		protected override void Execute()
		{
			Flash.Instance.Execute(this.mScene.PlayState.Scene, this.mDuration);
		}

		// Token: 0x06001C08 RID: 7176 RVA: 0x000BF8BE File Offset: 0x000BDABE
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x170006E9 RID: 1769
		// (get) Token: 0x06001C09 RID: 7177 RVA: 0x000BF8C6 File Offset: 0x000BDAC6
		// (set) Token: 0x06001C0A RID: 7178 RVA: 0x000BF8CE File Offset: 0x000BDACE
		public float Duration
		{
			get
			{
				return this.mDuration;
			}
			set
			{
				this.mDuration = value;
			}
		}

		// Token: 0x04001E47 RID: 7751
		private float mDuration = 0.5f;
	}
}
