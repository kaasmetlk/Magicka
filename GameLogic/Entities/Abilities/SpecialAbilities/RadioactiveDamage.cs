// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.RadioactiveDamage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class RadioactiveDamage(Magicka.Animations iAnimation) : SpecialAbility(iAnimation, Helper.GetHashCodeCustom("#item_specab_radioactive"))
{
  private static readonly int EFFECT = "radioactive_damage".GetHashCodeCustom();
  public static readonly int SOUND = "magick_raise_dead".GetHashCodeCustom();

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    Vector3 position = iOwner.Position;
    Vector3 direction = iOwner.Direction;
    EffectManager.Instance.StartEffect(RadioactiveDamage.EFFECT, ref position, ref direction, out VisualEffectReference _);
    int num = (int) Helper.CircleDamage(iPlayState, iOwner as Entity, iPlayState.PlayTime, iOwner as Entity, ref position, 7.5f, ref new Damage()
    {
      Amount = 20f,
      Magnitude = 1f,
      Element = Elements.Poison,
      AttackProperty = AttackProperties.Status
    });
    AudioManager.Instance.PlayCue(Banks.Spells, RadioactiveDamage.SOUND, iOwner.AudioEmitter);
    return true;
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState) => false;
}
