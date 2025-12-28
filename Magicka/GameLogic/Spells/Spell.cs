using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Magicka.Achievements;
using Magicka.Audio;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.Gamers;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Magicka.GameLogic.Spells
{
	// Token: 0x02000633 RID: 1587
	[StructLayout(LayoutKind.Auto)]
	public struct Spell : IEquatable<Spell>
	{
		// Token: 0x17000B5D RID: 2909
		public float this[Elements iElement]
		{
			get
			{
				if (iElement <= Elements.Arcane)
				{
					if (iElement <= Elements.Fire)
					{
						switch (iElement)
						{
						case Elements.Earth:
							return this.EarthMagnitude;
						case Elements.Water:
							return this.WaterMagnitude;
						case Elements.Earth | Elements.Water:
							break;
						case Elements.Cold:
							return this.ColdMagnitude;
						default:
							if (iElement == Elements.Fire)
							{
								return this.FireMagnitude;
							}
							break;
						}
					}
					else
					{
						if (iElement == Elements.Lightning)
						{
							return this.LightningMagnitude;
						}
						if (iElement == Elements.Arcane)
						{
							return this.ArcaneMagnitude;
						}
					}
				}
				else if (iElement <= Elements.Shield)
				{
					if (iElement == Elements.Life)
					{
						return this.LifeMagnitude;
					}
					if (iElement == Elements.Shield)
					{
						return this.ShieldMagnitude;
					}
				}
				else
				{
					if (iElement == Elements.Ice)
					{
						return this.IceMagnitude;
					}
					if (iElement == Elements.Steam)
					{
						return this.SteamMagnitude;
					}
					if (iElement == Elements.Poison)
					{
						return this.PoisonMagnitude;
					}
				}
				return 0f;
			}
			set
			{
				if (iElement <= Elements.Arcane)
				{
					if (iElement <= Elements.Fire)
					{
						switch (iElement)
						{
						case Elements.Earth:
							this.EarthMagnitude = value;
							return;
						case Elements.Water:
							this.WaterMagnitude = value;
							return;
						case Elements.Earth | Elements.Water:
							break;
						case Elements.Cold:
							this.ColdMagnitude = value;
							return;
						default:
							if (iElement != Elements.Fire)
							{
								return;
							}
							this.FireMagnitude = value;
							return;
						}
					}
					else
					{
						if (iElement == Elements.Lightning)
						{
							this.LightningMagnitude = value;
							return;
						}
						if (iElement != Elements.Arcane)
						{
							return;
						}
						this.ArcaneMagnitude = value;
						return;
					}
				}
				else if (iElement <= Elements.Shield)
				{
					if (iElement == Elements.Life)
					{
						this.LifeMagnitude = value;
						return;
					}
					if (iElement != Elements.Shield)
					{
						return;
					}
					this.ShieldMagnitude = value;
					return;
				}
				else
				{
					if (iElement == Elements.Ice)
					{
						this.IceMagnitude = value;
						return;
					}
					if (iElement == Elements.Steam)
					{
						this.SteamMagnitude = value;
						return;
					}
					if (iElement != Elements.Poison)
					{
						return;
					}
					this.PoisonMagnitude = value;
				}
			}
		}

		// Token: 0x06002FE0 RID: 12256 RVA: 0x001847DE File Offset: 0x001829DE
		public static int ElementIndex(Elements iElement)
		{
			if (iElement == Elements.All)
			{
				return 15;
			}
			return (int)(Math.Log((double)iElement) * Defines.ONEOVERLN2 + 0.5);
		}

		// Token: 0x06002FE1 RID: 12257 RVA: 0x00184803 File Offset: 0x00182A03
		public static Elements ElementFromIndex(int iIndex)
		{
			return (Elements)(1 << iIndex);
		}

		// Token: 0x06002FE2 RID: 12258 RVA: 0x0018480C File Offset: 0x00182A0C
		public SpellType GetSpellType()
		{
			if (this.Element == Elements.All)
			{
				return SpellType.Magick;
			}
			if (this.Element == Elements.None)
			{
				return SpellType.Push;
			}
			if ((this.Element & Elements.Shield) != Elements.None)
			{
				return SpellType.Shield;
			}
			if ((this.Element & Elements.PhysicalElements) != Elements.None)
			{
				return SpellType.Projectile;
			}
			if ((this.Element & Elements.Beams) != Elements.None)
			{
				return SpellType.Beam;
			}
			if ((this.Element & Elements.Lightning) != Elements.None && (this.Element & Elements.Steam) != Elements.Steam)
			{
				return SpellType.Lightning;
			}
			return SpellType.Spray;
		}

		// Token: 0x06002FE3 RID: 12259 RVA: 0x00184880 File Offset: 0x00182A80
		public bool ContainsElement(Elements iElement)
		{
			return this[iElement] > float.Epsilon;
		}

		// Token: 0x06002FE4 RID: 12260 RVA: 0x00184890 File Offset: 0x00182A90
		public void Cast(bool iFromStaff, ISpellCaster iOwner, CastType iCastType)
		{
			if (iCastType == CastType.None)
			{
				return;
			}
			SpellEffect spellEffect = null;
			SpellType spellType = this.GetSpellType();
			if (iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer))
			{
				if (this.Element == Elements.Ice)
				{
					AchievementsManager.Instance.AwardAchievement(iOwner.PlayState, "vanillaice");
				}
				if ((this.Element & Elements.Steam) != Elements.None)
				{
					AchievementsManager.Instance.AwardAchievement(iOwner.PlayState, "letoffsomesteam");
				}
				int num = 0;
				for (int i = 0; i < 11; i++)
				{
					if ((this.Element & (Elements)(1 << i)) != Elements.None)
					{
						num++;
					}
				}
				if (num >= 3)
				{
					AchievementsManager.Instance.AwardAchievement(iOwner.PlayState, "statealchemist");
				}
				if (num >= 5)
				{
					AchievementsManager.Instance.AwardAchievement(iOwner.PlayState, "imthewizardkingicand");
				}
			}
			switch (spellType)
			{
			case SpellType.Push:
				spellEffect = PushSpell.GetFromCache();
				break;
			case SpellType.Spray:
				spellEffect = SpraySpell.GetFromCache();
				break;
			case SpellType.Projectile:
				spellEffect = ProjectileSpell.GetFromCache();
				break;
			case SpellType.Shield:
				spellEffect = ShieldSpell.GetFromCache();
				break;
			case SpellType.Beam:
				if (iOwner is Avatar && iCastType == CastType.Force)
				{
					TutorialManager.Instance.SetTip(TutorialManager.Tips.Beams, TutorialManager.Position.Top);
				}
				spellEffect = RailGunSpell.GetFromCache();
				break;
			case SpellType.Lightning:
				spellEffect = LightningSpell.GetFromCache();
				break;
			}
			if (spellEffect == null)
			{
				return;
			}
			switch (iCastType)
			{
			case CastType.Force:
				iOwner.CurrentSpell = spellEffect;
				iOwner.CurrentSpell.CastForce(this, iOwner, iFromStaff);
				return;
			case CastType.Area:
				iOwner.CurrentSpell = spellEffect;
				iOwner.CurrentSpell.CastArea(this, iOwner, iFromStaff);
				return;
			case CastType.Self:
				iOwner.CurrentSpell = spellEffect;
				iOwner.CurrentSpell.CastSelf(this, iOwner, iFromStaff);
				return;
			case CastType.Weapon:
				iOwner.CurrentSpell = spellEffect;
				iOwner.CurrentSpell.CastWeapon(this, iOwner, iFromStaff);
				return;
			default:
				return;
			}
		}

		// Token: 0x06002FE5 RID: 12261 RVA: 0x00184A60 File Offset: 0x00182C60
		public float TotalMagnitude()
		{
			return this.EarthMagnitude + this.WaterMagnitude + this.ColdMagnitude + this.FireMagnitude + this.LightningMagnitude + this.ArcaneMagnitude + this.LifeMagnitude + this.ShieldMagnitude + this.IceMagnitude + this.SteamMagnitude + this.PoisonMagnitude;
		}

		// Token: 0x06002FE6 RID: 12262 RVA: 0x00184AB9 File Offset: 0x00182CB9
		public float TotalSprayMagnitude()
		{
			return this.FireMagnitude + this.ColdMagnitude + this.SteamMagnitude + this.WaterMagnitude + this.PoisonMagnitude;
		}

		// Token: 0x06002FE7 RID: 12263 RVA: 0x00184AE0 File Offset: 0x00182CE0
		public float BlastSize()
		{
			float num = 0f;
			switch (this.GetSpellType())
			{
			case SpellType.Spray:
				num = Math.Max(this.SteamMagnitude, Math.Max(this.FireMagnitude, Math.Max(this.ColdMagnitude, Math.Max(this.WaterMagnitude, this.PoisonMagnitude))));
				break;
			case SpellType.Projectile:
				num = Math.Max(this.EarthMagnitude, this.IceMagnitude);
				break;
			case SpellType.Beam:
				num = Math.Max(this.ArcaneMagnitude, this.LifeMagnitude);
				break;
			}
			return (float)Math.Sqrt((double)(num / 5f));
		}

		// Token: 0x06002FE8 RID: 12264 RVA: 0x00184B7D File Offset: 0x00182D7D
		public static float ArcaneMultiplier(float iMagnitude)
		{
			return (float)Math.Pow((double)(iMagnitude / 5f), 0.5) * 3f;
		}

		// Token: 0x06002FE9 RID: 12265 RVA: 0x001851F8 File Offset: 0x001833F8
		public IEnumerable<Cue> PlaySound(SpellType iSpellType, CastType iCastType)
		{
			SpellSoundVariables ssv = default(SpellSoundVariables);
			for (int i = 0; i < 11; i++)
			{
				if (this[Spell.ElementFromIndex(i)] > 0f)
				{
					ssv.mMagnitude = this[Spell.ElementFromIndex(i)];
					switch (iSpellType)
					{
					case SpellType.Spray:
						switch (iCastType)
						{
						case CastType.None:
							yield return AudioManager.Instance.GetCue<SpellSoundVariables>(Banks.Spells, Defines.SOUNDS_BLAST[i], ssv);
							break;
						case CastType.Force:
							yield return AudioManager.Instance.GetCue<SpellSoundVariables>(Banks.Spells, Defines.SOUNDS_SPRAY[i], ssv);
							break;
						case CastType.Area:
						case CastType.Self:
							yield return AudioManager.Instance.GetCue<SpellSoundVariables>(Banks.Spells, Defines.SOUNDS_AREA[i], ssv);
							break;
						}
						break;
					case SpellType.Projectile:
						switch (iCastType)
						{
						case CastType.None:
							yield return AudioManager.Instance.GetCue<SpellSoundVariables>(Banks.Spells, Defines.SOUNDS_BLAST[i], ssv);
							break;
						case CastType.Force:
							yield return AudioManager.Instance.GetCue<SpellSoundVariables>(Banks.Spells, Defines.SOUNDS_SPRAY[i], ssv);
							break;
						case CastType.Area:
						case CastType.Self:
							yield return AudioManager.Instance.GetCue<SpellSoundVariables>(Banks.Spells, Defines.SOUNDS_AREA[i], ssv);
							break;
						}
						break;
					case SpellType.Shield:
						switch (iCastType)
						{
						case CastType.None:
							yield return AudioManager.Instance.GetCue<SpellSoundVariables>(Banks.Spells, Defines.SOUNDS_BLAST[i], ssv);
							break;
						case CastType.Force:
							yield return AudioManager.Instance.GetCue<SpellSoundVariables>(Banks.Spells, Defines.SOUNDS_SPRAY[i], ssv);
							break;
						case CastType.Area:
						case CastType.Self:
							yield return AudioManager.Instance.GetCue<SpellSoundVariables>(Banks.Spells, Defines.SOUNDS_AREA[i], ssv);
							break;
						}
						break;
					case SpellType.Beam:
						switch (iCastType)
						{
						case CastType.None:
							yield return AudioManager.Instance.GetCue<SpellSoundVariables>(Banks.Spells, Defines.SOUNDS_BLAST[i], ssv);
							break;
						case CastType.Force:
							yield return AudioManager.Instance.GetCue<SpellSoundVariables>(Banks.Spells, Defines.SOUNDS_SPRAY[i], ssv);
							break;
						case CastType.Area:
						case CastType.Weapon:
							yield return AudioManager.Instance.GetCue<SpellSoundVariables>(Banks.Spells, Defines.SOUNDS_AREA[i], ssv);
							break;
						case CastType.Self:
							yield return AudioManager.Instance.GetCue<SpellSoundVariables>(Banks.Spells, Defines.SOUNDS_SELF[i], ssv);
							break;
						}
						break;
					case SpellType.Lightning:
						switch (iCastType)
						{
						case CastType.None:
						case CastType.Force:
						case CastType.Area:
						case CastType.Self:
							yield return AudioManager.Instance.GetCue<SpellSoundVariables>(Banks.Spells, Defines.SOUNDS_SPRAY[i], ssv);
							break;
						}
						break;
					case SpellType.Magick:
						switch (iCastType)
						{
						case CastType.None:
							yield return AudioManager.Instance.GetCue<SpellSoundVariables>(Banks.Spells, Defines.SOUNDS_BLAST[i], ssv);
							break;
						case CastType.Force:
							yield return AudioManager.Instance.GetCue<SpellSoundVariables>(Banks.Spells, Defines.SOUNDS_SPRAY[i], ssv);
							break;
						case CastType.Area:
						case CastType.Self:
							yield return AudioManager.Instance.GetCue<SpellSoundVariables>(Banks.Spells, Defines.SOUNDS_AREA[i], ssv);
							break;
						}
						break;
					}
				}
			}
			yield break;
		}

		// Token: 0x06002FEA RID: 12266 RVA: 0x00185228 File Offset: 0x00183428
		public void CalculateDamage(SpellType iSpellType, CastType iCastType, out DamageCollection5 oDamages)
		{
			oDamages = default(DamageCollection5);
			Damage iDamage = default(Damage);
			switch (iSpellType)
			{
			case SpellType.Spray:
				switch (iCastType)
				{
				case CastType.Force:
				case CastType.Area:
					if ((this.Element & Elements.StatusEffects) != Elements.None)
					{
						if ((this.Element & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Steam | Elements.Poison)) != Elements.None)
						{
							iDamage = new Damage
							{
								AttackProperty = AttackProperties.Status,
								Element = (this.Element & Elements.StatusEffects),
								Amount = Defines.STATUS_POISON_DAMAGE * Math.Min(1f, this[Elements.Poison]) + Defines.STATUS_BURN_DAMAGE * Math.Min(1f, this[Elements.Fire]),
								Magnitude = Math.Min(1f, this[Elements.Water] + this[Elements.Fire] + this[Elements.Cold] + this[Elements.Poison] + this[Elements.Steam])
							};
							oDamages.AddDamage(iDamage);
						}
						if ((this.Element & Elements.Life) != Elements.None)
						{
							iDamage = default(Damage);
							iDamage.AttackProperty = AttackProperties.Status;
							iDamage.Element = (this.Element & Elements.Life);
							iDamage.Amount += this.ScaleAmount(Defines.STATUS_LIFE_DAMAGE, this[Elements.Life]);
							iDamage.Magnitude = 1f;
							if (iCastType == CastType.Self)
							{
								iDamage.Magnitude = this[Elements.Life];
							}
							oDamages.AddDamage(iDamage);
						}
					}
					if ((this.Element & (Elements.Cold | Elements.Fire | Elements.Lightning | Elements.Steam)) != Elements.None)
					{
						iDamage = default(Damage);
						iDamage.AttackProperty = AttackProperties.Damage;
						iDamage.Element = (this.Element & (Elements.Cold | Elements.Fire | Elements.Lightning | Elements.Steam));
						iDamage.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_FIRE, this[Elements.Fire]);
						iDamage.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_COLD, this[Elements.Cold]);
						iDamage.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_STEAM, this[Elements.Steam]);
						iDamage.Magnitude = Math.Min(this[Elements.Fire] + this[Elements.Cold] + this[Elements.Steam] + this[Elements.Lightning], 1f);
						oDamages.AddDamage(iDamage);
					}
					if (this[Elements.Water] > 0f)
					{
						iDamage = new Damage
						{
							AttackProperty = AttackProperties.Pushed,
							Element = Elements.Water,
							Magnitude = 1f + this[Elements.Water] / 4f,
							Amount = (float)((int)(Defines.SPELL_STRENGTH_WATER * this[Elements.Water]))
						};
						oDamages.AddDamage(iDamage);
						iDamage = new Damage
						{
							AttackProperty = AttackProperties.Damage,
							Element = Elements.Water,
							Magnitude = 1f,
							Amount = 0f
						};
						oDamages.AddDamage(iDamage);
						return;
					}
					break;
				case CastType.Self:
					if ((this.Element & Elements.StatusEffects) != Elements.None)
					{
						if ((this.Element & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Steam | Elements.Poison)) != Elements.None)
						{
							iDamage = new Damage
							{
								AttackProperty = AttackProperties.Status,
								Element = (this.Element & Elements.StatusEffects),
								Amount = Defines.STATUS_POISON_DAMAGE * Math.Min(1f, this[Elements.Poison]) + Defines.STATUS_BURN_DAMAGE * Math.Min(1f, this[Elements.Fire]),
								Magnitude = Math.Min(1f, this[Elements.Water] + this[Elements.Fire] + this[Elements.Cold] + this[Elements.Poison] + this[Elements.Steam])
							};
							oDamages.AddDamage(iDamage);
						}
						if ((this.Element & Elements.Life) != Elements.None)
						{
							iDamage = default(Damage);
							iDamage.AttackProperty = AttackProperties.Status;
							iDamage.Element = (this.Element & Elements.Life);
							iDamage.Amount += this.ScaleAmount(Defines.STATUS_LIFE_DAMAGE, this[Elements.Life]);
							iDamage.Magnitude = 1f;
							if (iCastType == CastType.Self)
							{
								iDamage.Magnitude = this[Elements.Life];
							}
							oDamages.AddDamage(iDamage);
						}
					}
					if ((this.Element & (Elements.Cold | Elements.Fire | Elements.Lightning | Elements.Steam)) != Elements.None)
					{
						iDamage = default(Damage);
						iDamage.AttackProperty = AttackProperties.Damage;
						iDamage.Element = (this.Element & (Elements.Cold | Elements.Fire | Elements.Lightning | Elements.Steam));
						iDamage.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_SELF_FIRE, this[Elements.Fire]);
						iDamage.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_COLD, this[Elements.Cold]);
						iDamage.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_SELF_STEAM, this[Elements.Steam]);
						iDamage.Magnitude = Math.Min(this[Elements.Fire] + this[Elements.Cold] + this[Elements.Steam] + this[Elements.Lightning], 1f);
						oDamages.AddDamage(iDamage);
						return;
					}
					break;
				case CastType.Weapon:
					if ((this.Element & Elements.StatusEffects) != Elements.None)
					{
						if ((this.Element & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Steam | Elements.Poison)) != Elements.None)
						{
							iDamage = new Damage
							{
								AttackProperty = AttackProperties.Status,
								Element = (this.Element & Elements.StatusEffects),
								Amount = Defines.STATUS_POISON_DAMAGE * Math.Min(1f, this[Elements.Poison]) + Defines.STATUS_BURN_DAMAGE * Math.Min(1f, this[Elements.Fire]),
								Magnitude = Math.Min(1f, this[Elements.Water] + this[Elements.Fire] + this[Elements.Cold] + this[Elements.Poison] + this[Elements.Steam])
							};
							oDamages.AddDamage(iDamage);
						}
						if ((this.Element & Elements.Life) != Elements.None)
						{
							iDamage = default(Damage);
							iDamage.AttackProperty = AttackProperties.Status;
							iDamage.Element = (this.Element & Elements.Life);
							iDamage.Amount += this.ScaleAmount(Defines.STATUS_LIFE_DAMAGE, this[Elements.Life]);
							iDamage.Magnitude = 1f;
							if (iCastType == CastType.Self)
							{
								iDamage.Magnitude = this[Elements.Life];
							}
							oDamages.AddDamage(iDamage);
						}
					}
					if ((this.Element & (Elements.Cold | Elements.Fire | Elements.Lightning | Elements.Steam)) != Elements.None)
					{
						iDamage = default(Damage);
						iDamage.AttackProperty = AttackProperties.Damage;
						iDamage.Element = (this.Element & (Elements.Cold | Elements.Fire | Elements.Lightning | Elements.Steam));
						iDamage.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_FIRE, this[Elements.Fire]);
						iDamage.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_COLD, this[Elements.Cold]);
						iDamage.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_STEAM, this[Elements.Steam]);
						iDamage.Magnitude = Math.Min(this[Elements.Fire] + this[Elements.Cold] + this[Elements.Steam] + this[Elements.Lightning], 1f);
						oDamages.AddDamage(iDamage);
					}
					if (this[Elements.Water] > 0f)
					{
						iDamage = new Damage
						{
							AttackProperty = AttackProperties.Pushed,
							Element = Elements.Water,
							Magnitude = this[Elements.Water],
							Amount = (float)((int)(Defines.SPELL_STRENGTH_WATER * this[Elements.Water]))
						};
						oDamages.AddDamage(iDamage);
						iDamage = new Damage
						{
							AttackProperty = AttackProperties.Damage,
							Element = Elements.Water,
							Magnitude = 1f,
							Amount = 0f
						};
						oDamages.AddDamage(iDamage);
						return;
					}
					break;
				case CastType.Magick:
					break;
				default:
					return;
				}
				break;
			case SpellType.Projectile:
				switch (iCastType)
				{
				case CastType.Force:
					if (this[Elements.Earth] > 0f)
					{
						iDamage = default(Damage);
						iDamage.AttackProperty = AttackProperties.Damage;
						iDamage.Element = (this.Element & Elements.PhysicalElements);
						if (this[Elements.Ice] > 0f)
						{
							iDamage.Element |= Elements.Cold;
						}
						iDamage.Amount += Defines.SPELL_DAMAGE_EARTH * this[Elements.Earth] * this[Elements.Earth];
						iDamage.Amount += Defines.SPELL_DAMAGE_ICEEARTH * this[Elements.Ice] * this[Elements.Ice];
						iDamage.Magnitude = 1f;
						oDamages.AddDamage(iDamage);
					}
					else if (this[Elements.Ice] > 0f)
					{
						iDamage = default(Damage);
						iDamage.AttackProperty = AttackProperties.Damage;
						iDamage.Element = (this.Element | Elements.Cold | Elements.Earth);
						iDamage.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_ICEEARTH, this[Elements.Ice]);
						iDamage.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_LIGHTNING, this[Elements.Lightning]);
						iDamage.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_ARCANE, this[Elements.Arcane]);
						iDamage.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_LIFE, this[Elements.Life]);
						iDamage.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_LIFE, this[Elements.Life]);
						iDamage.Magnitude = 1f;
						oDamages.AddDamage(iDamage);
					}
					if (this[Elements.Ice] > 0f && (this.Element & Elements.StatusEffects) != Elements.None)
					{
						iDamage = default(Damage);
						iDamage.AttackProperty = AttackProperties.Status;
						iDamage.Element = (this.Element & Elements.StatusEffects);
						iDamage.Amount = Defines.STATUS_POISON_DAMAGE * Math.Min(1f, this[Elements.Poison]);
						iDamage.Amount += Defines.STATUS_LIFE_DAMAGE * Math.Min(1f, this[Elements.Life]);
						iDamage.Magnitude = Math.Min(1f, this[Elements.Water] + this[Elements.Cold] + this[Elements.Poison] + this[Elements.Life]);
						oDamages.AddDamage(iDamage);
						return;
					}
					break;
				case CastType.Area:
					if ((this.Element & Elements.StatusEffects) != Elements.None)
					{
						if ((this.Element & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Steam)) != Elements.None)
						{
							iDamage = new Damage
							{
								AttackProperty = AttackProperties.Status,
								Element = (this.Element & Elements.StatusEffects),
								Amount = Defines.STATUS_BURN_DAMAGE * Math.Min(1f, this[Elements.Fire]),
								Magnitude = Math.Min(1f, this[Elements.Water] + this[Elements.Fire] + this[Elements.Cold] + this[Elements.Steam])
							};
							oDamages.AddDamage(iDamage);
						}
						if ((this.Element & Elements.Life) != Elements.None)
						{
							iDamage = default(Damage);
							iDamage.AttackProperty = AttackProperties.Status;
							iDamage.Element = (this.Element & Elements.Life);
							iDamage.Amount += this.ScaleAmount(Defines.STATUS_LIFE_DAMAGE, this[Elements.Life]);
							iDamage.Magnitude = 1f;
							if (iCastType == CastType.Self)
							{
								iDamage.Magnitude = this[Elements.Life];
							}
							oDamages.AddDamage(iDamage);
						}
					}
					iDamage = default(Damage);
					iDamage.AttackProperty = AttackProperties.Damage;
					iDamage.Element = ((this.Element & Elements.Instant) | Elements.Earth);
					iDamage.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_FIRE, this[Elements.Fire]);
					iDamage.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_COLD, this[Elements.Cold]);
					iDamage.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_STEAM, this[Elements.Steam]);
					iDamage.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_LIGHTNING, this[Elements.Lightning]);
					iDamage.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_BARRIER_ICE, this[Elements.Ice]);
					iDamage.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_LIFE, this[Elements.Life]);
					iDamage.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_ARCANE, this[Elements.Arcane]);
					iDamage.Magnitude = Math.Min(this[Elements.Fire] + this[Elements.Cold] + this[Elements.Steam] + this[Elements.Lightning] + this[Elements.Ice] + this[Elements.Arcane], 1f);
					oDamages.AddDamage(iDamage);
					if (this[Elements.Water] > 0f)
					{
						iDamage = new Damage
						{
							AttackProperty = AttackProperties.Pushed,
							Element = Elements.Water,
							Magnitude = this[Elements.Water],
							Amount = (float)((int)(Defines.SPELL_STRENGTH_WATER * this[Elements.Water]))
						};
						oDamages.AddDamage(iDamage);
					}
					if (this[Elements.Earth] > 0f)
					{
						iDamage = new Damage
						{
							AttackProperty = AttackProperties.Knockdown,
							Element = Elements.Earth,
							Magnitude = this[Elements.Earth],
							Amount = (float)((int)(Defines.SPELL_DAMAGE_EARTH * this[Elements.Earth]))
						};
						oDamages.AddDamage(iDamage);
						return;
					}
					break;
				case CastType.Self:
					break;
				case CastType.Weapon:
					if ((this.Element & Elements.Ice) == Elements.Ice)
					{
						oDamages.AddDamage(this.CalculateInstantDamage());
					}
					oDamages.AddDamage(this.CalculateMiscEffects(iCastType));
					oDamages.AddDamage(this.CalculateStatus());
					oDamages.AddDamage(this.CalculateInstantBlast());
					return;
				default:
					return;
				}
				break;
			case SpellType.Shield:
			{
				Damage iDamage2 = default(Damage);
				if ((this.Element & ~Elements.Shield & Elements.Life) == Elements.Life)
				{
					iDamage2.AttackProperty = AttackProperties.Damage;
					iDamage2.Element = (this.Element & Elements.Beams);
					iDamage2.Amount = Defines.SPELL_DAMAGE_BARRIER_LIFE;
					iDamage2.Magnitude = 1f;
					oDamages.AddDamage(iDamage2);
				}
				else if ((this.Element & ~Elements.Shield & Elements.Arcane) == Elements.Arcane)
				{
					iDamage2.AttackProperty = AttackProperties.Damage;
					iDamage2.Element = Elements.Arcane;
					iDamage2.Amount = Defines.SPELL_DAMAGE_BARRIER_ARCANE;
					iDamage2.Magnitude = 1f;
					oDamages.AddDamage(iDamage2);
				}
				if ((this.Element & Elements.StatusEffects) != Elements.None)
				{
					iDamage = default(Damage);
					iDamage.AttackProperty = AttackProperties.Status;
					iDamage.Element = (this.Element & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Arcane | Elements.Life | Elements.Steam | Elements.Poison));
					if (this[Elements.Steam] > 0f)
					{
						iDamage.Element |= Elements.Water;
					}
					iDamage.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_FIRE, this[Elements.Fire]);
					iDamage.Magnitude = Math.Min(1f, this[Elements.Water] + this[Elements.Fire] + this[Elements.Cold] + this[Elements.Steam] * 0.5f);
					oDamages.AddDamage(iDamage);
				}
				if (this.WaterMagnitude > 0f && (this.EarthMagnitude > 0f || this.IceMagnitude > 0f))
				{
					iDamage = default(Damage);
					iDamage.AttackProperty = AttackProperties.Pushed;
					iDamage.Element = Elements.Water;
					float num = (float)Math.Pow((double)this.WaterMagnitude, 0.5);
					iDamage.Amount = (float)((int)(75f * num));
					iDamage.Magnitude = num;
					oDamages.AddDamage(iDamage);
				}
				if (this.LightningMagnitude > 0f)
				{
					iDamage = new Damage
					{
						AttackProperty = AttackProperties.Damage,
						Element = Elements.Lightning,
						Magnitude = 1f,
						Amount = this.ScaleAmount(Defines.SPELL_DAMAGE_BARRIER_LIGHTNING, this.LightningMagnitude)
					};
					oDamages.AddDamage(iDamage);
				}
				if (this.EarthMagnitude + this.IceMagnitude <= 0f && this.ArcaneMagnitude > 0f)
				{
					iDamage = new Damage
					{
						AttackProperty = AttackProperties.Knockback,
						Element = Elements.None,
						Magnitude = 2f,
						Amount = 500f
					};
					oDamages.AddDamage(iDamage);
					return;
				}
				break;
			}
			case SpellType.Beam:
				if ((this.Element & Elements.StatusEffects) != Elements.None)
				{
					if ((this.Element & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Steam | Elements.Poison)) != Elements.None)
					{
						iDamage = default(Damage);
						iDamage.AttackProperty = AttackProperties.Status;
						if (this[Elements.Water] > 0f)
						{
							iDamage.AttackProperty |= AttackProperties.Damage;
						}
						iDamage.Element = (this.Element & Elements.StatusEffects);
						if (this[Elements.Steam] > 0f)
						{
							iDamage.Element |= Elements.Water;
						}
						iDamage.Amount = Defines.STATUS_POISON_DAMAGE * Math.Min(1f, this[Elements.Poison]) + Defines.STATUS_BURN_DAMAGE * Math.Min(1f, this[Elements.Fire]);
						iDamage.Magnitude = Math.Min(1f, this[Elements.Water] + this[Elements.Fire] + this[Elements.Cold] + this[Elements.Poison] + this[Elements.Steam]);
						oDamages.AddDamage(iDamage);
					}
					if ((this.Element & Elements.Life) != Elements.None)
					{
						iDamage = default(Damage);
						iDamage.AttackProperty = AttackProperties.Status;
						iDamage.Element = (this.Element & Elements.Life);
						iDamage.Amount += this.ScaleAmount(Defines.STATUS_LIFE_DAMAGE, this[Elements.Life]);
						iDamage.Magnitude = 1f;
						if (iCastType == CastType.Self)
						{
							iDamage.Magnitude = this[Elements.Life];
						}
						oDamages.AddDamage(iDamage);
					}
				}
				iDamage = default(Damage);
				iDamage.AttackProperty = AttackProperties.Damage;
				iDamage.Element = (this.Element & (Elements.Cold | Elements.Fire | Elements.Lightning | Elements.Arcane | Elements.Life | Elements.Steam));
				if (iCastType != CastType.Self)
				{
					iDamage.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_FIRE, this[Elements.Fire]);
					iDamage.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_COLD, this[Elements.Cold]);
					iDamage.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_LIGHTNING * 0.5f, this[Elements.Lightning]);
					iDamage.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_STEAM, this[Elements.Steam]);
					iDamage.Amount += Defines.SPELL_DAMAGE_LIFE * Math.Min(1f, this[Elements.Life]);
					iDamage.Amount += Defines.SPELL_DAMAGE_ARCANE * Math.Min(1f, this[Elements.Arcane]);
				}
				else
				{
					iDamage.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_SELF_FIRE, this[Elements.Fire]);
					iDamage.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_COLD, this[Elements.Cold]);
					iDamage.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_SELF_LIGHTNING, this[Elements.Lightning]);
					iDamage.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_SELF_STEAM, this[Elements.Steam]);
					iDamage.Amount += Defines.SPELL_DAMAGE_LIFE * Math.Min(1f, this[Elements.Life]);
					iDamage.Amount += Defines.SPELL_DAMAGE_SELF_ARCANE * Math.Min(1f, this[Elements.Arcane]);
				}
				iDamage.Magnitude = 1f;
				oDamages.AddDamage(iDamage);
				if (this[Elements.Water] > 0f)
				{
					iDamage = new Damage
					{
						AttackProperty = AttackProperties.Pushed,
						Element = Elements.Water,
						Magnitude = this[Elements.Water],
						Amount = (float)((int)(Defines.SPELL_STRENGTH_WATER * this[Elements.Water]))
					};
					oDamages.AddDamage(iDamage);
					return;
				}
				break;
			case SpellType.Lightning:
			{
				Damage iDamage3 = default(Damage);
				iDamage3.AttackProperty |= AttackProperties.Damage;
				iDamage3.Element |= this.Element;
				iDamage3.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_LIGHTNING, this[Elements.Lightning]);
				iDamage3.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_ARCANE, this[Elements.Arcane]);
				iDamage3.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_LIFE, this[Elements.Life]);
				iDamage3.Magnitude = 1f;
				oDamages.AddDamage(iDamage3);
				if ((this.Element & Elements.StatusEffects) != Elements.None && (this.Element & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Steam | Elements.Poison)) != Elements.None)
				{
					iDamage = default(Damage);
					iDamage.AttackProperty = AttackProperties.Status;
					if (this[Elements.Water] > 0f)
					{
						iDamage.AttackProperty |= AttackProperties.Damage;
					}
					iDamage.Element = (this.Element & Elements.StatusEffects);
					iDamage.Amount = Defines.STATUS_POISON_DAMAGE * Math.Min(1f, this[Elements.Poison]) + Defines.STATUS_BURN_DAMAGE * Math.Min(1f, this[Elements.Fire]);
					iDamage.Magnitude = Math.Min(1f, this[Elements.Water] + this[Elements.Fire] + this[Elements.Cold] + this[Elements.Poison] + this[Elements.Steam]);
					oDamages.AddDamage(iDamage);
				}
				oDamages.AddDamage(this.CalculateInstantBlast());
				oDamages.AddDamage(this.CalculateMiscEffects(CastType.Area));
				if ((this.Element & Elements.StatusEffects) != Elements.None || this[Elements.Steam] > 0f)
				{
					oDamages.AddDamage(this.CalculateStatus());
					return;
				}
				break;
			}
			default:
				throw new Exception(string.Concat(new string[]
				{
					"No damage available for ",
					iCastType.ToString(),
					",",
					iSpellType.ToString(),
					", or is there?"
				}));
			}
		}

		// Token: 0x06002FEB RID: 12267 RVA: 0x00186812 File Offset: 0x00184A12
		private float ScaleAmount(float iBaseDamage, float iMagnitude)
		{
			return iBaseDamage * (float)Math.Sqrt((double)iMagnitude);
		}

		// Token: 0x06002FEC RID: 12268 RVA: 0x00186820 File Offset: 0x00184A20
		private Damage CalculateLightningDamage()
		{
			Damage result = default(Damage);
			result.AttackProperty |= AttackProperties.Damage;
			result.Element |= (this.Element & Elements.InstantNonPhysical);
			result.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_LIGHTNING, this[Elements.Lightning]);
			result.Magnitude = 1f;
			return result;
		}

		// Token: 0x06002FED RID: 12269 RVA: 0x00186890 File Offset: 0x00184A90
		private Damage CalculateInstantDamage()
		{
			Damage result = default(Damage);
			result.AttackProperty = AttackProperties.Damage;
			if (this[Elements.Ice] > 0f)
			{
				result.AttackProperty |= AttackProperties.Piercing;
			}
			result.Element = Elements.Earth;
			result.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_ICEEARTH, this[Elements.Ice]) * 2f;
			result.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_EARTH, this[Elements.Earth]);
			result.Magnitude = 1f;
			return result;
		}

		// Token: 0x06002FEE RID: 12270 RVA: 0x00186930 File Offset: 0x00184B30
		private Damage CalculateMiscEffects(CastType iCastType)
		{
			Damage result = default(Damage);
			if ((this.Element & (Elements.Water | Elements.Arcane)) != Elements.None)
			{
				result.AttackProperty = AttackProperties.Pushed;
			}
			result.Element |= (this.Element & Elements.Water);
			result.Magnitude += this[Elements.Water];
			result.Amount = (float)((int)(70f * this[Elements.Water]));
			if (iCastType == CastType.Weapon || iCastType == CastType.Area)
			{
				result.Element |= (this.Element & (Elements.Earth | Elements.Arcane));
				result.Amount += (float)((int)(120f * this[Elements.Arcane]));
				result.Magnitude += this[Elements.Earth] + this[Elements.Arcane];
				if (this[Elements.Earth] > 0f)
				{
					result.AttackProperty |= AttackProperties.Knockdown;
				}
			}
			return result;
		}

		// Token: 0x06002FEF RID: 12271 RVA: 0x00186A14 File Offset: 0x00184C14
		private Damage CalculateInstantBlast()
		{
			return default(Damage);
		}

		// Token: 0x06002FF0 RID: 12272 RVA: 0x00186A2C File Offset: 0x00184C2C
		private Damage CalculateStatus()
		{
			Damage result = default(Damage);
			result.AttackProperty = AttackProperties.Status;
			result.Element = (this.Element & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Arcane | Elements.Life | Elements.Steam | Elements.Poison));
			if (this[Elements.Steam] > 0f)
			{
				result.Element |= Elements.Water;
			}
			result.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_FIRE, this[Elements.Fire]);
			result.Magnitude = Math.Min(1f, this[Elements.Water] + this[Elements.Fire] + this[Elements.Cold] + this[Elements.Steam] * 0.5f);
			return result;
		}

		// Token: 0x06002FF1 RID: 12273 RVA: 0x00186ADC File Offset: 0x00184CDC
		public Vector3 GetColor()
		{
			Vector3 vector = default(Vector3);
			float num = 0f;
			if ((this.Element & Elements.Lightning) != Elements.None)
			{
				num += this[Elements.Lightning];
				vector += Spell.LIGHTNINGCOLOR * this[Elements.Lightning];
			}
			if ((this.Element & Elements.Fire) != Elements.None)
			{
				num += this[Elements.Fire];
				vector += Spell.FIRECOLOR * this[Elements.Fire];
			}
			if ((this.Element & Elements.Cold) != Elements.None)
			{
				num += 1f;
				vector += Spell.COLDCOLOR;
			}
			if ((this.Element & Elements.Life) != Elements.None)
			{
				num += 1f;
				vector += Spell.LIFECOLOR;
			}
			if ((this.Element & Elements.Arcane) != Elements.None)
			{
				num += 1f;
				vector += Spell.ARCANECOLOR;
			}
			if ((this.Element & Elements.Water) != Elements.None)
			{
				num += 1f;
				vector += Spell.WATERCOLOR;
			}
			if ((this.Element & Elements.Ice) != Elements.None)
			{
				num += 1f;
				vector += Spell.ICECOLOR;
			}
			if ((this.Element & Elements.Earth) != Elements.None)
			{
				num += 1f;
				vector += Spell.EARTHCOLOR;
			}
			if ((this.Element & Elements.Shield) != Elements.None)
			{
				num += 1f;
				vector += Spell.SHIELDCOLOR;
			}
			return vector / num;
		}

		// Token: 0x06002FF2 RID: 12274 RVA: 0x00186C38 File Offset: 0x00184E38
		public static Vector3 GetColor(Elements iElements)
		{
			Vector3 vector = default(Vector3);
			float num = 0f;
			if ((iElements & Elements.Lightning) != Elements.None)
			{
				num += 1f;
				vector += Spell.LIGHTNINGCOLOR;
			}
			if ((iElements & Elements.Fire) != Elements.None)
			{
				num += 1f;
				vector += Spell.FIRECOLOR;
			}
			if ((iElements & Elements.Cold) != Elements.None)
			{
				num += 1f;
				vector += Spell.COLDCOLOR;
			}
			if ((iElements & Elements.Life) != Elements.None)
			{
				num += 1f;
				vector += Spell.LIFECOLOR;
			}
			if ((iElements & Elements.Arcane) != Elements.None)
			{
				num += 1f;
				vector += Spell.ARCANECOLOR;
			}
			if ((iElements & Elements.Water) != Elements.None)
			{
				num += 1f;
				vector += Spell.WATERCOLOR;
			}
			if ((iElements & Elements.Ice) != Elements.None)
			{
				num += 1f;
				vector += Spell.ICECOLOR;
			}
			if ((iElements & Elements.Earth) != Elements.None)
			{
				num += 1f;
				vector += Spell.EARTHCOLOR;
			}
			if ((iElements & Elements.Shield) != Elements.None)
			{
				num += 1f;
				vector += Spell.SHIELDCOLOR;
			}
			return vector / num;
		}

		// Token: 0x06002FF3 RID: 12275 RVA: 0x00186D48 File Offset: 0x00184F48
		public bool Equals(Spell other)
		{
			return this == other;
		}

		// Token: 0x06002FF4 RID: 12276 RVA: 0x00186D58 File Offset: 0x00184F58
		public static Spell operator +(Spell A, Spell B)
		{
			Spell result;
			result.ColdMagnitude = 0f;
			result.EarthMagnitude = 0f;
			result.FireMagnitude = 0f;
			result.LifeMagnitude = 0f;
			result.IceMagnitude = 0f;
			result.LightningMagnitude = 0f;
			result.ArcaneMagnitude = 0f;
			result.ShieldMagnitude = 0f;
			result.SteamMagnitude = 0f;
			result.WaterMagnitude = 0f;
			result.PoisonMagnitude = 0f;
			result.EarthMagnitude = Math.Max(A.EarthMagnitude + B.EarthMagnitude - (A.LightningMagnitude + B.LightningMagnitude), 0f);
			result.ColdMagnitude = Math.Max(A.ColdMagnitude + B.ColdMagnitude - (A.FireMagnitude + B.FireMagnitude), 0f);
			result.FireMagnitude = Math.Max(A.FireMagnitude + B.FireMagnitude - (A.ColdMagnitude + B.ColdMagnitude), 0f);
			result.LightningMagnitude = Math.Max(A.LightningMagnitude + B.LightningMagnitude - (A.EarthMagnitude + B.EarthMagnitude), 0f);
			result.PoisonMagnitude = Math.Max(A.PoisonMagnitude + B.PoisonMagnitude - (A.LifeMagnitude + B.LifeMagnitude), 0f);
			float num = 0f;
			if ((result.IceMagnitude += A.IceMagnitude + B.IceMagnitude) > 0f && A.FireMagnitude + B.FireMagnitude > 0f)
			{
				result.WaterMagnitude += (num += Math.Min(result.IceMagnitude - num, A.FireMagnitude + B.FireMagnitude));
				result.FireMagnitude -= num;
				result.IceMagnitude -= num;
				num = 0f;
			}
			if ((result.SteamMagnitude += A.SteamMagnitude + B.SteamMagnitude) > 0f && A.ColdMagnitude + B.ColdMagnitude > 0f)
			{
				result.WaterMagnitude += (num += Math.Min(result.SteamMagnitude - num, A.ColdMagnitude + B.ColdMagnitude));
				result.ColdMagnitude -= num;
				result.SteamMagnitude -= num;
			}
			result.LifeMagnitude += A.LifeMagnitude + B.LifeMagnitude;
			result.ArcaneMagnitude += A.ArcaneMagnitude + B.ArcaneMagnitude;
			result.ShieldMagnitude += A.ShieldMagnitude + B.ShieldMagnitude;
			result.WaterMagnitude += A.WaterMagnitude + B.WaterMagnitude;
			float num2;
			result.SteamMagnitude += (num2 = Math.Min(A.WaterMagnitude + B.WaterMagnitude, A.FireMagnitude + B.FireMagnitude));
			result.WaterMagnitude -= num2;
			result.FireMagnitude -= num2;
			result.IceMagnitude += (num2 = Math.Min(A.WaterMagnitude + B.WaterMagnitude, A.ColdMagnitude + B.ColdMagnitude));
			result.WaterMagnitude -= num2;
			result.ColdMagnitude -= num2;
			result.Element = Elements.None;
			if (result.ArcaneMagnitude > 0f)
			{
				result.Element |= Elements.Arcane;
			}
			if (result.SteamMagnitude > 0f)
			{
				result.Element |= Elements.Steam;
			}
			if (result.IceMagnitude > 0f)
			{
				result.Element |= Elements.Ice;
			}
			if (result.WaterMagnitude > 0f)
			{
				result.Element |= Elements.Water;
			}
			if (result.EarthMagnitude > 0f)
			{
				result.Element |= Elements.Earth;
			}
			if (result.LightningMagnitude > 0f)
			{
				result.Element |= Elements.Lightning;
			}
			if (result.ColdMagnitude > 0f)
			{
				result.Element |= Elements.Cold;
			}
			if (result.FireMagnitude > 0f)
			{
				result.Element |= Elements.Fire;
			}
			if (result.LifeMagnitude > 0f)
			{
				result.Element |= Elements.Life;
			}
			if (result.ShieldMagnitude > 0f)
			{
				result.Element |= Elements.Shield;
			}
			if (result.PoisonMagnitude > 0f)
			{
				result.Element |= Elements.Poison;
			}
			return result;
		}

		// Token: 0x06002FF5 RID: 12277 RVA: 0x00187280 File Offset: 0x00185480
		public static bool operator ==(Spell A, Spell B)
		{
			return (A.EarthMagnitude == B.EarthMagnitude && A.WaterMagnitude == B.WaterMagnitude && A.ColdMagnitude == B.ColdMagnitude && A.FireMagnitude == B.FireMagnitude && A.LightningMagnitude == B.LightningMagnitude && A.LifeMagnitude == B.LifeMagnitude && A.ShieldMagnitude == B.ShieldMagnitude && A.SteamMagnitude == B.SteamMagnitude && A.ArcaneMagnitude == B.ArcaneMagnitude && A.IceMagnitude == B.IceMagnitude && A.PoisonMagnitude == B.PoisonMagnitude) || A.Element == B.Element;
		}

		// Token: 0x06002FF6 RID: 12278 RVA: 0x00187358 File Offset: 0x00185558
		public static bool operator !=(Spell A, Spell B)
		{
			return A.EarthMagnitude != B.EarthMagnitude || A.WaterMagnitude != B.WaterMagnitude || A.ColdMagnitude != B.ColdMagnitude || A.FireMagnitude != B.FireMagnitude || A.LightningMagnitude != B.LightningMagnitude || A.LifeMagnitude != B.LifeMagnitude || A.ShieldMagnitude != B.ShieldMagnitude || A.SteamMagnitude != B.SteamMagnitude || A.ArcaneMagnitude != B.ArcaneMagnitude || A.IceMagnitude != B.IceMagnitude || A.PoisonMagnitude != B.PoisonMagnitude || A.Element != B.Element;
		}

		// Token: 0x06002FF7 RID: 12279 RVA: 0x00187438 File Offset: 0x00185638
		public static void DefaultSpell(Elements iElements, out Spell oSpell)
		{
			oSpell = default(Spell);
			oSpell.Element = iElements;
			for (int i = 0; i < 11; i++)
			{
				Elements elements = Defines.ElementFromIndex(i);
				Elements elements2 = iElements & elements;
				if (elements2 <= Elements.Arcane)
				{
					if (elements2 <= Elements.Fire)
					{
						switch (elements2)
						{
						case Elements.Earth:
							oSpell.EarthMagnitude = 1f;
							break;
						case Elements.Water:
							oSpell.WaterMagnitude = 1f;
							break;
						case Elements.Earth | Elements.Water:
							break;
						case Elements.Cold:
							oSpell.ColdMagnitude = 1f;
							break;
						default:
							if (elements2 == Elements.Fire)
							{
								oSpell.FireMagnitude = 1f;
							}
							break;
						}
					}
					else if (elements2 != Elements.Lightning)
					{
						if (elements2 == Elements.Arcane)
						{
							oSpell.ArcaneMagnitude = 1f;
						}
					}
					else
					{
						oSpell.LightningMagnitude = 1f;
					}
				}
				else if (elements2 <= Elements.Shield)
				{
					if (elements2 != Elements.Life)
					{
						if (elements2 == Elements.Shield)
						{
							oSpell.ShieldMagnitude = 1f;
						}
					}
					else
					{
						oSpell.LifeMagnitude = 1f;
					}
				}
				else if (elements2 != Elements.Ice)
				{
					if (elements2 != Elements.Steam)
					{
						if (elements2 == Elements.Poison)
						{
							oSpell.PoisonMagnitude = 1f;
						}
					}
					else
					{
						oSpell.SteamMagnitude = 1f;
					}
				}
				else
				{
					oSpell.IceMagnitude = 1f;
				}
			}
		}

		// Token: 0x06002FF8 RID: 12280 RVA: 0x0018758C File Offset: 0x0018578C
		public static void DefaultSpell(Elements[] iElements, out Spell oSpell)
		{
			oSpell = default(Spell);
			foreach (Elements elements in iElements)
			{
				oSpell.Element |= elements;
				for (int j = 0; j < 11; j++)
				{
					Elements elements2 = Defines.ElementFromIndex(j);
					Elements elements3 = elements & elements2;
					if (elements3 <= Elements.Arcane)
					{
						if (elements3 <= Elements.Fire)
						{
							switch (elements3)
							{
							case Elements.Earth:
								oSpell.EarthMagnitude += 1f;
								break;
							case Elements.Water:
								oSpell.WaterMagnitude += 1f;
								break;
							case Elements.Earth | Elements.Water:
								break;
							case Elements.Cold:
								oSpell.ColdMagnitude += 1f;
								break;
							default:
								if (elements3 == Elements.Fire)
								{
									oSpell.FireMagnitude += 1f;
								}
								break;
							}
						}
						else if (elements3 != Elements.Lightning)
						{
							if (elements3 == Elements.Arcane)
							{
								oSpell.ArcaneMagnitude += 1f;
							}
						}
						else
						{
							oSpell.LightningMagnitude += 1f;
						}
					}
					else if (elements3 <= Elements.Shield)
					{
						if (elements3 != Elements.Life)
						{
							if (elements3 == Elements.Shield)
							{
								oSpell.ShieldMagnitude += 1f;
							}
						}
						else
						{
							oSpell.LifeMagnitude += 1f;
						}
					}
					else if (elements3 != Elements.Ice)
					{
						if (elements3 != Elements.Steam)
						{
							if (elements3 == Elements.Poison)
							{
								oSpell.PoisonMagnitude += 1f;
							}
						}
						else
						{
							oSpell.SteamMagnitude += 1f;
						}
					}
					else
					{
						oSpell.IceMagnitude += 1f;
					}
				}
			}
		}

		// Token: 0x06002FF9 RID: 12281 RVA: 0x00187768 File Offset: 0x00185968
		internal Spell Normalize()
		{
			Spell result = this;
			float num = this.TotalMagnitude();
			result.EarthMagnitude /= num;
			result.WaterMagnitude /= num;
			result.ColdMagnitude /= num;
			result.FireMagnitude /= num;
			result.LightningMagnitude /= num;
			result.LifeMagnitude /= num;
			result.ShieldMagnitude /= num;
			result.SteamMagnitude /= num;
			result.ArcaneMagnitude /= num;
			result.IceMagnitude /= num;
			result.PoisonMagnitude /= num;
			return result;
		}

		// Token: 0x040033E8 RID: 13288
		public static readonly Vector3 EARTHCOLOR = new Vector3(0.3f, 0.2f, 0.1f);

		// Token: 0x040033E9 RID: 13289
		public static readonly Vector3 WATERCOLOR = new Vector3(0f, 0.7f, 1.3f);

		// Token: 0x040033EA RID: 13290
		public static readonly Vector3 COLDCOLOR = new Vector3(1f, 1f, 1.4f);

		// Token: 0x040033EB RID: 13291
		public static readonly Vector3 FIRECOLOR = new Vector3(1.8f, 0.6f, 0.4f);

		// Token: 0x040033EC RID: 13292
		public static readonly Vector3 LIGHTNINGCOLOR = new Vector3(0.75f, 0.5f, 1f);

		// Token: 0x040033ED RID: 13293
		public static readonly Vector3 ARCANECOLOR = new Vector3(2f, 0.4f, 0.6f);

		// Token: 0x040033EE RID: 13294
		public static readonly Vector3 LIFECOLOR = new Vector3(0.2f, 1.6f, 0.2f);

		// Token: 0x040033EF RID: 13295
		public static readonly Vector3 SHIELDCOLOR = new Vector3(2f, 1.5f, 1f);

		// Token: 0x040033F0 RID: 13296
		public static readonly Vector3 STEAMCOLOR = new Vector3(1f, 1f, 1f);

		// Token: 0x040033F1 RID: 13297
		public static readonly Vector3 ICECOLOR = new Vector3(0.8f, 0.9f, 1.4f);

		// Token: 0x040033F2 RID: 13298
		public static readonly Vector3 POISONCOLOR = new Vector3(1f, 1.2f, 0f);

		// Token: 0x040033F3 RID: 13299
		public Elements Element;

		// Token: 0x040033F4 RID: 13300
		public float EarthMagnitude;

		// Token: 0x040033F5 RID: 13301
		public float WaterMagnitude;

		// Token: 0x040033F6 RID: 13302
		public float ColdMagnitude;

		// Token: 0x040033F7 RID: 13303
		public float FireMagnitude;

		// Token: 0x040033F8 RID: 13304
		public float LightningMagnitude;

		// Token: 0x040033F9 RID: 13305
		public float ArcaneMagnitude;

		// Token: 0x040033FA RID: 13306
		public float LifeMagnitude;

		// Token: 0x040033FB RID: 13307
		public float ShieldMagnitude;

		// Token: 0x040033FC RID: 13308
		public float IceMagnitude;

		// Token: 0x040033FD RID: 13309
		public float SteamMagnitude;

		// Token: 0x040033FE RID: 13310
		public float PoisonMagnitude;
	}
}
