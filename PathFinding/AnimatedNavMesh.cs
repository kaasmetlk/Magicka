// Decompiled with JetBrains decompiler
// Type: Magicka.PathFinding.AnimatedNavMesh
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

#nullable disable
namespace Magicka.PathFinding;

internal class AnimatedNavMesh
{
  public const float MAXDISTANCE = 0.25f;
  public const float MAXDISTANCESQ = 0.0625f;
  private static Vector3 BBBias = new Vector3(0.25f);
  internal Matrix mTransform = Matrix.Identity;
  internal Matrix mInvTransform = Matrix.Identity;
  private Dictionary<ushort, List<NeighbourInfo>> mDynamicNeighbours = new Dictionary<ushort, List<NeighbourInfo>>();
  private Vector3[] mVertices;
  private Triangle[] mTriangles;
  private NavMeshOctree mMesh;
  private bool mDirty = true;

  public AnimatedNavMesh(ContentReader iInput)
  {
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

  public void GetNearestPosition(
    ref Vector3 iPoint,
    out Vector3 oPoint,
    MovementProperties iProperties)
  {
    double closestPoint = (double) this.mMesh.FindClosestPoint(ref iPoint, out oPoint, out int _, iProperties);
  }

  public void UpdateTransform(ref Matrix iTransform)
  {
    this.mDirty = true;
    this.mTransform = iTransform;
    Matrix.Invert(ref iTransform, out this.mInvTransform);
  }

  internal void ClearNeighbours()
  {
    foreach (List<NeighbourInfo> neighbourInfoList in this.mDynamicNeighbours.Values)
      neighbourInfoList.Clear();
  }

  internal NavMeshOctree Mesh => this.mMesh;

  internal Dictionary<ushort, List<NeighbourInfo>> DynamicNeighbours => this.mDynamicNeighbours;

  public Triangle[] Triangles => this.mTriangles;

  public Vector3[] Vertices => this.mVertices;

  public bool Dirty => this.mDirty;

  internal void FindNeighbours(ushort iThisId, List<AnimatedNavMesh> iMeshes)
  {
    this.mDirty = false;
    Vector3 vector3 = new Vector3();
    vector3.X = vector3.Y = vector3.Z = 0.25f;
    foreach (KeyValuePair<ushort, List<NeighbourInfo>> dynamicNeighbour in this.mDynamicNeighbours)
    {
      Vector3 result1 = this.mVertices[(int) this.mTriangles[(int) dynamicNeighbour.Key].VertexA];
      Vector3 result2 = this.mVertices[(int) this.mTriangles[(int) dynamicNeighbour.Key].VertexB];
      Vector3 result3 = this.mVertices[(int) this.mTriangles[(int) dynamicNeighbour.Key].VertexC];
      Vector3.Transform(ref result1, ref this.mTransform, out result1);
      Vector3.Transform(ref result2, ref this.mTransform, out result2);
      Vector3.Transform(ref result3, ref this.mTransform, out result3);
      for (int index = (int) iThisId + 1; index < iMeshes.Count; ++index)
      {
        AnimatedNavMesh iMesh = iMeshes[index];
        Vector3 result4;
        Vector3.Transform(ref result1, ref iMesh.mInvTransform, out result4);
        Vector3 result5;
        Vector3.Transform(ref result2, ref iMesh.mInvTransform, out result5);
        Vector3 result6;
        Vector3.Transform(ref result3, ref iMesh.mInvTransform, out result6);
        this.FindNeighbours(dynamicNeighbour.Key, ref result4, ref result5, ref result6, dynamicNeighbour.Value, iMesh, iThisId, (ushort) index);
      }
    }
  }

  private unsafe void FindNeighbours(
    ushort iTri,
    ref Vector3 iA,
    ref Vector3 iB,
    ref Vector3 iC,
    List<NeighbourInfo> iNeighbours,
    AnimatedNavMesh iMesh,
    ushort iThisId,
    ushort iMeshID)
  {
    BoundingBox testBox;
    Vector3.Max(ref iA, ref iB, out testBox.Max);
    Vector3.Max(ref iC, ref testBox.Max, out testBox.Max);
    Vector3.Add(ref testBox.Max, ref AnimatedNavMesh.BBBias, out testBox.Max);
    Vector3.Min(ref iA, ref iB, out testBox.Min);
    Vector3.Min(ref iC, ref testBox.Min, out testBox.Min);
    Vector3.Subtract(ref testBox.Min, ref AnimatedNavMesh.BBBias, out testBox.Min);
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
        Vector3.Transform(ref neighbourInfo.EdgePos, ref this.mTransform, out neighbourInfo.EdgePos);
        iNeighbours.Add(neighbourInfo);
        neighbourInfo.Triangle = iTri;
        neighbourInfo.Mesh = iThisId;
        iMesh.DynamicNeighbours[(ushort) alloced[index]].Add(neighbourInfo);
      }
    }
    DetectFunctor.FreeStackAlloc(alloced);
  }
}
