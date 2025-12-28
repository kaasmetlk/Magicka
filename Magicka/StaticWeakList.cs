using System;
using System.Collections;
using System.Collections.Generic;

namespace Magicka
{
	// Token: 0x020003E0 RID: 992
	public class StaticWeakList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable where T : class
	{
		// Token: 0x06001E58 RID: 7768 RVA: 0x000D4D20 File Offset: 0x000D2F20
		public StaticWeakList(int iCapacity)
		{
			this.mObjects = new WeakReference[iCapacity];
			for (int i = 0; i < iCapacity; i++)
			{
				this.mObjects[i] = new WeakReference(null);
			}
		}

		// Token: 0x06001E59 RID: 7769 RVA: 0x000D4D5C File Offset: 0x000D2F5C
		public virtual void Add(T iItem)
		{
			if (this.mCount < this.mObjects.Length)
			{
				this.mObjects[this.mCount].Target = iItem;
				this.mCount++;
				return;
			}
			throw new StaticWeakListNeedsToExpandException(this.mCount, iItem.ToString());
		}

		// Token: 0x06001E5A RID: 7770 RVA: 0x000D4DB8 File Offset: 0x000D2FB8
		public virtual void Clear()
		{
			this.mCount = 0;
		}

		// Token: 0x06001E5B RID: 7771 RVA: 0x000D4DC1 File Offset: 0x000D2FC1
		public virtual bool Contains(T iItem)
		{
			return this.IndexOf(iItem) >= 0;
		}

		// Token: 0x06001E5C RID: 7772 RVA: 0x000D4DD0 File Offset: 0x000D2FD0
		public virtual void CopyTo(T[] iArray, int iArrayIndex)
		{
			for (int i = 0; i < this.mCount; i++)
			{
				iArray[iArrayIndex + i] = (this.mObjects[i].Target as T);
			}
		}

		// Token: 0x17000767 RID: 1895
		// (get) Token: 0x06001E5D RID: 7773 RVA: 0x000D4E0E File Offset: 0x000D300E
		public virtual int Count
		{
			get
			{
				return this.mCount;
			}
		}

		// Token: 0x17000768 RID: 1896
		// (get) Token: 0x06001E5E RID: 7774 RVA: 0x000D4E16 File Offset: 0x000D3016
		public virtual int Capacity
		{
			get
			{
				return this.mObjects.Length;
			}
		}

		// Token: 0x17000769 RID: 1897
		// (get) Token: 0x06001E5F RID: 7775 RVA: 0x000D4E20 File Offset: 0x000D3020
		public virtual bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06001E60 RID: 7776 RVA: 0x000D4E24 File Offset: 0x000D3024
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

		// Token: 0x06001E61 RID: 7777 RVA: 0x000D4E47 File Offset: 0x000D3047
		public virtual IEnumerator<T> GetEnumerator()
		{
			return new StaticWeakList<T>.StaticListEnumerator<T>(this.mObjects, this.mCount);
		}

		// Token: 0x06001E62 RID: 7778 RVA: 0x000D4E5A File Offset: 0x000D305A
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new StaticWeakList<T>.StaticListEnumerator<T>(this.mObjects, this.mCount);
		}

		// Token: 0x06001E63 RID: 7779 RVA: 0x000D4E70 File Offset: 0x000D3070
		public int IndexOf(T iItem)
		{
			for (int i = 0; i < this.mCount; i++)
			{
				object target = this.mObjects[i].Target;
				if (target != null && target.Equals(iItem))
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06001E64 RID: 7780 RVA: 0x000D4EB0 File Offset: 0x000D30B0
		public virtual void Insert(int iIndex, T iItem)
		{
			if (iIndex < 0 || iIndex > this.mCount)
			{
				throw new IndexOutOfRangeException();
			}
			for (int i = this.mCount; i > iIndex; i--)
			{
				this.mObjects[i].Target = this.mObjects[i - 1].Target;
			}
			this.mObjects[iIndex].Target = iItem;
			this.mCount++;
		}

		// Token: 0x06001E65 RID: 7781 RVA: 0x000D4F20 File Offset: 0x000D3120
		public virtual void RemoveAt(int iIndex)
		{
			if (iIndex < 0 || iIndex >= this.mCount)
			{
				throw new IndexOutOfRangeException();
			}
			for (int i = iIndex + 1; i < this.mCount; i++)
			{
				this.mObjects[i - 1].Target = this.mObjects[i].Target;
			}
			this.mCount--;
		}

		// Token: 0x1700076A RID: 1898
		public virtual T this[int iIndex]
		{
			get
			{
				if (iIndex < 0 || iIndex >= this.mCount)
				{
					throw new IndexOutOfRangeException();
				}
				return this.mObjects[iIndex].Target as T;
			}
			set
			{
				if (iIndex < 0 || iIndex >= this.mCount)
				{
					throw new IndexOutOfRangeException();
				}
				this.mObjects[iIndex].Target = value;
			}
		}

		// Token: 0x06001E68 RID: 7784 RVA: 0x000D4FD1 File Offset: 0x000D31D1
		internal void Expand()
		{
			this.Expand(this.mObjects.Length);
		}

		// Token: 0x06001E69 RID: 7785 RVA: 0x000D4FE4 File Offset: 0x000D31E4
		internal void Expand(int ammount)
		{
			List<WeakReference> list = new List<WeakReference>();
			lock (this.mObjects)
			{
				list.AddRange(this.mObjects);
				for (int i = 0; i < ammount; i++)
				{
					list.Add(new WeakReference(null));
				}
				this.mObjects = list.ToArray();
			}
		}

		// Token: 0x040020C9 RID: 8393
		protected WeakReference[] mObjects;

		// Token: 0x040020CA RID: 8394
		protected int mCount;

		// Token: 0x020003E1 RID: 993
		public class StaticListEnumerator<U> : IEnumerator<U>, IDisposable, IEnumerator where U : class
		{
			// Token: 0x06001E6A RID: 7786 RVA: 0x000D5050 File Offset: 0x000D3250
			public StaticListEnumerator(WeakReference[] iObjects, int iCount)
			{
				this.mObjects = iObjects;
				this.mCount = iCount;
			}

			// Token: 0x1700076B RID: 1899
			// (get) Token: 0x06001E6B RID: 7787 RVA: 0x000D506D File Offset: 0x000D326D
			public U Current
			{
				get
				{
					return this.mObjects[this.mCurrentIndex].Target as U;
				}
			}

			// Token: 0x06001E6C RID: 7788 RVA: 0x000D508B File Offset: 0x000D328B
			public void Dispose()
			{
				this.mObjects = null;
			}

			// Token: 0x1700076C RID: 1900
			// (get) Token: 0x06001E6D RID: 7789 RVA: 0x000D5094 File Offset: 0x000D3294
			object IEnumerator.Current
			{
				get
				{
					return this.mObjects[this.mCurrentIndex].Target;
				}
			}

			// Token: 0x06001E6E RID: 7790 RVA: 0x000D50A8 File Offset: 0x000D32A8
			public bool MoveNext()
			{
				this.mCurrentIndex++;
				return this.mCurrentIndex < this.mCount;
			}

			// Token: 0x06001E6F RID: 7791 RVA: 0x000D50C9 File Offset: 0x000D32C9
			public void Reset()
			{
				this.mCurrentIndex = -1;
			}

			// Token: 0x040020CB RID: 8395
			private WeakReference[] mObjects;

			// Token: 0x040020CC RID: 8396
			private int mCount;

			// Token: 0x040020CD RID: 8397
			private int mCurrentIndex = -1;
		}
	}
}
