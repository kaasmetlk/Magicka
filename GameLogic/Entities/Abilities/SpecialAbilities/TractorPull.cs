// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.TractorPull
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.Graphics.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

internal class TractorPull : SpecialAbility
{
  private const float RADIUS = 15f;
  private static TractorPull sSingelton;
  private static volatile object sSingeltonLock = new object();
  private static readonly int SOUND_EFFECT = "spell_push_area".GetHashCodeCustom();
  private static readonly int MAGICK_EFFECT = "magick_tractorpull".GetHashCodeCustom();
  private ISpellCaster mSpellCaster;
  private static AudioEmitter sAudioEmitter;

  public static TractorPull Instance
  {
    get
    {
      if (TractorPull.sSingelton == null)
      {
        lock (TractorPull.sSingeltonLock)
        {
          if (TractorPull.sSingelton == null)
            TractorPull.sSingelton = new TractorPull();
        }
      }
      return TractorPull.sSingelton;
    }
  }

  private TractorPull()
    : base(Magicka.Animations.cast_area_ground, "#magick_tractorpull".GetHashCodeCustom())
  {
    TractorPull.sAudioEmitter = new AudioEmitter();
    TractorPull.sAudioEmitter.Forward = Vector3.Forward;
    TractorPull.sAudioEmitter.Up = Vector3.Up;
    TractorPull.sAudioEmitter.Position = Vector3.Zero;
  }

  public TractorPull(Magicka.Animations iAnimation)
    : base(iAnimation, "#magick_tractorpull".GetHashCodeCustom())
  {
    TractorPull.sAudioEmitter = new AudioEmitter();
    TractorPull.sAudioEmitter.Forward = Vector3.Forward;
    TractorPull.sAudioEmitter.Up = Vector3.Up;
    TractorPull.sAudioEmitter.Position = Vector3.Zero;
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    List<Entity> entities = iPlayState.EntityManager.GetEntities(iPosition, 15f, true);
    for (int index = 0; index < entities.Count; ++index)
    {
      if (entities[index] is Character character)
      {
        if (character != this.mSpellCaster && !character.IsEthereal)
        {
          if (character.IsGripped && character.Gripper != null)
            character.Gripper.ReleaseAttachedCharacter();
        }
        else
          continue;
      }
      Vector3 result1 = entities[index].Position;
      Vector3 result2;
      Vector3.Subtract(ref iPosition, ref result1, out result2);
      float num1 = result2.Length();
      result2.Y = 0.0f;
      if ((double) num1 > 1.4012984643248171E-45)
      {
        float num2 = 2.5f * (float) Math.Pow((double) num1 / 15.0, 0.5);
        float mass = entities[index].Body.Mass;
        float num3 = 0.17453292f;
        Damage iDamage = new Damage(AttackProperties.Pushed, Elements.Earth, mass, num2);
        if (entities[index] is IDamageable && !(entities[index] is MissileEntity))
        {
          Vector3.Negate(ref result2, out result2);
          Vector3.Add(ref result1, ref result2, out result1);
          int num4 = (int) (entities[index] as IDamageable).Damage(iDamage, this.mSpellCaster as Entity, this.mTimeStamp, result1);
        }
        else
        {
          if (entities[index] is Item)
          {
            if (!(entities[index] as Item).IgnoreTractorPull)
            {
              if ((entities[index] as Item).Body.Immovable)
              {
                (entities[index] as Item).Body.Immovable = false;
                (entities[index] as Item).Body.SetActive();
              }
            }
            else
              continue;
          }
          entities[index].AddImpulseVelocity(result2, num3 + (float) ((double) num3 * 0.78539818525314331 * 0.5), mass, num2);
        }
      }
    }
    iPlayState.EntityManager.ReturnEntityList(entities);
    iPlayState.Camera.CameraShake(1f, 0.5f);
    RadialBlur.GetRadialBlur().Initialize(ref iPosition, 15f, 1f, iPlayState.Scene);
    PushSpell.PushMagnitudeVolumAdjustAndOthers iVariables = new PushSpell.PushMagnitudeVolumAdjustAndOthers();
    iVariables.magnitude = 1f;
    AudioEmitter iEmitter;
    if (this.mSpellCaster == null)
    {
      TractorPull.sAudioEmitter.Position = iPosition;
      iEmitter = TractorPull.sAudioEmitter;
    }
    else
      iEmitter = this.mSpellCaster.AudioEmitter;
    AudioManager.Instance.PlayCue<PushSpell.PushMagnitudeVolumAdjustAndOthers>(Banks.Spells, TractorPull.SOUND_EFFECT, iVariables, iEmitter);
    return true;
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    this.mSpellCaster = iOwner;
    bool flag = this.Execute(iOwner.Position, iPlayState);
    this.mSpellCaster = (ISpellCaster) null;
    return flag;
  }
}
