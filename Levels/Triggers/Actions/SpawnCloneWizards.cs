// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.SpawnCloneWizards
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.AI;
using Magicka.GameLogic;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class SpawnCloneWizards : Action
{
  private string mName = "CloneWizard";
  private CharacterTemplate mTemplate;
  private XmlNode mNode;
  private List<SpawnCloneWizards.HandleAndHealth> mSpawnedEntityHP = new List<SpawnCloneWizards.HandleAndHealth>(4);
  private bool mSnapToNavMesh;
  private string[] mArea = new string[4];
  private int[] mAreaID = new int[4];
  private int mTypeID;
  private int mAmount;
  private string mUniqueName;
  private int mUniqueID;
  private string mDialog;
  private int mDialogID;
  private float mHealth = 1f;
  private Order mOrder = Order.Attack;
  private Order mReaction = Order.Attack;
  private ReactTo mReactTo = ReactTo.Attack | ReactTo.Proximity;
  private string mTrigger;
  private int mTriggerID;
  private AIEvent[] mEvents;
  private Animations mSpawnAnimation;
  private Animations mSpecialIdleAnimation;
  private bool mNeverRemove;
  private string mTargetArea;
  private int mTargetAreaID;
  private float mWanderSpeed = 1f;
  private string mPriorityTarget;
  private int mPriorityTargetHash;
  private int mPriorityAbility = -1;
  private string mOnDeathTrigger;
  private int mOnDeathTriggerID;
  private string mOnDamageTrigger;
  private int mOnDamageTriggerID;

  public SpawnCloneWizards(Magicka.Levels.Triggers.Trigger iTrigger, GameScene iScene, XmlNode iNode)
    : base(iTrigger, iScene)
  {
    this.mNode = iNode;
  }

  public float GetTotalHitPoins()
  {
    float totalHitPoins = 0.0f;
    for (int index = 0; index < this.mAmount; ++index)
      totalHitPoins += this.mTemplate.MaxHitpoints;
    return totalHitPoins;
  }

  public override void Initialize()
  {
    base.Initialize();
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
    NonPlayerCharacter[] nonPlayerCharacterArray = new NonPlayerCharacter[4];
    for (int iRandomOverride = 0; iRandomOverride < Magicka.Game.Instance.Players.Length; ++iRandomOverride)
    {
      Matrix oLocator = Matrix.Identity;
      Player player = Magicka.Game.Instance.Players[iRandomOverride];
      if (player.Playing && player.Avatar != null && !player.Avatar.Dead)
      {
        this.GameScene.GetLocator(this.mAreaID[iRandomOverride], out oLocator);
        nonPlayerCharacterArray[iRandomOverride] = NonPlayerCharacter.GetInstance(this.GameScene.PlayState);
        this.mUniqueID = (this.mName + (object) iRandomOverride).ToLowerInvariant().GetHashCodeCustom();
        this.mTemplate = player.Avatar.Template;
        this.mTypeID = player.Avatar.Template.ID;
        nonPlayerCharacterArray[iRandomOverride].Initialize(this.mTemplate, iRandomOverride, oLocator.Translation, this.mUniqueID);
        nonPlayerCharacterArray[iRandomOverride].Color = Defines.PLAYERCOLORS[(int) player.Color];
        nonPlayerCharacterArray[iRandomOverride].HitPoints = player.Avatar.MaxHitPoints;
        nonPlayerCharacterArray[iRandomOverride].OnDeathTrigger = this.mOnDeathTriggerID;
        nonPlayerCharacterArray[iRandomOverride].OnDamageTrigger = this.mOnDamageTriggerID;
        Agent ai = nonPlayerCharacterArray[iRandomOverride].AI;
        ai.WanderSpeed = this.mWanderSpeed;
        ai.TargetArea = this.mTargetAreaID;
        ai.SetOrder(this.mOrder, this.mReactTo, this.mReaction, this.mPriorityTargetHash, this.mPriorityAbility, this.mTriggerID, this.mEvents);
        nonPlayerCharacterArray[iRandomOverride].Dialog = this.mDialogID;
        Matrix matrix = oLocator with
        {
          Translation = new Vector3()
        };
        nonPlayerCharacterArray[iRandomOverride].CharacterBody.Orientation = matrix;
        nonPlayerCharacterArray[iRandomOverride].CharacterBody.DesiredDirection = matrix.Forward;
        nonPlayerCharacterArray[iRandomOverride].RemoveAfterDeath = !this.mNeverRemove;
        if (this.mSpawnAnimation != Animations.None && this.mSpawnAnimation != Animations.idle && this.mSpawnAnimation != Animations.idle_agg)
        {
          nonPlayerCharacterArray[iRandomOverride].SpawnAnimation = this.mSpawnAnimation;
          nonPlayerCharacterArray[iRandomOverride].ChangeState((BaseState) RessurectionState.Instance);
        }
        if (this.mSpecialIdleAnimation != Animations.None)
        {
          nonPlayerCharacterArray[iRandomOverride].SpecialIdleAnimation = this.mSpecialIdleAnimation;
          if (!(nonPlayerCharacterArray[iRandomOverride].CurrentState is RessurectionState))
            nonPlayerCharacterArray[iRandomOverride].ForceAnimation(this.mSpecialIdleAnimation);
        }
        if (this.GameScene.RuleSet != null && this.GameScene.RuleSet is SurvivalRuleset)
        {
          nonPlayerCharacterArray[iRandomOverride].Faction = Factions.EVIL;
          (this.GameScene.RuleSet as SurvivalRuleset).AddedCharacter(nonPlayerCharacterArray[iRandomOverride], false);
        }
        this.GameScene.PlayState.EntityManager.AddEntity((Entity) nonPlayerCharacterArray[iRandomOverride]);
        this.mSpawnedEntityHP.Add(new SpawnCloneWizards.HandleAndHealth((int) nonPlayerCharacterArray[iRandomOverride].Handle, nonPlayerCharacterArray[iRandomOverride].HitPoints));
        if (NetworkManager.Instance.State == NetworkState.Server)
          NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
          {
            ActionType = TriggerActionType.SpawnNPC,
            Handle = nonPlayerCharacterArray[iRandomOverride].Handle,
            Template = this.mTypeID,
            Id = this.mUniqueID,
            Position = oLocator.Translation,
            Direction = matrix.Forward,
            Point0 = this.mDialogID,
            Point1 = 0,
            Point2 = (int) this.mSpawnAnimation,
            Point3 = (int) this.mSpecialIdleAnimation,
            Arg = iRandomOverride,
            Color = nonPlayerCharacterArray[iRandomOverride].Color
          });
      }
    }
  }

  public override void QuickExecute() => this.Execute();

  public string Area
  {
    get => this.mArea[0];
    set
    {
      for (int index = 0; index < 4; ++index)
      {
        this.mArea[index] = value + (object) index;
        this.mAreaID[index] = this.mArea[index].GetHashCodeCustom();
      }
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

  public Animations Animation
  {
    get => this.mSpecialIdleAnimation;
    set => this.mSpecialIdleAnimation = value;
  }

  public Animations SpawnAnimation
  {
    get => this.mSpawnAnimation;
    set => this.mSpawnAnimation = value;
  }

  protected override object Tag
  {
    get
    {
      return (object) new List<SpawnCloneWizards.HandleAndHealth>((IEnumerable<SpawnCloneWizards.HandleAndHealth>) this.mSpawnedEntityHP);
    }
    set
    {
      this.mSpawnedEntityHP = new List<SpawnCloneWizards.HandleAndHealth>((IEnumerable<SpawnCloneWizards.HandleAndHealth>) (value as List<SpawnCloneWizards.HandleAndHealth>));
    }
  }

  protected override void WriteTag(BinaryWriter iWriter, object mTag)
  {
    List<SpawnCloneWizards.HandleAndHealth> handleAndHealthList = mTag as List<SpawnCloneWizards.HandleAndHealth>;
    iWriter.Write(handleAndHealthList.Count);
    foreach (SpawnCloneWizards.HandleAndHealth handleAndHealth in handleAndHealthList)
      iWriter.Write(handleAndHealth.Health);
  }

  protected override object ReadTag(BinaryReader iReader)
  {
    int capacity = iReader.ReadInt32();
    List<SpawnCloneWizards.HandleAndHealth> handleAndHealthList = new List<SpawnCloneWizards.HandleAndHealth>(capacity);
    for (int index = 0; index < capacity; ++index)
      handleAndHealthList.Add(new SpawnCloneWizards.HandleAndHealth((int) ushort.MaxValue, iReader.ReadSingle()));
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

  private struct HandleAndHealth(int iHandle, float iHealth)
  {
    public int Handle = iHandle;
    public float Health = iHealth;
  }
}
