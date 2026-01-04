// Decompiled with JetBrains decompiler
// Type: Magicka.AI.Agent
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

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
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.AI;

public class Agent : IAI
{
  public const float PRIORITY_TARGET_BIAS = 1f;
  public const float PRIORITY_ABILITY_BIAS = 0.5f;
  private float mAngerWeight = 1.5f;
  private float mDistanceWeight = 1f;
  private float mHealthWeight = 0.25f;
  private float mAlertRadius = 15f;
  private float mGroupChase;
  private float mGroupSeparation;
  private float mGroupCohesion;
  private float mGroupAlignment;
  private float mGroupWander;
  private float mFriendlyAvoidance;
  private float mEnemyAvoidance;
  private float mSightAvoidance;
  private float mDangerAvoidance;
  private float mBreakFreeStrength;
  private bool mFlocking;
  private static Random sRandom = new Random();
  private NonPlayerCharacter mOwner;
  private Stack<IAIState> mStates = new Stack<IAIState>(8);
  private List<float> mStateAge = new List<float>(8);
  private SyncedList<ushort, float> mBeenAttackedBy = new SyncedList<ushort, float>(32 /*0x20*/);
  private List<PathNode> mPath = new List<PathNode>(256 /*0x0100*/);
  private float mWanderAngle;
  private bool mLoopEvents;
  private AIEvent[] mEvents;
  private int mEventIndex;
  private float mCurrentEventDelay;
  private Stack<IDamageable> mTargets = new Stack<IDamageable>(4);
  private Ability mNextAbility;
  private Ability mBusyAbility;
  private CastSpell mLastSpellAbility;
  private List<float> mTargetAges = new List<float>(4);
  private int mPriorityTargetID;
  private IDamageable mPriorityTarget;
  private int mPriorityAbilityIdx;
  private Vector3 mWayPoint;
  private int mTargetArea;
  private float mWanderSpeed = 1f;
  private float mWanderPause;
  private float mWanderTimer;
  private bool mWanderPaused;
  private Agent mLeader;
  private float mLeaderAge;
  private int mLastVisitedNode = -1;
  private AlertMode mAlertMode;
  private Order mOrder = Order.Attack;
  private ReactTo mReactsTo = ReactTo.Attack | ReactTo.Proximity;
  private Order mReactionOrder;
  private int mReactionTrigger;
  private float mTimeSinceLastUpdate;
  private float[] mFuzzySortValues = new float[8];
  private Ability[] mFuzzySortAbilities = new Ability[8];
  private IDamageable[] mFuzzySortEntities = new IDamageable[8];
  private CastSpell mSpellRecovery;
  private IDamageable mLastTarget;

  public Agent Leader
  {
    get => this.mLeader;
    set
    {
      this.mLeader = value;
      this.mLeaderAge = 0.0f;
    }
  }

  internal void CopyPolymorphValues(Agent ai, ref Polymorph.NPCPolymorphData iData)
  {
    this.SetOrder(ai.Order, ai.ReactsTo, ai.ReactionOrder, ai.mPriorityTargetID, -1, ai.mReactionTrigger, (AIEvent[]) null);
    this.mLeader = ai.mLeader;
    this.mLeaderAge = ai.mLeaderAge;
    this.mLoopEvents = ai.mLoopEvents;
    this.mFlocking = ai.mFlocking;
    this.mPath.Clear();
    this.mTargets.Clear();
    this.mTargetAges.Clear();
    this.mNextAbility = (Ability) null;
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
    }
    else
    {
      this.mEvents = iData.Events;
      this.mEventIndex = iData.EventIndex;
      iData.Active = false;
    }
  }

  public float BreakFreeStrength => this.mBreakFreeStrength;

  public int TargetArea
  {
    get => this.mTargetArea;
    set => this.mTargetArea = value;
  }

  public float WanderSpeed
  {
    get => this.mWanderSpeed;
    set => this.mWanderSpeed = value;
  }

  public float WanderPause
  {
    get => this.mWanderPause;
    set => this.mWanderPause = value;
  }

  public bool WanderPaused
  {
    get => this.mWanderPaused;
    set => this.mWanderPaused = value;
  }

  public float WanderTimer
  {
    get => this.mWanderTimer;
    set => this.mWanderTimer = value;
  }

  public bool IsLeader => this.mLeader == null;

  public IDamageable CurrentTarget
  {
    get => this.mTargets.Count != 0 ? this.mTargets.Peek() : (IDamageable) null;
  }

  public IDamageable LastTarget => this.mLastTarget;

  public Ability NextAbility
  {
    get => this.mNextAbility;
    set => this.mNextAbility = value;
  }

  public Ability BusyAbility
  {
    get => this.mBusyAbility;
    set => this.mBusyAbility = value;
  }

  public float CurrentTargetAge
  {
    get
    {
      return this.mTargetAges.Count != 0 ? this.mTargetAges[this.mTargetAges.Count - 1] : float.PositiveInfinity;
    }
  }

  public float GroupChase => this.mGroupChase;

  public float GroupSeparation => this.mGroupSeparation;

  public float GroupCohesion => this.mGroupCohesion;

  public float GroupAlignment => this.mGroupAlignment;

  public float GroupWander => this.mGroupWander;

  public IDamageable PriorityTarget
  {
    get => this.mPriorityTarget;
    set => this.mPriorityTarget = value;
  }

  public int PriorityTargetID => this.mPriorityTargetID;

  public int PriorityAbility => this.mPriorityAbilityIdx;

  public int CurrentEvent
  {
    get => this.mEventIndex;
    set
    {
      this.mEventIndex = value;
      if (!this.mLoopEvents || this.mEventIndex < this.mEvents.Length)
        return;
      this.mEventIndex = 0;
    }
  }

  public AIEvent[] Events => this.mEvents;

  public float CurrentEventDelay
  {
    get => this.mCurrentEventDelay;
    set => this.mCurrentEventDelay = value;
  }

  public bool LoopEvents
  {
    get => this.mLoopEvents;
    set => this.mLoopEvents = value;
  }

  public float WanderAngle
  {
    get => this.mWanderAngle;
    set => this.mWanderAngle = value;
  }

  public List<PathNode> Path => this.mPath;

  public float LeaderAge => this.mLeaderAge;

  public float AlertRadius
  {
    get => this.mAlertRadius;
    set => this.mAlertRadius = value;
  }

  public Order Order => this.mOrder;

  public Agent(NonPlayerCharacter iOwner)
  {
    this.mOwner = iOwner;
    this.mSpellRecovery = new CastSpell(0.0f, Target.Enemy, (Expression) null, new Magicka.Animations[1]
    {
      Magicka.Animations.attack_melee0
    }, 0.0f, 10f, 1f, 0.0f, 1f);
  }

  public void Initialize(NonPlayerCharacter iOwner, CharacterTemplate iTemplate)
  {
    this.mTargetAges.Clear();
    this.mTargets.Clear();
    this.mNextAbility = (Ability) null;
    this.mBusyAbility = (Ability) null;
    this.mOrder = Order.Attack;
    this.mReactsTo = ReactTo.None;
    this.mStates.Clear();
    this.mStates.Push((IAIState) AIStateIdle.Instance);
    this.mStateAge.Add(0.0f);
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
    this.mWanderAngle = (float) Agent.sRandom.NextDouble() * 6.28318548f;
  }

  public void Reset()
  {
    this.mOrder = Order.Attack;
    this.mReactsTo = ReactTo.None;
    this.mReactionOrder = Order.None;
    this.mReactionTrigger = 0;
    this.mBeenAttackedBy.Clear();
    this.mTargets.Clear();
    this.mTargetAges.Clear();
    this.mNextAbility = (Ability) null;
    this.mBusyAbility = (Ability) null;
    this.mPriorityTargetID = 0;
    this.mPriorityTarget = (IDamageable) null;
    this.mPath.Clear();
    this.mStates.Clear();
    this.mStateAge.Clear();
    this.mStates.Push((IAIState) AIStateIdle.Instance);
    this.mStateAge.Add(0.0f);
  }

  public void Disable() => AIManager.Instance.Agents.Remove(this);

  public void Enable()
  {
    AIManager instance = AIManager.Instance;
    if (instance.Agents.Contains(this))
      return;
    instance.Agents.Add(this);
  }

  public void ReduceAggro(float iDeltaTime)
  {
    for (int iIndex = 0; iIndex < this.mBeenAttackedBy.Count; ++iIndex)
    {
      KeyValuePair<ushort, float> keyValuePair = this.mBeenAttackedBy.GetKeyValuePair(iIndex);
      keyValuePair = new KeyValuePair<ushort, float>(keyValuePair.Key, Math.Min(keyValuePair.Value - 25f * iDeltaTime, 500f));
      bool flag = Entity.GetFromHandle((int) keyValuePair.Key) is Avatar fromHandle && (fromHandle.Equipment[1].Item.PassiveAbility.Ability == Item.PassiveAbilities.ReduceAggro || (this.Owner.Faction & Factions.UNDEAD) == Factions.UNDEAD && fromHandle.Equipment[1].Item.PassiveAbility.Ability == Item.PassiveAbilities.ZombieDeterrent);
      if ((double) keyValuePair.Value < 0.0)
        this.mBeenAttackedBy.Remove(keyValuePair.Key);
      else
        this.mBeenAttackedBy[keyValuePair.Key] = !flag ? keyValuePair.Value : keyValuePair.Value * 0.5f;
    }
  }

  public void UpdateTime(float iDeltaTime) => this.mTimeSinceLastUpdate += iDeltaTime;

  public void Update()
  {
    if (this.mOwner.Dead)
      return;
    this.ReduceAggro(this.mTimeSinceLastUpdate);
    this.mLeaderAge += this.mTimeSinceLastUpdate;
    for (int index1 = 0; index1 < this.mTargetAges.Count; ++index1)
    {
      List<float> mTargetAges;
      int index2;
      (mTargetAges = this.mTargetAges)[index2 = index1] = mTargetAges[index2] + this.mTimeSinceLastUpdate;
    }
    for (int index3 = 0; index3 < this.mStateAge.Count; ++index3)
    {
      List<float> mStateAge;
      int index4;
      (mStateAge = this.mStateAge)[index4 = index3] = mStateAge[index4] + this.mTimeSinceLastUpdate;
    }
    bool flag = false;
    Character character1 = (Character) null;
    if ((this.mReactsTo & ReactTo.Attack) == ReactTo.Attack)
    {
      float num = 0.0f;
      for (int iIndex = 0; iIndex < this.mBeenAttackedBy.Count; ++iIndex)
      {
        KeyValuePair<ushort, float> keyValuePair = this.mBeenAttackedBy.GetKeyValuePair(iIndex);
        if ((double) keyValuePair.Value > (double) num)
          character1 = (Character) Entity.GetFromHandle((int) keyValuePair.Key);
      }
      flag = character1 != null;
    }
    if (!flag && (this.mReactsTo & ReactTo.Proximity) == ReactTo.Proximity)
    {
      List<Entity> entities = this.Owner.PlayState.EntityManager.GetEntities(this.Owner.Position, this.mAlertRadius, false);
      for (int index = 0; index < entities.Count; ++index)
      {
        if (entities[index] is Character character2 && (this.Owner.Faction & character2.Faction) == Factions.NONE && !character2.IsInvisibile | this.mOwner.CanSeeInvisible)
        {
          flag = true;
          if (this.mReactionOrder != Order.None && this.mReactionOrder != this.mOrder)
            this.mAlertMode = AlertMode.Danger;
        }
      }
      this.Owner.PlayState.EntityManager.ReturnEntityList(entities);
    }
    if (flag)
      this.React();
    this.mStates.Peek().OnExecute((IAI) this, this.mTimeSinceLastUpdate);
    this.NextAbility?.Update(this, this.mTimeSinceLastUpdate);
    this.mTimeSinceLastUpdate = 0.0f;
  }

  public float ChooseTarget(out IDamageable oTarget, out Ability oAbility)
  {
    this.mPriorityTarget = Entity.GetByID(this.mPriorityTargetID) as IDamageable;
    for (int index = 0; index < this.mFuzzySortValues.Length; ++index)
    {
      this.mFuzzySortEntities[index] = (IDamageable) null;
      this.mFuzzySortValues[index] = float.MinValue;
    }
    List<Entity> entities = this.Owner.PlayState.EntityManager.GetEntities(this.Owner.Position, 40f, false);
    foreach (Player player in Magicka.Game.Instance.Players)
    {
      Avatar avatar = player.Avatar;
      if (!entities.Contains((Entity) avatar))
        entities.Add((Entity) avatar);
    }
    if (!entities.Contains(this.mPriorityTarget as Entity))
      entities.Add(this.mPriorityTarget as Entity);
    int length = this.mFuzzySortValues.Length;
    int num1 = length / 2;
    for (int index1 = 0; index1 < entities.Count; ++index1)
    {
      IDamageable iTarget = entities[index1] as IDamageable;
      Character character = entities[index1] as Character;
      if (iTarget != null && !(iTarget.Dead | iTarget == this.mOwner) && (iTarget == this.mPriorityTarget || iTarget is Character) && (character == null || !(character.IsInvisibile & !this.mOwner.CanSeeInvisible)))
      {
        float iDistance = Vector3.Distance(this.Owner.Body.Position, iTarget.Position);
        float num2 = 0.0f;
        if (iTarget == this.mPriorityTarget)
          ++num2;
        float num3 = 1f;
        if (iTarget is Character)
          num3 = (iTarget as Character).GetAgroMultiplier();
        float num4 = (num2 + FuzzyMath.FuzzyAnger(this, iTarget as Entity) * this.mAngerWeight + (float) (1.0 - (double) iTarget.HitPoints / (double) iTarget.MaxHitPoints) * this.mHealthWeight + FuzzyMath.FuzzyDistanceExponential(iDistance, this.mAlertRadius) * this.mDistanceWeight) * num3;
        if (character != null && (character.Faction & this.mOwner.Faction) == Factions.NONE)
        {
          for (int index2 = 0; index2 < num1; ++index2)
          {
            if ((double) num4 > (double) this.mFuzzySortValues[index2])
            {
              for (int index3 = num1 - 1; index3 > index2; --index3)
              {
                this.mFuzzySortEntities[index3] = this.mFuzzySortEntities[index3 - 1];
                this.mFuzzySortValues[index3] = this.mFuzzySortValues[index3 - 1];
              }
              this.mFuzzySortEntities[index2] = iTarget;
              this.mFuzzySortValues[index2] = num4;
              break;
            }
          }
        }
        else
        {
          for (int index4 = num1; index4 < length; ++index4)
          {
            if ((double) num4 > (double) this.mFuzzySortValues[index4])
            {
              for (int index5 = length - 1; index5 > index4; --index5)
              {
                this.mFuzzySortEntities[index5] = this.mFuzzySortEntities[index5 - 1];
                this.mFuzzySortValues[index5] = this.mFuzzySortValues[index5 - 1];
              }
              this.mFuzzySortEntities[index4] = iTarget;
              this.mFuzzySortValues[index4] = num4;
              break;
            }
          }
        }
      }
    }
    this.Owner.PlayState.EntityManager.ReturnEntityList(entities);
    ExpressionArguments iArgs;
    iArgs.AI = this;
    iArgs.AIPos = this.mOwner.Position;
    iArgs.AIDir = this.mOwner.Direction;
    for (int index = 0; index < length; ++index)
    {
      IDamageable mFuzzySortEntity = this.mFuzzySortEntities[index];
      if (mFuzzySortEntity != null)
      {
        iArgs.Target = mFuzzySortEntity;
        iArgs.TargetPos = mFuzzySortEntity.Position;
        iArgs.TargetDir = mFuzzySortEntity.Body.Orientation.Forward;
        Vector3.Subtract(ref iArgs.TargetPos, ref iArgs.AIPos, out iArgs.Delta);
        iArgs.Distance = iArgs.Delta.Length();
        Vector3.Divide(ref iArgs.Delta, iArgs.Distance, out iArgs.DeltaNormalized);
        this.mFuzzySortValues[index] += this.ChooseAbility(ref iArgs, out this.mFuzzySortAbilities[index]);
      }
    }
    oTarget = (IDamageable) null;
    oAbility = (Ability) null;
    float num5 = float.MinValue;
    for (int index = 0; index < this.mFuzzySortValues.Length; ++index)
    {
      if ((double) this.mFuzzySortValues[index] > (double) num5)
      {
        oTarget = this.mFuzzySortEntities[index];
        oAbility = this.mFuzzySortAbilities[index];
        num5 = this.mFuzzySortValues[index];
      }
    }
    iArgs.Target = (IDamageable) this.mOwner;
    iArgs.TargetPos = this.mOwner.Position;
    iArgs.TargetDir = this.mOwner.Direction;
    iArgs.Delta = new Vector3();
    iArgs.Distance = 0.0f;
    iArgs.DeltaNormalized = new Vector3();
    Ability oAbility1;
    float num6 = this.ChooseAbility(ref iArgs, out oAbility1);
    if ((double) num6 > (double) num5)
    {
      num5 = num6;
      oAbility = oAbility1;
      oTarget = (IDamageable) this.mOwner;
    }
    if (oAbility != this.mSpellRecovery)
      this.mLastSpellAbility = oAbility as CastSpell;
    if (oTarget != null)
      this.mLastTarget = oTarget;
    return num5;
  }

  public float ChooseAbility(ref ExpressionArguments iArgs, out Ability oAbility)
  {
    float num = float.MinValue;
    oAbility = (Ability) null;
    Ability[] abilities = this.mOwner.Abilities;
    for (int index = 0; index < abilities.Length; ++index)
    {
      if ((double) this.mOwner.AbilityCooldown[index] <= 0.0)
      {
        Ability ability = abilities[index];
        Character target = iArgs.Target as Character;
        if (((ability.Target == Target.Self & iArgs.Target != this.mOwner ? 1 : 0) | (ability.Target == Target.Friendly ? 1 : 0) & (iArgs.Target == this.mOwner || target == null ? 1 : ((target.Faction & this.mOwner.Faction) == Factions.NONE ? 1 : 0)) | (ability.Target == Target.Enemy ? 1 : 0) & (target == null ? 0 : ((target.Faction & this.mOwner.Faction) != Factions.NONE ? 1 : 0))) == 0 && (this.mOwner.SpellQueue.Count <= 0 || ability is CastSpell && SpellManager.Equatable((ability as CastSpell).Elements, this.mOwner.SpellQueue)))
        {
          float desirability = ability.GetDesirability(ref iArgs);
          if (index == this.mPriorityAbilityIdx & iArgs.Target == this.mPriorityTarget)
            desirability += 0.5f;
          if ((double) desirability > 0.0 && (double) desirability > (double) num)
          {
            num = desirability;
            oAbility = ability;
          }
        }
      }
    }
    if (oAbility == null && this.mOwner.SpellQueue.Count > 0 && this.mLastSpellAbility != null && this.mNextAbility != this.mSpellRecovery)
    {
      Character target = iArgs.Target as Character;
      if (((this.mLastSpellAbility.Target == Target.Enemy ? 1 : 0) & (target == null ? 1 : ((target.Faction & this.mOwner.Faction) == Factions.NONE ? 1 : 0))) != 0)
      {
        this.mSpellRecovery.Animations = this.mLastSpellAbility.Animations;
        this.mSpellRecovery.CastType = this.mLastSpellAbility.CastType;
        this.mSpellRecovery.Target = this.mLastSpellAbility.Target;
        this.mSpellRecovery.Cooldown = this.mLastSpellAbility.Cooldown;
        Spell[] iArray = new Spell[this.mOwner.SpellQueue.Count];
        this.mOwner.SpellQueue.CopyTo(iArray, 0);
        Elements[] elementsArray = new Elements[this.mOwner.SpellQueue.Count];
        for (int index = 0; index < iArray.Length; ++index)
          elementsArray[index] = iArray[index].Element;
        this.mSpellRecovery.Elements = elementsArray;
        oAbility = (Ability) this.mSpellRecovery;
        num = 2.802597E-45f;
      }
    }
    return num;
  }

  internal void SetOrder(
    Order iGoal,
    ReactTo iReactTo,
    Order iReactionOrder,
    int iPriorityTarget,
    int iPriorityAbility,
    int iReactionTrigger,
    AIEvent[] iEvents)
  {
    if (this.mOrder != iGoal)
    {
      while (this.CurrentState != AIStateIdle.Instance)
      {
        if (this.CurrentState is AIStateAttack)
          this.ReleaseTarget();
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
      this.mCurrentEventDelay = 0.0f;
    this.mPriorityTargetID = iPriorityTarget;
    this.mPriorityTarget = Entity.GetByID(this.mPriorityTargetID) as IDamageable;
    this.mPriorityAbilityIdx = iPriorityAbility;
  }

  public IAIState CurrentState => this.mStates.Peek();

  public float CurrentStateAge => this.mStateAge[this.mStateAge.Count - 1];

  public ReactTo ReactsTo
  {
    get => this.mReactsTo;
    set => this.mReactsTo = value;
  }

  public Order ReactionOrder => this.mReactionOrder;

  public int ReactionTrigger => this.mReactionTrigger;

  public void PushState(IAIState iNewState)
  {
    IAIState aiState = this.mStates.Peek();
    if (iNewState == aiState)
      return;
    aiState.OnExit((IAI) this);
    this.mStates.Push(iNewState);
    this.mStateAge.Add(0.0f);
    iNewState.OnEnter((IAI) this);
  }

  public void PopState()
  {
    this.mStates.Peek().OnExit((IAI) this);
    this.mStates.Pop();
    this.mStateAge.RemoveAt(this.mStateAge.Count - 1);
    this.mStates.Peek().OnEnter((IAI) this);
  }

  public Character Owner => (Character) this.mOwner;

  public NonPlayerCharacter NPC => this.mOwner;

  public Vector3 WayPoint
  {
    get => this.mWayPoint;
    set => this.mWayPoint = value;
  }

  public SyncedList<ushort, float> AttackedBy => this.mBeenAttackedBy;

  public void Alarm(Character iTarget)
  {
    if (this.mReactionOrder == Order.None || this.mReactionOrder == this.mOrder)
      return;
    this.mAlertMode = AlertMode.Danger;
  }

  public void AddAttackedBy(Character iAttacker, float iAmount)
  {
    float num1;
    this.mBeenAttackedBy.TryGetValue(iAttacker.Handle, out num1);
    float num2 = (iAttacker.Faction & this.Owner.Faction) == Factions.NONE ? num1 + iAmount : num1 + iAmount * 0.0f;
    this.mBeenAttackedBy[iAttacker.Handle] = num2;
    if ((double) this.mBeenAttackedBy[iAttacker.Handle] > 1.4012984643248171E-45)
    {
      if (((double) this.mBeenAttackedBy[iAttacker.Handle] <= 1.4012984643248171E-45 || this.Owner.Faction != Factions.FRIENDLY && this.Owner.Faction != Factions.NEUTRAL) && !(iAttacker is Avatar) || (this.Owner.Faction & Factions.FRIENDLY & Factions.NEUTRAL) == Factions.NONE && !(iAttacker is Avatar))
        return;
      List<Entity> entities = this.Owner.PlayState.EntityManager.GetEntities(this.Owner.Position, this.mAlertRadius, false);
      for (int index = 0; index < entities.Count; ++index)
      {
        if ((entities[index] != null || entities[index] == this.Owner) && entities[index] is NonPlayerCharacter && (this.Owner.Faction & (entities[index] as NonPlayerCharacter).Faction) != Factions.NONE)
        {
          float num3 = iAmount;
          NonPlayerCharacter nonPlayerCharacter = entities[index] as NonPlayerCharacter;
          if ((iAttacker.Faction & nonPlayerCharacter.Faction) != Factions.NONE)
            num3 *= 0.0f;
          if (nonPlayerCharacter.AI != null && (double) num3 > 1.4012984643248171E-45)
          {
            nonPlayerCharacter.AI.AttackedBy.TryGetValue(iAttacker.Handle, out num2);
            num2 += num3;
            nonPlayerCharacter.AI.AttackedBy[iAttacker.Handle] = num2;
          }
        }
      }
      this.Owner.PlayState.EntityManager.ReturnEntityList(entities);
    }
    else
      this.mBeenAttackedBy.Remove(iAttacker.Handle);
  }

  public int LastVisitedNode
  {
    get => this.mLastVisitedNode;
    set => this.mLastVisitedNode = value;
  }

  public void HandleMessage(Magicka.AI.Messaging.Message iMessage)
  {
    switch (iMessage.MessageType)
    {
      case MessageType.TakingDamage:
        this.AddAttackedBy(iMessage.Tag as Character, iMessage.Value);
        break;
    }
  }

  internal void React()
  {
    if (this.mReactionOrder != Order.None)
      this.mOrder = this.mReactionOrder;
    if (this.mReactionTrigger == 0)
      return;
    this.Owner.PlayState.Level.CurrentScene.ExecuteTrigger(this.mReactionTrigger, this.Owner, false);
  }

  public void ReleaseTarget()
  {
    if (this.mTargets.Count > 0)
    {
      this.mTargets.Pop();
      this.mTargetAges.RemoveAt(this.mTargetAges.Count - 1);
    }
    this.mNextAbility = (Ability) null;
  }

  public void AddTarget(IDamageable iDamageable, Ability iAbility)
  {
    if (iDamageable == this.CurrentTarget)
      return;
    if (iDamageable == null)
      throw new ArgumentException("iDamageable connot be null!", nameof (iDamageable));
    this.mTargets.Push(iDamageable);
    this.NextAbility = iAbility;
    this.mTargetAges.Add(0.0f);
  }

  public MovementProperties MoveAbilities => this.mOwner.MoveAbilities;

  public AlertMode AlertMode => this.mAlertMode;

  public Dictionary<byte, Magicka.Animations[]> MoveAnimations => this.mOwner.MoveAnimations;

  internal void ChangeTarget(IDamageable iDamageable, Ability iAbility)
  {
    if (iDamageable == null)
      return;
    this.mTargets.Pop();
    this.mTargets.Push(iDamageable);
    this.NextAbility = iAbility;
    this.mTargetAges[this.mTargetAges.Count - 1] = 0.0f;
  }

  public void GetAvoidance(out Vector3 oAvoidance)
  {
    Vector3 result1 = new Vector3();
    Vector3 result2 = new Vector3();
    Vector3 result3 = new Vector3();
    Vector3 result4 = new Vector3();
    Vector3 position1 = this.mOwner.Position;
    List<Entity> entities = this.mOwner.PlayState.EntityManager.GetEntities(position1, 12f, false);
    entities.Remove((Entity) this.mOwner);
    for (int index = 0; index < entities.Count; ++index)
    {
      Entity entity = entities[index];
      if (entity is Character)
      {
        Character character = entity as Character;
        if (!character.IsInvisibile)
        {
          if ((character.Faction & this.mOwner.Faction) != Factions.NONE)
          {
            Vector3 position2 = character.Position;
            Vector3 result5;
            Vector3.Subtract(ref position1, ref position2, out result5);
            float num = result5.Length();
            float scaleFactor = (double) num >= 9.9999999747524271E-07 ? (float) (1.0 - (double) num / 12.0) : 1f;
            result5.Normalize();
            Vector3.Multiply(ref result5, scaleFactor, out result5);
            Vector3.Multiply(ref result5, character.GetDanger(), out result5);
            Vector3.Add(ref result2, ref result5, out result2);
          }
          else
          {
            Vector3 position3 = character.Position;
            Vector3 direction = character.Direction;
            Vector3 result6;
            Vector3.Subtract(ref position1, ref position3, out result6);
            float num1 = result6.Length();
            float scaleFactor1;
            if ((double) num1 >= 9.9999999747524271E-07)
            {
              scaleFactor1 = (float) (1.0 - (double) num1 / 12.0);
              Vector3.Divide(ref result6, num1, out result6);
              Vector3 result7;
              Vector3.Cross(ref result6, ref direction, out result7);
              Vector3 result8;
              Vector3.Cross(ref result6, ref new Vector3()
              {
                Y = (double) result7.Y > 0.0 ? 1f : -1f
              }, out result8);
              result8.Normalize();
              float result9;
              Vector3.Dot(ref result6, ref direction, out result9);
              float num2 = num1 / 12f;
              float scaleFactor2 = (float) ((double) num2 * (double) num2 * (1.0 - (double) num2) * 6.75);
              result9 = (float) Math.Pow((double) MathHelper.Clamp(result9, 0.0f, 1f), 0.25);
              Vector3.Multiply(ref result8, result9, out result8);
              Vector3.Multiply(ref result8, scaleFactor2, out result8);
              Vector3.Add(ref result1, ref result8, out result1);
            }
            else
              scaleFactor1 = 1f;
            float danger = character.GetDanger();
            result6.Normalize();
            Vector3.Multiply(ref result6, scaleFactor1, out result6);
            Vector3.Multiply(ref result6, danger, out result6);
            Vector3.Add(ref result3, ref result6, out result3);
          }
        }
      }
      else
      {
        float danger = entity.GetDanger();
        Vector3 position4 = entity.Position;
        Vector3 result10;
        Vector3.Subtract(ref position1, ref position4, out result10);
        result10.Y = 0.0f;
        float num = result10.Length();
        float scaleFactor = (float) (1.0 - (double) num / 12.0);
        if ((double) num > 9.9999999747524271E-07)
          result10.Normalize();
        else
          result10 = new Vector3();
        Vector3.Multiply(ref result10, scaleFactor, out result10);
        Vector3.Multiply(ref result10, danger, out result10);
        Vector3.Add(ref result4, ref result10, out result4);
      }
    }
    this.mOwner.PlayState.EntityManager.ReturnEntityList(entities);
    Vector3.Multiply(ref result1, this.mSightAvoidance, out result1);
    Vector3.Multiply(ref result2, this.mFriendlyAvoidance, out result2);
    Vector3.Multiply(ref result3, this.mEnemyAvoidance, out result3);
    Vector3.Multiply(ref result4, this.mDangerAvoidance, out result4);
    Vector3.Add(ref result1, ref result2, out oAvoidance);
    Vector3.Add(ref result3, ref oAvoidance, out oAvoidance);
    Vector3.Add(ref result4, ref oAvoidance, out oAvoidance);
    float f = oAvoidance.Length();
    if (float.IsInfinity(f))
      oAvoidance = new Vector3();
    if ((double) f <= 100000.0)
      return;
    Vector3.Divide(ref oAvoidance, f / 100000f, out oAvoidance);
  }
}
