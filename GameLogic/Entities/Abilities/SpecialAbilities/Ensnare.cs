// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Ensnare
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.Network;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class Ensnare : SpecialAbility
{
  private static Ensnare mSingelton;
  private static volatile object mSingeltonLock = new object();

  public static Ensnare Instance
  {
    get
    {
      if (Ensnare.mSingelton == null)
      {
        lock (Ensnare.mSingeltonLock)
        {
          if (Ensnare.mSingelton == null)
            Ensnare.mSingelton = new Ensnare();
        }
      }
      return Ensnare.mSingelton;
    }
  }

  public Ensnare(Magicka.Animations iAnimation)
    : base(iAnimation, "#specab_root".GetHashCodeCustom())
  {
  }

  private Ensnare()
    : base(Magicka.Animations.cast_magick_direct, "#specab_root".GetHashCodeCustom())
  {
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    return this.Execute(iPosition, (ISpellCaster) null, iPlayState);
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    return this.Execute(iOwner.Position, iOwner, iPlayState);
  }

  private bool Execute(Vector3 iPosition, ISpellCaster iOwner, PlayState iPlayState)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return false;
    base.Execute(iOwner, iPlayState);
    Character character1 = (Character) null;
    float num = float.MaxValue;
    List<Entity> entities = iPlayState.EntityManager.GetEntities(iOwner.Position, 10f, true);
    Vector3 vector3 = iPosition;
    Vector3 vector1 = new Vector3();
    if (iOwner != null)
    {
      vector1 = iOwner.Direction;
    }
    else
    {
      vector1 = new Vector3((float) SpecialAbility.RANDOM.NextDouble(), 0.0f, (float) SpecialAbility.RANDOM.NextDouble());
      vector1.Normalize();
    }
    vector3.Y = 0.0f;
    for (int index = 0; index < entities.Count; ++index)
    {
      if (entities[index] is Character character2 && (iOwner == null || character2 != iOwner))
      {
        Vector3 position = character2.Position with
        {
          Y = 0.0f
        };
        float result1;
        Vector3.DistanceSquared(ref vector3, ref position, out result1);
        if ((double) result1 < (double) num)
        {
          Vector3 result2;
          Vector3.Subtract(ref position, ref vector3, out result2);
          result2.Normalize();
          float result3;
          Vector3.Dot(ref vector1, ref result2, out result3);
          if ((double) result3 > 0.699999988079071)
          {
            character1 = character2;
            num = result1;
          }
        }
      }
    }
    iPlayState.EntityManager.ReturnEntityList(entities);
    if (character1 != null)
    {
      character1.Entangle(4f);
      return true;
    }
    AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL, iOwner.AudioEmitter);
    return false;
  }
}
