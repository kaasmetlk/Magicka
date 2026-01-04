// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.CharacterStates.ChargeState
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework.Audio;

#nullable disable
namespace Magicka.GameLogic.Entities.CharacterStates;

public class ChargeState : BaseState
{
  private static ChargeState mSingelton;
  private static volatile object mSingeltonLock = new object();
  public static readonly int CHARGE_SOUND_HASH = "spell_projectile_precharge".GetHashCodeCustom();
  public static readonly int CHARGE_LOOP_SOUND_HASH = "spell_projectile_charge_loop".GetHashCodeCustom();

  public static ChargeState Instance
  {
    get
    {
      if (ChargeState.mSingelton == null)
      {
        lock (ChargeState.mSingeltonLock)
        {
          if (ChargeState.mSingelton == null)
            ChargeState.mSingelton = new ChargeState();
        }
      }
      return ChargeState.mSingelton;
    }
  }

  public override void OnEnter(Character iOwner)
  {
    iOwner.SetInvisible(0.0f);
    iOwner.Ethereal(false, 1f, 1f);
    switch (iOwner.CastType)
    {
      case CastType.Force:
        iOwner.GoToAnimation(Animations.charge_force, 0.075f);
        break;
      case CastType.Area:
        iOwner.GoToAnimation(Animations.charge_area, 0.075f);
        break;
    }
    iOwner.ChargeCue = AudioManager.Instance.PlayCue(Banks.Spells, ChargeState.CHARGE_SOUND_HASH, iOwner.AudioEmitter);
    iOwner.TurnSpeed *= 0.125f;
    iOwner.SpellPower = 0.0f;
    if (!(iOwner is Avatar))
      return;
    (iOwner as Avatar).ChargeUnlocked = false;
  }

  public override BaseState Update(Character iOwner, float iDeltaTime)
  {
    BaseState baseState = this.UpdateBloatDeath(iOwner, iDeltaTime) ?? this.UpdateHit(iOwner, iDeltaTime);
    if (baseState != null)
      return baseState;
    if (iOwner.PlayState.IsInCutscene)
      return (BaseState) IdleState.Instance;
    if (!iOwner.AnimationController.IsLooping)
    {
      if (iOwner.AnimationController.HasFinished)
      {
        switch (iOwner.CastType)
        {
          case CastType.Force:
            iOwner.GoToAnimation(Animations.charge_force_loop, 0.075f);
            break;
          case CastType.Area:
            iOwner.GoToAnimation(Animations.charge_area_loop, 0.075f);
            break;
        }
        if (iOwner.ChargeCue != null)
          iOwner.ChargeCue.Stop(AudioStopOptions.AsAuthored);
        iOwner.ChargeCue = AudioManager.Instance.PlayCue(Banks.Spells, ChargeState.CHARGE_LOOP_SOUND_HASH, iOwner.AudioEmitter);
        iOwner.SpellPower = 1f;
      }
      else if (!iOwner.AnimationController.CrossFadeEnabled)
        iOwner.SpellPower = iOwner.AnimationController.Time / iOwner.AnimationController.AnimationClip.Duration;
    }
    return iOwner is Avatar && ((iOwner.CastType != CastType.Force ? 0 : (!(iOwner as Avatar).CastButton(CastType.Force) ? 1 : 0)) | (iOwner.CastType != CastType.Area ? 0 : (!(iOwner as Avatar).CastButton(CastType.Area) ? 1 : 0))) != 0 ? (BaseState) CastState.Instance : (BaseState) null;
  }

  public override void OnExit(Character iOwner)
  {
    if (iOwner.ChargeCue != null && !iOwner.ChargeCue.IsStopping)
      iOwner.ChargeCue.Stop(AudioStopOptions.AsAuthored);
    if (iOwner.Spell.Element == Elements.None)
      return;
    iOwner.CastSpell(true, "");
  }
}
