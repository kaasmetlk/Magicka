using System;
using System.Collections.Generic;
using System.Linq;
using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka.GameLogic.GameStates.InGameMenus
{
	// Token: 0x020003AD RID: 941
	internal class InGameMenuOptionsResolution : InGameMenu
	{
		// Token: 0x17000711 RID: 1809
		// (get) Token: 0x06001CDE RID: 7390 RVA: 0x000CCA2C File Offset: 0x000CAC2C
		public static InGameMenuOptionsResolution Instance
		{
			get
			{
				if (InGameMenuOptionsResolution.sSingelton == null)
				{
					lock (InGameMenuOptionsResolution.sSingeltonLock)
					{
						if (InGameMenuOptionsResolution.sSingelton == null)
						{
							InGameMenuOptionsResolution.sSingelton = new InGameMenuOptionsResolution();
						}
					}
				}
				return InGameMenuOptionsResolution.sSingelton;
			}
		}

		// Token: 0x06001CDF RID: 7391 RVA: 0x000CCA80 File Offset: 0x000CAC80
		private InGameMenuOptionsResolution()
		{
			this.mFont = FontManager.Instance.GetFont(MagickaFont.Maiandra16);
			this.mResolutions = new SortedList<uint, KeyValuePair<MenuTextItem, List<int>>>();
			this.mScrollBar = new MenuScrollBar(default(Vector2), (float)(this.mFont.LineHeight * 12), 0);
			this.mScrollBar.TextureOffset = new Vector2(-384f, 224f);
			this.mBackgroundSize = new Vector2(400f, 500f);
			Vector2 iPosition = new Vector2(InGameMenu.sScreenSize.X * 0.5f, InGameMenu.sScreenSize.Y * 0.5f + this.mBackgroundSize.Y * 0.5f - 64f);
			this.mMenuItems.Add(new MenuTextItem("#menu_back".GetHashCodeCustom(), iPosition, this.mFont, TextAlign.Center));
		}

		// Token: 0x06001CE0 RID: 7392 RVA: 0x000CCB64 File Offset: 0x000CAD64
		public override void LanguageChanged()
		{
			base.LanguageChanged();
			if (this.mResolutions != null && this.mResolutions.Count > 0)
			{
				this.mResolutions.Last<KeyValuePair<uint, KeyValuePair<MenuTextItem, List<int>>>>().Value.Key.LanguageChanged();
			}
		}

		// Token: 0x06001CE1 RID: 7393 RVA: 0x000CCBB0 File Offset: 0x000CADB0
		public override void UpdatePositions()
		{
			for (int i = 0; i < this.mResolutions.Count; i++)
			{
				this.mResolutions.Values[i].Key.Scale = InGameMenu.sScale;
			}
			this.mMenuItems[0].Position = new Vector2(InGameMenu.sScreenSize.X * 0.5f, InGameMenu.sScreenSize.Y * 0.5f + (this.mBackgroundSize.Y * 0.5f - 64f) * InGameMenu.sScale);
			this.mMenuItems[0].Scale = InGameMenu.sScale;
			this.mScrollBar.Position = new Vector2(InGameMenu.sScreenSize.X * 0.5f + 120f * InGameMenu.sScale, InGameMenu.sScreenSize.Y * 0.5f);
			this.mScrollBar.Scale = InGameMenu.sScale;
		}

		// Token: 0x06001CE2 RID: 7394 RVA: 0x000CCCAC File Offset: 0x000CAEAC
		protected override string IGetHighlightedButtonName()
		{
			if (this.mSelectedItem != this.mResolutions.Count)
			{
				return this.mResolutions.Values[this.mSelectedItem].Key.Name;
			}
			return "back";
		}

		// Token: 0x06001CE3 RID: 7395 RVA: 0x000CCCF8 File Offset: 0x000CAEF8
		protected override void IControllerSelect(Controller iSender)
		{
			if (this.mSelectedItem == this.mResolutions.Count)
			{
				AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_DECREASE);
				InGameMenu.PopMenu();
				return;
			}
			if (this.mSelectedItem >= 0)
			{
				AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
				ResolutionData resolutionData = default(ResolutionData);
				uint num = this.mResolutions.Keys[this.mSelectedItem];
				resolutionData.Width = (int)(65535U & num >> 16);
				resolutionData.Height = (int)(65535U & num);
				GlobalSettings.Instance.Resolution = resolutionData;
				ResolutionMessageBox.Instance.NotifyResolutionChanged(resolutionData);
				InGameMenu.PopMenu();
			}
		}

		// Token: 0x06001CE4 RID: 7396 RVA: 0x000CCDA2 File Offset: 0x000CAFA2
		protected override void IControllerBack(Controller iSender)
		{
			AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_DECREASE);
			InGameMenu.PopMenu();
		}

		// Token: 0x06001CE5 RID: 7397 RVA: 0x000CCDBC File Offset: 0x000CAFBC
		protected override void IControllerMove(Controller iSender, ControllerDirection iDirection)
		{
			switch (iDirection)
			{
			case ControllerDirection.Right:
				if (this.mSelectedItem < this.mResolutions.Count)
				{
					this.mSelectedItem = this.mResolutions.Count;
				}
				else
				{
					this.mSelectedItem = 0;
				}
				break;
			case ControllerDirection.Up:
			{
				int num = this.mSelectedItem;
				do
				{
					num--;
					if (num < 0)
					{
						num += this.mResolutions.Count;
					}
				}
				while (!this.mResolutions.Values[num].Key.Enabled);
				this.mSelectedItem = num;
				AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_MOVE);
				break;
			}
			case ControllerDirection.UpRight:
				break;
			case ControllerDirection.Left:
				if (this.mSelectedItem < this.mResolutions.Count)
				{
					this.mSelectedItem = this.mResolutions.Count;
				}
				else
				{
					this.mSelectedItem = 0;
				}
				break;
			default:
				if (iDirection == ControllerDirection.Down)
				{
					int num2 = this.mSelectedItem;
					do
					{
						num2++;
						if (num2 >= this.mResolutions.Count)
						{
							num2 -= this.mResolutions.Count;
						}
					}
					while (!this.mResolutions.Values[num2].Key.Enabled);
					this.mSelectedItem = num2;
					AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_MOVE);
				}
				break;
			}
			while (this.mSelectedItem < this.mScrollBar.Value)
			{
				this.mScrollBar.Value--;
			}
			while (this.mSelectedItem >= this.mScrollBar.Value + 12 && this.mSelectedItem < this.mResolutions.Count)
			{
				this.mScrollBar.Value++;
			}
		}

		// Token: 0x06001CE6 RID: 7398 RVA: 0x000CCF74 File Offset: 0x000CB174
		protected override void IMouseMove(Controller iSender, ref Vector2 iMousePosition)
		{
			if (this.mScrollBar.Grabbed)
			{
				this.mSelectedItem = -1;
				if (this.mScrollBar.InsideDragUpBounds(iMousePosition))
				{
					this.mScrollBar.Value--;
					return;
				}
				if (this.mScrollBar.InsideDragDownBounds(iMousePosition))
				{
					this.mScrollBar.Value++;
					return;
				}
			}
			else
			{
				int num = -1;
				for (int i = 0; i < this.mResolutions.Count; i++)
				{
					MenuTextItem key = this.mResolutions.Values[i].Key;
					if (key.Enabled && key.InsideBounds(ref iMousePosition))
					{
						num = i;
						break;
					}
				}
				if (num == -1 && this.mMenuItems[0].InsideBounds(ref iMousePosition))
				{
					num = this.mResolutions.Count;
				}
				if (this.mSelectedItem != num & num >= 0)
				{
					AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_MOVE);
				}
				this.mSelectedItem = num;
			}
		}

		// Token: 0x06001CE7 RID: 7399 RVA: 0x000CD080 File Offset: 0x000CB280
		protected override void IMouseScroll(Controller iSender, ref Vector2 iMousePos, int iValue)
		{
			if (this.mScrollBar.InsideBounds(ref iMousePos))
			{
				if (iValue > 0)
				{
					this.mScrollBar.Value--;
					return;
				}
				if (iValue < 0)
				{
					this.mScrollBar.Value++;
					return;
				}
			}
			else
			{
				int i = 0;
				while (i < this.mResolutions.Count)
				{
					MenuTextItem key = this.mResolutions.Values[i].Key;
					if (key.Enabled && key.InsideBounds(ref iMousePos))
					{
						if (iValue > 0)
						{
							this.mScrollBar.Value--;
							return;
						}
						if (iValue < 0)
						{
							this.mScrollBar.Value++;
							return;
						}
						break;
					}
					else
					{
						i++;
					}
				}
			}
		}

		// Token: 0x06001CE8 RID: 7400 RVA: 0x000CD140 File Offset: 0x000CB340
		protected override void IMouseDown(Controller iSender, ref Vector2 iMousePosition)
		{
			base.IMouseDown(iSender, ref iMousePosition);
			if (this.mScrollBar.InsideDragBounds(iMousePosition))
			{
				this.mScrollBar.Grabbed = true;
				return;
			}
			if (!this.mScrollBar.InsideUpBounds(iMousePosition) && !this.mScrollBar.InsideDownBounds(iMousePosition) && this.mScrollBar.InsideBounds(ref iMousePosition))
			{
				this.mScrollBar.ScrollTo(iMousePosition.Y);
			}
		}

		// Token: 0x06001CE9 RID: 7401 RVA: 0x000CD1BC File Offset: 0x000CB3BC
		protected override void IMouseUp(Controller iSender, ref Vector2 iMousePosition)
		{
			base.IMouseUp(iSender, ref iMousePosition);
			if (!this.mScrollBar.Grabbed)
			{
				if (this.mScrollBar.InsideUpBounds(iMousePosition))
				{
					this.mScrollBar.Value--;
				}
				else if (this.mScrollBar.InsideDownBounds(iMousePosition))
				{
					this.mScrollBar.Value++;
				}
			}
			this.mScrollBar.Grabbed = false;
		}

		// Token: 0x06001CEA RID: 7402 RVA: 0x000CD238 File Offset: 0x000CB438
		protected override void OnEnter()
		{
			if (InGameMenu.sController is KeyboardMouseController)
			{
				this.mSelectedItem = -1;
			}
			else
			{
				this.mSelectedItem = 0;
			}
			this.mScrollBar.Value = 0;
			this.mResolutions.Clear();
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
			this.mScrollBar.SetMaxValue(this.mResolutions.Count - 12);
			this.UpdatePositions();
		}

		// Token: 0x06001CEB RID: 7403 RVA: 0x000CD3D0 File Offset: 0x000CB5D0
		protected override void IDraw(float iDeltaTime, ref Vector2 iBackgroundSize)
		{
			Vector4 color = default(Vector4);
			color.X = (color.Y = (color.Z = 1f));
			color.W = this.mAlpha;
			Vector4 colorSelected = default(Vector4);
			colorSelected.X = (colorSelected.Y = (colorSelected.Z = 0f));
			colorSelected.W = this.mAlpha;
			Vector4 colorDisabled = default(Vector4);
			colorDisabled.X = (colorDisabled.Y = (colorDisabled.Z = 0.4f));
			colorDisabled.W = this.mAlpha;
			float num = (float)this.mFont.LineHeight;
			Vector2 position = default(Vector2);
			position.X = InGameMenu.sScreenSize.X * 0.5f;
			position.Y = InGameMenu.sScreenSize.Y * 0.5f - num * 11f * 0.5f * InGameMenu.sScale;
			for (int i = this.mScrollBar.Value; i < Math.Min(this.mScrollBar.Value + 12, this.mResolutions.Count); i++)
			{
				MenuItem key = this.mResolutions.Values[i].Key;
				key.Position = position;
				key.Color = color;
				key.ColorSelected = colorSelected;
				key.ColorDisabled = colorDisabled;
				key.Selected = (key.Enabled & this.mSelectedItem == i);
				if (key.Selected)
				{
					Matrix transform = default(Matrix);
					transform.M44 = 1f;
					transform.M11 = iBackgroundSize.X * InGameMenu.sScale;
					transform.M22 = key.BottomRight.Y - key.TopLeft.Y;
					transform.M41 = key.Position.X - iBackgroundSize.X * 0.5f * InGameMenu.sScale;
					transform.M42 = key.TopLeft.Y;
					InGameMenu.sEffect.Transform = transform;
					Vector4 color2 = default(Vector4);
					color2.X = (color2.Y = (color2.Z = 1f));
					color2.W = 0.8f * this.mAlpha;
					InGameMenu.sEffect.Color = color2;
					InGameMenu.sEffect.GraphicsDevice.Vertices[0].SetSource(InGameMenu.sBackground, 0, 8);
					InGameMenu.sEffect.GraphicsDevice.VertexDeclaration = InGameMenu.sBackgroundDeclaration;
					InGameMenu.sEffect.CommitChanges();
					InGameMenu.sEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
				}
				position.Y += num * InGameMenu.sScale;
			}
			this.mMenuItems[0].Selected = (this.mSelectedItem == this.mResolutions.Count);
			this.mMenuItems[0].Color = color;
			this.mMenuItems[0].ColorSelected = colorSelected;
			if (this.mMenuItems[0].Selected)
			{
				Matrix transform2 = default(Matrix);
				transform2.M44 = 1f;
				transform2.M11 = iBackgroundSize.X * InGameMenu.sScale;
				transform2.M22 = this.mMenuItems[0].BottomRight.Y - this.mMenuItems[0].TopLeft.Y;
				transform2.M41 = this.mMenuItems[0].Position.X - iBackgroundSize.X * 0.5f * InGameMenu.sScale;
				transform2.M42 = this.mMenuItems[0].TopLeft.Y;
				InGameMenu.sEffect.Transform = transform2;
				Vector4 color3 = default(Vector4);
				color3.X = (color3.Y = (color3.Z = 1f));
				color3.W = 0.8f * this.mAlpha;
				InGameMenu.sEffect.Color = color3;
				InGameMenu.sEffect.GraphicsDevice.Vertices[0].SetSource(InGameMenu.sBackground, 0, 8);
				InGameMenu.sEffect.GraphicsDevice.VertexDeclaration = InGameMenu.sBackgroundDeclaration;
				InGameMenu.sEffect.CommitChanges();
				InGameMenu.sEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
			}
			this.mScrollBar.Color = color;
			this.mScrollBar.Draw(InGameMenu.sEffect);
			for (int j = this.mScrollBar.Value; j < Math.Min(this.mScrollBar.Value + 12, this.mResolutions.Count); j++)
			{
				this.mResolutions.Values[j].Key.Draw(InGameMenu.sEffect);
			}
			this.mMenuItems[0].Draw(InGameMenu.sEffect);
		}

		// Token: 0x06001CEC RID: 7404 RVA: 0x000CD8F8 File Offset: 0x000CBAF8
		protected override void OnExit()
		{
			Point screenSize = RenderManager.Instance.ScreenSize;
			InGameMenu.sPlayState.Camera.AspectRation = (float)screenSize.X / (float)screenSize.Y;
			TutorialManager.Instance.UpdateResolution();
			if (BossFight.Instance.IsSetup && !BossFight.Instance.Dead)
			{
				BossFight.Instance.UpdateResolution();
			}
			DamageNotifyer.Instance.UpdateResolution();
			InGameMenu.UpdateAllPositions();
			SaveManager.Instance.SaveSettings();
		}

		// Token: 0x04001F88 RID: 8072
		private const string OPTION_BACK = "back";

		// Token: 0x04001F89 RID: 8073
		private const int VISIBLE_ITEMS = 12;

		// Token: 0x04001F8A RID: 8074
		private static InGameMenuOptionsResolution sSingelton;

		// Token: 0x04001F8B RID: 8075
		private static volatile object sSingeltonLock = new object();

		// Token: 0x04001F8C RID: 8076
		private MenuScrollBar mScrollBar;

		// Token: 0x04001F8D RID: 8077
		private BitmapFont mFont;

		// Token: 0x04001F8E RID: 8078
		private SortedList<uint, KeyValuePair<MenuTextItem, List<int>>> mResolutions;
	}
}
