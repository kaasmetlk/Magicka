using System;

namespace Magicka.WebTools.JSON
{
	// Token: 0x0200036B RID: 875
	public class JSONString : JSONValue
	{
		// Token: 0x06001AC5 RID: 6853 RVA: 0x000B593B File Offset: 0x000B3B3B
		public JSONString(string name, string value)
		{
			this.mName = name;
			this.mValue = value;
		}

		// Token: 0x06001AC6 RID: 6854 RVA: 0x000B5951 File Offset: 0x000B3B51
		protected override string OneLine()
		{
			return string.Format("\"{0}\":\"{1}\"", this.mName, this.mValue);
		}

		// Token: 0x04001CF4 RID: 7412
		private string mValue;
	}
}
