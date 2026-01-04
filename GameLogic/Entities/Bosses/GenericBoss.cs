// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Bosses.GenericBoss
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Microsoft.Xna.Framework;
using PolygonHead;

#nullable disable
namespace Magicka.GameLogic.Entities.Bosses;

public class GenericBoss : NonPlayerCharacter, IBoss
{
  private new int mType;
  private int mMeshIdx;

  public GenericBoss(PlayState iPlayState, int iType, int iUniqueID, int iMeshIdx)
    : base(iPlayState)
  {
    this.mType = iType;
    this.mUniqueID = iUniqueID;
    this.mMeshIdx = iMeshIdx;
  }

  public void Initialize(ref Matrix iOrientation, int iUniqueID)
  {
    this.Initialize(ref iOrientation);
  }

  public void Initialize(ref Matrix iOrientation)
  {
    this.Initialize(CharacterTemplate.GetCachedTemplate(this.mType), this.mMeshIdx, iOrientation.Translation, this.mUniqueID);
    this.CharacterBody.DesiredDirection = iOrientation.Forward;
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

  public void NetworkInitialize(ref BossInitializeMessage iMsg)
  {
  }

  public void NetworkUpdate(ref BossUpdateMessage iMsg)
  {
  }

  public BossEnum GetBossType() => BossEnum.Generic;

  public bool NetworkInitialized => true;
}
