// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.GreaseTrail
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class GreaseTrail : SpecialAbility, IAbilityEffect
{
  private static List<GreaseTrail> sCache;
  public static readonly int SOUNDHASH = "magick_grease".GetHashCodeCustom();
  private Magicka.GameLogic.Entities.Character mOwner;
  public PlayState mPlayState;
  private VisualEffectReference mEffect;
  private float mTTL;
  private float mInterval;
  private Magicka.Animations mAnimationName;
  private readonly float GREASE_INTERVAL = 0.35f;

  public static GreaseTrail GetInstance()
  {
    if (GreaseTrail.sCache.Count <= 0)
      return new GreaseTrail();
    GreaseTrail instance = GreaseTrail.sCache[GreaseTrail.sCache.Count - 1];
    GreaseTrail.sCache.RemoveAt(GreaseTrail.sCache.Count - 1);
    return instance;
  }

  public static void InitializeCache(int iNr, PlayState iPlayState)
  {
    GreaseTrail.sCache = new List<GreaseTrail>(iNr);
    for (int index = 0; index < iNr; ++index)
      GreaseTrail.sCache.Add(new GreaseTrail());
  }

  public GreaseTrail(Magicka.Animations iAnimation)
    : base(iAnimation, "#magick_grease".GetHashCodeCustom())
  {
  }

  private GreaseTrail()
    : base(Magicka.Animations.cast_magick_sweep, "#magick_grease".GetHashCodeCustom())
  {
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    throw new Exception("GreaseTrail can not be cast without an owner!");
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    NetworkState state = NetworkManager.Instance.State;
    if ((state == NetworkState.Client || iOwner is Avatar && (iOwner as Avatar).Player.Gamer is NetworkGamer) && (state != NetworkState.Client || !(iOwner is Avatar) || (iOwner as Avatar).Player.Gamer is NetworkGamer))
      return false;
    base.Execute(iOwner, iPlayState);
    this.mTTL = 20f;
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    this.mOwner = iOwner as Magicka.GameLogic.Entities.Character;
    this.mPlayState = iPlayState;
    this.mAnimationName = this.mOwner.CurrentAnimation;
    return true;
  }

  public bool IsDead => (double) this.mTTL <= 0.0;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mTTL -= iDeltaTime;
    this.mInterval -= iDeltaTime;
    if (this.mAnimationName != this.mOwner.CurrentAnimation)
      this.mTTL = 0.0f;
    if ((double) this.mInterval >= 0.0)
      return;
    this.mInterval = this.GREASE_INTERVAL;
    Vector3 position = this.mOwner.Position;
    position.X += (float) (MagickaMath.Random.NextDouble() * 3.0);
    position.Z += (float) (MagickaMath.Random.NextDouble() * 3.0);
    position.Y = 0.0f;
    Vector3 up = Vector3.Up;
    AnimatedLevelPart iAnimation = (AnimatedLevelPart) null;
    Grease.GreaseField instance = Grease.GreaseField.GetInstance(this.mPlayState);
    instance.Initialize((ISpellCaster) this.mOwner, iAnimation, ref position, ref up);
    this.mPlayState.EntityManager.AddEntity((Entity) instance);
    if (NetworkManager.Instance.State != NetworkState.Server)
      return;
    NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
    {
      Handle = instance.Handle,
      Position = position,
      Direction = up,
      Arg = iAnimation == null ? (int) ushort.MaxValue : (int) iAnimation.Handle,
      Id = (int) this.mOwner.Handle,
      ActionType = TriggerActionType.SpawnGrease
    });
  }

  public void OnRemove()
  {
    this.mTTL = 0.0f;
    EffectManager.Instance.Stop(ref this.mEffect);
    GreaseTrail.sCache.Add(this);
  }
}
