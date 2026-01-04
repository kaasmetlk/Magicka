// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.MenuTextButtonItem
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

public class MenuTextButtonItem : MenuItem
{
  public static readonly Vector2 DEFAULT_UV_OFFSET = new Vector2(768f, 384f);
  public static readonly Vector2 DEFAULT_SIZE = new Vector2(128f, 64f);
  private static VertexBuffer sVertices;
  private static VertexDeclaration sDeclaration;
  private int mText;
  private Text mTitle;
  private BitmapFont mFont;
  private Texture2D mTexture;
  private Vector2 mSize;
  private float mMinWidth;
  private float mMaxWidth;
  private Vector2 mTextSize;
  private Vector2 mUVOffset;
  private Vector2 mUVScale;
  private TextAlign mAlignment;
  private float mLeftPosition;
  private float mMiddlePosition;
  private float mRightPosition;

  static MenuTextButtonItem()
  {
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      MenuTextButtonItem.sVertices = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, Defines.QUAD_TEX_VERTS_C.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
      MenuTextButtonItem.sVertices.SetData<VertexPositionTexture>(Defines.QUAD_TEX_VERTS_C);
      MenuTextButtonItem.sDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
    }
  }

  public MenuTextButtonItem(
    Vector2 iPosition,
    Texture2D iTexture,
    Vector2 iUVOffset,
    Vector2 iUVSize,
    int iText,
    BitmapFont iFont,
    float iMinWidth,
    TextAlign iButtonAlignment)
    : this(iPosition, iTexture, iUVOffset, iUVSize, iText, iFont, iMinWidth, float.MaxValue, iButtonAlignment)
  {
  }

  public MenuTextButtonItem(
    Vector2 iPosition,
    Texture2D iTexture,
    Vector2 iUVOffset,
    Vector2 iUVSize,
    int iText,
    BitmapFont iFont,
    float iMinWidth,
    float iMaxWidth,
    TextAlign iButtonAlignment)
  {
    this.mFont = iFont;
    this.mText = iText;
    this.mTexture = iTexture;
    this.mPosition = iPosition;
    this.mUVOffset.X = iUVOffset.X / (float) iTexture.Width;
    this.mUVOffset.Y = iUVOffset.Y / (float) iTexture.Height;
    this.mSize = iUVSize;
    this.mUVScale.X = iUVSize.X / (float) iTexture.Width;
    this.mUVScale.Y = iUVSize.Y / (float) iTexture.Height;
    this.mAlignment = iButtonAlignment;
    this.mTitle = new Text(40, iFont, TextAlign.Center, false);
    string iText1 = LanguageManager.Instance.GetString(iText);
    this.mTitle.SetText(iText1);
    this.mTextSize = this.mFont.MeasureText(iText1, true);
    this.mMinWidth = iMinWidth;
    this.mMaxWidth = iMaxWidth;
    this.UpdateBoundingBox();
    this.mTransform = Matrix.Identity;
    this.mTransform.M41 = this.mPosition.X;
    this.mTransform.M42 = this.mPosition.Y;
    this.mColor = Defines.DIALOGUE_COLOR_DEFAULT;
    this.mColorDisabled = this.mColor;
  }

  protected override void UpdateBoundingBox()
  {
    float num = Math.Max(this.mMinWidth, Math.Min(this.mMaxWidth, this.mTextSize.X + this.mSize.X * 0.5f));
    switch (this.mAlignment)
    {
      case TextAlign.Left:
        this.mLeftPosition = 0.0f;
        this.mMiddlePosition = num * 0.5f;
        this.mRightPosition = num;
        break;
      case TextAlign.Right:
        this.mLeftPosition = -num;
        this.mMiddlePosition = (float) (-(double) num * 0.5);
        this.mRightPosition = 0.0f;
        break;
      default:
        this.mLeftPosition = (float) (-(double) num * 0.5);
        this.mMiddlePosition = 0.0f;
        this.mRightPosition = num * 0.5f;
        break;
    }
    this.mTopLeft.X = this.mPosition.X + this.mLeftPosition * this.mScale;
    this.mTopLeft.Y = this.mPosition.Y - this.mSize.Y * 0.5f * this.mScale;
    this.mBottomRight.X = this.mPosition.X + this.mRightPosition * this.mScale;
    this.mBottomRight.Y = this.mPosition.Y + this.mSize.Y * 0.5f * this.mScale;
  }

  public void SetText(string iText)
  {
    this.mText = 0;
    this.mTitle.SetText(iText);
    this.mTextSize = this.mFont.MeasureText(iText, true);
    this.UpdateBoundingBox();
  }

  public void SetText(int iText)
  {
    this.mText = iText;
    this.LanguageChanged();
  }

  public Vector2 UVOffset
  {
    get => this.mUVOffset;
    set => this.mUVOffset = value;
  }

  public Vector2 UVScale
  {
    get => this.mUVScale;
    set => this.mUVScale = value;
  }

  public Vector2 Size => this.mSize;

  public float RealWidth
  {
    get
    {
      return Math.Max(this.mMinWidth, Math.Min(this.mMaxWidth, this.mTextSize.X + this.mSize.X * 0.5f));
    }
  }

  public override void Draw(GUIBasicEffect iEffect) => this.Draw(iEffect, this.mScale, this.mAlpha);

  public override void Draw(GUIBasicEffect iEffect, float iScale)
  {
    this.Draw(iEffect, iScale, this.mAlpha);
  }

  public void Draw(GUIBasicEffect iEffect, float iScale, float iAlpha)
  {
    float num = Math.Max(this.mMinWidth, Math.Min(this.mMaxWidth, this.mTextSize.X + this.mSize.X * 0.5f));
    Vector2 mUvOffset = this.mUVOffset;
    Vector2 mUvScale = this.mUVScale;
    iEffect.GraphicsDevice.Vertices[0].SetSource(MenuTextButtonItem.sVertices, 0, VertexPositionTexture.SizeInBytes);
    iEffect.GraphicsDevice.VertexDeclaration = MenuTextButtonItem.sDeclaration;
    Vector4 one = Vector4.One with { W = iAlpha };
    iEffect.Color = one;
    iEffect.Texture = (Texture) this.mTexture;
    iEffect.TextureEnabled = true;
    iEffect.VertexColorEnabled = false;
    iEffect.Saturation = this.mEnabled ? (!this.mSelected ? 1f : 1.5f) : 0.0f;
    Matrix mTransform = this.mTransform with
    {
      M22 = iScale * this.mSize.Y,
      M11 = (float) ((double) iScale * (double) this.mSize.X * 0.25),
      M41 = this.mPosition.X + (this.mLeftPosition + (float) ((double) this.mSize.X * 0.25 * 0.5)) * iScale,
      M42 = this.mPosition.Y
    };
    iEffect.Transform = mTransform;
    iEffect.TextureOffset = mUvOffset;
    mUvScale.X = this.mUVScale.X * 0.25f;
    iEffect.TextureScale = mUvScale;
    iEffect.CommitChanges();
    iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
    mTransform.M11 = iScale * (num - this.mSize.X * 0.5f);
    mTransform.M41 = this.mPosition.X + this.mMiddlePosition * iScale;
    iEffect.Transform = mTransform;
    mUvOffset.X = this.mUVOffset.X + this.mUVScale.X * 0.25f;
    iEffect.TextureOffset = mUvOffset;
    mUvScale.X = this.mUVScale.X * 0.5f;
    iEffect.TextureScale = mUvScale;
    iEffect.CommitChanges();
    iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
    mTransform.M11 = (float) ((double) iScale * (double) this.mSize.X * 0.25);
    mTransform.M41 = this.mPosition.X + (this.mRightPosition - (float) ((double) this.mSize.X * 0.25 * 0.5)) * iScale;
    iEffect.Transform = mTransform;
    mUvOffset.X = this.mUVOffset.X + this.mUVScale.X * 0.75f;
    iEffect.TextureOffset = mUvOffset;
    mUvScale.X = this.mUVScale.X * 0.25f;
    iEffect.TextureScale = mUvScale;
    iEffect.CommitChanges();
    iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
    iEffect.GraphicsDevice.Vertices[0].SetSource((VertexBuffer) null, 0, 0);
    Vector4 vector4 = this.mEnabled ? (this.mSelected ? this.mColorSelected : this.mColor) : this.mColorDisabled;
    vector4.W *= iAlpha * iAlpha;
    iEffect.Color = vector4;
    mTransform.M11 = Math.Min((this.mMaxWidth - this.mSize.X * 0.5f) / this.mTextSize.X, 1f) * iScale;
    mTransform.M22 = iScale;
    mTransform.M41 = this.mPosition.X + this.mMiddlePosition * iScale;
    mTransform.M42 = this.mPosition.Y - this.mTextSize.Y * 0.5f * iScale;
    this.mTitle.Draw(iEffect, ref mTransform);
  }

  public override void LanguageChanged()
  {
    if (this.mText == 0)
      return;
    string iText = LanguageManager.Instance.GetString(this.mText);
    this.mTitle.SetText(iText);
    this.mTextSize = this.mFont.MeasureText(iText, true);
    this.UpdateBoundingBox();
  }
}
