// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.StartEffect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class StartEffect(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private string mName;
  private int mNameHash;

  protected override void Execute()
  {
    if (string.IsNullOrEmpty(this.mName))
      this.mNameHash = this.GameScene.GetGeneratedEffectID();
    this.GameScene.StartEffect(this.mNameHash);
  }

  public override void QuickExecute() => this.Execute();

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
