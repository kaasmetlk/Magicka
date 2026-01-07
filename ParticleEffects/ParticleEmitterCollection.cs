// Decompiled with JetBrains decompiler
// Type: PolygonHead.ParticleEffects.ParticleEmitterCollection
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using System.Collections.Generic;

#nullable disable
namespace PolygonHead.ParticleEffects;

public class ParticleEmitterCollection : List<IParticleEmitter>
{
  public void Update(
    float iDeltaTime,
    float iPreviousTotalTime,
    float iTotalTime,
    float iTimeLinePosition,
    float iPreviousTimeLinePosition,
    ref Matrix iTransform,
    ref Matrix iPreviousTransform)
  {
    foreach (IParticleEmitter particleEmitter in (List<IParticleEmitter>) this)
      particleEmitter.Update(iDeltaTime, iPreviousTotalTime, iTotalTime, iTimeLinePosition, iPreviousTimeLinePosition, ref iTransform, ref iPreviousTransform);
  }
}
