using System;
using System.Collections.Generic;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.UI.Popup;
using Magicka.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;

namespace Magicka.GameLogic.UI
{
	// Token: 0x02000495 RID: 1173
	internal class CountryCodeMessageBox : MessageBox
	{
		// Token: 0x1700087B RID: 2171
		// (get) Token: 0x06002393 RID: 9107 RVA: 0x00100030 File Offset: 0x000FE230
		public static CountryCodeMessageBox Instance
		{
			get
			{
				if (CountryCodeMessageBox.sSingleton == null)
				{
					lock (CountryCodeMessageBox.sSingletonLock)
					{
						if (CountryCodeMessageBox.sSingleton == null)
						{
							CountryCodeMessageBox.sSingleton = new CountryCodeMessageBox();
						}
					}
				}
				return CountryCodeMessageBox.sSingleton;
			}
		}

		// Token: 0x14000012 RID: 18
		// (add) Token: 0x06002394 RID: 9108 RVA: 0x00100084 File Offset: 0x000FE284
		// (remove) Token: 0x06002395 RID: 9109 RVA: 0x0010009D File Offset: 0x000FE29D
		private event Action<string, string> Complete;

		// Token: 0x06002396 RID: 9110 RVA: 0x001000B8 File Offset: 0x000FE2B8
		private CountryCodeMessageBox() : base(CountryCodeMessageBox.LOC_COUNTRYCODE)
		{
			this.mItems = new List<MenuTextItem>();
			this.mCountries = new Dictionary<string, string>();
			Country[] list = Country.List;
			new List<string>();
			for (int i = 0; i < list.Length; i++)
			{
				this.mCountries.Add(list[i].Name, list[i].TwoLetterCode);
				this.mItems.Add(new MenuTextItem(list[i].Name, Vector2.Zero, this.mFont, TextAlign.Center));
			}
			this.mSize *= 1.75f;
			this.mScrollBar = new MenuScrollBar(Vector2.Zero, (float)(this.mFont.LineHeight * 21), this.mItems.Count - 21);
			this.mScrollBar.Scale = 0.65f;
			this.mBackItem = new MenuTextItem(SubMenu.LOC_BACK, Vector2.Zero, this.mFont, TextAlign.Center, true);
			this.mBackItem.ColorDisabled = Defines.MESSAGEBOX_COLOR_DEFAULT * 0.5f;
			this.mBackItem.Color = Defines.MESSAGEBOX_COLOR_DEFAULT;
			this.mBackItem.ColorSelected = Vector4.One;
			ResolutionMessageBox.Instance.Complete += this.ResolutionChanged;
			this.ResolutionChanged(GlobalSettings.Instance.Resolution);
		}

		// Token: 0x06002397 RID: 9111 RVA: 0x0010021A File Offset: 0x000FE41A
		public override void LanguageChanged()
		{
			base.LanguageChanged();
			this.mBackItem.LanguageChanged();
		}

		// Token: 0x06002398 RID: 9112 RVA: 0x00100230 File Offset: 0x000FE430
		private void ResolutionChanged(ResolutionData iData)
		{
			this.mScale = (float)iData.Height / PopupSystem.REFERENCE_SIZE.Y;
			Vector2 vector = this.mSize * this.mScale;
			Vector2 zero = Vector2.Zero;
			this.mCenter = new Vector2((float)iData.Width * 0.5f, (float)iData.Height * 0.5f);
			zero = new Vector2(this.mCenter.X + vector.X * 0.5f - 75f * this.mScale, this.mCenter.Y);
			this.mScrollBar.Position = zero;
			this.mScrollBar.Height = (float)(this.mFont.LineHeight * 21);
			zero = new Vector2(this.mCenter.X, this.mCenter.Y + vector.Y * 0.5f - 70f * this.mScale);
			this.mBackItem.Position = zero;
			foreach (MenuTextItem menuTextItem in this.mItems)
			{
				menuTextItem.Scale = this.mScale;
			}
		}

		// Token: 0x06002399 RID: 9113 RVA: 0x00100384 File Offset: 0x000FE584
		public void Show(string iDefault, Action<string, string> iCallback)
		{
			base.Show();
			for (int i = 0; i < this.mItems.Count; i++)
			{
				if (this.mItems[i].Name == iDefault)
				{
					this.mSelectedItem = i;
					break;
				}
			}
			int num = this.mSelectedItem - 10;
			num = ((num < 0) ? 0 : ((num >= this.mScrollBar.MaxValue) ? this.mScrollBar.MaxValue : num));
			this.mScrollBar.Value = num;
			this.Complete = iCallback;
		}

		// Token: 0x0600239A RID: 9114 RVA: 0x00100410 File Offset: 0x000FE610
		public override void OnTextInput(char iChar)
		{
		}

		// Token: 0x0600239B RID: 9115 RVA: 0x00100414 File Offset: 0x000FE614
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
					this.mSelectedItem = 0;
					this.mScrollBar.Value = 0;
					return;
				}
				this.mSelectedItem++;
				if (this.mSelectedItem >= this.mItems.Count)
				{
					this.mSelectedItem = -1;
					this.mBackItem.Selected = true;
					return;
				}
				while (this.mSelectedItem > this.mScrollBar.Value + 21)
				{
					this.mScrollBar.Value++;
				}
				return;
			}
			else
			{
				if (this.mBackItem.Selected)
				{
					this.mBackItem.Selected = false;
					this.mSelectedItem = this.mItems.Count - 1;
					this.mScrollBar.Value = this.mScrollBar.MaxValue;
					return;
				}
				this.mSelectedItem--;
				if (this.mSelectedItem < 0)
				{
					this.mSelectedItem = -1;
					this.mBackItem.Selected = true;
					return;
				}
				while (this.mSelectedItem < this.mScrollBar.Value)
				{
					this.mScrollBar.Value--;
				}
				return;
			}
		}

		// Token: 0x0600239C RID: 9116 RVA: 0x00100550 File Offset: 0x000FE750
		public override void OnSelect(Controller iSender)
		{
			if (this.Complete != null && (this.mSelectedItem >= 0 & this.mSelectedItem < this.mItems.Count))
			{
				string name = this.mItems[this.mSelectedItem].Name;
				string text = this.mCountries[name];
				this.Complete.Invoke(name, text);
			}
			this.Kill();
		}

		// Token: 0x0600239D RID: 9117 RVA: 0x001005C0 File Offset: 0x000FE7C0
		public override void OnMouseMove(MouseState iNewState, MouseState iOldState)
		{
			this.mBackItem.Selected = false;
			this.mSelectedItem = -1;
			if (this.mScrollBar.Grabbed)
			{
				this.mScrollBar.ScrollTo((float)iNewState.Y);
			}
			else
			{
				for (int i = this.mScrollBar.Value; i < Math.Min(this.mScrollBar.Value + 21, this.mItems.Count); i++)
				{
					MenuItem menuItem = this.mItems[i];
					if (menuItem.Enabled & menuItem.InsideBounds(iNewState))
					{
						this.mSelectedItem = i;
						break;
					}
				}
				if (this.mSelectedItem == -1 && this.mBackItem.Enabled && this.mBackItem.InsideBounds((float)iNewState.X, (float)iNewState.Y))
				{
					this.mBackItem.Selected = true;
				}
			}
			if (iNewState.LeftButton == ButtonState.Released)
			{
				this.mScrollBar.Grabbed = false;
			}
		}

		// Token: 0x0600239E RID: 9118 RVA: 0x001006B4 File Offset: 0x000FE8B4
		public override void OnMouseClick(MouseState iNewState, MouseState iOldState)
		{
			if (iNewState.LeftButton == ButtonState.Released && iOldState.LeftButton == ButtonState.Pressed && this.mBackItem.Enabled && this.mBackItem.InsideBounds(iNewState))
			{
				this.Kill();
				return;
			}
			if (iNewState.ScrollWheelValue > iOldState.ScrollWheelValue)
			{
				this.mScrollBar.Value--;
				return;
			}
			if (iNewState.ScrollWheelValue < iOldState.ScrollWheelValue)
			{
				this.mScrollBar.Value++;
				return;
			}
			if (this.mScrollBar.InsideBounds(iNewState))
			{
				if (this.mScrollBar.InsideDragBounds(iNewState))
				{
					this.mScrollBar.Grabbed = true;
					return;
				}
				if (iNewState.LeftButton == ButtonState.Released && iOldState.LeftButton == ButtonState.Pressed)
				{
					this.mScrollBar.ScrollTo((float)iNewState.Y);
					return;
				}
			}
			else
			{
				for (int i = this.mScrollBar.Value; i < Math.Min(this.mScrollBar.Value + 21, this.mItems.Count); i++)
				{
					MenuItem menuItem = this.mItems[i];
					if (menuItem.Enabled & menuItem.InsideBounds(iNewState))
					{
						this.OnSelect(ControlManager.Instance.MenuController);
						return;
					}
				}
			}
		}

		// Token: 0x0600239F RID: 9119 RVA: 0x001007F0 File Offset: 0x000FE9F0
		public override void Draw(float iDeltaTime)
		{
			if (DaisyWheel.IsDisplaying)
			{
				return;
			}
			MessageBox.sGUIBasicEffect.SetScreenSize(RenderManager.Instance.ScreenSize.X, RenderManager.Instance.ScreenSize.Y);
			if (this.mDead)
			{
				this.mAlpha = Math.Max(this.mAlpha - iDeltaTime * 4f, 0f);
			}
			else
			{
				this.mAlpha = Math.Min(this.mAlpha + iDeltaTime * 4f, 1f);
			}
			Matrix transform = default(Matrix);
			transform.M11 = 1.75f * this.mScale;
			transform.M22 = 1.75f * this.mScale;
			transform.M33 = 1f;
			transform.M44 = 1f;
			transform.M41 = this.mCenter.X;
			transform.M42 = this.mCenter.Y;
			MessageBox.sGUIBasicEffect.Transform = transform;
			Vector4 color = default(Vector4);
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
			float num = (float)this.mFont.LineHeight * this.mScale;
			Vector2 position = new Vector2(this.mCenter.X, this.mCenter.Y - num * 21f * 0.5f);
			position.Y += num * 0.5f;
			for (int i = this.mScrollBar.Value; i < Math.Min(this.mScrollBar.Value + 21, this.mItems.Count); i++)
			{
				MenuItem menuItem = this.mItems[i];
				menuItem.Selected = (this.mSelectedItem == i);
				Vector4 messagebox_COLOR_DEFAULT = Defines.MESSAGEBOX_COLOR_DEFAULT;
				messagebox_COLOR_DEFAULT.W = this.mAlpha;
				menuItem.Color = messagebox_COLOR_DEFAULT;
				menuItem.Position = position;
				menuItem.Draw(MessageBox.sGUIBasicEffect, this.mScale * 0.9f);
				position.Y += num;
			}
			if (this.mScrollBar.MaxValue > 0)
			{
				this.mScrollBar.Color = new Vector4(1f, 1f, 1f, this.mAlpha);
				this.mScrollBar.Draw(MessageBox.sGUIBasicEffect);
			}
			this.mBackItem.Alpha = this.mAlpha;
			this.mBackItem.Draw(MessageBox.sGUIBasicEffect, this.mScale);
			MessageBox.sGUIBasicEffect.CurrentTechnique.Passes[0].End();
			MessageBox.sGUIBasicEffect.End();
		}

		// Token: 0x040026A3 RID: 9891
		private const float MARGIN_SIDE = 75f;

		// Token: 0x040026A4 RID: 9892
		private const float MARGIN_BOTTOM = 70f;

		// Token: 0x040026A5 RID: 9893
		private const int VISIBLE_ITEMS = 21;

		// Token: 0x040026A6 RID: 9894
		private const float DEFAULT_SCALE_BACKGROUND = 1.75f;

		// Token: 0x040026A7 RID: 9895
		private const float DEFAULT_SCALE_SCROLLBAR = 0.65f;

		// Token: 0x040026A8 RID: 9896
		private const float DELTA_MULTIPLIER = 4f;

		// Token: 0x040026A9 RID: 9897
		private static readonly int LOC_COUNTRYCODE = "#menu_opt_06".GetHashCodeCustom();

		// Token: 0x040026AA RID: 9898
		private static CountryCodeMessageBox sSingleton;

		// Token: 0x040026AB RID: 9899
		private static volatile object sSingletonLock = new object();

		// Token: 0x040026AD RID: 9901
		private MenuScrollBar mScrollBar;

		// Token: 0x040026AE RID: 9902
		private List<MenuTextItem> mItems;

		// Token: 0x040026AF RID: 9903
		private MenuTextItem mBackItem;

		// Token: 0x040026B0 RID: 9904
		private int mSelectedItem;

		// Token: 0x040026B1 RID: 9905
		private Dictionary<string, string> mCountries;

		// Token: 0x040026B2 RID: 9906
		private float mScale = 1f;
	}
}
