// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Teleport
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.Achievements;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Levels.Triggers;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class Teleport : SpecialAbility
{
  private static Teleport mSingelton;
  private static volatile object mSingeltonLock = new object();
  public static readonly int TELEPORT_EFFECT_DISAPPEAR = "magick_teleport_disappear".GetHashCodeCustom();
  public static readonly int TELEPORT_EFFECT_APPEAR = "magick_teleport_appear".GetHashCodeCustom();
  public static readonly int TELEPORT_SOUND_ORIGIN = "magick_teleporta".GetHashCodeCustom();
  public static readonly int TELEPORT_SOUND_DESTINATION = "magick_teleportb".GetHashCodeCustom();
  public static readonly int SMOKEBOMB_EFFECT_DISAPPEAR = "smoke_bomb".GetHashCodeCustom();
  public static readonly int SMOKEBOMB_EFFECT_APPEAR = "smoke_bomb".GetHashCodeCustom();
  public static readonly int SMOKEBOMB_SOUND = "spell_fire_self".GetHashCodeCustom();
  private AudioEmitter mOriginEmitter = new AudioEmitter();
  private AudioEmitter mDestinationEmitter = new AudioEmitter();
  private Random mRandom = new Random();

  public static Teleport Instance
  {
    get
    {
      if (Teleport.mSingelton == null)
      {
        lock (Teleport.mSingeltonLock)
        {
          if (Teleport.mSingelton == null)
            Teleport.mSingelton = new Teleport();
        }
      }
      return Teleport.mSingelton;
    }
  }

  public Teleport(Magicka.Animations iAnimation)
    : base(iAnimation, "#magick_teleport".GetHashCodeCustom())
  {
  }

  private Teleport()
    : base(Magicka.Animations.cast_magick_self, "#magick_teleport".GetHashCodeCustom())
  {
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    throw new Exception("Teleport must be called by an entity!");
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return true;
    base.Execute(iOwner, iPlayState);
    Vector3 result1 = iOwner.Direction;
    Vector3 result2 = iOwner.Position;
    float scaleFactor = 10f;
    Vector3.Multiply(ref result1, scaleFactor, out result1);
    Vector3.Add(ref result1, ref result2, out result2);
    return this.DoTeleport(iOwner, result2, iOwner.Direction, Teleport.TeleportType.Regular);
  }

  public bool DoTeleport(
    ISpellCaster iOwner,
    Vector3 iPosition,
    Vector3 iDirection,
    Teleport.TeleportType iTeleportType)
  {
    if (NetworkManager.Instance.State == NetworkState.Server)
      NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
      {
        Handle = iOwner.Handle,
        Param0F = iPosition.X,
        Param1F = iPosition.Y,
        Param2F = iPosition.Z,
        Action = ActionType.Magick,
        Param3I = 5,
        Param4I = (int) iTeleportType
      });
    PlayState playState = iOwner.PlayState;
    Vector3 position = iOwner.Position;
    Vector3 iDirection1 = iDirection;
    Cue cue = (Cue) null;
    VisualEffectReference oRef;
    switch (iTeleportType)
    {
      case Teleport.TeleportType.Regular:
        EffectManager.Instance.StartEffect(Teleport.TELEPORT_EFFECT_DISAPPEAR, ref position, ref iDirection1, out oRef);
        cue = AudioManager.Instance.GetCue(Banks.Spells, Teleport.TELEPORT_SOUND_ORIGIN);
        break;
      case Teleport.TeleportType.SmokeBomb:
        EffectManager.Instance.StartEffect(Teleport.SMOKEBOMB_EFFECT_DISAPPEAR, ref position, ref iDirection1, out oRef);
        cue = AudioManager.Instance.GetCue(Banks.Spells, Teleport.SMOKEBOMB_SOUND);
        break;
    }
    this.mOriginEmitter.Up = Vector3.Up;
    this.mOriginEmitter.Position = position;
    this.mOriginEmitter.Forward = iDirection1;
    cue.Apply3D(playState.Camera.Listener, this.mOriginEmitter);
    cue.Play();
    Vector3 oPoint;
    double nearestPosition = (double) playState.Level.CurrentScene.NavMesh.GetNearestPosition(ref iPosition, out oPoint, MovementProperties.Water);
    iPosition = oPoint;
    Segment segment = new Segment();
    segment.Origin = oPoint;
    segment.Delta.Y = -4f;
    Vector3 oPos;
    if (playState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out Vector3 _, segment))
    {
      iPosition = oPos;
      iPosition.Y += 0.1f;
    }
    if (iOwner is Character)
    {
      Character character = iOwner as Character;
      iPosition.Y -= character.HeightOffset;
      character.ReleaseEntanglement();
      character.ReleaseAttachedCharacter();
      if (character.Gripper != null)
        character.Gripper.ReleaseAttachedCharacter();
    }
    else
      iPosition.Y += iOwner.Radius;
    switch (iTeleportType)
    {
      case Teleport.TeleportType.Regular:
        EffectManager.Instance.StartEffect(Teleport.TELEPORT_EFFECT_APPEAR, ref iPosition, ref iDirection1, out oRef);
        cue = AudioManager.Instance.GetCue(Banks.Spells, Teleport.TELEPORT_SOUND_DESTINATION);
        break;
      case Teleport.TeleportType.SmokeBomb:
        EffectManager.Instance.StartEffect(Teleport.SMOKEBOMB_EFFECT_DISAPPEAR, ref iPosition, ref iDirection1, out oRef);
        cue = AudioManager.Instance.GetCue(Banks.Spells, Teleport.SMOKEBOMB_SOUND);
        break;
    }
    this.mDestinationEmitter.Up = Vector3.Up;
    this.mDestinationEmitter.Position = iPosition;
    this.mDestinationEmitter.Forward = iDirection1;
    cue.Apply3D(playState.Camera.Listener, this.mDestinationEmitter);
    cue.Play();
    if (iOwner is Entity)
    {
      Entity iEntity = iOwner as Entity;
      segment.Origin = position;
      Vector3.Subtract(ref iPosition, ref segment.Origin, out segment.Delta);
      foreach (TriggerArea triggerArea in playState.Level.CurrentScene.LevelModel.TriggerAreas.Values)
      {
        if (!(triggerArea is AnyTriggerArea) && triggerArea.CollisionSkin.SegmentIntersect(out float _, out Vector3 _, out Vector3 _, segment))
          triggerArea.AddEntity(iEntity);
      }
    }
    Matrix orientation = iOwner.Body.Orientation;
    if (iOwner is Avatar)
    {
      Avatar avatar = iOwner as Avatar;
      Segment seg = new Segment();
      seg.Origin = iOwner.Position;
      seg.Origin.Y += 0.5f;
      seg.Delta.Y = -15.5f;
      bool flag = true;
      float num;
      Vector3 vector3_1;
      Vector3 vector3_2;
      for (int index = 0; index < playState.Level.CurrentScene.Liquids.Length; ++index)
      {
        if (playState.Level.CurrentScene.Liquids[index].SegmentIntersect(out num, out vector3_1, out vector3_2, ref seg, true, false, false))
          flag = false;
      }
      if (!(avatar.Player.Gamer is NetworkGamer) && !playState.Level.CurrentScene.SegmentIntersect(out num, out vector3_1, out vector3_2, seg) && flag)
        AchievementsManager.Instance.AwardAchievement(iOwner.PlayState, "101stairborne");
      iOwner.Body.MoveTo(iPosition, orientation);
      avatar.ResetAfterImages();
    }
    else
      iOwner.Body.MoveTo(iPosition, orientation);
    return true;
  }

  public enum TeleportType
  {
    Regular = 1,
    SmokeBomb = 2,
  }
}
