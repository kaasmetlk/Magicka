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

namespace Magicka.GameLogic.GameStates.Menu.Main.Options
{
	// Token: 0x02000647 RID: 1607
	internal class SubMenuOptionsKeyboard : SubMenu
	{
		// Token: 0x17000B7E RID: 2942
		// (get) Token: 0x060030D6 RID: 12502 RVA: 0x001906FC File Offset: 0x0018E8FC
		public static SubMenuOptionsKeyboard Instance
		{
			get
			{
				if (SubMenuOptionsKeyboard.mSingelton == null)
				{
					lock (SubMenuOptionsKeyboard.mSingeltonLock)
					{
						if (SubMenuOptionsKeyboard.mSingelton == null)
						{
							SubMenuOptionsKeyboard.mSingelton = new SubMenuOptionsKeyboard();
						}
					}
				}
				return SubMenuOptionsKeyboard.mSingelton;
			}
		}

		// Token: 0x060030D7 RID: 12503 RVA: 0x00190750 File Offset: 0x0018E950
		private SubMenuOptionsKeyboard()
		{
			LanguageManager instance = LanguageManager.Instance;
			this.mFont = FontManager.Instance.GetFont(MagickaFont.MenuOption);
			this.mSelectedPosition = 0;
			this.mMenuItems = new List<MenuItem>();
			this.mMenuOptions = new List<MenuTextItem>();
			this.mMenuTitle = new Text(48, FontManager.Instance.GetFont(MagickaFont.MenuTitle), TextAlign.Center, false);
			this.mMenuTitle.SetText(instance.GetString(SubMenuOptionsKeyboard.LOC_KEYB_OPTS));
			base.AddMenuTextItem(SubMenuOptionsKeyboard.LOC_DEFAULTS);
			this.mMenuItems[this.mMenuItems.Count - 1].Position = new Vector2(512f, this.mPosition.Y + (float)((this.mMenuItems.Count - 1) * this.mFont.LineHeight) + 20f);
			this.AddMenuOptions(KeyboardMouseController.KeyToString(KeyboardBindings.Water));
			this.AddMenuTextItem(SubMenuOptionsKeyboard.LOC_WATER);
			this.AddMenuOptions(KeyboardMouseController.KeyToString(KeyboardBindings.Life));
			this.AddMenuTextItem(SubMenuOptionsKeyboard.LOC_LIFE);
			this.AddMenuOptions(KeyboardMouseController.KeyToString(KeyboardBindings.Shield));
			this.AddMenuTextItem(SubMenuOptionsKeyboard.LOC_SHIELD);
			this.AddMenuOptions(KeyboardMouseController.KeyToString(KeyboardBindings.Cold));
			this.AddMenuTextItem(SubMenuOptionsKeyboard.LOC_COLD);
			this.AddMenuOptions(KeyboardMouseController.KeyToString(KeyboardBindings.Lightning));
			this.AddMenuTextItem(SubMenuOptionsKeyboard.LOC_LIGHTNING);
			this.AddMenuOptions(KeyboardMouseController.KeyToString(KeyboardBindings.Arcane));
			this.AddMenuTextItem(SubMenuOptionsKeyboard.LOC_ARCANE);
			this.AddMenuOptions(KeyboardMouseController.KeyToString(KeyboardBindings.Earth));
			this.AddMenuTextItem(SubMenuOptionsKeyboard.LOC_EARTH);
			this.AddMenuOptions(KeyboardMouseController.KeyToString(KeyboardBindings.Fire));
			this.AddMenuTextItem(SubMenuOptionsKeyboard.LOC_FIRE);
			this.AddMenuOptions(KeyboardMouseController.KeyToString(KeyboardBindings.Cast));
			this.AddMenuTextItem(instance.GetString(SubMenuOptionsKeyboard.LOC_CAST_FORCE) + '/' + instance.GetString(SubMenuOptionsKeyboard.LOC_CAST_AREA));
			this.AddMenuOptions(KeyboardMouseController.KeyToString(KeyboardBindings.CastSelf));
			this.AddMenuTextItem(SubMenuOptionsKeyboard.LOC_CAST_SELF);
			this.AddMenuOptions(KeyboardMouseController.KeyToString(KeyboardBindings.Boost));
			this.AddMenuTextItem(instance.GetString(SubMenuOptionsKeyboard.LOC_KB_BOOST) + '/' + instance.GetString(SubMenuOptionsKeyboard.LOC_KB_CAST_MAGICK));
			this.AddMenuOptions(KeyboardMouseController.KeyToString(KeyboardBindings.Move));
			this.AddMenuTextItem(instance.GetString(SubMenuOptionsKeyboard.LOC_MOVE) + '/' + instance.GetString(SubMenuOptionsKeyboard.LOC_ATTACK));
			this.AddMenuOptions(KeyboardMouseController.KeyToString(KeyboardBindings.Block));
			this.AddMenuTextItem(SubMenuOptionsKeyboard.LOC_KB_BLOCK);
			this.AddMenuOptions(KeyboardMouseController.KeyToString(KeyboardBindings.Shift));
			this.AddMenuTextItem(SubMenuOptionsKeyboard.LOC_KB_SHIFT);
			this.AddMenuOptions(KeyboardMouseController.KeyToString(KeyboardBindings.Inventory));
			this.AddMenuTextItem(SubMenuOptionsKeyboard.LOC_KB_INVENTORY);
			this.AddMenuOptions(KeyboardMouseController.KeyToString(KeyboardBindings.PrevMagick));
			this.AddMenuTextItem(SubMenuOptionsKeyboard.LOC_MAGICK_PREV);
			this.AddMenuOptions(KeyboardMouseController.KeyToString(KeyboardBindings.NextMagick));
			this.AddMenuTextItem(SubMenuOptionsKeyboard.LOC_MAGICK_NEXT);
			this.mBackItem = new MenuImageTextItem(SubMenu.BACK_POSITION, SubMenu.sPagesTexture, SubMenu.BACK_UVOFFSET, SubMenu.BACK_UVSCALE, 0, default(Vector2), SubMenu.BACK_TEXT_ALIGN, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenu.BACK_SIZE);
			this.mScrollBar = new MenuScrollBar(new Vector2(928f, 512f), (float)(this.mFont.LineHeight * 13), 5);
		}

		// Token: 0x060030D8 RID: 12504 RVA: 0x00190A98 File Offset: 0x0018EC98
		public override void LanguageChanged()
		{
			base.LanguageChanged();
			this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenuOptionsKeyboard.LOC_KEYB_OPTS));
			LanguageManager instance = LanguageManager.Instance;
			(this.mMenuItems[9] as MenuTextItem).SetText(instance.GetString(SubMenuOptionsKeyboard.LOC_CAST_FORCE) + '/' + instance.GetString(SubMenuOptionsKeyboard.LOC_CAST_AREA));
			(this.mMenuItems[11] as MenuTextItem).SetText(instance.GetString(SubMenuOptionsKeyboard.LOC_KB_BOOST) + '/' + instance.GetString(SubMenuOptionsKeyboard.LOC_KB_CAST_MAGICK));
			(this.mMenuItems[12] as MenuTextItem).SetText(instance.GetString(SubMenuOptionsKeyboard.LOC_MOVE) + '/' + instance.GetString(SubMenuOptionsKeyboard.LOC_ATTACK));
			this.UpdateMenuOptions();
		}

		// Token: 0x060030D9 RID: 12505 RVA: 0x00190B7C File Offset: 0x0018ED7C
		public override MenuTextItem AddMenuTextItem(int iText)
		{
			Vector2 mPosition = this.mPosition;
			mPosition.X -= 20f;
			mPosition.Y += (float)(this.mFont.LineHeight * this.mMenuItems.Count);
			MenuTextItem menuTextItem = new MenuTextItem(iText, mPosition, this.mFont, TextAlign.Right);
			this.mMenuItems.Add(menuTextItem);
			return menuTextItem;
		}

		// Token: 0x060030DA RID: 12506 RVA: 0x00190BE8 File Offset: 0x0018EDE8
		public new void AddMenuTextItem(string iText)
		{
			Vector2 mPosition = this.mPosition;
			mPosition.X -= 20f;
			mPosition.Y += (float)(this.mFont.LineHeight * this.mMenuItems.Count);
			this.mMenuItems.Add(new MenuTextItem(iText, mPosition, this.mFont, TextAlign.Right));
		}

		// Token: 0x060030DB RID: 12507 RVA: 0x00190C50 File Offset: 0x0018EE50
		public void AddMenuOptions(string iText)
		{
			Vector2 mPosition = this.mPosition;
			mPosition.X += 20f;
			mPosition.Y += (float)(this.mFont.LineHeight * this.mMenuItems.Count);
			this.mMenuOptions.Add(new MenuTextItem(iText, mPosition, this.mFont, TextAlign.Left, false));
		}

		// Token: 0x060030DC RID: 12508 RVA: 0x00190CB8 File Offset: 0x0018EEB8
		public override void Draw(Viewport iLeftSide, Viewport iRightSide)
		{
			this.mEffect.GraphicsDevice.Viewport = iRightSide;
			this.mEffect.Begin();
			this.mEffect.CurrentTechnique.Passes[0].Begin();
			this.mEffect.VertexColorEnabled = false;
			this.mEffect.Color = MenuItem.COLOR;
			this.mMenuTitle.Draw(this.mEffect, 512f, 96f);
			this.mScrollBar.Draw(this.mEffect);
			this.mBackItem.Selected = (this.mSelectedPosition == this.mMenuItems.Count);
			this.mBackItem.Draw(this.mEffect);
			base.DrawGraphics(SubMenu.sPagesTexture, new Rectangle(448, 976, 608, 48), new Rectangle(208, 220, 608, 48));
			float num = (float)this.mFont.LineHeight;
			float num2 = this.mPosition.Y - num * 0.5f;
			MenuItem menuItem;
			Vector2 position;
			for (int i = this.mScrollBar.Value + 1; i < this.mScrollBar.Value + 12 + 1; i++)
			{
				menuItem = this.mMenuItems[i];
				menuItem.Selected = (i == this.mSelectedPosition);
				position = menuItem.Position;
				position.Y = num2;
				menuItem.Position = position;
				menuItem.Draw(this.mEffect);
				menuItem = this.mMenuOptions[i - 1];
				menuItem.Selected = (i == this.mSelectedPosition);
				position = menuItem.Position;
				position.Y = num2;
				menuItem.Position = position;
				menuItem.Draw(this.mEffect);
				num2 += num;
			}
			menuItem = this.mMenuItems[0];
			menuItem.Selected = (this.mSelectedPosition == 0);
			position = menuItem.Position;
			position.Y = num2 + 20f;
			menuItem.Position = position;
			menuItem.Draw(this.mEffect);
			this.mEffect.CurrentTechnique.Passes[0].End();
			this.mEffect.End();
		}

		// Token: 0x060030DD RID: 12509 RVA: 0x00190EEC File Offset: 0x0018F0EC
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			for (int i = 0; i < this.mMenuItems.Count; i++)
			{
				(this.mMenuItems[i] as MenuTextItem).MaxWidth = 400f;
			}
			if (this.mKeyboardSelection)
			{
				for (int j = 0; j < this.mMenuItems.Count; j++)
				{
					this.mMenuItems[j].Selected = (j == this.mSelectedPosition);
					if (j < this.mMenuOptions.Count)
					{
						this.mMenuOptions[j].Selected = (j == this.mSelectedPosition);
					}
				}
			}
			if (this.mIsKeyRead & !KeyboardMouseController.mCatchKeyActive)
			{
				this.UpdateMenuOptions();
				this.mIsKeyRead = false;
				this.mKeysHasChanged = true;
			}
		}

		// Token: 0x060030DE RID: 12510 RVA: 0x00190FAF File Offset: 0x0018F1AF
		protected override void ControllerMouseClicked(Controller iSender)
		{
			if (this.mSelectedPosition == this.mMenuItems.Count)
			{
				Tome.Instance.PopMenu();
				return;
			}
			this.mKeyboardSelection = true;
			this.ControllerRight(iSender);
			this.mKeyboardSelection = false;
		}

		// Token: 0x060030DF RID: 12511 RVA: 0x00190FE4 File Offset: 0x0018F1E4
		public override void ControllerMouseMove(Controller iSender, Point iScreenSize, MouseState iState, MouseState iOldState)
		{
			if (this.mIsKeyRead)
			{
				return;
			}
			Vector2 iPoint;
			bool flag;
			if (Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out iPoint, out flag) && flag)
			{
				if (this.mScrollBar.Grabbed)
				{
					if (this.mScrollBar.InsideDragUpBounds(iPoint))
					{
						this.mScrollBar.Value--;
						return;
					}
					if (this.mScrollBar.InsideDragDownBounds(iPoint))
					{
						this.mScrollBar.Value++;
						return;
					}
				}
				else
				{
					int num = -1;
					if (this.mMenuItems[0].InsideBounds(ref iPoint))
					{
						num = 0;
					}
					if (this.mBackItem.InsideBounds(ref iPoint))
					{
						this.mSelectedPosition = this.mMenuItems.Count;
					}
					for (int i = this.mScrollBar.Value + 1; i < this.mScrollBar.Value + 1 + 12; i++)
					{
						MenuItem menuItem = this.mMenuItems[i];
						MenuItem menuItem2 = null;
						if (i > 0 && i <= this.mMenuOptions.Count)
						{
							menuItem2 = this.mMenuOptions[i - 1];
						}
						if (menuItem.Enabled && (menuItem.InsideBounds(ref iPoint) || (menuItem2 != null && menuItem2.InsideBounds(ref iPoint))))
						{
							num = i;
							break;
						}
					}
					if (num >= 0)
					{
						if (this.mSelectedPosition != num)
						{
							AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_mouseover".GetHashCodeCustom());
						}
						this.mKeyboardSelection = false;
					}
					this.mSelectedPosition = num;
				}
			}
		}

		// Token: 0x060030E0 RID: 12512 RVA: 0x0019115C File Offset: 0x0018F35C
		public override void ControllerMouseAction(Controller iSender, Point iScreenSize, MouseState iState, MouseState iOldState)
		{
			if (this.mIsKeyRead)
			{
				return;
			}
			if (iState.LeftButton == ButtonState.Released)
			{
				this.mScrollBar.Grabbed = false;
			}
			if (iState.ScrollWheelValue > iOldState.ScrollWheelValue)
			{
				this.mScrollBar.Value--;
				return;
			}
			if (iState.ScrollWheelValue < iOldState.ScrollWheelValue)
			{
				this.mScrollBar.Value++;
				return;
			}
			Vector2 iPoint;
			bool flag;
			if (Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out iPoint, out flag) && flag)
			{
				if (iState.LeftButton == ButtonState.Pressed && iOldState.LeftButton == ButtonState.Released)
				{
					if (this.mScrollBar.InsideBounds(ref iPoint))
					{
						if (this.mScrollBar.InsideDragBounds(iPoint))
						{
							this.mScrollBar.Grabbed = true;
							return;
						}
						if (this.mScrollBar.InsideUpBounds(iPoint))
						{
							this.mScrollBar.Value--;
							return;
						}
						if (this.mScrollBar.InsideDownBounds(iPoint))
						{
							this.mScrollBar.Value++;
							return;
						}
						this.mScrollBar.ScrollTo(iPoint.Y);
						return;
					}
					else if (this.mBackItem.Enabled && this.mBackItem.InsideBounds(ref iPoint))
					{
						Tome.Instance.PopMenu();
						return;
					}
				}
				else if (iState.LeftButton == ButtonState.Released && iOldState.LeftButton == ButtonState.Pressed && this.mSelectedPosition >= 0)
				{
					MenuItem menuItem = this.mMenuItems[this.mSelectedPosition];
					MenuItem menuItem2 = null;
					if (this.mSelectedPosition > 0 && this.mSelectedPosition <= this.mMenuOptions.Count)
					{
						menuItem2 = this.mMenuOptions[this.mSelectedPosition - 1];
					}
					if (menuItem.Enabled && (menuItem.InsideBounds(ref iPoint) || (menuItem2 != null && menuItem2.InsideBounds(ref iPoint))))
					{
						this.ControllerMouseClicked(iSender);
					}
				}
			}
		}

		// Token: 0x060030E1 RID: 12513 RVA: 0x0019133F File Offset: 0x0018F53F
		public override void ControllerA(Controller iSender)
		{
			if (this.mSelectedPosition == this.mMenuItems.Count)
			{
				Tome.Instance.PopMenu();
				return;
			}
			base.ControllerA(iSender);
			this.ControllerRight(iSender);
		}

		// Token: 0x060030E2 RID: 12514 RVA: 0x0019136D File Offset: 0x0018F56D
		public override void ControllerB(Controller iSender)
		{
			if (!this.mIsKeyRead)
			{
				base.ControllerB(iSender);
			}
		}

		// Token: 0x060030E3 RID: 12515 RVA: 0x00191380 File Offset: 0x0018F580
		public override void ControllerUp(Controller iSender)
		{
			if (this.mSelectedPosition == 1)
			{
				this.mSelectedPosition = this.mMenuItems.Count;
				return;
			}
			if (this.mSelectedPosition == this.mMenuItems.Count)
			{
				this.mSelectedPosition = 0;
				return;
			}
			if (this.mSelectedPosition == 0)
			{
				this.mSelectedPosition = this.mMenuItems.Count - 1;
				this.mScrollBar.Value = this.mScrollBar.MaxValue;
				return;
			}
			base.ControllerUp(iSender);
			if (this.mSelectedPosition > 0 && this.mSelectedPosition <= this.mMenuOptions.Count)
			{
				while (this.mScrollBar.Value + 1 > this.mSelectedPosition)
				{
					this.mScrollBar.Value--;
				}
				while (this.mScrollBar.Value + 12 < this.mSelectedPosition)
				{
					this.mScrollBar.Value++;
				}
			}
		}

		// Token: 0x060030E4 RID: 12516 RVA: 0x00191470 File Offset: 0x0018F670
		public override void ControllerDown(Controller iSender)
		{
			if (this.mSelectedPosition == 0)
			{
				this.mSelectedPosition = this.mMenuItems.Count;
				return;
			}
			if (this.mSelectedPosition == this.mMenuItems.Count)
			{
				this.mSelectedPosition = 1;
				this.mScrollBar.Value = 0;
				return;
			}
			base.ControllerDown(iSender);
			if (this.mSelectedPosition > 0 && this.mSelectedPosition <= this.mMenuOptions.Count)
			{
				while (this.mScrollBar.Value + 1 > this.mSelectedPosition)
				{
					this.mScrollBar.Value--;
				}
				while (this.mScrollBar.Value + 12 < this.mSelectedPosition)
				{
					this.mScrollBar.Value++;
				}
			}
		}

		// Token: 0x060030E5 RID: 12517 RVA: 0x00191538 File Offset: 0x0018F738
		public override void ControllerRight(Controller iSender)
		{
			if (!this.mKeyboardSelection)
			{
				return;
			}
			if (this.mSelectedPosition == 0)
			{
				KeyboardMouseController.LoadDefaults();
				this.UpdateMenuOptions();
				this.mKeysHasChanged = true;
				return;
			}
			if (this.mSelectedPosition > 0 && this.mSelectedPosition <= 17)
			{
				this.ActivateCatchKeyIndicator((KeyboardBindings)(this.mSelectedPosition - 1));
			}
		}

		// Token: 0x060030E6 RID: 12518 RVA: 0x0019158A File Offset: 0x0018F78A
		public override void OnEnter()
		{
			base.OnEnter();
			GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Bottom, SubMenuOptionsKeyboard.LOC_CHANGEBINDING);
			GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Right, SubMenu.LOC_BACK);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Left);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Top);
		}

		// Token: 0x060030E7 RID: 12519 RVA: 0x001915C8 File Offset: 0x0018F7C8
		public override void OnExit()
		{
			this.UpdateMenuOptions();
			if (this.mKeysHasChanged)
			{
				KeyboardMouseController.mKeyboardBindings.CopyTo(SaveManager.Instance.KeyBindings, 0);
				SaveManager.Instance.SaveSettings();
				KeyboardHUD.Instance.UpdateControls();
			}
			this.mKeysHasChanged = false;
			this.mIsKeyRead = false;
		}

		// Token: 0x060030E8 RID: 12520 RVA: 0x0019161C File Offset: 0x0018F81C
		public void UpdateMenuOptions()
		{
			this.mSelectedPosition = -1;
			for (int i = 0; i < 17; i++)
			{
				this.mMenuOptions[i].SetText(KeyboardMouseController.KeyToString((KeyboardBindings)i));
			}
		}

		// Token: 0x060030E9 RID: 12521 RVA: 0x00191654 File Offset: 0x0018F854
		public void ActivateCatchKeyIndicator(KeyboardBindings iKeyBinding)
		{
			this.mIsKeyRead = true;
			KeyboardMouseController.mCatchKeyActive = true;
			KeyboardMouseController.mCatchKeyIndex = iKeyBinding;
			this.mMenuOptions[(int)iKeyBinding].SetText(KeyboardMouseController.KeyToString(iKeyBinding) + " ???");
		}

		// Token: 0x040034CC RID: 13516
		private const int VISIBLE_ITEMS = 12;

		// Token: 0x040034CD RID: 13517
		private static readonly int LOC_KEYB_OPTS = "#menu_opt_11".GetHashCodeCustom();

		// Token: 0x040034CE RID: 13518
		private static readonly int LOC_RECONFIG = "#menu_reconfig".GetHashCodeCustom();

		// Token: 0x040034CF RID: 13519
		private static readonly int LOC_DEFAULTS = "#menu_restore_d".GetHashCodeCustom();

		// Token: 0x040034D0 RID: 13520
		private static readonly int LOC_KB_INVENTORY = "#ctrl_kb_inventory".GetHashCodeCustom();

		// Token: 0x040034D1 RID: 13521
		private static readonly int LOC_KB_BOOST = "#ctrl_kb_boost".GetHashCodeCustom();

		// Token: 0x040034D2 RID: 13522
		private static readonly int LOC_KB_CASTSELF = "#ctrl_cast_self".GetHashCodeCustom();

		// Token: 0x040034D3 RID: 13523
		private static readonly int LOC_KB_BLOCK = "#ctrl_kb_block".GetHashCodeCustom();

		// Token: 0x040034D4 RID: 13524
		private static readonly int LOC_KB_SHIFT = "#ctrl_kb_shift".GetHashCodeCustom();

		// Token: 0x040034D5 RID: 13525
		private static readonly int LOC_KB_CAST = "#ctrl_kb_cast".GetHashCodeCustom();

		// Token: 0x040034D6 RID: 13526
		private static readonly int LOC_KB_CAST_MAGICK = "#ctrl_kb_cast_magick".GetHashCodeCustom();

		// Token: 0x040034D7 RID: 13527
		private static readonly int LOC_KB_CAST_SELF = "#ctrl_kb_cast_self".GetHashCodeCustom();

		// Token: 0x040034D8 RID: 13528
		private static readonly int LOC_FIRE = "#element_fire".GetHashCodeCustom();

		// Token: 0x040034D9 RID: 13529
		private static readonly int LOC_COLD = "#element_cold".GetHashCodeCustom();

		// Token: 0x040034DA RID: 13530
		private static readonly int LOC_WATER = "#element_water".GetHashCodeCustom();

		// Token: 0x040034DB RID: 13531
		private static readonly int LOC_EARTH = "#element_earth".GetHashCodeCustom();

		// Token: 0x040034DC RID: 13532
		private static readonly int LOC_ARCANE = "#element_arcane".GetHashCodeCustom();

		// Token: 0x040034DD RID: 13533
		private static readonly int LOC_LIFE = "#element_life".GetHashCodeCustom();

		// Token: 0x040034DE RID: 13534
		private static readonly int LOC_LIGHTNING = "#element_lightning".GetHashCodeCustom();

		// Token: 0x040034DF RID: 13535
		private static readonly int LOC_SHIELD = "#element_shield".GetHashCodeCustom();

		// Token: 0x040034E0 RID: 13536
		private static readonly int LOC_ICE = "#element_ice".GetHashCodeCustom();

		// Token: 0x040034E1 RID: 13537
		private static readonly int LOC_STEAM = "#element_steam".GetHashCodeCustom();

		// Token: 0x040034E2 RID: 13538
		private static readonly int LOC_POISON = "#element_poison".GetHashCodeCustom();

		// Token: 0x040034E3 RID: 13539
		private static readonly int LOC_CAST_FORCE = "#menu_ctrl_01".GetHashCodeCustom();

		// Token: 0x040034E4 RID: 13540
		private static readonly int LOC_CAST_AREA = "#menu_ctrl_02".GetHashCodeCustom();

		// Token: 0x040034E5 RID: 13541
		private static readonly int LOC_CAST_SELF = "#menu_ctrl_03".GetHashCodeCustom();

		// Token: 0x040034E6 RID: 13542
		private static readonly int LOC_CAST_WEAPON = "#menu_ctrl_04".GetHashCodeCustom();

		// Token: 0x040034E7 RID: 13543
		private static readonly int LOC_CAST_MAGICK = "#menu_ctrl_05".GetHashCodeCustom();

		// Token: 0x040034E8 RID: 13544
		private static readonly int LOC_BOOST = "#menu_ctrl_06".GetHashCodeCustom();

		// Token: 0x040034E9 RID: 13545
		private static readonly int LOC_ITEM_ABILITY = "#menu_ctrl_07".GetHashCodeCustom();

		// Token: 0x040034EA RID: 13546
		private static readonly int LOC_INTERACT = "#menu_ctrl_08".GetHashCodeCustom();

		// Token: 0x040034EB RID: 13547
		private static readonly int LOC_ATTACK = "#menu_ctrl_09".GetHashCodeCustom();

		// Token: 0x040034EC RID: 13548
		private static readonly int LOC_WALK = "#menu_ctrl_10".GetHashCodeCustom();

		// Token: 0x040034ED RID: 13549
		private static readonly int LOC_CONJURE_SPELL = "#menu_ctrl_11".GetHashCodeCustom();

		// Token: 0x040034EE RID: 13550
		private static readonly int LOC_TOME = "#menu_ctrl_12".GetHashCodeCustom();

		// Token: 0x040034EF RID: 13551
		private static readonly int LOC_LEFT = "#menu_ctrl_13".GetHashCodeCustom();

		// Token: 0x040034F0 RID: 13552
		private static readonly int LOC_RIGHT = "#menu_ctrl_14".GetHashCodeCustom();

		// Token: 0x040034F1 RID: 13553
		private static readonly int LOC_SHOW = "#menu_ctrl_15".GetHashCodeCustom();

		// Token: 0x040034F2 RID: 13554
		private static readonly int LOC_HIDE = "#menu_ctrl_16".GetHashCodeCustom();

		// Token: 0x040034F3 RID: 13555
		private static readonly int LOC_BLOCK = "#menu_ctrl_17".GetHashCodeCustom();

		// Token: 0x040034F4 RID: 13556
		private static readonly int LOC_MOVE = "#ctrl_move".GetHashCodeCustom();

		// Token: 0x040034F5 RID: 13557
		private static readonly int LOC_CHANGEBINDING = "#opt_changebinding".GetHashCodeCustom();

		// Token: 0x040034F6 RID: 13558
		private static readonly int LOC_MAGICK_PREV = "#ctrl_magick_prev".GetHashCodeCustom();

		// Token: 0x040034F7 RID: 13559
		private static readonly int LOC_MAGICK_NEXT = "#ctrl_magick_next".GetHashCodeCustom();

		// Token: 0x040034F8 RID: 13560
		private static SubMenuOptionsKeyboard mSingelton;

		// Token: 0x040034F9 RID: 13561
		private static volatile object mSingeltonLock = new object();

		// Token: 0x040034FA RID: 13562
		private bool mIsKeyRead;

		// Token: 0x040034FB RID: 13563
		private bool mKeysHasChanged;

		// Token: 0x040034FC RID: 13564
		private readonly int mSampleHash = "misc_gib".GetHashCodeCustom();

		// Token: 0x040034FD RID: 13565
		private BitmapFont mFont;

		// Token: 0x040034FE RID: 13566
		private List<MenuTextItem> mMenuOptions;

		// Token: 0x040034FF RID: 13567
		private MenuScrollBar mScrollBar;

		// Token: 0x04003500 RID: 13568
		private MenuImageTextItem mBackItem;
	}
}
