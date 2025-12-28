using System;
using System.Collections.Generic;
using JigLibX.Geometry;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.Graphics;
using Magicka.Graphics.Lights;
using Magicka.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PolygonHead;
using PolygonHead.Lights;

namespace Magicka.GameLogic.Spells.SpellEffects
{
	// Token: 0x020003F9 RID: 1017
	internal class SpraySpell : SpellEffect
	{
		// Token: 0x06001F55 RID: 8021 RVA: 0x000DB660 File Offset: 0x000D9860
		public static void IntializeCache(int iNum)
		{
			SpraySpell.mCache = new List<SpraySpell>(iNum);
			for (int i = 0; i < iNum; i++)
			{
				SpraySpell.mCache.Add(new SpraySpell());
			}
		}

		// Token: 0x06001F56 RID: 8022 RVA: 0x000DB694 File Offset: 0x000D9894
		public static SpellEffect GetFromCache()
		{
			SpraySpell spraySpell = SpraySpell.mCache[SpraySpell.mCache.Count - 1];
			SpraySpell.mCache.Remove(spraySpell);
			SpellEffect.mPlayState.SpellEffects.Add(spraySpell);
			return spraySpell;
		}

		// Token: 0x06001F57 RID: 8023 RVA: 0x000DB6D8 File Offset: 0x000D98D8
		public static void ReturnToCache(SpraySpell iEffect)
		{
			iEffect.mHitList.Clear();
			foreach (Cue cue in iEffect.mSpellCues)
			{
				if (!cue.IsStopped || !cue.IsStopping)
				{
					cue.Stop(AudioStopOptions.AsAuthored);
				}
			}
			iEffect.mSpellCues.Clear();
			SpellEffect.mPlayState.SpellEffects.Remove(iEffect);
			SpraySpell.mCache.Add(iEffect);
		}

		// Token: 0x06001F58 RID: 8024 RVA: 0x000DB770 File Offset: 0x000D9970
		static SpraySpell()
		{
			SpraySpell.SPRAY_EFFECTS = new int[11];
			SpraySpell.SWORD_EFFECTS = new int[11];
			int i = 0;
			while (i < 11)
			{
				Elements elements = Spell.ElementFromIndex(i);
				string arg;
				string iString;
				if (elements <= Elements.Fire)
				{
					switch (elements)
					{
					case Elements.Water:
						arg = "spray_water";
						iString = "weapon_spray_water";
						break;
					case Elements.Earth | Elements.Water:
						goto IL_68;
					case Elements.Cold:
						arg = "spray_cold";
						iString = "weapon_spray_cold";
						break;
					default:
						if (elements != Elements.Fire)
						{
							goto IL_68;
						}
						arg = "spray_fire";
						iString = "weapon_spray_fire";
						break;
					}
				}
				else if (elements != Elements.Lightning)
				{
					if (elements != Elements.Steam)
					{
						if (elements != Elements.Poison)
						{
							goto IL_68;
						}
						arg = "spray_poison";
						iString = "weapon_spray_poison";
					}
					else
					{
						arg = "spray_steam";
						iString = "weapon_spray_steam";
					}
				}
				else
				{
					arg = "spray_lightning";
					iString = "weapon_spray_lightning";
				}
				IL_C8:
				SpraySpell.SPRAY_EFFECTS[i] = (arg + 1).GetHashCodeCustom();
				SpraySpell.SWORD_EFFECTS[i] = iString.GetHashCodeCustom();
				i++;
				continue;
				IL_68:
				arg = "";
				iString = "";
				goto IL_C8;
			}
		}

		// Token: 0x06001F59 RID: 8025 RVA: 0x000DB878 File Offset: 0x000D9A78
		public SpraySpell()
		{
			this.mVisualEffects = new List<VisualEffectReference>(8);
			this.mHitList = new HitList(32);
			this.mSpellCues = new List<Cue>(5);
			this.mSprayDecals = new Dictionary<int, DecalManager.DecalReference[]>(24);
			this.mScourgeEffects = new Dictionary<int, VisualEffectReference[]>(24);
			for (int i = 0; i < 24; i++)
			{
				DecalManager.DecalReference[] array = new DecalManager.DecalReference[4];
				for (int j = 0; j < array.Length; j++)
				{
					array[j].Index = -1;
				}
				this.mSprayDecals.Add(i, array);
				VisualEffectReference[] array2 = new VisualEffectReference[2];
				for (int k = 0; k < array2.Length; k++)
				{
					array2[k].ID = -1;
				}
				this.mScourgeEffects.Add(i, array2);
			}
		}

		// Token: 0x06001F5A RID: 8026 RVA: 0x000DB93C File Offset: 0x000D9B3C
		public override void CastArea(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
		{
			base.CastArea(iSpell, iOwner, iFromStaff);
			this.mCastType = CastType.Area;
			this.mOwner = iOwner;
			this.mTimeStamp = iOwner.PlayState.PlayTime;
			this.mMaxTTL = this.mMinTTL;
			this.mAngle = 6.2831855f;
			this.mRange = iSpell.BlastSize() * 10f;
			this.mRangeMultiplier = 0f;
			this.mDamageCollection = default(DamageCollection5);
			this.mSpell = iSpell;
			this.mSpell.CalculateDamage(SpellType.Spray, CastType.Area, out this.mDamageCollection);
			base.PlaySound(this.mSpell.GetSpellType(), this.mCastType, iOwner);
			Blast.FullBlast(iOwner.PlayState, iOwner as Entity, this.mTimeStamp, iOwner as Entity, this.mRange, iOwner.Position, this.mDamageCollection);
			if ((this.mDamageCollection.GetAllElements() & (Elements.Water | Elements.Steam)) == Elements.Water)
			{
				this.mOwner.Electrocute(this.mOwner, 1f);
			}
			if (iOwner.HasStatus(StatusEffects.Greased))
			{
				if ((this.mDamageCollection.A.Element & Elements.Fire) == Elements.Fire)
				{
					iOwner.Damage(this.mDamageCollection.A, iOwner as Entity, this.mTimeStamp, iOwner.CastSource.Translation);
					return;
				}
				if ((this.mDamageCollection.B.Element & Elements.Fire) == Elements.Fire)
				{
					iOwner.Damage(this.mDamageCollection.B, iOwner as Entity, this.mTimeStamp, iOwner.CastSource.Translation);
					return;
				}
				if ((this.mDamageCollection.C.Element & Elements.Fire) == Elements.Fire)
				{
					iOwner.Damage(this.mDamageCollection.C, iOwner as Entity, this.mTimeStamp, iOwner.CastSource.Translation);
					return;
				}
				if ((this.mDamageCollection.D.Element & Elements.Fire) == Elements.Fire)
				{
					iOwner.Damage(this.mDamageCollection.D, iOwner as Entity, this.mTimeStamp, iOwner.CastSource.Translation);
				}
			}
		}

		// Token: 0x06001F5B RID: 8027 RVA: 0x000DBB54 File Offset: 0x000D9D54
		public override void CastForce(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
		{
			base.CastForce(iSpell, iOwner, iFromStaff);
			if (iOwner is Magicka.GameLogic.Entities.Character && !((iOwner as Magicka.GameLogic.Entities.Character).CurrentState is PanicCastState) && this.mFromStaff)
			{
				this.mFromStaff = false;
			}
			this.mCastType = CastType.Force;
			this.mOwner = iOwner;
			this.mTimeStamp = iOwner.PlayState.PlayTime;
			this.mTTL = (this.mMaxTTL = 4f);
			this.mSpell = iSpell;
			this.mAngle = 0.31415927f;
			this.mRange = 5f + iSpell.TotalSprayMagnitude() / 5f * 10f / 2f;
			this.mRangeMultiplier = 0f;
			this.mDamageCollection = default(DamageCollection5);
			this.mSpell.CalculateDamage(SpellType.Spray, CastType.Force, out this.mDamageCollection);
			base.PlaySound(SpellType.Spray, CastType.Force, iOwner);
			Vector3 diffuseColor = default(Vector3);
			Vector3 ambientColor = Spell.ARCANECOLOR;
			Vector3.Multiply(ref ambientColor, iSpell[Elements.Arcane], out ambientColor);
			Vector3.Add(ref ambientColor, ref diffuseColor, out diffuseColor);
			ambientColor = Spell.COLDCOLOR;
			Vector3.Multiply(ref ambientColor, iSpell[Elements.Cold], out ambientColor);
			Vector3.Add(ref ambientColor, ref diffuseColor, out diffuseColor);
			ambientColor = Spell.FIRECOLOR;
			Vector3.Multiply(ref ambientColor, iSpell[Elements.Fire], out ambientColor);
			Vector3.Add(ref ambientColor, ref diffuseColor, out diffuseColor);
			ambientColor = Spell.LIFECOLOR;
			Vector3.Multiply(ref ambientColor, iSpell[Elements.Life], out ambientColor);
			Vector3.Add(ref ambientColor, ref diffuseColor, out diffuseColor);
			ambientColor = Spell.LIGHTNINGCOLOR;
			Vector3.Multiply(ref ambientColor, iSpell[Elements.Lightning], out ambientColor);
			Vector3.Add(ref ambientColor, ref diffuseColor, out diffuseColor);
			if (diffuseColor.LengthSquared() > 1E-45f)
			{
				Vector3.Multiply(ref diffuseColor, 1.5f, out diffuseColor);
				Vector3.Multiply(ref diffuseColor, 0.25f, out ambientColor);
				this.mScene = iOwner.PlayState.Scene;
				this.mPointLight = DynamicLight.GetCachedLight();
				this.mPointLight.SpecularAmount = diffuseColor.Length();
				this.mPointLight.DiffuseColor = diffuseColor;
				this.mPointLight.AmbientColor = ambientColor;
				this.mPointLight.Position = iOwner.CastSource.Translation + iOwner.Direction * (this.mRange * 0.5f);
				this.mPointLight.Radius = this.mRange * 1.25f;
				this.mPointLight.Intensity = 1f;
				this.mPointLight.Enable(this.mScene, LightTransitionType.Linear, 0.5f);
			}
			if ((this.mSpell.Element & Elements.Fire) == Elements.Fire)
			{
				this.mDecalType = SpraySpell.DecalTypes.Fire;
			}
			else if ((this.mSpell.Element & Elements.Cold) == Elements.Cold)
			{
				this.mDecalType = SpraySpell.DecalTypes.Cold;
			}
			else if ((this.mSpell.Element & Elements.Water) == Elements.Water)
			{
				this.mDecalType = SpraySpell.DecalTypes.Water;
			}
			for (int i = 0; i < 11; i++)
			{
				int num = (int)iSpell[Spell.ElementFromIndex(i)];
				if (num > 0)
				{
					int iHash = SpraySpell.SPRAY_EFFECTS[i];
					Vector3 translation = iOwner.CastSource.Translation;
					Vector3 direction = iOwner.Direction;
					VisualEffectReference item;
					EffectManager.Instance.StartEffect(iHash, ref translation, ref direction, out item);
					this.mVisualEffects.Add(item);
				}
			}
			if (iOwner.HasStatus(StatusEffects.Greased))
			{
				if ((this.mDamageCollection.A.Element & Elements.Fire) == Elements.Fire)
				{
					iOwner.Damage(this.mDamageCollection.A, iOwner as Entity, this.mTimeStamp, iOwner.CastSource.Translation);
					return;
				}
				if ((this.mDamageCollection.B.Element & Elements.Fire) == Elements.Fire)
				{
					iOwner.Damage(this.mDamageCollection.B, iOwner as Entity, this.mTimeStamp, iOwner.CastSource.Translation);
					return;
				}
				if ((this.mDamageCollection.C.Element & Elements.Fire) == Elements.Fire)
				{
					iOwner.Damage(this.mDamageCollection.C, iOwner as Entity, this.mTimeStamp, iOwner.CastSource.Translation);
					return;
				}
				if ((this.mDamageCollection.D.Element & Elements.Fire) == Elements.Fire)
				{
					iOwner.Damage(this.mDamageCollection.D, iOwner as Entity, this.mTimeStamp, iOwner.CastSource.Translation);
				}
			}
		}

		// Token: 0x06001F5C RID: 8028 RVA: 0x000DBF94 File Offset: 0x000DA194
		public override void CastSelf(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
		{
			base.CastSelf(iSpell, iOwner, iFromStaff);
			this.mCastType = CastType.Self;
			this.mOwner = iOwner;
			this.mTimeStamp = iOwner.PlayState.PlayTime;
			this.mSpell = iSpell;
			this.mSpell.CalculateDamage(SpellType.Spray, CastType.Self, out this.mDamageCollection);
			this.mTTL = 1f;
			if ((this.mDamageCollection.GetAllElements() & (Elements.Water | Elements.Steam)) == Elements.Water)
			{
				this.mOwner.Electrocute(this.mOwner, 1f);
			}
			Vector3 position = iOwner.Position;
			Vector3 direction = iOwner.Direction;
			for (int i = 0; i < 11; i++)
			{
				int num = (int)iSpell[Spell.ElementFromIndex(i)];
				if (num > 0)
				{
					int iHash = SpellEffect.SelfCastEffectHash[i];
					VisualEffectReference item;
					EffectManager.Instance.StartEffect(iHash, ref position, ref direction, out item);
					this.mVisualEffects.Add(item);
				}
			}
			iOwner.Damage(this.mDamageCollection, iOwner as Entity, this.mTimeStamp, iOwner.Position);
		}

		// Token: 0x06001F5D RID: 8029 RVA: 0x000DC090 File Offset: 0x000DA290
		public override void CastWeapon(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
		{
			base.CastSelf(iSpell, iOwner, iFromStaff);
			this.mCastType = CastType.Weapon;
			this.mOwner = iOwner;
			this.mTimeStamp = iOwner.PlayState.PlayTime;
			this.mSpell = iSpell;
			this.mSpell.CalculateDamage(SpellType.Spray, CastType.Weapon, out this.mDamageCollection);
			this.mTTL = this.mMaxTTL;
			Vector3 translation = iOwner.WeaponSource.Translation;
			Vector3 direction = iOwner.Direction;
			for (int i = 0; i < 11; i++)
			{
				Elements elements = Spell.ElementFromIndex(i);
				if ((elements & Elements.StatusEffects) != Elements.None || (elements & Elements.Steam) != Elements.None)
				{
					int num = (int)iSpell[elements];
					if (num > 0)
					{
						int iHash = SpraySpell.SWORD_EFFECTS[i];
						VisualEffectReference item;
						EffectManager.Instance.StartEffect(iHash, ref translation, ref direction, out item);
						this.mVisualEffects.Add(item);
					}
				}
			}
			Helper.ArcDamage(iOwner.PlayState, iOwner as Entity, this.mTimeStamp, iOwner as Entity, ref translation, ref direction, 6f, 1.5707964f, ref this.mDamageCollection);
			if (iOwner.HasStatus(StatusEffects.Greased))
			{
				if ((this.mDamageCollection.A.Element & Elements.Fire) == Elements.Fire)
				{
					iOwner.Damage(this.mDamageCollection.A, iOwner as Entity, this.mTimeStamp, iOwner.CastSource.Translation);
					return;
				}
				if ((this.mDamageCollection.B.Element & Elements.Fire) == Elements.Fire)
				{
					iOwner.Damage(this.mDamageCollection.B, iOwner as Entity, this.mTimeStamp, iOwner.CastSource.Translation);
					return;
				}
				if ((this.mDamageCollection.C.Element & Elements.Fire) == Elements.Fire)
				{
					iOwner.Damage(this.mDamageCollection.C, iOwner as Entity, this.mTimeStamp, iOwner.CastSource.Translation);
					return;
				}
				if ((this.mDamageCollection.D.Element & Elements.Fire) == Elements.Fire)
				{
					iOwner.Damage(this.mDamageCollection.D, iOwner as Entity, this.mTimeStamp, iOwner.CastSource.Translation);
				}
			}
		}

		// Token: 0x06001F5E RID: 8030 RVA: 0x000DC2AC File Offset: 0x000DA4AC
		private void Execute(ISpellCaster iOwner, float iRange)
		{
			Vector3 iDirection;
			Vector3 translation;
			if (base.CastType == CastType.Weapon)
			{
				iDirection = iOwner.WeaponSource.Forward;
				translation = iOwner.WeaponSource.Translation;
			}
			else if (this.mFromStaff)
			{
				iDirection = iOwner.CastSource.Forward;
				translation = iOwner.CastSource.Translation;
			}
			else
			{
				iDirection = iOwner.Direction;
				translation = iOwner.CastSource.Translation;
			}
			Segment segment;
			segment.Origin = translation;
			Vector3.Multiply(ref iDirection, iRange, out segment.Delta);
			float num = iRange * 0.1f;
			float num2;
			Vector3 vector;
			Vector3 vector2;
			if (iOwner.PlayState.Level.CurrentScene.SegmentIntersect(out num2, out vector, out vector2, segment))
			{
				num *= num2;
				Vector3.Multiply(ref segment.Delta, num2, out segment.Delta);
			}
			List<Shield> shields = iOwner.PlayState.EntityManager.Shields;
			for (int i = 0; i < shields.Count; i++)
			{
				if (shields[i].Body.CollisionSkin.SegmentIntersect(out num2, out vector, out vector, segment))
				{
					num *= num2;
					Vector3.Multiply(ref segment.Delta, num2, out segment.Delta);
				}
			}
			Vector3.Multiply(ref iDirection, num * iRange, out segment.Delta);
			bool flag = (this.mDamageCollection.GetAllElements() & (Elements.Water | Elements.Steam)) == Elements.Water;
			if (flag)
			{
				this.mOwner.Electrocute(this.mOwner, 0.25f);
			}
			List<Entity> entities = iOwner.PlayState.EntityManager.GetEntities(iOwner.Position, iRange, true);
			entities.Remove(iOwner as Entity);
			DamageCollection5 iDamage = this.mDamageCollection;
			iDamage.MultiplyMagnitude(0.25f);
			for (int j = 0; j < entities.Count; j++)
			{
				Entity entity = entities[j];
				IDamageable damageable = entity as IDamageable;
				Vector3 vector3;
				if (damageable != null && !this.mHitList.ContainsKey(entity.Handle) && ((damageable is Shield && damageable.ArcIntersect(out vector3, segment.Origin, iDirection, iRange, this.mAngle, 5f)) || damageable.ArcIntersect(out vector3, segment.Origin, iDirection, iRange * num, this.mAngle, 5f)))
				{
					damageable.Damage(iDamage, iOwner as Entity, this.mTimeStamp, segment.Origin);
					this.mHitList.Add(entity.Handle, 0.25f);
					if (flag)
					{
						damageable.Electrocute(this.mOwner, 0.25f);
					}
				}
			}
			iOwner.PlayState.EntityManager.ReturnEntityList(entities);
		}

		// Token: 0x06001F5F RID: 8031 RVA: 0x000DC550 File Offset: 0x000DA750
		public override bool CastUpdate(float iDeltaTime, ISpellCaster iOwner, out float oTurnSpeed)
		{
			this.mHitList.Update(iDeltaTime);
			oTurnSpeed = 0.6f;
			Vector3 origin = iOwner.Position;
			Vector3 vector;
			if (base.CastType == CastType.Weapon)
			{
				vector = iOwner.WeaponSource.Forward;
				origin = iOwner.WeaponSource.Translation;
			}
			else if (this.mFromStaff)
			{
				vector = iOwner.CastSource.Forward;
				origin = iOwner.CastSource.Translation;
			}
			else
			{
				vector = iOwner.Direction;
				origin = iOwner.CastSource.Translation;
			}
			Avatar avatar = iOwner as Avatar;
			if (avatar != null && avatar.Player.Controller != null)
			{
				avatar.Player.Controller.Rumble(0.8f, 0.8f);
			}
			if (this.mCastType != CastType.Self)
			{
				if (this.mCastType == CastType.Force)
				{
					this.mTTL -= iDeltaTime;
					this.mRangeMultiplier += iDeltaTime * 4f;
					this.mRangeMultiplier = Math.Min(this.mRangeMultiplier, 1f);
					if (this.mTTL > 0f)
					{
						this.Execute(iOwner, this.mRange * this.mRangeMultiplier);
					}
					if (this.mFromStaff)
					{
						origin = iOwner.WeaponSource.Translation;
					}
					else
					{
						origin = iOwner.CastSource.Translation;
					}
					Vector3 up = Vector3.Up;
					Matrix matrix;
					Matrix.CreateWorld(ref origin, ref vector, ref up, out matrix);
					float num = this.mRange;
					if (this.mAngle <= 0.5f)
					{
						Segment segment;
						segment.Origin = origin;
						Vector3.Multiply(ref vector, this.mRange, out segment.Delta);
						num = this.mRange * 0.1f;
						float num2;
						Vector3 vector2;
						Vector3 vector3;
						if (iOwner.PlayState.Level.CurrentScene.SegmentIntersect(out num2, out vector2, out vector3, segment))
						{
							num *= num2;
							Vector3.Multiply(ref segment.Delta, num2, out segment.Delta);
						}
						List<Shield> shields = iOwner.PlayState.EntityManager.Shields;
						for (int i = 0; i < shields.Count; i++)
						{
							if (shields[i].Body.CollisionSkin.SegmentIntersect(out num2, out vector2, out vector2, segment))
							{
								num *= num2;
								Vector3.Multiply(ref segment.Delta, num2, out segment.Delta);
							}
						}
						List<Entity> entities = iOwner.PlayState.EntityManager.GetEntities(segment.Origin, segment.Delta.Length(), false);
						for (int j = 0; j < entities.Count; j++)
						{
							IDamageable damageable = entities[j] as IDamageable;
							if (damageable is JormungandrCollisionZone && damageable.Body.CollisionSkin.SegmentIntersect(out num2, out vector2, out vector3, segment))
							{
								num *= num2 * 1.5f;
								Vector3.Multiply(ref segment.Delta, num2, out segment.Delta);
								break;
							}
						}
						iOwner.PlayState.EntityManager.ReturnEntityList(entities);
						MagickaMath.UniformMatrixScale(ref matrix, num);
						if (this.mPointLight != null && this.mPointLight.Enabled)
						{
							Vector3 vector4 = default(Vector3);
							Vector3 position = this.mPointLight.Position;
							Vector3.Subtract(ref position, ref origin, out position);
							Matrix matrix2;
							Matrix.CreateWorld(ref vector4, ref position, ref up, out matrix2);
							Quaternion quaternion;
							Quaternion.CreateFromRotationMatrix(ref matrix2, out quaternion);
							Matrix orientation = iOwner.Body.Orientation;
							Quaternion quaternion2;
							Quaternion.CreateFromRotationMatrix(ref orientation, out quaternion2);
							Quaternion.Lerp(ref quaternion, ref quaternion2, MathHelper.Clamp(iDeltaTime * 4f, 0f, 1f), out quaternion2);
							Vector3 vector5 = default(Vector3);
							vector5.Z = -1f;
							Vector3.Transform(ref vector5, ref quaternion2, out vector5);
							Vector3.Multiply(ref vector5, this.mRange * 0.5f, out vector5);
							Vector3 position2;
							Vector3.Add(ref vector5, ref origin, out position2);
							this.mPointLight.Position = position2;
						}
					}
					if ((this.mSpell.Element & Elements.StatusEffects) != Elements.None)
					{
						Vector2 vector6 = new Vector2(vector.X, vector.Z);
						float num3 = MagickaMath.Angle(vector6);
						if (num3 < 0f)
						{
							num3 += 6.2831855f;
						}
						float num4 = num3;
						int key;
						this.NormalizeAngle(ref num4, out key);
						DecalManager.DecalReference[] array = this.mSprayDecals[key];
						VisualEffectReference[] array2 = this.mScourgeEffects[key];
						for (int k = 0; k < 4; k++)
						{
							if (array[k].Index >= 0)
							{
								DecalManager.Instance.AddAlphaBlendedDecalAlpha(ref array[k], iDeltaTime * 0.5f * (float)(k + 1));
							}
							else if (array[k].Index == -1)
							{
								Vector3 translation = iOwner.CastSource.Translation;
								float num5 = num * 10f + 2f;
								Segment iSeg = default(Segment);
								iSeg.Delta.Y = iSeg.Delta.Y - 2f;
								Vector2 vector7 = default(Vector2);
								vector7.X = (vector7.Y = Math.Max(2.5f, (float)(1 + k) * num));
								Vector3 vector8;
								Vector3.Multiply(ref vector, num5 * 0.2f + (float)k * 0.2f * num5 * 0.6f, out vector8);
								Vector3 origin2;
								Vector3.Add(ref translation, ref vector8, out origin2);
								float num6 = 0f;
								Vector3.Distance(ref origin2, ref origin, out num6);
								if (num6 < num5 - 1f)
								{
									Matrix identity = Matrix.Identity;
									iSeg.Origin = origin2;
									iSeg.Origin.X = iSeg.Origin.X + (float)(MagickaMath.Random.NextDouble() - 0.5) * vector7.X * 0.125f * (float)k;
									iSeg.Origin.Y = iSeg.Origin.Y + (float)(MagickaMath.Random.NextDouble() - 0.5) * vector7.Y * 0.125f * (float)k;
									float num7;
									Vector3 vector9;
									Vector3 vector10;
									AnimatedLevelPart iAnimation;
									if (SpellEffect.mPlayState.Level.CurrentScene.SegmentIntersect(out num7, out vector9, out vector10, out iAnimation, iSeg))
									{
										Vector3 translation2 = vector9;
										translation2.Y += 0.5f;
										identity.Translation = translation2;
										DecalManager.Instance.AddAlphaBlendedDecal(Decal.Scorched + (int)this.mDecalType, iAnimation, ref vector7, ref vector9, null, ref vector10, 60f, 0f, out array[k]);
										if (k > 1 && this.mSpell[Elements.Fire] >= 1f && !EffectManager.Instance.IsActive(ref array2[k - 2]))
										{
											EffectManager.Instance.StartEffect(SpraySpell.FIRE_SCOURGE_EFFECT, ref identity, out array2[k - 2]);
										}
									}
									else
									{
										array[k].Index = -2;
									}
								}
								else
								{
									array[k].Index = -2;
								}
							}
						}
						if (this.mSpell[Elements.Fire] >= 1f)
						{
							for (int l = 0; l < 2; l++)
							{
								EffectManager.Instance.RestartEffect(ref array2[l]);
							}
						}
					}
					for (int m = 0; m < this.mVisualEffects.Count; m++)
					{
						VisualEffectReference value = this.mVisualEffects[m];
						EffectManager.Instance.UpdateOrientation(ref value, ref matrix);
						this.mVisualEffects[m] = value;
					}
					Vector3.Multiply(ref vector, this.mRange, out vector);
					Liquid.Freeze(iOwner.PlayState.Level.CurrentScene, ref origin, ref vector, this.mAngle, iDeltaTime, ref this.mDamageCollection);
				}
				else
				{
					vector = iOwner.Direction;
					for (int n = 0; n < this.mVisualEffects.Count; n++)
					{
						VisualEffectReference value2 = this.mVisualEffects[n];
						EffectManager.Instance.UpdatePositionDirection(ref value2, ref origin, ref vector);
						this.mVisualEffects[n] = value2;
					}
				}
			}
			if (this.mTTL <= 0f || this.mMaxTTL <= 0f)
			{
				this.DeInitialize(iOwner);
				return false;
			}
			float num8;
			return base.CastUpdate(iDeltaTime, iOwner, out num8);
		}

		// Token: 0x06001F60 RID: 8032 RVA: 0x000DCD5C File Offset: 0x000DAF5C
		public override void DeInitialize(ISpellCaster iOwner)
		{
			if (!base.Active)
			{
				return;
			}
			base.Active = false;
			foreach (DecalManager.DecalReference[] array in this.mSprayDecals.Values)
			{
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Index = -1;
				}
			}
			foreach (VisualEffectReference[] array2 in this.mScourgeEffects.Values)
			{
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j].ID = -1;
				}
			}
			Avatar avatar = this.mOwner as Avatar;
			if (avatar != null && avatar.Player.Controller != null)
			{
				avatar.Player.Controller.Rumble(0.8f, 0.8f);
			}
			if (this.mPointLight != null && this.mPointLight.Enabled)
			{
				this.mPointLight.Disable(LightTransitionType.Linear, 0.5f);
				this.mPointLight = null;
			}
			this.mHitList.Clear();
			for (int k = 0; k < this.mVisualEffects.Count; k++)
			{
				VisualEffectReference visualEffectReference = this.mVisualEffects[k];
				EffectManager.Instance.Stop(ref visualEffectReference);
			}
			foreach (Cue cue in this.mSpellCues)
			{
				if (!cue.IsStopping || !cue.IsStopped)
				{
					cue.Stop(AudioStopOptions.AsAuthored);
				}
			}
			SpraySpell.ReturnToCache(this);
		}

		// Token: 0x06001F61 RID: 8033 RVA: 0x000DCF3C File Offset: 0x000DB13C
		public void NormalizeAngle(ref float oAngle, out int oKey)
		{
			oAngle *= 3.8197186f;
			oAngle = (float)Math.Floor((double)oAngle);
			oKey = (int)oAngle;
			oAngle *= 0.2617994f;
		}

		// Token: 0x040021C4 RID: 8644
		private static List<SpraySpell> mCache;

		// Token: 0x040021C5 RID: 8645
		public static readonly int[] SPRAY_EFFECTS;

		// Token: 0x040021C6 RID: 8646
		public static readonly int[] SWORD_EFFECTS;

		// Token: 0x040021C7 RID: 8647
		private static readonly int FIRE_SCOURGE_EFFECT = "fire_scourge".GetHashCodeCustom();

		// Token: 0x040021C8 RID: 8648
		private float mTTL;

		// Token: 0x040021C9 RID: 8649
		private HitList mHitList;

		// Token: 0x040021CA RID: 8650
		private float mAngle;

		// Token: 0x040021CB RID: 8651
		private float mRange;

		// Token: 0x040021CC RID: 8652
		private float mRangeMultiplier;

		// Token: 0x040021CD RID: 8653
		private DamageCollection5 mDamageCollection;

		// Token: 0x040021CE RID: 8654
		private List<VisualEffectReference> mVisualEffects;

		// Token: 0x040021CF RID: 8655
		private SpraySpell.DecalTypes mDecalType;

		// Token: 0x040021D0 RID: 8656
		private Dictionary<int, DecalManager.DecalReference[]> mSprayDecals;

		// Token: 0x040021D1 RID: 8657
		private Dictionary<int, VisualEffectReference[]> mScourgeEffects;

		// Token: 0x040021D2 RID: 8658
		private ISpellCaster mOwner;

		// Token: 0x040021D3 RID: 8659
		private DynamicLight mPointLight;

		// Token: 0x040021D4 RID: 8660
		private Scene mScene;

		// Token: 0x020003FA RID: 1018
		private enum DecalTypes
		{
			// Token: 0x040021D6 RID: 8662
			Fire,
			// Token: 0x040021D7 RID: 8663
			Cold = 2,
			// Token: 0x040021D8 RID: 8664
			Water
		}
	}
}
