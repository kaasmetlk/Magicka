// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Bosses.Vlad
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.AI;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.UI;
using Microsoft.Xna.Framework;
using PolygonHead;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Bosses;

public class Vlad : IBoss
{
  private static readonly int TRIGGER_INTRO = "trigger_enter".GetHashCodeCustom();
  private static readonly int DIALOG_OUTRO1 = "vladoutro1".GetHashCodeCustom();
  private static readonly int DIALOG_OUTRO2 = "vladoutro2".GetHashCodeCustom();
  private static readonly int[] DIALOG_BATTLE = new int[4]
  {
    "vladbattle1".GetHashCodeCustom(),
    "vladbattle2".GetHashCodeCustom(),
    "vladbattle3".GetHashCodeCustom(),
    "vladbattle4".GetHashCodeCustom()
  };
  private Random mRandom;
  private bool mDead;
  private VladCharacter mVlad;
  private PlayState mPlayState;
  private IBossState<Vlad> mCurrentState;

  public Vlad(PlayState iPlayState)
  {
    this.mPlayState = iPlayState;
    this.mRandom = new Random();
    iPlayState.Content.Load<CharacterTemplate>("Data/Characters/Boss_Vlad_Swamp");
    this.mVlad = new VladCharacter(iPlayState);
  }

  public void Initialize(ref Matrix iOrientation, int iUniqueID)
  {
    this.Initialize(ref iOrientation);
  }

  public void Initialize(ref Matrix iOrientation)
  {
    this.mDead = false;
    CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate("boss_vlad_swamp".GetHashCodeCustom());
    Vector3 vector3 = iOrientation.Translation;
    iOrientation.Translation = new Vector3();
    Segment seg = new Segment();
    seg.Origin = vector3 + Vector3.Up;
    seg.Delta.Y -= 8f;
    Vector3 pos;
    if (this.mPlayState.Level.CurrentScene.CollisionSkin.SegmentIntersect(out float _, out pos, out Vector3 _, seg))
      vector3 = pos;
    this.mVlad.Initialize(cachedTemplate, vector3, 0);
    this.mVlad.CannotDieWithoutExplicitKill = true;
    vector3.Y += (float) ((double) this.mVlad.Capsule.Radius + (double) this.mVlad.Capsule.Length * 0.5 + 0.10000000149011612);
    this.mVlad.CharacterBody.MoveTo(vector3, iOrientation);
    this.mVlad.CharacterBody.DesiredDirection = iOrientation.Forward;
    this.mPlayState.EntityManager.AddEntity((Entity) this.mVlad);
    this.mCurrentState = (IBossState<Vlad>) Vlad.IntroState.Instance;
    this.mCurrentState.OnEnter(this);
  }

  protected void ChangeState(IBossState<Vlad> iState)
  {
    this.mCurrentState.OnExit(this);
    this.mCurrentState = iState;
    this.mCurrentState.OnEnter(this);
  }

  public void UpdateBoss(DataChannel iDataChannel, float iDeltaTime, bool iFightStarted)
  {
    if (this.mCurrentState is Vlad.IntroState && iFightStarted)
      this.ChangeState((IBossState<Vlad>) Vlad.BattleState.Instance);
    this.mCurrentState.OnUpdate(iDeltaTime, this);
  }

  public void DeInitialize()
  {
    this.mVlad.CannotDieWithoutExplicitKill = false;
    this.mVlad.Kill();
  }

  public void ScriptMessage(BossMessages iMessage)
  {
    if (iMessage != BossMessages.VladMortal || this.mVlad == null)
      return;
    this.mVlad.CannotDieWithoutExplicitKill = false;
    this.mVlad.Kill();
    this.mVlad.Terminate(true, false);
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

  public bool AddImpulseVelocity(
    Vector3 iDirection,
    float iElevation,
    float iMassPower,
    float iDistance)
  {
    throw new NotImplementedException();
  }

  public bool Dead => this.mDead;

  public float MaxHitPoints => this.mVlad.MaxHitPoints;

  public float HitPoints => this.mVlad.HitPoints;

  public StatusEffect[] GetStatusEffects() => throw new NotImplementedException();

  public void NetworkUpdate(ref BossUpdateMessage iMsg)
  {
  }

  public void NetworkInitialize(ref BossInitializeMessage iMsg)
  {
    throw new NotImplementedException();
  }

  public BossEnum GetBossType() => BossEnum.Vlad;

  public bool NetworkInitialized => true;

  public float ResistanceAgainst(Elements iElement)
  {
    return this.mVlad != null ? this.mVlad.ResistanceAgainst(iElement) : 1f;
  }

  protected class IntroState : IBossState<Vlad>
  {
    private static Vlad.IntroState mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Vlad.IntroState Instance
    {
      get
      {
        if (Vlad.IntroState.mSingelton == null)
        {
          lock (Vlad.IntroState.mSingeltonLock)
          {
            if (Vlad.IntroState.mSingelton == null)
              Vlad.IntroState.mSingelton = new Vlad.IntroState();
          }
        }
        return Vlad.IntroState.mSingelton;
      }
    }

    private IntroState()
    {
    }

    public void OnEnter(Vlad iOwner)
    {
      iOwner.mVlad.AI.SetOrder(Order.Idle, ReactTo.None, Order.Idle, 0, 0, 0, (AIEvent[]) null);
      iOwner.mVlad.Equipment[0].Item.Visible = false;
      iOwner.mVlad.Equipment[1].Item.Visible = false;
    }

    public void OnUpdate(float iDeltaTime, Vlad iOwner)
    {
    }

    public void OnExit(Vlad iOwner)
    {
    }
  }

  protected class BattleState : IBossState<Vlad>
  {
    private const float TAUNT_TIME = 5f;
    private static Vlad.BattleState mSingelton;
    private static volatile object mSingeltonLock = new object();
    private float mIdleTimer;

    public static Vlad.BattleState Instance
    {
      get
      {
        if (Vlad.BattleState.mSingelton == null)
        {
          lock (Vlad.BattleState.mSingeltonLock)
          {
            if (Vlad.BattleState.mSingelton == null)
              Vlad.BattleState.mSingelton = new Vlad.BattleState();
          }
        }
        return Vlad.BattleState.mSingelton;
      }
    }

    private BattleState()
    {
    }

    public void OnEnter(Vlad iOwner)
    {
      iOwner.mVlad.AI.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, 0, 0, (AIEvent[]) null);
      this.mIdleTimer = (float) (5.0 + iOwner.mRandom.NextDouble() * 5.0);
    }

    public void OnUpdate(float iDeltaTime, Vlad iOwner)
    {
      this.mIdleTimer -= iDeltaTime;
      if ((double) iOwner.mVlad.HitPoints <= 0.0)
      {
        iOwner.ChangeState((IBossState<Vlad>) Vlad.OutroState.Instance);
      }
      else
      {
        if ((double) this.mIdleTimer > 0.0)
          return;
        DialogManager.Instance.StartDialog(Vlad.DIALOG_BATTLE[iOwner.mRandom.Next(Vlad.DIALOG_BATTLE.Length)], (Entity) iOwner.mVlad, (Controller) null);
        this.mIdleTimer = (float) (5.0 + iOwner.mRandom.NextDouble() * 5.0);
      }
    }

    public void OnExit(Vlad iOwner)
    {
    }
  }

  protected class OutroState : IBossState<Vlad>
  {
    private static Vlad.OutroState mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Vlad.OutroState Instance
    {
      get
      {
        if (Vlad.OutroState.mSingelton == null)
        {
          lock (Vlad.OutroState.mSingeltonLock)
          {
            if (Vlad.OutroState.mSingelton == null)
              Vlad.OutroState.mSingelton = new Vlad.OutroState();
          }
        }
        return Vlad.OutroState.mSingelton;
      }
    }

    private OutroState()
    {
    }

    public void OnEnter(Vlad iOwner)
    {
    }

    public void OnUpdate(float iDeltaTime, Vlad iOwner)
    {
    }

    public void OnExit(Vlad iOwner)
    {
    }
  }
}
