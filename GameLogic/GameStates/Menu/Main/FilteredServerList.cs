// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.Main.FilteredServerList
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.UI;
using Magicka.Network;
using SteamWrapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu.Main;

public class FilteredServerList
{
  private FilteredServerList.ServerComparer mServerSorter = new FilteredServerList.ServerComparer(FilteredServerList.SortType.None);
  private List<GameServerItem> mVisibleServers = new List<GameServerItem>(32 /*0x20*/);
  private ReadOnlyCollection<GameServerItem> mVisibleServersReadOnly;
  private ReadOnlyCollection<GameServerItem> mAllServersReadOnly;
  private string mVersion;

  public FilteredServerList.SortType Sort
  {
    get => this.mServerSorter.Sort;
    private set => this.mServerSorter.Sort = value;
  }

  public FilteredServerList()
  {
    this.mVisibleServersReadOnly = new ReadOnlyCollection<GameServerItem>((IList<GameServerItem>) this.mVisibleServers);
    NetworkManager.ListResponse.Instance.ServerListChanged += new Action<ReadOnlyCollection<GameServerItem>>(this.OnServerListChanged);
    this.mVersion = Application.ProductVersion;
    this.mVersion = this.mVersion.Replace(".", "");
  }

  public ReadOnlyCollection<GameServerItem> GetVisibleServers(FilterData iFilterData)
  {
    NetworkManager.Instance.RequestServers(iFilterData);
    lock (this.mVisibleServersReadOnly)
      this.mVisibleServers.Clear();
    return this.mVisibleServersReadOnly;
  }

  public void OnServerListChanged(ReadOnlyCollection<GameServerItem> iNewServerList)
  {
    this.mAllServersReadOnly = iNewServerList;
    this.FilterList(GlobalSettings.Instance.Filter);
  }

  public void FilterList(FilterData iFilter)
  {
    if (this.mAllServersReadOnly == null)
      return;
    lock (this.mVisibleServersReadOnly)
    {
      this.mVisibleServers.Clear();
      lock (this.mAllServersReadOnly)
      {
        for (int index = 0; index < this.mAllServersReadOnly.Count; ++index)
          this.AddServer(ref iFilter, this.mAllServersReadOnly[index], this.mVisibleServers);
      }
      if (this.Sort == FilteredServerList.SortType.None)
        return;
      this.mVisibleServers.Sort((IComparer<GameServerItem>) this.mServerSorter);
    }
  }

  public void SortList(FilteredServerList.SortType iType)
  {
    this.mServerSorter.Ascending = iType != this.Sort || !this.mServerSorter.Ascending;
    this.Sort = iType;
    if (this.Sort == FilteredServerList.SortType.None)
      return;
    lock (this.mVisibleServersReadOnly)
      this.mVisibleServers.Sort((IComparer<GameServerItem>) this.mServerSorter);
  }

  private void AddServer(
    ref FilterData iFilter,
    GameServerItem iServer,
    List<GameServerItem> iVisibleServers)
  {
    if ((int) iServer.m_AppID != (int) SteamUtils.GetAppID() || this.mVisibleServers.Contains(iServer) || iFilter.VACOnly && !iServer.m_Secure || iFilter.Scope == Scope.WAN && (iFilter.FilterPassword && iServer.m_Password || iFilter.MaxLatency > (short) 0 && iServer.m_Ping > (int) iFilter.MaxLatency) || iServer.GameTags.Length == 0 || iFilter.FilterPlaying && iServer.Playing() || (iFilter.GameType & iServer.GameType()) == (GameType) 0)
      return;
    iVisibleServers.Add(iServer);
  }

  public enum SortType
  {
    None,
    Name,
    Ping,
    Players,
    Level,
  }

  private class ServerComparer : IComparer<GameServerItem>
  {
    public bool Ascending = true;
    private FilteredServerList.SortType mType;

    public ServerComparer(FilteredServerList.SortType iType) => this.Sort = iType;

    public FilteredServerList.SortType Sort
    {
      get => this.mType;
      set => this.mType = value;
    }

    public int Compare(GameServerItem x, GameServerItem y)
    {
      int num = -1;
      switch (this.mType)
      {
        case FilteredServerList.SortType.Name:
          num = !this.Ascending ? y.m_Name.CompareTo(x.m_Name) : x.m_Name.CompareTo(y.m_Name);
          break;
        case FilteredServerList.SortType.Ping:
          num = !this.Ascending ? y.m_Ping.CompareTo(x.m_Ping) : x.m_Ping.CompareTo(y.m_Ping);
          break;
        case FilteredServerList.SortType.Players:
          num = !this.Ascending ? y.m_Players.CompareTo(x.m_Players) : x.m_Players.CompareTo(y.m_Players);
          break;
        case FilteredServerList.SortType.Level:
          string levelString1 = SubMenuOnline.GetLevelString(x.m_Map);
          string levelString2 = SubMenuOnline.GetLevelString(y.m_Map);
          num = !this.Ascending ? levelString2.CompareTo(levelString1) : levelString1.CompareTo(levelString2);
          break;
      }
      return num;
    }
  }
}
