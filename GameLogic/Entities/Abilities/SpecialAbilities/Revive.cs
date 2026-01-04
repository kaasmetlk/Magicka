// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Revive
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Levels.Versus;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using PolygonHead.Lights;
using SteamWrapper;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class Revive : SpecialAbility, IAbilityEffect
{
  private static List<Revive> sCache;
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
  public static readonly int SOUND_REVIVE_HASH = "magick_revive_cast".GetHashCodeCustom();
  public static readonly int SOUNDHASH = "magick_revive".GetHashCodeCustom();
  public static readonly int SOUND_2_HASH = "magick_revive2".GetHashCodeCustom();
  public static readonly int EFFECT = "magick_revive".GetHashCodeCustom();
  private bool mDead;
  private bool mRessed;
  private float mTimer;
  private Vector3 mPosition;
  private Magicka.GameLogic.Player mRevivee;
  private PlayState mPlayState;
  private Revive.GodRayRenderData[] mRenderData;
  private SpotLight mSpotLight;
  private ISpellCaster mOwner;
  private float mHealthPercentage;
  private bool mIDSpecific;
  private int mPlayerID;
  private int mTypeID;

  public static Revive GetInstance()
  {
    if (Revive.sCache.Count <= 0)
      return new Revive();
    Revive instance = Revive.sCache[Revive.sCache.Count - 1];
    Revive.sCache.RemoveAt(Revive.sCache.Count - 1);
    return instance;
  }

  public static void InitializeCache(int iNr)
  {
    Revive.sCache = new List<Revive>(iNr);
    for (int index = 0; index < iNr; ++index)
      Revive.sCache.Add(new Revive());
  }

  public void SetSpecificPlayer(int iID)
  {
    this.mPlayerID = iID;
    this.mIDSpecific = true;
  }

  private Revive()
    : base(Magicka.Animations.cast_magick_self, "#magick_revive".GetHashCodeCustom())
  {
    Texture2D texture2D = Magicka.Game.Instance.Content.Load<Texture2D>("Levels/Textures/Surface/Nature/Atmosphere/light_ray01");
    this.mRenderData = new Revive.GodRayRenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      this.mRenderData[index] = new Revive.GodRayRenderData();
      this.mRenderData[index].Texture = texture2D;
    }
    lock (Magicka.Game.Instance.GraphicsDevice)
      this.mSpotLight = new SpotLight(Magicka.Game.Instance.GraphicsDevice);
  }

  public Revive(Magicka.Animations iAnimation)
    : base(iAnimation, "#magick_revive".GetHashCodeCustom())
  {
    Texture2D texture2D = Magicka.Game.Instance.Content.Load<Texture2D>("Levels/Textures/Surface/Nature/Atmosphere/light_ray01");
    this.mRenderData = new Revive.GodRayRenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      this.mRenderData[index] = new Revive.GodRayRenderData();
      this.mRenderData[index].Texture = texture2D;
    }
    lock (Magicka.Game.Instance.GraphicsDevice)
      this.mSpotLight = new SpotLight(Magicka.Game.Instance.GraphicsDevice);
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    return this.Execute(iPosition, iPlayState, 1f);
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    this.mOwner = iOwner;
    Vector3 result1 = iOwner.Position;
    Vector3 result2 = iOwner.Direction;
    Vector3.Multiply(ref result2, 4f, out result2);
    Vector3.Add(ref result1, ref result2, out result1);
    return this.Execute(result1, iPlayState, 0.1f);
  }

  internal bool Execute(Vector3 iPosition, PlayState iPlayState, float iHealth)
  {
    VersusRuleset ruleSet = iPlayState.Level.CurrentScene.RuleSet as VersusRuleset;
    Magicka.GameLogic.Player iReviver = (Magicka.GameLogic.Player) null;
    if (this.mOwner is Avatar)
      iReviver = (this.mOwner as Avatar).Player;
    this.mHealthPercentage = iHealth;
    this.mPlayState = iPlayState;
    float num1 = float.Epsilon;
    int num2 = -1;
    Magicka.GameLogic.Player[] players = Magicka.Game.Instance.Players;
    if (this.mIDSpecific)
    {
      Magicka.GameLogic.Player iRevivee = players[this.mPlayerID];
      if (iRevivee.Playing && !iRevivee.Ressing && (iRevivee.Avatar == null || iRevivee.Avatar.Overkilled || iRevivee.Avatar.Dead && !iRevivee.Avatar.Undying && float.IsNaN(iRevivee.Avatar.UndyingTimer)) && (double) iRevivee.DeadAge > (double) num1)
      {
        if (iReviver != null && ruleSet != null && !ruleSet.CanRevive(iReviver, iRevivee))
        {
          this.mIDSpecific = false;
          AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL);
          Revive.sCache.Add(this);
          return false;
        }
        num2 = this.mPlayerID;
        this.mRevivee = iRevivee;
      }
    }
    else
    {
      for (int index = 0; index < players.Length; ++index)
      {
        Magicka.GameLogic.Player iRevivee = players[index];
        if (iRevivee.Playing && !iRevivee.Ressing && (iRevivee.Avatar == null || iRevivee.Avatar.Overkilled || iRevivee.Avatar.Dead && !iRevivee.Avatar.Undying && float.IsNaN(iRevivee.Avatar.UndyingTimer)) && (double) iRevivee.DeadAge > (double) num1 && (iReviver == null || ruleSet == null || ruleSet.CanRevive(iReviver, iRevivee)))
        {
          num2 = index;
          num1 = iRevivee.DeadAge;
          this.mRevivee = iRevivee;
        }
      }
    }
    if (num2 == -1)
    {
      this.mIDSpecific = false;
      AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL);
      Revive.sCache.Add(this);
      return false;
    }
    this.mRevivee.Ressing = true;
    this.mTypeID = this.mRevivee.Gamer.Avatar.Type;
    if (this.mOwner != null)
      AudioManager.Instance.PlayCue(Banks.Spells, Revive.SOUND_REVIVE_HASH, this.mOwner.AudioEmitter);
    else
      AudioManager.Instance.PlayCue(Banks.Spells, Revive.SOUND_REVIVE_HASH);
    Vector3 oPoint;
    double nearestPosition = (double) this.mPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref iPosition, out oPoint, MovementProperties.Default);
    iPosition = oPoint;
    this.mPosition = iPosition;
    this.mDead = false;
    this.mRessed = false;
    this.mTimer = 0.0f;
    this.mSpotLight.AmbientColor = Vector3.Zero;
    this.mSpotLight.DiffuseColor = Vector3.Zero;
    this.mSpotLight.SpecularAmount = 0.0f;
    this.mSpotLight.Direction = Vector3.Down;
    this.mSpotLight.Sharpness = 1f;
    this.mSpotLight.UseAttenuation = true;
    this.mSpotLight.CutoffAngle = 0.4712389f;
    this.mSpotLight.Range = 13f;
    this.mSpotLight.Direction = Revive.GodRayRenderData.sRayRotation.Down;
    Vector3 result;
    Vector3.Add(ref this.mPosition, ref Revive.GodRayRenderData.sOffset1, out result);
    this.mSpotLight.Position = result;
    this.mSpotLight.Enable(this.mPlayState.Scene);
    for (int index = 0; index < 3; ++index)
    {
      Vector3.Add(ref this.mPosition, ref Revive.GodRayRenderData.sOffset1, out this.mRenderData[index].Position1);
      Vector3.Add(ref this.mPosition, ref Revive.GodRayRenderData.sOffset2, out this.mRenderData[index].Position2);
    }
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    return true;
  }

  public bool IsDead => this.mDead;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    Revive.GodRayRenderData iObject = this.mRenderData[(int) iDataChannel];
    float num = Math.Min(this.mTimer, 1.3f);
    iObject.Alpha = num;
    Vector3 vector3 = new Vector3(num * 10f);
    this.mSpotLight.AmbientColor = vector3;
    this.mSpotLight.DiffuseColor = vector3;
    this.mSpotLight.SpecularAmount = num;
    if (!this.mRessed)
    {
      this.mTimer += iDeltaTime * 4f;
      if ((double) this.mTimer > 1.0)
      {
        if (NetworkManager.Instance.State != NetworkState.Client && this.mRevivee.Playing && this.mRevivee.Gamer != null)
        {
          this.mRevivee.Weapon = (string) null;
          this.mRevivee.Staff = (string) null;
          Avatar fromCache = Avatar.GetFromCache(this.mRevivee);
          CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(this.mTypeID);
          fromCache.Initialize(cachedTemplate, this.mPosition, Magicka.GameLogic.Player.UNIQUE_ID[this.mRevivee.ID]);
          if (this.mPlayState.Level.CurrentScene.RuleSet is VersusRuleset)
            fromCache.Faction &= ~Factions.FRIENDLY;
          fromCache.HitPoints = fromCache.MaxHitPoints * this.mHealthPercentage;
          fromCache.SpawnAnimation = Magicka.Animations.revive;
          fromCache.ChangeState((BaseState) RessurectionState.Instance);
          this.mPlayState.EntityManager.AddEntity((Entity) fromCache);
          AudioManager.Instance.PlayCue(Banks.Spells, Revive.SOUNDHASH, fromCache.AudioEmitter);
          this.mRevivee.Avatar = fromCache;
          this.mRevivee.Ressing = false;
          if (fromCache.Player != null && fromCache.Player.Controller is XInputController)
            (fromCache.Player.Controller as XInputController).Rumble(2f, 2f);
          if (NetworkManager.Instance.State == NetworkState.Server)
            NetworkManager.Instance.Interface.SendMessage<SpawnPlayerMessage>(ref new SpawnPlayerMessage()
            {
              Handle = fromCache.Handle,
              Id = (byte) this.mRevivee.ID,
              MagickRevive = true,
              Position = fromCache.Position,
              Direction = fromCache.CharacterBody.DesiredDirection
            }, P2PSend.Reliable);
        }
        else if (NetworkManager.Instance.State == NetworkState.Client && this.mOwner is Avatar)
          (this.mOwner as Avatar).RequestForcedSyncingOfPlayers();
        this.mTimer = 3f;
        this.mRessed = true;
      }
    }
    else
    {
      this.mTimer -= iDeltaTime;
      if ((double) this.mTimer <= 0.0)
        this.mDead = true;
    }
    this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, (IRenderableAdditiveObject) iObject);
  }

  public void OnRemove()
  {
    this.mIDSpecific = false;
    this.mTypeID = Avatar.WIZARDHASH;
    this.mSpotLight.AmbientColor = Vector3.Zero;
    this.mSpotLight.DiffuseColor = Vector3.Zero;
    this.mSpotLight.SpecularAmount = 0.0f;
    this.mSpotLight.Sharpness = 1f;
    this.mSpotLight.UseAttenuation = true;
    this.mSpotLight.Direction = Vector3.Down;
    if (this.mRevivee != null)
      this.mRevivee.Ressing = false;
    this.mSpotLight.Disable();
    Revive.sCache.Add(this);
  }

  protected class GodRayRenderData : IRenderableAdditiveObject, IPreRenderRenderer
  {
    private static IndexBuffer sIndexBuffer;
    private static VertexBuffer sVertexBuffer;
    private static VertexDeclaration sVertexDeclaration;
    private static int VERTICEHASH;
    public static Vector3 sOffset1 = new Vector3(1.5f, 11f, 0.0f);
    public static Vector3 sOffset2 = new Vector3(2.05f, 9f, 0.75f);
    public static Matrix sRayRotation = Matrix.CreateFromYawPitchRoll(0.0f, 0.0f, -0.24f);
    public float Alpha;
    public Texture2D Texture;
    public Vector3 Position1;
    public Vector3 Position2;
    private BoundingSphere mBoundingSphere;
    private Vector4 mColorTint = new Vector4(1f, 1f, 1f, 0.0f);
    private Matrix mLookAt = Matrix.Identity;

    public GodRayRenderData()
    {
      if (Revive.GodRayRenderData.sVertexBuffer == null)
      {
        lock (Magicka.Game.Instance.GraphicsDevice)
        {
          Revive.GodRayRenderData.sIndexBuffer = new IndexBuffer(Magicka.Game.Instance.GraphicsDevice, 2 * Revive.INDICES.Length, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
          Revive.GodRayRenderData.sIndexBuffer.SetData<ushort>(Revive.INDICES);
          Revive.GodRayRenderData.sVertexBuffer = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, Revive.VERTEXSTRIDE * Revive.QUAD.Length, BufferUsage.WriteOnly);
          Revive.GodRayRenderData.sVertexBuffer.SetData<VertexPositionTexture>(Revive.QUAD);
          Revive.GodRayRenderData.sVertexDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
        }
        Revive.GodRayRenderData.VERTICEHASH = Revive.GodRayRenderData.sVertexBuffer.GetHashCode();
      }
      this.mBoundingSphere = new BoundingSphere();
      this.mBoundingSphere.Radius = 30f;
    }

    public int Effect => AdditiveEffect.TYPEHASH;

    public int Technique => 0;

    public VertexBuffer Vertices => Revive.GodRayRenderData.sVertexBuffer;

    public int VerticesHashCode => Revive.GodRayRenderData.VERTICEHASH;

    public int VertexStride => Revive.VERTEXSTRIDE;

    public IndexBuffer Indices => Revive.GodRayRenderData.sIndexBuffer;

    public VertexDeclaration VertexDeclaration => Revive.GodRayRenderData.sVertexDeclaration;

    public bool Cull(BoundingFrustum iViewFrustum)
    {
      this.mBoundingSphere.Center = this.Position1;
      return this.mBoundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
    }

    public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      AdditiveEffect additiveEffect = iEffect as AdditiveEffect;
      this.mLookAt.Translation = this.Position1;
      additiveEffect.World = this.mLookAt;
      this.mColorTint.W = this.Alpha * 0.5f;
      additiveEffect.ColorTint = this.mColorTint;
      additiveEffect.TextureOffset = Vector2.Zero;
      additiveEffect.TextureScale = Vector2.One;
      additiveEffect.Texture = this.Texture;
      additiveEffect.TextureEnabled = true;
      additiveEffect.VertexColorEnabled = false;
      additiveEffect.CommitChanges();
      additiveEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
      this.mColorTint.W = this.Alpha * 0.35f;
      additiveEffect.ColorTint = this.mColorTint;
      this.mLookAt.Translation = this.Position2;
      additiveEffect.World = this.mLookAt;
      additiveEffect.CommitChanges();
      additiveEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
    }

    public void PreRenderUpdate(
      DataChannel iDataChannel,
      float iDeltaTime,
      ref Matrix iViewProjectionMatrix,
      ref Vector3 iCameraPosition,
      ref Vector3 iCameraDirection)
    {
      Vector3 result1 = Vector3.Up;
      Vector3 position1 = this.Position1;
      Vector3 result2;
      Vector3.Subtract(ref iCameraPosition, ref position1, out result2);
      result2.Normalize();
      Vector3 result3;
      Vector3.Cross(ref result1, ref result2, out result3);
      Vector3.Cross(ref result2, ref result3, out result1);
      this.mLookAt.Forward = result2;
      this.mLookAt.Right = result3;
      this.mLookAt.Up = result1;
      Matrix.Multiply(ref Revive.GodRayRenderData.sRayRotation, ref this.mLookAt, out this.mLookAt);
      this.mLookAt.M11 *= 6.5f;
      this.mLookAt.M12 *= 6.5f;
      this.mLookAt.M13 *= 6.5f;
      this.mLookAt.M21 *= 17f;
      this.mLookAt.M22 *= 17f;
      this.mLookAt.M23 *= 17f;
    }
  }
}
