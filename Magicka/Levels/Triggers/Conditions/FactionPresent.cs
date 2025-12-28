using System;
using Magicka.GameLogic.Entities;

namespace Magicka.Levels.Triggers.Conditions
{
	// Token: 0x02000281 RID: 641
	internal class FactionPresent : Condition
	{
		// Token: 0x060012F1 RID: 4849 RVA: 0x0007575C File Offset: 0x0007395C
		public FactionPresent(GameScene iScene) : base(iScene)
		{
		}

		// Token: 0x060012F2 RID: 4850 RVA: 0x0007576C File Offset: 0x0007396C
		protected override bool InternalMet(Character iSender)
		{
			TriggerArea triggerArea = base.Scene.GetTriggerArea(this.mAreaID);
			int num = triggerArea.GetFactionCount(this.mFactions);
			if (!this.mIgnoreInvisibility)
			{
				StaticWeakList<Character> presentCharacters = triggerArea.PresentCharacters;
				foreach (Character character in presentCharacters)
				{
					if ((character.Faction & this.mFactions) != Factions.NONE && character.IsInvisibile)
					{
						num--;
					}
				}
			}
			switch (this.mOperator)
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

		// Token: 0x170004CE RID: 1230
		// (get) Token: 0x060012F3 RID: 4851 RVA: 0x0007583C File Offset: 0x00073A3C
		// (set) Token: 0x060012F4 RID: 4852 RVA: 0x00075844 File Offset: 0x00073A44
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

		// Token: 0x170004CF RID: 1231
		// (get) Token: 0x060012F5 RID: 4853 RVA: 0x0007584D File Offset: 0x00073A4D
		// (set) Token: 0x060012F6 RID: 4854 RVA: 0x00075855 File Offset: 0x00073A55
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

		// Token: 0x170004D0 RID: 1232
		// (get) Token: 0x060012F7 RID: 4855 RVA: 0x0007586F File Offset: 0x00073A6F
		// (set) Token: 0x060012F8 RID: 4856 RVA: 0x00075877 File Offset: 0x00073A77
		public Factions Factions
		{
			get
			{
				return this.mFactions;
			}
			set
			{
				this.mFactions = value;
			}
		}

		// Token: 0x170004D1 RID: 1233
		// (get) Token: 0x060012F9 RID: 4857 RVA: 0x00075880 File Offset: 0x00073A80
		// (set) Token: 0x060012FA RID: 4858 RVA: 0x00075888 File Offset: 0x00073A88
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

		// Token: 0x170004D2 RID: 1234
		// (get) Token: 0x060012FB RID: 4859 RVA: 0x00075891 File Offset: 0x00073A91
		// (set) Token: 0x060012FC RID: 4860 RVA: 0x00075899 File Offset: 0x00073A99
		public CompareMethod CompareMethod
		{
			get
			{
				return this.mOperator;
			}
			set
			{
				this.mOperator = value;
			}
		}

		// Token: 0x040014BF RID: 5311
		private string mArea;

		// Token: 0x040014C0 RID: 5312
		private int mAreaID;

		// Token: 0x040014C1 RID: 5313
		private Factions mFactions;

		// Token: 0x040014C2 RID: 5314
		private int mNr;

		// Token: 0x040014C3 RID: 5315
		private CompareMethod mOperator;

		// Token: 0x040014C4 RID: 5316
		private bool mIgnoreInvisibility = true;
	}
}
