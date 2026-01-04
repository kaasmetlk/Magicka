// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Dispenser
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Magicka.AI;
using Magicka.GameLogic.GameStates;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using System;
using System.Collections.Generic;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

#nullable disable
namespace Magicka.GameLogic.Entities;

internal class Dispenser : Entity, IDamageable
{
  public const float RADIUS = 1.25f;
  public const float LENGTH = 1f;
  private static List<Dispenser> mCache;
  private static SkinnedModel[] sDispenserModels;
  private static AnimationClip[][] sAnimationClips;
  private Dispenser.RenderData[] mRenderData;
  private SkinnedModel mModel;
  private AnimationController mAnimationController;
  public Dispensers mType;
  private float mHitPoints;
  private float mMaxHitPoints;
  private float mTimeBetween = 2f;
  private float mTimer;
  private List<int> mTypeID;
  private List<int> mTypeAmount;
  private bool mActive = true;
  protected float mVolume;
  protected Resistance[] mResistances;

  public static void InitializeCache(int iNrOfShields, PlayState iPlayState)
  {
    Dispenser.mCache = new List<Dispenser>(iNrOfShields);
    for (int index = 0; index < iNrOfShields; ++index)
      Dispenser.mCache.Add(new Dispenser(iPlayState));
  }

  public static Dispenser GetFromCache(PlayState iPlayState)
  {
    if (Dispenser.mCache.Count <= 0)
      return new Dispenser(iPlayState);
    Dispenser fromCache = Dispenser.mCache[Dispenser.mCache.Count - 1];
    Dispenser.mCache.RemoveAt(Dispenser.mCache.Count - 1);
    return fromCache;
  }

  public Dispenser(PlayState iPlayState)
    : base(iPlayState)
  {
    if (Dispenser.sDispenserModels == null)
    {
      int length = 3;
      Dispenser.sAnimationClips = new AnimationClip[length][];
      Dispenser.sDispenserModels = new SkinnedModel[length];
      lock (Magicka.Game.Instance.GraphicsDevice)
      {
        for (int index = 0; index < length; ++index)
        {
          Dispenser.sDispenserModels[index] = Magicka.Game.Instance.Content.Load<SkinnedModel>("Models/Effects/" + ((Dispensers) index).ToString());
          Dispenser.sAnimationClips[index] = new AnimationClip[4];
          Dispenser.sDispenserModels[index].AnimationClips.TryGetValue("destroyed", out Dispenser.sAnimationClips[index][3]);
          Dispenser.sDispenserModels[index].AnimationClips.TryGetValue("spawn", out Dispenser.sAnimationClips[index][0]);
          Dispenser.sDispenserModels[index].AnimationClips.TryGetValue("idle", out Dispenser.sAnimationClips[index][2]);
          Dispenser.sDispenserModels[index].AnimationClips.TryGetValue("execute", out Dispenser.sAnimationClips[index][1]);
        }
      }
    }
    this.mAnimationController = new AnimationController();
    this.mAnimationController.ClipSpeed = 1f;
    this.mBody = new Body();
    this.mCollision = new CollisionSkin(this.mBody);
    this.mCollision.AddPrimitive((Primitive) new Capsule(Vector3.Down, Matrix.CreateRotationX(1.57079637f), 1.25f, 1f), 1, new MaterialProperties(0.0f, 0.8f, 0.8f));
    this.mCollision.callbackFn += new CollisionCallbackFn(this.OnCollision);
    this.mBody.CollisionSkin = this.mCollision;
    this.mBody.Immovable = true;
    this.mBody.Tag = (object) this;
    this.mRenderData = new Dispenser.RenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      Dispenser.RenderData renderData = new Dispenser.RenderData();
      this.mRenderData[index] = renderData;
    }
    this.mTypeID = new List<int>(32 /*0x20*/);
    this.mTypeAmount = new List<int>(32 /*0x20*/);
    this.mResistances = new Resistance[11];
    for (int iIndex = 0; iIndex < this.mResistances.Length; ++iIndex)
    {
      this.mResistances[iIndex].ResistanceAgainst = Defines.ElementFromIndex(iIndex);
      this.mResistances[iIndex].Multiplier = 1f;
      this.mResistances[iIndex].Modifier = 0.0f;
    }
    this.mType = Dispensers.NrOfModels;
  }

  protected bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
  {
    return false;
  }

  public override Matrix GetOrientation()
  {
    Vector3 position = this.mBody.Position;
    position.Y -= 1.75f;
    return this.mBody.Orientation with
    {
      Translation = position
    };
  }

  public void Initialize(
    Matrix iTransform,
    Dispensers iModel,
    int[] iTypeID,
    int[] iAmount,
    float iTimeBetween,
    bool iActive)
  {
    this.mActive = iActive;
    this.mType = iModel;
    for (int index = 0; index < iTypeID.Length; ++index)
      this.mTypeID.Add(iTypeID[index]);
    for (int index = 0; index < iAmount.Length; ++index)
      this.mTypeAmount.Add(iAmount[index]);
    if (this.mTypeID.Count < this.mTypeAmount.Count)
      this.mTypeAmount.RemoveRange(this.mTypeID.Count, this.mTypeAmount.Count - this.mTypeID.Count);
    this.mTimeBetween = iTimeBetween;
    this.mTimer = 0.0f;
    this.mModel = Dispenser.sDispenserModels[(int) iModel];
    this.mRadius = 1.25f;
    (this.mCollision.GetPrimitiveLocal(0) as Capsule).Radius = this.mRadius;
    (this.mCollision.GetPrimitiveNewWorld(0) as Capsule).Radius = this.mRadius;
    (this.mCollision.GetPrimitiveOldWorld(0) as Capsule).Radius = this.mRadius;
    this.mVolume = (this.mCollision.GetPrimitiveLocal(0) as Capsule).GetVolume();
    Segment iSeg = new Segment(iTransform.Translation + Vector3.Up, Vector3.Down * 4f);
    Vector3 oPos;
    if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out Vector3 _, iSeg))
      iTransform.Translation = oPos;
    iTransform.Translation += new Vector3(0.0f, 1.75f, 0.0f);
    Matrix orientation = iTransform with
    {
      Translation = new Vector3()
    };
    this.mBody.MoveTo(iTransform.Translation, orientation);
    this.mHitPoints = 500f;
    this.mMaxHitPoints = this.mHitPoints;
    this.mPlayState.EntityManager.AddEntity((Entity) this);
    this.mAnimationController.Skeleton = this.mModel.SkeletonBones;
    for (int index = 0; index < 3; ++index)
    {
      Dispenser.RenderData renderData = this.mRenderData[index];
      renderData.mBoundingSphere.Center = this.Position;
      renderData.mBoundingSphere.Radius = this.mRadius * 1.333f;
      renderData.SetMesh(this.mModel.Model.Meshes[0].VertexBuffer, this.mModel.Model.Meshes[0].IndexBuffer, this.mModel.Model.Meshes[0].MeshParts[0], SkinnedModelBasicEffect.TYPEHASH);
    }
    if (Dispenser.sAnimationClips[(int) this.mType][0] != null)
      this.mAnimationController.StartClip(Dispenser.sAnimationClips[(int) this.mType][0], false);
    this.Initialize();
  }

  public override void Deinitialize()
  {
    this.mAnimationController.StartClip(Dispenser.sAnimationClips[(int) this.mType][3], false);
    this.mDead = true;
    this.mHitPoints = 0.0f;
    base.Deinitialize();
    Dispenser.mCache.Add(this);
  }

  public float ResistanceAgainst(Elements iElement) => 0.0f;

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    Matrix orientation = this.GetOrientation();
    this.mAnimationController.Update(iDeltaTime, ref orientation, true);
    base.Update(iDataChannel, iDeltaTime);
    if (this.mAnimationController.HasFinished && this.mActive)
    {
      this.mTimer -= iDeltaTime;
      if ((double) this.mTimer <= 0.0)
      {
        this.mTimer = this.mTimeBetween;
        if (!this.Spawn())
        {
          this.mHitPoints = 0.0f;
          if (Dispenser.sAnimationClips[(int) this.mType][3] != null)
            this.mAnimationController.StartClip(Dispenser.sAnimationClips[(int) this.mType][3], false);
        }
      }
    }
    Dispenser.RenderData iObject = this.mRenderData[(int) iDataChannel];
    this.mAnimationController.SkinnedBoneTransforms.CopyTo((Array) iObject.mBones, 0);
    this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) iObject);
  }

  private bool Spawn()
  {
    if (this.mTypeID.Count == 0)
      return false;
    if (this.mTypeAmount.Count != 1)
    {
      for (int index = 0; index < this.mTypeAmount.Count; ++index)
      {
        if (this.mTypeAmount[index] == 0)
        {
          this.mTypeAmount.RemoveAt(index);
          this.mTypeID.RemoveAt(index);
          --index;
        }
      }
    }
    int index1 = MagickaMath.Random.Next(0, this.mTypeID.Count);
    if ((this.mTypeAmount.Count < index1 || this.mTypeAmount[index1] <= 0) && (this.mTypeAmount.Count != 1 || this.mTypeAmount[0] <= 0))
      return false;
    if (Dispenser.sAnimationClips[(int) this.mType][1] != null)
      this.mAnimationController.StartClip(Dispenser.sAnimationClips[(int) this.mType][1], false);
    NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(this.mPlayState);
    CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(this.mTypeID[index1]);
    instance.Initialize(cachedTemplate, this.Position, 0);
    instance.AI.SetOrder(Order.Attack, ReactTo.None, Order.None, 0, -1, 0, (AIEvent[]) null);
    instance.GoToAnimation(Animations.spawn, 1f / 1000f);
    instance.Body.Orientation = this.mBody.Orientation;
    instance.CharacterBody.DesiredDirection = this.mBody.Orientation.Forward;
    this.mPlayState.EntityManager.AddEntity((Entity) instance);
    if (this.mPlayState.Level.CurrentScene.RuleSet is SurvivalRuleset)
      (this.mPlayState.Level.CurrentScene.RuleSet as SurvivalRuleset).AddedCharacter(instance, false);
    if (this.mTypeAmount[index1] != -1)
    {
      List<int> mTypeAmount;
      int index2;
      (mTypeAmount = this.mTypeAmount)[index2 = index1] = mTypeAmount[index2] - 1;
    }
    else
    {
      List<int> mTypeAmount;
      (mTypeAmount = this.mTypeAmount)[0] = mTypeAmount[0] - 1;
    }
    return true;
  }

  public override Vector3 CalcImpulseVelocity(
    Vector3 iDirection,
    float iElevation,
    float iMassPower,
    float iDistance)
  {
    return new Vector3();
  }

  public override bool Dead => (double) this.mHitPoints <= 0.0;

  public override bool Removable => this.Dead && this.mAnimationController.HasFinished;

  public override Vector3 Position => this.mBody.Position;

  public virtual float Volume => this.mVolume;

  public void SetSlow()
  {
  }

  public void ActiveToggle() => this.mActive = !this.mActive;

  internal void Activate() => this.mActive = true;

  public void Damage(float iDamage) => this.mHitPoints -= iDamage;

  public virtual DamageResult InternalDamage(
    DamageCollection5 iDamages,
    Entity iAttacker,
    double iTimeStamp,
    Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
    return DamageResult.None | this.InternalDamage(iDamages.A, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.B, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.C, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.D, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.E, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
  }

  public DamageResult InternalDamage(
    Magicka.GameLogic.Damage iDamage,
    Entity iAttacker,
    double iTimeStamp,
    Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
    DamageResult damageResult1 = DamageResult.None;
    float num1 = 0.0f;
    float num2 = 0.0f;
    for (int iIndex = 0; iIndex < this.mResistances.Length; ++iIndex)
    {
      Elements elements = Defines.ElementFromIndex(iIndex);
      if ((iDamage.Element & elements) == elements)
      {
        if (iDamage.Element == Elements.Earth && (double) this.mResistances[iIndex].Modifier != 0.0)
          iDamage.Amount = Math.Max(iDamage.Amount + (float) (int) this.mResistances[iIndex].Modifier, 0.0f);
        else
          iDamage.Amount += (float) (int) this.mResistances[iIndex].Modifier;
        num1 += this.mResistances[iIndex].Multiplier;
        ++num2;
      }
    }
    if ((double) num2 != 0.0)
      iDamage.Magnitude *= num1 / num2;
    iDamage.Amount = (float) (int) ((double) iDamage.Amount * (double) iDamage.Magnitude);
    DamageResult damageResult2 = (double) iDamage.Amount > 0.0 ? damageResult1 | DamageResult.Damaged : damageResult1 | DamageResult.Deflected;
    if (Defines.FeatureDamage(iFeatures))
      this.mHitPoints -= iDamage.Amount;
    return damageResult2;
  }

  public void Electrocute(IDamageable iTarget, float iMultiplyer)
  {
  }

  public override void Kill() => this.mHitPoints = 0.0f;

  public Capsule Capsule => this.mCollision.GetPrimitiveNewWorld(0) as Capsule;

  public float HitPoints => this.mHitPoints;

  public float MaxHitPoints => this.mMaxHitPoints;

  public bool SegmentIntersect(out Vector3 oPosition, Segment iSeg, float iSegmentRadius)
  {
    Segment seg0 = new Segment();
    seg0.Origin = this.Capsule.Position;
    seg0.Delta = this.Capsule.Orientation.Forward;
    Vector3.Multiply(ref seg0.Delta, this.Capsule.Length, out seg0.Delta);
    float t0;
    float t1;
    float num1 = Distance.SegmentSegmentDistanceSq(out t0, out t1, seg0, iSeg);
    float num2 = iSegmentRadius + this.Capsule.Radius;
    float num3 = num2 * num2;
    if ((double) num1 > (double) num3)
    {
      oPosition = new Vector3();
      return false;
    }
    Vector3 result1;
    Vector3.Multiply(ref seg0.Delta, t0, out result1);
    Vector3.Add(ref seg0.Origin, ref result1, out result1);
    Vector3 result2;
    Vector3.Multiply(ref iSeg.Delta, t1, out result2);
    Vector3.Add(ref iSeg.Origin, ref result2, out result2);
    result2.Normalize();
    Vector3.Multiply(ref result2, this.Capsule.Radius, out result2);
    Vector3.Subtract(ref result2, ref result1, out oPosition);
    Vector3.Add(ref seg0.Origin, ref result1, out oPosition);
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
    float radius = this.Capsule.Radius;
    if ((double) num1 - (double) radius > (double) iRange)
    {
      oPosition = new Vector3();
      return false;
    }
    Vector3.Divide(ref result1, num1, out result1);
    float result2;
    Vector3.Dot(ref result1, ref iDirection, out result2);
    float num2 = (float) Math.Acos((double) -result2);
    float num3 = -2f * num1 * num1;
    float num4 = (float) Math.Acos(((double) radius * (double) radius + (double) num3) / (double) num3);
    if ((double) num2 - (double) num4 < (double) iAngle)
    {
      Vector3.Multiply(ref result1, radius, out result1);
      Vector3 position2 = this.Position;
      Vector3.Add(ref position2, ref result1, out oPosition);
      return true;
    }
    oPosition = new Vector3();
    return false;
  }

  public void OverKill() => this.mHitPoints = 0.0f;

  protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
  {
    oMsg = new EntityUpdateMessage();
  }

  protected class RenderData : IRenderableObject
  {
    protected int mEffect;
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
    public BoundingSphere mBoundingSphere;
    private SkinnedModelMaterial mSkinnedModelMaterial;
    public int mVerticesHash;

    public RenderData() => this.mBones = new Matrix[80 /*0x50*/];

    public int Effect => this.mEffect;

    public int DepthTechnique => 3;

    public int Technique => 0;

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

    public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      SkinnedModelBasicEffect iEffect1 = iEffect as SkinnedModelBasicEffect;
      this.mSkinnedModelMaterial.AssignToEffect(iEffect1);
      iEffect1.Bones = this.mBones;
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
    }

    public void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      SkinnedModelBasicEffect iEffect1 = iEffect as SkinnedModelBasicEffect;
      this.mSkinnedModelMaterial.AssignOpacityToEffect(iEffect1);
      iEffect1.Bones = this.mBones;
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
    }

    public void SetMesh(
      VertexBuffer iVertices,
      IndexBuffer iIndices,
      ModelMeshPart iMeshPart,
      int iEffectHash)
    {
      this.mVertexBuffer = iVertices;
      this.mVertexStride = iVertices.GetHashCode();
      this.mIndexBuffer = iIndices;
      this.mEffect = iEffectHash;
      this.mVertexDeclaration = iMeshPart.VertexDeclaration;
      this.mBaseVertex = iMeshPart.BaseVertex;
      this.mNumVertices = iMeshPart.NumVertices;
      this.mPrimitiveCount = iMeshPart.PrimitiveCount;
      this.mStartIndex = iMeshPart.StartIndex;
      this.mStreamOffset = iMeshPart.StreamOffset;
      this.mVertexStride = iMeshPart.VertexStride;
      SkinnedModelMaterial.CreateFromEffect(iMeshPart.Effect as SkinnedModelBasicEffect, out this.mSkinnedModelMaterial);
    }
  }
}
