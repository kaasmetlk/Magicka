// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Conditions.IsCampaignLooped
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.GameStates.Menu.Main;
using System;

#nullable disable
namespace Magicka.Levels.Triggers.Conditions;

internal class IsCampaignLooped(GameScene iScene) : Condition(iScene)
{
  protected override bool InternalMet(Character iSender)
  {
    if (this.Scene.PlayState.GameType != GameType.Campaign)
      throw new Exception("IsCampaignLooped can only be used in gametype campaign!");
    return SubMenuCampaignSelect_SaveSlotSelect.Instance.CurrentSaveData.Looped;
  }
}
