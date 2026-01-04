// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Trigger
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.Achievements;
using Magicka.AI;
using Magicka.Audio;
using Magicka.GameLogic;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.GameLogic.Spells;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Levels.Triggers.Conditions;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml;

#nullable disable
namespace Magicka.Levels.Triggers;

public class Trigger
{
  protected static Random sRandom = new Random();
  protected string mIDString;
  protected int mID;
  protected float mRepeat;
  protected float mTimeTillRepeat;
  protected bool mHasTriggered;
  protected Condition[][] mConditions;
  protected Magicka.Levels.Triggers.Actions.Action[][] mActions;
  protected bool mAutoRun = true;
  protected GameScene mGameScene;

  protected Trigger(GameScene iGameScene) => this.mGameScene = iGameScene;

  public virtual void Update(float iDeltaTime)
  {
    this.mTimeTillRepeat -= iDeltaTime;
    if (this.mAutoRun & NetworkManager.Instance.State != NetworkState.Client)
      this.Execute((Magicka.GameLogic.Entities.Character) null, false);
    for (int index = 0; index < this.mActions.Length; ++index)
    {
      foreach (Magicka.Levels.Triggers.Actions.Action action in this.mActions[index])
        action.Update(iDeltaTime);
    }
  }

  public virtual void Execute(Magicka.GameLogic.Entities.Character iSender, bool iIgnoreConditions)
  {
    if (((iIgnoreConditions | !this.mHasTriggered ? 1 : 0) | ((double) this.mRepeat <= -1.4012984643248171E-45 ? 0 : ((double) this.mTimeTillRepeat < 1.4012984643248171E-45 ? 1 : 0))) == 0)
      return;
    bool flag = true;
    for (int index1 = 0; index1 < this.mConditions.Length; ++index1)
    {
      flag = true;
      for (int index2 = 0; index2 < this.mConditions[index1].Length; ++index2)
      {
        if (!this.mConditions[index1][index2].IsMet(iSender))
        {
          flag = false;
          break;
        }
      }
      if (flag)
        break;
    }
    if (!(flag | iIgnoreConditions))
      return;
    this.mHasTriggered = true;
    this.mTimeTillRepeat = this.mRepeat;
    int index3 = 0;
    if (this.mActions.Length > 1)
      index3 = Trigger.sRandom.Next(this.mActions.Length);
    if (NetworkManager.Instance.State == NetworkState.Server)
      NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
      {
        ActionType = TriggerActionType.TriggerExecute,
        Id = this.mID,
        Scene = this.mGameScene.ID,
        Arg = index3,
        Handle = iSender == null ? ushort.MaxValue : iSender.Handle
      });
    for (int index4 = 0; index4 < this.mActions[index3].Length; ++index4)
      this.mActions[index3][index4].OnTrigger(iSender);
  }

  public void ClearDelayedActions()
  {
    for (int index = 0; index < this.mActions.Length; ++index)
    {
      foreach (Magicka.Levels.Triggers.Actions.Action action in this.mActions[index])
        action.ClearDelayed();
    }
  }

  public static Trigger Read(GameScene iScene, XmlNode iNode)
  {
    Trigger iTrigger = new Trigger(iScene);
    iTrigger.mID = 0;
    iTrigger.mRepeat = -1f;
    for (int i = 0; i < iNode.Attributes.Count; ++i)
    {
      XmlAttribute attribute = iNode.Attributes[i];
      if (attribute.Name.Equals("ID", StringComparison.OrdinalIgnoreCase))
      {
        iTrigger.mIDString = attribute.Value.ToLowerInvariant();
        iTrigger.mID = iTrigger.mIDString.GetHashCodeCustom();
      }
      else if (attribute.Name.Equals("Autorun", StringComparison.OrdinalIgnoreCase))
      {
        iTrigger.mAutoRun = bool.Parse(attribute.Value);
      }
      else
      {
        if (!attribute.Name.Equals("Repeat", StringComparison.OrdinalIgnoreCase))
          throw new NotImplementedException();
        try
        {
          iTrigger.mRepeat = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
        }
        catch (FormatException ex)
        {
          bool flag = bool.Parse(attribute.Value);
          iTrigger.mRepeat = flag ? 0.0f : -1f;
        }
      }
    }
    List<Condition[]> conditionArrayList = new List<Condition[]>();
    for (int i = 0; i < iNode.ChildNodes.Count; ++i)
    {
      XmlNode childNode = iNode.ChildNodes[i];
      if (childNode.Name.Equals("If", StringComparison.OrdinalIgnoreCase))
        conditionArrayList.Add(Trigger.ReadConditions(iScene, childNode));
      else if (childNode.Name.Equals("Then", StringComparison.OrdinalIgnoreCase))
        iTrigger.mActions = Trigger.ReadActions(iScene, iTrigger, childNode);
    }
    iTrigger.mConditions = conditionArrayList.ToArray();
    return iTrigger;
  }

  protected static Condition[] ReadConditions(GameScene iScene, XmlNode iNode)
  {
    List<Condition> conditionList = new List<Condition>();
    for (int i = 0; i < iNode.ChildNodes.Count; ++i)
    {
      XmlNode childNode = iNode.ChildNodes[i];
      if (!(childNode is XmlComment))
        conditionList.Add(Condition.Read(iScene, childNode));
    }
    return conditionList.ToArray();
  }

  public static Magicka.Levels.Triggers.Actions.Action[][] ReadActions(
    GameScene iScene,
    Trigger iTrigger,
    XmlNode iNode)
  {
    List<Magicka.Levels.Triggers.Actions.Action> collection = new List<Magicka.Levels.Triggers.Actions.Action>();
    List<List<Magicka.Levels.Triggers.Actions.Action>> actionListList = new List<List<Magicka.Levels.Triggers.Actions.Action>>();
    for (int i1 = 0; i1 < iNode.ChildNodes.Count; ++i1)
    {
      XmlNode childNode1 = iNode.ChildNodes[i1];
      if (!(childNode1 is XmlComment))
      {
        if (childNode1.Name.Equals("Looped", StringComparison.OrdinalIgnoreCase))
        {
          if (NetworkManager.Instance.State == NetworkState.Client)
          {
            NetworkClient networkClient = NetworkManager.Instance.Interface as NetworkClient;
            while (!networkClient.SaveSlot.IsValid)
              Thread.Sleep(1);
            if (!networkClient.SaveSlot.Looped)
              continue;
          }
          else if (!SubMenuCampaignSelect_SaveSlotSelect.Instance.CurrentSaveData.Looped)
            continue;
          for (int i2 = 0; i2 < childNode1.ChildNodes.Count; ++i2)
          {
            XmlNode childNode2 = childNode1.ChildNodes[i2];
            if (childNode2.Name.Equals("Random", StringComparison.OrdinalIgnoreCase))
            {
              List<Magicka.Levels.Triggers.Actions.Action> actionList = new List<Magicka.Levels.Triggers.Actions.Action>();
              for (int i3 = 0; i3 < childNode2.ChildNodes.Count; ++i3)
                actionList.Add(Magicka.Levels.Triggers.Actions.Action.Read(iScene, iTrigger, childNode2.ChildNodes[i3]));
              actionListList.Add(actionList);
            }
            else
              collection.Add(Magicka.Levels.Triggers.Actions.Action.Read(iScene, iTrigger, childNode2));
          }
        }
        else if (childNode1.Name.Equals("Random", StringComparison.OrdinalIgnoreCase))
        {
          List<Magicka.Levels.Triggers.Actions.Action> actionList = new List<Magicka.Levels.Triggers.Actions.Action>();
          for (int i4 = 0; i4 < childNode1.ChildNodes.Count; ++i4)
            actionList.Add(Magicka.Levels.Triggers.Actions.Action.Read(iScene, iTrigger, childNode1.ChildNodes[i4]));
          actionListList.Add(actionList);
        }
        else
          collection.Add(Magicka.Levels.Triggers.Actions.Action.Read(iScene, iTrigger, childNode1));
      }
    }
    if (actionListList.Count == 0)
      actionListList.Add(new List<Magicka.Levels.Triggers.Actions.Action>());
    for (int index = 0; index < actionListList.Count; ++index)
      actionListList[index].AddRange((IEnumerable<Magicka.Levels.Triggers.Actions.Action>) collection);
    Magicka.Levels.Triggers.Actions.Action[][] actionArray = new Magicka.Levels.Triggers.Actions.Action[actionListList.Count][];
    for (int index = 0; index < actionListList.Count; ++index)
      actionArray[index] = actionListList[index].ToArray();
    return actionArray;
  }

  public int ID => this.mID;

  public bool HasTriggered => this.mHasTriggered;

  public void ResetTimers() => this.mTimeTillRepeat = 0.0f;

  public virtual void Initialize()
  {
    if (this.mConditions == null)
      this.mConditions = new Condition[0][];
    for (int index = 0; index < this.mConditions.Length; ++index)
      this.mConditions[index].Initialize();
    for (int index = 0; index < this.mActions.Length; ++index)
    {
      foreach (Magicka.Levels.Triggers.Actions.Action action in this.mActions[index])
        action.Initialize();
    }
  }

  public virtual Trigger.State GetState() => new Trigger.State(this);

  internal static void NetworkAction(ref TriggerActionMessage iMsg)
  {
    switch (iMsg.ActionType)
    {
      case TriggerActionType.TriggerExecute:
        Trigger.TriggerExecute(ref iMsg);
        break;
      case TriggerActionType.SpawnNPC:
        Trigger.SpawnNPC(ref iMsg);
        break;
      case TriggerActionType.SpawnElemental:
        Trigger.SpawnElemental(ref iMsg);
        break;
      case TriggerActionType.SpawnLuggage:
        Trigger.SpawnLuggage(ref iMsg);
        break;
      case TriggerActionType.SpawnItem:
        Trigger.SpawnItem(ref iMsg);
        break;
      case TriggerActionType.SpawnMagick:
        Trigger.SpawnMagick(ref iMsg);
        break;
      case TriggerActionType.ChangeScene:
        Trigger.ChangeScene(ref iMsg);
        break;
      case TriggerActionType.EnterScene:
        Trigger.EnterScene(ref iMsg);
        break;
      case TriggerActionType.ThunderBolt:
        Trigger.Thunderbolt(ref iMsg);
        break;
      case TriggerActionType.LightningBolt:
        Trigger.TrigLightningBolt(ref iMsg);
        break;
      case TriggerActionType.SpawnGrease:
        Trigger.SpawnGrease(ref iMsg);
        break;
      case TriggerActionType.Nullify:
        Trigger.Nullify(ref iMsg);
        break;
      case TriggerActionType.SpawnTornado:
        Trigger.SpawnTornado(ref iMsg);
        break;
      case TriggerActionType.NapalmStrike:
        Trigger.NapalmStrike(ref iMsg);
        break;
      case TriggerActionType.Charm:
        Trigger.Charm(ref iMsg);
        break;
      case TriggerActionType.Starfall:
        Trigger.Starfall(ref iMsg);
        break;
      case TriggerActionType.OtherworldlyDischarge:
        Trigger.OtherworldlyDischarge(ref iMsg);
        break;
      case TriggerActionType.OtherworldlyBoltDestroyed:
        Trigger.OtherworldlyBoltDestroyed(ref iMsg);
        break;
      case TriggerActionType.Confuse:
        Trigger.Confuse(ref iMsg);
        break;
      case TriggerActionType.StarGaze:
        Trigger.StarGaze(ref iMsg);
        break;
      default:
        throw new NotImplementedException();
    }
  }

  private static void TriggerExecute(ref TriggerActionMessage iMsg)
  {
    GameScene scene = PlayState.RecentPlayState.Level.GetScene(iMsg.Scene);
    if (scene == null)
      return;
    Entity oEntity;
    Entity.TryGetFromHandle(iMsg.Handle, out oEntity);
    Trigger trigger = scene.Triggers[iMsg.Id];
    trigger.mHasTriggered = true;
    trigger.mTimeTillRepeat = trigger.mRepeat;
    for (int index = 0; index < trigger.mActions[iMsg.Arg].Length; ++index)
      trigger.mActions[iMsg.Arg][index].OnTrigger(oEntity as Magicka.GameLogic.Entities.Character);
  }

  private static void SpawnElemental(ref TriggerActionMessage iMsg)
  {
    PlayState recentPlayState = PlayState.RecentPlayState;
    ElementalEgg fromHandle1 = Entity.GetFromHandle((int) iMsg.Handle) as ElementalEgg;
    fromHandle1.Initialize(ref iMsg.Position, ref iMsg.Direction, iMsg.Id);
    fromHandle1.Proximity = iMsg.Time;
    if (iMsg.Arg != 0 && Entity.GetFromHandle(iMsg.Arg) is Magicka.GameLogic.Entities.Character fromHandle2)
      fromHandle1.SetSummoned((ISpellCaster) fromHandle2);
    recentPlayState.EntityManager.AddEntity((Entity) fromHandle1);
  }

  private static void SpawnNPC(ref TriggerActionMessage iMsg)
  {
    PlayState recentPlayState = PlayState.RecentPlayState;
    NonPlayerCharacter fromHandle1 = Entity.GetFromHandle((int) iMsg.Handle) as NonPlayerCharacter;
    CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(iMsg.Template);
    fromHandle1.Initialize(cachedTemplate, iMsg.Arg, iMsg.Position, iMsg.Id);
    fromHandle1.Dialog = iMsg.Point0;
    if (iMsg.Bool0)
      fromHandle1.ForceDraw();
    fromHandle1.Color = iMsg.Color;
    Vector3 result;
    Vector3.Cross(ref iMsg.Direction, ref new Vector3()
    {
      Y = 1f
    }, out result);
    fromHandle1.CharacterBody.Orientation = new Matrix()
    {
      M44 = 1f,
      M22 = 1f,
      Forward = iMsg.Direction,
      Right = result
    };
    fromHandle1.CharacterBody.DesiredDirection = iMsg.Direction;
    Magicka.Animations point1 = (Magicka.Animations) iMsg.Point1;
    if (point1 != Magicka.Animations.None)
      fromHandle1.ForceAnimation(point1);
    Magicka.Animations point2 = (Magicka.Animations) iMsg.Point2;
    switch (point2)
    {
      case Magicka.Animations.None:
      case Magicka.Animations.idle:
      case Magicka.Animations.idle_agg:
        Magicka.Animations point3 = (Magicka.Animations) iMsg.Point3;
        if (point3 != Magicka.Animations.None)
        {
          fromHandle1.SpecialIdleAnimation = point3;
          if (!(fromHandle1.CurrentState is RessurectionState))
            fromHandle1.ForceAnimation(point3);
        }
        if (iMsg.Scene != 0)
        {
          if (Entity.GetFromHandle(iMsg.Scene) is Magicka.GameLogic.Entities.Character fromHandle2)
            fromHandle1.Summoned(fromHandle2);
        }
        else if (recentPlayState.Level.CurrentScene.RuleSet != null && recentPlayState.Level.CurrentScene.RuleSet is SurvivalRuleset)
        {
          fromHandle1.Faction = Factions.EVIL;
          (recentPlayState.Level.CurrentScene.RuleSet as SurvivalRuleset).AddedCharacter(fromHandle1, iMsg.Bool0);
        }
        recentPlayState.EntityManager.AddEntity((Entity) fromHandle1);
        break;
      default:
        fromHandle1.SpawnAnimation = point2;
        fromHandle1.ChangeState((BaseState) RessurectionState.Instance);
        goto case Magicka.Animations.None;
    }
  }

  private static void SpawnLuggage(ref TriggerActionMessage iMsg)
  {
    PlayState recentPlayState = PlayState.RecentPlayState;
    NonPlayerCharacter fromHandle = Entity.GetFromHandle((int) iMsg.Handle) as NonPlayerCharacter;
    fromHandle.Initialize(CharacterTemplate.GetCachedTemplate(iMsg.Template), iMsg.Position, iMsg.Id);
    Vector3 result;
    Vector3.Cross(ref iMsg.Direction, ref new Vector3()
    {
      Y = 1f
    }, out result);
    Matrix iTransform = new Matrix();
    iTransform.M44 = 1f;
    iTransform.M22 = 1f;
    iTransform.Forward = iMsg.Direction;
    iTransform.Right = result;
    fromHandle.CharacterBody.Orientation = iTransform;
    fromHandle.CharacterBody.DesiredDirection = iMsg.Direction;
    fromHandle.SpawnAnimation = (Magicka.Animations) iMsg.Point0;
    fromHandle.ChangeState((BaseState) RessurectionState.Instance);
    iTransform.Translation = iMsg.Position;
    EffectManager.Instance.StartEffect("luggage_spawn".GetHashCodeCustom(), ref iTransform, out VisualEffectReference _);
    recentPlayState.EntityManager.AddEntity((Entity) fromHandle);
  }

  private static void SpawnItem(ref TriggerActionMessage iMsg)
  {
    PlayState recentPlayState = PlayState.RecentPlayState;
    if (!(Entity.GetFromHandle((int) iMsg.Handle) is Item fromHandle))
    {
      if (!Debugger.IsAttached)
        return;
      Debugger.Log(1, nameof (Trigger), "Avoiding NULL-exception in SpawnItem() ! The requested item will not be spawnd. \n");
    }
    else
    {
      try
      {
        Item.GetCachedWeapon(iMsg.Template, fromHandle);
      }
      catch (Exception ex)
      {
        if (!Debugger.IsAttached)
          return;
        Debugger.Log(1, nameof (Trigger), $"Ignoring exception in Trigger::SpawnItem() !\nMessage: {ex.Message}\n");
        return;
      }
      fromHandle.OnPickup = iMsg.Point0;
      fromHandle.Detach();
      Vector3 position = iMsg.Position;
      Matrix result;
      Matrix.CreateFromQuaternion(ref iMsg.Orientation, out result);
      fromHandle.Body.MoveTo(position, result);
      fromHandle.Body.Velocity = iMsg.Direction;
      if ((double) iMsg.Time > 0.0)
        fromHandle.Despawn(iMsg.Time);
      recentPlayState.EntityManager.AddEntity((Entity) fromHandle);
      if (iMsg.Bool0)
        fromHandle.Body.EnableBody();
      fromHandle.IgnoreTractorPull = iMsg.Bool1;
      AnimatedLevelPart animatedLevelPart;
      if (iMsg.Point1 == 0 || !recentPlayState.Level.CurrentScene.LevelModel.AnimatedLevelParts.TryGetValue(iMsg.Point1, out animatedLevelPart))
        return;
      animatedLevelPart.AddEntity((Entity) fromHandle);
      fromHandle.AnimatedLevelPartID = iMsg.Point1;
    }
  }

  private static void SpawnMagick(ref TriggerActionMessage iMsg)
  {
    PlayState recentPlayState = PlayState.RecentPlayState;
    Vector3 position = iMsg.Position;
    Matrix result;
    Matrix.CreateFromQuaternion(ref iMsg.Orientation, out result);
    BookOfMagick fromHandle = Entity.GetFromHandle((int) iMsg.Handle) as BookOfMagick;
    Vector3 direction = iMsg.Direction;
    fromHandle.Initialize(position, result, (MagickType) iMsg.Template, iMsg.Bool0, direction, iMsg.Time, iMsg.Point0);
    recentPlayState.EntityManager.AddEntity((Entity) fromHandle);
  }

  private static unsafe void ChangeScene(ref TriggerActionMessage iMsg)
  {
    PlayState recentPlayState = PlayState.RecentPlayState;
    SpawnPoint iSpawnPoint = new SpawnPoint();
    iSpawnPoint.Scene = iMsg.Scene;
    iSpawnPoint.Locations[0] = iMsg.Point0;
    iSpawnPoint.Locations[1] = iMsg.Point1;
    iSpawnPoint.Locations[2] = iMsg.Point2;
    iSpawnPoint.Locations[3] = iMsg.Point3;
    iSpawnPoint.SpawnPlayers = iMsg.Bool0;
    recentPlayState.Level.GoToScene(iSpawnPoint, (Transitions) iMsg.Template, iMsg.Time, iMsg.Bool1, (System.Action) null);
  }

  private static unsafe void EnterScene(ref TriggerActionMessage iMsg)
  {
    PlayState ps = PlayState.RecentPlayState;
    SpawnPoint iSpawnPoint = new SpawnPoint();
    iSpawnPoint.Scene = iMsg.Scene;
    iSpawnPoint.Locations[0] = iMsg.Point0;
    iSpawnPoint.Locations[1] = iMsg.Point1;
    iSpawnPoint.Locations[2] = iMsg.Point2;
    iSpawnPoint.Locations[3] = iMsg.Point3;
    iSpawnPoint.SpawnPlayers = true;
    int[] targets = new int[4]
    {
      iMsg.Target0,
      iMsg.Target1,
      iMsg.Target2,
      iMsg.Target3
    };
    bool fixedDirection = iMsg.Bool2;
    bool ignoreTriggers = iMsg.Bool0;
    int moveTrigger = iMsg.Arg;
    ps.Level.GoToScene(iSpawnPoint, (Transitions) iMsg.Template, iMsg.Time, iMsg.Bool1, (System.Action) (() =>
    {
      GameScene currentScene = ps.Level.CurrentScene;
      AIEvent aiEvent = new AIEvent();
      aiEvent.EventType = AIEventType.Move;
      aiEvent.MoveEvent.Delay = 0.0f;
      aiEvent.MoveEvent.Speed = 1f;
      aiEvent.MoveEvent.FixedDirection = fixedDirection;
      aiEvent.MoveEvent.Trigger = moveTrigger;
      Player[] players = Magicka.Game.Instance.Players;
      for (int index = 0; index < players.Length; ++index)
      {
        Player player = players[index];
        Matrix oLocator;
        if (player.Playing && player.Avatar != null && currentScene.TryGetLocator(targets[index], out oLocator))
        {
          aiEvent.MoveEvent.Waypoint = oLocator.Translation;
          aiEvent.MoveEvent.Direction = oLocator.Forward;
          player.Avatar.Events = new AIEvent[1]{ aiEvent };
          player.Avatar.IgnoreTriggers = ignoreTriggers;
        }
      }
    }));
  }

  private static void Thunderbolt(ref TriggerActionMessage iMsg)
  {
    PlayState recentPlayState = PlayState.RecentPlayState;
    Entity fromHandle1 = Entity.GetFromHandle((int) iMsg.Handle);
    Entity fromHandle2 = Entity.GetFromHandle(iMsg.Id);
    Vector3 vector3 = iMsg.Position;
    Vector3 iPosition = vector3;
    iPosition.Y += 40f;
    Magicka.Graphics.Flash.Instance.Execute(recentPlayState.Scene, 0.125f);
    LightningBolt lightning = LightningBolt.GetLightning();
    Vector3 iDirection = new Vector3(0.0f, -1f, 0.0f);
    Vector3 lightningcolor = Spell.LIGHTNINGCOLOR;
    Vector3 position = recentPlayState.Scene.Camera.Position;
    if (iMsg.Id != 0)
      iDirection = Vector3.Right;
    float iScale = 1f;
    lightning.InitializeEffect(ref iPosition, ref iDirection, ref vector3, ref position, ref lightningcolor, false, iScale, 1f, recentPlayState);
    EffectManager.Instance.StartEffect(Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Thunderbolt.EFFECT, ref vector3, ref iDirection, out VisualEffectReference _);
    if (!(fromHandle2 is Shield))
    {
      Segment iSeg = new Segment();
      iSeg.Origin = vector3;
      ++iSeg.Origin.Y;
      iSeg.Delta.Y -= 10f;
      Vector3 oPos;
      Vector3 oNrm;
      AnimatedLevelPart oAnimatedLevelPart;
      if (recentPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out oNrm, out oAnimatedLevelPart, iSeg))
      {
        vector3 = oPos;
        DecalManager.Instance.AddAlphaBlendedDecal(Decal.Scorched, oAnimatedLevelPart, 4f, ref vector3, ref oNrm, 60f);
      }
    }
    Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Thunderbolt.sAudioEmitter.Position = vector3;
    Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Thunderbolt.sAudioEmitter.Up = Vector3.Up;
    Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Thunderbolt.sAudioEmitter.Forward = Vector3.Right;
    AudioManager.Instance.PlayCue(Banks.Spells, Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Thunderbolt.SOUND, Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Thunderbolt.sAudioEmitter);
    recentPlayState.Camera.CameraShake(vector3, 1.5f, 0.333f);
    if (iMsg.Id != 0)
    {
      int num = (int) (fromHandle2 as IDamageable).Damage(Thunderstorm.sDamage, fromHandle1, recentPlayState.PlayTime, vector3);
      if (fromHandle2 is Avatar && (double) (fromHandle2 as Avatar).HitPoints > 0.0 && !((fromHandle2 as Avatar).Player.Gamer is NetworkGamer))
        AchievementsManager.Instance.AwardAchievement(recentPlayState, "oneinamillion");
    }
    Thunderstorm.Instance.CheckPerfectStorm();
  }

  private static void TrigLightningBolt(ref TriggerActionMessage iMsg)
  {
    PlayState recentPlayState = PlayState.RecentPlayState;
    Entity fromHandle = Entity.GetFromHandle((int) iMsg.Handle);
    Entity.GetFromHandle(iMsg.Id);
    Vector3 position = iMsg.Position;
    Vector3 direction = iMsg.Direction;
    Spell spell = iMsg.Spell;
    Magicka.Graphics.Flash.Instance.Execute(recentPlayState.Scene, 0.075f);
    recentPlayState.Camera.CameraShake(position, 0.5f, 0.075f);
    LightningBolt lightning = LightningBolt.GetLightning();
    lightning.AirToSurface = true;
    HitList iHitList = new HitList(16 /*0x10*/);
    DamageCollection5 oDamages;
    spell.CalculateDamage(SpellType.Lightning, CastType.Force, out oDamages);
    lightning.Cast(fromHandle as ISpellCaster, position, position - direction, iHitList, Spell.LIGHTNINGCOLOR, 10f, ref oDamages, new Spell?(spell), recentPlayState);
  }

  private static void Nullify(ref TriggerActionMessage iMsg)
  {
    Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Nullify.Instance.NullifyArea(PlayState.RecentPlayState, iMsg.Position, iMsg.Bool0);
  }

  private static void SpawnGrease(ref TriggerActionMessage iMsg)
  {
    PlayState recentPlayState = PlayState.RecentPlayState;
    Magicka.GameLogic.Entities.Character fromHandle = Entity.GetFromHandle(iMsg.Id) as Magicka.GameLogic.Entities.Character;
    Grease.GreaseField specificInstance = Grease.GreaseField.GetSpecificInstance(iMsg.Handle);
    AnimatedLevelPart iAnimation = (AnimatedLevelPart) null;
    if (iMsg.Arg != (int) ushort.MaxValue)
      iAnimation = AnimatedLevelPart.GetFromHandle(iMsg.Arg);
    specificInstance.Initialize((ISpellCaster) fromHandle, iAnimation, ref iMsg.Position, ref iMsg.Direction);
    recentPlayState.EntityManager.AddEntity((Entity) specificInstance);
  }

  private static void SpawnTornado(ref TriggerActionMessage iMsg)
  {
    PlayState recentPlayState = PlayState.RecentPlayState;
    ISpellCaster fromHandle = Entity.GetFromHandle(iMsg.Id) as ISpellCaster;
    TornadoEntity specificInstance = TornadoEntity.GetSpecificInstance(iMsg.Handle);
    Matrix result;
    Matrix.CreateFromQuaternion(ref iMsg.Orientation, out result);
    result.Translation = iMsg.Position;
    specificInstance.Initialize(recentPlayState, result, fromHandle);
    recentPlayState.EntityManager.AddEntity((Entity) specificInstance);
  }

  private static void NapalmStrike(ref TriggerActionMessage iMsg)
  {
    PlayState recentPlayState = PlayState.RecentPlayState;
    Vector3 up = Vector3.Up;
    EffectManager.Instance.StartEffect(Napalm.NAPALM_EFFECT, ref iMsg.Position, ref up, out VisualEffectReference _);
    AudioManager.Instance.PlayCue(Banks.Additional, Napalm.NAPALM_SOUND);
    Magicka.GameLogic.Entities.Character fromHandle = Entity.GetFromHandle((int) iMsg.Handle) as Magicka.GameLogic.Entities.Character;
    recentPlayState.Camera.CameraShake(iMsg.Position, 0.25f, 0.5f);
    if (iMsg.Id != int.MinValue)
    {
      Grease.GreaseField specificInstance = Grease.GreaseField.GetSpecificInstance((ushort) iMsg.Id);
      AnimatedLevelPart iAnimation = (AnimatedLevelPart) null;
      if (iMsg.Arg != (int) ushort.MaxValue)
        iAnimation = AnimatedLevelPart.GetFromHandle(iMsg.Arg);
      specificInstance.Initialize((ISpellCaster) fromHandle, iAnimation, ref iMsg.Position, ref iMsg.Direction);
      specificInstance.Burn(3f);
      recentPlayState.EntityManager.AddEntity((Entity) specificInstance);
    }
    int num = (int) Napalm.NapalmBlast(recentPlayState, ref iMsg.Position, (Entity) fromHandle, iMsg.TimeStamp);
  }

  private static void Charm(ref TriggerActionMessage iMsg)
  {
    if (!(Entity.GetFromHandle((int) iMsg.Handle) is Magicka.GameLogic.Entities.Character fromHandle1))
      return;
    AudioManager.Instance.PlayCue(Banks.Spells, Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Charm.SOUND, fromHandle1.AudioEmitter);
    Entity fromHandle2 = Entity.GetFromHandle((int) (ushort) iMsg.Id);
    fromHandle1.Charm(fromHandle2, iMsg.Time, iMsg.Arg);
  }

  private static void Confuse(ref TriggerActionMessage iMsg)
  {
    if (!(Entity.GetFromHandle((int) iMsg.Handle) is NonPlayerCharacter fromHandle))
      return;
    if (iMsg.Bool0)
      fromHandle.Confuse(Factions.NONE);
    else
      fromHandle.Confuse(fromHandle.Template.Faction);
  }

  private static void Starfall(ref TriggerActionMessage iMsg)
  {
    Entity oEntity;
    Entity.TryGetFromHandle(iMsg.Handle, out oEntity);
    Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Starfall.Instance.Execute(oEntity as ISpellCaster, PlayState.RecentPlayState, iMsg.Position, false);
  }

  private static void OtherworldlyDischarge(ref TriggerActionMessage iMsg)
  {
    Entity oEntity1;
    Entity oEntity2;
    if (!Entity.TryGetFromHandle(iMsg.Handle, out oEntity1) || !Entity.TryGetFromHandle((ushort) iMsg.Arg, out oEntity2) || !(oEntity2 is IDamageable))
      return;
    Magicka.GameLogic.Entities.Abilities.SpecialAbilities.OtherworldlyDischarge.Instance.Execute(oEntity2 as IDamageable, oEntity1 as ISpellCaster, oEntity2.PlayState);
  }

  private static void OtherworldlyBoltDestroyed(ref TriggerActionMessage iMsg)
  {
    Entity oEntity1;
    Entity oEntity2;
    if (!Entity.TryGetFromHandle(iMsg.Handle, out oEntity1) || !Entity.TryGetFromHandle((ushort) iMsg.Arg, out oEntity2) || !(oEntity1 is OtherworldlyBolt))
      return;
    bool bool0 = iMsg.Bool0;
    bool bool1 = iMsg.Bool1;
    bool bool2 = iMsg.Bool2;
    (oEntity1 as OtherworldlyBolt).Destroy(bool0, bool1, oEntity2, bool2);
  }

  private static void StarGaze(ref TriggerActionMessage iMsg)
  {
    Entity oEntity1;
    Entity oEntity2;
    if (!Entity.TryGetFromHandle(iMsg.Handle, out oEntity1) || !Entity.TryGetFromHandle((ushort) iMsg.Arg, out oEntity2) || !(oEntity2 is Magicka.GameLogic.Entities.Character) || !(oEntity1 is ISpellCaster))
      return;
    Magicka.GameLogic.Entities.Abilities.SpecialAbilities.StarGaze.Instance.Execute(oEntity1 as ISpellCaster, oEntity2 as Magicka.GameLogic.Entities.Character, oEntity2.PlayState);
  }

  internal static void NetworkAction(ref TriggerRequestMessage iMsg)
  {
    GameScene scene = PlayState.RecentPlayState.Level.GetScene(iMsg.Scene);
    Entity oEntity;
    Entity.TryGetFromHandle(iMsg.Handle, out oEntity);
    scene.ExecuteTrigger(iMsg.Id, oEntity as Magicka.GameLogic.Entities.Character, true);
  }

  public class State
  {
    protected Trigger mTrigger;
    private float mTimeTillRepeat;
    private bool mHasTriggered;
    protected Magicka.Levels.Triggers.Actions.Action.State[][] mActions;

    public State(Trigger iTrigger)
    {
      this.mTrigger = iTrigger;
      this.mActions = new Magicka.Levels.Triggers.Actions.Action.State[this.mTrigger.mActions.Length][];
      for (int index = 0; index < this.mActions.Length; ++index)
        this.mActions[index] = new Magicka.Levels.Triggers.Actions.Action.State[this.mTrigger.mActions[index].Length];
      this.UpdateState();
    }

    public virtual void UpdateState()
    {
      this.mTimeTillRepeat = this.mTrigger.mTimeTillRepeat;
      this.mHasTriggered = this.mTrigger.mHasTriggered;
      for (int index1 = 0; index1 < this.mActions.Length; ++index1)
      {
        for (int index2 = 0; index2 < this.mActions[index1].Length; ++index2)
          this.mActions[index1][index2] = new Magicka.Levels.Triggers.Actions.Action.State(this.mTrigger.mActions[index1][index2]);
      }
    }

    public virtual void ResetState()
    {
      this.mTrigger.mTimeTillRepeat = 0.0f;
      this.mTrigger.mHasTriggered = false;
      for (int index1 = 0; index1 < this.mActions.Length; ++index1)
      {
        for (int index2 = 0; index2 < this.mActions[index1].Length; ++index2)
          this.mActions[index1][index2].Reset(this.mTrigger.mActions[index1][index2]);
      }
    }

    public virtual void ApplyState()
    {
      this.mTrigger.mTimeTillRepeat = this.mTimeTillRepeat;
      this.mTrigger.mHasTriggered = this.mHasTriggered;
      for (int index1 = 0; index1 < this.mActions.Length; ++index1)
      {
        for (int index2 = 0; index2 < this.mActions[index1].Length; ++index2)
          this.mActions[index1][index2].AssignToAction(this.mTrigger.mActions[index1][index2]);
      }
    }

    public Trigger Trigger => this.mTrigger;

    internal void Write(BinaryWriter iWriter)
    {
      iWriter.Write(this.mTimeTillRepeat);
      iWriter.Write(this.mHasTriggered);
      for (int index1 = 0; index1 < this.mActions.Length; ++index1)
      {
        for (int index2 = 0; index2 < this.mActions[index1].Length; ++index2)
          this.mActions[index1][index2].Write(iWriter, this.mTrigger.mActions[index1][index2]);
      }
    }

    internal void Read(BinaryReader iReader)
    {
      this.mTimeTillRepeat = iReader.ReadSingle();
      this.mHasTriggered = iReader.ReadBoolean();
      for (int index1 = 0; index1 < this.mActions.Length; ++index1)
      {
        for (int index2 = 0; index2 < this.mActions[index1].Length; ++index2)
          this.mActions[index1][index2] = new Magicka.Levels.Triggers.Actions.Action.State(iReader, this.mTrigger.mActions[index1][index2]);
      }
    }
  }
}
