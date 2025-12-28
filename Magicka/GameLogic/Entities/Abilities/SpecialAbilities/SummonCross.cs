using System;
using System.Collections.Generic;
using Magicka.AI;
using Magicka.GameLogic.Entities.Buffs;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x0200058C RID: 1420
	public class SummonCross : SpecialAbility, IAbilityEffect
	{
		// Token: 0x06002A63 RID: 10851 RVA: 0x0014D154 File Offset: 0x0014B354
		public static SummonCross GetInstance()
		{
			if (SummonCross.sCache.Count > 0)
			{
				SummonCross result = SummonCross.sCache[SummonCross.sCache.Count - 1];
				SummonCross.sCache.RemoveAt(SummonCross.sCache.Count - 1);
				return result;
			}
			return new SummonCross();
		}

		// Token: 0x06002A64 RID: 10852 RVA: 0x0014D1A4 File Offset: 0x0014B3A4
		public static void InitializeCache(int iNr, PlayState iPlayState)
		{
			SummonCross.sTemplate = iPlayState.Content.Load<CharacterTemplate>("data/characters/cross");
			SummonCross.sCache = new List<SummonCross>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				SummonCross.sCache.Add(new SummonCross());
			}
		}

		// Token: 0x06002A65 RID: 10853 RVA: 0x0014D1EC File Offset: 0x0014B3EC
		private SummonCross() : base(Animations.cast_magick_global, "#magick_cross".GetHashCodeCustom())
		{
		}

		// Token: 0x06002A66 RID: 10854 RVA: 0x0014D200 File Offset: 0x0014B400
		public SummonCross(Animations iAnimation) : base(iAnimation, "#magick_cross".GetHashCodeCustom())
		{
		}

		// Token: 0x06002A67 RID: 10855 RVA: 0x0014D213 File Offset: 0x0014B413
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			this.mPlayState = iPlayState;
			this.mPosition = iPosition;
			return this.Execute();
		}

		// Token: 0x06002A68 RID: 10856 RVA: 0x0014D229 File Offset: 0x0014B429
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			this.mPlayState = iPlayState;
			this.mOwner = iOwner;
			this.mPosition = this.mOwner.Position;
			return this.Execute();
		}

		// Token: 0x06002A69 RID: 10857 RVA: 0x0014D250 File Offset: 0x0014B450
		private bool Execute()
		{
			this.mCross = NonPlayerCharacter.GetInstance(this.mPlayState);
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				if (this.mCross == null)
				{
					return false;
				}
				Vector3 vector = this.mPosition;
				vector.Y += 3f;
				this.mPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref vector, out this.mPosition, MovementProperties.Default);
				this.mCross.Initialize(SummonCross.sTemplate, this.mPosition, 0);
				if (this.mOwner is Character)
				{
					this.mCross.Summoned(this.mOwner as Character);
				}
				Agent ai = this.mCross.AI;
				ai.SetOrder(Order.None, ReactTo.None, Order.None, 0, 0, 0, null);
				ai.AlertRadius = 0f;
				Matrix orientation;
				Matrix.CreateRotationY((float)SpecialAbility.RANDOM.NextDouble() * 6.2831855f, out orientation);
				this.mCross.Body.Orientation = orientation;
				this.mPlayState.EntityManager.AddEntity(this.mCross);
				ai.Owner.SpawnAnimation = Animations.spawn;
				ai.Owner.ChangeState(RessurectionState.Instance);
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
					triggerActionMessage.ActionType = TriggerActionType.SpawnNPC;
					triggerActionMessage.Handle = this.mCross.Handle;
					triggerActionMessage.Template = this.mCross.Type;
					triggerActionMessage.Id = this.mCross.UniqueID;
					triggerActionMessage.Position = this.mCross.Position;
					triggerActionMessage.Direction = orientation.Forward;
					if (this.mOwner != null)
					{
						triggerActionMessage.Scene = (int)this.mOwner.Handle;
					}
					triggerActionMessage.Bool0 = false;
					triggerActionMessage.Point1 = 170;
					triggerActionMessage.Point2 = 170;
					NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
				}
			}
			AuraBuff iAura = new AuraBuff(new BuffStorage(new BuffResistance(new Resistance
			{
				ResistanceAgainst = Elements.All,
				Multiplier = 0f
			}), VisualCategory.Defensive, Spell.SHIELDCOLOR));
			AuraStorage aura = new AuraStorage(iAura, AuraTarget.Self, AuraType.Buff, SummonCross.EFFECT_BUBBLE, 100000000f, 0f, VisualCategory.Special, Spell.SHIELDCOLOR, null, Factions.NONE);
			(this.mOwner as Character).AddBubbleShield(aura);
			SpellManager.Instance.AddSpellEffect(this);
			return true;
		}

		// Token: 0x170009F5 RID: 2549
		// (get) Token: 0x06002A6A RID: 10858 RVA: 0x0014D4BE File Offset: 0x0014B6BE
		public bool IsDead
		{
			get
			{
				return this.mCross.Dead;
			}
		}

		// Token: 0x06002A6B RID: 10859 RVA: 0x0014D4CB File Offset: 0x0014B6CB
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
		}

		// Token: 0x06002A6C RID: 10860 RVA: 0x0014D4CD File Offset: 0x0014B6CD
		public void OnRemove()
		{
		}

		// Token: 0x04002DC3 RID: 11715
		public static readonly int EFFECT_BUBBLE = "paladin_bubble".GetHashCodeCustom();

		// Token: 0x04002DC4 RID: 11716
		private static List<SummonCross> sCache;

		// Token: 0x04002DC5 RID: 11717
		private PlayState mPlayState;

		// Token: 0x04002DC6 RID: 11718
		private static CharacterTemplate sTemplate;

		// Token: 0x04002DC7 RID: 11719
		private NonPlayerCharacter mCross;

		// Token: 0x04002DC8 RID: 11720
		private ISpellCaster mOwner;

		// Token: 0x04002DC9 RID: 11721
		private Vector3 mPosition;
	}
}
