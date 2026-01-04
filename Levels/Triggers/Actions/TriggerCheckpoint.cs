// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.TriggerCheckpoint
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Localization;
using Magicka.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Xml;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class TriggerCheckpoint : Action
{
  public readonly int CHECKPOINT = "#add_checkpoint".GetHashCodeCustom();
  private bool mSaveToDisk = true;
  private string mSpawnPoint;
  private int[] mSpawnPoints = new int[4];
  private Matrix[] mSpawnPointMatrices = new Matrix[4];
  private List<int> mIgnoreList = new List<int>();
  private TextBox mTextBox;
  private bool mSpawnFairy;

  public TriggerCheckpoint(Trigger iTrigger, GameScene iScene, XmlNode iNode)
    : base(iTrigger, iScene)
  {
    this.mSpawnFairy = true;
    for (int i1 = 0; i1 < iNode.ChildNodes.Count; ++i1)
    {
      XmlNode childNode = iNode.ChildNodes[i1];
      if (!(childNode is XmlComment))
      {
        if (!childNode.Name.Equals("ignore", StringComparison.OrdinalIgnoreCase))
          throw new Exception($"Invalid node \"{childNode.Name}\" in \"TriggerCheckpoint\"!");
        for (int i2 = 0; i2 < childNode.Attributes.Count; ++i2)
        {
          XmlAttribute attribute = childNode.Attributes[i2];
          if (attribute.Name.Equals("trigger", StringComparison.OrdinalIgnoreCase))
            this.mIgnoreList.Add(attribute.Value.ToLowerInvariant().GetHashCodeCustom());
        }
      }
    }
    this.mTextBox = new TextBox();
  }

  public override void Initialize()
  {
    base.Initialize();
    for (int index = 0; index < 4; ++index)
      this.GameScene.GetLocator(this.mSpawnPoints[index], out this.mSpawnPointMatrices[index]);
  }

  protected override void Execute()
  {
    this.mTextBox.Initialize(this.GameScene.Scene, MagickaFont.Maiandra18, LanguageManager.Instance.GetString(this.CHECKPOINT), new Vector2(), new Vector2(0.0f, 1f), true, 0, 2f);
    DialogManager.Instance.AddTextBox(this.mTextBox);
    Player[] players = Magicka.Game.Instance.Players;
    if (this.SpawnFairy && Magicka.Game.Instance.PlayerCount == 1 && this.GameScene.PlayState.GameType != GameType.Versus)
    {
      for (int index = 0; index < players.Length; ++index)
      {
        Player player = players[index];
        if (player != null && player.Playing)
        {
          player.Avatar.RevivalFairy.Initialize(this.GameScene.PlayState, true);
          break;
        }
      }
    }
    this.GameScene.PlayState.UpdateCheckPoint(this.mSpawnPointMatrices, this.mIgnoreList, this.mSaveToDisk);
    AudioManager.Instance.PlayCue(Banks.UI, "ui_checkpoint01".GetHashCodeCustom());
    for (int index = 0; index < players.Length; ++index)
    {
      if (players[index].Playing)
      {
        if (players[index].Avatar != null && players[index].Avatar.Dead)
        {
          if (NetworkManager.Instance.State != NetworkState.Client)
          {
            Revive instance = Revive.GetInstance();
            instance.SetSpecificPlayer(players[index].ID);
            Matrix oLocator;
            this.GameScene.GetLocator(this.mSpawnPoints[index], out oLocator);
            instance.Execute(oLocator.Translation, this.mScene.PlayState);
          }
        }
        else
        {
          players[index].Avatar.HitPoints = players[index].Avatar.MaxHitPoints;
          if (players[index].Avatar.Equipment[1].Item.Type == "staff_of_war".GetHashCodeCustom())
            players[index].Avatar.HitPoints = players[index].Avatar.MaxHitPoints * 2f;
        }
      }
    }
  }

  public override void QuickExecute()
  {
  }

  public string SpawnPoint
  {
    get => this.mSpawnPoint;
    set
    {
      this.mSpawnPoint = value;
      for (int index = 0; index < 4; ++index)
        this.mSpawnPoints[index] = (this.mSpawnPoint + (object) index).GetHashCodeCustom();
    }
  }

  public bool SpawnFairy
  {
    get => this.mSpawnFairy;
    set => this.mSpawnFairy = value;
  }

  public bool SaveToDisk
  {
    get => this.mSaveToDisk;
    set => this.mSaveToDisk = value;
  }
}
