// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Bosses.CountryDefense
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Bosses;

internal class CountryDefense : IBoss
{
  private List<DamageablePhysicsEntity> mHouseEntities = new List<DamageablePhysicsEntity>(5);
  private float[] mHouseAnimations;
  private float mDepth;
  private float mDepthOffset;
  private float mDepthDivisor;
  private float mTimer;
  private CountryDefense.RenderData[] mRenderData;
  private PlayState mPlayState;
  private NonPlayerCharacter mChieftain;

  public CountryDefense(PlayState iPlayState)
  {
    this.mPlayState = iPlayState;
    Texture3D texture3D = (Texture3D) null;
    VertexPositionTexture[] data = new VertexPositionTexture[4]
    {
      new VertexPositionTexture(new Vector3(0.0f, 1f, 0.0f), new Vector2(0.0f, 1f)),
      new VertexPositionTexture(new Vector3(0.0f, 0.0f, 0.0f), new Vector2(0.0f, 0.0f)),
      new VertexPositionTexture(new Vector3(1f, 0.0f, 0.0f), new Vector2(1f, 0.0f)),
      new VertexPositionTexture(new Vector3(1f, 1f, 0.0f), new Vector2(1f, 1f))
    };
    VertexBuffer vertexBuffer;
    VertexDeclaration vertexDeclaration;
    GUIBasicEffect guiBasicEffect;
    lock (this.mPlayState.Scene.GraphicsDevice)
    {
      vertexBuffer = new VertexBuffer(this.mPlayState.Scene.GraphicsDevice, VertexPositionTexture.SizeInBytes * data.Length, BufferUsage.WriteOnly);
      vertexBuffer.SetData<VertexPositionTexture>(data);
      vertexDeclaration = new VertexDeclaration(this.mPlayState.Scene.GraphicsDevice, VertexPositionTexture.VertexElements);
      guiBasicEffect = new GUIBasicEffect(this.mPlayState.Scene.GraphicsDevice, RenderManager.Instance.GlobalDummyEffect.EffectPool);
      guiBasicEffect.SetTechnique(GUIBasicEffect.Technique.Texture3D);
      texture3D = this.mPlayState.Content.Load<Texture3D>("UI/Boss/CountryDefense");
    }
    guiBasicEffect.Texture = (Texture) texture3D;
    this.mDepth = (float) texture3D.Depth;
    this.mDepthDivisor = 1f / this.mDepth;
    this.mDepthOffset = this.mDepthDivisor * 0.5f;
    this.mHouseAnimations = new float[5];
    this.mRenderData = new CountryDefense.RenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      this.mRenderData[index] = new CountryDefense.RenderData();
      this.mRenderData[index].mHouseAnimations = new float[5];
      this.mRenderData[index].Effect = guiBasicEffect;
      this.mRenderData[index].Size = new Vector2((float) texture3D.Width, (float) texture3D.Height);
      this.mRenderData[index].DivSize = new Vector2(1f / (float) texture3D.Width, 1f / (float) texture3D.Height);
      this.mRenderData[index].VertexBuffer = vertexBuffer;
      this.mRenderData[index].VertexDeclaration = vertexDeclaration;
    }
  }

  public void Initialize(ref Matrix iOrientation, int iUniqueID)
  {
    this.Initialize(ref iOrientation);
  }

  public void Initialize(ref Matrix iOrientation)
  {
    this.mHouseAnimations = new float[5];
    this.mHouseEntities.Clear();
    this.mHouseEntities.Add((DamageablePhysicsEntity) Entity.GetByID("house0".GetHashCodeCustom()));
    this.mHouseEntities.Add((DamageablePhysicsEntity) Entity.GetByID("house1".GetHashCodeCustom()));
    this.mHouseEntities.Add((DamageablePhysicsEntity) Entity.GetByID("house2".GetHashCodeCustom()));
    this.mHouseEntities.Add((DamageablePhysicsEntity) Entity.GetByID("house3".GetHashCodeCustom()));
    this.mHouseEntities.Add((DamageablePhysicsEntity) Entity.GetByID("house4".GetHashCodeCustom()));
    this.mChieftain = (NonPlayerCharacter) Entity.GetByID("#ent_beast_chieftain".GetHashCodeCustom());
    this.mTimer = 0.0f;
  }

  public void UpdateBoss(DataChannel iDataChannel, float iDeltaTime, bool iFightStarted)
  {
    if (!this.mPlayState.IsInCutscene)
      this.mTimer += iDeltaTime;
    if (this.mChieftain == null)
      this.mChieftain = (NonPlayerCharacter) Entity.GetByID("#ent_beast_chieftain".GetHashCodeCustom());
    for (int index = 0; index < this.mHouseEntities.Count; ++index)
    {
      if (this.mHouseEntities[index] == null)
      {
        this.mHouseAnimations[index] = 1f - this.mDepthOffset;
        this.mHouseEntities.RemoveAt(index--);
      }
      else
      {
        float num = Math.Max(this.mHouseEntities[index].HitPoints / this.mHouseEntities[index].MaxHitPoints, 0.0f);
        this.mHouseAnimations[index] = (float) Math.Ceiling((double) num * ((double) this.mDepth - 1.0)) * this.mDepthDivisor + this.mDepthDivisor;
        this.mHouseAnimations[index] = 1f - this.mHouseAnimations[index];
        this.mHouseAnimations[index] += this.mDepthOffset;
      }
    }
    CountryDefense.RenderData iObject = this.mRenderData[(int) iDataChannel];
    this.mHouseAnimations.CopyTo((Array) iObject.mHouseAnimations, 0);
    iObject.Alpha = Math.Min(this.mTimer, 1f);
    this.mPlayState.Scene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) iObject);
  }

  public void DeInitialize()
  {
  }

  public bool Dead => false;

  public float MaxHitPoints => this.mChieftain == null ? 1f : this.mChieftain.MaxHitPoints;

  public float HitPoints => this.mChieftain == null ? 1f : this.mChieftain.HitPoints;

  public DamageResult Damage(
    int iPartIndex,
    Magicka.GameLogic.Damage iDamage,
    Entity iAttacker,
    ref Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
    return DamageResult.None;
  }

  public void Damage(int iPartIndex, float iDamage, Elements iElement)
  {
  }

  public void SetSlow(int iIndex)
  {
  }

  public void GetRandomPositionOnCollisionSkin(int iIndex, out Vector3 oPosition)
  {
    oPosition = new Vector3();
  }

  public bool HasStatus(int iIndex, StatusEffects iStatus) => false;

  public float StatusMagnitude(int iIndex, StatusEffects iStatus) => 0.0f;

  public StatusEffect[] GetStatusEffects() => (StatusEffect[]) null;

  public bool AddImpulseVelocity(
    Vector3 iDirection,
    float iElevation,
    float iMassPower,
    float iDistance)
  {
    return false;
  }

  public void ScriptMessage(BossMessages iMessage)
  {
  }

  public void NetworkUpdate(ref BossUpdateMessage iMsg)
  {
  }

  public void NetworkInitialize(ref BossInitializeMessage iMsg)
  {
  }

  public BossEnum GetBossType() => BossEnum.CountryDefense;

  public bool NetworkInitialized => true;

  public float ResistanceAgainst(Elements iElement) => 1f;

  protected class RenderData : IRenderableGUIObject
  {
    private const float PADDING = 8f;
    public VertexBuffer VertexBuffer;
    public VertexDeclaration VertexDeclaration;
    public GUIBasicEffect Effect;
    public float Alpha;
    public Vector2 Size;
    public Vector2 DivSize;
    public float[] mHouseAnimations;

    public void Draw(float iDeltaTime)
    {
      Point screenSize = RenderManager.Instance.ScreenSize;
      float num1 = Math.Min(RenderManager.Instance.GUIScale, 1f);
      this.Effect.SetScreenSize(screenSize.X, screenSize.Y);
      this.Effect.GraphicsDevice.Vertices[0].SetSource(this.VertexBuffer, 0, VertexPositionTexture.SizeInBytes);
      this.Effect.GraphicsDevice.VertexDeclaration = this.VertexDeclaration;
      Vector2 vector2 = new Vector2(this.Size.X * num1, this.Size.Y * num1);
      int length = this.mHouseAnimations.Length;
      float num2 = (float) screenSize.X * 0.1f;
      Matrix identity = Matrix.Identity with
      {
        M11 = vector2.X,
        M22 = vector2.Y,
        M41 = num2,
        M42 = 32f
      };
      this.Effect.Transform = identity;
      this.Effect.TextureEnabled = true;
      this.Effect.VertexColorEnabled = false;
      this.Effect.Color = new Vector4(1f, 1f, 1f, this.Alpha);
      this.Effect.Begin();
      this.Effect.CurrentTechnique.Passes[0].Begin();
      for (int index = 0; index < this.mHouseAnimations.Length; ++index)
      {
        this.Effect.Transform = identity;
        this.Effect.CommitChanges();
        this.Effect.W = this.mHouseAnimations[index];
        this.Effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
        identity.M41 += 8f + vector2.X;
      }
      this.Effect.CurrentTechnique.Passes[0].End();
      this.Effect.End();
    }

    public int ZIndex => 100;
  }
}
