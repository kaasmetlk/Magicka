using System;
using System.Collections.Generic;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x0200042D RID: 1069
	public class Haste : SpecialAbility, IAbilityEffect
	{
		// Token: 0x0600211F RID: 8479 RVA: 0x000EBC40 File Offset: 0x000E9E40
		public static Haste GetInstance()
		{
			if (Haste.sCache.Count > 0)
			{
				Haste haste = Haste.sCache[Haste.sCache.Count - 1];
				Haste.sCache.RemoveAt(Haste.sCache.Count - 1);
				Haste.sActiveHastes.Add(haste);
				return haste;
			}
			Haste haste2 = new Haste();
			Haste.sActiveHastes.Add(haste2);
			return haste2;
		}

		// Token: 0x06002120 RID: 8480 RVA: 0x000EBCA8 File Offset: 0x000E9EA8
		public static void InitializeCache(int iNr)
		{
			Haste.sCache = new List<Haste>(iNr);
			Haste.sActiveHastes = new List<Haste>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				Haste.sCache.Add(new Haste());
			}
		}

		// Token: 0x06002121 RID: 8481 RVA: 0x000EBCE6 File Offset: 0x000E9EE6
		private Haste() : base(Animations.cast_magick_self, "#magick_haste".GetHashCodeCustom())
		{
		}

		// Token: 0x17000814 RID: 2068
		// (get) Token: 0x06002122 RID: 8482 RVA: 0x000EBCFA File Offset: 0x000E9EFA
		// (set) Token: 0x06002123 RID: 8483 RVA: 0x000EBD02 File Offset: 0x000E9F02
		public float CustomTTL
		{
			get
			{
				return this.mCustomTTL;
			}
			set
			{
				this.mCustomTTL = value;
				this.mOverrideTTL = true;
			}
		}

		// Token: 0x06002124 RID: 8484 RVA: 0x000EBD12 File Offset: 0x000E9F12
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			return this.Execute(iOwner, iPlayState, true);
		}

		// Token: 0x06002125 RID: 8485 RVA: 0x000EBD1D File Offset: 0x000E9F1D
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new Exception("Haste have to be cast by a character!");
		}

		// Token: 0x06002126 RID: 8486 RVA: 0x000EBD2C File Offset: 0x000E9F2C
		public bool Execute(ISpellCaster iOwner, PlayState iPlayState, bool iPlaySound)
		{
			base.Execute(iOwner, iPlayState);
			if (!(iOwner is Character))
			{
				this.OnRemove();
				return false;
			}
			for (int i = 0; i < Haste.sActiveHastes.Count; i++)
			{
				if (Haste.sActiveHastes[i].mOwner == iOwner)
				{
					if (this.mOverrideTTL)
					{
						Haste.sActiveHastes[i].mTTL = Math.Max(this.mCustomTTL, Haste.sActiveHastes[i].mTTL);
					}
					else
					{
						Haste.sActiveHastes[i].mTTL = 10f;
					}
					if (iPlaySound)
					{
						AudioManager.Instance.PlayCue(Banks.Spells, Haste.SOUNDHASH, iOwner.AudioEmitter);
					}
					this.OnRemove();
					return true;
				}
			}
			this.mOwner = (iOwner as Character);
			if (this.mOverrideTTL)
			{
				this.mTTL = this.mCustomTTL;
			}
			else
			{
				this.mTTL = 10f;
			}
			if (this.mOwner != null)
			{
				if (iPlaySound)
				{
					AudioManager.Instance.PlayCue(Banks.Spells, Haste.SOUNDHASH, this.mOwner.AudioEmitter);
				}
				SpellManager.Instance.AddSpellEffect(this);
				return true;
			}
			this.OnRemove();
			return false;
		}

		// Token: 0x17000815 RID: 2069
		// (get) Token: 0x06002127 RID: 8487 RVA: 0x000EBE56 File Offset: 0x000EA056
		public bool IsDead
		{
			get
			{
				return this.mTTL <= 0f;
			}
		}

		// Token: 0x06002128 RID: 8488 RVA: 0x000EBE68 File Offset: 0x000EA068
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mTTL -= iDeltaTime;
			if (this.mOwner.CharacterBody.IsBodyEnabled)
			{
				float num = (float)Math.Pow(0.5, (double)this.mTTL);
				this.mOwner.CharacterBody.SpeedMultiplier *= 1f + (1f - num);
				return;
			}
			this.mTTL = 0f;
		}

		// Token: 0x06002129 RID: 8489 RVA: 0x000EBEDC File Offset: 0x000EA0DC
		public void OnRemove()
		{
			this.mOverrideTTL = false;
			this.mOwner = null;
			Haste.sActiveHastes.Remove(this);
			Haste.sCache.Add(this);
		}

		// Token: 0x040023B9 RID: 9145
		private const float TTL = 10f;

		// Token: 0x040023BA RID: 9146
		private static List<Haste> sCache;

		// Token: 0x040023BB RID: 9147
		private static List<Haste> sActiveHastes;

		// Token: 0x040023BC RID: 9148
		public static readonly int SOUNDHASH = "magick_haste".GetHashCodeCustom();

		// Token: 0x040023BD RID: 9149
		private Character mOwner;

		// Token: 0x040023BE RID: 9150
		private float mTTL;

		// Token: 0x040023BF RID: 9151
		private bool mOverrideTTL;

		// Token: 0x040023C0 RID: 9152
		private float mCustomTTL;
	}
}
