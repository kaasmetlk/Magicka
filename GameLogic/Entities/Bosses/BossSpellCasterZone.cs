// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Bosses.BossSpellCasterZone
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Microsoft.Xna.Framework;
using XNAnimation;
using XNAnimation.Controllers;

#nullable disable
namespace Magicka.GameLogic.Entities.Bosses;

internal class BossSpellCasterZone : BossDamageZone, ISpellCaster, IStatusEffected, IDamageable
{
  private IBossSpellCaster mSpellCasterParent;
  private AnimationController mController;
  private BindJoint mCastAttach;
  private BindJoint mWeaponAttach;
  private static Matrix YROT = Matrix.CreateRotationY(3.14159274f);

  public BossSpellCasterZone(
    PlayState iPlayState,
    IBossSpellCaster iParent,
    AnimationController iController,
    int iCastBoneIndex,
    int iWeaponBoneIndex,
    int iIndex,
    float iRadius,
    params Primitive[] iPrimitives)
    : base(iPlayState, (IBoss) iParent, iIndex, iRadius, iPrimitives)
  {
    this.mController = iController;
    for (int index = 0; index < this.mController.Skeleton.Count; ++index)
    {
      SkinnedModelBone skinnedModelBone = this.mController.Skeleton[index];
      if ((int) skinnedModelBone.Index == iCastBoneIndex)
      {
        this.mCastAttach.mIndex = (int) skinnedModelBone.Index;
        this.mCastAttach.mBindPose = skinnedModelBone.InverseBindPoseTransform;
        Matrix.Invert(ref this.mCastAttach.mBindPose, out this.mCastAttach.mBindPose);
        Matrix.Multiply(ref BossSpellCasterZone.YROT, ref this.mCastAttach.mBindPose, out this.mCastAttach.mBindPose);
      }
      else if ((int) skinnedModelBone.Index == iWeaponBoneIndex)
      {
        this.mWeaponAttach.mIndex = (int) skinnedModelBone.Index;
        this.mWeaponAttach.mBindPose = skinnedModelBone.InverseBindPoseTransform;
        Matrix.Invert(ref this.mWeaponAttach.mBindPose, out this.mWeaponAttach.mBindPose);
        Matrix.Multiply(ref BossSpellCasterZone.YROT, ref this.mWeaponAttach.mBindPose, out this.mWeaponAttach.mBindPose);
      }
    }
    this.mSpellCasterParent = iParent;
  }

  public int Index => this.mIndex;

  public new float ResistanceAgainst(Elements iElement)
  {
    return 1f - MathHelper.Clamp((float) (0.0 / 300.0) + 0.0f, -1f, 1f);
  }

  public AnimationController AnimationController => this.mController;

  public void AddSelfShield(Spell iSpell)
  {
    this.mSpellCasterParent.AddSelfShield(this.mIndex, iSpell);
  }

  public void RemoveSelfShield(Character.SelfShieldType iType)
  {
    this.mSpellCasterParent.RemoveSelfShield(this.mIndex, iType);
  }

  public new Vector3 Direction => this.mBody.Orientation.Forward;

  public CastType CastType => this.mSpellCasterParent.CastType(this.mIndex);

  public MissileEntity GetMissileInstance() => MissileEntity.GetInstance(this.mPlayState);

  public Matrix CastSource
  {
    get
    {
      Matrix result;
      Matrix.Multiply(ref this.mCastAttach.mBindPose, ref this.mController.SkinnedBoneTransforms[this.mCastAttach.mIndex], out result);
      return result;
    }
  }

  public Matrix WeaponSource
  {
    get
    {
      Matrix result;
      Matrix.Multiply(ref this.mWeaponAttach.mBindPose, ref this.mController.SkinnedBoneTransforms[this.mWeaponAttach.mIndex], out result);
      return result;
    }
  }

  public float SpellPower
  {
    get => this.mSpellCasterParent.SpellPower(this.mIndex);
    set => this.mSpellCasterParent.SpellPower(this.mIndex, value);
  }

  public SpellEffect CurrentSpell
  {
    get => this.mSpellCasterParent.CurrentSpell(this.mIndex);
    set => this.mSpellCasterParent.CurrentSpell(this.mIndex, value);
  }

  public virtual bool HasPassiveAbility(Item.PassiveAbilities iAbility) => false;
}
