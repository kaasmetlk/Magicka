// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.Effects.CapsuleLightEffect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace Magicka.Graphics.Effects;

public class CapsuleLightEffect : Effect
{
  public static readonly int TYPEHASH = typeof (CapsuleLightEffect).GetHashCode();
  private EffectParameter mWorldParameter;
  private Matrix mWorld;
  private EffectParameter mViewProjectionParameter;
  private Matrix mViewProjection;
  private EffectParameter mInverseViewProjectionParameter;
  private Matrix mInverseViewProjection;
  private EffectParameter mLengthParameter;
  private float mLength;
  private EffectParameter mRadiusParameter;
  private float mRadius;
  private EffectParameter mStartParameter;
  private Vector3 mStart;
  private EffectParameter mEndParameter;
  private Vector3 mEnd;
  private EffectParameter mDiffuseColorParameter;
  private Vector3 mDiffuseColor;
  private EffectParameter mAmbientColorParameter;
  private Vector3 mAmbientColor;
  private EffectParameter mHalfPixelParameter;
  private Vector3 mCameraPosition;
  private EffectParameter mCameraPositionParameter;
  private Vector2 mHalfPixel;
  private EffectParameter mNormalMapParameter;
  private Texture mNormalMap;
  private EffectParameter mDepthMapParameter;
  private Texture mDepthMap;

  public CapsuleLightEffect(GraphicsDevice iDevice, ContentManager iContentManager)
    : base(iDevice, iContentManager.Load<Effect>("Shaders/CapsuleLight"))
  {
    this.mWorldParameter = this.Parameters[nameof (World)];
    this.mViewProjectionParameter = this.Parameters[nameof (ViewProjection)];
    this.mInverseViewProjectionParameter = this.Parameters[nameof (InverseViewProjection)];
    this.mLengthParameter = this.Parameters[nameof (Length)];
    this.mRadiusParameter = this.Parameters[nameof (Radius)];
    this.mStartParameter = this.Parameters[nameof (Start)];
    this.mEndParameter = this.Parameters[nameof (End)];
    this.mDiffuseColorParameter = this.Parameters[nameof (DiffuseColor)];
    this.mAmbientColorParameter = this.Parameters[nameof (AmbientColor)];
    this.mCameraPositionParameter = this.Parameters[nameof (CameraPosition)];
    this.mHalfPixelParameter = this.Parameters[nameof (HalfPixel)];
    this.mNormalMapParameter = this.Parameters[nameof (NormalMap)];
    this.mDepthMapParameter = this.Parameters[nameof (DepthMap)];
  }

  public Matrix World
  {
    get => this.mWorld;
    set
    {
      this.mWorld = value;
      this.mWorldParameter.SetValue(value);
    }
  }

  public Matrix ViewProjection
  {
    get => this.mViewProjection;
    set
    {
      this.mViewProjection = value;
      this.mViewProjectionParameter.SetValue(value);
    }
  }

  public Matrix InverseViewProjection
  {
    get => this.mInverseViewProjection;
    set
    {
      this.mInverseViewProjection = value;
      this.mInverseViewProjectionParameter.SetValue(value);
    }
  }

  public float Length
  {
    get => this.mLength;
    set
    {
      this.mLength = value;
      this.mLengthParameter.SetValue(value);
    }
  }

  public float Radius
  {
    get => this.mRadius;
    set
    {
      this.mRadius = value;
      this.mRadiusParameter.SetValue(value);
    }
  }

  public Vector3 Start
  {
    get => this.mStart;
    set
    {
      this.mStart = value;
      this.mStartParameter.SetValue(value);
    }
  }

  public Vector3 End
  {
    get => this.mEnd;
    set
    {
      this.mEnd = value;
      this.mEndParameter.SetValue(value);
    }
  }

  public Vector3 DiffuseColor
  {
    get => this.mDiffuseColor;
    set
    {
      this.mDiffuseColor = value;
      this.mDiffuseColorParameter.SetValue(value);
    }
  }

  public Vector3 AmbientColor
  {
    get => this.mAmbientColor;
    set
    {
      this.mAmbientColor = value;
      this.mAmbientColorParameter.SetValue(value);
    }
  }

  public Vector2 HalfPixel
  {
    get => this.mHalfPixel;
    set
    {
      this.mHalfPixel = value;
      this.mHalfPixelParameter.SetValue(value);
    }
  }

  public Vector3 CameraPosition
  {
    get => this.mCameraPosition;
    set
    {
      this.mCameraPosition = value;
      this.mCameraPositionParameter.SetValue(value);
    }
  }

  public Texture NormalMap
  {
    get => this.mNormalMap;
    set
    {
      this.mNormalMap = value;
      this.mNormalMapParameter.SetValue(value);
    }
  }

  public Texture DepthMap
  {
    get => this.mDepthMap;
    set
    {
      this.mDepthMap = value;
      this.mDepthMapParameter.SetValue(value);
    }
  }
}
