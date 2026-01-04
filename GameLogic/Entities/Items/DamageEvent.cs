// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Items.DamageEvent
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

#nullable disable
namespace Magicka.GameLogic.Entities.Items;

public struct DamageEvent
{
  public Damage Damage;
  public bool VelocityBased;

  public DamageEvent(AttackProperties iAProp, Elements iElements, float iAmount, float iMagnitude)
    : this(iAProp, iElements, iAmount, iMagnitude, false)
  {
  }

  public DamageEvent(
    AttackProperties iAProp,
    Elements iElements,
    float iAmount,
    float iMagnitude,
    bool iVelocityBased)
  {
    this.Damage = new Damage()
    {
      AttackProperty = iAProp,
      Element = iElements,
      Amount = iAmount,
      Magnitude = iMagnitude
    };
    this.VelocityBased = iVelocityBased;
  }

  public DamageEvent(Damage iDamage)
    : this(iDamage, false)
  {
  }

  public DamageEvent(Damage iDamage, bool iVelocityBased)
  {
    this.VelocityBased = iVelocityBased;
    this.Damage = iDamage;
  }

  public DamageEvent(ContentReader iInput)
  {
    Damage damage = new Damage();
    damage.AttackProperty = (AttackProperties) iInput.ReadInt32();
    damage.Element = (Elements) iInput.ReadInt32();
    damage.Amount = iInput.ReadSingle();
    damage.Magnitude = iInput.ReadSingle();
    this.VelocityBased = iInput.ReadBoolean();
    this.Damage = damage;
  }

  public DamageResult Execute(Entity iItem, Entity iTarget)
  {
    DamageResult damageResult = DamageResult.None;
    if (iTarget is IDamageable)
    {
      Damage damage = this.Damage;
      Vector3 result1 = iItem.Position;
      if (iItem is Item obj && obj.Owner != null && obj.IsGunClass && iTarget != null)
      {
        result1 = iTarget.Position;
        Vector3 result2 = iItem.Position;
        Vector3.Subtract(ref result2, ref result1, out result2);
        result2.Normalize();
        Vector3.Multiply(ref result2, iTarget.Radius, out result2);
        Vector3.Add(ref result1, ref result2, out result1);
      }
      Entity iAttacker;
      if (iItem is MissileEntity)
        iAttacker = (iItem as MissileEntity).Owner;
      else if (iItem is Item)
      {
        iAttacker = (Entity) (iItem as Item).Owner;
        if ((iItem as Item).Owner.MeleeBoosted)
          damage.Amount *= (iItem as Item).Owner.MeleeBoostAmount;
      }
      else
        iAttacker = iItem;
      if (this.VelocityBased)
      {
        float normalizedVelocity = (iItem as MissileEntity).NormalizedVelocity;
        damage.Amount *= normalizedVelocity;
      }
      damageResult |= (iTarget as IDamageable).Damage(damage, iAttacker, iAttacker.PlayState.PlayTime, result1);
    }
    return damageResult;
  }
}
