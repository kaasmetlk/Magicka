// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.Effects.MistEffect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead.Effects;

#nullable disable
namespace Magicka.Graphics.Effects;

public class MistEffect(GraphicsDevice iGraphicsDevice, ContentManager iContent) : 
  PostProcessingEffect(iGraphicsDevice, iContent.Load<Effect>("Shaders/Mist"))
{
}
