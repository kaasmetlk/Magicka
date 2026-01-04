// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Spells.SpellEffects.PushSpell
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.Audio;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.Graphics;
using Magicka.Graphics.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Spells.SpellEffects;

public class PushSpell : SpellEffect
{
  private static List<PushSpell> mCache;
  private static int PUSH_FORCE = "push_force".GetHashCodeCustom();
  private static int PUSH_AREA = "push_area".GetHashCodeCustom();
  private static string PUSH_VARIABLE = "Magnitude";
  private static int PUSH_STR_FORCE = "spell_push_force".GetHashCodeCustom();
  private static int PUSH_STR_AREA = "spell_push_area".GetHashCodeCustom();
  private List<Entity> mHitList;
  private float mRange;
  private float mTime;
  private float mAngle;
  private float mElevation;
  private float mForce;
  private Vector3 mInitialDirection;
  private ISpellCaster mOwner;
  private Damage mDamage;
  private new double mTimeStamp;

  public static void IntializeCache(int iNum)
  {
    PushSpell.mCache = new List<PushSpell>(iNum);
    for (int index = 0; index < iNum; ++index)
      PushSpell.mCache.Add(new PushSpell());
  }

  public static SpellEffect GetFromCache()
  {
    if (PushSpell.mCache.Count <= 0)
      return (SpellEffect) new PushSpell();
    PushSpell fromCache = PushSpell.mCache[PushSpell.mCache.Count - 1];
    PushSpell.mCache.Remove(fromCache);
    SpellEffect.mPlayState.SpellEffects.Add((SpellEffect) fromCache);
    return (SpellEffect) fromCache;
  }

  public static void ReturnToCache(PushSpell iEffect)
  {
    iEffect.mHitList.Clear();
    SpellEffect.mPlayState.SpellEffects.Remove((SpellEffect) iEffect);
    PushSpell.mCache.Add(iEffect);
  }

  public PushSpell() => this.mHitList = new List<Entity>(256 /*0x0100*/);

  public override void CastArea(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
  {
    base.CastArea(iSpell, iOwner, iFromStaff);
    this.mOwner = iOwner;
    this.mTimeStamp = iOwner.PlayState.PlayTime;
    this.mMinTTL = 0.4f;
    this.mForce = (float) (50.0 + 500.0 * (double) iOwner.SpellPower);
    this.mElevation = iOwner.SpellPower * 0.1f;
    this.mDamage = new Damage(AttackProperties.Pushed, Elements.Earth, this.mForce, 1.5f);
    this.Active = true;
    this.mAngle = 3.14159274f;
    this.mRange = 3.5f;
    this.mTime = 0.25f;
    RadialBlur radialBlur = RadialBlur.GetRadialBlur();
    this.mInitialDirection = iOwner.Direction;
    Vector3 position1 = iOwner.Position;
    radialBlur.Initialize(ref position1, ref this.mInitialDirection, this.mAngle, this.mRange, 0.5f, iOwner.PlayState.Scene);
    Vector3 position2 = iOwner.Position;
    Vector3 direction = iOwner.Direction;
    EffectManager.Instance.StartEffect(PushSpell.PUSH_AREA, ref position2, ref direction, out VisualEffectReference _);
    AudioManager.Instance.PlayCue<PushSpell.PushMagnitudeVolumAdjustAndOthers>(Banks.Spells, PushSpell.PUSH_STR_AREA, new PushSpell.PushMagnitudeVolumAdjustAndOthers()
    {
      magnitude = iOwner.SpellPower
    }, iOwner.AudioEmitter);
    iOwner.SpellPower = 0.0f;
  }

  public override void CastForce(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
  {
    base.CastForce(iSpell, iOwner, iFromStaff);
    this.mOwner = iOwner;
    this.mTimeStamp = iOwner.PlayState.PlayTime;
    if (!((iOwner as Character).CurrentState is PanicCastState) && iOwner is Character && this.mFromStaff)
      this.mFromStaff = false;
    this.mMinTTL = 0.4f;
    this.mForce = (float) (50.0 + 500.0 * (double) iOwner.SpellPower);
    this.mElevation = iOwner.SpellPower * 0.1f;
    this.mDamage = new Damage(AttackProperties.Pushed, Elements.Earth, this.mForce, 1.5f);
    this.Active = true;
    this.mAngle = 0.5235988f;
    this.mRange = 10f;
    this.mTime = 0.25f;
    RadialBlur radialBlur = RadialBlur.GetRadialBlur();
    this.mInitialDirection = iOwner.Direction;
    Vector3 position = iOwner.Position;
    radialBlur.Initialize(ref position, ref this.mInitialDirection, this.mAngle + this.mAngle * 0.2f, this.mRange, 0.5f, iOwner.PlayState.Scene);
    Vector3 translation = iOwner.CastSource.Translation;
    Vector3 direction = iOwner.Direction;
    if (EffectManager.Instance.StartEffect(PushSpell.PUSH_FORCE, ref translation, ref direction, out VisualEffectReference _))
      AudioManager.Instance.PlayCue<PushSpell.PushMagnitudeVolumAdjustAndOthers>(Banks.Spells, PushSpell.PUSH_STR_FORCE, new PushSpell.PushMagnitudeVolumAdjustAndOthers()
      {
        magnitude = iOwner.SpellPower
      }, iOwner.AudioEmitter);
    iOwner.SpellPower = 0.0f;
  }

  public override void CastSelf(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
  {
    base.CastSelf(iSpell, iOwner, iFromStaff);
    this.mOwner = iOwner;
    this.mTimeStamp = iOwner.PlayState.PlayTime;
    this.mMinTTL = 0.4f;
  }

  public override void CastWeapon(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
  {
    base.CastWeapon(iSpell, iOwner, iFromStaff);
    this.mOwner = iOwner;
    this.mTimeStamp = iOwner.PlayState.PlayTime;
    this.mMinTTL = 0.4f;
  }

  public override bool CastUpdate(float iDeltaTime, ISpellCaster iOwner, out float oTurnSpeed)
  {
    oTurnSpeed = 0.0f;
    this.mTime -= iDeltaTime;
    this.mMinTTL -= iDeltaTime;
    if ((double) this.mTime > 0.0)
    {
      float num1 = (float) ((1.0 - (double) MathHelper.Clamp(this.mTime, 0.0f, 0.25f) / 0.25) * (double) this.mRange * 0.75 + (double) this.mRange * 0.25);
      List<Entity> entities = iOwner.PlayState.EntityManager.GetEntities(iOwner.Position, num1, true);
      entities.Remove(iOwner as Entity);
      Vector3 position = iOwner.Position;
      Segment iSeg;
      iSeg.Origin = position;
      for (int index = 0; index < entities.Count; ++index)
      {
        Entity t = entities[index];
        if (!this.mHitList.Contains(t) && !t.Dead && ((double) this.mAngle == 3.1415927410125732 || t.ArcIntersect(out Vector3 _, position, this.mInitialDirection, num1, this.mAngle, 4f)))
        {
          iSeg.Delta = t.Position;
          Vector3.Subtract(ref iSeg.Delta, ref iSeg.Origin, out iSeg.Delta);
          if (!iOwner.PlayState.Level.CurrentScene.SegmentIntersect(out float _, out Vector3 _, out Vector3 _, iSeg))
          {
            Vector3 result = t.Position - iOwner.Position;
            float num2 = result.Length();
            if ((double) num2 > 1.4012984643248171E-45)
            {
              result.Y = 0.0f;
              Vector3.Divide(ref result, num2, out result);
              float iDistance = 1.5f;
              this.mHitList.Add(t);
              if (t is IDamageable && !(t is MissileEntity))
              {
                int num3 = (int) (t as IDamageable).Damage(this.mDamage, this.mOwner as Entity, this.mTimeStamp, this.mOwner.Position);
              }
              else
                t.AddImpulseVelocity(result, (float) (0.17453292012214661 + (double) this.mElevation * 0.78539818525314331 * 0.5), this.mForce, iDistance);
            }
          }
        }
      }
      iOwner.PlayState.EntityManager.ReturnEntityList(entities);
    }
    else if ((double) this.mMinTTL < 0.0)
    {
      this.DeInitialize(iOwner);
      return false;
    }
    return true;
  }

  public override void DeInitialize(ISpellCaster iOwner)
  {
    if (!this.Active)
      return;
    this.Active = false;
    PushSpell.ReturnToCache(this);
  }

  public struct PushMagnitudeVolumAdjustAndOthers : IAudioVariables
  {
    public float magnitude;

    public void AssignToCue(Cue iCue)
    {
      iCue.SetVariable(PushSpell.PUSH_VARIABLE, this.magnitude * 3f);
    }
  }
}
