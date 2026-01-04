// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Bosses.JormungandrCollisionZone
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using JigLibX.Math;
using Magicka.GameLogic.GameStates;
using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.GameLogic.Entities.Bosses;

public class JormungandrCollisionZone(
  PlayState iPlayState,
  IBoss iParent,
  params Primitive[] iPrimitives) : BossCollisionZone(iPlayState, iParent, iPrimitives)
{
  public override Vector3 Position
  {
    get
    {
      Transform oTransform;
      this.mCollision.GetPrimitiveLocal(16 /*0x10*/).GetTransform(out oTransform);
      return oTransform.Position with { Y = 1f };
    }
  }
}
