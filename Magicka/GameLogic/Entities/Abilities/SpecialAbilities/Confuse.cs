using System;
using System.Collections.Generic;
using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000366 RID: 870
	internal class Confuse : SpecialAbility, IAbilityEffect, ITargetAbility
	{
		// Token: 0x06001A87 RID: 6791 RVA: 0x000B49AC File Offset: 0x000B2BAC
		public static Confuse GetInstance()
		{
			if (Confuse.sCache.Count > 0)
			{
				Confuse confuse = Confuse.sCache[Confuse.sCache.Count - 1];
				Confuse.sCache.RemoveAt(Confuse.sCache.Count - 1);
				Confuse.sActiveCaches.Add(confuse);
				return confuse;
			}
			Confuse confuse2 = new Confuse();
			Confuse.sActiveCaches.Add(confuse2);
			return confuse2;
		}

		// Token: 0x06001A88 RID: 6792 RVA: 0x000B4A14 File Offset: 0x000B2C14
		public static void InitializeCache(int iNr)
		{
			Confuse.sCache = new List<Confuse>(iNr);
			Confuse.sActiveCaches = new List<Confuse>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				Confuse.sCache.Add(new Confuse());
			}
		}

		// Token: 0x1700069C RID: 1692
		// (get) Token: 0x06001A89 RID: 6793 RVA: 0x000B4A52 File Offset: 0x000B2C52
		// (set) Token: 0x06001A8A RID: 6794 RVA: 0x000B4A5A File Offset: 0x000B2C5A
		public float TTL
		{
			get
			{
				return this.mTTL;
			}
			set
			{
				this.mTTL = value;
			}
		}

		// Token: 0x06001A8B RID: 6795 RVA: 0x000B4A63 File Offset: 0x000B2C63
		private Confuse() : base(Animations.cast_magick_direct, "#magick_confuse".GetHashCodeCustom())
		{
			this.mTTL = 15f;
		}

		// Token: 0x06001A8C RID: 6796 RVA: 0x000B4A84 File Offset: 0x000B2C84
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			base.Execute(iPosition, iPlayState);
			IDamageable closestIDamageable = iPlayState.EntityManager.GetClosestIDamageable(null, iPosition, 2f, false);
			if (closestIDamageable == null)
			{
				this.OnRemove();
				return false;
			}
			for (int i = 0; i < Confuse.sActiveCaches.Count; i++)
			{
				if (Confuse.sActiveCaches[i].mTarget == closestIDamageable)
				{
					Confuse.sActiveCaches[i].mTTL = 15f;
					this.OnRemove();
					return true;
				}
			}
			return this.Execute(null, closestIDamageable as Entity, iPlayState);
		}

		// Token: 0x06001A8D RID: 6797 RVA: 0x000B4B0C File Offset: 0x000B2D0C
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			Vector3 position = iOwner.Position;
			Vector3 direction = iOwner.Direction;
			List<Entity> entities = iPlayState.EntityManager.GetEntities(position, 14f, true);
			float num = float.MaxValue;
			Character character = null;
			for (int i = 0; i < entities.Count; i++)
			{
				if (entities[i] is Character && entities[i] != iOwner)
				{
					Vector3 position2 = entities[i].Position;
					Vector3 vector;
					Vector3.Subtract(ref position2, ref position, out vector);
					vector.Y = 0f;
					vector.Normalize();
					float num2 = MagickaMath.Angle(ref direction, ref vector);
					if (num2 < num)
					{
						num = num2;
						character = (entities[i] as Character);
					}
				}
			}
			iPlayState.EntityManager.ReturnEntityList(entities);
			if (character == null)
			{
				AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL, iOwner.AudioEmitter);
				this.OnRemove();
				return false;
			}
			return this.Execute(iOwner, character, iPlayState);
		}

		// Token: 0x06001A8E RID: 6798 RVA: 0x000B4C04 File Offset: 0x000B2E04
		public bool Execute(ISpellCaster iOwner, Entity iTarget, PlayState iPlayState)
		{
			if (!(iTarget is Character))
			{
				return false;
			}
			for (int i = 0; i < Confuse.sActiveCaches.Count; i++)
			{
				if (Confuse.sActiveCaches[i].mTarget == iTarget)
				{
					Confuse.sActiveCaches[i].mTTL = 15f;
					AudioManager.Instance.PlayCue(Banks.Characters, Confuse.SOUND_EFFECT, iOwner.AudioEmitter);
					Confuse.sCache.Add(this);
					return true;
				}
			}
			this.mTarget = (iTarget as Character);
			if (this.mTarget is Avatar && (this.mTarget as Avatar).Player != null && !((this.mTarget as Avatar).Player.Gamer is NetworkGamer))
			{
				this.mControllerTarget = (this.mTarget as Avatar).Player.Controller;
				this.mControllerTarget.Invert(true);
			}
			else if (this.mTarget is NonPlayerCharacter)
			{
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
					triggerActionMessage.Handle = this.mTarget.Handle;
					triggerActionMessage.Bool0 = true;
					triggerActionMessage.ActionType = TriggerActionType.Confuse;
					NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
				}
				(this.mTarget as NonPlayerCharacter).Confuse(Factions.NONE);
			}
			this.mTTL = 15f;
			AudioManager.Instance.PlayCue(Banks.Characters, Confuse.SOUND_EFFECT, iTarget.AudioEmitter);
			SpellManager.Instance.AddSpellEffect(this);
			return true;
		}

		// Token: 0x1700069D RID: 1693
		// (get) Token: 0x06001A8F RID: 6799 RVA: 0x000B4D83 File Offset: 0x000B2F83
		public bool IsDead
		{
			get
			{
				return this.mTTL <= 0f;
			}
		}

		// Token: 0x06001A90 RID: 6800 RVA: 0x000B4D98 File Offset: 0x000B2F98
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (this.mTarget == null || this.mTarget.HitPoints <= 0f)
			{
				this.mTTL = 0f;
				return;
			}
			this.mTTL -= iDeltaTime;
			Matrix identity = Matrix.Identity;
			identity.Translation = this.mTarget.Position;
			if (!EffectManager.Instance.UpdateOrientation(ref this.mEffect, ref identity))
			{
				EffectManager.Instance.StartEffect(Confuse.EFFECT_ID, ref identity, out this.mEffect);
			}
		}

		// Token: 0x06001A91 RID: 6801 RVA: 0x000B4E20 File Offset: 0x000B3020
		public void OnRemove()
		{
			EffectManager.Instance.Stop(ref this.mEffect);
			if (this.mControllerTarget != null)
			{
				this.mControllerTarget.Invert(false);
			}
			if ((!(this.mTarget is Avatar) || (this.mTarget as Avatar).Player == null || (this.mTarget as Avatar).Player.Gamer is NetworkGamer) && this.mTarget is NonPlayerCharacter)
			{
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
					triggerActionMessage.Handle = this.mTarget.Handle;
					triggerActionMessage.Bool0 = false;
					triggerActionMessage.ActionType = TriggerActionType.Confuse;
					NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
				}
				(this.mTarget as NonPlayerCharacter).Confuse(this.mTarget.Template.Faction);
			}
			this.mControllerTarget = null;
			this.mTarget = null;
			this.mTTL = 0f;
			Confuse.sActiveCaches.Remove(this);
			Confuse.sCache.Add(this);
		}

		// Token: 0x04001CDC RID: 7388
		private const float DEFAULT_TTL = 15f;

		// Token: 0x04001CDD RID: 7389
		private const float RADIUS = 14f;

		// Token: 0x04001CDE RID: 7390
		private static List<Confuse> sCache;

		// Token: 0x04001CDF RID: 7391
		private static List<Confuse> sActiveCaches;

		// Token: 0x04001CE0 RID: 7392
		private static readonly int SOUND_EFFECT = "boss_fafnir_confuse".GetHashCodeCustom();

		// Token: 0x04001CE1 RID: 7393
		private static readonly int EFFECT_ID = "confused".GetHashCodeCustom();

		// Token: 0x04001CE2 RID: 7394
		private float mTTL;

		// Token: 0x04001CE3 RID: 7395
		private Character mTarget;

		// Token: 0x04001CE4 RID: 7396
		private Controller mControllerTarget;

		// Token: 0x04001CE5 RID: 7397
		private VisualEffectReference mEffect;
	}
}
