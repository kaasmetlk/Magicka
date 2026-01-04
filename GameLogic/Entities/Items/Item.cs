// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Items.Item
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using JigLibX.Math;
using Magicka.Achievements;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Abilities;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.Entities.AnimationActions;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.Entities.Buffs;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Graphics.Lights;
using Magicka.Levels;
using Magicka.Levels.Versus;
using Magicka.Localization;
using Magicka.Network;
using Magicka.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using PolygonHead.Lights;
using System;
using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace Magicka.GameLogic.Entities.Items;

public class Item : Pickable
{
  private static Queue<Item> sPickableCache;
  public static readonly int[] ChantEffects = new int[13]
  {
    "swordelementeffect_earth".GetHashCodeCustom(),
    "swordelementeffect_water".GetHashCodeCustom(),
    "swordelementeffect_cold".GetHashCodeCustom(),
    "swordelementeffect_fire".GetHashCodeCustom(),
    "swordelementeffect_lightning".GetHashCodeCustom(),
    "swordelementeffect_arcane".GetHashCodeCustom(),
    "swordelementeffect_life".GetHashCodeCustom(),
    "swordelementeffect_shield".GetHashCodeCustom(),
    "swordelementeffect_ice".GetHashCodeCustom(),
    "swordelementeffect_steam".GetHashCodeCustom(),
    "swordelementeffect_cold".GetHashCodeCustom(),
    "swordelementeffect_cold".GetHashCodeCustom(),
    "swordelementeffect_cold".GetHashCodeCustom()
  };
  public static readonly int SPECIAL_READY_EFFECT = "special_ready".GetHashCodeCustom();
  public static readonly int RECHARGE_SOUND = "staff_recharge01".GetHashCodeCustom();
  public static readonly int TYRFING_TYPE = "weapon_tyrfing".GetHashCodeCustom();
  public static readonly int SONIC_SCREWDRIVER = "weapon_sonicscrewdriver".GetHashCodeCustom();
  public static readonly int SONIC_SCREWDRIVER_HITEFFECT = "magick_generic_2".GetHashCodeCustom();
  private static Dictionary<int, Item> CachedWeapons = new Dictionary<int, Item>();
  private static Random sRandom = new Random();
  private Magicka.GameLogic.Entities.Character mOwner;
  private Matrix mAttach0;
  private Matrix mAttach1;
  private Vector3 mEffect0Pos;
  private Vector3 mEffect1Pos;
  protected Matrix mAttach0AbsoluteTransform;
  protected Matrix mAttach1AbsoluteTransform;
  protected Vector3 mEffect0AbsolutePos;
  protected Vector3 mEffect1AbsolutePos;
  protected Vector3 mLastAttachAbsolutePosition;
  private Item.ElementRenderData[] mElementRenderData;
  private static readonly int DEATH_SOUND = "wep_deathblow".GetHashCodeCustom();
  private static readonly int[] GUN_HIT_EFFECTS = new int[10]
  {
    "gunhit_generic".GetHashCodeCustom(),
    "gunhit_gravel".GetHashCodeCustom(),
    "gunhit_grass".GetHashCodeCustom(),
    "gunhit_wood".GetHashCodeCustom(),
    "gunhit_snow".GetHashCodeCustom(),
    "gunhit_stone".GetHashCodeCustom(),
    "gunhit_mud".GetHashCodeCustom(),
    "gunhit_generic".GetHashCodeCustom(),
    "gunhit_water".GetHashCodeCustom(),
    "gunhit_lava".GetHashCodeCustom()
  };
  private static readonly int GUN_WATER_HIT_EFFECT = "footstep_water".GetHashCodeCustom();
  private static readonly int GUN_ICE_HIT_EFFECT = "footstep_snow".GetHashCodeCustom();
  private static readonly int GUN_LAVA_HIT_EFFECT = "footstep_lava".GetHashCodeCustom();
  private static readonly int GUN_CRUST_HIT_EFFECT = "footstep_gravel".GetHashCodeCustom();
  private Model mProjectileModel;
  protected bool mMeleeMultiHit;
  protected float mHoming;
  protected float mRangedDanger;
  protected float mRangedElevation;
  protected bool mContinueHitting;
  private float mTeleportCooldown;
  private float mShieldDeflectCoolDown;
  private float mScale = 1f;
  private float mTimeToRemove = 15f;
  protected float mMeleeRange;
  protected ConditionCollection mMeleeConditions;
  private float mRangedRange;
  private bool mFacing;
  private ConditionCollection mRangedConditions;
  private ConditionCollection mGunConditions;
  private Banks mGunSoundBank;
  private int mGunSoundID;
  private int mGunMuzzleEffectID;
  private int mGunShellsEffectID;
  private int mGunClip;
  private int mGunCurrentClip;
  private float mGunAccuracy;
  private float mGunCurrentAccuracy;
  private float mGunRate;
  private float mGunRateTimer;
  private float mDespawnTime;
  private VisualEffectReference mDespawnEffect;
  private float mGunRange;
  private bool mFiring;
  private float mTracerVelocity = 100f;
  private float mTracerSprite = 35f;
  private float mNonTracerSprite = -1f;
  private uint mTracerCount;
  private Cue mGunSound;
  private VisualEffectReference mGunShellsEffect;
  private StaticList<Spell> mSpellQueue;
  private VisualEffectReference[] mSpellEffects;
  private int[] mEffects;
  private VisualEffectReference[] mFollowEffects = new VisualEffectReference[4];
  private VisualEffectReference[] mVisualEffects = new VisualEffectReference[4];
  private KeyValuePair<int, Banks>[] mSounds;
  private Cue[] mSoundCues = new Cue[4];
  private Item.PointLightHolder mPointLightHolder;
  private DynamicLight mPointLight;
  private int mBlockValue;
  private WeaponClass mWeaponClass;
  private List<ActiveAura> mAuras = new List<ActiveAura>();
  private float mCooldownTime;
  private bool mHideModel;
  private bool mHideEffect;
  private bool mPauseSounds;
  private float mCooldownTimer;
  private float mWorldCollisionTimer;
  private bool mAnimationDetached;
  private bool mAttached;
  private Ability mSourceAbility;
  private Magicka.GameLogic.Entities.Abilities.SpecialAbilities.SpecialAbility mSpecialAbility;
  private float mSpecialAbilityRechargeTime;
  private float mSpecialAbilityCooldown;
  protected Magicka.GameLogic.Entities.Resistance[] mResistances = new Magicka.GameLogic.Entities.Resistance[11];
  private static readonly int STEAM_EFFECT = "birch_steam".GetHashCodeCustom();
  private static readonly int GUNGNER_EFFECT = "gungner".GetHashCodeCustom();
  protected Item.PassiveAbilityStruct mPassiveAbility;
  protected VisualEffectReference mPassiveAbilityEffect;
  protected float mPassiveAbilityTimer;
  protected TeslaField mTeslaField;
  protected float mGlow;
  protected float mGlowTarget;
  protected MissileEntity mGungnirMissile;
  protected List<IDamageable> mHitlist;
  protected int mNextToDamage;
  protected Vector3 mSpellColor;
  protected float mSpellTime;
  private static readonly int ITEM_PICKUP_LOC = "#item_pick_up".GetHashCodeCustom();
  protected string mPickUpString;
  protected bool mIgnoreTractorPull;
  protected int mAnimatedLevelPartID;
  protected bool mBound;

  public static Item GetPickableIntstance()
  {
    Item pickableIntstance = Item.sPickableCache.Dequeue();
    Item.sPickableCache.Enqueue(pickableIntstance);
    return pickableIntstance;
  }

  public static void InitializePickableCache(int iNr, PlayState iPlayState)
  {
    Item.sPickableCache = new Queue<Item>(iNr);
    for (int index = 0; index < iNr; ++index)
      Item.sPickableCache.Enqueue(new Item(iPlayState, (Magicka.GameLogic.Entities.Character) null));
  }

  public static void GetCachedWeapon(int iType, Item iTarget)
  {
    if (!Item.CachedWeapons.ContainsKey(iType))
      throw new Exception("Weapon not cached");
    Item.CachedWeapons[iType].Copy(iTarget);
  }

  public static Item GetCachedWeapon(int iType)
  {
    return Item.CachedWeapons.ContainsKey(iType) ? Item.CachedWeapons[iType] : throw new Exception("Weapon not cached");
  }

  public static void CacheWeapon(int iType, Item iItem) => Item.CachedWeapons[iType] = iItem;

  public static void ClearCache() => Item.CachedWeapons.Clear();

  public Item(PlayState iPlayState, Magicka.GameLogic.Entities.Character iOwner)
    : base(iPlayState)
  {
    if (iPlayState != null)
    {
      this.mElementRenderData = new Item.ElementRenderData[3];
      for (int index = 0; index < 3; ++index)
      {
        Item.RenderData renderData = new Item.RenderData();
        this.mRenderData[index] = (Pickable.RenderData) renderData;
        this.mElementRenderData[index] = new Item.ElementRenderData();
      }
      this.mOwner = iOwner;
      this.mHitlist = new List<IDamageable>(64 /*0x40*/);
    }
    this.mMeleeConditions = new ConditionCollection();
    this.mRangedConditions = new ConditionCollection();
    this.mGunConditions = new ConditionCollection();
    this.mSpellQueue = (StaticList<Spell>) new StaticEquatableList<Spell>(5);
    this.mSpellEffects = new VisualEffectReference[11];
    for (int index = 0; index < this.mSpellEffects.Length; ++index)
      this.mSpellEffects[index].ID = -1;
    this.mAttached = true;
    this.mBound = false;
    this.mAnimationDetached = false;
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if (!this.mAttached)
    {
      this.mVisible = true;
      if (!this.mPickable)
        this.mTimeToRemove -= iDeltaTime;
    }
    if (this.mType == Item.TYRFING_TYPE)
    {
      double num1 = (double) System.Math.Max(0.0f, this.mTeleportCooldown -= iDeltaTime);
    }
    if ((double) this.mTimeToRemove < 0.0)
      this.Deinitialize();
    this.mShieldDeflectCoolDown -= iDeltaTime;
    this.mCooldownTimer -= iDeltaTime;
    this.mWorldCollisionTimer -= iDeltaTime;
    if (this.mPassiveAbility.Ability == Item.PassiveAbilities.Gungner)
    {
      Vector3 position = this.Position;
      Vector3 up = this.mBody.Orientation.Up;
      if (this.mGungnirMissile != null)
      {
        if (this.mGungnirMissile.Dead)
        {
          EffectManager.Instance.UpdatePositionDirection(ref this.mPassiveAbilityEffect, ref position, ref up);
          this.mGungnirMissile = (MissileEntity) null;
          this.mVisible = true;
        }
        else
        {
          if (!EffectManager.Instance.UpdatePositionDirection(ref this.mPassiveAbilityEffect, ref position, ref up))
            EffectManager.Instance.StartEffect(Item.GUNGNER_EFFECT, ref position, ref up, out this.mPassiveAbilityEffect);
          this.mVisible = false;
        }
      }
      else
      {
        EffectManager.Instance.UpdatePositionDirection(ref this.mPassiveAbilityEffect, ref position, ref up);
        this.mVisible = true;
      }
    }
    base.Update(iDataChannel, iDeltaTime);
    Matrix orientation = this.mBody.Orientation with
    {
      Translation = this.mBody.Position
    };
    if (!this.mAttached && this.mPickable)
    {
      orientation.M11 *= 1.5f;
      orientation.M12 *= 1.5f;
      orientation.M13 *= 1.5f;
      orientation.M21 *= 1.5f;
      orientation.M22 *= 1.5f;
      orientation.M23 *= 1.5f;
      orientation.M31 *= 1.5f;
      orientation.M32 *= 1.5f;
      orientation.M33 *= 1.5f;
    }
    this.mRenderData[(int) iDataChannel].mTransform = orientation;
    this.mHighlightRenderData[(int) iDataChannel].mTransform = orientation;
    this.mLastAttachAbsolutePosition = this.mAttach0AbsoluteTransform.Translation;
    Matrix.Multiply(ref this.mAttach0, ref orientation, out this.mAttach0AbsoluteTransform);
    Matrix.Multiply(ref this.mAttach1, ref orientation, out this.mAttach1AbsoluteTransform);
    Vector3.Transform(ref this.mEffect0Pos, ref orientation, out this.mEffect0AbsolutePos);
    Vector3.Transform(ref this.mEffect1Pos, ref orientation, out this.mEffect1AbsolutePos);
    bool flag1 = (double) this.mSpecialAbilityCooldown > 0.0;
    this.mSpecialAbilityCooldown -= iDeltaTime;
    if (flag1 && (double) this.mSpecialAbilityCooldown <= 0.0)
    {
      EffectManager.Instance.StartEffect(Item.SPECIAL_READY_EFFECT, ref this.mAttach0AbsoluteTransform, out VisualEffectReference _);
      AudioManager.Instance.PlayCue(Banks.Weapons, Item.RECHARGE_SOUND, this.AudioEmitter);
    }
    Vector3 forward = this.mAttach0AbsoluteTransform.Forward;
    Vector3 translation1 = this.mAttach0AbsoluteTransform.Translation;
    Elements elements1 = Elements.None;
    for (int iIndex = 0; iIndex < this.mSpellQueue.Count; ++iIndex)
      elements1 |= this.mSpellQueue[iIndex].Element;
    Matrix iTransform = new Matrix();
    Vector3 result1;
    Vector3.Subtract(ref this.mEffect1AbsolutePos, ref this.mEffect0AbsolutePos, out result1);
    if ((double) result1.LengthSquared() <= 9.9999999747524271E-07)
      result1 = forward;
    else
      result1.Normalize();
    Vector3 result2 = new Vector3();
    result2.Y = 1f;
    Vector3 result3;
    Vector3.Cross(ref result2, ref result1, out result3);
    Vector3.Cross(ref result1, ref result3, out result2);
    result3.Normalize();
    result2.Normalize();
    iTransform.M11 = result3.X;
    iTransform.M12 = result3.Y;
    iTransform.M13 = result3.Z;
    iTransform.M21 = result2.X;
    iTransform.M22 = result2.Y;
    iTransform.M23 = result2.Z;
    iTransform.M31 = result1.X;
    iTransform.M32 = result1.Y;
    iTransform.M33 = result1.Z;
    iTransform.M41 = this.mEffect0AbsolutePos.X;
    iTransform.M42 = this.mEffect0AbsolutePos.Y;
    iTransform.M43 = this.mEffect0AbsolutePos.Z;
    iTransform.M44 = 1f;
    if (this.mAttached)
    {
      for (int iIndex = 0; iIndex < this.mSpellEffects.Length; ++iIndex)
      {
        Elements elements2 = Spell.ElementFromIndex(iIndex);
        if ((elements1 & elements2) == elements2)
        {
          if (this.mSpellEffects[iIndex].ID < 0)
            EffectManager.Instance.StartEffect(Item.ChantEffects[iIndex], ref iTransform, out this.mSpellEffects[iIndex]);
          else
            EffectManager.Instance.UpdateOrientation(ref this.mSpellEffects[iIndex], ref iTransform);
        }
        else if (this.mSpellEffects[iIndex].ID >= 0)
          EffectManager.Instance.Stop(ref this.mSpellEffects[iIndex]);
      }
      if (this.mEffects != null)
      {
        for (int index = 0; index < this.mEffects.Length; ++index)
        {
          if (this.mEffects[index] != 0 && index < this.mVisualEffects.Length)
          {
            if (this.mVisible && !this.mIsInvisible && !this.HideEffects)
            {
              if (!EffectManager.Instance.UpdatePositionDirection(ref this.mVisualEffects[index], ref this.mEffect0AbsolutePos, ref result1))
                EffectManager.Instance.StartEffect(this.mEffects[index], ref translation1, ref forward, out this.mVisualEffects[index]);
            }
            else
              EffectManager.Instance.Stop(ref this.mVisualEffects[index]);
          }
        }
      }
      if (this.mFollowEffects != null)
      {
        for (int index = 0; index < this.mFollowEffects.Length; ++index)
        {
          if (EffectManager.Instance.IsActive(ref this.mFollowEffects[index]))
          {
            if (this.mVisible && !this.mIsInvisible && !this.HideEffects)
            {
              if (!EffectManager.Instance.UpdatePositionDirection(ref this.mFollowEffects[index], ref this.mEffect0AbsolutePos, ref result1))
                EffectManager.Instance.StartEffect(this.mFollowEffects[index].ID, ref this.mEffect0AbsolutePos, ref result1, out this.mFollowEffects[index]);
            }
            else
              EffectManager.Instance.Stop(ref this.mFollowEffects[index]);
          }
        }
      }
      if (this.mSounds != null)
      {
        for (int index = 0; index < this.mSounds.Length; ++index)
        {
          if (this.mSounds[index].Key != 0 && index < this.mSoundCues.Length)
          {
            if (this.mPauseSounds)
            {
              if (this.mSoundCues[index] != null && !this.mSoundCues[index].IsPaused)
                this.mSoundCues[index].Pause();
            }
            else if (this.mSoundCues[index] == null || this.mSoundCues[index].IsStopped || this.mSoundCues[index].IsPaused)
              this.mSoundCues[index] = AudioManager.Instance.PlayCue(this.mSounds[index].Value, this.mSounds[index].Key, this.mAudioEmitter);
          }
        }
      }
      if (this.mPointLightHolder.ContainsLight && !this.mPointLightHolder.Enabled && this.mOwner != null)
      {
        if (this.mVisible && !this.mIsInvisible && !this.mHideModel)
        {
          this.mPointLightHolder.Enabled = true;
          this.mPointLight = DynamicLight.GetCachedLight();
          this.mPointLight.AmbientColor = this.mPointLightHolder.AmbientColor;
          this.mPointLight.DiffuseColor = this.mPointLightHolder.DiffuseColor;
          this.mPointLight.Radius = this.mPointLightHolder.Radius;
          this.mPointLight.SpecularAmount = this.mPointLightHolder.SpecularAmount;
          this.mPointLight.VariationAmount = this.mPointLightHolder.VariationAmount;
          this.mPointLight.VariationSpeed = this.mPointLightHolder.VariationSpeed;
          this.mPointLight.VariationType = this.mPointLightHolder.VariationType;
          this.mPointLight.Position = this.mAttach0AbsoluteTransform.Translation;
          this.mPointLight.Speed = 1f;
          this.mPointLight.Intensity = 1f;
          this.mPointLight.Enable(this.mOwner.PlayState.Scene);
        }
      }
      else if (this.mPointLight != null && this.mPointLight.Enabled)
      {
        if (this.mVisible && !this.mIsInvisible && !this.mHideModel)
        {
          this.mPointLight.Position = this.mAttach0AbsoluteTransform.Translation;
        }
        else
        {
          this.mPointLight.Disable();
          this.mPointLight = (DynamicLight) null;
          this.mPointLightHolder.Enabled = false;
        }
      }
      if (this.mOwner != null)
      {
        for (int index = 0; index < this.mAuras.Count; ++index)
        {
          ActiveAura mAura = this.mAuras[index];
          mAura.Execute(this.mOwner, iDeltaTime);
          this.mAuras[index] = mAura;
          Vector3 position = this.mOwner.Position;
          Vector3 direction = this.mOwner.Direction;
          VisualEffectReference mEffect = this.mAuras[index].mEffect;
          EffectManager.Instance.UpdatePositionDirection(ref mEffect, ref position, ref direction);
        }
      }
    }
    else
    {
      for (int index = 0; index < this.mSpellEffects.Length; ++index)
      {
        if (this.mSpellEffects[index].ID >= 0)
          EffectManager.Instance.Stop(ref this.mSpellEffects[index]);
      }
      if (this.mEffects != null)
      {
        for (int index = 0; index < this.mEffects.Length; ++index)
        {
          if (index < this.mVisualEffects.Length && this.mVisualEffects[index].ID != -1)
            EffectManager.Instance.Stop(ref this.mVisualEffects[index]);
        }
      }
      if (this.mSounds != null)
      {
        for (int index = 0; index < this.mSoundCues.Length; ++index)
        {
          if (this.mSoundCues[index] != null && !this.mSoundCues[index].IsStopping)
            this.mSoundCues[index].Stop(AudioStopOptions.AsAuthored);
        }
      }
      if (this.mPointLightHolder.ContainsLight && this.mPointLightHolder.Enabled && this.mOwner != null)
      {
        this.mPointLightHolder.Enabled = false;
        if (this.mPointLight != null)
          this.mPointLight.Disable();
        this.mPointLight = (DynamicLight) null;
      }
    }
    if ((double) this.mSpecialAbilityCooldown > 0.0)
      (this.mRenderData[(int) iDataChannel] as Item.RenderData).EmissiveMultiplyer = System.Math.Min((float) System.Math.Pow(0.05, (double) this.mSpecialAbilityCooldown), 1f);
    if (this.mOwner != null)
    {
      switch (this.mPassiveAbility.Ability)
      {
        case Item.PassiveAbilities.AreaLifeDrain:
          this.mPassiveAbilityTimer -= iDeltaTime;
          if ((double) this.mPassiveAbilityTimer <= 0.0 && this.mOwner != null)
          {
            this.mPassiveAbilityTimer += 0.25f;
            List<Entity> entities = this.mPlayState.EntityManager.GetEntities(translation1, 7f, true);
            entities.Remove((Entity) this.mOwner);
            if (this.mPassiveAbility.Ability == Item.PassiveAbilities.AreaLifeDrain)
            {
              foreach (Entity entity in entities)
              {
                if (entity is Magicka.GameLogic.Entities.Character character && !character.Dead)
                {
                  character.Damage(4f, Elements.Arcane);
                  this.mOwner.Damage(-4f, Elements.Life);
                }
              }
            }
            this.mPlayState.EntityManager.ReturnEntityList(entities);
            break;
          }
          break;
        case Item.PassiveAbilities.EnhanceAllyMelee:
          List<Entity> entities1 = this.mPlayState.EntityManager.GetEntities(translation1, 7f, true);
          foreach (Entity entity in entities1)
          {
            if (entity is Magicka.GameLogic.Entities.Character character && (character.Faction & this.mOwner.Faction) != Factions.NONE)
              character.MeleeDamageBoost(1.5f);
          }
          this.mPlayState.EntityManager.ReturnEntityList(entities1);
          break;
        case Item.PassiveAbilities.AreaRegeneration:
          this.mPassiveAbilityTimer -= iDeltaTime;
          if ((double) this.mPassiveAbilityTimer <= 0.0 && this.mOwner != null)
          {
            this.mPassiveAbilityTimer = 0.25f;
            List<Entity> entities2 = this.mPlayState.EntityManager.GetEntities(translation1, 7f, true);
            foreach (Entity entity in entities2)
            {
              if (entity is Magicka.GameLogic.Entities.Character character && !character.Dead && (character.Faction & this.mOwner.Faction) != Factions.NONE)
                character.Damage(-2f, Elements.Life);
            }
            this.mPlayState.EntityManager.ReturnEntityList(entities2);
            break;
          }
          break;
        case Item.PassiveAbilities.Zap:
          if (this.mAttached)
          {
            if (this.mTeslaField == null)
            {
              Spell iSpell = new Spell();
              iSpell.Element = Elements.Lightning;
              iSpell[Elements.Lightning] = 1f;
              this.mTeslaField = TeslaField.GetFromCache(this.mPlayState);
              this.mTeslaField.Initialize(this.mOwner, iSpell);
              this.mTeslaField.ItemAbility = true;
            }
            this.mTeslaField.Update(iDataChannel, iDeltaTime);
            break;
          }
          if (this.mTeslaField != null)
          {
            this.mTeslaField.Deinitialize();
            break;
          }
          break;
        case Item.PassiveAbilities.BirchSteam:
          if (this.mAttached)
          {
            Vector3 right = Vector3.Right;
            if (EffectManager.Instance.IsActive(ref this.mPassiveAbilityEffect))
            {
              EffectManager.Instance.UpdatePositionDirection(ref this.mPassiveAbilityEffect, ref translation1, ref right);
              break;
            }
            EffectManager.Instance.StartEffect(Item.STEAM_EFFECT, ref translation1, ref right, out this.mPassiveAbilityEffect);
            break;
          }
          EffectManager.Instance.Stop(ref this.mPassiveAbilityEffect);
          break;
        case Item.PassiveAbilities.MoveSpeed:
          this.mOwner.CharacterBody.SpeedMultiplier *= this.mPassiveAbility.Variable;
          break;
        case Item.PassiveAbilities.Glow:
          List<Entity> entities3 = this.mPlayState.EntityManager.GetEntities(translation1, this.mPassiveAbility.Variable, true);
          entities3.Remove((Entity) this.mOwner);
          bool flag2 = false;
          foreach (Entity entity in entities3)
          {
            if (entity is Magicka.GameLogic.Entities.Character character && (character.Faction & this.Owner.Faction) == Factions.NONE)
            {
              flag2 = true;
              break;
            }
          }
          this.mPlayState.EntityManager.ReturnEntityList(entities3);
          this.mGlowTarget = flag2 ? 1f : 0.0f;
          this.mGlow += (float) (((double) this.mGlowTarget - (double) this.mGlow) * (double) iDeltaTime * 0.5);
          (this.mRenderData[(int) iDataChannel] as Item.RenderData).EmissiveMultiplyer = System.Math.Max(this.mGlow, 0.0f);
          break;
      }
    }
    if (this.IsGunClass && (double) this.mGunRateTimer > 0.0 && this.mOwner != null && this.mGunCurrentClip > 0)
    {
      this.mGunRateTimer -= iDeltaTime;
      if (this.mGunSound != null)
        this.mGunSound.Apply3D(this.mPlayState.Camera.Listener, this.mAudioEmitter);
      EffectManager.Instance.UpdateOrientation(ref this.mGunShellsEffect, ref this.mAttach1AbsoluteTransform);
      if ((double) this.mGunRateTimer <= 0.0)
      {
        if (this.mFiring)
        {
          this.mGunRateTimer += 1f / this.mGunRate;
          --this.mGunCurrentClip;
        }
        EffectManager.Instance.StartEffect(this.mGunMuzzleEffectID, ref this.mAttach0AbsoluteTransform, out VisualEffectReference _);
        if (this.mOwner.CurrentAnimation != Magicka.Animations.pickup_weapon)
        {
          Vector3 translation2 = this.mAttach0AbsoluteTransform.Translation;
          Vector3 result4 = this.mAttach1AbsoluteTransform.Translation;
          Vector3.Subtract(ref translation2, ref result4, out result4);
          result4.Normalize();
          float num2 = (float) ((1.0 - (double) this.mGunCurrentAccuracy) * 0.20000000298023224);
          float num3 = (float) System.Math.Atan2((double) result4.Z, (double) result4.X);
          float num4 = MagickaMath.RandomBetween(Item.sRandom, (float) (-(double) num2 * 0.5), num2 * 0.5f);
          float num5 = (float) System.Math.Sin((double) num3 + (double) num4);
          float num6 = (float) System.Math.Cos((double) num3 + (double) num4);
          result4.X = num6;
          result4.Z = num5;
          result4.Y += MagickaMath.RandomBetween(Item.sRandom, (float) (-(double) num2 * 2.0), num2 * 2f);
          result4.Normalize();
          this.FireShot(ref translation2, ref result4, 0.0f, this.mGunRange, (Entity) this.mOwner);
          ++this.mTracerCount;
        }
      }
    }
    if (this.mSpellQueue.Count > 0 && this.Model != null)
    {
      this.mSpellTime += iDeltaTime;
      Item.ElementRenderData iObject = this.mElementRenderData[(int) iDataChannel];
      iObject.mBoundingSphere = this.mRenderData[(int) iDataChannel].mBoundingSphere;
      iObject.Pos0 = this.mEffect0AbsolutePos;
      iObject.Pos1 = this.mEffect1AbsolutePos;
      iObject.Alpha = (float) (System.Math.Sin((double) this.mSpellTime * 2.0) * 0.25 + 0.75) * System.Math.Min(this.mSpellTime, 1f);
      iObject.Color = this.mSpellColor;
      this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, (IRenderableAdditiveObject) iObject);
    }
    else
      this.mSpellTime = 0.0f;
    if ((double) this.mDespawnTime > 0.0 && this.Model != null)
    {
      this.mDespawnTime -= iDeltaTime;
      Vector3 position = this.Position;
      Vector3 right = Vector3.Right;
      bool flag3 = EffectManager.Instance.UpdatePositionDirection(ref this.mDespawnEffect, ref position, ref right);
      if ((double) this.mDespawnTime <= 0.0)
      {
        EffectManager.Instance.StartEffect(BookOfMagick.DISAPPEAR_EFFECT, ref position, ref right, out VisualEffectReference _);
        if (NetworkManager.Instance.State != NetworkState.Client)
          this.Kill();
      }
      else if (!flag3 && (double) this.mDespawnTime <= 4.0)
        EffectManager.Instance.StartEffect(BookOfMagick.TIMEOUT_EFFECT, ref position, ref right, out this.mDespawnEffect);
    }
    if (!this.mAttached)
      return;
    this.mDespawnTime = 0.0f;
  }

  private void FireShot(
    ref Vector3 iOrigin,
    ref Vector3 iDir,
    float iDelay,
    float iRange,
    Entity iIgnored)
  {
    Segment segment;
    segment.Origin = iOrigin;
    Vector3.Multiply(ref iDir, iRange, out segment.Delta);
    Vector3 point;
    segment.GetPoint(0.5f, out point);
    float oFrac;
    Vector3 oPos1;
    Vector3 oNrm;
    int oPrim;
    bool flag1 = this.mPlayState.Level.CurrentScene.SegmentIntersect(out oFrac, out oPos1, out oNrm, out AnimatedLevelPart _, out oPrim, segment);
    if (flag1)
      Vector3.Multiply(ref segment.Delta, oFrac, out segment.Delta);
    List<Entity> entities = this.mPlayState.EntityManager.GetEntities(point, iRange * 0.5f, true);
    Entity entity1 = (Entity) null;
    float d = float.MaxValue;
    Vector3 end = new Vector3();
    for (int index = 0; index < entities.Count; ++index)
    {
      Entity entity2 = entities[index];
      if (entity2 != iIgnored)
      {
        IDamageable damageable = entities[index] as IDamageable;
        Portal.PortalEntity portalEntity = entities[index] as Portal.PortalEntity;
        if (damageable != null && !this.mHitlist.Contains(damageable) && !(entity2 is Grease.GreaseField) && !(entity2 is TornadoEntity))
        {
          Vector3 oPosition;
          if (damageable.SegmentIntersect(out oPosition, segment, 0.05f))
          {
            float result;
            Vector3.DistanceSquared(ref segment.Origin, ref oPosition, out result);
            if ((double) result < (double) d)
            {
              end = oPosition;
              d = result;
              entity1 = damageable as Entity;
            }
          }
        }
        else
        {
          Vector3 pos;
          if (portalEntity != null && Portal.Instance.Connected && portalEntity.Body.CollisionSkin.SegmentIntersect(out float _, out pos, out Vector3 _, segment))
          {
            float result;
            Vector3.DistanceSquared(ref segment.Origin, ref pos, out result);
            if ((double) result < (double) d)
            {
              end = pos;
              d = result;
              entity1 = (Entity) portalEntity;
            }
          }
        }
      }
    }
    this.mPlayState.EntityManager.ReturnEntityList(entities);
    if (entity1 != null)
    {
      Magicka.GameLogic.Entities.Character character = entity1 as Magicka.GameLogic.Entities.Character;
      IDamageable damageable = entity1 as IDamageable;
      if (entity1 is Portal.PortalEntity iPortalEntity)
      {
        float num = (float) System.Math.Sqrt((double) d);
        float iDelay1 = (num - 2f) / this.mTracerVelocity;
        Vector3 oPos2;
        iPortalEntity.GetOutPos(ref end, out oPos2);
        this.FireShot(ref oPos2, ref iDir, iDelay1, iRange - num, (Entity) Portal.OtherPortal(iPortalEntity));
      }
      else if (character != null && character.HasAura(AuraType.Deflect))
      {
        float num = (float) System.Math.Sqrt((double) d);
        float iDelay2 = (num - 2f) / this.mTracerVelocity;
        Vector3 position;
        if (!character.Body.CollisionSkin.SegmentIntersect(out float _, out Vector3 _, out position, segment))
        {
          position = character.Position;
          Vector3.Subtract(ref end, ref position, out position);
        }
        position.Y = 0.0f;
        Vector3 result;
        if ((double) position.LengthSquared() < 9.9999999747524271E-07)
        {
          result = iDir;
        }
        else
        {
          position.Normalize();
          Vector3.Reflect(ref iDir, ref position, out result);
        }
        this.FireShot(ref end, ref result, iDelay2, iRange - num, entity1);
      }
      else if (damageable != null)
      {
        bool flag2 = (double) damageable.HitPoints <= 0.0;
        DamageResult oDamageResult;
        this.mGunConditions.ExecuteAll((Entity) this, entity1, ref new EventCondition()
        {
          Position = new Vector3?(end),
          EventConditionType = EventConditionType.Hit
        }, out oDamageResult);
        if (this.mOwner is Avatar && !((this.mOwner as Avatar).Player.Gamer is NetworkGamer) && !flag2 && (oDamageResult & (DamageResult.Killed | DamageResult.OverKilled)) != DamageResult.None)
        {
          Profile.Instance.AddLedKill(this.PlayState);
          AchievementsManager.Instance.AwardAchievement(this.PlayState, "firstblood");
        }
        if (entity1 is PhysicsEntity)
        {
          Vector3 result;
          Vector3.Negate(ref iDir, out result);
          EffectManager.Instance.StartEffect((entity1 as PhysicsEntity).HitEffect, ref end, ref result, out VisualEffectReference _);
        }
      }
    }
    else if (flag1)
    {
      end = oPos1;
      this.mGunConditions.ExecuteAll((Entity) this, (Entity) null, ref new EventCondition()
      {
        Position = new Vector3?(oPos1),
        EventConditionType = EventConditionType.Collision
      }, out DamageResult _);
      if (oPrim >= 0)
        EffectManager.Instance.StartEffect(Item.GUN_HIT_EFFECTS[oPrim], ref oPos1, ref oNrm, out VisualEffectReference _);
    }
    else
      segment.GetEnd(out end);
    if (this.mTracerCount % 5U == 0U && (double) this.mTracerSprite >= 0.0)
    {
      TracerMan.Instance.AddTracer(ref segment.Origin, ref end, this.mTracerVelocity, 1f, (byte) this.mTracerSprite, iDelay);
    }
    else
    {
      if ((double) this.mNonTracerSprite < 0.0)
        return;
      TracerMan.Instance.AddTracer(ref segment.Origin, ref end, this.mTracerVelocity, 1f, (byte) this.mNonTracerSprite, iDelay);
    }
  }

  public static Item Read(ContentReader iInput)
  {
    Item iItem = new Item((PlayState) null, (Magicka.GameLogic.Entities.Character) null);
    iItem.mName = iInput.ReadString();
    iItem.mType = iItem.mName.GetHashCodeCustom();
    iItem.mAnimatedLevelPartID = 0;
    string iString1 = iInput.ReadString();
    iItem.mDisplayName = iString1.GetHashCodeCustom();
    iItem.mPickUpString = LanguageManager.Instance.GetString(Item.ITEM_PICKUP_LOC);
    if (!string.IsNullOrEmpty(iString1))
      iItem.mPickUpString = iItem.mPickUpString.Replace("#1;", $"[c=1,1,1]{LanguageManager.Instance.GetString(iItem.mDisplayName)}[/c]");
    iItem.mDescription = iInput.ReadString().GetHashCodeCustom();
    int length1 = iInput.ReadInt32();
    iItem.mSounds = new KeyValuePair<int, Banks>[length1];
    for (int index = 0; index < length1; ++index)
    {
      string str = iInput.ReadString();
      Banks banks = (Banks) iInput.ReadInt32();
      int hashCodeCustom = str.ToLowerInvariant().GetHashCodeCustom();
      iItem.mSounds[index] = new KeyValuePair<int, Banks>(hashCodeCustom, banks);
    }
    iItem.mPickable = iInput.ReadBoolean();
    iItem.mBound = iInput.ReadBoolean();
    iItem.mBlockValue = iInput.ReadInt32();
    iItem.mWeaponClass = (WeaponClass) iInput.ReadByte();
    iItem.mCooldownTimer = 0.0f;
    iItem.mCooldownTime = iInput.ReadSingle();
    iItem.mHideModel = iInput.ReadBoolean();
    iItem.mHideEffect = iInput.ReadBoolean();
    iItem.mPauseSounds = iInput.ReadBoolean();
    for (int iIndex = 0; iIndex < 11; ++iIndex)
    {
      Magicka.GameLogic.Entities.Resistance resistance;
      resistance.ResistanceAgainst = Spell.ElementFromIndex(iIndex);
      resistance.Multiplier = 1f;
      resistance.Modifier = 0.0f;
      resistance.StatusResistance = false;
      iItem.mResistances[iIndex] = resistance;
    }
    int num1 = iInput.ReadInt32();
    for (int index = 0; index < num1; ++index)
    {
      Magicka.GameLogic.Entities.Resistance resistance;
      resistance.ResistanceAgainst = (Elements) iInput.ReadInt32();
      resistance.Multiplier = iInput.ReadSingle();
      resistance.Modifier = iInput.ReadSingle();
      resistance.StatusResistance = iInput.ReadBoolean();
      iItem.mResistances[Spell.ElementIndex(resistance.ResistanceAgainst)] = resistance;
    }
    iItem.mPassiveAbility = new Item.PassiveAbilityStruct((Item.PassiveAbilities) iInput.ReadByte(), iInput.ReadSingle());
    int length2 = iInput.ReadInt32();
    iItem.mEffects = new int[length2];
    for (int index = 0; index < length2; ++index)
    {
      string str = iInput.ReadString();
      iItem.mEffects[index] = str.ToLowerInvariant().GetHashCodeCustom();
    }
    int num2 = iInput.ReadInt32();
    for (int index = 0; index < num2; ++index)
    {
      if (index != 0)
        throw new Exception("Items may Only have One Light!");
      iItem.mPointLightHolder.ContainsLight = true;
      iItem.mPointLightHolder.Radius = iInput.ReadSingle();
      iItem.mPointLightHolder.DiffuseColor = iInput.ReadVector3();
      iItem.mPointLightHolder.AmbientColor = iInput.ReadVector3();
      iItem.mPointLightHolder.SpecularAmount = iInput.ReadSingle();
      iItem.mPointLightHolder.VariationType = (LightVariationType) iInput.ReadByte();
      iItem.mPointLightHolder.VariationAmount = iInput.ReadSingle();
      iItem.mPointLightHolder.VariationSpeed = iInput.ReadSingle();
    }
    if (iInput.ReadBoolean())
    {
      iItem.mSpecialAbilityRechargeTime = iInput.ReadSingle();
      iItem.mSpecialAbility = Magicka.GameLogic.Entities.Abilities.SpecialAbilities.SpecialAbility.Read(iInput);
    }
    iItem.mMeleeRange = iInput.ReadSingle();
    iItem.mMeleeMultiHit = iInput.ReadBoolean();
    iItem.mMeleeConditions = new ConditionCollection(iInput);
    iItem.mRangedRange = iInput.ReadSingle();
    iItem.mFacing = iInput.ReadBoolean();
    iItem.mHoming = iInput.ReadSingle();
    iItem.mRangedElevation = iInput.ReadSingle();
    iItem.mRangedElevation = MathHelper.ToRadians(iItem.mRangedElevation);
    iItem.mRangedDanger = iInput.ReadSingle();
    iItem.mGunRange = iInput.ReadSingle();
    iItem.mGunClip = iInput.ReadInt32();
    iItem.mGunRate = (float) iInput.ReadInt32();
    iItem.mGunAccuracy = iInput.ReadSingle();
    string str1 = iInput.ReadString();
    iItem.mGunSoundBank = Banks.Weapons;
    if (string.IsNullOrEmpty(str1))
    {
      iItem.mGunSoundID = 0;
    }
    else
    {
      string[] strArray = str1.Split('/');
      if (strArray != null && strArray.Length > 1)
      {
        iItem.mGunSoundBank = (Banks) Enum.Parse(typeof (Banks), strArray[0], true);
        iItem.mGunSoundID = strArray[1].ToLower().GetHashCodeCustom();
      }
      else
      {
        iItem.mGunSoundBank = Banks.Weapons;
        iItem.mGunSoundID = str1.ToLower().GetHashCodeCustom();
      }
    }
    string iString2 = iInput.ReadString();
    iItem.mGunMuzzleEffectID = !string.IsNullOrEmpty(iString2) ? iString2.GetHashCodeCustom() : 0;
    string iString3 = iInput.ReadString();
    iItem.mGunShellsEffectID = !string.IsNullOrEmpty(iString3) ? iString3.GetHashCodeCustom() : 0;
    iItem.mTracerVelocity = iInput.ReadSingle();
    string str2 = iInput.ReadString();
    if (string.IsNullOrEmpty(str2))
    {
      iItem.mNonTracerSprite = -1f;
    }
    else
    {
      string[] strArray = str2.Split('/');
      if (strArray.Length > 1)
      {
        iItem.mNonTracerSprite = float.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
        if (strArray[0].Equals("B", StringComparison.InvariantCultureIgnoreCase))
          iItem.mNonTracerSprite += 64f;
        if (strArray[0].Equals("C", StringComparison.InvariantCultureIgnoreCase))
          iItem.mNonTracerSprite += 128f;
        if (strArray[0].Equals("D", StringComparison.InvariantCultureIgnoreCase))
          iItem.mNonTracerSprite += 192f;
        if (strArray[0].Equals("E", StringComparison.InvariantCultureIgnoreCase))
          iItem.mNonTracerSprite += 256f;
      }
      else
        iItem.mNonTracerSprite = float.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
    }
    string str3 = iInput.ReadString();
    if (string.IsNullOrEmpty(str3))
    {
      iItem.mTracerSprite = -1f;
    }
    else
    {
      string[] strArray = str3.Split('/');
      if (strArray.Length > 1)
      {
        iItem.mTracerSprite = float.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
        if (strArray[0].Equals("B", StringComparison.InvariantCultureIgnoreCase))
          iItem.mTracerSprite += 64f;
        if (strArray[0].Equals("C", StringComparison.InvariantCultureIgnoreCase))
          iItem.mTracerSprite += 128f;
        if (strArray[0].Equals("D", StringComparison.InvariantCultureIgnoreCase))
          iItem.mTracerSprite += 192f;
        if (strArray[0].Equals("E", StringComparison.InvariantCultureIgnoreCase))
          iItem.mTracerSprite += 256f;
      }
      else
        iItem.mTracerSprite = float.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
    }
    iItem.mGunConditions = new ConditionCollection(iInput);
    lock (Magicka.Game.Instance.GraphicsDevice)
      iItem.ProjectileModel = iInput.ReadExternalReference<Model>();
    iItem.mRangedConditions = new ConditionCollection(iInput);
    iItem.mScale = iInput.ReadSingle();
    lock (Magicka.Game.Instance.GraphicsDevice)
      iItem.Model = iInput.ReadExternalReference<Model>();
    iItem.mAttach0 = Matrix.Identity;
    iItem.mGlow = 1f;
    iItem.mGlowTarget = 1f;
    if (iItem.Model != null)
    {
      VertexElement[] vertexElements;
      lock (Magicka.Game.Instance.GraphicsDevice)
        vertexElements = iItem.Model.Meshes[0].MeshParts[0].VertexDeclaration.GetVertexElements();
      int offsetInBytes = -1;
      for (int index = 0; index < vertexElements.Length; ++index)
      {
        if (vertexElements[index].VertexElementUsage == VertexElementUsage.Position)
        {
          offsetInBytes = (int) vertexElements[index].Offset;
          break;
        }
      }
      if (offsetInBytes < 0)
        throw new Exception("No positions found");
      Vector3[] vector3Array = new Vector3[iItem.Model.Meshes[0].MeshParts[0].NumVertices];
      iItem.Model.Meshes[0].VertexBuffer.GetData<Vector3>(offsetInBytes, vector3Array, iItem.Model.Meshes[0].MeshParts[0].StartIndex, vector3Array.Length, iItem.Model.Meshes[0].MeshParts[0].VertexStride);
      iItem.mBoundingBox = BoundingBox.CreateFromPoints((IEnumerable<Vector3>) vector3Array);
      for (int index = 0; index < iItem.Model.Bones.Count; ++index)
      {
        if (iItem.Model.Bones[index].Name.Equals("attach0", StringComparison.OrdinalIgnoreCase))
        {
          ModelBone modelBone = iItem.Model.Bones[index];
          Matrix result = modelBone.Transform;
          while (modelBone.Parent != null)
          {
            modelBone = modelBone.Parent;
            Matrix transform = modelBone.Transform;
            Matrix.Multiply(ref transform, ref result, out result);
          }
          iItem.mAttach0 = result;
        }
        else if (iItem.Model.Bones[index].Name.Equals("attach1", StringComparison.OrdinalIgnoreCase))
        {
          ModelBone modelBone = iItem.Model.Bones[index];
          Matrix result = modelBone.Transform;
          while (modelBone.Parent != null)
          {
            modelBone = modelBone.Parent;
            Matrix transform = modelBone.Transform;
            Matrix.Multiply(ref transform, ref result, out result);
          }
          iItem.mAttach1 = result;
        }
        else if (iItem.Model.Bones[index].Name.Equals("effect0", StringComparison.OrdinalIgnoreCase))
        {
          ModelBone modelBone = iItem.Model.Bones[index];
          Matrix result = modelBone.Transform;
          while (modelBone.Parent != null)
          {
            modelBone = modelBone.Parent;
            Matrix transform = modelBone.Transform;
            Matrix.Multiply(ref transform, ref result, out result);
          }
          iItem.mEffect0Pos = result.Translation;
        }
        else if (iItem.Model.Bones[index].Name.Equals("effect1", StringComparison.OrdinalIgnoreCase))
        {
          ModelBone modelBone = iItem.Model.Bones[index];
          Matrix result = modelBone.Transform;
          while (modelBone.Parent != null)
          {
            modelBone = modelBone.Parent;
            Matrix transform = modelBone.Transform;
            Matrix.Multiply(ref transform, ref result, out result);
          }
          iItem.mEffect1Pos = result.Translation;
        }
      }
    }
    iItem.mIgnoreTractorPull = false;
    int capacity = iInput.ReadInt32();
    iItem.mAuras = new List<ActiveAura>(capacity);
    for (int index = 0; index < capacity; ++index)
      iItem.mAuras.Add(new ActiveAura()
      {
        Aura = new AuraStorage(iInput)
      });
    Item.CacheWeapon(iItem.mType, iItem);
    return iItem;
  }

  public static void Swap(Item iAItem, Item iBItem)
  {
    Helper.Swap<string>(ref iAItem.mName, ref iBItem.mName);
    Helper.Swap<int>(ref iAItem.mType, ref iBItem.mType);
    Helper.Swap<int>(ref iAItem.mDisplayName, ref iBItem.mDisplayName);
    Helper.Swap<int>(ref iAItem.mDescription, ref iBItem.mDescription);
    Helper.Swap<string>(ref iAItem.mPickUpString, ref iBItem.mPickUpString);
    Helper.Swap<bool>(ref iAItem.mPickable, ref iBItem.mPickable);
    Helper.Swap<bool>(ref iAItem.mBound, ref iBItem.mBound);
    Helper.Swap<WeaponClass>(ref iAItem.mWeaponClass, ref iBItem.mWeaponClass);
    Helper.Swap<int[]>(ref iAItem.mEffects, ref iBItem.mEffects);
    Helper.Swap<bool>(ref iAItem.mFacing, ref iBItem.mFacing);
    Helper.Swap<float>(ref iAItem.mDespawnTime, ref iBItem.mDespawnTime);
    Helper.Swap<float>(ref iAItem.mCooldownTimer, ref iBItem.mCooldownTimer);
    Helper.Swap<float>(ref iAItem.mCooldownTime, ref iBItem.mCooldownTime);
    Helper.Swap<bool>(ref iAItem.mHideModel, ref iBItem.mHideModel);
    Helper.Swap<bool>(ref iAItem.mHideEffect, ref iBItem.mHideEffect);
    Helper.Swap<bool>(ref iAItem.mPauseSounds, ref iBItem.mPauseSounds);
    Helper.Swap<bool>(ref iAItem.mIgnoreTractorPull, ref iBItem.mIgnoreTractorPull);
    AnimatedLevelPart animatedLevelPart1;
    if (iAItem.mAnimatedLevelPartID != 0 && iAItem.mPlayState.Level.CurrentScene.LevelModel.AnimatedLevelParts.TryGetValue(iAItem.mAnimatedLevelPartID, out animatedLevelPart1))
    {
      animatedLevelPart1.RemoveEntity((Entity) iAItem);
      iAItem.AnimatedLevelPartID = 0;
    }
    AnimatedLevelPart animatedLevelPart2;
    if (iBItem.mAnimatedLevelPartID != 0 && iBItem.mPlayState.Level.CurrentScene.LevelModel.AnimatedLevelParts.TryGetValue(iBItem.mAnimatedLevelPartID, out animatedLevelPart2))
    {
      animatedLevelPart2.RemoveEntity((Entity) iBItem);
      iBItem.AnimatedLevelPartID = 0;
    }
    EffectManager.Instance.Stop(ref iAItem.mDespawnEffect);
    EffectManager.Instance.Stop(ref iBItem.mDespawnEffect);
    for (int index = 0; index < iAItem.mVisualEffects.Length; ++index)
    {
      if (iAItem.mVisualEffects[index].Hash != 0 && iAItem.mVisualEffects[index].ID >= 0)
        EffectManager.Instance.Stop(ref iAItem.mVisualEffects[index]);
    }
    for (int index = 0; index < iBItem.mVisualEffects.Length; ++index)
    {
      if (iBItem.mVisualEffects[index].Hash != 0 && iBItem.mVisualEffects[index].ID >= 0)
        EffectManager.Instance.Stop(ref iBItem.mVisualEffects[index]);
    }
    Helper.Swap<KeyValuePair<int, Banks>[]>(ref iAItem.mSounds, ref iBItem.mSounds);
    for (int index = 0; index < iAItem.mSoundCues.Length; ++index)
    {
      if (iAItem.mSoundCues[index] != null && !iAItem.mSoundCues[index].IsStopping)
        iAItem.mSoundCues[index].Stop(AudioStopOptions.AsAuthored);
    }
    for (int index = 0; index < iBItem.mSoundCues.Length; ++index)
    {
      if (iBItem.mSoundCues[index] != null && !iBItem.mSoundCues[index].IsStopping)
        iBItem.mSoundCues[index].Stop(AudioStopOptions.AsAuthored);
    }
    if (iAItem.mGunSound != null && !iAItem.mGunSound.IsStopping)
    {
      iAItem.mGunSound.Stop(AudioStopOptions.AsAuthored);
      iAItem.mGunSound = (Cue) null;
    }
    EffectManager.Instance.Stop(ref iAItem.mGunShellsEffect);
    if (iBItem.mGunSound != null && !iBItem.mGunSound.IsStopping)
    {
      iBItem.mGunSound.Stop(AudioStopOptions.AsAuthored);
      iBItem.mGunSound = (Cue) null;
    }
    EffectManager.Instance.Stop(ref iBItem.mGunShellsEffect);
    Helper.Swap<Item.PointLightHolder>(ref iAItem.mPointLightHolder, ref iBItem.mPointLightHolder);
    if (iAItem.mPointLight != null)
    {
      iAItem.mPointLight.Disable();
      iAItem.mPointLight = (DynamicLight) null;
    }
    if (iBItem.mPointLight != null)
    {
      iBItem.mPointLight.Disable();
      iBItem.mPointLight = (DynamicLight) null;
    }
    iAItem.mPointLightHolder.Enabled = false;
    iBItem.mPointLightHolder.Enabled = false;
    Model model = iBItem.Model;
    iBItem.Model = iAItem.Model;
    iAItem.Model = model;
    Helper.Swap<Model>(ref iAItem.mProjectileModel, ref iBItem.mProjectileModel);
    Helper.Swap<float>(ref iAItem.mMeleeRange, ref iBItem.mMeleeRange);
    Helper.Swap<ConditionCollection>(ref iAItem.mMeleeConditions, ref iBItem.mMeleeConditions);
    Helper.Swap<bool>(ref iAItem.mMeleeMultiHit, ref iBItem.mMeleeMultiHit);
    Helper.Swap<ConditionCollection>(ref iAItem.mGunConditions, ref iBItem.mGunConditions);
    Helper.Swap<float>(ref iAItem.mRangedRange, ref iBItem.mRangedRange);
    Helper.Swap<ConditionCollection>(ref iAItem.mRangedConditions, ref iBItem.mRangedConditions);
    iAItem.mSpellQueue.Clear();
    iBItem.mSpellQueue.Clear();
    Helper.Swap<VisualEffectReference[]>(ref iAItem.mSpellEffects, ref iBItem.mSpellEffects);
    Helper.Swap<VisualEffectReference[]>(ref iAItem.mVisualEffects, ref iBItem.mVisualEffects);
    Helper.Swap<Matrix>(ref iAItem.mAttach0, ref iBItem.mAttach0);
    Helper.Swap<Matrix>(ref iAItem.mAttach1, ref iBItem.mAttach1);
    Helper.Swap<Vector3>(ref iAItem.mEffect0Pos, ref iBItem.mEffect0Pos);
    Helper.Swap<Vector3>(ref iAItem.mEffect1Pos, ref iBItem.mEffect1Pos);
    Helper.Swap<BoundingBox>(ref iAItem.mBoundingBox, ref iBItem.mBoundingBox);
    Helper.Swap<int>(ref iAItem.mBlockValue, ref iBItem.mBlockValue);
    Helper.Swap<float>(ref iAItem.mRangedDanger, ref iBItem.mRangedDanger);
    Helper.Swap<float>(ref iAItem.mRangedElevation, ref iBItem.mRangedElevation);
    Helper.Swap<float>(ref iAItem.mHoming, ref iBItem.mHoming);
    Helper.Swap<float>(ref iAItem.mGunRange, ref iBItem.mGunRange);
    Helper.Swap<int>(ref iAItem.mGunClip, ref iBItem.mGunClip);
    Helper.Swap<float>(ref iAItem.mGunRate, ref iBItem.mGunRate);
    Helper.Swap<float>(ref iAItem.mGunAccuracy, ref iBItem.mGunAccuracy);
    Helper.Swap<int>(ref iAItem.mGunSoundID, ref iBItem.mGunSoundID);
    Helper.Swap<Banks>(ref iAItem.mGunSoundBank, ref iBItem.mGunSoundBank);
    Helper.Swap<int>(ref iAItem.mGunMuzzleEffectID, ref iBItem.mGunMuzzleEffectID);
    Helper.Swap<int>(ref iAItem.mGunShellsEffectID, ref iBItem.mGunShellsEffectID);
    Helper.Swap<uint>(ref iAItem.mTracerCount, ref iBItem.mTracerCount);
    Helper.Swap<float>(ref iAItem.mTracerSprite, ref iBItem.mTracerSprite);
    Helper.Swap<float>(ref iAItem.mNonTracerSprite, ref iBItem.mNonTracerSprite);
    Helper.Swap<float>(ref iAItem.mTracerVelocity, ref iBItem.mTracerVelocity);
    Helper.Swap<float>(ref iAItem.mSpecialAbilityCooldown, ref iBItem.mSpecialAbilityCooldown);
    Helper.Swap<float>(ref iAItem.mSpecialAbilityRechargeTime, ref iBItem.mSpecialAbilityRechargeTime);
    Helper.Swap<Magicka.GameLogic.Entities.Abilities.SpecialAbilities.SpecialAbility>(ref iAItem.mSpecialAbility, ref iBItem.mSpecialAbility);
    Helper.Swap<Item.PassiveAbilityStruct>(ref iAItem.mPassiveAbility, ref iBItem.mPassiveAbility);
    Helper.Swap<TeslaField>(ref iAItem.mTeslaField, ref iBItem.mTeslaField);
    Helper.Swap<float>(ref iAItem.mGlow, ref iBItem.mGlow);
    Helper.Swap<float>(ref iAItem.mGlowTarget, ref iBItem.mGlowTarget);
    Helper.Swap<MissileEntity>(ref iAItem.mGungnirMissile, ref iBItem.mGungnirMissile);
    Helper.Swap<Magicka.GameLogic.Entities.Resistance[]>(ref iAItem.mResistances, ref iBItem.mResistances);
    for (int index = 0; index < iAItem.mAuras.Count; ++index)
    {
      VisualEffectReference mEffect = iAItem.mAuras[index].mEffect;
      EffectManager.Instance.Stop(ref mEffect);
    }
    for (int index = 0; index < iBItem.mAuras.Count; ++index)
    {
      VisualEffectReference mEffect = iBItem.mAuras[index].mEffect;
      EffectManager.Instance.Stop(ref mEffect);
    }
    Helper.Swap<List<ActiveAura>>(ref iAItem.mAuras, ref iBItem.mAuras);
    if (iAItem.mOwner == null)
      return;
    for (int index = 0; index < iAItem.mAuras.Count; ++index)
    {
      VisualEffectReference oRef = new VisualEffectReference();
      ActiveAura mAura = iAItem.mAuras[index];
      if (iAItem.mAuras[index].Aura.Effect != 0)
      {
        int effect = iAItem.mAuras[index].Aura.Effect;
        Vector3 position = iAItem.mOwner.Position;
        Vector3 direction = iAItem.mOwner.Direction;
        EffectManager.Instance.StartEffect(effect, ref position, ref direction, out oRef);
      }
      mAura.mEffect = oRef;
      iAItem.mAuras[index] = mAura;
    }
  }

  public void Copy(Item iCopy)
  {
    iCopy.mName = this.mName;
    iCopy.mType = this.mType;
    iCopy.mDescription = this.mDescription;
    iCopy.mDisplayName = this.mDisplayName;
    iCopy.mPickUpString = this.mPickUpString;
    iCopy.mPickable = this.mPickable;
    iCopy.mBound = this.mBound;
    iCopy.mWeaponClass = this.mWeaponClass;
    iCopy.mEffects = this.mEffects;
    iCopy.mGunConditions = this.mGunConditions;
    iCopy.Model = this.Model;
    iCopy.ProjectileModel = this.mProjectileModel;
    iCopy.mMeleeRange = this.mMeleeRange;
    iCopy.mMeleeConditions = this.mMeleeConditions;
    iCopy.mMeleeMultiHit = this.mMeleeMultiHit;
    iCopy.mRangedRange = this.mRangedRange;
    iCopy.mRangedConditions = this.mRangedConditions;
    iCopy.mHoming = this.mHoming;
    iCopy.mFacing = this.mFacing;
    iCopy.mRangedElevation = this.mRangedElevation;
    iCopy.mRangedDanger = this.mRangedDanger;
    iCopy.PreviousOwner = this.mPreviousOwner;
    iCopy.mIgnoreTractorPull = this.mIgnoreTractorPull;
    iCopy.mAnimatedLevelPartID = this.mAnimatedLevelPartID;
    iCopy.mGunRange = this.mGunRange;
    iCopy.mGunClip = this.mGunClip;
    iCopy.mGunRate = this.mGunRate;
    iCopy.mGunAccuracy = this.mGunAccuracy;
    iCopy.mGunSoundBank = this.mGunSoundBank;
    iCopy.mGunSoundID = this.mGunSoundID;
    iCopy.mGunMuzzleEffectID = this.mGunMuzzleEffectID;
    iCopy.mGunShellsEffectID = this.mGunShellsEffectID;
    iCopy.mNonTracerSprite = this.mNonTracerSprite;
    iCopy.mTracerSprite = this.mTracerSprite;
    iCopy.mTracerVelocity = this.mTracerVelocity;
    iCopy.mTracerCount = this.mTracerCount;
    iCopy.mCooldownTimer = this.mCooldownTimer = 0.0f;
    iCopy.mCooldownTime = this.mCooldownTime;
    iCopy.mHideModel = this.mHideModel;
    iCopy.mHideEffect = this.mHideEffect;
    iCopy.mPauseSounds = this.mPauseSounds;
    iCopy.mSpellQueue.Clear();
    for (int iIndex = 0; iIndex < this.mSpellQueue.Count; ++iIndex)
      iCopy.mSpellQueue.Add(this.mSpellQueue[iIndex]);
    this.mSpellEffects.CopyTo((Array) iCopy.mSpellEffects, 0);
    iCopy.mSounds = this.mSounds;
    iCopy.mSoundCues = this.mSoundCues;
    if (this.mPointLight != null)
    {
      this.mPointLight.Disable();
      this.mPointLight = (DynamicLight) null;
      this.mPointLightHolder.Enabled = false;
    }
    if (iCopy.mPointLight != null)
    {
      iCopy.mPointLight.Disable();
      iCopy.mPointLight = (DynamicLight) null;
      iCopy.mPointLightHolder.Enabled = false;
    }
    iCopy.mPointLightHolder = this.mPointLightHolder;
    iCopy.mAttach0 = this.mAttach0;
    iCopy.mAttach1 = this.mAttach1;
    iCopy.mEffect0Pos = this.mEffect0Pos;
    iCopy.mEffect1Pos = this.mEffect1Pos;
    iCopy.mBoundingBox = this.mBoundingBox;
    iCopy.mBlockValue = this.mBlockValue;
    this.mResistances.CopyTo((Array) iCopy.mResistances, 0);
    iCopy.mSpecialAbilityCooldown = this.mSpecialAbilityCooldown;
    iCopy.mSpecialAbilityRechargeTime = this.mSpecialAbilityRechargeTime;
    iCopy.mSpecialAbility = this.mSpecialAbility;
    iCopy.mPassiveAbility = this.mPassiveAbility;
    iCopy.mTeslaField = this.mTeslaField;
    iCopy.mGlow = this.mGlow;
    iCopy.mGlowTarget = this.mGlowTarget;
    iCopy.mGungnirMissile = this.mGungnirMissile;
    if (iCopy.mBody != null && this.Model != null)
    {
      Vector3 result;
      Vector3.Subtract(ref this.mBoundingBox.Max, ref this.mBoundingBox.Min, out result);
      (iCopy.mCollision.GetPrimitiveLocal(0) as Box).SideLengths = result;
      (iCopy.mCollision.GetPrimitiveOldWorld(0) as Box).SideLengths = result;
      (iCopy.mCollision.GetPrimitiveNewWorld(0) as Box).SideLengths = result;
      Vector3 vector3_1 = (this.mBoundingBox.Min + this.mBoundingBox.Max) * 0.5f;
      Vector3 vector3_2 = iCopy.SetMass(50f);
      JigLibX.Math.Transform transform = new JigLibX.Math.Transform();
      Vector3.Negate(ref vector3_2, out transform.Position);
      Vector3.Add(ref transform.Position, ref vector3_1, out transform.Position);
      transform.Orientation = Matrix.Identity;
      iCopy.mCollision.ApplyLocalTransform(transform);
      iCopy.mBody.SetOrientation(this.Transform);
    }
    iCopy.mAuras.Clear();
    for (int index = 0; index < this.mAuras.Count; ++index)
      iCopy.mAuras.Add(this.mAuras[index]);
  }

  internal bool IgnoreTractorPull
  {
    get => this.mIgnoreTractorPull;
    set => this.mIgnoreTractorPull = value;
  }

  internal int AnimatedLevelPartID
  {
    get => this.mAnimatedLevelPartID;
    set => this.mAnimatedLevelPartID = value;
  }

  public void Despawn(float iTime)
  {
    this.mDespawnTime = iTime;
    EffectManager.Instance.Stop(ref this.mDespawnEffect);
  }

  public float DespawnTime => this.mDespawnTime;

  public string Name => this.mName;

  public string PickUpString => this.mPickUpString;

  public bool AnimationDetached => this.mAnimationDetached;

  public void AnimationDetach() => this.mAnimationDetached = true;

  public bool Attached => this.mAttached;

  public bool SpecialAbilityReady
  {
    get => (double) this.mSpecialAbilityCooldown <= 0.0 && this.mSpecialAbility != null;
  }

  public bool CooldownHintTime
  {
    get
    {
      return (double) this.mSpecialAbilityCooldown < 0.0 && (double) this.mSpecialAbilityCooldown > -2.0;
    }
  }

  public bool IsCoolingdown => (double) this.mCooldownTimer > 0.0;

  public Spell PeekSpell() => SpellManager.Instance.Combine(this.mSpellQueue);

  public Spell RetrieveSpell()
  {
    Spell spell = SpellManager.Instance.Combine(this.mSpellQueue);
    this.mSpellQueue.Clear();
    for (int index = 0; index < this.mSpellEffects.Length; ++index)
      EffectManager.Instance.Stop(ref this.mSpellEffects[index]);
    return spell;
  }

  public Item.PassiveAbilityStruct PassiveAbility => this.mPassiveAbility;

  public bool IsBound => this.mBound;

  public bool TryAddToQueue(ref Spell iSpell, bool iLightningPrecedence)
  {
    int num = this.mSpellQueue.Count + 1;
    if (this.mSpellQueue.Count < 5)
    {
      this.mSpellQueue.Add(iSpell.Normalize());
      SpellManager.FindOppositesAndCombinables((Magicka.GameLogic.Player) null, this.mOwner, this.mSpellQueue);
    }
    else
    {
      int iIndex = SpellManager.FindOpposites(this.mSpellQueue, 4, iSpell.Element);
      if (iIndex >= 0)
      {
        this.mSpellQueue.RemoveAt(iIndex);
      }
      else
      {
        iIndex = SpellManager.FindRevertables(this.mSpellQueue, 4, iSpell.Element);
        if (iIndex >= 0)
        {
          Spell spell = iSpell + this.mSpellQueue[iIndex];
          this.mSpellQueue[iIndex] = spell;
        }
        else
        {
          iIndex = SpellManager.FindCombines(this.mSpellQueue, 4, iSpell.Element);
          if (iIndex >= 0)
          {
            Spell spell = iSpell + this.mSpellQueue[iIndex];
            this.mSpellQueue[iIndex] = spell;
          }
        }
      }
      if (iLightningPrecedence && iIndex == -1)
      {
        int differentElement = SpellManager.FindDifferentElement(this.mSpellQueue, 4, Elements.Lightning);
        if (differentElement >= 0)
        {
          Spell oSpell;
          Spell.DefaultSpell(Elements.Lightning, out oSpell);
          this.mSpellQueue[differentElement] = oSpell;
        }
      }
    }
    if (num == this.mSpellQueue.Count)
    {
      for (int iIndex = 0; iIndex < 11; ++iIndex)
      {
        Elements elements = Defines.ElementFromIndex(iIndex);
        if ((iSpell.Element & elements) == elements)
        {
          EffectManager.Instance.Stop(ref this.mSpellEffects[iIndex]);
          Vector3 position = this.Position;
          Vector3 forward = this.mBody.Orientation.Forward;
          EffectManager.Instance.StartEffect(Item.ChantEffects[iIndex], ref position, ref forward, out this.mSpellEffects[iIndex]);
        }
      }
      this.mSpellColor = this.PeekSpell().GetColor();
      return true;
    }
    for (int iIndex = 0; iIndex < 11; ++iIndex)
    {
      Elements elements = Defines.ElementFromIndex(iIndex);
      if ((iSpell.Element & elements) == elements)
        EffectManager.Instance.Stop(ref this.mSpellEffects[iIndex]);
    }
    this.mSpellColor = this.PeekSpell().GetColor();
    return false;
  }

  public void StopEffects()
  {
    for (int index = 0; index < this.mSpellEffects.Length; ++index)
      EffectManager.Instance.Stop(ref this.mSpellEffects[index]);
    EffectManager.Instance.Stop(ref this.mPassiveAbilityEffect);
    for (int index = 0; index < this.mAuras.Count; ++index)
    {
      VisualEffectReference mEffect = this.mAuras[index].mEffect;
      EffectManager.Instance.Stop(ref mEffect);
    }
    for (int index = 0; index < this.mFollowEffects.Length; ++index)
      EffectManager.Instance.Stop(ref this.mFollowEffects[index]);
  }

  public bool SpellCharged => this.mSpellQueue.Count > 0;

  public StaticList<Spell> SpellList => this.mSpellQueue;

  public void ClearHitlist()
  {
    this.mHitlist.Clear();
    this.mNextToDamage = 0;
  }

  public Model ProjectileModel
  {
    get => this.mProjectileModel;
    set
    {
      if (value == null)
        return;
      this.mProjectileModel = value;
    }
  }

  public override Vector3 CalcImpulseVelocity(
    Vector3 iDirection,
    float iElevation,
    float iMassPower,
    float iDistance)
  {
    return !this.mBody.IsBodyEnabled ? new Vector3() : base.CalcImpulseVelocity(iDirection, iElevation, iMassPower, iDistance);
  }

  public int BlockValue => this.mBlockValue;

  public override bool Dead => this.Model == null || this.mAttached;

  public override bool Removable => this.Dead;

  public override void Deinitialize()
  {
    if (this.mBody != null)
      this.mBody.SetActive();
    if (!this.mAnimationDetached)
      this.mTimeToRemove = 15f;
    this.mAttached = true;
    this.mSpellQueue.Clear();
    this.StopEffects();
    for (int index = 0; index < this.mSoundCues.Length; ++index)
    {
      if (this.mSoundCues[index] != null && !this.mSoundCues[index].IsStopping)
        this.mSoundCues[index].Stop(AudioStopOptions.AsAuthored);
    }
    if (this.mEffects != null)
    {
      for (int index = 0; index < this.mEffects.Length; ++index)
      {
        if (index < this.mVisualEffects.Length)
        {
          EffectManager.Instance.Stop(ref this.mVisualEffects[index]);
          this.mVisualEffects[index].ID = -1;
        }
      }
    }
    for (int index = 0; index < this.mFollowEffects.Length; ++index)
      EffectManager.Instance.Stop(ref this.mFollowEffects[index]);
    EffectManager.Instance.Stop(ref this.mGunShellsEffect);
    if (this.mPointLight != null)
    {
      this.mPointLight.Disable();
      this.mPointLight = (DynamicLight) null;
      this.mPointLightHolder.Enabled = false;
    }
    base.Deinitialize();
  }

  public Matrix AttachAbsoluteTransform => this.mAttach0AbsoluteTransform;

  public Magicka.GameLogic.Entities.Character Owner
  {
    get => this.mOwner;
    set => this.mOwner = value;
  }

  public bool IsGunClass
  {
    get
    {
      return this.mWeaponClass == WeaponClass.Handgun | this.mWeaponClass == WeaponClass.Rifle | this.mWeaponClass == WeaponClass.Machinegun | this.mWeaponClass == WeaponClass.Heavy;
    }
  }

  public WeaponClass WeaponClass => this.mWeaponClass;

  public List<ActiveAura> Auras => this.mAuras;

  public void PrepareToExecute(Ability iAb) => this.mSourceAbility = iAb;

  public int SpecialAbilityName => this.mSpecialAbility.DisplayName;

  public bool HasSpecialAbility => this.mSpecialAbility != null;

  public void ExecuteSpecialAbility()
  {
    if ((double) this.mSpecialAbilityCooldown > 0.0 || this.mSpecialAbility == null)
      return;
    this.mOwner.SpecialAbilityAnimation(this.mSpecialAbility.Animation);
    this.mSpecialAbility.Execute((ISpellCaster) this.mOwner, this.mPlayState);
    if (this.mSpecialAbility is Invisibility)
      this.mOwner.JustCastInvisible = true;
    this.mSpecialAbilityCooldown = this.mSpecialAbilityRechargeTime;
  }

  public void AddEffectReference(int iEffectHash, VisualEffectReference iEffect)
  {
    for (int index = 0; index < this.mFollowEffects.Length; ++index)
    {
      if (!EffectManager.Instance.IsActive(ref this.mFollowEffects[index]))
      {
        this.mFollowEffects[index] = iEffect;
        return;
      }
    }
    EffectManager.Instance.Stop(ref iEffect);
  }

  public void StopExecute()
  {
    for (int index = 0; index < this.mFollowEffects.Length; ++index)
    {
      if (this.mFollowEffects[index].ID != -1)
        EffectManager.Instance.Stop(ref this.mFollowEffects[index]);
    }
  }

  public void StopGunfire()
  {
    if (!this.mFiring || this.mOwner is Avatar && (!(this.mOwner is Avatar) || (this.mOwner.Attacking || this.mWeaponClass != WeaponClass.Machinegun) && !(this.mWeaponClass == WeaponClass.Rifle | this.mWeaponClass == WeaponClass.Handgun)))
      return;
    this.mGunCurrentClip = 0;
    this.mGunRateTimer = 0.0f;
    this.mFiring = false;
    if (this.mGunSound != null && !this.mGunSound.IsStopping)
      this.mGunSound.Stop(AudioStopOptions.AsAuthored);
    EffectManager.Instance.Stop(ref this.mGunShellsEffect);
  }

  public void ExecuteGun(float iAccuracy)
  {
    if (this.mGunConditions[0] == null && this.mGunConditions[1] == null && this.mGunConditions[2] == null && this.mGunConditions[3] == null && this.mGunConditions[4] == null)
      return;
    this.mGunCurrentAccuracy = this.mGunAccuracy * iAccuracy;
    this.mGunCurrentClip = this.mGunClip;
    if ((double) this.mGunRateTimer <= 0.0)
    {
      this.mGunRateTimer = float.Epsilon;
      this.mGunSound = AudioManager.Instance.PlayCue(this.mGunSoundBank, this.mGunSoundID, this.mAudioEmitter);
      EffectManager.Instance.StartEffect(this.mGunShellsEffectID, ref this.mAttach1AbsoluteTransform, out this.mGunShellsEffect);
    }
    this.mFiring = true;
    this.mCooldownTimer = this.mCooldownTime;
  }

  public virtual void Execute(DealDamage.Targets iTargets)
  {
    if (this.mMeleeConditions[0] == null && this.mMeleeConditions[1] == null && this.mMeleeConditions[2] == null && this.mMeleeConditions[3] == null && this.mMeleeConditions[4] == null)
      return;
    IDamageable damageable1 = (IDamageable) null;
    if (this.mOwner is NonPlayerCharacter)
      damageable1 = (this.mOwner as NonPlayerCharacter).AI.CurrentTarget;
    if (this.mType == Item.TYRFING_TYPE & (double) this.mTeleportCooldown <= 0.0)
    {
      this.mTeleportCooldown = 0.5f;
      List<Entity> entities = this.mOwner.PlayState.EntityManager.GetEntities(this.mOwner.Position, 20f, false);
      float num1 = 1.57079637f;
      float num2 = float.MaxValue;
      Magicka.GameLogic.Entities.Character character = (Magicka.GameLogic.Entities.Character) null;
      Vector3 direction = this.mOwner.Direction;
      for (int index = 0; index < entities.Count; ++index)
      {
        if (entities[index] is Magicka.GameLogic.Entities.Character && entities[index] != this.mOwner)
        {
          Vector3 vector = entities[index].Position - this.mOwner.Position;
          float num3 = vector.Length();
          vector.Normalize();
          float num4 = MagickaMath.Angle(ref vector, ref direction);
          if ((double) num4 < (double) num1 && (double) num4 < 0.39269909262657166 && (double) num3 < (double) num2)
          {
            num1 = num4;
            num2 = num3;
            character = entities[index] as Magicka.GameLogic.Entities.Character;
          }
        }
      }
      if (character != null && (double) num2 > (double) this.mOwner.Radius + 3.0 + (double) character.Radius && NetworkManager.Instance.State != NetworkState.Client)
      {
        Vector3 vector3 = (this.mOwner.Position - character.Position) with
        {
          Y = 0.0f
        };
        vector3.Normalize();
        Vector3 iPosition = vector3 * (float) ((double) character.Radius + (double) this.mOwner.Radius + 0.20000000298023224) + character.Position;
        Vector3 iDirection = (character.Position - iPosition) with
        {
          Y = 0.0f
        };
        iDirection.Normalize();
        if (NetworkManager.Instance.State == NetworkState.Server)
          NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
          {
            Handle = this.mOwner.Handle,
            Param0F = iPosition.X,
            Param1F = iPosition.Y,
            Param2F = iPosition.Z,
            Param4I = 1,
            TargetHandle = character.Handle,
            Action = ActionType.Magick,
            Param3I = 5
          });
        Teleport.Instance.DoTeleport((ISpellCaster) this.mOwner, iPosition, iDirection, Teleport.TeleportType.Regular);
      }
      this.mOwner.PlayState.EntityManager.ReturnEntityList(entities);
    }
    this.mContinueHitting = this.mHitlist.Count == 0;
    if (!this.mMeleeMultiHit && !this.mContinueHitting || (double) this.mShieldDeflectCoolDown >= 0.0)
      return;
    this.mCooldownTimer = this.mCooldownTime;
    Segment iSeg1 = new Segment();
    iSeg1.Origin = this.mAttach0AbsoluteTransform.Translation;
    Vector3.Subtract(ref this.mLastAttachAbsolutePosition, ref iSeg1.Origin, out iSeg1.Delta);
    List<Shield> shields = this.mPlayState.EntityManager.Shields;
    Segment iSeg2 = new Segment();
    iSeg2.Origin = this.mOwner.Position;
    Vector3.Subtract(ref iSeg1.Origin, ref iSeg2.Origin, out iSeg2.Delta);
    for (int index = 0; index < shields.Count; ++index)
    {
      Shield iTarget = shields[index];
      Vector3 oPosition;
      if (iTarget.SegmentIntersect(out oPosition, iSeg1, this.mMeleeRange) || iTarget.SegmentIntersect(out oPosition, iSeg2, this.mMeleeRange))
      {
        DamageResult oDamageResult;
        this.mMeleeConditions.ExecuteAll((Entity) this.mOwner, (Entity) iTarget, ref new EventCondition()
        {
          EventConditionType = EventConditionType.Hit
        }, out oDamageResult);
        DamageResult damageResult = DamageResult.Damaged | DamageResult.Hit | DamageResult.Killed;
        if (this.mPassiveAbility.Ability == Item.PassiveAbilities.Mjolnr && (oDamageResult & damageResult) != DamageResult.None)
          new Magick() { MagickType = MagickType.ThunderB }.Effect.Execute((ISpellCaster) this.mOwner, this.mPlayState);
        this.mShieldDeflectCoolDown = 0.25f;
        this.mOwner.GoToAnimation(Magicka.Animations.attack_recoil, 0.1f);
        return;
      }
    }
    List<Entity> entities1 = this.mPlayState.EntityManager.GetEntities(this.mAttach0AbsoluteTransform.Translation, this.mMeleeRange + 5f, true);
    for (int index = 0; index < entities1.Count; ++index)
    {
      IDamageable damageable2 = entities1[index] as IDamageable;
      if (damageable2 != this.mOwner && damageable2 != null && !this.mHitlist.Contains(damageable2))
      {
        if (damageable2 != damageable1)
        {
          if (damageable2 is Magicka.GameLogic.Entities.Character)
          {
            bool flag = ((damageable2 as Magicka.GameLogic.Entities.Character).Faction & this.mOwner.Faction) != Factions.NONE;
            if (flag && (iTargets & DealDamage.Targets.Friendly) == DealDamage.Targets.None || !flag && (iTargets & DealDamage.Targets.Enemy) == DealDamage.Targets.None)
              continue;
          }
          else if ((iTargets & DealDamage.Targets.NonCharacters) == DealDamage.Targets.None)
            continue;
        }
        if (damageable2.SegmentIntersect(out Vector3 _, iSeg1, this.mMeleeRange))
        {
          this.mHitlist.Add(damageable2);
          if (!this.mMeleeMultiHit)
            break;
        }
      }
    }
    this.mPlayState.EntityManager.ReturnEntityList(entities1);
    if ((double) this.mWorldCollisionTimer <= 0.0)
    {
      for (int iIndex = 0; iIndex < this.mMeleeConditions.Count; ++iIndex)
      {
        if (this.mMeleeConditions[iIndex] != null && this.mMeleeConditions[iIndex].Condition.EventConditionType == EventConditionType.Collision)
        {
          if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out Vector3 _, out Vector3 _, out AnimatedLevelPart _, iSeg1))
          {
            this.mMeleeConditions.ExecuteAll((Entity) this, (Entity) null, ref new EventCondition()
            {
              EventConditionType = EventConditionType.Collision
            }, out DamageResult _);
            this.mOwner.GoToAnimation(Magicka.Animations.attack_recoil, 0.1f);
            this.mWorldCollisionTimer = 0.25f;
            break;
          }
          break;
        }
      }
    }
    for (int mNextToDamage = this.mNextToDamage; mNextToDamage < this.mHitlist.Count; ++mNextToDamage)
    {
      IDamageable damageable3 = this.mHitlist[mNextToDamage];
      DamageResult oDamageResult;
      this.mMeleeConditions.ExecuteAll((Entity) this, (Entity) damageable3, ref new EventCondition()
      {
        EventConditionType = EventConditionType.Hit
      }, out oDamageResult);
      if (this.mPassiveAbility.Ability == Item.PassiveAbilities.DragonSlayer && !((this.mOwner as Avatar).Player.Gamer is NetworkGamer) && damageable3 is BossDamageZone && (damageable3 as BossDamageZone).Owner is Fafnir)
      {
        AchievementsManager.Instance.AwardAchievement(this.PlayState, "stuffoflegends");
        damageable3.OverKill();
        oDamageResult = DamageResult.Hit;
      }
      DamageResult damageResult = DamageResult.Damaged | DamageResult.Hit | DamageResult.Killed;
      if (this.mPassiveAbility.Ability == Item.PassiveAbilities.Mjolnr && (oDamageResult & damageResult) != DamageResult.None)
        this.MjolnirStrike(damageable3);
      if (this.mType == Item.SONIC_SCREWDRIVER && (oDamageResult & DamageResult.Healed) == DamageResult.Healed)
      {
        if (damageable3 is ISpellCaster)
        {
          Vector3 direction = this.mOwner.Direction;
          Vector3 position = damageable3.Position;
          EffectManager.Instance.StartEffect(Item.SONIC_SCREWDRIVER_HITEFFECT, ref position, ref direction, out VisualEffectReference _);
          Haste.GetInstance().Execute((ISpellCaster) damageable3, this.PlayState, true);
        }
        else if (damageable3 is DamageablePhysicsEntity)
          (damageable3 as DamageablePhysicsEntity).Damage(100f, Elements.None);
      }
      if ((oDamageResult & (DamageResult.Killed | DamageResult.OverKilled)) != DamageResult.None && damageable3 is Entity)
        AudioManager.Instance.PlayCue(Banks.Weapons, Item.DEATH_SOUND, (damageable3 as Entity).AudioEmitter);
      this.mContinueHitting = this.mContinueHitting && (oDamageResult & (DamageResult.Knockeddown | DamageResult.Knockedback | DamageResult.Pushed | DamageResult.Killed)) != DamageResult.None;
      if (!this.mContinueHitting && (oDamageResult & DamageResult.Deflected) == DamageResult.Deflected)
        this.mOwner.GoToAnimation(Magicka.Animations.attack_recoil, 0.1f);
      this.mNextToDamage = mNextToDamage + 1;
      if (!this.mMeleeMultiHit)
        break;
    }
  }

  public void ExecuteRanged(ref MissileEntity iMissile, Vector3? iVelocity, bool iItemAligned)
  {
    if (this.mRangedConditions[0] == null && this.mRangedConditions[1] == null && this.mRangedConditions[2] == null && this.mRangedConditions[3] == null && this.mRangedConditions[4] == null)
      return;
    NetworkState state = NetworkManager.Instance.State;
    if (iMissile == null && (state == NetworkState.Client || this.mOwner is Avatar && (this.mOwner as Avatar).Player.Gamer is NetworkGamer) && (state != NetworkState.Client || !(this.mOwner is Avatar) || (this.mOwner as Avatar).Player.Gamer is NetworkGamer) || this.mPassiveAbility.Ability == Item.PassiveAbilities.Gungner && this.mGungnirMissile != null || this.mPassiveAbility.Ability == Item.PassiveAbilities.MasterSword && (double) this.mOwner.HitPoints < (double) this.mOwner.MaxHitPoints)
      return;
    this.mCooldownTimer = this.mCooldownTime;
    if (iMissile == null)
      iMissile = this.mOwner.GetMissileInstance();
    iMissile.Danger = this.mRangedDanger;
    iMissile.FacingVelocity = this.mFacing;
    Entity iTarget = (Entity) null;
    Vector3 result1 = this.mOwner.CharacterBody.Direction;
    Vector3 translation = this.mAttach0AbsoluteTransform.Translation;
    if (iVelocity.HasValue)
    {
      Matrix matrix = !iItemAligned ? this.mOwner.Body.Orientation : this.mBody.Orientation;
      result1 = iVelocity.Value;
      Vector3.TransformNormal(ref result1, ref matrix, out result1);
      if (this.mSourceAbility != null && this.mSourceAbility is Ranged && (this.mOwner as NonPlayerCharacter).AI.CurrentTarget != null)
        iTarget = (this.mOwner as NonPlayerCharacter).AI.CurrentTarget as Entity;
    }
    else if (this.mSourceAbility != null && this.mSourceAbility is Ranged && (this.mOwner as NonPlayerCharacter).AI.CurrentTarget != null)
    {
      Vector3 position1 = (this.mOwner as NonPlayerCharacter).AI.CurrentTarget.Position;
      iTarget = (this.mOwner as NonPlayerCharacter).AI.CurrentTarget as Entity;
      Vector3 position2 = this.Position;
      float num1 = position1.Y - position2.Y;
      position1.Y = position2.Y = 0.0f;
      float result2;
      Vector3.Distance(ref position1, ref position2, out result2);
      float num2 = (this.mSourceAbility as Ranged).GetElevation() + this.mRangedElevation;
      float num3 = result1.Y = (float) System.Math.Sin((double) num2);
      float num4 = (float) System.Math.Cos((double) num2);
      result1.X *= num4;
      result1.Z *= num4;
      float accuracy = (this.mSourceAbility as Ranged).GetAccuracy();
      float num5 = accuracy * (MagickaMath.RandomBetween(-1f, 1f) * System.Math.Max(1f - accuracy, 0.0f));
      float f = (float) System.Math.Sqrt((double) PhysicsManager.Instance.Simulator.Gravity.Y * -1.0 * (double) result2 * (double) result2 / (2.0 * ((double) result2 * (double) num3 / (double) num4 - (double) num1) * (double) num4 * (double) num4));
      if (float.IsNaN(f) || float.IsInfinity(f))
        f = this.RangedRange * 2f;
      float scaleFactor = f * (num5 + 1f);
      Quaternion result3;
      Quaternion.CreateFromYawPitchRoll(num5 * 0.7853982f, 0.0f, 0.0f, out result3);
      Vector3.Transform(ref result1, ref result3, out result1);
      Vector3.Multiply(ref result1, scaleFactor, out result1);
    }
    else
    {
      if ((double) this.mRangedElevation > 0.0)
      {
        float num6 = (float) System.Math.Sin((double) this.mRangedElevation);
        float num7 = (float) System.Math.Cos((double) this.mRangedElevation);
        result1.Y = num6;
        result1.X *= num7;
        result1.Z *= num7;
      }
      else
        result1.Y = 0.05f;
      result1 *= this.mRangedRange * 2f;
    }
    if (this.mPassiveAbility.Ability == Item.PassiveAbilities.Gungner)
    {
      iMissile.Initialize((Entity) this.mOwner, 0.5f, ref translation, ref result1, this.mProjectileModel, this.mRangedConditions, false);
      this.mGungnirMissile = iMissile;
      iMissile.FacingVelocity = true;
    }
    else if (iTarget != null)
    {
      if (this.mProjectileModel != null && this.mWeaponClass != WeaponClass.Heavy)
        iMissile.Initialize((Entity) this.mOwner, iTarget, this.mHoming, this.mProjectileModel.Meshes[0].BoundingSphere.Radius, ref translation, ref result1, this.mProjectileModel, this.mRangedConditions, false);
      else
        iMissile.Initialize((Entity) this.mOwner, iTarget, this.mHoming, 0.25f, ref translation, ref result1, this.mProjectileModel, this.mRangedConditions, false);
    }
    else if (this.mProjectileModel != null && this.mWeaponClass != WeaponClass.Heavy)
      iMissile.Initialize((Entity) this.mOwner, this.mProjectileModel.Meshes[0].BoundingSphere.Radius, ref translation, ref result1, this.mProjectileModel, this.mRangedConditions, false);
    else
      iMissile.Initialize((Entity) this.mOwner, 0.25f, ref translation, ref result1, this.mProjectileModel, this.mRangedConditions, false);
    if (NetworkManager.Instance.State != NetworkState.Offline)
      NetworkManager.Instance.Interface.SendMessage<SpawnMissileMessage>(ref new SpawnMissileMessage()
      {
        Type = SpawnMissileMessage.MissileType.Item,
        Owner = this.mOwner.Handle,
        Item = this.Handle,
        Handle = iMissile.Handle,
        Position = iMissile.Position,
        Velocity = iMissile.Body.Velocity,
        Homing = this.mHoming
      });
    this.mPlayState.EntityManager.AddEntity((Entity) iMissile);
  }

  private void MjolnirStrike(IDamageable iTarget)
  {
    Damage mjolnirDamage = Defines.MJOLNIR_DAMAGE;
    if (this.mPlayState.Level.CurrentScene.Indoors)
      return;
    Flash.Instance.Execute(this.mPlayState.Scene, 0.125f);
    Vector3 oPosition = iTarget.Position;
    Segment iSeg = new Segment();
    iSeg.Origin = oPosition;
    iSeg.Origin.Y += 25f;
    iSeg.Delta.Y -= 35f;
    List<Shield> shields = this.mPlayState.EntityManager.Shields;
    for (int index = 0; index < shields.Count; ++index)
    {
      if (shields[index].ShieldType == ShieldType.SPHERE && shields[index].SegmentIntersect(out oPosition, iSeg, 0.2f))
      {
        iTarget = (IDamageable) shields[index];
        oPosition.Y += iTarget.Body.CollisionSkin.WorldBoundingBox.Max.Y * 0.5f;
        break;
      }
    }
    if (!(iTarget is Shield))
    {
      iSeg.Origin = oPosition;
      iSeg.Delta.Y -= 10f;
      Vector3 oPos;
      Vector3 oNrm;
      AnimatedLevelPart oAnimatedLevelPart;
      if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out oNrm, out oAnimatedLevelPart, iSeg))
      {
        oPosition = oPos;
        DecalManager.Instance.AddAlphaBlendedDecal(Decal.Scorched, oAnimatedLevelPart, 4f, ref oPosition, ref oNrm, 60f);
      }
    }
    LightningBolt lightning = LightningBolt.GetLightning();
    Vector3 iPosition = oPosition;
    iPosition.Y += 40f;
    Vector3 iDirection = new Vector3(0.0f, -1f, 0.0f);
    Vector3 lightningcolor = Spell.LIGHTNINGCOLOR;
    Vector3 position = this.mPlayState.Scene.Camera.Position;
    float iScale = 1f;
    lightning.InitializeEffect(ref iPosition, ref iDirection, ref oPosition, ref position, ref lightningcolor, false, iScale, 1f, this.mPlayState);
    int num = (int) iTarget.Damage(mjolnirDamage, (Entity) this.mOwner, this.mOwner.PlayState.PlayTime, this.mOwner.Position);
    Vector3 right = Vector3.Right;
    EffectManager.Instance.StartEffect(Thunderbolt.EFFECT, ref oPosition, ref right, out VisualEffectReference _);
    AudioManager.Instance.PlayCue(Banks.Spells, Thunderbolt.SOUND, (iTarget as Entity).AudioEmitter);
    this.mPlayState.Camera.CameraShake(oPosition, 1.5f, 0.333f);
  }

  protected override void AddImpulseVelocity(ref Vector3 iVelocity)
  {
    AnimatedLevelPart animatedLevelPart;
    if (this.mAnimatedLevelPartID != 0 && !this.mIgnoreTractorPull && this.mPlayState.Level.CurrentScene.LevelModel.AnimatedLevelParts.TryGetValue(this.mAnimatedLevelPartID, out animatedLevelPart))
    {
      animatedLevelPart.RemoveEntity((Entity) this);
      this.AnimatedLevelPartID = 0;
    }
    base.AddImpulseVelocity(ref iVelocity);
  }

  public Magicka.GameLogic.Entities.Resistance[] Resistance => this.mResistances;

  public float RangedRange => this.mRangedRange;

  public float Homing => this.mHoming;

  public bool Facing => this.mFacing;

  public float Danger => this.mRangedDanger;

  public ConditionCollection MeleeConditions
  {
    get => this.mMeleeConditions;
    set => this.mMeleeConditions = value;
  }

  public ConditionCollection RangedConditions
  {
    get => this.mRangedConditions;
    set => this.mRangedConditions = value;
  }

  public void Detach()
  {
    this.mAttached = false;
    this.mBody.Velocity = new Vector3();
    this.StopEffects();
    for (int index = 0; index < this.mSoundCues.Length; ++index)
    {
      if (this.mSoundCues[index] != null && !this.mSoundCues[index].IsStopping)
        this.mSoundCues[index].Stop(AudioStopOptions.AsAuthored);
    }
    if (this.mEffects != null)
    {
      for (int index = 0; index < this.mVisualEffects.Length; ++index)
        EffectManager.Instance.Stop(ref this.mVisualEffects[index]);
    }
    if (this.mPointLight != null)
    {
      this.mPointLight.Disable();
      this.mPointLight = (DynamicLight) null;
      this.mPointLightHolder.Enabled = false;
    }
    if (this.mGunSound != null && !this.mGunSound.IsStopping)
    {
      this.mGunSound.Stop(AudioStopOptions.AsAuthored);
      this.mGunSound = (Cue) null;
    }
    EffectManager.Instance.Stop(ref this.mGunShellsEffect);
    for (int index = 0; index < this.mAuras.Count; ++index)
    {
      VisualEffectReference mEffect = this.mAuras[index].mEffect;
      EffectManager.Instance.Stop(ref mEffect);
    }
    if (!(this.mPlayState.Level.CurrentScene.RuleSet is VersusRuleset))
      return;
    this.Despawn(20f);
  }

  public int Type => this.mType;

  protected override bool HideModel
  {
    get
    {
      if (this.mWeaponClass != WeaponClass.Staff && this.mHideModel && this.IsCoolingdown)
        return true;
      return this.mWeaponClass == WeaponClass.Staff && this.mHideModel && !this.SpecialAbilityReady;
    }
  }

  private bool HideEffects
  {
    get
    {
      if (this.mWeaponClass != WeaponClass.Staff && this.mHideEffect && this.IsCoolingdown)
        return true;
      return this.mWeaponClass == WeaponClass.Staff && this.mHideEffect && !this.SpecialAbilityReady;
    }
  }

  private bool PauseSounds
  {
    get
    {
      if (this.mWeaponClass != WeaponClass.Staff && this.mPauseSounds && this.IsCoolingdown)
        return true;
      return this.mWeaponClass == WeaponClass.Staff && this.mPauseSounds && !this.SpecialAbilityReady;
    }
  }

  public override void Kill()
  {
    this.Model = (Model) null;
    this.mDespawnTime = 0.0f;
    EffectManager.Instance.Stop(ref this.mDespawnEffect);
  }

  protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
  {
    oMsg = new EntityUpdateMessage();
    if (!(this.mPickable & this.mBody.IsActive))
      return;
    JigLibX.Math.Transform transform = this.mBody.Transform;
    TransformRate transformRate = this.mBody.TransformRate;
    transform.ApplyTransformRate(ref transformRate, iPrediction);
    oMsg.Features |= EntityFeatures.Position;
    oMsg.Position = transform.Position;
    oMsg.Features |= EntityFeatures.Orientation;
    Quaternion.CreateFromRotationMatrix(ref transform.Orientation, out oMsg.Orientation);
    oMsg.Features |= EntityFeatures.Velocity;
    oMsg.Velocity = this.mBody.Velocity;
  }

  public override bool Permanent
  {
    get => this.mPickable && !this.Dead && (double) this.mDespawnTime <= 0.0;
  }

  internal new void SetUniqueID(int iID) => base.SetUniqueID(iID);

  public struct PointLightHolder
  {
    public bool Enabled;
    public bool ContainsLight;
    public float Radius;
    public Vector3 DiffuseColor;
    public Vector3 AmbientColor;
    public float SpecularAmount;
    public LightVariationType VariationType;
    public float VariationAmount;
    public float VariationSpeed;
  }

  protected class ElementRenderData : IRenderableAdditiveObject, IPreRenderRenderer
  {
    private static int sVerticesHash;
    private static VertexBuffer sVertices;
    private static VertexDeclaration sVertexDeclaration;
    private static Texture2D sTexture;
    public Vector3 Pos0;
    public Vector3 Pos1;
    private Matrix[] mBones = new Matrix[2];
    public float Alpha;
    public Vector3 Color;
    public BoundingSphere mBoundingSphere;

    public ElementRenderData()
    {
      if (Item.ElementRenderData.sVertices != null && !Item.ElementRenderData.sVertices.IsDisposed)
        return;
      Item.ElementRenderData.VertexPositionTextureIndex[] data = new Item.ElementRenderData.VertexPositionTextureIndex[8];
      data[0].Position.X = -1f;
      data[0].Position.Y = -1f;
      data[0].TexCoord.X = 0.0f;
      data[0].TexCoord.Y = 1f;
      data[0].BlendIndices.X = 0.0f;
      data[0].BlendWeights.X = 1f;
      data[1].Position.X = -1f;
      data[1].Position.Y = 1f;
      data[1].TexCoord.X = 0.0f;
      data[1].TexCoord.Y = 0.0f;
      data[1].BlendIndices.X = 0.0f;
      data[1].BlendWeights.X = 1f;
      data[2].Position.X = 0.0f;
      data[2].Position.Y = -1f;
      data[2].TexCoord.X = 0.25f;
      data[2].TexCoord.Y = 1f;
      data[2].BlendIndices.X = 0.0f;
      data[2].BlendWeights.X = 1f;
      data[3].Position.X = 0.0f;
      data[3].Position.Y = 1f;
      data[3].TexCoord.X = 0.25f;
      data[3].TexCoord.Y = 0.0f;
      data[3].BlendIndices.X = 0.0f;
      data[3].BlendWeights.X = 1f;
      data[4].Position.X = 0.0f;
      data[4].Position.Y = -1f;
      data[4].TexCoord.X = 0.75f;
      data[4].TexCoord.Y = 1f;
      data[4].BlendIndices.X = 1f;
      data[4].BlendWeights.X = 1f;
      data[5].Position.X = 0.0f;
      data[5].Position.Y = 1f;
      data[5].TexCoord.X = 0.75f;
      data[5].TexCoord.Y = 0.0f;
      data[5].BlendIndices.X = 1f;
      data[5].BlendWeights.X = 1f;
      data[6].Position.X = 1f;
      data[6].Position.Y = -1f;
      data[6].TexCoord.X = 1f;
      data[6].TexCoord.Y = 1f;
      data[6].BlendIndices.X = 1f;
      data[6].BlendWeights.X = 1f;
      data[7].Position.X = 1f;
      data[7].Position.Y = 1f;
      data[7].TexCoord.X = 1f;
      data[7].TexCoord.Y = 0.0f;
      data[7].BlendIndices.X = 1f;
      data[7].BlendWeights.X = 1f;
      GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
      lock (graphicsDevice)
      {
        Item.ElementRenderData.sVertices = new VertexBuffer(graphicsDevice, 52 * data.Length, BufferUsage.WriteOnly);
        Item.ElementRenderData.sVertices.SetData<Item.ElementRenderData.VertexPositionTextureIndex>(data);
        Item.ElementRenderData.sVertexDeclaration = new VertexDeclaration(graphicsDevice, Item.ElementRenderData.VertexPositionTextureIndex.VertexElements);
        Item.ElementRenderData.sTexture = Magicka.Game.Instance.Content.Load<Texture2D>("effectTextures/bladeElements");
      }
      Item.ElementRenderData.sVerticesHash = Item.ElementRenderData.sVertices.GetHashCode();
    }

    public int Effect => SkinnedModelDeferredEffect.TYPEHASH;

    public int Technique => 2;

    public VertexBuffer Vertices => Item.ElementRenderData.sVertices;

    public int VerticesHashCode => Item.ElementRenderData.sVerticesHash;

    public int VertexStride => 52;

    public IndexBuffer Indices => (IndexBuffer) null;

    public VertexDeclaration VertexDeclaration => Item.ElementRenderData.sVertexDeclaration;

    public bool Cull(BoundingFrustum iViewFrustum)
    {
      return this.mBoundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
    }

    public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      SkinnedModelDeferredEffect iEffect1 = iEffect as SkinnedModelDeferredEffect;
      new SkinnedModelDeferredAdvancedMaterial()
      {
        DiffuseColor = this.Color,
        DiffuseMap0 = Item.ElementRenderData.sTexture,
        DiffuseMap0Enabled = true,
        Alpha = this.Alpha
      }.AssignToEffect(iEffect1);
      iEffect1.Bones = this.mBones;
      iEffect1.GraphicsDevice.RenderState.DepthBias = -1f / 500f;
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleStrip, 0, 6);
      iEffect1.GraphicsDevice.RenderState.DepthBias = 0.0f;
    }

    public void PreRenderUpdate(
      DataChannel iDataChannel,
      float iDeltaTime,
      ref Matrix iViewProjectionMatrix,
      ref Vector3 iCameraPosition,
      ref Vector3 iCameraDirection)
    {
      Vector3 result1;
      Vector3.Subtract(ref this.Pos1, ref this.Pos0, out result1);
      float result2;
      Vector3.Dot(ref result1, ref iCameraDirection, out result2);
      float d = result1.LengthSquared();
      if ((double) d > 9.9999999747524271E-07 & (double) System.Math.Abs(result2) < 0.99999898672103882)
      {
        Vector3.Divide(ref result1, (float) System.Math.Sqrt((double) d), out result1);
      }
      else
      {
        result1.X = 1f;
        result1.Y = 0.0f;
        result1.Z = 0.0f;
      }
      Vector3 result3 = new Vector3();
      result3.Y = 1f;
      Vector3 result4;
      Vector3.Cross(ref iCameraDirection, ref result3, out result4);
      result4.Normalize();
      Vector3 result5;
      Vector3.Cross(ref result4, ref iCameraDirection, out result5);
      Vector2 vector2 = new Vector2();
      Vector3.Dot(ref result1, ref result4, out vector2.X);
      Vector3.Dot(ref result1, ref result5, out vector2.Y);
      vector2.Normalize();
      result1.X = (float) ((double) vector2.X * (double) result4.X + (double) vector2.Y * (double) result5.X);
      result1.Y = (float) ((double) vector2.X * (double) result4.Y + (double) vector2.Y * (double) result5.Y);
      result1.Z = (float) ((double) vector2.X * (double) result4.Z + (double) vector2.Y * (double) result5.Z);
      Vector3.Cross(ref result1, ref iCameraDirection, out result3);
      Vector3 result6;
      Vector3.Negate(ref iCameraDirection, out result6);
      this.mBones[0].M11 = result1.X * 0.333f;
      this.mBones[0].M12 = result1.Y * 0.333f;
      this.mBones[0].M13 = result1.Z * 0.333f;
      this.mBones[0].M21 = result3.X * 0.333f;
      this.mBones[0].M22 = result3.Y * 0.333f;
      this.mBones[0].M23 = result3.Z * 0.333f;
      this.mBones[0].M31 = result6.X * 0.333f;
      this.mBones[0].M32 = result6.Y * 0.333f;
      this.mBones[0].M33 = result6.Z * 0.333f;
      this.mBones[0].M41 = this.Pos0.X;
      this.mBones[0].M42 = this.Pos0.Y;
      this.mBones[0].M43 = this.Pos0.Z;
      this.mBones[0].M44 = 1f;
      this.mBones[1].M11 = result1.X * 0.333f;
      this.mBones[1].M12 = result1.Y * 0.333f;
      this.mBones[1].M13 = result1.Z * 0.333f;
      this.mBones[1].M21 = result3.X * 0.333f;
      this.mBones[1].M22 = result3.Y * 0.333f;
      this.mBones[1].M23 = result3.Z * 0.333f;
      this.mBones[1].M31 = result6.X * 0.333f;
      this.mBones[1].M32 = result6.Y * 0.333f;
      this.mBones[1].M33 = result6.Z * 0.333f;
      this.mBones[1].M41 = this.Pos1.X;
      this.mBones[1].M42 = this.Pos1.Y;
      this.mBones[1].M43 = this.Pos1.Z;
      this.mBones[1].M44 = 1f;
    }

    private struct VertexPositionTextureIndex
    {
      public const int SIZEINBYTES = 52;
      public Vector3 Position;
      public Vector2 TexCoord;
      public Vector4 BlendIndices;
      public Vector4 BlendWeights;
      public static readonly VertexElement[] VertexElements = new VertexElement[4]
      {
        new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
        new VertexElement((short) 0, (short) 12, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 0),
        new VertexElement((short) 0, (short) 20, VertexElementFormat.Vector4, VertexElementMethod.Default, VertexElementUsage.BlendIndices, (byte) 0),
        new VertexElement((short) 0, (short) 36, VertexElementFormat.Vector4, VertexElementMethod.Default, VertexElementUsage.BlendWeight, (byte) 0)
      };
    }
  }

  protected new class RenderData : Pickable.RenderData
  {
    public float EmissiveMultiplyer = 1f;

    public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      RenderDeferredEffect iEffect1 = iEffect as RenderDeferredEffect;
      this.mMaterial.AssignToEffect(iEffect1);
      iEffect1.EmissiveAmount0 *= this.EmissiveMultiplyer;
      iEffect1.EmissiveAmount1 *= this.EmissiveMultiplyer;
      iEffect1.World = this.mTransform;
      iEffect1.GraphicsDevice.RenderState.DepthBias = -5E-06f;
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
      iEffect1.GraphicsDevice.RenderState.DepthBufferFunction = CompareFunction.LessEqual;
      iEffect1.GraphicsDevice.RenderState.DepthBias = 0.0f;
    }
  }

  public enum PassiveAbilities : byte
  {
    None,
    ShieldBoost,
    AreaLifeDrain,
    ZombieDeterrent,
    ReduceAggro,
    EnhanceAllyMelee,
    AreaRegeneration,
    InverseArcaneLife,
    Zap,
    BirchSteam,
    WetLightning,
    MoveSpeed,
    Glow,
    Mjolnr,
    Gungner,
    MasterSword,
    DragonSlayer,
  }

  public struct PassiveAbilityStruct(Item.PassiveAbilities iAbility, float iVar)
  {
    public Item.PassiveAbilities Ability = iAbility;
    public float Variable = iVar;
  }
}
