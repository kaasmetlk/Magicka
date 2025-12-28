using System;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000143 RID: 323
	internal class EnablePoi : Action
	{
		// Token: 0x06000920 RID: 2336 RVA: 0x000397C7 File Offset: 0x000379C7
		public EnablePoi(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06000921 RID: 2337 RVA: 0x000397D1 File Offset: 0x000379D1
		protected override void Execute()
		{
			(base.GameScene.Triggers[this.mIDHash] as Interactable).Enabled = true;
		}

		// Token: 0x06000922 RID: 2338 RVA: 0x000397F4 File Offset: 0x000379F4
		public override void QuickExecute()
		{
			(base.GameScene.Triggers[this.mIDHash] as Interactable).Enabled = true;
		}

		// Token: 0x170001D9 RID: 473
		// (get) Token: 0x06000923 RID: 2339 RVA: 0x00039817 File Offset: 0x00037A17
		// (set) Token: 0x06000924 RID: 2340 RVA: 0x0003981F File Offset: 0x00037A1F
		public string ID
		{
			get
			{
				return this.mID;
			}
			set
			{
				this.mID = value;
				this.mIDHash = this.mID.GetHashCodeCustom();
			}
		}

		// Token: 0x04000878 RID: 2168
		private string mID;

		// Token: 0x04000879 RID: 2169
		private int mIDHash;
	}
}
