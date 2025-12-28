using System;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.GameStates.Menu.Main;

namespace Magicka.Levels.Triggers.Conditions
{
	// Token: 0x020002C6 RID: 710
	internal class IsCampaignLooped : Condition
	{
		// Token: 0x060015A1 RID: 5537 RVA: 0x0008A8A3 File Offset: 0x00088AA3
		public IsCampaignLooped(GameScene iScene) : base(iScene)
		{
		}

		// Token: 0x060015A2 RID: 5538 RVA: 0x0008A8AC File Offset: 0x00088AAC
		protected override bool InternalMet(Character iSender)
		{
			if (base.Scene.PlayState.GameType != GameType.Campaign)
			{
				throw new Exception("IsCampaignLooped can only be used in gametype campaign!");
			}
			return SubMenuCampaignSelect_SaveSlotSelect.Instance.CurrentSaveData.Looped;
		}
	}
}
