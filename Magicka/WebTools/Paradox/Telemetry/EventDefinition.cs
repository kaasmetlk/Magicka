using System;

namespace Magicka.WebTools.Paradox.Telemetry
{
	// Token: 0x02000320 RID: 800
	public class EventDefinition
	{
		// Token: 0x17000621 RID: 1569
		// (get) Token: 0x06001889 RID: 6281 RVA: 0x000A2973 File Offset: 0x000A0B73
		public int Count
		{
			get
			{
				return this.mParameters.Length;
			}
		}

		// Token: 0x0600188A RID: 6282 RVA: 0x000A297D File Offset: 0x000A0B7D
		public EventDefinition(params EventDefinition.Parameter[] iParameters)
		{
			if (iParameters.Length > 9)
			{
				throw new Exception(string.Format("Too many arguments ({0}) were passed to a definition with a maximum allowed of {1}.", iParameters.Length, 9));
			}
			this.mParameters = iParameters;
		}

		// Token: 0x0600188B RID: 6283 RVA: 0x000A29B4 File Offset: 0x000A0BB4
		public EventDefinition(params object[] iArgs)
		{
			int num = iArgs.Length;
			if (num % 2 != 0)
			{
				throw new Exception("The provided number of arguments are odd. It should be even. Are you missing a parameter name?");
			}
			if (num > 18)
			{
				throw new Exception(string.Format("Too many arguments ({0}) were passed to a definition with a maximum allowed of {1}.", num, 18));
			}
			int num2 = num / 2;
			this.mParameters = new EventDefinition.Parameter[num2];
			if (num2 > 0)
			{
				for (int i = 0; i < num2; i++)
				{
					int num3 = i * 2;
					int num4 = i * 2 + 1;
					object obj = iArgs[num3];
					object obj2 = iArgs[num4];
					Type type = obj.GetType();
					Type type2 = obj2.GetType();
					if (type != typeof(string))
					{
						throw new Exception(string.Format("The argument at position {0} is expected to be a parameter name of type String, received {1} instead.", num3, type.ToString()));
					}
					if (type2 != typeof(EventParameter.Type))
					{
						throw new Exception(string.Format("The argument at position {0} is expected to be a parameter name of type EventParameter.Type, received {1} instead.", num4, type2.ToString()));
					}
					this.mParameters[i] = new EventDefinition.Parameter((string)obj, (EventParameter.Type)obj2);
				}
			}
		}

		// Token: 0x0600188C RID: 6284 RVA: 0x000A2ACC File Offset: 0x000A0CCC
		public string GetParameterName(int iParameterIndex)
		{
			if (iParameterIndex >= this.mParameters.Length)
			{
				throw new IndexOutOfRangeException(string.Format("Index out of range. Provided index {0} with a maximum allowed of {1}.", iParameterIndex, this.mParameters.Length - 1));
			}
			if (this.mParameters.Length > 0)
			{
				return this.mParameters[iParameterIndex].Name;
			}
			return string.Empty;
		}

		// Token: 0x0600188D RID: 6285 RVA: 0x000A2B30 File Offset: 0x000A0D30
		public EventParameter.Type GetParameterType(int iParameterIndex)
		{
			if (iParameterIndex < 0 || iParameterIndex >= this.mParameters.Length)
			{
				throw new IndexOutOfRangeException(string.Format("Index out of range. Provided index {0} with a maximum allowed of {1}.", iParameterIndex, this.mParameters.Length - 1));
			}
			return this.mParameters[iParameterIndex].Type;
		}

		// Token: 0x0600188E RID: 6286 RVA: 0x000A2B88 File Offset: 0x000A0D88
		public Type GetParameterSystemType(int iParameterIndex)
		{
			if (iParameterIndex < 0 || iParameterIndex >= this.mParameters.Length)
			{
				throw new IndexOutOfRangeException(string.Format("Index out of range. Provided index {0} with a maximum allowed of {1}.", iParameterIndex, this.mParameters.Length - 1));
			}
			return EventParameter.ToSystemType(this.mParameters[iParameterIndex].Type);
		}

		// Token: 0x0600188F RID: 6287 RVA: 0x000A2BE4 File Offset: 0x000A0DE4
		public bool IsValid(object[] iValues)
		{
			if (this.mParameters == null)
			{
				return iValues.Length == 0;
			}
			if (iValues.Length != this.mParameters.Length)
			{
				return false;
			}
			for (int i = 0; i < iValues.Length; i++)
			{
				if (!this.IsValid(i, iValues[i]))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06001890 RID: 6288 RVA: 0x000A2C2C File Offset: 0x000A0E2C
		public bool IsValid(int iParameterIndex, EventParameter.Type iParameterType)
		{
			if (this.mParameters.Length == 0)
			{
				return false;
			}
			if (iParameterIndex < this.mParameters.Length)
			{
				return this.mParameters[iParameterIndex].Type == iParameterType;
			}
			throw new IndexOutOfRangeException(string.Format("Index out of range. Provided index {0} with a maximum allowed of {1}.", iParameterIndex, this.mParameters.Length - 1));
		}

		// Token: 0x06001891 RID: 6289 RVA: 0x000A2C90 File Offset: 0x000A0E90
		public bool IsValid(int iParameterIndex, object iParamObject)
		{
			if (this.mParameters.Length == 0)
			{
				return false;
			}
			if (iParameterIndex < this.mParameters.Length)
			{
				return EventParameter.MatchType(iParamObject, this.mParameters[iParameterIndex].Type);
			}
			throw new IndexOutOfRangeException(string.Format("Index out of range. Provided index {0} with a maximum allowed of {1}.", iParameterIndex, this.mParameters.Length - 1));
		}

		// Token: 0x06001892 RID: 6290 RVA: 0x000A2D14 File Offset: 0x000A0F14
		public bool IsValid(string iParameterName, EventParameter.Type iParameterType)
		{
			int num = Array.FindIndex<EventDefinition.Parameter>(this.mParameters, (EventDefinition.Parameter row) => row.Name == iParameterName);
			if (num >= 0)
			{
				return this.IsValid(num, iParameterType);
			}
			throw new Exception(string.Format("Couldn't find a parameter with name {0}.", iParameterName));
		}

		// Token: 0x06001893 RID: 6291 RVA: 0x000A2D84 File Offset: 0x000A0F84
		public bool IsValid(string iParameterName, object iParameterObject)
		{
			int num = Array.FindIndex<EventDefinition.Parameter>(this.mParameters, (EventDefinition.Parameter row) => row.Name == iParameterName);
			if (num >= 0)
			{
				return this.IsValid(num, iParameterObject);
			}
			throw new Exception(string.Format("Couldn't find a parameter with name {0}.", iParameterName));
		}

		// Token: 0x04001A39 RID: 6713
		public const int MAX_EVENT_PARAM = 9;

		// Token: 0x04001A3A RID: 6714
		private const string EXCEPTION_TOO_MANY_PARAMETERS = "Too many parameters ({0}) were passed to a definition with a maximum allowed of {1}.";

		// Token: 0x04001A3B RID: 6715
		private const string EXCEPTION_TOO_MANY_ARGUMENTS = "Too many arguments ({0}) were passed to a definition with a maximum allowed of {1}.";

		// Token: 0x04001A3C RID: 6716
		private const string EXCEPTION_ARGUMENT_COUNT_ODD = "The provided number of arguments are odd. It should be even. Are you missing a parameter name?";

		// Token: 0x04001A3D RID: 6717
		private const string EXCEPTION_EXPECTED_TYPE_STRING = "The argument at position {0} is expected to be a parameter name of type String, received {1} instead.";

		// Token: 0x04001A3E RID: 6718
		private const string EXCEPTION_EXPECTED_TYPE_TYPE = "The argument at position {0} is expected to be a parameter name of type EventParameter.Type, received {1} instead.";

		// Token: 0x04001A3F RID: 6719
		private const string EXCEPTION_INDEX_OUT_OF_RANGE = "Index out of range. Provided index {0} with a maximum allowed of {1}.";

		// Token: 0x04001A40 RID: 6720
		private const string EXCEPTION_PARAMETER_NOT_FOUND = "Couldn't find a parameter with name {0}.";

		// Token: 0x04001A41 RID: 6721
		private readonly EventDefinition.Parameter[] mParameters;

		// Token: 0x02000321 RID: 801
		public struct Parameter
		{
			// Token: 0x06001894 RID: 6292 RVA: 0x000A2DD8 File Offset: 0x000A0FD8
			public Parameter(string iName, EventParameter.Type iType)
			{
				this.Name = iName;
				this.Type = iType;
			}

			// Token: 0x04001A42 RID: 6722
			public readonly string Name;

			// Token: 0x04001A43 RID: 6723
			public readonly EventParameter.Type Type;
		}
	}
}
