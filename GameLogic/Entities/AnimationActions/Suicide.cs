// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.AnimationActions.Suicide
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework.Content;
using XNAnimation;

#nullable disable
namespace Magicka.GameLogic.Entities.AnimationActions;

internal class Suicide : AnimationAction
{
  private bool mOverkill;

  public Suicide(ContentReader iInput, SkinnedModelBoneCollection iSkeleton)
    : base(iInput, iSkeleton)
  {
    this.mOverkill = iInput.ReadBoolean();
  }

  protected override void InternalExecute(Character iOwner, bool iFirstExecution)
  {
    if (this.mOverkill)
    {
      iOwner.OverKill();
    }
    else
    {
      iOwner.Kill();
      iOwner.Die();
    }
  }

  public override bool UsesBones => false;
}
