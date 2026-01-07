// Decompiled with JetBrains decompiler
// Type: PolygonHead.Heap`1
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using System;

#nullable disable
namespace PolygonHead;

public class Heap<T> : IHeap<T> where T : struct, IComparable<T>
{
  protected T[] mHeap;
  protected int mLastIndex;

  public bool IsEmpty => this.mLastIndex == 0;

  public Heap(int iSize) => this.mHeap = new T[iSize + 1];

  protected void IncreasSize()
  {
    T[] objArray = new T[(this.mHeap.Length - 1) * 2 + 1];
    for (int index = 0; index < this.mHeap.Length; ++index)
      objArray[index] = this.mHeap[index];
    this.mHeap = objArray;
  }

  public void Push(T iValue)
  {
    ++this.mLastIndex;
    this.mHeap[this.mLastIndex] = iValue;
    int index1 = this.mLastIndex;
    int index2 = index1 / 2;
    for (T other = this.mHeap[index2]; index1 > 1 && iValue.CompareTo(other) < 0; other = this.mHeap[index2])
    {
      this.mHeap[index1] = other;
      index1 = index2;
      index2 = index1 / 2;
    }
    this.mHeap[index1] = iValue;
  }

  public T Pop()
  {
    T obj1 = this.mHeap[1];
    T other1 = this.mHeap[this.mLastIndex];
    --this.mLastIndex;
    int index1 = 1;
    int index2 = 2;
    int index3 = 3;
    while (true)
    {
      int index4 = 0;
      if (index2 <= this.mLastIndex)
      {
        T other2 = this.mHeap[index2];
        if (other2.CompareTo(other1) < 0)
          index4 = index2;
        T obj2 = this.mHeap[index3];
        if (index3 <= this.mLastIndex && obj2.CompareTo(other1) < 0 && obj2.CompareTo(other2) < 0)
          index4 = index3;
      }
      if (index4 >= 1)
      {
        this.mHeap[index1] = this.mHeap[index4];
        index1 = index4;
        index2 = index1 * 2;
        index3 = index2 + 1;
      }
      else
        break;
    }
    this.mHeap[index1] = other1;
    return obj1;
  }

  public T Peek() => this.mHeap[1];

  public void Clear() => this.mLastIndex = 0;

  public int Count => this.mLastIndex;

  public bool Contains(T iValue) => this.Contains(iValue, 1);

  public bool Contains(T iValue, int iNode)
  {
    if (iNode > this.mLastIndex)
      return false;
    T other = this.mHeap[iNode];
    if (iValue.Equals((object) other))
      return true;
    if (iValue.CompareTo(other) < 0)
      return false;
    return this.Contains(iValue, iNode * 2) || this.Contains(iValue, iNode * 2 + 1);
  }
}
