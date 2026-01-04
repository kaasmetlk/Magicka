// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Spells.SpellEffects.SpraySpell
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.Graphics;
using Magicka.Graphics.Lights;
using Magicka.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PolygonHead;
using PolygonHead.Lights;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Spells.SpellEffects;

internal class SpraySpell : SpellEffect
{
  private static List<SpraySpell> mCache;
  public static readonly int[] SPRAY_EFFECTS;
  public static readonly int[] SWORD_EFFECTS;
  private static readonly int FIRE_SCOURGE_EFFECT = "fire_scourge".GetHashCodeCustom();
  private float mTTL;
  private HitList mHitList;
  private float mAngle;
  private float mRange;
  private float mRangeMultiplier;
  private DamageCollection5 mDamageCollection;
  private List<VisualEffectReference> mVisualEffects;
  private SpraySpell.DecalTypes mDecalType;
  private Dictionary<int, DecalManager.DecalReference[]> mSprayDecals;
  private Dictionary<int, VisualEffectReference[]> mScourgeEffects;
  private ISpellCaster mOwner;
  private DynamicLight mPointLight;
  private Scene mScene;

  public static void IntializeCache(int iNum)
  {
    SpraySpell.mCache = new List<SpraySpell>(iNum);
    for (int index = 0; index < iNum; ++index)
      SpraySpell.mCache.Add(new SpraySpell());
  }

  public static SpellEffect GetFromCache()
  {
    SpraySpell fromCache = SpraySpell.mCache[SpraySpell.mCache.Count - 1];
    SpraySpell.mCache.Remove(fromCache);
    SpellEffect.mPlayState.SpellEffects.Add((SpellEffect) fromCache);
    return (SpellEffect) fromCache;
  }

  public static void ReturnToCache(SpraySpell iEffect)
  {
    iEffect.mHitList.Clear();
    foreach (Cue mSpellCue in iEffect.mSpellCues)
    {
      if (!mSpellCue.IsStopped || !mSpellCue.IsStopping)
        mSpellCue.Stop(AudioStopOptions.AsAuthored);
    }
    iEffect.mSpellCues.Clear();
    SpellEffect.mPlayState.SpellEffects.Remove((SpellEffect) iEffect);
    SpraySpell.mCache.Add(iEffect);
  }

  static SpraySpell()
  {
    SpraySpell.SPRAY_EFFECTS = new int[11];
    SpraySpell.SWORD_EFFECTS = new int[11];
    for (int iIndex = 0; iIndex < 11; ++iIndex)
    {
      string str;
      string iString;
      switch (Spell.ElementFromIndex(iIndex))
      {
        case Elements.Water:
          str = "spray_water";
          iString = "weapon_spray_water";
          break;
        case Elements.Cold:
          str = "spray_cold";
          iString = "weapon_spray_cold";
          break;
        case Elements.Fire:
          str = "spray_fire";
          iString = "weapon_spray_fire";
          break;
        case Elements.Lightning:
          str = "spray_lightning";
          iString = "weapon_spray_lightning";
          break;
        case Elements.Steam:
          str = "spray_steam";
          iString = "weapon_spray_steam";
          break;
        case Elements.Poison:
          str = "spray_poison";
          iString = "weapon_spray_poison";
          break;
        default:
          str = "";
          iString = "";
          break;
      }
      SpraySpell.SPRAY_EFFECTS[iIndex] = (str + (object) 1).GetHashCodeCustom();
      SpraySpell.SWORD_EFFECTS[iIndex] = iString.GetHashCodeCustom();
    }
  }

  public SpraySpell()
  {
    this.mVisualEffects = new List<VisualEffectReference>(8);
    this.mHitList = new HitList(32 /*0x20*/);
    this.mSpellCues = new List<Cue>(5);
    this.mSprayDecals = new Dictionary<int, DecalManager.DecalReference[]>(24);
    this.mScourgeEffects = new Dictionary<int, VisualEffectReference[]>(24);
    for (int key = 0; key < 24; ++key)
    {
      DecalManager.DecalReference[] decalReferenceArray = new DecalManager.DecalReference[4];
      for (int index = 0; index < decalReferenceArray.Length; ++index)
        decalReferenceArray[index].Index = -1;
      this.mSprayDecals.Add(key, decalReferenceArray);
      VisualEffectReference[] visualEffectReferenceArray = new VisualEffectReference[2];
      for (int index = 0; index < visualEffectReferenceArray.Length; ++index)
        visualEffectReferenceArray[index].ID = -1;
      this.mScourgeEffects.Add(key, visualEffectReferenceArray);
    }
  }

  public override void CastArea(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
  {
    base.CastArea(iSpell, iOwner, iFromStaff);
    this.mCastType = CastType.Area;
    this.mOwner = iOwner;
    this.mTimeStamp = iOwner.PlayState.PlayTime;
    this.mMaxTTL = this.mMinTTL;
    this.mAngle = 6.28318548f;
    this.mRange = iSpell.BlastSize() * 10f;
    this.mRangeMultiplier = 0.0f;
    this.mDamageCollection = new DamageCollection5();
    this.mSpell = iSpell;
    this.mSpell.CalculateDamage(SpellType.Spray, CastType.Area, out this.mDamageCollection);
    this.PlaySound(this.mSpell.GetSpellType(), this.mCastType, iOwner);
    int num1 = (int) Blast.FullBlast(iOwner.PlayState, iOwner as Entity, this.mTimeStamp, iOwner as Entity, this.mRange, iOwner.Position, this.mDamageCollection);
    if ((this.mDamageCollection.GetAllElements() & (Elements.Water | Elements.Steam)) == Elements.Water)
      this.mOwner.Electrocute((IDamageable) this.mOwner, 1f);
    if (!iOwner.HasStatus(StatusEffects.Greased))
      return;
    if ((this.mDamageCollection.A.Element & Elements.Fire) == Elements.Fire)
    {
      int num2 = (int) iOwner.Damage(this.mDamageCollection.A, iOwner as Entity, this.mTimeStamp, iOwner.CastSource.Translation);
    }
    else if ((this.mDamageCollection.B.Element & Elements.Fire) == Elements.Fire)
    {
      int num3 = (int) iOwner.Damage(this.mDamageCollection.B, iOwner as Entity, this.mTimeStamp, iOwner.CastSource.Translation);
    }
    else if ((this.mDamageCollection.C.Element & Elements.Fire) == Elements.Fire)
    {
      int num4 = (int) iOwner.Damage(this.mDamageCollection.C, iOwner as Entity, this.mTimeStamp, iOwner.CastSource.Translation);
    }
    else
    {
      if ((this.mDamageCollection.D.Element & Elements.Fire) != Elements.Fire)
        return;
      int num5 = (int) iOwner.Damage(this.mDamageCollection.D, iOwner as Entity, this.mTimeStamp, iOwner.CastSource.Translation);
    }
  }

  public override void CastForce(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
  {
    base.CastForce(iSpell, iOwner, iFromStaff);
    if (iOwner is Magicka.GameLogic.Entities.Character && !((iOwner as Magicka.GameLogic.Entities.Character).CurrentState is PanicCastState) && this.mFromStaff)
      this.mFromStaff = false;
    this.mCastType = CastType.Force;
    this.mOwner = iOwner;
    this.mTimeStamp = iOwner.PlayState.PlayTime;
    this.mTTL = this.mMaxTTL = 4f;
    this.mSpell = iSpell;
    this.mAngle = 0.314159274f;
    this.mRange = (float) (5.0 + (double) iSpell.TotalSprayMagnitude() / 5.0 * 10.0 / 2.0);
    this.mRangeMultiplier = 0.0f;
    this.mDamageCollection = new DamageCollection5();
    this.mSpell.CalculateDamage(SpellType.Spray, CastType.Force, out this.mDamageCollection);
    this.PlaySound(SpellType.Spray, CastType.Force, iOwner);
    Vector3 result1 = new Vector3();
    Vector3 result2 = Spell.ARCANECOLOR;
    Vector3.Multiply(ref result2, iSpell[Elements.Arcane], out result2);
    Vector3.Add(ref result2, ref result1, out result1);
    Vector3 result3 = Spell.COLDCOLOR;
    Vector3.Multiply(ref result3, iSpell[Elements.Cold], out result3);
    Vector3.Add(ref result3, ref result1, out result1);
    result3 = Spell.FIRECOLOR;
    Vector3.Multiply(ref result3, iSpell[Elements.Fire], out result3);
    Vector3.Add(ref result3, ref result1, out result1);
    result3 = Spell.LIFECOLOR;
    Vector3.Multiply(ref result3, iSpell[Elements.Life], out result3);
    Vector3.Add(ref result3, ref result1, out result1);
    result3 = Spell.LIGHTNINGCOLOR;
    Vector3.Multiply(ref result3, iSpell[Elements.Lightning], out result3);
    Vector3.Add(ref result3, ref result1, out result1);
    if ((double) result1.LengthSquared() > 1.4012984643248171E-45)
    {
      Vector3.Multiply(ref result1, 1.5f, out result1);
      Vector3.Multiply(ref result1, 0.25f, out result3);
      this.mScene = iOwner.PlayState.Scene;
      this.mPointLight = DynamicLight.GetCachedLight();
      this.mPointLight.SpecularAmount = result1.Length();
      this.mPointLight.DiffuseColor = result1;
      this.mPointLight.AmbientColor = result3;
      this.mPointLight.Position = iOwner.CastSource.Translation + iOwner.Direction * (this.mRange * 0.5f);
      this.mPointLight.Radius = this.mRange * 1.25f;
      this.mPointLight.Intensity = 1f;
      this.mPointLight.Enable(this.mScene, LightTransitionType.Linear, 0.5f);
    }
    if ((this.mSpell.Element & Elements.Fire) == Elements.Fire)
      this.mDecalType = SpraySpell.DecalTypes.Fire;
    else if ((this.mSpell.Element & Elements.Cold) == Elements.Cold)
      this.mDecalType = SpraySpell.DecalTypes.Cold;
    else if ((this.mSpell.Element & Elements.Water) == Elements.Water)
      this.mDecalType = SpraySpell.DecalTypes.Water;
    for (int iIndex = 0; iIndex < 11; ++iIndex)
    {
      if ((int) iSpell[Spell.ElementFromIndex(iIndex)] > 0)
      {
        int iHash = SpraySpell.SPRAY_EFFECTS[iIndex];
        Vector3 translation = iOwner.CastSource.Translation;
        Vector3 direction = iOwner.Direction;
        VisualEffectReference oRef;
        EffectManager.Instance.StartEffect(iHash, ref translation, ref direction, out oRef);
        this.mVisualEffects.Add(oRef);
      }
    }
    if (!iOwner.HasStatus(StatusEffects.Greased))
      return;
    if ((this.mDamageCollection.A.Element & Elements.Fire) == Elements.Fire)
    {
      int num1 = (int) iOwner.Damage(this.mDamageCollection.A, iOwner as Entity, this.mTimeStamp, iOwner.CastSource.Translation);
    }
    else if ((this.mDamageCollection.B.Element & Elements.Fire) == Elements.Fire)
    {
      int num2 = (int) iOwner.Damage(this.mDamageCollection.B, iOwner as Entity, this.mTimeStamp, iOwner.CastSource.Translation);
    }
    else if ((this.mDamageCollection.C.Element & Elements.Fire) == Elements.Fire)
    {
      int num3 = (int) iOwner.Damage(this.mDamageCollection.C, iOwner as Entity, this.mTimeStamp, iOwner.CastSource.Translation);
    }
    else
    {
      if ((this.mDamageCollection.D.Element & Elements.Fire) != Elements.Fire)
        return;
      int num4 = (int) iOwner.Damage(this.mDamageCollection.D, iOwner as Entity, this.mTimeStamp, iOwner.CastSource.Translation);
    }
  }

  public override void CastSelf(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
  {
    base.CastSelf(iSpell, iOwner, iFromStaff);
    this.mCastType = CastType.Self;
    this.mOwner = iOwner;
    this.mTimeStamp = iOwner.PlayState.PlayTime;
    this.mSpell = iSpell;
    this.mSpell.CalculateDamage(SpellType.Spray, CastType.Self, out this.mDamageCollection);
    this.mTTL = 1f;
    if ((this.mDamageCollection.GetAllElements() & (Elements.Water | Elements.Steam)) == Elements.Water)
      this.mOwner.Electrocute((IDamageable) this.mOwner, 1f);
    Vector3 position = iOwner.Position;
    Vector3 direction = iOwner.Direction;
    for (int iIndex = 0; iIndex < 11; ++iIndex)
    {
      if ((int) iSpell[Spell.ElementFromIndex(iIndex)] > 0)
      {
        VisualEffectReference oRef;
        EffectManager.Instance.StartEffect(SpellEffect.SelfCastEffectHash[iIndex], ref position, ref direction, out oRef);
        this.mVisualEffects.Add(oRef);
      }
    }
    int num = (int) iOwner.Damage(this.mDamageCollection, iOwner as Entity, this.mTimeStamp, iOwner.Position);
  }

  public override void CastWeapon(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
  {
    base.CastSelf(iSpell, iOwner, iFromStaff);
    this.mCastType = CastType.Weapon;
    this.mOwner = iOwner;
    this.mTimeStamp = iOwner.PlayState.PlayTime;
    this.mSpell = iSpell;
    this.mSpell.CalculateDamage(SpellType.Spray, CastType.Weapon, out this.mDamageCollection);
    this.mTTL = this.mMaxTTL;
    Vector3 translation = iOwner.WeaponSource.Translation;
    Vector3 direction = iOwner.Direction;
    for (int iIndex = 0; iIndex < 11; ++iIndex)
    {
      Elements iElement = Spell.ElementFromIndex(iIndex);
      if (((iElement & Elements.StatusEffects) != Elements.None || (iElement & Elements.Steam) != Elements.None) && (int) iSpell[iElement] > 0)
      {
        VisualEffectReference oRef;
        EffectManager.Instance.StartEffect(SpraySpell.SWORD_EFFECTS[iIndex], ref translation, ref direction, out oRef);
        this.mVisualEffects.Add(oRef);
      }
    }
    int num1 = (int) Helper.ArcDamage(iOwner.PlayState, iOwner as Entity, this.mTimeStamp, iOwner as Entity, ref translation, ref direction, 6f, 1.57079637f, ref this.mDamageCollection);
    if (!iOwner.HasStatus(StatusEffects.Greased))
      return;
    if ((this.mDamageCollection.A.Element & Elements.Fire) == Elements.Fire)
    {
      int num2 = (int) iOwner.Damage(this.mDamageCollection.A, iOwner as Entity, this.mTimeStamp, iOwner.CastSource.Translation);
    }
    else if ((this.mDamageCollection.B.Element & Elements.Fire) == Elements.Fire)
    {
      int num3 = (int) iOwner.Damage(this.mDamageCollection.B, iOwner as Entity, this.mTimeStamp, iOwner.CastSource.Translation);
    }
    else if ((this.mDamageCollection.C.Element & Elements.Fire) == Elements.Fire)
    {
      int num4 = (int) iOwner.Damage(this.mDamageCollection.C, iOwner as Entity, this.mTimeStamp, iOwner.CastSource.Translation);
    }
    else
    {
      if ((this.mDamageCollection.D.Element & Elements.Fire) != Elements.Fire)
        return;
      int num5 = (int) iOwner.Damage(this.mDamageCollection.D, iOwner as Entity, this.mTimeStamp, iOwner.CastSource.Translation);
    }
  }

  private void Execute(ISpellCaster iOwner, float iRange)
  {
    Vector3 iDirection;
    Vector3 translation;
    if (this.CastType == CastType.Weapon)
    {
      iDirection = iOwner.WeaponSource.Forward;
      translation = iOwner.WeaponSource.Translation;
    }
    else if (this.mFromStaff)
    {
      iDirection = iOwner.CastSource.Forward;
      translation = iOwner.CastSource.Translation;
    }
    else
    {
      iDirection = iOwner.Direction;
      translation = iOwner.CastSource.Translation;
    }
    Segment segment;
    segment.Origin = translation;
    Vector3.Multiply(ref iDirection, iRange, out segment.Delta);
    float num1 = iRange * 0.1f;
    float scaleFactor;
    Vector3 vector3;
    if (iOwner.PlayState.Level.CurrentScene.SegmentIntersect(out scaleFactor, out vector3, out Vector3 _, segment))
    {
      num1 *= scaleFactor;
      Vector3.Multiply(ref segment.Delta, scaleFactor, out segment.Delta);
    }
    List<Shield> shields = iOwner.PlayState.EntityManager.Shields;
    for (int index = 0; index < shields.Count; ++index)
    {
      if (shields[index].Body.CollisionSkin.SegmentIntersect(out scaleFactor, out vector3, out vector3, segment))
      {
        num1 *= scaleFactor;
        Vector3.Multiply(ref segment.Delta, scaleFactor, out segment.Delta);
      }
    }
    Vector3.Multiply(ref iDirection, num1 * iRange, out segment.Delta);
    bool flag = (this.mDamageCollection.GetAllElements() & (Elements.Water | Elements.Steam)) == Elements.Water;
    if (flag)
      this.mOwner.Electrocute((IDamageable) this.mOwner, 0.25f);
    List<Entity> entities = iOwner.PlayState.EntityManager.GetEntities(iOwner.Position, iRange, true);
    entities.Remove(iOwner as Entity);
    DamageCollection5 damageCollection = this.mDamageCollection;
    damageCollection.MultiplyMagnitude(0.25f);
    for (int index = 0; index < entities.Count; ++index)
    {
      Entity entity = entities[index];
      Vector3 oPosition;
      if (entity is IDamageable t && !this.mHitList.ContainsKey(entity.Handle) && (t is Shield && t.ArcIntersect(out oPosition, segment.Origin, iDirection, iRange, this.mAngle, 5f) || t.ArcIntersect(out oPosition, segment.Origin, iDirection, iRange * num1, this.mAngle, 5f)))
      {
        int num2 = (int) t.Damage(damageCollection, iOwner as Entity, this.mTimeStamp, segment.Origin);
        this.mHitList.Add(entity.Handle, 0.25f);
        if (flag)
          t.Electrocute((IDamageable) this.mOwner, 0.25f);
      }
    }
    iOwner.PlayState.EntityManager.ReturnEntityList(entities);
  }

  public override bool CastUpdate(float iDeltaTime, ISpellCaster iOwner, out float oTurnSpeed)
  {
    this.mHitList.Update(iDeltaTime);
    oTurnSpeed = 0.6f;
    Vector3 position1 = iOwner.Position;
    Vector3 result1;
    Vector3 translation1;
    if (this.CastType == CastType.Weapon)
    {
      result1 = iOwner.WeaponSource.Forward;
      translation1 = iOwner.WeaponSource.Translation;
    }
    else if (this.mFromStaff)
    {
      result1 = iOwner.CastSource.Forward;
      translation1 = iOwner.CastSource.Translation;
    }
    else
    {
      result1 = iOwner.Direction;
      translation1 = iOwner.CastSource.Translation;
    }
    if (iOwner is Avatar avatar && avatar.Player.Controller != null)
      avatar.Player.Controller.Rumble(0.8f, 0.8f);
    if (this.mCastType != CastType.Self)
    {
      if (this.mCastType == CastType.Force)
      {
        this.mTTL -= iDeltaTime;
        this.mRangeMultiplier += iDeltaTime * 4f;
        this.mRangeMultiplier = Math.Min(this.mRangeMultiplier, 1f);
        if ((double) this.mTTL > 0.0)
          this.Execute(iOwner, this.mRange * this.mRangeMultiplier);
        Vector3 vector3_1 = !this.mFromStaff ? iOwner.CastSource.Translation : iOwner.WeaponSource.Translation;
        Vector3 up = Vector3.Up;
        Matrix result2;
        Matrix.CreateWorld(ref vector3_1, ref result1, ref up, out result2);
        float iScale1 = this.mRange;
        if ((double) this.mAngle <= 0.5)
        {
          Segment segment;
          segment.Origin = vector3_1;
          Vector3.Multiply(ref result1, this.mRange, out segment.Delta);
          iScale1 = this.mRange * 0.1f;
          float scaleFactor;
          Vector3 vector3_2;
          Vector3 vector3_3;
          if (iOwner.PlayState.Level.CurrentScene.SegmentIntersect(out scaleFactor, out vector3_2, out vector3_3, segment))
          {
            iScale1 *= scaleFactor;
            Vector3.Multiply(ref segment.Delta, scaleFactor, out segment.Delta);
          }
          List<Shield> shields = iOwner.PlayState.EntityManager.Shields;
          for (int index = 0; index < shields.Count; ++index)
          {
            if (shields[index].Body.CollisionSkin.SegmentIntersect(out scaleFactor, out vector3_2, out vector3_2, segment))
            {
              iScale1 *= scaleFactor;
              Vector3.Multiply(ref segment.Delta, scaleFactor, out segment.Delta);
            }
          }
          List<Entity> entities = iOwner.PlayState.EntityManager.GetEntities(segment.Origin, segment.Delta.Length(), false);
          for (int index = 0; index < entities.Count; ++index)
          {
            IDamageable damageable = entities[index] as IDamageable;
            if (damageable is JormungandrCollisionZone && damageable.Body.CollisionSkin.SegmentIntersect(out scaleFactor, out vector3_2, out vector3_3, segment))
            {
              iScale1 *= scaleFactor * 1.5f;
              Vector3.Multiply(ref segment.Delta, scaleFactor, out segment.Delta);
              break;
            }
          }
          iOwner.PlayState.EntityManager.ReturnEntityList(entities);
          MagickaMath.UniformMatrixScale(ref result2, iScale1);
          if (this.mPointLight != null && this.mPointLight.Enabled)
          {
            Vector3 position2 = new Vector3();
            Vector3 result3 = this.mPointLight.Position;
            Vector3.Subtract(ref result3, ref vector3_1, out result3);
            Matrix result4;
            Matrix.CreateWorld(ref position2, ref result3, ref up, out result4);
            Quaternion result5;
            Quaternion.CreateFromRotationMatrix(ref result4, out result5);
            Matrix orientation = iOwner.Body.Orientation;
            Quaternion result6;
            Quaternion.CreateFromRotationMatrix(ref orientation, out result6);
            Quaternion.Lerp(ref result5, ref result6, MathHelper.Clamp(iDeltaTime * 4f, 0.0f, 1f), out result6);
            Vector3 result7 = new Vector3();
            result7.Z = -1f;
            Vector3.Transform(ref result7, ref result6, out result7);
            Vector3.Multiply(ref result7, this.mRange * 0.5f, out result7);
            Vector3 result8;
            Vector3.Add(ref result7, ref vector3_1, out result8);
            this.mPointLight.Position = result8;
          }
        }
        if ((this.mSpell.Element & Elements.StatusEffects) != Elements.None)
        {
          float num1 = MagickaMath.Angle(new Vector2(result1.X, result1.Z));
          if ((double) num1 < 0.0)
            num1 += 6.28318548f;
          float oAngle = num1;
          int oKey;
          this.NormalizeAngle(ref oAngle, out oKey);
          DecalManager.DecalReference[] mSprayDecal = this.mSprayDecals[oKey];
          VisualEffectReference[] mScourgeEffect = this.mScourgeEffects[oKey];
          for (int index = 0; index < 4; ++index)
          {
            if (mSprayDecal[index].Index >= 0)
              DecalManager.Instance.AddAlphaBlendedDecalAlpha(ref mSprayDecal[index], iDeltaTime * 0.5f * (float) (index + 1));
            else if (mSprayDecal[index].Index == -1)
            {
              Vector3 translation2 = iOwner.CastSource.Translation;
              float num2 = (float) ((double) iScale1 * 10.0 + 2.0);
              Segment iSeg = new Segment();
              iSeg.Delta.Y -= 2f;
              Vector2 iScale2 = new Vector2();
              iScale2.X = iScale2.Y = Math.Max(2.5f, (float) (1 + index) * iScale1);
              Vector3 result9;
              Vector3.Multiply(ref result1, (float) ((double) num2 * 0.20000000298023224 + (double) index * 0.20000000298023224 * (double) num2 * 0.60000002384185791), out result9);
              Vector3 result10;
              Vector3.Add(ref translation2, ref result9, out result10);
              float result11 = 0.0f;
              Vector3.Distance(ref result10, ref vector3_1, out result11);
              if ((double) result11 < (double) num2 - 1.0)
              {
                Matrix identity = Matrix.Identity;
                iSeg.Origin = result10;
                iSeg.Origin.X += (float) ((MagickaMath.Random.NextDouble() - 0.5) * (double) iScale2.X * 0.125) * (float) index;
                iSeg.Origin.Y += (float) ((MagickaMath.Random.NextDouble() - 0.5) * (double) iScale2.Y * 0.125) * (float) index;
                Vector3 oPos;
                Vector3 oNrm;
                AnimatedLevelPart oAnimatedLevelPart;
                if (SpellEffect.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out oNrm, out oAnimatedLevelPart, iSeg))
                {
                  Vector3 vector3_4 = oPos;
                  vector3_4.Y += 0.5f;
                  identity.Translation = vector3_4;
                  DecalManager.Instance.AddAlphaBlendedDecal((Decal) (20 + this.mDecalType), oAnimatedLevelPart, ref iScale2, ref oPos, new Vector3?(), ref oNrm, 60f, 0.0f, out mSprayDecal[index]);
                  if (index > 1 && (double) this.mSpell[Elements.Fire] >= 1.0 && !EffectManager.Instance.IsActive(ref mScourgeEffect[index - 2]))
                    EffectManager.Instance.StartEffect(SpraySpell.FIRE_SCOURGE_EFFECT, ref identity, out mScourgeEffect[index - 2]);
                }
                else
                  mSprayDecal[index].Index = -2;
              }
              else
                mSprayDecal[index].Index = -2;
            }
          }
          if ((double) this.mSpell[Elements.Fire] >= 1.0)
          {
            for (int index = 0; index < 2; ++index)
              EffectManager.Instance.RestartEffect(ref mScourgeEffect[index]);
          }
        }
        for (int index = 0; index < this.mVisualEffects.Count; ++index)
        {
          VisualEffectReference mVisualEffect = this.mVisualEffects[index];
          EffectManager.Instance.UpdateOrientation(ref mVisualEffect, ref result2);
          this.mVisualEffects[index] = mVisualEffect;
        }
        Vector3.Multiply(ref result1, this.mRange, out result1);
        Liquid.Freeze(iOwner.PlayState.Level.CurrentScene, ref vector3_1, ref result1, this.mAngle, iDeltaTime, ref this.mDamageCollection);
      }
      else
      {
        Vector3 direction = iOwner.Direction;
        for (int index = 0; index < this.mVisualEffects.Count; ++index)
        {
          VisualEffectReference mVisualEffect = this.mVisualEffects[index];
          EffectManager.Instance.UpdatePositionDirection(ref mVisualEffect, ref translation1, ref direction);
          this.mVisualEffects[index] = mVisualEffect;
        }
      }
    }
    if ((double) this.mTTL > 0.0 && (double) this.mMaxTTL > 0.0)
      return base.CastUpdate(iDeltaTime, iOwner, out float _);
    this.DeInitialize(iOwner);
    return false;
  }

  public override void DeInitialize(ISpellCaster iOwner)
  {
    if (!this.Active)
      return;
    this.Active = false;
    foreach (DecalManager.DecalReference[] decalReferenceArray in this.mSprayDecals.Values)
    {
      foreach (DecalManager.DecalReference decalReference in decalReferenceArray)
        decalReference.Index = -1;
    }
    foreach (VisualEffectReference[] visualEffectReferenceArray in this.mScourgeEffects.Values)
    {
      foreach (VisualEffectReference visualEffectReference in visualEffectReferenceArray)
        visualEffectReference.ID = -1;
    }
    if (this.mOwner is Avatar mOwner && mOwner.Player.Controller != null)
      mOwner.Player.Controller.Rumble(0.8f, 0.8f);
    if (this.mPointLight != null && this.mPointLight.Enabled)
    {
      this.mPointLight.Disable(LightTransitionType.Linear, 0.5f);
      this.mPointLight = (DynamicLight) null;
    }
    this.mHitList.Clear();
    for (int index = 0; index < this.mVisualEffects.Count; ++index)
    {
      VisualEffectReference mVisualEffect = this.mVisualEffects[index];
      EffectManager.Instance.Stop(ref mVisualEffect);
    }
    foreach (Cue mSpellCue in this.mSpellCues)
    {
      if (!mSpellCue.IsStopping || !mSpellCue.IsStopped)
        mSpellCue.Stop(AudioStopOptions.AsAuthored);
    }
    SpraySpell.ReturnToCache(this);
  }

  public void NormalizeAngle(ref float oAngle, out int oKey)
  {
    oAngle *= 3.8197186f;
    oAngle = (float) Math.Floor((double) oAngle);
    oKey = (int) oAngle;
    oAngle *= 0.2617994f;
  }

  private enum DecalTypes
  {
    Fire = 0,
    Cold = 2,
    Water = 3,
  }
}
