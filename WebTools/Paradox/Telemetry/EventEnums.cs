// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.Paradox.Telemetry.EventEnums
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.WebTools.Paradox.Telemetry;

public static class EventEnums
{
  public enum TutorialAction
  {
    Started,
    Completed,
    Failed,
    Skipped,
    Restarted,
    Quit,
  }

  public enum ControllerType
  {
    Keyboard,
    Gamepad,
    NotApplicable,
  }

  public enum DeathCategory
  {
    Unknown,
    Suicide,
    PlayerVsPlayer,
    FriendlyFire,
    NPC,
    Environment,
  }
}
