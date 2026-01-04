// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.PlaySound
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using System;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class PlaySound(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private string mIDStr;
  private int mID;
  private Banks mBank;
  private string mCue;
  private int mCueID;
  private string mArea;
  private int mAreaID;
  private float mVolume;
  private float mRadius;
  private bool mApply3D;
  private static readonly string VOLUME_VAR_NAME = nameof (Volume);

  protected override void Execute()
  {
    if (!string.IsNullOrEmpty(this.mIDStr))
    {
      if (string.IsNullOrEmpty(this.mArea))
        this.GameScene.PlayAmbientSound(this.mID, this.mBank, this.mCueID, this.mVolume);
      else
        this.GameScene.PlayAmbientSound(this.mID, this.mBank, this.mCueID, this.mVolume, this.mAreaID, this.mRadius, this.mApply3D);
    }
    else if (string.IsNullOrEmpty(this.mArea))
    {
      Microsoft.Xna.Framework.Audio.Cue cue = AudioManager.Instance.GetCue(this.mBank, this.mCueID);
      cue.SetVariable(PlaySound.VOLUME_VAR_NAME, this.mVolume);
      cue.Play();
    }
    else
    {
      AudioLocator audioLocator = new AudioLocator(0, this.mBank, this.mCueID, this.mVolume, this.mAreaID, this.mRadius, this.mApply3D);
      audioLocator.Play();
      audioLocator.Update(this.mScene);
    }
  }

  public override void QuickExecute()
  {
  }

  public string ID
  {
    get => this.mIDStr;
    set
    {
      this.mIDStr = value;
      this.mID = this.mIDStr.GetHashCodeCustom();
    }
  }

  public string Cue
  {
    get => this.mCue;
    set
    {
      string[] strArray = value.Split('/');
      if (strArray.Length == 1)
      {
        this.mCue = strArray[0];
        this.mBank = Banks.Ambience;
      }
      else
      {
        this.mCue = strArray.Length == 2 ? strArray[1] : throw new Exception("Invalid Syntax!");
        this.mBank = (Banks) Enum.Parse(typeof (Banks), strArray[0], true);
      }
      this.mCueID = this.mCue.GetHashCodeCustom();
    }
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

  public float Volume
  {
    get => this.mVolume;
    set => this.mVolume = value;
  }

  public float Radius
  {
    get => this.mRadius;
    set => this.mRadius = value;
  }

  public bool Apply3D
  {
    get => this.mApply3D;
    set => this.mApply3D = value;
  }
}
