using System;
using Magicka.GameLogic.Entities;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.Controls
{
	// Token: 0x0200018F RID: 399
	internal abstract class Controller
	{
		// Token: 0x170002D8 RID: 728
		// (get) Token: 0x06000C12 RID: 3090 RVA: 0x000489B5 File Offset: 0x00046BB5
		// (set) Token: 0x06000C13 RID: 3091 RVA: 0x000489BD File Offset: 0x00046BBD
		public Player Player
		{
			get
			{
				return this.mPlayer;
			}
			set
			{
				this.mPlayer = value;
			}
		}

		// Token: 0x06000C14 RID: 3092 RVA: 0x000489C8 File Offset: 0x00046BC8
		protected ControllerDirection GetDirection(Vector2 iValue)
		{
			float num = iValue.Length();
			if (num < 0.9f)
			{
				return ControllerDirection.Center;
			}
			if (num > 0.95f)
			{
				Vector2.Divide(ref iValue, num, out iValue);
				float num2 = (float)Math.Acos((double)Vector2.Dot(iValue, Vector2.UnitX));
				if (iValue.Y < 0f)
				{
					num2 *= -1f;
				}
				float num3 = Math.Abs(num2);
				if (num3 <= 0.62831855f)
				{
					return ControllerDirection.Right;
				}
				if (num3 >= 2.5132742f)
				{
					return ControllerDirection.Left;
				}
				if (num2 >= 0.9424778f && num2 <= 2.1991148f)
				{
					return ControllerDirection.Up;
				}
				if (num2 <= -0.9424778f && num2 >= -2.1991148f)
				{
					return ControllerDirection.Down;
				}
			}
			return ControllerDirection.Center;
		}

		// Token: 0x06000C15 RID: 3093
		public abstract void Update(DataChannel iDataChannel, float iDeltaTime);

		// Token: 0x06000C16 RID: 3094
		public abstract void Rumble(float iLeft, float iRight);

		// Token: 0x06000C17 RID: 3095
		public abstract float LeftRumble();

		// Token: 0x06000C18 RID: 3096
		public abstract float RightRumble();

		// Token: 0x06000C19 RID: 3097
		public abstract void Clear();

		// Token: 0x06000C1A RID: 3098
		public abstract void Invert(bool iInvert);

		// Token: 0x170002D9 RID: 729
		// (get) Token: 0x06000C1B RID: 3099 RVA: 0x00048A62 File Offset: 0x00046C62
		public bool Inverted
		{
			get
			{
				return this.mInverted;
			}
		}

		// Token: 0x04000B35 RID: 2869
		public const float MOVETIME = 0.2f;

		// Token: 0x04000B36 RID: 2870
		public const float FADETIME = 0.2f;

		// Token: 0x04000B37 RID: 2871
		protected Player mPlayer;

		// Token: 0x04000B38 RID: 2872
		protected Avatar mAvatar;

		// Token: 0x04000B39 RID: 2873
		protected bool mInverted;
	}
}
