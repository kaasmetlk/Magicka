using System;
using System.Collections.Generic;
using JigLibX.Geometry;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Magicka.Graphics.Lights;
using Magicka.Levels;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Spells
{
	// Token: 0x020005DE RID: 1502
	internal static class Blast
	{
		// Token: 0x06002CD1 RID: 11473 RVA: 0x0015F748 File Offset: 0x0015D948
		static Blast()
		{
			Blast.SEGMENT_LOOKUP.Add(0f, 0);
			Blast.SEGMENT_LOOKUP.Add(0.7853982f, 1);
			Blast.SEGMENT_LOOKUP.Add(1.5707964f, 2);
			Blast.SEGMENT_LOOKUP.Add(2.3561945f, 3);
			Blast.SEGMENT_LOOKUP.Add(3.1415927f, 4);
			Blast.SEGMENT_LOOKUP.Add(3.926991f, 5);
			Blast.SEGMENT_LOOKUP.Add(4.712389f, 6);
			Blast.SEGMENT_LOOKUP.Add(5.4977875f, 7);
			Blast.BLAST_EFFECT_LOOKUP = new int[11];
			Blast.GROUND_EFFECT_LOOKUP = new int[11];
			Blast.GROUND_EFFECT_LOOKUP[0] = "area_ground_earth".GetHashCodeCustom();
			for (int i = 1; i < Blast.GROUND_EFFECT_LOOKUP.Length; i++)
			{
				Elements elements = Spell.ElementFromIndex(i);
				Blast.GROUND_EFFECT_LOOKUP[i] = ("area_ground_" + elements.ToString().ToLowerInvariant()).GetHashCodeCustom();
			}
			Blast.BLAST_EFFECT_LOOKUP[0] = "area_ground_earth".GetHashCodeCustom();
			for (int j = 1; j < Blast.BLAST_EFFECT_LOOKUP.Length; j++)
			{
				Elements elements2 = Spell.ElementFromIndex(j);
				Blast.BLAST_EFFECT_LOOKUP[j] = ("area_" + elements2.ToString().ToLowerInvariant() + "1_b").GetHashCodeCustom();
			}
		}

		// Token: 0x06002CD2 RID: 11474 RVA: 0x0015F8A0 File Offset: 0x0015DAA0
		public static DamageResult FullBlast(PlayState iPlayState, Entity iOwner, double iTimeStamp, Entity iIgnoreEntity, float iRadius, Vector3 iPosition, Damage iDamage)
		{
			float[] blastScalars = Blast.GetBlastScalars(iPlayState, iOwner, iRadius, iPosition);
			if ((iDamage.Element & Elements.Beams) != Elements.None)
			{
				ArcaneBlast instance = ArcaneBlast.GetInstance();
				Spell spell;
				Spell.DefaultSpell(iDamage.Element, out spell);
				instance.Initialize(iOwner, iPosition, spell.GetColor(), iRadius, iDamage.Element);
			}
			else
			{
				for (int i = 0; i < 11; i++)
				{
					Elements elements = Spell.ElementFromIndex(i);
					if ((elements & Elements.Earth) != Elements.Earth & (iDamage.Element & elements) == elements)
					{
						Blast.BlastVisual(iPlayState, iOwner, iRadius, iPosition, iDamage.Magnitude, blastScalars, new int?(Blast.BLAST_EFFECT_LOOKUP[i]), null, null, null);
					}
				}
			}
			iPlayState.Camera.CameraShake(iPosition, iRadius * 0.333f, 0.4f);
			return Blast.BlastDamage(iPlayState, iOwner, iTimeStamp, iIgnoreEntity, iPosition, iRadius, iDamage, true);
		}

		// Token: 0x06002CD3 RID: 11475 RVA: 0x0015F98C File Offset: 0x0015DB8C
		public unsafe static DamageResult FullBlast(PlayState iPlayState, Entity iOwner, double iTimeStamp, Entity iIgnoreEntity, float iRadius, Vector3 iPosition, DamageCollection5 iDamage)
		{
			float[] blastScalars = Blast.GetBlastScalars(iPlayState, iOwner, iRadius, iPosition);
			Damage* ptr = &iDamage.A;
			if ((iDamage.GetAllElements() & Elements.Beams) != Elements.None)
			{
				ArcaneBlast instance = ArcaneBlast.GetInstance();
				Spell spell;
				Spell.DefaultSpell(iDamage.GetAllElements(), out spell);
				instance.Initialize(iOwner, iPosition, spell.GetColor(), iRadius, iDamage.GetAllElements());
			}
			else
			{
				for (int i = 0; i < 5; i++)
				{
					Decal? iDecal = null;
					float? iDecalAlpha = null;
					if (ptr[i].Element != Elements.None)
					{
						int num = MagickaMath.CountTrailingZeroBits((uint)ptr[i].Element);
						if (ptr[i].Element == Elements.Fire)
						{
							iDecal = new Decal?(Decal.Scorched);
						}
						else if (ptr[i].Element == Elements.Cold)
						{
							iDecal = new Decal?(Decal.Cold);
						}
						else if (ptr[i].Element == Elements.Water)
						{
							iDecal = new Decal?(Decal.Water);
						}
						Blast.BlastVisual(iPlayState, iOwner, iRadius, iPosition, ptr[i].Magnitude, blastScalars, new int?(Blast.BLAST_EFFECT_LOOKUP[num]), null, iDecal, iDecalAlpha);
					}
				}
			}
			iPlayState.Camera.CameraShake(iPosition, iRadius * 0.333f, 0.3f);
			return Blast.BlastDamage(iPlayState, iOwner, iTimeStamp, iIgnoreEntity, iPosition, iRadius, iDamage, true, false);
		}

		// Token: 0x06002CD4 RID: 11476 RVA: 0x0015FB00 File Offset: 0x0015DD00
		public static DamageResult BlastDamage(PlayState iPlayState, Entity iOwner, double iTimeStamp, Entity iIgnoreEntity, Vector3 iPosition, float iRadius, DamageCollection5 iDamage, bool iFalloff, bool iGroundBlast)
		{
			DamageResult result = DamageResult.None;
			List<Entity> entities = iPlayState.EntityManager.GetEntities(iPosition, iRadius, true);
			entities.Remove(iIgnoreEntity);
			Segment segment;
			segment.Origin = iPosition;
			bool flag = (iDamage.GetAllElements() & (Elements.Water | Elements.Steam)) == Elements.Water & iIgnoreEntity == iOwner;
			for (int i = 0; i < entities.Count; i++)
			{
				IDamageable damageable = entities[i] as IDamageable;
				if (damageable != null && (!(damageable is Character) || (!(damageable as Character).IsEthereal && (!iGroundBlast || !(damageable as Character).IsLevitating) && !(damageable as Character).IsEthereal)))
				{
					segment.Delta = entities[i].Position;
					Vector3.Subtract(ref segment.Delta, ref segment.Origin, out segment.Delta);
					float scaleFactor;
					Vector3 vector;
					Vector3 vector2;
					if (damageable.Body.CollisionSkin.SegmentIntersect(out scaleFactor, out vector, out vector2, segment))
					{
						Vector3.Multiply(ref segment.Delta, scaleFactor, out segment.Delta);
					}
					if (!iPlayState.Level.CurrentScene.SegmentIntersect(out scaleFactor, out vector, out vector2, segment))
					{
						float num = segment.Delta.Length();
						num -= damageable.Radius;
						if (num <= iRadius)
						{
							if (flag)
							{
								damageable.Electrocute(iOwner as IDamageable, 1f);
							}
							DamageCollection5 iDamage2 = iDamage;
							if (iFalloff && (iDamage2.GetAllElements() & Elements.Beams) == Elements.None)
							{
								float iMultiplier = Blast.BlastDamageScale(num, iRadius);
								iDamage2.MultiplyMagnitude(iMultiplier);
							}
							if (iOwner != null)
							{
								damageable.Damage(iDamage2, iOwner, iTimeStamp, iPosition);
							}
							else
							{
								damageable.Damage(iDamage2, null, iTimeStamp, iPosition);
							}
						}
					}
				}
			}
			iPlayState.EntityManager.ReturnEntityList(entities);
			Vector3 right = Vector3.Right;
			Vector3.Multiply(ref right, iRadius, out right);
			Liquid.Freeze(iPlayState.Level.CurrentScene, ref iPosition, ref right, 6.2831855f, 1f, ref iDamage);
			return result;
		}

		// Token: 0x06002CD5 RID: 11477 RVA: 0x0015FCEC File Offset: 0x0015DEEC
		public static DamageResult BlastDamage(PlayState iPlayState, Entity iOwner, double iTimeStamp, Entity iIgnoreEntity, Vector3 iPosition, float iRadius, Damage iDamage, bool iFalloff)
		{
			DamageResult damageResult = DamageResult.None;
			List<Entity> entities = iPlayState.EntityManager.GetEntities(iPosition, iRadius, true);
			entities.Remove(iIgnoreEntity);
			Segment iSeg;
			iSeg.Origin = iPosition;
			for (int i = 0; i < entities.Count; i++)
			{
				IDamageable damageable = entities[i] as IDamageable;
				if (damageable != null)
				{
					iSeg.Delta = entities[i].Position;
					Vector3.Subtract(ref iSeg.Delta, ref iSeg.Origin, out iSeg.Delta);
					float num;
					Vector3 vector;
					Vector3 vector2;
					if (!iPlayState.Level.CurrentScene.SegmentIntersect(out num, out vector, out vector2, iSeg))
					{
						float num2 = iSeg.Delta.Length();
						num2 -= damageable.Radius;
						if (num2 <= iRadius)
						{
							Damage iDamage2 = iDamage;
							if (iFalloff && (iDamage2.Element & Elements.Beams) == Elements.None)
							{
								iDamage2.Magnitude *= Blast.BlastDamageScale(num2, iRadius);
							}
							damageResult |= damageable.Damage(iDamage2, iOwner, iTimeStamp, iPosition);
						}
					}
				}
			}
			iPlayState.EntityManager.ReturnEntityList(entities);
			Vector3 right = Vector3.Right;
			Vector3.Multiply(ref right, iRadius, out right);
			Liquid.Freeze(iPlayState.Level.CurrentScene, ref iPosition, ref right, 6.2831855f, 1f, ref iDamage);
			return damageResult;
		}

		// Token: 0x06002CD6 RID: 11478 RVA: 0x0015FE2C File Offset: 0x0015E02C
		private static float BlastDamageScale(float iDistance, float iRange)
		{
			float num = Math.Max(iDistance / iRange, 0f);
			return 1f - num * num * num;
		}

		// Token: 0x06002CD7 RID: 11479 RVA: 0x0015FE54 File Offset: 0x0015E054
		public static void BlastVisual(PlayState iPlayState, Entity iOwner, float iRadius, Vector3 iPosition, float iMagnitude, float[] iFracs, int? iBlastEffect, int? iAfterEffect, Decal? iDecal, float? iDecalAlpha)
		{
			float[] array = new float[8];
			if (iFracs != null)
			{
				iFracs.CopyTo(array, 0);
			}
			else
			{
				array = Blast.GetBlastScalars(iPlayState, iOwner, iRadius, iPosition);
			}
			Vector3 up = Vector3.Up;
			for (int i = 0; i < 8; i++)
			{
				Matrix matrix;
				Matrix.CreateRotationY((float)i * 0.125f * 6.2831855f, out matrix);
				Vector3 right = matrix.Right;
				array[i] *= iRadius * 0.1f;
				matrix.Translation = iPosition;
				matrix.M11 *= array[i];
				matrix.M12 *= array[i];
				matrix.M13 *= array[i];
				matrix.M31 *= array[i];
				matrix.M32 *= array[i];
				matrix.M33 *= array[i];
				if (iBlastEffect != null)
				{
					VisualEffectReference visualEffectReference;
					EffectManager.Instance.StartEffect(iBlastEffect.Value, ref matrix, out visualEffectReference);
				}
				Vector2 vector = new Vector2(iRadius, iRadius);
				Vector3 vector2 = iPosition;
				Vector3.Multiply(ref right, iRadius * array[i], out right);
				Vector3.Add(ref vector2, ref right, out vector2);
				Segment iSeg = default(Segment);
				iSeg.Origin = vector2;
				iSeg.Delta.Y = -2f;
				float num;
				Vector3 vector3;
				AnimatedLevelPart iAnimation;
				if (iPlayState.Level.CurrentScene.SegmentIntersect(out num, out vector2, out vector3, out iAnimation, iSeg))
				{
					vector2.Y += 0.1f;
					if (iDecal != null)
					{
						float iAlpha;
						if (iDecalAlpha == null)
						{
							iAlpha = MathHelper.Clamp(iMagnitude / 5f, 0.25f, 0.75f);
						}
						else
						{
							iAlpha = iDecalAlpha.Value;
						}
						DecalManager.Instance.AddAlphaBlendedDecal(iDecal.Value, iAnimation, ref vector, ref vector2, new Vector3?(right), ref up, 60f, iAlpha);
					}
					if (iAfterEffect != null)
					{
						matrix.Translation = vector2;
						matrix.M11 *= vector.X * array[i];
						matrix.M12 *= vector.X * array[i];
						matrix.M13 *= vector.X * array[i];
						matrix.M31 *= vector.Y * array[i];
						matrix.M32 *= vector.Y * array[i];
						matrix.M33 *= vector.Y * array[i];
						VisualEffectReference visualEffectReference;
						EffectManager.Instance.StartEffect(iAfterEffect.Value, ref matrix, out visualEffectReference);
					}
				}
			}
		}

		// Token: 0x06002CD8 RID: 11480 RVA: 0x0016011A File Offset: 0x0015E31A
		public static float[] GetBlastScalars(PlayState iPlayState, Entity iOwner, float iRadius, Vector3 iPosition)
		{
			return Blast.GetBlastScalars(iPlayState, iOwner, iRadius, iPosition, false, false);
		}

		// Token: 0x06002CD9 RID: 11481 RVA: 0x00160128 File Offset: 0x0015E328
		public static float[] GetBlastScalars(PlayState iPlayState, Entity iOwner, float iRadius, Vector3 iPosition, bool iIgnoreWorld, bool iIgnoreShields)
		{
			float[] array = new float[8];
			List<Shield> shields = iPlayState.EntityManager.Shields;
			Segment segment = new Segment(iPosition, Vector3.Zero);
			for (int i = 0; i < 8; i++)
			{
				Matrix matrix;
				Matrix.CreateRotationY((float)i * 0.125f * 6.2831855f, out matrix);
				Vector3 right = matrix.Right;
				Vector3.Multiply(ref right, iRadius, out segment.Delta);
				if (iIgnoreWorld || !iPlayState.Level.CurrentScene.SegmentIntersect(out array[i], out right, out right, segment))
				{
					array[i] = 1f;
				}
				float num = array[i];
				if (!iIgnoreShields)
				{
					for (int j = 0; j < shields.Count; j++)
					{
						if (shields[j].Body.CollisionSkin.SegmentIntersect(out num, out right, out right, segment))
						{
							array[i] = ((array[i] > num) ? num : array[i]);
						}
					}
				}
			}
			return array;
		}

		// Token: 0x06002CDA RID: 11482 RVA: 0x00160219 File Offset: 0x0015E419
		private static int GetSegmentIndex(float iAngle)
		{
			iAngle *= 1.2732395f;
			iAngle = (float)Math.Floor((double)iAngle);
			iAngle *= 0.7853982f;
			return Blast.SEGMENT_LOOKUP[iAngle];
		}

		// Token: 0x06002CDB RID: 11483 RVA: 0x00160244 File Offset: 0x0015E444
		public unsafe static DamageResult GroundBlast(PlayState iPlayState, Entity iOwner, double iTimeStamp, Entity iIgnoreEntity, float iRadius, Vector3 iPosition, DamageCollection5 iDamage)
		{
			float[] blastScalars = Blast.GetBlastScalars(iPlayState, iOwner, iRadius, iPosition, true, true);
			Damage* ptr = &iDamage.A;
			Spell spell;
			Spell.DefaultSpell(iDamage.GetAllElements() & ~Elements.Earth, out spell);
			if (spell.TotalMagnitude() > 0f)
			{
				DynamicLight cachedLight = DynamicLight.GetCachedLight();
				cachedLight.Initialize(iPosition, spell.GetColor(), 6f, iRadius * 2f, 4f, 1f, 1f);
				cachedLight.Enable();
			}
			for (int i = 0; i < 5; i++)
			{
				Decal? iDecal = null;
				float? iDecalAlpha = null;
				if (ptr[i].Element != Elements.None)
				{
					int num = MagickaMath.CountTrailingZeroBits((uint)ptr[i].Element);
					if ((ptr[i].Element & Elements.PhysicalElements) != Elements.None)
					{
						iDecal = new Decal?(Decal.Cracks);
						iDecalAlpha = new float?(1f);
					}
					if (ptr[i].Element == Elements.Fire)
					{
						iDecal = new Decal?(Decal.Scorched);
					}
					else if (ptr[i].Element == Elements.Cold)
					{
						iDecal = new Decal?(Decal.Cold);
					}
					else if (ptr[i].Element == Elements.Water)
					{
						iDecal = new Decal?(Decal.Water);
					}
					if ((ptr[i].Element & Elements.Ice) != Elements.None)
					{
						IceSpikes.GetInstance().Initialize(iOwner.PlayState, ref iPosition, iRadius);
					}
					Blast.BlastVisual(iPlayState, iOwner, iRadius, iPosition, ptr[i].Magnitude, blastScalars, new int?(Blast.GROUND_EFFECT_LOOKUP[num]), null, iDecal, iDecalAlpha);
				}
			}
			iPlayState.Camera.CameraShake(iPosition, 2f, 0.9f);
			return Blast.BlastDamage(iPlayState, iOwner, iTimeStamp, iIgnoreEntity, iPosition, iRadius, iDamage, false, true);
		}

		// Token: 0x04003087 RID: 12423
		private const int SEGMENTS = 8;

		// Token: 0x04003088 RID: 12424
		private const float SEGMENTS_DIVISOR = 0.125f;

		// Token: 0x04003089 RID: 12425
		private static readonly Dictionary<float, int> SEGMENT_LOOKUP = new Dictionary<float, int>(8);

		// Token: 0x0400308A RID: 12426
		private static readonly int[] BLAST_EFFECT_LOOKUP;

		// Token: 0x0400308B RID: 12427
		private static readonly int[] GROUND_EFFECT_LOOKUP;
	}
}
