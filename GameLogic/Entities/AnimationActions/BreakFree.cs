// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.AnimationActions.BreakFree
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework.Content;
using XNAnimation;

#nullable disable
namespace Magicka.GameLogic.Entities.AnimationActions;

public class BreakFree : AnimationAction
{
  private float mMagnitude;
  private int mWeapon;

  public BreakFree(ContentReader iInput, SkinnedModelBoneCollection iSkeleton)
    : base(iInput, iSkeleton)
  {
    this.mMagnitude = iInput.ReadSingle();
    this.mWeapon = iInput.ReadInt32();
  }

  protected override void InternalExecute(Character iOwner, bool iFirstExecution)
  {
    if (!iFirstExecution)
      return;
    if (iOwner.IsEntangled)
    {
      iOwner.DamageEntanglement(this.mMagnitude, Elements.Earth);
    }
    else
    {
      if (!iOwner.IsGripped)
        return;
      iOwner.BreakFree();
    }
  }

  public override bool UsesBones => false;
}
