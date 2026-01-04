// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Tornado
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Magicka.Network;
using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class Tornado : SpecialAbility
{
  public const float MAGICK_TTL = 15f;
  private static Tornado sSingelton;
  private static volatile object sSingeltonLock = new object();
  public static readonly int AMBIENCE = "magick_tornado_rumble".GetHashCodeCustom();
  public static readonly int EFFECT = "magick_tornado".GetHashCodeCustom();
  public static readonly int HIT_EFFECT = "magick_tornado_hit".GetHashCodeCustom();

  public static Tornado Instance
  {
    get
    {
      if (Tornado.sSingelton == null)
      {
        lock (Tornado.sSingeltonLock)
        {
          if (Tornado.sSingelton == null)
            Tornado.sSingelton = new Tornado();
        }
      }
      return Tornado.sSingelton;
    }
  }

  private Tornado()
    : base(Magicka.Animations.cast_magick_direct, "#magick_tornado".GetHashCodeCustom())
  {
  }

  public Tornado(Magicka.Animations iAnimation)
    : base(iAnimation, "#magick_tornado".GetHashCodeCustom())
  {
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    if (NetworkManager.Instance.State != NetworkState.Client)
    {
      Vector3 oPoint;
      double nearestPosition = (double) iPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref iPosition, out oPoint, MovementProperties.Default);
      iPosition = oPoint;
      Matrix identity = Matrix.Identity with
      {
        Translation = iPosition
      };
      TornadoEntity instance = TornadoEntity.GetInstance();
      instance.Initialize(iPlayState, identity, (ISpellCaster) null);
      iPlayState.EntityManager.AddEntity((Entity) instance);
      if (NetworkManager.Instance.State == NetworkState.Server)
      {
        TriggerActionMessage iMessage = new TriggerActionMessage();
        iMessage.Handle = instance.Handle;
        iMessage.Id = 0;
        iMessage.Position = iPosition;
        Quaternion.CreateFromRotationMatrix(ref identity, out iMessage.Orientation);
        NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref iMessage);
      }
    }
    return true;
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    if (NetworkManager.Instance.State != NetworkState.Client)
    {
      Vector3 result1 = iOwner.Position;
      Vector3 result2 = iOwner.Direction;
      Vector3.Multiply(ref result2, 4f, out result2);
      Vector3.Add(ref result2, ref result1, out result1);
      Vector3 oPoint;
      double nearestPosition = (double) iPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref result1, out oPoint, MovementProperties.Default);
      result1 = oPoint;
      Matrix identity = Matrix.Identity with
      {
        Translation = result1,
        Forward = result2
      };
      TornadoEntity instance = TornadoEntity.GetInstance();
      instance.Initialize(iPlayState, identity, iOwner);
      iPlayState.EntityManager.AddEntity((Entity) instance);
      if (NetworkManager.Instance.State == NetworkState.Server)
      {
        TriggerActionMessage iMessage = new TriggerActionMessage();
        iMessage.ActionType = TriggerActionType.SpawnTornado;
        iMessage.Handle = instance.Handle;
        iMessage.Id = (int) iOwner.Handle;
        iMessage.Position = result1;
        Quaternion.CreateFromRotationMatrix(ref identity, out iMessage.Orientation);
        NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref iMessage);
      }
    }
    return true;
  }
}
