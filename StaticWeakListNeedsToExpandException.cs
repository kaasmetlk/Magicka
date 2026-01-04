// Decompiled with JetBrains decompiler
// Type: Magicka.StaticWeakListNeedsToExpandException
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System;

#nullable disable
namespace Magicka;

public class StaticWeakListNeedsToExpandException(int currentCount, string attemptedAdd) : Exception($"StaticWeakList with {currentCount} elements is full ! Failed to add {attemptedAdd}, need to expand.")
{
}
