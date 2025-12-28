using System;
using Magicka.Audio;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000239 RID: 569
	public class StopMusic : Action
	{
		// Token: 0x06001172 RID: 4466 RVA: 0x0006BE4C File Offset: 0x0006A04C
		public StopMusic(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06001173 RID: 4467 RVA: 0x0006BE56 File Offset: 0x0006A056
		protected override void Execute()
		{
			AudioManager.Instance.StopMusic();
		}

		// Token: 0x06001174 RID: 4468 RVA: 0x0006BE62 File Offset: 0x0006A062
		public override void QuickExecute()
		{
		}
	}
}
