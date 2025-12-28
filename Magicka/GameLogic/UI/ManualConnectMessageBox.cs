using System;
using System.Net;
using System.Text;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using SteamWrapper;

namespace Magicka.GameLogic.UI
{
	// Token: 0x02000634 RID: 1588
	internal class ManualConnectMessageBox : MessageBox
	{
		// Token: 0x17000B5E RID: 2910
		// (get) Token: 0x06002FFB RID: 12283 RVA: 0x0018794C File Offset: 0x00185B4C
		public static ManualConnectMessageBox Instance
		{
			get
			{
				if (ManualConnectMessageBox.sSingelton == null)
				{
					lock (ManualConnectMessageBox.sSingeltonLock)
					{
						if (ManualConnectMessageBox.sSingelton == null)
						{
							ManualConnectMessageBox.sSingelton = new ManualConnectMessageBox();
						}
					}
				}
				return ManualConnectMessageBox.sSingelton;
			}
		}

		// Token: 0x1400001B RID: 27
		// (add) Token: 0x06002FFC RID: 12284 RVA: 0x001879A0 File Offset: 0x00185BA0
		// (remove) Token: 0x06002FFD RID: 12285 RVA: 0x001879B9 File Offset: 0x00185BB9
		public event Action<IPAddress, string> Complete;

		// Token: 0x06002FFE RID: 12286 RVA: 0x001879D4 File Offset: 0x00185BD4
		private ManualConnectMessageBox() : base(LanguageManager.Instance.GetString(SubMenuOnline.LOC_IP))
		{
			this.mAddress = new StringBuilder(32, 32);
			this.mPassword = new StringBuilder(32, 32);
			this.mAddressText = new Text(32, this.mFont, TextAlign.Left, true, false);
			this.mAddressText.DefaultColor = MenuItem.COLOR;
			this.mAddressRect.Width = 320;
			this.mAddressRect.Height = this.mFont.LineHeight * 2;
			this.mPasswordText = new Text(32, this.mFont, TextAlign.Left, true, false);
			this.mPasswordText.DefaultColor = MenuItem.COLOR;
			this.mPasswordRect.Width = 320;
			this.mPasswordRect.Height = this.mFont.LineHeight * 2;
			this.mPasswordTitle = new Text(32, this.mFont, TextAlign.Center, false);
			this.mPasswordTitle.SetText(LanguageManager.Instance.GetString(SubMenuOnline.LOC_SETTINGS_PASSWORD));
			this.mPasswordTitle.DefaultColor = MenuItem.COLOR;
			Vector2 iPosition = default(Vector2);
			iPosition.X = this.mCenter.X + 16f;
			iPosition.Y = this.mCenter.Y + this.mSize.Y * 0.5f - 16f - (float)this.mFont.LineHeight;
			this.mCancelButton = new MenuTextItem(Defines.LOC_GEN_CANCEL, iPosition, this.mFont, TextAlign.Left);
			this.mCancelButton.ColorDisabled = MenuItem.COLOR_SELECTED * 0.5f;
			this.mCancelButton.Color = MenuItem.COLOR;
			this.mCancelButton.ColorSelected = Vector4.One;
			iPosition.X = this.mCenter.X - 16f;
			this.mOkButton = new MenuTextItem(Defines.LOC_GEN_OK, iPosition, this.mFont, TextAlign.Right);
			this.mOkButton.ColorDisabled = MenuItem.COLOR_SELECTED * 0.5f;
			this.mOkButton.Color = MenuItem.COLOR;
			this.mOkButton.ColorSelected = Vector4.One;
		}

		// Token: 0x06002FFF RID: 12287 RVA: 0x00187C04 File Offset: 0x00185E04
		public override void LanguageChanged()
		{
			base.LanguageChanged();
			this.mCancelButton.LanguageChanged();
			this.mOkButton.LanguageChanged();
			this.mMessage.SetText(LanguageManager.Instance.GetString(SubMenuOnline.LOC_IP));
			this.mPasswordTitle.SetText(LanguageManager.Instance.GetString(SubMenuOnline.LOC_SETTINGS_PASSWORD));
		}

		// Token: 0x06003000 RID: 12288 RVA: 0x00187C61 File Offset: 0x00185E61
		public override void Show()
		{
			this.mSelectedPosition = ManualConnectMessageBox.Items.IP;
			this.mEditPassword = false;
			base.Show();
		}

		// Token: 0x06003001 RID: 12289 RVA: 0x00187C77 File Offset: 0x00185E77
		private void DaisyWheelIPInput(string text)
		{
			DaisyWheel.SetActionToCallWhenComplete(null);
			this.mAddress = new StringBuilder(text);
			this.mAddressText.SetText(text);
		}

		// Token: 0x06003002 RID: 12290 RVA: 0x00187C97 File Offset: 0x00185E97
		private void DaisyWheelPasswordInput(string text)
		{
			DaisyWheel.SetActionToCallWhenComplete(null);
			this.mPassword = new StringBuilder(text);
			this.mPasswordText.SetText(text);
		}

		// Token: 0x06003003 RID: 12291 RVA: 0x00187CB8 File Offset: 0x00185EB8
		public override void OnTextInput(char iChar)
		{
			if (this.mEditPassword)
			{
				if (iChar == '\b')
				{
					if (this.mPassword.Length > 0)
					{
						this.mPassword.Length--;
					}
				}
				else if (this.mPassword.Length < 10)
				{
					this.mPassword.Append(iChar);
				}
				this.mPasswordText.SetText(this.mPassword.ToString());
			}
			if (this.mEditAddress)
			{
				if (iChar == '\b')
				{
					if (this.mAddress.Length > 0)
					{
						this.mAddress.Length--;
					}
				}
				else if (this.mAddress.Length < 25 & (char.IsDigit(iChar) | iChar == '.'))
				{
					this.mAddress.Append(iChar);
				}
				this.mAddressText.SetText(this.mAddress.ToString());
			}
		}

		// Token: 0x06003004 RID: 12292 RVA: 0x00187D98 File Offset: 0x00185F98
		public override void OnMove(Controller iSender, ControllerDirection iDirection)
		{
			switch (iDirection)
			{
			case ControllerDirection.Right:
				this.mSelectedPosition++;
				if (this.mSelectedPosition >= ManualConnectMessageBox.Items.NrOfItems)
				{
					this.mSelectedPosition = ManualConnectMessageBox.Items.IP;
				}
				break;
			case ControllerDirection.Up:
				this.mSelectedPosition--;
				if (this.mSelectedPosition < ManualConnectMessageBox.Items.IP)
				{
					this.mSelectedPosition = ManualConnectMessageBox.Items.Cancel;
				}
				break;
			case ControllerDirection.UpRight:
				break;
			case ControllerDirection.Left:
				this.mSelectedPosition--;
				if (this.mSelectedPosition < ManualConnectMessageBox.Items.IP)
				{
					this.mSelectedPosition = ManualConnectMessageBox.Items.Cancel;
				}
				break;
			default:
				if (iDirection == ControllerDirection.Down)
				{
					this.mSelectedPosition++;
					if (this.mSelectedPosition >= ManualConnectMessageBox.Items.NrOfItems)
					{
						this.mSelectedPosition = ManualConnectMessageBox.Items.IP;
					}
				}
				break;
			}
			Console.WriteLine("Position: " + this.mSelectedPosition.ToString());
		}

		// Token: 0x06003005 RID: 12293 RVA: 0x00187E64 File Offset: 0x00186064
		public override void OnMouseMove(MouseState iNewState, MouseState iOldState)
		{
			if (this.mAddressRect.Contains(iNewState.X, iNewState.Y))
			{
				this.mSelectedPosition = ManualConnectMessageBox.Items.IP;
				return;
			}
			if (this.mPasswordRect.Contains(iNewState.X, iNewState.Y))
			{
				this.mSelectedPosition = ManualConnectMessageBox.Items.Password;
				return;
			}
			if (this.mOkButton.InsideBounds((float)iNewState.X, (float)iNewState.Y))
			{
				this.mSelectedPosition = ManualConnectMessageBox.Items.Ok;
				return;
			}
			if (this.mCancelButton.InsideBounds((float)iNewState.X, (float)iNewState.Y))
			{
				this.mSelectedPosition = ManualConnectMessageBox.Items.Cancel;
				return;
			}
			this.mSelectedPosition = ManualConnectMessageBox.Items.Invalid;
		}

		// Token: 0x06003006 RID: 12294 RVA: 0x00187F08 File Offset: 0x00186108
		public override void OnMouseClick(MouseState iNewState, MouseState iOldState)
		{
			if (iNewState.LeftButton == ButtonState.Pressed)
			{
				return;
			}
			Vector2 vector = default(Vector2);
			vector.X = (float)iNewState.X;
			vector.Y = (float)iNewState.Y;
			int lineHeight = this.mFont.LineHeight;
			this.mEditAddress = false;
			this.mEditPassword = false;
			if (this.mPasswordRect.Contains(iNewState.X, iNewState.Y))
			{
				this.mEditPassword = true;
			}
			if (this.mAddressRect.Contains(iNewState.X, iNewState.Y))
			{
				this.mEditAddress = true;
			}
			if (this.mOkButton.InsideBounds(ref vector))
			{
				this.mSelectedPosition = ManualConnectMessageBox.Items.Ok;
				this.OnSelect(ControlManager.Instance.MenuController);
			}
			if (this.mCancelButton.InsideBounds(ref vector))
			{
				this.mSelectedPosition = ManualConnectMessageBox.Items.Cancel;
				this.OnSelect(ControlManager.Instance.MenuController);
			}
		}

		// Token: 0x06003007 RID: 12295 RVA: 0x00187FF0 File Offset: 0x001861F0
		public override void OnSelect(Controller iSender)
		{
			if (this.mEditAddress)
			{
				this.mEditAddress = false;
			}
			if (this.mEditPassword)
			{
				this.mEditPassword = false;
			}
			if (!DaisyWheel.IsDisplaying && iSender is XInputController && (this.mSelectedPosition == ManualConnectMessageBox.Items.IP || this.mSelectedPosition == ManualConnectMessageBox.Items.Password))
			{
				this.DaisyWheelInput(iSender);
				return;
			}
			if (DaisyWheel.IsDisplaying)
			{
				return;
			}
			switch (this.mSelectedPosition)
			{
			case ManualConnectMessageBox.Items.IP:
				this.mEditAddress = true;
				return;
			case ManualConnectMessageBox.Items.Password:
				this.mEditPassword = true;
				return;
			case ManualConnectMessageBox.Items.Ok:
			{
				IPAddress ipaddress;
				if (IPAddress.TryParse(this.mAddress.ToString(), out ipaddress))
				{
					if (this.Complete != null)
					{
						if (string.IsNullOrEmpty(this.mPassword.ToString()))
						{
							this.Complete.Invoke(ipaddress, null);
						}
						else
						{
							this.Complete.Invoke(ipaddress, this.mPassword.ToString());
						}
					}
					this.Kill();
					return;
				}
				ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString("#add_menu_err".GetHashCodeCustom()));
				return;
			}
			case ManualConnectMessageBox.Items.Cancel:
				this.Kill();
				return;
			default:
				this.Kill();
				return;
			}
		}

		// Token: 0x06003008 RID: 12296 RVA: 0x00188104 File Offset: 0x00186304
		private void DaisyWheelInput(Controller iSender)
		{
			switch (this.mSelectedPosition)
			{
			case ManualConnectMessageBox.Items.IP:
				this.mEditAddress = true;
				DaisyWheel.SetActionToCallWhenComplete(new Action<string>(this.DaisyWheelIPInput));
				if (!DaisyWheel.TryShow(iSender, LanguageManager.Instance.GetString(ManualConnectMessageBox.LOC_IP).ToUpper()))
				{
					DaisyWheel.SetActionToCallWhenComplete(null);
					return;
				}
				this.mAddress = new StringBuilder("");
				this.mAddressText.SetText("");
				return;
			case ManualConnectMessageBox.Items.Password:
				this.mEditPassword = true;
				DaisyWheel.SetActionToCallWhenComplete(new Action<string>(this.DaisyWheelPasswordInput));
				if (!DaisyWheel.TryShow(iSender, LanguageManager.Instance.GetString(ManualConnectMessageBox.LOC_PASSWORD).ToUpper(), false, GamepadTextInputLineMode.GamepadTextInputLineModeSingleLine, 11U))
				{
					DaisyWheel.SetActionToCallWhenComplete(null);
					return;
				}
				this.mPassword = new StringBuilder("");
				this.mPasswordText.SetText("");
				return;
			default:
				return;
			}
		}

		// Token: 0x06003009 RID: 12297 RVA: 0x001881E4 File Offset: 0x001863E4
		public override void Draw(float iDeltaTime)
		{
			base.Draw(iDeltaTime);
			Vector2 mCenter = this.mCenter;
			this.mTimer -= iDeltaTime;
			while (this.mTimer < 0f)
			{
				this.mTimer += 0.5f;
				this.mLine = !this.mLine;
				if (this.mEditAddress)
				{
					if (this.mLine)
					{
						this.mAddressText.Characters[this.mAddress.Length] = '_';
						this.mAddressText.Characters[this.mAddress.Length + 1] = '\0';
					}
					else
					{
						this.mAddressText.Characters[this.mAddress.Length] = '\0';
					}
				}
				else
				{
					this.mAddressText.Characters[this.mAddress.Length] = '\0';
				}
				if (this.mEditPassword)
				{
					if (this.mLine)
					{
						this.mPasswordText.Characters[this.mPassword.Length] = '_';
						this.mPasswordText.Characters[this.mPassword.Length + 1] = '\0';
					}
					else
					{
						this.mPasswordText.Characters[this.mPassword.Length] = '\0';
					}
				}
				else
				{
					this.mPasswordText.Characters[this.mPassword.Length] = '\0';
				}
				this.mPasswordText.MarkAsDirty();
				this.mAddressText.MarkAsDirty();
			}
			Vector4 color = MenuItem.COLOR;
			Matrix transform = default(Matrix);
			transform.M11 = (transform.M22 = (transform.M33 = 1f));
			transform.M44 = 1f;
			transform.M41 = mCenter.X;
			transform.M42 = mCenter.Y;
			MessageBox.sGUIBasicEffect.Transform = transform;
			MessageBox.sGUIBasicEffect.Color = new Vector4(1f, 1f, 1f, this.mAlpha);
			MessageBox.sGUIBasicEffect.TextureEnabled = true;
			float num = (float)this.mFont.LineHeight;
			mCenter.Y -= num * 3f;
			color = ((this.mSelectedPosition == ManualConnectMessageBox.Items.IP) ? MenuItem.COLOR_SELECTED : MenuItem.COLOR);
			color.W *= this.mAlpha;
			MessageBox.sGUIBasicEffect.Color = color;
			this.mMessage.Draw(MessageBox.sGUIBasicEffect, mCenter.X, mCenter.Y);
			this.mAddressRect.X = (int)mCenter.X - 160;
			this.mAddressRect.Y = (int)mCenter.Y;
			mCenter.Y += num;
			this.mAddressText.Draw(MessageBox.sGUIBasicEffect, mCenter.X - 120f, mCenter.Y);
			mCenter.Y += num;
			color = ((this.mSelectedPosition == ManualConnectMessageBox.Items.Password) ? MenuItem.COLOR_SELECTED : MenuItem.COLOR);
			color.W *= this.mAlpha;
			MessageBox.sGUIBasicEffect.Color = color;
			this.mPasswordTitle.Draw(MessageBox.sGUIBasicEffect, mCenter.X, mCenter.Y);
			this.mPasswordRect.X = (int)mCenter.X - 160;
			this.mPasswordRect.Y = (int)mCenter.Y;
			mCenter.Y += num;
			this.mPasswordText.Draw(MessageBox.sGUIBasicEffect, mCenter.X - 120f, mCenter.Y);
			this.mOkButton.Selected = (this.mSelectedPosition == ManualConnectMessageBox.Items.Ok);
			this.mOkButton.Alpha = this.mAlpha;
			this.mOkButton.Draw(MessageBox.sGUIBasicEffect);
			this.mCancelButton.Selected = (this.mSelectedPosition == ManualConnectMessageBox.Items.Cancel);
			this.mCancelButton.Alpha = this.mAlpha;
			this.mCancelButton.Draw(MessageBox.sGUIBasicEffect);
			MessageBox.sGUIBasicEffect.CurrentTechnique.Passes[0].End();
			MessageBox.sGUIBasicEffect.End();
		}

		// Token: 0x040033FF RID: 13311
		private static ManualConnectMessageBox sSingelton;

		// Token: 0x04003400 RID: 13312
		private static volatile object sSingeltonLock = new object();

		// Token: 0x04003401 RID: 13313
		private static readonly int LOC_IP = "#network_03".GetHashCodeCustom();

		// Token: 0x04003402 RID: 13314
		private static readonly int LOC_PASSWORD = "#settings_p01_password".GetHashCodeCustom();

		// Token: 0x04003403 RID: 13315
		private bool mEditAddress;

		// Token: 0x04003404 RID: 13316
		private bool mEditPassword;

		// Token: 0x04003405 RID: 13317
		private ManualConnectMessageBox.Items mSelectedPosition;

		// Token: 0x04003406 RID: 13318
		private MenuTextItem mCancelButton;

		// Token: 0x04003407 RID: 13319
		private MenuTextItem mOkButton;

		// Token: 0x04003408 RID: 13320
		private Text mAddressText;

		// Token: 0x04003409 RID: 13321
		private StringBuilder mAddress;

		// Token: 0x0400340A RID: 13322
		private Rectangle mAddressRect;

		// Token: 0x0400340B RID: 13323
		private Text mPasswordTitle;

		// Token: 0x0400340C RID: 13324
		private Text mPasswordText;

		// Token: 0x0400340D RID: 13325
		private StringBuilder mPassword;

		// Token: 0x0400340E RID: 13326
		private Rectangle mPasswordRect;

		// Token: 0x0400340F RID: 13327
		private Controller mSender;

		// Token: 0x04003410 RID: 13328
		private float mTimer;

		// Token: 0x04003411 RID: 13329
		private bool mLine;

		// Token: 0x02000635 RID: 1589
		private enum Items
		{
			// Token: 0x04003414 RID: 13332
			Invalid = -1,
			// Token: 0x04003415 RID: 13333
			IP,
			// Token: 0x04003416 RID: 13334
			Password,
			// Token: 0x04003417 RID: 13335
			Ok,
			// Token: 0x04003418 RID: 13336
			Cancel,
			// Token: 0x04003419 RID: 13337
			NrOfItems
		}
	}
}
