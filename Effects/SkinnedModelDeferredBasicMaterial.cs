// Decompiled with JetBrains decompiler
// Type: PolygonHead.Effects.SkinnedModelDeferredBasicMaterial
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead.Effects;

public struct SkinnedModelDeferredBasicMaterial : IMaterial<SkinnedModelDeferredEffect>
{
  public Vector3 DiffuseColor;
  public Vector3 TintColor;
  public float EmissiveAmount;
  public float SpecularAmount;
  public float SpecularPower;
  public float Alpha;
  public float NormalPower;
  public float Damage;
  public float OverlayAlpha;
  public bool DiffuseMap0Enabled;
  public bool DiffuseMap1Enabled;
  public bool SpecularMapEnabled;
  public bool NormalMapEnabled;
  public bool DamageMap0Enabled;
  public bool DamageMap1Enabled;
  public bool UseSoftLightBlend;
  public Texture2D DiffuseMap0;
  public Texture2D DiffuseMap1;
  public Texture2D NormalMap;
  public Texture2D MaterialMap;
  public Texture2D DamageMap0;
  public Texture2D DamageMap1;

  public void CopyFrom(ref SkinnedModelDeferredAdvancedMaterial iMaterial)
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
    this.SpecularPower = iEffect.SpecularPower;
    this.Alpha = iEffect.Alpha;
    this.NormalPower = iEffect.NormalPower;
    this.Damage = iEffect.Damage;
    this.OverlayAlpha = iEffect.OverlayAlpha;
    this.DiffuseMap0Enabled = iEffect.DiffuseMap0Enabled;
    this.DiffuseMap1Enabled = iEffect.DiffuseMap1Enabled;
    this.SpecularMapEnabled = iEffect.SpecularMapEnabled;
    this.NormalMapEnabled = iEffect.NormalMapEnabled;
    this.DamageMap0Enabled = iEffect.DamageMap0Enabled;
    this.DamageMap1Enabled = iEffect.DamageMap1Enabled;
    this.UseSoftLightBlend = iEffect.UseSoftLightBlend;
    this.DiffuseMap0 = iEffect.DiffuseMap0;
    this.DiffuseMap1 = iEffect.DiffuseMap1;
    this.NormalMap = iEffect.NormalMap;
    this.MaterialMap = iEffect.MaterialMap;
    this.DamageMap0 = iEffect.DamageMap0;
    this.DamageMap1 = iEffect.DamageMap1;
  }

  public void AssignOpacityToEffect(SkinnedModelDeferredEffect iEffect)
  {
    iEffect.Alpha = this.Alpha;
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
    iEffect.SpecularPower = this.SpecularPower;
    iEffect.Alpha = this.Alpha;
    iEffect.NormalPower = this.NormalPower;
    iEffect.Damage = this.Damage;
    iEffect.OverlayAlpha = this.OverlayAlpha;
    iEffect.DiffuseMap0Enabled = this.DiffuseMap0Enabled;
    iEffect.DiffuseMap1Enabled = this.DiffuseMap1Enabled;
    iEffect.SpecularMapEnabled = this.SpecularMapEnabled;
    iEffect.NormalMapEnabled = this.NormalMapEnabled;
    iEffect.DamageMap0Enabled = this.DamageMap0Enabled;
    iEffect.DamageMap1Enabled = this.DamageMap1Enabled;
    iEffect.UseSoftLightBlend = this.UseSoftLightBlend;
    iEffect.DiffuseMap0 = this.DiffuseMap0;
    iEffect.DiffuseMap1 = this.DiffuseMap1;
    iEffect.NormalMap = this.NormalMap;
    iEffect.MaterialMap = this.MaterialMap;
    iEffect.DamageMap0 = this.DamageMap0;
    iEffect.DamageMap1 = this.DamageMap1;
  }
}
