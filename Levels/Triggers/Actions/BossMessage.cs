// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.BossMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities.Bosses;
using System;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class BossMessage(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private BossMessages mMessage;

  protected override void Execute() => BossFight.Instance.PassMessage(this.mMessage);

  public override void QuickExecute() => this.Execute();

  public string Message
  {
    get => "";
    set => this.mMessage = (BossMessages) Enum.Parse(typeof (BossMessages), value, true);
  }
}
