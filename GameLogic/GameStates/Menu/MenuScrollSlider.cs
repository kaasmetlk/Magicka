// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.MenuScrollSlider
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using PolygonHead.Effects;
using System;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu;

public class MenuScrollSlider : MenuItem
{
  private const float SIZE = 64f;
  private static Texture2D sTexture;
  private static VertexBuffer sVertices;
  private static VertexDeclaration sVertexDeclaration;
  private static int sVertexStride;
  private int mMaxValue;
  private float mWidth;
  private float mScrollLength;
  private int mValue;
  private Vector2 mTextureOffset;
  private Vector2 mSize;
  private Vector4 mScrollLeft;
  private Vector4 mScrollRight;
  private Vector4 mScrollDrag;
  private float mScrollDragLeft;
  private float mScrollDragRight;

  public MenuScrollSlider(Vector2 iPosition, float iWidth, int iMaxValue)
  {
    iMaxValue = Math.Max(1, iMaxValue);
    this.mWidth = iWidth - 128f;
    this.mMaxValue = iMaxValue;
    this.mColor = new Vector4(1f);
    this.mValue = 0;
    this.mScrollLength = this.mWidth / (float) iMaxValue;
    this.mSize.X = iWidth;
    this.mSize.Y = 64f;
    this.mPosition = iPosition;
    this.mScrollLeft = new Vector4();
    this.mScrollRight = new Vector4();
    this.mScrollDrag = new Vector4();
    this.UpdateBoundingBox();
    Matrix.CreateRotationZ(1.57079637f, out this.mTransform);
    this.mTransform.M41 = this.mPosition.X;
    this.mTransform.M42 = this.mPosition.Y;
    if (MenuScrollSlider.sVertices != null)
      return;
    MenuScrollSlider.sTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/Pages");
    float num1 = 64f / (float) MenuScrollSlider.sTexture.Width;
    float num2 = 64f / (float) MenuScrollSlider.sTexture.Height;
    VertexPositionTexture[] data = new VertexPositionTexture[24];
    float x1 = 1280f / (float) MenuScrollSlider.sTexture.Width;
    float y1 = 96f / (float) MenuScrollSlider.sTexture.Height;
    data[0].Position = new Vector3(-32f, -32f, 0.0f);
    data[0].TextureCoordinate = new Vector2(x1, y1);
    data[1].Position = new Vector3(32f, -32f, 0.0f);
    data[1].TextureCoordinate = new Vector2(x1 + num1, y1);
    data[2].Position = new Vector3(32f, 32f, 0.0f);
    data[2].TextureCoordinate = new Vector2(x1 + num1, y1 + num2);
    data[3].Position = new Vector3(-32f, 32f, 0.0f);
    data[3].TextureCoordinate = new Vector2(x1, y1 + num2);
    data[4].Position = new Vector3(-32f, -32f, 0.0f);
    data[4].TextureCoordinate = new Vector2(x1, y1);
    data[5].Position = new Vector3(32f, -32f, 0.0f);
    data[5].TextureCoordinate = new Vector2(x1 + num1, y1);
    data[6].Position = new Vector3(32f, 32f, 0.0f);
    data[6].TextureCoordinate = new Vector2(x1 + num1, y1 + num2);
    data[7].Position = new Vector3(-32f, 32f, 0.0f);
    data[7].TextureCoordinate = new Vector2(x1, y1 + num2);
    float y2 = 64f / (float) MenuScrollSlider.sTexture.Width;
    data[8].Position = new Vector3(-32f, -32f, 0.0f);
    data[8].TextureCoordinate = new Vector2(x1, y2);
    data[9].Position = new Vector3(32f, -32f, 0.0f);
    data[9].TextureCoordinate = new Vector2(x1 + num1, y2);
    data[10].Position = new Vector3(32f, 32f, 0.0f);
    data[10].TextureCoordinate = new Vector2(x1 + num1, y2 + num2);
    data[11].Position = new Vector3(-32f, 32f, 0.0f);
    data[11].TextureCoordinate = new Vector2(x1, y2 + num2);
    float x2 = 1216f / (float) MenuScrollSlider.sTexture.Width;
    float y3 = 64f / (float) MenuScrollSlider.sTexture.Height;
    data[12].Position = new Vector3(-32f, -32f, 0.0f);
    data[12].TextureCoordinate = new Vector2(x2, y3);
    data[13].Position = new Vector3(32f, -32f, 0.0f);
    data[13].TextureCoordinate = new Vector2(x2 + num1, y3);
    data[14].Position = new Vector3(32f, 32f, 0.0f);
    data[14].TextureCoordinate = new Vector2(x2 + num1, y3 + num2);
    data[15].Position = new Vector3(-32f, 32f, 0.0f);
    data[15].TextureCoordinate = new Vector2(x2, y3 + num2);
    float y4 = 32f / (float) MenuScrollSlider.sTexture.Height;
    float num3 = 32f / (float) MenuScrollSlider.sTexture.Height;
    data[16 /*0x10*/].Position = new Vector3(-32f, -16f, 0.0f);
    data[16 /*0x10*/].TextureCoordinate = new Vector2(x2, y4);
    data[17].Position = new Vector3(32f, -16f, 0.0f);
    data[17].TextureCoordinate = new Vector2(x2 + num1, y4);
    data[18].Position = new Vector3(32f, 16f, 0.0f);
    data[18].TextureCoordinate = new Vector2(x2 + num1, y4 + num3);
    data[19].Position = new Vector3(-32f, 16f, 0.0f);
    data[19].TextureCoordinate = new Vector2(x2, y4 + num3);
    float y5 = 128f / (float) MenuScrollSlider.sTexture.Height;
    data[20].Position = new Vector3(-32f, -16f, 0.0f);
    data[20].TextureCoordinate = new Vector2(x2, y5);
    data[21].Position = new Vector3(32f, -16f, 0.0f);
    data[21].TextureCoordinate = new Vector2(x2 + num1, y5);
    data[22].Position = new Vector3(32f, 16f, 0.0f);
    data[22].TextureCoordinate = new Vector2(x2 + num1, y5 + num3);
    data[23].Position = new Vector3(-32f, 16f, 0.0f);
    data[23].TextureCoordinate = new Vector2(x2, y5 + num3);
    MenuScrollSlider.sVertexStride = VertexPositionTexture.SizeInBytes;
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    lock (graphicsDevice)
    {
      MenuScrollSlider.sVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(VertexPositionTexture.VertexElements);
      MenuScrollSlider.sVertices = new VertexBuffer(graphicsDevice, data.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
      MenuScrollSlider.sVertices.SetData<VertexPositionTexture>(data);
    }
  }

  public int Value
  {
    get => this.mValue;
    set
    {
      this.mValue = value >= 0 ? (value <= this.mMaxValue ? value : this.mMaxValue) : 0;
      this.UpdateScrollDrag();
    }
  }

  public int MaxValue => this.mMaxValue;

  public float Width
  {
    get => this.mWidth + 128f;
    set
    {
      this.mWidth = value - 128f;
      this.mSize.X = value;
      this.mScrollLength = this.mWidth / (float) this.mMaxValue;
      this.UpdateBoundingBox();
    }
  }

  public Vector2 TextureOffset
  {
    get
    {
      return new Vector2()
      {
        X = this.mTextureOffset.X * (float) MenuScrollSlider.sTexture.Width,
        Y = this.mTextureOffset.Y * (float) MenuScrollSlider.sTexture.Height
      };
    }
    set
    {
      this.mTextureOffset.X = value.X / (float) MenuScrollSlider.sTexture.Width;
      this.mTextureOffset.Y = value.Y / (float) MenuScrollSlider.sTexture.Height;
    }
  }

  public void ScrollTo(float iX)
  {
    this.Value = (int) (0.5 + (double) ((float) ((double) iX - (double) this.mTopLeft.X - 64.0 * (double) this.mScale) / this.mScale) / (double) this.mScrollLength);
  }

  public bool Grabbed { get; set; }

  public void SetMaxValue(int iMaxValue)
  {
    iMaxValue = Math.Max(0, iMaxValue);
    if (this.mMaxValue == iMaxValue)
      return;
    this.mMaxValue = iMaxValue;
    this.mValue = 0;
    this.mScrollLength = this.mWidth / (float) iMaxValue;
    this.UpdateBoundingBox();
  }

  public bool InsideDragBounds(Vector2 iPoint)
  {
    return (double) iPoint.X >= (double) this.mScrollDrag.X & (double) iPoint.Y >= (double) this.mScrollDrag.Y & (double) iPoint.X <= (double) this.mScrollDrag.Z & (double) iPoint.Y <= (double) this.mScrollDrag.W;
  }

  public bool InsideDragBounds(MouseState iPoint)
  {
    return (double) iPoint.X >= (double) this.mScrollDrag.X & (double) iPoint.Y >= (double) this.mScrollDrag.Y & (double) iPoint.X <= (double) this.mScrollDrag.Z & (double) iPoint.Y <= (double) this.mScrollDrag.W;
  }

  public bool InsideDragLeftBounds(Vector2 iPoint)
  {
    return (double) iPoint.X < (double) this.mScrollDragLeft;
  }

  public bool InsideDragLeftBounds(MouseState iPoint)
  {
    return (double) iPoint.X < (double) this.mScrollDragLeft;
  }

  public bool InsideDragRightBounds(Vector2 iPoint)
  {
    return (double) iPoint.X > (double) this.mScrollDragRight;
  }

  public bool InsideDragRightBounds(MouseState iPoint)
  {
    return (double) iPoint.X > (double) this.mScrollDragRight;
  }

  public bool InsideLeftBounds(Vector2 iPoint)
  {
    return (double) iPoint.X >= (double) this.mScrollLeft.X & (double) iPoint.Y >= (double) this.mScrollLeft.Y & (double) iPoint.X <= (double) this.mScrollLeft.Z & (double) iPoint.Y <= (double) this.mScrollLeft.W;
  }

  public bool InsideLeftBounds(MouseState iPoint)
  {
    return (double) iPoint.X >= (double) this.mScrollLeft.X & (double) iPoint.Y <= (double) this.mScrollLeft.W & (double) iPoint.X <= (double) this.mScrollLeft.Z & (double) iPoint.Y >= (double) this.mScrollLeft.Y;
  }

  public bool InsideRightBounds(Vector2 iPoint)
  {
    return (double) iPoint.X >= (double) this.mScrollRight.X & (double) iPoint.Y <= (double) this.mScrollRight.W & (double) iPoint.X <= (double) this.mScrollRight.Z & (double) iPoint.Y >= (double) this.mScrollRight.Y;
  }

  public bool InsideRightBounds(MouseState iPoint)
  {
    return (double) iPoint.X >= (double) this.mScrollRight.X & (double) iPoint.Y <= (double) this.mScrollRight.W & (double) iPoint.X <= (double) this.mScrollRight.Z & (double) iPoint.Y >= (double) this.mScrollRight.Y;
  }

  private void UpdateScrollDrag()
  {
    this.mScrollDrag.X = this.mTopLeft.X + (float) (48.0 + (double) this.mValue * (double) this.mScrollLength) * this.mScale;
    this.mScrollDrag.Y = this.mTopLeft.Y;
    this.mScrollDrag.Z = this.mTopLeft.X + (float) (48.0 + (double) this.mValue * (double) this.mScrollLength + 32.0) * this.mScale;
    this.mScrollDrag.W = this.mBottomRight.Y;
    this.mScrollDragLeft = this.mScrollDrag.X;
    this.mScrollDragRight = this.mScrollDrag.Z;
    this.mScrollLeft.X = this.mTopLeft.X;
    this.mScrollLeft.Y = this.mTopLeft.Y;
    this.mScrollLeft.Z = this.mTopLeft.X + 64f * this.mScale;
    this.mScrollLeft.W = this.mBottomRight.Y;
    this.mScrollRight.X = this.mBottomRight.X - 64f * this.mScale;
    this.mScrollRight.Y = this.mTopLeft.Y;
    this.mScrollRight.Z = this.mBottomRight.X;
    this.mScrollRight.W = this.mBottomRight.Y;
  }

  protected override void UpdateBoundingBox()
  {
    this.mTopLeft.X = this.mPosition.X - this.mSize.X * 0.5f * this.mScale;
    this.mTopLeft.Y = this.mPosition.Y - this.mSize.Y * 0.5f * this.mScale;
    this.mBottomRight.X = this.mPosition.X + this.mSize.X * 0.5f * this.mScale;
    this.mBottomRight.Y = this.mPosition.Y + this.mSize.Y * 0.5f * this.mScale;
    this.UpdateScrollDrag();
  }

  public override void Draw(GUIBasicEffect iEffect) => this.Draw(iEffect, this.mScale);

  public override void Draw(GUIBasicEffect iEffect, float iScale)
  {
    iEffect.GraphicsDevice.Vertices[0].SetSource(MenuScrollSlider.sVertices, 0, MenuScrollSlider.sVertexStride);
    iEffect.GraphicsDevice.VertexDeclaration = MenuScrollSlider.sVertexDeclaration;
    iEffect.TextureOffset = this.mTextureOffset;
    iEffect.Saturation = 1f;
    iEffect.Color = this.mColor;
    iEffect.Texture = (Texture) MenuScrollSlider.sTexture;
    iEffect.TextureEnabled = true;
    iEffect.TextureScale = Vector2.One;
    Matrix matrix = new Matrix();
    matrix.M44 = 1f;
    matrix.M41 = this.mPosition.X;
    matrix.M42 = this.mPosition.Y;
    matrix.M12 = -iScale;
    matrix.M21 = iScale * (this.mWidth - 32f) / this.mSize.Y;
    iEffect.Transform = matrix;
    iEffect.CommitChanges();
    iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 12, 2);
    matrix.M41 = this.mTopLeft.X + 64f * iScale;
    matrix.M42 = this.mPosition.Y;
    matrix.M12 = -iScale;
    matrix.M21 = iScale;
    iEffect.Transform = matrix;
    iEffect.CommitChanges();
    iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 16 /*0x10*/, 2);
    matrix.M41 = this.BottomRight.X - 64f * iScale;
    matrix.M42 = this.mPosition.Y;
    matrix.M12 = -iScale;
    matrix.M21 = iScale;
    iEffect.Transform = matrix;
    iEffect.CommitChanges();
    iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 20, 2);
    matrix.M41 = this.mPosition.X + (float) ((double) this.mSize.Y * 0.5 + (double) this.mWidth * 0.5) * iScale;
    matrix.M12 = iScale;
    matrix.M21 = -iScale;
    iEffect.Transform = matrix;
    iEffect.CommitChanges();
    iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
    matrix.M41 = this.mPosition.X - (float) ((double) this.mSize.Y * 0.5 + (double) this.mWidth * 0.5) * iScale;
    matrix.M12 *= -1f;
    matrix.M21 *= -1f;
    iEffect.Transform = matrix;
    iEffect.CommitChanges();
    iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 4, 2);
    matrix.M12 = iScale;
    matrix.M21 = -1f;
    matrix.M11 = matrix.M22 = 0.0f;
    matrix.M41 = this.mTopLeft.X + (this.mSize.Y + (float) this.mValue * this.mScrollLength) * iScale;
    iEffect.Transform = matrix;
    iEffect.CommitChanges();
    iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 8, 2);
    iEffect.TextureOffset = new Vector2();
  }

  public override void LanguageChanged()
  {
  }
}
