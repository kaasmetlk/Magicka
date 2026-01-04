// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Spells.Railgun
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.Achievements;
using Magicka.Audio;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.GameStates;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Graphics.Effects;
using Magicka.Graphics.Lights;
using Magicka.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Spells;

internal class Railgun : IAbilityEffect
{
  private const float EXPLODE_COOLDOWN = 0.25f;
  private const int MAX_RAY_DEPTH = 6;
  public const int VERTEXCOUNT = 512 /*0x0200*/;
  public const int PRIMITIVECOUNT = 510;
  private const float SPEED = 30f;
  private static List<Railgun> sCache;
  private static List<Railgun> sActiveRails;
  private static readonly int ARCANEHITEFFECTHASH = "arcane_hit".GetHashCodeCustom();
  private static readonly int ARCANESOURCEEFFECTHASH = "arcane_source".GetHashCodeCustom();
  private static readonly int LIFEHITEFFECTHASH = "life_hit".GetHashCodeCustom();
  private static readonly int LIFESOURCEEFFECTHASH = "life_source".GetHashCodeCustom();
  public static readonly int[] LIFESTAGESOUNDSHASH = new int[4]
  {
    "spell_life_ray_stage1".GetHashCodeCustom(),
    "spell_life_ray_stage2".GetHashCodeCustom(),
    "spell_life_ray_stage3".GetHashCodeCustom(),
    "spell_life_ray_stage4".GetHashCodeCustom()
  };
  public static readonly int[] ARCANESTAGESOUNDSHASH = new int[4]
  {
    "spell_arcane_ray_stage1".GetHashCodeCustom(),
    "spell_arcane_ray_stage2".GetHashCodeCustom(),
    "spell_arcane_ray_stage3".GetHashCodeCustom(),
    "spell_arcane_ray_stage4".GetHashCodeCustom()
  };
  private static Random sRandom = new Random();
  private static AudioEmitter sEmitter = new AudioEmitter();
  private static Texture2D sTexture;
  private static VertexBuffer sVertexBuffer;
  private static int sVertexBufferHash;
  private static VertexDeclaration sVertexDeclaration;
  private static VertexBuffer sAdditionalVertexBuffer;
  private static int sAdditionalVertexBufferHash;
  private static VertexDeclaration sAdditionalVertexDeclaration;
  public static readonly Vector2[] ELEMENT_OFFSET_LOOKUP = new Vector2[11]
  {
    new Vector2(),
    new Vector2(0.0f, 0.0f),
    new Vector2(0.0f, 0.125f),
    new Vector2(0.0f, 0.25f),
    new Vector2(0.0f, 0.375f),
    new Vector2(0.0f, 0.5f),
    new Vector2(0.0f, 0.625f),
    new Vector2(),
    new Vector2(),
    new Vector2(0.0f, 0.75f),
    new Vector2(0.0f, 0.875f)
  };
  private float mTime;
  private float mLength;
  private Railgun.RenderData[] mRenderData;
  private Railgun.AdditionalRenderData[] mAdditionalRenderData;
  private Vector3 mPosition;
  private Vector3 mDirection;
  private Railgun mChild;
  private List<Railgun> mParents = new List<Railgun>(8);
  private Vector3 mColor;
  private bool mDead;
  private float mCut;
  private VisualEffectReference mSourceEffect;
  private VisualEffectReference mHitEffect;
  private SortedList<ushort, DamageCollection5> mDamages = new SortedList<ushort, DamageCollection5>(4);
  private bool mArcane;
  private Cue mStageStartCue;
  private Cue mStageHitCue;
  private List<Cue> mAdditionalElementCues;
  private float mDamageTimer;
  private int mHitSoundHash;
  private CapsuleLight mLight;
  private Elements mAdditionalElements;
  private Elements mAllCombinedElements;
  private bool mImmaFirinMahLazer;
  private bool mIsOposits;
  private bool mWillExplode;
  private bool mLocked;
  private Railgun mExplosionCompanion;
  private float mExplosionCountdown;
  private Vector3 mExplosionPosition;
  private double mTimeStamp;
  private int mDepth;
  private Portal.PortalEntity mOutPortal;

  public static void InitializeCache(int iSize)
  {
    Railgun.sCache = new List<Railgun>(iSize);
    Railgun.sActiveRails = new List<Railgun>(iSize);
    for (int index = 0; index < iSize; ++index)
      Railgun.sCache.Add(new Railgun());
  }

  public static Railgun GetFromCache()
  {
    if (Railgun.sCache.Count <= 0)
      return new Railgun();
    Railgun fromCache = Railgun.sCache[Railgun.sCache.Count - 1];
    Railgun.sCache.RemoveAt(Railgun.sCache.Count - 1);
    return fromCache;
  }

  private Railgun()
  {
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    if (!(RenderManager.Instance.GetEffect(ArcaneEffect.TYPEHASH) is ArcaneEffect iEffect))
    {
      lock (graphicsDevice)
        iEffect = new ArcaneEffect(graphicsDevice, Magicka.Game.Instance.Content);
      RenderManager.Instance.RegisterEffect((Effect) iEffect);
    }
    this.mAdditionalElementCues = new List<Cue>(8);
    if (Railgun.sVertexBuffer == null)
    {
      Vector4[] data1 = new Vector4[512 /*0x0200*/];
      for (int index = 0; index < 256 /*0x0100*/; ++index)
      {
        float num1 = (float) index / (float) byte.MaxValue;
        data1[index * 2].Z = (float) index / 8f;
        data1[index * 2 + 1].Z = (float) index / 8f;
        float num2 = num1 * num1;
        data1[index * 2].X = num2;
        data1[index * 2 + 1].X = num2;
        data1[index * 2].Y = 1f;
        data1[index * 2 + 1].Y = -1f;
        data1[index * 2].W = 0.5f;
        data1[index * 2 + 1].W = 0.625f;
      }
      lock (graphicsDevice)
      {
        Railgun.sTexture = Magicka.Game.Instance.Content.Load<Texture2D>("EffectTextures/Beams");
        Railgun.sVertexBuffer = new VertexBuffer(graphicsDevice, data1.Length * 4 * 4, BufferUsage.WriteOnly);
        Railgun.sVertexBuffer.SetData<Vector4>(data1);
        Railgun.sVertexDeclaration = new VertexDeclaration(graphicsDevice, new VertexElement[3]
        {
          new VertexElement((short) 0, (short) 0, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
          new VertexElement((short) 0, (short) 4, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.Normal, (byte) 0),
          new VertexElement((short) 0, (short) 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 0)
        });
      }
      Railgun.sVertexBufferHash = Railgun.sVertexBuffer.GetHashCode();
      Vector4[] data2 = new Vector4[4];
      data2[0].X = 1f;
      data2[0].Y = 1f;
      data2[0].Z = 1f;
      data2[0].W = 0.125f;
      data2[1].X = -1f;
      data2[1].Y = 1f;
      data2[1].Z = 1f;
      data2[1].W = 0.0f;
      data2[2].X = -1f;
      data2[2].Y = 0.0f;
      data2[2].Z = 0.0f;
      data2[2].W = 0.0f;
      data2[3].X = 1f;
      data2[3].Y = 0.0f;
      data2[3].Z = 0.0f;
      data2[3].W = 0.125f;
      lock (graphicsDevice)
      {
        Railgun.sAdditionalVertexBuffer = new VertexBuffer(graphicsDevice, 64 /*0x40*/, BufferUsage.WriteOnly);
        Railgun.sAdditionalVertexBuffer.SetData<Vector4>(data2);
        Railgun.sAdditionalVertexDeclaration = new VertexDeclaration(graphicsDevice, new VertexElement[2]
        {
          new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
          new VertexElement((short) 0, (short) 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 0)
        });
      }
      Railgun.sAdditionalVertexBufferHash = Railgun.sAdditionalVertexBuffer.GetHashCode();
    }
    this.mLight = new CapsuleLight(Magicka.Game.Instance.Content);
    this.mRenderData = new Railgun.RenderData[3];
    this.mAdditionalRenderData = new Railgun.AdditionalRenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      Railgun.RenderData renderData = new Railgun.RenderData();
      this.mRenderData[index] = renderData;
      Railgun.AdditionalRenderData additionalRenderData = new Railgun.AdditionalRenderData();
      this.mAdditionalRenderData[index] = additionalRenderData;
    }
  }

  public void Initialize(
    ISpellCaster iOwner,
    Vector3 iPosition,
    Vector3 iDirection,
    Vector3 iColor,
    ref DamageCollection5 iDamages,
    ref Spell iSpell)
  {
    this.mOutPortal = (Portal.PortalEntity) null;
    this.mDepth = 0;
    this.mAdditionalElementCues.Clear();
    this.mTime = 0.0f;
    this.mLength = 0.0f;
    this.mDamageTimer = 0.25f;
    this.mCut = 0.0f;
    this.mImmaFirinMahLazer = false;
    this.mIsOposits = false;
    this.mWillExplode = false;
    this.mLocked = false;
    this.mChild = (Railgun) null;
    this.mParents.Clear();
    this.mTimeStamp = iOwner.PlayState.PlayTime;
    Elements element = iSpell.Element;
    this.mArcane = (element & Elements.Arcane) == Elements.Arcane;
    this.mAdditionalElements = element & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Lightning | Elements.Steam | Elements.Poison);
    this.mAllCombinedElements = iSpell.Element;
    int iHash1;
    int iHash2;
    if (this.mArcane)
    {
      iHash1 = Railgun.ARCANESOURCEEFFECTHASH;
      iHash2 = Railgun.ARCANEHITEFFECTHASH;
      this.mHitSoundHash = Railgun.ARCANESTAGESOUNDSHASH[1];
    }
    else
    {
      iHash1 = Railgun.LIFESOURCEEFFECTHASH;
      iHash2 = Railgun.LIFEHITEFFECTHASH;
      this.mHitSoundHash = Railgun.LIFESTAGESOUNDSHASH[1];
    }
    EffectManager.Instance.StartEffect(iHash1, ref iPosition, ref iDirection, out this.mSourceEffect);
    EffectManager.Instance.StartEffect(iHash2, ref iPosition, ref iDirection, out this.mHitEffect);
    this.mDamages.Clear();
    this.mDamages.Add(iOwner.Handle, iDamages);
    this.mColor = iColor;
    for (int index = 0; index < 3; ++index)
    {
      Railgun.RenderData renderData = this.mRenderData[index];
      renderData.ColorCenter.X = this.mColor.X * 0.666f;
      renderData.ColorCenter.Y = this.mColor.Y * 0.666f;
      renderData.ColorCenter.Z = this.mColor.Z * 0.666f;
      renderData.ColorEdge.X = this.mColor.X * 0.333f;
      renderData.ColorEdge.Y = this.mColor.Y * 0.333f;
      renderData.ColorEdge.Z = this.mColor.Z * 0.333f;
    }
    Railgun.sEmitter.Position = iPosition;
    Railgun.sEmitter.Forward = iDirection;
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    this.mStageStartCue = AudioManager.Instance.PlayCue(Banks.Spells, this.mArcane ? Railgun.ARCANESTAGESOUNDSHASH[0] : Railgun.LIFESTAGESOUNDSHASH[0], Railgun.sEmitter);
    Spell oSpell;
    Spell.DefaultSpell(this.mAdditionalElements, out oSpell);
    foreach (Cue cue in oSpell.PlaySound(SpellType.Spray, CastType.Force))
    {
      if (cue != null)
        this.mAdditionalElementCues.Add(cue);
      cue.Apply3D(AudioManager.Instance.getListener(), Railgun.sEmitter);
      cue.Play();
    }
    this.mDead = false;
    Railgun.sEmitter.Up = Vector3.Up;
    this.mLight.Start = this.mPosition;
    this.mLight.End = this.mPosition;
    this.mLight.DiffuseColor = iColor * 1f;
    this.mLight.AmbientColor = iColor * 0.0f;
    this.mLight.Radius = 5f;
    this.mLight.Intensity = 0.6f;
    this.mLight.Enable(iOwner.PlayState.Scene);
    Railgun.sActiveRails.Add(this);
  }

  private void Initialize(
    Railgun iParentA,
    Railgun iParentB,
    ref Vector3 iPosition,
    ref Vector3 iDirection,
    int iDepth,
    Portal.PortalEntity iOutPortal)
  {
    this.mOutPortal = iOutPortal;
    this.mDepth = iDepth;
    this.mTime = 0.0f;
    this.mLength = 0.0f;
    this.mDamageTimer = 0.25f;
    this.mCut = 0.0f;
    this.mIsOposits = false;
    this.mWillExplode = false;
    this.mLocked = false;
    this.mTimeStamp = iParentA.mTimeStamp;
    this.mChild = (Railgun) null;
    this.mParents.Clear();
    this.mParents.Add(iParentA);
    if (iParentB != null)
      this.mParents.Add(iParentB);
    iParentA.mChild = this;
    this.mArcane = iParentA.mArcane;
    this.mAdditionalElements = iParentA.mAdditionalElements;
    this.mAllCombinedElements = iParentA.mAllCombinedElements;
    int iHash;
    if (this.mArcane)
    {
      iHash = Railgun.ARCANEHITEFFECTHASH;
      this.mHitSoundHash = Railgun.ARCANESTAGESOUNDSHASH[1];
    }
    else
    {
      iHash = Railgun.LIFEHITEFFECTHASH;
      this.mHitSoundHash = Railgun.LIFESTAGESOUNDSHASH[1];
    }
    EffectManager.Instance.StartEffect(iHash, ref iPosition, ref iDirection, out this.mHitEffect);
    this.mDamages.Clear();
    for (int index = 0; index < iParentA.mDamages.Count; ++index)
    {
      if (!this.mDamages.ContainsKey(iParentA.mDamages.Keys[index]))
        this.mDamages.Add(iParentA.mDamages.Keys[index], iParentA.mDamages.Values[index]);
    }
    this.mColor = iParentA.mColor;
    if (iParentB != null)
    {
      iParentB.mChild = this;
      this.mAdditionalElements |= iParentB.mAdditionalElements;
      this.mAllCombinedElements |= iParentB.mAllCombinedElements;
      for (int index = 0; index < iParentB.mDamages.Count; ++index)
      {
        if (!this.mDamages.ContainsKey(iParentB.mDamages.Keys[index]))
          this.mDamages.Add(iParentB.mDamages.Keys[index], iParentB.mDamages.Values[index]);
      }
      Vector3.Lerp(ref this.mColor, ref iParentB.mColor, 0.5f, out this.mColor);
      this.mImmaFirinMahLazer = true;
    }
    for (int index = 0; index < 3; ++index)
    {
      Railgun.RenderData renderData = this.mRenderData[index];
      renderData.ColorCenter.X = this.mColor.X * 0.666f;
      renderData.ColorCenter.Y = this.mColor.Y * 0.666f;
      renderData.ColorCenter.Z = this.mColor.Z * 0.666f;
      renderData.ColorEdge.X = this.mColor.X * 0.333f;
      renderData.ColorEdge.Y = this.mColor.Y * 0.333f;
      renderData.ColorEdge.Z = this.mColor.Z * 0.333f;
    }
    Railgun.sEmitter.Position = iPosition;
    Railgun.sEmitter.Forward = iDirection;
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    this.mStageStartCue = AudioManager.Instance.PlayCue(Banks.Spells, this.mArcane ? Railgun.ARCANESTAGESOUNDSHASH[0] : Railgun.LIFESTAGESOUNDSHASH[0], Railgun.sEmitter);
    Spell oSpell;
    Spell.DefaultSpell(this.mAdditionalElements, out oSpell);
    foreach (Cue cue in oSpell.PlaySound(SpellType.Spray, CastType.Force))
    {
      if (cue != null)
        this.mAdditionalElementCues.Add(cue);
      cue.Apply3D(AudioManager.Instance.getListener(), Railgun.sEmitter);
      cue.Play();
    }
    this.mDead = false;
    Railgun.sEmitter.Up = Vector3.Up;
    this.mLight.Start = this.mPosition;
    this.mLight.End = this.mPosition;
    this.mLight.DiffuseColor = this.mColor;
    this.mLight.AmbientColor = this.mColor * 0.5f;
    this.mLight.Radius = 5f;
    this.mLight.Intensity = 0.6f;
    this.mLight.Enable(Entity.GetFromHandle((int) this.mDamages.Keys[0]).PlayState.Scene);
    Railgun.sActiveRails.Add(this);
  }

  internal void Kill()
  {
    this.mDead = true;
    EffectManager.Instance.Stop(ref this.mSourceEffect);
  }

  public Vector3 Position
  {
    get => this.mPosition;
    set
    {
      if (this.mLocked)
        return;
      this.mPosition = value;
    }
  }

  public Vector3 Direction
  {
    get => this.mDirection;
    set
    {
      if (this.mLocked)
        return;
      this.mDirection = value;
    }
  }

  public bool IsDead => this.mDead;

  public unsafe void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if (this.IsDead)
      return;
    bool flag1 = this.mParents.Count > 0;
    for (int index = 0; index < this.mParents.Count; ++index)
    {
      if (!this.mParents[index].mDead)
      {
        flag1 = false;
        break;
      }
    }
    if (flag1)
      this.Kill();
    if (this.mParents.Count > 1)
    {
      Vector3 result1 = new Vector3();
      Vector3 result2 = new Vector3();
      for (int index = 0; index < this.mParents.Count; ++index)
      {
        Vector3 result3;
        Vector3.Multiply(ref this.mParents[index].mDirection, this.mParents[index].mLength, out result3);
        Vector3.Add(ref this.mParents[index].mPosition, ref result3, out result3);
        Vector3.Add(ref result1, ref result3, out result1);
        Vector3.Add(ref result2, ref this.mParents[index].mDirection, out result2);
      }
      Vector3.Divide(ref result1, (float) this.mParents.Count, out this.mPosition);
      Vector3.Normalize(ref result2, out this.mDirection);
    }
    this.mTime += iDeltaTime * 30f;
    PlayState playState = Entity.GetFromHandle((int) this.mDamages.Keys[0]).PlayState;
    if (this.mWillExplode)
    {
      this.mImmaFirinMahLazer = false;
      this.mExplosionCountdown -= iDeltaTime;
      if ((double) this.mExplosionCountdown <= 0.0)
        this.Explode(playState);
    }
    else if (!this.mLocked)
    {
      if (!(this.mDead & this.mChild != null))
        this.mLength = Math.Min(this.mLength + (float) ((double) iDeltaTime * 30.0 * 3.0), 75f);
      Segment iSeg;
      iSeg.Origin = this.mPosition;
      Vector3.Multiply(ref this.mDirection, this.mLength, out iSeg.Delta);
      Segment oSeg;
      MagickaMath.SegmentNegate(ref iSeg, out oSeg);
      List<Entity> entities = playState.EntityManager.GetEntities(this.mPosition, this.mLength * 1.25f, false);
      float d = float.MaxValue;
      Entity entity1 = (Entity) null;
      Vector3 iAttackPosition = this.mPosition;
      Vector3 vector3_1;
      float scaleFactor;
      Vector3 vector3_2;
      for (int index = 0; index < entities.Count; ++index)
      {
        Entity entity2 = entities[index];
        switch (entity2)
        {
          case IDamageable _:
label_22:
            if (entity2 != this.mOutPortal && ((this.mDamages.Count == 1 & (int) this.mDamages.Keys[0] == (int) entity2.Handle | entity2 is Grease.GreaseField ? 1 : 0) | (!(entity2 is Barrier) ? 0 : (!(entity2 as Barrier).Solid ? 1 : 0))) == 0)
            {
              IDamageable damageable = entity2 as IDamageable;
              if (!(damageable is CthulhuMist) || !(damageable as CthulhuMist).IgnoreElements(this.mAllCombinedElements))
              {
                Portal.PortalEntity portalEntity = entity2 as Portal.PortalEntity;
                if (damageable != null && damageable.SegmentIntersect(out vector3_1, iSeg, 0.25f) || portalEntity != null && portalEntity.Body.CollisionSkin.SegmentIntersect(out scaleFactor, out vector3_1, out vector3_2, iSeg))
                {
                  float result;
                  Vector3.DistanceSquared(ref vector3_1, ref this.mPosition, out result);
                  if ((double) result < (double) d)
                  {
                    if ((double) result > 9.9999999747524271E-07)
                    {
                      d = result;
                      entity1 = entity2;
                      iAttackPosition = vector3_1;
                      break;
                    }
                    if (damageable != null && damageable.SegmentIntersect(out vector3_1, iSeg, 0.25f) || portalEntity != null && portalEntity.Body.CollisionSkin.SegmentIntersect(out scaleFactor, out vector3_1, out vector3_2, iSeg))
                    {
                      Vector3.DistanceSquared(ref vector3_1, ref this.mPosition, out result);
                      if ((double) result > 9.9999999747524271E-07 & (double) result < (double) d)
                      {
                        d = result;
                        entity1 = entity2;
                        iAttackPosition = vector3_1;
                        break;
                      }
                      break;
                    }
                    break;
                  }
                  break;
                }
                break;
              }
              break;
            }
            break;
          case Portal.PortalEntity _:
            if (!Portal.Instance.Connected)
              break;
            goto label_22;
        }
      }
      playState.EntityManager.ReturnEntityList(entities);
      if (entity1 != null)
        this.mLength = (float) Math.Sqrt((double) d);
      else
        this.mDamageTimer = 0.25f;
      bool flag2 = entity1 != null;
      Vector3.Multiply(ref this.mDirection, this.mLength, out iSeg.Delta);
      Vector3 iPosition1 = new Vector3();
      AnimatedLevelPart oAnimatedLevelPart;
      int oPrim;
      float result4;
      if (playState.Level.CurrentScene.SegmentIntersect(out scaleFactor, out vector3_1, out vector3_2, out oAnimatedLevelPart, out oPrim, iSeg))
      {
        Vector3.Dot(ref vector3_2, ref this.mDirection, out result4);
        if ((double) result4 > 0.0)
        {
          Vector3 result5;
          Vector3.Multiply(ref this.mDirection, 0.1f, out result5);
          Vector3.Add(ref iSeg.Origin, ref result5, out iSeg.Origin);
          Vector3.Subtract(ref iSeg.Delta, ref result5, out iSeg.Delta);
          if (playState.Level.CurrentScene.SegmentIntersect(out scaleFactor, out vector3_1, out vector3_2, out oAnimatedLevelPart, out oPrim, iSeg))
          {
            flag2 = true;
            this.mLength *= scaleFactor;
            Vector3.Multiply(ref iSeg.Delta, scaleFactor, out iSeg.Delta);
            Vector3.Add(ref this.mPosition, ref iSeg.Delta, out iPosition1);
            entity1 = (Entity) null;
          }
        }
        else
        {
          flag2 = true;
          this.mLength *= scaleFactor;
          Vector3.Multiply(ref iSeg.Delta, scaleFactor, out iSeg.Delta);
          Vector3.Add(ref this.mPosition, ref iSeg.Delta, out iPosition1);
          entity1 = (Entity) null;
        }
      }
      Vector3 iPosition2 = vector3_1;
      Vector3 normal = vector3_2;
      CollisionMaterials collisionMaterials = (CollisionMaterials) oPrim;
      if (oAnimatedLevelPart != null)
        collisionMaterials = oAnimatedLevelPart.CollisionMaterial;
      bool flag3 = flag2 && (double) Vector3.Dot(normal, this.mDirection) < 0.0 && collisionMaterials == CollisionMaterials.Reflect && (double) normal.LengthSquared() > 9.9999999747524271E-07;
      if (!this.mDead)
      {
        Railgun iParentB = (Railgun) null;
        bool flag4 = false;
        for (int index = 0; index < Railgun.sActiveRails.Count; ++index)
        {
          Railgun sActiveRail = Railgun.sActiveRails[index];
          if (!(sActiveRail == this | this.mParents.Contains(sActiveRail) | sActiveRail == this.mChild | sActiveRail.mParents.Contains(this) | sActiveRail.mDead))
          {
            Segment seg1 = new Segment();
            seg1.Origin = sActiveRail.mPosition;
            Vector3.Multiply(ref sActiveRail.mDirection, sActiveRail.mLength, out seg1.Delta);
            float t0;
            float t1;
            if ((double) Distance.SegmentSegmentDistanceSq(out t0, out t1, iSeg, seg1) <= 1.0 & (double) t0 > 9.9999999747524271E-07)
            {
              float num1 = flag4 ? (this.mLength - 1f) / this.mLength : 1f;
              float num2 = 1f / this.mLength;
              if ((double) t0 <= (double) num1 & (double) t1 >= (double) num2)
              {
                entity1 = (Entity) null;
                flag4 = (double) t1 * (double) sActiveRail.mLength < 1.0;
                flag2 = false;
                this.mLength *= t0;
                Vector3.Multiply(ref iSeg.Delta, t0, out iSeg.Delta);
                iParentB = sActiveRail;
              }
            }
          }
        }
        if (iParentB != null)
        {
          bool flag5 = false;
          if (!flag4)
          {
            for (int index = 0; index < this.mDamages.Count; ++index)
            {
              if (iParentB.mDamages.ContainsKey(this.mDamages.Keys[index]))
                flag5 = true;
            }
          }
          bool flag6 = flag5 | SpellManager.InclusiveOpposites(this.mAdditionalElements, iParentB.mAdditionalElements) | this.mArcane != iParentB.mArcane;
          Vector3.Add(ref this.mPosition, ref iSeg.Delta, out iPosition1);
          if (flag6)
          {
            this.mIsOposits = true;
            this.mWillExplode = true;
            this.mExplosionCompanion = iParentB;
            this.mExplosionCountdown = 0.25f;
            this.mExplosionPosition = iPosition1;
            Vector3.Distance(ref this.mPosition, ref iPosition1, out this.mLength);
            Railgun railgun = this;
            while (railgun.mChild != null)
              railgun = railgun.mChild;
            railgun.LockAll();
          }
          else
          {
            if (iParentB.mChild != this.mChild & iParentB != this.mChild)
            {
              if (this.mChild != null)
                this.mChild.Kill();
              if (iParentB.mChild != null && iParentB.mChild.mParents.Count <= 2)
                iParentB.mChild.Kill();
            }
            if (flag4 & this.mChild == null && !iParentB.mParents.Contains(this))
            {
              for (int index = 0; index < this.mDamages.Count; ++index)
              {
                ushort key = this.mDamages.Keys[index];
                if (iParentB.mDamages.ContainsKey(key))
                {
                  this.mWillExplode = true;
                  this.mExplosionCompanion = iParentB;
                  this.mExplosionCountdown = 0.25f;
                  this.mExplosionPosition = iPosition1;
                  Vector3.Distance(ref this.mPosition, ref iPosition1, out this.mLength);
                  Railgun railgun = iParentB;
                  while (railgun.mChild != null)
                    railgun = railgun.mChild;
                  railgun.LockAll();
                  break;
                }
                iParentB.mDamages.Add(key, this.mDamages.Values[index]);
              }
              if (!this.mWillExplode)
              {
                this.mChild = iParentB;
                iParentB.mParents.Add(this);
              }
            }
            Vector3 result6;
            Vector3.Lerp(ref this.mDirection, ref iParentB.mDirection, 0.5f, out result6);
            if ((double) result6.Length() < 0.20000000298023224)
            {
              this.mWillExplode = true;
              this.mExplosionCompanion = iParentB;
              this.mExplosionCountdown = 0.25f;
              this.mExplosionPosition = iPosition1;
              Railgun railgun = this;
              while (railgun.mChild != null)
                railgun = railgun.mChild;
              railgun.LockAll();
            }
            else if (this.mChild == null)
            {
              result6.Normalize();
              int num = Math.Max(this.mDepth, iParentB.mDepth);
              if (num < 6)
                Railgun.GetFromCache().Initialize(this, iParentB, ref iPosition1, ref result6, num + 1, (Portal.PortalEntity) null);
            }
          }
        }
        else
        {
          int num3 = this.mChild != null ? 1 : 0;
          int num4;
          switch (entity1)
          {
            case Shield _:
            case Portal.PortalEntity _:
label_90:
              num4 = 0;
              break;
            case Magicka.GameLogic.Entities.Character _:
              if ((entity1 as Magicka.GameLogic.Entities.Character).CurrentSelfShieldType == Magicka.GameLogic.Entities.Character.SelfShieldType.Shield)
                goto label_90;
              goto default;
            default:
              num4 = !flag3 ? 1 : 0;
              break;
          }
          if ((num3 & num4) != 0)
            this.mChild.Kill();
        }
      }
      if (entity1 != null)
      {
        Vector3.Multiply(ref this.mDirection, this.mLength, out iPosition1);
        Vector3.Add(ref this.mPosition, ref iPosition1, out iPosition1);
        if (!entity1.Dead)
        {
          switch (entity1)
          {
            case Shield _:
              this.mDamageTimer = 0.25f;
              Shield shield = entity1 as Shield;
              Vector3.Multiply(ref this.mDirection, this.mLength + 1f, out iSeg.Delta);
              float frac;
              if (shield.Body.CollisionSkin.SegmentIntersect(out frac, out iPosition1, out vector3_2, iSeg))
              {
                if ((double) frac <= 9.9999999747524271E-07)
                  shield.Body.CollisionSkin.SegmentIntersect(out frac, out iPosition1, out vector3_2, oSeg);
                vector3_2.Y = 0.0f;
                Vector3 result7;
                Vector3.Reflect(ref this.mDirection, ref vector3_2, out result7);
                result7.Normalize();
                Vector3.Dot(ref result7, ref this.mDirection, out result4);
                if ((double) result4 < -0.99000000953674316)
                {
                  this.mWillExplode = true;
                  this.mExplosionCompanion = (Railgun) null;
                  this.mExplosionCountdown = 0.25f;
                  this.GetFirstJunction(out this.mExplosionPosition);
                  Railgun railgun = this;
                  while (railgun.mChild != null)
                    railgun = railgun.mChild;
                  railgun.LockAll();
                  break;
                }
                if (this.mChild != null && this.mChild.mParents.Count > 1)
                {
                  this.mChild.Kill();
                  this.mChild = (Railgun) null;
                }
                if (this.mChild == null && this.mDepth < 6)
                  Railgun.GetFromCache().Initialize(this, (Railgun) null, ref iPosition1, ref result7, this.mDepth + 1, (Portal.PortalEntity) null);
                if (this.mChild != null)
                {
                  this.mChild.Position = iPosition1;
                  this.mChild.Direction = result7;
                  break;
                }
                break;
              }
              if (this.mChild != null)
              {
                this.mChild.Kill();
                break;
              }
              break;
            case Magicka.GameLogic.Entities.Character _ when (entity1 as Magicka.GameLogic.Entities.Character).CurrentSelfShieldType == Magicka.GameLogic.Entities.Character.SelfShieldType.Shield:
              this.mDamageTimer = 0.25f;
              Magicka.GameLogic.Entities.Character character1 = entity1 as Magicka.GameLogic.Entities.Character;
              Vector3.Multiply(ref this.mDirection, this.mLength + 1f, out iSeg.Delta);
              float t;
              if (character1.Body.CollisionSkin.SegmentIntersect(out t, out iPosition1, out vector3_2, iSeg))
              {
                if ((double) t <= 9.9999999747524271E-07)
                  character1.Body.CollisionSkin.SegmentIntersect(out t, out iPosition1, out vector3_2, oSeg);
              }
              else
              {
                Vector3 position = character1.Position;
                double num = (double) Distance.PointSegmentDistanceSq(out t, position, iSeg);
                iSeg.GetPoint(t, out iPosition1);
                Vector3.Subtract(ref iPosition1, ref position, out vector3_2);
              }
              if ((double) t > 9.9999999747524271E-07)
              {
                vector3_2.Y = 0.0f;
                Vector3 result8;
                Vector3.Reflect(ref this.mDirection, ref vector3_2, out result8);
                result8.Normalize();
                Vector3.Dot(ref result8, ref this.mDirection, out result4);
                if ((double) result4 < -0.99000000953674316)
                {
                  this.mWillExplode = true;
                  this.mExplosionCompanion = (Railgun) null;
                  this.mExplosionCountdown = 0.25f;
                  this.GetFirstJunction(out this.mExplosionPosition);
                  Railgun railgun = this;
                  while (railgun.mChild != null)
                    railgun = railgun.mChild;
                  railgun.LockAll();
                  break;
                }
                if (this.mChild != null && this.mChild.mParents.Count > 1)
                {
                  this.mChild.Kill();
                  this.mChild = (Railgun) null;
                }
                if (this.mChild == null && this.mDepth < 6)
                  Railgun.GetFromCache().Initialize(this, (Railgun) null, ref iPosition1, ref result8, this.mDepth + 1, (Portal.PortalEntity) null);
                if (this.mChild != null)
                {
                  this.mChild.Position = iPosition1;
                  this.mChild.Direction = result8;
                  break;
                }
                break;
              }
              if (this.mChild != null)
              {
                this.mChild.Kill();
                break;
              }
              break;
            case Portal.PortalEntity _:
              this.mDamageTimer = 0.25f;
              Portal.PortalEntity iOutPortal = Portal.OtherPortal(entity1 as Portal.PortalEntity);
              Vector3 mDirection = this.mDirection;
              Vector3 result9 = entity1.Position;
              Vector3.Subtract(ref iPosition1, ref result9, out result9);
              Vector3 result10 = iOutPortal.Position;
              Vector3.Add(ref result10, ref result9, out result10);
              this.mLength += 0.5f;
              if (this.mChild != null && this.mChild.mParents.Count > 1)
              {
                this.mChild.Kill();
                this.mChild = (Railgun) null;
              }
              if (this.mChild == null && this.mDepth < 6)
                Railgun.GetFromCache().Initialize(this, (Railgun) null, ref result10, ref mDirection, this.mDepth + 1, iOutPortal);
              if (this.mChild != null)
              {
                this.mChild.Position = result10;
                this.mChild.Direction = mDirection;
                break;
              }
              break;
            default:
              if ((double) this.mDamageTimer <= 0.0 && entity1 is IDamageable)
              {
                this.mDamageTimer += 0.25f;
                for (int index1 = 0; index1 < this.mDamages.Count; ++index1)
                {
                  DamageCollection5 iDamage = this.mDamages.Values[index1];
                  iDamage.MultiplyMagnitude(0.25f);
                  Entity fromHandle = Entity.GetFromHandle((int) this.mDamages.Keys[index1]);
                  int num = (int) (entity1 as IDamageable).Damage(iDamage, fromHandle, this.mTimeStamp, iAttackPosition);
                  if (entity1 is Magicka.GameLogic.Entities.Character character2)
                  {
                    Damage* damagePtr = &iDamage.A;
                    for (int index2 = 0; index2 < 5; ++index2)
                    {
                      if ((damagePtr[index2].Element & Elements.Beams) != Elements.None)
                      {
                        character2.AccumulateArcaneDamage(damagePtr[index2].Element, damagePtr[index2].Magnitude);
                        if ((double) (entity1 as IDamageable).HitPoints <= 0.0 & !character2.Bloating)
                          character2.BloatKill(damagePtr[index2].Element, fromHandle);
                      }
                    }
                  }
                }
                break;
              }
              break;
          }
          this.mDamageTimer -= iDeltaTime;
        }
      }
      else if (flag3)
      {
        this.mDamageTimer = 0.25f;
        Vector3.Multiply(ref this.mDirection, this.mLength + 1f, out iSeg.Delta);
        Vector3 result11;
        Vector3.Reflect(ref this.mDirection, ref normal, out result11);
        result11.Normalize();
        Vector3.Dot(ref result11, ref this.mDirection, out result4);
        if ((double) result4 < -0.99000000953674316)
        {
          this.mWillExplode = true;
          this.mExplosionCompanion = (Railgun) null;
          this.mExplosionCountdown = 0.25f;
          this.GetFirstJunction(out this.mExplosionPosition);
          Railgun railgun = this;
          while (railgun.mChild != null)
            railgun = railgun.mChild;
          railgun.LockAll();
        }
        else
        {
          if (this.mChild != null && this.mChild.mParents.Count > 1)
          {
            this.mChild.Kill();
            this.mChild = (Railgun) null;
          }
          if (this.mChild == null && this.mDepth < 6)
            Railgun.GetFromCache().Initialize(this, (Railgun) null, ref iPosition2, ref result11, this.mDepth + 1, (Portal.PortalEntity) null);
          if (this.mChild != null)
          {
            this.mChild.Position = iPosition2;
            this.mChild.Direction = result11;
          }
        }
      }
      if (flag2)
      {
        Railgun.sEmitter.Position = iPosition1;
        Railgun.sEmitter.Forward = this.mDirection;
        if (this.mStageHitCue == null)
          this.mStageHitCue = AudioManager.Instance.PlayCue(Banks.Spells, this.mHitSoundHash, Railgun.sEmitter);
        else
          this.mStageHitCue.Apply3D(playState.Camera.Listener, Railgun.sEmitter);
      }
      else
      {
        Vector3.Multiply(ref this.mDirection, this.mLength, out iPosition1);
        Vector3.Add(ref this.mPosition, ref iPosition1, out iPosition1);
        if (this.mStageHitCue != null)
        {
          this.mStageHitCue.Stop(AudioStopOptions.AsAuthored);
          this.mStageHitCue = (Cue) null;
        }
      }
      if (this.mDead)
        this.mCut += (float) ((double) iDeltaTime / (double) this.mLength * 30.0 * 3.0);
      EffectManager.Instance.UpdatePositionDirection(ref this.mSourceEffect, ref this.mPosition, ref this.mDirection);
      EffectManager.Instance.UpdatePositionDirection(ref this.mHitEffect, ref iPosition1, ref this.mDirection);
      Vector3 result12;
      Vector3.Lerp(ref this.mPosition, ref iPosition1, this.mCut, out result12);
      this.mLight.Start = result12;
      this.mLight.End = iPosition1;
    }
    if (this.mAdditionalElements == Elements.None)
    {
      Railgun.RenderData iObject = this.mRenderData[(int) iDataChannel];
      if (this.mParents.Count > 0)
      {
        iObject.Nr = 2;
        for (int index = 0; index < this.mDamages.Count; ++index)
          iObject.Nr += 2;
      }
      else
        iObject.Nr = 4;
      iObject.Time = this.mTime;
      iObject.Cut = this.mCut;
      iObject.Position = this.mPosition;
      iObject.Direction = this.mDirection;
      iObject.Length = this.mLength;
      iObject.Branch = this.mParents.Count > 0;
      playState.Scene.AddRenderableAdditiveObject(iDataChannel, (IRenderableAdditiveObject) iObject);
    }
    else
    {
      Railgun.AdditionalRenderData iObject = this.mAdditionalRenderData[(int) iDataChannel];
      iObject.Time = this.mTime;
      iObject.Cut = this.mCut;
      iObject.Position = this.mPosition;
      iObject.Direction = this.mDirection;
      iObject.Length = this.mLength;
      iObject.Elements = this.mAdditionalElements;
      playState.Scene.AddRenderableAdditiveObject(iDataChannel, (IRenderableAdditiveObject) iObject);
    }
  }

  private void Explode(PlayState iPlayState)
  {
    Vector3.Lerp(ref this.mExplosionPosition, ref this.mPosition, 0.0001f, out this.mExplosionPosition);
    Railgun railgun = this;
    while (railgun.mChild != null)
      railgun = railgun.mChild;
    int count = this.mDamages.Count;
    if (this.mExplosionCompanion != null)
      count += this.mExplosionCompanion.mDamages.Count;
    float iRadius = (float) (3.0 + 1.0 * (double) count);
    railgun.KillAll();
    bool flag1 = false;
    for (int index = 0; index < this.mDamages.Count; ++index)
    {
      if ((Blast.FullBlast(iPlayState, Entity.GetFromHandle((int) this.mDamages.Keys[index]), this.mTimeStamp, (Entity) null, iRadius, this.mExplosionPosition, this.mDamages.Values[index]) & (DamageResult.Killed | DamageResult.OverKilled)) != DamageResult.None)
        flag1 = true;
    }
    if (this.mExplosionCompanion != null)
    {
      for (int index = 0; index < this.mExplosionCompanion.mDamages.Count; ++index)
      {
        if ((Blast.FullBlast(iPlayState, Entity.GetFromHandle((int) this.mExplosionCompanion.mDamages.Keys[index]), this.mTimeStamp, (Entity) null, iRadius, this.mExplosionPosition, this.mExplosionCompanion.mDamages.Values[index]) & (DamageResult.Killed | DamageResult.OverKilled)) != DamageResult.None)
          flag1 = true;
      }
    }
    Damage iDamage;
    iDamage.AttackProperty = AttackProperties.Knockback;
    iDamage.Element = Elements.None;
    iDamage.Amount = 500f;
    iDamage.Magnitude = 2f;
    if ((Blast.FullBlast(iPlayState, (Entity) null, this.mTimeStamp, (Entity) null, 5f, this.mExplosionPosition, iDamage) & (DamageResult.Killed | DamageResult.OverKilled)) != DamageResult.None)
      flag1 = true;
    if (!this.mIsOposits)
      return;
    bool flag2 = false;
    foreach (ushort key in (IEnumerable<ushort>) this.mDamages.Keys)
    {
      for (int index = 0; index < Magicka.Game.Instance.Players.Length; ++index)
      {
        if (Magicka.Game.Instance.Players[index].Playing && (int) key == (int) Magicka.Game.Instance.Players[index].Avatar.Handle && !(Magicka.Game.Instance.Players[index].Gamer is NetworkGamer))
          flag2 = true;
      }
    }
    if (!flag1 || !flag2)
      return;
    AchievementsManager.Instance.AwardAchievement(iPlayState, "nevercrossthebeams");
  }

  private void GetFirstJunction(out Vector3 mExplosionPosition)
  {
    if (this.mParents.Count == 1)
      this.mParents[0].GetFirstJunction(out mExplosionPosition);
    else
      mExplosionPosition = this.mPosition;
  }

  private void LockAll()
  {
    this.mLocked = true;
    for (int index = 0; index < this.mParents.Count; ++index)
      this.mParents[index].LockAll();
  }

  private void KillAll()
  {
    this.mDead = true;
    this.mCut = 1f;
    if (this.mChild != null)
      throw new Exception();
    for (int index = 0; index < this.mParents.Count; ++index)
    {
      this.mParents[index].mChild = (Railgun) null;
      this.mParents[index].KillAll();
    }
    this.mParents.Clear();
  }

  public void OnRemove()
  {
    this.mLight.Disable();
    if (this.mImmaFirinMahLazer)
    {
      bool flag = false;
      PlayState iPlayState = (PlayState) null;
      foreach (ushort key in (IEnumerable<ushort>) this.mDamages.Keys)
      {
        for (int index = 0; index < Magicka.Game.Instance.Players.Length; ++index)
        {
          if (Magicka.Game.Instance.Players[index].Playing && (int) key == (int) Magicka.Game.Instance.Players[index].Avatar.Handle && !(Magicka.Game.Instance.Players[index].Gamer is NetworkGamer))
          {
            flag = true;
            iPlayState = Magicka.Game.Instance.Players[index].Avatar.PlayState;
          }
        }
      }
      if (flag)
        AchievementsManager.Instance.AwardAchievement(iPlayState, "immafirinmahlazer");
    }
    if (this.mChild != null)
    {
      this.mChild.Kill();
      this.mChild = (Railgun) null;
    }
    for (int index = 0; index < this.mParents.Count; ++index)
      this.mParents[index].mChild = (Railgun) null;
    this.mParents.Clear();
    EffectManager.Instance.Stop(ref this.mSourceEffect);
    EffectManager.Instance.Stop(ref this.mHitEffect);
    this.mDamages.Clear();
    if (this.mStageStartCue != null)
      this.mStageStartCue.Stop(AudioStopOptions.AsAuthored);
    if (this.mStageHitCue != null)
      this.mStageHitCue.Stop(AudioStopOptions.AsAuthored);
    for (int index = 0; index < this.mAdditionalElementCues.Count; ++index)
    {
      if (this.mAdditionalElementCues[index] != null)
        this.mAdditionalElementCues[index].Stop(AudioStopOptions.AsAuthored);
    }
    this.mAdditionalElementCues.Clear();
    Railgun.sActiveRails.Remove(this);
    Railgun.sCache.Add(this);
  }

  protected class RenderData : IRenderableAdditiveObject
  {
    public Vector3 Position;
    public Vector3 Direction;
    public float Length;
    public Vector3 ColorCenter;
    public Vector3 ColorEdge;
    public float Cut;
    public float Time;
    public int Nr;
    public bool Branch;

    int IRenderableAdditiveObject.Effect => ArcaneEffect.TYPEHASH;

    int IRenderableAdditiveObject.Technique => 1;

    VertexBuffer IRenderableAdditiveObject.Vertices => Railgun.sVertexBuffer;

    int IRenderableAdditiveObject.VerticesHashCode => Railgun.sVertexBufferHash;

    int IRenderableAdditiveObject.VertexStride => 16 /*0x10*/;

    IndexBuffer IRenderableAdditiveObject.Indices => (IndexBuffer) null;

    VertexDeclaration IRenderableAdditiveObject.VertexDeclaration => Railgun.sVertexDeclaration;

    bool IRenderableAdditiveObject.Cull(BoundingFrustum iViewFrustum) => false;

    void IRenderableAdditiveObject.Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      ArcaneEffect arcaneEffect = iEffect as ArcaneEffect;
      arcaneEffect.Origin = this.Position;
      arcaneEffect.Direction = this.Direction;
      arcaneEffect.Length = this.Length;
      arcaneEffect.ColorCenter = this.ColorCenter;
      arcaneEffect.ColorEdge = this.ColorEdge;
      arcaneEffect.Cut = this.Cut;
      arcaneEffect.Alpha = 1f;
      arcaneEffect.StartLength = 4f;
      arcaneEffect.Dropoff = 0.666f;
      arcaneEffect.MinRadius = 0.0f;
      arcaneEffect.MaxRadius = 0.0f;
      arcaneEffect.RayRadius = 0.333f;
      arcaneEffect.Texture = Railgun.sTexture;
      arcaneEffect.Time = this.Time;
      arcaneEffect.TextureScale = 0.1f;
      arcaneEffect.CommitChanges();
      arcaneEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleStrip, 0, 510);
      float d = 0.5f;
      for (int index = 0; index < this.Nr; ++index)
      {
        float num = (float) Math.Sqrt((double) d);
        arcaneEffect.Time = (this.Time + d * 10f) * num;
        arcaneEffect.Clockwice = index % 2 == 0;
        arcaneEffect.WaveScale = 4f * num;
        arcaneEffect.MinRadius = 0.15f * num;
        arcaneEffect.MaxRadius = !this.Branch ? 1.25f * num : 0.15f * num;
        arcaneEffect.RayRadius = 0.1f / num;
        arcaneEffect.CommitChanges();
        arcaneEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleStrip, 0, 510);
        d += 0.25f;
      }
    }
  }

  protected class AdditionalRenderData : IRenderableAdditiveObject, IPreRenderRenderer
  {
    public Vector3 Position;
    public Vector3 Direction;
    public float Length;
    public float Cut;
    public float Time;
    public Elements Elements;
    private Matrix mTransform;

    public int Effect => AdditiveEffect.TYPEHASH;

    public int Technique => 0;

    public VertexBuffer Vertices => Railgun.sAdditionalVertexBuffer;

    public int VerticesHashCode => Railgun.sAdditionalVertexBufferHash;

    public int VertexStride => 16 /*0x10*/;

    public IndexBuffer Indices => (IndexBuffer) null;

    public VertexDeclaration VertexDeclaration => Railgun.sAdditionalVertexDeclaration;

    public bool Cull(BoundingFrustum iViewFrustum) => false;

    public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      AdditiveEffect additiveEffect = iEffect as AdditiveEffect;
      additiveEffect.VertexColorEnabled = false;
      additiveEffect.Texture = Railgun.sTexture;
      additiveEffect.TextureEnabled = true;
      Vector4 vector4 = new Vector4();
      vector4.W = 1f;
      Vector2 vector2_1 = new Vector2();
      vector2_1.X = this.Length * 0.0333f;
      vector2_1.Y = 1f;
      additiveEffect.TextureScale = vector2_1;
      for (int index = 0; index < 11; ++index)
      {
        Elements elements = (Elements) (1 << index);
        if ((elements & this.Elements) == elements)
        {
          if (elements == Elements.Lightning)
          {
            vector4.X = Spell.LIGHTNINGCOLOR.X * 2f;
            vector4.Y = Spell.LIGHTNINGCOLOR.Y * 2f;
            vector4.Z = Spell.LIGHTNINGCOLOR.Z * 2f;
          }
          else
            vector4.X = vector4.Y = vector4.Z = 2f;
          additiveEffect.ColorTint = vector4;
          additiveEffect.World = this.mTransform;
          vector2_1.Y = 1f;
          additiveEffect.TextureScale = vector2_1;
          Vector2 vector2_2 = Railgun.ELEMENT_OFFSET_LOOKUP[index] with
          {
            X = this.Time * -0.05f + this.Cut
          };
          additiveEffect.TextureOffset = vector2_2;
          additiveEffect.CommitChanges();
          additiveEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
          vector2_2.X = (float) ((double) vector2_2.X * 1.3256675004959106 + 0.35465168952941895);
          vector2_2.Y += 0.125f;
          additiveEffect.TextureOffset = vector2_2;
          vector2_1.Y = -vector2_1.Y;
          additiveEffect.TextureScale = vector2_1;
          Matrix mTransform = this.mTransform;
          mTransform.M11 *= 0.666f;
          mTransform.M12 *= 0.666f;
          mTransform.M13 *= 0.666f;
          additiveEffect.World = mTransform;
          additiveEffect.CommitChanges();
          additiveEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
        }
      }
    }

    public void PreRenderUpdate(
      DataChannel iDataChannel,
      float iDeltaTime,
      ref Matrix iViewProjectionMatrix,
      ref Vector3 iCameraPosition,
      ref Vector3 iCameraDirection)
    {
      Matrix.CreateConstrainedBillboard(ref this.Position, ref iCameraPosition, ref this.Direction, new Vector3?(iCameraDirection), new Vector3?(), out this.mTransform);
      float num = this.Length * (1f - this.Cut);
      Vector3 result = this.mTransform.Up;
      Vector3.Multiply(ref result, this.Length * this.Cut, out result);
      Vector3.Add(ref result, ref this.Position, out result);
      this.mTransform.Translation = result;
      this.mTransform.M21 *= num;
      this.mTransform.M22 *= num;
      this.mTransform.M23 *= num;
    }
  }

  public enum RailStage
  {
    Start,
    Hit,
    Bloat,
    Explode,
  }
}
