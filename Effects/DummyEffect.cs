// Decompiled with JetBrains decompiler
// Type: PolygonHead.Effects.DummyEffect
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead.Effects;

public class DummyEffect : Effect
{
  private EffectParameter mViewParameter;
  private EffectParameter mProjectionParameter;
  private EffectParameter mViewProjectionParameter;
  private EffectParameter mInverseViewProjectionParameter;
  private EffectParameter mCameraPositionParameter;
  private EffectParameter mEyeOfTheBeholderParameter;
  private EffectParameter mTimeParameter;

  public DummyEffect(GraphicsDevice iGraphicsDevice, EffectPool iPool)
    : base(iGraphicsDevice, DummyEffectCode.CODE, CompilerOptions.None, iPool)
  {
    this.CacheParameters();
  }

  public DummyEffect(GraphicsDevice iGraphicsDevice, Effect iCloneSource)
    : base(iGraphicsDevice, iCloneSource)
  {
    this.CacheParameters();
  }

  private void CacheParameters()
  {
    this.mViewParameter = this.Parameters["View"];
    this.mProjectionParameter = this.Parameters["Projection"];
    this.mViewProjectionParameter = this.Parameters["ViewProjection"];
    this.mInverseViewProjectionParameter = this.Parameters["InverseViewProjection"];
    this.mCameraPositionParameter = this.Parameters["CameraPosition"];
    this.mEyeOfTheBeholderParameter = this.Parameters["EyeOfTheBeholder"];
    this.mTimeParameter = this.Parameters["Time"];
  }

  public Matrix View
  {
    get => this.mViewParameter.GetValueMatrix();
    set => this.mViewParameter.SetValue(value);
  }

  public Matrix Projection
  {
    get => this.mProjectionParameter.GetValueMatrix();
    set => this.mProjectionParameter.SetValue(value);
  }

  public Matrix ViewProjection
  {
    get => this.mViewProjectionParameter.GetValueMatrix();
    set => this.mViewProjectionParameter.SetValue(value);
  }

  public Matrix InverseViewProjection
  {
    get => this.mInverseViewProjectionParameter.GetValueMatrix();
    set => this.mInverseViewProjectionParameter.SetValue(value);
  }

  public Vector3 CameraPosition
  {
    get => this.mCameraPositionParameter.GetValueVector3();
    set => this.mCameraPositionParameter.SetValue(value);
  }

  public Vector3 EyeOfTheBeholder
  {
    get
    {
      return this.mEyeOfTheBeholderParameter != null ? this.mEyeOfTheBeholderParameter.GetValueVector3() : new Vector3();
    }
    set
    {
      if (this.mEyeOfTheBeholderParameter == null)
        return;
      this.mEyeOfTheBeholderParameter.SetValue(value);
    }
  }

  public float Time
  {
    get => this.mTimeParameter.GetValueSingle();
    set => this.mTimeParameter.SetValue(value);
  }
}
