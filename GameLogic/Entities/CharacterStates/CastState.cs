// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.CharacterStates.CastState
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities.Abilities;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;

#nullable disable
namespace Magicka.GameLogic.Entities.CharacterStates;

public class CastState : BaseState
{
  private static CastState mSingelton;
  private static volatile object mSingeltonLock = new object();

  public static CastState Instance
  {
    get
    {
      if (CastState.mSingelton == null)
      {
        lock (CastState.mSingeltonLock)
        {
          if (CastState.mSingelton == null)
            CastState.mSingelton = new CastState();
        }
      }
      return CastState.mSingelton;
    }
  }

  public override void OnEnter(Character iOwner)
  {
    iOwner.SetInvisible(0.0f);
    iOwner.Ethereal(false, 1f, 1f);
    SpellType spellType = iOwner.Spell.GetSpellType();
    if (spellType == SpellType.Magick)
    {
      iOwner.Magick = new SpellMagickConverter()
      {
        Spell = iOwner.Spell
      }.Magick.Effect;
      iOwner.GoToAnimation(iOwner.Magick.Animation, 0.075f);
      iOwner.CharacterBody.AllowRotate = false;
    }
    else
    {
      switch (iOwner.CastType)
      {
        case CastType.Force:
          switch (spellType)
          {
            case SpellType.Push:
              iOwner.GoToAnimation(Animations.cast_force_push, 0.075f);
              break;
            case SpellType.Spray:
              iOwner.GoToAnimation(Animations.cast_force_spray, 0.075f);
              break;
            case SpellType.Projectile:
              iOwner.GoToAnimation(Animations.cast_force_projectile, 0.075f);
              break;
            case SpellType.Shield:
              iOwner.GoToAnimation(Animations.cast_force_shield, 0.075f);
              break;
            case SpellType.Beam:
              iOwner.GoToAnimation(Animations.cast_force_railgun, 0.075f);
              break;
            case SpellType.Lightning:
              iOwner.GoToAnimation(Animations.cast_force_lightning, 0.075f);
              break;
          }
          break;
        case CastType.Area:
          switch (spellType)
          {
            case SpellType.Push:
              iOwner.GoToAnimation(Animations.cast_area_push, 0.075f);
              break;
            case SpellType.Spray:
              iOwner.GoToAnimation(Animations.cast_area_blast, 0.075f);
              break;
            case SpellType.Projectile:
              iOwner.GoToAnimation(Animations.cast_area_ground, 0.075f);
              break;
            case SpellType.Shield:
              iOwner.GoToAnimation(Animations.cast_area_fireworks, 0.075f);
              break;
            case SpellType.Beam:
              iOwner.GoToAnimation(Animations.cast_area_blast, 0.075f);
              break;
            case SpellType.Lightning:
              iOwner.GoToAnimation(Animations.cast_area_lightning, 0.075f);
              break;
          }
          break;
        case CastType.Self:
          iOwner.GoToAnimation(Animations.cast_self, 0.075f);
          break;
        case CastType.Weapon:
          if (iOwner.Equipment[0] != null && iOwner.Equipment[0].Item != null && iOwner.Equipment[0].Item.SpellCharged && iOwner.SpellQueue.Count == 0)
          {
            switch (iOwner.Equipment[0].Item.PeekSpell().GetSpellType())
            {
              case SpellType.Spray:
                iOwner.GoToAnimation(Animations.cast_sword_spray, 0.075f);
                break;
              case SpellType.Projectile:
                iOwner.GoToAnimation(Animations.cast_sword_projectile, 0.075f);
                break;
              case SpellType.Shield:
                iOwner.GoToAnimation(Animations.cast_sword_projectile, 0.075f);
                break;
              case SpellType.Beam:
                iOwner.GoToAnimation(Animations.cast_sword_railgun, 0.075f);
                break;
              case SpellType.Lightning:
                iOwner.GoToAnimation(Animations.cast_sword_lightning, 0.075f);
                break;
              default:
                iOwner.GoToAnimation(Animations.cast_sword, 0.075f);
                break;
            }
          }
          else
          {
            iOwner.GoToAnimation(Animations.cast_sword, 0.15f);
            break;
          }
          break;
      }
    }
    if (!iOwner.HasStatus(StatusEffects.Frozen) || iOwner.CastType != CastType.Self)
      return;
    iOwner.CastSpell(true, "");
  }

  public override BaseState Update(Character iOwner, float iDeltaTime)
  {
    BaseState baseState = this.UpdateBloatDeath(iOwner, iDeltaTime);
    if (baseState != null)
      return baseState;
    if (iOwner.CurrentAnimation == Animations.idle)
      return (BaseState) IdleState.Instance;
    if (iOwner.CharacterBody.IsPushed)
      return (BaseState) PushedState.Instance;
    if (iOwner.IsKnockedDown)
      return (BaseState) KnockDownState.Instance;
    if (iOwner.AnimationController.IsLooping && !iOwner.AnimationController.CrossFadeEnabled && iOwner.CastType != CastType.Force)
    {
      if ((double) iOwner.CharacterBody.Movement.Length() <= 1.4012984643248171E-45)
        return (BaseState) IdleState.Instance;
      iOwner.GoToAnimation(Animations.move_walk, 0.25f);
      return (BaseState) MoveState.Instance;
    }
    if (iOwner.AnimationController.HasFinished)
    {
      if ((double) iOwner.CharacterBody.Movement.Length() <= 1.4012984643248171E-45)
        return (BaseState) IdleState.Instance;
      iOwner.GoToAnimation(Animations.move_walk, 0.25f);
      return (BaseState) MoveState.Instance;
    }
    if (iOwner.CurrentSpell != null && !iOwner.CurrentSpell.Active)
    {
      if (iOwner.Attacking)
        return (BaseState) AttackState.Instance;
      if ((double) iOwner.CharacterBody.Movement.Length() <= 1.4012984643248171E-45)
        return (BaseState) IdleState.Instance;
      iOwner.GoToAnimation(Animations.move_walk, 0.25f);
      return (BaseState) MoveState.Instance;
    }
    return iOwner.HasStatus(StatusEffects.Frozen) ? (BaseState) IdleState.Instance : (BaseState) null;
  }

  public override void OnExit(Character iOwner)
  {
    if (iOwner is NonPlayerCharacter nonPlayerCharacter)
      nonPlayerCharacter.AI.BusyAbility = (Ability) null;
    if (iOwner.CurrentSpell != null)
    {
      iOwner.CurrentSpell.Stop((ISpellCaster) iOwner);
      iOwner.CurrentSpell = (SpellEffect) null;
    }
    iOwner.CastType = CastType.None;
    iOwner.CharacterBody.AllowRotate = true;
  }
}
