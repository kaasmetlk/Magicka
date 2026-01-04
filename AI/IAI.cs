// Decompiled with JetBrains decompiler
// Type: Magicka.AI.IAI
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.AI.AgentStates;
using Magicka.GameLogic.Entities;
using Magicka.PathFinding;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

#nullable disable
namespace Magicka.AI;

public interface IAI
{
  Character Owner { get; }

  AIEvent[] Events { get; }

  int CurrentEvent { get; set; }

  bool LoopEvents { get; }

  Vector3 WayPoint { get; set; }

  float CurrentEventDelay { get; set; }

  List<PathNode> Path { get; }

  float CurrentStateAge { get; }

  float WanderAngle { get; set; }

  MovementProperties MoveAbilities { get; }

  Dictionary<byte, Animations[]> MoveAnimations { get; }

  void PushState(IAIState iAIState);

  void PopState();
}
