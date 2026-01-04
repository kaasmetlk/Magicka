// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Level
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.UI;
using Magicka.Levels.Versus;
using Magicka.Network;
using Magicka.Physics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using PolygonHead.Lights;
using PolygonHead.ParticleEffects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Xml;

#nullable disable
namespace Magicka.Levels;

public class Level : IDisposable
{
  private string mName;
  private Dictionary<int, int> mCounters = new Dictionary<int, int>(32 /*0x20*/);
  private List<Level.TimerBody> mTimers = new List<Level.TimerBody>();
  private List<Level.TimerBody> mPlayedTimers = new List<Level.TimerBody>();
  private PlayState mPlayState;
  private Dictionary<int, GameScene> mScenes = new Dictionary<int, GameScene>();
  private GameScene mCurrentScene;
  private SpawnPoint mSpawnPoint;
  private GameScene mNextScene;
  private Transitions mNextSceneTransition;
  private float mNextSceneTransitionTime;
  private bool mNextSceneSaveNPCs;
  private Action mNextSceneDoneCallback;
  private Action mOnComplete;
  private SpawnPoint mNextSceneSpawnPoint;
  private bool mNoItems;
  private bool mForceCamera;
  private bool mSpawnFairy = true;
  private bool mBusy;
  private Level.AvatarItem[] mAdditionalAvatarItems;
  private DialogCollection mDialogs;
  private int mDescription;
  private int mDisplayName;
  private byte[] mShaHash;
  private byte[] mDialogHash;
  private TitleRenderer mTitleRenderer;
  private bool mHasTitleDisplaying;
  private VersusRuleset.Settings mSettings;
  private Action<float> gotoSceneLoadingDelegate;

  internal unsafe Level(
    string iFileName,
    XmlDocument iInput,
    PlayState iPlayState,
    SpawnPoint? iSpawnPoint,
    VersusRuleset.Settings iSettings)
  {
    this.mSettings = iSettings;
    SHA256 shA256 = SHA256.Create();
    try
    {
      using (FileStream inputStream = File.OpenRead(iFileName))
        this.mShaHash = shA256.ComputeHash((Stream) inputStream);
    }
    catch (Exception ex)
    {
      this.mShaHash = new byte[32 /*0x20*/];
    }
    this.mPlayState = iPlayState;
    this.mSpawnPoint.SpawnPlayers = true;
    if (iPlayState.GameType == GameType.Campaign | iPlayState.GameType == GameType.Mythos)
      this.mForceCamera = true;
    string directoryName = Path.GetDirectoryName(iFileName);
    this.mName = Path.GetFileNameWithoutExtension(iFileName);
    XmlNode xmlNode = (XmlNode) null;
    for (int i = 0; i < iInput.ChildNodes.Count; ++i)
    {
      if (iInput.ChildNodes[i].Name.Equals(nameof (Level), StringComparison.OrdinalIgnoreCase))
      {
        xmlNode = iInput.ChildNodes[i];
        break;
      }
    }
    if (xmlNode == null)
      throw new Exception("No Level node found in level XML!");
    List<Level.AvatarItem> avatarItemList = new List<Level.AvatarItem>();
    if (iSpawnPoint.HasValue)
      this.mSpawnPoint = iSpawnPoint.Value;
    for (int i1 = 0; i1 < xmlNode.ChildNodes.Count; ++i1)
    {
      XmlNode childNode1 = xmlNode.ChildNodes[i1];
      if (childNode1.Name.Equals(nameof (Name), StringComparison.OrdinalIgnoreCase))
        this.mDisplayName = childNode1.InnerText.ToLowerInvariant().GetHashCodeCustom();
      else if (childNode1.Name.Equals(nameof (Description), StringComparison.OrdinalIgnoreCase))
        this.mDescription = childNode1.InnerText.ToLowerInvariant().GetHashCodeCustom();
      else if (childNode1.Name.Equals("Start", StringComparison.OrdinalIgnoreCase) & !iSpawnPoint.HasValue)
      {
        for (int i2 = 0; i2 < childNode1.Attributes.Count; ++i2)
        {
          XmlAttribute attribute = childNode1.Attributes[i2];
          if (attribute.Name.Equals("Scene", StringComparison.OrdinalIgnoreCase))
            this.mSpawnPoint.Scene = attribute.Value.ToLowerInvariant().GetHashCodeCustom();
          else if (attribute.Name.Equals("Area", StringComparison.OrdinalIgnoreCase))
          {
            fixed (int* numPtr = this.mSpawnPoint.Locations)
            {
              for (int index = 0; index < 4; ++index)
                numPtr[index] = (attribute.Value + (object) index).GetHashCodeCustom();
            }
          }
          else if (attribute.Name.Equals("SpawnPlayers", StringComparison.OrdinalIgnoreCase))
            this.mSpawnPoint.SpawnPlayers = bool.Parse(attribute.Value);
          else if (attribute.Name.Equals(nameof (NoItems), StringComparison.OrdinalIgnoreCase))
            this.mNoItems = bool.Parse(attribute.Value);
          else if (attribute.Name.Equals(nameof (ForceCamera), StringComparison.OrdinalIgnoreCase))
            this.mForceCamera = bool.Parse(attribute.Value);
          else if (attribute.Name.Equals(nameof (SpawnFairy), StringComparison.OrdinalIgnoreCase))
            this.mSpawnFairy = bool.Parse(attribute.Value);
        }
      }
      else if (childNode1.Name.Equals("Dialogs", StringComparison.OrdinalIgnoreCase))
      {
        string str = Path.Combine(directoryName, childNode1.InnerText);
        this.mDialogs = new DialogCollection(str);
        try
        {
          using (FileStream inputStream = File.OpenRead(str))
            this.mDialogHash = shA256.ComputeHash((Stream) inputStream);
        }
        catch (Exception ex)
        {
          this.mDialogHash = new byte[32 /*0x20*/];
        }
      }
      else if (childNode1.Name.Equals(nameof (Scenes), StringComparison.OrdinalIgnoreCase))
        this.ReadScenes(childNode1, directoryName, iPlayState.Content);
      else if (childNode1.Name.Equals("AdditionalItems", StringComparison.OrdinalIgnoreCase))
      {
        foreach (XmlNode childNode2 in childNode1.ChildNodes)
        {
          if (!(childNode2 is XmlComment) && childNode2.Name.Equals("Item", StringComparison.OrdinalIgnoreCase))
          {
            Level.AvatarItem avatarItem = new Level.AvatarItem();
            foreach (XmlAttribute attribute in (XmlNamedNodeMap) childNode2.Attributes)
            {
              if (attribute.Name.Equals("item", StringComparison.OrdinalIgnoreCase))
                avatarItem.Item = iPlayState.Content.Load<Item>(attribute.Value);
              else if (attribute.Name.Equals("Bone", StringComparison.OrdinalIgnoreCase))
                avatarItem.Bone = attribute.Value;
            }
            if (avatarItem.Item != null && !string.IsNullOrEmpty(avatarItem.Bone))
              avatarItemList.Add(avatarItem);
          }
        }
      }
    }
    if (this.mDialogs != null)
      this.mDialogs.Initialize(iPlayState.Scene);
    else
      this.mDialogHash = new byte[32 /*0x20*/];
    this.mAdditionalAvatarItems = avatarItemList.ToArray();
    this.mHasTitleDisplaying = false;
  }

  public byte[] ShaHash => this.mShaHash;

  public byte[] DialogHash => this.mDialogHash;

  internal Level.AvatarItem[] AdditionalAvatarItems => this.mAdditionalAvatarItems;

  private void ReadScenes(XmlNode iNode, string iRelativePath, ContentManager iContent)
  {
    for (int i = 0; i < iNode.ChildNodes.Count; ++i)
    {
      XmlNode childNode = iNode.ChildNodes[i];
      if (!(childNode is XmlComment))
      {
        if (!childNode.Name.Equals("Scene", StringComparison.OrdinalIgnoreCase))
          throw new Exception("Invalid XML node in level file! Node name: " + iNode.Name);
        XmlDocument iInput = new XmlDocument();
        string str = Path.Combine(iRelativePath, childNode.InnerText);
        iInput.Load(str);
        GameScene gameScene = new GameScene(this, str, iInput, iContent, this.mSettings);
        this.mScenes.Add(gameScene.ID, gameScene);
      }
    }
  }

  public void Initialize() => DialogManager.Instance.SetDialogs(this.mDialogs);

  public GameScene CurrentScene => this.mCurrentScene;

  public string Name => this.mName;

  public bool ForceCamera => this.mForceCamera;

  public bool SpawnFairy => this.mSpawnFairy;

  public IEnumerable<GameScene> Scenes => (IEnumerable<GameScene>) this.mScenes.Values;

  public void UpdateAnimatedLevelParts(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mCurrentScene.UpdateAnimatedLevelParts(iDataChannel, iDeltaTime);
  }

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if (!this.mPlayState.IsPaused && !this.PlayState.IsGameEnded)
    {
      if (this.mHasTitleDisplaying)
      {
        TitleRenderData iObject = this.mTitleRenderer.Update((int) iDataChannel, iDeltaTime);
        if (iObject == null)
          this.mHasTitleDisplaying = false;
        else
          this.mPlayState.Scene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) iObject);
      }
      if (!this.PlayState.IsInCutscene)
      {
        for (int index = 0; index < this.mPlayedTimers.Count; ++index)
        {
          Level.TimerBody mPlayedTimer = this.mPlayedTimers[index];
          if (!mPlayedTimer.Paused)
          {
            mPlayedTimer.Time += iDeltaTime;
            this.mPlayedTimers[index] = mPlayedTimer;
          }
        }
      }
    }
    for (int index = 0; index < this.mTimers.Count; ++index)
    {
      Level.TimerBody mTimer = this.mTimers[index];
      if (!mTimer.Paused)
      {
        mTimer.Time += iDeltaTime;
        this.mTimers[index] = mTimer;
      }
    }
    this.mCurrentScene.Update(iDataChannel, iDeltaTime);
  }

  public PlayState PlayState => this.mPlayState;

  public SpawnPoint SpawnPoint => this.mSpawnPoint;

  public SpawnPoint NextSceneSpawnPoint => this.mNextSceneSpawnPoint;

  public int GetCounterValue(int iID)
  {
    int num;
    return this.mCounters.TryGetValue(iID, out num) ? num : 0;
  }

  public void SetCounterValue(int iID, int iValue) => this.mCounters[iID] = iValue;

  public void AddToCounter(int iID, int iValue)
  {
    int num;
    if (!this.mCounters.TryGetValue(iID, out num))
      num = 0;
    this.mCounters[iID] = num + iValue;
  }

  public void AddTimer(int iID, bool iRealTime, float iValue)
  {
    if (iRealTime)
    {
      for (int index = 0; index < this.mPlayedTimers.Count; ++index)
      {
        if (this.mPlayedTimers[index].ID == iID)
          return;
      }
      this.mPlayedTimers.Add(new Level.TimerBody(iID, iValue, false));
    }
    else
    {
      for (int index = 0; index < this.mTimers.Count; ++index)
      {
        if (this.mTimers[index].ID == iID)
          return;
      }
      this.mTimers.Add(new Level.TimerBody(iID, iValue, false));
    }
  }

  public void PauseTimer(int iID, bool iPause)
  {
    for (int index = 0; index < this.mPlayedTimers.Count; ++index)
    {
      if (this.mPlayedTimers[index].ID == iID)
      {
        Level.TimerBody mPlayedTimer = this.mPlayedTimers[index] with
        {
          Paused = iPause
        };
        this.mPlayedTimers[index] = mPlayedTimer;
        return;
      }
    }
    for (int index = 0; index < this.mTimers.Count; ++index)
    {
      if (this.mTimers[index].ID == iID)
      {
        Level.TimerBody mTimer = this.mTimers[index] with
        {
          Paused = iPause
        };
        this.mTimers[index] = mTimer;
        break;
      }
    }
  }

  public void SetTimer(int iID, float iValue)
  {
    for (int index = 0; index < this.mPlayedTimers.Count; ++index)
    {
      if (this.mPlayedTimers[index].ID == iID)
      {
        Level.TimerBody mPlayedTimer = this.mPlayedTimers[index] with
        {
          Time = iValue
        };
        this.mPlayedTimers[index] = mPlayedTimer;
        return;
      }
    }
    for (int index = 0; index < this.mTimers.Count; ++index)
    {
      if (this.mTimers[index].ID == iID)
      {
        Level.TimerBody mTimer = this.mTimers[index] with
        {
          Time = iValue
        };
        this.mTimers[index] = mTimer;
        break;
      }
    }
  }

  public float GetTimerValue(int iID)
  {
    for (int index = 0; index < this.mPlayedTimers.Count; ++index)
    {
      if (this.mPlayedTimers[index].ID == iID)
        return this.mPlayedTimers[index].Time;
    }
    for (int index = 0; index < this.mTimers.Count; ++index)
    {
      if (this.mTimers[index].ID == iID)
        return this.mTimers[index].Time;
    }
    return 0.0f;
  }

  public void GoToScene(
    SpawnPoint iSpawnPoint,
    Transitions iTransition,
    float iTransitionTime,
    bool iSaveNPCs,
    Action iOnComplete)
  {
    this.GoToScene(iSpawnPoint, iTransition, iTransitionTime, iSaveNPCs, iOnComplete, (Action<float>) null);
  }

  public void GoToScene(
    SpawnPoint iSpawnPoint,
    Transitions iTransition,
    float iTransitionTime,
    bool iSaveNPCs,
    Action iOnComplete,
    Action<float> reportProgressAction)
  {
    this.gotoSceneLoadingDelegate = reportProgressAction;
    this.ClearDisplayTitles();
    this.mNextScene = this.mScenes[iSpawnPoint.Scene];
    this.mNextSceneTransition = iTransition;
    this.mNextSceneTransitionTime = iTransitionTime;
    this.mNextSceneSaveNPCs = iSaveNPCs;
    this.mNextSceneDoneCallback = (Action) null;
    this.mOnComplete = iOnComplete;
    this.mNextSceneSpawnPoint = iSpawnPoint;
    if (this.mCurrentScene == null)
    {
      this.mBusy = true;
      if (this.mNoItems)
      {
        Magicka.GameLogic.Player[] players = Game.Instance.Players;
        Item obj = this.PlayState.Content.Load<Item>("Data/Items/Wizard/weapon_unarmed");
        for (int index = 0; index < players.Length; ++index)
        {
          if (players[index].Playing && players[index].Avatar != null)
          {
            Avatar avatar = players[index].Avatar;
            obj.Copy(avatar.Equipment[1].Item);
            obj.Copy(avatar.Equipment[0].Item);
            players[index].Weapon = "weapon_unarmed";
            players[index].Staff = "weapon_unarmed";
          }
        }
      }
      this.ChangeScene();
    }
    else
    {
      if (iTransition == Transitions.None || (double) iTransitionTime < 1.4012984643248171E-45)
      {
        this.mBusy = true;
        Game.Instance.AddLoadTask(new Action(this.ChangeScene));
      }
      else
      {
        RenderManager.Instance.BeginTransition(iTransition, Color.Black, iTransitionTime);
        RenderManager.Instance.TransitionEnd += new TransitionEnd(this.TransitionFinish);
      }
      AudioManager.Instance.TargetReverbMix = 0.0f;
    }
  }

  private void TransitionFinish(TransitionEffect iOldEffect)
  {
    RenderManager.Instance.TransitionEnd -= new TransitionEnd(this.TransitionFinish);
    this.mBusy = true;
    Game.Instance.AddLoadTask(new Action(this.ChangeScene));
  }

  private void ChangeScene()
  {
    this.mBusy = true;
    while (!this.mPlayState.Busy)
      Thread.Sleep(100);
    if (this.gotoSceneLoadingDelegate == null && this.mNextScene != null && this.mNextScene.GetNumStartupActions() > 300)
      this.mPlayState.HookupLoadingScreen(out this.gotoSceneLoadingDelegate, out this.mNextSceneDoneCallback, 0.0f, 1f);
    this.mNextSceneDoneCallback += this.mOnComplete;
    if (this.PlayState.BossFight != null)
      this.PlayState.BossFight.Reset();
    if (this.mPlayState.GenericHealthBar != null)
      this.mPlayState.GenericHealthBar.Reset();
    GameScene mCurrentScene = this.mCurrentScene;
    mCurrentScene?.Destroy(this.mNextSceneSaveNPCs);
    this.mCurrentScene = this.mNextScene;
    this.mNextScene.LoadLevel();
    if (this.mCurrentScene != mCurrentScene && mCurrentScene != null)
      mCurrentScene.UnloadContent();
    this.mNextScene.Initialize(this.mNextSceneSpawnPoint, false, this.gotoSceneLoadingDelegate);
    AudioManager instance = AudioManager.Instance;
    instance.TargetReverbMix = this.mNextScene.ReverbMix;
    this.PlayState.ChangingScene();
    instance.SetRoomType(this.mNextScene.RoomType);
    Thread.Sleep(0);
    if (this.mPlayState.Initialized)
    {
      NetworkServer networkServer = NetworkManager.Instance.Interface as NetworkServer;
      NetworkClient networkClient = NetworkManager.Instance.Interface as NetworkClient;
      if (networkServer != null)
      {
        while (!networkServer.AllClientsReady)
          Thread.Sleep(100);
        GameEndLoadMessage iMessage = new GameEndLoadMessage();
        networkServer.SendMessage<GameEndLoadMessage>(ref iMessage);
        networkServer.SetAllClientsBusy();
      }
      else if (networkClient != null)
      {
        GameEndLoadMessage iMessage = new GameEndLoadMessage();
        networkClient.SendMessage<GameEndLoadMessage>(ref iMessage, 0);
        while (PlayState.WaitingForPlayers)
          Thread.Sleep(100);
        PlayState.WaitingForPlayers = true;
      }
      if (this.mNextSceneDoneCallback != null)
      {
        this.mNextSceneDoneCallback();
        this.mNextSceneDoneCallback = (Action) null;
      }
      PhysicsManager.Instance.Update(0.0f);
      this.mCurrentScene.Update(DataChannel.None, 0.0f);
      Thread.Sleep(0);
      if (networkServer != null)
      {
        while (!networkServer.AllClientsReady)
          Thread.Sleep(100);
        GameEndLoadMessage iMessage = new GameEndLoadMessage();
        networkServer.SendMessage<GameEndLoadMessage>(ref iMessage);
        networkServer.SetAllClientsBusy();
      }
      else if (networkClient != null)
      {
        GameEndLoadMessage iMessage = new GameEndLoadMessage();
        networkClient.SendMessage<GameEndLoadMessage>(ref iMessage, 0);
        while (PlayState.WaitingForPlayers)
          Thread.Sleep(100);
        PlayState.WaitingForPlayers = true;
      }
    }
    if (this.mNextSceneTransition != Transitions.Invalid)
      RenderManager.Instance.EndTransition(this.mNextSceneTransition, Color.Black, this.mNextSceneTransitionTime);
    if (this.mNextSceneTransition != Transitions.Invalid)
    {
      StaticList<Entity> entities = this.PlayState.EntityManager.Entities;
      if (entities != null && entities.Count > 0)
      {
        for (int iIndex = 0; iIndex < entities.Count; ++iIndex)
        {
          if (entities[iIndex] is Magicka.GameLogic.Entities.Character iOwner && iOwner is Avatar && iOwner is Avatar avatar && avatar.Player != null)
          {
            StaticList<Spell> spellQueue = avatar.Player.SpellQueue;
            if (spellQueue != null && spellQueue.Count > 0)
            {
              foreach (Spell spell in spellQueue)
              {
                ChantSpells iChantSpell = new ChantSpells(spell.Element, iOwner);
                ChantSpellManager.Add(ref iChantSpell);
              }
            }
          }
        }
      }
    }
    this.mBusy = false;
  }

  public void Dispose()
  {
    foreach (GameScene gameScene in this.mScenes.Values)
      gameScene.Dispose();
    this.mHasTitleDisplaying = false;
    if (this.mTitleRenderer != null)
      this.mTitleRenderer.Clear();
    this.mTitleRenderer = (TitleRenderer) null;
    this.mScenes = (Dictionary<int, GameScene>) null;
    this.mCurrentScene = (GameScene) null;
    this.mNextScene = (GameScene) null;
    this.mDialogs = (DialogCollection) null;
  }

  public bool Busy => this.mBusy;

  internal GameScene GetScene(int iScene)
  {
    return this.mScenes == null || !this.mScenes.ContainsKey(iScene) ? (GameScene) null : this.mScenes[iScene];
  }

  public bool NoItems => this.mNoItems;

  public int DisplayName => this.mDisplayName;

  public int Description => this.mDescription;

  internal void ClearTransition()
  {
    this.mPlayState.IgnoreInitFade = true;
    this.mNextSceneTransition = Transitions.Invalid;
  }

  public void DisplayTitle(
    string iTitle,
    string iSubTitle,
    float iDisplayTime,
    float iFadeIn,
    float iFadeOut,
    TextAlign iTextAlignment)
  {
    if (this.mHasTitleDisplaying)
      return;
    if (this.mTitleRenderer != null)
      this.mTitleRenderer.Clear();
    this.mTitleRenderer = new TitleRenderer(iTextAlignment);
    this.mTitleRenderer.SetTitles(iTitle, iSubTitle, iDisplayTime, iFadeIn, iFadeOut);
    this.mTitleRenderer.Start();
    this.mHasTitleDisplaying = true;
  }

  public void ClearDisplayTitles()
  {
    this.mHasTitleDisplaying = false;
    if (this.mTitleRenderer == null)
      return;
    this.mTitleRenderer.Clear();
  }

  public class State
  {
    private Level mLevel;
    private Dictionary<int, int> mCounters;
    private List<Level.TimerBody> mTimers;
    private List<Level.TimerBody> mPlayedTimers;
    private GameScene.State[] mInactiveScenes;
    private GameScene mCurrentScene;
    private GameScene mNextScene;
    public Action<float> ProgressReportBackAction;

    public State(Level iLevel)
    {
      this.mLevel = iLevel;
      this.mCounters = new Dictionary<int, int>(10);
      this.mTimers = new List<Level.TimerBody>();
      this.mPlayedTimers = new List<Level.TimerBody>();
      this.mInactiveScenes = new GameScene.State[this.mLevel.mScenes.Count];
      int num = 0;
      foreach (GameScene iScene in this.mLevel.mScenes.Values)
        this.mInactiveScenes[num++] = new GameScene.State(iScene);
      this.UpdateState();
    }

    public void UpdateState()
    {
      this.mCurrentScene = this.mLevel.mCurrentScene;
      this.mNextScene = this.mLevel.mNextScene;
      this.mTimers.Clear();
      for (int index = 0; index < this.mLevel.mTimers.Count; ++index)
        this.mTimers.Add(this.mLevel.mTimers[index]);
      this.mPlayedTimers.Clear();
      for (int index = 0; index < this.mLevel.mPlayedTimers.Count; ++index)
        this.mPlayedTimers.Add(this.mLevel.mPlayedTimers[index]);
      this.mCounters.Clear();
      foreach (KeyValuePair<int, int> mCounter in this.mLevel.mCounters)
        this.mCounters[mCounter.Key] = mCounter.Value;
      for (int index = 0; index < this.mInactiveScenes.Length; ++index)
        this.mInactiveScenes[index].UpdateState();
    }

    public void ApplyState(List<int> iIgnoredTriggers)
    {
      this.ApplyState(iIgnoredTriggers, (Action<float>) null);
    }

    public void ApplyState(List<int> iIgnoredTriggers, Action<float> reportBack)
    {
      this.mLevel.mBusy = true;
      while (!this.mLevel.mPlayState.Busy)
        Thread.Sleep(1);
      GameScene mCurrentScene = this.mLevel.mCurrentScene;
      if (this.mLevel.mCurrentScene != this.mCurrentScene && mCurrentScene != null)
        mCurrentScene.Destroy(false);
      foreach (GameScene gameScene in this.mLevel.mScenes.Values)
      {
        if (gameScene.RuleSet != null)
          gameScene.RuleSet.Initialize();
      }
      for (int index = 0; index < this.mInactiveScenes.Length; ++index)
      {
        if (this.mInactiveScenes[index].Scene == this.mCurrentScene)
          this.mInactiveScenes[index].ApplyState(iIgnoredTriggers);
        else
          this.mInactiveScenes[index].ApplyState((List<int>) null);
      }
      if (this.mLevel.mCurrentScene != this.mCurrentScene)
      {
        this.mLevel.mCurrentScene = this.mCurrentScene;
        if (this.mLevel.mCurrentScene != null)
          this.mLevel.mCurrentScene.LoadLevel();
        mCurrentScene?.UnloadContent();
        SpawnPoint iSpawnPoint = new SpawnPoint();
        if (this.mLevel.mCurrentScene != null)
          this.mLevel.mCurrentScene.Initialize(iSpawnPoint, true, reportBack);
      }
      else
      {
        ParticleSystem.Instance.Clear();
        ParticleLightBatcher.Instance.Clear();
        PointLightBatcher.Instance.Clear();
        this.mLevel.mCurrentScene.RestoreSavedAnimations();
        this.mLevel.mCurrentScene.RestoreDynamicLights();
        this.mLevel.mCurrentScene.AddSavedEntities();
        this.mLevel.mCurrentScene.RunStartupActions(reportBack);
      }
      reportBack = (Action<float>) null;
      this.mLevel.mNextScene = this.mNextScene;
      this.mLevel.mTimers.Clear();
      for (int index = 0; index < this.mTimers.Count; ++index)
        this.mLevel.mTimers.Add(this.mTimers[index]);
      this.mLevel.mPlayedTimers.Clear();
      for (int index = 0; index < this.mPlayedTimers.Count; ++index)
        this.mLevel.mPlayedTimers.Add(this.mPlayedTimers[index]);
      this.mLevel.mCounters.Clear();
      foreach (KeyValuePair<int, int> mCounter in this.mCounters)
        this.mLevel.mCounters[mCounter.Key] = mCounter.Value;
      if (NetworkManager.Instance.State != NetworkState.Offline)
        NetworkManager.Instance.Interface.Sync();
      PhysicsManager.Instance.Update(0.0f);
      this.mLevel.mBusy = false;
    }

    internal void Write(BinaryWriter iWriter)
    {
      iWriter.Write(this.mCurrentScene != null ? this.mCurrentScene.ID : 0);
      iWriter.Write(this.mNextScene != null ? this.mNextScene.ID : 0);
      iWriter.Write(this.mTimers.Count);
      foreach (Level.TimerBody mTimer in this.mTimers)
        mTimer.Write(iWriter);
      iWriter.Write(this.mPlayedTimers.Count);
      foreach (Level.TimerBody mPlayedTimer in this.mPlayedTimers)
        mPlayedTimer.Write(iWriter);
      iWriter.Write(this.mCounters.Count);
      foreach (KeyValuePair<int, int> mCounter in this.mCounters)
      {
        iWriter.Write(mCounter.Key);
        iWriter.Write(mCounter.Value);
      }
      iWriter.Write(this.mInactiveScenes.Length);
      for (int index = 0; index < this.mInactiveScenes.Length; ++index)
      {
        iWriter.Write(this.mInactiveScenes[index].Scene.ID);
        this.mInactiveScenes[index].Write(iWriter);
      }
    }

    internal void Read(BinaryReader iReader)
    {
      int iScene1 = iReader.ReadInt32();
      this.mCurrentScene = iScene1 == 0 ? (GameScene) null : this.mLevel.GetScene(iScene1);
      int iScene2 = iReader.ReadInt32();
      this.mNextScene = iScene2 == 0 ? (GameScene) null : this.mLevel.GetScene(iScene2);
      this.mTimers.Clear();
      int num1 = iReader.ReadInt32();
      for (int index = 0; index < num1; ++index)
        this.mTimers.Add(new Level.TimerBody(iReader));
      this.mPlayedTimers.Clear();
      int num2 = iReader.ReadInt32();
      for (int index = 0; index < num2; ++index)
        this.mPlayedTimers.Add(new Level.TimerBody(iReader));
      this.mCounters.Clear();
      int num3 = iReader.ReadInt32();
      for (int index = 0; index < num3; ++index)
        this.mCounters[iReader.ReadInt32()] = iReader.ReadInt32();
      int num4 = iReader.ReadInt32();
      for (int index = 0; index < num4; ++index)
      {
        if (iReader.ReadInt32() != this.mInactiveScenes[index].Scene.ID)
          throw new InvalidOperationException();
        this.mInactiveScenes[index].Read(iReader);
      }
    }
  }

  private struct TimerBody
  {
    public int ID;
    public float Time;
    public bool Paused;

    public TimerBody(int iID, float iTime, bool iPaused)
    {
      this.ID = iID;
      this.Time = iTime;
      this.Paused = iPaused;
    }

    public TimerBody(BinaryReader iReader)
    {
      this.ID = iReader.ReadInt32();
      this.Time = iReader.ReadSingle();
      this.Paused = iReader.ReadBoolean();
    }

    public void Write(BinaryWriter iWriter)
    {
      iWriter.Write(this.ID);
      iWriter.Write(this.Time);
      iWriter.Write(this.Paused);
    }
  }

  internal struct AvatarItem
  {
    public Item Item;
    public string Bone;
  }
}
