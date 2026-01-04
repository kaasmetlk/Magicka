// Decompiled with JetBrains decompiler
// Type: Magicka.AI.AgentStates.AIStateBreakFree
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System;

#nullable disable
namespace Magicka.AI.AgentStates;

internal class AIStateBreakFree : IAIState
{
  private static AIStateBreakFree sSingelton;

  public static AIStateBreakFree Instance
  {
    get
    {
      if (AIStateBreakFree.sSingelton == null)
        AIStateBreakFree.sSingelton = new AIStateBreakFree();
      return AIStateBreakFree.sSingelton;
    }
  }

  public void OnEnter(IAI iOwner)
  {
  }

  public void OnExit(IAI iOwner)
  {
  }

  public void IncrementEvent(IAI iOwner)
  {
  }

  public void OnExecute(IAI iOwner, float dTime)
  {
    if (!(iOwner is Agent agent))
      throw new NotImplementedException();
    iOwner.Owner.BreakFree();
    if (!(!iOwner.Owner.IsGripped & !iOwner.Owner.IsEntangled))
      return;
    agent.PopState();
  }
}
