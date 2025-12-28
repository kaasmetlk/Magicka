using System;
using System.Collections;
using System.Collections.Generic;

namespace Magicka
{
	// Token: 0x0200019E RID: 414
	public abstract class StaticList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
	{
		// Token: 0x06000C4B RID: 3147 RVA: 0x0004A0FA File Offset: 0x000482FA
		public StaticList(int iCapacity)
		{
			this.mObjects = new T[iCapacity];
		}

		// Token: 0x06000C4C RID: 3148 RVA: 0x0004A110 File Offset: 0x00048310
		public virtual void Add(T iItem)
		{
			lock (this.mObjects)
			{
				this.mObjects[this.mCount] = iItem;
				this.mCount++;
			}
		}

		// Token: 0x06000C4D RID: 3149 RVA: 0x0004A164 File Offset: 0x00048364
		public virtual void Clear()
		{
			for (int i = 0; i < this.mCount; i++)
			{
				this.mObjects[i] = default(T);
			}
			this.mCount = 0;
		}

		// Token: 0x06000C4E RID: 3150 RVA: 0x0004A19E File Offset: 0x0004839E
		public virtual bool Contains(T iItem)
		{
			return this.IndexOf(iItem) >= 0;
		}

		// Token: 0x06000C4F RID: 3151 RVA: 0x0004A1B0 File Offset: 0x000483B0
		public virtual void CopyTo(T[] iArray, int iArrayIndex)
		{
			for (int i = 0; i < this.mCount; i++)
			{
				iArray[iArrayIndex + i] = this.mObjects[i];
			}
		}

		// Token: 0x170002DC RID: 732
		// (get) Token: 0x06000C50 RID: 3152 RVA: 0x0004A1E3 File Offset: 0x000483E3
		public virtual int Count
		{
			get
			{
				return this.mCount;
			}
		}

		// Token: 0x170002DD RID: 733
		// (get) Token: 0x06000C51 RID: 3153 RVA: 0x0004A1EB File Offset: 0x000483EB
		public virtual int Capacity
		{
			get
			{
				return this.mObjects.Length;
			}
		}

		// Token: 0x170002DE RID: 734
		// (get) Token: 0x06000C52 RID: 3154 RVA: 0x0004A1F5 File Offset: 0x000483F5
		public virtual bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06000C53 RID: 3155 RVA: 0x0004A1F8 File Offset: 0x000483F8
		public virtual bool Remove(T iItem)
		{
			int num = this.IndexOf(iItem);
			if (num < 0)
			{
				return false;
			}
			this.RemoveAt(num);
			return true;
		}

		// Token: 0x06000C54 RID: 3156 RVA: 0x0004A21B File Offset: 0x0004841B
		public virtual IEnumerator<T> GetEnumerator()
		{
			return new StaticList<T>.StaticListEnumerator<T>(this.mObjects, this.mCount);
		}

		// Token: 0x06000C55 RID: 3157 RVA: 0x0004A233 File Offset: 0x00048433
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new StaticList<T>.StaticListEnumerator<T>(this.mObjects, this.mCount);
		}

		// Token: 0x06000C56 RID: 3158
		public abstract int IndexOf(T iItem);

		// Token: 0x06000C57 RID: 3159 RVA: 0x0004A24C File Offset: 0x0004844C
		public virtual void Insert(int iIndex, T iItem)
		{
			lock (this.mObjects)
			{
				if (iIndex < 0 || iIndex > this.mCount)
				{
					throw new IndexOutOfRangeException();
				}
				for (int i = this.mCount; i > iIndex; i--)
				{
					this.mObjects[i] = this.mObjects[i - 1];
				}
				this.mObjects[iIndex] = iItem;
				this.mCount++;
			}
		}

		// Token: 0x06000C58 RID: 3160 RVA: 0x0004A2D8 File Offset: 0x000484D8
		public virtual void RemoveAt(int iIndex)
		{
			if (iIndex < 0 || iIndex >= this.mCount)
			{
				throw new IndexOutOfRangeException();
			}
			for (int i = iIndex + 1; i < this.mCount; i++)
			{
				this.mObjects[i - 1] = this.mObjects[i];
			}
			this.mCount--;
			this.mObjects[this.mCount] = default(T);
		}

		// Token: 0x170002DF RID: 735
		public virtual T this[int iIndex]
		{
			get
			{
				if (iIndex < 0 || iIndex >= this.mCount)
				{
					throw new IndexOutOfRangeException();
				}
				return this.mObjects[iIndex];
			}
			set
			{
				if (iIndex < 0 || iIndex >= this.mCount)
				{
					throw new IndexOutOfRangeException();
				}
				this.mObjects[iIndex] = value;
			}
		}

		// Token: 0x04000B65 RID: 2917
		protected T[] mObjects;

		// Token: 0x04000B66 RID: 2918
		protected int mCount;

		// Token: 0x0200019F RID: 415
		public struct StaticListEnumerator<U> : IEnumerator<U>, IDisposable, IEnumerator
		{
			// Token: 0x06000C5B RID: 3163 RVA: 0x0004A390 File Offset: 0x00048590
			public StaticListEnumerator(U[] iObjects, int iCount)
			{
				this.mObjects = iObjects;
				this.mCount = iCount;
				this.mCurrentIndex = -1;
			}

			// Token: 0x170002E0 RID: 736
			// (get) Token: 0x06000C5C RID: 3164 RVA: 0x0004A3A7 File Offset: 0x000485A7
			public U Current
			{
				get
				{
					return this.mObjects[this.mCurrentIndex];
				}
			}

			// Token: 0x06000C5D RID: 3165 RVA: 0x0004A3BA File Offset: 0x000485BA
			public void Dispose()
			{
				this.mObjects = null;
			}

			// Token: 0x170002E1 RID: 737
			// (get) Token: 0x06000C5E RID: 3166 RVA: 0x0004A3C3 File Offset: 0x000485C3
			object IEnumerator.Current
			{
				get
				{
					return this.mObjects[this.mCurrentIndex];
				}
			}

			// Token: 0x06000C5F RID: 3167 RVA: 0x0004A3DB File Offset: 0x000485DB
			public bool MoveNext()
			{
				this.mCurrentIndex++;
				return this.mCurrentIndex < this.mCount;
			}

			// Token: 0x06000C60 RID: 3168 RVA: 0x0004A3FC File Offset: 0x000485FC
			public void Reset()
			{
				this.mCurrentIndex = -1;
			}

			// Token: 0x04000B67 RID: 2919
			private U[] mObjects;

			// Token: 0x04000B68 RID: 2920
			private int mCount;

			// Token: 0x04000B69 RID: 2921
			private int mCurrentIndex;
		}
	}
}
