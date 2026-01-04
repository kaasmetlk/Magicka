// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Spells.SpellTree
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.GameLogic.Spells;

internal class SpellTree
{
  private SpellNode root;
  private SpellNode currentNode;

  public SpellTree() => this.root = new SpellNode((SpellNode) null);

  public void GoToRoot() => this.currentNode = this.root;

  public bool Move(ControllerDirection iDirection, out Spell oSpell)
  {
    SpellNode child = this.currentNode.GetChild(iDirection);
    oSpell = new Spell();
    if (child == null)
    {
      oSpell = this.currentNode.Content;
      this.GoToRoot();
      return false;
    }
    this.currentNode = child;
    oSpell = this.currentNode.Content;
    return true;
  }

  public SpellNode MoveAndAdd(ControllerDirection iDirection)
  {
    SpellNode iChild = this.currentNode.GetChild(iDirection);
    if (iChild == null)
    {
      iChild = new SpellNode(this.currentNode);
      this.currentNode.SetChild(iDirection, iChild);
    }
    this.currentNode = iChild;
    return this.currentNode;
  }
}
