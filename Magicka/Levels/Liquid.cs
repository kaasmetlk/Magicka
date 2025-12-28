using System;
using JigLibX.Collision;
using JigLibX.Geometry;
using Magicka.GameLogic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.Levels
{
	// Token: 0x020003E2 RID: 994
	public abstract class Liquid
	{
		// Token: 0x06001E70 RID: 7792 RVA: 0x000D50D2 File Offset: 0x000D32D2
		protected Liquid(AnimatedLevelPart iParent)
		{
			this.mParent = iParent;
		}

		// Token: 0x1700076D RID: 1901
		// (get) Token: 0x06001E71 RID: 7793 RVA: 0x000D50E1 File Offset: 0x000D32E1
		public AnimatedLevelPart Parent
		{
			get
			{
				return this.mParent;
			}
		}

		// Token: 0x06001E72 RID: 7794 RVA: 0x000D50EC File Offset: 0x000D32EC
		public static Liquid Read(ContentReader iInput, LevelModel iLevel, AnimatedLevelPart iParent)
		{
			Effect effect = iInput.ReadObject<Effect>();
			if (effect is RenderDeferredLiquidEffect)
			{
				return new Water(effect as RenderDeferredLiquidEffect, iInput, iLevel, iParent);
			}
			if (effect is LavaEffect)
			{
				return new Lava(effect as LavaEffect, iInput, iLevel, iParent);
			}
			throw new NotImplementedException();
		}

		// Token: 0x06001E73 RID: 7795
		public abstract void Initialize();

		// Token: 0x06001E74 RID: 7796 RVA: 0x000D5134 File Offset: 0x000D3334
		public void Update(DataChannel iDataChannel, float iDeltaTime, Scene iScene)
		{
			Matrix identity = Matrix.Identity;
			this.Update(iDataChannel, iDeltaTime, iScene, ref identity, ref identity);
		}

		// Token: 0x06001E75 RID: 7797
		public abstract void Update(DataChannel iDataChannel, float iDeltaTime, Scene iScene, ref Matrix iTransform, ref Matrix iInvTransform);

		// Token: 0x06001E76 RID: 7798
		public abstract bool SegmentIntersect(out float frac, out Vector3 pos, out Vector3 normal, ref Segment seg, bool ignoreBackfaces, bool ignoreWater, bool ignoreIce);

		// Token: 0x06001E77 RID: 7799
		protected abstract void Freeze(ref Vector3 iOrigin, ref Vector3 iDirection, float iSpread, float iMagnitude);

		// Token: 0x06001E78 RID: 7800
		public abstract void FreezeAll(float iAmount);

		// Token: 0x1700076E RID: 1902
		// (get) Token: 0x06001E79 RID: 7801
		public abstract CollisionSkin CollisionSkin { get; }

		// Token: 0x1700076F RID: 1903
		// (get) Token: 0x06001E7A RID: 7802
		internal abstract bool AutoFreeze { get; }

		// Token: 0x06001E7B RID: 7803 RVA: 0x000D5154 File Offset: 0x000D3354
		public unsafe static void Freeze(GameScene iScene, ref Vector3 iOrigin, ref Vector3 iDirection, float iSpread, float iMultiplyer, ref Damage iDamage)
		{
			fixed (Damage* ptr = &iDamage)
			{
				Liquid.Freeze(iScene, ref iOrigin, ref iDirection, iSpread, iMultiplyer, ptr, 1);
			}
		}

		// Token: 0x06001E7C RID: 7804 RVA: 0x000D5178 File Offset: 0x000D3378
		public unsafe static void Freeze(GameScene iScene, ref Vector3 iOrigin, ref Vector3 iDirection, float iSpread, float iMultiplyer, ref DamageCollection5 iDamage)
		{
			fixed (Damage* ptr = &iDamage.A)
			{
				Liquid.Freeze(iScene, ref iOrigin, ref iDirection, iSpread, iMultiplyer, ptr, 5);
			}
		}

		// Token: 0x06001E7D RID: 7805 RVA: 0x000D51A0 File Offset: 0x000D33A0
		public unsafe static void Freeze(GameScene iScene, ref Vector3 iOrigin, ref Vector3 iDirection, float iSpread, float iMultiplyer, Damage* iDamage, int iNrOfDamages)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			for (int i = 0; i < iNrOfDamages; i++)
			{
				if ((iDamage[i].Element & Elements.Cold) != Elements.None)
				{
					num += iDamage[i].Magnitude;
				}
				if ((iDamage[i].Element & Elements.Water) != Elements.None)
				{
					num2 += iDamage[i].Magnitude;
				}
				if ((iDamage[i].Element & Elements.Fire) != Elements.None)
				{
					num3 += iDamage[i].Magnitude;
				}
			}
			if (iScene == null)
			{
				return;
			}
			if (iScene.Liquids == null)
			{
				return;
			}
			for (int j = 0; j < iScene.Liquids.Length; j++)
			{
				Liquid liquid = iScene.Liquids[j];
				float num4 = num - num3;
				if (liquid is Lava)
				{
					num4 += num2;
				}
				float num5 = 1f - (float)Math.Pow(0.75, (double)(Math.Abs(num4) * 3f));
				num4 = num5 * 3f * (float)Math.Sign(num4) * iMultiplyer;
				liquid.Freeze(ref iOrigin, ref iDirection, iSpread, num4);
			}
		}

		// Token: 0x040020CE RID: 8398
		protected AnimatedLevelPart mParent;
	}
}
