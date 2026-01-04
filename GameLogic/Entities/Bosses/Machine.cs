// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Bosses.Machine
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.AI;
using Magicka.AI.AgentStates;
using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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

public class Machine : BossStatusEffected, IBoss
{
  private const float NETWORK_UPDATE_PERIOD = 0.0333333351f;
  private const int NPC_SPAWN_CAP = 2;
  private const float TOTAL_DURATION = 120f;
  private const float TOTAL_DIVISOR = 0.008333334f;
  private const float TIME_BETWEEN_SPAWNS = 1f;
  private const float TIME_BETWEEN_TAUNTS = 9f;
  private const float MAXHITPOINTS = 5000f;
  private const float DRILL_LENGTH = 6.2f;
  protected float mNetworkUpdateTimer;
  private float mDrillTime;
  private float mIdleTimer;
  private static readonly int MACHINE_BREAK_EFFECT = "machine_break".GetHashCodeCustom();
  private static readonly int[] MACHINE_SHRAPNEL_EFFECT = new int[3]
  {
    "machine_shrapnel_cog".GetHashCodeCustom(),
    "machine_shrapnel_plank".GetHashCodeCustom(),
    "machine_shrapnel_misc".GetHashCodeCustom()
  };
  private int mCurrentDialog;
  private static readonly int DIALOG_MACHINE_DEAD = "machinedead".GetHashCodeCustom();
  private static readonly int DIALOG_KING1 = "kingsaved1".GetHashCodeCustom();
  private static readonly int DIALOG_KING2 = "kingsaved2".GetHashCodeCustom();
  private static List<int> DIALOG_MACHINE;
  private static readonly int MACHINE_DEATH_TRIGGER_ID = "machine_destroyed".GetHashCodeCustom();
  private static readonly int BIPEDAL_DONE_TRIGGER_ID = "bipedal_done".GetHashCodeCustom();
  private static readonly int GUNTER_DEFEATED_TRIGGER_ID = "gunther_pissed".GetHashCodeCustom();
  private static readonly int GUNTER_DEAD_TRIGGER_ID = "gunther_dead".GetHashCodeCustom();
  private static readonly int BLOOD_EFFECT = "gore_splash_black".GetHashCodeCustom();
  private float mPeddleSpeed = 1f;
  private float mPeddleTargetSpeed = 1f;
  private float mEmperorLightningTimer;
  protected IBossState<Machine> mMachineState;
  protected IBossState<Machine> mCurrentStage;
  private Machine.RenderData[] mKingRenderData;
  private Machine.RenderData[] mMachineRenderData;
  private Machine.OrcRenderData[] mOrcRenderData;
  private Random mRandom;
  private List<NonPlayerCharacter> mNPCs;
  private NonPlayerCharacter mWarlock;
  private bool mDead;
  private BossDamageZone mMachineZone;
  private BossDamageZone mOrcZone;
  private PlayState mPlayState;
  private SkinnedModel mKingModel;
  private AnimationClip[] mKingClips;
  private AnimationController mKingController;
  private SkinnedModel mMachineModel;
  private AnimationClip[] mMachineClips;
  private AnimationController mMachineController;
  private AnimationClip[] mOrcClips;
  private AnimationController mOrcController;
  private int mDrillIndex;
  private Matrix mMachineOrientation;
  private Vector3 mBicyclePosition;
  private bool mRenderKing;
  private Matrix mKingSpawnOrientation;
  private Matrix mWarlockSpawnOrientation;
  private Matrix mWarlockEmperorOrientation;
  private AIEvent[] mWarlockIntroEvent;
  private AIEvent[] mWarlockFinalEvent;
  private AIEvent[] mWarlockEmperorEvent;
  private AIEvent[][] mAvatarMoveEvent;
  private AudioEmitter mOriginEmitter;
  private AudioEmitter mDestinationEmitter;
  private AudioEmitter mAudioEmitter;
  private AudioEmitter mGibAudioEmitter;
  private static readonly Random RANDOM = new Random();
  private HitList mShrapnelHitList;
  private Machine.RulerRenderData[] mRulerRenderData;
  private Shield mOrcStageShield;
  private int mDrillyIndex;
  private Matrix mDrillyBindPose;
  private float mRulerAlpha;
  private float mDamageFlashTimer;
  private static readonly Vector3 DRILLY_START = new Vector3(19.44f, 2.57f, -57.97f);
  private static readonly Vector3 DRILLY_END = new Vector3(21.36f, 1.7f, -58.5f);
  private static readonly float DISTANCE = Vector3.Distance(Machine.DRILLY_END, Machine.DRILLY_START);
  private static readonly Matrix INV_SCENE_ROTATION = Matrix.CreateRotationY(0.3455752f);
  private SkinnedModelDeferredAdvancedMaterial mMaterial;
  private TextureCube mIceCubeMap;
  private TextureCube mIceCubeNormalMap;
  private bool mNetworkInitialized;

  public unsafe Machine(PlayState iPlayState)
  {
    this.mAudioEmitter = new AudioEmitter();
    this.mGibAudioEmitter = new AudioEmitter();
    this.mRandom = new Random();
    this.mShrapnelHitList = new HitList(16 /*0x10*/);
    this.mPlayState = iPlayState;
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/boss_machine_warlock");
      this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/warlock");
      this.mMachineModel = this.mPlayState.Content.Load<SkinnedModel>("Models/Bosses/theMachine/themachine");
      this.mKingModel = this.mPlayState.Content.Load<SkinnedModel>("Models/Characters/Human/King_animation");
      this.mIceCubeMap = this.mPlayState.Content.Load<TextureCube>("EffectTextures/iceCube");
      this.mIceCubeNormalMap = this.mPlayState.Content.Load<TextureCube>("EffectTextures/iceCube_NRM");
    }
    Matrix result1;
    Matrix.CreateRotationY(3.14159274f, out result1);
    for (int index = 0; index < this.mMachineModel.SkeletonBones.Count; ++index)
    {
      SkinnedModelBone skeletonBone = this.mMachineModel.SkeletonBones[index];
      if (skeletonBone.Name.Equals("Platform", StringComparison.OrdinalIgnoreCase))
        this.mDrillIndex = (int) skeletonBone.Index;
      else if (skeletonBone.Name.Equals("BicycleRoot", StringComparison.OrdinalIgnoreCase))
      {
        Quaternion result2 = skeletonBone.BindPose.Orientation;
        Quaternion result3;
        Quaternion.CreateFromRotationMatrix(ref result1, out result3);
        Quaternion.Multiply(ref result2, ref result3, out result2);
        this.mBicyclePosition = skeletonBone.BindPose.Translation;
        Vector3.Transform(ref this.mBicyclePosition, ref result2, out this.mBicyclePosition);
      }
      else if (skeletonBone.Name.Equals("Drilly", StringComparison.OrdinalIgnoreCase))
      {
        this.mDrillyIndex = (int) skeletonBone.Index;
        Matrix result4 = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref result4, ref result1, out result4);
        Matrix.Invert(ref result4, out this.mDrillyBindPose);
      }
    }
    this.mMachineZone = new BossDamageZone(this.mPlayState, (IBoss) this, 0, 4f, (Primitive) new Box(new Vector3(-4.25f, 0.0f, -3.75f), Matrix.Identity, new Vector3(8.5f, 3f, 8f)));
    this.mOrcZone = new BossDamageZone(this.mPlayState, (IBoss) this, 1, 3f, (Primitive) new Box(new Vector3(-1.5f, -1f, -0.75f), Matrix.Identity, new Vector3(2.5f, 3.75f, 2.5f)));
    this.mOrcController = new AnimationController();
    this.mOrcController.Skeleton = this.mMachineModel.SkeletonBones;
    this.mOrcClips = new AnimationClip[4];
    this.mOrcClips[0] = this.mMachineModel.AnimationClips["pedal"];
    this.mOrcClips[1] = this.mMachineModel.AnimationClips["breakfree"];
    this.mOrcClips[2] = this.mMachineModel.AnimationClips["nod"];
    this.mOrcClips[3] = this.mMachineModel.AnimationClips["idle"];
    this.mMachineController = new AnimationController();
    this.mMachineController.Skeleton = this.mMachineModel.SkeletonBones;
    this.mMachineClips = new AnimationClip[3];
    this.mMachineClips[1] = this.mMachineModel.AnimationClips["idle"];
    this.mMachineClips[2] = this.mMachineModel.AnimationClips["breakfree"];
    this.mMachineClips[0] = this.mMachineModel.AnimationClips["pedal"];
    this.mKingController = new AnimationController();
    this.mKingController.Skeleton = this.mKingModel.SkeletonBones;
    this.mKingClips = new AnimationClip[5];
    this.mKingClips[0] = this.mKingModel.AnimationClips["sit_bound"];
    this.mKingClips[3] = this.mKingModel.AnimationClips["sit_bound"];
    this.mKingClips[4] = this.mKingModel.AnimationClips["king_vader"];
    this.mKingClips[1] = this.mKingModel.AnimationClips["talk_oldandweak"];
    this.mKingClips[2] = this.mKingModel.AnimationClips["talk_strikeatdawn"];
    ModelMesh mesh1 = this.mMachineModel.Model.Meshes[1];
    ModelMeshPart meshPart1 = mesh1.MeshParts[0];
    ModelMesh mesh2 = this.mMachineModel.Model.Meshes[0];
    ModelMeshPart meshPart2 = mesh2.MeshParts[1];
    ModelMeshPart meshPart3 = mesh2.MeshParts[0];
    ModelMesh mesh3 = this.mKingModel.Model.Meshes[0];
    ModelMeshPart meshPart4 = mesh3.MeshParts[0];
    SkinnedModelDeferredAdvancedMaterial oMaterial1;
    Helper.SkinnedModelDeferredMaterialFromBasicEffect(meshPart1.Effect as SkinnedModelBasicEffect, out oMaterial1);
    SkinnedModelDeferredAdvancedMaterial oMaterial2;
    Helper.SkinnedModelDeferredMaterialFromBasicEffect(meshPart4.Effect as SkinnedModelBasicEffect, out oMaterial2);
    Helper.SkinnedModelDeferredMaterialFromBasicEffect(meshPart2.Effect as SkinnedModelBasicEffect, out this.mMaterial);
    SkinnedModelDeferredAdvancedMaterial oMaterial3;
    Helper.SkinnedModelDeferredMaterialFromBasicEffect(meshPart3.Effect as SkinnedModelBasicEffect, out oMaterial3);
    this.mKingRenderData = new Machine.RenderData[3];
    this.mMachineRenderData = new Machine.RenderData[3];
    this.mOrcRenderData = new Machine.OrcRenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      this.mKingRenderData[index] = new Machine.RenderData();
      this.mKingRenderData[index].SetMesh(mesh3.VertexBuffer, mesh3.IndexBuffer, meshPart4, 0, 3, 4);
      this.mKingRenderData[index].mMaterial = oMaterial2;
      this.mMachineRenderData[index] = new Machine.RenderData();
      this.mMachineRenderData[index].SetMesh(mesh1.VertexBuffer, mesh1.IndexBuffer, meshPart1, 0, 3, 4);
      this.mMachineRenderData[index].mMaterial = oMaterial1;
      this.mOrcRenderData[index] = new Machine.OrcRenderData();
      this.mOrcRenderData[index].SetMesh(mesh2.VertexBuffer, mesh2.IndexBuffer, meshPart2, meshPart3, 0, 3, 4);
      this.mOrcRenderData[index].mMaterial = this.mMaterial;
      this.mOrcRenderData[index].mMaterial2 = oMaterial3;
    }
    this.mNPCs = new List<NonPlayerCharacter>(16 /*0x10*/);
    this.mOriginEmitter = new AudioEmitter();
    this.mDestinationEmitter = new AudioEmitter();
    for (int iIndex = 0; iIndex < this.mResistances.Length; ++iIndex)
    {
      this.mResistances[iIndex].ResistanceAgainst = Spell.ElementFromIndex(iIndex);
      this.mResistances[iIndex].Modifier = 0.0f;
      this.mResistances[iIndex].Multiplier = 1f;
    }
    int index1 = Spell.ElementIndex(Elements.Earth);
    this.mResistances[index1].Modifier = -200f;
    this.mResistances[index1].Multiplier = 0.75f;
    int index2 = Spell.ElementIndex(Elements.Arcane);
    this.mResistances[index2].Modifier = 0.0f;
    this.mResistances[index2].Multiplier = 0.5f;
    int index3 = Spell.ElementIndex(Elements.Lightning);
    this.mResistances[index3].Modifier = 0.0f;
    this.mResistances[index3].Multiplier = 0.5f;
    int index4 = Spell.ElementIndex(Elements.Fire);
    this.mResistances[index4].Modifier = 0.0f;
    this.mResistances[index4].Multiplier = 0.5f;
    Machine.DIALOG_MACHINE = new List<int>(3);
    Machine.DIALOG_MACHINE.Add("warlocktaunt1".GetHashCodeCustom());
    Machine.DIALOG_MACHINE.Add("warlocktaunt2".GetHashCodeCustom());
    Machine.DIALOG_MACHINE.Add("warlocktaunt3".GetHashCodeCustom());
    Machine.IdleState instance1 = Machine.IdleState.Instance;
    Machine.DrillState instance2 = Machine.DrillState.Instance;
    Machine.KillKingState instance3 = Machine.KillKingState.Instance;
    Machine.BrokenState instance4 = Machine.BrokenState.Instance;
    Machine.IntroStage instance5 = Machine.IntroStage.Instance;
    Machine.PreGunterStage instance6 = Machine.PreGunterStage.Instance;
    Machine.WarlockStage instance7 = Machine.WarlockStage.Instance;
    Machine.FinalStage instance8 = Machine.FinalStage.Instance;
    if (NetworkManager.Instance.State != NetworkState.Client)
    {
      this.mWarlock = NonPlayerCharacter.GetInstance(this.mPlayState);
      if (NetworkManager.Instance.State == NetworkState.Server)
      {
        Machine.InitializeMessage initializeMessage;
        initializeMessage.Handle = this.mWarlock.Handle;
        BossFight.Instance.SendInitializeMessage<Machine.InitializeMessage>((IBoss) this, (ushort) 0, (void*) &initializeMessage);
      }
    }
    this.mNetworkInitialized = NetworkManager.Instance.State != NetworkState.Client;
    Texture2D texture2D = this.mPlayState.Content.Load<Texture2D>("UI/Boss/MachineRuler");
    VertexPositionTexture[] vertexPositionTextureArray = new VertexPositionTexture[8];
    vertexPositionTextureArray[0].TextureCoordinate = new Vector2(0.0f, 1f);
    vertexPositionTextureArray[1].TextureCoordinate = new Vector2(0.0f, 0.0f);
    vertexPositionTextureArray[2].TextureCoordinate = new Vector2(0.333333343f, 1f);
    vertexPositionTextureArray[3].TextureCoordinate = new Vector2(0.333333343f, 0.0f);
    vertexPositionTextureArray[4].TextureCoordinate = new Vector2(0.6666667f, 1f);
    vertexPositionTextureArray[5].TextureCoordinate = new Vector2(0.6666667f, 0.0f);
    vertexPositionTextureArray[6].TextureCoordinate = new Vector2(1f, 1f);
    vertexPositionTextureArray[7].TextureCoordinate = new Vector2(1f, 0.0f);
    VertexDeclaration vertexDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
    BasicEffect basicEffect = new BasicEffect(Magicka.Game.Instance.GraphicsDevice, (EffectPool) null);
    basicEffect.Alpha = 1f;
    basicEffect.DiffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
    basicEffect.VertexColorEnabled = false;
    basicEffect.Texture = texture2D;
    GUIBasicEffect guiBasicEffect = new GUIBasicEffect(Magicka.Game.Instance.GraphicsDevice, (EffectPool) null);
    guiBasicEffect.TextureEnabled = false;
    guiBasicEffect.Color = new Vector4(1f, 1f, 1f, 1f);
    guiBasicEffect.ScaleToHDR = true;
    guiBasicEffect.VertexColorEnabled = true;
    this.mRulerRenderData = new Machine.RulerRenderData[3];
    for (int index5 = 0; index5 < 3; ++index5)
    {
      this.mRulerRenderData[index5] = new Machine.RulerRenderData();
      this.mRulerRenderData[index5].Effect = basicEffect;
      this.mRulerRenderData[index5].TextEffect = guiBasicEffect;
      this.mRulerRenderData[index5].Vertices = vertexPositionTextureArray;
      this.mRulerRenderData[index5].Alpha = 0.0f;
      this.mRulerRenderData[index5].VertexDeclaration = vertexDeclaration;
    }
  }

  public void Initialize(ref Matrix iOrientation, int iUniqueID)
  {
    this.Initialize(ref iOrientation);
  }

  public void Initialize(ref Matrix iOrientation)
  {
    this.mRenderKing = true;
    this.mDead = false;
    this.mMaxHitPoints = 5000f;
    this.mHitPoints = 5000f;
    this.mIdleTimer = 0.0f;
    this.mDrillTime = 0.0f;
    this.mPeddleSpeed = 1f;
    this.mPeddleTargetSpeed = 1f;
    this.mMachineOrientation = iOrientation;
    this.mOrcStageShield = (Shield) null;
    Matrix oLocator1;
    this.mPlayState.Level.CurrentScene.GetLocator("spawn_warlock".GetHashCodeCustom(), out oLocator1);
    CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate("boss_machine_warlock".GetHashCodeCustom());
    Vector3 iPosition = oLocator1.Translation;
    Segment seg = new Segment();
    seg.Origin = iPosition;
    ++seg.Origin.Y;
    seg.Delta.Y -= 5f;
    Vector3 pos;
    if (this.mPlayState.Level.CurrentScene.CollisionSkin.SegmentIntersect(out float _, out pos, out Vector3 _, seg))
      iPosition = pos;
    iPosition.Y += (float) ((double) cachedTemplate.Radius + (double) cachedTemplate.Length * 0.5 + 0.10000000149011612);
    this.mWarlock.Initialize(cachedTemplate, iPosition, "boss_machine_warlock".GetHashCodeCustom());
    this.mWarlock.AI.SetOrder(Order.Idle, ReactTo.None, Order.Flee, 0, 0, 0, (AIEvent[]) null);
    this.mWarlock.CharacterBody.DesiredDirection = oLocator1.Forward;
    this.mPlayState.EntityManager.AddEntity((Magicka.GameLogic.Entities.Entity) this.mWarlock);
    this.mWarlock.CannotDieWithoutExplicitKill = true;
    this.mWarlock.SetImmortalTime(float.PositiveInfinity);
    this.mWarlock.AI.SetOrder(Order.Idle, ReactTo.None, Order.Idle, 0, 0, 0, (AIEvent[]) null);
    this.mMachineZone.Initialize();
    this.mMachineZone.Body.Immovable = true;
    this.mMachineZone.Body.MoveTo(this.mMachineOrientation.Translation, Machine.INV_SCENE_ROTATION);
    this.mMachineZone.Body.CollisionSkin.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
    this.mPlayState.EntityManager.AddEntity((Magicka.GameLogic.Entities.Entity) this.mMachineZone);
    this.mOrcZone.Initialize();
    this.mOrcZone.Body.Immovable = true;
    this.mOrcZone.Body.MoveTo(this.mMachineOrientation.Translation + this.mBicyclePosition, Machine.INV_SCENE_ROTATION);
    this.mOrcZone.Body.CollisionSkin.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
    this.mOrcZone.Body.CollisionSkin.NonCollidables.Add(this.mMachineZone.Body.CollisionSkin);
    this.mPlayState.EntityManager.AddEntity((Magicka.GameLogic.Entities.Entity) this.mOrcZone);
    this.mAudioEmitter.Position = this.mOrcZone.Position;
    this.mAudioEmitter.Forward = Vector3.Right;
    this.mAudioEmitter.Up = Vector3.Up;
    this.mOrcController.Speed = 1f;
    this.mMachineController.Speed = 1f;
    this.mPeddleSpeed = 1f;
    this.mPeddleTargetSpeed = 1f;
    this.mCurrentDialog = 0;
    MoveEvent moveEvent1 = new MoveEvent();
    Locator oLocator2;
    this.mPlayState.Level.CurrentScene.GetLocator("spawn_king".GetHashCodeCustom(), out oLocator2);
    this.mKingSpawnOrientation = oLocator2.Transform;
    MagickaMath.UniformMatrixScale(ref this.mKingSpawnOrientation, 1.3f);
    this.mPlayState.Level.CurrentScene.GetLocator("spawn_warlock".GetHashCodeCustom(), out oLocator2);
    this.mWarlockSpawnOrientation = oLocator2.Transform;
    this.mPlayState.Level.CurrentScene.GetLocator("teleport_warlock".GetHashCodeCustom(), out oLocator2);
    this.mWarlockEmperorOrientation = oLocator2.Transform;
    this.mNPCs.Clear();
    this.mWarlockIntroEvent = new AIEvent[1];
    MoveEvent moveEvent2 = new MoveEvent()
    {
      Direction = this.mKingSpawnOrientation.Translation - this.mWarlockSpawnOrientation.Translation
    };
    moveEvent2.Direction.Y = 0.0f;
    moveEvent2.Direction.Normalize();
    moveEvent2.Delay = 0.0f;
    moveEvent2.FixedDirection = true;
    moveEvent2.Waypoint = this.mWarlockSpawnOrientation.Translation;
    this.mWarlockIntroEvent[0].EventType = AIEventType.Move;
    this.mWarlockIntroEvent[0].MoveEvent = moveEvent2;
    this.mWarlockFinalEvent = new AIEvent[1];
    moveEvent2 = new MoveEvent();
    moveEvent2.Waypoint = this.mWarlockSpawnOrientation.Translation;
    Vector3 vector3 = (this.mKingSpawnOrientation.Translation - this.mWarlockSpawnOrientation.Translation) with
    {
      Y = 0.0f
    };
    vector3.Normalize();
    moveEvent2.Direction = vector3;
    moveEvent2.FixedDirection = true;
    this.mWarlockFinalEvent[0].EventType = AIEventType.Move;
    this.mWarlockFinalEvent[0].MoveEvent = moveEvent2;
    this.mWarlockEmperorEvent = new AIEvent[2];
    this.mWarlockEmperorEvent[0].AnimationEvent.Animation = Magicka.Animations.spec_action0;
    this.mWarlockEmperorEvent[0].AnimationEvent.BlendTime = 0.01f;
    this.mWarlockEmperorEvent[0].AnimationEvent.Delay = 0.15f;
    this.mWarlockEmperorEvent[0].EventType = AIEventType.Animation;
    this.mWarlockEmperorEvent[1].AnimationEvent.Animation = Magicka.Animations.spec_action0;
    this.mWarlockEmperorEvent[1].AnimationEvent.BlendTime = 0.01f;
    this.mWarlockEmperorEvent[1].AnimationEvent.Delay = 0.15f;
    this.mWarlockEmperorEvent[1].EventType = AIEventType.Animation;
    this.mAvatarMoveEvent = new AIEvent[4][];
    this.mAvatarMoveEvent[0] = new AIEvent[1];
    moveEvent2.Speed = 1f;
    moveEvent2.FixedDirection = true;
    moveEvent2.Waypoint = this.mKingSpawnOrientation.Translation + Vector3.Transform(new Vector3(-3f, 1f, 8f), Machine.INV_SCENE_ROTATION);
    moveEvent2.Direction = Vector3.Subtract(this.mWarlockSpawnOrientation.Translation, moveEvent2.Waypoint);
    moveEvent2.Direction.Normalize();
    this.mAvatarMoveEvent[0][0].EventType = AIEventType.Move;
    this.mAvatarMoveEvent[0][0].MoveEvent = moveEvent2;
    this.mAvatarMoveEvent[1] = new AIEvent[1];
    moveEvent2.Speed = 1f;
    moveEvent2.FixedDirection = true;
    moveEvent2.Waypoint = this.mKingSpawnOrientation.Translation + Vector3.Transform(new Vector3(-1f, 1f, 8f), Machine.INV_SCENE_ROTATION);
    moveEvent2.Direction = Vector3.Subtract(this.mWarlockSpawnOrientation.Translation, moveEvent2.Waypoint);
    moveEvent2.Direction.Normalize();
    this.mAvatarMoveEvent[1][0].EventType = AIEventType.Move;
    this.mAvatarMoveEvent[1][0].MoveEvent = moveEvent2;
    this.mAvatarMoveEvent[2] = new AIEvent[1];
    moveEvent2.Speed = 1f;
    moveEvent2.FixedDirection = true;
    moveEvent2.Waypoint = this.mKingSpawnOrientation.Translation + Vector3.Transform(new Vector3(1f, 1f, 8f), Machine.INV_SCENE_ROTATION);
    moveEvent2.Direction = Vector3.Subtract(this.mWarlockSpawnOrientation.Translation, moveEvent2.Waypoint);
    moveEvent2.Direction.Normalize();
    this.mAvatarMoveEvent[2][0].EventType = AIEventType.Move;
    this.mAvatarMoveEvent[2][0].MoveEvent = moveEvent2;
    this.mAvatarMoveEvent[3] = new AIEvent[1];
    moveEvent2.Speed = 1f;
    moveEvent2.FixedDirection = true;
    moveEvent2.Waypoint = this.mKingSpawnOrientation.Translation + Vector3.Transform(new Vector3(3f, 1f, 8f), Machine.INV_SCENE_ROTATION);
    moveEvent2.Direction = Vector3.Subtract(this.mWarlockSpawnOrientation.Translation, moveEvent2.Waypoint);
    moveEvent2.Direction.Normalize();
    this.mAvatarMoveEvent[3][0].EventType = AIEventType.Move;
    this.mAvatarMoveEvent[3][0].MoveEvent = moveEvent2;
    for (int index = 0; index < this.mStatusEffects.Length; ++index)
      this.mStatusEffects[index].Stop();
    this.mCurrentStage = (IBossState<Machine>) Machine.IntroStage.Instance;
    this.mCurrentStage.OnEnter(this);
    this.mMachineState = (IBossState<Machine>) Machine.IdleState.Instance;
    this.mMachineState.OnEnter(this);
    for (int index = 0; index < 3; ++index)
    {
      this.mKingRenderData[index].mBoundingSphere = new BoundingSphere(this.mKingSpawnOrientation.Translation, 10f);
      this.mMachineRenderData[index].mBoundingSphere = new BoundingSphere(this.mMachineOrientation.Translation, 12f);
      this.mOrcRenderData[index].mBoundingSphere = new BoundingSphere(this.mOrcZone.Position, 5f);
      this.mRulerRenderData[index].Alpha = 0.0f;
    }
    this.mRulerAlpha = 0.0f;
    this.mDamageFlashTimer = 0.0f;
  }

  public void DeInitialize()
  {
    this.mOrcStageShield.Kill();
    this.mOrcStageShield = (Shield) null;
    this.mNPCs.Clear();
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
    if (iFightStarted && this.mCurrentStage is Machine.IntroStage)
    {
      this.ChangeMachineState(Machine.States.Drill);
      this.ChangeStage(Machine.States.PreGunter);
    }
    this.mCurrentStage.OnUpdate(iDeltaTime, this);
    this.mMachineState.OnUpdate(iDeltaTime, this);
    this.mDamageFlashTimer = Math.Max(this.mDamageFlashTimer - iDeltaTime, 0.0f);
    this.mPeddleSpeed += (float) (((double) this.mPeddleTargetSpeed - (double) this.mPeddleSpeed) * (double) iDeltaTime * 4.0);
    if (this.HasStatus(1, StatusEffects.Burning))
      this.mPeddleTargetSpeed += (float) ((3.0 - (double) this.mPeddleTargetSpeed) * (double) iDeltaTime * 4.0);
    else if (this.HasStatus(1, StatusEffects.Frozen))
    {
      this.mPeddleSpeed = 0.0f;
      this.mPeddleTargetSpeed = 0.0f;
    }
    else if (this.HasStatus(1, StatusEffects.Cold))
      this.mPeddleTargetSpeed += (1f - this.StatusMagnitude(1, StatusEffects.Cold) - this.mPeddleTargetSpeed) * iDeltaTime;
    else
      this.mPeddleTargetSpeed += (float) ((1.0 - (double) this.mPeddleTargetSpeed) * (double) iDeltaTime * 0.25);
    this.mShrapnelHitList.Update(iDeltaTime);
    this.UpdateStatusEffects(iDeltaTime);
    this.UpdateDamage(iDeltaTime);
    this.mMachineRenderData[(int) iDataChannel].Flash = this.mDamageFlashTimer * 10f;
    this.mMachineController.PreUpdate(iDeltaTime, ref this.mMachineOrientation);
    Pose[] localBonePoses = this.mMachineController.LocalBonePoses;
    Vector3 result1 = new Vector3(0.0f, 0.0f, this.mDrillTime * 0.008333334f * Machine.DISTANCE);
    Quaternion orientation = localBonePoses[this.mDrillIndex].Orientation;
    Vector3.Transform(ref result1, ref orientation, out result1);
    Vector3.Add(ref result1, ref localBonePoses[this.mDrillIndex].Translation, out localBonePoses[this.mDrillIndex].Translation);
    this.mMachineController.UpdateAbsoluteBoneTransforms(ref this.mMachineOrientation);
    this.mMachineController.SkinnedBoneTransforms.CopyTo((Array) this.mMachineRenderData[(int) iDataChannel].mSkeleton, 0);
    this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) this.mMachineRenderData[(int) iDataChannel]);
    if (this.HasStatus(1, StatusEffects.Frozen))
    {
      this.mOrcRenderData[(int) iDataChannel].mMaterial.CubeMapRotation = Matrix.Identity;
      this.mOrcRenderData[(int) iDataChannel].mMaterial.Bloat = 0.1f;
      this.mOrcRenderData[(int) iDataChannel].mMaterial.EmissiveAmount = 2f;
      this.mOrcRenderData[(int) iDataChannel].mMaterial.SpecularBias = 0.8f;
      this.mOrcRenderData[(int) iDataChannel].mMaterial.SpecularPower = 20f;
      this.mOrcRenderData[(int) iDataChannel].mMaterial.CubeMapEnabled = true;
      this.mOrcRenderData[(int) iDataChannel].mMaterial.CubeNormalMapEnabled = true;
      this.mOrcRenderData[(int) iDataChannel].mMaterial.CubeMap = this.mIceCubeMap;
      this.mOrcRenderData[(int) iDataChannel].mMaterial.CubeNormalMap = this.mIceCubeNormalMap;
      this.mOrcRenderData[(int) iDataChannel].mMaterial.CubeMapColor.X = this.mOrcRenderData[(int) iDataChannel].mMaterial.CubeMapColor.Y = this.mOrcRenderData[(int) iDataChannel].mMaterial.CubeMapColor.Z = 1f;
      this.mOrcRenderData[(int) iDataChannel].mMaterial.CubeMapColor.W = 1f - (float) Math.Pow(0.20000000298023224, (double) this.StatusMagnitude(1, StatusEffects.Frozen));
    }
    else
    {
      this.mOrcRenderData[(int) iDataChannel].mMaterial.Bloat = 0.0f;
      this.mOrcRenderData[(int) iDataChannel].mMaterial.EmissiveAmount = this.mMaterial.EmissiveAmount;
      this.mOrcRenderData[(int) iDataChannel].mMaterial.SpecularBias = this.mMaterial.SpecularBias;
      this.mOrcRenderData[(int) iDataChannel].mMaterial.SpecularPower = this.mMaterial.SpecularPower;
      this.mOrcRenderData[(int) iDataChannel].mMaterial.CubeMapEnabled = false;
      this.mOrcRenderData[(int) iDataChannel].mMaterial.CubeNormalMapEnabled = false;
    }
    this.mOrcRenderData[(int) iDataChannel].Flash = this.mDamageFlashTimer * 10f;
    this.mOrcController.Update(iDeltaTime, ref this.mMachineOrientation, true);
    this.mOrcController.SkinnedBoneTransforms.CopyTo((Array) this.mOrcRenderData[(int) iDataChannel].mSkeleton, 0);
    this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) this.mOrcRenderData[(int) iDataChannel]);
    if (this.mRenderKing)
    {
      this.mKingController.Update(iDeltaTime, ref this.mKingSpawnOrientation, true);
      this.mKingController.SkinnedBoneTransforms.CopyTo((Array) this.mKingRenderData[(int) iDataChannel].mSkeleton, 0);
      this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) this.mKingRenderData[(int) iDataChannel]);
    }
    for (int index = 0; index < this.mNPCs.Count; ++index)
    {
      if (this.mNPCs[index].Dead)
        this.mNPCs.RemoveAt(index--);
    }
    Matrix result2 = this.mDrillyBindPose;
    Matrix.Multiply(ref result2, ref this.mMachineController.SkinnedBoneTransforms[this.mDrillyIndex], out result2);
    Vector3 result3 = result2.Translation;
    Vector3 result4 = result2.Right;
    Vector3.Multiply(ref result4, 6.2f, out result4);
    Vector3.Add(ref result4, ref result3, out result3);
    result2.Translation = result3;
    this.mRulerRenderData[(int) iDataChannel].Transform = result2;
    this.mRulerRenderData[(int) iDataChannel].Time = this.mDrillTime;
    this.mRulerRenderData[(int) iDataChannel].Alpha = Math.Min(Math.Max(this.mRulerAlpha, 0.0f), 1f);
    this.mPlayState.Scene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) this.mRulerRenderData[(int) iDataChannel]);
  }

  private IBossState<Machine> GetState(Machine.States iState)
  {
    switch (iState)
    {
      case Machine.States.Idle:
        return (IBossState<Machine>) Machine.IdleState.Instance;
      case Machine.States.Drill:
        return (IBossState<Machine>) Machine.DrillState.Instance;
      case Machine.States.Broken:
        return (IBossState<Machine>) Machine.BrokenState.Instance;
      case Machine.States.KillKing:
        return (IBossState<Machine>) Machine.KillKingState.Instance;
      case Machine.States.Intro:
        return (IBossState<Machine>) Machine.IntroStage.Instance;
      case Machine.States.PreGunter:
        return (IBossState<Machine>) Machine.PreGunterStage.Instance;
      case Machine.States.Warlock:
        return (IBossState<Machine>) Machine.WarlockStage.Instance;
      case Machine.States.Final:
        return (IBossState<Machine>) Machine.FinalStage.Instance;
      case Machine.States.PostFinal:
        return (IBossState<Machine>) Machine.PostFinalStage.Instance;
      case Machine.States.KingKilled:
        return (IBossState<Machine>) Machine.KingKilledStage.Instance;
      default:
        return (IBossState<Machine>) null;
    }
  }

  protected unsafe void ChangeMachineState(Machine.States iState)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    if (NetworkManager.Instance.State == NetworkState.Server)
    {
      Machine.ChangeMachineStateMessage machineStateMessage;
      machineStateMessage.NewState = iState;
      BossFight.Instance.SendMessage<Machine.ChangeMachineStateMessage>((IBoss) this, (ushort) 2, (void*) &machineStateMessage, true);
    }
    this.mMachineState.OnExit(this);
    this.mMachineState = this.GetState(iState);
    this.mMachineState.OnEnter(this);
  }

  protected unsafe void ChangeStage(Machine.States iState)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    if (NetworkManager.Instance.State == NetworkState.Server)
    {
      Machine.ChangeFightStateMessage fightStateMessage;
      fightStateMessage.NewState = iState;
      BossFight.Instance.SendMessage<Machine.ChangeFightStateMessage>((IBoss) this, (ushort) 3, (void*) &fightStateMessage, true);
    }
    this.mCurrentStage.OnExit(this);
    this.mCurrentStage = this.GetState(iState);
    this.mCurrentStage.OnEnter(this);
  }

  public unsafe void TeleportWarlock(Matrix iOrientation)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    Vector3 position = this.mWarlock.Position;
    Vector3 iDirection = this.mWarlock.Direction;
    VisualEffectReference oRef;
    EffectManager.Instance.StartEffect(Teleport.TELEPORT_EFFECT_DISAPPEAR, ref position, ref iDirection, out oRef);
    Cue cue1 = AudioManager.Instance.GetCue(Banks.Spells, Teleport.TELEPORT_SOUND_ORIGIN);
    this.mOriginEmitter.Up = Vector3.Up;
    this.mOriginEmitter.Position = position;
    this.mOriginEmitter.Forward = iDirection;
    cue1.Apply3D(this.mPlayState.Camera.Listener, this.mOriginEmitter);
    cue1.Play();
    Matrix orientation = iOrientation;
    Vector3 iPosition = orientation.Translation;
    iDirection = orientation.Forward;
    Vector3 pos;
    if (this.mPlayState.Level.CurrentScene.CollisionSkin.SegmentIntersect(out float _, out pos, out Vector3 _, new Segment()
    {
      Origin = iPosition + Vector3.Up,
      Delta = {
        Y = -3f
      }
    }))
    {
      iPosition = pos;
      iPosition.Y += 0.1f;
    }
    iPosition.Y += this.mWarlock.Capsule.Length * 0.5f + this.mWarlock.Capsule.Radius;
    EffectManager.Instance.StartEffect(Teleport.TELEPORT_EFFECT_APPEAR, ref iPosition, ref iDirection, out oRef);
    Cue cue2 = AudioManager.Instance.GetCue(Banks.Spells, Teleport.TELEPORT_SOUND_DESTINATION);
    this.mDestinationEmitter.Up = Vector3.Up;
    this.mDestinationEmitter.Position = iPosition;
    this.mDestinationEmitter.Forward = iDirection;
    cue2.Apply3D(this.mPlayState.Camera.Listener, this.mDestinationEmitter);
    cue2.Play();
    orientation.Translation = new Vector3();
    this.mWarlock.CharacterBody.DesiredDirection = iOrientation.Forward;
    this.mWarlock.StopStatusEffects(StatusEffects.Burning);
    this.mWarlock.StopStatusEffects(StatusEffects.Cold);
    this.mWarlock.CharacterBody.Force = new Vector3();
    this.mWarlock.CharacterBody.Velocity = new Vector3();
    this.mWarlock.CharacterBody.Movement = new Vector3();
    this.mWarlock.Body.MoveTo(iPosition, orientation);
    if (NetworkManager.Instance.State != NetworkState.Server)
      return;
    Machine.TeleportWarlockMessage teleportWarlockMessage;
    teleportWarlockMessage.Position = iPosition;
    teleportWarlockMessage.Direction = iDirection;
    BossFight.Instance.SendMessage<Machine.TeleportWarlockMessage>((IBoss) this, (ushort) 5, (void*) &teleportWarlockMessage, true);
  }

  public void OverKillKing()
  {
    Matrix result1 = this.mDrillyBindPose;
    Matrix.Multiply(ref result1, ref this.mMachineController.SkinnedBoneTransforms[this.mDrillyIndex], out result1);
    Vector3 translation1 = result1.Translation;
    Vector3 result2 = result1.Right;
    Vector3.Multiply(ref result2, 6.2f, out result2);
    Vector3.Add(ref result2, ref translation1, out translation1);
    result1.Translation = translation1;
    BloodType iBloodType = BloodType.wood;
    Vector3 translation2;
    for (int index = 0; index < this.mWarlock.Gibs.Count; ++index)
    {
      Gib fromCache = Gib.GetFromCache();
      translation2.X = result1.Translation.X + (float) this.mRandom.NextDouble();
      translation2.Y = result1.Translation.Y + (float) this.mRandom.NextDouble();
      translation2.Z = result1.Translation.Z + (float) this.mRandom.NextDouble();
      fromCache.Initialize(this.mWarlock.Gibs[index].mModel, this.mWarlock.Gibs[index].mMass, this.mWarlock.Gibs[index].mScale, translation2, this.mWarlock.Body.Velocity * 0.1f + new Vector3((float) this.mRandom.NextDouble() * 3f, (float) this.mRandom.NextDouble() * 10f, (float) MagickaMath.Random.NextDouble() * 3f), 15f, (Magicka.GameLogic.Entities.Entity) null, iBloodType, Gib.GORE_GIB_TRAIL_EFFECTS[(int) iBloodType], this.HasStatus(1, StatusEffects.Frozen));
      fromCache.Body.ApplyBodyAngImpulse(new Vector3((float) this.mRandom.NextDouble() * 25f, (float) this.mRandom.NextDouble() * 25f, (float) this.mRandom.NextDouble() * 25f));
      this.mPlayState.EntityManager.AddEntity((Magicka.GameLogic.Entities.Entity) fromCache);
    }
    Vector3 forward = result1.Forward;
    translation2 = result1.Translation;
    EffectManager.Instance.StartEffect(Gib.GORE_GIB_MEDIUM_EFFECTS[(int) iBloodType], ref translation2, ref forward, out VisualEffectReference _);
    Vector3 oNrm;
    AnimatedLevelPart oAnimatedLevelPart;
    if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out translation1, out oNrm, out oAnimatedLevelPart, new Segment()
    {
      Origin = translation2 + new Vector3((float) this.mRandom.NextDouble(), 0.0f, (float) MagickaMath.Random.NextDouble()),
      Delta = {
        Y = -2f
      }
    }))
      DecalManager.Instance.AddAlphaBlendedDecal((Decal) ((int) iBloodType * 4), oAnimatedLevelPart, 4f, ref translation1, ref oNrm, 60f);
    this.mGibAudioEmitter.Position = result1.Translation;
    this.mGibAudioEmitter.Forward = result1.Forward;
    this.mGibAudioEmitter.Up = Vector3.Up;
    AudioManager.Instance.PlayCue(Banks.Misc, "misc_gib".GetHashCodeCustom(), this.mGibAudioEmitter);
    this.mPlayState.Camera.CameraShake(1f, 1f);
    this.mRenderKing = false;
  }

  public DamageResult Damage(
    int iPartIndex,
    Magicka.GameLogic.Damage iDamage,
    Magicka.GameLogic.Entities.Entity iAttacker,
    ref Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
    if (this.mMachineState is Machine.BrokenState)
      return DamageResult.None;
    if (iPartIndex != 1 && (iDamage.AttackProperty & AttackProperties.Status) == AttackProperties.Status)
    {
      iDamage.AttackProperty &= ~AttackProperties.Status;
      if (iDamage.AttackProperty == (AttackProperties) 0)
        return DamageResult.None;
    }
    DamageResult damageResult = this.Damage(iDamage, iAttacker, iAttackPosition, iFeatures);
    if ((double) this.mHitPoints <= 0.0)
      this.ChangeMachineState(Machine.States.Broken);
    return damageResult;
  }

  public void Damage(int iPartIndex, float iDamage, Elements iElement)
  {
    if (!(this.mCurrentStage is Machine.IntroStage) && !(this.mCurrentStage is Machine.PreGunterStage))
      return;
    this.Damage(iDamage, iElement);
  }

  public unsafe void ScriptMessage(BossMessages iMessage)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    if (NetworkManager.Instance.State == NetworkState.Server)
    {
      Machine.ScriptMessageMessage scriptMessageMessage;
      scriptMessageMessage.Message = iMessage;
      BossFight.Instance.SendMessage<Machine.ScriptMessageMessage>((IBoss) this, (ushort) 4, (void*) &scriptMessageMessage, true);
    }
    switch (iMessage)
    {
      case BossMessages.OrcBipedal:
        this.mOrcController.Speed = 1f;
        this.mOrcController.CrossFade(this.mOrcClips[1], 0.25f, false);
        break;
      case BossMessages.KingTalk1:
        this.mKingController.CrossFade(this.mKingClips[1], 0.25f, false);
        break;
      case BossMessages.KingTalk2:
        this.mKingController.CrossFade(this.mKingClips[2], 0.25f, false);
        break;
    }
  }

  public void SetSlow(int iIndex)
  {
  }

  public void GetRandomPositionOnCollisionSkin(int iIndex, out Vector3 oPosition)
  {
    throw new NotImplementedException();
  }

  public bool HasStatus(int iIndex, StatusEffects iStatus) => this.HasStatus(iStatus);

  public float StatusMagnitude(int iIndex, StatusEffects iStatus) => this.StatusMagnitude(iStatus);

  public bool AddImpulseVelocity(
    Vector3 iDirection,
    float iElevation,
    float iMassPower,
    float iDistance)
  {
    return false;
  }

  public override bool Dead => this.mDead;

  public float MaxHitPoints
  {
    get
    {
      if (this.mCurrentStage is Machine.IntroStage || this.mCurrentStage is Machine.PreGunterStage)
        return 5000f;
      return !(this.mCurrentStage is Machine.PostFinalStage) ? this.mWarlock.MaxHitPoints : 1f;
    }
  }

  public float HitPoints
  {
    get
    {
      if (this.mCurrentStage is Machine.IntroStage || this.mCurrentStage is Machine.PreGunterStage)
        return Math.Max(this.mHitPoints, 0.0f) + 1f;
      return !(this.mCurrentStage is Machine.PostFinalStage) ? Math.Max(this.mWarlock.HitPoints, 0.0f) + 1f : 0.0f;
    }
  }

  protected override BossDamageZone Entity => this.mOrcZone;

  protected override float Radius => 3f;

  protected override float Length => 1f;

  protected override int BloodEffect => Machine.BLOOD_EFFECT;

  protected override Vector3 NotifierTextPostion
  {
    get
    {
      Vector3 position = this.mOrcZone.Position;
      position.Y += 2f;
      return position;
    }
  }

  private unsafe void NetworkUpdate()
  {
    if (!(NetworkManager.Instance.Interface is NetworkServer networkServer))
      return;
    Machine.UpdateMessage updateMessage = new Machine.UpdateMessage();
    updateMessage.Animation = (byte) 0;
    while ((int) updateMessage.Animation < this.mMachineClips.Length && this.mMachineController.AnimationClip != this.mMachineClips[(int) updateMessage.Animation])
      ++updateMessage.Animation;
    updateMessage.AnimationTime = this.mMachineController.Time;
    updateMessage.Hitpoints = this.mHitPoints;
    for (int index = 0; index < networkServer.Connections; ++index)
    {
      float num = networkServer.GetLatency(index) * 0.5f;
      updateMessage.AnimationTime += num;
      BossFight.Instance.SendMessage<Machine.UpdateMessage>((IBoss) this, (ushort) 1, (void*) &updateMessage, false, index);
    }
  }

  public unsafe void NetworkUpdate(ref BossUpdateMessage iMsg)
  {
    switch (iMsg.Type)
    {
      case 1:
        Machine.UpdateMessage updateMessage;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &updateMessage);
        if (this.mMachineController.AnimationClip != this.mMachineClips[(int) updateMessage.Animation])
          this.mMachineController.StartClip(this.mMachineClips[(int) updateMessage.Animation], false);
        this.mMachineController.Time = updateMessage.AnimationTime;
        this.mHitPoints = updateMessage.Hitpoints;
        break;
      case 2:
        Machine.ChangeMachineStateMessage machineStateMessage;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &machineStateMessage);
        this.mMachineState.OnExit(this);
        this.mMachineState = this.GetState(machineStateMessage.NewState);
        this.mMachineState.OnEnter(this);
        break;
      case 3:
        Machine.ChangeFightStateMessage fightStateMessage;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &fightStateMessage);
        this.mCurrentStage.OnExit(this);
        this.mCurrentStage = this.GetState(fightStateMessage.NewState);
        this.mCurrentStage.OnEnter(this);
        break;
      case 4:
        Machine.ScriptMessageMessage scriptMessageMessage;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &scriptMessageMessage);
        switch (scriptMessageMessage.Message)
        {
          case BossMessages.OrcBipedal:
            this.mOrcController.Speed = 1f;
            this.mOrcController.CrossFade(this.mOrcClips[1], 0.25f, false);
            return;
          case BossMessages.GunterFight:
            return;
          case BossMessages.KingTalk1:
            this.mKingController.CrossFade(this.mKingClips[1], 0.25f, false);
            return;
          case BossMessages.KingTalk2:
            this.mKingController.CrossFade(this.mKingClips[2], 0.25f, false);
            return;
          default:
            throw new Exception($"Incorrect message, {(object) scriptMessageMessage.Message} passed to {this.GetType().Name}");
        }
      case 5:
        Machine.TeleportWarlockMessage teleportWarlockMessage;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &teleportWarlockMessage);
        Vector3 position = this.mWarlock.Position;
        Vector3 direction = this.mWarlock.Direction;
        VisualEffectReference oRef;
        EffectManager.Instance.StartEffect(Teleport.TELEPORT_EFFECT_DISAPPEAR, ref position, ref direction, out oRef);
        Cue cue1 = AudioManager.Instance.GetCue(Banks.Spells, Teleport.TELEPORT_SOUND_ORIGIN);
        this.mOriginEmitter.Up = Vector3.Up;
        this.mOriginEmitter.Position = position;
        this.mOriginEmitter.Forward = direction;
        cue1.Apply3D(this.mPlayState.Camera.Listener, this.mOriginEmitter);
        cue1.Play();
        EffectManager.Instance.StartEffect(Teleport.TELEPORT_EFFECT_APPEAR, ref teleportWarlockMessage.Position, ref teleportWarlockMessage.Direction, out oRef);
        Cue cue2 = AudioManager.Instance.GetCue(Banks.Spells, Teleport.TELEPORT_SOUND_DESTINATION);
        this.mDestinationEmitter.Up = Vector3.Up;
        this.mDestinationEmitter.Position = position;
        this.mDestinationEmitter.Forward = direction;
        cue2.Apply3D(this.mPlayState.Camera.Listener, this.mDestinationEmitter);
        cue2.Play();
        this.mWarlock.CharacterBody.DesiredDirection = teleportWarlockMessage.Direction;
        this.mWarlock.StopStatusEffects(StatusEffects.Burning);
        this.mWarlock.StopStatusEffects(StatusEffects.Cold);
        this.mWarlock.CharacterBody.Force = new Vector3();
        this.mWarlock.CharacterBody.Velocity = new Vector3();
        this.mWarlock.CharacterBody.Movement = new Vector3();
        this.mWarlock.Body.MoveTo(position, Matrix.CreateWorld(Vector3.Zero, teleportWarlockMessage.Direction, Vector3.Up));
        break;
    }
  }

  public unsafe void NetworkInitialize(ref BossInitializeMessage iMsg)
  {
    if (iMsg.Type != (ushort) 0)
      return;
    Machine.InitializeMessage initializeMessage;
    BossInitializeMessage.ConvertTo(ref iMsg, (void*) &initializeMessage);
    this.mWarlock = Magicka.GameLogic.Entities.Entity.GetFromHandle((int) initializeMessage.Handle) as NonPlayerCharacter;
    this.mNetworkInitialized = true;
  }

  public BossEnum GetBossType() => BossEnum.Machine;

  public bool NetworkInitialized => this.mNetworkInitialized;

  public float ResistanceAgainst(Elements iElement)
  {
    float num1 = 0.0f;
    float num2 = 0.0f;
    for (int iIndex = 0; iIndex < this.mResistances.Length; ++iIndex)
    {
      Elements elements = Defines.ElementFromIndex(iIndex);
      if ((iElement & elements) != Elements.None)
      {
        float multiplier = this.mResistances[iIndex].Multiplier;
        float modifier = this.mResistances[iIndex].Modifier;
        if (this.HasStatus(StatusEffects.Frozen) && (iElement & Elements.Earth) != Elements.None)
          modifier -= 350f;
        if (this.HasStatus(StatusEffects.Greased) && (iElement & Elements.Fire) != Elements.None)
          multiplier *= 2f;
        num1 += modifier;
        num2 += multiplier;
      }
    }
    return 1f - MathHelper.Clamp(num1 / 300f + num2, -1f, 1f);
  }

  protected class RenderData : 
    RenderableObject<SkinnedModelDeferredEffect, SkinnedModelDeferredAdvancedMaterial>
  {
    public Matrix[] mSkeleton;
    public float Flash;

    public RenderData() => this.mSkeleton = new Matrix[80 /*0x50*/];

    public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      SkinnedModelDeferredEffect modelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
      modelDeferredEffect.Bones = this.mSkeleton;
      modelDeferredEffect.OverrideColor = new Vector4(1f, 1f, 1f, this.Flash);
      base.Draw(iEffect, iViewFrustum);
      modelDeferredEffect.OverrideColor = Vector4.Zero;
    }

    public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      SkinnedModelDeferredEffect modelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
      modelDeferredEffect.Bones = this.mSkeleton;
      modelDeferredEffect.OverrideColor = new Vector4(1f, 1f, 1f, this.Flash);
      base.DrawShadow(iEffect, iViewFrustum);
      modelDeferredEffect.OverrideColor = Vector4.Zero;
    }
  }

  protected class OrcRenderData : 
    RenderableObject<SkinnedModelDeferredEffect, SkinnedModelDeferredAdvancedMaterial>
  {
    public SkinnedModelDeferredAdvancedMaterial mMaterial2;
    protected VertexDeclaration mVertexDeclaration2;
    protected int mBaseVertex2;
    protected int mNumVertices2;
    protected int mStartIndex2;
    protected int mPrimitiveCount2;
    protected int mVertexStride2;
    public Matrix[] mSkeleton;
    public float Flash;

    public OrcRenderData() => this.mSkeleton = new Matrix[80 /*0x50*/];

    public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      SkinnedModelDeferredEffect iEffect1 = iEffect as SkinnedModelDeferredEffect;
      iEffect1.Bones = this.mSkeleton;
      iEffect1.OverrideColor = new Vector4(1f, 1f, 1f, this.Flash);
      base.Draw(iEffect, iViewFrustum);
      this.mMaterial2.AssignToEffect(iEffect1);
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex2, 0, this.mNumVertices2, this.mStartIndex2, this.mPrimitiveCount2);
      iEffect1.OverrideColor = Vector4.Zero;
    }

    public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      SkinnedModelDeferredEffect iEffect1 = iEffect as SkinnedModelDeferredEffect;
      iEffect1.Bones = this.mSkeleton;
      base.Draw(iEffect, iViewFrustum);
      this.mMaterial2.AssignToEffect(iEffect1);
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex2, 0, this.mNumVertices2, this.mStartIndex2, this.mPrimitiveCount2);
    }

    public void SetMesh(
      VertexBuffer iVertices,
      IndexBuffer iIndices,
      ModelMeshPart iMeshPart,
      ModelMeshPart iMeshPart2,
      int iTechnique,
      int iDepthTechnique,
      int iShadowTechnique)
    {
      this.mVertexDeclaration2 = iMeshPart2.VertexDeclaration;
      this.mBaseVertex2 = iMeshPart2.BaseVertex;
      this.mNumVertices2 = iMeshPart2.NumVertices;
      this.mStartIndex2 = iMeshPart2.StartIndex;
      this.mPrimitiveCount2 = iMeshPart2.PrimitiveCount;
      this.mVertexStride2 = iMeshPart2.VertexStride;
      this.SetMesh(iVertices, iIndices, iMeshPart, iTechnique, iDepthTechnique, iShadowTechnique);
    }
  }

  protected class RulerRenderData : IRenderableGUIObject, IPreRenderRenderer
  {
    public BasicEffect Effect;
    public VertexPositionTexture[] Vertices;
    public VertexDeclaration VertexDeclaration;
    public Matrix Transform;
    public float Time;
    public float Alpha;
    private Matrix mViewProjection;
    private Text mText;
    public GUIBasicEffect TextEffect;
    private Vector2 mTextPosition;

    public RulerRenderData()
    {
      this.mText = new Text(20, FontManager.Instance.GetFont(MagickaFont.Maiandra14), TextAlign.Center, true);
    }

    public void Draw(float iDeltaTime)
    {
      Point screenSize = RenderManager.Instance.ScreenSize;
      this.Effect.VertexColorEnabled = false;
      this.Effect.TextureEnabled = true;
      this.Effect.Alpha = this.Alpha;
      this.Transform.Up = Vector3.Up;
      this.Effect.World = this.Transform;
      this.Effect.View = Matrix.Identity;
      this.Effect.Projection = this.mViewProjection;
      this.Effect.GraphicsDevice.VertexDeclaration = this.VertexDeclaration;
      this.Effect.Begin();
      this.Effect.CurrentTechnique.Passes[0].Begin();
      this.Effect.GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleStrip, this.Vertices, 0, 6);
      this.Effect.CurrentTechnique.Passes[0].End();
      this.Effect.End();
      this.TextEffect.VertexColorEnabled = true;
      this.TextEffect.SetScreenSize(screenSize.X, screenSize.Y);
      this.TextEffect.Color = new Vector4(1f, 1f, 1f, this.Alpha);
      this.TextEffect.Begin();
      this.TextEffect.CurrentTechnique.Passes[0].Begin();
      this.mText.Draw(this.TextEffect, (float) Math.Floor((double) this.mTextPosition.X), (float) Math.Floor((double) this.mTextPosition.Y));
      this.TextEffect.CurrentTechnique.Passes[0].End();
      this.TextEffect.End();
    }

    public int ZIndex => 100;

    public void PreRenderUpdate(
      DataChannel iDataChannel,
      float iDeltaTime,
      ref Matrix iViewProjectionMatrix,
      ref Vector3 iCameraPosition,
      ref Vector3 iCameraDirection)
    {
      this.mViewProjection = iViewProjectionMatrix;
      float x = (float) ((120.0 - (double) this.Time) * 0.0083333337679505348) * Machine.DISTANCE;
      int num1 = (int) x;
      int num2 = (int) ((double) x * 10.0 % 10.0);
      int num3 = (int) ((double) x * 100.0 % 10.0);
      int num4 = (int) ((double) x * 1000.0 % 10.0);
      this.mText.Characters[0] = (char) (48 /*0x30*/ + num1);
      this.mText.Characters[1] = '.';
      this.mText.Characters[2] = (char) (48 /*0x30*/ + num2);
      this.mText.Characters[3] = (char) (48 /*0x30*/ + num3);
      this.mText.Characters[4] = (char) (48 /*0x30*/ + num4);
      this.mText.Characters[5] = 'm';
      this.mText.Characters[6] = char.MinValue;
      this.mText.MarkAsDirty();
      Vector3 iWorldPosition = this.Transform.Translation + new Vector3(0.5f, 1.3f, 0.0f);
      this.mTextPosition = MagickaMath.WorldToScreenPosition(ref iWorldPosition, ref iViewProjectionMatrix);
      this.Vertices[0].Position = new Vector3(-0.25f, -0.1f, 0.0f);
      this.Vertices[1].Position = new Vector3(-0.25f, 0.9f, 0.0f);
      this.Vertices[2].Position = new Vector3(0.0f, -0.1f, 0.0f);
      this.Vertices[3].Position = new Vector3(0.0f, 0.9f, 0.0f);
      this.Vertices[4].Position = new Vector3(x, -0.1f, 0.0f);
      this.Vertices[5].Position = new Vector3(x, 0.9f, 0.0f);
      this.Vertices[6].Position = new Vector3(0.25f + x, -0.1f, 0.0f);
      this.Vertices[7].Position = new Vector3(0.25f + x, 0.9f, 0.0f);
    }
  }

  protected class IdleState : IBossState<Machine>
  {
    private static Machine.IdleState mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Machine.IdleState Instance
    {
      get
      {
        if (Machine.IdleState.mSingelton == null)
        {
          lock (Machine.IdleState.mSingeltonLock)
          {
            if (Machine.IdleState.mSingelton == null)
              Machine.IdleState.mSingelton = new Machine.IdleState();
          }
        }
        return Machine.IdleState.mSingelton;
      }
    }

    private IdleState()
    {
    }

    public void OnEnter(Machine iOwner)
    {
      iOwner.mMachineController.StartClip(iOwner.mMachineClips[1], true);
      iOwner.mOrcController.StartClip(iOwner.mOrcClips[3], true);
    }

    public void OnUpdate(float iDeltaTime, Machine iOwner)
    {
    }

    public void OnExit(Machine iOwner)
    {
    }
  }

  protected class DrillState : IBossState<Machine>
  {
    private static Machine.DrillState mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Machine.DrillState Instance
    {
      get
      {
        if (Machine.DrillState.mSingelton == null)
        {
          lock (Machine.DrillState.mSingeltonLock)
          {
            if (Machine.DrillState.mSingelton == null)
              Machine.DrillState.mSingelton = new Machine.DrillState();
          }
        }
        return Machine.DrillState.mSingelton;
      }
    }

    private DrillState()
    {
    }

    public void OnEnter(Machine iOwner)
    {
      iOwner.mOrcController.CrossFade(iOwner.mOrcClips[2], 0.2f, false);
      iOwner.mOrcController.Speed = 1f;
    }

    public void OnUpdate(float iDeltaTime, Machine iOwner)
    {
      iOwner.mRulerAlpha += iDeltaTime;
      if (iOwner.mOrcClips[2] == iOwner.mOrcController.AnimationClip)
      {
        if (!iOwner.mOrcController.HasFinished || iOwner.mOrcController.CrossFadeEnabled)
          return;
        iOwner.mOrcController.CrossFade(iOwner.mOrcClips[0], 0.2f, true);
        iOwner.mOrcController.Speed = 0.1f;
        iOwner.mMachineController.CrossFade(iOwner.mMachineClips[0], 0.2f, true);
        iOwner.mMachineController.Speed = 0.1f;
      }
      else
      {
        if ((double) iOwner.mDrillTime >= 120.0)
          iOwner.ChangeMachineState(Machine.States.KillKing);
        iOwner.mDrillTime += iDeltaTime * iOwner.mPeddleSpeed;
        iOwner.mMachineController.Speed = iOwner.mPeddleSpeed;
        iOwner.mOrcController.Speed = iOwner.mPeddleSpeed;
      }
    }

    public void OnExit(Machine iOwner)
    {
    }
  }

  protected class BrokenState : IBossState<Machine>
  {
    private static Machine.BrokenState mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Machine.BrokenState Instance
    {
      get
      {
        if (Machine.BrokenState.mSingelton == null)
        {
          lock (Machine.BrokenState.mSingeltonLock)
          {
            if (Machine.BrokenState.mSingelton == null)
              Machine.BrokenState.mSingelton = new Machine.BrokenState();
          }
        }
        return Machine.BrokenState.mSingelton;
      }
    }

    private BrokenState()
    {
    }

    public void OnEnter(Machine iOwner)
    {
      Matrix machineOrientation = iOwner.mMachineOrientation;
      Vector3 translation = machineOrientation.Translation with
      {
        Y = 0.0f
      };
      machineOrientation.Translation = translation;
      EffectManager.Instance.StartEffect(Machine.MACHINE_BREAK_EFFECT, ref machineOrientation, out VisualEffectReference _);
      iOwner.mMachineController.CrossFade(iOwner.mMachineClips[2], 0.2f, false);
      iOwner.mMachineController.Speed = 1f;
      foreach (Magicka.GameLogic.Entities.Entity entity in iOwner.mPlayState.EntityManager.Entities)
      {
        if (entity is Barrier | entity is MissileEntity | entity is SpellMine | entity is Grease.GreaseField)
          entity.Kill();
      }
      iOwner.mPlayState.Level.CurrentScene.ExecuteTrigger(Machine.MACHINE_DEATH_TRIGGER_ID, (Magicka.GameLogic.Entities.Character) null, false);
      AudioManager.Instance.PlayCue(Banks.Misc, "misc_machine_break".GetHashCodeCustom(), iOwner.mAudioEmitter);
      iOwner.mRulerAlpha = 0.0f;
    }

    public void OnUpdate(float iDeltaTime, Machine iOwner)
    {
      iOwner.mPeddleTargetSpeed = 1f;
      iOwner.mRulerAlpha -= iDeltaTime;
      if (!iOwner.mOrcController.HasFinished || iOwner.mOrcController.CrossFadeEnabled || iOwner.mOrcController.AnimationClip != iOwner.mOrcClips[1] || iOwner.mPlayState.Level.CurrentScene.Triggers[Machine.BIPEDAL_DONE_TRIGGER_ID].HasTriggered)
        return;
      iOwner.mPlayState.Level.CurrentScene.Triggers[Machine.BIPEDAL_DONE_TRIGGER_ID].Execute((Magicka.GameLogic.Entities.Character) null, false);
      iOwner.ChangeStage(Machine.States.Warlock);
    }

    public void OnExit(Machine iOwner)
    {
    }
  }

  protected class KillKingState : IBossState<Machine>
  {
    private static Machine.KillKingState mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Machine.KillKingState Instance
    {
      get
      {
        if (Machine.KillKingState.mSingelton == null)
        {
          lock (Machine.KillKingState.mSingeltonLock)
          {
            if (Machine.KillKingState.mSingelton == null)
              Machine.KillKingState.mSingelton = new Machine.KillKingState();
          }
        }
        return Machine.KillKingState.mSingelton;
      }
    }

    private KillKingState()
    {
    }

    public void OnEnter(Machine iOwner)
    {
      iOwner.mOrcController.CrossFade(iOwner.mOrcClips[3], 0.2f, true);
      iOwner.mOrcController.Speed = 1f;
      iOwner.mMachineController.CrossFade(iOwner.mMachineClips[1], 0.2f, true);
      iOwner.mOrcController.Speed = 1f;
      iOwner.ChangeStage(Machine.States.KingKilled);
    }

    public void OnUpdate(float iDeltaTime, Machine iOwner)
    {
    }

    public void OnExit(Machine iOwner)
    {
    }
  }

  protected class IntroStage : IBossState<Machine>
  {
    private static Machine.IntroStage mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Machine.IntroStage Instance
    {
      get
      {
        if (Machine.IntroStage.mSingelton == null)
        {
          lock (Machine.IntroStage.mSingeltonLock)
          {
            if (Machine.IntroStage.mSingelton == null)
              Machine.IntroStage.mSingelton = new Machine.IntroStage();
          }
        }
        return Machine.IntroStage.mSingelton;
      }
    }

    private IntroStage()
    {
    }

    public void OnEnter(Machine iOwner)
    {
      iOwner.mKingController.StartClip(iOwner.mKingClips[0], true);
    }

    public void OnUpdate(float iDeltaTime, Machine iOwner)
    {
    }

    public void OnExit(Machine iOwner)
    {
    }
  }

  protected class PreGunterStage : IBossState<Machine>
  {
    private static Machine.PreGunterStage mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Machine.PreGunterStage Instance
    {
      get
      {
        if (Machine.PreGunterStage.mSingelton == null)
        {
          lock (Machine.PreGunterStage.mSingeltonLock)
          {
            if (Machine.PreGunterStage.mSingelton == null)
              Machine.PreGunterStage.mSingelton = new Machine.PreGunterStage();
          }
        }
        return Machine.PreGunterStage.mSingelton;
      }
    }

    private PreGunterStage()
    {
    }

    public void OnEnter(Machine iOwner)
    {
      iOwner.mIdleTimer = 0.0f;
      iOwner.mKingController.CrossFade(iOwner.mKingClips[3], 0.1f, true);
      iOwner.mWarlock.AI.SetOrder(Order.Idle, ReactTo.None, Order.Idle, 0, 0, 0, (AIEvent[]) null);
      iOwner.mWarlock.SetImmortalTime(float.PositiveInfinity);
    }

    public void OnUpdate(float iDeltaTime, Machine iOwner)
    {
      if ((double) iOwner.mIdleTimer > 9.0)
      {
        iOwner.mIdleTimer -= 9f;
        iOwner.mCurrentDialog = Machine.DIALOG_MACHINE[iOwner.mRandom.Next(Machine.DIALOG_MACHINE.Count)];
        DialogManager.Instance.StartDialog(iOwner.mCurrentDialog, (Magicka.GameLogic.Entities.Entity) iOwner.mWarlock, (Controller) null);
      }
      iOwner.mIdleTimer += iDeltaTime;
    }

    public void OnExit(Machine iOwner)
    {
    }
  }

  protected class WarlockStage : IBossState<Machine>
  {
    private static Machine.WarlockStage mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Machine.WarlockStage Instance
    {
      get
      {
        if (Machine.WarlockStage.mSingelton == null)
        {
          lock (Machine.WarlockStage.mSingeltonLock)
          {
            if (Machine.WarlockStage.mSingelton == null)
              Machine.WarlockStage.mSingelton = new Machine.WarlockStage();
          }
        }
        return Machine.WarlockStage.mSingelton;
      }
    }

    private WarlockStage()
    {
    }

    public void OnEnter(Machine iOwner)
    {
      iOwner.mWarlock.AI.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, 0, 0, (AIEvent[]) null);
      iOwner.mWarlock.SetImmortalTime(0.0f);
      iOwner.mWarlock.HitPoints = iOwner.mWarlock.MaxHitPoints;
    }

    public void OnUpdate(float iDeltaTime, Machine iOwner)
    {
      if ((double) iOwner.mWarlock.HitPoints > 0.0)
        return;
      iOwner.ChangeStage(Machine.States.Final);
    }

    public void OnExit(Machine iOwner)
    {
    }
  }

  protected class FinalStage : IBossState<Machine>
  {
    private static Machine.FinalStage mSingelton;
    private static volatile object mSingeltonLock = new object();
    private SpellSoundVariables mSSV;
    private bool mLevelAnimation;

    public static Machine.FinalStage Instance
    {
      get
      {
        if (Machine.FinalStage.mSingelton == null)
        {
          lock (Machine.FinalStage.mSingeltonLock)
          {
            if (Machine.FinalStage.mSingelton == null)
              Machine.FinalStage.mSingelton = new Machine.FinalStage();
          }
        }
        return Machine.FinalStage.mSingelton;
      }
    }

    private FinalStage() => this.mSSV.mMagnitude = 2f;

    public void OnEnter(Machine iOwner)
    {
      iOwner.mPlayState.Level.CurrentScene.ExecuteTrigger(Machine.GUNTER_DEFEATED_TRIGGER_ID, (Magicka.GameLogic.Entities.Character) null, false);
      iOwner.mWarlock.StopStatusEffects(StatusEffects.Wet);
      iOwner.mWarlock.StopStatusEffects(StatusEffects.Burning);
      iOwner.mWarlock.StopStatusEffects(StatusEffects.Cold);
      iOwner.mWarlock.StopStatusEffects(StatusEffects.Steamed);
      iOwner.TeleportWarlock(iOwner.mWarlockEmperorOrientation);
      while (iOwner.mWarlock.AI.CurrentState != AIStateIdle.Instance)
      {
        if (iOwner.mWarlock.AI.CurrentState is AIStateAttack)
          iOwner.mWarlock.AI.ReleaseTarget();
        iOwner.mWarlock.AI.PopState();
      }
      iOwner.mWarlock.AI.SetOrder(Order.Idle, ReactTo.None, Order.Idle, 0, 0, 0, iOwner.mWarlockEmperorEvent);
      AudioManager.Instance.PlayCue(Banks.Misc, "misc_trapdoor".GetHashCodeCustom(), iOwner.mAudioEmitter);
      iOwner.mKingController.CrossFade(iOwner.mKingClips[4], 0.25f, false);
      iOwner.mEmperorLightningTimer = 0.0f;
      iOwner.mIdleTimer = 0.0f;
      this.mLevelAnimation = false;
    }

    public void OnUpdate(float iDeltaTime, Machine iOwner)
    {
      iOwner.mIdleTimer += iDeltaTime;
      iOwner.mEmperorLightningTimer += iDeltaTime;
      Matrix emperorOrientation = iOwner.mWarlockEmperorOrientation with
      {
        Translation = new Vector3()
      };
      Vector3 oPos = iOwner.mWarlockEmperorOrientation.Translation;
      Segment iSeg = new Segment();
      iSeg.Origin = oPos;
      iSeg.Delta.Y -= 2f;
      iOwner.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out Vector3 _, iSeg);
      oPos.Y -= iOwner.mWarlock.HeightOffset;
      iOwner.mWarlock.Body.MoveTo(oPos, emperorOrientation);
      if ((double) iOwner.mEmperorLightningTimer > 5.5)
      {
        if (!this.mLevelAnimation && (double) iOwner.mEmperorLightningTimer > 9.0)
        {
          iOwner.mPlayState.Level.CurrentScene.LevelModel.GetAnimatedLevelPart("hatch".GetHashCodeCustom()).Play(true, -1f, -1f, 1f, false, false);
          this.mLevelAnimation = true;
        }
        if ((double) iOwner.mIdleTimer > 0.25)
        {
          iOwner.mIdleTimer -= 0.25f;
          LightningBolt lightning1 = LightningBolt.GetLightning();
          Vector3 translation1 = iOwner.mWarlock.GetRightAttachOrientation().Translation;
          Vector3 translation2 = iOwner.mWarlock.GetHipAttachOrientation().Translation;
          Vector3 result;
          Vector3.Subtract(ref translation1, ref translation2, out result);
          result.Normalize();
          result.X *= (float) Machine.RANDOM.NextDouble() * 5f;
          result.Y *= (float) Machine.RANDOM.NextDouble() * 5f;
          result.Z *= (float) Machine.RANDOM.NextDouble() * 5f;
          result.Normalize();
          lightning1.InitializeEffect(ref translation1, result, Spell.LIGHTNINGCOLOR, false, 1f, 10f, iOwner.mPlayState);
          LightningBolt lightning2 = LightningBolt.GetLightning();
          Vector3 translation3 = iOwner.mWarlock.GetLeftAttachOrientation().Translation;
          Vector3.Subtract(ref translation3, ref translation2, out result);
          result.Normalize();
          result.X *= (float) Machine.RANDOM.NextDouble() * 5f;
          result.Y *= (float) Machine.RANDOM.NextDouble() * 5f;
          result.Z *= (float) Machine.RANDOM.NextDouble() * 5f;
          result.Normalize();
          lightning2.InitializeEffect(ref translation3, result, Spell.LIGHTNINGCOLOR, false, 1f, 10f, iOwner.mPlayState);
        }
        iOwner.mIdleTimer += iDeltaTime;
      }
      if (iOwner.mKingController.CrossFadeEnabled || !iOwner.mKingController.HasFinished)
        return;
      iOwner.ChangeStage(Machine.States.PostFinal);
    }

    public void OnExit(Machine iOwner)
    {
    }
  }

  protected class PostFinalStage : IBossState<Machine>
  {
    private static Machine.PostFinalStage mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Machine.PostFinalStage Instance
    {
      get
      {
        if (Machine.PostFinalStage.mSingelton == null)
        {
          lock (Machine.PostFinalStage.mSingeltonLock)
          {
            if (Machine.PostFinalStage.mSingelton == null)
              Machine.PostFinalStage.mSingelton = new Machine.PostFinalStage();
          }
        }
        return Machine.PostFinalStage.mSingelton;
      }
    }

    private PostFinalStage()
    {
    }

    public void OnEnter(Machine iOwner)
    {
      iOwner.mKingController.CrossFade(iOwner.mKingClips[1], 0.1f, false);
      iOwner.mWarlock.Body.MoveTo(new Vector3(0.0f, -101f, 0.0f), Matrix.Identity);
      iOwner.mWarlock.CannotDieWithoutExplicitKill = false;
      iOwner.mWarlock.SetImmortalTime(0.0f);
      iOwner.mWarlock.Terminate(true, false);
      iOwner.mPlayState.Level.CurrentScene.ExecuteTrigger(Machine.GUNTER_DEAD_TRIGGER_ID, (Magicka.GameLogic.Entities.Character) null, false);
    }

    public void OnUpdate(float iDeltaTime, Machine iOwner)
    {
    }

    public void OnExit(Machine iOwner)
    {
    }
  }

  protected class KingKilledStage : IBossState<Machine>
  {
    private static Machine.KingKilledStage mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Machine.KingKilledStage Instance
    {
      get
      {
        if (Machine.KingKilledStage.mSingelton == null)
        {
          lock (Machine.KingKilledStage.mSingeltonLock)
          {
            if (Machine.KingKilledStage.mSingelton == null)
              Machine.KingKilledStage.mSingelton = new Machine.KingKilledStage();
          }
        }
        return Machine.KingKilledStage.mSingelton;
      }
    }

    private KingKilledStage()
    {
    }

    public void OnEnter(Machine iOwner)
    {
      iOwner.mIdleTimer = 0.0f;
      for (int index = 0; index < iOwner.mNPCs.Count; ++index)
      {
        while (iOwner.mNPCs[index].AI.CurrentState != AIStateIdle.Instance)
        {
          if (iOwner.mNPCs[index].AI.CurrentState is AIStateAttack)
            iOwner.mNPCs[index].AI.ReleaseTarget();
          iOwner.mNPCs[index].AI.PopState();
        }
        iOwner.mNPCs[index].AI.SetOrder(Order.Idle, ReactTo.None, Order.Idle, 0, 0, 0, (AIEvent[]) null);
      }
      iOwner.mPlayState.Endgame(EndGameCondition.Defeat, true, false, 0.0f);
      iOwner.OverKillKing();
    }

    public void OnUpdate(float iDeltaTime, Machine iOwner)
    {
    }

    public void OnExit(Machine iOwner)
    {
    }
  }

  private enum MessageType : ushort
  {
    Initialize,
    Update,
    ChangeMachineState,
    ChangeFightState,
    ScriptMessage,
    Teleport,
  }

  internal struct InitializeMessage
  {
    public const ushort TYPE = 0;
    public ushort Handle;
  }

  internal struct UpdateMessage
  {
    public const ushort TYPE = 1;
    public byte Animation;
    public float AnimationTime;
    public float Hitpoints;
  }

  internal struct ScriptMessageMessage
  {
    public const ushort TYPE = 4;
    public BossMessages Message;
  }

  internal struct ChangeMachineStateMessage
  {
    public const ushort TYPE = 2;
    public Machine.States NewState;
  }

  internal struct ChangeFightStateMessage
  {
    public const ushort TYPE = 3;
    public Machine.States NewState;
  }

  internal struct TeleportWarlockMessage
  {
    public const ushort TYPE = 5;
    public Vector3 Position;
    public Vector3 Direction;
  }

  public enum States
  {
    Idle,
    Drill,
    Broken,
    KillKing,
    Intro,
    PreGunter,
    Warlock,
    Final,
    PostFinal,
    KingKilled,
  }

  private enum KingAnimations
  {
    Idle,
    Dialog,
    DialogDawn,
    Struggle,
    Vader,
    NrOfAnimations,
  }

  private enum MachineAnimations
  {
    Drill,
    Idle,
    Break,
    NrOfAnimations,
  }

  private enum OrcAnimations
  {
    Peddle,
    BreakFree,
    Nod,
    Idle,
    NrOfAnimations,
  }

  public enum Sharpnel
  {
    Cog,
    Plank,
    Misc,
    NrOfSharpnel,
  }
}
