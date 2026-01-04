// Decompiled with JetBrains decompiler
// Type: Magicka.Network.AuthenticationToken
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.Network;

internal struct AuthenticationToken
{
  public const int MAX_SIZE = 1024 /*0x0400*/;
  public unsafe fixed byte Data[1024];
  public int Length;
}
