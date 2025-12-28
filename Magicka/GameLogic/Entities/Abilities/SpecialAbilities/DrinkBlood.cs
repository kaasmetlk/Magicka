using System;
using System.Collections.Generic;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x0200058D RID: 1421
	public class DrinkBlood : SpecialAbility, IAbilityEffect
	{
		// Token: 0x06002A6E RID: 10862 RVA: 0x0014D4E0 File Offset: 0x0014B6E0
		public DrinkBlood(Animations iAnimation) : base(iAnimation, "#specab_drain".GetHashCodeCustom())
		{
		}

		// Token: 0x06002A6F RID: 10863 RVA: 0x0014D4F4 File Offset: 0x0014B6F4
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			this.mOwner = iOwner;
			this.mTTL = 0.78f;
			SpellManager.Instance.AddSpellEffect(this);
			this.mPlayState = iPlayState;
			this.mFoundTarget = false;
			Haste instance = Haste.GetInstance();
			instance.CustomTTL = 0.78f;
			return instance.Execute(iOwner, iPlayState, false);
		}

		// Token: 0x06002A70 RID: 10864 RVA: 0x0014D54F File Offset: 0x0014B74F
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new NotImplementedException();
		}

		// Token: 0x170009F6 RID: 2550
		// (get) Token: 0x06002A71 RID: 10865 RVA: 0x0014D556 File Offset: 0x0014B756
		public bool IsDead
		{
			get
			{
				return this.mTTL <= 0f;
			}
		}

		// Token: 0x06002A72 RID: 10866 RVA: 0x0014D568 File Offset: 0x0014B768
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mTTL -= iDeltaTime;
			if (this.mFoundTarget)
			{
				return;
			}
			if (this.mOwner.HitPoints <= 0f)
			{
				this.mTTL = 0f;
				return;
			}
			List<Entity> entities = this.mOwner.PlayState.EntityManager.GetEntities(this.mOwner.Position, 8f, false);
			Entity entity = null;
			float num = float.MaxValue;
			Vector3 position = this.mOwner.Position;
			foreach (Entity entity2 in entities)
			{
				if (entity2 is Character && entity2 != this.mOwner)
				{
					Vector3 position2 = entity2.Position;
					float num2;
					Vector3.Distance(ref position2, ref position, out num2);
					if (num2 < num)
					{
						entity = entity2;
						num = num2;
					}
				}
			}
			Character character = entity as Character;
			if (character != null && num < this.mOwner.Radius + character.Radius + 1f)
			{
				Vector3 vector = this.mOwner.Position - character.Position;
				vector.Normalize();
				Vector3 direction = this.mOwner.Direction;
				float num3 = MagickaMath.Angle(ref vector, ref direction);
				if (num3 < 3.1415927f && num3 > 2.3561945f)
				{
					this.LifeStealAmount = this.mOwner.MaxHitPoints - this.mOwner.HitPoints;
					if (this.LifeStealAmount > character.HitPoints)
					{
						this.LifeStealAmount = character.HitPoints;
					}
					if (this.LifeStealAmount > 0f)
					{
						this.mFoundTarget = true;
						(this.mOwner as Character).GoToAnimation(Animations.special4, 0.1f);
						character.Damage(this.LifeStealAmount, Elements.Arcane);
						this.mTTL = 0.83f;
						if (this.LifeStealAmount < 0f)
						{
							this.LifeStealAmount = 0f;
						}
						this.mOwner.Damage(-this.LifeStealAmount, Elements.Life);
						this.LifeStealAmount = 0f;
					}
					else
					{
						(this.mOwner as Character).GoToAnimation(Animations.idle, 0.05f);
						this.mTTL = 0f;
					}
					character.Stun(1.5f);
				}
			}
			this.mOwner.PlayState.EntityManager.ReturnEntityList(entities);
		}

		// Token: 0x06002A73 RID: 10867 RVA: 0x0014D7D0 File Offset: 0x0014B9D0
		public void OnRemove()
		{
		}

		// Token: 0x04002DCA RID: 11722
		private const float RANGE = 8f;

		// Token: 0x04002DCB RID: 11723
		private const float CHARGE_TIME = 0.78f;

		// Token: 0x04002DCC RID: 11724
		private const float DRINK_TIME = 0.83f;

		// Token: 0x04002DCD RID: 11725
		private ISpellCaster mOwner;

		// Token: 0x04002DCE RID: 11726
		private PlayState mPlayState;

		// Token: 0x04002DCF RID: 11727
		private float mTTL;

		// Token: 0x04002DD0 RID: 11728
		private float LifeStealAmount;

		// Token: 0x04002DD1 RID: 11729
		private bool mFoundTarget;
	}
}
