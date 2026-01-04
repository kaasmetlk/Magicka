// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.JSON.JSONValue
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.WebTools.JSON;

public abstract class JSONValue
{
  protected string mName;

  public virtual string JSONLine() => this.JSONLine(true, true);

  public string JSONLine(bool addComma, bool addNewLine)
  {
    return $"{this.OneLine()}{(addComma ? (object) ", " : (object) "")}{(addNewLine ? (object) "\n" : (object) "")}".Trim();
  }

  protected abstract string OneLine();
}
