// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.CTD
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Localization;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

internal class CTD : SpecialAbility
{
  private static CTD sSingelton;
  private static volatile object sSingeltonLock = new object();
  internal static readonly int EFFECT = "magick_ctd".GetHashCodeCustom();
  internal static readonly int SOUND = "magick_ctd".GetHashCodeCustom();
  internal static readonly int MESSAGE = "#add_connectiontolost".GetHashCodeCustom();
  private AudioEmitter mAudioEmitter = new AudioEmitter();

  public static CTD Instance
  {
    get
    {
      if (CTD.sSingelton == null)
      {
        lock (CTD.sSingeltonLock)
        {
          if (CTD.sSingelton == null)
            CTD.sSingelton = new CTD();
        }
      }
      return CTD.sSingelton;
    }
  }

  private CTD()
    : base(Magicka.Animations.cast_magick_sweep, "#magick_ctd".GetHashCodeCustom())
  {
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    NetworkState state = NetworkManager.Instance.State;
    if (state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer)) || state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer))
    {
      Vector3 position = iOwner.Position;
      Vector3 direction = iOwner.Direction;
      Vector2 vector2 = new Vector2(direction.X, direction.Z);
      Character character = (Character) null;
      List<Entity> entities = iPlayState.EntityManager.GetEntities(position, 20f, true, true);
      for (int index = 0; index < entities.Count; ++index)
      {
        if (!(entities[index] is Character))
          entities.RemoveAt(index--);
        else if (entities[index] is Character && ((entities[index] as Character).CannotDieWithoutExplicitKill || (double) (entities[index] as Character).MaxHitPoints > 100000.0))
          entities.RemoveAt(index--);
      }
      if (entities.Count > 0)
        character = entities[SpecialAbility.RANDOM.Next(entities.Count)] as Character;
      iPlayState.EntityManager.ReturnEntityList(entities);
      if (character != null)
      {
        Matrix result;
        Matrix.CreateScale(character.Radius, character.Capsule.Length + character.Radius * 2f, character.Radius, out result);
        result.Translation = character.Position;
        EffectManager.Instance.StartEffect(CTD.EFFECT, ref result, out VisualEffectReference _);
        AudioManager.Instance.PlayCue(Banks.Additional, CTD.SOUND, character.AudioEmitter);
        if (state != NetworkState.Client)
          character.Terminate(true, false);
        if (state != NetworkState.Offline)
        {
          if (iPlayState.Level.CurrentScene.RuleSet is SurvivalRuleset && character is NonPlayerCharacter && character.DisplayName != 0)
            NetworkChat.Instance.AddMessage(LanguageManager.Instance.GetString(CTD.MESSAGE).Replace("#1;", LanguageManager.Instance.GetString(character.DisplayName)));
          NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
          {
            Action = ActionType.Magick,
            Handle = iOwner.Handle,
            TargetHandle = character.Handle,
            Param3I = 23
          });
        }
        return true;
      }
      AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL, iOwner.AudioEmitter);
    }
    return false;
  }
}
