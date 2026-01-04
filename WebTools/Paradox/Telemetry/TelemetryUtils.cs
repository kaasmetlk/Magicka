// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.Paradox.Telemetry.TelemetryUtils
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.UI;
using Magicka.Levels.Versus;
using Magicka.Misc;
using Magicka.Network;

#nullable disable
namespace Magicka.WebTools.Paradox.Telemetry;

public static class TelemetryUtils
{
  private const string SPELL_SEPERATOR = ", ";
  private const string KILLED_BY_PLAYER = "killed_by_player";
  private const string DROWNED = "drowned";
  private const string ELECTROCUTED = "electric_on_self";
  private const string ELECTROCUTED_WET = "electic_on_self_wet";
  private const string BURNT = "fire_on_self";
  private const string METEOR = "rock_on_self";
  private const string ARCANE = "arcane_on_self";

  public static object[] GetUnhandledExceptionParameters(string iVariant, string iErrorMessage)
  {
    return new object[9]
    {
      (object) iVariant,
      (object) iErrorMessage,
      (object) HardwareInfoManager.OSVersion,
      (object) HardwareInfoManager.SystemMemory,
      (object) HardwareInfoManager.GfxDevice,
      (object) HardwareInfoManager.GfxMem,
      (object) HardwareInfoManager.GfxDriver,
      (object) HardwareInfoManager.CPUType,
      (object) HardwareInfoManager.LogicalProcessors
    };
  }

  public static void SendHardwareReport()
  {
    Singleton<ParadoxServices>.Instance.TelemetryEvent("hardware_report", (object) Singleton<GameSparksAccount>.Instance.Variant, (object) Singleton<PlayerSegmentManager>.Instance.CurrentSegment.ToSegmentString(), (object) HardwareInfoManager.OSVersion, (object) HardwareInfoManager.SystemMemory, (object) HardwareInfoManager.GfxDevice, (object) HardwareInfoManager.GfxMem, (object) HardwareInfoManager.GfxDriver, (object) HardwareInfoManager.CPUType, (object) HardwareInfoManager.LogicalProcessors);
  }

  public static void SendDLCPromotionClicked()
  {
    Singleton<ParadoxServices>.Instance.TelemetryEvent("dlc_ad_clicked", (object) Singleton<GameSparksAccount>.Instance.Variant, (object) Singleton<PlayerSegmentManager>.Instance.CurrentSegment.ToSegmentString(), (object) DLC_StatusHelper.Instance.CurrentPromotion_Name, (object) DLC_StatusHelper.Instance.CurrentPromotion_Name, (object) DLC_StatusHelper.Instance.CurrentPromotion_AppID);
  }

  public static EventData GetGameplayStartedData(
    string iGameType,
    string iLevelName,
    int iPlayerCount)
  {
    return new EventData("gameplay_started", new object[6]
    {
      (object) Singleton<GameSparksAccount>.Instance.Variant,
      (object) Singleton<PlayerSegmentManager>.Instance.CurrentSegment.ToSegmentString(),
      (object) iGameType,
      (object) iLevelName,
      (object) iPlayerCount,
      (object) NetworkManager.Instance.State
    });
  }

  public static EventData GetTutorialActionData(
    EventEnums.TutorialAction iAction,
    string iTutorialName,
    string iTutorialStep,
    int iTimeSpent)
  {
    return new EventData("tutorial_action", new object[6]
    {
      (object) Singleton<GameSparksAccount>.Instance.Variant,
      (object) Singleton<PlayerSegmentManager>.Instance.CurrentSegment.ToSegmentString(),
      (object) iAction,
      (object) iTutorialName,
      (object) iTutorialStep,
      (object) iTimeSpent
    });
  }

  public static void SendTutorialAction(
    EventEnums.TutorialAction iAction,
    string iTutorialName,
    string iTutorialStep,
    int iTimeSpent)
  {
    Singleton<ParadoxServices>.Instance.TelemetryEvent("tutorial_action", (object) Singleton<GameSparksAccount>.Instance.Variant, (object) Singleton<PlayerSegmentManager>.Instance.CurrentSegment.ToSegmentString(), (object) iAction, (object) iTutorialName, (object) iTutorialStep, (object) iTimeSpent);
  }

  public static void SendCollectSpellbook(MagickType iMagickType)
  {
    Singleton<ParadoxServices>.Instance.TelemetryEvent("collect_spellbook", (object) Singleton<GameSparksAccount>.Instance.Variant, (object) Singleton<PlayerSegmentManager>.Instance.CurrentSegment.ToSegmentString(), (object) iMagickType);
  }

  public static void SendInGameMenuButtonPressTelemetry(
    string iGameTypeString,
    string iLevelNameString,
    string iButtonName)
  {
    Singleton<ParadoxServices>.Instance.TelemetryEvent("ingame_menu_clicked", (object) Singleton<GameSparksAccount>.Instance.Variant, (object) Singleton<PlayerSegmentManager>.Instance.CurrentSegment.ToSegmentString(), (object) iGameTypeString, (object) iLevelNameString, (object) Game.Instance.PlayerCount, (object) NetworkManager.Instance.State, (object) iButtonName);
  }

  public static EventData GetControllerChangedData(Magicka.GameLogic.Player[] iPlayers)
  {
    EventEnums.ControllerType[] controllerTypeArray = new EventEnums.ControllerType[4];
    for (int index = 0; index < iPlayers.Length; ++index)
    {
      Magicka.GameLogic.Player iPlayer = iPlayers[index];
      controllerTypeArray[index] = iPlayer == null || iPlayer.Controller == null ? EventEnums.ControllerType.NotApplicable : (iPlayer.Controller is XInputController || iPlayer.Controller is DirectInputController ? EventEnums.ControllerType.Gamepad : (!(iPlayer.Controller is KeyboardMouseController) ? EventEnums.ControllerType.NotApplicable : EventEnums.ControllerType.Keyboard));
    }
    return new EventData("controller_setup", new object[6]
    {
      (object) Singleton<GameSparksAccount>.Instance.Variant,
      (object) Singleton<PlayerSegmentManager>.Instance.CurrentSegment.ToSegmentString(),
      (object) controllerTypeArray[0],
      (object) controllerTypeArray[1],
      (object) controllerTypeArray[2],
      (object) controllerTypeArray[3]
    });
  }

  public static void SendSpellCast(Character iCharacter)
  {
    Spell spell = iCharacter.Spell;
    string str = string.Empty + TelemetryUtils.GetSpellCombo("earth", (int) spell.EarthMagnitude) + TelemetryUtils.GetSpellCombo("water", (int) spell.WaterMagnitude) + TelemetryUtils.GetSpellCombo("cold", (int) spell.ColdMagnitude) + TelemetryUtils.GetSpellCombo("fire", (int) spell.FireMagnitude) + TelemetryUtils.GetSpellCombo("lightning", (int) spell.LightningMagnitude) + TelemetryUtils.GetSpellCombo("arcane", (int) spell.ArcaneMagnitude) + TelemetryUtils.GetSpellCombo("life", (int) spell.LifeMagnitude) + TelemetryUtils.GetSpellCombo("shield", (int) spell.ShieldMagnitude) + TelemetryUtils.GetSpellCombo("ice", (int) spell.IceMagnitude) + TelemetryUtils.GetSpellCombo("steam", (int) spell.SteamMagnitude) + TelemetryUtils.GetSpellCombo("poison", (int) spell.PoisonMagnitude);
    if (str.EndsWith(", "))
      str = str.Substring(0, str.Length - 2);
    Singleton<ParadoxServices>.Instance.TelemetryEvent("spell_cast", (object) Singleton<GameSparksAccount>.Instance.Variant, (object) Singleton<PlayerSegmentManager>.Instance.CurrentSegment.ToSegmentString(), (object) iCharacter.PlayState.GameType.ToString(), (object) iCharacter.PlayState.Level.Name, (object) str);
  }

  private static string GetSpellCombo(string iElement, int iCount)
  {
    string spellCombo = string.Empty;
    if (iCount != 0)
    {
      for (int index = 0; index < iCount; ++index)
        spellCombo = spellCombo + iElement + ", ";
    }
    return spellCombo;
  }

  public static void SendPlayerDeath(Character iCharacter)
  {
    string str = string.Empty;
    bool flag1 = iCharacter.LastAttacker is Avatar lastAttacker && lastAttacker.Player != null;
    bool flag2 = iCharacter.LastAttacker is NonPlayerCharacter;
    bool flag3 = iCharacter.PlayState.Level.CurrentScene.RuleSet is VersusRuleset;
    EventEnums.DeathCategory deathCategory;
    if (iCharacter == iCharacter.LastAttacker)
    {
      deathCategory = EventEnums.DeathCategory.Suicide;
      if ((iCharacter.LastDamageElement & Elements.Lightning) == Elements.Lightning)
        str = iCharacter.HasStatus(StatusEffects.Wet) ? "electic_on_self_wet" : "electric_on_self";
      else if (iCharacter.HasStatus(StatusEffects.Burning) && (iCharacter.LastDamageElement & Elements.Fire) == Elements.Fire)
        str = "fire_on_self";
      else if ((iCharacter.LastDamageElement & Elements.Earth) == Elements.Earth)
        str = "rock_on_self";
      else if ((iCharacter.LastDamageElement & Elements.Arcane) == Elements.Arcane)
        str = "arcane_on_self";
    }
    else if (flag1)
    {
      deathCategory = flag3 ? EventEnums.DeathCategory.PlayerVsPlayer : EventEnums.DeathCategory.FriendlyFire;
      str = "killed_by_player";
    }
    else
    {
      deathCategory = flag2 ? EventEnums.DeathCategory.NPC : EventEnums.DeathCategory.Environment;
      if (iCharacter.Drowning)
        str = "drowned";
      else if (iCharacter.LastAttacker != null)
        str = !(iCharacter.LastAttacker is BossDamageZone) ? (iCharacter.LastAttacker as NonPlayerCharacter).Name : (iCharacter.LastAttacker as BossDamageZone).Owner.GetBossType().ToString();
    }
    Singleton<ParadoxServices>.Instance.TelemetryEvent("player_death", (object) Singleton<GameSparksAccount>.Instance.Variant, (object) Singleton<PlayerSegmentManager>.Instance.CurrentSegment.ToSegmentString(), (object) iCharacter.PlayState.GameType.ToString(), (object) iCharacter.PlayState.Level.Name, (object) deathCategory, (object) str);
  }
}
