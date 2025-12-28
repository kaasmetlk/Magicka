using System;
using System.Collections.Generic;
using System.Linq;

namespace Magicka.WebTools.JSON
{
	// Token: 0x020004EE RID: 1262
	public class JSONObject
	{
		// Token: 0x06002556 RID: 9558 RVA: 0x0010FC38 File Offset: 0x0010DE38
		public JSONObject()
		{
			this.collection = new List<JSONValue>();
		}

		// Token: 0x06002557 RID: 9559 RVA: 0x0010FC56 File Offset: 0x0010DE56
		public void Add(JSONValue js)
		{
			this.collection.Add(js);
		}

		// Token: 0x06002558 RID: 9560 RVA: 0x0010FC64 File Offset: 0x0010DE64
		public new virtual string ToString()
		{
			string text = "{\n";
			foreach (JSONValue jsonvalue in this.collection)
			{
				if (jsonvalue == this.collection.Last<JSONValue>())
				{
					text = string.Format("{0}\t{1}", text, jsonvalue.JSONLine());
					int startIndex = text.LastIndexOf(",");
					text = text.Remove(startIndex, 1);
				}
				else
				{
					text = string.Format("{0}\t{1}", text, jsonvalue.JSONLine());
				}
			}
			text += "},";
			return text;
		}

		// Token: 0x040028D3 RID: 10451
		protected readonly string TIMESTAMP = "timestamp";

		// Token: 0x040028D4 RID: 10452
		protected List<JSONValue> collection;
	}
}
