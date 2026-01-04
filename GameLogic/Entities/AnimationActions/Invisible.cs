// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.AnimationActions.Invisible
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework.Content;
using XNAnimation;

#nullable disable
namespace Magicka.GameLogic.Entities.AnimationActions;

internal class Invisible : AnimationAction
{
  private bool mNoEffect;

  public Invisible(ContentReader iInput, SkinnedModelBoneCollection iSkeleton)
    : base(iInput, iSkeleton)
  {
    this.mNoEffect = iInput.ReadBoolean();
  }

  protected override void InternalExecute(Character iOwner, bool iFirstExecution)
  {
    if (!iFirstExecution)
      return;
    if (this.mNoEffect)
      iOwner.DoNotRender = true;
    else
      iOwner.SetInvisible(float.MaxValue);
  }

  public override bool UsesBones => false;

  public override void Kill(Character iOwner)
  {
    if (this.mNoEffect)
      iOwner.DoNotRender = false;
    else
      iOwner.SetInvisible(0.0f);
    base.Kill(iOwner);
  }
}
