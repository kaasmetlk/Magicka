using System;

namespace Magicka.WebTools.JSON
{
	// Token: 0x0200036F RID: 879
	public class JSONFloatingPoint : JSONValue
	{
		// Token: 0x06001AD8 RID: 6872 RVA: 0x000B5C92 File Offset: 0x000B3E92
		public JSONFloatingPoint(string name, float value)
		{
			this.mName = name;
			this.mValue = (double)value;
		}

		// Token: 0x06001AD9 RID: 6873 RVA: 0x000B5CA9 File Offset: 0x000B3EA9
		public JSONFloatingPoint(string name, double value)
		{
			this.mName = name;
			this.mValue = value;
		}

		// Token: 0x06001ADA RID: 6874 RVA: 0x000B5CBF File Offset: 0x000B3EBF
		protected override string OneLine()
		{
			return string.Format("\"{0}\": \"{1}\"", this.mName, this.mValue);
		}

		// Token: 0x04001CFD RID: 7421
		private double mValue;
	}
}
