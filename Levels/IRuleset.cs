// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.IRuleset
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Network;
using PolygonHead;

#nullable disable
namespace Magicka.Levels;

public interface IRuleset
{
  int GetAnyArea();

  void Update(float iDeltaTime, DataChannel iDataChannel);

  void LocalUpdate(float iDeltaTime, DataChannel iDataChannel);

  void NetworkUpdate(ref RulesetMessage iMsg);

  void Initialize();

  void DeInitialize();

  Rulesets RulesetType { get; }
}
