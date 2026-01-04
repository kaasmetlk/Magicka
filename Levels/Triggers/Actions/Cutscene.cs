// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.Cutscene
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System.Xml;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class Cutscene(Trigger iTrigger, GameScene iScene, XmlNode iNode) : Action(iTrigger, iScene)
{
  private string mOnSkip;
  private int mOnSkipID;
  private bool mSkipBarMove;
  private bool mKillDialogs = true;

  protected override void Execute()
  {
    this.mScene.PlayState.BeginCutscene(this.mOnSkipID, this.mSkipBarMove, this.mKillDialogs);
  }

  public override void QuickExecute()
  {
    this.mScene.PlayState.BeginCutscene(this.mOnSkipID, true, this.mKillDialogs);
  }

  public bool SkipBarMove
  {
    get => this.mSkipBarMove;
    set => this.mSkipBarMove = value;
  }

  public bool KillDialogs
  {
    get => this.mKillDialogs;
    set => this.mKillDialogs = value;
  }

  public string OnSkip
  {
    get => this.mOnSkip;
    set
    {
      this.mOnSkip = value;
      this.mOnSkipID = value.GetHashCodeCustom();
    }
  }
}
