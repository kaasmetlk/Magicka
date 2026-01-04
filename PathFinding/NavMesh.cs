// Decompiled with JetBrains decompiler
// Type: Magicka.PathFinding.NavMesh
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using Magicka.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.PathFinding;

internal class NavMesh
{
  private static Vector3 BBBias = new Vector3(0.25f);
  private Vector3[] mVertices;
  private Triangle[] mTriangles;
  private NavMeshOctree mMesh;
  private List<uint> mTrianglePath = new List<uint>(512 /*0x0200*/);
  private List<int> mPathSmoothingTriangleList = new List<int>(512 /*0x0200*/);
  private Dictionary<uint, float> mCostList = new Dictionary<uint, float>();
  private Dictionary<uint, TriangleNode> mClosedList = new Dictionary<uint, TriangleNode>();
  private TriangleHeap mOpenList = new TriangleHeap(512 /*0x0200*/);
  private Dictionary<ushort, List<NeighbourInfo>> mDynamicNeighbours = new Dictionary<ushort, List<NeighbourInfo>>();
  private List<AnimatedNavMesh> mAnimatedParts = new List<AnimatedNavMesh>();
  private LevelModel mLevel;

  public NavMesh(LevelModel iLevel, ContentReader iInput)
  {
    this.mLevel = iLevel;
    this.mVertices = new Vector3[(int) iInput.ReadUInt16()];
    for (int index = 0; index < this.mVertices.Length; ++index)
      this.mVertices[index] = iInput.ReadVector3();
    this.mTriangles = new Triangle[(int) iInput.ReadUInt16()];
    for (int index = 0; index < this.mTriangles.Length; ++index)
      this.mTriangles[index] = new Triangle(iInput);
    TriangleVertexIndices[] iTris = new TriangleVertexIndices[this.mTriangles.Length];
    MovementProperties[] iTriangleProperties = new MovementProperties[this.mTriangles.Length];
    for (int key = 0; key < this.mTriangles.Length; ++key)
    {
      iTris[key].I0 = (int) this.mTriangles[key].VertexA;
      iTris[key].I1 = (int) this.mTriangles[key].VertexB;
      iTris[key].I2 = (int) this.mTriangles[key].VertexC;
      if ((this.mTriangles[key].Properties & MovementProperties.Dynamic) == MovementProperties.Dynamic)
        this.mDynamicNeighbours.Add((ushort) key, new List<NeighbourInfo>(10));
      iTriangleProperties[key] = this.mTriangles[key].Properties;
    }
    this.mMesh = new NavMeshOctree(this.mVertices, iTris, iTriangleProperties);
  }

  public float GetNearestPosition(
    ref Vector3 iPoint,
    out Vector3 oPoint,
    MovementProperties iProperties)
  {
    return this.GetNearestPosition(ref iPoint, out oPoint, iProperties, out ushort _, out ushort _);
  }

  public float GetNearestPosition(
    ref Vector3 iPoint,
    out Vector3 oPoint,
    MovementProperties iProperties,
    out ushort oTriangle,
    out ushort oMesh)
  {
    oMesh = ushort.MaxValue;
    int oTriangle1;
    float iStartDist = this.mMesh.FindClosestPoint(ref iPoint, out oPoint, out oTriangle1, iProperties);
    for (int index = 0; index < this.mAnimatedParts.Count; ++index)
    {
      Vector3 result;
      Vector3.Transform(ref iPoint, ref this.mAnimatedParts[index].mInvTransform, out result);
      Vector3 oPoint1;
      int oTriangle2;
      float closestPoint = this.mAnimatedParts[index].Mesh.FindClosestPoint(ref result, out oPoint1, out oTriangle2, iProperties, iStartDist);
      if ((double) closestPoint < (double) iStartDist)
      {
        iStartDist = closestPoint;
        Vector3.Transform(ref oPoint1, ref this.mAnimatedParts[index].mTransform, out oPoint);
        oTriangle1 = oTriangle2;
        oMesh = (ushort) index;
      }
    }
    oTriangle = (ushort) oTriangle1;
    return iStartDist;
  }

  public bool FindShortestPath(
    ref Vector3 iStartPos,
    ref Vector3 iEndPos,
    List<PathNode> iPath,
    MovementProperties iMoveAbilities)
  {
    bool path = this.FindPath(ref iStartPos, ref iEndPos, this.mTrianglePath, iMoveAbilities);
    this.StraightenPath(ref iStartPos, ref iEndPos, this.mTrianglePath, iPath, iMoveAbilities);
    return path;
  }

  public void UpdateNeighbours()
  {
    foreach (List<NeighbourInfo> neighbourInfoList in this.mDynamicNeighbours.Values)
      neighbourInfoList.Clear();
    for (int index = 0; index < this.mAnimatedParts.Count; ++index)
      this.mAnimatedParts[index].ClearNeighbours();
    Vector3 vector3 = new Vector3();
    vector3.X = vector3.Y = vector3.Z = 0.25f;
    foreach (KeyValuePair<ushort, List<NeighbourInfo>> dynamicNeighbour in this.mDynamicNeighbours)
    {
      Vector3 mVertex1 = this.mVertices[(int) this.mTriangles[(int) dynamicNeighbour.Key].VertexA];
      Vector3 mVertex2 = this.mVertices[(int) this.mTriangles[(int) dynamicNeighbour.Key].VertexB];
      Vector3 mVertex3 = this.mVertices[(int) this.mTriangles[(int) dynamicNeighbour.Key].VertexC];
      for (int index = 0; index < this.mAnimatedParts.Count; ++index)
      {
        AnimatedNavMesh mAnimatedPart = this.mAnimatedParts[index];
        Vector3 result1;
        Vector3.Transform(ref mVertex1, ref mAnimatedPart.mInvTransform, out result1);
        Vector3 result2;
        Vector3.Transform(ref mVertex2, ref mAnimatedPart.mInvTransform, out result2);
        Vector3 result3;
        Vector3.Transform(ref mVertex3, ref mAnimatedPart.mInvTransform, out result3);
        this.FindNeighbours(dynamicNeighbour.Key, ref result1, ref result2, ref result3, dynamicNeighbour.Value, mAnimatedPart, (ushort) index);
      }
    }
    for (int index = 0; index < this.mAnimatedParts.Count; ++index)
      this.mAnimatedParts[index].FindNeighbours((ushort) index, this.mAnimatedParts);
  }

  private unsafe void FindNeighbours(
    ushort iTri,
    ref Vector3 iA,
    ref Vector3 iB,
    ref Vector3 iC,
    List<NeighbourInfo> iNeighbours,
    AnimatedNavMesh iMesh,
    ushort iMeshID)
  {
    BoundingBox testBox;
    Vector3.Max(ref iA, ref iB, out testBox.Max);
    Vector3.Max(ref iC, ref testBox.Max, out testBox.Max);
    Vector3.Add(ref testBox.Max, ref NavMesh.BBBias, out testBox.Max);
    Vector3.Min(ref iA, ref iB, out testBox.Min);
    Vector3.Min(ref iC, ref testBox.Min, out testBox.Min);
    Vector3.Subtract(ref testBox.Min, ref NavMesh.BBBias, out testBox.Min);
    JigLibX.Geometry.Triangle tri1 = new JigLibX.Geometry.Triangle(ref iA, ref iB, ref iC);
    int[] alloced = DetectFunctor.IntStackAlloc();
    int intersectingtAaBox;
    fixed (int* triangles = alloced)
      intersectingtAaBox = iMesh.Mesh.GetDynamicTrianglesIntersectingtAABox(triangles, 2048 /*0x0800*/, ref testBox);
    Vector3 result1;
    Vector3.Add(ref iA, ref iB, out result1);
    Vector3.Add(ref iC, ref result1, out result1);
    Vector3.Multiply(ref result1, 0.333333343f, out result1);
    for (int index = 0; index < intersectingtAaBox; ++index)
    {
      int i0;
      int i1;
      int i2;
      iMesh.Mesh.GetTriangle(alloced[index]).GetVertexIndices(out i0, out i1, out i2);
      Vector3 result2;
      iMesh.Mesh.GetVertex(i0, out result2);
      Vector3 result3;
      iMesh.Mesh.GetVertex(i1, out result3);
      Vector3 result4;
      iMesh.Mesh.GetVertex(i2, out result4);
      JigLibX.Geometry.Triangle tri0 = new JigLibX.Geometry.Triangle(ref result2, ref result3, ref result4);
      Vector3 result5;
      Vector3.Add(ref result2, ref result3, out result5);
      Vector3.Add(ref result4, ref result5, out result5);
      Vector3.Multiply(ref result5, 0.333333343f, out result5);
      if ((double) Distance.TriangleTriangleDistanceSq(ref tri0, ref tri1) <= 1.0 / 16.0)
      {
        NeighbourInfo neighbourInfo;
        neighbourInfo.Mesh = iMeshID;
        neighbourInfo.Triangle = (ushort) alloced[index];
        Vector3.Distance(ref result1, ref result5, out neighbourInfo.Cost);
        Vector3.Add(ref result2, ref result3, out neighbourInfo.EdgePos);
        Vector3.Add(ref result4, ref neighbourInfo.EdgePos, out neighbourInfo.EdgePos);
        Vector3.Multiply(ref neighbourInfo.EdgePos, 0.333333343f, out neighbourInfo.EdgePos);
        Vector3.Transform(ref neighbourInfo.EdgePos, ref iMesh.mTransform, out neighbourInfo.EdgePos);
        iNeighbours.Add(neighbourInfo);
        neighbourInfo.Triangle = iTri;
        neighbourInfo.Mesh = ushort.MaxValue;
        iMesh.DynamicNeighbours[(ushort) alloced[index]].Add(neighbourInfo);
      }
    }
    DetectFunctor.FreeStackAlloc(alloced);
  }

  public unsafe bool FindPath(
    ref Vector3 iStartPos,
    ref Vector3 iEndPos,
    List<uint> iPath,
    MovementProperties iMoveAbilities)
  {
    if (float.IsNaN(iStartPos.X + iStartPos.Y + iStartPos.Z))
      return false;
    bool flag1 = false;
    for (int index = 0; index < this.mAnimatedParts.Count & !flag1; ++index)
      flag1 = this.mAnimatedParts[index].Dirty;
    this.UpdateNeighbours();
    iMoveAbilities |= MovementProperties.Dynamic;
    iMoveAbilities |= MovementProperties.Water;
    this.mClosedList.Clear();
    this.mCostList.Clear();
    this.mOpenList.Clear();
    iPath.Clear();
    Vector3 oPoint1;
    ushort oTriangle1;
    ushort oMesh1;
    double nearestPosition1 = (double) this.GetNearestPosition(ref iStartPos, out oPoint1, iMoveAbilities, out oTriangle1, out oMesh1);
    ushort oMesh2 = ushort.MaxValue;
    Vector3 oPoint2;
    ushort oTriangle2;
    double nearestPosition2 = (double) this.GetNearestPosition(ref iEndPos, out oPoint2, iMoveAbilities, out oTriangle2, out oMesh2);
    TriangleNode iValue1;
    iValue1.Cost = 0.0f;
    iValue1.TotalCost = this.ManhattanDistance(ref oPoint1, ref oPoint2);
    iValue1.Id = oTriangle1;
    iValue1.Mesh = oMesh1;
    iValue1.ParentId = (uint) ((int) oMesh1 << 16 /*0x10*/ | (int) ushort.MaxValue);
    this.mOpenList.Push(iValue1);
    float totalCost = iValue1.TotalCost;
    TriangleNode triangleNode1 = iValue1;
    bool path = false;
    while (!this.mOpenList.IsEmpty)
    {
      TriangleNode triangleNode2 = this.mOpenList.Pop();
      if (!this.mClosedList.ContainsKey(triangleNode2.LongId))
      {
        this.mClosedList.Add(triangleNode2.LongId, triangleNode2);
        if ((int) triangleNode2.Id == (int) oTriangle2 & (int) triangleNode2.Mesh == (int) oMesh2)
        {
          triangleNode1 = triangleNode2;
          path = true;
          break;
        }
        Triangle[] triangleArray1;
        Vector3[] iVertices;
        if (triangleNode2.Mesh == ushort.MaxValue)
        {
          triangleArray1 = this.mTriangles;
          iVertices = this.mVertices;
        }
        else
        {
          triangleArray1 = this.mAnimatedParts[(int) triangleNode2.Mesh].Triangles;
          iVertices = this.mAnimatedParts[(int) triangleNode2.Mesh].Vertices;
        }
        Triangle triangle1 = triangleArray1[(int) triangleNode2.Id];
        ushort* numPtr = &triangle1.NeighbourA;
        for (byte iTargetEdge = 0; iTargetEdge < (byte) 3; ++iTargetEdge)
        {
          ushort index = numPtr[iTargetEdge];
          uint key = (uint) triangleNode2.Mesh << 16 /*0x10*/ | (uint) index;
          if (!(index == ushort.MaxValue | this.mClosedList.ContainsKey(key)))
          {
            MovementProperties properties = triangleArray1[(int) index].Properties;
            if (!(properties != MovementProperties.Default & (properties & iMoveAbilities) == MovementProperties.Default))
            {
              if ((properties & MovementProperties.Water) == MovementProperties.Water)
              {
                Segment seg = new Segment();
                triangleArray1[(int) index].GetCenter(iVertices, out seg.Origin);
                ++seg.Origin.Y;
                seg.Delta.Y = -4f;
                bool flag2 = false;
                foreach (Liquid water in this.mLevel.Waters)
                {
                  if (water.SegmentIntersect(out float _, out Vector3 _, out Vector3 _, ref seg, false, true, false))
                  {
                    flag2 = true;
                    break;
                  }
                }
                if (!flag2)
                  continue;
              }
              Vector3 result1;
              Vector3.Add(ref iVertices[(int) (&triangle1.VertexA)[iTargetEdge]], ref iVertices[(int) (&triangle1.VertexA)[((int) iTargetEdge + 1) % 3]], out result1);
              Vector3.Multiply(ref result1, 0.5f, out result1);
              if (triangleNode2.Mesh != ushort.MaxValue)
                Vector3.Transform(ref result1, ref this.mAnimatedParts[(int) triangleNode2.Mesh].mTransform, out result1);
              float result2;
              if (((int) triangleNode2.ParentId & (int) ushort.MaxValue) == (int) ushort.MaxValue)
                Vector3.Distance(ref oPoint1, ref result1, out result2);
              else
                result2 = (int) (ushort) (triangleNode2.ParentId >> 16 /*0x10*/) != (int) triangleNode2.Mesh ? 0.5f : triangle1.GetCostFrom((ushort) triangleNode2.ParentId, iTargetEdge);
              if ((int) index == (int) oTriangle2 & (int) triangleNode2.Mesh == (int) oMesh2)
              {
                float result3;
                Vector3.Distance(ref result1, ref oPoint2, out result3);
                result2 += result3;
              }
              result2 += triangleNode2.Cost;
              float num;
              if (!this.mCostList.TryGetValue(key, out num) | (double) num > (double) result2)
              {
                this.mCostList[key] = result2;
                TriangleNode iValue2;
                iValue2.Mesh = triangleNode2.Mesh;
                iValue2.Id = index;
                iValue2.ParentId = triangleNode2.LongId;
                iValue2.Cost = result2;
                iValue2.TotalCost = result2 + this.ManhattanDistance(ref result1, ref oPoint2);
                this.mOpenList.Push(iValue2);
                if ((double) iValue2.TotalCost < (double) totalCost)
                {
                  totalCost = iValue2.TotalCost;
                  triangleNode1 = iValue2;
                }
              }
            }
          }
        }
        if ((triangle1.Properties & MovementProperties.Dynamic) == MovementProperties.Dynamic)
        {
          List<NeighbourInfo> neighbourInfoList = triangleNode2.Mesh != ushort.MaxValue ? this.mAnimatedParts[(int) triangleNode2.Mesh].DynamicNeighbours[triangleNode2.Id] : this.mDynamicNeighbours[triangleNode2.Id];
          for (byte index = 0; (int) index < neighbourInfoList.Count; ++index)
          {
            NeighbourInfo neighbourInfo = neighbourInfoList[(int) index];
            Triangle[] triangleArray2 = neighbourInfo.Mesh != ushort.MaxValue ? this.mAnimatedParts[(int) neighbourInfo.Mesh].Triangles : this.mTriangles;
            ushort triangle2 = neighbourInfo.Triangle;
            uint key = (uint) neighbourInfo.Mesh << 16 /*0x10*/ | (uint) triangle2;
            if (!(triangle2 == ushort.MaxValue | this.mClosedList.ContainsKey(key)))
            {
              MovementProperties properties = triangleArray2[(int) triangle2].Properties;
              if (!(properties != MovementProperties.Default & (properties & iMoveAbilities) == MovementProperties.Default))
              {
                float result4;
                if (((int) triangleNode2.ParentId & (int) ushort.MaxValue) == (int) ushort.MaxValue)
                  Vector3.Distance(ref oPoint1, ref neighbourInfo.EdgePos, out result4);
                else
                  result4 = neighbourInfo.Cost;
                if ((int) triangle2 == (int) oTriangle2 & (int) neighbourInfo.Mesh == (int) oMesh2)
                {
                  float result5;
                  Vector3.Distance(ref neighbourInfo.EdgePos, ref oPoint2, out result5);
                  result4 += result5;
                }
                result4 += neighbourInfo.Cost;
                float num;
                if (!this.mCostList.TryGetValue(key, out num) | (double) num > (double) result4)
                {
                  this.mCostList[key] = result4;
                  TriangleNode iValue3;
                  iValue3.Mesh = neighbourInfo.Mesh;
                  iValue3.Id = triangle2;
                  iValue3.ParentId = triangleNode2.LongId;
                  iValue3.Cost = result4;
                  iValue3.TotalCost = result4 + this.ManhattanDistance(ref neighbourInfo.EdgePos, ref oPoint2);
                  this.mOpenList.Push(iValue3);
                  if ((double) iValue3.TotalCost < (double) totalCost)
                  {
                    totalCost = iValue3.TotalCost;
                    triangleNode1 = iValue3;
                  }
                }
              }
            }
          }
        }
      }
    }
    for (uint key = triangleNode1.LongId; (ushort) key != ushort.MaxValue; key = this.mClosedList[key].ParentId)
      iPath.Add(key);
    return path;
  }

  public float ManhattanDistance(ref Vector3 startPos, ref Vector3 endPos)
  {
    return Math.Abs(startPos.X - endPos.X) + Math.Abs(startPos.Z - endPos.Z) * 1.75f;
  }

  public float TriArea2D(ref Vector3 iA, ref Vector3 iB, ref Vector3 iC)
  {
    return (float) (((double) iB.X * (double) iA.Z - (double) iA.X * (double) iB.Z + ((double) iC.X * (double) iB.Z - (double) iB.X * (double) iC.Z) + ((double) iA.X * (double) iC.Z - (double) iC.X * (double) iA.Z)) * 0.5);
  }

  public void StraightenPath(
    ref Vector3 iStartPos,
    ref Vector3 iEndPos,
    List<uint> iTrianglePath,
    List<PathNode> iStraightPath,
    MovementProperties iMoveAbilities)
  {
    iStraightPath.Clear();
    if (iTrianglePath.Count == 0 || iTrianglePath[0] == (uint) ushort.MaxValue)
      return;
    uint num1 = this.mTrianglePath[iTrianglePath.Count - 1];
    ushort index1 = (ushort) (num1 >> 16 /*0x10*/);
    ushort index2 = (ushort) num1;
    Triangle[] triangleArray1;
    Vector3[] vector3Array1;
    if (index1 == ushort.MaxValue)
    {
      triangleArray1 = this.mTriangles;
      vector3Array1 = this.mVertices;
    }
    else
    {
      triangleArray1 = this.mAnimatedParts[(int) index1].Triangles;
      vector3Array1 = this.mAnimatedParts[(int) index1].Vertices;
    }
    Vector3 vector3_1 = iStartPos;
    if (index1 != ushort.MaxValue)
      Vector3.Transform(ref vector3_1, ref this.mAnimatedParts[(int) index1].mInvTransform, out vector3_1);
    JigLibX.Geometry.Triangle rkTri1 = new JigLibX.Geometry.Triangle(ref vector3Array1[(int) triangleArray1[(int) index2].VertexA], ref vector3Array1[(int) triangleArray1[(int) index2].VertexB], ref vector3Array1[(int) triangleArray1[(int) index2].VertexC]);
    float pfSParam;
    float pfTParam;
    double num2 = (double) Distance.PointTriangleDistanceSq(out pfSParam, out pfTParam, ref vector3_1, ref rkTri1);
    rkTri1.GetPoint(pfSParam, pfTParam, out vector3_1);
    if (index1 != ushort.MaxValue)
      Vector3.Transform(ref vector3_1, ref this.mAnimatedParts[(int) index1].mTransform, out vector3_1);
    uint num3 = this.mTrianglePath[0];
    ushort index3 = (ushort) (num3 >> 16 /*0x10*/);
    ushort index4 = (ushort) num3;
    Triangle[] triangleArray2;
    Vector3[] vector3Array2;
    if (index3 == ushort.MaxValue)
    {
      triangleArray2 = this.mTriangles;
      vector3Array2 = this.mVertices;
    }
    else
    {
      triangleArray2 = this.mAnimatedParts[(int) index3].Triangles;
      vector3Array2 = this.mAnimatedParts[(int) index3].Vertices;
    }
    Vector3 vector3_2 = iEndPos;
    if (index3 != ushort.MaxValue)
      Vector3.Transform(ref vector3_2, ref this.mAnimatedParts[(int) index3].mInvTransform, out vector3_2);
    JigLibX.Geometry.Triangle rkTri2 = new JigLibX.Geometry.Triangle(ref vector3Array2[(int) triangleArray2[(int) index4].VertexA], ref vector3Array2[(int) triangleArray2[(int) index4].VertexB], ref vector3Array2[(int) triangleArray2[(int) index4].VertexC]);
    double num4 = (double) Distance.PointTriangleDistanceSq(out pfSParam, out pfTParam, ref vector3_2, ref rkTri2);
    rkTri2.GetPoint(pfSParam, pfTParam, out vector3_2);
    if (index3 != ushort.MaxValue)
      Vector3.Transform(ref vector3_2, ref this.mAnimatedParts[(int) index3].mTransform, out vector3_2);
    this.mPathSmoothingTriangleList.Clear();
    PathNode pathNode = new PathNode();
    Triangle[] triangleArray3;
    Vector3[] vector3Array3;
    if (index1 == ushort.MaxValue)
    {
      triangleArray3 = this.mTriangles;
      vector3Array3 = this.mVertices;
    }
    else
    {
      triangleArray3 = this.mAnimatedParts[(int) index1].Triangles;
      vector3Array3 = this.mAnimatedParts[(int) index1].Vertices;
    }
    pathNode.Position = vector3_1;
    pathNode.Properties = triangleArray3[(int) index2].Properties;
    iStraightPath.Add(pathNode);
    this.mPathSmoothingTriangleList.Add(iTrianglePath.Count - 1);
    Vector3 iA = vector3_1;
    Vector3 iB1 = iA;
    Vector3 iB2 = iA;
    int num5 = 0;
    int num6 = 0;
    for (int index5 = iTrianglePath.Count - 1; index5 >= 0; --index5)
    {
      uint num7 = iTrianglePath[index5];
      ushort index6 = (ushort) (num7 >> 16 /*0x10*/);
      ushort index7 = (ushort) num7;
      Triangle[] triangleArray4;
      Vector3[] iVertices;
      if (index6 == ushort.MaxValue)
      {
        triangleArray4 = this.mTriangles;
        iVertices = this.mVertices;
      }
      else
      {
        triangleArray4 = this.mAnimatedParts[(int) index6].Triangles;
        iVertices = this.mAnimatedParts[(int) index6].Vertices;
      }
      Vector3 vector3_3;
      Vector3 vector3_4;
      if (index5 > 0)
      {
        if ((int) index6 == (int) (ushort) (iTrianglePath[index5 - 1] >> 16 /*0x10*/))
        {
          triangleArray4[(int) index7].GetPortalPoints(iVertices, (ushort) iTrianglePath[index5 - 1], out vector3_3, out vector3_4);
        }
        else
        {
          Vector3.Add(ref iVertices[(int) triangleArray4[(int) index7].VertexA], ref iVertices[(int) triangleArray4[(int) index7].VertexB], out vector3_3);
          Vector3.Add(ref iVertices[(int) triangleArray4[(int) index7].VertexC], ref vector3_3, out vector3_3);
          Vector3.Multiply(ref vector3_3, 0.333333343f, out vector3_3);
          vector3_4 = vector3_3;
        }
        if (index6 != ushort.MaxValue)
        {
          Vector3.Transform(ref vector3_3, ref this.mAnimatedParts[(int) index6].mTransform, out vector3_3);
          Vector3.Transform(ref vector3_4, ref this.mAnimatedParts[(int) index6].mTransform, out vector3_4);
        }
      }
      else
      {
        vector3_3 = vector3_2;
        vector3_4 = vector3_2;
      }
      float result;
      Vector3.DistanceSquared(ref iA, ref iB2, out result);
      if ((double) result < 1.0 / 1000.0)
      {
        iB2 = vector3_4;
        num6 = index5;
      }
      else if ((double) this.TriArea2D(ref iA, ref iB2, ref vector3_4) <= 0.0)
      {
        if ((double) this.TriArea2D(ref iA, ref iB1, ref vector3_4) > 0.0)
        {
          iB2 = vector3_4;
          num6 = index5;
        }
        else
        {
          iA = iB1;
          int index8 = num5;
          Vector3 position = iStraightPath[iStraightPath.Count - 1].Position;
          Vector3.DistanceSquared(ref position, ref iA, out result);
          if ((double) result >= 1.0 / 1000.0)
          {
            uint num8 = iTrianglePath[index8];
            ushort index9 = (ushort) (num8 >> 16 /*0x10*/);
            ushort index10 = (ushort) num8;
            Triangle[] triangleArray5 = index9 != ushort.MaxValue ? this.mAnimatedParts[(int) index9].Triangles : this.mTriangles;
            pathNode.Position = iA;
            pathNode.Properties = triangleArray5[(int) index10].Properties;
            iStraightPath.Add(pathNode);
            this.mPathSmoothingTriangleList.Add(index5);
          }
          iB1 = iA;
          iB2 = iA;
          num5 = index8;
          num6 = index8;
          index5 = index8;
          continue;
        }
      }
      Vector3.DistanceSquared(ref iA, ref iB1, out result);
      if ((double) result < 1.0 / 1000.0)
      {
        iB1 = vector3_3;
        num5 = index5;
      }
      else if ((double) this.TriArea2D(ref iA, ref iB1, ref vector3_3) >= 0.0)
      {
        if ((double) this.TriArea2D(ref iA, ref iB2, ref vector3_3) < 0.0)
        {
          iB1 = vector3_3;
          num5 = index5;
        }
        else
        {
          iA = iB2;
          int index11 = num6;
          Vector3 position = iStraightPath[iStraightPath.Count - 1].Position;
          Vector3.DistanceSquared(ref position, ref iA, out result);
          if ((double) result >= 1.0 / 1000.0)
          {
            uint num9 = iTrianglePath[index11];
            ushort index12 = (ushort) (num9 >> 16 /*0x10*/);
            ushort index13 = (ushort) num9;
            Triangle[] triangleArray6 = index12 != ushort.MaxValue ? this.mAnimatedParts[(int) index12].Triangles : this.mTriangles;
            pathNode.Position = iA;
            pathNode.Properties = triangleArray6[(int) index13].Properties;
            iStraightPath.Add(pathNode);
            this.mPathSmoothingTriangleList.Add(index5);
          }
          iB1 = iA;
          iB2 = iA;
          num5 = index11;
          num6 = index11;
          index5 = index11;
        }
      }
    }
    Triangle[] triangleArray7;
    if (index3 == ushort.MaxValue)
    {
      triangleArray7 = this.mTriangles;
      vector3Array3 = this.mVertices;
    }
    else
    {
      triangleArray7 = this.mAnimatedParts[(int) index3].Triangles;
      vector3Array3 = this.mAnimatedParts[(int) index3].Vertices;
    }
    pathNode.Position = vector3_2;
    pathNode.Properties = triangleArray7[(int) index4].Properties;
    iStraightPath.Add(pathNode);
    this.mPathSmoothingTriangleList.Add(0);
    for (int index14 = 0; index14 < iStraightPath.Count - 1; ++index14)
    {
      for (int smoothingTriangle = this.mPathSmoothingTriangleList[index14]; smoothingTriangle > this.mPathSmoothingTriangleList[index14 + 1]; --smoothingTriangle)
      {
        uint num10 = iTrianglePath[smoothingTriangle];
        ushort index15 = (ushort) (num10 >> 16 /*0x10*/);
        ushort index16 = (ushort) num10;
        ushort index17 = (ushort) (num10 >> 16 /*0x10*/);
        ushort index18 = (ushort) num10;
        Triangle[] triangleArray8;
        Vector3[] iVertices;
        if (index15 == ushort.MaxValue)
        {
          triangleArray8 = this.mTriangles;
          iVertices = this.mVertices;
        }
        else
        {
          triangleArray8 = this.mAnimatedParts[(int) index15].Triangles;
          iVertices = this.mAnimatedParts[(int) index15].Vertices;
        }
        Triangle[] triangleArray9;
        if (index17 == ushort.MaxValue)
        {
          triangleArray9 = this.mTriangles;
        }
        else
        {
          triangleArray9 = this.mAnimatedParts[(int) index17].Triangles;
          Vector3[] vertices = this.mAnimatedParts[(int) index17].Vertices;
        }
        if (triangleArray8[(int) index16].Properties != triangleArray9[(int) index18].Properties)
        {
          Vector3 vector3_5;
          Vector3 oRight;
          if ((int) index15 == (int) index17)
          {
            triangleArray8[(int) index16].GetPortalPoints(iVertices, (ushort) iTrianglePath[smoothingTriangle - 1], out vector3_5, out oRight);
          }
          else
          {
            Vector3.Add(ref iVertices[(int) triangleArray8[(int) index16].VertexA], ref iVertices[(int) triangleArray8[(int) index16].VertexB], out vector3_5);
            Vector3.Add(ref iVertices[(int) triangleArray8[(int) index16].VertexC], ref vector3_5, out vector3_5);
            Vector3.Multiply(ref vector3_5, 0.333333343f, out vector3_5);
            oRight = vector3_5;
          }
          Vector3 position1 = iStraightPath[index14].Position;
          Vector3 position2 = iStraightPath[index14 + 1].Position;
          Vector3 result1;
          Vector3.Subtract(ref oRight, ref vector3_5, out result1);
          Vector3 result2;
          Vector3.Subtract(ref position2, ref position1, out result2);
          Vector3 result3;
          Vector3.Subtract(ref position1, ref vector3_5, out result3);
          float num11 = (float) ((double) result3.X * (double) result2.Z - (double) result3.Z * (double) result2.X);
          float num12 = (float) ((double) result1.X * (double) result2.Z - (double) result1.Z * (double) result2.X);
          float result4 = (float) ((double) num11 * (double) num12 / ((double) num12 * (double) num12));
          Vector3.Lerp(ref vector3_5, ref oRight, result4, out pathNode.Position);
          pathNode.Properties = triangleArray8[(int) index16].Properties;
          Vector3.DistanceSquared(ref position1, ref pathNode.Position, out result4);
          if ((double) result4 > 1.0 / 1000.0)
            Vector3.DistanceSquared(ref position2, ref pathNode.Position, out result4);
          if ((double) result4 > 1.0 / 1000.0)
          {
            iStraightPath.Insert(index14 + 1, pathNode);
            this.mPathSmoothingTriangleList.Insert(index14 + 1, smoothingTriangle);
            break;
          }
        }
      }
    }
  }

  internal NavMeshOctree Octree => this.mMesh;

  public Triangle[] Triangles => this.mTriangles;

  public Vector3[] Vertices => this.mVertices;

  internal List<AnimatedNavMesh> AnimatedParts => this.mAnimatedParts;
}
