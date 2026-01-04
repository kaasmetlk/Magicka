// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.SpawnGenericBoss
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.AI;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.Entities.CharacterStates;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Xml;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class SpawnGenericBoss : Action
{
  private XmlNode mNode;
  private GenericBoss mBossRef;
  private string mArea;
  private int mAreaID;
  private string mType;
  private int mTypeID;
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
  private bool mDelayed;
  private float mHealthAppearDelay;
  private float mFreezeTime;
  private float mHealthBarWidth = 0.8f;
  private bool mCannotDieWithoutExplicitKill;

  public SpawnGenericBoss(Magicka.Levels.Triggers.Trigger iTrigger, GameScene iScene, XmlNode iNode)
    : base(iTrigger, iScene)
  {
    this.mNode = iNode;
  }

  public override void Initialize()
  {
    base.Initialize();
    this.mBossRef = new GenericBoss(this.GameScene.PlayState, this.mTypeID, this.mUniqueID, this.mMeshIdx);
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
    BossFight instance = BossFight.Instance;
    if (!instance.IsSetup)
      BossFight.Instance.Setup(this.GameScene.PlayState, this.mFreezeTime, this.mHealthAppearDelay, this.mHealthBarWidth);
    Matrix oLocator;
    this.GameScene.GetLocator(this.mAreaID, out oLocator);
    instance.Initialize((IBoss) this.mBossRef, this.mAreaID, this.mUniqueID);
    if (this.mForceDraw)
      this.mBossRef.ForceDraw();
    this.mBossRef.HitPoints = this.mBossRef.MaxHitPoints * this.mHealth;
    this.mBossRef.OnDeathTrigger = this.mOnDeathTriggerID;
    this.mBossRef.OnDamageTrigger = this.mOnDamageTriggerID;
    Agent ai = this.mBossRef.AI;
    ai.WanderSpeed = this.mWanderSpeed;
    ai.TargetArea = this.mTargetAreaID;
    ai.SetOrder(this.mOrder, this.mReactTo, this.mReaction, this.mPriorityTargetHash, this.mPriorityAbility, this.mTriggerID, this.mEvents);
    this.mBossRef.Dialog = this.mDialogID;
    Matrix matrix = oLocator with
    {
      Translation = new Vector3()
    };
    this.mBossRef.CharacterBody.Orientation = matrix;
    this.mBossRef.CharacterBody.DesiredDirection = matrix.Forward;
    this.mBossRef.RemoveAfterDeath = !this.mNeverRemove;
    this.mBossRef.ForceCamera = this.mForceCamera;
    this.mBossRef.ForceNavMesh = this.mForceNavMesh;
    this.mBossRef.CannotDieWithoutExplicitKill = this.mCannotDieWithoutExplicitKill;
    if (this.mAnimation != Magicka.Animations.None)
      this.mBossRef.ForceAnimation(this.mAnimation);
    if (this.mSpawnAnimation != Magicka.Animations.None && this.mSpawnAnimation != Magicka.Animations.idle && this.mSpawnAnimation != Magicka.Animations.idle_agg)
    {
      this.mBossRef.SpawnAnimation = this.mSpawnAnimation;
      this.mBossRef.ChangeState((BaseState) RessurectionState.Instance);
    }
    if (this.mSpecialIdleAnimation != Magicka.Animations.None)
    {
      this.mBossRef.SpecialIdleAnimation = this.mSpecialIdleAnimation;
      if (!(this.mBossRef.CurrentState is RessurectionState))
        this.mBossRef.ForceAnimation(this.mSpecialIdleAnimation);
    }
    if (this.GameScene.RuleSet != null && this.GameScene.RuleSet is SurvivalRuleset)
    {
      this.mBossRef.Faction = Factions.EVIL;
      (this.GameScene.RuleSet as SurvivalRuleset).AddedCharacter((NonPlayerCharacter) this.mBossRef, false);
    }
    if (this.mDelayed)
      return;
    instance.Start();
  }

  public override void QuickExecute()
  {
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

  public bool Delayed
  {
    get => this.mDelayed;
    set => this.mDelayed = value;
  }

  public float HealthAppearDelay
  {
    get => this.mHealthAppearDelay;
    set => this.mHealthAppearDelay = value;
  }

  public float FreezeTime
  {
    get => this.mFreezeTime;
    set => this.mFreezeTime = value;
  }

  public float HealthBarWidth
  {
    get => this.mHealthBarWidth;
    set => this.mHealthBarWidth = value;
  }

  public bool CannotDieWithoutExplicitKill
  {
    get => this.mCannotDieWithoutExplicitKill;
    set => this.mCannotDieWithoutExplicitKill = value;
  }
}
