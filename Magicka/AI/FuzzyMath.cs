using System;
using System.Collections.Generic;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;

namespace Magicka.AI
{
	// Token: 0x0200054D RID: 1357
	public static class FuzzyMath
	{
		// Token: 0x0600284E RID: 10318 RVA: 0x0013BFE2 File Offset: 0x0013A1E2
		public static float FuzzyDistanceExponential(float iDistance, float iMaxDistance)
		{
			return FuzzyMath.FuzzyDistanceExponential(iDistance, -iMaxDistance, iMaxDistance);
		}

		// Token: 0x0600284F RID: 10319 RVA: 0x0013BFF0 File Offset: 0x0013A1F0
		public static float FuzzyDistanceExponential(float iDistance, float iMinDistance, float iMaxDistance)
		{
			double num = (double)(iMinDistance + iMaxDistance) * 0.5;
			double x = Math.Pow(2.718281828459045, -2.302585092994046 / (num - (double)iMaxDistance));
			return (float)Math.Pow(x, -Math.Abs((double)iDistance - num));
		}

		// Token: 0x06002850 RID: 10320 RVA: 0x0013C03A File Offset: 0x0013A23A
		public static float FuzzyDistanceLinear(float iDistance, float iMaxDistance)
		{
			return 1f - iDistance / iMaxDistance;
		}

		// Token: 0x06002851 RID: 10321 RVA: 0x0013C048 File Offset: 0x0013A248
		public static float FuzzyDistanceLinear(float iDistance, float iMinDistance, float iMaxDistance)
		{
			float num = (iMaxDistance + iMinDistance) * 0.5f;
			float num2 = 2f / (iMaxDistance - iMinDistance);
			if (iDistance >= num)
			{
				return (iMaxDistance - iDistance) * num2;
			}
			return (iDistance - iMinDistance) * num2;
		}

		// Token: 0x06002852 RID: 10322 RVA: 0x0013C078 File Offset: 0x0013A278
		public static float FuzzyAnger(Agent iOwner, Entity iTarget)
		{
			float num;
			iOwner.AttackedBy.TryGetValue(iTarget.Handle, out num);
			num = 1f - (float)Math.Pow(0.977237221, (double)num);
			return num;
		}

		// Token: 0x06002853 RID: 10323 RVA: 0x0013C0B4 File Offset: 0x0013A2B4
		public static float FuzzyAngle(ref Vector3 iA, ref Vector3 iB)
		{
			float num;
			Vector3.Dot(ref iA, ref iB, out num);
			return (num + 1f) * 0.5f;
		}

		// Token: 0x06002854 RID: 10324 RVA: 0x0013C0D8 File Offset: 0x0013A2D8
		public static float FuzzyEnemyDensity(Entity iEntity, Factions iFactions, float iRange)
		{
			if (iEntity.PlayState.EntityManager != null)
			{
				List<Entity> entities = iEntity.PlayState.EntityManager.GetEntities(iEntity.Position, iRange, false);
				double num = 0.0;
				for (int i = 0; i < entities.Count; i++)
				{
					Character character = entities[i] as Character;
					if ((character != null & character != iEntity) && (character.Faction & iFactions) == Factions.NONE)
					{
						num += 1.0;
					}
				}
				iEntity.PlayState.EntityManager.ReturnEntityList(entities);
				return 1f - (float)Math.Pow(0.5, num);
			}
			return 1f;
		}

		// Token: 0x06002855 RID: 10325 RVA: 0x0013C18C File Offset: 0x0013A38C
		public static float FuzzyFriendlyDensity(Entity iEntity, Factions iFactions, float iRange)
		{
			if (iEntity.PlayState.EntityManager != null)
			{
				List<Entity> entities = iEntity.PlayState.EntityManager.GetEntities(iEntity.Position, iRange, false);
				double num = 0.0;
				for (int i = 0; i < entities.Count; i++)
				{
					Character character = entities[i] as Character;
					if ((character != null & character != iEntity) && (character.Faction & iFactions) != Factions.NONE)
					{
						num += 1.0;
					}
				}
				iEntity.PlayState.EntityManager.ReturnEntityList(entities);
				return 1f - (float)Math.Pow(0.5, num);
			}
			return 1f;
		}

		// Token: 0x06002856 RID: 10326 RVA: 0x0013C240 File Offset: 0x0013A440
		public static float FuzzyThreat(NonPlayerCharacter iOwner, Character iTarget, Elements iElements)
		{
			Vector3 direction = iTarget.Direction;
			Vector3 position = iOwner.Position;
			Vector3 position2 = iTarget.Position;
			Vector3.Subtract(ref position, ref position2, out position2);
			float num = position2.Length();
			if (num > 1E-06f)
			{
				Vector3.Divide(ref position2, num, out position2);
				float num2;
				if (iTarget.CurrentState is AttackState)
				{
					num2 = FuzzyMath.FuzzyAngle(ref position2, ref direction) * FuzzyMath.FuzzyDistanceExponential(num, 20f);
				}
				else
				{
					num2 = Math.Max(FuzzyMath.FuzzyAngle(ref position2, ref direction), FuzzyMath.FuzzyDistanceExponential(num, 20f));
					Spell spell = SpellManager.Instance.Combine(iTarget.SpellQueue);
					float num3 = 0f;
					for (int i = 0; i < 11; i++)
					{
						Elements elements = (Elements)(1 << i);
						if ((elements & iElements) == elements)
						{
							num3 += spell[elements];
						}
					}
					num3 *= 0.2f;
					if (iTarget.CurrentState is ChargeState || iTarget.CurrentState is CastState)
					{
						num3 = (float)Math.Pow((double)num3, 0.25);
					}
					num2 *= num3;
				}
				return num2;
			}
			return 1f;
		}

		// Token: 0x06002857 RID: 10327 RVA: 0x0013C360 File Offset: 0x0013A560
		public static float FuzzyDanger(NonPlayerCharacter iCharacter, Elements iElements)
		{
			if (iCharacter.PlayState.EntityManager != null)
			{
				List<Entity> entities = iCharacter.PlayState.EntityManager.GetEntities(iCharacter.Position, iCharacter.AI.AlertRadius, true);
				float num = 0f;
				for (int i = 0; i < entities.Count; i++)
				{
					Character character = entities[i] as Character;
					if ((character != null & character != iCharacter) && (character.Faction & iCharacter.Faction) == Factions.NONE)
					{
						num += FuzzyMath.FuzzyThreat(iCharacter, character, iElements);
					}
				}
				num += iCharacter.GetDanger();
				iCharacter.PlayState.EntityManager.ReturnEntityList(entities);
				if (iCharacter.ZapTimer > 0f & (iElements & Elements.Lightning) != Elements.None)
				{
					num += 1f;
				}
				return 1f - (float)Math.Pow(0.25, (double)num);
			}
			return 1f;
		}
	}
}
