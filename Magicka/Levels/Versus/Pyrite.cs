using System;
using System.Xml;
using Magicka.GameLogic;

namespace Magicka.Levels.Versus
{
	// Token: 0x0200047E RID: 1150
	internal class Pyrite : VersusRuleset
	{
		// Token: 0x060022D5 RID: 8917 RVA: 0x000FB698 File Offset: 0x000F9898
		public Pyrite(GameScene iScene, XmlNode iNode, Pyrite.Settings iSettings) : base(iScene, iNode)
		{
			this.mSettings = iSettings;
		}

		// Token: 0x060022D6 RID: 8918 RVA: 0x000FB6A9 File Offset: 0x000F98A9
		public override void Initialize()
		{
			base.Initialize();
		}

		// Token: 0x060022D7 RID: 8919 RVA: 0x000FB6B1 File Offset: 0x000F98B1
		public override void DeInitialize()
		{
		}

		// Token: 0x17000845 RID: 2117
		// (get) Token: 0x060022D8 RID: 8920 RVA: 0x000FB6B3 File Offset: 0x000F98B3
		public override Rulesets RulesetType
		{
			get
			{
				return Rulesets.Pyrite;
			}
		}

		// Token: 0x060022D9 RID: 8921 RVA: 0x000FB6B6 File Offset: 0x000F98B6
		public override bool CanRevive(Player iReviver, Player iRevivee)
		{
			return false;
		}

		// Token: 0x060022DA RID: 8922 RVA: 0x000FB6B9 File Offset: 0x000F98B9
		internal override short[] GetScores()
		{
			return null;
		}

		// Token: 0x17000846 RID: 2118
		// (get) Token: 0x060022DB RID: 8923 RVA: 0x000FB6BC File Offset: 0x000F98BC
		internal override bool Teams
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060022DC RID: 8924 RVA: 0x000FB6BF File Offset: 0x000F98BF
		internal override short[] GetTeamScores()
		{
			return null;
		}

		// Token: 0x04002611 RID: 9745
		private Pyrite.Settings mSettings;

		// Token: 0x0200047F RID: 1151
		internal new class Settings : VersusRuleset.Settings
		{
			// Token: 0x17000847 RID: 2119
			// (get) Token: 0x060022DD RID: 8925 RVA: 0x000FB6C2 File Offset: 0x000F98C2
			public override bool TeamsEnabled
			{
				get
				{
					return false;
				}
			}
		}
	}
}
