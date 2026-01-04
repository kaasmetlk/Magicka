// Decompiled with JetBrains decompiler
// Type: Magicka.PathFinding.Triangle
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;

#nullable disable
namespace Magicka.PathFinding;

internal struct Triangle
{
  public ushort NeighbourA;
  public ushort NeighbourB;
  public ushort NeighbourC;
  public ushort VertexA;
  public ushort VertexB;
  public ushort VertexC;
  public float CostAB;
  public float CostBC;
  public float CostCA;
  public MovementProperties Properties;

  public Triangle(ContentReader iInput)
  {
    this.VertexA = iInput.ReadUInt16();
    this.VertexB = iInput.ReadUInt16();
    this.VertexC = iInput.ReadUInt16();
    this.NeighbourA = iInput.ReadUInt16();
    this.NeighbourB = iInput.ReadUInt16();
    this.NeighbourC = iInput.ReadUInt16();
    this.CostAB = iInput.ReadSingle();
    this.CostBC = iInput.ReadSingle();
    this.CostCA = iInput.ReadSingle();
    this.Properties = (MovementProperties) iInput.ReadByte();
  }

  public float GetCostFrom(ushort iParent, byte iTargetEdge)
  {
    switch (iTargetEdge)
    {
      case 0:
        if ((int) iParent == (int) this.NeighbourB)
          return this.CostAB;
        if ((int) iParent == (int) this.NeighbourC)
          return this.CostCA;
        break;
      case 1:
        if ((int) iParent == (int) this.NeighbourA)
          return this.CostAB;
        if ((int) iParent == (int) this.NeighbourC)
          return this.CostBC;
        break;
      case 2:
        if ((int) iParent == (int) this.NeighbourA)
          return this.CostCA;
        if ((int) iParent == (int) this.NeighbourB)
          return this.CostBC;
        break;
      default:
        throw new ArgumentException("Invalid edge index! Must be >= 0 and < 3");
    }
    throw new Exception("iParent and iTargetEdge refers to the same edge!");
  }

  internal void GetPortalPoints(
    Vector3[] iVertices,
    ushort iNeighbour,
    out Vector3 oLeft,
    out Vector3 oRight)
  {
    if ((int) iNeighbour == (int) this.NeighbourA)
    {
      oRight = iVertices[(int) this.VertexA];
      oLeft = iVertices[(int) this.VertexB];
    }
    else if ((int) iNeighbour == (int) this.NeighbourB)
    {
      oRight = iVertices[(int) this.VertexB];
      oLeft = iVertices[(int) this.VertexC];
    }
    else
    {
      if ((int) iNeighbour != (int) this.NeighbourC)
        throw new Exception("Invalid neighbour!");
      oRight = iVertices[(int) this.VertexC];
      oLeft = iVertices[(int) this.VertexA];
    }
  }

  public void GetCenter(Vector3[] iVertices, out Vector3 oCenter)
  {
    Vector3.Add(ref iVertices[(int) this.VertexA], ref iVertices[(int) this.VertexB], out oCenter);
    Vector3.Add(ref iVertices[(int) this.VertexC], ref oCenter, out oCenter);
    Vector3.Multiply(ref oCenter, 0.333333f, out oCenter);
  }
}
