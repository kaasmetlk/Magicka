// Decompiled with JetBrains decompiler
// Type: PolygonHead.Effects.RenderDeferredMaterial
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead.Effects;

public struct RenderDeferredMaterial : IMaterial<RenderDeferredEffect>
{
  public Matrix WorldTransform;
  public float Alpha;
  public float Sharpness;
  public bool VertexColorEnabled;
  public bool UseMaterialTextureForReflectiveness;
  public TextureCube ReflectionMap;
  public bool DiffuseTexture0AlphaDisabled;
  public bool AlphaMask0Enabled;
  public Vector3 DiffuseColor0;
  public float SpecAmount0;
  public float SpecPower0;
  public float EmissiveAmount0;
  public float NormalPower0;
  public float Reflectiveness0;
  public Texture2D DiffuseTexture0;
  public Texture2D MaterialTexture0;
  public Texture2D NormalTexture0;
  public bool DiffuseTexture1AlphaDisabled;
  public bool AlphaMask1Enabled;
  public Vector3 DiffuseColor1;
  public float SpecAmount1;
  public float SpecPower1;
  public float EmissiveAmount1;
  public float NormalPower1;
  public float Reflectiveness1;
  public Texture2D DiffuseTexture1;
  public Texture2D MaterialTexture1;
  public Texture2D NormalTexture1;

  public void FetchFromEffect(RenderDeferredEffect iEffect)
  {
    this.WorldTransform = Matrix.Identity;
    this.Alpha = iEffect.Alpha;
    this.Sharpness = iEffect.Sharpness;
    this.VertexColorEnabled = iEffect.VertexColorEnabled;
    this.UseMaterialTextureForReflectiveness = iEffect.UseMaterialTextureForReflectiveness;
    this.ReflectionMap = iEffect.ReflectionMap;
    this.DiffuseTexture0AlphaDisabled = iEffect.DiffuseTexture0AlphaDisabled;
    this.AlphaMask0Enabled = iEffect.AlphaMask0Enabled;
    this.DiffuseColor0 = iEffect.DiffuseColor0;
    this.SpecAmount0 = iEffect.SpecAmount0;
    this.SpecPower0 = iEffect.SpecPower0;
    this.EmissiveAmount0 = iEffect.EmissiveAmount0;
    this.NormalPower0 = iEffect.NormalPower0;
    this.Reflectiveness0 = iEffect.Reflectiveness0;
    this.DiffuseTexture0 = iEffect.DiffuseTexture0;
    this.MaterialTexture0 = iEffect.MaterialTexture0;
    this.NormalTexture0 = iEffect.NormalTexture0;
    this.DiffuseTexture1AlphaDisabled = iEffect.DiffuseTexture1AlphaDisabled;
    this.AlphaMask1Enabled = iEffect.AlphaMask1Enabled;
    this.DiffuseColor1 = iEffect.DiffuseColor1;
    this.SpecAmount1 = iEffect.SpecAmount1;
    this.SpecPower1 = iEffect.SpecPower1;
    this.EmissiveAmount1 = iEffect.EmissiveAmount1;
    this.NormalPower1 = iEffect.NormalPower1;
    this.Reflectiveness1 = iEffect.Reflectiveness1;
    this.DiffuseTexture1 = iEffect.DiffuseTexture1;
    this.MaterialTexture1 = iEffect.MaterialTexture1;
    this.NormalTexture1 = iEffect.NormalTexture1;
  }

  public void AssignOpacityToEffect(RenderDeferredEffect iEffect)
  {
    iEffect.World = this.WorldTransform;
    iEffect.Alpha = this.Alpha;
    iEffect.Sharpness = this.Sharpness;
    iEffect.VertexColorEnabled = this.VertexColorEnabled;
    iEffect.DiffuseTexture0AlphaDisabled = this.DiffuseTexture0AlphaDisabled;
    iEffect.AlphaMask0Enabled = this.AlphaMask0Enabled;
    iEffect.DiffuseTexture0 = this.DiffuseTexture0;
    iEffect.DiffuseTexture1AlphaDisabled = this.DiffuseTexture1AlphaDisabled;
    iEffect.AlphaMask1Enabled = this.AlphaMask1Enabled;
    iEffect.DiffuseTexture1 = this.DiffuseTexture1;
  }

  public void AssignToEffect(RenderDeferredEffect iEffect)
  {
    iEffect.World = this.WorldTransform;
    iEffect.Alpha = this.Alpha;
    iEffect.Sharpness = this.Sharpness;
    iEffect.VertexColorEnabled = this.VertexColorEnabled;
    iEffect.UseMaterialTextureForReflectiveness = this.UseMaterialTextureForReflectiveness;
    iEffect.ReflectionMap = this.ReflectionMap;
    iEffect.DiffuseTexture0AlphaDisabled = this.DiffuseTexture0AlphaDisabled;
    iEffect.AlphaMask0Enabled = this.AlphaMask0Enabled;
    iEffect.DiffuseColor0 = this.DiffuseColor0;
    iEffect.SpecAmount0 = this.SpecAmount0;
    iEffect.SpecPower0 = this.SpecPower0;
    iEffect.EmissiveAmount0 = this.EmissiveAmount0;
    iEffect.NormalPower0 = this.NormalPower0;
    iEffect.Reflectiveness0 = this.Reflectiveness0;
    iEffect.DiffuseTexture0 = this.DiffuseTexture0;
    iEffect.MaterialTexture0 = this.MaterialTexture0;
    iEffect.NormalTexture0 = this.NormalTexture0;
    iEffect.DiffuseTexture1AlphaDisabled = this.DiffuseTexture1AlphaDisabled;
    iEffect.AlphaMask1Enabled = this.AlphaMask1Enabled;
    iEffect.DiffuseColor1 = this.DiffuseColor1;
    iEffect.SpecAmount1 = this.SpecAmount1;
    iEffect.SpecPower1 = this.SpecPower1;
    iEffect.EmissiveAmount1 = this.EmissiveAmount1;
    iEffect.NormalPower1 = this.NormalPower1;
    iEffect.Reflectiveness1 = this.Reflectiveness1;
    iEffect.DiffuseTexture1 = this.DiffuseTexture1;
    iEffect.MaterialTexture1 = this.MaterialTexture1;
    iEffect.NormalTexture1 = this.NormalTexture1;
  }
}
