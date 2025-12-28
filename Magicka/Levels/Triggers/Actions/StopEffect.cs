using System;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000283 RID: 643
	internal class StopEffect : Action
	{
		// Token: 0x06001304 RID: 4868 RVA: 0x00075960 File Offset: 0x00073B60
		public StopEffect(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06001305 RID: 4869 RVA: 0x0007596A File Offset: 0x00073B6A
		protected override void Execute()
		{
			base.GameScene.StopEffect(this.mNameHash);
		}

		// Token: 0x06001306 RID: 4870 RVA: 0x0007597E File Offset: 0x00073B7E
		public override void QuickExecute()
		{
			base.GameScene.StopEffect(this.mNameHash);
		}

		// Token: 0x170004D5 RID: 1237
		// (get) Token: 0x06001307 RID: 4871 RVA: 0x00075992 File Offset: 0x00073B92
		// (set) Token: 0x06001308 RID: 4872 RVA: 0x0007599A File Offset: 0x00073B9A
		public string Name
		{
			get
			{
				return this.mName;
			}
			set
			{
				this.mName = value;
				this.mNameHash = this.mName.GetHashCodeCustom();
			}
		}

		// Token: 0x040014C7 RID: 5319
		private string mName;

		// Token: 0x040014C8 RID: 5320
		private int mNameHash;
	}
}
