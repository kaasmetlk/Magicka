using System;
using System.Collections.Generic;
using System.Globalization;
using JigLibX.Geometry;
using Magicka.GameLogic.Entities;
using Microsoft.Xna.Framework;

namespace Magicka.AI.Arithmetics
{
	// Token: 0x0200046E RID: 1134
	internal class Variable : Expression
	{
		// Token: 0x0600225F RID: 8799 RVA: 0x000F5AD0 File Offset: 0x000F3CD0
		public Variable(string iText)
		{
			string[] array = iText.Replace(" ", "").Split(new char[]
			{
				',',
				'/',
				'(',
				')'
			}, StringSplitOptions.RemoveEmptyEntries);
			int num = 0;
			try
			{
				string[] array2 = array[num].Split(new char[]
				{
					'.'
				});
				this.Scope = (Variable.VariableScope)Enum.Parse(typeof(Variable.VariableScope), array2[0], true);
				string[] array3 = new string[array.Length + 1];
				array3[0] = array2[0];
				array3[1] = array2[1];
				for (int i = 1; i < array.Length; i++)
				{
					array3[i + 1] = array[i];
				}
				array = array3;
				num++;
			}
			catch
			{
				this.Scope = Variable.VariableScope.Global;
			}
			this.Type = (Variable.VariableType)Enum.Parse(typeof(Variable.VariableType), array[num++], true);
			Variable.VariableType type = this.Type;
			switch (type)
			{
			case Variable.VariableType.DistanceLinear:
			case Variable.VariableType.DistanceExponential:
				this.ArgumentSingle2 = float.Parse(array[num++], CultureInfo.InvariantCulture.NumberFormat);
				if (num < array.Length)
				{
					this.ArgumentSingle1 = this.ArgumentSingle2;
					this.ArgumentSingle2 = float.Parse(array[num++], CultureInfo.InvariantCulture.NumberFormat);
					return;
				}
				this.ArgumentSingle1 = -this.ArgumentSingle2;
				return;
			case Variable.VariableType.Behind:
			case Variable.VariableType.Gripped:
			case Variable.VariableType.Gripping:
			case Variable.VariableType.Entangled:
			case Variable.VariableType.Speed:
			case Variable.VariableType.Health:
				break;
			case Variable.VariableType.Danger:
			case Variable.VariableType.Threat:
				array[num] = array[num].Replace('|', ',');
				this.ArgumentElement = (Elements)Enum.Parse(typeof(Elements), array[num++], true);
				return;
			case Variable.VariableType.HasStatus:
				this.ArgumentStatus = (StatusEffects)Enum.Parse(typeof(StatusEffects), array[num++], true);
				return;
			case Variable.VariableType.FriendlyDensity:
			case Variable.VariableType.EnemyDensity:
				if (num < array.Length)
				{
					this.ArgumentSingle1 = float.Parse(array[num++], CultureInfo.InvariantCulture.NumberFormat);
					return;
				}
				this.ArgumentSingle1 = 6f;
				return;
			default:
				if (type != Variable.VariableType.LOSC)
				{
					return;
				}
				this.ArgumentSingle1 = 0f;
				this.ArgumentSingle2 = 0f;
				this.ArgumentSingle3 = 0f;
				this.ArgumentSingle4 = -1f;
				if (array.Length > 1)
				{
					this.ArgumentSingle1 = float.Parse(array[num++], CultureInfo.InvariantCulture.NumberFormat);
					if (array.Length > 2)
					{
						this.ArgumentSingle2 = float.Parse(array[num++], CultureInfo.InvariantCulture.NumberFormat);
						if (array.Length > 3)
						{
							this.ArgumentSingle3 = float.Parse(array[num++], CultureInfo.InvariantCulture.NumberFormat);
							if (array.Length > 4)
							{
								this.ArgumentSingle4 = float.Parse(array[num++], CultureInfo.InvariantCulture.NumberFormat);
							}
						}
					}
				}
				break;
			}
		}

		// Token: 0x06002260 RID: 8800 RVA: 0x000F5D9C File Offset: 0x000F3F9C
		public override float GetValue(ref ExpressionArguments iArgs)
		{
			bool flag = false;
			switch (this.Type)
			{
			case Variable.VariableType.DistanceLinear:
				return FuzzyMath.FuzzyDistanceLinear(iArgs.Distance, this.ArgumentSingle1, this.ArgumentSingle2);
			case Variable.VariableType.DistanceExponential:
				return FuzzyMath.FuzzyDistanceExponential(iArgs.Distance, this.ArgumentSingle1, this.ArgumentSingle2);
			case Variable.VariableType.Behind:
				return FuzzyMath.FuzzyAngle(ref iArgs.DeltaNormalized, ref iArgs.TargetDir);
			case Variable.VariableType.Danger:
				return FuzzyMath.FuzzyDanger(iArgs.AI.NPC, this.ArgumentElement);
			case Variable.VariableType.HasStatus:
				if (this.Scope == Variable.VariableScope.Self)
				{
					flag = iArgs.AI.Owner.HasStatus(this.ArgumentStatus);
				}
				else
				{
					if (this.Scope != Variable.VariableScope.Target)
					{
						throw new Exception();
					}
					IStatusEffected statusEffected = iArgs.Target as IStatusEffected;
					flag = (statusEffected != null && statusEffected.HasStatus(this.ArgumentStatus));
				}
				break;
			case Variable.VariableType.Gripped:
				if (this.Scope == Variable.VariableScope.Self)
				{
					flag = iArgs.AI.Owner.IsGripped;
				}
				else
				{
					if (this.Scope != Variable.VariableScope.Target)
					{
						throw new Exception();
					}
					Character character = iArgs.Target as Character;
					flag = (character != null && character.IsGripped);
				}
				break;
			case Variable.VariableType.Gripping:
				if (this.Scope == Variable.VariableScope.Self)
				{
					flag = iArgs.AI.Owner.IsGripping;
				}
				else
				{
					if (this.Scope != Variable.VariableScope.Target)
					{
						throw new Exception();
					}
					Character character2 = iArgs.Target as Character;
					flag = (character2 != null && character2.IsGripping);
				}
				break;
			case Variable.VariableType.Entangled:
				if (this.Scope == Variable.VariableScope.Self)
				{
					flag = iArgs.AI.Owner.IsEntangled;
				}
				else
				{
					if (this.Scope != Variable.VariableScope.Target)
					{
						throw new Exception();
					}
					Character character3 = iArgs.Target as Character;
					flag = (character3 != null && character3.IsEntangled);
				}
				break;
			case Variable.VariableType.Speed:
			{
				if (this.Scope == Variable.VariableScope.Self)
				{
					return iArgs.AI.Owner.CharacterBody.Movement.Length();
				}
				if (this.Scope != Variable.VariableScope.Target)
				{
					throw new Exception();
				}
				Character character4 = iArgs.Target as Character;
				if (character4 == null)
				{
					return 0f;
				}
				return character4.CharacterBody.Movement.Length();
			}
			case Variable.VariableType.Health:
				if (this.Scope == Variable.VariableScope.Self)
				{
					return iArgs.AI.Owner.HitPoints / iArgs.AI.Owner.MaxHitPoints;
				}
				return iArgs.Target.HitPoints / iArgs.Target.MaxHitPoints;
			case Variable.VariableType.FriendlyDensity:
				return FuzzyMath.FuzzyFriendlyDensity((this.Scope == Variable.VariableScope.Self) ? iArgs.AI.Owner : (iArgs.Target as Entity), iArgs.AI.Owner.Faction, this.ArgumentSingle1);
			case Variable.VariableType.EnemyDensity:
				return FuzzyMath.FuzzyEnemyDensity((this.Scope == Variable.VariableScope.Self) ? iArgs.AI.Owner : (iArgs.Target as Entity), iArgs.AI.Owner.Faction, this.ArgumentSingle1);
			case Variable.VariableType.Threat:
			{
				if (this.Scope != Variable.VariableScope.Target)
				{
					throw new Exception("Invalid scope!");
				}
				Character character5 = iArgs.Target as Character;
				if (character5 != null)
				{
					return FuzzyMath.FuzzyThreat(iArgs.AI.NPC, character5, this.ArgumentElement);
				}
				return 0f;
			}
			case Variable.VariableType.LOS:
			{
				Segment segment = default(Segment);
				segment.Origin = iArgs.AIPos;
				segment.Delta = iArgs.Delta;
				Vector3 iCenter;
				segment.GetPoint(0.5f, out iCenter);
				List<Entity> entities = iArgs.AI.Owner.PlayState.EntityManager.GetEntities(iCenter, iArgs.Distance * 0.5f, false);
				bool flag2 = false;
				int num = 0;
				while (num < entities.Count && !flag2)
				{
					Entity entity = entities[num];
					if (entity != iArgs.AI.Owner && entity != iArgs.Target)
					{
						if (entity is IDamageable)
						{
							Vector3 vector;
							flag2 = (entity as IDamageable).SegmentIntersect(out vector, segment, iArgs.AI.Owner.Radius);
						}
						else
						{
							Vector3 vector;
							float num2;
							Vector3 vector2;
							flag2 = entity.Body.CollisionSkin.SegmentIntersect(out num2, out vector, out vector2, segment);
						}
					}
					num++;
				}
				iArgs.AI.Owner.PlayState.EntityManager.ReturnEntityList(entities);
				if (!flag2)
				{
					return 1f;
				}
				return 0f;
			}
			case Variable.VariableType.IsCharacter:
				if (this.Scope == Variable.VariableScope.Target)
				{
					if (!(iArgs.Target is Character))
					{
						return 0f;
					}
					return 1f;
				}
				else
				{
					if (this.Scope == Variable.VariableScope.Self)
					{
						return 1f;
					}
					return 0f;
				}
				break;
			case Variable.VariableType.IsEthereal:
				if (this.Scope == Variable.VariableScope.Target)
				{
					if (!(iArgs.Target is Character) || !(iArgs.Target as Character).IsEthereal)
					{
						return 0f;
					}
					return 1f;
				}
				else
				{
					if (iArgs.AI.Owner == null || !iArgs.AI.Owner.IsEthereal)
					{
						return 0f;
					}
					return 1f;
				}
				break;
			case Variable.VariableType.Resistance:
			{
				if (this.Scope == Variable.VariableScope.Target)
				{
					float result = 0f;
					if (iArgs.Target != null)
					{
						result = 1f - iArgs.Target.ResistanceAgainst(this.ArgumentElement);
					}
					return result;
				}
				float result2 = 0f;
				if (iArgs.AI.Owner != null)
				{
					result2 = 1f - ((IDamageable)iArgs.AI.Owner).ResistanceAgainst(this.ArgumentElement);
				}
				return result2;
			}
			case Variable.VariableType.Shielded:
				if (this.Scope == Variable.VariableScope.Target)
				{
					if (!(iArgs.Target is Character) || !(iArgs.Target as Character).IsSelfShielded)
					{
						return 0f;
					}
					return 1f;
				}
				else if (this.Scope == Variable.VariableScope.Self)
				{
					if (iArgs.AI.Owner == null || !iArgs.AI.Owner.IsSelfShielded)
					{
						return 0f;
					}
					return 1f;
				}
				else if (this.Scope == Variable.VariableScope.Global)
				{
				}
				break;
			case Variable.VariableType.LOSC:
			{
				Segment iSeg = default(Segment);
				iSeg.Origin = iArgs.AIPos;
				iSeg.Origin.X = iSeg.Origin.X + iArgs.AIDir.X * this.ArgumentSingle1;
				iSeg.Origin.Y = iSeg.Origin.Y + this.ArgumentSingle2;
				iSeg.Origin.Z = iSeg.Origin.Z + iArgs.AIDir.Z * this.ArgumentSingle3;
				if (this.ArgumentSingle4 <= 0f)
				{
					iSeg.Delta = iArgs.Delta;
				}
				else
				{
					float num3 = (this.ArgumentSingle4 > iArgs.Distance) ? iArgs.Distance : this.ArgumentSingle4;
					iSeg.Delta.X = iArgs.DeltaNormalized.X * num3;
					iSeg.Delta.Y = iArgs.DeltaNormalized.Y * num3;
					iSeg.Delta.Z = iArgs.DeltaNormalized.Z * num3;
				}
				float num4;
				Vector3 vector3;
				Vector3 vector4;
				if (iArgs.AI.Owner.PlayState.Level.CurrentScene.SegmentIntersect(out num4, out vector3, out vector4, iSeg))
				{
					return 0f;
				}
				return 1f;
			}
			case Variable.VariableType.IsShield:
				if (this.Scope != Variable.VariableScope.Target)
				{
					return 0f;
				}
				if (!(iArgs.Target is Shield))
				{
					return 0f;
				}
				return 1f;
			case Variable.VariableType.IsBarrier:
				if (this.Scope != Variable.VariableScope.Target)
				{
					return 0f;
				}
				if (!(iArgs.Target is Barrier))
				{
					return 0f;
				}
				return 1f;
			case Variable.VariableType.GripDamageAccumulation:
				if (iArgs.AI.Owner.IsGripping)
				{
					return iArgs.AI.Owner.GripDamageAccumulation;
				}
				return 0f;
			case Variable.VariableType.GripDamageAccumulationNormalized:
				if (iArgs.AI.Owner.IsGripping)
				{
					return iArgs.AI.Owner.GripDamageAccumulation / iArgs.AI.Owner.HitTolerance;
				}
				return 0f;
			case Variable.VariableType.IsOnGround:
			{
				Character character6 = null;
				if (this.Scope == Variable.VariableScope.Target)
				{
					character6 = (iArgs.Target as Character);
				}
				else if (this.Scope == Variable.VariableScope.Self)
				{
					character6 = iArgs.AI.Owner;
				}
				if (character6 == null)
				{
					return 0f;
				}
				if (!character6.CharacterBody.IsTouchingSolidGround)
				{
					return 0f;
				}
				return 1f;
			}
			case Variable.VariableType.Angle:
			{
				Vector3 delta = iArgs.Delta;
				Vector3 aidir = iArgs.AIDir;
				delta.Y = 0f;
				aidir.Y = 0f;
				float result3 = 0f;
				float num5 = 1E-06f;
				if (delta.LengthSquared() > num5 && aidir.LengthSquared() > num5)
				{
					delta.Normalize();
					aidir.Normalize();
					Vector3.Dot(ref delta, ref aidir, out result3);
				}
				return result3;
			}
			default:
				return 0f;
			}
			if (!flag)
			{
				return 0f;
			}
			return 1f;
		}

		// Token: 0x0400256F RID: 9583
		public Variable.VariableScope Scope;

		// Token: 0x04002570 RID: 9584
		public Variable.VariableType Type;

		// Token: 0x04002571 RID: 9585
		public float ArgumentSingle1;

		// Token: 0x04002572 RID: 9586
		public float ArgumentSingle2;

		// Token: 0x04002573 RID: 9587
		public float ArgumentSingle3;

		// Token: 0x04002574 RID: 9588
		public float ArgumentSingle4;

		// Token: 0x04002575 RID: 9589
		public StatusEffects ArgumentStatus;

		// Token: 0x04002576 RID: 9590
		public Elements ArgumentElement;

		// Token: 0x0200046F RID: 1135
		public enum VariableType
		{
			// Token: 0x04002578 RID: 9592
			DistanceLinear,
			// Token: 0x04002579 RID: 9593
			DistLin = 0,
			// Token: 0x0400257A RID: 9594
			DistanceExponential,
			// Token: 0x0400257B RID: 9595
			DistExp = 1,
			// Token: 0x0400257C RID: 9596
			Behind,
			// Token: 0x0400257D RID: 9597
			Danger,
			// Token: 0x0400257E RID: 9598
			HasStatus,
			// Token: 0x0400257F RID: 9599
			Status = 4,
			// Token: 0x04002580 RID: 9600
			Gripped,
			// Token: 0x04002581 RID: 9601
			Gripping,
			// Token: 0x04002582 RID: 9602
			Entangled,
			// Token: 0x04002583 RID: 9603
			Speed,
			// Token: 0x04002584 RID: 9604
			Health,
			// Token: 0x04002585 RID: 9605
			FriendlyDensity,
			// Token: 0x04002586 RID: 9606
			FDensity = 10,
			// Token: 0x04002587 RID: 9607
			EnemyDensity,
			// Token: 0x04002588 RID: 9608
			EDensity = 11,
			// Token: 0x04002589 RID: 9609
			Threat,
			// Token: 0x0400258A RID: 9610
			LOS,
			// Token: 0x0400258B RID: 9611
			LineOfSight = 13,
			// Token: 0x0400258C RID: 9612
			IsCharacter,
			// Token: 0x0400258D RID: 9613
			Character = 14,
			// Token: 0x0400258E RID: 9614
			IsC = 14,
			// Token: 0x0400258F RID: 9615
			IsEthereal,
			// Token: 0x04002590 RID: 9616
			Ethereal = 15,
			// Token: 0x04002591 RID: 9617
			Resistance,
			// Token: 0x04002592 RID: 9618
			Res = 16,
			// Token: 0x04002593 RID: 9619
			Shielded,
			// Token: 0x04002594 RID: 9620
			LOSC,
			// Token: 0x04002595 RID: 9621
			LineOfSightCollision = 18,
			// Token: 0x04002596 RID: 9622
			IsShield,
			// Token: 0x04002597 RID: 9623
			IsS = 19,
			// Token: 0x04002598 RID: 9624
			Shield = 19,
			// Token: 0x04002599 RID: 9625
			IsBarrier,
			// Token: 0x0400259A RID: 9626
			IsB = 20,
			// Token: 0x0400259B RID: 9627
			Barrier = 20,
			// Token: 0x0400259C RID: 9628
			GripDamageAccumulation,
			// Token: 0x0400259D RID: 9629
			GripDmgAcc = 21,
			// Token: 0x0400259E RID: 9630
			GripDamageAccumulationNormalized,
			// Token: 0x0400259F RID: 9631
			GripDmgAccNrm = 22,
			// Token: 0x040025A0 RID: 9632
			IsOnGround,
			// Token: 0x040025A1 RID: 9633
			Angle
		}

		// Token: 0x02000470 RID: 1136
		public enum VariableScope
		{
			// Token: 0x040025A3 RID: 9635
			Global,
			// Token: 0x040025A4 RID: 9636
			G = 0,
			// Token: 0x040025A5 RID: 9637
			Self,
			// Token: 0x040025A6 RID: 9638
			S = 1,
			// Token: 0x040025A7 RID: 9639
			Target,
			// Token: 0x040025A8 RID: 9640
			T = 2
		}
	}
}
