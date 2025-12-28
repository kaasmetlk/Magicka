using System;
using System.Collections.Generic;
using System.Text;

namespace Magicka.WebTools.JSON
{
	// Token: 0x0200036D RID: 877
	public class JSONDataEntry : JSONValue
	{
		// Token: 0x06001ACD RID: 6861 RVA: 0x000B5A34 File Offset: 0x000B3C34
		public void SetTimeStamp(string time)
		{
			this.timeStamp = new JSONString(this.timeStampKey, time);
		}

		// Token: 0x06001ACE RID: 6862 RVA: 0x000B5A48 File Offset: 0x000B3C48
		public void Add(JSONValue val)
		{
			if (this.values == null)
			{
				this.values = new List<JSONValue>();
			}
			this.values.Add(val);
		}

		// Token: 0x06001ACF RID: 6863 RVA: 0x000B5A6C File Offset: 0x000B3C6C
		protected override string OneLine()
		{
			if (this.timeStamp == null)
			{
				this.SetTimeStamp(DateTime.Now.ToUniversalTime().ToString());
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("{");
			stringBuilder.Append(this.timeStamp.JSONLine(true, false));
			if (this.values != null && this.values.Count > 0)
			{
				for (int i = 0; i < this.values.Count; i++)
				{
					bool addComma = i < this.values.Count - 1;
					stringBuilder.Append(this.values[i].JSONLine(addComma, false));
				}
			}
			stringBuilder.Append("}");
			return stringBuilder.ToString().Trim();
		}

		// Token: 0x04001CF8 RID: 7416
		private List<JSONValue> values;

		// Token: 0x04001CF9 RID: 7417
		private JSONValue timeStamp;

		// Token: 0x04001CFA RID: 7418
		private readonly string timeStampKey = "timestamp";
	}
}
