using System;
using JigLibX.Collision;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;

namespace Magicka.Physics
{
	// Token: 0x02000596 RID: 1430
	public class HollowSphereBox : DetectFunctor
	{
		// Token: 0x06002AAE RID: 10926 RVA: 0x00150FC1 File Offset: 0x0014F1C1
		public HollowSphereBox() : base("HollowSphereBox", PrimitiveType.NumTypes, PrimitiveType.Box)
		{
		}

		// Token: 0x06002AAF RID: 10927 RVA: 0x00150FDC File Offset: 0x0014F1DC
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
			Box box = info.Skin1.GetPrimitiveOldWorld(info.IndexPrim1) as Box;
			Box box2 = info.Skin1.GetPrimitiveNewWorld(info.IndexPrim1) as Box;
			Vector3 vector;
			box.GetCentre(out vector);
			Vector3 vector2;
			box.GetHalfSideLengths(out vector2);
			float num = vector2.Length();
			Vector3 value3;
			box2.GetCentre(out value3);
			box2.GetHalfSideLengths(out vector2);
			float num2 = vector2.Length();
			Vector3 value4 = hollowSphere.Position - vector;
			Vector3 value5 = hollowSphere2.Position - value3;
			Vector3 vector3 = -value5;
			vector3 = Vector3.TransformNormal(vector3, hollowSphere2.InverseTransformMatrix);
			vector3.Normalize();
			if (MagickaMath.Angle(vector3, Vector3.Forward) > hollowSphere2.MaxAngle)
			{
				return;
			}
			float num3 = value4.LengthSquared();
			float num4 = value5.LengthSquared();
			float num5 = hollowSphere2.Radius + num2;
			float num6 = hollowSphere2.Radius + collTolerance;
			num6 *= num6;
			float num7 = num5 + collTolerance;
			num7 *= num7;
			float num8 = hollowSphere2.Radius - num2 + collTolerance;
			num8 *= num8;
			if (num3 < num6 && num4 > num8)
			{
				float num9 = (float)Math.Sqrt((double)num3);
				float num10 = num9 + num2 - hollowSphere2.Radius;
				if (num9 > 1E-06f)
				{
					value4 /= num9;
				}
				else
				{
					value4 = Vector3.Transform(Vector3.Backward, Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians((float)this.random.Next(360))));
				}
				Vector3 value6 = vector + (num - 0.5f * num10) * value4;
				value4 *= -1f;
				SmallCollPointInfo smallCollPointInfo = new SmallCollPointInfo(value6 - value, value6 - value2, num10);
				collisionFunctor.CollisionNotify(ref info, ref value4, &smallCollPointInfo, 1);
				return;
			}
			if (num3 > num6 && num4 < num7)
			{
				float num11 = (float)Math.Sqrt((double)num3);
				float num12 = num5 - num11;
				if (num11 > 1E-06f)
				{
					value4 /= num11;
				}
				else
				{
					value4 = Vector3.Transform(Vector3.Backward, Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians((float)this.random.Next(360))));
				}
				Vector3 value7 = vector + (num - 0.5f * num12) * value4;
				SmallCollPointInfo smallCollPointInfo2 = new SmallCollPointInfo(value7 - value, value7 - value2, num12);
				collisionFunctor.CollisionNotify(ref info, ref value4, &smallCollPointInfo2, 1);
			}
		}

		// Token: 0x04002E02 RID: 11778
		private Random random = new Random();
	}
}
