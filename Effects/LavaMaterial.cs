// Decompiled with JetBrains decompiler
// Type: PolygonHead.Effects.LavaMaterial
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead.Effects;

public struct LavaMaterial : IMaterial<LavaEffect>
{
  public float MaskDistortion;
  public Vector2 Speed0;
  public Vector2 Speed1;
  public float LavaHotEmissiveAmount;
  public float LavaColdEmissiveAmount;
  public float LavaSpecAmount;
  public float LavaSpecPower;
  public float TempFrequency;
  public Texture2D ToneMap;
  public Texture2D TempMap;
  public Texture2D MaskMap;
  public Vector3 RockColor;
  public float RockSpecAmount;
  public float RockSpecPower;
  public float RockEmissiveAmount;
  public float RockNormalPower;
  public Texture2D RockTexture;
  public bool RockNormalMapEnabled;
  public Texture2D RockNormalMap;

  public void FetchFromEffect(LavaEffect iEffect)
  {
    this.MaskDistortion = iEffect.MaskDistortion;
    this.Speed0 = iEffect.Speed0;
    this.Speed1 = iEffect.Speed1;
    this.LavaHotEmissiveAmount = iEffect.LavaHotEmissiveAmount;
    this.LavaColdEmissiveAmount = iEffect.LavaColdEmissiveAmount;
    this.LavaSpecAmount = iEffect.LavaSpecAmount;
    this.LavaSpecPower = iEffect.LavaSpecPower;
    this.TempFrequency = iEffect.TempFrequency;
    this.ToneMap = iEffect.ToneMap;
    this.TempMap = iEffect.TempMap;
    this.MaskMap = iEffect.MaskMap;
    this.RockColor = iEffect.RockColor;
    this.RockSpecAmount = iEffect.RockSpecAmount;
    this.RockSpecPower = iEffect.RockSpecPower;
    this.RockEmissiveAmount = iEffect.RockEmissiveAmount;
    this.RockNormalPower = iEffect.RockNormalPower;
    this.RockTexture = iEffect.RockTexture;
    this.RockNormalMapEnabled = iEffect.RockNormalMapEnabled;
    this.RockNormalMap = iEffect.RockNormalMap;
  }

  public void AssignOpacityToEffect(LavaEffect iEffect)
  {
  }

  public void AssignToEffect(LavaEffect iEffect)
  {
    iEffect.MaskDistortion = this.MaskDistortion;
    iEffect.Speed0 = this.Speed0;
    iEffect.Speed1 = this.Speed1;
    iEffect.LavaHotEmissiveAmount = this.LavaHotEmissiveAmount;
    iEffect.LavaColdEmissiveAmount = this.LavaColdEmissiveAmount;
    iEffect.LavaSpecAmount = this.LavaSpecAmount;
    iEffect.LavaSpecPower = this.LavaSpecPower;
    iEffect.TempFrequency = this.TempFrequency;
    iEffect.ToneMap = this.ToneMap;
    iEffect.TempMap = this.TempMap;
    iEffect.MaskMap = this.MaskMap;
    iEffect.RockColor = this.RockColor;
    iEffect.RockSpecAmount = this.RockSpecAmount;
    iEffect.RockSpecPower = this.RockSpecPower;
    iEffect.RockEmissiveAmount = this.RockEmissiveAmount;
    iEffect.RockNormalPower = this.RockNormalPower;
    iEffect.RockTexture = this.RockTexture;
    iEffect.RockNormalMapEnabled = this.RockNormalMapEnabled;
    iEffect.RockNormalMap = this.RockNormalMap;
  }
}
