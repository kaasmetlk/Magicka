// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.EndGame
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class EndGame(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private EndGameCondition mType;
  private bool mFreezeGame = true;
  private bool mPhony;

  protected override void Execute()
  {
    this.GameScene.PlayState.Endgame(this.mType, this.mFreezeGame, this.mPhony, 0.0f);
  }

  public override void QuickExecute()
  {
  }

  public EndGameCondition Type
  {
    get => this.mType;
    set => this.mType = value;
  }

  public bool FreezeGame
  {
    get => this.mFreezeGame;
    set => this.mFreezeGame = value;
  }

  public bool Phony
  {
    get => this.mPhony;
    set => this.mPhony = value;
  }
}
