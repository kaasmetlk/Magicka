// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Zap
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class Zap : SpecialAbility
{
  private const float RANGE = 15f;
  private static List<Zap> sCache;
  private HitList mHitList = new HitList(32 /*0x20*/);
  public static readonly int SOUND = "magick_thunderbolt".GetHashCodeCustom();

  public static Zap GetInstance()
  {
    if (Zap.sCache.Count <= 0)
      return new Zap();
    Zap instance = Zap.sCache[Zap.sCache.Count - 1];
    Zap.sCache.RemoveAt(Zap.sCache.Count - 1);
    return instance;
  }

  public static void InitializeCache(int iNr)
  {
    Zap.sCache = new List<Zap>(iNr);
    for (int index = 0; index < iNr; ++index)
      Zap.sCache.Add(new Zap());
  }

  public Zap(Magicka.Animations iAnimation)
    : base(iAnimation, "#specab_lightbolt".GetHashCodeCustom())
  {
  }

  private Zap()
    : base(Magicka.Animations.cast_magick_direct, "#magick_chainlightning".GetHashCodeCustom())
  {
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    Vector3 direction = iOwner.Direction;
    LightningBolt lightning = LightningBolt.GetLightning();
    this.mHitList.Clear();
    if (!iOwner.HasStatus(StatusEffects.Wet))
      this.mHitList.Add(iOwner);
    DamageCollection5 iDamages = new DamageCollection5();
    iDamages.AddDamage(new Damage()
    {
      Amount = 850f,
      AttackProperty = AttackProperties.Damage,
      Element = Elements.Lightning,
      Magnitude = 1f
    });
    lightning.Cast(iOwner, iOwner.CastSource.Translation, direction, this.mHitList, Spell.LIGHTNINGCOLOR * 2f, 2f, 15f, ref iDamages, new Spell?(), iPlayState);
    AudioManager.Instance.PlayCue(Banks.Spells, Zap.SOUND, iOwner.AudioEmitter);
    iPlayState.Camera.CameraShake(iOwner.Position, 1.5f, 0.333f);
    return true;
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState) => false;
}
