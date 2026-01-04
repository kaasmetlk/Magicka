// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.CharacterStates.BoostState
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.CharacterStates;

public class BoostState : BaseState
{
  private static BoostState mSingelton;
  private static volatile object mSingeltonLock = new object();

  public static BoostState Instance
  {
    get
    {
      if (BoostState.mSingelton == null)
      {
        lock (BoostState.mSingeltonLock)
        {
          if (BoostState.mSingelton == null)
            BoostState.mSingelton = new BoostState();
        }
      }
      return BoostState.mSingelton;
    }
  }

  public override void OnEnter(Character iOwner) => iOwner.GoToAnimation(Animations.boost, 0.2f);

  public override BaseState Update(Character iOwner, float iDeltaTime)
  {
    BaseState baseState = this.UpdateBloatDeath(iOwner, iDeltaTime);
    if (baseState != null)
      return baseState;
    if (iOwner.IsSelfShielded && !iOwner.IsSolidSelfShielded)
    {
      iOwner.HealSelfShield(-50f * (float) iOwner.Boosts);
      iOwner.Boosts = 0;
    }
    else
    {
      Shield boost = this.ShieldToBoost(iOwner);
      if (boost != null)
      {
        boost.Damage(-100f * (float) iOwner.Boosts);
        iOwner.Boosts = 0;
      }
      if (boost == null)
        return (double) iOwner.CharacterBody.Movement.Length() < 1.4012984643248171E-45 ? (BaseState) IdleState.Instance : (BaseState) MoveState.Instance;
    }
    if ((double) iOwner.BoostCooldown > 0.0)
      return (BaseState) null;
    return (double) iOwner.CharacterBody.Movement.Length() < 1.4012984643248171E-45 ? (BaseState) IdleState.Instance : (BaseState) MoveState.Instance;
  }

  public override void OnExit(Character iOwner)
  {
  }

  public Shield ShieldToBoost(Character iOwner)
  {
    Shield boost = (Shield) null;
    float num1 = 400f;
    List<Shield> shields = iOwner.PlayState.EntityManager.Shields;
    Vector3 direction = iOwner.CharacterBody.Direction with
    {
      Y = 0.0f
    };
    Vector3 position = iOwner.Position;
    Vector3 vector3 = position with { Y = 0.0f };
    Segment seg = new Segment();
    seg.Origin = position;
    Vector3.Multiply(ref direction, 20f, out seg.Delta);
    for (int index = 0; index < shields.Count; ++index)
    {
      Shield shield = shields[index];
      Vector3 pos;
      if ((double) shield.HitPoints > 0.0 && shield.Body.CollisionSkin.SegmentIntersect(out float _, out pos, out Vector3 _, seg))
      {
        pos.Y = 0.0f;
        Vector3 result;
        Vector3.Subtract(ref pos, ref vector3, out result);
        float num2 = result.LengthSquared();
        if ((double) num2 < (double) num1)
        {
          result.Normalize();
          if ((double) num2 <= 1.4012984643248171E-45 || (double) MagickaMath.Angle(ref result, ref direction) < 0.78539818525314331)
          {
            num1 = num2;
            boost = shield;
          }
        }
      }
    }
    return boost;
  }
}
