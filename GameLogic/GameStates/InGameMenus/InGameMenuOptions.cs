// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.InGameMenus.InGameMenuOptions
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using PolygonHead;

#nullable disable
namespace Magicka.GameLogic.GameStates.InGameMenus;

internal class InGameMenuOptions : InGameMenu
{
  private const string OPTION_GAME = "game";
  private const string OPTION_SOUND = "sound";
  private const string OPTION_GRAPHICS = "graphics";
  private const string OPTION_BACK = "back";
  public const float BIG_SEPARATION = 10f;
  private static readonly string[] OPTION_STRINGS = new string[4]
  {
    "game",
    "sound",
    "graphics",
    "back"
  };
  private static InGameMenuOptions sSingelton;
  private static volatile object sSingeltonLock = new object();

  public static InGameMenuOptions Instance
  {
    get
    {
      if (InGameMenuOptions.sSingelton == null)
      {
        lock (InGameMenuOptions.sSingeltonLock)
        {
          if (InGameMenuOptions.sSingelton == null)
            InGameMenuOptions.sSingelton = new InGameMenuOptions();
        }
      }
      return InGameMenuOptions.sSingelton;
    }
  }

  private InGameMenuOptions()
  {
    BitmapFont font = FontManager.Instance.GetFont(MagickaFont.Maiandra18);
    this.AddMenuTextItem("#menu_opt_01".GetHashCodeCustom(), font, TextAlign.Center);
    this.AddMenuTextItem("#menu_opt_03".GetHashCodeCustom(), font, TextAlign.Center);
    this.AddMenuTextItem("#menu_opt_04".GetHashCodeCustom(), font, TextAlign.Center);
    this.AddMenuTextItem("#menu_back".GetHashCodeCustom(), font, TextAlign.Center);
    this.mBackgroundSize = new Vector2(400f, 400f);
  }

  public override void UpdatePositions()
  {
    base.UpdatePositions();
    this.mMenuItems[this.mMenuItems.Count - 1].Position += new Vector2(0.0f, 10f * InGameMenu.sScale);
  }

  protected override void IControllerSelect(Controller iSender)
  {
    switch (this.mSelectedItem)
    {
      case 0:
        AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
        InGameMenu.PushMenu((InGameMenu) InGameMenuOptionsGame.Instance);
        break;
      case 1:
        AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
        InGameMenu.PushMenu((InGameMenu) InGameMenuOptionsSound.Instance);
        break;
      case 2:
        AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
        InGameMenu.PushMenu((InGameMenu) InGameMenuOptionsGraphics.Instance);
        break;
      case 3:
        AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_DECREASE);
        InGameMenu.PopMenu();
        break;
    }
  }

  protected override void IControllerBack(Controller iSender) => InGameMenu.PopMenu();

  protected override string IGetHighlightedButtonName()
  {
    return InGameMenuOptions.OPTION_STRINGS[this.mSelectedItem];
  }

  protected override void OnEnter()
  {
    if (InGameMenu.sController is KeyboardMouseController)
      this.mSelectedItem = -1;
    else
      this.mSelectedItem = 0;
  }

  protected override void OnExit()
  {
  }
}
