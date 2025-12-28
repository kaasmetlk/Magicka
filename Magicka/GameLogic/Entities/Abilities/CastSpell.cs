using System;
using Magicka.AI;
using Magicka.AI.Arithmetics;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Abilities
{
	// Token: 0x02000134 RID: 308
	public class CastSpell : Ability
	{
		// Token: 0x060008A5 RID: 2213 RVA: 0x00037B2C File Offset: 0x00035D2C
		public CastSpell(float iCooldown, Target iTarget, Expression iExpression, Animations[] iAnimationKeys, float iMinRange, float iMaxRange, float iAngle, float iChantSpeed, float iPower) : base(iCooldown, iTarget, iExpression, iAnimationKeys)
		{
			this.mMinRange = iMinRange;
			this.mMaxRange = iMaxRange;
			this.mArc = iAngle;
			this.mChantSpeed = iChantSpeed;
			this.mPower = iPower;
		}

		// Token: 0x060008A6 RID: 2214 RVA: 0x00037B64 File Offset: 0x00035D64
		public CastSpell(CastSpell iCloneSource) : base(iCloneSource)
		{
			this.mMinRange = iCloneSource.mMinRange;
			this.mMaxRange = iCloneSource.mMaxRange;
			this.mArc = iCloneSource.mArc;
			this.mChantSpeed = iCloneSource.mChantSpeed;
			this.mPower = iCloneSource.mPower;
			this.mCastType = iCloneSource.mCastType;
			this.mElements = new Elements[iCloneSource.mElements.Length];
			iCloneSource.mElements.CopyTo(this.mElements, 0);
			for (int i = 0; i < this.mElements.Length; i++)
			{
				this.mCombinedElements |= this.mElements[i];
			}
		}

		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x060008A7 RID: 2215 RVA: 0x00037C0D File Offset: 0x00035E0D
		// (set) Token: 0x060008A8 RID: 2216 RVA: 0x00037C15 File Offset: 0x00035E15
		public Expression FuzzyExpression
		{
			get
			{
				return this.mFuzzyExpression;
			}
			set
			{
				this.mFuzzyExpression = value;
			}
		}

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x060008A9 RID: 2217 RVA: 0x00037C1E File Offset: 0x00035E1E
		// (set) Token: 0x060008AA RID: 2218 RVA: 0x00037C26 File Offset: 0x00035E26
		public Elements[] Elements
		{
			get
			{
				return this.mElements;
			}
			set
			{
				this.mElements = value;
			}
		}

		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x060008AB RID: 2219 RVA: 0x00037C2F File Offset: 0x00035E2F
		// (set) Token: 0x060008AC RID: 2220 RVA: 0x00037C37 File Offset: 0x00035E37
		public CastType CastType
		{
			get
			{
				return this.mCastType;
			}
			set
			{
				this.mCastType = value;
			}
		}

		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x060008AD RID: 2221 RVA: 0x00037C40 File Offset: 0x00035E40
		// (set) Token: 0x060008AE RID: 2222 RVA: 0x00037C48 File Offset: 0x00035E48
		public Animations[] Animations
		{
			get
			{
				return this.mAnimationKeys;
			}
			set
			{
				this.mAnimationKeys = value;
			}
		}

		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x060008AF RID: 2223 RVA: 0x00037C51 File Offset: 0x00035E51
		// (set) Token: 0x060008B0 RID: 2224 RVA: 0x00037C59 File Offset: 0x00035E59
		public new Target Target
		{
			get
			{
				return this.mTarget;
			}
			set
			{
				this.mTarget = value;
			}
		}

		// Token: 0x170001BA RID: 442
		// (get) Token: 0x060008B1 RID: 2225 RVA: 0x00037C62 File Offset: 0x00035E62
		// (set) Token: 0x060008B2 RID: 2226 RVA: 0x00037C6A File Offset: 0x00035E6A
		public float MinRange
		{
			get
			{
				return this.mMinRange;
			}
			set
			{
				this.mMinRange = value;
			}
		}

		// Token: 0x170001BB RID: 443
		// (get) Token: 0x060008B3 RID: 2227 RVA: 0x00037C73 File Offset: 0x00035E73
		// (set) Token: 0x060008B4 RID: 2228 RVA: 0x00037C7B File Offset: 0x00035E7B
		public float MaxRange
		{
			get
			{
				return this.mMaxRange;
			}
			set
			{
				this.mMaxRange = value;
			}
		}

		// Token: 0x170001BC RID: 444
		// (get) Token: 0x060008B5 RID: 2229 RVA: 0x00037C84 File Offset: 0x00035E84
		// (set) Token: 0x060008B6 RID: 2230 RVA: 0x00037C8C File Offset: 0x00035E8C
		public new float Cooldown
		{
			get
			{
				return this.mCooldown;
			}
			set
			{
				this.mCooldown = value;
			}
		}

		// Token: 0x060008B7 RID: 2231 RVA: 0x00037C98 File Offset: 0x00035E98
		public CastSpell(ContentReader iInput, AnimationClipAction[][] iAnimations) : base(iInput, iAnimations)
		{
			this.mMinRange = iInput.ReadSingle();
			this.mMaxRange = iInput.ReadSingle();
			this.mArc = iInput.ReadSingle();
			this.mChantSpeed = iInput.ReadSingle();
			this.mPower = iInput.ReadSingle();
			this.mCastType = (CastType)iInput.ReadInt32();
			this.mElements = new Elements[iInput.ReadInt32()];
			for (int i = 0; i < this.mElements.Length; i++)
			{
				this.mElements[i] = (Elements)iInput.ReadInt32();
				this.mCombinedElements |= this.mElements[i];
			}
		}

		// Token: 0x060008B8 RID: 2232 RVA: 0x00037D3C File Offset: 0x00035F3C
		protected override float Desirability(ref ExpressionArguments iArgs)
		{
			throw new NotImplementedException("Cast spell must define a desirability expression!");
		}

		// Token: 0x060008B9 RID: 2233 RVA: 0x00037D48 File Offset: 0x00035F48
		public override void Update(Agent iAgent, float iDeltaTime)
		{
			if (iAgent.BusyAbility == iAgent.NextAbility || iAgent.NPC.CurrentSpell != null)
			{
				return;
			}
			if ((iAgent.NPC.ChantCooldown < 0f & iAgent.Owner.SpellQueue.Count < this.mElements.Length) && !iAgent.NPC.Attacking && Vector3.DistanceSquared(iAgent.Owner.Position, iAgent.CurrentTarget.Position) <= this.mMaxRange * this.mMaxRange * 1.25f * 1.25f)
			{
				if (this.mCastType == CastType.Weapon)
				{
					for (int i = 0; i < this.mElements.Length; i++)
					{
						iAgent.NPC.ConjureSpell(this.mElements[i]);
					}
					iAgent.NPC.ChantCooldown = this.mChantSpeed;
					return;
				}
				iAgent.NPC.ChantCooldown = this.mChantSpeed;
				iAgent.NPC.ConjureSpell(this.mElements[iAgent.NPC.SpellQueue.Count]);
			}
		}

		// Token: 0x060008BA RID: 2234 RVA: 0x00037E60 File Offset: 0x00036060
		public override bool InternalExecute(Agent iAgent)
		{
			if (iAgent.BusyAbility == iAgent.NextAbility)
			{
				return false;
			}
			if (iAgent.Owner.SpellQueue.Count >= this.mElements.Length && !iAgent.NPC.Attacking)
			{
				iAgent.Owner.SpellPower = this.mPower;
				iAgent.Owner.CastType = this.mCastType;
				iAgent.NPC.ChantCooldown = this.mChantSpeed;
				iAgent.NPC.Attack(this.mAnimationKeys[Ability.sRandom.Next(this.mAnimationKeys.Length)], true);
				return true;
			}
			return false;
		}

		// Token: 0x060008BB RID: 2235 RVA: 0x00037EFF File Offset: 0x000360FF
		public override float GetMinRange(Agent iAgent)
		{
			return this.mMinRange;
		}

		// Token: 0x060008BC RID: 2236 RVA: 0x00037F07 File Offset: 0x00036107
		public override float GetMaxRange(Agent iAgent)
		{
			return this.mMaxRange;
		}

		// Token: 0x060008BD RID: 2237 RVA: 0x00037F0F File Offset: 0x0003610F
		public override float GetArc(Agent iAgent)
		{
			return this.mArc;
		}

		// Token: 0x060008BE RID: 2238 RVA: 0x00037F17 File Offset: 0x00036117
		public override int[] GetWeapons()
		{
			return null;
		}

		// Token: 0x060008BF RID: 2239 RVA: 0x00037F1C File Offset: 0x0003611C
		public override Vector3 GetDesiredDirection(Agent iAgent)
		{
			if (iAgent.Owner == iAgent.CurrentTarget)
			{
				return iAgent.Owner.Body.Orientation.Forward;
			}
			Vector3 position = iAgent.Owner.Position;
			Vector3 position2 = iAgent.CurrentTarget.Position;
			Vector3 result;
			Vector3.Subtract(ref position2, ref position, out result);
			float num = result.Length();
			if (num > 1E-06f)
			{
				Vector3.Divide(ref result, num, out result);
			}
			else
			{
				result.Z = 1f;
			}
			return result;
		}

		// Token: 0x04000817 RID: 2071
		private CastType mCastType;

		// Token: 0x04000818 RID: 2072
		private Elements[] mElements;

		// Token: 0x04000819 RID: 2073
		private Elements mCombinedElements;

		// Token: 0x0400081A RID: 2074
		private float mMinRange;

		// Token: 0x0400081B RID: 2075
		private float mMaxRange;

		// Token: 0x0400081C RID: 2076
		private float mArc;

		// Token: 0x0400081D RID: 2077
		private float mChantSpeed;

		// Token: 0x0400081E RID: 2078
		private float mPower;
	}
}
