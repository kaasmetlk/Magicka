// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.PhysAnimationControl
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Levels;

#nullable disable
namespace Magicka.GameLogic.Entities;

internal struct PhysAnimationControl
{
  public AnimatedLevelPart AnimatedLevelPart;
  public float Start;
  public float End;
  public float Speed;
  public bool Children;
}
