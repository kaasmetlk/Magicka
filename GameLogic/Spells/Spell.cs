// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Spells.Spell
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Achievements;
using Magicka.Audio;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.Gamers;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#nullable disable
namespace Magicka.GameLogic.Spells;

[StructLayout(LayoutKind.Auto)]
public struct Spell : IEquatable<Spell>
{
  public static readonly Vector3 EARTHCOLOR = new Vector3(0.3f, 0.2f, 0.1f);
  public static readonly Vector3 WATERCOLOR = new Vector3(0.0f, 0.7f, 1.3f);
  public static readonly Vector3 COLDCOLOR = new Vector3(1f, 1f, 1.4f);
  public static readonly Vector3 FIRECOLOR = new Vector3(1.8f, 0.6f, 0.4f);
  public static readonly Vector3 LIGHTNINGCOLOR = new Vector3(0.75f, 0.5f, 1f);
  public static readonly Vector3 ARCANECOLOR = new Vector3(2f, 0.4f, 0.6f);
  public static readonly Vector3 LIFECOLOR = new Vector3(0.2f, 1.6f, 0.2f);
  public static readonly Vector3 SHIELDCOLOR = new Vector3(2f, 1.5f, 1f);
  public static readonly Vector3 STEAMCOLOR = new Vector3(1f, 1f, 1f);
  public static readonly Vector3 ICECOLOR = new Vector3(0.8f, 0.9f, 1.4f);
  public static readonly Vector3 POISONCOLOR = new Vector3(1f, 1.2f, 0.0f);
  public Elements Element;
  public float EarthMagnitude;
  public float WaterMagnitude;
  public float ColdMagnitude;
  public float FireMagnitude;
  public float LightningMagnitude;
  public float ArcaneMagnitude;
  public float LifeMagnitude;
  public float ShieldMagnitude;
  public float IceMagnitude;
  public float SteamMagnitude;
  public float PoisonMagnitude;

  public float this[Elements iElement]
  {
    get
    {
      switch (iElement)
      {
        case Elements.Earth:
          return this.EarthMagnitude;
        case Elements.Water:
          return this.WaterMagnitude;
        case Elements.Cold:
          return this.ColdMagnitude;
        case Elements.Fire:
          return this.FireMagnitude;
        case Elements.Lightning:
          return this.LightningMagnitude;
        case Elements.Arcane:
          return this.ArcaneMagnitude;
        case Elements.Life:
          return this.LifeMagnitude;
        case Elements.Shield:
          return this.ShieldMagnitude;
        case Elements.Ice:
          return this.IceMagnitude;
        case Elements.Steam:
          return this.SteamMagnitude;
        case Elements.Poison:
          return this.PoisonMagnitude;
        default:
          return 0.0f;
      }
    }
    set
    {
      switch (iElement)
      {
        case Elements.Earth:
          this.EarthMagnitude = value;
          break;
        case Elements.Water:
          this.WaterMagnitude = value;
          break;
        case Elements.Cold:
          this.ColdMagnitude = value;
          break;
        case Elements.Fire:
          this.FireMagnitude = value;
          break;
        case Elements.Lightning:
          this.LightningMagnitude = value;
          break;
        case Elements.Arcane:
          this.ArcaneMagnitude = value;
          break;
        case Elements.Life:
          this.LifeMagnitude = value;
          break;
        case Elements.Shield:
          this.ShieldMagnitude = value;
          break;
        case Elements.Ice:
          this.IceMagnitude = value;
          break;
        case Elements.Steam:
          this.SteamMagnitude = value;
          break;
        case Elements.Poison:
          this.PoisonMagnitude = value;
          break;
      }
    }
  }

  public static int ElementIndex(Elements iElement)
  {
    return iElement == Elements.All ? 15 : (int) (Math.Log((double) iElement) * Defines.ONEOVERLN2 + 0.5);
  }

  public static Elements ElementFromIndex(int iIndex) => (Elements) (1 << iIndex);

  public SpellType GetSpellType()
  {
    if (this.Element == Elements.All)
      return SpellType.Magick;
    if (this.Element == Elements.None)
      return SpellType.Push;
    if ((this.Element & Elements.Shield) != Elements.None)
      return SpellType.Shield;
    if ((this.Element & Elements.PhysicalElements) != Elements.None)
      return SpellType.Projectile;
    if ((this.Element & Elements.Beams) != Elements.None)
      return SpellType.Beam;
    return (this.Element & Elements.Lightning) != Elements.None && (this.Element & Elements.Steam) != Elements.Steam ? SpellType.Lightning : SpellType.Spray;
  }

  public bool ContainsElement(Elements iElement)
  {
    return (double) this[iElement] > 1.4012984643248171E-45;
  }

  public void Cast(bool iFromStaff, ISpellCaster iOwner, CastType iCastType)
  {
    if (iCastType == CastType.None)
      return;
    SpellEffect spellEffect = (SpellEffect) null;
    SpellType spellType = this.GetSpellType();
    if (iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer))
    {
      if (this.Element == Elements.Ice)
        AchievementsManager.Instance.AwardAchievement(iOwner.PlayState, "vanillaice");
      if ((this.Element & Elements.Steam) != Elements.None)
        AchievementsManager.Instance.AwardAchievement(iOwner.PlayState, "letoffsomesteam");
      int num = 0;
      for (int index = 0; index < 11; ++index)
      {
        if ((this.Element & (Elements) (1 << index)) != Elements.None)
          ++num;
      }
      if (num >= 3)
        AchievementsManager.Instance.AwardAchievement(iOwner.PlayState, "statealchemist");
      if (num >= 5)
        AchievementsManager.Instance.AwardAchievement(iOwner.PlayState, "imthewizardkingicand");
    }
    switch (spellType)
    {
      case SpellType.Push:
        spellEffect = PushSpell.GetFromCache();
        break;
      case SpellType.Spray:
        spellEffect = SpraySpell.GetFromCache();
        break;
      case SpellType.Projectile:
        spellEffect = ProjectileSpell.GetFromCache();
        break;
      case SpellType.Shield:
        spellEffect = ShieldSpell.GetFromCache();
        break;
      case SpellType.Beam:
        if (iOwner is Avatar && iCastType == CastType.Force)
          TutorialManager.Instance.SetTip(TutorialManager.Tips.Beams, TutorialManager.Position.Top);
        spellEffect = RailGunSpell.GetFromCache();
        break;
      case SpellType.Lightning:
        spellEffect = LightningSpell.GetFromCache();
        break;
    }
    if (spellEffect == null)
      return;
    switch (iCastType)
    {
      case CastType.Force:
        iOwner.CurrentSpell = spellEffect;
        iOwner.CurrentSpell.CastForce(this, iOwner, iFromStaff);
        break;
      case CastType.Area:
        iOwner.CurrentSpell = spellEffect;
        iOwner.CurrentSpell.CastArea(this, iOwner, iFromStaff);
        break;
      case CastType.Self:
        iOwner.CurrentSpell = spellEffect;
        iOwner.CurrentSpell.CastSelf(this, iOwner, iFromStaff);
        break;
      case CastType.Weapon:
        iOwner.CurrentSpell = spellEffect;
        iOwner.CurrentSpell.CastWeapon(this, iOwner, iFromStaff);
        break;
    }
  }

  public float TotalMagnitude()
  {
    return this.EarthMagnitude + this.WaterMagnitude + this.ColdMagnitude + this.FireMagnitude + this.LightningMagnitude + this.ArcaneMagnitude + this.LifeMagnitude + this.ShieldMagnitude + this.IceMagnitude + this.SteamMagnitude + this.PoisonMagnitude;
  }

  public float TotalSprayMagnitude()
  {
    return this.FireMagnitude + this.ColdMagnitude + this.SteamMagnitude + this.WaterMagnitude + this.PoisonMagnitude;
  }

  public float BlastSize()
  {
    float num = 0.0f;
    switch (this.GetSpellType())
    {
      case SpellType.Spray:
        num = Math.Max(this.SteamMagnitude, Math.Max(this.FireMagnitude, Math.Max(this.ColdMagnitude, Math.Max(this.WaterMagnitude, this.PoisonMagnitude))));
        break;
      case SpellType.Projectile:
        num = Math.Max(this.EarthMagnitude, this.IceMagnitude);
        break;
      case SpellType.Beam:
        num = Math.Max(this.ArcaneMagnitude, this.LifeMagnitude);
        break;
    }
    return (float) Math.Sqrt((double) num / 5.0);
  }

  public static float ArcaneMultiplier(float iMagnitude)
  {
    return (float) Math.Pow((double) iMagnitude / 5.0, 0.5) * 3f;
  }

  public IEnumerable<Cue> PlaySound(SpellType iSpellType, CastType iCastType)
  {
    SpellSoundVariables ssv = new SpellSoundVariables();
    for (int i = 0; i < 11; ++i)
    {
      if ((double) this[Spell.ElementFromIndex(i)] > 0.0)
      {
        ssv.mMagnitude = this[Spell.ElementFromIndex(i)];
        switch (iSpellType)
        {
          case SpellType.Spray:
            switch (iCastType)
            {
              case CastType.None:
                yield return AudioManager.Instance.GetCue<SpellSoundVariables>(Banks.Spells, Defines.SOUNDS_BLAST[i], ssv);
                continue;
              case CastType.Force:
                yield return AudioManager.Instance.GetCue<SpellSoundVariables>(Banks.Spells, Defines.SOUNDS_SPRAY[i], ssv);
                continue;
              case CastType.Area:
              case CastType.Self:
                yield return AudioManager.Instance.GetCue<SpellSoundVariables>(Banks.Spells, Defines.SOUNDS_AREA[i], ssv);
                continue;
              default:
                continue;
            }
          case SpellType.Projectile:
            switch (iCastType)
            {
              case CastType.None:
                yield return AudioManager.Instance.GetCue<SpellSoundVariables>(Banks.Spells, Defines.SOUNDS_BLAST[i], ssv);
                continue;
              case CastType.Force:
                yield return AudioManager.Instance.GetCue<SpellSoundVariables>(Banks.Spells, Defines.SOUNDS_SPRAY[i], ssv);
                continue;
              case CastType.Area:
              case CastType.Self:
                yield return AudioManager.Instance.GetCue<SpellSoundVariables>(Banks.Spells, Defines.SOUNDS_AREA[i], ssv);
                continue;
              default:
                continue;
            }
          case SpellType.Shield:
            switch (iCastType)
            {
              case CastType.None:
                yield return AudioManager.Instance.GetCue<SpellSoundVariables>(Banks.Spells, Defines.SOUNDS_BLAST[i], ssv);
                continue;
              case CastType.Force:
                yield return AudioManager.Instance.GetCue<SpellSoundVariables>(Banks.Spells, Defines.SOUNDS_SPRAY[i], ssv);
                continue;
              case CastType.Area:
              case CastType.Self:
                yield return AudioManager.Instance.GetCue<SpellSoundVariables>(Banks.Spells, Defines.SOUNDS_AREA[i], ssv);
                continue;
              default:
                continue;
            }
          case SpellType.Beam:
            switch (iCastType)
            {
              case CastType.None:
                yield return AudioManager.Instance.GetCue<SpellSoundVariables>(Banks.Spells, Defines.SOUNDS_BLAST[i], ssv);
                continue;
              case CastType.Force:
                yield return AudioManager.Instance.GetCue<SpellSoundVariables>(Banks.Spells, Defines.SOUNDS_SPRAY[i], ssv);
                continue;
              case CastType.Area:
              case CastType.Weapon:
                yield return AudioManager.Instance.GetCue<SpellSoundVariables>(Banks.Spells, Defines.SOUNDS_AREA[i], ssv);
                continue;
              case CastType.Self:
                yield return AudioManager.Instance.GetCue<SpellSoundVariables>(Banks.Spells, Defines.SOUNDS_SELF[i], ssv);
                continue;
              default:
                continue;
            }
          case SpellType.Lightning:
            switch (iCastType)
            {
              case CastType.None:
              case CastType.Force:
              case CastType.Area:
              case CastType.Self:
                yield return AudioManager.Instance.GetCue<SpellSoundVariables>(Banks.Spells, Defines.SOUNDS_SPRAY[i], ssv);
                continue;
              default:
                continue;
            }
          case SpellType.Magick:
            switch (iCastType)
            {
              case CastType.None:
                yield return AudioManager.Instance.GetCue<SpellSoundVariables>(Banks.Spells, Defines.SOUNDS_BLAST[i], ssv);
                continue;
              case CastType.Force:
                yield return AudioManager.Instance.GetCue<SpellSoundVariables>(Banks.Spells, Defines.SOUNDS_SPRAY[i], ssv);
                continue;
              case CastType.Area:
              case CastType.Self:
                yield return AudioManager.Instance.GetCue<SpellSoundVariables>(Banks.Spells, Defines.SOUNDS_AREA[i], ssv);
                continue;
              default:
                continue;
            }
          default:
            continue;
        }
      }
    }
  }

  public void CalculateDamage(
    SpellType iSpellType,
    CastType iCastType,
    out DamageCollection5 oDamages)
  {
    oDamages = new DamageCollection5();
    Damage damage = new Damage();
    switch (iSpellType)
    {
      case SpellType.Spray:
        switch (iCastType)
        {
          case CastType.Force:
          case CastType.Area:
            Damage iDamage1;
            if ((this.Element & Elements.StatusEffects) != Elements.None)
            {
              if ((this.Element & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Steam | Elements.Poison)) != Elements.None)
              {
                iDamage1 = new Damage();
                iDamage1.AttackProperty = AttackProperties.Status;
                iDamage1.Element = this.Element & Elements.StatusEffects;
                iDamage1.Amount = (float) ((double) Defines.STATUS_POISON_DAMAGE * (double) Math.Min(1f, this[Elements.Poison]) + (double) Defines.STATUS_BURN_DAMAGE * (double) Math.Min(1f, this[Elements.Fire]));
                iDamage1.Magnitude = Math.Min(1f, this[Elements.Water] + this[Elements.Fire] + this[Elements.Cold] + this[Elements.Poison] + this[Elements.Steam]);
                oDamages.AddDamage(iDamage1);
              }
              if ((this.Element & Elements.Life) != Elements.None)
              {
                iDamage1 = new Damage();
                iDamage1.AttackProperty = AttackProperties.Status;
                iDamage1.Element = this.Element & Elements.Life;
                iDamage1.Amount += this.ScaleAmount(Defines.STATUS_LIFE_DAMAGE, this[Elements.Life]);
                iDamage1.Magnitude = 1f;
                if (iCastType == CastType.Self)
                  iDamage1.Magnitude = this[Elements.Life];
                oDamages.AddDamage(iDamage1);
              }
            }
            if ((this.Element & (Elements.Cold | Elements.Fire | Elements.Lightning | Elements.Steam)) != Elements.None)
            {
              iDamage1 = new Damage();
              iDamage1.AttackProperty = AttackProperties.Damage;
              iDamage1.Element = this.Element & (Elements.Cold | Elements.Fire | Elements.Lightning | Elements.Steam);
              iDamage1.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_FIRE, this[Elements.Fire]);
              iDamage1.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_COLD, this[Elements.Cold]);
              iDamage1.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_STEAM, this[Elements.Steam]);
              iDamage1.Magnitude = Math.Min(this[Elements.Fire] + this[Elements.Cold] + this[Elements.Steam] + this[Elements.Lightning], 1f);
              oDamages.AddDamage(iDamage1);
            }
            if ((double) this[Elements.Water] <= 0.0)
              return;
            iDamage1 = new Damage();
            iDamage1.AttackProperty = AttackProperties.Pushed;
            iDamage1.Element = Elements.Water;
            iDamage1.Magnitude = (float) (1.0 + (double) this[Elements.Water] / 4.0);
            iDamage1.Amount = (float) (int) ((double) Defines.SPELL_STRENGTH_WATER * (double) this[Elements.Water]);
            oDamages.AddDamage(iDamage1);
            iDamage1 = new Damage();
            iDamage1.AttackProperty = AttackProperties.Damage;
            iDamage1.Element = Elements.Water;
            iDamage1.Magnitude = 1f;
            iDamage1.Amount = 0.0f;
            oDamages.AddDamage(iDamage1);
            return;
          case CastType.Self:
            Damage iDamage2;
            if ((this.Element & Elements.StatusEffects) != Elements.None)
            {
              if ((this.Element & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Steam | Elements.Poison)) != Elements.None)
              {
                iDamage2 = new Damage();
                iDamage2.AttackProperty = AttackProperties.Status;
                iDamage2.Element = this.Element & Elements.StatusEffects;
                iDamage2.Amount = (float) ((double) Defines.STATUS_POISON_DAMAGE * (double) Math.Min(1f, this[Elements.Poison]) + (double) Defines.STATUS_BURN_DAMAGE * (double) Math.Min(1f, this[Elements.Fire]));
                iDamage2.Magnitude = Math.Min(1f, this[Elements.Water] + this[Elements.Fire] + this[Elements.Cold] + this[Elements.Poison] + this[Elements.Steam]);
                oDamages.AddDamage(iDamage2);
              }
              if ((this.Element & Elements.Life) != Elements.None)
              {
                iDamage2 = new Damage();
                iDamage2.AttackProperty = AttackProperties.Status;
                iDamage2.Element = this.Element & Elements.Life;
                iDamage2.Amount += this.ScaleAmount(Defines.STATUS_LIFE_DAMAGE, this[Elements.Life]);
                iDamage2.Magnitude = 1f;
                if (iCastType == CastType.Self)
                  iDamage2.Magnitude = this[Elements.Life];
                oDamages.AddDamage(iDamage2);
              }
            }
            if ((this.Element & (Elements.Cold | Elements.Fire | Elements.Lightning | Elements.Steam)) == Elements.None)
              return;
            iDamage2 = new Damage();
            iDamage2.AttackProperty = AttackProperties.Damage;
            iDamage2.Element = this.Element & (Elements.Cold | Elements.Fire | Elements.Lightning | Elements.Steam);
            iDamage2.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_SELF_FIRE, this[Elements.Fire]);
            iDamage2.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_COLD, this[Elements.Cold]);
            iDamage2.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_SELF_STEAM, this[Elements.Steam]);
            iDamage2.Magnitude = Math.Min(this[Elements.Fire] + this[Elements.Cold] + this[Elements.Steam] + this[Elements.Lightning], 1f);
            oDamages.AddDamage(iDamage2);
            return;
          case CastType.Weapon:
            Damage iDamage3;
            if ((this.Element & Elements.StatusEffects) != Elements.None)
            {
              if ((this.Element & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Steam | Elements.Poison)) != Elements.None)
              {
                iDamage3 = new Damage();
                iDamage3.AttackProperty = AttackProperties.Status;
                iDamage3.Element = this.Element & Elements.StatusEffects;
                iDamage3.Amount = (float) ((double) Defines.STATUS_POISON_DAMAGE * (double) Math.Min(1f, this[Elements.Poison]) + (double) Defines.STATUS_BURN_DAMAGE * (double) Math.Min(1f, this[Elements.Fire]));
                iDamage3.Magnitude = Math.Min(1f, this[Elements.Water] + this[Elements.Fire] + this[Elements.Cold] + this[Elements.Poison] + this[Elements.Steam]);
                oDamages.AddDamage(iDamage3);
              }
              if ((this.Element & Elements.Life) != Elements.None)
              {
                iDamage3 = new Damage();
                iDamage3.AttackProperty = AttackProperties.Status;
                iDamage3.Element = this.Element & Elements.Life;
                iDamage3.Amount += this.ScaleAmount(Defines.STATUS_LIFE_DAMAGE, this[Elements.Life]);
                iDamage3.Magnitude = 1f;
                if (iCastType == CastType.Self)
                  iDamage3.Magnitude = this[Elements.Life];
                oDamages.AddDamage(iDamage3);
              }
            }
            if ((this.Element & (Elements.Cold | Elements.Fire | Elements.Lightning | Elements.Steam)) != Elements.None)
            {
              iDamage3 = new Damage();
              iDamage3.AttackProperty = AttackProperties.Damage;
              iDamage3.Element = this.Element & (Elements.Cold | Elements.Fire | Elements.Lightning | Elements.Steam);
              iDamage3.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_FIRE, this[Elements.Fire]);
              iDamage3.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_COLD, this[Elements.Cold]);
              iDamage3.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_STEAM, this[Elements.Steam]);
              iDamage3.Magnitude = Math.Min(this[Elements.Fire] + this[Elements.Cold] + this[Elements.Steam] + this[Elements.Lightning], 1f);
              oDamages.AddDamage(iDamage3);
            }
            if ((double) this[Elements.Water] <= 0.0)
              return;
            iDamage3 = new Damage();
            iDamage3.AttackProperty = AttackProperties.Pushed;
            iDamage3.Element = Elements.Water;
            iDamage3.Magnitude = this[Elements.Water];
            iDamage3.Amount = (float) (int) ((double) Defines.SPELL_STRENGTH_WATER * (double) this[Elements.Water]);
            oDamages.AddDamage(iDamage3);
            iDamage3 = new Damage();
            iDamage3.AttackProperty = AttackProperties.Damage;
            iDamage3.Element = Elements.Water;
            iDamage3.Magnitude = 1f;
            iDamage3.Amount = 0.0f;
            oDamages.AddDamage(iDamage3);
            return;
          case CastType.Magick:
            return;
          default:
            return;
        }
      case SpellType.Projectile:
        switch (iCastType)
        {
          case CastType.Force:
            Damage iDamage4;
            if ((double) this[Elements.Earth] > 0.0)
            {
              iDamage4 = new Damage();
              iDamage4.AttackProperty = AttackProperties.Damage;
              iDamage4.Element = this.Element & Elements.PhysicalElements;
              if ((double) this[Elements.Ice] > 0.0)
                iDamage4.Element |= Elements.Cold;
              iDamage4.Amount += Defines.SPELL_DAMAGE_EARTH * this[Elements.Earth] * this[Elements.Earth];
              iDamage4.Amount += Defines.SPELL_DAMAGE_ICEEARTH * this[Elements.Ice] * this[Elements.Ice];
              iDamage4.Magnitude = 1f;
              oDamages.AddDamage(iDamage4);
            }
            else if ((double) this[Elements.Ice] > 0.0)
            {
              iDamage4 = new Damage();
              iDamage4.AttackProperty = AttackProperties.Damage;
              iDamage4.Element = this.Element | Elements.Cold | Elements.Earth;
              iDamage4.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_ICEEARTH, this[Elements.Ice]);
              iDamage4.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_LIGHTNING, this[Elements.Lightning]);
              iDamage4.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_ARCANE, this[Elements.Arcane]);
              iDamage4.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_LIFE, this[Elements.Life]);
              iDamage4.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_LIFE, this[Elements.Life]);
              iDamage4.Magnitude = 1f;
              oDamages.AddDamage(iDamage4);
            }
            if ((double) this[Elements.Ice] <= 0.0 || (this.Element & Elements.StatusEffects) == Elements.None)
              return;
            iDamage4 = new Damage();
            iDamage4.AttackProperty = AttackProperties.Status;
            iDamage4.Element = this.Element & Elements.StatusEffects;
            iDamage4.Amount = Defines.STATUS_POISON_DAMAGE * Math.Min(1f, this[Elements.Poison]);
            iDamage4.Amount += Defines.STATUS_LIFE_DAMAGE * Math.Min(1f, this[Elements.Life]);
            iDamage4.Magnitude = Math.Min(1f, this[Elements.Water] + this[Elements.Cold] + this[Elements.Poison] + this[Elements.Life]);
            oDamages.AddDamage(iDamage4);
            return;
          case CastType.Area:
            Damage iDamage5;
            if ((this.Element & Elements.StatusEffects) != Elements.None)
            {
              if ((this.Element & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Steam)) != Elements.None)
              {
                iDamage5 = new Damage();
                iDamage5.AttackProperty = AttackProperties.Status;
                iDamage5.Element = this.Element & Elements.StatusEffects;
                iDamage5.Amount = Defines.STATUS_BURN_DAMAGE * Math.Min(1f, this[Elements.Fire]);
                iDamage5.Magnitude = Math.Min(1f, this[Elements.Water] + this[Elements.Fire] + this[Elements.Cold] + this[Elements.Steam]);
                oDamages.AddDamage(iDamage5);
              }
              if ((this.Element & Elements.Life) != Elements.None)
              {
                iDamage5 = new Damage();
                iDamage5.AttackProperty = AttackProperties.Status;
                iDamage5.Element = this.Element & Elements.Life;
                iDamage5.Amount += this.ScaleAmount(Defines.STATUS_LIFE_DAMAGE, this[Elements.Life]);
                iDamage5.Magnitude = 1f;
                if (iCastType == CastType.Self)
                  iDamage5.Magnitude = this[Elements.Life];
                oDamages.AddDamage(iDamage5);
              }
            }
            iDamage5 = new Damage();
            iDamage5.AttackProperty = AttackProperties.Damage;
            iDamage5.Element = this.Element & Elements.Instant | Elements.Earth;
            iDamage5.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_FIRE, this[Elements.Fire]);
            iDamage5.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_COLD, this[Elements.Cold]);
            iDamage5.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_STEAM, this[Elements.Steam]);
            iDamage5.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_LIGHTNING, this[Elements.Lightning]);
            iDamage5.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_BARRIER_ICE, this[Elements.Ice]);
            iDamage5.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_LIFE, this[Elements.Life]);
            iDamage5.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_ARCANE, this[Elements.Arcane]);
            iDamage5.Magnitude = Math.Min(this[Elements.Fire] + this[Elements.Cold] + this[Elements.Steam] + this[Elements.Lightning] + this[Elements.Ice] + this[Elements.Arcane], 1f);
            oDamages.AddDamage(iDamage5);
            if ((double) this[Elements.Water] > 0.0)
            {
              iDamage5 = new Damage();
              iDamage5.AttackProperty = AttackProperties.Pushed;
              iDamage5.Element = Elements.Water;
              iDamage5.Magnitude = this[Elements.Water];
              iDamage5.Amount = (float) (int) ((double) Defines.SPELL_STRENGTH_WATER * (double) this[Elements.Water]);
              oDamages.AddDamage(iDamage5);
            }
            if ((double) this[Elements.Earth] <= 0.0)
              return;
            iDamage5 = new Damage();
            iDamage5.AttackProperty = AttackProperties.Knockdown;
            iDamage5.Element = Elements.Earth;
            iDamage5.Magnitude = this[Elements.Earth];
            iDamage5.Amount = (float) (int) ((double) Defines.SPELL_DAMAGE_EARTH * (double) this[Elements.Earth]);
            oDamages.AddDamage(iDamage5);
            return;
          case CastType.Self:
            return;
          case CastType.Weapon:
            if ((this.Element & Elements.Ice) == Elements.Ice)
              oDamages.AddDamage(this.CalculateInstantDamage());
            oDamages.AddDamage(this.CalculateMiscEffects(iCastType));
            oDamages.AddDamage(this.CalculateStatus());
            oDamages.AddDamage(this.CalculateInstantBlast());
            return;
          default:
            return;
        }
      case SpellType.Shield:
        Damage iDamage6 = new Damage();
        if ((this.Element & ~Elements.Shield & Elements.Life) == Elements.Life)
        {
          iDamage6.AttackProperty = AttackProperties.Damage;
          iDamage6.Element = this.Element & Elements.Beams;
          iDamage6.Amount = Defines.SPELL_DAMAGE_BARRIER_LIFE;
          iDamage6.Magnitude = 1f;
          oDamages.AddDamage(iDamage6);
        }
        else if ((this.Element & ~Elements.Shield & Elements.Arcane) == Elements.Arcane)
        {
          iDamage6.AttackProperty = AttackProperties.Damage;
          iDamage6.Element = Elements.Arcane;
          iDamage6.Amount = Defines.SPELL_DAMAGE_BARRIER_ARCANE;
          iDamage6.Magnitude = 1f;
          oDamages.AddDamage(iDamage6);
        }
        Damage iDamage7;
        if ((this.Element & Elements.StatusEffects) != Elements.None)
        {
          iDamage7 = new Damage();
          iDamage7.AttackProperty = AttackProperties.Status;
          iDamage7.Element = this.Element & (Elements.StatusEffects | Elements.Arcane);
          if ((double) this[Elements.Steam] > 0.0)
            iDamage7.Element |= Elements.Water;
          iDamage7.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_FIRE, this[Elements.Fire]);
          iDamage7.Magnitude = Math.Min(1f, (float) ((double) this[Elements.Water] + (double) this[Elements.Fire] + (double) this[Elements.Cold] + (double) this[Elements.Steam] * 0.5));
          oDamages.AddDamage(iDamage7);
        }
        if ((double) this.WaterMagnitude > 0.0 && ((double) this.EarthMagnitude > 0.0 || (double) this.IceMagnitude > 0.0))
        {
          iDamage7 = new Damage();
          iDamage7.AttackProperty = AttackProperties.Pushed;
          iDamage7.Element = Elements.Water;
          float num = (float) Math.Pow((double) this.WaterMagnitude, 0.5);
          iDamage7.Amount = (float) (int) (75.0 * (double) num);
          iDamage7.Magnitude = num;
          oDamages.AddDamage(iDamage7);
        }
        if ((double) this.LightningMagnitude > 0.0)
        {
          iDamage7 = new Damage();
          iDamage7.AttackProperty = AttackProperties.Damage;
          iDamage7.Element = Elements.Lightning;
          iDamage7.Magnitude = 1f;
          iDamage7.Amount = this.ScaleAmount(Defines.SPELL_DAMAGE_BARRIER_LIGHTNING, this.LightningMagnitude);
          oDamages.AddDamage(iDamage7);
        }
        if ((double) this.EarthMagnitude + (double) this.IceMagnitude > 0.0 || (double) this.ArcaneMagnitude <= 0.0)
          break;
        iDamage7 = new Damage();
        iDamage7.AttackProperty = AttackProperties.Knockback;
        iDamage7.Element = Elements.None;
        iDamage7.Magnitude = 2f;
        iDamage7.Amount = 500f;
        oDamages.AddDamage(iDamage7);
        break;
      case SpellType.Beam:
        Damage iDamage8;
        if ((this.Element & Elements.StatusEffects) != Elements.None)
        {
          if ((this.Element & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Steam | Elements.Poison)) != Elements.None)
          {
            iDamage8 = new Damage();
            iDamage8.AttackProperty = AttackProperties.Status;
            if ((double) this[Elements.Water] > 0.0)
              iDamage8.AttackProperty |= AttackProperties.Damage;
            iDamage8.Element = this.Element & Elements.StatusEffects;
            if ((double) this[Elements.Steam] > 0.0)
              iDamage8.Element |= Elements.Water;
            iDamage8.Amount = (float) ((double) Defines.STATUS_POISON_DAMAGE * (double) Math.Min(1f, this[Elements.Poison]) + (double) Defines.STATUS_BURN_DAMAGE * (double) Math.Min(1f, this[Elements.Fire]));
            iDamage8.Magnitude = Math.Min(1f, this[Elements.Water] + this[Elements.Fire] + this[Elements.Cold] + this[Elements.Poison] + this[Elements.Steam]);
            oDamages.AddDamage(iDamage8);
          }
          if ((this.Element & Elements.Life) != Elements.None)
          {
            iDamage8 = new Damage();
            iDamage8.AttackProperty = AttackProperties.Status;
            iDamage8.Element = this.Element & Elements.Life;
            iDamage8.Amount += this.ScaleAmount(Defines.STATUS_LIFE_DAMAGE, this[Elements.Life]);
            iDamage8.Magnitude = 1f;
            if (iCastType == CastType.Self)
              iDamage8.Magnitude = this[Elements.Life];
            oDamages.AddDamage(iDamage8);
          }
        }
        iDamage8 = new Damage();
        iDamage8.AttackProperty = AttackProperties.Damage;
        iDamage8.Element = this.Element & (Elements.InstantNonPhysical | Elements.Cold | Elements.Fire);
        if (iCastType != CastType.Self)
        {
          iDamage8.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_FIRE, this[Elements.Fire]);
          iDamage8.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_COLD, this[Elements.Cold]);
          iDamage8.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_LIGHTNING * 0.5f, this[Elements.Lightning]);
          iDamage8.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_STEAM, this[Elements.Steam]);
          iDamage8.Amount += Defines.SPELL_DAMAGE_LIFE * Math.Min(1f, this[Elements.Life]);
          iDamage8.Amount += Defines.SPELL_DAMAGE_ARCANE * Math.Min(1f, this[Elements.Arcane]);
        }
        else
        {
          iDamage8.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_SELF_FIRE, this[Elements.Fire]);
          iDamage8.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_COLD, this[Elements.Cold]);
          iDamage8.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_SELF_LIGHTNING, this[Elements.Lightning]);
          iDamage8.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_SELF_STEAM, this[Elements.Steam]);
          iDamage8.Amount += Defines.SPELL_DAMAGE_LIFE * Math.Min(1f, this[Elements.Life]);
          iDamage8.Amount += Defines.SPELL_DAMAGE_SELF_ARCANE * Math.Min(1f, this[Elements.Arcane]);
        }
        iDamage8.Magnitude = 1f;
        oDamages.AddDamage(iDamage8);
        if ((double) this[Elements.Water] <= 0.0)
          break;
        iDamage8 = new Damage();
        iDamage8.AttackProperty = AttackProperties.Pushed;
        iDamage8.Element = Elements.Water;
        iDamage8.Magnitude = this[Elements.Water];
        iDamage8.Amount = (float) (int) ((double) Defines.SPELL_STRENGTH_WATER * (double) this[Elements.Water]);
        oDamages.AddDamage(iDamage8);
        break;
      case SpellType.Lightning:
        Damage iDamage9 = new Damage();
        iDamage9.AttackProperty |= AttackProperties.Damage;
        iDamage9.Element |= this.Element;
        iDamage9.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_LIGHTNING, this[Elements.Lightning]);
        iDamage9.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_ARCANE, this[Elements.Arcane]);
        iDamage9.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_LIFE, this[Elements.Life]);
        iDamage9.Magnitude = 1f;
        oDamages.AddDamage(iDamage9);
        if ((this.Element & Elements.StatusEffects) != Elements.None && (this.Element & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Steam | Elements.Poison)) != Elements.None)
        {
          Damage iDamage10 = new Damage();
          iDamage10.AttackProperty = AttackProperties.Status;
          if ((double) this[Elements.Water] > 0.0)
            iDamage10.AttackProperty |= AttackProperties.Damage;
          iDamage10.Element = this.Element & Elements.StatusEffects;
          iDamage10.Amount = (float) ((double) Defines.STATUS_POISON_DAMAGE * (double) Math.Min(1f, this[Elements.Poison]) + (double) Defines.STATUS_BURN_DAMAGE * (double) Math.Min(1f, this[Elements.Fire]));
          iDamage10.Magnitude = Math.Min(1f, this[Elements.Water] + this[Elements.Fire] + this[Elements.Cold] + this[Elements.Poison] + this[Elements.Steam]);
          oDamages.AddDamage(iDamage10);
        }
        oDamages.AddDamage(this.CalculateInstantBlast());
        oDamages.AddDamage(this.CalculateMiscEffects(CastType.Area));
        if ((this.Element & Elements.StatusEffects) == Elements.None && (double) this[Elements.Steam] <= 0.0)
          break;
        oDamages.AddDamage(this.CalculateStatus());
        break;
      default:
        throw new Exception($"No damage available for {iCastType.ToString()},{iSpellType.ToString()}, or is there?");
    }
  }

  private float ScaleAmount(float iBaseDamage, float iMagnitude)
  {
    return iBaseDamage * (float) Math.Sqrt((double) iMagnitude);
  }

  private Damage CalculateLightningDamage()
  {
    Damage lightningDamage = new Damage();
    lightningDamage.AttackProperty |= AttackProperties.Damage;
    lightningDamage.Element |= this.Element & Elements.InstantNonPhysical;
    lightningDamage.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_LIGHTNING, this[Elements.Lightning]);
    lightningDamage.Magnitude = 1f;
    return lightningDamage;
  }

  private Damage CalculateInstantDamage()
  {
    Damage instantDamage = new Damage();
    instantDamage.AttackProperty = AttackProperties.Damage;
    if ((double) this[Elements.Ice] > 0.0)
      instantDamage.AttackProperty |= AttackProperties.Piercing;
    instantDamage.Element = Elements.Earth;
    instantDamage.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_ICEEARTH, this[Elements.Ice]) * 2f;
    instantDamage.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_EARTH, this[Elements.Earth]);
    instantDamage.Magnitude = 1f;
    return instantDamage;
  }

  private Damage CalculateMiscEffects(CastType iCastType)
  {
    Damage miscEffects = new Damage();
    if ((this.Element & (Elements.Water | Elements.Arcane)) != Elements.None)
      miscEffects.AttackProperty = AttackProperties.Pushed;
    miscEffects.Element |= this.Element & Elements.Water;
    miscEffects.Magnitude += this[Elements.Water];
    miscEffects.Amount = (float) (int) (70.0 * (double) this[Elements.Water]);
    if (iCastType == CastType.Weapon || iCastType == CastType.Area)
    {
      miscEffects.Element |= this.Element & (Elements.Earth | Elements.Arcane);
      miscEffects.Amount += (float) (int) (120.0 * (double) this[Elements.Arcane]);
      miscEffects.Magnitude += this[Elements.Earth] + this[Elements.Arcane];
      if ((double) this[Elements.Earth] > 0.0)
        miscEffects.AttackProperty |= AttackProperties.Knockdown;
    }
    return miscEffects;
  }

  private Damage CalculateInstantBlast() => new Damage();

  private Damage CalculateStatus()
  {
    Damage status = new Damage();
    status.AttackProperty = AttackProperties.Status;
    status.Element = this.Element & (Elements.StatusEffects | Elements.Arcane);
    if ((double) this[Elements.Steam] > 0.0)
      status.Element |= Elements.Water;
    status.Amount += this.ScaleAmount(Defines.SPELL_DAMAGE_FIRE, this[Elements.Fire]);
    status.Magnitude = Math.Min(1f, (float) ((double) this[Elements.Water] + (double) this[Elements.Fire] + (double) this[Elements.Cold] + (double) this[Elements.Steam] * 0.5));
    return status;
  }

  public Vector3 GetColor()
  {
    Vector3 vector3 = new Vector3();
    float num = 0.0f;
    if ((this.Element & Elements.Lightning) != Elements.None)
    {
      num += this[Elements.Lightning];
      vector3 += Spell.LIGHTNINGCOLOR * this[Elements.Lightning];
    }
    if ((this.Element & Elements.Fire) != Elements.None)
    {
      num += this[Elements.Fire];
      vector3 += Spell.FIRECOLOR * this[Elements.Fire];
    }
    if ((this.Element & Elements.Cold) != Elements.None)
    {
      ++num;
      vector3 += Spell.COLDCOLOR;
    }
    if ((this.Element & Elements.Life) != Elements.None)
    {
      ++num;
      vector3 += Spell.LIFECOLOR;
    }
    if ((this.Element & Elements.Arcane) != Elements.None)
    {
      ++num;
      vector3 += Spell.ARCANECOLOR;
    }
    if ((this.Element & Elements.Water) != Elements.None)
    {
      ++num;
      vector3 += Spell.WATERCOLOR;
    }
    if ((this.Element & Elements.Ice) != Elements.None)
    {
      ++num;
      vector3 += Spell.ICECOLOR;
    }
    if ((this.Element & Elements.Earth) != Elements.None)
    {
      ++num;
      vector3 += Spell.EARTHCOLOR;
    }
    if ((this.Element & Elements.Shield) != Elements.None)
    {
      ++num;
      vector3 += Spell.SHIELDCOLOR;
    }
    return vector3 / num;
  }

  public static Vector3 GetColor(Elements iElements)
  {
    Vector3 vector3 = new Vector3();
    float num = 0.0f;
    if ((iElements & Elements.Lightning) != Elements.None)
    {
      ++num;
      vector3 += Spell.LIGHTNINGCOLOR;
    }
    if ((iElements & Elements.Fire) != Elements.None)
    {
      ++num;
      vector3 += Spell.FIRECOLOR;
    }
    if ((iElements & Elements.Cold) != Elements.None)
    {
      ++num;
      vector3 += Spell.COLDCOLOR;
    }
    if ((iElements & Elements.Life) != Elements.None)
    {
      ++num;
      vector3 += Spell.LIFECOLOR;
    }
    if ((iElements & Elements.Arcane) != Elements.None)
    {
      ++num;
      vector3 += Spell.ARCANECOLOR;
    }
    if ((iElements & Elements.Water) != Elements.None)
    {
      ++num;
      vector3 += Spell.WATERCOLOR;
    }
    if ((iElements & Elements.Ice) != Elements.None)
    {
      ++num;
      vector3 += Spell.ICECOLOR;
    }
    if ((iElements & Elements.Earth) != Elements.None)
    {
      ++num;
      vector3 += Spell.EARTHCOLOR;
    }
    if ((iElements & Elements.Shield) != Elements.None)
    {
      ++num;
      vector3 += Spell.SHIELDCOLOR;
    }
    return vector3 / num;
  }

  public bool Equals(Spell other) => this == other;

  public static Spell operator +(Spell A, Spell B)
  {
    Spell spell;
    spell.ColdMagnitude = 0.0f;
    spell.EarthMagnitude = 0.0f;
    spell.FireMagnitude = 0.0f;
    spell.LifeMagnitude = 0.0f;
    spell.IceMagnitude = 0.0f;
    spell.LightningMagnitude = 0.0f;
    spell.ArcaneMagnitude = 0.0f;
    spell.ShieldMagnitude = 0.0f;
    spell.SteamMagnitude = 0.0f;
    spell.WaterMagnitude = 0.0f;
    spell.PoisonMagnitude = 0.0f;
    spell.EarthMagnitude = Math.Max((float) ((double) A.EarthMagnitude + (double) B.EarthMagnitude - ((double) A.LightningMagnitude + (double) B.LightningMagnitude)), 0.0f);
    spell.ColdMagnitude = Math.Max((float) ((double) A.ColdMagnitude + (double) B.ColdMagnitude - ((double) A.FireMagnitude + (double) B.FireMagnitude)), 0.0f);
    spell.FireMagnitude = Math.Max((float) ((double) A.FireMagnitude + (double) B.FireMagnitude - ((double) A.ColdMagnitude + (double) B.ColdMagnitude)), 0.0f);
    spell.LightningMagnitude = Math.Max((float) ((double) A.LightningMagnitude + (double) B.LightningMagnitude - ((double) A.EarthMagnitude + (double) B.EarthMagnitude)), 0.0f);
    spell.PoisonMagnitude = Math.Max((float) ((double) A.PoisonMagnitude + (double) B.PoisonMagnitude - ((double) A.LifeMagnitude + (double) B.LifeMagnitude)), 0.0f);
    float num1 = 0.0f;
    if ((double) (spell.IceMagnitude += A.IceMagnitude + B.IceMagnitude) > 0.0 && (double) A.FireMagnitude + (double) B.FireMagnitude > 0.0)
    {
      float num2;
      spell.WaterMagnitude += num2 = num1 + Math.Min(spell.IceMagnitude - num1, A.FireMagnitude + B.FireMagnitude);
      spell.FireMagnitude -= num2;
      spell.IceMagnitude -= num2;
      num1 = 0.0f;
    }
    if ((double) (spell.SteamMagnitude += A.SteamMagnitude + B.SteamMagnitude) > 0.0 && (double) A.ColdMagnitude + (double) B.ColdMagnitude > 0.0)
    {
      float num3;
      spell.WaterMagnitude += num3 = num1 + Math.Min(spell.SteamMagnitude - num1, A.ColdMagnitude + B.ColdMagnitude);
      spell.ColdMagnitude -= num3;
      spell.SteamMagnitude -= num3;
    }
    spell.LifeMagnitude += A.LifeMagnitude + B.LifeMagnitude;
    spell.ArcaneMagnitude += A.ArcaneMagnitude + B.ArcaneMagnitude;
    spell.ShieldMagnitude += A.ShieldMagnitude + B.ShieldMagnitude;
    spell.WaterMagnitude += A.WaterMagnitude + B.WaterMagnitude;
    float num4 = 0.0f;
    float num5;
    spell.SteamMagnitude += num5 = Math.Min(A.WaterMagnitude + B.WaterMagnitude, A.FireMagnitude + B.FireMagnitude);
    spell.WaterMagnitude -= num5;
    spell.FireMagnitude -= num5;
    num4 = 0.0f;
    float num6;
    spell.IceMagnitude += num6 = Math.Min(A.WaterMagnitude + B.WaterMagnitude, A.ColdMagnitude + B.ColdMagnitude);
    spell.WaterMagnitude -= num6;
    spell.ColdMagnitude -= num6;
    spell.Element = Elements.None;
    if ((double) spell.ArcaneMagnitude > 0.0)
      spell.Element |= Elements.Arcane;
    if ((double) spell.SteamMagnitude > 0.0)
      spell.Element |= Elements.Steam;
    if ((double) spell.IceMagnitude > 0.0)
      spell.Element |= Elements.Ice;
    if ((double) spell.WaterMagnitude > 0.0)
      spell.Element |= Elements.Water;
    if ((double) spell.EarthMagnitude > 0.0)
      spell.Element |= Elements.Earth;
    if ((double) spell.LightningMagnitude > 0.0)
      spell.Element |= Elements.Lightning;
    if ((double) spell.ColdMagnitude > 0.0)
      spell.Element |= Elements.Cold;
    if ((double) spell.FireMagnitude > 0.0)
      spell.Element |= Elements.Fire;
    if ((double) spell.LifeMagnitude > 0.0)
      spell.Element |= Elements.Life;
    if ((double) spell.ShieldMagnitude > 0.0)
      spell.Element |= Elements.Shield;
    if ((double) spell.PoisonMagnitude > 0.0)
      spell.Element |= Elements.Poison;
    return spell;
  }

  public static bool operator ==(Spell A, Spell B)
  {
    return (double) A.EarthMagnitude == (double) B.EarthMagnitude && (double) A.WaterMagnitude == (double) B.WaterMagnitude && (double) A.ColdMagnitude == (double) B.ColdMagnitude && (double) A.FireMagnitude == (double) B.FireMagnitude && (double) A.LightningMagnitude == (double) B.LightningMagnitude && (double) A.LifeMagnitude == (double) B.LifeMagnitude && (double) A.ShieldMagnitude == (double) B.ShieldMagnitude && (double) A.SteamMagnitude == (double) B.SteamMagnitude && (double) A.ArcaneMagnitude == (double) B.ArcaneMagnitude && (double) A.IceMagnitude == (double) B.IceMagnitude && (double) A.PoisonMagnitude == (double) B.PoisonMagnitude || A.Element == B.Element;
  }

  public static bool operator !=(Spell A, Spell B)
  {
    return (double) A.EarthMagnitude != (double) B.EarthMagnitude || (double) A.WaterMagnitude != (double) B.WaterMagnitude || (double) A.ColdMagnitude != (double) B.ColdMagnitude || (double) A.FireMagnitude != (double) B.FireMagnitude || (double) A.LightningMagnitude != (double) B.LightningMagnitude || (double) A.LifeMagnitude != (double) B.LifeMagnitude || (double) A.ShieldMagnitude != (double) B.ShieldMagnitude || (double) A.SteamMagnitude != (double) B.SteamMagnitude || (double) A.ArcaneMagnitude != (double) B.ArcaneMagnitude || (double) A.IceMagnitude != (double) B.IceMagnitude || (double) A.PoisonMagnitude != (double) B.PoisonMagnitude || A.Element != B.Element;
  }

  public static void DefaultSpell(Elements iElements, out Spell oSpell)
  {
    oSpell = new Spell();
    oSpell.Element = iElements;
    for (int iIndex = 0; iIndex < 11; ++iIndex)
    {
      Elements elements = Defines.ElementFromIndex(iIndex);
      switch (iElements & elements)
      {
        case Elements.Earth:
          oSpell.EarthMagnitude = 1f;
          break;
        case Elements.Water:
          oSpell.WaterMagnitude = 1f;
          break;
        case Elements.Cold:
          oSpell.ColdMagnitude = 1f;
          break;
        case Elements.Fire:
          oSpell.FireMagnitude = 1f;
          break;
        case Elements.Lightning:
          oSpell.LightningMagnitude = 1f;
          break;
        case Elements.Arcane:
          oSpell.ArcaneMagnitude = 1f;
          break;
        case Elements.Life:
          oSpell.LifeMagnitude = 1f;
          break;
        case Elements.Shield:
          oSpell.ShieldMagnitude = 1f;
          break;
        case Elements.Ice:
          oSpell.IceMagnitude = 1f;
          break;
        case Elements.Steam:
          oSpell.SteamMagnitude = 1f;
          break;
        case Elements.Poison:
          oSpell.PoisonMagnitude = 1f;
          break;
      }
    }
  }

  public static void DefaultSpell(Elements[] iElements, out Spell oSpell)
  {
    oSpell = new Spell();
    foreach (Elements iElement in iElements)
    {
      oSpell.Element |= iElement;
      for (int iIndex = 0; iIndex < 11; ++iIndex)
      {
        Elements elements = Defines.ElementFromIndex(iIndex);
        switch (iElement & elements)
        {
          case Elements.Earth:
            ++oSpell.EarthMagnitude;
            break;
          case Elements.Water:
            ++oSpell.WaterMagnitude;
            break;
          case Elements.Cold:
            ++oSpell.ColdMagnitude;
            break;
          case Elements.Fire:
            ++oSpell.FireMagnitude;
            break;
          case Elements.Lightning:
            ++oSpell.LightningMagnitude;
            break;
          case Elements.Arcane:
            ++oSpell.ArcaneMagnitude;
            break;
          case Elements.Life:
            ++oSpell.LifeMagnitude;
            break;
          case Elements.Shield:
            ++oSpell.ShieldMagnitude;
            break;
          case Elements.Ice:
            ++oSpell.IceMagnitude;
            break;
          case Elements.Steam:
            ++oSpell.SteamMagnitude;
            break;
          case Elements.Poison:
            ++oSpell.PoisonMagnitude;
            break;
        }
      }
    }
  }

  internal Spell Normalize()
  {
    Spell spell = this;
    float num = this.TotalMagnitude();
    spell.EarthMagnitude /= num;
    spell.WaterMagnitude /= num;
    spell.ColdMagnitude /= num;
    spell.FireMagnitude /= num;
    spell.LightningMagnitude /= num;
    spell.LifeMagnitude /= num;
    spell.ShieldMagnitude /= num;
    spell.SteamMagnitude /= num;
    spell.ArcaneMagnitude /= num;
    spell.IceMagnitude /= num;
    spell.PoisonMagnitude /= num;
    return spell;
  }
}
