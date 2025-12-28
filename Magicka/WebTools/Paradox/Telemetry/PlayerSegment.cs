using System;
using System.Collections.Generic;

namespace Magicka.WebTools.Paradox.Telemetry
{
	// Token: 0x02000002 RID: 2
	public class PlayerSegment
	{
		// Token: 0x17000001 RID: 1
		public PlayerSegment.Section this[int iIndex]
		{
			get
			{
				if (this.mSections == null)
				{
					return PlayerSegment.Section.NotApplicable;
				}
				if (iIndex >= this.mSections.Length)
				{
					throw new IndexOutOfRangeException(string.Format("Index out of range. Provided index {0} with a maximum allowed of {1}.", iIndex, this.mSections.Length - 1));
				}
				return this.mSections[iIndex];
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000002 RID: 2 RVA: 0x0000209F File Offset: 0x0000029F
		// (set) Token: 0x06000003 RID: 3 RVA: 0x000020B3 File Offset: 0x000002B3
		public int Length
		{
			get
			{
				if (this.mSections == null)
				{
					return 0;
				}
				return this.mSections.Length;
			}
			set
			{
				Array.Resize<PlayerSegment.Section>(ref this.mSections, value);
			}
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000020C1 File Offset: 0x000002C1
		private PlayerSegment()
		{
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000020CC File Offset: 0x000002CC
		public PlayerSegment(int iSize)
		{
			this.mSections = new PlayerSegment.Section[iSize];
			for (int i = 0; i < iSize; i++)
			{
				this.mSections[i] = PlayerSegment.Section.NotApplicable;
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002100 File Offset: 0x00000300
		public PlayerSegment(int iSize, PlayerSegment.Section iDefault)
		{
			this.mSections = new PlayerSegment.Section[iSize];
			for (int i = 0; i < iSize; i++)
			{
				this.mSections[i] = iDefault;
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002134 File Offset: 0x00000334
		public PlayerSegment(params PlayerSegment.Section[] iSections)
		{
			if (iSections == null)
			{
				throw new NullReferenceException("Cannot pass a null to a PlayerSegment.");
			}
			this.mSections = iSections;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002154 File Offset: 0x00000354
		public PlayerSegment(params int[] iSections)
		{
			if (iSections == null)
			{
				throw new NullReferenceException("Cannot pass a null to a PlayerSegment.");
			}
			this.mSections = new PlayerSegment.Section[iSections.Length];
			for (int i = 0; i < iSections.Length; i++)
			{
				int num = iSections[i];
				if (num < -1 || num > 1)
				{
					throw new Exception(string.Format("The section at index {0} have an unexpected value {1}. Expecting only -1, 0 or 1.", i, num));
				}
				this.mSections[i] = (PlayerSegment.Section)num;
			}
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000021C2 File Offset: 0x000003C2
		public string ToSegmentString()
		{
			return PlayerSegment.ToSegmentString(this.mSections);
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000021D0 File Offset: 0x000003D0
		public static string ToSegmentString(params PlayerSegment.Section[] iSections)
		{
			char[] array = new char[iSections.Length];
			for (int i = 0; i < iSections.Length; i++)
			{
				array[i] = PlayerSegment.sConvertionDictionary[iSections[i]];
			}
			return new string(array);
		}

		// Token: 0x0600000B RID: 11 RVA: 0x0000220C File Offset: 0x0000040C
		public static bool IsSegmentString(string iString)
		{
			for (int i = 0; i < iString.Length; i++)
			{
				if (!"NTF".Contains(iString[i].ToString()))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x04000001 RID: 1
		private const string EXCEPTION_INDEX_OUT_OF_RANGE = "Index out of range. Provided index {0} with a maximum allowed of {1}.";

		// Token: 0x04000002 RID: 2
		private const string EXCEPTION_UNEXPECTED_SECTION_VALUE = "The section at index {0} have an unexpected value {1}. Expecting only -1, 0 or 1.";

		// Token: 0x04000003 RID: 3
		private const string EXCEPTION_NULL_SECTIONS_ARRAY = "Cannot pass a null to a PlayerSegment.";

		// Token: 0x04000004 RID: 4
		private const char NOT_APPLICABLE_CHAR = 'N';

		// Token: 0x04000005 RID: 5
		private const char TRUE_CHAR = 'T';

		// Token: 0x04000006 RID: 6
		private const char FALSE_CHAR = 'F';

		// Token: 0x04000007 RID: 7
		private const string SEGMENT_STRING_ALLOWED_CHAR = "NTF";

		// Token: 0x04000008 RID: 8
		private static readonly Dictionary<PlayerSegment.Section, char> sConvertionDictionary = new Dictionary<PlayerSegment.Section, char>
		{
			{
				PlayerSegment.Section.NotApplicable,
				'N'
			},
			{
				PlayerSegment.Section.True,
				'T'
			},
			{
				PlayerSegment.Section.False,
				'F'
			}
		};

		// Token: 0x04000009 RID: 9
		private PlayerSegment.Section[] mSections;

		// Token: 0x02000003 RID: 3
		public enum Section
		{
			// Token: 0x0400000B RID: 11
			NotApplicable = -1,
			// Token: 0x0400000C RID: 12
			False,
			// Token: 0x0400000D RID: 13
			True
		}
	}
}
