// Decompiled with JetBrains decompiler
// Type: Magicka.Network.NetworkClient
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.DRM;
using Magicka.GameLogic;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.GameLogic.Statistics;
using Magicka.GameLogic.UI;
using Magicka.Gamers;
using Magicka.Levels;
using Magicka.Levels.Packs;
using Magicka.Levels.Triggers;
using Magicka.Levels.Versus;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using SteamWrapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;

#nullable disable
namespace Magicka.Network;

internal class NetworkClient : NetworkInterface
{
  private byte[] mRxBuffer;
  private byte[] mTxBuffer;
  public BinaryReader mReader;
  public BinaryWriter mWriter;
  private Queue<uint> mSyncPoints = new Queue<uint>();
  private List<uint> mFinishedSyncPoints = new List<uint>();
  private SteamID mServerID;
  private int mRetryCount;
  private List<NetworkClient.Connection> mConnections;
  private OptionsMessageBox mErrorMessageBox;
  private long mLastLatencyCheck;
  private SteamID mSteamIDLobby;
  private Action<ConnectionStatus> mOnComplete;
  private byte[] mDummyData = new byte[1];
  private bool mVAC;
  private SaveSlotInfo mSaveSlot;
  private SortedList<long, int> mBytesSent = new SortedList<long, int>();
  private int mBytesPerSecond;

  public NetworkClient(SteamID iSteamID, Action<ConnectionStatus> iOnComplete)
  {
    Magicka.GameLogic.Player[] players = Magicka.Game.Instance.Players;
    for (int index = 0; index < players.Length; ++index)
    {
      if (players[index].Playing)
        players[index].Leave();
    }
    this.mConnections = new List<NetworkClient.Connection>();
    this.mOnComplete = iOnComplete;
    this.mServerID = iSteamID;
    this.mTxBuffer = new byte[1048576 /*0x100000*/];
    this.mRxBuffer = new byte[1048576 /*0x100000*/];
    this.mReader = new BinaryReader((Stream) new MemoryStream(this.mRxBuffer));
    this.mWriter = new BinaryWriter((Stream) new MemoryStream(this.mTxBuffer));
    NetworkClient.Connection connection;
    connection.ID = this.mServerID;
    connection.Timer = new Stopwatch();
    connection.Latency = 0.0f;
    connection.TxBuffer = new byte[4400];
    connection.Writer = new BinaryWriter((Stream) new MemoryStream(connection.TxBuffer));
    this.mConnections.Add(connection);
    ConnectRequestMessage iMessage = new ConnectRequestMessage();
    this.SendMessage<ConnectRequestMessage>(ref iMessage);
    this.mLastLatencyCheck = DateTime.Now.Ticks;
    SteamAPI.P2PSessionRequest += new Action<P2PSessionRequest>(this.SteamAPI_P2PSessionRequest);
    SteamAPI.P2PSessionConnectFail += new Action<P2PSessionConnectFail>(this.SteamAPI_P2PSessionConnectFail);
    this.mErrorMessageBox = new OptionsMessageBox(SubMenuOnline.LOC_ERROR_UNKNOWN, new int[1]
    {
      Defines.LOC_GEN_OK
    });
  }

  public override SteamID ServerID => this.mServerID;

  public override bool IsVACSecure => this.mVAC;

  private void SteamAPI_P2PSessionRequest(P2PSessionRequest iParam)
  {
    SteamNetworking.AcceptP2PSessionWithUser(iParam.mSteamIDRemote);
  }

  private void SteamAPI_P2PSessionConnectFail(P2PSessionConnectFail iParam)
  {
    lock (this.mConnections)
    {
      for (int index = 0; index < this.mConnections.Count; ++index)
      {
        if (this.mConnections[index].ID == iParam.mSteamIDRemote)
        {
          if (index == 0)
          {
            ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_AUTHFAIL));
            this.Dispose();
            return;
          }
          this.mConnections.RemoveAt(index--);
        }
      }
    }
    SteamNetworking.CloseP2PSessionWithUser(iParam.mSteamIDRemote);
  }

  private void OnJoinLobby(LobbyEnter iLobby)
  {
  }

  public override void Sync()
  {
    while (this.mSyncPoints.Count == 0)
      Thread.Sleep(10);
    EnterSyncMessage iMessage;
    lock (this.mSyncPoints)
      iMessage.ID = this.mSyncPoints.Dequeue();
    this.SendMessage<EnterSyncMessage>(ref iMessage, 0);
    while (!this.mFinishedSyncPoints.Contains(iMessage.ID))
      Thread.Sleep(10);
    lock (this.mFinishedSyncPoints)
      this.mFinishedSyncPoints.Remove(iMessage.ID);
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
    if (NetworkManager.Instance.ConnectionStatus == ConnectionStatus.Connecting)
    {
      if (ticks - this.mLastLatencyCheck > 70000000L)
      {
        this.mLastLatencyCheck = ticks;
        ++this.mRetryCount;
        if (this.mRetryCount < 3)
        {
          ConnectRequestMessage iMessage = new ConnectRequestMessage();
          this.SendMessage<ConnectRequestMessage>(ref iMessage);
        }
        else
        {
          NetworkManager.Instance.ConnectionStatus = ConnectionStatus.Failed_Timeout;
          this.mOnComplete(NetworkManager.Instance.ConnectionStatus);
          NetworkManager.Instance.EndSession();
          return;
        }
      }
    }
    else if (ticks - this.mLastLatencyCheck > 10000000L)
    {
      for (int index = 0; index < this.mConnections.Count; ++index)
      {
        NetworkClient.Connection mConnection = this.mConnections[index];
        mConnection.Timer.Reset();
        mConnection.Timer.Start();
        this.SendUdpMessage<PingRequestMessage>(ref new PingRequestMessage()
        {
          Payload = (int) ticks
        }, index);
      }
      this.mLastLatencyCheck = ticks;
    }
    uint pcubMsgSize;
    while (SteamNetworking.IsP2PPacketAvailable(out pcubMsgSize))
    {
      SteamID psteamIDRemote;
      if (SteamNetworking.ReadP2PPacket(this.mRxBuffer, out pcubMsgSize, out psteamIDRemote) && this.GetConnection(psteamIDRemote) >= 0)
      {
        this.mReader.BaseStream.Position = 0L;
        while (this.mReader.BaseStream.Position < (long) pcubMsgSize)
          this.ReadMessage(this.mReader, psteamIDRemote);
      }
    }
  }

  public override void FlushMessageBuffers()
  {
    for (int index = 0; index < this.mConnections.Count; ++index)
    {
      NetworkClient.Connection mConnection = this.mConnections[index];
      uint position = (uint) mConnection.Writer.BaseStream.Position;
      if (position > 0U)
        this.FlushUDPBatch(index, position);
      P2PSessionState pConnectionState;
      SteamNetworking.GetP2PSessionState(mConnection.ID, out pConnectionState);
      if (pConnectionState.ConnectionActive && pConnectionState.PacketsQueuedForSend > 0)
        SteamNetworking.SendP2PPacket(mConnection.ID, this.mDummyData, 0U, P2PSend.Reliable);
    }
  }

  private int GetConnection(SteamID iID)
  {
    for (int index = 0; index < this.mConnections.Count; ++index)
    {
      if (this.mConnections[index].ID == iID)
        return index;
    }
    return -1;
  }

  private unsafe void ReadMessage(BinaryReader iReader, SteamID iSender)
  {
    PacketType packetType = PacketType.ChatMessage;
    try
    {
      packetType = (PacketType) iReader.ReadByte();
      if ((packetType & PacketType.Request) == PacketType.Request)
      {
        packetType &= ~PacketType.Request;
        switch (packetType)
        {
          case PacketType.Ping:
            PingRequestMessage pingRequestMessage = new PingRequestMessage();
            pingRequestMessage.Read(iReader);
            this.SendUdpMessage<PingReplyMessage>(ref new PingReplyMessage()
            {
              Payload = pingRequestMessage.Payload
            });
            break;
          case PacketType.GamerJoin:
            GamerJoinRequestMessage joinRequestMessage = new GamerJoinRequestMessage();
            joinRequestMessage.Read(iReader);
            if (iSender != this.mServerID)
            {
              if (!Debugger.IsAttached)
                break;
              Debugger.Break();
              break;
            }
            Magicka.GameLogic.Player player = Magicka.Game.Instance.Players[(int) joinRequestMessage.Id];
            if (player.Controller != null)
            {
              player.Controller.Player = (Magicka.GameLogic.Player) null;
              player.Controller = (Controller) null;
            }
            player.Gamer = (Gamer) new NetworkGamer(joinRequestMessage.Gamer, joinRequestMessage.Color, joinRequestMessage.AvatarThumb, joinRequestMessage.AvatarType, joinRequestMessage.SteamID);
            player.Playing = true;
            SubMenuCharacterSelect.Instance.UpdateGamer(player, player.Gamer);
            break;
          case PacketType.Damage:
            DamageRequestMessage damageRequestMessage = new DamageRequestMessage();
            damageRequestMessage.Read(iReader);
            Entity fromHandle1 = Entity.GetFromHandle((int) damageRequestMessage.AttackerHandle);
            if (!(Entity.GetFromHandle((int) damageRequestMessage.TargetHandle) is IDamageable fromHandle2))
              break;
            Vector3 result = damageRequestMessage.RelativeAttackPosition;
            Vector3 position = fromHandle2.Position;
            Vector3.Add(ref position, ref result, out result);
            Defines.DamageFeatures iFeatures = Defines.DamageFeatures.NK;
            int num = (int) fromHandle2.InternalDamage(damageRequestMessage.Damage, fromHandle1, damageRequestMessage.TimeStamp, result, iFeatures);
            break;
          default:
            throw new Exception($"Unhandled request ({packetType.ToString()})!");
        }
      }
      else
      {
        switch (packetType)
        {
          case PacketType.Ping:
            PingReplyMessage pingReplyMessage = new PingReplyMessage();
            pingReplyMessage.Read(iReader);
            if (pingReplyMessage.Payload != (int) this.mLastLatencyCheck)
              break;
            int connection1 = this.GetConnection(iSender);
            NetworkClient.Connection mConnection = this.mConnections[connection1];
            mConnection.Timer.Stop();
            mConnection.Latency = (float) mConnection.Timer.Elapsed.TotalSeconds;
            this.mConnections[connection1] = mConnection;
            break;
          case PacketType.Connect:
            ConnectReplyMessage connectReplyMessage = new ConnectReplyMessage();
            connectReplyMessage.Read(iReader);
            this.mVAC = connectReplyMessage.VACSecure;
            if (this.mVAC && HackHelper.LicenseStatus == HackHelper.Status.Hacked)
              break;
            NetworkManager.Instance.ConnectionStatus = ConnectionStatus.Authenticating;
            NetworkManager.Instance.GameName = connectReplyMessage.ServerName;
            string str1 = LanguageManager.Instance.GetString("#add_menu_not_connected".GetHashCodeCustom());
            Vector4 vector4 = new Vector4();
            vector4.X = Defines.DIALOGUE_COLOR_DEFAULT.X / 0.7037f;
            vector4.Y = Defines.DIALOGUE_COLOR_DEFAULT.Y / 0.7037f;
            vector4.Z = Defines.DIALOGUE_COLOR_DEFAULT.Z / 0.7037f;
            vector4.W = Defines.DIALOGUE_COLOR_DEFAULT.W;
            IFormatProvider numberFormat = (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat;
            string str2 = $"[c={vector4.X.ToString(numberFormat)},{vector4.Y.ToString(numberFormat)},{vector4.Z.ToString(numberFormat)},{vector4.W.ToString(numberFormat)}]";
            NetworkChat.Instance.SetTitle(str1.Replace("#1;", $"{str2}{connectReplyMessage.ServerName}[/c]").Replace("#2;", $"{str2}{27016.ToString()}[/c]"));
            P2PSessionState pConnectionState;
            SteamNetworking.GetP2PSessionState(connectReplyMessage.ServerID, out pConnectionState);
            AuthenticateRequestMessage iMessage1 = new AuthenticateRequestMessage()
            {
              NrOfPlayers = (byte) Magicka.Game.Instance.PlayerCount,
              Version = Magicka.Game.Instance.Version,
              Password = NetworkManager.Instance.Password
            };
            iMessage1.Token.Length = SteamUser.InitiateGameConnection((void*) iMessage1.Token.Data, 1024 /*0x0400*/, connectReplyMessage.ServerID, pConnectionState.RemoteIP, pConnectionState.RemotePort, connectReplyMessage.VACSecure);
            this.SendMessage<AuthenticateRequestMessage>(ref iMessage1, 0);
            break;
          case PacketType.Authenticate:
            AuthenticateReplyMessage authenticateReplyMessage = new AuthenticateReplyMessage();
            authenticateReplyMessage.Read(iReader);
            if (iSender != this.mServerID)
              break;
            if (authenticateReplyMessage.Response == AuthenticateReplyMessage.Reply.Ok)
            {
              NetworkManager.Instance.ConnectionStatus = ConnectionStatus.Connected;
              if (authenticateReplyMessage.GameInfo.GameName != null)
              {
                NetworkManager.Instance.GameName = authenticateReplyMessage.GameInfo.GameName;
                SubMenuCharacterSelect.Instance.SetSettings(authenticateReplyMessage.GameInfo.GameType, authenticateReplyMessage.GameInfo.Level, false);
                if (authenticateReplyMessage.GameInfo.GameType == GameType.Versus)
                  SubMenuCharacterSelect.Instance.SetVsSettings(ref authenticateReplyMessage.VersusSettings);
              }
            }
            else
            {
              if (authenticateReplyMessage.Response == AuthenticateReplyMessage.Reply.Error_AuthFailed)
              {
                ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_AUTHFAIL));
                NetworkManager.Instance.ConnectionStatus = ConnectionStatus.Failed_Authentication;
              }
              else if (authenticateReplyMessage.Response == AuthenticateReplyMessage.Reply.Error_ServerFull)
              {
                ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_SERVERFULL));
                NetworkManager.Instance.ConnectionStatus = ConnectionStatus.Failed_GameFull;
              }
              else if (authenticateReplyMessage.Response == AuthenticateReplyMessage.Reply.Error_GamePlaying)
              {
                ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_INPROGRESS));
                NetworkManager.Instance.ConnectionStatus = ConnectionStatus.Failed_GamePlaying;
              }
              else if (authenticateReplyMessage.Response == AuthenticateReplyMessage.Reply.Error_Version)
              {
                ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_VERSION));
                NetworkManager.Instance.ConnectionStatus = ConnectionStatus.Failed_Version;
              }
              else if (authenticateReplyMessage.Response == AuthenticateReplyMessage.Reply.Error_Password)
              {
                ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_PASSWORD));
                NetworkManager.Instance.ConnectionStatus = ConnectionStatus.Failed_Password;
              }
              else
              {
                ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_UNKNOWN));
                NetworkManager.Instance.ConnectionStatus = ConnectionStatus.Failed_Unknown;
              }
              NetworkManager.Instance.EndSession();
            }
            this.mOnComplete(NetworkManager.Instance.ConnectionStatus);
            break;
          case PacketType.LobbyInfo:
            LobbyInfoMessage lobbyInfoMessage = new LobbyInfoMessage();
            lobbyInfoMessage.Read(iReader);
            this.mSteamIDLobby = new SteamID(lobbyInfoMessage.SteamID);
            SteamMatchmaking.JoinLobby(this.mSteamIDLobby, new Action<LobbyEnter>(this.OnJoinLobby));
            break;
          case PacketType.ClientConnected:
            ClientConnectedMessage iMessage2 = new ClientConnectedMessage();
            iMessage2.Read(iReader);
            if (this.GetConnection(iSender) != 0)
              break;
            NetworkClient.Connection connection2;
            connection2.ID = iMessage2.ID;
            connection2.Timer = new Stopwatch();
            connection2.Latency = 0.0f;
            connection2.TxBuffer = new byte[4400];
            connection2.Writer = new BinaryWriter((Stream) new MemoryStream(connection2.TxBuffer));
            this.mConnections.Add(connection2);
            this.SendMessage<ClientConnectedMessage>(ref iMessage2, this.mConnections.Count - 1);
            iMessage2.ID = SteamUser.GetSteamID();
            break;
          case PacketType.ConnectionClosed:
            ConnectionClosedMessage connectionClosedMessage = new ConnectionClosedMessage();
            connectionClosedMessage.Read(iReader);
            if (iSender == this.mServerID)
            {
              if (connectionClosedMessage.Reason == ConnectionClosedMessage.CReason.Kicked)
                ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString("#add_connectionlost".GetHashCodeCustom()));
              else
                ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString("#add_connectionlost".GetHashCodeCustom()));
              NetworkManager.Instance.EndSession();
              break;
            }
            this.CloseConnection(iSender, connectionClosedMessage.Reason);
            break;
          case PacketType.GameInfo:
            GameInfoMessage gameInfoMessage = new GameInfoMessage();
            gameInfoMessage.Read(iReader);
            SubMenuCharacterSelect.Instance.SetSettings(gameInfoMessage.GameType, gameInfoMessage.Level, false);
            break;
          case PacketType.VersusOptions:
            VersusRuleset.Settings.OptionsMessage iVersusSettings = new VersusRuleset.Settings.OptionsMessage();
            iVersusSettings.Read(iReader);
            SubMenuCharacterSelect.Instance.SetVsSettings(ref iVersusSettings);
            break;
          case PacketType.PackOptions:
            new PackOptionsMessage().Read(iReader);
            break;
          case PacketType.ChatMessage:
            ChatMessage chatMessage = new ChatMessage();
            chatMessage.Read(iReader);
            NetworkChat.Instance.AddMessage(chatMessage.ToString());
            break;
          case PacketType.GamerJoin:
            GamerJoinAcceptMessage iMessage3 = new GamerJoinAcceptMessage();
            iMessage3.Read(iReader);
            Magicka.GameLogic.Player.JoinServerGranted(ref iMessage3);
            break;
          case PacketType.GamerChanged:
            GamerChangedMessage gamerChangedMessage = new GamerChangedMessage();
            gamerChangedMessage.Read(iReader);
            Magicka.GameLogic.Player player1 = Magicka.Game.Instance.Players[(int) gamerChangedMessage.Id];
            player1.UnlockedMagicks = gamerChangedMessage.UnlockedMagicks;
            Gamer gamer = player1.Gamer;
            if (gamer == null)
              break;
            gamer.Color = gamerChangedMessage.Color;
            Profile.PlayableAvatar playableAvatar = new Profile.PlayableAvatar();
            playableAvatar.ThumbPath = gamerChangedMessage.AvatarThumb;
            lock (Magicka.Game.Instance.GraphicsDevice)
              playableAvatar.Thumb = Magicka.Game.Instance.Content.Load<Texture2D>(playableAvatar.ThumbPath);
            playableAvatar.PortraitPath = gamerChangedMessage.AvatarPortrait;
            lock (Magicka.Game.Instance.GraphicsDevice)
              playableAvatar.Portrait = Magicka.Game.Instance.Content.Load<Texture2D>(playableAvatar.PortraitPath);
            playableAvatar.TypeName = gamerChangedMessage.AvatarType;
            playableAvatar.Type = gamerChangedMessage.AvatarType.GetHashCodeCustom();
            playableAvatar.Name = player1.Gamer.Avatar.Name;
            playableAvatar.AllowCampaign = gamerChangedMessage.AvatarAllowCampaign;
            playableAvatar.AllowChallenge = gamerChangedMessage.AvatarAllowChallenge;
            playableAvatar.AllowPVP = gamerChangedMessage.AvatarAllowPVP;
            gamer.Avatar = playableAvatar;
            gamer.GamerTag = gamerChangedMessage.GamerTag;
            SubMenuCharacterSelect.Instance.UpdateGamer(player1, gamer);
            break;
          case PacketType.GamerLeave:
            GamerLeaveMessage gamerLeaveMessage = new GamerLeaveMessage();
            gamerLeaveMessage.Read(iReader);
            Magicka.GameLogic.Player player2 = Magicka.Game.Instance.Players[(int) gamerLeaveMessage.Id];
            player2.Playing = false;
            player2.Team = Factions.NONE;
            player2.Gamer = (Gamer) null;
            SubMenuCharacterSelect.Instance.UpdateGamer(player2, (Gamer) null);
            break;
          case PacketType.GamerReady:
            GamerReadyMessage gamerReadyMessage = new GamerReadyMessage();
            gamerReadyMessage.Read(iReader);
            SubMenuCharacterSelect.Instance.SetReady(gamerReadyMessage.Ready, gamerReadyMessage.Id);
            break;
          case PacketType.SaveData:
            this.mSaveSlot.Read(iReader);
            break;
          case PacketType.MenuSelection:
            MenuSelectMessage iMessage4 = new MenuSelectMessage();
            iMessage4.Read(iReader);
            MenuState.Instance.NetworkInput(ref iMessage4);
            break;
          case PacketType.GameEndLoad:
            new GameEndLoadMessage().Read(iReader);
            PlayState.WaitingForPlayers = false;
            break;
          case PacketType.GameEnd:
            GameEndMessage iMsg1 = new GameEndMessage();
            iMsg1.Read(iReader);
            PlayState.RecentPlayState.Endgame(ref iMsg1);
            break;
          case PacketType.GameRestart:
            GameRestartMessage gameRestartMessage = new GameRestartMessage();
            gameRestartMessage.Read(iReader);
            PlayState.RecentPlayState.Restart((object) this, gameRestartMessage.Type);
            break;
          case PacketType.TriggerAction:
            TriggerActionMessage iMsg2 = new TriggerActionMessage();
            iMsg2.Read(iReader);
            Trigger.NetworkAction(ref iMsg2);
            break;
          case PacketType.StatisticsUpdate:
            StatisticsMessage iMsg3 = new StatisticsMessage();
            iMsg3.Read(iReader);
            StatisticsManager.Instance.NetworkUpdate(ref iMsg3);
            break;
          case PacketType.LeaderboardEntry:
            LeaderboardMessage leaderboardMessage = new LeaderboardMessage();
            leaderboardMessage.Read(iReader);
            SteamUserStats.UploadLeaderboardScore(leaderboardMessage.SteamLeaderboard, leaderboardMessage.ScoreMethod, leaderboardMessage.Score, new int[1]
            {
              leaderboardMessage.Data
            }, new Action<LeaderboardScoreUploaded>(StatisticsManager.Instance.OnlineScoreUploaded));
            break;
          case PacketType.RulesetUpdate:
            RulesetMessage iMsg4 = new RulesetMessage();
            iMsg4.Read(iReader);
            PlayState recentPlayState = PlayState.RecentPlayState;
            if (recentPlayState.Level.CurrentScene.RuleSet == null)
              break;
            recentPlayState.Level.CurrentScene.RuleSet.NetworkUpdate(ref iMsg4);
            break;
          case PacketType.DialogAdvance:
            DialogAdvanceMessage dialogAdvanceMessage = new DialogAdvanceMessage();
            dialogAdvanceMessage.Read(iReader);
            DialogManager.Instance.NetworkAdvance(dialogAdvanceMessage.Interact);
            break;
          case PacketType.EntityUpdate:
            EntityUpdateMessage iMsg5 = new EntityUpdateMessage();
            iMsg5.Read(iReader);
            Entity.GetFromHandle((int) iMsg5.Handle)?.NetworkUpdate(iSender, ref iMsg5);
            break;
          case PacketType.CharacterAction:
            CharacterActionMessage iMsg6 = new CharacterActionMessage();
            iMsg6.Read(iReader);
            if (!(Entity.GetFromHandle((int) iMsg6.Handle) is Magicka.GameLogic.Entities.Character fromHandle3))
              break;
            fromHandle3.NetworkAction(ref iMsg6);
            break;
          case PacketType.AnimatedLevelPartUpdate:
            AnimatedLevelPartUpdateMessage iMsg7 = new AnimatedLevelPartUpdateMessage();
            iMsg7.Read(iReader);
            AnimatedLevelPart.GetFromHandle((int) iMsg7.Handle)?.NetworkUpdate(ref iMsg7);
            break;
          case PacketType.BossUpdate:
            BossUpdateMessage iMsg8 = new BossUpdateMessage();
            iMsg8.Read(iReader);
            BossFight.Instance.NetworkUpdate(ref iMsg8);
            break;
          case PacketType.BossInitialize:
            BossInitializeMessage iMsg9 = new BossInitializeMessage();
            iMsg9.Read(iReader);
            BossFight.Instance.NetworkInitialize(ref iMsg9);
            break;
          case PacketType.SpawnPlayer:
            SpawnPlayerMessage spawnPlayerMessage = new SpawnPlayerMessage();
            spawnPlayerMessage.Read(iReader);
            Magicka.GameLogic.Player player3 = Magicka.Game.Instance.Players[(int) spawnPlayerMessage.Id];
            if (player3 == null)
              break;
            lock (player3)
            {
              Avatar fromCache = Avatar.GetFromCache(player3, spawnPlayerMessage.Handle);
              if (fromCache == null)
                break;
              player3.Weapon = (string) null;
              player3.Staff = (string) null;
              fromCache.Initialize(CharacterTemplate.GetCachedTemplate(player3.Gamer.Avatar.Type), spawnPlayerMessage.Position, Magicka.GameLogic.Player.UNIQUE_ID[(int) spawnPlayerMessage.Id]);
              fromCache.SpawnAnimation = Magicka.Animations.revive;
              fromCache.ChangeState((BaseState) RessurectionState.Instance);
              fromCache.CharacterBody.DesiredDirection = spawnPlayerMessage.Direction;
              player3.Avatar = fromCache;
              if (spawnPlayerMessage.MagickRevive && player3.Controller is XInputController)
                (player3.Controller as XInputController).Rumble(2f, 2f);
              fromCache.PlayState.EntityManager.AddEntity((Entity) fromCache);
              AudioManager.Instance.PlayCue(Banks.Spells, Revive.SOUNDHASH, fromCache.AudioEmitter);
              break;
            }
          case PacketType.SpawnMissile:
            SpawnMissileMessage spawnMissileMessage = new SpawnMissileMessage();
            spawnMissileMessage.Read(iReader);
            if (!(Entity.GetFromHandle((int) spawnMissileMessage.Handle) is MissileEntity fromHandle4))
              throw new Exception("Caches out of sync!");
            switch (spawnMissileMessage.Type)
            {
              case SpawnMissileMessage.MissileType.Spell:
                ProjectileSpell.SpawnMissile(ref fromHandle4, Entity.GetFromHandle((int) spawnMissileMessage.Owner) as ISpellCaster, spawnMissileMessage.Homing, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity, ref spawnMissileMessage.Spell, spawnMissileMessage.Splash, (int) spawnMissileMessage.Item);
                return;
              case SpawnMissileMessage.MissileType.Item:
                Item fromHandle5 = Entity.GetFromHandle((int) spawnMissileMessage.Item) as Item;
                Entity fromHandle6 = Entity.GetFromHandle((int) spawnMissileMessage.Owner);
                if (fromHandle5.ProjectileModel != null)
                  fromHandle4.Initialize(fromHandle6, fromHandle5.ProjectileModel.Meshes[0].BoundingSphere.Radius, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity, fromHandle5.ProjectileModel, fromHandle5.RangedConditions, false);
                else
                  fromHandle4.Initialize(fromHandle6, 0.75f, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity, (Model) null, fromHandle5.RangedConditions, false);
                fromHandle4.Danger = fromHandle5.Danger;
                fromHandle4.Homing = fromHandle5.Homing;
                fromHandle4.FacingVelocity = fromHandle5.Facing;
                fromHandle4.PlayState.EntityManager.AddEntity((Entity) fromHandle4);
                return;
              case SpawnMissileMessage.MissileType.HolyGrenade:
                HolyGrenade.SpawnGrenade(ref fromHandle4, Entity.GetFromHandle((int) spawnMissileMessage.Owner) as ISpellCaster, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity);
                fromHandle4.FacingVelocity = false;
                return;
              case SpawnMissileMessage.MissileType.Grenade:
                Grenade.SpawnGrenade(ref fromHandle4, Entity.GetFromHandle((int) spawnMissileMessage.Owner) as ISpellCaster, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity);
                fromHandle4.FacingVelocity = false;
                return;
              case SpawnMissileMessage.MissileType.FireFlask:
                Fireflask.SpawnGrenade(ref fromHandle4, Entity.GetFromHandle((int) spawnMissileMessage.Owner) as ISpellCaster, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity);
                fromHandle4.FacingVelocity = false;
                return;
              case SpawnMissileMessage.MissileType.ProppMagick:
                ProppMagick.Spawn(ref fromHandle4, Entity.GetFromHandle((int) spawnMissileMessage.Owner) as ISpellCaster, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity, ref spawnMissileMessage.AngularVelocity, ref spawnMissileMessage.Lever);
                return;
              case SpawnMissileMessage.MissileType.JudgementMissile:
                Entity fromHandle7 = Entity.GetFromHandle((int) spawnMissileMessage.Target);
                JudgementSpray.SpawnProjectile(ref fromHandle4, Entity.GetFromHandle((int) spawnMissileMessage.Owner) as ISpellCaster, ref fromHandle7, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity);
                fromHandle4.FacingVelocity = false;
                return;
              case SpawnMissileMessage.MissileType.GreaseLump:
                Entity.GetFromHandle((int) spawnMissileMessage.Target);
                GreaseLump.SpawnLump(ref fromHandle4, Entity.GetFromHandle((int) spawnMissileMessage.Owner) as ISpellCaster, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity);
                fromHandle4.FacingVelocity = false;
                return;
              case SpawnMissileMessage.MissileType.PotionFlask:
                Potion.SpawnFlask(ref fromHandle4, Entity.GetFromHandle((int) spawnMissileMessage.Owner) as ISpellCaster, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity);
                fromHandle4.FacingVelocity = false;
                return;
              default:
                throw new Exception();
            }
          case PacketType.SpawnShield:
            SpawnShieldMessage spawnShieldMessage = new SpawnShieldMessage();
            spawnShieldMessage.Read(iReader);
            Shield fromHandle8 = Entity.GetFromHandle((int) spawnShieldMessage.Handle) as Shield;
            ISpellCaster fromHandle9 = Entity.GetFromHandle((int) spawnShieldMessage.OwnerHandle) as ISpellCaster;
            fromHandle8.Initialize(fromHandle9, spawnShieldMessage.Position, spawnShieldMessage.Radius, spawnShieldMessage.Direction, spawnShieldMessage.ShieldType, spawnShieldMessage.HitPoints, Spell.SHIELDCOLOR);
            fromHandle8.PlayState.EntityManager.AddEntity((Entity) fromHandle8);
            break;
          case PacketType.SpawnBarrier:
            SpawnBarrierMessage spawnBarrierMessage = new SpawnBarrierMessage();
            spawnBarrierMessage.Read(iReader);
            Barrier fromHandle10 = Entity.GetFromHandle((int) spawnBarrierMessage.Handle) as Barrier;
            Barrier.HitListWithBarriers byHandle1 = Barrier.HitListWithBarriers.GetByHandle(spawnBarrierMessage.HitlistHandle);
            ISpellCaster fromHandle11 = Entity.GetFromHandle((int) spawnBarrierMessage.OwnerHandle) as ISpellCaster;
            AnimatedLevelPart iAnimation1 = (AnimatedLevelPart) null;
            if (spawnBarrierMessage.AnimationHandle < ushort.MaxValue)
              iAnimation1 = AnimatedLevelPart.GetFromHandle((int) spawnBarrierMessage.AnimationHandle);
            fromHandle10.Initialize(fromHandle11, spawnBarrierMessage.Position, spawnBarrierMessage.Direction, spawnBarrierMessage.Scale, 0.0f, new Vector3(), Quaternion.Identity, 0.0f, ref spawnBarrierMessage.Spell, ref spawnBarrierMessage.Damage, byHandle1, iAnimation1);
            fromHandle10.PlayState.EntityManager.AddEntity((Entity) fromHandle10);
            break;
          case PacketType.SpawnWave:
            SpawnWaveMessage spawnWaveMessage = new SpawnWaveMessage();
            spawnWaveMessage.Read(iReader);
            WaveEntity fromHandle12 = Entity.GetFromHandle((int) spawnWaveMessage.Handle) as WaveEntity;
            Barrier.HitListWithBarriers byHandle2 = Barrier.HitListWithBarriers.GetByHandle(spawnWaveMessage.HitlistHandle);
            ISpellCaster fromHandle13 = Entity.GetFromHandle((int) spawnWaveMessage.OwnerHandle) as ISpellCaster;
            AnimatedLevelPart iAnimation2 = (AnimatedLevelPart) null;
            if (spawnWaveMessage.AnimationHandle < ushort.MaxValue)
              iAnimation2 = AnimatedLevelPart.GetFromHandle((int) spawnWaveMessage.AnimationHandle);
            Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Wave instance = Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Wave.GetInstance();
            fromHandle12.Initialize(fromHandle13, spawnWaveMessage.Position, spawnWaveMessage.Direction, spawnWaveMessage.Scale, 0.0f, new Vector3(), Quaternion.Identity, 0.0f, ref spawnWaveMessage.Spell, ref spawnWaveMessage.Damage, ref byHandle2, iAnimation2, ref instance);
            fromHandle12.PlayState.EntityManager.AddEntity((Entity) fromHandle12);
            break;
          case PacketType.SpawnMine:
            SpawnMineMessage spawnMineMessage = new SpawnMineMessage();
            spawnMineMessage.Read(iReader);
            SpellMine fromHandle14 = Entity.GetFromHandle((int) spawnMineMessage.Handle) as SpellMine;
            ISpellCaster fromHandle15 = Entity.GetFromHandle((int) spawnMineMessage.OwnerHandle) as ISpellCaster;
            AnimatedLevelPart iAnimation3 = (AnimatedLevelPart) null;
            if (spawnMineMessage.AnimationHandle < ushort.MaxValue)
              iAnimation3 = AnimatedLevelPart.GetFromHandle((int) spawnMineMessage.AnimationHandle);
            fromHandle14.Initialize(fromHandle15, spawnMineMessage.Position, spawnMineMessage.Direction, spawnMineMessage.Scale, 0.0f, new Vector3(), Quaternion.Identity, 0.0f, ref spawnMineMessage.Spell, ref spawnMineMessage.Damage, iAnimation3);
            fromHandle14.PlayState.EntityManager.AddEntity((Entity) fromHandle14);
            break;
          case PacketType.SpawnVortex:
            SpawnVortexMessage spawnVortexMessage = new SpawnVortexMessage();
            spawnVortexMessage.Read(iReader);
            VortexEntity specificInstance = VortexEntity.GetSpecificInstance(spawnVortexMessage.Handle);
            Entity fromHandle16 = Entity.GetFromHandle((int) spawnVortexMessage.OwnerHandle);
            specificInstance.Initialize(fromHandle16 as ISpellCaster, spawnVortexMessage.Position);
            specificInstance.PlayState.EntityManager.AddEntity((Entity) specificInstance);
            break;
          case PacketType.SpawnPortal:
            SpawnPortalMessage iMsg10 = new SpawnPortalMessage();
            iMsg10.Read(iReader);
            Portal.Instance.SpawnPortal(ref iMsg10);
            break;
          case PacketType.EntityRemove:
            EntityRemoveMessage entityRemoveMessage = new EntityRemoveMessage();
            entityRemoveMessage.Read(iReader);
            Entity fromHandle17 = Entity.GetFromHandle((int) entityRemoveMessage.Handle);
            if (fromHandle17 is Magicka.GameLogic.Entities.Character)
            {
              (fromHandle17 as Magicka.GameLogic.Entities.Character).Terminate(false, true);
              break;
            }
            if (fromHandle17.Dead)
              break;
            fromHandle17.Kill();
            break;
          case PacketType.CharacterDie:
            CharacterDieMessage characterDieMessage = new CharacterDieMessage();
            characterDieMessage.Read(iReader);
            Magicka.GameLogic.Entities.Character fromHandle18 = Entity.GetFromHandle((int) characterDieMessage.Handle) as Magicka.GameLogic.Entities.Character;
            if (!fromHandle18.NotedKilledEvent)
            {
              StatisticsManager.Instance.AddKillEvent(fromHandle18.PlayState, (Entity) fromHandle18, fromHandle18.LastAttacker);
              fromHandle18.NotedKilledEvent = true;
            }
            if (characterDieMessage.Overkill)
            {
              if (!fromHandle18.mDead)
                fromHandle18.Die();
              fromHandle18.RemoveAfterDeath = true;
              if (fromHandle18.HasGibs())
              {
                fromHandle18.SpawnGibs();
                AudioManager.Instance.PlayCue(Banks.Misc, Magicka.GameLogic.Entities.CharacterStates.DeadState.SOUND_GIB, fromHandle18.AudioEmitter);
                break;
              }
              if (!fromHandle18.BloatKilled)
                break;
              fromHandle18.Terminate(false, true);
              break;
            }
            fromHandle18.Die();
            break;
          case PacketType.MissileEntity:
            MissileEntityEventMessage iMsg11 = new MissileEntityEventMessage();
            iMsg11.Read(iReader);
            (Entity.GetFromHandle((int) iMsg11.Handle) as MissileEntity).NetworkEventMessage(ref iMsg11);
            break;
          case PacketType.Threat:
            ThreatMessage threatMessage = new ThreatMessage();
            threatMessage.Read(iReader);
            AudioManager.Instance.Threat = threatMessage.Threat;
            break;
          case PacketType.EnterSync:
            EnterSyncMessage enterSyncMessage = new EnterSyncMessage();
            enterSyncMessage.Read(iReader);
            lock (this.mSyncPoints)
            {
              this.mSyncPoints.Enqueue(enterSyncMessage.ID);
              break;
            }
          case PacketType.LeaveSync:
            LeaveSyncMessage leaveSyncMessage = new LeaveSyncMessage();
            leaveSyncMessage.Read(iReader);
            lock (this.mFinishedSyncPoints)
            {
              this.mFinishedSyncPoints.Add(leaveSyncMessage.ID);
              break;
            }
          case PacketType.Checkpoint:
            int count = iReader.ReadInt32();
            byte[] buffer = new byte[count];
            iReader.Read(buffer, 0, count);
            PlayState.RecentPlayState.CheckpointStream = new MemoryStream(buffer);
            break;
          case PacketType.ForceSyncPlayersMessage:
            ForceSyncPlayerStatusesMessage playerStatusesMessage = new ForceSyncPlayerStatusesMessage();
            playerStatusesMessage.Read(iReader);
            for (int index = 0; index < (int) playerStatusesMessage.numPlayers; ++index)
            {
              EntityUpdateMessage playerUpdateMessage = playerStatusesMessage.playerUpdateMessages[index];
              int handle = (int) playerUpdateMessage.Handle;
              Magicka.GameLogic.Player player4 = (Magicka.GameLogic.Player) null;
              foreach (Magicka.GameLogic.Player player5 in Magicka.Game.Instance.Players)
              {
                player4 = player5;
                if (player4 != null && player4.ID == handle)
                  break;
              }
              player4?.Avatar.ForcedNetworkUpdate(iSender, ref playerUpdateMessage);
            }
            break;
          default:
            throw new Exception($"Unhandled message type ({packetType.ToString()})!");
        }
      }
    }
    catch (NullReferenceException ex)
    {
      string str = SteamFriends.GetFriendPersonaName(iSender);
      if (string.IsNullOrEmpty(str))
        str = "( server ? )";
      throw new Exception($"System.NullReferenceException when reading message of type: {packetType.ToString()} from {str} !", (Exception) ex);
    }
  }

  public override void SendMessage<T>(ref T iMessage, P2PSend sendType)
  {
    lock (this.mTxBuffer)
    {
      this.mTxBuffer[0] = (byte) iMessage.PacketType;
      this.mWriter.BaseStream.Position = 1L;
      iMessage.Write(this.mWriter);
      for (int index = 0; index < this.mConnections.Count; ++index)
      {
        if (!SteamNetworking.SendP2PPacket(this.mConnections[index].ID, this.mTxBuffer, (uint) this.mWriter.BaseStream.Position, sendType))
          ;
        long ticks = DateTime.Now.Ticks;
        int num;
        if (!this.mBytesSent.TryGetValue(ticks, out num))
          num = 0;
        this.mBytesSent[ticks] = num + (int) this.mWriter.BaseStream.Position;
      }
    }
  }

  public override void SendMessage<T>(ref T iMessage, int iPeer, P2PSend sendType)
  {
    this.SendMessage<T>(ref iMessage, this.mConnections[iPeer].ID, sendType);
  }

  public override void SendMessage<T>(ref T iMessage, SteamID iPeer, P2PSend sendType)
  {
    lock (this.mTxBuffer)
    {
      this.mTxBuffer[0] = (byte) iMessage.PacketType;
      this.mWriter.BaseStream.Position = 1L;
      iMessage.Write(this.mWriter);
      if (!SteamNetworking.SendP2PPacket(iPeer, this.mTxBuffer, (uint) this.mWriter.BaseStream.Position, sendType))
      {
        int num1 = iPeer == this.mServerID ? 1 : 0;
      }
      long ticks = DateTime.Now.Ticks;
      int num2;
      if (!this.mBytesSent.TryGetValue(ticks, out num2))
        num2 = 0;
      this.mBytesSent[ticks] = num2 + (int) this.mWriter.BaseStream.Position;
    }
  }

  public override void SendUdpMessage<T>(ref T iMessage)
  {
    lock (this.mTxBuffer)
    {
      for (int iClient = 0; iClient < this.mConnections.Count; ++iClient)
        this.QueueUDPMessage<T>(iClient, ref iMessage);
    }
  }

  public override void SendUdpMessage<T>(ref T iMessage, int iPeer)
  {
    this.QueueUDPMessage<T>(iPeer, ref iMessage);
  }

  public override void SendUdpMessage<T>(ref T iMessage, SteamID iPeer)
  {
    int connection = this.GetConnection(iPeer);
    if (connection >= 0)
    {
      this.QueueUDPMessage<T>(connection, ref iMessage);
    }
    else
    {
      this.mTxBuffer[0] = (byte) iMessage.PacketType;
      this.mWriter.BaseStream.Position = 1L;
      iMessage.Write(this.mWriter);
      if (!SteamNetworking.SendP2PPacket(iPeer, this.mTxBuffer, (uint) this.mWriter.BaseStream.Position, P2PSend.UnreliableNoDelay))
      {
        int num1 = iPeer == this.mServerID ? 1 : 0;
      }
      long ticks = DateTime.Now.Ticks;
      int num2;
      if (!this.mBytesSent.TryGetValue(ticks, out num2))
        num2 = 0;
      this.mBytesSent[ticks] = num2 + (int) this.mWriter.BaseStream.Position;
    }
  }

  public override unsafe void SendRaw(PacketType iType, void* iPtr, int iLength)
  {
    throw new NotImplementedException();
  }

  public override unsafe void SendRaw(PacketType iType, void* iPtr, int iLength, int iPeer)
  {
    throw new NotImplementedException();
  }

  public override unsafe void SendRaw(PacketType iType, void* iPtr, int iLength, SteamID iPeer)
  {
    throw new NotImplementedException();
  }

  public override void Dispose()
  {
    Magicka.GameLogic.Player[] players = Magicka.Game.Instance.Players;
    for (int index = 0; index < players.Length; ++index)
    {
      if (players[index].Playing && !(players[index].Gamer is NetworkGamer))
        players[index].Leave();
    }
    for (int index = 0; index < this.mConnections.Count; ++index)
      SteamNetworking.CloseP2PSessionWithUser(this.mConnections[index].ID);
    uint punGameServerIP = 0;
    ushort punGameServerPort = 0;
    SteamID psteamIDGameServer = new SteamID();
    if (SteamMatchmaking.GetLobbyGameServer(this.mSteamIDLobby, ref punGameServerIP, ref punGameServerPort, ref psteamIDGameServer) && punGameServerIP != 0U && punGameServerPort != (ushort) 0)
      SteamUser.TerminateGameConnection(punGameServerIP, punGameServerPort);
    SteamMatchmaking.LeaveLobby(this.mSteamIDLobby);
    SteamAPI.P2PSessionRequest -= new Action<P2PSessionRequest>(this.SteamAPI_P2PSessionRequest);
    SteamAPI.P2PSessionConnectFail -= new Action<P2PSessionConnectFail>(this.SteamAPI_P2PSessionConnectFail);
    LanguageManager.Instance.LanguageChanged -= new Action(((MessageBox) this.mErrorMessageBox).LanguageChanged);
    if (GameStateManager.Instance.CurrentState is PlayState)
    {
      RenderManager.Instance.TransitionEnd += new TransitionEnd(this.TransitionFinish);
      RenderManager.Instance.BeginTransition(Transitions.Fade, Color.Black, 0.5f);
    }
    Magicka.Game.Instance.AddLoadTask(new Action(PackMan.Instance.UpdatePackLicense));
  }

  private void TransitionFinish(TransitionEffect iOldEffect)
  {
    RenderManager.Instance.TransitionEnd -= new TransitionEnd(this.TransitionFinish);
    while (!(Tome.Instance.CurrentMenu is SubMenuOnline) && !(Tome.Instance.CurrentMenu is SubMenuMain))
      Tome.Instance.PopMenuInstant();
    if (GameStateManager.Instance.CurrentState is PlayState)
      GameStateManager.Instance.PopState();
    RenderManager.Instance.EndTransition(Transitions.Fade, Color.Black, 0.5f);
  }

  public override string ToString() => SteamFriends.GetFriendPersonaName(this.mServerID);

  public SaveSlotInfo SaveSlot => this.mSaveSlot;

  public void ClearSaveSlot() => this.mSaveSlot = new SaveSlotInfo();

  public override int Connections => this.mConnections.Count;

  public override float GetLatency(int iConnection) => this.mConnections[iConnection].Latency;

  public override int GetLatencyMS(int iConnection)
  {
    return (int) ((double) this.mConnections[iConnection].Latency * 1000.0);
  }

  public override float GetLatency(SteamID iConnection)
  {
    for (int index = 0; index < this.mConnections.Count; ++index)
    {
      NetworkClient.Connection mConnection = this.mConnections[index];
      if (mConnection.ID == iConnection)
        return mConnection.Latency;
    }
    return 0.0f;
  }

  public override int GetLatencyMS(SteamID iConnection)
  {
    for (int index = 0; index < this.mConnections.Count; ++index)
    {
      NetworkClient.Connection mConnection = this.mConnections[index];
      if (mConnection.ID == iConnection)
        return (int) ((double) mConnection.Latency * 1000.0);
    }
    return 0;
  }

  private void CloseConnection(SteamID iID, ConnectionClosedMessage.CReason iReason)
  {
    switch (iReason)
    {
      case ConnectionClosedMessage.CReason.Kicked:
        NetworkChat.Instance.AddMessage($"{SteamFriends.GetFriendPersonaName(iID)} {LanguageManager.Instance.GetString("#add_menu_not_kicked".GetHashCodeCustom())}");
        break;
      case ConnectionClosedMessage.CReason.Left:
        NetworkChat.Instance.AddMessage($"{SteamFriends.GetFriendPersonaName(iID)} {LanguageManager.Instance.GetString("#add_menu_not_left".GetHashCodeCustom())}");
        break;
      default:
        NetworkChat.Instance.AddMessage(LanguageManager.Instance.GetString("#add_connectiontolost".GetHashCodeCustom()).Replace("#1;", SteamFriends.GetFriendPersonaName(iID)));
        break;
    }
    lock (this.mConnections)
    {
      for (int index1 = 0; index1 < this.mConnections.Count; ++index1)
      {
        if (this.mConnections[index1].ID == iID)
        {
          List<NetworkClient.Connection> mConnections = this.mConnections;
          int index2 = index1;
          int num = index2 - 1;
          mConnections.RemoveAt(index2);
          break;
        }
      }
    }
    SteamNetworking.CloseP2PSessionWithUser(iID);
  }

  public override void CloseConnection()
  {
    SteamMatchmaking.LeaveLobby(this.mSteamIDLobby);
    this.SendMessage<ConnectionClosedMessage>(ref new ConnectionClosedMessage()
    {
      Reason = ConnectionClosedMessage.CReason.Left
    });
    Thread.Sleep(250);
  }

  private void QueueUDPMessage<T>(int iClient, ref T iMessage) where T : ISendable
  {
    lock (this.mConnections)
    {
      NetworkClient.Connection mConnection = this.mConnections[iClient];
      long position = mConnection.Writer.BaseStream.Position;
      mConnection.Writer.Write((byte) iMessage.PacketType);
      iMessage.Write(mConnection.Writer);
      if (mConnection.Writer.BaseStream.Position <= 1100L)
        return;
      this.FlushUDPBatch(iClient, (uint) position);
      mConnection.Writer.Write((byte) iMessage.PacketType);
      iMessage.Write(mConnection.Writer);
    }
  }

  private void FlushUDPBatch(int iClient, uint iLength)
  {
    lock (this.mConnections)
    {
      NetworkClient.Connection mConnection = this.mConnections[iClient];
      SteamNetworking.SendP2PPacket(mConnection.ID, mConnection.TxBuffer, iLength, P2PSend.UnreliableNoDelay);
      long ticks = DateTime.Now.Ticks;
      int num;
      if (!this.mBytesSent.TryGetValue(ticks, out num))
        num = 0;
      this.mBytesSent[ticks] = num + (int) iLength;
      mConnection.Writer.BaseStream.Position = 0L;
    }
  }

  private struct Connection
  {
    public SteamID ID;
    public byte[] TxBuffer;
    public BinaryWriter Writer;
    public Stopwatch Timer;
    public float Latency;
  }
}
