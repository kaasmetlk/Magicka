// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Portal
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
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

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

internal class Portal : SpecialAbility
{
  private const string SOUND_VARIABLE = "Health";
  private const float MAX_DISTANCE = 30f;
  private static readonly int EFFECT_BLUE = "magick_portal_blue".GetHashCodeCustom();
  private static readonly int EFFECT_ORANGE = "magick_portal_orange".GetHashCodeCustom();
  private static readonly int EFFECT_INACTIVE = "magick_portal_inactive".GetHashCodeCustom();
  private static readonly int SOUND_CONNECTED = "spell_shield".GetHashCodeCustom();
  private static readonly int SOUND_SPAWN = "magick_teleportb".GetHashCodeCustom();
  private static readonly int SOUND_TELEPORT = "magick_teleporta".GetHashCodeCustom();
  private static Portal sSingelton;
  private static volatile object sSingeltonLock = new object();
  private static Portal.PortalEntity sPortalA;
  private static Portal.PortalEntity sPortalB;
  private static Cue sSoundA;
  private static Cue sSoundB;
  private static bool sConnected;
  private PlayState mPlayState;

  public static Portal Instance
  {
    get
    {
      if (Portal.sSingelton == null)
      {
        lock (Portal.sSingeltonLock)
        {
          if (Portal.sSingelton == null)
            Portal.sSingelton = new Portal();
        }
      }
      return Portal.sSingelton;
    }
  }

  private Portal()
    : base(Magicka.Animations.cast_area_fireworks, 0)
  {
  }

  public bool Connected => Portal.sConnected;

  public void Initialize(PlayState iPlayState)
  {
    Portal.sPortalA = new Portal.PortalEntity(iPlayState, Portal.PortalEntity.PortalType.Blue);
    Portal.sPortalB = new Portal.PortalEntity(iPlayState, Portal.PortalEntity.PortalType.Orange);
  }

  internal static Portal.PortalEntity OtherPortal(Portal.PortalEntity iPortalEntity)
  {
    return iPortalEntity == Portal.sPortalA ? Portal.sPortalB : Portal.sPortalA;
  }

  public void Kill()
  {
    Portal.sPortalA.Kill();
    Portal.sPortalB.Kill();
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    this.mPlayState = iPlayState;
    Vector3 result1 = iOwner.Position;
    Vector3 result2 = iOwner.Direction;
    Vector3.Multiply(ref result2, 2f, out result2);
    Vector3.Add(ref result1, ref result2, out result1);
    Vector3 oPoint;
    double nearestPosition = (double) iOwner.PlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref result1, out oPoint, MovementProperties.Water | MovementProperties.Dynamic);
    float result3;
    Vector3.DistanceSquared(ref oPoint, ref result1, out result3);
    if ((double) result3 > 25.0)
    {
      AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL, iOwner.AudioEmitter);
      return false;
    }
    if (NetworkManager.Instance.State != NetworkState.Client)
    {
      Helper.Swap<Portal.PortalEntity>(ref Portal.sPortalA, ref Portal.sPortalB);
      Portal.sPortalA.Initialize(ref oPoint);
    }
    return true;
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    if (NetworkManager.Instance.State != NetworkState.Client)
    {
      Vector3 iPoint = iPosition;
      Vector3 oPoint;
      double nearestPosition = (double) iPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref iPoint, out oPoint, MovementProperties.Water | MovementProperties.Dynamic);
      float result;
      Vector3.DistanceSquared(ref oPoint, ref iPoint, out result);
      if ((double) result > 25.0)
        return false;
      Helper.Swap<Portal.PortalEntity>(ref Portal.sPortalA, ref Portal.sPortalB);
      Portal.sPortalA.Initialize(ref oPoint);
    }
    return true;
  }

  internal void SpawnPortal(ref SpawnPortalMessage iMsg)
  {
    Helper.Swap<Portal.PortalEntity>(ref Portal.sPortalA, ref Portal.sPortalB);
    Portal.sPortalA.Initialize(ref iMsg);
  }

  public static void Update(PlayState iPlayState)
  {
    Vector3 position1 = Portal.sPortalA.Position;
    Vector3 position2 = Portal.sPortalB.Position;
    float result;
    Vector3.DistanceSquared(ref position1, ref position2, out result);
    Portal.sConnected = !Portal.sPortalA.Dead && !Portal.sPortalB.Dead && (double) result <= 900.0 / (double) iPlayState.Camera.Magnification;
    if (Portal.sConnected)
    {
      float num = (float) Math.Sqrt((double) result);
      if (Portal.sSoundA == null)
        Portal.sSoundA = AudioManager.Instance.GetCue(Banks.Spells, Portal.SOUND_CONNECTED);
      if (Portal.sSoundB == null)
        Portal.sSoundB = AudioManager.Instance.GetCue(Banks.Spells, Portal.SOUND_CONNECTED);
      Portal.sSoundA.Apply3D(Portal.sPortalA.PlayState.Camera.Listener, Portal.sPortalA.AudioEmitter);
      Portal.sSoundA.SetVariable("Volume", -8f);
      Portal.sSoundA.SetVariable("Health", (float) (1.0 - (double) num / 30.0));
      Portal.sSoundB.Apply3D(Portal.sPortalA.PlayState.Camera.Listener, Portal.sPortalB.AudioEmitter);
      Portal.sSoundB.SetVariable("Volume", -8f);
      Portal.sSoundB.SetVariable("Health", (float) (1.0 - (double) num / 30.0));
      if (Portal.sSoundA.IsPrepared)
        Portal.sSoundA.Play();
      if (!Portal.sSoundB.IsPrepared)
        return;
      Portal.sSoundB.Play();
    }
    else
    {
      if (Portal.sSoundA != null)
      {
        Portal.sSoundA.Stop(AudioStopOptions.AsAuthored);
        Portal.sSoundA = (Cue) null;
      }
      if (Portal.sSoundB == null)
        return;
      Portal.sSoundB.Stop(AudioStopOptions.AsAuthored);
      Portal.sSoundB = (Cue) null;
    }
  }

  public sealed class PortalEntity : Entity
  {
    private const float HEIGHT = 2f;
    private const float RADIUS = 0.5f;
    public static int VERTEXSTRIDE = VertexPositionTexture.SizeInBytes;
    public static readonly VertexPositionTexture[] QUAD = new VertexPositionTexture[4]
    {
      new VertexPositionTexture(new Vector3(-0.5f, -0.5f, 0.0f), new Vector2(0.0f, 1f)),
      new VertexPositionTexture(new Vector3(-0.5f, 0.5f, 0.0f), new Vector2(0.0f, 0.0f)),
      new VertexPositionTexture(new Vector3(0.5f, 0.5f, 0.0f), new Vector2(1f, 0.0f)),
      new VertexPositionTexture(new Vector3(0.5f, -0.5f, 0.0f), new Vector2(1f, 1f))
    };
    public static readonly ushort[] INDICES = new ushort[6]
    {
      (ushort) 0,
      (ushort) 1,
      (ushort) 2,
      (ushort) 0,
      (ushort) 2,
      (ushort) 3
    };
    private static Matrix sRotateX90 = Matrix.CreateRotationX(-1.57079637f);
    private bool mConnected;
    private VisualEffectReference mEffect;
    private new bool mDead;
    private AnimatedLevelPart mAnimatedPart;
    private Queue<Entity> mTeleportQueue = new Queue<Entity>(16 /*0x10*/);
    private static HitList sIgnoredEntities = new HitList(32 /*0x20*/);
    private float mEffectAlpha;
    private Portal.PortalEntity.PortalType mPortalType;
    private Portal.PortalEntity.RenderData[] mRenderData;

    internal PortalEntity(PlayState iPlayState, Portal.PortalEntity.PortalType iType)
      : base(iPlayState)
    {
      this.mPortalType = iType;
      this.mDead = true;
      this.mBody = new Body();
      this.mCollision = new CollisionSkin(this.mBody);
      this.mCollision.AddPrimitive((Primitive) new Capsule(new Vector3(0.0f, -1f, 0.0f), Portal.PortalEntity.sRotateX90, 0.5f, 2f), 1, new MaterialProperties(0.0f, 0.0f, 0.0f));
      this.mCollision.callbackFn += new CollisionCallbackFn(this.OnCollision);
      this.mBody.CollisionSkin = this.mCollision;
      this.mBody.Immovable = false;
      this.mBody.ApplyGravity = false;
      this.mBody.Tag = (object) this;
      this.mRenderData = new Portal.PortalEntity.RenderData[3];
      for (int index = 0; index < this.mRenderData.Length; ++index)
      {
        this.mRenderData[index] = new Portal.PortalEntity.RenderData();
        this.mRenderData[index].TextureOffset = this.mPortalType != Portal.PortalEntity.PortalType.Blue ? new Vector2(0.5f, 0.0f) : new Vector2(0.0f, 0.0f);
        this.mRenderData[index].Texture = Magicka.Game.Instance.Content.Load<Texture2D>("EffectTextures/portal");
      }
    }

    private bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
    {
      if (!Portal.Instance.Connected || iSkin1.Owner == null || !(iSkin1.Owner.Tag is Entity tag))
        return false;
      switch (tag)
      {
        case Barrier _:
        case Shield _:
        case Grease.GreaseField _:
label_6:
          return false;
        default:
          if (!Portal.PortalEntity.sIgnoredEntities.ContainsKey(tag.Handle))
            this.mTeleportQueue.Enqueue(tag);
          Portal.PortalEntity.sIgnoredEntities[tag.Handle] = 0.1f;
          goto label_6;
      }
    }

    public override Vector3 CalcImpulseVelocity(
      Vector3 iDirection,
      float iElevation,
      float iMassPower,
      float iDistance)
    {
      return new Vector3();
    }

    protected override void AddImpulseVelocity(ref Vector3 iVelocity)
    {
    }

    public void Initialize(ref Vector3 iPosition)
    {
      this.Deinitialize();
      this.mDead = false;
      this.mEffectAlpha = 0.0f;
      Segment iSeg = new Segment();
      iSeg.Origin = iPosition;
      iSeg.Origin.Y += 2f;
      iSeg.Delta.Y = -4f;
      Vector3 oPos;
      if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out Vector3 _, out this.mAnimatedPart, iSeg))
      {
        iPosition = oPos;
        iPosition.Y += 1.5f;
      }
      if (this.mAnimatedPart != null)
        this.mAnimatedPart.AddEntity((Entity) this);
      if (NetworkManager.Instance.State == NetworkState.Server)
      {
        SpawnPortalMessage iMessage;
        iMessage.Position = iPosition;
        iMessage.AnimationHandle = this.mAnimatedPart == null ? ushort.MaxValue : this.mAnimatedPart.Handle;
        NetworkManager.Instance.Interface.SendMessage<SpawnPortalMessage>(ref iMessage);
      }
      this.mBody.MoveTo(iPosition, Matrix.Identity);
      this.mConnected = Portal.Instance.Connected;
      Vector3 forward = Vector3.Forward;
      if (this.mConnected)
      {
        if (this.mPortalType == Portal.PortalEntity.PortalType.Blue)
          EffectManager.Instance.StartEffect(Portal.EFFECT_BLUE, ref iPosition, ref forward, out this.mEffect);
        else
          EffectManager.Instance.StartEffect(Portal.EFFECT_ORANGE, ref iPosition, ref forward, out this.mEffect);
      }
      this.mPlayState.EntityManager.AddEntity((Entity) this);
      this.Initialize();
      AudioManager.Instance.PlayCue(Banks.Spells, Portal.SOUND_SPAWN, this.AudioEmitter);
    }

    internal void Initialize(ref SpawnPortalMessage iMsg)
    {
      this.Deinitialize();
      this.mDead = false;
      this.mEffectAlpha = 0.0f;
      if (iMsg.AnimationHandle < ushort.MaxValue)
      {
        this.mAnimatedPart = AnimatedLevelPart.GetFromHandle((int) iMsg.AnimationHandle);
        if (this.mAnimatedPart != null)
          this.mAnimatedPart.AddEntity((Entity) this);
      }
      this.mBody.MoveTo(iMsg.Position, Matrix.Identity);
      this.mConnected = Portal.Instance.Connected;
      Vector3 forward = Vector3.Forward;
      if (this.mConnected)
      {
        if (this.mPortalType == Portal.PortalEntity.PortalType.Blue)
          EffectManager.Instance.StartEffect(Portal.EFFECT_BLUE, ref iMsg.Position, ref forward, out this.mEffect);
        else
          EffectManager.Instance.StartEffect(Portal.EFFECT_ORANGE, ref iMsg.Position, ref forward, out this.mEffect);
      }
      this.mPlayState.EntityManager.AddEntity((Entity) this);
      this.Initialize();
      AudioManager.Instance.PlayCue(Banks.Spells, Portal.SOUND_SPAWN, this.AudioEmitter);
    }

    public override bool Dead => this.mDead;

    public override bool Removable => this.mDead;

    public override void Kill() => this.mDead = true;

    public override void Deinitialize()
    {
      this.mDead = true;
      EffectManager.Instance.Stop(ref this.mEffect);
      if (this.mAnimatedPart != null)
        this.mAnimatedPart.RemoveEntity((Entity) this);
      this.mAnimatedPart = (AnimatedLevelPart) null;
      base.Deinitialize();
    }

    public override void Update(DataChannel iDataChannel, float iDeltaTime)
    {
      this.mBody.AllowFreezing = this.mAnimatedPart == null;
      if (this == Portal.sPortalA)
        Portal.PortalEntity.sIgnoredEntities.Update(iDeltaTime);
      while (this.mTeleportQueue.Count > 0)
      {
        Entity entity = this.mTeleportQueue.Dequeue();
        Vector3 position = entity.Body.Position;
        Vector3 oPos;
        this.GetOutPos(ref position, out oPos);
        entity.Body.Position = oPos;
        AudioManager.Instance.PlayCue(Banks.Spells, Portal.SOUND_TELEPORT, this.AudioEmitter);
      }
      Matrix orientation = this.mBody.Orientation with
      {
        Translation = this.mBody.Position
      };
      if (this.mConnected != Portal.Instance.Connected)
      {
        EffectManager.Instance.Stop(ref this.mEffect);
        this.mConnected = Portal.Instance.Connected;
        if (this.mConnected)
        {
          if (this.mPortalType == Portal.PortalEntity.PortalType.Blue)
            EffectManager.Instance.StartEffect(Portal.EFFECT_BLUE, ref orientation, out this.mEffect);
          else
            EffectManager.Instance.StartEffect(Portal.EFFECT_ORANGE, ref orientation, out this.mEffect);
        }
      }
      EffectManager.Instance.UpdateOrientation(ref this.mEffect, ref orientation);
      this.mEffectAlpha = Math.Min(this.mEffectAlpha + iDeltaTime, 1f);
      base.Update(iDataChannel, iDeltaTime);
      this.mRenderData[(int) iDataChannel].Position = this.mBody.Position;
      this.mRenderData[(int) iDataChannel].Alpha = this.mEffectAlpha;
      this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, (IRenderableAdditiveObject) this.mRenderData[(int) iDataChannel]);
    }

    internal override bool SendsNetworkUpdate(NetworkState iState) => false;

    protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
    {
      oMsg = new EntityUpdateMessage();
    }

    internal void GetOutPos(ref Vector3 iPos, out Vector3 oPos)
    {
      Vector3 result = this.mBody.Position;
      Vector3.Subtract(ref iPos, ref result, out result);
      Vector3.Multiply(ref result, 0.8f, out result);
      Portal.PortalEntity portalEntity = Portal.OtherPortal(this);
      oPos = portalEntity.Position;
      Vector3.Add(ref oPos, ref result, out oPos);
    }

    private class RenderData : IRenderableAdditiveObject, IPreRenderRenderer
    {
      private static IndexBuffer sIndexBuffer;
      private static VertexBuffer sVertexBuffer;
      private static VertexDeclaration sVertexDeclaration;
      private static int VERTICEHASH;
      public Texture2D Texture;
      public Vector3 Position;
      private Matrix mLookAt = Matrix.Identity;
      private BoundingSphere mBoundingSphere;
      public float Alpha;
      public Vector2 TextureOffset;

      public RenderData()
      {
        if (Portal.PortalEntity.RenderData.sVertexBuffer == null)
        {
          lock (Magicka.Game.Instance.GraphicsDevice)
          {
            Portal.PortalEntity.RenderData.sIndexBuffer = new IndexBuffer(Magicka.Game.Instance.GraphicsDevice, 2 * Portal.PortalEntity.INDICES.Length, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
            Portal.PortalEntity.RenderData.sIndexBuffer.SetData<ushort>(Portal.PortalEntity.INDICES);
            Portal.PortalEntity.RenderData.sVertexBuffer = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, Portal.PortalEntity.VERTEXSTRIDE * Portal.PortalEntity.QUAD.Length, BufferUsage.WriteOnly);
            Portal.PortalEntity.RenderData.sVertexBuffer.SetData<VertexPositionTexture>(Portal.PortalEntity.QUAD);
            Portal.PortalEntity.RenderData.sVertexDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
          }
          Portal.PortalEntity.RenderData.VERTICEHASH = Portal.PortalEntity.RenderData.sVertexBuffer.GetHashCode();
        }
        this.mBoundingSphere = new BoundingSphere();
        this.mBoundingSphere.Radius = 8f;
      }

      public int Effect => AdditiveEffect.TYPEHASH;

      public int Technique => 0;

      public VertexBuffer Vertices => Portal.PortalEntity.RenderData.sVertexBuffer;

      public int VerticesHashCode => Portal.PortalEntity.RenderData.VERTICEHASH;

      public int VertexStride => Portal.PortalEntity.VERTEXSTRIDE;

      public IndexBuffer Indices => Portal.PortalEntity.RenderData.sIndexBuffer;

      public VertexDeclaration VertexDeclaration
      {
        get => Portal.PortalEntity.RenderData.sVertexDeclaration;
      }

      public bool Cull(BoundingFrustum iViewFrustum)
      {
        this.mBoundingSphere.Center = this.Position;
        return this.mBoundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
      }

      public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
      {
        AdditiveEffect additiveEffect = iEffect as AdditiveEffect;
        this.mLookAt.Translation = this.Position;
        additiveEffect.World = this.mLookAt;
        additiveEffect.ColorTint = new Vector4(1f, 1f, 1f, this.Alpha);
        additiveEffect.TextureOffset = this.TextureOffset;
        additiveEffect.TextureScale = new Vector2(0.5f, 1f);
        additiveEffect.Texture = this.Texture;
        additiveEffect.TextureEnabled = true;
        additiveEffect.VertexColorEnabled = false;
        additiveEffect.CommitChanges();
        additiveEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
      }

      public void PreRenderUpdate(
        DataChannel iDataChannel,
        float iDeltaTime,
        ref Matrix iViewProjectionMatrix,
        ref Vector3 iCameraPosition,
        ref Vector3 iCameraDirection)
      {
        Vector3 result1 = Vector3.Up;
        Vector3 position = this.Position;
        Vector3 result2;
        Vector3.Subtract(ref iCameraPosition, ref position, out result2);
        result2.Normalize();
        Vector3 result3;
        Vector3.Cross(ref result1, ref result2, out result3);
        Vector3.Cross(ref result2, ref result3, out result1);
        this.mLookAt.Forward = result2;
        this.mLookAt.Right = result3;
        this.mLookAt.Up = result1;
        this.mLookAt.M11 *= 2f;
        this.mLookAt.M12 *= 2f;
        this.mLookAt.M13 *= 2f;
        this.mLookAt.M21 *= 4f;
        this.mLookAt.M22 *= 4f;
        this.mLookAt.M23 *= 4f;
      }
    }

    public enum PortalType
    {
      Blue,
      Orange,
    }
  }
}
