// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.StartDialog
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class StartDialog(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  public static readonly int INTERACTORHASH = "interactor".GetHashCodeCustom();
  private string mDialog;
  private int mID;
  private string mPositionStr = "interactor";
  private int mOwner = StartDialog.INTERACTORHASH;
  private Vector3 mPosition;
  private bool mForceOnScreen;
  private bool mIs3DPos;
  private bool mIs2DPos;
  private Queue<object> mArgs = new Queue<object>(10);

  public override void OnTrigger(Character iArg)
  {
    base.OnTrigger(iArg);
    this.mArgs.Enqueue((object) iArg);
  }

  protected override void Execute()
  {
    Character character = this.mArgs.Count > 0 ? this.mArgs.Dequeue() as Character : (Character) null;
    if (this.mIs2DPos)
      DialogManager.Instance.StartDialog(this.mID, new Vector2(this.mPosition.X, this.mPosition.Y), (Controller) null);
    else if (this.mIs3DPos)
    {
      DialogManager.Instance.StartDialog(this.mID, this.mPosition, (Controller) null);
    }
    else
    {
      Vector3 iWorldPosition = new Vector3();
      if (this.mOwner == StartDialog.INTERACTORHASH)
        iOwner = character;
      else if (!(Entity.GetByID(this.mOwner) is Character iOwner))
      {
        Matrix oLocator;
        this.GameScene.GetLocator(this.mOwner, out oLocator);
        iWorldPosition = oLocator.Translation;
      }
      if (iOwner == null)
        DialogManager.Instance.StartDialog(this.mID, iWorldPosition, (Controller) null);
      else if (iOwner is Avatar)
        DialogManager.Instance.StartDialog(this.mID, (Entity) iOwner, (iOwner as Avatar).Player.Controller);
      else
        DialogManager.Instance.StartDialog(this.mID, (Entity) iOwner, (Controller) null);
    }
  }

  public override void QuickExecute()
  {
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

  public bool ForceOnScreen
  {
    get => this.mForceOnScreen;
    set => this.mForceOnScreen = value;
  }

  public string Position
  {
    get => this.mPositionStr;
    set
    {
      this.mPositionStr = value;
      string[] strArray = value.Split(',');
      if (strArray.Length == 2)
      {
        this.mPosition.X = float.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
        this.mPosition.Y = float.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
        this.mIs2DPos = true;
        this.mIs3DPos = false;
      }
      else if (strArray.Length == 3)
      {
        this.mPosition.X = float.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
        this.mPosition.Y = float.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
        this.mPosition.Y = float.Parse(strArray[2], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
        this.mIs2DPos = false;
        this.mIs3DPos = true;
      }
      else
      {
        this.mOwner = this.mPositionStr.GetHashCodeCustom();
        this.mIs2DPos = false;
        this.mIs3DPos = false;
      }
    }
  }
}
