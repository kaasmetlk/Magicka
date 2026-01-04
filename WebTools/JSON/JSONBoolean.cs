// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.JSON.JSONBoolean
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.WebTools.JSON;

public class JSONBoolean : JSONValue
{
  private bool mValue;

  public JSONBoolean(string name, bool value)
  {
    this.mName = name;
    this.mValue = value;
  }

  protected override string OneLine() => $"\"{this.mName}\": \"{this.mValue}\"";
}
