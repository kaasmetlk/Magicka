using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Buffs
{
	// Token: 0x0200034C RID: 844
	[StructLayout(LayoutKind.Explicit)]
	public struct BuffStorage
	{
		// Token: 0x060019C1 RID: 6593 RVA: 0x000ACCBC File Offset: 0x000AAEBC
		public BuffStorage(ContentReader iInput)
		{
			this = default(BuffStorage);
			this.UniqueID = BuffStorage.IDCounter++;
			this.SelfCasted = false;
			this.BuffType = (BuffType)iInput.ReadByte();
			this.VisualCategory = (VisualCategory)iInput.ReadByte();
			this.Color = iInput.ReadVector3();
			this.TTL = iInput.ReadSingle();
			string text = iInput.ReadString().ToLowerInvariant();
			if (text.Equals(""))
			{
				this.Effect = 0;
			}
			else
			{
				this.Effect = text.GetHashCodeCustom();
			}
			switch (this.BuffType)
			{
			case BuffType.BoostDamage:
				this.BuffBoostDamage = new BuffBoostDamage(iInput);
				return;
			case BuffType.DealDamage:
				this.BuffDealDamage = new BuffDealDamage(iInput);
				return;
			case BuffType.Resistance:
				this.BuffResistance = new BuffResistance(iInput);
				return;
			case BuffType.Undying:
				this.BuffUndying = new BuffUndying(iInput);
				return;
			case BuffType.Boost:
				this.BuffBoost = new BuffBoost(iInput);
				return;
			case BuffType.ReduceAgro:
				this.BuffReduceAgro = new BuffReduceAgro(iInput);
				return;
			case BuffType.ModifyHitPoints:
				this.BuffModifyHitPoints = new BuffModifyHitPoints(iInput);
				return;
			case BuffType.ModifySpellTTL:
				this.BuffModifySpellTTL = new BuffModifySpellTTL(iInput);
				return;
			case BuffType.ModifySpellRange:
				this.BuffModifySpellRange = new BuffModifySpellRange(iInput);
				return;
			default:
				return;
			}
		}

		// Token: 0x060019C2 RID: 6594 RVA: 0x000ACDEE File Offset: 0x000AAFEE
		public BuffStorage(BuffBoostDamage iBuff, VisualCategory iVisualCategory, Vector3 iColor)
		{
			this = new BuffStorage(iBuff, 0.5f, iVisualCategory, iColor);
		}

		// Token: 0x060019C3 RID: 6595 RVA: 0x000ACDFE File Offset: 0x000AAFFE
		public BuffStorage(BuffBoostDamage iBuff, float iTTL, VisualCategory iVisualCategory, Vector3 iColor)
		{
			this = default(BuffStorage);
			this.UniqueID = BuffStorage.IDCounter++;
			this.Color = iColor;
			this.BuffType = BuffType.BoostDamage;
			this.BuffBoostDamage = iBuff;
			this.TTL = iTTL;
			this.VisualCategory = iVisualCategory;
		}

		// Token: 0x060019C4 RID: 6596 RVA: 0x000ACE3E File Offset: 0x000AB03E
		public BuffStorage(BuffDealDamage iBuff, VisualCategory iVisualCategory, Vector3 iColor)
		{
			this = new BuffStorage(iBuff, 0.5f, iVisualCategory, iColor);
		}

		// Token: 0x060019C5 RID: 6597 RVA: 0x000ACE4E File Offset: 0x000AB04E
		public BuffStorage(BuffDealDamage iBuff, float iTTL, VisualCategory iVisualCategory, Vector3 iColor)
		{
			this = default(BuffStorage);
			this.UniqueID = BuffStorage.IDCounter++;
			this.Color = iColor;
			this.BuffType = BuffType.DealDamage;
			this.BuffDealDamage = iBuff;
			this.TTL = iTTL;
			this.VisualCategory = iVisualCategory;
		}

		// Token: 0x060019C6 RID: 6598 RVA: 0x000ACE8E File Offset: 0x000AB08E
		public BuffStorage(BuffResistance iBuff, VisualCategory iVisualCategory, Vector3 iColor)
		{
			this = new BuffStorage(iBuff, 0.5f, iVisualCategory, iColor);
		}

		// Token: 0x060019C7 RID: 6599 RVA: 0x000ACE9E File Offset: 0x000AB09E
		public BuffStorage(BuffResistance iBuff, float iTTL, VisualCategory iVisualCategory, Vector3 iColor)
		{
			this = default(BuffStorage);
			this.UniqueID = BuffStorage.IDCounter++;
			this.Color = iColor;
			this.BuffType = BuffType.Resistance;
			this.BuffResistance = iBuff;
			this.TTL = iTTL;
			this.VisualCategory = iVisualCategory;
		}

		// Token: 0x060019C8 RID: 6600 RVA: 0x000ACEDE File Offset: 0x000AB0DE
		public BuffStorage(BuffUndying iBuff, VisualCategory iVisualCategory, Vector3 iColor)
		{
			this = new BuffStorage(iBuff, 0.5f, iVisualCategory, iColor);
		}

		// Token: 0x060019C9 RID: 6601 RVA: 0x000ACEEE File Offset: 0x000AB0EE
		public BuffStorage(BuffUndying iBuff, float iTTL, VisualCategory iVisualCategory, Vector3 iColor)
		{
			this = default(BuffStorage);
			this.UniqueID = BuffStorage.IDCounter++;
			this.Color = iColor;
			this.BuffType = BuffType.Undying;
			this.BuffUndying = iBuff;
			this.TTL = iTTL;
			this.VisualCategory = iVisualCategory;
		}

		// Token: 0x060019CA RID: 6602 RVA: 0x000ACF2E File Offset: 0x000AB12E
		public BuffStorage(BuffBoost iBuff, VisualCategory iVisualCategory, Vector3 iColor)
		{
			this = new BuffStorage(iBuff, 0.5f, iVisualCategory, iColor);
		}

		// Token: 0x060019CB RID: 6603 RVA: 0x000ACF3E File Offset: 0x000AB13E
		public BuffStorage(BuffBoost iBuff, float iTTL, VisualCategory iVisualCategory, Vector3 iColor)
		{
			this = default(BuffStorage);
			this.UniqueID = BuffStorage.IDCounter++;
			this.Color = iColor;
			this.BuffType = BuffType.Boost;
			this.BuffBoost = iBuff;
			this.TTL = iTTL;
			this.VisualCategory = iVisualCategory;
		}

		// Token: 0x060019CC RID: 6604 RVA: 0x000ACF7E File Offset: 0x000AB17E
		public BuffStorage(BuffReduceAgro iBuff, VisualCategory iVisualCategory, Vector3 iColor)
		{
			this = new BuffStorage(iBuff, 0.5f, iVisualCategory, iColor);
		}

		// Token: 0x060019CD RID: 6605 RVA: 0x000ACF8E File Offset: 0x000AB18E
		public BuffStorage(BuffReduceAgro iBuff, float iTTL, VisualCategory iVisualCategory, Vector3 iColor)
		{
			this = default(BuffStorage);
			this.UniqueID = BuffStorage.IDCounter++;
			this.Color = iColor;
			this.BuffType = BuffType.ReduceAgro;
			this.BuffReduceAgro = iBuff;
			this.TTL = iTTL;
			this.VisualCategory = iVisualCategory;
		}

		// Token: 0x060019CE RID: 6606 RVA: 0x000ACFCE File Offset: 0x000AB1CE
		public BuffStorage(BuffModifyHitPoints iBuff, VisualCategory iVisualCategory, Vector3 iColor, float iHitPointsMultiplyer, float iHitPointsModifier)
		{
			this = new BuffStorage(iBuff, 0.5f, iVisualCategory, iColor, iHitPointsMultiplyer, iHitPointsModifier);
		}

		// Token: 0x060019CF RID: 6607 RVA: 0x000ACFE4 File Offset: 0x000AB1E4
		public BuffStorage(BuffModifyHitPoints iBuff, float iTTL, VisualCategory iVisualCategory, Vector3 iColor, float iHitPointsMultiplyer, float iHitPointsModifier)
		{
			this = default(BuffStorage);
			this.UniqueID = BuffStorage.IDCounter++;
			this.Color = iColor;
			this.BuffType = BuffType.ModifyHitPoints;
			this.BuffModifyHitPoints = iBuff;
			this.BuffModifyHitPoints.HitPointsMultiplier = iHitPointsMultiplyer;
			this.BuffModifyHitPoints.HitPointsModifier = iHitPointsModifier;
			this.TTL = iTTL;
			this.VisualCategory = iVisualCategory;
		}

		// Token: 0x060019D0 RID: 6608 RVA: 0x000AD049 File Offset: 0x000AB249
		public BuffStorage(BuffModifySpellTTL iBuff, VisualCategory iVisualCategory, Vector3 iColor, float iTTLMultiplyer, float iTTLModifier)
		{
			this = new BuffStorage(iBuff, 0.5f, iVisualCategory, iColor, iTTLMultiplyer, iTTLModifier);
		}

		// Token: 0x060019D1 RID: 6609 RVA: 0x000AD060 File Offset: 0x000AB260
		public BuffStorage(BuffModifySpellTTL iBuff, float iTTL, VisualCategory iVisualCategory, Vector3 iColor, float iTTLMultiplyer, float iTTLModifier)
		{
			this = default(BuffStorage);
			this.UniqueID = BuffStorage.IDCounter++;
			this.Color = iColor;
			this.BuffType = BuffType.ModifySpellTTL;
			this.BuffModifySpellTTL = iBuff;
			this.BuffModifySpellTTL.TTLMultiplier = iTTLMultiplyer;
			this.BuffModifySpellTTL.TTLModifier = iTTLModifier;
			this.TTL = iTTL;
			this.VisualCategory = iVisualCategory;
		}

		// Token: 0x060019D2 RID: 6610 RVA: 0x000AD0C5 File Offset: 0x000AB2C5
		public BuffStorage(BuffModifySpellRange iBuff, VisualCategory iVisualCategory, Vector3 iColor, float iSpellRangeMultiplyer, float iSpellRangeModifier)
		{
			this = new BuffStorage(iBuff, 0.5f, iVisualCategory, iColor, iSpellRangeMultiplyer, iSpellRangeModifier);
		}

		// Token: 0x060019D3 RID: 6611 RVA: 0x000AD0DC File Offset: 0x000AB2DC
		public BuffStorage(BuffModifySpellRange iBuff, float iTTL, VisualCategory iVisualCategory, Vector3 iColor, float iSpellRangeMultiplyer, float iSpellRangeModifier)
		{
			this = default(BuffStorage);
			this.UniqueID = BuffStorage.IDCounter++;
			this.Color = iColor;
			this.BuffType = BuffType.ModifyHitPoints;
			this.BuffModifySpellRange = iBuff;
			this.BuffModifySpellRange.RangeMultiplier = iSpellRangeMultiplyer;
			this.BuffModifySpellRange.RangeModifier = iSpellRangeModifier;
			this.TTL = iTTL;
			this.VisualCategory = iVisualCategory;
		}

		// Token: 0x060019D4 RID: 6612 RVA: 0x000AD144 File Offset: 0x000AB344
		public void Execute(Character iOwner, float iDeltatime)
		{
			this.TTL -= iDeltatime;
			switch (this.BuffType)
			{
			case BuffType.BoostDamage:
				this.BuffBoostDamage.Execute(iOwner);
				return;
			case BuffType.DealDamage:
				this.BuffDealDamage.Execute(iOwner, iDeltatime);
				return;
			case BuffType.Resistance:
				this.BuffResistance.Execute(iOwner);
				return;
			case BuffType.Undying:
				this.BuffUndying.Execute(iOwner);
				return;
			case BuffType.Boost:
				this.BuffBoost.Execute(iOwner);
				return;
			case BuffType.ReduceAgro:
				this.BuffReduceAgro.Execute(iOwner);
				return;
			case BuffType.ModifyHitPoints:
				this.BuffModifyHitPoints.Execute(iOwner);
				return;
			case BuffType.ModifySpellTTL:
				this.BuffModifySpellTTL.Execute(iOwner);
				return;
			case BuffType.ModifySpellRange:
				this.BuffModifySpellRange.Execute(iOwner);
				return;
			default:
				return;
			}
		}

		// Token: 0x04001C06 RID: 7174
		private static uint IDCounter;

		// Token: 0x04001C07 RID: 7175
		[FieldOffset(0)]
		public readonly uint UniqueID;

		// Token: 0x04001C08 RID: 7176
		[FieldOffset(4)]
		public BuffType BuffType;

		// Token: 0x04001C09 RID: 7177
		[FieldOffset(5)]
		public VisualCategory VisualCategory;

		// Token: 0x04001C0A RID: 7178
		[FieldOffset(8)]
		public Vector3 Color;

		// Token: 0x04001C0B RID: 7179
		[FieldOffset(20)]
		public float TTL;

		// Token: 0x04001C0C RID: 7180
		[FieldOffset(24)]
		public int Effect;

		// Token: 0x04001C0D RID: 7181
		[FieldOffset(28)]
		public bool SelfCasted;

		// Token: 0x04001C0E RID: 7182
		[FieldOffset(32)]
		public BuffBoostDamage BuffBoostDamage;

		// Token: 0x04001C0F RID: 7183
		[FieldOffset(32)]
		public BuffDealDamage BuffDealDamage;

		// Token: 0x04001C10 RID: 7184
		[FieldOffset(32)]
		public BuffResistance BuffResistance;

		// Token: 0x04001C11 RID: 7185
		[FieldOffset(32)]
		public BuffUndying BuffUndying;

		// Token: 0x04001C12 RID: 7186
		[FieldOffset(32)]
		public BuffBoost BuffBoost;

		// Token: 0x04001C13 RID: 7187
		[FieldOffset(32)]
		public BuffReduceAgro BuffReduceAgro;

		// Token: 0x04001C14 RID: 7188
		[FieldOffset(32)]
		public BuffModifyHitPoints BuffModifyHitPoints;

		// Token: 0x04001C15 RID: 7189
		[FieldOffset(32)]
		public BuffModifySpellTTL BuffModifySpellTTL;

		// Token: 0x04001C16 RID: 7190
		[FieldOffset(32)]
		public BuffModifySpellRange BuffModifySpellRange;
	}
}
