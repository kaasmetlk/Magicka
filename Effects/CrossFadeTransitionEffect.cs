// Decompiled with JetBrains decompiler
// Type: PolygonHead.Effects.CrossFadeTransitionEffect
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead.Effects;

public class CrossFadeTransitionEffect(GraphicsDevice iDevice) : TransitionEffect(iDevice, CrossFadeTransitionEffectCode.CODE, (EffectPool) null)
{
  private float mFadeAmount;
  private EffectParameter mTextureParameter;
  private EffectParameter mFadeAmountParameter;

  public override void CacheParameters()
  {
    base.CacheParameters();
    this.mFadeAmountParameter = this.Parameters["FadeAmount"];
    this.mTextureParameter = this.Parameters["Texture"];
  }

  public override void Update(float iDeltaTime)
  {
    if (this.mTransitionDirection == TransitionEffect.TransitionDirections.In)
      this.mFadeAmount -= iDeltaTime / this.mTransitionTime;
    else
      this.mFadeAmount += iDeltaTime / this.mTransitionTime;
    this.mFadeAmount = MathHelper.Clamp(this.mFadeAmount, 0.0f, 1f);
    this.mFadeAmountParameter.SetValue(this.mFadeAmount);
  }

  public override void Start(
    TransitionEffect.TransitionDirections iTransitionDirection,
    float iTransitionTime)
  {
    base.Start(iTransitionDirection, iTransitionTime);
    if (iTransitionDirection == TransitionEffect.TransitionDirections.In)
      this.mFadeAmount = 1f;
    else
      this.mFadeAmount = 0.0f;
  }

  public override bool Finished
  {
    get
    {
      return this.mTransitionDirection == TransitionEffect.TransitionDirections.In & (double) this.mFadeAmount <= 0.0 | this.mTransitionDirection == TransitionEffect.TransitionDirections.Out & (double) this.mFadeAmount >= 1.0;
    }
  }
}
