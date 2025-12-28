using System;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Levels.Campaign;
using Magicka.Network;
using Magicka.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka.GameLogic.GameStates.Menu.Main
{
	// Token: 0x02000577 RID: 1399
	internal class SubMenuEndGame : SubMenu
	{
		// Token: 0x170009DF RID: 2527
		// (get) Token: 0x060029CE RID: 10702 RVA: 0x00147384 File Offset: 0x00145584
		public static SubMenuEndGame Instance
		{
			get
			{
				if (SubMenuEndGame.sSingelton == null)
				{
					lock (SubMenuEndGame.sSingeltonLock)
					{
						if (SubMenuEndGame.sSingelton == null)
						{
							SubMenuEndGame.sSingelton = new SubMenuEndGame();
						}
					}
				}
				return SubMenuEndGame.sSingelton;
			}
		}

		// Token: 0x060029CF RID: 10703 RVA: 0x001473D8 File Offset: 0x001455D8
		private SubMenuEndGame()
		{
			this.mRUSureMessageBox = new OptionsMessageBox("#add_menu_rus".GetHashCodeCustom(), new int[]
			{
				"#add_menu_yes".GetHashCodeCustom(),
				"#add_menu_no".GetHashCodeCustom()
			});
			this.mRUSureMessageBox.Select += new Action<OptionsMessageBox, int>(this.RUSureCallback);
			this.mMenuTitle = new Text(32, FontManager.Instance.GetFont(MagickaFont.MenuDefault), TextAlign.Center, false);
			this.mPosition = new Vector2(256f, 386f);
			this.mVertexDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, new VertexElement[]
			{
				new VertexElement(0, 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, 0),
				new VertexElement(0, 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0)
			});
			this.mVertices = new Vector4[4];
			this.mVertices[0].X = -400f;
			this.mVertices[0].Y = -320f;
			this.mVertices[0].Z = 0f;
			this.mVertices[1].X = 400f;
			this.mVertices[1].Y = -320f;
			this.mVertices[1].Z = 1f;
			this.mVertices[2].X = 400f;
			this.mVertices[2].Y = 320f;
			this.mVertices[2].Z = 1f;
			this.mVertices[3].X = -400f;
			this.mVertices[3].Y = 320f;
			this.mVertices[3].Z = 0f;
			this.CreditsEnd = false;
		}

		// Token: 0x060029D0 RID: 10704 RVA: 0x001475D0 File Offset: 0x001457D0
		private void RUSureCallback(MessageBox iSender, int iOption)
		{
			if (iOption == 0)
			{
				if (this.mIsCampaign && this.mSaveSlot != null)
				{
					SaveData saveData = this.mSaveSlot;
					saveData.Level -= 1;
					this.mSaveSlot.TotalPlayTime -= (int)PlayState.RecentPlayState.PlayTime;
					SaveManager.Instance.SaveCampaign();
				}
				PlayState.RecentPlayState.Restart(this, RestartType.StartOfLevel);
				Tome.Instance.CloseTomeInstant();
				GameStateManager.Instance.PushState(PlayState.RecentPlayState);
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					MenuSelectMessage menuSelectMessage = default(MenuSelectMessage);
					menuSelectMessage.IntendedMenu = MenuSelectMessage.MenuType.Statistics;
					menuSelectMessage.Option = 1;
					menuSelectMessage.Param0I = 0;
					NetworkManager.Instance.Interface.SendMessage<MenuSelectMessage>(ref menuSelectMessage);
				}
			}
		}

		// Token: 0x060029D1 RID: 10705 RVA: 0x00147694 File Offset: 0x00145894
		public new void AddMenuTextItem(string iText)
		{
			Vector2 mPosition = this.mPosition;
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuDefault);
			mPosition.Y += (float)(font.LineHeight * this.mMenuItems.Count);
			this.mMenuItems.Add(new MenuTextItem(iText + " (NonLoc)", mPosition, font, TextAlign.Center));
		}

		// Token: 0x170009E0 RID: 2528
		// (get) Token: 0x060029D2 RID: 10706 RVA: 0x001476F3 File Offset: 0x001458F3
		// (set) Token: 0x060029D3 RID: 10707 RVA: 0x001476FB File Offset: 0x001458FB
		public bool CreditsEnd { get; set; }

		// Token: 0x060029D4 RID: 10708 RVA: 0x00147704 File Offset: 0x00145904
		public override void ControllerA(Controller iSender)
		{
			if (this.mScreenShot != null || this.CreditsEnd)
			{
				return;
			}
			if (this.mSelectedPosition == 2)
			{
				Tome.Instance.PopMenu();
				return;
			}
			base.ControllerA(iSender);
			if (!this.mIsCampaign & this.mSelectedPosition == 0)
			{
				this.mSelectedPosition = 2;
			}
			if (this.mSelectedPosition == 1)
			{
				Tome.Instance.PopMenuInstant();
				this.mRUSureMessageBox.SelectedIndex = 1;
				this.mRUSureMessageBox.Show();
				return;
			}
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				MenuSelectMessage menuSelectMessage = default(MenuSelectMessage);
				menuSelectMessage.IntendedMenu = MenuSelectMessage.MenuType.Statistics;
				menuSelectMessage.Option = 2;
				menuSelectMessage.Param0I = 0;
				NetworkManager.Instance.Interface.SendMessage<MenuSelectMessage>(ref menuSelectMessage);
			}
			Tome.Instance.PopMenu();
		}

		// Token: 0x060029D5 RID: 10709 RVA: 0x001477CC File Offset: 0x001459CC
		private void OnTomeClose()
		{
			Tome.Instance.OnBackClose -= new Action(this.OnTomeClose);
			while (!(Tome.Instance.CurrentMenu is SubMenuMain))
			{
				Tome.Instance.PopMenuInstant();
			}
		}

		// Token: 0x060029D6 RID: 10710 RVA: 0x00147804 File Offset: 0x00145A04
		internal unsafe override void NetworkInput(ref MenuSelectMessage iMessage)
		{
			switch (iMessage.Option)
			{
			case 0:
			{
				byte[] array = new byte[32];
				fixed (int* ptr = &iMessage.Param0I)
				{
					byte* ptr2 = (byte*)ptr;
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = ptr2[i];
					}
				}
				int num;
				LevelManager.Instance.GetLevel(SubMenuCharacterSelect.Instance.GameType, array, out num);
				if (num == -1)
				{
					Tome.Instance.PopMenu();
					return;
				}
				SubMenuCutscene.Instance.Level = num;
				SubMenuCutscene.Instance.Play = true;
				Tome.Instance.PushMenu(SubMenuCutscene.Instance, 1);
				return;
			}
			case 1:
				PlayState.RecentPlayState.Restart(this, RestartType.StartOfLevel);
				Tome.Instance.CloseTomeInstant();
				GameStateManager.Instance.PushState(PlayState.RecentPlayState);
				return;
			case 2:
				Tome.Instance.PopMenu();
				return;
			default:
				return;
			}
		}

		// Token: 0x060029D7 RID: 10711 RVA: 0x001478D7 File Offset: 0x00145AD7
		public override void ControllerB(Controller iSender)
		{
			if (this.mScreenShot != null)
			{
				return;
			}
			base.ControllerB(iSender);
			Tome.Instance.PopMenu();
		}

		// Token: 0x060029D8 RID: 10712 RVA: 0x001478F4 File Offset: 0x00145AF4
		public override void ControllerUp(Controller iSender)
		{
			if (this.mMenuItems == null)
			{
				return;
			}
			this.mSelectedPosition--;
			if (this.mSelectedPosition < 0)
			{
				this.mSelectedPosition = this.mMenuItems.Count - 1;
			}
			if (!this.mIsCampaign & this.mSelectedPosition == 0)
			{
				this.mSelectedPosition = this.mMenuItems.Count - 1;
			}
		}

		// Token: 0x060029D9 RID: 10713 RVA: 0x0014795C File Offset: 0x00145B5C
		public override void ControllerDown(Controller iSender)
		{
			if (this.mMenuItems == null)
			{
				return;
			}
			this.mSelectedPosition++;
			if (this.mSelectedPosition >= this.mMenuItems.Count)
			{
				this.mSelectedPosition = 0;
			}
			if (!this.mIsCampaign & this.mSelectedPosition == 0)
			{
				this.mSelectedPosition = 1;
			}
		}

		// Token: 0x060029DA RID: 10714 RVA: 0x001479B8 File Offset: 0x00145BB8
		public override void Draw(Viewport iLeftSide, Viewport iRightSide)
		{
			this.mEffect.GraphicsDevice.Viewport = iRightSide;
			this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
			this.mEffect.Texture = this.mScreenShot;
			Matrix transform = default(Matrix);
			transform.M11 = (transform.M22 = (transform.M33 = (transform.M44 = 1f)));
			transform.M41 = 512f;
			transform.M42 = 512f;
			this.mEffect.Color = new Vector4(1f);
			this.mEffect.Transform = transform;
			this.mEffect.Begin();
			this.mEffect.CurrentTechnique.Passes[0].Begin();
			this.mEffect.GraphicsDevice.DrawUserPrimitives<Vector4>(PrimitiveType.TriangleFan, this.mVertices, 0, 2);
			this.mEffect.CurrentTechnique.Passes[0].End();
			this.mEffect.End();
		}

		// Token: 0x060029DB RID: 10715 RVA: 0x00147AD0 File Offset: 0x00145CD0
		public unsafe override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (!Tome.Instance.CameraMoving)
			{
				if (Tome.Instance.CurrentState is Tome.OpenState)
				{
					if (this.CreditsEnd)
					{
						Tome.Instance.ChangeState(Tome.CloseToBack.Instance);
						Tome.Instance.OnBackClose += new Action(this.OnTomeClose);
						return;
					}
					if (!this.mIsCampaign)
					{
						Tome.Instance.PopMenu();
						return;
					}
					if (NetworkManager.Instance.State == NetworkState.Server)
					{
						CampaignNode campaignNode = (SubMenuCharacterSelect.Instance.GameType == GameType.Mythos) ? LevelManager.Instance.MythosCampaign[(int)this.mSaveSlot.Level] : LevelManager.Instance.VanillaCampaign[(int)this.mSaveSlot.Level];
						SaveSlotInfo saveSlotInfo = new SaveSlotInfo(this.mSaveSlot);
						NetworkManager.Instance.Interface.SendMessage<SaveSlotInfo>(ref saveSlotInfo);
						MenuSelectMessage menuSelectMessage = default(MenuSelectMessage);
						menuSelectMessage.Option = 0;
						menuSelectMessage.IntendedMenu = MenuSelectMessage.MenuType.Statistics;
						byte* ptr = (byte*)(&menuSelectMessage.Param0I);
						byte[] combinedHash = campaignNode.GetCombinedHash();
						for (int i = 0; i < combinedHash.Length; i++)
						{
							ptr[i] = combinedHash[i];
						}
						NetworkManager.Instance.Interface.SendMessage<MenuSelectMessage>(ref menuSelectMessage);
					}
					if (NetworkManager.Instance.State != NetworkState.Client)
					{
						if (this.mSaveSlot != null)
						{
							SubMenuCutscene.Instance.Level = (int)this.mSaveSlot.Level;
							SubMenuCutscene.Instance.Play = true;
							Tome.Instance.PushMenu(SubMenuCutscene.Instance, 1);
							return;
						}
						Tome.Instance.PopMenu();
						return;
					}
				}
				else if (Tome.Instance.CurrentState is Tome.OpenState)
				{
					this.mScreenShot = null;
				}
			}
		}

		// Token: 0x060029DC RID: 10716 RVA: 0x00147C73 File Offset: 0x00145E73
		public override void OnEnter()
		{
			base.OnEnter();
			Tome.Instance.CrossfadeCameraAnimation(Tome.CameraAnimation.Idle, 1f);
		}

		// Token: 0x060029DD RID: 10717 RVA: 0x00147C8B File Offset: 0x00145E8B
		public override void OnExit()
		{
			base.OnExit();
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Bottom);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Right);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Left);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Top);
			this.CreditsEnd = false;
		}

		// Token: 0x060029DE RID: 10718 RVA: 0x00147CC6 File Offset: 0x00145EC6
		public void Set(bool iIsCampaign, SaveData iSaveSlot)
		{
			this.mIsCampaign = iIsCampaign;
			this.mSaveSlot = iSaveSlot;
		}

		// Token: 0x170009E1 RID: 2529
		// (get) Token: 0x060029DF RID: 10719 RVA: 0x00147CD6 File Offset: 0x00145ED6
		// (set) Token: 0x060029E0 RID: 10720 RVA: 0x00147CE0 File Offset: 0x00145EE0
		public Texture2D ScreenShot
		{
			get
			{
				return this.mScreenShot;
			}
			set
			{
				this.mScreenShot = value;
				float num = 1.25f / ((float)this.mScreenShot.Width / (float)this.mScreenShot.Height);
				float num2 = (1f - num) * 0.5f;
				this.mVertices[0].Z = num2;
				this.mVertices[1].Z = num2 + num;
				this.mVertices[2].Z = num2 + num;
				this.mVertices[3].Z = num2;
				this.mVertices[0].W = 0f;
				this.mVertices[1].W = 0f;
				this.mVertices[2].W = 1f;
				this.mVertices[3].W = 1f;
			}
		}

		// Token: 0x04002D2E RID: 11566
		private const float SCREENSHOT_WIDTH = 800f;

		// Token: 0x04002D2F RID: 11567
		private const float SCREENSHOT_HEIGHT = 640f;

		// Token: 0x04002D30 RID: 11568
		private static SubMenuEndGame sSingelton;

		// Token: 0x04002D31 RID: 11569
		private static volatile object sSingeltonLock = new object();

		// Token: 0x04002D32 RID: 11570
		private bool mIsCampaign;

		// Token: 0x04002D33 RID: 11571
		private SaveData mSaveSlot;

		// Token: 0x04002D34 RID: 11572
		private Texture2D mScreenShot;

		// Token: 0x04002D35 RID: 11573
		private Vector4[] mVertices;

		// Token: 0x04002D36 RID: 11574
		private VertexDeclaration mVertexDeclaration;

		// Token: 0x04002D37 RID: 11575
		private OptionsMessageBox mRUSureMessageBox;
	}
}
