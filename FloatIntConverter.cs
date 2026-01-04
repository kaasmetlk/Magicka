// Decompiled with JetBrains decompiler
// Type: Magicka.FloatIntConverter
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System.Runtime.InteropServices;

#nullable disable
namespace Magicka;

[StructLayout(LayoutKind.Explicit)]
public struct FloatIntConverter
{
  [FieldOffset(0)]
  public float Float;
  [FieldOffset(0)]
  public int Int;

  public FloatIntConverter(float _float)
  {
    this.Int = 0;
    this.Float = _float;
  }

  public FloatIntConverter(int _int)
  {
    this.Float = 0.0f;
    this.Int = _int;
  }
}
