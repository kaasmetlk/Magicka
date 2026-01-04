// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.AnimationActions.Jump
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using XNAnimation;

#nullable disable
namespace Magicka.GameLogic.Entities.AnimationActions;

internal class Jump : AnimationAction
{
  private float mElevation;
  private float? mMinRange;
  private float? mMaxRange;

  public Jump(ContentReader iInput, SkinnedModelBoneCollection iSkeleton)
    : base(iInput, iSkeleton)
  {
    this.mElevation = iInput.ReadSingle();
    this.mElevation = MathHelper.ToRadians(this.mElevation);
    if (iInput.ReadBoolean())
      this.mMinRange = new float?(iInput.ReadSingle());
    if (!iInput.ReadBoolean())
      return;
    this.mMaxRange = new float?(iInput.ReadSingle());
  }

  protected override void InternalExecute(Character iOwner, bool iFirstExecution)
  {
    if (!iFirstExecution || !(iOwner is NonPlayerCharacter nonPlayerCharacter))
      return;
    Vector3 position = iOwner.Position;
    Vector3 vector3 = new Vector3();
    Vector3 result1 = new Vector3();
    Vector3 result2;
    if (nonPlayerCharacter.AI.CurrentTarget != null)
    {
      result2 = nonPlayerCharacter.AI.CurrentTarget.Position;
      result1 = nonPlayerCharacter.AI.CurrentTarget.Body.Velocity;
    }
    else
    {
      result2 = iOwner.Direction;
      Vector3.Multiply(ref result2, this.mMinRange.GetValueOrDefault() + (float) (((double) this.mMaxRange.GetValueOrDefault() - (double) this.mMinRange.GetValueOrDefault()) * 0.5), out result2);
    }
    if ((double) result1.LengthSquared() > 1.4012984643248171E-45)
    {
      Vector3.Multiply(ref result1, 0.5f, out result1);
      Vector3.Add(ref result2, ref result1, out result2);
    }
    Vector3 result3;
    Vector3.Subtract(ref result2, ref position, out result3);
    float num1 = result3.LengthSquared();
    if ((double) num1 < 1.4012984643248171E-45)
      return;
    Vector3.Normalize(ref result3, out result3);
    float num2 = num1 * 1.1f;
    if (this.mMinRange.HasValue)
      num2 = Math.Max(this.mMinRange.Value, num2);
    if (this.mMaxRange.HasValue)
      num2 = Math.Min(this.mMaxRange.Value, num2);
    Vector3.Multiply(ref result3, num2, out result3);
    nonPlayerCharacter.CharacterBody.AllowMove = true;
    nonPlayerCharacter.Jump(result3, this.mElevation);
  }

  public override bool UsesBones => false;
}
