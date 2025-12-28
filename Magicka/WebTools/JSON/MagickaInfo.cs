using System;
using System.Text;
using SteamWrapper;

namespace Magicka.WebTools.JSON
{
	// Token: 0x020004EF RID: 1263
	public class MagickaInfo : JSONObject
	{
		// Token: 0x170008AA RID: 2218
		// (get) Token: 0x06002559 RID: 9561 RVA: 0x0010FD0C File Offset: 0x0010DF0C
		public string TimeStampString
		{
			get
			{
				return this.timeStampStr;
			}
		}

		// Token: 0x0600255A RID: 9562 RVA: 0x0010FD14 File Offset: 0x0010DF14
		public MagickaInfo()
		{
			this.mUserID = SteamUser.GetSteamID();
			this.timeStampStr = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fff");
			this.collection.Add(new JSONString(this.GAME, this.GAMENAME));
			this.collection.Add(new JSONString(this.UNIVERSE, this.PLATFORM));
			this.collection.Add(new JSONIntegral(this.USER_ID, this.mUserID.AsUInt64));
		}

		// Token: 0x0600255B RID: 9563 RVA: 0x0010FDDC File Offset: 0x0010DFDC
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("{\n");
			foreach (JSONValue jsonvalue in this.collection)
			{
				stringBuilder.AppendLine("\t" + jsonvalue.JSONLine());
			}
			stringBuilder.Append("\n}");
			return stringBuilder.ToString().Trim();
		}

		// Token: 0x040028D5 RID: 10453
		private readonly string GAME = "game";

		// Token: 0x040028D6 RID: 10454
		private readonly string GAMENAME = "magicka";

		// Token: 0x040028D7 RID: 10455
		private readonly string UNIVERSE = "universe";

		// Token: 0x040028D8 RID: 10456
		private readonly string PLATFORM = "steam";

		// Token: 0x040028D9 RID: 10457
		private readonly string USER_ID = "userid";

		// Token: 0x040028DA RID: 10458
		private SteamID mUserID;

		// Token: 0x040028DB RID: 10459
		private string timeStampStr;
	}
}
