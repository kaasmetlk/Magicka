// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Napalm
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Magicka.Graphics.Lights;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using PolygonHead.Lights;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

internal class Napalm : SpecialAbility, IAbilityEffect
{
  internal const int NR_OF_FIELDS = 6;
  private const int NAPALM_HITS = 6;
  private const float NAPALM_TIME = 2f;
  private const float NAPALM_WAIT_TIME = 5f;
  private static Napalm sSingelton;
  private static volatile object sSingeltonLock = new object();
  private static Model sModel;
  private static readonly int TARGET_EFFECT = "magick_smoke_target".GetHashCodeCustom();
  public static readonly int NAPALM_EFFECT = "magick_napalm".GetHashCodeCustom();
  public static readonly int NAPALM_SOUND = "magick_napalm".GetHashCodeCustom();
  private static readonly int PLANE_PASSBY_SOUND = "magick_plane_passby".GetHashCodeCustom();
  public static readonly DamageCollection5 sNapalmDamage = new DamageCollection5();
  private bool mDead;
  private Napalm.RenderData[] mRenderData;
  private int mNapalmHits;
  private float mTimer;
  private PlayState mPlayState;
  private ISpellCaster mOwner;
  private VisualEffectReference mSmokeEffect;
  private bool mAirstrike;
  private Matrix mFocalPoint;
  private bool mNoLight;

  public static Napalm Instance
  {
    get
    {
      if (Napalm.sSingelton == null)
      {
        lock (Napalm.sSingeltonLock)
        {
          if (Napalm.sSingelton == null)
            Napalm.sSingelton = new Napalm();
        }
      }
      return Napalm.sSingelton;
    }
  }

  static Napalm()
  {
    Napalm.sNapalmDamage.AddDamage(new Damage(AttackProperties.Damage, Elements.Fire, 2000f, 1f));
    Napalm.sNapalmDamage.AddDamage(new Damage(AttackProperties.Knockback, Elements.Earth, 500f, 2f));
    Napalm.sNapalmDamage.AddDamage(new Damage(AttackProperties.Status, Elements.Fire, 300f, 3f));
  }

  private Napalm()
    : base(Magicka.Animations.cast_magick_sweep, "#magick_napalm".GetHashCodeCustom())
  {
    try
    {
      if (Napalm.sModel == null)
      {
        lock (Magicka.Game.Instance.GraphicsDevice)
          Napalm.sModel = Magicka.Game.Instance.Content.Load<Model>("Models/Magicks/f4");
      }
      this.mRenderData = new Napalm.RenderData[3];
      for (int index = 0; index < 3; ++index)
      {
        this.mRenderData[index] = new Napalm.RenderData();
        this.mRenderData[index].SetMesh(Napalm.sModel.Meshes[0], Napalm.sModel.Meshes[0].MeshParts[0], 4, 0, 5);
      }
    }
    catch
    {
    }
    this.mDead = true;
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    if (Napalm.sModel == null)
      return false;
    if (iPlayState.Level.CurrentScene.Indoors)
    {
      AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL, iOwner.AudioEmitter);
      return false;
    }
    if (!this.mDead)
    {
      if (this.mAirstrike)
      {
        AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL, iOwner.AudioEmitter);
        return false;
      }
      this.mNoLight = iPlayState.Level.CurrentScene.DirectionalLightSettings.Length == 0;
      this.mOwner = iOwner;
      Vector3 result1 = iOwner.Position;
      Vector3 result2 = iOwner.Direction;
      Vector3.Multiply(ref result2, 15f, out result2);
      Vector3.Add(ref result1, ref result2, out result1);
      this.mFocalPoint.Translation = result1;
      EffectManager.Instance.Stop(ref this.mSmokeEffect);
      EffectManager.Instance.StartEffect(Napalm.TARGET_EFFECT, ref this.mFocalPoint, out this.mSmokeEffect);
      return true;
    }
    this.mNoLight = iPlayState.Level.CurrentScene.DirectionalLightSettings.Length == 0;
    EffectManager.Instance.Stop(ref this.mSmokeEffect);
    this.mDead = false;
    this.mOwner = iOwner;
    Vector3 result3 = iOwner.Position;
    Vector3 result4 = iOwner.Direction;
    Vector3.Multiply(ref result4, 15f, out result4);
    Vector3.Add(ref result3, ref result4, out result3);
    Vector3 result5 = iOwner.Direction;
    Matrix result6;
    Matrix.CreateRotationY(1.57079637f, out result6);
    Vector3.TransformNormal(ref result5, ref result6, out result5);
    Vector3 up = Vector3.Up;
    Matrix.CreateWorld(ref result3, ref result5, ref up, out this.mFocalPoint);
    this.mPlayState = iPlayState;
    EffectManager.Instance.StartEffect(Napalm.TARGET_EFFECT, ref this.mFocalPoint, out this.mSmokeEffect);
    this.mTimer = 5f;
    this.mAirstrike = false;
    this.mNapalmHits = 0;
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    return true;
  }

  public bool IsDead => this.mDead;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mTimer -= iDeltaTime;
    if (this.mAirstrike)
    {
      Vector3 forward = this.mFocalPoint.Forward;
      Vector3 result1 = this.mFocalPoint.Translation;
      Vector3 result2 = result1;
      Vector3 result3;
      Vector3.Multiply(ref forward, -120f, out result3);
      Vector3.Add(ref result3, ref result1, out result1);
      Vector3.Multiply(ref forward, 120f, out result3);
      Vector3.Add(ref result3, ref result2, out result2);
      float amount = (float) (1.0 - (double) this.mTimer / 2.0);
      float num1 = 0.25f;
      float num2 = (float) (0.5 + (double) num1 - 0.044999998062849045);
      float num3 = num2 + (float) this.mNapalmHits * 0.015f;
      Vector3 result4;
      Vector3.Lerp(ref result2, ref result1, amount, out result4);
      Matrix result5 = this.mFocalPoint with
      {
        Translation = result4
      };
      if (NetworkManager.Instance.State != NetworkState.Client && (double) amount >= (double) num2 && (double) amount >= (double) num3 && this.mNapalmHits < 6)
      {
        Segment seg = new Segment();
        Vector3.Lerp(ref result2, ref result1, amount - num1, out seg.Origin);
        seg.Origin.Y += 10f;
        seg.Delta.Y = -20f;
        float num4;
        Vector3 iPosition;
        Vector3 vector3;
        AnimatedLevelPart oAnimatedLevelPart;
        bool flag1 = this.mPlayState.Level.CurrentScene.SegmentIntersect(out num4, out iPosition, out vector3, out oAnimatedLevelPart, seg);
        bool flag2 = false;
        if (!flag1)
        {
          for (int index = 0; index < this.mPlayState.Level.CurrentScene.Liquids.Length; ++index)
          {
            if (this.mPlayState.Level.CurrentScene.Liquids[index].SegmentIntersect(out num4, out iPosition, out vector3, ref seg, true, true, false))
            {
              flag2 = true;
              flag1 = true;
              break;
            }
          }
        }
        if (flag1)
        {
          Vector3 up = Vector3.Up;
          EffectManager.Instance.StartEffect(Napalm.NAPALM_EFFECT, ref iPosition, ref up, out VisualEffectReference _);
          AudioManager.Instance.PlayCue(Banks.Additional, Napalm.NAPALM_SOUND);
          this.mPlayState.Camera.CameraShake(iPosition, 0.25f, 0.5f);
          Grease.GreaseField iEntity = (Grease.GreaseField) null;
          if (!flag2)
          {
            iEntity = Grease.GreaseField.GetInstance(this.mPlayState);
            iEntity.Initialize(this.mOwner, oAnimatedLevelPart, ref iPosition, ref up);
            iEntity.Burn(3f);
            this.mPlayState.EntityManager.AddEntity((Entity) iEntity);
          }
          int num5 = (int) Napalm.NapalmBlast(this.mPlayState, ref iPosition, this.mOwner as Entity, this.mTimeStamp);
          if (NetworkManager.Instance.State == NetworkState.Server)
            NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
            {
              Handle = this.mOwner.Handle,
              Position = iPosition,
              Direction = up,
              Arg = oAnimatedLevelPart == null ? (int) ushort.MaxValue : (int) oAnimatedLevelPart.Handle,
              Id = iEntity == null ? int.MinValue : (int) iEntity.Handle,
              ActionType = TriggerActionType.NapalmStrike,
              TimeStamp = this.mTimeStamp
            });
        }
        ++this.mNapalmHits;
      }
      if (!this.mNoLight)
      {
        Matrix result6;
        Matrix.CreateScale(2.5f, out result6);
        Matrix.Multiply(ref result6, ref result5, out result5);
        Vector3 result7 = (this.mPlayState.Level.CurrentScene.DirectionalLightSettings[0].Light as DirectionalLight).LightDirection;
        Vector3.Negate(ref result7, out result7);
        Vector3.Multiply(ref result7, 20f, out result7);
        Vector3.Add(ref result7, ref result4, out result4);
        result5.Translation = result4;
        this.mRenderData[(int) iDataChannel].Transform = result5;
        this.mRenderData[(int) iDataChannel].mBoundingSphere.Center = this.mFocalPoint.Translation;
        this.mRenderData[(int) iDataChannel].mBoundingSphere.Radius = 30f;
        this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) this.mRenderData[(int) iDataChannel]);
      }
      if ((double) this.mTimer > 0.0)
        return;
      this.mDead = true;
    }
    else
    {
      if ((double) this.mTimer > 0.0)
        return;
      this.mAirstrike = true;
      this.mTimer = 2f;
      this.mPlayState.Camera.CameraShake(this.mFocalPoint.Translation, 0.5f, 2f);
      AudioManager.Instance.PlayCue(Banks.Additional, Napalm.PLANE_PASSBY_SOUND);
    }
  }

  public void OnRemove()
  {
    EffectManager.Instance.Stop(ref this.mSmokeEffect);
    this.mDead = true;
    this.mAirstrike = false;
  }

  internal static DamageResult NapalmBlast(
    PlayState iPlayState,
    ref Vector3 iPosition,
    Entity iOwner,
    double iTimeStamp)
  {
    List<Entity> entities = iPlayState.EntityManager.GetEntities(iPosition, 5f, true, false);
    for (int index = 0; index < entities.Count; ++index)
    {
      if (entities[index] is IDamageable t && (!(t is Magicka.GameLogic.Entities.Character) || !(t as Magicka.GameLogic.Entities.Character).IsEthereal))
      {
        int num = (int) t.Damage(Napalm.sNapalmDamage, iOwner, iTimeStamp, iPosition);
      }
    }
    iPlayState.EntityManager.ReturnEntityList(entities);
    Vector3 iDirection = new Vector3(3.75f, 0.0f, 0.0f);
    Damage c = Napalm.sNapalmDamage.C;
    Liquid.Freeze(iPlayState.Level.CurrentScene, ref iPosition, ref iDirection, 6.28318548f, 10f, ref c);
    DynamicLight cachedLight = DynamicLight.GetCachedLight();
    Vector3 iPosition1 = iPosition;
    iPosition1.Y += 0.5f;
    Vector3 iColor = new Vector3(1f, 1f, 0.0f);
    cachedLight.Initialize(iPosition1, iColor, 3f, 16f, 5f, 1.5f, 0.5f);
    cachedLight.Enable(iPlayState.Scene);
    cachedLight.Disable(LightTransitionType.Linear, 2f);
    return DamageResult.None;
  }

  private static void LightCallback(DynamicLight iLight)
  {
  }

  private class RenderData : RenderableObject<RenderDeferredEffect, RenderDeferredMaterial>
  {
    public Matrix Transform;

    public override int DepthTechnique => -1;

    public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
    }

    public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      this.mMaterial.WorldTransform = this.Transform;
      base.DrawShadow(iEffect, iViewFrustum);
    }
  }
}
