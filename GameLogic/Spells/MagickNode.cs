// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Spells.MagickNode
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System;

#nullable disable
namespace Magicka.GameLogic.Spells;

public class MagickNode
{
  private WeakReference mParent;
  private MagickNode[] mChildren;
  private MagickType mMagickType;

  public MagickNode(MagickNode parent)
  {
    this.mParent = new WeakReference((object) parent);
    this.mChildren = new MagickNode[11];
  }

  public MagickType Content
  {
    set => this.mMagickType = value;
    get => this.mMagickType;
  }

  public MagickNode Parent => this.mParent.Target as MagickNode;

  public MagickNode GetChild(Elements iElement)
  {
    int index = Spell.ElementIndex(iElement);
    return index < 11 ? this.mChildren[index] : (MagickNode) null;
  }

  public void SetChild(Elements iElement, MagickNode iChild)
  {
    this.mChildren[Spell.ElementIndex(iElement)] = iChild;
  }
}
