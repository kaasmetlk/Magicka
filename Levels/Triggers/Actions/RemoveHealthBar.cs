// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.RemoveHealthBar
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class RemoveHealthBar(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  protected override void Execute() => this.GameScene.PlayState.GenericHealthBar.Remove();

  public override void QuickExecute() => this.Execute();
}
