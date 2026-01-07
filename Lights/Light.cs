// Decompiled with JetBrains decompiler
// Type: PolygonHead.Lights.Light
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace PolygonHead.Lights;

public abstract class Light
{
  private static Random sRandom = new Random();
  private bool mEnabled;
  private LightTransitionType mTransitionEffect;
  private float mTransitionTime;
  private float mTime;
  private LightVariationType mVariationType;
  private float mVariationSpeed = 1f;
  private float mVariationAmount = 0.5f;
  private float mVariationTimer;
  private float mCurrentVariation;
  private Scene mScene;
  protected float mIntensity;

  public string Name { get; set; }

  public abstract Vector3 DiffuseColor { get; set; }

  public abstract Vector3 AmbientColor { get; set; }

  public abstract float SpecularAmount { get; set; }

  public abstract int Effect { get; }

  public abstract int Technique { get; }

  public abstract VertexBuffer VertexBuffer { get; }

  public abstract IndexBuffer IndexBuffer { get; }

  public abstract VertexDeclaration VertexDeclaration { get; }

  public abstract int VertexStride { get; }

  public Scene Scene => this.mScene;

  public LightVariationType VariationType
  {
    get => this.mVariationType;
    set => this.mVariationType = value;
  }

  public float VariationAmount
  {
    get => this.mVariationAmount;
    set => this.mVariationAmount = value;
  }

  public float VariationSpeed
  {
    get => this.mVariationSpeed;
    set => this.mVariationSpeed = value;
  }

  public abstract bool CastShadows { get; set; }

  public abstract int ShadowMapSize { get; set; }

  public abstract bool ShouldDraw(BoundingFrustum iViewFrustum);

  protected static float GetIntensity(LightTransitionType iType, float iNormalizedTime)
  {
    switch (iType)
    {
      case LightTransitionType.Linear:
        return iNormalizedTime;
      case LightTransitionType.Flicker:
        return Light.sRandom.NextDouble() >= (double) iNormalizedTime ? 0.0f : 1f;
      default:
        return 1f;
    }
  }

  protected static float GetIntensity(
    LightVariationType iType,
    float iDeltaTime,
    float iVariationSpeed,
    float iVariationAmount,
    ref float iVariationTimer,
    ref float iCurrentVariation)
  {
    switch (iType)
    {
      case LightVariationType.Sine:
        iVariationTimer = MathHelper.WrapAngle(iVariationTimer + iDeltaTime * iVariationSpeed);
        return (float) (Math.Sin((double) iVariationTimer) * (double) iVariationAmount + 1.0);
      case LightVariationType.Flicker:
        iVariationTimer -= iDeltaTime;
        if ((double) iVariationTimer <= 0.0)
        {
          iCurrentVariation = (double) iCurrentVariation <= 0.5 ? 1f : 0.0f;
          float num = (float) Light.sRandom.NextDouble() / iVariationSpeed;
          iVariationTimer = num;
        }
        return iCurrentVariation;
      case LightVariationType.Candle:
        iVariationTimer += (float) (Light.sRandom.NextDouble() - 0.5) * iVariationAmount * iDeltaTime;
        iVariationTimer -= (float) ((double) iVariationSpeed * (double) iCurrentVariation + 0.800000011920929 * (double) iVariationTimer) * iDeltaTime;
        iCurrentVariation += iVariationTimer * iDeltaTime;
        return 1f + iCurrentVariation;
      case LightVariationType.Strobe:
        iVariationTimer -= iDeltaTime;
        if ((double) iVariationTimer <= 0.0)
        {
          if ((double) iCurrentVariation < 0.5)
          {
            iCurrentVariation = 1f;
            iVariationTimer += (1f - iVariationAmount) / iVariationSpeed;
          }
          else
          {
            iCurrentVariation = 0.0f;
            iVariationTimer += iVariationAmount / iVariationSpeed;
          }
        }
        return iCurrentVariation;
      default:
        return 1f;
    }
  }

  protected internal virtual void Update(
    DataChannel iDataChannel,
    float iDeltaTime,
    ref Vector3 iCameraPosition,
    ref Vector3 iCameraDirection)
  {
    if (this.mTransitionEffect != LightTransitionType.None)
    {
      if (this.mEnabled)
      {
        this.mTime += iDeltaTime;
        if ((double) this.mTime >= (double) this.mTransitionTime)
        {
          this.mIntensity = 1f;
          this.mTransitionEffect = LightTransitionType.None;
        }
      }
      else
      {
        this.mTime -= iDeltaTime;
        if ((double) this.mTime <= 0.0)
        {
          this.OnRemove();
          this.mScene.RemoveLight(this);
          this.mIntensity = 0.0f;
        }
      }
      this.mIntensity = Light.GetIntensity(this.mTransitionEffect, this.mTime / this.mTransitionTime);
    }
    else
      this.mIntensity = 1f;
    this.mIntensity *= Light.GetIntensity(this.mVariationType, iDeltaTime, this.mVariationSpeed, this.mVariationAmount, ref this.mVariationTimer, ref this.mCurrentVariation);
  }

  public abstract void DrawShadows(DataChannel iDataChannel, float iDeltaTime, Scene iScene);

  public abstract void Draw(
    Microsoft.Xna.Framework.Graphics.Effect iEffect,
    DataChannel iDataChannel,
    float iDeltaTime,
    Texture2D iNormalMap,
    Texture2D iDepthMap);

  public abstract void DisposeShadowMap();

  public abstract void CreateShadowMap();

  public bool Enabled => this.mEnabled;

  public void Enable(Scene iScene) => this.Enable(iScene, LightTransitionType.None, 0.0f);

  public void Enable(Scene iScene, LightTransitionType iEffect, float iTransitionTime)
  {
    if (this.mEnabled)
    {
      if (iScene == this.mScene)
        return;
      this.Disable();
    }
    this.mScene = iScene;
    this.mIntensity = 0.0f;
    this.mEnabled = true;
    this.mTransitionEffect = iEffect;
    this.mTransitionTime = iTransitionTime;
    this.mTime = 0.0f;
    this.mScene.AddLight(this);
  }

  public void Disable() => this.Disable(LightTransitionType.None, 0.0f);

  public void Disable(LightTransitionType iEffect, float iTransitionTime)
  {
    if (!this.mEnabled)
      return;
    this.mEnabled = false;
    this.mTransitionEffect = iEffect;
    this.mTransitionTime = iTransitionTime;
    this.mTime = iTransitionTime;
    if (!(iEffect == LightTransitionType.None | (double) iTransitionTime < 1.4012984643248171E-45))
      return;
    this.OnRemove();
    this.mScene.RemoveLight(this);
  }

  protected virtual void OnRemove()
  {
  }

  public override string ToString()
  {
    return !string.IsNullOrEmpty(this.Name) ? this.Name : base.ToString();
  }
}
