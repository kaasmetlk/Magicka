// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.DropDownBox
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu;

internal abstract class DropDownBox : MenuItem
{
  private const float OFFSETX = 832f;
  private const float OFFSETY = 128f;
  private const float SIZE = 128f;
  private const float MARGIN = 16f;
  protected int mSelectedIndex;
  private int mNewSelection;
  protected string[] mNames;
  private Text[] mTexts;
  private float[] mTextScales;
  private BitmapFont mFont;
  private int mWidth;
  private Vector2 mSize;
  private bool mIsDown;
  private float mDownAlpha;
  private Vector4 mColorItem = Defines.DIALOGUE_COLOR_DEFAULT;
  private VertexDeclaration mVertexDeclaration;
  private Vector4[] mVertices;
  private VertexBuffer mVertexBuffer;
  private static IndexBuffer sIndices;
  private static Texture2D sTexture;

  public event Action<DropDownBox, int> SelectedIndexChanged;

  public DropDownBox(BitmapFont iFont, string[] iNames, int iWidth)
  {
    this.mWidth = iWidth;
    this.mFont = iFont;
    this.SetNames(iNames);
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    if (DropDownBox.sTexture == null || DropDownBox.sTexture.IsDisposed)
    {
      lock (graphicsDevice)
      {
        if (DropDownBox.sTexture != null)
        {
          if (!DropDownBox.sTexture.IsDisposed)
            goto label_6;
        }
        DropDownBox.sTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/Pages");
      }
    }
label_6:
    Vector2 vector2 = new Vector2();
    vector2.X = 1f / (float) DropDownBox.sTexture.Width;
    vector2.Y = 1f / (float) DropDownBox.sTexture.Height;
    this.mVertices = new Vector4[20];
    this.mVertices[0].X = 0.0f;
    this.mVertices[0].Y = 0.0f;
    this.mVertices[0].Z = 802f * vector2.X;
    this.mVertices[0].W = 130f * vector2.Y;
    this.mVertices[1].X = 28f;
    this.mVertices[1].Y = 0.0f;
    this.mVertices[1].Z = 830f * vector2.X;
    this.mVertices[1].W = 130f * vector2.Y;
    this.mVertices[2].X = 28f;
    this.mVertices[2].Y = 38f;
    this.mVertices[2].Z = 830f * vector2.X;
    this.mVertices[2].W = 168f * vector2.Y;
    this.mVertices[3].X = 0.0f;
    this.mVertices[3].Y = 38f;
    this.mVertices[3].Z = 802f * vector2.X;
    this.mVertices[3].W = 168f * vector2.Y;
    ushort[] data = new ushort[TextBox.INDICES.Length];
    TextBox.INDICES.CopyTo((Array) data, 0);
    for (int index = 0; index < data.Length; ++index)
      data[index] += (ushort) 4;
    lock (graphicsDevice)
    {
      this.mVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(new VertexElement[2]
      {
        new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
        new VertexElement((short) 0, (short) 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 0)
      });
      this.mVertexBuffer = new VertexBuffer(graphicsDevice, 320, BufferUsage.WriteOnly);
      if (DropDownBox.sIndices != null && !DropDownBox.sIndices.IsDisposed)
        return;
      DropDownBox.sIndices = new IndexBuffer(graphicsDevice, data.Length * 2, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
      DropDownBox.sIndices.SetData<ushort>(data);
    }
  }

  protected void SetNames(string[] iNames)
  {
    this.mNames = iNames;
    this.mSelectedIndex = 0;
    this.mTexts = new Text[this.mNames.Length];
    this.mTextScales = new float[this.mNames.Length];
    for (int index = 0; index < this.mTexts.Length; ++index)
    {
      this.mTexts[index] = new Text(64 /*0x40*/, this.mFont, TextAlign.Right, false);
      this.mTextScales[index] = 1f;
    }
  }

  public Vector4 ColorItem => this.mColorItem;

  public Vector2 Size => this.mSize;

  public BitmapFont Font => this.mFont;

  public int Count => this.mNames.Length;

  public int SelectedIndex
  {
    get => this.mSelectedIndex;
    set
    {
      if (!(value >= 0 & value < this.mNames.Length & value != this.mSelectedIndex))
        return;
      this.mSelectedIndex = value;
      this.OnSelectedIndexChanged();
    }
  }

  public int NewSelection
  {
    get => this.mNewSelection;
    set => this.mNewSelection = value;
  }

  public string SelectedName => this.mNames[this.mSelectedIndex];

  protected virtual void OnSelectedIndexChanged()
  {
    if (this.SelectedIndexChanged == null)
      return;
    this.SelectedIndexChanged(this, this.mSelectedIndex);
  }

  protected void UpdateVertices()
  {
    Vector2 vector2 = new Vector2();
    vector2.X = 1f / (float) DropDownBox.sTexture.Width;
    vector2.Y = 1f / (float) DropDownBox.sTexture.Height;
    Vector2 mSize = this.mSize;
    mSize.Y *= (float) this.mNames.Length;
    mSize.Y += 32f;
    Vector2 iMargin = new Vector2();
    iMargin.X = iMargin.Y = 16f;
    SubMenuCharacterSelect.CreateVertices(this.mVertices, 4, ref mSize, ref iMargin, ref new Vector2()
    {
      X = 832f * vector2.X,
      Y = 128f * vector2.Y
    }, ref new Vector2()
    {
      X = 128f * vector2.X,
      Y = 128f * vector2.Y
    }, ref new Vector2()
    {
      X = 16f * vector2.X,
      Y = 16f * vector2.Y
    });
    lock (this.mVertexBuffer.GraphicsDevice)
      this.mVertexBuffer.SetData<Vector4>(this.mVertices);
  }

  protected override void UpdateBoundingBox()
  {
    this.mTopLeft.X = this.mPosition.X;
    this.mBottomRight.X = this.mPosition.X + this.mSize.X * this.mScale;
    if (this.mIsDown)
    {
      this.mTopLeft.Y = this.mPosition.Y + this.mSize.Y * this.mScale;
      this.mBottomRight.Y = this.mPosition.Y + this.mSize.Y * ((float) this.mNames.Length + 1f) * this.mScale;
    }
    else
    {
      this.mTopLeft.Y = this.mPosition.Y;
      this.mBottomRight.Y = this.mPosition.Y + this.mSize.Y * this.mScale;
    }
  }

  internal int GetHitIndex(ref Vector2 iPoint)
  {
    if (!this.mIsDown)
      return -1;
    float x = this.mPosition.X;
    float num1 = this.mPosition.Y + (this.mSize.Y + 16f) * this.mScale;
    float num2 = this.mPosition.X + this.mSize.X * this.mScale;
    float num3 = num1 + this.mSize.Y * this.mScale;
    if ((double) iPoint.X < (double) x || (double) iPoint.X > (double) num2)
      return -1;
    for (int hitIndex = 0; hitIndex < this.mNames.Length; ++hitIndex)
    {
      if ((double) iPoint.Y >= (double) num1 && (double) iPoint.Y <= (double) num3)
        return hitIndex;
      num1 += this.mSize.Y * this.mScale;
      num3 += this.mSize.Y * this.mScale;
    }
    return -1;
  }

  public bool IsDown
  {
    get => this.mIsDown;
    set
    {
      this.mNewSelection = -1;
      this.mIsDown = value;
      this.mSelected = false;
      this.UpdateBoundingBox();
    }
  }

  public void Update(float iDeltaTime)
  {
    if (this.mIsDown && this.mEnabled)
      this.mDownAlpha = Math.Min(this.mDownAlpha + iDeltaTime * 4f, 1f);
    else
      this.mDownAlpha = Math.Max(this.mDownAlpha - iDeltaTime * 4f, 0.0f);
  }

  public override void Draw(GUIBasicEffect iEffect) => this.Draw(iEffect, this.mScale);

  public override void Draw(GUIBasicEffect iEffect, float iScale) => this.Draw(iEffect, iScale, 1f);

  public void Draw(GUIBasicEffect iEffect, float iScale, float iAlpha)
  {
    if (this.mTexts.Length == 0)
      return;
    Vector4 vector4 = new Vector4();
    iEffect.Saturation = this.mEnabled ? (!(this.mSelected & !this.mIsDown) ? 1f : 1.5f) : 0.0f;
    Matrix iTransform1 = new Matrix();
    iTransform1.M11 = iTransform1.M22 = iScale;
    iTransform1.M44 = 1f;
    iTransform1.M41 = this.mPosition.X + (this.mSize.X - 28f) * iScale;
    iTransform1.M42 = this.mPosition.Y;
    iEffect.Transform = iTransform1;
    iEffect.Texture = (Texture) DropDownBox.sTexture;
    vector4.X = vector4.Y = vector4.Z = 1f;
    vector4.W = iAlpha;
    iEffect.Color = vector4;
    iEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
    iEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, 16 /*0x10*/);
    iEffect.CommitChanges();
    iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
    iEffect.Saturation = 1f;
    vector4 = this.mEnabled ? (this.mSelected & !this.mIsDown ? this.mColorSelected : this.mColor) : this.mColorDisabled;
    vector4.W *= iAlpha;
    iEffect.Color = vector4;
    iTransform1.M11 = this.mTextScales[this.mSelectedIndex] * iScale;
    iTransform1.M22 = iScale;
    iTransform1.M41 = this.mPosition.X + (this.mSize.X - 32f) * iScale;
    iTransform1.M42 = this.mPosition.Y;
    this.mTexts[this.mSelectedIndex].Draw(iEffect, ref iTransform1);
    float num = this.mDownAlpha * iAlpha;
    if ((double) num <= 0.0)
      return;
    vector4.X = vector4.Y = vector4.Z = 1f;
    vector4.W = num;
    iEffect.Color = vector4;
    Matrix iTransform2 = new Matrix();
    iTransform2.M11 = iScale;
    iTransform2.M22 = iScale;
    iTransform2.M41 = this.mPosition.X;
    iTransform2.M42 = this.mPosition.Y + this.mSize.Y * iScale;
    iTransform2.M44 = 1f;
    iEffect.Transform = iTransform2;
    iEffect.Texture = (Texture) DropDownBox.sTexture;
    iEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
    iEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, 16 /*0x10*/);
    iEffect.GraphicsDevice.Indices = DropDownBox.sIndices;
    iEffect.CommitChanges();
    iEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 20, 0, 18);
    Vector2 mPosition = this.mPosition;
    mPosition.X += (this.mSize.X - 16f) * iScale;
    mPosition.Y += (this.mSize.Y + 16f) * iScale;
    for (int index = 0; index < this.mTexts.Length; ++index)
    {
      vector4 = index == this.mNewSelection ? this.mColorSelected : this.mColorItem;
      vector4.W *= num * num;
      iEffect.Color = vector4;
      iTransform2.M11 = this.mTextScales[index] * iScale;
      iTransform2.M22 = iScale;
      iTransform2.M41 = mPosition.X;
      iTransform2.M42 = mPosition.Y;
      this.mTexts[index].Draw(iEffect, ref iTransform2);
      mPosition.Y += this.mSize.Y * iScale;
    }
  }

  public override void LanguageChanged()
  {
    Vector2 vector2 = new Vector2();
    vector2.X = (float) this.mWidth;
    vector2.Y = (float) this.mFont.LineHeight;
    for (int index = 0; index < this.mNames.Length; ++index)
    {
      this.mTexts[index].SetText(this.mNames[index]);
      this.mTextScales[index] = Math.Min(((float) this.mWidth - 32f) / this.mFont.MeasureText(this.mNames[index], true).X, 1f);
    }
    this.mSize = vector2;
    this.UpdateVertices();
    this.UpdateBoundingBox();
  }
}
