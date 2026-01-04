// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.MissileEntity
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using JigLibX.Physics;
using Magicka.Achievements;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Graphics.Lights;
using Magicka.Levels;
using Magicka.Network;
using Magicka.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities;

public class MissileEntity : Entity, IDamageable
{
  private static List<MissileEntity> sCache;
  private Model mModel;
  private bool mFacingVelocity = true;
  private bool mHasCollided;
  private float mCollisionVelocity;
  private bool mIsDamaged;
  private float mDamageAmount;
  private Elements mDamageElements;
  private float mLevelCollisionTimer;
  private Entity mCollisionTarget;
  private float mHomingTolerance;
  private float mHomingPower;
  private float mTimeAlive;
  private ConditionCollection mConditionCollection;
  private SpellEffect mCurrentSpell;
  private List<DynamicLight> mLights = new List<DynamicLight>(2);
  private float mSpawnVelocity;
  private Entity mOwner;
  private Entity mTarget;
  private Vector3 mLastKnownTargetPosition;
  private HitList mHitList;
  private HitList mGibbedHitList;
  private MissileEntity.RenderData[][] mRenderData;
  private List<Cue> mSoundList;
  private Dictionary<int, VisualEffectReference> mEffectReferences;
  private float mDanger;
  private bool mPiercing;
  private int mDeepImpactCount;
  private bool mDeepImpactPossible;
  private Vector3 mProppMagickLever;
  private VisualEffectReference mProppMagickEffect;
  private bool mProppMagickEffectActive;
  private bool mVelocityChange;
  private Vector3 mNewVelocity;
  private Elements mCombinedElements;

  public static MissileEntity GetInstance(PlayState iPlayState)
  {
    MissileEntity instance;
    lock (MissileEntity.sCache)
    {
      instance = MissileEntity.sCache[0];
      MissileEntity.sCache.RemoveAt(0);
      MissileEntity.sCache.Add(instance);
    }
    return instance;
  }

  public static MissileEntity GetSpecificInstance(ushort iHandle)
  {
    MissileEntity fromHandle;
    lock (MissileEntity.sCache)
    {
      fromHandle = Entity.GetFromHandle((int) iHandle) as MissileEntity;
      MissileEntity.sCache.Remove(fromHandle);
      MissileEntity.sCache.Add(fromHandle);
    }
    return fromHandle;
  }

  public static void InitializeCache(int iNr, PlayState iPlayState)
  {
    MissileEntity.sCache = new List<MissileEntity>(iNr);
    for (int index = 0; index < iNr; ++index)
      MissileEntity.sCache.Add(new MissileEntity(iPlayState));
  }

  public float TimeAlive => this.mTimeAlive;

  public MissileEntity(PlayState iPlayState)
    : base(iPlayState)
  {
    this.mBody = new Body();
    this.mCollision = new CollisionSkin(this.mBody);
    this.mCollision.AddPrimitive((Primitive) new Box(Vector3.Zero, Matrix.Identity, new Vector3(1f)), 1, new MaterialProperties(0.1f, 0.8f, 0.8f));
    this.mBody.CollisionSkin = this.mCollision;
    Vector3 vector3 = this.SetMass(4f);
    Transform transform = new Transform();
    Vector3.Negate(ref vector3, out transform.Position);
    transform.Orientation = Matrix.Identity;
    this.mCollision.ApplyLocalTransform(transform);
    this.mCollision.callbackFn += new CollisionCallbackFn(this.OnCollision);
    this.mBody.Tag = (object) this;
    this.mBody.Immovable = false;
    this.mBody.AllowFreezing = true;
    this.mOwner = (Entity) null;
    this.mHitList = new HitList(32 /*0x20*/);
    this.mGibbedHitList = new HitList(16 /*0x10*/);
    this.mConditionCollection = new ConditionCollection();
    this.mRenderData = new MissileEntity.RenderData[3][];
    this.mRenderData[0] = new MissileEntity.RenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      MissileEntity.RenderData renderData = new MissileEntity.RenderData();
      this.mRenderData[0][index] = renderData;
    }
    this.mRenderData[1] = new MissileEntity.RenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      MissileEntity.RenderData renderData = new MissileEntity.RenderData();
      this.mRenderData[1][index] = renderData;
    }
    this.mRenderData[2] = new MissileEntity.RenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      MissileEntity.RenderData renderData = new MissileEntity.RenderData();
      this.mRenderData[2][index] = renderData;
    }
    this.mEffectReferences = new Dictionary<int, VisualEffectReference>(20);
  }

  public void Initialize(
    Entity iOwner,
    Entity iTarget,
    float iHoming,
    float iRadius,
    ref Vector3 iPosition,
    ref Vector3 iVelocity,
    Model iModel,
    ConditionCollection iConditions,
    bool iCanHitOwner)
  {
    this.Initialize(iOwner, iRadius, ref iPosition, ref iVelocity, iModel, iConditions, iCanHitOwner);
    this.mTarget = iTarget;
    this.mHomingPower = iHoming;
  }

  public void Initialize(
    Entity iOwner,
    float iRadius,
    ref Vector3 iPosition,
    ref Vector3 iVelocity,
    Model iModel,
    ConditionCollection iConditions,
    bool iCanHitOwner,
    Spell iSpell)
  {
    this.Initialize(iOwner, iRadius, ref iPosition, ref iVelocity, iModel, iConditions, iCanHitOwner);
    if (iSpell.Element != Elements.Earth || !(iOwner is Avatar) || (iOwner as Avatar).Player == null || (iOwner as Avatar).Player.Gamer is NetworkGamer)
      return;
    this.mDeepImpactPossible = true;
  }

  public void Initialize(
    Entity iOwner,
    float iRadius,
    ref Vector3 iPosition,
    ref Vector3 iVelocity,
    Model iModel,
    ConditionCollection iConditions,
    bool iCanHitOwner)
  {
    foreach (KeyValuePair<int, VisualEffectReference> mEffectReference in this.mEffectReferences)
    {
      VisualEffectReference iRef = mEffectReference.Value;
      EffectManager.Instance.Stop(ref iRef);
    }
    this.mEffectReferences.Clear();
    this.mDeepImpactPossible = false;
    this.mDeepImpactCount = 0;
    this.mTarget = (Entity) null;
    this.mLastKnownTargetPosition = Vector3.Zero;
    this.mModel = iModel;
    this.MarkRenderDataDirty();
    if (iConditions != null)
      iConditions.CopyTo(this.mConditionCollection);
    else
      this.mConditionCollection.Clear();
    this.mCombinedElements = Elements.None;
    this.mPiercing = false;
    for (int iIndex = 0; iIndex < this.mConditionCollection.Count; ++iIndex)
    {
      EventCollection mCondition = this.mConditionCollection[iIndex];
      for (int index = 0; index < mCondition.Count; ++index)
      {
        EventStorage eventStorage = mCondition[index];
        if (eventStorage.EventType == EventType.Damage)
        {
          this.mCombinedElements |= eventStorage.DamageEvent.Damage.Element;
          if ((eventStorage.DamageEvent.Damage.AttackProperty & AttackProperties.Piercing) != (AttackProperties) 0)
            this.mPiercing = true;
        }
        else if (eventStorage.EventType == EventType.Blast)
        {
          this.mCombinedElements |= eventStorage.BlastEvent.Damage.A.Element;
          this.mCombinedElements |= eventStorage.BlastEvent.Damage.B.Element;
          this.mCombinedElements |= eventStorage.BlastEvent.Damage.C.Element;
          this.mCombinedElements |= eventStorage.BlastEvent.Damage.D.Element;
          this.mCombinedElements |= eventStorage.BlastEvent.Damage.E.Element;
        }
        else if (eventStorage.EventType == EventType.Splash)
          this.mCombinedElements |= eventStorage.SplashEvent.Damage.Element;
      }
    }
    this.mDead = false;
    this.mHasCollided = false;
    this.mIsDamaged = false;
    this.mLevelCollisionTimer = 0.0f;
    this.mDanger = 2f;
    this.mHitList.Clear();
    this.mGibbedHitList.Clear();
    this.mRadius = iRadius;
    this.mSoundList = new List<Cue>(4);
    this.Initialize();
    this.mBody.ApplyGravity = true;
    this.mOwner = iOwner;
    this.mSpawnVelocity = iVelocity.Length();
    this.mHomingPower = 0.0f;
    this.mHomingTolerance = -0.75f;
    Vector3 vector3_1 = new Vector3(iRadius * 2f);
    Vector3 vector3_2 = new Vector3(iRadius);
    (this.mCollision.GetPrimitiveLocal(0) as Box).SideLengths = vector3_1;
    (this.mCollision.GetPrimitiveLocal(0) as Box).Position = -vector3_2;
    (this.mCollision.GetPrimitiveNewWorld(0) as Box).SideLengths = vector3_1;
    (this.mCollision.GetPrimitiveNewWorld(0) as Box).Position = -vector3_2;
    (this.mCollision.GetPrimitiveOldWorld(0) as Box).SideLengths = vector3_1;
    (this.mCollision.GetPrimitiveOldWorld(0) as Box).Position = -vector3_2;
    this.mBody.MoveTo(iPosition, Matrix.Identity);
    this.mAudioEmitter.Position = iPosition;
    this.mAudioEmitter.Forward = Vector3.Forward;
    this.mAudioEmitter.Up = Vector3.Up;
    this.mCollision.NonCollidables.Clear();
    if (iOwner != null)
    {
      if (iCanHitOwner)
        this.mHitList.Add(iOwner.Handle);
      else
        this.mCollision.NonCollidables.Add(iOwner.Body.CollisionSkin);
    }
    this.mBody.Velocity = iVelocity;
    this.mBody.EnableBody();
    iConditions.ExecuteAll((Entity) this, (Entity) null, ref new EventCondition()
    {
      EventConditionType = EventConditionType.Default
    });
    this.mTimeAlive = 0.0f;
    this.mCollisionTarget = (Entity) null;
  }

  public override void Deinitialize()
  {
    foreach (KeyValuePair<int, VisualEffectReference> mEffectReference in this.mEffectReferences)
    {
      VisualEffectReference iRef = mEffectReference.Value;
      EffectManager.Instance.Stop(ref iRef);
    }
    this.mEffectReferences.Clear();
    for (int index = 0; index < this.mSoundList.Count; ++index)
    {
      if (!this.mSoundList[index].IsStopped && !this.mSoundList[index].IsStopping)
        this.mSoundList[index].Stop(AudioStopOptions.AsAuthored);
    }
    this.mSoundList.Clear();
    for (int index = 0; index < this.mLights.Count; ++index)
      this.mLights[index].Stop(true);
    this.mLights.Clear();
    this.mProppMagickEffectActive = false;
    EffectManager.Instance.Stop(ref this.mProppMagickEffect);
    this.mEffectReferences.Clear();
    this.mConditionCollection.Clear();
    this.mCurrentSpell = (SpellEffect) null;
    base.Deinitialize();
  }

  public float ResistanceAgainst(Elements iElement) => 0.0f;

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if (this.mEffectReferences != null)
    {
      foreach (KeyValuePair<int, VisualEffectReference> mEffectReference in this.mEffectReferences)
      {
        VisualEffectReference iRef = mEffectReference.Value;
        Vector3 position = this.Position;
        Vector3 forward = this.GetOrientation().Forward;
        EffectManager.Instance.UpdatePositionDirection(ref iRef, ref position, ref forward);
      }
    }
    this.mHitList.Update(iDeltaTime);
    this.mGibbedHitList.Update(iDeltaTime);
    this.mTimeAlive += iDeltaTime;
    if (this.mVelocityChange)
    {
      this.mBody.Velocity = this.mNewVelocity;
      this.mVelocityChange = false;
    }
    Vector3 position1 = this.mBody.Position;
    if ((double) this.mHomingPower > 0.0)
    {
      if (this.mTarget != null && this.mTarget.Body != null)
      {
        this.mLastKnownTargetPosition = this.mTarget.Position;
        this.mLastKnownTargetPosition.Y += this.mTarget.Radius;
      }
      Vector3 result1 = this.mBody.Velocity;
      if ((double) result1.LengthSquared() > 9.9999999747524271E-07)
      {
        Vector3 result2;
        Vector3.Normalize(ref result1, out result2);
        Vector3 result3;
        Vector3.Subtract(ref this.mLastKnownTargetPosition, ref position1, out result3);
        result3.Normalize();
        Vector3 result4;
        Vector3.Cross(ref result2, ref result3, out result4);
        float result5;
        Vector3.Dot(ref result2, ref result3, out result5);
        if ((double) result5 <= (double) this.mHomingTolerance)
          this.mHomingPower = 0.0f;
        Quaternion result6;
        Quaternion.CreateFromAxisAngle(ref result4, iDeltaTime * 10f * this.mHomingPower, out result6);
        Vector3.Transform(ref result1, ref result6, out result1);
        this.mBody.Velocity = result1;
      }
    }
    EventCondition iArgs = new EventCondition();
    iArgs.EventConditionType = EventConditionType.Timer;
    iArgs.Time = this.mTimeAlive;
    if (this.mHasCollided)
    {
      if ((double) this.mLevelCollisionTimer <= 1.4012984643248171E-45)
      {
        iArgs.EventConditionType |= EventConditionType.Collision;
        iArgs.Threshold = this.mCollisionVelocity;
      }
      this.mLevelCollisionTimer += iDeltaTime;
      if ((double) this.mLevelCollisionTimer >= 0.15000000596046448)
      {
        this.mHasCollided = false;
        this.mLevelCollisionTimer = 0.0f;
      }
      this.mCollisionTarget = (Entity) null;
    }
    this.mCollisionVelocity = 0.0f;
    if (this.mIsDamaged)
    {
      iArgs.EventConditionType |= EventConditionType.Damaged;
      iArgs.Hitpoints = this.mDamageAmount;
      iArgs.Elements = this.mDamageElements;
    }
    NetworkState state = NetworkManager.Instance.State;
    if (this.SendsNetworkUpdate(state) && this.mConditionCollection.ExecuteAll((Entity) this, this.mCollisionTarget, ref iArgs, out DamageResult _) && state != NetworkState.Offline)
    {
      MissileEntityEventMessage iMessage = new MissileEntityEventMessage();
      iMessage.Handle = this.Handle;
      iMessage.EventConditionType = iArgs.EventConditionType;
      if (this.mCollisionTarget != null)
        iMessage.TargetHandle = this.mCollisionTarget.Handle;
      iMessage.TimeAlive = iArgs.Time;
      iMessage.Threshold = iArgs.Threshold;
      iMessage.HitPoints = iArgs.Hitpoints;
      iMessage.Elements = iArgs.Elements;
      iMessage.OnCollision = false;
      NetworkManager.Instance.Interface.SendMessage<MissileEntityEventMessage>(ref iMessage);
    }
    this.mDamageElements = Elements.None;
    this.mDamageAmount = 0.0f;
    if ((double) position1.Y <= -50.0)
      this.Kill();
    BoundingSphere boundingSphere = new BoundingSphere();
    boundingSphere.Center = this.Position;
    boundingSphere.Radius = this.mRadius;
    for (int index = 0; index < 3; ++index)
    {
      Model mModel = this.mModel;
      MissileEntity.RenderData iObject = this.mRenderData[index][(int) iDataChannel];
      if (mModel != null)
      {
        iObject.mTransform = this.GetOrientation();
        iObject.mBoundingSphere = boundingSphere;
        if (iObject.IsDirty)
        {
          ModelMesh mesh = mModel.Meshes[0];
          ModelMeshPart meshPart = mesh.MeshParts[0];
          iObject.SetMesh(mesh.VertexBuffer, mesh.IndexBuffer, meshPart);
        }
        if (iObject.Effect == AdditiveEffect.TYPEHASH)
          this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, (IRenderableAdditiveObject) iObject);
        else
          this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) iObject);
      }
      else if (iObject.IsDirty)
        iObject.SetMesh((VertexBuffer) null, (IndexBuffer) null, (ModelMeshPart) null);
    }
    if (this.mProppMagickEffectActive)
    {
      Matrix orientation = this.mBody.Orientation;
      Vector3 result7 = this.mBody.Position;
      Vector3 result8;
      Vector3.TransformNormal(ref this.mProppMagickLever, ref orientation, out result8);
      Vector3.Add(ref result7, ref result8, out result7);
      orientation.Translation = result7;
      if (!EffectManager.Instance.UpdateOrientation(ref this.mProppMagickEffect, ref orientation))
        this.mProppMagickEffectActive = false;
    }
    base.Update(iDataChannel, iDeltaTime);
    for (int index = 0; index < this.mSoundList.Count; ++index)
      this.mSoundList[index].Apply3D(this.mPlayState.Camera.Listener, this.AudioEmitter);
    for (int index = 0; index < this.mLights.Count; ++index)
      this.mLights[index].Position = this.Position;
  }

  protected void MarkRenderDataDirty()
  {
    for (int index1 = 0; index1 < 3; ++index1)
    {
      for (int index2 = 0; index2 < 3; ++index2)
        this.mRenderData[index1][index2].SetDirty();
    }
  }

  public bool FacingVelocity
  {
    get => this.mFacingVelocity;
    set => this.mFacingVelocity = value;
  }

  public float NormalizedVelocity => this.mBody.Velocity.Length() / 50f;

  public override Matrix GetOrientation()
  {
    if (!this.mFacingVelocity)
      return base.GetOrientation();
    Matrix orientation = this.mBody.Orientation;
    Vector3 velocity = this.mBody.Velocity;
    velocity.Normalize();
    orientation.Forward = velocity;
    orientation.Right = Vector3.Normalize(Vector3.Cross(orientation.Forward, Vector3.Up));
    orientation.Up = Vector3.Normalize(Vector3.Cross(orientation.Right, orientation.Forward));
    orientation.Translation += this.mBody.Position;
    return orientation;
  }

  public Elements CombinedDamageElements => this.mCombinedElements;

  protected bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
  {
    if (iSkin1.Owner == null)
    {
      if (!(iSkin1.Tag is LevelModel | iSkin1.Tag is Water | iSkin1.Tag is Lava))
        return false;
      this.mCollisionTarget = (Entity) null;
      if (this.mHasCollided)
        return true;
      if ((double) this.mLevelCollisionTimer > 0.0)
        return false;
      this.mHasCollided = true;
      this.mCollisionVelocity = System.Math.Max(this.mCollisionVelocity, this.mBody.Velocity.LengthSquared());
      return true;
    }
    if (iSkin1.Owner is PhysicsObjectBody)
    {
      this.mHasCollided = true;
      this.mCollisionVelocity = System.Math.Max(this.mCollisionVelocity, this.mBody.Velocity.LengthSquared());
      if (this.mCollisionTarget == null)
        this.mCollisionTarget = iSkin1.Owner.Tag as Entity;
    }
    if (iSkin1.Owner != null && !(iSkin1.Owner.Tag is Pickable) && (!(iSkin1.Owner.Tag is Character) || !(iSkin1.Owner.Tag as Character).IsEthereal) && !(iSkin1.Owner.Tag is CthulhuMist))
    {
      if (iSkin1.Owner.Tag is BossDamageZone)
      {
        if ((iSkin1.Owner.Tag as BossDamageZone).IsEthereal)
          return false;
      }
      else if (iSkin1.Owner.Tag is Grease.GreaseField)
        return false;
      if (!(iSkin1.Owner.Tag is Entity tag) || tag is BossCollisionZone && !(tag is BossDamageZone))
        return true;
      if (tag.Dead | !(tag is IDamageable) || ((!(tag is Barrier) ? 0 : (!(tag as Barrier).Solid ? 1 : 0)) | (tag is SpellMine ? 1 : 0) | (tag is ElementalEgg ? 1 : 0) | (tag is TornadoEntity ? 1 : 0) | (tag is Grease.GreaseField ? 1 : 0)) != 0 || tag is MissileEntity && (tag as MissileEntity).Owner == this.mOwner || tag == null || this.mGibbedHitList.ContainsKey(tag.Handle))
        return false;
      if (!this.mHitList.ContainsKey(tag.Handle))
      {
        EventCondition iArgs = new EventCondition();
        iArgs.EventConditionType = EventConditionType.Hit;
        NetworkState state = NetworkManager.Instance.State;
        if (!this.SendsNetworkUpdate(state))
          return true;
        DamageResult oDamageResult = DamageResult.None;
        if (this.mConditionCollection.ExecuteAll((Entity) this, tag, ref iArgs, out oDamageResult) && state != NetworkState.Offline)
        {
          MissileEntityEventMessage iMessage = new MissileEntityEventMessage();
          iMessage.Handle = this.Handle;
          iMessage.EventConditionType = iArgs.EventConditionType;
          if (tag != null)
            iMessage.TargetHandle = tag.Handle;
          iMessage.TimeAlive = iArgs.Time;
          iMessage.Threshold = iArgs.Threshold;
          iMessage.HitPoints = iArgs.Hitpoints;
          iMessage.Elements = iArgs.Elements;
          iMessage.OnCollision = true;
          NetworkManager.Instance.Interface.SendMessage<MissileEntityEventMessage>(ref iMessage);
        }
        if (this.mDeepImpactPossible && tag is Character && (oDamageResult & (DamageResult.Killed | DamageResult.OverKilled)) != DamageResult.None)
        {
          ++this.mDeepImpactCount;
          if (this.mDeepImpactCount >= 5)
            AchievementsManager.Instance.AwardAchievement(this.PlayState, "deepimpact");
        }
        if ((oDamageResult & DamageResult.OverKilled) == DamageResult.OverKilled || (oDamageResult & DamageResult.Pierced) == DamageResult.Pierced || (oDamageResult & DamageResult.Killed) == DamageResult.Killed && tag is Barrier)
        {
          if ((oDamageResult & DamageResult.Pierced) != DamageResult.Pierced)
          {
            Vector3 position1 = tag.Position;
            Vector3 position2 = this.Position;
            Vector3 result1 = this.mBody.Velocity;
            float num = result1.Length();
            Vector3 result2;
            Vector3.Subtract(ref position2, ref position1, out result2);
            Vector3.Normalize(ref result2, out result2);
            Vector3.Multiply(ref result2, num * 0.3f, out result2);
            Vector3.Normalize(ref result1, out result1);
            Vector3.Multiply(ref result1, num * 0.9f, out result1);
            Vector3 result3;
            Vector3.Add(ref result2, ref result1, out result3);
            this.mVelocityChange = true;
            this.mNewVelocity = result3;
          }
          this.mGibbedHitList.Add(tag.Handle, 5f);
          return false;
        }
        this.mHitList.Add(tag.Handle);
        if (tag is Shield | tag is Barrier || (oDamageResult & DamageResult.Deflected) != DamageResult.None | (oDamageResult & DamageResult.Hit) != DamageResult.None)
          return true;
      }
      else
        return iSkin1.Owner != null && (iSkin1.Owner.Tag is Shield || iSkin1.Owner.Tag is Barrier) || !this.mPiercing;
    }
    return false;
  }

  internal void SetProppMagickEffect(int iEffect, ref Vector3 iLeverFromMissile)
  {
    Matrix orientation = this.mBody.Orientation;
    Vector3 position = this.mBody.Position;
    this.mProppMagickEffectActive = true;
    this.mProppMagickLever = iLeverFromMissile;
    orientation.Translation = position;
    EffectManager.Instance.StartEffect(iEffect, ref orientation, out this.mProppMagickEffect);
  }

  internal override bool SendsNetworkUpdate(NetworkState iState)
  {
    if (this.mOwner is Avatar mOwner && !(mOwner.Player.Gamer is NetworkGamer))
      return true;
    return mOwner == null && iState != NetworkState.Client;
  }

  internal void NetworkEventMessage(ref MissileEntityEventMessage iMsg)
  {
    EventCondition iArgs = new EventCondition();
    iArgs.EventConditionType = iMsg.EventConditionType;
    iArgs.Time = iMsg.TimeAlive;
    iArgs.Threshold = iMsg.Threshold;
    iArgs.Hitpoints = iMsg.HitPoints;
    iArgs.Elements = iMsg.Elements;
    Entity fromHandle = Entity.GetFromHandle((int) iMsg.TargetHandle);
    DamageResult oDamageResult;
    this.mConditionCollection.ExecuteAll((Entity) this, fromHandle, ref iArgs, out oDamageResult);
    if (!iMsg.OnCollision)
      return;
    if (this.mDeepImpactPossible && fromHandle is Character && (oDamageResult & (DamageResult.Killed | DamageResult.OverKilled)) != DamageResult.None)
    {
      ++this.mDeepImpactCount;
      if (this.mDeepImpactCount >= 5)
        AchievementsManager.Instance.AwardAchievement(this.PlayState, "deepimpact");
    }
    if (this.mGibbedHitList.Contains(fromHandle as IDamageable))
      return;
    if ((oDamageResult & DamageResult.OverKilled) == DamageResult.OverKilled || (oDamageResult & DamageResult.Pierced) == DamageResult.Pierced || (oDamageResult & DamageResult.Killed) == DamageResult.Killed && fromHandle is Barrier)
      this.mGibbedHitList.Add(fromHandle.Handle, 5f);
    else
      this.mHitList.Add(fromHandle.Handle);
  }

  public bool SegmentIntersect(out Vector3 oPosition, Segment iSeg, float iSegmentRadius)
  {
    Vector3 position = this.mBody.Position;
    float t;
    float result;
    Distance.PointSegmentDistanceSq(out t, out result, ref position, ref iSeg);
    float num1 = iSegmentRadius + this.mRadius;
    float num2 = num1 * num1;
    if ((double) result > (double) num2)
    {
      oPosition = new Vector3();
      return false;
    }
    float num3 = (float) System.Math.Sqrt((double) result);
    Vector3 point;
    iSeg.GetPoint(t, out point);
    Vector3.Lerp(ref position, ref point, System.Math.Min(this.mRadius / num3, 1f), out oPosition);
    return true;
  }

  public bool ArcIntersect(
    out Vector3 oPosition,
    ref Vector3 iOrigin,
    ref Vector3 iDirection,
    float iRange,
    float iAngle,
    float iHeightDifference)
  {
    iOrigin.Y = 0.0f;
    iDirection.Y = 0.0f;
    Vector3 position1 = this.Position with { Y = 0.0f };
    Vector3 result1;
    Vector3.Subtract(ref iOrigin, ref position1, out result1);
    float num1 = result1.Length();
    if ((double) num1 - (double) this.mRadius > (double) iRange)
    {
      oPosition = new Vector3();
      return false;
    }
    Vector3.Divide(ref result1, num1, out result1);
    float result2;
    Vector3.Dot(ref result1, ref iDirection, out result2);
    float num2 = (float) System.Math.Acos((double) -result2);
    float num3 = -2f * num1 * num1;
    float num4 = (float) System.Math.Acos(((double) this.mRadius * (double) this.mRadius + (double) num3) / (double) num3);
    if ((double) num2 - (double) num4 < (double) iAngle)
    {
      Vector3.Multiply(ref result1, this.mRadius, out result1);
      Vector3 position2 = this.Position;
      Vector3.Add(ref position2, ref result1, out oPosition);
      return true;
    }
    oPosition = new Vector3();
    return false;
  }

  public override Vector3 CalcImpulseVelocity(
    Vector3 iDirection,
    float iElevation,
    float iMassPower,
    float iDistance)
  {
    return base.CalcImpulseVelocity(iDirection, iElevation, iMassPower, iDistance);
  }

  public virtual DamageResult InternalDamage(
    DamageCollection5 iDamages,
    Entity iAttacker,
    double iTimeStamp,
    Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
    DamageResult damageResult1 = this.InternalDamage(iDamages.A, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
    DamageResult damageResult2 = this.InternalDamage(iDamages.B, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
    DamageResult damageResult3 = this.InternalDamage(iDamages.C, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
    DamageResult damageResult4 = this.InternalDamage(iDamages.D, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
    int num = (int) this.InternalDamage(iDamages.E, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
    return damageResult1 | damageResult2 | damageResult3 | damageResult4;
  }

  public DamageResult InternalDamage(
    Damage iDamage,
    Entity iAttacker,
    double iTimeStamp,
    Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
    if ((double) iDamage.Amount != 0.0 && !(iAttacker is Grease.GreaseField))
    {
      this.mIsDamaged = true;
      this.mDamageAmount += iDamage.Amount;
      this.mDamageElements |= iDamage.Element;
    }
    return DamageResult.None;
  }

  public void Electrocute(IDamageable iTarget, float iMultiplyer)
  {
  }

  public float HomingTolerance
  {
    get => this.mHomingTolerance;
    set => this.mHomingTolerance = value;
  }

  public float Homing
  {
    get => this.mHomingPower;
    set => this.mHomingPower = value;
  }

  public override void Kill() => this.mDead = true;

  public void OverKill() => this.mDead = true;

  public float HitPoints => 0.0f;

  public float MaxHitPoints => 0.0f;

  public override bool Dead => this.mDead;

  public override bool Removable => this.mDead;

  public Entity Owner => this.mOwner;

  public SpellEffect CurrentSpell
  {
    get => this.mCurrentSpell;
    set => this.mCurrentSpell = value;
  }

  public void AddEffectReference(int iEffectHash, VisualEffectReference iEffect)
  {
    if (this.mEffectReferences.ContainsKey(iEffectHash))
      throw new Exception();
    this.mEffectReferences.Add(iEffectHash, iEffect);
  }

  public void AddCue(Cue iCue) => this.mSoundList.Add(iCue);

  public void AddLight(DynamicLight iLight) => this.mLights.Add(iLight);

  public float Danger
  {
    get => this.Danger;
    set => this.mDanger = value;
  }

  protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
  {
    oMsg = new EntityUpdateMessage();
    oMsg.Features |= EntityFeatures.Position;
    oMsg.Position = this.Position;
    Vector3 result = this.mBody.Velocity;
    Vector3.Multiply(ref result, iPrediction, out result);
    Vector3.Add(ref result, ref oMsg.Position, out oMsg.Position);
    oMsg.Features |= EntityFeatures.Velocity;
    oMsg.Velocity = this.mBody.Velocity;
  }

  internal override float GetDanger() => this.mDanger;

  protected class RenderData : IRenderableObject, IRenderableAdditiveObject
  {
    protected AdditiveMaterial mAdditiveMaterial;
    protected RenderDeferredMaterial mRenderDeferredMaterial;
    protected int mEffect;
    protected VertexDeclaration mVertexDeclaration;
    protected int mBaseVertex;
    protected int mNumVertices;
    protected int mPrimitiveCount;
    protected int mStartIndex;
    protected int mStreamOffset;
    protected int mVertexStride;
    protected VertexBuffer mVertexBuffer;
    protected int mVerticesHash;
    protected IndexBuffer mIndexBuffer;
    public Matrix mTransform;
    public BoundingSphere mBoundingSphere;
    protected bool mDirty;

    public bool IsDirty => this.mDirty;

    public void SetDirty() => this.mDirty = true;

    public int Effect => this.mEffect;

    public int DepthTechnique => 4;

    public int Technique => this.mEffect != AdditiveEffect.TYPEHASH ? 0 : 0;

    public int ShadowTechnique => 5;

    public VertexBuffer Vertices => this.mVertexBuffer;

    public IndexBuffer Indices => this.mIndexBuffer;

    public VertexDeclaration VertexDeclaration => this.mVertexDeclaration;

    public int VertexStride => this.mVertexStride;

    public int VerticesHashCode => this.mVerticesHash;

    public bool Cull(BoundingFrustum iViewFrustum)
    {
      return this.mBoundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
    }

    public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      AdditiveEffect iEffect1 = iEffect as AdditiveEffect;
      RenderDeferredEffect iEffect2 = iEffect as RenderDeferredEffect;
      if (iEffect1 != null)
      {
        this.mAdditiveMaterial.AssignToEffect(iEffect1);
        iEffect1.World = this.mTransform;
      }
      if (iEffect2 != null)
      {
        this.mRenderDeferredMaterial.AssignToEffect(iEffect2);
        iEffect2.World = this.mTransform;
      }
      iEffect.CommitChanges();
      iEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
    }

    public void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      RenderDeferredEffect iEffect1 = iEffect as RenderDeferredEffect;
      this.mRenderDeferredMaterial.AssignToEffect(iEffect1);
      iEffect1.World = this.mTransform;
      iEffect.CommitChanges();
      iEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
    }

    public void SetMesh(VertexBuffer iVertices, IndexBuffer iIndices, ModelMeshPart iMeshPart)
    {
      this.mDirty = false;
      if (iMeshPart == null)
      {
        this.mAdditiveMaterial = new AdditiveMaterial();
        this.mRenderDeferredMaterial = new RenderDeferredMaterial();
        this.mVertexBuffer = (VertexBuffer) null;
        this.mVerticesHash = 0;
        this.mIndexBuffer = (IndexBuffer) null;
        this.mVertexDeclaration = (VertexDeclaration) null;
        this.mBaseVertex = 0;
        this.mNumVertices = 0;
        this.mPrimitiveCount = 0;
        this.mStartIndex = 0;
        this.mStreamOffset = 0;
        this.mVertexStride = 0;
        this.mEffect = 0;
      }
      else
      {
        AdditiveEffect effect1 = iMeshPart.Effect as AdditiveEffect;
        RenderDeferredEffect effect2 = iMeshPart.Effect as RenderDeferredEffect;
        if (effect1 != null)
        {
          this.mAdditiveMaterial.FetchFromEffect(effect1);
          this.mEffect = AdditiveEffect.TYPEHASH;
        }
        if (effect2 != null)
        {
          this.mRenderDeferredMaterial.FetchFromEffect(effect2);
          this.mEffect = RenderDeferredEffect.TYPEHASH;
        }
        this.mVertexBuffer = iVertices;
        this.mVerticesHash = iVertices.GetHashCode();
        this.mIndexBuffer = iIndices;
        this.mVertexDeclaration = iMeshPart.VertexDeclaration;
        this.mBaseVertex = iMeshPart.BaseVertex;
        this.mNumVertices = iMeshPart.NumVertices;
        this.mPrimitiveCount = iMeshPart.PrimitiveCount;
        this.mStartIndex = iMeshPart.StartIndex;
        this.mStreamOffset = iMeshPart.StreamOffset;
        this.mVertexStride = iMeshPart.VertexStride;
      }
    }
  }
}
