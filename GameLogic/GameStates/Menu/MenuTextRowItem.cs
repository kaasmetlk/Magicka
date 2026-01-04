// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.MenuTextRowItem
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using PolygonHead;
using PolygonHead.Effects;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu;

public class MenuTextRowItem : MenuItem
{
  private RowItem[] mItems;
  private Text[] mTexts;
  private Vector2 mSize;
  private BitmapFont mFont;

  public MenuTextRowItem(
    Vector2 iPosition,
    Vector2 iSize,
    BitmapFont iFont,
    params RowItem[] iItems)
  {
    this.mScale = 1f;
    this.mFont = iFont;
    this.mSize = iSize;
    this.mItems = iItems;
    this.mPosition = iPosition;
    this.mTexts = new Text[this.mItems.Length];
    for (int index = 0; index < this.mTexts.Length; ++index)
      this.mTexts[index] = new Text(64 /*0x40*/, this.mFont, iItems[index].Alignment, false);
    this.LanguageChanged();
    this.UpdateBoundingBox();
  }

  protected override void UpdateBoundingBox()
  {
    this.mTopLeft.X = this.mPosition.X * this.mScale;
    this.mTopLeft.Y = this.mPosition.Y * this.mScale;
    this.mBottomRight.X = this.mPosition.X + this.mSize.X * this.mScale;
    this.mBottomRight.Y = this.mPosition.Y + this.mSize.Y * this.mScale;
  }

  public override void Draw(GUIBasicEffect iEffect) => this.Draw(iEffect, this.mScale);

  public override void Draw(GUIBasicEffect iEffect, float iScale)
  {
    iEffect.Color = this.mEnabled ? (this.mSelected ? this.mColorSelected : this.mColor) : this.mColorDisabled;
    for (int index = 0; index < this.mTexts.Length; ++index)
      this.mTexts[index].Draw(iEffect, this.mPosition.X + this.mItems[index].RelativePosition * this.mSize.X * iScale, this.mPosition.Y);
  }

  public void SetItemText(int iIndex, string iText)
  {
    if (iIndex < 0 | iIndex >= this.mItems.Length)
      return;
    this.mItems[iIndex].Text = iText;
    this.LanguageChanged();
  }

  public void SetItemPosition(int iIndex, float iRelativePosition)
  {
    if (iIndex < 0 | iIndex >= this.mItems.Length)
      return;
    this.mItems[iIndex].RelativePosition = iRelativePosition;
  }

  public override void LanguageChanged()
  {
    for (int index = 0; index < this.mTexts.Length; ++index)
      this.mTexts[index].SetText(this.mItems[index].Text);
  }
}
