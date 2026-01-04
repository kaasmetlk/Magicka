// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.CameraShake
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class CameraShake(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private float mMagnitude;
  private float mTime;
  private string mArea;
  private int mAreaID;

  protected override void Execute()
  {
    if (string.IsNullOrEmpty(this.mArea))
    {
      this.GameScene.PlayState.Camera.CameraShake(this.mMagnitude, this.mTime);
    }
    else
    {
      Matrix oLocator;
      this.GameScene.GetLocator(this.mAreaID, out oLocator);
      this.GameScene.PlayState.Camera.CameraShake(oLocator.Translation, this.mMagnitude, this.mTime);
    }
  }

  public override void QuickExecute()
  {
  }

  public string Area
  {
    get => this.mArea;
    set
    {
      this.mArea = value;
      this.mAreaID = this.mArea.GetHashCodeCustom();
    }
  }

  public float Magnitude
  {
    get => this.mMagnitude;
    set => this.mMagnitude = value;
  }

  public float Time
  {
    get => this.mTime;
    set => this.mTime = value;
  }
}
