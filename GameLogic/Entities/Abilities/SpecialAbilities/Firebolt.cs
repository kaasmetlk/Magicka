// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Firebolt
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.Gamers;
using Magicka.Network;
using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class Firebolt : SpecialAbility
{
  private const float VELOCITY_MODIFIER = 50f;
  private const float DAMAGE_RADIUS = 5f;
  public static readonly int EFFECT_TRAIL_HASH = "firebolt_trail".GetHashCode();
  public static readonly int EFFECT_HIT_HASH = "firebolt_hit".GetHashCode();
  public static readonly int SOUND_HASH = "spell_fire_projectile".GetHashCode();
  public static readonly int SOUND_HIT_HASH = "spell_fire_blast".GetHashCode();
  public static readonly int SOUND_FORCE_HASH = "spell_earth_force".GetHashCode();
  private static readonly int ROBE_ROGUE = "wizard_rogue".GetHashCodeCustom();
  private Spell mSpell;

  public Firebolt(Magicka.Animations iAnimation)
    : base(iAnimation, "#specab_tsal_firebolt".GetHashCodeCustom())
  {
    this.mSpell = new Spell();
    this.mSpell.Element = Elements.Earth | Elements.Fire;
    this.mSpell.EarthMagnitude = 2f;
    this.mSpell.FireMagnitude = 5f;
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    NetworkState state = NetworkManager.Instance.State;
    if ((state == NetworkState.Client || iOwner is Avatar && (iOwner as Avatar).Player.Gamer is NetworkGamer) && (state != NetworkState.Client || !(iOwner is Avatar) || (iOwner as Avatar).Player.Gamer is NetworkGamer))
      return false;
    base.Execute(iOwner, iPlayState);
    Vector3 translation = iOwner.CastSource.Translation;
    Vector3 result = iOwner.Direction;
    if (iOwner is Avatar && (iOwner as Avatar).Template.ID == Firebolt.ROBE_ROGUE)
      ++translation.Y;
    Vector3.Multiply(ref result, 50f, out result);
    MissileEntity missileInstance = iOwner.GetMissileInstance();
    ProjectileSpell.SpawnMissile(ref missileInstance, iOwner, 0.15f, ref translation, ref result, ref this.mSpell, 5f, 1);
    if (NetworkManager.Instance.State != NetworkState.Offline)
      NetworkManager.Instance.Interface.SendMessage<SpawnMissileMessage>(ref new SpawnMissileMessage()
      {
        Type = SpawnMissileMessage.MissileType.Spell,
        Handle = missileInstance.Handle,
        Item = (ushort) 0,
        Owner = iOwner.Handle,
        Position = translation,
        Velocity = result,
        Spell = this.mSpell,
        Homing = 0.15f,
        Splash = 5f
      });
    return true;
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState) => false;
}
