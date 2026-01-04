// Decompiled with JetBrains decompiler
// Type: Magicka.Network.NetworkServer
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.GameLogic.UI;
using Magicka.Gamers;
using Magicka.Levels;
using Magicka.Levels.Triggers;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SteamWrapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

#nullable disable
namespace Magicka.Network;

internal class NetworkServer : NetworkInterface
{
  private const uint ANY_IP = 0;
  public const int GAME_PORT = 27015;
  public const int QUERY_PORT = 27016;
  public const int AUTHENTICATION_PORT = 8766;
  private SteamID mSteamIDLobby;
  private byte[] mRxBuffer;
  private byte[] mTxBuffer;
  private string mPassword;
  private BinaryReader mReader;
  private BinaryWriter mWriter;
  private uint mSyncPointCounter;
  private List<NetworkServer.PendingUser> mPendingClients = new List<NetworkServer.PendingUser>();
  private byte[] mDummyData = new byte[1];
  private bool mUPnPEnabled;
  private bool mPlaying;
  private long mLastLatencyCheck = DateTime.Now.Ticks;
  private bool mListening;
  private byte[] mLocalIPNumber = new byte[4]
  {
    (byte) 127 /*0x7F*/,
    (byte) 0,
    (byte) 0,
    (byte) 1
  };
  private List<NetworkServer.Connection> mClients = new List<NetworkServer.Connection>(8);
  private OptionsMessageBox mLostConnectionMB;
  private SortedList<long, int> mBytesSent = new SortedList<long, int>();
  private int mBytesPerSecond;
  private bool mVAC;
  private string mLevelName;

  public NetworkServer(GameType iGameType, bool iVAC)
  {
    this.mPlaying = false;
    this.mGameType = iGameType;
    this.mLevelName = NetworkManager.Instance.LevelName;
    this.mPassword = NetworkManager.Instance.Password;
    this.mVAC = iVAC;
    if (!SteamGameServer.Init(0U, (ushort) 8766, (ushort) 27015, (ushort) 0, (ushort) 27016, iVAC ? ServerMode.AuthenticationAndSecure : ServerMode.Authentication, "magicka", Application.ProductVersion))
      throw new Exception();
    Magicka.Game.Instance.AddLoadTask((Action) (() =>
    {
      try
      {
        this.mUPnPEnabled = NAT.Discover();
        if (!this.mUPnPEnabled)
          return;
        NAT.ForwardPort(27016, ProtocolType.Udp, $"Magicka: {NetworkManager.Instance.GameName} (UDP)");
      }
      catch
      {
        this.mUPnPEnabled = false;
      }
    }));
    SteamGameServer.SteamServersConnected += new Action<SteamServersConnected>(this.SteamGameServer_SteamServersConnected);
    SteamGameServer.SteamServersDisconnected += new Action<SteamServersDisconnected>(this.SteamGameServer_SteamServersDisconnected);
    SteamGameServer.GSPolicyResponse += new Action<GSPolicyResponse>(this.SteamGameServer_GSPolicyResponse);
    SteamGameServer.GSClientApprove += new Action<GSClientApprove>(this.SteamGameServer_GSClientApprove);
    SteamGameServer.GSClientDeny += new Action<GSClientDeny>(this.SteamGameServer_GSClientDeny);
    SteamGameServer.GSClientKick += new Action<GSClientKick>(this.SteamGameServer_GSClientKick);
    SteamGameServer.P2PSessionRequest += new Action<P2PSessionRequest>(this.SteamGameServer_P2PSessionRequest);
    SteamGameServer.P2PSessionConnectFail += new Action<P2PSessionConnectFail>(this.SteamGameServer_P2PSessionConnectFail);
    this.UpdateGameTags();
    this.mRxBuffer = new byte[1048576 /*0x100000*/];
    this.mTxBuffer = new byte[1048576 /*0x100000*/];
    this.mReader = new BinaryReader((Stream) new MemoryStream(this.mRxBuffer));
    this.mWriter = new BinaryWriter((Stream) new MemoryStream(this.mTxBuffer));
    this.mListening = true;
    this.mLostConnectionMB = new OptionsMessageBox("Connection lost to                                               ", new string[1]
    {
      "Ok"
    });
  }

  public override SteamID ServerID => SteamGameServer.GetSteamID();

  public override bool IsVACSecure => this.mVAC;

  public bool Playing
  {
    get => this.mPlaying;
    set
    {
      if (this.mPlaying == value)
        return;
      this.mPlaying = value;
      SteamMatchmaking.SetLobbyJoinable(this.mSteamIDLobby, !value);
      SteamMatchmaking.SetLobbyData(this.mSteamIDLobby, "playing", value ? "1" : "0");
      this.UpdateGameTags();
      if (!value)
        return;
      this.mPendingClients.Clear();
    }
  }

  private void UpdateGameTags()
  {
    SteamGameServer.SetGameType($"P{(this.mPlaying ? (object) "1" : (object) "0")}T{(int) this.mGameType}");
  }

  private void OnCreatedLobby(LobbyCreated iLobby)
  {
    if (iLobby.mResult != Result.OK)
      return;
    this.mSteamIDLobby = new SteamID(iLobby.mSteamIDLobby);
    if (!NetworkManager.Instance.HasHostSettings)
    {
      SteamMatchmaking.LeaveLobby(this.mSteamIDLobby);
      SteamMatchmaking.SetLobbyJoinable(this.mSteamIDLobby, false);
    }
    else
    {
      SteamMatchmaking.SetLobbyData(this.mSteamIDLobby, "slots", (4 - this.mClients.Count).ToString());
      SteamMatchmaking.SetLobbyData(this.mSteamIDLobby, "playing", "0");
      if (string.IsNullOrEmpty(this.mPassword))
        SteamMatchmaking.SetLobbyData(this.mSteamIDLobby, "password", "0");
      else
        SteamMatchmaking.SetLobbyData(this.mSteamIDLobby, "password", "1");
      SteamMatchmaking.SetLobbyData(this.mSteamIDLobby, "version", Application.ProductVersion);
      if (this.mGameType != GameType.Any)
        SteamMatchmaking.SetLobbyData(this.mSteamIDLobby, "gametype", this.mGameType.ToString());
      SteamID steamId = SteamGameServer.GetSteamID();
      SteamMatchmaking.SetLobbyGameServer(this.mSteamIDLobby, SteamGameServer.GetPublicIP(), (ushort) 27015, steamId);
    }
  }

  private void SteamGameServer_SteamServersConnected(SteamServersConnected iParam)
  {
    if (!this.mSteamIDLobby.IsValid)
      SteamMatchmaking.CreateLobby(LobbyType.Public, 4, new Action<LobbyCreated>(this.OnCreatedLobby));
    this.SendUpdatedGameDataToSteam();
  }

  private void SteamGameServer_SteamServersDisconnected(SteamServersDisconnected iParam)
  {
  }

  private void SteamGameServer_GSPolicyResponse(GSPolicyResponse iParam)
  {
  }

  private void SteamGameServer_GSClientApprove(GSClientApprove iParam)
  {
    for (int index1 = 0; index1 < this.mPendingClients.Count; ++index1)
    {
      NetworkServer.PendingUser mPendingClient = this.mPendingClients[index1];
      if (mPendingClient.ID == iParam.mSteamID)
      {
        AuthenticateReplyMessage iMessage1 = new AuthenticateReplyMessage();
        iMessage1.Response = AuthenticateReplyMessage.Reply.Ok;
        SubMenuCharacterSelect.Instance.GetSettings(out iMessage1.GameInfo.GameType, out iMessage1.GameInfo.Level, out iMessage1.VersusSettings);
        iMessage1.GameInfo.GameName = NetworkManager.Instance.GameName;
        NetworkServer.Connection connection;
        connection.ID = mPendingClient.ID;
        connection.Ready = false;
        connection.Latency = 0.0f;
        connection.Timer = Stopwatch.StartNew();
        connection.TxBuffer = new byte[4400];
        connection.Writer = new BinaryWriter((Stream) new MemoryStream(connection.TxBuffer));
        connection.SyncPoints = new List<uint>();
        this.mClients.Add(connection);
        this.SendMessage<AuthenticateReplyMessage>(ref iMessage1, mPendingClient.ID);
        this.SendMessage<LobbyInfoMessage>(ref new LobbyInfoMessage()
        {
          SteamID = this.mSteamIDLobby.AsUInt64
        }, mPendingClient.ID);
        int hashCodeCustom = "#add_menu_not_joined".GetHashCodeCustom();
        NetworkChat.Instance.AddMessage($"{SteamFriends.GetFriendPersonaName(iParam.mSteamID)} {LanguageManager.Instance.GetString(hashCodeCustom)}");
        ClientConnectedMessage iMessage2;
        iMessage2.ID = mPendingClient.ID;
        for (int iPeer = 0; iPeer < this.mClients.Count - 1; ++iPeer)
          this.SendMessage<ClientConnectedMessage>(ref iMessage2, iPeer);
        for (int index2 = 0; index2 < this.mClients.Count - 1; ++index2)
        {
          iMessage2.ID = this.mClients[index2].ID;
          this.SendMessage<ClientConnectedMessage>(ref iMessage2, mPendingClient.ID);
        }
        bool[] ready = SubMenuCharacterSelect.Instance.Ready;
        for (int index3 = 0; index3 < this.mClients.Count - 1; ++index3)
        {
          GamerReadyMessage iMessage3;
          iMessage3.Id = (byte) index3;
          iMessage3.Ready = ready[index3];
          this.SendMessage<GamerReadyMessage>(ref iMessage3, mPendingClient.ID);
        }
        Player[] players = Magicka.Game.Instance.Players;
        for (int iID = 0; iID < 4; ++iID)
        {
          if (players[iID].Playing)
          {
            GamerJoinRequestMessage iMessage4;
            iMessage4.Id = (sbyte) iID;
            iMessage4.Gamer = players[iID].Gamer.GamerTag;
            iMessage4.AvatarThumb = players[iID].Gamer.Avatar.ThumbPath;
            iMessage4.AvatarPortrait = players[iID].Gamer.Avatar.PortraitPath;
            iMessage4.AvatarType = players[iID].Gamer.Avatar.TypeName;
            iMessage4.Color = players[iID].Gamer.Color;
            iMessage4.SteamID = !(players[iID].Gamer is NetworkGamer) ? this.ServerID : (players[iID].Gamer as NetworkGamer).ClientID;
            this.SendMessage<GamerJoinRequestMessage>(ref iMessage4, mPendingClient.ID);
            this.SendMessage<GamerReadyMessage>(ref new GamerReadyMessage()
            {
              Id = (byte) iID,
              Ready = SubMenuCharacterSelect.Instance.GetReady(iID)
            }, mPendingClient.ID);
            this.SendMessage<MenuSelectMessage>(ref new MenuSelectMessage()
            {
              IntendedMenu = MenuSelectMessage.MenuType.CharacterSelect,
              Option = 2,
              Param0I = iID,
              Param1I = (int) players[iID].Team
            }, mPendingClient.ID);
          }
        }
        SteamFriends.GetFriendPersonaName(iParam.mSteamID);
        SteamFriends.GetFriendAvatar(iParam.mSteamID, AvatarSize.AvatarSize32x32);
        SteamFriends.GetFriendAvatar(iParam.mSteamID, AvatarSize.AvatarSize64x64);
        this.mPendingClients.RemoveAt(index1);
        break;
      }
    }
    SteamMatchmaking.SetLobbyData(this.mSteamIDLobby, "slots", (4 - this.mClients.Count).ToString());
  }

  private void SteamGameServer_GSClientDeny(GSClientDeny iParam)
  {
    for (int index = 0; index < this.mPendingClients.Count; ++index)
    {
      NetworkServer.PendingUser mPendingClient = this.mPendingClients[index];
      if (mPendingClient.ID == iParam.mSteamID)
      {
        this.SendMessage<AuthenticateReplyMessage>(ref new AuthenticateReplyMessage()
        {
          Response = AuthenticateReplyMessage.Reply.Error_AuthFailed
        }, mPendingClient.ID);
        this.mPendingClients.RemoveAt(index);
        break;
      }
    }
  }

  private void SteamGameServer_GSClientKick(GSClientKick iParam)
  {
    ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_AUTHFAIL));
    NetworkManager.Instance.EndSession();
  }

  private void SteamGameServer_P2PSessionRequest(P2PSessionRequest iParam)
  {
    SteamGameServerNetworking.AcceptP2PSessionWithUser(iParam.mSteamIDRemote);
  }

  private void SteamGameServer_P2PSessionConnectFail(P2PSessionConnectFail iParam)
  {
    for (int index = 0; index < this.mClients.Count; ++index)
    {
      if (this.mClients[index].ID == iParam.mSteamIDRemote)
      {
        this.CloseConnection(iParam.mSteamIDRemote, ConnectionClosedMessage.CReason.LostConnection);
        SteamGameServer.SendUserDisconnect(iParam.mSteamIDRemote);
      }
    }
    for (int index = 0; index < this.mPendingClients.Count; ++index)
    {
      if (this.mPendingClients[index].ID == iParam.mSteamIDRemote)
      {
        this.mPendingClients.RemoveAt(index);
        SteamGameServer.SendUserDisconnect(iParam.mSteamIDRemote);
      }
    }
  }

  internal void UpdateLevel(string iLevelName)
  {
    this.mLevelName = iLevelName;
    string str = $"{(ValueType) (byte) this.mGameType}|{this.mLevelName}";
    SteamGameServer.UpdateServerStatus(this.mClients.Count + 1, 4, 0, NetworkManager.Instance.GameName, str, str, !string.IsNullOrEmpty(NetworkManager.Instance.Password));
  }

  private unsafe void SendUpdatedGameDataToSteam()
  {
    SteamMasterServerUpdater.SetBasicServerData((ushort) 7, true, "255", "Magicka", (ushort) 4, !string.IsNullOrEmpty(this.mPassword), "Magicka");
    SteamMasterServerUpdater.SetActive(true);
    string str = $"{(ValueType) (byte) this.mGameType}|{this.mLevelName}";
    SteamGameServer.UpdateServerStatus(this.mClients.Count + 1, 4, 0, NetworkManager.Instance.GameName, str, str, !string.IsNullOrEmpty(NetworkManager.Instance.Password));
    SteamID steamId1 = SteamUser.GetSteamID();
    P2PSessionState pConnectionState1;
    SteamGameServerNetworking.GetP2PSessionState(steamId1, out pConnectionState1);
    SteamID steamId2 = SteamGameServer.GetSteamID();
    P2PSessionState pConnectionState2;
    SteamGameServerNetworking.GetP2PSessionState(steamId2, out pConnectionState2);
    AuthenticationToken authenticationToken = new AuthenticationToken();
    int publicIp = (int) SteamGameServer.GetPublicIP();
    authenticationToken.Length = SteamUser.InitiateGameConnection((void*) authenticationToken.Data, 1024 /*0x0400*/, steamId2, pConnectionState2.RemoteIP, (ushort) 27015, true);
    if (SteamGameServer.SendUserConnectAndAuthenticate(pConnectionState1.RemoteIP, (void*) authenticationToken.Data, (uint) authenticationToken.Length, ref steamId1))
      return;
    SteamGameServer.SendUserDisconnect(steamId1);
  }

  public override void Sync()
  {
    EnterSyncMessage iMessage1;
    lock (this)
    {
      ++this.mSyncPointCounter;
      iMessage1.ID = this.mSyncPointCounter;
    }
    this.SendMessage<EnterSyncMessage>(ref iMessage1);
    Thread.Sleep(100);
    while (!this.AllClientsReachedSyncPoint(iMessage1.ID))
      Thread.Sleep(10);
    LeaveSyncMessage iMessage2;
    iMessage2.ID = iMessage1.ID;
    this.SendMessage<LeaveSyncMessage>(ref iMessage2);
    lock (this.mClients)
    {
      for (int index = 0; index < this.mClients.Count; ++index)
        this.mClients[index].SyncPoints.Remove(iMessage1.ID);
    }
  }

  private bool AllClientsReachedSyncPoint(uint iID)
  {
    bool flag = true;
    lock (this.mClients)
    {
      for (int index = 0; index < this.mClients.Count; ++index)
      {
        if (!this.mClients[index].SyncPoints.Contains(iID))
        {
          flag = false;
          break;
        }
      }
    }
    return flag;
  }

  public override void Update()
  {
    long ticks = DateTime.Now.Ticks;
    int num = 0;
    for (int index = 0; index < this.mBytesSent.Count; ++index)
    {
      if (ticks - this.mBytesSent.Keys[index] > 10000000L)
      {
        this.mBytesSent.RemoveAt(index);
        --index;
      }
      else
        num += this.mBytesSent.Values[index];
    }
    this.mBytesPerSecond = num;
    if (ticks - this.mLastLatencyCheck > 10000000L)
    {
      this.mLastLatencyCheck = ticks;
      for (int index = 0; index < this.mClients.Count; ++index)
      {
        this.mClients[index].Timer.Reset();
        this.mClients[index].Timer.Start();
      }
      this.SendUdpMessage<PingRequestMessage>(ref new PingRequestMessage()
      {
        Payload = (int) ticks
      });
    }
    SteamGameServer.RunCallbacks();
    uint pcubMsgSize;
    SteamID psteamIDRemote;
    while (SteamGameServerNetworking.IsP2PPacketAvailable(out pcubMsgSize) && SteamGameServerNetworking.ReadP2PPacket(this.mRxBuffer, out pcubMsgSize, out psteamIDRemote))
    {
      this.mReader.BaseStream.Position = 0L;
      while (this.mReader.BaseStream.Position < (long) pcubMsgSize)
        this.ReadMessage(this.mReader, psteamIDRemote);
    }
  }

  public override void FlushMessageBuffers()
  {
    for (int index = 0; index < this.mClients.Count; ++index)
    {
      NetworkServer.Connection mClient = this.mClients[index];
      uint position = (uint) mClient.Writer.BaseStream.Position;
      if (position > 0U)
        this.FlushUDPBatch(index, position);
      P2PSessionState pConnectionState;
      SteamGameServerNetworking.GetP2PSessionState(mClient.ID, out pConnectionState);
      if (pConnectionState.ConnectionActive && pConnectionState.PacketsQueuedForSend > 0)
        SteamGameServerNetworking.SendP2PPacket(mClient.ID, this.mDummyData, 0U, P2PSend.Reliable);
    }
  }

  private int GetClient(SteamID iID)
  {
    for (int index = 0; index < this.mClients.Count; ++index)
    {
      if (this.mClients[index].ID == iID)
        return index;
    }
    return -1;
  }

  private unsafe void ReadMessage(BinaryReader iReader, SteamID iSender)
  {
    PacketType packetType1 = PacketType.ChatMessage;
    try
    {
      packetType1 = (PacketType) iReader.ReadByte();
      if ((packetType1 & PacketType.Request) == PacketType.Request)
      {
        packetType1 &= ~PacketType.Request;
        PacketType packetType2 = packetType1;
        if ((uint) packetType2 <= 11U)
        {
          switch (packetType2)
          {
            case PacketType.Ping:
              PingRequestMessage pingRequestMessage = new PingRequestMessage();
              pingRequestMessage.Read(iReader);
              this.SendUdpMessage<PingReplyMessage>(ref new PingReplyMessage()
              {
                Payload = pingRequestMessage.Payload
              }, iSender);
              return;
            case PacketType.Connect:
              new ConnectRequestMessage().Read(iReader);
              this.SendMessage<ConnectReplyMessage>(ref new ConnectReplyMessage()
              {
                ServerID = SteamGameServer.GetSteamID(),
                VACSecure = SteamGameServer.BSecure(),
                ServerName = NetworkManager.Instance.GameName
              }, iSender);
              return;
            case PacketType.Authenticate:
              AuthenticateRequestMessage authenticateRequestMessage = new AuthenticateRequestMessage();
              authenticateRequestMessage.Read(iReader);
              if ((long) authenticateRequestMessage.Version != (long) Magicka.Game.Instance.Version)
              {
                this.SendMessage<AuthenticateReplyMessage>(ref new AuthenticateReplyMessage()
                {
                  Response = AuthenticateReplyMessage.Reply.Error_Version
                }, iSender);
                for (int index = 0; index < this.mPendingClients.Count; ++index)
                {
                  if (this.mPendingClients[index].ID == iSender)
                    this.mPendingClients.RemoveAt(index--);
                }
                return;
              }
              if (!string.IsNullOrEmpty(this.mPassword) && !string.Equals(authenticateRequestMessage.Password, this.mPassword))
              {
                this.SendMessage<AuthenticateReplyMessage>(ref new AuthenticateReplyMessage()
                {
                  Response = AuthenticateReplyMessage.Reply.Error_Password
                }, iSender);
                for (int index = 0; index < this.mPendingClients.Count; ++index)
                {
                  if (this.mPendingClients[index].ID == iSender)
                    this.mPendingClients.RemoveAt(index--);
                }
                return;
              }
              bool flag = false;
              for (int index = 0; index < this.mClients.Count; ++index)
              {
                if (this.mClients[index].ID == iSender)
                {
                  flag = true;
                  break;
                }
              }
              if (flag)
                return;
              if (Math.Max(this.mClients.Count + 1, Magicka.Game.Instance.PlayerCount) + this.mPendingClients.Count + 1 > 4)
              {
                this.SendMessage<AuthenticateReplyMessage>(ref new AuthenticateReplyMessage()
                {
                  Response = AuthenticateReplyMessage.Reply.Error_ServerFull
                }, iSender);
                for (int index = 0; index < this.mPendingClients.Count; ++index)
                {
                  if (this.mPendingClients[index].ID == iSender)
                    this.mPendingClients.RemoveAt(index--);
                }
                return;
              }
              if (!this.mPlaying)
              {
                NetworkServer.PendingUser pendingUser;
                pendingUser.ID = iSender;
                pendingUser.Players = authenticateRequestMessage.NrOfPlayers;
                if (this.mPendingClients.Contains(pendingUser))
                  return;
                this.mPendingClients.Add(pendingUser);
                P2PSessionState pConnectionState;
                SteamGameServerNetworking.GetP2PSessionState(iSender, out pConnectionState);
                SteamID pSteamIDUser = iSender;
                if (SteamGameServer.SendUserConnectAndAuthenticate(pConnectionState.RemoteIP, (void*) authenticateRequestMessage.Token.Data, (uint) authenticateRequestMessage.Token.Length, ref pSteamIDUser))
                  return;
                this.SendMessage<AuthenticateReplyMessage>(ref new AuthenticateReplyMessage()
                {
                  Response = AuthenticateReplyMessage.Reply.Error_AuthFailed
                }, iSender);
                for (int index = 0; index < this.mPendingClients.Count; ++index)
                {
                  if (this.mPendingClients[index].ID == iSender)
                    this.mPendingClients.RemoveAt(index--);
                }
                SteamGameServer.SendUserDisconnect(pSteamIDUser);
                return;
              }
              this.SendMessage<AuthenticateReplyMessage>(ref new AuthenticateReplyMessage()
              {
                Response = AuthenticateReplyMessage.Reply.Error_GamePlaying
              }, iSender);
              return;
            case PacketType.GamerJoin:
              GamerJoinRequestMessage iMessage1 = new GamerJoinRequestMessage();
              iMessage1.Read(iReader);
              Player player = Player.Join((Controller) null, (int) iMessage1.Id, (Gamer) new NetworkGamer(iMessage1.Gamer, iMessage1.Color, iMessage1.AvatarThumb, iMessage1.AvatarType, iSender));
              if (player == null)
                return;
              GamerJoinAcceptMessage iMessage2;
              iMessage2.Gamer = iMessage1.Gamer;
              iMessage2.Id = (sbyte) player.ID;
              this.SendMessage<GamerJoinAcceptMessage>(ref iMessage2, iSender);
              iMessage1.Id = (sbyte) player.ID;
              for (int index = 0; index < this.mClients.Count; ++index)
              {
                if (iSender != this.mClients[index].ID)
                  this.SendMessage<GamerJoinRequestMessage>(ref iMessage1, index);
              }
              return;
          }
        }
        else
        {
          switch (packetType2)
          {
            case PacketType.TriggerAction:
              TriggerRequestMessage iMsg = new TriggerRequestMessage();
              iMsg.Read(iReader);
              Trigger.NetworkAction(ref iMsg);
              return;
            case PacketType.SpawnShield:
              SpawnShieldRequestMessage shieldRequestMessage = new SpawnShieldRequestMessage();
              shieldRequestMessage.Read(iReader);
              Shield fromCache1 = Shield.GetFromCache(PlayState.RecentPlayState);
              ISpellCaster fromHandle1 = Entity.GetFromHandle((int) shieldRequestMessage.OwnerHandle) as ISpellCaster;
              fromCache1.Initialize(fromHandle1, shieldRequestMessage.Position, shieldRequestMessage.Radius, shieldRequestMessage.Direction, shieldRequestMessage.ShieldType, shieldRequestMessage.HitPoints, Spell.SHIELDCOLOR);
              fromCache1.PlayState.EntityManager.AddEntity((Entity) fromCache1);
              SpawnShieldMessage iMessage3;
              iMessage3.Handle = fromCache1.Handle;
              iMessage3.OwnerHandle = shieldRequestMessage.OwnerHandle;
              iMessage3.Position = shieldRequestMessage.Position;
              iMessage3.Radius = shieldRequestMessage.Radius;
              iMessage3.Direction = shieldRequestMessage.Direction;
              iMessage3.ShieldType = shieldRequestMessage.ShieldType;
              iMessage3.HitPoints = shieldRequestMessage.HitPoints;
              this.SendMessage<SpawnShieldMessage>(ref iMessage3);
              return;
            case PacketType.SpawnBarrier:
              SpawnBarrierRequestMessage barrierRequestMessage = new SpawnBarrierRequestMessage();
              barrierRequestMessage.Read(iReader);
              Barrier fromCache2 = Barrier.GetFromCache(PlayState.RecentPlayState);
              Barrier.HitListWithBarriers fromCache3 = Barrier.HitListWithBarriers.GetFromCache();
              ISpellCaster fromHandle2 = Entity.GetFromHandle((int) barrierRequestMessage.OwnerHandle) as ISpellCaster;
              AnimatedLevelPart iAnimation1 = (AnimatedLevelPart) null;
              if (barrierRequestMessage.AnimationHandle < ushort.MaxValue)
                iAnimation1 = AnimatedLevelPart.GetFromHandle((int) barrierRequestMessage.AnimationHandle);
              fromCache2.Initialize(fromHandle2, barrierRequestMessage.Position, barrierRequestMessage.Direction, barrierRequestMessage.Scale, barrierRequestMessage.Range, barrierRequestMessage.NextDir, barrierRequestMessage.NextRotation, barrierRequestMessage.Distance, ref barrierRequestMessage.Spell, ref barrierRequestMessage.Damage, fromCache3, iAnimation1);
              fromCache2.PlayState.EntityManager.AddEntity((Entity) fromCache2);
              SpawnBarrierMessage iMessage4;
              iMessage4.Handle = fromCache2.Handle;
              iMessage4.OwnerHandle = barrierRequestMessage.OwnerHandle;
              iMessage4.AnimationHandle = barrierRequestMessage.AnimationHandle;
              iMessage4.Position = barrierRequestMessage.Position;
              iMessage4.Direction = barrierRequestMessage.Direction;
              iMessage4.Scale = barrierRequestMessage.Scale;
              iMessage4.Spell = barrierRequestMessage.Spell;
              iMessage4.Damage = barrierRequestMessage.Damage;
              iMessage4.HitlistHandle = fromCache3.Handle;
              this.SendMessage<SpawnBarrierMessage>(ref iMessage4);
              return;
            case PacketType.SpawnWave:
              SpawnWaveRequestMessage waveRequestMessage = new SpawnWaveRequestMessage();
              waveRequestMessage.Read(iReader);
              WaveEntity fromCache4 = WaveEntity.GetFromCache(PlayState.RecentPlayState);
              Barrier.HitListWithBarriers fromCache5 = Barrier.HitListWithBarriers.GetFromCache();
              ISpellCaster fromHandle3 = Entity.GetFromHandle((int) waveRequestMessage.OwnerHandle) as ISpellCaster;
              AnimatedLevelPart iAnimation2 = (AnimatedLevelPart) null;
              if (waveRequestMessage.AnimationHandle < ushort.MaxValue)
                iAnimation2 = AnimatedLevelPart.GetFromHandle((int) waveRequestMessage.AnimationHandle);
              Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Wave instance1 = Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Wave.GetInstance();
              fromCache4.Initialize(fromHandle3, waveRequestMessage.Position, waveRequestMessage.Direction, waveRequestMessage.Scale, waveRequestMessage.Range, waveRequestMessage.NextDir, waveRequestMessage.NextRotation, waveRequestMessage.Distance, ref waveRequestMessage.Spell, ref waveRequestMessage.Damage, ref fromCache5, iAnimation2, ref instance1);
              fromCache4.PlayState.EntityManager.AddEntity((Entity) fromCache4);
              SpawnWaveMessage iMessage5;
              iMessage5.Handle = fromCache4.Handle;
              iMessage5.OwnerHandle = waveRequestMessage.OwnerHandle;
              iMessage5.AnimationHandle = waveRequestMessage.AnimationHandle;
              iMessage5.ParentHandle = waveRequestMessage.ParentHandle;
              iMessage5.Position = waveRequestMessage.Position;
              iMessage5.Direction = waveRequestMessage.Direction;
              iMessage5.Scale = waveRequestMessage.Scale;
              iMessage5.Spell = waveRequestMessage.Spell;
              iMessage5.Damage = waveRequestMessage.Damage;
              iMessage5.HitlistHandle = fromCache5.Handle;
              this.SendMessage<SpawnWaveMessage>(ref iMessage5);
              return;
            case PacketType.SpawnMine:
              SpawnMineRequestMessage mineRequestMessage = new SpawnMineRequestMessage();
              mineRequestMessage.Read(iReader);
              SpellMine instance2 = SpellMine.GetInstance();
              ISpellCaster fromHandle4 = Entity.GetFromHandle((int) mineRequestMessage.OwnerHandle) as ISpellCaster;
              AnimatedLevelPart iAnimation3 = (AnimatedLevelPart) null;
              if (mineRequestMessage.AnimationHandle < ushort.MaxValue)
                iAnimation3 = AnimatedLevelPart.GetFromHandle((int) mineRequestMessage.AnimationHandle);
              instance2.Initialize(fromHandle4, mineRequestMessage.Position, mineRequestMessage.Direction, mineRequestMessage.Scale, mineRequestMessage.Range, mineRequestMessage.NextDir, mineRequestMessage.NextRotation, mineRequestMessage.Distance, ref mineRequestMessage.Spell, ref mineRequestMessage.Damage, iAnimation3);
              instance2.PlayState.EntityManager.AddEntity((Entity) instance2);
              SpawnMineMessage iMessage6;
              iMessage6.Handle = instance2.Handle;
              iMessage6.OwnerHandle = mineRequestMessage.OwnerHandle;
              iMessage6.AnimationHandle = mineRequestMessage.AnimationHandle;
              iMessage6.Position = mineRequestMessage.Position;
              iMessage6.Direction = mineRequestMessage.Direction;
              iMessage6.Scale = mineRequestMessage.Scale;
              iMessage6.Spell = mineRequestMessage.Spell;
              iMessage6.Damage = mineRequestMessage.Damage;
              this.SendMessage<SpawnMineMessage>(ref iMessage6);
              return;
            case PacketType.SpawnVortex:
              SpawnVortexMessage spawnVortexMessage = new SpawnVortexMessage();
              spawnVortexMessage.Read(iReader);
              VortexEntity specificInstance = VortexEntity.GetSpecificInstance(spawnVortexMessage.Handle);
              Entity fromHandle5 = Entity.GetFromHandle((int) spawnVortexMessage.OwnerHandle);
              specificInstance.Initialize(fromHandle5 as ISpellCaster, spawnVortexMessage.Position);
              specificInstance.PlayState.EntityManager.AddEntity((Entity) specificInstance);
              return;
            case PacketType.Damage:
              DamageRequestMessage damageRequestMessage = new DamageRequestMessage();
              damageRequestMessage.Read(iReader);
              Entity fromHandle6 = Entity.GetFromHandle((int) damageRequestMessage.AttackerHandle);
              IDamageable fromHandle7 = Entity.GetFromHandle((int) damageRequestMessage.TargetHandle) as IDamageable;
              if (fromHandle7.Dead && fromHandle7 is Avatar && (fromHandle7 as Avatar).Player != null)
                return;
              Vector3 result = damageRequestMessage.RelativeAttackPosition;
              Vector3 position = fromHandle7.Position;
              Vector3.Add(ref position, ref result, out result);
              Defines.DamageFeatures iFeatures = Defines.DamageFeatures.DNK;
              int num = (int) fromHandle7.InternalDamage(damageRequestMessage.Damage, fromHandle6, damageRequestMessage.TimeStamp, result, iFeatures);
              return;
          }
        }
        throw new Exception($"Unhandled request ({packetType1.ToString()})!");
      }
      PacketType packetType3 = packetType1;
      if ((uint) packetType3 <= 23U)
      {
        switch (packetType3)
        {
          case PacketType.Ping:
            PingReplyMessage pingReplyMessage = new PingReplyMessage();
            pingReplyMessage.Read(iReader);
            if (pingReplyMessage.Payload != (int) this.mLastLatencyCheck || this.mClients == null)
              return;
            for (int index = 0; index < this.mClients.Count; ++index)
            {
              NetworkServer.Connection mClient = this.mClients[index];
              if (mClient.ID == iSender)
              {
                mClient.Timer.Stop();
                mClient.Latency = (float) mClient.Timer.Elapsed.TotalSeconds;
                this.mClients[index] = mClient;
                break;
              }
            }
            return;
          case PacketType.ConnectionClosed:
            ConnectionClosedMessage connectionClosedMessage = new ConnectionClosedMessage();
            connectionClosedMessage.Read(iReader);
            this.CloseConnection(iSender, connectionClosedMessage.Reason);
            return;
          case PacketType.ChatMessage:
            ChatMessage chatMessage = new ChatMessage();
            chatMessage.Read(iReader);
            NetworkChat.Instance.AddMessage(chatMessage.ToString());
            return;
          case PacketType.GamerChanged:
            GamerChangedMessage gamerChangedMessage = new GamerChangedMessage();
            gamerChangedMessage.Read(iReader);
            Player player1 = Magicka.Game.Instance.Players[(int) gamerChangedMessage.Id];
            player1.UnlockedMagicks = gamerChangedMessage.UnlockedMagicks;
            Gamer gamer = player1.Gamer;
            gamer.Color = gamerChangedMessage.Color;
            gamer.GamerTag = gamerChangedMessage.GamerTag;
            Profile.PlayableAvatar playableAvatar = new Profile.PlayableAvatar();
            playableAvatar.ThumbPath = gamerChangedMessage.AvatarThumb;
            lock (Magicka.Game.Instance.GraphicsDevice)
              playableAvatar.Thumb = Magicka.Game.Instance.Content.Load<Texture2D>(playableAvatar.ThumbPath);
            playableAvatar.PortraitPath = gamerChangedMessage.AvatarPortrait;
            lock (Magicka.Game.Instance.GraphicsDevice)
              playableAvatar.Portrait = Magicka.Game.Instance.Content.Load<Texture2D>(playableAvatar.PortraitPath);
            playableAvatar.TypeName = gamerChangedMessage.AvatarType;
            playableAvatar.Type = playableAvatar.TypeName.GetHashCodeCustom();
            playableAvatar.Name = player1.Gamer.Avatar.Name;
            playableAvatar.AllowCampaign = gamerChangedMessage.AvatarAllowCampaign;
            playableAvatar.AllowChallenge = gamerChangedMessage.AvatarAllowChallenge;
            playableAvatar.AllowPVP = gamerChangedMessage.AvatarAllowPVP;
            player1.Gamer.Avatar = playableAvatar;
            SubMenuCharacterSelect.Instance.UpdateGamer(player1, gamer);
            return;
          case PacketType.GamerLeave:
            GamerLeaveMessage gamerLeaveMessage = new GamerLeaveMessage();
            gamerLeaveMessage.Read(iReader);
            SubMenuCharacterSelect.Instance.SetReady(false);
            Player player2 = Magicka.Game.Instance.Players[(int) gamerLeaveMessage.Id];
            player2.Playing = false;
            player2.Team = Factions.NONE;
            player2.Gamer = (Gamer) null;
            SubMenuCharacterSelect.Instance.UpdateGamer(player2, (Gamer) null);
            if (!SubMenuCharacterSelect.Instance.NeedToUpdateDefaultAvatarsUponClientLeaving())
              return;
            SubMenuCharacterSelect.Instance.DefaultAvatars();
            return;
          case PacketType.GamerReady:
            GamerReadyMessage gamerReadyMessage = new GamerReadyMessage();
            gamerReadyMessage.Read(iReader);
            SubMenuCharacterSelect.Instance.SetReady(gamerReadyMessage.Ready, gamerReadyMessage.Id);
            return;
          case PacketType.MenuSelection:
            MenuSelectMessage iMessage = new MenuSelectMessage();
            iMessage.Read(iReader);
            if (GameStateManager.Instance.CurrentState is MenuState currentState)
              currentState.NetworkInput(ref iMessage);
            for (int index = 0; index < this.mClients.Count; ++index)
            {
              SteamID id = this.mClients[index].ID;
              if (id != iSender)
                this.SendMessage<MenuSelectMessage>(ref iMessage, id);
            }
            return;
          case PacketType.GameEndLoad:
            new GameEndLoadMessage().Read(iReader);
            for (int index = 0; index < this.mClients.Count; ++index)
            {
              NetworkServer.Connection mClient = this.mClients[index];
              if (mClient.ID == iSender)
              {
                mClient.Ready = true;
                this.mClients[index] = mClient;
                break;
              }
            }
            return;
          case PacketType.TriggerAction:
            TriggerActionMessage iMsg1 = new TriggerActionMessage();
            iMsg1.Read(iReader);
            Trigger.NetworkAction(ref iMsg1);
            return;
        }
      }
      else if ((uint) packetType3 <= 37U)
      {
        switch (packetType3)
        {
          case PacketType.DialogAdvance:
            DialogAdvanceMessage dialogAdvanceMessage = new DialogAdvanceMessage();
            dialogAdvanceMessage.Read(iReader);
            DialogManager.Instance.NetworkAdvance(dialogAdvanceMessage.Interact);
            return;
          case PacketType.EntityUpdate:
            EntityUpdateMessage iMsg2 = new EntityUpdateMessage();
            iMsg2.Read(iReader);
            Entity.GetFromHandle((int) iMsg2.Handle)?.NetworkUpdate(iSender, ref iMsg2);
            return;
          case PacketType.CharacterAction:
            CharacterActionMessage iMsg3 = new CharacterActionMessage();
            iMsg3.Read(iReader);
            Entity fromHandle8 = Entity.GetFromHandle((int) iMsg3.Handle);
            if (fromHandle8 == null || !(fromHandle8 is Character character))
              return;
            character.NetworkAction(ref iMsg3);
            return;
          case PacketType.SpawnMissile:
            SpawnMissileMessage spawnMissileMessage = new SpawnMissileMessage();
            spawnMissileMessage.Read(iReader);
            if (!(Entity.GetFromHandle((int) spawnMissileMessage.Handle) is MissileEntity fromHandle9))
              throw new Exception("Caches out of sync!");
            switch (spawnMissileMessage.Type)
            {
              case SpawnMissileMessage.MissileType.Spell:
                ProjectileSpell.SpawnMissile(ref fromHandle9, Entity.GetFromHandle((int) spawnMissileMessage.Owner) as ISpellCaster, spawnMissileMessage.Homing, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity, ref spawnMissileMessage.Spell, spawnMissileMessage.Splash, (int) spawnMissileMessage.Item);
                return;
              case SpawnMissileMessage.MissileType.Item:
                Item fromHandle10 = Entity.GetFromHandle((int) spawnMissileMessage.Item) as Item;
                Entity fromHandle11 = Entity.GetFromHandle((int) spawnMissileMessage.Owner);
                if (fromHandle10.ProjectileModel != null)
                  fromHandle9.Initialize(fromHandle11, fromHandle10.ProjectileModel.Meshes[0].BoundingSphere.Radius, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity, fromHandle10.ProjectileModel, fromHandle10.RangedConditions, false);
                else
                  fromHandle9.Initialize(fromHandle11, 0.75f, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity, (Model) null, fromHandle10.RangedConditions, false);
                fromHandle9.Danger = fromHandle10.Danger;
                fromHandle9.Homing = fromHandle10.Homing;
                fromHandle9.FacingVelocity = fromHandle10.Facing;
                fromHandle9.PlayState.EntityManager.AddEntity((Entity) fromHandle9);
                return;
              case SpawnMissileMessage.MissileType.HolyGrenade:
                HolyGrenade.SpawnGrenade(ref fromHandle9, Entity.GetFromHandle((int) spawnMissileMessage.Owner) as ISpellCaster, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity);
                return;
              case SpawnMissileMessage.MissileType.Grenade:
                Grenade.SpawnGrenade(ref fromHandle9, Entity.GetFromHandle((int) spawnMissileMessage.Owner) as ISpellCaster, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity);
                return;
              case SpawnMissileMessage.MissileType.FireFlask:
                Fireflask.SpawnGrenade(ref fromHandle9, Entity.GetFromHandle((int) spawnMissileMessage.Owner) as ISpellCaster, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity);
                return;
              case SpawnMissileMessage.MissileType.ProppMagick:
                ProppMagick.Spawn(ref fromHandle9, Entity.GetFromHandle((int) spawnMissileMessage.Owner) as ISpellCaster, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity, ref spawnMissileMessage.AngularVelocity, ref spawnMissileMessage.Lever);
                return;
              case SpawnMissileMessage.MissileType.JudgementMissile:
                Entity fromHandle12 = Entity.GetFromHandle((int) spawnMissileMessage.Target);
                JudgementSpray.SpawnProjectile(ref fromHandle9, Entity.GetFromHandle((int) spawnMissileMessage.Owner) as ISpellCaster, ref fromHandle12, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity);
                return;
              case SpawnMissileMessage.MissileType.GreaseLump:
                Entity.GetFromHandle((int) spawnMissileMessage.Target);
                GreaseLump.SpawnLump(ref fromHandle9, Entity.GetFromHandle((int) spawnMissileMessage.Owner) as ISpellCaster, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity);
                return;
              case SpawnMissileMessage.MissileType.PotionFlask:
                Entity.GetFromHandle((int) spawnMissileMessage.Target);
                Potion.SpawnFlask(ref fromHandle9, Entity.GetFromHandle((int) spawnMissileMessage.Owner) as ISpellCaster, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity);
                return;
              default:
                throw new Exception();
            }
        }
      }
      else
      {
        switch (packetType3)
        {
          case PacketType.SpawnVortex:
            SpawnVortexMessage spawnVortexMessage1 = new SpawnVortexMessage();
            spawnVortexMessage1.Read(iReader);
            VortexEntity specificInstance1 = VortexEntity.GetSpecificInstance(spawnVortexMessage1.Handle);
            Entity fromHandle13 = Entity.GetFromHandle((int) spawnVortexMessage1.OwnerHandle);
            specificInstance1.Initialize(fromHandle13 as ISpellCaster, spawnVortexMessage1.Position);
            specificInstance1.PlayState.EntityManager.AddEntity((Entity) specificInstance1);
            return;
          case PacketType.MissileEntity:
            MissileEntityEventMessage iMsg4 = new MissileEntityEventMessage();
            iMsg4.Read(iReader);
            (Entity.GetFromHandle((int) iMsg4.Handle) as MissileEntity).NetworkEventMessage(ref iMsg4);
            return;
          case PacketType.EnterSync:
            EnterSyncMessage enterSyncMessage = new EnterSyncMessage();
            enterSyncMessage.Read(iReader);
            int client = this.GetClient(iSender);
            lock (this.mClients)
            {
              this.mClients[client].SyncPoints.Add(enterSyncMessage.ID);
              return;
            }
          case PacketType.RequestForcedPlayerStatusSync:
            RequestForcedPlayerStatusSync playerStatusSync = new RequestForcedPlayerStatusSync();
            playerStatusSync.Read(iReader);
            if (!(Entity.GetFromHandle((int) playerStatusSync.Handle) is Character fromHandle14) || !(fromHandle14 is Avatar avatar) || avatar.Player == null)
              return;
            this.SendForcedSyncMessageToClient(iSender, false);
            return;
        }
      }
      throw new Exception($"Unhandled message type ({packetType1.ToString()})!");
    }
    catch (IOException ex)
    {
      this.CloseConnection(iSender, ConnectionClosedMessage.CReason.LostConnection);
    }
    catch (NullReferenceException ex)
    {
      string str = SteamFriends.GetFriendPersonaName(iSender);
      if (string.IsNullOrEmpty(str))
        str = "--";
      throw new Exception($"System.NullReferenceException when reading message of type: {packetType1.ToString()} from {str} !", (Exception) ex);
    }
  }

  public void SetAllClientsBusy()
  {
    for (int index = 0; index < this.mClients.Count; ++index)
    {
      NetworkServer.Connection mClient = this.mClients[index] with
      {
        Ready = false
      };
      this.mClients[index] = mClient;
    }
  }

  public bool AllClientsReady
  {
    get
    {
      for (int index = 0; index < this.mClients.Count; ++index)
      {
        if (!this.mClients[index].Ready)
          return false;
      }
      return true;
    }
  }

  public override void SendMessage<T>(ref T iMessage, P2PSend sendType)
  {
    lock (this.mTxBuffer)
    {
      this.mTxBuffer[0] = (byte) iMessage.PacketType;
      this.mWriter.BaseStream.Position = 1L;
      iMessage.Write(this.mWriter);
      uint position = (uint) this.mWriter.BaseStream.Position;
      for (int index = 0; index < this.mClients.Count; ++index)
      {
        SteamGameServerNetworking.SendP2PPacket(this.mClients[index].ID, this.mTxBuffer, position, sendType);
        long ticks = DateTime.Now.Ticks;
        int num;
        if (!this.mBytesSent.TryGetValue(ticks, out num))
          num = 0;
        this.mBytesSent[ticks] = num + (int) position;
      }
    }
  }

  public override void SendMessage<T>(ref T iMessage, int iClientIndex)
  {
    this.SendMessage<T>(ref iMessage, iClientIndex, P2PSend.ReliableWithBuffering);
  }

  public override void SendMessage<T>(ref T iMessage, int iClientIndex, P2PSend sendType)
  {
    this.SendMessage<T>(ref iMessage, this.mClients[iClientIndex].ID, sendType);
  }

  public override void SendMessage<T>(ref T iMessage, SteamID iClientID)
  {
    this.SendMessage<T>(ref iMessage, iClientID, P2PSend.ReliableWithBuffering);
  }

  public override void SendMessage<T>(ref T iMessage, SteamID iClientID, P2PSend sendType)
  {
    lock (this.mTxBuffer)
    {
      this.mTxBuffer[0] = (byte) iMessage.PacketType;
      this.mWriter.BaseStream.Position = 1L;
      iMessage.Write(this.mWriter);
      uint position = (uint) this.mWriter.BaseStream.Position;
      SteamGameServerNetworking.SendP2PPacket(iClientID, this.mTxBuffer, position, sendType);
      long ticks = DateTime.Now.Ticks;
      int num;
      if (!this.mBytesSent.TryGetValue(ticks, out num))
        num = 0;
      this.mBytesSent[ticks] = num + (int) position;
    }
  }

  public override void SendUdpMessage<T>(ref T iMessage)
  {
    lock (this.mTxBuffer)
    {
      for (int iClient = 0; iClient < this.mClients.Count; ++iClient)
        this.QueueUDPMessage<T>(iClient, ref iMessage);
    }
  }

  public override void SendUdpMessage<T>(ref T iMessage, int iClientIndex)
  {
    lock (this.mTxBuffer)
      this.QueueUDPMessage<T>(iClientIndex, ref iMessage);
  }

  public override void SendUdpMessage<T>(ref T iMessage, SteamID iClientID)
  {
    int client = this.GetClient(iClientID);
    if (client >= 0)
    {
      this.QueueUDPMessage<T>(client, ref iMessage);
    }
    else
    {
      this.mTxBuffer[0] = (byte) iMessage.PacketType;
      this.mWriter.BaseStream.Position = 1L;
      iMessage.Write(this.mWriter);
      uint position = (uint) this.mWriter.BaseStream.Position;
      SteamGameServerNetworking.SendP2PPacket(iClientID, this.mTxBuffer, position, P2PSend.UnreliableNoDelay);
      long ticks = DateTime.Now.Ticks;
      int num;
      if (!this.mBytesSent.TryGetValue(ticks, out num))
        num = 0;
      this.mBytesSent[ticks] = num + (int) position;
    }
  }

  public override unsafe void SendRaw(PacketType iType, void* iPtr, int iLength)
  {
    for (int index = 0; index < this.mClients.Count; ++index)
      this.SendRaw(iType, iPtr, iLength, this.mClients[index].ID);
  }

  public override unsafe void SendRaw(PacketType iType, void* iPtr, int iLength, int iPeer)
  {
    this.SendRaw(iType, iPtr, iLength, this.mClients[iPeer].ID);
  }

  public override unsafe void SendRaw(PacketType iType, void* iPtr, int iLength, SteamID iPeer)
  {
    byte[] numArray = new byte[iLength + 1 + 4];
    numArray[0] = (byte) iType;
    numArray[1] = (byte) iLength;
    numArray[2] = (byte) (iLength >> 8);
    numArray[3] = (byte) (iLength >> 16 /*0x10*/);
    numArray[4] = (byte) (iLength >> 24);
    if (iLength > 0)
      Marshal.Copy(new IntPtr(iPtr), numArray, 5, iLength);
    SteamGameServerNetworking.SendP2PPacket(iPeer, numArray, (uint) numArray.Length, P2PSend.ReliableWithBuffering);
  }

  public override void Dispose()
  {
    Player[] players = Magicka.Game.Instance.Players;
    for (int index = 0; index < players.Length; ++index)
    {
      if (players[index].Playing)
      {
        this.SendMessage<GamerLeaveMessage>(ref new GamerLeaveMessage()
        {
          Id = (byte) index
        });
        if (players[index].Gamer is NetworkGamer)
          players[index].Playing = false;
      }
    }
    NetworkManager.Instance.ClearHost();
    this.mListening = false;
    for (int index = 0; index < this.mClients.Count; ++index)
      SteamNetworking.CloseP2PSessionWithUser(this.mClients[index].ID);
    SteamMasterServerUpdater.SetActive(false);
    SteamMatchmaking.LeaveLobby(this.mSteamIDLobby);
    SteamGameServer.SteamServersConnected -= new Action<SteamServersConnected>(this.SteamGameServer_SteamServersConnected);
    SteamGameServer.SteamServersDisconnected -= new Action<SteamServersDisconnected>(this.SteamGameServer_SteamServersDisconnected);
    SteamGameServer.GSPolicyResponse -= new Action<GSPolicyResponse>(this.SteamGameServer_GSPolicyResponse);
    SteamGameServer.GSClientApprove -= new Action<GSClientApprove>(this.SteamGameServer_GSClientApprove);
    SteamGameServer.GSClientDeny -= new Action<GSClientDeny>(this.SteamGameServer_GSClientDeny);
    SteamGameServer.GSClientKick -= new Action<GSClientKick>(this.SteamGameServer_GSClientKick);
    SteamGameServer.P2PSessionRequest -= new Action<P2PSessionRequest>(this.SteamGameServer_P2PSessionRequest);
    SteamGameServer.P2PSessionConnectFail -= new Action<P2PSessionConnectFail>(this.SteamGameServer_P2PSessionConnectFail);
    SteamGameServer.LogOff();
    SteamMasterServerUpdater.NotifyShutdown();
    SteamGameServer.Shutdown();
    Magicka.Game.Instance.AddLoadTask((Action) (() =>
    {
      try
      {
        if (!this.mUPnPEnabled)
          return;
        NAT.DeleteForwardingRule(27016, ProtocolType.Udp);
      }
      catch
      {
      }
    }));
  }

  public override int Connections => this.mClients.Count;

  public override float GetLatency(int iConnection) => this.mClients[iConnection].Latency;

  public override int GetLatencyMS(int iConnection)
  {
    return (int) ((double) this.mClients[iConnection].Latency * 1000.0);
  }

  public override float GetLatency(SteamID iConnection)
  {
    for (int index = 0; index < this.mClients.Count; ++index)
    {
      NetworkServer.Connection mClient = this.mClients[index];
      if (mClient.ID == iConnection)
        return mClient.Latency;
    }
    return 0.0f;
  }

  public override int GetLatencyMS(SteamID iConnection)
  {
    for (int index = 0; index < this.mClients.Count; ++index)
    {
      NetworkServer.Connection mClient = this.mClients[index];
      if (mClient.ID == iConnection)
        return (int) ((double) mClient.Latency * 1000.0);
    }
    return 0;
  }

  public void CloseConnection(SteamID iClient, ConnectionClosedMessage.CReason iReason)
  {
    for (int index = 0; index < this.mClients.Count; ++index)
    {
      if (this.mClients[index].ID == iClient)
      {
        this.CloseConnection(index, iReason);
        break;
      }
    }
  }

  public void CloseConnection(int iClient, ConnectionClosedMessage.CReason iReason)
  {
    NetworkServer.Connection mClient = this.mClients[iClient];
    switch (iReason)
    {
      case ConnectionClosedMessage.CReason.Kicked:
        NetworkChat.Instance.AddMessage($"{SteamFriends.GetFriendPersonaName(this.mClients[iClient].ID)} {LanguageManager.Instance.GetString("#add_menu_not_kicked".GetHashCodeCustom())}");
        this.SendMessage<ConnectionClosedMessage>(ref new ConnectionClosedMessage()
        {
          Reason = ConnectionClosedMessage.CReason.Kicked
        }, iClient);
        Thread.Sleep(250);
        break;
      case ConnectionClosedMessage.CReason.Left:
        NetworkChat.Instance.AddMessage($"{SteamFriends.GetFriendPersonaName(this.mClients[iClient].ID)} {LanguageManager.Instance.GetString("#add_menu_not_left".GetHashCodeCustom())}");
        break;
      default:
        NetworkChat.Instance.AddMessage(LanguageManager.Instance.GetString("#add_connectiontolost".GetHashCodeCustom()).Replace("#1;", SteamFriends.GetFriendPersonaName(this.mClients[iClient].ID)));
        break;
    }
    SteamGameServer.SendUserDisconnect(mClient.ID);
    SteamGameServerNetworking.CloseP2PSessionWithUser(mClient.ID);
    this.mClients.RemoveAt(iClient);
    foreach (Player player in Magicka.Game.Instance.Players)
    {
      if (player.Gamer is NetworkGamer gamer && gamer.ClientID == mClient.ID)
        player.Leave();
    }
    SteamMatchmaking.SetLobbyData(this.mSteamIDLobby, "slots", (4 - this.mClients.Count).ToString());
  }

  public override void CloseConnection()
  {
    this.SendMessage<ConnectionClosedMessage>(ref new ConnectionClosedMessage()
    {
      Reason = ConnectionClosedMessage.CReason.Left
    });
    Thread.Sleep(250);
  }

  internal SteamID GetSteamID(int iIndex) => this.mClients[iIndex].ID;

  private void QueueUDPMessage<T>(int iClient, ref T iMessage) where T : ISendable
  {
    NetworkServer.Connection mClient;
    lock (this.mClients)
      mClient = this.mClients[iClient];
    long position = mClient.Writer.BaseStream.Position;
    mClient.Writer.Write((byte) iMessage.PacketType);
    iMessage.Write(mClient.Writer);
    if (mClient.Writer.BaseStream.Position <= 1100L)
      return;
    this.FlushUDPBatch(iClient, (uint) position);
    mClient.Writer.Write((byte) iMessage.PacketType);
    iMessage.Write(mClient.Writer);
  }

  private void FlushUDPBatch(int iClient, uint iLength)
  {
    NetworkServer.Connection mClient;
    lock (this.mClients)
      mClient = this.mClients[iClient];
    SteamGameServerNetworking.SendP2PPacket(mClient.ID, mClient.TxBuffer, iLength, P2PSend.UnreliableNoDelay);
    long ticks = DateTime.Now.Ticks;
    int num;
    if (!this.mBytesSent.TryGetValue(ticks, out num))
      num = 0;
    lock (this.mBytesSent)
      this.mBytesSent[ticks] = num + (int) iLength;
    mClient.Writer.BaseStream.Position = 0L;
  }

  private void SendForcedSyncMessageToClient(SteamID iSteamID, bool syncAfterwards)
  {
    this.SendForcedSyncMessageToClient(this.GetClient(iSteamID), syncAfterwards);
  }

  private void SendForcedSyncMessageToClient(Avatar iAvatar, bool syncAfterwards)
  {
    if (iAvatar == null)
      return;
    Player player = iAvatar.Player;
    if (player == null || player.Gamer == null || !(player.Gamer is NetworkGamer gamer))
      return;
    this.SendForcedSyncMessageToClient(this.GetClient(gamer.ClientID), syncAfterwards);
  }

  private void SendForcedSyncMessageToClient(int clientIndex, bool syncAfterwards)
  {
    Player[] players = Magicka.Game.Instance.Players;
    ForceSyncPlayerStatusesMessage iMessage = new ForceSyncPlayerStatusesMessage();
    int length = 0;
    for (int index = 0; index < players.Length; ++index)
    {
      if (players[index] != null && players[index].Gamer != null && players[index].Gamer is NetworkGamer)
        ++length;
    }
    if (length == 0)
      return;
    iMessage.playerUpdateMessages = new EntityUpdateMessage[length];
    iMessage.numPlayers = (short) length;
    EntityUpdateMessage[] entityUpdateMessageArray = new EntityUpdateMessage[length];
    for (int index = 0; index < length; ++index)
    {
      players[index].Avatar.GetNetworkUpdate(out iMessage.playerUpdateMessages[index], NetworkState.Server, 1f);
      iMessage.playerUpdateMessages[index].Handle = (ushort) players[index].ID;
    }
    iMessage.playerUpdateMessages = entityUpdateMessageArray;
    this.SendMessage<ForceSyncPlayerStatusesMessage>(ref iMessage, clientIndex, P2PSend.Reliable);
    if (!syncAfterwards)
      return;
    this.Sync();
  }

  private struct Connection
  {
    public SteamID ID;
    public byte[] TxBuffer;
    public BinaryWriter Writer;
    public List<uint> SyncPoints;
    public Stopwatch Timer;
    public float Latency;
    public bool Ready;
  }

  private struct PendingUser
  {
    public SteamID ID;
    public byte Players;
  }
}
