// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.FoundSecretArea
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class FoundSecretArea(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private string mID;

  protected override void Execute()
  {
    Profile.Instance.FoundSecretArea(this.GameScene.PlayState, this.mID);
  }

  public override void QuickExecute()
  {
  }

  public string ID
  {
    get => this.mID;
    set => this.mID = value;
  }
}
