using System;
using System.Collections.Generic;
using Microsoft.DirectX.DirectInput;
using Microsoft.Xna.Framework;
using PolygonHead;
using XInput;

namespace Magicka.GameLogic.Controls
{
	// Token: 0x02000616 RID: 1558
	internal class ControlManager
	{
		// Token: 0x17000AF7 RID: 2807
		// (get) Token: 0x06002EAD RID: 11949 RVA: 0x0017ADD4 File Offset: 0x00178FD4
		public static ControlManager Instance
		{
			get
			{
				if (ControlManager.mSingelton == null)
				{
					lock (ControlManager.mSingeltonLock)
					{
						if (ControlManager.mSingelton == null)
						{
							ControlManager.mSingelton = new ControlManager();
						}
					}
				}
				return ControlManager.mSingelton;
			}
		}

		// Token: 0x17000AF8 RID: 2808
		// (get) Token: 0x06002EAE RID: 11950 RVA: 0x0017AE28 File Offset: 0x00179028
		public KeyboardMouseController MenuController
		{
			get
			{
				return this.mMenuController;
			}
		}

		// Token: 0x17000AF9 RID: 2809
		// (get) Token: 0x06002EAF RID: 11951 RVA: 0x0017AE30 File Offset: 0x00179030
		public List<DirectInputController> DInputPads
		{
			get
			{
				return this.mDInputPads;
			}
		}

		// Token: 0x17000AFA RID: 2810
		// (get) Token: 0x06002EB0 RID: 11952 RVA: 0x0017AE38 File Offset: 0x00179038
		public List<XInputController> XInputPads
		{
			get
			{
				return this.mXInputPads;
			}
		}

		// Token: 0x06002EB1 RID: 11953 RVA: 0x0017AE40 File Offset: 0x00179040
		private ControlManager()
		{
			this.mInputMessageFilter = new InputMessageFilter();
			this.mInputMessageFilter.TranslateMessage = true;
			this.mMenuController = new KeyboardMouseController(this.mInputMessageFilter);
			this.mXInputPads.Add(new XInputController(PlayerIndex.One));
			this.mXInputPads.Add(new XInputController(PlayerIndex.Two));
			this.mXInputPads.Add(new XInputController(PlayerIndex.Three));
			this.mXInputPads.Add(new XInputController(PlayerIndex.Four));
		}

		// Token: 0x06002EB2 RID: 11954 RVA: 0x0017AEEC File Offset: 0x001790EC
		public unsafe void FindNewGamePads()
		{
			DeviceList devices = Manager.GetDevices(DeviceType.Gamepad, EnumDevicesFlags.AttachedOnly);
			foreach (object obj in devices)
			{
				DeviceInstance deviceInstance = (DeviceInstance)obj;
				Guid productGuid = deviceInstance.ProductGuid;
				GUID* pGuidProductFromDirectInput = (GUID*)(&productGuid);
				if (!XInputHelper.IsXInputDevice((GUID*)pGuidProductFromDirectInput))
				{
					bool flag = false;
					for (int i = 0; i < this.mDInputPads.Count; i++)
					{
						if (this.mDInputPads[i].Device.DeviceInformation.InstanceGuid == deviceInstance.InstanceGuid)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						DirectInputController item = new DirectInputController(deviceInstance.InstanceGuid);
						this.mDInputPads.Add(item);
					}
				}
			}
			devices = Manager.GetDevices(DeviceType.Joystick, EnumDevicesFlags.AttachedOnly);
			foreach (object obj2 in devices)
			{
				DeviceInstance deviceInstance2 = (DeviceInstance)obj2;
				Guid productGuid2 = deviceInstance2.ProductGuid;
				GUID* pGuidProductFromDirectInput2 = (GUID*)(&productGuid2);
				if (!XInputHelper.IsXInputDevice((GUID*)pGuidProductFromDirectInput2))
				{
					bool flag2 = false;
					for (int j = 0; j < this.mDInputPads.Count; j++)
					{
						if (this.mDInputPads[j].Device.DeviceInformation.InstanceGuid == deviceInstance2.InstanceGuid)
						{
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						DirectInputController item2 = new DirectInputController(deviceInstance2.InstanceGuid);
						this.mDInputPads.Add(item2);
					}
				}
			}
		}

		// Token: 0x06002EB3 RID: 11955 RVA: 0x0017B0AC File Offset: 0x001792AC
		public void ClearControllers()
		{
			for (int i = 0; i < this.mXInputPads.Count; i++)
			{
				this.mXInputPads[i].Clear();
			}
			for (int j = 0; j < this.mDInputPads.Count; j++)
			{
				this.mDInputPads[j].Clear();
			}
			this.mMenuController.Clear();
			this.mLimitInput.Clear();
		}

		// Token: 0x06002EB4 RID: 11956 RVA: 0x0017B120 File Offset: 0x00179320
		public void HandleInput(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mLimitInputCooldown -= iDeltaTime;
			for (int i = 0; i < this.mXInputPads.Count; i++)
			{
				this.mXInputPads[i].Update(iDataChannel, iDeltaTime);
			}
			for (int j = 0; j < this.mDInputPads.Count; j++)
			{
				this.mDInputPads[j].Update(iDataChannel, iDeltaTime);
			}
			this.mMenuController.Update(iDataChannel, iDeltaTime);
		}

		// Token: 0x06002EB5 RID: 11957 RVA: 0x0017B19A File Offset: 0x0017939A
		public void LimitInput(object iLocker)
		{
			if (!this.mLimitInput.Contains(iLocker))
			{
				this.mLimitInput.Add(iLocker);
			}
			this.mLimitInputCooldown = 0.1f;
		}

		// Token: 0x06002EB6 RID: 11958 RVA: 0x0017B1C1 File Offset: 0x001793C1
		public void UnlimitInput(object iLocker)
		{
			if (this.mLimitInput.Remove(iLocker))
			{
				this.mLimitInputCooldown = 0.1f;
			}
		}

		// Token: 0x17000AFB RID: 2811
		// (get) Token: 0x06002EB7 RID: 11959 RVA: 0x0017B1DC File Offset: 0x001793DC
		public bool IsInputLimited
		{
			get
			{
				return this.mLimitInput.Count > 0 || this.mLimitInputCooldown > 0f;
			}
		}

		// Token: 0x06002EB8 RID: 11960 RVA: 0x0017B1FB File Offset: 0x001793FB
		public void UnlimitInput()
		{
			this.mLimitInput.Clear();
			this.mLimitInputCooldown = 0.1f;
		}

		// Token: 0x06002EB9 RID: 11961 RVA: 0x0017B213 File Offset: 0x00179413
		public void LockPlayerInput(int iPlayerIndex)
		{
			this.mPlayerInputLocked[iPlayerIndex] = true;
		}

		// Token: 0x06002EBA RID: 11962 RVA: 0x0017B21E File Offset: 0x0017941E
		public bool IsPlayerInputLocked(int iPlayerIndex)
		{
			return this.mPlayerInputLocked[iPlayerIndex];
		}

		// Token: 0x06002EBB RID: 11963 RVA: 0x0017B228 File Offset: 0x00179428
		public void UnlockPlayerInput(int iPlayerIndex)
		{
			this.mPlayerInputLocked[iPlayerIndex] = false;
		}

		// Token: 0x06002EBC RID: 11964 RVA: 0x0017B233 File Offset: 0x00179433
		public void LockPlayerInput(Controller iSender)
		{
			this.mPlayerInputLocked[iSender.Player.ID] = true;
		}

		// Token: 0x06002EBD RID: 11965 RVA: 0x0017B248 File Offset: 0x00179448
		public bool IsPlayerInputLocked(Controller iSender)
		{
			return this.mPlayerInputLocked[iSender.Player.ID];
		}

		// Token: 0x06002EBE RID: 11966 RVA: 0x0017B25C File Offset: 0x0017945C
		public void UnlockPlayerInput(Controller iSender)
		{
			this.mPlayerInputLocked[iSender.Player.ID] = false;
		}

		// Token: 0x17000AFC RID: 2812
		// (get) Token: 0x06002EBF RID: 11967 RVA: 0x0017B274 File Offset: 0x00179474
		public int GamePadCount
		{
			get
			{
				int num = 0;
				for (int i = 0; i < this.mDInputPads.Count; i++)
				{
					if (this.mDInputPads[i].IsConnected && this.mDInputPads[i].Configured)
					{
						num++;
					}
				}
				for (int j = 0; j < this.mXInputPads.Count; j++)
				{
					if (this.mXInputPads[j].IsConnected)
					{
						num++;
					}
				}
				return num;
			}
		}

		// Token: 0x06002EC0 RID: 11968 RVA: 0x0017B2F4 File Offset: 0x001794F4
		public void UnlockPlayerInput()
		{
			for (int i = 0; i < 4; i++)
			{
				this.mPlayerInputLocked[i] = false;
			}
		}

		// Token: 0x040032C9 RID: 13001
		private static ControlManager mSingelton;

		// Token: 0x040032CA RID: 13002
		private static volatile object mSingeltonLock = new object();

		// Token: 0x040032CB RID: 13003
		private List<object> mLimitInput = new List<object>();

		// Token: 0x040032CC RID: 13004
		private List<XInputController> mXInputPads = new List<XInputController>();

		// Token: 0x040032CD RID: 13005
		private List<DirectInputController> mDInputPads = new List<DirectInputController>();

		// Token: 0x040032CE RID: 13006
		private float mLimitInputCooldown;

		// Token: 0x040032CF RID: 13007
		private InputMessageFilter mInputMessageFilter;

		// Token: 0x040032D0 RID: 13008
		private bool[] mPlayerInputLocked = new bool[4];

		// Token: 0x040032D1 RID: 13009
		private KeyboardMouseController mMenuController;
	}
}
