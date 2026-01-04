// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.CharacterStates.DeadState
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Statistics;
using Magicka.Network;
using Magicka.WebTools.Paradox.Telemetry;

#nullable disable
namespace Magicka.GameLogic.Entities.CharacterStates;

public class DeadState : BaseState
{
  private static DeadState mSingelton;
  private static volatile object mSingeltonLock = new object();
  public static readonly int SOUND_GIB = "misc_gib".GetHashCodeCustom();

  public static DeadState Instance
  {
    get
    {
      if (DeadState.mSingelton == null)
      {
        lock (DeadState.mSingeltonLock)
        {
          if (DeadState.mSingelton == null)
            DeadState.mSingelton = new DeadState();
        }
      }
      return DeadState.mSingelton;
    }
  }

  public override void OnEnter(Character iOwner)
  {
    if (iOwner.Gripper != null)
      iOwner.Gripper.ReleaseAttachedCharacter();
    iOwner.ReleaseAttachedCharacter();
    iOwner.ReleaseEntanglement();
    if (!iOwner.Undying)
    {
      if (iOwner is NonPlayerCharacter)
        (iOwner as NonPlayerCharacter).AI.Disable();
      for (int index = 0; index < iOwner.Equipment.Length; ++index)
      {
        if (iOwner is Avatar)
          iOwner.Equipment[index].Item.PreviousOwner = (Entity) iOwner;
        else
          iOwner.Equipment[index].Item.PreviousOwner = (Entity) null;
        iOwner.Equipment[index].Item.StopEffects();
        iOwner.Equipment[index].Release(iOwner.PlayState);
        iOwner.Equipment[index].Item.Body.SetActive();
      }
    }
    iOwner.CharacterBody.AllowMove = false;
    iOwner.CharacterBody.AllowRotate = false;
    double num = MagickaMath.Random.NextDouble();
    if (iOwner.Drowning)
    {
      if (iOwner.HasAnimation(Animations.die_drown1))
      {
        if (iOwner.HasAnimation(Animations.die_drown2))
        {
          if (num > 0.66600000858306885)
            iOwner.ForceAnimation(Animations.die_drown2);
          else if (num > 0.33300000429153442)
            iOwner.ForceAnimation(Animations.die_drown1);
          else
            iOwner.ForceAnimation(Animations.die_drown);
        }
        else if (num > 0.5)
          iOwner.ForceAnimation(Animations.die_drown1);
        else
          iOwner.ForceAnimation(Animations.die_drown);
      }
      else
        iOwner.ForceAnimation(Animations.die_drown);
    }
    else if (iOwner.HasAnimation(Animations.die1) && num > 0.5)
      iOwner.GoToAnimation(Animations.die1, 0.05f);
    else
      iOwner.GoToAnimation(Animations.die0, 0.05f);
    if (!(iOwner is Avatar avatar) || avatar.Player == null || avatar.Player != ControlManager.Instance.MenuController.Player)
      return;
    TelemetryUtils.SendPlayerDeath(iOwner);
  }

  public override BaseState Update(Character iOwner, float iDeltaTime)
  {
    if ((double) iOwner.HitPoints > 0.0 && !float.IsNaN(iOwner.UndyingTimer) && !iOwner.AnimationController.IsPlaying)
    {
      iOwner.SpawnAnimation = iOwner.Template.AnimationClips[0][150] != null ? Animations.revive : Animations.hit;
      return (BaseState) RessurectionState.Instance;
    }
    if (iOwner.AnimationController.HasFinished && !iOwner.Dead)
    {
      NetworkState state = NetworkManager.Instance.State;
      if (state == NetworkState.Server)
      {
        CharacterDieMessage iMessage;
        iMessage.Handle = iOwner.Handle;
        iMessage.Drown = iOwner.Drowning;
        iMessage.Overkill = false;
        iMessage.KillerHandle = iOwner.LastAttacker == null ? ushort.MaxValue : iOwner.LastAttacker.Handle;
        NetworkManager.Instance.Interface.SendMessage<CharacterDieMessage>(ref iMessage);
      }
      if (state != NetworkState.Client)
      {
        if (!iOwner.NotedKilledEvent)
        {
          StatisticsManager.Instance.AddKillEvent(iOwner.PlayState, (Entity) iOwner, iOwner.LastAttacker);
          iOwner.NotedKilledEvent = true;
        }
        iOwner.Die();
      }
    }
    else if (iOwner.Overkilled && !iOwner.CannotDieWithoutExplicitKill)
    {
      NetworkState state = NetworkManager.Instance.State;
      if (state == NetworkState.Server)
      {
        CharacterDieMessage iMessage;
        iMessage.Handle = iOwner.Handle;
        iMessage.Drown = false;
        iMessage.Overkill = true;
        iMessage.KillerHandle = iOwner.LastAttacker == null ? ushort.MaxValue : iOwner.LastAttacker.Handle;
        NetworkManager.Instance.Interface.SendMessage<CharacterDieMessage>(ref iMessage);
      }
      if (state != NetworkState.Client)
      {
        if (!iOwner.NotedKilledEvent)
        {
          StatisticsManager.Instance.AddKillEvent(iOwner.PlayState, (Entity) iOwner, iOwner.LastAttacker);
          iOwner.NotedKilledEvent = true;
        }
        if (!iOwner.mDead)
          iOwner.Die();
        iOwner.RemoveAfterDeath = true;
        if (iOwner.HasGibs())
        {
          iOwner.SpawnGibs();
          AudioManager.Instance.PlayCue(Banks.Misc, DeadState.SOUND_GIB, iOwner.AudioEmitter);
        }
        else if (iOwner.BloatKilled)
          iOwner.Terminate(false, false);
      }
    }
    return (BaseState) null;
  }

  public override void OnExit(Character iOwner)
  {
  }
}
