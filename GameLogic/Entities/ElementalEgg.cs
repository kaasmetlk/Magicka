// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.ElementalEgg
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Magicka.AI;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

#nullable disable
namespace Magicka.GameLogic.Entities;

internal class ElementalEgg : Entity, IStatusEffected, IDamageable
{
  private const Elements ACCEPTABLE_ELEMENTS = Elements.StatusEffects | Elements.Lightning | Elements.Arcane;
  private static List<ElementalEgg> sCache;
  private float mHitpoints;
  private float mMaxHitPoints;
  private static readonly Elements[] RANDOM_LOOKUP = new Elements[8]
  {
    Elements.Arcane,
    Elements.Cold,
    Elements.Poison,
    Elements.Fire,
    Elements.Life,
    Elements.Water,
    Elements.Steam,
    Elements.Lightning
  };
  private Dictionary<Elements, float> mDamageMemory = new Dictionary<Elements, float>(1);
  private static readonly Random sRandom = new Random();
  private ElementalEgg.RenderData[] mRenderData;
  private SkinnedModel mModel;
  private AnimationController mController;
  private AnimationClip mClip;
  private Elements mSpawnElement;
  private static Dictionary<Elements, CharacterTemplate> sTemplateLookup;
  private ISpellCaster mSummoner;
  private float mProximity;
  private float mProximityTimer;
  private float mHeightOffset;
  private float mScale;
  protected float mRestingTimer = 1f;

  public static ElementalEgg GetInstance(PlayState iPlayState)
  {
    ElementalEgg instance = ElementalEgg.sCache.Count > 0 ? ElementalEgg.sCache[0] : throw new Exception("Cache was not initialized!");
    ElementalEgg.sCache.RemoveAt(0);
    ElementalEgg.sCache.Add(instance);
    return instance;
  }

  public static void InitializeCache(int iNr, PlayState iPlayState)
  {
    ElementalEgg.sTemplateLookup = new Dictionary<Elements, CharacterTemplate>(8);
    for (int iIndex = 0; iIndex < 11; ++iIndex)
    {
      Elements key = Spell.ElementFromIndex(iIndex);
      if ((key & (Elements.StatusEffects | Elements.Lightning | Elements.Arcane)) == key)
        ElementalEgg.sTemplateLookup.Add(key, iPlayState.Content.Load<CharacterTemplate>($"Data/Characters/Elemental_{key}"));
    }
    ElementalEgg.sCache = new List<ElementalEgg>(iNr);
    for (int index = 0; index < iNr; ++index)
      ElementalEgg.sCache.Add(new ElementalEgg(iPlayState));
  }

  private ElementalEgg(PlayState iPlayState)
    : base(iPlayState)
  {
    SkinnedModel skinnedModel = (SkinnedModel) null;
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      this.mModel = iPlayState.Content.Load<SkinnedModel>("Models/Characters/Elemental/elemental_mesh");
      skinnedModel = iPlayState.Content.Load<SkinnedModel>("Models/Characters/Elemental/elemental_animation");
    }
    this.mBody = new Body();
    this.mCollision = new CollisionSkin(this.mBody);
    this.mBody.CollisionSkin = this.mCollision;
    this.mBody.AllowFreezing = false;
    this.mBody.ApplyGravity = false;
    this.mBody.Tag = (object) this;
    this.mCollision.callbackFn += new CollisionCallbackFn(this.OnCollision);
    this.mCollision.postCollisionCallbackFn += new PostCollisionCallbackFn(this.PostCollision);
    this.mScale = ElementalEgg.sTemplateLookup[Elements.Fire].Models[0].Scale;
    this.mRadius = ElementalEgg.sTemplateLookup[Elements.Fire].Radius;
    float length = ElementalEgg.sTemplateLookup[Elements.Fire].Length;
    this.mHeightOffset = ElementalEgg.sTemplateLookup[Elements.Fire].Length * 0.5f + ElementalEgg.sTemplateLookup[Elements.Fire].Radius;
    this.mCollision.AddPrimitive((Primitive) new Capsule(new Vector3(), Matrix.CreateRotationX(-1.57079637f), this.mRadius, length), 1, new MaterialProperties(0.0f, 0.0f, 0.0f));
    this.mController = new AnimationController();
    this.mController.Skeleton = this.mModel.SkeletonBones;
    this.mClip = skinnedModel.AnimationClips["spawn"];
    SkinnedModelDeferredBasicMaterial oMaterial;
    Helper.SkinnedModelDeferredMaterialFromBasicEffect(this.mModel.Model.Meshes[0].MeshParts[0].Effect as SkinnedModelBasicEffect, out oMaterial);
    this.mRenderData = new ElementalEgg.RenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      this.mRenderData[index] = new ElementalEgg.RenderData();
      this.mRenderData[index].SetMesh(this.mModel.Model.Meshes[0].VertexBuffer, this.mModel.Model.Meshes[0].IndexBuffer, this.mModel.Model.Meshes[0].MeshParts[0], 0, 3, 4);
      this.mRenderData[index].mMaterial = oMaterial;
    }
  }

  public float ResistanceAgainst(Elements iElement) => 0.0f;

  public float Proximity
  {
    get => this.mProximity;
    set => this.mProximity = value;
  }

  private bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
  {
    return false;
  }

  private void PostCollision(ref CollisionInfo iInfo)
  {
    if (iInfo.SkinInfo.Skin0 == this.mCollision)
      iInfo.SkinInfo.IgnoreSkin0 = true;
    else
      iInfo.SkinInfo.IgnoreSkin1 = true;
  }

  public void Initialize(ref Vector3 iPosition, ref Vector3 iDirection, int iUniqueID)
  {
    this.Initialize(iUniqueID);
    this.mHitpoints = 1f;
    this.mMaxHitPoints = 1f;
    Segment iSeg = new Segment(iPosition, new Vector3(0.0f, -10f, 0.0f));
    ++iSeg.Origin.Y;
    Vector3 oPos;
    if (!this.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out Vector3 _, iSeg))
      oPos = iPosition;
    oPos.Y += this.mRadius;
    Vector3 up = Vector3.Up;
    Vector3 result;
    Vector3.Cross(ref iDirection, ref up, out result);
    this.mBody.MoveTo(oPos, new Matrix()
    {
      Forward = iDirection,
      Up = up,
      Right = result
    });
    this.mDamageMemory.Clear();
    this.mSpawnElement = Elements.None;
    this.mSummoner = (ISpellCaster) null;
    this.mProximityTimer = 0.0f;
    this.mController.StartClip(this.mClip, true);
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    base.Update(iDataChannel, iDeltaTime);
    Matrix result1 = this.mBody.Transform.Orientation;
    Matrix.Multiply(ref result1, this.mScale, out result1);
    Vector3 position1 = this.mBody.Transform.Position;
    result1.Translation = position1;
    this.mController.Update(iDeltaTime, ref result1, true);
    this.mController.SkinnedBoneTransforms.CopyTo((Array) this.mRenderData[(int) iDataChannel].mBones, 0);
    this.mProximityTimer += iDeltaTime;
    this.mSpawnElement = Elements.None;
    float minValue = float.MinValue;
    foreach (KeyValuePair<Elements, float> keyValuePair in this.mDamageMemory)
    {
      if ((double) keyValuePair.Value > (double) minValue)
      {
        this.mSpawnElement = keyValuePair.Key;
        minValue = keyValuePair.Value;
      }
    }
    this.mRenderData[(int) iDataChannel].mBoundingSphere.Center = result1.Translation;
    this.mRenderData[(int) iDataChannel].mBoundingSphere.Radius = this.mRadius;
    this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) this.mRenderData[(int) iDataChannel]);
    if ((double) this.mBody.Velocity.LengthSquared() > 9.9999999747524271E-07)
      this.mRestingTimer = 1f;
    else
      this.mRestingTimer -= iDeltaTime;
    if (!((double) this.mProximity > 0.0 & (double) this.mProximityTimer > 1.0))
      return;
    --this.mProximityTimer;
    List<Entity> entities = this.mPlayState.EntityManager.GetEntities(position1, this.mProximity, false, false);
    for (int index = 0; index < entities.Count; ++index)
    {
      if (entities[index] is Avatar)
      {
        Vector3 position2 = entities[index].Position;
        float result2;
        Vector3.DistanceSquared(ref position1, ref position2, out result2);
        if ((double) result2 <= (double) this.mProximity * (double) this.mProximity)
        {
          this.mSpawnElement = ElementalEgg.RANDOM_LOOKUP[ElementalEgg.sRandom.Next(ElementalEgg.RANDOM_LOOKUP.Length)];
          break;
        }
      }
    }
    this.mPlayState.EntityManager.ReturnEntityList(entities);
  }

  public void SetSummoned(ISpellCaster iCharacter) => this.mSummoner = iCharacter;

  public bool IsSummoned => this.mSummoner != null;

  public override void Deinitialize()
  {
    base.Deinitialize();
    if ((this.mSpawnElement & (Elements.StatusEffects | Elements.Lightning | Elements.Arcane)) == Elements.None)
      return;
    this.Spawn();
  }

  private void Spawn()
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    Vector3 position = this.Position;
    Matrix orientation = this.mBody.Orientation;
    CharacterTemplate iTemplate = ElementalEgg.sTemplateLookup[this.mSpawnElement];
    NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(this.mPlayState);
    instance.Initialize(iTemplate, position, 0, 0.0f);
    instance.CharacterBody.Orientation = orientation;
    instance.CharacterBody.DesiredDirection = orientation.Forward;
    instance.AI.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, -1, -1, 0, (AIEvent[]) null);
    instance.SpawnAnimation = Animations.special0;
    instance.ChangeState((BaseState) RessurectionState.Instance);
    if (this.mSummoner is Character)
      instance.Summoned(this.mSummoner as Character);
    this.mPlayState.EntityManager.AddEntity((Entity) instance);
    if (NetworkManager.Instance.State != NetworkState.Server)
      return;
    TriggerActionMessage iMessage = new TriggerActionMessage();
    iMessage.ActionType = TriggerActionType.SpawnNPC;
    iMessage.Handle = instance.Handle;
    if (this.mSummoner != null)
      iMessage.Scene = (int) this.mSummoner.Handle;
    iMessage.Template = instance.Type;
    iMessage.Position = instance.Position;
    iMessage.Direction = orientation.Forward;
    iMessage.Point1 = 170;
    iMessage.Point2 = 170;
    NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref iMessage);
  }

  public bool HasStatus(StatusEffects iStatus) => false;

  public StatusEffect[] GetStatusEffects() => (StatusEffect[]) null;

  public float StatusMagnitude(StatusEffects iStatus) => 0.0f;

  public void Damage(float iDamage, Elements iElement)
  {
  }

  public float Volume => this.mCollision.GetPrimitiveLocal(0).GetVolume();

  public float HitPoints => this.mHitpoints;

  public float MaxHitPoints => this.mMaxHitPoints;

  public bool SegmentIntersect(out Vector3 oPosition, Segment iSeg, float iSegmentRadius)
  {
    float t1;
    float num1 = Distance.SegmentSegmentDistanceSq(out float _, out t1, new Segment()
    {
      Origin = (this.mCollision.GetPrimitiveNewWorld(0) as Capsule).Position,
      Delta = {
        Y = (this.mCollision.GetPrimitiveNewWorld(0) as Capsule).Length
      }
    }, iSeg);
    iSeg.GetPoint(t1, out oPosition);
    float num2 = iSegmentRadius + this.mRadius;
    return (double) num1 <= (double) num2 * (double) num2;
  }

  public DamageResult InternalDamage(
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
    if ((iDamage.Element & (Elements.StatusEffects | Elements.Lightning | Elements.Arcane)) == Elements.None)
      return DamageResult.None;
    int num1 = Helper.CountSetBits((uint) iDamage.Element);
    float num2 = iDamage.Amount * iDamage.Magnitude;
    if (num1 > 1)
    {
      for (int iIndex = 0; iIndex < 11; ++iIndex)
      {
        Elements key = Spell.ElementFromIndex(iIndex);
        if ((key & iDamage.Element) == key && (key & (Elements.StatusEffects | Elements.Lightning | Elements.Arcane)) == key)
          this.mDamageMemory[key] = num2;
      }
    }
    else
      this.mDamageMemory[iDamage.Element] = num2;
    return DamageResult.Hit;
  }

  public void OverKill() => this.Kill();

  public void Electrocute(IDamageable iTarget, float iMultiplyer)
  {
  }

  public override bool Dead => this.mSpawnElement != Elements.None;

  public override bool Removable => this.mSpawnElement != Elements.None;

  public override void Kill()
  {
    if (this.mSpawnElement != Elements.None)
      return;
    this.mSpawnElement = Elements.Shield;
  }

  public bool Resting => (double) this.mRestingTimer < 0.0;

  protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
  {
    oMsg = new EntityUpdateMessage();
    if (this.Resting)
      return;
    oMsg.Features |= EntityFeatures.Position;
    oMsg.Position = this.Position;
  }

  internal override float GetDanger() => 0.0f;

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct State
  {
    public State(BinaryReader iReader) => throw new NotImplementedException();

    public State(ElementalEgg iEgg) => throw new NotImplementedException();

    public void ApplyTo(ElementalEgg iEgg) => throw new NotImplementedException();

    public void Write(BinaryWriter iWriter) => throw new NotImplementedException();
  }

  private class RenderData : 
    RenderableObject<SkinnedModelDeferredEffect, SkinnedModelDeferredBasicMaterial>
  {
    public Matrix[] mBones;

    public RenderData() => this.mBones = new Matrix[80 /*0x50*/];

    public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      (iEffect as SkinnedModelDeferredEffect).Bones = this.mBones;
      base.Draw(iEffect, iViewFrustum);
    }

    public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      (iEffect as SkinnedModelDeferredEffect).Bones = this.mBones;
      base.DrawShadow(iEffect, iViewFrustum);
    }
  }
}
