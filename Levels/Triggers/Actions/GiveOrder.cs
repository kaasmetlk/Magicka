// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.GiveOrder
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.AI;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;
using Magicka.Network;
using System.Collections.Generic;
using System.Xml;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class GiveOrder : Action
{
  public static readonly int ANYID = "any".GetHashCodeCustom();
  private static PlayState sPlayState;
  private static List<GiveOrder> sInstances = new List<GiveOrder>();
  private ushort mHandle;
  private XmlNode mNode;
  protected Order mOrder;
  protected ReactTo mReactTo;
  protected Order mReaction = Order.Attack;
  protected string mTrigger;
  protected int mTriggerID;
  protected Animations mAnimation;
  protected Animations mIdleAnimation;
  protected string mID;
  protected int mIDHash;
  protected Factions mFactions;
  protected string mType;
  protected int mTypeHash;
  protected string mArea;
  protected int mAreaHash;
  protected string mTargetArea;
  protected int mTargetAreaHash;
  protected float mWanderSpeed = 1f;
  protected bool mIsSpecific;
  internal AIEvent[] mEvents;
  private string mPriorityTarget;
  private int mPriorityTargetHash;
  private int mPriorityAbility = -1;
  private bool mExplicitFaction;

  public GiveOrder(Magicka.Levels.Triggers.Trigger iTrigger, GameScene iScene, XmlNode iNode)
    : base(iTrigger, iScene)
  {
    if (GiveOrder.sPlayState != iScene.PlayState)
    {
      GiveOrder.sPlayState = iScene.PlayState;
      GiveOrder.sInstances.Clear();
    }
    this.mHandle = (ushort) GiveOrder.sInstances.Count;
    GiveOrder.sInstances.Add(this);
    this.mNode = iNode;
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

  protected override void Execute() => this.Exec();

  public static void ExecuteByHandle(ushort iHandle) => GiveOrder.sInstances[(int) iHandle].Exec();

  private void Exec()
  {
    if (this.mIsSpecific)
    {
      if (!(Entity.GetByID(this.mIDHash) is NonPlayerCharacter byId))
        return;
      byId.ReleaseAttachedCharacter();
      if (byId.IsGripped)
        byId.Gripper.ReleaseAttachedCharacter();
      byId.AI.WanderSpeed = this.mWanderSpeed;
      byId.AI.TargetArea = this.mTargetAreaHash;
      byId.AI.SetOrder(this.mOrder, this.mReactTo, this.mReaction, this.mPriorityTargetHash, this.mPriorityAbility, this.mTriggerID, this.mEvents);
      if (this.mIdleAnimation != Animations.None)
        byId.SpecialIdleAnimation = this.mIdleAnimation;
      if (this.mAnimation != Animations.None)
        byId.GoToAnimation(this.mAnimation, 0.2f);
      if (this.mOrder == Order.Panic)
        byId.OrderPanic();
      if (!(this.mAnimation != Animations.None | this.mIdleAnimation != Animations.None) || NetworkManager.Instance.State == NetworkState.Offline)
        return;
      NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
      {
        Handle = byId.Handle,
        Action = ActionType.EventAnimation,
        Param0I = (int) this.mAnimation,
        Param1F = 0.2f,
        Param2I = (int) this.mIdleAnimation
      });
    }
    else
    {
      TriggerArea triggerArea = this.GameScene.GetTriggerArea(this.mAreaHash);
      for (int iIndex = 0; iIndex < triggerArea.PresentCharacters.Count; ++iIndex)
      {
        if (triggerArea.PresentCharacters[iIndex] is NonPlayerCharacter presentCharacter && (this.mTypeHash == GiveOrder.ANYID || presentCharacter.Type == this.mTypeHash || !this.mExplicitFaction && (presentCharacter.GetOriginalFaction & this.mFactions) != Factions.NONE || this.mExplicitFaction && presentCharacter.GetOriginalFaction == this.mFactions))
        {
          presentCharacter.ReleaseAttachedCharacter();
          if (presentCharacter.IsGripped)
            presentCharacter.Gripper.ReleaseAttachedCharacter();
          presentCharacter.AI.WanderSpeed = this.mWanderSpeed;
          presentCharacter.AI.TargetArea = this.mTargetAreaHash;
          presentCharacter.AI.SetOrder(this.mOrder, this.mReactTo, this.mReaction, this.mPriorityTargetHash, this.mPriorityAbility, this.mTriggerID, this.mEvents);
          if (this.mIdleAnimation != Animations.None)
            presentCharacter.SpecialIdleAnimation = this.mIdleAnimation;
          if (this.mAnimation != Animations.None)
            presentCharacter.GoToAnimation(this.mAnimation, 0.2f);
          if (this.mAnimation != Animations.None | this.mIdleAnimation != Animations.None && NetworkManager.Instance.State != NetworkState.Offline)
            NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
            {
              Handle = presentCharacter.Handle,
              Action = ActionType.EventAnimation,
              Param0I = (int) this.mAnimation,
              Param1F = 0.2f,
              Param2I = (int) this.mIdleAnimation
            });
        }
      }
    }
  }

  public override void QuickExecute()
  {
    if (this.mAreaHash != 0)
      this.GameScene.GetTriggerArea(this.mAreaHash).UpdatePresent(this.GameScene.PlayState.EntityManager);
    this.Execute();
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

  public string Trigger
  {
    get => this.mTrigger;
    set
    {
      this.mTrigger = value;
      this.mTriggerID = this.mTrigger.GetHashCodeCustom();
    }
  }

  public Order Reaction
  {
    get => this.mReaction;
    set => this.mReaction = value;
  }

  public Animations Animation
  {
    get => this.mAnimation;
    set => this.mAnimation = value;
  }

  public Animations IdleAnimation
  {
    get => this.mIdleAnimation;
    set => this.mIdleAnimation = value;
  }

  public Factions Factions
  {
    get => this.mFactions;
    set => this.mFactions = value;
  }

  public string ID
  {
    get => this.mID;
    set
    {
      this.mID = value;
      if (!string.IsNullOrEmpty(this.mID))
      {
        this.mIsSpecific = true;
        this.mIDHash = this.mID.GetHashCodeCustom();
      }
      else
      {
        this.mIsSpecific = false;
        this.mIDHash = 0;
      }
    }
  }

  public string Type
  {
    get => this.mType;
    set
    {
      this.mType = value;
      this.mTypeHash = this.mType.GetHashCodeCustom();
    }
  }

  public string Area
  {
    get => this.mArea;
    set
    {
      this.mArea = value;
      this.mAreaHash = this.mArea.GetHashCodeCustom();
    }
  }

  public string TargetArea
  {
    get => this.mTargetArea;
    set
    {
      this.mTargetArea = value;
      this.mTargetAreaHash = this.mTargetArea.GetHashCodeCustom();
      if (this.mTargetAreaHash != GiveOrder.ANYID)
        return;
      this.mTargetAreaHash = 0;
    }
  }

  public float Speed
  {
    get => this.mWanderSpeed;
    set => this.mWanderSpeed = value;
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

  public bool Explicit
  {
    set => this.mExplicitFaction = value;
  }
}
