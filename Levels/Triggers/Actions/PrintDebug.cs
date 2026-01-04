// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.PrintDebug
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class PrintDebug(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private string mMessage = "";
  private bool mBreak;
  private string mCounterName = "";
  private int mCounterID;
  private string mEntityID;
  private int mEntityIDHash;
  private static readonly string PLAYER1 = "player1";
  private static readonly string PLAYER2 = "player2";
  private static readonly string PLAYER3 = "player3";
  private static readonly string PLAYER4 = "player4";

  public string EntityPos
  {
    get => this.mEntityID;
    set => this.mEntityID = value;
  }

  public bool Break
  {
    get => this.mBreak;
    set => this.mBreak = value;
  }

  public string CounterName
  {
    get => this.mCounterName;
    set => this.mCounterName = value;
  }

  public string Message
  {
    get => this.mMessage;
    set => this.mMessage = value;
  }

  public override void Initialize()
  {
  }

  public override void QuickExecute()
  {
  }

  private string CounterValue() => "";

  private string EntityPosition() => "";

  protected override void Execute()
  {
  }
}
