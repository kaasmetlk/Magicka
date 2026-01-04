// Decompiled with JetBrains decompiler
// Type: Magicka.PathFinding.NavMeshOctree
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.PathFinding;

internal class NavMeshOctree : Octree
{
  private MovementProperties[] mProperties;

  public NavMeshOctree(
    Vector3[] iPositions,
    TriangleVertexIndices[] iTris,
    MovementProperties[] iTriangleProperties)
    : base(iPositions, iTris)
  {
    this.mProperties = iTriangleProperties;
  }

  public float FindClosestPoint(
    ref Vector3 iPoint,
    out Vector3 oPoint,
    out int oTriangle,
    MovementProperties iProperties)
  {
    return this.FindClosestPoint(ref iPoint, out oPoint, out oTriangle, iProperties, float.MaxValue);
  }

  public float FindClosestPoint(
    ref Vector3 iPoint,
    out Vector3 oPoint,
    out int oTriangle,
    MovementProperties iProperties,
    float iStartDist)
  {
    oPoint = new Vector3();
    oTriangle = -1;
    if (this.nodes.Length == 0)
      return float.PositiveInfinity;
    int index1 = 0;
    int num1 = 1;
    this.nodeStack[0] = (ushort) 0;
    float closestPoint = iStartDist;
    while (index1 < num1)
    {
      Octree.Node node = this.nodes[(int) this.nodeStack[index1]];
      ++index1;
      for (int index2 = 0; index2 < node.triIndices.Length; ++index2)
      {
        int triIndex = node.triIndices[index2];
        if (!(this.mProperties[triIndex] != MovementProperties.Default & (this.mProperties[triIndex] & iProperties) == MovementProperties.Default))
        {
          TriangleVertexIndices tri = this.tris[triIndex];
          Vector3 position1 = this.positions[tri.I0];
          Vector3 position2 = this.positions[tri.I1];
          Vector3 position3 = this.positions[tri.I2];
          JigLibX.Geometry.Triangle rkTri = new JigLibX.Geometry.Triangle(ref position1, ref position2, ref position3);
          float pfSParam;
          float pfTParam;
          float num2 = Distance.PointTriangleDistanceSq(out pfSParam, out pfTParam, ref iPoint, ref rkTri);
          if ((double) num2 < (double) closestPoint)
          {
            closestPoint = num2;
            oTriangle = triIndex;
            rkTri.GetPoint(pfSParam, pfTParam, out oPoint);
          }
        }
      }
      int length = node.nodeIndices.Length;
      for (int index3 = 0; index3 < length; ++index3)
      {
        if ((double) Distance.SqrDistance(ref iPoint, ref this.nodes[(int) node.nodeIndices[index3]].box, out float _, out float _, out float _) < (double) closestPoint)
          this.nodeStack[num1++] = node.nodeIndices[index3];
      }
    }
    return closestPoint;
  }

  public unsafe int GetDynamicTrianglesIntersectingtAABox(
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
      Octree.Node node = this.nodes[(int) this.nodeStack[index1]];
      ++index1;
      ContainmentType result;
      testBox.Contains(ref node.box, out result);
      switch (result)
      {
        case ContainmentType.Contains:
          int length1 = node.triIndices.Length;
          for (int index2 = 0; index2 < length1; ++index2)
          {
            if (intersectingtAaBox < maxTriangles & (this.mProperties[node.triIndices[index2]] & MovementProperties.Dynamic) == MovementProperties.Dynamic)
              triangles[intersectingtAaBox++] = node.triIndices[index2];
          }
          int length2 = node.nodeIndices.Length;
          for (int index3 = 0; index3 < length2; ++index3)
            this.nodeStack[num++] = node.nodeIndices[index3];
          continue;
        case ContainmentType.Intersects:
          int length3 = node.triIndices.Length;
          for (int index4 = 0; index4 < length3; ++index4)
          {
            testBox.Contains(ref this.triBoxes[node.triIndices[index4]], out result);
            if (result != ContainmentType.Disjoint && intersectingtAaBox < maxTriangles & (this.mProperties[node.triIndices[index4]] & MovementProperties.Dynamic) == MovementProperties.Dynamic)
              triangles[intersectingtAaBox++] = node.triIndices[index4];
          }
          int length4 = node.nodeIndices.Length;
          for (int index5 = 0; index5 < length4; ++index5)
            this.nodeStack[num++] = node.nodeIndices[index5];
          continue;
        default:
          continue;
      }
    }
    return intersectingtAaBox;
  }
}
