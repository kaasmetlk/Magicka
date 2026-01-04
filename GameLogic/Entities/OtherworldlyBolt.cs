// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.OtherworldlyBolt
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PolygonHead;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities;

internal class OtherworldlyBolt : Entity
{
  private Avatar mCurrentTarget;
  private VisualEffectReference mEffectRef;
  private static readonly int EFFECT_HIT = "cthulhu_otherworldly_bolt_hit".GetHashCodeCustom();
  private static readonly int EFFECT = "cthulhu_otherworldly_bolt".GetHashCodeCustom();
  private static readonly int STARSPAWN_HASH = "starspawn".GetHashCodeCustom();
  private float mAcceleration;
  private float mTerminalVelocity;
  private Vector3 mPosition;
  private Vector3 mVelocity;
  private float mCheckTargetTimer;
  private bool mRemovable;
  private bool mDone;
  private bool mOkToStart;
  private static readonly int SOUND_SPAWN = "cthulhu_bolt_spawn".GetHashCodeCustom();
  private static readonly int SOUND_IDLE = "cthulhu_bolt_idle".GetHashCodeCustom();
  private static readonly int SOUND_HIT = "cthulhu_bolt_hit".GetHashCodeCustom();
  private Cue mSoundCueIdle;
  private Vector3 mToTarget;

  public OtherworldlyBolt(PlayState iPlayState)
    : base(iPlayState)
  {
    OtherworldlyDischarge.Instance.Initialize(iPlayState);
    this.mBody = new Body();
    this.mBody.ApplyGravity = false;
    this.mBody.Immovable = true;
    this.mCollision = new CollisionSkin(this.mBody);
    this.mCollision.AddPrimitive((Primitive) new Sphere(Vector3.Zero, 0.5f), 1, new MaterialProperties(0.0f, 0.0f, 0.0f));
    this.mBody.CollisionSkin = this.mCollision;
    this.mBody.CollisionSkin.callbackFn += new CollisionCallbackFn(this.OnCollision);
  }

  public void Spawn(
    PlayState iPlayState,
    ref Vector3 iSpawnPosition,
    ref Vector3 iSpawnDirection,
    float iSpeed)
  {
    this.mPlayState = iPlayState;
    this.mPlayState.EntityManager.AddEntity((Entity) this);
    this.mPosition = iSpawnPosition;
    Matrix identity = Matrix.Identity;
    this.mBody.MoveTo(ref this.mPosition, ref identity);
    this.mCheckTargetTimer = 0.0f;
    this.mAcceleration = iSpeed;
    this.mTerminalVelocity = iSpeed;
    this.mVelocity = iSpawnDirection * iSpeed;
    this.Initialize();
    this.mDone = false;
    this.mRemovable = false;
    this.mOkToStart = false;
    this.Play(ref iSpawnPosition);
    AudioManager.Instance.PlayCue(Banks.Additional, OtherworldlyBolt.SOUND_SPAWN, this.AudioEmitter);
  }

  public void GoHunt()
  {
    this.mOkToStart = true;
    this.mSoundCueIdle = AudioManager.Instance.PlayCue(Banks.Additional, OtherworldlyBolt.SOUND_IDLE, this.AudioEmitter);
  }

  private void Play(ref Vector3 iSpawnPosition)
  {
    if (EffectManager.Instance.IsActive(ref this.mEffectRef))
      return;
    Matrix translation = Matrix.CreateTranslation(iSpawnPosition);
    EffectManager.Instance.StartEffect(OtherworldlyBolt.EFFECT, ref translation, out this.mEffectRef);
  }

  private void Stop(bool iInSilence)
  {
    EffectManager.Instance.Stop(ref this.mEffectRef);
    if (this.mSoundCueIdle != null)
      this.mSoundCueIdle.Stop(AudioStopOptions.AsAuthored);
    if (iInSilence)
      return;
    AudioManager.Instance.PlayCue(Banks.Additional, OtherworldlyBolt.SOUND_HIT, this.AudioEmitter);
  }

  protected override void AddImpulseVelocity(ref Vector3 iVelocity)
  {
    if (!this.mOkToStart)
      return;
    Vector3.Multiply(ref iVelocity, 2f, out iVelocity);
    Vector3.Add(ref this.mVelocity, ref iVelocity, out this.mVelocity);
  }

  private bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
  {
    if (!this.mOkToStart || this.mDone)
      return false;
    if (iSkin1.Owner != null)
    {
      object tag = iSkin1.Owner.Tag;
      bool iCultistSpawned = false;
      bool flag = false;
      bool iKillTarget = false;
      switch (tag)
      {
        case Character _:
          Character iCharacter = tag as Character;
          if (iCharacter.Type == OtherworldlyBolt.STARSPAWN_HASH)
            return false;
          if (!iCharacter.IsEthereal && (!iCharacter.IsSelfShielded || iCharacter.IsSolidSelfShielded))
          {
            this.SpawnCultist(iCharacter);
            iCultistSpawned = true;
          }
          flag = true;
          break;
        case Barrier _:
        case Shield _:
          flag = true;
          iKillTarget = true;
          break;
        case MissileEntity _:
          if ((tag as MissileEntity).Owner is Character owner && (owner.Faction & Factions.FRIENDLY) == Factions.NONE)
            return true;
          flag = true;
          iKillTarget = true;
          break;
      }
      if (flag && NetworkManager.Instance.State != NetworkState.Client)
      {
        if (NetworkManager.Instance.State == NetworkState.Server)
          NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
          {
            ActionType = TriggerActionType.OtherworldlyBoltDestroyed,
            Handle = this.Handle,
            Arg = (int) (tag as Entity).Handle,
            Bool0 = iCultistSpawned,
            Bool1 = iKillTarget,
            Bool2 = false
          });
        this.Destroy(iCultistSpawned, iKillTarget, tag as Entity, false);
        return false;
      }
    }
    return true;
  }

  internal void Reset()
  {
    this.mDone = true;
    this.mRemovable = true;
    this.mOkToStart = false;
  }

  public void Destroy(bool iCultistSpawned, bool iKillTarget, Entity iTarget, bool iInSilence)
  {
    if (!iCultistSpawned && EffectManager.Instance.IsActive(ref this.mEffectRef))
    {
      Vector3 right = Vector3.Right;
      EffectManager.Instance.StartEffect(OtherworldlyBolt.EFFECT_HIT, ref this.mPosition, ref right, out VisualEffectReference _);
    }
    if (iKillTarget && iTarget != null)
      iTarget.Kill();
    if (iTarget != null && iTarget is Character)
      (iTarget as Character).RemoveSelfShield();
    this.mRemovable = true;
    this.Stop(iInSilence);
  }

  public void DestroyOnNetwork(
    bool iCultistSpawned,
    bool iKillTarget,
    Entity iTarget,
    bool iInSilence)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    if (NetworkManager.Instance.State == NetworkState.Server)
    {
      TriggerActionMessage iMessage = new TriggerActionMessage();
      iMessage.ActionType = TriggerActionType.OtherworldlyBoltDestroyed;
      iMessage.Handle = this.Handle;
      if (iTarget != null)
        iMessage.Arg = (int) iTarget.Handle;
      iMessage.Bool0 = iCultistSpawned;
      iMessage.Bool1 = iKillTarget;
      iMessage.Bool2 = iInSilence;
      NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref iMessage);
    }
    this.Destroy(iCultistSpawned, iKillTarget, iTarget, iInSilence);
  }

  private void SpawnCultist(Character iCharacter)
  {
    if (NetworkManager.Instance.State == NetworkState.Server)
      NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
      {
        ActionType = TriggerActionType.OtherworldlyDischarge,
        Handle = this.Handle,
        Arg = (int) iCharacter.Handle
      });
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    OtherworldlyDischarge.Instance.Execute((IDamageable) iCharacter, this as ISpellCaster, this.mPlayState);
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if (!this.mOkToStart)
      return;
    if (NetworkManager.Instance.State != NetworkState.Client)
    {
      this.mCheckTargetTimer -= iDeltaTime;
      if ((double) this.mCheckTargetTimer <= 0.0)
      {
        this.FindClosestAvatar(out this.mCurrentTarget);
        this.mCheckTargetTimer = 1f;
      }
    }
    EntityManager entityManager = this.mPlayState.EntityManager;
    List<Entity> entities = entityManager.GetEntities(this.mPosition, 5f, true);
    for (int index = 0; index < entities.Count; ++index)
    {
      if (entities[index] is Character character && (character.Faction & Factions.FRIENDLY) == Factions.NONE)
      {
        Vector3 result = (character.Position - this.mPosition) with
        {
          Y = 0.0f
        };
        float num1 = result.LengthSquared();
        if ((double) num1 > 9.9999999747524271E-07)
        {
          result.Normalize();
          float num2 = num1 / 25f;
          Vector3.Multiply(ref result, iDeltaTime / num2, out result);
          Vector3.Subtract(ref this.mVelocity, ref result, out this.mVelocity);
          Matrix identity = Matrix.Identity;
          this.mBody.MoveTo(ref this.mPosition, ref identity);
        }
      }
    }
    entityManager.ReturnEntityList(entities);
    this.FlyTowardsTarget(iDeltaTime);
  }

  private void FindClosestAvatar(out Avatar oClosestAvatar)
  {
    Player[] players = Magicka.Game.Instance.Players;
    bool flag = false;
    float num1 = float.MaxValue;
    Avatar avatar1 = (Avatar) null;
    for (int index = 0; index < players.Length && !flag; ++index)
    {
      if (players[index].Playing)
      {
        Avatar avatar2 = players[index].Avatar;
        if (avatar2 != null && !avatar2.Dead && !avatar2.IsEthereal)
        {
          float num2 = ((avatar2.Position - this.mPosition) with
          {
            Y = 0.0f
          }).LengthSquared();
          if ((double) num2 < (double) num1)
          {
            num1 = num2;
            avatar1 = avatar2;
          }
        }
      }
    }
    oClosestAvatar = avatar1;
  }

  private void FlyTowardsTarget(float iDeltaTime)
  {
    if (this.mCurrentTarget != null && !this.mCurrentTarget.Dead && !this.mCurrentTarget.IsEthereal)
    {
      this.mToTarget = this.mCurrentTarget.Position;
      Vector3.Subtract(ref this.mToTarget, ref this.mPosition, out this.mToTarget);
    }
    if ((double) this.mPosition.Y <= 0.0)
    {
      this.mToTarget.Y = 0.0f;
      this.mPosition.Y = 0.0f;
    }
    if ((double) this.mToTarget.LengthSquared() <= 9.9999999747524271E-07)
      return;
    this.mToTarget.Normalize();
    Vector3 result1;
    Vector3.Multiply(ref this.mToTarget, (float) ((double) iDeltaTime * (double) this.mAcceleration * 2.0), out result1);
    Vector3 result2 = this.mVelocity;
    Vector3.Add(ref result2, ref result1, out result2);
    if ((double) result2.LengthSquared() > (double) this.mTerminalVelocity * (double) this.mTerminalVelocity)
    {
      result2.Normalize();
      Vector3.Multiply(ref result2, this.mTerminalVelocity, out result2);
    }
    this.mVelocity = result2;
    Vector3 result3;
    Vector3.Multiply(ref this.mVelocity, iDeltaTime, out result3);
    Vector3.Add(ref this.mPosition, ref result3, out this.mPosition);
    Matrix identity = Matrix.Identity;
    this.mBody.MoveTo(ref this.mPosition, ref identity);
    EffectManager.Instance.UpdatePositionDirection(ref this.mEffectRef, ref this.mPosition, ref this.mToTarget);
  }

  public override bool Dead => this.mDead;

  public override bool Removable => this.mRemovable;

  public override void Kill() => this.mDead = this.mRemovable = true;

  protected override void INetworkUpdate(ref EntityUpdateMessage iMsg)
  {
    base.INetworkUpdate(ref iMsg);
    this.mPosition = iMsg.Position;
    if ((iMsg.Features & EntityFeatures.GenericUShort) == EntityFeatures.None)
      return;
    this.mCurrentTarget = Entity.GetFromHandle((int) iMsg.GenericUShort) as Avatar;
  }

  protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
  {
    oMsg = new EntityUpdateMessage();
    oMsg.Features |= EntityFeatures.Position;
    oMsg.Position = this.Position;
    if (this.mCurrentTarget == null)
      return;
    oMsg.Features |= EntityFeatures.GenericUShort;
    oMsg.GenericUShort = this.mCurrentTarget.Handle;
  }
}
