// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Bosses.Warlord
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.AI;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.UI;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Bosses;

public class Warlord : IBoss
{
  private const float TAUNT_COOLDOWN = 9f;
  private static readonly int DIALOG_DEATH_TALK = "warlorddeathtalk".GetHashCodeCustom();
  private static readonly int KHAN_ID = "#boss_n06".GetHashCodeCustom();
  private static readonly int[] DIALOG_TAUNTS = new int[3]
  {
    "warlordtaunt1".GetHashCodeCustom(),
    "warlordtaunt2".GetHashCodeCustom(),
    "warlordtaunt3".GetHashCodeCustom()
  };
  private float mTauntTimer;
  private Random mRandom;
  private WarlordCharacter mWarlord;
  private PlayState mPlayState;
  private IBossState<Warlord> mCurrentState;

  public Warlord(PlayState iPlayState)
  {
    this.mPlayState = iPlayState;
    this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/Boss_Warlord");
    this.mWarlord = new WarlordCharacter(iPlayState);
    this.mRandom = new Random();
  }

  public void Initialize(ref Matrix iOrientation, int iUniqueID)
  {
    this.Initialize(ref iOrientation);
  }

  public void Initialize(ref Matrix iOrientation)
  {
    CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate("boss_warlord".GetHashCodeCustom());
    Vector3 vector3 = iOrientation.Translation;
    iOrientation.Translation = new Vector3();
    Segment seg = new Segment();
    seg.Origin = vector3 + Vector3.Up;
    seg.Delta.Y -= 6f;
    Vector3 pos;
    if (this.mPlayState.Level.CurrentScene.CollisionSkin.SegmentIntersect(out float _, out pos, out Vector3 _, seg))
      vector3 = pos;
    vector3.Y += (float) ((double) this.mWarlord.Capsule.Radius + (double) this.mWarlord.Capsule.Length * 0.5 + 0.10000000149011612);
    this.mWarlord.Initialize(cachedTemplate, vector3, Warlord.KHAN_ID, float.MaxValue);
    this.mWarlord.CharacterBody.MoveTo(vector3, iOrientation);
    this.mWarlord.CharacterBody.DesiredDirection = iOrientation.Forward;
    this.mWarlord.CannotDieWithoutExplicitKill = true;
    this.mPlayState.EntityManager.AddEntity((Entity) this.mWarlord);
    this.mCurrentState = (IBossState<Warlord>) Warlord.IntroState.Instance;
    this.mCurrentState.OnEnter(this);
  }

  public void DeInitialize()
  {
  }

  public unsafe void UpdateBoss(DataChannel iDataChannel, float iDeltaTime, bool iFightStarted)
  {
    if (this.mCurrentState is Warlord.IntroState && iFightStarted && NetworkManager.Instance.State != NetworkState.Client)
    {
      if (NetworkManager.Instance.State == NetworkState.Server)
      {
        Warlord.ChangeStateMessage changeStateMessage;
        changeStateMessage.NewState = Warlord.State.Battle;
        BossFight.Instance.SendMessage<Warlord.ChangeStateMessage>((IBoss) this, (ushort) 0, (void*) &changeStateMessage, true);
      }
      this.ChangeState((IBossState<Warlord>) Warlord.BattleState.Instance);
    }
    if ((double) this.mWarlord.HitPoints <= 0.0 && this.mCurrentState != Warlord.DeathState.Instance && NetworkManager.Instance.State != NetworkState.Client)
    {
      if (NetworkManager.Instance.State == NetworkState.Server)
      {
        Warlord.ChangeStateMessage changeStateMessage;
        changeStateMessage.NewState = Warlord.State.Death;
        BossFight.Instance.SendMessage<Warlord.ChangeStateMessage>((IBoss) this, (ushort) 0, (void*) &changeStateMessage, true);
      }
      this.ChangeState((IBossState<Warlord>) Warlord.DeathState.Instance);
    }
    this.mCurrentState.OnUpdate(iDeltaTime, this);
  }

  public void ChangeState(IBossState<Warlord> iState)
  {
    this.mCurrentState.OnExit(this);
    this.mCurrentState = iState;
    this.mCurrentState.OnEnter(this);
  }

  public bool AddImpulseVelocity(
    Vector3 iDirection,
    float iElevation,
    float iMassPower,
    float iDistance)
  {
    throw new NotImplementedException();
  }

  public bool Dead => this.mWarlord.Dead;

  public float MaxHitPoints => this.mWarlord.MaxHitPoints;

  public float HitPoints => this.mWarlord.HitPoints;

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

  public StatusEffect[] GetStatusEffects() => throw new NotImplementedException();

  public void ScriptMessage(BossMessages iMessage)
  {
  }

  public unsafe void NetworkUpdate(ref BossUpdateMessage iMsg)
  {
    if (iMsg.Type != (ushort) 0)
      return;
    Warlord.ChangeStateMessage changeStateMessage;
    BossUpdateMessage.ConvertTo(ref iMsg, (void*) &changeStateMessage);
    switch (changeStateMessage.NewState)
    {
      case Warlord.State.Intro:
        this.ChangeState((IBossState<Warlord>) Warlord.IntroState.Instance);
        break;
      case Warlord.State.Battle:
        this.ChangeState((IBossState<Warlord>) Warlord.BattleState.Instance);
        break;
      case Warlord.State.Death:
        this.ChangeState((IBossState<Warlord>) Warlord.DeathState.Instance);
        break;
    }
  }

  public void NetworkInitialize(ref BossInitializeMessage iMsg)
  {
    throw new NotImplementedException();
  }

  public BossEnum GetBossType() => BossEnum.Khan;

  public bool NetworkInitialized => true;

  public float ResistanceAgainst(Elements iElement)
  {
    return this.mWarlord != null ? this.mWarlord.ResistanceAgainst(iElement) : 1f;
  }

  private enum MessageType : ushort
  {
    ChangeState,
  }

  internal struct ChangeStateMessage
  {
    public const ushort TYPE = 0;
    public Warlord.State NewState;
  }

  public enum State : byte
  {
    Intro,
    Battle,
    Death,
  }

  public class IntroState : IBossState<Warlord>
  {
    private static Warlord.IntroState mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Warlord.IntroState Instance
    {
      get
      {
        if (Warlord.IntroState.mSingelton == null)
        {
          lock (Warlord.IntroState.mSingeltonLock)
          {
            if (Warlord.IntroState.mSingelton == null)
              Warlord.IntroState.mSingelton = new Warlord.IntroState();
          }
        }
        return Warlord.IntroState.mSingelton;
      }
    }

    private IntroState()
    {
    }

    public void OnEnter(Warlord iOwner)
    {
    }

    public void OnUpdate(float iDeltaTime, Warlord iOwner)
    {
    }

    public void OnExit(Warlord iOwner)
    {
    }
  }

  public class BattleState : IBossState<Warlord>
  {
    private static Warlord.BattleState mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Warlord.BattleState Instance
    {
      get
      {
        if (Warlord.BattleState.mSingelton == null)
        {
          lock (Warlord.BattleState.mSingeltonLock)
          {
            if (Warlord.BattleState.mSingelton == null)
              Warlord.BattleState.mSingelton = new Warlord.BattleState();
          }
        }
        return Warlord.BattleState.mSingelton;
      }
    }

    private BattleState()
    {
    }

    public void OnEnter(Warlord iOwner)
    {
      iOwner.mWarlord.AI.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, 0, 0, (AIEvent[]) null);
    }

    public void OnUpdate(float iDeltaTime, Warlord iOwner)
    {
      if (NetworkManager.Instance.State == NetworkState.Client || NetworkManager.Instance.State != NetworkState.Server)
        return;
      iOwner.mTauntTimer -= iDeltaTime;
      if ((double) iOwner.mTauntTimer > 0.0)
        return;
      iOwner.mTauntTimer += 9f;
      DialogManager.Instance.StartDialog(Warlord.DIALOG_TAUNTS[iOwner.mRandom.Next(Warlord.DIALOG_TAUNTS.Length)], (Entity) iOwner.mWarlord, (Controller) null);
    }

    public void OnExit(Warlord iOwner)
    {
    }
  }

  public class DeathState : IBossState<Warlord>
  {
    private static Warlord.DeathState mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Warlord.DeathState Instance
    {
      get
      {
        if (Warlord.DeathState.mSingelton == null)
        {
          lock (Warlord.DeathState.mSingeltonLock)
          {
            if (Warlord.DeathState.mSingelton == null)
              Warlord.DeathState.mSingelton = new Warlord.DeathState();
          }
        }
        return Warlord.DeathState.mSingelton;
      }
    }

    private DeathState()
    {
    }

    public void OnEnter(Warlord iOwner)
    {
      iOwner.mWarlord.SpecialIdleAnimation = Magicka.Animations.idlelong_agg2;
    }

    public void OnUpdate(float iDeltaTime, Warlord iOwner)
    {
    }

    public void OnExit(Warlord iOwner)
    {
    }
  }
}
