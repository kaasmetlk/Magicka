// Decompiled with JetBrains decompiler
// Type: PolygonHead.Effects.FilterEffect
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead.Effects;

public class FilterEffect(GraphicsDevice iDevice, EffectPool iPool) : PostProcessingEffect(iDevice, FilterEffectCode.CODE, iPool)
{
  private EffectParameter mBrightnessParameter;
  private EffectParameter mContrastParameter;
  private EffectParameter mSaturationParameter;

  public override void CacheParameters()
  {
    base.CacheParameters();
    this.mBrightnessParameter = this.Parameters["Brightness"];
    this.mContrastParameter = this.Parameters["Contrast"];
    this.mSaturationParameter = this.Parameters["Saturation"];
  }

  public float Brightness
  {
    get => this.mBrightnessParameter.GetValueSingle();
    set => this.mBrightnessParameter.SetValue(value);
  }

  public float Contrast
  {
    get => this.mContrastParameter.GetValueSingle();
    set => this.mContrastParameter.SetValue(value);
  }

  public float Saturation
  {
    get => this.mSaturationParameter.GetValueSingle();
    set => this.mSaturationParameter.SetValue(value);
  }
}
