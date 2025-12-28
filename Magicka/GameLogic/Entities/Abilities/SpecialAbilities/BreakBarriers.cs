using System;
using System.Collections.Generic;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000319 RID: 793
	public class BreakBarriers : SpecialAbility, IAbilityEffect
	{
		// Token: 0x06001858 RID: 6232 RVA: 0x000A16E8 File Offset: 0x0009F8E8
		public static BreakBarriers GetInstance()
		{
			if (BreakBarriers.sCache.Count > 0)
			{
				BreakBarriers result = BreakBarriers.sCache[BreakBarriers.sCache.Count - 1];
				BreakBarriers.sCache.RemoveAt(BreakBarriers.sCache.Count - 1);
				return result;
			}
			return new BreakBarriers();
		}

		// Token: 0x06001859 RID: 6233 RVA: 0x000A1738 File Offset: 0x0009F938
		public static void InitializeCache(int iNr, PlayState iPlayState)
		{
			BreakBarriers.sCache = new List<BreakBarriers>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				BreakBarriers.sCache.Add(new BreakBarriers());
			}
		}

		// Token: 0x0600185A RID: 6234 RVA: 0x000A176B File Offset: 0x0009F96B
		public BreakBarriers(Animations iAnimation) : base(iAnimation, "#magick_grease".GetHashCodeCustom())
		{
		}

		// Token: 0x0600185B RID: 6235 RVA: 0x000A1789 File Offset: 0x0009F989
		private BreakBarriers() : base(Animations.None, "#magick_grease".GetHashCodeCustom())
		{
		}

		// Token: 0x0600185C RID: 6236 RVA: 0x000A17A7 File Offset: 0x0009F9A7
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new Exception("BarrierBreak needs an owner!");
		}

		// Token: 0x0600185D RID: 6237 RVA: 0x000A17B3 File Offset: 0x0009F9B3
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			this.mTTL = this.TIME;
			this.mPlayState = iPlayState;
			this.mOwner = (iOwner as Character);
			SpellManager.Instance.AddSpellEffect(this);
			return true;
		}

		// Token: 0x1700061D RID: 1565
		// (get) Token: 0x0600185E RID: 6238 RVA: 0x000A17E9 File Offset: 0x0009F9E9
		public bool IsDead
		{
			get
			{
				return this.mTTL <= 0f;
			}
		}

		// Token: 0x0600185F RID: 6239 RVA: 0x000A17FC File Offset: 0x0009F9FC
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mTTL -= iDeltaTime;
			if (this.mOwner == null)
			{
				return;
			}
			List<Entity> entities = this.mPlayState.EntityManager.GetEntities(this.mOwner.Position, 3.5f * this.mOwner.Radius, false);
			for (int i = 0; i < entities.Count; i++)
			{
				if (entities[i] is Barrier || entities[i] is Shield)
				{
					entities[i].Kill();
				}
			}
			this.mPlayState.EntityManager.ReturnEntityList(entities);
		}

		// Token: 0x06001860 RID: 6240 RVA: 0x000A1898 File Offset: 0x0009FA98
		public void OnRemove()
		{
			this.mTTL = 0f;
			EffectManager.Instance.Stop(ref this.mEffect);
			BreakBarriers.sCache.Add(this);
		}

		// Token: 0x04001A1E RID: 6686
		private static List<BreakBarriers> sCache;

		// Token: 0x04001A1F RID: 6687
		private Character mOwner;

		// Token: 0x04001A20 RID: 6688
		public PlayState mPlayState;

		// Token: 0x04001A21 RID: 6689
		private VisualEffectReference mEffect;

		// Token: 0x04001A22 RID: 6690
		private float TIME = 5f;

		// Token: 0x04001A23 RID: 6691
		private float mTTL;
	}
}
