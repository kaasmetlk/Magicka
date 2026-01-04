// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.MenuImageTextItem
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu;

public class MenuImageTextItem : MenuItem
{
  private int mText;
  private Text mTitle;
  private BitmapFont mFont;
  private string mTitleString;
  private Texture2D mTexture;
  private Vector2 mSize;
  private TextAlign mAlignment;
  private Vector2 mTextPosition;
  private Vector2 mTextureOffset;
  private Vector2 mTextureScale;
  private float mLineHeight;
  private Rectangle mHitBox;
  private float mSelectedPower = 1.5f;
  private static VertexBuffer sVertices;
  private static VertexDeclaration sDeclaration;
  private float mSelectedSaturation;
  private float mNormalSaturation;
  private float mDisabledSaturation;

  static MenuImageTextItem()
  {
    VertexPositionTexture[] data = new VertexPositionTexture[4]
    {
      new VertexPositionTexture(new Vector3(0.0f, 0.0f, 0.0f), new Vector2(0.0f, 0.0f)),
      new VertexPositionTexture(new Vector3(1f, 0.0f, 0.0f), new Vector2(1f, 0.0f)),
      new VertexPositionTexture(new Vector3(1f, 1f, 0.0f), new Vector2(1f, 1f)),
      new VertexPositionTexture(new Vector3(0.0f, 1f, 0.0f), new Vector2(0.0f, 1f))
    };
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      MenuImageTextItem.sVertices = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, data.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
      MenuImageTextItem.sVertices.SetData<VertexPositionTexture>(data);
      MenuImageTextItem.sDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
    }
  }

  public Vector2 TextPosition
  {
    get => this.mTextPosition;
    set => this.mTextPosition = value;
  }

  public void SetTitle(int iText)
  {
    this.mText = iText;
    this.mTitleString = this.mText == 0 ? " " : LanguageManager.Instance.GetString(this.mText);
    this.mTitle.SetText(this.mTitleString);
  }

  public void SetTitle(string iText)
  {
    this.mTitleString = iText;
    this.mTitle.SetText(this.mTitleString);
  }

  public MenuImageTextItem(
    Vector2 iPosition,
    Texture2D iTexture,
    Vector2 iTextureOffset,
    Vector2 iTextureScale,
    int iText,
    Vector2 iTextPosition,
    TextAlign iAlignment,
    BitmapFont iFont,
    Vector2 iSize)
  {
    this.mNormalSaturation = 1f;
    this.mSelectedSaturation = 1.3f;
    this.mDisabledSaturation = 0.0f;
    this.mTexture = iTexture;
    this.mPosition = iPosition;
    this.mTextPosition = iTextPosition;
    this.mTextureOffset = iTextureOffset;
    this.mTextureScale = iTextureScale;
    this.mLineHeight = (float) iFont.LineHeight;
    this.mHitBox.Width = (int) iSize.X;
    this.mHitBox.Height = (int) iSize.Y;
    this.mSize = iSize;
    this.mFont = iFont;
    this.mText = iText;
    this.mAlignment = iAlignment;
    this.mTitle = new Text(80 /*0x50*/, iFont, iAlignment, false);
    this.mTitleString = iText == 0 ? " " : LanguageManager.Instance.GetString(iText);
    this.mTitle.SetText(this.mTitleString);
    this.UpdateBoundingBox();
    this.mTransform = Matrix.Identity;
    this.mTransform.M11 = this.mSize.X;
    this.mTransform.M22 = this.mSize.Y;
    this.mTransform.M41 = this.mPosition.X;
    this.mTransform.M42 = this.mPosition.Y;
  }

  public float SelectedPower
  {
    get => this.mSelectedPower;
    set => this.mSelectedPower = value;
  }

  public Text Text => this.mTitle;

  public void SetText(string iText)
  {
    this.mText = 0;
    this.mTitleString = iText;
    this.mTitle.SetText(iText);
    this.UpdateBoundingBox();
  }

  protected override void UpdateBoundingBox()
  {
    this.mTopLeft.X = this.mPosition.X + (float) this.mHitBox.X * this.mScale;
    this.mTopLeft.Y = this.mPosition.Y + (float) this.mHitBox.Y * this.mScale;
    this.mBottomRight.X = this.mPosition.X + (float) (this.mHitBox.X + this.mHitBox.Width) * this.mScale;
    this.mBottomRight.Y = this.mPosition.Y + (float) (this.mHitBox.Y + this.mHitBox.Height) * this.mScale;
  }

  public void ResetHitArea()
  {
    this.mHitBox.X = 0;
    this.mHitBox.Y = 0;
    this.mHitBox.Width = (int) this.mSize.X;
    this.mHitBox.Height = (int) this.mSize.Y;
    this.UpdateBoundingBox();
  }

  public void SetHitArea(int iX, int iY, int iWidth, int iHeight)
  {
    this.mHitBox.X = iX;
    this.mHitBox.Y = iY;
    this.mHitBox.Width = iWidth;
    this.mHitBox.Height = iHeight;
    this.UpdateBoundingBox();
  }

  public Vector2 Size => this.mSize;

  public override void Draw(GUIBasicEffect iEffect) => this.Draw(iEffect, this.mScale);

  public override void Draw(GUIBasicEffect iEffect, float iScale)
  {
    iEffect.GraphicsDevice.Vertices[0].SetSource(MenuImageTextItem.sVertices, 0, VertexPositionTexture.SizeInBytes);
    iEffect.GraphicsDevice.VertexDeclaration = MenuImageTextItem.sDeclaration;
    iEffect.VertexColorEnabled = false;
    Vector4 vector4_1 = Vector4.One;
    if (this.mSelected)
      vector4_1 = new Vector4(this.mSelectedPower, this.mSelectedPower, this.mSelectedPower, Math.Min(this.mSelectedPower, 1f));
    vector4_1.W *= this.mAlpha;
    iEffect.Color = vector4_1;
    this.mTransform.M11 = this.mSize.X * iScale;
    this.mTransform.M22 = this.mSize.Y * iScale;
    iEffect.Texture = (Texture) this.mTexture;
    iEffect.TextureEnabled = true;
    iEffect.TextureOffset = this.mTextureOffset;
    iEffect.TextureScale = this.mTextureScale;
    iEffect.Transform = this.mTransform;
    iEffect.Saturation = this.mEnabled ? (this.mSelected ? this.mSelectedSaturation : this.mNormalSaturation) : this.mDisabledSaturation;
    iEffect.CommitChanges();
    iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
    iEffect.GraphicsDevice.Vertices[0].SetSource((VertexBuffer) null, 0, 0);
    Vector4 vector4_2 = this.mEnabled ? (this.mSelected ? this.mColorSelected : this.mColor) : this.mColorDisabled;
    vector4_2.W *= this.mAlpha;
    iEffect.Color = vector4_2;
    iEffect.TextureOffset = Vector2.Zero;
    iEffect.TextureScale = Vector2.One;
    this.mTitle.Draw(iEffect, (float) ((double) this.mPosition.X + (double) this.mTextPosition.X + 4.0), (float) ((double) this.mPosition.Y + (double) this.mTextPosition.Y - (double) this.mLineHeight + 4.0), iScale);
  }

  public override void LanguageChanged()
  {
    if (this.mText == 0)
      return;
    this.mTitle.SetText(LanguageManager.Instance.GetString(this.mText));
    this.UpdateBoundingBox();
  }
}
