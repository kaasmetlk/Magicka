// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Bosses.CthulhuMist
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;

#nullable disable
namespace Magicka.GameLogic.Entities.Bosses;

public class CthulhuMist : BossDamageZone
{
  private const float ACTIVE_TIME = 10f;
  private const float INITIAL_RADIUS = 2f;
  private VisualEffectReference mEffectRef;
  private int mMistEffect = "cthulhu_mist".GetHashCodeCustom();
  private VisualEffectReference mFireEffectRef;
  private int mMistOnFireEffect = "cthulhu_mist_on_fire".GetHashCodeCustom();
  private int mPlayerCharmEffect = Charm.CHARM_EFFECT;
  private static readonly int STARSPAWN_HASH = "starspawn".GetHashCodeCustom();
  private HitList mHitList;
  private float mTimer;
  private bool mActive;

  public bool Active => this.mActive;

  public CthulhuMist(Cthulhu iOwner, byte iID, int iDamageZoneIndex, PlayState iPlayState)
    : base(iPlayState, (IBoss) iOwner, iDamageZoneIndex, 2f, (Primitive) new Sphere(Vector3.Zero, 2f))
  {
    this.mPlayState = iPlayState;
    this.mBody.CollisionSkin.callbackFn += new CollisionCallbackFn(this.OnCollision);
    this.mHitList = new HitList(10);
  }

  internal void Initialize(Vector3 iPosition)
  {
    this.Initialize(0);
    this.SetPosition(ref iPosition);
    if (!EffectManager.Instance.IsActive(ref this.mEffectRef))
    {
      Matrix translation = Matrix.CreateTranslation(iPosition);
      EffectManager.Instance.StartEffect(this.mMistEffect, ref translation, out this.mEffectRef);
    }
    this.mActive = true;
    this.mTimer = 10f;
  }

  private void Deactivate()
  {
    if (EffectManager.Instance.IsActive(ref this.mEffectRef))
      EffectManager.Instance.Stop(ref this.mEffectRef);
    this.mActive = false;
  }

  public override bool Removable => !this.mActive;

  private void SetOnFire()
  {
    Vector3 position = this.Position;
    if (EffectManager.Instance.IsActive(ref this.mEffectRef))
    {
      EffectManager.Instance.Stop(ref this.mEffectRef);
      Matrix result;
      Matrix.CreateTranslation(ref position, out result);
      EffectManager.Instance.StartEffect(this.mMistOnFireEffect, ref result, out this.mFireEffectRef);
    }
    this.mActive = false;
    Magicka.GameLogic.Damage iDamage = new Magicka.GameLogic.Damage(AttackProperties.Status, Elements.Fire, 100f, 2f);
    int num = (int) Helper.CircleDamage(this.mPlayState, (Entity) this, this.mPlayState.PlayTime, (Entity) this, ref position, this.mRadius * 1.5f, ref iDamage);
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mHitList.Update(iDeltaTime);
    if (!this.mActive)
      return;
    base.Update(iDataChannel, iDeltaTime);
    this.mTimer -= iDeltaTime;
    if ((double) this.mTimer > 0.0)
      return;
    this.Deactivate();
  }

  private unsafe bool OnCollision(
    CollisionSkin iSkin0,
    int iPrim0,
    CollisionSkin iSkin1,
    int iPrim1)
  {
    if (!this.mActive || iSkin1.Owner == null)
      return false;
    object tag = iSkin1.Owner.Tag;
    if (tag is IDamageable)
    {
      IDamageable damageable = tag as IDamageable;
      if (tag is IStatusEffected && ((IStatusEffected) tag).HasStatus(StatusEffects.Burning))
        this.SetOnFire();
      if (!this.mHitList.Contains(damageable))
      {
        switch (damageable)
        {
          case MissileEntity _:
            this.mHitList.Add(damageable);
            if (((damageable as MissileEntity).CombinedDamageElements & Elements.Fire) == Elements.Fire)
            {
              this.SetOnFire();
              goto label_15;
            }
            goto label_15;
          case Magicka.GameLogic.Entities.Character _:
            if ((damageable as Magicka.GameLogic.Entities.Character).Type == CthulhuMist.STARSPAWN_HASH)
              return false;
            break;
        }
        Magicka.GameLogic.Damage iDamage = new Magicka.GameLogic.Damage(AttackProperties.Status, Elements.Poison, 50f, 1f);
        int num = (int) damageable.Damage(iDamage, (Entity) null, this.mPlayState.PlayTime, Vector3.Zero);
        Avatar iAvatar = damageable as Avatar;
        if (NetworkManager.Instance.State != NetworkState.Client && iAvatar != null && !iAvatar.IsCharmed && !iAvatar.Dead && !iAvatar.IsImmortal)
        {
          if (NetworkManager.Instance.State == NetworkState.Server)
            BossFight.Instance.SendMessage<Cthulhu.CharmAndConfuseMessage>(this.Owner, (ushort) 9, (void*) &new Cthulhu.CharmAndConfuseMessage()
            {
              Handle = iAvatar.Handle
            }, true);
          this.CharmAndConfuse(iAvatar);
        }
      }
    }
label_15:
    return false;
  }

  public void CharmAndConfuse(Avatar iAvatar)
  {
    if (iAvatar == null)
      return;
    float iTTL = 7.5f;
    Confuse effect = new Magick()
    {
      MagickType = MagickType.Confuse
    }.Effect as Confuse;
    effect.TTL = iTTL;
    effect.Execute(iAvatar.Position, this.mPlayState);
    iAvatar.Charm((Entity) this, iTTL, this.mPlayerCharmEffect);
  }

  public DamageResult Damage(Magicka.GameLogic.Damage iDamage, Entity iAttacker, ref Vector3 iAttackPosition)
  {
    if (iDamage.Element == Elements.Fire)
      this.SetOnFire();
    return DamageResult.OverKilled;
  }

  public override bool SegmentIntersect(out Vector3 oPosition, Segment iSeg, float iSegmentRadius)
  {
    if (this.mActive)
      return base.SegmentIntersect(out oPosition, iSeg, iSegmentRadius);
    oPosition = this.Position;
    return false;
  }

  internal new void Damage(float iDamage, Elements iElement)
  {
  }

  public override bool ArcIntersect(
    out Vector3 oPosition,
    Vector3 iOrigin,
    Vector3 iDirection,
    float iRange,
    float iAngle,
    float iHeightDifference)
  {
    if (this.mActive)
      return base.ArcIntersect(out oPosition, iOrigin, iDirection, iRange, iAngle, iHeightDifference);
    oPosition = this.Position;
    return false;
  }

  internal bool IgnoreElements(Elements iElements) => (iElements & Elements.Fire) == Elements.None;
}
