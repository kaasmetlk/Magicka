// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Spells.MagickTree
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.GameLogic.Spells;

internal class MagickTree
{
  private MagickNode root;
  private MagickNode currentNode;

  public MagickTree() => this.root = new MagickNode((MagickNode) null);

  public void GoToRoot() => this.currentNode = this.root;

  public bool Move(Elements iElement, out MagickType oMagick)
  {
    MagickNode child = this.currentNode.GetChild(iElement);
    oMagick = MagickType.None;
    if (child == null)
    {
      oMagick = this.currentNode.Content;
      this.GoToRoot();
      return false;
    }
    this.currentNode = child;
    oMagick = this.currentNode.Content;
    return true;
  }

  public MagickNode MoveAndAdd(Elements iElement)
  {
    MagickNode iChild = this.currentNode.GetChild(iElement);
    if (iChild == null)
    {
      iChild = new MagickNode(this.currentNode);
      this.currentNode.SetChild(iElement, iChild);
    }
    this.currentNode = iChild;
    return this.currentNode;
  }
}
