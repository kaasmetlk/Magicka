// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.RemoveAllStatusEffects
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class RemoveAllStatusEffects(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  protected override void Execute()
  {
    StaticList<Entity> entities = this.mScene.PlayState.EntityManager.Entities;
    for (int iIndex = 0; iIndex < entities.Count; ++iIndex)
    {
      if (entities[iIndex] is Character iOwner)
      {
        iOwner.StopStatusEffects(StatusEffects.Burning | StatusEffects.Wet | StatusEffects.Frozen | StatusEffects.Cold | StatusEffects.Poisoned | StatusEffects.Healing | StatusEffects.Greased | StatusEffects.Steamed);
        if (iOwner.IsEntangled)
          iOwner.ReleaseEntanglement();
        if (iOwner.IsSelfShielded)
          iOwner.RemoveSelfShield();
        if (iOwner.IsFeared)
          iOwner.RemoveFear();
        if (iOwner.IsInvisibile)
          iOwner.RemoveInvisibility();
        if (iOwner.IsHypnotized)
          iOwner.StopHypnotize();
        if (iOwner.CurrentSpell != null)
          iOwner.CurrentSpell.Stop((ISpellCaster) iOwner);
        if (iOwner.SpellQueue.Count > 0)
          iOwner.SpellQueue.Clear();
        if (iOwner.IsLevitating)
          iOwner.StopLevitate();
        if (!iOwner.mBubble)
          iOwner.ClearAura();
      }
    }
  }

  public override void QuickExecute() => this.Execute();
}
