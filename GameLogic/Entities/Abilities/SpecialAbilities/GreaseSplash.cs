// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.GreaseSplash
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class GreaseSplash : SpecialAbility
{
  private static GreaseSplash sSingelton;
  private static volatile object sSingeltonLock = new object();
  private static readonly int NUM_OF_FIELDS = 7;
  private static readonly float RANGE = 3f;

  public static GreaseSplash Instance
  {
    get
    {
      if (GreaseSplash.sSingelton == null)
      {
        lock (GreaseSplash.sSingeltonLock)
        {
          if (GreaseSplash.sSingelton == null)
            GreaseSplash.sSingelton = new GreaseSplash();
        }
      }
      return GreaseSplash.sSingelton;
    }
  }

  private GreaseSplash()
    : base(Magicka.Animations.cast_magick_direct, "#magick_grease".GetHashCodeCustom())
  {
  }

  public GreaseSplash(Magicka.Animations iAnimation)
    : base(iAnimation, "#magick_grease".GetHashCodeCustom())
  {
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    throw new Exception("Grease cannot be spawned without an owner!");
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    if (NetworkManager.Instance.State == NetworkState.Client)
      return false;
    Vector3 result1 = iOwner.Direction;
    Vector3 up = Vector3.Up;
    AnimatedLevelPart iAnimation = (AnimatedLevelPart) null;
    for (int index = 0; index < GreaseSplash.NUM_OF_FIELDS; ++index)
    {
      Quaternion result2;
      Quaternion.CreateFromYawPitchRoll(3.14159274f / (float) GreaseSplash.NUM_OF_FIELDS * (float) index, 0.0f, 0.0f, out result2);
      Vector3.Transform(ref result1, ref result2, out result1);
      float scaleFactor = (float) (1.0 + MagickaMath.Random.NextDouble() * (double) GreaseSplash.RANGE);
      Vector3 result3;
      Vector3.Multiply(ref result1, scaleFactor, out result3);
      result3 = (iOwner.Position + result3) with
      {
        Y = 0.0f
      };
      Grease.GreaseField instance = Grease.GreaseField.GetInstance(iOwner.PlayState);
      instance.Initialize(iOwner, iAnimation, ref result3, ref up);
      iOwner.PlayState.EntityManager.AddEntity((Entity) instance);
      if (NetworkManager.Instance.State == NetworkState.Server)
        NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
        {
          Handle = instance.Handle,
          Position = result3,
          Direction = result1,
          Arg = iAnimation == null ? (int) ushort.MaxValue : (int) iAnimation.Handle,
          Id = (int) iOwner.Handle,
          ActionType = TriggerActionType.SpawnGrease
        });
    }
    return true;
  }
}
