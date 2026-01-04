// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Conflagration
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
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

public class Conflagration : SpecialAbility, IAbilityEffect
{
  private const float RANGE = 20f;
  private const float TTL = 1f;
  private const float INV_TTL = 1f;
  private static List<Conflagration> sCache;
  public static readonly int SOUND = "magick_conflagration".GetHashCodeCustom();
  public static readonly int EFFECT = "magick_conflagration".GetHashCodeCustom();
  private static IndexBuffer sIndices;
  private static VertexBuffer sVertices;
  private static VertexDeclaration sVertexDeclaration;
  private static int sPrimitiveCount;
  private static BoundingBox sBoundingBox;
  private static Damage sInstantDamage = new Damage(AttackProperties.Damage, Elements.Fire, 600f, 1f);
  private static Damage sStatusDamage = new Damage(AttackProperties.Status, Elements.Fire, 75f, 2f);
  private float mTTL;
  private float mTime;
  private ISpellCaster mOwner;
  private PlayState mPlayState;
  private Conflagration.RenderData[] mRenderData;
  private AudioEmitter mAudioEmitter;
  private Vector3 mPosition;
  private Vector3 mDirection;
  private Vector3 mStartPosition;
  private HitList mHitList = new HitList(64 /*0x40*/);
  private VisualEffectReference mEffect;
  private new double mTimeStamp;

  public static Conflagration GetInstance()
  {
    if (Conflagration.sCache.Count <= 0)
      return new Conflagration();
    Conflagration instance = Conflagration.sCache[Conflagration.sCache.Count - 1];
    Conflagration.sCache.RemoveAt(Conflagration.sCache.Count - 1);
    return instance;
  }

  public static void InitializeCache(int iNr)
  {
    Conflagration.sCache = new List<Conflagration>(iNr);
    for (int index = 0; index < iNr; ++index)
      Conflagration.sCache.Add(new Conflagration());
  }

  private Conflagration()
    : base(Magicka.Animations.cast_magick_sweep, "#magick_conflagration".GetHashCodeCustom())
  {
    this.mAudioEmitter = new AudioEmitter();
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    if (Conflagration.sVertices == null)
    {
      Conflagration.Vertex[] data1 = new Conflagration.Vertex[32 /*0x20*/];
      int index1 = 0;
      float num1 = -2.3561945f;
      for (int index2 = 0; index2 < 8; ++index2)
      {
        float num2 = (float) Math.Cos((double) num1);
        float num3 = (float) Math.Sin((double) num1);
        data1[index1].Position.X = data1[index1].TexCoord.X = num2 * 4f;
        data1[index1].Position.Z = data1[index1].TexCoord.Y = (float) ((double) num3 * 2.0 + 3.5);
        Conflagration.Vertex[] vertexArray1 = data1;
        int index3 = index1;
        int index4 = index3 + 1;
        vertexArray1[index3].Alpha = 0.0f;
        data1[index4].Position.X = data1[index4].TexCoord.X = num2 * 6f;
        data1[index4].Position.Z = data1[index4].TexCoord.Y = (float) ((double) num3 * 3.0 + 3.5);
        Conflagration.Vertex[] vertexArray2 = data1;
        int index5 = index4;
        int index6 = index5 + 1;
        vertexArray2[index5].Alpha = index2 == 0 | index2 == 7 ? 0.0f : (index2 == 1 | index2 == 6 ? 0.5f : 1f);
        data1[index6].Position.X = data1[index6].TexCoord.X = num2 * 8f;
        data1[index6].Position.Z = data1[index6].TexCoord.Y = (float) ((double) num3 * 4.0 + 3.5);
        Conflagration.Vertex[] vertexArray3 = data1;
        int index7 = index6;
        int index8 = index7 + 1;
        vertexArray3[index7].Alpha = index2 == 0 | index2 == 7 ? 0.0f : 1f;
        data1[index8].Position.X = data1[index8].TexCoord.X = num2 * 10f;
        data1[index8].Position.Z = data1[index8].TexCoord.Y = (float) ((double) num3 * 5.0 + 3.5);
        Conflagration.Vertex[] vertexArray4 = data1;
        int index9 = index8;
        index1 = index9 + 1;
        vertexArray4[index9].Alpha = 0.0f;
        num1 += (float) Math.PI / 14f;
      }
      Conflagration.sBoundingBox.Max.X = float.MinValue;
      Conflagration.sBoundingBox.Max.Y = 1f;
      Conflagration.sBoundingBox.Max.Z = float.MinValue;
      Conflagration.sBoundingBox.Min.X = float.MaxValue;
      Conflagration.sBoundingBox.Min.Y = -1f;
      Conflagration.sBoundingBox.Min.Z = float.MaxValue;
      Conflagration.sBoundingBox.Max = new Vector3(5.5f, 1f, 1f);
      Conflagration.sBoundingBox.Min = new Vector3(-5.5f, -1f, -1f);
      ushort[] data2 = new ushort[126];
      int num4 = 0;
      for (int index10 = 0; index10 < 7; ++index10)
      {
        ushort[] numArray1 = data2;
        int index11 = num4;
        int num5 = index11 + 1;
        int num6 = (int) (ushort) (index10 * 4);
        numArray1[index11] = (ushort) num6;
        ushort[] numArray2 = data2;
        int index12 = num5;
        int num7 = index12 + 1;
        int num8 = (int) (ushort) (index10 * 4 + 1);
        numArray2[index12] = (ushort) num8;
        ushort[] numArray3 = data2;
        int index13 = num7;
        int num9 = index13 + 1;
        int num10 = (int) (ushort) (index10 * 4 + 4);
        numArray3[index13] = (ushort) num10;
        ushort[] numArray4 = data2;
        int index14 = num9;
        int num11 = index14 + 1;
        int num12 = (int) (ushort) (index10 * 4 + 1);
        numArray4[index14] = (ushort) num12;
        ushort[] numArray5 = data2;
        int index15 = num11;
        int num13 = index15 + 1;
        int num14 = (int) (ushort) (index10 * 4 + 5);
        numArray5[index15] = (ushort) num14;
        ushort[] numArray6 = data2;
        int index16 = num13;
        int num15 = index16 + 1;
        int num16 = (int) (ushort) (index10 * 4 + 4);
        numArray6[index16] = (ushort) num16;
        ushort[] numArray7 = data2;
        int index17 = num15;
        int num17 = index17 + 1;
        int num18 = (int) (ushort) (index10 * 4 + 1);
        numArray7[index17] = (ushort) num18;
        ushort[] numArray8 = data2;
        int index18 = num17;
        int num19 = index18 + 1;
        int num20 = (int) (ushort) (index10 * 4 + 2);
        numArray8[index18] = (ushort) num20;
        ushort[] numArray9 = data2;
        int index19 = num19;
        int num21 = index19 + 1;
        int num22 = (int) (ushort) (index10 * 4 + 5);
        numArray9[index19] = (ushort) num22;
        ushort[] numArray10 = data2;
        int index20 = num21;
        int num23 = index20 + 1;
        int num24 = (int) (ushort) (index10 * 4 + 2);
        numArray10[index20] = (ushort) num24;
        ushort[] numArray11 = data2;
        int index21 = num23;
        int num25 = index21 + 1;
        int num26 = (int) (ushort) (index10 * 4 + 6);
        numArray11[index21] = (ushort) num26;
        ushort[] numArray12 = data2;
        int index22 = num25;
        int num27 = index22 + 1;
        int num28 = (int) (ushort) (index10 * 4 + 5);
        numArray12[index22] = (ushort) num28;
        ushort[] numArray13 = data2;
        int index23 = num27;
        int num29 = index23 + 1;
        int num30 = (int) (ushort) (index10 * 4 + 2);
        numArray13[index23] = (ushort) num30;
        ushort[] numArray14 = data2;
        int index24 = num29;
        int num31 = index24 + 1;
        int num32 = (int) (ushort) (index10 * 4 + 3);
        numArray14[index24] = (ushort) num32;
        ushort[] numArray15 = data2;
        int index25 = num31;
        int num33 = index25 + 1;
        int num34 = (int) (ushort) (index10 * 4 + 6);
        numArray15[index25] = (ushort) num34;
        ushort[] numArray16 = data2;
        int index26 = num33;
        int num35 = index26 + 1;
        int num36 = (int) (ushort) (index10 * 4 + 3);
        numArray16[index26] = (ushort) num36;
        ushort[] numArray17 = data2;
        int index27 = num35;
        int num37 = index27 + 1;
        int num38 = (int) (ushort) (index10 * 4 + 7);
        numArray17[index27] = (ushort) num38;
        ushort[] numArray18 = data2;
        int index28 = num37;
        num4 = index28 + 1;
        int num39 = (int) (ushort) (index10 * 4 + 6);
        numArray18[index28] = (ushort) num39;
      }
      Conflagration.sVertices = new VertexBuffer(graphicsDevice, 24 * data1.Length, BufferUsage.WriteOnly);
      Conflagration.sVertices.SetData<Conflagration.Vertex>(data1);
      Conflagration.sIndices = new IndexBuffer(graphicsDevice, 2 * data2.Length, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
      Conflagration.sIndices.SetData<ushort>(data2);
      Conflagration.sPrimitiveCount = data2.Length / 3;
      Conflagration.sVertexDeclaration = new VertexDeclaration(graphicsDevice, Conflagration.Vertex.VertexElements);
    }
    NormalDistortionEffect iEffect = new NormalDistortionEffect(graphicsDevice, Magicka.Game.Instance.Content);
    iEffect.NormalTexture = Magicka.Game.Instance.Content.Load<Texture2D>("EffectTextures/NormalDistortion");
    this.mRenderData = new Conflagration.RenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      Conflagration.RenderData renderData = new Conflagration.RenderData(iEffect);
      this.mRenderData[index] = renderData;
    }
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    this.mHitList.Clear();
    this.mTimeStamp = iPlayState.PlayTime;
    this.mTTL = 1f;
    this.mTime = 0.0f;
    this.mPlayState = iPlayState;
    this.mOwner = (ISpellCaster) null;
    this.mPosition = iPosition;
    this.mStartPosition = iPosition;
    this.mDirection = new Vector3((float) SpecialAbility.RANDOM.NextDouble(), 0.0f, (float) SpecialAbility.RANDOM.NextDouble());
    this.mDirection.Normalize();
    Vector3.Add(ref this.mPosition, ref this.mDirection, out this.mPosition);
    Vector3.Add(ref this.mPosition, ref this.mDirection, out this.mPosition);
    Vector3.Add(ref this.mPosition, ref this.mDirection, out this.mPosition);
    this.mAudioEmitter.Position = this.mPosition;
    this.mAudioEmitter.Up = Vector3.Up;
    this.mAudioEmitter.Forward = Vector3.Right;
    AudioManager.Instance.PlayCue(Banks.Spells, Conflagration.SOUND, this.mAudioEmitter);
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    return true;
  }

  public bool Execute(Vector3 iPosition, Vector3 iDirection, PlayState iPlayState)
  {
    this.mHitList.Clear();
    this.mTimeStamp = iPlayState.PlayTime;
    this.mTTL = 1f;
    this.mTime = 0.0f;
    this.mPlayState = iPlayState;
    this.mOwner = (ISpellCaster) null;
    this.mPosition = iPosition;
    this.mStartPosition = iPosition;
    this.mDirection = iDirection;
    this.mAudioEmitter.Position = this.mPosition;
    this.mAudioEmitter.Up = Vector3.Up;
    this.mAudioEmitter.Forward = Vector3.Right;
    AudioManager.Instance.PlayCue(Banks.Spells, Conflagration.SOUND, this.mAudioEmitter);
    EffectManager.Instance.StartEffect(Conflagration.EFFECT, ref this.mPosition, ref this.mDirection, out this.mEffect);
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    return true;
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    this.mHitList.Clear();
    this.mTTL = 1f;
    this.mTime = 0.0f;
    this.mPlayState = iPlayState;
    this.mOwner = iOwner;
    this.mTimeStamp = iOwner.PlayState.PlayTime;
    this.mPosition = iOwner.Position;
    this.mStartPosition = this.mPosition;
    this.mDirection = iOwner.Direction;
    Vector3.Add(ref this.mPosition, ref this.mDirection, out this.mPosition);
    Vector3.Add(ref this.mPosition, ref this.mDirection, out this.mPosition);
    Vector3.Add(ref this.mPosition, ref this.mDirection, out this.mPosition);
    this.mAudioEmitter.Position = this.mPosition;
    this.mAudioEmitter.Up = Vector3.Up;
    this.mAudioEmitter.Forward = Vector3.Right;
    AudioManager.Instance.PlayCue(Banks.Spells, Conflagration.SOUND, this.mAudioEmitter);
    EffectManager.Instance.StartEffect(Conflagration.EFFECT, ref this.mPosition, ref this.mDirection, out this.mEffect);
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    return true;
  }

  public bool IsDead => (double) this.mTTL <= 0.0;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mTTL -= iDeltaTime;
    this.mTime += iDeltaTime * 1f;
    Conflagration.RenderData iObject = this.mRenderData[(int) iDataChannel];
    Vector3 result1;
    Vector3.Multiply(ref this.mDirection, (float) ((double) this.mTime * (double) this.mTime * 20.0), out result1);
    Vector3.Add(ref this.mPosition, ref result1, out result1);
    NetworkState state = NetworkManager.Instance.State;
    Matrix.CreateWorld(ref result1, ref this.mDirection, ref new Vector3()
    {
      Y = 1f
    }, out iObject.Transform);
    iObject.Time = this.mTime;
    if (state != NetworkState.Client && (!(this.mOwner is Avatar) || !((this.mOwner as Avatar).Player.Gamer is NetworkGamer)) || state == NetworkState.Client && this.mOwner is Avatar && !((this.mOwner as Avatar).Player.Gamer is NetworkGamer))
    {
      BoundingSphere boundingSphere = new BoundingSphere();
      Matrix result2;
      Matrix.Invert(ref iObject.Transform, out result2);
      List<Entity> entities = this.mPlayState.EntityManager.GetEntities(result1, 10f, false);
      List<Shield> shields = this.mPlayState.EntityManager.Shields;
      for (int index1 = 0; index1 < entities.Count; ++index1)
      {
        IDamageable t = entities[index1] as IDamageable;
        if (((t == null ? 1 : 0) | (this.mOwner == null ? 0 : (t == this.mOwner ? 1 : 0)) | (this.mHitList.ContainsKey(entities[index1].Handle) ? 1 : 0)) == 0)
        {
          boundingSphere.Center = t.Position;
          Vector3.Transform(ref boundingSphere.Center, ref result2, out boundingSphere.Center);
          boundingSphere.Radius = t.Radius;
          ContainmentType result3;
          boundingSphere.Contains(ref Conflagration.sBoundingBox, out result3);
          Segment iSeg = new Segment();
          iSeg.Origin = this.mStartPosition;
          Vector3 position = t.Position;
          Vector3.Subtract(ref position, ref this.mStartPosition, out iSeg.Delta);
          bool flag = false;
          for (int index2 = 0; index2 < shields.Count; ++index2)
          {
            if (shields[index2].SegmentIntersect(out Vector3 _, iSeg, 1f))
            {
              flag = true;
              break;
            }
          }
          if (result3 != ContainmentType.Disjoint && !flag)
          {
            if (t is Magicka.GameLogic.Entities.Character)
            {
              if (!(t as Magicka.GameLogic.Entities.Character).IsEthereal)
              {
                int num1 = (int) t.Damage(Conflagration.sInstantDamage, this.mOwner as Entity, this.mTimeStamp, this.mPosition);
                int num2 = (int) t.Damage(Conflagration.sStatusDamage, this.mOwner as Entity, this.mTimeStamp, this.mPosition);
                this.mHitList.Add(t.Handle);
              }
            }
            else
            {
              int num3 = (int) t.Damage(Conflagration.sInstantDamage, this.mOwner as Entity, this.mTimeStamp, this.mPosition);
              int num4 = (int) t.Damage(Conflagration.sStatusDamage, this.mOwner as Entity, this.mTimeStamp, this.mPosition);
              this.mHitList.Add(t.Handle);
            }
          }
        }
      }
      this.mPlayState.EntityManager.ReturnEntityList(entities);
    }
    iObject.Transform.M31 *= 1f + this.mTime;
    iObject.Transform.M32 *= 1f + this.mTime;
    iObject.Transform.M33 *= 1f + this.mTime;
    this.mAudioEmitter.Position = this.mPosition;
    this.mAudioEmitter.Up = Vector3.Up;
    this.mAudioEmitter.Forward = Vector3.Right;
    EffectManager.Instance.UpdatePositionDirection(ref this.mEffect, ref result1, ref this.mDirection);
    this.mPlayState.Scene.AddPostEffect(iDataChannel, (IPostEffect) iObject);
  }

  public void OnRemove()
  {
    EffectManager.Instance.Stop(ref this.mEffect);
    Conflagration.sCache.Add(this);
  }

  private class RenderData : IPostEffect
  {
    public float Alpha = 1f;
    public Matrix Transform;
    public float Time;
    private NormalDistortionEffect mEffect;

    public RenderData(NormalDistortionEffect iEffect) => this.mEffect = iEffect;

    public int ZIndex => 89;

    public void Draw(
      float iDeltaTime,
      ref Vector2 iPixelSize,
      ref Matrix iViewMatrix,
      ref Matrix iProjectionMatrix,
      Texture2D iCandidate,
      Texture2D iDepthMap,
      Texture2D iNormalMap)
    {
      this.mEffect.PixelSize = iPixelSize;
      this.mEffect.World = this.Transform;
      this.mEffect.View = iViewMatrix;
      this.mEffect.Projection = iProjectionMatrix;
      this.mEffect.SourceTexture = iCandidate;
      this.mEffect.DepthTexture = iDepthMap;
      this.mEffect.Time = this.Time;
      this.mEffect.Distortion = (float) ((double) this.Time * (double) this.Time * (1.0 - (double) this.Time) * 6.75);
      this.mEffect.NormalTexture = Magicka.Game.Instance.Content.Load<Texture2D>("EffectTextures/NormalDistortion");
      this.mEffect.GraphicsDevice.Vertices[0].SetSource(Conflagration.sVertices, 0, 24);
      this.mEffect.GraphicsDevice.VertexDeclaration = Conflagration.sVertexDeclaration;
      this.mEffect.GraphicsDevice.Indices = Conflagration.sIndices;
      this.mEffect.TextureScale = 4f;
      this.mEffect.Begin();
      this.mEffect.CurrentTechnique.Passes[0].Begin();
      this.mEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 0, 32 /*0x20*/, 0, Conflagration.sPrimitiveCount);
      this.mEffect.CurrentTechnique.Passes[0].End();
      this.mEffect.End();
    }
  }

  private struct Vertex
  {
    public const int SIZEINBYTES = 24;
    public Vector3 Position;
    public Vector2 TexCoord;
    public float Alpha;
    public static readonly VertexElement[] VertexElements = new VertexElement[3]
    {
      new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
      new VertexElement((short) 0, (short) 12, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 0),
      new VertexElement((short) 0, (short) 20, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.Color, (byte) 0)
    };
  }
}
