// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Items.EventStorage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Magicka.GameLogic.Entities.Items;

[StructLayout(LayoutKind.Explicit)]
public struct EventStorage
{
  [FieldOffset(0)]
  private EventType mEventType;
  [FieldOffset(4)]
  public DamageEvent DamageEvent;
  [FieldOffset(4)]
  public SplashEvent SplashEvent;
  [FieldOffset(4)]
  public RemoveEvent RemoveProjectileEvent;
  [FieldOffset(4)]
  public PlayEffectEvent PlayEffectEvent;
  [FieldOffset(4)]
  public PlaySoundEvent PlaySoundEvent;
  [FieldOffset(4)]
  public SpawnDecalEvent SpawnDecalEvent;
  [FieldOffset(4)]
  public CameraShakeEvent CameraShakeEvent;
  [FieldOffset(4)]
  public BlastEvent BlastEvent;
  [FieldOffset(4)]
  public SpawnEvent SpawnEvent;
  [FieldOffset(4)]
  public OverKillEvent OverKillEvent;
  [FieldOffset(4)]
  public SpawnGibsEvent SpawnGibsEvent;
  [FieldOffset(4)]
  public SpawnItemEvent SpawnItemEvent;
  [FieldOffset(4)]
  public SpawnMagickEvent SpawnMagickEvent;
  [FieldOffset(4)]
  public SpawnMissileEvent SpawnMissileEvent;
  [FieldOffset(4)]
  public LightEvent LightEvent;
  [FieldOffset(8)]
  public CastMagickEvent CastMagickEvent;
  [FieldOffset(4)]
  public DamageOwnerEvent DamageOwnerEvent;
  [FieldOffset(4)]
  public CallbackEvent CallbackEvent;

  public EventType EventType => this.mEventType;

  public EventStorage(DamageEvent iEvent)
    : this()
  {
    this.mEventType = EventType.Damage;
    this.DamageEvent = iEvent;
  }

  public EventStorage(SplashEvent iEvent)
    : this()
  {
    this.mEventType = EventType.Splash;
    this.SplashEvent = iEvent;
  }

  public EventStorage(RemoveEvent iEvent)
    : this()
  {
    this.mEventType = EventType.Remove;
    this.RemoveProjectileEvent = iEvent;
  }

  public EventStorage(PlayEffectEvent iEvent)
    : this()
  {
    this.mEventType = EventType.Effect;
    this.PlayEffectEvent = iEvent;
  }

  public EventStorage(PlaySoundEvent iEvent)
    : this()
  {
    this.mEventType = EventType.Sound;
    this.PlaySoundEvent = iEvent;
  }

  public EventStorage(SpawnDecalEvent iEvent)
    : this()
  {
    this.mEventType = EventType.Decal;
    this.SpawnDecalEvent = iEvent;
  }

  public EventStorage(CameraShakeEvent iEvent)
    : this()
  {
    this.mEventType = EventType.CameraShake;
    this.CameraShakeEvent = iEvent;
  }

  public EventStorage(BlastEvent iEvent)
    : this()
  {
    this.mEventType = EventType.Blast;
    this.BlastEvent = iEvent;
  }

  public EventStorage(SpawnEvent iEvent)
    : this()
  {
    this.mEventType = EventType.Spawn;
    this.SpawnEvent = iEvent;
  }

  public EventStorage(OverKillEvent iEvent)
    : this()
  {
    this.mEventType = EventType.Overkill;
    this.OverKillEvent = iEvent;
  }

  public EventStorage(SpawnItemEvent iEvent)
    : this()
  {
    this.mEventType = EventType.SpawnItem;
    this.SpawnItemEvent = iEvent;
  }

  public EventStorage(SpawnMagickEvent iEvent)
    : this()
  {
    this.mEventType = EventType.SpawnMagick;
    this.SpawnMagickEvent = iEvent;
  }

  public EventStorage(LightEvent iEvent)
    : this()
  {
    this.mEventType = EventType.Light;
    this.LightEvent = iEvent;
  }

  public EventStorage(CastMagickEvent iEvent)
    : this()
  {
    this.mEventType = EventType.CastMagick;
    this.CastMagickEvent = iEvent;
  }

  public EventStorage(CallbackEvent iEvent)
    : this()
  {
    this.mEventType = EventType.Callback;
    this.CallbackEvent = iEvent;
  }

  public DamageResult Execute(Entity iItem, Entity iTarget, ref Vector3? iPosition)
  {
    DamageResult damageResult = DamageResult.None;
    switch (this.mEventType)
    {
      case EventType.Damage:
        damageResult |= this.DamageEvent.Execute(iItem, iTarget);
        break;
      case EventType.Splash:
        damageResult |= this.SplashEvent.Execute(iItem, iTarget, ref iPosition);
        break;
      case EventType.Sound:
        this.PlaySoundEvent.Execute(iItem, iTarget, ref iPosition);
        break;
      case EventType.Effect:
        this.PlayEffectEvent.Execute(iItem, iTarget, ref iPosition);
        break;
      case EventType.Remove:
        this.RemoveProjectileEvent.Execute(iItem, iTarget);
        break;
      case EventType.CameraShake:
        this.CameraShakeEvent.Execute(iItem, iTarget, ref iPosition);
        break;
      case EventType.Decal:
        this.SpawnDecalEvent.Execute(iItem, iTarget, ref iPosition);
        break;
      case EventType.Blast:
        damageResult |= this.BlastEvent.Execute(iItem, iTarget, ref iPosition);
        break;
      case EventType.Spawn:
        this.SpawnEvent.Execute(iItem, iTarget);
        break;
      case EventType.Overkill:
        this.OverKillEvent.Execute(iItem, iTarget);
        break;
      case EventType.SpawnGibs:
        this.SpawnGibsEvent.Execute(iItem, iTarget);
        break;
      case EventType.SpawnItem:
        this.SpawnItemEvent.Execute(iItem, iTarget);
        break;
      case EventType.SpawnMagick:
        this.SpawnMagickEvent.Execute(iItem, iTarget);
        break;
      case EventType.SpawnMissile:
        this.SpawnMissileEvent.Execute(iItem, iTarget);
        break;
      case EventType.Light:
        this.LightEvent.Execute(iItem, iTarget);
        break;
      case EventType.CastMagick:
        this.CastMagickEvent.Execute(iItem, iTarget);
        break;
      case EventType.DamageOwner:
        int num = (int) this.DamageOwnerEvent.Execute(iItem, iTarget);
        break;
      case EventType.Callback:
        this.CallbackEvent.Execute(iItem, iTarget);
        break;
    }
    return damageResult;
  }

  public EventStorage(ContentReader iInput)
    : this()
  {
    this.mEventType = (EventType) iInput.ReadByte();
    switch (this.mEventType)
    {
      case EventType.Damage:
        this.DamageEvent = new DamageEvent(iInput);
        break;
      case EventType.Splash:
        this.SplashEvent = new SplashEvent(iInput);
        break;
      case EventType.Sound:
        this.PlaySoundEvent = new PlaySoundEvent(iInput);
        break;
      case EventType.Effect:
        this.PlayEffectEvent = new PlayEffectEvent(iInput);
        break;
      case EventType.Remove:
        this.RemoveProjectileEvent = new RemoveEvent(iInput);
        break;
      case EventType.CameraShake:
        this.CameraShakeEvent = new CameraShakeEvent(iInput);
        break;
      case EventType.Decal:
        this.SpawnDecalEvent = new SpawnDecalEvent(iInput);
        break;
      case EventType.Blast:
        this.BlastEvent = new BlastEvent(iInput);
        break;
      case EventType.Spawn:
        this.SpawnEvent = new SpawnEvent(iInput);
        break;
      case EventType.Overkill:
        this.OverKillEvent = new OverKillEvent(iInput);
        break;
      case EventType.SpawnGibs:
        this.SpawnGibsEvent = new SpawnGibsEvent(iInput);
        break;
      case EventType.SpawnItem:
        this.SpawnItemEvent = new SpawnItemEvent(iInput);
        break;
      case EventType.SpawnMagick:
        this.SpawnMagickEvent = new SpawnMagickEvent(iInput);
        break;
      case EventType.SpawnMissile:
        this.SpawnMissileEvent = new SpawnMissileEvent(iInput);
        break;
      case EventType.Light:
        this.LightEvent = new LightEvent(iInput);
        break;
      case EventType.CastMagick:
        this.CastMagickEvent = new CastMagickEvent(iInput);
        break;
      case EventType.DamageOwner:
        this.DamageOwnerEvent = new DamageOwnerEvent(iInput);
        break;
      case EventType.Callback:
        this.CallbackEvent = new CallbackEvent(iInput);
        break;
      default:
        throw new Exception("No event specified");
    }
  }
}
