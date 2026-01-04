// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.RandomTeleportOther
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Magicka.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class RandomTeleportOther(Magicka.Animations iAnimation) : SpecialAbility(iAnimation, Helper.GetHashCodeCustom("#specab_randomteleportother"))
{
  private const float RADIUS = 14f;
  public static readonly int EFFECT = "horrible_staff".GetHashCodeCustom();

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    throw new Exception("RandomTeleport must be called by an entity!");
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    if (!(iOwner is Character))
      return false;
    Vector3 position1 = iOwner.Position;
    Vector3 direction = iOwner.Direction;
    List<Entity> entities = iPlayState.EntityManager.GetEntities(position1, 14f, true);
    Character iOwner1 = (Character) null;
    float num1 = float.MaxValue;
    for (int index = 0; index < entities.Count; ++index)
    {
      if (entities[index] is Character && entities[index] != iOwner)
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
          iOwner1 = entities[index] as Character;
        }
      }
    }
    iPlayState.EntityManager.ReturnEntityList(entities);
    if (iOwner1 == null)
    {
      AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL, iOwner.AudioEmitter);
      return false;
    }
    Vector3 position3 = iOwner1.Position;
    float num3 = (float) Math.Pow(SpecialAbility.RANDOM.NextDouble(), 0.25);
    float num4 = (float) SpecialAbility.RANDOM.NextDouble() * 6.28318548f;
    float num5 = num3 * (float) Math.Cos((double) num4);
    float num6 = num3 * (float) Math.Sin((double) num4);
    position3.X += 20f * num5;
    position3.Z += 20f * num6;
    Vector3 result1 = iOwner1.Position;
    Vector3.Subtract(ref position3, ref result1, out result1);
    result1.Y = 0.0f;
    result1.Normalize();
    Vector3 right = Vector3.Right;
    Vector3 translation = iOwner.CastSource.Translation;
    EffectManager.Instance.StartEffect(RandomTeleportOther.EFFECT, ref translation, ref right, out VisualEffectReference _);
    return NetworkManager.Instance.State == NetworkState.Client || Teleport.Instance.DoTeleport((ISpellCaster) iOwner1, position3, result1, Teleport.TeleportType.Regular);
  }
}
