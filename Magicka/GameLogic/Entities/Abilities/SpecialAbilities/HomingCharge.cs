using System;
using System.Collections.Generic;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000407 RID: 1031
	public class HomingCharge : SpecialAbility, IAbilityEffect
	{
		// Token: 0x06001FC1 RID: 8129 RVA: 0x000DEE0C File Offset: 0x000DD00C
		public static HomingCharge GetInstance()
		{
			if (HomingCharge.sCache.Count > 0)
			{
				HomingCharge result = HomingCharge.sCache[HomingCharge.sCache.Count - 1];
				HomingCharge.sCache.RemoveAt(HomingCharge.sCache.Count - 1);
				return result;
			}
			return new HomingCharge();
		}

		// Token: 0x06001FC2 RID: 8130 RVA: 0x000DEE5C File Offset: 0x000DD05C
		public static void InitializeCache(int iNr, PlayState iPlayState)
		{
			HomingCharge.sCache = new List<HomingCharge>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				HomingCharge.sCache.Add(new HomingCharge());
			}
		}

		// Token: 0x06001FC3 RID: 8131 RVA: 0x000DEE8F File Offset: 0x000DD08F
		public HomingCharge(Animations iAnimation) : base(iAnimation, "#magick_grease".GetHashCodeCustom())
		{
		}

		// Token: 0x06001FC4 RID: 8132 RVA: 0x000DEECE File Offset: 0x000DD0CE
		private HomingCharge() : base(Animations.None, "#magick_grease".GetHashCodeCustom())
		{
		}

		// Token: 0x06001FC5 RID: 8133 RVA: 0x000DEF0D File Offset: 0x000DD10D
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new Exception("HomingCharge needs an owner!");
		}

		// Token: 0x06001FC6 RID: 8134 RVA: 0x000DEF1C File Offset: 0x000DD11C
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			this.mTTL = this.TIME;
			this.mCharging = false;
			this.mPlayState = iPlayState;
			this.mOwner = (iOwner as Character);
			this.deltaArray = new float[3];
			this.thresholdArray = new float[5];
			this.thresholdArray[0] = this.THRESHOLD;
			this.thresholdArray[1] = this.THRESHOLD;
			this.thresholdArray[2] = this.THRESHOLD;
			this.thresholdArray[3] = this.THRESHOLD;
			this.thresholdArray[4] = this.THRESHOLD;
			SpellManager.Instance.AddSpellEffect(this);
			return true;
		}

		// Token: 0x06001FC7 RID: 8135 RVA: 0x000DEFC4 File Offset: 0x000DD1C4
		private void pushThreshold(float val)
		{
			this.thresholdArray[0] = this.thresholdArray[1];
			this.thresholdArray[1] = this.thresholdArray[2];
			this.thresholdArray[2] = this.thresholdArray[3];
			this.thresholdArray[3] = this.thresholdArray[4];
			this.thresholdArray[4] = val;
		}

		// Token: 0x06001FC8 RID: 8136 RVA: 0x000DF01A File Offset: 0x000DD21A
		private void pushDelta(float val)
		{
			this.deltaArray[0] = this.deltaArray[1];
			this.deltaArray[1] = this.deltaArray[2];
			this.deltaArray[2] = val;
		}

		// Token: 0x06001FC9 RID: 8137 RVA: 0x000DF048 File Offset: 0x000DD248
		private float Threshold()
		{
			float num = 0f;
			for (int i = 0; i < 3; i++)
			{
				num += this.thresholdArray[i];
			}
			return num / 3f;
		}

		// Token: 0x06001FCA RID: 8138 RVA: 0x000DF07C File Offset: 0x000DD27C
		private float Delta()
		{
			float num = 0f;
			foreach (float num2 in this.deltaArray)
			{
				num += num2;
			}
			return num / (float)this.deltaArray.Length;
		}

		// Token: 0x170007C4 RID: 1988
		// (get) Token: 0x06001FCB RID: 8139 RVA: 0x000DF0B8 File Offset: 0x000DD2B8
		public bool IsDead
		{
			get
			{
				return this.mTTL <= 0f;
			}
		}

		// Token: 0x06001FCC RID: 8140 RVA: 0x000DF0CC File Offset: 0x000DD2CC
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mTTL -= iDeltaTime * this.mOwner.CharacterBody.SpeedMultiplier;
			if (this.mOwner == null)
			{
				return;
			}
			Vector3 position = this.mOwner.Position;
			this.pushThreshold(this.THRESHOLD * this.mOwner.CharacterBody.SpeedMultiplier);
			this.pushDelta(Vector3.Distance(position, this.mLastPosition));
			if (this.mTTL < this.TIME - this.START_CHARGE && !this.mCharging && this.Delta() > 0f)
			{
				this.mCharging = true;
			}
			if (this.mCharging && this.Delta() < this.Threshold())
			{
				this.mCharging = false;
				this.mOwner.GoToAnimation(Animations.special2, 0.01f);
				this.mTTL = 0f;
				return;
			}
			float num = 0f;
			List<Entity> entities = this.mPlayState.EntityManager.GetEntities(this.mOwner.Position, 5f, false);
			foreach (Entity entity in entities)
			{
				if (entity is Avatar)
				{
					num = Vector3.Distance(this.mOwner.Position, (entity as Avatar).Position);
				}
			}
			this.mOwner.PlayState.EntityManager.ReturnEntityList(entities);
			if (num > 0f && num < 2f)
			{
				this.mOwner.GoToAnimation(Animations.special2, 0.01f);
				this.mTTL = 0f;
			}
			this.mLastPosition = position;
		}

		// Token: 0x06001FCD RID: 8141 RVA: 0x000DF280 File Offset: 0x000DD480
		public void OnRemove()
		{
			this.mTTL = 0f;
			EffectManager.Instance.Stop(ref this.mEffect);
			HomingCharge.sCache.Add(this);
		}

		// Token: 0x04002211 RID: 8721
		private static List<HomingCharge> sCache;

		// Token: 0x04002212 RID: 8722
		private Character mOwner;

		// Token: 0x04002213 RID: 8723
		public PlayState mPlayState;

		// Token: 0x04002214 RID: 8724
		private VisualEffectReference mEffect;

		// Token: 0x04002215 RID: 8725
		private float TIME = 5f;

		// Token: 0x04002216 RID: 8726
		private readonly float START_CHARGE = 0.8f;

		// Token: 0x04002217 RID: 8727
		private readonly float STOP_CHARGE = 2.5f;

		// Token: 0x04002218 RID: 8728
		private readonly float THRESHOLD = 0.015f;

		// Token: 0x04002219 RID: 8729
		private Vector3 mLastPosition;

		// Token: 0x0400221A RID: 8730
		private bool mCharging;

		// Token: 0x0400221B RID: 8731
		private float mTTL;

		// Token: 0x0400221C RID: 8732
		private float[] thresholdArray;

		// Token: 0x0400221D RID: 8733
		private float[] deltaArray;
	}
}
