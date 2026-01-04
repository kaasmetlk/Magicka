// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.AddTimer
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class AddTimer(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private string mName;
  private int mID;
  private float mValue;
  private bool mPlayedTimer;

  protected override void Execute()
  {
    this.GameScene.Level.AddTimer(this.mID, this.mPlayedTimer, this.mValue);
  }

  public override void QuickExecute() => this.Execute();

  public string Name
  {
    get => this.mName;
    set
    {
      this.mName = value;
      this.mID = this.mName.GetHashCodeCustom();
    }
  }

  public float Value
  {
    get => this.mValue;
    set => this.mValue = value;
  }

  public bool PlayedTimer
  {
    get => this.mPlayedTimer;
    set => this.mPlayedTimer = value;
  }
}
