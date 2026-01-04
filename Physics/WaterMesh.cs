// Decompiled with JetBrains decompiler
// Type: Magicka.Physics.WaterMesh
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace Magicka.Physics;

public class WaterMesh : TriangleMesh
{
  internal float[] frozenVertices;

  public void CreateMesh(
    Vector3[] vertices,
    float[] frozenVertices,
    TriangleVertexIndices[] triangleVertexIndices,
    int maxTrianglesPerCell,
    float minCellSize)
  {
    this.octree.Clear(true);
    this.octree.AddTriangles(vertices, triangleVertexIndices);
    this.octree.BuildOctree(maxTrianglesPerCell, minCellSize);
    this.frozenVertices = frozenVertices;
    this.maxTrianglesPerCell = maxTrianglesPerCell;
    this.minCellSize = minCellSize;
  }

  public unsafe int GetAllTrianglesIntersectingtAABox(
    int* triangles,
    int maxTriangles,
    ref BoundingBox bb)
  {
    return base.GetTrianglesIntersectingtAABox(triangles, maxTriangles, ref bb);
  }

  public override unsafe int GetTrianglesIntersectingtAABox(
    int* triangles,
    int maxTriangles,
    ref BoundingBox bb)
  {
    int intersectingtAaBox = base.GetTrianglesIntersectingtAABox(triangles, maxTriangles, ref bb);
    for (int index1 = 0; index1 < intersectingtAaBox; ++index1)
    {
      int i0;
      int i1;
      int i2;
      this.octree.GetTriangle(triangles[index1]).GetVertexIndices(out i0, out i1, out i2);
      if ((double) this.frozenVertices[i0] >= 0.5 || (double) this.frozenVertices[i1] >= 0.5 || (double) this.frozenVertices[i2] >= 0.5)
      {
        for (int index2 = index1 + 1; index2 < intersectingtAaBox; ++index2)
          triangles[index2 - 1] = triangles[index2];
        --intersectingtAaBox;
        --index1;
      }
    }
    return intersectingtAaBox;
  }

  public override Primitive Clone()
  {
    WaterMesh waterMesh = new WaterMesh();
    waterMesh.octree = this.octree;
    waterMesh.SetTransform(ref this.transform);
    waterMesh.frozenVertices = this.frozenVertices;
    return (Primitive) waterMesh;
  }

  public IceMesh CloneToIceMesh()
  {
    IceMesh iceMesh = new IceMesh();
    iceMesh.SetOctree(this.octree);
    iceMesh.SetTransform(ref this.transform);
    iceMesh.frozenVertices = this.frozenVertices;
    return iceMesh;
  }

  public unsafe int GetVerticesIntersectingArc(
    int* iVertices,
    int iMaxVertices,
    ref Vector3 iOrigin,
    ref Vector3 iDirection,
    float iSpread)
  {
    iOrigin.Y = 0.0f;
    iDirection.Y = 0.0f;
    float num1 = iDirection.Length();
    Vector3 result1;
    Vector3.Divide(ref iDirection, num1, out result1);
    BoundingBox initialBox = BoundingBoxHelper.InitialBox;
    initialBox.Min.Y = -10000f;
    initialBox.Max.Y = 10000f;
    BoundingBoxHelper.AddPoint(ref iOrigin, ref initialBox);
    Vector3 result2;
    if ((double) MagickaMath.Angle(ref result1, ref new Vector3()
    {
      X = -1f
    }) <= (double) iSpread)
    {
      result2 = iOrigin;
      result2.X -= num1;
      BoundingBoxHelper.AddPoint(ref result2, ref initialBox);
    }
    result2 = new Vector3();
    result2.X = 1f;
    if ((double) MagickaMath.Angle(ref result1, ref result2) <= (double) iSpread)
    {
      result2 = iOrigin;
      result2.X += num1;
      BoundingBoxHelper.AddPoint(ref result2, ref initialBox);
    }
    result2.Z = -1f;
    if ((double) MagickaMath.Angle(ref result1, ref result2) <= (double) iSpread)
    {
      result2 = iOrigin;
      result2.Z -= num1;
      BoundingBoxHelper.AddPoint(ref result2, ref initialBox);
    }
    result2 = new Vector3();
    result2.Z = 1f;
    if ((double) MagickaMath.Angle(ref result1, ref result2) <= (double) iSpread)
    {
      result2 = iOrigin;
      result2.Z += num1;
      BoundingBoxHelper.AddPoint(ref result2, ref initialBox);
    }
    Quaternion result3;
    Quaternion.CreateFromYawPitchRoll(iSpread, 0.0f, 0.0f, out result3);
    Vector3.Transform(ref iDirection, ref result3, out result2);
    Vector3.Add(ref iOrigin, ref result2, out result2);
    BoundingBoxHelper.AddPoint(ref result2, ref initialBox);
    Quaternion.CreateFromYawPitchRoll(-iSpread, 0.0f, 0.0f, out result3);
    Vector3.Transform(ref iDirection, ref result3, out result2);
    Vector3.Add(ref iOrigin, ref result2, out result2);
    BoundingBoxHelper.AddPoint(ref result2, ref initialBox);
    float num2 = num1 * num1;
    int verticesIntersectingArc = 0;
    int[] alloced = DetectFunctor.IntStackAlloc();
    fixed (int* triangles = alloced)
    {
      int intersectingtAaBox = base.GetTrianglesIntersectingtAABox(triangles, 2048 /*0x0800*/, ref initialBox);
      for (int index1 = 0; index1 < intersectingtAaBox; ++index1)
      {
        IndexedTriangle triangle = this.GetTriangle(triangles[index1]);
        for (int iCorner = 0; iCorner < 3; ++iCorner)
        {
          int vertexIndex = triangle.GetVertexIndex(iCorner);
          Vector3 result4 = this.octree.GetVertex(vertexIndex) with
          {
            Y = 0.0f
          };
          float result5;
          Vector3.DistanceSquared(ref iOrigin, ref result4, out result5);
          if ((double) result5 <= (double) num2)
          {
            bool flag = false;
            for (int index2 = 0; index2 < verticesIntersectingArc; ++index2)
            {
              if (iVertices[index2] == vertexIndex)
              {
                flag = true;
                break;
              }
            }
            if (!flag)
            {
              result5 = (float) Math.Sqrt((double) result5);
              Vector3.Subtract(ref result4, ref iOrigin, out result4);
              Vector3.Divide(ref result4, result5, out result4);
              if ((double) MagickaMath.Angle(ref result1, ref result4) <= (double) iSpread)
                iVertices[verticesIntersectingArc++] = vertexIndex;
            }
          }
          if (verticesIntersectingArc == iMaxVertices)
            break;
        }
      }
    }
    DetectFunctor.FreeStackAlloc(alloced);
    return verticesIntersectingArc;
  }
}
