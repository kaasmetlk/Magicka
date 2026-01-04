// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.ChantSpells
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Magicka.Graphics.Lights;
using Microsoft.Xna.Framework;
using PolygonHead;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities;

public struct ChantSpells
{
  public ChantSpellState State;
  private Elements mElement;
  private Elements mNewElement;
  public bool Active;
  public int Index;
  private float mHorizontalTime;
  private float mVerticalTime;
  private float mHorizontalOffset;
  private float mVerticalOffset;
  private float mHorizontalSpeed;
  private float mVerticalSpeed;
  private Vector3 mTargetPosition;
  private Vector3 mSourcePosition;
  private Vector3 mPosition;
  private VisualEffectReference mEffect;
  private DynamicLight mLight;
  public Character Owner;
  private int mMergeTarget;
  private double mTimeStamp;
  private float mTTL;

  public ChantSpells(Elements iElement, Character iOwner)
  {
    this.Active = false;
    this.mTimeStamp = iOwner.PlayState.PlayTime;
    this.mMergeTarget = 0;
    this.mSourcePosition = iOwner.Position;
    this.State = iOwner.SpellQueue.Count < 5 ? ChantSpellState.Orbiting : ChantSpellState.Escaping;
    this.Index = 0;
    this.mElement = iElement;
    this.mNewElement = Elements.None;
    this.mTargetPosition = iOwner.CastSource.Translation;
    this.mPosition = (double) this.mTargetPosition.LengthSquared() <= 1.4012984643248171E-45 ? iOwner.Position : iOwner.CastSource.Translation;
    this.mHorizontalTime = 0.0f;
    this.mVerticalTime = 0.0f;
    this.mHorizontalOffset = (float) (MagickaMath.Random.NextDouble() * 0.5 + 0.75);
    this.mVerticalOffset = (float) MagickaMath.Random.NextDouble();
    this.mHorizontalSpeed = (float) MagickaMath.Random.NextDouble() + 0.5f;
    this.mVerticalSpeed = (float) ((MagickaMath.Random.NextDouble() - 0.5) * 2.0);
    if ((double) this.mVerticalSpeed < 0.0)
      this.mHorizontalSpeed *= -1f;
    this.mTargetPosition = this.mPosition + new Vector3((float) MagickaMath.Random.NextDouble() - 0.5f, 0.0f, (float) MagickaMath.Random.NextDouble() - 0.5f);
    this.Owner = iOwner;
    Vector3 vector3 = new Vector3();
    switch (iElement)
    {
      case Elements.Water:
        vector3 = Spell.WATERCOLOR;
        break;
      case Elements.Cold:
        vector3 = Spell.COLDCOLOR;
        break;
      case Elements.Fire:
        vector3 = Spell.FIRECOLOR;
        break;
      case Elements.Lightning:
        vector3 = Spell.LIGHTNINGCOLOR;
        break;
      case Elements.Arcane:
        vector3 = Spell.ARCANECOLOR;
        break;
      case Elements.Life:
        vector3 = Spell.LIFECOLOR;
        break;
      case Elements.Shield:
        vector3 = Spell.SHIELDCOLOR;
        break;
      case Elements.Ice:
        vector3 = Spell.ICECOLOR;
        break;
      case Elements.Poison:
        vector3 = Spell.LIFECOLOR;
        break;
    }
    Vector3 iColor = vector3 * 0.75f;
    if ((double) iColor.LengthSquared() > 0.10000000149011612)
    {
      Vector3 result;
      Vector3.Multiply(ref iColor, 0.25f, out result);
      this.mLight = DynamicLight.GetCachedLight();
      this.mLight.Initialize(this.mPosition, iColor, 0.5f, 7f, 5f, 1f);
      this.mLight.AmbientColor = result;
      this.mLight.Enable();
    }
    else
      this.mLight = (DynamicLight) null;
    Vector3 direction = iOwner.Direction;
    int index = Defines.ElementIndex(iElement);
    EffectManager.Instance.StartEffect(Defines.ChantEffects[index], ref this.mPosition, ref direction, out this.mEffect);
    this.mTTL = 1f + (float) MagickaMath.Random.NextDouble();
  }

  public void Update(float iDeltaTime)
  {
    switch (this.State)
    {
      case ChantSpellState.Orbiting:
        if (this.Owner == null || this.Owner.Dead || this.Owner.Polymorphed)
        {
          this.Owner = (Character) null;
          this.State = ChantSpellState.Escaping;
          this.mTTL = 1f;
          break;
        }
        if (this.mElement == Elements.Lightning & this.Owner.HasStatus(StatusEffects.Wet) && !this.Owner.HasPassiveAbility(Item.PassiveAbilities.WetLightning))
        {
          Spell oSpell;
          Spell.DefaultSpell(Elements.Lightning, out oSpell);
          DamageCollection5 oDamages;
          oSpell.CalculateDamage(SpellType.Lightning, CastType.Self, out oDamages);
          oDamages.MultiplyMagnitude(0.5f);
          int num = (int) this.Owner.Damage(oDamages, (Entity) this.Owner, this.mTimeStamp, this.mPosition);
          Vector3 result = this.Owner.Position;
          Vector3.Subtract(ref result, ref this.mPosition, out result);
          LightningBolt.GetLightning().InitializeEffect(ref this.mPosition, result, Spell.LIGHTNINGCOLOR, false, 1f, 1f, this.Owner.PlayState);
          for (int iIndex = 0; iIndex < this.Owner.SpellQueue.Count; ++iIndex)
          {
            if ((this.Owner.SpellQueue[iIndex].Element & Elements.Lightning) == Elements.Lightning)
            {
              this.Owner.SpellQueue.RemoveAt(iIndex);
              break;
            }
          }
          if (this.Owner is Avatar)
            (this.Owner as Avatar).Player.IconRenderer.ClearElements(Elements.Lightning);
          this.State = ChantSpellState.MoveToTarget;
          this.mTTL = 0.0f;
          break;
        }
        this.mSourcePosition = this.Owner.Position;
        if (!this.Owner.Chanting)
        {
          this.Stop();
          break;
        }
        this.mVerticalTime = MathHelper.WrapAngle(this.mVerticalTime + iDeltaTime * 4f * this.mVerticalSpeed);
        this.mHorizontalTime = MathHelper.WrapAngle(this.mHorizontalTime + iDeltaTime * 5f * this.mHorizontalSpeed);
        Vector3 zero = Vector3.Zero;
        MathApproximation.FastSinCos(this.mHorizontalTime, out zero.X, out zero.Z);
        MathApproximation.FastSin(this.mVerticalTime, out zero.Y);
        zero.X *= this.mHorizontalOffset;
        zero.Z *= this.mHorizontalOffset;
        zero.Y *= this.mVerticalOffset;
        Vector3.Add(ref this.mSourcePosition, ref zero, out this.mTargetPosition);
        break;
      case ChantSpellState.Escaping:
        this.mTTL -= iDeltaTime;
        float num1 = (float) Math.Sin((double) this.mTTL * 3.1415927410125732);
        Vector3 vector3 = this.mTargetPosition - this.mPosition;
        vector3.Normalize();
        this.mTargetPosition += vector3 + new Vector3(num1 + 0.4f, (float) ((double) num1 * 0.20000000298023224 + 0.40000000596046448), (float) ((double) num1 * -1.2000000476837158 - 0.40000000596046448));
        if ((double) this.mTTL < 0.0)
        {
          this.Stop();
          break;
        }
        break;
      case ChantSpellState.Merging:
        this.mTargetPosition = ChantSpellManager.GetChantSpell(this.mMergeTarget).mPosition;
        if ((double) (this.mPosition - ChantSpellManager.GetChantSpell(this.mMergeTarget).mPosition).LengthSquared() < 0.019999999552965164)
        {
          ChantSpellManager.GetChantSpell(this.mMergeTarget).Stop();
          if (this.mNewElement != Elements.None)
          {
            this.Reinitialize();
            return;
          }
          this.Stop();
          return;
        }
        break;
      case ChantSpellState.MoveToTarget:
        if ((double) this.mTTL <= 0.0)
        {
          this.Stop();
          break;
        }
        break;
    }
    this.mPosition += Vector3.Normalize(this.mTargetPosition - this.mPosition) * iDeltaTime * 5f;
    Vector3 forward = Vector3.Forward;
    EffectManager.Instance.UpdatePositionDirection(ref this.mEffect, ref this.mPosition, ref forward);
    if (this.mLight == null)
      return;
    this.mLight.Position = this.mPosition;
  }

  public void Reinitialize()
  {
    EffectManager.Instance.Stop(ref this.mEffect);
    if (this.mLight != null)
    {
      this.mLight.Stop(false);
      this.mLight = (DynamicLight) null;
    }
    this.State = ChantSpellState.Orbiting;
    this.mElement = this.mNewElement;
    this.mNewElement = Elements.None;
    this.mHorizontalTime = 0.0f;
    this.mVerticalTime = 0.0f;
    this.mHorizontalOffset = (float) (MagickaMath.Random.NextDouble() * 0.5 + 0.75);
    this.mVerticalOffset = (float) MagickaMath.Random.NextDouble();
    this.mHorizontalSpeed = (float) MagickaMath.Random.NextDouble() + 0.5f;
    this.mVerticalSpeed = (float) ((MagickaMath.Random.NextDouble() - 0.5) * 2.0);
    if ((double) this.mVerticalSpeed < 0.0)
      this.mHorizontalSpeed *= -1f;
    this.mTargetPosition = this.mPosition + new Vector3((float) MagickaMath.Random.NextDouble() - 0.5f, 0.0f, (float) MagickaMath.Random.NextDouble() - 0.5f);
    Vector3 vector3 = new Vector3();
    switch (this.mElement)
    {
      case Elements.Water:
        vector3 = Spell.WATERCOLOR;
        break;
      case Elements.Cold:
        vector3 = Spell.COLDCOLOR;
        break;
      case Elements.Fire:
        vector3 = Spell.FIRECOLOR;
        break;
      case Elements.Lightning:
        vector3 = Spell.LIGHTNINGCOLOR;
        break;
      case Elements.Arcane:
        vector3 = Spell.ARCANECOLOR;
        break;
      case Elements.Life:
        vector3 = Spell.LIFECOLOR;
        break;
      case Elements.Shield:
        vector3 = Spell.SHIELDCOLOR;
        break;
      case Elements.Ice:
        vector3 = Spell.ICECOLOR;
        break;
      case Elements.Poison:
        vector3 = Spell.LIFECOLOR;
        break;
    }
    Vector3 iColor = vector3 * 0.75f;
    if ((double) iColor.LengthSquared() > 0.10000000149011612)
    {
      Vector3 result;
      Vector3.Multiply(ref iColor, 0.25f, out result);
      this.mLight = DynamicLight.GetCachedLight();
      this.mLight.Initialize(this.mPosition, iColor, 0.5f, 7f, 5f, 1f);
      this.mLight.AmbientColor = result;
      this.mLight.Enable();
    }
    else
      this.mLight = (DynamicLight) null;
    if (this.Owner.SpellQueue.Count >= 5)
      this.State = ChantSpellState.Escaping;
    Vector3 direction = this.Owner.Direction;
    int index = Defines.ElementIndex(this.mElement);
    EffectManager.Instance.StartEffect(Defines.ChantEffects[index], ref this.mPosition, ref direction, out this.mEffect);
    this.mTTL = 1f + (float) MagickaMath.Random.NextDouble();
  }

  public void Stop()
  {
    EffectManager.Instance.Stop(ref this.mEffect);
    if (this.mLight != null)
    {
      this.mLight.Stop(false);
      this.mLight = (DynamicLight) null;
    }
    ChantSpellManager.Remove(ref this);
  }

  public Elements Element => this.mElement;

  public void MergeWith(ChantSpells iSpell, Elements iNewElement)
  {
    this.State = ChantSpellState.Merging;
    this.mMergeTarget = iSpell.Index;
    if (iNewElement != Elements.None)
    {
      this.mNewElement = iNewElement;
      iSpell.mNewElement = iNewElement;
    }
    iSpell.mMergeTarget = this.Index;
    iSpell.State = ChantSpellState.Merging;
    ChantSpellManager.Set(iSpell);
  }

  public float TTL
  {
    get => this.mTTL;
    set => this.mTTL = value;
  }
}
