// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Wave
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using System;
using System.Collections.Generic;
using System.Xml;

#nullable disable
namespace Magicka.Levels;

public class Wave
{
  private bool mWaveStarted;
  private bool mExecuting;
  private float mDelay;
  private GameScene mGameScene;
  private List<WaveActions> mWaveActions;
  private List<WeakReference> mCharacters;
  private float mTotalHitPoints;
  private float mPreReadHitPoints;

  public Wave(GameScene iGameScene)
  {
    this.mGameScene = iGameScene;
    this.mCharacters = new List<WeakReference>();
  }

  public void Initialize(SurvivalRuleset iRules)
  {
    this.mPreReadHitPoints = 0.0f;
    this.mTotalHitPoints = 0.0f;
    this.mExecuting = false;
    this.mCharacters.Clear();
    this.mWaveStarted = false;
    for (int index = 0; index < this.mWaveActions.Count; ++index)
      this.mWaveActions[index].Initialize(iRules);
  }

  public void Update(float iDeltaTime, SurvivalRuleset iRules)
  {
    if ((double) this.mDelay <= 0.0)
    {
      this.mWaveStarted = true;
      this.mExecuting = false;
      for (int index = 0; index < this.mWaveActions.Count; ++index)
        this.mWaveActions[index].Update(iDeltaTime, this.mGameScene, iRules, ref this.mExecuting);
    }
    else
      this.mExecuting = true;
  }

  internal void TrackCharacter(NonPlayerCharacter iChar, bool iItemEvent)
  {
    this.mCharacters.Add(new WeakReference((object) iChar));
    if (!iItemEvent)
      this.mPreReadHitPoints -= iChar.HitPoints;
    else
      this.mTotalHitPoints += iChar.HitPoints;
  }

  public float PreReadHitPoints
  {
    get => this.mPreReadHitPoints;
    set => this.mPreReadHitPoints = value;
  }

  public float HitPointPercentage()
  {
    float num = 0.0f;
    for (int index = 0; index < this.mCharacters.Count; ++index)
    {
      if (this.mCharacters[index].Target != null && (this.mCharacters[index].Target as Character).Faction == Factions.EVIL)
        num += Math.Max(0.0f, (this.mCharacters[index].Target as NonPlayerCharacter).HitPoints);
    }
    return (num + this.mPreReadHitPoints) / this.mTotalHitPoints;
  }

  public float TotalHitPoints
  {
    get => this.mTotalHitPoints;
    set => this.mTotalHitPoints = value;
  }

  public bool HasStarted() => this.mWaveStarted;

  public void StartWave(float iDelay) => this.mDelay = iDelay;

  public bool IsDone() => !this.mExecuting;

  public void Read(XmlNode iNode)
  {
    this.mWaveActions = new List<WaveActions>();
    for (int i = 0; i < iNode.ChildNodes.Count; ++i)
    {
      XmlNode childNode = iNode.ChildNodes[i];
      WaveActions waveActions = new WaveActions(this.mGameScene, this);
      if (!(childNode is XmlComment))
      {
        waveActions.Read(this.mGameScene, childNode);
        this.mWaveActions.Add(waveActions);
      }
    }
  }
}
