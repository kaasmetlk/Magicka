using System;
using System.Threading;
using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.GameLogic.GameStates.Menu.Main.Options;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Network;
using Magicka.Storage;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.GameStates
{
	// Token: 0x02000496 RID: 1174
	internal class MenuState : GameState
	{
		// Token: 0x1700087C RID: 2172
		// (get) Token: 0x060023A1 RID: 9121 RVA: 0x00100B64 File Offset: 0x000FED64
		public static MenuState Instance
		{
			get
			{
				if (MenuState.mSingelton == null)
				{
					lock (MenuState.mSingeltonLock)
					{
						if (MenuState.mSingelton == null)
						{
							MenuState.mSingelton = new MenuState();
						}
					}
				}
				return MenuState.mSingelton;
			}
		}

		// Token: 0x060023A2 RID: 9122 RVA: 0x00100BB8 File Offset: 0x000FEDB8
		private MenuState() : base(new Camera(MenuState.CAMERA_STARTPOS, Vector3.Right, Vector3.Up, 0.7853982f, 1.7777778f, 1f, 500f))
		{
		}

		// Token: 0x060023A3 RID: 9123 RVA: 0x00100C09 File Offset: 0x000FEE09
		public void Initialize()
		{
		}

		// Token: 0x060023A4 RID: 9124 RVA: 0x00100C0C File Offset: 0x000FEE0C
		public override void OnEnter()
		{
			this.mStoreShowing = false;
			this.mStorePosition = 1.25f;
			AudioManager instance = AudioManager.Instance;
			while ((ushort)(instance.InitializedBanks & (Banks.Music | Banks.UI)) != 10)
			{
				Thread.Sleep(1);
			}
			AudioManager.Instance.PlayMusic(Banks.Music, MenuState.MUSIC_MENU, null);
			Vector4 vector = default(Vector4);
			vector.W = 0.75f;
			Vector4 vector2 = default(Vector4);
			vector2.X = (vector2.Y = (vector2.Z = 1f));
			vector2.W = 1f;
			ToolTipMan.Instance.Initialize(400, ref vector, ref vector2);
			MenuState.mCameraSpeed = 0.001f;
			if (SaveManager.Instance.AlreadyLoaded)
			{
				SaveManager.Instance.SaveLeaderBoards();
			}
			else
			{
				Game.Instance.AddLoadTask(new Action(SaveManager.Instance.LoadLeaderBoards));
			}
			ulong startupLobby = GlobalSettings.Instance.StartupLobby;
			if (startupLobby != 0UL)
			{
				Tome.Instance.ControllerA(null);
			}
		}

		// Token: 0x060023A5 RID: 9125 RVA: 0x00100D17 File Offset: 0x000FEF17
		public override void OnExit()
		{
			ToolTipMan.Instance.KillAll(true);
			ControlManager.Instance.ClearControllers();
		}

		// Token: 0x060023A6 RID: 9126 RVA: 0x00100D2E File Offset: 0x000FEF2E
		public void SetCurrentPosition(Vector3 iNewPosition, float iSpeed)
		{
			MenuState.mCameraSpeed = iSpeed;
			MenuState.mPosition = iNewPosition;
		}

		// Token: 0x060023A7 RID: 9127 RVA: 0x00100D3C File Offset: 0x000FEF3C
		private bool isApproximately(float a, float b, float precision)
		{
			return Math.Abs(a - b) <= precision;
		}

		// Token: 0x060023A8 RID: 9128 RVA: 0x00100D4C File Offset: 0x000FEF4C
		private bool isApproximately(Vector3 a, Vector3 b, float precision)
		{
			return this.isApproximately(a.X, b.X, precision) && this.isApproximately(a.Y, b.Y, precision) && this.isApproximately(a.Z, b.Z, precision);
		}

		// Token: 0x060023A9 RID: 9129 RVA: 0x00100DA4 File Offset: 0x000FEFA4
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			base.Update(iDataChannel, iDeltaTime);
			if (this.mFindGamepadsTimer > 0f)
			{
				this.mFindGamepadsTimer -= iDeltaTime;
				if (this.mFindGamepadsTimer <= 0f)
				{
					Game.Instance.AddLoadTask(new Action(this.FindNewControllers));
				}
			}
			Tome.Instance.Update(iDataChannel, iDeltaTime);
			GamePadMenuHelp.Instance.Update(iDataChannel, iDeltaTime);
			this.mScene.Camera.Update(iDataChannel, iDeltaTime);
			Matrix matrix;
			this.mScene.Camera.GetViewProjectionMatrix(iDataChannel, out matrix);
			DialogManager.Instance.Update(iDataChannel, iDeltaTime, ref matrix);
			ToolTipMan.Instance.Update(this.mScene, iDataChannel, iDeltaTime);
		}

		// Token: 0x060023AA RID: 9130 RVA: 0x00100E54 File Offset: 0x000FF054
		private void FindNewControllers()
		{
			ControlManager.Instance.FindNewGamePads();
			if (GamePadConfigMessageBox.Instance.Dead)
			{
				for (int i = 0; i < ControlManager.Instance.DInputPads.Count; i++)
				{
					if (!ControlManager.Instance.DInputPads[i].Configured)
					{
						GamePadConfigMessageBox.Instance.GamePad = ControlManager.Instance.DInputPads[i];
						GamePadConfigMessageBox.Instance.Show();
						break;
					}
				}
			}
			SubMenuOptionsControls.Instance.UpdateControllers();
			this.mFindGamepadsTimer = 5f;
		}

		// Token: 0x1700087D RID: 2173
		// (get) Token: 0x060023AB RID: 9131 RVA: 0x00100EE3 File Offset: 0x000FF0E3
		public bool TomeTakesInput
		{
			get
			{
				return !this.mStoreShowing && this.mStorePosition >= 1f;
			}
		}

		// Token: 0x1700087E RID: 2174
		// (get) Token: 0x060023AC RID: 9132 RVA: 0x00100EFF File Offset: 0x000FF0FF
		public bool StoreTakesInput
		{
			get
			{
				return this.mStoreShowing && this.mStorePosition <= 1E-06f;
			}
		}

		// Token: 0x060023AD RID: 9133 RVA: 0x00100F1C File Offset: 0x000FF11C
		internal void NetworkInput(ref MenuSelectMessage iMessage)
		{
			switch (iMessage.IntendedMenu)
			{
			case MenuSelectMessage.MenuType.CharacterSelect:
				SubMenuCharacterSelect.Instance.NetworkInput(ref iMessage);
				return;
			case MenuSelectMessage.MenuType.Statistics:
				SubMenuEndGame.Instance.NetworkInput(ref iMessage);
				return;
			default:
				throw new Exception("Invalid target menu!");
			}
		}

		// Token: 0x040026B3 RID: 9907
		private static readonly int MUSIC_MENU = "music_menu".GetHashCodeCustom();

		// Token: 0x040026B4 RID: 9908
		private static readonly int MENU_AMBIENCE = "amb_library_menu".GetHashCodeCustom();

		// Token: 0x040026B5 RID: 9909
		private static MenuState mSingelton;

		// Token: 0x040026B6 RID: 9910
		private static volatile object mSingeltonLock = new object();

		// Token: 0x040026B7 RID: 9911
		private static readonly Vector3 CAMERA_STARTPOS = new Vector3(-1f, 0f, 0f);

		// Token: 0x040026B8 RID: 9912
		private static float mCameraSpeed;

		// Token: 0x040026B9 RID: 9913
		private static Vector3 mPosition;

		// Token: 0x040026BA RID: 9914
		private bool mStoreShowing;

		// Token: 0x040026BB RID: 9915
		private float mStorePosition = 1.25f;

		// Token: 0x040026BC RID: 9916
		private float mFindGamepadsTimer = 1f;
	}
}
