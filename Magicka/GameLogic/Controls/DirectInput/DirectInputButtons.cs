using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.DirectX.DirectInput;

namespace Magicka.GameLogic.Controls.DirectInput
{
	// Token: 0x02000041 RID: 65
	internal struct DirectInputButtons
	{
		// Token: 0x06000293 RID: 659 RVA: 0x000109D8 File Offset: 0x0000EBD8
		public unsafe DirectInputButtons(Device iDevice, JoystickState iState)
		{
			this = default(DirectInputButtons);
			byte[] buttons = iState.GetButtons();
			fixed (uint* ptr = &this.mButtons.FixedElementField)
			{
				this.NrOfButtons = iDevice.Caps.NumberButtons;
				int num = Math.Max(this.NrOfButtons / 32, 1);
				for (int i = 0; i < num; i++)
				{
					uint num2 = 0U;
					int num3 = Math.Min(this.NrOfButtons - 32 * i, 32);
					for (int j = 0; j < num3; j++)
					{
						num2 |= ((buttons[i * 32 + j] == 0) ? 0U : 1U) << j;
					}
					ptr[i] = num2;
				}
			}
		}

		// Token: 0x1700008A RID: 138
		public unsafe bool this[int index]
		{
			get
			{
				if (index < 0 | index >= this.NrOfButtons)
				{
					return false;
				}
				fixed (uint* ptr = &this.mButtons.FixedElementField)
				{
					return (1U & ptr[index / 32] >> index % 32) != 0U;
				}
			}
		}

		// Token: 0x0400021F RID: 543
		[FixedBuffer(typeof(uint), 8)]
		private DirectInputButtons.<mButtons>e__FixedBuffer1 mButtons;

		// Token: 0x04000220 RID: 544
		public int NrOfButtons;

		// Token: 0x02000042 RID: 66
		[UnsafeValueType]
		[CompilerGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 32)]
		public struct <mButtons>e__FixedBuffer1
		{
			// Token: 0x04000221 RID: 545
			public uint FixedElementField;
		}
	}
}
