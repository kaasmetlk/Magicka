// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.EnablePoi
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class EnablePoi(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private string mID;
  private int mIDHash;

  protected override void Execute()
  {
    (this.GameScene.Triggers[this.mIDHash] as Interactable).Enabled = true;
  }

  public override void QuickExecute()
  {
    (this.GameScene.Triggers[this.mIDHash] as Interactable).Enabled = true;
  }

  public string ID
  {
    get => this.mID;
    set
    {
      this.mID = value;
      this.mIDHash = this.mID.GetHashCodeCustom();
    }
  }
}
