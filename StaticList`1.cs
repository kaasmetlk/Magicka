// Decompiled with JetBrains decompiler
// Type: Magicka.StaticList`1
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Magicka;

public abstract class StaticList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
  protected T[] mObjects;
  protected int mCount;

  public StaticList(int iCapacity) => this.mObjects = new T[iCapacity];

  public virtual void Add(T iItem)
  {
    lock (this.mObjects)
    {
      this.mObjects[this.mCount] = iItem;
      ++this.mCount;
    }
  }

  public virtual void Clear()
  {
    for (int index = 0; index < this.mCount; ++index)
      this.mObjects[index] = default (T);
    this.mCount = 0;
  }

  public virtual bool Contains(T iItem) => this.IndexOf(iItem) >= 0;

  public virtual void CopyTo(T[] iArray, int iArrayIndex)
  {
    for (int index = 0; index < this.mCount; ++index)
      iArray[iArrayIndex + index] = this.mObjects[index];
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
    return (IEnumerator<T>) new StaticList<T>.StaticListEnumerator<T>(this.mObjects, this.mCount);
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return (IEnumerator) new StaticList<T>.StaticListEnumerator<T>(this.mObjects, this.mCount);
  }

  public abstract int IndexOf(T iItem);

  public virtual void Insert(int iIndex, T iItem)
  {
    lock (this.mObjects)
    {
      if (iIndex < 0 || iIndex > this.mCount)
        throw new IndexOutOfRangeException();
      for (int mCount = this.mCount; mCount > iIndex; --mCount)
        this.mObjects[mCount] = this.mObjects[mCount - 1];
      this.mObjects[iIndex] = iItem;
      ++this.mCount;
    }
  }

  public virtual void RemoveAt(int iIndex)
  {
    if (iIndex < 0 || iIndex >= this.mCount)
      throw new IndexOutOfRangeException();
    for (int index = iIndex + 1; index < this.mCount; ++index)
      this.mObjects[index - 1] = this.mObjects[index];
    --this.mCount;
    this.mObjects[this.mCount] = default (T);
  }

  public virtual T this[int iIndex]
  {
    get
    {
      return iIndex >= 0 && iIndex < this.mCount ? this.mObjects[iIndex] : throw new IndexOutOfRangeException();
    }
    set
    {
      if (iIndex < 0 || iIndex >= this.mCount)
        throw new IndexOutOfRangeException();
      this.mObjects[iIndex] = value;
    }
  }

  public struct StaticListEnumerator<U>(U[] iObjects, int iCount) : 
    IEnumerator<U>,
    IDisposable,
    IEnumerator
  {
    private U[] mObjects = iObjects;
    private int mCount = iCount;
    private int mCurrentIndex = -1;

    public U Current => this.mObjects[this.mCurrentIndex];

    public void Dispose() => this.mObjects = (U[]) null;

    object IEnumerator.Current => (object) this.mObjects[this.mCurrentIndex];

    public bool MoveNext()
    {
      ++this.mCurrentIndex;
      return this.mCurrentIndex < this.mCount;
    }

    public void Reset() => this.mCurrentIndex = -1;
  }
}
