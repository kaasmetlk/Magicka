using System;
using JigLibX.Collision;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;

namespace Magicka.Physics
{
	// Token: 0x02000474 RID: 1140
	public class CustomCapsuleCapsule : DetectFunctor
	{
		// Token: 0x0600226C RID: 8812 RVA: 0x000F7397 File Offset: 0x000F5597
		public CustomCapsuleCapsule() : base("CustomCapsuleCapsule", PrimitiveType.Capsule, PrimitiveType.Capsule)
		{
		}

		// Token: 0x0600226D RID: 8813 RVA: 0x000F73B4 File Offset: 0x000F55B4
		public unsafe override void CollDetect(CollDetectInfo info, float collTolerance, ICollisionFunctor collisionFunctor)
		{
			Vector3 value = (info.Skin0.Owner != null) ? info.Skin0.Owner.OldPosition : Vector3.Zero;
			Vector3 value2 = (info.Skin1.Owner != null) ? info.Skin1.Owner.OldPosition : Vector3.Zero;
			Capsule capsule = info.Skin0.GetPrimitiveOldWorld(info.IndexPrim0) as Capsule;
			Capsule capsule2 = info.Skin0.GetPrimitiveNewWorld(info.IndexPrim0) as Capsule;
			Segment seg = new Segment(capsule.Position, capsule.Length * capsule.Orientation.Backward);
			Segment seg2 = new Segment(capsule2.Position, capsule2.Length * capsule2.Orientation.Backward);
			Capsule capsule3 = info.Skin1.GetPrimitiveOldWorld(info.IndexPrim1) as Capsule;
			Capsule capsule4 = info.Skin1.GetPrimitiveNewWorld(info.IndexPrim1) as Capsule;
			Segment seg3 = new Segment(capsule3.Position, capsule3.Length * capsule3.Orientation.Backward);
			Segment seg4 = new Segment(capsule4.Position, capsule4.Length * capsule4.Orientation.Backward);
			float num = capsule2.Radius + capsule4.Radius;
			float t;
			float t2;
			float num2 = Distance.SegmentSegmentDistanceSq(out t, out t2, seg, seg3);
			float num3;
			float num4;
			float val = Distance.SegmentSegmentDistanceSq(out num3, out num4, seg2, seg4);
			if (Math.Min(num2, val) < (num + collTolerance) * (num + collTolerance))
			{
				Vector3 value3;
				seg.GetPoint(t, out value3);
				Vector3 vector;
				seg3.GetPoint(t2, out vector);
				value3.Y = (value3.Y + vector.Y) * 0.5f;
				vector.Y = value3.Y;
				Vector3 value4 = value3 - vector;
				float num5 = (float)Math.Sqrt((double)num2);
				float num6 = num - num5;
				if (num5 > 1E-06f)
				{
					value4 /= num5;
				}
				else
				{
					value4 = Vector3.Transform(Vector3.Backward, Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians((float)this.random.Next(360))));
				}
				Vector3 value5 = vector + (capsule3.Radius - 0.5f * num6) * value4;
				SmallCollPointInfo smallCollPointInfo = new SmallCollPointInfo(value5 - value, value5 - value2, num6);
				collisionFunctor.CollisionNotify(ref info, ref value4, &smallCollPointInfo, 1);
			}
		}

		// Token: 0x040025BF RID: 9663
		private Random random = new Random();
	}
}
