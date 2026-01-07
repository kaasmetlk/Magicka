// Decompiled with JetBrains decompiler
// Type: PolygonHead.Effects.RestoreDepthEffect
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead.Effects;

public class RestoreDepthEffect(GraphicsDevice iDevice, EffectPool iPool) : PostProcessingEffect(iDevice, RestoreDepthEffectCode.CODE, iPool)
{
  private EffectParameter mColor0Parameter;
  private EffectParameter mColor1Parameter;

  public override void CacheParameters()
  {
    base.CacheParameters();
    this.mColor0Parameter = this.Parameters["Color0"];
    this.mColor1Parameter = this.Parameters["Color1"];
  }

  public Vector4 Color0
  {
    get => this.mColor0Parameter.GetValueVector4();
    set => this.mColor0Parameter.SetValue(value);
  }

  public Vector4 Color1
  {
    get => this.mColor1Parameter.GetValueVector4();
    set => this.mColor1Parameter.SetValue(value);
  }
}
