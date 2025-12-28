using System;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;

namespace Magicka.Levels.Triggers.Conditions
{
	// Token: 0x0200055B RID: 1371
	public class CharacterCasting : Condition
	{
		// Token: 0x060028DE RID: 10462 RVA: 0x001419E8 File Offset: 0x0013FBE8
		public CharacterCasting(GameScene iScene) : base(iScene)
		{
		}

		// Token: 0x060028DF RID: 10463 RVA: 0x00141A04 File Offset: 0x0013FC04
		protected override bool InternalMet(Character iSender)
		{
			Character character = Entity.GetByID(this.mID) as Character;
			bool flag = character != null && character.CurrentSpell != null;
			if (flag)
			{
				SpellEffect currentSpell = character.CurrentSpell;
				flag &= (currentSpell.CastType == this.mCastType);
				flag &= currentSpell.Spell.ContainsElement(this.mElement);
			}
			return flag;
		}

		// Token: 0x1700099D RID: 2461
		// (get) Token: 0x060028E0 RID: 10464 RVA: 0x00141A68 File Offset: 0x0013FC68
		// (set) Token: 0x060028E1 RID: 10465 RVA: 0x00141A70 File Offset: 0x0013FC70
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

		// Token: 0x1700099E RID: 2462
		// (get) Token: 0x060028E2 RID: 10466 RVA: 0x00141A8A File Offset: 0x0013FC8A
		// (set) Token: 0x060028E3 RID: 10467 RVA: 0x00141A92 File Offset: 0x0013FC92
		public CastType CastType
		{
			get
			{
				return this.mCastType;
			}
			set
			{
				this.mCastType = value;
			}
		}

		// Token: 0x1700099F RID: 2463
		// (get) Token: 0x060028E4 RID: 10468 RVA: 0x00141A9B File Offset: 0x0013FC9B
		// (set) Token: 0x060028E5 RID: 10469 RVA: 0x00141AA3 File Offset: 0x0013FCA3
		public Elements Element
		{
			get
			{
				return this.mElement;
			}
			set
			{
				this.mElement = value;
			}
		}

		// Token: 0x04002C51 RID: 11345
		private string mName = string.Empty;

		// Token: 0x04002C52 RID: 11346
		private int mID = -1;

		// Token: 0x04002C53 RID: 11347
		private CastType mCastType;

		// Token: 0x04002C54 RID: 11348
		private Elements mElement;
	}
}
