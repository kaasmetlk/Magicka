// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.JudgementSpray
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.Gamers;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class JudgementSpray : SpecialAbility, IAbilityEffect
{
  private static volatile JudgementSpray sSingelton;
  private static volatile object sSingeltonLock = new object();
  private static List<MissileEntity> mMissiles = new List<MissileEntity>(128 /*0x80*/);
  private static Model sModel;
  private readonly float OPTIMAL_DISTANCE = 12f;
  private readonly float RANGE = 20f;
  private readonly float SPEED = 30f;
  private readonly float TTL = 1f;
  private static readonly float PROJECTILE_TTL = 2f;
  private readonly int NUM_OF_PROJECTILES = 20;
  private static float HOMING_EFFICIENCY = 0.55f;
  private static readonly float COLD_DAMAGE = 273f;
  private static readonly float STATUS_DAMAGE = 100f;
  private ISpellCaster mOwner;
  private float mTTL;
  private float mInterval;
  private bool mLeft;
  private static readonly int EFFECT = "magick_judgementspray_hit".GetHashCodeCustom();
  private static readonly int SOUND_HIT = "wep_handgrenade_explode".GetHashCodeCustom();
  private static readonly int SOUND_FIRE = "woot_magick_judgment_fire".GetHashCodeCustom();
  private static readonly int PULSE = "magick_judgementspray_trail".GetHashCodeCustom();
  private static readonly int MISSILE = "magick_judgementspray_missile".GetHashCodeCustom();

  public static JudgementSpray Instance
  {
    get
    {
      if (JudgementSpray.sSingelton == null)
      {
        lock (JudgementSpray.sSingeltonLock)
        {
          if (JudgementSpray.sSingelton == null)
            JudgementSpray.sSingelton = new JudgementSpray();
        }
      }
      return JudgementSpray.sSingelton;
    }
  }

  public JudgementSpray()
    : base(Magicka.Animations.cast_magick_global, "#specab_tsal_antioch".GetHashCodeCustom())
  {
    if (JudgementSpray.sModel != null)
      return;
    lock (Magicka.Game.Instance.GraphicsDevice)
      JudgementSpray.sModel = Magicka.Game.Instance.Content.Load<Model>("Models/Missiles/JudgementSprayMissile");
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    NetworkState state = NetworkManager.Instance.State;
    if ((state == NetworkState.Client || iOwner is Avatar && (iOwner as Avatar).Player.Gamer is NetworkGamer) && (state != NetworkState.Client || !(iOwner is Avatar) || (iOwner as Avatar).Player.Gamer is NetworkGamer))
      return false;
    base.Execute(iOwner, iPlayState);
    this.mOwner = iOwner;
    this.mTTL = this.TTL;
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    AudioManager.Instance.PlayCue(Banks.Additional, JudgementSpray.SOUND_FIRE, iOwner.AudioEmitter);
    return true;
  }

  internal static void SpawnProjectile(
    ref MissileEntity m,
    ISpellCaster iOwner,
    ref Entity iTarget,
    ref Vector3 iPosition,
    ref Vector3 iVelocity)
  {
    ConditionCollection iConditions;
    lock (ProjectileSpell.sCachedConditions)
      iConditions = ProjectileSpell.sCachedConditions.Dequeue();
    iConditions.Clear();
    Damage iDamage1 = new Damage(AttackProperties.Damage, Elements.Cold, JudgementSpray.COLD_DAMAGE, 1f);
    Damage iDamage2 = new Damage(AttackProperties.Status, Elements.Cold, JudgementSpray.STATUS_DAMAGE, 1f);
    iConditions[0].Condition.EventConditionType = EventConditionType.Damaged;
    iConditions[0].Condition.Elements = Elements.Fire | Elements.Lightning | Elements.Arcane;
    iConditions[0].Condition.Hitpoints = 20f;
    iConditions[0].Add(new EventStorage(new PlayEffectEvent(JudgementSpray.EFFECT, false, true)));
    iConditions[0].Add(new EventStorage(new PlaySoundEvent(Banks.Additional, JudgementSpray.SOUND_HIT, false)));
    iConditions[0].Add(new EventStorage(new DamageEvent(iDamage1)));
    iConditions[0].Add(new EventStorage(new DamageEvent(iDamage2)));
    iConditions[0].Add(new EventStorage(new RemoveEvent()));
    iConditions[1].Condition.EventConditionType = EventConditionType.Hit;
    iConditions[1].Add(new EventStorage(new PlayEffectEvent(JudgementSpray.EFFECT, false, true)));
    iConditions[1].Add(new EventStorage(new PlaySoundEvent(Banks.Additional, JudgementSpray.SOUND_HIT, false)));
    iConditions[1].Add(new EventStorage(new DamageEvent(iDamage1)));
    iConditions[1].Add(new EventStorage(new DamageEvent(iDamage2)));
    iConditions[1].Add(new EventStorage(new RemoveEvent()));
    iConditions[3].Condition.EventConditionType = EventConditionType.Collision;
    iConditions[3].Condition.Threshold = 0.0f;
    iConditions[3].Add(new EventStorage(new PlayEffectEvent(JudgementSpray.EFFECT, false)));
    iConditions[3].Add(new EventStorage(new PlaySoundEvent(Banks.Additional, JudgementSpray.SOUND_HIT, false)));
    iConditions[3].Add(new EventStorage(new DamageEvent(iDamage1)));
    iConditions[3].Add(new EventStorage(new DamageEvent(iDamage2)));
    iConditions[3].Add(new EventStorage(new RemoveEvent()));
    iConditions[4].Condition.EventConditionType = EventConditionType.Timer;
    iConditions[4].Condition.Time = JudgementSpray.PROJECTILE_TTL;
    iConditions[4].Add(new EventStorage(new PlayEffectEvent(JudgementSpray.EFFECT, false, true)));
    iConditions[4].Add(new EventStorage(new PlaySoundEvent(Banks.Additional, JudgementSpray.SOUND_HIT, false)));
    iConditions[4].Add(new EventStorage(new DamageEvent(iDamage1)));
    iConditions[4].Add(new EventStorage(new DamageEvent(iDamage2)));
    iConditions[4].Add(new EventStorage(new RemoveEvent()));
    iConditions[2].Condition.EventConditionType = EventConditionType.Default;
    iConditions[2].Condition.Repeat = true;
    iConditions[2].Add(new EventStorage(new PlayEffectEvent(JudgementSpray.PULSE, true)));
    iConditions[2].Add(new EventStorage(new PlayEffectEvent(JudgementSpray.MISSILE, true)));
    m.Initialize(iOwner as Entity, iTarget, JudgementSpray.HOMING_EFFICIENCY, JudgementSpray.sModel.Meshes[0].BoundingSphere.Radius * 0.75f, ref iPosition, ref iVelocity, JudgementSpray.sModel, iConditions, false);
    m.Body.AngularVelocity = new Vector3(0.0f, 0.0f, 2f * m.Body.Mass);
    m.Danger = 30f;
    iOwner.PlayState.EntityManager.AddEntity((Entity) m);
    JudgementSpray.mMissiles.Add(m);
    lock (ProjectileSpell.sCachedConditions)
      ProjectileSpell.sCachedConditions.Enqueue(iConditions);
    m.FacingVelocity = false;
    NetworkState state = NetworkManager.Instance.State;
    switch (state)
    {
      case NetworkState.Offline:
        return;
      case NetworkState.Client:
        if (state != NetworkState.Client || !(iOwner is Avatar) || (iOwner as Avatar).Player.Gamer is NetworkGamer)
          return;
        break;
      default:
        if (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer))
          break;
        goto case NetworkState.Client;
    }
    SpawnMissileMessage iMessage;
    if (iTarget != null)
      iMessage = new SpawnMissileMessage()
      {
        Type = SpawnMissileMessage.MissileType.JudgementMissile,
        Handle = m.Handle,
        Item = (ushort) 0,
        Owner = iOwner.Handle,
        Target = iTarget.Handle,
        Position = iPosition,
        Velocity = iVelocity
      };
    else
      iMessage = new SpawnMissileMessage()
      {
        Type = SpawnMissileMessage.MissileType.JudgementMissile,
        Handle = m.Handle,
        Item = (ushort) 0,
        Owner = iOwner.Handle,
        Target = (ushort) 0,
        Position = iPosition,
        Velocity = iVelocity
      };
    NetworkManager.Instance.Interface.SendMessage<SpawnMissileMessage>(ref iMessage);
  }

  public bool IsDead => (double) this.mTTL <= 0.0;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if (this.mOwner == null)
      return;
    this.mTTL -= iDeltaTime;
    this.mInterval -= iDeltaTime;
    if ((double) this.mInterval >= 0.0)
      return;
    this.mInterval = this.TTL / (float) this.NUM_OF_PROJECTILES;
    List<Entity> entities = this.mOwner.PlayState.EntityManager.GetEntities(this.mOwner.Position, this.RANGE, false);
    Entity iTarget = (Entity) null;
    float num1 = float.MaxValue;
    Vector3 position1 = this.mOwner.Position;
    foreach (Entity entity in entities)
    {
      if (entity is Magicka.GameLogic.Entities.Character && entity != this.mOwner)
      {
        Vector3 position2 = entity.Position;
        Vector3 vector = this.mOwner.Position - position2;
        vector.Normalize();
        Vector3 direction = this.mOwner.Direction;
        float num2 = MagickaMath.Angle(ref vector, ref direction);
        if ((double) num2 < 3.9269909858703613 && (double) num2 > 1.5707963705062866)
        {
          float result;
          Vector3.Distance(ref position2, ref position1, out result);
          float num3 = this.OPTIMAL_DISTANCE - result;
          if ((double) Math.Abs(num3) < (double) num1)
          {
            iTarget = entity;
            num1 = num3;
          }
        }
      }
    }
    Vector3 translation = this.mOwner.CastSource.Translation;
    if (this.mOwner is Magicka.GameLogic.Entities.Character)
      translation = (this.mOwner as Magicka.GameLogic.Entities.Character).GetLeftAttachOrientation().Translation;
    Vector3 direction1 = this.mOwner.Direction;
    Vector3 vector1_1 = new Vector3(0.0f, 1f, 0.0f);
    Vector3 vector1_2 = new Vector3(0.0f, -1f, 0.0f);
    this.mLeft = !this.mLeft;
    direction1.Y = 1f;
    direction1.Normalize();
    Vector3 result1;
    if (this.mLeft)
      Vector3.Cross(ref vector1_1, ref direction1, out result1);
    else
      Vector3.Cross(ref vector1_2, ref direction1, out result1);
    float scaleFactor = (float) ((double) this.SPEED / 2.0 + (0.5 + MagickaMath.Random.NextDouble()) * (double) this.SPEED / 2.0);
    Vector3.Multiply(ref result1, scaleFactor, out result1);
    MissileEntity missileInstance = this.mOwner.GetMissileInstance();
    JudgementSpray.SpawnProjectile(ref missileInstance, this.mOwner, ref iTarget, ref translation, ref result1);
    this.mOwner.PlayState.EntityManager.ReturnEntityList(entities);
  }

  public void OnRemove()
  {
  }
}
