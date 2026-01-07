// Decompiled with JetBrains decompiler
// Type: PolygonHead.Text
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead.Effects;
using System;

#nullable disable
namespace PolygonHead;

public class Text : IDisposable
{
  public static readonly Vector2 DEFAULT_SHADOW_OFFSET = new Vector2(2f, 2f);
  public static readonly char[] COLORCHARS = new char[2]
  {
    'c',
    '='
  };
  public static readonly char[] RESTORECOLORCHARS = new char[2]
  {
    '/',
    'c'
  };
  protected char[] mText;
  protected Character[] mCharacters;
  protected BitmapFont mFont;
  protected int mEndIndex;
  protected VertexBuffer mVertices;
  protected VertexDeclaration mVertexDeclaration;
  protected IndexBuffer mIndices;
  protected int mVertexStride;
  protected int mPrimitiveCount;
  protected Vector4 mDefaultColor;
  protected bool mUseKerning = true;
  protected bool mDirty;
  protected TextAlign mAlign;
  protected bool mUseFormatting;
  protected bool mDrawShadows;
  protected Vector2 mShadowOffset;
  protected float mShadowAlpha;
  protected Vector2 mPosition = Vector2.Zero;

  public int EndIndex
  {
    get => this.mEndIndex;
    protected set => this.mEndIndex = value;
  }

  public Vector2 Position
  {
    get => this.mPosition;
    set => this.mPosition = value;
  }

  public Text(int iLength, BitmapFont iFont, TextAlign iAlign, bool iDynamic)
    : this(iLength, iFont, iAlign, iDynamic, true)
  {
  }

  public Text(
    int iLength,
    BitmapFont iFont,
    TextAlign iAlign,
    bool iDynamic,
    bool iUseFormatting)
  {
    this.mUseFormatting = iUseFormatting;
    this.mAlign = iAlign;
    this.mText = new char[iLength];
    this.mCharacters = new Character[iLength];
    this.mFont = iFont;
    this.mVertices = !iDynamic ? new VertexBuffer(iFont.Texture.GraphicsDevice, 128 /*0x80*/ * iLength, BufferUsage.WriteOnly) : (VertexBuffer) new DynamicVertexBuffer(iFont.Texture.GraphicsDevice, 128 /*0x80*/ * iLength, BufferUsage.WriteOnly);
    ushort[] data = new ushort[6 * iLength];
    for (int index = 0; index < iLength; ++index)
    {
      data[index * 6] = (ushort) (index * 4);
      data[index * 6 + 1] = (ushort) (index * 4 + 1);
      data[index * 6 + 2] = (ushort) (index * 4 + 2);
      data[index * 6 + 3] = (ushort) (index * 4 + 2);
      data[index * 6 + 4] = (ushort) (index * 4 + 3);
      data[index * 6 + 5] = (ushort) (index * 4);
    }
    this.mIndices = new IndexBuffer(iFont.Texture.GraphicsDevice, 6 * iLength * 2, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
    this.mIndices.SetData<ushort>(data);
    this.mVertexStride = 32 /*0x20*/;
    this.mVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(CharacterVertex.VertexElements);
    this.mDefaultColor = new Vector4();
    this.mDefaultColor.X = this.mDefaultColor.Y = this.mDefaultColor.Z = this.mDefaultColor.W = 1f;
    this.mEndIndex = 0;
  }

  public char[] Characters => this.mText;

  public Vector4 DefaultColor
  {
    get => this.mDefaultColor;
    set
    {
      this.mDefaultColor = value;
      this.MarkAsDirty();
    }
  }

  public bool UseKerning
  {
    get => this.mUseKerning;
    set
    {
      this.mUseKerning = value;
      this.MarkAsDirty();
    }
  }

  public virtual void SetText(string iText)
  {
    if (iText != null)
    {
      for (int index = 0; index < iText.Length; ++index)
        this.mText[index] = iText[index];
      if (iText.Length < this.mText.Length)
      {
        this.mText[iText.Length] = char.MinValue;
        this.mEndIndex = iText.Length;
      }
      else
        this.mEndIndex = this.mText.Length;
    }
    else
    {
      this.mText[0] = char.MinValue;
      this.mEndIndex = 0;
    }
    this.MarkAsDirty();
  }

  public void Clear()
  {
    this.mText[0] = char.MinValue;
    this.mEndIndex = 0;
    this.MarkAsDirty();
  }

  public void Append(char[] iChars)
  {
    for (int index = 0; index < iChars.Length; ++index)
      this.mText[this.mEndIndex + index] = iChars[index];
    this.mEndIndex += iChars.Length;
    this.mText[this.mEndIndex] = char.MinValue;
    this.MarkAsDirty();
  }

  public void Append(string iChars)
  {
    for (int index = 0; index < iChars.Length; ++index)
      this.mText[this.mEndIndex + index] = iChars[index];
    this.mEndIndex += iChars.Length;
    this.mText[this.mEndIndex] = char.MinValue;
    this.MarkAsDirty();
  }

  public void Append(int iValue)
  {
    int num = Math.Max((int) Math.Log10((double) Math.Abs(iValue) + 0.5), 0) + 1;
    if (iValue < 0)
    {
      this.mText[this.mEndIndex] = '-';
      ++this.mEndIndex;
    }
    for (int index = this.mEndIndex + num - 1; index >= this.mEndIndex; --index)
    {
      this.mText[index] = (char) (48 /*0x30*/ + iValue % 10);
      iValue /= 10;
    }
    if (iValue < 0)
      ++num;
    this.mEndIndex += num;
    this.mText[this.mEndIndex] = char.MinValue;
    this.MarkAsDirty();
  }

  public bool IsDirty => this.mDirty;

  public void MarkAsDirty() => this.mDirty = true;

  protected virtual void UpdateVertices()
  {
    Vector2 iOffset = new Vector2();
    Vector4 mDefaultColor = this.mDefaultColor;
    bool flag = false;
    int index1 = 0;
    int index2 = 0;
    int elementCount = 0;
    char iLeft = char.MinValue;
    int num1 = 0;
    for (int iIndex = 0; iIndex < this.mText.Length; ++iIndex)
    {
      char ch = this.mText[iIndex];
      if (flag)
      {
        switch (ch)
        {
          case '[':
            Character oChar;
            this.mFont.GetCharacter(ch, out oChar);
            Glyph oGlyph;
            this.mFont.GetGlyph(ch, out oGlyph);
            if (!oGlyph.ForceWhite)
              oChar.Color = mDefaultColor;
            if (this.mUseKerning)
              iOffset.X += (float) this.mFont.CalcKern(iLeft, ch);
            iOffset.X += (float) this.mFont.Spacing;
            oChar.ApplyOffset(ref iOffset);
            iOffset.X += (float) oGlyph.AdvanceWidth;
            this.mCharacters[elementCount] = oChar;
            ++elementCount;
            ++num1;
            iLeft = ch;
            flag = false;
            index1 = 0;
            break;
          case ']':
            flag = false;
            index1 = 0;
            break;
        }
        if ((int) ch == (int) Text.COLORCHARS[index1])
        {
          ++index1;
          if (index1 == Text.COLORCHARS.Length)
          {
            index1 = 0;
            ++iIndex;
            mDefaultColor.X = Text.ParseFloat(this.mText, ref iIndex);
            ++iIndex;
            mDefaultColor.Y = Text.ParseFloat(this.mText, ref iIndex);
            ++iIndex;
            mDefaultColor.Z = Text.ParseFloat(this.mText, ref iIndex);
            if (this.mText[iIndex] == ',')
            {
              ++iIndex;
              mDefaultColor.W = Text.ParseFloat(this.mText, ref iIndex);
            }
            else
              mDefaultColor.W = 1f;
            --iIndex;
          }
        }
        if ((int) ch == (int) Text.RESTORECOLORCHARS[index2])
        {
          ++index2;
          if (index2 == Text.RESTORECOLORCHARS.Length)
          {
            index2 = 0;
            mDefaultColor = this.mDefaultColor;
          }
        }
      }
      else if (ch != char.MinValue)
      {
        if (ch == '[' & this.mUseFormatting)
        {
          flag = true;
          index1 = 0;
        }
        else if (ch == '\n')
        {
          if (this.mAlign == TextAlign.Center)
          {
            float num2 = (float) Math.Floor((double) iOffset.X * -0.5);
            for (int index3 = elementCount - 1; index3 >= elementCount - num1; --index3)
            {
              this.mCharacters[index3].VertexA.Position.X += num2;
              this.mCharacters[index3].VertexB.Position.X += num2;
              this.mCharacters[index3].VertexC.Position.X += num2;
              this.mCharacters[index3].VertexD.Position.X += num2;
            }
          }
          else if (this.mAlign == TextAlign.Right)
          {
            float num3 = (float) Math.Floor((double) iOffset.X * -1.0);
            for (int index4 = elementCount - 1; index4 >= elementCount - num1; --index4)
            {
              this.mCharacters[index4].VertexA.Position.X += num3;
              this.mCharacters[index4].VertexB.Position.X += num3;
              this.mCharacters[index4].VertexC.Position.X += num3;
              this.mCharacters[index4].VertexD.Position.X += num3;
            }
          }
          num1 = 0;
          iOffset.X = 0.0f;
          iOffset.Y += (float) this.mFont.LineHeight;
        }
        else
        {
          Character oChar;
          this.mFont.GetCharacter(ch, out oChar);
          Glyph oGlyph;
          this.mFont.GetGlyph(ch, out oGlyph);
          if (!oGlyph.ForceWhite)
            oChar.Color = mDefaultColor;
          if (this.mUseKerning)
            iOffset.X += (float) this.mFont.CalcKern(iLeft, ch);
          iOffset.X += (float) this.mFont.Spacing;
          oChar.ApplyOffset(ref iOffset);
          iOffset.X += (float) oGlyph.AdvanceWidth;
          this.mCharacters[elementCount] = oChar;
          ++elementCount;
          ++num1;
          iLeft = ch;
        }
      }
      else
        break;
    }
    if (this.mAlign == TextAlign.Center)
    {
      float num4 = (float) Math.Floor((double) iOffset.X * -0.5);
      for (int index5 = elementCount - 1; index5 >= elementCount - num1; --index5)
      {
        this.mCharacters[index5].VertexA.Position.X += num4;
        this.mCharacters[index5].VertexB.Position.X += num4;
        this.mCharacters[index5].VertexC.Position.X += num4;
        this.mCharacters[index5].VertexD.Position.X += num4;
      }
    }
    else if (this.mAlign == TextAlign.Right)
    {
      float num5 = (float) Math.Floor((double) iOffset.X * -1.0);
      for (int index6 = elementCount - 1; index6 >= elementCount - num1; --index6)
      {
        this.mCharacters[index6].VertexA.Position.X += num5;
        this.mCharacters[index6].VertexB.Position.X += num5;
        this.mCharacters[index6].VertexC.Position.X += num5;
        this.mCharacters[index6].VertexD.Position.X += num5;
      }
    }
    if (elementCount > 0)
      this.mVertices.SetData<Character>(this.mCharacters, 0, elementCount);
    this.mPrimitiveCount = elementCount * 2;
    this.mDirty = false;
  }

  public static float ParseFloat(char[] iCharacters, ref int iIndex)
  {
    bool flag = true;
    float num1 = 0.0f;
    float num2 = 1f;
    if (iCharacters[iIndex] == '-')
    {
      flag = false;
      ++iIndex;
    }
    else if (iCharacters[iIndex] == '+')
    {
      flag = true;
      ++iIndex;
    }
    while (iIndex < iCharacters.Length)
    {
      char iCharacter = iCharacters[iIndex];
      if (iCharacter == '.')
        ++iIndex;
      if (char.IsDigit(iCharacter))
      {
        num1 = num1 * 10f + (float) ((int) iCharacter - 48 /*0x30*/);
        ++iIndex;
      }
      else
        break;
    }
    while (iIndex < iCharacters.Length)
    {
      char iCharacter = iCharacters[iIndex];
      if (char.IsDigit(iCharacter))
      {
        num2 *= 0.1f;
        int num3 = (int) iCharacter - 48 /*0x30*/;
        num1 += (float) num3 * num2;
        ++iIndex;
      }
      else
        break;
    }
    if (!flag)
      num1 = -num1;
    return num1;
  }

  public VertexBuffer Vertices => this.mVertices;

  public IndexBuffer Indices => this.mIndices;

  public VertexDeclaration VertexDeclaration => this.mVertexDeclaration;

  public int VertexStride => this.mVertexStride;

  public int PrimitiveCount => this.mPrimitiveCount;

  public BitmapFont Font
  {
    get => this.mFont;
    set
    {
      this.mFont = value;
      this.MarkAsDirty();
    }
  }

  public static int ToText(int iValue, char[] iArray, int iStart)
  {
    int num = Math.Max((int) Math.Log10((double) Math.Abs(iValue) + 0.5), 0) + 1;
    if (iValue < 0)
      iArray[iStart++] = '-';
    for (int index = iStart + num - 1; index >= iStart; --index)
    {
      iArray[index] = (char) (48 /*0x30*/ + iValue % 10);
      iValue /= 10;
    }
    return iValue < 0 ? num + 1 : num;
  }

  public virtual void Draw(GUIBasicEffect iEffect)
  {
    Vector4 color = iEffect.Color;
    iEffect.Color = this.mDefaultColor;
    this.Draw(iEffect, this.mPosition.X, this.mPosition.Y);
    iEffect.Color = color;
  }

  public virtual void Draw(GUIBasicEffect iEffect, float iScale)
  {
    Vector4 color = iEffect.Color;
    iEffect.Color = this.mDefaultColor;
    this.Draw(iEffect, this.mPosition.X, this.mPosition.Y, iScale);
    iEffect.Color = color;
  }

  public virtual void Draw(GUIBasicEffect iEffect, float iX, float iY)
  {
    this.Draw(iEffect, ref new Matrix()
    {
      M11 = 1f,
      M22 = 1f,
      M33 = 1f,
      M44 = 1f,
      M41 = iX,
      M42 = iY
    });
  }

  public virtual void Draw(GUIBasicEffect iEffect, float iX, float iY, float iScale)
  {
    this.Draw(iEffect, ref new Matrix()
    {
      M11 = iScale,
      M22 = iScale,
      M33 = 1f,
      M44 = 1f,
      M41 = iX,
      M42 = iY
    });
  }

  public virtual void Draw(
    GUIBasicEffect iEffect,
    float iX,
    float iY,
    float iScale,
    float iRotation)
  {
    Matrix iTransform = new Matrix()
    {
      M11 = (float) Math.Cos((double) iRotation) * iScale,
      M12 = (float) Math.Sin((double) iRotation) * iScale
    };
    iTransform.M21 = -iTransform.M12 * iScale;
    iTransform.M22 = iTransform.M11 * iScale;
    iTransform.M33 = iScale;
    iTransform.M44 = 1f;
    iTransform.M41 = iX;
    iTransform.M42 = iY;
    this.Draw(iEffect, ref iTransform);
  }

  public virtual void Draw(GUIBasicEffect iEffect, ref Matrix iTransform)
  {
    if (this.mDirty)
      this.UpdateVertices();
    if (this.mPrimitiveCount == 0)
      return;
    iEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertices, 0, this.mVertexStride);
    iEffect.GraphicsDevice.Indices = this.mIndices;
    iEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
    iEffect.TextureOffset = Vector2.Zero;
    iEffect.TextureScale = Vector2.One;
    iEffect.Texture = (Texture) this.mFont.Texture;
    iEffect.TextureEnabled = true;
    if (this.mDrawShadows)
    {
      Vector4 color = iEffect.Color;
      iEffect.Color = new Vector4(0.0f, 0.0f, 0.0f, this.mShadowAlpha * color.W);
      Matrix matrix = iTransform;
      matrix.M41 += this.mShadowOffset.X;
      matrix.M42 += this.mShadowOffset.Y;
      iEffect.Transform = matrix;
      iEffect.CommitChanges();
      iEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.mPrimitiveCount * 2, 0, this.mPrimitiveCount);
      iEffect.Color = color;
    }
    iEffect.Transform = iTransform;
    iEffect.CommitChanges();
    iEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.mPrimitiveCount * 2, 0, this.mPrimitiveCount);
    iEffect.GraphicsDevice.Vertices[0].SetSource((VertexBuffer) null, 0, 0);
  }

  public bool DrawShadows
  {
    get => this.mDrawShadows;
    set => this.mDrawShadows = value;
  }

  public Vector2 ShadowsOffset
  {
    get => this.mShadowOffset;
    set => this.mShadowOffset = value;
  }

  public float ShadowAlpha
  {
    get => this.mShadowAlpha;
    set => this.mShadowAlpha = value;
  }

  public TextAlign Alignment
  {
    get => this.mAlign;
    set
    {
      this.mAlign = value;
      this.MarkAsDirty();
    }
  }

  public void Dispose()
  {
    this.mIndices.Dispose();
    this.mText = (char[]) null;
    this.mCharacters = (Character[]) null;
  }
}
