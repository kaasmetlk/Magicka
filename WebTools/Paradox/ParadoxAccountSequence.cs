// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.Paradox.ParadoxAccountSequence
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.CoreFramework;

#nullable disable
namespace Magicka.WebTools.Paradox;

public abstract class ParadoxAccountSequence
{
  private const Logger.Source LOGGER_SOURCE = Logger.Source.ParadoxAccountSequence;
  private string EXCEPTION_WRONG_PHASE = "Wrong startup phase. Expected : [{0}]. Current : [{1}].";
  private readonly ParadoxAccount mAccount;
  private readonly ParadoxAccountSequence.ExecutionDoneDelegate mCallback;
  private ParadoxAccountSequence.SequencePhase mCurrentPhase;

  public bool Completed => this.mCurrentPhase == ParadoxAccountSequence.SequencePhase.Complete;

  protected ParadoxAccount Account => this.mAccount;

  private ParadoxAccountSequence()
  {
  }

  public ParadoxAccountSequence(
    ParadoxAccount iAccount,
    ParadoxAccountSequence.ExecutionDoneDelegate iCallback)
  {
    this.mAccount = iAccount;
    this.mCallback = iCallback;
  }

  public void Execute()
  {
    if (this.mCurrentPhase != ParadoxAccountSequence.SequencePhase.Enter)
      return;
    this.OnExecute();
  }

  protected abstract void OnExecute();

  protected void ExitSuccess()
  {
    this.ChangePhase(ParadoxAccountSequence.SequencePhase.Complete);
    if (this.mCallback == null)
      return;
    this.mCallback(true, ParadoxAccount.ErrorCode.None);
  }

  protected void ExitFailure(ParadoxAccount.ErrorCode iErrorCode)
  {
    this.ChangePhase(ParadoxAccountSequence.SequencePhase.Complete);
    if (this.mCallback == null)
      return;
    this.mCallback(false, iErrorCode);
  }

  protected bool CheckPhase(
    params ParadoxAccountSequence.SequencePhase[] iExpected)
  {
    return true;
  }

  protected void ChangePhase(ParadoxAccountSequence.SequencePhase iPhase)
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountSequence, $"Changed phase from [{this.mCurrentPhase.ToString()}] to [{iPhase.ToString()}]");
    this.mCurrentPhase = iPhase;
  }

  private void DisplayPhaseError(
    ParadoxAccountSequence.SequencePhase iCurrent,
    params ParadoxAccountSequence.SequencePhase[] iExpected)
  {
    string[] strArray = new string[iExpected.Length];
    for (int index = 0; index < iExpected.Length; ++index)
      strArray[index] = iExpected[index].ToString();
    Logger.LogError(Logger.Source.ParadoxAccountSequence, string.Format(this.EXCEPTION_WRONG_PHASE, (object) string.Join(", ", strArray), (object) iCurrent.ToString()));
  }

  public delegate void ExecutionDoneDelegate(bool iSuccess, ParadoxAccount.ErrorCode iErrorCode);

  protected enum SequencePhase
  {
    Enter,
    AccountCreate,
    AccountLogin,
    AccountLoginWithAuthToken,
    AccountLoginWithSteamTicket,
    AccountCreateShadow,
    AccountLoginShadow,
    AccountAddCredentials,
    AccountUpdateDetails,
    AccountGetDetails,
    AccountMergeShadow,
    AccountGetMergeStatus,
    AuthTokenInvalidate,
    AccountConnectAccountSteam,
    AccountDisconnectAccountSteam,
    AccountConnections,
    GameSparksRegister,
    GameSparksAuthenticate,
    Complete,
  }

  private enum ErrorCode
  {
    Default,
  }
}
