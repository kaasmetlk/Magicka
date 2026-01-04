// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.WaveActions
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using Magicka.Levels.Triggers;
using Magicka.Levels.Triggers.Actions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Xml;

#nullable disable
namespace Magicka.Levels;

public class WaveActions
{
  private List<Magicka.Levels.Triggers.Actions.Action> mActions;
  private bool mHasExecuted;
  private float mDelay;
  private float mInitalDelay;
  private string mArea;
  private float mSpawnedHitPoins;
  private GameScene mGameScene;
  private Wave mWave;

  public WaveActions(GameScene iGameScene, Wave iWave)
  {
    this.mWave = iWave;
    this.mGameScene = iGameScene;
  }

  public void Initialize(SurvivalRuleset iRules)
  {
    this.mSpawnedHitPoins = 0.0f;
    this.mHasExecuted = false;
    this.mSpawnedHitPoins = 0.0f;
    this.mDelay = this.mInitalDelay;
    if (this.mArea == null || this.mArea.Equals("any", StringComparison.OrdinalIgnoreCase))
      this.mArea = iRules.GetAreas()[MagickaMath.Random.Next(0, iRules.GetAreas().Count)];
    for (int index1 = 0; index1 < this.mActions.Count; ++index1)
    {
      this.mActions[index1].Initialize();
      PropertyInfo[] properties = this.mActions[index1].GetType().GetProperties();
      for (int index2 = 0; index2 < properties.Length; ++index2)
      {
        if (properties[index2].Name.Equals("Area"))
        {
          properties[index2].SetValue((object) this.mActions[index1], (object) this.mArea, (object[]) null);
          break;
        }
      }
      if (this.mActions[index1] is Spawn)
        this.mSpawnedHitPoins += (this.mActions[index1] as Spawn).GetTotalHitPoins();
      if (this.mActions[index1] is SpawnDispenser)
        this.mSpawnedHitPoins += (this.mActions[index1] as SpawnDispenser).GetTotalHitPoins();
    }
    this.mWave.PreReadHitPoints += this.mSpawnedHitPoins;
    this.mWave.TotalHitPoints += this.mSpawnedHitPoins;
  }

  public void Update(
    float iDeltaTime,
    GameScene iScene,
    SurvivalRuleset iRules,
    ref bool oExecuting)
  {
    this.mDelay -= iDeltaTime;
    if ((double) this.mDelay <= 0.0 && !this.mHasExecuted)
    {
      for (int index = 0; index < this.mActions.Count; ++index)
        this.mActions[index].OnTrigger((Character) null);
      this.mHasExecuted = true;
    }
    for (int index = 0; index < this.mActions.Count; ++index)
    {
      this.mActions[index].Update(iDeltaTime);
      if ((double) this.mDelay > 0.0 || !this.mActions[index].HasFinishedExecuting())
        oExecuting = true;
    }
  }

  public void Read(GameScene iGameScene, XmlNode iNode)
  {
    foreach (XmlAttribute attribute in (XmlNamedNodeMap) iNode.Attributes)
    {
      if (attribute.Name.Equals("area", StringComparison.OrdinalIgnoreCase))
      {
        if (!attribute.Value.Equals("any", StringComparison.OrdinalIgnoreCase))
          this.mArea = attribute.Value;
      }
      else if (attribute.Name.Equals("delay", StringComparison.OrdinalIgnoreCase))
        this.mDelay = this.mInitalDelay = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
    }
    Magicka.Levels.Triggers.Actions.Action[][] actionArray = Trigger.ReadActions(iGameScene, (Trigger) null, iNode);
    this.mActions = actionArray.Length <= 1 ? new List<Magicka.Levels.Triggers.Actions.Action>(actionArray[0].Length) : throw new Exception("Can't use RANDOM in survivalmode, you're making the leaderboards unbalanced!");
    for (int index = 0; index < actionArray[0].Length; ++index)
      this.mActions.Add(actionArray[0][index]);
  }
}
