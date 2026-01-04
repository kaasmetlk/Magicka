// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.EndCutscene
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System.Xml;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class EndCutscene(Trigger iTrigger, GameScene iScene, XmlNode iNode) : Action(iTrigger, iScene)
{
  private bool mSkipBarMove;

  protected override void Execute() => this.mScene.PlayState.EndCutscene(this.mSkipBarMove);

  public override void QuickExecute() => this.mScene.PlayState.EndCutscene(true);

  public bool SkipBarMove
  {
    get => this.mSkipBarMove;
    set => this.mSkipBarMove = value;
  }
}
