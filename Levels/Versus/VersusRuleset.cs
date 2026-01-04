// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Versus.VersusRuleset
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.Audio;
using Magicka.GameLogic;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Statistics;
using Magicka.GameLogic.UI;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Graphics.Effects;
using Magicka.Levels.Packs;
using Magicka.Levels.Triggers;
using Magicka.Localization;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

#nullable disable
namespace Magicka.Levels.Versus;

public abstract class VersusRuleset : IRuleset
{
  internal const int TEAM_A_RED = 0;
  internal const int TEAM_B_BLUE = 1;
  protected const float COUNTDOWN = 4f;
  internal static readonly int LOC_TEAMRED = "#add_team_red".GetHashCodeCustom();
  internal static readonly int LOC_TEAMBLUE = "#add_team_blue".GetHashCodeCustom();
  internal static readonly int LOC_GAME_OVER = "#game_over".GetHashCodeCustom();
  internal static readonly int SOUND_GAME_END = "game_end".GetHashCodeCustom();
  internal static readonly int SOUND_GAME_START = "game_start".GetHashCodeCustom();
  internal static readonly int SOUND_GAME_CLOCK = "game_clock".GetHashCodeCustom();
  protected static Random RANDOM = new Random();
  protected GameScene mScene;
  protected int[] mAreas_All;
  protected int[] mAreas_TeamA;
  protected int[] mAreas_TeamB;
  protected VersusRuleset.RenderData[] mRenderData;
  protected List<VersusRuleset.Score> mScoreUIs;
  protected Magicka.GameLogic.Player[] mPlayers;
  protected List<int> mTemporarySpawns;
  protected Dictionary<int, int> mIDToScoreUILookUp = new Dictionary<int, int>(4);
  protected float mCountDownTimer;
  protected CharacterTemplate mItemTemplate;
  protected CharacterTemplate mMagickTemplate;
  private ConditionCollection mItemConditions;
  private ConditionCollection mMagickConditions;
  private List<int> mLuggageItems;
  private List<MagickType> mLuggageMagicks;
  protected Cue mGameClockCue;
  protected float mIntroCueTimer;
  protected bool mGameOver;

  public VersusRuleset(GameScene iScene, XmlNode iNode)
  {
    this.mScene = iScene;
    List<string> stringList1 = new List<string>();
    List<string> stringList2 = new List<string>();
    List<string> stringList3 = new List<string>();
    for (int i1 = 0; i1 < iNode.ChildNodes.Count; ++i1)
    {
      XmlNode childNode1 = iNode.ChildNodes[i1];
      if (childNode1.Name.Equals("Areas", StringComparison.OrdinalIgnoreCase))
      {
        for (int i2 = 0; i2 < childNode1.ChildNodes.Count; ++i2)
        {
          XmlNode childNode2 = childNode1.ChildNodes[i2];
          if (childNode2.Name.Equals("Area", StringComparison.OrdinalIgnoreCase))
          {
            for (int i3 = 0; i3 < childNode2.Attributes.Count; ++i3)
            {
              XmlAttribute attribute = childNode2.Attributes[i3];
              if (attribute.Name.Equals("team", StringComparison.OrdinalIgnoreCase))
              {
                if (attribute.Value.Equals("B", StringComparison.OrdinalIgnoreCase) || attribute.Value.Equals("blue", StringComparison.OrdinalIgnoreCase))
                  stringList3.Add(childNode2.InnerText);
                else if (attribute.Value.Equals("a", StringComparison.OrdinalIgnoreCase) || attribute.Value.Equals("red", StringComparison.OrdinalIgnoreCase))
                  stringList2.Add(childNode2.InnerText);
              }
            }
            stringList1.Add(childNode2.InnerText);
          }
        }
      }
    }
    this.mAreas_All = new int[stringList1.Count];
    for (int index = 0; index < stringList1.Count; ++index)
      this.mAreas_All[index] = stringList1[index].ToLowerInvariant().GetHashCodeCustom();
    this.mAreas_TeamA = new int[stringList2.Count];
    for (int index = 0; index < stringList2.Count; ++index)
      this.mAreas_TeamA[index] = stringList2[index].ToLowerInvariant().GetHashCodeCustom();
    this.mAreas_TeamB = new int[stringList3.Count];
    for (int index = 0; index < stringList3.Count; ++index)
      this.mAreas_TeamB[index] = stringList3[index].ToLowerInvariant().GetHashCodeCustom();
    this.mItemTemplate = this.mScene.PlayState.Content.Load<CharacterTemplate>("Data/Characters/Luggage_item");
    this.mMagickTemplate = this.mScene.PlayState.Content.Load<CharacterTemplate>("Data/Characters/Luggage_magick");
    this.mItemConditions = new ConditionCollection();
    this.mMagickConditions = new ConditionCollection();
    ItemPack[] itemPacks = PackMan.Instance.ItemPacks;
    for (int index = 0; index < itemPacks.Length; ++index)
    {
      if (itemPacks[index].Enabled)
      {
        foreach (string assetName in itemPacks[index].Items)
          this.mScene.PlayState.Content.Load<Item>(assetName);
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
    this.mScoreUIs = new List<VersusRuleset.Score>();
    GUIBasicEffect iEffect = (GUIBasicEffect) null;
    PieEffect iPieEffect = (PieEffect) null;
    Texture2D iPieTexture = (Texture2D) null;
    Texture2D iCountdownTexture = (Texture2D) null;
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      iEffect = new GUIBasicEffect(RenderManager.Instance.GraphicsDevice, (EffectPool) null);
      iPieEffect = new PieEffect(RenderManager.Instance.GraphicsDevice, Magicka.Game.Instance.Content);
      iCountdownTexture = this.mScene.PlayState.Content.Load<Texture2D>("UI/HUD/versus_countdown");
      iPieTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/HUD/hud");
    }
    iPieEffect.Radius = 32f;
    iPieEffect.MaxAngle = 6.28318548f;
    iPieEffect.SetTechnique(PieEffect.Technique.Technique1);
    Point screenSize = RenderManager.Instance.ScreenSize;
    iPieEffect.SetScreenSize(screenSize.X, screenSize.Y);
    this.mRenderData = new VersusRuleset.RenderData[3];
    for (int index = 0; index < 3; ++index)
      this.mRenderData[index] = new VersusRuleset.RenderData(iEffect, iPieEffect, iPieTexture, iCountdownTexture, this.mScoreUIs);
    this.mTemporarySpawns = new List<int>(this.mAreas_All.Length);
    KeyboardHUD.Instance.Reset();
  }

  protected void CountDown(float iTime)
  {
    this.mCountDownTimer = iTime;
    ControlManager.Instance.LimitInput((object) this.mScene.Level);
  }

  public virtual void OnPlayerKill(Avatar atkAvatar, Avatar tarAvatar)
  {
  }

  public virtual void OnPlayerDeath(Magicka.GameLogic.Player iPlayer)
  {
  }

  protected void EndGame()
  {
    if (this.mGameOver)
      return;
    if (this.mGameClockCue != null && !this.mGameClockCue.IsStopping)
      this.mGameClockCue.Stop(AudioStopOptions.AsAuthored);
    AudioManager.Instance.PlayCue(Banks.Additional, VersusRuleset.SOUND_GAME_END);
    this.mScene.PlayState.Endgame(EndGameCondition.EndOffGame, true, false, 0.0f);
    if (NetworkManager.Instance.State == NetworkState.Server)
      NetworkManager.Instance.Interface.SendMessage<RulesetMessage>(ref new RulesetMessage()
      {
        Type = this.RulesetType,
        Byte01 = (byte) 1
      });
    this.OnEndGame();
  }

  public virtual void OnEndGame()
  {
  }

  public virtual int GetAnyArea()
  {
    return this.mAreas_All[VersusRuleset.RANDOM.Next(this.mAreas_All.Length)];
  }

  public virtual void Update(float iDeltaTime, DataChannel iDataChannel)
  {
    if ((double) this.mIntroCueTimer < 0.25)
    {
      this.mIntroCueTimer += iDeltaTime;
      if ((double) this.mIntroCueTimer >= 0.25)
        AudioManager.Instance.PlayCue(Banks.Additional, VersusRuleset.SOUND_GAME_START);
    }
    for (int index1 = 0; index1 < this.mPlayers.Length; ++index1)
    {
      int index2 = this.mIDToScoreUILookUp[index1];
      if (index2 != -1 && !this.mPlayers[index1].Playing)
      {
        this.mScoreUIs[index2].RemovePlayer(index1);
        this.mIDToScoreUILookUp[index1] = -1;
      }
    }
    if (Magicka.Game.Instance.PlayerCount <= 1 && !this.mGameOver)
      this.EndGame();
    VersusRuleset.RenderData iObject = this.mRenderData[(int) iDataChannel];
    iObject.GameOver = this.mScene.PlayState.IsGameEnded;
    if ((double) this.mCountDownTimer > -1.0)
    {
      this.mCountDownTimer -= iDeltaTime;
      if ((double) this.mCountDownTimer <= 0.0)
        ControlManager.Instance.UnlimitInput((object) this.mScene.Level);
    }
    iObject.SetCountDown(this.mCountDownTimer);
    this.mScene.Scene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) iObject);
  }

  public virtual void LocalUpdate(float iDeltaTime, DataChannel iDataChannel)
  {
    if ((double) this.mIntroCueTimer < 0.25)
    {
      this.mIntroCueTimer += iDeltaTime;
      if ((double) this.mIntroCueTimer >= 0.25)
        AudioManager.Instance.PlayCue(Banks.Additional, VersusRuleset.SOUND_GAME_START);
    }
    VersusRuleset.RenderData iObject = this.mRenderData[(int) iDataChannel];
    if ((double) this.mCountDownTimer > -1.0)
    {
      this.mCountDownTimer -= iDeltaTime;
      if ((double) this.mCountDownTimer <= 0.0)
        ControlManager.Instance.UnlimitInput((object) this.mScene.Level);
    }
    iObject.SetCountDown(this.mCountDownTimer);
    this.mScene.Scene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) iObject);
  }

  public virtual void NetworkUpdate(ref RulesetMessage iMsg)
  {
    switch (iMsg.Byte01)
    {
      case 1:
        this.EndGame();
        break;
      case 3:
        if (!this.mPlayers[(int) iMsg.Byte02].Playing)
          break;
        Matrix oOrientation;
        this.GetMatrix(iMsg.Integer01, out oOrientation);
        if (this.mPlayers[(int) iMsg.Byte02].Avatar == null || this.mPlayers[(int) iMsg.Byte02].Avatar.Dead)
        {
          int num = (int) this.RevivePlayer((int) iMsg.Byte02, iMsg.Integer01, ref oOrientation, new ushort?(iMsg.UShort01));
          break;
        }
        this.MovePlayer((int) iMsg.Byte02, ref oOrientation);
        this.mPlayers[(int) iMsg.Byte02].Avatar.HitPoints = this.mPlayers[(int) iMsg.Byte02].Avatar.MaxHitPoints;
        this.mPlayers[(int) iMsg.Byte02].Avatar.StopStatusEffects(StatusEffects.Burning | StatusEffects.Wet | StatusEffects.Frozen | StatusEffects.Cold | StatusEffects.Poisoned | StatusEffects.Healing | StatusEffects.Greased | StatusEffects.Steamed);
        break;
    }
  }

  public virtual void Initialize()
  {
    this.mScoreUIs.Clear();
    this.mPlayers = Magicka.Game.Instance.Players;
    for (int index = 0; index < this.mPlayers.Length; ++index)
      this.mPlayers[index].UnlockedMagicks = 0UL;
    StatisticsManager.Instance.SurvivalReset();
    for (int key = 0; key < this.mPlayers.Length; ++key)
    {
      if (this.mPlayers[key].Playing)
        this.mPlayers[key].Avatar.Faction &= ~Factions.FRIENDLY;
      this.mIDToScoreUILookUp[key] = -1;
    }
    if (this.mScene.Level.CurrentScene.Indoors)
    {
      for (int index = 0; index < this.mLuggageMagicks.Count; ++index)
      {
        if (this.mLuggageMagicks[index] == MagickType.Blizzard | this.mLuggageMagicks[index] == MagickType.MeteorS | this.mLuggageMagicks[index] == MagickType.Napalm | this.mLuggageMagicks[index] == MagickType.Rain | this.mLuggageMagicks[index] == MagickType.SPhoenix | this.mLuggageMagicks[index] == MagickType.ThunderB | this.mLuggageMagicks[index] == MagickType.ThunderS)
          this.mLuggageMagicks.RemoveAt(index--);
      }
    }
    this.mGameOver = false;
    this.CountDown(4f);
    if (this.mGameClockCue != null && !this.mGameClockCue.IsStopping)
      this.mGameClockCue.Stop(AudioStopOptions.AsAuthored);
    this.mIntroCueTimer = 0.0f;
  }

  public abstract void DeInitialize();

  public abstract Rulesets RulesetType { get; }

  public int GetTeamArea(Factions iFaction)
  {
    if ((iFaction & Factions.TEAM_RED) == Factions.TEAM_RED)
      return this.GetTeamArea(0);
    return (iFaction & Factions.TEAM_BLUE) == Factions.TEAM_BLUE ? this.GetTeamArea(1) : this.GetAnyArea();
  }

  protected virtual int GetTeamArea(int iTeam)
  {
    if (iTeam == 0)
      return this.mAreas_TeamA[VersusRuleset.RANDOM.Next(this.mAreas_TeamA.Length)];
    return 1 == iTeam ? this.mAreas_TeamB[VersusRuleset.RANDOM.Next(this.mAreas_TeamB.Length)] : this.GetAnyArea();
  }

  protected bool GetMatrix(int iArea, out Matrix oOrientation)
  {
    oOrientation = Matrix.Identity;
    Matrix oLocator;
    if (!this.mScene.TryGetLocator(iArea, out oLocator))
    {
      oLocator = Matrix.Identity;
      TriggerArea oArea;
      if (!this.mScene.TryGetTriggerArea(iArea, out oArea))
        return false;
      oLocator.Translation = oArea.GetRandomLocation();
    }
    Segment seg = new Segment();
    seg.Origin = oLocator.Translation;
    ++seg.Origin.Y;
    seg.Delta.Y = -3f;
    float num;
    Vector3 vector3_1;
    Vector3 vector3_2;
    if (!this.mScene.Level.CurrentScene.SegmentIntersect(out num, out vector3_1, out vector3_2, seg))
    {
      bool flag = false;
      for (int index = 0; index < this.mScene.Level.CurrentScene.Liquids.Length; ++index)
      {
        if (this.mScene.Level.CurrentScene.Liquids[index].SegmentIntersect(out num, out vector3_1, out vector3_2, ref seg, true, true, false))
        {
          oLocator.Translation = vector3_1;
          flag = true;
          break;
        }
      }
      if (!flag)
        return false;
    }
    else
      oLocator.Translation = vector3_1;
    oOrientation = oLocator;
    return true;
  }

  protected void SetupPlayer(int iID, int iArea)
  {
    Matrix oOrientation;
    this.GetMatrix(iArea, out oOrientation);
    if (this.mPlayers[iID].Avatar != null && !this.mPlayers[iID].Avatar.Dead)
    {
      if (!(this.mPlayers[iID].Gamer is NetworkGamer))
      {
        this.MovePlayer(iID, ref oOrientation);
        this.mPlayers[iID].Avatar.HitPoints = this.mPlayers[iID].Avatar.MaxHitPoints;
        this.mPlayers[iID].Avatar.StopStatusEffects(StatusEffects.Burning | StatusEffects.Wet | StatusEffects.Frozen | StatusEffects.Cold | StatusEffects.Poisoned | StatusEffects.Healing | StatusEffects.Greased | StatusEffects.Steamed);
      }
      else
      {
        if (NetworkManager.Instance.State != NetworkState.Server)
          return;
        NetworkManager.Instance.Interface.SendMessage<RulesetMessage>(ref new RulesetMessage()
        {
          Type = this.RulesetType,
          Byte01 = (byte) 3,
          Byte02 = (byte) iID,
          Integer01 = iArea,
          UShort01 = this.mPlayers[iID].Avatar.Handle
        });
      }
    }
    else
    {
      if (NetworkManager.Instance.State == NetworkState.Client)
        return;
      ushort num = this.RevivePlayer(iID, iArea, ref oOrientation, new ushort?());
      if (NetworkManager.Instance.State != NetworkState.Server)
        return;
      NetworkManager.Instance.Interface.SendMessage<RulesetMessage>(ref new RulesetMessage()
      {
        Type = this.RulesetType,
        Byte01 = (byte) 3,
        Byte02 = (byte) iID,
        Integer01 = iArea,
        UShort01 = num
      });
    }
  }

  protected void MovePlayer(int iID, ref Matrix iTransform)
  {
    Avatar avatar = this.mPlayers[iID].Avatar;
    Vector3 translation = iTransform.Translation;
    iTransform.Translation = new Vector3();
    translation.Y += (float) ((double) avatar.Capsule.Length * 0.5 + (double) avatar.Radius + 0.15000000596046448);
    avatar.Body.MoveTo(translation, iTransform);
    avatar.Body.SetActive();
    avatar.StopAllActions();
    avatar.ResetSpell();
    if (avatar.Player != null)
      avatar.Player.IconRenderer.Clear();
    avatar.SpellQueue.Clear();
    avatar.CastType = CastType.None;
    avatar.ChangeState((BaseState) IdleState.Instance);
    avatar.ResetRestingTimers();
  }

  protected ushort RevivePlayer(int iID, int iArea, ref Matrix iTransform, ushort? iHandle)
  {
    Vector3 translation = iTransform.Translation;
    iTransform.Translation = new Vector3();
    Vector3 iDirection = new Vector3(2f, 0.0f, 0.0f);
    Damage iDamage = new Damage(AttackProperties.Status, Elements.Cold, 100f, 4f);
    Liquid.Freeze(this.mScene.Level.CurrentScene, ref translation, ref iDirection, 6.28318548f, 2f, ref iDamage);
    CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(this.mPlayers[iID].Gamer.Avatar.Type);
    translation.Y += (float) ((double) cachedTemplate.Length * 0.5 + (double) cachedTemplate.Radius + 0.15000000596046448);
    this.mPlayers[iID].Weapon = (string) null;
    this.mPlayers[iID].Staff = (string) null;
    Avatar iEntity = !iHandle.HasValue ? Avatar.GetFromCache(this.mPlayers[iID]) : Avatar.GetFromCache(this.mPlayers[iID], iHandle.Value);
    iEntity.Initialize(cachedTemplate, translation, Magicka.GameLogic.Player.UNIQUE_ID[iID]);
    iEntity.TimedEthereal(0.0f, true);
    iEntity.TimedEthereal(2.25f, false);
    iEntity.Body.MoveTo(translation, iTransform);
    iEntity.Faction &= ~Factions.FRIENDLY;
    iEntity.HitPoints = iEntity.MaxHitPoints;
    iEntity.SpawnAnimation = Magicka.Animations.revive;
    iEntity.ChangeState((BaseState) RessurectionState.Instance);
    this.mScene.PlayState.EntityManager.AddEntity((Entity) iEntity);
    AudioManager.Instance.PlayCue(Banks.Spells, Revive.SOUNDHASH, iEntity.AudioEmitter);
    this.mPlayers[iID].Avatar = iEntity;
    this.mPlayers[iID].Ressing = false;
    if (this.mPlayers[iID].Controller is XInputController)
      (this.mPlayers[iID].Controller as XInputController).Rumble(2f, 2f);
    return iEntity.Handle;
  }

  protected void SpawnLuggage(bool iSpawnItem)
  {
    CharacterTemplate iTemplate;
    ConditionCollection conditionCollection;
    if (iSpawnItem && this.mLuggageItems.Count > 0)
    {
      iTemplate = this.mItemTemplate;
      conditionCollection = this.mItemConditions;
      int mLuggageItem = this.mLuggageItems[VersusRuleset.RANDOM.Next(this.mLuggageItems.Count)];
      conditionCollection[0].Clear();
      conditionCollection[0].Add(new EventStorage(new SpawnItemEvent(mLuggageItem, 12f)));
      conditionCollection[1].Clear();
      conditionCollection[1].Add(new EventStorage(new SpawnItemEvent(mLuggageItem, 12f)));
    }
    else
    {
      if (this.mLuggageMagicks.Count <= 0)
        return;
      iTemplate = this.mMagickTemplate;
      conditionCollection = this.mMagickConditions;
      MagickType mLuggageMagick = this.mLuggageMagicks[VersusRuleset.RANDOM.Next(this.mLuggageMagicks.Count)];
      conditionCollection[0].Clear();
      conditionCollection[0].Add(new EventStorage(new SpawnMagickEvent(mLuggageMagick, 12f)));
      conditionCollection[1].Clear();
      conditionCollection[1].Add(new EventStorage(new SpawnMagickEvent(mLuggageMagick, 12f)));
    }
    conditionCollection[0].Condition.Repeat = false;
    conditionCollection[0].Condition.Count = 1;
    conditionCollection[0].Condition.Activated = false;
    conditionCollection[0].Condition.EventConditionType = EventConditionType.Death;
    conditionCollection[1].Condition.Repeat = false;
    conditionCollection[1].Condition.Count = 1;
    conditionCollection[1].Condition.Activated = false;
    conditionCollection[1].Condition.EventConditionType = EventConditionType.OverKill;
    int anyArea = this.GetAnyArea();
    Matrix oLocator;
    if (!this.mScene.TryGetLocator(anyArea, out oLocator))
    {
      oLocator = Matrix.Identity;
      TriggerArea oArea;
      if (this.mScene.TryGetTriggerArea(anyArea, out oArea))
        oLocator.Translation = oArea.GetRandomLocation();
    }
    Vector3 translation = oLocator.Translation;
    translation.Y += iTemplate.Length * 0.5f + iTemplate.Radius;
    oLocator.Translation = new Vector3();
    NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(this.mScene.PlayState);
    instance.Initialize(iTemplate, translation, 0);
    instance.Body.Orientation = oLocator;
    instance.CharacterBody.DesiredDirection = oLocator.Forward;
    instance.SpawnAnimation = Magicka.Animations.spawn;
    instance.ChangeState((BaseState) RessurectionState.Instance);
    instance.Faction = Factions.EVIL;
    this.mScene.PlayState.EntityManager.AddEntity((Entity) instance);
    instance.EventConditions = conditionCollection;
    oLocator.Translation = translation;
    EffectManager.Instance.StartEffect("luggage_spawn".GetHashCodeCustom(), ref oLocator, out VisualEffectReference _);
    if (NetworkManager.Instance.State != NetworkState.Server)
      return;
    NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
    {
      ActionType = TriggerActionType.SpawnLuggage,
      Handle = instance.Handle,
      Template = instance.Template.ID,
      Position = instance.Position,
      Direction = instance.Direction,
      Point0 = 170
    });
  }

  public virtual bool DropMagicks => false;

  public abstract bool CanRevive(Magicka.GameLogic.Player iReviver, Magicka.GameLogic.Player iRevivee);

  internal abstract short[] GetTeamScores();

  internal abstract short[] GetScores();

  internal abstract bool Teams { get; }

  internal abstract class Settings
  {
    public const int DROPDOWNBOX_WIDTH = 200;
    protected int LOC_TIME_LIMIT = "#menu_vs_03".GetHashCodeCustom();
    protected int LOC_SCORE_LIMIT = "#menu_vs_04".GetHashCodeCustom();
    protected int LOC_UNLIMITED = "#menu_opt_alt_unlimited".GetHashCodeCustom();
    protected int LOC_NEVER = "#menu_opt_alt_never".GetHashCodeCustom();
    protected int LOC_TEAMS = "#menu_vs_09".GetHashCodeCustom();
    protected int LOC_OFF = "#menu_opt_alt_02".GetHashCodeCustom();
    protected int LOC_NO = "#add_menu_no".GetHashCodeCustom();
    protected int LOC_YES = "#add_menu_yes".GetHashCodeCustom();
    protected int LOC_LIVES = "#opt_vs_lives".GetHashCodeCustom();
    protected int LOC_LUGGAGE_INTERVAL = "#opt_vs_luggageinterval".GetHashCodeCustom();
    protected int LOC_LOW = "#menu_opt_alt_04".GetHashCodeCustom();
    protected int LOC_MEDIUM = "#menu_opt_alt_05".GetHashCodeCustom();
    protected int LOC_HIGH = "#menu_opt_alt_06".GetHashCodeCustom();
    protected int LOC_NONE = "#menu_opt_alt_09".GetHashCodeCustom();
    protected int LOC_TT_TIME = "#tooltip_vs_time".GetHashCodeCustom();
    protected int LOC_TT_SCORE = "#tooltip_vs_score".GetHashCodeCustom();
    protected int LOC_TT_LIVES = "#tooltip_vs_lives".GetHashCodeCustom();
    protected int LOC_TT_TEAMS = "#tooltip_vs_teams".GetHashCodeCustom();
    protected int LOC_TT_LUGGAGE = "#tooltip_vs_luggage".GetHashCodeCustom();
    private List<DropDownBox> mMenuItems;
    private List<int> mMenuTitles;
    private List<int> mToolTips;

    public event VersusRuleset.Settings.SettingChanged Changed;

    protected Settings()
    {
      this.mMenuItems = new List<DropDownBox>();
      this.mMenuTitles = new List<int>();
      this.mToolTips = new List<int>();
    }

    public DropDownBox<T> AddOption<T>(
      int iTitle,
      int iToolTip,
      T[] iValues,
      int?[] iLocalization)
    {
      DropDownBox<T> dropDownBox = new DropDownBox<T>(FontManager.Instance.GetFont(MagickaFont.MenuOption), iValues, iLocalization, 200);
      dropDownBox.SelectedIndexChanged += new Action<DropDownBox, int>(this.OnChanged);
      this.mMenuItems.Add((DropDownBox) dropDownBox);
      this.mMenuTitles.Add(iTitle);
      this.mToolTips.Add(iToolTip);
      return dropDownBox;
    }

    public IList<DropDownBox> MenuItems => (IList<DropDownBox>) this.mMenuItems;

    public IList<int> MenuTitles => (IList<int>) this.mMenuTitles;

    public IList<int> ToolTips => (IList<int>) this.mToolTips;

    public abstract bool TeamsEnabled { get; }

    protected void OnChanged(DropDownBox iBox, int iNewIndex)
    {
      if (this.Changed == null)
        return;
      this.Changed(this.mMenuItems.IndexOf(iBox), iNewIndex);
    }

    public unsafe void GetMessage(out VersusRuleset.Settings.OptionsMessage oMessage)
    {
      switch (this)
      {
        case DeathMatch.Settings _:
          oMessage.Ruleset = Rulesets.DeathMatch;
          break;
        case Brawl.Settings _:
          oMessage.Ruleset = Rulesets.Brawl;
          break;
        case Krietor.Settings _:
          oMessage.Ruleset = Rulesets.Kreitor;
          break;
        case King.Settings _:
          oMessage.Ruleset = Rulesets.King;
          break;
        case Pyrite.Settings _:
          oMessage.Ruleset = Rulesets.Pyrite;
          break;
        default:
          throw new NotImplementedException();
      }
      oMessage.NrOfSettings = (byte) this.mMenuItems.Count;
      fixed (byte* numPtr = oMessage.Settings)
      {
        for (int index = 0; index < this.mMenuItems.Count; ++index)
          numPtr[index] = (byte) this.mMenuItems[index].SelectedIndex;
      }
    }

    public static unsafe void ApplyMessage(
      ref VersusRuleset.Settings iSettings,
      ref VersusRuleset.Settings.OptionsMessage iMessage)
    {
      switch (iMessage.Ruleset)
      {
        case Rulesets.DeathMatch:
          if (!(iSettings is DeathMatch.Settings))
          {
            iSettings = (VersusRuleset.Settings) new DeathMatch.Settings();
            break;
          }
          break;
        case Rulesets.Brawl:
          if (!(iSettings is Brawl.Settings))
          {
            iSettings = (VersusRuleset.Settings) new Brawl.Settings();
            break;
          }
          break;
        case Rulesets.Pyrite:
          if (!(iSettings is Pyrite.Settings))
          {
            iSettings = (VersusRuleset.Settings) new Pyrite.Settings();
            break;
          }
          break;
        case Rulesets.Kreitor:
          if (!(iSettings is Krietor.Settings))
          {
            iSettings = (VersusRuleset.Settings) new Krietor.Settings();
            break;
          }
          break;
        case Rulesets.King:
          if (!(iSettings is King.Settings))
          {
            iSettings = (VersusRuleset.Settings) new King.Settings();
            break;
          }
          break;
        default:
          iSettings = (VersusRuleset.Settings) null;
          Console.WriteLine("Invalid Ruleset type: " + (object) iMessage.Ruleset);
          return;
      }
      fixed (byte* numPtr = iMessage.Settings)
      {
        for (int index = 0; index < iSettings.mMenuItems.Count; ++index)
          iSettings.mMenuItems[index].SelectedIndex = (int) numPtr[index];
      }
    }

    public struct OptionsMessage : ISendable
    {
      public Rulesets Ruleset;
      public byte NrOfSettings;
      public unsafe fixed byte Settings[32];

      public PacketType PacketType => PacketType.VersusOptions;

      public unsafe void Write(BinaryWriter iWriter)
      {
        iWriter.Write((byte) this.Ruleset);
        iWriter.Write(this.NrOfSettings);
        fixed (byte* numPtr = this.Settings)
        {
          for (int index = 0; index < (int) this.NrOfSettings; ++index)
            iWriter.Write(numPtr[index]);
        }
      }

      public unsafe void Read(BinaryReader iReader)
      {
        this.Ruleset = (Rulesets) iReader.ReadByte();
        this.NrOfSettings = iReader.ReadByte();
        fixed (byte* numPtr = this.Settings)
        {
          for (int index = 0; index < (int) this.NrOfSettings; ++index)
            numPtr[index] = iReader.ReadByte();
        }
      }
    }

    public delegate void SettingChanged(int iOption, int iNewSelection);
  }

  protected struct DirtyItem<T>
  {
    public T Value;
    public T NewValue;
    public bool Dirty;
  }

  protected class Score
  {
    private const float SCORE_TEXT_WIDTH = 80f;
    private static VertexBuffer sVertexBuffer;
    private static VertexDeclaration sVertexDeclaration;
    private static BitmapFont sFont;
    private static Texture2D sTexture;
    private List<VersusRuleset.Score.Player> mPlayers;
    private List<VersusRuleset.DirtyItem<int>> mTimers;
    private List<Text> mRespawnTexts;
    private static readonly Vector2 sSize = new Vector2(74f, 74f);
    private VersusRuleset.DirtyItem<bool> mLeftAligned;
    private VersusRuleset.DirtyItem<int> mScore;
    private Text mScoreText;
    private float mScoreFontLineHeight;
    private float mTimerFontLineHeight;
    public bool HideNegativeScore;
    private BitmapFont mScoreFont;
    private BitmapFont mRespawnFont;

    public Score(bool iLeftAligned)
    {
      if (VersusRuleset.Score.sVertexBuffer == null)
      {
        lock (Magicka.Game.Instance.GraphicsDevice)
        {
          VersusRuleset.Score.sVertexBuffer = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, Defines.QUAD_TEX_VERTS_TL.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
          VersusRuleset.Score.sVertexBuffer.SetData<VertexPositionTexture>(Defines.QUAD_TEX_VERTS_TL);
          VersusRuleset.Score.sVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(VertexPositionTexture.VertexElements);
          VersusRuleset.Score.sFont = FontManager.Instance.GetFont(MagickaFont.Maiandra16);
          VersusRuleset.Score.sTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/HUD/hud");
        }
      }
      this.mPlayers = new List<VersusRuleset.Score.Player>();
      this.mLeftAligned.Dirty = false;
      this.mLeftAligned.Value = iLeftAligned;
      this.mScore.Dirty = false;
      this.mScore.Value = 0;
      this.mScoreFont = FontManager.Instance.GetFont(MagickaFont.VersusText);
      this.mScoreFontLineHeight = (float) this.mScoreFont.LineHeight;
      this.mScoreText = new Text(32 /*0x20*/, this.mScoreFont, TextAlign.Center, false);
      this.mScoreText.SetText(this.mScore.Value.ToString());
      this.mTimers = new List<VersusRuleset.DirtyItem<int>>();
      this.mRespawnTexts = new List<Text>();
      this.mRespawnFont = FontManager.Instance.GetFont(MagickaFont.VersusText);
      this.mTimerFontLineHeight = (float) this.mRespawnFont.LineHeight;
    }

    public void AddPlayer(string iName, int iID, Texture2D iTexture, Vector3 iColor)
    {
      VersusRuleset.Score.Player player = new VersusRuleset.Score.Player();
      player.ID = iID;
      player.Texture = iTexture;
      player.Color = new Vector4(iColor, 1f);
      this.mTimers.Add(new VersusRuleset.DirtyItem<int>()
      {
        Dirty = false,
        Value = 0
      });
      Text text = new Text(32 /*0x20*/, this.mRespawnFont, TextAlign.Center, false);
      text.SetText("");
      this.mRespawnTexts.Add(text);
      this.mPlayers.Add(player);
    }

    public void SetScore(int iScore)
    {
      if (iScore == this.mScore.Value)
        return;
      this.mScore.Dirty = true;
      this.mScore.NewValue = iScore;
    }

    public void SetTimer(int iID, int iTimer)
    {
      for (int index = 0; index < this.mPlayers.Count; ++index)
      {
        if (this.mPlayers[index].ID == iID && this.mTimers[index].Value != iTimer)
        {
          VersusRuleset.DirtyItem<int> mTimer = this.mTimers[index] with
          {
            NewValue = iTimer,
            Dirty = true
          };
          this.mTimers[index] = mTimer;
        }
      }
    }

    public void RemovePlayer(int iID)
    {
      for (int index = 0; index < this.mPlayers.Count; ++index)
      {
        if (this.mPlayers[index].ID == iID)
        {
          this.mPlayers.RemoveAt(index);
          this.mRespawnTexts.RemoveAt(index);
          this.mTimers.RemoveAt(index);
          break;
        }
      }
    }

    public bool LeftAligned
    {
      get => this.mLeftAligned.Value;
      set
      {
        if (value == this.mLeftAligned.Value)
          return;
        this.mLeftAligned.NewValue = value;
        this.mLeftAligned.Dirty = true;
      }
    }

    public float Width
    {
      get => (float) ((double) this.mPlayers.Count * (double) VersusRuleset.Score.sSize.X + 80.0);
    }

    public void Draw(GUIBasicEffect iEffect, float iX, float iY)
    {
      if (this.mPlayers.Count == 0)
        return;
      if (this.mLeftAligned.Dirty)
      {
        this.mLeftAligned.Value = this.mLeftAligned.NewValue;
        this.mLeftAligned.Dirty = false;
        BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuTitle);
        this.mScoreText = !this.mLeftAligned.Value ? new Text(32 /*0x20*/, font, TextAlign.Left, false) : new Text(32 /*0x20*/, font, TextAlign.Right, false);
      }
      if (this.mScore.Dirty)
      {
        this.mScore.Value = this.mScore.NewValue;
        this.mScore.Dirty = false;
        this.mScoreText.SetText(this.mScore.Value.ToString());
      }
      for (int index = 0; index < this.mTimers.Count; ++index)
      {
        VersusRuleset.DirtyItem<int> mTimer = this.mTimers[index];
        if (mTimer.Dirty)
        {
          mTimer.Value = this.mTimers[index].NewValue;
          mTimer.Dirty = false;
          this.mTimers[index] = mTimer;
          this.mRespawnTexts[index].SetText(mTimer.Value.ToString());
        }
      }
      iEffect.GraphicsDevice.Vertices[0].SetSource(VersusRuleset.Score.sVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
      iEffect.GraphicsDevice.VertexDeclaration = VersusRuleset.Score.sVertexDeclaration;
      iEffect.VertexColorEnabled = false;
      iEffect.GraphicsDevice.RenderState.CullMode = CullMode.None;
      Point screenSize = RenderManager.Instance.ScreenSize;
      iEffect.SetScreenSize(screenSize.X, screenSize.Y);
      float m41 = this.mLeftAligned.Value ? iX - this.Width : iX + 80f;
      Matrix matrix = !this.mLeftAligned.Value ? new Matrix(-23f, 0.0f, 0.0f, 0.0f, 0.0f, 106f, 0.0f, 0.0f, 0.0f, 0.0f, 1f, 0.0f, (float) ((double) m41 + (double) this.mPlayers.Count * (double) VersusRuleset.Score.sSize.X + 23.0), iY, 0.0f, 1f) : new Matrix(23f, 0.0f, 0.0f, 0.0f, 0.0f, 106f, 0.0f, 0.0f, 0.0f, 0.0f, 1f, 0.0f, m41 - 23f, iY, 0.0f, 1f);
      iEffect.Transform = matrix;
      iEffect.Texture = (Texture) VersusRuleset.Score.sTexture;
      iEffect.TextureOffset = new Vector2(0.5f, 0.0f);
      iEffect.TextureScale = new Vector2(23f / (float) VersusRuleset.Score.sTexture.Width, 106f / (float) VersusRuleset.Score.sTexture.Height);
      iEffect.TextureEnabled = true;
      iEffect.CommitChanges();
      iEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
      for (int iIndex = 0; iIndex < this.mPlayers.Count; ++iIndex)
      {
        matrix = new Matrix(74f, 0.0f, 0.0f, 0.0f, 0.0f, 32f, 0.0f, 0.0f, 0.0f, 0.0f, 1f, 0.0f, m41 + (float) iIndex * VersusRuleset.Score.sSize.X, iY, 0.0f, 1f);
        iEffect.Transform = matrix;
        iEffect.Texture = (Texture) VersusRuleset.Score.sTexture;
        iEffect.TextureOffset = new Vector2((float) (0.5 + 23.0 / (double) VersusRuleset.Score.sTexture.Width), 0.0f);
        iEffect.TextureScale = new Vector2(74f / (float) VersusRuleset.Score.sTexture.Width, 32f / (float) VersusRuleset.Score.sTexture.Height);
        iEffect.TextureEnabled = true;
        iEffect.CommitChanges();
        iEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
        float num = iY + 32f;
        this.DrawPlayer(iEffect, iIndex, m41 + (float) iIndex * VersusRuleset.Score.sSize.X, num);
        iEffect.Texture = (Texture) VersusRuleset.Score.sTexture;
        iEffect.TextureOffset = new Vector2((float) (0.5 + 23.0 / (double) VersusRuleset.Score.sTexture.Width), 32f / (float) VersusRuleset.Score.sTexture.Height);
        iEffect.TextureScale = new Vector2(74f / (float) VersusRuleset.Score.sTexture.Width, 74f / (float) VersusRuleset.Score.sTexture.Height);
        matrix = new Matrix(74f, 0.0f, 0.0f, 0.0f, 0.0f, 74f, 0.0f, 0.0f, 0.0f, 0.0f, 1f, 0.0f, m41 + (float) iIndex * VersusRuleset.Score.sSize.X, num, 0.0f, 1f);
        iEffect.Transform = matrix;
        iEffect.CommitChanges();
        iEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
      }
      matrix = !this.mLeftAligned.Value ? new Matrix(-64f, 0.0f, 0.0f, 0.0f, 0.0f, 90f, 0.0f, 0.0f, 0.0f, 0.0f, 1f, 0.0f, m41, iY, 0.0f, 1f) : new Matrix(64f, 0.0f, 0.0f, 0.0f, 0.0f, 90f, 0.0f, 0.0f, 0.0f, 0.0f, 1f, 0.0f, m41 + (float) this.mPlayers.Count * VersusRuleset.Score.sSize.X, iY, 0.0f, 1f);
      iEffect.Transform = matrix;
      iEffect.Texture = (Texture) VersusRuleset.Score.sTexture;
      iEffect.TextureOffset = new Vector2((float) (0.5 + 97.0 / (double) VersusRuleset.Score.sTexture.Width), 0.0f);
      iEffect.TextureScale = new Vector2(64f / (float) VersusRuleset.Score.sTexture.Width, 90f / (float) VersusRuleset.Score.sTexture.Height);
      iEffect.TextureEnabled = true;
      iEffect.CommitChanges();
      iEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
      iEffect.Color = new Vector4(1f);
      iEffect.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
      if (this.HideNegativeScore && this.mScore.Value < 0)
        return;
      if (this.mLeftAligned.Value)
        this.mScoreText.Draw(iEffect, (float) ((double) iX - 40.0 - 10.0), (float) ((double) iY + 24.0 + (double) VersusRuleset.Score.sSize.X * 0.5 - (double) this.mScoreFontLineHeight * 0.5 + 8.0), 0.8f);
      else
        this.mScoreText.Draw(iEffect, (float) ((double) iX + 40.0 + 11.0), (float) ((double) iY + 24.0 + (double) VersusRuleset.Score.sSize.X * 0.5 - (double) this.mScoreFontLineHeight * 0.5 + 8.0), 0.8f);
    }

    private void DrawPlayer(GUIBasicEffect iEffect, int iIndex, float iX, float iY)
    {
      Matrix matrix = new Matrix(64f, 0.0f, 0.0f, 0.0f, 0.0f, 64f, 0.0f, 0.0f, 0.0f, 0.0f, 1f, 0.0f, iX + 5f, iY + 5f, 0.0f, 1f);
      iEffect.Transform = matrix;
      iEffect.Color = new Vector4(1f);
      iEffect.TextureEnabled = true;
      iEffect.Texture = (Texture) VersusRuleset.Score.sTexture;
      iEffect.TextureOffset = new Vector2(448f / (float) VersusRuleset.Score.sTexture.Width, 0.0f);
      iEffect.TextureScale = new Vector2(64f / (float) VersusRuleset.Score.sTexture.Width, 64f / (float) VersusRuleset.Score.sTexture.Height);
      iEffect.CommitChanges();
      iEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
      iEffect.TextureOffset = new Vector2(0.0f, 0.5f);
      iEffect.TextureScale = new Vector2(1f, 0.5f);
      iEffect.Color = this.mPlayers[iIndex].Color;
      iEffect.Texture = (Texture) this.mPlayers[iIndex].Texture;
      iEffect.CommitChanges();
      iEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
      iEffect.TextureOffset = new Vector2(0.0f, 0.0f);
      iEffect.Color = new Vector4(1f);
      iEffect.CommitChanges();
      iEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
      if ((double) this.mTimers[iIndex].Value > 0.0)
      {
        iEffect.Color = new Vector4(1f);
        this.mRespawnTexts[iIndex].Draw(iEffect, (float) ((double) iX + (double) VersusRuleset.Score.sSize.X * 0.5 + 4.0), (float) ((double) iY + (double) VersusRuleset.Score.sSize.Y * 0.5 - (double) this.mTimerFontLineHeight * 0.5));
      }
      iEffect.GraphicsDevice.Vertices[0].SetSource(VersusRuleset.Score.sVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
      iEffect.GraphicsDevice.VertexDeclaration = VersusRuleset.Score.sVertexDeclaration;
    }

    private struct Player
    {
      public Texture2D Texture;
      public Vector4 Color;
      public int ID;
    }
  }

  protected class RenderData : IRenderableGUIObject
  {
    private const float PADDING = 4f;
    public const float CLOCK_RADIUS = 32f;
    private const int CLOCK_VERTS = 33;
    private static VertexBuffer sVertexBuffer;
    private static VertexDeclaration sVertexDeclaration;
    private static Texture2D sTexture;
    private static VertexBuffer sPieVertexBuffer;
    private static VertexDeclaration sPieVertexDeclaration;
    private static readonly Vector2 sPieOffset = new Vector2(15f / 16f, 3f / 16f);
    private static int LOC_SUDDEN_DEATH = "#opt_vs_sudden_death".GetHashCodeCustom();
    private Texture2D mCountdownTexture;
    private List<VersusRuleset.Score> mScores;
    private GUIBasicEffect mEffect;
    private PieEffect mPieEffect;
    public bool DrawTime;
    public float Time;
    public float TimeLimit;
    private VersusRuleset.DirtyItem<int> mTime;
    private Text mTimeText;
    private float mTimeFontHeight;
    private VersusRuleset.DirtyItem<int> mCountDown;
    private float mCountDownScale;
    private float mCountDownFloat;
    private VersusRuleset.DirtyItem<bool> mGameOver;
    private Text mGameOverText;
    private VersusRuleset.DirtyItem<bool> mSuddenDeath;
    private VersusRuleset.DirtyItem<int> mMagickTime;
    private Text mMagickTimeText;
    private VersusRuleset.DirtyItem<MagickType> mMagickType;
    private Text mMagickText;
    private float mMagickWidth;
    private float mMagickFontHeight;
    private BitmapFont mMagickTypeFont;

    public RenderData(
      GUIBasicEffect iEffect,
      PieEffect iPieEffect,
      Texture2D iPieTexture,
      Texture2D iCountdownTexture,
      List<VersusRuleset.Score> iScores)
    {
      this.mEffect = iEffect;
      this.mPieEffect = iPieEffect;
      this.mPieEffect.Texture = (Texture) iPieTexture;
      this.mScores = iScores;
      this.mMagickType.Value = MagickType.None;
      this.mMagickType.Dirty = false;
      this.mMagickTime.Value = 0;
      this.mMagickTime.Dirty = false;
      BitmapFont font = FontManager.Instance.GetFont(MagickaFont.VersusText);
      this.mTimeFontHeight = (float) font.LineHeight;
      this.mTimeText = new Text(10, font, TextAlign.Center, false);
      this.mTimeText.SetText("");
      this.mTimeText.DrawShadows = true;
      this.mTimeText.ShadowsOffset = new Vector2(2f, 2f);
      this.mMagickTypeFont = FontManager.Instance.GetFont(MagickaFont.VersusText);
      this.mMagickWidth = 0.0f;
      this.mMagickFontHeight = (float) font.LineHeight;
      this.mMagickText = new Text(50, font, TextAlign.Center, false);
      this.mMagickText.SetText("");
      this.mMagickText.DrawShadows = true;
      this.mMagickText.ShadowsOffset = new Vector2(2f, 2f);
      this.mMagickTimeText = new Text(20, font, TextAlign.Left, false);
      this.mMagickTimeText.SetText("");
      this.mMagickTimeText.DrawShadows = true;
      this.mMagickTimeText.ShadowsOffset = new Vector2(2f, 2f);
      this.mCountDown.Dirty = false;
      this.mCountDown.Value = this.mCountDown.NewValue = 0;
      this.mSuddenDeath.Dirty = false;
      this.mSuddenDeath.Value = false;
      this.mGameOverText = new Text(64 /*0x40*/, FontManager.Instance.GetFont(MagickaFont.VersusText), TextAlign.Center, false);
      this.mGameOverText.SetText(LanguageManager.Instance.GetString(VersusRuleset.LOC_GAME_OVER));
      this.mGameOver.Dirty = false;
      this.mGameOver.Value = false;
      this.mCountdownTexture = iCountdownTexture;
      if (VersusRuleset.RenderData.sPieVertexBuffer == null)
      {
        VertexPositionTexture[] data = new VertexPositionTexture[33];
        float num1 = 32f;
        Vector2 vector2 = new Vector2(0.0f, 0.0f);
        data[0].Position.X = 0.0f;
        data[0].Position.Y = 0.0f;
        data[0].TextureCoordinate.X = vector2.X;
        data[0].TextureCoordinate.Y = vector2.Y;
        for (int index = 1; index < 33; ++index)
        {
          float num2 = (float) (index - 1) / 31f;
          float num3 = (float) Math.Cos(((double) num2 - 0.25) * 6.2831854820251465);
          float num4 = (float) Math.Sin(((double) num2 - 0.25) * 6.2831854820251465);
          data[index].Position.X = 1f;
          data[index].Position.Y = num2;
          data[index].TextureCoordinate.X = vector2.X + -num3 * num1 / (float) iPieTexture.Width;
          data[index].TextureCoordinate.Y = vector2.Y + num4 * num1 / (float) iPieTexture.Height;
        }
        lock (Magicka.Game.Instance.GraphicsDevice)
        {
          VersusRuleset.RenderData.sPieVertexBuffer = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, data.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
          VersusRuleset.RenderData.sPieVertexBuffer.SetData<VertexPositionTexture>(data);
          VersusRuleset.RenderData.sPieVertexDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
        }
      }
      if (VersusRuleset.RenderData.sVertexBuffer != null)
        return;
      lock (Magicka.Game.Instance.GraphicsDevice)
      {
        VersusRuleset.RenderData.sVertexBuffer = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, Defines.QUAD_TEX_VERTS_TL.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
        VersusRuleset.RenderData.sVertexBuffer.SetData<VertexPositionTexture>(Defines.QUAD_TEX_VERTS_TL);
        VersusRuleset.RenderData.sVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(VertexPositionTexture.VertexElements);
        VersusRuleset.RenderData.sTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/HUD/hud");
      }
    }

    public void SetUnlockMagick(MagickType iMagick)
    {
      if (this.mMagickType.Value == iMagick)
        return;
      this.mMagickType.NewValue = iMagick;
      this.mMagickType.Dirty = true;
    }

    public void SetUnlockMagickTime(float iTime)
    {
      if (this.mMagickTime.Value == (int) iTime)
        return;
      this.mMagickTime.NewValue = (int) iTime;
      this.mMagickTime.Dirty = true;
    }

    public void SetCountDown(float iTime)
    {
      if (this.mCountDown.Value != (int) iTime)
      {
        this.mCountDown.NewValue = (int) iTime;
        this.mCountDown.Dirty = true;
      }
      this.mCountDownFloat = iTime - (float) this.mCountDown.NewValue;
    }

    public void SetTimeText(int iTime)
    {
      if (this.mTime.Value == iTime)
        return;
      this.mTime.NewValue = iTime;
      this.mTime.Dirty = true;
    }

    public void SuddenDeath(bool iBool)
    {
      if (this.mSuddenDeath.Value == iBool)
        return;
      this.mSuddenDeath.NewValue = iBool;
      this.mSuddenDeath.Dirty = true;
    }

    public bool GameOver
    {
      get => this.mGameOver.Value;
      set
      {
        if (this.mGameOver.Value == value)
          return;
        this.mGameOver.NewValue = value;
        this.mGameOver.Dirty = true;
      }
    }

    public void Draw(float iDeltaTime)
    {
      if (this.mCountDown.Dirty)
      {
        this.mCountDown.Value = this.mCountDown.NewValue;
        this.mCountDown.Dirty = false;
      }
      if (this.mTime.Dirty)
      {
        this.mTime.Value = this.mTime.NewValue;
        this.mTime.Dirty = false;
        this.mTimeText.SetText(this.mTime.Value.ToString());
      }
      if (this.mSuddenDeath.Dirty)
      {
        this.mSuddenDeath.Value = this.mSuddenDeath.NewValue;
        this.mSuddenDeath.Dirty = false;
        if (this.mSuddenDeath.Value)
        {
          this.mMagickText.SetText(LanguageManager.Instance.GetString(VersusRuleset.RenderData.LOC_SUDDEN_DEATH));
          this.mMagickType.Value = MagickType.None;
        }
      }
      if (this.mMagickType.Dirty)
      {
        this.mMagickType.Value = this.mMagickType.NewValue;
        this.mMagickType.Dirty = false;
        this.mMagickText.SetText(LanguageManager.Instance.GetString(Magick.NAME_LOCALIZATION[(int) this.mMagickType.Value]));
      }
      if (this.mMagickTime.Dirty)
      {
        this.mMagickTime.Value = this.mMagickTime.NewValue;
        this.mMagickTime.Dirty = false;
        this.mMagickTimeText.SetText(this.mMagickTime.Value.ToString());
        this.mMagickWidth = this.mMagickTypeFont.MeasureText(this.mMagickText.Characters, true).X;
      }
      if (this.mGameOver.Dirty)
      {
        this.mGameOver.Dirty = false;
        this.mGameOver.Value = this.mGameOver.NewValue;
      }
      Point screenSize = RenderManager.Instance.ScreenSize;
      Matrix matrix;
      if (this.DrawTime)
      {
        this.mEffect.GraphicsDevice.Vertices[0].SetSource(VersusRuleset.RenderData.sVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
        this.mEffect.GraphicsDevice.VertexDeclaration = VersusRuleset.RenderData.sVertexDeclaration;
        this.mEffect.VertexColorEnabled = false;
        this.mEffect.SetScreenSize(screenSize.X, screenSize.Y);
        this.mEffect.Transform = new Matrix(94f, 0.0f, 0.0f, 0.0f, 0.0f, 94f, 0.0f, 0.0f, 0.0f, 0.0f, 1f, 0.0f, (float) ((double) screenSize.X * 0.5 - 47.0), 6f, 0.0f, 1f);
        this.mEffect.Texture = (Texture) VersusRuleset.RenderData.sTexture;
        this.mEffect.TextureOffset = new Vector2(384f / (float) VersusRuleset.RenderData.sTexture.Width, 128f / (float) VersusRuleset.RenderData.sTexture.Height);
        this.mEffect.TextureScale = new Vector2(94f / (float) VersusRuleset.RenderData.sTexture.Width, 94f / (float) VersusRuleset.RenderData.sTexture.Height);
        this.mEffect.TextureEnabled = true;
        this.mEffect.Begin();
        this.mEffect.CurrentTechnique.Passes[0].Begin();
        this.mEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
        this.mEffect.CurrentTechnique.Passes[0].End();
        this.mEffect.End();
        this.mPieEffect.GraphicsDevice.RenderState.CullMode = CullMode.None;
        this.mPieEffect.GraphicsDevice.Vertices[0].SetSource(VersusRuleset.RenderData.sPieVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
        this.mPieEffect.GraphicsDevice.VertexDeclaration = VersusRuleset.RenderData.sPieVertexDeclaration;
        this.mPieEffect.TextureOffset = VersusRuleset.RenderData.sPieOffset;
        matrix = Matrix.Identity with
        {
          M41 = (float) screenSize.X * 0.5f,
          M42 = 54f,
          M11 = 0.0f,
          M12 = -1.5f,
          M21 = -1.5f,
          M22 = 0.0f,
          M33 = -1.5f
        };
        this.mPieEffect.Transform = matrix;
        this.mPieEffect.MaxAngle = (float) (6.2831854820251465 * ((double) this.Time / (double) this.TimeLimit));
        this.mPieEffect.Begin();
        this.mPieEffect.CurrentTechnique.Passes[0].Begin();
        this.mPieEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 31 /*0x1F*/);
        this.mPieEffect.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
        this.mPieEffect.CurrentTechnique.Passes[0].End();
        this.mPieEffect.End();
        this.mEffect.GraphicsDevice.Vertices[0].SetSource(VersusRuleset.RenderData.sVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
        this.mEffect.GraphicsDevice.VertexDeclaration = VersusRuleset.RenderData.sVertexDeclaration;
        this.mEffect.VertexColorEnabled = false;
        this.mEffect.SetScreenSize(screenSize.X, screenSize.Y);
        matrix = new Matrix(106f, 0.0f, 0.0f, 0.0f, 0.0f, 106f, 0.0f, 0.0f, 0.0f, 0.0f, 1f, 0.0f, (float) ((double) screenSize.X * 0.5 - 53.0), 0.0f, 0.0f, 1f);
        this.mEffect.Transform = matrix;
        this.mEffect.Texture = (Texture) VersusRuleset.RenderData.sTexture;
        this.mEffect.TextureOffset = new Vector2(0.5f, 128f / (float) VersusRuleset.RenderData.sTexture.Height);
        this.mEffect.TextureScale = new Vector2(106f / (float) VersusRuleset.RenderData.sTexture.Width, 106f / (float) VersusRuleset.RenderData.sTexture.Height);
        this.mEffect.TextureEnabled = true;
        this.mEffect.Begin();
        this.mEffect.CurrentTechnique.Passes[0].Begin();
        this.mEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
        this.mEffect.CurrentTechnique.Passes[0].End();
        this.mEffect.End();
      }
      this.mEffect.SetScreenSize(screenSize.X, screenSize.Y);
      this.mEffect.Begin();
      this.mEffect.CurrentTechnique.Passes[0].Begin();
      float iX = (float) screenSize.X * 0.5f;
      if (this.mCountDown.Value >= 0)
      {
        this.mEffect.GraphicsDevice.Vertices[0].SetSource(VersusRuleset.RenderData.sVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
        this.mEffect.GraphicsDevice.VertexDeclaration = VersusRuleset.RenderData.sVertexDeclaration;
        this.mEffect.VertexColorEnabled = false;
        this.mEffect.Texture = (Texture) this.mCountdownTexture;
        this.mEffect.TextureEnabled = true;
        this.mEffect.TextureScale = new Vector2(0.25f, 1f);
        this.mEffect.TextureOffset = new Vector2((float) this.mCountDown.Value / 4f, 0.0f);
        float num1 = 256f;
        float num2 = 0.0f;
        float y = 1f - this.mCountDownFloat;
        this.mCountDownScale = (float) ((2.0 * Math.Pow(1.0 / 1000.0, (double) y) + 1.0) * (((1.0 - Math.Pow(9.9999997473787516E-05, (double) y)) * 1.0 + 0.0) / 2.0));
        float num3;
        if (this.mCountDown.Value <= 0)
        {
          num3 = num1 * (this.mCountDownScale * 1.5f);
          num2 = 16f;
        }
        else
          num3 = num1 * this.mCountDownScale;
        matrix = new Matrix(num3, 0.0f, 0.0f, 0.0f, 0.0f, num3, 0.0f, 0.0f, 0.0f, 0.0f, 1f, 0.0f, (float) ((double) screenSize.X * 0.5 - (double) num3 * 0.5) + num2, (float) ((double) screenSize.Y * 0.5 - (double) num3 * 0.5), 0.0f, 1f);
        this.mEffect.Transform = matrix;
        this.mEffect.CommitChanges();
        this.mEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
      }
      if (this.mGameOver.Value)
        this.mGameOverText.Draw(this.mEffect, iX, (float) screenSize.Y * 0.35f);
      if (this.mTime.Value > 0)
        this.mTimeText.Draw(this.mEffect, iX + 1f, (float) (54.0 - (double) this.mTimeFontHeight * 0.5));
      float num4 = 0.0f;
      float num5 = 0.0f;
      for (int index = 0; index < this.mScores.Count; ++index)
      {
        if (this.mScores[index].LeftAligned)
        {
          this.mScores[index].Draw(this.mEffect, (float) ((double) iX - (double) num4 - 48.0), 0.0f);
          num4 += this.mScores[index].Width + 16f;
        }
        else
        {
          this.mScores[index].Draw(this.mEffect, (float) ((double) iX + (double) num5 + 48.0), 0.0f);
          num5 += this.mScores[index].Width + 16f;
        }
      }
      if (this.mMagickType.Value != MagickType.None && (double) this.mMagickTime.Value > 0.0)
      {
        this.mMagickText.Draw(this.mEffect, iX, 96f);
        this.mMagickTimeText.Draw(this.mEffect, (float) ((double) iX + (double) this.mMagickWidth * 0.5 + 8.0), 96f);
      }
      if (this.mSuddenDeath.Value)
        this.mMagickText.Draw(this.mEffect, iX, 96f);
      this.mEffect.CurrentTechnique.Passes[0].End();
      this.mEffect.End();
    }

    public int ZIndex => 100;
  }

  public enum MessageTypes : byte
  {
    Time,
    End,
    SuddenDeath,
    Setup,
    Score,
  }
}
