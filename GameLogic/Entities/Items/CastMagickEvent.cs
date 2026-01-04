// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Items.CastMagickEvent
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework.Content;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Items;

public struct CastMagickEvent
{
  public MagickType MagickType;
  public Elements CombinedElements;

  public CastMagickEvent(ContentReader iInput)
  {
    this.MagickType = (MagickType) Enum.Parse(typeof (MagickType), iInput.ReadString(), true);
    Elements[] elementsArray = new Elements[iInput.ReadInt32()];
    this.CombinedElements = Elements.None;
    if (elementsArray.Length <= 0)
      return;
    try
    {
      for (int index = 0; index < elementsArray.Length; ++index)
      {
        elementsArray[index] = (Elements) iInput.ReadInt32();
        this.CombinedElements |= elementsArray[index];
      }
    }
    catch (Exception ex)
    {
    }
  }

  public CastMagickEvent(MagickType iType)
  {
    this.MagickType = iType;
    this.CombinedElements = Elements.None;
  }

  public CastMagickEvent(MagickType iType, Elements elements)
  {
    this.MagickType = iType;
    this.CombinedElements = elements;
  }

  public void Execute(Entity iItem, Entity iTarget)
  {
    Magick magick = new Magick();
    magick.MagickType = this.MagickType;
    magick.Element = this.CombinedElements;
    ISpellCaster iOwner = (ISpellCaster) null;
    if (iItem is Item)
      iOwner = (ISpellCaster) (iItem as Item).Owner;
    else if (iItem is MissileEntity)
      iOwner = (iItem as MissileEntity).Owner as ISpellCaster;
    if (iOwner != null)
    {
      if (magick.Effect is ITargetAbility)
        (magick.Effect as ITargetAbility).Execute(iOwner, iTarget, iItem.PlayState);
      else if (magick.Element != Elements.None)
        magick.Effect.Execute(iItem as ISpellCaster, magick.Element, iItem.PlayState);
      else
        magick.Effect.Execute(iOwner, iItem.PlayState);
    }
    else if (iItem is ISpellCaster)
    {
      if (magick.Element == Elements.None)
        magick.Effect.Execute(iItem as ISpellCaster, iItem.PlayState);
      else
        magick.Effect.Execute(iItem as ISpellCaster, magick.Element, iItem.PlayState);
    }
    else
    {
      if (iItem == null)
        return;
      magick.Effect.Execute(iItem.Position, iItem.PlayState);
    }
  }
}
