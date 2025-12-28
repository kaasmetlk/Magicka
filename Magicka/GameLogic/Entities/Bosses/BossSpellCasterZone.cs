using System;
using JigLibX.Geometry;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Microsoft.Xna.Framework;
using XNAnimation;
using XNAnimation.Controllers;

namespace Magicka.GameLogic.Entities.Bosses
{
	// Token: 0x020002A8 RID: 680
	internal class BossSpellCasterZone : BossDamageZone, ISpellCaster, IStatusEffected, IDamageable
	{
		// Token: 0x06001488 RID: 5256 RVA: 0x0007F8E0 File Offset: 0x0007DAE0
		public BossSpellCasterZone(PlayState iPlayState, IBossSpellCaster iParent, AnimationController iController, int iCastBoneIndex, int iWeaponBoneIndex, int iIndex, float iRadius, params Primitive[] iPrimitives) : base(iPlayState, iParent, iIndex, iRadius, iPrimitives)
		{
			this.mController = iController;
			for (int i = 0; i < this.mController.Skeleton.Count; i++)
			{
				SkinnedModelBone skinnedModelBone = this.mController.Skeleton[i];
				if ((int)skinnedModelBone.Index == iCastBoneIndex)
				{
					this.mCastAttach.mIndex = (int)skinnedModelBone.Index;
					this.mCastAttach.mBindPose = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Invert(ref this.mCastAttach.mBindPose, out this.mCastAttach.mBindPose);
					Matrix.Multiply(ref BossSpellCasterZone.YROT, ref this.mCastAttach.mBindPose, out this.mCastAttach.mBindPose);
				}
				else if ((int)skinnedModelBone.Index == iWeaponBoneIndex)
				{
					this.mWeaponAttach.mIndex = (int)skinnedModelBone.Index;
					this.mWeaponAttach.mBindPose = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Invert(ref this.mWeaponAttach.mBindPose, out this.mWeaponAttach.mBindPose);
					Matrix.Multiply(ref BossSpellCasterZone.YROT, ref this.mWeaponAttach.mBindPose, out this.mWeaponAttach.mBindPose);
				}
			}
			this.mSpellCasterParent = iParent;
		}

		// Token: 0x17000543 RID: 1347
		// (get) Token: 0x06001489 RID: 5257 RVA: 0x0007FA0C File Offset: 0x0007DC0C
		public int Index
		{
			get
			{
				return this.mIndex;
			}
		}

		// Token: 0x0600148A RID: 5258 RVA: 0x0007FA14 File Offset: 0x0007DC14
		public new float ResistanceAgainst(Elements iElement)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = MathHelper.Clamp(num / 300f + num2, -1f, 1f);
			return 1f - num3;
		}

		// Token: 0x17000544 RID: 1348
		// (get) Token: 0x0600148B RID: 5259 RVA: 0x0007FA4D File Offset: 0x0007DC4D
		public AnimationController AnimationController
		{
			get
			{
				return this.mController;
			}
		}

		// Token: 0x0600148C RID: 5260 RVA: 0x0007FA55 File Offset: 0x0007DC55
		public void AddSelfShield(Spell iSpell)
		{
			this.mSpellCasterParent.AddSelfShield(this.mIndex, iSpell);
		}

		// Token: 0x0600148D RID: 5261 RVA: 0x0007FA69 File Offset: 0x0007DC69
		public void RemoveSelfShield(Character.SelfShieldType iType)
		{
			this.mSpellCasterParent.RemoveSelfShield(this.mIndex, iType);
		}

		// Token: 0x17000545 RID: 1349
		// (get) Token: 0x0600148E RID: 5262 RVA: 0x0007FA80 File Offset: 0x0007DC80
		public new Vector3 Direction
		{
			get
			{
				return this.mBody.Orientation.Forward;
			}
		}

		// Token: 0x17000546 RID: 1350
		// (get) Token: 0x0600148F RID: 5263 RVA: 0x0007FAA0 File Offset: 0x0007DCA0
		public CastType CastType
		{
			get
			{
				return this.mSpellCasterParent.CastType(this.mIndex);
			}
		}

		// Token: 0x06001490 RID: 5264 RVA: 0x0007FAB3 File Offset: 0x0007DCB3
		public MissileEntity GetMissileInstance()
		{
			return MissileEntity.GetInstance(this.mPlayState);
		}

		// Token: 0x17000547 RID: 1351
		// (get) Token: 0x06001491 RID: 5265 RVA: 0x0007FAC0 File Offset: 0x0007DCC0
		public Matrix CastSource
		{
			get
			{
				Matrix result;
				Matrix.Multiply(ref this.mCastAttach.mBindPose, ref this.mController.SkinnedBoneTransforms[this.mCastAttach.mIndex], out result);
				return result;
			}
		}

		// Token: 0x17000548 RID: 1352
		// (get) Token: 0x06001492 RID: 5266 RVA: 0x0007FAFC File Offset: 0x0007DCFC
		public Matrix WeaponSource
		{
			get
			{
				Matrix result;
				Matrix.Multiply(ref this.mWeaponAttach.mBindPose, ref this.mController.SkinnedBoneTransforms[this.mWeaponAttach.mIndex], out result);
				return result;
			}
		}

		// Token: 0x17000549 RID: 1353
		// (get) Token: 0x06001493 RID: 5267 RVA: 0x0007FB37 File Offset: 0x0007DD37
		// (set) Token: 0x06001494 RID: 5268 RVA: 0x0007FB4A File Offset: 0x0007DD4A
		public float SpellPower
		{
			get
			{
				return this.mSpellCasterParent.SpellPower(this.mIndex);
			}
			set
			{
				this.mSpellCasterParent.SpellPower(this.mIndex, value);
			}
		}

		// Token: 0x1700054A RID: 1354
		// (get) Token: 0x06001495 RID: 5269 RVA: 0x0007FB5E File Offset: 0x0007DD5E
		// (set) Token: 0x06001496 RID: 5270 RVA: 0x0007FB71 File Offset: 0x0007DD71
		public SpellEffect CurrentSpell
		{
			get
			{
				return this.mSpellCasterParent.CurrentSpell(this.mIndex);
			}
			set
			{
				this.mSpellCasterParent.CurrentSpell(this.mIndex, value);
			}
		}

		// Token: 0x06001497 RID: 5271 RVA: 0x0007FB85 File Offset: 0x0007DD85
		public virtual bool HasPassiveAbility(Item.PassiveAbilities iAbility)
		{
			return false;
		}

		// Token: 0x040015E0 RID: 5600
		private IBossSpellCaster mSpellCasterParent;

		// Token: 0x040015E1 RID: 5601
		private AnimationController mController;

		// Token: 0x040015E2 RID: 5602
		private BindJoint mCastAttach;

		// Token: 0x040015E3 RID: 5603
		private BindJoint mWeaponAttach;

		// Token: 0x040015E4 RID: 5604
		private static Matrix YROT = Matrix.CreateRotationY(3.1415927f);
	}
}
