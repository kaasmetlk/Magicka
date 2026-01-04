// Decompiled with JetBrains decompiler
// Type: Magicka.Network.Extrapolator
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace Magicka.Network;

internal class Extrapolator
{
  private Vector3 mSnapPos;
  private Vector3 mSnapVel;
  private Vector3 mAimPos;
  private Vector3 mLastPacketPos;
  private float mSnapTime;
  private float mAimTime;
  private float mLastPacketTime;
  private float mLatency;
  private float mUpdateTime;

  private bool Estimates(float iPacketTime, float iCurrentTime)
  {
    if ((double) iPacketTime <= (double) this.mLastPacketTime)
      return false;
    float num1 = iCurrentTime - iPacketTime;
    if ((double) num1 < 0.0)
      num1 = 0.0f;
    this.mLatency = (double) num1 <= (double) this.mLatency ? (float) (((double) this.mLatency * 7.0 + (double) num1) * 0.125) : (float) (((double) this.mLatency + (double) num1) * 0.5);
    float num2 = iPacketTime - this.mLastPacketTime;
    this.mUpdateTime = (double) num2 <= (double) this.mUpdateTime ? (float) (((double) this.mUpdateTime * 7.0 + (double) num2) * 0.125) : (float) (((double) this.mUpdateTime + (double) num2) * 0.5);
    return true;
  }

  public Extrapolator(int iCount)
  {
  }

  public bool AddSample(float iPacketTime, float iCurrentTime, ref Vector3 iPosition)
  {
    Vector3 result = new Vector3();
    float num = iPacketTime - this.mLastPacketTime;
    if ((double) num > 9.9999999747524271E-07)
    {
      Vector3.Subtract(ref iPosition, ref this.mLastPacketPos, out result);
      Vector3.Divide(ref result, num, out result);
    }
    return this.AddSample(iPacketTime, iCurrentTime, ref iPosition, ref result);
  }

  public bool AddSample(
    float iPacketTime,
    float iCurrentTime,
    ref Vector3 iPosition,
    ref Vector3 iVelocity)
  {
    if (!this.Estimates(iPacketTime, iCurrentTime))
      return false;
    this.mLastPacketPos = iPosition;
    this.mLastPacketTime = iPacketTime;
    this.ReadPosition(iCurrentTime, out this.mSnapPos);
    this.mAimTime = iCurrentTime + this.mUpdateTime;
    float scaleFactor1 = this.mAimTime - iPacketTime;
    this.mSnapTime = iCurrentTime;
    Vector3.Multiply(ref iVelocity, scaleFactor1, out this.mAimPos);
    Vector3.Add(ref iPosition, ref this.mAimPos, out this.mAimPos);
    if ((double) Math.Abs(this.mAimTime - this.mSnapTime) < 9.9999999747524271E-07)
    {
      this.mSnapVel = iVelocity;
    }
    else
    {
      float scaleFactor2 = (float) (1.0 / ((double) this.mAimTime - (double) this.mSnapTime));
      Vector3.Subtract(ref this.mAimPos, ref this.mSnapPos, out this.mSnapVel);
      Vector3.Multiply(ref this.mSnapVel, scaleFactor2, out this.mSnapVel);
    }
    return true;
  }

  public bool ReadPosition(float iForTime, out Vector3 oPosition)
  {
    return this.ReadPosition(iForTime, out oPosition, out Vector3 _);
  }

  public bool ReadPosition(float iForTime, out Vector3 oPosition, out Vector3 oVelocity)
  {
    bool flag = true;
    if ((double) iForTime < (double) this.mSnapTime)
    {
      iForTime = this.mSnapTime;
      flag = false;
    }
    float num = this.mAimTime + this.mUpdateTime;
    if ((double) iForTime > (double) num)
    {
      iForTime = num;
      flag = false;
    }
    oVelocity = this.mSnapVel;
    Vector3.Multiply(ref oVelocity, iForTime - this.mSnapTime, out oPosition);
    Vector3.Add(ref this.mSnapPos, ref oPosition, out oPosition);
    if (!flag)
      oVelocity = new Vector3();
    return flag;
  }

  public void Reset(ref Vector3 iPosition)
  {
    this.mLastPacketTime = 0.0f;
    this.mLastPacketPos = iPosition;
    this.mSnapTime = 0.0f;
    this.mSnapPos = iPosition;
    this.mUpdateTime = 0.0f;
    this.mLatency = 0.0f;
    this.mAimTime = 0.0f;
    this.mSnapVel = new Vector3();
    this.mAimPos = iPosition;
  }
}
