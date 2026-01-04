// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.ConfuseWho
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class ConfuseWho : SpecialAbility, IAbilityEffect
{
  public const float TIME_TO_LIVE = 10f;
  private const float RANGE = 5f;
  private static ConfuseWho sSingelton = (ConfuseWho) null;
  private static volatile object sSingeltonLock = new object();
  private static List<ConfuseWho.VictimInfo> sVictims = new List<ConfuseWho.VictimInfo>(4);
  public static readonly int EFFECT_CAST = "elderthing_stargaze".GetHashCodeCustom();
  public static readonly int EFFECT_HIT = "elderthing_stargaze_hit".GetHashCodeCustom();
  public static readonly int EFFECT_STATUS = "confused".GetHashCodeCustom();
  public static readonly int SOUND = "magick_thunderbolt".GetHashCodeCustom();

  public static ConfuseWho Instance
  {
    get
    {
      if (ConfuseWho.sSingelton == null)
      {
        lock (ConfuseWho.sSingeltonLock)
        {
          if (ConfuseWho.sSingelton == null)
            ConfuseWho.sSingelton = new ConfuseWho();
        }
      }
      return ConfuseWho.sSingelton;
    }
  }

  private ConfuseWho()
    : base(Magicka.Animations.cast_magick_direct, 0)
  {
  }

  public ConfuseWho(Magicka.Animations iAnimation)
    : base(iAnimation, "#specab_confuewho".GetHashCodeCustom())
  {
    ConfuseWho instance = ConfuseWho.Instance;
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    Vector3 position = iOwner.Position;
    Vector3 result = iOwner.Direction;
    if ((double) result.Z > 0.0)
      Vector3.Negate(ref result, out result);
    EffectManager.Instance.StartEffect(ConfuseWho.EFFECT_CAST, ref position, ref result, out VisualEffectReference _);
    NetworkState state = NetworkManager.Instance.State;
    if (state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer)) || state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer))
    {
      Magicka.GameLogic.Entities.Character iTarget = (Magicka.GameLogic.Entities.Character) null;
      if (iOwner is NonPlayerCharacter)
        iTarget = (iOwner as NonPlayerCharacter).AI.CurrentTarget as Magicka.GameLogic.Entities.Character;
      if (iTarget == null)
        iTarget = this.GetTarget(iPlayState.EntityManager, iOwner, iOwner.Position, iOwner.Direction);
      if (iTarget != null)
      {
        if (state != NetworkState.Offline)
          NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
          {
            ActionType = TriggerActionType.StarGaze,
            Handle = iOwner.Handle,
            Arg = (int) iTarget.Handle
          });
        this.Execute(iOwner, iTarget, iPlayState);
      }
    }
    return true;
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    throw new NotImplementedException();
  }

  private Magicka.GameLogic.Entities.Character GetTarget(
    EntityManager iEntityMan,
    ISpellCaster iOwner,
    Vector3 iPos,
    Vector3 iDir)
  {
    Magicka.GameLogic.Entities.Character target = (Magicka.GameLogic.Entities.Character) null;
    float num = float.MaxValue;
    List<Entity> entities = iEntityMan.GetEntities(iPos, 5f, true);
    foreach (Entity entity in entities)
    {
      Vector3 oPosition;
      if (entity is Magicka.GameLogic.Entities.Character character && character != iOwner && character.ArcIntersect(out oPosition, iPos, iDir, 5f, 0.7853982f, 4f))
      {
        float result;
        Vector3.DistanceSquared(ref iPos, ref oPosition, out result);
        if ((double) result < (double) num)
        {
          num = result;
          target = character;
        }
      }
    }
    iEntityMan.ReturnEntityList(entities);
    return target;
  }

  internal void Execute(ISpellCaster iOwner, Magicka.GameLogic.Entities.Character iTarget, PlayState iPlayState)
  {
    if (iOwner == null || iTarget == null)
      return;
    bool flag = true;
    ConfuseWho.VictimInfo victimInfo = new ConfuseWho.VictimInfo();
    victimInfo.TTL = 10f;
    victimInfo.Victim = iTarget;
    Vector3 position = victimInfo.Victim.Position;
    Vector3 direction = victimInfo.Victim.Direction;
    EffectManager.Instance.StartEffect(ConfuseWho.EFFECT_HIT, ref position, ref direction, out VisualEffectReference _);
    iTarget.Charm(iOwner as Entity, 10f, 0);
    if (iTarget is Avatar)
    {
      victimInfo.Controller = (iTarget as Avatar).Player.Controller;
      if (victimInfo.Controller != null)
        victimInfo.Controller.Invert(true);
    }
    for (int index = 0; index < ConfuseWho.sVictims.Count; ++index)
    {
      if (ConfuseWho.sVictims[index].Victim == iTarget)
      {
        flag = false;
        victimInfo.Effect = ConfuseWho.sVictims[index].Effect;
        ConfuseWho.sVictims[index] = victimInfo;
        break;
      }
    }
    if (flag)
    {
      EffectManager.Instance.StartEffect(ConfuseWho.EFFECT_STATUS, ref position, ref direction, out victimInfo.Effect);
      ConfuseWho.sVictims.Add(victimInfo);
    }
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) ConfuseWho.Instance);
  }

  public bool IsDead => ConfuseWho.sVictims.Count == 0;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    for (int index = 0; index < ConfuseWho.sVictims.Count; ++index)
    {
      ConfuseWho.VictimInfo sVictim = ConfuseWho.sVictims[index];
      sVictim.TTL -= iDeltaTime;
      if ((double) sVictim.TTL <= 0.0 || sVictim.Victim.Dead)
      {
        EffectManager.Instance.Stop(ref sVictim.Effect);
        if (sVictim.Victim is NonPlayerCharacter)
          (sVictim.Victim as NonPlayerCharacter).Confuse(sVictim.Victim.Template.Faction);
        else if (sVictim.Victim is Avatar && sVictim.Controller != null)
          sVictim.Controller.Invert(false);
        ConfuseWho.sVictims.RemoveAt(index);
        --index;
      }
      else
      {
        Vector3 position = sVictim.Victim.Position;
        Vector3 direction = sVictim.Victim.Direction;
        EffectManager.Instance.UpdatePositionDirection(ref sVictim.Effect, ref position, ref direction);
        ConfuseWho.sVictims[index] = sVictim;
      }
    }
  }

  public void OnRemove()
  {
  }

  private struct VictimInfo
  {
    public float TTL;
    public Magicka.GameLogic.Entities.Character Victim;
    public VisualEffectReference Effect;
    public Controller Controller;
  }
}
