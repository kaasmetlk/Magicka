// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Nullify
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Magicka.Network;
using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

internal class Nullify : SpecialAbility
{
  private static Nullify sSingelton;
  private static volatile object sSingeltonLock = new object();
  public static readonly int EFFECT = "magick_nullified".GetHashCodeCustom();
  public static readonly int SOUND_HASH = "magick_nullify".GetHashCodeCustom();

  public static Nullify Instance
  {
    get
    {
      if (Nullify.sSingelton == null)
      {
        lock (Nullify.sSingeltonLock)
        {
          if (Nullify.sSingelton == null)
            Nullify.sSingelton = new Nullify();
        }
      }
      return Nullify.sSingelton;
    }
  }

  private Nullify()
    : base(Magicka.Animations.cast_magick_self, "#magick_nullify".GetHashCodeCustom())
  {
  }

  public Nullify(Magicka.Animations iAnimation)
    : base(iAnimation, "#magick_nullify".GetHashCodeCustom())
  {
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    return this.Execute(iOwner.Position, iPlayState);
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    if (NetworkManager.Instance.State != NetworkState.Client)
      this.NullifyArea(iPlayState, iPosition, true);
    return true;
  }

  public void NullifyArea(PlayState iPlayState, Vector3 iPosition, bool iHitBoss)
  {
    AudioManager.Instance.PlayCue(Banks.Spells, Nullify.SOUND_HASH);
    Flash.Instance.Execute(iPlayState.Scene, 0.2f);
    Portal.Instance.Kill();
    SpellManager.Instance.ClearMagicks();
    StaticList<Entity> entities = iPlayState.EntityManager.Entities;
    for (int iIndex = 0; iIndex < entities.Count; ++iIndex)
    {
      IDamageable damageable = entities[iIndex] as IDamageable;
      if (entities[iIndex] is TornadoEntity)
        entities[iIndex].Kill();
      else if (entities[iIndex] is VortexEntity)
        entities[iIndex].Kill();
      else if (entities[iIndex] is MissileEntity)
        entities[iIndex].Kill();
      else if (entities[iIndex] is OtherworldlyBolt)
        (entities[iIndex] as OtherworldlyBolt).DestroyOnNetwork(false, false, (Entity) null, false);
      else if (damageable == null)
        continue;
      bool flag = false;
      if (entities[iIndex] is Character iOwner)
      {
        iOwner.StopStatusEffects(StatusEffects.Burning | StatusEffects.Wet | StatusEffects.Frozen | StatusEffects.Cold | StatusEffects.Poisoned | StatusEffects.Healing | StatusEffects.Greased | StatusEffects.Steamed);
        if (iOwner.IsEntangled)
        {
          flag = true;
          iOwner.ReleaseEntanglement();
        }
        if (iOwner.IsSelfShielded)
        {
          flag = true;
          iOwner.RemoveSelfShield();
        }
        if (iOwner.IsFeared)
        {
          flag = true;
          iOwner.RemoveFear();
        }
        if (iOwner.IsInvisibile)
        {
          flag = true;
          iOwner.RemoveInvisibility();
        }
        if (iOwner.IsHypnotized)
        {
          flag = true;
          iOwner.StopHypnotize();
        }
        if (iOwner.CurrentSpell != null)
        {
          flag = true;
          iOwner.CurrentSpell.Stop((ISpellCaster) iOwner);
        }
        if (iOwner.SpellQueue.Count > 0)
        {
          flag = true;
          iOwner.SpellQueue.Clear();
        }
        if (iOwner.IsLevitating)
          iOwner.StopLevitate();
        if (!iOwner.mBubble)
          iOwner.ClearAura();
        NonPlayerCharacter nonPlayerCharacter = iOwner as NonPlayerCharacter;
        if (iOwner is Avatar avatar)
          avatar.Player.IconRenderer.Clear();
        else if (nonPlayerCharacter != null)
        {
          if (nonPlayerCharacter.IsCharmed)
          {
            flag = true;
            nonPlayerCharacter.EndCharm();
          }
          if (nonPlayerCharacter.IsSummoned && nonPlayerCharacter.Name != "cross")
          {
            flag = true;
            nonPlayerCharacter.Kill();
          }
        }
      }
      else if (entities[iIndex] is Barrier | entities[iIndex] is SpellMine | entities[iIndex] is Grease.GreaseField | entities[iIndex] is SummonDeath.MagickDeath)
      {
        damageable.Kill();
        flag = true;
      }
      else if (entities[iIndex] is Shield)
        damageable.Kill();
      else if (entities[iIndex] is BossSpellCasterZone && iHitBoss)
      {
        BossSpellCasterZone bossSpellCasterZone = entities[iIndex] as BossSpellCasterZone;
        if (bossSpellCasterZone.Owner is Grimnir2 && bossSpellCasterZone.Index == 0)
          (bossSpellCasterZone.Owner as Grimnir2).Nullify();
      }
      if (flag)
      {
        Matrix orientation = damageable.Body.Orientation with
        {
          Translation = damageable.Position
        };
        EffectManager.Instance.StartEffect(Nullify.EFFECT, ref orientation, out VisualEffectReference _);
      }
    }
    if (NetworkManager.Instance.State != NetworkState.Server)
      return;
    NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
    {
      ActionType = TriggerActionType.Nullify,
      Position = iPosition,
      Bool0 = iHitBoss
    });
  }
}
