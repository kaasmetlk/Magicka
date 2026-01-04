// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.Flash
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class Flash(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private float mDuration = 0.5f;

  protected override void Execute()
  {
    Magicka.Graphics.Flash.Instance.Execute(this.mScene.PlayState.Scene, this.mDuration);
  }

  public override void QuickExecute() => this.Execute();

  public float Duration
  {
    get => this.mDuration;
    set => this.mDuration = value;
  }
}
