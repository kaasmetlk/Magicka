// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.Spawn
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.AI;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class Spawn : Action
{
  private XmlNode mNode;
  private List<Spawn.HandleAndHealth> mSpawnedEntityHP = new List<Spawn.HandleAndHealth>(10);
  private bool mSnapToNavMesh;
  private string mArea;
  private int mAreaID;
  private string mType;
  private int mTypeID;
  private int mAmount = 1;
  private string mUniqueName;
  private int mUniqueID;
  private string mDialog;
  private int mDialogID;
  private bool mForceDraw;
  private float mHealth = 1f;
  private Order mOrder = Order.Attack;
  private Order mReaction = Order.Attack;
  private ReactTo mReactTo = ReactTo.Attack | ReactTo.Proximity;
  private string mTrigger;
  private int mTriggerID;
  private AIEvent[] mEvents;
  private Magicka.Animations mAnimation;
  private Magicka.Animations mSpawnAnimation;
  private Magicka.Animations mSpecialIdleAnimation;
  private bool mNeverRemove;
  private string mTargetArea;
  private int mTargetAreaID;
  private float mWanderSpeed = 1f;
  private string mPriorityTarget;
  private int mPriorityTargetHash;
  private int mPriorityAbility = -1;
  private int mMeshIdx = -1;
  private string mOnDeathTrigger;
  private int mOnDeathTriggerID;
  private string mOnDamageTrigger;
  private int mOnDamageTriggerID;
  private bool mForceCamera;
  private bool mForceNavMesh;
  private bool mCannotDieWithoutExplicitKill;

  public Spawn(Magicka.Levels.Triggers.Trigger iTrigger, GameScene iScene, XmlNode iNode)
    : base(iTrigger, iScene)
  {
    this.mNode = iNode;
  }

  public float GetTotalHitPoins()
  {
    float totalHitPoins = 0.0f;
    for (int index = 0; index < this.mAmount; ++index)
    {
      CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(this.mTypeID);
      totalHitPoins += cachedTemplate.MaxHitpoints;
    }
    return totalHitPoins;
  }

  public override void Initialize()
  {
    base.Initialize();
    this.GameScene.PlayState.Content.Load<CharacterTemplate>("Data/Characters/" + this.mType);
    LevelModel levelModel = this.GameScene.LevelModel;
    List<AIEvent> aiEventList = new List<AIEvent>();
    foreach (XmlNode childNode in this.mNode.ChildNodes)
    {
      if (!(childNode is XmlComment))
        aiEventList.Add(new AIEvent(levelModel, childNode));
    }
    this.mEvents = aiEventList.ToArray();
  }

  protected override void Execute()
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(this.mTypeID);
    for (int index = 0; index < this.mAmount; ++index)
    {
      NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(this.GameScene.PlayState);
      if (instance == null)
        break;
      if (instance is GenericBoss)
      {
        int num = 0;
        while (true)
        {
          switch (instance)
          {
            case null:
            case GenericBoss _:
              ++num;
              instance = NonPlayerCharacter.GetInstance(this.GameScene.PlayState);
              continue;
            default:
              goto label_9;
          }
        }
      }
label_9:
      Matrix oLocator;
      this.GameScene.GetLocator(this.mAreaID, out oLocator);
      if (this.mSnapToNavMesh)
      {
        Vector3 translation = oLocator.Translation;
        Vector3 oPoint;
        double nearestPosition = (double) this.GameScene.LevelModel.NavMesh.GetNearestPosition(ref translation, out oPoint, cachedTemplate.MoveAbilities);
        oLocator.Translation = oPoint;
      }
      instance.Initialize(cachedTemplate, this.mMeshIdx, oLocator.Translation, this.mUniqueID);
      if (this.mForceDraw)
        instance.ForceDraw();
      instance.HitPoints = instance.MaxHitPoints * this.mHealth;
      instance.OnDeathTrigger = this.mOnDeathTriggerID;
      instance.OnDamageTrigger = this.mOnDamageTriggerID;
      Agent ai = instance.AI;
      ai.WanderSpeed = this.mWanderSpeed;
      ai.TargetArea = this.mTargetAreaID;
      ai.SetOrder(this.mOrder, this.mReactTo, this.mReaction, this.mPriorityTargetHash, this.mPriorityAbility, this.mTriggerID, this.mEvents);
      instance.Dialog = this.mDialogID;
      Matrix matrix = oLocator with
      {
        Translation = new Vector3()
      };
      instance.CharacterBody.Orientation = matrix;
      instance.CharacterBody.DesiredDirection = matrix.Forward;
      instance.RemoveAfterDeath = !this.mNeverRemove;
      instance.ForceCamera = this.mForceCamera;
      instance.ForceNavMesh = this.mForceNavMesh;
      instance.CannotDieWithoutExplicitKill = this.mCannotDieWithoutExplicitKill;
      if (this.mAnimation != Magicka.Animations.None)
        instance.ForceAnimation(this.mAnimation);
      if (this.mSpawnAnimation != Magicka.Animations.None && this.mSpawnAnimation != Magicka.Animations.idle && this.mSpawnAnimation != Magicka.Animations.idle_agg)
      {
        instance.SpawnAnimation = this.mSpawnAnimation;
        instance.ChangeState((BaseState) RessurectionState.Instance);
      }
      if (this.mSpecialIdleAnimation != Magicka.Animations.None)
      {
        instance.SpecialIdleAnimation = this.mSpecialIdleAnimation;
        if (!(instance.CurrentState is RessurectionState))
          instance.ForceAnimation(this.mSpecialIdleAnimation);
      }
      if (this.GameScene.RuleSet != null && this.GameScene.RuleSet is SurvivalRuleset)
      {
        instance.Faction = Factions.EVIL;
        (this.GameScene.RuleSet as SurvivalRuleset).AddedCharacter(instance, false);
      }
      this.GameScene.PlayState.EntityManager.AddEntity((Entity) instance);
      this.mSpawnedEntityHP.Add(new Spawn.HandleAndHealth(instance.Handle, instance.HitPoints));
      if (NetworkManager.Instance.State == NetworkState.Server)
        NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
        {
          ActionType = TriggerActionType.SpawnNPC,
          Handle = instance.Handle,
          Template = this.mTypeID,
          Id = this.mUniqueID,
          Position = instance.Position,
          Direction = matrix.Forward,
          Bool0 = this.mForceDraw,
          Point0 = this.mDialogID,
          Point1 = (int) this.mAnimation,
          Point2 = (int) this.mSpawnAnimation,
          Point3 = (int) this.mSpecialIdleAnimation,
          Arg = this.mMeshIdx,
          Color = instance.Color
        });
    }
  }

  public override void QuickExecute()
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(this.mTypeID);
    for (int index = 0; index < this.mSpawnedEntityHP.Count; ++index)
    {
      Spawn.HandleAndHealth handleAndHealth = this.mSpawnedEntityHP[index];
      NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(this.GameScene.PlayState);
      Matrix oLocator;
      this.GameScene.GetLocator(this.mAreaID, out oLocator);
      if (this.mSnapToNavMesh)
      {
        Vector3 translation = oLocator.Translation;
        Vector3 oPoint;
        double nearestPosition = (double) this.GameScene.LevelModel.NavMesh.GetNearestPosition(ref translation, out oPoint, cachedTemplate.MoveAbilities);
        oLocator.Translation = oPoint;
      }
      instance.Initialize(cachedTemplate, this.mMeshIdx, oLocator.Translation, this.mUniqueID);
      instance.OnDeathTrigger = this.mOnDeathTriggerID;
      instance.OnDamageTrigger = this.mOnDamageTriggerID;
      instance.AI.SetOrder(this.mOrder, this.mReactTo, this.mReaction, this.mPriorityTargetHash, this.mPriorityAbility, this.mTriggerID, this.mEvents);
      instance.Dialog = this.mDialogID;
      Matrix matrix = oLocator with
      {
        Translation = new Vector3()
      };
      instance.CharacterBody.Orientation = matrix;
      instance.CharacterBody.DesiredDirection = matrix.Forward;
      instance.RemoveAfterDeath = !this.mNeverRemove;
      if (this.mAnimation != Magicka.Animations.None)
        instance.ForceAnimation(this.mAnimation);
      if (this.mSpawnAnimation != Magicka.Animations.None && this.mSpawnAnimation != Magicka.Animations.idle && this.mSpawnAnimation != Magicka.Animations.idle_agg)
      {
        instance.SpawnAnimation = this.mSpawnAnimation;
        instance.ChangeState((BaseState) RessurectionState.Instance);
      }
      if (this.mSpecialIdleAnimation != Magicka.Animations.None)
        instance.SpecialIdleAnimation = this.mSpecialIdleAnimation;
      if (this.GameScene.RuleSet is SurvivalRuleset)
      {
        instance.Faction = Factions.EVIL;
        (this.GameScene.RuleSet as SurvivalRuleset).AddedCharacter(instance, false);
      }
      this.GameScene.PlayState.EntityManager.AddEntity((Entity) instance);
      instance.HitPoints = handleAndHealth.Health;
      handleAndHealth.Handle = instance.Handle;
      this.mSpawnedEntityHP[index] = handleAndHealth;
      if (NetworkManager.Instance.State == NetworkState.Server)
        NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
        {
          ActionType = TriggerActionType.SpawnNPC,
          Handle = instance.Handle,
          Template = this.mTypeID,
          Id = this.mUniqueID,
          Position = instance.Position,
          Direction = matrix.Forward,
          Point0 = this.mDialogID,
          Point1 = (int) this.mAnimation,
          Point2 = (int) this.mSpawnAnimation,
          Point3 = (int) this.mSpecialIdleAnimation
        });
    }
  }

  public override void Update(float iDeltaTime)
  {
    for (int index = 0; index < this.mSpawnedEntityHP.Count; ++index)
    {
      Spawn.HandleAndHealth handleAndHealth = this.mSpawnedEntityHP[index];
      if (handleAndHealth.Handle != ushort.MaxValue)
      {
        Character fromHandle = Entity.GetFromHandle((int) handleAndHealth.Handle) as Character;
        handleAndHealth.Health = fromHandle.HitPoints;
        if ((double) handleAndHealth.Health <= 0.0)
        {
          this.mSpawnedEntityHP.RemoveAt(index);
          --index;
        }
        else
          this.mSpawnedEntityHP[index] = handleAndHealth;
      }
    }
    base.Update(iDeltaTime);
  }

  public string Area
  {
    get => this.mArea;
    set
    {
      this.mArea = value;
      this.mAreaID = this.mArea.GetHashCodeCustom();
    }
  }

  public bool SnapToNavMesh
  {
    get => this.mSnapToNavMesh;
    set => this.mSnapToNavMesh = value;
  }

  public string TargetArea
  {
    get => this.mTargetArea;
    set
    {
      this.mTargetArea = value;
      this.mTargetAreaID = this.mTargetArea.GetHashCodeCustom();
      if (this.mTargetAreaID != GiveOrder.ANYID)
        return;
      this.mTargetAreaID = 0;
    }
  }

  public float Speed
  {
    get => this.mWanderSpeed;
    set => this.mWanderSpeed = value;
  }

  public string Type
  {
    get => this.mType;
    set
    {
      this.mType = value;
      this.mTypeID = this.mType.GetHashCodeCustom();
    }
  }

  public int Nr
  {
    get => this.mAmount;
    set => this.mAmount = value;
  }

  public float Health
  {
    get => this.mHealth;
    set
    {
      this.mHealth = value;
      if ((double) this.mHealth > 1.0 || (double) this.mHealth < 0.0)
        throw new Exception("Health must be between 0.0 and 1.0!");
    }
  }

  public string ID
  {
    get => this.mUniqueName;
    set
    {
      this.mUniqueName = value;
      this.mUniqueID = this.mUniqueName.GetHashCodeCustom();
    }
  }

  public string Dialog
  {
    get => this.mDialog;
    set
    {
      this.mDialog = value;
      this.mDialogID = this.mDialog.GetHashCodeCustom();
    }
  }

  public Order Order
  {
    get => this.mOrder;
    set => this.mOrder = value;
  }

  public ReactTo ReactTo
  {
    get => this.mReactTo;
    set => this.mReactTo = value;
  }

  public Order Reaction
  {
    get => this.mReaction;
    set => this.mReaction = value;
  }

  public string Trigger
  {
    get => this.mTrigger;
    set
    {
      this.mTrigger = value;
      this.mTriggerID = this.mTrigger.GetHashCodeCustom();
    }
  }

  public bool NeverRemove
  {
    get => this.mNeverRemove;
    set => this.mNeverRemove = value;
  }

  public Magicka.Animations Animation
  {
    get => this.mAnimation;
    set => this.mAnimation = value;
  }

  public Magicka.Animations IdleAnimation
  {
    get => this.mSpecialIdleAnimation;
    set => this.mSpecialIdleAnimation = value;
  }

  public Magicka.Animations SpawnAnimation
  {
    get => this.mSpawnAnimation;
    set => this.mSpawnAnimation = value;
  }

  protected override object Tag
  {
    get
    {
      return (object) new List<Spawn.HandleAndHealth>((IEnumerable<Spawn.HandleAndHealth>) this.mSpawnedEntityHP);
    }
    set
    {
      this.mSpawnedEntityHP = new List<Spawn.HandleAndHealth>((IEnumerable<Spawn.HandleAndHealth>) (value as List<Spawn.HandleAndHealth>));
    }
  }

  protected override void WriteTag(BinaryWriter iWriter, object mTag)
  {
    List<Spawn.HandleAndHealth> handleAndHealthList = mTag as List<Spawn.HandleAndHealth>;
    iWriter.Write(handleAndHealthList.Count);
    foreach (Spawn.HandleAndHealth handleAndHealth in handleAndHealthList)
      iWriter.Write(handleAndHealth.Health);
  }

  protected override object ReadTag(BinaryReader iReader)
  {
    int capacity = iReader.ReadInt32();
    List<Spawn.HandleAndHealth> handleAndHealthList = new List<Spawn.HandleAndHealth>(capacity);
    for (int index = 0; index < capacity; ++index)
      handleAndHealthList.Add(new Spawn.HandleAndHealth(ushort.MaxValue, iReader.ReadSingle()));
    return (object) handleAndHealthList;
  }

  public string PriorityTarget
  {
    set
    {
      this.mPriorityTarget = value;
      this.mPriorityTargetHash = this.mPriorityTarget.GetHashCodeCustom();
    }
  }

  public int PriorityAbility
  {
    set => this.mPriorityAbility = value;
  }

  public string OnDeath
  {
    get => this.mOnDeathTrigger;
    set
    {
      this.mOnDeathTrigger = value;
      this.mOnDeathTriggerID = value.GetHashCodeCustom();
    }
  }

  public string OnDamage
  {
    get => this.mOnDamageTrigger;
    set
    {
      this.mOnDamageTrigger = value;
      this.mOnDamageTriggerID = value.GetHashCodeCustom();
    }
  }

  public bool ForceDraw
  {
    get => this.mForceDraw;
    set => this.mForceDraw = value;
  }

  public int MeshId
  {
    get => this.mMeshIdx;
    set => this.mMeshIdx = value;
  }

  public bool ForceCamera
  {
    get => this.mForceCamera;
    set => this.mForceCamera = value;
  }

  public bool ForceNavMesh
  {
    get => this.mForceNavMesh;
    set => this.mForceNavMesh = value;
  }

  public bool CannotDieWithoutExplicitKill
  {
    get => this.mCannotDieWithoutExplicitKill;
    set => this.mCannotDieWithoutExplicitKill = value;
  }

  private struct HandleAndHealth(ushort iHandle, float iHealth)
  {
    public ushort Handle = iHandle;
    public float Health = iHealth;
  }
}
