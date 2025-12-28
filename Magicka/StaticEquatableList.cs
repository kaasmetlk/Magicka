using System;

namespace Magicka
{
	// Token: 0x020001A1 RID: 417
	public class StaticEquatableList<T> : StaticList<T> where T : struct, IEquatable<T>
	{
		// Token: 0x06000C63 RID: 3171 RVA: 0x0004A452 File Offset: 0x00048652
		public StaticEquatableList(int iCapacity) : base(iCapacity)
		{
		}

		// Token: 0x06000C64 RID: 3172 RVA: 0x0004A45C File Offset: 0x0004865C
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
