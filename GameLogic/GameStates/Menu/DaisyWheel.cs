// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.DaisyWheel
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using SteamWrapper;
using System;
using System.Threading;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu;

internal static class DaisyWheel
{
  private const uint MAX_CHARS = 255 /*0xFF*/;
  private static bool mIsDisplaying = false;
  private static volatile object mLockObject = new object();
  private static Action<string> DaisyWheelTextRecived = (Action<string>) null;
  private static uint submittedTextLength;

  public static bool IsDisplaying
  {
    get => DaisyWheel.mIsDisplaying;
    private set => DaisyWheel.mIsDisplaying = value;
  }

  public static uint SubmittedTextLength
  {
    get => DaisyWheel.submittedTextLength;
    private set => DaisyWheel.submittedTextLength = value;
  }

  public static void SetActionToCallWhenComplete(Action<string> target)
  {
    lock (DaisyWheel.mLockObject)
      DaisyWheel.DaisyWheelTextRecived = target == null || DaisyWheel.DaisyWheelTextRecived == null ? target : throw new Exception("DaisyWheel::SetActionToCallWhenComplete() Error ! Must be nulled by previous caller first !");
  }

  private static void SteamAPI_GamepadTextInputDismissed(GamepadTextInputDismissed iRequest)
  {
    SteamAPI.GamepadTextInputDismissed -= new Action<GamepadTextInputDismissed>(DaisyWheel.SteamAPI_GamepadTextInputDismissed);
    lock (DaisyWheel.mLockObject)
    {
      DaisyWheel.submittedTextLength = iRequest.m_unSubmittedText;
      if (iRequest.m_bSubmitted)
      {
        string text = "";
        bool flag;
        try
        {
          flag = SteamUtils.GetEnteredGamepadTextInput(out text, (uint) byte.MaxValue);
        }
        catch (Exception ex)
        {
          flag = false;
        }
        if (flag)
        {
          if (string.IsNullOrEmpty(text))
            text = "";
          if (DaisyWheel.DaisyWheelTextRecived != null)
            DaisyWheel.DaisyWheelTextRecived(text);
        }
      }
      else if (DaisyWheel.DaisyWheelTextRecived != null)
        DaisyWheel.DaisyWheelTextRecived("");
    }
    Thread.Sleep(500);
    ControlManager.Instance.ClearControllers();
    Thread.Sleep(0);
    DaisyWheel.mIsDisplaying = false;
  }

  public static bool TryShow(Controller iSender, string iDescr)
  {
    return DaisyWheel.TryShow(iSender, iDescr, false);
  }

  public static bool TryShow(Controller iSender, string iDescr, bool isPassword)
  {
    return DaisyWheel.TryShow(iSender, iDescr, isPassword, GamepadTextInputLineMode.GamepadTextInputLineModeSingleLine, 15U);
  }

  public static bool TryShow(
    Controller iSender,
    string iDescr,
    bool isPassword,
    GamepadTextInputLineMode iLineMode,
    uint iMaxChars)
  {
    if (DaisyWheel.mIsDisplaying)
      return false;
    DaisyWheel.submittedTextLength = 0U;
    Thread.Sleep(0);
    ControlManager.Instance.ClearControllers();
    Thread.Sleep(500);
    lock (DaisyWheel.mLockObject)
    {
      if (iMaxChars > (uint) byte.MaxValue)
        iMaxChars = (uint) byte.MaxValue;
      if (iSender != null && iSender is XInputController)
      {
        if (iDescr == null)
          iDescr = "";
        SteamAPI.GamepadTextInputDismissed += new Action<GamepadTextInputDismissed>(DaisyWheel.SteamAPI_GamepadTextInputDismissed);
        try
        {
          DaisyWheel.mIsDisplaying = SteamUtils.ShowGamepadTextInput(isPassword ? GamepadTextInputMode.GamepadTextInputModePassword : GamepadTextInputMode.GamepadTextInputModeNormal, GamepadTextInputLineMode.GamepadTextInputLineModeSingleLine, iDescr, iMaxChars);
        }
        catch (Exception ex)
        {
          DaisyWheel.mIsDisplaying = false;
          SteamAPI.GamepadTextInputDismissed -= new Action<GamepadTextInputDismissed>(DaisyWheel.SteamAPI_GamepadTextInputDismissed);
        }
        if (!DaisyWheel.IsDisplaying)
        {
          SteamAPI.GamepadTextInputDismissed -= new Action<GamepadTextInputDismissed>(DaisyWheel.SteamAPI_GamepadTextInputDismissed);
          DaisyWheel.DaisyWheelTextRecived((string) null);
        }
      }
      else
        DaisyWheel.mIsDisplaying = false;
    }
    return DaisyWheel.mIsDisplaying;
  }
}
