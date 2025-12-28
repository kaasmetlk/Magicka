using System;
using System.Collections.Generic;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Gamers;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x020001FE RID: 510
	public class Amalgameddon : SpecialAbility, IAbilityEffect
	{
		// Token: 0x17000432 RID: 1074
		// (get) Token: 0x060010B5 RID: 4277 RVA: 0x0006911C File Offset: 0x0006731C
		public static Amalgameddon Instance
		{
			get
			{
				if (Amalgameddon.sSingelton == null)
				{
					lock (Amalgameddon.sSingeltonLock)
					{
						if (Amalgameddon.sSingelton == null)
						{
							Amalgameddon.sSingelton = new Amalgameddon();
						}
					}
				}
				return Amalgameddon.sSingelton;
			}
		}

		// Token: 0x060010B6 RID: 4278 RVA: 0x00069178 File Offset: 0x00067378
		public Amalgameddon() : base(Animations.cast_magick_global, "#magick_amalgameddond".GetHashCodeCustom())
		{
		}

		// Token: 0x060010B7 RID: 4279 RVA: 0x00069197 File Offset: 0x00067397
		public Amalgameddon(Animations iAnimation) : base(Animations.cast_magick_global, "#magick_amalgameddond".GetHashCodeCustom())
		{
		}

		// Token: 0x060010B8 RID: 4280 RVA: 0x000691B8 File Offset: 0x000673B8
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			if (this.mTTL > 0f)
			{
				AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL);
				return false;
			}
			NetworkState state = NetworkManager.Instance.State;
			if ((state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer))) || (state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer)))
			{
				base.Execute(iOwner, iPlayState);
				this.mState = Amalgameddon.AmalgamationState.None;
				this.mOwner = iOwner;
				this.mTTL = 10f;
				List<Entity> entities = this.mOwner.PlayState.EntityManager.GetEntities(this.mOwner.Position, this.RANGE, false);
				foreach (Entity entity in entities)
				{
					if (entity is Character && entity != this.mOwner)
					{
						foreach (Entity entity2 in entities)
						{
							if (entity2 is Character && entity2 != entity && entity2 != this.mOwner && (entity as Character).Type == (entity2 as Character).Type)
							{
								this.mFirstUnit = (entity as Character);
								this.mSecondUnit = (entity2 as Character);
								this.mFirstUnit.CharacterBody.ApplyGravity = false;
								this.mSecondUnit.CharacterBody.ApplyGravity = false;
								this.mState = Amalgameddon.AmalgamationState.Rising;
								this.mOwner.PlayState.EntityManager.ReturnEntityList(entities);
								SpellManager.Instance.AddSpellEffect(this);
								return true;
							}
						}
					}
				}
				this.mFirstUnit = null;
				this.mSecondUnit = null;
				this.mSorryUnit = null;
				AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL);
				this.mTTL = 0f;
				return false;
			}
			return false;
		}

		// Token: 0x17000433 RID: 1075
		// (get) Token: 0x060010B9 RID: 4281 RVA: 0x000693F4 File Offset: 0x000675F4
		public bool IsDead
		{
			get
			{
				return this.mTTL <= 0f;
			}
		}

		// Token: 0x060010BA RID: 4282 RVA: 0x00069408 File Offset: 0x00067608
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (this.mOwner == null)
			{
				return;
			}
			this.mTTL -= iDeltaTime;
			if (this.mTTL < 0f)
			{
				this.mFirstUnit.CharacterBody.ApplyGravity = true;
				this.mSecondUnit.CharacterBody.ApplyGravity = true;
			}
			if (this.mState == Amalgameddon.AmalgamationState.Rising)
			{
				Vector3 position = this.mFirstUnit.Position;
				position.Y += 0.015f;
				this.mFirstUnit.Body.Position = position;
				position = this.mSecondUnit.Position;
				position.Y += 0.015f;
				this.mSecondUnit.Body.Position = position;
				this.mFirstUnit.Stun(10f);
				this.mSecondUnit.Stun(10f);
				if (this.mFirstUnit.Position.Y > 5f)
				{
					this.mState = Amalgameddon.AmalgamationState.PushApart;
					this.mHappyUnit = this.mFirstUnit;
					this.mSorryUnit = this.mSecondUnit;
					this.mLifeAmount = this.mSorryUnit.HitPoints;
					this.mTimer = 0.2f;
					return;
				}
			}
			else if (this.mState == Amalgameddon.AmalgamationState.PushApart)
			{
				this.mTimer -= iDeltaTime;
				Vector3 value = this.mHappyUnit.Position - this.mSorryUnit.Position;
				value.Normalize();
				Vector3 vector = this.mHappyUnit.Position;
				Vector3.Multiply(ref value, 0.25f, out value);
				vector += value;
				this.mHappyUnit.Body.Position = vector;
				vector = this.mSorryUnit.Position - value;
				this.mSorryUnit.Body.Position = vector;
				if (this.mTimer < 0f)
				{
					this.mState = Amalgameddon.AmalgamationState.SmashTogether;
					return;
				}
			}
			else if (this.mState == Amalgameddon.AmalgamationState.SmashTogether)
			{
				Vector3 value2 = this.mHappyUnit.Position - this.mSorryUnit.Position;
				value2.Normalize();
				Vector3 vector2 = this.mHappyUnit.Position;
				Vector3.Multiply(ref value2, -0.6f, out value2);
				vector2 += value2;
				this.mHappyUnit.Body.Position = vector2;
				vector2 = this.mSorryUnit.Position - value2;
				this.mSorryUnit.Body.Position = vector2;
				if ((this.mHappyUnit.Position - this.mSorryUnit.Position).Length() < this.mHappyUnit.Radius + this.mSorryUnit.Radius)
				{
					this.mSorryUnit.OverKill();
					this.mHappyUnit.Damage(-this.mLifeAmount, Elements.Life);
					this.mHappyUnit.CharacterBody.ApplyGravity = true;
					this.mHappyUnit.Unstun();
					this.mTTL = 0f;
				}
			}
		}

		// Token: 0x060010BB RID: 4283 RVA: 0x000696F6 File Offset: 0x000678F6
		public void OnRemove()
		{
			this.mTTL = 0f;
		}

		// Token: 0x04000F48 RID: 3912
		private static volatile Amalgameddon sSingelton;

		// Token: 0x04000F49 RID: 3913
		private static volatile object sSingeltonLock = new object();

		// Token: 0x04000F4A RID: 3914
		private readonly float RANGE = 20f;

		// Token: 0x04000F4B RID: 3915
		private ISpellCaster mOwner;

		// Token: 0x04000F4C RID: 3916
		private float mTTL;

		// Token: 0x04000F4D RID: 3917
		private float mTimer;

		// Token: 0x04000F4E RID: 3918
		private float mLifeAmount;

		// Token: 0x04000F4F RID: 3919
		private Character mFirstUnit;

		// Token: 0x04000F50 RID: 3920
		private Character mSecondUnit;

		// Token: 0x04000F51 RID: 3921
		private Character mSorryUnit;

		// Token: 0x04000F52 RID: 3922
		private Character mHappyUnit;

		// Token: 0x04000F53 RID: 3923
		private Amalgameddon.AmalgamationState mState;

		// Token: 0x020001FF RID: 511
		private enum AmalgamationState
		{
			// Token: 0x04000F55 RID: 3925
			None,
			// Token: 0x04000F56 RID: 3926
			Rising,
			// Token: 0x04000F57 RID: 3927
			PushApart,
			// Token: 0x04000F58 RID: 3928
			SmashTogether
		}
	}
}
