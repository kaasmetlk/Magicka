// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.ContextMenu
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.Graphics;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu;

internal class ContextMenu : MenuItem
{
  private const float OFFSETX = 832f;
  private const float OFFSETY = 128f;
  private const float SIZE = 128f;
  protected const float MARGIN = 16f;
  protected int mSelectedIndex;
  protected List<int[]> mNames;
  protected List<PolygonHead.Text> mTexts;
  protected List<float> mTextScales;
  protected BitmapFont mFont;
  protected int mWidth;
  protected Vector2 mSize;
  private bool mIsVisible;
  private float mDownAlpha;
  private Vector4 mColorItem = Defines.DIALOGUE_COLOR_DEFAULT;
  private VertexDeclaration mVertexDeclaration;
  private Vector4[] mVertices;
  private VertexBuffer mVertexBuffer;
  private static IndexBuffer sIndices;
  private static Texture2D sTexture;
  protected TextAlign mAlignment;
  protected bool mAutoSize;

  public ContextMenu(BitmapFont iFont, TextAlign iAlignment, int? iWidth)
  {
    this.mNames = new List<int[]>();
    this.mSelectedIndex = 0;
    this.mAlignment = iAlignment;
    this.mFont = iFont;
    this.mTexts = new List<PolygonHead.Text>();
    this.mTextScales = new List<float>();
    this.mAutoSize = !iWidth.HasValue;
    if (!this.mAutoSize)
      this.mWidth = iWidth.Value;
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    if (ContextMenu.sTexture == null || ContextMenu.sTexture.IsDisposed)
    {
      lock (graphicsDevice)
      {
        if (ContextMenu.sTexture != null)
        {
          if (!ContextMenu.sTexture.IsDisposed)
            goto label_8;
        }
        ContextMenu.sTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/Pages");
      }
    }
label_8:
    Vector2 vector2 = new Vector2()
    {
      X = 1f / (float) ContextMenu.sTexture.Width,
      Y = 1f / (float) ContextMenu.sTexture.Height
    };
    this.mVertices = new Vector4[16 /*0x10*/];
    lock (graphicsDevice)
    {
      this.mVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(new VertexElement[2]
      {
        new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
        new VertexElement((short) 0, (short) 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 0)
      });
      this.mVertexBuffer = new VertexBuffer(graphicsDevice, 320, BufferUsage.WriteOnly);
      if (ContextMenu.sIndices != null && !ContextMenu.sIndices.IsDisposed)
        return;
      ContextMenu.sIndices = new IndexBuffer(graphicsDevice, TextBox.INDICES.Length * 2, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
      ContextMenu.sIndices.SetData<ushort>(TextBox.INDICES);
    }
  }

  public int AddOption(int iLocValue)
  {
    lock (this.mNames)
    {
      this.mTextScales.Add(1f);
      this.mTexts.Add(new PolygonHead.Text(64 /*0x40*/, this.mFont, this.mAlignment, false));
      this.mNames.Add(new int[1]{ iLocValue });
    }
    this.LanguageChanged();
    return this.mTexts.Count - 1;
  }

  public virtual void RemoveAt(int iIndex)
  {
    this.mNames.RemoveAt(iIndex);
    this.mTexts.RemoveAt(iIndex);
    this.mTextScales.RemoveAt(iIndex);
  }

  public virtual void Clear()
  {
    this.mNames.Clear();
    this.mTexts.Clear();
    this.mTextScales.Clear();
  }

  public Vector4 ColorItem => this.mColorItem;

  public Vector2 Size => this.mSize;

  public BitmapFont Font => this.mFont;

  public int Count => this.mNames.Count;

  public int SelectedIndex
  {
    get => this.mSelectedIndex;
    set => this.mSelectedIndex = value;
  }

  protected void UpdateVertices()
  {
    Vector2 vector2 = new Vector2();
    vector2.X = 1f / (float) ContextMenu.sTexture.Width;
    vector2.Y = 1f / (float) ContextMenu.sTexture.Height;
    Vector2 mSize = this.mSize;
    mSize.Y *= (float) this.mNames.Count;
    mSize.Y += 32f;
    Vector2 iMargin = new Vector2();
    iMargin.X = iMargin.Y = 16f;
    SubMenuCharacterSelect.CreateVertices(this.mVertices, 0, ref mSize, ref iMargin, ref new Vector2()
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
    if (!this.mIsVisible)
      return;
    this.mTopLeft.Y = this.mPosition.Y;
    this.mBottomRight.Y = this.mPosition.Y + this.mSize.Y * (float) this.mNames.Count * this.mScale;
  }

  internal int GetHitIndex(ref Vector2 iPoint)
  {
    if (!this.mIsVisible)
      return -1;
    float x = this.mPosition.X;
    float num1 = this.mPosition.Y + 16f * this.mScale;
    float num2 = this.mPosition.X + this.mSize.X * this.mScale;
    float num3 = num1 + this.mSize.Y * this.mScale;
    if ((double) iPoint.X < (double) x || (double) iPoint.X > (double) num2)
      return -1;
    for (int hitIndex = 0; hitIndex < this.mNames.Count; ++hitIndex)
    {
      if ((double) iPoint.Y >= (double) num1 && (double) iPoint.Y <= (double) num3)
        return hitIndex;
      num1 += this.mSize.Y * this.mScale;
      num3 += this.mSize.Y * this.mScale;
    }
    return -1;
  }

  public void Show(int iX, int iY)
  {
    this.mPosition.X = (float) iX;
    this.mPosition.Y = (float) iY;
    if (this.mNames.Count == 0)
      return;
    if (!this.mIsVisible)
    {
      this.mSelectedIndex = -1;
      this.UpdateBoundingBox();
    }
    this.mIsVisible = true;
  }

  public void Hide() => this.mIsVisible = false;

  public bool IsVisible => this.mIsVisible;

  public void Update(float iDeltaTime)
  {
    if (this.mIsVisible && this.mEnabled)
      this.mDownAlpha = Math.Min(this.mDownAlpha + iDeltaTime * 4f, 1f);
    else
      this.mDownAlpha = Math.Max(this.mDownAlpha - iDeltaTime * 4f, 0.0f);
  }

  public override void Draw(GUIBasicEffect iEffect) => this.Draw(iEffect, this.mScale);

  public override void Draw(GUIBasicEffect iEffect, float iScale)
  {
    if (!this.mIsVisible)
      return;
    float mDownAlpha = this.mDownAlpha;
    if ((double) mDownAlpha <= 0.0)
      return;
    Vector4 vector4;
    vector4.X = vector4.Y = vector4.Z = 1f;
    vector4.W = mDownAlpha;
    iEffect.Color = vector4;
    Matrix iTransform = new Matrix();
    iTransform.M11 = iScale;
    iTransform.M22 = iScale;
    iTransform.M41 = this.mPosition.X;
    iTransform.M42 = this.mPosition.Y;
    iTransform.M44 = 1f;
    iEffect.Transform = iTransform;
    iEffect.Texture = (Texture) ContextMenu.sTexture;
    iEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
    iEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, 16 /*0x10*/);
    iEffect.GraphicsDevice.Indices = ContextMenu.sIndices;
    iEffect.CommitChanges();
    iEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 16 /*0x10*/, 0, 18);
    Vector2 mPosition = this.mPosition;
    mPosition.X += (this.mSize.X - 16f) * iScale;
    mPosition.Y += 16f * iScale;
    lock (this.mNames)
    {
      for (int index = 0; index < this.mNames.Count; ++index)
      {
        vector4 = index == this.mSelectedIndex ? this.mColorSelected : this.mColorItem;
        vector4.W *= mDownAlpha * mDownAlpha;
        iEffect.Color = vector4;
        iTransform.M11 = this.mTextScales[index] * iScale;
        iTransform.M22 = iScale;
        switch (this.mAlignment)
        {
          case TextAlign.Left:
            iTransform.M41 = mPosition.X - (float) this.mWidth;
            break;
          case TextAlign.Center:
            iTransform.M41 = mPosition.X - (float) (this.mWidth / 2);
            break;
          case TextAlign.Right:
            iTransform.M41 = mPosition.X;
            break;
        }
        iTransform.M42 = mPosition.Y;
        this.mTexts[index].Draw(iEffect, ref iTransform);
        mPosition.Y += this.mSize.Y * iScale;
      }
    }
  }

  public override void LanguageChanged()
  {
    LanguageManager instance = LanguageManager.Instance;
    lock (this.mNames)
    {
      for (int index1 = 0; index1 < this.mNames.Count; ++index1)
      {
        StringBuilder stringBuilder = new StringBuilder();
        int[] mName = this.mNames[index1];
        for (int index2 = 0; index2 < mName.Length; ++index2)
        {
          string str = instance.GetString(mName[index2]);
          stringBuilder.Append(str);
          if (index2 < mName.Length - 1)
            stringBuilder.Append(" - ");
        }
        string iText = stringBuilder.ToString();
        this.mTexts[index1].SetText(iText);
        if (this.mAutoSize)
        {
          this.mWidth = (int) Math.Max(this.mFont.MeasureText(iText, true).X, (float) this.mWidth);
          this.mTextScales[index1] = 1f;
        }
        else
          this.mTextScales[index1] = Math.Min(((float) this.mWidth - 32f) / this.mFont.MeasureText(iText, true).X, 1f);
      }
    }
    this.mSize = new Vector2()
    {
      X = (float) this.mWidth + 32f,
      Y = (float) this.mFont.LineHeight
    };
    this.UpdateVertices();
    this.UpdateBoundingBox();
  }
}
