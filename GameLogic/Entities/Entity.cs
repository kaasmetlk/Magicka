// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Entity
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Magicka.GameLogic.GameStates;
using Magicka.Network;
using Magicka.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PolygonHead;
using SteamWrapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#nullable disable
namespace Magicka.GameLogic.Entities;

public abstract class Entity
{
  private static List<Entity> mInstances;
  private static ReadOnlyCollection<Entity> mProtectedInstances;
  private ushort mHandle;
  protected int mUniqueID;
  protected PlayState mPlayState;
  protected bool mDead;
  protected Body mBody;
  protected float mRadius;
  protected CollisionSkin mCollision;
  protected AudioEmitter mAudioEmitter;
  protected static Dictionary<int, Entity> mUniqueEntities = new Dictionary<int, Entity>(128 /*0x80*/);
  private Dictionary<ulong, ushort> mInBoundUDPStamp = new Dictionary<ulong, ushort>();
  private int mOutBoundUDPStamp = -1;

  public static Entity GetByID(int iID)
  {
    Entity entity = (Entity) null;
    return Entity.mUniqueEntities.TryGetValue(iID, out entity) ? entity : (Entity) null;
  }

  static Entity()
  {
    Entity.mInstances = new List<Entity>(512 /*0x0200*/);
    Entity.mProtectedInstances = new ReadOnlyCollection<Entity>((IList<Entity>) Entity.mInstances);
  }

  protected Entity(PlayState iPlayState)
  {
    this.mAudioEmitter = new AudioEmitter();
    lock (Entity.mInstances)
    {
      this.mHandle = (ushort) Entity.mInstances.Count;
      Entity.mInstances.Add(this);
    }
    this.mPlayState = iPlayState;
  }

  public static Entity GetFromHandle(int iHandle)
  {
    return iHandle >= Entity.mInstances.Count ? (Entity) null : Entity.mInstances[iHandle];
  }

  protected void Initialize(int iUniqueID)
  {
    this.mInBoundUDPStamp.Clear();
    this.mOutBoundUDPStamp = -1;
    this.mUniqueID = iUniqueID;
    if (iUniqueID != 0)
      Entity.mUniqueEntities[iUniqueID] = this;
    this.mDead = false;
    this.mBody.EnableBody();
  }

  public void SetUniqueID(int iUniqueID)
  {
    this.mUniqueID = iUniqueID;
    Entity.mUniqueEntities[iUniqueID] = this;
  }

  protected void Initialize()
  {
    this.mInBoundUDPStamp.Clear();
    this.mOutBoundUDPStamp = -1;
    this.AudioEmitter.Position = this.mBody.Position;
    this.AudioEmitter.Forward = this.mBody.Orientation.Forward;
    this.AudioEmitter.Up = Vector3.Up;
    this.mUniqueID = 0;
    this.mDead = false;
    this.mBody.EnableBody();
  }

  public int OutboundUDPStamp
  {
    get => this.mOutBoundUDPStamp;
    set => this.mOutBoundUDPStamp = value;
  }

  public virtual void Deinitialize()
  {
    Entity entity;
    if (Entity.mUniqueEntities.TryGetValue(this.mUniqueID, out entity) && entity == this)
      Entity.mUniqueEntities.Remove(this.mUniqueID);
    this.mBody.DisableBody();
  }

  public virtual Matrix GetOrientation()
  {
    return this.mBody.Orientation with
    {
      Translation = this.mBody.Position
    };
  }

  public static ReadOnlyCollection<Entity> AllEntities => Entity.mProtectedInstances;

  protected Vector3 SetMass(float mass)
  {
    float mass1;
    Vector3 centerOfMass;
    Matrix inertiaTensorCoM;
    this.mCollision.GetMassProperties(new PrimitiveProperties(PrimitiveProperties.MassDistributionEnum.Solid, PrimitiveProperties.MassTypeEnum.Mass, mass), out mass1, out centerOfMass, out Matrix _, out inertiaTensorCoM);
    this.mBody.BodyInertia = inertiaTensorCoM;
    this.mBody.Mass = mass1;
    return centerOfMass;
  }

  public int UniqueID => this.mUniqueID;

  public virtual void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    Matrix orientation = this.GetOrientation();
    this.mAudioEmitter.Position = orientation.Translation;
    this.mAudioEmitter.Forward = orientation.Forward;
    this.mAudioEmitter.Up = orientation.Up;
  }

  public virtual Vector3 CalcImpulseVelocity(
    Vector3 iDirection,
    float iElevation,
    float iMassPower,
    float iDistance)
  {
    iDistance *= Math.Max(0.0f, (float) Math.Sqrt(1.0 - ((double) this.Body.Mass - (double) iMassPower * 0.25) / (double) iMassPower) * 2f);
    if (float.IsNaN(iDistance) || (double) iDistance < 9.9999999747524271E-07)
      return new Vector3();
    iDirection.Y = 0.0f;
    iDirection.Normalize();
    if (float.IsNaN(iDirection.X))
      return new Vector3();
    Vector3 result1;
    Vector3.Multiply(ref iDirection, iDistance, out result1);
    Vector3 position = this.Position;
    result1.Y = position.Y = 0.0f;
    Vector3.Add(ref result1, ref position, out result1);
    float result2;
    Vector3.Distance(ref result1, ref position, out result2);
    float num1 = iElevation;
    float num2 = iDirection.Y = (float) Math.Sin((double) num1);
    float num3 = (float) Math.Cos((double) num1);
    iDirection.X *= num3;
    iDirection.Z *= num3;
    float scaleFactor = 1.4f * (float) Math.Sqrt((double) PhysicsManager.Instance.Simulator.Gravity.Y * -1.0 * (double) result2 * (double) result2 / (2.0 * ((double) result2 * (double) num2 / (double) num3) * (double) num3 * (double) num3));
    Vector3.Multiply(ref iDirection, scaleFactor, out iDirection);
    return iDirection;
  }

  public bool AddImpulseVelocity(
    Vector3 iDirection,
    float iElevation,
    float iMassPower,
    float iDistance)
  {
    Vector3 iVelocity = this.CalcImpulseVelocity(iDirection, iElevation, iMassPower, iDistance);
    bool flag = (double) iVelocity.LengthSquared() > 9.9999999747524271E-07;
    if (flag)
      this.AddImpulseVelocity(ref iVelocity);
    return flag;
  }

  protected virtual void AddImpulseVelocity(ref Vector3 iVelocity)
  {
    if (this.mBody.Immovable)
      return;
    this.mBody.SetActive();
    this.mBody.Velocity = iVelocity;
  }

  public virtual bool ArcIntersect(
    out Vector3 oPosition,
    Vector3 iOrigin,
    Vector3 iDirection,
    float iRange,
    float iAngle,
    float iHeightDifference)
  {
    if ((double) Math.Abs(this.Position.Y - iOrigin.Y) > (double) iHeightDifference)
    {
      oPosition = new Vector3();
      return false;
    }
    iOrigin.Y = 0.0f;
    iDirection.Y = 0.0f;
    Vector3 position1 = this.Position with { Y = 0.0f };
    Vector3 result1;
    Vector3.Subtract(ref iOrigin, ref position1, out result1);
    float num1 = result1.Length();
    float mRadius = this.mRadius;
    if ((double) num1 - (double) mRadius > (double) iRange)
    {
      oPosition = new Vector3();
      return false;
    }
    Vector3.Divide(ref result1, num1, out result1);
    float result2;
    Vector3.Dot(ref result1, ref iDirection, out result2);
    result2 = -result2;
    float num2 = (float) Math.Acos((double) result2);
    float num3 = -2f * num1 * num1;
    float num4 = (float) Math.Acos(((double) mRadius * (double) mRadius + (double) num3) / (double) num3);
    if ((double) num2 - (double) num4 < (double) iAngle)
    {
      Vector3.Multiply(ref result1, mRadius, out result1);
      Vector3 position2 = this.Position;
      Vector3.Add(ref position2, ref result1, out oPosition);
      return true;
    }
    oPosition = new Vector3();
    return false;
  }

  public AudioEmitter AudioEmitter => this.mAudioEmitter;

  public PlayState PlayState => this.mPlayState;

  public virtual Body Body => this.mBody;

  public virtual float Radius => this.mRadius;

  public virtual Vector3 Position => this.mBody.Position;

  public virtual Vector3 Direction => this.mBody.Orientation.Forward;

  public ushort Handle => this.mHandle;

  public abstract bool Dead { get; }

  public abstract bool Removable { get; }

  public static void ClearHandles() => Entity.mInstances.Clear();

  public abstract void Kill();

  internal virtual bool SendsNetworkUpdate(NetworkState iState) => iState == NetworkState.Server;

  internal bool GetNetworkUpdate(
    out EntityUpdateMessage oMsg,
    NetworkState iState,
    float iPrediction)
  {
    if (this.SendsNetworkUpdate(iState))
    {
      this.IGetNetworkUpdate(out oMsg, iPrediction);
      this.mOutBoundUDPStamp = (int) (ushort) (this.mOutBoundUDPStamp + 1);
      oMsg.UDPStamp = (ushort) this.mOutBoundUDPStamp;
      return oMsg.Features != EntityFeatures.None;
    }
    oMsg = new EntityUpdateMessage();
    return false;
  }

  protected abstract void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction);

  internal void NetworkUpdate(SteamID iSender, ref EntityUpdateMessage iMsg)
  {
    ushort num;
    if (this.mInBoundUDPStamp.TryGetValue(iSender.AsUInt64, out num) && ((int) iMsg.UDPStamp < (int) num || (int) iMsg.UDPStamp >= (int) num + (int) short.MaxValue) && (int) iMsg.UDPStamp >= (int) num - (int) short.MaxValue)
      return;
    this.mInBoundUDPStamp[iSender.AsUInt64] = iMsg.UDPStamp;
    this.INetworkUpdate(ref iMsg);
  }

  internal void ForcedNetworkUpdate(SteamID iSender, ref EntityUpdateMessage iMsg)
  {
    this.mInBoundUDPStamp[iSender.AsUInt64] = iMsg.UDPStamp;
    this.INetworkUpdate(ref iMsg);
  }

  protected virtual void INetworkUpdate(ref EntityUpdateMessage iMsg)
  {
    bool flag1 = false;
    if (this.mBody == null)
      return;
    if ((iMsg.Features & EntityFeatures.Position) != EntityFeatures.None)
    {
      flag1 |= this.mBody.Position != iMsg.Position;
      this.mBody.Position = iMsg.Position;
    }
    if ((iMsg.Features & EntityFeatures.Orientation) != EntityFeatures.None)
    {
      Matrix result;
      Matrix.CreateFromQuaternion(ref iMsg.Orientation, out result);
      flag1 |= this.mBody.Orientation != result;
      this.mBody.Orientation = result;
    }
    if (flag1)
      this.mBody.SetActive();
    if ((iMsg.Features & EntityFeatures.Velocity) == EntityFeatures.None)
      return;
    bool flag2 = flag1 | this.mBody.Velocity != iMsg.Velocity;
    this.mBody.Velocity = iMsg.Velocity;
  }

  internal static bool TryGetFromHandle(ushort iHandle, out Entity oEntity)
  {
    if ((int) iHandle >= Entity.mInstances.Count)
    {
      oEntity = (Entity) null;
      return false;
    }
    oEntity = Entity.mInstances[(int) iHandle];
    return true;
  }

  internal virtual float GetDanger() => 0.0f;
}
