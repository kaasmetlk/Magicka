// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Spells.SpellEffects.ProjectileSpell
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.Audio;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.Entities.Items;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Spells.SpellEffects;

internal class ProjectileSpell : SpellEffect
{
  private static List<ProjectileSpell> mCache;
  internal static Queue<ConditionCollection> sCachedConditions = new Queue<ConditionCollection>();
  public static readonly int[] SPELL_AREA_SOUND = new int[11]
  {
    "spell_earth_area".GetHashCodeCustom(),
    "spell_water_area".GetHashCodeCustom(),
    "spell_cold_area".GetHashCodeCustom(),
    "spell_fire_area".GetHashCodeCustom(),
    "spell_lightning_area".GetHashCodeCustom(),
    "spell_arcane_area".GetHashCodeCustom(),
    "spell_life_area".GetHashCodeCustom(),
    0,
    "spell_ice_area".GetHashCodeCustom(),
    "spell_steam_area".GetHashCodeCustom(),
    "spell_steam_area".GetHashCodeCustom()
  };
  public static readonly int[] SPELL_FORCE_SOUND = new int[11]
  {
    "spell_earth_force".GetHashCodeCustom(),
    "spell_water_force".GetHashCodeCustom(),
    "spell_cold_force".GetHashCodeCustom(),
    "spell_fire_force".GetHashCodeCustom(),
    "spell_lightning_force".GetHashCodeCustom(),
    "spell_arcane_force".GetHashCodeCustom(),
    "spell_life_force".GetHashCodeCustom(),
    0,
    "spell_ice_force".GetHashCodeCustom(),
    "spell_steam_force".GetHashCodeCustom(),
    "spell_steam_force".GetHashCodeCustom()
  };
  private static Model[] sEarthModels;
  private static Model[] sIceModels;
  private static Model[] sIceEarthModels;
  private int mNumProjectilesToCast;
  private int mNumTotalProjectiles;
  private float mRadius;
  private float mProjectileForce;
  private float mCharge;
  private float mCoolDown;
  private float mMass;
  private float mCoolDownBetweenSpells;
  private new double mTimeStamp;
  private DamageCollection5 mWeaponDamage;
  private IceBlade mIceBlade;

  public static void InitializeCache(int iSize)
  {
    ProjectileSpell.mCache = new List<ProjectileSpell>(iSize);
    for (int index = 0; index < iSize; ++index)
      ProjectileSpell.mCache.Add(new ProjectileSpell());
    ProjectileSpell.sCachedConditions = new Queue<ConditionCollection>(3);
    for (int index = 0; index < 3; ++index)
      ProjectileSpell.sCachedConditions.Enqueue(new ConditionCollection());
  }

  public static SpellEffect GetFromCache()
  {
    ProjectileSpell fromCache = ProjectileSpell.mCache[ProjectileSpell.mCache.Count - 1];
    ProjectileSpell.mCache.Remove(fromCache);
    SpellEffect.mPlayState.SpellEffects.Add((SpellEffect) fromCache);
    return (SpellEffect) fromCache;
  }

  public static void ReturnToCache(ProjectileSpell iEffect)
  {
    SpellEffect.mPlayState.SpellEffects.Remove((SpellEffect) iEffect);
    ProjectileSpell.mCache.Add(iEffect);
  }

  public ProjectileSpell()
  {
    this.mCastType = CastType.None;
    this.mSpellCues = new List<Cue>(8);
    if (ProjectileSpell.sEarthModels != null)
      return;
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      ProjectileSpell.sEarthModels = new Model[5];
      ProjectileSpell.sEarthModels[0] = Magicka.Game.Instance.Content.Load<Model>("Models/Missiles/Earth0");
      ProjectileSpell.sEarthModels[1] = Magicka.Game.Instance.Content.Load<Model>("Models/Missiles/Earth1");
      ProjectileSpell.sEarthModels[2] = Magicka.Game.Instance.Content.Load<Model>("Models/Missiles/Earth2");
      ProjectileSpell.sEarthModels[3] = Magicka.Game.Instance.Content.Load<Model>("Models/Missiles/Earth2");
      ProjectileSpell.sEarthModels[4] = Magicka.Game.Instance.Content.Load<Model>("Models/Missiles/Earth3");
      ProjectileSpell.sIceModels = new Model[1];
      ProjectileSpell.sIceModels[0] = Magicka.Game.Instance.Content.Load<Model>("Models/Missiles/Ice3");
      ProjectileSpell.sIceEarthModels = new Model[5];
      ProjectileSpell.sIceEarthModels[0] = Magicka.Game.Instance.Content.Load<Model>("Models/Missiles/IceEarth0");
      ProjectileSpell.sIceEarthModels[1] = Magicka.Game.Instance.Content.Load<Model>("Models/Missiles/IceEarth1");
      ProjectileSpell.sIceEarthModels[2] = Magicka.Game.Instance.Content.Load<Model>("Models/Missiles/IceEarth2");
      ProjectileSpell.sIceEarthModels[3] = Magicka.Game.Instance.Content.Load<Model>("Models/Missiles/IceEarth2");
      ProjectileSpell.sIceEarthModels[4] = Magicka.Game.Instance.Content.Load<Model>("Models/Missiles/IceEarth3");
    }
  }

  public override void CastArea(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
  {
    base.CastArea(iSpell, iOwner, iFromStaff);
    this.mTimeStamp = iOwner.PlayState.PlayTime;
    this.mCharge = 0.0f;
    this.mRadius = iSpell.BlastSize() * 10f;
    this.mCoolDownBetweenSpells = 0.25f;
    this.mCastType = CastType.Area;
    this.mNumProjectilesToCast = 0;
    this.mSpell = iSpell;
    this.mCoolDown = 0.0f;
    DamageCollection5 oDamages;
    this.mSpell.CalculateDamage(SpellType.Projectile, CastType.Area, out oDamages);
    for (int iIndex = 0; iIndex < 11; ++iIndex)
    {
      int num = (int) this.mSpell[Spell.ElementFromIndex(iIndex)];
      if (num > 0)
        AudioManager.Instance.PlayCue<SpellSoundVariables>(Banks.Spells, ProjectileSpell.SPELL_AREA_SOUND[iIndex], new SpellSoundVariables()
        {
          mMagnitude = (float) num
        });
    }
    int num1 = (int) Blast.GroundBlast(iOwner.PlayState, iOwner as Entity, this.mTimeStamp, iOwner as Entity, this.mRadius, iOwner.Position, oDamages);
  }

  public override void CastForce(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
  {
    base.CastForce(iSpell, iOwner, iFromStaff);
    this.mTimeStamp = iOwner.PlayState.PlayTime;
    switch (iOwner)
    {
      case Avatar _:
      case BossSpellCasterZone _:
        this.mCharge = iOwner.SpellPower;
        break;
      default:
        this.mCharge = 0.5f;
        break;
    }
    this.mProjectileForce = (float) (25.0 + 85.0 * (double) iOwner.SpellPower);
    this.mCastType = CastType.Force;
    this.mSpell = iSpell;
    this.mMass = 1f + Math.Max(0.0f, 50f * this.mSpell[Elements.Earth]);
    this.mCoolDown = 0.0f;
    Spell spell = iSpell;
    spell.Element &= ~Elements.Earth;
    this.mRadius = spell.BlastSize() * 10f;
    SpellSoundVariables iVariables = new SpellSoundVariables();
    iVariables.mMagnitude = iOwner.SpellPower * 3f;
    iOwner.SpellPower = 0.0f;
    if ((iSpell.Element & Elements.Earth) == Elements.Earth)
    {
      this.mProjectileForce *= (float) (0.5 + 0.5 / (double) iSpell[Elements.Earth]);
      if ((iSpell.Element & Elements.Ice) == Elements.Ice)
        AudioManager.Instance.PlayCue<SpellSoundVariables>(Banks.Spells, ProjectileSpell.SPELL_FORCE_SOUND[Spell.ElementIndex(Elements.Ice)], iVariables);
      else
        AudioManager.Instance.PlayCue<SpellSoundVariables>(Banks.Spells, ProjectileSpell.SPELL_FORCE_SOUND[Spell.ElementIndex(Elements.Earth)], iVariables);
      this.mNumProjectilesToCast = 1;
      this.mNumTotalProjectiles = 1;
    }
    else
    {
      if ((iSpell.Element & Elements.Ice) != Elements.Ice)
        throw new Exception("Invalid projectile spell!");
      AudioManager.Instance.PlayCue<SpellSoundVariables>(Banks.Spells, ProjectileSpell.SPELL_FORCE_SOUND[Spell.ElementIndex(Elements.Ice)], iVariables);
      this.mNumProjectilesToCast = (int) (3.0 * (double) iSpell.IceMagnitude);
      this.mNumTotalProjectiles = this.mNumProjectilesToCast;
    }
    this.mCoolDownBetweenSpells = (float) (0.087499998509883881 + (double) this.mSpell.TotalMagnitude() / 5.0 * 0.34999999403953552 * 0.75) / (float) this.mNumProjectilesToCast;
  }

  public override void CastSelf(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
  {
    base.CastSelf(iSpell, iOwner, iFromStaff);
    this.mTimeStamp = iOwner.PlayState.PlayTime;
    this.mCharge = 0.5f;
    this.mProjectileForce = (float) (25.0 + 85.0 * (double) iOwner.SpellPower);
    this.mCastType = CastType.Self;
    this.mSpell = iSpell;
    this.mMass = 1f + Math.Max(0.0f, 50f * this.mSpell[Elements.Earth]);
    this.mCoolDown = 0.0f;
    Spell spell = iSpell;
    spell.Element &= ~Elements.Earth;
    this.mRadius = spell.BlastSize() * 10f;
    SpellSoundVariables iVariables = new SpellSoundVariables();
    iVariables.mMagnitude = iOwner.SpellPower * 3f;
    iOwner.SpellPower = 0.0f;
    if ((iSpell.Element & Elements.Earth) == Elements.Earth)
    {
      this.mProjectileForce *= (float) (0.5 + 0.5 / (double) iSpell[Elements.Earth]);
      if ((iSpell.Element & Elements.Ice) == Elements.Ice)
        AudioManager.Instance.PlayCue<SpellSoundVariables>(Banks.Spells, ProjectileSpell.SPELL_FORCE_SOUND[Spell.ElementIndex(Elements.Ice)], iVariables);
      else
        AudioManager.Instance.PlayCue<SpellSoundVariables>(Banks.Spells, ProjectileSpell.SPELL_FORCE_SOUND[Spell.ElementIndex(Elements.Earth)], iVariables);
      this.mNumProjectilesToCast = 1;
      this.mNumTotalProjectiles = 1;
    }
    else
    {
      if ((iSpell.Element & Elements.Ice) != Elements.Ice)
        throw new Exception("Invalid projectile spell!");
      AudioManager.Instance.PlayCue<SpellSoundVariables>(Banks.Spells, ProjectileSpell.SPELL_FORCE_SOUND[Spell.ElementIndex(Elements.Ice)], iVariables);
      this.mNumProjectilesToCast = (int) (3.0 * (double) iSpell.IceMagnitude);
      this.mNumTotalProjectiles = this.mNumProjectilesToCast;
    }
    this.mCoolDownBetweenSpells = (float) (0.087499998509883881 + (double) this.mSpell.TotalMagnitude() / 5.0 * 0.34999999403953552 * 0.75) / (float) this.mNumProjectilesToCast;
  }

  public override void CastWeapon(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
  {
    base.CastWeapon(iSpell, iOwner, iFromStaff);
    this.mTimeStamp = iOwner.PlayState.PlayTime;
    if (iOwner is Character && !((iOwner as Character).CurrentState is PanicCastState) && this.mFromStaff)
      this.mFromStaff = false;
    this.mCastType = CastType.Weapon;
    this.mSpell = iSpell;
    this.mSpell.CalculateDamage(SpellType.Projectile, CastType.Weapon, out this.mWeaponDamage);
    this.mCoolDownBetweenSpells = 0.25f;
    this.mCoolDown = 0.0f;
    if ((double) this.mSpell[Elements.Ice] <= 0.0)
      return;
    this.mCoolDownBetweenSpells = 0.25f;
    Spell spell = iSpell with { EarthMagnitude = 0.0f };
    spell.Element &= ~Elements.Earth;
    DamageCollection5 oDamages;
    spell.CalculateDamage(SpellType.Projectile, CastType.Weapon, out oDamages);
    float iRange = (float) (1.25 * (double) spell.IceMagnitude + 3.75);
    this.mIceBlade = IceBlade.GetInstance();
    this.mIceBlade.Initialize((iOwner as Character).Equipment[0].Item, ref oDamages, iRange);
  }

  internal override void AnimationEnd(ISpellCaster iOwner)
  {
    base.AnimationEnd(iOwner);
    if (this.mCastType != CastType.Weapon)
      return;
    if (this.mIceBlade != null)
    {
      this.mIceBlade.Kill();
      this.mIceBlade = (IceBlade) null;
    }
    if ((this.mSpell.Element & Elements.Earth) != Elements.Earth)
      return;
    Spell mSpell = this.mSpell with { IceMagnitude = 0.0f };
    mSpell.Element &= ~Elements.Ice;
    Vector3 translation = iOwner.WeaponSource.Translation;
    Vector2 iVelocity = new Vector2();
    iVelocity.X = iOwner.Direction.X * 40f;
    iVelocity.Y = iOwner.Direction.Z * 40f;
    DamageCollection5 oDamages;
    mSpell.CalculateDamage(SpellType.Projectile, CastType.Weapon, out oDamages);
    UnderGroundAttack.GetFromCache(iOwner.PlayState).Initialize(ref translation, ref iVelocity, iOwner, this.mTimeStamp, 15f, oDamages, true);
  }

  public override bool CastUpdate(float iDeltaTime, ISpellCaster iOwner, out float oTurnSpeed)
  {
    oTurnSpeed = 0.0f;
    Vector3 position1 = iOwner.Position;
    Vector3 result1 = !this.mFromStaff ? iOwner.Direction : (this.CastType != CastType.Weapon ? iOwner.Direction : iOwner.WeaponSource.Forward);
    switch (this.mCastType)
    {
      case CastType.Force:
      case CastType.Area:
      case CastType.Self:
        this.mCoolDown -= iDeltaTime;
        while ((double) this.mCoolDown <= 0.0)
        {
          this.mCoolDown += this.mCoolDownBetweenSpells;
          Vector3 result2 = iOwner.CastSource.Translation;
          --this.mNumProjectilesToCast;
          bool flag = false;
          if (this.mCastType == CastType.Force)
          {
            Segment iSeg = new Segment();
            iSeg.Origin = iOwner.Position;
            Vector3 vector3 = result2;
            Vector3.Subtract(ref vector3, ref iSeg.Origin, out iSeg.Delta);
            foreach (Shield shield in iOwner.PlayState.EntityManager.Shields)
            {
              Vector3 oPosition;
              if (shield.SegmentIntersect(out oPosition, iSeg, (float) (0.10000000149011612 + 0.075000002980232239 * (double) this.mSpell[Elements.Earth])))
              {
                int iHash = (double) this.mSpell[Elements.Earth] <= 0.0 ? Defines.PROJECTILE_HIT_SMALL[Defines.ElementIndex(Elements.Ice)] : Defines.PROJECTILE_HIT_SMALL[Defines.ElementIndex(Elements.Earth)];
                Vector3 result3;
                Vector3.Negate(ref iSeg.Delta, out result3);
                result3.Normalize();
                EffectManager.Instance.StartEffect(iHash, ref oPosition, ref result3, out VisualEffectReference _);
                flag = true;
                break;
              }
            }
          }
          if (!flag)
          {
            Vector3 result4;
            if (this.mNumProjectilesToCast > 0)
            {
              if (this.mCastType == CastType.Self)
              {
                float num1 = 6f;
                float num2 = (float) SpellEffect.RANDOM.NextDouble() * 6.28318548f;
                Vector3 vector3 = new Vector3(num1 * (float) Math.Cos((double) num2), 10f, num1 * (float) Math.Sin((double) num2));
                Vector3.Add(ref result2, ref vector3, out result2);
                Vector3 position2 = iOwner.Position;
                Vector3.Subtract(ref position2, ref result2, out result1);
                result1.Normalize();
              }
              else
              {
                Matrix result5;
                Matrix.CreateRotationY((float) ((SpellEffect.RANDOM.NextDouble() - 0.5) * 1.5707963705062866 * (1.0 - (double) this.mCharge)), out result5);
                Vector3.Transform(ref result1, ref result5, out result1);
              }
              float scaleFactor = this.mProjectileForce * (float) (1.25 + 0.75 * SpellEffect.RANDOM.NextDouble());
              Vector3.Multiply(ref result1, scaleFactor, out result4);
            }
            else
            {
              if (this.mCastType == CastType.Self)
              {
                result2.Y += 8f;
                result1 = new Vector3(1f / 1000f, -1f, 0.0f);
              }
              Vector3.Multiply(ref result1, this.mProjectileForce, out result4);
            }
            if (this.mNumProjectilesToCast >= 0)
            {
              NetworkState state = NetworkManager.Instance.State;
              if ((state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer)) || state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer)) && (this.mCastType == CastType.Force || this.mCastType == CastType.Self))
              {
                MissileEntity iMissile = (MissileEntity) null;
                ProjectileSpell.SpawnMissile(ref iMissile, iOwner, 0.0f, ref result2, ref result4, ref this.mSpell, this.mRadius, this.mNumTotalProjectiles);
                if (NetworkManager.Instance.State != NetworkState.Offline)
                  NetworkManager.Instance.Interface.SendMessage<SpawnMissileMessage>(ref new SpawnMissileMessage()
                  {
                    Type = SpawnMissileMessage.MissileType.Spell,
                    Handle = iMissile.Handle,
                    Item = (ushort) this.mNumTotalProjectiles,
                    Owner = iOwner.Handle,
                    Position = result2,
                    Velocity = result4,
                    Spell = this.mSpell,
                    Homing = 0.0f,
                    Splash = this.mRadius
                  });
              }
            }
          }
        }
        break;
    }
    if (this.mCastType != CastType.Weapon && this.mNumProjectilesToCast <= 0 && this.Active)
      this.DeInitialize(iOwner);
    return base.CastUpdate(iDeltaTime, iOwner, out float _);
  }

  public override void DeInitialize(ISpellCaster iOwner)
  {
    if (!this.Active)
      return;
    if (this.mIceBlade != null)
    {
      this.mIceBlade.Kill();
      this.mIceBlade = (IceBlade) null;
    }
    this.Active = false;
    this.mCoolDown = 0.0f;
    ProjectileSpell.ReturnToCache(this);
  }

  public static void SpawnMissile(
    ref MissileEntity iMissile,
    ISpellCaster iOwner,
    float iHoming,
    ref Vector3 iPosition,
    ref Vector3 iVelocity,
    ref Spell iSpell,
    float iSplash,
    int iNumProjectiles)
  {
    ProjectileSpell.SpawnMissile(ref iMissile, (Model) null, iOwner, iHoming, ref iPosition, ref iVelocity, ref iSpell, iSplash, iNumProjectiles);
  }

  public static void SpawnMissile(
    ref MissileEntity iMissile,
    Model mdl,
    ISpellCaster iOwner,
    float iHoming,
    ref Vector3 iPosition,
    ref Vector3 iVelocity,
    ref Spell iSpell,
    float iSplash,
    int iNumProjectiles)
  {
    ConditionCollection iConditions;
    lock (ProjectileSpell.sCachedConditions)
      iConditions = ProjectileSpell.sCachedConditions.Dequeue();
    DamageCollection5 oDamages1;
    iSpell.CalculateDamage(SpellType.Projectile, CastType.Force, out oDamages1);
    Model iModel = (Model) null;
    if (mdl != null)
      iModel = mdl;
    iConditions.Clear();
    if ((iSpell.Element & Elements.PhysicalElements) == Elements.PhysicalElements)
    {
      if (iModel == null)
        iModel = ProjectileSpell.sIceEarthModels[Math.Min(Math.Max((int) iSpell.IceMagnitude + (int) iSpell.EarthMagnitude - 1, 0), 4)];
      iConditions[0].Condition.EventConditionType = EventConditionType.Default;
      iConditions[1].Condition.EventConditionType = EventConditionType.Hit;
      iConditions[1].Add(new EventStorage(new DamageEvent(oDamages1.A, true)));
      iConditions[1].Add(new EventStorage(new DamageEvent(oDamages1.B)));
      iConditions[1].Add(new EventStorage(new DamageEvent(oDamages1.C)));
      iConditions[1].Add(new EventStorage(new DamageEvent(oDamages1.D)));
      iConditions[1].Add(new EventStorage(new DamageEvent(oDamages1.E)));
      iConditions[1].Add(new EventStorage(new RemoveEvent()));
      iConditions[2].Condition.EventConditionType = EventConditionType.Collision;
      iConditions[2].Add(new EventStorage(new SpawnDecalEvent(Decal.Crater, (float) ((double) iSpell.EarthMagnitude + (double) iSpell.IceMagnitude - 1.0))));
      iConditions[2].Add(new EventStorage(new RemoveEvent(1)));
      for (int iIndex = 0; iIndex < 11; ++iIndex)
      {
        Elements iElement = Spell.ElementFromIndex(iIndex);
        float iMagnitude = iSpell[iElement];
        if ((double) iMagnitude > 0.0)
        {
          iConditions[0].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Defines.SOUNDS_PROJECTILE[iIndex], iMagnitude)));
          iConditions[0].Add(new EventStorage(new PlayEffectEvent(Defines.PROJECTILE_TRAIL_BIG[iIndex], true)));
          iConditions[1].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Defines.SOUNDS_BLAST[iIndex], false, iMagnitude)));
          iConditions[2].Add(new EventStorage(new PlayEffectEvent(Defines.PROJECTILE_HIT_BIG[iIndex], false)));
          iConditions[2].Add(new EventStorage(new PlayEffectEvent(Defines.PROJECTILE_HIT_BIG[iIndex], false)));
          iConditions[2].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Defines.SOUNDS_BLAST[iIndex], false, iMagnitude)));
        }
      }
      iConditions[3].Condition.EventConditionType = EventConditionType.Timer;
      iConditions[3].Condition.Time = 10f;
      iConditions[3].Add(new EventStorage(new RemoveEvent()));
    }
    else if ((iSpell.Element & Elements.Earth) == Elements.Earth)
    {
      if (iModel == null)
        iModel = ProjectileSpell.sEarthModels[Math.Min(Math.Max((int) iSpell.EarthMagnitude - 1, 0), 4)];
      iConditions[0].Condition.EventConditionType = EventConditionType.Default;
      iConditions[1].Condition.EventConditionType = EventConditionType.Hit;
      iConditions[1].Add(new EventStorage(new DamageEvent(oDamages1.A, true)));
      iConditions[1].Add(new EventStorage(new DamageEvent(oDamages1.B)));
      iConditions[1].Add(new EventStorage(new DamageEvent(oDamages1.C)));
      iConditions[1].Add(new EventStorage(new DamageEvent(oDamages1.D)));
      iConditions[1].Add(new EventStorage(new DamageEvent(oDamages1.E)));
      iConditions[2].Condition.EventConditionType = EventConditionType.Collision;
      iConditions[2].Add(new EventStorage(new SpawnDecalEvent(Decal.Crater, (float) (1.0 + ((double) iSpell.EarthMagnitude - 1.0) * 0.5))));
      if ((double) iSplash > 1.4012984643248171E-45)
      {
        Spell spell = iSpell with { EarthMagnitude = 0.0f };
        spell.Element &= ~Elements.Earth;
        if (spell.Element != Elements.None)
        {
          DamageCollection5 oDamages2;
          iSpell.CalculateDamage(spell.GetSpellType(), CastType.Area, out oDamages2);
          iConditions[1].Add(new EventStorage(new BlastEvent(iSplash, oDamages2)));
          iConditions[2].Add(new EventStorage(new BlastEvent(iSplash, oDamages2)));
        }
        iConditions[1].Add(new EventStorage(new RemoveEvent()));
        iConditions[2].Add(new EventStorage(new RemoveEvent()));
      }
      else
      {
        iConditions[1].Add(new EventStorage(new RemoveEvent(25)));
        iConditions[2].Add(new EventStorage(new RemoveEvent(1)));
      }
      for (int iIndex = 0; iIndex < 11; ++iIndex)
      {
        Elements iElement = Spell.ElementFromIndex(iIndex);
        float iMagnitude = iSpell[iElement];
        if ((double) iMagnitude > 0.0)
        {
          iConditions[0].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Defines.SOUNDS_PROJECTILE[iIndex])));
          iConditions[0].Add(new EventStorage(new PlayEffectEvent(Defines.PROJECTILE_TRAIL_BIG[iIndex], true)));
          if ((double) iSplash > 0.0)
          {
            iConditions[1].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Defines.SOUNDS_BLAST[iIndex], false, iMagnitude)));
            iConditions[2].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Defines.SOUNDS_BLAST[iIndex], false, iMagnitude)));
          }
          else
          {
            iConditions[2].Add(new EventStorage(new PlayEffectEvent(Defines.PROJECTILE_HIT_BIG[iIndex], false)));
            iConditions[2].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Defines.SOUNDS_HIT[iIndex], false, iMagnitude)));
          }
        }
      }
      iConditions[3].Condition.EventConditionType = EventConditionType.Timer;
      iConditions[3].Condition.Time = 10f;
      iConditions[3].Add(new EventStorage(new RemoveEvent()));
    }
    else if ((iSpell.Element & Elements.Ice) == Elements.Ice)
    {
      if (iModel == null)
        iModel = ProjectileSpell.sIceModels[0];
      oDamages1.A.AttackProperty |= AttackProperties.Piercing;
      oDamages1.B.AttackProperty |= AttackProperties.Piercing;
      oDamages1.C.AttackProperty |= AttackProperties.Piercing;
      oDamages1.D.AttackProperty |= AttackProperties.Piercing;
      oDamages1.E.AttackProperty |= AttackProperties.Piercing;
      int iHash1 = Defines.SOUNDS_HIT[Defines.ElementIndex(Elements.Ice)];
      iConditions[0].Condition.EventConditionType = EventConditionType.Default;
      for (int iIndex = 0; iIndex < 11; ++iIndex)
      {
        Elements iElement = Spell.ElementFromIndex(iIndex);
        if ((double) iSpell[iElement] > 0.0)
        {
          int iHash2 = Defines.PROJECTILE_TRAIL_SMALL[iIndex];
          iConditions[0].Add(new EventStorage(new PlayEffectEvent(iHash2, true)));
          int iHash3 = Defines.PROJECTILE_HIT_SMALL[iIndex];
          iConditions[1].Add(new EventStorage(new PlayEffectEvent(iHash3)));
          iConditions[2].Add(new EventStorage(new PlayEffectEvent(iHash3)));
        }
      }
      oDamages1.MultiplyMagnitude(1f / (float) iNumProjectiles);
      iConditions[1].Condition.EventConditionType = EventConditionType.Hit;
      iConditions[1].Add(new EventStorage(new DamageEvent(oDamages1.A, true)));
      iConditions[1].Add(new EventStorage(new DamageEvent(oDamages1.B)));
      iConditions[1].Add(new EventStorage(new DamageEvent(oDamages1.C)));
      iConditions[1].Add(new EventStorage(new DamageEvent(oDamages1.D)));
      iConditions[1].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, iHash1)));
      iConditions[2].Condition.EventConditionType = EventConditionType.Collision;
      iConditions[2].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, iHash1)));
      iConditions[2].Add(new EventStorage(new RemoveEvent()));
      iConditions[3].Condition.EventConditionType = EventConditionType.Timer;
      iConditions[3].Condition.Time = 10f;
      iConditions[3].Add(new EventStorage(new RemoveEvent()));
    }
    else
    {
      Console.WriteLine("I think projectile is a pretty cool guy, eh bounces into things and doesn't afraid of anything!");
      iConditions[0].Condition.EventConditionType = EventConditionType.Default;
      iConditions[1].Condition.EventConditionType = EventConditionType.Hit;
      iConditions[1].Add(new EventStorage(new DamageEvent(oDamages1.A, true)));
      iConditions[1].Add(new EventStorage(new DamageEvent(oDamages1.B)));
      iConditions[1].Add(new EventStorage(new DamageEvent(oDamages1.C)));
      iConditions[1].Add(new EventStorage(new DamageEvent(oDamages1.D)));
      iConditions[1].Add(new EventStorage(new DamageEvent(oDamages1.E)));
      iConditions[2].Condition.EventConditionType = EventConditionType.Collision;
      iConditions[2].Add(new EventStorage(new SpawnDecalEvent(Decal.Crater, (float) (1.0 + ((double) iSpell.EarthMagnitude - 1.0) * 0.5))));
      if ((double) iSplash > 1.4012984643248171E-45)
      {
        Spell spell = iSpell with { EarthMagnitude = 0.0f };
        spell.Element &= ~Elements.Earth;
        if (spell.Element != Elements.None)
        {
          DamageCollection5 oDamages3;
          iSpell.CalculateDamage(spell.GetSpellType(), CastType.Area, out oDamages3);
          iConditions[1].Add(new EventStorage(new BlastEvent(iSplash, oDamages3)));
          iConditions[2].Add(new EventStorage(new BlastEvent(iSplash, oDamages3)));
        }
        iConditions[1].Add(new EventStorage(new RemoveEvent()));
        iConditions[2].Add(new EventStorage(new RemoveEvent()));
      }
      else
      {
        iConditions[1].Add(new EventStorage(new RemoveEvent(25)));
        iConditions[2].Add(new EventStorage(new RemoveEvent(1)));
      }
      for (int iIndex = 0; iIndex < 11; ++iIndex)
      {
        Elements iElement = Spell.ElementFromIndex(iIndex);
        float iMagnitude = iSpell[iElement];
        if ((double) iMagnitude > 0.0)
        {
          iConditions[0].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Defines.SOUNDS_PROJECTILE[iIndex])));
          iConditions[0].Add(new EventStorage(new PlayEffectEvent(Defines.PROJECTILE_TRAIL_BIG[iIndex], true)));
          if ((double) iSplash > 0.0)
          {
            iConditions[1].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Defines.SOUNDS_BLAST[iIndex], false, iMagnitude)));
            iConditions[2].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Defines.SOUNDS_BLAST[iIndex], false, iMagnitude)));
          }
          else
          {
            iConditions[2].Add(new EventStorage(new PlayEffectEvent(Defines.PROJECTILE_HIT_BIG[iIndex], false)));
            iConditions[2].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Defines.SOUNDS_HIT[iIndex], false, iMagnitude)));
          }
        }
      }
      iConditions[3].Condition.EventConditionType = EventConditionType.Timer;
      iConditions[3].Condition.Time = 10f;
      iConditions[3].Add(new EventStorage(new RemoveEvent()));
    }
    if (iMissile == null)
      iMissile = iOwner.GetMissileInstance();
    Vector3 result1 = new Vector3();
    float num = 0.0f;
    if ((double) iSpell.FireMagnitude > 1.4012984643248171E-45)
    {
      result1.X += Spell.FIRECOLOR.X;
      result1.Y += Spell.FIRECOLOR.Y;
      result1.Z += Spell.FIRECOLOR.Z;
      ++num;
    }
    if ((double) iSpell.ArcaneMagnitude > 1.4012984643248171E-45)
    {
      result1.X += Spell.ARCANECOLOR.X;
      result1.Y += Spell.ARCANECOLOR.Y;
      result1.Z += Spell.ARCANECOLOR.Z;
      ++num;
    }
    if ((double) iSpell.LifeMagnitude > 1.4012984643248171E-45)
    {
      result1.X += Spell.LIFECOLOR.X;
      result1.Y += Spell.LIFECOLOR.Y;
      result1.Z += Spell.LIFECOLOR.Z;
      ++num;
    }
    if ((double) iSpell.LightningMagnitude > 1.4012984643248171E-45)
    {
      result1.X += Spell.LIGHTNINGCOLOR.X;
      result1.Y += Spell.LIGHTNINGCOLOR.Y;
      result1.Z += Spell.LIGHTNINGCOLOR.Z;
      ++num;
    }
    if ((double) iSpell.ColdMagnitude > 1.4012984643248171E-45)
    {
      result1.X += Spell.COLDCOLOR.X;
      result1.Y += Spell.COLDCOLOR.Y;
      result1.Z += Spell.COLDCOLOR.Z;
      ++num;
    }
    if ((double) num > 1.4012984643248171E-45)
    {
      Vector3.Divide(ref result1, num, out result1);
      Vector3 result2;
      Vector3.Multiply(ref result1, 1.333f, out result2);
      Vector3 result3;
      Vector3.Multiply(ref result1, 0.666f, out result3);
      iConditions[0].Add(new EventStorage(new LightEvent(6f, result2, result3, result1.Length())));
    }
    iMissile.Initialize(iOwner as Entity, (float) (0.075000002980232239 + 0.075000002980232239 * (double) iSpell[Elements.Earth]), ref iPosition, ref iVelocity, iModel, iConditions, true, iSpell);
    iOwner.PlayState.EntityManager.AddEntity((Entity) iMissile);
    lock (ProjectileSpell.sCachedConditions)
      ProjectileSpell.sCachedConditions.Enqueue(iConditions);
  }
}
