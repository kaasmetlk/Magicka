using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Buffs
{
	// Token: 0x020002FE RID: 766
	[StructLayout(LayoutKind.Explicit)]
	public struct AuraStorage
	{
		// Token: 0x06001798 RID: 6040 RVA: 0x0009B3FC File Offset: 0x000995FC
		public AuraStorage(AuraBuff iAura, AuraTarget iAuraTarget, AuraType iAuraType, int iEffect, float iTTL, float iRadius, VisualCategory iVisualCategory, Vector3 iColor, int[] iTargetType, Factions iTargetFaction)
		{
			this = default(AuraStorage);
			this.Color = iColor;
			this.AuraBuff = iAura;
			this.AuraTarget = iAuraTarget;
			this.AuraType = iAuraType;
			this.Effect = iEffect;
			this.TTL = iTTL;
			this.Radius = iRadius;
			this.VisualCategory = iVisualCategory;
			this.TargetType = iTargetType;
			this.TargetFaction = iTargetFaction;
		}

		// Token: 0x06001799 RID: 6041 RVA: 0x0009B460 File Offset: 0x00099660
		public AuraStorage(AuraDeflect iAura, AuraTarget iAuraTarget, AuraType iAuraType, int iEffect, float iTTL, float iRadius, VisualCategory iVisualCategory, Vector3 iColor, int[] iTargetType, Factions iTargetFaction)
		{
			this = default(AuraStorage);
			this.Color = iColor;
			this.AuraDeflect = iAura;
			this.AuraTarget = iAuraTarget;
			this.AuraType = iAuraType;
			this.Effect = iEffect;
			this.TTL = iTTL;
			this.Radius = iRadius;
			this.VisualCategory = iVisualCategory;
			this.TargetType = iTargetType;
			this.TargetFaction = iTargetFaction;
		}

		// Token: 0x0600179A RID: 6042 RVA: 0x0009B4C4 File Offset: 0x000996C4
		public AuraStorage(AuraBoost iAura, AuraTarget iAuraTarget, AuraType iAuraType, int iEffect, float iTTL, float iRadius, VisualCategory iVisualCategory, Vector3 iColor, int[] iTargetType, Factions iTargetFaction)
		{
			this = default(AuraStorage);
			this.Color = iColor;
			this.AuraBoost = iAura;
			this.AuraTarget = iAuraTarget;
			this.AuraType = iAuraType;
			this.Effect = iEffect;
			this.TTL = iTTL;
			this.Radius = iRadius;
			this.VisualCategory = iVisualCategory;
			this.TargetType = iTargetType;
			this.TargetFaction = iTargetFaction;
		}

		// Token: 0x0600179B RID: 6043 RVA: 0x0009B528 File Offset: 0x00099728
		public AuraStorage(AuraLifeSteal iAura, AuraTarget iAuraTarget, AuraType iAuraType, int iEffect, float iTTL, float iRadius, VisualCategory iVisualCategory, Vector3 iColor, int[] iTargetType, Factions iTargetFaction)
		{
			this = default(AuraStorage);
			this.Color = iColor;
			this.AuraLifeSteal = iAura;
			this.AuraTarget = iAuraTarget;
			this.AuraType = iAuraType;
			this.Effect = iEffect;
			this.TTL = iTTL;
			this.Radius = iRadius;
			this.VisualCategory = iVisualCategory;
			this.TargetType = iTargetType;
			this.TargetFaction = iTargetFaction;
		}

		// Token: 0x0600179C RID: 6044 RVA: 0x0009B58C File Offset: 0x0009978C
		public AuraStorage(AuraLove iAura, AuraTarget iAuraTarget, AuraType iAuraType, int iEffect, float iTTL, float iRadius, VisualCategory iVisualCategory, Vector3 iColor, int[] iTargetType, Factions iTargetFaction)
		{
			this = default(AuraStorage);
			this.Color = iColor;
			this.AuraLove = iAura;
			this.AuraTarget = iAuraTarget;
			this.AuraType = iAuraType;
			this.Effect = iEffect;
			this.TTL = iTTL;
			this.Radius = iRadius;
			this.VisualCategory = iVisualCategory;
			this.TargetType = iTargetType;
			this.TargetFaction = iTargetFaction;
		}

		// Token: 0x0600179D RID: 6045 RVA: 0x0009B5F0 File Offset: 0x000997F0
		public AuraStorage(ContentReader iInput)
		{
			this = default(AuraStorage);
			this.AuraTarget = (AuraTarget)iInput.ReadByte();
			this.AuraType = (AuraType)iInput.ReadByte();
			this.VisualCategory = (VisualCategory)iInput.ReadByte();
			this.Color = iInput.ReadVector3();
			string text = iInput.ReadString().ToLowerInvariant();
			if (text.Equals(""))
			{
				this.Effect = 0;
			}
			else
			{
				this.Effect = text.GetHashCodeCustom();
			}
			this.TTL = iInput.ReadSingle();
			this.Radius = iInput.ReadSingle();
			string text2 = iInput.ReadString().ToLowerInvariant();
			string[] array = text2.Split(new char[]
			{
				','
			});
			int num = array.Length;
			this.TargetType = new int[num];
			for (int i = 0; i < num; i++)
			{
				this.TargetType[i] = array[i].GetHashCodeCustom();
			}
			this.TargetFaction = (Factions)iInput.ReadInt32();
			switch (this.AuraType)
			{
			case AuraType.Buff:
				this.AuraBuff = new AuraBuff(iInput);
				return;
			case AuraType.Deflect:
				this.AuraDeflect = new AuraDeflect(iInput);
				return;
			case AuraType.Boost:
				this.AuraBoost = new AuraBoost(iInput);
				return;
			case AuraType.LifeSteal:
				this.AuraLifeSteal = new AuraLifeSteal(iInput);
				return;
			case AuraType.Love:
				this.AuraLove = new AuraLove(iInput);
				return;
			default:
				return;
			}
		}

		// Token: 0x0600179E RID: 6046 RVA: 0x0009B740 File Offset: 0x00099940
		public void Execute(Character iOwner, float iDeltaTime)
		{
			this.TTL -= iDeltaTime;
			switch (this.AuraType)
			{
			case AuraType.Buff:
				this.mPeriod -= iDeltaTime;
				while (this.mPeriod <= 0f)
				{
					this.mPeriod += 0.25f;
					this.AuraBuff.Execute(iOwner, this.AuraTarget, this.Effect, this.Radius, this.TargetType, this.TargetFaction);
				}
				return;
			case AuraType.Deflect:
				this.AuraDeflect.Execute(iOwner, iDeltaTime, this.AuraTarget, this.Effect, this.Radius);
				return;
			case AuraType.Boost:
				this.mPeriod -= iDeltaTime;
				while (this.mPeriod <= 0f)
				{
					this.mPeriod += 0.25f;
					this.AuraBoost.Execute(iOwner, this.AuraTarget, this.Effect, this.Radius);
				}
				return;
			case AuraType.LifeSteal:
				this.mPeriod -= iDeltaTime;
				while (this.mPeriod <= 0f)
				{
					this.mPeriod += 0.25f;
					this.AuraLifeSteal.Execute(iOwner, this.AuraTarget, this.Effect, this.Radius);
				}
				return;
			case AuraType.Love:
				this.mPeriod -= iDeltaTime;
				while (this.mPeriod <= 0f)
				{
					this.mPeriod += 0.25f;
					this.AuraLove.Execute(iOwner, this.AuraTarget, this.Effect, this.Radius);
				}
				return;
			default:
				return;
			}
		}

		// Token: 0x04001954 RID: 6484
		[FieldOffset(0)]
		public AuraTarget AuraTarget;

		// Token: 0x04001955 RID: 6485
		[FieldOffset(1)]
		public AuraType AuraType;

		// Token: 0x04001956 RID: 6486
		[FieldOffset(2)]
		public VisualCategory VisualCategory;

		// Token: 0x04001957 RID: 6487
		[FieldOffset(4)]
		public Vector3 Color;

		// Token: 0x04001958 RID: 6488
		[FieldOffset(16)]
		private float mPeriod;

		// Token: 0x04001959 RID: 6489
		[FieldOffset(20)]
		public float TTL;

		// Token: 0x0400195A RID: 6490
		[FieldOffset(24)]
		public int Effect;

		// Token: 0x0400195B RID: 6491
		[FieldOffset(28)]
		public float Radius;

		// Token: 0x0400195C RID: 6492
		[FieldOffset(32)]
		public int[] TargetType;

		// Token: 0x0400195D RID: 6493
		[FieldOffset(36)]
		public Factions TargetFaction;

		// Token: 0x0400195E RID: 6494
		[FieldOffset(40)]
		public AuraBuff AuraBuff;

		// Token: 0x0400195F RID: 6495
		[FieldOffset(40)]
		public AuraDeflect AuraDeflect;

		// Token: 0x04001960 RID: 6496
		[FieldOffset(40)]
		public AuraBoost AuraBoost;

		// Token: 0x04001961 RID: 6497
		[FieldOffset(40)]
		public AuraLifeSteal AuraLifeSteal;

		// Token: 0x04001962 RID: 6498
		[FieldOffset(40)]
		public AuraLove AuraLove;
	}
}
