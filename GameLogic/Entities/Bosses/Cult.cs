// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Bosses.Cult
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.AI;
using Magicka.AI.AgentStates;
using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Levels.Triggers;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PolygonHead;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#nullable disable
namespace Magicka.GameLogic.Entities.Bosses;

public class Cult : IBoss
{
  private const int SPAWN = 0;
  private const int SPECTATE = 1;
  private const int SPECTATOR_COUNT = 26;
  private static readonly int DIALOG_INTRO = "cult_intro".GetHashCodeCustom();
  private static readonly int DIALOG_REVIVE = "cult_domyself".GetHashCodeCustom();
  private static readonly int DIALOG_TAUNT1 = "cult_taunt1".GetHashCodeCustom();
  private static readonly int DIALOG_TAUNT2 = "cult_taunt2".GetHashCodeCustom();
  private static readonly int DIALOG_DEATH = "cult_death".GetHashCodeCustom();
  private static readonly int DEFEATED_TRIGGER = "boss_defeated".GetHashCodeCustom();
  private static readonly Random sRandom = new Random();
  private static readonly int[,] WARLOCK_POSITIONS = new int[5, 2]
  {
    {
      "spawn_warlock5".GetHashCodeCustom(),
      "spectate_warlock5".GetHashCodeCustom()
    },
    {
      "spawn_warlock1".GetHashCodeCustom(),
      "spectate_warlock1".GetHashCodeCustom()
    },
    {
      "spawn_warlock2".GetHashCodeCustom(),
      "spectate_warlock2".GetHashCodeCustom()
    },
    {
      "spawn_warlock3".GetHashCodeCustom(),
      "spectate_warlock3".GetHashCodeCustom()
    },
    {
      "spawn_warlock4".GetHashCodeCustom(),
      "spectate_warlock4".GetHashCodeCustom()
    }
  };
  private Vector3[][] mWarlockPosition;
  private Vector3[][] mWarlockDirection;
  private float mTotalMaxHitPoints;
  private float mTotalHitPoints;
  private bool mDead;
  private Cult.IntroState mIntroState;
  private Cult.BattleState mBattleState;
  private Cult.ReviveState mReviveState;
  private Cult.SpectatorState mSpectatorState;
  private Cult.FinalState mFinalState;
  private Cult.DeadState mDeadState;
  private AIEvent[] mLeaderReviveMoveEvent;
  private AIEvent[] mLeaderReviveAnimation;
  private NonPlayerCharacter mLeaderWarlock;
  private List<NonPlayerCharacter> mWarlocks;
  private List<NonPlayerCharacter> mSpectators;
  private List<NonPlayerCharacter> mFightingGoblins;
  private Player[] mPlayers;
  private PlayState mPlayState;
  private IBossState<Cult> mCurrentState;
  private CharacterTemplate[] mTemplates;
  private AudioEmitter mTeleportEmitter;
  private int[] mReviveeTemplateType;
  private bool mNetworkInitialized;

  public unsafe Cult(PlayState iPlayState)
  {
    this.mPlayState = iPlayState;
    this.mTemplates = new CharacterTemplate[5];
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      this.mTemplates[0] = this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/Boss_Cult_Cold");
      this.mTemplates[1] = this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/Boss_Cult_Fire");
      this.mTemplates[2] = this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/Boss_Cult_Lightning");
      this.mTemplates[3] = this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/Boss_Cult_Nature");
      this.mTemplates[4] = this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/Boss_Cult_Leader");
    }
    this.mWarlocks = new List<NonPlayerCharacter>(26);
    this.mSpectators = new List<NonPlayerCharacter>(26);
    this.mFightingGoblins = new List<NonPlayerCharacter>(26);
    if (NetworkManager.Instance.State != NetworkState.Client)
    {
      for (int index = 0; index < 26; ++index)
      {
        NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(this.mPlayState);
        this.mWarlocks.Add(instance);
        this.mSpectators.Add(instance);
      }
      this.mLeaderWarlock = NonPlayerCharacter.GetInstance(this.mPlayState);
      if (NetworkManager.Instance.State == NetworkState.Server)
      {
        Cult.InitializeMessage initializeMessage = new Cult.InitializeMessage();
        initializeMessage.LeaderHandle = this.mLeaderWarlock.Handle;
        for (int index = 0; index < 26; ++index)
          initializeMessage.Handles[index] = this.mWarlocks[index].Handle;
        BossFight.Instance.SendInitializeMessage<Cult.InitializeMessage>((IBoss) this, (ushort) 4, (void*) &initializeMessage);
      }
    }
    this.mIntroState = new Cult.IntroState();
    this.mBattleState = new Cult.BattleState();
    this.mReviveState = new Cult.ReviveState();
    this.mSpectatorState = new Cult.SpectatorState();
    this.mFinalState = new Cult.FinalState();
    this.mDeadState = new Cult.DeadState();
    this.mTeleportEmitter = new AudioEmitter();
    this.mNetworkInitialized = NetworkManager.Instance.State != NetworkState.Client;
  }

  public void Initialize(ref Matrix iOrientation, int iUniqueID)
  {
    this.Initialize(ref iOrientation);
  }

  public void Initialize(ref Matrix iOrientation)
  {
    this.mWarlockDirection = new Vector3[5][];
    this.mWarlockPosition = new Vector3[5][];
    for (int index = 0; index < this.mWarlockPosition.Length; ++index)
    {
      this.mWarlockPosition[index] = new Vector3[2];
      this.mWarlockDirection[index] = new Vector3[2];
      Matrix oLocator;
      this.mPlayState.Level.CurrentScene.GetLocator(Cult.WARLOCK_POSITIONS[index, 0], out oLocator);
      this.mWarlockPosition[index][0] = oLocator.Translation;
      this.mWarlockDirection[index][0] = oLocator.Forward;
    }
    this.mLeaderReviveMoveEvent = new AIEvent[1];
    this.mLeaderReviveMoveEvent[0].EventType = AIEventType.Move;
    this.mLeaderReviveMoveEvent[0].MoveEvent.Direction = this.mWarlockDirection[0][0];
    this.mLeaderReviveMoveEvent[0].MoveEvent.Waypoint = this.mWarlockPosition[0][0];
    this.mLeaderReviveMoveEvent[0].MoveEvent.FixedDirection = true;
    this.mLeaderReviveMoveEvent[0].MoveEvent.Speed = 2f;
    this.mLeaderReviveMoveEvent[0].MoveEvent.Delay = 0.1f;
    this.mLeaderReviveAnimation = new AIEvent[1];
    this.mLeaderReviveAnimation[0].EventType = AIEventType.Animation;
    this.mLeaderReviveAnimation[0].AnimationEvent.Animation = Magicka.Animations.cast_spell0;
    this.mLeaderReviveAnimation[0].AnimationEvent.BlendTime = 0.25f;
    this.mLeaderReviveAnimation[0].AnimationEvent.Delay = 0.0f;
    this.mFightingGoblins.Clear();
    this.mSpectators.Clear();
    for (int index = 0; index < 26; ++index)
    {
      Matrix oLocator;
      this.mPlayState.Level.CurrentScene.GetLocator($"spectator{index}".GetHashCodeCustom(), out oLocator);
      Vector3 translation = oLocator.Translation;
      oLocator.Translation = new Vector3();
      if (index == 0)
        this.mWarlocks[index].Initialize(this.mTemplates[index % 4], translation, "bill".GetHashCodeCustom(), 2f);
      else if (index == 1)
        this.mWarlocks[index].Initialize(this.mTemplates[index % 4], translation, "bull".GetHashCodeCustom(), 2f);
      else
        this.mWarlocks[index].Initialize(this.mTemplates[index % 4], translation, 0, 2f);
      this.mWarlocks[index].Body.MoveTo(translation, oLocator);
      switch (index % 4)
      {
        case 0:
          this.mWarlocks[index].Color = Spell.COLDCOLOR;
          break;
        case 1:
          this.mWarlocks[index].Color = Spell.FIRECOLOR;
          break;
        case 2:
          this.mWarlocks[index].Color = Spell.LIGHTNINGCOLOR;
          break;
        case 3:
          this.mWarlocks[index].Color = Spell.LIFECOLOR;
          break;
      }
      this.mWarlocks[index].AI.SetOrder(Order.Idle, ReactTo.None, Order.Idle, 0, 0, 0, (AIEvent[]) null);
      this.mPlayState.EntityManager.AddEntity((Entity) this.mWarlocks[index]);
      this.mSpectators.Add(this.mWarlocks[index]);
      this.mTotalMaxHitPoints += this.mWarlocks[index].MaxHitPoints;
    }
    AIEvent[] iEvents = new AIEvent[1];
    iEvents[0].EventType = AIEventType.Animation;
    iEvents[0].AnimationEvent.Animation = Magicka.Animations.idlelong_agg;
    iEvents[0].AnimationEvent.BlendTime = 0.25f;
    iEvents[0].AnimationEvent.Delay = 0.0f;
    this.mLeaderWarlock.Initialize(this.mTemplates[4], Vector3.Zero, "leader".GetHashCodeCustom(), float.MaxValue);
    this.mLeaderWarlock.Body.MoveTo(this.mWarlockPosition[0][0], Matrix.CreateWorld(Vector3.Zero, this.mWarlockDirection[0][0], Vector3.Up));
    this.mLeaderWarlock.AI.SetOrder(Order.Idle, ReactTo.None, Order.Idle, 0, 0, 0, iEvents);
    this.mLeaderWarlock.Color = Vector3.Zero;
    this.mPlayState.EntityManager.AddEntity((Entity) this.mLeaderWarlock);
    this.mSpectators[0].Body.MoveTo(this.mWarlockPosition[1][0], Matrix.CreateWorld(Vector3.Zero, this.mWarlockDirection[1][0], Vector3.Up));
    this.mFightingGoblins.Add(this.mSpectators[0]);
    this.mSpectators.Remove(this.mSpectators[0]);
    this.mSpectators[0].Body.MoveTo(this.mWarlockPosition[4][0], Matrix.CreateWorld(Vector3.Zero, this.mWarlockDirection[4][0], Vector3.Up));
    this.mFightingGoblins.Add(this.mSpectators[0]);
    this.mSpectators.Remove(this.mSpectators[0]);
    this.mTotalMaxHitPoints += this.mLeaderWarlock.MaxHitPoints;
    this.mCurrentState = (IBossState<Cult>) this.mIntroState;
    this.mCurrentState.OnEnter(this);
    this.mPlayers = Magicka.Game.Instance.Players;
    this.mReviveeTemplateType = new int[5];
    this.mBattleState.mSpectatorShow = false;
    this.mDead = false;
    this.mSpectatorState.Reset();
  }

  private IBossState<Cult> GetState(Cult.States iState)
  {
    switch (iState)
    {
      case Cult.States.Intro:
        return (IBossState<Cult>) this.mIntroState;
      case Cult.States.Battle:
        return (IBossState<Cult>) this.mBattleState;
      case Cult.States.Revive:
        return (IBossState<Cult>) this.mReviveState;
      case Cult.States.Spectator:
        return (IBossState<Cult>) this.mSpectatorState;
      case Cult.States.Final:
        return (IBossState<Cult>) this.mFinalState;
      case Cult.States.Dead:
        return (IBossState<Cult>) this.mDeadState;
      default:
        return (IBossState<Cult>) null;
    }
  }

  protected unsafe void ChangeState(Cult.States iState)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    if (NetworkManager.Instance.State == NetworkState.Server)
    {
      Cult.ChangeStateMessage changeStateMessage;
      changeStateMessage.State = iState;
      BossFight.Instance.SendMessage<Cult.ChangeStateMessage>((IBoss) this, (ushort) 2, (void*) &changeStateMessage, true);
    }
    this.mCurrentState.OnExit(this);
    this.mCurrentState = this.GetState(iState);
    this.mCurrentState.OnEnter(this);
  }

  public void UpdateBoss(DataChannel iDataChannel, float iDeltaTime, bool iFightStarted)
  {
    if (this.mCurrentState is Cult.IntroState && iFightStarted)
      this.ChangeState(Cult.States.Battle);
    this.mTotalHitPoints = 0.0f;
    if (this.mCurrentState is Cult.FinalState)
    {
      for (int index = 0; index < this.mFightingGoblins.Count; ++index)
        this.mTotalHitPoints += Math.Max(this.mFightingGoblins[index].HitPoints, 0.0f);
    }
    else
    {
      for (int index = 0; index < this.mWarlocks.Count; ++index)
        this.mTotalHitPoints += Math.Max(this.mWarlocks[index].HitPoints, 0.0f);
      for (int index = 0; index < this.mFightingGoblins.Count; ++index)
      {
        if (this.mFightingGoblins[index] == null || this.mFightingGoblins[index].Dead)
          this.mFightingGoblins.RemoveAt(index--);
      }
    }
    this.mTotalHitPoints += Math.Max(this.mLeaderWarlock.HitPoints, 0.0f);
    if ((double) this.mLeaderWarlock.HitPoints <= 0.0 && !(this.mCurrentState is Cult.DeadState))
      this.ChangeState(Cult.States.Dead);
    this.mCurrentState.OnUpdate(iDeltaTime, this);
  }

  public unsafe void TeleportWarlock(NonPlayerCharacter iWarlock, int iLocator)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    Matrix oLocator;
    this.mPlayState.Level.CurrentScene.GetLocator(iLocator, out oLocator);
    if (NetworkManager.Instance.State == NetworkState.Server)
    {
      Cult.TeleportMessage teleportMessage;
      teleportMessage.Handle = iWarlock.Handle;
      teleportMessage.Position = oLocator.Translation;
      teleportMessage.Direction = oLocator.Forward;
      BossFight.Instance.SendMessage<Cult.ChangeStateMessage>((IBoss) this, (ushort) 2, (void*) &teleportMessage, true);
    }
    this.TeleportWarlock(iWarlock, oLocator.Translation, oLocator.Forward);
  }

  public void TeleportWarlock(NonPlayerCharacter iWarlock, Vector3 iPosition, Vector3 iDirection)
  {
    Vector3 position = iWarlock.Position;
    Matrix result = iWarlock.Body.Orientation;
    Vector3 right = Vector3.Right;
    VisualEffectReference oRef;
    EffectManager.Instance.StartEffect(Teleport.TELEPORT_EFFECT_DISAPPEAR, ref position, ref right, out oRef);
    this.mTeleportEmitter.Position = position;
    this.mTeleportEmitter.Up = Vector3.Up;
    this.mTeleportEmitter.Forward = result.Forward;
    AudioManager.Instance.PlayCue(Banks.Spells, Teleport.TELEPORT_SOUND_ORIGIN, this.mTeleportEmitter);
    Vector3 up = Vector3.Up;
    Vector3 zero = Vector3.Zero;
    Matrix.CreateWorld(ref zero, ref iDirection, ref up, out result);
    Vector3 iPosition1 = iPosition;
    Segment iSeg = new Segment();
    iSeg.Origin = iPosition1;
    iSeg.Delta.Y -= 5f;
    Vector3 oPos;
    if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out Vector3 _, iSeg))
      iPosition1 = oPos;
    iPosition1.Y += (float) ((double) iWarlock.Capsule.Radius + (double) iWarlock.Capsule.Length * 0.5 + 0.10000000149011612);
    iWarlock.CharacterBody.MoveTo(iPosition1, result);
    EffectManager.Instance.StartEffect(Teleport.TELEPORT_EFFECT_APPEAR, ref iPosition1, ref right, out oRef);
    AudioManager.Instance.PlayCue(Banks.Spells, Teleport.TELEPORT_SOUND_DESTINATION, iWarlock.AudioEmitter);
  }

  public void DeInitialize()
  {
  }

  public bool Dead => this.mDead;

  public float MaxHitPoints => this.mTotalMaxHitPoints;

  public float HitPoints => this.mTotalHitPoints;

  public void ScriptMessage(BossMessages iMessage)
  {
  }

  public bool AddImpulseVelocity(
    Vector3 iDirection,
    float iElevation,
    float iMassPower,
    float iDistance)
  {
    throw new NotImplementedException();
  }

  public DamageResult Damage(
    int iPartIndex,
    Magicka.GameLogic.Damage iDamage,
    Entity iAttacker,
    ref Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
    throw new NotImplementedException();
  }

  public void Damage(int iPartIndex, float iDamage, Elements iElement)
  {
    throw new NotImplementedException();
  }

  public void SetSlow(int iIndex) => throw new NotImplementedException();

  public void GetRandomPositionOnCollisionSkin(int iIndex, out Vector3 oPosition)
  {
    throw new NotImplementedException();
  }

  public bool HasStatus(int iIndex, StatusEffects iStatus) => throw new NotImplementedException();

  public float StatusMagnitude(int iIndex, StatusEffects iStatus)
  {
    throw new NotImplementedException();
  }

  public StatusEffect[] GetStatusEffects() => (StatusEffect[]) null;

  public unsafe void NetworkUpdate(ref BossUpdateMessage iMsg)
  {
    switch (iMsg.Type)
    {
      case 1:
        Cult.TeleportMessage teleportMessage;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &teleportMessage);
        NonPlayerCharacter fromHandle = Entity.GetFromHandle((int) teleportMessage.Handle) as NonPlayerCharacter;
        Vector3 position1 = fromHandle.Position;
        Matrix result = fromHandle.Body.Orientation;
        Vector3 right = Vector3.Right;
        VisualEffectReference oRef;
        EffectManager.Instance.StartEffect(Teleport.TELEPORT_EFFECT_DISAPPEAR, ref position1, ref right, out oRef);
        this.mTeleportEmitter.Position = position1;
        this.mTeleportEmitter.Up = Vector3.Up;
        this.mTeleportEmitter.Forward = result.Forward;
        AudioManager.Instance.PlayCue(Banks.Spells, Teleport.TELEPORT_SOUND_ORIGIN, this.mTeleportEmitter);
        Vector3 up = Vector3.Up;
        Vector3 zero = Vector3.Zero;
        Matrix.CreateWorld(ref zero, ref teleportMessage.Direction, ref up, out result);
        position1 = teleportMessage.Position;
        fromHandle.CharacterBody.MoveTo(position1, result);
        EffectManager.Instance.StartEffect(Teleport.TELEPORT_EFFECT_APPEAR, ref position1, ref right, out oRef);
        AudioManager.Instance.PlayCue(Banks.Spells, Teleport.TELEPORT_SOUND_DESTINATION, fromHandle.AudioEmitter);
        break;
      case 2:
        Cult.ChangeStateMessage changeStateMessage;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &changeStateMessage);
        this.mCurrentState.OnExit(this);
        this.mCurrentState = this.GetState(changeStateMessage.State);
        this.mCurrentState.OnEnter(this);
        break;
      case 3:
        Cult.ReviveMessage reviveMessage;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &reviveMessage);
        int index = reviveMessage.Index;
        Vector3 position2 = reviveMessage.Position;
        Vector3 direction = reviveMessage.Direction;
        this.mWarlocks[index].Initialize(this.mTemplates[reviveMessage.Index % 4], position2, 0, float.MaxValue);
        this.mWarlocks[index].AI.SetOrder(Order.Idle, ReactTo.None, Order.Idle, 0, 0, 0, (AIEvent[]) null);
        this.mWarlocks[index].CharacterBody.DesiredDirection = direction;
        switch (index % 4)
        {
          case 0:
            this.mWarlocks[index].Color = Spell.COLDCOLOR;
            break;
          case 1:
            this.mWarlocks[index].Color = Spell.FIRECOLOR;
            break;
          case 2:
            this.mWarlocks[index].Color = Spell.LIGHTNINGCOLOR;
            break;
          case 3:
            this.mWarlocks[index].Color = Spell.LIFECOLOR;
            break;
        }
        EffectManager.Instance.StartEffect(Revive.EFFECT, ref position2, ref direction, out VisualEffectReference _);
        AudioManager.Instance.PlayCue(Banks.Spells, Revive.SOUND_REVIVE_HASH, this.mWarlocks[index].AudioEmitter);
        if (!this.mPlayState.EntityManager.Entities.Contains((Entity) this.mWarlocks[index]))
          this.mPlayState.EntityManager.AddEntity((Entity) this.mWarlocks[index]);
        this.mFightingGoblins.Add(this.mWarlocks[index]);
        break;
    }
  }

  public unsafe void NetworkInitialize(ref BossInitializeMessage iMsg)
  {
    if (iMsg.Type != (ushort) 4)
      return;
    Cult.InitializeMessage initializeMessage;
    BossInitializeMessage.ConvertTo(ref iMsg, (void*) &initializeMessage);
    this.mLeaderWarlock = Entity.GetFromHandle((int) initializeMessage.LeaderHandle) as NonPlayerCharacter;
    for (int index = 0; index < 26; ++index)
    {
      this.mWarlocks.Add(Entity.GetFromHandle((int) initializeMessage.Handles[index]) as NonPlayerCharacter);
      this.mSpectators.Add(Entity.GetFromHandle((int) initializeMessage.Handles[index]) as NonPlayerCharacter);
    }
    this.mNetworkInitialized = true;
  }

  public BossEnum GetBossType() => BossEnum.Cult;

  public bool NetworkInitialized => this.mNetworkInitialized;

  public float ResistanceAgainst(Elements iElement) => 1f;

  public class IntroState : IBossState<Cult>
  {
    public void OnEnter(Cult iOwner)
    {
    }

    public void OnUpdate(float iDeltaTime, Cult iOwner)
    {
    }

    public void OnExit(Cult iOwner)
    {
    }
  }

  public class BattleState : IBossState<Cult>
  {
    private const float PAUSE_TIME = 2.5f;
    private float mPauseTimer;
    public bool mSpectatorShow;
    private TriggerArea mSpawnArea;

    public void OnEnter(Cult iOwner)
    {
      this.mSpawnArea = iOwner.mPlayState.Level.CurrentScene.GetTriggerArea("trigger_spawn_area".GetHashCodeCustom());
      this.mPauseTimer = 0.0f;
    }

    public void OnUpdate(float iDeltaTime, Cult iOwner)
    {
      bool flag = true;
      for (int index = 0; index < iOwner.mFightingGoblins.Count; ++index)
      {
        if (!iOwner.mFightingGoblins[index].Dead)
          flag = false;
      }
      if (!flag)
        return;
      if (this.mSpectatorShow)
      {
        if ((double) this.mPauseTimer <= 0.0)
        {
          this.mPauseTimer = 2.5f;
        }
        else
        {
          this.mPauseTimer -= iDeltaTime;
          if ((double) this.mPauseTimer <= 1.25 && (double) this.mPauseTimer > 0.0)
          {
            if (DialogManager.Instance.DialogActive(Cult.DIALOG_TAUNT1) || DialogManager.Instance.DialogActive(Cult.DIALOG_TAUNT2))
              return;
            DialogManager.Instance.StartDialog(Cult.sRandom.Next(2) == 0 ? Cult.DIALOG_TAUNT1 : Cult.DIALOG_TAUNT2, (Entity) iOwner.mLeaderWarlock, (Controller) null);
          }
          else
          {
            if ((double) this.mPauseTimer > 0.0)
              return;
            if (iOwner.mSpectators.Count > 0)
            {
              int num1 = Math.Min(iOwner.mSpectators.Count, 4);
              int index = 0;
              int num2 = 0;
              while (index < num1)
              {
                Matrix oLocator;
                iOwner.mPlayState.Level.CurrentScene.GetLocator(Cult.WARLOCK_POSITIONS[1 + num2, 0], out oLocator);
                iOwner.TeleportWarlock(iOwner.mSpectators[index], oLocator.Translation, oLocator.Forward);
                iOwner.mSpectators[index].AI.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, 0, 0, (AIEvent[]) null);
                iOwner.mFightingGoblins.Add(iOwner.mSpectators[index]);
                iOwner.mSpectators.Remove(iOwner.mSpectators[index]);
                int num3 = index - 1;
                --num1;
                index = num3 + 1;
                ++num2;
              }
            }
            else
              iOwner.ChangeState(Cult.States.Revive);
          }
        }
      }
      else
        iOwner.ChangeState(Cult.States.Spectator);
    }

    public void OnExit(Cult iOwner)
    {
    }
  }

  public class SpectatorState : IBossState<Cult>
  {
    private float mTimer;
    private bool mRelease;
    private bool mPan;
    private bool mBackPan;
    private bool mTeleported;
    private TriggerArea mSpawnArea;
    private bool mChangedState;

    public void OnEnter(Cult iOwner)
    {
      this.mSpawnArea = iOwner.mPlayState.Level.CurrentScene.GetTriggerArea("area_temp".GetHashCodeCustom());
      AIEvent[] aiEventArray = new AIEvent[1];
      aiEventArray[0].EventType = AIEventType.Animation;
      aiEventArray[0].AnimationEvent.Animation = Magicka.Animations.spec_alert3;
      aiEventArray[0].AnimationEvent.Delay = 0.1f;
      aiEventArray[0].AnimationEvent.BlendTime = 0.3f;
      iOwner.mPlayState.Camera.LockInput = true;
      this.mTimer = 0.0f;
      this.mPan = false;
      this.mChangedState = false;
    }

    public void OnUpdate(float iDeltaTime, Cult iOwner)
    {
      this.mTimer += iDeltaTime;
      if (this.mPan)
      {
        if ((double) this.mTimer <= 2.75)
          return;
        if (this.mTeleported && !this.mChangedState)
        {
          iOwner.mPlayState.Camera.Release(2f);
          iOwner.ChangeState(Cult.States.Battle);
          this.mChangedState = true;
          for (int index = 0; index < 4; ++index)
          {
            iOwner.mFightingGoblins.Add(iOwner.mSpectators[8]);
            iOwner.mSpectators.Remove(iOwner.mSpectators[8]);
          }
          for (int index = 0; index < iOwner.mFightingGoblins.Count; ++index)
            iOwner.mFightingGoblins[index].AI.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, 0, 0, (AIEvent[]) null);
        }
        else if (this.mRelease)
        {
          for (int index = 8; index < 12; ++index)
          {
            Matrix oLocator;
            iOwner.mPlayState.Level.CurrentScene.GetLocator(Cult.WARLOCK_POSITIONS[1 + index % 4, 0], out oLocator);
            iOwner.TeleportWarlock(iOwner.mSpectators[index], oLocator.Translation, oLocator.Forward);
          }
          this.mTeleported = true;
          this.mTimer -= 0.75f;
        }
        else if (this.mBackPan)
        {
          iOwner.mPlayState.Camera.MoveTo(iOwner.mWarlockPosition[0][0], 1.75f);
          this.mRelease = true;
          this.mTimer -= 2.25f;
        }
        else
        {
          Vector3 right = Vector3.Right;
          for (int index = 8; index < 12; ++index)
          {
            Vector3 randomLocation = this.mSpawnArea.GetRandomLocation();
            iOwner.TeleportWarlock(iOwner.mSpectators[index], randomLocation, right);
            iOwner.mSpectators[index].AI.SetOrder(Order.Idle, ReactTo.None, Order.Idle, 0, 0, 0, (AIEvent[]) null);
          }
          this.mBackPan = true;
          --this.mTimer;
        }
      }
      else
      {
        if ((double) this.mTimer <= 1.5)
          return;
        iOwner.mPlayState.Camera.MoveTo(iOwner.mSpectators[8].Position, 2f);
        this.mTimer = 0.0f;
        this.mPan = true;
      }
    }

    public void OnExit(Cult iOwner)
    {
      iOwner.mBattleState.mSpectatorShow = true;
      iOwner.mPlayState.Camera.LockInput = false;
    }

    public void Reset() => this.mTeleported = false;
  }

  public class ReviveState : IBossState<Cult>
  {
    public void OnEnter(Cult iOwner)
    {
      iOwner.TeleportWarlock(iOwner.mLeaderWarlock, iOwner.mLeaderReviveMoveEvent[0].MoveEvent.Waypoint, iOwner.mLeaderReviveMoveEvent[0].MoveEvent.Direction);
      iOwner.mLeaderWarlock.AI.SetOrder(Order.Idle, ReactTo.None, Order.Idle, 0, 0, 0, iOwner.mLeaderReviveMoveEvent);
      iOwner.mFightingGoblins.Clear();
      iOwner.mLeaderWarlock.CannotDieWithoutExplicitKill = true;
      ControlManager.Instance.LimitInput((object) this);
      iOwner.mPlayState.Camera.Follow((Entity) iOwner.mLeaderWarlock);
    }

    public unsafe void OnUpdate(float iDeltaTime, Cult iOwner)
    {
      if (NetworkManager.Instance.State == NetworkState.Client || iOwner.mLeaderWarlock.AI.Events != null && iOwner.mLeaderWarlock.AI.CurrentEvent < iOwner.mLeaderWarlock.AI.Events.Length)
        return;
      if (DialogManager.Instance.IsDialogDone(Cult.DIALOG_REVIVE, -1))
      {
        for (int index1 = 0; index1 < 5; ++index1)
        {
          int index2 = Cult.sRandom.Next(iOwner.mWarlocks.Count);
          while (!iOwner.mWarlocks[index2].Dead)
            index2 = Cult.sRandom.Next(iOwner.mWarlocks.Count);
          Vector3 position1 = iOwner.mWarlocks[index2].Position;
          Vector3 position2 = iOwner.mLeaderWarlock.Position;
          Vector3 result;
          Vector3.Subtract(ref position2, ref position1, out result);
          iOwner.mReviveeTemplateType[index1] = index2 % 4;
          iOwner.mWarlocks[index2].Initialize(iOwner.mTemplates[index2 % 4], position1, 0, float.MaxValue);
          iOwner.mWarlocks[index2].AI.SetOrder(Order.Idle, ReactTo.None, Order.Idle, 0, 0, 0, (AIEvent[]) null);
          iOwner.mWarlocks[index2].CharacterBody.DesiredDirection = result;
          switch (index2 % 4)
          {
            case 0:
              iOwner.mWarlocks[index2].Color = Spell.COLDCOLOR;
              break;
            case 1:
              iOwner.mWarlocks[index2].Color = Spell.FIRECOLOR;
              break;
            case 2:
              iOwner.mWarlocks[index2].Color = Spell.LIGHTNINGCOLOR;
              break;
            case 3:
              iOwner.mWarlocks[index2].Color = Spell.LIFECOLOR;
              break;
          }
          result.Normalize();
          EffectManager.Instance.StartEffect(Revive.EFFECT, ref position1, ref result, out VisualEffectReference _);
          AudioManager.Instance.PlayCue(Banks.Spells, Revive.SOUND_REVIVE_HASH, iOwner.mWarlocks[index2].AudioEmitter);
          if (!iOwner.mPlayState.EntityManager.Entities.Contains((Entity) iOwner.mWarlocks[index2]))
            iOwner.mPlayState.EntityManager.AddEntity((Entity) iOwner.mWarlocks[index2]);
          iOwner.mFightingGoblins.Add(iOwner.mWarlocks[index2]);
          if (NetworkManager.Instance.State == NetworkState.Server)
          {
            Cult.ReviveMessage reviveMessage;
            reviveMessage.Index = index2;
            reviveMessage.Position = iOwner.mWarlocks[index2].Position;
            reviveMessage.Direction = iOwner.mWarlocks[index2].Direction;
            BossFight.Instance.SendMessage<Cult.ReviveMessage>((IBoss) iOwner, (ushort) 3, (void*) &reviveMessage, true);
          }
        }
        ControlManager.Instance.UnlimitInput((object) this);
        iOwner.mPlayState.Camera.Release(1f);
        iOwner.ChangeState(Cult.States.Final);
      }
      else
      {
        if (DialogManager.Instance.DialogActive(Cult.DIALOG_REVIVE))
          return;
        iOwner.mLeaderWarlock.AI.SetOrder(Order.Idle, ReactTo.None, Order.Idle, 0, 0, 0, (AIEvent[]) null);
        DialogManager.Instance.StartDialog(Cult.DIALOG_REVIVE, (Entity) iOwner.mLeaderWarlock, (Controller) null);
      }
    }

    public void OnExit(Cult iOwner)
    {
      ControlManager.Instance.UnlimitInput((object) this);
      iOwner.mPlayState.Camera.Release(1f);
    }
  }

  public class FinalState : IBossState<Cult>
  {
    private const float REVIVE_COOLDOWN = 5f;
    private float mReviveCoolDown;
    private int mReviveIndex;

    public void OnEnter(Cult iOwner)
    {
      iOwner.mTotalMaxHitPoints = 0.0f;
      iOwner.mLeaderWarlock.AI.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, 0, 0, (AIEvent[]) null);
      iOwner.mTotalMaxHitPoints += iOwner.mLeaderWarlock.MaxHitPoints;
      for (int index = 0; index < iOwner.mFightingGoblins.Count; ++index)
      {
        iOwner.mTotalMaxHitPoints += iOwner.mFightingGoblins[index].MaxHitPoints;
        iOwner.mFightingGoblins[index].AI.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, 0, 0, (AIEvent[]) null);
      }
      this.mReviveIndex = -1;
    }

    public void OnUpdate(float iDeltaTime, Cult iOwner)
    {
      this.mReviveCoolDown -= iDeltaTime;
      if ((double) this.mReviveCoolDown >= 0.0)
        return;
      if (this.mReviveIndex < 0)
      {
        int num = Cult.sRandom.Next(iOwner.mFightingGoblins.Count);
        for (int index = num; index < num + iOwner.mFightingGoblins.Count; ++index)
        {
          if (iOwner.mFightingGoblins != null && iOwner.mFightingGoblins[index % iOwner.mFightingGoblins.Count].Dead)
          {
            this.mReviveIndex = index % iOwner.mFightingGoblins.Count;
            break;
          }
        }
      }
      else if (iOwner.mLeaderWarlock.AI.Events == null)
      {
        while (iOwner.mLeaderWarlock.AI.CurrentState != AIStateIdle.Instance)
        {
          if (iOwner.mLeaderWarlock.AI.CurrentState is AIStateAttack)
            iOwner.mLeaderWarlock.AI.ReleaseTarget();
          iOwner.mLeaderWarlock.AI.PopState();
        }
        iOwner.mLeaderWarlock.AI.SetOrder(Order.Idle, ReactTo.None, Order.Idle, 0, 0, 0, iOwner.mLeaderReviveAnimation);
      }
      else
      {
        if (iOwner.mLeaderWarlock.AI.CurrentEvent < iOwner.mLeaderWarlock.AI.Events.Length)
          return;
        Vector3 iPosition = iOwner.mWarlockPosition[this.mReviveIndex][0];
        Vector3 iDirection = iOwner.mWarlockDirection[this.mReviveIndex][0];
        iOwner.mFightingGoblins[this.mReviveIndex].Initialize(iOwner.mTemplates[iOwner.mReviveeTemplateType[this.mReviveIndex]], iPosition, 0, 2f);
        iOwner.mFightingGoblins[this.mReviveIndex].AI.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, 0, 0, (AIEvent[]) null);
        iOwner.mFightingGoblins[this.mReviveIndex].CharacterBody.DesiredDirection = iDirection;
        switch (this.mReviveIndex % 4)
        {
          case 0:
            iOwner.mFightingGoblins[this.mReviveIndex].Color = Spell.COLDCOLOR;
            break;
          case 1:
            iOwner.mFightingGoblins[this.mReviveIndex].Color = Spell.FIRECOLOR;
            break;
          case 2:
            iOwner.mFightingGoblins[this.mReviveIndex].Color = Spell.LIGHTNINGCOLOR;
            break;
          case 3:
            iOwner.mFightingGoblins[this.mReviveIndex].Color = Spell.LIFECOLOR;
            break;
        }
        EffectManager.Instance.StartEffect(Revive.EFFECT, ref iPosition, ref iDirection, out VisualEffectReference _);
        AudioManager.Instance.PlayCue(Banks.Spells, Revive.SOUND_REVIVE_HASH, iOwner.mFightingGoblins[this.mReviveIndex].AudioEmitter);
        if (!iOwner.mPlayState.EntityManager.Entities.Contains((Entity) iOwner.mFightingGoblins[this.mReviveIndex]))
          iOwner.mPlayState.EntityManager.AddEntity((Entity) iOwner.mFightingGoblins[this.mReviveIndex]);
        this.mReviveIndex = -1;
        this.mReviveCoolDown = 5f;
        iOwner.mLeaderWarlock.AI.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, 0, 0, (AIEvent[]) null);
      }
    }

    public void OnExit(Cult iOwner)
    {
    }
  }

  public class DeadState : IBossState<Cult>
  {
    private bool mTriggerExecuted;

    public void OnEnter(Cult iOwner)
    {
      DialogManager.Instance.StartDialog(Cult.DIALOG_DEATH, (Entity) iOwner.mLeaderWarlock, (Controller) null);
      this.mTriggerExecuted = false;
    }

    public void OnUpdate(float iDeltaTime, Cult iOwner)
    {
      if (this.mTriggerExecuted || !DialogManager.Instance.IsDialogDone(Cult.DIALOG_DEATH, -1))
        return;
      iOwner.mLeaderWarlock.CannotDieWithoutExplicitKill = false;
      iOwner.mLeaderWarlock.Kill();
      this.mTriggerExecuted = true;
      iOwner.mPlayState.Level.CurrentScene.ExecuteTrigger(Cult.DEFEATED_TRIGGER, (Magicka.GameLogic.Entities.Character) null, false);
    }

    public void OnExit(Cult iOwner)
    {
    }
  }

  public enum MessageType : ushort
  {
    Update,
    Teleport,
    ChangeState,
    Spawn,
    Initialize,
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  internal struct UpdateMessage
  {
    public const ushort TYPE = 0;
  }

  internal struct ReviveMessage
  {
    public const ushort TYPE = 3;
    public int Index;
    public Vector3 Position;
    public Vector3 Direction;
  }

  internal struct TeleportMessage
  {
    public const ushort TYPE = 1;
    public ushort Handle;
    public Vector3 Position;
    public Vector3 Direction;
  }

  internal struct ChangeStateMessage
  {
    public const ushort TYPE = 2;
    public Cult.States State;
  }

  internal struct InitializeMessage
  {
    public const ushort TYPE = 4;
    public unsafe fixed ushort Handles[26];
    public ushort LeaderHandle;
  }

  public enum States
  {
    Intro,
    Battle,
    Revive,
    Spectator,
    Final,
    Dead,
  }
}
