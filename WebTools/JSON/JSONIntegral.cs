// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.JSON.JSONIntegral
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System;

#nullable disable
namespace Magicka.WebTools.JSON;

public class JSONIntegral : JSONValue
{
  private long mValue;
  private ulong mUValue;
  private bool mUnsigned;

  public JSONIntegral(string name, int value)
  {
    this.mName = name;
    this.mValue = (long) value;
  }

  public JSONIntegral(string name, short value)
  {
    this.mName = name;
    this.mValue = (long) value;
  }

  public JSONIntegral(string name, long value)
  {
    this.mName = name;
    this.mValue = value;
  }

  public JSONIntegral(string name, double value)
  {
    this.mName = name;
    this.mValue = (long) Math.Floor(value);
  }

  public JSONIntegral(string name, ulong value)
  {
    this.mName = name;
    this.mUValue = value;
    this.mUnsigned = true;
  }

  protected override string OneLine()
  {
    return this.mUnsigned ? $"\"{this.mName}\":\"{this.mUValue.ToString()}\"" : $"\"{this.mName}\":\"{this.mValue.ToString()}\"";
  }
}
