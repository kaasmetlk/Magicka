using System;
using Magicka.GameLogic.Entities;

namespace Magicka.Levels.Triggers
{
	// Token: 0x02000386 RID: 902
	public class AnyTriggerArea : TriggerArea
	{
		// Token: 0x06001B7E RID: 7038 RVA: 0x000BDDB7 File Offset: 0x000BBFB7
		public AnyTriggerArea() : base(null)
		{
		}

		// Token: 0x06001B7F RID: 7039 RVA: 0x000BDDC0 File Offset: 0x000BBFC0
		public void Count(EntityManager iEntityManager)
		{
			StaticList<Entity> entities = iEntityManager.Entities;
			int i = 0;
			while (i < entities.Count)
			{
				if (!(entities[i] is Avatar))
				{
					goto IL_32;
				}
				if (!(entities[i] as Avatar).IgnoreTriggers)
				{
					goto Block_2;
				}
				IL_12B:
				i++;
				continue;
				Block_2:
				try
				{
					IL_32:
					this.mPresentEntities.Add(entities[i]);
				}
				catch (StaticWeakListNeedsToExpandException)
				{
					this.mPresentEntities.Expand();
					this.mPresentEntities.Add(entities[i]);
				}
				this.mTotalEntities++;
				Character character = entities[i] as Character;
				if (character != null && (!character.Dead || character.Template.Undying))
				{
					this.mPresentCharacters.Add(character);
					int num;
					if (!this.mTypeCount.TryGetValue(character.Type, out num))
					{
						num = 0;
					}
					this.mTypeCount[character.Type] = num + 1;
					for (int j = 0; j < 8; j++)
					{
						Factions factions = (Factions)(1 << j);
						if ((factions & character.GetOriginalFaction) != Factions.NONE)
						{
							if (!this.mFactionCount.TryGetValue((int)factions, out num))
							{
								num = 0;
							}
							this.mFactionCount[(int)factions] = num + 1;
						}
					}
					this.mTotalCharacters++;
					goto IL_12B;
				}
				goto IL_12B;
			}
		}

		// Token: 0x06001B80 RID: 7040 RVA: 0x000BDF18 File Offset: 0x000BC118
		public override void Register()
		{
		}

		// Token: 0x06001B81 RID: 7041 RVA: 0x000BDF1A File Offset: 0x000BC11A
		internal override void UpdatePresent(EntityManager iManager)
		{
			base.Reset();
			this.Count(iManager);
		}
	}
}
