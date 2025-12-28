using System;
using JigLibX.Geometry;
using JigLibX.Math;
using Microsoft.Xna.Framework;

namespace Magicka.Physics
{
	// Token: 0x02000139 RID: 313
	public class HollowSphere : Primitive
	{
		// Token: 0x060008D7 RID: 2263 RVA: 0x00038967 File Offset: 0x00036B67
		public HollowSphere(Vector3 pos, float radius) : base(PrimitiveType.NumTypes)
		{
			this.transform.Position = pos;
			this.radius = radius;
			this.maxAngle = 3.1415927f;
		}

		// Token: 0x060008D8 RID: 2264 RVA: 0x0003898E File Offset: 0x00036B8E
		public override Primitive Clone()
		{
			return new HollowSphere(this.transform.Position, this.radius);
		}

		// Token: 0x060008D9 RID: 2265 RVA: 0x000389A8 File Offset: 0x00036BA8
		public bool ArcIntersect(ref Vector3 iOrigin, ref Vector3 iDelta, float iSpread, out Vector3 oPos)
		{
			Segment seg;
			seg.Origin = iOrigin;
			seg.Delta = iDelta;
			float scaleFactor;
			bool result = this.SegmentHollowSphereIntersection(out scaleFactor, seg);
			Vector3.Multiply(ref seg.Delta, scaleFactor, out seg.Delta);
			Vector3.Add(ref seg.Origin, ref seg.Delta, out oPos);
			return result;
		}

		// Token: 0x060008DA RID: 2266 RVA: 0x00038A04 File Offset: 0x00036C04
		public bool SegmentHollowSphereIntersection(out float ts, Segment seg)
		{
			Vector3 position = this.Position;
			Vector3.Subtract(ref position, ref seg.Origin, out position);
			float num;
			Vector3.Dot(ref seg.Delta, ref seg.Delta, out num);
			float num2;
			Vector3.Dot(ref seg.Delta, ref position, out num2);
			float num3;
			Vector3.Dot(ref position, ref position, out num3);
			num3 -= this.radius * this.radius;
			float num4 = (float)Math.Sqrt((double)(num2 * num2 - num * num3));
			float num5 = 1f / num;
			float num6 = (num2 + num4) * num5;
			float num7 = (num2 - num4) * num5;
			bool flag = num6 > 0f & num6 < 1f & !float.IsNaN(num6);
			bool flag2 = num7 > 0f & num7 < 1f & !float.IsNaN(num7);
			if (!flag & !flag2)
			{
				ts = 0f;
				return false;
			}
			Vector3 forward = this.transform.Orientation.Forward;
			Vector3 vector;
			seg.GetPoint(num6, out vector);
			Vector3.Subtract(ref vector, ref this.transform.Position, out vector);
			vector.Normalize();
			float num8;
			Vector3.Dot(ref vector, ref forward, out num8);
			num8 = (float)Math.Acos((double)num8);
			Vector3 vector2;
			seg.GetPoint(num7, out vector2);
			Vector3.Subtract(ref vector2, ref this.transform.Position, out vector2);
			vector2.Normalize();
			float num9;
			Vector3.Dot(ref vector2, ref forward, out num9);
			num9 = (float)Math.Acos((double)num9);
			if (num8 > this.maxAngle)
			{
				flag = false;
			}
			if (num9 > this.maxAngle)
			{
				flag2 = false;
			}
			if (flag & (num6 < num7 | !flag2))
			{
				ts = num6;
			}
			else
			{
				if (!(flag2 & (num7 < num6 | !flag)))
				{
					ts = 0f;
					return false;
				}
				ts = num7;
			}
			return true;
		}

		// Token: 0x060008DB RID: 2267 RVA: 0x00038BC8 File Offset: 0x00036DC8
		public override bool SegmentIntersect(out float frac, out Vector3 pos, out Vector3 normal, ref Segment seg)
		{
			bool flag = this.SegmentHollowSphereIntersection(out frac, seg);
			if (flag)
			{
				seg.GetPoint(frac, out pos);
				normal = pos - this.transform.Position;
				JiggleMath.NormalizeSafe(ref normal);
			}
			else
			{
				pos = Vector3.Zero;
				normal = Vector3.Zero;
			}
			return flag;
		}

		// Token: 0x060008DC RID: 2268 RVA: 0x00038C2C File Offset: 0x00036E2C
		public override void GetMassProperties(PrimitiveProperties primitiveProperties, out float mass, out Vector3 centerOfMass, out Matrix inertiaTensor)
		{
			if (primitiveProperties.MassType == PrimitiveProperties.MassTypeEnum.Mass)
			{
				mass = primitiveProperties.MassOrDensity;
			}
			else if (primitiveProperties.MassDistribution == PrimitiveProperties.MassDistributionEnum.Solid)
			{
				mass = this.GetVolume() * primitiveProperties.MassOrDensity;
			}
			else
			{
				mass = this.GetSurfaceArea() * primitiveProperties.MassOrDensity;
			}
			centerOfMass = this.transform.Position;
			float m;
			if (primitiveProperties.MassDistribution == PrimitiveProperties.MassDistributionEnum.Solid)
			{
				m = 0.4f * mass * this.radius;
			}
			else
			{
				m = 0.6666667f * mass * this.radius;
			}
			inertiaTensor = Matrix.Identity;
			inertiaTensor.M11 = (inertiaTensor.M22 = (inertiaTensor.M33 = m));
			inertiaTensor.M11 += mass * (centerOfMass.Y * centerOfMass.Y + centerOfMass.Z * centerOfMass.Z);
			inertiaTensor.M22 += mass * (centerOfMass.Z * centerOfMass.Z + centerOfMass.X * centerOfMass.X);
			inertiaTensor.M33 += mass * (centerOfMass.X * centerOfMass.X + centerOfMass.Y * centerOfMass.Y);
			inertiaTensor.M12 = (inertiaTensor.M21 = inertiaTensor.M12 - mass * centerOfMass.X * centerOfMass.Y);
			inertiaTensor.M23 = (inertiaTensor.M32 = inertiaTensor.M23 - mass * centerOfMass.Y * centerOfMass.Z);
			inertiaTensor.M31 = (inertiaTensor.M13 = inertiaTensor.M31 - mass * centerOfMass.Z * centerOfMass.X);
		}

		// Token: 0x060008DD RID: 2269 RVA: 0x00038DE2 File Offset: 0x00036FE2
		public override float GetVolume()
		{
			return 4.1887903f * this.radius * this.radius * this.radius;
		}

		// Token: 0x060008DE RID: 2270 RVA: 0x00038DFE File Offset: 0x00036FFE
		public override float GetSurfaceArea()
		{
			return 12.566371f * this.radius * this.radius;
		}

		// Token: 0x170001C3 RID: 451
		// (get) Token: 0x060008DF RID: 2271 RVA: 0x00038E13 File Offset: 0x00037013
		// (set) Token: 0x060008E0 RID: 2272 RVA: 0x00038E20 File Offset: 0x00037020
		public Vector3 Position
		{
			get
			{
				return this.transform.Position;
			}
			set
			{
				this.transform.Position = value;
			}
		}

		// Token: 0x170001C4 RID: 452
		// (get) Token: 0x060008E1 RID: 2273 RVA: 0x00038E2E File Offset: 0x0003702E
		// (set) Token: 0x060008E2 RID: 2274 RVA: 0x00038E36 File Offset: 0x00037036
		public float Radius
		{
			get
			{
				return this.radius;
			}
			set
			{
				this.radius = value;
			}
		}

		// Token: 0x170001C5 RID: 453
		// (get) Token: 0x060008E3 RID: 2275 RVA: 0x00038E3F File Offset: 0x0003703F
		// (set) Token: 0x060008E4 RID: 2276 RVA: 0x00038E47 File Offset: 0x00037047
		public float MaxAngle
		{
			get
			{
				return this.maxAngle;
			}
			set
			{
				this.maxAngle = value;
			}
		}

		// Token: 0x060008E5 RID: 2277 RVA: 0x00038E50 File Offset: 0x00037050
		public override void GetBoundingBox(out AABox box)
		{
			Vector3 position = this.transform.Position;
			position.X -= this.radius + HollowSphere.THICKNESS * 2f;
			position.Y -= this.radius + HollowSphere.THICKNESS * 2f;
			position.Z -= this.radius + HollowSphere.THICKNESS * 2f;
			Vector3 position2 = this.transform.Position;
			position2.X += this.radius + HollowSphere.THICKNESS * 2f;
			position2.Y += this.radius + HollowSphere.THICKNESS * 2f;
			position2.Z += this.radius + HollowSphere.THICKNESS * 2f;
			box = new AABox(position, position2);
		}

		// Token: 0x04000838 RID: 2104
		private float radius;

		// Token: 0x04000839 RID: 2105
		public static float THICKNESS = 0.5f;

		// Token: 0x0400083A RID: 2106
		private float maxAngle;
	}
}
