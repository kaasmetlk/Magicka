// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Bosses.PropBoss
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Microsoft.Xna.Framework;
using PolygonHead;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Bosses;

internal class PropBoss : DamageablePhysicsEntity, IBoss
{
  private string mType;
  private int mOnDeathId;
  private int mOnDamageId;

  public PropBoss(
    PlayState iPlayState,
    string iType,
    int iUniqueID,
    int iOnDeathId,
    int iOnDamageId)
    : base(iPlayState)
  {
    this.mType = iType;
    this.mUniqueID = iUniqueID;
    this.mOnDeathId = iOnDeathId;
    this.mOnDamageId = iOnDamageId;
    this.mTemplate = this.mPlayState.Content.Load<PhysicsEntityTemplate>("Data/PhysicsEntities/" + this.mType);
  }

  public void Initialize(ref Matrix iOrientation, int iUniqueID)
  {
    this.Initialize(ref iOrientation);
  }

  public void Initialize(ref Matrix iOrientation)
  {
    this.Initialize(this.mTemplate, iOrientation, this.mUniqueID);
    if (this.mOnDeathId != 0)
      this.OnDeath = this.mOnDeathId;
    if (this.mOnDeathId != 0)
      this.OnDamage = this.mOnDamageId;
    this.mPlayState.EntityManager.AddEntity((Entity) this);
  }

  public void DeInitialize()
  {
  }

  public void ScriptMessage(BossMessages iMessage)
  {
  }

  public void UpdateBoss(DataChannel iDataChannel, float iDeltaTime, bool iFightStarted)
  {
  }

  public DamageResult Damage(
    int iPartIndex,
    Magicka.GameLogic.Damage iDamage,
    Entity iAttacker,
    ref Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
    return DamageResult.None;
  }

  public void Damage(int iPartIndex, float iDamage, Elements iElement)
  {
  }

  public void SetSlow(int iIndex)
  {
  }

  public void GetRandomPositionOnCollisionSkin(int iIndex, out Vector3 oPosition)
  {
    oPosition = new Vector3();
  }

  public bool HasStatus(int iIndex, StatusEffects iStatus) => false;

  public float StatusMagnitude(int iIndex, StatusEffects iStatus) => 0.0f;

  public void NetworkUpdate(ref BossUpdateMessage iMsg)
  {
  }

  public void NetworkInitialize(ref BossInitializeMessage iMsg)
  {
    throw new NotImplementedException();
  }

  public BossEnum GetBossType() => BossEnum.Generic;

  public bool NetworkInitialized => true;
}
