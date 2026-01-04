// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.EpicBuff
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Entities.Buffs;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class EpicBuff : SpecialAbility
{
  private const float RANGE = 10f;
  public const float FEAR_TIME = 6f;
  public static readonly int SOUND = "magick_revive_cast".GetHashCodeCustom();
  public static readonly int EFFECT = "epic_buff".GetHashCodeCustom();
  public static readonly int FEARED_EFFECT = "magick_feared".GetHashCodeCustom();
  private AudioEmitter mAudioEmitter = new AudioEmitter();
  private AuraStorage mDamageAura;

  public EpicBuff(Magicka.Animations iAnimation)
    : base(iAnimation, "#item_steam_ability02".GetHashCodeCustom())
  {
    this.mDamageAura = new AuraStorage(new AuraBuff(new BuffStorage(new BuffBoostDamage(new Damage(AttackProperties.Damage, Elements.All, 1f, 1.1f)), VisualCategory.Offensive, new Vector3())), AuraTarget.Self, AuraType.Buff, 0, 8f, 0.0f, VisualCategory.None, new Vector3(), (int[]) null, Factions.NONE);
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    if (!(iOwner is Character))
      return false;
    Vector3 right = Vector3.Right;
    Vector3 translation = iOwner.CastSource.Translation;
    EffectManager.Instance.StartEffect(EpicBuff.EFFECT, ref translation, ref right, out VisualEffectReference _);
    (iOwner as Character).AddAura(ref this.mDamageAura, true);
    AudioManager.Instance.PlayCue(Banks.Spells, EpicBuff.SOUND, iOwner.AudioEmitter);
    return true;
  }
}
