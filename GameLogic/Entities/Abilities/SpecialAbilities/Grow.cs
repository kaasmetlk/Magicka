// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Grow
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

internal class Grow : SpecialAbility, IAbilityEffect
{
  private const float TTL = 20f;
  private static List<Grow> sCache;
  private static List<Grow> sActiveCache;
  private static readonly int MAGICK_EFFECT = "magick_grow".GetHashCodeCustom();
  private static readonly int SOUND_EFFECT = "magick_grow".GetHashCodeCustom();
  private Magicka.GameLogic.Entities.Character mOwner;
  private float mTTL;
  private float mAnimationTime;
  private ActiveAura mDamageAura;
  private ActiveAura mHealthAura;

  public static Grow GetInstance()
  {
    if (Grow.sCache.Count > 0)
    {
      Grow instance = Grow.sCache[Grow.sCache.Count - 1];
      Grow.sCache.RemoveAt(Grow.sCache.Count - 1);
      Grow.sActiveCache.Add(instance);
      return instance;
    }
    Grow instance1 = new Grow();
    Grow.sActiveCache.Add(instance1);
    return instance1;
  }

  public static void InitializeCache(int iNr)
  {
    Grow.sCache = new List<Grow>(iNr);
    Grow.sActiveCache = new List<Grow>(iNr);
    for (int index = 0; index < iNr; ++index)
      Grow.sCache.Add(new Grow());
  }

  private Grow()
    : base(Magicka.Animations.cast_spell0, "#magick_grow".GetHashCodeCustom())
  {
    BuffStorage iBuff = new BuffStorage(new BuffBoostDamage(new Damage(AttackProperties.Damage, Elements.Earth, 0.0f, 3f)), VisualCategory.None, new Vector3());
    AuraBuff iAura = new AuraBuff(iBuff);
    AuraStorage auraStorage = new AuraStorage(iAura, AuraTarget.Self, AuraType.Buff, 0, float.MaxValue, 0.0f, VisualCategory.None, new Vector3(), (int[]) null, Factions.NONE);
    this.mDamageAura = new ActiveAura();
    this.mDamageAura.Aura = auraStorage;
    iBuff = new BuffStorage(new BuffModifyHitPoints(2f, 0.0f), VisualCategory.None, new Vector3(), 2f, 0.0f);
    iAura = new AuraBuff(iBuff);
    auraStorage = new AuraStorage(iAura, AuraTarget.Self, AuraType.Buff, 0, float.MaxValue, 0.0f, VisualCategory.None, new Vector3(), (int[]) null, Factions.NONE);
    this.mHealthAura = new ActiveAura();
    this.mHealthAura.Aura = auraStorage;
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    throw new Exception("Grow have to be cast by a character!");
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    if (!(iOwner is Magicka.GameLogic.Entities.Character))
    {
      this.OnRemove();
      return false;
    }
    if (Shrink.KillShrinkOnCharacter(iOwner))
    {
      AudioManager.Instance.PlayCue(Banks.Additional, Grow.SOUND_EFFECT, iOwner.AudioEmitter);
      this.OnRemove();
      return true;
    }
    for (int index = 0; index < Grow.sActiveCache.Count; ++index)
    {
      if (Grow.sActiveCache[index].mOwner == iOwner)
      {
        Grow.sActiveCache[index].mTTL = 20f;
        this.OnRemove();
        return true;
      }
    }
    this.mOwner = iOwner as Magicka.GameLogic.Entities.Character;
    this.mTTL = 20f;
    this.mAnimationTime = 0.0f;
    if (this.mOwner != null)
    {
      AudioManager.Instance.PlayCue(Banks.Additional, Grow.SOUND_EFFECT, this.mOwner.AudioEmitter);
      SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
      Vector3 position = this.mOwner.Position;
      Vector3 direction = this.mOwner.Direction;
      EffectManager.Instance.StartEffect(Grow.MAGICK_EFFECT, ref position, ref direction, out VisualEffectReference _);
      return true;
    }
    this.OnRemove();
    return false;
  }

  internal static bool KillGrowOnCharacter(ISpellCaster iOwner)
  {
    for (int index = 0; index < Grow.sActiveCache.Count; ++index)
    {
      if (Grow.sActiveCache[index].mOwner == iOwner)
      {
        Grow.sActiveCache[index].mTTL = 0.0f;
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
    float num2 = num1 + 1f;
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
    this.mOwner.CharacterBody.SpeedMultiplier *= (float) (1.0 - 0.5 * (double) num1);
    this.mOwner.Body.Mass = this.mOwner.Template.Mass + 750f * num1;
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
    Grow.sActiveCache.Remove(this);
    Grow.sCache.Add(this);
  }
}
