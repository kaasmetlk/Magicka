// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.Popup.MenuMessagePopup
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.Graphics;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using PolygonHead.Effects;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.UI.Popup;

public class MenuMessagePopup : MenuImagePopup
{
  public const float DEFAULT_BTN_SIZE = 200f;
  private const float BUTTON_PADDING = 20f;
  private const float LOADING_IMAGE_PADDING = 100f;
  private const float LOADING_IMAGE_FADE_TIME = 1.5f;
  private const int MAX_TEXT_LENGTH = 256 /*0x0100*/;
  private const string TAG_SPRITESHEET_NAME = "UI/Menu/tag_spritesheet";
  public static readonly Vector2 DEFAULT_UV = new Vector2(517f, 4f);
  public static readonly Vector2 DEFAULT_SCALE = new Vector2(499f, 357f);
  public static readonly Vector4 DEFAULT_MARGINS = new Vector4(25f, 25f, 80f, 100f);
  public static readonly Vector2 DEFAULT_SIZE = MenuMessagePopup.DEFAULT_SCALE * 1.6f;
  private static readonly Vector2 LOADING_IMAGE_UV = new Vector2(889f, 406f);
  private static readonly Vector2 LOADING_IMAGE_SCALE = new Vector2(120f, 94f);
  private static readonly Vector2 LOADING_IMAGE_SIZE = new Vector2(120f, 94f);
  protected static readonly int LOC_BTN_OK = "#add_menu_ok".GetHashCodeCustom();
  protected static readonly int LOC_BTN_BACK = "#menu_back".GetHashCodeCustom();
  protected static readonly int LOC_BTN_YES = "#add_menu_yes".GetHashCodeCustom();
  protected static readonly int LOC_BTN_NO = "#add_menu_no".GetHashCodeCustom();
  protected static BitmapFont sDefaultTextFont;
  protected static BitmapFont sDefaultTitleFont = FontManager.Instance.GetFont(MagickaFont.MenuOption);
  protected static BitmapFont sDefaultButtonFont;
  protected static Texture2D sTagTexture;
  private int mTitleLocId;
  private Text mTitleText;
  private int mTitleLineCount = 1;
  private int mMessageLocId;
  private Text mMessageText;
  private int mMessageLineCount = 1;
  private int mExtraLocId;
  private Text mExtraText;
  private int mExtraLineCount = 1;
  private Vector4 mMargins;
  private int mLineHeight;
  private float mMinButtonWidth;
  private ButtonConfig mButtonConfig;
  private List<MenuTextButtonItem> mButtons;
  private MenuMessagePopup.LoadingImageState mLoadingImageState;
  private bool mClearOnHide = true;

  public bool ClearOnHide
  {
    set => this.mClearOnHide = value;
  }

  static MenuMessagePopup()
  {
    MenuMessagePopup.sDefaultTextFont = FontManager.Instance.GetFont(MagickaFont.MenuOption);
    MenuMessagePopup.sDefaultButtonFont = FontManager.Instance.GetFont(MagickaFont.MenuOption);
    MenuMessagePopup.sTagTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/tag_spritesheet");
  }

  public MenuMessagePopup()
    : this(MenuMessagePopup.sTagTexture, MenuMessagePopup.DEFAULT_UV, MenuMessagePopup.DEFAULT_SCALE, MenuMessagePopup.DEFAULT_MARGINS, 200f)
  {
    this.mSize = MenuMessagePopup.DEFAULT_SIZE;
  }

  public MenuMessagePopup(
    Texture2D iTexture,
    Vector2 iTextureUV,
    Vector2 iTextureSize,
    Vector4 iMargins,
    float iButtonMinWidth)
    : base(iTexture, iTextureUV, iTextureSize)
  {
    this.mButtons = new List<MenuTextButtonItem>();
    this.mLineHeight = MenuMessagePopup.sDefaultTextFont.LineHeight;
    this.mMargins = iMargins;
    this.mMinButtonWidth = iButtonMinWidth;
    this.mTitleText = new Text(256 /*0x0100*/, MenuMessagePopup.sDefaultTextFont, TextAlign.Center, true);
    this.mTitleText.SetText(string.Empty);
    this.mMessageText = new Text(256 /*0x0100*/, MenuMessagePopup.sDefaultTextFont, TextAlign.Center, true);
    this.mMessageText.SetText(string.Empty);
    this.mExtraText = new Text(256 /*0x0100*/, MenuMessagePopup.sDefaultTextFont, TextAlign.Right, true);
    this.mExtraText.SetText(string.Empty);
    this.SetButtonType(ButtonConfig.Ok);
  }

  public override void LanguageChanged()
  {
    this.SetTitle(this.mTitleLocId, new Color(this.mTitleText.DefaultColor));
    this.SetMessage(this.mMessageLocId, new Color(this.mMessageText.DefaultColor));
    this.SetExtra(this.mExtraLocId, new Color(this.mExtraText.DefaultColor));
    foreach (MenuItem mButton in this.mButtons)
      mButton.LanguageChanged();
    this.UpdateBoundingBox();
  }

  public void SetTitle(int iText, Vector4 iColour)
  {
    this.mTitleLocId = iText;
    this.mTitleText.DefaultColor = iColour;
    if (this.mTitleLocId == 0)
    {
      this.mTitleLineCount = 1;
      this.mTitleText.SetText(string.Empty);
    }
    else
    {
      string oText = LanguageManager.Instance.GetString(iText);
      this.mTitleLineCount = this.WrapText(ref oText, MenuMessagePopup.sDefaultTitleFont);
      this.mTitleText.SetText(oText);
    }
  }

  public void SetTitle(int iText, Color iColour) => this.SetTitle(iText, iColour.ToVector4());

  public void SetTitle(string iText, Vector4 iColour)
  {
    this.mTitleLocId = 0;
    this.mTitleText.DefaultColor = iColour;
    string oText = iText;
    this.mTitleLineCount = this.WrapText(ref oText, MenuMessagePopup.sDefaultTitleFont);
    this.mTitleText.SetText(oText);
  }

  public void SetMessage(int iText, Vector4 iColour)
  {
    this.mMessageLocId = iText;
    this.mMessageText.DefaultColor = iColour;
    if (this.mMessageLocId == 0)
    {
      this.mMessageLineCount = 1;
      this.mMessageText.SetText(string.Empty);
    }
    else
    {
      string oText = LanguageManager.Instance.GetString(iText);
      this.mMessageLineCount = this.WrapText(ref oText, MenuMessagePopup.sDefaultTextFont);
      this.mMessageText.SetText(oText);
    }
  }

  public void SetMessage(string iText, Vector4 iColour)
  {
    this.mMessageLocId = 0;
    this.mMessageText.DefaultColor = iColour;
    string oText = iText;
    this.mMessageLineCount = this.WrapText(ref oText, MenuMessagePopup.sDefaultTextFont);
    this.mMessageText.SetText(oText);
  }

  public void SetMessage(int iText, Color iColour) => this.SetMessage(iText, iColour.ToVector4());

  public void SetExtra(int iText, Vector4 iColour)
  {
    this.mExtraLocId = iText;
    this.mExtraText.DefaultColor = iColour;
    if (this.mExtraLocId == 0)
    {
      this.mExtraLineCount = 1;
      this.mExtraText.SetText(string.Empty);
    }
    else
    {
      string oText = LanguageManager.Instance.GetString(iText);
      this.mExtraLineCount = this.WrapText(ref oText, MenuMessagePopup.sDefaultTextFont);
      this.mExtraText.SetText(oText);
    }
  }

  public void SetExtra(string iText, Vector4 iColour)
  {
    this.mExtraLocId = 0;
    this.mExtraText.DefaultColor = iColour;
    string oText = iText;
    this.mExtraLineCount = this.WrapText(ref oText, MenuMessagePopup.sDefaultTextFont);
    this.mExtraText.SetText(oText);
  }

  public void SetExtra(int iText, Color iColour) => this.SetExtra(iText, iColour.ToVector4());

  public void Clear()
  {
    this.SetTitle(0, Color.White);
    this.SetMessage(0, Color.White);
  }

  private int WrapText(ref string oText, BitmapFont iFont)
  {
    int iTargetLineWidth = (int) ((double) this.mSize.X - (double) this.mMargins.X - (double) this.mMargins.Y);
    oText = iFont.Wrap(oText, iTargetLineWidth, true);
    oText = oText.Replace("\\n", "\n");
    return oText.Split('\n').Length;
  }

  private MenuTextButtonItem CreateButton(int iLoc)
  {
    return new MenuTextButtonItem(Vector2.Zero, MenuImagePopup.sPagesTexture, MenuTextButtonItem.DEFAULT_UV_OFFSET, MenuTextButtonItem.DEFAULT_SIZE, iLoc, MenuMessagePopup.sDefaultButtonFont, this.mMinButtonWidth, TextAlign.Center);
  }

  public void SetButtonType(ButtonConfig iType)
  {
    this.mButtons.Clear();
    this.mButtonConfig = iType;
    switch (this.mButtonConfig)
    {
      case ButtonConfig.Ok:
        this.mButtons.Add(this.CreateButton(MenuMessagePopup.LOC_BTN_OK));
        break;
      case ButtonConfig.Back:
        this.mButtons.Add(this.CreateButton(MenuMessagePopup.LOC_BTN_BACK));
        break;
      case ButtonConfig.OkCancel:
        this.mButtons.Add(this.CreateButton(MenuMessagePopup.LOC_BTN_YES));
        this.mButtons.Add(this.CreateButton(MenuMessagePopup.LOC_BTN_NO));
        break;
    }
    this.AlignButtons();
  }

  private void AlignButtons()
  {
    Vector2 vector2 = new Vector2(this.mPosition.X, (float) ((double) this.mPosition.Y + (double) this.mSize.Y * 0.5 * (double) this.mScale - (double) this.mMargins.W * (double) this.mScale));
    if (this.mButtons.Count == 1)
    {
      this.mButtons[0].Position = vector2;
    }
    else
    {
      if (this.mButtons.Count != 2)
        return;
      this.mButtons[0].Position = vector2 - new Vector2((float) (0.5 * ((double) this.mButtons[0].RealWidth + 20.0)), 0.0f);
      this.mButtons[1].Position = vector2 + new Vector2((float) (0.5 * ((double) this.mButtons[1].RealWidth + 20.0)), 0.0f);
    }
  }

  public void EnableLoadingIcon()
  {
    this.mLoadingImageState = new MenuMessagePopup.LoadingImageState(MenuMessagePopup.sTagTexture, MenuMessagePopup.LOADING_IMAGE_UV, MenuMessagePopup.LOADING_IMAGE_SCALE, 1.5f);
  }

  public override void OnShow()
  {
    base.OnShow();
    this.mTitleText.MarkAsDirty();
    this.mTitleText.Position = new Vector2(this.mPosition.X, (float) ((double) this.mPosition.Y - (double) this.mSize.Y * 0.5 * (double) this.mScale + (double) this.mMargins.Z * (double) this.mScale));
    this.mMessageText.MarkAsDirty();
    this.mMessageText.Position = new Vector2(this.mPosition.X, this.mPosition.Y - (float) (this.mLineHeight * this.mMessageLineCount) * 0.5f * this.mScale);
    this.mExtraText.MarkAsDirty();
    this.mExtraText.Position = new Vector2((float) ((double) this.mPosition.X + (double) this.mSize.X * 0.5 * (double) this.mScale - (double) this.mMargins.Z * (double) this.mScale), (float) ((double) this.mPosition.Y + (double) this.mSize.Y * 0.5 * (double) this.mScale - (double) this.mMargins.W * (double) this.mScale - (double) (this.mLineHeight * this.mExtraLineCount) * 0.5 * (double) this.mScale));
    this.AlignButtons();
    foreach (MenuItem mButton in this.mButtons)
      mButton.Selected = false;
    if (this.mLoadingImageState == null)
      return;
    Vector2 mPosition = this.mPosition;
    mPosition.X -= MenuMessagePopup.LOADING_IMAGE_SIZE.X * 0.5f * this.mScale;
    mPosition.Y += (float) ((double) this.mSize.Y * 0.5 * (double) this.mScale - (100.0 + (double) MenuMessagePopup.LOADING_IMAGE_SIZE.Y) * (double) this.mScale);
    this.mLoadingImageState.SetTransform(mPosition, MenuMessagePopup.LOADING_IMAGE_SIZE * this.mScale);
    this.mLoadingImageState.Reset();
  }

  public override void OnHide()
  {
    if (!this.mClearOnHide)
      return;
    this.SetTitle(0, Color.White);
    this.SetMessage(0, Color.White);
    this.SetExtra(0, Color.White);
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    base.Update(iDataChannel, iDeltaTime);
    if (this.mLoadingImageState == null)
      return;
    this.mLoadingImageState.Update(iDeltaTime);
  }

  public override void Draw(GUIBasicEffect iEffect)
  {
    base.Draw(iEffect);
    this.mTitleText.Draw(iEffect, this.mScale);
    this.mMessageText.Draw(iEffect, this.mScale);
    this.mExtraText.Draw(iEffect, this.mScale);
    foreach (MenuItem mButton in this.mButtons)
      mButton.Draw(iEffect, this.mScale);
    if (this.mLoadingImageState == null)
      return;
    iEffect.GraphicsDevice.Vertices[0].SetSource(MenuImagePopup.sVertices, 0, VertexPositionTexture.SizeInBytes);
    iEffect.GraphicsDevice.VertexDeclaration = MenuImagePopup.sDeclaration;
    iEffect.VertexColorEnabled = false;
    Vector4 vector4 = new Vector4(Vector3.One, this.mLoadingImageState.Alpha);
    iEffect.Color = vector4;
    iEffect.Saturation = 1f;
    iEffect.Texture = (Texture) this.mLoadingImageState.Img;
    iEffect.TextureOffset = this.mLoadingImageState.TextureCoord;
    iEffect.TextureScale = this.mLoadingImageState.TextureScale;
    iEffect.TextureEnabled = true;
    iEffect.Transform = this.mLoadingImageState.Transform;
    iEffect.CommitChanges();
    iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
    iEffect.GraphicsDevice.Vertices[0].SetSource((VertexBuffer) null, 0, 0);
    iEffect.Color = this.mColour;
  }

  internal override void ControllerMouseMove(
    Controller iSender,
    Point iScreenSize,
    MouseState iState,
    MouseState iOldState)
  {
    this.mSelectedItem = -1;
    for (int index = 0; index < this.mButtons.Count; ++index)
    {
      if (this.mButtons[index].InsideBounds(iState))
      {
        this.mButtons[index].Selected = true;
        this.mSelectedItem = index;
      }
      else
        this.mButtons[index].Selected = false;
    }
  }

  internal override void ControllerMouseAction(
    Controller iSender,
    Point iScreenSize,
    MouseState iState,
    MouseState iOldState)
  {
    if (this.mSelectedItem == 0)
    {
      this.Dismiss();
      if (this.mOnPositiveClickDelegate == null)
        return;
      this.mOnPositiveClickDelegate();
      this.mOnPositiveClickDelegate = (Action) null;
    }
    else
    {
      if (this.mSelectedItem != 1)
        return;
      this.Dismiss();
      if (this.mOnNegativeClickDelegate == null)
        return;
      this.mOnNegativeClickDelegate();
      this.mOnNegativeClickDelegate = (Action) null;
    }
  }

  internal override void ControllerA(Controller iSender)
  {
    if (this.mSelectedItem == 0)
    {
      this.Dismiss();
      if (this.mOnPositiveClickDelegate == null)
        return;
      this.mOnPositiveClickDelegate();
      this.mOnPositiveClickDelegate = (Action) null;
    }
    else
    {
      if (this.mSelectedItem != 1)
        return;
      this.Dismiss();
      if (this.mOnNegativeClickDelegate == null)
        return;
      this.mOnNegativeClickDelegate();
      this.mOnNegativeClickDelegate = (Action) null;
    }
  }

  private enum FadeState
  {
    FadeIn,
    FadeOut,
  }

  protected class LoadingImageState
  {
    public readonly Texture2D Img;
    public readonly Vector2 TextureCoord;
    public readonly Vector2 TextureScale;
    private MenuMessagePopup.FadeState mFadeState;
    private float mFadeTime;

    public float Alpha { get; private set; }

    public Matrix Transform { get; private set; }

    public LoadingImageState(
      Texture2D iTexture,
      Vector2 iTextureCoord,
      Vector2 iTextureScale,
      float iFadeTime)
    {
      this.Alpha = 1f;
      this.mFadeState = MenuMessagePopup.FadeState.FadeOut;
      this.Img = iTexture;
      this.TextureCoord = iTextureCoord / new Vector2((float) iTexture.Width, (float) iTexture.Height);
      this.TextureScale = iTextureScale / new Vector2((float) iTexture.Width, (float) iTexture.Height);
      this.mFadeTime = iFadeTime;
    }

    public void SetTransform(Vector2 iPosition, Vector2 iSize)
    {
      this.Transform = Matrix.Identity with
      {
        M11 = iSize.X,
        M22 = iSize.Y,
        M41 = iPosition.X,
        M42 = iPosition.Y
      };
    }

    public void Reset()
    {
      this.Alpha = 1f;
      this.mFadeState = MenuMessagePopup.FadeState.FadeOut;
    }

    public void Update(float iDeltaTime)
    {
      this.Alpha += (this.mFadeState == MenuMessagePopup.FadeState.FadeIn ? iDeltaTime : -iDeltaTime) / this.mFadeTime;
      if ((double) this.Alpha >= 0.0 && (double) this.Alpha <= 1.0)
        return;
      this.mFadeState = this.mFadeState == MenuMessagePopup.FadeState.FadeIn ? MenuMessagePopup.FadeState.FadeOut : MenuMessagePopup.FadeState.FadeIn;
      this.Alpha = (double) this.Alpha < 0.0 ? 0.0f : ((double) this.Alpha > 1.0 ? 1f : this.Alpha);
    }
  }
}
