// Decompiled with JetBrains decompiler
// Type: Magicka.AI.AIManager
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.AI.Messaging;
using Magicka.Audio;
using Magicka.Network;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.AI;

public class AIManager
{
  private const int STEPS = 20;
  private static AIManager mSingelton;
  private static volatile object mSingeltonLock = new object();
  private List<Agent> mAgents = new List<Agent>(256 /*0x0100*/);
  private int mUpdateIndex;

  public static AIManager Instance
  {
    get
    {
      if (AIManager.mSingelton == null)
      {
        lock (AIManager.mSingeltonLock)
        {
          if (AIManager.mSingelton == null)
            AIManager.mSingelton = new AIManager();
        }
      }
      return AIManager.mSingelton;
    }
  }

  private AIManager()
  {
  }

  public void Clear() => this.mAgents.Clear();

  public void Update(float iDeltaTime)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    MessageDispatcher.Instance.DischargeDelayedMessages(iDeltaTime);
    bool flag = false;
    if (this.mAgents.Count > 0)
    {
      this.mUpdateIndex = (this.mUpdateIndex + 20) % this.mAgents.Count;
      int num = Math.Min(20, this.mAgents.Count);
      for (int index = 0; index < num; ++index)
        this.mAgents[(index + this.mUpdateIndex) % this.mAgents.Count].Update();
      for (int index = 0; index < this.Agents.Count; ++index)
      {
        Agent mAgent = this.mAgents[index];
        mAgent.UpdateTime(iDeltaTime);
        flag |= mAgent.CurrentTarget != null;
      }
    }
    AudioManager.Instance.Threat = flag;
  }

  public List<Agent> Agents => this.mAgents;
}
