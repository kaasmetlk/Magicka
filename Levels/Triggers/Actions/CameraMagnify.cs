// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.CameraMagnify
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Graphics;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class CameraMagnify(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private float mMagnification = 1f;
  private float mTime = 1f;
  private MagickCamera mCamera;

  public override void Initialize()
  {
    base.Initialize();
    this.mCamera = this.GameScene.PlayState.Camera;
  }

  protected override void Execute()
  {
    this.mCamera.Magnification = this.mMagnification;
    this.mCamera.Time = this.mTime;
  }

  public override void QuickExecute()
  {
  }

  public float Magnification
  {
    get => this.mMagnification;
    set => this.mMagnification = value;
  }

  public float Time
  {
    get => this.mTime;
    set => this.mTime = value;
  }
}
