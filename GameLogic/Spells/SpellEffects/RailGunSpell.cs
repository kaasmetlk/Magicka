// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Spells.SpellEffects.RailGunSpell
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Entities;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Spells.SpellEffects;

public class RailGunSpell : SpellEffect
{
  private static List<RailGunSpell> mCache;
  private ISpellCaster mCaster;
  private static int WeaponEffectHash = "weapon_arcane4".GetHashCodeCustom();
  private static int LifeAreaSoundEffect = "spell_life_area_ray_stage1".GetHashCodeCustom();
  private static int ArcaneAreaSoundEffect = "spell_arcane_area_ray_stage1".GetHashCodeCustom();
  private DamageCollection5 mDamage;
  private bool mHasCast;
  private Railgun mRailGun;
  private ArcaneBlade mBlade;
  private float mTTL;
  private VisualEffectReference[] mSelfCastEffectReference = new VisualEffectReference[11];
  private new double mTimeStamp;

  public static void InitializeCache(int iSize)
  {
    RailGunSpell.mCache = new List<RailGunSpell>(iSize);
    for (int index = 0; index < iSize; ++index)
      RailGunSpell.mCache.Add(new RailGunSpell());
  }

  public static SpellEffect GetFromCache()
  {
    if (RailGunSpell.mCache.Count <= 0)
      return (SpellEffect) null;
    RailGunSpell fromCache = RailGunSpell.mCache[RailGunSpell.mCache.Count - 1];
    RailGunSpell.mCache.Remove(fromCache);
    SpellEffect.mPlayState.SpellEffects.Add((SpellEffect) fromCache);
    return (SpellEffect) fromCache;
  }

  public static void ReturnToCache(RailGunSpell iEffect)
  {
    SpellEffect.mPlayState.SpellEffects.Remove((SpellEffect) iEffect);
    RailGunSpell.mCache.Add(iEffect);
  }

  public override void CastArea(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
  {
    base.CastArea(iSpell, iOwner, iFromStaff);
    this.mTimeStamp = iOwner.PlayState.PlayTime;
    this.mTTL = 0.0f;
    DamageCollection5 oDamages;
    iSpell.CalculateDamage(SpellType.Beam, CastType.Area, out oDamages);
    SpellSoundVariables iVariables = new SpellSoundVariables();
    if ((double) iSpell[Elements.Arcane] > 0.0)
    {
      iVariables.mMagnitude = iSpell[Elements.Arcane];
      AudioManager.Instance.PlayCue<SpellSoundVariables>(Banks.Spells, "spell_arcane_area".GetHashCodeCustom(), iVariables);
    }
    else
    {
      iVariables.mMagnitude = iSpell[Elements.Life];
      AudioManager.Instance.PlayCue<SpellSoundVariables>(Banks.Spells, "spell_life_area".GetHashCodeCustom(), iVariables);
    }
    int num = (int) Blast.FullBlast(iOwner.PlayState, iOwner as Entity, this.mTimeStamp, iOwner as Entity, iSpell.BlastSize() * 10f, iOwner.Position, oDamages);
  }

  public override void CastForce(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
  {
    base.CastForce(iSpell, iOwner, iFromStaff);
    this.mTimeStamp = iOwner.PlayState.PlayTime;
    this.mTTL = (float) (1.0 + 2.0 * ((double) iSpell.ArcaneMagnitude + (double) iSpell.LifeMagnitude));
    if (iOwner is Character)
      (iOwner as Character).GetSpellTTLModifier(ref this.mTTL);
    this.mCaster = iOwner;
    this.mSpell = iSpell;
    this.mFromStaff = iFromStaff;
    DamageCollection5 oDamages;
    this.mSpell.CalculateDamage(SpellType.Beam, CastType.Force, out oDamages);
    this.mRailGun = Railgun.GetFromCache();
    this.mRailGun.Initialize(iOwner, iOwner.Position, iOwner.Direction, iSpell.GetColor(), ref oDamages, ref iSpell);
  }

  public override void CastSelf(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
  {
    base.CastSelf(iSpell, iOwner, iFromStaff);
    this.mTimeStamp = iOwner.PlayState.PlayTime;
    this.mTTL = 1f;
    this.mCaster = iOwner;
    this.mSpell = iSpell;
    this.mSpell.CalculateDamage(SpellType.Beam, CastType.Self, out this.mDamage);
    SpellEffect.mPlayState.SpellEffects.Add((SpellEffect) this);
    Vector3 position = iOwner.Position;
    Vector3 direction = iOwner.Direction;
    for (int iIndex = 0; iIndex < 11; ++iIndex)
    {
      Elements iElement = Defines.ElementFromIndex(iIndex);
      if ((iElement & iSpell.Element) == iElement)
        EffectManager.Instance.StartEffect(SpellEffect.SelfCastEffectHash[Defines.ElementIndex(iElement)], ref position, ref direction, out this.mSelfCastEffectReference[iIndex]);
    }
    foreach (Cue cue in this.mSpell.PlaySound(SpellType.Beam, CastType.Self))
    {
      if (cue != null)
      {
        cue.Apply3D(SpellEffect.mPlayState.Camera.Listener, iOwner.AudioEmitter);
        cue.Play();
      }
    }
  }

  public override void CastWeapon(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
  {
    base.CastWeapon(iSpell, iOwner, iFromStaff);
    this.mTimeStamp = iOwner.PlayState.PlayTime;
    iSpell.CalculateDamage(SpellType.Beam, this.mCastType, out this.mDamage);
    this.mSpell = iSpell;
    foreach (Cue cue in this.mSpell.PlaySound(SpellType.Beam, CastType.Weapon))
    {
      cue.Apply3D(SpellEffect.mPlayState.Camera.Listener, iOwner.AudioEmitter);
      cue.Play();
    }
    this.mBlade = ArcaneBlade.GetInstance();
    float num1 = (float) ((double) iSpell.TotalMagnitude() * 0.5 + 1.5);
    this.mBlade.Initialize(SpellEffect.mPlayState, (iOwner as Character).Equipment[0].Item, iSpell.Element, num1);
    Vector3 position = iOwner.Position;
    Vector3 direction = iOwner.Direction;
    EntityManager entityManager = iOwner.PlayState.EntityManager;
    List<Entity> entities = entityManager.GetEntities(position, num1, true);
    for (int index = 0; index < entities.Count; ++index)
    {
      Vector3 oPosition;
      if (entities[index] is IDamageable t && t.ArcIntersect(out oPosition, position, direction, num1, 1.41371667f, 2f))
      {
        int num2 = (int) t.Damage(this.mDamage, iOwner as Entity, this.mTimeStamp, oPosition);
      }
    }
    entityManager.ReturnEntityList(entities);
  }

  public override bool CastUpdate(float iDeltaTime, ISpellCaster iOwner, out float oTurnSpeed)
  {
    oTurnSpeed = 0.0325f;
    if (this.mCastType == CastType.Self)
    {
      if (!this.mHasCast)
      {
        int num = (int) this.mCaster.Damage(this.mDamage, this.mCaster as Entity, this.mTimeStamp, this.mCaster.Position);
        this.mHasCast = true;
      }
      Vector3 position = iOwner.Position;
      Vector3 direction = iOwner.Direction;
      for (int index = 0; index < this.mSelfCastEffectReference.Length; ++index)
        EffectManager.Instance.UpdatePositionDirection(ref this.mSelfCastEffectReference[index], ref position, ref direction);
      if (((iOwner.Dead ? 1 : 0) | (!(iOwner is Avatar) ? 0 : ((iOwner as Avatar).CastType == CastType.None ? 1 : 0))) != 0)
        this.mTTL = 0.0f;
    }
    this.mTTL -= iDeltaTime;
    if (this.mRailGun != null)
    {
      this.mRailGun.Position = iOwner.CastSource.Translation;
      this.mRailGun.Direction = iOwner.Direction;
    }
    if (this.mRailGun != null && this.mRailGun.IsDead)
    {
      this.DeInitialize(iOwner);
      return false;
    }
    if (iOwner.CastType == CastType.Weapon || !((double) this.mTTL <= 0.0 | iOwner.Dead | iOwner.CastType == CastType.None))
      return base.CastUpdate(iDeltaTime, iOwner, out float _);
    this.DeInitialize(iOwner);
    return false;
  }

  private RailGunSpell()
  {
  }

  public override void DeInitialize(ISpellCaster iOwner)
  {
    if (!this.Active)
      return;
    this.mHasCast = false;
    if (this.mRailGun != null)
      this.mRailGun.Kill();
    this.mRailGun = (Railgun) null;
    if (this.mBlade != null)
    {
      this.mBlade.Kill();
      this.mBlade = (ArcaneBlade) null;
    }
    for (int index = 0; index < this.mSelfCastEffectReference.Length; ++index)
      EffectManager.Instance.Stop(ref this.mSelfCastEffectReference[index]);
    SpellEffect.mPlayState.SpellEffects.Remove((SpellEffect) this);
    RailGunSpell.ReturnToCache(this);
    this.Active = false;
  }
}
