using System;
using System.Collections.Generic;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.Graphics;
using Magicka.Localization;
using Magicka.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PolygonHead;

namespace Magicka.GameLogic.UI
{
	// Token: 0x020001B7 RID: 439
	internal class LanguageMessageBox : MessageBox
	{
		// Token: 0x1700033B RID: 827
		// (get) Token: 0x06000D69 RID: 3433 RVA: 0x0004EAA4 File Offset: 0x0004CCA4
		public static LanguageMessageBox Instance
		{
			get
			{
				if (LanguageMessageBox.sSingelton == null)
				{
					lock (LanguageMessageBox.sSingeltonLock)
					{
						if (LanguageMessageBox.sSingelton == null)
						{
							LanguageMessageBox.sSingelton = new LanguageMessageBox();
						}
					}
				}
				return LanguageMessageBox.sSingelton;
			}
		}

		// Token: 0x1400000B RID: 11
		// (add) Token: 0x06000D6A RID: 3434 RVA: 0x0004EAF8 File Offset: 0x0004CCF8
		// (remove) Token: 0x06000D6B RID: 3435 RVA: 0x0004EB11 File Offset: 0x0004CD11
		public event Action<Language> Complete;

		// Token: 0x06000D6C RID: 3436 RVA: 0x0004EB2C File Offset: 0x0004CD2C
		private LanguageMessageBox() : base(LanguageMessageBox.LOC_LANGUAGES)
		{
			this.mFont = Game.Instance.Content.Load<BitmapFont>("UI/Font/LanguageFont");
			float num = (float)this.mFont.LineHeight;
			this.mLanguages = new List<MenuTextItem>();
			LanguageManager instance = LanguageManager.Instance;
			for (int i = 0; i < instance.AllLanguages.Length; i++)
			{
				string nativeName = instance.GetNativeName(instance.AllLanguages[i]);
				this.mLanguages.Add(new MenuTextItem(nativeName, Vector2.Zero, this.mFont, TextAlign.Center));
			}
			Vector2 iPosition = new Vector2(this.mCenter.X + this.mSize.X * 0.5f - 64f, this.mCenter.Y);
			this.mLanguageScrollBar = new MenuScrollBar(iPosition, num * 8f * 2f, this.mLanguages.Count - 8);
			this.mLanguageScrollBar.Scale = 0.5f;
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
			iPosition = new Vector2(this.mCenter.X, this.mCenter.Y + this.mSize.Y * 0.5f - 64f);
			this.mBackItem = new MenuTextItem(SubMenu.LOC_CANCEL, iPosition, font, TextAlign.Center, true);
			this.mBackItem.ColorDisabled = Defines.MESSAGEBOX_COLOR_DEFAULT * 0.5f;
			this.mBackItem.Color = Defines.MESSAGEBOX_COLOR_DEFAULT;
			this.mBackItem.ColorSelected = Vector4.One;
		}

		// Token: 0x06000D6D RID: 3437 RVA: 0x0004ECB9 File Offset: 0x0004CEB9
		public override void LanguageChanged()
		{
			base.LanguageChanged();
			this.mBackItem.LanguageChanged();
		}

		// Token: 0x06000D6E RID: 3438 RVA: 0x0004ECCC File Offset: 0x0004CECC
		public override void Show()
		{
			base.Show();
			this.mLanguageScrollBar.Position = new Vector2(this.mCenter.X + this.mSize.X * 0.5f - 64f, this.mCenter.Y);
			this.mBackItem.Position = new Vector2(this.mCenter.X, this.mCenter.Y + this.mSize.Y * 0.5f - 64f);
		}

		// Token: 0x06000D6F RID: 3439 RVA: 0x0004ED5B File Offset: 0x0004CF5B
		public override void OnTextInput(char iChar)
		{
		}

		// Token: 0x06000D70 RID: 3440 RVA: 0x0004ED60 File Offset: 0x0004CF60
		public override void OnMove(Controller iSender, ControllerDirection iDirection)
		{
			if (iDirection != ControllerDirection.Up)
			{
				if (iDirection != ControllerDirection.Down)
				{
					return;
				}
				if (this.mBackItem.Selected)
				{
					this.mBackItem.Selected = false;
					this.mCurrentLanguage = 0;
					this.mLanguageScrollBar.Value = 0;
					return;
				}
				this.mCurrentLanguage++;
				if (this.mCurrentLanguage >= this.mLanguages.Count)
				{
					this.mCurrentLanguage = -1;
					this.mBackItem.Selected = true;
					return;
				}
				while (this.mCurrentLanguage > this.mLanguageScrollBar.Value + 8)
				{
					this.mLanguageScrollBar.Value++;
				}
				return;
			}
			else
			{
				if (this.mBackItem.Selected)
				{
					this.mBackItem.Selected = false;
					this.mCurrentLanguage = this.mLanguages.Count - 1;
					this.mLanguageScrollBar.Value = this.mLanguageScrollBar.MaxValue;
					return;
				}
				this.mCurrentLanguage--;
				if (this.mCurrentLanguage < 0)
				{
					this.mCurrentLanguage = -1;
					this.mBackItem.Selected = true;
					return;
				}
				while (this.mCurrentLanguage < this.mLanguageScrollBar.Value)
				{
					this.mLanguageScrollBar.Value--;
				}
				return;
			}
		}

		// Token: 0x06000D71 RID: 3441 RVA: 0x0004EE9C File Offset: 0x0004D09C
		public override void OnSelect(Controller iSender)
		{
			if (this.Complete != null & this.mCurrentLanguage >= 0 & this.mCurrentLanguage < this.mLanguages.Count)
			{
				this.Complete(LanguageManager.Instance.AllLanguages[this.mCurrentLanguage]);
			}
			this.Kill();
		}

		// Token: 0x06000D72 RID: 3442 RVA: 0x0004EEFA File Offset: 0x0004D0FA
		public override void Kill()
		{
			base.Kill();
			SaveManager.Instance.SaveSettings();
		}

		// Token: 0x06000D73 RID: 3443 RVA: 0x0004EF0C File Offset: 0x0004D10C
		public override void OnMouseMove(MouseState iNewState, MouseState iOldState)
		{
			if (this.mBackItem.Enabled && this.mBackItem.InsideBounds((float)iNewState.X, (float)iNewState.Y))
			{
				this.mBackItem.Selected = true;
			}
			else if ((float)iNewState.X > this.mCenter.X - this.mSize.X * 0.5f && (float)iNewState.X < this.mCenter.X + this.mSize.X * 0.5f && (float)iNewState.Y > this.mCenter.Y - this.mSize.Y && (float)iNewState.Y < this.mCenter.Y + this.mSize.Y)
			{
				this.mBackItem.Selected = false;
				this.mCurrentLanguage = -1;
				if (this.mLanguageScrollBar.Grabbed)
				{
					this.mLanguageScrollBar.ScrollTo((float)iNewState.Y);
				}
				else
				{
					for (int i = this.mLanguageScrollBar.Value; i < Math.Min(this.mLanguageScrollBar.Value + 8, this.mLanguages.Count); i++)
					{
						MenuItem menuItem = this.mLanguages[i];
						if (menuItem.Enabled & menuItem.InsideBounds(iNewState))
						{
							this.mCurrentLanguage = i;
							break;
						}
					}
				}
			}
			if (iNewState.LeftButton == ButtonState.Released)
			{
				this.mLanguageScrollBar.Grabbed = false;
			}
		}

		// Token: 0x06000D74 RID: 3444 RVA: 0x0004F094 File Offset: 0x0004D294
		public override void OnMouseClick(MouseState iNewState, MouseState iOldState)
		{
			if (this.mBackItem.Enabled && this.mBackItem.InsideBounds((float)iNewState.X, (float)iNewState.Y))
			{
				this.Kill();
				return;
			}
			if (iNewState.ScrollWheelValue > iOldState.ScrollWheelValue)
			{
				this.mLanguageScrollBar.Value--;
				return;
			}
			if (iNewState.ScrollWheelValue < iOldState.ScrollWheelValue)
			{
				this.mLanguageScrollBar.Value++;
				return;
			}
			if (this.mLanguageScrollBar.InsideBounds(iNewState))
			{
				if (this.mLanguageScrollBar.InsideDragBounds(iNewState))
				{
					this.mLanguageScrollBar.Grabbed = true;
					return;
				}
				if (iNewState.LeftButton == ButtonState.Released && iOldState.LeftButton == ButtonState.Pressed)
				{
					this.mLanguageScrollBar.ScrollTo((float)iNewState.Y);
					return;
				}
			}
			else
			{
				for (int i = this.mLanguageScrollBar.Value; i < Math.Min(this.mLanguageScrollBar.Value + 8, this.mLanguages.Count); i++)
				{
					MenuItem menuItem = this.mLanguages[i];
					if (menuItem.Enabled & menuItem.InsideBounds(iNewState))
					{
						this.OnSelect(ControlManager.Instance.MenuController);
						return;
					}
				}
			}
		}

		// Token: 0x06000D75 RID: 3445 RVA: 0x0004F1CC File Offset: 0x0004D3CC
		public override void Draw(float iDeltaTime)
		{
			base.Draw(iDeltaTime);
			float num = (float)this.mFont.LineHeight;
			Vector2 position = new Vector2(this.mCenter.X, this.mCenter.Y - num * 8f * 0.5f);
			position.Y += num * 0.5f;
			for (int i = this.mLanguageScrollBar.Value; i < Math.Min(this.mLanguageScrollBar.Value + 8, this.mLanguages.Count); i++)
			{
				MenuItem menuItem = this.mLanguages[i];
				menuItem.Color = new Vector4(Defines.MESSAGEBOX_COLOR_DEFAULT.X, Defines.MESSAGEBOX_COLOR_DEFAULT.Y, Defines.MESSAGEBOX_COLOR_DEFAULT.Z, this.mAlpha);
				menuItem.Position = position;
				menuItem.Draw(MessageBox.sGUIBasicEffect);
				position.Y += num;
				menuItem.Selected = (this.mCurrentLanguage == i);
			}
			if (this.mLanguageScrollBar.MaxValue > 0)
			{
				this.mLanguageScrollBar.Color = new Vector4(1f, 1f, 1f, this.mAlpha);
				this.mLanguageScrollBar.Draw(MessageBox.sGUIBasicEffect);
			}
			this.mBackItem.Alpha = this.mAlpha;
			this.mBackItem.Position = new Vector2(this.mCenter.X, this.mCenter.Y + this.mSize.Y * 0.5f - 64f);
			this.mBackItem.Draw(MessageBox.sGUIBasicEffect);
			MessageBox.sGUIBasicEffect.CurrentTechnique.Passes[0].End();
			MessageBox.sGUIBasicEffect.End();
		}

		// Token: 0x04000C2C RID: 3116
		private const float BACK_PADDING = 64f;

		// Token: 0x04000C2D RID: 3117
		private const int VISIBLE_LANGUAGES = 8;

		// Token: 0x04000C2E RID: 3118
		private static readonly int LOC_LANGUAGES = "#menu_opt_06".GetHashCodeCustom();

		// Token: 0x04000C2F RID: 3119
		private static LanguageMessageBox sSingelton;

		// Token: 0x04000C30 RID: 3120
		private static volatile object sSingeltonLock = new object();

		// Token: 0x04000C32 RID: 3122
		private MenuScrollBar mLanguageScrollBar;

		// Token: 0x04000C33 RID: 3123
		private List<MenuTextItem> mLanguages;

		// Token: 0x04000C34 RID: 3124
		private MenuTextItem mBackItem;

		// Token: 0x04000C35 RID: 3125
		private int mCurrentLanguage;

		// Token: 0x04000C36 RID: 3126
		private new BitmapFont mFont;
	}
}
