using System;
using System.Collections.Generic;
using Magicka.AI;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.GameStates;
using Magicka.Levels;
using Magicka.Misc;
using Magicka.Network;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000182 RID: 386
	public class SpawnSlime : SpecialAbility
	{
		// Token: 0x06000BBF RID: 3007 RVA: 0x000466E0 File Offset: 0x000448E0
		internal static void InitializeCache(PlayState iPlayState)
		{
			iPlayState.Content.Load<CharacterTemplate>("data/characters/dungeons_slimecube_poison");
			iPlayState.Content.Load<CharacterTemplate>("data/characters/dungeons_slimecube_poison_small");
			iPlayState.Content.Load<CharacterTemplate>("data/characters/dungeons_slimecube_arcane");
			iPlayState.Content.Load<CharacterTemplate>("data/characters/dungeons_slimecube_arcane_small");
		}

		// Token: 0x170002D0 RID: 720
		// (get) Token: 0x06000BC0 RID: 3008 RVA: 0x00046734 File Offset: 0x00044934
		public static SpawnSlime Instance
		{
			get
			{
				if (SpawnSlime.sSingelton == null)
				{
					lock (SpawnSlime.sSingeltonLock)
					{
						if (SpawnSlime.sSingelton == null)
						{
							SpawnSlime.sSingelton = new SpawnSlime();
						}
					}
				}
				return SpawnSlime.sSingelton;
			}
		}

		// Token: 0x06000BC1 RID: 3009 RVA: 0x00046788 File Offset: 0x00044988
		private SpawnSlime() : base(Animations.cast_magick_direct, "#Ent_Dungeons_Slime_Medium".GetHashCodeCustom())
		{
		}

		// Token: 0x06000BC2 RID: 3010 RVA: 0x000467A3 File Offset: 0x000449A3
		public SpawnSlime(Animations iAnimation) : base(iAnimation, "#Ent_Dungeons_Slime_Medium".GetHashCodeCustom())
		{
		}

		// Token: 0x06000BC3 RID: 3011 RVA: 0x000467C0 File Offset: 0x000449C0
		private SpawnSlime(Elements[] elements) : base(Animations.cast_magick_direct, "#Ent_Dungeons_Slime_Medium".GetHashCodeCustom())
		{
			for (int i = 0; i < elements.Length; i++)
			{
				this.mElements |= elements[i];
			}
		}

		// Token: 0x06000BC4 RID: 3012 RVA: 0x00046804 File Offset: 0x00044A04
		public SpawnSlime(Animations iAnimation, Elements[] elements) : base(iAnimation, "#Ent_Dungeons_Slime_Medium".GetHashCodeCustom())
		{
			for (int i = 0; i < elements.Length; i++)
			{
				this.mElements |= elements[i];
			}
		}

		// Token: 0x06000BC5 RID: 3013 RVA: 0x00046847 File Offset: 0x00044A47
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new Exception("New slime cannot be spawned without a parent (iOwner)!");
		}

		// Token: 0x06000BC6 RID: 3014 RVA: 0x00046853 File Offset: 0x00044A53
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			return this.Execute(iOwner, this.mElements, iPlayState);
		}

		// Token: 0x06000BC7 RID: 3015 RVA: 0x00046864 File Offset: 0x00044A64
		public override bool Execute(ISpellCaster iOwner, Elements elements, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				return false;
			}
			this.mOwner = iOwner;
			this.mPlayState = iPlayState;
			List<Pair<SpawnSlime.SlimeSize, Elements>> list = new List<Pair<SpawnSlime.SlimeSize, Elements>>();
			string name;
			switch (name = (iOwner as Character).Name)
			{
			case "dungeons_slimecube_poison_large":
			{
				Elements second;
				if (elements != Elements.None)
				{
					second = this.RandomElement(elements);
				}
				else
				{
					second = Elements.Poison;
				}
				for (int i = 0; i < 3; i++)
				{
					list.Add(new Pair<SpawnSlime.SlimeSize, Elements>(SpawnSlime.SlimeSize.Medium, second));
				}
				goto IL_21A;
			}
			case "dungeons_slimecube_poison_medium":
			case "dungeons_slimecube_medium":
			case "dungeons_slimecube_poison":
			{
				Elements second2;
				if (elements != Elements.None)
				{
					second2 = this.RandomElement(elements);
				}
				else
				{
					second2 = Elements.Poison;
				}
				for (int j = 0; j < 3; j++)
				{
					list.Add(new Pair<SpawnSlime.SlimeSize, Elements>(SpawnSlime.SlimeSize.Small, second2));
				}
				goto IL_21A;
			}
			case "dungeons_slimecube_arcane":
			{
				Elements second3;
				if (elements != Elements.None)
				{
					second3 = this.RandomElement(elements);
				}
				else
				{
					second3 = Elements.Arcane;
				}
				for (int k = 0; k < 3; k++)
				{
					list.Add(new Pair<SpawnSlime.SlimeSize, Elements>(SpawnSlime.SlimeSize.Small, second3));
				}
				goto IL_21A;
			}
			case "dungeons_acolyte_a":
			{
				Elements second4;
				if (elements != Elements.None)
				{
					second4 = this.RandomElement(elements);
				}
				else
				{
					second4 = Elements.Poison;
				}
				for (int l = 0; l < 3; l++)
				{
					list.Add(new Pair<SpawnSlime.SlimeSize, Elements>(SpawnSlime.SlimeSize.Small, second4));
				}
				goto IL_21A;
			}
			case "dungeons_acolyte_c":
			{
				Elements second5;
				if (elements != Elements.None)
				{
					second5 = this.RandomElement(elements);
				}
				else
				{
					second5 = Elements.Poison;
				}
				list.Add(new Pair<SpawnSlime.SlimeSize, Elements>(SpawnSlime.SlimeSize.Medium, second5));
				list.Add(new Pair<SpawnSlime.SlimeSize, Elements>(SpawnSlime.SlimeSize.Medium, second5));
				list.Add(new Pair<SpawnSlime.SlimeSize, Elements>(SpawnSlime.SlimeSize.Small, second5));
				list.Add(new Pair<SpawnSlime.SlimeSize, Elements>(SpawnSlime.SlimeSize.Small, second5));
				goto IL_21A;
			}
			}
			Elements second6 = Elements.Poison;
			list.Add(new Pair<SpawnSlime.SlimeSize, Elements>(SpawnSlime.SlimeSize.Small, second6));
			IL_21A:
			this.CreateEntities(list);
			return true;
		}

		// Token: 0x06000BC8 RID: 3016 RVA: 0x00046A94 File Offset: 0x00044C94
		protected Elements RandomElement(Elements elements)
		{
			Elements elements2;
			do
			{
				elements2 = Defines.ElementFromIndex(MagickaMath.Random.Next(11));
			}
			while ((elements2 & elements) == Elements.None);
			return elements2;
		}

		// Token: 0x06000BC9 RID: 3017 RVA: 0x00046ABC File Offset: 0x00044CBC
		private int GetTemplateId(SpawnSlime.SlimeSize size, Elements elem)
		{
			if (elem <= Elements.Arcane)
			{
				if (elem <= Elements.Fire)
				{
					switch (elem)
					{
					case Elements.Earth:
					case Elements.Water:
					case Elements.Cold:
						break;
					case Elements.Earth | Elements.Water:
						goto IL_8C;
					default:
						if (elem != Elements.Fire)
						{
							goto IL_8C;
						}
						break;
					}
				}
				else if (elem != Elements.Lightning && elem != Elements.Arcane)
				{
					goto IL_8C;
				}
				if (size == SpawnSlime.SlimeSize.Small)
				{
					return SpawnSlime.ARCANE_SMALL;
				}
				if (size == SpawnSlime.SlimeSize.Medium)
				{
					return SpawnSlime.ARCANE_MEDIUM;
				}
			}
			else
			{
				if (elem <= Elements.Shield)
				{
					if (elem != Elements.Life && elem != Elements.Shield)
					{
						goto IL_8C;
					}
				}
				else if (elem != Elements.Ice && elem != Elements.Steam && elem != Elements.Poison)
				{
					goto IL_8C;
				}
				if (size == SpawnSlime.SlimeSize.Small)
				{
					return SpawnSlime.POISON_SMALL;
				}
				if (size == SpawnSlime.SlimeSize.Medium)
				{
					return SpawnSlime.POISON_MEDIUM;
				}
			}
			IL_8C:
			return SpawnSlime.POISON_SMALL;
		}

		// Token: 0x06000BCA RID: 3018 RVA: 0x00046B5C File Offset: 0x00044D5C
		protected void CreateEntities(List<Pair<SpawnSlime.SlimeSize, Elements>> entitiesToSpawn)
		{
			Vector3 position = this.mOwner.Position;
			Vector3 direction = this.mOwner.Direction;
			for (int i = 0; i < entitiesToSpawn.Count; i++)
			{
				int templateId = this.GetTemplateId(entitiesToSpawn[i].First, entitiesToSpawn[i].Second);
				CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(templateId);
				NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(this.mOwner.PlayState);
				Vector3 position2 = this.mOwner.Position;
				float num = (float)Math.Sqrt(SpecialAbility.RANDOM.NextDouble());
				float num2 = (float)(SpecialAbility.RANDOM.NextDouble() * 6.2831854820251465);
				float num3 = (float)((double)num * Math.Cos((double)num2));
				float num4 = (float)((double)num * Math.Sin((double)num2));
				position2.X += (float)this.SPREAD * num3;
				position2.Z += (float)this.SPREAD * num4;
				Vector3 vector;
				this.mPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref position2, out vector, MovementProperties.Default);
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
					triggerActionMessage.ActionType = TriggerActionType.SpawnNPC;
					triggerActionMessage.Handle = instance.Handle;
					triggerActionMessage.Template = templateId;
					triggerActionMessage.Id = 0;
					triggerActionMessage.Position = vector;
					triggerActionMessage.Direction = direction;
					triggerActionMessage.Bool0 = false;
					triggerActionMessage.Point0 = 0;
					triggerActionMessage.Point1 = 0;
					triggerActionMessage.Point2 = (int)this.SpawnAnimation;
					triggerActionMessage.Point3 = (int)this.IdleAnimation;
					NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
				}
				instance.Initialize(cachedTemplate, vector, 0);
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
			}
		}

		// Token: 0x06000BCB RID: 3019 RVA: 0x00046E10 File Offset: 0x00045010
		protected void SpawnSlimes(int iTemplateId, int iAmount)
		{
			CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(iTemplateId);
			Vector3 position = this.mOwner.Position;
			Vector3 direction = this.mOwner.Direction;
			for (int i = 0; i < iAmount; i++)
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
				instance.Initialize(cachedTemplate, vector2, 0);
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
			}
		}

		// Token: 0x04000ABF RID: 2751
		protected readonly int SPREAD = 5;

		// Token: 0x04000AC0 RID: 2752
		protected static readonly int POISON_SMALL = "dungeons_slimecube_poison_small".GetHashCodeCustom();

		// Token: 0x04000AC1 RID: 2753
		protected static readonly int POISON_MEDIUM = "dungeons_slimecube_poison".GetHashCodeCustom();

		// Token: 0x04000AC2 RID: 2754
		protected static readonly int POISON_LARGE = "dungeons_slimecube_poison_large".GetHashCodeCustom();

		// Token: 0x04000AC3 RID: 2755
		protected static readonly int ARCANE_SMALL = "dungeons_slimecube_arcane_small".GetHashCodeCustom();

		// Token: 0x04000AC4 RID: 2756
		protected static readonly int ARCANE_MEDIUM = "dungeons_slimecube_arcane".GetHashCodeCustom();

		// Token: 0x04000AC5 RID: 2757
		protected static readonly int ARCANE_LARGE = "dungeons_slimecube_arcane_large".GetHashCodeCustom();

		// Token: 0x04000AC6 RID: 2758
		protected ISpellCaster mOwner;

		// Token: 0x04000AC7 RID: 2759
		protected PlayState mPlayState;

		// Token: 0x04000AC8 RID: 2760
		protected Elements mElements;

		// Token: 0x04000AC9 RID: 2761
		public Animations IdleAnimation;

		// Token: 0x04000ACA RID: 2762
		public Animations SpawnAnimation;

		// Token: 0x04000ACB RID: 2763
		private static SpawnSlime sSingelton;

		// Token: 0x04000ACC RID: 2764
		private static volatile object sSingeltonLock = new object();

		// Token: 0x02000183 RID: 387
		public enum SlimeSize
		{
			// Token: 0x04000ACE RID: 2766
			Small,
			// Token: 0x04000ACF RID: 2767
			Medium,
			// Token: 0x04000AD0 RID: 2768
			Large
		}
	}
}
