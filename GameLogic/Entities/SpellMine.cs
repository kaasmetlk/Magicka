// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.SpellMine
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Bosses;
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
namespace Magicka.GameLogic.Entities;

public class SpellMine : Entity, IDamageable
{
  public const float RADIUS = 1f;
  public const float MAX_DISTANCE_SQ = 900f;
  private const float DETONATION_TIMER = 0.5f;
  private static Queue<SpellMine> sCache;
  public static readonly int[] DETONATION_EFFECTS = new int[11]
  {
    0,
    "water_detonation".GetHashCodeCustom(),
    "cold_detonation".GetHashCodeCustom(),
    "fire_detonation".GetHashCodeCustom(),
    "lightning_detonation".GetHashCodeCustom(),
    "arcane_detonation".GetHashCodeCustom(),
    "life_detonation".GetHashCodeCustom(),
    0,
    0,
    "steam_detonation".GetHashCodeCustom(),
    0
  };
  public static readonly int EXPLOSION_EFFECT = "mine_explosion".GetHashCodeCustom();
  public static readonly int ACTIVATION_EFFECT = "mine_activation".GetHashCodeCustom();
  public static readonly int SOUND_CAST = "spell_mine01".GetHashCodeCustom();
  public static readonly int SOUND_EXPLOSION = "spell_mine02".GetHashCodeCustom();
  public static readonly int SOUND_ACTIVATE = "spell_mine_activate".GetHashCodeCustom();
  private ISpellCaster mOwner;
  private DamageCollection5 mDamage;
  private Spell mSpell;
  private bool mActivated;
  private bool mOwnerCollision;
  private float mDetonationTimer;
  private float mNextMineTTL;
  private Vector3 mNextMineDir;
  private Quaternion mNextMineRotation;
  private float mNextMineRange;
  private float mDistanceBetweenMines;
  private float mScale;
  private SkinnedModelDeferredBasicMaterial mMaterial;
  private SkinnedModel mModel;
  private AnimationController mController;
  private AnimationClip mClip;
  private SpellMine.RenderData[] mRenderData;
  private Vector3 mDirection;
  private Vector3 mModelSize;
  private VisualEffectReference mActivationEffect;
  private double mTimeStamp;
  protected float mRestingTimer = 1f;
  private Matrix mModelMatrix;
  private static readonly Random RANDOM = new Random();

  public static SpellMine GetInstance()
  {
    SpellMine instance = SpellMine.sCache.Dequeue();
    SpellMine.sCache.Enqueue(instance);
    return instance;
  }

  public static void InitializeCache(int iNr, PlayState iPlayState)
  {
    SpellMine.sCache = new Queue<SpellMine>(iNr);
    for (int index = 0; index < iNr; ++index)
      SpellMine.sCache.Enqueue(new SpellMine(iPlayState));
  }

  public SpellMine(PlayState iPlayState)
    : base(iPlayState)
  {
    this.mBody = new Body();
    this.mBody.AllowFreezing = false;
    this.mBody.ApplyGravity = false;
    this.mBody.Tag = (object) this;
    this.mCollision = new CollisionSkin(this.mBody);
    this.mBody.CollisionSkin = this.mCollision;
    lock (Magicka.Game.Instance.GraphicsDevice)
      this.mModel = iPlayState.Content.Load<SkinnedModel>("Models/Effects/mine");
    VertexElement[] vertexElements;
    lock (Magicka.Game.Instance.GraphicsDevice)
      vertexElements = this.mModel.Model.Meshes[0].MeshParts[0].VertexDeclaration.GetVertexElements();
    int offsetInBytes = -1;
    for (int index = 0; index < vertexElements.Length; ++index)
    {
      if (vertexElements[index].VertexElementUsage == VertexElementUsage.Position)
      {
        offsetInBytes = (int) vertexElements[index].Offset;
        break;
      }
    }
    if (offsetInBytes < 0)
      throw new Exception("No positions found");
    Vector3[] vector3Array = new Vector3[this.mModel.Model.Meshes[0].MeshParts[0].NumVertices];
    this.mModel.Model.Meshes[0].VertexBuffer.GetData<Vector3>(offsetInBytes, vector3Array, this.mModel.Model.Meshes[0].MeshParts[0].StartIndex, vector3Array.Length, this.mModel.Model.Meshes[0].MeshParts[0].VertexStride);
    BoundingBox fromPoints = BoundingBox.CreateFromPoints((IEnumerable<Vector3>) vector3Array);
    Vector3.Subtract(ref fromPoints.Max, ref fromPoints.Min, out this.mModelSize);
    this.mController = new AnimationController();
    this.mController.Skeleton = this.mModel.SkeletonBones;
    this.mClip = this.mModel.AnimationClips["Take 001"];
    Helper.SkinnedModelDeferredMaterialFromBasicEffect(this.mModel.Model.Meshes[0].MeshParts[0].Effect as SkinnedModelBasicEffect, out this.mMaterial);
    this.mRenderData = new SpellMine.RenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      this.mRenderData[index] = new SpellMine.RenderData();
      this.mRenderData[index].SetMesh(this.mModel.Model.Meshes[0].VertexBuffer, this.mModel.Model.Meshes[0].IndexBuffer, this.mModel.Model.Meshes[0].MeshParts[0], ref this.mMaterial, (SkinnedModelDeferredEffect.Technique) (this.mModel.Model.Meshes[0].MeshParts[0].Effect as SkinnedModelBasicEffect).ActiveTechnique);
    }
    this.mCollision.AddPrimitive((Primitive) new Sphere(Vector3.Zero, 1f), 1, new MaterialProperties(0.0f, 0.8f, 0.8f));
    this.mCollision.callbackFn += new CollisionCallbackFn(this.OnCollision);
  }

  public void Initialize(
    ISpellCaster iOwner,
    Vector3 iPosition,
    Vector3 iDirection,
    float iScale,
    float iRange,
    Vector3 iNextDirection,
    Quaternion iNextRotation,
    float iDistanceBetweenMines,
    ref Spell iSpell,
    ref DamageCollection5 iDamage,
    AnimatedLevelPart iAnimation)
  {
    iAnimation?.AddEntity((Entity) this);
    iPosition.Y += 0.1f;
    this.Deinitialize();
    this.mTimeStamp = iOwner.PlayState.PlayTime;
    this.mScale = iScale;
    this.mRadius = 1f;
    this.mSpell = iSpell;
    this.mDirection = iDirection;
    this.mNextMineTTL = 0.1f;
    Vector3.Transform(ref iNextDirection, ref iNextRotation, out this.mNextMineDir);
    this.mNextMineRotation = iNextRotation;
    this.mNextMineRange = iRange - iDistanceBetweenMines;
    this.mDistanceBetweenMines = iDistanceBetweenMines;
    this.mActivationEffect.Hash = 0;
    this.mDetonationTimer = 0.0f;
    this.mOwner = iOwner;
    this.mDead = false;
    this.mOwnerCollision = false;
    this.mModelMatrix = Matrix.CreateScale(2.1f);
    Matrix rotationY = Matrix.CreateRotationY((float) SpellMine.RANDOM.NextDouble() * 6.28318548f);
    Matrix.Multiply(ref this.mModelMatrix, ref rotationY, out this.mModelMatrix);
    this.mBody.MoveTo(iPosition, Matrix.Identity);
    this.mSpell = iSpell;
    iSpell.CalculateDamage(SpellType.Shield, CastType.Force, out this.mDamage);
    this.mCollision.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
    (this.mCollision.GetPrimitiveLocal(0) as Sphere).Radius = 0.9f;
    (this.mCollision.GetPrimitiveNewWorld(0) as Sphere).Radius = 0.9f;
    (this.mCollision.GetPrimitiveOldWorld(0) as Sphere).Radius = 0.9f;
    this.Initialize();
    this.mActivated = false;
    this.mMaterial.TintColor = iSpell.GetColor();
    for (int index = 0; index < 3; ++index)
    {
      this.mRenderData[index].mMaterial.TintColor = iSpell.GetColor();
      this.mRenderData[index].mBoundingSphere.Center = iPosition;
      this.mRenderData[index].mBoundingSphere.Radius = 1f;
    }
    AudioManager.Instance.PlayCue(Banks.Spells, SpellMine.SOUND_CAST, this.AudioEmitter);
    this.mController.Speed = 3f;
    this.mController.PlaybackMode = PlaybackMode.Forward;
    this.mController.StartClip(this.mClip, false);
    List<Entity> entities = this.mPlayState.EntityManager.GetEntities(iPosition, 1f, false);
    for (int index = 0; index < entities.Count; ++index)
    {
      Barrier barrier = entities[index] as Barrier;
      SpellMine spellMine = entities[index] as SpellMine;
      if (barrier != null)
        barrier.Kill();
      else
        spellMine?.Detonate();
    }
    this.mPlayState.EntityManager.ReturnEntityList(entities);
  }

  public bool Resting => (double) this.mRestingTimer < 0.0;

  public void Detonate()
  {
    Vector3 position = this.Position;
    Vector3 forward = this.mBody.Orientation.Forward;
    int num = (int) Blast.FullBlast(this.mPlayState, this.mOwner as Entity, this.mTimeStamp, (Entity) this, 3f, position, this.mDamage);
    EffectManager.Instance.StartEffect(SpellMine.EXPLOSION_EFFECT, ref position, ref forward, out VisualEffectReference _);
    Elements elements1 = this.mSpell.Element & ~Elements.Shield;
    for (int iIndex = 0; iIndex < 11; ++iIndex)
    {
      Elements elements2 = Defines.ElementFromIndex(iIndex);
      if ((elements2 & elements1) == elements2)
        AudioManager.Instance.PlayCue(Banks.Spells, Defines.SOUNDS_AREA[iIndex], this.AudioEmitter);
    }
    this.mDead = true;
  }

  protected bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
  {
    if (iSkin1.Owner != null & !this.mDead & (double) this.mDetonationTimer <= 0.0 & this.mController.HasFinished && iSkin1.Owner.Tag is IDamageable tag && !tag.Dead)
    {
      switch (tag)
      {
        case Character _:
        case BossDamageZone _:
        case MissileEntity _:
          if (tag is Character && ((tag as Character).IsEthereal || (tag as Character).IsLevitating))
            return false;
          if (tag == this.mOwner && !this.mActivated)
          {
            this.mOwnerCollision = true;
            break;
          }
          if (this.mController.HasFinished)
          {
            this.mDetonationTimer = 0.5f;
            if (this.mActivationEffect.Hash == 0)
            {
              Vector3 position = this.Position;
              Vector3 forward = Vector3.Forward;
              EffectManager.Instance.StartEffect(SpellMine.ACTIVATION_EFFECT, ref position, ref forward, out this.mActivationEffect);
              AudioManager.Instance.PlayCue(Banks.Spells, SpellMine.SOUND_ACTIVATE, this.AudioEmitter);
              break;
            }
            break;
          }
          break;
      }
    }
    return false;
  }

  public float ResistanceAgainst(Elements iElement) => 0.0f;

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    base.Update(iDataChannel, iDeltaTime);
    if ((double) this.mNextMineRange > 1.4012984643248171E-45)
    {
      this.mNextMineTTL -= iDeltaTime;
      if ((double) this.mNextMineTTL < 0.0)
      {
        this.SpawnNextMine();
        this.mNextMineRange = 0.0f;
      }
    }
    Matrix result1 = this.mBody.Transform.Orientation with
    {
      Translation = this.mBody.Transform.Position
    };
    if (!this.mOwnerCollision)
    {
      this.mActivated = true;
      if ((double) this.mDetonationTimer > 0.0)
      {
        this.mDetonationTimer -= iDeltaTime;
        if ((double) this.mDetonationTimer <= 0.0)
          this.Detonate();
      }
    }
    if (this.mOwner != null)
    {
      Vector3 position1 = this.mOwner.Position;
      Vector3 position2 = this.Position;
      float result2;
      Vector3.DistanceSquared(ref position1, ref position2, out result2);
      if ((double) result2 > 900.0)
        this.Detonate();
    }
    if ((double) this.mBody.Velocity.LengthSquared() > 9.9999999747524271E-07)
      this.mRestingTimer = 1f;
    else
      this.mRestingTimer -= iDeltaTime;
    SpellMine.RenderData iObject = this.mRenderData[(int) iDataChannel];
    Matrix.Multiply(ref this.mModelMatrix, ref result1, out result1);
    result1.Translation -= new Vector3(0.0f, 0.1f, 0.0f);
    iObject.mBoundingSphere.Center = result1.Translation;
    this.mController.Update(iDeltaTime, ref result1, true);
    Array.Copy((Array) this.mController.SkinnedBoneTransforms, 0, (Array) iObject.mBones, 0, 80 /*0x50*/);
    this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) iObject);
    this.mOwnerCollision = false;
  }

  protected void SpawnNextMine()
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    Vector3 vector3 = this.Position + this.mNextMineDir;
    Segment iSeg1 = new Segment();
    iSeg1.Delta.Y = -1.5f;
    iSeg1.Origin = vector3;
    iSeg1.Origin.Y += 0.75f;
    List<Shield> shields = this.mPlayState.EntityManager.Shields;
    Segment iSeg2 = new Segment();
    iSeg2.Origin = this.Position;
    Vector3.Subtract(ref iSeg1.Origin, ref iSeg2.Origin, out iSeg2.Delta);
    bool flag = false;
    for (int index = 0; index < shields.Count; ++index)
    {
      if (shields[index].SegmentIntersect(out Vector3 _, iSeg2, 1f))
        flag = true;
    }
    Vector3 oPos;
    AnimatedLevelPart oAnimatedLevelPart;
    if (flag || !this.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out Vector3 _, out oAnimatedLevelPart, iSeg1))
      return;
    Vector3 result = this.mBody.Orientation.Forward;
    Vector3.Transform(ref result, ref this.mNextMineRotation, out result);
    List<Entity> entities = this.mPlayState.EntityManager.GetEntities(oPos, 1f, false);
    for (int index = 0; index < entities.Count; ++index)
    {
      if (entities[index] is Barrier)
        entities[index].Kill();
      else if (entities[index] is SpellMine)
        (entities[index] as SpellMine).Detonate();
    }
    this.mPlayState.EntityManager.ReturnEntityList(entities);
    SpellMine instance = SpellMine.GetInstance();
    if (NetworkManager.Instance.State == NetworkState.Server)
    {
      SpawnMineMessage iMessage;
      iMessage.Handle = instance.Handle;
      iMessage.OwnerHandle = this.mOwner.Handle;
      iMessage.AnimationHandle = oAnimatedLevelPart == null ? ushort.MaxValue : oAnimatedLevelPart.Handle;
      iMessage.Position = oPos;
      iMessage.Direction = result;
      iMessage.Scale = this.mScale;
      iMessage.Spell = this.mSpell;
      iMessage.Damage = this.mDamage;
      NetworkManager.Instance.Interface.SendMessage<SpawnMineMessage>(ref iMessage);
    }
    instance.Initialize(this.mOwner, oPos, result, this.mScale, this.mNextMineRange, this.mNextMineDir, this.mNextMineRotation, this.mDistanceBetweenMines, ref this.mSpell, ref this.mDamage, oAnimatedLevelPart);
    this.mPlayState.EntityManager.AddEntity((Entity) instance);
  }

  public override void Deinitialize()
  {
    base.Deinitialize();
    this.mCollision.NonCollidables.Clear();
  }

  public override bool Dead => this.mDead;

  public override bool Removable => this.mDead && this.mController.HasFinished;

  public override void Kill() => this.mDead = true;

  protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
  {
    oMsg = new EntityUpdateMessage();
    if (this.Resting)
      return;
    oMsg.Features |= EntityFeatures.Position;
    oMsg.Position = this.Position;
  }

  internal override float GetDanger() => (double) this.mDetonationTimer <= 0.0 ? 1f : 10000f;

  public float HitPoints => 1f;

  public float MaxHitPoints => 1f;

  public bool SegmentIntersect(out Vector3 oPosition, Segment iSeg, float iSegmentRadius)
  {
    return this.mBody.CollisionSkin.SegmentIntersect(out float _, out oPosition, out Vector3 _, iSeg);
  }

  public DamageResult InternalDamage(
    DamageCollection5 iDamages,
    Entity iAttacker,
    double iTimeStamp,
    Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
    DamageResult damageResult = DamageResult.None | this.InternalDamage(iDamages.A, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.B, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.C, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.D, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.E, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
    if ((damageResult & DamageResult.Damaged) == DamageResult.Damaged)
      this.mDetonationTimer = 0.1f;
    return damageResult;
  }

  public DamageResult InternalDamage(
    Damage iDamage,
    Entity iAttacker,
    double iTimeStamp,
    Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
    return (double) Math.Abs(iDamage.Amount * iDamage.Magnitude) > 0.0 && ((iDamage.AttackProperty & AttackProperties.Damage) == AttackProperties.Damage || (iDamage.AttackProperty & AttackProperties.Status) == AttackProperties.Status) ? DamageResult.Damaged : DamageResult.None;
  }

  public void OverKill() => this.Detonate();

  public void Electrocute(IDamageable iTarget, float iMultiplyer)
  {
  }

  protected class RenderData : IRenderableObject
  {
    public BoundingSphere mBoundingSphere;
    protected VertexDeclaration mVertexDeclaration;
    protected int mBaseVertex;
    protected int mNumVertices;
    protected int mPrimitiveCount;
    protected int mStartIndex;
    protected int mStreamOffset;
    protected int mVertexStride;
    protected VertexBuffer mVertexBuffer;
    protected IndexBuffer mIndexBuffer;
    public Matrix[] mBones;
    public SkinnedModelDeferredAdvancedMaterial mMaterial;
    public SkinnedModelDeferredEffect.Technique mTechnique;
    protected int mVerticesHash;
    protected bool mMeshDirty = true;

    public RenderData() => this.mBones = new Matrix[80 /*0x50*/];

    public bool MeshDirty => this.mMeshDirty;

    public int Effect => SkinnedModelDeferredEffect.TYPEHASH;

    public int DepthTechnique => 3;

    public int Technique => (int) this.mTechnique;

    public int ShadowTechnique => 4;

    public VertexBuffer Vertices => this.mVertexBuffer;

    public IndexBuffer Indices => this.mIndexBuffer;

    public VertexDeclaration VertexDeclaration => this.mVertexDeclaration;

    public int VertexStride => this.mVertexStride;

    public int VerticesHashCode => this.mVerticesHash;

    public bool Cull(BoundingFrustum iViewFrustum)
    {
      return this.mBoundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
    }

    public virtual void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      SkinnedModelDeferredEffect iEffect1 = iEffect as SkinnedModelDeferredEffect;
      this.mMaterial.AssignToEffect(iEffect1);
      iEffect1.EmissiveAmount = 2f;
      iEffect1.Bones = this.mBones;
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
    }

    public virtual void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      if (this.mTechnique == SkinnedModelDeferredEffect.Technique.Additive)
        return;
      SkinnedModelDeferredEffect iEffect1 = iEffect as SkinnedModelDeferredEffect;
      this.mMaterial.AssignOpacityToEffect(iEffect1);
      iEffect1.Bones = this.mBones;
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
    }

    public void SetMeshDirty() => this.mMeshDirty = true;

    public void SetMesh(
      VertexBuffer iVertices,
      IndexBuffer iIndices,
      ModelMeshPart iMeshPart,
      ref SkinnedModelDeferredBasicMaterial iBasicMaterial,
      SkinnedModelDeferredEffect.Technique iTechnique)
    {
      this.mMeshDirty = false;
      this.mMaterial.CopyFrom(ref iBasicMaterial);
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
      this.mTechnique = iTechnique;
    }
  }
}
