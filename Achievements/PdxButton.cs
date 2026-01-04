// Decompiled with JetBrains decompiler
// Type: Magicka.Achievements.PdxButton
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;

#nullable disable
namespace Magicka.Achievements;

internal class PdxButton : PdxUIElement
{
  private const float sUpColor = 0.5019608f;
  private const float sOverColor = 0.627451f;
  private Texture2D mTexture;
  private int mTextId;
  private Text mText;
  private BitmapFont mFont;
  private Vector2 mTextSize;
  private VertexBuffer mVertices;
  private PdxButton.ButtonState mState;

  public PdxButton(
    Texture2D iTexture,
    Rectangle iOffRectangle,
    Rectangle iOnRectangle,
    BitmapFont iFont,
    int iText)
  {
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    this.mTexture = iTexture;
    this.mTextId = iText;
    this.mFont = iFont;
    if (iFont != null)
      this.mText = new Text(32 /*0x20*/, this.mFont, TextAlign.Center, false);
    Vector4[] vector4Array = new Vector4[8];
    int iIndex = 0;
    PdxWidgetWindow.CreateVertices(vector4Array, ref iIndex, ref iOffRectangle);
    PdxWidgetWindow.CreateVertices(vector4Array, ref iIndex, ref iOnRectangle);
    lock (graphicsDevice)
    {
      this.mVertices = new VertexBuffer(graphicsDevice, 16 /*0x10*/ * vector4Array.Length, BufferUsage.WriteOnly);
      this.mVertices.SetData<Vector4>(vector4Array);
    }
    this.mSize.X = (float) iOffRectangle.Width;
    this.mSize.Y = (float) iOffRectangle.Height;
    this.OnLanguageChanged();
  }

  public PdxButton.ButtonState State
  {
    get => this.mState;
    set => this.mState = value;
  }

  public override void Draw(GUIBasicEffect iEffect, float iAlpha)
  {
    iEffect.Transform = new Matrix()
    {
      M11 = 1f,
      M22 = 1f,
      M41 = this.mPosition.X,
      M42 = this.mPosition.Y,
      M44 = 1f
    };
    iEffect.Texture = (Texture) this.mTexture;
    Vector4 vector4 = new Vector4();
    vector4.X = vector4.Y = vector4.Z = 1f;
    vector4.W = iAlpha;
    iEffect.Color = vector4;
    iEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertices, 0, 16 /*0x10*/);
    iEffect.GraphicsDevice.VertexDeclaration = PdxUIElement.sVertexDeclaration;
    iEffect.CommitChanges();
    Vector2 vector2 = new Vector2();
    vector2.X = (float) ((double) this.mPosition.X + Math.Floor((double) this.mSize.X * 0.5) - 1.0);
    vector2.Y = this.mPosition.Y + (float) Math.Floor(((double) this.mSize.Y - (double) this.mTextSize.Y) * 0.5);
    if ((this.mState & (PdxButton.ButtonState.Down | PdxButton.ButtonState.Over)) == (PdxButton.ButtonState.Down | PdxButton.ButtonState.Over))
    {
      iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 4, 2);
      ++vector2.X;
      ++vector2.Y;
    }
    else
      iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
    if (this.mText == null)
      return;
    vector4.X = (this.mState & PdxButton.ButtonState.Over) != PdxButton.ButtonState.Over ? (vector4.Y = vector4.Z = 0.5019608f) : (vector4.Y = vector4.Z = 0.627451f);
    iEffect.Color = vector4;
    this.mText.Draw(iEffect, vector2.X, vector2.Y);
  }

  public override void OnLanguageChanged()
  {
    base.OnLanguageChanged();
    if (this.mFont == null)
      return;
    this.mText.SetText(AchievementsManager.Instance.GetTranslation(this.mTextId));
    this.mTextSize = this.mFont.MeasureText(this.mText.Characters, true);
  }

  public enum ButtonState
  {
    Up,
    Down,
    Over,
  }
}
