// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Thunderstorm
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.Achievements;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Gamers;
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

public class Thunderstorm : SpecialAbility, IAbilityEffect
{
  private const float TIME_BETWEEN_BOLTS = 0.5f;
  private const float RANGE = 5f;
  private static Thunderstorm mSingelton;
  private static volatile object mSingeltonLock = new object();
  private bool[] mPlayersAliveAtStart = new bool[4];
  private bool mPerfectStorm;
  protected static readonly int EFFECT = Thunderbolt.EFFECT;
  protected static readonly int SOUND = Thunderbolt.SOUND;
  protected static readonly int AMBIENCE = "magick_thunderstorm".GetHashCodeCustom();
  public static readonly Damage sDamage = Thunderbolt.sDamage;
  protected static AudioEmitter mAudioEmitter = new AudioEmitter();
  private float mBoltTTL;
  private Rain mRain = Rain.Instance;
  private bool mIndoor;
  private ISpellCaster mOwner;
  private Vector3 mPosition;
  private PlayState mPlayState;
  private Cue mAmbience;

  public static Thunderstorm Instance
  {
    get
    {
      if (Thunderstorm.mSingelton == null)
      {
        lock (Thunderstorm.mSingeltonLock)
        {
          if (Thunderstorm.mSingelton == null)
            Thunderstorm.mSingelton = new Thunderstorm();
        }
      }
      return Thunderstorm.mSingelton;
    }
  }

  private Thunderstorm()
    : base(Magicka.Animations.cast_magick_global, "#magick_thunders".GetHashCodeCustom())
  {
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    if (iPlayState.Level.CurrentScene.Indoors)
      return false;
    this.mTimeStamp = 0.0;
    this.mPerfectStorm = true;
    for (int index = 0; index < Magicka.Game.Instance.Players.Length; ++index)
    {
      this.mPlayersAliveAtStart[index] = false;
      if (Magicka.Game.Instance.Players[index].Playing && Magicka.Game.Instance.Players[index].Avatar != null)
      {
        this.mPlayersAliveAtStart[index] = !Magicka.Game.Instance.Players[index].Avatar.Dead;
        if (Magicka.Game.Instance.Players[index].Avatar.Dead)
          this.mPerfectStorm = false;
      }
    }
    this.mPosition = iPosition;
    this.mOwner = (ISpellCaster) null;
    this.mPlayState = iPlayState;
    this.mIndoor = this.mPlayState.Level.CurrentScene.Indoors;
    this.mRain.Execute(iPosition, iPlayState);
    this.mBoltTTL = 2f;
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    if (this.mAmbience != null && !this.mAmbience.IsPlaying)
      this.mAmbience = AudioManager.Instance.PlayCue(Banks.Spells, Thunderstorm.AMBIENCE);
    return true;
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    if (iPlayState.Level.CurrentScene.Indoors)
      return false;
    this.mTimeStamp = iOwner.PlayState.PlayTime;
    this.mPerfectStorm = true;
    for (int index = 0; index < Magicka.Game.Instance.Players.Length; ++index)
    {
      this.mPlayersAliveAtStart[index] = false;
      if (Magicka.Game.Instance.Players[index].Playing && Magicka.Game.Instance.Players[index].Avatar != null)
      {
        this.mPlayersAliveAtStart[index] = !Magicka.Game.Instance.Players[index].Avatar.Dead;
        if (Magicka.Game.Instance.Players[index].Avatar.Dead)
          this.mPerfectStorm = false;
      }
    }
    this.mOwner = iOwner;
    this.mPlayState = iPlayState;
    this.mIndoor = this.mPlayState.Level.CurrentScene.Indoors;
    this.mRain.Execute(iOwner, iPlayState);
    this.mBoltTTL = 2f;
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    if (this.mAmbience != null && !this.mAmbience.IsPlaying)
      this.mAmbience = AudioManager.Instance.PlayCue(Banks.Spells, Thunderstorm.AMBIENCE);
    return true;
  }

  public void CheckPerfectStorm()
  {
    if (this.IsDead)
      return;
    for (int index = 0; index < Magicka.Game.Instance.Players.Length; ++index)
    {
      if (Magicka.Game.Instance.Players[index].Playing && Magicka.Game.Instance.Players[index].Avatar != null && this.mPlayersAliveAtStart[index] == Magicka.Game.Instance.Players[index].Avatar.Dead)
        this.mPerfectStorm = false;
    }
  }

  public bool IsDead => this.mRain.IsDead;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if (this.mIndoor || NetworkManager.Instance.State == NetworkState.Client)
      return;
    this.mBoltTTL -= iDeltaTime;
    this.CheckPerfectStorm();
    if ((double) this.mBoltTTL > 0.0)
      return;
    Vector3 iPosition = this.mOwner != null ? this.mOwner.Position : this.mPosition;
    this.mBoltTTL = (float) (0.5 + SpecialAbility.RANDOM.NextDouble() * 0.5);
    int num1 = SpecialAbility.RANDOM.Next(2) + 1;
    for (int index1 = 0; index1 < num1; ++index1)
    {
      Vector3 iCenter = iPosition;
      float num2 = (float) Math.Sqrt(SpecialAbility.RANDOM.NextDouble());
      float num3 = (float) SpecialAbility.RANDOM.NextDouble() * 6.28318548f;
      float num4 = num2 * (float) Math.Cos((double) num3);
      float num5 = num2 * (float) Math.Sin((double) num3);
      iCenter.X += 20f * num4;
      iCenter.Z += 20f * num5;
      Vector3 oPoint;
      double nearestPosition = (double) this.mPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref iCenter, out oPoint, MovementProperties.All);
      iCenter = oPoint;
      IDamageable t = (IDamageable) null;
      float num6 = float.MinValue;
      List<Entity> entities = this.mPlayState.EntityManager.GetEntities(iCenter, 5f, true, true);
      for (int index2 = 0; index2 < entities.Count; ++index2)
      {
        if (entities[index2] is IDamageable && (!(entities[index2] is Magicka.GameLogic.Entities.Character) || !(entities[index2] as Magicka.GameLogic.Entities.Character).IsEthereal) && (!(entities[index2] is BossDamageZone) || !((entities[index2] as BossDamageZone).Owner is Grimnir2)) && !(entities[index2] is MissileEntity))
        {
          float y = entities[index2].Body.CollisionSkin.WorldBoundingBox.Max.Y;
          if ((double) y > (double) num6)
          {
            t = entities[index2] as IDamageable;
            num6 = y;
          }
        }
      }
      this.mPlayState.EntityManager.ReturnEntityList(entities);
      Flash.Instance.Execute(this.mPlayState.Scene, 0.125f);
      LightningBolt lightning = LightningBolt.GetLightning();
      Vector3 iDirection = new Vector3(0.0f, -1f, 0.0f);
      Vector3 lightningcolor = Spell.LIGHTNINGCOLOR;
      float iScale = 1f;
      if (t != null)
      {
        iCenter = t.Position;
        iPosition = iCenter;
        iPosition.Y += 20f;
        if (t is Shield shield)
        {
          if (shield.ShieldType == ShieldType.SPHERE)
            iCenter.Y += t.Body.CollisionSkin.WorldBoundingBox.Max.Y * 0.5f;
          else
            iCenter += shield.Body.Orientation.Forward * shield.Radius;
        }
        int num7 = (int) t.Damage(Thunderstorm.sDamage, this.mOwner as Entity, this.mTimeStamp, iPosition);
        if (t is Avatar && (double) t.HitPoints > 0.0 && !((t as Avatar).Player.Gamer is NetworkGamer))
          AchievementsManager.Instance.AwardAchievement(this.mPlayState, "oneinamillion");
      }
      Vector3 position = this.mPlayState.Camera.Position;
      iPosition = iCenter;
      iPosition.Y += 40f;
      lightning.InitializeEffect(ref iPosition, ref iDirection, ref iCenter, ref position, ref lightningcolor, false, iScale, 1f, this.mPlayState);
      if (NetworkManager.Instance.State == NetworkState.Server)
      {
        TriggerActionMessage iMessage = new TriggerActionMessage();
        iMessage.ActionType = TriggerActionType.ThunderBolt;
        if (this.mOwner != null)
          iMessage.Handle = this.mOwner.Handle;
        if (t != null)
          iMessage.Id = (int) t.Handle;
        iMessage.Position = iCenter;
        NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref iMessage);
      }
      Vector3 right = Vector3.Right;
      EffectManager.Instance.StartEffect(Thunderstorm.EFFECT, ref iCenter, ref right, out VisualEffectReference _);
      if (!(t is Shield))
      {
        Segment iSeg = new Segment();
        iSeg.Origin = iCenter;
        iSeg.Delta.Y -= 10f;
        Vector3 oPos;
        Vector3 oNrm;
        AnimatedLevelPart oAnimatedLevelPart;
        if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out oNrm, out oAnimatedLevelPart, iSeg))
        {
          iCenter = oPos;
          DecalManager.Instance.AddAlphaBlendedDecal(Decal.Scorched, oAnimatedLevelPart, 4f, ref iCenter, ref oNrm, 60f);
        }
      }
      Thunderstorm.mAudioEmitter.Position = iPosition;
      Thunderstorm.mAudioEmitter.Up = Vector3.Up;
      Thunderstorm.mAudioEmitter.Forward = Vector3.Right;
      AudioManager.Instance.PlayCue(Banks.Spells, Thunderstorm.SOUND, Thunderstorm.mAudioEmitter);
      this.mPlayState.Camera.CameraShake(iPosition, 1.2f, 0.333f);
    }
  }

  public void OnRemove()
  {
    this.mBoltTTL = 0.0f;
    if (this.mPerfectStorm)
      AchievementsManager.Instance.AwardAchievement(this.mPlayState, "theperfectstorm");
    if (this.mAmbience == null || this.mAmbience.IsStopping)
      return;
    this.mAmbience.Stop(AudioStopOptions.AsAuthored);
  }
}
