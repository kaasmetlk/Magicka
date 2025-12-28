using System;

namespace Magicka.GameLogic.Controls.DirectInput
{
	// Token: 0x020005BB RID: 1467
	internal struct DirectInputDPad
	{
		// Token: 0x06002BD5 RID: 11221 RVA: 0x0015A668 File Offset: 0x00158868
		public DirectInputDPad(int direction)
		{
			this.Direction = ControllerDirection.Center;
			if (direction == -1)
			{
				return;
			}
			if (0 < direction & direction < 18000)
			{
				this.Direction |= ControllerDirection.Right;
			}
			if (direction > 27000 | direction < 9000)
			{
				this.Direction |= ControllerDirection.Up;
			}
			if (18000 < direction)
			{
				this.Direction |= ControllerDirection.Left;
			}
			if (9000 < direction & direction < 27000)
			{
				this.Direction |= ControllerDirection.Down;
			}
		}

		// Token: 0x17000A3F RID: 2623
		public bool this[int iIndex]
		{
			get
			{
				return ((int)this.Direction & 1 << iIndex) != 0;
			}
		}

		// Token: 0x04002F91 RID: 12177
		public ControllerDirection Direction;
	}
}
