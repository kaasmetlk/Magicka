// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.GameSparks.PropertySets.AdsABTestPropertySet
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.WebTools.GameSparks.PropertySets;

internal class AdsABTestPropertySet : GameSparksPropertySet
{
  private const string AD_PROPERTY_NAME = "Ads";
  private const string SET_NAME = "ABTestAdLevel";

  public override string Name => "ABTestAdLevel";

  protected override void SetDefaults()
  {
    this.mDefaultProperties.Add("Ads", new GameSparksPropertySet.Property("Ads", GameSparksPropertySet.PropertyType.BOOL, (object) true));
  }
}
