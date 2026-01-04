// Decompiled with JetBrains decompiler
// Type: Magicka.AI.AgentStates.IAIState
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.AI.AgentStates;

public interface IAIState
{
  void OnEnter(IAI iOwner);

  void OnExit(IAI iOwner);

  void OnExecute(IAI iOwner, float dTime);

  void IncrementEvent(IAI iOwner);
}
