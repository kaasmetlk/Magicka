// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.ClearDelayedActions
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System.Collections.Generic;
using System.Xml;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class ClearDelayedActions(Magicka.Levels.Triggers.Trigger iTrigger, GameScene iScene, XmlNode iNode) : 
  Action(iTrigger, iScene)
{
  private static readonly int ALL = "all".GetHashCodeCustom();
  private static readonly int ALLBUTTHIS = "allbutthis".GetHashCodeCustom();
  private string mTriggerName;
  private int mTriggerID;

  protected override void Execute()
  {
    if (this.mTriggerID == ClearDelayedActions.ALL || this.mTriggerID == 0)
    {
      foreach (Magicka.Levels.Triggers.Trigger trigger in (IEnumerable<Magicka.Levels.Triggers.Trigger>) this.mScene.Triggers.Values)
        trigger.ClearDelayedActions();
    }
    else if (this.mTriggerID == ClearDelayedActions.ALLBUTTHIS)
    {
      foreach (Magicka.Levels.Triggers.Trigger trigger in (IEnumerable<Magicka.Levels.Triggers.Trigger>) this.mScene.Triggers.Values)
      {
        if (trigger != this.mTrigger)
          trigger.ClearDelayedActions();
      }
    }
    else
      this.mScene.Triggers[this.mTriggerID].ClearDelayedActions();
  }

  public override void QuickExecute()
  {
  }

  public string Trigger
  {
    get => this.mTriggerName;
    set
    {
      this.mTriggerName = value;
      this.mTriggerID = this.mTriggerName.ToLowerInvariant().GetHashCodeCustom();
    }
  }
}
