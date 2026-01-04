// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.ChangeScene
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Network;
using PolygonHead;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class ChangeScene(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private string mSceneString;
  private Transitions mTransition = Transitions.Fade;
  private float mTransitionTime = 1f;
  private string mSpawnPointName;
  private Magicka.Levels.SpawnPoint mSpawnPoint;
  private bool mSaveNPCs;

  protected override unsafe void Execute()
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    if (NetworkManager.Instance.State == NetworkState.Server)
    {
      TriggerActionMessage iMessage = new TriggerActionMessage();
      iMessage.ActionType = TriggerActionType.ChangeScene;
      Magicka.Levels.SpawnPoint mSpawnPoint = this.mSpawnPoint;
      iMessage.Scene = mSpawnPoint.Scene;
      // ISSUE: reference to a compiler-generated field
      iMessage.Point0 = mSpawnPoint.Locations.FixedElementField;
      iMessage.Point1 = mSpawnPoint.Locations[1];
      iMessage.Point2 = mSpawnPoint.Locations[2];
      iMessage.Point3 = mSpawnPoint.Locations[3];
      iMessage.Bool0 = mSpawnPoint.SpawnPlayers;
      iMessage.Bool1 = this.mSaveNPCs;
      iMessage.Template = (int) this.mTransition;
      iMessage.Time = this.mTransitionTime;
      (NetworkManager.Instance.Interface as NetworkServer).SetAllClientsBusy();
      NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref iMessage);
    }
    this.GameScene.Level.GoToScene(this.mSpawnPoint, this.mTransition, this.mTransitionTime, this.mSaveNPCs, (System.Action) null);
  }

  public override void QuickExecute()
  {
  }

  public string Scene
  {
    get => this.mSceneString;
    set
    {
      this.mSceneString = value;
      this.mSpawnPoint.Scene = this.mSceneString.GetHashCodeCustom();
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

  public bool SpawnPlayers
  {
    get => this.mSpawnPoint.SpawnPlayers;
    set => this.mSpawnPoint.SpawnPlayers = value;
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

  public bool SaveNPCs
  {
    get => this.mSaveNPCs;
    set => this.mSaveNPCs = value;
  }
}
