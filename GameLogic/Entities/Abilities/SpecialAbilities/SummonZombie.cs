// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.SummonZombie
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.AI;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PolygonHead;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class SummonZombie : SpecialAbility, IAbilityEffect
{
  private const float TIME_BETWEEN_SPAWNS = 2f;
  private const float MAGICK_TIME = 8.1f;
  private static List<SummonZombie> sCache;
  private static CharacterTemplate sTemplate;
  public static readonly int SOUNDHASH = "magick_raise_dead".GetHashCodeCustom();
  public static readonly int EFFECT = "magick_summonundead_ground".GetHashCodeCustom();
  public static readonly int SUMMON_EFFECT = "magick_friendlysummon".GetHashCodeCustom();
  private float mTTL;
  private float mSpawnTimer;
  private PlayState mPlayState;
  private ISpellCaster mOwner;
  private AudioEmitter mAudioEmitter;
  private Vector3 mPosition;
  private VisualEffectReference mEffect;

  public static SummonZombie GetInstance()
  {
    if (SummonZombie.sCache.Count <= 0)
      return new SummonZombie();
    SummonZombie instance = SummonZombie.sCache[SummonZombie.sCache.Count - 1];
    SummonZombie.sCache.RemoveAt(SummonZombie.sCache.Count - 1);
    return instance;
  }

  public static void InitializeCache(int iNr, PlayState iPlayState)
  {
    SummonZombie.sTemplate = iPlayState.Content.Load<CharacterTemplate>("data/characters/zombie");
    SummonZombie.sCache = new List<SummonZombie>(iNr);
    for (int index = 0; index < iNr; ++index)
      SummonZombie.sCache.Add(new SummonZombie());
  }

  public SummonZombie(Magicka.Animations iAnimation)
    : base(iAnimation, "#magick_sundead".GetHashCodeCustom())
  {
    this.mAudioEmitter = new AudioEmitter();
  }

  private SummonZombie()
    : base(Magicka.Animations.cast_magick_direct, "#magick_sundead".GetHashCodeCustom())
  {
    this.mAudioEmitter = new AudioEmitter();
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    this.mOwner = (ISpellCaster) null;
    this.mPlayState = iPlayState;
    return this.Execute(iPosition);
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    this.mOwner = iOwner;
    this.mPlayState = iPlayState;
    Vector3 result1 = iOwner.Position;
    Vector3 result2 = iOwner.Direction;
    Vector3.Multiply(ref result2, 4f, out result2);
    Vector3.Add(ref result1, ref result2, out result1);
    return this.Execute(result1);
  }

  private bool Execute(Vector3 iPosition)
  {
    this.mTTL = 8.1f;
    this.mSpawnTimer = 0.0f;
    double nearestPosition = (double) this.mPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref iPosition, out this.mPosition, MovementProperties.Default);
    Segment iSeg = new Segment(this.mPosition, Vector3.Down * 3f);
    Vector3 oPos;
    if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out Vector3 _, iSeg))
      iPosition = oPos;
    Matrix identity = Matrix.Identity;
    identity.M11 *= 4f;
    identity.M12 *= 4f;
    identity.M13 *= 4f;
    identity.M31 *= 4f;
    identity.M32 *= 4f;
    identity.M33 *= 4f;
    identity.Translation = iPosition;
    EffectManager.Instance.StartEffect(SummonZombie.EFFECT, ref identity, out this.mEffect);
    this.mAudioEmitter.Position = iPosition;
    this.mAudioEmitter.Up = Vector3.Up;
    this.mAudioEmitter.Forward = Vector3.Right;
    AudioManager.Instance.PlayCue(Banks.Spells, SummonZombie.SOUNDHASH, this.mAudioEmitter);
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    return true;
  }

  public bool IsDead => (double) this.mTTL <= 0.0;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mTTL -= iDeltaTime;
    this.mSpawnTimer -= iDeltaTime;
    this.mAudioEmitter.Position = this.mPosition;
    this.mAudioEmitter.Up = Vector3.Up;
    this.mAudioEmitter.Forward = Vector3.Right;
    if (NetworkManager.Instance.State == NetworkState.Client || (double) this.mSpawnTimer > 0.0)
      return;
    this.mSpawnTimer += 2f;
    NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(this.mPlayState);
    if (instance == null)
      return;
    Vector3 oPoint = this.mPosition;
    Vector3 result1 = new Vector3((float) ((SpecialAbility.RANDOM.NextDouble() - 0.5) * 3.0), 0.0f, (float) ((SpecialAbility.RANDOM.NextDouble() - 0.5) * 3.0));
    Vector3.Add(ref oPoint, ref result1, out result1);
    double nearestPosition = (double) this.mPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref result1, out oPoint, MovementProperties.Default);
    Vector3 oPos;
    if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out Vector3 _, new Segment()
    {
      Origin = oPoint,
      Delta = {
        Y = -10f
      }
    }))
      oPoint = oPos;
    instance.Initialize(SummonZombie.sTemplate, oPoint, 0);
    if (this.mOwner is Magicka.GameLogic.Entities.Character)
      instance.Summoned(this.mOwner as Magicka.GameLogic.Entities.Character);
    instance.ForceAnimation(Magicka.Animations.spawn);
    Agent ai = instance.AI;
    ai.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, 0, 0, (AIEvent[]) null);
    ai.AlertRadius = 12f;
    Matrix result2;
    Matrix.CreateRotationY((float) SpecialAbility.RANDOM.NextDouble() * 6.28318548f, out result2);
    instance.Body.Orientation = result2;
    this.mPlayState.EntityManager.AddEntity((Entity) instance);
    if (NetworkManager.Instance.State != NetworkState.Server)
      return;
    TriggerActionMessage iMessage = new TriggerActionMessage();
    iMessage.ActionType = TriggerActionType.SpawnNPC;
    iMessage.Handle = instance.Handle;
    iMessage.Template = instance.Type;
    iMessage.Id = instance.UniqueID;
    iMessage.Position = instance.Position;
    iMessage.Direction = result2.Forward;
    iMessage.Bool0 = false;
    if (this.mOwner != null)
      iMessage.Scene = (int) this.mOwner.Handle;
    iMessage.Point1 = 170;
    iMessage.Point2 = 170;
    NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref iMessage);
  }

  public void OnRemove()
  {
    EffectManager.Instance.Stop(ref this.mEffect);
    SummonZombie.sCache.Add(this);
  }
}
