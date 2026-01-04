// Decompiled with JetBrains decompiler
// Type: Magicka.Network.ActionType
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.Network;

public enum ActionType : byte
{
  ConjureElement,
  CastSpell,
  Attack,
  Block,
  PickUp,
  PickUpRequest,
  Interact,
  Boost,
  BreakFree,
  Grip,
  GripAttack,
  Entangle,
  Release,
  Dash,
  Jump,
  ItemSpecial,
  Magick,
  SelfShield,
  EventAnimation,
  EventComplete,
}
