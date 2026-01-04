// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.Teleport
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.Audio;
using Magicka.GameLogic.Entities;
using Magicka.Graphics;
using Magicka.Physics;
using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class Teleport(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  public static readonly int ANYID = "any".GetHashCodeCustom();
  protected string mID;
  protected int mIDHash;
  protected string mTarget;
  protected int mTargetHash;
  protected string mEffect;
  protected int mEffectHash;
  protected string mSound;
  protected int mSoundHash;

  protected override void Execute()
  {
    if (this.mIDHash == 0)
      return;
    if (this.mEffectHash == 0)
      this.mEffectHash = Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Teleport.TELEPORT_EFFECT_APPEAR;
    if (this.mSoundHash == 0)
      this.mSoundHash = Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Teleport.TELEPORT_SOUND_DESTINATION;
    Entity byId = Entity.GetByID(this.mIDHash);
    if (byId == null)
      return;
    Vector3 position = byId.Position;
    Vector3 right = Vector3.Right;
    VisualEffectReference oRef;
    EffectManager.Instance.StartEffect(this.mEffectHash, ref position, ref right, out oRef);
    Matrix oLocator;
    this.GameScene.PlayState.Level.CurrentScene.GetLocator(this.mTargetHash, out oLocator);
    EffectManager.Instance.StartEffect(this.mEffectHash, ref oLocator, out oRef);
    Vector3 pos = oLocator.Translation;
    oLocator.Translation = Vector3.Zero;
    Segment iSeg = new Segment();
    iSeg.Origin = pos;
    iSeg.Delta.Y -= 4f;
    Vector3 oPos;
    if (this.GameScene.PlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out Vector3 _, iSeg))
      pos = oPos;
    if (byId.Body.CollisionSkin.GetPrimitiveLocal(0) is Sphere)
      pos.Y += (byId.Body.CollisionSkin.GetPrimitiveLocal(0) as Sphere).Radius;
    else if (byId.Body.CollisionSkin.GetPrimitiveLocal(0) is Capsule)
      pos.Y += (byId.Body.CollisionSkin.GetPrimitiveLocal(0) as Capsule).Radius + (byId.Body.CollisionSkin.GetPrimitiveLocal(0) as Capsule).Length * 0.5f;
    pos.Y += 0.1f;
    byId.Body.MoveTo(pos, oLocator);
    if (byId.Body is CharacterBody)
      (byId.Body as CharacterBody).DesiredDirection = oLocator.Forward;
    AudioManager.Instance.PlayCue(Banks.Spells, this.mSoundHash, byId.AudioEmitter);
  }

  public override void QuickExecute() => this.Execute();

  public string Effect
  {
    get => this.mEffect;
    set
    {
      this.mEffect = value;
      this.mEffectHash = this.mEffect.GetHashCodeCustom();
    }
  }

  public string Sound
  {
    get => this.mSound;
    set
    {
      this.mSound = value;
      this.mSoundHash = this.mSound.GetHashCodeCustom();
    }
  }

  public string ID
  {
    get => this.mID;
    set
    {
      this.mID = value;
      this.mIDHash = this.mID.GetHashCodeCustom();
    }
  }

  public string Target
  {
    get => this.Target;
    set
    {
      this.mTarget = value;
      this.mTargetHash = this.mTarget.GetHashCodeCustom();
    }
  }
}
