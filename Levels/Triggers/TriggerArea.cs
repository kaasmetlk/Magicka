// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.TriggerArea
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Magicka.GameLogic.Entities;
using Magicka.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

#nullable disable
namespace Magicka.Levels.Triggers;

public class TriggerArea
{
  public static readonly int ANYID = "any".GetHashCodeCustom();
  protected CollisionSkin mCollisionSkin;
  protected int mTotalCharacters;
  protected StaticWeakList<Character> mPresentCharacters;
  protected int mTotalEntities;
  protected Vector3 mCenter;
  protected float mRadius;
  protected StaticWeakList<Entity> mPresentEntities;
  protected Dictionary<int, int> mTypeCount;
  protected Dictionary<int, int> mFactionCount;

  public TriggerArea(Box iBox)
  {
    if (iBox != null)
    {
      this.mCollisionSkin = new CollisionSkin((Body) null);
      this.mCollisionSkin.callbackFn += new CollisionCallbackFn(this.OnCollision);
      this.mCollisionSkin.AddPrimitive((Primitive) iBox, 1, new MaterialProperties(0.0f, 0.8f, 0.8f));
      iBox.GetCentre(out this.mCenter);
      this.mRadius = iBox.SideLengths.Length() * 0.5f;
    }
    this.mTypeCount = new Dictionary<int, int>(10);
    this.mFactionCount = new Dictionary<int, int>(5);
    this.mPresentCharacters = new StaticWeakList<Character>(256 /*0x0100*/);
    this.mPresentEntities = new StaticWeakList<Entity>(256 /*0x0100*/);
  }

  protected bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
  {
    if (iSkin1.Owner != null)
      this.AddEntity(iSkin1.Owner.Tag as Entity);
    else if (iSkin0.Owner != null)
      this.AddEntity(iSkin0.Owner.Tag as Entity);
    return false;
  }

  public int GetCount(int iType)
  {
    if (iType == TriggerArea.ANYID)
      return this.mTotalCharacters;
    int num;
    return this.mTypeCount.TryGetValue(iType, out num) ? num : 0;
  }

  public Vector3 GetRandomLocation()
  {
    Vector3 position = new Vector3();
    Box primitiveLocal = (Box) this.mCollisionSkin.GetPrimitiveLocal(0);
    Vector3 sideLengths = primitiveLocal.SideLengths;
    position.X = MagickaMath.RandomBetween(0.0f, sideLengths.X);
    position.Y = MagickaMath.RandomBetween(0.0f, sideLengths.Y);
    position.Z = MagickaMath.RandomBetween(0.0f, sideLengths.Z);
    return Vector3.Transform(position, primitiveLocal.TransformMatrix);
  }

  public int GetFactionCount(Factions iFaction)
  {
    int factionCount = 0;
    for (int index = 0; index <= 15; ++index)
    {
      int key = 1 << index;
      int num;
      if (((Factions) key & iFaction) != Factions.NONE && this.mFactionCount.TryGetValue(key, out num))
        factionCount += num;
    }
    return factionCount;
  }

  public virtual void Register()
  {
    this.Reset();
    if (PhysicsManager.Instance.Simulator.CollisionSystem.CollisionSkins.Contains(this.mCollisionSkin))
      return;
    PhysicsManager.Instance.Simulator.CollisionSystem.AddCollisionSkin(this.mCollisionSkin);
  }

  public void Reset()
  {
    this.mPresentEntities.Clear();
    this.mTotalEntities = 0;
    this.mPresentCharacters.Clear();
    this.mTotalCharacters = 0;
    this.mTypeCount.Clear();
    this.mFactionCount.Clear();
  }

  public StaticWeakList<Character> PresentCharacters => this.mPresentCharacters;

  public StaticWeakList<Entity> PresentEntities => this.mPresentEntities;

  public static TriggerArea Read(ContentReader iInput)
  {
    Vector3 pos = iInput.ReadVector3();
    Vector3 sideLengths = iInput.ReadVector3();
    Quaternion quaternion = iInput.ReadQuaternion();
    Matrix result;
    Matrix.CreateFromQuaternion(ref quaternion, out result);
    return new TriggerArea(new Box(pos, result, sideLengths));
  }

  public CollisionSkin CollisionSkin => this.mCollisionSkin;

  internal virtual void UpdatePresent(EntityManager iManager)
  {
    this.Reset();
    StaticList<Entity> entities = iManager.Entities;
    Box primitiveNewWorld = this.mCollisionSkin.GetPrimitiveNewWorld(0) as Box;
    for (int iIndex = 0; iIndex < entities.Count; ++iIndex)
    {
      Entity iEntity = entities[iIndex];
      if ((double) primitiveNewWorld.GetDistanceToPoint(out Vector3 _, iEntity.Position) <= (double) iEntity.Radius)
        this.AddEntity(iEntity);
    }
  }

  internal void AddEntity(Entity iEntity)
  {
    if (iEntity is Avatar && (iEntity as Avatar).IgnoreTriggers)
      return;
    this.mPresentEntities.Add(iEntity);
    ++this.mTotalEntities;
    if (!(iEntity is Character iItem))
      return;
    this.mPresentCharacters.Add(iItem);
    if (iItem.Dead && !iItem.Template.Undying)
      return;
    int num;
    if (!this.mTypeCount.TryGetValue(iItem.Type, out num))
      num = 0;
    this.mTypeCount[iItem.Type] = num + 1;
    for (int index = 0; index <= 15; ++index)
    {
      Factions key = (Factions) (1 << index);
      if ((key & iItem.GetOriginalFaction) != Factions.NONE)
      {
        if (!this.mFactionCount.TryGetValue((int) key, out num))
          num = 0;
        this.mFactionCount[(int) key] = num + 1;
      }
    }
    ++this.mTotalCharacters;
  }

  internal Box Box
  {
    get
    {
      return this.mCollisionSkin != null ? this.mCollisionSkin.GetPrimitiveLocal(0) as Box : (Box) null;
    }
  }
}
