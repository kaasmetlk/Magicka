// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.Effects.GUIHardwareInstancingEffect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

#nullable disable
namespace Magicka.Graphics.Effects;

public class GUIHardwareInstancingEffect : Effect
{
  public const int MAXINSTANCES = 40;
  private EffectTechnique mSpritesTechnique;
  private EffectTechnique mNumbersTechnique;
  private EffectTechnique mHealthbarsTechnique;
  private EffectParameter mTextureOffsetsParameter;
  private EffectParameter mPositionsParameter;
  private EffectParameter mScalesParameter;
  private EffectParameter mValuesParameter;
  private EffectParameter mColorsParameter;
  private EffectParameter mSaturationParameter;
  private EffectParameter mDigitWidthParameter;
  private EffectParameter mTransformToScreenParameter;
  private EffectParameter mTextureParameter;
  private EffectParameter mWorldPositionsParameter;
  private EffectParameter mWorldToScreenParameter;
  private EffectParameter mScreenRectangleParameter;

  public GUIHardwareInstancingEffect(GraphicsDevice iDevice, ContentManager iContentManager)
    : base(iDevice, iContentManager.Load<Effect>("shaders/GUIHardwareInstancing"))
  {
    this.mSpritesTechnique = this.Techniques["Sprites"];
    this.mNumbersTechnique = this.Techniques["Numbers"];
    this.mHealthbarsTechnique = this.Techniques["Healthbars"];
    this.mTextureOffsetsParameter = this.Parameters[nameof (TextureOffsets)];
    this.mPositionsParameter = this.Parameters[nameof (Positions)];
    this.mScalesParameter = this.Parameters[nameof (Scales)];
    this.mValuesParameter = this.Parameters[nameof (Values)];
    this.mColorsParameter = this.Parameters[nameof (Colors)];
    this.mSaturationParameter = this.Parameters[nameof (Saturations)];
    this.mDigitWidthParameter = this.Parameters[nameof (DigitWidth)];
    this.mTransformToScreenParameter = this.Parameters["TransformToScreen"];
    this.mTextureParameter = this.Parameters[nameof (Texture)];
    this.mWorldPositionsParameter = this.Parameters[nameof (WorldPositions)];
    this.mWorldToScreenParameter = this.Parameters[nameof (WorldToScreen)];
    this.mScreenRectangleParameter = this.Parameters[nameof (ScreenSize)];
  }

  public void SetTechnique(GUIHardwareInstancingEffect.Technique iTechnique)
  {
    switch (iTechnique)
    {
      case GUIHardwareInstancingEffect.Technique.Sprites:
        this.CurrentTechnique = this.mSpritesTechnique;
        break;
      case GUIHardwareInstancingEffect.Technique.Numbers:
        this.CurrentTechnique = this.mNumbersTechnique;
        break;
      case GUIHardwareInstancingEffect.Technique.Healthbars:
        this.CurrentTechnique = this.mHealthbarsTechnique;
        break;
    }
  }

  public void SetScreenSize(int iWidth, int iHeight)
  {
    Matrix oTransform;
    RenderManager.Instance.CreateTransformPixelsToScreen((float) iWidth, (float) iHeight, out oTransform);
    this.mTransformToScreenParameter.SetValue(oTransform);
  }

  public Texture Texture
  {
    get => (Texture) this.mTextureParameter.GetValueTexture2D();
    set => this.mTextureParameter.SetValue(value);
  }

  public Vector3[] WorldPositions
  {
    get => this.mWorldPositionsParameter.GetValueVector3Array(40);
    set => this.mWorldPositionsParameter.SetValue(value);
  }

  public Point ScreenSize
  {
    get
    {
      Point screenSize = new Point();
      Vector2 valueVector2 = this.mScreenRectangleParameter.GetValueVector2();
      screenSize.X = (int) valueVector2.X;
      screenSize.Y = (int) valueVector2.Y;
      return screenSize;
    }
    set
    {
      this.mScreenRectangleParameter.SetValue(new Vector2()
      {
        X = (float) value.X,
        Y = (float) value.Y
      });
    }
  }

  public Matrix WorldToScreen
  {
    get => this.mWorldToScreenParameter.GetValueMatrix();
    set => this.mWorldToScreenParameter.SetValue(value);
  }

  public float[] Saturations
  {
    get => this.mSaturationParameter.GetValueSingleArray(40);
    set => this.mSaturationParameter.SetValue(value);
  }

  public Vector2[] TextureOffsets
  {
    get => this.mTextureOffsetsParameter.GetValueVector2Array(40);
    set => this.mTextureOffsetsParameter.SetValue(value);
  }

  public Vector2[] Positions
  {
    get => this.mPositionsParameter.GetValueVector2Array(40);
    set => this.mPositionsParameter.SetValue(value);
  }

  public Vector3[] Scales
  {
    get => this.mScalesParameter.GetValueVector3Array(40);
    set => this.mScalesParameter.SetValue(value);
  }

  public float[] Values
  {
    get => this.mValuesParameter.GetValueSingleArray(40);
    set => this.mValuesParameter.SetValue(value);
  }

  public Vector4[] Colors
  {
    get => this.mColorsParameter.GetValueVector4Array(40);
    set => this.mColorsParameter.SetValue(value);
  }

  public float DigitWidth
  {
    get => this.mDigitWidthParameter.GetValueSingle();
    set => this.mDigitWidthParameter.SetValue(value);
  }

  public enum Technique
  {
    Sprites,
    Numbers,
    Healthbars,
  }
}
