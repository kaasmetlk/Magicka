// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.Paradox.AccountSequence.SteamLinkSequence
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.CoreFramework;
using Magicka.Misc;
using Magicka.Storage;

#nullable disable
namespace Magicka.WebTools.Paradox.AccountSequence;

public class SteamLinkSequence(
  ParadoxAccount iAccount,
  ParadoxAccountSequence.ExecutionDoneDelegate iCallback) : ParadoxAccountSequence(iAccount, iCallback)
{
  private const Logger.Source LOGGER_SOURCE = Logger.Source.ParadoxAccountSteamLink;
  private const string ALREADY_LINKED = "existing-steam-to-account-connection";

  protected override void OnExecute()
  {
    if (this.Account.IsLoggedFull)
    {
      if (this.Account.IsLinkedToSteam)
        this.ExitFailure(ParadoxAccount.ErrorCode.SteamLink_AlreadyLinkedToSteam);
      else
        this.RequestAccountConnectAccountSteam();
    }
    else
      this.ExitFailure(ParadoxAccount.ErrorCode.SteamLink_NotAuthenticated);
  }

  private void RequestAccountConnectAccountSteam()
  {
    if (!this.CheckPhase(new ParadoxAccountSequence.SequencePhase[1]))
      return;
    this.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountConnectAccountSteam);
    Singleton<ParadoxServices>.Instance.AccountConnectAccountSteam(InternalSteamUtils.GetSteamAppID().ToString(), InternalSteamUtils.GetSteamAuthToken(), new ParadoxServices.AccountConnectAccountSteamDelegate(this.AccountConnectAccountSteamCallback), new ParadoxServices.HandledExceptionDelegate(this.AccountConnectAccountSteamFailedCallback));
  }

  private void AccountConnectAccountSteamCallback(bool iSuccess)
  {
    if (iSuccess)
    {
      Singleton<ParadoxAccountSaveData>.Instance.ClearAuthToken();
      this.Account.IsLinkedToSteam = true;
      this.ExitSuccess();
    }
    else
      this.ExitFailure(ParadoxAccount.ErrorCode.SteamLink_LinkFailed);
  }

  private void AccountConnectAccountSteamFailedCallback(string iReason)
  {
    Logger.LogError(Logger.Source.ParadoxAccountSteamLink, "AccountConnectAccountSteam failed : " + iReason);
    if (iReason.Equals("existing-steam-to-account-connection"))
      this.ExitFailure(ParadoxAccount.ErrorCode.SteamLink_LinkAlreadyExistWithAnotherAccount);
    else
      this.ExitFailure(ParadoxAccount.ErrorCode.SteamLink_LinkFailed);
  }
}
