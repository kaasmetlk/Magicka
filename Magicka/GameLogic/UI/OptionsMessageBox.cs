using System;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PolygonHead;

namespace Magicka.GameLogic.UI
{
	// Token: 0x0200014E RID: 334
	internal class OptionsMessageBox : MessageBox
	{
		// Token: 0x14000002 RID: 2
		// (add) Token: 0x060009C9 RID: 2505 RVA: 0x0003B931 File Offset: 0x00039B31
		// (remove) Token: 0x060009CA RID: 2506 RVA: 0x0003B94A File Offset: 0x00039B4A
		public event Action<OptionsMessageBox, int> Select;

		// Token: 0x060009CB RID: 2507 RVA: 0x0003B964 File Offset: 0x00039B64
		public OptionsMessageBox(string iMessage, params string[] iOptions) : base(iMessage)
		{
			this.mMessageHash = 0;
			this.mOptions = new MenuTextItem[iOptions.Length];
			for (int i = 0; i < iOptions.Length; i++)
			{
				MenuTextItem menuTextItem = new MenuTextItem(iOptions[i], default(Vector2), this.mFont, TextAlign.Center);
				this.mOptions[i] = menuTextItem;
			}
			LanguageManager.Instance.LanguageChanged += new Action(this.LanguageChanged);
		}

		// Token: 0x060009CC RID: 2508 RVA: 0x0003B9D4 File Offset: 0x00039BD4
		public OptionsMessageBox(int iMessage, params int[] iOptions) : base(iMessage)
		{
			this.mMessageHash = iMessage;
			this.mOptions = new MenuTextItem[iOptions.Length];
			for (int i = 0; i < iOptions.Length; i++)
			{
				MenuTextItem menuTextItem = new MenuTextItem(iOptions[i], default(Vector2), this.mFont, TextAlign.Center);
				this.mOptions[i] = menuTextItem;
			}
			LanguageManager.Instance.LanguageChanged += new Action(this.LanguageChanged);
		}

		// Token: 0x060009CD RID: 2509 RVA: 0x0003BA44 File Offset: 0x00039C44
		public override void LanguageChanged()
		{
			base.LanguageChanged();
			for (int i = 0; i < this.mOptions.Length; i++)
			{
				this.mOptions[i].LanguageChanged();
			}
			if (this.mMessageHash != 0)
			{
				this.mMessage.SetText(this.mFont.Wrap(LanguageManager.Instance.GetString(this.mMessageHash), (int)(this.mSize.X * 0.9f), true));
			}
		}

		// Token: 0x060009CE RID: 2510 RVA: 0x0003BAB8 File Offset: 0x00039CB8
		public override void OnMove(Controller iSender, ControllerDirection iDirection)
		{
			if (!this.mKeyboardSelection)
			{
				this.SelectedIndex = 0;
				this.mKeyboardSelection = true;
				return;
			}
			switch (iDirection)
			{
			case ControllerDirection.Right:
			case ControllerDirection.UpRight:
			case ControllerDirection.Left:
				break;
			case ControllerDirection.Up:
				this.SelectedIndex--;
				return;
			default:
				if (iDirection != ControllerDirection.Down)
				{
					return;
				}
				this.SelectedIndex++;
				break;
			}
		}

		// Token: 0x060009CF RID: 2511 RVA: 0x0003BB18 File Offset: 0x00039D18
		public override void OnMouseMove(MouseState iNewState, MouseState iOldState)
		{
			bool flag = false;
			for (int i = 0; i < this.mOptions.Length; i++)
			{
				MenuTextItem menuTextItem = this.mOptions[i];
				if (menuTextItem.InsideBounds((float)iNewState.X, (float)iNewState.Y))
				{
					this.mKeyboardSelection = false;
					this.mSelectedIndex = i;
					for (int j = 0; j < this.mOptions.Length; j++)
					{
						this.mOptions[j].Selected = false;
					}
					menuTextItem.Selected = true;
					flag = true;
					break;
				}
			}
			if (!flag && !this.mKeyboardSelection)
			{
				for (int k = 0; k < this.mOptions.Length; k++)
				{
					this.mOptions[k].Selected = false;
				}
			}
		}

		// Token: 0x060009D0 RID: 2512 RVA: 0x0003BBC8 File Offset: 0x00039DC8
		public override void OnMouseClick(MouseState iNewState, MouseState iOldState)
		{
			if (iNewState.LeftButton == ButtonState.Pressed)
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i < this.mOptions.Length; i++)
			{
				MenuTextItem menuTextItem = this.mOptions[i];
				if (menuTextItem.InsideBounds((float)iNewState.X, (float)iNewState.Y))
				{
					this.mKeyboardSelection = false;
					this.mSelectedIndex = i;
					for (int j = 0; j < this.mOptions.Length; j++)
					{
						this.mOptions[j].Selected = false;
					}
					menuTextItem.Selected = true;
					flag = true;
					this.OnSelect(ControlManager.Instance.MenuController);
					break;
				}
			}
			if (!flag && !this.mKeyboardSelection)
			{
				for (int k = 0; k < this.mOptions.Length; k++)
				{
					this.mOptions[k].Selected = false;
				}
			}
		}

		// Token: 0x060009D1 RID: 2513 RVA: 0x0003BC91 File Offset: 0x00039E91
		public override void OnSelect(Controller iSender)
		{
			if (this.Select != null)
			{
				this.Select.Invoke(this, this.mSelectedIndex);
			}
			this.Kill();
		}

		// Token: 0x060009D2 RID: 2514 RVA: 0x0003BCB3 File Offset: 0x00039EB3
		public override void OnTextInput(char iChar)
		{
		}

		// Token: 0x060009D3 RID: 2515 RVA: 0x0003BCB8 File Offset: 0x00039EB8
		public override void Draw(float iDeltaTime)
		{
			base.Draw(iDeltaTime);
			int num = this.mSelectedIndex;
			Vector2 mCenter = this.mCenter;
			MessageBox.sGUIBasicEffect.Color = new Vector4(1f, 1f, 1f, this.mAlpha);
			MessageBox.sGUIBasicEffect.SetScreenSize(RenderManager.Instance.ScreenSize.X, RenderManager.Instance.ScreenSize.Y);
			float num2 = Math.Min(this.mMessageHeight, 460f);
			mCenter.Y -= 0.5f * (num2 + (float)(this.mFont.LineHeight * this.mOptions.Length));
			Matrix matrix = default(Matrix);
			matrix.M11 = (matrix.M33 = 1f);
			matrix.M22 = num2 / this.mMessageHeight;
			matrix.M44 = 1f;
			matrix.M41 = mCenter.X;
			matrix.M42 = mCenter.Y;
			MessageBox.sGUIBasicEffect.VertexColorEnabled = true;
			this.mMessage.Draw(MessageBox.sGUIBasicEffect, ref matrix);
			MessageBox.sGUIBasicEffect.VertexColorEnabled = false;
			mCenter.Y += num2 + 0.5f * (float)this.mFont.LineHeight;
			if (this.mKeyboardSelection)
			{
				for (int i = 0; i < this.mOptions.Length; i++)
				{
					this.mOptions[i].Selected = (i == num);
					this.mOptions[i].Position = mCenter;
					this.mOptions[i].Draw(MessageBox.sGUIBasicEffect);
					mCenter.Y += (float)this.mFont.LineHeight;
				}
			}
			else
			{
				for (int j = 0; j < this.mOptions.Length; j++)
				{
					this.mOptions[j].Position = mCenter;
					this.mOptions[j].Alpha = this.mAlpha;
					this.mOptions[j].Draw(MessageBox.sGUIBasicEffect);
					mCenter.Y += (float)this.mFont.LineHeight;
				}
			}
			MessageBox.sGUIBasicEffect.CurrentTechnique.Passes[0].End();
			MessageBox.sGUIBasicEffect.End();
		}

		// Token: 0x17000217 RID: 535
		// (get) Token: 0x060009D4 RID: 2516 RVA: 0x0003BEFE File Offset: 0x0003A0FE
		// (set) Token: 0x060009D5 RID: 2517 RVA: 0x0003BF06 File Offset: 0x0003A106
		public int SelectedIndex
		{
			get
			{
				return this.mSelectedIndex;
			}
			set
			{
				if (value >= this.mOptions.Length)
				{
					value -= this.mOptions.Length;
				}
				if (value < 0)
				{
					value += this.mOptions.Length;
				}
				this.mSelectedIndex = value;
			}
		}

		// Token: 0x060009D6 RID: 2518 RVA: 0x0003BF36 File Offset: 0x0003A136
		internal void SetMessage(string p)
		{
			p = this.mFont.Wrap(p, 320, true);
			this.mMessage.SetText(p);
			this.mMessageHeight = this.mFont.MeasureText(p, true).Y;
		}

		// Token: 0x17000218 RID: 536
		// (get) Token: 0x060009D7 RID: 2519 RVA: 0x0003BF70 File Offset: 0x0003A170
		public override int ZIndex
		{
			get
			{
				return 2002;
			}
		}

		// Token: 0x060009D8 RID: 2520 RVA: 0x0003BF77 File Offset: 0x0003A177
		public override void Show()
		{
			base.Show();
		}

		// Token: 0x060009D9 RID: 2521 RVA: 0x0003BF7F File Offset: 0x0003A17F
		public void Show(int iMessage)
		{
			this.mMessage.SetText(LanguageManager.Instance.GetString(iMessage));
		}

		// Token: 0x040008F4 RID: 2292
		private bool mKeyboardSelection;

		// Token: 0x040008F5 RID: 2293
		private int mSelectedIndex;

		// Token: 0x040008F6 RID: 2294
		private MenuTextItem[] mOptions;

		// Token: 0x040008F7 RID: 2295
		private int mMessageHash;
	}
}
