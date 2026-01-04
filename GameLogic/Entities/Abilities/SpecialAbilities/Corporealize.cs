// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Corporealize
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

internal class Corporealize : SpecialAbility
{
  private static Corporealize mSingelton;
  private static volatile object mSingeltonLock = new object();
  private static readonly int SOUND_HASH = "magick_corporealize".GetHashCodeCustom();

  public static Corporealize Instance
  {
    get
    {
      if (Corporealize.mSingelton == null)
      {
        lock (Corporealize.mSingeltonLock)
        {
          if (Corporealize.mSingelton == null)
            Corporealize.mSingelton = new Corporealize();
        }
      }
      return Corporealize.mSingelton;
    }
  }

  public Corporealize(Magicka.Animations iAnimation)
    : base(iAnimation, "#magick_corporealize".GetHashCodeCustom())
  {
  }

  public Corporealize()
    : base(Magicka.Animations.cast_magick_self, "#magick_corporealize".GetHashCodeCustom())
  {
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    return this.Execute((ISpellCaster) null, iPlayState);
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    AudioManager.Instance.PlayCue(Banks.Spells, Corporealize.SOUND_HASH);
    base.Execute(iOwner, iPlayState);
    Flash.Instance.Execute(iPlayState.Scene, 0.2f);
    List<Entity> entities = iPlayState.EntityManager.GetEntities(iOwner.Position, 30f, false);
    for (int index = 0; index < entities.Count; ++index)
    {
      Entity entity = entities[index];
      if (entity is Character character)
        character.IsEthereal = false;
      if (entity is BossDamageZone bossDamageZone)
      {
        bossDamageZone.IsEthereal = false;
        if (bossDamageZone.Owner is Grimnir2)
          (bossDamageZone.Owner as Grimnir2).Corporealize();
        if (bossDamageZone.Owner is Grimnir)
          (bossDamageZone.Owner as Grimnir).Corporealize();
        if (bossDamageZone.Owner is Death)
          (bossDamageZone.Owner as Death).Corporealize();
      }
      if (entity is SummonDeath.MagickDeath)
        (entity as SummonDeath.MagickDeath).IsEthereal = false;
    }
    return true;
  }
}
