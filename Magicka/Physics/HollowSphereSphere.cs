using System;
using JigLibX.Collision;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;

namespace Magicka.Physics
{
	// Token: 0x02000329 RID: 809
	public class HollowSphereSphere : DetectFunctor
	{
		// Token: 0x060018C1 RID: 6337 RVA: 0x000A3593 File Offset: 0x000A1793
		public HollowSphereSphere() : base("HollowSphereSphere", PrimitiveType.NumTypes, PrimitiveType.Sphere)
		{
		}

		// Token: 0x060018C2 RID: 6338 RVA: 0x000A35B0 File Offset: 0x000A17B0
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
			HollowSphere hollowSphere = info.Skin0.GetPrimitiveOldWorld(info.IndexPrim0) as HollowSphere;
			HollowSphere hollowSphere2 = info.Skin0.GetPrimitiveNewWorld(info.IndexPrim0) as HollowSphere;
			Sphere sphere = info.Skin1.GetPrimitiveOldWorld(info.IndexPrim1) as Sphere;
			Sphere sphere2 = info.Skin1.GetPrimitiveNewWorld(info.IndexPrim1) as Sphere;
			Vector3 value3 = hollowSphere.Position - sphere.Position;
			Vector3 value4 = hollowSphere2.Position - sphere2.Position;
			Vector3 vector = -value4;
			vector = Vector3.TransformNormal(vector, hollowSphere2.InverseTransformMatrix);
			vector.Normalize();
			if (MagickaMath.Angle(vector, Vector3.Forward) > hollowSphere2.MaxAngle)
			{
				return;
			}
			float num = value3.LengthSquared();
			float num2 = value4.LengthSquared();
			float num3 = hollowSphere2.Radius + sphere2.Radius;
			float num4 = hollowSphere2.Radius + collTolerance;
			num4 *= num4;
			float num5 = num3 + collTolerance;
			num5 *= num5;
			float num6 = hollowSphere2.Radius - sphere2.Radius + collTolerance;
			num6 *= num6;
			if (num < num4 && num2 > num6)
			{
				float num7 = (float)Math.Sqrt((double)num);
				float num8 = num7 + sphere2.Radius - hollowSphere2.Radius;
				if (num7 > 1E-06f)
				{
					value3 /= num7;
				}
				else
				{
					value3 = Vector3.Transform(Vector3.Backward, Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians((float)this.random.Next(360))));
				}
				Vector3 value5 = sphere.Position + (sphere.Radius - 0.5f * num8) * value3;
				value3 *= -1f;
				SmallCollPointInfo smallCollPointInfo = new SmallCollPointInfo(value5 - value, value5 - value2, num8);
				collisionFunctor.CollisionNotify(ref info, ref value3, &smallCollPointInfo, 1);
				return;
			}
			if (num > num4 && num2 < num5)
			{
				float num9 = (float)Math.Sqrt((double)num);
				float num10 = num3 - num9;
				if (num9 > 1E-06f)
				{
					value3 /= num9;
				}
				else
				{
					value3 = Vector3.Transform(Vector3.Backward, Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians((float)this.random.Next(360))));
				}
				Vector3 value6 = sphere.Position + (sphere.Radius - 0.5f * num10) * value3;
				SmallCollPointInfo smallCollPointInfo2 = new SmallCollPointInfo(value6 - value, value6 - value2, num10);
				collisionFunctor.CollisionNotify(ref info, ref value3, &smallCollPointInfo2, 1);
			}
		}

		// Token: 0x04001A96 RID: 6806
		private Random random = new Random();
	}
}
