// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.Main.SubMenuOptions
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu.Main.Options;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Localization;
using Magicka.Misc;
using Magicka.WebTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu.Main;

internal class SubMenuOptions : SubMenu
{
  private static readonly int LOC_GAME = "#menu_opt_01".GetHashCodeCustom();
  private static readonly int LOC_CONTROLS = "#menu_opt_02".GetHashCodeCustom();
  private static readonly int LOC_SOUND = "#menu_opt_03".GetHashCodeCustom();
  private static readonly int LOC_GRAPHICS = "#menu_opt_04".GetHashCodeCustom();
  private static readonly int LOC_MENU_MAIN_06 = "#menu_main_06".GetHashCodeCustom();
  private static readonly int LOC_PARADOX_ACCOUNT = "#paradox_account".GetHashCodeCustom();
  private static SubMenuOptions mSingelton;
  private static volatile object mSingeltonLock = new object();

  public static SubMenuOptions Instance
  {
    get
    {
      if (SubMenuOptions.mSingelton == null)
      {
        lock (SubMenuOptions.mSingeltonLock)
        {
          if (SubMenuOptions.mSingelton == null)
            SubMenuOptions.mSingelton = new SubMenuOptions();
        }
      }
      return SubMenuOptions.mSingelton;
    }
  }

  public override void LanguageChanged()
  {
    base.LanguageChanged();
    this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenuOptions.LOC_MENU_MAIN_06));
  }

  private SubMenuOptions()
  {
    this.mSelectedPosition = 0;
    this.mMenuItems = new List<MenuItem>();
    this.mMenuTitle = new Text(32 /*0x20*/, FontManager.Instance.GetFont(MagickaFont.MenuTitle), TextAlign.Center, false);
    this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenuOptions.LOC_MENU_MAIN_06));
    this.AddMenuTextItem(SubMenuOptions.LOC_GAME);
    this.AddMenuTextItem(SubMenuOptions.LOC_SOUND);
    this.AddMenuTextItem(SubMenuOptions.LOC_GRAPHICS);
    this.AddMenuTextItem(SubMenuOptions.LOC_CONTROLS);
    this.AddMenuTextItem(SubMenuOptions.LOC_PARADOX_ACCOUNT);
    this.mMenuItems.Add((MenuItem) new MenuImageTextItem(SubMenu.BACK_POSITION, SubMenu.sPagesTexture, SubMenu.BACK_UVOFFSET, SubMenu.BACK_UVSCALE, 0, new Vector2(), SubMenu.BACK_TEXT_ALIGN, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenu.BACK_SIZE));
  }

  public override void ControllerUp(Controller iSender)
  {
    if (!this.mKeyboardSelection)
    {
      this.mSelectedPosition = 0;
      this.mKeyboardSelection = true;
    }
    else
    {
      --this.mSelectedPosition;
      if (this.mSelectedPosition >= 0)
        return;
      this.mSelectedPosition = this.mMenuItems.Count - 1;
    }
  }

  public override void ControllerDown(Controller iSender)
  {
    if (!this.mKeyboardSelection)
    {
      this.mSelectedPosition = 0;
      this.mKeyboardSelection = true;
    }
    else
    {
      ++this.mSelectedPosition;
      if (this.mSelectedPosition < this.mMenuItems.Count)
        return;
      this.mSelectedPosition = 0;
    }
  }

  public override void ControllerA(Controller iSender)
  {
    switch (this.mSelectedPosition)
    {
      case 0:
        Tome.Instance.PushMenu((SubMenu) SubMenuOptionsGame.Instance, 1);
        break;
      case 1:
        Tome.Instance.PushMenu((SubMenu) SubMenuOptionsSound.Instance, 3);
        break;
      case 2:
        Tome.Instance.PushMenu((SubMenu) SubMenuOptionsGraphics.Instance, 4);
        break;
      case 3:
        Tome.Instance.PushMenu((SubMenu) SubMenuOptionsControls.Instance, 1);
        break;
      case 4:
        Tome.Instance.PushMenu((SubMenu) SubMenuOptionsParadoxAccount.Instance, 1);
        break;
      case 5:
        Tome.Instance.PopMenu();
        break;
    }
  }

  public override void Draw(Viewport iLeftSide, Viewport iRightSide)
  {
    base.Draw(iLeftSide, iRightSide);
    this.mEffect.Begin();
    this.mEffect.CurrentTechnique.Passes[0].Begin();
    this.DrawGraphics(SubMenu.sPagesTexture, new Rectangle(448, 976, 608, 48 /*0x30*/), new Rectangle(208 /*0xD0*/, 220, 608, 48 /*0x30*/));
    this.DrawGraphics(SubMenu.sPagesTexture, new Rectangle(448, 768 /*0x0300*/, 496, 208 /*0xD0*/), new Rectangle(264, 620, 496, 208 /*0xD0*/));
    this.mEffect.CurrentTechnique.Passes[0].End();
    this.mEffect.End();
  }

  public override void OnEnter()
  {
    GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Bottom, GamePadMenuHelp.LOC_SELECT);
    GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Right, GamePadMenuHelp.LOC_BACK);
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Left);
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Top);
    this.mMenuItems[4].Enabled = Singleton<ParadoxAccount>.Instance.IsLoggedFull;
  }

  public override void OnExit()
  {
  }

  internal enum MenuItemId
  {
    Game,
    Sound,
    Graphics,
    Controls,
    Paradox,
    Back,
  }
}
