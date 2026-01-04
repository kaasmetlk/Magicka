// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.MenuTextItem
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Localization;
using Microsoft.Xna.Framework;
using PolygonHead;
using PolygonHead.Effects;
using System;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu;

public class MenuTextItem : MenuItem
{
  private Text mTitle;
  private int mValue;
  private Vector2 mSize;
  private TextAlign mAlign;
  private BitmapFont mFont;
  private string mTitleString;
  private float mMaxWidth;
  private new float mAlpha = 1f;

  public new float Alpha
  {
    get => this.mAlpha;
    set => this.mAlpha = value;
  }

  public MenuTextItem(int iTitle, Vector2 iPosition, BitmapFont iFont, TextAlign iAlign)
  {
    this.mAlign = iAlign;
    this.mFont = iFont;
    this.mValue = iTitle;
    this.mTitle = new Text(40, iFont, iAlign, false);
    this.mTitleString = LanguageManager.Instance.GetString(iTitle);
    this.mTitle.SetText(this.mTitleString);
    this.mPosition = iPosition;
    this.mSize = this.mFont.MeasureText(this.mTitle.Characters, true);
    this.UpdateBoundingBox();
  }

  public MenuTextItem(string iTitle, Vector2 iPosition, BitmapFont iFont, TextAlign iAlign)
  {
    this.mAlign = iAlign;
    this.mFont = iFont;
    this.mValue = 0;
    this.mTitleString = iTitle;
    this.mTitle = new Text(64 /*0x40*/, iFont, iAlign, false);
    this.mTitle.SetText(iTitle);
    this.mPosition = iPosition;
    this.mSize = this.mFont.MeasureText(this.mTitle.Characters, true);
    this.UpdateBoundingBox();
  }

  public MenuTextItem(
    int iTitle,
    Vector2 iPosition,
    BitmapFont iFont,
    TextAlign iAlign,
    bool iUseFormatting)
  {
    this.mAlign = iAlign;
    this.mFont = iFont;
    this.mValue = iTitle;
    this.mTitle = new Text(40, iFont, iAlign, false, iUseFormatting);
    this.mTitleString = LanguageManager.Instance.GetString(iTitle);
    this.mTitle.SetText(this.mTitleString);
    this.mPosition = iPosition;
    this.mSize = this.mFont.MeasureText(this.mTitle.Characters, true);
    this.UpdateBoundingBox();
  }

  public MenuTextItem(
    int iTitle,
    int iCharSize,
    Vector2 iPosition,
    BitmapFont iFont,
    TextAlign iAlign,
    bool iUseFormatting)
  {
    this.mAlign = iAlign;
    this.mFont = iFont;
    this.mValue = iTitle;
    this.mTitle = new Text(iCharSize, iFont, iAlign, false, iUseFormatting);
    this.mTitleString = LanguageManager.Instance.GetString(iTitle);
    this.mTitle.SetText(this.mTitleString);
    this.mPosition = iPosition;
    this.mSize = this.mFont.MeasureText(this.mTitle.Characters, true);
    this.UpdateBoundingBox();
  }

  public MenuTextItem(
    string iTitle,
    Vector2 iPosition,
    BitmapFont iFont,
    TextAlign iAlign,
    bool iUseFormatting)
  {
    this.mAlign = iAlign;
    this.mFont = iFont;
    this.mValue = 0;
    this.mTitleString = iTitle;
    this.mTitle = new Text(40, iFont, iAlign, false, iUseFormatting);
    this.mTitle.SetText(iTitle);
    this.mPosition = iPosition;
    this.mSize = this.mFont.MeasureText(this.mTitle.Characters, true);
    this.UpdateBoundingBox();
  }

  public int Title
  {
    get => this.mValue;
    set
    {
      this.mValue = value;
      this.mTitleString = LanguageManager.Instance.GetString(value);
      this.mTitle.SetText(this.mTitleString);
      this.mTitle.MarkAsDirty();
      this.mSize = this.mFont.MeasureText(this.mTitle.Characters, true);
      this.UpdateBoundingBox();
    }
  }

  public float MaxWidth
  {
    get => this.mMaxWidth;
    set
    {
      this.mMaxWidth = value;
      this.UpdateBoundingBox();
    }
  }

  public Text Text => this.mTitle;

  public string Name => this.mTitleString;

  public int Hash
  {
    get => this.mValue;
    set => this.mValue = value;
  }

  public void SetText(string iText)
  {
    this.mTitle.SetText(iText);
    this.mTitleString = iText;
    this.mSize = this.mFont.MeasureText(this.mTitle.Characters, true);
    this.UpdateBoundingBox();
  }

  public void SetText(int iTextHash)
  {
    this.mValue = iTextHash;
    this.mTitle.SetText(LanguageManager.Instance.GetString(iTextHash));
    this.mTitleString = LanguageManager.Instance.GetString(iTextHash);
    this.mSize = this.mFont.MeasureText(this.mTitle.Characters, true);
    this.UpdateBoundingBox();
  }

  public void WrapText(int iLength)
  {
    this.SetText(this.mFont.Wrap(LanguageManager.Instance.GetString(this.mValue), iLength, true));
  }

  protected override void UpdateBoundingBox()
  {
    this.mSize = this.mFont.MeasureText(this.mTitle.Characters, true);
    this.mSize.X = Math.Max(this.mSize.X, 20f);
    this.mSize.Y = Math.Max(this.mSize.Y, 20f);
    if ((double) this.mMaxWidth > 1.4012984643248171E-45)
      this.mSize.X = Math.Min(this.mSize.X, this.mMaxWidth);
    switch (this.mAlign)
    {
      case TextAlign.Left:
        this.mBottomRight.X = this.mPosition.X + this.mSize.X * this.mScale;
        this.mTopLeft.X = this.mPosition.X;
        break;
      case TextAlign.Center:
        this.mBottomRight.X = this.mPosition.X + this.mSize.X * 0.5f * this.mScale;
        this.mTopLeft.X = this.mPosition.X - this.mSize.X * 0.5f * this.mScale;
        break;
      case TextAlign.Right:
        this.mBottomRight.X = this.mPosition.X;
        this.mTopLeft.X = this.mPosition.X - this.mSize.X * this.mScale;
        break;
    }
    this.mTopLeft.Y = this.mPosition.Y - this.mSize.Y * 0.5f * this.mScale;
    this.mBottomRight.Y = this.mPosition.Y + this.mSize.Y * 0.5f * this.mScale;
  }

  public override void Draw(GUIBasicEffect iEffect) => this.Draw(iEffect, this.mScale);

  public Vector4 GetCurrentColor()
  {
    if (!this.mEnabled)
      return this.mColorDisabled;
    return !this.mSelected ? this.mColor : this.mColorSelected;
  }

  public override void Draw(GUIBasicEffect iEffect, float iScale)
  {
    Vector4 vector4 = this.mEnabled ? (this.mSelected ? this.mColorSelected : this.mColor) : this.mColorDisabled;
    iEffect.Color = new Vector4(vector4.X, vector4.Y, vector4.Z, vector4.W * this.mAlpha);
    Matrix iTransform = new Matrix();
    if ((double) this.mMaxWidth > 1.4012984643248171E-45)
    {
      float x = this.mTitle.Font.MeasureText(this.mTitle.Characters, true).X;
      iTransform.M11 = Math.Min(this.mMaxWidth / x, 1f) * iScale;
    }
    else
      iTransform.M11 = iScale;
    iTransform.M22 = iScale;
    iTransform.M41 = this.mPosition.X;
    iTransform.M42 = (float) ((double) this.mPosition.Y - (double) this.mTitle.Font.LineHeight * 0.5 * (double) iScale + 0.5);
    iTransform.M44 = 1f;
    this.mTitle.Draw(iEffect, ref iTransform);
  }

  public override void LanguageChanged()
  {
    if (this.mValue == 0)
      return;
    this.mTitle.SetText(LanguageManager.Instance.GetString(this.mValue));
    this.UpdateBoundingBox();
  }
}
