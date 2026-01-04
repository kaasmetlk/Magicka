// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.SurvivalRuleset
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.GameLogic;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Statistics;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Graphics.Effects;
using Magicka.Levels.Packs;
using Magicka.Levels.Triggers;
using Magicka.Localization;
using Magicka.Network;
using Magicka.PathFinding;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;
using System.Collections.Generic;
using System.Xml;

#nullable disable
namespace Magicka.Levels;

public class SurvivalRuleset : IRuleset
{
  private const int PIEVERTEXCOUNT = 33;
  private const float SCORE_QUEUE_DELAY = 0.5f;
  private const float SCORE_TIME = 10f;
  private const float NETWORK_UPDATE_FREQ = 1f;
  private const float TIME_MULTIPLIER_DECAY = 30f;
  private const float DAMAGE_MULTIPLIER_CAP = 10000f;
  private static readonly char[] SCORE_DECIMAL = new char[1]
  {
    '.'
  };
  private static readonly char[] SCORE_SPACING = new char[1]
  {
    ' '
  };
  private static readonly char[] SCORE_POSITIVE = new char[1]
  {
    '+'
  };
  private static readonly char[] SCORE_MULTIPLY = new char[1]
  {
    'x'
  };
  private static readonly Random RANDOM = new Random();
  private static readonly BitmapFont SCROLL_FONT = FontManager.Instance.GetFont(MagickaFont.Maiandra14);
  private static readonly BitmapFont MULTI_FONT = FontManager.Instance.GetFont(MagickaFont.Maiandra16);
  private static readonly BitmapFont TOTAL_FONT = FontManager.Instance.GetFont(MagickaFont.MenuDefault);
  private static readonly Vector2 TIME_EFFECT_OFFSET = new Vector2(188f, 20f);
  private static readonly Vector2 TIME_TEXT_OFFSET = new Vector2(188f, 20f);
  private static readonly Vector2 TOTAL_SCORE_OFFSET = new Vector2(461f, 16f);
  private static readonly Vector2 TOTAL_SCORE_OVERLAY = new Vector2(256f, 0.0f);
  private static readonly Vector2 TOTAL_MULTIPLIER_OFFSET = new Vector2(224f, 32f);
  private static readonly Vector2 DAMAGE_BAR_OFFSET = new Vector2(48f, 0.0f);
  private static readonly Vector2 DAMAGE_TEXT_OFFSET = new Vector2(110f, 2f);
  private SortedList<SurvivalRuleset.ScrollScore, float> mScoreQueue = new SortedList<SurvivalRuleset.ScrollScore, float>(64 /*0x40*/);
  private SurvivalRuleset.ScrollScore[] mScrollScores = new SurvivalRuleset.ScrollScore[16 /*0x10*/];
  private Text[] mScrollTexts = new Text[16 /*0x10*/];
  private int mNrOfActiveTexts;
  private SurvivalRuleset.RenderData[] mRenderData;
  private Dictionary<int, string> mNames = new Dictionary<int, string>(64 /*0x40*/);
  private float mNetworkTimer;
  private int mCurrentTotalScore;
  private float mCurrentTotalScoreFloat;
  private int mTotalScore;
  private float mWaveTime;
  private int mTimeMultiplier;
  private float mCurrentDamageAmount;
  private float mTotalDamageStartAmount;
  private int mDamageMultiplier;
  private bool mNewTotalMultiplier;
  private int mTotalMultiplier;
  private int mPlayersAliveLastUpdate;
  private Vector2 mHudBarPosition;
  private int mNrOfSpawnedTreats;
  private float mLuggageTimer;
  private static readonly float TIMEBETWEENLUGGAGE = 30f;
  private CharacterTemplate mMagickTemplate;
  private CharacterTemplate mItemTemplate;
  private ConditionCollection mMagickConditions;
  private ConditionCollection mItemConditions;
  private List<int> mLuggageItems;
  private List<MagickType> mLuggageMagicks;
  public static float TIMEBETWEENLEVELS = 5f;
  private bool mInitialized;
  private int mTotalWavesNr;
  private int mCurrentWaveNr;
  private float mLevelChangeCountDown;
  public List<Wave> mWaves;
  public List<string> mAreas;
  private Wave mCurrentWave;
  private GameScene mGameScene;
  private WaveIndicator mWaveIndicator;
  private BossHealthBar mHealthBar;
  private float mPercentage;
  private Player[] mPlayers;

  public SurvivalRuleset(GameScene iScene, XmlNode iNode)
  {
    for (int index = 0; index < 16 /*0x10*/; ++index)
    {
      this.mScrollTexts[index] = new Text(200, SurvivalRuleset.SCROLL_FONT, TextAlign.Left, true);
      this.mScrollTexts[index].DrawShadows = true;
      this.mScrollTexts[index].ShadowAlpha = 1f;
      this.mScrollTexts[index].ShadowsOffset = new Vector2(1f, 1f);
      this.mScrollScores[index].Kill();
    }
    this.mNrOfActiveTexts = 0;
    this.mMagickTemplate = iScene.PlayState.Content.Load<CharacterTemplate>("Data/Characters/Luggage_magick");
    this.mItemTemplate = iScene.PlayState.Content.Load<CharacterTemplate>("Data/Characters/Luggage_item");
    this.mMagickConditions = new ConditionCollection();
    this.mItemConditions = new ConditionCollection();
    this.mGameScene = iScene;
    ItemPack[] itemPacks = PackMan.Instance.ItemPacks;
    for (int index = 0; index < itemPacks.Length; ++index)
    {
      if (itemPacks[index].Enabled)
      {
        foreach (string assetName in itemPacks[index].Items)
          this.mGameScene.PlayState.Content.Load<Item>(assetName);
      }
    }
    this.mLuggageItems = new List<int>(8);
    for (int index = 0; index < itemPacks.Length; ++index)
    {
      if (itemPacks[index].Enabled)
        this.mLuggageItems.AddRange((IEnumerable<int>) itemPacks[index].ItemIDs);
    }
    MagickPack[] magickPacks = PackMan.Instance.MagickPacks;
    this.mLuggageMagicks = new List<MagickType>(8);
    for (int index = 0; index < magickPacks.Length; ++index)
    {
      if (magickPacks[index].Enabled)
        this.mLuggageMagicks.AddRange((IEnumerable<MagickType>) magickPacks[index].Magicks);
    }
    if (iNode.Name.Equals("Ruleset", StringComparison.OrdinalIgnoreCase))
    {
      for (int i = 0; i < iNode.Attributes.Count; ++i)
      {
        if (iNode.Attributes[i].Name.Equals("waves", StringComparison.OrdinalIgnoreCase))
          this.mTotalWavesNr = int.Parse(iNode.Attributes[i].Value);
      }
    }
    this.mAreas = new List<string>();
    for (int i1 = 0; i1 < iNode.ChildNodes.Count; ++i1)
    {
      XmlNode childNode = iNode.ChildNodes[i1];
      if (childNode.Name.Equals("spawnAreas", StringComparison.OrdinalIgnoreCase))
      {
        int count = childNode.ChildNodes.Count;
        for (int i2 = 0; i2 < count; ++i2)
          this.mAreas.Add(childNode.ChildNodes[i2].InnerText);
      }
    }
    this.mWaves = new List<Wave>(this.mTotalWavesNr);
    PieEffect iPieEffect = (PieEffect) null;
    GUIBasicEffect iEffect;
    Texture2D iTexture;
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      iEffect = new GUIBasicEffect(Magicka.Game.Instance.GraphicsDevice, (EffectPool) null);
      iPieEffect = new PieEffect(Magicka.Game.Instance.GraphicsDevice, Magicka.Game.Instance.Content);
      iTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/HUD/hud");
    }
    Point screenSize = RenderManager.Instance.ScreenSize;
    int x = screenSize.X;
    this.mHudBarPosition.X = (float) (x / 2 - (int) (0.800000011920929 * (double) x) / 2);
    this.mHudBarPosition.Y = 32f;
    VertexPositionTexture[] data1 = new VertexPositionTexture[24];
    float num1 = 512f;
    float num2 = 64f;
    float num3 = 256f / (float) iTexture.Height;
    float num4 = 64f / (float) iTexture.Height;
    float num5 = 512f / (float) iTexture.Width;
    data1[0].Position.X = 0.0f;
    data1[0].Position.Y = num2;
    data1[0].TextureCoordinate.X = 0.0f;
    data1[0].TextureCoordinate.Y = num3 + num4;
    data1[1].Position.X = 0.0f;
    data1[1].Position.Y = 0.0f;
    data1[1].TextureCoordinate.X = 0.0f;
    data1[1].TextureCoordinate.Y = num3;
    data1[2].Position.X = num1;
    data1[2].Position.Y = 0.0f;
    data1[2].TextureCoordinate.X = num5;
    data1[2].TextureCoordinate.Y = num3;
    data1[3].Position.X = num1;
    data1[3].Position.Y = 0.0f;
    data1[3].TextureCoordinate.X = num5;
    data1[3].TextureCoordinate.Y = num3;
    data1[4].Position.X = num1;
    data1[4].Position.Y = num2;
    data1[4].TextureCoordinate.X = num5;
    data1[4].TextureCoordinate.Y = num3 + num4;
    data1[5].Position.X = 0.0f;
    data1[5].Position.Y = num2;
    data1[5].TextureCoordinate.X = 0.0f;
    data1[5].TextureCoordinate.Y = num3 + num4;
    float num6 = 320f / (float) iTexture.Height;
    float num7 = 128f / (float) iTexture.Width;
    float num8 = 32f / (float) iTexture.Height;
    float num9 = 96f / (float) iTexture.Width;
    data1[6].Position.X = 0.0f;
    data1[6].Position.Y = 32f;
    data1[6].TextureCoordinate.X = num7;
    data1[6].TextureCoordinate.Y = num6 + num8;
    data1[7].Position.X = 0.0f;
    data1[7].Position.Y = 0.0f;
    data1[7].TextureCoordinate.X = num7;
    data1[7].TextureCoordinate.Y = num6;
    data1[8].Position.X = 96f;
    data1[8].Position.Y = 0.0f;
    data1[8].TextureCoordinate.X = num7 + num9;
    data1[8].TextureCoordinate.Y = num6;
    data1[9].Position.X = 96f;
    data1[9].Position.Y = 0.0f;
    data1[9].TextureCoordinate.X = num7 + num9;
    data1[9].TextureCoordinate.Y = num6;
    data1[10].Position.X = 96f;
    data1[10].Position.Y = 32f;
    data1[10].TextureCoordinate.X = num7 + num9;
    data1[10].TextureCoordinate.Y = num6 + num8;
    data1[11].Position.X = 0.0f;
    data1[11].Position.Y = 32f;
    data1[11].TextureCoordinate.X = num7;
    data1[11].TextureCoordinate.Y = num6 + num8;
    data1[12].Position.X = 0.0f;
    data1[12].Position.Y = 32f;
    data1[12].TextureCoordinate.X = 0.0f;
    data1[12].TextureCoordinate.Y = num6 + num8;
    data1[13].Position.X = 0.0f;
    data1[13].Position.Y = 0.0f;
    data1[13].TextureCoordinate.X = 0.0f;
    data1[13].TextureCoordinate.Y = num6;
    data1[14].Position.X = 112f;
    data1[14].Position.Y = 0.0f;
    data1[14].TextureCoordinate.X = num9;
    data1[14].TextureCoordinate.Y = num6;
    data1[15].Position.X = 112f;
    data1[15].Position.Y = 0.0f;
    data1[15].TextureCoordinate.X = num9;
    data1[15].TextureCoordinate.Y = num6;
    data1[16 /*0x10*/].Position.X = 112f;
    data1[16 /*0x10*/].Position.Y = 32f;
    data1[16 /*0x10*/].TextureCoordinate.X = num9;
    data1[16 /*0x10*/].TextureCoordinate.Y = num6 + num8;
    data1[17].Position.X = 0.0f;
    data1[17].Position.Y = 32f;
    data1[17].TextureCoordinate.X = 0.0f;
    data1[17].TextureCoordinate.Y = num6 + num8;
    float num10 = 256f / (float) iTexture.Width;
    float num11 = 32f / (float) iTexture.Height;
    float num12 = 208f / (float) iTexture.Width;
    data1[18].Position.X = 0.0f;
    data1[18].Position.Y = 32f;
    data1[18].TextureCoordinate.X = num10;
    data1[18].TextureCoordinate.Y = num6 + num11;
    data1[19].Position.X = 0.0f;
    data1[19].Position.Y = 0.0f;
    data1[19].TextureCoordinate.X = num10;
    data1[19].TextureCoordinate.Y = num6;
    data1[20].Position.X = 208f;
    data1[20].Position.Y = 0.0f;
    data1[20].TextureCoordinate.X = num10 + num12;
    data1[20].TextureCoordinate.Y = num6;
    data1[21].Position.X = 208f;
    data1[21].Position.Y = 0.0f;
    data1[21].TextureCoordinate.X = num10 + num12;
    data1[21].TextureCoordinate.Y = num6;
    data1[22].Position.X = 208f;
    data1[22].Position.Y = 32f;
    data1[22].TextureCoordinate.X = num10 + num12;
    data1[22].TextureCoordinate.Y = num6 + num11;
    data1[23].Position.X = 0.0f;
    data1[23].Position.Y = 32f;
    data1[23].TextureCoordinate.X = num10;
    data1[23].TextureCoordinate.Y = num6 + num11;
    VertexPositionTexture[] data2 = new VertexPositionTexture[33];
    float num13 = 32f;
    Vector2 vector2;
    vector2.X = 32f / (float) iTexture.Width;
    vector2.Y = 384f / (float) iTexture.Height;
    data2[0].Position.X = 0.0f;
    data2[0].Position.Y = 0.0f;
    data2[0].TextureCoordinate.X = vector2.X;
    data2[0].TextureCoordinate.Y = vector2.Y;
    iPieEffect.Radius = 20f;
    iPieEffect.MaxAngle = 6.28318548f;
    iPieEffect.SetTechnique(PieEffect.Technique.Technique1);
    iPieEffect.Texture = (Texture) iTexture;
    iPieEffect.SetScreenSize(screenSize.X, screenSize.Y);
    for (int index = 1; index < 33; ++index)
    {
      float num14 = (float) (index - 1) / 31f;
      float num15 = (float) Math.Cos(((double) num14 - 0.25) * 6.2831854820251465);
      float num16 = (float) Math.Sin(((double) num14 - 0.25) * 6.2831854820251465);
      data2[index].Position.X = 1f;
      data2[index].Position.Y = num14;
      data2[index].TextureCoordinate.X = vector2.X + -num15 * num13 / (float) iTexture.Width;
      data2[index].TextureCoordinate.Y = vector2.Y + num16 * num13 / (float) iTexture.Height;
    }
    VertexDeclaration iVertexDeclaration;
    VertexBuffer iVertexBuffer;
    VertexDeclaration iPieVertexDeclaration;
    VertexBuffer iPieVertexBuffer;
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      iVertexDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
      iVertexBuffer = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, data1.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
      iVertexBuffer.SetData<VertexPositionTexture>(data1);
      iPieVertexDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
      iPieVertexBuffer = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, data2.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
      iPieVertexBuffer.SetData<VertexPositionTexture>(data2);
    }
    iEffect.TextureEnabled = true;
    iEffect.Texture = (Texture) iTexture;
    iEffect.Color = new Vector4(1f);
    iEffect.SetScreenSize(screenSize.X, screenSize.Y);
    this.mRenderData = new SurvivalRuleset.RenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      this.mRenderData[index] = new SurvivalRuleset.RenderData(iEffect, iVertexBuffer, iVertexDeclaration, VertexPositionTexture.SizeInBytes, iTexture, iPieEffect, iPieVertexBuffer, iPieVertexDeclaration);
      this.mRenderData[index].Texts = this.mScrollTexts;
    }
  }

  internal void ReadWave(XmlNode iNode)
  {
    Wave wave = new Wave(this.mGameScene);
    wave.Read(iNode);
    this.mWaves.Add(wave);
  }

  public void Initialize()
  {
    for (int index = 0; index < this.mWaves.Count; ++index)
      this.mWaves[index].Initialize(this);
    this.mHealthBar = new BossHealthBar(this.mGameScene.PlayState.Scene);
    this.mWaveIndicator = new WaveIndicator();
    this.mCurrentWave = this.mWaves[0];
    this.mCurrentWaveNr = 1;
    this.mTotalWavesNr = this.mWaves.Count + 1;
    this.mWaveIndicator.SetWave(this.mCurrentWaveNr);
    this.mLevelChangeCountDown = SurvivalRuleset.TIMEBETWEENLEVELS;
    this.mInitialized = true;
    this.mLuggageTimer = 0.0f;
    this.mCurrentTotalScore = 0;
    this.mCurrentTotalScoreFloat = 0.0f;
    this.mTotalScore = 0;
    this.mWaveTime = 0.0f;
    this.mTimeMultiplier = 5;
    this.mTotalDamageStartAmount = 0.0f;
    this.mDamageMultiplier = 1;
    this.mNrOfSpawnedTreats = 0;
    this.mNewTotalMultiplier = true;
    this.mTotalMultiplier = 6;
    this.mPercentage = 0.0f;
    StatisticsManager.Instance.SurvivalReset();
    this.mLuggageTimer = SurvivalRuleset.TIMEBETWEENLUGGAGE * 0.25f;
    this.mPlayersAliveLastUpdate = Magicka.Game.Instance.PlayerCount;
    this.mPlayers = Magicka.Game.Instance.Players;
    if (!this.mGameScene.Level.CurrentScene.Indoors)
      return;
    for (int index = 0; index < this.mLuggageMagicks.Count; ++index)
    {
      if (this.mLuggageMagicks[index] == MagickType.Blizzard | this.mLuggageMagicks[index] == MagickType.MeteorS | this.mLuggageMagicks[index] == MagickType.Napalm | this.mLuggageMagicks[index] == MagickType.Rain | this.mLuggageMagicks[index] == MagickType.SPhoenix | this.mLuggageMagicks[index] == MagickType.ThunderB | this.mLuggageMagicks[index] == MagickType.ThunderS)
        this.mLuggageMagicks.RemoveAt(index--);
    }
  }

  public void DeInitialize()
  {
  }

  public int ScoreMultiplier => this.mTimeMultiplier * this.mDamageMultiplier;

  public void AddScore(int iName, int iScore)
  {
    if (iScore == 0)
      return;
    for (int index = 0; index < this.mScoreQueue.Keys.Count; ++index)
    {
      if (this.mScoreQueue.Keys[index].NameID == iName)
      {
        SurvivalRuleset.ScrollScore key = this.mScoreQueue.Keys[index];
        ++key.MultiKills;
        float num = this.mScoreQueue.Values[index];
        this.mScoreQueue.RemoveAt(index);
        this.mScoreQueue.Add(key, num);
        return;
      }
    }
    this.mScoreQueue.Add(new SurvivalRuleset.ScrollScore()
    {
      Score = iScore,
      NameID = iName,
      MultiKills = 1
    }, 0.5f);
  }

  protected bool AddScrollScore(ref SurvivalRuleset.ScrollScore iScore)
  {
    for (int index = 0; index < this.mScrollScores.Length; ++index)
    {
      if (this.mScrollScores[index].Dead)
      {
        iScore.Position = (float) (this.mNrOfActiveTexts + 5);
        iScore.TargetPosition = 0.0f;
        this.mScrollScores[index] = iScore;
        this.mScrollTexts[index].Clear();
        string iChars = LanguageManager.Instance.GetString(iScore.NameID);
        this.mScrollTexts[index].Append(iChars);
        if (iScore.MultiKills > 1)
        {
          this.mScrollTexts[index].Append(SurvivalRuleset.SCORE_SPACING);
          this.mScrollTexts[index].Append(SurvivalRuleset.SCORE_MULTIPLY);
          this.mScrollTexts[index].Append(iScore.MultiKills);
        }
        this.mScrollTexts[index].Append(SurvivalRuleset.SCORE_SPACING);
        if (iScore.Score > 0)
          this.mScrollTexts[index].Append(SurvivalRuleset.SCORE_POSITIVE);
        this.mScrollTexts[index].Append(iScore.Score);
        ++this.mNrOfActiveTexts;
        this.mTotalScore += iScore.Score * iScore.MultiKills;
        return true;
      }
    }
    return false;
  }

  public void Update(float iDeltaTime, DataChannel iDataChan)
  {
    if (!this.mInitialized || iDataChan == DataChannel.None)
      return;
    if (NetworkManager.Instance.State == NetworkState.Server)
    {
      if ((double) this.mNetworkTimer <= 0.0)
      {
        this.mNetworkTimer = 1f;
        this.NetworkUpdate();
      }
      this.mNetworkTimer -= iDeltaTime;
    }
    if (this.mCurrentWave != null)
    {
      this.mWaveTime += iDeltaTime;
      this.mCurrentWave.Update(iDeltaTime, this);
      TriggerArea triggerArea = this.mGameScene.PlayState.Level.CurrentScene.GetTriggerArea(TriggerArea.ANYID);
      int count1 = triggerArea.GetCount(this.mItemTemplate.ID);
      int count2 = triggerArea.GetCount(this.mMagickTemplate.ID);
      Factions iFaction = Factions.EVIL | Factions.WILD | Factions.DEMON;
      if (this.mCurrentWave.HasStarted() && this.mCurrentWave.IsDone() && triggerArea.GetFactionCount(iFaction) <= count1 + count2)
      {
        if ((double) this.mLevelChangeCountDown <= 0.0)
        {
          if (this.mCurrentWaveNr + 1 > this.mWaves.Count)
          {
            if (!this.mGameScene.PlayState.IsGameEnded)
            {
              if (NetworkManager.Instance.State == NetworkState.Server)
              {
                GameEndMessage iMessage;
                iMessage.Condition = EndGameCondition.Victory;
                iMessage.Phony = false;
                iMessage.DelayTime = 0.0f;
                iMessage.Argument = 1;
                NetworkManager.Instance.Interface.SendMessage<GameEndMessage>(ref iMessage);
              }
              this.mGameScene.PlayState.Endgame(EndGameCondition.Victory, true, false, 0.0f);
            }
          }
          else
          {
            this.mCurrentWave = this.mWaves[this.mCurrentWaveNr++];
            this.mWaveIndicator.SetWave(this.mCurrentWaveNr);
            this.mLevelChangeCountDown = SurvivalRuleset.TIMEBETWEENLEVELS;
            this.mNrOfSpawnedTreats = 0;
            this.mTimeMultiplier = 5;
            this.mWaveTime = 0.0f;
          }
        }
        else
          this.mLevelChangeCountDown -= iDeltaTime;
      }
    }
    this.mHealthBar.SetNormalizedHealth(this.mCurrentWave.HitPointPercentage());
    this.mHealthBar.Update(iDataChan, iDeltaTime);
    this.mWaveIndicator.Update(iDeltaTime, iDataChan, this.mGameScene.Scene);
    this.mLuggageTimer -= iDeltaTime;
    if ((double) this.mLuggageTimer <= 0.0 && this.mNrOfSpawnedTreats < 2)
    {
      TriggerArea triggerArea = this.mGameScene.PlayState.Level.CurrentScene.GetTriggerArea(TriggerArea.ANYID);
      int count3 = triggerArea.GetCount(this.mItemTemplate.ID);
      int count4 = triggerArea.GetCount(this.mMagickTemplate.ID);
      if (count3 == 0 && count4 == 0)
      {
        if (this.mNrOfSpawnedTreats == 0)
          this.SpawnLuggage(false);
        else
          this.SpawnLuggage(true);
        ++this.mNrOfSpawnedTreats;
        this.mLuggageTimer += SurvivalRuleset.TIMEBETWEENLUGGAGE;
      }
      else
        this.mLuggageTimer += SurvivalRuleset.TIMEBETWEENLUGGAGE * 0.5f;
    }
    SurvivalRuleset.RenderData iObject = this.mRenderData[(int) iDataChan];
    for (int index = 0; index < this.mScoreQueue.Count; ++index)
    {
      float num1 = this.mScoreQueue.Values[index] - iDeltaTime;
      if ((double) num1 <= 0.0)
      {
        SurvivalRuleset.ScrollScore key = this.mScoreQueue.Keys[index];
        if (this.AddScrollScore(ref key))
        {
          this.mScoreQueue.RemoveAt(index);
        }
        else
        {
          float num2 = num1 + 0.25f;
        }
      }
      else
        this.mScoreQueue[this.mScoreQueue.Keys[index]] = num1;
    }
    for (int index = 0; index < this.mScrollScores.Length; ++index)
    {
      if (!this.mScrollScores[index].Dead && this.mScrollScores[index].Update(iDeltaTime))
        --this.mNrOfActiveTexts;
    }
    this.mScrollScores.CopyTo((Array) iObject.mScrollScore, 0);
    this.mCurrentTotalScoreFloat += (float) (((double) StatisticsManager.Instance.SurvivalTotalScore - (double) this.mCurrentTotalScore) * (double) iDeltaTime * 5.0);
    this.mCurrentTotalScore = (int) this.mCurrentTotalScoreFloat;
    iObject.SetTotalScore(this.mCurrentTotalScore);
    int playerCount = Magicka.Game.Instance.PlayerCount;
    Player[] players = Magicka.Game.Instance.Players;
    for (int index = 0; index < players.Length; ++index)
    {
      if (players[index].Playing && (players[index].Avatar == null || players[index].Avatar.Dead))
        --playerCount;
    }
    if (playerCount < this.mPlayersAliveLastUpdate)
    {
      this.mDamageMultiplier = 1;
      this.mTotalDamageStartAmount = StatisticsManager.Instance.SurvivalTotalDamage;
    }
    this.mPlayersAliveLastUpdate = playerCount;
    if (this.mTimeMultiplier > 1 && (double) this.mWaveTime > 30.0)
    {
      this.mWaveTime -= 30f;
      --this.mTimeMultiplier;
      this.mNewTotalMultiplier = true;
    }
    float num = StatisticsManager.Instance.SurvivalTotalDamage - this.mTotalDamageStartAmount;
    if (this.mDamageMultiplier < 5 && (double) num > 10000.0)
    {
      this.mTotalDamageStartAmount += 10000f;
      num -= 10000f;
      ++this.mDamageMultiplier;
      this.mNewTotalMultiplier = true;
    }
    if (this.mNewTotalMultiplier)
      this.mTotalMultiplier = this.mDamageMultiplier * this.mTimeMultiplier;
    this.mCurrentDamageAmount += (float) (((double) num - (double) this.mCurrentDamageAmount) * (double) iDeltaTime * 5.0);
    iObject.SetTotalMultiPlier(this.mTotalMultiplier, 0);
    iObject.SetSurvivalMultiplier(this.mDamageMultiplier, 0);
    iObject.SetTimeMultiplier(this.mTimeMultiplier, 0);
    iObject.NormalizedTime = this.mTimeMultiplier <= 1 ? 1f : (float) (1.0 - (double) this.mWaveTime / 30.0);
    iObject.DamageAmount = Math.Min(this.mCurrentDamageAmount / 10000f, 1f);
    iObject.DamageBarPosition = 0.0f;
    iObject.BarPosition = this.mHudBarPosition;
    this.mGameScene.Scene.AddRenderableGUIObject(iDataChan, (IRenderableGUIObject) iObject);
  }

  public void LocalUpdate(float iDeltaTime, DataChannel iDataChan)
  {
    if (!this.mInitialized || iDataChan == DataChannel.None)
      return;
    this.mHealthBar.SetNormalizedHealth(this.mPercentage);
    this.mHealthBar.Update(iDataChan, iDeltaTime);
    this.mWaveIndicator.Update(iDeltaTime, iDataChan, this.mGameScene.Scene);
    SurvivalRuleset.RenderData iObject = this.mRenderData[(int) iDataChan];
    for (int index = 0; index < this.mScoreQueue.Count; ++index)
    {
      float num1 = this.mScoreQueue.Values[index] - iDeltaTime;
      if ((double) num1 <= 0.0)
      {
        SurvivalRuleset.ScrollScore key = this.mScoreQueue.Keys[index];
        if (this.AddScrollScore(ref key))
        {
          this.mScoreQueue.RemoveAt(index);
        }
        else
        {
          float num2 = num1 + 0.25f;
          this.mScoreQueue[this.mScoreQueue.Keys[index]] = num2;
        }
      }
      else
        this.mScoreQueue[this.mScoreQueue.Keys[index]] = num1;
    }
    for (int index = 0; index < this.mScrollScores.Length; ++index)
    {
      if (!this.mScrollScores[index].Dead && this.mScrollScores[index].Update(iDeltaTime))
        --this.mNrOfActiveTexts;
    }
    this.mScrollScores.CopyTo((Array) iObject.mScrollScore, 0);
    this.mCurrentTotalScoreFloat += (float) (((double) StatisticsManager.Instance.SurvivalTotalScore - (double) this.mCurrentTotalScore) * (double) iDeltaTime * 5.0);
    this.mCurrentTotalScore = (int) this.mCurrentTotalScoreFloat;
    iObject.SetTotalScore(this.mCurrentTotalScore);
    this.mCurrentDamageAmount += (float) (((double) (StatisticsManager.Instance.SurvivalTotalDamage - this.mTotalDamageStartAmount) - (double) this.mCurrentDamageAmount) * (double) iDeltaTime * 5.0);
    iObject.SetTotalMultiPlier(this.mTotalMultiplier, 0);
    iObject.SetSurvivalMultiplier(this.mDamageMultiplier, 0);
    iObject.SetTimeMultiplier(this.mTimeMultiplier, 0);
    iObject.NormalizedTime = this.mTimeMultiplier <= 1 ? 1f : (float) (1.0 - (double) this.mWaveTime / 30.0);
    if (this.mNewTotalMultiplier)
      this.mTotalMultiplier = this.mDamageMultiplier * this.mTimeMultiplier;
    iObject.DamageAmount = Math.Min(this.mCurrentDamageAmount / 10000f, 1f);
    iObject.DamageBarPosition = 0.0f;
    iObject.BarPosition = this.mHudBarPosition;
    this.mGameScene.Scene.AddRenderableGUIObject(iDataChan, (IRenderableGUIObject) iObject);
  }

  public void AddedCharacter(NonPlayerCharacter iChar, bool iItemEvent)
  {
    if ((iChar.Faction & (Factions.EVIL | Factions.WILD | Factions.DEMON)) == Factions.NONE)
      return;
    this.mCurrentWave.TrackCharacter(iChar, iItemEvent);
  }

  private void NetworkUpdate()
  {
    NetworkManager.Instance.Interface.SendMessage<RulesetMessage>(ref new RulesetMessage()
    {
      Type = Rulesets.Survival,
      Float01 = this.mCurrentDamageAmount,
      Byte02 = (byte) this.mDamageMultiplier,
      Byte01 = (byte) this.mTimeMultiplier,
      Float02 = this.mTotalDamageStartAmount,
      PackedFloat = this.mCurrentWave.HitPointPercentage(),
      Byte03 = (byte) this.mWaveIndicator.WaveNum
    });
  }

  void IRuleset.NetworkUpdate(ref RulesetMessage iMsg)
  {
    this.mCurrentDamageAmount = iMsg.Float01;
    if ((int) iMsg.Byte02 != this.mDamageMultiplier)
      this.mNewTotalMultiplier = true;
    this.mDamageMultiplier = (int) iMsg.Byte02;
    if ((int) iMsg.Byte01 != this.mTimeMultiplier)
      this.mNewTotalMultiplier = true;
    this.mTimeMultiplier = (int) iMsg.Byte01;
    this.mPercentage = iMsg.PackedFloat;
    this.mTotalDamageStartAmount = iMsg.Float02;
    if ((int) iMsg.Byte03 == this.mWaveIndicator.WaveNum)
      return;
    this.mWaveIndicator.SetWave((int) iMsg.Byte03);
    this.mCurrentWaveNr = (int) iMsg.Byte03;
  }

  protected void SpawnLuggage(bool iSpawnItem)
  {
    Camera camera = (Camera) this.mGameScene.PlayState.Camera;
    Matrix oMatrix;
    camera.GetViewProjectionMatrix(Magicka.Game.Instance.UpdatingDataChannel, out oMatrix);
    Matrix result1;
    Matrix.Invert(ref oMatrix, out result1);
    Vector4 vector = new Vector4();
    vector.Z = 1f;
    vector.W = 1f;
    Microsoft.Xna.Framework.Ray ray = new Microsoft.Xna.Framework.Ray();
    ray.Position = camera.Position;
    Microsoft.Xna.Framework.Plane plane = new Microsoft.Xna.Framework.Plane();
    plane.Normal = Vector3.Up;
    plane.D = (float) -((double) ray.Position.Y - (double) MagickCamera.CAMERAOFFSET.Y);
    NavMesh navMesh = this.mGameScene.PlayState.Level.CurrentScene.NavMesh;
    for (int index1 = 0; index1 < 10; ++index1)
    {
      vector.X = (float) ((SurvivalRuleset.RANDOM.NextDouble() - 0.5) * 1.8999999761581421);
      vector.Y = (float) ((SurvivalRuleset.RANDOM.NextDouble() - 0.5) * 1.8999999761581421);
      Vector4 result2;
      Vector4.Transform(ref vector, ref result1, out result2);
      float num1 = 1f / result2.W;
      ray.Direction.X = result2.X * num1;
      ray.Direction.Y = result2.Y * num1;
      ray.Direction.Z = result2.Z * num1;
      Vector3.Subtract(ref ray.Direction, ref ray.Position, out ray.Direction);
      float? result3;
      ray.Intersects(ref plane, out result3);
      Vector3.Multiply(ref ray.Direction, result3.Value, out ray.Direction);
      Vector3.Add(ref ray.Position, ref ray.Direction, out ray.Direction);
      Vector3 oPoint;
      double nearestPosition = (double) navMesh.GetNearestPosition(ref ray.Direction, out oPoint, MovementProperties.Default);
      Vector4 result4;
      Vector4.Transform(ref oPoint, ref oMatrix, out result4);
      if ((double) Math.Abs(result4.X / result4.W) < 1.0 && (double) Math.Abs(result4.X / result4.W) < 1.0)
      {
        Vector3 oPos;
        if (this.mGameScene.PlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out Vector3 _, new Segment()
        {
          Origin = oPoint,
          Delta = {
            Y = -4f
          }
        }))
        {
          CharacterTemplate iTemplate;
          ConditionCollection conditionCollection;
          if (iSpawnItem && this.mLuggageItems.Count > 0)
          {
            iTemplate = this.mItemTemplate;
            conditionCollection = this.mItemConditions;
            int mLuggageItem = this.mLuggageItems[SurvivalRuleset.RANDOM.Next(this.mLuggageItems.Count)];
            conditionCollection[0].Clear();
            conditionCollection[0].Add(new EventStorage(new SpawnItemEvent(mLuggageItem)));
            conditionCollection[1].Clear();
            conditionCollection[1].Add(new EventStorage(new SpawnItemEvent(mLuggageItem)));
          }
          else
          {
            if (this.mLuggageMagicks.Count <= 0)
              break;
            iTemplate = this.mMagickTemplate;
            conditionCollection = this.mMagickConditions;
            int num2 = 0;
            bool flag;
            MagickType iType;
            do
            {
              flag = true;
              iType = this.mLuggageMagicks[(int) (Math.Pow(SurvivalRuleset.RANDOM.NextDouble(), 2.0) * (double) this.mLuggageMagicks.Count)];
              ++num2;
              if (iType == MagickType.Corporealize)
                flag = false;
              if (!Helper.CheckMagickDLC(iType))
                flag = false;
              ulong num3 = 1UL << (int) (iType & (MagickType.Wave | MagickType.PerformanceEnchantment));
              for (int index2 = 0; index2 < this.mPlayers.Length; ++index2)
              {
                if (this.mPlayers[index2].Playing && ((long) this.mPlayers[index2].UnlockedMagicks & (long) num3) != 0L)
                  flag = false;
              }
              if (num2 > 100)
              {
                flag = true;
                iType = MagickType.Revive;
              }
            }
            while (!flag);
            conditionCollection[0].Clear();
            conditionCollection[0].Add(new EventStorage(new SpawnMagickEvent(iType)));
            conditionCollection[1].Clear();
            conditionCollection[1].Add(new EventStorage(new SpawnMagickEvent(iType)));
          }
          conditionCollection[0].Condition.Repeat = false;
          conditionCollection[0].Condition.Count = 1;
          conditionCollection[0].Condition.Activated = false;
          conditionCollection[0].Condition.EventConditionType = EventConditionType.Death;
          conditionCollection[1].Condition.Repeat = false;
          conditionCollection[1].Condition.Count = 1;
          conditionCollection[1].Condition.Activated = false;
          conditionCollection[1].Condition.EventConditionType = EventConditionType.OverKill;
          oPos.Y += iTemplate.Length * 0.5f + iTemplate.Radius;
          Matrix result5;
          Matrix.CreateRotationY((float) SurvivalRuleset.RANDOM.NextDouble() * 6.28318548f, out result5);
          NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(this.mGameScene.PlayState);
          instance.Initialize(iTemplate, oPos, 0);
          instance.Body.Orientation = result5;
          instance.CharacterBody.DesiredDirection = result5.Forward;
          instance.SpawnAnimation = Animations.spawn;
          instance.ChangeState((BaseState) RessurectionState.Instance);
          this.mGameScene.PlayState.EntityManager.AddEntity((Entity) instance);
          instance.EventConditions = conditionCollection;
          result5.Translation = oPos;
          EffectManager.Instance.StartEffect("luggage_spawn".GetHashCodeCustom(), ref result5, out VisualEffectReference _);
          instance.Faction = Factions.EVIL;
          if (NetworkManager.Instance.State != NetworkState.Server)
            break;
          NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
          {
            ActionType = TriggerActionType.SpawnLuggage,
            Handle = instance.Handle,
            Template = instance.Template.ID,
            Position = instance.Position,
            Direction = instance.Direction,
            Point0 = 170
          });
          break;
        }
      }
    }
  }

  public int GetAnyArea()
  {
    return this.mAreas[MagickaMath.Random.Next(this.mAreas.Count)].GetHashCodeCustom();
  }

  public int WaveIndex => this.mCurrentWaveNr;

  public Wave CurrentWave
  {
    get => this.mCurrentWave;
    set => this.mCurrentWave = value;
  }

  public List<string> GetAreas() => this.mAreas;

  public Rulesets RulesetType => Rulesets.Survival;

  public bool IsVersusRuleset => false;

  protected class RenderData : IRenderableGUIObject
  {
    private PieEffect mPieEffect;
    private VertexBuffer mPieVertexBuffer;
    private VertexDeclaration mPieVertexDeclaration;
    private GUIBasicEffect mGUIBasicEffect;
    private VertexBuffer mVertexBuffer;
    private VertexDeclaration mVertexDeclaration;
    private int mVertexStride;
    private bool mDamageDirty;
    private int mDamageMul;
    private int mDamageDecimalMul;
    private Text mDamageMulText;
    public float DamageAmount;
    public float DamageBarPosition;
    public Vector2 BarPosition;
    private bool mTotalDirty;
    private int mTotalMul;
    private int mTotalDecimalMul;
    private Text mTotalMultiPlierText;
    private bool mTimeDirty;
    private int mTimeMul;
    private int mTimeDecimalMul;
    private Text mTimeMulText;
    private float mTimeMulOffset;
    public float NormalizedTime;
    private bool mTotalScoreDirty;
    private int mTotalScore;
    private Text mTotalScoreText;
    private Texture2D mHudTexture;
    public SurvivalRuleset.ScrollScore[] mScrollScore = new SurvivalRuleset.ScrollScore[16 /*0x10*/];
    public Text[] Texts;
    private float mScrollOffset;
    private float mHalfLineHeight;
    private float mTotalLineHeight;

    public RenderData(
      GUIBasicEffect iEffect,
      VertexBuffer iVertexBuffer,
      VertexDeclaration iVertexDeclaration,
      int iVertexStride,
      Texture2D iTexture,
      PieEffect iPieEffect,
      VertexBuffer iPieVertexBuffer,
      VertexDeclaration iPieVertexDeclaration)
    {
      this.mPieEffect = iPieEffect;
      this.mPieVertexBuffer = iPieVertexBuffer;
      this.mPieVertexDeclaration = iPieVertexDeclaration;
      this.mHudTexture = iTexture;
      this.mDamageMulText = new Text(10, SurvivalRuleset.MULTI_FONT, TextAlign.Right, false);
      this.mDamageMulText.Append(SurvivalRuleset.SCORE_MULTIPLY);
      this.mDamageMulText.Append(5);
      this.mDamageMulText.DrawShadows = true;
      this.mDamageMulText.ShadowAlpha = 1f;
      this.mDamageMulText.ShadowsOffset = new Vector2(1f, 1f);
      this.mTimeMulText = new Text(10, SurvivalRuleset.MULTI_FONT, TextAlign.Center, false);
      this.mTimeMulText.Append(SurvivalRuleset.SCORE_MULTIPLY);
      this.mTimeMulText.Append(5);
      this.mTimeMulText.DrawShadows = true;
      this.mTimeMulText.ShadowAlpha = 1f;
      this.mTimeMulText.ShadowsOffset = new Vector2(1f, 1f);
      this.mTotalScoreText = new Text(200, SurvivalRuleset.TOTAL_FONT, TextAlign.Right, true);
      this.mTotalScoreText.Append(0);
      this.mTotalScoreText.DrawShadows = true;
      this.mTotalScoreText.ShadowAlpha = 1f;
      this.mTotalScoreText.ShadowsOffset = new Vector2(1f, 1f);
      this.mTotalMultiPlierText = new Text(200, SurvivalRuleset.TOTAL_FONT, TextAlign.Center, false);
      this.mTotalMultiPlierText.Append(SurvivalRuleset.SCORE_MULTIPLY);
      this.mTotalMultiPlierText.Append(10);
      this.mTotalMultiPlierText.DrawShadows = true;
      this.mTotalMultiPlierText.ShadowAlpha = 1f;
      this.mTotalMultiPlierText.ShadowsOffset = new Vector2(1f, 1f);
      this.mTimeMulOffset = SurvivalRuleset.MULTI_FONT.MeasureText(this.mDamageMulText.Characters, true).X;
      this.mScrollOffset = (float) SurvivalRuleset.MULTI_FONT.LineHeight;
      this.mGUIBasicEffect = iEffect;
      this.mVertexBuffer = iVertexBuffer;
      this.mVertexDeclaration = iVertexDeclaration;
      this.mVertexStride = iVertexStride;
      this.mHalfLineHeight = (float) SurvivalRuleset.MULTI_FONT.LineHeight * 0.5f;
      this.mTotalLineHeight = (float) SurvivalRuleset.TOTAL_FONT.LineHeight * 0.5f;
    }

    public void SetTotalMultiPlier(int iMul, int iDecimalMul)
    {
      if (iMul == this.mTotalMul && iDecimalMul == this.mTotalDecimalMul)
        return;
      this.mTotalDecimalMul = iDecimalMul;
      this.mTotalMul = iMul;
      this.mTotalDirty = true;
    }

    public void SetSurvivalMultiplier(int iMul, int iDecimalMul)
    {
      if (iMul == this.mDamageMul && iDecimalMul == this.mDamageDecimalMul)
        return;
      this.mDamageDecimalMul = iDecimalMul;
      this.mDamageMul = iMul;
      this.mDamageDirty = true;
    }

    public void SetTimeMultiplier(int iMul, int iDecimalMul)
    {
      if (iMul == this.mTimeMul && iDecimalMul == this.mTimeDecimalMul)
        return;
      this.mTimeDecimalMul = iDecimalMul;
      this.mTimeMul = iMul;
      this.mTimeDirty = true;
    }

    public void SetTotalScore(int iScore)
    {
      if (iScore == this.mTotalScore)
        return;
      this.mTotalScore = iScore;
      this.mTotalScoreDirty = true;
    }

    public void Draw(float iDeltaTime)
    {
      if (this.mTotalDirty)
      {
        this.mTotalMultiPlierText.Clear();
        this.mTotalMultiPlierText.Append(this.mTotalMul);
        this.mTotalMultiPlierText.Append(SurvivalRuleset.SCORE_MULTIPLY);
        this.mTotalDirty = false;
      }
      if (this.mDamageDirty)
      {
        this.mDamageMulText.Clear();
        this.mDamageMulText.Append(this.mDamageMul);
        this.mDamageMulText.Append(SurvivalRuleset.SCORE_MULTIPLY);
        this.mDamageDirty = false;
      }
      if (this.mTimeDirty)
      {
        this.mTimeMulText.Clear();
        this.mTimeMulText.Append(this.mTimeMul);
        this.mTimeMulText.Append(SurvivalRuleset.SCORE_MULTIPLY);
        this.mTimeDirty = false;
      }
      if (this.mTotalScoreDirty)
      {
        this.mTotalScoreText.Clear();
        this.mTotalScoreText.Append(this.mTotalScore);
        this.mTotalScoreDirty = false;
      }
      Point screenSize = RenderManager.Instance.ScreenSize;
      float iScale = (float) screenSize.Y / 720f;
      Vector2 vector2 = new Vector2((float) Math.Floor((double) screenSize.X * 0.05000000074505806 + 0.5), (float) Math.Floor((double) screenSize.Y * 0.05000000074505806 + 0.5));
      this.mGUIBasicEffect.SetScreenSize(screenSize.X, screenSize.Y);
      this.mGUIBasicEffect.Color = Vector4.One;
      this.mGUIBasicEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, this.mVertexStride);
      this.mGUIBasicEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
      this.mGUIBasicEffect.Texture = (Texture) this.mHudTexture;
      this.mGUIBasicEffect.TextureEnabled = true;
      Matrix identity = Matrix.Identity with
      {
        M41 = this.BarPosition.X,
        M42 = this.BarPosition.Y
      };
      this.mGUIBasicEffect.Transform = identity;
      this.mGUIBasicEffect.Begin();
      this.mGUIBasicEffect.CurrentTechnique.Passes[0].Begin();
      this.mGUIBasicEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 2);
      identity.M11 = this.DamageAmount;
      identity.M41 = this.BarPosition.X + SurvivalRuleset.DAMAGE_BAR_OFFSET.X;
      identity.M42 = this.BarPosition.Y + SurvivalRuleset.DAMAGE_BAR_OFFSET.Y;
      this.mGUIBasicEffect.Transform = identity;
      this.mGUIBasicEffect.CommitChanges();
      this.mGUIBasicEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 6, 2);
      identity.M11 = 1f;
      this.mGUIBasicEffect.Transform = identity;
      this.mGUIBasicEffect.CommitChanges();
      this.mGUIBasicEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 12, 2);
      this.mGUIBasicEffect.CurrentTechnique.Passes[0].End();
      this.mGUIBasicEffect.End();
      this.mPieEffect.GraphicsDevice.RenderState.CullMode = CullMode.None;
      this.mPieEffect.GraphicsDevice.Vertices[0].SetSource(this.mPieVertexBuffer, 0, this.mVertexStride);
      this.mPieEffect.GraphicsDevice.VertexDeclaration = this.mPieVertexDeclaration;
      identity.M41 = this.BarPosition.X + SurvivalRuleset.TIME_EFFECT_OFFSET.X;
      identity.M42 = this.BarPosition.Y + SurvivalRuleset.TIME_EFFECT_OFFSET.Y;
      identity.M11 = 0.0f;
      identity.M12 = -1f;
      identity.M21 = -1f;
      identity.M22 = 0.0f;
      identity.M33 = -1f;
      this.mPieEffect.Transform = identity;
      this.mPieEffect.MaxAngle = this.NormalizedTime * 6.28318548f;
      this.mPieEffect.Begin();
      this.mPieEffect.CurrentTechnique.Passes[0].Begin();
      this.mPieEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 31 /*0x1F*/);
      this.mPieEffect.CurrentTechnique.Passes[0].End();
      this.mPieEffect.End();
      this.mPieEffect.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
      this.mGUIBasicEffect.Begin();
      this.mGUIBasicEffect.CurrentTechnique.Passes[0].Begin();
      this.mTotalScoreText.Draw(this.mGUIBasicEffect, this.BarPosition.X + SurvivalRuleset.TOTAL_SCORE_OFFSET.X, this.BarPosition.Y + SurvivalRuleset.TOTAL_SCORE_OFFSET.Y - this.mTotalLineHeight);
      this.mDamageMulText.Draw(this.mGUIBasicEffect, this.BarPosition.X + SurvivalRuleset.DAMAGE_TEXT_OFFSET.X, this.BarPosition.Y + SurvivalRuleset.DAMAGE_TEXT_OFFSET.Y);
      this.mTimeMulText.Draw(this.mGUIBasicEffect, this.BarPosition.X + SurvivalRuleset.TIME_TEXT_OFFSET.X, this.BarPosition.Y + SurvivalRuleset.TIME_TEXT_OFFSET.Y - this.mHalfLineHeight);
      this.mTotalMultiPlierText.Draw(this.mGUIBasicEffect, this.BarPosition.X + SurvivalRuleset.TOTAL_MULTIPLIER_OFFSET.X, this.BarPosition.Y + SurvivalRuleset.TOTAL_MULTIPLIER_OFFSET.Y - this.mTotalLineHeight);
      this.mGUIBasicEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, this.mVertexStride);
      this.mGUIBasicEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
      this.mGUIBasicEffect.Texture = (Texture) this.mHudTexture;
      this.mGUIBasicEffect.TextureEnabled = true;
      identity = Matrix.Identity with
      {
        M41 = this.BarPosition.X + SurvivalRuleset.TOTAL_SCORE_OVERLAY.X,
        M42 = this.BarPosition.Y + SurvivalRuleset.TOTAL_SCORE_OVERLAY.Y
      };
      this.mGUIBasicEffect.Color = Vector4.One;
      this.mGUIBasicEffect.Transform = identity;
      this.mGUIBasicEffect.CommitChanges();
      this.mGUIBasicEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 18, 2);
      Vector4 one = Vector4.One;
      for (int index = 0; index < this.mScrollScore.Length; ++index)
      {
        if (!this.mScrollScore[index].Dead)
        {
          one.W = this.mScrollScore[index].Alpha;
          this.mGUIBasicEffect.Color = one;
          this.Texts[index].Draw(this.mGUIBasicEffect, vector2.X, (float) ((double) vector2.Y + (double) this.mScrollOffset * 2.0 + (double) this.mScrollScore[index].Position * 16.0), iScale);
        }
      }
      this.mGUIBasicEffect.CurrentTechnique.Passes[0].End();
      this.mGUIBasicEffect.End();
    }

    public int ZIndex => 205;
  }

  public struct ScrollScore : IComparable<SurvivalRuleset.ScrollScore>
  {
    private float TimeAlive;
    public float Position;
    public float TargetPosition;
    public int Score;
    public int NameID;
    public int MultiKills;
    private float mAlpha;

    public float Alpha => this.mAlpha;

    public void Kill() => this.Position = 0.0f;

    public bool Dead => (double) this.Position <= 0.0;

    public bool Update(float iDeltaTime)
    {
      this.TimeAlive += iDeltaTime;
      this.Position -= iDeltaTime * 2f;
      this.mAlpha = MathHelper.Min(this.TimeAlive, 0.25f) * 4f;
      this.mAlpha *= MathHelper.Max(MathHelper.Min(0.25f, this.Position), 0.0f) * 4f;
      return this.Dead;
    }

    public int CompareTo(SurvivalRuleset.ScrollScore other)
    {
      if (this.NameID > other.NameID)
        return 1;
      return this.NameID < other.NameID ? -1 : 0;
    }
  }
}
