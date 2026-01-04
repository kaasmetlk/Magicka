// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.GameSparks.GameSparksProperties
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using GameSparks.Api.Responses;
using Magicka.CoreFramework;
using Magicka.Misc;
using Magicka.WebTools.GameSparks.PropertySets;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.WebTools.GameSparks;

internal class GameSparksProperties : Singleton<GameSparksProperties>
{
  private const Logger.Source LOGGER_SOURCE = Logger.Source.GameSparksProperties;
  private const string INVALID_SET_NAME = "{0} is not the name of any known Property Set";
  private List<GameSparksPropertySet> mPropertySets = new List<GameSparksPropertySet>()
  {
    (GameSparksPropertySet) new AdsABTestPropertySet()
  };
  private int mSetCount;

  public event Action OnPropertiesLoaded;

  public void ConfirmPropertySetReady(GameSparksPropertySet iSet)
  {
    ++this.mSetCount;
    if (this.mSetCount != this.mPropertySets.Count || this.OnPropertiesLoaded == null)
      return;
    this.OnPropertiesLoaded();
  }

  public void RetrievePropertySetsFromGameSparks()
  {
    foreach (GameSparksPropertySet mPropertySet in this.mPropertySets)
      Singleton<GameSparksServices>.Instance.RequestPropertySet(mPropertySet.Name, new Action<GetPropertySetResponse>(mPropertySet.Callback));
  }

  public T GetProperty<T>(string iSetKey, string iPropertyKey)
  {
    return this.mPropertySets.Find((Predicate<GameSparksPropertySet>) (x => x.Name == iSetKey)).GetProperty<T>(iPropertyKey);
  }
}
