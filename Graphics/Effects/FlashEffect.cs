// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.Effects.FlashEffect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace Magicka.Graphics.Effects;

public class FlashEffect : Effect
{
  private EffectParameter mColorParam;

  public FlashEffect()
    : base(Magicka.Game.Instance.GraphicsDevice, Magicka.Game.Instance.Content.Load<Effect>("Shaders/FlashEffect"))
  {
    this.mColorParam = this.Parameters[nameof (Color)];
  }

  public Vector4 Color
  {
    get => this.mColorParam.GetValueVector4();
    set => this.mColorParam.SetValue(value);
  }
}
