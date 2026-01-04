// Decompiled with JetBrains decompiler
// Type: Magicka.PathFinding.TriangleNode
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.PathFinding;

public struct TriangleNode
{
  public float Cost;
  public float TotalCost;
  public ushort Mesh;
  public ushort Id;
  public uint ParentId;

  public uint LongId => (uint) this.Mesh << 16 /*0x10*/ | (uint) this.Id;
}
