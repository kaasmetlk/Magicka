// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.Effects.ForceFieldMaterial
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

#nullable disable
namespace Magicka.Graphics.Effects;

internal struct ForceFieldMaterial : IMaterial<ForceFieldEffect>
{
  public bool VertexColorEnabled;
  public Vector3 Color;
  public float Width;
  public float AlphaPower;
  public float AlphaFalloffPower;
  public float MaxRadius;
  public float RippleDistortion;
  public float MapDistortion;
  public Texture2D DisplacementMap;

  public void FetchFromEffect(ForceFieldEffect iEffect)
  {
    this.VertexColorEnabled = iEffect.VertexColorEnabled;
    this.Color = iEffect.Color;
    this.Width = iEffect.Width;
    this.AlphaPower = iEffect.AlphaPower;
    this.AlphaFalloffPower = iEffect.AlphaFalloffPower;
    this.MaxRadius = iEffect.MaxRadius;
    this.RippleDistortion = iEffect.RippleDistortion;
    this.MapDistortion = iEffect.MapDistortion;
    this.DisplacementMap = iEffect.DisplacementMap;
  }

  public void AssignOpacityToEffect(ForceFieldEffect iEffect)
  {
  }

  public void AssignToEffect(ForceFieldEffect iEffect)
  {
    iEffect.VertexColorEnabled = this.VertexColorEnabled;
    iEffect.Color = this.Color;
    iEffect.Width = this.Width;
    iEffect.AlphaPower = this.AlphaPower;
    iEffect.AlphaFalloffPower = this.AlphaFalloffPower;
    iEffect.MaxRadius = this.MaxRadius;
    iEffect.RippleDistortion = this.RippleDistortion;
    iEffect.MapDistortion = this.MapDistortion;
    iEffect.DisplacementMap = this.DisplacementMap;
  }
}
