// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.Effects.BuffEffect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace Magicka.Graphics.Effects;

public class BuffEffect : Effect
{
  public const int MAX_INSTANCES = 32 /*0x20*/;
  public static readonly int TYPEHASH = typeof (BuffEffect).GetHashCode();
  private EffectParameter mPositionsParameter;
  private EffectParameter mColorsParameter;
  private EffectParameter mTextureOffsetsParameter;
  private EffectParameter mTimeParameter;
  private EffectParameter mViewProjectionParameter;
  private EffectParameter mTextureParameter;

  public BuffEffect(GraphicsDevice iDevice, ContentManager iContent)
    : base(iDevice, iContent.Load<Effect>("Shaders/BuffEffect"))
  {
    this.mPositionsParameter = this.Parameters[nameof (Positions)];
    this.mColorsParameter = this.Parameters[nameof (Colors)];
    this.mTextureOffsetsParameter = this.Parameters[nameof (TextureOffsets)];
    this.mTimeParameter = this.Parameters[nameof (Time)];
    this.mViewProjectionParameter = this.Parameters[nameof (ViewProjection)];
    this.mTextureParameter = this.Parameters[nameof (Texture)];
  }

  public Vector4[] Positions
  {
    get => this.mPositionsParameter.GetValueVector4Array(32 /*0x20*/);
    set => this.mPositionsParameter.SetValue(value);
  }

  public Vector4[] Colors
  {
    get => this.mColorsParameter.GetValueVector4Array(32 /*0x20*/);
    set => this.mColorsParameter.SetValue(value);
  }

  public Vector2[] TextureOffsets
  {
    get => this.mTextureOffsetsParameter.GetValueVector2Array(32 /*0x20*/);
    set => this.mTextureOffsetsParameter.SetValue(value);
  }

  public float Time
  {
    get => this.mTimeParameter.GetValueSingle();
    set => this.mTimeParameter.SetValue(value);
  }

  public Matrix ViewProjection
  {
    get => this.mViewProjectionParameter.GetValueMatrix();
    set => this.mViewProjectionParameter.SetValue(value);
  }

  public Texture2D Texture
  {
    get => this.mTextureParameter.GetValueTexture2D();
    set => this.mTextureParameter.SetValue((Microsoft.Xna.Framework.Graphics.Texture) value);
  }
}
