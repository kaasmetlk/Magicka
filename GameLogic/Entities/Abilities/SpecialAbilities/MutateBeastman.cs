// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.MutateBeastman
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.Audio;
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

public class MutateBeastman : SpecialAbility, IAbilityEffect
{
  private const float RANGE = 6f;
  private static MutateBeastman sSingelton = (MutateBeastman) null;
  private static volatile object sSingeltonLock = new object();
  private static readonly float SPREAD = MathHelper.ToRadians(30f);
  private static readonly float SPREAD_COS = (float) Math.Cos((double) MutateBeastman.SPREAD);
  private static readonly int EFFECT_HIT = "starspawn_owd_hit".GetHashCodeCustom();
  private static readonly int SOUND_CAST = "misc_flash".GetHashCodeCustom();
  private static List<MutateBeastman.Info> sDelayedSpawns = new List<MutateBeastman.Info>(2);
  private static CharacterTemplate sTemplate;

  public static MutateBeastman Instance
  {
    get
    {
      if (MutateBeastman.sSingelton == null)
      {
        lock (MutateBeastman.sSingeltonLock)
        {
          if (MutateBeastman.sSingelton == null)
            MutateBeastman.sSingelton = new MutateBeastman();
        }
      }
      return MutateBeastman.sSingelton;
    }
  }

  private MutateBeastman()
    : base(Magicka.Animations.cast_magick_direct, 0)
  {
  }

  public MutateBeastman(Magicka.Animations iAnimation)
    : base(iAnimation, 0)
  {
    MutateBeastman.Instance.Initialize(PlayState.RecentPlayState);
  }

  public void Initialize(PlayState iState)
  {
    MutateBeastman.sTemplate = iState.Content.Load<CharacterTemplate>("Data/Characters/Skeleton_darksoul_elite_noscore");
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    NetworkState state = NetworkManager.Instance.State;
    if (state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer)) || state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer))
    {
      Vector3 position1 = iOwner.Position;
      Vector3 direction = iOwner.Direction;
      IDamageable iTarget = (IDamageable) null;
      if (iOwner is NonPlayerCharacter && (iOwner as NonPlayerCharacter).AI.CurrentTarget != null)
      {
        iTarget = (iOwner as NonPlayerCharacter).AI.CurrentTarget;
        Vector3 position2 = iTarget.Position;
        float result1;
        Vector3.DistanceSquared(ref position1, ref position2, out result1);
        if ((double) result1 > 36.0)
          iTarget = (IDamageable) null;
        else if ((double) result1 > 9.9999999747524271E-07)
        {
          Vector3 result2;
          Vector3.Subtract(ref position2, ref position1, out result2);
          Vector3.Divide(ref result2, (float) Math.Sqrt((double) result1), out result2);
          float result3;
          Vector3.Dot(ref result2, ref direction, out result3);
          if ((double) result3 < (double) MutateBeastman.SPREAD_COS)
          {
            iTarget = (IDamageable) null;
          }
          else
          {
            Segment iSeg;
            iSeg.Origin = position1;
            iSeg.Delta = iTarget.Position;
            Vector3.Subtract(ref iSeg.Delta, ref position1, out iSeg.Delta);
            foreach (Shield shield in iPlayState.EntityManager.Shields)
            {
              if (shield.SegmentIntersect(out Vector3 _, iSeg, 0.5f))
              {
                iTarget = (IDamageable) null;
                break;
              }
            }
          }
        }
      }
      if (iTarget == null)
        iTarget = this.GetTarget(iPlayState.EntityManager, iOwner, position1, direction);
      if (iTarget != null && iTarget is Magicka.GameLogic.Entities.Character character && character.IsSelfShielded && !character.IsSolidSelfShielded)
      {
        character.RemoveSelfShield();
        iTarget = (IDamageable) null;
      }
      if (iTarget != null)
      {
        if (state != NetworkState.Offline)
          NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
          {
            ActionType = TriggerActionType.OtherworldlyDischarge,
            Handle = iOwner.Handle,
            Arg = (int) iTarget.Handle
          });
        this.Execute(iTarget, iOwner, iPlayState);
      }
    }
    AudioManager.Instance.PlayCue(Banks.Misc, MutateBeastman.SOUND_CAST, iOwner.AudioEmitter);
    return true;
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    throw new NotImplementedException();
  }

  private IDamageable GetTarget(
    EntityManager iEntityMan,
    ISpellCaster iOwner,
    Vector3 iPos,
    Vector3 iDir)
  {
    IDamageable target = (IDamageable) null;
    float num = float.MaxValue;
    List<Entity> entities = iEntityMan.GetEntities(iPos, 6f, true);
    foreach (Entity entity in entities)
    {
      Vector3 oPosition;
      if (entity is IDamageable damageable && damageable != iOwner && !(damageable is MissileEntity) && damageable.ArcIntersect(out oPosition, iPos, iDir, 6f, MutateBeastman.SPREAD, 4f))
      {
        float result = -1f;
        if (damageable is Magicka.GameLogic.Entities.Character)
        {
          Magicka.GameLogic.Entities.Character character = damageable as Magicka.GameLogic.Entities.Character;
          if (character.IsSelfShielded && !character.IsSolidSelfShielded)
            result = 6f;
        }
        if ((double) result < 0.0)
          Vector3.DistanceSquared(ref iPos, ref oPosition, out result);
        if ((double) result < (double) num)
        {
          num = result;
          target = damageable;
        }
      }
    }
    iEntityMan.ReturnEntityList(entities);
    return target;
  }

  public void Execute(IDamageable iTarget, ISpellCaster iOwner, PlayState iPlayState)
  {
    Magicka.GameLogic.Entities.Character character = iTarget as Magicka.GameLogic.Entities.Character;
    iTarget.OverKill();
    if (character == null)
      return;
    character.BloatKill(Elements.None, iOwner as Entity);
    MutateBeastman.sDelayedSpawns.Add(new MutateBeastman.Info()
    {
      TTL = 0.333f,
      Victim = character
    });
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) MutateBeastman.Instance);
  }

  public bool IsDead => MutateBeastman.sDelayedSpawns.Count == 0;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    for (int index = 0; index < MutateBeastman.sDelayedSpawns.Count; ++index)
    {
      MutateBeastman.Info sDelayedSpawn = MutateBeastman.sDelayedSpawns[index];
      sDelayedSpawn.TTL -= iDeltaTime;
      if ((double) sDelayedSpawn.TTL <= 0.0)
      {
        Vector3 position = sDelayedSpawn.Victim.Position;
        if (NetworkManager.Instance.State != NetworkState.Client)
        {
          NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(sDelayedSpawn.Victim.PlayState);
          instance.Initialize(MutateBeastman.sTemplate, position, 0);
          Vector3 direction = sDelayedSpawn.Victim.Direction;
          Vector3 result;
          Vector3.Cross(ref direction, ref new Vector3()
          {
            Y = 1f
          }, out result);
          instance.CharacterBody.Orientation = new Matrix()
          {
            M44 = 1f,
            M22 = 1f,
            Forward = direction,
            Right = result
          };
          instance.CharacterBody.DesiredDirection = direction;
          sDelayedSpawn.Victim.PlayState.EntityManager.AddEntity((Entity) instance);
          if (NetworkManager.Instance.State == NetworkState.Server)
            NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
            {
              ActionType = TriggerActionType.SpawnNPC,
              Handle = instance.Handle,
              Template = MutateBeastman.sTemplate.ID,
              Id = 0,
              Position = position,
              Direction = direction,
              Bool0 = false
            });
        }
        Vector3 forward = Vector3.Forward;
        EffectManager.Instance.StartEffect(MutateBeastman.EFFECT_HIT, ref position, ref forward, out VisualEffectReference _);
        MutateBeastman.sDelayedSpawns.RemoveAt(index);
        --index;
      }
      else
        MutateBeastman.sDelayedSpawns[index] = sDelayedSpawn;
    }
  }

  public void OnRemove()
  {
  }

  private struct Info
  {
    public float TTL;
    public Magicka.GameLogic.Entities.Character Victim;
  }
}
