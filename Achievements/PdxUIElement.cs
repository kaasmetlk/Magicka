// Decompiled with JetBrains decompiler
// Type: Magicka.Achievements.PdxUIElement
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead.Effects;

#nullable disable
namespace Magicka.Achievements;

internal abstract class PdxUIElement
{
  protected Vector2 mPosition;
  protected Vector2 mSize;
  protected static VertexDeclaration sVertexDeclaration;

  public PdxUIElement()
  {
    if (PdxUIElement.sVertexDeclaration != null)
      return;
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    lock (graphicsDevice)
      PdxUIElement.sVertexDeclaration = new VertexDeclaration(graphicsDevice, new VertexElement[2]
      {
        new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
        new VertexElement((short) 0, (short) 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 0)
      });
  }

  public abstract void Draw(GUIBasicEffect iEffect, float iAlpha);

  public virtual void OnLanguageChanged()
  {
  }

  public bool InsideBounds(ref Vector2 iPoint)
  {
    return (double) iPoint.X >= (double) this.mPosition.X & (double) iPoint.Y >= (double) this.mPosition.Y & (double) iPoint.X <= (double) this.mPosition.X + (double) this.mSize.X & (double) iPoint.Y <= (double) this.mPosition.Y + (double) this.mSize.Y;
  }

  public Vector2 Position
  {
    get => this.mPosition;
    set => this.mPosition = value;
  }
}
