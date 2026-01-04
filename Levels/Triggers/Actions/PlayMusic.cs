// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.PlayMusic
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using System;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class PlayMusic(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private Banks mSoundBank = Banks.Music;
  private string mCue;
  private int mCueID;
  private float? mFocusValue;

  protected override void Execute()
  {
    this.GameScene.PlayState.PlayMusic(this.mSoundBank, this.mCueID, this.mFocusValue);
  }

  public override void QuickExecute()
  {
  }

  public string Cue
  {
    get => this.mCue;
    set
    {
      this.mCue = value;
      string[] strArray = this.mCue.Split('/');
      if (strArray != null && strArray.Length > 1)
      {
        this.mSoundBank = (Banks) Enum.Parse(typeof (Banks), strArray[0], true);
        this.mCueID = strArray[1].GetHashCodeCustom();
      }
      else
        this.mCueID = this.mCue.GetHashCodeCustom();
    }
  }

  public float FocusValue
  {
    get => this.mFocusValue ?? 1f;
    set => this.mFocusValue = new float?(value);
  }
}
