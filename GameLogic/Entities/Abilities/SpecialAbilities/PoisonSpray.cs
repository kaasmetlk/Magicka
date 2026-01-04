// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.PoisonSpray
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PolygonHead;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class PoisonSpray(Magicka.Animations iAnimation) : SpecialAbility(iAnimation, Helper.GetHashCodeCustom("#specab_poison")), IAbilityEffect
{
  public const float GREASESPRAY_TTL = 0.5f;
  public const int NR_OF_FIELDS = 6;
  public const float NR_OF_FIELDS_F = 6f;
  public static readonly int EFFECT = "scythe_spray".GetHashCodeCustom();
  public static readonly int SOUNDHASH = "spell_poison_spray".GetHashCodeCustom();
  private ISpellCaster mOwner;
  private PlayState mPlayState;
  private VisualEffectReference mEffect;
  private Cue mCue;
  private float mTTL;

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    this.mOwner = iOwner;
    if (this.mOwner == null)
      throw new Exception("Grease can not be cast without an owner!");
    this.mPlayState = iPlayState;
    this.mTTL = 0.5f;
    this.mCue = AudioManager.Instance.PlayCue(Banks.Spells, PoisonSpray.SOUNDHASH, this.mOwner.AudioEmitter);
    Vector3 translation = iOwner.CastSource.Translation;
    Vector3 direction = iOwner.Direction;
    EffectManager.Instance.StartEffect(PoisonSpray.EFFECT, ref translation, ref direction, out this.mEffect);
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    return true;
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState) => false;

  public bool IsDead => (double) this.mTTL <= 0.0;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mTTL -= iDeltaTime;
    Vector3 position = this.mOwner.Position;
    Vector3 result1 = this.mOwner.Direction;
    Quaternion result2;
    Quaternion.CreateFromYawPitchRoll((float) (-((double) this.mTTL / 0.5 * 6.0 - 2.5) / 2.5 * 0.78539818525314331), 0.0f, 0.0f, out result2);
    float num1 = 6f;
    Segment segment;
    segment.Origin = position;
    Vector3.Transform(ref result1, ref result2, out result1);
    Vector3.Multiply(ref result1, num1, out segment.Delta);
    if (NetworkManager.Instance.State != NetworkState.Client)
    {
      GameScene currentScene = this.mOwner.PlayState.Level.CurrentScene;
      List<Shield> shields = this.mOwner.PlayState.EntityManager.Shields;
      float scaleFactor;
      Vector3 vector3_1;
      Vector3 vector3_2;
      for (int index = 0; index < shields.Count; ++index)
      {
        if (shields[index].Body.CollisionSkin.SegmentIntersect(out scaleFactor, out vector3_1, out vector3_2, segment))
        {
          num1 *= scaleFactor;
          Vector3.Multiply(ref segment.Delta, scaleFactor, out segment.Delta);
        }
      }
      if (currentScene.SegmentIntersect(out scaleFactor, out vector3_1, out vector3_2, segment))
      {
        num1 *= scaleFactor;
        Vector3.Multiply(ref segment.Delta, scaleFactor, out segment.Delta);
      }
      List<Entity> entities = this.mPlayState.EntityManager.GetEntities(position, num1, true);
      entities.Remove(this.mOwner as Entity);
      for (int index = 0; index < entities.Count; ++index)
      {
        Vector3 oPosition;
        if (entities[index] is Magicka.GameLogic.Entities.Character t && !t.HasStatus(StatusEffects.Greased) && t.ArcIntersect(out oPosition, segment.Origin, result1, num1, 0.17453292f, 5f))
        {
          int num2 = (int) t.Damage(new Damage(AttackProperties.Status, Elements.Poison, 75f, 1f), this.mOwner as Entity, this.mTimeStamp, oPosition);
        }
      }
      this.mPlayState.EntityManager.ReturnEntityList(entities);
    }
    Vector3 translation = this.mOwner.CastSource.Translation;
    EffectManager.Instance.UpdatePositionDirection(ref this.mEffect, ref translation, ref result1);
  }

  public void OnRemove()
  {
    EffectManager.Instance.Stop(ref this.mEffect);
    if (!this.mCue.IsPlaying)
      return;
    this.mCue.Stop(AudioStopOptions.AsAuthored);
  }
}
