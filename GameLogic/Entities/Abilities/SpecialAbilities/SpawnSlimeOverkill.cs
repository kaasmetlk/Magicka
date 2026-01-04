// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.SpawnSlimeOverkill
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Magicka.Misc;
using Magicka.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class SpawnSlimeOverkill : SpawnSlime
{
  private static SpawnSlimeOverkill sSingelton;
  private static volatile object sSingeltonLock = new object();

  public static SpawnSlimeOverkill Instance
  {
    get
    {
      if (SpawnSlimeOverkill.sSingelton == null)
      {
        lock (SpawnSlimeOverkill.sSingeltonLock)
        {
          if (SpawnSlimeOverkill.sSingelton == null)
            SpawnSlimeOverkill.sSingelton = new SpawnSlimeOverkill();
        }
      }
      return SpawnSlimeOverkill.sSingelton;
    }
  }

  private SpawnSlimeOverkill()
    : base(Magicka.Animations.cast_magick_direct)
  {
  }

  public SpawnSlimeOverkill(Magicka.Animations iAnimation)
    : base(iAnimation)
  {
  }

  private SpawnSlimeOverkill(Elements[] elements)
    : base(Magicka.Animations.cast_magick_direct, elements)
  {
  }

  public SpawnSlimeOverkill(Magicka.Animations iAnimation, Elements[] elements)
    : base(iAnimation, elements)
  {
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    throw new Exception("New slime cannot be spawned without a parent (iOwner)!");
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    return this.Execute(iOwner, this.mElements, iPlayState);
  }

  public override bool Execute(ISpellCaster iOwner, Elements elements, PlayState iPlayState)
  {
    if (iOwner != null)
      this.mTimeStamp = iOwner.PlayState.PlayTime;
    if (NetworkManager.Instance.State == NetworkState.Client)
      return false;
    this.mOwner = iOwner;
    this.mPlayState = iPlayState;
    List<Pair<SpawnSlime.SlimeSize, Elements>> entitiesToSpawn = new List<Pair<SpawnSlime.SlimeSize, Elements>>();
    switch ((iOwner as Character).Name)
    {
      case "dungeons_slimecube_poison_large":
        Elements second1 = elements == Elements.None ? Elements.Poison : this.RandomElement(elements);
        for (int index = 0; index < 8; ++index)
          entitiesToSpawn.Add(new Pair<SpawnSlime.SlimeSize, Elements>(SpawnSlime.SlimeSize.Small, second1));
        break;
      case "dungeons_slimecube_poison_medium":
      case "dungeons_slimecube_medium":
      case "dungeons_slimecube_poison":
        Elements second2 = elements == Elements.None ? Elements.Poison : this.RandomElement(elements);
        for (int index = 0; index < 3; ++index)
          entitiesToSpawn.Add(new Pair<SpawnSlime.SlimeSize, Elements>(SpawnSlime.SlimeSize.Small, second2));
        break;
      case "dungeons_slimecube_arcane":
        Elements second3 = elements == Elements.None ? Elements.Arcane : this.RandomElement(elements);
        for (int index = 0; index < 3; ++index)
          entitiesToSpawn.Add(new Pair<SpawnSlime.SlimeSize, Elements>(SpawnSlime.SlimeSize.Small, second3));
        break;
      case "dungeons_acolyte_a":
        Elements second4 = elements == Elements.None ? Elements.Poison : this.RandomElement(elements);
        for (int index = 0; index < 3; ++index)
          entitiesToSpawn.Add(new Pair<SpawnSlime.SlimeSize, Elements>(SpawnSlime.SlimeSize.Small, second4));
        break;
      case "dungeons_acolyte_c":
        Elements second5 = elements == Elements.None ? Elements.Poison : this.RandomElement(elements);
        entitiesToSpawn.Add(new Pair<SpawnSlime.SlimeSize, Elements>(SpawnSlime.SlimeSize.Medium, second5));
        entitiesToSpawn.Add(new Pair<SpawnSlime.SlimeSize, Elements>(SpawnSlime.SlimeSize.Medium, second5));
        entitiesToSpawn.Add(new Pair<SpawnSlime.SlimeSize, Elements>(SpawnSlime.SlimeSize.Small, second5));
        entitiesToSpawn.Add(new Pair<SpawnSlime.SlimeSize, Elements>(SpawnSlime.SlimeSize.Small, second5));
        break;
      default:
        Elements second6 = Elements.Poison;
        entitiesToSpawn.Add(new Pair<SpawnSlime.SlimeSize, Elements>(SpawnSlime.SlimeSize.Small, second6));
        break;
    }
    this.CreateEntities(entitiesToSpawn);
    return true;
  }
}
