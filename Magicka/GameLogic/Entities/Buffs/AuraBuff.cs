using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Buffs
{
	// Token: 0x020002FF RID: 767
	public struct AuraBuff
	{
		// Token: 0x0600179F RID: 6047 RVA: 0x0009B8E5 File Offset: 0x00099AE5
		public AuraBuff(BuffStorage iBuff)
		{
			this.Buff = iBuff;
		}

		// Token: 0x060017A0 RID: 6048 RVA: 0x0009B8EE File Offset: 0x00099AEE
		public AuraBuff(ContentReader iInput)
		{
			this.Buff = new BuffStorage(iInput);
		}

		// Token: 0x060017A1 RID: 6049 RVA: 0x0009B8FC File Offset: 0x00099AFC
		public float Execute(Character iOwner, AuraTarget iAuraTarget, int iEffect, float iRadius, int[] iTargetTypes, Factions TargetFactions)
		{
			if (iAuraTarget != AuraTarget.Self)
			{
				List<Entity> entities = iOwner.PlayState.EntityManager.GetEntities(iOwner.Position, iRadius, false, true);
				foreach (Entity entity in entities)
				{
					Character character = entity as Character;
					if (character != null && !character.IsEthereal)
					{
						switch (iAuraTarget)
						{
						case AuraTarget.Friendly:
							if ((character.Faction & iOwner.Faction) != Factions.NONE)
							{
								BuffStorage buff = this.Buff;
								if (buff.BuffType == BuffType.DealDamage)
								{
									buff.BuffDealDamage.mAuraOwnerHandle = (int)iOwner.Handle;
								}
								character.AddBuff(ref buff);
							}
							break;
						case AuraTarget.FriendlyButSelf:
							if (character != iOwner && (character.Faction & iOwner.Faction) != Factions.NONE)
							{
								BuffStorage buff2 = this.Buff;
								if (buff2.BuffType == BuffType.DealDamage)
								{
									buff2.BuffDealDamage.mAuraOwnerHandle = (int)iOwner.Handle;
								}
								character.AddBuff(ref buff2);
							}
							break;
						case AuraTarget.Enemy:
							if ((character.Faction & iOwner.Faction) == Factions.NONE)
							{
								BuffStorage buff3 = this.Buff;
								if (buff3.BuffType == BuffType.DealDamage)
								{
									buff3.BuffDealDamage.mAuraOwnerHandle = (int)iOwner.Handle;
								}
								character.AddBuff(ref buff3);
							}
							break;
						case AuraTarget.All:
						{
							BuffStorage buff4 = this.Buff;
							if (buff4.BuffType == BuffType.DealDamage)
							{
								buff4.BuffDealDamage.mAuraOwnerHandle = (int)iOwner.Handle;
							}
							buff4.SelfCasted = (character == iOwner);
							character.AddBuff(ref buff4);
							break;
						}
						case AuraTarget.AllButSelf:
							if (character != iOwner)
							{
								BuffStorage buff5 = this.Buff;
								if (buff5.BuffType == BuffType.DealDamage)
								{
									buff5.BuffDealDamage.mAuraOwnerHandle = (int)iOwner.Handle;
								}
								character.AddBuff(ref buff5);
							}
							break;
						case AuraTarget.Type:
							for (int i = 0; i < iTargetTypes.Length; i++)
							{
								if (character.Type == iTargetTypes[i])
								{
									BuffStorage buff6 = this.Buff;
									if (buff6.BuffType == BuffType.DealDamage)
									{
										buff6.BuffDealDamage.mAuraOwnerHandle = (int)iOwner.Handle;
									}
									character.AddBuff(ref buff6);
								}
							}
							break;
						case AuraTarget.TypeButSelf:
							if (character != iOwner)
							{
								for (int j = 0; j < iTargetTypes.Length; j++)
								{
									if (character.Type == iTargetTypes[j])
									{
										BuffStorage buff7 = this.Buff;
										if (buff7.BuffType == BuffType.DealDamage)
										{
											buff7.BuffDealDamage.mAuraOwnerHandle = (int)iOwner.Handle;
										}
										character.AddBuff(ref buff7);
									}
								}
							}
							break;
						case AuraTarget.Faction:
							if ((character.Faction & TargetFactions) != Factions.NONE)
							{
								BuffStorage buff8 = this.Buff;
								if (buff8.BuffType == BuffType.DealDamage)
								{
									buff8.BuffDealDamage.mAuraOwnerHandle = (int)iOwner.Handle;
								}
								character.AddBuff(ref buff8);
							}
							break;
						case AuraTarget.FactionButSelf:
							if (character != iOwner && (character.Faction & TargetFactions) != Factions.NONE)
							{
								BuffStorage buff9 = this.Buff;
								if (buff9.BuffType == BuffType.DealDamage)
								{
									buff9.BuffDealDamage.mAuraOwnerHandle = (int)iOwner.Handle;
								}
								character.AddBuff(ref buff9);
							}
							break;
						}
					}
				}
				iOwner.PlayState.EntityManager.ReturnEntityList(entities);
			}
			else
			{
				BuffStorage buff10 = this.Buff;
				if (buff10.BuffType == BuffType.DealDamage)
				{
					buff10.BuffDealDamage.mAuraOwnerHandle = (int)iOwner.Handle;
				}
				iOwner.AddBuff(ref buff10);
			}
			return 1f;
		}

		// Token: 0x04001963 RID: 6499
		public BuffStorage Buff;
	}
}
