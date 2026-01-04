// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.CameraRelease
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class CameraRelease(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private float mTime = 1f;

  protected override void Execute() => this.GameScene.PlayState.Camera.Release(this.mTime);

  public override void QuickExecute()
  {
  }

  public float Time
  {
    get => this.mTime;
    set => this.mTime = value;
  }
}
