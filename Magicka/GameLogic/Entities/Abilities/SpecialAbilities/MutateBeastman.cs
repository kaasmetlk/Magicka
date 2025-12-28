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
	// Token: 0x020002B4 RID: 692
	public class MutateBeastman : SpecialAbility, IAbilityEffect
	{
		// Token: 0x1700055B RID: 1371
		// (get) Token: 0x060014E2 RID: 5346 RVA: 0x000822D0 File Offset: 0x000804D0
		public static MutateBeastman Instance
		{
			get
			{
				if (MutateBeastman.sSingelton == null)
				{
					lock (MutateBeastman.sSingeltonLock)
					{
						if (MutateBeastman.sSingelton == null)
						{
							MutateBeastman.sSingelton = new MutateBeastman();
						}
					}
				}
				return MutateBeastman.sSingelton;
			}
		}

		// Token: 0x060014E3 RID: 5347 RVA: 0x00082324 File Offset: 0x00080524
		private MutateBeastman() : base(Animations.cast_magick_direct, 0)
		{
		}

		// Token: 0x060014E4 RID: 5348 RVA: 0x0008232F File Offset: 0x0008052F
		public MutateBeastman(Animations iAnimation) : base(iAnimation, 0)
		{
			MutateBeastman.Instance.Initialize(PlayState.RecentPlayState);
		}

		// Token: 0x060014E5 RID: 5349 RVA: 0x00082348 File Offset: 0x00080548
		public void Initialize(PlayState iState)
		{
			MutateBeastman.sTemplate = iState.Content.Load<CharacterTemplate>("Data/Characters/Skeleton_darksoul_elite_noscore");
		}

		// Token: 0x060014E6 RID: 5350 RVA: 0x00082360 File Offset: 0x00080560
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
						if (num2 < MutateBeastman.SPREAD_COS)
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
			AudioManager.Instance.PlayCue(Banks.Misc, MutateBeastman.SOUND_CAST, iOwner.AudioEmitter);
			return true;
		}

		// Token: 0x060014E7 RID: 5351 RVA: 0x000825A4 File Offset: 0x000807A4
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060014E8 RID: 5352 RVA: 0x000825AC File Offset: 0x000807AC
		private IDamageable GetTarget(EntityManager iEntityMan, ISpellCaster iOwner, Vector3 iPos, Vector3 iDir)
		{
			IDamageable result = null;
			float num = float.MaxValue;
			List<Entity> entities = iEntityMan.GetEntities(iPos, 6f, true);
			foreach (Entity entity in entities)
			{
				IDamageable damageable = entity as IDamageable;
				Vector3 vector;
				if (damageable != null && damageable != iOwner && !(damageable is MissileEntity) && damageable.ArcIntersect(out vector, iPos, iDir, 6f, MutateBeastman.SPREAD, 4f))
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

		// Token: 0x060014E9 RID: 5353 RVA: 0x000826A0 File Offset: 0x000808A0
		public void Execute(IDamageable iTarget, ISpellCaster iOwner, PlayState iPlayState)
		{
			Character character = iTarget as Character;
			iTarget.OverKill();
			if (character != null)
			{
				character.BloatKill(Elements.None, iOwner as Entity);
				MutateBeastman.sDelayedSpawns.Add(new MutateBeastman.Info
				{
					TTL = 0.333f,
					Victim = character
				});
				SpellManager.Instance.AddSpellEffect(MutateBeastman.Instance);
			}
		}

		// Token: 0x1700055C RID: 1372
		// (get) Token: 0x060014EA RID: 5354 RVA: 0x00082702 File Offset: 0x00080902
		public bool IsDead
		{
			get
			{
				return MutateBeastman.sDelayedSpawns.Count == 0;
			}
		}

		// Token: 0x060014EB RID: 5355 RVA: 0x00082714 File Offset: 0x00080914
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			for (int i = 0; i < MutateBeastman.sDelayedSpawns.Count; i++)
			{
				MutateBeastman.Info value = MutateBeastman.sDelayedSpawns[i];
				value.TTL -= iDeltaTime;
				if (value.TTL <= 0f)
				{
					Vector3 position = value.Victim.Position;
					if (NetworkManager.Instance.State != NetworkState.Client)
					{
						NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(value.Victim.PlayState);
						instance.Initialize(MutateBeastman.sTemplate, position, 0);
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
							triggerActionMessage.Template = MutateBeastman.sTemplate.ID;
							triggerActionMessage.Id = 0;
							triggerActionMessage.Position = position;
							triggerActionMessage.Direction = direction;
							triggerActionMessage.Bool0 = false;
							NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
						}
					}
					Vector3 forward = Vector3.Forward;
					VisualEffectReference visualEffectReference;
					EffectManager.Instance.StartEffect(MutateBeastman.EFFECT_HIT, ref position, ref forward, out visualEffectReference);
					MutateBeastman.sDelayedSpawns.RemoveAt(i);
					i--;
				}
				else
				{
					MutateBeastman.sDelayedSpawns[i] = value;
				}
			}
		}

		// Token: 0x060014EC RID: 5356 RVA: 0x000828DB File Offset: 0x00080ADB
		public void OnRemove()
		{
		}

		// Token: 0x0400164A RID: 5706
		private const float RANGE = 6f;

		// Token: 0x0400164B RID: 5707
		private static MutateBeastman sSingelton = null;

		// Token: 0x0400164C RID: 5708
		private static volatile object sSingeltonLock = new object();

		// Token: 0x0400164D RID: 5709
		private static readonly float SPREAD = MathHelper.ToRadians(30f);

		// Token: 0x0400164E RID: 5710
		private static readonly float SPREAD_COS = (float)Math.Cos((double)MutateBeastman.SPREAD);

		// Token: 0x0400164F RID: 5711
		private static readonly int EFFECT_HIT = "starspawn_owd_hit".GetHashCodeCustom();

		// Token: 0x04001650 RID: 5712
		private static readonly int SOUND_CAST = "misc_flash".GetHashCodeCustom();

		// Token: 0x04001651 RID: 5713
		private static List<MutateBeastman.Info> sDelayedSpawns = new List<MutateBeastman.Info>(2);

		// Token: 0x04001652 RID: 5714
		private static CharacterTemplate sTemplate;

		// Token: 0x020002B5 RID: 693
		private struct Info
		{
			// Token: 0x04001653 RID: 5715
			public float TTL;

			// Token: 0x04001654 RID: 5716
			public Character Victim;
		}
	}
}
