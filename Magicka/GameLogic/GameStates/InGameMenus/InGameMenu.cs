using System;
using System.Collections.Generic;
using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.Localization;
using Magicka.WebTools.Paradox.Telemetry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.GameStates.InGameMenus
{
	// Token: 0x02000032 RID: 50
	internal abstract class InGameMenu : IRenderableGUIObject
	{
		// Token: 0x060001BF RID: 447 RVA: 0x0000CA78 File Offset: 0x0000AC78
		public virtual void LanguageChanged()
		{
			if (this.mMenuItems == null)
			{
				return;
			}
			for (int i = 0; i < this.mMenuItems.Count; i++)
			{
				this.mMenuItems[i].LanguageChanged();
			}
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x0000CAB5 File Offset: 0x0000ACB5
		protected static int GetOnOffLoc(bool iOnOff)
		{
			if (iOnOff)
			{
				return "#menu_opt_alt_01".GetHashCodeCustom();
			}
			return "#menu_opt_alt_02".GetHashCodeCustom();
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x0000CAD0 File Offset: 0x0000ACD0
		protected static int GetSettingLoc(SettingOptions iSetting)
		{
			switch (iSetting)
			{
			case SettingOptions.Off:
				return InGameMenu.LOC_OFF;
			case SettingOptions.On:
				return InGameMenu.LOC_ON;
			case SettingOptions.Players_Only:
				return InGameMenu.LOC_PLAYERS_ONLY;
			case SettingOptions.Low:
				return InGameMenu.LOC_LOW;
			case SettingOptions.Medium:
				return InGameMenu.LOC_MEDIUM;
			case SettingOptions.High:
				return InGameMenu.LOC_HIGH;
			default:
				return 0;
			}
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x0000CB24 File Offset: 0x0000AD24
		static InGameMenu()
		{
			InGameMenu.sMenuStack = new Stack<InGameMenu>();
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x0000CBC4 File Offset: 0x0000ADC4
		public InGameMenu()
		{
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			Point point;
			point.X = GlobalSettings.Instance.Resolution.Width;
			point.Y = GlobalSettings.Instance.Resolution.Height;
			InGameMenu.sScreenSize.X = (float)point.X;
			InGameMenu.sScreenSize.Y = (float)point.Y;
			if (InGameMenu.sBackgroundTexture == null || InGameMenu.sBackgroundTexture.IsDisposed)
			{
				lock (graphicsDevice)
				{
					InGameMenu.sBackgroundTexture = Game.Instance.Content.Load<Texture2D>("UI/Menu/FadeBox");
				}
			}
			if (InGameMenu.sEffect == null || InGameMenu.sEffect.IsDisposed)
			{
				lock (graphicsDevice)
				{
					InGameMenu.sEffect = new GUIBasicEffect(graphicsDevice, null);
				}
			}
			if (InGameMenu.sBackground == null || InGameMenu.sBackground.IsDisposed)
			{
				Vector2[] array = new Vector2[4];
				array[0].X = 0f;
				array[0].Y = 0f;
				array[1].X = 1f;
				array[1].Y = 0f;
				array[2].X = 1f;
				array[2].Y = 1f;
				array[3].X = 0f;
				array[3].Y = 1f;
				lock (graphicsDevice)
				{
					InGameMenu.sBackground = new VertexBuffer(graphicsDevice, 32, BufferUsage.WriteOnly);
					InGameMenu.sBackground.SetData<Vector2>(array);
				}
			}
			if (InGameMenu.sBackgroundDeclaration == null || InGameMenu.sBackgroundDeclaration.IsDisposed)
			{
				lock (graphicsDevice)
				{
					InGameMenu.sBackgroundDeclaration = new VertexDeclaration(graphicsDevice, new VertexElement[]
					{
						new VertexElement(0, 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, 0),
						new VertexElement(0, 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0)
					});
				}
			}
			InGameMenu.sEffect.SetScreenSize(point.X, point.Y);
			InGameMenu.sScale = InGameMenu.sScreenSize.Y / 720f;
			LanguageManager.Instance.LanguageChanged += new Action(this.LanguageChanged);
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x0000CE60 File Offset: 0x0000B060
		protected void DrawGraphics(Texture2D iTexture, Rectangle iScrRect, Vector4 iDestRect)
		{
			this.DrawGraphics(iTexture, iScrRect, iDestRect, Vector4.One);
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x0000CE70 File Offset: 0x0000B070
		protected void DrawGraphics(Texture2D iTexture, Rectangle iScrRect, Vector4 iDestRect, Vector4 iColor)
		{
			Vector2 textureOffset = default(Vector2);
			Vector2 textureScale = default(Vector2);
			if (iTexture != null)
			{
				textureOffset.X = (float)iScrRect.X / (float)iTexture.Width;
				textureOffset.Y = (float)iScrRect.Y / (float)iTexture.Height;
				textureScale.X = (float)iScrRect.Width / (float)iTexture.Width;
				textureScale.Y = (float)iScrRect.Height / (float)iTexture.Height;
				InGameMenu.sEffect.TextureOffset = textureOffset;
				InGameMenu.sEffect.TextureScale = textureScale;
			}
			Matrix transform = default(Matrix);
			transform.M11 = iDestRect.Z;
			transform.M22 = iDestRect.W;
			transform.M41 = iDestRect.X;
			transform.M42 = iDestRect.Y;
			transform.M44 = 1f;
			InGameMenu.sEffect.Transform = transform;
			InGameMenu.sEffect.Texture = iTexture;
			InGameMenu.sEffect.TextureEnabled = (iTexture != null);
			InGameMenu.sEffect.Color = iColor;
			InGameMenu.sEffect.GraphicsDevice.Vertices[0].SetSource(InGameMenu.sBackground, 0, 8);
			InGameMenu.sEffect.GraphicsDevice.VertexDeclaration = InGameMenu.sBackgroundDeclaration;
			InGameMenu.sEffect.CommitChanges();
			InGameMenu.sEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x0000CFD4 File Offset: 0x0000B1D4
		public virtual void UpdatePositions()
		{
			Vector2 position = default(Vector2);
			position.X = InGameMenu.sScreenSize.X * 0.5f;
			position.Y = 290f * InGameMenu.sScale;
			for (int i = 0; i < this.mMenuItems.Count; i++)
			{
				MenuItem menuItem = this.mMenuItems[i];
				menuItem.Scale = InGameMenu.sScale;
				menuItem.Position = position;
				position.Y += menuItem.BottomRight.Y - menuItem.TopLeft.Y;
			}
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x0000D06C File Offset: 0x0000B26C
		protected virtual void AddMenuTextItem(int iText, BitmapFont iFont, TextAlign iTextAlign)
		{
			MenuTextItem menuTextItem = new MenuTextItem(iText, default(Vector2), iFont, iTextAlign);
			menuTextItem.Scale = InGameMenu.sScale;
			menuTextItem.ColorDisabled = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
			menuTextItem.Color = new Vector4(1f, 1f, 1f, 1f);
			menuTextItem.ColorSelected = new Vector4(10f, 10f, 10f, 1f);
			this.mMenuItems.Add(menuTextItem);
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x0000D100 File Offset: 0x0000B300
		protected virtual void AddMenuTextItem(string iText, BitmapFont iFont, TextAlign iTextAlign)
		{
			MenuTextItem menuTextItem = new MenuTextItem(iText, default(Vector2), iFont, iTextAlign);
			menuTextItem.Scale = InGameMenu.sScale;
			menuTextItem.ColorDisabled = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
			menuTextItem.Color = new Vector4(1f, 1f, 1f, 1f);
			menuTextItem.ColorSelected = new Vector4(10f, 10f, 10f, 1f);
			this.mMenuItems.Add(menuTextItem);
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x0000D194 File Offset: 0x0000B394
		protected virtual void IMouseMove(Controller iSender, ref Vector2 iMousePosition)
		{
			int num = -1;
			for (int i = 0; i < this.mMenuItems.Count; i++)
			{
				if (this.mMenuItems[i].Enabled && this.mMenuItems[i].InsideBounds(ref iMousePosition))
				{
					num = i;
					break;
				}
			}
			if (this.mSelectedItem != num & num >= 0)
			{
				AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_MOVE);
			}
			this.mSelectedItem = num;
		}

		// Token: 0x060001CA RID: 458 RVA: 0x0000D212 File Offset: 0x0000B412
		protected virtual void IMouseScroll(Controller iSender, ref Vector2 iMousePos, int iValue)
		{
		}

		// Token: 0x060001CB RID: 459 RVA: 0x0000D214 File Offset: 0x0000B414
		protected virtual void IMouseDown(Controller iSender, ref Vector2 iMousePosition)
		{
			this.mDownedSelection = this.mSelectedItem;
		}

		// Token: 0x060001CC RID: 460 RVA: 0x0000D222 File Offset: 0x0000B422
		protected virtual void IMouseUp(Controller iSender, ref Vector2 iMousePosition)
		{
			if (this.mDownedSelection == this.mSelectedItem)
			{
				InGameMenu.ControllerSelect(iSender);
			}
		}

		// Token: 0x060001CD RID: 461 RVA: 0x0000D238 File Offset: 0x0000B438
		protected virtual void IControllerMove(Controller iSender, ControllerDirection iDirection)
		{
			switch (iDirection)
			{
			case ControllerDirection.Right:
			case ControllerDirection.UpRight:
			case ControllerDirection.Left:
				break;
			case ControllerDirection.Up:
			{
				int num = this.mSelectedItem;
				do
				{
					num--;
					if (num < 0)
					{
						num += this.mMenuItems.Count;
					}
				}
				while (!this.mMenuItems[num].Enabled);
				this.mSelectedItem = num;
				AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_MOVE);
				return;
			}
			default:
			{
				if (iDirection != ControllerDirection.Down)
				{
					return;
				}
				int num2 = this.mSelectedItem;
				do
				{
					num2++;
					if (num2 >= this.mMenuItems.Count)
					{
						num2 -= this.mMenuItems.Count;
					}
				}
				while (!this.mMenuItems[num2].Enabled);
				this.mSelectedItem = num2;
				AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_MOVE);
				break;
			}
			}
		}

		// Token: 0x060001CE RID: 462 RVA: 0x0000D300 File Offset: 0x0000B500
		protected void SendButtonPressTelemetry()
		{
			if (InGameMenu.sMenuStack.Peek().mSelectedItem >= 0)
			{
				TelemetryUtils.SendInGameMenuButtonPressTelemetry(InGameMenu.sPlayState.GameType.ToString(), InGameMenu.sPlayState.Level.Name.Substring(InGameMenu.sPlayState.Level.Name.IndexOf('#') + 1), InGameMenu.sMenuStack.Peek().IGetHighlightedButtonName());
			}
		}

		// Token: 0x060001CF RID: 463
		protected abstract string IGetHighlightedButtonName();

		// Token: 0x060001D0 RID: 464
		protected abstract void IControllerSelect(Controller iSender);

		// Token: 0x060001D1 RID: 465
		protected abstract void IControllerBack(Controller iSender);

		// Token: 0x060001D2 RID: 466
		protected abstract void OnEnter();

		// Token: 0x060001D3 RID: 467 RVA: 0x0000D373 File Offset: 0x0000B573
		protected virtual void IUpdate(DataChannel iDataChannel, float iDeltaTime)
		{
			InGameMenu.sPlayState.Scene.AddRenderableGUIObject(iDataChannel, this);
		}

		// Token: 0x060001D4 RID: 468
		protected abstract void OnExit();

		// Token: 0x060001D5 RID: 469 RVA: 0x0000D388 File Offset: 0x0000B588
		public virtual void Draw(float iDeltaTime)
		{
			if (InGameMenu.sBackgroundVisible)
			{
				InGameMenu.sBackgroundAlpha = Math.Min(InGameMenu.sBackgroundAlpha + iDeltaTime * 5f, 1f);
			}
			else
			{
				InGameMenu.sBackgroundAlpha = Math.Max(InGameMenu.sBackgroundAlpha - iDeltaTime * 5f, 0f);
			}
			if (this.mVisible)
			{
				this.mAlpha = Math.Min(this.mAlpha + iDeltaTime * 5f, 1f);
			}
			else
			{
				this.mAlpha = Math.Max(this.mAlpha - iDeltaTime * 5f, 0f);
			}
			if (this.mAlpha >= 1f)
			{
				InGameMenu.sLastBackgroundSize = this.mBackgroundSize;
			}
			Vector2 vector;
			Vector2.SmoothStep(ref InGameMenu.sLastBackgroundSize, ref this.mBackgroundSize, this.mAlpha, out vector);
			Vector4 color = default(Vector4);
			color.W = 0.5f * InGameMenu.sBackgroundAlpha;
			Matrix transform = default(Matrix);
			transform.M11 = InGameMenu.sScreenSize.X;
			transform.M22 = InGameMenu.sScreenSize.Y;
			transform.M44 = 1f;
			InGameMenu.sEffect.Color = color;
			InGameMenu.sEffect.Transform = transform;
			InGameMenu.sEffect.TextureEnabled = false;
			InGameMenu.sEffect.VertexColorEnabled = false;
			InGameMenu.sEffect.Texture = InGameMenu.sBackgroundTexture;
			InGameMenu.sEffect.GraphicsDevice.Vertices[0].SetSource(InGameMenu.sBackground, 0, 8);
			InGameMenu.sEffect.GraphicsDevice.VertexDeclaration = InGameMenu.sBackgroundDeclaration;
			InGameMenu.sEffect.Begin();
			InGameMenu.sEffect.CurrentTechnique.Passes[0].Begin();
			InGameMenu.sEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
			transform.M11 = vector.X * InGameMenu.sScale;
			transform.M22 = vector.Y * InGameMenu.sScale;
			transform.M41 = (InGameMenu.sScreenSize.X - vector.X * InGameMenu.sScale) * 0.5f;
			transform.M42 = (720f - vector.Y) * 0.5f * InGameMenu.sScale;
			color.W = 0.666f * InGameMenu.sBackgroundAlpha;
			InGameMenu.sEffect.Color = color;
			InGameMenu.sEffect.Transform = transform;
			InGameMenu.sEffect.TextureEnabled = true;
			InGameMenu.sEffect.CommitChanges();
			InGameMenu.sEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
			this.IDraw(iDeltaTime, ref vector);
			InGameMenu.sEffect.CurrentTechnique.Passes[0].End();
			InGameMenu.sEffect.End();
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x0000D62C File Offset: 0x0000B82C
		protected virtual void IDraw(float iDeltaTime, ref Vector2 iBackgroundSize)
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
			for (int i = 0; i < this.mMenuItems.Count; i++)
			{
				MenuItem menuItem = this.mMenuItems[i];
				menuItem.Color = color;
				menuItem.ColorSelected = colorSelected;
				menuItem.ColorDisabled = colorDisabled;
				menuItem.Selected = (menuItem.Enabled & this.mSelectedItem == i);
				if (menuItem.Selected)
				{
					Matrix transform = default(Matrix);
					transform.M44 = 1f;
					transform.M11 = iBackgroundSize.X * InGameMenu.sScale;
					transform.M22 = menuItem.BottomRight.Y - menuItem.TopLeft.Y;
					transform.M41 = menuItem.Position.X - iBackgroundSize.X * 0.5f * InGameMenu.sScale;
					transform.M42 = menuItem.TopLeft.Y;
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
			}
			for (int j = 0; j < this.mMenuItems.Count; j++)
			{
				if (this.mMenuItems[j].Enabled)
				{
					this.mMenuItems[j].Draw(InGameMenu.sEffect);
				}
			}
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060001D7 RID: 471 RVA: 0x0000D8B7 File Offset: 0x0000BAB7
		public int ZIndex
		{
			get
			{
				return 1000;
			}
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060001D8 RID: 472 RVA: 0x0000D8BE File Offset: 0x0000BABE
		public static InGameMenu CurrentMenu
		{
			get
			{
				if (InGameMenu.sMenuStack.Count == 0)
				{
					return null;
				}
				return InGameMenu.sMenuStack.Peek();
			}
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x0000D8D8 File Offset: 0x0000BAD8
		public static void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			InGameMenu inGameMenu = InGameMenu.sMenuStack.Peek();
			inGameMenu.IUpdate(iDataChannel, iDeltaTime);
			if (!inGameMenu.mVisible && inGameMenu.mAlpha < 1E-45f)
			{
				lock (Game.Instance.GraphicsDevice)
				{
					inGameMenu.OnExit();
					if (InGameMenu.sNextMenu == null)
					{
						if (InGameMenu.sMenuStack.Count > 1)
						{
							InGameMenu.sMenuStack.Pop();
						}
					}
					else
					{
						InGameMenu.sMenuStack.Push(InGameMenu.sNextMenu);
						InGameMenu.sNextMenu = null;
					}
					InGameMenu.sMenuStack.Peek().OnEnter();
					InGameMenu.sMenuStack.Peek().mVisible = true;
					InGameMenu.sMenuStack.Peek().mAlpha = 0f;
				}
			}
		}

		// Token: 0x060001DA RID: 474 RVA: 0x0000D9AC File Offset: 0x0000BBAC
		public static void PushMenu(InGameMenu iNewMenu)
		{
			InGameMenu.sNextMenu = iNewMenu;
			InGameMenu.sMenuStack.Peek().mVisible = false;
		}

		// Token: 0x060001DB RID: 475 RVA: 0x0000D9C4 File Offset: 0x0000BBC4
		public static void PopMenu()
		{
			InGameMenu.sMenuStack.Peek().mVisible = false;
		}

		// Token: 0x060001DC RID: 476 RVA: 0x0000D9D8 File Offset: 0x0000BBD8
		public static void ControllerSelect(Controller iSender)
		{
			if (InGameMenu.sBackgroundVisible && InGameMenu.IsControllerAllowed(iSender) && InGameMenu.sMenuStack.Peek().mVisible)
			{
				InGameMenu.sMenuStack.Peek().SendButtonPressTelemetry();
				InGameMenu.sMenuStack.Peek().IControllerSelect(iSender);
			}
		}

		// Token: 0x060001DD RID: 477 RVA: 0x0000DA24 File Offset: 0x0000BC24
		public static void ControllerBack(Controller iSender)
		{
			if (InGameMenu.sBackgroundVisible && InGameMenu.IsControllerAllowed(iSender) && InGameMenu.sMenuStack.Peek().mVisible)
			{
				InGameMenu.sMenuStack.Peek().IControllerBack(iSender);
			}
		}

		// Token: 0x060001DE RID: 478 RVA: 0x0000DA56 File Offset: 0x0000BC56
		public static void MouseScroll(Controller iSender, ref Vector2 iMousePos, int iValue)
		{
			if (InGameMenu.sBackgroundVisible && InGameMenu.IsControllerAllowed(iSender) && InGameMenu.sMenuStack.Peek().mVisible)
			{
				InGameMenu.sMenuStack.Peek().IMouseScroll(iSender, ref iMousePos, iValue);
			}
		}

		// Token: 0x060001DF RID: 479 RVA: 0x0000DA8A File Offset: 0x0000BC8A
		public static void MouseMove(Controller iSender, ref Vector2 iMousePos)
		{
			if (InGameMenu.sBackgroundVisible && InGameMenu.IsControllerAllowed(iSender) && InGameMenu.sMenuStack.Peek().mVisible)
			{
				InGameMenu.sMenuStack.Peek().IMouseMove(iSender, ref iMousePos);
			}
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x0000DABD File Offset: 0x0000BCBD
		public static void MouseDown(Controller iSender, ref Vector2 iMousePos)
		{
			if (InGameMenu.sBackgroundVisible && InGameMenu.IsControllerAllowed(iSender) && InGameMenu.sMenuStack.Peek().mVisible)
			{
				InGameMenu.sMenuStack.Peek().IMouseDown(iSender, ref iMousePos);
			}
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x0000DAF0 File Offset: 0x0000BCF0
		public static void MouseUp(Controller iSender, ref Vector2 iMousePos)
		{
			if (InGameMenu.sBackgroundVisible && InGameMenu.IsControllerAllowed(iSender) && InGameMenu.sMenuStack.Peek().mVisible)
			{
				InGameMenu.sMenuStack.Peek().IMouseUp(iSender, ref iMousePos);
			}
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x0000DB23 File Offset: 0x0000BD23
		public static void ControllerMove(Controller iSender, ControllerDirection iDirection)
		{
			if (InGameMenu.sBackgroundVisible && InGameMenu.IsControllerAllowed(iSender) && InGameMenu.sMenuStack.Peek().mVisible)
			{
				InGameMenu.sMenuStack.Peek().IControllerMove(iSender, iDirection);
			}
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x0000DB56 File Offset: 0x0000BD56
		public static void Initialize(PlayState iPlayState)
		{
			InGameMenu.sPlayState = iPlayState;
			InGameMenu.sBackgroundVisible = false;
			InGameMenu.sMenuStack.Clear();
			InGameMenu.sMenuStack.Push(InGameMenuMain.Instance);
			InGameMenu.sBackgroundAlpha = 0f;
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x0000DB88 File Offset: 0x0000BD88
		public static void Show(Controller iController)
		{
			InGameMenu.sController = iController;
			if (!InGameMenu.sBackgroundVisible)
			{
				Point screenSize = RenderManager.Instance.ScreenSize;
				InGameMenu.sEffect.SetScreenSize(screenSize.X, screenSize.Y);
				InGameMenu.sScreenSize.X = (float)screenSize.X;
				InGameMenu.sScreenSize.Y = (float)screenSize.Y;
				InGameMenu.sScale = (float)screenSize.Y / 720f;
				InGameMenu.UpdateAllPositions();
				while (InGameMenu.sMenuStack.Count > 1)
				{
					InGameMenu.sMenuStack.Pop();
				}
				InGameMenu.sLastBackgroundSize = InGameMenu.sMenuStack.Peek().mBackgroundSize;
				InGameMenu.sMenuStack.Peek().mAlpha = 0f;
				InGameMenu.sMenuStack.Peek().mVisible = true;
				InGameMenu.sMenuStack.Peek().OnEnter();
				InGameMenu.sBackgroundVisible = true;
			}
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x0000DC6B File Offset: 0x0000BE6B
		public static void Hide()
		{
			InGameMenu.sBackgroundVisible = false;
			InGameMenu.sMenuStack.Peek().mVisible = false;
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x0000DC83 File Offset: 0x0000BE83
		public static void HideInstant()
		{
			InGameMenu.sBackgroundVisible = false;
			InGameMenu.sMenuStack.Peek().mVisible = false;
			InGameMenu.sBackgroundAlpha = 0f;
		}

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060001E7 RID: 487 RVA: 0x0000DCA5 File Offset: 0x0000BEA5
		public static bool Visible
		{
			get
			{
				return InGameMenu.sBackgroundVisible | InGameMenu.sBackgroundAlpha > 0f;
			}
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x0000DCB9 File Offset: 0x0000BEB9
		public static bool IsControllerAllowed(Controller iController)
		{
			return InGameMenu.sController == null || InGameMenu.sController == iController;
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x0000DCCC File Offset: 0x0000BECC
		protected static void UpdateAllPositions()
		{
			Point point;
			point.X = GlobalSettings.Instance.Resolution.Width;
			point.Y = GlobalSettings.Instance.Resolution.Height;
			InGameMenu.sEffect.SetScreenSize(point.X, point.Y);
			InGameMenu.sScreenSize.X = (float)point.X;
			InGameMenu.sScreenSize.Y = (float)point.Y;
			InGameMenu.sScale = (float)point.Y / 720f;
			InGameMenuMain.Instance.UpdatePositions();
			InGameMenuMagicks.Instance.UpdatePositions();
			InGameMenuOptions.Instance.UpdatePositions();
			InGameMenuOptionsGame.Instance.UpdatePositions();
			InGameMenuOptionsSound.Instance.UpdatePositions();
			InGameMenuOptionsGraphics.Instance.UpdatePositions();
			InGameMenuOptionsResolution.Instance.UpdatePositions();
		}

		// Token: 0x0400018F RID: 399
		protected static VertexBuffer sBackground;

		// Token: 0x04000190 RID: 400
		protected static VertexDeclaration sBackgroundDeclaration;

		// Token: 0x04000191 RID: 401
		protected static GUIBasicEffect sEffect;

		// Token: 0x04000192 RID: 402
		protected static Texture2D sBackgroundTexture;

		// Token: 0x04000193 RID: 403
		protected static float sBackgroundAlpha;

		// Token: 0x04000194 RID: 404
		protected static bool sBackgroundVisible;

		// Token: 0x04000195 RID: 405
		protected static Vector2 sLastBackgroundSize;

		// Token: 0x04000196 RID: 406
		protected static PlayState sPlayState;

		// Token: 0x04000197 RID: 407
		protected static Controller sController;

		// Token: 0x04000198 RID: 408
		protected static Vector2 sScreenSize;

		// Token: 0x04000199 RID: 409
		protected static float sScale;

		// Token: 0x0400019A RID: 410
		protected static Stack<InGameMenu> sMenuStack;

		// Token: 0x0400019B RID: 411
		private static InGameMenu sNextMenu;

		// Token: 0x0400019C RID: 412
		public static readonly int SOUND_MOVE = "ui_menu_scroll".GetHashCodeCustom();

		// Token: 0x0400019D RID: 413
		public static readonly int SOUND_INCREASE = "ui_menu_increase".GetHashCodeCustom();

		// Token: 0x0400019E RID: 414
		public static readonly int SOUND_DECREASE = "ui_menu_decrease".GetHashCodeCustom();

		// Token: 0x0400019F RID: 415
		internal static readonly int LOC_ON = "#menu_opt_alt_01".GetHashCodeCustom();

		// Token: 0x040001A0 RID: 416
		internal static readonly int LOC_OFF = "#menu_opt_alt_02".GetHashCodeCustom();

		// Token: 0x040001A1 RID: 417
		internal static readonly int LOC_PLAYERS_ONLY = "#menu_opt_alt_03".GetHashCodeCustom();

		// Token: 0x040001A2 RID: 418
		internal static readonly int LOC_LOW = "#menu_opt_alt_04".GetHashCodeCustom();

		// Token: 0x040001A3 RID: 419
		internal static readonly int LOC_MEDIUM = "#menu_opt_alt_05".GetHashCodeCustom();

		// Token: 0x040001A4 RID: 420
		internal static readonly int LOC_HIGH = "#menu_opt_alt_06".GetHashCodeCustom();

		// Token: 0x040001A5 RID: 421
		protected List<MenuItem> mMenuItems = new List<MenuItem>();

		// Token: 0x040001A6 RID: 422
		protected Vector2 mBackgroundSize;

		// Token: 0x040001A7 RID: 423
		protected float mAlpha;

		// Token: 0x040001A8 RID: 424
		protected bool mVisible;

		// Token: 0x040001A9 RID: 425
		protected int mSelectedItem;

		// Token: 0x040001AA RID: 426
		protected int mDownedSelection;
	}
}
