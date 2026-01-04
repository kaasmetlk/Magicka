// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.AnimationActions.CastSpell
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework.Content;
using XNAnimation;

#nullable disable
namespace Magicka.GameLogic.Entities.AnimationActions;

public class CastSpell : AnimationAction
{
  private bool mFromStaff;
  private string mSource;

  public CastSpell(ContentReader iInput, SkinnedModelBoneCollection iSkeleton)
    : base(iInput, iSkeleton)
  {
    this.mFromStaff = iInput.ReadBoolean();
    if (this.mFromStaff)
      return;
    this.mSource = iInput.ReadString();
  }

  protected override void InternalExecute(Character iOwner, bool iFirstExecution)
  {
    if (!iFirstExecution || iOwner.CurrentSpell != null)
      return;
    iOwner.CastSpell(this.mFromStaff, this.mSource);
  }

  public override void Kill(Character iOwner)
  {
    base.Kill(iOwner);
    iOwner.CurrentSpell?.AnimationEnd((ISpellCaster) iOwner);
  }

  public override bool UsesBones => true;
}
