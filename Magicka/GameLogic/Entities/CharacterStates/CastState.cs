using System;
using Magicka.GameLogic.Spells;

namespace Magicka.GameLogic.Entities.CharacterStates
{
	// Token: 0x02000437 RID: 1079
	public class CastState : BaseState
	{
		// Token: 0x17000820 RID: 2080
		// (get) Token: 0x06002170 RID: 8560 RVA: 0x000EEA0C File Offset: 0x000ECC0C
		public static CastState Instance
		{
			get
			{
				if (CastState.mSingelton == null)
				{
					lock (CastState.mSingeltonLock)
					{
						if (CastState.mSingelton == null)
						{
							CastState.mSingelton = new CastState();
						}
					}
				}
				return CastState.mSingelton;
			}
		}

		// Token: 0x06002171 RID: 8561 RVA: 0x000EEA60 File Offset: 0x000ECC60
		public override void OnEnter(Character iOwner)
		{
			iOwner.SetInvisible(0f);
			iOwner.Ethereal(false, 1f, 1f);
			SpellType spellType = iOwner.Spell.GetSpellType();
			if (spellType == SpellType.Magick)
			{
				SpellMagickConverter spellMagickConverter = default(SpellMagickConverter);
				spellMagickConverter.Spell = iOwner.Spell;
				iOwner.Magick = spellMagickConverter.Magick.Effect;
				iOwner.GoToAnimation(iOwner.Magick.Animation, 0.075f);
				iOwner.CharacterBody.AllowRotate = false;
			}
			else
			{
				switch (iOwner.CastType)
				{
				case CastType.Force:
					switch (spellType)
					{
					case SpellType.Push:
						iOwner.GoToAnimation(Animations.cast_force_push, 0.075f);
						break;
					case SpellType.Spray:
						iOwner.GoToAnimation(Animations.cast_force_spray, 0.075f);
						break;
					case SpellType.Projectile:
						iOwner.GoToAnimation(Animations.cast_force_projectile, 0.075f);
						break;
					case SpellType.Shield:
						iOwner.GoToAnimation(Animations.cast_force_shield, 0.075f);
						break;
					case SpellType.Beam:
						iOwner.GoToAnimation(Animations.cast_force_railgun, 0.075f);
						break;
					case SpellType.Lightning:
						iOwner.GoToAnimation(Animations.cast_force_lightning, 0.075f);
						break;
					}
					break;
				case CastType.Area:
					switch (spellType)
					{
					case SpellType.Push:
						iOwner.GoToAnimation(Animations.cast_area_push, 0.075f);
						break;
					case SpellType.Spray:
						iOwner.GoToAnimation(Animations.cast_area_blast, 0.075f);
						break;
					case SpellType.Projectile:
						iOwner.GoToAnimation(Animations.cast_area_ground, 0.075f);
						break;
					case SpellType.Shield:
						iOwner.GoToAnimation(Animations.cast_area_fireworks, 0.075f);
						break;
					case SpellType.Beam:
						iOwner.GoToAnimation(Animations.cast_area_blast, 0.075f);
						break;
					case SpellType.Lightning:
						iOwner.GoToAnimation(Animations.cast_area_lightning, 0.075f);
						break;
					}
					break;
				case CastType.Self:
					iOwner.GoToAnimation(Animations.cast_self, 0.075f);
					break;
				case CastType.Weapon:
					if (iOwner.Equipment[0] != null && iOwner.Equipment[0].Item != null && iOwner.Equipment[0].Item.SpellCharged && iOwner.SpellQueue.Count == 0)
					{
						switch (iOwner.Equipment[0].Item.PeekSpell().GetSpellType())
						{
						case SpellType.Spray:
							iOwner.GoToAnimation(Animations.cast_sword_spray, 0.075f);
							goto IL_2CB;
						case SpellType.Projectile:
							iOwner.GoToAnimation(Animations.cast_sword_projectile, 0.075f);
							goto IL_2CB;
						case SpellType.Shield:
							iOwner.GoToAnimation(Animations.cast_sword_projectile, 0.075f);
							goto IL_2CB;
						case SpellType.Beam:
							iOwner.GoToAnimation(Animations.cast_sword_railgun, 0.075f);
							goto IL_2CB;
						case SpellType.Lightning:
							iOwner.GoToAnimation(Animations.cast_sword_lightning, 0.075f);
							goto IL_2CB;
						}
						iOwner.GoToAnimation(Animations.cast_sword, 0.075f);
					}
					else
					{
						iOwner.GoToAnimation(Animations.cast_sword, 0.15f);
					}
					break;
				}
			}
			IL_2CB:
			if (iOwner.HasStatus(StatusEffects.Frozen) && iOwner.CastType == CastType.Self)
			{
				iOwner.CastSpell(true, "");
			}
		}

		// Token: 0x06002172 RID: 8562 RVA: 0x000EED58 File Offset: 0x000ECF58
		public override BaseState Update(Character iOwner, float iDeltaTime)
		{
			BaseState baseState = this.UpdateBloatDeath(iOwner, iDeltaTime);
			if (baseState != null)
			{
				return baseState;
			}
			if (iOwner.CurrentAnimation == Animations.idle)
			{
				return IdleState.Instance;
			}
			if (iOwner.CharacterBody.IsPushed)
			{
				return PushedState.Instance;
			}
			if (iOwner.IsKnockedDown)
			{
				return KnockDownState.Instance;
			}
			if (iOwner.AnimationController.IsLooping && !iOwner.AnimationController.CrossFadeEnabled && iOwner.CastType != CastType.Force)
			{
				if (iOwner.CharacterBody.Movement.Length() > 1E-45f)
				{
					iOwner.GoToAnimation(Animations.move_walk, 0.25f);
					return MoveState.Instance;
				}
				return IdleState.Instance;
			}
			else if (iOwner.AnimationController.HasFinished)
			{
				if (iOwner.CharacterBody.Movement.Length() > 1E-45f)
				{
					iOwner.GoToAnimation(Animations.move_walk, 0.25f);
					return MoveState.Instance;
				}
				return IdleState.Instance;
			}
			else if (iOwner.CurrentSpell != null && !iOwner.CurrentSpell.Active)
			{
				if (iOwner.Attacking)
				{
					return AttackState.Instance;
				}
				if (iOwner.CharacterBody.Movement.Length() > 1E-45f)
				{
					iOwner.GoToAnimation(Animations.move_walk, 0.25f);
					return MoveState.Instance;
				}
				return IdleState.Instance;
			}
			else
			{
				if (iOwner.HasStatus(StatusEffects.Frozen))
				{
					return IdleState.Instance;
				}
				return null;
			}
		}

		// Token: 0x06002173 RID: 8563 RVA: 0x000EEEA8 File Offset: 0x000ED0A8
		public override void OnExit(Character iOwner)
		{
			NonPlayerCharacter nonPlayerCharacter = iOwner as NonPlayerCharacter;
			if (nonPlayerCharacter != null)
			{
				nonPlayerCharacter.AI.BusyAbility = null;
			}
			if (iOwner.CurrentSpell != null)
			{
				iOwner.CurrentSpell.Stop(iOwner);
				iOwner.CurrentSpell = null;
			}
			iOwner.CastType = CastType.None;
			iOwner.CharacterBody.AllowRotate = true;
		}

		// Token: 0x0400243E RID: 9278
		private static CastState mSingelton;

		// Token: 0x0400243F RID: 9279
		private static volatile object mSingeltonLock = new object();
	}
}
