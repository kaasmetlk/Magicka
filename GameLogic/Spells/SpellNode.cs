// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Spells.SpellNode
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System;

#nullable disable
namespace Magicka.GameLogic.Spells;

internal class SpellNode
{
  private WeakReference parent;
  private SpellNode centerChild;
  private SpellNode rightChild;
  private SpellNode upChild;
  private SpellNode leftChild;
  private SpellNode downChild;
  private Spell spell;

  public SpellNode(SpellNode parent) => this.parent = new WeakReference((object) parent);

  public Spell Content
  {
    set => this.spell = value;
    get => this.spell;
  }

  public SpellNode Parent => this.parent.Target as SpellNode;

  public SpellNode GetChild(ControllerDirection iDirection)
  {
    switch (iDirection)
    {
      case ControllerDirection.Center:
        return this.centerChild;
      case ControllerDirection.Right:
        return this.rightChild;
      case ControllerDirection.Up:
        return this.upChild;
      case ControllerDirection.Left:
        return this.leftChild;
      case ControllerDirection.Down:
        return this.downChild;
      default:
        return (SpellNode) null;
    }
  }

  public void SetChild(ControllerDirection iDirection, SpellNode iChild)
  {
    switch (iDirection)
    {
      case ControllerDirection.Center:
        this.centerChild = iChild;
        break;
      case ControllerDirection.Right:
        this.rightChild = iChild;
        break;
      case ControllerDirection.Up:
        this.upChild = iChild;
        break;
      case ControllerDirection.Left:
        this.leftChild = iChild;
        break;
      case ControllerDirection.Down:
        this.downChild = iChild;
        break;
    }
  }
}
