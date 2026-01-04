// Decompiled with JetBrains decompiler
// Type: Magicka.Defines
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace Magicka;

public static class Defines
{
  public const string CONTENT_ROOT = "content";
  public const float NETWORK_RESTING_TIME = 1f;
  public const double SQRT2 = 1.4142135623730951;
  public const double SQRT3 = 1.7320508075688772;
  public const int SECOND = 10000000;
  public const double dSECOND = 10000000.0;
  private const float Col = 0.003921569f;
  public const float FALLING_DAMAGE_POWER = -50f;
  public const float FALLING_KNOCKDOWN_THRESHOLD = 100f;
  public const float FALLING_DAMAGE_THRESHOLD = 0.0f;
  public const float CHARACTERS_PER_SECONDS = 40f;
  public const float UI_RADIUS = 64f;
  public const float ICONS_HEIGHT_OFFSET = 44f;
  public const int ICON_WIDTH = 50;
  public const int ICON_HEIGHT = 50;
  public const float TEXT_PUNCTUATION_PAUSE = 0.25f;
  public const float PASSIVE_ABILITY_TIMER = 0.25f;
  public const float PASSIVE_ABILITY_AREA_REGEN = 2f;
  public const float PASSIVE_ABILITY_AREA_DRAINLIFE = 4f;
  public const float PASSIVE_ABILITY_MELEE_BOOST = 1.5f;
  public const float PASSIVE_ABILITY_MELEE_BOOST_RANGE = 7f;
  public const float PASSIVE_ABILITY_AREA_REGEN_RANGE = 7f;
  public const float PASSIVE_ABILITY_AREA_DRAINLIFE_RANGE = 7f;
  public const float ITEM_DESPAWN_TIME = 20f;
  public const int SETTINGS_LUGGAGE_INTERVARL_HIGH = 15;
  public const int SETTINGS_LUGGAGE_INTERVARL_MEDIUM = 30;
  public const int SETTINGS_LUGGAGE_INTERVARL_LOW = 90;
  public const int SPELL_QUECAP = 5;
  public const int NR_OF_ELEMENTS = 11;
  public const int MAX_NR_OF_SUMMONS_PER_CHARACTER = 16 /*0x10*/;
  public const int MAX_NR_OF_UNDEAD_SUMMONS = 1;
  public const int MAX_NR_OF_FLAMER_SUMMONS = 4;
  public const float SPELL_RAILGUN_BASE_TTL = 1f;
  public const float SPELL_RAILGUN_MOD_TTL = 2f;
  public const float SPELL_STRENGTH_PUSH_BASE = 50f;
  public const float SPELL_STRENGTH_PUSH_VAR = 500f;
  public const float SPELL_STRENGTH_SHIELD = 500f;
  public const float SPELL_STRENGTH_BARRIER = 500f;
  public const float SPELL_STRENGTH_BARRIER_ELEMENTAL = 100f;
  public const float SPELL_ARMOR_EARTH = 90f;
  public const float SPELL_ARMOR_ICE = 50f;
  public const float SPELL_TIME_GROUND = 15f;
  public const float SPELL_TIME_PUSH = 0.25f;
  public const float SPELL_TIME_ICE = 0.35f;
  public const float SPELL_TIME_SPRAY = 4f;
  public const float SPELL_TIME_LIGHTNING = 1f;
  public const int SPELL_AMOUNT_LIGHTNINGS = 8;
  public const int SPELL_AMOUNT_ICE = 3;
  public const float SELF_SHIELD_HEALTH = 500f;
  public const float SELF_SHIELD_DECAY = 100f;
  public const float SELF_STONE_HEALTH = 1500f;
  public const float SELF_STONE_DECAY = 0.0f;
  public const float SELF_STONE_MODIFIER = 10f;
  public const float SELF_STONE_MULTIPLIER = 0.15f;
  public const float SELF_ICE_HEALTH = 900f;
  public const float SELF_ICE_DECAY = 0.0f;
  public const float SELF_ICE_MODIFIER = 70f;
  public const float SELF_ICE_MULTIPLIER = 0.05f;
  public const float SELF_LIGHTNING_RANGE = 5f;
  public const float SELF_LIGHTNING_TIME = 0.3f;
  public const float DIFFICULTY_RESISTANCE_MODIFIER = 0.00666f;
  public const float SPELL_ANGLE_PUSH = 0.5235988f;
  public const float SPELL_ANGLE_SPRAY = 0.314159274f;
  public const float SPELL_ANGLE_LIGHTNING = 0.7853982f;
  public const float SPELL_RANGE_PROJECTILE_BASE = 25f;
  public const float SPELL_RANGE_PROJECTILE_VAR = 85f;
  public const float SPELL_RANGE_RAILGUN = 15f;
  public const float SPELL_RANGE_SHIELD = 5.5f;
  public const float SPELL_RANGE_SHIELD_MIN = 3f;
  public const float SPELL_RANGE_BUBBLE = 1.3f;
  public const float SPELL_RANGE_RAILGUN_WIDTH = 0.75f;
  public const float SPELL_RANGE_PUSH = 1.5f;
  public const float SPELL_RANGE_LIGHTNING = 8f;
  public const float SPELL_RANGE_SPRAY = 10f;
  public const float SPELL_RANGE_AREA = 10f;
  public const float SPELL_RANGE_AREA_PUSH = 3.5f;
  public const float SPELL_RANGE_AREA_LIGHTNING = 4f;
  public const float SPELL_RANGE_AREA_PROJECTILE = 6f;
  public const float SPELL_PROJECTILE_DANGER = 30f;
  public const int SPELL_MISC_WATER_PUSH_THRESHOLD = 2;
  public const float SPELL_MISC_LIGHTNING_LIGHT_INTENSITY = 1.5f;
  public const float SPELL_MISC_EARTH_BOUNCE_THRESHOLD = 8f;
  public const float SPELL_MISC_LIGHTNING_DECAY = 1f;
  public const float BLOATTIME = 0.333f;
  public const int NR_OF_STATUSEFFECTS = 9;
  public const float STATUS_DRYING_TIME = 0.9f;
  public const float STATUS_POISON_TIME = 0.5f;
  public const float STATUS_BURN_TIME = 0.2f;
  public const float STATUS_COLD_TIME = 0.1f;
  public const float STATUS_WET_TIME = 0.0333333351f;
  public const float STATUS_FROZEN_TIME = 0.25f;
  public const int STATUS_FROZEN_ARMOR = 50;
  public const int STATUS_FROZEN_MAGNITUDE = 10;
  public const float MAX_STATUS_MAGNITUDE = 3f;
  public const float REGENERATION_TIMER = 1f;
  public const float HITLIST_COOLDOWN = 0.25f;
  public const float KNOCKBACK_ELEVATION = 0.6980619f;
  public const float PUSH_ELEVATION = 0.17453292f;
  public const float BLOCKANGLE = 1.0995574f;
  public const int BARRIER_DAMAGE_OVER_TIME = 10;
  public const int WAVE_DAMAGE_OVER_TIME = 500;
  public const float DECALTIMEOUT = 60f;
  public const int GUI_HEIGHT = 64 /*0x40*/;
  public const int MAX_ENEMY_HEALTHBARS = 512 /*0x0200*/;
  public const int MAX_SHADOW_BLOBS = 512 /*0x0200*/;
  public const int SAVESLOTS = 3;
  public const int MAX_NAME_LENGTH = 15;
  public const float MAX_DRAW_LATANCY = 0.2f;
  public const int MAXDYNAMICLIGHTS = 128 /*0x80*/;
  public const float PICKUPRADIUS = 2.5f;
  public const int MAXELEMENTS = 20;
  public const float MAXBOOSTRANGE = 20f;
  public const float LIGHTNING_FRIEDTIME = 0.1f;
  public const int EVENT_CONDITION_DEFAULT_SIZE = 8;
  public const float SELF_ELEMENTAL_AURA_TTL = 10f;
  public const float SELF_ELEMENTAL_AURA_BASE_TTL = 15f;
  public const float SELF_ELEMENTAL_AURA_MODIFIER_TTL = 5f;
  public const float SELF_ELEMENTAL_AURA_RADIUS_MODIFIER = 1.5f;
  public const float SELF_ELEMENTAL_AURA_RADIUS_BASE = 0.0f;
  public const float SELF_ELEMENTAL_AURA_BUFF_RESTSTANCE_MULTIPLIER = 0.0f;
  public const float BARRIER_DAMAGE_RADIUS_MODIFIER = 2f;
  public const float BARRIER_ICE_EFFECT_TTL_MAGNITUDE_BASE = 1f;
  public const float BARRIER_EARTH_EFFECT_TTL_MAGNITUDE_BASE = 5f;
  public const float BARRIER_BLAST_RADIUS = 2.6f;
  public const ulong VERSION1471 = 281492157038593 /*0x01000400070001*/;
  public const ulong VERSION1460 = 281492156973056 /*0x01000400060000*/;
  public const ulong VERSION1430 = 281492156776448 /*0x01000400030000*/;
  public const ulong VERSION1420 = 281492156710912 /*0x01000400020000*/;
  public const ulong VERSION1410 = 281492156645376 /*0x01000400010000*/;
  public const ulong VERSION1403 = 281492156579843 /*0x01000400000003*/;
  public const ulong VERSION1370 = 281487862071296 /*0x01000300070000*/;
  public const ulong VERSION1362 = 281487862005762 /*0x01000300060002*/;
  public const ulong VERSION1355 = 281487861940229 /*0x01000300050005*/;
  public const ulong VERSION1354 = 281487861940228 /*0x01000300050004*/;
  public const ulong VERSION1350 = 281487861940224 /*0x01000300050000*/;
  public const ulong VERSION1334 = 281487861809156 /*0x01000300030004*/;
  public const ulong VERSION1330 = 281487861809152 /*0x01000300030000*/;
  public const ulong VERSION1290 = 281483567235072 /*0x01000200090000*/;
  public static readonly VertexPositionTexture[] QUAD_TEX_VERTS_C = new VertexPositionTexture[4]
  {
    new VertexPositionTexture(new Vector3(-0.5f, -0.5f, 0.0f), new Vector2(0.0f, 0.0f)),
    new VertexPositionTexture(new Vector3(0.5f, -0.5f, 0.0f), new Vector2(1f, 0.0f)),
    new VertexPositionTexture(new Vector3(0.5f, 0.5f, 0.0f), new Vector2(1f, 1f)),
    new VertexPositionTexture(new Vector3(-0.5f, 0.5f, 0.0f), new Vector2(0.0f, 1f))
  };
  public static readonly VertexPositionTexture[] QUAD_TEX_VERTS_TL = new VertexPositionTexture[4]
  {
    new VertexPositionTexture(new Vector3(0.0f, 0.0f, 0.0f), new Vector2(0.0f, 0.0f)),
    new VertexPositionTexture(new Vector3(1f, 0.0f, 0.0f), new Vector2(1f, 0.0f)),
    new VertexPositionTexture(new Vector3(1f, 1f, 0.0f), new Vector2(1f, 1f)),
    new VertexPositionTexture(new Vector3(0.0f, 1f, 0.0f), new Vector2(0.0f, 1f))
  };
  public static readonly VertexPositionColor[] QUAD_COL_VERTS_C = new VertexPositionColor[4]
  {
    new VertexPositionColor(new Vector3(-0.5f, -0.5f, 0.0f), Color.Black),
    new VertexPositionColor(new Vector3(0.5f, -0.5f, 0.0f), Color.Black),
    new VertexPositionColor(new Vector3(0.5f, 0.5f, 0.0f), Color.Black),
    new VertexPositionColor(new Vector3(-0.5f, 0.5f, 0.0f), Color.Black)
  };
  public static readonly VertexPositionColor[] QUAD_COL_VERTS_TL = new VertexPositionColor[4]
  {
    new VertexPositionColor(new Vector3(0.0f, 0.0f, 0.0f), Color.Black),
    new VertexPositionColor(new Vector3(1f, 0.0f, 0.0f), Color.Black),
    new VertexPositionColor(new Vector3(1f, 1f, 0.0f), Color.Black),
    new VertexPositionColor(new Vector3(0.0f, 1f, 0.0f), Color.Black)
  };
  public static readonly int[] INTERACTSTRINGS = new int[13]
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
  public static readonly int[] ELEMENT_STRINGS = new int[11]
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
  public static readonly int LOC_GEN_OK = "#add_menu_ok".GetHashCodeCustom();
  public static readonly int LOC_GEN_CANCEL = "#add_menu_cancel".GetHashCodeCustom();
  public static readonly int LOC_GEN_JOIN = "#add_menu_join".GetHashCodeCustom();
  public static readonly int LOC_GEN_CHARSEL = "#add_menu_charsel".GetHashCodeCustom();
  public static readonly int LOC_GEN_READY = "#add_menu_ready".GetHashCodeCustom();
  public static readonly int LOC_GEN_YES = "#add_menu_yes".GetHashCodeCustom();
  public static readonly int LOC_GEN_NO = "#add_menu_no".GetHashCodeCustom();
  public static readonly int LOC_GEN_RUS = "#add_menu_rus".GetHashCodeCustom();
  public static readonly int LOC_GEN_RESTART = "#add_menu_restart".GetHashCodeCustom();
  public static readonly int LOC_GEN_RESTARTCHECKPOINT = "#add_menu_restartcheckpoint".GetHashCodeCustom();
  public static readonly int LOC_GEN_RETRY = "#add_menu_retry".GetHashCodeCustom();
  public static readonly int LOC_GEN_QUIT = "#add_menu_quit".GetHashCodeCustom();
  public static readonly int LOC_GAME_VICTORY = "#add_game_victory".GetHashCodeCustom();
  public static readonly int LOC_GAME_LEVCOMP = "#add_game_level_complete".GetHashCodeCustom();
  public static readonly int LOC_GAME_DEFEATED = "#add_game_defeated".GetHashCodeCustom();
  public static readonly int LOC_GAME_WINNER = "#add_game_winner".GetHashCodeCustom();
  public static readonly int LOC_GAME_PRESSSTART = "#add_game_pressstart".GetHashCodeCustom();
  public static readonly int LOC_GAME_VS_WINNER = "#add_vs_winner".GetHashCodeCustom();
  public static readonly int LOC_GAME_CONTINUED = "#TITLE_dungeons_to_be_continued".GetHashCodeCustom();
  public static readonly int MAXTIMELIMIT = 99;
  public static readonly int MAXSCORELIMIT = 999;
  public static readonly int MAXRESPAWNTIME = 60;
  public static readonly float FOOTSTEP_WATER_OFFSET = 0.2f;
  public static readonly int WATER_SPLASH_EFFECT = "water_splash".GetHashCodeCustom();
  public static readonly int WATER_DROWN_EFFECT = "water_drown".GetHashCodeCustom();
  public static readonly int LAVA_SPLASH_EFFECT = "lava_splash".GetHashCodeCustom();
  public static readonly int LAVA_DROWN_EFFECT = "lava_drown".GetHashCodeCustom();
  public static readonly double ONEOVERLN2 = 1.0 / Math.Log(2.0);
  public static readonly Vector4 DIALOGUE_COLOR_DEFAULT = new Vector4(0.7037f, 0.5576f, 0.3202f, 1f);
  public static readonly Vector4 MESSAGEBOX_COLOR_DEFAULT = new Vector4(0.1037f, 0.1576f, 0.0202f, 1f);
  public static readonly Vector3[] PLAYERCOLORS = new Vector3[15]
  {
    new Vector3(240f, 20f, 20f) * 0.003921569f,
    new Vector3(145f, 0.0f, 70f) * 0.003921569f,
    new Vector3(160f, 0.0f, (float) byte.MaxValue) * 0.003921569f,
    new Vector3(0.0f, 132f, (float) byte.MaxValue) * 0.003921569f,
    new Vector3(0.0f, (float) byte.MaxValue, 226f) * 0.003921569f,
    new Vector3(0.0f, 180f, 10f) * 0.003921569f,
    new Vector3(160f, 270f, 0.0f) * 0.003921569f,
    new Vector3(363f, 298f, 0.0f) * 0.003921569f,
    new Vector3(112f, 112f, 112f) * 0.003921569f,
    new Vector3(12f, 12f, 12f) * 0.003921569f,
    new Vector3(320f, 320f, 320f) * 0.003921569f,
    new Vector3(0.0f, 20f, 150f) * 0.003921569f,
    new Vector3(148f, 71f, 0.0f) * 0.003921569f,
    new Vector3(333f, 173f, 0.0f) * 0.003921569f,
    new Vector3(338f, 186f, 229f) * 0.003921569f
  };
  public static int PLAYERCOLORS_UNLOCKED = 15;
  public static readonly Vector3[] TEAMCOLORS = new Vector3[2]
  {
    new Vector3(240f, 20f, 20f) * 0.003921569f,
    new Vector3(0.0f, 0.0f, 200f) * 0.003921569f
  };
  public static readonly float[] ElementUIRadian = new float[8]
  {
    4.31969f,
    2.74889374f,
    0.3926991f,
    5.89048624f,
    3.53429174f,
    3f * (float) Math.PI / 8f,
    1.96349549f,
    5.105088f
  };
  public static readonly int[] ChantEffects = new int[13]
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
  public static readonly int[] PROJECTILE_TRAIL_BIG = new int[11]
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
  public static readonly int[] PROJECTILE_TRAIL_SMALL = new int[11]
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
  public static readonly int[] PROJECTILE_HIT_SMALL = new int[11]
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
  public static readonly int[] PROJECTILE_HIT_BIG = new int[11]
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
  public static float SPELL_DAMAGE_BARRIER_LIGHTNING = 130f;
  public static float SPELL_DAMAGE_BARRIER_ICE = 275f;
  public static float SPELL_DAMAGE_BARRIER_LIFE = -600f;
  public static float SPELL_DAMAGE_BARRIER_ARCANE = 225f;
  public static float SPELL_DAMAGE_BARRIER_STEAM = 225f;
  public static float SPELL_DAMAGE_BARRIER_FIRE = 225f;
  public static float SPELL_DAMAGE_BARRIER_POISON = 225f;
  public static float SPELL_DAMAGE_EARTH = 150f;
  public static float SPELL_DAMAGE_ICEEARTH = 275f;
  public static float SPELL_DAMAGE_ICE = 180f;
  public static float SPELL_DAMAGE_LIGHTNING = 250f;
  public static float SPELL_DAMAGE_LIFE = -180f;
  public static float SPELL_DAMAGE_ARCANE = 225f;
  public static float SPELL_DAMAGE_FIRE = 60f;
  public static float SPELL_DAMAGE_COLD = 25f;
  public static float SPELL_DAMAGE_STEAM = 280f;
  public static float SPELL_STRENGTH_WATER = 70f;
  public static float STATUS_BURN_DAMAGE = 60f;
  public static float STATUS_POISON_DAMAGE = 50f;
  public static float STATUS_LIFE_DAMAGE = -80f;
  public static float SPELL_DAMAGE_SELF_FIRE = 60f;
  public static float SPELL_DAMAGE_SELF_EARTH = 100f;
  public static float SPELL_DAMAGE_SELF_ARCANE = 200f;
  public static float SPELL_DAMAGE_SELF_ICE = 120f;
  public static float SPELL_DAMAGE_SELF_STEAM = 80f;
  public static float SPELL_DAMAGE_SELF_LIGHTNING = 80f;
  public static float SPELL_DAMAGE_SELF_POISON = 50f;
  public static readonly Damage MJOLNIR_DAMAGE = new Damage(AttackProperties.Damage | AttackProperties.Pushed, Elements.Lightning, 1000f, 1f);
  public static readonly int STATUS_DRYING_EFFECT_HASH = "drying_steam".GetHashCodeCustom();
  public static readonly int STEAM_CUE = "spell_steam_hit".GetHashCodeCustom();
  public static readonly int[] SOUNDS_UI_ELEMENT = new int[11]
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
  public static readonly int[] SOUNDS_SPRAY = new int[11]
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
  public static readonly int[] SOUNDS_PROJECTILE = new int[11]
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
  public static readonly int[] SOUNDS_HIT = new int[11]
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
  public static readonly int[] SOUNDS_CHASM = new int[11]
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
  public static readonly int[] SOUNDS_BLAST = new int[11]
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
  public static readonly int[] SOUNDS_BARRIER = new int[11]
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
  public static readonly int[] SOUNDS_AREA = new int[11]
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
  public static readonly int[] SOUNDS_SELF = new int[10]
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
  public static readonly int[] SOUNDS_GORE = new int[6]
  {
    "misc_hit_blood".GetHashCodeCustom(),
    "misc_hit_blood".GetHashCodeCustom(),
    "misc_hit_blood".GetHashCodeCustom(),
    "misc_hit_blood".GetHashCodeCustom(),
    "misc_hit_blood".GetHashCodeCustom(),
    "".GetHashCodeCustom()
  };
  public static readonly int WC_3_HASH = "wc_s3".GetHashCodeCustom();

  public static bool FeatureDamage(Defines.DamageFeatures iFeatures)
  {
    return (iFeatures & Defines.DamageFeatures.Damage) == Defines.DamageFeatures.Damage;
  }

  public static bool FeatureKnockback(Defines.DamageFeatures iFeatures)
  {
    return (iFeatures & Defines.DamageFeatures.Knockback) == Defines.DamageFeatures.Knockback;
  }

  public static bool FeatureNotify(Defines.DamageFeatures iFeatures)
  {
    return (iFeatures & Defines.DamageFeatures.Notify) == Defines.DamageFeatures.Notify;
  }

  public static bool FeatureEffects(Defines.DamageFeatures iFeatures)
  {
    return (iFeatures & Defines.DamageFeatures.Effects) == Defines.DamageFeatures.Effects;
  }

  public static bool IsWeapon(WeaponClass iClass)
  {
    return iClass == WeaponClass.Thrust_Fast || iClass == WeaponClass.Thrust_Medium || iClass == WeaponClass.Thrust_Slow || iClass == WeaponClass.Crush_Fast || iClass == WeaponClass.Crush_Medium || iClass == WeaponClass.Crush_Slow || iClass == WeaponClass.Slash_Fast || iClass == WeaponClass.Slash_Medium || iClass == WeaponClass.Slash_Slow || iClass == WeaponClass.Throw_Fast || iClass == WeaponClass.Throw_Medium || iClass == WeaponClass.Throw_Slow || iClass == WeaponClass.Handgun || iClass == WeaponClass.Rifle || iClass == WeaponClass.Machinegun || iClass == WeaponClass.Heavy;
  }

  public static int ElementIndex(Elements iElement)
  {
    return iElement == Elements.All ? 11 : (int) (Math.Log((double) iElement) * Defines.ONEOVERLN2 + 0.5);
  }

  public static Elements ElementFromIndex(int iIndex)
  {
    return iIndex == 11 ? Elements.All : (Elements) (Math.Pow(2.0, (double) iIndex) + 0.5);
  }

  internal static int DecalLimit()
  {
    switch (GlobalSettings.Instance.DecalLimit)
    {
      case SettingOptions.Low:
        return 150;
      case SettingOptions.High:
        return 600;
      default:
        return 300;
    }
  }

  internal static float ParticleMultiplyer()
  {
    switch (GlobalSettings.Instance.Particles)
    {
      case SettingOptions.Low:
        return 0.5f;
      case SettingOptions.High:
        return 1f;
      default:
        return 0.75f;
    }
  }

  internal static int ParticleLimit()
  {
    switch (GlobalSettings.Instance.Particles)
    {
      case SettingOptions.Low:
        return 16384 /*0x4000*/;
      case SettingOptions.High:
        return 32768 /*0x8000*/;
      default:
        return 24576 /*0x6000*/;
    }
  }

  public static int DamageTypeIndex(AttackProperties iDamageType)
  {
    return (int) (Math.Log((double) iDamageType) * Defines.ONEOVERLN2 + 0.5);
  }

  public enum DamageFeatures : byte
  {
    None = 0,
    Damage = 1,
    Knockback = 2,
    Notify = 4,
    NK = 6,
    DNK = 7,
    Effects = 8,
    NKE = 14, // 0x0E
    DNKE = 15, // 0x0F
  }

  public enum ElementUIDirections
  {
    DownLeft,
    LeftUp,
    RightUp,
    RightDown,
    LeftDown,
    UpRight,
    UpLeft,
    DownRight,
  }
}
