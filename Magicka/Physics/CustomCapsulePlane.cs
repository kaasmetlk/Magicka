using System;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using Microsoft.Xna.Framework;

namespace Magicka.Physics
{
	// Token: 0x02000473 RID: 1139
	internal class CustomCapsulePlane : DetectFunctor
	{
		// Token: 0x0600226A RID: 8810 RVA: 0x000F70A6 File Offset: 0x000F52A6
		public CustomCapsulePlane() : base("CustomCapsulePlane", PrimitiveType.Capsule, PrimitiveType.Plane)
		{
		}

		// Token: 0x0600226B RID: 8811 RVA: 0x000F70B8 File Offset: 0x000F52B8
		public unsafe override void CollDetect(CollDetectInfo info, float collTolerance, ICollisionFunctor collisionFunctor)
		{
			if (info.Skin0.GetPrimitiveOldWorld(info.IndexPrim0).Type == base.Type1)
			{
				CollisionSkin skin = info.Skin0;
				info.Skin0 = info.Skin1;
				info.Skin1 = skin;
				int indexPrim = info.IndexPrim0;
				info.IndexPrim0 = info.IndexPrim1;
				info.IndexPrim1 = indexPrim;
			}
			Vector3 value = (info.Skin0.Owner != null) ? info.Skin0.Owner.OldPosition : Vector3.Zero;
			Vector3 value2 = (info.Skin1.Owner != null) ? info.Skin1.Owner.OldPosition : Vector3.Zero;
			Capsule capsule = (Capsule)info.Skin0.GetPrimitiveOldWorld(info.IndexPrim0);
			Capsule capsule2 = (Capsule)info.Skin0.GetPrimitiveNewWorld(info.IndexPrim0);
			JigLibX.Geometry.Plane plane = (JigLibX.Geometry.Plane)info.Skin1.GetPrimitiveOldWorld(info.IndexPrim1);
			JigLibX.Geometry.Plane plane2 = (JigLibX.Geometry.Plane)info.Skin1.GetPrimitiveNewWorld(info.IndexPrim1);
			Matrix inverseTransformMatrix = plane2.InverseTransformMatrix;
			Matrix inverseTransformMatrix2 = plane.InverseTransformMatrix;
			SmallCollPointInfo[] array = DetectFunctor.SCPIStackAlloc();
			fixed (SmallCollPointInfo* ptr = array)
			{
				int num = 0;
				Vector3 vector = Vector3.Transform(capsule.Position, inverseTransformMatrix2);
				Vector3 vector2 = Vector3.Transform(capsule2.Position, inverseTransformMatrix);
				float num2 = Distance.PointPlaneDistance(ref vector, plane);
				float value3 = Distance.PointPlaneDistance(ref vector2, plane2);
				if (MathHelper.Min(value3, num2) < collTolerance + capsule2.Radius)
				{
					float initialPenetration = capsule.Radius - num2;
					Vector3 value4 = capsule.Position - capsule.Radius * plane.Normal;
					ptr[(IntPtr)(num++) * (IntPtr)sizeof(SmallCollPointInfo)] = new SmallCollPointInfo(value4 - value, value4 - value2, initialPenetration);
				}
				Vector3 vector3 = Vector3.Transform(capsule.GetEnd(), inverseTransformMatrix2);
				Vector3 vector4 = Vector3.Transform(capsule2.GetEnd(), inverseTransformMatrix);
				float num3 = Distance.PointPlaneDistance(ref vector3, plane);
				float val = Distance.PointPlaneDistance(ref vector4, plane2);
				if (Math.Min(val, num3) < collTolerance + capsule2.Radius)
				{
					float initialPenetration2 = capsule.Radius - num3;
					Vector3 value5 = capsule.GetEnd() - capsule.Radius * plane.Normal;
					ptr[(IntPtr)(num++) * (IntPtr)sizeof(SmallCollPointInfo)] = new SmallCollPointInfo(value5 - value, value5 - value2, initialPenetration2);
				}
				if (num > 0)
				{
					Vector3 normal = plane.Normal;
					normal.Y = 0f;
					JiggleMath.NormalizeSafe(ref normal);
					collisionFunctor.CollisionNotify(ref info, ref normal, ptr, num);
				}
			}
			DetectFunctor.FreeStackAlloc(array);
		}
	}
}
