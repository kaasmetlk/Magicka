using System;
using JigLibX.Geometry;
using JigLibX.Physics;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities
{
	// Token: 0x020000DB RID: 219
	public interface IDamageable
	{
		// Token: 0x1700014C RID: 332
		// (get) Token: 0x06000699 RID: 1689
		ushort Handle { get; }

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x0600069A RID: 1690
		bool Dead { get; }

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x0600069B RID: 1691
		Vector3 Position { get; }

		// Token: 0x1700014F RID: 335
		// (get) Token: 0x0600069C RID: 1692
		float HitPoints { get; }

		// Token: 0x17000150 RID: 336
		// (get) Token: 0x0600069D RID: 1693
		float MaxHitPoints { get; }

		// Token: 0x17000151 RID: 337
		// (get) Token: 0x0600069E RID: 1694
		float Radius { get; }

		// Token: 0x17000152 RID: 338
		// (get) Token: 0x0600069F RID: 1695
		Body Body { get; }

		// Token: 0x060006A0 RID: 1696
		bool SegmentIntersect(out Vector3 oPosition, Segment iSeg, float iSegmentRadius);

		// Token: 0x060006A1 RID: 1697
		DamageResult InternalDamage(DamageCollection5 iDamages, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures);

		// Token: 0x060006A2 RID: 1698
		DamageResult InternalDamage(Damage iDamage, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures);

		// Token: 0x060006A3 RID: 1699
		bool ArcIntersect(out Vector3 oPosition, Vector3 iOrigin, Vector3 iDirection, float iRange, float iAngle, float iHeightDifference);

		// Token: 0x060006A4 RID: 1700
		void Kill();

		// Token: 0x060006A5 RID: 1701
		void OverKill();

		// Token: 0x060006A6 RID: 1702
		void Electrocute(IDamageable iTarget, float iMultiplyer);

		// Token: 0x060006A7 RID: 1703
		float ResistanceAgainst(Elements iElement);
	}
}
