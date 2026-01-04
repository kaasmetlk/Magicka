// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Conditions.DialogDone
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using Magicka.GameLogic.UI;

#nullable disable
namespace Magicka.Levels.Triggers.Conditions;

public class DialogDone(GameScene iScene) : Condition(iScene)
{
  private string mDialog;
  private int mID;
  private int mInteractIndex = -1;

  protected override bool InternalMet(Character iSender)
  {
    return DialogManager.Instance.IsDialogDone(this.mID, this.mInteractIndex);
  }

  public string Dialog
  {
    get => this.mDialog;
    set
    {
      this.mDialog = value;
      this.mID = this.mDialog.GetHashCodeCustom();
    }
  }

  public int InteractIndex
  {
    get => this.mInteractIndex;
    set => this.mInteractIndex = value;
  }
}
