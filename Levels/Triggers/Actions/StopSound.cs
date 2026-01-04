// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.StopSound
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class StopSound(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private string mIDStr;
  private int mID;
  private bool mInstant;

  protected override void Execute() => this.GameScene.StopSound(this.mID, this.mInstant);

  public override void QuickExecute()
  {
  }

  public string ID
  {
    get => this.mIDStr;
    set
    {
      this.mIDStr = value;
      this.mID = this.mIDStr.GetHashCodeCustom();
    }
  }

  public bool Instant
  {
    get => this.mInstant;
    set => this.mInstant = value;
  }
}
