// Decompiled with JetBrains decompiler
// Type: PolygonHead.Effects.SkinnedModelDeferredNormalMappedMaterial
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead.Effects;

public struct SkinnedModelDeferredNormalMappedMaterial : 
  IMaterial<SkinnedModelDeferredNormalMappedEffect>
{
  public Vector3 DiffuseColor;
  public Vector4 OverrideColor;
  public float SpecularAmount;
  public float SpecularPower;
  public float EmissiveAmount;
  public float NormalPower;
  public Texture2D DiffuseMap;
  public Texture2D MaterialMap;
  public Texture2D DamageMap;
  public Texture2D NormalMap;
  public Texture2D NormalDamageMap;
  public float Bloat;
  public float Damage;
  public Vector4 Colorize;

  public void FetchFromEffect(SkinnedModelDeferredNormalMappedEffect iEffect)
  {
    this.DiffuseColor = iEffect.DiffuseColor;
    this.OverrideColor = iEffect.OverrideColor;
    this.SpecularAmount = iEffect.SpecularAmount;
    this.SpecularPower = iEffect.SpecularPower;
    this.EmissiveAmount = iEffect.EmissiveAmount;
    this.NormalPower = iEffect.NormalPower;
    this.DiffuseMap = iEffect.DiffuseMap;
    this.MaterialMap = iEffect.MaterialMap;
    this.DamageMap = iEffect.DamageMap;
    this.NormalMap = iEffect.NormalMap;
    this.NormalDamageMap = iEffect.NormalDamageMap;
    this.Bloat = iEffect.Bloat;
    this.Damage = iEffect.Damage;
    this.Colorize = iEffect.Colorize;
  }

  public void AssignOpacityToEffect(SkinnedModelDeferredNormalMappedEffect iEffect)
  {
    iEffect.DiffuseMap = this.DiffuseMap;
  }

  public void AssignToEffect(SkinnedModelDeferredNormalMappedEffect iEffect)
  {
    iEffect.DiffuseColor = this.DiffuseColor;
    iEffect.OverrideColor = this.OverrideColor;
    iEffect.SpecularAmount = this.SpecularAmount;
    iEffect.SpecularPower = this.SpecularPower;
    iEffect.EmissiveAmount = this.EmissiveAmount;
    iEffect.NormalPower = this.NormalPower;
    iEffect.DiffuseMap = this.DiffuseMap;
    iEffect.MaterialMap = this.MaterialMap;
    iEffect.DamageMap = this.DamageMap;
    iEffect.NormalMap = this.NormalMap;
    iEffect.NormalDamageMap = this.NormalDamageMap;
    iEffect.Bloat = this.Bloat;
    iEffect.Damage = this.Damage;
    iEffect.Colorize = this.Colorize;
  }
}
