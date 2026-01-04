// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Spells.UnderGroundAttack
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.Audio;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PolygonHead;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Spells;

public class UnderGroundAttack : IAbilityEffect
{
  public static readonly int[] EFFECTS = new int[11]
  {
    "underground_earth".GetHashCodeCustom(),
    "underground_water".GetHashCodeCustom(),
    "underground_cold".GetHashCodeCustom(),
    "underground_fire".GetHashCodeCustom(),
    0,
    "underground_arcane".GetHashCodeCustom(),
    "underground_life".GetHashCodeCustom(),
    0,
    0,
    "underground_steam".GetHashCodeCustom(),
    "underground_steam".GetHashCodeCustom()
  };
  public static readonly int[] SFX = new int[11]
  {
    "spell_earth_ground".GetHashCodeCustom(),
    "spell_water_spray".GetHashCodeCustom(),
    "spell_cold_spray".GetHashCodeCustom(),
    "spell_fire_spray".GetHashCodeCustom(),
    0,
    "spell_arcane_ray_stage2".GetHashCodeCustom(),
    "spell_life_ray_stage2".GetHashCodeCustom(),
    0,
    0,
    "spell_steam_spray".GetHashCodeCustom(),
    "spell_poison_spray".GetHashCodeCustom()
  };
  private static List<UnderGroundAttack> sCache;
  private float mRange;
  private ISpellCaster mOwner;
  private DamageCollection5 mDamage;
  private Vector3 mPosition;
  private Vector3 mDirection;
  private Vector2 mVelocity;
  private Cue[] mCues;
  private AudioEmitter mAE;
  private PlayState mPlayState;
  private double mTimeStamp;
  private VisualEffectReference[] mEffect = new VisualEffectReference[5];
  private List<ushort> mHitList = new List<ushort>(32 /*0x20*/);

  public static void InitializeCache(int iNr, PlayState iPlayState)
  {
    UnderGroundAttack.sCache = new List<UnderGroundAttack>(iNr);
    for (int index = 0; index < iNr; ++index)
      UnderGroundAttack.sCache.Add(new UnderGroundAttack(iPlayState));
  }

  public static UnderGroundAttack GetFromCache(PlayState iPlayState)
  {
    if (UnderGroundAttack.sCache.Count <= 0)
      return new UnderGroundAttack(iPlayState);
    UnderGroundAttack fromCache = UnderGroundAttack.sCache[UnderGroundAttack.sCache.Count - 1];
    UnderGroundAttack.sCache.RemoveAt(UnderGroundAttack.sCache.Count - 1);
    return fromCache;
  }

  private UnderGroundAttack(PlayState iPlayState)
  {
    this.mPlayState = iPlayState;
    this.mCues = new Cue[11];
    this.mAE = new AudioEmitter();
    this.mAE.Up = Vector3.Up;
  }

  public void Initialize(
    ref Vector3 iPosition,
    ref Vector2 iVelocity,
    ISpellCaster iOwner,
    double iTimeStamp,
    float iRange,
    DamageCollection5 iDamage,
    bool iPiercing)
  {
    this.mHitList.Clear();
    this.mHitList.Add(iOwner.Handle);
    this.mRange = iRange;
    this.mPosition = iPosition;
    this.mVelocity = iVelocity;
    this.mOwner = iOwner;
    this.mTimeStamp = iTimeStamp;
    this.mDamage = iDamage;
    this.mAE.Velocity = new Vector3(this.mVelocity.X, 0.0f, this.mVelocity.Y);
    this.mAE.Forward = Vector3.Normalize(this.mAE.Velocity);
    Vector2 result;
    Vector2.Normalize(ref iVelocity, out result);
    this.mDirection.X = result.X;
    this.mDirection.Z = result.Y;
    Segment iSeg = new Segment();
    iSeg.Origin = iPosition;
    ++iSeg.Origin.Y;
    iSeg.Delta.Y = -4f;
    Vector3 oNrm;
    if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out this.mPosition, out oNrm, iSeg) && (double) oNrm.Y >= 0.699999988079071)
    {
      Elements allElements = iDamage.GetAllElements();
      int num = 0;
      for (int index = 0; index < 11; ++index)
      {
        Elements elements = (Elements) (1 << index);
        if ((elements & allElements) == elements)
        {
          EffectManager.Instance.StartEffect(UnderGroundAttack.EFFECTS[index], ref this.mPosition, ref this.mDirection, out this.mEffect[num++]);
          this.mCues[index] = AudioManager.Instance.GetCue(Banks.Spells, UnderGroundAttack.SFX[index]);
          this.mCues[index].Apply3D(this.mPlayState.Camera.Listener, this.mAE);
          this.mCues[index].Play();
        }
      }
      SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    }
    else
      UnderGroundAttack.sCache.Add(this);
  }

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    Vector2 result;
    Vector2.Multiply(ref this.mVelocity, iDeltaTime, out result);
    Segment iSeg1 = new Segment();
    iSeg1.Origin.X = this.mPosition.X + result.X;
    iSeg1.Origin.Y = this.mPosition.Y + 1f;
    iSeg1.Origin.Z = this.mPosition.Z + result.Y;
    iSeg1.Delta.Y = -2f;
    this.mAE.Position = this.mPosition;
    Vector3 mPosition = this.mPosition;
    float oFrac;
    Vector3 oNrm;
    if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out oFrac, out this.mPosition, out oNrm, iSeg1) && (double) oNrm.Y >= 0.699999988079071)
    {
      Segment iSeg2;
      iSeg2.Origin = mPosition;
      Vector3.Subtract(ref this.mPosition, ref mPosition, out iSeg2.Delta);
      List<Shield> shields = this.mPlayState.EntityManager.Shields;
      bool flag = true;
      for (int index = 0; index < shields.Count; ++index)
      {
        if (shields[index].SegmentIntersect(out Vector3 _, iSeg2, 1f))
        {
          this.mPosition = mPosition;
          flag = false;
          this.Kill();
          break;
        }
      }
      if (flag)
      {
        ++iSeg2.Origin.Y;
        if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out oFrac, out Vector3 _, out oNrm, iSeg2))
        {
          this.mPosition = mPosition;
          flag = false;
          this.Kill();
        }
      }
      if (!flag)
        return;
      for (int index = 0; index < this.mEffect.Length; ++index)
        EffectManager.Instance.UpdatePositionDirection(ref this.mEffect[index], ref this.mPosition, ref this.mDirection);
      this.mRange -= result.Length();
      List<Entity> entities = this.mPlayState.EntityManager.GetEntities(this.mPosition, 1f, true);
      for (int index = 0; index < entities.Count; ++index)
      {
        if (entities[index] is IDamageable t && !this.mHitList.Contains(t.Handle))
        {
          this.mHitList.Add(t.Handle);
          int num = (int) t.Damage(this.mDamage, this.mOwner as Entity, this.mTimeStamp, this.mPosition);
        }
      }
      this.mPlayState.EntityManager.ReturnEntityList(entities);
    }
    else
    {
      this.mPosition = mPosition;
      this.Kill();
    }
  }

  public void Kill() => this.mRange = 0.0f;

  public bool IsDead => (double) this.mRange <= 0.0;

  public void OnRemove()
  {
    for (int index = 0; index < this.mEffect.Length; ++index)
      EffectManager.Instance.Stop(ref this.mEffect[index]);
    for (int index = 0; index < 11; ++index)
    {
      if (this.mCues[index] != null)
      {
        if (!this.mCues[index].IsStopped || !this.mCues[index].IsStopping)
          this.mCues[index].Stop(AudioStopOptions.AsAuthored);
        this.mCues[index] = (Cue) null;
      }
    }
    UnderGroundAttack.sCache.Add(this);
  }
}
