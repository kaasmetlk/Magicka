// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.SpawnSlime
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.AI;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.GameStates;
using Magicka.Levels;
using Magicka.Misc;
using Magicka.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class SpawnSlime : SpecialAbility
{
  protected readonly int SPREAD = 5;
  protected static readonly int POISON_SMALL = "dungeons_slimecube_poison_small".GetHashCodeCustom();
  protected static readonly int POISON_MEDIUM = "dungeons_slimecube_poison".GetHashCodeCustom();
  protected static readonly int POISON_LARGE = "dungeons_slimecube_poison_large".GetHashCodeCustom();
  protected static readonly int ARCANE_SMALL = "dungeons_slimecube_arcane_small".GetHashCodeCustom();
  protected static readonly int ARCANE_MEDIUM = "dungeons_slimecube_arcane".GetHashCodeCustom();
  protected static readonly int ARCANE_LARGE = "dungeons_slimecube_arcane_large".GetHashCodeCustom();
  protected ISpellCaster mOwner;
  protected PlayState mPlayState;
  protected Elements mElements;
  public Magicka.Animations IdleAnimation;
  public Magicka.Animations SpawnAnimation;
  private static SpawnSlime sSingelton;
  private static volatile object sSingeltonLock = new object();

  internal static void InitializeCache(PlayState iPlayState)
  {
    iPlayState.Content.Load<CharacterTemplate>("data/characters/dungeons_slimecube_poison");
    iPlayState.Content.Load<CharacterTemplate>("data/characters/dungeons_slimecube_poison_small");
    iPlayState.Content.Load<CharacterTemplate>("data/characters/dungeons_slimecube_arcane");
    iPlayState.Content.Load<CharacterTemplate>("data/characters/dungeons_slimecube_arcane_small");
  }

  public static SpawnSlime Instance
  {
    get
    {
      if (SpawnSlime.sSingelton == null)
      {
        lock (SpawnSlime.sSingeltonLock)
        {
          if (SpawnSlime.sSingelton == null)
            SpawnSlime.sSingelton = new SpawnSlime();
        }
      }
      return SpawnSlime.sSingelton;
    }
  }

  private SpawnSlime()
    : base(Magicka.Animations.cast_magick_direct, "#Ent_Dungeons_Slime_Medium".GetHashCodeCustom())
  {
  }

  public SpawnSlime(Magicka.Animations iAnimation)
    : base(iAnimation, "#Ent_Dungeons_Slime_Medium".GetHashCodeCustom())
  {
  }

  private SpawnSlime(Elements[] elements)
    : base(Magicka.Animations.cast_magick_direct, "#Ent_Dungeons_Slime_Medium".GetHashCodeCustom())
  {
    for (int index = 0; index < elements.Length; ++index)
      this.mElements |= elements[index];
  }

  public SpawnSlime(Magicka.Animations iAnimation, Elements[] elements)
    : base(iAnimation, "#Ent_Dungeons_Slime_Medium".GetHashCodeCustom())
  {
    for (int index = 0; index < elements.Length; ++index)
      this.mElements |= elements[index];
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
    base.Execute(iOwner, iPlayState);
    if (NetworkManager.Instance.State == NetworkState.Client)
      return false;
    this.mOwner = iOwner;
    this.mPlayState = iPlayState;
    List<Pair<SpawnSlime.SlimeSize, Elements>> entitiesToSpawn = new List<Pair<SpawnSlime.SlimeSize, Elements>>();
    switch ((iOwner as Character).Name)
    {
      case "dungeons_slimecube_poison_large":
        Elements second1 = elements == Elements.None ? Elements.Poison : this.RandomElement(elements);
        for (int index = 0; index < 3; ++index)
          entitiesToSpawn.Add(new Pair<SpawnSlime.SlimeSize, Elements>(SpawnSlime.SlimeSize.Medium, second1));
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

  protected Elements RandomElement(Elements elements)
  {
    Elements elements1;
    do
    {
      elements1 = Defines.ElementFromIndex(MagickaMath.Random.Next(11));
    }
    while ((elements1 & elements) == Elements.None);
    return elements1;
  }

  private int GetTemplateId(SpawnSlime.SlimeSize size, Elements elem)
  {
    switch (elem)
    {
      case Elements.Earth:
      case Elements.Water:
      case Elements.Cold:
      case Elements.Fire:
      case Elements.Lightning:
      case Elements.Arcane:
        if (size == SpawnSlime.SlimeSize.Small)
          return SpawnSlime.ARCANE_SMALL;
        if (size == SpawnSlime.SlimeSize.Medium)
          return SpawnSlime.ARCANE_MEDIUM;
        break;
      case Elements.Life:
      case Elements.Shield:
      case Elements.Ice:
      case Elements.Steam:
      case Elements.Poison:
        return size == SpawnSlime.SlimeSize.Small || size != SpawnSlime.SlimeSize.Medium ? SpawnSlime.POISON_SMALL : SpawnSlime.POISON_MEDIUM;
    }
    return SpawnSlime.POISON_SMALL;
  }

  protected void CreateEntities(
    List<Pair<SpawnSlime.SlimeSize, Elements>> entitiesToSpawn)
  {
    Vector3 position1 = this.mOwner.Position;
    Vector3 direction = this.mOwner.Direction;
    for (int index = 0; index < entitiesToSpawn.Count; ++index)
    {
      int templateId = this.GetTemplateId(entitiesToSpawn[index].First, entitiesToSpawn[index].Second);
      CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(templateId);
      NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(this.mOwner.PlayState);
      Vector3 position2 = this.mOwner.Position;
      float num1 = (float) Math.Sqrt(SpecialAbility.RANDOM.NextDouble());
      float num2 = (float) (SpecialAbility.RANDOM.NextDouble() * 6.2831854820251465);
      float num3 = num1 * (float) Math.Cos((double) num2);
      float num4 = num1 * (float) Math.Sin((double) num2);
      position2.X += (float) this.SPREAD * num3;
      position2.Z += (float) this.SPREAD * num4;
      Vector3 oPoint;
      double nearestPosition = (double) this.mPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref position2, out oPoint, MovementProperties.Default);
      if (NetworkManager.Instance.State == NetworkState.Server)
        NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
        {
          ActionType = TriggerActionType.SpawnNPC,
          Handle = instance.Handle,
          Template = templateId,
          Id = 0,
          Position = oPoint,
          Direction = direction,
          Bool0 = false,
          Point0 = 0,
          Point1 = 0,
          Point2 = (int) this.SpawnAnimation,
          Point3 = (int) this.IdleAnimation
        });
      instance.Initialize(cachedTemplate, oPoint, 0);
      if (this.mOwner is Character)
        instance.Faction = (this.mOwner as Character).Faction;
      if (this.mOwner.PlayState.Level.CurrentScene.RuleSet != null && this.mOwner.PlayState.Level.CurrentScene.RuleSet is SurvivalRuleset)
        (this.mOwner.PlayState.Level.CurrentScene.RuleSet as SurvivalRuleset).AddedCharacter(instance, true);
      instance.HitPoints = instance.MaxHitPoints;
      instance.AI.SetOrder(Order.Attack, ReactTo.Proximity, Order.Attack, 0, 0, 0, (AIEvent[]) null);
      if (this.SpawnAnimation != Magicka.Animations.None && this.SpawnAnimation != Magicka.Animations.idle && this.SpawnAnimation != Magicka.Animations.idle_agg)
      {
        instance.SpawnAnimation = this.SpawnAnimation;
        instance.ChangeState((BaseState) RessurectionState.Instance);
      }
      if (this.IdleAnimation != Magicka.Animations.None)
        instance.SpecialIdleAnimation = this.IdleAnimation;
      this.mOwner.PlayState.EntityManager.AddEntity((Entity) instance);
    }
  }

  protected void SpawnSlimes(int iTemplateId, int iAmount)
  {
    CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(iTemplateId);
    Vector3 position = this.mOwner.Position;
    Vector3 direction = this.mOwner.Direction;
    for (int index = 0; index < iAmount; ++index)
    {
      NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(this.mOwner.PlayState);
      Vector3 vector3;
      vector3.X = (float) (MagickaMath.Random.NextDouble() - 0.5) * (float) this.SPREAD;
      vector3.Y = position.Y;
      vector3.Z = (float) (MagickaMath.Random.NextDouble() - 0.5) * (float) this.SPREAD;
      Vector3 iPoint = position + vector3;
      Vector3 oPoint;
      double nearestPosition = (double) this.mPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref iPoint, out oPoint, MovementProperties.Default);
      if (NetworkManager.Instance.State == NetworkState.Server)
        NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
        {
          ActionType = TriggerActionType.SpawnNPC,
          Handle = instance.Handle,
          Template = iTemplateId,
          Id = 0,
          Position = oPoint,
          Direction = direction,
          Bool0 = false,
          Point0 = 0,
          Point1 = 0,
          Point2 = (int) this.SpawnAnimation,
          Point3 = (int) this.IdleAnimation
        });
      instance.Initialize(cachedTemplate, oPoint, 0);
      if (this.mOwner is Character)
        instance.Faction = (this.mOwner as Character).Faction;
      if (this.mOwner.PlayState.Level.CurrentScene.RuleSet != null && this.mOwner.PlayState.Level.CurrentScene.RuleSet is SurvivalRuleset)
        (this.mOwner.PlayState.Level.CurrentScene.RuleSet as SurvivalRuleset).AddedCharacter(instance, true);
      instance.HitPoints = instance.MaxHitPoints;
      instance.AI.SetOrder(Order.Attack, ReactTo.Proximity, Order.Attack, 0, 0, 0, (AIEvent[]) null);
      if (this.SpawnAnimation != Magicka.Animations.None && this.SpawnAnimation != Magicka.Animations.idle && this.SpawnAnimation != Magicka.Animations.idle_agg)
      {
        instance.SpawnAnimation = this.SpawnAnimation;
        instance.ChangeState((BaseState) RessurectionState.Instance);
      }
      if (this.IdleAnimation != Magicka.Animations.None)
        instance.SpecialIdleAnimation = this.IdleAnimation;
      this.mOwner.PlayState.EntityManager.AddEntity((Entity) instance);
    }
  }

  public enum SlimeSize
  {
    Small,
    Medium,
    Large,
  }
}
