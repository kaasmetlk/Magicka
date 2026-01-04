// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.LanguageMessageBox
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.Graphics;
using Magicka.Localization;
using Magicka.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.UI;

internal class LanguageMessageBox : MessageBox
{
  private const float BACK_PADDING = 64f;
  private const int VISIBLE_LANGUAGES = 8;
  private static readonly int LOC_LANGUAGES = "#menu_opt_06".GetHashCodeCustom();
  private static LanguageMessageBox sSingelton;
  private static volatile object sSingeltonLock = new object();
  private MenuScrollBar mLanguageScrollBar;
  private List<MenuTextItem> mLanguages;
  private MenuTextItem mBackItem;
  private int mCurrentLanguage;
  private new BitmapFont mFont;

  public static LanguageMessageBox Instance
  {
    get
    {
      if (LanguageMessageBox.sSingelton == null)
      {
        lock (LanguageMessageBox.sSingeltonLock)
        {
          if (LanguageMessageBox.sSingelton == null)
            LanguageMessageBox.sSingelton = new LanguageMessageBox();
        }
      }
      return LanguageMessageBox.sSingelton;
    }
  }

  public event Action<Language> Complete;

  private LanguageMessageBox()
    : base(LanguageMessageBox.LOC_LANGUAGES)
  {
    this.mFont = Magicka.Game.Instance.Content.Load<BitmapFont>("UI/Font/LanguageFont");
    float lineHeight = (float) this.mFont.LineHeight;
    this.mLanguages = new List<MenuTextItem>();
    LanguageManager instance = LanguageManager.Instance;
    for (int index = 0; index < instance.AllLanguages.Length; ++index)
      this.mLanguages.Add(new MenuTextItem(instance.GetNativeName(instance.AllLanguages[index]), Vector2.Zero, this.mFont, TextAlign.Center));
    Vector2 iPosition = new Vector2((float) ((double) this.mCenter.X + (double) this.mSize.X * 0.5 - 64.0), this.mCenter.Y);
    this.mLanguageScrollBar = new MenuScrollBar(iPosition, (float) ((double) lineHeight * 8.0 * 2.0), this.mLanguages.Count - 8);
    this.mLanguageScrollBar.Scale = 0.5f;
    BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
    iPosition = new Vector2(this.mCenter.X, (float) ((double) this.mCenter.Y + (double) this.mSize.Y * 0.5 - 64.0));
    this.mBackItem = new MenuTextItem(SubMenu.LOC_CANCEL, iPosition, font, TextAlign.Center, true);
    this.mBackItem.ColorDisabled = Defines.MESSAGEBOX_COLOR_DEFAULT * 0.5f;
    this.mBackItem.Color = Defines.MESSAGEBOX_COLOR_DEFAULT;
    this.mBackItem.ColorSelected = Vector4.One;
  }

  public override void LanguageChanged()
  {
    base.LanguageChanged();
    this.mBackItem.LanguageChanged();
  }

  public override void Show()
  {
    base.Show();
    this.mLanguageScrollBar.Position = new Vector2((float) ((double) this.mCenter.X + (double) this.mSize.X * 0.5 - 64.0), this.mCenter.Y);
    this.mBackItem.Position = new Vector2(this.mCenter.X, (float) ((double) this.mCenter.Y + (double) this.mSize.Y * 0.5 - 64.0));
  }

  public override void OnTextInput(char iChar)
  {
  }

  public override void OnMove(Controller iSender, ControllerDirection iDirection)
  {
    switch (iDirection)
    {
      case ControllerDirection.Up:
        if (this.mBackItem.Selected)
        {
          this.mBackItem.Selected = false;
          this.mCurrentLanguage = this.mLanguages.Count - 1;
          this.mLanguageScrollBar.Value = this.mLanguageScrollBar.MaxValue;
          break;
        }
        --this.mCurrentLanguage;
        if (this.mCurrentLanguage < 0)
        {
          this.mCurrentLanguage = -1;
          this.mBackItem.Selected = true;
          break;
        }
        while (this.mCurrentLanguage < this.mLanguageScrollBar.Value)
          --this.mLanguageScrollBar.Value;
        break;
      case ControllerDirection.Down:
        if (this.mBackItem.Selected)
        {
          this.mBackItem.Selected = false;
          this.mCurrentLanguage = 0;
          this.mLanguageScrollBar.Value = 0;
          break;
        }
        ++this.mCurrentLanguage;
        if (this.mCurrentLanguage >= this.mLanguages.Count)
        {
          this.mCurrentLanguage = -1;
          this.mBackItem.Selected = true;
          break;
        }
        while (this.mCurrentLanguage > this.mLanguageScrollBar.Value + 8)
          ++this.mLanguageScrollBar.Value;
        break;
    }
  }

  public override void OnSelect(Controller iSender)
  {
    if (this.Complete != null & this.mCurrentLanguage >= 0 & this.mCurrentLanguage < this.mLanguages.Count)
      this.Complete(LanguageManager.Instance.AllLanguages[this.mCurrentLanguage]);
    this.Kill();
  }

  public override void Kill()
  {
    base.Kill();
    SaveManager.Instance.SaveSettings();
  }

  public override void OnMouseMove(MouseState iNewState, MouseState iOldState)
  {
    if (this.mBackItem.Enabled && this.mBackItem.InsideBounds((float) iNewState.X, (float) iNewState.Y))
      this.mBackItem.Selected = true;
    else if ((double) iNewState.X > (double) this.mCenter.X - (double) this.mSize.X * 0.5 && (double) iNewState.X < (double) this.mCenter.X + (double) this.mSize.X * 0.5 && (double) iNewState.Y > (double) this.mCenter.Y - (double) this.mSize.Y && (double) iNewState.Y < (double) this.mCenter.Y + (double) this.mSize.Y)
    {
      this.mBackItem.Selected = false;
      this.mCurrentLanguage = -1;
      if (this.mLanguageScrollBar.Grabbed)
      {
        this.mLanguageScrollBar.ScrollTo((float) iNewState.Y);
      }
      else
      {
        for (int index = this.mLanguageScrollBar.Value; index < Math.Min(this.mLanguageScrollBar.Value + 8, this.mLanguages.Count); ++index)
        {
          MenuItem mLanguage = (MenuItem) this.mLanguages[index];
          if (mLanguage.Enabled & mLanguage.InsideBounds(iNewState))
          {
            this.mCurrentLanguage = index;
            break;
          }
        }
      }
    }
    if (iNewState.LeftButton != ButtonState.Released)
      return;
    this.mLanguageScrollBar.Grabbed = false;
  }

  public override void OnMouseClick(MouseState iNewState, MouseState iOldState)
  {
    if (this.mBackItem.Enabled && this.mBackItem.InsideBounds((float) iNewState.X, (float) iNewState.Y))
      this.Kill();
    else if (iNewState.ScrollWheelValue > iOldState.ScrollWheelValue)
      --this.mLanguageScrollBar.Value;
    else if (iNewState.ScrollWheelValue < iOldState.ScrollWheelValue)
      ++this.mLanguageScrollBar.Value;
    else if (this.mLanguageScrollBar.InsideBounds(iNewState))
    {
      if (this.mLanguageScrollBar.InsideDragBounds(iNewState))
      {
        this.mLanguageScrollBar.Grabbed = true;
      }
      else
      {
        if (iNewState.LeftButton != ButtonState.Released || iOldState.LeftButton != ButtonState.Pressed)
          return;
        this.mLanguageScrollBar.ScrollTo((float) iNewState.Y);
      }
    }
    else
    {
      for (int index = this.mLanguageScrollBar.Value; index < Math.Min(this.mLanguageScrollBar.Value + 8, this.mLanguages.Count); ++index)
      {
        MenuItem mLanguage = (MenuItem) this.mLanguages[index];
        if (mLanguage.Enabled & mLanguage.InsideBounds(iNewState))
        {
          this.OnSelect((Controller) ControlManager.Instance.MenuController);
          break;
        }
      }
    }
  }

  public override void Draw(float iDeltaTime)
  {
    base.Draw(iDeltaTime);
    float lineHeight = (float) this.mFont.LineHeight;
    Vector2 vector2 = new Vector2(this.mCenter.X, this.mCenter.Y - (float) ((double) lineHeight * 8.0 * 0.5));
    vector2.Y += lineHeight * 0.5f;
    for (int index = this.mLanguageScrollBar.Value; index < Math.Min(this.mLanguageScrollBar.Value + 8, this.mLanguages.Count); ++index)
    {
      MenuItem mLanguage = (MenuItem) this.mLanguages[index];
      mLanguage.Color = new Vector4(Defines.MESSAGEBOX_COLOR_DEFAULT.X, Defines.MESSAGEBOX_COLOR_DEFAULT.Y, Defines.MESSAGEBOX_COLOR_DEFAULT.Z, this.mAlpha);
      mLanguage.Position = vector2;
      mLanguage.Draw(MessageBox.sGUIBasicEffect);
      vector2.Y += lineHeight;
      mLanguage.Selected = this.mCurrentLanguage == index;
    }
    if (this.mLanguageScrollBar.MaxValue > 0)
    {
      this.mLanguageScrollBar.Color = new Vector4(1f, 1f, 1f, this.mAlpha);
      this.mLanguageScrollBar.Draw(MessageBox.sGUIBasicEffect);
    }
    this.mBackItem.Alpha = this.mAlpha;
    this.mBackItem.Position = new Vector2(this.mCenter.X, (float) ((double) this.mCenter.Y + (double) this.mSize.Y * 0.5 - 64.0));
    this.mBackItem.Draw(MessageBox.sGUIBasicEffect);
    MessageBox.sGUIBasicEffect.CurrentTechnique.Passes[0].End();
    MessageBox.sGUIBasicEffect.End();
  }
}
