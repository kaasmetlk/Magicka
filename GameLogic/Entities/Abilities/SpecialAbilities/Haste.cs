// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Haste
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using PolygonHead;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class Haste : SpecialAbility, IAbilityEffect
{
  private const float TTL = 10f;
  private static List<Haste> sCache;
  private static List<Haste> sActiveHastes;
  public static readonly int SOUNDHASH = "magick_haste".GetHashCodeCustom();
  private Magicka.GameLogic.Entities.Character mOwner;
  private float mTTL;
  private bool mOverrideTTL;
  private float mCustomTTL;

  public static Haste GetInstance()
  {
    if (Haste.sCache.Count > 0)
    {
      Haste instance = Haste.sCache[Haste.sCache.Count - 1];
      Haste.sCache.RemoveAt(Haste.sCache.Count - 1);
      Haste.sActiveHastes.Add(instance);
      return instance;
    }
    Haste instance1 = new Haste();
    Haste.sActiveHastes.Add(instance1);
    return instance1;
  }

  public static void InitializeCache(int iNr)
  {
    Haste.sCache = new List<Haste>(iNr);
    Haste.sActiveHastes = new List<Haste>(iNr);
    for (int index = 0; index < iNr; ++index)
      Haste.sCache.Add(new Haste());
  }

  private Haste()
    : base(Magicka.Animations.cast_magick_self, "#magick_haste".GetHashCodeCustom())
  {
  }

  public float CustomTTL
  {
    get => this.mCustomTTL;
    set
    {
      this.mCustomTTL = value;
      this.mOverrideTTL = true;
    }
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    return this.Execute(iOwner, iPlayState, true);
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    throw new Exception("Haste have to be cast by a character!");
  }

  public bool Execute(ISpellCaster iOwner, PlayState iPlayState, bool iPlaySound)
  {
    base.Execute(iOwner, iPlayState);
    if (!(iOwner is Magicka.GameLogic.Entities.Character))
    {
      this.OnRemove();
      return false;
    }
    for (int index = 0; index < Haste.sActiveHastes.Count; ++index)
    {
      if (Haste.sActiveHastes[index].mOwner == iOwner)
      {
        Haste.sActiveHastes[index].mTTL = !this.mOverrideTTL ? 10f : Math.Max(this.mCustomTTL, Haste.sActiveHastes[index].mTTL);
        if (iPlaySound)
          AudioManager.Instance.PlayCue(Banks.Spells, Haste.SOUNDHASH, iOwner.AudioEmitter);
        this.OnRemove();
        return true;
      }
    }
    this.mOwner = iOwner as Magicka.GameLogic.Entities.Character;
    this.mTTL = !this.mOverrideTTL ? 10f : this.mCustomTTL;
    if (this.mOwner != null)
    {
      if (iPlaySound)
        AudioManager.Instance.PlayCue(Banks.Spells, Haste.SOUNDHASH, this.mOwner.AudioEmitter);
      SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
      return true;
    }
    this.OnRemove();
    return false;
  }

  public bool IsDead => (double) this.mTTL <= 0.0;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mTTL -= iDeltaTime;
    if (this.mOwner.CharacterBody.IsBodyEnabled)
      this.mOwner.CharacterBody.SpeedMultiplier *= (float) (1.0 + (1.0 - Math.Pow(0.5, (double) this.mTTL)));
    else
      this.mTTL = 0.0f;
  }

  public void OnRemove()
  {
    this.mOverrideTTL = false;
    this.mOwner = (Magicka.GameLogic.Entities.Character) null;
    Haste.sActiveHastes.Remove(this);
    Haste.sCache.Add(this);
  }
}
