using System;
using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.Graphics;
using Magicka.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka.GameLogic.GameStates.InGameMenus
{
	// Token: 0x0200057A RID: 1402
	internal class InGameMenuOptionsSound : InGameMenu
	{
		// Token: 0x170009E4 RID: 2532
		// (get) Token: 0x060029F8 RID: 10744 RVA: 0x0014A014 File Offset: 0x00148214
		public static InGameMenuOptionsSound Instance
		{
			get
			{
				if (InGameMenuOptionsSound.sSingelton == null)
				{
					lock (InGameMenuOptionsSound.sSingeltonLock)
					{
						if (InGameMenuOptionsSound.sSingelton == null)
						{
							InGameMenuOptionsSound.sSingelton = new InGameMenuOptionsSound();
						}
					}
				}
				return InGameMenuOptionsSound.sSingelton;
			}
		}

		// Token: 0x060029F9 RID: 10745 RVA: 0x0014A068 File Offset: 0x00148268
		private InGameMenuOptionsSound()
		{
			this.mFont = FontManager.Instance.GetFont(MagickaFont.Maiandra18);
			this.AddMenuTextItem("#menu_opt_sfx_01".GetHashCodeCustom(), this.mFont, TextAlign.Right);
			this.AddMenuTextItem("#menu_opt_sfx_02".GetHashCodeCustom(), this.mFont, TextAlign.Right);
			this.AddMenuTextItem("#menu_back".GetHashCodeCustom(), this.mFont, TextAlign.Center);
			this.mMusicScrollSlider = new MenuScrollSlider(default(Vector2), 240f, 10);
			this.mMusicScrollSlider.TextureOffset = new Vector2(-384f, 224f);
			this.mSFXScrollSlider = new MenuScrollSlider(default(Vector2), 240f, 10);
			this.mSFXScrollSlider.TextureOffset = new Vector2(-384f, 224f);
			this.mBackgroundSize = new Vector2(400f, 200f);
			Vector2 value = default(Vector2);
			value.X = InGameMenu.sScreenSize.X * 0.5f - 60f * InGameMenu.sScale;
			value.Y = InGameMenu.sScreenSize.Y * 0.5f - 30f * InGameMenu.sScale;
			this.mMusicScrollSlider.Position = value + new Vector2(120f * InGameMenu.sScale, 0f);
			value.Y += (float)this.mFont.LineHeight;
			this.mSFXScrollSlider.Position = value + new Vector2(120f * InGameMenu.sScale, 0f);
			this.mMusicScrollSlider.Scale = 0.7f;
			this.mSFXScrollSlider.Scale = 0.7f;
		}

		// Token: 0x060029FA RID: 10746 RVA: 0x0014A224 File Offset: 0x00148424
		public override void UpdatePositions()
		{
			Vector2 vector = default(Vector2);
			vector.X = InGameMenu.sScreenSize.X * 0.5f - 60f * InGameMenu.sScale;
			vector.Y = InGameMenu.sScreenSize.Y * 0.5f - 30f * InGameMenu.sScale;
			for (int i = 0; i < 2; i++)
			{
				MenuItem menuItem = this.mMenuItems[i];
				menuItem.Scale = InGameMenu.sScale;
				menuItem.Position = vector;
				vector.Y += menuItem.BottomRight.Y - menuItem.TopLeft.Y;
			}
			vector.X = InGameMenu.sScreenSize.X * 0.5f;
			vector.Y += 10f * InGameMenu.sScale;
			this.mMenuItems[2].Scale = InGameMenu.sScale;
			this.mMenuItems[2].Position = vector;
			vector = default(Vector2);
			vector.X = InGameMenu.sScreenSize.X * 0.5f - 60f * InGameMenu.sScale;
			vector.Y = InGameMenu.sScreenSize.Y * 0.5f - 30f * InGameMenu.sScale;
			this.mMusicScrollSlider.Position = vector + new Vector2(120f * InGameMenu.sScale, 0f);
			vector.Y += (float)this.mFont.LineHeight;
			this.mSFXScrollSlider.Position = vector + new Vector2(120f * InGameMenu.sScale, 0f);
		}

		// Token: 0x060029FB RID: 10747 RVA: 0x0014A3DB File Offset: 0x001485DB
		protected override string IGetHighlightedButtonName()
		{
			return "back";
		}

		// Token: 0x060029FC RID: 10748 RVA: 0x0014A3E2 File Offset: 0x001485E2
		protected override void IControllerSelect(Controller iSender)
		{
			if (this.mSelectedItem == 2)
			{
				AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_DECREASE);
				InGameMenu.PopMenu();
			}
		}

		// Token: 0x060029FD RID: 10749 RVA: 0x0014A403 File Offset: 0x00148603
		protected override void IControllerBack(Controller iSender)
		{
			AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_DECREASE);
			InGameMenu.PopMenu();
		}

		// Token: 0x060029FE RID: 10750 RVA: 0x0014A41C File Offset: 0x0014861C
		protected override void IControllerMove(Controller iSender, ControllerDirection iDirection)
		{
			base.IControllerMove(iSender, iDirection);
			if (iDirection != ControllerDirection.Right)
			{
				if (iDirection != ControllerDirection.Left)
				{
					return;
				}
				if (this.mSelectedItem == 0 && this.mMusicScrollSlider.Value > 0)
				{
					this.mMusicScrollSlider.Value--;
					AudioManager.Instance.VolumeMusic(this.mMusicScrollSlider.Value);
					return;
				}
				if (this.mSelectedItem == 1 && this.mSFXScrollSlider.Value > 0)
				{
					this.mSFXScrollSlider.Value--;
					AudioManager.Instance.VolumeSound(this.mSFXScrollSlider.Value);
					AudioManager.Instance.PlayCue(Banks.Misc, InGameMenuOptionsSound.SOUND_THUD);
				}
			}
			else
			{
				if (this.mSelectedItem == 0 && this.mMusicScrollSlider.Value < 10)
				{
					this.mMusicScrollSlider.Value++;
					AudioManager.Instance.VolumeMusic(this.mMusicScrollSlider.Value);
					return;
				}
				if (this.mSelectedItem == 1 && this.mSFXScrollSlider.Value < 10)
				{
					this.mSFXScrollSlider.Value++;
					AudioManager.Instance.VolumeSound(this.mSFXScrollSlider.Value);
					AudioManager.Instance.PlayCue(Banks.Misc, InGameMenuOptionsSound.SOUND_THUD);
					return;
				}
			}
		}

		// Token: 0x060029FF RID: 10751 RVA: 0x0014A570 File Offset: 0x00148770
		protected override void IMouseDown(Controller iSender, ref Vector2 iMousePosition)
		{
			if (this.mMusicScrollSlider.InsideBounds(ref iMousePosition))
			{
				if (this.mMusicScrollSlider.InsideDragBounds(iMousePosition))
				{
					this.mMusicScrollSlider.Grabbed = true;
					this.mSelectedItem = -1;
					return;
				}
				if (!this.mMusicScrollSlider.InsideLeftBounds(iMousePosition) & !this.mMusicScrollSlider.InsideRightBounds(iMousePosition))
				{
					int value = this.mMusicScrollSlider.Value;
					this.mMusicScrollSlider.ScrollTo(iMousePosition.X);
					if (value != this.mMusicScrollSlider.Value)
					{
						AudioManager.Instance.VolumeMusic(this.mMusicScrollSlider.Value);
						AudioManager.Instance.PlayCue(Banks.Misc, InGameMenuOptionsSound.SOUND_THUD);
						return;
					}
				}
			}
			else if (this.mSFXScrollSlider.InsideBounds(ref iMousePosition))
			{
				if (this.mSFXScrollSlider.InsideDragBounds(iMousePosition))
				{
					this.mSFXScrollSlider.Grabbed = true;
					this.mSelectedItem = -1;
					return;
				}
				if (!this.mSFXScrollSlider.InsideLeftBounds(iMousePosition) & !this.mSFXScrollSlider.InsideRightBounds(iMousePosition))
				{
					int value2 = this.mSFXScrollSlider.Value;
					this.mSFXScrollSlider.ScrollTo(iMousePosition.X);
					if (value2 != this.mSFXScrollSlider.Value)
					{
						AudioManager.Instance.VolumeSound(this.mSFXScrollSlider.Value);
						AudioManager.Instance.PlayCue(Banks.Misc, InGameMenuOptionsSound.SOUND_THUD);
						return;
					}
				}
			}
			else
			{
				base.IMouseDown(iSender, ref iMousePosition);
			}
		}

		// Token: 0x06002A00 RID: 10752 RVA: 0x0014A700 File Offset: 0x00148900
		protected override void IMouseScroll(Controller iSender, ref Vector2 iMousePosition, int iValue)
		{
			if (this.mMusicScrollSlider.InsideBounds(ref iMousePosition))
			{
				if (iValue > 0)
				{
					this.mMusicScrollSlider.Value--;
					return;
				}
				if (iValue < 0)
				{
					this.mMusicScrollSlider.Value++;
					return;
				}
			}
			else if (this.mSFXScrollSlider.InsideBounds(ref iMousePosition))
			{
				if (iValue > 0)
				{
					this.mSFXScrollSlider.Value--;
					return;
				}
				if (iValue < 0)
				{
					this.mSFXScrollSlider.Value++;
				}
			}
		}

		// Token: 0x06002A01 RID: 10753 RVA: 0x0014A788 File Offset: 0x00148988
		protected override void IMouseUp(Controller iSender, ref Vector2 iMousePosition)
		{
			if (this.mMusicScrollSlider.Grabbed | this.mSFXScrollSlider.Grabbed)
			{
				this.mMusicScrollSlider.Grabbed = false;
				this.mSFXScrollSlider.Grabbed = false;
				return;
			}
			if (this.mMusicScrollSlider.Value > 0 & this.mMusicScrollSlider.InsideLeftBounds(iMousePosition))
			{
				this.mMusicScrollSlider.Value--;
				AudioManager.Instance.VolumeMusic(this.mMusicScrollSlider.Value);
				AudioManager.Instance.PlayCue(Banks.Misc, InGameMenuOptionsSound.SOUND_THUD);
				return;
			}
			if (this.mMusicScrollSlider.Value < 10 & this.mMusicScrollSlider.InsideRightBounds(iMousePosition))
			{
				this.mMusicScrollSlider.Value++;
				AudioManager.Instance.VolumeMusic(this.mMusicScrollSlider.Value);
				AudioManager.Instance.PlayCue(Banks.Misc, InGameMenuOptionsSound.SOUND_THUD);
				return;
			}
			if (this.mSFXScrollSlider.Value > 0 & this.mSFXScrollSlider.InsideLeftBounds(iMousePosition))
			{
				this.mSFXScrollSlider.Value--;
				AudioManager.Instance.VolumeSound(this.mSFXScrollSlider.Value);
				AudioManager.Instance.PlayCue(Banks.Misc, InGameMenuOptionsSound.SOUND_THUD);
				return;
			}
			if (this.mSFXScrollSlider.Value < 10 & this.mSFXScrollSlider.InsideRightBounds(iMousePosition))
			{
				this.mSFXScrollSlider.Value++;
				AudioManager.Instance.VolumeSound(this.mSFXScrollSlider.Value);
				AudioManager.Instance.PlayCue(Banks.Misc, InGameMenuOptionsSound.SOUND_THUD);
				return;
			}
			base.IMouseUp(iSender, ref iMousePosition);
		}

		// Token: 0x06002A02 RID: 10754 RVA: 0x0014A954 File Offset: 0x00148B54
		protected override void IMouseMove(Controller iSender, ref Vector2 iMousePosition)
		{
			if (this.mMusicScrollSlider.Grabbed)
			{
				if (this.mMusicScrollSlider.Value > 0 & this.mMusicScrollSlider.InsideDragLeftBounds(iMousePosition))
				{
					this.mMusicScrollSlider.Value--;
					AudioManager.Instance.VolumeMusic(this.mMusicScrollSlider.Value);
					AudioManager.Instance.PlayCue(Banks.Misc, InGameMenuOptionsSound.SOUND_THUD);
				}
				if (this.mMusicScrollSlider.Value < 10 & this.mMusicScrollSlider.InsideDragRightBounds(iMousePosition))
				{
					this.mMusicScrollSlider.Value++;
					AudioManager.Instance.VolumeMusic(this.mMusicScrollSlider.Value);
					AudioManager.Instance.PlayCue(Banks.Misc, InGameMenuOptionsSound.SOUND_THUD);
					return;
				}
			}
			else if (this.mSFXScrollSlider.Grabbed)
			{
				if (this.mSFXScrollSlider.Value > 0 & this.mSFXScrollSlider.InsideDragLeftBounds(iMousePosition))
				{
					this.mSFXScrollSlider.Value--;
					AudioManager.Instance.VolumeSound(this.mSFXScrollSlider.Value);
					AudioManager.Instance.PlayCue(Banks.Misc, InGameMenuOptionsSound.SOUND_THUD);
				}
				if (this.mSFXScrollSlider.Value < 10 & this.mSFXScrollSlider.InsideDragRightBounds(iMousePosition))
				{
					this.mSFXScrollSlider.Value++;
					AudioManager.Instance.VolumeSound(this.mSFXScrollSlider.Value);
					AudioManager.Instance.PlayCue(Banks.Misc, InGameMenuOptionsSound.SOUND_THUD);
					return;
				}
			}
			else
			{
				if (this.mMenuItems[2].InsideBounds(ref iMousePosition))
				{
					if (this.mSelectedItem != 2)
					{
						AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_MOVE);
					}
					this.mSelectedItem = 2;
					return;
				}
				this.mSelectedItem = -1;
			}
		}

		// Token: 0x06002A03 RID: 10755 RVA: 0x0014AB44 File Offset: 0x00148D44
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
			this.mMusicScrollSlider.Value = (this.mMusicLevel = AudioManager.Instance.VolumeMusic());
			this.mSFXScrollSlider.Value = (this.mSfxLevel = AudioManager.Instance.VolumeSound());
		}

		// Token: 0x06002A04 RID: 10756 RVA: 0x0014ABAC File Offset: 0x00148DAC
		protected override void IDraw(float iDeltaTime, ref Vector2 iBackgroundSize)
		{
			this.UpdatePositions();
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
					transform.M41 = InGameMenu.sScreenSize.X * 0.5f - iBackgroundSize.X * 0.5f * InGameMenu.sScale;
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
				this.mMenuItems[j].Draw(InGameMenu.sEffect);
			}
			this.mMusicScrollSlider.Color = color;
			this.mSFXScrollSlider.Color = color;
			this.mMusicScrollSlider.Draw(InGameMenu.sEffect);
			this.mSFXScrollSlider.Draw(InGameMenu.sEffect);
		}

		// Token: 0x06002A05 RID: 10757 RVA: 0x0014AE68 File Offset: 0x00149068
		protected override void OnExit()
		{
			GlobalSettings instance = GlobalSettings.Instance;
			instance.VolumeMusic = AudioManager.Instance.VolumeMusic();
			instance.VolumeSound = AudioManager.Instance.VolumeSound();
			if (this.mMusicLevel != instance.VolumeMusic | this.mSfxLevel != instance.VolumeSound)
			{
				SaveManager.Instance.SaveSettings();
			}
		}

		// Token: 0x04002D74 RID: 11636
		private const string OPTION_BACK = "back";

		// Token: 0x04002D75 RID: 11637
		private static InGameMenuOptionsSound sSingelton;

		// Token: 0x04002D76 RID: 11638
		private static volatile object sSingeltonLock = new object();

		// Token: 0x04002D77 RID: 11639
		protected static readonly int SOUND_THUD = "misc_thud01".GetHashCodeCustom();

		// Token: 0x04002D78 RID: 11640
		private MenuScrollSlider mMusicScrollSlider;

		// Token: 0x04002D79 RID: 11641
		private MenuScrollSlider mSFXScrollSlider;

		// Token: 0x04002D7A RID: 11642
		private int mMusicLevel;

		// Token: 0x04002D7B RID: 11643
		private int mSfxLevel;

		// Token: 0x04002D7C RID: 11644
		private BitmapFont mFont;
	}
}
