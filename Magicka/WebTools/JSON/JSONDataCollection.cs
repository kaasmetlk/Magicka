using System;
using System.Collections.Generic;
using System.Text;

namespace Magicka.WebTools.JSON
{
	// Token: 0x0200036E RID: 878
	public class JSONDataCollection : JSONValue
	{
		// Token: 0x06001AD1 RID: 6865 RVA: 0x000B5B4C File Offset: 0x000B3D4C
		public JSONDataCollection() : this(null)
		{
		}

		// Token: 0x06001AD2 RID: 6866 RVA: 0x000B5B58 File Offset: 0x000B3D58
		public JSONDataCollection(string time)
		{
			if (string.IsNullOrEmpty(time))
			{
				this.timeStamp = DateTime.Now.ToUniversalTime().ToString();
				return;
			}
			this.timeStamp = time;
		}

		// Token: 0x06001AD3 RID: 6867 RVA: 0x000B5B9C File Offset: 0x000B3D9C
		public void Add(JSONDataEntry val)
		{
			this.Add(val, true);
		}

		// Token: 0x06001AD4 RID: 6868 RVA: 0x000B5BA6 File Offset: 0x000B3DA6
		public void Add(JSONDataEntry val, bool useThisTimeStamp)
		{
			if (this.values == null)
			{
				this.values = new List<JSONDataEntry>();
			}
			if (useThisTimeStamp)
			{
				val.SetTimeStamp(this.timeStamp);
			}
			this.values.Add(val);
		}

		// Token: 0x06001AD5 RID: 6869 RVA: 0x000B5BD6 File Offset: 0x000B3DD6
		public override string JSONLine()
		{
			return this.OneLine();
		}

		// Token: 0x06001AD6 RID: 6870 RVA: 0x000B5BE0 File Offset: 0x000B3DE0
		protected override string OneLine()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("\"data\": [\n");
			if (this.values != null && this.values.Count > 0)
			{
				for (int i = 0; i < this.values.Count; i++)
				{
					if (this.values[i] != null)
					{
						bool flag = i == this.values.Count - 1;
						stringBuilder.Append("\t" + this.values[i].JSONLine(!flag, true));
					}
				}
			}
			stringBuilder.Append("\n]");
			return stringBuilder.ToString().Trim();
		}

		// Token: 0x06001AD7 RID: 6871 RVA: 0x000B5C8A File Offset: 0x000B3E8A
		public override string ToString()
		{
			return this.OneLine();
		}

		// Token: 0x04001CFB RID: 7419
		private List<JSONDataEntry> values;

		// Token: 0x04001CFC RID: 7420
		private string timeStamp;
	}
}
