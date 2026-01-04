// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.HomingCharge
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using PolygonHead;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class HomingCharge : SpecialAbility, IAbilityEffect
{
  private static List<HomingCharge> sCache;
  private Magicka.GameLogic.Entities.Character mOwner;
  public PlayState mPlayState;
  private VisualEffectReference mEffect;
  private float TIME = 5f;
  private readonly float START_CHARGE = 0.8f;
  private readonly float STOP_CHARGE = 2.5f;
  private readonly float THRESHOLD = 0.015f;
  private Vector3 mLastPosition;
  private bool mCharging;
  private float mTTL;
  private float[] thresholdArray;
  private float[] deltaArray;

  public static HomingCharge GetInstance()
  {
    if (HomingCharge.sCache.Count <= 0)
      return new HomingCharge();
    HomingCharge instance = HomingCharge.sCache[HomingCharge.sCache.Count - 1];
    HomingCharge.sCache.RemoveAt(HomingCharge.sCache.Count - 1);
    return instance;
  }

  public static void InitializeCache(int iNr, PlayState iPlayState)
  {
    HomingCharge.sCache = new List<HomingCharge>(iNr);
    for (int index = 0; index < iNr; ++index)
      HomingCharge.sCache.Add(new HomingCharge());
  }

  public HomingCharge(Magicka.Animations iAnimation)
    : base(iAnimation, "#magick_grease".GetHashCodeCustom())
  {
  }

  private HomingCharge()
    : base(Magicka.Animations.None, "#magick_grease".GetHashCodeCustom())
  {
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    throw new Exception("HomingCharge needs an owner!");
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    this.mTTL = this.TIME;
    this.mCharging = false;
    this.mPlayState = iPlayState;
    this.mOwner = iOwner as Magicka.GameLogic.Entities.Character;
    this.deltaArray = new float[3];
    this.thresholdArray = new float[5];
    this.thresholdArray[0] = this.THRESHOLD;
    this.thresholdArray[1] = this.THRESHOLD;
    this.thresholdArray[2] = this.THRESHOLD;
    this.thresholdArray[3] = this.THRESHOLD;
    this.thresholdArray[4] = this.THRESHOLD;
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    return true;
  }

  private void pushThreshold(float val)
  {
    this.thresholdArray[0] = this.thresholdArray[1];
    this.thresholdArray[1] = this.thresholdArray[2];
    this.thresholdArray[2] = this.thresholdArray[3];
    this.thresholdArray[3] = this.thresholdArray[4];
    this.thresholdArray[4] = val;
  }

  private void pushDelta(float val)
  {
    this.deltaArray[0] = this.deltaArray[1];
    this.deltaArray[1] = this.deltaArray[2];
    this.deltaArray[2] = val;
  }

  private float Threshold()
  {
    float num = 0.0f;
    for (int index = 0; index < 3; ++index)
      num += this.thresholdArray[index];
    return num / 3f;
  }

  private float Delta()
  {
    float num = 0.0f;
    foreach (float delta in this.deltaArray)
      num += delta;
    return num / (float) this.deltaArray.Length;
  }

  public bool IsDead => (double) this.mTTL <= 0.0;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mTTL -= iDeltaTime * this.mOwner.CharacterBody.SpeedMultiplier;
    if (this.mOwner == null)
      return;
    Vector3 position = this.mOwner.Position;
    this.pushThreshold(this.THRESHOLD * this.mOwner.CharacterBody.SpeedMultiplier);
    this.pushDelta(Vector3.Distance(position, this.mLastPosition));
    if ((double) this.mTTL < (double) this.TIME - (double) this.START_CHARGE && !this.mCharging && (double) this.Delta() > 0.0)
      this.mCharging = true;
    if (this.mCharging && (double) this.Delta() < (double) this.Threshold())
    {
      this.mCharging = false;
      this.mOwner.GoToAnimation(Magicka.Animations.special2, 0.01f);
      this.mTTL = 0.0f;
    }
    else
    {
      float num = 0.0f;
      List<Entity> entities = this.mPlayState.EntityManager.GetEntities(this.mOwner.Position, 5f, false);
      foreach (Entity entity in entities)
      {
        if (entity is Avatar)
          num = Vector3.Distance(this.mOwner.Position, (entity as Avatar).Position);
      }
      this.mOwner.PlayState.EntityManager.ReturnEntityList(entities);
      if ((double) num > 0.0 && (double) num < 2.0)
      {
        this.mOwner.GoToAnimation(Magicka.Animations.special2, 0.01f);
        this.mTTL = 0.0f;
      }
      this.mLastPosition = position;
    }
  }

  public void OnRemove()
  {
    this.mTTL = 0.0f;
    EffectManager.Instance.Stop(ref this.mEffect);
    HomingCharge.sCache.Add(this);
  }
}
