// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Potion
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells.SpellEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

internal class Potion : SpecialAbility
{
  private static readonly int DISPLAY_NAME = "#specab_drink".GetHashCodeCustom();
  private readonly Damage mHealing;
  private readonly float HITPOINTS = 501f;
  private static readonly float FLASK_TTL = 15f;
  private static Model sModel;

  public Potion(Magicka.Animations iAnimation)
    : base(Magicka.Animations.special0, Potion.DISPLAY_NAME)
  {
    this.mHealing = new Damage(AttackProperties.Damage, Elements.Life, -this.HITPOINTS, 1f);
    if (Potion.sModel != null)
      return;
    lock (Magicka.Game.Instance.GraphicsDevice)
      Potion.sModel = Magicka.Game.Instance.Content.Load<Model>("Models/Missiles/flask_healing_potion");
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    throw new Exception("Potion have to be cast by a character!");
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    int num = (int) iOwner.Damage(this.mHealing, iOwner as Entity, this.mTimeStamp, iOwner.CastSource.Translation);
    return true;
  }

  internal static void SpawnFlask(
    ref MissileEntity iMissile,
    ISpellCaster iOwner,
    ref Vector3 iPosition,
    ref Vector3 iVelocity)
  {
    ConditionCollection iConditions;
    lock (ProjectileSpell.sCachedConditions)
      iConditions = ProjectileSpell.sCachedConditions.Dequeue();
    iConditions.Clear();
    iConditions[0].Condition.EventConditionType = EventConditionType.Damaged;
    iConditions[0].Condition.Elements = Elements.None;
    iConditions[0].Condition.Hitpoints = 20f;
    iConditions[0].Add(new EventStorage(new RemoveEvent()));
    iConditions[1].Condition.EventConditionType = EventConditionType.Timer;
    iConditions[1].Condition.Time = Potion.FLASK_TTL;
    iConditions[1].Add(new EventStorage(new RemoveEvent()));
    iConditions[2].Condition.EventConditionType = EventConditionType.Default;
    iConditions[2].Condition.Repeat = true;
    iMissile.Initialize(iOwner as Entity, Potion.sModel.Meshes[0].BoundingSphere.Radius * 0.75f, ref iPosition, ref iVelocity, Potion.sModel, iConditions, false);
    iMissile.Body.AngularVelocity = new Vector3(0.0f, 0.0f, 2f * iMissile.Body.Mass);
    iMissile.Danger = 0.0f;
    iOwner.PlayState.EntityManager.AddEntity((Entity) iMissile);
    lock (ProjectileSpell.sCachedConditions)
      ProjectileSpell.sCachedConditions.Enqueue(iConditions);
  }
}
