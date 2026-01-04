// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.Effects.ForceFieldEffect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace Magicka.Graphics.Effects;

internal class ForceFieldEffect : Effect
{
  public const int NR_COLL_PTS = 32 /*0x20*/;
  public static readonly int TYPEHASH = typeof (ForceFieldEffect).GetHashCode();
  private EffectParameter mWorldParameter;
  private EffectParameter mViewParameter;
  private EffectParameter mProjectionParameter;
  private EffectParameter mCameraPositionParameter;
  private EffectParameter mEyeOfTheBeholderParameter;
  private EffectParameter mVertexColorEnabledParameter;
  private EffectParameter mColorParameter;
  private EffectParameter mWidthParameter;
  private EffectParameter mAlphaPowerParameter;
  private EffectParameter mAlphaFalloffPowerParameter;
  private EffectParameter mMaxRadiusParameter;
  private EffectParameter mRippleDistortionParameter;
  private EffectParameter mMapDistortionParameter;
  private EffectParameter mDisplacementMapParameter;
  private EffectParameter mCollPointsParameter;
  private EffectParameter mCandidateParameter;
  private EffectParameter mDepthMapParameter;
  private EffectParameter mHalfPixelParameter;

  public ForceFieldEffect(GraphicsDevice iDevice, ContentManager iContent)
    : base(iDevice, iContent.Load<Effect>("Shaders/ForceFieldEffect"))
  {
    this.mWorldParameter = this.Parameters[nameof (World)];
    this.mViewParameter = this.Parameters[nameof (View)];
    this.mProjectionParameter = this.Parameters[nameof (Projection)];
    this.mCameraPositionParameter = this.Parameters[nameof (CameraPosition)];
    this.mEyeOfTheBeholderParameter = this.Parameters[nameof (EyeOfTheBeholder)];
    this.mVertexColorEnabledParameter = this.Parameters[nameof (VertexColorEnabled)];
    this.mColorParameter = this.Parameters[nameof (Color)];
    this.mWidthParameter = this.Parameters[nameof (Width)];
    this.mAlphaPowerParameter = this.Parameters[nameof (AlphaPower)];
    this.mAlphaFalloffPowerParameter = this.Parameters[nameof (AlphaFalloffPower)];
    this.mMaxRadiusParameter = this.Parameters[nameof (MaxRadius)];
    this.mRippleDistortionParameter = this.Parameters[nameof (RippleDistortion)];
    this.mMapDistortionParameter = this.Parameters[nameof (MapDistortion)];
    this.mDisplacementMapParameter = this.Parameters[nameof (DisplacementMap)];
    this.mCollPointsParameter = this.Parameters[nameof (CollPoints)];
    this.mCandidateParameter = this.Parameters[nameof (Candidate)];
    this.mDepthMapParameter = this.Parameters[nameof (DepthMap)];
    this.mHalfPixelParameter = this.Parameters["HalfPixel"];
  }

  public Matrix World
  {
    get => this.mWorldParameter.GetValueMatrix();
    set => this.mWorldParameter.SetValue(value);
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

  public Vector3 CameraPosition
  {
    get => this.mCameraPositionParameter.GetValueVector3();
    set => this.mCameraPositionParameter.SetValue(value);
  }

  public Vector3 EyeOfTheBeholder
  {
    get => this.mEyeOfTheBeholderParameter.GetValueVector3();
    set => this.mEyeOfTheBeholderParameter.SetValue(value);
  }

  public bool VertexColorEnabled
  {
    get => this.mVertexColorEnabledParameter.GetValueBoolean();
    set => this.mVertexColorEnabledParameter.SetValue(value);
  }

  public Vector3 Color
  {
    get => this.mColorParameter.GetValueVector3();
    set => this.mColorParameter.SetValue(value);
  }

  public float Width
  {
    get => this.mWidthParameter.GetValueSingle();
    set => this.mWidthParameter.SetValue(value);
  }

  public float AlphaPower
  {
    get => this.mAlphaPowerParameter.GetValueSingle();
    set => this.mAlphaPowerParameter.SetValue(value);
  }

  public float AlphaFalloffPower
  {
    get => this.mAlphaFalloffPowerParameter.GetValueSingle();
    set => this.mAlphaFalloffPowerParameter.SetValue(value);
  }

  public float MaxRadius
  {
    get => this.mMaxRadiusParameter.GetValueSingle();
    set => this.mMaxRadiusParameter.SetValue(value);
  }

  public float RippleDistortion
  {
    get => this.mRippleDistortionParameter.GetValueSingle();
    set => this.mRippleDistortionParameter.SetValue(value);
  }

  public float MapDistortion
  {
    get => this.mMapDistortionParameter.GetValueSingle();
    set => this.mMapDistortionParameter.SetValue(value);
  }

  public Texture2D DisplacementMap
  {
    get => this.mDisplacementMapParameter.GetValueTexture2D();
    set => this.mDisplacementMapParameter.SetValue((Texture) value);
  }

  public Vector4[] CollPoints
  {
    get => this.mCollPointsParameter.GetValueVector4Array(32 /*0x20*/);
    set => this.mCollPointsParameter.SetValue(value);
  }

  public Texture2D Candidate
  {
    get => this.mCandidateParameter.GetValueTexture2D();
    set => this.mCandidateParameter.SetValue((Texture) value);
  }

  public Texture2D DepthMap
  {
    get => this.mDepthMapParameter.GetValueTexture2D();
    set => this.mDepthMapParameter.SetValue((Texture) value);
  }

  public Point DestinationDimentions
  {
    get
    {
      Point destinationDimentions = new Point();
      Vector2 valueVector2 = this.mHalfPixelParameter.GetValueVector2();
      destinationDimentions.X = (int) (0.5 / (double) valueVector2.X);
      destinationDimentions.Y = (int) (0.5 / (double) valueVector2.Y);
      return destinationDimentions;
    }
    set
    {
      this.mHalfPixelParameter.SetValue(new Vector2()
      {
        X = 0.5f / (float) value.X,
        Y = 0.5f / (float) value.Y
      });
    }
  }
}
