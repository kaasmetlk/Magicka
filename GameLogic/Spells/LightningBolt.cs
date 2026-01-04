// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Spells.LightningBolt
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Magicka.Graphics.Lights;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Spells;

public class LightningBolt : IAbilityEffect
{
  private static readonly int HITEFFECTHASH = "lightning_hit".GetHashCodeCustom();
  private static readonly int SOURCEEFFECTHASH = "lightning_source".GetHashCodeCustom();
  private static List<LightningBolt> sCache;
  private static ContentManager sContent;
  private Matrix mOrientation;
  private VertexBuffer mVertices;
  private VertexDeclaration mVertexDeclaration;
  private int mDrawCount;
  private float mTTL;
  private Scene mScene;
  private CapsuleLight mLight;
  private double mTimeStamp;
  private LightningBolt.RenderData[] mRenderData;
  private bool mAirToSurface;

  public bool AirToSurface
  {
    get => this.mAirToSurface;
    set => this.mAirToSurface = value;
  }

  private LightningBolt(ContentManager iContent)
  {
    this.mLight = new CapsuleLight(iContent);
    VertexPositionColorTexture[] data = new VertexPositionColorTexture[34];
    VertexPositionColorTexture positionColorTexture = new VertexPositionColorTexture();
    positionColorTexture.Color = Color.White;
    float num1 = (float) MagickaMath.Random.Next(8) * 0.125f;
    float num2 = 0.0f;
    for (int index = 0; index < data.Length; ++index)
    {
      if (index > 0 & index % 2 == 0)
        num2 = MagickaMath.RandomBetween(-0.75f, 0.75f);
      positionColorTexture.Position.X = (float) (((double) (index % 2) - 0.5 + (double) num2) * -1.0);
      positionColorTexture.Position.Y = 0.0f;
      positionColorTexture.Position.Z = (float) -(index / 2) * 2f;
      positionColorTexture.TextureCoordinate.X = (float) (index / 2) * 0.125f + num1;
      positionColorTexture.TextureCoordinate.Y = (float) (0.375 + (double) (index % 2) / 8.0);
      data[index] = positionColorTexture;
    }
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    lock (graphicsDevice)
    {
      this.mVertices = new VertexBuffer(graphicsDevice, data.Length * VertexPositionColorTexture.SizeInBytes, BufferUsage.WriteOnly);
      this.mVertices.SetData<VertexPositionColorTexture>(data);
      this.mVertexDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, VertexPositionColorTexture.VertexElements);
    }
    AdditiveMaterial additiveMaterial = new AdditiveMaterial();
    lock (graphicsDevice)
      additiveMaterial.Texture = iContent.Load<Texture2D>("EffectTextures/Beams");
    additiveMaterial.TextureEnabled = true;
    additiveMaterial.VertexColorEnabled = false;
    this.mRenderData = new LightningBolt.RenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      LightningBolt.RenderData renderData = new LightningBolt.RenderData();
      this.mRenderData[index] = renderData;
      renderData.mVertices = this.mVertices;
      renderData.mVerticesHash = this.mVertices.GetHashCode();
      renderData.mVertexDeclaration = this.mVertexDeclaration;
      renderData.mMaterial = additiveMaterial;
    }
  }

  public void Dispose()
  {
    this.mVertices.Dispose();
    this.mVertexDeclaration.Dispose();
  }

  public static void InitializeCache(ContentManager iContent, int iSize)
  {
    LightningBolt.sContent = iContent;
    LightningBolt.sCache = new List<LightningBolt>(iSize);
    for (int index = 0; index < iSize; ++index)
      LightningBolt.sCache.Add(new LightningBolt(LightningBolt.sContent));
  }

  public static LightningBolt GetLightning()
  {
    if (LightningBolt.sCache.Count == 0)
      return new LightningBolt(LightningBolt.sContent);
    int index = MagickaMath.Random.Next(LightningBolt.sCache.Count);
    LightningBolt lightning = LightningBolt.sCache[index];
    LightningBolt.sCache.RemoveAt(index);
    return lightning;
  }

  public void Cast(
    ISpellCaster iCaster,
    Vector3 iSource,
    Entity iTarget,
    HitList iHitList,
    Vector3 iColor,
    float iScale,
    float iRange,
    ref DamageCollection5 iDamages,
    PlayState iState)
  {
    if (iCaster is Magicka.GameLogic.Entities.Character)
      (iCaster as Magicka.GameLogic.Entities.Character).GetSpellRangeModifier(ref iRange);
    iHitList.Add(iTarget);
    DamageResult damageResult1 = DamageResult.None;
    this.mTimeStamp = iCaster.PlayState.PlayTime;
    DamageResult damageResult2;
    if (iTarget is Magicka.GameLogic.Entities.Character && (iTarget as Magicka.GameLogic.Entities.Character).IsBlocking)
    {
      Spell oSpell;
      Spell.DefaultSpell(Elements.Lightning, out oSpell);
      (iTarget as Magicka.GameLogic.Entities.Character).Equipment[0].Item.TryAddToQueue(ref oSpell, true);
      damageResult2 = damageResult1 | DamageResult.Deflected;
    }
    else
      damageResult2 = ((IDamageable) iTarget).Damage(iDamages, iCaster as Entity, this.mTimeStamp, iSource);
    if (damageResult2 == DamageResult.Deflected)
    {
      Vector3 position1 = iTarget.Position;
      Vector3 position2 = iState.Scene.Camera.Position;
      Vector3 result;
      Vector3.Subtract(ref position1, ref iSource, out result);
      result.Normalize();
      this.InitializeEffect(ref iSource, ref result, ref position1, ref position2, ref iColor, false, iScale, 0.4f, iState);
    }
    else
    {
      Vector3 iCenter = iTarget.Position;
      if (iTarget is Shield shield)
        iCenter = shield.GetNearestPosition(iSource);
      Vector3 position3 = iState.Scene.Camera.Position;
      Vector3 result1;
      Vector3.Subtract(ref iCenter, ref iSource, out result1);
      result1.Normalize();
      this.InitializeEffect(ref iSource, ref result1, ref iCenter, ref position3, ref iColor, false, iScale, 0.4f, iState);
      if ((double) iRange <= 0.05000000074505806 || iTarget is Barrier | iTarget is Shield | iTarget is BossDamageZone)
        return;
      List<Entity> entities;
      if (this.AirToSurface)
      {
        float y = iSource.Y;
        iSource.Y = 0.0f;
        entities = iState.EntityManager.GetEntities(iCenter, iRange, true);
        iSource.Y = y;
      }
      else
        entities = iState.EntityManager.GetEntities(iCenter, iRange, true);
      IDamageable iTarget1 = (IDamageable) null;
      float d = float.MaxValue;
      new Segment().Origin = iCenter;
      for (int index = 0; index < entities.Count; ++index)
      {
        if (entities[index] is IDamageable iDamageable && !iDamageable.Dead && !iHitList.Contains(iDamageable) && ((iDamageable is ElementalEgg | iDamageable is DamageablePhysicsEntity ? 1 : 0) | (!(iDamageable is Magicka.GameLogic.Entities.Character) ? 0 : (!(iDamageable as Magicka.GameLogic.Entities.Character).IsEthereal ? 1 : 0)) | (!(iDamageable is Barrier) ? 0 : ((iDamageable as Barrier).Solid ? 1 : 0)) | (iDamageable is MissileEntity ? 1 : 0) | (iDamageable is BossDamageZone ? 1 : 0)) != 0 && (!(iDamageable is IStatusEffected statusEffected) || !statusEffected.HasStatus(StatusEffects.Frozen)))
        {
          float num1 = Vector3.DistanceSquared(iCenter, iDamageable.Position);
          Vector3 position4 = iDamageable.Position;
          Vector3 vector = result1;
          Vector3 result2;
          Vector3.Subtract(ref position4, ref iCenter, out result2);
          result2.Y = 0.0f;
          result2.Normalize();
          vector.Y = 0.0f;
          if (vector != Vector3.Zero)
            vector.Normalize();
          float num2 = 0.7853982f;
          if ((double) num1 < (double) d && (double) MagickaMath.Angle(ref vector, ref result2) <= (double) num2)
          {
            d = num1;
            iTarget1 = iDamageable;
          }
        }
      }
      iCaster.PlayState.EntityManager.ReturnEntityList(entities);
      if (iTarget1 != null)
      {
        float num = (float) Math.Sqrt((double) d);
        LightningBolt.GetLightning().Cast(iCaster, iTarget.Position, iTarget1 as Entity, iHitList, iColor, iScale, iRange - num, ref iDamages, iState);
      }
      else
        LightningBolt.GetLightning().InitializeEffect(ref iCenter, result1, iColor, true, iScale, iRange, iState);
    }
  }

  public void Cast(
    ISpellCaster iCaster,
    Vector3 iSource,
    Vector3 iDirection,
    HitList iHitList,
    Vector3 iColor,
    float iRange,
    ref DamageCollection5 iDamages,
    Spell? iSpell,
    PlayState iState)
  {
    this.Cast(iCaster, iSource, iDirection, iHitList, iColor, 1f, iRange, ref iDamages, iSpell, iState);
  }

  public void Cast(
    ISpellCaster iCaster,
    Vector3 iSource,
    Vector3 iDirection,
    HitList iHitList,
    Vector3 iColor,
    float iSize,
    float iRange,
    ref DamageCollection5 iDamages,
    Spell? iSpell,
    PlayState iState)
  {
    if ((double) iRange <= 0.05000000074505806)
      return;
    this.mTimeStamp = iCaster.PlayState.PlayTime;
    List<Entity> entities;
    if (this.AirToSurface)
    {
      float y = iSource.Y;
      iSource.Y = 0.0f;
      entities = iState.EntityManager.GetEntities(iSource, iRange, true);
      iSource.Y = y;
    }
    else
      entities = iState.EntityManager.GetEntities(iSource, iRange, true);
    IDamageable damageable = (IDamageable) null;
    float num1 = float.MaxValue;
    float num2 = float.MaxValue;
    Segment seg = new Segment();
    seg.Origin = iSource;
    List<Shield> shields = iCaster.PlayState.EntityManager.Shields;
    for (int index1 = 0; index1 < entities.Count; ++index1)
    {
      if (entities[index1] is IDamageable iDamageable && !iDamageable.Dead && !iHitList.Contains(iDamageable) && ((iDamageable is ElementalEgg | iDamageable is DamageablePhysicsEntity ? 1 : 0) | (!(iDamageable is Magicka.GameLogic.Entities.Character) ? 0 : (!(iDamageable as Magicka.GameLogic.Entities.Character).IsEthereal ? 1 : 0)) | (!(iDamageable is Barrier) ? 0 : ((iDamageable as Barrier).Solid ? 1 : 0)) | (iDamageable is MissileEntity ? 1 : 0) | (iDamageable is Shield ? 1 : 0) | (iDamageable is BossDamageZone ? 1 : 0) | (iDamageable is Tentacle ? 1 : 0)) != 0)
      {
        if (!(iDamageable is Shield))
        {
          bool flag = false;
          for (int index2 = 0; index2 < shields.Count; ++index2)
          {
            seg.Delta = iDamageable.Position - seg.Origin;
            Shield shield = shields[index2];
            if (shield != null && shield.Body.CollisionSkin.SegmentIntersect(out float _, out Vector3 _, out Vector3 _, seg))
            {
              flag = true;
              break;
            }
          }
          if (flag)
            continue;
        }
        if (!(iDamageable is IStatusEffected statusEffected) || !statusEffected.HasStatus(StatusEffects.Frozen))
        {
          Vector3 vector3 = iDamageable.Position;
          if (iDamageable is Shield shield)
            vector3 = shield.GetNearestPosition(iSource);
          float num3 = Vector3.DistanceSquared(iSource, vector3);
          Vector3 vector = iDirection;
          Vector3 result;
          Vector3.Subtract(ref vector3, ref iSource, out result);
          result.Y = 0.0f;
          result.Normalize();
          vector.Y = 0.0f;
          if (vector != Vector3.Zero)
            vector.Normalize();
          float num4 = MagickaMath.Angle(ref vector, ref result);
          if ((double) num4 <= 1.5707963705062866 && (double) num4 < (double) num2 && (double) num3 < (double) num1 || (double) num3 < 1.4012984643248171E-45)
          {
            num2 = num4;
            num1 = num3;
            damageable = iDamageable;
          }
        }
      }
    }
    iCaster.PlayState.EntityManager.ReturnEntityList(entities);
    if (damageable == null)
    {
      Quaternion fromYawPitchRoll = Quaternion.CreateFromYawPitchRoll((float) ((MagickaMath.Random.NextDouble() - 0.5) * 0.5 * 0.78539818525314331), 0.0f, 0.0f);
      Vector3.Transform(ref iDirection, ref fromYawPitchRoll, out iDirection);
      this.InitializeEffect(ref iSource, iDirection, iColor, true, iSize, iRange, iState);
      iHitList.Clear();
    }
    else
    {
      DamageResult damageResult1 = DamageResult.None;
      iHitList.Add(damageable);
      Vector3 iTarget = damageable.Position;
      DamageResult damageResult2;
      if (damageable is Magicka.GameLogic.Entities.Character && (damageable as Magicka.GameLogic.Entities.Character).IsBlocking && (damageable as Magicka.GameLogic.Entities.Character).BlockItem >= 0)
      {
        Spell oSpell;
        if (iSpell.HasValue)
          oSpell = iSpell.Value;
        else
          Spell.DefaultSpell(Elements.Lightning, out oSpell);
        (damageable as Magicka.GameLogic.Entities.Character).Equipment[0].Item.TryAddToQueue(ref oSpell, true);
        iTarget = (damageable as Magicka.GameLogic.Entities.Character).Equipment[0].Item.Position;
        damageResult2 = damageResult1 | DamageResult.Deflected;
      }
      else
        damageResult2 = damageable.Damage(iDamages, iCaster as Entity, this.mTimeStamp, iSource);
      if (damageable is Shield shield)
        iTarget = shield.GetNearestPosition(iSource);
      Vector3 position = iState.Scene.Camera.Position;
      Vector3 result;
      Vector3.Subtract(ref iTarget, ref iSource, out result);
      result.Normalize();
      this.InitializeEffect(ref iSource, ref result, ref iTarget, ref position, ref iColor, false, iSize, 0.25f, iState);
      if (damageResult2 == DamageResult.Deflected || damageable == null || damageable is Barrier | damageable is Shield | damageable is BossDamageZone)
        return;
      LightningBolt lightning = LightningBolt.GetLightning();
      DamageCollection5 iDamages1 = iDamages;
      lightning.Cast(iCaster, iTarget, result, iHitList, iColor, iSize, iRange, ref iDamages1, iSpell, iState);
    }
  }

  public void InitializeEffect(
    ref Vector3 iCastFrom,
    Vector3 iDirection,
    Vector3 iColor,
    bool iCut,
    float iScale,
    float iRange,
    PlayState iState)
  {
    Vector3 iPosition = iCastFrom;
    Vector3 iTarget = iCastFrom + iDirection * iRange;
    Vector3 position = iState.Scene.Camera.Position;
    this.InitializeEffect(ref iPosition, ref iDirection, ref iTarget, ref position, ref iColor, iCut, iScale, 0.25f, iState);
  }

  public void InitializeEffect(
    ref Vector3 iPosition,
    ref Vector3 iDirection,
    ref Vector3 iTarget,
    ref Vector3 iEyePosition,
    ref Vector3 iColor,
    bool iCut,
    float iScale,
    float iTTL,
    PlayState iState)
  {
    if (iCut)
    {
      Segment iSeg;
      iSeg.Origin = iPosition;
      Vector3.Subtract(ref iTarget, ref iPosition, out iSeg.Delta);
      Vector3 oPos;
      if (iState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out Vector3 _, iSeg))
        iTarget = oPos;
    }
    EffectManager instance = EffectManager.Instance;
    VisualEffectReference oRef;
    instance.StartEffect(LightningBolt.SOURCEEFFECTHASH, ref iPosition, ref iDirection, out oRef);
    instance.StartEffect(LightningBolt.HITEFFECTHASH, ref iTarget, ref iDirection, out oRef);
    this.mTTL = iTTL;
    Vector3 result1;
    Vector3.Subtract(ref iEyePosition, ref iPosition, out result1);
    float result2;
    Vector3.Distance(ref iPosition, ref iTarget, out result2);
    result2 /= iScale;
    this.mDrawCount = Math.Min((int) Math.Floor((double) result2 / 2.0 + 0.5) * 2, 32 /*0x20*/);
    for (int index = 0; index < 3; ++index)
    {
      this.mRenderData[index].mDrawCount = this.mDrawCount;
      this.mRenderData[index].mMaterial.ColorTint = new Vector4(iColor * 2f, 1f);
    }
    Matrix scale = Matrix.CreateScale(iScale, iScale, iScale * (result2 / (float) this.mDrawCount));
    Matrix identity = Matrix.Identity;
    Vector3 result3;
    Vector3.Subtract(ref iTarget, ref iPosition, out result3);
    Vector3.Normalize(ref result3, out result3);
    Vector3 result4;
    Vector3.Cross(ref result3, ref result1, out result4);
    Vector3.Normalize(ref result4, out result4);
    Vector3 result5;
    Vector3.Cross(ref result4, ref result3, out result5);
    Vector3.Normalize(ref result5, out result5);
    identity.Forward = result3;
    identity.Right = result4;
    identity.Up = result5;
    Matrix.Multiply(ref scale, ref identity, out this.mOrientation);
    this.mOrientation.Translation = iPosition;
    this.mScene = iState.Scene;
    this.mLight.DiffuseColor = new Vector3()
    {
      X = iColor.X,
      Y = iColor.Y,
      Z = iColor.Z
    };
    this.mLight.End = iTarget;
    this.mLight.Start = iPosition;
    this.mLight.Radius = (float) (3.0 + 2.0 * (double) iScale);
    this.mLight.Enable(this.mScene);
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
  }

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mTTL -= iDeltaTime;
    if ((double) this.mTTL <= 0.0 || this.mDrawCount <= 0)
      return;
    LightningBolt.RenderData iObject = this.mRenderData[(int) iDataChannel];
    iObject.mMaterial.ColorTint.W = (float) Math.Sqrt((double) this.TTL * 10.0);
    this.mLight.Intensity = (float) Math.Sqrt((double) this.TTL * 1.0) * 1.5f;
    iObject.mTransform = this.mOrientation;
    this.mScene.AddRenderableAdditiveObject(iDataChannel, (IRenderableAdditiveObject) iObject);
  }

  public void OnRemove()
  {
    this.mLight.Disable();
    this.mScene = (Scene) null;
    LightningBolt.sCache.Add(this);
  }

  public bool IsDead => (double) this.mTTL <= 0.0;

  public float TTL
  {
    get => this.mTTL;
    set => this.mTTL = value;
  }

  protected class RenderData : IRenderableAdditiveObject
  {
    public Matrix mTransform;
    public int mDrawCount;
    public VertexBuffer mVertices;
    public int mVerticesHash;
    public VertexDeclaration mVertexDeclaration;
    public AdditiveMaterial mMaterial;
    public BoundingSphere mBoundingSphere;

    public int Effect => AdditiveEffect.TYPEHASH;

    public int Technique => 0;

    public VertexBuffer Vertices => this.mVertices;

    public IndexBuffer Indices => (IndexBuffer) null;

    public VertexDeclaration VertexDeclaration => this.mVertexDeclaration;

    public int VertexStride => VertexPositionColorTexture.SizeInBytes;

    public int VerticesHashCode => this.mVerticesHash;

    public bool Cull(BoundingFrustum iViewFrustum) => false;

    public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      int mDrawCount = this.mDrawCount;
      if (mDrawCount <= 0)
        return;
      AdditiveEffect iEffect1 = iEffect as AdditiveEffect;
      this.mMaterial.AssignToEffect(iEffect1);
      iEffect1.World = this.mTransform;
      iEffect1.TextureOffset = new Vector2();
      iEffect1.TextureScale = new Vector2(1f);
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleStrip, 0, mDrawCount);
    }
  }
}
