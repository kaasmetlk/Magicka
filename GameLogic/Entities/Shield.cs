// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Shield
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using JigLibX.Physics;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.UI;
using Magicka.Graphics.Effects;
using Magicka.Network;
using Magicka.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities;

public class Shield : Entity, IDamageable
{
  private const int HEIGHTDIVISIONS = 32 /*0x20*/;
  private const int AXISDIVISIONS = 64 /*0x40*/;
  private const float WALL_LENGTH = 6f;
  private const float WALL_HEIGHT = 3f;
  private const float WALL_WIDTH = 0.2f;
  private static readonly int SOUNDHASH = "spell_shield".GetHashCodeCustom();
  private static readonly string HEALTH_VAR_NAME = "Health";
  private static List<Shield> mCache;
  protected float mHitPoints;
  protected float mMaxHitPoints;
  protected ISpellCaster mOwner;
  protected float mDamageTimer;
  protected ShieldType mShieldType;
  private Vector2 mNoiseOffset0;
  private Vector2 mNoiseOffset1;
  private Vector2 mNoiseOffset2;
  private Vector2 mTextureScale;
  private Vector4 mTint = Vector4.One;
  private Vector4 mTargetTint = Vector4.One;
  private static ShieldEffect mEffect;
  private static VertexDeclaration sVertexDeclaration;
  private static VertexBuffer sSphereVertices;
  private static IndexBuffer sSphereIndices;
  private static int sSphereNumVertices;
  private static int sSphereNumPrimitives;
  private static VertexBuffer sWallVertices;
  private static IndexBuffer sWallIndices;
  private static int sWallNumVertices;
  private static int sWallNumPrimitives;
  private Shield.RenderData[] mRenderData;
  private Vector4[] mDamagePoints;
  private Cue mCue;
  protected float mRestingTimer = 1f;
  private float mIntersectKillTimer;
  private Vector3 mLastPosition;

  public static void InitializeCache(int iNrOfShields, PlayState iPlayState)
  {
    Shield.mCache = new List<Shield>(iNrOfShields);
    for (int index = 0; index < iNrOfShields; ++index)
      Shield.mCache.Add(new Shield(iPlayState));
  }

  public static Shield GetFromCache(PlayState iPlayState)
  {
    if (Shield.mCache.Count <= 0)
      return new Shield(iPlayState);
    Shield fromCache = Shield.mCache[Shield.mCache.Count - 1];
    Shield.mCache.RemoveAt(Shield.mCache.Count - 1);
    return fromCache;
  }

  protected Shield(PlayState iPlayState)
    : base(iPlayState)
  {
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    if (Shield.mEffect == null || Shield.mEffect.IsDisposed)
    {
      lock (graphicsDevice)
      {
        Shield.mEffect = new ShieldEffect(graphicsDevice, iPlayState.Content);
        Shield.mEffect.Texture = iPlayState.Content.Load<Texture2D>("EffectTextures/shield");
      }
    }
    if (Shield.sVertexDeclaration == null || Shield.sVertexDeclaration.IsDisposed)
    {
      lock (graphicsDevice)
        Shield.sVertexDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, VertexPositionNormalTexture.VertexElements);
    }
    if (Shield.sSphereVertices == null || Shield.sSphereVertices.IsDisposed)
    {
      VertexPositionNormalTexture[] data1 = new VertexPositionNormalTexture[System.Math.Max(31 /*0x1F*/, 0) * 65];
      VertexPositionNormalTexture positionNormalTexture = new VertexPositionNormalTexture();
      for (int index1 = 0; index1 <= 64 /*0x40*/; ++index1)
      {
        float num1 = (float) (6.2831854820251465 * (double) index1 / 64.0);
        float num2 = (float) System.Math.Cos((double) num1);
        float num3 = (float) System.Math.Sin((double) num1);
        int num4 = index1 * 31 /*0x1F*/;
        positionNormalTexture.TextureCoordinate.X = (float) (1.0 * (double) index1 / 64.0);
        for (int index2 = 0; index2 < 31 /*0x1F*/; ++index2)
        {
          float num5 = (float) (3.1415927410125732 * ((double) index2 + 1.8999999761581421) / 32.0 - 1.5707963705062866);
          positionNormalTexture.Position.Y = (float) System.Math.Sin((double) num5);
          float num6 = (float) System.Math.Cos((double) num5);
          positionNormalTexture.Position.X = num2 * num6;
          positionNormalTexture.Position.Z = num3 * num6;
          positionNormalTexture.TextureCoordinate.Y = (float) (1.0 * (double) index2 / 32.0);
          positionNormalTexture.Normal = positionNormalTexture.Position;
          data1[num4 + index2] = positionNormalTexture;
        }
      }
      ushort[] data2 = new ushort[11520];
      int num7 = 31 /*0x1F*/;
      int num8 = 0;
      for (int index3 = 0; index3 < 64 /*0x40*/; ++index3)
      {
        int num9 = index3 * num7;
        for (int index4 = 0; index4 < num7 - 1; ++index4)
        {
          ushort[] numArray1 = data2;
          int index5 = num8;
          int num10 = index5 + 1;
          int num11 = (int) (ushort) (num9 + num7);
          numArray1[index5] = (ushort) num11;
          ushort[] numArray2 = data2;
          int index6 = num10;
          int num12 = index6 + 1;
          int num13 = (int) (ushort) (num9 + 1);
          numArray2[index6] = (ushort) num13;
          ushort[] numArray3 = data2;
          int index7 = num12;
          int num14 = index7 + 1;
          int num15 = (int) (ushort) num9;
          numArray3[index7] = (ushort) num15;
          ushort[] numArray4 = data2;
          int index8 = num14;
          int num16 = index8 + 1;
          int num17 = (int) (ushort) (num9 + num7);
          numArray4[index8] = (ushort) num17;
          ushort[] numArray5 = data2;
          int index9 = num16;
          int num18 = index9 + 1;
          int num19 = (int) (ushort) (num9 + num7 + 1);
          numArray5[index9] = (ushort) num19;
          ushort[] numArray6 = data2;
          int index10 = num18;
          num8 = index10 + 1;
          int num20 = (int) (ushort) (num9 + 1);
          numArray6[index10] = (ushort) num20;
          ++num9;
        }
      }
      lock (graphicsDevice)
      {
        Shield.sSphereVertices = new VertexBuffer(graphicsDevice, data1.Length * VertexPositionNormalTexture.SizeInBytes, BufferUsage.WriteOnly);
        Shield.sSphereVertices.SetData<VertexPositionNormalTexture>(data1);
        Shield.sSphereIndices = new IndexBuffer(graphicsDevice, data2.Length * 2, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
        Shield.sSphereIndices.SetData<ushort>(data2);
      }
      Shield.sSphereNumVertices = data1.Length;
      Shield.sSphereNumPrimitives = data2.Length / 3;
    }
    if (Shield.sWallVertices == null || Shield.sWallVertices.IsDisposed)
    {
      VertexPositionNormalTexture[] data3 = new VertexPositionNormalTexture[242];
      ushort[] data4 = new ushort[1200];
      data3[120].Position.X = 0.2f;
      data3[120].Normal.X = 1f;
      data3[241].Position.X = -0.2f;
      data3[241].Normal.X = -1f;
      float num21 = -1f;
      float num22 = 0.1f;
      for (int index = 0; index < 20; ++index)
      {
        float num23 = -(float) System.Math.Cos((double) ((float) index / 20f) * 3.1415927410125732);
        data3[index].Position.Z = num23 * 6f;
        data3[index].Position.Y = (float) System.Math.Pow(1.0 - (double) num23 * (double) num23, 0.15) * 3f;
        data3[index].Normal.X = 1f;
        data3[index].TextureCoordinate.X = num23;
        data3[index].TextureCoordinate.Y = (float) System.Math.Pow(1.0 - (double) num23 * (double) num23, 0.15);
        data3[index].TextureCoordinate.Normalize();
        data3[index + 20].Position.Z = -data3[index].Position.Z;
        data3[index + 20].Position.Y = -data3[index].Position.Y;
        data3[index + 20].Normal.X = 1f;
        data3[index + 20].TextureCoordinate.X = -data3[index].TextureCoordinate.X;
        data3[index + 20].TextureCoordinate.Y = -data3[index].TextureCoordinate.Y;
        data3[index + 40].Position.Z = data3[index].Position.Z * 0.666f;
        data3[index + 40].Position.Y = data3[index].Position.Y * 0.666f;
        data3[index + 40].Position.X = 0.1332f;
        data3[index + 40].Normal.X = 1f;
        data3[index + 40].TextureCoordinate.X = data3[index].TextureCoordinate.X * 0.666f;
        data3[index + 40].TextureCoordinate.Y = data3[index].TextureCoordinate.Y * 0.666f;
        data3[index + 60].Position.Z = -data3[index + 40].Position.Z;
        data3[index + 60].Position.Y = -data3[index + 40].Position.Y;
        data3[index + 60].Position.X = 0.1332f;
        data3[index + 60].Normal.X = 1f;
        data3[index + 60].TextureCoordinate.X = -data3[index + 40].TextureCoordinate.X;
        data3[index + 60].TextureCoordinate.Y = -data3[index + 40].TextureCoordinate.Y;
        data3[index + 80 /*0x50*/].Position.Z = data3[index].Position.Z * 0.333f;
        data3[index + 80 /*0x50*/].Position.Y = data3[index].Position.Y * 0.333f;
        data3[index + 80 /*0x50*/].Position.X = 0.179999992f;
        data3[index + 80 /*0x50*/].Normal.X = 1f;
        data3[index + 80 /*0x50*/].TextureCoordinate.X = data3[index].TextureCoordinate.X * 0.333f;
        data3[index + 80 /*0x50*/].TextureCoordinate.Y = data3[index].TextureCoordinate.Y * 0.333f;
        data3[index + 100].Position.Z = -data3[index + 80 /*0x50*/].Position.Z;
        data3[index + 100].Position.Y = -data3[index + 80 /*0x50*/].Position.Y;
        data3[index + 100].Position.X = 0.179999992f;
        data3[index + 100].Normal.X = 1f;
        data3[index + 100].TextureCoordinate.X = -data3[index + 80 /*0x50*/].TextureCoordinate.X;
        data3[index + 100].TextureCoordinate.Y = -data3[index + 80 /*0x50*/].TextureCoordinate.Y;
        data3[index + 1 + 120].Position.Z = data3[index].Position.Z;
        data3[index + 1 + 120].Position.Y = data3[index].Position.Y;
        data3[index + 1 + 120].Normal.X = -1f;
        data3[index + 1 + 120].TextureCoordinate.X = data3[index].TextureCoordinate.X;
        data3[index + 1 + 120].TextureCoordinate.Y = data3[index].TextureCoordinate.Y;
        data3[index + 1 + 140].Position.Z = -data3[index].Position.Z;
        data3[index + 1 + 140].Position.Y = -data3[index].Position.Y;
        data3[index + 1 + 140].Normal.X = -1f;
        data3[index + 1 + 140].TextureCoordinate.X = -data3[index].TextureCoordinate.X;
        data3[index + 1 + 140].TextureCoordinate.Y = -data3[index].TextureCoordinate.Y;
        data3[index + 1 + 160 /*0xA0*/].Position.Z = data3[index].Position.Z * 0.666f;
        data3[index + 1 + 160 /*0xA0*/].Position.Y = data3[index].Position.Y * 0.666f;
        data3[index + 1 + 160 /*0xA0*/].Position.X = -0.1332f;
        data3[index + 1 + 160 /*0xA0*/].Normal.X = -1f;
        data3[index + 1 + 160 /*0xA0*/].TextureCoordinate.X = data3[index + 1 + 120].TextureCoordinate.X * 0.666f;
        data3[index + 1 + 160 /*0xA0*/].TextureCoordinate.Y = data3[index + 1 + 120].TextureCoordinate.Y * 0.666f;
        data3[index + 1 + 180].Position.Z = -data3[index + 40].Position.Z;
        data3[index + 1 + 180].Position.Y = -data3[index + 40].Position.Y;
        data3[index + 1 + 180].Position.X = -0.1332f;
        data3[index + 1 + 180].Normal.X = -1f;
        data3[index + 1 + 180].TextureCoordinate.X = -data3[index + 40].TextureCoordinate.X;
        data3[index + 1 + 180].TextureCoordinate.Y = -data3[index + 40].TextureCoordinate.Y;
        data3[index + 1 + 200].Position.Z = data3[index].Position.Z * 0.333f;
        data3[index + 1 + 200].Position.Y = data3[index].Position.Y * 0.333f;
        data3[index + 1 + 200].Position.X = -0.179999992f;
        data3[index + 1 + 200].Normal.X = -1f;
        data3[index + 1 + 200].TextureCoordinate.X = data3[index].TextureCoordinate.X * 0.333f;
        data3[index + 1 + 200].TextureCoordinate.Y = data3[index].TextureCoordinate.Y * 0.333f;
        data3[index + 1 + 220].Position.Z = -data3[index + 80 /*0x50*/].Position.Z;
        data3[index + 1 + 220].Position.Y = -data3[index + 80 /*0x50*/].Position.Y;
        data3[index + 1 + 220].Position.X = -0.179999992f;
        data3[index + 1 + 220].Normal.X = -1f;
        data3[index + 1 + 220].TextureCoordinate.X = -data3[index + 80 /*0x50*/].TextureCoordinate.X;
        data3[index + 1 + 220].TextureCoordinate.Y = -data3[index + 80 /*0x50*/].TextureCoordinate.Y;
        num21 = num23 + num22;
      }
      for (int index = 0; index < 40; ++index)
      {
        data4[index * 15] = (ushort) index;
        data4[index * 15 + 1] = (ushort) (index + 40);
        data4[index * 15 + 2] = (ushort) ((index + 1) % 40);
        data4[index * 15 + 3] = (ushort) (index + 40);
        data4[index * 15 + 4] = (ushort) ((index + 1) % 40 + 40);
        data4[index * 15 + 5] = (ushort) ((index + 1) % 40);
        data4[index * 15 + 6] = (ushort) (index + 40);
        data4[index * 15 + 7] = (ushort) (index + 80 /*0x50*/);
        data4[index * 15 + 8] = (ushort) ((index + 1) % 40 + 40);
        data4[index * 15 + 9] = (ushort) (index + 80 /*0x50*/);
        data4[index * 15 + 10] = (ushort) ((index + 1) % 40 + 80 /*0x50*/);
        data4[index * 15 + 11] = (ushort) ((index + 1) % 40 + 40);
        data4[index * 15 + 12] = (ushort) (index + 80 /*0x50*/);
        data4[index * 15 + 13] = (ushort) 120;
        data4[index * 15 + 14] = (ushort) ((index + 1) % 40 + 80 /*0x50*/);
        data4[600 + index * 15] = (ushort) (index + 120 + 1);
        data4[600 + index * 15 + 1] = (ushort) ((index + 1) % 40 + 120 + 1);
        data4[600 + index * 15 + 2] = (ushort) (index + 160 /*0xA0*/ + 1);
        data4[600 + index * 15 + 3] = (ushort) (index + 160 /*0xA0*/ + 1);
        data4[600 + index * 15 + 4] = (ushort) ((index + 1) % 40 + 120 + 1);
        data4[600 + index * 15 + 5] = (ushort) ((index + 1) % 40 + 160 /*0xA0*/ + 1);
        data4[600 + index * 15 + 6] = (ushort) (index + 160 /*0xA0*/ + 1);
        data4[600 + index * 15 + 7] = (ushort) ((index + 1) % 40 + 160 /*0xA0*/ + 1);
        data4[600 + index * 15 + 8] = (ushort) (index + 200 + 1);
        data4[600 + index * 15 + 9] = (ushort) (index + 200 + 1);
        data4[600 + index * 15 + 10] = (ushort) ((index + 1) % 40 + 160 /*0xA0*/ + 1);
        data4[600 + index * 15 + 11] = (ushort) ((index + 1) % 40 + 200 + 1);
        data4[600 + index * 15 + 12] = (ushort) (index + 200 + 1);
        data4[600 + index * 15 + 13] = (ushort) ((index + 1) % 40 + 200 + 1);
        data4[600 + index * 15 + 14] = (ushort) 241;
      }
      lock (graphicsDevice)
      {
        Shield.sWallVertices = new VertexBuffer(graphicsDevice, data3.Length * VertexPositionNormalTexture.SizeInBytes, BufferUsage.WriteOnly);
        Shield.sWallVertices.SetData<VertexPositionNormalTexture>(data3);
        Shield.sWallIndices = new IndexBuffer(graphicsDevice, data4.Length * 2, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
        Shield.sWallIndices.SetData<ushort>(data4);
      }
      Shield.sWallNumVertices = data3.Length;
      Shield.sWallNumPrimitives = data4.Length / 3;
    }
    this.mBody = new Body();
    this.mBody.Immovable = false;
    this.mBody.ApplyGravity = false;
    this.mBody.Tag = (object) this;
    this.mBody.AllowFreezing = false;
    this.mCollision = new CollisionSkin(this.mBody);
    this.mBody.CollisionSkin = this.mCollision;
    this.mCollision.AddPrimitive((Primitive) new HollowSphere(Vector3.Zero, 1f), 1, new MaterialProperties(1f, 0.8f, 0.8f));
    this.mTargetTint = this.mTint;
    this.mCollision.AddPrimitive((Primitive) new Box(new Vector3(-0.2f, -3f, -6f), Matrix.Identity, new Vector3(0.4f, 6f, 12f)), 1, new MaterialProperties(1f, 0.8f, 0.8f));
    this.mCollision.ApplyLocalTransform(Transform.Identity);
    this.mCollision.callbackFn += new CollisionCallbackFn(this.OnCollision);
    this.mCollision.postCollisionCallbackFn += new PostCollisionCallbackFn(this.PostCollision);
    this.mRenderData = new Shield.RenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      Shield.RenderData renderData = new Shield.RenderData();
      this.mRenderData[index] = renderData;
    }
    this.mDamagePoints = new Vector4[16 /*0x10*/];
  }

  private bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
  {
    return !((double) this.mIntersectKillTimer > 0.0 | (double) this.mHitPoints <= 0.0) || !(iSkin1.Owner is CharacterBody);
  }

  private void PostCollision(ref CollisionInfo iCollisionInfo)
  {
    if (iCollisionInfo.SkinInfo.Skin0 == this.mBody.CollisionSkin)
    {
      iCollisionInfo.SkinInfo.IgnoreSkin0 = true;
    }
    else
    {
      if (iCollisionInfo.SkinInfo.Skin1 != this.mBody.CollisionSkin)
        throw new Exception();
      iCollisionInfo.SkinInfo.IgnoreSkin1 = true;
    }
    int num = 0;
    if (iCollisionInfo.SkinInfo.Skin0.GetPrimitiveNewWorld(iCollisionInfo.SkinInfo.IndexPrim0).Type == JigLibX.Geometry.PrimitiveType.TriangleMesh || iCollisionInfo.SkinInfo.Skin1.GetPrimitiveNewWorld(iCollisionInfo.SkinInfo.IndexPrim1).Type == JigLibX.Geometry.PrimitiveType.TriangleMesh)
      ++num;
    for (int index = 0; index < this.mCollision.Collisions.Count; ++index)
    {
      CollDetectInfo skinInfo = this.mCollision.Collisions[index].SkinInfo;
      if (skinInfo.Skin0.GetPrimitiveNewWorld(skinInfo.IndexPrim0).Type == JigLibX.Geometry.PrimitiveType.TriangleMesh || skinInfo.Skin1.GetPrimitiveNewWorld(skinInfo.IndexPrim1).Type == JigLibX.Geometry.PrimitiveType.TriangleMesh)
        ++num;
    }
    if (num < 2)
      return;
    Vector3 position = this.mBody.Position;
    float result;
    Vector3.DistanceSquared(ref this.mLastPosition, ref position, out result);
    if ((double) result <= 9.9999999747524271E-07)
      return;
    this.Kill();
  }

  public bool Resting => (double) this.mRestingTimer < 0.0;

  public override Vector3 CalcImpulseVelocity(
    Vector3 iDirection,
    float iElevation,
    float iMassPower,
    float iDistance)
  {
    return new Vector3();
  }

  public virtual void Initialize(
    ISpellCaster iOwner,
    Vector3 iPosition,
    float iRadius,
    Vector3 iDirection,
    ShieldType iShieldType,
    float iHitpoints,
    Vector3 iColor)
  {
    this.mMaxHitPoints = 5000f;
    this.mHitPoints = iHitpoints;
    this.mOwner = iOwner;
    Matrix identity = Matrix.Identity with
    {
      Forward = iDirection,
      Up = Vector3.Up
    };
    identity.Right = Vector3.Cross(identity.Forward, identity.Up);
    identity.Up = Vector3.Cross(identity.Right, identity.Forward);
    identity.Up = Vector3.Normalize(identity.Up);
    identity.Right = Vector3.Normalize(identity.Right);
    this.mBody.MoveTo(iPosition, identity);
    this.mLastPosition = iPosition;
    this.mRadius = iRadius;
    this.mShieldType = iShieldType;
    float num = 3.14159274f;
    if (this.mShieldType == ShieldType.DISC)
      num = 0.7853982f;
    (this.mCollision.GetPrimitiveLocal(0) as HollowSphere).MaxAngle = num;
    (this.mCollision.GetPrimitiveNewWorld(0) as HollowSphere).MaxAngle = num;
    (this.mCollision.GetPrimitiveOldWorld(0) as HollowSphere).MaxAngle = num;
    (this.mCollision.GetPrimitiveLocal(0) as HollowSphere).Radius = iRadius;
    (this.mCollision.GetPrimitiveNewWorld(0) as HollowSphere).Radius = iRadius;
    (this.mCollision.GetPrimitiveOldWorld(0) as HollowSphere).Radius = iRadius;
    this.mCollision.UpdateWorldBoundingBox();
    this.Initialize();
    this.AudioEmitter.Position = iPosition;
    this.AudioEmitter.Forward = iDirection;
    this.AudioEmitter.Up = Vector3.Up;
    for (int index = 0; index < this.mDamagePoints.Length; ++index)
      this.mDamagePoints[index] = new Vector4();
    this.mNoiseOffset2 = new Vector2();
    this.mTextureScale = new Vector2((float) System.Math.Ceiling((double) iRadius * 0.66600000858306885));
    this.mTint.X = iColor.X;
    this.mTint.Y = iColor.Y;
    this.mTint.Z = iColor.Z;
    this.mTint.W = 4f;
    this.mTargetTint = this.mTint;
    this.mTint.W = 0.0f;
    Matrix result;
    Matrix.CreateRotationX(MathHelper.ToRadians(-40f), out result);
    this.mCue = AudioManager.Instance.GetCue(Banks.Spells, Shield.SOUNDHASH);
    this.mCue.SetVariable(Shield.HEALTH_VAR_NAME, this.mHitPoints / this.mMaxHitPoints);
    this.mCue.Apply3D(this.mPlayState.Camera.Listener, this.AudioEmitter);
    this.mCue.Play();
    this.mIntersectKillTimer = 0.0f;
    for (int index = 0; index < 3; ++index)
    {
      Shield.RenderData renderData = this.mRenderData[index];
      renderData.mBoundingSphere.Center = iPosition;
      renderData.mBoundingSphere.Radius = iRadius;
      renderData.mMinDotProduct = (float) System.Math.Cos((double) num);
      renderData.mDirection = iDirection;
      renderData.mDrawBack = iShieldType != ShieldType.WALL;
      switch (this.mShieldType)
      {
        case ShieldType.WALL:
          renderData.mVertices = Shield.sWallVertices;
          renderData.mIndices = Shield.sWallIndices;
          renderData.mNumVertices = Shield.sWallNumVertices;
          renderData.mNumPrimitives = Shield.sWallNumPrimitives;
          renderData.mRotation = Matrix.Identity;
          renderData.mTechnique = ShieldEffect.Technique.Wall;
          break;
        default:
          renderData.mVertices = Shield.sSphereVertices;
          renderData.mIndices = Shield.sSphereIndices;
          renderData.mNumVertices = Shield.sSphereNumVertices;
          renderData.mNumPrimitives = Shield.sSphereNumPrimitives;
          renderData.mRotation = result;
          renderData.mTechnique = ShieldEffect.Technique.Sphere;
          break;
      }
    }
    switch (this.mShieldType)
    {
      case ShieldType.WALL:
        this.mCollision.DisablePrimitive(0);
        this.mCollision.EnablePrimitive(1);
        break;
      default:
        this.mCollision.EnablePrimitive(0);
        this.mCollision.DisablePrimitive(1);
        break;
    }
    List<Shield> shields = this.mPlayState.EntityManager.Shields;
    Vector2 iCenterA = new Vector2(iPosition.X, iPosition.Z);
    Vector2 iDirA = new Vector2(iDirection.X, iDirection.Z);
    iDirA.Normalize();
    for (int index = 0; index < shields.Count; ++index)
    {
      Shield shield = shields[index];
      if (shield != this)
      {
        Vector2 iCenterB = new Vector2();
        iCenterB.X = shield.Position.X;
        iCenterB.Y = shield.Position.Z;
        Vector2 iDirB = new Vector2();
        iDirB.X = shield.Body.Orientation.Forward.X;
        iDirB.Y = shield.Body.Orientation.Forward.Z;
        if ((this.mShieldType == ShieldType.SPHERE | this.mShieldType == ShieldType.DISC) & (shield.mShieldType == ShieldType.SPHERE | shield.mShieldType == ShieldType.DISC))
        {
          float iMaxAngleB = shields[index].mShieldType == ShieldType.SPHERE ? 3.14159274f : 0.7853982f;
          if (Shield.CircleCircleIntersect(ref iCenterA, ref iDirA, this.mRadius, num, ref iCenterB, ref iDirB, shield.mRadius, iMaxAngleB))
          {
            shield.Kill();
            this.mIntersectKillTimer = 0.25f;
          }
        }
        else if (this.mShieldType == ShieldType.WALL && shield.mShieldType == ShieldType.WALL)
        {
          Vector2 iStartA = new Vector2(iPosition.X, iPosition.Z);
          Vector2 iEndA = iStartA + iDirA * 6f;
          iStartA -= iDirA * 6f;
          Vector2 iStartB = new Vector2(shield.Position.X, shield.Position.Z);
          Vector2 iEndB = iStartB + iDirB * 6f;
          iStartB -= iDirB * 6f;
          if (Shield.SegmentSegmentIntersect(ref iStartA, ref iEndA, ref iStartB, ref iEndB))
          {
            shield.Kill();
            this.mIntersectKillTimer = 0.25f;
          }
        }
        else
        {
          Segment iSeg;
          if (this.mShieldType == ShieldType.WALL)
          {
            iSeg.Origin = iPosition - iDirection * 6f;
            iSeg.Delta = iDirection * 6f * 2f;
          }
          else
          {
            if (shield.mShieldType != ShieldType.WALL)
              throw new Exception("Invalid ShieldType!");
            iSeg.Origin = shield.mBody.Position - shield.mBody.Orientation.Forward * 6f;
            iSeg.Delta = shield.mBody.Orientation.Forward * 6f * 2f;
          }
          Vector3 iCenter;
          Vector3 iDirection1;
          float mRadius;
          float iMaxAngle;
          if (this.mShieldType == ShieldType.DISC || this.mShieldType == ShieldType.SPHERE)
          {
            iCenter = iPosition;
            iDirection1 = iDirection;
            mRadius = this.mRadius;
            iMaxAngle = num;
          }
          else
          {
            if (shield.mShieldType != ShieldType.DISC && shield.mShieldType != ShieldType.SPHERE)
              throw new Exception("Invalid ShieldType!");
            iCenter = shield.Body.Position;
            iDirection1 = shield.Body.Orientation.Forward;
            mRadius = shield.mRadius;
            iMaxAngle = shield.mShieldType == ShieldType.SPHERE ? 3.14159274f : 0.7853982f;
          }
          if (Shield.SegmentCircleIntersect(ref iSeg, ref iCenter, ref iDirection1, mRadius, iMaxAngle))
          {
            shield.Kill();
            this.mIntersectKillTimer = 0.25f;
          }
        }
      }
    }
  }

  private static bool CircleCircleIntersect(
    ref Vector2 iCenterA,
    ref Vector2 iDirA,
    float iRadiusA,
    float iMaxAngleA,
    ref Vector2 iCenterB,
    ref Vector2 iDirB,
    float iRadiusB,
    float iMaxAngleB)
  {
    float result1;
    Vector2.Distance(ref iCenterA, ref iCenterB, out result1);
    if ((double) result1 + (double) System.Math.Abs(iRadiusA - iRadiusB) <= 1.4012984643248171E-45)
    {
      if ((double) MagickaMath.Angle(ref iDirA, ref iDirB) < (double) iMaxAngleA + (double) iMaxAngleB)
        return true;
    }
    else if ((double) result1 < (double) iRadiusB + (double) iRadiusA)
    {
      float scaleFactor = (float) (((double) iRadiusA * (double) iRadiusA - (double) iRadiusB * (double) iRadiusB + (double) result1 * (double) result1) / (2.0 * (double) result1));
      float num1 = (float) System.Math.Sqrt((double) iRadiusA * (double) iRadiusA - (double) scaleFactor * (double) scaleFactor);
      Vector2 result2;
      Vector2.Subtract(ref iCenterB, ref iCenterA, out result2);
      Vector2.Multiply(ref result2, scaleFactor, out result2);
      Vector2.Divide(ref result2, result1, out result2);
      Vector2.Add(ref iCenterA, ref result2, out result2);
      Vector2 vector2_1 = new Vector2(result2.X + num1 * (iCenterB.Y - iCenterA.Y) / result1, result2.Y - num1 * (iCenterB.X - iCenterA.X) / result1);
      Vector2 vector2_2 = new Vector2(result2.X - num1 * (iCenterB.Y - iCenterA.Y) / result1, result2.Y + num1 * (iCenterB.X - iCenterA.X) / result1);
      Vector2 result3;
      Vector2.Subtract(ref vector2_1, ref iCenterB, out result3);
      result3.Normalize();
      Vector2 result4;
      Vector2.Subtract(ref vector2_2, ref iCenterB, out result4);
      result4.Normalize();
      Vector2 result5;
      Vector2.Subtract(ref vector2_1, ref iCenterA, out result5);
      result5.Normalize();
      Vector2 result6;
      Vector2.Subtract(ref vector2_2, ref iCenterA, out result6);
      result6.Normalize();
      float num2 = MagickaMath.Angle(ref iDirB, ref result3);
      float num3 = MagickaMath.Angle(ref iDirB, ref result4);
      float num4 = MagickaMath.Angle(ref iDirA, ref result5);
      float num5 = MagickaMath.Angle(ref iDirA, ref result6);
      if ((double) num2 <= (double) iMaxAngleB | (double) num3 <= (double) iMaxAngleB && (double) num4 <= (double) iMaxAngleA | (double) num5 <= (double) iMaxAngleA)
        return true;
    }
    return false;
  }

  private static bool SegmentSegmentIntersect(
    ref Vector2 iStartA,
    ref Vector2 iEndA,
    ref Vector2 iStartB,
    ref Vector2 iEndB)
  {
    return (double) Distance.SegmentSegmentDistanceSq(out float _, out float _, new Segment()
    {
      Origin = {
        X = iStartA.X,
        Y = iStartA.Y
      },
      Delta = {
        X = iEndA.X - iStartA.X,
        Y = iEndA.Y - iStartA.Y
      }
    }, new Segment()
    {
      Origin = {
        X = iStartB.X,
        Y = iStartB.Y
      },
      Delta = {
        X = iEndB.X - iStartB.X,
        Y = iEndB.Y - iStartB.Y
      }
    }) <= 0.25;
  }

  private static bool SegmentCircleIntersect(
    ref Segment iSeg,
    ref Vector3 iCenter,
    ref Vector3 iDirection,
    float iRadius,
    float iMaxAngle)
  {
    Vector3 result1 = iCenter;
    Vector3.Subtract(ref result1, ref iSeg.Origin, out result1);
    float result2;
    Vector3.Dot(ref iSeg.Delta, ref iSeg.Delta, out result2);
    float result3;
    Vector3.Dot(ref iSeg.Delta, ref result1, out result3);
    float result4;
    Vector3.Dot(ref result1, ref result1, out result4);
    float num1 = result4 - iRadius * iRadius;
    float num2 = (float) System.Math.Sqrt((double) result3 * (double) result3 - (double) result2 * (double) num1);
    float num3 = 1f / result2;
    float num4 = (result3 + num2) * num3;
    float num5 = (result3 - num2) * num3;
    bool flag1 = (double) num4 > 0.0 & (double) num4 < 1.0 & !float.IsNaN(num4);
    bool flag2 = (double) num5 > 0.0 & (double) num5 < 1.0 & !float.IsNaN(num5);
    if (!flag1 & !flag2)
      return false;
    Vector3 vector1_1;
    iSeg.GetPoint(num4, out vector1_1);
    Vector3.Subtract(ref vector1_1, ref iCenter, out vector1_1);
    vector1_1.Normalize();
    float result5;
    Vector3.Dot(ref vector1_1, ref iDirection, out result5);
    float num6 = (float) System.Math.Acos((double) result5);
    Vector3 vector1_2;
    iSeg.GetPoint(num5, out vector1_2);
    Vector3.Subtract(ref vector1_2, ref iCenter, out vector1_2);
    vector1_2.Normalize();
    float result6;
    Vector3.Dot(ref vector1_2, ref iDirection, out result6);
    result6 = (float) System.Math.Acos((double) result6);
    if ((double) num6 > (double) iMaxAngle)
      flag1 = false;
    if ((double) result6 > (double) iMaxAngle)
      flag2 = false;
    return flag1 & ((double) num4 < (double) num5 | !flag2) || flag2 & ((double) num5 < (double) num4 | !flag1);
  }

  public override Matrix GetOrientation()
  {
    Matrix orientation = this.mBody.Orientation;
    if (this.mShieldType != ShieldType.WALL)
    {
      orientation.M11 *= this.mRadius;
      orientation.M12 *= this.mRadius;
      orientation.M13 *= this.mRadius;
      orientation.M21 *= this.mRadius;
      orientation.M22 *= this.mRadius;
      orientation.M23 *= this.mRadius;
      orientation.M31 *= this.mRadius;
      orientation.M32 *= this.mRadius;
      orientation.M33 *= this.mRadius;
    }
    orientation.Translation = this.mBody.Position;
    return orientation;
  }

  public float ResistanceAgainst(Elements iElement)
  {
    return 1f - MathHelper.Clamp((float) (0.0 / 300.0) + 0.0f, -1f, 1f);
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    base.Update(iDataChannel, iDeltaTime);
    this.mDamageTimer -= iDeltaTime;
    float num1 = 25f;
    if ((double) this.mIntersectKillTimer > 0.0)
    {
      this.mIntersectKillTimer -= iDeltaTime;
      if ((double) this.mIntersectKillTimer < 0.0)
        this.mHitPoints = 0.0f;
    }
    while ((double) this.mDamageTimer < 0.0)
    {
      this.mDamageTimer += 0.1f;
      this.mHitPoints -= 10f;
    }
    if ((double) this.mHitPoints <= 0.0)
    {
      num1 = 10f;
      this.mTargetTint.W = 0.0f;
      if ((double) this.mTint.W <= 0.0099999997764825821)
        this.mDead = true;
    }
    else
      this.mTargetTint.W = (double) this.mTint.W <= 3.7999999523162842 ? 1f : 1f;
    if ((double) this.mHitPoints > (double) this.mMaxHitPoints)
      this.mHitPoints = this.mMaxHitPoints;
    if ((double) this.mBody.Velocity.LengthSquared() > 9.9999999747524271E-07)
      this.mRestingTimer = 1f;
    else
      this.mRestingTimer -= iDeltaTime;
    this.mLastPosition = this.mBody.Position;
    this.mCue.SetVariable(Shield.HEALTH_VAR_NAME, this.mHitPoints / this.mMaxHitPoints);
    this.mCue.Apply3D(this.mPlayState.Camera.Listener, this.AudioEmitter);
    this.mTint.W += (this.mTargetTint.W - this.mTint.W) * iDeltaTime * num1;
    Vector3 position = this.Position;
    Vector2 vector2_1 = new Vector2();
    if (this.mShieldType == ShieldType.SPHERE)
    {
      position.Z += this.mRadius;
      vector2_1.Y = 12f;
    }
    else if (this.mShieldType == ShieldType.DISC)
    {
      Vector3 forward = this.mBody.Orientation.Forward;
      Vector3 right = Vector3.Right;
      float result;
      Vector3.Dot(ref forward, ref right, out result);
      result = System.Math.Abs(result);
      result = (float) System.Math.Pow((double) result, 2.0);
      position.X += forward.X * this.mRadius;
      position.Z += (float) ((double) forward.Z * (double) this.mRadius + (double) result * (double) this.mRadius * 0.66600000858306885 + 0.5);
      vector2_1.Y = 16f;
    }
    else if (this.mShieldType == ShieldType.WALL)
    {
      Vector3 forward = this.mBody.Orientation.Forward;
      Vector2 result1 = new Vector2(forward.X, forward.Z);
      Vector2 vector2_2 = new Vector2(1f, 0.0f);
      Vector2 vector2_3 = new Vector2(0.0f, 1f);
      float result2;
      Vector2.Dot(ref result1, ref vector2_3, out result2);
      Vector2.Dot(ref result1, ref vector2_2, out float _);
      Vector2.Multiply(ref result1, 6f * result2, out result1);
      position.X += result1.X;
      position.Z += result1.Y;
      vector2_1.Y = 24f;
    }
    Healthbars.Instance.AddHealthBar(position, this.mHitPoints / this.mMaxHitPoints, this.mRadius, 1f, 1f, false, new Vector4?(this.mTint), new Vector2?(vector2_1));
    float num2 = (float) (0.5 * ((double) this.mMaxHitPoints - (double) this.mHitPoints) / (double) this.mMaxHitPoints + 0.05000000074505806);
    this.mNoiseOffset2.Y -= iDeltaTime * num2;
    this.mNoiseOffset0.Y += iDeltaTime * 0.3f * num2;
    this.mNoiseOffset0.X -= iDeltaTime * 0.7f * num2;
    this.mNoiseOffset1.Y += iDeltaTime * 0.7f * num2;
    this.mNoiseOffset1.X += iDeltaTime * 0.4f * num2;
    Shield.RenderData iObject = this.mRenderData[(int) iDataChannel];
    iObject.mNoise0Offset = this.mNoiseOffset0;
    iObject.mNoise1Offset = this.mNoiseOffset1;
    iObject.mNoise2Offset = this.mNoiseOffset2;
    iObject.mTextureScale = this.mTextureScale;
    iObject.mTint = this.mTint;
    iObject.mTransform = this.GetOrientation();
    this.mDamagePoints.CopyTo((Array) iObject.mDamagePoints, 0);
    this.mPlayState.Scene.AddPostEffect(iDataChannel, (IPostEffect) iObject);
    for (int index = 0; index < this.mDamagePoints.Length; ++index)
      this.mDamagePoints[index].W -= iDeltaTime * 50f;
  }

  public new float Radius
  {
    get => base.Radius;
    set
    {
      this.mRadius = value;
      (this.mBody.CollisionSkin.GetPrimitiveLocal(0) as HollowSphere).Radius = this.mRadius;
      (this.mBody.CollisionSkin.GetPrimitiveNewWorld(0) as HollowSphere).Radius = this.mRadius;
      (this.mBody.CollisionSkin.GetPrimitiveOldWorld(0) as HollowSphere).Radius = this.mRadius;
      this.mBody.CollisionSkin.UpdateWorldBoundingBox();
    }
  }

  public override void Deinitialize()
  {
    base.Deinitialize();
    this.mCue.Stop(AudioStopOptions.AsAuthored);
    PhysicsManager.Instance.Simulator.CollisionSystem.RemoveCollisionSkin(this.mCollision);
    Shield.mCache.Add(this);
  }

  public virtual void Damage(float iDamage) => this.mHitPoints -= iDamage;

  public override bool Dead => this.mDead;

  public override bool Removable => this.Dead;

  public ShieldType ShieldType => this.mShieldType;

  public Vector3 GetNearestPosition(Vector3 iPosition)
  {
    Vector3 position = this.mBody.Position;
    Vector3.Subtract(ref iPosition, ref position, out iPosition);
    Vector3 result;
    if ((double) iPosition.LengthSquared() > 1.4012984643248171E-45)
    {
      Vector3.Normalize(ref iPosition, out result);
      Vector3.Multiply(ref result, this.mRadius, out result);
    }
    else
    {
      result = this.mBody.Orientation.Forward;
      Vector3.Multiply(ref result, this.mRadius, out result);
    }
    Vector3.Add(ref result, ref position, out result);
    return result;
  }

  public bool SegmentIntersect(out Vector3 oPosition, Segment iSeg, float iSegmentRadius)
  {
    return this.SegmentIntersect(out float _, out oPosition, iSeg, iSegmentRadius);
  }

  public bool SegmentIntersect(
    out float oFrac,
    out Vector3 oPosition,
    Segment iSeg,
    float iSegmentRadius)
  {
    Vector3 normal;
    return this.mShieldType == ShieldType.WALL ? this.mCollision.GetPrimitiveNewWorld(1).SegmentIntersect(out oFrac, out oPosition, out normal, ref iSeg) : this.mCollision.GetPrimitiveNewWorld(0).SegmentIntersect(out oFrac, out oPosition, out normal, ref iSeg);
  }

  public float HitPoints
  {
    get => this.mHitPoints;
    set => this.mHitPoints = value;
  }

  public float MaxHitPoints => this.mMaxHitPoints;

  public ISpellCaster Owner => this.mOwner;

  private void AddDamagePoint(Vector3 iPos, int iDamage)
  {
    for (int index = 0; index < this.mDamagePoints.Length; ++index)
    {
      Vector4 mDamagePoint = this.mDamagePoints[index];
      if ((double) this.mDamagePoints[index].W <= 0.0)
      {
        mDamagePoint.X = iPos.X;
        mDamagePoint.Y = iPos.Y;
        mDamagePoint.Z = iPos.Z;
        mDamagePoint.W = System.Math.Min(10f, (float) iDamage * 0.025f);
        this.mDamagePoints[index] = mDamagePoint;
        break;
      }
      float result;
      Vector3.DistanceSquared(ref new Vector3()
      {
        X = mDamagePoint.X,
        Y = mDamagePoint.Y,
        Z = mDamagePoint.Z
      }, ref iPos, out result);
      if ((double) result < 0.20000000298023224)
      {
        mDamagePoint.W = System.Math.Min(10f, mDamagePoint.W + (float) iDamage * 0.025f);
        this.mDamagePoints[index] = mDamagePoint;
        break;
      }
    }
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
    if ((iDamage.AttackProperty & (AttackProperties.Damage | AttackProperties.Status)) == (AttackProperties) 0)
      return DamageResult.Deflected;
    if (Defines.FeatureDamage(iFeatures) && (double) iDamage.Amount * (double) iDamage.Magnitude >= 0.0)
      this.mHitPoints -= iDamage.Amount * iDamage.Magnitude;
    this.AddDamagePoint(iAttackPosition, (int) iDamage.Amount);
    return DamageResult.Hit;
  }

  public override void Kill() => this.mHitPoints = 0.0f;

  public void Kill(float iIntersectTimer) => this.mIntersectKillTimer = iIntersectTimer;

  public override bool ArcIntersect(
    out Vector3 oPosition,
    Vector3 iOrigin,
    Vector3 iDirection,
    float iRange,
    float iAngle,
    float iHeightDifference)
  {
    Segment iSeg;
    iSeg.Origin = iOrigin;
    Vector3.Multiply(ref iDirection, iRange, out iSeg.Delta);
    return this.SegmentIntersect(out oPosition, iSeg, 0.5f);
  }

  public void OverKill() => this.mHitPoints = -this.mMaxHitPoints;

  protected override void INetworkUpdate(ref EntityUpdateMessage iMsg)
  {
    this.mHitPoints = iMsg.HitPoints;
    base.INetworkUpdate(ref iMsg);
  }

  protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
  {
    oMsg = new EntityUpdateMessage();
    if (!this.Resting)
    {
      oMsg.Features |= EntityFeatures.Position;
      oMsg.Position = this.Position;
      oMsg.Features |= EntityFeatures.Orientation;
      Matrix orientation = this.mBody.Orientation;
      Quaternion.CreateFromRotationMatrix(ref orientation, out oMsg.Orientation);
    }
    oMsg.Features |= EntityFeatures.Damageable;
    oMsg.HitPoints = this.mHitPoints;
  }

  public void Electrocute(IDamageable iTarget, float iMultiplyer)
  {
  }

  private class RenderData : IPostEffect
  {
    public bool mDrawBack;
    public Matrix mTransform;
    public Vector2 mNoise0Offset;
    public Vector2 mNoise1Offset;
    public Vector2 mNoise2Offset;
    public Vector2 mTextureScale;
    public Vector4 mTint;
    public float mMinDotProduct;
    public ShieldEffect.Technique mTechnique;
    public Vector3 mDirection;
    public Matrix mRotation;
    public int mNumVertices;
    public int mNumPrimitives;
    public IndexBuffer mIndices;
    public VertexBuffer mVertices;
    public Vector4[] mDamagePoints;
    public BoundingSphere mBoundingSphere;

    public RenderData() => this.mDamagePoints = new Vector4[16 /*0x10*/];

    public int ZIndex => 90;

    public void Draw(
      float iDeltaTime,
      ref Vector2 iPixelSize,
      ref Matrix iViewMatrix,
      ref Matrix iProjectionMatrix,
      Texture2D iCandidate,
      Texture2D iDepthMap,
      Texture2D iNormalMap)
    {
      Shield.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertices, 0, VertexPositionNormalTexture.SizeInBytes);
      Shield.mEffect.GraphicsDevice.Indices = this.mIndices;
      Shield.mEffect.GraphicsDevice.VertexDeclaration = Shield.sVertexDeclaration;
      Shield.mEffect.View = iViewMatrix;
      Shield.mEffect.Projection = iProjectionMatrix;
      Shield.mEffect.Thickness = 0.2f;
      Shield.mEffect.ColorTint = this.mTint;
      Shield.mEffect.DamagePoints = this.mDamagePoints;
      Shield.mEffect.TextureScale = this.mTextureScale;
      Shield.mEffect.Noise0Offset = this.mNoise0Offset;
      Shield.mEffect.Noise1Offset = this.mNoise1Offset;
      Shield.mEffect.Noise2Offset = this.mNoise2Offset;
      Shield.mEffect.MinDotProduct = this.mMinDotProduct;
      Shield.mEffect.Direction = this.mDirection;
      Shield.mEffect.DepthMap = iDepthMap;
      Matrix result;
      Matrix.Multiply(ref this.mTransform, ref this.mRotation, out result);
      result.Translation = this.mTransform.Translation;
      Shield.mEffect.World = result;
      Shield.mEffect.SetTechnique(this.mTechnique);
      Shield.mEffect.Begin();
      Shield.mEffect.CurrentTechnique.Passes[0].Begin();
      Shield.mEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 0, this.mNumVertices, 0, this.mNumPrimitives);
      Shield.mEffect.CurrentTechnique.Passes[0].End();
      Shield.mEffect.End();
    }
  }
}
