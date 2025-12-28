using System;
using Magicka.GameLogic.Entities;

namespace Magicka.Levels.Triggers.Conditions
{
	// Token: 0x0200038C RID: 908
	public class CharacterHealth : Condition
	{
		// Token: 0x06001BC8 RID: 7112 RVA: 0x000BF0F0 File Offset: 0x000BD2F0
		public CharacterHealth(GameScene iScene) : base(iScene)
		{
		}

		// Token: 0x06001BC9 RID: 7113 RVA: 0x000BF0FC File Offset: 0x000BD2FC
		protected override bool InternalMet(Character iSender)
		{
			Character character = Entity.GetByID(this.mID) as Character;
			float num = (character == null || character.Dead) ? 0f : (character.HitPoints / character.MaxHitPoints);
			switch (this.mCompareMethod)
			{
			case CompareMethod.LESS:
				return this.mHealth > num;
			case CompareMethod.EQUAL:
				return Math.Abs(this.mHealth - num) < 0.0001f;
			case CompareMethod.GREATER:
				return this.mHealth < num;
			default:
				return false;
			}
		}

		// Token: 0x170006D3 RID: 1747
		// (get) Token: 0x06001BCA RID: 7114 RVA: 0x000BF181 File Offset: 0x000BD381
		// (set) Token: 0x06001BCB RID: 7115 RVA: 0x000BF189 File Offset: 0x000BD389
		public string ID
		{
			get
			{
				return this.mName;
			}
			set
			{
				this.mName = value;
				this.mID = this.mName.GetHashCodeCustom();
			}
		}

		// Token: 0x170006D4 RID: 1748
		// (get) Token: 0x06001BCC RID: 7116 RVA: 0x000BF1A3 File Offset: 0x000BD3A3
		// (set) Token: 0x06001BCD RID: 7117 RVA: 0x000BF1AB File Offset: 0x000BD3AB
		public CompareMethod CompareMethod
		{
			get
			{
				return this.mCompareMethod;
			}
			set
			{
				this.mCompareMethod = value;
			}
		}

		// Token: 0x170006D5 RID: 1749
		// (get) Token: 0x06001BCE RID: 7118 RVA: 0x000BF1B4 File Offset: 0x000BD3B4
		// (set) Token: 0x06001BCF RID: 7119 RVA: 0x000BF1BC File Offset: 0x000BD3BC
		public float Health
		{
			get
			{
				return this.mHealth;
			}
			set
			{
				this.mHealth = value;
				if (this.mHealth > 1f || this.mHealth < 0f)
				{
					throw new Exception("Health must be between 0.0 and 1.0!");
				}
			}
		}

		// Token: 0x04001E23 RID: 7715
		private string mName;

		// Token: 0x04001E24 RID: 7716
		private int mID;

		// Token: 0x04001E25 RID: 7717
		private CompareMethod mCompareMethod;

		// Token: 0x04001E26 RID: 7718
		private float mHealth;
	}
}
