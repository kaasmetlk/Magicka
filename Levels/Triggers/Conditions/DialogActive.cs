// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Conditions.DialogActive
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using Magicka.GameLogic.UI;
using System;

#nullable disable
namespace Magicka.Levels.Triggers.Conditions;

internal class DialogActive(GameScene iScene) : Condition(iScene)
{
  private string mDialog;
  private int mID;

  protected override bool InternalMet(Character iSender)
  {
    return string.IsNullOrEmpty(this.mDialog) || this.mDialog.Equals("any", StringComparison.InvariantCultureIgnoreCase) ? DialogManager.Instance.MessageBoxActive : DialogManager.Instance.DialogActive(this.mID);
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
}
