using System;
using Magicka.Audio;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework.Audio;

namespace Magicka.GameLogic.Entities.CharacterStates
{
	// Token: 0x0200057D RID: 1405
	public class ChargeState : BaseState
	{
		// Token: 0x170009E7 RID: 2535
		// (get) Token: 0x06002A13 RID: 10771 RVA: 0x0014B260 File Offset: 0x00149460
		public static ChargeState Instance
		{
			get
			{
				if (ChargeState.mSingelton == null)
				{
					lock (ChargeState.mSingeltonLock)
					{
						if (ChargeState.mSingelton == null)
						{
							ChargeState.mSingelton = new ChargeState();
						}
					}
				}
				return ChargeState.mSingelton;
			}
		}

		// Token: 0x06002A14 RID: 10772 RVA: 0x0014B2B4 File Offset: 0x001494B4
		public override void OnEnter(Character iOwner)
		{
			iOwner.SetInvisible(0f);
			iOwner.Ethereal(false, 1f, 1f);
			switch (iOwner.CastType)
			{
			case CastType.Force:
				iOwner.GoToAnimation(Animations.charge_force, 0.075f);
				break;
			case CastType.Area:
				iOwner.GoToAnimation(Animations.charge_area, 0.075f);
				break;
			}
			iOwner.ChargeCue = AudioManager.Instance.PlayCue(Banks.Spells, ChargeState.CHARGE_SOUND_HASH, iOwner.AudioEmitter);
			iOwner.TurnSpeed *= 0.125f;
			iOwner.SpellPower = 0f;
			if (iOwner is Avatar)
			{
				(iOwner as Avatar).ChargeUnlocked = false;
			}
		}

		// Token: 0x06002A15 RID: 10773 RVA: 0x0014B364 File Offset: 0x00149564
		public override BaseState Update(Character iOwner, float iDeltaTime)
		{
			BaseState baseState = this.UpdateBloatDeath(iOwner, iDeltaTime);
			if (baseState == null)
			{
				baseState = this.UpdateHit(iOwner, iDeltaTime);
			}
			if (baseState != null)
			{
				return baseState;
			}
			if (iOwner.PlayState.IsInCutscene)
			{
				return IdleState.Instance;
			}
			if (!iOwner.AnimationController.IsLooping)
			{
				if (iOwner.AnimationController.HasFinished)
				{
					switch (iOwner.CastType)
					{
					case CastType.Force:
						iOwner.GoToAnimation(Animations.charge_force_loop, 0.075f);
						break;
					case CastType.Area:
						iOwner.GoToAnimation(Animations.charge_area_loop, 0.075f);
						break;
					}
					if (iOwner.ChargeCue != null)
					{
						iOwner.ChargeCue.Stop(AudioStopOptions.AsAuthored);
					}
					iOwner.ChargeCue = AudioManager.Instance.PlayCue(Banks.Spells, ChargeState.CHARGE_LOOP_SOUND_HASH, iOwner.AudioEmitter);
					iOwner.SpellPower = 1f;
				}
				else if (!iOwner.AnimationController.CrossFadeEnabled)
				{
					iOwner.SpellPower = iOwner.AnimationController.Time / iOwner.AnimationController.AnimationClip.Duration;
				}
			}
			if (iOwner is Avatar && ((iOwner.CastType == CastType.Force && !(iOwner as Avatar).CastButton(CastType.Force)) | (iOwner.CastType == CastType.Area && !(iOwner as Avatar).CastButton(CastType.Area))))
			{
				return CastState.Instance;
			}
			return null;
		}

		// Token: 0x06002A16 RID: 10774 RVA: 0x0014B4A7 File Offset: 0x001496A7
		public override void OnExit(Character iOwner)
		{
			if (iOwner.ChargeCue != null && !iOwner.ChargeCue.IsStopping)
			{
				iOwner.ChargeCue.Stop(AudioStopOptions.AsAuthored);
			}
			if (iOwner.Spell.Element != Elements.None)
			{
				iOwner.CastSpell(true, "");
			}
		}

		// Token: 0x04002D81 RID: 11649
		private static ChargeState mSingelton;

		// Token: 0x04002D82 RID: 11650
		private static volatile object mSingeltonLock = new object();

		// Token: 0x04002D83 RID: 11651
		public static readonly int CHARGE_SOUND_HASH = "spell_projectile_precharge".GetHashCodeCustom();

		// Token: 0x04002D84 RID: 11652
		public static readonly int CHARGE_LOOP_SOUND_HASH = "spell_projectile_charge_loop".GetHashCodeCustom();
	}
}
