// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.MenuItemMultiLineText
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Graphics;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using PolygonHead;
using PolygonHead.Effects;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu;

public class MenuItemMultiLineText : MenuItem
{
  private const int MAX_TEXT_LENGTH = 128 /*0x80*/;
  private Vector2 mSize = Vector2.Zero;
  private BitmapFont mFont;
  private List<Text> mText;
  private List<int> mLocIds;
  private float mLineSpacing;

  public MenuItemMultiLineText(Vector2 iPosition, MagickaFont iFont)
    : this(iPosition, iFont, 0.0f)
  {
  }

  public MenuItemMultiLineText(Vector2 iPosition, MagickaFont iFont, float iSpacing)
  {
    this.mText = new List<Text>();
    this.mLocIds = new List<int>();
    this.mPosition = iPosition;
    this.mFont = FontManager.Instance.GetFont(iFont);
    this.mLineSpacing = iSpacing;
  }

  public Vector2 Size => this.mSize;

  public override void LanguageChanged()
  {
    for (int index = 0; index < this.mText.Count; ++index)
    {
      if (this.mLocIds[index] != 0)
        this.mText[index].SetText(LanguageManager.Instance.GetString(this.mLocIds[index]));
    }
  }

  public void AddLines(int iLoc, TextAlign iAlign, Vector4 iColour, int iMaxWidth)
  {
    string str = this.mFont.Wrap(LanguageManager.Instance.GetString(iLoc), iMaxWidth, true);
    char[] chArray = new char[1]{ '\n' };
    foreach (string iLoc1 in str.Split(chArray))
      this.AddNewLine(iLoc1, iAlign, iColour);
    this.UpdateBoundingBox();
  }

  public void AddNewLine(int iLoc, TextAlign iAlign, Vector4 iColour)
  {
    Text text = new Text(128 /*0x80*/, this.mFont, TextAlign.Center, true);
    text.SetText(LanguageManager.Instance.GetString(iLoc));
    text.DefaultColor = iColour;
    this.mText.Add(text);
    this.mLocIds.Add(iLoc);
    this.UpdateBoundingBox();
  }

  public void AddNewLine(string iLoc, TextAlign iAlign, Vector4 iColour)
  {
    Text text = new Text(100, this.mFont, iAlign, true);
    text.SetText(iLoc);
    text.DefaultColor = iColour;
    text.Position = new Vector2(0.0f, (float) this.mText.Count * ((float) this.mFont.LineHeight + this.mLineSpacing));
    this.mText.Add(text);
    this.mLocIds.Add(0);
    this.UpdateBoundingBox();
  }

  public void SetText(int iIndex, int iLoc)
  {
    if (iIndex < 0 || iIndex >= this.mText.Count)
      return;
    this.SetText(iIndex, LanguageManager.Instance.GetString(iLoc));
    this.mLocIds[iIndex] = iLoc;
  }

  public void SetText(int iIndex, string iLoc)
  {
    if (iIndex < 0 || iIndex >= this.mText.Count)
      return;
    this.mText[iIndex].SetText(iLoc);
    this.UpdateBoundingBox();
  }

  public void SetColour(int iIndex, Vector4 iColour)
  {
    if (iIndex < 0 || iIndex >= this.mText.Count)
      return;
    this.mText[iIndex].DefaultColor = iColour;
  }

  public void SetColour(int iIndex, Microsoft.Xna.Framework.Graphics.Color iColour)
  {
    this.SetColour(iIndex, iColour.ToVector4());
  }

  public void MarkAsDirty()
  {
    foreach (Text text in this.mText)
      text.MarkAsDirty();
  }

  protected override void UpdateBoundingBox()
  {
    this.mSize = Vector2.Zero;
    float y = this.mPosition.Y - (float) this.mFont.LineHeight * 0.5f;
    for (int index = 0; index < this.mText.Count; ++index)
    {
      float x = this.mFont.MeasureText(this.mText[index].Characters, true).X;
      if ((double) x > (double) this.mSize.X)
        this.mSize.X = x;
      this.mText[index].Position = new Vector2(this.mPosition.X, y);
      this.mSize.Y += (float) this.mFont.LineHeight + this.mLineSpacing;
      y += (float) this.mFont.LineHeight + this.mLineSpacing;
    }
    this.mSize.Y -= this.mLineSpacing;
    this.mTopLeft.X = this.mPosition.X - this.mSize.X * 0.5f * this.mScale;
    this.mTopLeft.Y = this.mPosition.Y;
    this.mBottomRight.X = this.mPosition.X + this.mSize.X * 0.5f * this.mScale;
    this.mBottomRight.Y = this.mPosition.Y + this.mSize.Y * this.mScale;
    this.mTopLeft.Y -= (float) this.mFont.LineHeight * 0.5f * this.mScale;
    this.mBottomRight.Y -= (float) this.mFont.LineHeight * 0.5f * this.mScale;
  }

  public override void Draw(GUIBasicEffect iEffect) => this.Draw(iEffect, this.mScale);

  public override void Draw(GUIBasicEffect iEffect, float iScale)
  {
    for (int index = 0; index < this.mText.Count; ++index)
    {
      Vector4 defaultColor = this.mText[index].DefaultColor;
      Vector4 vector4 = this.mEnabled ? (this.mSelected ? this.mColorSelected : defaultColor) : this.mColorDisabled;
      this.mText[index].DefaultColor = new Vector4(vector4.X, vector4.Y, vector4.Z, vector4.W * this.mAlpha);
      this.mText[index].Draw(iEffect, iScale);
      this.mText[index].DefaultColor = defaultColor;
    }
  }
}
