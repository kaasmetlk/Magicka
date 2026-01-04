// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.StarGaze
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

public class StarGaze : SpecialAbility, IAbilityEffect
{
  public const float TIME_TO_LIVE = 10f;
  private const float RANGE = 20f;
  private static StarGaze sSingelton;
  private static volatile object sSingeltonLock = new object();
  private static List<StarGaze.VictimInfo> sVictims = new List<StarGaze.VictimInfo>(4);
  public static readonly int EFFECT_CAST = "elderthing_stargaze".GetHashCodeCustom();
  public static readonly int EFFECT_HIT = "elderthing_stargaze_hit".GetHashCodeCustom();
  public static readonly int EFFECT_STATUS = "confused".GetHashCodeCustom();

  public static StarGaze Instance
  {
    get
    {
      if (StarGaze.sSingelton == null)
      {
        lock (StarGaze.sSingeltonLock)
        {
          if (StarGaze.sSingelton == null)
            StarGaze.sSingelton = new StarGaze();
        }
      }
      return StarGaze.sSingelton;
    }
  }

  private StarGaze()
    : base(Magicka.Animations.cast_magick_direct, 0)
  {
  }

  public StarGaze(Magicka.Animations iAnimation)
    : base(iAnimation, 0)
  {
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    Vector3 position = iOwner.Position;
    Vector3 result = iOwner.Direction;
    if ((double) result.Z > 0.0)
      Vector3.Negate(ref result, out result);
    EffectManager.Instance.StartEffect(StarGaze.EFFECT_CAST, ref position, ref result, out VisualEffectReference _);
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
    List<Entity> entities = iEntityMan.GetEntities(iPos, 20f, true);
    foreach (Entity entity in entities)
    {
      Vector3 oPosition;
      if (entity is Magicka.GameLogic.Entities.Character character && character != iOwner && character.ArcIntersect(out oPosition, iPos, iDir, 20f, 0.7853982f, 4f))
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
    StarGaze.VictimInfo victimInfo = new StarGaze.VictimInfo();
    victimInfo.TTL = 10f;
    victimInfo.Victim = iTarget;
    Vector3 position = victimInfo.Victim.Position;
    Vector3 direction = victimInfo.Victim.Direction;
    EffectManager.Instance.StartEffect(StarGaze.EFFECT_HIT, ref position, ref direction, out VisualEffectReference _);
    iTarget.Charm(iOwner as Entity, 10f, 0);
    switch (iTarget)
    {
      case Avatar _:
        victimInfo.Controller = (iTarget as Avatar).Player.Controller;
        if (victimInfo.Controller != null)
        {
          victimInfo.Controller.Invert(true);
          break;
        }
        break;
      case NonPlayerCharacter _:
        (iTarget as NonPlayerCharacter).Confuse(Factions.NONE);
        break;
    }
    for (int index = 0; index < StarGaze.sVictims.Count; ++index)
    {
      if (StarGaze.sVictims[index].Victim == iTarget)
      {
        flag = false;
        victimInfo.Effect = StarGaze.sVictims[index].Effect;
        StarGaze.sVictims[index] = victimInfo;
        break;
      }
    }
    if (flag)
    {
      EffectManager.Instance.StartEffect(StarGaze.EFFECT_STATUS, ref position, ref direction, out victimInfo.Effect);
      StarGaze.sVictims.Add(victimInfo);
    }
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) StarGaze.Instance);
  }

  public bool IsDead => StarGaze.sVictims.Count == 0;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    for (int index = 0; index < StarGaze.sVictims.Count; ++index)
    {
      StarGaze.VictimInfo sVictim = StarGaze.sVictims[index];
      sVictim.TTL -= iDeltaTime;
      if ((double) sVictim.TTL <= 0.0 || sVictim.Victim.Dead)
      {
        EffectManager.Instance.Stop(ref sVictim.Effect);
        if (sVictim.Victim is NonPlayerCharacter)
          (sVictim.Victim as NonPlayerCharacter).Confuse(sVictim.Victim.Template.Faction);
        else if (sVictim.Victim is Avatar && sVictim.Controller != null)
          sVictim.Controller.Invert(false);
        StarGaze.sVictims.RemoveAt(index);
        --index;
      }
      else
      {
        Vector3 position = sVictim.Victim.Position;
        Vector3 direction = sVictim.Victim.Direction;
        EffectManager.Instance.UpdatePositionDirection(ref sVictim.Effect, ref position, ref direction);
        StarGaze.sVictims[index] = sVictim;
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
