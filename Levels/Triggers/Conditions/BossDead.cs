// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Conditions.BossDead
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;

#nullable disable
namespace Magicka.Levels.Triggers.Conditions;

public class BossDead(GameScene iScene) : Condition(iScene)
{
  public override bool IsMet(Character iSender)
  {
    return this.Scene.PlayState.BossFight != null && base.IsMet(iSender);
  }

  protected override bool InternalMet(Character iSender) => this.Scene.PlayState.BossFight.Dead;
}
