using System;
using System.Collections.Generic;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;

namespace Magicka.GameLogic.UI
{
	// Token: 0x020002E9 RID: 745
	internal class ResolutionMessageBox : MessageBox
	{
		// Token: 0x170005C8 RID: 1480
		// (get) Token: 0x060016D9 RID: 5849 RVA: 0x00092D5C File Offset: 0x00090F5C
		public static ResolutionMessageBox Instance
		{
			get
			{
				if (ResolutionMessageBox.sSingelton == null)
				{
					lock (ResolutionMessageBox.sSingeltonLock)
					{
						if (ResolutionMessageBox.sSingelton == null)
						{
							ResolutionMessageBox.sSingelton = new ResolutionMessageBox();
						}
					}
				}
				return ResolutionMessageBox.sSingelton;
			}
		}

		// Token: 0x1400000F RID: 15
		// (add) Token: 0x060016DA RID: 5850 RVA: 0x00092DB0 File Offset: 0x00090FB0
		// (remove) Token: 0x060016DB RID: 5851 RVA: 0x00092DC9 File Offset: 0x00090FC9
		public event Action<ResolutionData> Complete;

		// Token: 0x060016DC RID: 5852 RVA: 0x00092DE4 File Offset: 0x00090FE4
		private ResolutionMessageBox() : base(ResolutionMessageBox.LOC_RESOLUTIONS)
		{
			this.mResolutions = new SortedList<uint, KeyValuePair<MenuTextItem, List<int>>>();
			float num = (float)this.mFont.LineHeight;
			foreach (DisplayMode displayMode in Game.Instance.GraphicsDevice.CreationParameters.Adapter.SupportedDisplayModes)
			{
				if (displayMode.Width >= 800 && displayMode.Height >= 600)
				{
					uint key = (uint)((ulong)-65536 & (ulong)((long)((long)((ushort)displayMode.Width) << 16))) | (uint)(ushort.MaxValue & (ushort)displayMode.Height);
					KeyValuePair<MenuTextItem, List<int>> value;
					if (!this.mResolutions.TryGetValue(key, out value))
					{
						string iTitle = string.Format("{0} x {1}", displayMode.Width, displayMode.Height);
						value = new KeyValuePair<MenuTextItem, List<int>>(new MenuTextItem(iTitle, default(Vector2), this.mFont, TextAlign.Center), new List<int>());
						this.mResolutions.Add(key, value);
					}
					if (!value.Value.Contains(displayMode.RefreshRate))
					{
						value.Value.Add(displayMode.RefreshRate);
					}
				}
			}
			Vector2 iPosition = new Vector2(this.mCenter.X + this.mSize.X * 0.5f - 64f, this.mCenter.Y);
			this.mResolutionScrollBar = new MenuScrollBar(iPosition, num * 12f * 1.25f, this.mResolutions.Count - 12);
			this.mResolutionScrollBar.Scale = 0.8f;
			iPosition = new Vector2(this.mCenter.X, this.mCenter.Y + this.mSize.Y * 0.5f - 48f);
			this.mBackItem = new MenuTextItem(SubMenu.LOC_CANCEL, iPosition, this.mFont, TextAlign.Center);
			this.mBackItem.ColorDisabled = Defines.MESSAGEBOX_COLOR_DEFAULT * 0.5f;
			this.mBackItem.Color = Defines.MESSAGEBOX_COLOR_DEFAULT;
			this.mBackItem.ColorSelected = Vector4.One;
		}

		// Token: 0x060016DD RID: 5853 RVA: 0x00093040 File Offset: 0x00091240
		public override void LanguageChanged()
		{
			base.LanguageChanged();
			this.mBackItem.LanguageChanged();
		}

		// Token: 0x060016DE RID: 5854 RVA: 0x00093054 File Offset: 0x00091254
		public override void Show()
		{
			base.Show();
			this.mResolutionScrollBar.Position = new Vector2(this.mCenter.X + this.mSize.X * 0.5f - 64f, this.mCenter.Y);
			this.mBackItem.Position = new Vector2(this.mCenter.X, this.mCenter.Y + this.mSize.Y * 0.5f - 48f);
		}

		// Token: 0x060016DF RID: 5855 RVA: 0x000930E3 File Offset: 0x000912E3
		public override void OnTextInput(char iChar)
		{
		}

		// Token: 0x060016E0 RID: 5856 RVA: 0x000930E8 File Offset: 0x000912E8
		public override void OnMove(Controller iSender, ControllerDirection iDirection)
		{
			switch (iDirection)
			{
			case ControllerDirection.Right:
				if (this.mBackItem.Selected)
				{
					this.mBackItem.Selected = false;
					this.mCurrentResolution = 0;
				}
				else
				{
					this.mBackItem.Selected = true;
					this.mCurrentResolution = -1;
				}
				break;
			case ControllerDirection.Up:
				if (this.mBackItem.Selected)
				{
					this.mBackItem.Selected = false;
					this.mCurrentResolution = this.mResolutions.Count - 1;
				}
				else
				{
					this.mCurrentResolution--;
					if (this.mCurrentResolution < 0)
					{
						this.mBackItem.Selected = true;
						return;
					}
				}
				break;
			case ControllerDirection.UpRight:
				break;
			case ControllerDirection.Left:
				if (this.mBackItem.Selected)
				{
					this.mBackItem.Selected = false;
					this.mCurrentResolution = 0;
				}
				else
				{
					this.mBackItem.Selected = true;
					this.mCurrentResolution = -1;
				}
				break;
			default:
				if (iDirection == ControllerDirection.Down)
				{
					if (this.mBackItem.Selected)
					{
						this.mBackItem.Selected = false;
						this.mCurrentResolution = 0;
					}
					else
					{
						this.mCurrentResolution++;
						if (this.mCurrentResolution >= this.mResolutions.Count)
						{
							this.mBackItem.Selected = true;
							return;
						}
					}
				}
				break;
			}
			if (this.mCurrentResolution >= 0)
			{
				while (this.mResolutionScrollBar.Value > this.mCurrentResolution)
				{
					this.mResolutionScrollBar.Value--;
				}
				while (this.mResolutionScrollBar.Value + 12 <= this.mCurrentResolution)
				{
					this.mResolutionScrollBar.Value++;
				}
			}
		}

		// Token: 0x060016E1 RID: 5857 RVA: 0x00093294 File Offset: 0x00091494
		public override void OnSelect(Controller iSender)
		{
			if (this.mCurrentResolution < this.mResolutions.Count && this.mCurrentResolution >= 0)
			{
				ResolutionData obj = default(ResolutionData);
				uint num = this.mResolutions.Keys[this.mCurrentResolution];
				obj.Width = (int)(65535U & num >> 16);
				obj.Height = (int)(65535U & num);
				if (this.Complete != null)
				{
					this.Complete(obj);
				}
			}
			this.Kill();
		}

		// Token: 0x060016E2 RID: 5858 RVA: 0x00093315 File Offset: 0x00091515
		public override void Kill()
		{
			base.Kill();
			SaveManager.Instance.SaveSettings();
		}

		// Token: 0x060016E3 RID: 5859 RVA: 0x00093328 File Offset: 0x00091528
		public override void OnMouseMove(MouseState iNewState, MouseState iOldState)
		{
			if (this.mBackItem.Enabled && this.mBackItem.InsideBounds((float)iNewState.X, (float)iNewState.Y))
			{
				this.mBackItem.Selected = true;
				return;
			}
			this.mBackItem.Selected = false;
			this.mCurrentResolution = -1;
			if (this.mResolutionScrollBar.Grabbed)
			{
				this.mResolutionScrollBar.ScrollTo((float)iNewState.Y);
				return;
			}
			for (int i = this.mResolutionScrollBar.Value; i < Math.Min(this.mResolutionScrollBar.Value + 12, this.mResolutions.Count); i++)
			{
				MenuItem key = this.mResolutions.Values[i].Key;
				if (key.Enabled & key.InsideBounds(iNewState))
				{
					this.mCurrentResolution = i;
					return;
				}
			}
		}

		// Token: 0x060016E4 RID: 5860 RVA: 0x00093408 File Offset: 0x00091608
		public override void OnMouseClick(MouseState iNewState, MouseState iOldState)
		{
			if (iNewState.LeftButton == ButtonState.Released)
			{
				this.mResolutionScrollBar.Grabbed = false;
			}
			if (iNewState.LeftButton == ButtonState.Released && iOldState.LeftButton == ButtonState.Pressed && this.mBackItem.Enabled && this.mBackItem.InsideBounds((float)iNewState.X, (float)iNewState.Y))
			{
				this.Kill();
				return;
			}
			if (this.mResolutionScrollBar.InsideBounds(iNewState))
			{
				if (iNewState.LeftButton == ButtonState.Pressed && this.mResolutionScrollBar.InsideDragBounds(iNewState))
				{
					this.mResolutionScrollBar.Grabbed = true;
					return;
				}
				if (iNewState.LeftButton == ButtonState.Released && iOldState.LeftButton == ButtonState.Pressed)
				{
					if (this.mResolutionScrollBar.InsideUpBounds(iNewState))
					{
						this.mResolutionScrollBar.Value--;
						return;
					}
					if (this.mResolutionScrollBar.InsideDownBounds(iNewState))
					{
						this.mResolutionScrollBar.Value++;
						return;
					}
					this.mResolutionScrollBar.ScrollTo((float)iNewState.Y);
					return;
				}
			}
			else
			{
				if (iNewState.ScrollWheelValue > iOldState.ScrollWheelValue)
				{
					this.mResolutionScrollBar.Value--;
					return;
				}
				if (iNewState.ScrollWheelValue < iOldState.ScrollWheelValue)
				{
					this.mResolutionScrollBar.Value++;
					return;
				}
				if (iNewState.LeftButton == ButtonState.Released && iOldState.LeftButton == ButtonState.Pressed)
				{
					for (int i = this.mResolutionScrollBar.Value; i < Math.Min(this.mResolutionScrollBar.Value + 12, this.mResolutions.Count); i++)
					{
						MenuItem key = this.mResolutions.Values[i].Key;
						if (key.Enabled & key.InsideBounds(iNewState))
						{
							this.OnSelect(ControlManager.Instance.MenuController);
							return;
						}
					}
				}
			}
		}

		// Token: 0x060016E5 RID: 5861 RVA: 0x000935E0 File Offset: 0x000917E0
		public override void Draw(float iDeltaTime)
		{
			base.Draw(iDeltaTime);
			float num = (float)this.mFont.LineHeight;
			Vector2 position = new Vector2(this.mCenter.X, this.mCenter.Y - num * 12f * 0.5f);
			position.Y += num * 0.5f;
			for (int i = this.mResolutionScrollBar.Value; i < Math.Min(this.mResolutionScrollBar.Value + 12, this.mResolutions.Count); i++)
			{
				MenuTextItem key = this.mResolutions.Values[i].Key;
				key.Color = new Vector4(Defines.MESSAGEBOX_COLOR_DEFAULT.X, Defines.MESSAGEBOX_COLOR_DEFAULT.Y, Defines.MESSAGEBOX_COLOR_DEFAULT.Z, this.mAlpha);
				key.Position = position;
				key.Draw(MessageBox.sGUIBasicEffect);
				position.Y += num;
				key.Selected = (this.mCurrentResolution == i);
			}
			this.mResolutionScrollBar.Color = new Vector4(1f, 1f, 1f, this.mAlpha);
			this.mResolutionScrollBar.Draw(MessageBox.sGUIBasicEffect);
			this.mBackItem.Alpha = this.mAlpha;
			this.mBackItem.Draw(MessageBox.sGUIBasicEffect);
			MessageBox.sGUIBasicEffect.CurrentTechnique.Passes[0].End();
			MessageBox.sGUIBasicEffect.End();
		}

		// Token: 0x060016E6 RID: 5862 RVA: 0x0009376B File Offset: 0x0009196B
		public void NotifyResolutionChanged(ResolutionData iData)
		{
			if (this.Complete != null)
			{
				this.Complete(iData);
			}
		}

		// Token: 0x04001846 RID: 6214
		private const float BACK_PADDING = 48f;

		// Token: 0x04001847 RID: 6215
		private const int VISIBLE_RESOLUTIONS = 12;

		// Token: 0x04001848 RID: 6216
		private static readonly int LOC_RESOLUTIONS = "#menu_opt_gfx_10".GetHashCodeCustom();

		// Token: 0x04001849 RID: 6217
		private static ResolutionMessageBox sSingelton;

		// Token: 0x0400184A RID: 6218
		private static volatile object sSingeltonLock = new object();

		// Token: 0x0400184C RID: 6220
		private SortedList<uint, KeyValuePair<MenuTextItem, List<int>>> mResolutions;

		// Token: 0x0400184D RID: 6221
		private MenuScrollBar mResolutionScrollBar;

		// Token: 0x0400184E RID: 6222
		private MenuTextItem mBackItem;

		// Token: 0x0400184F RID: 6223
		private int mCurrentResolution;
	}
}
