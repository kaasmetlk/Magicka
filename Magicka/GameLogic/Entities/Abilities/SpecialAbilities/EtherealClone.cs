using System;
using Magicka.AI;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.GameStates;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x020003D1 RID: 977
	public class EtherealClone : SpecialAbility
	{
		// Token: 0x06001DEA RID: 7658 RVA: 0x000D2C75 File Offset: 0x000D0E75
		internal static void InitializeCache(PlayState iPlayState)
		{
			iPlayState.Content.Load<CharacterTemplate>("data/characters/dungeons_human_adventurer_clone");
		}

		// Token: 0x1700075A RID: 1882
		// (get) Token: 0x06001DEB RID: 7659 RVA: 0x000D2C88 File Offset: 0x000D0E88
		public static EtherealClone Instance
		{
			get
			{
				if (EtherealClone.sSingelton == null)
				{
					lock (EtherealClone.sSingeltonLock)
					{
						if (EtherealClone.sSingelton == null)
						{
							EtherealClone.sSingelton = new EtherealClone();
						}
					}
				}
				return EtherealClone.sSingelton;
			}
		}

		// Token: 0x06001DEC RID: 7660 RVA: 0x000D2CDC File Offset: 0x000D0EDC
		private EtherealClone() : base(Animations.cast_magick_direct, "#Ent_Dungeons_Slime_Medium".GetHashCodeCustom())
		{
		}

		// Token: 0x06001DED RID: 7661 RVA: 0x000D2CF8 File Offset: 0x000D0EF8
		public EtherealClone(Animations iAnimation) : base(iAnimation, "#Ent_Dungeons_Slime_Medium".GetHashCodeCustom())
		{
		}

		// Token: 0x06001DEE RID: 7662 RVA: 0x000D2D13 File Offset: 0x000D0F13
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new Exception("Ethereal clone cannot be spawned without a parent (iOwner)!");
		}

		// Token: 0x06001DEF RID: 7663 RVA: 0x000D2D20 File Offset: 0x000D0F20
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				return false;
			}
			this.mOwner = iOwner;
			this.mPlayState = iPlayState;
			string name;
			CharacterTemplate characterTemplate;
			int iTemplateId;
			uint iAmount;
			if ((name = (iOwner as Character).Name) != null && name == "dungeons_human_adventurer")
			{
				characterTemplate = CharacterTemplate.GetCachedTemplate(EtherealClone.HUMAN_ADVENTURER);
				iTemplateId = EtherealClone.HUMAN_ADVENTURER;
				iAmount = 1U;
			}
			else
			{
				characterTemplate = null;
				iTemplateId = -1;
				iAmount = 0U;
			}
			if (characterTemplate == null)
			{
				return false;
			}
			this.SpawnClone(characterTemplate, iTemplateId, iAmount);
			return true;
		}

		// Token: 0x06001DF0 RID: 7664 RVA: 0x000D2D9C File Offset: 0x000D0F9C
		protected void SpawnClone(CharacterTemplate template, int iTemplateId, uint iAmount)
		{
			Vector3 position = this.mOwner.Position;
			Vector3 direction = this.mOwner.Direction;
			int num = 0;
			while ((long)num < (long)((ulong)iAmount))
			{
				NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(this.mOwner.PlayState);
				Vector3 value;
				value.X = (float)((MagickaMath.Random.NextDouble() - 0.5) * (double)this.SPREAD);
				value.Y = position.Y;
				value.Z = (float)((MagickaMath.Random.NextDouble() - 0.5) * (double)this.SPREAD);
				Vector3 vector = position + value;
				Vector3 vector2;
				this.mPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref vector, out vector2, MovementProperties.Default);
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
					triggerActionMessage.ActionType = TriggerActionType.SpawnNPC;
					triggerActionMessage.Handle = instance.Handle;
					triggerActionMessage.Template = iTemplateId;
					triggerActionMessage.Id = 0;
					triggerActionMessage.Position = vector2;
					triggerActionMessage.Direction = direction;
					triggerActionMessage.Bool0 = false;
					triggerActionMessage.Point0 = 0;
					triggerActionMessage.Point1 = 0;
					triggerActionMessage.Point2 = (int)this.SpawnAnimation;
					triggerActionMessage.Point3 = (int)this.IdleAnimation;
					NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
				}
				instance.Initialize(template, vector2, 0);
				if (this.mOwner is Character)
				{
					instance.Faction = (this.mOwner as Character).Faction;
				}
				if (this.mOwner.PlayState.Level.CurrentScene.RuleSet != null && this.mOwner.PlayState.Level.CurrentScene.RuleSet is SurvivalRuleset)
				{
					(this.mOwner.PlayState.Level.CurrentScene.RuleSet as SurvivalRuleset).AddedCharacter(instance, true);
				}
				instance.HitPoints = instance.MaxHitPoints;
				instance.AI.SetOrder(Order.Attack, ReactTo.Proximity, Order.Attack, 0, 0, 0, null);
				if (this.SpawnAnimation != Animations.None && this.SpawnAnimation != Animations.idle && this.SpawnAnimation != Animations.idle_agg)
				{
					instance.SpawnAnimation = this.SpawnAnimation;
					instance.ChangeState(RessurectionState.Instance);
				}
				if (this.IdleAnimation != Animations.None)
				{
					instance.SpecialIdleAnimation = this.IdleAnimation;
				}
				this.mOwner.PlayState.EntityManager.AddEntity(instance);
				num++;
			}
		}

		// Token: 0x0400206E RID: 8302
		private readonly int SPREAD = 12;

		// Token: 0x0400206F RID: 8303
		protected static readonly int HUMAN_ADVENTURER = "dungeons_human_adventurer_clone".GetHashCodeCustom();

		// Token: 0x04002070 RID: 8304
		protected ISpellCaster mOwner;

		// Token: 0x04002071 RID: 8305
		protected PlayState mPlayState;

		// Token: 0x04002072 RID: 8306
		public Animations IdleAnimation;

		// Token: 0x04002073 RID: 8307
		public Animations SpawnAnimation;

		// Token: 0x04002074 RID: 8308
		private static EtherealClone sSingelton;

		// Token: 0x04002075 RID: 8309
		private static volatile object sSingeltonLock = new object();
	}
}
