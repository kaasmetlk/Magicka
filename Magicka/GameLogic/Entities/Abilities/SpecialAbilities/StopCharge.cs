using System;
using System.Collections.Generic;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x0200045D RID: 1117
	public class StopCharge : SpecialAbility, IAbilityEffect
	{
		// Token: 0x06002216 RID: 8726 RVA: 0x000F46D0 File Offset: 0x000F28D0
		public static StopCharge GetInstance()
		{
			if (StopCharge.sCache.Count > 0)
			{
				StopCharge result = StopCharge.sCache[StopCharge.sCache.Count - 1];
				StopCharge.sCache.RemoveAt(StopCharge.sCache.Count - 1);
				return result;
			}
			return new StopCharge();
		}

		// Token: 0x06002217 RID: 8727 RVA: 0x000F4720 File Offset: 0x000F2920
		public static void InitializeCache(int iNr, PlayState iPlayState)
		{
			StopCharge.sCache = new List<StopCharge>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				StopCharge.sCache.Add(new StopCharge());
			}
		}

		// Token: 0x06002218 RID: 8728 RVA: 0x000F4753 File Offset: 0x000F2953
		public StopCharge(Animations iAnimation) : base(iAnimation, "#magick_grease".GetHashCodeCustom())
		{
		}

		// Token: 0x06002219 RID: 8729 RVA: 0x000F4792 File Offset: 0x000F2992
		private StopCharge() : base(Animations.None, "#magick_grease".GetHashCodeCustom())
		{
		}

		// Token: 0x0600221A RID: 8730 RVA: 0x000F47D1 File Offset: 0x000F29D1
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new Exception("StopCharge needs an owner!");
		}

		// Token: 0x0600221B RID: 8731 RVA: 0x000F47E0 File Offset: 0x000F29E0
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

		// Token: 0x0600221C RID: 8732 RVA: 0x000F4888 File Offset: 0x000F2A88
		private void pushThreshold(float val)
		{
			this.thresholdArray[0] = this.thresholdArray[1];
			this.thresholdArray[1] = this.thresholdArray[2];
			this.thresholdArray[2] = this.thresholdArray[3];
			this.thresholdArray[3] = this.thresholdArray[4];
			this.thresholdArray[4] = val;
		}

		// Token: 0x0600221D RID: 8733 RVA: 0x000F48DE File Offset: 0x000F2ADE
		private void pushDelta(float val)
		{
			this.deltaArray[0] = this.deltaArray[1];
			this.deltaArray[1] = this.deltaArray[2];
			this.deltaArray[2] = val;
		}

		// Token: 0x0600221E RID: 8734 RVA: 0x000F490C File Offset: 0x000F2B0C
		private float Threshold()
		{
			float num = 0f;
			for (int i = 0; i < 3; i++)
			{
				num += this.thresholdArray[i];
			}
			return num / 3f;
		}

		// Token: 0x0600221F RID: 8735 RVA: 0x000F4940 File Offset: 0x000F2B40
		private float Delta()
		{
			float num = 0f;
			foreach (float num2 in this.deltaArray)
			{
				num += num2;
			}
			return num / (float)this.deltaArray.Length;
		}

		// Token: 0x1700082F RID: 2095
		// (get) Token: 0x06002220 RID: 8736 RVA: 0x000F497C File Offset: 0x000F2B7C
		public bool IsDead
		{
			get
			{
				return this.mTTL <= 0f;
			}
		}

		// Token: 0x06002221 RID: 8737 RVA: 0x000F4990 File Offset: 0x000F2B90
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
				GreaseSplash.Instance.Execute(this.mOwner, this.mPlayState);
				if (this.mTTL > this.TIME - this.STOP_CHARGE)
				{
					this.mOwner.GoToAnimation(Animations.special2, 0.1f);
				}
				this.mTTL = 0f;
			}
			this.mLastPosition = position;
		}

		// Token: 0x06002222 RID: 8738 RVA: 0x000F4AA1 File Offset: 0x000F2CA1
		public void OnRemove()
		{
			this.mTTL = 0f;
			EffectManager.Instance.Stop(ref this.mEffect);
			StopCharge.sCache.Add(this);
		}

		// Token: 0x04002530 RID: 9520
		private static List<StopCharge> sCache;

		// Token: 0x04002531 RID: 9521
		private Character mOwner;

		// Token: 0x04002532 RID: 9522
		public PlayState mPlayState;

		// Token: 0x04002533 RID: 9523
		private VisualEffectReference mEffect;

		// Token: 0x04002534 RID: 9524
		private float TIME = 5f;

		// Token: 0x04002535 RID: 9525
		private readonly float START_CHARGE = 0.8f;

		// Token: 0x04002536 RID: 9526
		private readonly float STOP_CHARGE = 2.5f;

		// Token: 0x04002537 RID: 9527
		private readonly float THRESHOLD = 0.015f;

		// Token: 0x04002538 RID: 9528
		private Vector3 mLastPosition;

		// Token: 0x04002539 RID: 9529
		private bool mCharging;

		// Token: 0x0400253A RID: 9530
		private float mTTL;

		// Token: 0x0400253B RID: 9531
		private float[] thresholdArray;

		// Token: 0x0400253C RID: 9532
		private float[] deltaArray;
	}
}
