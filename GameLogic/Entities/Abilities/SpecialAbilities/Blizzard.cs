// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Blizzard
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public sealed class Blizzard : SpecialAbility, IAbilityEffect
{
  private const float MAGICK_TTL = 10f;
  private static Blizzard mSingelton;
  private static volatile object mSingeltonLock = new object();
  private static readonly int AMBIENCE = "magick_blizzard".GetHashCodeCustom();
  private float mTTL;
  private float mColdTimer;
  private Vector2 mTextureOffset0;
  private Vector2 mTextureOffset1;
  private float mAlpha;
  private PlayState mPlayState;
  private GameScene mScene;
  private Cue mAmbience;
  private Damage mDamage;
  private ISpellCaster mCaster;
  private Blizzard.RenderData[] mRenderData;
  private bool mDoDamage;

  public static Blizzard Instance
  {
    get
    {
      if (Blizzard.mSingelton == null)
      {
        lock (Blizzard.mSingeltonLock)
        {
          if (Blizzard.mSingelton == null)
            Blizzard.mSingelton = new Blizzard();
        }
      }
      return Blizzard.mSingelton;
    }
  }

  private Blizzard()
    : base(Magicka.Animations.cast_magick_global, "#magick_blizzard".GetHashCodeCustom())
  {
    this.mDamage.AttackProperty = AttackProperties.Status;
    this.mDamage.Element = Elements.Cold;
    this.mDamage.Magnitude = 1f;
    this.mDamage.Amount = 0.0f;
    FogEffect iEffect = new FogEffect(Magicka.Game.Instance.GraphicsDevice, (EffectPool) null);
    Texture2D iTexture = Magicka.Game.Instance.Content.Load<Texture2D>("EffectTextures/Blizzard");
    this.mRenderData = new Blizzard.RenderData[3];
    for (int index = 0; index < 3; ++index)
      this.mRenderData[index] = new Blizzard.RenderData(iEffect, iTexture);
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    this.mCaster = (ISpellCaster) null;
    this.mTimeStamp = 0.0;
    this.mPlayState = iPlayState;
    this.mDoDamage = NetworkManager.Instance.State != NetworkState.Client;
    return this.Execute();
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    this.mCaster = iOwner;
    this.mTimeStamp = iOwner.PlayState.PlayTime;
    this.mPlayState = iPlayState;
    Avatar avatar = iOwner as Avatar;
    this.mDoDamage = NetworkManager.Instance.State != NetworkState.Client ? NetworkManager.Instance.State != NetworkState.Server || avatar == null || !(avatar.Player.Gamer is NetworkGamer) : avatar != null && !(avatar.Player.Gamer is NetworkGamer);
    return this.Execute();
  }

  private bool Execute()
  {
    if (this.mPlayState.Level.CurrentScene.Indoors)
      return false;
    this.mScene = this.mPlayState.Level.CurrentScene;
    if (this.IsDead)
    {
      this.mTextureOffset0 = new Vector2(0.0f, 0.5f);
      this.mTextureOffset1 = new Vector2(0.5f, 0.0f);
      this.mAlpha = 0.0f;
    }
    if (this.mAmbience == null || !this.mAmbience.IsPlaying)
      this.mAmbience = AudioManager.Instance.PlayCue(Banks.Spells, Blizzard.AMBIENCE);
    this.mTTL = 10f;
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    if (this.mCaster is Avatar)
      this.mPlayState.IncrementBlizzardRainCount();
    return true;
  }

  public bool IsDead => (double) this.mTTL <= 0.0 & (double) this.mAlpha <= 0.0;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mTTL -= iDeltaTime;
    this.mColdTimer -= iDeltaTime;
    this.mAlpha = (double) this.mTTL <= 0.0 ? Math.Max(this.mAlpha - iDeltaTime, 0.0f) : Math.Min(this.mAlpha + iDeltaTime, 1f);
    this.mTextureOffset0.X -= iDeltaTime * 0.738724f;
    this.mTextureOffset0.Y -= iDeltaTime * 1.234238f;
    this.mTextureOffset1.X -= iDeltaTime * 1.324678f;
    this.mTextureOffset1.Y -= iDeltaTime * 0.647534f;
    Vector3 result = this.mPlayState.Camera.Position;
    Vector3 cameraoffset = MagickCamera.CAMERAOFFSET;
    new Vector3().Z = -1f;
    Vector3.Subtract(ref result, ref cameraoffset, out result);
    this.mDamage.Magnitude = 0.25f;
    if ((double) this.mColdTimer <= 0.0)
    {
      if (this.mDoDamage)
      {
        EntityManager entityManager = this.mPlayState.EntityManager;
        foreach (Entity entity in entityManager.Entities)
        {
          IDamageable t = entity as IDamageable;
          Shield oShield = (Shield) null;
          if (!(t == null | entity is MissileEntity) && (double) t.ResistanceAgainst(Elements.Cold) != 1.0 && !entityManager.IsProtectedByShield(entity, out oShield))
          {
            int num = (int) t.Damage(this.mDamage, this.mCaster as Entity, this.mTimeStamp, new Vector3());
          }
        }
      }
      foreach (Liquid liquid in this.mPlayState.Level.CurrentScene.Liquids)
        liquid.FreezeAll(0.25f);
      this.mColdTimer = 0.25f;
    }
    Blizzard.RenderData iObject = this.mRenderData[(int) iDataChannel];
    iObject.TextureOffset0 = this.mTextureOffset0;
    iObject.TextureOffset1 = this.mTextureOffset1;
    iObject.Alpha = this.mAlpha;
    this.mPlayState.Scene.AddPostEffect(iDataChannel, (IPostEffect) iObject);
  }

  public void OnRemove()
  {
    this.mTTL = 0.0f;
    if (this.mAmbience == null)
      return;
    this.mAmbience.Stop(AudioStopOptions.AsAuthored);
  }

  private class RenderData : IPostEffect
  {
    private FogEffect mEffect;
    private Texture2D mTexture;
    public float Alpha;
    public Vector2 TextureOffset0;
    public Vector2 TextureOffset1;

    public RenderData(FogEffect iEffect, Texture2D iTexture)
    {
      this.mEffect = iEffect;
      this.mTexture = iTexture;
    }

    public int ZIndex => 10000;

    public void Draw(
      float iDeltaTime,
      ref Vector2 iPixelSize,
      ref Matrix iViewMatrix,
      ref Matrix iProjectionMatrix,
      Texture2D iCandidate,
      Texture2D iDepthMap,
      Texture2D iNormalMap)
    {
      this.mEffect.SourceTexture0 = this.mTexture;
      this.mEffect.SourceTexture1 = this.mTexture;
      this.mEffect.TextureOffset0 = this.TextureOffset0;
      this.mEffect.TextureOffset1 = this.TextureOffset1;
      this.mEffect.SourceTexture2 = iDepthMap;
      this.mEffect.FogStart = 180f;
      this.mEffect.FogEnd = 240f;
      this.mEffect.Color = new Vector3(1.4f, 1.8f, 2f);
      this.mEffect.Alpha = this.Alpha;
      this.mEffect.SetTechnique(FogEffect.Technique.Linear);
      this.mEffect.DestinationDimensions = new Vector2()
      {
        X = (float) iDepthMap.Width,
        Y = (float) iDepthMap.Height
      };
      this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mEffect.VertexBuffer, 0, VertexPositionTexture.SizeInBytes);
      this.mEffect.GraphicsDevice.VertexDeclaration = this.mEffect.VertexDeclaration;
      this.mEffect.Begin();
      this.mEffect.CurrentTechnique.Passes[0].Begin();
      this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
      this.mEffect.CurrentTechnique.Passes[0].End();
      this.mEffect.End();
    }
  }
}
