// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.SummonPhoenix
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using System;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class SummonPhoenix : SpecialAbility, IAbilityEffect
{
  private static SummonPhoenix mSingelton;
  private static volatile object mSingeltonLock = new object();
  private static SummonPhoenix.RenderData[] mRenderData;
  private static SkinnedModel sModel;
  private static AnimationController sController;
  private static AnimationClip sClip;
  private static AudioEmitter mAudioEmitter;
  private static bool sFinished = true;
  private static bool sCastFail;
  private static float sFailTTL;
  private static Matrix sMatrix;
  private static PlayState sPlayState;
  public static readonly int SOUND_FAIL_HASH = "magick_summon_phoenix_indoors".GetHashCodeCustom();
  public static readonly int SOUND_HASH = "magick_summon_phoenix".GetHashCodeCustom();
  public static readonly int REVIVE_EFFECT = "magick_revive".GetHashCodeCustom();
  public static readonly int BLAST_EFFECT = "magick_phoenix_blast".GetHashCodeCustom();
  private static DamageCollection5 sDamage;
  private static BoundingSphere sBoundingSphere;
  private ISpellCaster mOwner;

  public static SummonPhoenix Instance
  {
    get
    {
      if (SummonPhoenix.mSingelton == null)
      {
        lock (SummonPhoenix.mSingeltonLock)
        {
          if (SummonPhoenix.mSingelton == null)
            SummonPhoenix.mSingelton = new SummonPhoenix();
        }
      }
      return SummonPhoenix.mSingelton;
    }
  }

  static SummonPhoenix()
  {
    lock (Magicka.Game.Instance.GraphicsDevice)
      SummonPhoenix.sModel = Magicka.Game.Instance.Content.Load<SkinnedModel>("Models/Magicks/Phoenix");
    SummonPhoenix.mAudioEmitter = new AudioEmitter();
    SummonPhoenix.sController = new AnimationController();
    SummonPhoenix.sController.Skeleton = SummonPhoenix.sModel.SkeletonBones;
    SummonPhoenix.sClip = SummonPhoenix.sModel.AnimationClips["descend"];
    ModelMesh mesh = SummonPhoenix.sModel.Model.Meshes[0];
    ModelMeshPart meshPart = mesh.MeshParts[0];
    SummonPhoenix.sBoundingSphere = mesh.BoundingSphere;
    SummonPhoenix.mRenderData = new SummonPhoenix.RenderData[3];
    SummonPhoenix.mRenderData[0] = new SummonPhoenix.RenderData();
    SummonPhoenix.mRenderData[0].mBones = new Matrix[80 /*0x50*/];
    SummonPhoenix.mRenderData[0].SetMesh(mesh, meshPart, 3, 0, 4);
    SummonPhoenix.mRenderData[1] = new SummonPhoenix.RenderData();
    SummonPhoenix.mRenderData[1].mBones = new Matrix[80 /*0x50*/];
    SummonPhoenix.mRenderData[1].SetMesh(mesh, meshPart, 3, 0, 4);
    SummonPhoenix.mRenderData[2] = new SummonPhoenix.RenderData();
    SummonPhoenix.mRenderData[2].mBones = new Matrix[80 /*0x50*/];
    SummonPhoenix.mRenderData[2].SetMesh(mesh, meshPart, 3, 0, 4);
    SummonPhoenix.sDamage = new DamageCollection5();
    SummonPhoenix.sDamage.A = new Damage(AttackProperties.Damage, Elements.Fire, 400f, 1f);
    SummonPhoenix.sDamage.B = new Damage(AttackProperties.Status, Elements.Fire, 200f, 5f);
    SummonPhoenix.sDamage.C = new Damage(AttackProperties.Knockdown, Elements.Fire, 400f, 2f);
  }

  private SummonPhoenix()
    : base(Magicka.Animations.cast_magick_direct, "#magick_sphoenix".GetHashCodeCustom())
  {
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    SummonPhoenix.sPlayState = iPlayState;
    if (SummonPhoenix.sPlayState.Level.CurrentScene.Indoors)
    {
      AudioManager.Instance.PlayCue(Banks.Spells, SummonPhoenix.SOUND_FAIL_HASH, SummonPhoenix.mAudioEmitter);
      SummonPhoenix.sCastFail = true;
      SummonPhoenix.sFinished = false;
      SummonPhoenix.sFailTTL = 1.5f;
      return true;
    }
    if (!SummonPhoenix.sFinished)
      return false;
    SummonPhoenix.sCastFail = false;
    this.mOwner = (ISpellCaster) null;
    SummonPhoenix.sFinished = false;
    SummonPhoenix.sController.StartClip(SummonPhoenix.sClip, false);
    Vector3 result = iPosition;
    Vector3 vector3_1 = new Vector3((float) (SpecialAbility.RANDOM.NextDouble() - 0.5) * 3f, 0.0f, (float) (SpecialAbility.RANDOM.NextDouble() - 0.5) * 3f);
    Vector3.Add(ref result, ref vector3_1, out result);
    Vector3 vector3_2;
    double nearestPosition = (double) SummonPhoenix.sPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref result, out vector3_2, MovementProperties.Default);
    result = vector3_2;
    Segment iSeg = new Segment();
    iSeg.Origin = result;
    ++iSeg.Origin.Y;
    iSeg.Delta.Y = -3f;
    if (SummonPhoenix.sPlayState.Level.CurrentScene.SegmentIntersect(out float _, out vector3_2, out Vector3 _, iSeg))
    {
      result = vector3_2;
      ++result.Y;
    }
    Matrix.CreateRotationY(4.712389f, out SummonPhoenix.sMatrix);
    SummonPhoenix.sMatrix.Translation = result;
    SummonPhoenix.mAudioEmitter.Position = result;
    SummonPhoenix.mAudioEmitter.Up = Vector3.Up;
    SummonPhoenix.mAudioEmitter.Forward = Vector3.Right;
    AudioManager.Instance.PlayCue(Banks.Spells, SummonPhoenix.SOUND_HASH, SummonPhoenix.mAudioEmitter);
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    return true;
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    SummonPhoenix.sPlayState = iPlayState;
    Vector3 position = iOwner.Position;
    if (SummonPhoenix.sPlayState.Level.CurrentScene.Indoors)
    {
      AudioManager.Instance.PlayCue(Banks.Spells, SummonPhoenix.SOUND_FAIL_HASH, SummonPhoenix.mAudioEmitter);
      SummonPhoenix.sCastFail = true;
      SummonPhoenix.sFinished = false;
      SummonPhoenix.sFailTTL = 1.5f;
      return true;
    }
    if (SummonPhoenix.sFinished)
    {
      SummonPhoenix.sCastFail = false;
      this.mOwner = iOwner;
      SummonPhoenix.sFinished = false;
      SummonPhoenix.sController.StartClip(SummonPhoenix.sClip, false);
      Vector3 result = position;
      Vector3 vector3_1 = new Vector3((float) (SpecialAbility.RANDOM.NextDouble() - 0.5) * 4f, 0.0f, (float) (SpecialAbility.RANDOM.NextDouble() - 0.5) * 4f);
      Vector3.Add(ref result, ref vector3_1, out result);
      Vector3 vector3_2;
      double nearestPosition = (double) SummonPhoenix.sPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref result, out vector3_2, MovementProperties.Default);
      Vector3 vector3_3 = vector3_2;
      Segment iSeg = new Segment();
      iSeg.Origin = vector3_3;
      ++iSeg.Origin.Y;
      iSeg.Delta.Y = -3f;
      if (SummonPhoenix.sPlayState.Level.CurrentScene.SegmentIntersect(out float _, out vector3_2, out Vector3 _, iSeg))
        vector3_3 = vector3_2;
      Matrix.CreateRotationY(4.712389f, out SummonPhoenix.sMatrix);
      SummonPhoenix.sMatrix.Translation = vector3_3;
      SummonPhoenix.mAudioEmitter.Position = vector3_3;
      SummonPhoenix.mAudioEmitter.Up = Vector3.Up;
      SummonPhoenix.mAudioEmitter.Forward = Vector3.Right;
      AudioManager.Instance.PlayCue(Banks.Spells, SummonPhoenix.SOUND_HASH, SummonPhoenix.mAudioEmitter);
      SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    }
    return true;
  }

  public bool IsDead => SummonPhoenix.sFinished;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if (SummonPhoenix.sCastFail)
    {
      SummonPhoenix.sFailTTL -= iDeltaTime;
      if ((double) SummonPhoenix.sFailTTL > 0.0)
        return;
      SummonPhoenix.sPlayState.Camera.CameraShake(4f, 0.2f);
      SummonPhoenix.sFinished = true;
    }
    else
    {
      SummonPhoenix.mAudioEmitter.Position = SummonPhoenix.sMatrix.Translation;
      SummonPhoenix.mAudioEmitter.Up = Vector3.Up;
      SummonPhoenix.mAudioEmitter.Forward = Vector3.Right;
      SummonPhoenix.sBoundingSphere.Center = SummonPhoenix.mAudioEmitter.Position;
      SummonPhoenix.mRenderData[(int) iDataChannel].mBoundingSphere = SummonPhoenix.sBoundingSphere;
      SummonPhoenix.sController.Update(iDeltaTime, ref SummonPhoenix.sMatrix, true);
      SummonPhoenix.sController.SkinnedBoneTransforms.CopyTo((Array) SummonPhoenix.mRenderData[(int) iDataChannel].mBones, 0);
      SummonPhoenix.sPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) SummonPhoenix.mRenderData[(int) iDataChannel]);
      if (!SummonPhoenix.sController.HasFinished || SummonPhoenix.sFinished)
        return;
      Vector3 result1 = SummonPhoenix.sMatrix.Translation;
      result1.Y += 0.5f;
      Vector3 right = Vector3.Right;
      int num = (int) Helper.CircleDamage(SummonPhoenix.sPlayState, this.mOwner as Entity, this.mTimeStamp, (Entity) null, ref result1, 6f, ref SummonPhoenix.sDamage);
      SummonPhoenix.sPlayState.Camera.CameraShake(1f, 1f);
      EffectManager.Instance.StartEffect(SummonPhoenix.BLAST_EFFECT, ref result1, ref right, out VisualEffectReference _);
      AudioManager.Instance.PlayCue(Banks.Spells, "spell_fire_area".GetHashCodeCustom(), SummonPhoenix.mAudioEmitter);
      Player[] players = Magicka.Game.Instance.Players;
      for (int index = 0; index < players.Length; ++index)
      {
        if (NetworkManager.Instance.State != NetworkState.Client)
        {
          Matrix result2;
          Matrix.CreateRotationY((float) ((double) index * 6.2831854820251465 * 0.25), out result2);
          if (players[index].Playing && players[index].Avatar.Dead)
          {
            result1 = SummonPhoenix.sMatrix.Translation;
            Vector3 forward = result2.Forward;
            Vector3.Add(ref forward, ref result1, out result1);
            Vector3 vector3;
            double nearestPosition = (double) SummonPhoenix.sPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref result1, out vector3, MovementProperties.Default);
            result1 = vector3;
            Segment iSeg = new Segment();
            iSeg.Origin = result1;
            ++iSeg.Origin.Y;
            iSeg.Delta.Y = -3f;
            if (SummonPhoenix.sPlayState.Level.CurrentScene.SegmentIntersect(out float _, out vector3, out Vector3 _, iSeg))
              result1 = vector3;
            Revive instance = Revive.GetInstance();
            instance.SetSpecificPlayer(players[index].ID);
            instance.Execute(result1, SummonPhoenix.sPlayState);
          }
        }
      }
      SummonPhoenix.sFinished = true;
    }
  }

  public void OnRemove() => SummonPhoenix.sFinished = true;

  public class RenderData : RenderableObject<SkinnedModelBasicEffect, MagickaSkinnedModelMaterial>
  {
    public Matrix[] mBones;

    public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      (iEffect as SkinnedModelBasicEffect).Bones = this.mBones;
      base.Draw(iEffect, iViewFrustum);
    }

    public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      (iEffect as SkinnedModelBasicEffect).Bones = this.mBones;
      base.DrawShadow(iEffect, iViewFrustum);
    }
  }
}
