// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.AnimationActions.DealDamage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities.Items;
using Microsoft.Xna.Framework.Content;
using System;
using XNAnimation;

#nullable disable
namespace Magicka.GameLogic.Entities.AnimationActions;

public class DealDamage : AnimationAction
{
  private int mWeapon;
  private DealDamage.Targets mTarget;

  public DealDamage(ContentReader iInput, SkinnedModelBoneCollection iSkeleton)
    : base(iInput, iSkeleton)
  {
    this.mWeapon = iInput.ReadInt32();
    this.mTarget = (DealDamage.Targets) iInput.ReadByte();
  }

  protected override void InternalExecute(Character iOwner, bool iFirstExecution)
  {
    Item iOwner1 = iOwner.Equipment[this.mWeapon].Item;
    if (iFirstExecution)
    {
      iOwner1.ClearHitlist();
      iOwner1.MeleeConditions.ExecuteAll((Entity) iOwner1, (Entity) null, ref new EventCondition()
      {
        EventConditionType = EventConditionType.Default
      });
    }
    iOwner1.Execute(this.mTarget);
  }

  public override bool UsesBones => true;

  public override void Kill(Character iOwner)
  {
    base.Kill(iOwner);
    iOwner.Equipment[this.mWeapon].Item.StopExecute();
  }

  [Flags]
  public enum Targets : byte
  {
    None = 0,
    Target = 0,
    Friendly = 1,
    Enemy = 2,
    NonCharacters = 4,
    All = 255, // 0xFF
  }
}
