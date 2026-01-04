// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.Paradox.Telemetry.TypeValidators.MagickTypeTypeValidator
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Spells;

#nullable disable
namespace Magicka.WebTools.Paradox.Telemetry.TypeValidators;

public class MagickTypeTypeValidator : BaseTypeValidator<MagickType>
{
  private const string THUNDER_BOLT = "thunder_bolt";
  private const string METEOR_SHOWER = "meteor_shower";
  private const string THUNDER_STORM = "thunder_storm";
  private const string SUMMON_UNDEAD = "summon_undead";
  private const string SUMMON_ELEMENTAL = "summon_elemental";
  private const string SUMMON_DEATH = "summon_death";
  private const string SUMMON_PHOENIX = "summon_phoenix";
  private const string CRASH_TO_DESKTOP = "crash_to_desktop";

  protected override string ToString(MagickType iValue)
  {
    string empty = string.Empty;
    string str;
    switch (iValue)
    {
      case MagickType.ThunderB:
        str = "thunder_bolt";
        break;
      case MagickType.MeteorS:
        str = "meteor_shower";
        break;
      case MagickType.ThunderS:
        str = "thunder_storm";
        break;
      case MagickType.SUndead:
        str = "summon_undead";
        break;
      case MagickType.SElemental:
        str = "summon_elemental";
        break;
      case MagickType.SDeath:
        str = "summon_death";
        break;
      case MagickType.SPhoenix:
        str = "summon_phoenix";
        break;
      case MagickType.CTD:
        str = "crash_to_desktop";
        break;
      default:
        str = base.ToString(iValue);
        break;
    }
    return str;
  }
}
