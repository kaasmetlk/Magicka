// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Buffs.AuraLove
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.Gamers;
using Magicka.Network;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Buffs;

public struct AuraLove
{
  public float Radius;
  public float TTL;

  public AuraLove(float iTTL, float iRadius)
  {
    this.Radius = iRadius;
    this.TTL = iTTL;
  }

  public AuraLove(ContentReader iInput)
  {
    this.Radius = iInput.ReadSingle();
    this.TTL = iInput.ReadSingle();
  }

  public float Execute(Character iOwner, AuraTarget iAuraTarget, int iEffect, float iRadius)
  {
    NetworkState state = NetworkManager.Instance.State;
    if (state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer)) || state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer))
    {
      List<Entity> entities = iOwner.PlayState.EntityManager.GetEntities(iOwner.Position, this.Radius, true);
      entities.Remove((Entity) iOwner);
      foreach (Entity entity in entities)
      {
        if (entity is Character character && !character.IsCharmed && (character.Faction & iOwner.Faction) == Factions.NONE)
        {
          character.Charm((Entity) iOwner, this.TTL, Charm.CHARM_EFFECT);
          if (state != NetworkState.Offline)
            NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
            {
              Handle = character.Handle,
              Id = (int) iOwner.Handle,
              Time = this.TTL,
              Arg = Charm.CHARM_EFFECT,
              ActionType = TriggerActionType.Charm
            });
        }
      }
    }
    return 1f;
  }
}
