// Decompiled with JetBrains decompiler
// Type: PolygonHead.Effects.SkinnedModelDeferredAdvancedMaterial
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead.Effects;

public struct SkinnedModelDeferredAdvancedMaterial : IMaterial<SkinnedModelDeferredEffect>
{
  public Vector3 DiffuseColor;
  public Vector3 TintColor;
  public float EmissiveAmount;
  public float SpecularAmount;
  public float SpecularBias;
  public float SpecularPower;
  public float Alpha;
  public float NormalPower;
  public float Damage;
  public float Bloat;
  public Vector4 CubeMapColor;
  public Vector4 ProjectionMapColor;
  public Vector4 Colorize;
  public float OverlayAlpha;
  public bool DiffuseMap0Enabled;
  public bool DiffuseMap1Enabled;
  public bool SpecularMapEnabled;
  public bool NormalMapEnabled;
  public bool DamageMap0Enabled;
  public bool DamageMap1Enabled;
  public bool ProjectionMapEnabled;
  public bool ProjectionMapAdditive;
  public bool CubeMapEnabled;
  public bool CubeNormalMapEnabled;
  public bool UseSoftLightBlend;
  public Matrix ProjectionMapMatrix;
  public Matrix CubeMapRotation;
  public Texture2D DiffuseMap0;
  public Texture2D DiffuseMap1;
  public Texture2D NormalMap;
  public Texture2D MaterialMap;
  public Texture2D DamageMap0;
  public Texture2D DamageMap1;
  public Texture2D ProjectionMap;
  public TextureCube CubeMap;
  public TextureCube CubeNormalMap;

  public void CopyFrom(ref SkinnedModelDeferredAdvancedMaterial iMaterial)
  {
    this.DiffuseColor = iMaterial.DiffuseColor;
    this.TintColor = iMaterial.TintColor;
    this.EmissiveAmount = iMaterial.EmissiveAmount;
    this.SpecularAmount = iMaterial.SpecularAmount;
    this.SpecularBias = iMaterial.SpecularBias;
    this.SpecularPower = iMaterial.SpecularPower;
    this.Alpha = iMaterial.Alpha;
    this.NormalPower = iMaterial.NormalPower;
    this.Damage = iMaterial.Damage;
    this.Bloat = iMaterial.Bloat;
    this.CubeMapColor = iMaterial.CubeMapColor;
    this.ProjectionMapColor = iMaterial.ProjectionMapColor;
    this.Colorize = iMaterial.Colorize;
    this.OverlayAlpha = iMaterial.OverlayAlpha;
    this.DiffuseMap0Enabled = iMaterial.DiffuseMap0Enabled;
    this.DiffuseMap1Enabled = iMaterial.DiffuseMap1Enabled;
    this.SpecularMapEnabled = iMaterial.SpecularMapEnabled;
    this.NormalMapEnabled = iMaterial.NormalMapEnabled;
    this.DamageMap0Enabled = iMaterial.DamageMap0Enabled;
    this.DamageMap1Enabled = iMaterial.DamageMap1Enabled;
    this.ProjectionMapEnabled = iMaterial.ProjectionMapEnabled;
    this.ProjectionMapAdditive = iMaterial.ProjectionMapAdditive;
    this.CubeMapEnabled = iMaterial.CubeMapEnabled;
    this.CubeNormalMapEnabled = iMaterial.CubeNormalMapEnabled;
    this.UseSoftLightBlend = iMaterial.UseSoftLightBlend;
    this.ProjectionMapMatrix = iMaterial.ProjectionMapMatrix;
    this.CubeMapRotation = iMaterial.CubeMapRotation;
    this.DiffuseMap0 = iMaterial.DiffuseMap0;
    this.DiffuseMap1 = iMaterial.DiffuseMap1;
    this.NormalMap = iMaterial.NormalMap;
    this.MaterialMap = iMaterial.MaterialMap;
    this.DamageMap0 = iMaterial.DamageMap0;
    this.DamageMap1 = iMaterial.DamageMap1;
    this.ProjectionMap = iMaterial.ProjectionMap;
    this.CubeMap = iMaterial.CubeMap;
    this.CubeNormalMap = iMaterial.CubeNormalMap;
  }

  public void CopyFrom(ref SkinnedModelDeferredBasicMaterial iMaterial)
  {
    this.DiffuseColor = iMaterial.DiffuseColor;
    this.TintColor = iMaterial.TintColor;
    this.EmissiveAmount = iMaterial.EmissiveAmount;
    this.SpecularAmount = iMaterial.SpecularAmount;
    this.SpecularPower = iMaterial.SpecularPower;
    this.Alpha = iMaterial.Alpha;
    this.NormalPower = iMaterial.NormalPower;
    this.Damage = iMaterial.Damage;
    this.OverlayAlpha = iMaterial.OverlayAlpha;
    this.DiffuseMap0Enabled = iMaterial.DiffuseMap0Enabled;
    this.DiffuseMap1Enabled = iMaterial.DiffuseMap1Enabled;
    this.SpecularMapEnabled = iMaterial.SpecularMapEnabled;
    this.NormalMapEnabled = iMaterial.NormalMapEnabled;
    this.DamageMap0Enabled = iMaterial.DamageMap0Enabled;
    this.DamageMap1Enabled = iMaterial.DamageMap1Enabled;
    this.UseSoftLightBlend = iMaterial.UseSoftLightBlend;
    this.DiffuseMap0 = iMaterial.DiffuseMap0;
    this.DiffuseMap1 = iMaterial.DiffuseMap1;
    this.NormalMap = iMaterial.NormalMap;
    this.MaterialMap = iMaterial.MaterialMap;
    this.DamageMap0 = iMaterial.DamageMap0;
    this.DamageMap1 = iMaterial.DamageMap1;
  }

  public void FetchFromEffect(SkinnedModelDeferredEffect iEffect)
  {
    this.DiffuseColor = iEffect.DiffuseColor;
    this.TintColor = iEffect.TintColor;
    this.EmissiveAmount = iEffect.EmissiveAmount;
    this.SpecularAmount = iEffect.SpecularAmount;
    this.SpecularBias = iEffect.SpecularBias;
    this.SpecularPower = iEffect.SpecularPower;
    this.Alpha = iEffect.Alpha;
    this.NormalPower = iEffect.NormalPower;
    this.Damage = iEffect.Damage;
    this.Bloat = iEffect.Bloat;
    this.CubeMapColor = iEffect.CubeMapColor;
    this.Colorize = iEffect.Colorize;
    this.OverlayAlpha = iEffect.OverlayAlpha;
    this.DiffuseMap0Enabled = iEffect.DiffuseMap0Enabled;
    this.DiffuseMap1Enabled = iEffect.DiffuseMap1Enabled;
    this.SpecularMapEnabled = iEffect.SpecularMapEnabled;
    this.NormalMapEnabled = iEffect.NormalMapEnabled;
    this.DamageMap0Enabled = iEffect.DamageMap0Enabled;
    this.DamageMap1Enabled = iEffect.DamageMap1Enabled;
    this.ProjectionMapEnabled = iEffect.ProjectionMapEnabled;
    this.CubeMapEnabled = iEffect.CubeMapEnabled;
    this.CubeNormalMapEnabled = iEffect.CubeNormalMapEnabled;
    this.UseSoftLightBlend = iEffect.UseSoftLightBlend;
    this.ProjectionMapMatrix = iEffect.ProjectionMapMatrix;
    this.CubeMapRotation = iEffect.CubeMapRotation;
    this.DiffuseMap0 = iEffect.DiffuseMap0;
    this.DiffuseMap1 = iEffect.DiffuseMap1;
    this.NormalMap = iEffect.NormalMap;
    this.MaterialMap = iEffect.MaterialMap;
    this.DamageMap0 = iEffect.DamageMap0;
    this.DamageMap1 = iEffect.DamageMap1;
    this.ProjectionMap = iEffect.ProjectionMap;
    this.CubeMap = iEffect.CubeMap;
    this.CubeNormalMap = iEffect.CubeNormalMap;
  }

  public void AssignOpacityToEffect(SkinnedModelDeferredEffect iEffect)
  {
    iEffect.Alpha = this.Alpha;
    iEffect.Bloat = this.Bloat;
    iEffect.OverlayAlpha = this.OverlayAlpha;
    iEffect.DiffuseMap0Enabled = this.DiffuseMap0Enabled;
    iEffect.DiffuseMap1Enabled = this.DiffuseMap1Enabled;
    iEffect.DiffuseMap0 = this.DiffuseMap0;
    iEffect.DiffuseMap1 = this.DiffuseMap1;
  }

  public void AssignToEffect(SkinnedModelDeferredEffect iEffect)
  {
    iEffect.DiffuseColor = this.DiffuseColor;
    iEffect.TintColor = this.TintColor;
    iEffect.EmissiveAmount = this.EmissiveAmount;
    iEffect.SpecularAmount = this.SpecularAmount;
    iEffect.SpecularBias = this.SpecularBias;
    iEffect.SpecularPower = this.SpecularPower;
    iEffect.Alpha = this.Alpha;
    iEffect.NormalPower = this.NormalPower;
    iEffect.Damage = this.Damage;
    iEffect.Bloat = this.Bloat;
    iEffect.CubeMapColor = this.CubeMapColor;
    iEffect.Colorize = this.Colorize;
    iEffect.OverlayAlpha = this.OverlayAlpha;
    iEffect.DiffuseMap0Enabled = this.DiffuseMap0Enabled;
    iEffect.DiffuseMap1Enabled = this.DiffuseMap1Enabled;
    iEffect.SpecularMapEnabled = this.SpecularMapEnabled;
    iEffect.NormalMapEnabled = this.NormalMapEnabled;
    iEffect.DamageMap0Enabled = this.DamageMap0Enabled;
    iEffect.DamageMap1Enabled = this.DamageMap1Enabled;
    iEffect.ProjectionMapEnabled = this.ProjectionMapEnabled;
    iEffect.CubeMapEnabled = this.CubeMapEnabled;
    iEffect.CubeNormalMapEnabled = this.CubeNormalMapEnabled;
    iEffect.UseSoftLightBlend = this.UseSoftLightBlend;
    iEffect.ProjectionMapMatrix = this.ProjectionMapMatrix;
    iEffect.CubeMapRotation = this.CubeMapRotation;
    iEffect.DiffuseMap0 = this.DiffuseMap0;
    iEffect.DiffuseMap1 = this.DiffuseMap1;
    iEffect.NormalMap = this.NormalMap;
    iEffect.MaterialMap = this.MaterialMap;
    iEffect.DamageMap0 = this.DamageMap0;
    iEffect.DamageMap1 = this.DamageMap1;
    iEffect.ProjectionMap = this.ProjectionMap;
    iEffect.CubeMap = this.CubeMap;
    iEffect.CubeNormalMap = this.CubeNormalMap;
  }
}
