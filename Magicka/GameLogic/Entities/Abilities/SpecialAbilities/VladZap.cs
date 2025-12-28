using System;
using System.Collections.Generic;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000615 RID: 1557
	public class VladZap : SpecialAbility
	{
		// Token: 0x06002EA6 RID: 11942 RVA: 0x0017AB3C File Offset: 0x00178D3C
		public static VladZap GetInstance()
		{
			if (VladZap.sCache.Count > 0)
			{
				VladZap result = VladZap.sCache[VladZap.sCache.Count - 1];
				VladZap.sCache.RemoveAt(VladZap.sCache.Count - 1);
				return result;
			}
			return new VladZap();
		}

		// Token: 0x06002EA7 RID: 11943 RVA: 0x0017AB8C File Offset: 0x00178D8C
		public static void InitializeCache(int iNr)
		{
			VladZap.sCache = new List<VladZap>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				VladZap.sCache.Add(new VladZap());
			}
		}

		// Token: 0x06002EA8 RID: 11944 RVA: 0x0017ABBF File Offset: 0x00178DBF
		public VladZap(Animations iAnimation) : base(iAnimation, "#specab_lightbolt".GetHashCodeCustom())
		{
		}

		// Token: 0x06002EA9 RID: 11945 RVA: 0x0017ABDF File Offset: 0x00178DDF
		private VladZap() : base(Animations.cast_magick_direct, "#magick_vladzap".GetHashCodeCustom())
		{
		}

		// Token: 0x06002EAA RID: 11946 RVA: 0x0017AC00 File Offset: 0x00178E00
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			this.mHitList.Clear();
			if (!iOwner.HasStatus(StatusEffects.Wet))
			{
				this.mHitList.Add(iOwner);
			}
			DamageCollection5 damageCollection = default(DamageCollection5);
			damageCollection.AddDamage(new Damage
			{
				Amount = 850f,
				AttackProperty = AttackProperties.Damage,
				Element = Elements.Lightning,
				Magnitude = 1f
			});
			Vector3 position = iOwner.Position;
			List<Entity> entities = iOwner.PlayState.EntityManager.GetEntities(position, 12f, false);
			List<VladCharacter> list = new List<VladCharacter>();
			foreach (Entity entity in entities)
			{
				VladCharacter vladCharacter = entity as VladCharacter;
				if (vladCharacter != null)
				{
					list.Add(vladCharacter);
				}
			}
			int count = list.Count;
			if (count == 3)
			{
				for (int i = 0; i < count; i++)
				{
					VladCharacter vladCharacter2 = list[i];
					int index = (i + 1) % count;
					VladCharacter iTarget = list[index];
					LightningBolt lightning = LightningBolt.GetLightning();
					lightning.Cast(iOwner, vladCharacter2.CastSource.Translation, iTarget, this.mHitList, Spell.LIGHTNINGCOLOR * 2f, 1f, 15f, ref damageCollection, iPlayState);
				}
			}
			iOwner.PlayState.EntityManager.ReturnEntityList(entities);
			AudioManager.Instance.PlayCue(Banks.Spells, VladZap.SOUND, iOwner.AudioEmitter);
			iPlayState.Camera.CameraShake(iOwner.Position, 1.5f, 0.333f);
			return true;
		}

		// Token: 0x06002EAB RID: 11947 RVA: 0x0017ADC0 File Offset: 0x00178FC0
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			return false;
		}

		// Token: 0x040032C5 RID: 12997
		private const float RANGE = 15f;

		// Token: 0x040032C6 RID: 12998
		private static List<VladZap> sCache;

		// Token: 0x040032C7 RID: 12999
		private HitList mHitList = new HitList(32);

		// Token: 0x040032C8 RID: 13000
		public static readonly int SOUND = "magick_thunderbolt".GetHashCodeCustom();
	}
}
