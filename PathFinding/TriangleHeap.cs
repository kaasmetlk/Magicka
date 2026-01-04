// Decompiled with JetBrains decompiler
// Type: Magicka.PathFinding.TriangleHeap
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using PolygonHead;
using System;

#nullable disable
namespace Magicka.PathFinding;

public class TriangleHeap : IHeap<TriangleNode>
{
  protected TriangleNode[] mHeap;
  protected int mLastIndex;

  public bool IsEmpty => this.mLastIndex == 0;

  public TriangleHeap(int iSize) => this.mHeap = new TriangleNode[iSize + 1];

  protected void IncreasSize()
  {
    TriangleNode[] triangleNodeArray = new TriangleNode[(this.mHeap.Length - 1) * 2 + 1];
    for (int index = 0; index < this.mHeap.Length; ++index)
      triangleNodeArray[index] = this.mHeap[index];
    this.mHeap = triangleNodeArray;
  }

  public void Push(TriangleNode iValue)
  {
    ++this.mLastIndex;
    int index1 = this.mLastIndex;
    int index2 = index1 / 2;
    for (TriangleNode triangleNode = this.mHeap[index2]; index1 > 1 && (double) iValue.TotalCost < (double) triangleNode.TotalCost; triangleNode = this.mHeap[index2])
    {
      this.mHeap[index1] = triangleNode;
      index1 = index2;
      index2 = index1 / 2;
    }
    this.mHeap[index1] = iValue;
    if (index1 == 0)
      throw new Exception();
  }

  public TriangleNode Pop()
  {
    if (this.mLastIndex < 1)
      throw new Exception("Heap is empty!");
    TriangleNode triangleNode1 = this.mHeap[1];
    TriangleNode triangleNode2 = this.mHeap[this.mLastIndex];
    --this.mLastIndex;
    int index1 = 1;
    int index2 = 2;
    int index3 = 3;
    while (true)
    {
      int index4 = 0;
      if (index2 <= this.mLastIndex)
      {
        float totalCost1 = this.mHeap[index2].TotalCost;
        if ((double) totalCost1 < (double) triangleNode2.TotalCost)
          index4 = index2;
        float totalCost2 = this.mHeap[index3].TotalCost;
        if (index3 <= this.mLastIndex && (double) totalCost2 < (double) triangleNode2.TotalCost && (double) totalCost2 < (double) totalCost1)
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
    this.mHeap[index1] = triangleNode2;
    return triangleNode1;
  }

  public bool IsMinHeap => true;

  public TriangleNode Peek() => this.mHeap[1];

  public void Clear() => this.mLastIndex = 0;

  public int Count => this.mLastIndex;

  public bool Contains(TriangleNode iValue) => this.Contains(iValue, 1);

  private bool Contains(TriangleNode iValue, int iNode)
  {
    if (iNode > this.mLastIndex)
      return false;
    TriangleNode triangleNode = this.mHeap[iNode];
    if ((double) iValue.TotalCost == (double) triangleNode.TotalCost && (int) iValue.Id == (int) triangleNode.Id)
      return true;
    if ((double) iValue.TotalCost < (double) triangleNode.TotalCost)
      return false;
    return this.Contains(iValue, iNode * 2) || this.Contains(iValue, iNode * 2 + 1);
  }
}
