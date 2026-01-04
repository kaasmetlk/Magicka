// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.MenuScrollBar
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

public class MenuScrollBar : MenuItem
{
  public const float SIZE = 64f;
  private static Texture2D sTexture;
  private static VertexBuffer sVertices;
  private static VertexDeclaration sVertexDeclaration;
  private static int sVertexStride;
  private int mMaxValue;
  private float mHeight;
  private float mScrollLength;
  private int mValue;
  private Vector2 mTextureOffset;
  private Vector2 mSize;
  private Vector4 mScrollUp;
  private Vector4 mScrollDown;
  private Vector4 mScrollDrag;
  private float mScrollDragUp;
  private float mScrollDragDown;

  public MenuScrollBar(Vector2 iPosition, float iHeight, int iMaxValue)
  {
    iMaxValue = Math.Max(0, iMaxValue);
    this.mHeight = iHeight - 128f;
    this.mMaxValue = iMaxValue;
    this.mColor = new Vector4(1f);
    this.mValue = 0;
    this.mScrollLength = this.mHeight / (float) iMaxValue;
    this.mSize.X = 64f;
    this.mSize.Y = iHeight;
    this.mPosition = iPosition;
    this.mScrollUp = new Vector4();
    this.mScrollDown = new Vector4();
    this.mScrollDrag = new Vector4();
    this.UpdateBoundingBox();
    this.mTransform = Matrix.Identity;
    this.mTransform.M41 = this.mPosition.X;
    this.mTransform.M42 = this.mPosition.Y;
    if (MenuScrollBar.sVertices != null)
      return;
    MenuScrollBar.sTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/Pages");
    float num1 = 64f / (float) MenuScrollBar.sTexture.Width;
    float num2 = 64f / (float) MenuScrollBar.sTexture.Height;
    VertexPositionTexture[] data = new VertexPositionTexture[24];
    float x1 = 1280f / (float) MenuScrollBar.sTexture.Width;
    float y1 = 96f / (float) MenuScrollBar.sTexture.Height;
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
    float y2 = 64f / (float) MenuScrollBar.sTexture.Width;
    data[8].Position = new Vector3(-32f, -32f, 0.0f);
    data[8].TextureCoordinate = new Vector2(x1, y2);
    data[9].Position = new Vector3(32f, -32f, 0.0f);
    data[9].TextureCoordinate = new Vector2(x1 + num1, y2);
    data[10].Position = new Vector3(32f, 32f, 0.0f);
    data[10].TextureCoordinate = new Vector2(x1 + num1, y2 + num2);
    data[11].Position = new Vector3(-32f, 32f, 0.0f);
    data[11].TextureCoordinate = new Vector2(x1, y2 + num2);
    float x2 = 1216f / (float) MenuScrollBar.sTexture.Width;
    float y3 = 64f / (float) MenuScrollBar.sTexture.Height;
    data[12].Position = new Vector3(-32f, -32f, 0.0f);
    data[12].TextureCoordinate = new Vector2(x2, y3);
    data[13].Position = new Vector3(32f, -32f, 0.0f);
    data[13].TextureCoordinate = new Vector2(x2 + num1, y3);
    data[14].Position = new Vector3(32f, 32f, 0.0f);
    data[14].TextureCoordinate = new Vector2(x2 + num1, y3 + num2);
    data[15].Position = new Vector3(-32f, 32f, 0.0f);
    data[15].TextureCoordinate = new Vector2(x2, y3 + num2);
    float y4 = 32f / (float) MenuScrollBar.sTexture.Height;
    float num3 = 32f / (float) MenuScrollBar.sTexture.Height;
    data[16 /*0x10*/].Position = new Vector3(-32f, -16f, 0.0f);
    data[16 /*0x10*/].TextureCoordinate = new Vector2(x2, y4);
    data[17].Position = new Vector3(32f, -16f, 0.0f);
    data[17].TextureCoordinate = new Vector2(x2 + num1, y4);
    data[18].Position = new Vector3(32f, 16f, 0.0f);
    data[18].TextureCoordinate = new Vector2(x2 + num1, y4 + num3);
    data[19].Position = new Vector3(-32f, 16f, 0.0f);
    data[19].TextureCoordinate = new Vector2(x2, y4 + num3);
    float y5 = 128f / (float) MenuScrollBar.sTexture.Height;
    data[20].Position = new Vector3(-32f, -16f, 0.0f);
    data[20].TextureCoordinate = new Vector2(x2, y5);
    data[21].Position = new Vector3(32f, -16f, 0.0f);
    data[21].TextureCoordinate = new Vector2(x2 + num1, y5);
    data[22].Position = new Vector3(32f, 16f, 0.0f);
    data[22].TextureCoordinate = new Vector2(x2 + num1, y5 + num3);
    data[23].Position = new Vector3(-32f, 16f, 0.0f);
    data[23].TextureCoordinate = new Vector2(x2, y5 + num3);
    MenuScrollBar.sVertexStride = VertexPositionTexture.SizeInBytes;
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    lock (graphicsDevice)
    {
      MenuScrollBar.sVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(VertexPositionTexture.VertexElements);
      MenuScrollBar.sVertices = new VertexBuffer(graphicsDevice, data.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
      MenuScrollBar.sVertices.SetData<VertexPositionTexture>(data);
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

  public float Height
  {
    get => this.mHeight + 128f;
    set
    {
      this.mHeight = value - 128f;
      this.mSize.Y = value;
      this.mScrollLength = this.mHeight / (float) this.mMaxValue;
      this.UpdateBoundingBox();
    }
  }

  public Vector2 TextureOffset
  {
    get
    {
      return new Vector2()
      {
        X = this.mTextureOffset.X * (float) MenuScrollBar.sTexture.Width,
        Y = this.mTextureOffset.Y * (float) MenuScrollBar.sTexture.Height
      };
    }
    set
    {
      this.mTextureOffset.X = value.X / (float) MenuScrollBar.sTexture.Width;
      this.mTextureOffset.Y = value.Y / (float) MenuScrollBar.sTexture.Height;
    }
  }

  public void ScrollTo(float iY)
  {
    this.Value = (int) (0.5 + (double) ((float) ((double) iY - (double) this.mTopLeft.Y - 64.0 * (double) this.mScale) / this.mScale) / (double) this.mScrollLength);
  }

  public bool Grabbed { get; set; }

  public void SetMaxValue(int iMaxValue)
  {
    iMaxValue = Math.Max(0, iMaxValue);
    if (this.mMaxValue == iMaxValue)
      return;
    this.mMaxValue = iMaxValue;
    this.mValue = 0;
    this.mScrollLength = this.mHeight / (float) iMaxValue;
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

  public bool InsideDragUpBounds(Vector2 iPoint) => (double) iPoint.Y < (double) this.mScrollDragUp;

  public bool InsideDragUpBounds(MouseState iPoint)
  {
    return (double) iPoint.Y < (double) this.mScrollDragUp;
  }

  public bool InsideDragDownBounds(Vector2 iPoint)
  {
    return (double) iPoint.Y > (double) this.mScrollDragDown;
  }

  public bool InsideDragDownBounds(MouseState iPoint)
  {
    return (double) iPoint.Y > (double) this.mScrollDragDown;
  }

  public bool InsideUpBounds(Vector2 iPoint)
  {
    return (double) iPoint.X >= (double) this.mScrollUp.X & (double) iPoint.Y <= (double) this.mScrollUp.W & (double) iPoint.X <= (double) this.mScrollUp.Z & (double) iPoint.Y >= (double) this.mScrollUp.Y;
  }

  public bool InsideUpBounds(MouseState iPoint)
  {
    return (double) iPoint.X >= (double) this.mScrollUp.X & (double) iPoint.Y <= (double) this.mScrollUp.W & (double) iPoint.X <= (double) this.mScrollUp.Z & (double) iPoint.Y >= (double) this.mScrollUp.Y;
  }

  public bool InsideDownBounds(MouseState iPoint)
  {
    return (double) iPoint.X >= (double) this.mScrollDown.X & (double) iPoint.Y <= (double) this.mScrollDown.W & (double) iPoint.X <= (double) this.mScrollDown.Z & (double) iPoint.Y >= (double) this.mScrollDown.Y;
  }

  public bool InsideDownBounds(Vector2 iPoint)
  {
    return (double) iPoint.X >= (double) this.mScrollDown.X & (double) iPoint.Y <= (double) this.mScrollDown.W & (double) iPoint.X <= (double) this.mScrollDown.Z & (double) iPoint.Y >= (double) this.mScrollDown.Y;
  }

  private void UpdateScrollDrag()
  {
    this.mScrollDrag.X = this.mTopLeft.X;
    this.mScrollDrag.Y = this.mTopLeft.Y + (float) (32.0 + (double) this.mValue * (double) this.mScrollLength) * this.mScale;
    this.mScrollDrag.Z = this.mBottomRight.X;
    this.mScrollDrag.W = this.mTopLeft.Y + (float) (32.0 + (double) this.mValue * (double) this.mScrollLength + 64.0) * this.mScale;
    this.mScrollDragUp = this.mScrollDrag.Y + (float) ((64.0 - (double) this.mScrollLength) * 0.5) * this.mScale;
    this.mScrollDragDown = this.mScrollDrag.Y + (float) ((64.0 + (double) this.mScrollLength) * 0.5) * this.mScale;
    this.mScrollUp.X = this.mTopLeft.X;
    this.mScrollUp.Y = this.mTopLeft.Y;
    this.mScrollUp.Z = this.mBottomRight.X;
    this.mScrollUp.W = this.mTopLeft.Y + 64f * this.mScale;
    this.mScrollDown.X = this.mTopLeft.X;
    this.mScrollDown.Y = this.mBottomRight.Y - 64f * this.mScale;
    this.mScrollDown.Z = this.mBottomRight.X;
    this.mScrollDown.W = this.mBottomRight.Y;
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
    iEffect.GraphicsDevice.Vertices[0].SetSource(MenuScrollBar.sVertices, 0, MenuScrollBar.sVertexStride);
    iEffect.GraphicsDevice.VertexDeclaration = MenuScrollBar.sVertexDeclaration;
    iEffect.TextureOffset = this.mTextureOffset;
    iEffect.Saturation = 1f;
    iEffect.Color = this.mSelected ? this.mColorSelected : this.mColor;
    iEffect.Texture = (Texture) MenuScrollBar.sTexture;
    iEffect.TextureEnabled = true;
    iEffect.TextureScale = new Vector2(1f, 1f);
    Matrix matrix = new Matrix();
    matrix.M44 = 1f;
    matrix.M41 = this.mPosition.X;
    matrix.M42 = this.mPosition.Y;
    matrix.M11 = iScale;
    matrix.M22 = iScale * (this.mHeight - this.mSize.X * 0.5f) / this.mSize.X;
    iEffect.Transform = matrix;
    iEffect.CommitChanges();
    iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 12, 2);
    matrix.M41 = this.mPosition.X;
    matrix.M42 = this.mTopLeft.Y + this.mSize.X * iScale;
    matrix.M11 = iScale;
    matrix.M22 = iScale;
    iEffect.Transform = matrix;
    iEffect.CommitChanges();
    iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 16 /*0x10*/, 2);
    matrix.M41 = this.mPosition.X;
    matrix.M42 = this.mBottomRight.Y - this.mSize.X * iScale;
    iEffect.Transform = matrix;
    iEffect.CommitChanges();
    iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 20, 2);
    matrix.M41 = this.mPosition.X + (float) (32.0 - (double) this.mSize.X * 0.5) * iScale;
    matrix.M42 = this.mPosition.Y + (float) (32.0 - (double) this.mSize.Y * 0.5) * iScale;
    matrix.M11 = iScale;
    matrix.M22 = iScale;
    iEffect.Transform = matrix;
    iEffect.CommitChanges();
    iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
    matrix.M42 = this.mPosition.Y + (float) ((double) this.mSize.Y * 0.5 - 32.0) * iScale;
    matrix.M11 *= -1f;
    matrix.M22 *= -1f;
    iEffect.Transform = matrix;
    iEffect.CommitChanges();
    iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 4, 2);
    matrix.M42 = (float) ((double) this.mPosition.Y - (double) this.mHeight * 0.5 * (double) iScale + (double) this.mValue * (double) this.mScrollLength * (double) iScale);
    iEffect.Transform = matrix;
    iEffect.CommitChanges();
    iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 8, 2);
    iEffect.GraphicsDevice.Vertices[0].SetSource((VertexBuffer) null, 0, 0);
    iEffect.TextureOffset = new Vector2();
  }

  public override void LanguageChanged()
  {
  }
}
