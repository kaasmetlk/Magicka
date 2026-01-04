// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.Paradox.Telemetry.TypeValidators.PlayerSegmentTypeValidator
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.WebTools.Paradox.Telemetry.TypeValidators;

public class PlayerSegmentTypeValidator : BaseTypeValidator<string>
{
  protected override bool OnMatchType(string iValue) => PlayerSegment.IsSegmentString(iValue);
}
