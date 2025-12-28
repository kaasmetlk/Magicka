using System;
using JigLibX.Collision;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;

namespace Magicka.Physics
{
	// Token: 0x02000205 RID: 517
	public class HollowSphereCapsule : DetectFunctor
	{
		// Token: 0x060010ED RID: 4333 RVA: 0x00069A50 File Offset: 0x00067C50
		public HollowSphereCapsule() : base("HollowSphereCapsule", PrimitiveType.NumTypes, PrimitiveType.Capsule)
		{
		}

		// Token: 0x060010EE RID: 4334 RVA: 0x00069A6C File Offset: 0x00067C6C
		public unsafe override void CollDetect(CollDetectInfo infoOrig, float collTolerance, ICollisionFunctor collisionFunctor)
		{
			CollDetectInfo collDetectInfo = infoOrig;
			if (collDetectInfo.Skin0.GetPrimitiveOldWorld(collDetectInfo.IndexPrim0).Type == base.Type1)
			{
				CollisionSkin skin = collDetectInfo.Skin0;
				collDetectInfo.Skin0 = collDetectInfo.Skin1;
				collDetectInfo.Skin1 = skin;
				int indexPrim = collDetectInfo.IndexPrim0;
				collDetectInfo.IndexPrim0 = collDetectInfo.IndexPrim1;
				collDetectInfo.IndexPrim1 = indexPrim;
			}
			Vector3 value = (collDetectInfo.Skin0.Owner != null) ? collDetectInfo.Skin0.Owner.OldPosition : Vector3.Zero;
			Vector3 value2 = (collDetectInfo.Skin1.Owner != null) ? collDetectInfo.Skin1.Owner.OldPosition : Vector3.Zero;
			HollowSphere hollowSphere = collDetectInfo.Skin0.GetPrimitiveOldWorld(collDetectInfo.IndexPrim0) as HollowSphere;
			HollowSphere hollowSphere2 = collDetectInfo.Skin0.GetPrimitiveNewWorld(collDetectInfo.IndexPrim0) as HollowSphere;
			Capsule capsule = collDetectInfo.Skin1.GetPrimitiveOldWorld(collDetectInfo.IndexPrim1) as Capsule;
			Capsule capsule2 = collDetectInfo.Skin1.GetPrimitiveNewWorld(collDetectInfo.IndexPrim1) as Capsule;
			Segment seg = new Segment(capsule.Position, capsule.Length * capsule.Orientation.Backward);
			Segment seg2 = new Segment(capsule.Position, capsule2.Length * capsule2.Orientation.Backward);
			Vector3 vector = capsule2.Position - hollowSphere2.Position;
			vector = Vector3.TransformNormal(vector, hollowSphere2.InverseTransformMatrix);
			vector.Normalize();
			if (MagickaMath.Angle(vector, Vector3.Forward) > hollowSphere2.MaxAngle)
			{
				return;
			}
			float num = capsule2.Radius + hollowSphere2.Radius;
			float t;
			float num2 = Distance.PointSegmentDistanceSq(out t, hollowSphere.Position, seg);
			float num4;
			float num3 = Distance.PointSegmentDistanceSq(out num4, hollowSphere2.Position, seg2);
			float num5 = hollowSphere2.Radius + collTolerance;
			num5 *= num5;
			float num6 = num + HollowSphere.THICKNESS + collTolerance;
			num6 *= num6;
			float num7 = hollowSphere2.Radius - capsule2.Radius - HollowSphere.THICKNESS + collTolerance;
			num7 *= num7;
			if (num2 < num5 && num3 > num7)
			{
				Vector3 value3;
				seg.GetPoint(t, out value3);
				Vector3 value4 = value3 - hollowSphere.Position;
				float num8 = (float)Math.Sqrt((double)num2);
				float num9 = num8 + capsule2.Radius - hollowSphere2.Radius + HollowSphere.THICKNESS;
				if (num8 > 1E-06f)
				{
					value4 /= num8;
				}
				else
				{
					value4 = Vector3.Transform(Vector3.Backward, Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians((float)this.random.Next(360))));
				}
				Vector3 value5 = value3 + (capsule.Radius - 0.5f * num9) * value4;
				SmallCollPointInfo smallCollPointInfo = new SmallCollPointInfo(value5 - value, value5 - value2, num9);
				collisionFunctor.CollisionNotify(ref collDetectInfo, ref value4, &smallCollPointInfo, 1);
				return;
			}
			if (num2 > num5 && num3 < num6)
			{
				Vector3 vector2;
				seg.GetPoint(t, out vector2);
				Vector3 value6 = hollowSphere.Position - vector2;
				float num10 = (float)Math.Sqrt((double)num2);
				float num11 = num - num10 + HollowSphere.THICKNESS;
				if (num10 > 1E-06f)
				{
					value6 /= num10;
				}
				else
				{
					value6 = Vector3.Transform(Vector3.Backward, Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians((float)this.random.Next(360))));
				}
				Vector3 value7 = vector2 + (capsule.Radius - 0.5f * num11) * value6;
				SmallCollPointInfo smallCollPointInfo2 = new SmallCollPointInfo(value7 - value, value7 - value2, num11);
				collisionFunctor.CollisionNotify(ref collDetectInfo, ref value6, &smallCollPointInfo2, 1);
			}
		}

		// Token: 0x04000F74 RID: 3956
		private Random random = new Random();
	}
}
