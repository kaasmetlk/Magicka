// Decompiled with JetBrains decompiler
// Type: Magicka.StaticWeakList`1
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Magicka;

public class StaticWeakList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable where T : class
{
  protected WeakReference[] mObjects;
  protected int mCount;

  public StaticWeakList(int iCapacity)
  {
    this.mObjects = new WeakReference[iCapacity];
    for (int index = 0; index < iCapacity; ++index)
      this.mObjects[index] = new WeakReference((object) null);
  }

  public virtual void Add(T iItem)
  {
    if (this.mCount >= this.mObjects.Length)
      throw new StaticWeakListNeedsToExpandException(this.mCount, iItem.ToString());
    this.mObjects[this.mCount].Target = (object) iItem;
    ++this.mCount;
  }

  public virtual void Clear() => this.mCount = 0;

  public virtual bool Contains(T iItem) => this.IndexOf(iItem) >= 0;

  public virtual void CopyTo(T[] iArray, int iArrayIndex)
  {
    for (int index = 0; index < this.mCount; ++index)
      iArray[iArrayIndex + index] = this.mObjects[index].Target as T;
  }

  public virtual int Count => this.mCount;

  public virtual int Capacity => this.mObjects.Length;

  public virtual bool IsReadOnly => false;

  public virtual bool Remove(T iItem)
  {
    int iIndex = this.IndexOf(iItem);
    if (iIndex < 0)
      return false;
    this.RemoveAt(iIndex);
    return true;
  }

  public virtual IEnumerator<T> GetEnumerator()
  {
    return (IEnumerator<T>) new StaticWeakList<T>.StaticListEnumerator<T>(this.mObjects, this.mCount);
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return (IEnumerator) new StaticWeakList<T>.StaticListEnumerator<T>(this.mObjects, this.mCount);
  }

  public int IndexOf(T iItem)
  {
    for (int index = 0; index < this.mCount; ++index)
    {
      object target = this.mObjects[index].Target;
      if (target != null && target.Equals((object) iItem))
        return index;
    }
    return -1;
  }

  public virtual void Insert(int iIndex, T iItem)
  {
    if (iIndex < 0 || iIndex > this.mCount)
      throw new IndexOutOfRangeException();
    for (int mCount = this.mCount; mCount > iIndex; --mCount)
      this.mObjects[mCount].Target = this.mObjects[mCount - 1].Target;
    this.mObjects[iIndex].Target = (object) iItem;
    ++this.mCount;
  }

  public virtual void RemoveAt(int iIndex)
  {
    if (iIndex < 0 || iIndex >= this.mCount)
      throw new IndexOutOfRangeException();
    for (int index = iIndex + 1; index < this.mCount; ++index)
      this.mObjects[index - 1].Target = this.mObjects[index].Target;
    --this.mCount;
  }

  public virtual T this[int iIndex]
  {
    get
    {
      if (iIndex < 0 || iIndex >= this.mCount)
        throw new IndexOutOfRangeException();
      return this.mObjects[iIndex].Target as T;
    }
    set
    {
      if (iIndex < 0 || iIndex >= this.mCount)
        throw new IndexOutOfRangeException();
      this.mObjects[iIndex].Target = (object) value;
    }
  }

  internal void Expand() => this.Expand(this.mObjects.Length);

  internal void Expand(int ammount)
  {
    List<WeakReference> weakReferenceList = new List<WeakReference>();
    lock (this.mObjects)
    {
      weakReferenceList.AddRange((IEnumerable<WeakReference>) this.mObjects);
      for (int index = 0; index < ammount; ++index)
        weakReferenceList.Add(new WeakReference((object) null));
      this.mObjects = weakReferenceList.ToArray();
    }
  }

  public class StaticListEnumerator<U> : IEnumerator<U>, IDisposable, IEnumerator where U : class
  {
    private WeakReference[] mObjects;
    private int mCount;
    private int mCurrentIndex = -1;

    public StaticListEnumerator(WeakReference[] iObjects, int iCount)
    {
      this.mObjects = iObjects;
      this.mCount = iCount;
    }

    public U Current => this.mObjects[this.mCurrentIndex].Target as U;

    public void Dispose() => this.mObjects = (WeakReference[]) null;

    object IEnumerator.Current => this.mObjects[this.mCurrentIndex].Target;

    public bool MoveNext()
    {
      ++this.mCurrentIndex;
      return this.mCurrentIndex < this.mCount;
    }

    public void Reset() => this.mCurrentIndex = -1;
  }
}
