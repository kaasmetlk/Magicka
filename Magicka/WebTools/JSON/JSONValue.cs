using System;

namespace Magicka.WebTools.JSON
{
	// Token: 0x0200036A RID: 874
	public abstract class JSONValue
	{
		// Token: 0x06001AC1 RID: 6849 RVA: 0x000B58F4 File Offset: 0x000B3AF4
		public virtual string JSONLine()
		{
			return this.JSONLine(true, true);
		}

		// Token: 0x06001AC2 RID: 6850 RVA: 0x000B58FE File Offset: 0x000B3AFE
		public string JSONLine(bool addComma, bool addNewLine)
		{
			return string.Format("{0}{1}{2}", this.OneLine(), addComma ? ", " : "", addNewLine ? "\n" : "").Trim();
		}

		// Token: 0x06001AC3 RID: 6851
		protected abstract string OneLine();

		// Token: 0x04001CF3 RID: 7411
		protected string mName;
	}
}
