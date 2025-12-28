using System;

namespace Magicka.WebTools.Paradox.Telemetry
{
	// Token: 0x0200054F RID: 1359
	public class EventData
	{
		// Token: 0x06002863 RID: 10339 RVA: 0x0013C88B File Offset: 0x0013AA8B
		public EventData(string iEventName, params object[] iParameters)
		{
			if (string.IsNullOrEmpty(iEventName))
			{
				throw new Exception("Cannot send a telemetry event without name.");
			}
			this.mName = iEventName;
			this.mParameters = iParameters;
		}

		// Token: 0x17000973 RID: 2419
		// (get) Token: 0x06002864 RID: 10340 RVA: 0x0013C8BF File Offset: 0x0013AABF
		public string Name
		{
			get
			{
				return this.mName;
			}
		}

		// Token: 0x17000974 RID: 2420
		// (get) Token: 0x06002865 RID: 10341 RVA: 0x0013C8C7 File Offset: 0x0013AAC7
		public int ParameterCount
		{
			get
			{
				if (this.mParameters == null)
				{
					return 0;
				}
				return this.mParameters.Length;
			}
		}

		// Token: 0x17000975 RID: 2421
		// (get) Token: 0x06002866 RID: 10342 RVA: 0x0013C8DB File Offset: 0x0013AADB
		public object[] Parameters
		{
			get
			{
				if (this.mParameters == null)
				{
					return new object[0];
				}
				return this.mParameters;
			}
		}

		// Token: 0x06002867 RID: 10343 RVA: 0x0013C8F2 File Offset: 0x0013AAF2
		public object GetParameter(int iIndex)
		{
			if (this.mParameters == null)
			{
				throw new Exception("Cannot access a parameter from an event data with an empty parameter list.");
			}
			if (iIndex < 0 || iIndex >= this.mParameters.Length)
			{
				throw new IndexOutOfRangeException("Index out of range when trying to access a parameter from an event data.");
			}
			return this.mParameters[iIndex];
		}

		// Token: 0x04002BDF RID: 11231
		private const string EXCEPTION_NO_EVENT_NAME = "Cannot send a telemetry event without name.";

		// Token: 0x04002BE0 RID: 11232
		private const string EXCEPTION_NULL_PARAMETER_ARRAY = "Cannot access a parameter from an event data with an empty parameter list.";

		// Token: 0x04002BE1 RID: 11233
		private const string EXCEPTION_PARAMETER_INDEX_OUT_OF_RANGE = "Index out of range when trying to access a parameter from an event data.";

		// Token: 0x04002BE2 RID: 11234
		private readonly string mName = string.Empty;

		// Token: 0x04002BE3 RID: 11235
		private readonly object[] mParameters;
	}
}
