// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.Timer
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class Timer(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private string mName;
  private int mID;
  private float? mValue;
  private bool? mPause;

  protected override void Execute()
  {
    if (this.mValue.HasValue)
      this.GameScene.Level.SetTimer(this.mID, this.mValue.Value);
    if (!this.mPause.HasValue)
      return;
    this.GameScene.Level.PauseTimer(this.mID, this.mPause.Value);
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
    get => this.mValue.Value;
    set => this.mValue = new float?(value);
  }

  public bool Paused
  {
    get => this.mPause.Value;
    set => this.mPause = new bool?(value);
  }
}
