// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.CameraFollow
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using System;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class CameraFollow : Action
{
  private string mTarget;
  private int mTargetID;

  public CameraFollow(Trigger iTrigger, GameScene iScene)
    : base(iTrigger, iScene)
  {
    this.mScene = iScene;
  }

  protected override void Execute()
  {
    this.mScene.PlayState.Camera.Follow(Entity.GetByID(this.mTargetID));
  }

  public override void QuickExecute()
  {
  }

  public string ID
  {
    get => this.mTarget;
    set
    {
      this.mTarget = value;
      this.mTargetID = this.mTarget.GetHashCodeCustom();
      if (string.IsNullOrEmpty(this.mTarget))
        throw new Exception("Must input correct ID");
    }
  }
}
