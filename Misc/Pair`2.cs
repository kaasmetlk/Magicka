// Decompiled with JetBrains decompiler
// Type: Magicka.Misc.Pair`2
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.Misc;

public class Pair<T, U>
{
  public Pair()
  {
  }

  public Pair(T first, U second)
  {
    this.First = first;
    this.Second = second;
  }

  public T First { get; set; }

  public U Second { get; set; }
}
