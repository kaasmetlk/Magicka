// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Items.BlastEvent
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Items;

public struct BlastEvent
{
  public float Radius;
  public DamageCollection5 Damage;

  public BlastEvent(float iRadius, DamageCollection5 iDamage)
  {
    this.Radius = iRadius;
    this.Damage = iDamage;
  }

  public BlastEvent(float iRadius, Magicka.GameLogic.Damage iDamage)
  {
    this.Radius = iRadius;
    this.Damage = new DamageCollection5();
    this.Damage.A = iDamage;
  }

  public BlastEvent(ContentReader iInput) => throw new NotImplementedException();

  public DamageResult Execute(Entity iItem, Entity iTarget, ref Vector3? iPosition)
  {
    Vector3 oldPosition = iItem.Body.OldPosition;
    if (iPosition.HasValue)
      oldPosition = iPosition.Value;
    DamageResult damageResult = DamageResult.None;
    Entity iOwner;
    switch (iItem)
    {
      case MissileEntity _:
        iOwner = (iItem as MissileEntity).Owner;
        break;
      case Item _:
        iOwner = (Entity) (iItem as Item).Owner;
        break;
      default:
        iOwner = iItem;
        break;
    }
    return damageResult | Blast.FullBlast(iItem.PlayState, iOwner, iOwner.PlayState.PlayTime, iItem, this.Radius, oldPosition, this.Damage);
  }
}
