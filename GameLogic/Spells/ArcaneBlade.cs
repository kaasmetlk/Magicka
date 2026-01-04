// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Spells.ArcaneBlade
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Magicka.Graphics.Effects;
using Magicka.Graphics.Lights;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Spells;

public class ArcaneBlade : IAbilityEffect
{
  public const int VERTEXCOUNT = 32 /*0x20*/;
  private static List<ArcaneBlade> sCache;
  private static readonly int[] ELEMENT_EFFECTS = new int[11]
  {
    0,
    "weapon_arcane_water".GetHashCodeCustom(),
    "weapon_arcane_cold".GetHashCodeCustom(),
    "weapon_arcane_fire".GetHashCodeCustom(),
    "weapon_arcane_lightning".GetHashCodeCustom(),
    0,
    0,
    0,
    0,
    "weapon_arcane_steam".GetHashCodeCustom(),
    "weapon_arcane_poison".GetHashCodeCustom()
  };
  private bool mDead;
  private Item mOwner;
  private static VertexDeclaration sVertexDeclaration;
  private static Texture2D sTexture;
  private VertexPositionTexture[] mVertices;
  private ArcaneBlade.RenderData[] mRenderData;
  private PlayState mPlayState;
  private float mAlpha;
  private Vector3 mColor;
  private float mRange;
  private float mMaxRange;
  private Matrix mOrientation;
  private CapsuleLight mLight;
  private VisualEffectReference[] mEffects = new VisualEffectReference[4];

  public static ArcaneBlade GetInstance()
  {
    if (ArcaneBlade.sCache.Count <= 0)
      return new ArcaneBlade();
    ArcaneBlade instance = ArcaneBlade.sCache[ArcaneBlade.sCache.Count - 1];
    ArcaneBlade.sCache.RemoveAt(ArcaneBlade.sCache.Count - 1);
    return instance;
  }

  public static void InitializeCache(int iNr)
  {
    ArcaneBlade.sCache = new List<ArcaneBlade>(iNr);
    for (int index = 0; index < iNr; ++index)
      ArcaneBlade.sCache.Add(new ArcaneBlade());
  }

  private ArcaneBlade()
  {
    if (ArcaneBlade.sVertexDeclaration == null)
    {
      GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
      lock (graphicsDevice)
      {
        ArcaneBlade.sTexture = Magicka.Game.Instance.Content.Load<Texture2D>("EffectTextures/ArcaneBlade");
        ArcaneBlade.sVertexDeclaration = new VertexDeclaration(graphicsDevice, VertexPositionTexture.VertexElements);
      }
    }
    this.mVertices = new VertexPositionTexture[32 /*0x20*/];
    for (int index = 0; index < this.mVertices.Length; ++index)
    {
      this.mVertices[index].TextureCoordinate.X = (float) (index / 2) / 15f;
      this.mVertices[index].TextureCoordinate.Y = (float) (index % 2);
    }
    this.mRenderData = new ArcaneBlade.RenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      ArcaneBlade.RenderData renderData = new ArcaneBlade.RenderData(32 /*0x20*/);
      this.mRenderData[index] = renderData;
    }
    this.mLight = new CapsuleLight(Magicka.Game.Instance.Content);
  }

  public void Initialize(PlayState iPlayState, Item iItem, Elements iElements, float iRange)
  {
    this.mOwner = iItem;
    this.mPlayState = iPlayState;
    this.mDead = false;
    this.mAlpha = 1f;
    this.mRange = iRange;
    this.mMaxRange = iRange;
    Spell oSpell;
    Spell.DefaultSpell(iElements, out oSpell);
    this.mColor = oSpell.GetColor();
    if (this.mOwner != null)
    {
      this.mVertices[0].Position = this.mOwner.Position;
      Vector3 result = this.mOwner.Body.Orientation.Up;
      Vector3.Multiply(ref result, this.mRange, out result);
      Vector3.Add(ref this.mVertices[0].Position, ref result, out this.mVertices[1].Position);
      for (int index = 2; index < this.mVertices.Length; index += 2)
      {
        this.mVertices[index].Position = this.mVertices[index - 2].Position;
        this.mVertices[index + 1].Position = this.mVertices[index - 1].Position;
      }
    }
    this.mLight.DiffuseColor = this.mColor;
    this.mLight.Start = this.mVertices[0].Position;
    this.mLight.End = this.mVertices[1].Position;
    this.mLight.Radius = 4f;
    this.mLight.Intensity = 0.0f;
    this.mLight.Enable(this.mPlayState.Scene);
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    if (this.mOwner != null)
    {
      Matrix orientation = this.mOwner.Body.Orientation with
      {
        Translation = this.mOwner.Body.Position
      };
      orientation.M21 *= this.mRange;
      orientation.M22 *= this.mRange;
      orientation.M23 *= this.mRange;
      Elements elements1 = iElements & ~Elements.Beams;
      int num = 0;
      for (int index = 0; index < 11; ++index)
      {
        Elements elements2 = (Elements) (1 << index);
        if ((elements2 & elements1) == elements2)
          EffectManager.Instance.StartEffect(ArcaneBlade.ELEMENT_EFFECTS[index], ref orientation, out this.mEffects[num++]);
      }
    }
    else
    {
      Matrix identity = Matrix.Identity;
      Elements elements3 = iElements & ~Elements.Beams;
      int num = 0;
      for (int index = 0; index < 11; ++index)
      {
        Elements elements4 = (Elements) (1 << index);
        if ((elements4 & elements3) == elements4)
          EffectManager.Instance.StartEffect(ArcaneBlade.ELEMENT_EFFECTS[index], ref identity, out this.mEffects[num++]);
      }
    }
  }

  public void Kill() => this.mDead = true;

  public bool IsDead
  {
    get
    {
      float result;
      Vector3.DistanceSquared(ref this.mVertices[30].Position, ref this.mVertices[31 /*0x1F*/].Position, out result);
      return this.mDead & (double) result <= 0.10000000149011612;
    }
  }

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if (this.mDead)
    {
      this.mAlpha -= iDeltaTime * 5f;
      this.mRange = this.mMaxRange * (float) Math.Sqrt((double) Math.Max(this.mAlpha, 0.0f));
      this.mLight.Intensity = this.mAlpha;
    }
    else
      this.mLight.Intensity = Math.Min(this.mLight.Intensity + iDeltaTime * 10f, 1f);
    Vector3 result;
    if (this.mOwner == null)
    {
      this.mVertices[0].Position = this.mOrientation.Translation;
      result = this.mOrientation.Up;
    }
    else
    {
      this.mVertices[0].Position = this.mOwner.Position;
      result = this.mOwner.Body.Orientation.Up;
      Matrix orientation = this.mOwner.Body.Orientation with
      {
        Translation = this.mOwner.Body.Position
      };
      orientation.M21 *= this.mRange;
      orientation.M22 *= this.mRange;
      orientation.M23 *= this.mRange;
      for (int index = 0; index < this.mEffects.Length; ++index)
        EffectManager.Instance.UpdateOrientation(ref this.mEffects[index], ref orientation);
    }
    Vector3.Multiply(ref result, this.mRange, out result);
    Vector3.Add(ref this.mVertices[0].Position, ref result, out this.mVertices[1].Position);
    float amount = (float) Math.Pow(1.4012984643248171E-45, (double) iDeltaTime);
    for (int index = 2; index < this.mVertices.Length; index += 2)
    {
      Vector3.Lerp(ref this.mVertices[index - 2].Position, ref this.mVertices[index].Position, amount, out this.mVertices[index].Position);
      Vector3.Lerp(ref this.mVertices[index - 1].Position, ref this.mVertices[index + 1].Position, amount, out this.mVertices[index + 1].Position);
    }
    ArcaneBlade.RenderData iObject = this.mRenderData[(int) iDataChannel];
    Vector3 one = Vector3.One;
    Vector3.Add(ref this.mColor, ref one, out iObject.ColorCenter);
    iObject.ColorEdge = this.mColor;
    iObject.BoundingSphere.Center = this.mVertices[0].Position;
    iObject.BoundingSphere.Radius = 5f;
    iObject.Alpha = 1f;
    this.mVertices.CopyTo((Array) iObject.VertexArray, 0);
    this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, (IRenderableAdditiveObject) iObject);
    this.mLight.Start = this.mVertices[0].Position;
    this.mLight.End = this.mVertices[1].Position;
  }

  public void OnRemove()
  {
    ArcaneBlade.sCache.Add(this);
    this.mLight.Disable();
    for (int index = 0; index < this.mEffects.Length; ++index)
      EffectManager.Instance.Stop(ref this.mEffects[index]);
  }

  public Matrix Orientation
  {
    get => this.mOrientation;
    set => this.mOrientation = value;
  }

  public float Range => this.mRange;

  protected class RenderData : IRenderableAdditiveObject
  {
    public BoundingSphere BoundingSphere;
    public Vector3 ColorCenter;
    public Vector3 ColorEdge;
    public float Alpha;
    private VertexPositionTexture[] mVertices;

    public RenderData(int iNrOfVertices)
    {
      this.mVertices = new VertexPositionTexture[iNrOfVertices];
    }

    public VertexPositionTexture[] VertexArray => this.mVertices;

    public int Effect => ArcaneEffect.TYPEHASH;

    public int Technique => 0;

    public VertexBuffer Vertices => (VertexBuffer) null;

    public int VerticesHashCode => 0;

    public int VertexStride => VertexPositionTexture.SizeInBytes;

    public IndexBuffer Indices => (IndexBuffer) null;

    public VertexDeclaration VertexDeclaration => ArcaneBlade.sVertexDeclaration;

    public bool Cull(BoundingFrustum iViewFrustum)
    {
      return this.BoundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
    }

    public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      ArcaneEffect arcaneEffect = iEffect as ArcaneEffect;
      arcaneEffect.ColorCenter = this.ColorCenter;
      arcaneEffect.ColorEdge = this.ColorEdge;
      arcaneEffect.World = Matrix.Identity;
      arcaneEffect.Texture = ArcaneBlade.sTexture;
      arcaneEffect.Alpha = this.Alpha;
      arcaneEffect.GraphicsDevice.RenderState.CullMode = CullMode.None;
      iEffect.CommitChanges();
      iEffect.GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, this.mVertices, 0, this.mVertices.Length - 2);
    }
  }
}
