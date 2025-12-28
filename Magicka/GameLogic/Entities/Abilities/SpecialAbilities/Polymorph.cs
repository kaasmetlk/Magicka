using System;
using System.Collections.Generic;
using System.Linq;
using Magicka.AI;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000459 RID: 1113
	internal class Polymorph : SpecialAbility, IAbilityEffect, ITargetAbility
	{
		// Token: 0x06002203 RID: 8707 RVA: 0x000F3A80 File Offset: 0x000F1C80
		public static Polymorph GetInstance()
		{
			if (Polymorph.sCache.Count > 0)
			{
				Polymorph polymorph = Polymorph.sCache[Polymorph.sCache.Count - 1];
				Polymorph.sCache.RemoveAt(Polymorph.sCache.Count - 1);
				Polymorph.sActiveCaches.Add(polymorph);
				return polymorph;
			}
			Polymorph polymorph2 = new Polymorph(null);
			Polymorph.sActiveCaches.Add(polymorph2);
			return polymorph2;
		}

		// Token: 0x06002204 RID: 8708 RVA: 0x000F3AE8 File Offset: 0x000F1CE8
		public static void InitializeCache(int iNr, PlayState iPlaystate)
		{
			Polymorph.sCache = new List<Polymorph>(iNr);
			Polymorph.sActiveCaches = new List<Polymorph>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				Polymorph.sCache.Add(new Polymorph(iPlaystate));
			}
		}

		// Token: 0x06002205 RID: 8709 RVA: 0x000F3B28 File Offset: 0x000F1D28
		private Polymorph(PlayState iPlayState) : base(Animations.cast_magick_direct, "#magick_polymorph".GetHashCodeCustom())
		{
			if (iPlayState != null)
			{
				iPlayState.Content.Load<CharacterTemplate>("data/characters/polymorph_horse");
				iPlayState.Content.Load<CharacterTemplate>("data/characters/polymorph_imp");
				iPlayState.Content.Load<CharacterTemplate>("data/characters/polymorph_goblin");
				iPlayState.Content.Load<CharacterTemplate>("data/characters/polymorph_troll");
				iPlayState.Content.Load<CharacterTemplate>("data/characters/horse");
				iPlayState.Content.Load<CharacterTemplate>("data/characters/imp");
				iPlayState.Content.Load<CharacterTemplate>("data/characters/goblin_bomber");
				iPlayState.Content.Load<CharacterTemplate>("data/characters/troll_forest");
			}
		}

		// Token: 0x06002206 RID: 8710 RVA: 0x000F3BD8 File Offset: 0x000F1DD8
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			this.mPlayState = iPlayState;
			Vector3 position = iOwner.Position;
			Vector3 direction = iOwner.Direction;
			Vector3 vector;
			Vector3.Multiply(ref direction, 3f, out vector);
			Vector3.Add(ref vector, ref position, out vector);
			Character iTarget;
			if (!this.FindClosestTarget(iPlayState, iOwner, 6f, ref vector, out iTarget))
			{
				this.OnRemove();
				AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL, iOwner.AudioEmitter);
				return false;
			}
			return this.Execute(iOwner, iTarget, iPlayState);
		}

		// Token: 0x06002207 RID: 8711 RVA: 0x000F3C50 File Offset: 0x000F1E50
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			this.mPlayState = iPlayState;
			Character character;
			if (!this.FindClosestTarget(iPlayState, null, 6f, ref iPosition, out character))
			{
				this.OnRemove();
				return false;
			}
			if (character is GenericBoss || character is WarlordCharacter || character is IBoss)
			{
				this.OnRemove();
				return false;
			}
			for (int i = 0; i < Polymorph.sActiveCaches.Count; i++)
			{
				if (Polymorph.sActiveCaches[i].mTarget == character)
				{
					Polymorph.sActiveCaches[i].OnRemove();
					this.OnRemove();
					return true;
				}
			}
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				if (character is NonPlayerCharacter)
				{
					int iNewType = Polymorph.sNPCTypes[SpecialAbility.RANDOM.Next(Polymorph.sNPCTypes.Length)];
					NonPlayerCharacter nonPlayerCharacter;
					if (this.CreateNPCPolymorph(character as NonPlayerCharacter, iPlayState, iNewType, out nonPlayerCharacter))
					{
						this.mTarget = nonPlayerCharacter;
						iPlayState.EntityManager.AddEntity(this.mTarget);
					}
				}
				else if (character is Avatar)
				{
					int iNewType2 = Polymorph.sAvatarTypes[SpecialAbility.RANDOM.Next(Polymorph.sNPCTypes.Length)];
					this.mTypeBeforePolymorph = character.Template.ID;
					Polymorph.PolymorphAvatar(character as Avatar, iNewType2, ref Polymorph.sTemporaryDataHolder);
					this.mTarget = character;
				}
			}
			this.mTTL = 20f;
			SpellManager.Instance.AddSpellEffect(this);
			return true;
		}

		// Token: 0x06002208 RID: 8712 RVA: 0x000F3DA0 File Offset: 0x000F1FA0
		public bool Execute(ISpellCaster iOwner, Entity iTarget, PlayState iPlayState)
		{
			if (!(iTarget is Character))
			{
				return false;
			}
			this.mPlayState = iPlayState;
			for (int i = 0; i < Polymorph.sActiveCaches.Count; i++)
			{
				if (Polymorph.sActiveCaches[i].mTarget == iTarget)
				{
					Polymorph.sActiveCaches[i].OnRemove();
					this.OnRemove();
					return true;
				}
			}
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				if (iTarget is NonPlayerCharacter)
				{
					if (iTarget is GenericBoss)
					{
						return false;
					}
					int iNewType = Polymorph.sNPCTypes[SpecialAbility.RANDOM.Next(Polymorph.sNPCTypes.Length)];
					NonPlayerCharacter nonPlayerCharacter;
					if (this.CreateNPCPolymorph(iTarget as NonPlayerCharacter, iPlayState, iNewType, out nonPlayerCharacter))
					{
						this.mTarget = nonPlayerCharacter;
						iPlayState.EntityManager.AddEntity(this.mTarget);
					}
				}
				else if (iTarget is Avatar)
				{
					int iNewType2 = Polymorph.sAvatarTypes[SpecialAbility.RANDOM.Next(Polymorph.sNPCTypes.Length)];
					this.mTypeBeforePolymorph = (iTarget as Avatar).Template.ID;
					Polymorph.PolymorphAvatar(iTarget as Avatar, iNewType2, ref Polymorph.sTemporaryDataHolder);
					this.mTarget = (iTarget as Character);
				}
			}
			Vector3 position = iTarget.Position;
			Vector3 forward = iTarget.Body.Orientation.Forward;
			VisualEffectReference visualEffectReference;
			EffectManager.Instance.StartEffect(Polymorph.MAGICK_EFFECT, ref position, ref forward, out visualEffectReference);
			AudioManager.Instance.PlayCue(Banks.Additional, Polymorph.SOUND_EFFECT, iTarget.AudioEmitter);
			this.mTTL = 20f;
			SpellManager.Instance.AddSpellEffect(this);
			return true;
		}

		// Token: 0x1700082D RID: 2093
		// (get) Token: 0x06002209 RID: 8713 RVA: 0x000F3F1D File Offset: 0x000F211D
		public bool IsDead
		{
			get
			{
				return this.mTTL <= 0f;
			}
		}

		// Token: 0x0600220A RID: 8714 RVA: 0x000F3F2F File Offset: 0x000F212F
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mTTL -= iDeltaTime;
			if (this.mTarget == null || this.mTarget.HitPoints <= 0f)
			{
				this.mTTL = 0f;
			}
		}

		// Token: 0x0600220B RID: 8715 RVA: 0x000F3F64 File Offset: 0x000F2164
		public void OnRemove()
		{
			if (this.mTarget != null && this.mTarget.HitPoints > 0f && this.mPlayState != null && this.mPlayState.Level != null)
			{
				if (this.mTarget is NonPlayerCharacter)
				{
					NonPlayerCharacter nonPlayerCharacter;
					if (this.CreateNPCPolymorph(this.mTarget as NonPlayerCharacter, this.mTarget.PlayState, this.mTypeBeforePolymorph, out nonPlayerCharacter))
					{
						nonPlayerCharacter.PlayState.EntityManager.AddEntity(nonPlayerCharacter);
					}
				}
				else if (this.mTarget is Avatar)
				{
					Polymorph.PolymorphAvatar(this.mTarget as Avatar, this.mTypeBeforePolymorph, ref Polymorph.sTemporaryDataHolder);
				}
			}
			this.mStorage.Active = false;
			this.mStorage.EventIndex = -1;
			this.mStorage.Events = null;
			this.mTarget = null;
			this.mTTL = 0f;
			Polymorph.sActiveCaches.Remove(this);
			Polymorph.sCache.Add(this);
		}

		// Token: 0x0600220C RID: 8716 RVA: 0x000F4064 File Offset: 0x000F2264
		private bool FindClosestTarget(PlayState iPlayState, ISpellCaster iOwner, float iRadius, ref Vector3 iPosition, out Character oTarget)
		{
			List<Entity> entities = iPlayState.EntityManager.GetEntities(iPosition, iRadius, false, true);
			oTarget = null;
			float num = float.MaxValue;
			for (int i = 0; i < entities.Count; i++)
			{
				Character character = entities[i] as Character;
				if (character != null && character != iOwner && !character.IsEthereal && !string.Equals(character.Name, "boss_vlad_swamp", StringComparison.InvariantCultureIgnoreCase) && !string.Equals(character.Name, "vlad_vietnam", StringComparison.InvariantCultureIgnoreCase) && !string.Equals(character.Name, "vlad_future", StringComparison.InvariantCultureIgnoreCase))
				{
					Vector3 position = character.Position;
					float num2;
					Vector3.DistanceSquared(ref position, ref iPosition, out num2);
					if (num2 < num)
					{
						oTarget = character;
						num = num2;
					}
				}
			}
			iPlayState.EntityManager.ReturnEntityList(entities);
			return oTarget != null;
		}

		// Token: 0x0600220D RID: 8717 RVA: 0x000F4130 File Offset: 0x000F2330
		private bool CreateNPCPolymorph(NonPlayerCharacter iTarget, PlayState iPlayState, int iNewType, out NonPlayerCharacter oNPC)
		{
			this.mTypeBeforePolymorph = iTarget.Type;
			oNPC = NonPlayerCharacter.GetInstance(iPlayState);
			if (oNPC == null)
			{
				return false;
			}
			CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(iNewType);
			oNPC.Initialize(cachedTemplate, default(Vector3), iTarget.UniqueID);
			oNPC.CopyPolymorphValuesFrom(iTarget);
			oNPC.AI.CopyPolymorphValues(iTarget.AI, ref this.mStorage);
			Vector3 position = iTarget.Body.Position;
			Matrix orientation = iTarget.Body.Orientation;
			float num = oNPC.Radius + oNPC.Capsule.Length * 0.5f - (iTarget.Radius + iTarget.Capsule.Length * 0.5f);
			position.Y += num;
			oNPC.Body.MoveTo(position, orientation);
			iTarget.Terminate(false, false);
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
				triggerActionMessage.ActionType = TriggerActionType.SpawnNPC;
				triggerActionMessage.Handle = oNPC.Handle;
				triggerActionMessage.Template = iNewType;
				triggerActionMessage.Id = oNPC.UniqueID;
				triggerActionMessage.Position = position;
				triggerActionMessage.Direction = orientation.Forward;
				triggerActionMessage.Bool0 = false;
				NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
			}
			return true;
		}

		// Token: 0x0600220E RID: 8718 RVA: 0x000F4284 File Offset: 0x000F2484
		public static void PolymorphAvatar(Avatar iTarget, int iNewType, ref Polymorph.AvatarPolymorphData iData)
		{
			CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(iNewType);
			Vector3 position = iTarget.Body.Position;
			Matrix orientation = iTarget.Body.Orientation;
			float num = cachedTemplate.Radius + cachedTemplate.Length * 0.5f - (iTarget.Radius + iTarget.Capsule.Length * 0.5f);
			position.Y += num;
			iTarget.GetPolymorphValues(out iData);
			iTarget.KillAnimationActions();
			iTarget.SpellQueue.Clear();
			iTarget.Player.IconRenderer.Clear();
			if (Polymorph.sAvatarTypes.Contains(iNewType))
			{
				int outboundUDPStamp = iTarget.OutboundUDPStamp;
				iTarget.Polymorphed = true;
				iTarget.Initialize(cachedTemplate, -1, position, iTarget.UniqueID);
				iTarget.Polymorphed = true;
				iTarget.OutboundUDPStamp = outboundUDPStamp;
			}
			else
			{
				int outboundUDPStamp2 = iTarget.OutboundUDPStamp;
				iTarget.Polymorphed = false;
				iTarget.Initialize(cachedTemplate, -1, position, iTarget.UniqueID);
				iTarget.OutboundUDPStamp = outboundUDPStamp2;
			}
			iTarget.ApplyPolymorphValues(ref iData);
			iTarget.Body.MoveTo(position, orientation);
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
				characterActionMessage.Action = ActionType.Magick;
				characterActionMessage.Handle = iTarget.Handle;
				characterActionMessage.Param3I = 39;
				characterActionMessage.Param0I = iNewType;
				NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
			}
		}

		// Token: 0x0400250C RID: 9484
		private const float RADIUS = 6f;

		// Token: 0x0400250D RID: 9485
		private const float TTL = 20f;

		// Token: 0x0400250E RID: 9486
		private static List<Polymorph> sCache;

		// Token: 0x0400250F RID: 9487
		private static List<Polymorph> sActiveCaches;

		// Token: 0x04002510 RID: 9488
		private static readonly int MAGICK_EFFECT = "magick_polymorph".GetHashCodeCustom();

		// Token: 0x04002511 RID: 9489
		private static readonly int SOUND_EFFECT = "magick_polymorph".GetHashCodeCustom();

		// Token: 0x04002512 RID: 9490
		private static readonly int[] sAvatarTypes = new int[]
		{
			"polymorph_horse".GetHashCodeCustom(),
			"polymorph_imp".GetHashCodeCustom(),
			"polymorph_goblin".GetHashCodeCustom(),
			"polymorph_troll".GetHashCodeCustom()
		};

		// Token: 0x04002513 RID: 9491
		private static readonly int[] sNPCTypes = new int[]
		{
			"horse".GetHashCodeCustom(),
			"imp".GetHashCodeCustom(),
			"goblin_bomber".GetHashCodeCustom(),
			"troll_forest".GetHashCodeCustom()
		};

		// Token: 0x04002514 RID: 9492
		private Character mTarget;

		// Token: 0x04002515 RID: 9493
		private int mTypeBeforePolymorph;

		// Token: 0x04002516 RID: 9494
		private PlayState mPlayState;

		// Token: 0x04002517 RID: 9495
		private Polymorph.NPCPolymorphData mStorage;

		// Token: 0x04002518 RID: 9496
		internal static Polymorph.AvatarPolymorphData sTemporaryDataHolder;

		// Token: 0x04002519 RID: 9497
		private float mTTL;

		// Token: 0x0200045A RID: 1114
		internal struct AvatarPolymorphData
		{
			// Token: 0x0400251A RID: 9498
			public int Type;

			// Token: 0x0400251B RID: 9499
			public Factions Faction;

			// Token: 0x0400251C RID: 9500
			public float NormalizedHP;

			// Token: 0x0400251D RID: 9501
			public float TimeSinceLastDamage;

			// Token: 0x0400251E RID: 9502
			public float TimeSinceLastStatusDamage;

			// Token: 0x0400251F RID: 9503
			public Vector3 DesiredDirection;

			// Token: 0x04002520 RID: 9504
			public bool IsEthereal;

			// Token: 0x04002521 RID: 9505
			public Character FearedBy;

			// Token: 0x04002522 RID: 9506
			public Vector3 FearPosition;

			// Token: 0x04002523 RID: 9507
			public float FearTimer;

			// Token: 0x04002524 RID: 9508
			public Entity CharmOwner;

			// Token: 0x04002525 RID: 9509
			public VisualEffectReference CharmEffect;

			// Token: 0x04002526 RID: 9510
			public float CharmTimer;

			// Token: 0x04002527 RID: 9511
			public bool Hypnotized;

			// Token: 0x04002528 RID: 9512
			public VisualEffectReference HypnotizeEffect;

			// Token: 0x04002529 RID: 9513
			public Vector3 HypnotizeDirection;

			// Token: 0x0400252A RID: 9514
			public static StatusEffect[] sTempStatusEffects = new StatusEffect[9];
		}

		// Token: 0x0200045B RID: 1115
		internal struct NPCPolymorphData
		{
			// Token: 0x0400252B RID: 9515
			public bool Active;

			// Token: 0x0400252C RID: 9516
			public int EventIndex;

			// Token: 0x0400252D RID: 9517
			public AIEvent[] Events;
		}
	}
}
