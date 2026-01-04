// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Bosses.Grimnir
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.AI;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;
using System.Collections.Generic;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

#nullable disable
namespace Magicka.GameLogic.Entities.Bosses;

public class Grimnir : IBoss
{
  private const float NETWORK_UPDATE_PERIOD = 0.0333333351f;
  private const float DREAM_FADE_TRANSITION_TIME = 1.5f;
  private const float MINION_SPAWN_DELAY = 1f;
  private const float DAMAGE_PER_SUCCESSIVE_DREAM = 1f;
  private const float FADE_IN_TIME = 0.25f;
  private const float FADE_OUT_TIME = 0.25f;
  private const float FADE_GAP = 0.2f;
  private const float MAXHITPOINTS = 6f;
  private const float MAXHITPOINTSDIVISOR = 0.166666672f;
  protected float mNetworkUpdateTimer;
  private static readonly int SPAWN_MINION_EFFECT = Teleport.TELEPORT_EFFECT_APPEAR;
  private static readonly int SPAWN_MINION_SOUND = Teleport.TELEPORT_SOUND_ORIGIN;
  private static readonly Vector3 DIALOG_OFFSET = new Vector3(-3f, 3f, 0.0f);
  private static readonly int DIALOG_DEAD = "grimnirdead".GetHashCodeCustom();
  private static readonly int[] DIALOG_TAUNTS = new int[6]
  {
    "grimnirtaunt1".GetHashCodeCustom(),
    "grimnirtaunt2".GetHashCodeCustom(),
    "grimnirtaunt3".GetHashCodeCustom(),
    "grimnirtaunt4".GetHashCodeCustom(),
    "grimnirtaunt5".GetHashCodeCustom(),
    "grimnirtaunt6".GetHashCodeCustom()
  };
  private static readonly int[,] SPAWN_HASH = new int[6, 4]
  {
    {
      "starta0".GetHashCodeCustom(),
      "starta1".GetHashCodeCustom(),
      "starta2".GetHashCodeCustom(),
      "starta3".GetHashCodeCustom()
    },
    {
      "startc0".GetHashCodeCustom(),
      "startc1".GetHashCodeCustom(),
      "startc2".GetHashCodeCustom(),
      "startc3".GetHashCodeCustom()
    },
    {
      "startf0".GetHashCodeCustom(),
      "startf1".GetHashCodeCustom(),
      "startf2".GetHashCodeCustom(),
      "startf3".GetHashCodeCustom()
    },
    {
      "startd0".GetHashCodeCustom(),
      "startd1".GetHashCodeCustom(),
      "startd2".GetHashCodeCustom(),
      "startd3".GetHashCodeCustom()
    },
    {
      "starte0".GetHashCodeCustom(),
      "starte1".GetHashCodeCustom(),
      "starte2".GetHashCodeCustom(),
      "starte3".GetHashCodeCustom()
    },
    {
      "startb0".GetHashCodeCustom(),
      "startb1".GetHashCodeCustom(),
      "startb2".GetHashCodeCustom(),
      "startb3".GetHashCodeCustom()
    }
  };
  private static readonly int[,] MINION_SPAWN_HASH = new int[6, 4]
  {
    {
      "spawna0".GetHashCodeCustom(),
      "spawna1".GetHashCodeCustom(),
      "spawna2".GetHashCodeCustom(),
      "spawna3".GetHashCodeCustom()
    },
    {
      "spawnc0".GetHashCodeCustom(),
      "spawnc1".GetHashCodeCustom(),
      "spawnc2".GetHashCodeCustom(),
      "spawnc3".GetHashCodeCustom()
    },
    {
      "spawnf0".GetHashCodeCustom(),
      "spawnf1".GetHashCodeCustom(),
      "spawnf2".GetHashCodeCustom(),
      "spawnf3".GetHashCodeCustom()
    },
    {
      "spawnd0".GetHashCodeCustom(),
      "spawnd1".GetHashCodeCustom(),
      "spawnd2".GetHashCodeCustom(),
      "spawnd3".GetHashCodeCustom()
    },
    {
      "spawne0".GetHashCodeCustom(),
      "spawne1".GetHashCodeCustom(),
      "spawne2".GetHashCodeCustom(),
      "spawne3".GetHashCodeCustom()
    },
    {
      "spawnb0".GetHashCodeCustom(),
      "spawnb1".GetHashCodeCustom(),
      "spawnb2".GetHashCodeCustom(),
      "spawnb3".GetHashCodeCustom()
    }
  };
  private static readonly int[] MINION_SPAWN_COUNT = new int[6]
  {
    4,
    2,
    2,
    2,
    2,
    1
  };
  private static Vector3[] GRIMNIR_POSITIONS;
  private static readonly int[] MINION_HASH = new int[6]
  {
    "goblin_shaman".GetHashCodeCustom(),
    "druid".GetHashCodeCustom(),
    "warlock".GetHashCodeCustom(),
    "goblin_warlock".GetHashCodeCustom(),
    "necromancer".GetHashCodeCustom(),
    "dwarf_mage".GetHashCodeCustom()
  };
  private float mDreamCompleteTimer;
  private bool mTransitionBegun;
  private float mSpawnDelay;
  private bool mMinionsSpawned;
  private float mNormalizedHitPoints;
  private float mFadeTimer;
  private bool mFadeIn;
  private float mHitPoints = 6f;
  private bool mDead;
  private float mGrimnirFloatDelta;
  private List<NonPlayerCharacter> mActiveMinions;
  private int mNrOfPlayers;
  private Player[] mPlayers;
  private Random mRandom;
  private Grimnir.RenderData[] mRenderData;
  private SkinnedModel mModel;
  private AnimationController mController;
  private AnimationClip[] mClips;
  private PlayState mPlayState;
  private Matrix mGrimnirOrientation;
  private Grimnir.ActionStates mCurrentActionState;
  private Grimnir.ActionStates mNextActionState;
  private IBossState<Grimnir> mCurrentState;

  public Grimnir(PlayState iPlayState)
  {
    this.mPlayers = Magicka.Game.Instance.Players;
    for (int index = 0; index < this.mPlayers.Length; ++index)
    {
      if (this.mPlayers[index].Playing)
        ++this.mNrOfPlayers;
    }
    this.mPlayState = iPlayState;
    this.mRandom = new Random();
    SkinnedModel skinnedModel;
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/druid");
      this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/goblin_shaman");
      this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/goblin_warlock");
      this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/necromancer");
      this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/warlock");
      this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/dwarf_mage");
      this.mModel = this.mPlayState.Content.Load<SkinnedModel>("Models/Bosses/grimnir/grimnir_mesh");
      skinnedModel = this.mPlayState.Content.Load<SkinnedModel>("Models/Bosses/grimnir/grimnir_animation");
    }
    this.mActiveMinions = new List<NonPlayerCharacter>(4);
    this.mController = new AnimationController();
    this.mController.Skeleton = skinnedModel.SkeletonBones;
    this.mClips = new AnimationClip[1];
    this.mClips[0] = skinnedModel.AnimationClips["idle"];
    SkinnedModelDeferredBasicMaterial oMaterial;
    Helper.SkinnedModelDeferredMaterialFromBasicEffect(this.mModel.Model.Meshes[0].MeshParts[0].Effect as SkinnedModelBasicEffect, out oMaterial);
    this.mRenderData = new Grimnir.RenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      this.mRenderData[index] = new Grimnir.RenderData();
      this.mRenderData[index].SetMesh(this.mModel.Model.Meshes[0].VertexBuffer, this.mModel.Model.Meshes[0].IndexBuffer, this.mModel.Model.Meshes[0].MeshParts[0], 2);
      this.mRenderData[index].mMaterial = oMaterial;
    }
  }

  protected unsafe void ChangeState(Grimnir.States iState)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    if (NetworkManager.Instance.State == NetworkState.Server)
    {
      Grimnir.ChangeStateMessage changeStateMessage;
      changeStateMessage.State = iState;
      BossFight.Instance.SendMessage<Grimnir.ChangeStateMessage>((IBoss) this, (ushort) 2, (void*) &changeStateMessage, true);
    }
    this.mCurrentState.OnExit(this);
    this.mCurrentState = this.GetState(iState);
    this.mCurrentState.OnEnter(this);
  }

  private IBossState<Grimnir> GetState(Grimnir.States iState)
  {
    switch (iState)
    {
      case Grimnir.States.IntroFade:
        return (IBossState<Grimnir>) Grimnir.IntroFadeState.Instance;
      case Grimnir.States.OutroFade:
        return (IBossState<Grimnir>) Grimnir.OutroFadeState.Instance;
      case Grimnir.States.Action:
        return (IBossState<Grimnir>) Grimnir.ActionState.Instance;
      case Grimnir.States.FadeTransition:
        return (IBossState<Grimnir>) Grimnir.FadeTransitionState.Instance;
      default:
        return (IBossState<Grimnir>) null;
    }
  }

  public void Initialize(ref Matrix iOrientation, int iUniqueID)
  {
    this.Initialize(ref iOrientation);
  }

  public void Initialize(ref Matrix iOrientation)
  {
    this.mHitPoints = 6f;
    this.mGrimnirFloatDelta = 0.0f;
    this.mDead = false;
    this.mGrimnirOrientation = Matrix.CreateRotationY(3.14159274f);
    Grimnir.GRIMNIR_POSITIONS = new Vector3[6];
    Locator oLocator;
    this.mPlayState.Level.CurrentScene.GetLocator("grimnira".GetHashCodeCustom(), out oLocator);
    Grimnir.GRIMNIR_POSITIONS[0] = oLocator.Transform.Translation;
    this.mPlayState.Level.CurrentScene.GetLocator("grimnirb".GetHashCodeCustom(), out oLocator);
    Grimnir.GRIMNIR_POSITIONS[1] = oLocator.Transform.Translation;
    this.mPlayState.Level.CurrentScene.GetLocator("grimnirc".GetHashCodeCustom(), out oLocator);
    Grimnir.GRIMNIR_POSITIONS[2] = oLocator.Transform.Translation;
    this.mPlayState.Level.CurrentScene.GetLocator("grimnird".GetHashCodeCustom(), out oLocator);
    Grimnir.GRIMNIR_POSITIONS[3] = oLocator.Transform.Translation;
    this.mPlayState.Level.CurrentScene.GetLocator("grimnire".GetHashCodeCustom(), out oLocator);
    Grimnir.GRIMNIR_POSITIONS[4] = oLocator.Transform.Translation;
    this.mPlayState.Level.CurrentScene.GetLocator("grimnirf".GetHashCodeCustom(), out oLocator);
    Grimnir.GRIMNIR_POSITIONS[5] = oLocator.Transform.Translation;
    this.mController.StartClip(this.mClips[0], true);
    this.mCurrentActionState = Grimnir.ActionStates.Mountaindale;
    this.mNextActionState = this.mCurrentActionState;
    this.mCurrentState = (IBossState<Grimnir>) Grimnir.IntroFadeState.Instance;
    this.mCurrentState.OnEnter(this);
    this.mActiveMinions.Clear();
  }

  public void DeInitialize()
  {
  }

  public void UpdateBoss(DataChannel iDataChannel, float iDeltaTime, bool iFightStarted)
  {
    if (NetworkManager.Instance.State == NetworkState.Server)
    {
      this.mNetworkUpdateTimer -= iDeltaTime;
      if ((double) this.mNetworkUpdateTimer <= 0.0)
      {
        this.mNetworkUpdateTimer = 0.0333333351f;
        this.NetworkUpdate();
      }
    }
    this.mNormalizedHitPoints = this.mHitPoints * 0.166666672f;
    this.mCurrentState.OnUpdate(iDeltaTime, this);
    for (int index = 0; index < this.mActiveMinions.Count; ++index)
    {
      if (this.mActiveMinions[index].Dead)
        this.mActiveMinions.RemoveAt(index--);
    }
    this.mGrimnirFloatDelta += iDeltaTime;
    Vector3 result = this.mPlayState.Camera.Position;
    Vector3 cameraoffset = MagickCamera.CAMERAOFFSET;
    cameraoffset.Y += 28f;
    cameraoffset.Z += 30f;
    cameraoffset.Y += (float) Math.Sin((double) this.mGrimnirFloatDelta) * 0.125f;
    cameraoffset.X += (float) Math.Cos((double) this.mGrimnirFloatDelta) * 0.125f;
    Vector3.Subtract(ref result, ref cameraoffset, out result);
    this.mGrimnirOrientation.Translation = result;
    Matrix grimnirOrientation = this.mGrimnirOrientation;
    MagickaMath.UniformMatrixScale(ref grimnirOrientation, 7f);
    this.mRenderData[(int) iDataChannel].mMaterial.Alpha = 0.65f;
    this.mController.Update(iDeltaTime, ref grimnirOrientation, true);
    this.mController.SkinnedBoneTransforms.CopyTo((Array) this.mRenderData[(int) iDataChannel].mBones, 0);
    this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, (IRenderableAdditiveObject) this.mRenderData[(int) iDataChannel]);
    this.mNrOfPlayers = 0;
    for (int index = 0; index < this.mPlayers.Length; ++index)
    {
      if (this.mPlayers[index].Playing)
        ++this.mNrOfPlayers;
    }
  }

  public Vector3 StateCameraTarget(Grimnir.ActionStates iState)
  {
    Locator oLocator;
    this.mPlayState.Level.CurrentScene.GetLocator(Grimnir.SPAWN_HASH[(int) iState, 1], out oLocator);
    return oLocator.Transform.Translation;
  }

  public void Corporealize()
  {
    this.mHitPoints = 0.0f;
    this.ChangeState(Grimnir.States.OutroFade);
  }

  public unsafe void TeleportPlayers(int id)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    if (NetworkManager.Instance.State == NetworkState.Server)
    {
      Grimnir.TeleportMessage teleportMessage;
      teleportMessage.Venue = id;
      BossFight.Instance.SendMessage<Grimnir.TeleportMessage>((IBoss) this, (ushort) 1, (void*) &teleportMessage, true);
    }
    for (int index = 0; index < this.mPlayers.Length; ++index)
    {
      if (this.mPlayers[index].Playing && this.mPlayers[index].Avatar != null && !this.mPlayers[index].Avatar.Dead)
      {
        Locator oLocator;
        this.mPlayState.Level.CurrentScene.GetLocator(Grimnir.SPAWN_HASH[id, index], out oLocator);
        Matrix transform = oLocator.Transform with
        {
          Translation = new Vector3()
        };
        Vector3 pos = oLocator.Transform.Translation;
        Segment iSeg = new Segment();
        iSeg.Origin = pos;
        iSeg.Delta.Y -= 7f;
        Vector3 oPos;
        if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out Vector3 _, iSeg))
          pos = oPos;
        pos.Y += (float) ((double) this.mPlayers[index].Avatar.Radius + (double) this.mPlayers[index].Avatar.Capsule.Length * 0.5 + 0.10000000149011612);
        this.mPlayers[index].Avatar.Body.MoveTo(pos, transform);
      }
    }
  }

  private unsafe void SpawnMinions(Grimnir.ActionStates iState)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    int index1 = (int) iState;
    this.mActiveMinions.Clear();
    int num = this.mRandom.Next(4);
    for (int index2 = 0; index2 < Grimnir.MINION_SPAWN_COUNT[(int) iState]; ++index2)
    {
      Locator oLocator;
      this.mPlayState.Level.CurrentScene.GetLocator(Grimnir.MINION_SPAWN_HASH[index1, (num + 4 - index2) % 4], out oLocator);
      CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(Grimnir.MINION_HASH[index1]);
      NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(this.mPlayState);
      Vector3 iPosition = oLocator.Transform.Translation;
      Segment iSeg = new Segment();
      iSeg.Origin = iPosition + Vector3.Up;
      iSeg.Delta.Y -= 5f;
      Vector3 oPos;
      if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out Vector3 _, iSeg))
        iPosition = oPos;
      iPosition.Y += (float) ((double) cachedTemplate.Radius + (double) cachedTemplate.Length * 0.5 + 0.10000000149011612);
      instance.Initialize(cachedTemplate, iPosition, 0);
      Vector3 forward = oLocator.Transform.Forward;
      instance.CharacterBody.DesiredDirection = forward;
      instance.AI.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, 0, 0, (AIEvent[]) null);
      this.mPlayState.EntityManager.AddEntity((Entity) instance);
      EffectManager.Instance.StartEffect(Grimnir.SPAWN_MINION_EFFECT, ref iPosition, ref forward, out VisualEffectReference _);
      AudioManager.Instance.PlayCue(Banks.Spells, Grimnir.SPAWN_MINION_SOUND, instance.AudioEmitter);
      if (NetworkManager.Instance.State == NetworkState.Server)
      {
        Grimnir.SpawnMessage spawnMessage;
        spawnMessage.Handle = instance.Handle;
        spawnMessage.Position = instance.Position;
        spawnMessage.Direction = instance.Direction;
        spawnMessage.TypeID = Grimnir.MINION_HASH[index1];
        BossFight.Instance.SendMessage<Grimnir.SpawnMessage>((IBoss) this, (ushort) 3, (void*) &spawnMessage, true);
      }
      this.mActiveMinions.Add(instance);
    }
  }

  public bool AddImpulseVelocity(
    Vector3 iDirection,
    float iElevation,
    float iMassPower,
    float iDistance)
  {
    throw new NotImplementedException();
  }

  public bool Dead => this.mDead;

  public float MaxHitPoints => 6f;

  public float HitPoints => this.mHitPoints;

  public DamageResult Damage(
    int iPartIndex,
    Magicka.GameLogic.Damage iDamage,
    Entity iAttacker,
    ref Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
    throw new NotImplementedException();
  }

  public void Damage(int iPartIndex, float iDamage, Elements iElement)
  {
    throw new NotImplementedException();
  }

  public void SetSlow(int iIndex) => throw new NotImplementedException();

  public void GetRandomPositionOnCollisionSkin(int iIndex, out Vector3 oPosition)
  {
    throw new NotImplementedException();
  }

  public bool HasStatus(int iIndex, StatusEffects iStatus) => throw new NotImplementedException();

  public float StatusMagnitude(int iIndex, StatusEffects iStatus)
  {
    throw new NotImplementedException();
  }

  public StatusEffect[] GetStatusEffects() => throw new NotImplementedException();

  public void ScriptMessage(BossMessages iMessage)
  {
  }

  private unsafe void NetworkUpdate()
  {
    if (!(NetworkManager.Instance.Interface is NetworkServer networkServer))
      return;
    Grimnir.UpdateMessage updateMessage = new Grimnir.UpdateMessage();
    updateMessage.HitPoints = this.mHitPoints;
    for (int iClientIndex = 0; iClientIndex < networkServer.Connections; ++iClientIndex)
      BossFight.Instance.SendMessage<Grimnir.UpdateMessage>((IBoss) this, (ushort) 0, (void*) &updateMessage, false, iClientIndex);
  }

  public unsafe void NetworkUpdate(ref BossUpdateMessage iMsg)
  {
    switch (iMsg.Type)
    {
      case 0:
        Grimnir.UpdateMessage updateMessage;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &updateMessage);
        this.mHitPoints = updateMessage.HitPoints;
        break;
      case 1:
        Grimnir.TeleportMessage teleportMessage;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &teleportMessage);
        int venue = teleportMessage.Venue;
        for (int index = 0; index < this.mPlayers.Length; ++index)
        {
          if (this.mPlayers[index].Playing && this.mPlayers[index].Avatar != null && !this.mPlayers[index].Avatar.Dead)
          {
            Locator oLocator;
            this.mPlayState.Level.CurrentScene.GetLocator(Grimnir.SPAWN_HASH[venue, index], out oLocator);
            Matrix transform = oLocator.Transform with
            {
              Translation = new Vector3()
            };
            Vector3 pos = oLocator.Transform.Translation;
            Segment iSeg = new Segment();
            iSeg.Origin = pos;
            iSeg.Delta.Y -= 7f;
            Vector3 oPos;
            if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out Vector3 _, iSeg))
              pos = oPos;
            pos.Y += (float) ((double) this.mPlayers[index].Avatar.Radius + (double) this.mPlayers[index].Avatar.Capsule.Length * 0.5 + 0.10000000149011612);
            this.mPlayers[index].Avatar.Body.MoveTo(pos, transform);
          }
        }
        break;
      case 2:
        Grimnir.ChangeStateMessage changeStateMessage;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &changeStateMessage);
        this.mCurrentState.OnExit(this);
        this.mCurrentState = this.GetState(changeStateMessage.State);
        this.mCurrentState.OnEnter(this);
        break;
      case 3:
        Grimnir.SpawnMessage spawnMessage;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &spawnMessage);
        NonPlayerCharacter fromHandle = Entity.GetFromHandle((int) spawnMessage.Handle) as NonPlayerCharacter;
        fromHandle.Initialize(CharacterTemplate.GetCachedTemplate(spawnMessage.TypeID), spawnMessage.Position, 0);
        fromHandle.CharacterBody.DesiredDirection = spawnMessage.Direction;
        this.mPlayState.EntityManager.AddEntity((Entity) fromHandle);
        fromHandle.AI.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, 0, 0, (AIEvent[]) null);
        this.mActiveMinions.Add(fromHandle);
        EffectManager.Instance.StartEffect(Grimnir.SPAWN_MINION_EFFECT, ref spawnMessage.Position, ref spawnMessage.Direction, out VisualEffectReference _);
        AudioManager.Instance.PlayCue(Banks.Spells, Grimnir.SPAWN_MINION_SOUND, fromHandle.AudioEmitter);
        break;
      case 4:
        Grimnir.CameraMessage cameraMessage;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &cameraMessage);
        this.mPlayState.Camera.SetPosition(ref cameraMessage.Position);
        break;
    }
  }

  public void NetworkInitialize(ref BossInitializeMessage iMsg)
  {
    throw new NotImplementedException();
  }

  public BossEnum GetBossType() => BossEnum.Grimnir;

  public bool NetworkInitialized => true;

  public float ResistanceAgainst(Elements iElement) => 1f;

  protected class RenderData : 
    RenderableAdditiveObject<SkinnedModelDeferredEffect, SkinnedModelDeferredBasicMaterial>
  {
    protected static readonly Vector4 ColdColor = new Vector4(1f, 1.6f, 2f, 1f);
    public Matrix[] mBones;
    public float Flash;

    public RenderData() => this.mBones = new Matrix[80 /*0x50*/];

    public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      SkinnedModelDeferredEffect modelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
      modelDeferredEffect.ProjectionMapEnabled = false;
      modelDeferredEffect.Bones = this.mBones;
      modelDeferredEffect.Colorize = Grimnir.RenderData.ColdColor;
      modelDeferredEffect.OverrideColor = new Vector4(1f, 1f, 1f, this.Flash);
      base.Draw(iEffect, iViewFrustum);
      modelDeferredEffect.OverrideColor = Vector4.Zero;
      modelDeferredEffect.Colorize = Vector4.Zero;
    }
  }

  public class IntroFadeState : IBossState<Grimnir>
  {
    private static Grimnir.IntroFadeState mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Grimnir.IntroFadeState Instance
    {
      get
      {
        if (Grimnir.IntroFadeState.mSingelton == null)
        {
          lock (Grimnir.IntroFadeState.mSingeltonLock)
          {
            if (Grimnir.IntroFadeState.mSingelton == null)
              Grimnir.IntroFadeState.mSingelton = new Grimnir.IntroFadeState();
          }
        }
        return Grimnir.IntroFadeState.mSingelton;
      }
    }

    private IntroFadeState()
    {
    }

    public void OnEnter(Grimnir iOwner)
    {
      iOwner.mFadeTimer = 0.25f;
      RenderManager.Instance.EndTransition(Transitions.Fade, Color.White, iOwner.mFadeTimer);
      for (int index = 0; index < iOwner.mPlayers.Length; ++index)
      {
        if (iOwner.mPlayers[index].Playing && iOwner.mPlayers[index].Avatar != null)
        {
          Magicka.Animations iAnimation = (Magicka.Animations) (131 + index);
          iOwner.mPlayers[index].Avatar.GoToAnimation(iAnimation, 0.0f);
        }
      }
    }

    public void OnUpdate(float iDeltaTime, Grimnir iOwner)
    {
      iOwner.mFadeTimer -= iDeltaTime;
      if ((double) iOwner.mFadeTimer - 0.20000000298023224 > 0.0)
        return;
      iOwner.mCurrentActionState = iOwner.mNextActionState;
      iOwner.ChangeState(Grimnir.States.Action);
    }

    public void OnExit(Grimnir iOwner)
    {
    }
  }

  public class OutroFadeState : IBossState<Grimnir>
  {
    private static Grimnir.OutroFadeState mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Grimnir.OutroFadeState Instance
    {
      get
      {
        if (Grimnir.OutroFadeState.mSingelton == null)
        {
          lock (Grimnir.OutroFadeState.mSingeltonLock)
          {
            if (Grimnir.OutroFadeState.mSingelton == null)
              Grimnir.OutroFadeState.mSingelton = new Grimnir.OutroFadeState();
          }
        }
        return Grimnir.OutroFadeState.mSingelton;
      }
    }

    private OutroFadeState()
    {
    }

    public void OnEnter(Grimnir iOwner)
    {
    }

    public void OnUpdate(float iDeltaTime, Grimnir iOwner)
    {
    }

    public void OnExit(Grimnir iOwner)
    {
    }
  }

  public class ActionState : IBossState<Grimnir>
  {
    private static Grimnir.ActionState mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Grimnir.ActionState Instance
    {
      get
      {
        if (Grimnir.ActionState.mSingelton == null)
        {
          lock (Grimnir.ActionState.mSingeltonLock)
          {
            if (Grimnir.ActionState.mSingelton == null)
              Grimnir.ActionState.mSingelton = new Grimnir.ActionState();
          }
        }
        return Grimnir.ActionState.mSingelton;
      }
    }

    private ActionState()
    {
    }

    public void OnEnter(Grimnir iOwner)
    {
      iOwner.mMinionsSpawned = false;
      iOwner.mSpawnDelay = 1f;
    }

    public void OnUpdate(float iDeltaTime, Grimnir iOwner)
    {
      if (NetworkManager.Instance.State == NetworkState.Client)
        return;
      iOwner.mSpawnDelay -= iDeltaTime;
      if (!iOwner.mMinionsSpawned && (double) iOwner.mSpawnDelay <= 0.0)
      {
        iOwner.SpawnMinions(iOwner.mCurrentActionState);
        iOwner.mMinionsSpawned = true;
      }
      else
      {
        if (!iOwner.mMinionsSpawned || iOwner.mActiveMinions.Count != 0)
          return;
        --iOwner.mHitPoints;
        if ((double) iOwner.mHitPoints <= 0.0 || iOwner.mCurrentActionState == Grimnir.ActionStates.Ruins)
          iOwner.ChangeState(Grimnir.States.OutroFade);
        else
          iOwner.ChangeState(Grimnir.States.FadeTransition);
      }
    }

    public void OnExit(Grimnir iOwner)
    {
    }
  }

  public class FadeTransitionState : IBossState<Grimnir>
  {
    private static Grimnir.FadeTransitionState mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Grimnir.FadeTransitionState Instance
    {
      get
      {
        if (Grimnir.FadeTransitionState.mSingelton == null)
        {
          lock (Grimnir.FadeTransitionState.mSingeltonLock)
          {
            if (Grimnir.FadeTransitionState.mSingelton == null)
              Grimnir.FadeTransitionState.mSingelton = new Grimnir.FadeTransitionState();
          }
        }
        return Grimnir.FadeTransitionState.mSingelton;
      }
    }

    private FadeTransitionState()
    {
    }

    public void OnEnter(Grimnir iOwner)
    {
      iOwner.mFadeIn = true;
      iOwner.mFadeTimer = 0.25f;
      iOwner.mTransitionBegun = false;
      iOwner.mDreamCompleteTimer = 1.5f;
      ++iOwner.mNextActionState;
    }

    public unsafe void OnUpdate(float iDeltaTime, Grimnir iOwner)
    {
      if (NetworkManager.Instance.State == NetworkState.Client)
        return;
      if (!iOwner.mTransitionBegun)
      {
        iOwner.mDreamCompleteTimer -= iDeltaTime;
        if ((double) iOwner.mDreamCompleteTimer > 0.0)
          return;
        RenderManager.Instance.BeginTransition(Transitions.Fade, Color.White, iOwner.mFadeTimer);
        iOwner.mTransitionBegun = true;
      }
      else if (iOwner.mFadeIn)
      {
        iOwner.mFadeTimer -= iDeltaTime;
        if ((double) iOwner.mFadeTimer - 0.20000000298023224 > 0.0)
          return;
        iOwner.mFadeIn = false;
        iOwner.mFadeTimer = 0.25f;
        int mNextActionState = (int) iOwner.mNextActionState;
        iOwner.TeleportPlayers(mNextActionState);
        Vector3 iPosition = iOwner.StateCameraTarget(iOwner.mNextActionState);
        iOwner.mPlayState.Camera.SetPosition(ref iPosition);
        if (NetworkManager.Instance.State == NetworkState.Server)
        {
          Grimnir.CameraMessage cameraMessage;
          cameraMessage.Position = iPosition;
          BossFight.Instance.SendMessage<Grimnir.CameraMessage>((IBoss) iOwner, (ushort) 4, (void*) &cameraMessage, true);
        }
        RenderManager.Instance.EndTransition(Transitions.Fade, Color.White, iOwner.mFadeTimer);
      }
      else
      {
        iOwner.mFadeTimer -= iDeltaTime;
        if ((double) iOwner.mFadeTimer - 0.20000000298023224 > 0.0)
          return;
        iOwner.mCurrentActionState = iOwner.mNextActionState;
        iOwner.ChangeState(Grimnir.States.Action);
      }
    }

    public void OnExit(Grimnir iOwner)
    {
    }
  }

  public enum MessageType : ushort
  {
    Update,
    Teleport,
    ChangeState,
    Spawn,
    Camera,
  }

  internal struct UpdateMessage
  {
    public const ushort TYPE = 0;
    public float HitPoints;
  }

  internal struct CameraMessage
  {
    public const ushort TYPE = 4;
    public Vector3 Position;
  }

  internal struct SpawnMessage
  {
    public const ushort TYPE = 3;
    public ushort Handle;
    public int TypeID;
    public Vector3 Position;
    public Vector3 Direction;
  }

  internal struct TeleportMessage
  {
    public const ushort TYPE = 1;
    public int Venue;
  }

  internal struct ChangeStateMessage
  {
    public const ushort TYPE = 2;
    public Grimnir.States State;
  }

  public enum ActionStates
  {
    Mountaindale,
    Highlands,
    Havindr,
    Mines,
    Swamp,
    Ruins,
    NrOfStates,
  }

  public enum States
  {
    IntroFade,
    OutroFade,
    Action,
    FadeTransition,
  }

  private enum Animations
  {
    Idle,
    NrOfAnimations,
  }
}
