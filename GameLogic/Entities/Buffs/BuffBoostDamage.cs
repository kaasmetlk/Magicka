// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Buffs.BuffBoostDamage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework.Content;

#nullable disable
namespace Magicka.GameLogic.Entities.Buffs;

public struct BuffBoostDamage
{
  public Damage Damage;
  public ushort mAuraOwnerHandle;

  public BuffBoostDamage(Damage iDamage)
  {
    this.Damage = iDamage;
    this.mAuraOwnerHandle = ushort.MaxValue;
  }

  public BuffBoostDamage(ContentReader iInput)
  {
    this.Damage = new Damage();
    this.Damage.AttackProperty = (AttackProperties) iInput.ReadInt32();
    this.Damage.Element = (Elements) iInput.ReadInt32();
    this.Damage.Amount = iInput.ReadSingle();
    this.Damage.Magnitude = iInput.ReadSingle();
    this.mAuraOwnerHandle = ushort.MaxValue;
  }

  public float Execute(Character iOwner) => 1f;
}
