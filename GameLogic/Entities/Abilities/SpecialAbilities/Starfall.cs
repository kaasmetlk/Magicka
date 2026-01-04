// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Starfall
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PolygonHead;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class Starfall(Magicka.Animations iAnimation) : SpecialAbility(iAnimation, 0), IAbilityEffect
{
  private const float CAST_DELAY = 1f;
  private const float DAMAGE_DELAY = 0.5f;
  private static Starfall sSingelton;
  private static volatile object sSingeltonLock = new object();
  public static readonly int TEMPBLASTSOUND = "magick_meteor_blast".GetHashCodeCustom();
  public static readonly int SOUND = "magick_meteor_preblast".GetHashCodeCustom();
  public static readonly int EFFECT = "starspawn_starfall".GetHashCodeCustom();
  public static readonly float RANGE = 3f;
  private static DamageCollection5 sDamage;
  private static AudioEmitter sAudioEmitter;
  private static PlayState sPlayState;
  private static List<Starfall.Info> sQueue = new List<Starfall.Info>(1);

  public static Starfall Instance
  {
    get
    {
      if (Starfall.sSingelton == null)
      {
        lock (Starfall.sSingeltonLock)
        {
          if (Starfall.sSingelton == null)
            Starfall.sSingelton = new Starfall(Magicka.Animations.cast_magick_direct);
        }
      }
      return Starfall.sSingelton;
    }
  }

  static Starfall()
  {
    Starfall.sDamage.A = new Damage(AttackProperties.Damage, Elements.Arcane, 12000f, 1f);
    Starfall.sDamage.B = new Damage(AttackProperties.Damage | AttackProperties.ArmourPiercing, Elements.Earth, 6000f, 1f);
    Starfall.sAudioEmitter = new AudioEmitter();
    Starfall.sAudioEmitter.Forward = Vector3.Forward;
    Starfall.sAudioEmitter.Up = Vector3.Up;
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    NetworkState state = NetworkManager.Instance.State;
    if (state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer)) || state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer))
    {
      Vector3 iPosition;
      if (iOwner is NonPlayerCharacter && (iOwner as NonPlayerCharacter).AI.CurrentTarget != null)
      {
        iPosition = (iOwner as NonPlayerCharacter).AI.CurrentTarget.Position;
      }
      else
      {
        Vector3 result1 = iOwner.Position;
        Vector3 result2 = iOwner.Direction;
        Vector3.Multiply(ref result2, 10f, out result2);
        Vector3.Add(ref result2, ref result1, out result1);
        iPosition = this.GetTarget(result1, iPlayState);
      }
      if (state != NetworkState.Offline)
        NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
        {
          ActionType = TriggerActionType.Starfall,
          Position = iPosition,
          Handle = iOwner.Handle
        });
      this.Execute(iOwner, iPlayState, iPosition, true);
    }
    return true;
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    base.Execute(iPosition, iPlayState);
    NetworkState state = NetworkManager.Instance.State;
    if (state != NetworkState.Client)
    {
      iPosition = this.GetTarget(iPosition, iPlayState);
      if (state != NetworkState.Offline)
        NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
        {
          ActionType = TriggerActionType.Starfall,
          Position = iPosition,
          Handle = ushort.MaxValue
        });
      this.Execute((ISpellCaster) null, iPlayState, iPosition, true);
    }
    return true;
  }

  private Vector3 GetTarget(Vector3 iSource, PlayState iPlayState)
  {
    Vector3 target = iSource;
    float num1 = float.MaxValue;
    List<Entity> entities = iPlayState.EntityManager.GetEntities(iSource, 8f, false);
    foreach (Entity entity in entities)
    {
      if (entity is IDamageable)
      {
        Vector3 position = entity.Position;
        Vector3 result;
        Vector3.Subtract(ref position, ref iSource, out result);
        result.Y = 0.0f;
        float num2 = result.LengthSquared();
        if ((double) num2 < (double) num1)
        {
          num1 = num2;
          target = position;
        }
      }
    }
    iPlayState.EntityManager.ReturnEntityList(entities);
    return target;
  }

  public bool Execute(
    ISpellCaster iOwner,
    PlayState iPlayState,
    Vector3 iPosition,
    bool iDeadDamage)
  {
    Starfall.sPlayState = iPlayState;
    if (iDeadDamage)
    {
      Starfall.sQueue.Add(new Starfall.Info()
      {
        CastDelay = 1f,
        DamageDelay = 0.5f,
        DealDamage = iDeadDamage,
        Position = iPosition,
        Owner = iOwner as Entity
      });
      SpellManager.Instance.AddSpellEffect((IAbilityEffect) Starfall.Instance);
    }
    return true;
  }

  public bool IsDead => Starfall.sQueue.Count == 0;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    for (int index = 0; index < Starfall.sQueue.Count; ++index)
    {
      Starfall.Info s = Starfall.sQueue[index];
      if ((double) s.CastDelay > 0.0)
      {
        s.CastDelay -= iDeltaTime;
        if ((double) s.CastDelay <= 0.0)
        {
          Segment iSeg;
          iSeg.Delta = new Vector3();
          iSeg.Delta.X = -20f;
          iSeg.Delta.Y = -20f;
          Vector3.Subtract(ref s.Position, ref iSeg.Delta, out iSeg.Origin);
          float oFrac;
          Vector3 vector3;
          if (Starfall.sPlayState.Level.CurrentScene.LevelModel.SegmentIntersect(out oFrac, out vector3, out Vector3 _, out AnimatedLevelPart _, out int _, iSeg, true))
          {
            s.Position = vector3;
            Vector3.Multiply(ref iSeg.Delta, oFrac, out iSeg.Delta);
          }
          foreach (Shield shield in Starfall.sPlayState.EntityManager.Shields)
          {
            if (shield.SegmentIntersect(out oFrac, out vector3, iSeg, 0.5f))
            {
              s.Position = vector3;
              Vector3.Multiply(ref iSeg.Delta, oFrac, out iSeg.Delta);
            }
          }
          EffectManager.Instance.StartEffect(Starfall.EFFECT, ref s.Position, ref new Vector3()
          {
            X = -1f
          }, out VisualEffectReference _);
          Starfall.sAudioEmitter.Position = s.Position;
          AudioManager.Instance.PlayCue(Banks.Spells, Starfall.SOUND, Starfall.sAudioEmitter);
        }
        Starfall.sQueue[index] = s;
      }
      else if (s.DealDamage)
      {
        s.DamageDelay -= iDeltaTime;
        if ((double) s.DamageDelay <= 0.0)
        {
          int num = (int) Helper.CircleDamage(Starfall.sPlayState, s.Owner, Starfall.sPlayState.PlayTime, (Entity) null, ref s.Position, Starfall.RANGE, ref Starfall.sDamage);
          AudioManager.Instance.PlayCue(Banks.Spells, Starfall.TEMPBLASTSOUND);
          Starfall.sQueue.RemoveAt(index);
          --index;
        }
        else
          Starfall.sQueue[index] = s;
      }
      else
      {
        Starfall.sQueue.RemoveAt(index);
        --index;
      }
    }
  }

  public void OnRemove()
  {
  }

  private struct Info
  {
    public float CastDelay;
    public float DamageDelay;
    public bool DealDamage;
    public Vector3 Position;
    public Entity Owner;
  }
}
