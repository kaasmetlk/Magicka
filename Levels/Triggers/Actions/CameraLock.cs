// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.CameraLock
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.Graphics;
using System;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class CameraLock : Action
{
  private float mTime = 1f;
  private string mTarget;
  private int mTargetID;
  private new GameScene mScene;
  private MagickCamera mCamera;

  public CameraLock(Trigger iTrigger, GameScene iScene)
    : base(iTrigger, iScene)
  {
    this.mScene = iScene;
  }

  public override void Initialize() => this.mCamera = this.GameScene.PlayState.Camera;

  protected override void Execute()
  {
    this.mCamera.LockOn(this.mScene.GetTriggerArea(this.mTargetID).CollisionSkin.GetPrimitiveLocal(0) as Box, this.mTime);
  }

  public override void QuickExecute()
  {
  }

  public float Time
  {
    get => this.mTime;
    set => this.mTime = value;
  }

  public string Area
  {
    get => this.mTarget;
    set
    {
      this.mTarget = value;
      this.mTargetID = this.mTarget.GetHashCodeCustom();
      if (string.IsNullOrEmpty(this.mTarget))
        throw new Exception("Must input area");
    }
  }
}
