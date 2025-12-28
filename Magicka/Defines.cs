using System;
using Magicka.GameLogic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Magicka
{
	// Token: 0x02000275 RID: 629
	public static class Defines
	{
		// Token: 0x060012A1 RID: 4769 RVA: 0x000732F7 File Offset: 0x000714F7
		public static bool FeatureDamage(Defines.DamageFeatures iFeatures)
		{
			return (byte)(iFeatures & Defines.DamageFeatures.Damage) == 1;
		}

		// Token: 0x060012A2 RID: 4770 RVA: 0x00073300 File Offset: 0x00071500
		public static bool FeatureKnockback(Defines.DamageFeatures iFeatures)
		{
			return (byte)(iFeatures & Defines.DamageFeatures.Knockback) == 2;
		}

		// Token: 0x060012A3 RID: 4771 RVA: 0x00073309 File Offset: 0x00071509
		public static bool FeatureNotify(Defines.DamageFeatures iFeatures)
		{
			return (byte)(iFeatures & Defines.DamageFeatures.Notify) == 4;
		}

		// Token: 0x060012A4 RID: 4772 RVA: 0x00073312 File Offset: 0x00071512
		public static bool FeatureEffects(Defines.DamageFeatures iFeatures)
		{
			return (byte)(iFeatures & Defines.DamageFeatures.Effects) == 8;
		}

		// Token: 0x060012A5 RID: 4773 RVA: 0x0007331C File Offset: 0x0007151C
		public static bool IsWeapon(WeaponClass iClass)
		{
			return iClass == WeaponClass.Thrust_Fast || iClass == WeaponClass.Thrust_Medium || iClass == WeaponClass.Thrust_Slow || iClass == WeaponClass.Crush_Fast || iClass == WeaponClass.Crush_Medium || iClass == WeaponClass.Crush_Slow || iClass == WeaponClass.Slash_Fast || iClass == WeaponClass.Slash_Medium || iClass == WeaponClass.Slash_Slow || iClass == WeaponClass.Throw_Fast || iClass == WeaponClass.Throw_Medium || iClass == WeaponClass.Throw_Slow || iClass == WeaponClass.Handgun || iClass == WeaponClass.Rifle || iClass == WeaponClass.Machinegun || iClass == WeaponClass.Heavy;
		}

		// Token: 0x060012A6 RID: 4774 RVA: 0x00073374 File Offset: 0x00071574
		public static int ElementIndex(Elements iElement)
		{
			if (iElement == Elements.All)
			{
				return 11;
			}
			return (int)(Math.Log((double)iElement) * Defines.ONEOVERLN2 + 0.5);
		}

		// Token: 0x060012A7 RID: 4775 RVA: 0x00073399 File Offset: 0x00071599
		public static Elements ElementFromIndex(int iIndex)
		{
			if (iIndex == 11)
			{
				return Elements.All;
			}
			return (Elements)(Math.Pow(2.0, (double)iIndex) + 0.5);
		}

		// Token: 0x060012A8 RID: 4776 RVA: 0x000733C4 File Offset: 0x000715C4
		internal static int DecalLimit()
		{
			switch (GlobalSettings.Instance.DecalLimit)
			{
			case SettingOptions.Low:
				return 150;
			case SettingOptions.High:
				return 600;
			}
			return 300;
		}

		// Token: 0x060012A9 RID: 4777 RVA: 0x00073404 File Offset: 0x00071604
		internal static float ParticleMultiplyer()
		{
			switch (GlobalSettings.Instance.Particles)
			{
			case SettingOptions.Low:
				return 0.5f;
			case SettingOptions.High:
				return 1f;
			}
			return 0.75f;
		}

		// Token: 0x060012AA RID: 4778 RVA: 0x00073444 File Offset: 0x00071644
		internal static int ParticleLimit()
		{
			switch (GlobalSettings.Instance.Particles)
			{
			case SettingOptions.Low:
				return 16384;
			case SettingOptions.High:
				return 32768;
			}
			return 24576;
		}

		// Token: 0x060012AB RID: 4779 RVA: 0x00073483 File Offset: 0x00071683
		public static int DamageTypeIndex(AttackProperties iDamageType)
		{
			return (int)(Math.Log((double)iDamageType) * Defines.ONEOVERLN2 + 0.5);
		}

		// Token: 0x0400138E RID: 5006
		public const string CONTENT_ROOT = "content";

		// Token: 0x0400138F RID: 5007
		public const float NETWORK_RESTING_TIME = 1f;

		// Token: 0x04001390 RID: 5008
		public const double SQRT2 = 1.4142135623730951;

		// Token: 0x04001391 RID: 5009
		public const double SQRT3 = 1.7320508075688772;

		// Token: 0x04001392 RID: 5010
		public const int SECOND = 10000000;

		// Token: 0x04001393 RID: 5011
		public const double dSECOND = 10000000.0;

		// Token: 0x04001394 RID: 5012
		private const float Col = 0.003921569f;

		// Token: 0x04001395 RID: 5013
		public const float FALLING_DAMAGE_POWER = -50f;

		// Token: 0x04001396 RID: 5014
		public const float FALLING_KNOCKDOWN_THRESHOLD = 100f;

		// Token: 0x04001397 RID: 5015
		public const float FALLING_DAMAGE_THRESHOLD = 0f;

		// Token: 0x04001398 RID: 5016
		public const float CHARACTERS_PER_SECONDS = 40f;

		// Token: 0x04001399 RID: 5017
		public const float UI_RADIUS = 64f;

		// Token: 0x0400139A RID: 5018
		public const float ICONS_HEIGHT_OFFSET = 44f;

		// Token: 0x0400139B RID: 5019
		public const int ICON_WIDTH = 50;

		// Token: 0x0400139C RID: 5020
		public const int ICON_HEIGHT = 50;

		// Token: 0x0400139D RID: 5021
		public const float TEXT_PUNCTUATION_PAUSE = 0.25f;

		// Token: 0x0400139E RID: 5022
		public const float PASSIVE_ABILITY_TIMER = 0.25f;

		// Token: 0x0400139F RID: 5023
		public const float PASSIVE_ABILITY_AREA_REGEN = 2f;

		// Token: 0x040013A0 RID: 5024
		public const float PASSIVE_ABILITY_AREA_DRAINLIFE = 4f;

		// Token: 0x040013A1 RID: 5025
		public const float PASSIVE_ABILITY_MELEE_BOOST = 1.5f;

		// Token: 0x040013A2 RID: 5026
		public const float PASSIVE_ABILITY_MELEE_BOOST_RANGE = 7f;

		// Token: 0x040013A3 RID: 5027
		public const float PASSIVE_ABILITY_AREA_REGEN_RANGE = 7f;

		// Token: 0x040013A4 RID: 5028
		public const float PASSIVE_ABILITY_AREA_DRAINLIFE_RANGE = 7f;

		// Token: 0x040013A5 RID: 5029
		public const float ITEM_DESPAWN_TIME = 20f;

		// Token: 0x040013A6 RID: 5030
		public const int SETTINGS_LUGGAGE_INTERVARL_HIGH = 15;

		// Token: 0x040013A7 RID: 5031
		public const int SETTINGS_LUGGAGE_INTERVARL_MEDIUM = 30;

		// Token: 0x040013A8 RID: 5032
		public const int SETTINGS_LUGGAGE_INTERVARL_LOW = 90;

		// Token: 0x040013A9 RID: 5033
		public const int SPELL_QUECAP = 5;

		// Token: 0x040013AA RID: 5034
		public const int NR_OF_ELEMENTS = 11;

		// Token: 0x040013AB RID: 5035
		public const int MAX_NR_OF_SUMMONS_PER_CHARACTER = 16;

		// Token: 0x040013AC RID: 5036
		public const int MAX_NR_OF_UNDEAD_SUMMONS = 1;

		// Token: 0x040013AD RID: 5037
		public const int MAX_NR_OF_FLAMER_SUMMONS = 4;

		// Token: 0x040013AE RID: 5038
		public const float SPELL_RAILGUN_BASE_TTL = 1f;

		// Token: 0x040013AF RID: 5039
		public const float SPELL_RAILGUN_MOD_TTL = 2f;

		// Token: 0x040013B0 RID: 5040
		public const float SPELL_STRENGTH_PUSH_BASE = 50f;

		// Token: 0x040013B1 RID: 5041
		public const float SPELL_STRENGTH_PUSH_VAR = 500f;

		// Token: 0x040013B2 RID: 5042
		public const float SPELL_STRENGTH_SHIELD = 500f;

		// Token: 0x040013B3 RID: 5043
		public const float SPELL_STRENGTH_BARRIER = 500f;

		// Token: 0x040013B4 RID: 5044
		public const float SPELL_STRENGTH_BARRIER_ELEMENTAL = 100f;

		// Token: 0x040013B5 RID: 5045
		public const float SPELL_ARMOR_EARTH = 90f;

		// Token: 0x040013B6 RID: 5046
		public const float SPELL_ARMOR_ICE = 50f;

		// Token: 0x040013B7 RID: 5047
		public const float SPELL_TIME_GROUND = 15f;

		// Token: 0x040013B8 RID: 5048
		public const float SPELL_TIME_PUSH = 0.25f;

		// Token: 0x040013B9 RID: 5049
		public const float SPELL_TIME_ICE = 0.35f;

		// Token: 0x040013BA RID: 5050
		public const float SPELL_TIME_SPRAY = 4f;

		// Token: 0x040013BB RID: 5051
		public const float SPELL_TIME_LIGHTNING = 1f;

		// Token: 0x040013BC RID: 5052
		public const int SPELL_AMOUNT_LIGHTNINGS = 8;

		// Token: 0x040013BD RID: 5053
		public const int SPELL_AMOUNT_ICE = 3;

		// Token: 0x040013BE RID: 5054
		public const float SELF_SHIELD_HEALTH = 500f;

		// Token: 0x040013BF RID: 5055
		public const float SELF_SHIELD_DECAY = 100f;

		// Token: 0x040013C0 RID: 5056
		public const float SELF_STONE_HEALTH = 1500f;

		// Token: 0x040013C1 RID: 5057
		public const float SELF_STONE_DECAY = 0f;

		// Token: 0x040013C2 RID: 5058
		public const float SELF_STONE_MODIFIER = 10f;

		// Token: 0x040013C3 RID: 5059
		public const float SELF_STONE_MULTIPLIER = 0.15f;

		// Token: 0x040013C4 RID: 5060
		public const float SELF_ICE_HEALTH = 900f;

		// Token: 0x040013C5 RID: 5061
		public const float SELF_ICE_DECAY = 0f;

		// Token: 0x040013C6 RID: 5062
		public const float SELF_ICE_MODIFIER = 70f;

		// Token: 0x040013C7 RID: 5063
		public const float SELF_ICE_MULTIPLIER = 0.05f;

		// Token: 0x040013C8 RID: 5064
		public const float SELF_LIGHTNING_RANGE = 5f;

		// Token: 0x040013C9 RID: 5065
		public const float SELF_LIGHTNING_TIME = 0.3f;

		// Token: 0x040013CA RID: 5066
		public const float DIFFICULTY_RESISTANCE_MODIFIER = 0.00666f;

		// Token: 0x040013CB RID: 5067
		public const float SPELL_ANGLE_PUSH = 0.5235988f;

		// Token: 0x040013CC RID: 5068
		public const float SPELL_ANGLE_SPRAY = 0.31415927f;

		// Token: 0x040013CD RID: 5069
		public const float SPELL_ANGLE_LIGHTNING = 0.7853982f;

		// Token: 0x040013CE RID: 5070
		public const float SPELL_RANGE_PROJECTILE_BASE = 25f;

		// Token: 0x040013CF RID: 5071
		public const float SPELL_RANGE_PROJECTILE_VAR = 85f;

		// Token: 0x040013D0 RID: 5072
		public const float SPELL_RANGE_RAILGUN = 15f;

		// Token: 0x040013D1 RID: 5073
		public const float SPELL_RANGE_SHIELD = 5.5f;

		// Token: 0x040013D2 RID: 5074
		public const float SPELL_RANGE_SHIELD_MIN = 3f;

		// Token: 0x040013D3 RID: 5075
		public const float SPELL_RANGE_BUBBLE = 1.3f;

		// Token: 0x040013D4 RID: 5076
		public const float SPELL_RANGE_RAILGUN_WIDTH = 0.75f;

		// Token: 0x040013D5 RID: 5077
		public const float SPELL_RANGE_PUSH = 1.5f;

		// Token: 0x040013D6 RID: 5078
		public const float SPELL_RANGE_LIGHTNING = 8f;

		// Token: 0x040013D7 RID: 5079
		public const float SPELL_RANGE_SPRAY = 10f;

		// Token: 0x040013D8 RID: 5080
		public const float SPELL_RANGE_AREA = 10f;

		// Token: 0x040013D9 RID: 5081
		public const float SPELL_RANGE_AREA_PUSH = 3.5f;

		// Token: 0x040013DA RID: 5082
		public const float SPELL_RANGE_AREA_LIGHTNING = 4f;

		// Token: 0x040013DB RID: 5083
		public const float SPELL_RANGE_AREA_PROJECTILE = 6f;

		// Token: 0x040013DC RID: 5084
		public const float SPELL_PROJECTILE_DANGER = 30f;

		// Token: 0x040013DD RID: 5085
		public const int SPELL_MISC_WATER_PUSH_THRESHOLD = 2;

		// Token: 0x040013DE RID: 5086
		public const float SPELL_MISC_LIGHTNING_LIGHT_INTENSITY = 1.5f;

		// Token: 0x040013DF RID: 5087
		public const float SPELL_MISC_EARTH_BOUNCE_THRESHOLD = 8f;

		// Token: 0x040013E0 RID: 5088
		public const float SPELL_MISC_LIGHTNING_DECAY = 1f;

		// Token: 0x040013E1 RID: 5089
		public const float BLOATTIME = 0.333f;

		// Token: 0x040013E2 RID: 5090
		public const int NR_OF_STATUSEFFECTS = 9;

		// Token: 0x040013E3 RID: 5091
		public const float STATUS_DRYING_TIME = 0.9f;

		// Token: 0x040013E4 RID: 5092
		public const float STATUS_POISON_TIME = 0.5f;

		// Token: 0x040013E5 RID: 5093
		public const float STATUS_BURN_TIME = 0.2f;

		// Token: 0x040013E6 RID: 5094
		public const float STATUS_COLD_TIME = 0.1f;

		// Token: 0x040013E7 RID: 5095
		public const float STATUS_WET_TIME = 0.033333335f;

		// Token: 0x040013E8 RID: 5096
		public const float STATUS_FROZEN_TIME = 0.25f;

		// Token: 0x040013E9 RID: 5097
		public const int STATUS_FROZEN_ARMOR = 50;

		// Token: 0x040013EA RID: 5098
		public const int STATUS_FROZEN_MAGNITUDE = 10;

		// Token: 0x040013EB RID: 5099
		public const float MAX_STATUS_MAGNITUDE = 3f;

		// Token: 0x040013EC RID: 5100
		public const float REGENERATION_TIMER = 1f;

		// Token: 0x040013ED RID: 5101
		public const float HITLIST_COOLDOWN = 0.25f;

		// Token: 0x040013EE RID: 5102
		public const float KNOCKBACK_ELEVATION = 0.6980619f;

		// Token: 0x040013EF RID: 5103
		public const float PUSH_ELEVATION = 0.17453292f;

		// Token: 0x040013F0 RID: 5104
		public const float BLOCKANGLE = 1.0995574f;

		// Token: 0x040013F1 RID: 5105
		public const int BARRIER_DAMAGE_OVER_TIME = 10;

		// Token: 0x040013F2 RID: 5106
		public const int WAVE_DAMAGE_OVER_TIME = 500;

		// Token: 0x040013F3 RID: 5107
		public const float DECALTIMEOUT = 60f;

		// Token: 0x040013F4 RID: 5108
		public const int GUI_HEIGHT = 64;

		// Token: 0x040013F5 RID: 5109
		public const int MAX_ENEMY_HEALTHBARS = 512;

		// Token: 0x040013F6 RID: 5110
		public const int MAX_SHADOW_BLOBS = 512;

		// Token: 0x040013F7 RID: 5111
		public const int SAVESLOTS = 3;

		// Token: 0x040013F8 RID: 5112
		public const int MAX_NAME_LENGTH = 15;

		// Token: 0x040013F9 RID: 5113
		public const float MAX_DRAW_LATANCY = 0.2f;

		// Token: 0x040013FA RID: 5114
		public const int MAXDYNAMICLIGHTS = 128;

		// Token: 0x040013FB RID: 5115
		public const float PICKUPRADIUS = 2.5f;

		// Token: 0x040013FC RID: 5116
		public const int MAXELEMENTS = 20;

		// Token: 0x040013FD RID: 5117
		public const float MAXBOOSTRANGE = 20f;

		// Token: 0x040013FE RID: 5118
		public const float LIGHTNING_FRIEDTIME = 0.1f;

		// Token: 0x040013FF RID: 5119
		public const int EVENT_CONDITION_DEFAULT_SIZE = 8;

		// Token: 0x04001400 RID: 5120
		public const float SELF_ELEMENTAL_AURA_TTL = 10f;

		// Token: 0x04001401 RID: 5121
		public const float SELF_ELEMENTAL_AURA_BASE_TTL = 15f;

		// Token: 0x04001402 RID: 5122
		public const float SELF_ELEMENTAL_AURA_MODIFIER_TTL = 5f;

		// Token: 0x04001403 RID: 5123
		public const float SELF_ELEMENTAL_AURA_RADIUS_MODIFIER = 1.5f;

		// Token: 0x04001404 RID: 5124
		public const float SELF_ELEMENTAL_AURA_RADIUS_BASE = 0f;

		// Token: 0x04001405 RID: 5125
		public const float SELF_ELEMENTAL_AURA_BUFF_RESTSTANCE_MULTIPLIER = 0f;

		// Token: 0x04001406 RID: 5126
		public const float BARRIER_DAMAGE_RADIUS_MODIFIER = 2f;

		// Token: 0x04001407 RID: 5127
		public const float BARRIER_ICE_EFFECT_TTL_MAGNITUDE_BASE = 1f;

		// Token: 0x04001408 RID: 5128
		public const float BARRIER_EARTH_EFFECT_TTL_MAGNITUDE_BASE = 5f;

		// Token: 0x04001409 RID: 5129
		public const float BARRIER_BLAST_RADIUS = 2.6f;

		// Token: 0x0400140A RID: 5130
		public const ulong VERSION1471 = 281492157038593UL;

		// Token: 0x0400140B RID: 5131
		public const ulong VERSION1460 = 281492156973056UL;

		// Token: 0x0400140C RID: 5132
		public const ulong VERSION1430 = 281492156776448UL;

		// Token: 0x0400140D RID: 5133
		public const ulong VERSION1420 = 281492156710912UL;

		// Token: 0x0400140E RID: 5134
		public const ulong VERSION1410 = 281492156645376UL;

		// Token: 0x0400140F RID: 5135
		public const ulong VERSION1403 = 281492156579843UL;

		// Token: 0x04001410 RID: 5136
		public const ulong VERSION1370 = 281487862071296UL;

		// Token: 0x04001411 RID: 5137
		public const ulong VERSION1362 = 281487862005762UL;

		// Token: 0x04001412 RID: 5138
		public const ulong VERSION1355 = 281487861940229UL;

		// Token: 0x04001413 RID: 5139
		public const ulong VERSION1354 = 281487861940228UL;

		// Token: 0x04001414 RID: 5140
		public const ulong VERSION1350 = 281487861940224UL;

		// Token: 0x04001415 RID: 5141
		public const ulong VERSION1334 = 281487861809156UL;

		// Token: 0x04001416 RID: 5142
		public const ulong VERSION1330 = 281487861809152UL;

		// Token: 0x04001417 RID: 5143
		public const ulong VERSION1290 = 281483567235072UL;

		// Token: 0x04001418 RID: 5144
		public static readonly VertexPositionTexture[] QUAD_TEX_VERTS_C = new VertexPositionTexture[]
		{
			new VertexPositionTexture(new Vector3(-0.5f, -0.5f, 0f), new Vector2(0f, 0f)),
			new VertexPositionTexture(new Vector3(0.5f, -0.5f, 0f), new Vector2(1f, 0f)),
			new VertexPositionTexture(new Vector3(0.5f, 0.5f, 0f), new Vector2(1f, 1f)),
			new VertexPositionTexture(new Vector3(-0.5f, 0.5f, 0f), new Vector2(0f, 1f))
		};

		// Token: 0x04001419 RID: 5145
		public static readonly VertexPositionTexture[] QUAD_TEX_VERTS_TL = new VertexPositionTexture[]
		{
			new VertexPositionTexture(new Vector3(0f, 0f, 0f), new Vector2(0f, 0f)),
			new VertexPositionTexture(new Vector3(1f, 0f, 0f), new Vector2(1f, 0f)),
			new VertexPositionTexture(new Vector3(1f, 1f, 0f), new Vector2(1f, 1f)),
			new VertexPositionTexture(new Vector3(0f, 1f, 0f), new Vector2(0f, 1f))
		};

		// Token: 0x0400141A RID: 5146
		public static readonly VertexPositionColor[] QUAD_COL_VERTS_C = new VertexPositionColor[]
		{
			new VertexPositionColor(new Vector3(-0.5f, -0.5f, 0f), Color.Black),
			new VertexPositionColor(new Vector3(0.5f, -0.5f, 0f), Color.Black),
			new VertexPositionColor(new Vector3(0.5f, 0.5f, 0f), Color.Black),
			new VertexPositionColor(new Vector3(-0.5f, 0.5f, 0f), Color.Black)
		};

		// Token: 0x0400141B RID: 5147
		public static readonly VertexPositionColor[] QUAD_COL_VERTS_TL = new VertexPositionColor[]
		{
			new VertexPositionColor(new Vector3(0f, 0f, 0f), Color.Black),
			new VertexPositionColor(new Vector3(1f, 0f, 0f), Color.Black),
			new VertexPositionColor(new Vector3(1f, 1f, 0f), Color.Black),
			new VertexPositionColor(new Vector3(0f, 1f, 0f), Color.Black)
		};

		// Token: 0x0400141C RID: 5148
		public static readonly int[] INTERACTSTRINGS = new int[]
		{
			0,
			"#interact_01".GetHashCodeCustom(),
			"#interact_02".GetHashCodeCustom(),
			"#interact_03".GetHashCodeCustom(),
			"#interact_04".GetHashCodeCustom(),
			"#interact_05".GetHashCodeCustom(),
			"#interact_06".GetHashCodeCustom(),
			"#interact_07".GetHashCodeCustom(),
			"#interact_08".GetHashCodeCustom(),
			"#interact_09".GetHashCodeCustom(),
			"#interact_10".GetHashCodeCustom(),
			"#interact_11".GetHashCodeCustom(),
			"#interact_12".GetHashCodeCustom()
		};

		// Token: 0x0400141D RID: 5149
		public static readonly int[] ELEMENT_STRINGS = new int[]
		{
			"#element_earth".GetHashCodeCustom(),
			"#element_water".GetHashCodeCustom(),
			"#element_cold".GetHashCodeCustom(),
			"#element_fire".GetHashCodeCustom(),
			"#element_lightning".GetHashCodeCustom(),
			"#element_arcane".GetHashCodeCustom(),
			"#element_life".GetHashCodeCustom(),
			"#element_shield".GetHashCodeCustom(),
			"#element_ice".GetHashCodeCustom(),
			"#element_steam".GetHashCodeCustom(),
			"#element_poison".GetHashCodeCustom()
		};

		// Token: 0x0400141E RID: 5150
		public static readonly int LOC_GEN_OK = "#add_menu_ok".GetHashCodeCustom();

		// Token: 0x0400141F RID: 5151
		public static readonly int LOC_GEN_CANCEL = "#add_menu_cancel".GetHashCodeCustom();

		// Token: 0x04001420 RID: 5152
		public static readonly int LOC_GEN_JOIN = "#add_menu_join".GetHashCodeCustom();

		// Token: 0x04001421 RID: 5153
		public static readonly int LOC_GEN_CHARSEL = "#add_menu_charsel".GetHashCodeCustom();

		// Token: 0x04001422 RID: 5154
		public static readonly int LOC_GEN_READY = "#add_menu_ready".GetHashCodeCustom();

		// Token: 0x04001423 RID: 5155
		public static readonly int LOC_GEN_YES = "#add_menu_yes".GetHashCodeCustom();

		// Token: 0x04001424 RID: 5156
		public static readonly int LOC_GEN_NO = "#add_menu_no".GetHashCodeCustom();

		// Token: 0x04001425 RID: 5157
		public static readonly int LOC_GEN_RUS = "#add_menu_rus".GetHashCodeCustom();

		// Token: 0x04001426 RID: 5158
		public static readonly int LOC_GEN_RESTART = "#add_menu_restart".GetHashCodeCustom();

		// Token: 0x04001427 RID: 5159
		public static readonly int LOC_GEN_RESTARTCHECKPOINT = "#add_menu_restartcheckpoint".GetHashCodeCustom();

		// Token: 0x04001428 RID: 5160
		public static readonly int LOC_GEN_RETRY = "#add_menu_retry".GetHashCodeCustom();

		// Token: 0x04001429 RID: 5161
		public static readonly int LOC_GEN_QUIT = "#add_menu_quit".GetHashCodeCustom();

		// Token: 0x0400142A RID: 5162
		public static readonly int LOC_GAME_VICTORY = "#add_game_victory".GetHashCodeCustom();

		// Token: 0x0400142B RID: 5163
		public static readonly int LOC_GAME_LEVCOMP = "#add_game_level_complete".GetHashCodeCustom();

		// Token: 0x0400142C RID: 5164
		public static readonly int LOC_GAME_DEFEATED = "#add_game_defeated".GetHashCodeCustom();

		// Token: 0x0400142D RID: 5165
		public static readonly int LOC_GAME_WINNER = "#add_game_winner".GetHashCodeCustom();

		// Token: 0x0400142E RID: 5166
		public static readonly int LOC_GAME_PRESSSTART = "#add_game_pressstart".GetHashCodeCustom();

		// Token: 0x0400142F RID: 5167
		public static readonly int LOC_GAME_VS_WINNER = "#add_vs_winner".GetHashCodeCustom();

		// Token: 0x04001430 RID: 5168
		public static readonly int LOC_GAME_CONTINUED = "#TITLE_dungeons_to_be_continued".GetHashCodeCustom();

		// Token: 0x04001431 RID: 5169
		public static readonly int MAXTIMELIMIT = 99;

		// Token: 0x04001432 RID: 5170
		public static readonly int MAXSCORELIMIT = 999;

		// Token: 0x04001433 RID: 5171
		public static readonly int MAXRESPAWNTIME = 60;

		// Token: 0x04001434 RID: 5172
		public static readonly float FOOTSTEP_WATER_OFFSET = 0.2f;

		// Token: 0x04001435 RID: 5173
		public static readonly int WATER_SPLASH_EFFECT = "water_splash".GetHashCodeCustom();

		// Token: 0x04001436 RID: 5174
		public static readonly int WATER_DROWN_EFFECT = "water_drown".GetHashCodeCustom();

		// Token: 0x04001437 RID: 5175
		public static readonly int LAVA_SPLASH_EFFECT = "lava_splash".GetHashCodeCustom();

		// Token: 0x04001438 RID: 5176
		public static readonly int LAVA_DROWN_EFFECT = "lava_drown".GetHashCodeCustom();

		// Token: 0x04001439 RID: 5177
		public static readonly double ONEOVERLN2 = 1.0 / Math.Log(2.0);

		// Token: 0x0400143A RID: 5178
		public static readonly Vector4 DIALOGUE_COLOR_DEFAULT = new Vector4(0.7037f, 0.5576f, 0.3202f, 1f);

		// Token: 0x0400143B RID: 5179
		public static readonly Vector4 MESSAGEBOX_COLOR_DEFAULT = new Vector4(0.1037f, 0.1576f, 0.0202f, 1f);

		// Token: 0x0400143C RID: 5180
		public static readonly Vector3[] PLAYERCOLORS = new Vector3[]
		{
			new Vector3(240f, 20f, 20f) * 0.003921569f,
			new Vector3(145f, 0f, 70f) * 0.003921569f,
			new Vector3(160f, 0f, 255f) * 0.003921569f,
			new Vector3(0f, 132f, 255f) * 0.003921569f,
			new Vector3(0f, 255f, 226f) * 0.003921569f,
			new Vector3(0f, 180f, 10f) * 0.003921569f,
			new Vector3(160f, 270f, 0f) * 0.003921569f,
			new Vector3(363f, 298f, 0f) * 0.003921569f,
			new Vector3(112f, 112f, 112f) * 0.003921569f,
			new Vector3(12f, 12f, 12f) * 0.003921569f,
			new Vector3(320f, 320f, 320f) * 0.003921569f,
			new Vector3(0f, 20f, 150f) * 0.003921569f,
			new Vector3(148f, 71f, 0f) * 0.003921569f,
			new Vector3(333f, 173f, 0f) * 0.003921569f,
			new Vector3(338f, 186f, 229f) * 0.003921569f
		};

		// Token: 0x0400143D RID: 5181
		public static int PLAYERCOLORS_UNLOCKED = 15;

		// Token: 0x0400143E RID: 5182
		public static readonly Vector3[] TEAMCOLORS = new Vector3[]
		{
			new Vector3(240f, 20f, 20f) * 0.003921569f,
			new Vector3(0f, 0f, 200f) * 0.003921569f
		};

		// Token: 0x0400143F RID: 5183
		public static readonly float[] ElementUIRadian = new float[]
		{
			4.31969f,
			2.7488937f,
			0.3926991f,
			5.8904862f,
			3.5342917f,
			1.1780972f,
			1.9634955f,
			5.105088f
		};

		// Token: 0x04001440 RID: 5184
		public static readonly int[] ChantEffects = new int[]
		{
			"elementeffect_earth".GetHashCodeCustom(),
			"elementeffect_water".GetHashCodeCustom(),
			"elementeffect_cold".GetHashCodeCustom(),
			"elementeffect_fire".GetHashCodeCustom(),
			"elementeffect_lightning".GetHashCodeCustom(),
			"elementeffect_arcane".GetHashCodeCustom(),
			"elementeffect_life".GetHashCodeCustom(),
			"elementeffect_shield".GetHashCodeCustom(),
			"elementeffect_ice".GetHashCodeCustom(),
			"elementeffect_steam".GetHashCodeCustom(),
			"elementeffect_poison".GetHashCodeCustom(),
			"elementeffect_cold".GetHashCodeCustom(),
			"elementeffect_cold".GetHashCodeCustom()
		};

		// Token: 0x04001441 RID: 5185
		public static readonly int[] PROJECTILE_TRAIL_BIG = new int[]
		{
			"projectile_earth4".GetHashCodeCustom(),
			"projectile_water4".GetHashCodeCustom(),
			"projectile_cold4".GetHashCodeCustom(),
			"projectile_fire4".GetHashCodeCustom(),
			"projectile_lightning".GetHashCodeCustom(),
			"projectile_arcane4".GetHashCodeCustom(),
			"projectile_life4".GetHashCodeCustom(),
			"projectile_fire4".GetHashCodeCustom(),
			"projectile_ice4".GetHashCodeCustom(),
			"projectile_steam4".GetHashCodeCustom(),
			"projectile_poison4".GetHashCodeCustom()
		};

		// Token: 0x04001442 RID: 5186
		public static readonly int[] PROJECTILE_TRAIL_SMALL = new int[]
		{
			"projectiletrail_earth".GetHashCodeCustom(),
			"projectiletrail_water".GetHashCodeCustom(),
			"projectiletrail_cold".GetHashCodeCustom(),
			"projectiletrail_fire".GetHashCodeCustom(),
			"projectiletrail_lightning".GetHashCodeCustom(),
			"projectiletrail_arcane".GetHashCodeCustom(),
			"projectiletrail_life".GetHashCodeCustom(),
			"projectiletrail_fire".GetHashCodeCustom(),
			"projectiletrail_ice".GetHashCodeCustom(),
			"projectiletrail_steam".GetHashCodeCustom(),
			"projectiletrail_poison".GetHashCodeCustom()
		};

		// Token: 0x04001443 RID: 5187
		public static readonly int[] PROJECTILE_HIT_SMALL = new int[]
		{
			"projectilehit_earth".GetHashCodeCustom(),
			"projectilehit_water".GetHashCodeCustom(),
			"projectilehit_cold".GetHashCodeCustom(),
			"projectilehit_fire".GetHashCodeCustom(),
			"projectilehit_lightning".GetHashCodeCustom(),
			"projectilehit_arcane".GetHashCodeCustom(),
			"projectilehit_life".GetHashCodeCustom(),
			"projectilehit_fire".GetHashCodeCustom(),
			"projectilehit_ice".GetHashCodeCustom(),
			"projectilehit_steam".GetHashCodeCustom(),
			"projectilehit_poison".GetHashCodeCustom()
		};

		// Token: 0x04001444 RID: 5188
		public static readonly int[] PROJECTILE_HIT_BIG = new int[]
		{
			"projectilehit_big_earth".GetHashCodeCustom(),
			"projectilehit_big_water".GetHashCodeCustom(),
			"projectilehit_big_cold".GetHashCodeCustom(),
			"projectilehit_big_fire".GetHashCodeCustom(),
			"projectilehit_big_lightning".GetHashCodeCustom(),
			"projectilehit_big_arcane".GetHashCodeCustom(),
			"projectilehit_big_life".GetHashCodeCustom(),
			"projectilehit_big_fire".GetHashCodeCustom(),
			"projectilehit_big_ice".GetHashCodeCustom(),
			"projectilehit_big_steam".GetHashCodeCustom(),
			"projectilehit_big_poison".GetHashCodeCustom()
		};

		// Token: 0x04001445 RID: 5189
		public static float SPELL_DAMAGE_BARRIER_LIGHTNING = 130f;

		// Token: 0x04001446 RID: 5190
		public static float SPELL_DAMAGE_BARRIER_ICE = 275f;

		// Token: 0x04001447 RID: 5191
		public static float SPELL_DAMAGE_BARRIER_LIFE = -600f;

		// Token: 0x04001448 RID: 5192
		public static float SPELL_DAMAGE_BARRIER_ARCANE = 225f;

		// Token: 0x04001449 RID: 5193
		public static float SPELL_DAMAGE_BARRIER_STEAM = 225f;

		// Token: 0x0400144A RID: 5194
		public static float SPELL_DAMAGE_BARRIER_FIRE = 225f;

		// Token: 0x0400144B RID: 5195
		public static float SPELL_DAMAGE_BARRIER_POISON = 225f;

		// Token: 0x0400144C RID: 5196
		public static float SPELL_DAMAGE_EARTH = 150f;

		// Token: 0x0400144D RID: 5197
		public static float SPELL_DAMAGE_ICEEARTH = 275f;

		// Token: 0x0400144E RID: 5198
		public static float SPELL_DAMAGE_ICE = 180f;

		// Token: 0x0400144F RID: 5199
		public static float SPELL_DAMAGE_LIGHTNING = 250f;

		// Token: 0x04001450 RID: 5200
		public static float SPELL_DAMAGE_LIFE = -180f;

		// Token: 0x04001451 RID: 5201
		public static float SPELL_DAMAGE_ARCANE = 225f;

		// Token: 0x04001452 RID: 5202
		public static float SPELL_DAMAGE_FIRE = 60f;

		// Token: 0x04001453 RID: 5203
		public static float SPELL_DAMAGE_COLD = 25f;

		// Token: 0x04001454 RID: 5204
		public static float SPELL_DAMAGE_STEAM = 280f;

		// Token: 0x04001455 RID: 5205
		public static float SPELL_STRENGTH_WATER = 70f;

		// Token: 0x04001456 RID: 5206
		public static float STATUS_BURN_DAMAGE = 60f;

		// Token: 0x04001457 RID: 5207
		public static float STATUS_POISON_DAMAGE = 50f;

		// Token: 0x04001458 RID: 5208
		public static float STATUS_LIFE_DAMAGE = -80f;

		// Token: 0x04001459 RID: 5209
		public static float SPELL_DAMAGE_SELF_FIRE = 60f;

		// Token: 0x0400145A RID: 5210
		public static float SPELL_DAMAGE_SELF_EARTH = 100f;

		// Token: 0x0400145B RID: 5211
		public static float SPELL_DAMAGE_SELF_ARCANE = 200f;

		// Token: 0x0400145C RID: 5212
		public static float SPELL_DAMAGE_SELF_ICE = 120f;

		// Token: 0x0400145D RID: 5213
		public static float SPELL_DAMAGE_SELF_STEAM = 80f;

		// Token: 0x0400145E RID: 5214
		public static float SPELL_DAMAGE_SELF_LIGHTNING = 80f;

		// Token: 0x0400145F RID: 5215
		public static float SPELL_DAMAGE_SELF_POISON = 50f;

		// Token: 0x04001460 RID: 5216
		public static readonly Damage MJOLNIR_DAMAGE = new Damage(AttackProperties.Damage | AttackProperties.Pushed, Elements.Lightning, 1000f, 1f);

		// Token: 0x04001461 RID: 5217
		public static readonly int STATUS_DRYING_EFFECT_HASH = "drying_steam".GetHashCodeCustom();

		// Token: 0x04001462 RID: 5218
		public static readonly int STEAM_CUE = "spell_steam_hit".GetHashCodeCustom();

		// Token: 0x04001463 RID: 5219
		public static readonly int[] SOUNDS_UI_ELEMENT = new int[]
		{
			"ui_element_earth".GetHashCodeCustom(),
			"ui_element_water".GetHashCodeCustom(),
			"ui_element_cold".GetHashCodeCustom(),
			"ui_element_fire".GetHashCodeCustom(),
			"ui_element_lightning".GetHashCodeCustom(),
			"ui_element_arcane".GetHashCodeCustom(),
			"ui_element_life".GetHashCodeCustom(),
			"ui_element_shield".GetHashCodeCustom(),
			"ui_element_cold".GetHashCodeCustom(),
			"ui_element_fire".GetHashCodeCustom(),
			"ui_element_water".GetHashCodeCustom()
		};

		// Token: 0x04001464 RID: 5220
		public static readonly int[] SOUNDS_SPRAY = new int[]
		{
			"spell_earth_spray".GetHashCodeCustom(),
			"spell_water_spray".GetHashCodeCustom(),
			"spell_cold_spray".GetHashCodeCustom(),
			"spell_fire_spray".GetHashCodeCustom(),
			"spell_lightning_spray".GetHashCodeCustom(),
			"spell_arcane_spray".GetHashCodeCustom(),
			"spell_life_spray".GetHashCodeCustom(),
			"spell_shield_spray".GetHashCodeCustom(),
			"spell_ice_spray".GetHashCodeCustom(),
			"spell_steam_spray".GetHashCodeCustom(),
			"spell_poison_spray".GetHashCodeCustom()
		};

		// Token: 0x04001465 RID: 5221
		public static readonly int[] SOUNDS_PROJECTILE = new int[]
		{
			"spell_earth_projectile".GetHashCodeCustom(),
			"spell_water_projectile".GetHashCodeCustom(),
			"spell_cold_projectile".GetHashCodeCustom(),
			"spell_fire_projectile".GetHashCodeCustom(),
			"spell_lightning_projectile".GetHashCodeCustom(),
			"spell_arcane_projectile".GetHashCodeCustom(),
			"spell_life_projectile".GetHashCodeCustom(),
			"spell_shield_projectile".GetHashCodeCustom(),
			"spell_ice_projectile".GetHashCodeCustom(),
			"spell_steam_projectile".GetHashCodeCustom(),
			"spell_poison_projectile".GetHashCodeCustom()
		};

		// Token: 0x04001466 RID: 5222
		public static readonly int[] SOUNDS_HIT = new int[]
		{
			"spell_earth_hit".GetHashCodeCustom(),
			"spell_water_hit".GetHashCodeCustom(),
			"spell_cold_hit".GetHashCodeCustom(),
			"spell_fire_hit".GetHashCodeCustom(),
			"spell_lightning_hit".GetHashCodeCustom(),
			"spell_arcane_hit".GetHashCodeCustom(),
			"spell_life_hit".GetHashCodeCustom(),
			"spell_shield_hit".GetHashCodeCustom(),
			"spell_ice_hit".GetHashCodeCustom(),
			"spell_steam_hit".GetHashCodeCustom(),
			"spell_poison_hit".GetHashCodeCustom()
		};

		// Token: 0x04001467 RID: 5223
		public static readonly int[] SOUNDS_CHASM = new int[]
		{
			"spell_earth_chasm".GetHashCodeCustom(),
			"spell_water_chasm".GetHashCodeCustom(),
			"spell_cold_chasm".GetHashCodeCustom(),
			"spell_fire_chasm".GetHashCodeCustom(),
			"spell_lightning_chasm".GetHashCodeCustom(),
			"spell_arcane_chasm".GetHashCodeCustom(),
			"spell_life_chasm".GetHashCodeCustom(),
			"spell_shield_chasm".GetHashCodeCustom(),
			"spell_ice_chasm".GetHashCodeCustom(),
			"spell_steam_chasm".GetHashCodeCustom(),
			"spell_poison_chasm".GetHashCodeCustom()
		};

		// Token: 0x04001468 RID: 5224
		public static readonly int[] SOUNDS_BLAST = new int[]
		{
			"spell_earth_blast".GetHashCodeCustom(),
			"spell_water_blast".GetHashCodeCustom(),
			"spell_cold_blast".GetHashCodeCustom(),
			"spell_fire_blast".GetHashCodeCustom(),
			"spell_lightning_blast".GetHashCodeCustom(),
			"spell_arcane_blast".GetHashCodeCustom(),
			"spell_life_blast".GetHashCodeCustom(),
			"spell_shield_blast".GetHashCodeCustom(),
			"spell_ice_blast".GetHashCodeCustom(),
			"spell_steam_blast".GetHashCodeCustom(),
			"spell_poison_blast".GetHashCodeCustom()
		};

		// Token: 0x04001469 RID: 5225
		public static readonly int[] SOUNDS_BARRIER = new int[]
		{
			"spell_earth_barrier".GetHashCodeCustom(),
			"spell_water_barrier".GetHashCodeCustom(),
			"spell_cold_barrier".GetHashCodeCustom(),
			"spell_fire_barrier".GetHashCodeCustom(),
			"spell_lightning_barrier".GetHashCodeCustom(),
			"spell_arcane_barrier".GetHashCodeCustom(),
			"spell_life_barrier".GetHashCodeCustom(),
			"spell_shield_barrier".GetHashCodeCustom(),
			"spell_ice_barrier".GetHashCodeCustom(),
			"spell_steam_barrier".GetHashCodeCustom(),
			"spell_poison_barrier".GetHashCodeCustom()
		};

		// Token: 0x0400146A RID: 5226
		public static readonly int[] SOUNDS_AREA = new int[]
		{
			"spell_earth_area".GetHashCodeCustom(),
			"spell_water_area".GetHashCodeCustom(),
			"spell_cold_area".GetHashCodeCustom(),
			"spell_fire_area".GetHashCodeCustom(),
			"spell_lightning_area".GetHashCodeCustom(),
			"spell_arcane_area".GetHashCodeCustom(),
			"spell_life_area".GetHashCodeCustom(),
			"spell_shield_area".GetHashCodeCustom(),
			"spell_ice_area".GetHashCodeCustom(),
			"spell_steam_area".GetHashCodeCustom(),
			"spell_poison_area".GetHashCodeCustom()
		};

		// Token: 0x0400146B RID: 5227
		public static readonly int[] SOUNDS_SELF = new int[]
		{
			0,
			"spell_water_self".GetHashCodeCustom(),
			"spell_cold_self".GetHashCodeCustom(),
			"spell_fire_self".GetHashCodeCustom(),
			"spell_lightning_self".GetHashCodeCustom(),
			"spell_arcane_self".GetHashCodeCustom(),
			"spell_life_self".GetHashCodeCustom(),
			"spell_shield_self".GetHashCodeCustom(),
			0,
			"spell_steam_self".GetHashCodeCustom()
		};

		// Token: 0x0400146C RID: 5228
		public static readonly int[] SOUNDS_GORE = new int[]
		{
			"misc_hit_blood".GetHashCodeCustom(),
			"misc_hit_blood".GetHashCodeCustom(),
			"misc_hit_blood".GetHashCodeCustom(),
			"misc_hit_blood".GetHashCodeCustom(),
			"misc_hit_blood".GetHashCodeCustom(),
			"".GetHashCodeCustom()
		};

		// Token: 0x0400146D RID: 5229
		public static readonly int WC_3_HASH = "wc_s3".GetHashCodeCustom();

		// Token: 0x02000276 RID: 630
		public enum DamageFeatures : byte
		{
			// Token: 0x0400146F RID: 5231
			None,
			// Token: 0x04001470 RID: 5232
			Damage,
			// Token: 0x04001471 RID: 5233
			Knockback,
			// Token: 0x04001472 RID: 5234
			Notify = 4,
			// Token: 0x04001473 RID: 5235
			Effects = 8,
			// Token: 0x04001474 RID: 5236
			NK = 6,
			// Token: 0x04001475 RID: 5237
			NKE = 14,
			// Token: 0x04001476 RID: 5238
			DNK = 7,
			// Token: 0x04001477 RID: 5239
			DNKE = 15
		}

		// Token: 0x02000277 RID: 631
		public enum ElementUIDirections
		{
			// Token: 0x04001479 RID: 5241
			DownLeft,
			// Token: 0x0400147A RID: 5242
			LeftUp,
			// Token: 0x0400147B RID: 5243
			RightUp,
			// Token: 0x0400147C RID: 5244
			RightDown,
			// Token: 0x0400147D RID: 5245
			LeftDown,
			// Token: 0x0400147E RID: 5246
			UpRight,
			// Token: 0x0400147F RID: 5247
			UpLeft,
			// Token: 0x04001480 RID: 5248
			DownRight
		}
	}
}
