// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Fear
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

public class Fear : SpecialAbility, ITargetAbility
{
  private const float RANGE = 6f;
  public const float FEAR_TIME = 5f;
  private static Fear mSingelton;
  private static volatile object mSingeltonLock = new object();
  public static readonly int SOUND = "magick_fear".GetHashCodeCustom();
  public static readonly int EFFECT = "magick_fear".GetHashCodeCustom();
  public static readonly int FEARED_EFFECT = "magick_feared".GetHashCodeCustom();
  private AudioEmitter mAudioEmitter = new AudioEmitter();

  public static Fear Instance
  {
    get
    {
      if (Fear.mSingelton == null)
      {
        lock (Fear.mSingeltonLock)
        {
          if (Fear.mSingelton == null)
            Fear.mSingelton = new Fear();
        }
      }
      return Fear.mSingelton;
    }
  }

  public Fear(Magicka.Animations iAnimation)
    : base(iAnimation, "#magick_fear".GetHashCodeCustom())
  {
  }

  private Fear()
    : base(Magicka.Animations.cast_magick_self, "#magick_fear".GetHashCodeCustom())
  {
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    Vector3 right = Vector3.Right;
    Vector3 iPosition1 = iPosition;
    EffectManager.Instance.StartEffect(Fear.EFFECT, ref iPosition1, ref right, out VisualEffectReference _);
    if (NetworkManager.Instance.State != NetworkState.Client)
    {
      List<Entity> entities = iPlayState.EntityManager.GetEntities(iPosition, 6f, true);
      for (int index = 0; index < entities.Count; ++index)
      {
        if (entities[index] is Character character && (!character.IsSelfShielded || character.IsSolidSelfShielded))
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
    AudioManager.Instance.PlayCue(Banks.Spells, Fear.SOUND, this.mAudioEmitter);
    return true;
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    if (!(iOwner is Character))
      return false;
    Vector3 right = Vector3.Right;
    Vector3 position = iOwner.Position;
    EffectManager.Instance.StartEffect(Fear.EFFECT, ref position, ref right, out VisualEffectReference _);
    if (NetworkManager.Instance.State != NetworkState.Client)
    {
      List<Entity> entities = iPlayState.EntityManager.GetEntities(position, 6f, true);
      entities.Remove(iOwner as Entity);
      Character iFearedBy = iOwner as Character;
      for (int index = 0; index < entities.Count; ++index)
      {
        if (entities[index] is Character character && (character.Faction & iFearedBy.Faction) == Factions.NONE && (!character.IsSelfShielded || character.IsSolidSelfShielded))
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
    AudioManager.Instance.PlayCue(Banks.Spells, Fear.SOUND, iOwner.AudioEmitter);
    return true;
  }

  public bool Execute(ISpellCaster iOwner, Entity iTarget, PlayState iPlayState)
  {
    if (!(iTarget is Character))
      return false;
    if (NetworkManager.Instance.State != NetworkState.Client)
    {
      if (NetworkManager.Instance.State == NetworkState.Server)
        NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
        {
          Handle = iTarget.Handle,
          TargetHandle = iOwner.Handle,
          Action = ActionType.Magick,
          Param3I = 6
        });
      (iTarget as Character).Fear(iOwner as Character);
    }
    Vector3 right = Vector3.Right;
    Vector3 position = iOwner.Position;
    EffectManager.Instance.StartEffect(Fear.EFFECT, ref position, ref right, out VisualEffectReference _);
    AudioManager.Instance.PlayCue(Banks.Spells, Fear.SOUND, iOwner.AudioEmitter);
    return true;
  }
}
