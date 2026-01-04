// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.Lights.DynamicLight
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Lights;
using System;
using System.Collections.Generic;
using System.Threading;

#nullable disable
namespace Magicka.Graphics.Lights;

public class DynamicLight : PointLight
{
  private float mDynamicIntensity;
  private float mSpeed = 1f;
  private Vector3 mVelocity;
  private float mFriction;
  private float mTTL = -1f;
  private static WeakReference sScene;
  private static Queue<DynamicLight> sLightCache;

  private DynamicLight()
    : base(Magicka.Game.Instance.GraphicsDevice)
  {
  }

  public static void StopAndClearAll()
  {
    if (DynamicLight.sLightCache == null)
      return;
    foreach (DynamicLight dynamicLight in DynamicLight.sLightCache)
      dynamicLight.Stop(true);
    Thread.Sleep(0);
    DynamicLight.sLightCache.Clear();
  }

  public static void Initialize(Scene iScene)
  {
    DynamicLight.sLightCache = new Queue<DynamicLight>(128 /*0x80*/);
    for (int index = 0; index < 128 /*0x80*/; ++index)
      DynamicLight.sLightCache.Enqueue(new DynamicLight());
    DynamicLight.sScene = new WeakReference((object) iScene);
  }

  public static void DisposeCache()
  {
    lock (DynamicLight.sLightCache)
    {
      for (int index = 0; index < DynamicLight.sLightCache.Count; ++index)
      {
        DynamicLight dynamicLight = DynamicLight.sLightCache.Dequeue();
        dynamicLight.DisposeShadowMap();
        DynamicLight.sLightCache.Enqueue(dynamicLight);
      }
    }
  }

  public static DynamicLight GetCachedLight()
  {
    lock (DynamicLight.sLightCache)
      return DynamicLight.sLightCache.Count > 0 ? DynamicLight.sLightCache.Dequeue() : new DynamicLight();
  }

  public void Initialize() => this.Initialize(Vector3.Zero, Vector3.One, 1f, 1f, 1f, 1f);

  public void Initialize(Vector3 iPosition)
  {
    this.Initialize(iPosition, Vector3.One, 1f, 1f, 1f, 1f);
  }

  public void Initialize(
    Vector3 iPosition,
    Vector3 iColor,
    float iIntensity,
    float iRadius,
    float iSpeed,
    float iSpecularAmount)
  {
    this.Initialize(iPosition, iColor, iIntensity, iRadius, iSpeed, iSpecularAmount, Vector3.Zero, 1f, -1f);
  }

  public void Initialize(
    Vector3 iPosition,
    Vector3 iColor,
    float iIntensity,
    float iRadius,
    float iSpeed,
    float iSpecularAmount,
    float iTTL)
  {
    this.Initialize(iPosition, iColor, iIntensity, iRadius, iSpeed, iSpecularAmount, Vector3.Zero, 1f, iTTL);
  }

  public void Initialize(
    Vector3 iPosition,
    Vector3 iColor,
    float iIntensity,
    float iRadius,
    float iSpeed,
    float iSpecularAmount,
    Vector3 iVelocity,
    float iFriction)
  {
    this.Initialize(iPosition, iColor, iIntensity, iRadius, iSpeed, iSpecularAmount, iVelocity, iFriction, -1f);
  }

  public void Initialize(
    Vector3 iPosition,
    Vector3 iColor,
    float iIntensity,
    float iRadius,
    float iSpeed,
    float iSpecularAmount,
    Vector3 iVelocity,
    float iFriction,
    float iTTL)
  {
    if ((double) iSpeed <= 1.4012984643248171E-45)
      throw new ArgumentException("iSpeed must be greater than zero!", nameof (iSpeed));
    this.Position = iPosition;
    this.mDynamicIntensity = iIntensity;
    this.mSpeed = iSpeed;
    this.DiffuseColor = iColor;
    this.SpecularAmount = iSpecularAmount;
    this.mVelocity = iVelocity;
    this.mFriction = iFriction;
    this.mTTL = iTTL;
    this.Radius = iRadius;
    this.CastShadows = false;
    this.ShadowMapSize = GlobalSettings.Instance.ModShadowResolution(256 /*0x0100*/);
    this.VariationAmount = 0.0f;
    this.VariationSpeed = 0.0f;
    this.VariationType = LightVariationType.None;
  }

  public void Enable() => this.Enable(this.Scene, LightTransitionType.Linear, 1f / this.mSpeed);

  public void Stop(bool Immediate)
  {
    if (Immediate)
      this.Disable();
    else
      this.Disable(LightTransitionType.Linear, 1f / this.mSpeed);
  }

  protected override void Update(
    DataChannel iDataChannel,
    float iDeltaTime,
    ref Vector3 iCameraPosition,
    ref Vector3 iCameraDirection)
  {
    base.Update(iDataChannel, iDeltaTime, ref iCameraPosition, ref iCameraDirection);
    if ((double) this.mTTL != -1.0)
    {
      this.mTTL -= iDeltaTime;
      if ((double) this.mTTL <= 0.0)
        this.Stop(false);
    }
    this.mVelocity *= this.mFriction;
    DynamicLight dynamicLight = this;
    dynamicLight.Position = dynamicLight.Position + this.mVelocity;
  }

  protected override void OnRemove()
  {
    base.OnRemove();
    lock (DynamicLight.sLightCache)
      DynamicLight.sLightCache.Enqueue(this);
  }

  public override void Draw(
    Microsoft.Xna.Framework.Graphics.Effect iEffect,
    DataChannel iDataChannel,
    float iDeltaTime,
    Texture2D iNormalMap,
    Texture2D iDepthMap)
  {
    float mIntensity = this.mIntensity;
    this.mIntensity *= this.mDynamicIntensity;
    base.Draw(iEffect, iDataChannel, iDeltaTime, iNormalMap, iDepthMap);
    this.mIntensity = mIntensity;
  }

  public new Scene Scene => DynamicLight.sScene.Target as Scene;

  public float Friction
  {
    get => this.mFriction;
    set => this.mFriction = value;
  }

  public Vector3 Velocity
  {
    get => this.mVelocity;
    set => this.mVelocity = value;
  }

  public float Intensity
  {
    get => this.mDynamicIntensity;
    set => this.mDynamicIntensity = value;
  }

  public float Speed
  {
    get => this.mSpeed;
    set
    {
      this.mSpeed = (double) value > 1.4012984643248171E-45 ? value : throw new ArgumentException("iSpeed must be greater than zero!", "iSpeed");
    }
  }
}
