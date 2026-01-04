// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.InGameMenus.InGameMenuAreYouSure
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.Graphics;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using PolygonHead;
using System;

#nullable disable
namespace Magicka.GameLogic.GameStates.InGameMenus;

internal class InGameMenuAreYouSure : InGameMenu
{
  private const string OPTION_YES = "yes";
  private const string OPTION_NO = "no";
  private int[] mTexts;
  private Action mYesCallback;
  private BitmapFont mFont;
  private Text mRUSureText;
  private float mTextHeight;

  public override void LanguageChanged()
  {
    base.LanguageChanged();
    LanguageManager instance = LanguageManager.Instance;
    string iText = "";
    for (int index = 0; index < this.mTexts.Length; ++index)
      iText = iText + this.mFont.Wrap(instance.GetString(this.mTexts[index]), 300, true) + (object) '\n';
    this.mRUSureText.SetText(iText);
  }

  public InGameMenuAreYouSure(Action iYesCallback, params int[] iText)
  {
    this.mTexts = iText;
    this.mYesCallback = iYesCallback;
    this.mFont = FontManager.Instance.GetFont(MagickaFont.Maiandra18);
    this.AddMenuTextItem("#add_menu_yes".GetHashCodeCustom(), this.mFont, TextAlign.Center);
    this.AddMenuTextItem("#add_menu_no".GetHashCodeCustom(), this.mFont, TextAlign.Center);
    this.mRUSureText = new Text(512 /*0x0200*/, this.mFont, TextAlign.Center, false);
    LanguageManager instance = LanguageManager.Instance;
    string iText1 = "";
    for (int index = 0; index < this.mTexts.Length; ++index)
      iText1 = iText1 + this.mFont.Wrap(instance.GetString(this.mTexts[index]), 300, true) + (object) '\n';
    this.mRUSureText.SetText(iText1);
    this.mTextHeight = this.mFont.MeasureText(this.mRUSureText.Characters, true).Y;
    this.mBackgroundSize = new Vector2(400f, 300f);
  }

  public override void UpdatePositions()
  {
    Vector2 vector2 = new Vector2();
    vector2.X = InGameMenu.sScreenSize.X * 0.5f;
    vector2.Y = (float) ((double) InGameMenu.sScreenSize.Y * 0.5 + 40.0 * (double) InGameMenu.sScale);
    for (int index = 0; index < this.mMenuItems.Count; ++index)
    {
      MenuItem mMenuItem = this.mMenuItems[index];
      mMenuItem.Scale = InGameMenu.sScale;
      mMenuItem.Position = vector2;
      vector2.Y += mMenuItem.BottomRight.Y - mMenuItem.TopLeft.Y;
    }
  }

  protected override string IGetHighlightedButtonName() => this.mSelectedItem != 0 ? "no" : "yes";

  protected override void IControllerSelect(Controller iSender)
  {
    if (this.mSelectedItem == 0)
    {
      AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
      this.mYesCallback();
    }
    else
    {
      if (this.mSelectedItem <= 0)
        return;
      AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_DECREASE);
      InGameMenu.PopMenu();
    }
  }

  protected override void IControllerBack(Controller iSender) => InGameMenu.PopMenu();

  protected override void OnEnter()
  {
    if (InGameMenu.sController is KeyboardMouseController)
      this.mSelectedItem = -1;
    else
      this.mSelectedItem = 1;
  }

  protected override void IDraw(float iDeltaTime, ref Vector2 iBackgroundSize)
  {
    base.IDraw(iDeltaTime, ref iBackgroundSize);
    InGameMenu.sEffect.Color = this.mMenuItems[0].Color;
    this.mRUSureText.Draw(InGameMenu.sEffect, InGameMenu.sScreenSize.X * 0.5f, (float) ((double) InGameMenu.sScreenSize.Y * 0.5 + 30.0 * (double) InGameMenu.sScale - (double) this.mTextHeight * (double) InGameMenu.sScale), InGameMenu.sScale);
  }

  protected override void OnExit()
  {
  }
}
