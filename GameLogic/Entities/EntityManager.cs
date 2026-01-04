// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.EntityManager
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities;

public class EntityManager
{
  public const int MAXENTITIES = 512 /*0x0200*/;
  public const int QUADCELLSIZE = 8;
  public const int QUADCELLS = 16 /*0x10*/;
  private StaticList<Entity> mEntities;
  private List<Entity>[,] mQuadGrid;
  private List<Shield> mShields;
  private List<Barrier> mBarriers;
  private Queue<List<Entity>> mQuaryLists;
  private PlayState mPlayState;

  public EntityManager(PlayState iPlayState)
  {
    this.mEntities = (StaticList<Entity>) new StaticObjectList<Entity>(512 /*0x0200*/);
    this.mShields = new List<Shield>(256 /*0x0100*/);
    this.mBarriers = new List<Barrier>(256 /*0x0100*/);
    this.mPlayState = iPlayState;
    MissileEntity.InitializeCache(128 /*0x80*/, this.mPlayState);
    NonPlayerCharacter.InitializeCache(192 /*0xC0*/, this.mPlayState);
    ElementalEgg.InitializeCache(128 /*0x80*/, this.mPlayState);
    this.mQuadGrid = new List<Entity>[16 /*0x10*/, 16 /*0x10*/];
    for (int index1 = 0; index1 < 16 /*0x10*/; ++index1)
    {
      for (int index2 = 0; index2 < 16 /*0x10*/; ++index2)
        this.mQuadGrid[index1, index2] = new List<Entity>(512 /*0x0200*/);
    }
    this.mQuaryLists = new Queue<List<Entity>>();
    this.mQuaryLists.Enqueue(new List<Entity>(512 /*0x0200*/));
    this.mQuaryLists.Enqueue(new List<Entity>(512 /*0x0200*/));
    this.mQuaryLists.Enqueue(new List<Entity>(512 /*0x0200*/));
    this.mQuaryLists.Enqueue(new List<Entity>(512 /*0x0200*/));
  }

  private void PlaceEntitiesInGrid()
  {
    Vector2 vector2 = new Vector2();
    for (int iIndex = 0; iIndex < this.mEntities.Count; ++iIndex)
    {
      Entity mEntity = this.mEntities[iIndex];
      vector2.X = mEntity.Position.X * 0.125f;
      vector2.Y = mEntity.Position.Z * 0.125f;
      float val1 = mEntity.Radius * 0.125f;
      float val2 = Math.Min(val1, 8f);
      int num1 = (int) Math.Floor((double) vector2.X - (double) val2);
      int num2 = (int) Math.Floor((double) vector2.X + (double) val2);
      for (int index1 = num1; index1 <= num2; ++index1)
      {
        float num3 = (double) Math.Abs((float) index1 - vector2.X) >= (double) Math.Abs((float) (index1 + 1) - vector2.X) ? (float) Math.Sqrt((double) val1 * (double) val1 - ((double) (index1 + 1) - (double) vector2.X) * ((double) (index1 + 1) - (double) vector2.X)) : (float) Math.Sqrt((double) val1 * (double) val1 - ((double) index1 - (double) vector2.X) * ((double) index1 - (double) vector2.X));
        if (float.IsNaN(num3))
          num3 = val1;
        int num4 = (int) Math.Floor((double) vector2.Y - (double) Math.Min(num3, val2));
        int num5 = (int) Math.Floor((double) vector2.Y + (double) Math.Min(num3, val2));
        int index2 = index1 % 16 /*0x10*/;
        if (index2 < 0)
          index2 += 16 /*0x10*/;
        for (int index3 = num4; index3 <= num5; ++index3)
        {
          int index4 = index3 % 16 /*0x10*/;
          if (index4 < 0)
            index4 += 16 /*0x10*/;
          List<Entity> entityList = this.mQuadGrid[index2, index4];
          lock (entityList)
            entityList.Add(mEntity);
        }
      }
    }
  }

  public void UpdateQuadGrid()
  {
    lock (this.mQuadGrid)
    {
      for (int index1 = 0; index1 < 16 /*0x10*/; ++index1)
      {
        for (int index2 = 0; index2 < 16 /*0x10*/; ++index2)
          this.mQuadGrid[index1, index2].Clear();
      }
      this.PlaceEntitiesInGrid();
    }
  }

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if (iDataChannel == DataChannel.None)
      return;
    lock (this.mEntities)
    {
      for (int iIndex = 0; iIndex < this.mEntities.Count; ++iIndex)
        this.mEntities[iIndex].Update(iDataChannel, iDeltaTime);
    }
  }

  public void UpdateNetwork(int iClientIndex, float iPrediction)
  {
    int num = 0;
    NetworkState state = NetworkManager.Instance.State;
    NetworkInterface networkInterface = NetworkManager.Instance.Interface;
    for (int iIndex = 0; iIndex < this.mEntities.Count; ++iIndex)
    {
      Entity mEntity = this.mEntities[iIndex];
      EntityUpdateMessage oMsg;
      if (mEntity.GetNetworkUpdate(out oMsg, state, iPrediction))
      {
        ++num;
        oMsg.Handle = mEntity.Handle;
        networkInterface.SendUdpMessage<EntityUpdateMessage>(ref oMsg, iClientIndex);
      }
    }
  }

  public void RemoveDeadEntities()
  {
    NetworkManager instance = NetworkManager.Instance;
    for (int iIndex = 0; iIndex < this.mEntities.Count; ++iIndex)
    {
      Entity mEntity = this.mEntities[iIndex];
      if (mEntity.Removable)
      {
        if (instance.State == NetworkState.Server)
        {
          EntityRemoveMessage iMessage;
          iMessage.Handle = mEntity.Handle;
          instance.Interface.SendMessage<EntityRemoveMessage>(ref iMessage);
        }
        this.RemoveEntity(iIndex, mEntity);
        --iIndex;
      }
    }
  }

  public void ReturnEntityList(List<Entity> iList)
  {
    if (iList == null)
      throw new ArgumentException("Argument cannot be null", nameof (iList));
    if (this.mQuaryLists.Contains(iList))
      throw new Exception("This list is already present in the cache!");
    lock (this.mQuaryLists)
      this.mQuaryLists.Enqueue(iList);
  }

  public IDamageable GetClosestIDamageable(
    IDamageable iCaster,
    Vector3 iCenter,
    float iRadius,
    bool iIgnoreProtectedEntities)
  {
    int num1 = (int) Math.Floor(((double) iCenter.X - (double) iRadius) * 0.125);
    int num2 = (int) Math.Floor(((double) iCenter.X + (double) iRadius) * 0.125);
    int num3 = (int) Math.Floor(((double) iCenter.Z - (double) iRadius) * 0.125);
    int num4 = (int) Math.Floor(((double) iCenter.Z + (double) iRadius) * 0.125);
    if (num2 - num1 > 16 /*0x10*/)
    {
      num1 = 0;
      num2 = 15;
    }
    if (num4 - num3 > 16 /*0x10*/)
    {
      num3 = 0;
      num4 = 15;
    }
    float num5 = float.MaxValue;
    IDamageable closestIdamageable = (IDamageable) null;
    Segment iSeg;
    iSeg.Origin = iCenter;
    for (int index1 = num1; index1 <= num2; ++index1)
    {
      for (int index2 = num3; index2 <= num4; ++index2)
      {
        int index3 = index1 % 16 /*0x10*/;
        int index4 = index2 % 16 /*0x10*/;
        if (index3 < 0)
          index3 += 16 /*0x10*/;
        if (index4 < 0)
          index4 += 16 /*0x10*/;
        List<Entity> entityList = this.mQuadGrid[index3, index4];
        for (int index5 = 0; index5 < entityList.Count; ++index5)
        {
          if (entityList[index5] is IDamageable damageable && !(damageable.Dead | damageable == iCaster))
          {
            Vector3 position = damageable.Position;
            float result;
            Vector3.DistanceSquared(ref position, ref iCenter, out result);
            float num6 = iRadius + damageable.Radius;
            float num7 = num6 * num6;
            if ((double) result <= (double) num7 && (double) result < (double) num5)
            {
              bool flag = false;
              if (iIgnoreProtectedEntities)
              {
                for (int index6 = 0; index6 < this.mShields.Count; ++index6)
                {
                  iSeg.Delta = damageable.Position;
                  Vector3.Subtract(ref iSeg.Delta, ref iSeg.Origin, out iSeg.Delta);
                  if (this.mShields[index6].SegmentIntersect(out Vector3 _, iSeg, 0.5f))
                  {
                    flag = true;
                    break;
                  }
                }
              }
              if (!flag)
              {
                closestIdamageable = damageable;
                num5 = result;
              }
            }
          }
        }
      }
    }
    return closestIdamageable;
  }

  public List<Entity> GetEntities(Vector3 iCenter, float iRadius, bool iIgnoreProtectedEntities)
  {
    return this.GetEntities(iCenter, iRadius, iIgnoreProtectedEntities, false);
  }

  public List<Entity> GetEntities(
    Vector3 iCenter,
    float iRadius,
    bool iIgnoreProtectedEntities,
    bool iIgnoreYAxis)
  {
    List<Entity> entities;
    lock (this.mQuaryLists)
      entities = this.mQuaryLists.Count <= 0 ? new List<Entity>(512 /*0x0200*/) : this.mQuaryLists.Dequeue();
    entities.Clear();
    if (iIgnoreYAxis)
      iCenter.Y = 0.0f;
    int num1 = (int) Math.Floor(((double) iCenter.X - (double) iRadius) * 0.125);
    int num2 = (int) Math.Floor(((double) iCenter.X + (double) iRadius) * 0.125);
    int num3 = (int) Math.Floor(((double) iCenter.Z - (double) iRadius) * 0.125);
    int num4 = (int) Math.Floor(((double) iCenter.Z + (double) iRadius) * 0.125);
    if (num2 - num1 > 16 /*0x10*/)
    {
      num1 = 0;
      num2 = 15;
    }
    if (num4 - num3 > 16 /*0x10*/)
    {
      num3 = 0;
      num4 = 15;
    }
    float result;
    lock (this.mQuadGrid)
    {
      for (int index1 = num1; index1 <= num2; ++index1)
      {
        for (int index2 = num3; index2 <= num4; ++index2)
        {
          int index3 = index1 % 16 /*0x10*/;
          int index4 = index2 % 16 /*0x10*/;
          if (index3 < 0)
            index3 += 16 /*0x10*/;
          if (index4 < 0)
            index4 += 16 /*0x10*/;
          List<Entity> entityList = this.mQuadGrid[index3, index4];
          int count = entityList.Count;
          for (int index5 = 0; index5 < count; ++index5)
          {
            Entity entity = entityList[index5];
            if (!entity.Dead)
            {
              Vector3 position = entity.Position;
              if (iIgnoreYAxis)
                position.Y = 0.0f;
              Vector3.DistanceSquared(ref position, ref iCenter, out result);
              float num5 = iRadius + entity.Radius;
              float num6 = num5 * num5;
              if ((double) result <= (double) num6 && !entities.Contains(entity))
                entities.Add(entity);
            }
          }
        }
      }
    }
    if (iIgnoreProtectedEntities)
    {
      Segment iSeg;
      iSeg.Origin = iCenter;
      for (int index6 = 0; index6 < entities.Count; ++index6)
      {
        if (entities[index6] is Shield shield)
        {
          for (int index7 = 0; index7 < entities.Count; ++index7)
          {
            if (index6 != index7)
            {
              iSeg.Delta = entities[index7].Position;
              Vector3.Subtract(ref iSeg.Delta, ref iSeg.Origin, out iSeg.Delta);
              Vector3 oPosition;
              if (!(entities[index7] is IDamageable) | shield.SegmentIntersect(out oPosition, iSeg, 0.5f))
              {
                Vector3.DistanceSquared(ref oPosition, ref iCenter, out result);
                if ((double) result > 9.9999999747524271E-07)
                {
                  if (index7 < index6)
                    --index6;
                  entities.RemoveAt(index7--);
                }
              }
            }
          }
        }
      }
    }
    return entities;
  }

  public void AddEntity(Entity iEntity)
  {
    lock (this.mEntities)
    {
      if (this.mEntities.Contains(iEntity))
        return;
      this.mEntities.Add(iEntity);
    }
    if (iEntity is PhysicsEntity)
      (iEntity as PhysicsEntity).OnSpawn();
    if (iEntity is Shield)
    {
      this.mShields.Add(iEntity as Shield);
    }
    else
    {
      if (!(iEntity is Barrier))
        return;
      this.mBarriers.Add(iEntity as Barrier);
    }
  }

  private void RemoveEntity(int iIndex, Entity iEntity)
  {
    this.mEntities.RemoveAt(iIndex);
    iEntity.Deinitialize();
    switch (iEntity)
    {
      case Shield _:
        this.mShields.Remove(iEntity as Shield);
        break;
      case Barrier _:
        this.mBarriers.Remove(iEntity as Barrier);
        break;
    }
  }

  public void Clear()
  {
    while (this.mEntities.Count > 0)
      this.RemoveEntity(this.mEntities.Count - 1, this.mEntities[this.mEntities.Count - 1]);
    this.Shields.Clear();
    this.Barriers.Clear();
  }

  public StaticList<Entity> Entities => this.mEntities;

  public void GetCharacters(ref List<Character> chars)
  {
    int count = this.mEntities.Count;
    for (int iIndex = 0; iIndex < count; ++iIndex)
    {
      Character mEntity;
      try
      {
        mEntity = this.mEntities[iIndex] as Character;
      }
      catch
      {
        continue;
      }
      if (mEntity != null)
        chars.Add(mEntity);
    }
  }

  public List<Barrier> Barriers => this.mBarriers;

  public List<Shield> Shields => this.mShields;

  public void ClearAndStore(List<Entity> iStoreTarget)
  {
    for (int iIndex = 0; iIndex < this.mEntities.Count; ++iIndex)
    {
      Entity mEntity = this.mEntities[iIndex];
      mEntity.Body.DisableBody();
      if (!(mEntity is Avatar))
      {
        if (iStoreTarget != null && !mEntity.Removable)
        {
          switch (mEntity)
          {
            case PhysicsEntity _:
              iStoreTarget.Add(mEntity);
              continue;
            case NonPlayerCharacter nonPlayerCharacter when !nonPlayerCharacter.IsSummoned && ((double) nonPlayerCharacter.HitPoints > 0.0 || nonPlayerCharacter.Undying):
              nonPlayerCharacter.AI.Disable();
              iStoreTarget.Add(mEntity);
              continue;
            case Pickable pickable when pickable.IsPickable:
              iStoreTarget.Add(mEntity);
              continue;
            case ElementalEgg elementalEgg when !elementalEgg.IsSummoned:
              iStoreTarget.Add(mEntity);
              continue;
          }
        }
        mEntity.Deinitialize();
      }
    }
    this.mEntities.Clear();
  }

  public bool IsProtectedByShield(Entity iEntity, out Shield oShield)
  {
    Vector3 position1 = iEntity.Position;
    for (int index = 0; index < this.mShields.Count; ++index)
    {
      Shield mShield = this.mShields[index];
      Vector3 position2 = mShield.Position;
      float result;
      Vector3.DistanceSquared(ref position1, ref position2, out result);
      if (mShield.ShieldType == ShieldType.SPHERE & (double) result < (double) mShield.Radius * (double) mShield.Radius)
      {
        oShield = mShield;
        return true;
      }
    }
    oShield = (Shield) null;
    return false;
  }
}
