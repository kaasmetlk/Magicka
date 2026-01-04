// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.WizardControl
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class WizardControl(Trigger iTrigger, GameScene iScene) : Magicka.Levels.Triggers.Actions.Action(iTrigger, iScene)
{
  private WizardAction mAction;

  protected override void Execute()
  {
    switch (this.mAction)
    {
      case WizardAction.StaffLightOn:
        this.GameScene.PlayState.StaffLight = true;
        break;
      case WizardAction.StaffLightOff:
        this.GameScene.PlayState.StaffLight = false;
        break;
    }
  }

  public override void QuickExecute() => this.Execute();

  public WizardAction Action
  {
    get => this.mAction;
    set => this.mAction = value;
  }
}
