// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.JSON.JSONFloatingPoint
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.WebTools.JSON;

public class JSONFloatingPoint : JSONValue
{
  private double mValue;

  public JSONFloatingPoint(string name, float value)
  {
    this.mName = name;
    this.mValue = (double) value;
  }

  public JSONFloatingPoint(string name, double value)
  {
    this.mName = name;
    this.mValue = value;
  }

  protected override string OneLine() => $"\"{this.mName}\": \"{this.mValue}\"";
}
