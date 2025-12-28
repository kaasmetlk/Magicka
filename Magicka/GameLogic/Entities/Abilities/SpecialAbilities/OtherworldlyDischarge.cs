using System;
using System.Collections.Generic;
using JigLibX.Geometry;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000670 RID: 1648
	public class OtherworldlyDischarge : SpecialAbility, IAbilityEffect
	{
		// Token: 0x17000BA1 RID: 2977
		// (get) Token: 0x060031C1 RID: 12737 RVA: 0x001991A8 File Offset: 0x001973A8
		public static OtherworldlyDischarge Instance
		{
			get
			{
				if (OtherworldlyDischarge.sSingelton == null)
				{
					lock (OtherworldlyDischarge.sSingeltonLock)
					{
						if (OtherworldlyDischarge.sSingelton == null)
						{
							OtherworldlyDischarge.sSingelton = new OtherworldlyDischarge();
						}
					}
				}
				return OtherworldlyDischarge.sSingelton;
			}
		}

		// Token: 0x060031C2 RID: 12738 RVA: 0x001991FC File Offset: 0x001973FC
		private OtherworldlyDischarge() : base(Animations.cast_magick_direct, 0)
		{
		}

		// Token: 0x060031C3 RID: 12739 RVA: 0x00199207 File Offset: 0x00197407
		public OtherworldlyDischarge(Animations iAnimation) : base(iAnimation, 0)
		{
			OtherworldlyDischarge.Instance.Initialize(PlayState.RecentPlayState);
		}

		// Token: 0x060031C4 RID: 12740 RVA: 0x00199220 File Offset: 0x00197420
		public void Initialize(PlayState iState)
		{
			OtherworldlyDischarge.sTemplate = iState.Content.Load<CharacterTemplate>("Data/Characters/Cultist");
		}

		// Token: 0x060031C5 RID: 12741 RVA: 0x00199238 File Offset: 0x00197438
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			NetworkState state = NetworkManager.Instance.State;
			if ((state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer))) || (state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer)))
			{
				Vector3 position = iOwner.Position;
				Vector3 direction = iOwner.Direction;
				IDamageable damageable = null;
				if (iOwner is NonPlayerCharacter && (iOwner as NonPlayerCharacter).AI.CurrentTarget != null)
				{
					damageable = (iOwner as NonPlayerCharacter).AI.CurrentTarget;
					Vector3 position2 = damageable.Position;
					float num;
					Vector3.DistanceSquared(ref position, ref position2, out num);
					if (num > 36f)
					{
						damageable = null;
					}
					else if (num > 1E-06f)
					{
						Vector3 vector;
						Vector3.Subtract(ref position2, ref position, out vector);
						Vector3.Divide(ref vector, (float)Math.Sqrt((double)num), out vector);
						float num2;
						Vector3.Dot(ref vector, ref direction, out num2);
						if (num2 < OtherworldlyDischarge.SPREAD_COS)
						{
							damageable = null;
						}
						else
						{
							Segment iSeg;
							iSeg.Origin = position;
							iSeg.Delta = damageable.Position;
							Vector3.Subtract(ref iSeg.Delta, ref position, out iSeg.Delta);
							List<Shield> shields = iPlayState.EntityManager.Shields;
							foreach (Shield shield in shields)
							{
								Vector3 vector2;
								if (shield.SegmentIntersect(out vector2, iSeg, 0.5f))
								{
									damageable = null;
									break;
								}
							}
						}
					}
				}
				if (damageable == null)
				{
					damageable = this.GetTarget(iPlayState.EntityManager, iOwner, position, direction);
				}
				if (damageable != null)
				{
					Character character = damageable as Character;
					if (character != null && character.IsSelfShielded && !character.IsSolidSelfShielded)
					{
						character.RemoveSelfShield();
						damageable = null;
					}
				}
				if (damageable != null)
				{
					if (state != NetworkState.Offline)
					{
						TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
						triggerActionMessage.ActionType = TriggerActionType.OtherworldlyDischarge;
						triggerActionMessage.Handle = iOwner.Handle;
						triggerActionMessage.Arg = (int)damageable.Handle;
						NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
					}
					this.Execute(damageable, iOwner, iPlayState);
				}
			}
			AudioManager.Instance.PlayCue(Banks.Misc, OtherworldlyDischarge.SOUND_CAST, iOwner.AudioEmitter);
			return true;
		}

		// Token: 0x060031C6 RID: 12742 RVA: 0x0019947C File Offset: 0x0019767C
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060031C7 RID: 12743 RVA: 0x00199484 File Offset: 0x00197684
		private IDamageable GetTarget(EntityManager iEntityMan, ISpellCaster iOwner, Vector3 iPos, Vector3 iDir)
		{
			IDamageable result = null;
			float num = float.MaxValue;
			List<Entity> entities = iEntityMan.GetEntities(iPos, 6f, true);
			foreach (Entity entity in entities)
			{
				IDamageable damageable = entity as IDamageable;
				Vector3 vector;
				if (damageable != null && damageable != iOwner && !(damageable is MissileEntity) && damageable.ArcIntersect(out vector, iPos, iDir, 6f, OtherworldlyDischarge.SPREAD, 4f))
				{
					float num2 = -1f;
					if (damageable is Character)
					{
						Character character = damageable as Character;
						if (character.IsSelfShielded && !character.IsSolidSelfShielded)
						{
							num2 = 6f;
						}
					}
					if (num2 < 0f)
					{
						Vector3.DistanceSquared(ref iPos, ref vector, out num2);
					}
					if (num2 < num)
					{
						num = num2;
						result = damageable;
					}
				}
			}
			iEntityMan.ReturnEntityList(entities);
			return result;
		}

		// Token: 0x060031C8 RID: 12744 RVA: 0x00199578 File Offset: 0x00197778
		public void Execute(IDamageable iTarget, ISpellCaster iOwner, PlayState iPlayState)
		{
			Character character = iTarget as Character;
			iTarget.OverKill();
			if (character != null)
			{
				character.BloatKill(Elements.None, iOwner as Entity);
				OtherworldlyDischarge.sDelayedSpawns.Add(new OtherworldlyDischarge.Info
				{
					TTL = 0.333f,
					Victim = character
				});
				SpellManager.Instance.AddSpellEffect(OtherworldlyDischarge.Instance);
			}
		}

		// Token: 0x17000BA2 RID: 2978
		// (get) Token: 0x060031C9 RID: 12745 RVA: 0x001995DA File Offset: 0x001977DA
		public bool IsDead
		{
			get
			{
				return OtherworldlyDischarge.sDelayedSpawns.Count == 0;
			}
		}

		// Token: 0x060031CA RID: 12746 RVA: 0x001995EC File Offset: 0x001977EC
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			for (int i = 0; i < OtherworldlyDischarge.sDelayedSpawns.Count; i++)
			{
				OtherworldlyDischarge.Info value = OtherworldlyDischarge.sDelayedSpawns[i];
				value.TTL -= iDeltaTime;
				if (value.TTL <= 0f)
				{
					Vector3 position = value.Victim.Position;
					if (NetworkManager.Instance.State != NetworkState.Client)
					{
						NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(value.Victim.PlayState);
						instance.Initialize(OtherworldlyDischarge.sTemplate, position, 0);
						Vector3 direction = value.Victim.Direction;
						Vector3 vector = default(Vector3);
						vector.Y = 1f;
						Vector3 right;
						Vector3.Cross(ref direction, ref vector, out right);
						Matrix orientation = default(Matrix);
						orientation.M44 = 1f;
						orientation.M22 = 1f;
						orientation.Forward = direction;
						orientation.Right = right;
						instance.CharacterBody.Orientation = orientation;
						instance.CharacterBody.DesiredDirection = direction;
						value.Victim.PlayState.EntityManager.AddEntity(instance);
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
							triggerActionMessage.ActionType = TriggerActionType.SpawnNPC;
							triggerActionMessage.Handle = instance.Handle;
							triggerActionMessage.Template = OtherworldlyDischarge.sTemplate.ID;
							triggerActionMessage.Id = 0;
							triggerActionMessage.Position = position;
							triggerActionMessage.Direction = direction;
							triggerActionMessage.Bool0 = false;
							NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
						}
					}
					Vector3 forward = Vector3.Forward;
					VisualEffectReference visualEffectReference;
					EffectManager.Instance.StartEffect(OtherworldlyDischarge.EFFECT_HIT, ref position, ref forward, out visualEffectReference);
					OtherworldlyDischarge.sDelayedSpawns.RemoveAt(i);
					i--;
				}
				else
				{
					OtherworldlyDischarge.sDelayedSpawns[i] = value;
				}
			}
		}

		// Token: 0x060031CB RID: 12747 RVA: 0x001997B3 File Offset: 0x001979B3
		public void OnRemove()
		{
		}

		// Token: 0x0400362D RID: 13869
		private const float RANGE = 6f;

		// Token: 0x0400362E RID: 13870
		private static OtherworldlyDischarge sSingelton;

		// Token: 0x0400362F RID: 13871
		private static volatile object sSingeltonLock = new object();

		// Token: 0x04003630 RID: 13872
		private static readonly float SPREAD = MathHelper.ToRadians(30f);

		// Token: 0x04003631 RID: 13873
		private static readonly float SPREAD_COS = (float)Math.Cos((double)OtherworldlyDischarge.SPREAD);

		// Token: 0x04003632 RID: 13874
		private static readonly int EFFECT_HIT = "starspawn_owd_hit".GetHashCodeCustom();

		// Token: 0x04003633 RID: 13875
		private static readonly int SOUND_CAST = "misc_flash".GetHashCodeCustom();

		// Token: 0x04003634 RID: 13876
		private static List<OtherworldlyDischarge.Info> sDelayedSpawns = new List<OtherworldlyDischarge.Info>(2);

		// Token: 0x04003635 RID: 13877
		private static CharacterTemplate sTemplate;

		// Token: 0x02000671 RID: 1649
		private struct Info
		{
			// Token: 0x04003636 RID: 13878
			public float TTL;

			// Token: 0x04003637 RID: 13879
			public Character Victim;
		}
	}
}
