using System;
using System.Collections.Generic;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PolygonHead;

namespace Magicka.GameLogic.Entities
{
	// Token: 0x0200053D RID: 1341
	internal class OtherworldlyBolt : Entity
	{
		// Token: 0x060027DE RID: 10206 RVA: 0x00123480 File Offset: 0x00121680
		public OtherworldlyBolt(PlayState iPlayState) : base(iPlayState)
		{
			OtherworldlyDischarge.Instance.Initialize(iPlayState);
			this.mBody = new Body();
			this.mBody.ApplyGravity = false;
			this.mBody.Immovable = true;
			this.mCollision = new CollisionSkin(this.mBody);
			this.mCollision.AddPrimitive(new Sphere(Vector3.Zero, 0.5f), 1, new MaterialProperties(0f, 0f, 0f));
			this.mBody.CollisionSkin = this.mCollision;
			this.mBody.CollisionSkin.callbackFn += this.OnCollision;
		}

		// Token: 0x060027DF RID: 10207 RVA: 0x00123530 File Offset: 0x00121730
		public void Spawn(PlayState iPlayState, ref Vector3 iSpawnPosition, ref Vector3 iSpawnDirection, float iSpeed)
		{
			this.mPlayState = iPlayState;
			this.mPlayState.EntityManager.AddEntity(this);
			this.mPosition = iSpawnPosition;
			Matrix identity = Matrix.Identity;
			this.mBody.MoveTo(ref this.mPosition, ref identity);
			this.mCheckTargetTimer = 0f;
			this.mAcceleration = iSpeed;
			this.mTerminalVelocity = iSpeed;
			this.mVelocity = iSpawnDirection * iSpeed;
			base.Initialize();
			this.mDone = false;
			this.mRemovable = false;
			this.mOkToStart = false;
			this.Play(ref iSpawnPosition);
			AudioManager.Instance.PlayCue(Banks.Additional, OtherworldlyBolt.SOUND_SPAWN, base.AudioEmitter);
		}

		// Token: 0x060027E0 RID: 10208 RVA: 0x001235E5 File Offset: 0x001217E5
		public void GoHunt()
		{
			this.mOkToStart = true;
			this.mSoundCueIdle = AudioManager.Instance.PlayCue(Banks.Additional, OtherworldlyBolt.SOUND_IDLE, base.AudioEmitter);
		}

		// Token: 0x060027E1 RID: 10209 RVA: 0x00123610 File Offset: 0x00121810
		private void Play(ref Vector3 iSpawnPosition)
		{
			if (EffectManager.Instance.IsActive(ref this.mEffectRef))
			{
				return;
			}
			Matrix matrix = Matrix.CreateTranslation(iSpawnPosition);
			EffectManager.Instance.StartEffect(OtherworldlyBolt.EFFECT, ref matrix, out this.mEffectRef);
		}

		// Token: 0x060027E2 RID: 10210 RVA: 0x00123654 File Offset: 0x00121854
		private void Stop(bool iInSilence)
		{
			EffectManager.Instance.Stop(ref this.mEffectRef);
			if (this.mSoundCueIdle != null)
			{
				this.mSoundCueIdle.Stop(AudioStopOptions.AsAuthored);
			}
			if (!iInSilence)
			{
				AudioManager.Instance.PlayCue(Banks.Additional, OtherworldlyBolt.SOUND_HIT, base.AudioEmitter);
			}
		}

		// Token: 0x060027E3 RID: 10211 RVA: 0x001236A3 File Offset: 0x001218A3
		protected override void AddImpulseVelocity(ref Vector3 iVelocity)
		{
			if (!this.mOkToStart)
			{
				return;
			}
			Vector3.Multiply(ref iVelocity, 2f, out iVelocity);
			Vector3.Add(ref this.mVelocity, ref iVelocity, out this.mVelocity);
		}

		// Token: 0x060027E4 RID: 10212 RVA: 0x001236CC File Offset: 0x001218CC
		private bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
		{
			if (!this.mOkToStart)
			{
				return false;
			}
			if (this.mDone)
			{
				return false;
			}
			if (iSkin1.Owner != null)
			{
				object tag = iSkin1.Owner.Tag;
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				if (tag is Character)
				{
					Character character = tag as Character;
					if (character.Type == OtherworldlyBolt.STARSPAWN_HASH)
					{
						return false;
					}
					if (!character.IsEthereal && (!character.IsSelfShielded || character.IsSolidSelfShielded))
					{
						this.SpawnCultist(character);
						flag = true;
					}
					flag2 = true;
				}
				else if (tag is Barrier || tag is Shield)
				{
					flag2 = true;
					flag3 = true;
				}
				else if (tag is MissileEntity)
				{
					Character character2 = (tag as MissileEntity).Owner as Character;
					if (character2 != null && (character2.Faction & Factions.FRIENDLY) == Factions.NONE)
					{
						return true;
					}
					flag2 = true;
					flag3 = true;
				}
				if (flag2 && NetworkManager.Instance.State != NetworkState.Client)
				{
					if (NetworkManager.Instance.State == NetworkState.Server)
					{
						TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
						triggerActionMessage.ActionType = TriggerActionType.OtherworldlyBoltDestroyed;
						triggerActionMessage.Handle = base.Handle;
						triggerActionMessage.Arg = (int)(tag as Entity).Handle;
						triggerActionMessage.Bool0 = flag;
						triggerActionMessage.Bool1 = flag3;
						triggerActionMessage.Bool2 = false;
						NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
					}
					this.Destroy(flag, flag3, tag as Entity, false);
					return false;
				}
			}
			return true;
		}

		// Token: 0x060027E5 RID: 10213 RVA: 0x00123823 File Offset: 0x00121A23
		internal void Reset()
		{
			this.mDone = true;
			this.mRemovable = true;
			this.mOkToStart = false;
		}

		// Token: 0x060027E6 RID: 10214 RVA: 0x0012383C File Offset: 0x00121A3C
		public void Destroy(bool iCultistSpawned, bool iKillTarget, Entity iTarget, bool iInSilence)
		{
			if (!iCultistSpawned && EffectManager.Instance.IsActive(ref this.mEffectRef))
			{
				Vector3 right = Vector3.Right;
				VisualEffectReference visualEffectReference;
				EffectManager.Instance.StartEffect(OtherworldlyBolt.EFFECT_HIT, ref this.mPosition, ref right, out visualEffectReference);
			}
			if (iKillTarget && iTarget != null)
			{
				iTarget.Kill();
			}
			if (iTarget != null && iTarget is Character)
			{
				Character character = iTarget as Character;
				character.RemoveSelfShield();
			}
			this.mRemovable = true;
			this.Stop(iInSilence);
		}

		// Token: 0x060027E7 RID: 10215 RVA: 0x001238B4 File Offset: 0x00121AB4
		public void DestroyOnNetwork(bool iCultistSpawned, bool iKillTarget, Entity iTarget, bool iInSilence)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
					triggerActionMessage.ActionType = TriggerActionType.OtherworldlyBoltDestroyed;
					triggerActionMessage.Handle = base.Handle;
					if (iTarget != null)
					{
						triggerActionMessage.Arg = (int)iTarget.Handle;
					}
					triggerActionMessage.Bool0 = iCultistSpawned;
					triggerActionMessage.Bool1 = iKillTarget;
					triggerActionMessage.Bool2 = iInSilence;
					NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
				}
				this.Destroy(iCultistSpawned, iKillTarget, iTarget, iInSilence);
			}
		}

		// Token: 0x060027E8 RID: 10216 RVA: 0x00123940 File Offset: 0x00121B40
		private void SpawnCultist(Character iCharacter)
		{
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
				triggerActionMessage.ActionType = TriggerActionType.OtherworldlyDischarge;
				triggerActionMessage.Handle = base.Handle;
				triggerActionMessage.Arg = (int)iCharacter.Handle;
				NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
			}
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				OtherworldlyDischarge.Instance.Execute(iCharacter, this as ISpellCaster, this.mPlayState);
			}
		}

		// Token: 0x060027E9 RID: 10217 RVA: 0x001239BC File Offset: 0x00121BBC
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (!this.mOkToStart)
			{
				return;
			}
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				this.mCheckTargetTimer -= iDeltaTime;
				if (this.mCheckTargetTimer <= 0f)
				{
					this.FindClosestAvatar(out this.mCurrentTarget);
					this.mCheckTargetTimer = 1f;
				}
			}
			EntityManager entityManager = this.mPlayState.EntityManager;
			List<Entity> entities = entityManager.GetEntities(this.mPosition, 5f, true);
			for (int i = 0; i < entities.Count; i++)
			{
				Character character = entities[i] as Character;
				if (character != null && (character.Faction & Factions.FRIENDLY) == Factions.NONE)
				{
					Vector3 vector = character.Position - this.mPosition;
					vector.Y = 0f;
					float num = vector.LengthSquared();
					if (num > 1E-06f)
					{
						vector.Normalize();
						num /= 25f;
						Vector3.Multiply(ref vector, iDeltaTime / num, out vector);
						Vector3.Subtract(ref this.mVelocity, ref vector, out this.mVelocity);
						Matrix identity = Matrix.Identity;
						this.mBody.MoveTo(ref this.mPosition, ref identity);
					}
				}
			}
			entityManager.ReturnEntityList(entities);
			this.FlyTowardsTarget(iDeltaTime);
		}

		// Token: 0x060027EA RID: 10218 RVA: 0x00123AF0 File Offset: 0x00121CF0
		private void FindClosestAvatar(out Avatar oClosestAvatar)
		{
			Player[] players = Game.Instance.Players;
			bool flag = false;
			float num = float.MaxValue;
			Avatar avatar = null;
			int num2 = 0;
			while (num2 < players.Length && !flag)
			{
				if (players[num2].Playing)
				{
					Avatar avatar2 = players[num2].Avatar;
					if (avatar2 != null && !avatar2.Dead && !avatar2.IsEthereal)
					{
						Vector3 vector = avatar2.Position - this.mPosition;
						vector.Y = 0f;
						float num3 = vector.LengthSquared();
						if (num3 < num)
						{
							num = num3;
							avatar = avatar2;
						}
					}
				}
				num2++;
			}
			oClosestAvatar = avatar;
		}

		// Token: 0x060027EB RID: 10219 RVA: 0x00123B8C File Offset: 0x00121D8C
		private void FlyTowardsTarget(float iDeltaTime)
		{
			if (this.mCurrentTarget != null && !this.mCurrentTarget.Dead && !this.mCurrentTarget.IsEthereal)
			{
				this.mToTarget = this.mCurrentTarget.Position;
				Vector3.Subtract(ref this.mToTarget, ref this.mPosition, out this.mToTarget);
			}
			if (this.mPosition.Y <= 0f)
			{
				this.mToTarget.Y = 0f;
				this.mPosition.Y = 0f;
			}
			if (this.mToTarget.LengthSquared() > 1E-06f)
			{
				this.mToTarget.Normalize();
				Vector3 vector;
				Vector3.Multiply(ref this.mToTarget, iDeltaTime * this.mAcceleration * 2f, out vector);
				Vector3 vector2 = this.mVelocity;
				Vector3.Add(ref vector2, ref vector, out vector2);
				float num = vector2.LengthSquared();
				if (num > this.mTerminalVelocity * this.mTerminalVelocity)
				{
					vector2.Normalize();
					Vector3.Multiply(ref vector2, this.mTerminalVelocity, out vector2);
				}
				this.mVelocity = vector2;
				Vector3 vector3;
				Vector3.Multiply(ref this.mVelocity, iDeltaTime, out vector3);
				Vector3.Add(ref this.mPosition, ref vector3, out this.mPosition);
				Matrix identity = Matrix.Identity;
				this.mBody.MoveTo(ref this.mPosition, ref identity);
				EffectManager.Instance.UpdatePositionDirection(ref this.mEffectRef, ref this.mPosition, ref this.mToTarget);
			}
		}

		// Token: 0x17000954 RID: 2388
		// (get) Token: 0x060027EC RID: 10220 RVA: 0x00123CEF File Offset: 0x00121EEF
		public override bool Dead
		{
			get
			{
				return this.mDead;
			}
		}

		// Token: 0x17000955 RID: 2389
		// (get) Token: 0x060027ED RID: 10221 RVA: 0x00123CF7 File Offset: 0x00121EF7
		public override bool Removable
		{
			get
			{
				return this.mRemovable;
			}
		}

		// Token: 0x060027EE RID: 10222 RVA: 0x00123D00 File Offset: 0x00121F00
		public override void Kill()
		{
			this.mDead = (this.mRemovable = true);
		}

		// Token: 0x060027EF RID: 10223 RVA: 0x00123D1D File Offset: 0x00121F1D
		protected override void INetworkUpdate(ref EntityUpdateMessage iMsg)
		{
			base.INetworkUpdate(ref iMsg);
			this.mPosition = iMsg.Position;
			if ((ushort)(iMsg.Features & EntityFeatures.GenericUShort) != 0)
			{
				this.mCurrentTarget = (Entity.GetFromHandle((int)iMsg.GenericUShort) as Avatar);
			}
		}

		// Token: 0x060027F0 RID: 10224 RVA: 0x00123D58 File Offset: 0x00121F58
		protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
		{
			oMsg = default(EntityUpdateMessage);
			oMsg.Features |= EntityFeatures.Position;
			oMsg.Position = this.Position;
			if (this.mCurrentTarget != null)
			{
				oMsg.Features |= EntityFeatures.GenericUShort;
				oMsg.GenericUShort = this.mCurrentTarget.Handle;
			}
		}

		// Token: 0x04002B71 RID: 11121
		private Avatar mCurrentTarget;

		// Token: 0x04002B72 RID: 11122
		private VisualEffectReference mEffectRef;

		// Token: 0x04002B73 RID: 11123
		private static readonly int EFFECT_HIT = "cthulhu_otherworldly_bolt_hit".GetHashCodeCustom();

		// Token: 0x04002B74 RID: 11124
		private static readonly int EFFECT = "cthulhu_otherworldly_bolt".GetHashCodeCustom();

		// Token: 0x04002B75 RID: 11125
		private static readonly int STARSPAWN_HASH = "starspawn".GetHashCodeCustom();

		// Token: 0x04002B76 RID: 11126
		private float mAcceleration;

		// Token: 0x04002B77 RID: 11127
		private float mTerminalVelocity;

		// Token: 0x04002B78 RID: 11128
		private Vector3 mPosition;

		// Token: 0x04002B79 RID: 11129
		private Vector3 mVelocity;

		// Token: 0x04002B7A RID: 11130
		private float mCheckTargetTimer;

		// Token: 0x04002B7B RID: 11131
		private bool mRemovable;

		// Token: 0x04002B7C RID: 11132
		private bool mDone;

		// Token: 0x04002B7D RID: 11133
		private bool mOkToStart;

		// Token: 0x04002B7E RID: 11134
		private static readonly int SOUND_SPAWN = "cthulhu_bolt_spawn".GetHashCodeCustom();

		// Token: 0x04002B7F RID: 11135
		private static readonly int SOUND_IDLE = "cthulhu_bolt_idle".GetHashCodeCustom();

		// Token: 0x04002B80 RID: 11136
		private static readonly int SOUND_HIT = "cthulhu_bolt_hit".GetHashCodeCustom();

		// Token: 0x04002B81 RID: 11137
		private Cue mSoundCueIdle;

		// Token: 0x04002B82 RID: 11138
		private Vector3 mToTarget;
	}
}
