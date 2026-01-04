// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.ProppMagick
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.Gamers;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

internal class ProppMagick : SpecialAbility
{
  private static ProppMagick sSingelton;
  private static volatile object sSingeltonLock = new object();
  private static DamageCollection5 sDamage;
  private static readonly int EXPLOSION_EFFECT = "magick_propp_explosion".GetHashCodeCustom();
  private static readonly int SPELL_EFFECT = "magick_propp_spell".GetHashCodeCustom();
  private static readonly int CHARGE_EFFECT = "magick_propp_charge".GetHashCodeCustom();
  private static readonly int SPELL_SOUND = "magick_propps_plasma_loop".GetHashCodeCustom();
  private static readonly int EXPLOSION_SOUND = "magick_propps_plasma_explosion".GetHashCodeCustom();

  public static ProppMagick Instance
  {
    get
    {
      if (ProppMagick.sSingelton == null)
      {
        lock (ProppMagick.sSingeltonLock)
        {
          if (ProppMagick.sSingelton == null)
            ProppMagick.sSingelton = new ProppMagick();
        }
      }
      return ProppMagick.sSingelton;
    }
  }

  static ProppMagick()
  {
    ProppMagick.sDamage = new DamageCollection5();
    ProppMagick.sDamage.AddDamage(new Damage(AttackProperties.Damage, Elements.Arcane, 6000f, 1f));
    ProppMagick.sDamage.AddDamage(new Damage(AttackProperties.Damage, Elements.Fire, 2000f, 1f));
    ProppMagick.sDamage.AddDamage(new Damage(AttackProperties.Knockback, Elements.Earth, 200f, 3f));
  }

  private ProppMagick()
    : base(Magicka.Animations.cast_spell1, "magick_proppmagick".GetHashCodeCustom())
  {
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    NetworkState state = NetworkManager.Instance.State;
    if (state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer)) || state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer))
    {
      base.Execute(iOwner, iPlayState);
      Vector3 translation = iOwner.CastSource.Translation;
      translation.Y += 0.5f;
      MissileEntity missileInstance = iOwner.GetMissileInstance();
      Vector3 iVelocity = new Vector3();
      float radians = (float) (SpecialAbility.RANDOM.NextDouble() - 0.5) * 1.57079637f;
      Vector3 result1 = iOwner.Direction;
      Matrix result2;
      Matrix.CreateRotationY(radians, out result2);
      float d = (float) (10.0 + SpecialAbility.RANDOM.NextDouble() * 18.0);
      iVelocity.Y = 18f;
      Vector3.TransformNormal(ref result1, ref result2, out result1);
      iVelocity.X = result1.X * (float) Math.Sqrt((double) d);
      iVelocity.Z = result1.Z * (float) Math.Sqrt((double) d);
      float num1 = 6.28318548f * (float) SpecialAbility.RANDOM.NextDouble();
      float num2 = (float) Math.Acos(2.0 * SpecialAbility.RANDOM.NextDouble() - 1.0);
      Vector3 result3 = new Vector3((float) Math.Cos((double) num1) * (float) Math.Sin((double) num2), (float) Math.Sin((double) num1) * (float) Math.Sin((double) num2), (float) Math.Cos((double) num2));
      Vector3.Multiply(ref result3, 15f, out result3);
      float num3 = 6.28318548f * (float) SpecialAbility.RANDOM.NextDouble();
      float num4 = (float) Math.Acos(2.0 * SpecialAbility.RANDOM.NextDouble() - 1.0);
      Vector3 iLever = new Vector3((float) Math.Cos((double) num3) * (float) Math.Sin((double) num4), (float) Math.Sin((double) num3) * (float) Math.Sin((double) num4), (float) Math.Cos((double) num4));
      ProppMagick.Spawn(ref missileInstance, iOwner, ref translation, ref iVelocity, ref result3, ref iLever);
      if (NetworkManager.Instance.State != NetworkState.Offline)
        NetworkManager.Instance.Interface.SendMessage<SpawnMissileMessage>(ref new SpawnMissileMessage()
        {
          Type = SpawnMissileMessage.MissileType.ProppMagick,
          Handle = missileInstance.Handle,
          Item = (ushort) 0,
          Owner = iOwner.Handle,
          Position = translation,
          Velocity = iVelocity,
          AngularVelocity = result3,
          Lever = iLever
        });
    }
    return false;
  }

  internal static void Spawn(
    ref MissileEntity iMissile,
    ISpellCaster iOwner,
    ref Vector3 iPosition,
    ref Vector3 iVelocity,
    ref Vector3 iAngularVelocity,
    ref Vector3 iLever)
  {
    ConditionCollection iConditions;
    lock (ProjectileSpell.sCachedConditions)
      iConditions = ProjectileSpell.sCachedConditions.Dequeue();
    iConditions.Clear();
    iConditions[0].Condition.EventConditionType = EventConditionType.Collision;
    iConditions[0].Add(new EventStorage(new PlayEffectEvent(ProppMagick.EXPLOSION_EFFECT, false, true)));
    iConditions[0].Add(new EventStorage(new PlaySoundEvent(Banks.Additional, ProppMagick.EXPLOSION_SOUND, false)));
    iConditions[0].Add(new EventStorage(new RemoveEvent()));
    iConditions[0].Add(new EventStorage(new BlastEvent(4f, ProppMagick.sDamage)));
    iConditions[1].Condition.EventConditionType = EventConditionType.Hit;
    iConditions[1].Add(new EventStorage(new PlayEffectEvent(ProppMagick.EXPLOSION_EFFECT, false, true)));
    iConditions[1].Add(new EventStorage(new PlaySoundEvent(Banks.Additional, ProppMagick.EXPLOSION_SOUND, false)));
    iConditions[1].Add(new EventStorage(new RemoveEvent()));
    iConditions[1].Add(new EventStorage(new BlastEvent(4f, ProppMagick.sDamage)));
    iConditions[2].Condition.EventConditionType = EventConditionType.Timer;
    iConditions[2].Condition.Time = 8f;
    iConditions[2].Add(new EventStorage(new PlayEffectEvent(ProppMagick.EXPLOSION_EFFECT, false, true)));
    iConditions[2].Add(new EventStorage(new PlaySoundEvent(Banks.Additional, ProppMagick.EXPLOSION_SOUND, false)));
    iConditions[2].Add(new EventStorage(new RemoveEvent()));
    iConditions[1].Add(new EventStorage(new BlastEvent(4f, ProppMagick.sDamage)));
    iConditions[3].Condition.EventConditionType = EventConditionType.Default;
    iConditions[3].Condition.Repeat = true;
    iConditions[3].Add(new EventStorage(new PlaySoundEvent(Banks.Additional, ProppMagick.SPELL_SOUND, true)));
    iMissile.Initialize(iOwner as Entity, 0.6f, ref iPosition, ref iVelocity, (Model) null, iConditions, false);
    iMissile.Body.AngularVelocity = iAngularVelocity;
    Vector3 iLeverFromMissile = iLever;
    iMissile.SetProppMagickEffect(ProppMagick.SPELL_EFFECT, ref iLeverFromMissile);
    iMissile.Danger = 30f;
    iOwner.PlayState.EntityManager.AddEntity((Entity) iMissile);
    lock (ProjectileSpell.sCachedConditions)
      ProjectileSpell.sCachedConditions.Enqueue(iConditions);
  }
}
