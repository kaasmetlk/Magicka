// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.NonPlayerCharacter
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.AI;
using Magicka.AI.AgentStates;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Abilities;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;
using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace Magicka.GameLogic.Entities;

public class NonPlayerCharacter : Character
{
  public const int MAX_ABILITY_COUNT = 16 /*0x10*/;
  private static List<NonPlayerCharacter> sCache;
  public static Factions CharmFaction = Factions.FRIENDLY;
  protected Agent mAI;
  private Spell mSpellToCast;
  protected Ability[] mAbilities;
  protected float[] mAbilityCooldown = new float[16 /*0x10*/];
  private bool mSummoned;
  private bool mUndeadSummon;
  private bool mFlamerSummon;
  private Character mSummonMaster;
  private VisualEffectReference mSummonedEffect;
  private float mSpellCooldown = 4f;
  private float mChantCooldown = 1f;
  private float mSpellPushGrowthRate;
  private float mSpellAreaGrowthRate;
  private float mBlockTimer;
  private float mBreakFreeTimer;
  private Fairy mFairy;

  public static NonPlayerCharacter GetInstance(PlayState iPlayState)
  {
    lock (NonPlayerCharacter.sCache)
    {
      if (NonPlayerCharacter.sCache.Count <= 0)
        return new NonPlayerCharacter(iPlayState);
      NonPlayerCharacter instance = NonPlayerCharacter.sCache[0];
      NonPlayerCharacter.sCache.RemoveAt(0);
      return instance;
    }
  }

  public static NonPlayerCharacter GetSpecificInstance(ushort iHandle)
  {
    NonPlayerCharacter fromHandle;
    lock (NonPlayerCharacter.sCache)
    {
      fromHandle = Entity.GetFromHandle((int) iHandle) as NonPlayerCharacter;
      NonPlayerCharacter.sCache.Remove(fromHandle);
    }
    return fromHandle;
  }

  public static void InitializeCache(int iNr, PlayState iPlayState)
  {
    NonPlayerCharacter.sCache = new List<NonPlayerCharacter>(iNr);
    for (int index = 0; index < iNr; ++index)
      NonPlayerCharacter.sCache.Add(new NonPlayerCharacter(iPlayState));
  }

  internal Fairy RevivalFairy
  {
    get => this.mFairy;
    set => this.mFairy = value;
  }

  protected NonPlayerCharacter(PlayState iPlayState)
    : base(iPlayState)
  {
    this.mAI = new Agent(this);
    this.mFairy = Fairy.MakeFairy(iPlayState, (Character) this);
  }

  protected override void ApplyTemplate(CharacterTemplate iTemplate, ref int iModel)
  {
    base.ApplyTemplate(iTemplate, ref iModel);
    this.mAbilities = iTemplate.Abilities;
    for (int index = 0; index < this.mAbilityCooldown.Length; ++index)
      this.mAbilityCooldown[index] = 0.0f;
  }

  public override void Initialize(
    CharacterTemplate iTemplate,
    int iRandomOverride,
    Vector3 iPosition,
    int iUniqueID)
  {
    base.Initialize(iTemplate, iRandomOverride, iPosition, iUniqueID);
    this.mSpellToCast = new Spell();
    this.mSpellAreaGrowthRate = 0.0f;
    this.mSpellPushGrowthRate = 0.0f;
    this.mSummoned = false;
    this.mUndeadSummon = false;
    this.mBlockTimer = 0.0f;
    this.SpellQueue.Clear();
    this.mAI.Reset();
    this.mAI.Initialize(this, iTemplate);
    this.mAI.Enable();
    if (!this.HasFairy)
      return;
    this.RevivalFairy.Initialize(this.mPlayState, false);
  }

  public float SpellPushGrowthRate
  {
    get => this.mSpellPushGrowthRate;
    set => this.mSpellPushGrowthRate = value;
  }

  public float SpellAreaGrowthRate
  {
    get => this.mSpellAreaGrowthRate;
    set => this.mSpellAreaGrowthRate = value;
  }

  public void SetSpellToCast(ref Spell iSpell) => this.mSpellToCast = iSpell;

  public float ChantCooldown
  {
    get => this.mChantCooldown;
    set => this.mChantCooldown = value;
  }

  public virtual Ability[] Abilities => this.mAbilities;

  public float[] AbilityCooldown => this.mAbilityCooldown;

  public override void Charm(Entity iCassanova, float iTTL, int iEffect)
  {
    base.Charm(iCassanova, iTTL, iEffect);
    while (this.AI.CurrentState != AIStateIdle.Instance)
    {
      if (this.AI.CurrentState is AIStateAttack)
        this.AI.ReleaseTarget();
      this.AI.PopState();
    }
  }

  public override void Hypnotize(ref Vector3 iDirection, int iEffect)
  {
    base.Hypnotize(ref iDirection, iEffect);
    while (this.AI.CurrentState != AIStateIdle.Instance)
    {
      if (this.AI.CurrentState is AIStateAttack)
        this.AI.ReleaseTarget();
      this.AI.PopState();
    }
  }

  public override void StopHypnotize()
  {
    base.StopHypnotize();
    if (!this.mHypnotized)
      return;
    while (this.AI.CurrentState != AIStateIdle.Instance)
    {
      if (this.AI.CurrentState is AIStateAttack)
        this.AI.ReleaseTarget();
      this.AI.PopState();
    }
  }

  internal void Confuse(Factions iNewFaction)
  {
    this.mFaction = iNewFaction;
    while (this.AI.CurrentState != AIStateIdle.Instance)
    {
      if (this.AI.CurrentState is AIStateAttack)
        this.AI.ReleaseTarget();
      this.AI.PopState();
    }
  }

  public override void EndCharm()
  {
    base.EndCharm();
    while (this.AI.CurrentState != AIStateIdle.Instance)
    {
      if (this.AI.CurrentState is AIStateAttack)
        this.AI.ReleaseTarget();
      this.AI.PopState();
    }
  }

  public Agent AI => this.mAI;

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mBlockTimer -= iDeltaTime;
    if ((double) this.mBlockTimer <= 0.0 & this.IsBlocking)
      this.IsBlocking = false;
    if (GlobalSettings.Instance.HealthBars == SettingOptions.On && (double) this.HitPoints > 0.0)
    {
      float num = Math.Min(this.mTimeSinceLastDamage, this.mTimeSinceLastStatusDamage);
      Vector4 vector4 = new Vector4(1f, 0.0f, 0.0f, 1f);
      if (this.HasStatus(StatusEffects.Poisoned))
      {
        vector4.X = 0.0f;
        vector4.Y = 1f;
      }
      for (int index = 0; index < this.mNumberOfHealtBars; ++index)
      {
        Vector2 vector2 = new Vector2(0.0f, (float) index * 6f);
        if (index == 0)
        {
          vector4 = new Vector4(1f, 0.0f, 0.0f, 1f);
          if (this.HasStatus(StatusEffects.Poisoned) && this.mNumberOfHealtBars == 1)
            vector4 = new Vector4(0.0f, 1f, 0.0f, 1f);
        }
        else if (index == this.mNumberOfHealtBars - 1)
        {
          vector4 = new Vector4(2f, 1.5f, 1f, 1f);
          if (this.HasStatus(StatusEffects.Poisoned))
            vector4 = new Vector4(0.0f, 1f, 0.0f, 1f);
        }
        else
          vector4 = new Vector4(0.6f, 0.6f, 0.6f, 1f);
        Vector3 position = this.Position;
        position.Y -= this.Capsule.Length * 0.5f + this.Capsule.Radius;
        if (index == this.mNumberOfHealtBars - 1)
          Healthbars.Instance.AddHealthBar(position, this.NormalizedHitPoints, this.mRadius, 1f, num, true, new Vector4?(vector4), new Vector2?(vector2));
        else
          Healthbars.Instance.AddHealthBar(position, 1f, this.mRadius, 1f, Math.Max(num, 0.3f), true, new Vector4?(vector4), new Vector2?(vector2));
      }
    }
    this.mBreakFreeTimer -= iDeltaTime;
    Vector3 position1 = this.Position;
    Vector3 direction = this.Direction;
    if (this.mSummoned)
      EffectManager.Instance.UpdatePositionDirection(ref this.mSummonedEffect, ref position1, ref direction);
    for (int index = 0; index < 16 /*0x10*/; ++index)
      this.mAbilityCooldown[index] -= iDeltaTime;
    this.mChantCooldown -= iDeltaTime;
    this.mSpellCooldown -= iDeltaTime;
    base.Update(iDataChannel, iDeltaTime);
    this.mStaffOrb = this.GetLeftAttachOrientation();
    this.mWeaponTransform = this.GetRightAttachOrientation();
  }

  public override DamageResult InternalDamage(
    Magicka.GameLogic.Damage iDamage,
    Entity iAttacker,
    double iTimeStamp,
    Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
    if (iAttacker is Character && this.AI != null)
    {
      float iAmount = iDamage.Amount * iDamage.Magnitude;
      this.AI.AddAttackedBy((Character) iAttacker, iAmount);
    }
    return base.InternalDamage(iDamage, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
  }

  public override void Terminate(bool iKillItems, bool iIsKillPlane, bool iNetwork)
  {
    base.Terminate(iKillItems, iIsKillPlane, iNetwork);
    this.Die();
  }

  public override void Drown()
  {
    base.Drown();
    this.Die();
  }

  public override void OverKill()
  {
    base.OverKill();
    this.Die();
  }

  public override void Die()
  {
    base.Die();
    if (this.mAI != null & !this.mUndying)
      this.mAI.Disable();
    EffectManager.Instance.Stop(ref this.mSummonedEffect);
    lock (NonPlayerCharacter.sCache)
    {
      if (!NonPlayerCharacter.sCache.Contains(this))
        NonPlayerCharacter.sCache.Add(this);
    }
    if (this.Master == null)
      return;
    this.Master.RemoveBubbleShield();
  }

  public void Summoned(Character iMaster) => this.Summoned(iMaster, false, false);

  public void Summoned(Character iMaster, bool undead) => this.Summoned(iMaster, undead, false);

  public void Summoned(Character iMaster, bool undead, bool flamer)
  {
    this.mSummoned = true;
    this.mFlamerSummon = flamer;
    this.mUndeadSummon = undead;
    if (this.mUndeadSummon)
      this.mFlamerSummon = false;
    this.mSummonMaster = iMaster;
    this.mFaction = iMaster.Faction;
    this.mSummonMaster.OnSpawnedSummon(this);
  }

  public void Block()
  {
    this.mBlockTimer = 1f;
    this.IsBlocking = true;
  }

  public bool IsSummoned
  {
    get => this.mSummoned;
    set => this.mSummoned = value;
  }

  public bool IsUndeadSummon => this.mUndeadSummon;

  public bool IsFlamerSummon
  {
    get => this.mFlamerSummon;
    set
    {
      if (this.mUndeadSummon && value)
        this.mFlamerSummon = false;
      else
        this.mFlamerSummon = value;
    }
  }

  public Character Master => this.mSummonMaster;

  public override Factions Faction => this.IsCharmed ? Factions.FRIENDLY : base.Faction;

  public override void BreakFree()
  {
    if ((double) this.mBreakFreeTimer > 0.0)
      return;
    base.BreakFree();
    this.mBreakFreeTimer = 0.5f;
  }

  public void StopAttacking()
  {
    this.mAttacking = false;
    this.mGripAttack = false;
    this.mDashing = false;
    this.mNextAttackAnimation = Animations.None;
  }

  public override void Deinitialize()
  {
    base.Deinitialize();
    EffectManager.Instance.Stop(ref this.mSummonedEffect);
    for (int index = 0; index < this.Equipment.Length; ++index)
      this.Equipment[index].Item.Deinitialize();
    this.mAI.Disable();
    lock (NonPlayerCharacter.sCache)
    {
      if (!NonPlayerCharacter.sCache.Contains(this))
        NonPlayerCharacter.sCache.Add(this);
    }
    if (this.mSummonMaster != null)
      this.mSummonMaster.OnDespawnedSummon(this);
    this.mSummonMaster = (Character) null;
  }

  public override int Boosts
  {
    get => 0;
    set
    {
    }
  }

  public bool CanCastSpellNow => (double) this.mSpellCooldown < 0.0;

  public override bool IsInAEvent
  {
    get => this.mAI.Events != null && this.mAI.CurrentEvent < this.mAI.Events.Length;
  }

  public override float BoostCooldown => 0.0f;

  public override bool IsAggressive => this.mAI != null && this.mAI.CurrentTarget != null;

  public override void CastSpell(bool iFromStaff, string iJoint)
  {
    if (this.CastType == CastType.Weapon)
    {
      this.mSpell = SpellManager.Instance.Combine(this.mSpellQueue);
      this.SpellQueue.Clear();
      this.mSpell.Cast(iFromStaff, (ISpellCaster) this, this.CastType);
      this.mSpell = new Spell();
    }
    else
      base.CastSpell(iFromStaff, iJoint);
  }

  public override CastType CastType
  {
    set
    {
      if (value != CastType.Weapon & value != CastType.None & value != this.mCastType)
      {
        if (value == CastType.Magick)
          SpellManager.Instance.CombineMagick((Player) null, this.mPlayState.GameType, this.mSpellQueue);
        this.CombineSpell();
      }
      this.mCastType = value;
    }
  }

  internal void ConjureSpell(Elements iElements)
  {
    Spell oSpell;
    Spell.DefaultSpell(iElements, out oSpell);
    if (this.mSummonElementCue != 0)
      AudioManager.Instance.PlayCue(this.mSummonElementBank, this.mSummonElementCue, this.AudioEmitter);
    if (!SpellManager.Instance.TryAddToQueue((Player) null, (Character) this, this.mSpellQueue, this.mSpellQueue.Capacity, ref oSpell) || NetworkManager.Instance.State != NetworkState.Server)
      return;
    NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
    {
      Handle = this.Handle,
      Action = ActionType.ConjureElement,
      Param0I = (int) iElements
    });
  }

  internal void CopyPolymorphValuesFrom(NonPlayerCharacter iSource)
  {
    this.mType = iSource.Type;
    this.mFaction = iSource.mFaction;
    this.mHitPoints = iSource.mHitPoints / iSource.mMaxHitPoints * this.mMaxHitPoints;
    this.mTimeSinceLastDamage = iSource.mTimeSinceLastDamage;
    this.mTimeSinceLastStatusDamage = iSource.mTimeSinceLastStatusDamage;
    this.CharacterBody.DesiredDirection = iSource.CharacterBody.DesiredDirection;
    if (iSource.IsEthereal)
      this.Ethereal(true, 1f, 1f);
    if (iSource.IsFeared)
    {
      if (iSource.mFearedBy != null)
        this.Fear(iSource.mFearedBy);
      else
        this.Fear(iSource.mFearPosition);
      this.mFearTimer = iSource.mFearTimer;
    }
    foreach (StatusEffect statusEffect in iSource.GetStatusEffects())
    {
      int num = (int) this.AddStatusEffect(statusEffect);
    }
    if (iSource.IsSummoned)
    {
      this.mSummoned = true;
      this.mSummonMaster = iSource.mSummonMaster;
      this.mSummonMaster.OnDespawnedSummon(iSource);
      this.mSummonMaster.OnSpawnedSummon(this);
    }
    if (iSource.IsCharmed)
    {
      this.Charm(iSource.mCharmOwner, iSource.mCharmTimer, iSource.mCharmEffectID);
      this.mCharmTimer = iSource.mCharmTimer;
    }
    if (!iSource.IsHypnotized)
      return;
    this.Hypnotize(ref iSource.mHypnotizeDirection, iSource.mHypnotizeEffectID);
  }

  public struct State
  {
    public string Name;
    public int Model;
    public int UniqueID;
    public Vector3 Position;
    public float HitPoints;
    public bool IsEthereal;
    public int Dialog;
    public int OnDamageTrigger;
    public int OnDeathTrigger;
    public Order Order;
    public ReactTo ReactTo;
    public Order Reaction;
    public int PriorityTarget;
    public int PriorityAbility;
    public int ReactionTrigger;
    public AIEvent[] Events;
    public int CurrentEvent;
    public float CurrentEventDelay;

    public State(BinaryReader iReader)
    {
      this.Name = iReader.ReadString();
      this.Model = iReader.ReadInt32();
      this.UniqueID = iReader.ReadInt32();
      this.Position.X = iReader.ReadSingle();
      this.Position.Y = iReader.ReadSingle();
      this.Position.Z = iReader.ReadSingle();
      this.HitPoints = iReader.ReadSingle();
      this.IsEthereal = iReader.ReadBoolean();
      this.Dialog = iReader.ReadInt32();
      this.OnDamageTrigger = iReader.ReadInt32();
      this.OnDeathTrigger = iReader.ReadInt32();
      this.Order = (Order) iReader.ReadByte();
      this.ReactTo = (ReactTo) iReader.ReadByte();
      this.Reaction = (Order) iReader.ReadByte();
      this.PriorityTarget = iReader.ReadInt32();
      this.PriorityAbility = iReader.ReadInt32();
      this.ReactionTrigger = iReader.ReadInt32();
      this.Events = (AIEvent[]) null;
      int length = iReader.ReadInt32();
      if (length > 0)
      {
        this.Events = new AIEvent[length];
        for (int index = 0; index < length; ++index)
          this.Events[index] = new AIEvent(iReader);
      }
      this.CurrentEvent = iReader.ReadInt32();
      this.CurrentEventDelay = iReader.ReadSingle();
    }

    public State(NonPlayerCharacter iNPC)
    {
      this.Name = iNPC.Name;
      this.Model = iNPC.ModelIndex;
      this.UniqueID = iNPC.UniqueID;
      this.Position = iNPC.Position;
      this.HitPoints = iNPC.HitPoints;
      this.IsEthereal = iNPC.IsEthereal;
      this.Dialog = iNPC.Dialog;
      this.OnDamageTrigger = iNPC.OnDamageTrigger;
      this.OnDeathTrigger = iNPC.OnDeathTrigger;
      this.Order = iNPC.AI.Order;
      this.ReactTo = iNPC.AI.ReactsTo;
      this.Reaction = iNPC.AI.ReactionOrder;
      this.PriorityTarget = iNPC.AI.PriorityTargetID;
      this.PriorityAbility = iNPC.AI.PriorityAbility;
      this.ReactionTrigger = iNPC.AI.ReactionTrigger;
      this.Events = iNPC.AI.Events != null ? iNPC.AI.Events.Clone() as AIEvent[] : (AIEvent[]) null;
      this.CurrentEvent = iNPC.AI.CurrentEvent;
      this.CurrentEventDelay = iNPC.AI.CurrentEventDelay;
    }

    public void ApplyTo(NonPlayerCharacter iNPC)
    {
      CharacterTemplate iTemplate = iNPC.PlayState.Content.Load<CharacterTemplate>("data/characters/" + this.Name);
      iNPC.Initialize(iTemplate, this.Model, this.Position, this.UniqueID);
      iNPC.HitPoints = this.HitPoints;
      iNPC.IsEthereal = this.IsEthereal;
      iNPC.Dialog = this.Dialog;
      iNPC.OnDamageTrigger = this.OnDamageTrigger;
      iNPC.OnDeathTrigger = this.OnDeathTrigger;
      iNPC.AI.SetOrder(this.Order, this.ReactTo, this.Reaction, this.PriorityTarget, this.PriorityAbility, this.ReactionTrigger, this.Events);
      iNPC.AI.CurrentEvent = this.CurrentEvent;
      iNPC.AI.CurrentEventDelay = this.CurrentEventDelay;
    }

    public void Write(BinaryWriter iWriter)
    {
      iWriter.Write(this.Name);
      iWriter.Write(this.Model);
      iWriter.Write(this.UniqueID);
      iWriter.Write(this.Position.X);
      iWriter.Write(this.Position.Y);
      iWriter.Write(this.Position.Z);
      iWriter.Write(this.HitPoints);
      iWriter.Write(this.IsEthereal);
      iWriter.Write(this.Dialog);
      iWriter.Write(this.OnDamageTrigger);
      iWriter.Write(this.OnDeathTrigger);
      iWriter.Write((byte) this.Order);
      iWriter.Write((byte) this.ReactTo);
      iWriter.Write((byte) this.Reaction);
      iWriter.Write(this.PriorityTarget);
      iWriter.Write(this.PriorityAbility);
      iWriter.Write(this.ReactionTrigger);
      if (this.Events == null)
      {
        iWriter.Write(0);
      }
      else
      {
        iWriter.Write(this.Events.Length);
        for (int index = 0; index < this.Events.Length; ++index)
          this.Events[index].Write(iWriter);
      }
      iWriter.Write(this.CurrentEvent);
      iWriter.Write(this.CurrentEventDelay);
    }
  }
}
