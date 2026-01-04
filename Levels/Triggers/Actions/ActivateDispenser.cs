// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.ActivateDispenser
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.AI;
using Magicka.GameLogic.Entities;
using System.Collections.Generic;
using System.Xml;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class ActivateDispenser : Action
{
  public static readonly int ANYID = "any".GetHashCodeCustom();
  private XmlNode mNode;
  protected Order mOrder;
  protected ReactTo mReactTo;
  protected string mID;
  protected int mIDHash;
  protected Dispensers mType;
  protected string mArea;
  protected int mAreaHash;
  protected string mTargetArea;
  protected int mTargetAreaHash;
  protected float mWanderSpeed;
  protected bool mIsSpecific;
  internal AIEvent[] mEvents;

  public ActivateDispenser(Trigger iTrigger, GameScene iScene, XmlNode iNode)
    : base(iTrigger, iScene)
  {
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
    this.mWanderSpeed = 1f;
  }

  protected override void Execute()
  {
    if (this.mIsSpecific)
    {
      if (!(Entity.GetByID(this.mIDHash) is Dispenser byId))
        return;
      byId.Activate();
    }
    else
    {
      TriggerArea triggerArea = this.GameScene.GetTriggerArea(this.mAreaHash);
      for (int index = 0; index < triggerArea.PresentEntities.Count; ++index)
      {
        if (Entity.GetByID(this.mIDHash) is Dispenser byId && byId.mType == this.mType)
          byId.Activate();
      }
    }
  }

  public override void QuickExecute() => this.Execute();

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

  public Dispensers Type
  {
    get => this.mType;
    set => this.mType = value;
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
}
