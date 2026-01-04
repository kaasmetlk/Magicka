// Decompiled with JetBrains decompiler
// Type: Magicka.Physics.PointJoint
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Physics;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace Magicka.Physics;

public class PointJoint : Controller
{
  public PointJoint(Body iBody, Matrix iTransform)
  {
  }

  public override void UpdateController(float dt) => throw new NotImplementedException();
}
