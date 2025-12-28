using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace Win32
{
	// Token: 0x020001A4 RID: 420
	internal class PerfTimer
	{
		// Token: 0x06000C69 RID: 3177
		[DllImport("Kernel32.dll")]
		private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

		// Token: 0x06000C6A RID: 3178
		[DllImport("Kernel32.dll")]
		private static extern bool QueryPerformanceFrequency(out long lpFrequency);

		// Token: 0x06000C6B RID: 3179 RVA: 0x0004A8AF File Offset: 0x00048AAF
		public PerfTimer()
		{
			this.mStartTime = 0L;
			this.mStopTime = 0L;
			this.mRunning = false;
			if (!PerfTimer.QueryPerformanceFrequency(out this.mFreq))
			{
				throw new Win32Exception();
			}
		}

		// Token: 0x06000C6C RID: 3180 RVA: 0x0004A8E1 File Offset: 0x00048AE1
		public void Start()
		{
			if (this.mRunning)
			{
				return;
			}
			Thread.Sleep(0);
			PerfTimer.QueryPerformanceCounter(out this.mStartTime);
			this.mRunning = true;
		}

		// Token: 0x06000C6D RID: 3181 RVA: 0x0004A905 File Offset: 0x00048B05
		public void Stop()
		{
			PerfTimer.QueryPerformanceCounter(out this.mStopTime);
			this.mRunning = false;
		}

		// Token: 0x06000C6E RID: 3182 RVA: 0x0004A91A File Offset: 0x00048B1A
		public void Pause()
		{
			PerfTimer.QueryPerformanceCounter(out this.mStopTime);
			this.mPaused = true;
			this.mPausedTimer += (double)(this.mStopTime - this.mStartTime) / (double)this.mFreq;
		}

		// Token: 0x170002E2 RID: 738
		// (get) Token: 0x06000C6F RID: 3183 RVA: 0x0004A952 File Offset: 0x00048B52
		public bool Running
		{
			get
			{
				return this.mRunning;
			}
		}

		// Token: 0x170002E3 RID: 739
		// (get) Token: 0x06000C70 RID: 3184 RVA: 0x0004A95C File Offset: 0x00048B5C
		public double Duration
		{
			get
			{
				double result = (double)(this.mStopTime - this.mStartTime) / (double)this.mFreq + this.mPausedTimer;
				if (!this.mRunning)
				{
					this.mPausedTimer = 0.0;
				}
				return result;
			}
		}

		// Token: 0x04000B6A RID: 2922
		private long mStartTime;

		// Token: 0x04000B6B RID: 2923
		private long mStopTime;

		// Token: 0x04000B6C RID: 2924
		private long mFreq;

		// Token: 0x04000B6D RID: 2925
		private double mPausedTimer;

		// Token: 0x04000B6E RID: 2926
		private bool mRunning;

		// Token: 0x04000B6F RID: 2927
		private bool mPaused;
	}
}
