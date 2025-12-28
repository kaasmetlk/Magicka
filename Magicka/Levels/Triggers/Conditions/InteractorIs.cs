using System;
using Magicka.GameLogic.Entities;

namespace Magicka.Levels.Triggers.Conditions
{
	// Token: 0x020000B8 RID: 184
	public class InteractorIs : Condition
	{
		// Token: 0x0600055F RID: 1375 RVA: 0x00020146 File Offset: 0x0001E346
		public InteractorIs(GameScene iScene) : base(iScene)
		{
		}

		// Token: 0x06000560 RID: 1376 RVA: 0x0002014F File Offset: 0x0001E34F
		protected override bool InternalMet(Character iSender)
		{
			return iSender != null && iSender.UniqueID == this.mID;
		}

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x06000561 RID: 1377 RVA: 0x00020164 File Offset: 0x0001E364
		// (set) Token: 0x06000562 RID: 1378 RVA: 0x0002016C File Offset: 0x0001E36C
		public string ID
		{
			get
			{
				return this.mIDStr;
			}
			set
			{
				this.mIDStr = value;
				this.mID = this.mIDStr.GetHashCodeCustom();
			}
		}

		// Token: 0x0400042C RID: 1068
		private string mIDStr;

		// Token: 0x0400042D RID: 1069
		private int mID;
	}
}
