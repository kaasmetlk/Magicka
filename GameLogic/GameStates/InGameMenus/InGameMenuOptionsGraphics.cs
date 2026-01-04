// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.InGameMenus.InGameMenuOptionsGraphics
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.Graphics;
using Magicka.Localization;
using Magicka.Storage;
using Microsoft.Xna.Framework;
using PolygonHead;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.GameStates.InGameMenus;

internal class InGameMenuOptionsGraphics : InGameMenu
{
  private const string OPTION_WINDOWED = "windowed";
  private const string OPTION_RESOLUTION = "resolution";
  private const string OPTION_BACK = "back";
  private static readonly string[] OPTION_STRINGS = new string[3]
  {
    "windowed",
    "resolution",
    "back"
  };
  private static InGameMenuOptionsGraphics sSingelton;
  private static volatile object sSingeltonLock = new object();
  private List<MenuTextItem> mOptions;
  private GlobalSettings mGlobalSettings = GlobalSettings.Instance;
  private bool mWindowed;

  public static InGameMenuOptionsGraphics Instance
  {
    get
    {
      if (InGameMenuOptionsGraphics.sSingelton == null)
      {
        lock (InGameMenuOptionsGraphics.sSingeltonLock)
        {
          if (InGameMenuOptionsGraphics.sSingelton == null)
            InGameMenuOptionsGraphics.sSingelton = new InGameMenuOptionsGraphics();
        }
      }
      return InGameMenuOptionsGraphics.sSingelton;
    }
  }

  private InGameMenuOptionsGraphics()
  {
    BitmapFont font = FontManager.Instance.GetFont(MagickaFont.Maiandra18);
    this.mOptions = new List<MenuTextItem>();
    this.mOptions.Add(new MenuTextItem(InGameMenu.GetOnOffLoc(!this.mGlobalSettings.Fullscreen), new Vector2(), font, TextAlign.Left));
    this.AddMenuTextItem("#menu_opt_gfx_01".GetHashCodeCustom(), font, TextAlign.Right);
    this.mOptions.Add(new MenuTextItem("1280 x 720 - 60Hz", new Vector2(), font, TextAlign.Left));
    this.AddMenuTextItem("#menu_opt_gfx_02".GetHashCodeCustom(), font, TextAlign.Right);
    this.AddMenuTextItem("#menu_back".GetHashCodeCustom(), font, TextAlign.Center);
    this.mBackgroundSize = new Vector2(500f, 150f);
  }

  public override void LanguageChanged()
  {
    base.LanguageChanged();
    for (int index = 0; index < this.mOptions.Count; ++index)
      this.mOptions[index].LanguageChanged();
  }

  public override void UpdatePositions()
  {
    Vector2 vector2 = new Vector2();
    vector2.X = InGameMenu.sScreenSize.X * 0.5f;
    vector2.Y = (float) ((double) InGameMenu.sScreenSize.Y * 0.5 - 25.0 * (double) InGameMenu.sScale);
    for (int index = 0; index < this.mMenuItems.Count; ++index)
    {
      MenuItem mMenuItem = this.mMenuItems[index];
      mMenuItem.Scale = InGameMenu.sScale;
      mMenuItem.Position = vector2;
      vector2.Y += mMenuItem.BottomRight.Y - mMenuItem.TopLeft.Y;
    }
    for (int index = 0; index < this.mOptions.Count; ++index)
    {
      Vector2 position = this.mMenuItems[index].Position;
      position.X -= 15f * InGameMenu.sScale;
      this.mMenuItems[index].Position = position;
      position.X += 30f * InGameMenu.sScale;
      this.mOptions[index].Position = position;
      this.mOptions[index].Scale = InGameMenu.sScale;
    }
    this.mMenuItems[this.mMenuItems.Count - 1].Position += new Vector2(0.0f, 10f * InGameMenu.sScale);
    this.mBackgroundSize = new Vector2(500f, 150f);
  }

  protected override void IControllerSelect(Controller iSender)
  {
    switch (this.mSelectedItem)
    {
      case 0:
        AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
        this.mGlobalSettings.Fullscreen = !this.mGlobalSettings.Fullscreen;
        if (this.mGlobalSettings.Fullscreen)
        {
          this.mOptions[0].SetText("#menu_opt_alt_02".GetHashCodeCustom());
          break;
        }
        this.mOptions[0].SetText("#menu_opt_alt_01".GetHashCodeCustom());
        break;
      case 1:
        AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
        InGameMenu.PushMenu((InGameMenu) InGameMenuOptionsResolution.Instance);
        break;
      case 2:
        AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_DECREASE);
        InGameMenu.PopMenu();
        break;
    }
  }

  protected override string IGetHighlightedButtonName()
  {
    return InGameMenuOptionsGraphics.OPTION_STRINGS[this.mSelectedItem];
  }

  protected override void IControllerBack(Controller iSender)
  {
    AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_DECREASE);
    InGameMenu.PopMenu();
  }

  protected override void IControllerMove(Controller iSender, ControllerDirection iDirection)
  {
    base.IControllerMove(iSender, iDirection);
    if (!((iDirection & (ControllerDirection.Right | ControllerDirection.Left)) != ControllerDirection.Center & this.mSelectedItem == 0))
      return;
    AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
    this.mGlobalSettings.Fullscreen = !this.mGlobalSettings.Fullscreen;
    if (this.mGlobalSettings.Fullscreen)
      this.mOptions[0].SetText(LanguageManager.Instance.GetString("#menu_opt_alt_02".GetHashCodeCustom()));
    else
      this.mOptions[0].SetText(LanguageManager.Instance.GetString("#menu_opt_alt_01".GetHashCodeCustom()));
  }

  protected override void OnEnter()
  {
    if (InGameMenu.sController is KeyboardMouseController)
      this.mSelectedItem = -1;
    else
      this.mSelectedItem = 0;
    ResolutionData resolution = GlobalSettings.Instance.Resolution;
    this.mOptions[1].SetText($"{resolution.Width} x {resolution.Height}");
    this.mWindowed = this.mGlobalSettings.Fullscreen;
  }

  protected override void IDraw(float iDeltaTime, ref Vector2 iBackgroundSize)
  {
    base.IDraw(iDeltaTime, ref iBackgroundSize);
    this.UpdatePositions();
    Vector4 color = this.mMenuItems[0].Color;
    Vector4 colorSelected = this.mMenuItems[0].ColorSelected;
    Vector4 colorDisabled = this.mMenuItems[0].ColorDisabled;
    for (int index = 0; index < this.mOptions.Count; ++index)
    {
      MenuItem mOption = (MenuItem) this.mOptions[index];
      mOption.Color = color;
      mOption.ColorSelected = colorSelected;
      mOption.ColorDisabled = colorDisabled;
      mOption.Selected = this.mMenuItems[index].Selected;
      mOption.Enabled = this.mMenuItems[index].Enabled;
      mOption.Draw(InGameMenu.sEffect);
    }
  }

  protected override void OnExit()
  {
    if (this.mWindowed == this.mGlobalSettings.Fullscreen)
      return;
    SaveManager.Instance.SaveSettings();
  }
}
