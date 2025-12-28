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
	// Token: 0x02000180 RID: 384
	public class StarGaze : SpecialAbility, IAbilityEffect
	{
		// Token: 0x170002CE RID: 718
		// (get) Token: 0x06000BB4 RID: 2996 RVA: 0x000461BC File Offset: 0x000443BC
		public static StarGaze Instance
		{
			get
			{
				if (StarGaze.sSingelton == null)
				{
					lock (StarGaze.sSingeltonLock)
					{
						if (StarGaze.sSingelton == null)
						{
							StarGaze.sSingelton = new StarGaze();
						}
					}
				}
				return StarGaze.sSingelton;
			}
		}

		// Token: 0x06000BB5 RID: 2997 RVA: 0x00046210 File Offset: 0x00044410
		private StarGaze() : base(Animations.cast_magick_direct, 0)
		{
		}

		// Token: 0x06000BB6 RID: 2998 RVA: 0x0004621B File Offset: 0x0004441B
		public StarGaze(Animations iAnimation) : base(iAnimation, 0)
		{
		}

		// Token: 0x06000BB7 RID: 2999 RVA: 0x00046228 File Offset: 0x00044428
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
			EffectManager.Instance.StartEffect(StarGaze.EFFECT_CAST, ref position, ref direction, out visualEffectReference);
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

		// Token: 0x06000BB8 RID: 3000 RVA: 0x00046363 File Offset: 0x00044563
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000BB9 RID: 3001 RVA: 0x0004636C File Offset: 0x0004456C
		private Character GetTarget(EntityManager iEntityMan, ISpellCaster iOwner, Vector3 iPos, Vector3 iDir)
		{
			Character result = null;
			float num = float.MaxValue;
			List<Entity> entities = iEntityMan.GetEntities(iPos, 20f, true);
			foreach (Entity entity in entities)
			{
				Character character = entity as Character;
				Vector3 vector;
				if (character != null && character != iOwner && character.ArcIntersect(out vector, iPos, iDir, 20f, 0.7853982f, 4f))
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

		// Token: 0x06000BBA RID: 3002 RVA: 0x00046418 File Offset: 0x00044618
		internal void Execute(ISpellCaster iOwner, Character iTarget, PlayState iPlayState)
		{
			if (iOwner == null || iTarget == null)
			{
				return;
			}
			bool flag = true;
			StarGaze.VictimInfo victimInfo = default(StarGaze.VictimInfo);
			victimInfo.TTL = 10f;
			victimInfo.Victim = iTarget;
			Vector3 position = victimInfo.Victim.Position;
			Vector3 direction = victimInfo.Victim.Direction;
			VisualEffectReference visualEffectReference;
			EffectManager.Instance.StartEffect(StarGaze.EFFECT_HIT, ref position, ref direction, out visualEffectReference);
			iTarget.Charm(iOwner as Entity, 10f, 0);
			if (iTarget is Avatar)
			{
				victimInfo.Controller = (iTarget as Avatar).Player.Controller;
				if (victimInfo.Controller != null)
				{
					victimInfo.Controller.Invert(true);
				}
			}
			else if (iTarget is NonPlayerCharacter)
			{
				(iTarget as NonPlayerCharacter).Confuse(Factions.NONE);
			}
			for (int i = 0; i < StarGaze.sVictims.Count; i++)
			{
				if (StarGaze.sVictims[i].Victim == iTarget)
				{
					flag = false;
					victimInfo.Effect = StarGaze.sVictims[i].Effect;
					StarGaze.sVictims[i] = victimInfo;
					break;
				}
			}
			if (flag)
			{
				EffectManager.Instance.StartEffect(StarGaze.EFFECT_STATUS, ref position, ref direction, out victimInfo.Effect);
				StarGaze.sVictims.Add(victimInfo);
			}
			SpellManager.Instance.AddSpellEffect(StarGaze.Instance);
		}

		// Token: 0x170002CF RID: 719
		// (get) Token: 0x06000BBB RID: 3003 RVA: 0x00046565 File Offset: 0x00044765
		public bool IsDead
		{
			get
			{
				return StarGaze.sVictims.Count == 0;
			}
		}

		// Token: 0x06000BBC RID: 3004 RVA: 0x00046574 File Offset: 0x00044774
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			for (int i = 0; i < StarGaze.sVictims.Count; i++)
			{
				StarGaze.VictimInfo value = StarGaze.sVictims[i];
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
					StarGaze.sVictims.RemoveAt(i);
					i--;
				}
				else
				{
					Vector3 position = value.Victim.Position;
					Vector3 direction = value.Victim.Direction;
					EffectManager.Instance.UpdatePositionDirection(ref value.Effect, ref position, ref direction);
					StarGaze.sVictims[i] = value;
				}
			}
		}

		// Token: 0x06000BBD RID: 3005 RVA: 0x00046687 File Offset: 0x00044887
		public void OnRemove()
		{
		}

		// Token: 0x04000AB3 RID: 2739
		public const float TIME_TO_LIVE = 10f;

		// Token: 0x04000AB4 RID: 2740
		private const float RANGE = 20f;

		// Token: 0x04000AB5 RID: 2741
		private static StarGaze sSingelton;

		// Token: 0x04000AB6 RID: 2742
		private static volatile object sSingeltonLock = new object();

		// Token: 0x04000AB7 RID: 2743
		private static List<StarGaze.VictimInfo> sVictims = new List<StarGaze.VictimInfo>(4);

		// Token: 0x04000AB8 RID: 2744
		public static readonly int EFFECT_CAST = "elderthing_stargaze".GetHashCodeCustom();

		// Token: 0x04000AB9 RID: 2745
		public static readonly int EFFECT_HIT = "elderthing_stargaze_hit".GetHashCodeCustom();

		// Token: 0x04000ABA RID: 2746
		public static readonly int EFFECT_STATUS = "confused".GetHashCodeCustom();

		// Token: 0x02000181 RID: 385
		private struct VictimInfo
		{
			// Token: 0x04000ABB RID: 2747
			public float TTL;

			// Token: 0x04000ABC RID: 2748
			public Character Victim;

			// Token: 0x04000ABD RID: 2749
			public VisualEffectReference Effect;

			// Token: 0x04000ABE RID: 2750
			public Controller Controller;
		}
	}
}
