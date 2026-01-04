// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.AnimationActions.Immortal
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework.Content;
using XNAnimation;

#nullable disable
namespace Magicka.GameLogic.Entities.AnimationActions;

public class Immortal : AnimationAction
{
  private bool mCollide;

  public Immortal(ContentReader iInput, SkinnedModelBoneCollection iSkeleton)
    : base(iInput, iSkeleton)
  {
    this.mCollide = iInput.ReadBoolean();
  }

  protected override void InternalExecute(Character iOwner, bool iFirstExecution)
  {
    iOwner.SetImmortalTime(3f);
    if (this.mCollide)
      return;
    iOwner.CollisionIgnoreTime = 0.15f;
  }

  public override void Kill(Character iOwner)
  {
    iOwner.SetImmortalTime(0.0f);
    base.Kill(iOwner);
  }

  public override bool UsesBones => false;
}
