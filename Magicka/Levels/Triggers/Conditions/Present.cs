using System;
using Magicka.GameLogic.Entities;

namespace Magicka.Levels.Triggers.Conditions
{
	// Token: 0x020004F3 RID: 1267
	internal class Present : Condition
	{
		// Token: 0x06002576 RID: 9590 RVA: 0x001110EA File Offset: 0x0010F2EA
		public Present(GameScene iScene) : base(iScene)
		{
		}

		// Token: 0x06002577 RID: 9591 RVA: 0x001110FC File Offset: 0x0010F2FC
		protected override bool InternalMet(Character iSender)
		{
			TriggerArea triggerArea = base.Scene.GetTriggerArea(this.mAreaID);
			int num = triggerArea.GetCount(this.mTypeID);
			if (!this.mIgnoreInvisibility)
			{
				StaticWeakList<Character> presentCharacters = triggerArea.PresentCharacters;
				foreach (Character character in presentCharacters)
				{
					if (character.Type == this.mTypeID && character.IsInvisibile)
					{
						num--;
					}
				}
			}
			switch (this.mCompareMethod)
			{
			case CompareMethod.LESS:
				if (num < this.mNr)
				{
					return true;
				}
				break;
			case CompareMethod.EQUAL:
				if (num == this.mNr)
				{
					return true;
				}
				break;
			case CompareMethod.GREATER:
				if (num > this.mNr)
				{
					return true;
				}
				break;
			}
			return false;
		}

		// Token: 0x170008B3 RID: 2227
		// (get) Token: 0x06002578 RID: 9592 RVA: 0x001111CC File Offset: 0x0010F3CC
		// (set) Token: 0x06002579 RID: 9593 RVA: 0x001111D4 File Offset: 0x0010F3D4
		public bool IncludeInvisible
		{
			get
			{
				return this.mIgnoreInvisibility;
			}
			set
			{
				this.mIgnoreInvisibility = value;
			}
		}

		// Token: 0x170008B4 RID: 2228
		// (get) Token: 0x0600257A RID: 9594 RVA: 0x001111DD File Offset: 0x0010F3DD
		// (set) Token: 0x0600257B RID: 9595 RVA: 0x001111E5 File Offset: 0x0010F3E5
		public string Area
		{
			get
			{
				return this.mArea;
			}
			set
			{
				this.mArea = value;
				this.mAreaID = this.mArea.GetHashCodeCustom();
			}
		}

		// Token: 0x170008B5 RID: 2229
		// (get) Token: 0x0600257C RID: 9596 RVA: 0x001111FF File Offset: 0x0010F3FF
		// (set) Token: 0x0600257D RID: 9597 RVA: 0x00111207 File Offset: 0x0010F407
		public string Type
		{
			get
			{
				return this.mType;
			}
			set
			{
				this.mType = value;
				this.mTypeID = this.mType.GetHashCodeCustom();
			}
		}

		// Token: 0x170008B6 RID: 2230
		// (get) Token: 0x0600257E RID: 9598 RVA: 0x00111221 File Offset: 0x0010F421
		// (set) Token: 0x0600257F RID: 9599 RVA: 0x00111229 File Offset: 0x0010F429
		public int Nr
		{
			get
			{
				return this.mNr;
			}
			set
			{
				this.mNr = value;
			}
		}

		// Token: 0x170008B7 RID: 2231
		// (get) Token: 0x06002580 RID: 9600 RVA: 0x00111232 File Offset: 0x0010F432
		// (set) Token: 0x06002581 RID: 9601 RVA: 0x0011123A File Offset: 0x0010F43A
		public CompareMethod CompareMethod
		{
			get
			{
				return this.mCompareMethod;
			}
			set
			{
				this.mCompareMethod = value;
			}
		}

		// Token: 0x040028EE RID: 10478
		private string mArea;

		// Token: 0x040028EF RID: 10479
		private int mAreaID;

		// Token: 0x040028F0 RID: 10480
		private string mType;

		// Token: 0x040028F1 RID: 10481
		private int mTypeID;

		// Token: 0x040028F2 RID: 10482
		private int mNr;

		// Token: 0x040028F3 RID: 10483
		private bool mIgnoreInvisibility = true;

		// Token: 0x040028F4 RID: 10484
		private CompareMethod mCompareMethod;
	}
}
