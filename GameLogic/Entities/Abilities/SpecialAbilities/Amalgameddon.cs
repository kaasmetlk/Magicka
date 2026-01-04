// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Amalgameddon
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Gamers;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class Amalgameddon : SpecialAbility, IAbilityEffect
{
  private static volatile Amalgameddon sSingelton;
  private static volatile object sSingeltonLock = new object();
  private readonly float RANGE = 20f;
  private ISpellCaster mOwner;
  private float mTTL;
  private float mTimer;
  private float mLifeAmount;
  private Magicka.GameLogic.Entities.Character mFirstUnit;
  private Magicka.GameLogic.Entities.Character mSecondUnit;
  private Magicka.GameLogic.Entities.Character mSorryUnit;
  private Magicka.GameLogic.Entities.Character mHappyUnit;
  private Amalgameddon.AmalgamationState mState;

  public static Amalgameddon Instance
  {
    get
    {
      if (Amalgameddon.sSingelton == null)
      {
        lock (Amalgameddon.sSingeltonLock)
        {
          if (Amalgameddon.sSingelton == null)
            Amalgameddon.sSingelton = new Amalgameddon();
        }
      }
      return Amalgameddon.sSingelton;
    }
  }

  public Amalgameddon()
    : base(Magicka.Animations.cast_magick_global, "#magick_amalgameddond".GetHashCodeCustom())
  {
  }

  public Amalgameddon(Magicka.Animations iAnimation)
    : base(Magicka.Animations.cast_magick_global, "#magick_amalgameddond".GetHashCodeCustom())
  {
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    if ((double) this.mTTL > 0.0)
    {
      AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL);
      return false;
    }
    NetworkState state = NetworkManager.Instance.State;
    if ((state == NetworkState.Client || iOwner is Avatar && (iOwner as Avatar).Player.Gamer is NetworkGamer) && (state != NetworkState.Client || !(iOwner is Avatar) || (iOwner as Avatar).Player.Gamer is NetworkGamer))
      return false;
    base.Execute(iOwner, iPlayState);
    this.mState = Amalgameddon.AmalgamationState.None;
    this.mOwner = iOwner;
    this.mTTL = 10f;
    List<Entity> entities = this.mOwner.PlayState.EntityManager.GetEntities(this.mOwner.Position, this.RANGE, false);
    foreach (Entity entity1 in entities)
    {
      if (entity1 is Magicka.GameLogic.Entities.Character && entity1 != this.mOwner)
      {
        foreach (Entity entity2 in entities)
        {
          if (entity2 is Magicka.GameLogic.Entities.Character && entity2 != entity1 && entity2 != this.mOwner && (entity1 as Magicka.GameLogic.Entities.Character).Type == (entity2 as Magicka.GameLogic.Entities.Character).Type)
          {
            this.mFirstUnit = entity1 as Magicka.GameLogic.Entities.Character;
            this.mSecondUnit = entity2 as Magicka.GameLogic.Entities.Character;
            this.mFirstUnit.CharacterBody.ApplyGravity = false;
            this.mSecondUnit.CharacterBody.ApplyGravity = false;
            this.mState = Amalgameddon.AmalgamationState.Rising;
            this.mOwner.PlayState.EntityManager.ReturnEntityList(entities);
            SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
            return true;
          }
        }
      }
    }
    this.mFirstUnit = (Magicka.GameLogic.Entities.Character) null;
    this.mSecondUnit = (Magicka.GameLogic.Entities.Character) null;
    this.mSorryUnit = (Magicka.GameLogic.Entities.Character) null;
    AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL);
    this.mTTL = 0.0f;
    return false;
  }

  public bool IsDead => (double) this.mTTL <= 0.0;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if (this.mOwner == null)
      return;
    this.mTTL -= iDeltaTime;
    if ((double) this.mTTL < 0.0)
    {
      this.mFirstUnit.CharacterBody.ApplyGravity = true;
      this.mSecondUnit.CharacterBody.ApplyGravity = true;
    }
    if (this.mState == Amalgameddon.AmalgamationState.Rising)
    {
      Vector3 position1 = this.mFirstUnit.Position;
      position1.Y += 0.015f;
      this.mFirstUnit.Body.Position = position1;
      Vector3 position2 = this.mSecondUnit.Position;
      position2.Y += 0.015f;
      this.mSecondUnit.Body.Position = position2;
      this.mFirstUnit.Stun(10f);
      this.mSecondUnit.Stun(10f);
      if ((double) this.mFirstUnit.Position.Y <= 5.0)
        return;
      this.mState = Amalgameddon.AmalgamationState.PushApart;
      this.mHappyUnit = this.mFirstUnit;
      this.mSorryUnit = this.mSecondUnit;
      this.mLifeAmount = this.mSorryUnit.HitPoints;
      this.mTimer = 0.2f;
    }
    else if (this.mState == Amalgameddon.AmalgamationState.PushApart)
    {
      this.mTimer -= iDeltaTime;
      Vector3 result = this.mHappyUnit.Position - this.mSorryUnit.Position;
      result.Normalize();
      Vector3 position = this.mHappyUnit.Position;
      Vector3.Multiply(ref result, 0.25f, out result);
      this.mHappyUnit.Body.Position = position + result;
      this.mSorryUnit.Body.Position = this.mSorryUnit.Position - result;
      if ((double) this.mTimer >= 0.0)
        return;
      this.mState = Amalgameddon.AmalgamationState.SmashTogether;
    }
    else
    {
      if (this.mState != Amalgameddon.AmalgamationState.SmashTogether)
        return;
      Vector3 result = this.mHappyUnit.Position - this.mSorryUnit.Position;
      result.Normalize();
      Vector3 position = this.mHappyUnit.Position;
      Vector3.Multiply(ref result, -0.6f, out result);
      this.mHappyUnit.Body.Position = position + result;
      this.mSorryUnit.Body.Position = this.mSorryUnit.Position - result;
      if ((double) (this.mHappyUnit.Position - this.mSorryUnit.Position).Length() >= (double) this.mHappyUnit.Radius + (double) this.mSorryUnit.Radius)
        return;
      this.mSorryUnit.OverKill();
      this.mHappyUnit.Damage(-this.mLifeAmount, Elements.Life);
      this.mHappyUnit.CharacterBody.ApplyGravity = true;
      this.mHappyUnit.Unstun();
      this.mTTL = 0.0f;
    }
  }

  public void OnRemove() => this.mTTL = 0.0f;

  private enum AmalgamationState
  {
    None,
    Rising,
    PushApart,
    SmashTogether,
  }
}
