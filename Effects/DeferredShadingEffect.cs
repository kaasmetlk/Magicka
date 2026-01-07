// Decompiled with JetBrains decompiler
// Type: PolygonHead.Effects.DeferredShadingEffect
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead.Effects;

internal class DeferredShadingEffect(GraphicsDevice iGraphicsDevice, EffectPool iPool) : 
  PostProcessingEffect(iGraphicsDevice, DeferredShadingEffectCode.CODE, iPool)
{
  private Fog mFog;
  private EffectParameter mFogEnableParameter;
  private EffectParameter mFogStartParameter;
  private EffectParameter mFogEndParameter;
  private EffectParameter mFogColorParameter;
  private EffectParameter mSourceTexture4Parameter;

  public override void CacheParameters()
  {
    base.CacheParameters();
    this.mFogEnableParameter = this.Parameters["FogEnable"];
    this.mFogStartParameter = this.Parameters["FogStart"];
    this.mFogEndParameter = this.Parameters["FogEnd"];
    this.mFogColorParameter = this.Parameters["FogColor"];
    this.mSourceTexture4Parameter = this.Parameters["SourceTexture4"];
    this.mFog.Enabled = this.mFogEnableParameter.GetValueBoolean();
    this.mFog.Start = this.mFogStartParameter.GetValueSingle();
    this.mFog.End = this.mFogEndParameter.GetValueSingle();
    this.mFog.Color = this.mFogColorParameter.GetValueVector4();
  }

  public virtual Texture2D SourceTexture4
  {
    get => this.mSourceTexture4Parameter.GetValueTexture2D();
    set => this.mSourceTexture4Parameter.SetValue((Texture) value);
  }

  public virtual Fog Fog
  {
    get => this.mFog;
    set
    {
      this.mFog = value;
      this.mFogEnableParameter.SetValue(this.mFog.Enabled);
      this.mFogStartParameter.SetValue(this.mFog.Start);
      this.mFogEndParameter.SetValue(this.mFog.End);
      this.mFogColorParameter.SetValue(this.mFog.Color);
    }
  }
}
