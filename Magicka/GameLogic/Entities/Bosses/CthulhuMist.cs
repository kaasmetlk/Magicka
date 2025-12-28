using System;
using JigLibX.Collision;
using JigLibX.Geometry;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Bosses
{
	// Token: 0x02000584 RID: 1412
	public class CthulhuMist : BossDamageZone
	{
		// Token: 0x170009ED RID: 2541
		// (get) Token: 0x06002A30 RID: 10800 RVA: 0x0014BAF1 File Offset: 0x00149CF1
		public bool Active
		{
			get
			{
				return this.mActive;
			}
		}

		// Token: 0x06002A31 RID: 10801 RVA: 0x0014BAFC File Offset: 0x00149CFC
		public CthulhuMist(Cthulhu iOwner, byte iID, int iDamageZoneIndex, PlayState iPlayState) : base(iPlayState, iOwner, iDamageZoneIndex, 2f, new Sphere(Vector3.Zero, 2f))
		{
			this.mPlayState = iPlayState;
			this.mBody.CollisionSkin.callbackFn += this.OnCollision;
			this.mHitList = new HitList(10);
		}

		// Token: 0x06002A32 RID: 10802 RVA: 0x0014BB84 File Offset: 0x00149D84
		internal void Initialize(Vector3 iPosition)
		{
			base.Initialize(0);
			base.SetPosition(ref iPosition);
			if (!EffectManager.Instance.IsActive(ref this.mEffectRef))
			{
				Matrix matrix = Matrix.CreateTranslation(iPosition);
				EffectManager.Instance.StartEffect(this.mMistEffect, ref matrix, out this.mEffectRef);
			}
			this.mActive = true;
			this.mTimer = 10f;
		}

		// Token: 0x06002A33 RID: 10803 RVA: 0x0014BBE4 File Offset: 0x00149DE4
		private void Deactivate()
		{
			if (EffectManager.Instance.IsActive(ref this.mEffectRef))
			{
				EffectManager.Instance.Stop(ref this.mEffectRef);
			}
			this.mActive = false;
		}

		// Token: 0x170009EE RID: 2542
		// (get) Token: 0x06002A34 RID: 10804 RVA: 0x0014BC0F File Offset: 0x00149E0F
		public override bool Removable
		{
			get
			{
				return !this.mActive;
			}
		}

		// Token: 0x06002A35 RID: 10805 RVA: 0x0014BC1C File Offset: 0x00149E1C
		private void SetOnFire()
		{
			Vector3 position = this.Position;
			if (EffectManager.Instance.IsActive(ref this.mEffectRef))
			{
				EffectManager.Instance.Stop(ref this.mEffectRef);
				Matrix matrix;
				Matrix.CreateTranslation(ref position, out matrix);
				EffectManager.Instance.StartEffect(this.mMistOnFireEffect, ref matrix, out this.mFireEffectRef);
			}
			this.mActive = false;
			Damage damage = new Damage(AttackProperties.Status, Elements.Fire, 100f, 2f);
			Helper.CircleDamage(this.mPlayState, this, this.mPlayState.PlayTime, this, ref position, this.mRadius * 1.5f, ref damage);
		}

		// Token: 0x06002A36 RID: 10806 RVA: 0x0014BCBD File Offset: 0x00149EBD
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mHitList.Update(iDeltaTime);
			if (!this.mActive)
			{
				return;
			}
			base.Update(iDataChannel, iDeltaTime);
			this.mTimer -= iDeltaTime;
			if (this.mTimer <= 0f)
			{
				this.Deactivate();
			}
		}

		// Token: 0x06002A37 RID: 10807 RVA: 0x0014BD00 File Offset: 0x00149F00
		private unsafe bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
		{
			if (!this.mActive)
			{
				return false;
			}
			if (iSkin1.Owner == null)
			{
				return false;
			}
			object tag = iSkin1.Owner.Tag;
			if (tag is IDamageable)
			{
				IDamageable damageable = tag as IDamageable;
				if (tag is IStatusEffected && ((IStatusEffected)tag).HasStatus(StatusEffects.Burning))
				{
					this.SetOnFire();
				}
				if (!this.mHitList.Contains(damageable))
				{
					if (damageable is MissileEntity)
					{
						this.mHitList.Add(damageable);
						if (((damageable as MissileEntity).CombinedDamageElements & Elements.Fire) == Elements.Fire)
						{
							this.SetOnFire();
						}
					}
					else
					{
						if (damageable is Character && (damageable as Character).Type == CthulhuMist.STARSPAWN_HASH)
						{
							return false;
						}
						Damage iDamage = new Damage(AttackProperties.Status, Elements.Poison, 50f, 1f);
						damageable.Damage(iDamage, null, this.mPlayState.PlayTime, Vector3.Zero);
						Avatar avatar = damageable as Avatar;
						if (NetworkManager.Instance.State != NetworkState.Client && avatar != null && !avatar.IsCharmed && !avatar.Dead && !avatar.IsImmortal)
						{
							if (NetworkManager.Instance.State == NetworkState.Server)
							{
								Cthulhu.CharmAndConfuseMessage charmAndConfuseMessage = default(Cthulhu.CharmAndConfuseMessage);
								charmAndConfuseMessage.Handle = avatar.Handle;
								BossFight.Instance.SendMessage<Cthulhu.CharmAndConfuseMessage>(base.Owner, 9, (void*)(&charmAndConfuseMessage), true);
							}
							this.CharmAndConfuse(avatar);
						}
					}
				}
			}
			return false;
		}

		// Token: 0x06002A38 RID: 10808 RVA: 0x0014BE5C File Offset: 0x0014A05C
		public void CharmAndConfuse(Avatar iAvatar)
		{
			if (iAvatar == null)
			{
				return;
			}
			float num = 7.5f;
			Magick magick = default(Magick);
			magick.MagickType = MagickType.Confuse;
			Confuse confuse = magick.Effect as Confuse;
			confuse.TTL = num;
			confuse.Execute(iAvatar.Position, this.mPlayState);
			iAvatar.Charm(this, num, this.mPlayerCharmEffect);
		}

		// Token: 0x06002A39 RID: 10809 RVA: 0x0014BEB9 File Offset: 0x0014A0B9
		public DamageResult Damage(Damage iDamage, Entity iAttacker, ref Vector3 iAttackPosition)
		{
			if (iDamage.Element == Elements.Fire)
			{
				this.SetOnFire();
			}
			return DamageResult.OverKilled;
		}

		// Token: 0x06002A3A RID: 10810 RVA: 0x0014BED0 File Offset: 0x0014A0D0
		public override bool SegmentIntersect(out Vector3 oPosition, Segment iSeg, float iSegmentRadius)
		{
			if (!this.mActive)
			{
				oPosition = this.Position;
				return false;
			}
			return base.SegmentIntersect(out oPosition, iSeg, iSegmentRadius);
		}

		// Token: 0x06002A3B RID: 10811 RVA: 0x0014BEF1 File Offset: 0x0014A0F1
		internal new void Damage(float iDamage, Elements iElement)
		{
		}

		// Token: 0x06002A3C RID: 10812 RVA: 0x0014BEF3 File Offset: 0x0014A0F3
		public override bool ArcIntersect(out Vector3 oPosition, Vector3 iOrigin, Vector3 iDirection, float iRange, float iAngle, float iHeightDifference)
		{
			if (!this.mActive)
			{
				oPosition = this.Position;
				return false;
			}
			return base.ArcIntersect(out oPosition, iOrigin, iDirection, iRange, iAngle, iHeightDifference);
		}

		// Token: 0x06002A3D RID: 10813 RVA: 0x0014BF1A File Offset: 0x0014A11A
		internal bool IgnoreElements(Elements iElements)
		{
			return (iElements & Elements.Fire) == Elements.None;
		}

		// Token: 0x04002D97 RID: 11671
		private const float ACTIVE_TIME = 10f;

		// Token: 0x04002D98 RID: 11672
		private const float INITIAL_RADIUS = 2f;

		// Token: 0x04002D99 RID: 11673
		private VisualEffectReference mEffectRef;

		// Token: 0x04002D9A RID: 11674
		private int mMistEffect = "cthulhu_mist".GetHashCodeCustom();

		// Token: 0x04002D9B RID: 11675
		private VisualEffectReference mFireEffectRef;

		// Token: 0x04002D9C RID: 11676
		private int mMistOnFireEffect = "cthulhu_mist_on_fire".GetHashCodeCustom();

		// Token: 0x04002D9D RID: 11677
		private int mPlayerCharmEffect = Charm.CHARM_EFFECT;

		// Token: 0x04002D9E RID: 11678
		private static readonly int STARSPAWN_HASH = "starspawn".GetHashCodeCustom();

		// Token: 0x04002D9F RID: 11679
		private HitList mHitList;

		// Token: 0x04002DA0 RID: 11680
		private float mTimer;

		// Token: 0x04002DA1 RID: 11681
		private bool mActive;
	}
}
