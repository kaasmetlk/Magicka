// Decompiled with JetBrains decompiler
// Type: Win32.PerfTimer
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

#nullable disable
namespace Win32;

internal class PerfTimer
{
  private long mStartTime;
  private long mStopTime;
  private long mFreq;
  private double mPausedTimer;
  private bool mRunning;
  private bool mPaused;

  [DllImport("Kernel32.dll")]
  private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

  [DllImport("Kernel32.dll")]
  private static extern bool QueryPerformanceFrequency(out long lpFrequency);

  public PerfTimer()
  {
    this.mStartTime = 0L;
    this.mStopTime = 0L;
    this.mRunning = false;
    if (!PerfTimer.QueryPerformanceFrequency(out this.mFreq))
      throw new Win32Exception();
  }

  public void Start()
  {
    if (this.mRunning)
      return;
    Thread.Sleep(0);
    PerfTimer.QueryPerformanceCounter(out this.mStartTime);
    this.mRunning = true;
  }

  public void Stop()
  {
    PerfTimer.QueryPerformanceCounter(out this.mStopTime);
    this.mRunning = false;
  }

  public void Pause()
  {
    PerfTimer.QueryPerformanceCounter(out this.mStopTime);
    this.mPaused = true;
    this.mPausedTimer += (double) (this.mStopTime - this.mStartTime) / (double) this.mFreq;
  }

  public bool Running => this.mRunning;

  public double Duration
  {
    get
    {
      double duration = (double) (this.mStopTime - this.mStartTime) / (double) this.mFreq + this.mPausedTimer;
      if (!this.mRunning)
        this.mPausedTimer = 0.0;
      return duration;
    }
  }
}
