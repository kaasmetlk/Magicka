// Decompiled with JetBrains decompiler
// Type: PolygonHead.IntHeap
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using System;

#nullable disable
namespace PolygonHead;

public class IntHeap : IHeap<int>
{
  protected int[] mHeap;
  protected int mLastIndex;

  public bool IsEmpty => this.mLastIndex == 0;

  public IntHeap(int iSize) => this.mHeap = new int[iSize + 1];

  protected void IncreasSize()
  {
    int[] numArray = new int[(this.mHeap.Length - 1) * 2 + 1];
    for (int index = 0; index < this.mHeap.Length; ++index)
      numArray[index] = this.mHeap[index];
    this.mHeap = numArray;
  }

  public void Push(int iValue)
  {
    ++this.mLastIndex;
    int index1 = this.mLastIndex;
    int index2 = index1 / 2;
    for (int index3 = this.mHeap[index2]; index1 > 1 && iValue < index3; index3 = this.mHeap[index2])
    {
      this.mHeap[index1] = index3;
      index1 = index2;
      index2 = index1 / 2;
    }
    this.mHeap[index1] = iValue;
    if (index1 == 0)
      throw new Exception();
  }

  public int Pop()
  {
    if (this.mLastIndex < 1)
      throw new Exception("Heap is empty!");
    int num1 = this.mHeap[1];
    int num2 = this.mHeap[this.mLastIndex];
    --this.mLastIndex;
    int index1 = 1;
    int index2 = 2;
    int index3 = 3;
    while (true)
    {
      int index4 = 0;
      if (index2 <= this.mLastIndex)
      {
        int num3 = this.mHeap[index2];
        if (num3 < num2)
          index4 = index2;
        int num4 = this.mHeap[index3];
        if (index3 <= this.mLastIndex && num4 < num2 && num4 < num3)
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
    this.mHeap[index1] = num2;
    return num1;
  }

  public int Peek() => this.mHeap[1];

  public void Clear() => this.mLastIndex = 0;

  public int Count => this.mLastIndex;

  public bool Contains(int iValue) => this.Contains(iValue, 1);

  public bool Contains(int iValue, int iNode)
  {
    if (iNode > this.mLastIndex)
      return false;
    int num = this.mHeap[iNode];
    if (iValue == num)
      return true;
    if (iValue < num)
      return false;
    return this.Contains(iValue, iNode * 2) || this.Contains(iValue, iNode * 2 + 1);
  }
}
