// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Bosses.Vlad2
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.AI;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using PolygonHead;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Bosses;

public class Vlad2 : IBoss
{
  private Random mRandom;
  private bool mDead;
  private VladCharacter mVlad;
  private PlayState mPlayState;
  private IBossState<Vlad2> mCurrentState;

  public Vlad2(PlayState iPlayState)
  {
    this.mPlayState = iPlayState;
    this.mRandom = new Random();
    iPlayState.Content.Load<CharacterTemplate>("Data/Characters/Boss_Vlad_Diplomat");
    this.mVlad = new VladCharacter(iPlayState);
  }

  public void Initialize(ref Matrix iOrientation, int iUniqueID)
  {
    this.Initialize(ref iOrientation);
  }

  public void Initialize(ref Matrix iOrientation)
  {
    this.mDead = false;
    CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate("boss_vlad_diplomat".GetHashCodeCustom());
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
    this.mCurrentState = (IBossState<Vlad2>) Vlad2.IntroState.Instance;
    this.mCurrentState.OnEnter(this);
  }

  protected void ChangeState(IBossState<Vlad2> iState)
  {
    this.mCurrentState.OnExit(this);
    this.mCurrentState = iState;
    this.mCurrentState.OnEnter(this);
  }

  public void UpdateBoss(DataChannel iDataChannel, float iDeltaTime, bool iFightStarted)
  {
    if (this.mCurrentState is Vlad2.IntroState && iFightStarted)
      this.ChangeState((IBossState<Vlad2>) Vlad2.BattleState.Instance);
    this.mCurrentState.OnUpdate(iDeltaTime, this);
  }

  public void DeInitialize()
  {
    this.mVlad.CannotDieWithoutExplicitKill = false;
    this.mVlad.Kill();
    this.mVlad.Terminate(true, false);
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

  public BossEnum GetBossType() => BossEnum.Vlad2;

  public bool NetworkInitialized => true;

  public float ResistanceAgainst(Elements iElement)
  {
    return this.mVlad != null ? this.mVlad.ResistanceAgainst(iElement) : 1f;
  }

  protected class IntroState : IBossState<Vlad2>
  {
    private static Vlad2.IntroState mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Vlad2.IntroState Instance
    {
      get
      {
        if (Vlad2.IntroState.mSingelton == null)
        {
          lock (Vlad2.IntroState.mSingeltonLock)
          {
            if (Vlad2.IntroState.mSingelton == null)
              Vlad2.IntroState.mSingelton = new Vlad2.IntroState();
          }
        }
        return Vlad2.IntroState.mSingelton;
      }
    }

    private IntroState()
    {
    }

    public void OnEnter(Vlad2 iOwner)
    {
      iOwner.mVlad.AI.SetOrder(Order.Idle, ReactTo.None, Order.Idle, 0, 0, 0, (AIEvent[]) null);
      iOwner.mVlad.Equipment[0].Item.Visible = true;
      iOwner.mVlad.Equipment[1].Item.Visible = false;
    }

    public void OnUpdate(float iDeltaTime, Vlad2 iOwner)
    {
    }

    public void OnExit(Vlad2 iOwner)
    {
    }
  }

  protected class BattleState : IBossState<Vlad2>
  {
    private const float TAUNT_TIME = 5f;
    private static Vlad2.BattleState mSingelton;
    private static volatile object mSingeltonLock = new object();
    private float mIdleTimer;

    public static Vlad2.BattleState Instance
    {
      get
      {
        if (Vlad2.BattleState.mSingelton == null)
        {
          lock (Vlad2.BattleState.mSingeltonLock)
          {
            if (Vlad2.BattleState.mSingelton == null)
              Vlad2.BattleState.mSingelton = new Vlad2.BattleState();
          }
        }
        return Vlad2.BattleState.mSingelton;
      }
    }

    private BattleState()
    {
    }

    public void OnEnter(Vlad2 iOwner)
    {
      iOwner.mVlad.AI.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, 0, 0, (AIEvent[]) null);
      this.mIdleTimer = (float) (5.0 + iOwner.mRandom.NextDouble() * 5.0);
    }

    public void OnUpdate(float iDeltaTime, Vlad2 iOwner)
    {
      this.mIdleTimer -= iDeltaTime;
      if ((double) iOwner.mVlad.Position.Y < -5.0)
      {
        iOwner.mVlad.Magick = (SpecialAbility) Teleport.Instance;
        iOwner.mVlad.Magick.Execute((ISpellCaster) iOwner.mVlad, iOwner.mPlayState);
      }
      if (!iOwner.mVlad.HasStatus(StatusEffects.Frozen))
        return;
      iOwner.mVlad.StopStatusEffects(StatusEffects.Wet | StatusEffects.Frozen | StatusEffects.Cold);
    }

    public void OnExit(Vlad2 iOwner)
    {
    }
  }

  protected class OutroState : IBossState<Vlad2>
  {
    private static Vlad2.OutroState mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Vlad2.OutroState Instance
    {
      get
      {
        if (Vlad2.OutroState.mSingelton == null)
        {
          lock (Vlad2.OutroState.mSingeltonLock)
          {
            if (Vlad2.OutroState.mSingelton == null)
              Vlad2.OutroState.mSingelton = new Vlad2.OutroState();
          }
        }
        return Vlad2.OutroState.mSingelton;
      }
    }

    private OutroState()
    {
    }

    public void OnEnter(Vlad2 iOwner)
    {
    }

    public void OnUpdate(float iDeltaTime, Vlad2 iOwner)
    {
    }

    public void OnExit(Vlad2 iOwner)
    {
    }
  }
}
