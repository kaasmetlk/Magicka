// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Shrink
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Entities.Buffs;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using PolygonHead;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

internal class Shrink : SpecialAbility, IAbilityEffect
{
  private const float TTL = 10f;
  private static List<Shrink> sCache;
  private static List<Shrink> sActiveCache;
  private static readonly int SOUND_EFFECT = "magick_shrink".GetHashCodeCustom();
  private static readonly int MAGICK_EFFECT = "magick_shrink".GetHashCodeCustom();
  private Magicka.GameLogic.Entities.Character mOwner;
  private float mTTL;
  private float mAnimationTime;
  private ActiveAura mDamageAura;
  private ActiveAura mHealthAura;

  public static Shrink GetInstance()
  {
    if (Shrink.sCache.Count > 0)
    {
      Shrink instance = Shrink.sCache[Shrink.sCache.Count - 1];
      Shrink.sCache.RemoveAt(Shrink.sCache.Count - 1);
      Shrink.sActiveCache.Add(instance);
      return instance;
    }
    Shrink instance1 = new Shrink();
    Shrink.sActiveCache.Add(instance1);
    return instance1;
  }

  public static void InitializeCache(int iNr)
  {
    Shrink.sCache = new List<Shrink>(iNr);
    Shrink.sActiveCache = new List<Shrink>(iNr);
    for (int index = 0; index < iNr; ++index)
      Shrink.sCache.Add(new Shrink());
  }

  private Shrink()
    : base(Magicka.Animations.cast_spell0, "#magick_shrink".GetHashCodeCustom())
  {
    BuffStorage iBuff = new BuffStorage(new BuffBoostDamage(new Damage(AttackProperties.Damage, Elements.Earth, 0.0f, 0.5f)), VisualCategory.None, new Vector3());
    AuraBuff iAura = new AuraBuff(iBuff);
    AuraStorage auraStorage = new AuraStorage(iAura, AuraTarget.Self, AuraType.Buff, 0, float.MaxValue, 0.0f, VisualCategory.None, new Vector3(), (int[]) null, Factions.NONE);
    this.mDamageAura = new ActiveAura();
    this.mDamageAura.Aura = auraStorage;
    iBuff = new BuffStorage(new BuffModifyHitPoints(0.5f, 0.0f), VisualCategory.None, new Vector3(), 0.5f, 0.0f);
    iAura = new AuraBuff(iBuff);
    auraStorage = new AuraStorage(iAura, AuraTarget.Self, AuraType.Buff, 0, float.MaxValue, 0.0f, VisualCategory.None, new Vector3(), (int[]) null, Factions.NONE);
    this.mHealthAura = new ActiveAura();
    this.mHealthAura.Aura = auraStorage;
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    throw new Exception("Shrink have to be cast by a character!");
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    if (!(iOwner is Magicka.GameLogic.Entities.Character))
    {
      this.OnRemove();
      return false;
    }
    if (Grow.KillGrowOnCharacter(iOwner))
    {
      AudioManager.Instance.PlayCue(Banks.Additional, Shrink.SOUND_EFFECT, iOwner.AudioEmitter);
      this.OnRemove();
      return true;
    }
    for (int index = 0; index < Shrink.sActiveCache.Count; ++index)
    {
      if (Shrink.sActiveCache[index].mOwner == iOwner)
      {
        Shrink.sActiveCache[index].mTTL = 10f;
        this.OnRemove();
        return true;
      }
    }
    this.mOwner = iOwner as Magicka.GameLogic.Entities.Character;
    this.mTTL = 10f;
    this.mAnimationTime = 0.0f;
    if (this.mOwner != null)
    {
      AudioManager.Instance.PlayCue(Banks.Additional, Shrink.SOUND_EFFECT, this.mOwner.AudioEmitter);
      SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
      Vector3 position = this.mOwner.Position;
      Vector3 direction = this.mOwner.Direction;
      EffectManager.Instance.StartEffect(Shrink.MAGICK_EFFECT, ref position, ref direction, out VisualEffectReference _);
      return true;
    }
    this.OnRemove();
    return false;
  }

  internal static bool KillShrinkOnCharacter(ISpellCaster iOwner)
  {
    for (int index = 0; index < Shrink.sActiveCache.Count; ++index)
    {
      if (Shrink.sActiveCache[index].mOwner == iOwner)
      {
        Shrink.sActiveCache[index].mTTL = 0.0f;
        return true;
      }
    }
    return false;
  }

  public bool IsDead => (double) this.mTTL <= 0.0 && (double) this.mAnimationTime <= 0.0;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mTTL -= iDeltaTime;
    if ((double) this.mTTL <= 0.0 || (double) this.mOwner.HitPoints <= 0.0)
      this.mAnimationTime = Math.Max(this.mAnimationTime - iDeltaTime, 0.0f);
    else if ((double) this.mAnimationTime < 1.0)
      this.mAnimationTime = Math.Min(this.mAnimationTime + iDeltaTime, 1f);
    float num1 = Math.Max(Math.Min(this.mAnimationTime + (float) Math.Sin((double) this.mAnimationTime * 6.2831854820251465 * 4.0) * 0.125f, 1f), 0.0f);
    float num2 = (float) (1.0 - 0.5 * (double) num1);
    float length1 = this.mOwner.Template.Length;
    float radius1 = this.mOwner.Template.Radius;
    float length2 = this.mOwner.Capsule.Length;
    float radius2 = this.mOwner.Capsule.Radius;
    float iLength = length1 * num2;
    float iRadius = radius1 * num2;
    if ((double) iLength != (double) length2 || (double) iRadius != (double) radius2)
    {
      this.mOwner.SetCapsuleForm(iLength, iRadius);
      this.mOwner.SetStaticTransform(num2, num2, num2);
    }
    this.mOwner.CharacterBody.SpeedMultiplier *= (float) (1.0 + (double) num1 * 0.5);
    this.mOwner.Body.Mass = this.mOwner.Template.Mass * (float) (0.5 + (1.0 - (double) num1) * 0.5);
    this.mDamageAura.Execute(this.mOwner, iDeltaTime);
    this.mHealthAura.Execute(this.mOwner, iDeltaTime);
  }

  public void OnRemove()
  {
    if (this.mOwner != null)
    {
      this.mOwner.ResetCapsuleForm();
      this.mOwner.ResetStaticTransform();
      this.mOwner.Body.Mass = this.mOwner.Template.Mass;
    }
    this.mAnimationTime = 0.0f;
    this.mOwner = (Magicka.GameLogic.Entities.Character) null;
    Shrink.sActiveCache.Remove(this);
    Shrink.sCache.Add(this);
  }
}
