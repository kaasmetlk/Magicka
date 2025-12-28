using System;

namespace Magicka
{
	// Token: 0x020003DF RID: 991
	public class StaticWeakListNeedsToExpandException : Exception
	{
		// Token: 0x06001E57 RID: 7767 RVA: 0x000D4D07 File Offset: 0x000D2F07
		public StaticWeakListNeedsToExpandException(int currentCount, string attemptedAdd) : base(string.Format("StaticWeakList with {0} elements is full ! Failed to add {1}, need to expand.", currentCount, attemptedAdd))
		{
		}
	}
}
