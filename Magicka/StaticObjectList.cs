using System;

namespace Magicka
{
	// Token: 0x020001A0 RID: 416
	public class StaticObjectList<T> : StaticList<T> where T : class
	{
		// Token: 0x06000C61 RID: 3169 RVA: 0x0004A405 File Offset: 0x00048605
		public StaticObjectList(int iCapacity) : base(iCapacity)
		{
		}

		// Token: 0x06000C62 RID: 3170 RVA: 0x0004A410 File Offset: 0x00048610
		public override int IndexOf(T iItem)
		{
			for (int i = 0; i < this.mCount; i++)
			{
				if (this.mObjects[i].Equals(iItem))
				{
					return i;
				}
			}
			return -1;
		}
	}
}
