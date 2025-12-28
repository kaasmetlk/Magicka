using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.DirectX.DirectInput;

namespace Magicka.GameLogic.Controls.DirectInput
{
	// Token: 0x02000462 RID: 1122
	internal struct DirectInputAxes
	{
		// Token: 0x06002244 RID: 8772 RVA: 0x000F5298 File Offset: 0x000F3498
		public unsafe DirectInputAxes(Device iDevice, JoystickState iState)
		{
			fixed (float* ptr = &this.mAxes.FixedElementField)
			{
				*ptr = ((float)iState.X - 32767.5f) * 3.0518044E-05f;
				if (Math.Abs(*ptr) < DirectInputAxes.DeadZone)
				{
					*ptr = 0f;
				}
				ptr[1] = ((float)iState.Y - 32767.5f) * 3.0518044E-05f;
				if (Math.Abs(ptr[1]) < DirectInputAxes.DeadZone)
				{
					ptr[1] = 0f;
				}
				ptr[2] = ((float)iState.Z - 32767.5f) * 3.0518044E-05f;
				if (Math.Abs(ptr[2]) < DirectInputAxes.DeadZone)
				{
					ptr[2] = 0f;
				}
				ptr[3] = ((float)iState.Rx - 32767.5f) * 3.0518044E-05f;
				if (Math.Abs(ptr[3]) < DirectInputAxes.DeadZone)
				{
					ptr[3] = 0f;
				}
				ptr[4] = ((float)iState.Ry - 32767.5f) * 3.0518044E-05f;
				if (Math.Abs(ptr[4]) < DirectInputAxes.DeadZone)
				{
					ptr[4] = 0f;
				}
				ptr[5] = ((float)iState.Rz - 32767.5f) * 3.0518044E-05f;
				if (Math.Abs(ptr[5]) < DirectInputAxes.DeadZone)
				{
					ptr[5] = 0f;
				}
			}
		}

		// Token: 0x17000833 RID: 2099
		public unsafe float this[int index]
		{
			get
			{
				if (index < 0 | index >= 6)
				{
					return 0f;
				}
				fixed (float* ptr = &this.mAxes.FixedElementField)
				{
					return ptr[index];
				}
			}
		}

		// Token: 0x0400254F RID: 9551
		public const int NR_OF_AXES = 6;

		// Token: 0x04002550 RID: 9552
		private const float CENTER = 32767.5f;

		// Token: 0x04002551 RID: 9553
		private const float INVCENTER = 3.0518044E-05f;

		// Token: 0x04002552 RID: 9554
		public static float DeadZone = 0.1f;

		// Token: 0x04002553 RID: 9555
		[FixedBuffer(typeof(float), 6)]
		private DirectInputAxes.<mAxes>e__FixedBuffer8 mAxes;

		// Token: 0x02000463 RID: 1123
		[UnsafeValueType]
		[CompilerGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 24)]
		public struct <mAxes>e__FixedBuffer8
		{
			// Token: 0x04002554 RID: 9556
			public float FixedElementField;
		}
	}
}
