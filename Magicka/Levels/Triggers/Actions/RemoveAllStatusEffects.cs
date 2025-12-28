using System;
using Magicka.GameLogic.Entities;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x020000BC RID: 188
	internal class RemoveAllStatusEffects : Action
	{
		// Token: 0x06000578 RID: 1400 RVA: 0x000204B7 File Offset: 0x0001E6B7
		public RemoveAllStatusEffects(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06000579 RID: 1401 RVA: 0x000204C4 File Offset: 0x0001E6C4
		protected override void Execute()
		{
			StaticList<Entity> entities = this.mScene.PlayState.EntityManager.Entities;
			for (int i = 0; i < entities.Count; i++)
			{
				Character character = entities[i] as Character;
				if (character != null)
				{
					character.StopStatusEffects(StatusEffects.Burning | StatusEffects.Wet | StatusEffects.Frozen | StatusEffects.Cold | StatusEffects.Poisoned | StatusEffects.Healing | StatusEffects.Greased | StatusEffects.Steamed);
					if (character.IsEntangled)
					{
						character.ReleaseEntanglement();
					}
					if (character.IsSelfShielded)
					{
						character.RemoveSelfShield();
					}
					if (character.IsFeared)
					{
						character.RemoveFear();
					}
					if (character.IsInvisibile)
					{
						character.RemoveInvisibility();
					}
					if (character.IsHypnotized)
					{
						character.StopHypnotize();
					}
					if (character.CurrentSpell != null)
					{
						character.CurrentSpell.Stop(character);
					}
					if (character.SpellQueue.Count > 0)
					{
						character.SpellQueue.Clear();
					}
					if (character.IsLevitating)
					{
						character.StopLevitate();
					}
					if (!character.mBubble)
					{
						character.ClearAura();
					}
				}
			}
		}

		// Token: 0x0600057A RID: 1402 RVA: 0x000205AB File Offset: 0x0001E7AB
		public override void QuickExecute()
		{
			this.Execute();
		}
	}
}
