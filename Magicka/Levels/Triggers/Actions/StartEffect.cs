using System;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x020001A5 RID: 421
	public class StartEffect : Action
	{
		// Token: 0x06000C71 RID: 3185 RVA: 0x0004A99F File Offset: 0x00048B9F
		public StartEffect(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06000C72 RID: 3186 RVA: 0x0004A9A9 File Offset: 0x00048BA9
		protected override void Execute()
		{
			if (string.IsNullOrEmpty(this.mName))
			{
				this.mNameHash = base.GameScene.GetGeneratedEffectID();
			}
			base.GameScene.StartEffect(this.mNameHash);
		}

		// Token: 0x06000C73 RID: 3187 RVA: 0x0004A9DA File Offset: 0x00048BDA
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x170002E4 RID: 740
		// (get) Token: 0x06000C74 RID: 3188 RVA: 0x0004A9E2 File Offset: 0x00048BE2
		// (set) Token: 0x06000C75 RID: 3189 RVA: 0x0004A9EA File Offset: 0x00048BEA
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

		// Token: 0x04000B70 RID: 2928
		private string mName;

		// Token: 0x04000B71 RID: 2929
		private int mNameHash;
	}
}
