using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Magicka.GameLogic.Controls
{
	// Token: 0x02000189 RID: 393
	internal class InputMessageFilter : IMessageFilter
	{
		// Token: 0x06000BEE RID: 3054
		[DllImport("user32.dll", EntryPoint = "TranslateMessage")]
		protected static extern bool _TranslateMessage(ref Message m);

		// Token: 0x06000BEF RID: 3055
		[DllImport("user32.dll")]
		public static extern uint MapVirtualKey(uint code, uint mapType);

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000BF0 RID: 3056 RVA: 0x00047FA5 File Offset: 0x000461A5
		// (remove) Token: 0x06000BF1 RID: 3057 RVA: 0x00047FBE File Offset: 0x000461BE
		public event Action<KeyData> KeyDown;

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06000BF2 RID: 3058 RVA: 0x00047FD7 File Offset: 0x000461D7
		// (remove) Token: 0x06000BF3 RID: 3059 RVA: 0x00047FF0 File Offset: 0x000461F0
		public event Action<char, KeyModifiers> KeyPress;

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x06000BF4 RID: 3060 RVA: 0x00048009 File Offset: 0x00046209
		// (remove) Token: 0x06000BF5 RID: 3061 RVA: 0x00048022 File Offset: 0x00046222
		public event Action<KeyData> KeyUp;

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x06000BF6 RID: 3062 RVA: 0x0004803B File Offset: 0x0004623B
		// (remove) Token: 0x06000BF7 RID: 3063 RVA: 0x00048054 File Offset: 0x00046254
		public event Action<MouseButtons, Microsoft.Xna.Framework.Point> MouseMove;

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x06000BF8 RID: 3064 RVA: 0x0004806D File Offset: 0x0004626D
		// (remove) Token: 0x06000BF9 RID: 3065 RVA: 0x00048086 File Offset: 0x00046286
		public event Action<MouseButtons, Microsoft.Xna.Framework.Point> MouseLeave;

		// Token: 0x14000008 RID: 8
		// (add) Token: 0x06000BFA RID: 3066 RVA: 0x0004809F File Offset: 0x0004629F
		// (remove) Token: 0x06000BFB RID: 3067 RVA: 0x000480B8 File Offset: 0x000462B8
		public event Action<MouseButtons, Microsoft.Xna.Framework.Point> MouseDown;

		// Token: 0x14000009 RID: 9
		// (add) Token: 0x06000BFC RID: 3068 RVA: 0x000480D1 File Offset: 0x000462D1
		// (remove) Token: 0x06000BFD RID: 3069 RVA: 0x000480EA File Offset: 0x000462EA
		public event Action<MouseButtons, Microsoft.Xna.Framework.Point> MouseUp;

		// Token: 0x1400000A RID: 10
		// (add) Token: 0x06000BFE RID: 3070 RVA: 0x00048103 File Offset: 0x00046303
		// (remove) Token: 0x06000BFF RID: 3071 RVA: 0x0004811C File Offset: 0x0004631C
		public event Action<int, Microsoft.Xna.Framework.Point> MouseScroll;

		// Token: 0x170002D4 RID: 724
		// (get) Token: 0x06000C00 RID: 3072 RVA: 0x00048135 File Offset: 0x00046335
		// (set) Token: 0x06000C01 RID: 3073 RVA: 0x0004813D File Offset: 0x0004633D
		public bool Enabled { get; set; }

		// Token: 0x170002D5 RID: 725
		// (get) Token: 0x06000C02 RID: 3074 RVA: 0x00048146 File Offset: 0x00046346
		// (set) Token: 0x06000C03 RID: 3075 RVA: 0x0004814E File Offset: 0x0004634E
		public bool TranslateMessage { get; set; }

		// Token: 0x06000C04 RID: 3076 RVA: 0x00048160 File Offset: 0x00046360
		public InputMessageFilter()
		{
			Game.Instance.Form.BeginInvoke(new Action(delegate()
			{
				Application.AddMessageFilter(this);
			}));
		}

		// Token: 0x06000C05 RID: 3077 RVA: 0x00048198 File Offset: 0x00046398
		bool IMessageFilter.PreFilterMessage(ref Message m)
		{
			if (!Game.Instance.Form.Focused)
			{
				return false;
			}
			InputMessageFilter.Wm msg = (InputMessageFilter.Wm)m.Msg;
			if (msg <= InputMessageFilter.Wm.SysKeyUp)
			{
				if (msg != InputMessageFilter.Wm.Active)
				{
					switch (msg)
					{
					case InputMessageFilter.Wm.KeyDown:
					case InputMessageFilter.Wm.SysKeyDown:
						if ((m.LParam.ToInt32() & 1073741824) == 0)
						{
							KeyData obj;
							switch ((int)m.WParam)
							{
							case 16:
								if ((m.LParam.ToInt32() & 16777216) == 0)
								{
									obj = new KeyData(Microsoft.Xna.Framework.Input.Keys.LeftShift, this.mModifier);
									this.mModifier |= KeyModifiers.LeftShift;
								}
								else
								{
									obj = new KeyData(Microsoft.Xna.Framework.Input.Keys.RightShift, this.mModifier);
									this.mModifier |= KeyModifiers.RightShift;
								}
								break;
							case 17:
								if ((m.LParam.ToInt32() & 16777216) == 0)
								{
									obj = new KeyData(Microsoft.Xna.Framework.Input.Keys.LeftControl, this.mModifier);
									this.mModifier |= KeyModifiers.LeftControl;
								}
								else
								{
									obj = new KeyData(Microsoft.Xna.Framework.Input.Keys.RightControl, this.mModifier);
									this.mModifier |= KeyModifiers.RightControl;
								}
								break;
							case 18:
								if ((m.LParam.ToInt32() & 16777216) == 0)
								{
									obj = new KeyData(Microsoft.Xna.Framework.Input.Keys.LeftAlt, this.mModifier);
									this.mModifier |= KeyModifiers.LeftAlt;
								}
								else
								{
									obj = new KeyData(Microsoft.Xna.Framework.Input.Keys.RightAlt, this.mModifier);
									this.mModifier |= KeyModifiers.RightAlt;
								}
								break;
							default:
								obj = new KeyData((Microsoft.Xna.Framework.Input.Keys)((int)m.WParam), this.mModifier);
								break;
							}
							if (this.KeyDown != null)
							{
								this.KeyDown(obj);
							}
							if (obj.Key == Microsoft.Xna.Framework.Input.Keys.F4 & (obj.Modifier & KeyModifiers.Alt) != KeyModifiers.None)
							{
								return false;
							}
						}
						if (this.TranslateMessage)
						{
							InputMessageFilter._TranslateMessage(ref m);
						}
						return true;
					case InputMessageFilter.Wm.KeyUp:
					case InputMessageFilter.Wm.SysKeyUp:
					{
						KeyData obj;
						switch ((int)m.WParam)
						{
						case 16:
							if ((m.LParam.ToInt32() & 16777216) == 0)
							{
								this.mModifier &= ~KeyModifiers.LeftShift;
								obj = new KeyData(Microsoft.Xna.Framework.Input.Keys.LeftShift, this.mModifier);
							}
							else
							{
								this.mModifier &= ~KeyModifiers.RightShift;
								obj = new KeyData(Microsoft.Xna.Framework.Input.Keys.RightShift, this.mModifier);
							}
							break;
						case 17:
							if ((m.LParam.ToInt32() & 16777216) == 0)
							{
								this.mModifier &= ~KeyModifiers.LeftControl;
								obj = new KeyData(Microsoft.Xna.Framework.Input.Keys.LeftControl, this.mModifier);
							}
							else
							{
								this.mModifier &= ~KeyModifiers.RightControl;
								obj = new KeyData(Microsoft.Xna.Framework.Input.Keys.RightControl, this.mModifier);
							}
							break;
						case 18:
							if ((m.LParam.ToInt32() & 16777216) == 0)
							{
								this.mModifier &= ~KeyModifiers.LeftAlt;
								obj = new KeyData(Microsoft.Xna.Framework.Input.Keys.LeftAlt, this.mModifier);
							}
							else
							{
								this.mModifier &= ~KeyModifiers.RightAlt;
								obj = new KeyData(Microsoft.Xna.Framework.Input.Keys.RightAlt, this.mModifier);
							}
							break;
						default:
							obj = new KeyData((Microsoft.Xna.Framework.Input.Keys)((int)m.WParam), this.mModifier);
							break;
						}
						if (this.KeyUp != null)
						{
							this.KeyUp(obj);
						}
						return true;
					}
					case InputMessageFilter.Wm.Char:
					{
						char c = (char)((int)m.WParam);
						if (c >= ' ' || c == '\n' || c == '\r' || c == '\b')
						{
							if (c == '\r')
							{
								c = '\n';
							}
							if (this.KeyPress != null)
							{
								this.KeyPress.Invoke(c, this.mModifier);
							}
							return true;
						}
						break;
					}
					}
				}
				else if (((int)m.WParam & 65535) == 0)
				{
					this.mModifier = KeyModifiers.None;
				}
			}
			else
			{
				switch (msg)
				{
				case InputMessageFilter.Wm.MouseMove:
					if (this.MouseMove != null)
					{
						this.MouseMove.Invoke(this.GetButtons(m.WParam.ToInt32()), this.GetMousePosition(m.LParam.ToInt32()));
					}
					break;
				case InputMessageFilter.Wm.LeftButtonDown:
					if (this.MouseDown != null)
					{
						this.MouseDown.Invoke(MouseButtons.Left, this.GetMousePosition(m.LParam.ToInt32()));
					}
					break;
				case InputMessageFilter.Wm.LeftButtonUp:
					if (this.MouseUp != null)
					{
						this.MouseUp.Invoke(MouseButtons.Left, this.GetMousePosition(m.LParam.ToInt32()));
					}
					break;
				case (InputMessageFilter.Wm)515:
				case (InputMessageFilter.Wm)518:
				case (InputMessageFilter.Wm)521:
					break;
				case InputMessageFilter.Wm.RightButtonDown:
					if (this.MouseDown != null)
					{
						this.MouseDown.Invoke(MouseButtons.Right, this.GetMousePosition(m.LParam.ToInt32()));
					}
					break;
				case InputMessageFilter.Wm.RightButtonUp:
					if (this.MouseUp != null)
					{
						this.MouseUp.Invoke(MouseButtons.Right, this.GetMousePosition(m.LParam.ToInt32()));
					}
					break;
				case InputMessageFilter.Wm.MiddleButtonDown:
					if (this.MouseDown != null)
					{
						this.MouseDown.Invoke(MouseButtons.Middle, this.GetMousePosition(m.LParam.ToInt32()));
					}
					break;
				case InputMessageFilter.Wm.MiddleButtonUp:
					if (this.MouseUp != null)
					{
						this.MouseUp.Invoke(MouseButtons.Middle, this.GetMousePosition(m.LParam.ToInt32()));
					}
					break;
				case InputMessageFilter.Wm.MouseWheel:
					if (this.MouseScroll != null)
					{
						Microsoft.Xna.Framework.Point mousePosition = this.GetMousePosition(m.LParam.ToInt32());
						System.Drawing.Point point = Game.Instance.Form.PointToClient(new System.Drawing.Point(mousePosition.X, mousePosition.Y));
						Microsoft.Xna.Framework.Point point2 = new Microsoft.Xna.Framework.Point(point.X, point.Y);
						this.MouseScroll.Invoke((int)this.GetWheelDelta(m.WParam.ToInt32()), point2);
					}
					break;
				default:
					if (msg == InputMessageFilter.Wm.MouseLeave)
					{
						if (this.MouseLeave != null)
						{
							this.MouseLeave.Invoke(this.GetButtons(m.WParam.ToInt32()), this.GetMousePosition(m.LParam.ToInt32()));
						}
					}
					break;
				}
			}
			return false;
		}

		// Token: 0x06000C06 RID: 3078 RVA: 0x00048820 File Offset: 0x00046A20
		private MouseButtons GetButtons(int iWParam)
		{
			MouseButtons mouseButtons = MouseButtons.None;
			if ((iWParam & 1) != 0)
			{
				mouseButtons |= MouseButtons.Left;
			}
			if ((iWParam & 16) != 0)
			{
				mouseButtons |= MouseButtons.Middle;
			}
			if ((iWParam & 2) != 0)
			{
				mouseButtons |= MouseButtons.Right;
			}
			if ((iWParam & 32) != 0)
			{
				mouseButtons |= MouseButtons.XButton1;
			}
			if ((iWParam & 64) != 0)
			{
				mouseButtons |= MouseButtons.XButton2;
			}
			return mouseButtons;
		}

		// Token: 0x06000C07 RID: 3079 RVA: 0x00048874 File Offset: 0x00046A74
		private Microsoft.Xna.Framework.Point GetMousePosition(int iLParam)
		{
			Microsoft.Xna.Framework.Point result;
			result.X = (int)((ushort)iLParam);
			result.Y = (int)((ushort)(iLParam >> 16));
			return result;
		}

		// Token: 0x06000C08 RID: 3080 RVA: 0x00048897 File Offset: 0x00046A97
		private short GetWheelDelta(int iWParam)
		{
			return (short)(iWParam >> 16);
		}

		// Token: 0x04000AFE RID: 2814
		private KeyModifiers mModifier;

		// Token: 0x0200018A RID: 394
		public class Stack
		{
			// Token: 0x170002D6 RID: 726
			// (get) Token: 0x06000C0A RID: 3082 RVA: 0x0004889E File Offset: 0x00046A9E
			public int Capacity
			{
				get
				{
					return this.mStack.Length;
				}
			}

			// Token: 0x170002D7 RID: 727
			// (get) Token: 0x06000C0B RID: 3083 RVA: 0x000488A8 File Offset: 0x00046AA8
			// (set) Token: 0x06000C0C RID: 3084 RVA: 0x000488B0 File Offset: 0x00046AB0
			public int Count { get; private set; }

			// Token: 0x06000C0D RID: 3085 RVA: 0x000488B9 File Offset: 0x00046AB9
			public Stack() : this(32)
			{
			}

			// Token: 0x06000C0E RID: 3086 RVA: 0x000488C3 File Offset: 0x00046AC3
			public Stack(int capacity)
			{
				if (capacity < 0)
				{
					capacity = 0;
				}
				this.mStack = new KeyData[capacity];
			}

			// Token: 0x06000C0F RID: 3087 RVA: 0x000488E0 File Offset: 0x00046AE0
			public void Push(ref KeyData item)
			{
				if (this.Count == this.mStack.Length)
				{
					KeyData[] destinationArray = new KeyData[this.mStack.Length << 1];
					Array.Copy(this.mStack, 0, destinationArray, 0, this.mStack.Length);
					this.mStack = destinationArray;
				}
				this.mStack[this.Count] = item;
				this.Count++;
			}

			// Token: 0x06000C10 RID: 3088 RVA: 0x00048954 File Offset: 0x00046B54
			public void Pop(out KeyData item)
			{
				if (this.Count <= 0)
				{
					throw new InvalidOperationException("The stack is empty!");
				}
				item = this.mStack[this.Count];
				this.Count--;
			}

			// Token: 0x06000C11 RID: 3089 RVA: 0x00048994 File Offset: 0x00046B94
			public void PopSegment(out ArraySegment<KeyData> segment)
			{
				segment = new ArraySegment<KeyData>(this.mStack, 0, this.Count);
				this.Count = 0;
			}

			// Token: 0x04000B01 RID: 2817
			private KeyData[] mStack;
		}

		// Token: 0x0200018B RID: 395
		protected enum Wm
		{
			// Token: 0x04000B04 RID: 2820
			Active = 6,
			// Token: 0x04000B05 RID: 2821
			Char = 258,
			// Token: 0x04000B06 RID: 2822
			KeyDown = 256,
			// Token: 0x04000B07 RID: 2823
			KeyUp,
			// Token: 0x04000B08 RID: 2824
			SysKeyDown = 260,
			// Token: 0x04000B09 RID: 2825
			SysKeyUp,
			// Token: 0x04000B0A RID: 2826
			MouseMove = 512,
			// Token: 0x04000B0B RID: 2827
			MouseLeave = 675,
			// Token: 0x04000B0C RID: 2828
			LeftButtonDown = 513,
			// Token: 0x04000B0D RID: 2829
			LeftButtonUp,
			// Token: 0x04000B0E RID: 2830
			RightButtonDown = 516,
			// Token: 0x04000B0F RID: 2831
			RightButtonUp,
			// Token: 0x04000B10 RID: 2832
			MiddleButtonDown = 519,
			// Token: 0x04000B11 RID: 2833
			MiddleButtonUp,
			// Token: 0x04000B12 RID: 2834
			MouseWheel = 522
		}

		// Token: 0x0200018C RID: 396
		protected enum Wa
		{
			// Token: 0x04000B14 RID: 2836
			Inactive,
			// Token: 0x04000B15 RID: 2837
			Active,
			// Token: 0x04000B16 RID: 2838
			ClickActive
		}

		// Token: 0x0200018D RID: 397
		protected enum Vk
		{
			// Token: 0x04000B18 RID: 2840
			Alt = 18,
			// Token: 0x04000B19 RID: 2841
			Control = 17,
			// Token: 0x04000B1A RID: 2842
			Shift = 16
		}
	}
}
