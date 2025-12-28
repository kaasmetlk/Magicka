using System;
using System.Runtime.InteropServices;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.GameStates;

namespace Magicka.GameLogic.Spells
{
	// Token: 0x020002EC RID: 748
	[StructLayout(LayoutKind.Explicit)]
	public struct Magick
	{
		// Token: 0x060016E9 RID: 5865 RVA: 0x000937B2 File Offset: 0x000919B2
		public static bool IsInstant(MagickType iType)
		{
			return Magick.INSTANT_LOOKUP[(int)iType];
		}

		// Token: 0x060016EA RID: 5866 RVA: 0x000937BC File Offset: 0x000919BC
		public static void InitializeMagicks(PlayState iPlayState)
		{
			Haste.InitializeCache(16);
			Revive.InitializeCache(16);
			Rain instance = Rain.Instance;
			Thunderbolt instance2 = Thunderbolt.Instance;
			Teleport instance3 = Teleport.Instance;
			MeteorShower instance4 = MeteorShower.Instance;
			Charm instance5 = Charm.Instance;
			SummonSpirit.InitializeCache(iPlayState);
			SummonZombie.InitializeCache(16, iPlayState);
			SummonUndead.InitializeCache(iPlayState);
			Conflagration.InitializeCache(16);
			SummonPhoenix instance6 = SummonPhoenix.Instance;
			Fear instance7 = Fear.Instance;
			Invisibility instance8 = Invisibility.Instance;
			Thunderstorm instance9 = Thunderstorm.Instance;
			TimeWarp instance10 = TimeWarp.Instance;
			Blizzard instance11 = Blizzard.Instance;
			Tornado instance12 = Tornado.Instance;
			SpawnSlime instance13 = SpawnSlime.Instance;
			SpawnSlimeOverkill instance14 = SpawnSlimeOverkill.Instance;
			GreaseSplash instance15 = GreaseSplash.Instance;
			EtherealClone instance16 = EtherealClone.Instance;
			SpawnSlime.InitializeCache(iPlayState);
			EtherealClone.InitializeCache(iPlayState);
			TornadoEntity.InitializeCache(16);
			Wave.InitializeCache(16);
			PerformanceEnchantment.InitializeCache(16);
			Grease.InitializeCache(8, iPlayState);
			GreaseLump.InitializeCache(8, iPlayState);
			BreakBarriers.InitializeCache(8, iPlayState);
			StopCharge.InitializeCache(8, iPlayState);
			HomingCharge.InitializeCache(8, iPlayState);
			GreaseTrail.InitializeCache(4, iPlayState);
			FloorStomp.InitializeCache(4, iPlayState);
			Grease.GreaseField.InitializeCache(72, iPlayState);
			VortexEntity.InitializeCache(8, iPlayState);
			SummonElemental.Instance.SetTemplate(iPlayState.Content.Load<CharacterTemplate>("data/characters/elemental"));
			SummonDeath.Instance.Initialize(iPlayState);
			Corporealize instance17 = Corporealize.Instance;
			Nullify instance18 = Nullify.Instance;
			SummonBug.InitialzeCache(iPlayState);
			Napalm instance19 = Napalm.Instance;
			CTD instance20 = CTD.Instance;
			Portal.Instance.Initialize(iPlayState);
			TractorPull instance21 = TractorPull.Instance;
			ProppMagick instance22 = ProppMagick.Instance;
			Levitate instance23 = Levitate.Instance;
			SummonFlamer.InitializeCache(iPlayState);
			Confuse.InitializeCache(16);
			Etherealize instance24 = Etherealize.Instance;
			EarthQuake instance25 = EarthQuake.Instance;
			Grow.InitializeCache(16);
			Shrink.InitializeCache(16);
			Polymorph.InitializeCache(32, iPlayState);
			Zap.InitializeCache(8);
			VladZap.InitializeCache(8);
			SummonCross.InitializeCache(16, iPlayState);
		}

		// Token: 0x170005CA RID: 1482
		// (get) Token: 0x060016EB RID: 5867 RVA: 0x00093958 File Offset: 0x00091B58
		public SpecialAbility Effect
		{
			get
			{
				switch (this.MagickType)
				{
				case MagickType.Revive:
					return Revive.GetInstance();
				case MagickType.Grease:
					return Grease.GetInstance();
				case MagickType.Haste:
					return Haste.GetInstance();
				case MagickType.Invisibility:
					return Invisibility.Instance;
				case MagickType.Teleport:
					return Teleport.Instance;
				case MagickType.Fear:
					return Fear.Instance;
				case MagickType.Charm:
					return Charm.Instance;
				case MagickType.ThunderB:
					return Thunderbolt.Instance;
				case MagickType.Rain:
					return Rain.Instance;
				case MagickType.Tornado:
					return Tornado.Instance;
				case MagickType.Blizzard:
					return Blizzard.Instance;
				case MagickType.MeteorS:
					return MeteorShower.Instance;
				case MagickType.Conflagration:
					return Conflagration.GetInstance();
				case MagickType.ThunderS:
					return Thunderstorm.Instance;
				case MagickType.TimeWarp:
					return TimeWarp.Instance;
				case MagickType.Vortex:
					return Vortex.Instance;
				case MagickType.SUndead:
					return SummonZombie.GetInstance();
				case MagickType.SElemental:
					return SummonElemental.Instance;
				case MagickType.SDeath:
					return SummonDeath.Instance;
				case MagickType.SPhoenix:
					return SummonPhoenix.Instance;
				case MagickType.Nullify:
					return Nullify.Instance;
				case MagickType.Corporealize:
					return Corporealize.Instance;
				case MagickType.CTD:
					return CTD.Instance;
				case MagickType.Napalm:
					return Napalm.Instance;
				case MagickType.Portal:
					return Portal.Instance;
				case MagickType.TractorPull:
					return TractorPull.Instance;
				case MagickType.ProppMagick:
					return ProppMagick.Instance;
				case MagickType.Levitate:
					return Levitate.Instance;
				case MagickType.ChainLightning:
					return Zap.GetInstance();
				case MagickType.Confuse:
					return Confuse.GetInstance();
				case MagickType.Wave:
					return Wave.GetInstance();
				case MagickType.PerformanceEnchantment:
					return PerformanceEnchantment.GetInstance();
				case MagickType.JudgementSpray:
					return JudgementSpray.Instance;
				case MagickType.Amalgameddon:
					return Amalgameddon.Instance;
				case MagickType.Grow:
					return Grow.GetInstance();
				case MagickType.Shrink:
					return Shrink.GetInstance();
				case MagickType.Etherealize:
					return Etherealize.Instance;
				case MagickType.Polymorph:
					return Polymorph.GetInstance();
				case MagickType.Earthquake:
					return EarthQuake.Instance;
				case MagickType.Votal:
					return Votal.Instance;
				case MagickType.SummonCross:
					return SummonCross.GetInstance();
				case MagickType.VladZap:
					return VladZap.GetInstance();
				case MagickType.SpawnSlime:
					return SpawnSlime.Instance;
				case MagickType.SpawnSlimeOverkill:
					return SpawnSlimeOverkill.Instance;
				case MagickType.GreaseSplash:
					return GreaseSplash.Instance;
				case MagickType.EtherealClone:
					return EtherealClone.Instance;
				}
				throw new NotImplementedException(this.MagickType.ToString() + " is not implemented.");
			}
		}

		// Token: 0x04001883 RID: 6275
		private static readonly bool[] INSTANT_LOOKUP = new bool[]
		{
			false,
			true,
			false,
			true,
			true,
			true,
			false,
			false,
			false,
			false,
			false,
			false,
			false,
			false,
			false,
			false,
			false,
			false,
			false,
			false,
			false,
			false,
			false,
			false,
			true,
			false,
			false,
			false,
			false,
			true,
			false,
			false,
			false,
			false,
			false,
			false
		};

		// Token: 0x04001884 RID: 6276
		public static readonly int[] NAME_LOCALIZATION = new int[]
		{
			0,
			"#magick_revive".GetHashCodeCustom(),
			"#magick_grease".GetHashCodeCustom(),
			"#magick_haste".GetHashCodeCustom(),
			"#magick_invisibility".GetHashCodeCustom(),
			"#magick_teleport".GetHashCodeCustom(),
			"#magick_fear".GetHashCodeCustom(),
			"#magick_charm".GetHashCodeCustom(),
			"#magick_thunderb".GetHashCodeCustom(),
			"#magick_rain".GetHashCodeCustom(),
			"#magick_tornado".GetHashCodeCustom(),
			"#magick_blizzard".GetHashCodeCustom(),
			"#magick_meteors".GetHashCodeCustom(),
			"#magick_conflagration".GetHashCodeCustom(),
			"#magick_thunders".GetHashCodeCustom(),
			"#magick_timewarp".GetHashCodeCustom(),
			"#magick_vortex".GetHashCodeCustom(),
			"#magick_sundead".GetHashCodeCustom(),
			"#magick_selemental".GetHashCodeCustom(),
			"#magick_sdeath".GetHashCodeCustom(),
			"#magick_sphoenix".GetHashCodeCustom(),
			"#magick_nullify".GetHashCodeCustom(),
			"#magick_corporealize".GetHashCodeCustom(),
			"#magick_ctd".GetHashCodeCustom(),
			"#magick_napalm".GetHashCodeCustom(),
			"#magick_portal".GetHashCodeCustom(),
			"#magick_tractorpull".GetHashCodeCustom(),
			"#magick_proppmagick".GetHashCodeCustom(),
			"#magick_levitate".GetHashCodeCustom(),
			"#magick_chainlightning".GetHashCodeCustom(),
			"#magick_confuse".GetHashCodeCustom(),
			"#magick_wave".GetHashCodeCustom(),
			"#magick_performance".GetHashCodeCustom(),
			"#magick_judgementspray".GetHashCodeCustom(),
			"#magick_amalgameddon".GetHashCodeCustom(),
			"#Ent_Dungeons_Slime_Medium".GetHashCodeCustom(),
			"#Ent_Dungeons_Slime_Medium".GetHashCodeCustom(),
			"#magick_grease".GetHashCodeCustom(),
			"#magick_etherealize".GetHashCodeCustom()
		};

		// Token: 0x04001885 RID: 6277
		public static readonly int[] DESC_LOCALIZATION = new int[]
		{
			0,
			"#magick_revived".GetHashCodeCustom(),
			"#magick_greased".GetHashCodeCustom(),
			"#magick_hasted".GetHashCodeCustom(),
			"#magick_invisibilityd".GetHashCodeCustom(),
			"#magick_teleportd".GetHashCodeCustom(),
			"#magick_feard".GetHashCodeCustom(),
			"#magick_charmd".GetHashCodeCustom(),
			"#magick_thunderbd".GetHashCodeCustom(),
			"#magick_raind".GetHashCodeCustom(),
			"#magick_tornadod".GetHashCodeCustom(),
			"#magick_blizzardd".GetHashCodeCustom(),
			"#magick_meteorsd".GetHashCodeCustom(),
			"#magick_conflagrationd".GetHashCodeCustom(),
			"#magick_thundersd".GetHashCodeCustom(),
			"#magick_timewarpd".GetHashCodeCustom(),
			"#magick_vortexd".GetHashCodeCustom(),
			"#magick_sundeadd".GetHashCodeCustom(),
			"#magick_selementald".GetHashCodeCustom(),
			"#magick_sdeathd".GetHashCodeCustom(),
			"#magick_sphoenixd".GetHashCodeCustom(),
			"#magick_nullifyd".GetHashCodeCustom(),
			"#magick_corporealized".GetHashCodeCustom(),
			"#magick_ctdd".GetHashCodeCustom(),
			"#magick_napalmd".GetHashCodeCustom(),
			"#magick_portald".GetHashCodeCustom(),
			"#magick_tractorpulld".GetHashCodeCustom(),
			"#magick_proppmagickd".GetHashCodeCustom(),
			"#magick_levitated".GetHashCodeCustom(),
			"#magick_chainlightningd".GetHashCodeCustom(),
			"#magick_confused".GetHashCodeCustom(),
			"#magick_waved".GetHashCodeCustom(),
			"#magick_performanced".GetHashCodeCustom(),
			"#magick_judgementsprayd".GetHashCodeCustom(),
			"#magick_amalgameddond".GetHashCodeCustom(),
			"#Ent_Dungeons_Slime_Medium".GetHashCodeCustom(),
			"#Ent_Dungeons_Slime_Medium".GetHashCodeCustom(),
			"#magick_grease".GetHashCodeCustom()
		};

		// Token: 0x04001886 RID: 6278
		[FieldOffset(0)]
		public Elements Element;

		// Token: 0x04001887 RID: 6279
		[FieldOffset(4)]
		public MagickType MagickType;
	}
}
