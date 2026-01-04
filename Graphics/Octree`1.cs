// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.Octree`1
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.Graphics;

public class Octree<T> where T : IVertex
{
  private T[] positions;
  private Microsoft.Xna.Framework.BoundingBox[] triBoxes;
  private TriangleVertexIndices[] tris;
  private Octree<T>.Node[] nodes;
  private Microsoft.Xna.Framework.BoundingBox rootNodeBox;
  private AABox boundingBox;
  private ushort[] nodeStack;

  public Octree()
  {
  }

  public void Clear(bool NOTUSED)
  {
    this.positions = (T[]) null;
    this.triBoxes = (Microsoft.Xna.Framework.BoundingBox[]) null;
    this.tris = (TriangleVertexIndices[]) null;
    this.nodes = (Octree<T>.Node[]) null;
    this.boundingBox = (AABox) null;
  }

  public void AddTriangles(List<T> _positions, List<TriangleVertexIndices> _tris)
  {
    this.positions = new T[_positions.Count];
    _positions.CopyTo(this.positions);
    this.tris = new TriangleVertexIndices[_tris.Count];
    _tris.CopyTo(this.tris);
  }

  public void BuildOctree(int _maxTrisPerCellNOTUSED, float _minCellSizeNOTUSED)
  {
    this.triBoxes = new Microsoft.Xna.Framework.BoundingBox[this.tris.Length];
    this.rootNodeBox = new Microsoft.Xna.Framework.BoundingBox(new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity), new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity));
    for (int index = 0; index < this.tris.Length; ++index)
    {
      this.triBoxes[index].Min = Vector3.Min(this.positions[this.tris[index].I0].Position, Vector3.Min(this.positions[this.tris[index].I1].Position, this.positions[this.tris[index].I2].Position));
      this.triBoxes[index].Max = Vector3.Max(this.positions[this.tris[index].I0].Position, Vector3.Max(this.positions[this.tris[index].I1].Position, this.positions[this.tris[index].I2].Position));
      this.rootNodeBox.Min = Vector3.Min(this.rootNodeBox.Min, this.triBoxes[index].Min);
      this.rootNodeBox.Max = Vector3.Max(this.rootNodeBox.Max, this.triBoxes[index].Max);
    }
    this.boundingBox = new AABox(this.rootNodeBox.Min, this.rootNodeBox.Max);
    List<Octree<T>.BuildNode> buildNodeList = new List<Octree<T>.BuildNode>();
    buildNodeList.Add(new Octree<T>.BuildNode());
    buildNodeList[0].box = this.rootNodeBox;
    Microsoft.Xna.Framework.BoundingBox[] boundingBoxArray = new Microsoft.Xna.Framework.BoundingBox[8];
    for (int index1 = 0; index1 < this.tris.Length; ++index1)
    {
      int index2 = 0;
      Microsoft.Xna.Framework.BoundingBox rootNodeBox = this.rootNodeBox;
      while (rootNodeBox.Contains(this.triBoxes[index1]) == ContainmentType.Contains)
      {
        int index3 = -1;
        for (int child = 0; child < 8; ++child)
        {
          boundingBoxArray[child] = this.CreateAABox(rootNodeBox, (Octree<T>.EChild) child);
          if (boundingBoxArray[child].Contains(this.triBoxes[index1]) == ContainmentType.Contains)
          {
            index3 = child;
            break;
          }
        }
        if (index3 == -1)
        {
          buildNodeList[index2].triIndices.Add(index1);
          break;
        }
        int index4 = -1;
        for (int index5 = 0; index5 < buildNodeList[index2].nodeIndices.Count; ++index5)
        {
          if (buildNodeList[buildNodeList[index2].nodeIndices[index5]].childType == index3)
          {
            index4 = index5;
            break;
          }
        }
        if (index4 == -1)
        {
          Octree<T>.BuildNode buildNode = buildNodeList[index2];
          buildNodeList.Add(new Octree<T>.BuildNode()
          {
            childType = index3,
            box = boundingBoxArray[index3]
          });
          index2 = buildNodeList.Count - 1;
          rootNodeBox = boundingBoxArray[index3];
          buildNode.nodeIndices.Add(index2);
        }
        else
        {
          index2 = buildNodeList[index2].nodeIndices[index4];
          rootNodeBox = boundingBoxArray[index3];
        }
      }
    }
    this.nodes = new Octree<T>.Node[buildNodeList.Count];
    this.nodeStack = new ushort[buildNodeList.Count];
    for (int index6 = 0; index6 < this.nodes.Length; ++index6)
    {
      this.nodes[index6].nodeIndices = new ushort[buildNodeList[index6].nodeIndices.Count];
      for (int index7 = 0; index7 < this.nodes[index6].nodeIndices.Length; ++index7)
        this.nodes[index6].nodeIndices[index7] = (ushort) buildNodeList[index6].nodeIndices[index7];
      this.nodes[index6].triIndices = new int[buildNodeList[index6].triIndices.Count];
      buildNodeList[index6].triIndices.CopyTo(this.nodes[index6].triIndices);
      this.nodes[index6].box = buildNodeList[index6].box;
    }
  }

  public Octree(List<T> _positions, List<TriangleVertexIndices> _tris)
  {
    this.AddTriangles(_positions, _tris);
    this.BuildOctree(16 /*0x10*/, 1f);
  }

  private Microsoft.Xna.Framework.BoundingBox CreateAABox(Microsoft.Xna.Framework.BoundingBox aabb, Octree<T>.EChild child)
  {
    Vector3 vector3_1 = 0.5f * (aabb.Max - aabb.Min);
    Vector3 vector3_2 = new Vector3();
    switch (child)
    {
      case Octree<>.EChild.MMM:
        vector3_2 = new Vector3(0.0f, 0.0f, 0.0f);
        break;
      case Octree<>.EChild.XP:
        vector3_2 = new Vector3(1f, 0.0f, 0.0f);
        break;
      case Octree<>.EChild.YP:
        vector3_2 = new Vector3(0.0f, 1f, 0.0f);
        break;
      case Octree<>.EChild.PPM:
        vector3_2 = new Vector3(1f, 1f, 0.0f);
        break;
      case Octree<>.EChild.ZP:
        vector3_2 = new Vector3(0.0f, 0.0f, 1f);
        break;
      case Octree<>.EChild.PMP:
        vector3_2 = new Vector3(1f, 0.0f, 1f);
        break;
      case Octree<>.EChild.MPP:
        vector3_2 = new Vector3(0.0f, 1f, 1f);
        break;
      case Octree<>.EChild.PPP:
        vector3_2 = new Vector3(1f, 1f, 1f);
        break;
    }
    Microsoft.Xna.Framework.BoundingBox aaBox = new Microsoft.Xna.Framework.BoundingBox()
    {
      Min = aabb.Min + new Vector3(vector3_2.X * vector3_1.X, vector3_2.Y * vector3_1.Y, vector3_2.Z * vector3_1.Z)
    };
    aaBox.Max = aaBox.Min + vector3_1;
    float num = 1E-05f;
    aaBox.Min -= num * vector3_1;
    aaBox.Max += num * vector3_1;
    return aaBox;
  }

  private void GatherTriangles(int _nodeIndex, ref List<int> _tris)
  {
    _tris.AddRange((IEnumerable<int>) this.nodes[_nodeIndex].triIndices);
    int length = this.nodes[_nodeIndex].nodeIndices.Length;
    for (int index = 0; index < length; ++index)
      this.GatherTriangles((int) this.nodes[_nodeIndex].nodeIndices[index], ref _tris);
  }

  public unsafe int GetTrianglesIntersectingtAABox(
    int* triangles,
    int maxTriangles,
    ref Microsoft.Xna.Framework.BoundingBox testBox)
  {
    if (this.nodes.Length == 0)
      return 0;
    int index1 = 0;
    int num = 1;
    this.nodeStack[0] = (ushort) 0;
    int intersectingtAaBox = 0;
    while (index1 < num)
    {
      ushort node = this.nodeStack[index1];
      ++index1;
      if (this.nodes[(int) node].box.Contains(testBox) != ContainmentType.Disjoint)
      {
        for (int index2 = 0; index2 < this.nodes[(int) node].triIndices.Length; ++index2)
        {
          if (this.triBoxes[this.nodes[(int) node].triIndices[index2]].Contains(testBox) != ContainmentType.Disjoint && intersectingtAaBox < maxTriangles)
            triangles[intersectingtAaBox++] = this.nodes[(int) node].triIndices[index2];
        }
        int length = this.nodes[(int) node].nodeIndices.Length;
        for (int index3 = 0; index3 < length; ++index3)
          this.nodeStack[num++] = this.nodes[(int) node].nodeIndices[index3];
      }
    }
    return intersectingtAaBox;
  }

  public AABox BoundingBox => this.boundingBox;

  public Octree<T>.Node[] Nodes => this.nodes;

  public IndexedTriangle GetTriangle(int _index)
  {
    TriangleVertexIndices tri = this.tris[_index];
    return new IndexedTriangle(tri.I0, tri.I1, tri.I2, this.positions[tri.I0].Position, this.positions[tri.I1].Position, this.positions[tri.I2].Position);
  }

  public T GetVertex(int iVertex) => this.positions[iVertex];

  public void GetVertex(int iVertex, out T result) => result = this.positions[iVertex];

  public int NumTriangles => this.tris.Length;

  [Flags]
  internal enum EChild
  {
    XP = 1,
    YP = 2,
    ZP = 4,
    PPP = ZP | YP | XP, // 0x00000007
    PPM = YP | XP, // 0x00000003
    PMP = ZP | XP, // 0x00000005
    PMM = XP, // 0x00000001
    MPP = ZP | YP, // 0x00000006
    MPM = YP, // 0x00000002
    MMP = ZP, // 0x00000004
    MMM = 0,
  }

  public struct Node
  {
    public ushort[] nodeIndices;
    public int[] triIndices;
    public Microsoft.Xna.Framework.BoundingBox box;
  }

  private class BuildNode
  {
    public int childType;
    public List<int> nodeIndices = new List<int>();
    public List<int> triIndices = new List<int>();
    public Microsoft.Xna.Framework.BoundingBox box;
  }
}
