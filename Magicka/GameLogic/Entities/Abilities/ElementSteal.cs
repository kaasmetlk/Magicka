using System;
using System.Collections.Generic;
using Magicka.AI;
using Magicka.AI.Arithmetics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Abilities
{
	// Token: 0x0200058F RID: 1423
	public class ElementSteal : Ability
	{
		// Token: 0x06002A78 RID: 10872 RVA: 0x0014DA0B File Offset: 0x0014BC0B
		public ElementSteal(ContentReader iInput, AnimationClipAction[][] iAnimations) : base(iInput, iAnimations)
		{
			this.mRange = iInput.ReadSingle();
			this.mAngle = iInput.ReadSingle();
		}

		// Token: 0x06002A79 RID: 10873 RVA: 0x0014DA2D File Offset: 0x0014BC2D
		protected override float Desirability(ref ExpressionArguments iArgs)
		{
			throw new NotImplementedException("Elementsteal must define a desirability expression!");
		}

		// Token: 0x06002A7A RID: 10874 RVA: 0x0014DA39 File Offset: 0x0014BC39
		public override void Update(Agent iAgent, float iDeltaTime)
		{
		}

		// Token: 0x06002A7B RID: 10875 RVA: 0x0014DA3C File Offset: 0x0014BC3C
		public override bool InternalExecute(Agent iAgent)
		{
			if (iAgent.Owner.NextAttackAnimation != this.mAnimationKeys[0])
			{
				if (iAgent.Owner.CurrentAnimation == this.mAnimationKeys[0])
				{
					if (iAgent.Owner.CurrentAnimation == this.mAnimationKeys[0])
					{
						if (this.mAngle == 0f)
						{
							List<Entity> entities = iAgent.Owner.PlayState.EntityManager.GetEntities(iAgent.Owner.Position, this.mRange, true);
							for (int i = 0; i < entities.Count; i++)
							{
								Character character = entities[i] as Character;
								if (character != null && character.SpellQueue.Count > 0 && (character.Faction & iAgent.Owner.Faction) == Factions.NONE)
								{
									character.SpellQueue.Clear();
									if (character is Avatar)
									{
										(character as Avatar).Player.IconRenderer.Clear();
									}
								}
							}
							iAgent.Owner.PlayState.EntityManager.ReturnEntityList(entities);
						}
						else if (iAgent.CurrentTarget is Character)
						{
							(iAgent.CurrentTarget as Character).SpellQueue.Clear();
							if (iAgent.CurrentTarget is Avatar)
							{
								(iAgent.CurrentTarget as Avatar).Player.IconRenderer.Clear();
							}
						}
					}
					return true;
				}
				iAgent.Owner.Attack(this.mAnimationKeys[0], false);
			}
			return false;
		}

		// Token: 0x06002A7C RID: 10876 RVA: 0x0014DBAE File Offset: 0x0014BDAE
		public override float GetMaxRange(Agent iAgent)
		{
			return this.mRange;
		}

		// Token: 0x06002A7D RID: 10877 RVA: 0x0014DBB6 File Offset: 0x0014BDB6
		public override float GetMinRange(Agent iAgent)
		{
			return 0f;
		}

		// Token: 0x06002A7E RID: 10878 RVA: 0x0014DBBD File Offset: 0x0014BDBD
		public override float GetArc(Agent iAgent)
		{
			if (this.mAngle != 0f)
			{
				return this.mAngle;
			}
			return 6.2831855f;
		}

		// Token: 0x06002A7F RID: 10879 RVA: 0x0014DBD8 File Offset: 0x0014BDD8
		public override int[] GetWeapons()
		{
			return null;
		}

		// Token: 0x06002A80 RID: 10880 RVA: 0x0014DBDC File Offset: 0x0014BDDC
		public override Vector3 GetDesiredDirection(Agent iAgent)
		{
			return iAgent.Owner.Body.Orientation.Forward;
		}

		// Token: 0x04002DDB RID: 11739
		private float mRange;

		// Token: 0x04002DDC RID: 11740
		private float mAngle;
	}
}
