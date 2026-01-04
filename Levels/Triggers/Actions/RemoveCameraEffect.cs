// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.RemoveCameraEffect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class RemoveCameraEffect(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private string mCameraEffectName;
  private int mCameraEffectHash;

  protected override void Execute()
  {
    if (this.mCameraEffectHash == 0)
      this.GameScene.PlayState.Camera.RemoveEffects();
    else
      this.GameScene.PlayState.Camera.RemoveEffect(this.mCameraEffectHash);
  }

  public override void QuickExecute() => this.Execute();

  public string Effect
  {
    get => this.mCameraEffectName;
    set
    {
      this.mCameraEffectName = value;
      this.mCameraEffectHash = this.mCameraEffectName.GetHashCodeCustom();
    }
  }
}
