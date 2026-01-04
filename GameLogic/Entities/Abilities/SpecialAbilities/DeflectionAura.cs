// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.DeflectionAura
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities.Buffs;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using PolygonHead;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class DeflectionAura : SpecialAbility, IAbilityEffect
{
  public static readonly int EFFECT_HASH = "deflection_aura".GetHashCodeCustom();
  public static readonly int SOUND_HASH = "".GetHashCodeCustom();
  private BoundingSphere mSphere;
  private ISpellCaster mOwner;
  private PlayState mPlayState;

  public DeflectionAura(Magicka.Animations iAnimation)
    : base(iAnimation, "#specab_deflect".GetHashCodeCustom())
  {
    this.mSphere = new BoundingSphere(new Vector3(), 5f);
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    this.mOwner = iOwner;
    this.mSphere.Radius = 5f;
    this.mSphere.Center = iOwner.Position;
    this.mPlayState = iPlayState;
    AuraStorage iAura = new AuraStorage(new AuraDeflect(5f), AuraTarget.Self, AuraType.Deflect, DeflectionAura.EFFECT_HASH, 8f, 5f, VisualCategory.Special, Spell.ARCANECOLOR, (int[]) null, Factions.NONE);
    (iOwner as Magicka.GameLogic.Entities.Character).AddAura(ref iAura, false);
    return true;
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState) => throw new Exception();

  public bool IsDead => true;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
  }

  public void OnRemove()
  {
  }
}
