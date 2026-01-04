// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.EnterScene
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.AI;
using Magicka.GameLogic;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class EnterScene(Magicka.Levels.Triggers.Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private string mScene;
  private Transitions mTransition = Transitions.Fade;
  private float mTransitionTime = 1f;
  private string mSpawnPointName;
  private Magicka.Levels.SpawnPoint mSpawnPoint;
  private string mMoveTargetName;
  private int[] mMoveTargetIDs = new int[4];
  private bool mFacingDirection;
  private string mInstantTrigger;
  private int mInstantTriggerID;
  private string mMoveTrigger;
  private int mMoveTriggerID;
  private bool mIgnoreTriggers;
  private bool mSaveNPCs;

  protected override unsafe void Execute()
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    this.mSpawnPoint.SpawnPlayers = true;
    if (NetworkManager.Instance.State == NetworkState.Server)
    {
      TriggerActionMessage iMessage = new TriggerActionMessage();
      iMessage.ActionType = TriggerActionType.EnterScene;
      Magicka.Levels.SpawnPoint mSpawnPoint = this.mSpawnPoint;
      iMessage.Scene = mSpawnPoint.Scene;
      // ISSUE: reference to a compiler-generated field
      iMessage.Point0 = mSpawnPoint.Locations.FixedElementField;
      iMessage.Point1 = mSpawnPoint.Locations[1];
      iMessage.Point2 = mSpawnPoint.Locations[2];
      iMessage.Point3 = mSpawnPoint.Locations[3];
      iMessage.Target0 = this.mMoveTargetIDs[0];
      iMessage.Target1 = this.mMoveTargetIDs[1];
      iMessage.Target2 = this.mMoveTargetIDs[2];
      iMessage.Target3 = this.mMoveTargetIDs[3];
      iMessage.Bool0 = this.mIgnoreTriggers;
      iMessage.Bool1 = this.mSaveNPCs;
      iMessage.Bool2 = this.mFacingDirection;
      iMessage.Arg = this.mMoveTriggerID;
      iMessage.Template = (int) this.mTransition;
      iMessage.Time = this.mTransitionTime;
      (NetworkManager.Instance.Interface as NetworkServer).SetAllClientsBusy();
      NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref iMessage);
    }
    this.GameScene.Level.GoToScene(this.mSpawnPoint, this.mTransition, this.mTransitionTime, this.mSaveNPCs, new System.Action(this.OnSceneChanged));
  }

  private void OnSceneChanged()
  {
    if (!string.IsNullOrEmpty(this.mMoveTargetName))
    {
      GameScene currentScene = this.GameScene.Level.CurrentScene;
      AIEvent aiEvent = new AIEvent();
      aiEvent.EventType = AIEventType.Move;
      aiEvent.MoveEvent.Delay = 0.0f;
      aiEvent.MoveEvent.Speed = 1f;
      aiEvent.MoveEvent.FixedDirection = this.mFacingDirection;
      aiEvent.MoveEvent.Trigger = this.mMoveTriggerID;
      Player[] players = Magicka.Game.Instance.Players;
      for (int index = 0; index < players.Length; ++index)
      {
        Player player = players[index];
        Matrix oLocator;
        if (player.Playing && player.Avatar != null && currentScene.TryGetLocator(this.mMoveTargetIDs[index], out oLocator))
        {
          aiEvent.MoveEvent.Waypoint = oLocator.Translation;
          aiEvent.MoveEvent.Direction = oLocator.Forward;
          player.Avatar.Events = new AIEvent[1]{ aiEvent };
          player.Avatar.IgnoreTriggers = this.mIgnoreTriggers;
        }
      }
    }
    if (string.IsNullOrEmpty(this.mInstantTrigger))
      return;
    this.GameScene.Level.CurrentScene.ExecuteTrigger(this.mInstantTriggerID, (Magicka.GameLogic.Entities.Character) null, false);
  }

  public override void QuickExecute()
  {
  }

  public string Scene
  {
    get => this.mScene;
    set
    {
      this.mScene = value;
      this.mSpawnPoint.Scene = this.mScene.GetHashCodeCustom();
    }
  }

  public Transitions Transition
  {
    get => this.mTransition;
    set => this.mTransition = value;
  }

  public float TransitionTime
  {
    get => this.mTransitionTime;
    set => this.mTransitionTime = value;
  }

  public unsafe string SpawnPoint
  {
    get => this.mSpawnPointName;
    set
    {
      this.mSpawnPointName = value;
      fixed (int* numPtr = this.mSpawnPoint.Locations)
      {
        for (int index = 0; index < 4; ++index)
          numPtr[index] = (this.mSpawnPointName + (object) index).GetHashCodeCustom();
      }
    }
  }

  public string MoveTarget
  {
    get => this.mMoveTargetName;
    set
    {
      this.mMoveTargetName = value;
      for (int index = 0; index < this.mMoveTargetIDs.Length; ++index)
        this.mMoveTargetIDs[index] = (value + (object) index).GetHashCodeCustom();
    }
  }

  public bool FacingDirection
  {
    get => this.mFacingDirection;
    set => this.mFacingDirection = value;
  }

  public string InstantTrigger
  {
    get => this.mInstantTrigger;
    set
    {
      this.mInstantTrigger = value;
      this.mInstantTriggerID = value.GetHashCodeCustom();
    }
  }

  public string Trigger
  {
    get => this.mMoveTrigger;
    set
    {
      this.mMoveTrigger = value;
      this.mMoveTriggerID = value.GetHashCodeCustom();
    }
  }

  public bool IgnoreTriggers
  {
    get => this.mIgnoreTriggers;
    set => this.mIgnoreTriggers = value;
  }

  public bool SaveNPCs
  {
    get => this.mSaveNPCs;
    set => this.mSaveNPCs = value;
  }
}
