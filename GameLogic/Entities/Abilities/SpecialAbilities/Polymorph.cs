// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Polymorph
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.AI;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

internal class Polymorph : SpecialAbility, IAbilityEffect, ITargetAbility
{
  private const float RADIUS = 6f;
  private const float TTL = 20f;
  private static List<Polymorph> sCache;
  private static List<Polymorph> sActiveCaches;
  private static readonly int MAGICK_EFFECT = "magick_polymorph".GetHashCodeCustom();
  private static readonly int SOUND_EFFECT = "magick_polymorph".GetHashCodeCustom();
  private static readonly int[] sAvatarTypes = new int[4]
  {
    "polymorph_horse".GetHashCodeCustom(),
    "polymorph_imp".GetHashCodeCustom(),
    "polymorph_goblin".GetHashCodeCustom(),
    "polymorph_troll".GetHashCodeCustom()
  };
  private static readonly int[] sNPCTypes = new int[4]
  {
    "horse".GetHashCodeCustom(),
    "imp".GetHashCodeCustom(),
    "goblin_bomber".GetHashCodeCustom(),
    "troll_forest".GetHashCodeCustom()
  };
  private Magicka.GameLogic.Entities.Character mTarget;
  private int mTypeBeforePolymorph;
  private PlayState mPlayState;
  private Polymorph.NPCPolymorphData mStorage;
  internal static Polymorph.AvatarPolymorphData sTemporaryDataHolder;
  private float mTTL;

  public static Polymorph GetInstance()
  {
    if (Polymorph.sCache.Count > 0)
    {
      Polymorph instance = Polymorph.sCache[Polymorph.sCache.Count - 1];
      Polymorph.sCache.RemoveAt(Polymorph.sCache.Count - 1);
      Polymorph.sActiveCaches.Add(instance);
      return instance;
    }
    Polymorph instance1 = new Polymorph((PlayState) null);
    Polymorph.sActiveCaches.Add(instance1);
    return instance1;
  }

  public static void InitializeCache(int iNr, PlayState iPlaystate)
  {
    Polymorph.sCache = new List<Polymorph>(iNr);
    Polymorph.sActiveCaches = new List<Polymorph>(iNr);
    for (int index = 0; index < iNr; ++index)
      Polymorph.sCache.Add(new Polymorph(iPlaystate));
  }

  private Polymorph(PlayState iPlayState)
    : base(Magicka.Animations.cast_magick_direct, "#magick_polymorph".GetHashCodeCustom())
  {
    if (iPlayState == null)
      return;
    iPlayState.Content.Load<CharacterTemplate>("data/characters/polymorph_horse");
    iPlayState.Content.Load<CharacterTemplate>("data/characters/polymorph_imp");
    iPlayState.Content.Load<CharacterTemplate>("data/characters/polymorph_goblin");
    iPlayState.Content.Load<CharacterTemplate>("data/characters/polymorph_troll");
    iPlayState.Content.Load<CharacterTemplate>("data/characters/horse");
    iPlayState.Content.Load<CharacterTemplate>("data/characters/imp");
    iPlayState.Content.Load<CharacterTemplate>("data/characters/goblin_bomber");
    iPlayState.Content.Load<CharacterTemplate>("data/characters/troll_forest");
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    this.mPlayState = iPlayState;
    Vector3 position = iOwner.Position;
    Vector3 direction = iOwner.Direction;
    Vector3 result;
    Vector3.Multiply(ref direction, 3f, out result);
    Vector3.Add(ref result, ref position, out result);
    Magicka.GameLogic.Entities.Character oTarget;
    if (this.FindClosestTarget(iPlayState, iOwner, 6f, ref result, out oTarget))
      return this.Execute(iOwner, (Entity) oTarget, iPlayState);
    this.OnRemove();
    AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL, iOwner.AudioEmitter);
    return false;
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    this.mPlayState = iPlayState;
    Magicka.GameLogic.Entities.Character oTarget;
    if (!this.FindClosestTarget(iPlayState, (ISpellCaster) null, 6f, ref iPosition, out oTarget))
    {
      this.OnRemove();
      return false;
    }
    switch (oTarget)
    {
      case GenericBoss _:
      case WarlordCharacter _:
      case IBoss _:
        this.OnRemove();
        return false;
      default:
        for (int index = 0; index < Polymorph.sActiveCaches.Count; ++index)
        {
          if (Polymorph.sActiveCaches[index].mTarget == oTarget)
          {
            Polymorph.sActiveCaches[index].OnRemove();
            this.OnRemove();
            return true;
          }
        }
        if (NetworkManager.Instance.State != NetworkState.Client)
        {
          switch (oTarget)
          {
            case NonPlayerCharacter _:
              int sNpcType = Polymorph.sNPCTypes[SpecialAbility.RANDOM.Next(Polymorph.sNPCTypes.Length)];
              NonPlayerCharacter oNPC;
              if (this.CreateNPCPolymorph(oTarget as NonPlayerCharacter, iPlayState, sNpcType, out oNPC))
              {
                this.mTarget = (Magicka.GameLogic.Entities.Character) oNPC;
                iPlayState.EntityManager.AddEntity((Entity) this.mTarget);
                break;
              }
              break;
            case Avatar _:
              int sAvatarType = Polymorph.sAvatarTypes[SpecialAbility.RANDOM.Next(Polymorph.sNPCTypes.Length)];
              this.mTypeBeforePolymorph = oTarget.Template.ID;
              Polymorph.PolymorphAvatar(oTarget as Avatar, sAvatarType, ref Polymorph.sTemporaryDataHolder);
              this.mTarget = oTarget;
              break;
          }
        }
        this.mTTL = 20f;
        SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
        return true;
    }
  }

  public bool Execute(ISpellCaster iOwner, Entity iTarget, PlayState iPlayState)
  {
    if (!(iTarget is Magicka.GameLogic.Entities.Character))
      return false;
    this.mPlayState = iPlayState;
    for (int index = 0; index < Polymorph.sActiveCaches.Count; ++index)
    {
      if (Polymorph.sActiveCaches[index].mTarget == iTarget)
      {
        Polymorph.sActiveCaches[index].OnRemove();
        this.OnRemove();
        return true;
      }
    }
    if (NetworkManager.Instance.State != NetworkState.Client)
    {
      switch (iTarget)
      {
        case NonPlayerCharacter _:
          if (iTarget is GenericBoss)
            return false;
          int sNpcType = Polymorph.sNPCTypes[SpecialAbility.RANDOM.Next(Polymorph.sNPCTypes.Length)];
          NonPlayerCharacter oNPC;
          if (this.CreateNPCPolymorph(iTarget as NonPlayerCharacter, iPlayState, sNpcType, out oNPC))
          {
            this.mTarget = (Magicka.GameLogic.Entities.Character) oNPC;
            iPlayState.EntityManager.AddEntity((Entity) this.mTarget);
            break;
          }
          break;
        case Avatar _:
          int sAvatarType = Polymorph.sAvatarTypes[SpecialAbility.RANDOM.Next(Polymorph.sNPCTypes.Length)];
          this.mTypeBeforePolymorph = (iTarget as Avatar).Template.ID;
          Polymorph.PolymorphAvatar(iTarget as Avatar, sAvatarType, ref Polymorph.sTemporaryDataHolder);
          this.mTarget = iTarget as Magicka.GameLogic.Entities.Character;
          break;
      }
    }
    Vector3 position = iTarget.Position;
    Vector3 forward = iTarget.Body.Orientation.Forward;
    EffectManager.Instance.StartEffect(Polymorph.MAGICK_EFFECT, ref position, ref forward, out VisualEffectReference _);
    AudioManager.Instance.PlayCue(Banks.Additional, Polymorph.SOUND_EFFECT, iTarget.AudioEmitter);
    this.mTTL = 20f;
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    return true;
  }

  public bool IsDead => (double) this.mTTL <= 0.0;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mTTL -= iDeltaTime;
    if (this.mTarget != null && (double) this.mTarget.HitPoints > 0.0)
      return;
    this.mTTL = 0.0f;
  }

  public void OnRemove()
  {
    if (this.mTarget != null && (double) this.mTarget.HitPoints > 0.0 && this.mPlayState != null && this.mPlayState.Level != null)
    {
      if (this.mTarget is NonPlayerCharacter)
      {
        NonPlayerCharacter oNPC;
        if (this.CreateNPCPolymorph(this.mTarget as NonPlayerCharacter, this.mTarget.PlayState, this.mTypeBeforePolymorph, out oNPC))
          oNPC.PlayState.EntityManager.AddEntity((Entity) oNPC);
      }
      else if (this.mTarget is Avatar)
        Polymorph.PolymorphAvatar(this.mTarget as Avatar, this.mTypeBeforePolymorph, ref Polymorph.sTemporaryDataHolder);
    }
    this.mStorage.Active = false;
    this.mStorage.EventIndex = -1;
    this.mStorage.Events = (AIEvent[]) null;
    this.mTarget = (Magicka.GameLogic.Entities.Character) null;
    this.mTTL = 0.0f;
    Polymorph.sActiveCaches.Remove(this);
    Polymorph.sCache.Add(this);
  }

  private bool FindClosestTarget(
    PlayState iPlayState,
    ISpellCaster iOwner,
    float iRadius,
    ref Vector3 iPosition,
    out Magicka.GameLogic.Entities.Character oTarget)
  {
    List<Entity> entities = iPlayState.EntityManager.GetEntities(iPosition, iRadius, false, true);
    oTarget = (Magicka.GameLogic.Entities.Character) null;
    float num = float.MaxValue;
    for (int index = 0; index < entities.Count; ++index)
    {
      if (entities[index] is Magicka.GameLogic.Entities.Character character && character != iOwner && !character.IsEthereal && !string.Equals(character.Name, "boss_vlad_swamp", StringComparison.InvariantCultureIgnoreCase) && !string.Equals(character.Name, "vlad_vietnam", StringComparison.InvariantCultureIgnoreCase) && !string.Equals(character.Name, "vlad_future", StringComparison.InvariantCultureIgnoreCase))
      {
        Vector3 position = character.Position;
        float result;
        Vector3.DistanceSquared(ref position, ref iPosition, out result);
        if ((double) result < (double) num)
        {
          oTarget = character;
          num = result;
        }
      }
    }
    iPlayState.EntityManager.ReturnEntityList(entities);
    return oTarget != null;
  }

  private bool CreateNPCPolymorph(
    NonPlayerCharacter iTarget,
    PlayState iPlayState,
    int iNewType,
    out NonPlayerCharacter oNPC)
  {
    this.mTypeBeforePolymorph = iTarget.Type;
    oNPC = NonPlayerCharacter.GetInstance(iPlayState);
    if (oNPC == null)
      return false;
    CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(iNewType);
    oNPC.Initialize(cachedTemplate, new Vector3(), iTarget.UniqueID);
    oNPC.CopyPolymorphValuesFrom(iTarget);
    oNPC.AI.CopyPolymorphValues(iTarget.AI, ref this.mStorage);
    Vector3 position = iTarget.Body.Position;
    Matrix orientation = iTarget.Body.Orientation;
    float num = (float) ((double) oNPC.Radius + (double) oNPC.Capsule.Length * 0.5 - ((double) iTarget.Radius + (double) iTarget.Capsule.Length * 0.5));
    position.Y += num;
    oNPC.Body.MoveTo(position, orientation);
    iTarget.Terminate(false, false);
    if (NetworkManager.Instance.State == NetworkState.Server)
      NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
      {
        ActionType = TriggerActionType.SpawnNPC,
        Handle = oNPC.Handle,
        Template = iNewType,
        Id = oNPC.UniqueID,
        Position = position,
        Direction = orientation.Forward,
        Bool0 = false
      });
    return true;
  }

  public static void PolymorphAvatar(
    Avatar iTarget,
    int iNewType,
    ref Polymorph.AvatarPolymorphData iData)
  {
    CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(iNewType);
    Vector3 position = iTarget.Body.Position;
    Matrix orientation = iTarget.Body.Orientation;
    float num = (float) ((double) cachedTemplate.Radius + (double) cachedTemplate.Length * 0.5 - ((double) iTarget.Radius + (double) iTarget.Capsule.Length * 0.5));
    position.Y += num;
    iTarget.GetPolymorphValues(out iData);
    iTarget.KillAnimationActions();
    iTarget.SpellQueue.Clear();
    iTarget.Player.IconRenderer.Clear();
    if (((IEnumerable<int>) Polymorph.sAvatarTypes).Contains<int>(iNewType))
    {
      int outboundUdpStamp = iTarget.OutboundUDPStamp;
      iTarget.Polymorphed = true;
      iTarget.Initialize(cachedTemplate, -1, position, iTarget.UniqueID);
      iTarget.Polymorphed = true;
      iTarget.OutboundUDPStamp = outboundUdpStamp;
    }
    else
    {
      int outboundUdpStamp = iTarget.OutboundUDPStamp;
      iTarget.Polymorphed = false;
      iTarget.Initialize(cachedTemplate, -1, position, iTarget.UniqueID);
      iTarget.OutboundUDPStamp = outboundUdpStamp;
    }
    iTarget.ApplyPolymorphValues(ref iData);
    iTarget.Body.MoveTo(position, orientation);
    if (NetworkManager.Instance.State != NetworkState.Server)
      return;
    NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
    {
      Action = ActionType.Magick,
      Handle = iTarget.Handle,
      Param3I = 39,
      Param0I = iNewType
    });
  }

  internal struct AvatarPolymorphData
  {
    public int Type;
    public Factions Faction;
    public float NormalizedHP;
    public float TimeSinceLastDamage;
    public float TimeSinceLastStatusDamage;
    public Vector3 DesiredDirection;
    public bool IsEthereal;
    public Magicka.GameLogic.Entities.Character FearedBy;
    public Vector3 FearPosition;
    public float FearTimer;
    public Entity CharmOwner;
    public VisualEffectReference CharmEffect;
    public float CharmTimer;
    public bool Hypnotized;
    public VisualEffectReference HypnotizeEffect;
    public Vector3 HypnotizeDirection;
    public static StatusEffect[] sTempStatusEffects = new StatusEffect[9];
  }

  internal struct NPCPolymorphData
  {
    public bool Active;
    public int EventIndex;
    public AIEvent[] Events;
  }
}
