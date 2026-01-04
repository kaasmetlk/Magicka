// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Bosses.BossFight
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.UI;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Bosses;

public class BossFight
{
  private static BossFight mSingelton;
  private static volatile object mSingeltonLock = new object();
  private PlayState mPlayState;
  private List<IBoss> mBosses = new List<IBoss>();
  private float mNormalizedHealth;
  private float mHealthAppearDelay;
  private float mFreezeTime;
  private BossHealthBar mHealthbar;
  private bool mGotPlayState;
  private bool mDead;
  private bool mRunning;
  private bool mBossNameSet;
  private TitleRenderer mTitleRenderer;
  private List<BossUpdateMessage> mNetworkMessageQueue = new List<BossUpdateMessage>(4);
  private List<BossInitializeMessage> mNetworkInitializeMessageQueue = new List<BossInitializeMessage>(4);
  private List<KeyValuePair<BossEnum, int>> mInitializeParamQueue = new List<KeyValuePair<BossEnum, int>>(4);
  private float mHealthBarWidth;

  public static BossFight Instance
  {
    get
    {
      if (BossFight.mSingelton == null)
      {
        lock (BossFight.mSingeltonLock)
        {
          if (BossFight.mSingelton == null)
            BossFight.mSingelton = new BossFight();
        }
      }
      return BossFight.mSingelton;
    }
  }

  private BossFight()
  {
    this.mHealthbar = new BossHealthBar((Scene) null);
    this.mTitleRenderer = new TitleRenderer();
    this.mBossNameSet = false;
  }

  public void PassMessage(BossMessages iMessage)
  {
    for (int index = 0; index < this.mBosses.Count; ++index)
      this.mBosses[index].ScriptMessage(iMessage);
  }

  public void SetTitles(
    string iBossTitle,
    string iBossSubTitle,
    float iDisplayTime,
    float iFadeIn,
    float iFadeOut)
  {
    this.mTitleRenderer.SetTitles(iBossTitle, iBossSubTitle, iDisplayTime, iFadeIn, iFadeOut);
    this.mTitleRenderer.Start();
    this.mBossNameSet = true;
  }

  public void Setup(
    PlayState iPlayState,
    float iFreezeTime,
    float iHealthAppearDelay,
    float iHealthbarWidth)
  {
    this.mTitleRenderer.ResetTimer();
    this.mHealthbar.Reset();
    this.mHealthbar.Scene = iPlayState.Scene;
    this.mPlayState = iPlayState;
    this.mHealthAppearDelay = 0.0f;
    this.mHealthBarWidth = iHealthbarWidth;
    this.mHealthbar.SetWidth(iHealthbarWidth);
    this.mFreezeTime = iFreezeTime;
    this.mGotPlayState = true;
  }

  public void Clear()
  {
    this.mBosses.Clear();
    this.mGotPlayState = false;
    this.mDead = false;
    this.mBossNameSet = false;
    this.mTitleRenderer.ResetTimer();
    this.mNetworkMessageQueue.Clear();
    this.mNetworkInitializeMessageQueue.Clear();
  }

  public void Initialize(IBoss iBoss, int iAreaHash, int iUniqueID)
  {
    for (int index = 0; index < this.mNetworkInitializeMessageQueue.Count; ++index)
    {
      BossInitializeMessage initializeMessage = this.mNetworkInitializeMessageQueue[index];
      if (iBoss.GetBossType() == initializeMessage.BossID)
      {
        iBoss.NetworkInitialize(ref initializeMessage);
        this.mNetworkInitializeMessageQueue.RemoveAt(index--);
      }
    }
    Matrix oLocator;
    this.mPlayState.Level.CurrentScene.GetLocator(iAreaHash, out oLocator);
    iBoss.Initialize(ref oLocator, iUniqueID);
    if (!this.mBosses.Contains(iBoss))
      this.mBosses.Add(iBoss);
    this.mPlayState.BossFight = this;
    this.mDead = false;
    float num1 = 0.0f;
    float num2 = 0.0f;
    foreach (IBoss mBoss in this.mBosses)
    {
      num1 += mBoss.HitPoints;
      num2 += mBoss.MaxHitPoints;
    }
    this.mNormalizedHealth = MathHelper.Clamp(num1 / num2, 0.0f, 1f);
    this.mRunning = false;
    this.mHealthAppearDelay = 0.0f;
    this.mHealthbar.Destroy = false;
  }

  public void Start() => this.mRunning = true;

  public void Reset()
  {
    if (this.mPlayState != null)
    {
      this.mBosses.Clear();
      this.mPlayState.BossFight = (BossFight) null;
      this.mNormalizedHealth = 0.0f;
      this.mDead = false;
      this.mHealthbar.Reset();
      this.mBossNameSet = false;
      this.mTitleRenderer.ResetTimer();
    }
    this.mNetworkMessageQueue.Clear();
    this.mNetworkInitializeMessageQueue.Clear();
  }

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if (this.mBossNameSet)
    {
      TitleRenderData iObject = this.mTitleRenderer.Update((int) iDataChannel, iDeltaTime);
      if (iObject == null)
        this.mBossNameSet = false;
      else
        this.mPlayState.Scene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) iObject);
    }
    if (this.mRunning)
    {
      float num1 = 0.0f;
      float num2 = 0.0f;
      bool flag = true;
      foreach (IBoss mBoss in this.mBosses)
      {
        if (!mBoss.Dead)
          flag = false;
        num1 += Math.Max(mBoss.HitPoints, 0.0f);
        num2 += mBoss.MaxHitPoints;
      }
      this.mDead = flag && this.mBosses.Count > 0;
      this.mNormalizedHealth = MathHelper.Clamp(num1 / num2, 0.0f, 1f);
      if (!this.mHealthbar.Destroy && (double) num1 <= 0.0)
        this.mHealthbar.Destroy = true;
      this.mHealthbar.SetNormalizedHealth(this.mNormalizedHealth);
      this.mHealthAppearDelay -= iDeltaTime;
      if ((double) this.mHealthAppearDelay <= 0.0)
        this.mHealthbar.Update(iDataChannel, iDeltaTime);
    }
    else
    {
      this.mNormalizedHealth = 0.0001f;
      this.mHealthbar.SetNormalizedHealth(this.mNormalizedHealth);
    }
    for (int index1 = 0; index1 < this.mNetworkMessageQueue.Count; ++index1)
    {
      BossUpdateMessage mNetworkMessage = this.mNetworkMessageQueue[index1];
      for (int index2 = 0; index2 < this.mBosses.Count; ++index2)
      {
        if (this.mBosses[index2].GetBossType() == mNetworkMessage.BossID)
        {
          this.mNetworkMessageQueue.RemoveAt(index1--);
          this.mBosses[index2].NetworkUpdate(ref mNetworkMessage);
          break;
        }
      }
    }
    foreach (IBoss mBoss in this.mBosses)
      mBoss.UpdateBoss(iDataChannel, iDeltaTime, this.mRunning);
  }

  public bool IsSetup => this.mGotPlayState;

  public bool IsRunning => this.mBosses.Count > 0 && this.mRunning;

  public bool Dead => this.mDead;

  public float NormalizedHealth => this.mNormalizedHealth;

  internal void NetworkInitialize(ref BossInitializeMessage iMsg)
  {
    bool flag = false;
    for (int index = 0; index < this.mBosses.Count; ++index)
    {
      if (this.mBosses[index].GetBossType() == iMsg.BossID)
      {
        this.mBosses[index].NetworkInitialize(ref iMsg);
        flag = true;
        break;
      }
    }
    if (flag)
      return;
    this.mNetworkInitializeMessageQueue.Add(iMsg);
  }

  internal unsafe void SendInitializeMessage<T>(IBoss iSender, ushort iType, void* iMessage) where T : struct
  {
    BossInitializeMessage oMsg;
    BossInitializeMessage.ConvertFrom<T>(iType, iMessage, out oMsg);
    oMsg.BossID = iSender.GetBossType();
    NetworkManager.Instance.Interface.SendMessage<BossInitializeMessage>(ref oMsg);
  }

  internal unsafe void SendInitializeMessage<T>(
    IBoss iSender,
    ushort iType,
    void* iMessage,
    int iClientIndex)
    where T : struct
  {
    if (!(NetworkManager.Instance.Interface is NetworkServer networkServer))
      throw new Exception("This overload may only be called if this is the game server!");
    BossInitializeMessage oMsg;
    BossInitializeMessage.ConvertFrom<T>(iType, iMessage, out oMsg);
    oMsg.BossID = iSender.GetBossType();
    networkServer.SendMessage<BossInitializeMessage>(ref oMsg, iClientIndex);
  }

  internal void NetworkUpdate(ref BossUpdateMessage iMsg)
  {
    bool flag = false;
    for (int index = 0; index < this.mBosses.Count; ++index)
    {
      if (this.mBosses[index].GetBossType() == iMsg.BossID)
      {
        this.mBosses[index].NetworkUpdate(ref iMsg);
        flag = true;
      }
    }
    if (flag)
      return;
    this.mNetworkMessageQueue.Add(iMsg);
  }

  internal unsafe void SendMessage<T>(IBoss iSender, ushort iType, void* iMessage, bool iReliable) where T : struct
  {
    BossUpdateMessage oMsg;
    BossUpdateMessage.ConvertFrom<T>(iType, iMessage, out oMsg);
    oMsg.BossID = iSender.GetBossType();
    if (iReliable)
      NetworkManager.Instance.Interface.SendMessage<BossUpdateMessage>(ref oMsg);
    else
      NetworkManager.Instance.Interface.SendUdpMessage<BossUpdateMessage>(ref oMsg);
  }

  internal unsafe void SendMessage<T>(
    IBoss iSender,
    ushort iType,
    void* iMessage,
    bool iReliable,
    int iClientIndex)
    where T : struct
  {
    if (!(NetworkManager.Instance.Interface is NetworkServer networkServer))
      throw new Exception("This overload may only be called if this is the game server!");
    BossUpdateMessage oMsg;
    BossUpdateMessage.ConvertFrom<T>(iType, iMessage, out oMsg);
    oMsg.BossID = iSender.GetBossType();
    if (iReliable)
      networkServer.SendMessage<BossUpdateMessage>(ref oMsg, iClientIndex);
    else
      networkServer.SendUdpMessage<BossUpdateMessage>(ref oMsg, iClientIndex);
  }

  public void UpdateResolution() => this.mHealthbar.SetWidth(this.mHealthBarWidth);
}
