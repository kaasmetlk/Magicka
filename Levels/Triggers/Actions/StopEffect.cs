// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.StopEffect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class StopEffect(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private string mName;
  private int mNameHash;

  protected override void Execute() => this.GameScene.StopEffect(this.mNameHash);

  public override void QuickExecute() => this.GameScene.StopEffect(this.mNameHash);

  public string Name
  {
    get => this.mName;
    set
    {
      this.mName = value;
      this.mNameHash = this.mName.GetHashCodeCustom();
    }
  }
}
