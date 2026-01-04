// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.DisableElement
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Graphics;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class DisableElement(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private Elements mElement;

  protected override void Execute() => TutorialManager.Instance.DisableElement(this.mElement);

  public override void QuickExecute() => this.Execute();

  public Elements Element
  {
    get => this.mElement;
    set => this.mElement = value;
  }
}
