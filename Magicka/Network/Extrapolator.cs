using System;
using Microsoft.Xna.Framework;

namespace Magicka.Network
{
	// Token: 0x020002C4 RID: 708
	internal class Extrapolator
	{
		// Token: 0x0600158A RID: 5514 RVA: 0x0008A3D8 File Offset: 0x000885D8
		private bool Estimates(float iPacketTime, float iCurrentTime)
		{
			if (iPacketTime <= this.mLastPacketTime)
			{
				return false;
			}
			float num = iCurrentTime - iPacketTime;
			if (num < 0f)
			{
				num = 0f;
			}
			if (num > this.mLatency)
			{
				this.mLatency = (this.mLatency + num) * 0.5f;
			}
			else
			{
				this.mLatency = (this.mLatency * 7f + num) * 0.125f;
			}
			float num2 = iPacketTime - this.mLastPacketTime;
			if (num2 > this.mUpdateTime)
			{
				this.mUpdateTime = (this.mUpdateTime + num2) * 0.5f;
			}
			else
			{
				this.mUpdateTime = (this.mUpdateTime * 7f + num2) * 0.125f;
			}
			return true;
		}

		// Token: 0x0600158B RID: 5515 RVA: 0x0008A47E File Offset: 0x0008867E
		public Extrapolator(int iCount)
		{
		}

		// Token: 0x0600158C RID: 5516 RVA: 0x0008A488 File Offset: 0x00088688
		public bool AddSample(float iPacketTime, float iCurrentTime, ref Vector3 iPosition)
		{
			Vector3 vector = default(Vector3);
			float num = iPacketTime - this.mLastPacketTime;
			if (num > 1E-06f)
			{
				Vector3.Subtract(ref iPosition, ref this.mLastPacketPos, out vector);
				Vector3.Divide(ref vector, num, out vector);
			}
			return this.AddSample(iPacketTime, iCurrentTime, ref iPosition, ref vector);
		}

		// Token: 0x0600158D RID: 5517 RVA: 0x0008A4D4 File Offset: 0x000886D4
		public bool AddSample(float iPacketTime, float iCurrentTime, ref Vector3 iPosition, ref Vector3 iVelocity)
		{
			if (!this.Estimates(iPacketTime, iCurrentTime))
			{
				return false;
			}
			this.mLastPacketPos = iPosition;
			this.mLastPacketTime = iPacketTime;
			this.ReadPosition(iCurrentTime, out this.mSnapPos);
			this.mAimTime = iCurrentTime + this.mUpdateTime;
			float scaleFactor = this.mAimTime - iPacketTime;
			this.mSnapTime = iCurrentTime;
			Vector3.Multiply(ref iVelocity, scaleFactor, out this.mAimPos);
			Vector3.Add(ref iPosition, ref this.mAimPos, out this.mAimPos);
			if (Math.Abs(this.mAimTime - this.mSnapTime) < 1E-06f)
			{
				this.mSnapVel = iVelocity;
			}
			else
			{
				scaleFactor = 1f / (this.mAimTime - this.mSnapTime);
				Vector3.Subtract(ref this.mAimPos, ref this.mSnapPos, out this.mSnapVel);
				Vector3.Multiply(ref this.mSnapVel, scaleFactor, out this.mSnapVel);
			}
			return true;
		}

		// Token: 0x0600158E RID: 5518 RVA: 0x0008A5B4 File Offset: 0x000887B4
		public bool ReadPosition(float iForTime, out Vector3 oPosition)
		{
			Vector3 vector;
			return this.ReadPosition(iForTime, out oPosition, out vector);
		}

		// Token: 0x0600158F RID: 5519 RVA: 0x0008A5CC File Offset: 0x000887CC
		public bool ReadPosition(float iForTime, out Vector3 oPosition, out Vector3 oVelocity)
		{
			bool flag = true;
			if (iForTime < this.mSnapTime)
			{
				iForTime = this.mSnapTime;
				flag = false;
			}
			float num = this.mAimTime + this.mUpdateTime;
			if (iForTime > num)
			{
				iForTime = num;
				flag = false;
			}
			oVelocity = this.mSnapVel;
			Vector3.Multiply(ref oVelocity, iForTime - this.mSnapTime, out oPosition);
			Vector3.Add(ref this.mSnapPos, ref oPosition, out oPosition);
			if (!flag)
			{
				oVelocity = default(Vector3);
			}
			return flag;
		}

		// Token: 0x06001590 RID: 5520 RVA: 0x0008A638 File Offset: 0x00088838
		public void Reset(ref Vector3 iPosition)
		{
			this.mLastPacketTime = 0f;
			this.mLastPacketPos = iPosition;
			this.mSnapTime = 0f;
			this.mSnapPos = iPosition;
			this.mUpdateTime = 0f;
			this.mLatency = 0f;
			this.mAimTime = 0f;
			this.mSnapVel = default(Vector3);
			this.mAimPos = iPosition;
		}

		// Token: 0x040016EE RID: 5870
		private Vector3 mSnapPos;

		// Token: 0x040016EF RID: 5871
		private Vector3 mSnapVel;

		// Token: 0x040016F0 RID: 5872
		private Vector3 mAimPos;

		// Token: 0x040016F1 RID: 5873
		private Vector3 mLastPacketPos;

		// Token: 0x040016F2 RID: 5874
		private float mSnapTime;

		// Token: 0x040016F3 RID: 5875
		private float mAimTime;

		// Token: 0x040016F4 RID: 5876
		private float mLastPacketTime;

		// Token: 0x040016F5 RID: 5877
		private float mLatency;

		// Token: 0x040016F6 RID: 5878
		private float mUpdateTime;
	}
}
