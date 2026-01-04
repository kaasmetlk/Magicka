// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.Paradox.AccountSequence.SteamUnlinkSequence
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.CoreFramework;
using Magicka.Misc;
using Magicka.Storage;

#nullable disable
namespace Magicka.WebTools.Paradox.AccountSequence;

public class SteamUnlinkSequence(
  ParadoxAccount iAccount,
  ParadoxAccountSequence.ExecutionDoneDelegate iCallback) : ParadoxAccountSequence(iAccount, iCallback)
{
  private const Logger.Source LOGGER_SOURCE = Logger.Source.ParadoxAccountSteamUnlink;

  protected override void OnExecute()
  {
    if (this.Account.IsLoggedFull)
    {
      if (this.Account.IsLinkedToSteam)
        this.RequestAccountDisconnectAccountSteam();
      else
        this.ExitFailure(ParadoxAccount.ErrorCode.SteamUnlink_NotLinkedToSteam);
    }
    else
      this.ExitFailure(ParadoxAccount.ErrorCode.SteamUnlink_NotAuthenticated);
  }

  private void RequestAccountDisconnectAccountSteam()
  {
    if (!this.CheckPhase(new ParadoxAccountSequence.SequencePhase[1]))
      return;
    this.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountDisconnectAccountSteam);
    Singleton<ParadoxServices>.Instance.AccountDisconnectAccountSteam(new ParadoxServices.AccountDisconnectAccountSteamDelegate(this.AccountDisconnectAccountSteamCallback), new ParadoxServices.HandledExceptionDelegate(this.AccountDisconnectAccountSteamFailedCallback));
  }

  private void AccountDisconnectAccountSteamCallback(bool iSuccess)
  {
    if (iSuccess)
    {
      Singleton<ParadoxAccountSaveData>.Instance.SetAuthToken(Singleton<ParadoxServices>.Instance.RetrieveAuthToken());
      this.Account.IsLinkedToSteam = false;
      this.ExitSuccess();
    }
    else
      this.ExitFailure(ParadoxAccount.ErrorCode.SteamUnlink_UnlinkFailed);
  }

  private void AccountDisconnectAccountSteamFailedCallback(string iReason)
  {
    Logger.LogError(Logger.Source.ParadoxAccountSteamUnlink, "AccountDisconnectAccountSteam failed : " + iReason);
    this.ExitFailure(ParadoxAccount.ErrorCode.SteamUnlink_UnlinkFailed);
  }
}
