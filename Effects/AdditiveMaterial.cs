// Decompiled with JetBrains decompiler
// Type: PolygonHead.Effects.AdditiveMaterial
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead.Effects;

public struct AdditiveMaterial : IMaterial<AdditiveEffect>
{
  public Matrix WorldTransform;
  public Vector4 ColorTint;
  public Texture2D Texture;
  public bool TextureEnabled;
  public bool VertexColorEnabled;

  public void AssignOpacityToEffect(AdditiveEffect iEffect)
  {
    iEffect.World = this.WorldTransform;
    iEffect.ColorTint = this.ColorTint;
    iEffect.Texture = this.Texture;
    iEffect.TextureEnabled = this.TextureEnabled;
    iEffect.VertexColorEnabled = this.VertexColorEnabled;
  }

  public void AssignToEffect(AdditiveEffect iEffect)
  {
    iEffect.World = this.WorldTransform;
    iEffect.ColorTint = this.ColorTint;
    iEffect.Texture = this.Texture;
    iEffect.TextureEnabled = this.TextureEnabled;
    iEffect.TextureOffset = new Vector2();
    iEffect.TextureScale = new Vector2(1f);
    iEffect.VertexColorEnabled = this.VertexColorEnabled;
  }

  public void FetchFromEffect(AdditiveEffect iEffect)
  {
    this.WorldTransform = Matrix.Identity;
    this.ColorTint = iEffect.ColorTint;
    this.Texture = iEffect.Texture;
    this.TextureEnabled = iEffect.TextureEnabled;
    this.VertexColorEnabled = iEffect.VertexColorEnabled;
  }
}
