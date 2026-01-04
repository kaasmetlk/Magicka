// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.EffectManager
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using PolygonHead;
using PolygonHead.ParticleEffects;
using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace Magicka.Graphics;

public sealed class EffectManager
{
  public const int MAXEFFECTS = 256 /*0x0100*/;
  private static EffectManager mSingelton;
  private static volatile object mSingeltonLock = new object();
  private Random mRandomizer = new Random();
  private Dictionary<int, VisualEffect> mSourceEffects;
  private VisualEffect[] mEffects;
  private int[] mUniqueIDs;
  private IntHeap mFreeEffects;
  private int mLastActive;

  public static EffectManager Instance
  {
    get
    {
      if (EffectManager.mSingelton == null)
      {
        lock (EffectManager.mSingeltonLock)
        {
          if (EffectManager.mSingelton == null)
            EffectManager.mSingelton = new EffectManager();
        }
      }
      return EffectManager.mSingelton;
    }
  }

  private EffectManager()
  {
    this.mSourceEffects = new Dictionary<int, VisualEffect>();
    this.mEffects = new VisualEffect[256 /*0x0100*/];
    this.mUniqueIDs = new int[256 /*0x0100*/];
    this.mFreeEffects = new IntHeap(256 /*0x0100*/);
    for (int iValue = 0; iValue < 256 /*0x0100*/; ++iValue)
    {
      this.mFreeEffects.Push(iValue);
      this.mUniqueIDs[iValue] = int.MinValue;
    }
  }

  private void ReadDirectory(DirectoryInfo iDir)
  {
    foreach (FileInfo file in iDir.GetFiles("*.xml"))
      this.mSourceEffects.Add(Path.GetFileNameWithoutExtension(file.FullName).ToLowerInvariant().GetHashCodeCustom(), VisualEffect.FromFile(file.FullName));
    foreach (DirectoryInfo directory in iDir.GetDirectories())
      this.ReadDirectory(directory);
  }

  public void Initialize() => this.ReadDirectory(new DirectoryInfo("content/Effects"));

  public bool UpdatePositionDirection(
    ref VisualEffectReference iRef,
    ref Vector3 iPosition,
    ref Vector3 iDirection)
  {
    if (iRef.ID < 0 || this.mUniqueIDs[iRef.ID] != iRef.Hash)
    {
      iRef.Hash = -1;
      iRef.ID = -1;
      return false;
    }
    MagickaMath.MakeOrientationMatrix(ref iDirection, out this.mEffects[iRef.ID].Transform);
    this.mEffects[iRef.ID].Transform.Translation = iPosition;
    return true;
  }

  public bool UpdateOrientation(ref VisualEffectReference iRef, ref Matrix iTransform)
  {
    if (iRef.ID < 0 || this.mUniqueIDs[iRef.ID] != iRef.Hash)
    {
      iRef.Hash = 0;
      iRef.ID = -1;
      return false;
    }
    this.mEffects[iRef.ID].Transform = iTransform;
    return true;
  }

  public bool RestartEffect(ref VisualEffectReference iRef)
  {
    if (iRef.ID < 0 || this.mUniqueIDs[iRef.ID] != iRef.Hash)
    {
      iRef.Hash = 0;
      iRef.ID = -1;
      return false;
    }
    this.mEffects[iRef.ID].Reset();
    return true;
  }

  public bool IsActive(ref VisualEffectReference iReference)
  {
    return iReference.ID >= 0 && this.mUniqueIDs[iReference.ID] == iReference.Hash && this.mEffects[iReference.ID].IsActive;
  }

  public void Clear()
  {
    this.mFreeEffects.Clear();
    for (int iValue = 0; iValue < 256 /*0x0100*/; ++iValue)
    {
      this.mEffects[iValue] = new VisualEffect();
      this.mUniqueIDs[iValue] = int.MinValue;
      this.mFreeEffects.Push(iValue);
    }
  }

  public VisualEffect GetEffect(int iHash) => this.mSourceEffects[iHash];

  public bool TryGetEffect(int iHash, out VisualEffect oEffect)
  {
    return this.mSourceEffects.TryGetValue(iHash, out oEffect);
  }

  public bool StartEffect(
    int iHash,
    ref Vector3 iPosition,
    ref Vector3 iDirection,
    out VisualEffectReference oRef)
  {
    Vector3 vector3 = new Vector3();
    Vector3 up = (double) Math.Abs(iDirection.X) >= 1.4012984643248171E-45 || (double) Math.Abs(iDirection.Z) >= 1.4012984643248171E-45 ? Vector3.Up : Vector3.Forward;
    Matrix result;
    Matrix.CreateWorld(ref iPosition, ref iDirection, ref up, out result);
    return this.StartEffect(iHash, ref result, out oRef);
  }

  public bool StartEffect(int iHash, ref Matrix iTransform, out VisualEffectReference oRef)
  {
    oRef.Hash = 0;
    oRef.ID = -1;
    VisualEffect visualEffect;
    if (this.mFreeEffects.IsEmpty || !this.mSourceEffects.TryGetValue(iHash, out visualEffect))
      return false;
    int val1 = this.mFreeEffects.Pop();
    visualEffect.Start(ref iTransform);
    this.mEffects[val1] = visualEffect;
    this.mLastActive = Math.Max(val1, this.mLastActive);
    oRef.ID = val1;
    oRef.Hash = this.mRandomizer.Next(2147483646) + 1;
    this.mUniqueIDs[val1] = oRef.Hash;
    return true;
  }

  public void Stop(ref VisualEffectReference iRef)
  {
    lock (this)
    {
      if (iRef.ID < 0 || this.mUniqueIDs[iRef.ID] != iRef.Hash)
        return;
      this.mUniqueIDs[iRef.ID] = int.MinValue;
      this.mEffects[iRef.ID].Stop();
      this.mFreeEffects.Push(iRef.ID);
      iRef.Hash = 0;
      iRef.ID = -1;
    }
  }

  public void Update(float iDeltaTime)
  {
    for (int mLastActive = this.mLastActive; mLastActive >= 0; --mLastActive)
    {
      if (this.mEffects[mLastActive].IsActive)
      {
        this.mEffects[mLastActive].Update(iDeltaTime);
        if (!this.mEffects[mLastActive].IsActive)
        {
          this.mUniqueIDs[mLastActive] = int.MinValue;
          this.mFreeEffects.Push(mLastActive);
        }
      }
    }
    while (this.mLastActive >= 0 && !this.mEffects[this.mLastActive].IsActive)
      --this.mLastActive;
  }
}
