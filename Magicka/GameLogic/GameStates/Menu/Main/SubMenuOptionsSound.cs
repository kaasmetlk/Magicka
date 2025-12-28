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

namespace Magicka.GameLogic.GameStates.Menu.Main
{
	// Token: 0x0200029D RID: 669
	internal class SubMenuOptionsSound : SubMenu
	{
		// Token: 0x17000528 RID: 1320
		// (get) Token: 0x06001403 RID: 5123 RVA: 0x0007BE3C File Offset: 0x0007A03C
		public static SubMenuOptionsSound Instance
		{
			get
			{
				if (SubMenuOptionsSound.mSingelton == null)
				{
					lock (SubMenuOptionsSound.mSingeltonLock)
					{
						if (SubMenuOptionsSound.mSingelton == null)
						{
							SubMenuOptionsSound.mSingelton = new SubMenuOptionsSound();
						}
					}
				}
				return SubMenuOptionsSound.mSingelton;
			}
		}

		// Token: 0x06001404 RID: 5124 RVA: 0x0007BE90 File Offset: 0x0007A090
		public override void LanguageChanged()
		{
			base.LanguageChanged();
			this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenuOptionsSound.LOC_SOUND_OPTS));
		}

		// Token: 0x06001405 RID: 5125 RVA: 0x0007BEB4 File Offset: 0x0007A0B4
		private SubMenuOptionsSound()
		{
			this.mSelectedPosition = 0;
			this.mMenuItems = new List<MenuItem>();
			this.mMenuTitle = new Text(32, FontManager.Instance.GetFont(MagickaFont.MenuTitle), TextAlign.Center, false);
			this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenuOptionsSound.LOC_SOUND_OPTS));
			this.mPosition.X = this.mPosition.X - 40f;
			float scale = Math.Min(1f, ((float)FontManager.Instance.GetFont(MagickaFont.MenuOption).LineHeight + 10f) / 64f);
			this.AddMenuTextItem(SubMenuOptionsSound.LOC_MUSIC);
			Vector2 position = this.mMenuItems[0].Position;
			position.X += 200f;
			this.mMusicScrollSlider = new MenuScrollSlider(position, 320f, 10);
			this.mMusicScrollSlider.Scale = scale;
			this.mMusicScrollSlider.Value = this.mMusicVolume;
			this.AddMenuTextItem(SubMenuOptionsSound.LOC_SFX);
			position = this.mMenuItems[1].Position;
			position.X += 200f;
			this.mSFXScrollSlider = new MenuScrollSlider(position, 320f, 10);
			this.mSFXScrollSlider.Scale = scale;
			this.mSFXScrollSlider.Value = this.mSFXVolume;
			this.mMenuItems.Add(new MenuImageTextItem(SubMenu.BACK_POSITION, SubMenu.sPagesTexture, SubMenu.BACK_UVOFFSET, SubMenu.BACK_UVSCALE, 0, default(Vector2), SubMenu.BACK_TEXT_ALIGN, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenu.BACK_SIZE));
		}

		// Token: 0x06001406 RID: 5126 RVA: 0x0007C088 File Offset: 0x0007A288
		public override MenuTextItem AddMenuTextItem(int iText)
		{
			Vector2 mPosition = this.mPosition;
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
			mPosition.Y += ((float)font.LineHeight + 10f) * (float)this.mMenuItems.Count;
			MenuTextItem menuTextItem = new MenuTextItem(iText, mPosition, font, TextAlign.Right);
			this.mMenuItems.Add(menuTextItem);
			return menuTextItem;
		}

		// Token: 0x06001407 RID: 5127 RVA: 0x0007C0E8 File Offset: 0x0007A2E8
		public override void Draw(Viewport iLeftSide, Viewport iRightSide)
		{
			base.Draw(iLeftSide, iRightSide);
			this.mEffect.Begin();
			this.mEffect.CurrentTechnique.Passes[0].Begin();
			base.DrawGraphics(SubMenu.sPagesTexture, new Rectangle(448, 976, 608, 48), new Rectangle(208, 220, 608, 48));
			this.mMusicScrollSlider.Draw(this.mEffect);
			this.mSFXScrollSlider.Draw(this.mEffect);
			this.mEffect.CurrentTechnique.Passes[0].End();
			this.mEffect.End();
		}

		// Token: 0x06001408 RID: 5128 RVA: 0x0007C1A2 File Offset: 0x0007A3A2
		public override void ControllerA(Controller iSender)
		{
			if (this.mSelectedPosition == 2)
			{
				Tome.Instance.PopMenu();
				return;
			}
			this.ControllerRight(iSender);
		}

		// Token: 0x06001409 RID: 5129 RVA: 0x0007C1C0 File Offset: 0x0007A3C0
		public override void ControllerRight(Controller iSender)
		{
			switch (this.mSelectedPosition)
			{
			case 0:
				this.mMusicVolume++;
				if (this.mMusicVolume > 10)
				{
					this.mMusicVolume = 10;
				}
				this.mMusicScrollSlider.Value = this.mMusicVolume;
				AudioManager.Instance.VolumeMusic(this.mMusicVolume);
				return;
			case 1:
				this.mSFXVolume++;
				if (this.mSFXVolume > 10)
				{
					this.mSFXVolume = 10;
				}
				this.mSFXScrollSlider.Value = this.mSFXVolume;
				AudioManager.Instance.VolumeSound(this.mSFXVolume);
				AudioManager.Instance.PlayCue(Banks.Misc, this.mSampleHash);
				return;
			default:
				return;
			}
		}

		// Token: 0x0600140A RID: 5130 RVA: 0x0007C27C File Offset: 0x0007A47C
		public override void ControllerLeft(Controller iSender)
		{
			switch (this.mSelectedPosition)
			{
			case 0:
				this.mMusicVolume--;
				if (this.mMusicVolume < 0)
				{
					this.mMusicVolume = 0;
				}
				this.mMusicScrollSlider.Value = this.mMusicVolume;
				AudioManager.Instance.VolumeMusic(this.mMusicVolume);
				return;
			case 1:
				this.mSFXVolume--;
				if (this.mSFXVolume < 0)
				{
					this.mSFXVolume = 0;
				}
				this.mSFXScrollSlider.Value = this.mSFXVolume;
				AudioManager.Instance.VolumeSound(this.mSFXVolume);
				AudioManager.Instance.PlayCue(Banks.Misc, this.mSampleHash);
				return;
			default:
				return;
			}
		}

		// Token: 0x0600140B RID: 5131 RVA: 0x0007C334 File Offset: 0x0007A534
		public override void ControllerMouseAction(Controller iSender, Point iScreenSize, MouseState iState, MouseState iOldState)
		{
			if (this.mMenuItems == null || this.mMenuItems.Count == 0)
			{
				return;
			}
			Vector2 iPoint;
			bool flag;
			if (Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out iPoint, out flag) && flag)
			{
				if (this.mMusicScrollSlider.InsideBounds(ref iPoint))
				{
					if (iState.LeftButton == ButtonState.Pressed && iOldState.LeftButton == ButtonState.Released)
					{
						if (this.mMusicScrollSlider.InsideDragBounds(iPoint))
						{
							this.mMusicScrollSlider.Grabbed = true;
							return;
						}
						if (this.mMusicScrollSlider.InsideLeftBounds(iPoint))
						{
							this.mMusicScrollSlider.Value--;
							this.mMusicVolume = this.mMusicScrollSlider.Value;
							AudioManager.Instance.VolumeMusic(this.mMusicVolume);
							return;
						}
						if (this.mMusicScrollSlider.InsideRightBounds(iPoint))
						{
							this.mMusicScrollSlider.Value++;
							this.mMusicVolume = this.mMusicScrollSlider.Value;
							AudioManager.Instance.VolumeMusic(this.mMusicVolume);
							return;
						}
						this.mMusicScrollSlider.ScrollTo(iPoint.X);
						this.mMusicVolume = this.mMusicScrollSlider.Value;
						AudioManager.Instance.VolumeMusic(this.mMusicVolume);
						return;
					}
					else
					{
						if (this.mMusicScrollSlider.Value > 0 && iState.ScrollWheelValue > iOldState.ScrollWheelValue)
						{
							this.mMusicScrollSlider.Value--;
							this.mMusicVolume = this.mMusicScrollSlider.Value;
							AudioManager.Instance.VolumeMusic(this.mMusicVolume);
							return;
						}
						if (this.mMusicScrollSlider.Value < 10 && iState.ScrollWheelValue < iOldState.ScrollWheelValue)
						{
							this.mMusicScrollSlider.Value++;
							this.mMusicVolume = this.mMusicScrollSlider.Value;
							AudioManager.Instance.VolumeMusic(this.mMusicVolume);
							return;
						}
					}
				}
				else if (this.mSFXScrollSlider.InsideBounds(ref iPoint))
				{
					if (iState.LeftButton == ButtonState.Pressed && iOldState.LeftButton == ButtonState.Released)
					{
						if (this.mSFXScrollSlider.InsideDragBounds(iPoint))
						{
							this.mSFXScrollSlider.Grabbed = true;
							return;
						}
						if (this.mSFXScrollSlider.InsideLeftBounds(iPoint))
						{
							this.mSFXScrollSlider.Value--;
							this.mSFXVolume = this.mSFXScrollSlider.Value;
							AudioManager.Instance.VolumeSound(this.mSFXVolume);
							AudioManager.Instance.PlayCue(Banks.Misc, this.mSampleHash);
							return;
						}
						if (this.mSFXScrollSlider.InsideRightBounds(iPoint))
						{
							this.mSFXScrollSlider.Value++;
							this.mSFXVolume = this.mSFXScrollSlider.Value;
							AudioManager.Instance.VolumeSound(this.mSFXVolume);
							AudioManager.Instance.PlayCue(Banks.Misc, this.mSampleHash);
							return;
						}
						this.mSFXScrollSlider.ScrollTo(iPoint.X);
						this.mSFXVolume = this.mSFXScrollSlider.Value;
						AudioManager.Instance.VolumeSound(this.mSFXVolume);
						AudioManager.Instance.PlayCue(Banks.Misc, this.mSampleHash);
						return;
					}
					else
					{
						if (this.mSFXScrollSlider.Value > 0 && iState.ScrollWheelValue > iOldState.ScrollWheelValue)
						{
							this.mSFXScrollSlider.Value--;
							this.mSFXVolume = this.mSFXScrollSlider.Value;
							AudioManager.Instance.VolumeSound(this.mSFXVolume);
							AudioManager.Instance.PlayCue(Banks.Misc, this.mSampleHash);
							return;
						}
						if (this.mSFXScrollSlider.Value < 10 && iState.ScrollWheelValue < iOldState.ScrollWheelValue)
						{
							this.mSFXScrollSlider.Value++;
							this.mSFXVolume = this.mSFXScrollSlider.Value;
							AudioManager.Instance.VolumeSound(this.mSFXVolume);
							AudioManager.Instance.PlayCue(Banks.Misc, this.mSampleHash);
							return;
						}
					}
				}
				else if (this.mMenuItems[this.mMenuItems.Count - 1].InsideBounds(ref iPoint))
				{
					Tome.Instance.PopMenu();
				}
			}
		}

		// Token: 0x0600140C RID: 5132 RVA: 0x0007C76C File Offset: 0x0007A96C
		public override void ControllerMouseMove(Controller iSender, Point iScreenSize, MouseState iState, MouseState iOldState)
		{
			if (this.mMenuItems == null || this.mMenuItems.Count == 0)
			{
				return;
			}
			if (iState.LeftButton == ButtonState.Released)
			{
				this.mSFXScrollSlider.Grabbed = false;
				this.mMusicScrollSlider.Grabbed = false;
			}
			Vector2 vector;
			bool flag;
			if (Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out vector, out flag))
			{
				if (flag)
				{
					if (this.mMusicScrollSlider.Grabbed)
					{
						this.mMusicScrollSlider.ScrollTo(vector.X);
						this.mMusicVolume = this.mMusicScrollSlider.Value;
						AudioManager.Instance.VolumeMusic(this.mMusicVolume);
						return;
					}
					if (this.mSFXScrollSlider.Grabbed)
					{
						int value = this.mSFXScrollSlider.Value;
						this.mSFXScrollSlider.ScrollTo(vector.X);
						if (value != this.mSFXScrollSlider.Value)
						{
							this.mSFXVolume = this.mSFXScrollSlider.Value;
							AudioManager.Instance.VolumeSound(this.mSFXVolume);
							AudioManager.Instance.PlayCue(Banks.Misc, this.mSampleHash);
							return;
						}
					}
					else
					{
						bool flag2 = false;
						for (int i = 0; i < this.mMenuItems.Count; i++)
						{
							MenuItem menuItem = this.mMenuItems[i];
							if (menuItem.Enabled && menuItem.InsideBounds(ref vector))
							{
								if (this.mSelectedPosition != i)
								{
									AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_mouseover".GetHashCodeCustom());
								}
								this.mKeyboardSelection = false;
								this.mSelectedPosition = i;
								for (int j = 0; j < this.mMenuItems.Count; j++)
								{
									this.mMenuItems[j].Selected = false;
								}
								menuItem.Selected = true;
								flag2 = true;
								break;
							}
						}
						if (!flag2)
						{
							for (int k = 0; k < this.mMenuItems.Count; k++)
							{
								this.mMenuItems[k].Selected = false;
							}
							return;
						}
					}
				}
			}
			else if (!this.mKeyboardSelection)
			{
				for (int l = 0; l < this.mMenuItems.Count; l++)
				{
					this.mMenuItems[l].Selected = false;
				}
			}
		}

		// Token: 0x0600140D RID: 5133 RVA: 0x0007C99C File Offset: 0x0007AB9C
		public override void OnEnter()
		{
			GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Bottom, SubMenu.LOC_SELECT);
			GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Right, SubMenu.LOC_BACK);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Left);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Top);
			this.mMusicLevel = AudioManager.Instance.VolumeMusic();
			this.mSfxLevel = AudioManager.Instance.VolumeSound();
		}

		// Token: 0x0600140E RID: 5134 RVA: 0x0007CA00 File Offset: 0x0007AC00
		public override void OnExit()
		{
			GlobalSettings.Instance.VolumeMusic = this.mMusicVolume;
			GlobalSettings.Instance.VolumeSound = this.mSFXVolume;
			if (this.mMusicLevel != this.mMusicVolume | this.mSfxLevel != this.mSFXVolume)
			{
				SaveManager.Instance.SaveSettings();
			}
		}

		// Token: 0x04001574 RID: 5492
		private static readonly int LOC_MUSIC = "#menu_opt_sfx_01".GetHashCodeCustom();

		// Token: 0x04001575 RID: 5493
		private static readonly int LOC_SFX = "#menu_opt_sfx_02".GetHashCodeCustom();

		// Token: 0x04001576 RID: 5494
		private static readonly int LOC_UI = "#menu_opt_sfx_03".GetHashCodeCustom();

		// Token: 0x04001577 RID: 5495
		private static readonly int LOC_DIALOGUES = "#menu_opt_sfx_04".GetHashCodeCustom();

		// Token: 0x04001578 RID: 5496
		private static readonly int LOC_SOUND_OPTS = "#menu_opt_08".GetHashCodeCustom();

		// Token: 0x04001579 RID: 5497
		private static SubMenuOptionsSound mSingelton;

		// Token: 0x0400157A RID: 5498
		private static volatile object mSingeltonLock = new object();

		// Token: 0x0400157B RID: 5499
		private int mMusicVolume = AudioManager.Instance.VolumeMusic();

		// Token: 0x0400157C RID: 5500
		private int mSFXVolume = AudioManager.Instance.VolumeSound();

		// Token: 0x0400157D RID: 5501
		private readonly int mSampleHash = "misc_thud01".GetHashCodeCustom();

		// Token: 0x0400157E RID: 5502
		private MenuScrollSlider mMusicScrollSlider;

		// Token: 0x0400157F RID: 5503
		private MenuScrollSlider mSFXScrollSlider;

		// Token: 0x04001580 RID: 5504
		private int mMusicLevel;

		// Token: 0x04001581 RID: 5505
		private int mSfxLevel;
	}
}
