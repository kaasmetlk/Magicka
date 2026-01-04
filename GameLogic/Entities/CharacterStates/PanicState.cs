// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.CharacterStates.PanicState
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Spells;
using Magicka.Gamers;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;

#nullable disable
namespace Magicka.GameLogic.Entities.CharacterStates;

internal class PanicState : BaseState
{
  private static PanicState mSingelton;
  private static volatile object mSingeltonLock = new object();

  public static PanicState Instance
  {
    get
    {
      if (PanicState.mSingelton == null)
      {
        lock (PanicState.mSingeltonLock)
        {
          if (PanicState.mSingelton == null)
            PanicState.mSingelton = new PanicState();
        }
      }
      return PanicState.mSingelton;
    }
  }

  public override void OnEnter(Magicka.GameLogic.Entities.Character iOwner)
  {
    iOwner.SetInvisible(0.0f);
    iOwner.CharacterBody.AllowMove = true;
    iOwner.CharacterBody.AllowRotate = true;
    iOwner.ReleaseAttachedCharacter();
    if (iOwner.IsStumbling)
    {
      iOwner.CharacterBody.RunBackward = true;
      iOwner.GoToAnimation(Animations.move_stumble, 0.2f);
    }
    else
      iOwner.GoToAnimation(Animations.move_panic, 0.2f);
  }

  public override BaseState Update(Magicka.GameLogic.Entities.Character iOwner, float iDeltaTime)
  {
    BaseState baseState = this.UpdateBloatDeath(iOwner, iDeltaTime);
    if (baseState != null)
      return baseState;
    if (iOwner is Avatar && iOwner.CastType != CastType.None)
      return (BaseState) PanicCastState.Instance;
    if (iOwner.IsEntangled)
      return (BaseState) EntangledState.Instance;
    if (((NetworkManager.Instance.State != NetworkState.Client ? 1 : 0) | (!(iOwner is Avatar) ? 0 : (!((iOwner as Avatar).Player.Gamer is NetworkGamer) ? 1 : 0))) != 0)
      iOwner.WanderAngle = MathHelper.WrapAngle(iOwner.WanderAngle + (float) ((MagickaMath.Random.NextDouble() - 0.5) * 60.0) * iDeltaTime);
    Vector3 result1 = new Vector3();
    MathApproximation.FastSinCos(iOwner.WanderAngle, out result1.Z, out result1.X);
    Vector3 result2 = iOwner.Direction;
    Vector3.Multiply(ref result2, 4f, out result2);
    Vector3.Add(ref result2, ref result1, out result1);
    result1.Normalize();
    Vector3 result3 = !(iOwner is Avatar) ? iOwner.CharacterBody.Movement : (iOwner as Avatar).DesiredInputDirection;
    if (iOwner.IsPanicing)
    {
      iOwner.CharacterBody.SpeedMultiplier *= 0.8f;
      if ((double) result3.LengthSquared() > 9.9999999747524271E-07)
      {
        float panic = iOwner.Panic;
        Vector3.Multiply(ref result1, panic, out result1);
        Vector3.Multiply(ref result3, 1f - panic, out result3);
        Vector3.Add(ref result3, ref result1, out result3);
        result3.Normalize();
        iOwner.CharacterBody.Movement = result3;
      }
      else
        iOwner.CharacterBody.Movement = result1;
    }
    else if (iOwner.IsFeared)
    {
      iOwner.CharacterBody.SpeedMultiplier *= 0.8f;
      Vector3 result4 = iOwner.FearPosition;
      Vector3 position = iOwner.Position;
      Vector3.Subtract(ref position, ref result4, out result4);
      result4.Y = 0.0f;
      result4.X += 0.0001f;
      result4.Normalize();
      if ((double) result3.LengthSquared() > 9.9999999747524271E-07)
      {
        Vector3.Multiply(ref result4, 0.5f, out result4);
        Vector3.Multiply(ref result3, 0.5f, out result3);
        Vector3.Add(ref result3, ref result4, out result4);
        result4.Normalize();
        iOwner.CharacterBody.Movement = result4;
      }
      else
        iOwner.CharacterBody.Movement = result4;
    }
    else
    {
      if (!iOwner.IsStumbling)
        return (BaseState) IdleState.Instance;
      iOwner.CharacterBody.SpeedMultiplier *= 0.6f;
      Vector3.Multiply(ref result2, -1f, out result2);
      Vector3.Add(ref result2, ref result1, out result1);
      result1.Normalize();
      iOwner.CharacterBody.Movement = result1;
    }
    return (BaseState) null;
  }

  public override void OnExit(Magicka.GameLogic.Entities.Character iOwner)
  {
    iOwner.CharacterBody.RunBackward = false;
  }
}
