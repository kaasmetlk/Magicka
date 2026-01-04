// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.Counter
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class Counter(Trigger iTrigger, GameScene iScene) : Magicka.Levels.Triggers.Actions.Action(iTrigger, iScene)
{
  private string mName;
  private int mID;
  private CounterAction mAction;
  private int mValue;

  protected override void Execute()
  {
    switch (this.mAction)
    {
      case CounterAction.INC:
        this.GameScene.Level.AddToCounter(this.mID, this.mValue);
        break;
      case CounterAction.DEC:
        this.GameScene.Level.AddToCounter(this.mID, -this.mValue);
        break;
      case CounterAction.SET:
        this.GameScene.Level.SetCounterValue(this.mID, this.mValue);
        break;
    }
  }

  public override void QuickExecute()
  {
  }

  public string Name
  {
    get => this.mName;
    set
    {
      this.mName = value;
      this.mID = this.mName.GetHashCodeCustom();
    }
  }

  public CounterAction Action
  {
    get => this.mAction;
    set => this.mAction = value;
  }

  public int Value
  {
    get => this.mValue;
    set => this.mValue = value;
  }
}
