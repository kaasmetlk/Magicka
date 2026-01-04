// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Buffs.AuraBuff
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Buffs;

public struct AuraBuff
{
  public BuffStorage Buff;

  public AuraBuff(BuffStorage iBuff) => this.Buff = iBuff;

  public AuraBuff(ContentReader iInput) => this.Buff = new BuffStorage(iInput);

  public float Execute(
    Character iOwner,
    AuraTarget iAuraTarget,
    int iEffect,
    float iRadius,
    int[] iTargetTypes,
    Factions TargetFactions)
  {
    if (iAuraTarget != AuraTarget.Self)
    {
      List<Entity> entities = iOwner.PlayState.EntityManager.GetEntities(iOwner.Position, iRadius, false, true);
      foreach (Entity entity in entities)
      {
        if (entity is Character character && !character.IsEthereal)
        {
          switch (iAuraTarget)
          {
            case AuraTarget.Friendly:
              if ((character.Faction & iOwner.Faction) != Factions.NONE)
              {
                BuffStorage buff = this.Buff;
                if (buff.BuffType == BuffType.DealDamage)
                  buff.BuffDealDamage.mAuraOwnerHandle = (int) iOwner.Handle;
                character.AddBuff(ref buff);
                continue;
              }
              continue;
            case AuraTarget.FriendlyButSelf:
              if (character != iOwner && (character.Faction & iOwner.Faction) != Factions.NONE)
              {
                BuffStorage buff = this.Buff;
                if (buff.BuffType == BuffType.DealDamage)
                  buff.BuffDealDamage.mAuraOwnerHandle = (int) iOwner.Handle;
                character.AddBuff(ref buff);
                continue;
              }
              continue;
            case AuraTarget.Enemy:
              if ((character.Faction & iOwner.Faction) == Factions.NONE)
              {
                BuffStorage buff = this.Buff;
                if (buff.BuffType == BuffType.DealDamage)
                  buff.BuffDealDamage.mAuraOwnerHandle = (int) iOwner.Handle;
                character.AddBuff(ref buff);
                continue;
              }
              continue;
            case AuraTarget.All:
              BuffStorage buff1 = this.Buff;
              if (buff1.BuffType == BuffType.DealDamage)
                buff1.BuffDealDamage.mAuraOwnerHandle = (int) iOwner.Handle;
              buff1.SelfCasted = character == iOwner;
              character.AddBuff(ref buff1);
              continue;
            case AuraTarget.AllButSelf:
              if (character != iOwner)
              {
                BuffStorage buff2 = this.Buff;
                if (buff2.BuffType == BuffType.DealDamage)
                  buff2.BuffDealDamage.mAuraOwnerHandle = (int) iOwner.Handle;
                character.AddBuff(ref buff2);
                continue;
              }
              continue;
            case AuraTarget.Type:
              for (int index = 0; index < iTargetTypes.Length; ++index)
              {
                if (character.Type == iTargetTypes[index])
                {
                  BuffStorage buff3 = this.Buff;
                  if (buff3.BuffType == BuffType.DealDamage)
                    buff3.BuffDealDamage.mAuraOwnerHandle = (int) iOwner.Handle;
                  character.AddBuff(ref buff3);
                }
              }
              continue;
            case AuraTarget.TypeButSelf:
              if (character != iOwner)
              {
                for (int index = 0; index < iTargetTypes.Length; ++index)
                {
                  if (character.Type == iTargetTypes[index])
                  {
                    BuffStorage buff4 = this.Buff;
                    if (buff4.BuffType == BuffType.DealDamage)
                      buff4.BuffDealDamage.mAuraOwnerHandle = (int) iOwner.Handle;
                    character.AddBuff(ref buff4);
                  }
                }
                continue;
              }
              continue;
            case AuraTarget.Faction:
              if ((character.Faction & TargetFactions) != Factions.NONE)
              {
                BuffStorage buff5 = this.Buff;
                if (buff5.BuffType == BuffType.DealDamage)
                  buff5.BuffDealDamage.mAuraOwnerHandle = (int) iOwner.Handle;
                character.AddBuff(ref buff5);
                continue;
              }
              continue;
            case AuraTarget.FactionButSelf:
              if (character != iOwner && (character.Faction & TargetFactions) != Factions.NONE)
              {
                BuffStorage buff6 = this.Buff;
                if (buff6.BuffType == BuffType.DealDamage)
                  buff6.BuffDealDamage.mAuraOwnerHandle = (int) iOwner.Handle;
                character.AddBuff(ref buff6);
                continue;
              }
              continue;
            default:
              continue;
          }
        }
      }
      iOwner.PlayState.EntityManager.ReturnEntityList(entities);
    }
    else
    {
      BuffStorage buff = this.Buff;
      if (buff.BuffType == BuffType.DealDamage)
        buff.BuffDealDamage.mAuraOwnerHandle = (int) iOwner.Handle;
      iOwner.AddBuff(ref buff);
    }
    return 1f;
  }
}
