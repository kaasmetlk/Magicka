// Decompiled with JetBrains decompiler
// Type: Magicka.Achievements.PdxTextBox
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System.Text;

#nullable disable
namespace Magicka.Achievements;

internal class PdxTextBox : PdxUIElement
{
  private bool mMask;
  private bool mActive;
  private Texture2D mTexture;
  private StringBuilder mString;
  private PolygonHead.Text mText;
  private BitmapFont mFont;
  private VertexBuffer mVertices;
  private float mCursorTimer;
  private int mCursorPosition;

  public PdxTextBox(
    Texture2D iTexture,
    Rectangle iOffRectangle,
    Rectangle iOnRectangle,
    Rectangle iCursorRectangle,
    BitmapFont iFont,
    int iMaxLength)
  {
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    this.mString = new StringBuilder(iMaxLength, iMaxLength);
    this.mTexture = iTexture;
    this.mFont = iFont;
    this.mText = new PolygonHead.Text(iMaxLength, this.mFont, TextAlign.Left, false, false);
    Vector4[] vector4Array = new Vector4[12];
    int iIndex = 0;
    PdxWidgetWindow.CreateVertices(vector4Array, ref iIndex, ref iOffRectangle);
    PdxWidgetWindow.CreateVertices(vector4Array, ref iIndex, ref iOnRectangle);
    PdxWidgetWindow.CreateVertices(vector4Array, ref iIndex, ref iCursorRectangle);
    lock (graphicsDevice)
    {
      this.mVertices = new VertexBuffer(graphicsDevice, 16 /*0x10*/ * vector4Array.Length, BufferUsage.WriteOnly);
      this.mVertices.SetData<Vector4>(vector4Array);
    }
    this.mSize.X = (float) iOffRectangle.Width;
    this.mSize.Y = (float) iOffRectangle.Height;
  }

  public override void Draw(GUIBasicEffect iEffect, float iAlpha)
  {
    iEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertices, 0, 16 /*0x10*/);
    iEffect.GraphicsDevice.VertexDeclaration = PdxUIElement.sVertexDeclaration;
    Matrix matrix = new Matrix();
    matrix.M11 = 1f;
    matrix.M22 = 1f;
    matrix.M41 = this.mPosition.X;
    matrix.M42 = this.mPosition.Y;
    matrix.M44 = 1f;
    iEffect.Transform = matrix;
    iEffect.Texture = (Texture) this.mTexture;
    Vector4 vector4 = new Vector4();
    vector4.X = vector4.Y = vector4.Z = 1f;
    vector4.W = iAlpha;
    iEffect.Color = vector4;
    iEffect.CommitChanges();
    if (this.mActive)
      iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 4, 2);
    else
      iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
    matrix.M41 += 5f;
    matrix.M42 += 7f;
    for (int index = 0; index < this.mCursorPosition; ++index)
    {
      Glyph oGlyph;
      this.mFont.GetGlyph(this.mText.Characters[index], out oGlyph);
      matrix.M41 += (float) oGlyph.AdvanceWidth;
      if (index > 0)
        matrix.M41 += (float) this.mFont.CalcKern(this.mText.Characters[index - 1], this.mText.Characters[index]);
    }
    iEffect.Transform = matrix;
    iEffect.CommitChanges();
    if (this.mActive & (double) this.mCursorTimer < 0.5)
      iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 8, 2);
    iEffect.Color = vector4;
    this.mText.Draw(iEffect, this.mPosition.X + 5f, this.mPosition.Y + 4f);
  }

  public void Update(float iDeltaTime)
  {
    this.mCursorTimer = (float) (((double) this.mCursorTimer + (double) iDeltaTime) % 1.0);
  }

  public bool Mask
  {
    get => this.mMask;
    set => this.mMask = value;
  }

  public bool Active
  {
    get => this.mActive;
    set
    {
      if (value & !this.mActive)
        this.mCursorTimer = 0.0f;
      this.mActive = value;
    }
  }

  public void Delete()
  {
    if (this.mCursorPosition < this.mString.Length)
      this.mString.Remove(this.mCursorPosition, 1);
    if (this.mMask)
    {
      for (int index = 0; index < this.mString.Length; ++index)
        this.mText.Characters[index] = '*';
      if (this.mString.Length < this.mString.MaxCapacity)
        this.mText.Characters[this.mString.Length] = char.MinValue;
      this.mText.MarkAsDirty();
    }
    else
      this.mText.SetText(this.mString.ToString());
  }

  public void AppendChar(char iChar)
  {
    if (iChar == '\t' | iChar == '\n' | iChar == '\r')
      return;
    if (iChar == '\b')
    {
      if (this.mCursorPosition <= 0)
        return;
      this.mString.Remove(this.mCursorPosition - 1, 1);
      --this.mCursorPosition;
    }
    else
    {
      if (this.mString.Length >= this.mString.MaxCapacity)
        return;
      this.mString.Insert(this.mCursorPosition, iChar);
      ++this.mCursorPosition;
    }
    if (this.mMask)
    {
      for (int index = 0; index < this.mString.Length; ++index)
        this.mText.Characters[index] = '*';
      if (this.mString.Length < this.mString.MaxCapacity)
        this.mText.Characters[this.mString.Length] = char.MinValue;
      this.mText.MarkAsDirty();
    }
    else
      this.mText.SetText(this.mString.ToString());
  }

  public int Cursor
  {
    get => this.mCursorPosition;
    set
    {
      if (value < 0)
        this.mCursorPosition = 0;
      else if (value > this.mString.Length)
        this.mCursorPosition = this.mString.Length;
      else
        this.mCursorPosition = value;
    }
  }

  public string String => this.mString.ToString();
}
