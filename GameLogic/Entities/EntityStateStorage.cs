// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.EntityStateStorage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace Magicka.GameLogic.Entities;

internal class EntityStateStorage
{
  private List<Pickable.State> mPickableStates = new List<Pickable.State>();
  private List<PhysicsEntity.State> mPhysicsStates = new List<PhysicsEntity.State>();
  private PlayState mPlayState;

  public EntityStateStorage(PlayState iPlayState) => this.mPlayState = iPlayState;

  public void Read(BinaryReader iReader)
  {
    this.mPickableStates.Clear();
    int num1 = iReader.ReadInt32();
    for (int index = 0; index < num1; ++index)
      this.mPickableStates.Add(new Pickable.State(iReader));
    this.mPhysicsStates.Clear();
    int num2 = iReader.ReadInt32();
    for (int index = 0; index < num2; ++index)
      this.mPhysicsStates.Add(new PhysicsEntity.State(iReader));
  }

  public void Write(BinaryWriter iWriter)
  {
    iWriter.Write(this.mPickableStates.Count);
    for (int index = 0; index < this.mPickableStates.Count; ++index)
      this.mPickableStates[index].Write(iWriter);
    iWriter.Write(this.mPhysicsStates.Count);
    for (int index = 0; index < this.mPhysicsStates.Count; ++index)
      this.mPhysicsStates[index].Write(iWriter);
  }

  public void Store(IEnumerable<Entity> iEntity)
  {
    foreach (Entity iEntity1 in iEntity)
      this.Store(iEntity1);
  }

  public void Store(Entity iEntity)
  {
    Pickable iPickable = iEntity as Pickable;
    PhysicsEntity iPhysicsEntity = iEntity as PhysicsEntity;
    if (iPickable != null)
    {
      if (!iPickable.IsPickable || !iPickable.Permanent)
        return;
      this.mPickableStates.Add(new Pickable.State(iPickable));
    }
    else
    {
      if (iPhysicsEntity == null || iPhysicsEntity.Dead)
        return;
      this.mPhysicsStates.Add(new PhysicsEntity.State(iPhysicsEntity));
    }
  }

  public void Restore(List<Entity> iTarget)
  {
    foreach (Pickable.State mPickableState in this.mPickableStates)
    {
      Pickable pickable = mPickableState.Restore(this.mPlayState);
      if (pickable is Item)
        (pickable as Item).Detach();
      pickable.Body.DisableBody();
      iTarget.Add((Entity) pickable);
    }
    foreach (PhysicsEntity.State mPhysicsState in this.mPhysicsStates)
    {
      PhysicsEntity physicsEntity = mPhysicsState.ApplyTo(this.mPlayState);
      physicsEntity.Body.DisableBody();
      iTarget.Add((Entity) physicsEntity);
    }
  }

  public void Clear()
  {
    this.mPickableStates.Clear();
    this.mPhysicsStates.Clear();
  }
}
