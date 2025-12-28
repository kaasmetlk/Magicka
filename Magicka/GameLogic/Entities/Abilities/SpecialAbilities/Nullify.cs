using System;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Magicka.Network;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000544 RID: 1348
	internal class Nullify : SpecialAbility
	{
		// Token: 0x1700095A RID: 2394
		// (get) Token: 0x0600280E RID: 10254 RVA: 0x0012587C File Offset: 0x00123A7C
		public static Nullify Instance
		{
			get
			{
				if (Nullify.sSingelton == null)
				{
					lock (Nullify.sSingeltonLock)
					{
						if (Nullify.sSingelton == null)
						{
							Nullify.sSingelton = new Nullify();
						}
					}
				}
				return Nullify.sSingelton;
			}
		}

		// Token: 0x0600280F RID: 10255 RVA: 0x001258D0 File Offset: 0x00123AD0
		private Nullify() : base(Animations.cast_magick_self, "#magick_nullify".GetHashCodeCustom())
		{
		}

		// Token: 0x06002810 RID: 10256 RVA: 0x001258E4 File Offset: 0x00123AE4
		public Nullify(Animations iAnimation) : base(iAnimation, "#magick_nullify".GetHashCodeCustom())
		{
		}

		// Token: 0x06002811 RID: 10257 RVA: 0x001258F7 File Offset: 0x00123AF7
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			return this.Execute(iOwner.Position, iPlayState);
		}

		// Token: 0x06002812 RID: 10258 RVA: 0x0012590F File Offset: 0x00123B0F
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				this.NullifyArea(iPlayState, iPosition, true);
			}
			return true;
		}

		// Token: 0x06002813 RID: 10259 RVA: 0x00125928 File Offset: 0x00123B28
		public void NullifyArea(PlayState iPlayState, Vector3 iPosition, bool iHitBoss)
		{
			AudioManager.Instance.PlayCue(Banks.Spells, Nullify.SOUND_HASH);
			Flash.Instance.Execute(iPlayState.Scene, 0.2f);
			Portal.Instance.Kill();
			SpellManager.Instance.ClearMagicks();
			StaticList<Entity> entities = iPlayState.EntityManager.Entities;
			int i = 0;
			while (i < entities.Count)
			{
				IDamageable damageable = entities[i] as IDamageable;
				if (entities[i] is TornadoEntity)
				{
					entities[i].Kill();
					goto IL_DA;
				}
				if (entities[i] is VortexEntity)
				{
					entities[i].Kill();
					goto IL_DA;
				}
				if (entities[i] is MissileEntity)
				{
					entities[i].Kill();
					goto IL_DA;
				}
				if (entities[i] is OtherworldlyBolt)
				{
					(entities[i] as OtherworldlyBolt).DestroyOnNetwork(false, false, null, false);
					goto IL_DA;
				}
				if (damageable != null)
				{
					goto IL_DA;
				}
				IL_2FB:
				i++;
				continue;
				IL_DA:
				bool flag = false;
				Character character = entities[i] as Character;
				if (character != null)
				{
					character.StopStatusEffects(StatusEffects.Burning | StatusEffects.Wet | StatusEffects.Frozen | StatusEffects.Cold | StatusEffects.Poisoned | StatusEffects.Healing | StatusEffects.Greased | StatusEffects.Steamed);
					if (character.IsEntangled)
					{
						flag = true;
						character.ReleaseEntanglement();
					}
					if (character.IsSelfShielded)
					{
						flag = true;
						character.RemoveSelfShield();
					}
					if (character.IsFeared)
					{
						flag = true;
						character.RemoveFear();
					}
					if (character.IsInvisibile)
					{
						flag = true;
						character.RemoveInvisibility();
					}
					if (character.IsHypnotized)
					{
						flag = true;
						character.StopHypnotize();
					}
					if (character.CurrentSpell != null)
					{
						flag = true;
						character.CurrentSpell.Stop(character);
					}
					if (character.SpellQueue.Count > 0)
					{
						flag = true;
						character.SpellQueue.Clear();
					}
					if (character.IsLevitating)
					{
						character.StopLevitate();
					}
					if (!character.mBubble)
					{
						character.ClearAura();
					}
					NonPlayerCharacter nonPlayerCharacter = character as NonPlayerCharacter;
					Avatar avatar = character as Avatar;
					if (avatar != null)
					{
						avatar.Player.IconRenderer.Clear();
					}
					else if (nonPlayerCharacter != null)
					{
						if (nonPlayerCharacter.IsCharmed)
						{
							flag = true;
							nonPlayerCharacter.EndCharm();
						}
						if (nonPlayerCharacter.IsSummoned && nonPlayerCharacter.Name != "cross")
						{
							flag = true;
							nonPlayerCharacter.Kill();
						}
					}
				}
				else if (entities[i] is Barrier | entities[i] is SpellMine | entities[i] is Grease.GreaseField | entities[i] is SummonDeath.MagickDeath)
				{
					damageable.Kill();
					flag = true;
				}
				else if (entities[i] is Shield)
				{
					damageable.Kill();
				}
				else if (entities[i] is BossSpellCasterZone && iHitBoss)
				{
					BossSpellCasterZone bossSpellCasterZone = entities[i] as BossSpellCasterZone;
					if (bossSpellCasterZone.Owner is Grimnir2 && bossSpellCasterZone.Index == 0)
					{
						(bossSpellCasterZone.Owner as Grimnir2).Nullify();
					}
				}
				if (flag)
				{
					Matrix orientation = damageable.Body.Orientation;
					orientation.Translation = damageable.Position;
					VisualEffectReference visualEffectReference;
					EffectManager.Instance.StartEffect(Nullify.EFFECT, ref orientation, out visualEffectReference);
					goto IL_2FB;
				}
				goto IL_2FB;
			}
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
				triggerActionMessage.ActionType = TriggerActionType.Nullify;
				triggerActionMessage.Position = iPosition;
				triggerActionMessage.Bool0 = iHitBoss;
				NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
			}
		}

		// Token: 0x04002B9B RID: 11163
		private static Nullify sSingelton;

		// Token: 0x04002B9C RID: 11164
		private static volatile object sSingeltonLock = new object();

		// Token: 0x04002B9D RID: 11165
		public static readonly int EFFECT = "magick_nullified".GetHashCodeCustom();

		// Token: 0x04002B9E RID: 11166
		public static readonly int SOUND_HASH = "magick_nullify".GetHashCodeCustom();
	}
}
