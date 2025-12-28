using System;
using System.Collections.Generic;
using JigLibX.Geometry;
using Magicka.Audio;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.Entities.Items;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Magicka.GameLogic.Spells.SpellEffects
{
	// Token: 0x0200029C RID: 668
	internal class ProjectileSpell : SpellEffect
	{
		// Token: 0x060013F5 RID: 5109 RVA: 0x00079FE8 File Offset: 0x000781E8
		public static void InitializeCache(int iSize)
		{
			ProjectileSpell.mCache = new List<ProjectileSpell>(iSize);
			for (int i = 0; i < iSize; i++)
			{
				ProjectileSpell.mCache.Add(new ProjectileSpell());
			}
			ProjectileSpell.sCachedConditions = new Queue<ConditionCollection>(3);
			for (int j = 0; j < 3; j++)
			{
				ProjectileSpell.sCachedConditions.Enqueue(new ConditionCollection());
			}
		}

		// Token: 0x060013F6 RID: 5110 RVA: 0x0007A044 File Offset: 0x00078244
		public static SpellEffect GetFromCache()
		{
			ProjectileSpell projectileSpell = ProjectileSpell.mCache[ProjectileSpell.mCache.Count - 1];
			ProjectileSpell.mCache.Remove(projectileSpell);
			SpellEffect.mPlayState.SpellEffects.Add(projectileSpell);
			return projectileSpell;
		}

		// Token: 0x060013F7 RID: 5111 RVA: 0x0007A085 File Offset: 0x00078285
		public static void ReturnToCache(ProjectileSpell iEffect)
		{
			SpellEffect.mPlayState.SpellEffects.Remove(iEffect);
			ProjectileSpell.mCache.Add(iEffect);
		}

		// Token: 0x060013F9 RID: 5113 RVA: 0x0007A1E0 File Offset: 0x000783E0
		public ProjectileSpell()
		{
			this.mCastType = CastType.None;
			this.mSpellCues = new List<Cue>(8);
			if (ProjectileSpell.sEarthModels == null)
			{
				lock (Game.Instance.GraphicsDevice)
				{
					ProjectileSpell.sEarthModels = new Model[5];
					ProjectileSpell.sEarthModels[0] = Game.Instance.Content.Load<Model>("Models/Missiles/Earth0");
					ProjectileSpell.sEarthModels[1] = Game.Instance.Content.Load<Model>("Models/Missiles/Earth1");
					ProjectileSpell.sEarthModels[2] = Game.Instance.Content.Load<Model>("Models/Missiles/Earth2");
					ProjectileSpell.sEarthModels[3] = Game.Instance.Content.Load<Model>("Models/Missiles/Earth2");
					ProjectileSpell.sEarthModels[4] = Game.Instance.Content.Load<Model>("Models/Missiles/Earth3");
					ProjectileSpell.sIceModels = new Model[1];
					ProjectileSpell.sIceModels[0] = Game.Instance.Content.Load<Model>("Models/Missiles/Ice3");
					ProjectileSpell.sIceEarthModels = new Model[5];
					ProjectileSpell.sIceEarthModels[0] = Game.Instance.Content.Load<Model>("Models/Missiles/IceEarth0");
					ProjectileSpell.sIceEarthModels[1] = Game.Instance.Content.Load<Model>("Models/Missiles/IceEarth1");
					ProjectileSpell.sIceEarthModels[2] = Game.Instance.Content.Load<Model>("Models/Missiles/IceEarth2");
					ProjectileSpell.sIceEarthModels[3] = Game.Instance.Content.Load<Model>("Models/Missiles/IceEarth2");
					ProjectileSpell.sIceEarthModels[4] = Game.Instance.Content.Load<Model>("Models/Missiles/IceEarth3");
				}
			}
		}

		// Token: 0x060013FA RID: 5114 RVA: 0x0007A390 File Offset: 0x00078590
		public override void CastArea(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
		{
			base.CastArea(iSpell, iOwner, iFromStaff);
			this.mTimeStamp = iOwner.PlayState.PlayTime;
			this.mCharge = 0f;
			this.mRadius = iSpell.BlastSize() * 10f;
			this.mCoolDownBetweenSpells = 0.25f;
			this.mCastType = CastType.Area;
			this.mNumProjectilesToCast = 0;
			this.mSpell = iSpell;
			this.mCoolDown = 0f;
			DamageCollection5 iDamage;
			this.mSpell.CalculateDamage(SpellType.Projectile, CastType.Area, out iDamage);
			for (int i = 0; i < 11; i++)
			{
				Elements iElement = Spell.ElementFromIndex(i);
				int num = (int)this.mSpell[iElement];
				if (num > 0)
				{
					SpellSoundVariables iVariables = default(SpellSoundVariables);
					iVariables.mMagnitude = (float)num;
					AudioManager.Instance.PlayCue<SpellSoundVariables>(Banks.Spells, ProjectileSpell.SPELL_AREA_SOUND[i], iVariables);
				}
			}
			Blast.GroundBlast(iOwner.PlayState, iOwner as Entity, this.mTimeStamp, iOwner as Entity, this.mRadius, iOwner.Position, iDamage);
		}

		// Token: 0x060013FB RID: 5115 RVA: 0x0007A488 File Offset: 0x00078688
		public override void CastForce(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
		{
			base.CastForce(iSpell, iOwner, iFromStaff);
			this.mTimeStamp = iOwner.PlayState.PlayTime;
			if (iOwner is Avatar || iOwner is BossSpellCasterZone)
			{
				this.mCharge = iOwner.SpellPower;
			}
			else
			{
				this.mCharge = 0.5f;
			}
			this.mProjectileForce = 25f + 85f * iOwner.SpellPower;
			this.mCastType = CastType.Force;
			this.mSpell = iSpell;
			this.mMass = 1f + Math.Max(0f, 50f * this.mSpell[Elements.Earth]);
			this.mCoolDown = 0f;
			Spell spell = iSpell;
			spell.Element &= ~Elements.Earth;
			this.mRadius = spell.BlastSize() * 10f;
			SpellSoundVariables iVariables = default(SpellSoundVariables);
			iVariables.mMagnitude = iOwner.SpellPower * 3f;
			iOwner.SpellPower = 0f;
			if ((iSpell.Element & Elements.Earth) == Elements.Earth)
			{
				this.mProjectileForce *= 0.5f + 0.5f / iSpell[Elements.Earth];
				if ((iSpell.Element & Elements.Ice) == Elements.Ice)
				{
					AudioManager.Instance.PlayCue<SpellSoundVariables>(Banks.Spells, ProjectileSpell.SPELL_FORCE_SOUND[Spell.ElementIndex(Elements.Ice)], iVariables);
				}
				else
				{
					AudioManager.Instance.PlayCue<SpellSoundVariables>(Banks.Spells, ProjectileSpell.SPELL_FORCE_SOUND[Spell.ElementIndex(Elements.Earth)], iVariables);
				}
				this.mNumProjectilesToCast = 1;
				this.mNumTotalProjectiles = 1;
			}
			else
			{
				if ((iSpell.Element & Elements.Ice) != Elements.Ice)
				{
					throw new Exception("Invalid projectile spell!");
				}
				AudioManager.Instance.PlayCue<SpellSoundVariables>(Banks.Spells, ProjectileSpell.SPELL_FORCE_SOUND[Spell.ElementIndex(Elements.Ice)], iVariables);
				this.mNumProjectilesToCast = (int)(3f * iSpell.IceMagnitude);
				this.mNumTotalProjectiles = this.mNumProjectilesToCast;
			}
			this.mCoolDownBetweenSpells = (0.0875f + this.mSpell.TotalMagnitude() / 5f * 0.35f * 0.75f) / (float)this.mNumProjectilesToCast;
		}

		// Token: 0x060013FC RID: 5116 RVA: 0x0007A698 File Offset: 0x00078898
		public override void CastSelf(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
		{
			base.CastSelf(iSpell, iOwner, iFromStaff);
			this.mTimeStamp = iOwner.PlayState.PlayTime;
			this.mCharge = 0.5f;
			this.mProjectileForce = 25f + 85f * iOwner.SpellPower;
			this.mCastType = CastType.Self;
			this.mSpell = iSpell;
			this.mMass = 1f + Math.Max(0f, 50f * this.mSpell[Elements.Earth]);
			this.mCoolDown = 0f;
			Spell spell = iSpell;
			spell.Element &= ~Elements.Earth;
			this.mRadius = spell.BlastSize() * 10f;
			SpellSoundVariables iVariables = default(SpellSoundVariables);
			iVariables.mMagnitude = iOwner.SpellPower * 3f;
			iOwner.SpellPower = 0f;
			if ((iSpell.Element & Elements.Earth) == Elements.Earth)
			{
				this.mProjectileForce *= 0.5f + 0.5f / iSpell[Elements.Earth];
				if ((iSpell.Element & Elements.Ice) == Elements.Ice)
				{
					AudioManager.Instance.PlayCue<SpellSoundVariables>(Banks.Spells, ProjectileSpell.SPELL_FORCE_SOUND[Spell.ElementIndex(Elements.Ice)], iVariables);
				}
				else
				{
					AudioManager.Instance.PlayCue<SpellSoundVariables>(Banks.Spells, ProjectileSpell.SPELL_FORCE_SOUND[Spell.ElementIndex(Elements.Earth)], iVariables);
				}
				this.mNumProjectilesToCast = 1;
				this.mNumTotalProjectiles = 1;
			}
			else
			{
				if ((iSpell.Element & Elements.Ice) != Elements.Ice)
				{
					throw new Exception("Invalid projectile spell!");
				}
				AudioManager.Instance.PlayCue<SpellSoundVariables>(Banks.Spells, ProjectileSpell.SPELL_FORCE_SOUND[Spell.ElementIndex(Elements.Ice)], iVariables);
				this.mNumProjectilesToCast = (int)(3f * iSpell.IceMagnitude);
				this.mNumTotalProjectiles = this.mNumProjectilesToCast;
			}
			this.mCoolDownBetweenSpells = (0.0875f + this.mSpell.TotalMagnitude() / 5f * 0.35f * 0.75f) / (float)this.mNumProjectilesToCast;
		}

		// Token: 0x060013FD RID: 5117 RVA: 0x0007A88C File Offset: 0x00078A8C
		public override void CastWeapon(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
		{
			base.CastWeapon(iSpell, iOwner, iFromStaff);
			this.mTimeStamp = iOwner.PlayState.PlayTime;
			if (iOwner is Character && !((iOwner as Character).CurrentState is PanicCastState) && this.mFromStaff)
			{
				this.mFromStaff = false;
			}
			this.mCastType = CastType.Weapon;
			this.mSpell = iSpell;
			this.mSpell.CalculateDamage(SpellType.Projectile, CastType.Weapon, out this.mWeaponDamage);
			this.mCoolDownBetweenSpells = 0.25f;
			this.mCoolDown = 0f;
			if (this.mSpell[Elements.Ice] > 0f)
			{
				this.mCoolDownBetweenSpells = 0.25f;
				Spell spell = iSpell;
				spell.EarthMagnitude = 0f;
				spell.Element &= ~Elements.Earth;
				DamageCollection5 damageCollection;
				spell.CalculateDamage(SpellType.Projectile, CastType.Weapon, out damageCollection);
				float iRange = 1.25f * spell.IceMagnitude + 3.75f;
				this.mIceBlade = IceBlade.GetInstance();
				this.mIceBlade.Initialize((iOwner as Character).Equipment[0].Item, ref damageCollection, iRange);
			}
		}

		// Token: 0x060013FE RID: 5118 RVA: 0x0007A9A0 File Offset: 0x00078BA0
		internal override void AnimationEnd(ISpellCaster iOwner)
		{
			base.AnimationEnd(iOwner);
			if (this.mCastType == CastType.Weapon)
			{
				if (this.mIceBlade != null)
				{
					this.mIceBlade.Kill();
					this.mIceBlade = null;
				}
				if ((this.mSpell.Element & Elements.Earth) == Elements.Earth)
				{
					Spell mSpell = this.mSpell;
					mSpell.IceMagnitude = 0f;
					mSpell.Element &= ~Elements.Ice;
					Vector3 translation = iOwner.WeaponSource.Translation;
					Vector2 vector = default(Vector2);
					vector.X = iOwner.Direction.X * 40f;
					vector.Y = iOwner.Direction.Z * 40f;
					DamageCollection5 iDamage;
					mSpell.CalculateDamage(SpellType.Projectile, CastType.Weapon, out iDamage);
					UnderGroundAttack fromCache = UnderGroundAttack.GetFromCache(iOwner.PlayState);
					fromCache.Initialize(ref translation, ref vector, iOwner, this.mTimeStamp, 15f, iDamage, true);
				}
			}
		}

		// Token: 0x060013FF RID: 5119 RVA: 0x0007AA8C File Offset: 0x00078C8C
		public override bool CastUpdate(float iDeltaTime, ISpellCaster iOwner, out float oTurnSpeed)
		{
			oTurnSpeed = 0f;
			Vector3 vector = iOwner.Position;
			Vector3 vector2;
			if (this.mFromStaff)
			{
				if (base.CastType == CastType.Weapon)
				{
					vector2 = iOwner.WeaponSource.Forward;
				}
				else
				{
					vector2 = iOwner.Direction;
				}
			}
			else
			{
				vector2 = iOwner.Direction;
			}
			switch (this.mCastType)
			{
			case CastType.Force:
			case CastType.Area:
			case CastType.Self:
				this.mCoolDown -= iDeltaTime;
				while (this.mCoolDown <= 0f)
				{
					this.mCoolDown += this.mCoolDownBetweenSpells;
					vector = iOwner.CastSource.Translation;
					this.mNumProjectilesToCast--;
					bool flag = false;
					if (this.mCastType == CastType.Force)
					{
						Segment iSeg = default(Segment);
						iSeg.Origin = iOwner.Position;
						Vector3 vector3 = vector;
						Vector3.Subtract(ref vector3, ref iSeg.Origin, out iSeg.Delta);
						List<Shield> shields = iOwner.PlayState.EntityManager.Shields;
						foreach (Shield shield in shields)
						{
							Vector3 vector4;
							if (shield.SegmentIntersect(out vector4, iSeg, 0.1f + 0.075f * this.mSpell[Elements.Earth]))
							{
								int iHash;
								if (this.mSpell[Elements.Earth] > 0f)
								{
									iHash = Defines.PROJECTILE_HIT_SMALL[Defines.ElementIndex(Elements.Earth)];
								}
								else
								{
									iHash = Defines.PROJECTILE_HIT_SMALL[Defines.ElementIndex(Elements.Ice)];
								}
								Vector3 vector5;
								Vector3.Negate(ref iSeg.Delta, out vector5);
								vector5.Normalize();
								VisualEffectReference visualEffectReference;
								EffectManager.Instance.StartEffect(iHash, ref vector4, ref vector5, out visualEffectReference);
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						Vector3 velocity;
						if (this.mNumProjectilesToCast > 0)
						{
							if (this.mCastType == CastType.Self)
							{
								float num = 6f;
								float num2 = (float)SpellEffect.RANDOM.NextDouble() * 6.2831855f;
								float x = (float)((double)num * Math.Cos((double)num2));
								float z = (float)((double)num * Math.Sin((double)num2));
								Vector3 vector6 = new Vector3(x, 10f, z);
								Vector3.Add(ref vector, ref vector6, out vector);
								Vector3 position = iOwner.Position;
								Vector3.Subtract(ref position, ref vector, out vector2);
								vector2.Normalize();
							}
							else
							{
								Matrix matrix;
								Matrix.CreateRotationY(((float)SpellEffect.RANDOM.NextDouble() - 0.5f) * 1.5707964f * (1f - this.mCharge), out matrix);
								Vector3.Transform(ref vector2, ref matrix, out vector2);
							}
							float scaleFactor = this.mProjectileForce * (1.25f + 0.75f * (float)SpellEffect.RANDOM.NextDouble());
							Vector3.Multiply(ref vector2, scaleFactor, out velocity);
						}
						else
						{
							if (this.mCastType == CastType.Self)
							{
								vector.Y += 8f;
								vector2 = new Vector3(0.001f, -1f, 0f);
							}
							Vector3.Multiply(ref vector2, this.mProjectileForce, out velocity);
						}
						if (this.mNumProjectilesToCast >= 0)
						{
							NetworkState state = NetworkManager.Instance.State;
							if (((state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer))) || (state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer))) && (this.mCastType == CastType.Force || this.mCastType == CastType.Self))
							{
								MissileEntity missileEntity = null;
								ProjectileSpell.SpawnMissile(ref missileEntity, iOwner, 0f, ref vector, ref velocity, ref this.mSpell, this.mRadius, this.mNumTotalProjectiles);
								if (NetworkManager.Instance.State != NetworkState.Offline)
								{
									SpawnMissileMessage spawnMissileMessage = default(SpawnMissileMessage);
									spawnMissileMessage.Type = SpawnMissileMessage.MissileType.Spell;
									spawnMissileMessage.Handle = missileEntity.Handle;
									spawnMissileMessage.Item = (ushort)this.mNumTotalProjectiles;
									spawnMissileMessage.Owner = iOwner.Handle;
									spawnMissileMessage.Position = vector;
									spawnMissileMessage.Velocity = velocity;
									spawnMissileMessage.Spell = this.mSpell;
									spawnMissileMessage.Homing = 0f;
									spawnMissileMessage.Splash = this.mRadius;
									NetworkManager.Instance.Interface.SendMessage<SpawnMissileMessage>(ref spawnMissileMessage);
								}
							}
						}
					}
				}
				break;
			}
			if (this.mCastType != CastType.Weapon && this.mNumProjectilesToCast <= 0 && base.Active)
			{
				this.DeInitialize(iOwner);
			}
			float num3;
			return base.CastUpdate(iDeltaTime, iOwner, out num3);
		}

		// Token: 0x06001400 RID: 5120 RVA: 0x0007AF04 File Offset: 0x00079104
		public override void DeInitialize(ISpellCaster iOwner)
		{
			if (!base.Active)
			{
				return;
			}
			if (this.mIceBlade != null)
			{
				this.mIceBlade.Kill();
				this.mIceBlade = null;
			}
			base.Active = false;
			this.mCoolDown = 0f;
			ProjectileSpell.ReturnToCache(this);
		}

		// Token: 0x06001401 RID: 5121 RVA: 0x0007AF44 File Offset: 0x00079144
		public static void SpawnMissile(ref MissileEntity iMissile, ISpellCaster iOwner, float iHoming, ref Vector3 iPosition, ref Vector3 iVelocity, ref Spell iSpell, float iSplash, int iNumProjectiles)
		{
			ProjectileSpell.SpawnMissile(ref iMissile, null, iOwner, iHoming, ref iPosition, ref iVelocity, ref iSpell, iSplash, iNumProjectiles);
		}

		// Token: 0x06001402 RID: 5122 RVA: 0x0007AF64 File Offset: 0x00079164
		public static void SpawnMissile(ref MissileEntity iMissile, Model mdl, ISpellCaster iOwner, float iHoming, ref Vector3 iPosition, ref Vector3 iVelocity, ref Spell iSpell, float iSplash, int iNumProjectiles)
		{
			ConditionCollection conditionCollection;
			lock (ProjectileSpell.sCachedConditions)
			{
				conditionCollection = ProjectileSpell.sCachedConditions.Dequeue();
			}
			DamageCollection5 damageCollection;
			iSpell.CalculateDamage(SpellType.Projectile, CastType.Force, out damageCollection);
			Model model = null;
			if (mdl != null)
			{
				model = mdl;
			}
			conditionCollection.Clear();
			if ((iSpell.Element & Elements.PhysicalElements) == Elements.PhysicalElements)
			{
				if (model == null)
				{
					model = ProjectileSpell.sIceEarthModels[Math.Min(Math.Max((int)iSpell.IceMagnitude + (int)iSpell.EarthMagnitude - 1, 0), 4)];
				}
				conditionCollection[0].Condition.EventConditionType = EventConditionType.Default;
				conditionCollection[1].Condition.EventConditionType = EventConditionType.Hit;
				conditionCollection[1].Add(new EventStorage(new DamageEvent(damageCollection.A, true)));
				conditionCollection[1].Add(new EventStorage(new DamageEvent(damageCollection.B)));
				conditionCollection[1].Add(new EventStorage(new DamageEvent(damageCollection.C)));
				conditionCollection[1].Add(new EventStorage(new DamageEvent(damageCollection.D)));
				conditionCollection[1].Add(new EventStorage(new DamageEvent(damageCollection.E)));
				conditionCollection[1].Add(new EventStorage(default(RemoveEvent)));
				conditionCollection[2].Condition.EventConditionType = EventConditionType.Collision;
				conditionCollection[2].Add(new EventStorage(new SpawnDecalEvent(Decal.Crater, iSpell.EarthMagnitude + iSpell.IceMagnitude - 1f)));
				conditionCollection[2].Add(new EventStorage(new RemoveEvent(1)));
				for (int i = 0; i < 11; i++)
				{
					Elements iElement = Spell.ElementFromIndex(i);
					float num = iSpell[iElement];
					if (num > 0f)
					{
						conditionCollection[0].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Defines.SOUNDS_PROJECTILE[i], num)));
						conditionCollection[0].Add(new EventStorage(new PlayEffectEvent(Defines.PROJECTILE_TRAIL_BIG[i], true)));
						conditionCollection[1].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Defines.SOUNDS_BLAST[i], false, num)));
						conditionCollection[2].Add(new EventStorage(new PlayEffectEvent(Defines.PROJECTILE_HIT_BIG[i], false)));
						conditionCollection[2].Add(new EventStorage(new PlayEffectEvent(Defines.PROJECTILE_HIT_BIG[i], false)));
						conditionCollection[2].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Defines.SOUNDS_BLAST[i], false, num)));
					}
				}
				conditionCollection[3].Condition.EventConditionType = EventConditionType.Timer;
				conditionCollection[3].Condition.Time = 10f;
				conditionCollection[3].Add(new EventStorage(default(RemoveEvent)));
			}
			else if ((iSpell.Element & Elements.Earth) == Elements.Earth)
			{
				if (model == null)
				{
					model = ProjectileSpell.sEarthModels[Math.Min(Math.Max((int)iSpell.EarthMagnitude - 1, 0), 4)];
				}
				conditionCollection[0].Condition.EventConditionType = EventConditionType.Default;
				conditionCollection[1].Condition.EventConditionType = EventConditionType.Hit;
				conditionCollection[1].Add(new EventStorage(new DamageEvent(damageCollection.A, true)));
				conditionCollection[1].Add(new EventStorage(new DamageEvent(damageCollection.B)));
				conditionCollection[1].Add(new EventStorage(new DamageEvent(damageCollection.C)));
				conditionCollection[1].Add(new EventStorage(new DamageEvent(damageCollection.D)));
				conditionCollection[1].Add(new EventStorage(new DamageEvent(damageCollection.E)));
				conditionCollection[2].Condition.EventConditionType = EventConditionType.Collision;
				conditionCollection[2].Add(new EventStorage(new SpawnDecalEvent(Decal.Crater, 1f + (iSpell.EarthMagnitude - 1f) * 0.5f)));
				if (iSplash > 1E-45f)
				{
					Spell spell = iSpell;
					spell.EarthMagnitude = 0f;
					spell.Element &= ~Elements.Earth;
					if (spell.Element != Elements.None)
					{
						DamageCollection5 iDamage;
						iSpell.CalculateDamage(spell.GetSpellType(), CastType.Area, out iDamage);
						conditionCollection[1].Add(new EventStorage(new BlastEvent(iSplash, iDamage)));
						conditionCollection[2].Add(new EventStorage(new BlastEvent(iSplash, iDamage)));
					}
					conditionCollection[1].Add(new EventStorage(default(RemoveEvent)));
					conditionCollection[2].Add(new EventStorage(default(RemoveEvent)));
				}
				else
				{
					conditionCollection[1].Add(new EventStorage(new RemoveEvent(25)));
					conditionCollection[2].Add(new EventStorage(new RemoveEvent(1)));
				}
				for (int j = 0; j < 11; j++)
				{
					Elements iElement2 = Spell.ElementFromIndex(j);
					float num2 = iSpell[iElement2];
					if (num2 > 0f)
					{
						conditionCollection[0].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Defines.SOUNDS_PROJECTILE[j])));
						conditionCollection[0].Add(new EventStorage(new PlayEffectEvent(Defines.PROJECTILE_TRAIL_BIG[j], true)));
						if (iSplash > 0f)
						{
							conditionCollection[1].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Defines.SOUNDS_BLAST[j], false, num2)));
							conditionCollection[2].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Defines.SOUNDS_BLAST[j], false, num2)));
						}
						else
						{
							conditionCollection[2].Add(new EventStorage(new PlayEffectEvent(Defines.PROJECTILE_HIT_BIG[j], false)));
							conditionCollection[2].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Defines.SOUNDS_HIT[j], false, num2)));
						}
					}
				}
				conditionCollection[3].Condition.EventConditionType = EventConditionType.Timer;
				conditionCollection[3].Condition.Time = 10f;
				conditionCollection[3].Add(new EventStorage(default(RemoveEvent)));
			}
			else if ((iSpell.Element & Elements.Ice) == Elements.Ice)
			{
				if (model == null)
				{
					model = ProjectileSpell.sIceModels[0];
				}
				damageCollection.A.AttackProperty = (damageCollection.A.AttackProperty | AttackProperties.Piercing);
				damageCollection.B.AttackProperty = (damageCollection.B.AttackProperty | AttackProperties.Piercing);
				damageCollection.C.AttackProperty = (damageCollection.C.AttackProperty | AttackProperties.Piercing);
				damageCollection.D.AttackProperty = (damageCollection.D.AttackProperty | AttackProperties.Piercing);
				damageCollection.E.AttackProperty = (damageCollection.E.AttackProperty | AttackProperties.Piercing);
				int iHash = Defines.SOUNDS_HIT[Defines.ElementIndex(Elements.Ice)];
				conditionCollection[0].Condition.EventConditionType = EventConditionType.Default;
				for (int k = 0; k < 11; k++)
				{
					Elements iElement3 = Spell.ElementFromIndex(k);
					float num3 = iSpell[iElement3];
					if (num3 > 0f)
					{
						int iHash2 = Defines.PROJECTILE_TRAIL_SMALL[k];
						conditionCollection[0].Add(new EventStorage(new PlayEffectEvent(iHash2, true)));
						int iHash3 = Defines.PROJECTILE_HIT_SMALL[k];
						conditionCollection[1].Add(new EventStorage(new PlayEffectEvent(iHash3)));
						conditionCollection[2].Add(new EventStorage(new PlayEffectEvent(iHash3)));
					}
				}
				damageCollection.MultiplyMagnitude(1f / (float)iNumProjectiles);
				conditionCollection[1].Condition.EventConditionType = EventConditionType.Hit;
				conditionCollection[1].Add(new EventStorage(new DamageEvent(damageCollection.A, true)));
				conditionCollection[1].Add(new EventStorage(new DamageEvent(damageCollection.B)));
				conditionCollection[1].Add(new EventStorage(new DamageEvent(damageCollection.C)));
				conditionCollection[1].Add(new EventStorage(new DamageEvent(damageCollection.D)));
				conditionCollection[1].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, iHash)));
				conditionCollection[2].Condition.EventConditionType = EventConditionType.Collision;
				conditionCollection[2].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, iHash)));
				conditionCollection[2].Add(new EventStorage(default(RemoveEvent)));
				conditionCollection[3].Condition.EventConditionType = EventConditionType.Timer;
				conditionCollection[3].Condition.Time = 10f;
				conditionCollection[3].Add(new EventStorage(default(RemoveEvent)));
			}
			else
			{
				Console.WriteLine("I think projectile is a pretty cool guy, eh bounces into things and doesn't afraid of anything!");
				conditionCollection[0].Condition.EventConditionType = EventConditionType.Default;
				conditionCollection[1].Condition.EventConditionType = EventConditionType.Hit;
				conditionCollection[1].Add(new EventStorage(new DamageEvent(damageCollection.A, true)));
				conditionCollection[1].Add(new EventStorage(new DamageEvent(damageCollection.B)));
				conditionCollection[1].Add(new EventStorage(new DamageEvent(damageCollection.C)));
				conditionCollection[1].Add(new EventStorage(new DamageEvent(damageCollection.D)));
				conditionCollection[1].Add(new EventStorage(new DamageEvent(damageCollection.E)));
				conditionCollection[2].Condition.EventConditionType = EventConditionType.Collision;
				conditionCollection[2].Add(new EventStorage(new SpawnDecalEvent(Decal.Crater, 1f + (iSpell.EarthMagnitude - 1f) * 0.5f)));
				if (iSplash > 1E-45f)
				{
					Spell spell2 = iSpell;
					spell2.EarthMagnitude = 0f;
					spell2.Element &= ~Elements.Earth;
					if (spell2.Element != Elements.None)
					{
						DamageCollection5 iDamage;
						iSpell.CalculateDamage(spell2.GetSpellType(), CastType.Area, out iDamage);
						conditionCollection[1].Add(new EventStorage(new BlastEvent(iSplash, iDamage)));
						conditionCollection[2].Add(new EventStorage(new BlastEvent(iSplash, iDamage)));
					}
					conditionCollection[1].Add(new EventStorage(default(RemoveEvent)));
					conditionCollection[2].Add(new EventStorage(default(RemoveEvent)));
				}
				else
				{
					conditionCollection[1].Add(new EventStorage(new RemoveEvent(25)));
					conditionCollection[2].Add(new EventStorage(new RemoveEvent(1)));
				}
				for (int l = 0; l < 11; l++)
				{
					Elements iElement4 = Spell.ElementFromIndex(l);
					float num4 = iSpell[iElement4];
					if (num4 > 0f)
					{
						conditionCollection[0].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Defines.SOUNDS_PROJECTILE[l])));
						conditionCollection[0].Add(new EventStorage(new PlayEffectEvent(Defines.PROJECTILE_TRAIL_BIG[l], true)));
						if (iSplash > 0f)
						{
							conditionCollection[1].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Defines.SOUNDS_BLAST[l], false, num4)));
							conditionCollection[2].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Defines.SOUNDS_BLAST[l], false, num4)));
						}
						else
						{
							conditionCollection[2].Add(new EventStorage(new PlayEffectEvent(Defines.PROJECTILE_HIT_BIG[l], false)));
							conditionCollection[2].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Defines.SOUNDS_HIT[l], false, num4)));
						}
					}
				}
				conditionCollection[3].Condition.EventConditionType = EventConditionType.Timer;
				conditionCollection[3].Condition.Time = 10f;
				conditionCollection[3].Add(new EventStorage(default(RemoveEvent)));
			}
			if (iMissile == null)
			{
				iMissile = iOwner.GetMissileInstance();
			}
			Vector3 vector = default(Vector3);
			float num5 = 0f;
			if (iSpell.FireMagnitude > 1E-45f)
			{
				vector.X += Spell.FIRECOLOR.X;
				vector.Y += Spell.FIRECOLOR.Y;
				vector.Z += Spell.FIRECOLOR.Z;
				num5 += 1f;
			}
			if (iSpell.ArcaneMagnitude > 1E-45f)
			{
				vector.X += Spell.ARCANECOLOR.X;
				vector.Y += Spell.ARCANECOLOR.Y;
				vector.Z += Spell.ARCANECOLOR.Z;
				num5 += 1f;
			}
			if (iSpell.LifeMagnitude > 1E-45f)
			{
				vector.X += Spell.LIFECOLOR.X;
				vector.Y += Spell.LIFECOLOR.Y;
				vector.Z += Spell.LIFECOLOR.Z;
				num5 += 1f;
			}
			if (iSpell.LightningMagnitude > 1E-45f)
			{
				vector.X += Spell.LIGHTNINGCOLOR.X;
				vector.Y += Spell.LIGHTNINGCOLOR.Y;
				vector.Z += Spell.LIGHTNINGCOLOR.Z;
				num5 += 1f;
			}
			if (iSpell.ColdMagnitude > 1E-45f)
			{
				vector.X += Spell.COLDCOLOR.X;
				vector.Y += Spell.COLDCOLOR.Y;
				vector.Z += Spell.COLDCOLOR.Z;
				num5 += 1f;
			}
			if (num5 > 1E-45f)
			{
				Vector3.Divide(ref vector, num5, out vector);
				Vector3 iDiffuseColor;
				Vector3.Multiply(ref vector, 1.333f, out iDiffuseColor);
				Vector3 iAmbientColor;
				Vector3.Multiply(ref vector, 0.666f, out iAmbientColor);
				conditionCollection[0].Add(new EventStorage(new LightEvent(6f, iDiffuseColor, iAmbientColor, vector.Length())));
			}
			iMissile.Initialize(iOwner as Entity, 0.075f + 0.075f * iSpell[Elements.Earth], ref iPosition, ref iVelocity, model, conditionCollection, true, iSpell);
			iOwner.PlayState.EntityManager.AddEntity(iMissile);
			lock (ProjectileSpell.sCachedConditions)
			{
				ProjectileSpell.sCachedConditions.Enqueue(conditionCollection);
			}
		}

		// Token: 0x04001562 RID: 5474
		private static List<ProjectileSpell> mCache;

		// Token: 0x04001563 RID: 5475
		internal static Queue<ConditionCollection> sCachedConditions = new Queue<ConditionCollection>();

		// Token: 0x04001564 RID: 5476
		public static readonly int[] SPELL_AREA_SOUND = new int[]
		{
			"spell_earth_area".GetHashCodeCustom(),
			"spell_water_area".GetHashCodeCustom(),
			"spell_cold_area".GetHashCodeCustom(),
			"spell_fire_area".GetHashCodeCustom(),
			"spell_lightning_area".GetHashCodeCustom(),
			"spell_arcane_area".GetHashCodeCustom(),
			"spell_life_area".GetHashCodeCustom(),
			0,
			"spell_ice_area".GetHashCodeCustom(),
			"spell_steam_area".GetHashCodeCustom(),
			"spell_steam_area".GetHashCodeCustom()
		};

		// Token: 0x04001565 RID: 5477
		public static readonly int[] SPELL_FORCE_SOUND = new int[]
		{
			"spell_earth_force".GetHashCodeCustom(),
			"spell_water_force".GetHashCodeCustom(),
			"spell_cold_force".GetHashCodeCustom(),
			"spell_fire_force".GetHashCodeCustom(),
			"spell_lightning_force".GetHashCodeCustom(),
			"spell_arcane_force".GetHashCodeCustom(),
			"spell_life_force".GetHashCodeCustom(),
			0,
			"spell_ice_force".GetHashCodeCustom(),
			"spell_steam_force".GetHashCodeCustom(),
			"spell_steam_force".GetHashCodeCustom()
		};

		// Token: 0x04001566 RID: 5478
		private static Model[] sEarthModels;

		// Token: 0x04001567 RID: 5479
		private static Model[] sIceModels;

		// Token: 0x04001568 RID: 5480
		private static Model[] sIceEarthModels;

		// Token: 0x04001569 RID: 5481
		private int mNumProjectilesToCast;

		// Token: 0x0400156A RID: 5482
		private int mNumTotalProjectiles;

		// Token: 0x0400156B RID: 5483
		private float mRadius;

		// Token: 0x0400156C RID: 5484
		private float mProjectileForce;

		// Token: 0x0400156D RID: 5485
		private float mCharge;

		// Token: 0x0400156E RID: 5486
		private float mCoolDown;

		// Token: 0x0400156F RID: 5487
		private float mMass;

		// Token: 0x04001570 RID: 5488
		private float mCoolDownBetweenSpells;

		// Token: 0x04001571 RID: 5489
		private new double mTimeStamp;

		// Token: 0x04001572 RID: 5490
		private DamageCollection5 mWeaponDamage;

		// Token: 0x04001573 RID: 5491
		private IceBlade mIceBlade;
	}
}
