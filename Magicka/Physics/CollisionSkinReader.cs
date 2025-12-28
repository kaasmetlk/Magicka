using System;
using System.Collections.Generic;
using JigLibX.Collision;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.Physics
{
	// Token: 0x020001A3 RID: 419
	public class CollisionSkinReader : ContentTypeReader<CollisionSkin>
	{
		// Token: 0x06000C67 RID: 3175 RVA: 0x0004A7E4 File Offset: 0x000489E4
		protected override CollisionSkin Read(ContentReader input, CollisionSkin existingInstance)
		{
			CollisionSkin collisionSkin = new CollisionSkin(null);
			TriangleMesh triangleMesh = new TriangleMesh();
			List<Vector3> list = new List<Vector3>();
			int num = input.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				list.Add(input.ReadVector3());
			}
			List<TriangleVertexIndices> list2 = new List<TriangleVertexIndices>();
			int num2 = input.ReadInt32() / 3;
			for (int j = 0; j < num2; j++)
			{
				TriangleVertexIndices item;
				item.I2 = input.ReadInt32();
				item.I1 = input.ReadInt32();
				item.I0 = input.ReadInt32();
				list2.Add(item);
			}
			triangleMesh.CreateMesh(list, list2, 100, 10f);
			collisionSkin.AddPrimitive(triangleMesh, 1, new MaterialProperties(0f, 1f, 1f));
			return collisionSkin;
		}
	}
}
