using System;
using System.Collections.Generic;
using Magicka.AI.AgentStates;
using Magicka.AI.Arithmetics;
using Magicka.AI.Messaging;
using Magicka.GameLogic;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Abilities;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.Spells;
using Magicka.PathFinding;
using Microsoft.Xna.Framework;
using PolygonHead.Helpers;

namespace Magicka.AI
{
	// Token: 0x020005C1 RID: 1473
	public class Agent : IAI
	{
		// Token: 0x17000A4B RID: 2635
		// (get) Token: 0x06002BE9 RID: 11241 RVA: 0x0015A73E File Offset: 0x0015893E
		// (set) Token: 0x06002BEA RID: 11242 RVA: 0x0015A746 File Offset: 0x00158946
		public Agent Leader
		{
			get
			{
				return this.mLeader;
			}
			set
			{
				this.mLeader = value;
				this.mLeaderAge = 0f;
			}
		}

		// Token: 0x06002BEB RID: 11243 RVA: 0x0015A75C File Offset: 0x0015895C
		internal void CopyPolymorphValues(Agent ai, ref Polymorph.NPCPolymorphData iData)
		{
			this.SetOrder(ai.Order, ai.ReactsTo, ai.ReactionOrder, ai.mPriorityTargetID, -1, ai.mReactionTrigger, null);
			this.mLeader = ai.mLeader;
			this.mLeaderAge = ai.mLeaderAge;
			this.mLoopEvents = ai.mLoopEvents;
			this.mFlocking = ai.mFlocking;
			this.mPath.Clear();
			this.mTargets.Clear();
			this.mTargetAges.Clear();
			this.mNextAbility = null;
			this.mTargetArea = ai.mTargetArea;
			this.mTimeSinceLastUpdate = ai.mTimeSinceLastUpdate;
			this.mWanderAngle = ai.mWanderAngle;
			this.mWanderPause = ai.mWanderPause;
			this.mWanderPaused = ai.mWanderPaused;
			this.mWanderSpeed = ai.mWanderSpeed;
			this.mWanderTimer = ai.mWanderTimer;
			this.mWayPoint = ai.mWayPoint;
			if (!iData.Active)
			{
				iData.Events = ai.mEvents;
				iData.EventIndex = ai.mEventIndex;
				iData.Active = true;
				return;
			}
			this.mEvents = iData.Events;
			this.mEventIndex = iData.EventIndex;
			iData.Active = false;
		}

		// Token: 0x17000A4C RID: 2636
		// (get) Token: 0x06002BEC RID: 11244 RVA: 0x0015A88E File Offset: 0x00158A8E
		public float BreakFreeStrength
		{
			get
			{
				return this.mBreakFreeStrength;
			}
		}

		// Token: 0x17000A4D RID: 2637
		// (get) Token: 0x06002BED RID: 11245 RVA: 0x0015A896 File Offset: 0x00158A96
		// (set) Token: 0x06002BEE RID: 11246 RVA: 0x0015A89E File Offset: 0x00158A9E
		public int TargetArea
		{
			get
			{
				return this.mTargetArea;
			}
			set
			{
				this.mTargetArea = value;
			}
		}

		// Token: 0x17000A4E RID: 2638
		// (get) Token: 0x06002BEF RID: 11247 RVA: 0x0015A8A7 File Offset: 0x00158AA7
		// (set) Token: 0x06002BF0 RID: 11248 RVA: 0x0015A8AF File Offset: 0x00158AAF
		public float WanderSpeed
		{
			get
			{
				return this.mWanderSpeed;
			}
			set
			{
				this.mWanderSpeed = value;
			}
		}

		// Token: 0x17000A4F RID: 2639
		// (get) Token: 0x06002BF1 RID: 11249 RVA: 0x0015A8B8 File Offset: 0x00158AB8
		// (set) Token: 0x06002BF2 RID: 11250 RVA: 0x0015A8C0 File Offset: 0x00158AC0
		public float WanderPause
		{
			get
			{
				return this.mWanderPause;
			}
			set
			{
				this.mWanderPause = value;
			}
		}

		// Token: 0x17000A50 RID: 2640
		// (get) Token: 0x06002BF3 RID: 11251 RVA: 0x0015A8C9 File Offset: 0x00158AC9
		// (set) Token: 0x06002BF4 RID: 11252 RVA: 0x0015A8D1 File Offset: 0x00158AD1
		public bool WanderPaused
		{
			get
			{
				return this.mWanderPaused;
			}
			set
			{
				this.mWanderPaused = value;
			}
		}

		// Token: 0x17000A51 RID: 2641
		// (get) Token: 0x06002BF5 RID: 11253 RVA: 0x0015A8DA File Offset: 0x00158ADA
		// (set) Token: 0x06002BF6 RID: 11254 RVA: 0x0015A8E2 File Offset: 0x00158AE2
		public float WanderTimer
		{
			get
			{
				return this.mWanderTimer;
			}
			set
			{
				this.mWanderTimer = value;
			}
		}

		// Token: 0x17000A52 RID: 2642
		// (get) Token: 0x06002BF7 RID: 11255 RVA: 0x0015A8EB File Offset: 0x00158AEB
		public bool IsLeader
		{
			get
			{
				return this.mLeader == null;
			}
		}

		// Token: 0x17000A53 RID: 2643
		// (get) Token: 0x06002BF8 RID: 11256 RVA: 0x0015A8F6 File Offset: 0x00158AF6
		public IDamageable CurrentTarget
		{
			get
			{
				if (this.mTargets.Count != 0)
				{
					return this.mTargets.Peek();
				}
				return null;
			}
		}

		// Token: 0x17000A54 RID: 2644
		// (get) Token: 0x06002BF9 RID: 11257 RVA: 0x0015A912 File Offset: 0x00158B12
		public IDamageable LastTarget
		{
			get
			{
				return this.mLastTarget;
			}
		}

		// Token: 0x17000A55 RID: 2645
		// (get) Token: 0x06002BFA RID: 11258 RVA: 0x0015A91A File Offset: 0x00158B1A
		// (set) Token: 0x06002BFB RID: 11259 RVA: 0x0015A922 File Offset: 0x00158B22
		public Ability NextAbility
		{
			get
			{
				return this.mNextAbility;
			}
			set
			{
				this.mNextAbility = value;
			}
		}

		// Token: 0x17000A56 RID: 2646
		// (get) Token: 0x06002BFC RID: 11260 RVA: 0x0015A92B File Offset: 0x00158B2B
		// (set) Token: 0x06002BFD RID: 11261 RVA: 0x0015A933 File Offset: 0x00158B33
		public Ability BusyAbility
		{
			get
			{
				return this.mBusyAbility;
			}
			set
			{
				this.mBusyAbility = value;
			}
		}

		// Token: 0x17000A57 RID: 2647
		// (get) Token: 0x06002BFE RID: 11262 RVA: 0x0015A93C File Offset: 0x00158B3C
		public float CurrentTargetAge
		{
			get
			{
				if (this.mTargetAges.Count != 0)
				{
					return this.mTargetAges[this.mTargetAges.Count - 1];
				}
				return float.PositiveInfinity;
			}
		}

		// Token: 0x17000A58 RID: 2648
		// (get) Token: 0x06002BFF RID: 11263 RVA: 0x0015A969 File Offset: 0x00158B69
		public float GroupChase
		{
			get
			{
				return this.mGroupChase;
			}
		}

		// Token: 0x17000A59 RID: 2649
		// (get) Token: 0x06002C00 RID: 11264 RVA: 0x0015A971 File Offset: 0x00158B71
		public float GroupSeparation
		{
			get
			{
				return this.mGroupSeparation;
			}
		}

		// Token: 0x17000A5A RID: 2650
		// (get) Token: 0x06002C01 RID: 11265 RVA: 0x0015A979 File Offset: 0x00158B79
		public float GroupCohesion
		{
			get
			{
				return this.mGroupCohesion;
			}
		}

		// Token: 0x17000A5B RID: 2651
		// (get) Token: 0x06002C02 RID: 11266 RVA: 0x0015A981 File Offset: 0x00158B81
		public float GroupAlignment
		{
			get
			{
				return this.mGroupAlignment;
			}
		}

		// Token: 0x17000A5C RID: 2652
		// (get) Token: 0x06002C03 RID: 11267 RVA: 0x0015A989 File Offset: 0x00158B89
		public float GroupWander
		{
			get
			{
				return this.mGroupWander;
			}
		}

		// Token: 0x17000A5D RID: 2653
		// (get) Token: 0x06002C04 RID: 11268 RVA: 0x0015A991 File Offset: 0x00158B91
		// (set) Token: 0x06002C05 RID: 11269 RVA: 0x0015A999 File Offset: 0x00158B99
		public IDamageable PriorityTarget
		{
			get
			{
				return this.mPriorityTarget;
			}
			set
			{
				this.mPriorityTarget = value;
			}
		}

		// Token: 0x17000A5E RID: 2654
		// (get) Token: 0x06002C06 RID: 11270 RVA: 0x0015A9A2 File Offset: 0x00158BA2
		public int PriorityTargetID
		{
			get
			{
				return this.mPriorityTargetID;
			}
		}

		// Token: 0x17000A5F RID: 2655
		// (get) Token: 0x06002C07 RID: 11271 RVA: 0x0015A9AA File Offset: 0x00158BAA
		public int PriorityAbility
		{
			get
			{
				return this.mPriorityAbilityIdx;
			}
		}

		// Token: 0x17000A60 RID: 2656
		// (get) Token: 0x06002C08 RID: 11272 RVA: 0x0015A9B2 File Offset: 0x00158BB2
		// (set) Token: 0x06002C09 RID: 11273 RVA: 0x0015A9BA File Offset: 0x00158BBA
		public int CurrentEvent
		{
			get
			{
				return this.mEventIndex;
			}
			set
			{
				this.mEventIndex = value;
				if (this.mLoopEvents && this.mEventIndex >= this.mEvents.Length)
				{
					this.mEventIndex = 0;
				}
			}
		}

		// Token: 0x17000A61 RID: 2657
		// (get) Token: 0x06002C0A RID: 11274 RVA: 0x0015A9E2 File Offset: 0x00158BE2
		public AIEvent[] Events
		{
			get
			{
				return this.mEvents;
			}
		}

		// Token: 0x17000A62 RID: 2658
		// (get) Token: 0x06002C0B RID: 11275 RVA: 0x0015A9EA File Offset: 0x00158BEA
		// (set) Token: 0x06002C0C RID: 11276 RVA: 0x0015A9F2 File Offset: 0x00158BF2
		public float CurrentEventDelay
		{
			get
			{
				return this.mCurrentEventDelay;
			}
			set
			{
				this.mCurrentEventDelay = value;
			}
		}

		// Token: 0x17000A63 RID: 2659
		// (get) Token: 0x06002C0D RID: 11277 RVA: 0x0015A9FB File Offset: 0x00158BFB
		// (set) Token: 0x06002C0E RID: 11278 RVA: 0x0015AA03 File Offset: 0x00158C03
		public bool LoopEvents
		{
			get
			{
				return this.mLoopEvents;
			}
			set
			{
				this.mLoopEvents = value;
			}
		}

		// Token: 0x17000A64 RID: 2660
		// (get) Token: 0x06002C0F RID: 11279 RVA: 0x0015AA0C File Offset: 0x00158C0C
		// (set) Token: 0x06002C10 RID: 11280 RVA: 0x0015AA14 File Offset: 0x00158C14
		public float WanderAngle
		{
			get
			{
				return this.mWanderAngle;
			}
			set
			{
				this.mWanderAngle = value;
			}
		}

		// Token: 0x17000A65 RID: 2661
		// (get) Token: 0x06002C11 RID: 11281 RVA: 0x0015AA1D File Offset: 0x00158C1D
		public List<PathNode> Path
		{
			get
			{
				return this.mPath;
			}
		}

		// Token: 0x17000A66 RID: 2662
		// (get) Token: 0x06002C12 RID: 11282 RVA: 0x0015AA25 File Offset: 0x00158C25
		public float LeaderAge
		{
			get
			{
				return this.mLeaderAge;
			}
		}

		// Token: 0x17000A67 RID: 2663
		// (get) Token: 0x06002C13 RID: 11283 RVA: 0x0015AA2D File Offset: 0x00158C2D
		// (set) Token: 0x06002C14 RID: 11284 RVA: 0x0015AA35 File Offset: 0x00158C35
		public float AlertRadius
		{
			get
			{
				return this.mAlertRadius;
			}
			set
			{
				this.mAlertRadius = value;
			}
		}

		// Token: 0x17000A68 RID: 2664
		// (get) Token: 0x06002C15 RID: 11285 RVA: 0x0015AA3E File Offset: 0x00158C3E
		public Order Order
		{
			get
			{
				return this.mOrder;
			}
		}

		// Token: 0x06002C16 RID: 11286 RVA: 0x0015AA48 File Offset: 0x00158C48
		public Agent(NonPlayerCharacter iOwner)
		{
			this.mOwner = iOwner;
			this.mSpellRecovery = new CastSpell(0f, Target.Enemy, null, new Animations[]
			{
				Animations.attack_melee0
			}, 0f, 10f, 1f, 0f, 1f);
		}

		// Token: 0x06002C17 RID: 11287 RVA: 0x0015AB58 File Offset: 0x00158D58
		public void Initialize(NonPlayerCharacter iOwner, CharacterTemplate iTemplate)
		{
			this.mTargetAges.Clear();
			this.mTargets.Clear();
			this.mNextAbility = null;
			this.mBusyAbility = null;
			this.mOrder = Order.Attack;
			this.mReactsTo = ReactTo.None;
			this.mStates.Clear();
			this.mStates.Push(AIStateIdle.Instance);
			this.mStateAge.Add(0f);
			this.mAlertRadius = iTemplate.AlertRadius;
			this.mAngerWeight = iTemplate.AngerWeight;
			this.mDistanceWeight = iTemplate.DistanceWeight;
			this.mHealthWeight = iTemplate.HealthWeight;
			this.mGroupChase = iTemplate.GroupChase;
			this.mGroupSeparation = iTemplate.GroupSeparation;
			this.mGroupCohesion = iTemplate.GroupCohesion;
			this.mGroupAlignment = iTemplate.GroupAlignment;
			this.mGroupWander = iTemplate.GroupWander;
			this.mFriendlyAvoidance = iTemplate.FriendlyAvoidance;
			this.mEnemyAvoidance = iTemplate.EnemyAvoidance;
			this.mSightAvoidance = iTemplate.SightAvoidance;
			this.mDangerAvoidance = iTemplate.DangerAvoidance;
			this.mBreakFreeStrength = iTemplate.BreakFreeStrength;
			this.mFlocking = iTemplate.Flocking;
			this.mWanderAngle = (float)Agent.sRandom.NextDouble() * 6.2831855f;
		}

		// Token: 0x06002C18 RID: 11288 RVA: 0x0015AC90 File Offset: 0x00158E90
		public void Reset()
		{
			this.mOrder = Order.Attack;
			this.mReactsTo = ReactTo.None;
			this.mReactionOrder = Order.None;
			this.mReactionTrigger = 0;
			this.mBeenAttackedBy.Clear();
			this.mTargets.Clear();
			this.mTargetAges.Clear();
			this.mNextAbility = null;
			this.mBusyAbility = null;
			this.mPriorityTargetID = 0;
			this.mPriorityTarget = null;
			this.mPath.Clear();
			this.mStates.Clear();
			this.mStateAge.Clear();
			this.mStates.Push(AIStateIdle.Instance);
			this.mStateAge.Add(0f);
		}

		// Token: 0x06002C19 RID: 11289 RVA: 0x0015AD37 File Offset: 0x00158F37
		public void Disable()
		{
			AIManager.Instance.Agents.Remove(this);
		}

		// Token: 0x06002C1A RID: 11290 RVA: 0x0015AD4C File Offset: 0x00158F4C
		public void Enable()
		{
			AIManager instance = AIManager.Instance;
			if (!instance.Agents.Contains(this))
			{
				instance.Agents.Add(this);
			}
		}

		// Token: 0x06002C1B RID: 11291 RVA: 0x0015AD7C File Offset: 0x00158F7C
		public void ReduceAggro(float iDeltaTime)
		{
			for (int i = 0; i < this.mBeenAttackedBy.Count; i++)
			{
				KeyValuePair<ushort, float> keyValuePair = this.mBeenAttackedBy.GetKeyValuePair(i);
				keyValuePair = new KeyValuePair<ushort, float>(keyValuePair.Key, Math.Min(keyValuePair.Value - 25f * iDeltaTime, 500f));
				Avatar avatar = Entity.GetFromHandle((int)keyValuePair.Key) as Avatar;
				bool flag = avatar != null && (avatar.Equipment[1].Item.PassiveAbility.Ability == Item.PassiveAbilities.ReduceAggro || ((this.Owner.Faction & Factions.UNDEAD) == Factions.UNDEAD && avatar.Equipment[1].Item.PassiveAbility.Ability == Item.PassiveAbilities.ZombieDeterrent));
				if (keyValuePair.Value < 0f)
				{
					this.mBeenAttackedBy.Remove(keyValuePair.Key);
				}
				else if (flag)
				{
					this.mBeenAttackedBy[keyValuePair.Key] = keyValuePair.Value * 0.5f;
				}
				else
				{
					this.mBeenAttackedBy[keyValuePair.Key] = keyValuePair.Value;
				}
			}
		}

		// Token: 0x06002C1C RID: 11292 RVA: 0x0015AE9E File Offset: 0x0015909E
		public void UpdateTime(float iDeltaTime)
		{
			this.mTimeSinceLastUpdate += iDeltaTime;
		}

		// Token: 0x06002C1D RID: 11293 RVA: 0x0015AEB0 File Offset: 0x001590B0
		public void Update()
		{
			if (this.mOwner.Dead)
			{
				return;
			}
			this.ReduceAggro(this.mTimeSinceLastUpdate);
			this.mLeaderAge += this.mTimeSinceLastUpdate;
			for (int i = 0; i < this.mTargetAges.Count; i++)
			{
				List<float> list;
				int index;
				(list = this.mTargetAges)[index = i] = list[index] + this.mTimeSinceLastUpdate;
			}
			for (int j = 0; j < this.mStateAge.Count; j++)
			{
				List<float> list2;
				int index2;
				(list2 = this.mStateAge)[index2 = j] = list2[index2] + this.mTimeSinceLastUpdate;
			}
			bool flag = false;
			Character character = null;
			if ((byte)(this.mReactsTo & ReactTo.Attack) == 1)
			{
				float num = 0f;
				for (int k = 0; k < this.mBeenAttackedBy.Count; k++)
				{
					KeyValuePair<ushort, float> keyValuePair = this.mBeenAttackedBy.GetKeyValuePair(k);
					if (keyValuePair.Value > num)
					{
						character = (Character)Entity.GetFromHandle((int)keyValuePair.Key);
					}
				}
				flag = (character != null);
			}
			if (!flag && (byte)(this.mReactsTo & ReactTo.Proximity) == 2)
			{
				List<Entity> entities = this.Owner.PlayState.EntityManager.GetEntities(this.Owner.Position, this.mAlertRadius, false);
				for (int l = 0; l < entities.Count; l++)
				{
					Character character2 = entities[l] as Character;
					if (character2 != null && (this.Owner.Faction & character2.Faction) == Factions.NONE && (!character2.IsInvisibile | this.mOwner.CanSeeInvisible))
					{
						flag = true;
						if (this.mReactionOrder != Order.None && this.mReactionOrder != this.mOrder)
						{
							this.mAlertMode = AlertMode.Danger;
						}
					}
				}
				this.Owner.PlayState.EntityManager.ReturnEntityList(entities);
			}
			if (flag)
			{
				this.React();
			}
			this.mStates.Peek().OnExecute(this, this.mTimeSinceLastUpdate);
			Ability nextAbility = this.NextAbility;
			if (nextAbility != null)
			{
				nextAbility.Update(this, this.mTimeSinceLastUpdate);
			}
			this.mTimeSinceLastUpdate = 0f;
		}

		// Token: 0x06002C1E RID: 11294 RVA: 0x0015B0D4 File Offset: 0x001592D4
		public float ChooseTarget(out IDamageable oTarget, out Ability oAbility)
		{
			this.mPriorityTarget = (Entity.GetByID(this.mPriorityTargetID) as IDamageable);
			for (int i = 0; i < this.mFuzzySortValues.Length; i++)
			{
				this.mFuzzySortEntities[i] = null;
				this.mFuzzySortValues[i] = float.MinValue;
			}
			List<Entity> entities = this.Owner.PlayState.EntityManager.GetEntities(this.Owner.Position, 40f, false);
			Player[] players = Game.Instance.Players;
			for (int j = 0; j < players.Length; j++)
			{
				Avatar avatar = players[j].Avatar;
				if (!entities.Contains(avatar))
				{
					entities.Add(avatar);
				}
			}
			if (!entities.Contains(this.mPriorityTarget as Entity))
			{
				entities.Add(this.mPriorityTarget as Entity);
			}
			int num = this.mFuzzySortValues.Length;
			int num2 = num / 2;
			for (int k = 0; k < entities.Count; k++)
			{
				IDamageable damageable = entities[k] as IDamageable;
				Character character = entities[k] as Character;
				if (damageable != null && !(damageable.Dead | damageable == this.mOwner) && (damageable == this.mPriorityTarget || damageable is Character) && (character == null || !(character.IsInvisibile & !this.mOwner.CanSeeInvisible)))
				{
					float iDistance = Vector3.Distance(this.Owner.Body.Position, damageable.Position);
					float num3 = 0f;
					if (damageable == this.mPriorityTarget)
					{
						num3 += 1f;
					}
					float num4 = 1f;
					if (damageable is Character)
					{
						num4 = (damageable as Character).GetAgroMultiplier();
					}
					num3 += FuzzyMath.FuzzyAnger(this, damageable as Entity) * this.mAngerWeight;
					num3 += (1f - damageable.HitPoints / damageable.MaxHitPoints) * this.mHealthWeight;
					num3 += FuzzyMath.FuzzyDistanceExponential(iDistance, this.mAlertRadius) * this.mDistanceWeight;
					num3 *= num4;
					if (character != null && (character.Faction & this.mOwner.Faction) == Factions.NONE)
					{
						for (int l = 0; l < num2; l++)
						{
							if (num3 > this.mFuzzySortValues[l])
							{
								for (int m = num2 - 1; m > l; m--)
								{
									this.mFuzzySortEntities[m] = this.mFuzzySortEntities[m - 1];
									this.mFuzzySortValues[m] = this.mFuzzySortValues[m - 1];
								}
								this.mFuzzySortEntities[l] = damageable;
								this.mFuzzySortValues[l] = num3;
								break;
							}
						}
					}
					else
					{
						for (int n = num2; n < num; n++)
						{
							if (num3 > this.mFuzzySortValues[n])
							{
								for (int num5 = num - 1; num5 > n; num5--)
								{
									this.mFuzzySortEntities[num5] = this.mFuzzySortEntities[num5 - 1];
									this.mFuzzySortValues[num5] = this.mFuzzySortValues[num5 - 1];
								}
								this.mFuzzySortEntities[n] = damageable;
								this.mFuzzySortValues[n] = num3;
								break;
							}
						}
					}
				}
			}
			this.Owner.PlayState.EntityManager.ReturnEntityList(entities);
			ExpressionArguments expressionArguments;
			expressionArguments.AI = this;
			expressionArguments.AIPos = this.mOwner.Position;
			expressionArguments.AIDir = this.mOwner.Direction;
			for (int num6 = 0; num6 < num; num6++)
			{
				IDamageable damageable2 = this.mFuzzySortEntities[num6];
				if (damageable2 != null)
				{
					expressionArguments.Target = damageable2;
					expressionArguments.TargetPos = damageable2.Position;
					expressionArguments.TargetDir = damageable2.Body.Orientation.Forward;
					Vector3.Subtract(ref expressionArguments.TargetPos, ref expressionArguments.AIPos, out expressionArguments.Delta);
					expressionArguments.Distance = expressionArguments.Delta.Length();
					Vector3.Divide(ref expressionArguments.Delta, expressionArguments.Distance, out expressionArguments.DeltaNormalized);
					this.mFuzzySortValues[num6] += this.ChooseAbility(ref expressionArguments, out this.mFuzzySortAbilities[num6]);
				}
			}
			oTarget = null;
			oAbility = null;
			float num7 = float.MinValue;
			for (int num8 = 0; num8 < this.mFuzzySortValues.Length; num8++)
			{
				if (this.mFuzzySortValues[num8] > num7)
				{
					oTarget = this.mFuzzySortEntities[num8];
					oAbility = this.mFuzzySortAbilities[num8];
					num7 = this.mFuzzySortValues[num8];
				}
			}
			expressionArguments.Target = this.mOwner;
			expressionArguments.TargetPos = this.mOwner.Position;
			expressionArguments.TargetDir = this.mOwner.Direction;
			expressionArguments.Delta = default(Vector3);
			expressionArguments.Distance = 0f;
			expressionArguments.DeltaNormalized = default(Vector3);
			Ability ability;
			float num9 = this.ChooseAbility(ref expressionArguments, out ability);
			if (num9 > num7)
			{
				num7 = num9;
				oAbility = ability;
				oTarget = this.mOwner;
			}
			if (oAbility != this.mSpellRecovery)
			{
				this.mLastSpellAbility = (oAbility as CastSpell);
			}
			if (oTarget != null)
			{
				this.mLastTarget = oTarget;
			}
			return num7;
		}

		// Token: 0x06002C1F RID: 11295 RVA: 0x0015B5F8 File Offset: 0x001597F8
		public float ChooseAbility(ref ExpressionArguments iArgs, out Ability oAbility)
		{
			float num = float.MinValue;
			oAbility = null;
			Ability[] abilities = this.mOwner.Abilities;
			for (int i = 0; i < abilities.Length; i++)
			{
				if (this.mOwner.AbilityCooldown[i] <= 0f)
				{
					Ability ability = abilities[i];
					Character character = iArgs.Target as Character;
					if (!((ability.Target == Target.Self & iArgs.Target != this.mOwner) | (ability.Target == Target.Friendly & (iArgs.Target == this.mOwner || character == null || (character.Faction & this.mOwner.Faction) == Factions.NONE)) | (ability.Target == Target.Enemy & (character != null && (character.Faction & this.mOwner.Faction) != Factions.NONE))) && (this.mOwner.SpellQueue.Count <= 0 || (ability is CastSpell && SpellManager.Equatable((ability as CastSpell).Elements, this.mOwner.SpellQueue))))
					{
						float num2 = ability.GetDesirability(ref iArgs);
						if (i == this.mPriorityAbilityIdx & iArgs.Target == this.mPriorityTarget)
						{
							num2 += 0.5f;
						}
						if (num2 > 0f && num2 > num)
						{
							num = num2;
							oAbility = ability;
						}
					}
				}
			}
			if (oAbility == null && this.mOwner.SpellQueue.Count > 0 && this.mLastSpellAbility != null && this.mNextAbility != this.mSpellRecovery)
			{
				Character character2 = iArgs.Target as Character;
				if (this.mLastSpellAbility.Target == Target.Enemy & (character2 == null || (character2.Faction & this.mOwner.Faction) == Factions.NONE))
				{
					this.mSpellRecovery.Animations = this.mLastSpellAbility.Animations;
					this.mSpellRecovery.CastType = this.mLastSpellAbility.CastType;
					this.mSpellRecovery.Target = this.mLastSpellAbility.Target;
					this.mSpellRecovery.Cooldown = this.mLastSpellAbility.Cooldown;
					Spell[] array = new Spell[this.mOwner.SpellQueue.Count];
					this.mOwner.SpellQueue.CopyTo(array, 0);
					Elements[] array2 = new Elements[this.mOwner.SpellQueue.Count];
					for (int j = 0; j < array.Length; j++)
					{
						array2[j] = array[j].Element;
					}
					this.mSpellRecovery.Elements = array2;
					oAbility = this.mSpellRecovery;
					num = 3E-45f;
				}
			}
			return num;
		}

		// Token: 0x06002C20 RID: 11296 RVA: 0x0015B8A0 File Offset: 0x00159AA0
		internal void SetOrder(Order iGoal, ReactTo iReactTo, Order iReactionOrder, int iPriorityTarget, int iPriorityAbility, int iReactionTrigger, AIEvent[] iEvents)
		{
			if (this.mOrder != iGoal)
			{
				while (this.CurrentState != AIStateIdle.Instance)
				{
					if (this.CurrentState is AIStateAttack)
					{
						this.ReleaseTarget();
					}
					this.PopState();
				}
			}
			this.mOrder = iGoal;
			this.mReactsTo = iReactTo;
			this.mReactionOrder = iReactionOrder;
			this.mReactionTrigger = iReactionTrigger;
			this.mEvents = iEvents;
			this.mEventIndex = 0;
			if (iEvents != null)
			{
				this.mCurrentEventDelay = 0f;
			}
			this.mPriorityTargetID = iPriorityTarget;
			this.mPriorityTarget = (Entity.GetByID(this.mPriorityTargetID) as IDamageable);
			this.mPriorityAbilityIdx = iPriorityAbility;
		}

		// Token: 0x17000A69 RID: 2665
		// (get) Token: 0x06002C21 RID: 11297 RVA: 0x0015B93F File Offset: 0x00159B3F
		public IAIState CurrentState
		{
			get
			{
				return this.mStates.Peek();
			}
		}

		// Token: 0x17000A6A RID: 2666
		// (get) Token: 0x06002C22 RID: 11298 RVA: 0x0015B94C File Offset: 0x00159B4C
		public float CurrentStateAge
		{
			get
			{
				return this.mStateAge[this.mStateAge.Count - 1];
			}
		}

		// Token: 0x17000A6B RID: 2667
		// (get) Token: 0x06002C23 RID: 11299 RVA: 0x0015B966 File Offset: 0x00159B66
		// (set) Token: 0x06002C24 RID: 11300 RVA: 0x0015B96E File Offset: 0x00159B6E
		public ReactTo ReactsTo
		{
			get
			{
				return this.mReactsTo;
			}
			set
			{
				this.mReactsTo = value;
			}
		}

		// Token: 0x17000A6C RID: 2668
		// (get) Token: 0x06002C25 RID: 11301 RVA: 0x0015B977 File Offset: 0x00159B77
		public Order ReactionOrder
		{
			get
			{
				return this.mReactionOrder;
			}
		}

		// Token: 0x17000A6D RID: 2669
		// (get) Token: 0x06002C26 RID: 11302 RVA: 0x0015B97F File Offset: 0x00159B7F
		public int ReactionTrigger
		{
			get
			{
				return this.mReactionTrigger;
			}
		}

		// Token: 0x06002C27 RID: 11303 RVA: 0x0015B988 File Offset: 0x00159B88
		public void PushState(IAIState iNewState)
		{
			IAIState iaistate = this.mStates.Peek();
			if (iNewState != iaistate)
			{
				iaistate.OnExit(this);
				this.mStates.Push(iNewState);
				this.mStateAge.Add(0f);
				iNewState.OnEnter(this);
			}
		}

		// Token: 0x06002C28 RID: 11304 RVA: 0x0015B9D0 File Offset: 0x00159BD0
		public void PopState()
		{
			this.mStates.Peek().OnExit(this);
			this.mStates.Pop();
			this.mStateAge.RemoveAt(this.mStateAge.Count - 1);
			this.mStates.Peek().OnEnter(this);
		}

		// Token: 0x17000A6E RID: 2670
		// (get) Token: 0x06002C29 RID: 11305 RVA: 0x0015BA23 File Offset: 0x00159C23
		public Character Owner
		{
			get
			{
				return this.mOwner;
			}
		}

		// Token: 0x17000A6F RID: 2671
		// (get) Token: 0x06002C2A RID: 11306 RVA: 0x0015BA2B File Offset: 0x00159C2B
		public NonPlayerCharacter NPC
		{
			get
			{
				return this.mOwner;
			}
		}

		// Token: 0x17000A70 RID: 2672
		// (get) Token: 0x06002C2B RID: 11307 RVA: 0x0015BA33 File Offset: 0x00159C33
		// (set) Token: 0x06002C2C RID: 11308 RVA: 0x0015BA3B File Offset: 0x00159C3B
		public Vector3 WayPoint
		{
			get
			{
				return this.mWayPoint;
			}
			set
			{
				this.mWayPoint = value;
			}
		}

		// Token: 0x17000A71 RID: 2673
		// (get) Token: 0x06002C2D RID: 11309 RVA: 0x0015BA44 File Offset: 0x00159C44
		public SyncedList<ushort, float> AttackedBy
		{
			get
			{
				return this.mBeenAttackedBy;
			}
		}

		// Token: 0x06002C2E RID: 11310 RVA: 0x0015BA4C File Offset: 0x00159C4C
		public void Alarm(Character iTarget)
		{
			if (this.mReactionOrder != Order.None && this.mReactionOrder != this.mOrder)
			{
				this.mAlertMode = AlertMode.Danger;
			}
		}

		// Token: 0x06002C2F RID: 11311 RVA: 0x0015BA6C File Offset: 0x00159C6C
		public void AddAttackedBy(Character iAttacker, float iAmount)
		{
			float num;
			this.mBeenAttackedBy.TryGetValue(iAttacker.Handle, out num);
			if ((iAttacker.Faction & this.Owner.Faction) != Factions.NONE)
			{
				num += iAmount * 0f;
			}
			else
			{
				num += iAmount;
			}
			this.mBeenAttackedBy[iAttacker.Handle] = num;
			if (this.mBeenAttackedBy[iAttacker.Handle] > 1E-45f)
			{
				if (((this.mBeenAttackedBy[iAttacker.Handle] > 1E-45f && (this.Owner.Faction == Factions.FRIENDLY || this.Owner.Faction == Factions.NEUTRAL)) || iAttacker is Avatar) && ((this.Owner.Faction & Factions.FRIENDLY & Factions.NEUTRAL) != Factions.NONE || iAttacker is Avatar))
				{
					List<Entity> entities = this.Owner.PlayState.EntityManager.GetEntities(this.Owner.Position, this.mAlertRadius, false);
					for (int i = 0; i < entities.Count; i++)
					{
						if ((entities[i] != null || entities[i] == this.Owner) && entities[i] is NonPlayerCharacter && (this.Owner.Faction & (entities[i] as NonPlayerCharacter).Faction) != Factions.NONE)
						{
							float num2 = iAmount;
							NonPlayerCharacter nonPlayerCharacter = entities[i] as NonPlayerCharacter;
							if ((iAttacker.Faction & nonPlayerCharacter.Faction) != Factions.NONE)
							{
								num2 *= 0f;
							}
							if (nonPlayerCharacter.AI != null && num2 > 1E-45f)
							{
								nonPlayerCharacter.AI.AttackedBy.TryGetValue(iAttacker.Handle, out num);
								num += num2;
								nonPlayerCharacter.AI.AttackedBy[iAttacker.Handle] = num;
							}
						}
					}
					this.Owner.PlayState.EntityManager.ReturnEntityList(entities);
					return;
				}
			}
			else
			{
				this.mBeenAttackedBy.Remove(iAttacker.Handle);
			}
		}

		// Token: 0x17000A72 RID: 2674
		// (get) Token: 0x06002C30 RID: 11312 RVA: 0x0015BC63 File Offset: 0x00159E63
		// (set) Token: 0x06002C31 RID: 11313 RVA: 0x0015BC6B File Offset: 0x00159E6B
		public int LastVisitedNode
		{
			get
			{
				return this.mLastVisitedNode;
			}
			set
			{
				this.mLastVisitedNode = value;
			}
		}

		// Token: 0x06002C32 RID: 11314 RVA: 0x0015BC74 File Offset: 0x00159E74
		public void HandleMessage(Message iMessage)
		{
			switch (iMessage.MessageType)
			{
			case MessageType.TakingDamage:
				this.AddAttackedBy(iMessage.Tag as Character, iMessage.Value);
				break;
			case MessageType.AttackingTarget:
				break;
			default:
				return;
			}
		}

		// Token: 0x06002C33 RID: 11315 RVA: 0x0015BCB4 File Offset: 0x00159EB4
		internal void React()
		{
			if (this.mReactionOrder != Order.None)
			{
				this.mOrder = this.mReactionOrder;
			}
			if (this.mReactionTrigger != 0)
			{
				this.Owner.PlayState.Level.CurrentScene.ExecuteTrigger(this.mReactionTrigger, this.Owner, false);
			}
		}

		// Token: 0x06002C34 RID: 11316 RVA: 0x0015BD04 File Offset: 0x00159F04
		public void ReleaseTarget()
		{
			if (this.mTargets.Count > 0)
			{
				this.mTargets.Pop();
				this.mTargetAges.RemoveAt(this.mTargetAges.Count - 1);
			}
			this.mNextAbility = null;
		}

		// Token: 0x06002C35 RID: 11317 RVA: 0x0015BD40 File Offset: 0x00159F40
		public void AddTarget(IDamageable iDamageable, Ability iAbility)
		{
			if (iDamageable == this.CurrentTarget)
			{
				return;
			}
			if (iDamageable != null)
			{
				this.mTargets.Push(iDamageable);
				this.NextAbility = iAbility;
				this.mTargetAges.Add(0f);
				return;
			}
			throw new ArgumentException("iDamageable connot be null!", "iDamageable");
		}

		// Token: 0x17000A73 RID: 2675
		// (get) Token: 0x06002C36 RID: 11318 RVA: 0x0015BD8D File Offset: 0x00159F8D
		public MovementProperties MoveAbilities
		{
			get
			{
				return this.mOwner.MoveAbilities;
			}
		}

		// Token: 0x17000A74 RID: 2676
		// (get) Token: 0x06002C37 RID: 11319 RVA: 0x0015BD9A File Offset: 0x00159F9A
		public AlertMode AlertMode
		{
			get
			{
				return this.mAlertMode;
			}
		}

		// Token: 0x17000A75 RID: 2677
		// (get) Token: 0x06002C38 RID: 11320 RVA: 0x0015BDA2 File Offset: 0x00159FA2
		public Dictionary<byte, Animations[]> MoveAnimations
		{
			get
			{
				return this.mOwner.MoveAnimations;
			}
		}

		// Token: 0x06002C39 RID: 11321 RVA: 0x0015BDB0 File Offset: 0x00159FB0
		internal void ChangeTarget(IDamageable iDamageable, Ability iAbility)
		{
			if (iDamageable != null)
			{
				this.mTargets.Pop();
				this.mTargets.Push(iDamageable);
				this.NextAbility = iAbility;
				this.mTargetAges[this.mTargetAges.Count - 1] = 0f;
			}
		}

		// Token: 0x06002C3A RID: 11322 RVA: 0x0015BDFC File Offset: 0x00159FFC
		public void GetAvoidance(out Vector3 oAvoidance)
		{
			Vector3 vector = default(Vector3);
			Vector3 vector2 = default(Vector3);
			Vector3 vector3 = default(Vector3);
			Vector3 vector4 = default(Vector3);
			Vector3 position = this.mOwner.Position;
			List<Entity> entities = this.mOwner.PlayState.EntityManager.GetEntities(position, 12f, false);
			entities.Remove(this.mOwner);
			for (int i = 0; i < entities.Count; i++)
			{
				Entity entity = entities[i];
				if (entity is Character)
				{
					Character character = entity as Character;
					if (!character.IsInvisibile)
					{
						if ((character.Faction & this.mOwner.Faction) != Factions.NONE)
						{
							Vector3 position2 = character.Position;
							Vector3 vector5;
							Vector3.Subtract(ref position, ref position2, out vector5);
							float num = vector5.Length();
							float scaleFactor;
							if (num < 1E-06f)
							{
								scaleFactor = 1f;
							}
							else
							{
								scaleFactor = 1f - num / 12f;
							}
							vector5.Normalize();
							Vector3.Multiply(ref vector5, scaleFactor, out vector5);
							Vector3.Multiply(ref vector5, character.GetDanger(), out vector5);
							Vector3.Add(ref vector2, ref vector5, out vector2);
						}
						else
						{
							Vector3 position3 = character.Position;
							Vector3 direction = character.Direction;
							Vector3 vector6;
							Vector3.Subtract(ref position, ref position3, out vector6);
							float num2 = vector6.Length();
							float scaleFactor2;
							if (num2 >= 1E-06f)
							{
								scaleFactor2 = 1f - num2 / 12f;
								Vector3.Divide(ref vector6, num2, out vector6);
								Vector3 vector7;
								Vector3.Cross(ref vector6, ref direction, out vector7);
								Vector3 vector8 = default(Vector3);
								vector8.Y = ((vector7.Y > 0f) ? 1f : -1f);
								Vector3 vector9;
								Vector3.Cross(ref vector6, ref vector8, out vector9);
								vector9.Normalize();
								float num3;
								Vector3.Dot(ref vector6, ref direction, out num3);
								float num4 = num2 / 12f;
								num4 = num4 * num4 * (1f - num4) * 6.75f;
								num3 = MathHelper.Clamp(num3, 0f, 1f);
								num3 = (float)Math.Pow((double)num3, 0.25);
								Vector3.Multiply(ref vector9, num3, out vector9);
								Vector3.Multiply(ref vector9, num4, out vector9);
								Vector3.Add(ref vector, ref vector9, out vector);
							}
							else
							{
								scaleFactor2 = 1f;
							}
							float danger = character.GetDanger();
							vector6.Normalize();
							Vector3.Multiply(ref vector6, scaleFactor2, out vector6);
							Vector3.Multiply(ref vector6, danger, out vector6);
							Vector3.Add(ref vector3, ref vector6, out vector3);
						}
					}
				}
				else
				{
					float danger2 = entity.GetDanger();
					Vector3 position4 = entity.Position;
					Vector3 vector10;
					Vector3.Subtract(ref position, ref position4, out vector10);
					vector10.Y = 0f;
					float num5 = vector10.Length();
					float scaleFactor3 = 1f - num5 / 12f;
					if (num5 > 1E-06f)
					{
						vector10.Normalize();
					}
					else
					{
						vector10 = default(Vector3);
					}
					Vector3.Multiply(ref vector10, scaleFactor3, out vector10);
					Vector3.Multiply(ref vector10, danger2, out vector10);
					Vector3.Add(ref vector4, ref vector10, out vector4);
				}
			}
			this.mOwner.PlayState.EntityManager.ReturnEntityList(entities);
			Vector3.Multiply(ref vector, this.mSightAvoidance, out vector);
			Vector3.Multiply(ref vector2, this.mFriendlyAvoidance, out vector2);
			Vector3.Multiply(ref vector3, this.mEnemyAvoidance, out vector3);
			Vector3.Multiply(ref vector4, this.mDangerAvoidance, out vector4);
			Vector3.Add(ref vector, ref vector2, out oAvoidance);
			Vector3.Add(ref vector3, ref oAvoidance, out oAvoidance);
			Vector3.Add(ref vector4, ref oAvoidance, out oAvoidance);
			float num6 = oAvoidance.Length();
			if (float.IsInfinity(num6))
			{
				oAvoidance = default(Vector3);
			}
			if (num6 > 100000f)
			{
				Vector3.Divide(ref oAvoidance, num6 / 100000f, out oAvoidance);
			}
		}

		// Token: 0x04002FA8 RID: 12200
		public const float PRIORITY_TARGET_BIAS = 1f;

		// Token: 0x04002FA9 RID: 12201
		public const float PRIORITY_ABILITY_BIAS = 0.5f;

		// Token: 0x04002FAA RID: 12202
		private float mAngerWeight = 1.5f;

		// Token: 0x04002FAB RID: 12203
		private float mDistanceWeight = 1f;

		// Token: 0x04002FAC RID: 12204
		private float mHealthWeight = 0.25f;

		// Token: 0x04002FAD RID: 12205
		private float mAlertRadius = 15f;

		// Token: 0x04002FAE RID: 12206
		private float mGroupChase;

		// Token: 0x04002FAF RID: 12207
		private float mGroupSeparation;

		// Token: 0x04002FB0 RID: 12208
		private float mGroupCohesion;

		// Token: 0x04002FB1 RID: 12209
		private float mGroupAlignment;

		// Token: 0x04002FB2 RID: 12210
		private float mGroupWander;

		// Token: 0x04002FB3 RID: 12211
		private float mFriendlyAvoidance;

		// Token: 0x04002FB4 RID: 12212
		private float mEnemyAvoidance;

		// Token: 0x04002FB5 RID: 12213
		private float mSightAvoidance;

		// Token: 0x04002FB6 RID: 12214
		private float mDangerAvoidance;

		// Token: 0x04002FB7 RID: 12215
		private float mBreakFreeStrength;

		// Token: 0x04002FB8 RID: 12216
		private bool mFlocking;

		// Token: 0x04002FB9 RID: 12217
		private static Random sRandom = new Random();

		// Token: 0x04002FBA RID: 12218
		private NonPlayerCharacter mOwner;

		// Token: 0x04002FBB RID: 12219
		private Stack<IAIState> mStates = new Stack<IAIState>(8);

		// Token: 0x04002FBC RID: 12220
		private List<float> mStateAge = new List<float>(8);

		// Token: 0x04002FBD RID: 12221
		private SyncedList<ushort, float> mBeenAttackedBy = new SyncedList<ushort, float>(32);

		// Token: 0x04002FBE RID: 12222
		private List<PathNode> mPath = new List<PathNode>(256);

		// Token: 0x04002FBF RID: 12223
		private float mWanderAngle;

		// Token: 0x04002FC0 RID: 12224
		private bool mLoopEvents;

		// Token: 0x04002FC1 RID: 12225
		private AIEvent[] mEvents;

		// Token: 0x04002FC2 RID: 12226
		private int mEventIndex;

		// Token: 0x04002FC3 RID: 12227
		private float mCurrentEventDelay;

		// Token: 0x04002FC4 RID: 12228
		private Stack<IDamageable> mTargets = new Stack<IDamageable>(4);

		// Token: 0x04002FC5 RID: 12229
		private Ability mNextAbility;

		// Token: 0x04002FC6 RID: 12230
		private Ability mBusyAbility;

		// Token: 0x04002FC7 RID: 12231
		private CastSpell mLastSpellAbility;

		// Token: 0x04002FC8 RID: 12232
		private List<float> mTargetAges = new List<float>(4);

		// Token: 0x04002FC9 RID: 12233
		private int mPriorityTargetID;

		// Token: 0x04002FCA RID: 12234
		private IDamageable mPriorityTarget;

		// Token: 0x04002FCB RID: 12235
		private int mPriorityAbilityIdx;

		// Token: 0x04002FCC RID: 12236
		private Vector3 mWayPoint;

		// Token: 0x04002FCD RID: 12237
		private int mTargetArea;

		// Token: 0x04002FCE RID: 12238
		private float mWanderSpeed = 1f;

		// Token: 0x04002FCF RID: 12239
		private float mWanderPause;

		// Token: 0x04002FD0 RID: 12240
		private float mWanderTimer;

		// Token: 0x04002FD1 RID: 12241
		private bool mWanderPaused;

		// Token: 0x04002FD2 RID: 12242
		private Agent mLeader;

		// Token: 0x04002FD3 RID: 12243
		private float mLeaderAge;

		// Token: 0x04002FD4 RID: 12244
		private int mLastVisitedNode = -1;

		// Token: 0x04002FD5 RID: 12245
		private AlertMode mAlertMode;

		// Token: 0x04002FD6 RID: 12246
		private Order mOrder = Order.Attack;

		// Token: 0x04002FD7 RID: 12247
		private ReactTo mReactsTo = ReactTo.Attack | ReactTo.Proximity;

		// Token: 0x04002FD8 RID: 12248
		private Order mReactionOrder;

		// Token: 0x04002FD9 RID: 12249
		private int mReactionTrigger;

		// Token: 0x04002FDA RID: 12250
		private float mTimeSinceLastUpdate;

		// Token: 0x04002FDB RID: 12251
		private float[] mFuzzySortValues = new float[8];

		// Token: 0x04002FDC RID: 12252
		private Ability[] mFuzzySortAbilities = new Ability[8];

		// Token: 0x04002FDD RID: 12253
		private IDamageable[] mFuzzySortEntities = new IDamageable[8];

		// Token: 0x04002FDE RID: 12254
		private CastSpell mSpellRecovery;

		// Token: 0x04002FDF RID: 12255
		private IDamageable mLastTarget;
	}
}
