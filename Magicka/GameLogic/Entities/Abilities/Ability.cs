using System;
using System.Collections.Generic;
using System.Reflection;
using JigLibX.Geometry;
using Magicka.AI;
using Magicka.AI.Arithmetics;
using Magicka.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Abilities
{
	// Token: 0x02000133 RID: 307
	public abstract class Ability
	{
		// Token: 0x0600088C RID: 2188 RVA: 0x00037559 File Offset: 0x00035759
		protected Ability(float iCooldown, Target iTarget, Expression iExpression, Animations[] iAnimationKeys)
		{
			this.mCooldown = iCooldown;
			this.mTarget = iTarget;
			this.mFuzzyExpression = iExpression;
			this.mAnimationKeys = iAnimationKeys;
		}

		// Token: 0x0600088D RID: 2189 RVA: 0x00037580 File Offset: 0x00035780
		protected Ability(ContentReader iInput, AnimationClipAction[][] iAnimations)
		{
			this.mCooldown = iInput.ReadSingle();
			this.mTarget = (Target)iInput.ReadByte();
			if (iInput.ReadBoolean())
			{
				this.mFuzzyExpression = Expression.Read(iInput.ReadString());
			}
			this.mAnimationKeys = new Animations[iInput.ReadInt32()];
			for (int i = 0; i < this.mAnimationKeys.Length; i++)
			{
				this.mAnimationKeys[i] = (Animations)Enum.Parse(typeof(Animations), iInput.ReadString(), true);
			}
		}

		// Token: 0x0600088E RID: 2190 RVA: 0x0003760C File Offset: 0x0003580C
		protected Ability(Ability iCloneSource)
		{
			this.mIndex = iCloneSource.mIndex;
			this.mTarget = iCloneSource.mTarget;
			this.mFuzzyExpression = iCloneSource.mFuzzyExpression;
			this.mAnimationKeys = new Animations[iCloneSource.mAnimationKeys.Length];
			iCloneSource.mAnimationKeys.CopyTo(this.mAnimationKeys, 0);
		}

		// Token: 0x0600088F RID: 2191 RVA: 0x00037668 File Offset: 0x00035868
		public virtual float GetDesirability(ref ExpressionArguments iArgs)
		{
			if (!this.IsUseful(ref iArgs))
			{
				return float.MinValue;
			}
			if (iArgs.AI.NPC.AbilityCooldown[this.mIndex] > 0f)
			{
				return float.MinValue;
			}
			if (this.mFuzzyExpression != null)
			{
				return this.mFuzzyExpression.GetValue(ref iArgs);
			}
			return this.Desirability(ref iArgs);
		}

		// Token: 0x06000890 RID: 2192
		protected abstract float Desirability(ref ExpressionArguments iArgs);

		// Token: 0x06000891 RID: 2193 RVA: 0x000376C4 File Offset: 0x000358C4
		protected static Type GetType(string name)
		{
			Type[] types = Assembly.GetExecutingAssembly().GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				if (types[i].IsSubclassOf(typeof(Ability)) && types[i].Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
				{
					return types[i];
				}
			}
			return null;
		}

		// Token: 0x06000892 RID: 2194
		public abstract void Update(Agent iAgent, float iDeltaTime);

		// Token: 0x06000893 RID: 2195 RVA: 0x00037714 File Offset: 0x00035914
		public bool Execute(Agent iAgent)
		{
			bool flag = this.InternalExecute(iAgent);
			if (flag)
			{
				iAgent.BusyAbility = this;
			}
			return flag;
		}

		// Token: 0x06000894 RID: 2196
		public abstract bool InternalExecute(Agent iAgent);

		// Token: 0x06000895 RID: 2197 RVA: 0x00037734 File Offset: 0x00035934
		public virtual bool ForceExecute(Agent iAgent)
		{
			return this.InternalExecute(iAgent);
		}

		// Token: 0x06000896 RID: 2198
		public abstract float GetMaxRange(Agent iAgent);

		// Token: 0x06000897 RID: 2199
		public abstract float GetMinRange(Agent iAgent);

		// Token: 0x06000898 RID: 2200
		public abstract float GetArc(Agent iAgent);

		// Token: 0x06000899 RID: 2201
		public abstract int[] GetWeapons();

		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x0600089A RID: 2202 RVA: 0x0003773D File Offset: 0x0003593D
		public virtual float Cooldown
		{
			get
			{
				return this.mCooldown;
			}
		}

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x0600089B RID: 2203 RVA: 0x00037745 File Offset: 0x00035945
		public int Index
		{
			get
			{
				return this.mIndex;
			}
		}

		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x0600089C RID: 2204 RVA: 0x0003774D File Offset: 0x0003594D
		public Target Target
		{
			get
			{
				return this.mTarget;
			}
		}

		// Token: 0x0600089D RID: 2205 RVA: 0x00037758 File Offset: 0x00035958
		public static Ability Read(int iIndex, ContentReader iInput, AnimationClipAction[][] iAnimations)
		{
			string name = iInput.ReadString();
			Type type = Ability.GetType(name);
			Ability ability = (Ability)type.GetConstructor(new Type[]
			{
				typeof(ContentReader),
				typeof(AnimationClipAction[][])
			}).Invoke(new object[]
			{
				iInput,
				iAnimations
			});
			ability.mIndex = iIndex;
			return ability;
		}

		// Token: 0x0600089E RID: 2206 RVA: 0x000377C4 File Offset: 0x000359C4
		public virtual bool ChooseAttackAngle(Agent iAgent, out Vector3 oDirection)
		{
			Vector3 position = iAgent.Owner.Position;
			Vector3 position2 = iAgent.CurrentTarget.Position;
			float num = (this.GetMinRange(iAgent) + this.GetMaxRange(iAgent)) * 0.5f + iAgent.CurrentTarget.Radius;
			Vector3 vector;
			Vector3.Subtract(ref position, ref position2, out vector);
			vector.Y = 0f;
			if (vector.LengthSquared() <= 1E-06f)
			{
				oDirection = default(Vector3);
				return false;
			}
			vector.Normalize();
			Vector3.Multiply(ref vector, num, out vector);
			Segment iSeg;
			iSeg.Origin = position2;
			GameScene currentScene = iAgent.Owner.PlayState.Level.CurrentScene;
			List<Entity> entities = iAgent.Owner.PlayState.EntityManager.GetEntities(position2, num, true);
			for (int i = 0; i < entities.Count; i++)
			{
				Character character = entities[i] as Character;
				if (character == null || ((character.Faction & iAgent.Owner.Faction) != Factions.NONE && !iAgent.AttackedBy.ContainsKey(character.Handle)))
				{
					entities.RemoveAt(i);
					i--;
				}
			}
			for (int j = 0; j < 3; j++)
			{
				Quaternion quaternion;
				Quaternion.CreateFromYawPitchRoll((float)Ability.sRandom.NextDouble() * 3.1415927f - 1.5707964f, 0f, 0f, out quaternion);
				Vector3.Transform(ref vector, ref quaternion, out iSeg.Delta);
				float num2;
				Vector3 vector2;
				Vector3 vector3;
				if (!currentScene.SegmentIntersect(out num2, out vector2, out vector3, iSeg))
				{
					int num3 = 0;
					while (num3 < entities.Count && !(entities[num3] as Character).SegmentIntersect(out vector2, iSeg, 0.5f))
					{
						num3++;
					}
					oDirection = iSeg.Delta;
					iAgent.Owner.PlayState.EntityManager.ReturnEntityList(entities);
					return true;
				}
			}
			oDirection = default(Vector3);
			iAgent.Owner.PlayState.EntityManager.ReturnEntityList(entities);
			return false;
		}

		// Token: 0x0600089F RID: 2207
		public abstract Vector3 GetDesiredDirection(Agent iAgent);

		// Token: 0x060008A0 RID: 2208 RVA: 0x000379BD File Offset: 0x00035BBD
		public virtual bool IsUseful(ref ExpressionArguments iArgs)
		{
			return this.IsUseful(iArgs.AI);
		}

		// Token: 0x060008A1 RID: 2209 RVA: 0x000379CB File Offset: 0x00035BCB
		public virtual bool IsUseful(Agent iAgent)
		{
			return true;
		}

		// Token: 0x060008A2 RID: 2210 RVA: 0x000379D0 File Offset: 0x00035BD0
		public virtual bool InRange(Agent iAgent)
		{
			IDamageable currentTarget = iAgent.CurrentTarget;
			if (currentTarget == null)
			{
				return false;
			}
			float num = this.GetMaxRange(iAgent) + iAgent.Owner.Radius + iAgent.CurrentTarget.Radius;
			float num2 = Math.Max(this.GetMinRange(iAgent) - iAgent.CurrentTarget.Radius, 0f);
			num *= num;
			num2 *= num2;
			Vector3 position = iAgent.Owner.Position;
			position.Y = 0f;
			Vector3 position2 = currentTarget.Position;
			position2.Y = 0f;
			float num3;
			Vector3.DistanceSquared(ref position, ref position2, out num3);
			return num3 <= num & num3 >= num2;
		}

		// Token: 0x060008A3 RID: 2211 RVA: 0x00037A78 File Offset: 0x00035C78
		public virtual bool FacingTarget(Agent iAgent)
		{
			IDamageable currentTarget = iAgent.CurrentTarget;
			float arc = this.GetArc(iAgent);
			Vector3 direction = iAgent.Owner.CharacterBody.Direction;
			Vector3 position = iAgent.Owner.Position;
			Vector3 position2 = currentTarget.Position;
			Vector3 vector;
			Vector3.Subtract(ref position2, ref position, out vector);
			vector.Y = 0f;
			float num = vector.Length();
			if (num < 1E-06f)
			{
				return true;
			}
			Vector3.Divide(ref vector, num, out vector);
			if (MagickaMath.Angle(ref direction, ref vector) < arc)
			{
				Segment segment = default(Segment);
				segment.Origin = position;
				Vector3.Subtract(ref position2, ref position, out segment.Delta);
				return true;
			}
			return false;
		}

		// Token: 0x04000811 RID: 2065
		protected static Random sRandom = new Random();

		// Token: 0x04000812 RID: 2066
		private int mIndex;

		// Token: 0x04000813 RID: 2067
		protected float mCooldown;

		// Token: 0x04000814 RID: 2068
		protected Target mTarget;

		// Token: 0x04000815 RID: 2069
		protected Expression mFuzzyExpression;

		// Token: 0x04000816 RID: 2070
		protected Animations[] mAnimationKeys;
	}
}
