// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Regenerate
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class Regenerate : SpecialAbility
{
  private static Regenerate sSingelton;
  private static volatile object sSingeltonLock = new object();
  private int HITPOINTS = 13000;

  public static Regenerate Instance
  {
    get
    {
      if (Regenerate.sSingelton == null)
      {
        lock (Regenerate.sSingeltonLock)
        {
          if (Regenerate.sSingelton == null)
            Regenerate.sSingelton = new Regenerate();
        }
      }
      return Regenerate.sSingelton;
    }
  }

  private Regenerate()
    : base(Magicka.Animations.None, "#specab_drain".GetHashCodeCustom())
  {
  }

  public Regenerate(Magicka.Animations iAnimation)
    : base(iAnimation, "#specab_drain".GetHashCodeCustom())
  {
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    throw new Exception("Regenerate cannot be spawned without an owner!");
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    iOwner.Damage((float) -this.HITPOINTS, Elements.Life);
    return true;
  }
}
