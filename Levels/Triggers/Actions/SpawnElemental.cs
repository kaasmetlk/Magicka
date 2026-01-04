// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.SpawnElemental
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using Magicka.Network;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class SpawnElemental(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private List<ushort> mSpawnedEntities = new List<ushort>(10);
  private bool mSnapToNavMesh;
  private string mArea;
  private int mAreaID;
  private int mAmount;
  private string mUniqueName;
  private int mUniqueID;
  private float mProximity;

  public override void Initialize() => base.Initialize();

  protected override void Execute()
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    for (int index = 0; index < this.mAmount; ++index)
    {
      ElementalEgg instance = ElementalEgg.GetInstance(this.GameScene.PlayState);
      instance.Proximity = this.mProximity;
      Matrix oLocator;
      this.GameScene.GetLocator(this.mAreaID, out oLocator);
      if (this.mSnapToNavMesh)
      {
        Vector3 translation = oLocator.Translation;
        Vector3 oPoint;
        double nearestPosition = (double) this.GameScene.LevelModel.NavMesh.GetNearestPosition(ref translation, out oPoint, MovementProperties.Default);
        oLocator.Translation = oPoint;
      }
      Vector3 translation1 = oLocator.Translation;
      Vector3 forward = oLocator.Forward;
      instance.Initialize(ref translation1, ref forward, this.mUniqueID);
      this.GameScene.PlayState.EntityManager.AddEntity((Entity) instance);
      this.mSpawnedEntities.Add(instance.Handle);
      if (NetworkManager.Instance.State == NetworkState.Server)
        NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
        {
          ActionType = TriggerActionType.SpawnElemental,
          Handle = instance.Handle,
          Time = this.mProximity,
          Id = this.mUniqueID,
          Position = translation1,
          Direction = forward
        });
    }
  }

  public override void QuickExecute()
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    for (int index = 0; index < this.mSpawnedEntities.Count; ++index)
    {
      ElementalEgg instance = ElementalEgg.GetInstance(this.GameScene.PlayState);
      Matrix oLocator;
      this.GameScene.GetLocator(this.mAreaID, out oLocator);
      if (this.mSnapToNavMesh)
      {
        Vector3 translation = oLocator.Translation;
        Vector3 oPoint;
        double nearestPosition = (double) this.GameScene.LevelModel.NavMesh.GetNearestPosition(ref translation, out oPoint, MovementProperties.Default);
        oLocator.Translation = oPoint;
      }
      Vector3 translation1 = oLocator.Translation;
      Vector3 forward = oLocator.Forward;
      instance.Initialize(ref translation1, ref forward, this.mUniqueID);
      this.GameScene.PlayState.EntityManager.AddEntity((Entity) instance);
      this.mSpawnedEntities[index] = instance.Handle;
      if (NetworkManager.Instance.State == NetworkState.Server)
        NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
        {
          ActionType = TriggerActionType.SpawnElemental,
          Handle = instance.Handle,
          Id = this.mUniqueID,
          Position = translation1,
          Direction = forward
        });
    }
  }

  public override void Update(float iDeltaTime)
  {
    for (int index = 0; index < this.mSpawnedEntities.Count; ++index)
    {
      if ((Entity.GetFromHandle((int) this.mSpawnedEntities[index]) as ElementalEgg).Dead)
        this.mSpawnedEntities.RemoveAt(index--);
    }
    base.Update(iDeltaTime);
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

  public int Nr
  {
    get => this.mAmount;
    set => this.mAmount = value;
  }

  public float Proximity
  {
    get => this.mProximity;
    set => this.mProximity = value;
  }
}
