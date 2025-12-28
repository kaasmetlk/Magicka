using System;
using System.Text;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;

namespace Magicka.GameLogic.UI
{
	// Token: 0x0200048B RID: 1163
	internal class TextInputMessageBox : MessageBox
	{
		// Token: 0x17000867 RID: 2151
		// (set) Token: 0x0600233D RID: 9021 RVA: 0x000FC0C8 File Offset: 0x000FA2C8
		public bool SecureMode
		{
			set
			{
				this.mSecureMode = value;
			}
		}

		// Token: 0x17000868 RID: 2152
		// (set) Token: 0x0600233E RID: 9022 RVA: 0x000FC0D1 File Offset: 0x000FA2D1
		public TextInputMessageBox.MessageBoxInputType InputType
		{
			set
			{
				this.mInputType = value;
			}
		}

		// Token: 0x0600233F RID: 9023 RVA: 0x000FC0DC File Offset: 0x000FA2DC
		public TextInputMessageBox(string iMessage, int iMaxLength) : base(iMessage)
		{
			this.mMessageHash = 0;
			this.mString = new StringBuilder(iMaxLength, iMaxLength);
			this.mText = new Text(iMaxLength + 2, this.mFont, TextAlign.Left, true, false);
			this.mBlinkerText = new Text(iMaxLength + 2, this.mFont, TextAlign.Left, true, false);
			string @string = LanguageManager.Instance.GetString(Defines.LOC_GEN_OK);
			this.mMaxLength = iMaxLength;
			Vector2 vector = this.mFont.MeasureText(@string, true);
			Vector2 iPosition;
			iPosition.X = this.mCenter.X - 50f;
			iPosition.Y = this.mCenter.Y + 128f - vector.Y;
			this.mOkButton = new MenuTextItem(Defines.LOC_GEN_OK, iPosition, this.mFont, TextAlign.Right);
			this.mOkButton.ColorSelected = Vector4.One;
			this.mOkButton.Color = MenuItem.COLOR;
			@string = LanguageManager.Instance.GetString(Defines.LOC_GEN_CANCEL);
			iPosition.X = this.mCenter.X + 50f;
			this.mCancelButton = new MenuTextItem(Defines.LOC_GEN_CANCEL, iPosition, this.mFont, TextAlign.Left);
			this.mCancelButton.ColorSelected = Vector4.One;
			this.mCancelButton.Color = MenuItem.COLOR;
			LanguageManager.Instance.LanguageChanged += new Action(this.LanguageChanged);
			ResolutionMessageBox.Instance.Complete += this.ChangeResolution;
		}

		// Token: 0x06002340 RID: 9024 RVA: 0x000FC25F File Offset: 0x000FA45F
		public TextInputMessageBox(int iMessage, int iMaxLength) : this(LanguageManager.Instance.GetString(iMessage), iMaxLength)
		{
			this.mMessageHash = iMessage;
			LanguageManager.Instance.LanguageChanged += new Action(this.LanguageChanged);
		}

		// Token: 0x06002341 RID: 9025 RVA: 0x000FC294 File Offset: 0x000FA494
		private void ChangeResolution(ResolutionData iData)
		{
			Vector2 vector = this.mFont.MeasureText(LanguageManager.Instance.GetString(Defines.LOC_GEN_OK), true);
			this.mCenter = new Vector2((float)iData.Width, (float)iData.Height) * 0.5f;
			this.mOkButton.Position = this.mCenter + new Vector2(-50f, 128f - vector.Y);
			this.mCancelButton.Position = this.mCenter + new Vector2(50f, 128f - vector.Y);
		}

		// Token: 0x06002342 RID: 9026 RVA: 0x000FC33C File Offset: 0x000FA53C
		public override void LanguageChanged()
		{
			base.LanguageChanged();
			this.mOkButton.LanguageChanged();
			this.mCancelButton.LanguageChanged();
			if (this.mMessageHash != 0)
			{
				this.mMessage.SetText(LanguageManager.Instance.GetString(this.mMessageHash));
			}
		}

		// Token: 0x06002343 RID: 9027 RVA: 0x000FC388 File Offset: 0x000FA588
		public void Show(Action<string> iCallback)
		{
			this.Show(iCallback, null);
		}

		// Token: 0x06002344 RID: 9028 RVA: 0x000FC392 File Offset: 0x000FA592
		public void Show(Action<string> iCallback, Controller iSender)
		{
			this.Show(iCallback, iSender, "");
		}

		// Token: 0x06002345 RID: 9029 RVA: 0x000FC3A1 File Offset: 0x000FA5A1
		public void Show(Action<string> iCallback, Controller iSender, string iDescr)
		{
			this.Show(iCallback, iSender, iDescr, false);
		}

		// Token: 0x06002346 RID: 9030 RVA: 0x000FC3B0 File Offset: 0x000FA5B0
		public void Show(Action<string> iCallback, Controller iSender, string iDescr, bool isPassword)
		{
			this.mComplete = iCallback;
			base.Show();
			this.mString.Length = 0;
			if (this.mInputType == TextInputMessageBox.MessageBoxInputType.Date)
			{
				this.mCharacterIndex = 0;
				this.mString.Append("MM/DD/YYYY");
				this.mText.SetText("MM/DD/YYYY");
			}
			else
			{
				this.mText.SetText(null);
			}
			if (iSender is XInputController)
			{
				DaisyWheel.SetActionToCallWhenComplete(new Action<string>(this._DaisyWheelTextRecived));
				if (!DaisyWheel.TryShow(iSender, iDescr, isPassword))
				{
					DaisyWheel.SetActionToCallWhenComplete(null);
				}
			}
		}

		// Token: 0x06002347 RID: 9031 RVA: 0x000FC43F File Offset: 0x000FA63F
		private void _DaisyWheelTextRecived(string enteredText)
		{
			DaisyWheel.SetActionToCallWhenComplete(null);
			if (enteredText != null)
			{
				this.mString = new StringBuilder(enteredText);
				this.mText.SetText(enteredText);
				if (this.mComplete != null)
				{
					this.mComplete(enteredText);
				}
				this.Kill();
			}
		}

		// Token: 0x06002348 RID: 9032 RVA: 0x000FC47C File Offset: 0x000FA67C
		public override void Show()
		{
			throw new InvalidOperationException();
		}

		// Token: 0x06002349 RID: 9033 RVA: 0x000FC484 File Offset: 0x000FA684
		public override void OnMove(Controller iSender, ControllerDirection iDirection)
		{
			if (DaisyWheel.IsDisplaying)
			{
				return;
			}
			if (iDirection == ControllerDirection.Left)
			{
				this.HandleBackspace();
			}
			this.mText.SetText(this.mSecureMode ? this.GetSecureStr(this.mString.ToString()) : this.mString.ToString());
		}

		// Token: 0x0600234A RID: 9034 RVA: 0x000FC4D4 File Offset: 0x000FA6D4
		public override void OnMouseMove(MouseState iNewState, MouseState iOldState)
		{
			if (DaisyWheel.IsDisplaying)
			{
				return;
			}
			this.mCancelButton.Selected = false;
			this.mOkButton.Selected = false;
			if (this.mOkButton.InsideBounds((float)iNewState.X, (float)iNewState.Y))
			{
				this.mOkButton.Selected = true;
				return;
			}
			if (this.mCancelButton.InsideBounds((float)iNewState.X, (float)iNewState.Y))
			{
				this.mCancelButton.Selected = true;
			}
		}

		// Token: 0x0600234B RID: 9035 RVA: 0x000FC554 File Offset: 0x000FA754
		public override void OnMouseClick(MouseState iNewState, MouseState iOldState)
		{
			if (DaisyWheel.IsDisplaying)
			{
				return;
			}
			if (iNewState.LeftButton == ButtonState.Released)
			{
				if (this.mOkButton.InsideBounds((float)iNewState.X, (float)iNewState.Y))
				{
					this.OnSelect(ControlManager.Instance.MenuController);
					return;
				}
				if (this.mCancelButton.InsideBounds((float)iNewState.X, (float)iNewState.Y))
				{
					if (this.mComplete != null)
					{
						this.mComplete("");
					}
					this.Kill();
				}
			}
		}

		// Token: 0x0600234C RID: 9036 RVA: 0x000FC5DC File Offset: 0x000FA7DC
		public override void OnTextInput(char iChar)
		{
			if (DaisyWheel.IsDisplaying)
			{
				return;
			}
			bool flag = this.mInputType == TextInputMessageBox.MessageBoxInputType.Date;
			if (iChar == '\b')
			{
				this.HandleBackspace();
				return;
			}
			if ((flag && this.mCharacterIndex < "MM/DD/YYYY".Length) || (!flag && this.mString.Length < this.mString.Capacity))
			{
				this.HandleNewCharacter(iChar);
			}
		}

		// Token: 0x0600234D RID: 9037 RVA: 0x000FC640 File Offset: 0x000FA840
		private string WrapText()
		{
			int num = (int)(this.mSize.X * 0.8f);
			string text = this.mString.ToString();
			text = (this.mSecureMode ? this.GetSecureStr(text) : text);
			if (this.mFont.MeasureText(text, true).X > (float)num)
			{
				string text2 = string.Empty;
				float num2 = 0f;
				int num3 = 0;
				do
				{
					char c = text[num3];
					float x = this.mFont.MeasureText(text, true, 1).X;
					if (num2 + x >= (float)num)
					{
						text2 += "\n";
						num2 = 0f;
					}
					text2 += c;
					num2 += x;
					num3++;
				}
				while (num3 != text.Length);
				text = text2;
			}
			return text;
		}

		// Token: 0x0600234E RID: 9038 RVA: 0x000FC70C File Offset: 0x000FA90C
		private void HandleNewCharacter(char iChar)
		{
			string empty = string.Empty;
			if (this.mInputType == TextInputMessageBox.MessageBoxInputType.Standard || (this.mInputType == TextInputMessageBox.MessageBoxInputType.NumericOnly && char.IsNumber(iChar)))
			{
				this.mString.Append(iChar);
			}
			else if (this.mInputType == TextInputMessageBox.MessageBoxInputType.Date && char.IsNumber(iChar))
			{
				this.mCharacterIndex = Math.Max(0, this.mCharacterIndex);
				this.mString[this.mCharacterIndex] = iChar;
				this.mCharacterIndex++;
				if (this.mCharacterIndex < "MM/DD/YYYY".Length - 1 && "MM/DD/YYYY"[this.mCharacterIndex] == '/')
				{
					this.mCharacterIndex++;
				}
			}
			this.mText.SetText(this.mSecureMode ? this.GetSecureStr(this.mString.ToString()) : this.mString.ToString());
		}

		// Token: 0x0600234F RID: 9039 RVA: 0x000FC7F4 File Offset: 0x000FA9F4
		private void HandleBackspace()
		{
			bool flag = this.mInputType == TextInputMessageBox.MessageBoxInputType.Date;
			if (!flag && this.mString.Length > 0)
			{
				this.mString.Length--;
			}
			else if (flag && this.mCharacterIndex >= 0)
			{
				if (this.mCharacterIndex == 10)
				{
					this.mCharacterIndex = 9;
					this.mString[this.mCharacterIndex] = "MM/DD/YYYY"[this.mCharacterIndex];
				}
				else
				{
					this.mCharacterIndex--;
					if (this.mCharacterIndex > 0 && "MM/DD/YYYY"[this.mCharacterIndex] == '/')
					{
						this.mCharacterIndex--;
					}
					if (this.mCharacterIndex >= 0)
					{
						this.mString[this.mCharacterIndex] = "MM/DD/YYYY"[this.mCharacterIndex];
					}
				}
			}
			this.mText.SetText(this.mSecureMode ? this.GetSecureStr(this.mString.ToString()) : this.mString.ToString());
		}

		// Token: 0x06002350 RID: 9040 RVA: 0x000FC90F File Offset: 0x000FAB0F
		private string GetSecureStr(string iStr)
		{
			return new string('*', iStr.Length);
		}

		// Token: 0x06002351 RID: 9041 RVA: 0x000FC920 File Offset: 0x000FAB20
		public override void OnSelect(Controller iSender)
		{
			if (this.mAlpha < 1f | this.mDead)
			{
				return;
			}
			if (DaisyWheel.IsDisplaying)
			{
				return;
			}
			if (this.mComplete != null)
			{
				this.mComplete(this.mString.ToString());
			}
			this.Kill();
		}

		// Token: 0x06002352 RID: 9042 RVA: 0x000FC970 File Offset: 0x000FAB70
		public override void Draw(float iDeltaTime)
		{
			if (DaisyWheel.IsDisplaying)
			{
				return;
			}
			Matrix transform = default(Matrix);
			Vector4 color = default(Vector4);
			Point screenSize = RenderManager.Instance.ScreenSize;
			this.mCenter.X = (float)screenSize.X * 0.5f;
			this.mCenter.Y = (float)screenSize.Y * 0.5f;
			MessageBox.sGUIBasicEffect.SetScreenSize(screenSize.X, screenSize.Y);
			if (this.mDead)
			{
				this.mAlpha = Math.Max(this.mAlpha - iDeltaTime * 4f, 0f);
			}
			else
			{
				this.mAlpha = Math.Min(this.mAlpha + iDeltaTime * 4f, 1f);
			}
			transform.M12 = 1f;
			transform.M21 = -1f;
			transform.M33 = 1f;
			transform.M44 = 1f;
			transform.M41 = this.mCenter.X;
			transform.M42 = this.mCenter.Y;
			MessageBox.sGUIBasicEffect.Transform = transform;
			color.X = (color.Y = (color.Z = 1f));
			color.W = this.mAlpha;
			MessageBox.sGUIBasicEffect.Color = color;
			MessageBox.sGUIBasicEffect.VertexColorEnabled = false;
			MessageBox.sGUIBasicEffect.Texture = MessageBox.sTexture;
			MessageBox.sGUIBasicEffect.TextureEnabled = true;
			MessageBox.sGUIBasicEffect.GraphicsDevice.Vertices[0].SetSource(MessageBox.sVertexBuffer, 0, 16);
			MessageBox.sGUIBasicEffect.GraphicsDevice.VertexDeclaration = MessageBox.sVertexDeclaration;
			MessageBox.sGUIBasicEffect.Begin();
			MessageBox.sGUIBasicEffect.CurrentTechnique.Passes[0].Begin();
			MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
			Vector2 mCenter = this.mCenter;
			this.mTimer -= iDeltaTime;
			while (this.mTimer < 0f)
			{
				this.mTimer += 0.5f;
				this.mLine = !this.mLine;
				if (this.mLine)
				{
					this.mBlinkerText.SetText("_\0");
				}
				else
				{
					this.mBlinkerText.SetText("\0");
				}
				this.mText.MarkAsDirty();
			}
			color.X = (color.Y = (color.Z = 1f));
			color.W = this.mAlpha;
			MessageBox.sGUIBasicEffect.Color = color;
			string text = this.WrapText();
			string[] array = text.Split(new char[]
			{
				'\n'
			});
			int num = array.Length;
			float num2 = (float)this.mFont.LineHeight * 0.5f;
			MessageBox.sGUIBasicEffect.VertexColorEnabled = true;
			float num3 = this.mMessageHeight + ((num > 1) ? ((float)num * num2) : 0f);
			mCenter.Y -= num3;
			this.mMessage.Draw(MessageBox.sGUIBasicEffect, mCenter.X, mCenter.Y);
			mCenter.Y += num3;
			MessageBox.sGUIBasicEffect.VertexColorEnabled = false;
			this.mText.Font.MeasureText(this.mText.Characters, true, this.mString.Length);
			transform.M11 = 1f;
			transform.M12 = 0f;
			transform.M21 = 0f;
			transform.M22 = 1f;
			transform.M44 = 1f;
			float num4 = (mCenter.Y - ((num > 1) ? ((float)num * num2) : 0f)) * transform.M11;
			for (int i = 0; i < num; i++)
			{
				this.mText.SetText(array[i]);
				float x = this.mText.Font.MeasureText(array[i], true, array[i].Length).X;
				transform.M41 = (float)Math.Floor((double)(mCenter.X - 0.5f * x * transform.M11));
				transform.M42 = num4;
				this.mText.Draw(MessageBox.sGUIBasicEffect, ref transform);
				num4 += (float)this.mFont.LineHeight * transform.M11;
			}
			if (this.mInputType == TextInputMessageBox.MessageBoxInputType.Date)
			{
				if (this.mCharacterIndex < "MM/DD/YYYY".Length)
				{
					transform.M41 += this.mText.Font.MeasureText(this.mText.Characters, true, this.mCharacterIndex).X;
					this.mBlinkerText.Draw(MessageBox.sGUIBasicEffect, ref transform);
				}
			}
			else if (this.mString.Length < this.mString.Capacity)
			{
				if (num == 1)
				{
					transform.M41 += this.mText.Font.MeasureText(this.mText.Characters, true, this.mString.Length).X;
				}
				else
				{
					int num5 = text.LastIndexOf('\n');
					text = text.Substring(num5 + 1);
					float x2 = this.mText.Font.MeasureText(text, true, text.Length).X;
					transform.M41 += x2;
				}
				this.mBlinkerText.Draw(MessageBox.sGUIBasicEffect, ref transform);
			}
			float num6 = (num > 3) ? ((float)(num - 3) * num2) : 0f;
			Vector2 vector = this.mFont.MeasureText(LanguageManager.Instance.GetString(Defines.LOC_GEN_OK), true);
			this.mOkButton.Position = this.mCenter + new Vector2(-50f, 128f - vector.Y + num6);
			this.mCancelButton.Position = this.mCenter + new Vector2(50f, 128f - vector.Y + num6);
			this.mOkButton.Alpha = this.mAlpha;
			this.mOkButton.Draw(MessageBox.sGUIBasicEffect);
			this.mCancelButton.Alpha = this.mAlpha;
			this.mCancelButton.Draw(MessageBox.sGUIBasicEffect);
			MessageBox.sGUIBasicEffect.CurrentTechnique.Passes[0].End();
			MessageBox.sGUIBasicEffect.End();
		}

		// Token: 0x17000869 RID: 2153
		// (get) Token: 0x06002353 RID: 9043 RVA: 0x000FD006 File Offset: 0x000FB206
		public override int ZIndex
		{
			get
			{
				return 2002;
			}
		}

		// Token: 0x04002643 RID: 9795
		private const string DEFAULT_DATE_FORMAT = "MM/DD/YYYY";

		// Token: 0x04002644 RID: 9796
		private const int DATE_LENGTH = 10;

		// Token: 0x04002645 RID: 9797
		private const float BUTTON_SPREAD = 50f;

		// Token: 0x04002646 RID: 9798
		private const float BUTTON_OFFSET = 128f;

		// Token: 0x04002647 RID: 9799
		private const int MIN_LINES_TXT = 1;

		// Token: 0x04002648 RID: 9800
		private const int MIN_LINES_BTN = 3;

		// Token: 0x04002649 RID: 9801
		private const float MAX_WIDTH_PERCENT = 0.8f;

		// Token: 0x0400264A RID: 9802
		private MenuTextItem mOkButton;

		// Token: 0x0400264B RID: 9803
		private MenuTextItem mCancelButton;

		// Token: 0x0400264C RID: 9804
		private Text mText;

		// Token: 0x0400264D RID: 9805
		private Text mBlinkerText;

		// Token: 0x0400264E RID: 9806
		private StringBuilder mString;

		// Token: 0x0400264F RID: 9807
		private float mTimer;

		// Token: 0x04002650 RID: 9808
		private bool mLine;

		// Token: 0x04002651 RID: 9809
		private Action<string> mComplete;

		// Token: 0x04002652 RID: 9810
		private string mDaisyInputDescr = "";

		// Token: 0x04002653 RID: 9811
		private bool mDaisyInputIsPsw;

		// Token: 0x04002654 RID: 9812
		private bool mSecureMode;

		// Token: 0x04002655 RID: 9813
		private TextInputMessageBox.MessageBoxInputType mInputType;

		// Token: 0x04002656 RID: 9814
		private int mCharacterIndex;

		// Token: 0x04002657 RID: 9815
		private int mMaxLength;

		// Token: 0x04002658 RID: 9816
		private int mMessageHash;

		// Token: 0x0200048C RID: 1164
		public enum MessageBoxInputType
		{
			// Token: 0x0400265A RID: 9818
			Standard,
			// Token: 0x0400265B RID: 9819
			Date,
			// Token: 0x0400265C RID: 9820
			NumericOnly
		}
	}
}
