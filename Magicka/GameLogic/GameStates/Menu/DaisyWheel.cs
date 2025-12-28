using System;
using System.Threading;
using Magicka.GameLogic.Controls;
using SteamWrapper;

namespace Magicka.GameLogic.GameStates.Menu
{
	// Token: 0x020002EF RID: 751
	internal static class DaisyWheel
	{
		// Token: 0x170005DE RID: 1502
		// (get) Token: 0x06001716 RID: 5910 RVA: 0x00094B2E File Offset: 0x00092D2E
		// (set) Token: 0x06001717 RID: 5911 RVA: 0x00094B35 File Offset: 0x00092D35
		public static bool IsDisplaying
		{
			get
			{
				return DaisyWheel.mIsDisplaying;
			}
			private set
			{
				DaisyWheel.mIsDisplaying = value;
			}
		}

		// Token: 0x170005DF RID: 1503
		// (get) Token: 0x06001718 RID: 5912 RVA: 0x00094B3D File Offset: 0x00092D3D
		// (set) Token: 0x06001719 RID: 5913 RVA: 0x00094B44 File Offset: 0x00092D44
		public static uint SubmittedTextLength
		{
			get
			{
				return DaisyWheel.submittedTextLength;
			}
			private set
			{
				DaisyWheel.submittedTextLength = value;
			}
		}

		// Token: 0x0600171A RID: 5914 RVA: 0x00094B4C File Offset: 0x00092D4C
		public static void SetActionToCallWhenComplete(Action<string> target)
		{
			lock (DaisyWheel.mLockObject)
			{
				if (target != null && DaisyWheel.DaisyWheelTextRecived != null)
				{
					throw new Exception("DaisyWheel::SetActionToCallWhenComplete() Error ! Must be nulled by previous caller first !");
				}
				DaisyWheel.DaisyWheelTextRecived = target;
			}
		}

		// Token: 0x0600171B RID: 5915 RVA: 0x00094B9C File Offset: 0x00092D9C
		private static void SteamAPI_GamepadTextInputDismissed(GamepadTextInputDismissed iRequest)
		{
			SteamAPI.GamepadTextInputDismissed -= DaisyWheel.SteamAPI_GamepadTextInputDismissed;
			lock (DaisyWheel.mLockObject)
			{
				DaisyWheel.submittedTextLength = iRequest.m_unSubmittedText;
				if (iRequest.m_bSubmitted)
				{
					string text = "";
					bool flag = false;
					try
					{
						flag = SteamUtils.GetEnteredGamepadTextInput(out text, 255U);
					}
					catch (Exception)
					{
						flag = false;
					}
					if (flag)
					{
						if (string.IsNullOrEmpty(text))
						{
							text = "";
						}
						if (DaisyWheel.DaisyWheelTextRecived != null)
						{
							DaisyWheel.DaisyWheelTextRecived(text);
						}
					}
				}
				else if (DaisyWheel.DaisyWheelTextRecived != null)
				{
					DaisyWheel.DaisyWheelTextRecived("");
				}
			}
			Thread.Sleep(500);
			ControlManager.Instance.ClearControllers();
			Thread.Sleep(0);
			DaisyWheel.mIsDisplaying = false;
		}

		// Token: 0x0600171C RID: 5916 RVA: 0x00094C7C File Offset: 0x00092E7C
		public static bool TryShow(Controller iSender, string iDescr)
		{
			return DaisyWheel.TryShow(iSender, iDescr, false);
		}

		// Token: 0x0600171D RID: 5917 RVA: 0x00094C86 File Offset: 0x00092E86
		public static bool TryShow(Controller iSender, string iDescr, bool isPassword)
		{
			return DaisyWheel.TryShow(iSender, iDescr, isPassword, GamepadTextInputLineMode.GamepadTextInputLineModeSingleLine, 15U);
		}

		// Token: 0x0600171E RID: 5918 RVA: 0x00094C94 File Offset: 0x00092E94
		public static bool TryShow(Controller iSender, string iDescr, bool isPassword, GamepadTextInputLineMode iLineMode, uint iMaxChars)
		{
			if (DaisyWheel.mIsDisplaying)
			{
				return false;
			}
			DaisyWheel.submittedTextLength = 0U;
			Thread.Sleep(0);
			ControlManager.Instance.ClearControllers();
			Thread.Sleep(500);
			lock (DaisyWheel.mLockObject)
			{
				if (iMaxChars > 255U)
				{
					iMaxChars = 255U;
				}
				if (iSender != null && iSender is XInputController)
				{
					if (iDescr == null)
					{
						iDescr = "";
					}
					SteamAPI.GamepadTextInputDismissed += DaisyWheel.SteamAPI_GamepadTextInputDismissed;
					try
					{
						DaisyWheel.mIsDisplaying = SteamUtils.ShowGamepadTextInput(isPassword ? GamepadTextInputMode.GamepadTextInputModePassword : GamepadTextInputMode.GamepadTextInputModeNormal, GamepadTextInputLineMode.GamepadTextInputLineModeSingleLine, iDescr, iMaxChars);
					}
					catch (Exception)
					{
						DaisyWheel.mIsDisplaying = false;
						SteamAPI.GamepadTextInputDismissed -= DaisyWheel.SteamAPI_GamepadTextInputDismissed;
					}
					if (!DaisyWheel.IsDisplaying)
					{
						SteamAPI.GamepadTextInputDismissed -= DaisyWheel.SteamAPI_GamepadTextInputDismissed;
						DaisyWheel.DaisyWheelTextRecived(null);
					}
				}
				else
				{
					DaisyWheel.mIsDisplaying = false;
				}
			}
			return DaisyWheel.mIsDisplaying;
		}

		// Token: 0x0400189E RID: 6302
		private const uint MAX_CHARS = 255U;

		// Token: 0x0400189F RID: 6303
		private static bool mIsDisplaying = false;

		// Token: 0x040018A0 RID: 6304
		private static volatile object mLockObject = new object();

		// Token: 0x040018A1 RID: 6305
		private static Action<string> DaisyWheelTextRecived = null;

		// Token: 0x040018A2 RID: 6306
		private static uint submittedTextLength;
	}
}
