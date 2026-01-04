// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.SetMusicFocus
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class SetMusicFocus(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private string mFocusJoint;
  private int mFocusJointID;
  private float mFocusRadius;
  private float mDefaultValue = 1f;

  protected override void Execute()
  {
    Matrix oLocator;
    this.GameScene.GetLocator(this.mFocusJointID, out oLocator);
    AudioManager.Instance.SetMusicFocus(oLocator.Translation, this.mFocusRadius, this.mDefaultValue);
  }

  public override void QuickExecute() => this.Execute();

  public string ID
  {
    get => this.mFocusJoint;
    set
    {
      this.mFocusJoint = value;
      this.mFocusJointID = this.mFocusJoint.GetHashCodeCustom();
    }
  }

  public float Radius
  {
    get => this.mFocusRadius;
    set => this.mFocusRadius = value;
  }

  public float ClearTo
  {
    get => this.mDefaultValue;
    set => this.mDefaultValue = value;
  }
}
