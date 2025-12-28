using System;
using System.Collections.Generic;
using JigLibX.Geometry;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x020005B6 RID: 1462
	public class Starfall : SpecialAbility, IAbilityEffect
	{
		// Token: 0x17000A3B RID: 2619
		// (get) Token: 0x06002BBC RID: 11196 RVA: 0x001598D8 File Offset: 0x00157AD8
		public static Starfall Instance
		{
			get
			{
				if (Starfall.sSingelton == null)
				{
					lock (Starfall.sSingeltonLock)
					{
						if (Starfall.sSingelton == null)
						{
							Starfall.sSingelton = new Starfall(Animations.cast_magick_direct);
						}
					}
				}
				return Starfall.sSingelton;
			}
		}

		// Token: 0x06002BBD RID: 11197 RVA: 0x0015992C File Offset: 0x00157B2C
		static Starfall()
		{
			Starfall.sDamage.A = new Damage(AttackProperties.Damage, Elements.Arcane, 12000f, 1f);
			Starfall.sDamage.B = new Damage(AttackProperties.Damage | AttackProperties.ArmourPiercing, Elements.Earth, 6000f, 1f);
			Starfall.sAudioEmitter = new AudioEmitter();
			Starfall.sAudioEmitter.Forward = Vector3.Forward;
			Starfall.sAudioEmitter.Up = Vector3.Up;
		}

		// Token: 0x06002BBE RID: 11198 RVA: 0x001599E7 File Offset: 0x00157BE7
		public Starfall(Animations iAnimation) : base(iAnimation, 0)
		{
		}

		// Token: 0x06002BBF RID: 11199 RVA: 0x001599F4 File Offset: 0x00157BF4
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			NetworkState state = NetworkManager.Instance.State;
			if ((state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer))) || (state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer)))
			{
				Vector3 vector;
				if (iOwner is NonPlayerCharacter && (iOwner as NonPlayerCharacter).AI.CurrentTarget != null)
				{
					vector = (iOwner as NonPlayerCharacter).AI.CurrentTarget.Position;
				}
				else
				{
					vector = iOwner.Position;
					Vector3 direction = iOwner.Direction;
					Vector3.Multiply(ref direction, 10f, out direction);
					Vector3.Add(ref direction, ref vector, out vector);
					vector = this.GetTarget(vector, iPlayState);
				}
				if (state != NetworkState.Offline)
				{
					TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
					triggerActionMessage.ActionType = TriggerActionType.Starfall;
					triggerActionMessage.Position = vector;
					triggerActionMessage.Handle = iOwner.Handle;
					NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
				}
				this.Execute(iOwner, iPlayState, vector, true);
			}
			return true;
		}

		// Token: 0x06002BC0 RID: 11200 RVA: 0x00159B0C File Offset: 0x00157D0C
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			base.Execute(iPosition, iPlayState);
			NetworkState state = NetworkManager.Instance.State;
			if (state != NetworkState.Client)
			{
				iPosition = this.GetTarget(iPosition, iPlayState);
				if (state != NetworkState.Offline)
				{
					TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
					triggerActionMessage.ActionType = TriggerActionType.Starfall;
					triggerActionMessage.Position = iPosition;
					triggerActionMessage.Handle = ushort.MaxValue;
					NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
				}
				this.Execute(null, iPlayState, iPosition, true);
			}
			return true;
		}

		// Token: 0x06002BC1 RID: 11201 RVA: 0x00159B80 File Offset: 0x00157D80
		private Vector3 GetTarget(Vector3 iSource, PlayState iPlayState)
		{
			Vector3 result = iSource;
			float num = float.MaxValue;
			List<Entity> entities = iPlayState.EntityManager.GetEntities(iSource, 8f, false);
			foreach (Entity entity in entities)
			{
				if (entity is IDamageable)
				{
					Vector3 position = entity.Position;
					Vector3 vector;
					Vector3.Subtract(ref position, ref iSource, out vector);
					vector.Y = 0f;
					float num2 = vector.LengthSquared();
					if (num2 < num)
					{
						num = num2;
						result = position;
					}
				}
			}
			iPlayState.EntityManager.ReturnEntityList(entities);
			return result;
		}

		// Token: 0x06002BC2 RID: 11202 RVA: 0x00159C2C File Offset: 0x00157E2C
		public bool Execute(ISpellCaster iOwner, PlayState iPlayState, Vector3 iPosition, bool iDeadDamage)
		{
			Starfall.sPlayState = iPlayState;
			if (iDeadDamage)
			{
				Starfall.sQueue.Add(new Starfall.Info
				{
					CastDelay = 1f,
					DamageDelay = 0.5f,
					DealDamage = iDeadDamage,
					Position = iPosition,
					Owner = (iOwner as Entity)
				});
				SpellManager.Instance.AddSpellEffect(Starfall.Instance);
			}
			return true;
		}

		// Token: 0x17000A3C RID: 2620
		// (get) Token: 0x06002BC3 RID: 11203 RVA: 0x00159C9E File Offset: 0x00157E9E
		public bool IsDead
		{
			get
			{
				return Starfall.sQueue.Count == 0;
			}
		}

		// Token: 0x06002BC4 RID: 11204 RVA: 0x00159CB0 File Offset: 0x00157EB0
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			for (int i = 0; i < Starfall.sQueue.Count; i++)
			{
				Starfall.Info value = Starfall.sQueue[i];
				if (value.CastDelay > 0f)
				{
					value.CastDelay -= iDeltaTime;
					if (value.CastDelay <= 0f)
					{
						Segment iSeg;
						iSeg.Delta = default(Vector3);
						iSeg.Delta.X = -20f;
						iSeg.Delta.Y = -20f;
						Vector3.Subtract(ref value.Position, ref iSeg.Delta, out iSeg.Origin);
						float scaleFactor;
						Vector3 position;
						Vector3 vector;
						AnimatedLevelPart animatedLevelPart;
						int num;
						if (Starfall.sPlayState.Level.CurrentScene.LevelModel.SegmentIntersect(out scaleFactor, out position, out vector, out animatedLevelPart, out num, iSeg, true))
						{
							value.Position = position;
							Vector3.Multiply(ref iSeg.Delta, scaleFactor, out iSeg.Delta);
						}
						List<Shield> shields = Starfall.sPlayState.EntityManager.Shields;
						foreach (Shield shield in shields)
						{
							if (shield.SegmentIntersect(out scaleFactor, out position, iSeg, 0.5f))
							{
								value.Position = position;
								Vector3.Multiply(ref iSeg.Delta, scaleFactor, out iSeg.Delta);
							}
						}
						Vector3 vector2 = default(Vector3);
						vector2.X = -1f;
						VisualEffectReference visualEffectReference;
						EffectManager.Instance.StartEffect(Starfall.EFFECT, ref value.Position, ref vector2, out visualEffectReference);
						Starfall.sAudioEmitter.Position = value.Position;
						AudioManager.Instance.PlayCue(Banks.Spells, Starfall.SOUND, Starfall.sAudioEmitter);
					}
					Starfall.sQueue[i] = value;
				}
				else if (value.DealDamage)
				{
					value.DamageDelay -= iDeltaTime;
					if (value.DamageDelay <= 0f)
					{
						Helper.CircleDamage(Starfall.sPlayState, value.Owner, Starfall.sPlayState.PlayTime, null, ref value.Position, Starfall.RANGE, ref Starfall.sDamage);
						AudioManager.Instance.PlayCue(Banks.Spells, Starfall.TEMPBLASTSOUND);
						Starfall.sQueue.RemoveAt(i);
						i--;
					}
					else
					{
						Starfall.sQueue[i] = value;
					}
				}
				else
				{
					Starfall.sQueue.RemoveAt(i);
					i--;
				}
			}
		}

		// Token: 0x06002BC5 RID: 11205 RVA: 0x00159F1C File Offset: 0x0015811C
		public void OnRemove()
		{
		}

		// Token: 0x04002F71 RID: 12145
		private const float CAST_DELAY = 1f;

		// Token: 0x04002F72 RID: 12146
		private const float DAMAGE_DELAY = 0.5f;

		// Token: 0x04002F73 RID: 12147
		private static Starfall sSingelton;

		// Token: 0x04002F74 RID: 12148
		private static volatile object sSingeltonLock = new object();

		// Token: 0x04002F75 RID: 12149
		public static readonly int TEMPBLASTSOUND = "magick_meteor_blast".GetHashCodeCustom();

		// Token: 0x04002F76 RID: 12150
		public static readonly int SOUND = "magick_meteor_preblast".GetHashCodeCustom();

		// Token: 0x04002F77 RID: 12151
		public static readonly int EFFECT = "starspawn_starfall".GetHashCodeCustom();

		// Token: 0x04002F78 RID: 12152
		public static readonly float RANGE = 3f;

		// Token: 0x04002F79 RID: 12153
		private static DamageCollection5 sDamage;

		// Token: 0x04002F7A RID: 12154
		private static AudioEmitter sAudioEmitter;

		// Token: 0x04002F7B RID: 12155
		private static PlayState sPlayState;

		// Token: 0x04002F7C RID: 12156
		private static List<Starfall.Info> sQueue = new List<Starfall.Info>(1);

		// Token: 0x020005B7 RID: 1463
		private struct Info
		{
			// Token: 0x04002F7D RID: 12157
			public float CastDelay;

			// Token: 0x04002F7E RID: 12158
			public float DamageDelay;

			// Token: 0x04002F7F RID: 12159
			public bool DealDamage;

			// Token: 0x04002F80 RID: 12160
			public Vector3 Position;

			// Token: 0x04002F81 RID: 12161
			public Entity Owner;
		}
	}
}
