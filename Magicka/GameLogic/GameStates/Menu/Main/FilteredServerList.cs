using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Magicka.GameLogic.UI;
using Magicka.Network;
using SteamWrapper;

namespace Magicka.GameLogic.GameStates.Menu.Main
{
	// Token: 0x020003A7 RID: 935
	public class FilteredServerList
	{
		// Token: 0x1700070F RID: 1807
		// (get) Token: 0x06001CC9 RID: 7369 RVA: 0x000CA58D File Offset: 0x000C878D
		// (set) Token: 0x06001CCA RID: 7370 RVA: 0x000CA59A File Offset: 0x000C879A
		public FilteredServerList.SortType Sort
		{
			get
			{
				return this.mServerSorter.Sort;
			}
			private set
			{
				this.mServerSorter.Sort = value;
			}
		}

		// Token: 0x06001CCB RID: 7371 RVA: 0x000CA5A8 File Offset: 0x000C87A8
		public FilteredServerList()
		{
			this.mVisibleServersReadOnly = new ReadOnlyCollection<GameServerItem>(this.mVisibleServers);
			NetworkManager.ListResponse instance = NetworkManager.ListResponse.Instance;
			instance.ServerListChanged = (Action<ReadOnlyCollection<GameServerItem>>)Delegate.Combine(instance.ServerListChanged, new Action<ReadOnlyCollection<GameServerItem>>(this.OnServerListChanged));
			this.mVersion = Application.ProductVersion;
			this.mVersion = this.mVersion.Replace(".", "");
		}

		// Token: 0x06001CCC RID: 7372 RVA: 0x000CA634 File Offset: 0x000C8834
		public ReadOnlyCollection<GameServerItem> GetVisibleServers(FilterData iFilterData)
		{
			NetworkManager.Instance.RequestServers(iFilterData);
			lock (this.mVisibleServersReadOnly)
			{
				this.mVisibleServers.Clear();
			}
			return this.mVisibleServersReadOnly;
		}

		// Token: 0x06001CCD RID: 7373 RVA: 0x000CA684 File Offset: 0x000C8884
		public void OnServerListChanged(ReadOnlyCollection<GameServerItem> iNewServerList)
		{
			this.mAllServersReadOnly = iNewServerList;
			this.FilterList(GlobalSettings.Instance.Filter);
		}

		// Token: 0x06001CCE RID: 7374 RVA: 0x000CA6A0 File Offset: 0x000C88A0
		public void FilterList(FilterData iFilter)
		{
			if (this.mAllServersReadOnly == null)
			{
				return;
			}
			lock (this.mVisibleServersReadOnly)
			{
				this.mVisibleServers.Clear();
				lock (this.mAllServersReadOnly)
				{
					for (int i = 0; i < this.mAllServersReadOnly.Count; i++)
					{
						this.AddServer(ref iFilter, this.mAllServersReadOnly[i], this.mVisibleServers);
					}
				}
				if (this.Sort != FilteredServerList.SortType.None)
				{
					this.mVisibleServers.Sort(this.mServerSorter);
				}
			}
		}

		// Token: 0x06001CCF RID: 7375 RVA: 0x000CA754 File Offset: 0x000C8954
		public void SortList(FilteredServerList.SortType iType)
		{
			if (iType == this.Sort)
			{
				this.mServerSorter.Ascending = !this.mServerSorter.Ascending;
			}
			else
			{
				this.mServerSorter.Ascending = true;
			}
			this.Sort = iType;
			if (this.Sort != FilteredServerList.SortType.None)
			{
				lock (this.mVisibleServersReadOnly)
				{
					this.mVisibleServers.Sort(this.mServerSorter);
				}
			}
		}

		// Token: 0x06001CD0 RID: 7376 RVA: 0x000CA7D8 File Offset: 0x000C89D8
		private void AddServer(ref FilterData iFilter, GameServerItem iServer, List<GameServerItem> iVisibleServers)
		{
			if (iServer.m_AppID != SteamUtils.GetAppID() || this.mVisibleServers.Contains(iServer))
			{
				return;
			}
			if (iFilter.VACOnly && !iServer.m_Secure)
			{
				return;
			}
			if (iFilter.Scope == Scope.WAN && ((iFilter.FilterPassword && iServer.m_Password) || (iFilter.MaxLatency > 0 && iServer.m_Ping > (int)iFilter.MaxLatency)))
			{
				return;
			}
			if (iServer.GameTags.Length == 0)
			{
				return;
			}
			if (iFilter.FilterPlaying && iServer.Playing())
			{
				return;
			}
			if ((byte)(iFilter.GameType & iServer.GameType()) == 0)
			{
				return;
			}
			iVisibleServers.Add(iServer);
		}

		// Token: 0x04001F6C RID: 8044
		private FilteredServerList.ServerComparer mServerSorter = new FilteredServerList.ServerComparer(FilteredServerList.SortType.None);

		// Token: 0x04001F6D RID: 8045
		private List<GameServerItem> mVisibleServers = new List<GameServerItem>(32);

		// Token: 0x04001F6E RID: 8046
		private ReadOnlyCollection<GameServerItem> mVisibleServersReadOnly;

		// Token: 0x04001F6F RID: 8047
		private ReadOnlyCollection<GameServerItem> mAllServersReadOnly;

		// Token: 0x04001F70 RID: 8048
		private string mVersion;

		// Token: 0x020003A8 RID: 936
		public enum SortType
		{
			// Token: 0x04001F72 RID: 8050
			None,
			// Token: 0x04001F73 RID: 8051
			Name,
			// Token: 0x04001F74 RID: 8052
			Ping,
			// Token: 0x04001F75 RID: 8053
			Players,
			// Token: 0x04001F76 RID: 8054
			Level
		}

		// Token: 0x020003A9 RID: 937
		private class ServerComparer : IComparer<GameServerItem>
		{
			// Token: 0x06001CD1 RID: 7377 RVA: 0x000CA879 File Offset: 0x000C8A79
			public ServerComparer(FilteredServerList.SortType iType)
			{
				this.Sort = iType;
			}

			// Token: 0x17000710 RID: 1808
			// (get) Token: 0x06001CD2 RID: 7378 RVA: 0x000CA88F File Offset: 0x000C8A8F
			// (set) Token: 0x06001CD3 RID: 7379 RVA: 0x000CA897 File Offset: 0x000C8A97
			public FilteredServerList.SortType Sort
			{
				get
				{
					return this.mType;
				}
				set
				{
					this.mType = value;
				}
			}

			// Token: 0x06001CD4 RID: 7380 RVA: 0x000CA8A0 File Offset: 0x000C8AA0
			public int Compare(GameServerItem x, GameServerItem y)
			{
				int result = -1;
				switch (this.mType)
				{
				case FilteredServerList.SortType.Name:
					if (this.Ascending)
					{
						result = x.m_Name.CompareTo(y.m_Name);
					}
					else
					{
						result = y.m_Name.CompareTo(x.m_Name);
					}
					break;
				case FilteredServerList.SortType.Ping:
					if (this.Ascending)
					{
						result = x.m_Ping.CompareTo(y.m_Ping);
					}
					else
					{
						result = y.m_Ping.CompareTo(x.m_Ping);
					}
					break;
				case FilteredServerList.SortType.Players:
					if (this.Ascending)
					{
						result = x.m_Players.CompareTo(y.m_Players);
					}
					else
					{
						result = y.m_Players.CompareTo(x.m_Players);
					}
					break;
				case FilteredServerList.SortType.Level:
				{
					string levelString = SubMenuOnline.GetLevelString(x.m_Map);
					string levelString2 = SubMenuOnline.GetLevelString(y.m_Map);
					if (this.Ascending)
					{
						result = levelString.CompareTo(levelString2);
					}
					else
					{
						result = levelString2.CompareTo(levelString);
					}
					break;
				}
				}
				return result;
			}

			// Token: 0x04001F77 RID: 8055
			public bool Ascending = true;

			// Token: 0x04001F78 RID: 8056
			private FilteredServerList.SortType mType;
		}
	}
}
