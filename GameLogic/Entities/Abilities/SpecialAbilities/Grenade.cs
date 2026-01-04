// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Grenade
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

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class Grenade : SpecialAbility
{
  private static Model sModel;
  private static readonly int EFFECT = "liberty_explosion".GetHashCodeCustom();
  private static readonly int SOUND = "wep_handgrenade_explode".GetHashCodeCustom();
  private static readonly int SOUND2 = "goblin_bomb_explosion".GetHashCodeCustom();
  private static readonly int PULSE = "vietnam_grenade_trail".GetHashCodeCustom();

  public Grenade(Magicka.Animations iAnimation)
    : base(iAnimation, "#specab_grenade".GetHashCodeCustom())
  {
    if (Grenade.sModel != null)
      return;
    lock (Magicka.Game.Instance.GraphicsDevice)
      Grenade.sModel = Magicka.Game.Instance.Content.Load<Model>("Models/Missiles/liberty_grenade");
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    NetworkState state = NetworkManager.Instance.State;
    if ((state == NetworkState.Client || iOwner is Avatar && (iOwner as Avatar).Player.Gamer is NetworkGamer) && (state != NetworkState.Client || !(iOwner is Avatar) || (iOwner as Avatar).Player.Gamer is NetworkGamer))
      return false;
    base.Execute(iOwner, iPlayState);
    Vector3 translation = iOwner.CastSource.Translation;
    if (iOwner is Character)
      translation = (iOwner as Character).GetLeftAttachOrientation().Translation;
    Vector3 result = iOwner.Direction with { Y = 1f };
    result.Normalize();
    Vector3.Multiply(ref result, 13f, out result);
    MissileEntity missileInstance = iOwner.GetMissileInstance();
    Grenade.SpawnGrenade(ref missileInstance, iOwner, ref translation, ref result);
    missileInstance.FacingVelocity = false;
    if (NetworkManager.Instance.State != NetworkState.Offline)
      NetworkManager.Instance.Interface.SendMessage<SpawnMissileMessage>(ref new SpawnMissileMessage()
      {
        Type = SpawnMissileMessage.MissileType.Grenade,
        Handle = missileInstance.Handle,
        Item = (ushort) 0,
        Owner = iOwner.Handle,
        Position = translation,
        Velocity = result
      });
    return true;
  }

  internal static void SpawnGrenade(
    ref MissileEntity iMissile,
    ISpellCaster iOwner,
    ref Vector3 iPosition,
    ref Vector3 iVelocity)
  {
    ConditionCollection iConditions;
    lock (ProjectileSpell.sCachedConditions)
      iConditions = ProjectileSpell.sCachedConditions.Dequeue();
    iConditions.Clear();
    Damage iDamage1 = new Damage(AttackProperties.Damage, Elements.Earth | Elements.Fire, 800f, 2f);
    Damage iDamage2 = new Damage(AttackProperties.Knockback, Elements.Earth, 750f, 2f);
    iConditions[0].Condition.EventConditionType = EventConditionType.Damaged;
    iConditions[0].Condition.Elements = Elements.Fire | Elements.Lightning | Elements.Arcane;
    iConditions[0].Condition.Hitpoints = 20f;
    iConditions[0].Add(new EventStorage(new PlayEffectEvent(Grenade.EFFECT, false, true)));
    iConditions[0].Add(new EventStorage(new PlaySoundEvent(Banks.Additional, Grenade.SOUND, false)));
    iConditions[0].Add(new EventStorage(new BlastEvent(6f, iDamage2)));
    iConditions[0].Add(new EventStorage(new BlastEvent(6f, iDamage1)));
    iConditions[0].Add(new EventStorage(new RemoveEvent()));
    iConditions[1].Condition.EventConditionType = EventConditionType.Timer;
    iConditions[1].Condition.Time = 3f;
    iConditions[1].Add(new EventStorage(new PlayEffectEvent(Grenade.EFFECT, false, true)));
    iConditions[1].Add(new EventStorage(new PlaySoundEvent(Banks.Additional, Grenade.SOUND, false)));
    iConditions[1].Add(new EventStorage(new BlastEvent(6f, iDamage2)));
    iConditions[1].Add(new EventStorage(new BlastEvent(6f, iDamage1)));
    iConditions[1].Add(new EventStorage(new RemoveEvent()));
    iConditions[2].Condition.EventConditionType = EventConditionType.Default;
    iConditions[2].Condition.Repeat = true;
    iConditions[2].Add(new EventStorage(new PlayEffectEvent(Grenade.PULSE, true)));
    iMissile.Initialize(iOwner as Entity, Grenade.sModel.Meshes[0].BoundingSphere.Radius * 0.75f, ref iPosition, ref iVelocity, Grenade.sModel, iConditions, false);
    iMissile.Body.AngularVelocity = new Vector3(0.0f, 0.0f, 2f * iMissile.Body.Mass);
    iMissile.Danger = 30f;
    iOwner.PlayState.EntityManager.AddEntity((Entity) iMissile);
    lock (ProjectileSpell.sCachedConditions)
      ProjectileSpell.sCachedConditions.Enqueue(iConditions);
  }
}
