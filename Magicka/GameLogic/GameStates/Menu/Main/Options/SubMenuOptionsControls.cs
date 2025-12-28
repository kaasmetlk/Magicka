using System;
using System.Collections.Generic;
using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Localization;
using Magicka.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;

namespace Magicka.GameLogic.GameStates.Menu.Main.Options
{
	// Token: 0x020001B9 RID: 441
	internal class SubMenuOptionsControls : SubMenu
	{
		// Token: 0x1700033E RID: 830
		// (get) Token: 0x06000D8B RID: 3467 RVA: 0x00050914 File Offset: 0x0004EB14
		public static SubMenuOptionsControls Instance
		{
			get
			{
				if (SubMenuOptionsControls.mSingelton == null)
				{
					lock (SubMenuOptionsControls.mSingeltonLock)
					{
						if (SubMenuOptionsControls.mSingelton == null)
						{
							SubMenuOptionsControls.mSingelton = new SubMenuOptionsControls();
						}
					}
				}
				return SubMenuOptionsControls.mSingelton;
			}
		}

		// Token: 0x06000D8C RID: 3468 RVA: 0x00050968 File Offset: 0x0004EB68
		public override void LanguageChanged()
		{
			base.LanguageChanged();
			this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenuOptionsControls.LOC_CTRL_OPTS));
			string @string = LanguageManager.Instance.GetString(SubMenuOptionsControls.LOC_XINPUT);
			(this.mMenuItems[2] as MenuTextItem).SetText(@string.Replace("#1;", "1"));
			(this.mMenuItems[3] as MenuTextItem).SetText(@string.Replace("#1;", "2"));
			(this.mMenuItems[4] as MenuTextItem).SetText(@string.Replace("#1;", "3"));
			(this.mMenuItems[5] as MenuTextItem).SetText(@string.Replace("#1;", "4"));
		}

		// Token: 0x06000D8D RID: 3469 RVA: 0x00050A40 File Offset: 0x0004EC40
		private SubMenuOptionsControls()
		{
			this.mSelectedPosition = 0;
			this.mMenuItems = new List<MenuItem>();
			this.mMenuTitle = new Text(32, FontManager.Instance.GetFont(MagickaFont.MenuTitle), TextAlign.Center, false);
			this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenuOptionsControls.LOC_CTRL_OPTS));
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
			this.AddMenuTextItem(SubMenuOptionsControls.LOC_KEYBOARD_AND_MOUSE);
			this.mMenuItems.Add(new MenuItemSeparator(new Vector2(this.mPosition.X, this.mPosition.Y + ((float)font.LineHeight + 10f) * (float)this.mMenuItems.Count)));
			string @string = LanguageManager.Instance.GetString(SubMenuOptionsControls.LOC_XINPUT);
			this.AddMenuTextItem(@string.Replace("#1;", "1"));
			this.AddMenuTextItem(@string.Replace("#1;", "2"));
			this.AddMenuTextItem(@string.Replace("#1;", "3"));
			this.AddMenuTextItem(@string.Replace("#1;", "4"));
			this.mMenuItems.Add(new MenuItemSeparator(new Vector2(this.mPosition.X, this.mPosition.Y + ((float)font.LineHeight + 10f) * (float)this.mMenuItems.Count)));
			this.mScrollBar = new MenuScrollBar(default(Vector2), (float)(font.LineHeight * 10), 0);
			this.mMenuItems.Add(new MenuImageTextItem(SubMenu.BACK_POSITION, SubMenu.sPagesTexture, SubMenu.BACK_UVOFFSET, SubMenu.BACK_UVSCALE, 0, default(Vector2), SubMenu.BACK_TEXT_ALIGN, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenu.BACK_SIZE));
			this.UpdateControllers();
		}

		// Token: 0x06000D8E RID: 3470 RVA: 0x00050C1C File Offset: 0x0004EE1C
		public new void AddMenuTextItem(string iText)
		{
			Vector2 mPosition = this.mPosition;
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
			mPosition.Y += ((float)font.LineHeight + 10f) * (float)this.mMenuItems.Count;
			this.mMenuItems.Add(new MenuTextItem(iText, mPosition, font, TextAlign.Center));
		}

		// Token: 0x06000D8F RID: 3471 RVA: 0x00050C78 File Offset: 0x0004EE78
		public void UpdateControllers()
		{
			List<DirectInputController> dinputPads = ControlManager.Instance.DInputPads;
			int i;
			for (i = 0; i < dinputPads.Count; i++)
			{
				if (i >= this.mControllers.Count)
				{
					string text = dinputPads[i].Device.DeviceInformation.InstanceName;
					if (text.Length > 30)
					{
						text = text.Substring(0, 27) + "...";
					}
					this.mMenuItems.Insert(this.mMenuItems.Count - 1, new MenuTextItem(text, default(Vector2), FontManager.Instance.GetFont(MagickaFont.MenuOption), TextAlign.Center));
					this.mControllers.Add(dinputPads[i].Device.DeviceInformation.InstanceGuid);
				}
				else if (this.mControllers[i] != dinputPads[i].Device.DeviceInformation.InstanceGuid)
				{
					string text2 = dinputPads[i].Device.DeviceInformation.InstanceName;
					if (text2.Length > 30)
					{
						text2 = text2.Substring(0, 27) + "...";
					}
					(this.mMenuItems[i + 7] as MenuTextItem).SetText(text2);
					this.mControllers[i] = dinputPads[i].Device.DeviceInformation.InstanceGuid;
				}
			}
			while (i < this.mControllers.Count)
			{
				this.mMenuItems.RemoveAt(this.mMenuItems.Count - 2);
				this.mControllers.RemoveAt(this.mControllers.Count - 1);
			}
			int num = 0;
			for (int j = 0; j < ControlManager.Instance.XInputPads.Count; j++)
			{
				if (ControlManager.Instance.XInputPads[j].IsConnected)
				{
					num++;
				}
			}
			this.mScrollBar.SetMaxValue(this.mMenuItems.Count - (4 - num) - 10);
		}

		// Token: 0x06000D90 RID: 3472 RVA: 0x00050E94 File Offset: 0x0004F094
		public override void ControllerMouseAction(Controller iSender, Point iScreenSize, MouseState iState, MouseState iOldState)
		{
			Vector2 iPoint;
			bool flag;
			if (iState.ScrollWheelValue > iOldState.ScrollWheelValue)
			{
				this.mScrollBar.Value--;
			}
			else if (iState.ScrollWheelValue < iOldState.ScrollWheelValue)
			{
				this.mScrollBar.Value++;
			}
			else if (Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out iPoint, out flag))
			{
				if (iState.LeftButton == ButtonState.Pressed && iOldState.LeftButton == ButtonState.Released)
				{
					if (flag)
					{
						if (this.mScrollBar.InsideBounds(ref iPoint))
						{
							if (this.mScrollBar.InsideDragBounds(iPoint))
							{
								this.mScrollBar.Grabbed = true;
							}
							else if (this.mScrollBar.InsideUpBounds(iPoint))
							{
								this.mScrollBar.Value--;
							}
							else if (this.mScrollBar.InsideDownBounds(iPoint))
							{
								this.mScrollBar.Value++;
							}
							else
							{
								this.mScrollBar.ScrollTo(iPoint.Y);
							}
						}
						else
						{
							base.ControllerMouseAction(iSender, iScreenSize, iState, iOldState);
						}
					}
					else
					{
						base.ControllerMouseAction(iSender, iScreenSize, iState, iOldState);
					}
				}
				else
				{
					base.ControllerMouseAction(iSender, iScreenSize, iState, iOldState);
				}
			}
			if (iState.LeftButton == ButtonState.Released)
			{
				this.mScrollBar.Grabbed = false;
			}
		}

		// Token: 0x06000D91 RID: 3473 RVA: 0x00050FEC File Offset: 0x0004F1EC
		public override void ControllerMouseMove(Controller iSender, Point iScreenSize, MouseState iState, MouseState iOldState)
		{
			Vector2 vector;
			bool flag;
			if (Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out vector, out flag))
			{
				if (this.mScrollBar.Grabbed)
				{
					this.mScrollBar.ScrollTo(vector.Y);
					return;
				}
				if (flag)
				{
					bool flag2 = false;
					for (int i = this.mScrollBar.Value; i < Math.Min(this.mScrollBar.Value + 10, this.mMenuItems.Count); i++)
					{
						MenuItem menuItem = this.mMenuItems[i];
						if (menuItem.Enabled && menuItem.InsideBounds(ref vector))
						{
							if (this.mSelectedPosition != i)
							{
								AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_mouseover".GetHashCodeCustom());
							}
							this.mKeyboardSelection = false;
							this.mSelectedPosition = i;
							for (int j = 0; j < this.mMenuItems.Count; j++)
							{
								this.mMenuItems[j].Selected = (j == i);
							}
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						MenuItem menuItem = this.mMenuItems[this.mMenuItems.Count - 1];
						if (menuItem.InsideBounds(ref vector))
						{
							if (this.mSelectedPosition != this.mMenuItems.Count - 1)
							{
								AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_mouseover".GetHashCodeCustom());
							}
							this.mKeyboardSelection = false;
							this.mSelectedPosition = this.mMenuItems.Count - 1;
							for (int k = 0; k < this.mMenuItems.Count; k++)
							{
								this.mMenuItems[k].Selected = (k == this.mMenuItems.Count - 1);
							}
							return;
						}
						for (int l = 0; l < this.mMenuItems.Count; l++)
						{
							this.mMenuItems[l].Selected = false;
						}
						this.mSelectedPosition = -1;
					}
				}
			}
		}

		// Token: 0x06000D92 RID: 3474 RVA: 0x000511E8 File Offset: 0x0004F3E8
		public override void Draw(Viewport iLeftSide, Viewport iRightSide)
		{
			this.mEffect.GraphicsDevice.Viewport = iRightSide;
			this.mEffect.Begin();
			this.mEffect.CurrentTechnique.Passes[0].Begin();
			this.mEffect.Color = new Vector4(0f, 0f, 0f, 1f);
			int num = 0;
			for (int i = 0; i < ControlManager.Instance.XInputPads.Count; i++)
			{
				if (ControlManager.Instance.XInputPads[i].IsConnected)
				{
					num++;
				}
			}
			while (this.mSelectedPosition < this.mMenuItems.Count - 1)
			{
				if (this.mSelectedPosition - (4 - num) < this.mScrollBar.Value + 10)
				{
					break;
				}
				this.mScrollBar.Value++;
			}
			while (this.mSelectedPosition >= 0 && this.mSelectedPosition < this.mScrollBar.Value)
			{
				this.mScrollBar.Value--;
			}
			if (this.mMenuTitle != null)
			{
				this.mMenuTitle.Draw(this.mEffect, this.mPosition.X, 96f);
			}
			this.mScrollBar.Height = 512f;
			this.mScrollBar.Position = new Vector2(860f, 558f);
			if (this.mScrollBar.MaxValue > 0)
			{
				this.mScrollBar.Draw(this.mEffect);
			}
			Vector2 mPosition = this.mPosition;
			float num2 = 48f;
			for (int j = this.mScrollBar.Value; j < Math.Min(this.mScrollBar.Value + 10, this.mMenuItems.Count - 1); j++)
			{
				MenuItem menuItem = this.mMenuItems[j];
				if (j == this.mMenuItems.Count - 1)
				{
					menuItem.Draw(this.mEffect);
				}
				else if (!((j == 1 & num == 0) | (j == 6 & ControlManager.Instance.DInputPads.Count == 0)))
				{
					if ((j >= 2 & j < 6) && !ControlManager.Instance.XInputPads[j - 2].IsConnected)
					{
						menuItem.Enabled = false;
					}
					else
					{
						menuItem.Enabled = true;
						menuItem.Position = mPosition;
						menuItem.Draw(this.mEffect);
						mPosition.Y += num2;
					}
				}
			}
			this.mMenuItems[this.mMenuItems.Count - 1].Draw(this.mEffect);
			this.mEffect.GraphicsDevice.Viewport = iLeftSide;
			this.mEffect.Color = new Vector4(2f, 2f, 2f, 1f);
			this.mEffect.CurrentTechnique.Passes[0].End();
			this.mEffect.End();
		}

		// Token: 0x06000D93 RID: 3475 RVA: 0x000514EF File Offset: 0x0004F6EF
		protected override void ControllerMouseClicked(Controller iSender)
		{
			if (this.mSelectedPosition == this.mMenuItems.Count - 1)
			{
				Tome.Instance.PopMenu();
				return;
			}
			base.ControllerMouseClicked(iSender);
		}

		// Token: 0x06000D94 RID: 3476 RVA: 0x00051518 File Offset: 0x0004F718
		public override void ControllerA(Controller iSender)
		{
			if (this.mSelectedPosition == 1 || this.mSelectedPosition == 6)
			{
				return;
			}
			if (this.mSelectedPosition == 0)
			{
				Tome.Instance.PushMenu(SubMenuOptionsKeyboard.Instance, 1);
				return;
			}
			if (this.mSelectedPosition == this.mMenuItems.Count - 1)
			{
				Tome.Instance.PopMenu();
				return;
			}
			if (this.mSelectedPosition > 1 && this.mSelectedPosition < 6)
			{
				SubMenuOptionsGamepad.Instance.Controller = ControlManager.Instance.XInputPads[this.mSelectedPosition - 2];
				Tome.Instance.PushMenu(SubMenuOptionsGamepad.Instance, 1);
				return;
			}
			SubMenuOptionsGamepad.Instance.Controller = ControlManager.Instance.DInputPads[this.mSelectedPosition - 7];
			Tome.Instance.PushMenu(SubMenuOptionsGamepad.Instance, 1);
		}

		// Token: 0x06000D95 RID: 3477 RVA: 0x000515E6 File Offset: 0x0004F7E6
		public override void ControllerB(Controller iSender)
		{
			base.ControllerB(iSender);
		}

		// Token: 0x06000D96 RID: 3478 RVA: 0x000515EF File Offset: 0x0004F7EF
		public override void ControllerUp(Controller iSender)
		{
			base.ControllerUp(iSender);
		}

		// Token: 0x06000D97 RID: 3479 RVA: 0x000515F8 File Offset: 0x0004F7F8
		public override void ControllerDown(Controller iSender)
		{
			base.ControllerDown(iSender);
		}

		// Token: 0x06000D98 RID: 3480 RVA: 0x00051601 File Offset: 0x0004F801
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			base.Update(iDataChannel, iDeltaTime);
		}

		// Token: 0x06000D99 RID: 3481 RVA: 0x0005160B File Offset: 0x0004F80B
		public override void OnEnter()
		{
			this.UpdateControllers();
			GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Bottom, SubMenu.LOC_SELECT);
			GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Right, SubMenu.LOC_BACK);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Left);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Top);
		}

		// Token: 0x06000D9A RID: 3482 RVA: 0x00051649 File Offset: 0x0004F849
		public override void OnExit()
		{
			SaveManager.Instance.SaveSettings();
		}

		// Token: 0x04000C50 RID: 3152
		private const int VISIBLE_ITEMS = 10;

		// Token: 0x04000C51 RID: 3153
		private static readonly int LOC_CTRL_OPTS = "#menu_opt_07".GetHashCodeCustom();

		// Token: 0x04000C52 RID: 3154
		private static readonly int LOC_KEYBOARD_AND_MOUSE = "#menu_opt_alt_07".GetHashCodeCustom();

		// Token: 0x04000C53 RID: 3155
		private static readonly int LOC_XINPUT = "#menu_xinput_gamep".GetHashCodeCustom();

		// Token: 0x04000C54 RID: 3156
		private static SubMenuOptionsControls mSingelton;

		// Token: 0x04000C55 RID: 3157
		private static volatile object mSingeltonLock = new object();

		// Token: 0x04000C56 RID: 3158
		private List<Guid> mControllers = new List<Guid>();

		// Token: 0x04000C57 RID: 3159
		private MenuScrollBar mScrollBar;
	}
}
