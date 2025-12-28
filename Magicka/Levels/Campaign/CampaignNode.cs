using System;

namespace Magicka.Levels.Campaign
{
	// Token: 0x0200028F RID: 655
	internal class CampaignNode : LevelNode
	{
		// Token: 0x06001354 RID: 4948 RVA: 0x00076D37 File Offset: 0x00074F37
		public CampaignNode(string iFileName, SpawnPoint? iSpawnPoint) : base("", iFileName)
		{
			this.SpawnPoint = iSpawnPoint;
		}

		// Token: 0x040014F0 RID: 5360
		internal SpawnPoint? SpawnPoint;

		// Token: 0x040014F1 RID: 5361
		internal Cutscene Cutscene;
	}
}
