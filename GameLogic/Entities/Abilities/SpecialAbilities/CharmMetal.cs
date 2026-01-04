// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.CharmMetal
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Magicka.Gamers;
using Magicka.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class CharmMetal : SpecialAbility
{
  public const float CHARM_TIME = 15f;
  private const float RADIUS = 14f;
  private static CharmMetal mSingelton;
  private static volatile object mSingeltonLock = new object();
  public static readonly int CHARM_EFFECT = "magick_charm".GetHashCodeCustom();

  public static CharmMetal Instance
  {
    get
    {
      if (CharmMetal.mSingelton == null)
      {
        lock (CharmMetal.mSingeltonLock)
        {
          if (CharmMetal.mSingelton == null)
            CharmMetal.mSingelton = new CharmMetal();
        }
      }
      return CharmMetal.mSingelton;
    }
  }

  public CharmMetal(Magicka.Animations iAnimation)
    : base(iAnimation, "#magick_charm".GetHashCodeCustom())
  {
  }

  private CharmMetal()
    : base(Magicka.Animations.cast_magick_sweep, "#magick_charm".GetHashCodeCustom())
  {
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    throw new Exception("Charm have to be called by a character!");
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    NetworkState state = NetworkManager.Instance.State;
    if ((state == NetworkState.Client || iOwner is Avatar && (iOwner as Avatar).Player.Gamer is NetworkGamer) && (state != NetworkState.Client || !(iOwner is Avatar) || (iOwner as Avatar).Player.Gamer is NetworkGamer))
      return false;
    base.Execute(iOwner, iPlayState);
    if (!(iOwner is Character))
      return false;
    Vector3 position1 = iOwner.Position;
    Vector3 direction = iOwner.Direction;
    List<Entity> entities = iPlayState.EntityManager.GetEntities(position1, 14f, true);
    Character character = (Character) null;
    float num1 = float.MaxValue;
    for (int index = 0; index < entities.Count; ++index)
    {
      if (entities[index] is Character && entities[index] != iOwner && !(entities[index] as Character).IsCharmed && ((iOwner as Character).Faction & (entities[index] as Character).Faction) == Factions.NONE)
      {
        Vector3 position2 = entities[index].Position;
        Vector3 result;
        Vector3.Subtract(ref position2, ref position1, out result);
        result.Y = 0.0f;
        result.Normalize();
        float num2 = MagickaMath.Angle(ref direction, ref result);
        if ((double) num2 < (double) num1)
        {
          num1 = num2;
          character = entities[index] as Character;
        }
      }
    }
    iPlayState.EntityManager.ReturnEntityList(entities);
    if (character == null)
      return false;
    character.Charm((Entity) (iOwner as Character), 15f, CharmMetal.CHARM_EFFECT);
    if (state != NetworkState.Offline)
      NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
      {
        Handle = character.Handle,
        Id = (int) iOwner.Handle,
        Time = 15f,
        Arg = CharmMetal.CHARM_EFFECT,
        ActionType = TriggerActionType.Charm
      });
    return true;
  }
}
