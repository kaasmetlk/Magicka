// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Spells.IceBlade
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Items;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Spells;

public class IceBlade : IAbilityEffect
{
  public const int TRAIL_VERTEX_COUNT = 32 /*0x20*/;
  private static List<IceBlade> sCache;
  private static readonly int SPAWN_EFFECT = "weapon_ice_spawn".GetHashCodeCustom();
  private static readonly int DEATH_EFFECT = "weapon_ice_death".GetHashCodeCustom();
  private static Model sModel;
  private static BoundingSphere sBoundingSphere;
  private static Vector3 sAttach;
  private static VertexDeclaration sTrailVertexDeclaration;
  private static Texture2D sTrailTexture;
  private VertexPositionTexture[] mTrailVertices;
  private IceBlade.TrailRenderData[] mTrailRenderData;
  private RenderableObject<RenderDeferredEffect, RenderDeferredMaterial>[] mRenderData;
  private Item mOwner;
  private bool mDead;
  private List<ushort> mHitlist = new List<ushort>(32 /*0x20*/);
  private DamageCollection5 mDamage;
  private float mScale;
  private float mRange;
  private double mTimeStamp;

  public static IceBlade GetInstance()
  {
    if (IceBlade.sCache.Count <= 0)
      return new IceBlade();
    IceBlade instance = IceBlade.sCache[IceBlade.sCache.Count - 1];
    IceBlade.sCache.RemoveAt(IceBlade.sCache.Count - 1);
    return instance;
  }

  public static void InitializeCache(int iNr)
  {
    IceBlade.sCache = new List<IceBlade>(iNr);
    for (int index = 0; index < iNr; ++index)
      IceBlade.sCache.Add(new IceBlade());
  }

  private IceBlade()
  {
    if (IceBlade.sModel == null)
    {
      GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
      lock (graphicsDevice)
      {
        IceBlade.sModel = Magicka.Game.Instance.Content.Load<Model>("Models/Effects/Ice_Blade");
        IceBlade.sTrailTexture = Magicka.Game.Instance.Content.Load<Texture2D>("EffectTextures/IceBlade");
        IceBlade.sTrailVertexDeclaration = new VertexDeclaration(graphicsDevice, VertexPositionTexture.VertexElements);
      }
      IceBlade.sBoundingSphere = IceBlade.sModel.Meshes[0].BoundingSphere;
      for (int index = 0; index < IceBlade.sModel.Bones.Count; ++index)
      {
        if (IceBlade.sModel.Bones[index].Name.Equals("attach0", StringComparison.OrdinalIgnoreCase))
        {
          IceBlade.sAttach = IceBlade.sModel.Bones[index].Transform.Translation;
          break;
        }
      }
    }
    this.mTrailVertices = new VertexPositionTexture[32 /*0x20*/];
    for (int index = 0; index < this.mTrailVertices.Length; ++index)
    {
      this.mTrailVertices[index].TextureCoordinate.X = (float) (index / 2) / 15f;
      this.mTrailVertices[index].TextureCoordinate.Y = (float) (index % 2);
    }
    this.mRenderData = new RenderableObject<RenderDeferredEffect, RenderDeferredMaterial>[3];
    this.mTrailRenderData = new IceBlade.TrailRenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      RenderableObject<RenderDeferredEffect, RenderDeferredMaterial> renderableObject = new RenderableObject<RenderDeferredEffect, RenderDeferredMaterial>();
      this.mRenderData[index] = renderableObject;
      renderableObject.SetMesh(IceBlade.sModel.Meshes[0], IceBlade.sModel.Meshes[0].MeshParts[0], 4, 0, 5);
      this.mTrailRenderData[index] = new IceBlade.TrailRenderData(32 /*0x20*/);
    }
  }

  public void Initialize(Item iOwner, ref DamageCollection5 iDamage, float iRange)
  {
    this.mDead = false;
    this.mOwner = iOwner;
    this.mDamage = iDamage;
    this.mTimeStamp = iOwner.PlayState.PlayTime;
    Matrix orientation = this.mOwner.GetOrientation();
    this.mTrailVertices[0].Position = orientation.Translation;
    Vector3.Transform(ref IceBlade.sAttach, ref orientation, out this.mTrailVertices[1].Position);
    for (int index = 2; index < 32 /*0x20*/; index += 2)
    {
      this.mTrailVertices[index].Position = this.mTrailVertices[index - 2].Position;
      this.mTrailVertices[index + 1].Position = this.mTrailVertices[index - 1].Position;
    }
    this.mRange = iRange;
    this.mScale = iRange / IceBlade.sAttach.Y;
    EffectManager.Instance.StartEffect(IceBlade.SPAWN_EFFECT, ref orientation, out VisualEffectReference _);
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
  }

  public void Kill() => this.mDead = true;

  public bool IsDead => this.mDead;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    RenderableObject<RenderDeferredEffect, RenderDeferredMaterial> iObject1 = this.mRenderData[(int) iDataChannel];
    iObject1.mMaterial.WorldTransform = this.mOwner.GetOrientation();
    iObject1.mMaterial.WorldTransform.M11 *= this.mScale;
    iObject1.mMaterial.WorldTransform.M12 *= this.mScale;
    iObject1.mMaterial.WorldTransform.M13 *= this.mScale;
    iObject1.mMaterial.WorldTransform.M21 *= this.mScale;
    iObject1.mMaterial.WorldTransform.M22 *= this.mScale;
    iObject1.mMaterial.WorldTransform.M23 *= this.mScale;
    iObject1.mMaterial.WorldTransform.M31 *= this.mScale;
    iObject1.mMaterial.WorldTransform.M32 *= this.mScale;
    iObject1.mMaterial.WorldTransform.M33 *= this.mScale;
    iObject1.mBoundingSphere.Radius = IceBlade.sBoundingSphere.Radius;
    Vector3.Transform(ref IceBlade.sBoundingSphere.Center, ref iObject1.mMaterial.WorldTransform, out iObject1.mBoundingSphere.Center);
    Vector3 result;
    Vector3.Transform(ref IceBlade.sAttach, ref iObject1.mMaterial.WorldTransform, out result);
    Segment segment;
    segment.Origin = iObject1.mMaterial.WorldTransform.Translation;
    Vector3.Subtract(ref result, ref segment.Origin, out segment.Delta);
    this.mTrailVertices[0].Position = segment.Origin;
    this.mTrailVertices[1].Position = result;
    float amount = (float) Math.Pow(1.4012984643248171E-45, (double) iDeltaTime);
    for (int index = 2; index < 32 /*0x20*/; index += 2)
    {
      Vector3.Lerp(ref this.mTrailVertices[index - 2].Position, ref this.mTrailVertices[index].Position, amount, out this.mTrailVertices[index].Position);
      Vector3.Lerp(ref this.mTrailVertices[index - 1].Position, ref this.mTrailVertices[index + 1].Position, amount, out this.mTrailVertices[index + 1].Position);
    }
    this.mOwner.PlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) iObject1);
    IceBlade.TrailRenderData iObject2 = this.mTrailRenderData[(int) iDataChannel];
    iObject2.BoundingSphere = iObject1.mBoundingSphere;
    this.mTrailVertices.CopyTo((Array) iObject2.VertexArray, 0);
    this.mOwner.PlayState.Scene.AddRenderableAdditiveObject(iDataChannel, (IRenderableAdditiveObject) iObject2);
  }

  public void OnRemove()
  {
    Magicka.GameLogic.Entities.Character owner = this.mOwner.Owner;
    if (owner != null)
    {
      Segment iSeg;
      iSeg.Origin = owner.Position;
      iSeg.Delta = owner.Direction;
      Vector3.Multiply(ref iSeg.Delta, this.mRange, out iSeg.Delta);
      Vector3 point;
      iSeg.GetPoint(0.5f, out point);
      List<Entity> entities = owner.PlayState.EntityManager.GetEntities(point, this.mRange * 0.5f, true);
      for (int index = 0; index < entities.Count; ++index)
      {
        IDamageable t = entities[index] as IDamageable;
        Vector3 oPosition;
        if (t != owner & t != null && t.SegmentIntersect(out oPosition, iSeg, 1f))
        {
          int num = (int) t.Damage(this.mDamage, (Entity) owner, this.mTimeStamp, oPosition);
        }
      }
      owner.PlayState.EntityManager.ReturnEntityList(entities);
    }
    IceBlade.sCache.Add(this);
    Matrix orientation = this.mOwner.GetOrientation();
    EffectManager.Instance.StartEffect(IceBlade.DEATH_EFFECT, ref orientation, out VisualEffectReference _);
  }

  protected class TrailRenderData : IRenderableAdditiveObject
  {
    public BoundingSphere BoundingSphere;
    private VertexPositionTexture[] mVertices;

    public TrailRenderData(int iNrOfVertices)
    {
      this.mVertices = new VertexPositionTexture[iNrOfVertices];
    }

    public VertexPositionTexture[] VertexArray => this.mVertices;

    public int Effect => AdditiveEffect.TYPEHASH;

    public int Technique => 0;

    public VertexBuffer Vertices => (VertexBuffer) null;

    public int VerticesHashCode => 0;

    public int VertexStride => VertexPositionTexture.SizeInBytes;

    public IndexBuffer Indices => (IndexBuffer) null;

    public VertexDeclaration VertexDeclaration => IceBlade.sTrailVertexDeclaration;

    public bool Cull(BoundingFrustum iViewFrustum)
    {
      return this.BoundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
    }

    public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      AdditiveEffect additiveEffect = iEffect as AdditiveEffect;
      additiveEffect.ColorTint = new Vector4(1f);
      additiveEffect.TextureEnabled = true;
      additiveEffect.Texture = IceBlade.sTrailTexture;
      additiveEffect.TextureOffset = new Vector2();
      additiveEffect.TextureScale = new Vector2(1f);
      additiveEffect.VertexColorEnabled = false;
      additiveEffect.World = Matrix.Identity;
      additiveEffect.GraphicsDevice.RenderState.CullMode = CullMode.None;
      iEffect.CommitChanges();
      iEffect.GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleStrip, this.mVertices, 0, 30);
    }
  }
}
