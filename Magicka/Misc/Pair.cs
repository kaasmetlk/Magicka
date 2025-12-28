using System;

namespace Magicka.Misc
{
	// Token: 0x02000550 RID: 1360
	public class Pair<T, U>
	{
		// Token: 0x06002868 RID: 10344 RVA: 0x0013C929 File Offset: 0x0013AB29
		public Pair()
		{
		}

		// Token: 0x06002869 RID: 10345 RVA: 0x0013C931 File Offset: 0x0013AB31
		public Pair(T first, U second)
		{
			this.First = first;
			this.Second = second;
		}

		// Token: 0x17000976 RID: 2422
		// (get) Token: 0x0600286A RID: 10346 RVA: 0x0013C947 File Offset: 0x0013AB47
		// (set) Token: 0x0600286B RID: 10347 RVA: 0x0013C94F File Offset: 0x0013AB4F
		public T First { get; set; }

		// Token: 0x17000977 RID: 2423
		// (get) Token: 0x0600286C RID: 10348 RVA: 0x0013C958 File Offset: 0x0013AB58
		// (set) Token: 0x0600286D RID: 10349 RVA: 0x0013C960 File Offset: 0x0013AB60
		public U Second { get; set; }
	}
}
