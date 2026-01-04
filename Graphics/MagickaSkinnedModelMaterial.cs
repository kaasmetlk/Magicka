// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.MagickaSkinnedModelMaterial
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using XNAnimation.Effects;

#nullable disable
namespace Magicka.Graphics;

public struct MagickaSkinnedModelMaterial : IMaterial<SkinnedModelBasicEffect>
{
  public SkinnedModelBasicEffect.Technique Technique;
  public Texture2D DamageMap0;
  public bool DamageMap0Enabled;
  public Texture2D DamageMap1;
  public bool DamageMap1Enabled;
  public Texture2D DiffuseMap0;
  public bool DiffuseMap0Enabled;
  public Texture2D DiffuseMap1;
  public bool DiffuseMap1Enabled;
  public Vector3 DiffuseColor;
  public float EmissiveAmount;
  public float SpecularAmount;
  public float SpecularPower;
  public float OverlayAlpha;
  public Vector3 TintColor;
  public Texture2D SpecularMap;
  public bool SpecularMapEnabled;
  public bool UseSoftLightBlend;

  public void AssignOpacityToEffect(SkinnedModelBasicEffect iEffect)
  {
    iEffect.DiffuseMap0 = this.DiffuseMap0;
    iEffect.DiffuseMap0Enabled = this.DiffuseMap0Enabled;
    iEffect.DiffuseMap1 = this.DiffuseMap1;
    iEffect.DiffuseMap1Enabled = this.DiffuseMap1Enabled;
    iEffect.Diffuse1Alpha = this.OverlayAlpha;
  }

  public void AssignToEffect(SkinnedModelBasicEffect iEffect)
  {
    iEffect.DamageMap0 = this.DamageMap0;
    iEffect.DamageMap0Enabled = this.DamageMap0Enabled;
    iEffect.DamageMap1 = this.DamageMap1;
    iEffect.DamageMap1Enabled = this.DamageMap1Enabled;
    iEffect.DiffuseMap0 = this.DiffuseMap0;
    iEffect.DiffuseMap0Enabled = this.DiffuseMap0Enabled;
    iEffect.DiffuseMap1 = this.DiffuseMap1;
    iEffect.DiffuseMap1Enabled = this.DiffuseMap1Enabled;
    iEffect.DiffuseColor = this.DiffuseColor;
    iEffect.EmissiveAmount = this.EmissiveAmount;
    iEffect.SpecularAmount = this.SpecularAmount;
    iEffect.SpecularPower = this.SpecularPower;
    iEffect.TintColor = this.TintColor;
    iEffect.SpecularMap = this.SpecularMap;
    iEffect.SpecularMapEnabled = this.SpecularMapEnabled;
    iEffect.Diffuse1Alpha = this.OverlayAlpha;
    iEffect.UseSoftLightBlend = this.UseSoftLightBlend;
  }

  public void FetchFromEffect(SkinnedModelBasicEffect iEffect)
  {
    this.Technique = iEffect.ActiveTechnique;
    this.DamageMap0 = iEffect.DamageMap0;
    this.DamageMap0Enabled = iEffect.DamageMap0Enabled;
    this.DamageMap1 = iEffect.DamageMap1;
    this.DamageMap1Enabled = iEffect.DamageMap1Enabled;
    this.DiffuseMap0 = iEffect.DiffuseMap0;
    this.DiffuseMap0Enabled = iEffect.DiffuseMap0Enabled;
    this.DiffuseMap1 = iEffect.DiffuseMap1;
    this.DiffuseMap1Enabled = iEffect.DiffuseMap1Enabled;
    this.DiffuseColor = iEffect.DiffuseColor;
    this.EmissiveAmount = iEffect.EmissiveAmount;
    this.SpecularAmount = iEffect.SpecularAmount;
    this.SpecularPower = iEffect.SpecularPower;
    this.TintColor = iEffect.TintColor;
    this.SpecularMap = iEffect.SpecularMap;
    this.SpecularMapEnabled = iEffect.SpecularMapEnabled;
    this.OverlayAlpha = iEffect.Diffuse1Alpha;
    this.UseSoftLightBlend = iEffect.UseSoftLightBlend;
  }
}
