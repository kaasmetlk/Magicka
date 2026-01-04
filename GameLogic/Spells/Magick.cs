// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Spells.Magick
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.GameStates;
using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Magicka.GameLogic.Spells;

[StructLayout(LayoutKind.Explicit)]
public struct Magick
{
  private static readonly bool[] INSTANT_LOOKUP = new bool[36]
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
  public static readonly int[] NAME_LOCALIZATION = new int[39]
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
  public static readonly int[] DESC_LOCALIZATION = new int[38]
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
  [FieldOffset(0)]
  public Elements Element;
  [FieldOffset(4)]
  public MagickType MagickType;

  public static bool IsInstant(MagickType iType) => Magick.INSTANT_LOOKUP[(int) iType];

  public static void InitializeMagicks(PlayState iPlayState)
  {
    Haste.InitializeCache(16 /*0x10*/);
    Revive.InitializeCache(16 /*0x10*/);
    Rain instance1 = Rain.Instance;
    Thunderbolt instance2 = Thunderbolt.Instance;
    Teleport instance3 = Teleport.Instance;
    MeteorShower instance4 = MeteorShower.Instance;
    Charm instance5 = Charm.Instance;
    SummonSpirit.InitializeCache(iPlayState);
    SummonZombie.InitializeCache(16 /*0x10*/, iPlayState);
    SummonUndead.InitializeCache(iPlayState);
    Conflagration.InitializeCache(16 /*0x10*/);
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
    TornadoEntity.InitializeCache(16 /*0x10*/);
    Wave.InitializeCache(16 /*0x10*/);
    PerformanceEnchantment.InitializeCache(16 /*0x10*/);
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
    Confuse.InitializeCache(16 /*0x10*/);
    Etherealize instance24 = Etherealize.Instance;
    EarthQuake instance25 = EarthQuake.Instance;
    Grow.InitializeCache(16 /*0x10*/);
    Shrink.InitializeCache(16 /*0x10*/);
    Polymorph.InitializeCache(32 /*0x20*/, iPlayState);
    Zap.InitializeCache(8);
    VladZap.InitializeCache(8);
    SummonCross.InitializeCache(16 /*0x10*/, iPlayState);
  }

  public SpecialAbility Effect
  {
    get
    {
      switch (this.MagickType)
      {
        case MagickType.Revive:
          return (SpecialAbility) Revive.GetInstance();
        case MagickType.Grease:
          return (SpecialAbility) Grease.GetInstance();
        case MagickType.Haste:
          return (SpecialAbility) Haste.GetInstance();
        case MagickType.Invisibility:
          return (SpecialAbility) Invisibility.Instance;
        case MagickType.Teleport:
          return (SpecialAbility) Teleport.Instance;
        case MagickType.Fear:
          return (SpecialAbility) Fear.Instance;
        case MagickType.Charm:
          return (SpecialAbility) Charm.Instance;
        case MagickType.ThunderB:
          return (SpecialAbility) Thunderbolt.Instance;
        case MagickType.Rain:
          return (SpecialAbility) Rain.Instance;
        case MagickType.Tornado:
          return (SpecialAbility) Tornado.Instance;
        case MagickType.Blizzard:
          return (SpecialAbility) Blizzard.Instance;
        case MagickType.MeteorS:
          return (SpecialAbility) MeteorShower.Instance;
        case MagickType.Conflagration:
          return (SpecialAbility) Conflagration.GetInstance();
        case MagickType.ThunderS:
          return (SpecialAbility) Thunderstorm.Instance;
        case MagickType.TimeWarp:
          return (SpecialAbility) TimeWarp.Instance;
        case MagickType.Vortex:
          return (SpecialAbility) Vortex.Instance;
        case MagickType.SUndead:
          return (SpecialAbility) SummonZombie.GetInstance();
        case MagickType.SElemental:
          return (SpecialAbility) SummonElemental.Instance;
        case MagickType.SDeath:
          return (SpecialAbility) SummonDeath.Instance;
        case MagickType.SPhoenix:
          return (SpecialAbility) SummonPhoenix.Instance;
        case MagickType.Nullify:
          return (SpecialAbility) Nullify.Instance;
        case MagickType.Corporealize:
          return (SpecialAbility) Corporealize.Instance;
        case MagickType.CTD:
          return (SpecialAbility) CTD.Instance;
        case MagickType.Napalm:
          return (SpecialAbility) Napalm.Instance;
        case MagickType.Portal:
          return (SpecialAbility) Portal.Instance;
        case MagickType.TractorPull:
          return (SpecialAbility) TractorPull.Instance;
        case MagickType.ProppMagick:
          return (SpecialAbility) ProppMagick.Instance;
        case MagickType.Levitate:
          return (SpecialAbility) Levitate.Instance;
        case MagickType.ChainLightning:
          return (SpecialAbility) Zap.GetInstance();
        case MagickType.Confuse:
          return (SpecialAbility) Confuse.GetInstance();
        case MagickType.Wave:
          return (SpecialAbility) Wave.GetInstance();
        case MagickType.PerformanceEnchantment:
          return (SpecialAbility) PerformanceEnchantment.GetInstance();
        case MagickType.JudgementSpray:
          return (SpecialAbility) JudgementSpray.Instance;
        case MagickType.Amalgameddon:
          return (SpecialAbility) Amalgameddon.Instance;
        case MagickType.Grow:
          return (SpecialAbility) Grow.GetInstance();
        case MagickType.Shrink:
          return (SpecialAbility) Shrink.GetInstance();
        case MagickType.Etherealize:
          return (SpecialAbility) Etherealize.Instance;
        case MagickType.Polymorph:
          return (SpecialAbility) Polymorph.GetInstance();
        case MagickType.Earthquake:
          return (SpecialAbility) EarthQuake.Instance;
        case MagickType.Votal:
          return (SpecialAbility) Votal.Instance;
        case MagickType.SummonCross:
          return (SpecialAbility) SummonCross.GetInstance();
        case MagickType.VladZap:
          return (SpecialAbility) VladZap.GetInstance();
        case MagickType.SpawnSlime:
          return (SpecialAbility) SpawnSlime.Instance;
        case MagickType.SpawnSlimeOverkill:
          return (SpecialAbility) SpawnSlimeOverkill.Instance;
        case MagickType.GreaseSplash:
          return (SpecialAbility) GreaseSplash.Instance;
        case MagickType.EtherealClone:
          return (SpecialAbility) EtherealClone.Instance;
        default:
          throw new NotImplementedException(this.MagickType.ToString() + " is not implemented.");
      }
    }
  }
}
