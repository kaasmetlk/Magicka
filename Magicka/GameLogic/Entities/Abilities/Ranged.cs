using System;
using System.Collections.Generic;
using JigLibX.Geometry;
using Magicka.AI;
using Magicka.AI.Arithmetics;
using Magicka.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Abilities
{
	// Token: 0x0200053A RID: 1338
	public class Ranged : Ability
	{
		// Token: 0x060027CA RID: 10186 RVA: 0x00122DF8 File Offset: 0x00120FF8
		public Ranged(ContentReader iInput, AnimationClipAction[][] iAnimations) : base(iInput, iAnimations)
		{
			this.mMinRange = iInput.ReadSingle();
			this.mMaxRange = iInput.ReadSingle();
			this.mElevation = iInput.ReadSingle();
			this.mArc = iInput.ReadSingle();
			this.mArc = 3.1415927f;
			this.mAccuracy = iInput.ReadSingle();
			this.mWeapons = new int[iInput.ReadInt32()];
			for (int i = 0; i < this.mWeapons.Length; i++)
			{
				this.mWeapons[i] = iInput.ReadInt32();
			}
		}

		// Token: 0x060027CB RID: 10187 RVA: 0x00122E88 File Offset: 0x00121088
		protected override float Desirability(ref ExpressionArguments iArgs)
		{
			return FuzzyMath.FuzzyDistanceExponential(iArgs.Distance, this.mMinRange, this.mMaxRange);
		}

		// Token: 0x060027CC RID: 10188 RVA: 0x00122EAE File Offset: 0x001210AE
		public override void Update(Agent iAgent, float iDeltaTime)
		{
		}

		// Token: 0x060027CD RID: 10189 RVA: 0x00122EB0 File Offset: 0x001210B0
		public override bool InternalExecute(Agent iAgent)
		{
			Segment iSeg;
			iSeg.Origin = iAgent.Owner.Position;
			Vector3 position = iAgent.CurrentTarget.Position;
			Vector3.Subtract(ref position, ref iSeg.Origin, out iSeg.Delta);
			iAgent.Owner.CharacterBody.DesiredDirection = iSeg.Delta;
			Vector3.Multiply(ref iSeg.Delta, (float)Math.Cos((double)this.mElevation), out iSeg.Delta);
			Vector3 iCenter;
			iSeg.GetPoint(0.5f, out iCenter);
			List<Entity> entities = iAgent.Owner.PlayState.EntityManager.GetEntities(iCenter, this.mMaxRange, false);
			for (int i = 0; i < entities.Count; i++)
			{
				Character character = entities[i] as Character;
				Vector3 vector;
				if (character != null && character != iAgent.Owner && (character.Faction & iAgent.Owner.Faction) != Factions.NONE && character.SegmentIntersect(out vector, iSeg, 0.75f))
				{
					iAgent.Owner.PlayState.EntityManager.ReturnEntityList(entities);
					return false;
				}
			}
			iAgent.Owner.PlayState.EntityManager.ReturnEntityList(entities);
			for (int j = 0; j < this.mWeapons.Length; j++)
			{
				iAgent.Owner.Equipment[this.mWeapons[j]].Item.PrepareToExecute(this);
			}
			iAgent.Owner.Attack(this.mAnimationKeys[MagickaMath.Random.Next(this.mAnimationKeys.Length)], true);
			return true;
		}

		// Token: 0x060027CE RID: 10190 RVA: 0x00123036 File Offset: 0x00121236
		public override float GetMaxRange(Agent iAgent)
		{
			return this.mMaxRange;
		}

		// Token: 0x060027CF RID: 10191 RVA: 0x0012303E File Offset: 0x0012123E
		public override float GetMinRange(Agent iAgent)
		{
			return this.mMinRange;
		}

		// Token: 0x060027D0 RID: 10192 RVA: 0x00123046 File Offset: 0x00121246
		public float GetElevation()
		{
			return this.mElevation;
		}

		// Token: 0x060027D1 RID: 10193 RVA: 0x0012304E File Offset: 0x0012124E
		public float GetAccuracy()
		{
			return this.mAccuracy;
		}

		// Token: 0x060027D2 RID: 10194 RVA: 0x00123056 File Offset: 0x00121256
		public override float GetArc(Agent iAgent)
		{
			return this.mArc;
		}

		// Token: 0x060027D3 RID: 10195 RVA: 0x0012305E File Offset: 0x0012125E
		public override int[] GetWeapons()
		{
			return this.mWeapons;
		}

		// Token: 0x060027D4 RID: 10196 RVA: 0x00123068 File Offset: 0x00121268
		public override Vector3 GetDesiredDirection(Agent iAgent)
		{
			Vector3 position = iAgent.Owner.Position;
			Vector3 position2 = iAgent.CurrentTarget.Position;
			Vector3 result;
			Vector3.Subtract(ref position2, ref position, out result);
			float num = result.Length();
			if (num > 1E-45f)
			{
				Vector3.Divide(ref result, num, out result);
				return result;
			}
			return Vector3.Forward;
		}

		// Token: 0x060027D5 RID: 10197 RVA: 0x001230BC File Offset: 0x001212BC
		public override bool ChooseAttackAngle(Agent iAgent, out Vector3 oDirection)
		{
			Vector3 position = iAgent.Owner.Position;
			Vector3 position2 = iAgent.CurrentTarget.Position;
			float num = (this.GetMinRange(iAgent) + this.GetMaxRange(iAgent)) * 0.5f;
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
			List<Entity> entities = iAgent.Owner.PlayState.EntityManager.GetEntities(position2, num, false);
			for (int i = 0; i < entities.Count; i++)
			{
				Character character = entities[i] as Character;
				if (character == null || (character.Faction & iAgent.Owner.Faction) == Factions.NONE || !iAgent.AttackedBy.ContainsKey(character.Handle))
				{
					entities.RemoveAt(i);
					i--;
				}
			}
			for (int j = 0; j < 3; j++)
			{
				Quaternion quaternion;
				Quaternion.CreateFromYawPitchRoll((float)Ability.sRandom.NextDouble() * 3.1415927f - 1.5707964f, 0f, 0f, out quaternion);
				Vector3.Transform(ref vector, ref quaternion, out oDirection);
				Vector3.Multiply(ref oDirection, (1f - this.mElevation / 1.5707964f) * 0.5f, out iSeg.Delta);
				Segment iSeg2;
				Vector3.Add(ref iSeg.Origin, ref oDirection, out iSeg2.Origin);
				Vector3.Multiply(ref oDirection, (1f - this.mElevation / 1.5707964f) * -0.5f, out iSeg2.Delta);
				float num2;
				Vector3 vector2;
				Vector3 vector3;
				if (!currentScene.SegmentIntersect(out num2, out vector2, out vector3, iSeg) && !currentScene.SegmentIntersect(out num2, out vector2, out vector3, iSeg2))
				{
					int num3 = 0;
					while (num3 < entities.Count && !(entities[num3] as Character).SegmentIntersect(out vector2, iSeg, 0.5f) && !(entities[num3] as Character).SegmentIntersect(out vector2, iSeg2, 0.5f))
					{
						num3++;
					}
					iAgent.Owner.PlayState.EntityManager.ReturnEntityList(entities);
					return true;
				}
			}
			oDirection = default(Vector3);
			iAgent.Owner.PlayState.EntityManager.ReturnEntityList(entities);
			return false;
		}

		// Token: 0x04002B69 RID: 11113
		private int[] mWeapons;

		// Token: 0x04002B6A RID: 11114
		private float mMaxRange;

		// Token: 0x04002B6B RID: 11115
		private float mMinRange;

		// Token: 0x04002B6C RID: 11116
		private float mElevation;

		// Token: 0x04002B6D RID: 11117
		private float mArc;

		// Token: 0x04002B6E RID: 11118
		private float mAccuracy;
	}
}
