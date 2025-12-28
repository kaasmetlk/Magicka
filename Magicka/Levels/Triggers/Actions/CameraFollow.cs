using System;
using Magicka.GameLogic.Entities;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x0200001D RID: 29
	internal class CameraFollow : Action
	{
		// Token: 0x060000DB RID: 219 RVA: 0x00006EEC File Offset: 0x000050EC
		public CameraFollow(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
			this.mScene = iScene;
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00006F00 File Offset: 0x00005100
		protected override void Execute()
		{
			Entity byID = Entity.GetByID(this.mTargetID);
			this.mScene.PlayState.Camera.Follow(byID);
		}

		// Token: 0x060000DD RID: 221 RVA: 0x00006F2F File Offset: 0x0000512F
		public override void QuickExecute()
		{
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060000DE RID: 222 RVA: 0x00006F31 File Offset: 0x00005131
		// (set) Token: 0x060000DF RID: 223 RVA: 0x00006F39 File Offset: 0x00005139
		public string ID
		{
			get
			{
				return this.mTarget;
			}
			set
			{
				this.mTarget = value;
				this.mTargetID = this.mTarget.GetHashCodeCustom();
				if (string.IsNullOrEmpty(this.mTarget))
				{
					throw new Exception("Must input correct ID");
				}
			}
		}

		// Token: 0x040000A1 RID: 161
		private string mTarget;

		// Token: 0x040000A2 RID: 162
		private int mTargetID;
	}
}
