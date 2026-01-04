// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Items.SplashEvent
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Items;

public struct SplashEvent
{
  public float Radius;
  public Damage Damage;

  public SplashEvent(
    AttackProperties iAProp,
    Elements iElements,
    int iAmount,
    float iMagnitude,
    float iRadius)
  {
    this.Damage = new Damage()
    {
      AttackProperty = iAProp,
      Element = iElements,
      Amount = (float) iAmount,
      Magnitude = iMagnitude
    };
    this.Radius = iRadius;
  }

  public SplashEvent(Damage iDamage, float iRadius)
  {
    this.Damage = iDamage;
    this.Radius = iRadius;
  }

  public SplashEvent(ContentReader iInput)
  {
    this.Damage = new Damage()
    {
      AttackProperty = (AttackProperties) iInput.ReadInt32(),
      Element = (Elements) iInput.ReadInt32(),
      Amount = (float) iInput.ReadInt32(),
      Magnitude = iInput.ReadSingle()
    };
    this.Radius = iInput.ReadSingle();
    if ((double) this.Radius <= 1.4012984643248171E-45)
      throw new Exception("WTH");
  }

  public DamageResult Execute(Entity iItem, Entity iTarget, ref Vector3? iPosition)
  {
    DamageResult damageResult1 = DamageResult.None;
    Vector3 position = iItem.Position;
    if (iPosition.HasValue)
      position = iPosition.Value;
    DamageResult damageResult2;
    switch (iItem)
    {
      case MissileEntity _:
        Entity owner1 = (iItem as MissileEntity).Owner;
        damageResult2 = damageResult1 | Helper.CircleDamage(iItem.PlayState, owner1, iItem.PlayState.PlayTime, iItem, ref position, this.Radius, ref this.Damage);
        break;
      case Item _:
        Entity owner2 = (Entity) (iItem as Item).Owner;
        damageResult2 = damageResult1 | Helper.CircleDamage(iItem.PlayState, owner2, iItem.PlayState.PlayTime, owner2, ref position, this.Radius, ref this.Damage);
        break;
      default:
        damageResult2 = damageResult1 | Helper.CircleDamage(iItem.PlayState, (Entity) (iItem as Character), iItem.PlayState.PlayTime, iItem, ref position, this.Radius, ref this.Damage);
        break;
    }
    return damageResult2;
  }
}
