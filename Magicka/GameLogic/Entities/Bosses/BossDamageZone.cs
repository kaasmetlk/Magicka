using System;
using JigLibX.Geometry;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Bosses
{
	// Token: 0x020001CB RID: 459
	public class BossDamageZone : BossCollisionZone, IStatusEffected, IDamageable
	{
		// Token: 0x17000403 RID: 1027
		// (get) Token: 0x06000F84 RID: 3972 RVA: 0x000608E8 File Offset: 0x0005EAE8
		// (set) Token: 0x06000F85 RID: 3973 RVA: 0x000608F0 File Offset: 0x0005EAF0
		public bool IsEthereal
		{
			get
			{
				return this.mIsEthereal;
			}
			set
			{
				this.mIsEthereal = value;
			}
		}

		// Token: 0x06000F86 RID: 3974 RVA: 0x000608F9 File Offset: 0x0005EAF9
		public BossDamageZone(PlayState iPlayState, IBoss iParent, int iIndex, float iRadius, params Primitive[] iPrimitives) : base(iPlayState, iParent, iPrimitives)
		{
			this.mIsEthereal = false;
			this.mIndex = iIndex;
			this.mRadius = iRadius;
		}

		// Token: 0x06000F87 RID: 3975 RVA: 0x0006091C File Offset: 0x0005EB1C
		public BossDamageZone(PlayState iPlayState, IBoss iParent, int iIndex, float iRadius, Primitive iPrimitives) : base(iPlayState, iParent, new Primitive[]
		{
			iPrimitives
		})
		{
			this.mIsEthereal = false;
			this.mIndex = iIndex;
			this.mRadius = iRadius;
		}

		// Token: 0x06000F88 RID: 3976 RVA: 0x00060954 File Offset: 0x0005EB54
		public BossDamageZone(PlayState iPlayState, IBoss iParent, int iIndex, float iRadius) : base(iPlayState, iParent, new Primitive[]
		{
			new Sphere(default(Vector3), iRadius)
		})
		{
			this.mIsEthereal = false;
			this.mIndex = iIndex;
			this.mRadius = iRadius;
		}

		// Token: 0x06000F89 RID: 3977 RVA: 0x0006099A File Offset: 0x0005EB9A
		public float ResistanceAgainst(Elements iElement)
		{
			return this.mParent.ResistanceAgainst(iElement);
		}

		// Token: 0x06000F8A RID: 3978 RVA: 0x000609A8 File Offset: 0x0005EBA8
		public override Vector3 CalcImpulseVelocity(Vector3 iDirection, float iElevation, float iMassPower, float iDistance)
		{
			return default(Vector3);
		}

		// Token: 0x06000F8B RID: 3979 RVA: 0x000609BE File Offset: 0x0005EBBE
		public void SetPosition(ref Vector3 iPosition)
		{
			this.mBody.MoveTo(iPosition, Matrix.Identity);
		}

		// Token: 0x17000404 RID: 1028
		// (get) Token: 0x06000F8C RID: 3980 RVA: 0x000609D6 File Offset: 0x0005EBD6
		public int ZoneIndex
		{
			get
			{
				return this.mIndex;
			}
		}

		// Token: 0x06000F8D RID: 3981 RVA: 0x000609DE File Offset: 0x0005EBDE
		public DamageResult InternalDamage(Damage iDamage, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			return this.mParent.Damage(this.mIndex, iDamage, iAttacker, ref iAttackPosition, iFeatures);
		}

		// Token: 0x06000F8E RID: 3982 RVA: 0x000609F8 File Offset: 0x0005EBF8
		public override bool SegmentIntersect(out Vector3 oPosition, Segment iSeg, float iSegmentRadius)
		{
			if (this.mIsEthereal)
			{
				oPosition = default(Vector3);
				return false;
			}
			float num;
			Vector3 vector;
			return this.mCollision.SegmentIntersect(out num, out oPosition, out vector, iSeg);
		}

		// Token: 0x06000F8F RID: 3983 RVA: 0x00060A28 File Offset: 0x0005EC28
		public override bool ArcIntersect(out Vector3 oPosition, Vector3 iOrigin, Vector3 iDirection, float iRange, float iAngle, float iHeightDifference)
		{
			if (this.mIsEthereal)
			{
				oPosition = default(Vector3);
				return false;
			}
			float value = this.Position.Y - iOrigin.Y;
			if (Math.Abs(value) > iHeightDifference)
			{
				oPosition = default(Vector3);
				return false;
			}
			iOrigin.Y = 0f;
			iDirection.Y = 0f;
			Vector3 position = this.Position;
			position.Y = 0f;
			Vector3 vector;
			Vector3.Subtract(ref iOrigin, ref position, out vector);
			if (vector.LengthSquared() <= 1E-06f)
			{
				oPosition = default(Vector3);
				return false;
			}
			float num = vector.Length();
			if (num < this.Radius)
			{
				oPosition = iOrigin;
				return true;
			}
			float mRadius = this.mRadius;
			if (num - mRadius > iRange)
			{
				oPosition = default(Vector3);
				return false;
			}
			Vector3.Divide(ref vector, num, out vector);
			float num2;
			Vector3.Dot(ref vector, ref iDirection, out num2);
			num2 = -num2;
			float num3 = (float)Math.Acos((double)num2);
			float num4 = -2f * num * num;
			float num5 = (mRadius * mRadius + num4) / num4;
			float num6 = (float)Math.Acos((double)num5);
			if (num3 - num6 < iAngle)
			{
				Vector3.Multiply(ref vector, mRadius, out vector);
				position = this.Position;
				Vector3.Add(ref position, ref vector, out oPosition);
				return true;
			}
			oPosition = default(Vector3);
			return false;
		}

		// Token: 0x06000F90 RID: 3984 RVA: 0x00060B64 File Offset: 0x0005ED64
		public bool HasStatus(StatusEffects iStatus)
		{
			return this.mParent.HasStatus(this.mIndex, iStatus);
		}

		// Token: 0x06000F91 RID: 3985 RVA: 0x00060B78 File Offset: 0x0005ED78
		public StatusEffect[] GetStatusEffects()
		{
			return this.mParent.GetStatusEffects();
		}

		// Token: 0x06000F92 RID: 3986 RVA: 0x00060B85 File Offset: 0x0005ED85
		public float StatusMagnitude(StatusEffects iStatus)
		{
			return this.mParent.StatusMagnitude(this.mIndex, iStatus);
		}

		// Token: 0x06000F93 RID: 3987 RVA: 0x00060B99 File Offset: 0x0005ED99
		public void SetSlow()
		{
		}

		// Token: 0x06000F94 RID: 3988 RVA: 0x00060B9B File Offset: 0x0005ED9B
		public void Damage(float iDamage, Elements iElement)
		{
			this.mParent.Damage(this.mIndex, iDamage, iElement);
		}

		// Token: 0x17000405 RID: 1029
		// (get) Token: 0x06000F95 RID: 3989 RVA: 0x00060BB0 File Offset: 0x0005EDB0
		public float Volume
		{
			get
			{
				return 4.1887903f * this.mRadius * this.mRadius * this.mRadius;
			}
		}

		// Token: 0x06000F96 RID: 3990 RVA: 0x00060BCC File Offset: 0x0005EDCC
		public DamageResult InternalDamage(DamageCollection5 iDamages, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			DamageResult damageResult = DamageResult.None;
			damageResult |= this.InternalDamage(iDamages.A, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			damageResult |= this.InternalDamage(iDamages.B, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			damageResult |= this.InternalDamage(iDamages.C, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			damageResult |= this.InternalDamage(iDamages.D, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			return damageResult | this.InternalDamage(iDamages.E, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
		}

		// Token: 0x06000F97 RID: 3991 RVA: 0x00060C4C File Offset: 0x0005EE4C
		public void OverKill()
		{
			Vector3 position = this.Position;
			this.mParent.Damage(0, new Damage(AttackProperties.Damage, Elements.Earth | Elements.Arcane, 9000000f, 1f), null, ref position, Defines.DamageFeatures.Damage);
			this.mParent.Damage(1, new Damage(AttackProperties.Damage, Elements.Earth | Elements.Arcane, 9000000f, 1f), null, ref position, Defines.DamageFeatures.Damage);
			this.mParent.Damage(2, new Damage(AttackProperties.Damage, Elements.Earth | Elements.Arcane, 9000000f, 1f), null, ref position, Defines.DamageFeatures.Damage);
		}

		// Token: 0x06000F98 RID: 3992 RVA: 0x00060CC9 File Offset: 0x0005EEC9
		public void Electrocute(IDamageable iTarget, float iMultiplyer)
		{
		}

		// Token: 0x04000E16 RID: 3606
		protected int mIndex;

		// Token: 0x04000E17 RID: 3607
		private bool mIsEthereal;
	}
}
