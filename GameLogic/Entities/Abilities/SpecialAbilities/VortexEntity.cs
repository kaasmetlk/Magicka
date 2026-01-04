// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.VortexEntity
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Physics;
using Magicka.Achievements;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Statistics;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Graphics.Effects;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

internal class VortexEntity : Entity
{
  private const float MAX_RADIUS = 10f;
  private const float START_MASS = 0.25f;
  private const float D = 0.75f;
  private static List<VortexEntity> sCache = (List<VortexEntity>) null;
  public static readonly int EFFECTHASH = "magick_vortex".GetHashCodeCustom();
  public static readonly int SOUNDHASH = "magick_vortex".GetHashCodeCustom();
  private static readonly double sLogD = Math.Log(0.75);
  private float mMass;
  private float mTargetMass;
  private new float mRadius;
  private VortexEntity.RenderData[] mRenderData;
  private static VortexEffect sEffect;
  private static VertexBuffer sVertices;
  private static IndexBuffer sIndices;
  private static VertexDeclaration sVertexDeclaration;
  private static int sNumVertices;
  private static int sPrimitiveCount;
  private VisualEffectReference mParticleEffect;
  private ISpellCaster mOwner;
  private BoundingSphere mSphere;
  private Cue mVortexCue;
  private int mHawkingHoleCount;

  public static VortexEntity GetInstance()
  {
    VortexEntity instance;
    lock (VortexEntity.sCache)
    {
      instance = VortexEntity.sCache[0];
      VortexEntity.sCache.RemoveAt(0);
      VortexEntity.sCache.Add(instance);
    }
    return instance;
  }

  public static VortexEntity GetSpecificInstance(ushort iHandle)
  {
    VortexEntity fromHandle;
    lock (VortexEntity.sCache)
    {
      fromHandle = Entity.GetFromHandle((int) iHandle) as VortexEntity;
      VortexEntity.sCache.Remove(fromHandle);
      VortexEntity.sCache.Add(fromHandle);
    }
    return fromHandle;
  }

  public static void InitializeCache(int iNr, PlayState iPlayState)
  {
    VortexEntity.sCache = new List<VortexEntity>(iNr);
    for (int index = 0; index < iNr; ++index)
      VortexEntity.sCache.Add(new VortexEntity(iPlayState));
  }

  public VortexEntity(PlayState iPlayState)
    : base(iPlayState)
  {
    this.mHawkingHoleCount = 0;
    this.mAudioEmitter = new AudioEmitter();
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    if (VortexEntity.sVertices == null)
    {
      Vector3[] data1 = new Vector3[42];
      int num1 = 0;
      Vector3[] vector3Array1 = data1;
      int index1 = num1;
      int index2 = index1 + 1;
      vector3Array1[index1] = Vector3.Up;
      double num2 = -1.0 * Math.PI / 10.0;
      double num3 = Math.PI / 6.0;
      for (int index3 = 0; index3 < 5; ++index3)
      {
        double num4 = (double) index3 * (2.0 * Math.PI / 5.0) + num2;
        data1[index2].Y = (float) Math.Cos(num3);
        double num5 = Math.Sin(num3);
        data1[index2].X = (float) (Math.Cos(num4) * num5);
        data1[index2].Z = (float) (Math.Sin(num4) * num5);
        ++index2;
      }
      double num6 = -1.0 * Math.PI / 10.0;
      double num7 = Math.PI / 3.0;
      for (int index4 = 0; index4 < 3; ++index4)
      {
        for (int index5 = 0; index5 < 10; ++index5)
        {
          double num8 = (double) index5 * (Math.PI / 5.0) + num6;
          data1[index2].Y = (float) Math.Cos(num7);
          double num9 = Math.Sin(num7);
          data1[index2].X = (float) (Math.Cos(num8) * num9);
          data1[index2].Z = (float) (Math.Sin(num8) * num9);
          ++index2;
        }
        num6 += Math.PI / 10.0;
        num7 += Math.PI / 6.0;
      }
      double num10 = num6 - Math.PI / 10.0;
      for (int index6 = 0; index6 < 5; ++index6)
      {
        double num11 = (double) index6 * (2.0 * Math.PI / 5.0) + num10;
        data1[index2].Y = (float) Math.Cos(num7);
        double num12 = Math.Sin(num7);
        data1[index2].X = (float) (Math.Cos(num11) * num12);
        data1[index2].Z = (float) (Math.Sin(num11) * num12);
        ++index2;
      }
      Vector3[] vector3Array2 = data1;
      int index7 = index2;
      int num13 = index7 + 1;
      vector3Array2[index7] = Vector3.Down;
      VortexEntity.sNumVertices = data1.Length;
      lock (graphicsDevice)
      {
        VortexEntity.sVertices = new VertexBuffer(graphicsDevice, data1.Length * 12, BufferUsage.WriteOnly);
        VortexEntity.sVertices.SetData<Vector3>(data1);
      }
      ushort[] data2 = new ushort[240 /*0xF0*/];
      int num14 = 0;
      for (int index8 = 0; index8 < 5; ++index8)
      {
        ushort[] numArray1 = data2;
        int index9 = num14;
        int num15 = index9 + 1;
        numArray1[index9] = (ushort) 0;
        ushort[] numArray2 = data2;
        int index10 = num15;
        int num16 = index10 + 1;
        int num17 = (int) (ushort) (index8 + 1);
        numArray2[index10] = (ushort) num17;
        ushort[] numArray3 = data2;
        int index11 = num16;
        num14 = index11 + 1;
        int num18 = (int) (ushort) ((index8 + 1) % 5 + 1);
        numArray3[index11] = (ushort) num18;
      }
      for (int index12 = 0; index12 < 5; ++index12)
      {
        ushort[] numArray4 = data2;
        int index13 = num14;
        int num19 = index13 + 1;
        int num20 = (int) (ushort) (index12 + 1);
        numArray4[index13] = (ushort) num20;
        ushort[] numArray5 = data2;
        int index14 = num19;
        int num21 = index14 + 1;
        int num22 = (int) (ushort) (index12 * 2 + 6);
        numArray5[index14] = (ushort) num22;
        ushort[] numArray6 = data2;
        int index15 = num21;
        int num23 = index15 + 1;
        int num24 = (int) (ushort) (index12 * 2 + 7);
        numArray6[index15] = (ushort) num24;
        ushort[] numArray7 = data2;
        int index16 = num23;
        int num25 = index16 + 1;
        int num26 = (int) (ushort) (index12 + 1);
        numArray7[index16] = (ushort) num26;
        ushort[] numArray8 = data2;
        int index17 = num25;
        int num27 = index17 + 1;
        int num28 = (int) (ushort) (index12 * 2 + 7);
        numArray8[index17] = (ushort) num28;
        ushort[] numArray9 = data2;
        int index18 = num27;
        int num29 = index18 + 1;
        int num30 = (int) (ushort) ((index12 + 1) % 5 + 1);
        numArray9[index18] = (ushort) num30;
        ushort[] numArray10 = data2;
        int index19 = num29;
        int num31 = index19 + 1;
        int num32 = (int) (ushort) ((index12 + 1) % 5 + 1);
        numArray10[index19] = (ushort) num32;
        ushort[] numArray11 = data2;
        int index20 = num31;
        int num33 = index20 + 1;
        int num34 = (int) (ushort) (index12 * 2 + 7);
        numArray11[index20] = (ushort) num34;
        ushort[] numArray12 = data2;
        int index21 = num33;
        num14 = index21 + 1;
        int num35 = (int) (ushort) ((index12 * 2 + 2) % 10 + 6);
        numArray12[index21] = (ushort) num35;
      }
      for (int index22 = 0; index22 < 2; ++index22)
      {
        for (int index23 = 0; index23 < 5; ++index23)
        {
          ushort[] numArray13 = data2;
          int index24 = num14;
          int num36 = index24 + 1;
          int num37 = (int) (ushort) (index23 * 2 + 6 + index22 * 10);
          numArray13[index24] = (ushort) num37;
          ushort[] numArray14 = data2;
          int index25 = num36;
          int num38 = index25 + 1;
          int num39 = (int) (ushort) (index23 * 2 + 16 /*0x10*/ + index22 * 10);
          numArray14[index25] = (ushort) num39;
          ushort[] numArray15 = data2;
          int index26 = num38;
          int num40 = index26 + 1;
          int num41 = (int) (ushort) (index23 * 2 + 7 + index22 * 10);
          numArray15[index26] = (ushort) num41;
          ushort[] numArray16 = data2;
          int index27 = num40;
          int num42 = index27 + 1;
          int num43 = (int) (ushort) (index23 * 2 + 7 + index22 * 10);
          numArray16[index27] = (ushort) num43;
          ushort[] numArray17 = data2;
          int index28 = num42;
          int num44 = index28 + 1;
          int num45 = (int) (ushort) (index23 * 2 + 16 /*0x10*/ + index22 * 10);
          numArray17[index28] = (ushort) num45;
          ushort[] numArray18 = data2;
          int index29 = num44;
          int num46 = index29 + 1;
          int num47 = (int) (ushort) (index23 * 2 + 17 + index22 * 10);
          numArray18[index29] = (ushort) num47;
          ushort[] numArray19 = data2;
          int index30 = num46;
          int num48 = index30 + 1;
          int num49 = (int) (ushort) (index23 * 2 + 7 + index22 * 10);
          numArray19[index30] = (ushort) num49;
          ushort[] numArray20 = data2;
          int index31 = num48;
          int num50 = index31 + 1;
          int num51 = (int) (ushort) (index23 * 2 + 17 + index22 * 10);
          numArray20[index31] = (ushort) num51;
          ushort[] numArray21 = data2;
          int index32 = num50;
          int num52 = index32 + 1;
          int num53 = (int) (ushort) ((index23 * 2 + 2) % 10 + 6 + index22 * 10);
          numArray21[index32] = (ushort) num53;
          ushort[] numArray22 = data2;
          int index33 = num52;
          int num54 = index33 + 1;
          int num55 = (int) (ushort) ((index23 * 2 + 2) % 10 + 6 + index22 * 10);
          numArray22[index33] = (ushort) num55;
          ushort[] numArray23 = data2;
          int index34 = num54;
          int num56 = index34 + 1;
          int num57 = (int) (ushort) (index23 * 2 + 17 + index22 * 10);
          numArray23[index34] = (ushort) num57;
          ushort[] numArray24 = data2;
          int index35 = num56;
          num14 = index35 + 1;
          int num58 = (int) (ushort) ((index23 * 2 + 2) % 10 + 16 /*0x10*/ + index22 * 10);
          numArray24[index35] = (ushort) num58;
        }
      }
      for (int index36 = 0; index36 < 5; ++index36)
      {
        ushort[] numArray25 = data2;
        int index37 = num14;
        int num59 = index37 + 1;
        int num60 = (int) (ushort) (index36 * 2 + 26);
        numArray25[index37] = (ushort) num60;
        ushort[] numArray26 = data2;
        int index38 = num59;
        int num61 = index38 + 1;
        int num62 = (int) (ushort) (index36 + 36);
        numArray26[index38] = (ushort) num62;
        ushort[] numArray27 = data2;
        int index39 = num61;
        int num63 = index39 + 1;
        int num64 = (int) (ushort) (index36 * 2 + 27);
        numArray27[index39] = (ushort) num64;
        ushort[] numArray28 = data2;
        int index40 = num63;
        int num65 = index40 + 1;
        int num66 = (int) (ushort) (index36 * 2 + 27);
        numArray28[index40] = (ushort) num66;
        ushort[] numArray29 = data2;
        int index41 = num65;
        int num67 = index41 + 1;
        int num68 = (int) (ushort) (index36 + 36);
        numArray29[index41] = (ushort) num68;
        ushort[] numArray30 = data2;
        int index42 = num67;
        int num69 = index42 + 1;
        int num70 = (int) (ushort) ((index36 + 1) % 5 + 36);
        numArray30[index42] = (ushort) num70;
        ushort[] numArray31 = data2;
        int index43 = num69;
        int num71 = index43 + 1;
        int num72 = (int) (ushort) (index36 * 2 + 27);
        numArray31[index43] = (ushort) num72;
        ushort[] numArray32 = data2;
        int index44 = num71;
        int num73 = index44 + 1;
        int num74 = (int) (ushort) ((index36 + 1) % 5 + 36);
        numArray32[index44] = (ushort) num74;
        ushort[] numArray33 = data2;
        int index45 = num73;
        num14 = index45 + 1;
        int num75 = (int) (ushort) ((index36 * 2 + 2) % 10 + 26);
        numArray33[index45] = (ushort) num75;
      }
      for (int index46 = 0; index46 < 5; ++index46)
      {
        ushort[] numArray34 = data2;
        int index47 = num14;
        int num76 = index47 + 1;
        int num77 = (int) (ushort) (index46 + 36);
        numArray34[index47] = (ushort) num77;
        ushort[] numArray35 = data2;
        int index48 = num76;
        int num78 = index48 + 1;
        numArray35[index48] = (ushort) 41;
        ushort[] numArray36 = data2;
        int index49 = num78;
        num14 = index49 + 1;
        int num79 = (int) (ushort) ((index46 + 1) % 5 + 36);
        numArray36[index49] = (ushort) num79;
      }
      VortexEntity.sPrimitiveCount = data2.Length / 3;
      lock (graphicsDevice)
      {
        VortexEntity.sIndices = new IndexBuffer(graphicsDevice, 480, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
        VortexEntity.sIndices.SetData<ushort>(data2);
        VortexEntity.sVertexDeclaration = new VertexDeclaration(graphicsDevice, new VertexElement[1]
        {
          new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0)
        });
      }
    }
    VortexEntity.sEffect = new VortexEffect();
    this.mBody = new Body();
    this.mCollision = new CollisionSkin();
    this.mBody.CollisionSkin = this.mCollision;
    this.mRenderData = new VortexEntity.RenderData[3];
    for (int index = 0; index < 3; ++index)
      this.mRenderData[index] = new VortexEntity.RenderData();
  }

  public void Initialize(ISpellCaster iOwner, Vector3 iPosition)
  {
    if (this.mVortexCue != null && !this.mVortexCue.IsStopping)
      this.mVortexCue.Stop(AudioStopOptions.AsAuthored);
    if (this.mOwner is Avatar && !((this.mOwner as Avatar).Player.Gamer is NetworkGamer) && this.mHawkingHoleCount >= 50)
      AchievementsManager.Instance.AwardAchievement(iOwner.PlayState, "icallitahawkinghole");
    EffectManager.Instance.Stop(ref this.mParticleEffect);
    this.mSphere = new BoundingSphere(iPosition, 0.5f);
    this.mRadius = 0.0f;
    this.mMass = 0.25f;
    this.mTargetMass = 0.25f;
    this.mHawkingHoleCount = 0;
    this.mOwner = iOwner;
    Vector3 iDirection = new Vector3();
    iDirection.Z = -1f;
    this.mAudioEmitter.Position = iPosition;
    this.mAudioEmitter.Forward = Vector3.Right;
    this.mAudioEmitter.Up = Vector3.Up;
    if (this.mVortexCue == null || !this.mVortexCue.IsPlaying)
      this.mVortexCue = AudioManager.Instance.PlayCue(Banks.Spells, VortexEntity.SOUNDHASH, this.mAudioEmitter);
    EffectManager.Instance.StartEffect(VortexEntity.EFFECTHASH, ref this.mSphere.Center, ref iDirection, out this.mParticleEffect);
  }

  public override void Deinitialize()
  {
    base.Deinitialize();
    if (this.mVortexCue != null && !this.mVortexCue.IsStopping)
      this.mVortexCue.Stop(AudioStopOptions.AsAuthored);
    if (this.mOwner is Avatar && !((this.mOwner as Avatar).Player.Gamer is NetworkGamer) && this.mHawkingHoleCount >= 50)
      AchievementsManager.Instance.AwardAchievement(this.mOwner.PlayState, "icallitahawkinghole");
    EffectManager.Instance.Stop(ref this.mParticleEffect);
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    base.Update(iDataChannel, iDeltaTime);
    this.mMass = MathHelper.Lerp(this.mTargetMass, this.mMass, (float) Math.Pow(0.1, (double) iDeltaTime));
    if ((double) this.mMass < 9.9999999747524271E-07)
      return;
    this.mRadius = this.mMass * 10f;
    List<Entity> entities = this.mPlayState.EntityManager.GetEntities(this.mSphere.Center, this.mRadius, true);
    for (int index = 0; index < entities.Count; ++index)
    {
      Entity entity = entities[index];
      if (entity is VortexEntity)
        entities.RemoveAt(index--);
      else if (!entity.Dead && (!(entity is Magicka.GameLogic.Entities.Character) || !(entity as Magicka.GameLogic.Entities.Character).IsEthereal) && !(entity is Portal.PortalEntity))
      {
        IDamageable iTarget = entity as IDamageable;
        float mass = entity.Body.Mass;
        if (this.mSphere.Intersects(entity.Body.CollisionSkin.WorldBoundingBox))
        {
          if (iTarget != null)
          {
            if (iTarget is Magicka.GameLogic.Entities.Character && !(iTarget as Magicka.GameLogic.Entities.Character).HasGibs())
              (iTarget as Magicka.GameLogic.Entities.Character).Terminate(true, false);
            else
              iTarget.OverKill();
            if (this.mOwner is Avatar && !((this.mOwner as Avatar).Player.Gamer is NetworkGamer))
            {
              StatisticsManager.Instance.AddDamageEvent(this.mOwner.PlayState, (IDamageable) this.mOwner, iTarget, this.mOwner.PlayState.PlayTime, new Damage()
              {
                Amount = iTarget.HitPoints
              }, DamageResult.Killed | DamageResult.OverKilled);
              if (iTarget is Magicka.GameLogic.Entities.Character && ((iTarget as Magicka.GameLogic.Entities.Character).Faction & (this.mOwner as Avatar).Faction) != (this.mOwner as Avatar).Faction)
                ++this.mHawkingHoleCount;
            }
          }
          else
            entity.Kill();
          if (entity.Dead)
            this.mTargetMass = 1f - (float) Math.Pow(0.75, Math.Log(1.0 - (double) this.mTargetMass) / VortexEntity.sLogD + (double) mass);
        }
        else
        {
          Vector3 position = entity.Position;
          float result1;
          Vector3.Distance(ref this.mSphere.Center, ref position, out result1);
          if ((double) result1 > 9.9999999747524271E-07)
          {
            float scaleFactor = (float) (15.0 * (1.0 - (double) result1 / (double) this.mRadius));
            if ((double) scaleFactor > 0.0)
            {
              Vector3 result2;
              Vector3.Subtract(ref this.mSphere.Center, ref position, out result2);
              Vector3.Divide(ref result2, result1, out result2);
              Vector3.Multiply(ref result2, scaleFactor, out result2);
              if (entity is Magicka.GameLogic.Entities.Character character)
              {
                character.CharacterBody.AdditionalForce = result2;
              }
              else
              {
                Vector3 result3 = entity.Body.Velocity;
                Vector3.Add(ref result3, ref result2, out result3);
                entity.Body.Velocity = result3;
              }
            }
          }
        }
      }
    }
    this.mPlayState.EntityManager.ReturnEntityList(entities);
    Matrix identity = Matrix.Identity with
    {
      Translation = this.mSphere.Center
    };
    float iScale = (float) ((double) this.mRadius / 10.0 * 2.0);
    if ((double) iScale < 0.25)
      iScale = 0.25f;
    MagickaMath.UniformMatrixScale(ref identity, iScale);
    EffectManager.Instance.UpdateOrientation(ref this.mParticleEffect, ref identity);
    VortexEntity.RenderData iObject = this.mRenderData[(int) iDataChannel];
    Matrix result;
    Matrix.CreateScale(this.mRadius, out result);
    result.Translation = this.mSphere.Center;
    iObject.Transform = result;
    iObject.Time = this.mRadius;
    iObject.Radius = this.mRadius;
    this.mPlayState.Scene.AddPostEffect(iDataChannel, (IPostEffect) iObject);
    this.mTargetMass -= iDeltaTime * 0.05f;
    this.mAudioEmitter.Position = this.mSphere.Center;
    this.mAudioEmitter.Forward = Vector3.Right;
    this.mAudioEmitter.Up = Vector3.Up;
  }

  public override bool Dead => (double) this.mMass < 1.4012984643248171E-45;

  public override bool Removable => this.Dead;

  public override void Kill() => this.mTargetMass = 0.0f;

  protected override void INetworkUpdate(ref EntityUpdateMessage iMsg)
  {
    base.INetworkUpdate(ref iMsg);
    this.mMass = iMsg.GenericFloat;
    this.mTargetMass = iMsg.WanderAngle;
  }

  protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
  {
    oMsg = new EntityUpdateMessage();
    oMsg.Features |= EntityFeatures.GenericFloat;
    oMsg.GenericFloat = this.mMass;
    oMsg.Features |= EntityFeatures.WanderAngle;
    oMsg.WanderAngle = this.mTargetMass;
  }

  internal override float GetDanger() => this.mMass * 20f;

  private class RenderData : IPostEffect
  {
    public float Alpha = 1f;
    public Matrix Transform;
    public float Time;
    public float Radius;

    public int ZIndex => 100;

    public void Draw(
      float iDeltaTime,
      ref Vector2 iPixelSize,
      ref Matrix iViewMatrix,
      ref Matrix iProjectionMatrix,
      Texture2D iCandidate,
      Texture2D iDepthMap,
      Texture2D iNormalMap)
    {
      VortexEntity.sEffect.PixelSize = iPixelSize;
      VortexEntity.sEffect.World = this.Transform;
      VortexEntity.sEffect.View = iViewMatrix;
      VortexEntity.sEffect.Projection = iProjectionMatrix;
      VortexEntity.sEffect.SourceTexture = iCandidate;
      VortexEntity.sEffect.DepthTexture = iDepthMap;
      VortexEntity.sEffect.Distortion = 0.025f;
      VortexEntity.sEffect.DistortionPower = 2f;
      VortexEntity.sEffect.GraphicsDevice.Vertices[0].SetSource(VortexEntity.sVertices, 0, 12);
      VortexEntity.sEffect.GraphicsDevice.VertexDeclaration = VortexEntity.sVertexDeclaration;
      VortexEntity.sEffect.GraphicsDevice.Indices = VortexEntity.sIndices;
      VortexEntity.sEffect.Begin();
      VortexEntity.sEffect.CurrentTechnique.Passes[0].Begin();
      VortexEntity.sEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, VortexEntity.sNumVertices, 0, VortexEntity.sPrimitiveCount);
      VortexEntity.sEffect.CurrentTechnique.Passes[0].End();
      VortexEntity.sEffect.End();
    }
  }
}
