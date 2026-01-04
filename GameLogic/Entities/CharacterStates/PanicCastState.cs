// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.CharacterStates.PanicCastState
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.Gamers;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;

#nullable disable
namespace Magicka.GameLogic.Entities.CharacterStates;

internal class PanicCastState : BaseState
{
  private static PanicCastState mSingelton;
  private static volatile object mSingeltonLock = new object();

  public static PanicCastState Instance
  {
    get
    {
      if (PanicCastState.mSingelton == null)
      {
        lock (PanicCastState.mSingeltonLock)
        {
          if (PanicCastState.mSingelton == null)
            PanicCastState.mSingelton = new PanicCastState();
        }
      }
      return PanicCastState.mSingelton;
    }
  }

  private PanicCastState()
  {
  }

  public override void OnEnter(Magicka.GameLogic.Entities.Character iOwner)
  {
    iOwner.SetInvisible(0.0f);
    iOwner.CharacterBody.AllowMove = true;
    iOwner.CharacterBody.AllowRotate = true;
    if (iOwner.IsStumbling)
      iOwner.CharacterBody.RunBackward = true;
    iOwner.CastSpell(true, "");
  }

  public override BaseState Update(Magicka.GameLogic.Entities.Character iOwner, float iDeltaTime)
  {
    if (((iOwner.CurrentSpell == null ? 0 : (!iOwner.CurrentSpell.Active ? 1 : 0)) | (iOwner.CastType == CastType.Weapon ? 1 : 0)) != 0)
      return (BaseState) PanicState.Instance;
    if (((NetworkManager.Instance.State != NetworkState.Client ? 1 : 0) | (!(iOwner is Avatar) ? 0 : (!((iOwner as Avatar).Player.Gamer is NetworkGamer) ? 1 : 0))) != 0)
      iOwner.WanderAngle = MathHelper.WrapAngle(iOwner.WanderAngle + (float) ((MagickaMath.Random.NextDouble() - 0.5) * 60.0) * iDeltaTime);
    Vector3 result1 = new Vector3();
    MathApproximation.FastSinCos(iOwner.WanderAngle, out result1.Z, out result1.X);
    Vector3 result2 = iOwner.Direction;
    Vector3.Multiply(ref result2, 4f, out result2);
    if (iOwner.IsPanicing)
    {
      Vector3.Add(ref result2, ref result1, out result1);
      result1.Normalize();
      Vector3 result3 = iOwner.CharacterBody.Movement;
      if ((double) result3.LengthSquared() > 9.9999999747524271E-07)
      {
        float panic = iOwner.Panic;
        Vector3.Multiply(ref result1, panic, out result1);
        Vector3.Multiply(ref result3, 1f - panic, out result3);
        Vector3.Add(ref result3, ref result1, out result3);
      }
      else
        Vector3.Multiply(ref result1, 0.75f, out result3);
      iOwner.CharacterBody.Movement = result3;
    }
    else if (iOwner.IsFeared)
    {
      Vector3.Add(ref result2, ref result1, out result1);
      result1.Normalize();
      Vector3 result4 = iOwner.FearPosition;
      Vector3 position = iOwner.Position;
      Vector3.Subtract(ref position, ref result4, out result4);
      result4.Y = 0.0f;
      result4.Normalize();
      Vector3.Add(ref result4, ref result1, out result1);
      result1.Normalize();
      iOwner.CharacterBody.Movement = result1;
    }
    else if (iOwner.IsStumbling)
    {
      iOwner.CharacterBody.SpeedMultiplier *= 0.666f;
      Vector3.Multiply(ref result2, -1f, out result2);
      Vector3.Add(ref result2, ref result1, out result1);
      result1.Normalize();
      iOwner.CharacterBody.Movement = result1;
    }
    else
      return (double) iOwner.CharacterBody.Movement.Length() < 1.4012984643248171E-45 ? (BaseState) IdleState.Instance : (BaseState) MoveState.Instance;
    return (BaseState) null;
  }

  public override void OnExit(Magicka.GameLogic.Entities.Character iOwner)
  {
    if (iOwner.CurrentSpell != null)
    {
      iOwner.CurrentSpell.Stop((ISpellCaster) iOwner);
      iOwner.CurrentSpell = (SpellEffect) null;
    }
    iOwner.CastType = CastType.None;
    iOwner.CharacterBody.RunBackward = false;
  }
}
