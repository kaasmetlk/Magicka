// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Bosses.BossCollisionZone
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using JigLibX.Physics;
using Magicka.GameLogic.GameStates;
using Magicka.Network;
using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.GameLogic.Entities.Bosses;

public class BossCollisionZone : Entity
{
  protected IBoss mParent;

  public BossCollisionZone(PlayState iPlayState, IBoss iParent, params Primitive[] iPrimitives)
    : base(iPlayState)
  {
    this.mParent = iParent;
    this.mRadius = 100f;
    this.mBody = new Body();
    this.mCollision = new CollisionSkin(this.mBody);
    for (int index = 0; index < iPrimitives.Length; ++index)
      this.mCollision.AddPrimitive(iPrimitives[index], 1, new MaterialProperties(0.0f, 0.0f, 0.0f));
    this.mCollision.ApplyLocalTransform(Transform.Identity);
    this.mBody.CollisionSkin = this.mCollision;
    this.mBody.Immovable = true;
    this.mBody.Tag = (object) this;
    this.mBody.MoveTo(new Vector3(), Matrix.Identity);
  }

  public float HitPoints => this.mParent.HitPoints;

  public float MaxHitPoints => this.mParent.MaxHitPoints;

  public override Vector3 CalcImpulseVelocity(
    Vector3 iDirection,
    float iElevation,
    float iMassPower,
    float iDistance)
  {
    return new Vector3();
  }

  public void SetOrientation(ref Vector3 iPosition, ref Matrix iOrientation)
  {
    this.mBody.MoveTo(iPosition, iOrientation);
  }

  public new virtual void Initialize(int iUniqueID) => base.Initialize(iUniqueID);

  public new virtual void Initialize() => base.Initialize();

  public override bool Dead => false;

  public virtual bool SegmentIntersect(out Vector3 oPosition, Segment iSeg, float iSegmentRadius)
  {
    return this.mCollision.SegmentIntersect(out float _, out oPosition, out Vector3 _, iSeg);
  }

  public override bool ArcIntersect(
    out Vector3 oPosition,
    Vector3 iOrigin,
    Vector3 iDirection,
    float iRange,
    float iAngle,
    float iHeightDifference)
  {
    oPosition = new Vector3();
    return false;
  }

  public override void Kill()
  {
  }

  public IBoss Owner => this.mParent;

  public override bool Removable => false;

  protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
  {
    oMsg = new EntityUpdateMessage();
  }
}
