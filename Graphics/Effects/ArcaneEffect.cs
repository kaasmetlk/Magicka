// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.Effects.ArcaneEffect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace Magicka.Graphics.Effects;

public class ArcaneEffect : Effect
{
  public static readonly int TYPEHASH = typeof (ArcaneEffect).GetHashCode();
  private EffectParameter mViewProjectionParameter;
  private EffectParameter mWorldParameter;
  private EffectParameter mOriginParameter;
  private EffectParameter mDirectionParameter;
  private EffectParameter mColorCenterParameter;
  private EffectParameter mColorEdgeParameter;
  private EffectParameter mAlphaParameter;
  private EffectParameter mEyePosParameter;
  private EffectParameter mMinRadiusParameter;
  private EffectParameter mMaxRadiusParameter;
  private EffectParameter mStartLengthParameter;
  private EffectParameter mDropoffParameter;
  private EffectParameter mRayRadiusParameter;
  private EffectParameter mTimeParameter;
  private EffectParameter mTextureScaleParameter;
  private EffectParameter mWaveScaleParameter;
  private EffectParameter mClockwiceParameter;
  private EffectParameter mLengthParameter;
  private EffectParameter mTextureParameter;
  private EffectParameter mCutParameter;

  public ArcaneEffect(GraphicsDevice iDevice, ContentManager iContentManager)
    : base(iDevice, iContentManager.Load<Effect>("Shaders/ArcaneEffect"))
  {
    this.mViewProjectionParameter = this.Parameters[nameof (ViewProjection)];
    this.mWorldParameter = this.Parameters[nameof (World)];
    this.mOriginParameter = this.Parameters[nameof (Origin)];
    this.mDirectionParameter = this.Parameters[nameof (Direction)];
    this.mColorCenterParameter = this.Parameters[nameof (ColorCenter)];
    this.mColorEdgeParameter = this.Parameters[nameof (ColorEdge)];
    this.mAlphaParameter = this.Parameters[nameof (Alpha)];
    this.mCutParameter = this.Parameters[nameof (Cut)];
    this.mEyePosParameter = this.Parameters[nameof (EyePos)];
    this.mMinRadiusParameter = this.Parameters[nameof (MinRadius)];
    this.mMaxRadiusParameter = this.Parameters[nameof (MaxRadius)];
    this.mStartLengthParameter = this.Parameters[nameof (StartLength)];
    this.mDropoffParameter = this.Parameters[nameof (Dropoff)];
    this.mRayRadiusParameter = this.Parameters[nameof (RayRadius)];
    this.mTimeParameter = this.Parameters[nameof (Time)];
    this.mTextureScaleParameter = this.Parameters[nameof (TextureScale)];
    this.mWaveScaleParameter = this.Parameters[nameof (WaveScale)];
    this.mClockwiceParameter = this.Parameters[nameof (Clockwice)];
    this.mLengthParameter = this.Parameters[nameof (Length)];
    this.mTextureParameter = this.Parameters[nameof (Texture)];
  }

  public void SetTechnique(ArcaneEffect.Technique iTechnique)
  {
    this.CurrentTechnique = this.Techniques[(int) iTechnique];
  }

  public Matrix ViewProjection
  {
    get => this.mViewProjectionParameter.GetValueMatrix();
    set => this.mViewProjectionParameter.SetValue(value);
  }

  public Matrix World
  {
    get => this.mWorldParameter.GetValueMatrix();
    set => this.mWorldParameter.SetValue(value);
  }

  public Vector3 Origin
  {
    get => this.mOriginParameter.GetValueVector3();
    set => this.mOriginParameter.SetValue(value);
  }

  public Vector3 Direction
  {
    get => this.mDirectionParameter.GetValueVector3();
    set => this.mDirectionParameter.SetValue(value);
  }

  public Vector3 ColorCenter
  {
    get => this.mColorCenterParameter.GetValueVector3();
    set => this.mColorCenterParameter.SetValue(value);
  }

  public Vector3 ColorEdge
  {
    get => this.mColorEdgeParameter.GetValueVector3();
    set => this.mColorEdgeParameter.SetValue(value);
  }

  public float Alpha
  {
    get => this.mAlphaParameter.GetValueSingle();
    set => this.mAlphaParameter.SetValue(value);
  }

  public float Cut
  {
    get => this.mCutParameter.GetValueSingle();
    set => this.mCutParameter.SetValue(value);
  }

  public Vector3 EyePos
  {
    get => this.mEyePosParameter.GetValueVector3();
    set => this.mEyePosParameter.SetValue(value);
  }

  public float MinRadius
  {
    get => this.mMinRadiusParameter.GetValueSingle();
    set => this.mMinRadiusParameter.SetValue(value);
  }

  public float MaxRadius
  {
    get => this.mMaxRadiusParameter.GetValueSingle();
    set => this.mMaxRadiusParameter.SetValue(value);
  }

  public float StartLength
  {
    get => this.mStartLengthParameter.GetValueSingle();
    set => this.mStartLengthParameter.SetValue(value);
  }

  public float Dropoff
  {
    get => this.mDropoffParameter.GetValueSingle();
    set => this.mDropoffParameter.SetValue(value);
  }

  public float RayRadius
  {
    get => this.mRayRadiusParameter.GetValueSingle();
    set => this.mRayRadiusParameter.SetValue(value);
  }

  public float Time
  {
    get => this.mTimeParameter.GetValueSingle();
    set => this.mTimeParameter.SetValue(value);
  }

  public float TextureScale
  {
    get => this.mTextureScaleParameter.GetValueSingle();
    set => this.mTextureScaleParameter.SetValue(value);
  }

  public float WaveScale
  {
    get => this.mWaveScaleParameter.GetValueSingle();
    set => this.mWaveScaleParameter.SetValue(value);
  }

  public bool Clockwice
  {
    get => this.mClockwiceParameter.GetValueBoolean();
    set => this.mClockwiceParameter.SetValue(value);
  }

  public float Length
  {
    get => this.mLengthParameter.GetValueSingle();
    set => this.mLengthParameter.SetValue(value);
  }

  public Texture2D Texture
  {
    get => this.mTextureParameter.GetValueTexture2D();
    set => this.mTextureParameter.SetValue((Microsoft.Xna.Framework.Graphics.Texture) value);
  }

  public enum Technique
  {
    Generic,
    Beam,
  }
}
