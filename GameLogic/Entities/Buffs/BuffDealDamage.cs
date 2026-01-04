// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Buffs.BuffDealDamage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework.Content;

#nullable disable
namespace Magicka.GameLogic.Entities.Buffs;

public struct BuffDealDamage
{
  private Damage Damage;
  private float mBuffIntervall;
  private float mInternalIntervall;
  public int mAuraOwnerHandle;

  public BuffDealDamage(Damage iDamage)
  {
    this.Damage = iDamage;
    this.mBuffIntervall = 1f;
    this.mInternalIntervall = 1f;
    this.mAuraOwnerHandle = (int) ushort.MaxValue;
  }

  public BuffDealDamage(ContentReader iInput)
  {
    this.Damage = new Damage();
    this.Damage.AttackProperty = (AttackProperties) iInput.ReadInt32();
    this.Damage.Element = (Elements) iInput.ReadInt32();
    this.Damage.Amount = iInput.ReadSingle();
    this.Damage.Magnitude = iInput.ReadSingle();
    this.mBuffIntervall = 1f;
    this.mInternalIntervall = 1f;
    this.mAuraOwnerHandle = (int) ushort.MaxValue;
  }

  public float Execute(Character iOwner, float iDeltaTime)
  {
    this.mInternalIntervall -= iDeltaTime;
    if ((double) this.mInternalIntervall <= 0.0)
    {
      if (this.mAuraOwnerHandle != (int) ushort.MaxValue)
      {
        Character fromHandle = (Character) Entity.GetFromHandle(this.mAuraOwnerHandle);
        int num = (int) iOwner.Damage(this.Damage, (Entity) fromHandle, iOwner.PlayState.PlayTime, iOwner.Position);
      }
      else
      {
        int num1 = (int) iOwner.Damage(this.Damage, (Entity) iOwner, iOwner.PlayState.PlayTime, iOwner.Position);
      }
      this.mInternalIntervall = this.mBuffIntervall;
    }
    return 1f;
  }
}
