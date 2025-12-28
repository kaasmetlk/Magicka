using System;

namespace Magicka.WebTools.JSON
{
	// Token: 0x02000370 RID: 880
	public class JSONBoolean : JSONValue
	{
		// Token: 0x06001ADB RID: 6875 RVA: 0x000B5CDC File Offset: 0x000B3EDC
		public JSONBoolean(string name, bool value)
		{
			this.mName = name;
			this.mValue = value;
		}

		// Token: 0x06001ADC RID: 6876 RVA: 0x000B5CF2 File Offset: 0x000B3EF2
		protected override string OneLine()
		{
			return string.Format("\"{0}\": \"{1}\"", this.mName, this.mValue);
		}

		// Token: 0x04001CFE RID: 7422
		private bool mValue;
	}
}
