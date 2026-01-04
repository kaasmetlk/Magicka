// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.CameraBias
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class CameraBias(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private Vector3 mBias;
  private float mTime = 20f;

  protected override void Execute()
  {
    this.GameScene.PlayState.Camera.SetBias(ref this.mBias, this.mTime);
  }

  public override void QuickExecute()
  {
  }

  public Vector3 Bias
  {
    get => this.mBias;
    set => this.mBias = value;
  }

  public float Time
  {
    get => this.mTime;
    set => this.mTime = value;
  }
}
