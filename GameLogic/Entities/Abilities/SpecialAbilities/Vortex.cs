// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Vortex
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Magicka.Gamers;
using Magicka.Network;
using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class Vortex : SpecialAbility
{
  private static Vortex sSingelton;
  private static volatile object sSingeltonLock = new object();

  public static Vortex Instance
  {
    get
    {
      if (Vortex.sSingelton == null)
      {
        lock (Vortex.sSingeltonLock)
        {
          if (Vortex.sSingelton == null)
            Vortex.sSingelton = new Vortex();
        }
      }
      return Vortex.sSingelton;
    }
  }

  private Vortex()
    : base(Magicka.Animations.cast_magick_global, "#magick_vortex".GetHashCodeCustom())
  {
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    NetworkState state = NetworkManager.Instance.State;
    if (state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer)) || state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer))
    {
      Vector3 iPosition = iOwner.Position + iOwner.Direction * 10f;
      VortexEntity instance = VortexEntity.GetInstance();
      instance.Initialize(iOwner, iPosition);
      iPlayState.EntityManager.AddEntity((Entity) instance);
      if (state != NetworkState.Offline)
        NetworkManager.Instance.Interface.SendMessage<SpawnVortexMessage>(ref new SpawnVortexMessage()
        {
          Handle = instance.Handle,
          Position = iPosition,
          OwnerHandle = iOwner.Handle
        });
    }
    return true;
  }
}
