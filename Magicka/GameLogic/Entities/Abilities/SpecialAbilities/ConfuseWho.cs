using System;
using System.Collections.Generic;
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
	// Token: 0x020005B9 RID: 1465
	public class ConfuseWho : SpecialAbility, IAbilityEffect
	{
		// Token: 0x17000A3D RID: 2621
		// (get) Token: 0x06002BCA RID: 11210 RVA: 0x0015A138 File Offset: 0x00158338
		public static ConfuseWho Instance
		{
			get
			{
				if (ConfuseWho.sSingelton == null)
				{
					lock (ConfuseWho.sSingeltonLock)
					{
						if (ConfuseWho.sSingelton == null)
						{
							ConfuseWho.sSingelton = new ConfuseWho();
						}
					}
				}
				return ConfuseWho.sSingelton;
			}
		}

		// Token: 0x06002BCB RID: 11211 RVA: 0x0015A18C File Offset: 0x0015838C
		private ConfuseWho() : base(Animations.cast_magick_direct, 0)
		{
		}

		// Token: 0x06002BCC RID: 11212 RVA: 0x0015A197 File Offset: 0x00158397
		public ConfuseWho(Animations iAnimation) : base(iAnimation, "#specab_confuewho".GetHashCodeCustom())
		{
			ConfuseWho instance = ConfuseWho.Instance;
		}

		// Token: 0x06002BCD RID: 11213 RVA: 0x0015A1B0 File Offset: 0x001583B0
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			Vector3 position = iOwner.Position;
			Vector3 direction = iOwner.Direction;
			if (direction.Z > 0f)
			{
				Vector3.Negate(ref direction, out direction);
			}
			VisualEffectReference visualEffectReference;
			EffectManager.Instance.StartEffect(ConfuseWho.EFFECT_CAST, ref position, ref direction, out visualEffectReference);
			NetworkState state = NetworkManager.Instance.State;
			if ((state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer))) || (state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer)))
			{
				Character character = null;
				if (iOwner is NonPlayerCharacter)
				{
					character = ((iOwner as NonPlayerCharacter).AI.CurrentTarget as Character);
				}
				if (character == null)
				{
					character = this.GetTarget(iPlayState.EntityManager, iOwner, iOwner.Position, iOwner.Direction);
				}
				if (character != null)
				{
					if (state != NetworkState.Offline)
					{
						TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
						triggerActionMessage.ActionType = TriggerActionType.StarGaze;
						triggerActionMessage.Handle = iOwner.Handle;
						triggerActionMessage.Arg = (int)character.Handle;
						NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
					}
					this.Execute(iOwner, character, iPlayState);
				}
			}
			return true;
		}

		// Token: 0x06002BCE RID: 11214 RVA: 0x0015A2EB File Offset: 0x001584EB
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06002BCF RID: 11215 RVA: 0x0015A2F4 File Offset: 0x001584F4
		private Character GetTarget(EntityManager iEntityMan, ISpellCaster iOwner, Vector3 iPos, Vector3 iDir)
		{
			Character result = null;
			float num = float.MaxValue;
			List<Entity> entities = iEntityMan.GetEntities(iPos, 5f, true);
			foreach (Entity entity in entities)
			{
				Character character = entity as Character;
				Vector3 vector;
				if (character != null && character != iOwner && character.ArcIntersect(out vector, iPos, iDir, 5f, 0.7853982f, 4f))
				{
					float num2;
					Vector3.DistanceSquared(ref iPos, ref vector, out num2);
					if (num2 < num)
					{
						num = num2;
						result = character;
					}
				}
			}
			iEntityMan.ReturnEntityList(entities);
			return result;
		}

		// Token: 0x06002BD0 RID: 11216 RVA: 0x0015A3A0 File Offset: 0x001585A0
		internal void Execute(ISpellCaster iOwner, Character iTarget, PlayState iPlayState)
		{
			if (iOwner == null || iTarget == null)
			{
				return;
			}
			bool flag = true;
			ConfuseWho.VictimInfo victimInfo = default(ConfuseWho.VictimInfo);
			victimInfo.TTL = 10f;
			victimInfo.Victim = iTarget;
			Vector3 position = victimInfo.Victim.Position;
			Vector3 direction = victimInfo.Victim.Direction;
			VisualEffectReference visualEffectReference;
			EffectManager.Instance.StartEffect(ConfuseWho.EFFECT_HIT, ref position, ref direction, out visualEffectReference);
			iTarget.Charm(iOwner as Entity, 10f, 0);
			if (iTarget is Avatar)
			{
				victimInfo.Controller = (iTarget as Avatar).Player.Controller;
				if (victimInfo.Controller != null)
				{
					victimInfo.Controller.Invert(true);
				}
			}
			for (int i = 0; i < ConfuseWho.sVictims.Count; i++)
			{
				if (ConfuseWho.sVictims[i].Victim == iTarget)
				{
					flag = false;
					victimInfo.Effect = ConfuseWho.sVictims[i].Effect;
					ConfuseWho.sVictims[i] = victimInfo;
					break;
				}
			}
			if (flag)
			{
				EffectManager.Instance.StartEffect(ConfuseWho.EFFECT_STATUS, ref position, ref direction, out victimInfo.Effect);
				ConfuseWho.sVictims.Add(victimInfo);
			}
			SpellManager.Instance.AddSpellEffect(ConfuseWho.Instance);
		}

		// Token: 0x17000A3E RID: 2622
		// (get) Token: 0x06002BD1 RID: 11217 RVA: 0x0015A4D7 File Offset: 0x001586D7
		public bool IsDead
		{
			get
			{
				return ConfuseWho.sVictims.Count == 0;
			}
		}

		// Token: 0x06002BD2 RID: 11218 RVA: 0x0015A4E8 File Offset: 0x001586E8
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			for (int i = 0; i < ConfuseWho.sVictims.Count; i++)
			{
				ConfuseWho.VictimInfo value = ConfuseWho.sVictims[i];
				value.TTL -= iDeltaTime;
				if (value.TTL <= 0f || value.Victim.Dead)
				{
					EffectManager.Instance.Stop(ref value.Effect);
					if (value.Victim is NonPlayerCharacter)
					{
						(value.Victim as NonPlayerCharacter).Confuse(value.Victim.Template.Faction);
					}
					else if (value.Victim is Avatar && value.Controller != null)
					{
						value.Controller.Invert(false);
					}
					ConfuseWho.sVictims.RemoveAt(i);
					i--;
				}
				else
				{
					Vector3 position = value.Victim.Position;
					Vector3 direction = value.Victim.Direction;
					EffectManager.Instance.UpdatePositionDirection(ref value.Effect, ref position, ref direction);
					ConfuseWho.sVictims[i] = value;
				}
			}
		}

		// Token: 0x06002BD3 RID: 11219 RVA: 0x0015A5FB File Offset: 0x001587FB
		public void OnRemove()
		{
		}

		// Token: 0x04002F84 RID: 12164
		public const float TIME_TO_LIVE = 10f;

		// Token: 0x04002F85 RID: 12165
		private const float RANGE = 5f;

		// Token: 0x04002F86 RID: 12166
		private static ConfuseWho sSingelton = null;

		// Token: 0x04002F87 RID: 12167
		private static volatile object sSingeltonLock = new object();

		// Token: 0x04002F88 RID: 12168
		private static List<ConfuseWho.VictimInfo> sVictims = new List<ConfuseWho.VictimInfo>(4);

		// Token: 0x04002F89 RID: 12169
		public static readonly int EFFECT_CAST = "elderthing_stargaze".GetHashCodeCustom();

		// Token: 0x04002F8A RID: 12170
		public static readonly int EFFECT_HIT = "elderthing_stargaze_hit".GetHashCodeCustom();

		// Token: 0x04002F8B RID: 12171
		public static readonly int EFFECT_STATUS = "confused".GetHashCodeCustom();

		// Token: 0x04002F8C RID: 12172
		public static readonly int SOUND = "magick_thunderbolt".GetHashCodeCustom();

		// Token: 0x020005BA RID: 1466
		private struct VictimInfo
		{
			// Token: 0x04002F8D RID: 12173
			public float TTL;

			// Token: 0x04002F8E RID: 12174
			public Character Victim;

			// Token: 0x04002F8F RID: 12175
			public VisualEffectReference Effect;

			// Token: 0x04002F90 RID: 12176
			public Controller Controller;
		}
	}
}
