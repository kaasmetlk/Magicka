// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.AnimationActions.Ethereal
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework.Content;
using XNAnimation;

#nullable disable
namespace Magicka.GameLogic.Entities.AnimationActions;

internal class Ethereal : AnimationAction
{
  private bool mEthereal;
  private float mAlpha;
  private float mSpeed;

  public Ethereal(ContentReader iInput, SkinnedModelBoneCollection iSkeleton)
    : base(iInput, iSkeleton)
  {
    this.mEthereal = iInput.ReadBoolean();
    this.mAlpha = iInput.ReadSingle();
    this.mSpeed = iInput.ReadSingle();
  }

  protected override void InternalExecute(Character iOwner, bool iFirstExecution)
  {
    if (!iFirstExecution)
      return;
    if ((double) this.mSpeed == 0.0)
      iOwner.EtherealAlpha = this.mAlpha;
    else
      iOwner.Ethereal(this.mEthereal, this.mAlpha, this.mSpeed);
  }

  public override bool UsesBones => false;

  public override void Kill(Character iOwner) => base.Kill(iOwner);
}
