// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Bosses.Vlad3
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.AI;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using PolygonHead;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Bosses;

public class Vlad3 : IBoss
{
  private Random mRandom;
  private static readonly int DEATH_SOUND = "spell_arcane_area".GetHashCodeCustom();
  private static readonly int DEATH_EFFECT = "special_fairy_crash".GetHashCodeCustom();
  private bool mDead;
  private VladCharacter mVlad;
  private PlayState mPlayState;
  private IBossState<Vlad3> mCurrentState;

  public Vlad3(PlayState iPlayState)
  {
    this.mPlayState = iPlayState;
    this.mRandom = new Random();
    iPlayState.Content.Load<CharacterTemplate>("Data/Characters/Boss_Vlad_Spirit");
    this.mVlad = new VladCharacter(iPlayState);
  }

  public void Initialize(ref Matrix iOrientation) => this.Initialize(ref iOrientation, 0);

  public void Initialize(ref Matrix iOrientation, int iUniqueID)
  {
    this.mDead = false;
    CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate("boss_vlad_spirit".GetHashCodeCustom());
    Vector3 vector3 = iOrientation.Translation;
    iOrientation.Translation = new Vector3();
    Segment seg = new Segment();
    seg.Origin = vector3 + Vector3.Up;
    seg.Delta.Y -= 8f;
    Vector3 pos;
    if (this.mPlayState.Level.CurrentScene.CollisionSkin.SegmentIntersect(out float _, out pos, out Vector3 _, seg))
      vector3 = pos;
    this.mVlad.Initialize(cachedTemplate, vector3, iUniqueID);
    this.mVlad.CannotDieWithoutExplicitKill = true;
    vector3.Y += (float) ((double) this.mVlad.Capsule.Radius + (double) this.mVlad.Capsule.Length * 0.5 + 0.10000000149011612);
    this.mVlad.CharacterBody.MoveTo(vector3, iOrientation);
    this.mVlad.CharacterBody.DesiredDirection = iOrientation.Forward;
    this.mPlayState.EntityManager.AddEntity((Entity) this.mVlad);
    this.mCurrentState = (IBossState<Vlad3>) Vlad3.IntroState.Instance;
    this.mCurrentState.OnEnter(this);
  }

  protected void ChangeState(IBossState<Vlad3> iState)
  {
    this.mCurrentState.OnExit(this);
    this.mCurrentState = iState;
    this.mCurrentState.OnEnter(this);
  }

  public void UpdateBoss(DataChannel iDataChannel, float iDeltaTime, bool iFightStarted)
  {
    if (this.mCurrentState is Vlad3.IntroState && iFightStarted)
      this.ChangeState((IBossState<Vlad3>) Vlad3.BattleState.Instance);
    this.mCurrentState.OnUpdate(iDeltaTime, this);
  }

  public void DeInitialize()
  {
    this.mVlad.CannotDieWithoutExplicitKill = false;
    AudioManager.Instance.PlayCue(Banks.Spells, Vlad3.DEATH_SOUND);
    Vector3 position = this.mVlad.Position;
    Vector3 right = Vector3.Right;
    EffectManager.Instance.StartEffect(Vlad3.DEATH_EFFECT, ref position, ref right, out VisualEffectReference _);
    this.mVlad.Kill();
    this.mVlad.Terminate(true, false);
  }

  public void ScriptMessage(BossMessages iMessage)
  {
    if (iMessage != BossMessages.VladMortal || this.mVlad == null)
      return;
    double hitPoints = (double) this.mVlad.HitPoints;
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

  public BossEnum GetBossType() => BossEnum.Vlad3;

  public bool NetworkInitialized => true;

  public float ResistanceAgainst(Elements iElement)
  {
    return this.mVlad != null ? this.mVlad.ResistanceAgainst(iElement) : 1f;
  }

  protected class IntroState : IBossState<Vlad3>
  {
    private static Vlad3.IntroState mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Vlad3.IntroState Instance
    {
      get
      {
        if (Vlad3.IntroState.mSingelton == null)
        {
          lock (Vlad3.IntroState.mSingeltonLock)
          {
            if (Vlad3.IntroState.mSingelton == null)
              Vlad3.IntroState.mSingelton = new Vlad3.IntroState();
          }
        }
        return Vlad3.IntroState.mSingelton;
      }
    }

    private IntroState()
    {
    }

    public void OnEnter(Vlad3 iOwner)
    {
      iOwner.mVlad.AI.SetOrder(Order.Idle, ReactTo.None, Order.Idle, 0, 0, 0, (AIEvent[]) null);
      iOwner.mVlad.Equipment[0].Item.Visible = true;
      iOwner.mVlad.Equipment[1].Item.Visible = false;
    }

    public void OnUpdate(float iDeltaTime, Vlad3 iOwner)
    {
    }

    public void OnExit(Vlad3 iOwner)
    {
    }
  }

  protected class BattleState : IBossState<Vlad3>
  {
    private const float TAUNT_TIME = 5f;
    private static Vlad3.BattleState mSingelton;
    private static volatile object mSingeltonLock = new object();
    private float mIdleTimer;

    public static Vlad3.BattleState Instance
    {
      get
      {
        if (Vlad3.BattleState.mSingelton == null)
        {
          lock (Vlad3.BattleState.mSingeltonLock)
          {
            if (Vlad3.BattleState.mSingelton == null)
              Vlad3.BattleState.mSingelton = new Vlad3.BattleState();
          }
        }
        return Vlad3.BattleState.mSingelton;
      }
    }

    private BattleState()
    {
    }

    public void OnEnter(Vlad3 iOwner)
    {
      iOwner.mVlad.AI.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, 0, 0, (AIEvent[]) null);
      this.mIdleTimer = (float) (5.0 + iOwner.mRandom.NextDouble() * 5.0);
    }

    public void OnUpdate(float iDeltaTime, Vlad3 iOwner)
    {
      this.mIdleTimer -= iDeltaTime;
      if ((double) iOwner.mVlad.HitPoints <= 0.0)
      {
        iOwner.DeInitialize();
        iOwner.ChangeState((IBossState<Vlad3>) Vlad3.OutroState.Instance);
      }
      if ((double) iOwner.mVlad.Position.Y < -5.0)
      {
        iOwner.mVlad.Magick = (SpecialAbility) Teleport.Instance;
        iOwner.mVlad.Magick.Execute((ISpellCaster) iOwner.mVlad, iOwner.mPlayState);
      }
      if (!iOwner.mVlad.HasStatus(StatusEffects.Frozen))
        return;
      iOwner.mVlad.StopStatusEffects(StatusEffects.Wet | StatusEffects.Frozen | StatusEffects.Cold);
    }

    public void OnExit(Vlad3 iOwner)
    {
    }
  }

  protected class OutroState : IBossState<Vlad3>
  {
    private static Vlad3.OutroState mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Vlad3.OutroState Instance
    {
      get
      {
        if (Vlad3.OutroState.mSingelton == null)
        {
          lock (Vlad3.OutroState.mSingeltonLock)
          {
            if (Vlad3.OutroState.mSingelton == null)
              Vlad3.OutroState.mSingelton = new Vlad3.OutroState();
          }
        }
        return Vlad3.OutroState.mSingelton;
      }
    }

    private OutroState()
    {
    }

    public void OnEnter(Vlad3 iOwner)
    {
    }

    public void OnUpdate(float iDeltaTime, Vlad3 iOwner)
    {
    }

    public void OnExit(Vlad3 iOwner)
    {
    }
  }
}
