using System;
using Magicka.AI;
using Magicka.AI.Arithmetics;
using Magicka.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Abilities
{
	// Token: 0x020001F9 RID: 505
	public class ThrowGrip : Ability
	{
		// Token: 0x06001092 RID: 4242 RVA: 0x000683B4 File Offset: 0x000665B4
		public unsafe ThrowGrip(ContentReader iInput, AnimationClipAction[][] iAnimations) : base(iInput, iAnimations)
		{
			this.mMaxRange = iInput.ReadSingle();
			this.mMinRange = iInput.ReadSingle();
			this.mElevation = iInput.ReadSingle();
			int num = Math.Min(iInput.ReadInt32(), 4);
			DamageCollection5 damageCollection = default(DamageCollection5);
			Damage* ptr = &damageCollection.A;
			for (int i = 0; i < num; i++)
			{
				ptr[i] = new Damage((AttackProperties)iInput.ReadInt32(), (Elements)iInput.ReadInt32(), (float)iInput.ReadInt32(), iInput.ReadSingle());
			}
			this.mDamages = damageCollection;
		}

		// Token: 0x06001093 RID: 4243 RVA: 0x0006844D File Offset: 0x0006664D
		protected override float Desirability(ref ExpressionArguments iArgs)
		{
			if (!iArgs.AI.Owner.IsGripping)
			{
				return float.MinValue;
			}
			return iArgs.AI.Owner.GripDamageAccumulation / iArgs.AI.Owner.HitTolerance;
		}

		// Token: 0x06001094 RID: 4244 RVA: 0x00068488 File Offset: 0x00066688
		public override void Update(Agent iAgent, float iDeltaTime)
		{
		}

		// Token: 0x06001095 RID: 4245 RVA: 0x0006848A File Offset: 0x0006668A
		public override bool InternalExecute(Agent iAgent)
		{
			iAgent.Owner.Attack(this.mAnimationKeys[Ability.sRandom.Next(this.mAnimationKeys.Length)], false);
			iAgent.Owner.GrippedCharacter.SetCollisionDamage(ref this.mDamages);
			return true;
		}

		// Token: 0x06001096 RID: 4246 RVA: 0x000684C8 File Offset: 0x000666C8
		public override float GetMaxRange(Agent iAgent)
		{
			return 0f;
		}

		// Token: 0x06001097 RID: 4247 RVA: 0x000684CF File Offset: 0x000666CF
		public override float GetMinRange(Agent iAgent)
		{
			return 0f;
		}

		// Token: 0x06001098 RID: 4248 RVA: 0x000684D6 File Offset: 0x000666D6
		public override float GetArc(Agent iAgent)
		{
			return 3.1415927f;
		}

		// Token: 0x06001099 RID: 4249 RVA: 0x000684DD File Offset: 0x000666DD
		public override int[] GetWeapons()
		{
			return null;
		}

		// Token: 0x0600109A RID: 4250 RVA: 0x000684E0 File Offset: 0x000666E0
		public override bool IsUseful(Agent iAgent)
		{
			return iAgent.Owner.IsGripping;
		}

		// Token: 0x0600109B RID: 4251 RVA: 0x000684F0 File Offset: 0x000666F0
		public override Vector3 GetDesiredDirection(Agent iAgent)
		{
			Vector3 forward = Vector3.Forward;
			Quaternion quaternion;
			Quaternion.CreateFromYawPitchRoll((float)Ability.sRandom.NextDouble() * 6.2831855f, 0f, 0f, out quaternion);
			Vector3.Transform(ref forward, ref quaternion, out forward);
			return forward;
		}

		// Token: 0x0600109C RID: 4252 RVA: 0x00068534 File Offset: 0x00066734
		public void CalcThrow(Agent iAgent, out Vector3 oVelocity)
		{
			Vector3 position = iAgent.Owner.Position;
			oVelocity = iAgent.Owner.Direction;
			Vector3 vector;
			Vector3.Multiply(ref oVelocity, (this.mMinRange + this.mMaxRange) * 0.5f, out vector);
			Vector3.Add(ref vector, ref position, out vector);
			Vector3 position2 = iAgent.Owner.GrippedCharacter.Position;
			float num = vector.Y - position2.Y;
			vector.Y = (position2.Y = 0f);
			float num2;
			Vector3.Distance(ref vector, ref position2, out num2);
			float num3 = oVelocity.Y = (float)Math.Sin((double)this.mElevation);
			float num4 = (float)Math.Cos((double)this.mElevation);
			oVelocity.X *= num4;
			oVelocity.Z *= num4;
			float num5 = (float)Math.Sqrt((double)(PhysicsManager.Instance.Simulator.Gravity.Y * -1f * num2 * num2 / (2f * (num2 * num3 / num4 - num) * num4 * num4)));
			if (float.IsNaN(num5) || float.IsInfinity(num5))
			{
				return;
			}
			Vector3.Multiply(ref oVelocity, num5, out oVelocity);
		}

		// Token: 0x0600109D RID: 4253 RVA: 0x0006866A File Offset: 0x0006686A
		public override bool FacingTarget(Agent iAgent)
		{
			return true;
		}

		// Token: 0x0600109E RID: 4254 RVA: 0x0006866D File Offset: 0x0006686D
		public override bool InRange(Agent iAgent)
		{
			return true;
		}

		// Token: 0x04000F33 RID: 3891
		private float mMaxRange;

		// Token: 0x04000F34 RID: 3892
		private float mMinRange;

		// Token: 0x04000F35 RID: 3893
		private float mElevation;

		// Token: 0x04000F36 RID: 3894
		private DamageCollection5 mDamages;
	}
}
