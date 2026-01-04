// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.MenuImageItem
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead.Effects;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu;

public class MenuImageItem : MenuItem
{
  private Texture2D mTexture;
  private int mVertexStride;
  private VertexBuffer mVertices;
  private VertexDeclaration mVertexDeclaration;
  private Vector2 mSize;
  private int mVertexOffset;

  public MenuImageItem(
    Vector2 iPosition,
    Texture2D iTexture,
    VertexBuffer iVertices,
    VertexDeclaration iDeclaration,
    int iStride,
    float iXSize,
    float iYSize)
    : this(iPosition, iTexture, iVertices, iDeclaration, 0.0f, 0, iStride, iXSize, iYSize)
  {
  }

  public MenuImageItem(
    Vector2 iPosition,
    Texture2D iTexture,
    VertexBuffer iVertices,
    VertexDeclaration iDeclaration,
    float iRotation,
    int iOffset,
    int iStride,
    float iXSize,
    float iYSize)
  {
    this.mTexture = iTexture;
    this.mPosition = iPosition;
    this.mVertexOffset = iOffset;
    this.mVertices = iVertices;
    this.mVertexStride = iStride;
    this.mVertexDeclaration = iDeclaration;
    this.mSize.X = iXSize;
    this.mSize.Y = iYSize;
    this.UpdateBoundingBox();
    this.mTransform = Matrix.CreateRotationZ(iRotation);
    this.mTransform.M41 = this.mPosition.X;
    this.mTransform.M42 = this.mPosition.Y;
  }

  protected override void UpdateBoundingBox()
  {
    this.mTopLeft.X = this.mPosition.X - this.mSize.X * 0.5f * this.mScale;
    this.mTopLeft.Y = this.mPosition.Y - this.mSize.Y * 0.5f * this.mScale;
    this.mBottomRight.X = this.mPosition.X + this.mSize.X * 0.5f * this.mScale;
    this.mBottomRight.Y = this.mPosition.Y + this.mSize.Y * 0.5f * this.mScale;
  }

  public override void Draw(GUIBasicEffect iEffect) => this.Draw(iEffect, this.mScale);

  public override void Draw(GUIBasicEffect iEffect, float iScale)
  {
    if (!this.mEnabled)
      return;
    iEffect.Color = !this.mSelected ? Vector4.One : new Vector4(1.5f, 1.5f, 1.5f, 1f);
    iEffect.Saturation = this.mSelected ? 1.3f : 1f;
    iEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertices, 0, this.mVertexStride);
    iEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
    iEffect.GraphicsDevice.RenderState.CullMode = CullMode.None;
    Matrix mTransform = this.mTransform;
    mTransform.M11 *= iScale;
    mTransform.M12 *= iScale;
    mTransform.M13 *= iScale;
    mTransform.M21 *= iScale;
    mTransform.M22 *= iScale;
    mTransform.M23 *= iScale;
    iEffect.Transform = mTransform;
    iEffect.Texture = (Texture) this.mTexture;
    iEffect.TextureEnabled = true;
    iEffect.VertexColorEnabled = false;
    iEffect.CommitChanges();
    iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, this.mVertexOffset, 2);
    iEffect.GraphicsDevice.Vertices[0].SetSource((VertexBuffer) null, 0, 0);
    iEffect.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
  }

  public override void LanguageChanged()
  {
  }
}
