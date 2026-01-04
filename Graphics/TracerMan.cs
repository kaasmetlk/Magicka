// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.TracerMan
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using PolygonHead.ParticleEffects;
using System.Collections.Generic;

#nullable disable
namespace Magicka.Graphics;

internal class TracerMan
{
  private static TracerMan sSingelton;
  private static volatile object sSingeltonLock = new object();
  private List<float> mDelays = new List<float>(32 /*0x20*/);
  private List<Particle> mTracers = new List<Particle>(32 /*0x20*/);
  private Particle mTracerTemplate;

  public static TracerMan Instance
  {
    get
    {
      if (TracerMan.sSingelton == null)
      {
        lock (TracerMan.sSingeltonLock)
        {
          if (TracerMan.sSingelton == null)
            TracerMan.sSingelton = new TracerMan();
        }
      }
      return TracerMan.sSingelton;
    }
  }

  private TracerMan()
  {
    this.mTracerTemplate.Drag = 1f;
    this.mTracerTemplate.Gravity = 0.0f;
    this.mTracerTemplate.Rotation = float.NaN;
    this.mTracerTemplate.RotationVelocity = float.NaN;
    this.mTracerTemplate.Drag = 1f;
    this.mTracerTemplate.Color = Vector4.One;
    this.mTracerTemplate.AlphaBlended = false;
    this.mTracerTemplate.Colorize = false;
  }

  public void Clear()
  {
    this.mDelays.Clear();
    this.mTracers.Clear();
  }

  public void AddTracer(
    ref Vector3 iSourcePos,
    ref Vector3 iTargetPos,
    float iVelocity,
    float iRadius,
    byte iSprite)
  {
    this.AddTracer(ref iSourcePos, ref iTargetPos, iVelocity, iRadius, iSprite, 0.0f);
  }

  public void AddTracer(
    ref Vector3 iSourcePos,
    ref Vector3 iTargetPos,
    float iVelocity,
    float iRadius,
    byte iSprite,
    float iDelay)
  {
    this.mTracerTemplate.Sprite = iSprite;
    Vector3 result1;
    Vector3.Subtract(ref iTargetPos, ref iSourcePos, out result1);
    float num = result1.Length();
    Vector3 result2;
    Vector3.Divide(ref result1, num, out result2);
    Vector3 result3;
    Vector3.Multiply(ref result2, iRadius, out result3);
    this.mTracerTemplate.StartSize = iRadius;
    this.mTracerTemplate.EndSize = iRadius;
    this.mTracerTemplate.TTL = (num - iRadius * 2f) / iVelocity;
    Vector3.Add(ref iSourcePos, ref result3, out this.mTracerTemplate.Position);
    Vector3.Multiply(ref result2, iVelocity, out this.mTracerTemplate.Velocity);
    if ((double) this.mTracerTemplate.TTL <= 0.0)
      return;
    this.mTracers.Add(this.mTracerTemplate);
    this.mDelays.Add(iDelay);
  }

  public void Update(float iDeltaTime)
  {
    for (int index = 0; index < this.mDelays.Count; ++index)
    {
      float num = this.mDelays[index] - iDeltaTime;
      if ((double) num <= 0.0)
      {
        Particle mTracer = this.mTracers[index];
        ParticleSystem.Instance.SpawnParticle(ref mTracer);
        this.mDelays.RemoveAt(index);
        this.mTracers.RemoveAt(index);
        --index;
      }
      else
        this.mDelays[index] = num;
    }
  }
}
