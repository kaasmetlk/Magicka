using System;
using GameSparks.Core;
using Magicka.CoreFramework;
using Magicka.Misc;

namespace Magicka.WebTools.GameSparks
{
	// Token: 0x020003AC RID: 940
	public class GameSparksEventHandler : Singleton<GameSparksEventHandler>
	{
		// Token: 0x06001CDC RID: 7388 RVA: 0x000CC99C File Offset: 0x000CAB9C
		public void HandleResponse(GSData iScriptData)
		{
			if (!iScriptData.ContainsKey("EventType"))
			{
				Logger.LogWarning(string.Format("GameSparksEventHandler was sent a response with no EventType Key, Script Data JSON follows: /n {0} /n", iScriptData.JSON));
				return;
			}
			string @string = iScriptData.GetString("EventType");
			string a;
			if ((a = @string) != null)
			{
				if (a == "AdClicked")
				{
					return;
				}
				if (a == "VariantRequest")
				{
					Singleton<GameSparksAccount>.Instance.Variant = iScriptData.GetString("Variant");
					return;
				}
			}
			Logger.LogWarning(string.Format("GameSparksEventHandler was sent a response with an unknown EventType '{0}', Script Data JSON follows: /n {1} /n", @string, iScriptData.JSON));
		}

		// Token: 0x04001F84 RID: 8068
		private const string EVENT_TYPE_KEY = "EventType";

		// Token: 0x04001F85 RID: 8069
		private const string VARIANT_KEY = "Variant";

		// Token: 0x04001F86 RID: 8070
		private const string AD_CLICKED_TYPE = "AdClicked";

		// Token: 0x04001F87 RID: 8071
		private const string VARIANT_REQUEST_TYPE = "VariantRequest";
	}
}
