// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Items.SpawnGibsEvent
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Items;

public struct SpawnGibsEvent
{
  private static Random sRandom = new Random();
  public int StartIndex;
  public int EndIndex;

  public SpawnGibsEvent(ContentReader iInput)
  {
    this.StartIndex = iInput.ReadInt32();
    this.EndIndex = iInput.ReadInt32();
  }

  public void Execute(Entity iItem, Entity iTarget)
  {
    if (!(iItem is Character iOwner))
      return;
    Body body = iOwner.Body;
    Vector3 position = iOwner.Position;
    List<GibReference> gibs = iOwner.Gibs;
    for (int startIndex = this.StartIndex; startIndex <= this.EndIndex; ++startIndex)
    {
      GibReference gibReference = gibs[startIndex];
      Gib fromCache = Gib.GetFromCache();
      if (fromCache != null)
      {
        Vector3 positionOnCollisionSkin = iOwner.GetRandomPositionOnCollisionSkin();
        Vector3 result;
        Vector3.Subtract(ref positionOnCollisionSkin, ref position, out result);
        result.Normalize();
        result.X *= (float) (SpawnGibsEvent.sRandom.NextDouble() * 3.0 + (double) body.Velocity.X * 0.10000000149011612);
        result.Y *= (float) (SpawnGibsEvent.sRandom.NextDouble() * 9.0 + (double) body.Velocity.Y * 0.10000000149011612);
        result.Z *= (float) (SpawnGibsEvent.sRandom.NextDouble() * 3.0 + (double) body.Velocity.Z * 0.10000000149011612);
        fromCache.Initialize(gibReference.mModel, gibReference.mMass, gibReference.mScale, positionOnCollisionSkin, result, (float) (10.0 + SpawnGibsEvent.sRandom.NextDouble() * 10.0), (Entity) iOwner, iOwner.BloodType, Gib.GORE_GIB_TRAIL_EFFECTS[(int) iOwner.BloodType], iOwner.HasStatus(StatusEffects.Frozen));
        Vector3 angImpulse = new Vector3((float) ((SpawnGibsEvent.sRandom.NextDouble() - 0.5) * (double) fromCache.Mass * 0.5), (float) ((SpawnGibsEvent.sRandom.NextDouble() - 0.5) * (double) fromCache.Mass * 0.5), (float) ((SpawnGibsEvent.sRandom.NextDouble() - 0.5) * (double) fromCache.Mass * 0.5));
        fromCache.Body.ApplyBodyAngImpulse(angImpulse);
        iOwner.PlayState.EntityManager.AddEntity((Entity) fromCache);
      }
    }
  }
}
