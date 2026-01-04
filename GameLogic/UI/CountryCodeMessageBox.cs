// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.CountryCodeMessageBox
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.UI.Popup;
using Magicka.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.UI;

internal class CountryCodeMessageBox : MessageBox
{
  private const float MARGIN_SIDE = 75f;
  private const float MARGIN_BOTTOM = 70f;
  private const int VISIBLE_ITEMS = 21;
  private const float DEFAULT_SCALE_BACKGROUND = 1.75f;
  private const float DEFAULT_SCALE_SCROLLBAR = 0.65f;
  private const float DELTA_MULTIPLIER = 4f;
  private static readonly int LOC_COUNTRYCODE = "#menu_opt_06".GetHashCodeCustom();
  private static CountryCodeMessageBox sSingleton;
  private static volatile object sSingletonLock = new object();
  private MenuScrollBar mScrollBar;
  private List<MenuTextItem> mItems;
  private MenuTextItem mBackItem;
  private int mSelectedItem;
  private Dictionary<string, string> mCountries;
  private float mScale = 1f;

  public static CountryCodeMessageBox Instance
  {
    get
    {
      if (CountryCodeMessageBox.sSingleton == null)
      {
        lock (CountryCodeMessageBox.sSingletonLock)
        {
          if (CountryCodeMessageBox.sSingleton == null)
            CountryCodeMessageBox.sSingleton = new CountryCodeMessageBox();
        }
      }
      return CountryCodeMessageBox.sSingleton;
    }
  }

  private event Action<string, string> Complete;

  private CountryCodeMessageBox()
    : base(CountryCodeMessageBox.LOC_COUNTRYCODE)
  {
    this.mItems = new List<MenuTextItem>();
    this.mCountries = new Dictionary<string, string>();
    Country[] list = Country.List;
    List<string> stringList = new List<string>();
    for (int index = 0; index < list.Length; ++index)
    {
      this.mCountries.Add(list[index].Name, list[index].TwoLetterCode);
      this.mItems.Add(new MenuTextItem(list[index].Name, Vector2.Zero, this.mFont, TextAlign.Center));
    }
    CountryCodeMessageBox countryCodeMessageBox = this;
    countryCodeMessageBox.mSize = countryCodeMessageBox.mSize * 1.75f;
    this.mScrollBar = new MenuScrollBar(Vector2.Zero, (float) (this.mFont.LineHeight * 21), this.mItems.Count - 21);
    this.mScrollBar.Scale = 0.65f;
    this.mBackItem = new MenuTextItem(SubMenu.LOC_BACK, Vector2.Zero, this.mFont, TextAlign.Center, true);
    this.mBackItem.ColorDisabled = Defines.MESSAGEBOX_COLOR_DEFAULT * 0.5f;
    this.mBackItem.Color = Defines.MESSAGEBOX_COLOR_DEFAULT;
    this.mBackItem.ColorSelected = Vector4.One;
    ResolutionMessageBox.Instance.Complete += new Action<ResolutionData>(this.ResolutionChanged);
    this.ResolutionChanged(GlobalSettings.Instance.Resolution);
  }

  public override void LanguageChanged()
  {
    base.LanguageChanged();
    this.mBackItem.LanguageChanged();
  }

  private void ResolutionChanged(ResolutionData iData)
  {
    this.mScale = (float) iData.Height / PopupSystem.REFERENCE_SIZE.Y;
    Vector2 vector2_1 = this.mSize * this.mScale;
    Vector2 vector2_2 = Vector2.Zero;
    this.mCenter = new Vector2((float) iData.Width * 0.5f, (float) iData.Height * 0.5f);
    vector2_2 = new Vector2((float) ((double) this.mCenter.X + (double) vector2_1.X * 0.5 - 75.0 * (double) this.mScale), this.mCenter.Y);
    this.mScrollBar.Position = vector2_2;
    this.mScrollBar.Height = (float) (this.mFont.LineHeight * 21);
    vector2_2 = new Vector2(this.mCenter.X, (float) ((double) this.mCenter.Y + (double) vector2_1.Y * 0.5 - 70.0 * (double) this.mScale));
    this.mBackItem.Position = vector2_2;
    foreach (MenuItem mItem in this.mItems)
      mItem.Scale = this.mScale;
  }

  public void Show(string iDefault, Action<string, string> iCallback)
  {
    this.Show();
    for (int index = 0; index < this.mItems.Count; ++index)
    {
      if (this.mItems[index].Name == iDefault)
      {
        this.mSelectedItem = index;
        break;
      }
    }
    int num = this.mSelectedItem - 10;
    this.mScrollBar.Value = num < 0 ? 0 : (num >= this.mScrollBar.MaxValue ? this.mScrollBar.MaxValue : num);
    this.Complete = iCallback;
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
          this.mSelectedItem = this.mItems.Count - 1;
          this.mScrollBar.Value = this.mScrollBar.MaxValue;
          break;
        }
        --this.mSelectedItem;
        if (this.mSelectedItem < 0)
        {
          this.mSelectedItem = -1;
          this.mBackItem.Selected = true;
          break;
        }
        while (this.mSelectedItem < this.mScrollBar.Value)
          --this.mScrollBar.Value;
        break;
      case ControllerDirection.Down:
        if (this.mBackItem.Selected)
        {
          this.mBackItem.Selected = false;
          this.mSelectedItem = 0;
          this.mScrollBar.Value = 0;
          break;
        }
        ++this.mSelectedItem;
        if (this.mSelectedItem >= this.mItems.Count)
        {
          this.mSelectedItem = -1;
          this.mBackItem.Selected = true;
          break;
        }
        while (this.mSelectedItem > this.mScrollBar.Value + 21)
          ++this.mScrollBar.Value;
        break;
    }
  }

  public override void OnSelect(Controller iSender)
  {
    if (this.Complete != null && this.mSelectedItem >= 0 & this.mSelectedItem < this.mItems.Count)
    {
      string name = this.mItems[this.mSelectedItem].Name;
      string mCountry = this.mCountries[name];
      this.Complete(name, mCountry);
    }
    this.Kill();
  }

  public override void OnMouseMove(MouseState iNewState, MouseState iOldState)
  {
    this.mBackItem.Selected = false;
    this.mSelectedItem = -1;
    if (this.mScrollBar.Grabbed)
    {
      this.mScrollBar.ScrollTo((float) iNewState.Y);
    }
    else
    {
      for (int index = this.mScrollBar.Value; index < Math.Min(this.mScrollBar.Value + 21, this.mItems.Count); ++index)
      {
        MenuItem mItem = (MenuItem) this.mItems[index];
        if (mItem.Enabled & mItem.InsideBounds(iNewState))
        {
          this.mSelectedItem = index;
          break;
        }
      }
      if (this.mSelectedItem == -1 && this.mBackItem.Enabled && this.mBackItem.InsideBounds((float) iNewState.X, (float) iNewState.Y))
        this.mBackItem.Selected = true;
    }
    if (iNewState.LeftButton != ButtonState.Released)
      return;
    this.mScrollBar.Grabbed = false;
  }

  public override void OnMouseClick(MouseState iNewState, MouseState iOldState)
  {
    if (iNewState.LeftButton == ButtonState.Released && iOldState.LeftButton == ButtonState.Pressed && this.mBackItem.Enabled && this.mBackItem.InsideBounds(iNewState))
      this.Kill();
    else if (iNewState.ScrollWheelValue > iOldState.ScrollWheelValue)
      --this.mScrollBar.Value;
    else if (iNewState.ScrollWheelValue < iOldState.ScrollWheelValue)
      ++this.mScrollBar.Value;
    else if (this.mScrollBar.InsideBounds(iNewState))
    {
      if (this.mScrollBar.InsideDragBounds(iNewState))
      {
        this.mScrollBar.Grabbed = true;
      }
      else
      {
        if (iNewState.LeftButton != ButtonState.Released || iOldState.LeftButton != ButtonState.Pressed)
          return;
        this.mScrollBar.ScrollTo((float) iNewState.Y);
      }
    }
    else
    {
      for (int index = this.mScrollBar.Value; index < Math.Min(this.mScrollBar.Value + 21, this.mItems.Count); ++index)
      {
        MenuItem mItem = (MenuItem) this.mItems[index];
        if (mItem.Enabled & mItem.InsideBounds(iNewState))
        {
          this.OnSelect((Controller) ControlManager.Instance.MenuController);
          break;
        }
      }
    }
  }

  public override void Draw(float iDeltaTime)
  {
    if (DaisyWheel.IsDisplaying)
      return;
    MessageBox.sGUIBasicEffect.SetScreenSize(RenderManager.Instance.ScreenSize.X, RenderManager.Instance.ScreenSize.Y);
    if (this.mDead)
      this.mAlpha = Math.Max(this.mAlpha - iDeltaTime * 4f, 0.0f);
    else
      this.mAlpha = Math.Min(this.mAlpha + iDeltaTime * 4f, 1f);
    MessageBox.sGUIBasicEffect.Transform = new Matrix()
    {
      M11 = 1.75f * this.mScale,
      M22 = 1.75f * this.mScale,
      M33 = 1f,
      M44 = 1f,
      M41 = this.mCenter.X,
      M42 = this.mCenter.Y
    };
    Vector4 vector4 = new Vector4();
    vector4.X = vector4.Y = vector4.Z = 1f;
    vector4.W = this.mAlpha;
    MessageBox.sGUIBasicEffect.Color = vector4;
    MessageBox.sGUIBasicEffect.VertexColorEnabled = false;
    MessageBox.sGUIBasicEffect.Texture = (Texture) MessageBox.sTexture;
    MessageBox.sGUIBasicEffect.TextureEnabled = true;
    MessageBox.sGUIBasicEffect.GraphicsDevice.Vertices[0].SetSource(MessageBox.sVertexBuffer, 0, 16 /*0x10*/);
    MessageBox.sGUIBasicEffect.GraphicsDevice.VertexDeclaration = MessageBox.sVertexDeclaration;
    MessageBox.sGUIBasicEffect.Begin();
    MessageBox.sGUIBasicEffect.CurrentTechnique.Passes[0].Begin();
    MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
    float num = (float) this.mFont.LineHeight * this.mScale;
    Vector2 vector2 = new Vector2(this.mCenter.X, this.mCenter.Y - (float) ((double) num * 21.0 * 0.5));
    vector2.Y += num * 0.5f;
    for (int index = this.mScrollBar.Value; index < Math.Min(this.mScrollBar.Value + 21, this.mItems.Count); ++index)
    {
      MenuItem mItem = (MenuItem) this.mItems[index];
      mItem.Selected = this.mSelectedItem == index;
      Vector4 messageboxColorDefault = Defines.MESSAGEBOX_COLOR_DEFAULT with
      {
        W = this.mAlpha
      };
      mItem.Color = messageboxColorDefault;
      mItem.Position = vector2;
      mItem.Draw(MessageBox.sGUIBasicEffect, this.mScale * 0.9f);
      vector2.Y += num;
    }
    if (this.mScrollBar.MaxValue > 0)
    {
      this.mScrollBar.Color = new Vector4(1f, 1f, 1f, this.mAlpha);
      this.mScrollBar.Draw(MessageBox.sGUIBasicEffect);
    }
    this.mBackItem.Alpha = this.mAlpha;
    this.mBackItem.Draw(MessageBox.sGUIBasicEffect, this.mScale);
    MessageBox.sGUIBasicEffect.CurrentTechnique.Passes[0].End();
    MessageBox.sGUIBasicEffect.End();
  }
}
