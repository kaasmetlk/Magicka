// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Interactable
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using Magicka.Levels.Triggers.Conditions;
using Magicka.Network;
using System;
using System.Collections.Generic;
using System.Xml;

#nullable disable
namespace Magicka.Levels.Triggers;

public class Interactable : Trigger
{
  private InteractType mInteractType;
  private bool mEnabled = true;
  private List<string> mAnimHighlightNames;
  private int[][] mAnimHighlightIDs;
  private List<string> mPhysHighlightNames;
  private int[] mPhysHighlightIDs;

  public Interactable(XmlNode iNode, GameScene iScene)
    : base(iScene)
  {
    this.mAnimHighlightNames = new List<string>(1);
    this.mPhysHighlightNames = new List<string>(1);
    for (int i = 0; i < iNode.Attributes.Count; ++i)
    {
      XmlAttribute attribute = iNode.Attributes[i];
      if (attribute.Name.Equals("locator", StringComparison.OrdinalIgnoreCase))
      {
        this.mIDString = attribute.Value.ToLowerInvariant();
        this.mID = this.mIDString.GetHashCodeCustom();
      }
      else if (attribute.Name.Equals("type", StringComparison.OrdinalIgnoreCase))
        this.mInteractType = (InteractType) Enum.Parse(typeof (InteractType), attribute.Value, true);
      else if (attribute.Name.Equals("enabled", StringComparison.OrdinalIgnoreCase))
        this.mEnabled = bool.Parse(attribute.Value);
      else if (attribute.Name.Equals("animhighlight", StringComparison.OrdinalIgnoreCase))
      {
        string str1 = attribute.Value.ToLowerInvariant().Replace(" ", "");
        char[] chArray = new char[1]{ ',' };
        foreach (string str2 in str1.Split(chArray))
          this.mAnimHighlightNames.Add(str2);
      }
      else
      {
        string str3 = attribute.Name.Equals("physhighlight", StringComparison.OrdinalIgnoreCase) ? attribute.Value.ToLowerInvariant().Replace(" ", "") : throw new Exception($"Unexpected attribute \"{attribute.Name}\" in \"{iNode.Name}\"!");
        char[] chArray = new char[1]{ ',' };
        foreach (string str4 in str3.Split(chArray))
          this.mPhysHighlightNames.Add(str4);
      }
    }
    this.mAnimHighlightIDs = new int[this.mAnimHighlightNames.Count][];
    for (int index1 = 0; index1 < this.mAnimHighlightNames.Count; ++index1)
    {
      string[] strArray = this.mAnimHighlightNames[index1].Split('/');
      this.mAnimHighlightIDs[index1] = new int[strArray.Length];
      for (int index2 = 0; index2 < strArray.Length; ++index2)
        this.mAnimHighlightIDs[index1][index2] = strArray[index2].GetHashCodeCustom();
    }
    this.mPhysHighlightIDs = new int[this.mPhysHighlightNames.Count];
    for (int index = 0; index < this.mPhysHighlightNames.Count; ++index)
      this.mPhysHighlightIDs[index] = this.mPhysHighlightNames[index].GetHashCodeCustom();
    this.mConditions = new Condition[0][];
    this.mAutoRun = false;
    this.mActions = Trigger.ReadActions(iScene, (Trigger) this, iNode);
  }

  public void Interact(Character iCharacter)
  {
    if (!this.mEnabled)
      return;
    if (NetworkManager.Instance.State == NetworkState.Client)
    {
      NetworkManager.Instance.Interface.SendMessage<TriggerRequestMessage>(ref new TriggerRequestMessage()
      {
        Handle = iCharacter != null ? iCharacter.Handle : ushort.MaxValue,
        Id = this.mID,
        Scene = this.mGameScene.ID
      }, 0);
    }
    else
    {
      int index1 = Trigger.sRandom.Next(this.mActions.Length);
      Magicka.Levels.Triggers.Actions.Action[] mAction = this.mActions[index1];
      if (NetworkManager.Instance.State == NetworkState.Server)
        NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
        {
          ActionType = TriggerActionType.TriggerExecute,
          Id = this.mID,
          Scene = this.mGameScene.ID,
          Arg = index1,
          Handle = iCharacter != null ? iCharacter.Handle : ushort.MaxValue
        });
      for (int index2 = 0; index2 < mAction.Length; ++index2)
        mAction[index2].OnTrigger(iCharacter);
    }
  }

  public void Highlight()
  {
    for (int index1 = 0; index1 < this.mAnimHighlightIDs.Length; ++index1)
    {
      int[] mAnimHighlightId = this.mAnimHighlightIDs[index1];
      AnimatedLevelPart animatedLevelPart = this.mGameScene.LevelModel.GetAnimatedLevelPart(mAnimHighlightId[0]);
      for (int index2 = 1; index2 < mAnimHighlightId.Length; ++index2)
        animatedLevelPart = animatedLevelPart.GetChild(mAnimHighlightId[index2]);
      animatedLevelPart.Highlight(0.0f);
    }
    for (int index = 0; index < this.mPhysHighlightIDs.Length; ++index)
    {
      if (Entity.GetByID(this.mPhysHighlightIDs[index]) is PhysicsEntity byId)
        byId.Highlight(0.0f);
    }
  }

  public bool Enabled
  {
    get => this.mEnabled;
    set => this.mEnabled = value;
  }

  public InteractType InteractType => this.mInteractType;

  public Locator Locator
  {
    get
    {
      Locator oLocator;
      this.mGameScene.GetLocator(this.mID, out oLocator);
      return oLocator;
    }
  }

  public override Trigger.State GetState() => (Trigger.State) new Interactable.State(this);

  public new class State(Interactable iInteractable) : Trigger.State((Trigger) iInteractable)
  {
    private bool mEnabled;

    public override void UpdateState()
    {
      base.UpdateState();
      this.mEnabled = (this.mTrigger as Interactable).mEnabled;
    }

    public override void ApplyState()
    {
      base.ApplyState();
      (this.mTrigger as Interactable).mEnabled = this.mEnabled;
    }
  }
}
