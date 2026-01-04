// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.MenuItemSeparator
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead.Effects;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu;

public class MenuItemSeparator : MenuItem
{
  private static Texture2D sTexture;
  private static VertexBuffer sVertices;
  private static VertexDeclaration sVertexDeclaration;

  public MenuItemSeparator(Vector2 iPosition)
  {
    this.mPosition = iPosition;
    this.EnsureInitialized();
  }

  private void EnsureInitialized()
  {
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    if (MenuItemSeparator.sTexture == null || MenuItemSeparator.sTexture.IsDisposed)
    {
      lock (graphicsDevice)
        MenuItemSeparator.sTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/Pages");
    }
    if (MenuItemSeparator.sVertices == null || MenuItemSeparator.sVertices.IsDisposed)
    {
      Vector4[] data = new Vector4[4];
      Vector2 vector2 = new Vector2();
      vector2.X = 1f / (float) MenuItemSeparator.sTexture.Width;
      vector2.Y = 1f / (float) MenuItemSeparator.sTexture.Height;
      data[0].X = -304f;
      data[0].Y = -24f;
      data[0].Z = 448f * vector2.X;
      data[0].W = 976f * vector2.Y;
      data[1].X = 304f;
      data[1].Y = -24f;
      data[1].Z = 1056f * vector2.X;
      data[1].W = 976f * vector2.Y;
      data[2].X = 304f;
      data[2].Y = 24f;
      data[2].Z = 1056f * vector2.X;
      data[2].W = 1024f * vector2.Y;
      data[3].X = -304f;
      data[3].Y = 24f;
      data[3].Z = 448f * vector2.X;
      data[3].W = 1024f * vector2.Y;
      lock (graphicsDevice)
      {
        MenuItemSeparator.sVertices = new VertexBuffer(graphicsDevice, 16 /*0x10*/ * data.Length, BufferUsage.WriteOnly);
        MenuItemSeparator.sVertices.SetData<Vector4>(data);
      }
    }
    if (MenuItemSeparator.sVertexDeclaration != null && !MenuItemSeparator.sVertexDeclaration.IsDisposed)
      return;
    lock (graphicsDevice)
      MenuItemSeparator.sVertexDeclaration = new VertexDeclaration(graphicsDevice, new VertexElement[2]
      {
        new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
        new VertexElement((short) 0, (short) 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 0)
      });
  }

  public override bool Enabled
  {
    get => false;
    set
    {
    }
  }

  protected override void UpdateBoundingBox()
  {
    this.mTopLeft.X = this.mPosition.X - 304f;
    this.mTopLeft.Y = this.mPosition.Y - 24f;
    this.mBottomRight.X = this.mPosition.X + 304f;
    this.mBottomRight.Y = this.mPosition.Y + 24f;
  }

  public override void Draw(GUIBasicEffect iEffect) => this.Draw(iEffect, this.mScale);

  public override void Draw(GUIBasicEffect iEffect, float iScale)
  {
    iEffect.Texture = (Texture) MenuItemSeparator.sTexture;
    iEffect.TextureEnabled = true;
    Matrix matrix = new Matrix();
    matrix.M11 = matrix.M22 = 1f;
    matrix.M44 = 1f;
    matrix.M41 = this.mPosition.X;
    matrix.M42 = this.mPosition.Y;
    iEffect.Transform = matrix;
    Vector4 vector4 = new Vector4();
    vector4.X = vector4.Y = vector4.Z = 1f;
    vector4.W = 0.75f;
    iEffect.Color = vector4;
    iEffect.GraphicsDevice.Vertices[0].SetSource(MenuItemSeparator.sVertices, 0, 16 /*0x10*/);
    iEffect.GraphicsDevice.VertexDeclaration = MenuItemSeparator.sVertexDeclaration;
    iEffect.CommitChanges();
    iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
  }

  public override void LanguageChanged()
  {
  }
}
