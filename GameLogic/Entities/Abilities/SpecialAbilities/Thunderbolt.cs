// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Thunderbolt
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
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class Thunderbolt : SpecialAbility
{
  public const float RANGE = 16f;
  private static Thunderbolt mSingelton;
  private static volatile object mSingeltonLock = new object();
  public static readonly int EFFECT = "magick_thunderbolt".GetHashCodeCustom();
  public static readonly int SOUND = "magick_thunderbolt".GetHashCodeCustom();
  public static readonly Damage sDamage;
  public static AudioEmitter sAudioEmitter = new AudioEmitter();
  private PlayState mPlayState;
  private new double mTimeStamp;

  public static Thunderbolt Instance
  {
    get
    {
      if (Thunderbolt.mSingelton == null)
      {
        lock (Thunderbolt.mSingeltonLock)
        {
          if (Thunderbolt.mSingelton == null)
            Thunderbolt.mSingelton = new Thunderbolt();
        }
      }
      return Thunderbolt.mSingelton;
    }
  }

  static Thunderbolt()
  {
    Thunderbolt.sDamage = new Damage();
    Thunderbolt.sDamage.Amount = 5000f;
    Thunderbolt.sDamage.AttackProperty = AttackProperties.Knockback | AttackProperties.Damage;
    Thunderbolt.sDamage.Element = Elements.Lightning;
    Thunderbolt.sDamage.Magnitude = 1f;
  }

  private Thunderbolt()
    : base(Magicka.Animations.cast_magick_direct, "#magick_thunderb".GetHashCodeCustom())
  {
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    this.mPlayState = iPlayState;
    Vector3 iDirection = new Vector3((float) SpecialAbility.RANDOM.NextDouble(), 0.0f, (float) SpecialAbility.RANDOM.NextDouble());
    iDirection.Normalize();
    return this.Execute(iPosition, iDirection, (ISpellCaster) null);
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    this.mPlayState = iPlayState;
    return this.Execute(iOwner.Position, iOwner.Direction, iOwner);
  }

  private bool Execute(Vector3 iPosition, Vector3 iDirection, ISpellCaster iOwner)
  {
    if (this.mPlayState.Level.CurrentScene.Indoors)
    {
      AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL, iOwner.AudioEmitter);
      return false;
    }
    if (NetworkManager.Instance.State != NetworkState.Client)
    {
      this.mTimeStamp = iOwner.PlayState.PlayTime;
      float num1 = 16f;
      Flash.Instance.Execute(this.mPlayState.Scene, 0.125f);
      Vector3 vector3_1 = iDirection;
      Vector3 vector3_2 = iPosition with { Y = 0.0f };
      Vector3 result;
      Vector3.Multiply(ref vector3_1, num1 * 0.5f, out result);
      Vector3.Add(ref result, ref vector3_2, out result);
      List<Entity> entities = this.mPlayState.EntityManager.GetEntities(result, num1 * 0.5f, false, true);
      entities.Remove(iOwner as Entity);
      bool flag = false;
      IDamageable t = (IDamageable) null;
      if (this.mPlayState.EntityManager.IsProtectedByShield(iOwner as Entity, out oShield))
      {
        flag = true;
        t = (IDamageable) oShield;
      }
      Vector3 vector3_3 = new Vector3();
      Vector3 right = Vector3.Right;
      Segment segment = new Segment();
      segment.Origin = vector3_2;
      segment.Delta.Y += 22f;
      if (!flag)
      {
        segment.Origin = result;
        List<IDamageable> damageableList = new List<IDamageable>(entities.Count);
        for (int index = 0; index < entities.Count; ++index)
        {
          if (entities[index] is IDamageable damageable && (!(entities[index] is Character) || !(entities[index] as Character).IsEthereal) && (!(entities[index] is BossDamageZone) || !(entities[index] as BossDamageZone).IsEthereal) && !(entities[index] is MissileEntity) && !this.mPlayState.EntityManager.IsProtectedByShield(entities[index], out oShield))
            damageableList.Add(damageable);
        }
        this.mPlayState.EntityManager.ReturnEntityList(entities);
        float num2 = float.MinValue;
        for (int index = 0; index < damageableList.Count; ++index)
        {
          float y = damageableList[index].Body.CollisionSkin.WorldBoundingBox.Max.Y;
          if ((double) y > (double) num2)
          {
            t = damageableList[index];
            num2 = y;
          }
        }
      }
      LightningBolt lightning = LightningBolt.GetLightning();
      Vector3 iDirection1 = new Vector3(0.0f, -1f, 0.0f);
      Vector3 lightningcolor = Spell.LIGHTNINGCOLOR;
      Vector3 position = this.mPlayState.Scene.Camera.Position;
      float iScale = 1f;
      Vector3 iPosition1;
      if (t != null)
      {
        iPosition1 = t.Position;
        if (t is Shield oShield)
        {
          if (oShield.ShieldType == ShieldType.SPHERE)
            iPosition1.Y += t.Body.CollisionSkin.WorldBoundingBox.Max.Y * 0.5f;
          else
            iPosition1 += oShield.Body.Orientation.Forward * oShield.Radius;
        }
        int num3 = (int) t.Damage(Thunderbolt.sDamage, iOwner as Entity, this.mTimeStamp, result);
        if (t is Avatar && (double) t.HitPoints > 0.0 && !((t as Avatar).Player.Gamer is NetworkGamer))
          AchievementsManager.Instance.AwardAchievement(this.mPlayState, "oneinamillion");
      }
      else
      {
        result.X += (float) (MagickaMath.Random.NextDouble() - 0.5) * (num1 * 0.5f);
        result.Z += (float) (MagickaMath.Random.NextDouble() - 0.5) * (num1 * 0.5f);
        iPosition1 = result;
        iDirection1 = Vector3.Right;
      }
      result.Y += 40f;
      lightning.InitializeEffect(ref result, ref iDirection1, ref iPosition1, ref position, ref lightningcolor, false, iScale, 1f, this.mPlayState);
      if (NetworkManager.Instance.State == NetworkState.Server)
      {
        TriggerActionMessage iMessage = new TriggerActionMessage();
        iMessage.ActionType = TriggerActionType.ThunderBolt;
        if (iOwner != null)
          iMessage.Handle = iOwner.Handle;
        if (t != null)
          iMessage.Id = (int) t.Handle;
        iMessage.Position = iPosition1;
        NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref iMessage);
      }
      EffectManager.Instance.StartEffect(Thunderbolt.EFFECT, ref iPosition1, ref right, out VisualEffectReference _);
      if (!(t is Shield))
      {
        Segment iSeg = new Segment();
        iSeg.Origin = iPosition1;
        ++iSeg.Origin.Y;
        iSeg.Delta.Y -= 10f;
        Vector3 oPos;
        Vector3 oNrm;
        AnimatedLevelPart oAnimatedLevelPart;
        if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out oNrm, out oAnimatedLevelPart, iSeg))
        {
          iPosition1 = oPos;
          DecalManager.Instance.AddAlphaBlendedDecal(Decal.Scorched, oAnimatedLevelPart, 4f, ref iPosition1, ref oNrm, 60f);
        }
      }
      Thunderbolt.sAudioEmitter.Position = iPosition1;
      Thunderbolt.sAudioEmitter.Up = Vector3.Up;
      Thunderbolt.sAudioEmitter.Forward = Vector3.Right;
      AudioManager.Instance.PlayCue(Banks.Spells, Thunderbolt.SOUND, Thunderbolt.sAudioEmitter);
      this.mPlayState.Camera.CameraShake(iPosition1, 1.5f, 0.333f);
    }
    return true;
  }
}
