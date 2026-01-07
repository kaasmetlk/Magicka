// Decompiled with JetBrains decompiler
// Type: PolygonHead.Effects.RenderDeferredLiquidMaterial
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead.Effects;

public struct RenderDeferredLiquidMaterial : IMaterial<RenderDeferredLiquidEffect>
{
  public Matrix WorldTransform;
  public TextureCube ReflectionMap;
  public float WaveHeight;
  public Vector2 WaveSpeed0;
  public Vector2 WaveSpeed1;
  public float WaterReflectiveness;
  public Vector3 BottomColor;
  public Vector3 DeepBottomColor;
  public float WaterEmissiveAmount;
  public float WaterSpecAmount;
  public float WaterSpecPower;
  public Texture2D BottomTexture;
  public Texture2D WaterNormalMap;
  public float IceReflectiveness;
  public Vector3 IceColor;
  public float IceEmissiveAmount;
  public float IceSpecAmount;
  public float IceSpecPower;
  public Texture2D IceDiffuseMap;
  public Texture2D IceNormalMap;

  public void FetchFromEffect(RenderDeferredLiquidEffect iEffect)
  {
    this.WorldTransform = Matrix.Identity;
    this.ReflectionMap = iEffect.ReflectionMap;
    this.WaveHeight = iEffect.WaveHeight;
    this.WaveSpeed0 = iEffect.WaveSpeed0;
    this.WaveSpeed1 = iEffect.WaveSpeed1;
    this.WaterReflectiveness = iEffect.WaterReflectiveness;
    this.BottomColor = iEffect.BottomColor;
    this.DeepBottomColor = iEffect.DeepBottomColor;
    this.WaterEmissiveAmount = iEffect.WaterEmissiveAmount;
    this.WaterSpecAmount = iEffect.WaterSpecAmount;
    this.WaterSpecPower = iEffect.WaterSpecPower;
    this.BottomTexture = iEffect.BottomTexture;
    this.WaterNormalMap = iEffect.WaterNormalMap;
    this.IceReflectiveness = iEffect.IceReflectiveness;
    this.IceColor = iEffect.IceColor;
    this.IceEmissiveAmount = iEffect.IceEmissiveAmount;
    this.IceSpecAmount = iEffect.IceSpecAmount;
    this.IceSpecPower = iEffect.IceSpecPower;
    this.IceDiffuseMap = iEffect.IceDiffuseMap;
    this.IceNormalMap = iEffect.IceNormalMap;
  }

  public void AssignOpacityToEffect(RenderDeferredLiquidEffect iEffect)
  {
    iEffect.World = this.WorldTransform;
  }

  public void AssignToEffect(RenderDeferredLiquidEffect iEffect)
  {
    iEffect.World = this.WorldTransform;
    iEffect.ReflectionMap = this.ReflectionMap;
    iEffect.WaveHeight = this.WaveHeight;
    iEffect.WaveSpeed0 = this.WaveSpeed0;
    iEffect.WaveSpeed1 = this.WaveSpeed1;
    iEffect.WaterReflectiveness = this.WaterReflectiveness;
    iEffect.BottomColor = this.BottomColor;
    iEffect.DeepBottomColor = this.DeepBottomColor;
    iEffect.WaterEmissiveAmount = this.WaterEmissiveAmount;
    iEffect.WaterSpecAmount = this.WaterSpecAmount;
    iEffect.WaterSpecPower = this.WaterSpecPower;
    iEffect.BottomTexture = this.BottomTexture;
    iEffect.WaterNormalMap = this.WaterNormalMap;
    iEffect.IceReflectiveness = this.IceReflectiveness;
    iEffect.IceColor = this.IceColor;
    iEffect.IceEmissiveAmount = this.IceEmissiveAmount;
    iEffect.IceSpecAmount = this.IceSpecAmount;
    iEffect.IceSpecPower = this.IceSpecPower;
    iEffect.IceDiffuseMap = this.IceDiffuseMap;
    iEffect.IceNormalMap = this.IceNormalMap;
  }
}
