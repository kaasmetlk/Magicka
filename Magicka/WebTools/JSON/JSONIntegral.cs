using System;

namespace Magicka.WebTools.JSON
{
	// Token: 0x0200036C RID: 876
	public class JSONIntegral : JSONValue
	{
		// Token: 0x06001AC7 RID: 6855 RVA: 0x000B5969 File Offset: 0x000B3B69
		public JSONIntegral(string name, int value)
		{
			this.mName = name;
			this.mValue = (long)value;
		}

		// Token: 0x06001AC8 RID: 6856 RVA: 0x000B5980 File Offset: 0x000B3B80
		public JSONIntegral(string name, short value)
		{
			this.mName = name;
			this.mValue = (long)value;
		}

		// Token: 0x06001AC9 RID: 6857 RVA: 0x000B5997 File Offset: 0x000B3B97
		public JSONIntegral(string name, long value)
		{
			this.mName = name;
			this.mValue = value;
		}

		// Token: 0x06001ACA RID: 6858 RVA: 0x000B59AD File Offset: 0x000B3BAD
		public JSONIntegral(string name, double value)
		{
			this.mName = name;
			this.mValue = (long)Math.Floor(value);
		}

		// Token: 0x06001ACB RID: 6859 RVA: 0x000B59C9 File Offset: 0x000B3BC9
		public JSONIntegral(string name, ulong value)
		{
			this.mName = name;
			this.mUValue = value;
			this.mUnsigned = true;
		}

		// Token: 0x06001ACC RID: 6860 RVA: 0x000B59E8 File Offset: 0x000B3BE8
		protected override string OneLine()
		{
			if (this.mUnsigned)
			{
				return string.Format("\"{0}\":\"{1}\"", this.mName, this.mUValue.ToString());
			}
			return string.Format("\"{0}\":\"{1}\"", this.mName, this.mValue.ToString());
		}

		// Token: 0x04001CF5 RID: 7413
		private long mValue;

		// Token: 0x04001CF6 RID: 7414
		private ulong mUValue;

		// Token: 0x04001CF7 RID: 7415
		private bool mUnsigned;
	}
}
