// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.RemoveHint
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Graphics;
using System.Xml;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class RemoveHint(Trigger iTrigger, GameScene iScene, XmlNode iNode) : Action(iTrigger, iScene)
{
  protected override void Execute() => TutorialManager.Instance.RemoveHint();

  public override void QuickExecute() => this.Execute();
}
