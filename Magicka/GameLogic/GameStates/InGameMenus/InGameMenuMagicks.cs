using System;
using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka.GameLogic.GameStates.InGameMenus
{
	// Token: 0x020002F2 RID: 754
	internal class InGameMenuMagicks : InGameMenu
	{
		// Token: 0x170005E6 RID: 1510
		// (get) Token: 0x06001736 RID: 5942 RVA: 0x000958D4 File Offset: 0x00093AD4
		public static InGameMenuMagicks Instance
		{
			get
			{
				if (InGameMenuMagicks.sSingelton == null)
				{
					lock (InGameMenuMagicks.sSingeltonLock)
					{
						if (InGameMenuMagicks.sSingelton == null)
						{
							InGameMenuMagicks.sSingelton = new InGameMenuMagicks();
						}
					}
				}
				return InGameMenuMagicks.sSingelton;
			}
		}

		// Token: 0x06001737 RID: 5943 RVA: 0x00095928 File Offset: 0x00093B28
		private InGameMenuMagicks()
		{
			this.mBackgroundSize = new Vector2(1000f, 550f);
			this.mTexture = Game.Instance.Content.Load<Texture2D>("UI/Menu/Magicks");
			this.mTexture2 = Game.Instance.Content.Load<Texture2D>("UI/Menu/Magicks_2");
			this.mIconTexture = Game.Instance.Content.Load<Texture2D>("UI/HUD/HUD");
			Vector2 vector = default(Vector2);
			vector.X = 1f / (float)this.mTexture.Width;
			vector.Y = 1f / (float)this.mTexture.Height;
			Vector4[] array = new Vector4[8];
			array[0].X = -200f;
			array[0].Y = -125f;
			array[0].Z = 0f;
			array[0].W = 0f;
			array[1].X = 200f;
			array[1].Y = -125f;
			array[1].Z = 400f * vector.X;
			array[1].W = 0f;
			array[2].X = 200f;
			array[2].Y = 125f;
			array[2].Z = 400f * vector.X;
			array[2].W = 250f * vector.Y;
			array[3].X = -200f;
			array[3].Y = 125f;
			array[3].Z = 0f;
			array[3].W = 250f * vector.Y;
			vector.X = 1f / (float)this.mIconTexture.Width;
			vector.Y = 1f / (float)this.mIconTexture.Height;
			array[4].X = -25f;
			array[4].Y = -25f;
			array[4].Z = 0f * vector.X;
			array[4].W = 156f * vector.Y;
			array[5].X = 25f;
			array[5].Y = -25f;
			array[5].Z = 50f * vector.X;
			array[5].W = 156f * vector.Y;
			array[6].X = 25f;
			array[6].Y = 25f;
			array[6].Z = 50f * vector.X;
			array[6].W = 206f * vector.Y;
			array[7].X = -25f;
			array[7].Y = 25f;
			array[7].Z = 0f * vector.X;
			array[7].W = 206f * vector.Y;
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			lock (graphicsDevice)
			{
				this.mVertices = new VertexBuffer(graphicsDevice, 128, BufferUsage.WriteOnly);
				this.mVertices.SetData<Vector4>(array);
				this.mVertexDeclaration = new VertexDeclaration(graphicsDevice, new VertexElement[]
				{
					new VertexElement(0, 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, 0),
					new VertexElement(0, 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0)
				});
			}
			this.mFont = FontManager.Instance.GetFont(MagickaFont.Maiandra18);
			int num = 34;
			for (int i = 0; i < num; i++)
			{
				this.AddMenuTextItem("PLACEHOLDEROFDOOMLOLOLROFLCOPTER", this.mFont, TextAlign.Center);
			}
			this.AddMenuTextItem("#menu_back".GetHashCodeCustom(), this.mFont, TextAlign.Center);
			this.mDescriptions = new string[num];
			this.mDescriptionsHash = new int[num];
			this.mScrollBar = new MenuScrollBar(default(Vector2), (float)(this.mFont.LineHeight * 12), this.mMenuItems.Count - 12 - 1);
			this.mScrollBar.TextureOffset = new Vector2(-384f, 224f);
			this.mDescription = new Text(1024, this.mFont, TextAlign.Center, false);
		}

		// Token: 0x06001738 RID: 5944 RVA: 0x00095DF0 File Offset: 0x00093FF0
		public override void LanguageChanged()
		{
			base.LanguageChanged();
			for (int i = 0; i < this.mDescriptions.Length; i++)
			{
				this.mDescriptions[i] = this.mFont.Wrap(LanguageManager.Instance.GetString(this.mDescriptionsHash[i]), 500, true);
			}
			this.mDescription.SetText(this.mDescriptions[this.mMarkedItem]);
		}

		// Token: 0x06001739 RID: 5945 RVA: 0x00095E5C File Offset: 0x0009405C
		public override void UpdatePositions()
		{
			for (int i = 0; i < this.mMenuItems.Count; i++)
			{
				this.mMenuItems[i].Scale = InGameMenu.sScale;
			}
			this.mScrollBar.Position = new Vector2(InGameMenu.sScreenSize.X * 0.5f - 100f * InGameMenu.sScale, InGameMenu.sScreenSize.Y * 0.5f);
			this.mScrollBar.Scale = InGameMenu.sScale;
		}

		// Token: 0x0600173A RID: 5946 RVA: 0x00095EE4 File Offset: 0x000940E4
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
				while (i < this.mMenuItems.Count - 1)
				{
					if (this.mMenuItems[i].InsideBounds(ref iMousePos))
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

		// Token: 0x0600173B RID: 5947 RVA: 0x00095F8C File Offset: 0x0009418C
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
				for (int i = this.mScrollBar.Value; i < this.mScrollBar.Value + 12; i++)
				{
					if (this.mMenuItems[i].Enabled && this.mMenuItems[i].InsideBounds(ref iMousePosition))
					{
						num = i;
						break;
					}
				}
				if (num == -1 && this.mMenuItems[this.mMenuItems.Count - 1].InsideBounds(ref iMousePosition))
				{
					num = this.mMenuItems.Count - 1;
				}
				if (this.mSelectedItem != num & num >= 0)
				{
					AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_MOVE);
				}
				this.mSelectedItem = num;
			}
		}

		// Token: 0x0600173C RID: 5948 RVA: 0x000960B0 File Offset: 0x000942B0
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

		// Token: 0x0600173D RID: 5949 RVA: 0x0009612C File Offset: 0x0009432C
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

		// Token: 0x0600173E RID: 5950 RVA: 0x000961A8 File Offset: 0x000943A8
		protected override void IControllerMove(Controller iSender, ControllerDirection iDirection)
		{
			if (iDirection == ControllerDirection.Left)
			{
				if (this.mSelectedItem == this.mMenuItems.Count - 1)
				{
					this.mSelectedItem = 0;
				}
				else
				{
					this.mSelectedItem = this.mMenuItems.Count - 1;
				}
			}
			else if (iDirection == ControllerDirection.Right)
			{
				if (this.mSelectedItem == this.mMenuItems.Count - 1)
				{
					this.mSelectedItem = 0;
				}
				else
				{
					this.mSelectedItem = this.mMenuItems.Count - 1;
				}
			}
			else
			{
				base.IControllerMove(iSender, iDirection);
			}
			while (this.mSelectedItem >= this.mScrollBar.Value + 12)
			{
				if (this.mSelectedItem == this.mMenuItems.Count - 1)
				{
					break;
				}
				this.mScrollBar.Value++;
			}
			while (this.mSelectedItem < this.mScrollBar.Value)
			{
				this.mScrollBar.Value--;
			}
			if (this.mSelectedItem >= 0 && this.mSelectedItem < this.mMenuItems.Count - 1)
			{
				AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
				this.mDescription.SetText(this.mDescriptions[this.mSelectedItem]);
				this.mMarkedItem = this.mSelectedItem;
				return;
			}
			this.mMarkedItem = -1;
		}

		// Token: 0x0600173F RID: 5951 RVA: 0x000962F0 File Offset: 0x000944F0
		protected override void IControllerSelect(Controller iSender)
		{
			if (this.mSelectedItem == this.mMenuItems.Count - 1)
			{
				AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_DECREASE);
				InGameMenu.PopMenu();
				return;
			}
			if (this.mSelectedItem >= 0)
			{
				AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
				if (this.mSelectedItem >= 0 && this.mSelectedItem < this.mMenuItems.Count - 1)
				{
					this.mDescription.SetText(this.mDescriptions[this.mSelectedItem]);
				}
				this.mMarkedItem = this.mSelectedItem;
			}
		}

		// Token: 0x06001740 RID: 5952 RVA: 0x00096385 File Offset: 0x00094585
		protected override void IControllerBack(Controller iSender)
		{
			AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_DECREASE);
			InGameMenu.PopMenu();
		}

		// Token: 0x06001741 RID: 5953 RVA: 0x0009639D File Offset: 0x0009459D
		protected override string IGetHighlightedButtonName()
		{
			if (this.mSelectedItem != this.mMenuItems.Count - 1)
			{
				return ((MagickType)this.mSelectedItem).ToString();
			}
			return "Back";
		}

		// Token: 0x06001742 RID: 5954 RVA: 0x000963CC File Offset: 0x000945CC
		protected override void OnEnter()
		{
			this.mScrollBar.Value = 0;
			this.mSelectedItem = -1;
			this.mMarkedItem = -1;
			LanguageManager instance = LanguageManager.Instance;
			Player player = InGameMenu.sController.Player;
			if (player == null && InGameMenu.sPlayState.GameType != GameType.Versus)
			{
				for (int i = 0; i < Game.Instance.Players.Length; i++)
				{
					if (Game.Instance.Players[i].Playing)
					{
						player = Game.Instance.Players[i];
						i = Game.Instance.Players.Length;
					}
				}
			}
			int num = 34;
			for (int j = 0; j < num; j++)
			{
				MagickType magickType = j + MagickType.Revive;
				if (magickType >= (MagickType)Magick.DESC_LOCALIZATION.Length)
				{
					Console.WriteLine("Magick description out of range! Fix DESC_LOCALIZATION.");
				}
				else
				{
					this.mDescriptionsHash[j] = Magick.DESC_LOCALIZATION[(int)magickType];
					this.mDescriptions[j] = this.mFont.Wrap(instance.GetString(this.mDescriptionsHash[j]), 500, true);
				}
				if (SpellManager.Instance.IsMagickAllowed(player, InGameMenu.sPlayState.GameType, magickType) && player != null)
				{
					this.mMenuItems[j].Enabled = true;
					(this.mMenuItems[j] as MenuTextItem).SetText(Magick.NAME_LOCALIZATION[(int)magickType]);
				}
				else
				{
					this.mMenuItems[j].Enabled = false;
					(this.mMenuItems[j] as MenuTextItem).SetText("???");
				}
			}
		}

		// Token: 0x06001743 RID: 5955 RVA: 0x00096547 File Offset: 0x00094747
		protected override void IUpdate(DataChannel iDataChannel, float iDeltaTime)
		{
			base.IUpdate(iDataChannel, iDeltaTime);
		}

		// Token: 0x06001744 RID: 5956 RVA: 0x00096554 File Offset: 0x00094754
		protected override void IDraw(float iDeltaTime, ref Vector2 iBackgroundSize)
		{
			Matrix transform = default(Matrix);
			transform.M44 = 1f;
			Vector4 color = default(Vector4);
			color.X = (color.Y = (color.Z = 1f));
			Vector4 color2 = default(Vector4);
			color2.X = (color2.Y = (color2.Z = 1f));
			color2.W = this.mAlpha;
			Vector4 colorSelected = default(Vector4);
			colorSelected.X = (colorSelected.Y = (colorSelected.Z = 0f));
			colorSelected.W = this.mAlpha;
			Vector4 colorDisabled = default(Vector4);
			colorDisabled.X = (colorDisabled.Y = (colorDisabled.Z = 0.4f));
			colorDisabled.W = this.mAlpha;
			float num = (float)this.mFont.LineHeight;
			Vector2 position = default(Vector2);
			position.X = InGameMenu.sScreenSize.X * 0.5f - 250f * InGameMenu.sScale;
			position.Y = InGameMenu.sScreenSize.Y * 0.5f - num * 11f * 0.5f * InGameMenu.sScale;
			for (int i = this.mScrollBar.Value; i < this.mScrollBar.Value + 12; i++)
			{
				MenuItem menuItem = this.mMenuItems[i];
				menuItem.Position = position;
				menuItem.Color = color2;
				menuItem.ColorSelected = colorSelected;
				menuItem.ColorDisabled = colorDisabled;
				menuItem.Selected = (menuItem.Enabled & (this.mSelectedItem == i | this.mMarkedItem == i));
				if (menuItem.Selected)
				{
					transform.M11 = 300f * InGameMenu.sScale;
					transform.M22 = menuItem.BottomRight.Y - menuItem.TopLeft.Y;
					transform.M41 = menuItem.Position.X - 150f * InGameMenu.sScale;
					transform.M42 = menuItem.TopLeft.Y;
					InGameMenu.sEffect.Transform = transform;
					if (this.mSelectedItem == i)
					{
						color.W = 0.8f * this.mAlpha;
					}
					else
					{
						color.W = 0.5f * this.mAlpha;
					}
					InGameMenu.sEffect.Color = color;
					InGameMenu.sEffect.GraphicsDevice.Vertices[0].SetSource(InGameMenu.sBackground, 0, 8);
					InGameMenu.sEffect.GraphicsDevice.VertexDeclaration = InGameMenu.sBackgroundDeclaration;
					InGameMenu.sEffect.CommitChanges();
					InGameMenu.sEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
				}
				position.Y += num * InGameMenu.sScale;
			}
			this.mMenuItems[this.mMenuItems.Count - 1].Position = position;
			this.mMenuItems[this.mMenuItems.Count - 1].Color = color2;
			this.mMenuItems[this.mMenuItems.Count - 1].ColorSelected = colorSelected;
			this.mMenuItems[this.mMenuItems.Count - 1].ColorDisabled = colorDisabled;
			this.mMenuItems[this.mMenuItems.Count - 1].Selected = (this.mSelectedItem == this.mMenuItems.Count - 1);
			if (this.mMenuItems[this.mMenuItems.Count - 1].Selected)
			{
				transform.M11 = 100f * InGameMenu.sScale;
				transform.M22 = this.mMenuItems[this.mMenuItems.Count - 1].BottomRight.Y - this.mMenuItems[this.mMenuItems.Count - 1].TopLeft.Y;
				transform.M41 = this.mMenuItems[this.mMenuItems.Count - 1].Position.X - 50f * InGameMenu.sScale;
				transform.M42 = this.mMenuItems[this.mMenuItems.Count - 1].TopLeft.Y;
				InGameMenu.sEffect.Transform = transform;
				if (this.mSelectedItem == this.mMenuItems.Count - 1)
				{
					color.W = 0.8f * this.mAlpha;
				}
				else
				{
					color.W = 0.5f * this.mAlpha;
				}
				InGameMenu.sEffect.Color = color;
				InGameMenu.sEffect.GraphicsDevice.Vertices[0].SetSource(InGameMenu.sBackground, 0, 8);
				InGameMenu.sEffect.GraphicsDevice.VertexDeclaration = InGameMenu.sBackgroundDeclaration;
				InGameMenu.sEffect.CommitChanges();
				InGameMenu.sEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
			}
			for (int j = this.mScrollBar.Value; j < this.mScrollBar.Value + 12; j++)
			{
				this.mMenuItems[j].Draw(InGameMenu.sEffect);
			}
			this.mMenuItems[this.mMenuItems.Count - 1].Draw(InGameMenu.sEffect);
			color.W = this.mAlpha;
			this.mScrollBar.Color = color;
			this.mScrollBar.TextureOffset = new Vector2(-384f, 224f);
			this.mScrollBar.Draw(InGameMenu.sEffect);
			transform.M11 = InGameMenu.sScale;
			transform.M22 = InGameMenu.sScale;
			transform.M41 = InGameMenu.sScreenSize.X * 0.5f + 180f * InGameMenu.sScale;
			transform.M42 = 270f * InGameMenu.sScale;
			int num2 = this.mMarkedItem;
			MagickType magickType = num2 + MagickType.Revive;
			InGameMenu.sEffect.Transform = transform;
			InGameMenu.sEffect.Color = color;
			if (magickType >= MagickType.Confuse)
			{
				InGameMenu.sEffect.TextureScale = new Vector2(1f, 4f);
				InGameMenu.sEffect.Texture = this.mTexture2;
			}
			else
			{
				InGameMenu.sEffect.Texture = this.mTexture;
			}
			if (!(num2 < 0 | num2 == this.mMenuItems.Count - 1))
			{
				try
				{
					InGameMenu.sEffect.TextureOffset = InGameMenuMagicks.MAGICK_TEXTURE_LOOKUP[num2];
				}
				catch (Exception)
				{
					InGameMenu.sEffect.TextureOffset = InGameMenuMagicks.MAGICK_TEXTURE_LOOKUP[0];
				}
				InGameMenu.sEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertices, 0, 16);
				InGameMenu.sEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
				InGameMenu.sEffect.CommitChanges();
				InGameMenu.sEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
			}
			InGameMenu.sEffect.TextureScale = Vector2.One;
			if (this.mMarkedItem >= 0)
			{
				Elements[] magickCombo = SpellManager.Instance.GetMagickCombo(magickType);
				if (magickCombo != null)
				{
					Vector2 textureOffset = default(Vector2);
					InGameMenu.sEffect.Texture = this.mIconTexture;
					transform.M11 = InGameMenu.sScale * 0.75f;
					transform.M22 = InGameMenu.sScale * 0.75f;
					transform.M42 = 380f * InGameMenu.sScale;
					for (int k = 0; k < magickCombo.Length; k++)
					{
						int num3 = MagickaMath.CountTrailingZeroBits((uint)magickCombo[k]);
						transform.M41 = InGameMenu.sScreenSize.X * 0.5f + (180f - (float)magickCombo.Length * 0.5f * 38f + ((float)k + 0.5f) * 38f) * InGameMenu.sScale;
						InGameMenu.sEffect.Transform = transform;
						textureOffset.X = (float)(num3 % 5) * 50f / (float)this.mIconTexture.Width;
						textureOffset.Y = (float)(num3 / 5) * 50f / (float)this.mIconTexture.Height;
						InGameMenu.sEffect.TextureOffset = textureOffset;
						InGameMenu.sEffect.CommitChanges();
						InGameMenu.sEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 4, 2);
					}
				}
				this.mDescription.Draw(InGameMenu.sEffect, InGameMenu.sScreenSize.X * 0.5f + 180f * InGameMenu.sScale, 270f * InGameMenu.sScale + 125f * InGameMenu.sScale, InGameMenu.sScale);
			}
			InGameMenu.sEffect.TextureOffset = default(Vector2);
		}

		// Token: 0x06001745 RID: 5957 RVA: 0x00096E4C File Offset: 0x0009504C
		protected override void OnExit()
		{
		}

		// Token: 0x040018B8 RID: 6328
		private const string OPTION_BACK = "Back";

		// Token: 0x040018B9 RID: 6329
		private const int NONE_INDEX = 28;

		// Token: 0x040018BA RID: 6330
		private const int VISIBLE_ITEMS = 12;

		// Token: 0x040018BB RID: 6331
		private static InGameMenuMagicks sSingelton;

		// Token: 0x040018BC RID: 6332
		private static volatile object sSingeltonLock = new object();

		// Token: 0x040018BD RID: 6333
		private static readonly Vector2[] MAGICK_TEXTURE_LOOKUP = new Vector2[]
		{
			new Vector2(0f, 0f),
			new Vector2(0.1953125f, 0f),
			new Vector2(0.390625f, 0f),
			new Vector2(0.5859375f, 0f),
			new Vector2(0.78125f, 0f),
			new Vector2(0f, 0.125f),
			new Vector2(0.1953125f, 0.12207031f),
			new Vector2(0.390625f, 0.12207031f),
			new Vector2(0.5859375f, 0.12207031f),
			new Vector2(0.78125f, 0.12207031f),
			new Vector2(0f, 0.25f),
			new Vector2(0.1953125f, 0.24414062f),
			new Vector2(0.390625f, 0.24414062f),
			new Vector2(0.5859375f, 0.24414062f),
			new Vector2(0.78125f, 0.24414062f),
			new Vector2(0f, 0.375f),
			new Vector2(0.1953125f, 0.36621094f),
			new Vector2(0.390625f, 0.36621094f),
			new Vector2(0.5859375f, 0.36621094f),
			new Vector2(0.78125f, 0.36621094f),
			new Vector2(0f, 0.48828125f),
			new Vector2(0.1953125f, 0.48828125f),
			new Vector2(0f, 0.7324219f),
			new Vector2(0.1953125f, 0.7324219f),
			new Vector2(0.78125f, 0.7324219f),
			new Vector2(0.1953125f, 0.8544922f),
			new Vector2(0f, 0.8544922f),
			new Vector2(0.390625f, 0.7324219f),
			new Vector2(0.5859375f, 0.7324219f),
			new Vector2(0.1953125f, 0f),
			new Vector2(0.390625f, 0f),
			new Vector2(0.78125f, 0f),
			new Vector2(0.5859375f, 0f)
		};

		// Token: 0x040018BE RID: 6334
		private VertexBuffer mVertices;

		// Token: 0x040018BF RID: 6335
		private VertexDeclaration mVertexDeclaration;

		// Token: 0x040018C0 RID: 6336
		private Texture2D mTexture;

		// Token: 0x040018C1 RID: 6337
		private Texture2D mTexture2;

		// Token: 0x040018C2 RID: 6338
		private Texture2D mIconTexture;

		// Token: 0x040018C3 RID: 6339
		private string[] mDescriptions;

		// Token: 0x040018C4 RID: 6340
		private int[] mDescriptionsHash;

		// Token: 0x040018C5 RID: 6341
		private MenuScrollBar mScrollBar;

		// Token: 0x040018C6 RID: 6342
		private BitmapFont mFont;

		// Token: 0x040018C7 RID: 6343
		private int mMarkedItem;

		// Token: 0x040018C8 RID: 6344
		private Text mDescription;
	}
}
