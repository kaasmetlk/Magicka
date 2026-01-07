// Decompiled with JetBrains decompiler
// Type: PolygonHead.Effects.TransitionEffect
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead.Effects;

public abstract class TransitionEffect : PostProcessingEffect
{
  protected TransitionEffect.TransitionDirections mTransitionDirection;
  protected float mTransitionTime;
  private EffectParameter mColorParameter;

  public TransitionEffect(GraphicsDevice iDevice, Effect iCloneSource)
    : base(iDevice, iCloneSource)
  {
  }

  public TransitionEffect(GraphicsDevice iDevice, byte[] iEffectCode, EffectPool iPool)
    : base(iDevice, iEffectCode, iPool)
  {
  }

  public override void CacheParameters()
  {
    base.CacheParameters();
    this.mColorParameter = this.Parameters["Color"];
  }

  public virtual void Start(
    TransitionEffect.TransitionDirections iTransitionDirection,
    float iTransitionTime)
  {
    this.mTransitionDirection = iTransitionDirection;
    this.mTransitionTime = iTransitionTime;
  }

  public abstract void Update(float iDeltaTime);

  public Color Color
  {
    get => new Color(this.mColorParameter.GetValueVector3());
    set => this.mColorParameter.SetValue(value.ToVector3());
  }

  public float TransitionTime => this.mTransitionTime;

  public TransitionEffect.TransitionDirections TransitionDirection => this.mTransitionDirection;

  public abstract bool Finished { get; }

  public enum TransitionDirections
  {
    In,
    Out,
  }
}
