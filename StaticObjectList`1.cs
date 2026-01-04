// Decompiled with JetBrains decompiler
// Type: Magicka.StaticObjectList`1
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka;

public class StaticObjectList<T>(int iCapacity) : StaticList<T>(iCapacity) where T : class
{
  public override int IndexOf(T iItem)
  {
    for (int index = 0; index < this.mCount; ++index)
    {
      if (this.mObjects[index].Equals((object) iItem))
        return index;
    }
    return -1;
  }
}
