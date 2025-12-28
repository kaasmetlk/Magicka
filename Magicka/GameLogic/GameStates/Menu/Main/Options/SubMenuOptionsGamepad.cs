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
	// Token: 0x0200024B RID: 587
	internal class SubMenuOptionsGamepad : SubMenu
	{
		// Token: 0x0600122B RID: 4651 RVA: 0x0006E95C File Offset: 0x0006CB5C
		public override void LanguageChanged()
		{
			base.LanguageChanged();
			this.Controller = this.mController;
			this.mBackItem.LanguageChanged();
		}

		// Token: 0x170004B2 RID: 1202
		// (get) Token: 0x0600122C RID: 4652 RVA: 0x0006E97C File Offset: 0x0006CB7C
		public static SubMenuOptionsGamepad Instance
		{
			get
			{
				if (SubMenuOptionsGamepad.sSingelton == null)
				{
					lock (SubMenuOptionsGamepad.sSingeltonLock)
					{
						if (SubMenuOptionsGamepad.sSingelton == null)
						{
							SubMenuOptionsGamepad.sSingelton = new SubMenuOptionsGamepad();
						}
					}
				}
				return SubMenuOptionsGamepad.sSingelton;
			}
		}

		// Token: 0x0600122D RID: 4653 RVA: 0x0006E9D0 File Offset: 0x0006CBD0
		private SubMenuOptionsGamepad()
		{
			this.mMenuTitle = new Text(128, FontManager.Instance.GetFont(MagickaFont.MenuTitle), TextAlign.Center, false);
			this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenuOptionsGamepad.LOC_GAMEPAD));
			this.mMenuItems = new List<MenuItem>();
			this.mMenuOptions = new List<MenuTextItem>();
			this.mFont = FontManager.Instance.GetFont(MagickaFont.MenuOption);
			this.mMenuItems.Add(new MenuTextItem(LanguageManager.Instance.GetString(SubMenuOptionsGamepad.LOC_DEFAULTS), new Vector2(this.mPosition.X, 832f), this.mFont, TextAlign.Center));
			for (int i = 0; i < 24; i++)
			{
				string text = "#ctrl_" + ((ControllerFunction)i).ToString();
				this.AddMenuTextItem(text.ToLowerInvariant().GetHashCodeCustom());
				this.AddMenuOptions("-");
			}
			this.mBackItem = new MenuImageTextItem(SubMenu.BACK_POSITION, SubMenu.sPagesTexture, SubMenu.BACK_UVOFFSET, SubMenu.BACK_UVSCALE, 0, default(Vector2), SubMenu.BACK_TEXT_ALIGN, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenu.BACK_SIZE);
			this.mScrollBar = new MenuScrollBar(new Vector2(928f, 512f), (float)(this.mFont.LineHeight * 13), 12);
		}

		// Token: 0x0600122E RID: 4654 RVA: 0x0006EB30 File Offset: 0x0006CD30
		public new void AddMenuTextItem(string iText)
		{
			Vector2 mPosition = this.mPosition;
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
			mPosition.X -= 30f;
			mPosition.Y += ((float)font.LineHeight + 10f) * (float)this.mMenuItems.Count;
			this.mMenuItems.Add(new MenuTextItem(iText, mPosition, font, TextAlign.Right));
		}

		// Token: 0x0600122F RID: 4655 RVA: 0x0006EBA0 File Offset: 0x0006CDA0
		public override MenuTextItem AddMenuTextItem(int iText)
		{
			Vector2 mPosition = this.mPosition;
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
			mPosition.X -= 30f;
			mPosition.Y += ((float)font.LineHeight + 10f) * (float)this.mMenuItems.Count;
			MenuTextItem menuTextItem = new MenuTextItem(iText, mPosition, font, TextAlign.Right);
			this.mMenuItems.Add(menuTextItem);
			return menuTextItem;
		}

		// Token: 0x06001230 RID: 4656 RVA: 0x0006EC14 File Offset: 0x0006CE14
		public void AddMenuOptions(string iText)
		{
			Vector2 mPosition = this.mPosition;
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
			mPosition.X += 30f;
			mPosition.Y += ((float)font.LineHeight + 10f) * (float)this.mMenuItems.Count;
			this.mMenuOptions.Add(new MenuTextItem(iText, mPosition, font, TextAlign.Left, false));
		}

		// Token: 0x170004B3 RID: 1203
		// (get) Token: 0x06001231 RID: 4657 RVA: 0x0006EC84 File Offset: 0x0006CE84
		// (set) Token: 0x06001232 RID: 4658 RVA: 0x0006EC8C File Offset: 0x0006CE8C
		public Controller Controller
		{
			get
			{
				return this.mController;
			}
			set
			{
				this.mController = value;
				DirectInputController directInputController = this.mController as DirectInputController;
				XInputController xinputController = this.mController as XInputController;
				if (directInputController != null)
				{
					(this.mMenuItems[0] as MenuTextItem).SetText(LanguageManager.Instance.GetString(SubMenuOptionsGamepad.LOC_RECONFIG));
					this.mMenuTitle.SetText(directInputController.Device.DeviceInformation.InstanceName);
					for (int i = 0; i < 24; i++)
					{
						DirectInputController.Binding binding = directInputController.Bindings[i];
						if (binding.Type == DirectInputController.Binding.BindingType.Button)
						{
							this.mMenuOptions[i].SetText("B" + (binding.BindingIndex + 1));
						}
						else if (binding.Type == DirectInputController.Binding.BindingType.POV)
						{
							this.mMenuOptions[i].SetText(((ControllerDirection)(1 << binding.BindingIndex)).ToString());
						}
						else if (binding.Type == DirectInputController.Binding.BindingType.PositiveAxis)
						{
							string text = ((char)(88 + binding.BindingIndex % 3)).ToString();
							if (binding.BindingIndex >= 3)
							{
								text = 'R' + text;
							}
							text = '+' + text;
							this.mMenuOptions[i].SetText(text);
						}
						else if (binding.Type == DirectInputController.Binding.BindingType.NegativeAxis)
						{
							string text2 = ((char)(88 + binding.BindingIndex % 3)).ToString();
							if (binding.BindingIndex >= 3)
							{
								text2 = 'R' + text2;
							}
							text2 = '-' + text2;
							this.mMenuOptions[i].SetText(text2);
						}
						else
						{
							this.mMenuOptions[i].SetText("-");
						}
					}
					return;
				}
				if (xinputController != null)
				{
					(this.mMenuItems[0] as MenuTextItem).SetText(LanguageManager.Instance.GetString(SubMenuOptionsGamepad.LOC_DEFAULTS));
					string @string = LanguageManager.Instance.GetString(SubMenuOptionsGamepad.LOC_XINPUT);
					this.mMenuTitle.SetText(@string.Replace("#1;", ((int)(xinputController.PlayerIndex + 1)).ToString()));
					for (int j = 0; j < 24; j++)
					{
						XInputController.Binding binding2 = xinputController.Bindings[j];
						if (binding2.Type == XInputController.Binding.BindingType.Button)
						{
							this.mMenuOptions[j].SetText(XInputController.GetButtonName((Buttons)binding2.BindingIndex));
						}
						else if (binding2.Type == XInputController.Binding.BindingType.Trigger)
						{
							if (binding2.BindingIndex == 0)
							{
								this.mMenuOptions[j].SetText(XInputController.GetButtonName(Buttons.LeftTrigger));
							}
							else if (binding2.BindingIndex == 1)
							{
								this.mMenuOptions[j].SetText(XInputController.GetButtonName(Buttons.RightTrigger));
							}
							else
							{
								this.mMenuOptions[j].SetText("-");
							}
						}
						else if (binding2.Type == XInputController.Binding.BindingType.PositiveStick)
						{
							if (binding2.BindingIndex == 0)
							{
								this.mMenuOptions[j].SetText(XInputController.GetButtonName(Buttons.LeftThumbstickRight));
							}
							else if (binding2.BindingIndex == 1)
							{
								this.mMenuOptions[j].SetText(XInputController.GetButtonName(Buttons.LeftThumbstickUp));
							}
							else if (binding2.BindingIndex == 2)
							{
								this.mMenuOptions[j].SetText(XInputController.GetButtonName(Buttons.RightThumbstickRight));
							}
							else if (binding2.BindingIndex == 3)
							{
								this.mMenuOptions[j].SetText(XInputController.GetButtonName(Buttons.RightThumbstickUp));
							}
							else
							{
								this.mMenuOptions[j].SetText("-");
							}
						}
						else if (binding2.Type == XInputController.Binding.BindingType.NegativeStick)
						{
							if (binding2.BindingIndex == 0)
							{
								this.mMenuOptions[j].SetText(XInputController.GetButtonName(Buttons.LeftThumbstickLeft));
							}
							else if (binding2.BindingIndex == 1)
							{
								this.mMenuOptions[j].SetText(XInputController.GetButtonName(Buttons.LeftThumbstickDown));
							}
							else if (binding2.BindingIndex == 2)
							{
								this.mMenuOptions[j].SetText(XInputController.GetButtonName(Buttons.RightThumbstickLeft));
							}
							else if (binding2.BindingIndex == 3)
							{
								this.mMenuOptions[j].SetText(XInputController.GetButtonName(Buttons.RightThumbstickDown));
							}
							else
							{
								this.mMenuOptions[j].SetText("-");
							}
						}
						else
						{
							this.mMenuOptions[j].SetText("-");
						}
					}
				}
			}
		}

		// Token: 0x06001233 RID: 4659 RVA: 0x0006F15C File Offset: 0x0006D35C
		public override void ControllerUp(Controller iSender)
		{
			if (this.mCurrentSelection == SubMenuOptionsGamepad.Selection.Back)
			{
				this.mSelectedPosition = 0;
				this.mCurrentSelection = SubMenuOptionsGamepad.Selection.Bindings;
				this.mBackItem.Selected = false;
				this.mMenuItems[0].Selected = true;
				return;
			}
			if (this.mSelectedPosition == 1)
			{
				this.mCurrentSelection = SubMenuOptionsGamepad.Selection.Back;
				this.mBackItem.Selected = true;
				this.mMenuItems[1].Selected = false;
				this.mMenuOptions[0].Selected = false;
				return;
			}
			this.mBackItem.Selected = false;
			this.mSelectedPosition--;
			if (this.mSelectedPosition < 0)
			{
				this.mSelectedPosition = this.mMenuItems.Count - 1;
			}
			for (int i = 0; i < this.mMenuItems.Count; i++)
			{
				this.mMenuItems[i].Selected = (i == this.mSelectedPosition);
				if (i > 0)
				{
					this.mMenuOptions[i - 1].Selected = (i == this.mSelectedPosition);
				}
			}
		}

		// Token: 0x06001234 RID: 4660 RVA: 0x0006F268 File Offset: 0x0006D468
		public override void ControllerDown(Controller iSender)
		{
			if (this.mCurrentSelection == SubMenuOptionsGamepad.Selection.Back)
			{
				this.mSelectedPosition = 1;
				this.mCurrentSelection = SubMenuOptionsGamepad.Selection.Bindings;
				this.mBackItem.Selected = false;
				this.mMenuItems[this.mSelectedPosition].Selected = true;
				this.mMenuOptions[this.mSelectedPosition - 1].Selected = true;
				return;
			}
			if (this.mSelectedPosition == 0)
			{
				this.mCurrentSelection = SubMenuOptionsGamepad.Selection.Back;
				this.mBackItem.Selected = true;
				this.mMenuItems[0].Selected = false;
				return;
			}
			this.mCurrentSelection = SubMenuOptionsGamepad.Selection.Bindings;
			this.mBackItem.Selected = false;
			this.mSelectedPosition++;
			if (this.mSelectedPosition >= this.mMenuItems.Count)
			{
				this.mSelectedPosition = 0;
			}
			for (int i = 0; i < this.mMenuItems.Count; i++)
			{
				this.mMenuItems[i].Selected = (i == this.mSelectedPosition);
				if (i > 0)
				{
					this.mMenuOptions[i - 1].Selected = (i == this.mSelectedPosition);
				}
			}
		}

		// Token: 0x06001235 RID: 4661 RVA: 0x0006F384 File Offset: 0x0006D584
		public override void ControllerA(Controller iSender)
		{
			if (this.mCurrentSelection == SubMenuOptionsGamepad.Selection.Back)
			{
				Tome.Instance.PopMenu();
				return;
			}
			if (this.mSelectedPosition == 0)
			{
				if (this.mController is XInputController)
				{
					(this.mController as XInputController).LoadDefaults();
					this.Controller = this.mController;
					return;
				}
				if (this.mController is DirectInputController)
				{
					GamePadConfigMessageBox.Instance.GamePad = (this.mController as DirectInputController);
					GamePadConfigMessageBox.Instance.Show();
					return;
				}
			}
			else if (this.mSelectedPosition > 0)
			{
				this.mMenuOptions[this.mSelectedPosition - 1].SetText("???");
				this.mFunction = (ControllerFunction)(this.mSelectedPosition - 1);
				if (this.mController is DirectInputController)
				{
					(this.mController as DirectInputController).OnChange += this.OnBindingChanged;
					return;
				}
				if (this.mController is XInputController)
				{
					(this.mController as XInputController).OnChange += this.OnBindingChanged;
				}
			}
		}

		// Token: 0x06001236 RID: 4662 RVA: 0x0006F48D File Offset: 0x0006D68D
		public override void ControllerB(Controller iSender)
		{
			if (this.mFunction >= ControllerFunction.Move_Right)
			{
				this.mFunction = (ControllerFunction)(-1);
				return;
			}
			Tome.Instance.PopMenu();
		}

		// Token: 0x06001237 RID: 4663 RVA: 0x0006F4AC File Offset: 0x0006D6AC
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
			else if (Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out iPoint, out flag) && iState.LeftButton == ButtonState.Pressed && iOldState.LeftButton == ButtonState.Released && flag)
			{
				if (this.mScrollBar.InsideBounds(ref iPoint))
				{
					if (this.mScrollBar.InsideDragBounds(iPoint))
					{
						this.mScrollBar.Grabbed = true;
						return;
					}
					if (this.mScrollBar.InsideUpBounds(iPoint))
					{
						this.mScrollBar.Value--;
						return;
					}
					if (this.mScrollBar.InsideDownBounds(iPoint))
					{
						this.mScrollBar.Value++;
						return;
					}
					this.mScrollBar.ScrollTo(iPoint.Y);
					return;
				}
				else
				{
					if (this.mBackItem.Enabled && this.mBackItem.InsideBounds(ref iPoint))
					{
						Tome.Instance.PopMenu();
						return;
					}
					if (this.mFunction < ControllerFunction.Move_Right)
					{
						bool flag2 = false;
						MenuItem menuItem = this.mMenuItems[0];
						if (menuItem.Enabled & menuItem.InsideBounds(ref iPoint))
						{
							if (this.mSelectedPosition != 0)
							{
								AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_mouseover".GetHashCodeCustom());
							}
							this.mKeyboardSelection = false;
							this.mSelectedPosition = 0;
							menuItem.Selected = true;
							for (int i = 1; i < this.mMenuItems.Count; i++)
							{
								this.mMenuItems[i].Selected = false;
								this.mMenuOptions[i - 1].Selected = false;
							}
							flag2 = true;
							if (this.mController is XInputController)
							{
								(this.mController as XInputController).LoadDefaults();
								this.Controller = this.mController;
							}
							else if (this.mController is DirectInputController)
							{
								GamePadConfigMessageBox.Instance.GamePad = (this.mController as DirectInputController);
								GamePadConfigMessageBox.Instance.Show();
							}
						}
						if (!flag2)
						{
							int j = this.mScrollBar.Value + 1;
							while (j < this.mScrollBar.Value + 12 + 11)
							{
								menuItem = this.mMenuItems[j];
								MenuItem menuItem2 = this.mMenuOptions[j - 1];
								if (menuItem.Enabled && (menuItem.InsideBounds(ref iPoint) || menuItem2.InsideBounds(ref iPoint)))
								{
									if (this.mSelectedPosition != j)
									{
										AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_mouseover".GetHashCodeCustom());
									}
									this.mKeyboardSelection = false;
									this.mSelectedPosition = j;
									for (int k = 0; k < this.mMenuItems.Count; k++)
									{
										this.mMenuItems[k].Selected = (k == j);
										if (k > 0)
										{
											this.mMenuOptions[k - 1].Selected = (k == j);
										}
									}
									flag2 = true;
									if (j <= 0)
									{
										break;
									}
									(menuItem2 as MenuTextItem).SetText("???");
									this.mFunction = (ControllerFunction)(j - 1);
									if (this.mController is DirectInputController)
									{
										(this.mController as DirectInputController).OnChange += this.OnBindingChanged;
										break;
									}
									if (this.mController is XInputController)
									{
										(this.mController as XInputController).OnChange += this.OnBindingChanged;
										break;
									}
									break;
								}
								else
								{
									j++;
								}
							}
						}
						if (!flag2)
						{
							for (int l = 0; l < this.mMenuItems.Count; l++)
							{
								this.mMenuItems[l].Selected = false;
							}
						}
					}
				}
			}
			if (iState.LeftButton == ButtonState.Released)
			{
				this.mScrollBar.Grabbed = false;
			}
		}

		// Token: 0x06001238 RID: 4664 RVA: 0x0006F8A4 File Offset: 0x0006DAA4
		private void OnBindingChanged(DirectInputController.Binding iBinding)
		{
			if (this.mFunction < ControllerFunction.Move_Right)
			{
				return;
			}
			(this.mController as DirectInputController).Bindings[(int)this.mFunction] = iBinding;
			this.mFunction = (ControllerFunction)(-1);
			this.Controller = this.mController;
		}

		// Token: 0x06001239 RID: 4665 RVA: 0x0006F8E4 File Offset: 0x0006DAE4
		private void OnBindingChanged(XInputController.Binding iBinding)
		{
			if (this.mFunction < ControllerFunction.Move_Right)
			{
				return;
			}
			(this.mController as XInputController).Bindings[(int)this.mFunction] = iBinding;
			this.mFunction = (ControllerFunction)(-1);
			this.Controller = this.mController;
		}

		// Token: 0x0600123A RID: 4666 RVA: 0x0006F924 File Offset: 0x0006DB24
		public override void ControllerMouseMove(Controller iSender, Point iScreenSize, MouseState iState, MouseState iOldState)
		{
			Vector2 iPoint;
			bool flag;
			if (Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out iPoint, out flag) && flag)
			{
				this.mBackItem.Selected = false;
				if (this.mScrollBar.InsideBounds(ref iPoint))
				{
					if (this.mScrollBar.Grabbed)
					{
						if (this.mScrollBar.InsideDragUpBounds(iPoint))
						{
							this.mScrollBar.Value--;
							return;
						}
						if (this.mScrollBar.InsideDragDownBounds(iPoint))
						{
							this.mScrollBar.Value++;
							return;
						}
					}
				}
				else
				{
					if (this.mBackItem.Enabled && this.mBackItem.InsideBounds(ref iPoint))
					{
						this.mBackItem.Selected = true;
						this.mCurrentSelection = SubMenuOptionsGamepad.Selection.Back;
						return;
					}
					bool flag2 = false;
					MenuItem menuItem = this.mMenuItems[0];
					if (menuItem.Enabled & menuItem.InsideBounds(ref iPoint))
					{
						if (this.mSelectedPosition != 0)
						{
							AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_mouseover".GetHashCodeCustom());
						}
						this.mKeyboardSelection = false;
						this.mSelectedPosition = 0;
						menuItem.Selected = true;
						for (int i = 1; i < this.mMenuItems.Count; i++)
						{
							this.mMenuItems[i].Selected = false;
							this.mMenuOptions[i - 1].Selected = false;
						}
						flag2 = true;
						this.mCurrentSelection = SubMenuOptionsGamepad.Selection.Bindings;
					}
					if (!flag2)
					{
						for (int j = this.mScrollBar.Value + 1; j < this.mScrollBar.Value + 12 + 1; j++)
						{
							menuItem = this.mMenuItems[j];
							MenuItem menuItem2 = this.mMenuOptions[j - 1];
							if (menuItem.Enabled && (menuItem.InsideBounds(ref iPoint) || menuItem2.InsideBounds(ref iPoint)))
							{
								if (this.mSelectedPosition != j)
								{
									AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_mouseover".GetHashCodeCustom());
								}
								this.mKeyboardSelection = false;
								this.mSelectedPosition = j;
								for (int k = 0; k < this.mMenuItems.Count; k++)
								{
									this.mMenuItems[k].Selected = (k == j);
									if (k > 0)
									{
										this.mMenuOptions[k - 1].Selected = (k == j);
									}
								}
								flag2 = true;
								this.mCurrentSelection = SubMenuOptionsGamepad.Selection.Bindings;
								break;
							}
						}
					}
					if (!flag2)
					{
						this.mSelectedPosition = -1;
						for (int l = 0; l < this.mMenuItems.Count; l++)
						{
							this.mMenuItems[l].Selected = false;
							if (l > 0)
							{
								this.mMenuOptions[l - 1].Selected = false;
							}
						}
					}
				}
			}
		}

		// Token: 0x0600123B RID: 4667 RVA: 0x0006FBE9 File Offset: 0x0006DDE9
		protected override void ControllerMouseClicked(Controller iSender)
		{
			base.ControllerMouseClicked(iSender);
		}

		// Token: 0x0600123C RID: 4668 RVA: 0x0006FBF4 File Offset: 0x0006DDF4
		public override void OnEnter()
		{
			base.OnEnter();
			GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Bottom, SubMenuOptionsGamepad.LOC_CHANGEBINDING);
			GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Right, SubMenu.LOC_BACK);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Left);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Top);
			this.mFunction = (ControllerFunction)(-1);
			this.mCurrentSelection = SubMenuOptionsGamepad.Selection.Bindings;
			this.mSelectedPosition = 0;
			this.mBackItem.Selected = false;
		}

		// Token: 0x0600123D RID: 4669 RVA: 0x0006FC5E File Offset: 0x0006DE5E
		public override void OnExit()
		{
			base.OnExit();
			SaveManager.Instance.SaveSettings();
		}

		// Token: 0x0600123E RID: 4670 RVA: 0x0006FC70 File Offset: 0x0006DE70
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (this.mKeyboardSelection && this.mCurrentSelection == SubMenuOptionsGamepad.Selection.Bindings)
			{
				for (int i = 0; i < this.mMenuItems.Count; i++)
				{
					this.mMenuItems[i].Selected = (i == this.mSelectedPosition);
				}
			}
		}

		// Token: 0x0600123F RID: 4671 RVA: 0x0006FCC0 File Offset: 0x0006DEC0
		public override void Draw(Viewport iLeftSide, Viewport iRightSide)
		{
			this.mEffect.GraphicsDevice.Viewport = iRightSide;
			this.mEffect.Begin();
			this.mEffect.CurrentTechnique.Passes[0].Begin();
			this.mEffect.VertexColorEnabled = false;
			this.mEffect.Color = new Vector4(0f, 0f, 0f, 0.8f);
			this.mMenuTitle.Draw(this.mEffect, this.mPosition.X, 96f);
			this.mScrollBar.Draw(this.mEffect);
			this.mBackItem.Draw(this.mEffect);
			if (this.mSelectedPosition > 0)
			{
				while (this.mSelectedPosition <= this.mScrollBar.Value)
				{
					this.mScrollBar.Value--;
				}
				while (this.mSelectedPosition > this.mScrollBar.Value + 12)
				{
					this.mScrollBar.Value++;
				}
			}
			float num = (float)this.mFont.LineHeight;
			float num2 = this.mPosition.Y - num * 0.5f;
			MenuItem menuItem;
			Vector2 position;
			for (int i = this.mScrollBar.Value + 1; i < this.mScrollBar.Value + 12 + 1; i++)
			{
				menuItem = this.mMenuItems[i];
				if (this.mFunction >= ControllerFunction.Move_Right)
				{
					menuItem.Selected = (i == (int)(this.mFunction + 1));
				}
				position = menuItem.Position;
				position.Y = num2;
				menuItem.Position = position;
				menuItem.Draw(this.mEffect);
				menuItem = this.mMenuOptions[i - 1];
				if (this.mFunction >= ControllerFunction.Move_Right)
				{
					menuItem.Selected = (i == (int)(this.mFunction + 1));
				}
				position = menuItem.Position;
				position.Y = num2;
				menuItem.Position = position;
				menuItem.Draw(this.mEffect);
				num2 += num;
			}
			menuItem = this.mMenuItems[0];
			position = menuItem.Position;
			position.Y = num2 + 20f;
			menuItem.Position = position;
			menuItem.Draw(this.mEffect);
			this.mEffect.CurrentTechnique.Passes[0].End();
			this.mEffect.End();
		}

		// Token: 0x040010E1 RID: 4321
		private const int VISIBLE_ITEMS = 12;

		// Token: 0x040010E2 RID: 4322
		private static readonly int LOC_GAMEPAD = "#menu_opt_alt_08".GetHashCodeCustom();

		// Token: 0x040010E3 RID: 4323
		private static readonly int LOC_RECONFIG = "#menu_reconfig".GetHashCodeCustom();

		// Token: 0x040010E4 RID: 4324
		private static readonly int LOC_DEFAULTS = "#menu_restore_d".GetHashCodeCustom();

		// Token: 0x040010E5 RID: 4325
		private static readonly int LOC_XINPUT = "#menu_xinput_gamep".GetHashCodeCustom();

		// Token: 0x040010E6 RID: 4326
		private static readonly int LOC_CHANGEBINDING = "#opt_changebinding".GetHashCodeCustom();

		// Token: 0x040010E7 RID: 4327
		private static SubMenuOptionsGamepad sSingelton;

		// Token: 0x040010E8 RID: 4328
		private static volatile object sSingeltonLock = new object();

		// Token: 0x040010E9 RID: 4329
		private List<MenuTextItem> mMenuOptions;

		// Token: 0x040010EA RID: 4330
		private Controller mController;

		// Token: 0x040010EB RID: 4331
		private ControllerFunction mFunction;

		// Token: 0x040010EC RID: 4332
		private MenuScrollBar mScrollBar;

		// Token: 0x040010ED RID: 4333
		private MenuImageTextItem mBackItem;

		// Token: 0x040010EE RID: 4334
		private SubMenuOptionsGamepad.Selection mCurrentSelection = SubMenuOptionsGamepad.Selection.Bindings;

		// Token: 0x040010EF RID: 4335
		private BitmapFont mFont;

		// Token: 0x0200024C RID: 588
		private enum Selection
		{
			// Token: 0x040010F1 RID: 4337
			Back,
			// Token: 0x040010F2 RID: 4338
			Bindings
		}
	}
}
