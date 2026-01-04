// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Doom_Zap
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

public class Doom_Zap : SpecialAbility
{
  private const float RANGE = 10f;
  private const float DAMAGE = 300f;
  private static List<Doom_Zap> sCache = (List<Doom_Zap>) null;
  private static readonly int OTHER_GLOVE_NAME = "weapon_doomgauntlet".GetHashCodeCustom();
  private HitList mHitList = new HitList(32 /*0x20*/);
  public static readonly int SOUND = "magick_thunderbolt".GetHashCodeCustom();

  public static Doom_Zap GetInstance()
  {
    if (Doom_Zap.sCache.Count <= 0)
      return new Doom_Zap();
    Doom_Zap instance = Doom_Zap.sCache[Doom_Zap.sCache.Count - 1];
    Doom_Zap.sCache.RemoveAt(Doom_Zap.sCache.Count - 1);
    return instance;
  }

  public static void InitializeCache(int iNr)
  {
    Doom_Zap.sCache = new List<Doom_Zap>(iNr);
    for (int index = 0; index < iNr; ++index)
      Doom_Zap.sCache.Add(new Doom_Zap());
  }

  public Doom_Zap(Magicka.Animations iAnimation)
    : base(iAnimation, "#specab_doomzap".GetHashCodeCustom())
  {
  }

  private Doom_Zap()
    : base(Magicka.Animations.cast_magick_direct, "#specab_doomzap".GetHashCodeCustom())
  {
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    Vector3 direction = iOwner.Direction;
    LightningBolt lightning1 = LightningBolt.GetLightning();
    LightningBolt lightning2 = LightningBolt.GetLightning();
    this.mHitList.Clear();
    this.mHitList.Add(iOwner.Handle, 1f);
    DamageCollection5 iDamages = new DamageCollection5();
    iDamages.AddDamage(new Damage()
    {
      Amount = 300f,
      AttackProperty = AttackProperties.Damage,
      Element = Elements.Lightning,
      Magnitude = 1f
    });
    Quaternion fromAxisAngle1 = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(30f));
    Vector3 result;
    Vector3.Transform(ref direction, ref fromAxisAngle1, out result);
    lightning1.Cast(iOwner, iOwner.CastSource.Translation, result, this.mHitList, Spell.LIGHTNINGCOLOR * 2f, 2f, 10f, ref iDamages, new Spell?(), iPlayState);
    AudioManager.Instance.PlayCue(Banks.Spells, Doom_Zap.SOUND, iOwner.AudioEmitter);
    iPlayState.Camera.CameraShake(iOwner.Position, 1.5f, 0.333f);
    Quaternion fromAxisAngle2 = Quaternion.CreateFromAxisAngle(Vector3.Down, MathHelper.ToRadians(30f));
    Vector3.Transform(ref direction, ref fromAxisAngle2, out result);
    this.mHitList.Clear();
    this.mHitList.Add(iOwner.Handle, 1f);
    if (iOwner is Character && ((Character) iOwner).Equipment[0].Item.Type == Doom_Zap.OTHER_GLOVE_NAME)
    {
      iPlayState.Camera.CameraShake(iOwner.Position, 1.5f, 0.333f);
      AudioManager.Instance.PlayCue(Banks.Spells, Doom_Zap.SOUND, iOwner.AudioEmitter);
      lightning2.Cast(iOwner, ((Character) iOwner).Equipment[0].Item.Position, result, this.mHitList, Spell.LIGHTNINGCOLOR * 2f, 2f, 10f, ref iDamages, new Spell?(), iPlayState);
    }
    return true;
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState) => false;
}
