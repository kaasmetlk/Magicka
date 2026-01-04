// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.FearLight
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class FearLight(Magicka.Animations iAnimation) : SpecialAbility(iAnimation, Helper.GetHashCodeCustom("#specab_fear"))
{
  private const float RANGE = 10f;
  public const float FEAR_TIME = 6f;
  public static readonly int SOUND = "spell_arcane_ray_stage4".GetHashCodeCustom();
  public static readonly int EFFECT = "gandalf_fear".GetHashCodeCustom();
  public static readonly int FEARED_EFFECT = "magick_feared".GetHashCodeCustom();
  private AudioEmitter mAudioEmitter = new AudioEmitter();

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    Vector3 right = Vector3.Right;
    Vector3 iPosition1 = iPosition;
    EffectManager.Instance.StartEffect(FearLight.EFFECT, ref iPosition1, ref right, out VisualEffectReference _);
    if (NetworkManager.Instance.State != NetworkState.Client)
    {
      List<Entity> entities = iPlayState.EntityManager.GetEntities(iPosition, 10f, false);
      for (int index = 0; index < entities.Count; ++index)
      {
        if (entities[index] is Character character)
        {
          if (NetworkManager.Instance.State == NetworkState.Server)
            NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
            {
              Handle = character.Handle,
              TargetHandle = (ushort) 0,
              Param0F = iPosition.X,
              Param1F = iPosition.Y,
              Param2F = iPosition.Z,
              Action = ActionType.Magick,
              Param3I = 6
            });
          character.Fear(iPosition);
        }
      }
    }
    this.mAudioEmitter.Position = iPosition;
    this.mAudioEmitter.Up = Vector3.Up;
    this.mAudioEmitter.Forward = Vector3.Forward;
    AudioManager.Instance.PlayCue(Banks.Spells, FearLight.SOUND, this.mAudioEmitter);
    return true;
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    if (!(iOwner is Character))
      return false;
    Vector3 right = Vector3.Right;
    Vector3 position = iOwner.Position;
    EffectManager.Instance.StartEffect(FearLight.EFFECT, ref position, ref right, out VisualEffectReference _);
    if (NetworkManager.Instance.State != NetworkState.Client)
    {
      List<Entity> entities = iPlayState.EntityManager.GetEntities(position, 10f, false);
      entities.Remove(iOwner as Entity);
      Character iFearedBy = iOwner as Character;
      for (int index = 0; index < entities.Count; ++index)
      {
        if (entities[index] is Character character && (character.Faction & iFearedBy.Faction) == Factions.NONE)
        {
          if (NetworkManager.Instance.State == NetworkState.Server)
            NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
            {
              Handle = character.Handle,
              TargetHandle = iFearedBy.Handle,
              Action = ActionType.Magick,
              Param3I = 6
            });
          character.Fear(iFearedBy);
        }
      }
      iPlayState.EntityManager.ReturnEntityList(entities);
    }
    AudioManager.Instance.PlayCue(Banks.Spells, FearLight.SOUND, iOwner.AudioEmitter);
    return true;
  }
}
