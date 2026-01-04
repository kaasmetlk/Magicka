// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.SetDialog
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class SetDialog(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private string mDialog;
  private string mID;
  private int mDialogHash;
  private int mIDHash;

  protected override void Execute()
  {
    if (!(Entity.GetByID(this.mIDHash) is Character byId))
      return;
    byId.Dialog = this.mDialog.GetHashCodeCustom();
  }

  public override void QuickExecute() => this.Execute();

  public string Dialog
  {
    get => this.mDialog;
    set
    {
      this.mDialog = value;
      this.mDialogHash = this.mDialog.GetHashCodeCustom();
    }
  }

  public string ID
  {
    get => this.mID;
    set
    {
      this.mID = value;
      this.mIDHash = this.mID.GetHashCodeCustom();
    }
  }
}
