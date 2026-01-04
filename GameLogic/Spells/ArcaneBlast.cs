// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Spells.ArcaneBlast
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using Magicka.Graphics;
using Magicka.Graphics.Lights;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Spells;

internal class ArcaneBlast : IAbilityEffect
{
  private static List<ArcaneBlast> sCache;
  public static readonly int[] EFFECT_BEAM = new int[11]
  {
    0,
    "beam_water_explosion".GetHashCodeCustom(),
    "beam_cold_explosion".GetHashCodeCustom(),
    "beam_fire_explosion".GetHashCodeCustom(),
    "beam_lightning_explosion".GetHashCodeCustom(),
    "beam_arcane_explosion".GetHashCodeCustom(),
    "beam_life_explosion".GetHashCodeCustom(),
    0,
    0,
    "beam_steam_explosion".GetHashCodeCustom(),
    "beam_poison_explosion".GetHashCodeCustom()
  };
  private static int sVerticesHash;
  private static VertexBuffer sVertices;
  private static VertexDeclaration sVertexDeclaration;
  private static Texture2D sTexture;
  private ArcaneBlast.RenderData[] mRenderData;
  private float mRotation;
  private float mTTL;
  private float mMaxRadius;
  private Elements mElement;
  private Entity mOwner;
  private Vector3 mPosition;
  private Vector3 mColor;

  public static ArcaneBlast GetInstance()
  {
    if (ArcaneBlast.sCache.Count <= 0)
      return new ArcaneBlast();
    ArcaneBlast instance = ArcaneBlast.sCache[ArcaneBlast.sCache.Count - 1];
    ArcaneBlast.sCache.RemoveAt(ArcaneBlast.sCache.Count - 1);
    return instance;
  }

  public static void InitializeCache(int iNr)
  {
    ArcaneBlast.sCache = new List<ArcaneBlast>(iNr);
    for (int index = 0; index < iNr; ++index)
      ArcaneBlast.sCache.Add(new ArcaneBlast());
  }

  private ArcaneBlast()
  {
    if (ArcaneBlast.sVertices == null)
    {
      GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
      VertexPositionTexture[] data = new VertexPositionTexture[4];
      data[0].Position.X = -1f;
      data[0].Position.Z = -1f;
      data[0].TextureCoordinate.X = 0.0f;
      data[0].TextureCoordinate.Y = 0.0f;
      data[1].Position.X = 1f;
      data[1].Position.Z = -1f;
      data[1].TextureCoordinate.X = 1f;
      data[1].TextureCoordinate.Y = 0.0f;
      data[2].Position.X = 1f;
      data[2].Position.Z = 1f;
      data[2].TextureCoordinate.X = 1f;
      data[2].TextureCoordinate.Y = 1f;
      data[3].Position.X = -1f;
      data[3].Position.Z = 1f;
      data[3].TextureCoordinate.X = 0.0f;
      data[3].TextureCoordinate.Y = 1f;
      lock (graphicsDevice)
      {
        ArcaneBlast.sTexture = Magicka.Game.Instance.Content.Load<Texture2D>("EffectTextures/ArcaneDisc");
        ArcaneBlast.sVertices = new VertexBuffer(graphicsDevice, 4 * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
        ArcaneBlast.sVertices.SetData<VertexPositionTexture>(data);
        ArcaneBlast.sVertexDeclaration = new VertexDeclaration(graphicsDevice, VertexPositionTexture.VertexElements);
      }
      ArcaneBlast.sVerticesHash = ArcaneBlast.sVertices.GetHashCode();
    }
    this.mRenderData = new ArcaneBlast.RenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      ArcaneBlast.RenderData renderData = new ArcaneBlast.RenderData();
      this.mRenderData[index] = renderData;
    }
  }

  public void Initialize(
    Entity iOwner,
    Vector3 iPosition,
    Vector3 iColor,
    float iRadius,
    Elements iElements)
  {
    this.mElement = iElements;
    this.mMaxRadius = iRadius;
    this.mTTL = 1f;
    this.mPosition = iPosition;
    this.mColor = iColor;
    this.mOwner = iOwner;
    Vector3 forward = Vector3.Forward;
    for (int iIndex = 0; iIndex < 11; ++iIndex)
    {
      if ((this.mElement & Defines.ElementFromIndex(iIndex)) != Elements.None)
        EffectManager.Instance.StartEffect(ArcaneBlast.EFFECT_BEAM[iIndex], ref this.mPosition, ref forward, out VisualEffectReference _);
    }
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    DynamicLight cachedLight = DynamicLight.GetCachedLight();
    cachedLight.Initialize(iPosition, iColor, 2f, iRadius * 2f, 2f, 1f, this.mTTL * 0.25f);
    cachedLight.Enable();
  }

  public bool IsDead => (double) this.mTTL <= -1.0;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mTTL -= iDeltaTime * 5f;
    ArcaneBlast.RenderData iObject = this.mRenderData[(int) iDataChannel];
    this.mRotation = MathHelper.WrapAngle(this.mRotation + iDeltaTime * 4f);
    iObject.BoundingSphere.Center = this.mPosition;
    iObject.BoundingSphere.Radius = (1f - this.mTTL) * this.mMaxRadius;
    iObject.ColorEdge = this.mColor;
    Vector3.Multiply(ref this.mColor, 1f, out iObject.ColorCenter);
    iObject.Element = this.mElement;
    Matrix.CreateRotationY(this.mRotation, out iObject.Transform0);
    MagickaMath.UniformMatrixScale(ref iObject.Transform0, iObject.BoundingSphere.Radius);
    iObject.Transform0.Translation = this.mPosition;
    Matrix.CreateRotationY(-this.mRotation, out iObject.Transform1);
    MagickaMath.UniformMatrixScale(ref iObject.Transform1, iObject.BoundingSphere.Radius * 0.9f);
    iObject.Transform1.Translation = this.mPosition;
    iObject.Alpha = Math.Min(1f, (float) ((double) this.mTTL * 4.0 + 1.0));
    this.mOwner.PlayState.Scene.AddRenderableAdditiveObject(iDataChannel, (IRenderableAdditiveObject) iObject);
  }

  public void OnRemove() => ArcaneBlast.sCache.Add(this);

  protected class RenderData : IRenderableAdditiveObject
  {
    public BoundingSphere BoundingSphere;
    public Matrix Transform0;
    public Matrix Transform1;
    public Vector3 ColorCenter;
    public Vector3 ColorEdge;
    public float Alpha;
    public Elements Element;
    private static Vector2 sTextureSize = new Vector2(128f, 128f);

    public int Effect => AdditiveEffect.TYPEHASH;

    public int Technique => 0;

    public VertexBuffer Vertices => ArcaneBlast.sVertices;

    public int VerticesHashCode => ArcaneBlast.sVerticesHash;

    public int VertexStride => VertexPositionTexture.SizeInBytes;

    public IndexBuffer Indices => (IndexBuffer) null;

    public VertexDeclaration VertexDeclaration => ArcaneBlast.sVertexDeclaration;

    public bool Cull(BoundingFrustum iViewFrustum)
    {
      return this.BoundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
    }

    public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      AdditiveEffect additiveEffect = iEffect as AdditiveEffect;
      additiveEffect.Texture = ArcaneBlast.sTexture;
      additiveEffect.TextureEnabled = true;
      additiveEffect.TextureScale = new Vector2(1f, 0.125f);
      additiveEffect.ColorTint = new Vector4(1f, 1f, 1f, this.Alpha * 0.5f);
      additiveEffect.VertexColorEnabled = false;
      for (int iIndex = 0; iIndex < 11; ++iIndex)
      {
        Elements iElement = this.Element & Defines.ElementFromIndex(iIndex);
        if (iElement != Elements.None)
        {
          additiveEffect.TextureOffset = Railgun.ELEMENT_OFFSET_LOOKUP[Defines.ElementIndex(iElement)];
          additiveEffect.World = this.Transform0;
          iEffect.CommitChanges();
          iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
          additiveEffect.World = this.Transform1;
          iEffect.CommitChanges();
          iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
        }
      }
    }
  }
}
