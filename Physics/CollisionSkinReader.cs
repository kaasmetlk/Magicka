// Decompiled with JetBrains decompiler
// Type: Magicka.Physics.CollisionSkinReader
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

#nullable disable
namespace Magicka.Physics;

public class CollisionSkinReader : ContentTypeReader<CollisionSkin>
{
  protected override CollisionSkin Read(ContentReader input, CollisionSkin existingInstance)
  {
    CollisionSkin collisionSkin = new CollisionSkin((Body) null);
    TriangleMesh prim = new TriangleMesh();
    List<Vector3> vertices = new List<Vector3>();
    int num1 = input.ReadInt32();
    for (int index = 0; index < num1; ++index)
      vertices.Add(input.ReadVector3());
    List<TriangleVertexIndices> triangleVertexIndices1 = new List<TriangleVertexIndices>();
    int num2 = input.ReadInt32() / 3;
    for (int index = 0; index < num2; ++index)
    {
      TriangleVertexIndices triangleVertexIndices2;
      triangleVertexIndices2.I2 = input.ReadInt32();
      triangleVertexIndices2.I1 = input.ReadInt32();
      triangleVertexIndices2.I0 = input.ReadInt32();
      triangleVertexIndices1.Add(triangleVertexIndices2);
    }
    prim.CreateMesh(vertices, triangleVertexIndices1, 100, 10f);
    collisionSkin.AddPrimitive((Primitive) prim, 1, new MaterialProperties(0.0f, 1f, 1f));
    return collisionSkin;
  }
}
