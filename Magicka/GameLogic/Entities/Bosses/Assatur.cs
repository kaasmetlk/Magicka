using System;
using System.Collections.Generic;
using JigLibX.Geometry;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

namespace Magicka.GameLogic.Entities.Bosses
{
	// Token: 0x0200064D RID: 1613
	public class Assatur : BossStatusEffected, IBossSpellCaster, IBoss
	{
		// Token: 0x06003126 RID: 12582 RVA: 0x00194238 File Offset: 0x00192438
		static Assatur()
		{
			Grimnir2.SpellData spellData = default(Grimnir2.SpellData);
			spellData.SPELL = default(Spell);
			spellData.SPELL.Element = (Elements.Earth | Elements.Fire | Elements.Shield);
			spellData.SPELL.EarthMagnitude = 1f;
			spellData.SPELL.ShieldMagnitude = 1f;
			spellData.SPELL.FireMagnitude = 3f;
			spellData.CASTTYPE = Magicka.GameLogic.Spells.CastType.Weapon;
			Assatur.SPELLS[0] = spellData;
			spellData.SPELL = default(Spell);
			spellData.SPELL.Element = (Elements.Shield | Elements.Ice);
			spellData.SPELL.ShieldMagnitude = 1f;
			spellData.SPELL.IceMagnitude = 4f;
			spellData.CASTTYPE = Magicka.GameLogic.Spells.CastType.Force;
			spellData.SPELLPOWER = 1f;
			Assatur.SPELLS[1] = spellData;
			spellData.SPELL = default(Spell);
			spellData.SPELL.Element = Elements.Shield;
			spellData.SPELL.ShieldMagnitude = 1f;
			spellData.CASTTYPE = Magicka.GameLogic.Spells.CastType.Force;
			spellData.SPELLPOWER = 1f;
			Assatur.SPELLS[2] = spellData;
		}

		// Token: 0x06003127 RID: 12583 RVA: 0x00194640 File Offset: 0x00192840
		public Assatur(PlayState iPlayState)
		{
			this.mPlayState = iPlayState;
			this.mHitList = new HitList(32);
			this.mMissiles = new List<MissileEntity>(8);
			SkinnedModel skinnedModel;
			Model model;
			lock (Game.Instance.GraphicsDevice)
			{
				skinnedModel = this.mPlayState.Content.Load<SkinnedModel>("Models/Bosses/Assatur/Assatur");
				model = this.mPlayState.Content.Load<Model>("Models/Bosses/Assatur/Assatur_staff");
				this.mIceCubeMap = this.mPlayState.Content.Load<TextureCube>("EffectTextures/iceCube");
				this.mIceCubeNormalMap = this.mPlayState.Content.Load<TextureCube>("EffectTextures/iceCube_NRM");
			}
			this.mController = new AnimationController();
			this.mController.Skeleton = skinnedModel.SkeletonBones;
			this.mTentacleController = new AnimationController();
			this.mTentacleController.Skeleton = skinnedModel.SkeletonBones;
			this.mClips = new AnimationClip[18];
			this.mClips[11] = skinnedModel.AnimationClips["cast_blade"];
			this.mClips[8] = skinnedModel.AnimationClips["cast_blizzard"];
			this.mClips[10] = skinnedModel.AnimationClips["cast_ground"];
			this.mClips[15] = skinnedModel.AnimationClips["idle"];
			this.mClips[7] = skinnedModel.AnimationClips["cast_self"];
			this.mClips[6] = skinnedModel.AnimationClips["cast_emperor"];
			this.mClips[5] = skinnedModel.AnimationClips["cast_sky"];
			this.mClips[1] = skinnedModel.AnimationClips["cast_mm_left"];
			this.mClips[2] = skinnedModel.AnimationClips["cast_mm_right"];
			this.mClips[3] = skinnedModel.AnimationClips["cast_mm_sides"];
			this.mClips[0] = skinnedModel.AnimationClips["cast_mm_prepare"];
			this.mClips[14] = skinnedModel.AnimationClips["cast_nullify"];
			this.mClips[12] = skinnedModel.AnimationClips["cast_shield"];
			this.mClips[13] = skinnedModel.AnimationClips["cast_staff"];
			this.mClips[9] = skinnedModel.AnimationClips["cast_spray"];
			this.mClips[4] = skinnedModel.AnimationClips["cast_focus"];
			this.mClips[16] = skinnedModel.AnimationClips["tentacles"];
			this.mClips[17] = skinnedModel.AnimationClips["die"];
			this.mStates = new Assatur.AssaturState[17];
			this.mStates[9] = new Assatur.CastBlade();
			this.mStates[6] = new Assatur.CastBlizzard();
			this.mStates[8] = new Assatur.CastTimeWarp();
			this.mStates[5] = new Assatur.CastLife();
			this.mStates[4] = new Assatur.CastLightning();
			this.mStates[1] = new Assatur.CastMMState();
			this.mStates[3] = new Assatur.CastMeteor();
			this.mStates[12] = new Assatur.CastNullifyState();
			this.mStates[10] = new Assatur.CastShield();
			this.mStates[11] = new Assatur.CastStaffBeamState();
			this.mStates[7] = new Assatur.CastWater();
			this.mStates[2] = new Assatur.CastVortex();
			this.mStates[13] = new Assatur.CastBarrier();
			this.mStates[14] = new Assatur.IntroState();
			this.mStates[0] = new Assatur.BattleState();
			this.mStates[15] = new Assatur.DeathState();
			int iWeaponBoneIndex = 0;
			int iCastBoneIndex = 0;
			Matrix matrix;
			Matrix.CreateRotationY(3.1415927f, out matrix);
			foreach (SkinnedModelBone skinnedModelBone in skinnedModel.SkeletonBones)
			{
				if (skinnedModelBone.Name.Equals("SpineLower", StringComparison.OrdinalIgnoreCase))
				{
					this.mDamageJoint.mIndex = (int)skinnedModelBone.Index;
					this.mDamageJoint.mBindPose = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mDamageJoint.mBindPose, ref matrix, out this.mDamageJoint.mBindPose);
					Matrix.Invert(ref this.mDamageJoint.mBindPose, out this.mDamageJoint.mBindPose);
				}
				else if (skinnedModelBone.Name.Equals("joint1", StringComparison.OrdinalIgnoreCase))
				{
					this.mStaffJoint.mIndex = (int)skinnedModelBone.Index;
					this.mStaffJoint.mBindPose = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Invert(ref this.mStaffJoint.mBindPose, out this.mStaffJoint.mBindPose);
					Matrix.Multiply(ref matrix, ref this.mStaffJoint.mBindPose, out this.mStaffJoint.mBindPose);
				}
				else if (skinnedModelBone.Name.Equals("RightAttach", StringComparison.OrdinalIgnoreCase))
				{
					iWeaponBoneIndex = (int)skinnedModelBone.Index;
					this.mRightHandJoint.mIndex = (int)skinnedModelBone.Index;
					this.mRightHandJoint.mBindPose = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Invert(ref this.mRightHandJoint.mBindPose, out this.mRightHandJoint.mBindPose);
					Matrix.Multiply(ref matrix, ref this.mRightHandJoint.mBindPose, out this.mRightHandJoint.mBindPose);
				}
				else if (skinnedModelBone.Name.Equals("LeftAttach", StringComparison.OrdinalIgnoreCase))
				{
					iCastBoneIndex = (int)skinnedModelBone.Index;
					this.mLeftHandJoint.mIndex = (int)skinnedModelBone.Index;
					this.mLeftHandJoint.mBindPose = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Invert(ref this.mLeftHandJoint.mBindPose, out this.mLeftHandJoint.mBindPose);
					Matrix.Multiply(ref matrix, ref this.mLeftHandJoint.mBindPose, out this.mLeftHandJoint.mBindPose);
				}
			}
			Damage iDamage = new Damage(AttackProperties.Damage, Elements.Arcane, 100f, 1f);
			this.mMissileConditions = new ConditionCollection();
			this.mMissileConditions[0].Condition.EventConditionType = EventConditionType.Default;
			this.mMissileConditions[0].Condition.Repeat = true;
			this.mMissileConditions[0].Add(new EventStorage(new PlayEffectEvent(Assatur.MISSILE_TRAIL, true)));
			this.mMissileConditions[1].Condition.EventConditionType = EventConditionType.Hit;
			this.mMissileConditions[1].Add(new EventStorage(new PlayEffectEvent(Assatur.MISSILE_EXPLODE)));
			this.mMissileConditions[1].Add(new EventStorage(default(RemoveEvent)));
			this.mMissileConditions[1].Add(new EventStorage(new DamageEvent(iDamage)));
			this.mMissileConditions[2].Condition.EventConditionType = EventConditionType.Collision;
			this.mMissileConditions[2].Add(new EventStorage(new PlayEffectEvent(Assatur.MISSILE_EXPLODE)));
			this.mMissileConditions[2].Add(new EventStorage(default(RemoveEvent)));
			this.mMissileConditions[2].Add(new EventStorage(new DamageEvent(iDamage)));
			this.mMissileConditions[3].Condition.EventConditionType = EventConditionType.Timer;
			this.mMissileConditions[3].Condition.Time = 5f;
			this.mMissileConditions[3].Add(new EventStorage(default(RemoveEvent)));
			this.mMissileConditions[4].Condition.EventConditionType = EventConditionType.Damaged;
			this.mMissileConditions[4].Add(new EventStorage(new PlayEffectEvent(Assatur.MISSILE_EXPLODE)));
			this.mMissileConditions[4].Add(new EventStorage(default(RemoveEvent)));
			Capsule capsule = new Capsule(new Vector3(0f, -0.75f, 0f), Matrix.CreateRotationX(-1.5707964f), 1.5f, 5f);
			this.mBodyZone = new BossSpellCasterZone(iPlayState, this, this.mController, iCastBoneIndex, iWeaponBoneIndex, 0, 1.5f, new Primitive[]
			{
				capsule
			});
			Helper.SkinnedModelDeferredMaterialFromBasicEffect(skinnedModel.Model.Meshes[0].MeshParts[0].Effect as SkinnedModelBasicEffect, out this.mMaterial);
			this.mBoundingSphere = skinnedModel.Model.Meshes[0].BoundingSphere;
			this.mRenderData = new Assatur.RenderData[3];
			this.mStaffRenderData = new Assatur.StaffRenderData[3];
			for (int i = 0; i < 3; i++)
			{
				this.mRenderData[i] = new Assatur.RenderData();
				this.mRenderData[i].SetMesh(skinnedModel.Model.Meshes[0].VertexBuffer, skinnedModel.Model.Meshes[0].IndexBuffer, skinnedModel.Model.Meshes[0].MeshParts[0], 0, 3, 4);
				this.mRenderData[i].mMaterial = this.mMaterial;
				this.mStaffRenderData[i] = new Assatur.StaffRenderData();
				this.mStaffRenderData[i].SetMesh(model.Meshes[0], model.Meshes[0].MeshParts[0], 4, 0, 5);
			}
		}

		// Token: 0x06003128 RID: 12584 RVA: 0x00194FCC File Offset: 0x001931CC
		public void Initialize(ref Matrix iOrientation, int iUniqueID)
		{
			this.Initialize(ref iOrientation);
		}

		// Token: 0x06003129 RID: 12585 RVA: 0x00194FD8 File Offset: 0x001931D8
		public void Initialize(ref Matrix iOrientation)
		{
			this.mDead = false;
			this.mAssaturFloatTimer = 0f;
			this.mOrientation = iOrientation;
			this.mHitPoints = 50000f;
			for (int i = 0; i < this.mResistances.Length; i++)
			{
				this.mResistances[i].ResistanceAgainst = Spell.ElementFromIndex(i);
				this.mResistances[i].Modifier = 0f;
				if (this.mResistances[i].ResistanceAgainst == Elements.Water)
				{
					this.mResistances[i].Multiplier = 0f;
				}
				else if (this.mResistances[i].ResistanceAgainst == Elements.Cold)
				{
					this.mResistances[i].Multiplier = 0.05f;
				}
				else
				{
					this.mResistances[i].Multiplier = 1f;
				}
			}
			this.mDeathScale = 1f;
			this.mVortexPositions = new Vector3[Assatur.VORTEX_LOCATORS.Length];
			this.mStartOrientations = new Matrix[4];
			Locator locator;
			this.mPlayState.Level.CurrentScene.GetLocator("start0".GetHashCodeCustom(), out locator);
			this.mChasmOrientation = locator.Transform;
			this.mVortexPositions[0] = locator.Transform.Translation;
			this.mStartOrientations[0] = locator.Transform;
			this.mPlayState.Level.CurrentScene.GetLocator("start1".GetHashCodeCustom(), out locator);
			this.mVortexPositions[1] = locator.Transform.Translation;
			this.mStartOrientations[1] = locator.Transform;
			this.mPlayState.Level.CurrentScene.GetLocator("start2".GetHashCodeCustom(), out locator);
			this.mVortexPositions[2] = locator.Transform.Translation;
			this.mStartOrientations[2] = locator.Transform;
			this.mPlayState.Level.CurrentScene.GetLocator("start3".GetHashCodeCustom(), out locator);
			this.mVortexPositions[3] = locator.Transform.Translation;
			this.mStartOrientations[3] = locator.Transform;
			this.mPlayState.Level.CurrentScene.GetLocator("assatur_move0".GetHashCodeCustom(), out locator);
			this.mMoveLoc0 = locator.Transform;
			this.mPlayState.Level.CurrentScene.GetLocator("assatur_move1".GetHashCodeCustom(), out locator);
			this.mMoveLoc1 = locator.Transform;
			this.mBodyZone.Initialize("#boss_n15".GetHashCodeCustom());
			this.mPlayState.EntityManager.AddEntity(this.mBodyZone);
			this.mCurrentState = this.mStates[14];
			this.mCurrentState.OnEnter(this);
			this.mMaxHitPoints = 50000f;
			this.mHitPoints = this.MaxHitPoints;
			this.mTarget = Game.Instance.Players[0].Avatar;
			this.mTargettingPower = 1f;
			this.mTargetOrientation = this.mOrientation;
			Vector3 translation = this.mOrientation.Translation;
			translation.Y += 40f;
			this.mOrientation.Translation = translation;
			this.mLastState = this.mCurrentState;
		}

		// Token: 0x0600312A RID: 12586 RVA: 0x00195370 File Offset: 0x00193570
		public void UpdateBoss(DataChannel iDataChannel, float iDeltaTime, bool iFightStarted)
		{
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				this.mNetworkUpdateTimer -= iDeltaTime;
				if (this.mNetworkUpdateTimer <= 0f)
				{
					this.mNetworkUpdateTimer = 0.033333335f;
					this.NetworkUpdate();
				}
			}
			iDeltaTime /= this.mPlayState.TimeModifier * this.mPlayState.TimeMultiplier;
			this.UpdateDamage(iDeltaTime);
			this.UpdateStatusEffects(iDeltaTime);
			this.mShieldTimer -= iDeltaTime;
			if (this.mHitPoints <= 0f && !(this.mCurrentState is Assatur.DeathState))
			{
				this.ChangeState(Assatur.States.Dead);
			}
			Vector3 translation = this.mOrientation.Translation;
			translation.Y += (this.mTargetOrientation.Translation.Y - this.mOrientation.Translation.Y) * iDeltaTime * 2f;
			this.mOrientation.Translation = translation;
			this.mAssaturFloatTimer += iDeltaTime;
			if (this.mTarget != null)
			{
				Vector3 position = this.mTarget.Position;
				this.mTargetPosition.X = this.mTargetPosition.X + (position.X - this.mTargetPosition.X) * iDeltaTime * this.mTargettingPower;
				this.mTargetPosition.Y = this.mTargetPosition.Y + (position.Y - this.mTargetPosition.Y) * iDeltaTime * this.mTargettingPower;
				this.mTargetPosition.Z = this.mTargetPosition.Z + (position.Z - this.mTargetPosition.Z) * iDeltaTime * this.mTargettingPower;
			}
			Matrix matrix = this.mOrientation;
			matrix.Translation -= new Vector3(0f, 5f, 0f);
			if (this.mSpellEffect != null)
			{
				if (this.mSpellEffect.CastType == Magicka.GameLogic.Spells.CastType.Weapon)
				{
					this.mSpellEffect.AnimationEnd(this.mBodyZone);
				}
				float num;
				if (!this.mSpellEffect.CastUpdate(iDeltaTime, this.mBodyZone, out num))
				{
					this.mSpellEffect.DeInitialize(this.mBodyZone);
					this.mSpellEffect = null;
				}
			}
			if (this.mShieldTimer <= 0f && this.mProtectiveShield != null && !this.mProtectiveShield.Dead)
			{
				this.mShieldTimer += 0.2f;
				this.mProtectiveShield.Damage(100f);
			}
			this.mMagickEffect = this.mSpineOrientation;
			this.mMagickEffect.Translation = new Vector3(this.mMagickEffect.Translation.X, this.mMagickEffect.Translation.Y + 3f, this.mMagickEffect.Translation.Z + 3f);
			this.mSpineOrientation = this.mDamageJoint.mBindPose;
			Matrix.Multiply(ref this.mSpineOrientation, ref this.mController.SkinnedBoneTransforms[this.mDamageJoint.mIndex], out this.mSpineOrientation);
			if (iFightStarted)
			{
				this.mCurrentState.OnUpdate(iDeltaTime, this);
			}
			this.mController.Speed = 1f;
			if (base.HasStatus(StatusEffects.Frozen))
			{
				this.mRenderData[(int)iDataChannel].mMaterial.Bloat = 0.1f;
				this.mRenderData[(int)iDataChannel].mMaterial.EmissiveAmount = 3f;
				this.mRenderData[(int)iDataChannel].mMaterial.SpecularBias = 0.8f;
				this.mRenderData[(int)iDataChannel].mMaterial.SpecularPower = 20f;
				this.mRenderData[(int)iDataChannel].mMaterial.CubeMapRotation = Matrix.Identity;
				this.mRenderData[(int)iDataChannel].mMaterial.CubeMap = this.mIceCubeMap;
				this.mRenderData[(int)iDataChannel].mMaterial.CubeNormalMap = this.mIceCubeNormalMap;
				this.mRenderData[(int)iDataChannel].mMaterial.CubeMapColor = Vector4.One;
				this.mRenderData[(int)iDataChannel].mMaterial.CubeMapEnabled = true;
				this.mRenderData[(int)iDataChannel].mMaterial.CubeNormalMapEnabled = true;
				this.mRenderData[(int)iDataChannel].mMaterial.CubeMapColor.W = 1f - (float)Math.Pow(0.20000000298023224, (double)base.StatusMagnitude(StatusEffects.Frozen));
				this.mController.Speed = 0f;
			}
			else
			{
				if (base.HasStatus(StatusEffects.Cold))
				{
					this.mController.Speed *= 0.2f;
				}
				this.mRenderData[(int)iDataChannel].mMaterial.Bloat = 0f;
				this.mRenderData[(int)iDataChannel].mMaterial.EmissiveAmount = this.mMaterial.EmissiveAmount;
				this.mRenderData[(int)iDataChannel].mMaterial.SpecularBias = this.mMaterial.SpecularBias;
				this.mRenderData[(int)iDataChannel].mMaterial.SpecularPower = this.mMaterial.SpecularPower;
				this.mRenderData[(int)iDataChannel].mMaterial.CubeMapEnabled = false;
				this.mRenderData[(int)iDataChannel].mMaterial.CubeNormalMapEnabled = false;
			}
			Matrix matrix2;
			Matrix.Multiply(ref matrix, this.mDeathScale, out matrix2);
			matrix2.Translation = matrix.Translation;
			this.mController.PreUpdate(iDeltaTime, ref matrix2);
			this.mTentacleController.PreUpdate(iDeltaTime, ref matrix2);
			Array.Copy(this.mTentacleController.LocalBonePoses, this.mTentaclesIndex, this.mController.LocalBonePoses, this.mTentaclesIndex, this.mTentacleBones);
			this.mController.UpdateAbsoluteBoneTransforms(ref matrix2);
			this.mDamageFlashTimer = Math.Max(this.mDamageFlashTimer - iDeltaTime, 0f);
			Array.Copy(this.mController.SkinnedBoneTransforms, this.mRenderData[(int)iDataChannel].mBones, this.mRenderData[(int)iDataChannel].mBones.Length);
			this.mBoundingSphere.Center = this.mOrientation.Translation;
			this.mRenderData[(int)iDataChannel].Flash = this.mDamageFlashTimer * 10f;
			this.mRenderData[(int)iDataChannel].mBoundingSphere = this.mBoundingSphere;
			this.mRenderData[(int)iDataChannel].Damage = 1f - this.mHitPoints / 50000f;
			this.mPlayState.Scene.AddRenderableObject(iDataChannel, this.mRenderData[(int)iDataChannel]);
			Matrix mBindPose = this.mStaffJoint.mBindPose;
			Matrix.Multiply(ref mBindPose, ref this.mController.SkinnedBoneTransforms[this.mStaffJoint.mIndex], out mBindPose);
			if (this.mUseRailStaffOrientation)
			{
				this.mStaffRenderData[(int)iDataChannel].WorldOrientation = this.mRailStaffOrientation;
			}
			else
			{
				this.mStaffRenderData[(int)iDataChannel].WorldOrientation = mBindPose;
			}
			this.mStaffRenderData[(int)iDataChannel].mBoundingSphere = this.mBoundingSphere;
			this.mPlayState.Scene.AddRenderableObject(iDataChannel, this.mStaffRenderData[(int)iDataChannel]);
			Vector3 translation2 = this.mOrientation.Translation;
			mBindPose = this.mOrientation;
			mBindPose.Translation = default(Vector3);
			this.mBodyZone.Body.MoveTo(translation2, mBindPose);
			mBindPose = this.mDamageJoint.mBindPose;
			Matrix.Multiply(ref mBindPose, ref this.mController.SkinnedBoneTransforms[this.mDamageJoint.mIndex], out mBindPose);
			translation2 = mBindPose.Translation;
			if (this.mProtectiveShield != null)
			{
				if (this.mProtectiveShield.Dead)
				{
					this.mProtectiveShield = null;
				}
				else
				{
					mBindPose = this.mOrientation;
					translation2 = this.mSpineOrientation.Translation;
					mBindPose.Translation = default(Vector3);
					this.mProtectiveShield.Body.MoveTo(translation2, mBindPose);
				}
			}
			for (int i = 0; i < this.mMissiles.Count; i++)
			{
				if (this.mMissiles[i].Dead)
				{
					this.mMissiles.RemoveAt(i--);
				}
			}
		}

		// Token: 0x0600312B RID: 12587 RVA: 0x00195B38 File Offset: 0x00193D38
		public Matrix StaffOrientation()
		{
			Matrix mBindPose = this.mStaffJoint.mBindPose;
			Matrix.Multiply(ref mBindPose, ref this.mController.SkinnedBoneTransforms[this.mStaffJoint.mIndex], out mBindPose);
			return mBindPose;
		}

		// Token: 0x0600312C RID: 12588 RVA: 0x00195B78 File Offset: 0x00193D78
		public unsafe void ChangeState(Assatur.States iState)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					Assatur.ChangeStateMessage changeStateMessage;
					changeStateMessage.NewState = iState;
					BossFight.Instance.SendMessage<Assatur.ChangeStateMessage>(this, 1, (void*)(&changeStateMessage), true);
				}
				this.mCurrentState.OnExit(this);
				this.mLastState = this.mCurrentState;
				this.mCurrentState = this.mStates[(int)iState];
				this.mCurrentState.OnEnter(this);
			}
		}

		// Token: 0x0600312D RID: 12589 RVA: 0x00195BEC File Offset: 0x00193DEC
		private unsafe void SelectTarget()
		{
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				return;
			}
			Player[] players = Game.Instance.Players;
			int num = Assatur.sRandom.Next(4);
			for (int i = 0; i < players.Length; i++)
			{
				if (players[(i + num) % 4].Playing)
				{
					Player player = players[(i + num) % 4];
					if (player.Avatar != null && !player.Avatar.Dead)
					{
						this.mTarget = player.Avatar;
						break;
					}
				}
			}
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				Assatur.ChangeTargetMessage changeTargetMessage;
				if (this.mTarget != null)
				{
					changeTargetMessage.Target = this.mTarget.Handle;
				}
				else
				{
					changeTargetMessage.Target = ushort.MaxValue;
				}
				BossFight.Instance.SendMessage<Assatur.ChangeTargetMessage>(this, 2, (void*)(&changeTargetMessage), true);
			}
		}

		// Token: 0x0600312E RID: 12590 RVA: 0x00195CAA File Offset: 0x00193EAA
		public void DeInitialize()
		{
		}

		// Token: 0x17000B8E RID: 2958
		// (get) Token: 0x0600312F RID: 12591 RVA: 0x00195CAC File Offset: 0x00193EAC
		public override bool Dead
		{
			get
			{
				return this.mDead;
			}
		}

		// Token: 0x17000B8F RID: 2959
		// (get) Token: 0x06003130 RID: 12592 RVA: 0x00195CB4 File Offset: 0x00193EB4
		public float MaxHitPoints
		{
			get
			{
				return 50000f;
			}
		}

		// Token: 0x17000B90 RID: 2960
		// (get) Token: 0x06003131 RID: 12593 RVA: 0x00195CBB File Offset: 0x00193EBB
		public float HitPoints
		{
			get
			{
				return this.mHitPoints;
			}
		}

		// Token: 0x06003132 RID: 12594 RVA: 0x00195CC3 File Offset: 0x00193EC3
		public bool AddImpulseVelocity(Vector3 iDirection, float iElevation, float iMassPower, float iDistance)
		{
			return false;
		}

		// Token: 0x06003133 RID: 12595 RVA: 0x00195CC8 File Offset: 0x00193EC8
		public DamageResult Damage(int iPartIndex, Damage iDamage, Entity iAttacker, ref Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			if (iAttacker is BossCollisionZone)
			{
				return DamageResult.None;
			}
			DamageResult damageResult = this.Damage(iDamage, iAttacker, iAttackPosition, iFeatures);
			if ((damageResult & DamageResult.Hit) == DamageResult.Hit | (damageResult & DamageResult.Damaged) == DamageResult.Damaged)
			{
				if (this.mCurrentState is Assatur.CastLife)
				{
					this.ChangeState(Assatur.States.Battle);
				}
				this.mDamageFlashTimer = 0.1f;
			}
			return damageResult;
		}

		// Token: 0x06003134 RID: 12596 RVA: 0x00195D20 File Offset: 0x00193F20
		public void Damage(int iPartIndex, float iDamage, Elements iElement)
		{
			base.Damage(iDamage, iElement);
		}

		// Token: 0x06003135 RID: 12597 RVA: 0x00195D2C File Offset: 0x00193F2C
		public void ScriptMessage(BossMessages iMessage)
		{
			if (iMessage != BossMessages.KillAssatur)
			{
				return;
			}
			this.ChangeState(Assatur.States.Dead);
		}

		// Token: 0x06003136 RID: 12598 RVA: 0x00195D49 File Offset: 0x00193F49
		public void SetSlow(int iIndex)
		{
		}

		// Token: 0x06003137 RID: 12599 RVA: 0x00195D4B File Offset: 0x00193F4B
		public void GetRandomPositionOnCollisionSkin(int iIndex, out Vector3 oPosition)
		{
			oPosition = default(Vector3);
		}

		// Token: 0x06003138 RID: 12600 RVA: 0x00195D54 File Offset: 0x00193F54
		public bool HasStatus(int iIndex, StatusEffects iStatus)
		{
			return false;
		}

		// Token: 0x06003139 RID: 12601 RVA: 0x00195D57 File Offset: 0x00193F57
		public float StatusMagnitude(int iIndex, StatusEffects iStatus)
		{
			return 0f;
		}

		// Token: 0x0600313A RID: 12602 RVA: 0x00195D5E File Offset: 0x00193F5E
		public new StatusEffect[] GetStatusEffects()
		{
			return null;
		}

		// Token: 0x0600313B RID: 12603 RVA: 0x00195D61 File Offset: 0x00193F61
		public void AddSelfShield(int iIndex, Spell iSpell)
		{
		}

		// Token: 0x0600313C RID: 12604 RVA: 0x00195D63 File Offset: 0x00193F63
		public void RemoveSelfShield(int iIndex, Character.SelfShieldType iType)
		{
		}

		// Token: 0x0600313D RID: 12605 RVA: 0x00195D65 File Offset: 0x00193F65
		public CastType CastType(int iIndex)
		{
			return Magicka.GameLogic.Spells.CastType.None;
		}

		// Token: 0x0600313E RID: 12606 RVA: 0x00195D68 File Offset: 0x00193F68
		public float SpellPower(int iIndex)
		{
			return this.mSpellPower;
		}

		// Token: 0x0600313F RID: 12607 RVA: 0x00195D70 File Offset: 0x00193F70
		public void SpellPower(int iIndex, float iSpellPower)
		{
			this.mSpellPower = iSpellPower;
		}

		// Token: 0x06003140 RID: 12608 RVA: 0x00195D79 File Offset: 0x00193F79
		public SpellEffect CurrentSpell(int iIndex)
		{
			return this.mSpellEffect;
		}

		// Token: 0x06003141 RID: 12609 RVA: 0x00195D81 File Offset: 0x00193F81
		public void CurrentSpell(int iIndex, SpellEffect iEffect)
		{
			this.mSpellEffect = iEffect;
		}

		// Token: 0x17000B91 RID: 2961
		// (get) Token: 0x06003142 RID: 12610 RVA: 0x00195D8A File Offset: 0x00193F8A
		protected override BossDamageZone Entity
		{
			get
			{
				return this.mBodyZone;
			}
		}

		// Token: 0x17000B92 RID: 2962
		// (get) Token: 0x06003143 RID: 12611 RVA: 0x00195D92 File Offset: 0x00193F92
		protected override float Radius
		{
			get
			{
				return this.mBodyZone.Radius;
			}
		}

		// Token: 0x17000B93 RID: 2963
		// (get) Token: 0x06003144 RID: 12612 RVA: 0x00195D9F File Offset: 0x00193F9F
		protected override float Length
		{
			get
			{
				return this.mBodyZone.Radius * 1.5f;
			}
		}

		// Token: 0x17000B94 RID: 2964
		// (get) Token: 0x06003145 RID: 12613 RVA: 0x00195DB2 File Offset: 0x00193FB2
		protected override int BloodEffect
		{
			get
			{
				return Assatur.BLOOD_BLACK_EFFECT;
			}
		}

		// Token: 0x17000B95 RID: 2965
		// (get) Token: 0x06003146 RID: 12614 RVA: 0x00195DBC File Offset: 0x00193FBC
		protected override Vector3 NotifierTextPostion
		{
			get
			{
				Vector3 translation = this.mSpineOrientation.Translation;
				translation.Y += this.mBodyZone.Radius * 1.2f;
				return translation;
			}
		}

		// Token: 0x06003147 RID: 12615 RVA: 0x00195DF8 File Offset: 0x00193FF8
		private unsafe void NetworkUpdate()
		{
			NetworkServer networkServer = NetworkManager.Instance.Interface as NetworkServer;
			if (networkServer == null)
			{
				return;
			}
			Assatur.UpdateMessage updateMessage = default(Assatur.UpdateMessage);
			updateMessage.Animation = 0;
			while ((int)updateMessage.Animation < this.mClips.Length && this.mController.AnimationClip != this.mClips[(int)updateMessage.Animation])
			{
				updateMessage.Animation += 1;
			}
			updateMessage.AnimationTime = this.mController.Time;
			updateMessage.Hitpoints = this.mHitPoints;
			for (int i = 0; i < networkServer.Connections; i++)
			{
				float num = networkServer.GetLatency(i) * 0.5f;
				Assatur.UpdateMessage updateMessage2 = updateMessage;
				updateMessage2.AnimationTime += num;
				BossFight.Instance.SendMessage<Assatur.UpdateMessage>(this, 0, (void*)(&updateMessage), false, i);
			}
		}

		// Token: 0x06003148 RID: 12616 RVA: 0x00195EC8 File Offset: 0x001940C8
		public unsafe void NetworkUpdate(ref BossUpdateMessage iMsg)
		{
			if (iMsg.Type == 0)
			{
				if ((float)iMsg.TimeStamp < this.mLastNetworkUpdate)
				{
					return;
				}
				this.mLastNetworkUpdate = (float)iMsg.TimeStamp;
				Assatur.UpdateMessage updateMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&updateMessage));
				if (this.mController.AnimationClip != this.mClips[(int)updateMessage.Animation])
				{
					this.mController.StartClip(this.mClips[(int)updateMessage.Animation], false);
				}
				this.mController.Time = updateMessage.AnimationTime;
				this.mHitPoints = updateMessage.Hitpoints;
				return;
			}
			else
			{
				if (iMsg.Type == 5)
				{
					Assatur.CastShieldMessage castShieldMessage;
					BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&castShieldMessage));
					this.mProtectiveShield = (Magicka.GameLogic.Entities.Entity.GetFromHandle((int)castShieldMessage.Handle) as Shield);
					this.mProtectiveShield.Initialize(this.mBodyZone, castShieldMessage.Position, castShieldMessage.Radius, castShieldMessage.Direction, castShieldMessage.ShieldType, castShieldMessage.HitPoints, Spell.SHIELDCOLOR);
					this.mProtectiveShield.HitPoints = 5000f;
					this.mPlayState.EntityManager.AddEntity(this.mProtectiveShield);
					this.mShieldTimer = 15f;
					(this.mStates[10] as Assatur.CastShield).Shielded = true;
					return;
				}
				if (iMsg.Type == 4)
				{
					Assatur.CastBarrierMessage castBarrierMessage;
					BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&castBarrierMessage));
					return;
				}
				if (iMsg.Type == 3)
				{
					Assatur.CastMagickMessage castMagickMessage;
					BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&castMagickMessage));
					Magick magick = default(Magick);
					magick.MagickType = castMagickMessage.Magick;
					magick.Effect.Execute(this.mBodyZone, this.mPlayState);
					if (castMagickMessage.Effect != 0)
					{
						VisualEffectReference visualEffectReference;
						EffectManager.Instance.StartEffect(castMagickMessage.Effect, ref castMagickMessage.Position, ref castMagickMessage.Direction, out visualEffectReference);
						return;
					}
				}
				else if (iMsg.Type == 6)
				{
					Assatur.SpawnMMMessage spawnMMMessage;
					BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&spawnMMMessage));
					switch (spawnMMMessage.CastDir)
					{
					case 1:
					{
						Matrix matrix = this.mBodyZone.CastSource;
						(this.mStates[1] as Assatur.CastMMState).SpawnMissile(ref matrix, this, -0.75f);
						matrix = this.mBodyZone.WeaponSource;
						(this.mStates[1] as Assatur.CastMMState).SpawnMissile(ref matrix, this, -0.75f);
						return;
					}
					case 2:
					{
						Matrix castSource = this.mBodyZone.CastSource;
						(this.mStates[1] as Assatur.CastMMState).SpawnMissile(ref castSource, this, -0.25f);
						return;
					}
					case 3:
					{
						Matrix weaponSource = this.mBodyZone.WeaponSource;
						(this.mStates[1] as Assatur.CastMMState).SpawnMissile(ref weaponSource, this, -0.25f);
						return;
					}
					default:
						return;
					}
				}
				else if (iMsg.Type == 2)
				{
					Assatur.ChangeTargetMessage changeTargetMessage;
					BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&changeTargetMessage));
					if (changeTargetMessage.Target == 65535)
					{
						this.mTarget = null;
						return;
					}
					this.mTarget = (Magicka.GameLogic.Entities.Entity.GetFromHandle((int)changeTargetMessage.Target) as Character);
					return;
				}
				else if (iMsg.Type == 1)
				{
					Assatur.ChangeStateMessage changeStateMessage;
					BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&changeStateMessage));
					if (changeStateMessage.NewState != Assatur.States.None && changeStateMessage.NewState != Assatur.States.NrOfStates)
					{
						this.mCurrentState.OnExit(this);
						this.mLastState = this.mCurrentState;
						this.mCurrentState = this.mStates[(int)changeStateMessage.NewState];
						this.mCurrentState.OnEnter(this);
					}
				}
				return;
			}
		}

		// Token: 0x06003149 RID: 12617 RVA: 0x001961F9 File Offset: 0x001943F9
		public void NetworkInitialize(ref BossInitializeMessage iMsg)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600314A RID: 12618 RVA: 0x00196200 File Offset: 0x00194400
		public BossEnum GetBossType()
		{
			return BossEnum.Assatur;
		}

		// Token: 0x17000B96 RID: 2966
		// (get) Token: 0x0600314B RID: 12619 RVA: 0x00196204 File Offset: 0x00194404
		public bool NetworkInitialized
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600314C RID: 12620 RVA: 0x00196208 File Offset: 0x00194408
		public float ResistanceAgainst(Elements iElement)
		{
			float num = 0f;
			float num2 = 0f;
			for (int i = 0; i < this.mResistances.Length; i++)
			{
				Elements elements = Defines.ElementFromIndex(i);
				if ((iElement & elements) != Elements.None)
				{
					float num3 = this.mResistances[i].Multiplier;
					float num4 = this.mResistances[i].Modifier;
					if (base.HasStatus(StatusEffects.Frozen) && (iElement & Elements.Earth) != Elements.None)
					{
						num4 -= 350f;
					}
					if (base.HasStatus(StatusEffects.Greased) && (iElement & Elements.Fire) != Elements.None)
					{
						num3 *= 2f;
					}
					num += num4;
					num2 += num3;
				}
			}
			float num5 = MathHelper.Clamp(num / 300f + num2, -1f, 1f);
			return 1f - num5;
		}

		// Token: 0x04003552 RID: 13650
		private const float NETWORK_UPDATE_PERIOD = 0.033333335f;

		// Token: 0x04003553 RID: 13651
		private const float MISSILES_COOLDOWN = 0.25f;

		// Token: 0x04003554 RID: 13652
		private const float MISSILE_RADIUS = 0.25f;

		// Token: 0x04003555 RID: 13653
		private const float MISSILE_DAMAGE_AMOUNT = 100f;

		// Token: 0x04003556 RID: 13654
		private const float MISSILE_DAMAGE_MAGNITUDE = 1f;

		// Token: 0x04003557 RID: 13655
		private const float MAXHITPOINTS = 50000f;

		// Token: 0x04003558 RID: 13656
		private float mLastNetworkUpdate;

		// Token: 0x04003559 RID: 13657
		protected float mNetworkUpdateTimer;

		// Token: 0x0400355A RID: 13658
		private static readonly int DIALOG_END = "assaturdeath".GetHashCodeCustom();

		// Token: 0x0400355B RID: 13659
		private static readonly Vector3 DIALOG_OFFSET = new Vector3(-3f, 3f, 0f);

		// Token: 0x0400355C RID: 13660
		private static readonly Vector3 MISSILE_START_VELOCITY = new Vector3(0f, 0f, -30f);

		// Token: 0x0400355D RID: 13661
		private static readonly Random RANDOM = new Random();

		// Token: 0x0400355E RID: 13662
		private static readonly int ASSATUR_DEFEATED_TRIGGER = "assatur_defeated".GetHashCodeCustom();

		// Token: 0x0400355F RID: 13663
		private static readonly int ASSATUR_KILLED_TRIGGER = "assatur_killed".GetHashCodeCustom();

		// Token: 0x04003560 RID: 13664
		private static readonly int LAZER_EFFECT = "assatur_missile_explode".GetHashCodeCustom();

		// Token: 0x04003561 RID: 13665
		private static readonly int BLIZZARD_EFFECT = "assatur_blizzard".GetHashCodeCustom();

		// Token: 0x04003562 RID: 13666
		private static readonly int LIFE_EFFECT = "assatur_life".GetHashCodeCustom();

		// Token: 0x04003563 RID: 13667
		private static readonly int MISSILE_EXPLODE = "assatur_missile_explode".GetHashCodeCustom();

		// Token: 0x04003564 RID: 13668
		private static readonly int MISSILE_TRAIL = "assatur_missile_trail".GetHashCodeCustom();

		// Token: 0x04003565 RID: 13669
		private static readonly int NULLIFY_EFFECT = "assatur_nullify".GetHashCodeCustom();

		// Token: 0x04003566 RID: 13670
		private static readonly int GENERIC_MAGICK = "magick_generic".GetHashCodeCustom();

		// Token: 0x04003567 RID: 13671
		private static readonly int BLOOD_BLACK_EFFECT = "gore_splash_black".GetHashCodeCustom();

		// Token: 0x04003568 RID: 13672
		private static readonly int BARRIER_LOCATOR = "assatur_chasm".GetHashCodeCustom();

		// Token: 0x04003569 RID: 13673
		private static readonly int[] VORTEX_LOCATORS = new int[]
		{
			"assatur_vortex1".GetHashCodeCustom(),
			"assatur_vortex2".GetHashCodeCustom(),
			"assatur_vortex3".GetHashCodeCustom(),
			"assatur_vortex4".GetHashCodeCustom()
		};

		// Token: 0x0400356A RID: 13674
		private bool mDead;

		// Token: 0x0400356B RID: 13675
		private static readonly Random sRandom = new Random();

		// Token: 0x0400356C RID: 13676
		private static readonly Grimnir2.SpellData[] SPELLS = new Grimnir2.SpellData[3];

		// Token: 0x0400356D RID: 13677
		private HitList mHitList;

		// Token: 0x0400356E RID: 13678
		private VisualEffectReference mCurrentEffect;

		// Token: 0x0400356F RID: 13679
		private ConditionCollection mMissileConditions;

		// Token: 0x04003570 RID: 13680
		private List<MissileEntity> mMissiles;

		// Token: 0x04003571 RID: 13681
		private Character mTarget;

		// Token: 0x04003572 RID: 13682
		private float mTargettingPower;

		// Token: 0x04003573 RID: 13683
		private Vector3 mTargetPosition;

		// Token: 0x04003574 RID: 13684
		private float mCastTimeCooldown;

		// Token: 0x04003575 RID: 13685
		private SpellEffect mSpellEffect;

		// Token: 0x04003576 RID: 13686
		private float mSpellPower;

		// Token: 0x04003577 RID: 13687
		private int mTentaclesIndex = 44;

		// Token: 0x04003578 RID: 13688
		private int mTentacleBones = 34;

		// Token: 0x04003579 RID: 13689
		private static float[][] ANIMATION_TIMERS = new float[][]
		{
			new float[0],
			new float[]
			{
				0.3939394f,
				0.8484849f
			},
			new float[]
			{
				0.3939394f,
				0.8484849f
			},
			new float[]
			{
				0.3125f,
				0.625f
			},
			new float[]
			{
				1f
			},
			new float[]
			{
				0.625f
			},
			new float[]
			{
				0.40625f,
				1f
			},
			new float[]
			{
				0.16666667f,
				1f
			},
			new float[]
			{
				0.5f
			},
			new float[]
			{
				0f,
				1f
			},
			new float[]
			{
				0.3488372f
			},
			new float[]
			{
				0.25f,
				0.675f,
				0.45f,
				0.55f
			},
			new float[]
			{
				0.25f
			},
			new float[]
			{
				0.1f,
				0.15f,
				0.9f,
				0.95f
			},
			new float[]
			{
				0.25f
			},
			new float[0],
			new float[0]
		};

		// Token: 0x0400357A RID: 13690
		private BindJoint mLeftHandJoint;

		// Token: 0x0400357B RID: 13691
		private BindJoint mRightHandJoint;

		// Token: 0x0400357C RID: 13692
		private BindJoint mDamageJoint;

		// Token: 0x0400357D RID: 13693
		private BindJoint mStaffJoint;

		// Token: 0x0400357E RID: 13694
		private AnimationController mController;

		// Token: 0x0400357F RID: 13695
		private AnimationController mTentacleController;

		// Token: 0x04003580 RID: 13696
		private AnimationClip[] mClips;

		// Token: 0x04003581 RID: 13697
		private Matrix mMoveLoc0;

		// Token: 0x04003582 RID: 13698
		private Matrix mMoveLoc1;

		// Token: 0x04003583 RID: 13699
		private Matrix mChasmOrientation;

		// Token: 0x04003584 RID: 13700
		private Vector3[] mVortexPositions;

		// Token: 0x04003585 RID: 13701
		private Matrix[] mStartOrientations;

		// Token: 0x04003586 RID: 13702
		private Matrix mOrientation;

		// Token: 0x04003587 RID: 13703
		private Matrix mTargetOrientation;

		// Token: 0x04003588 RID: 13704
		private Matrix mSpineOrientation;

		// Token: 0x04003589 RID: 13705
		private Assatur.AssaturState[] mStates;

		// Token: 0x0400358A RID: 13706
		private Assatur.AssaturState mCurrentState;

		// Token: 0x0400358B RID: 13707
		private Assatur.AssaturState mLastState;

		// Token: 0x0400358C RID: 13708
		private Matrix mRailStaffOrientation;

		// Token: 0x0400358D RID: 13709
		private bool mUseRailStaffOrientation;

		// Token: 0x0400358E RID: 13710
		private Assatur.RenderData[] mRenderData;

		// Token: 0x0400358F RID: 13711
		private Assatur.StaffRenderData[] mStaffRenderData;

		// Token: 0x04003590 RID: 13712
		private BossSpellCasterZone mBodyZone;

		// Token: 0x04003591 RID: 13713
		private PlayState mPlayState;

		// Token: 0x04003592 RID: 13714
		private float mAssaturFloatTimer;

		// Token: 0x04003593 RID: 13715
		private Shield mProtectiveShield;

		// Token: 0x04003594 RID: 13716
		private float mShieldTimer;

		// Token: 0x04003595 RID: 13717
		private float mDamageFlashTimer;

		// Token: 0x04003596 RID: 13718
		private BoundingSphere mBoundingSphere;

		// Token: 0x04003597 RID: 13719
		private Matrix mMagickEffect;

		// Token: 0x04003598 RID: 13720
		private float mDeathScale;

		// Token: 0x04003599 RID: 13721
		private SkinnedModelDeferredAdvancedMaterial mMaterial;

		// Token: 0x0400359A RID: 13722
		private TextureCube mIceCubeMap;

		// Token: 0x0400359B RID: 13723
		private TextureCube mIceCubeNormalMap;

		// Token: 0x0200064E RID: 1614
		public class StaffRenderData : RenderableObject<RenderDeferredEffect, RenderDeferredMaterial>
		{
			// Token: 0x0600314D RID: 12621 RVA: 0x001962C3 File Offset: 0x001944C3
			public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				this.mMaterial.WorldTransform = this.WorldOrientation;
				base.Draw(iEffect, iViewFrustum);
			}

			// Token: 0x0600314E RID: 12622 RVA: 0x001962DE File Offset: 0x001944DE
			public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				this.mMaterial.WorldTransform = this.WorldOrientation;
				base.DrawShadow(iEffect, iViewFrustum);
			}

			// Token: 0x0400359C RID: 13724
			public Matrix WorldOrientation;
		}

		// Token: 0x0200064F RID: 1615
		public class RenderData : RenderableObject<SkinnedModelDeferredEffect, SkinnedModelDeferredAdvancedMaterial>
		{
			// Token: 0x06003150 RID: 12624 RVA: 0x00196301 File Offset: 0x00194501
			public RenderData()
			{
				this.mBones = new Matrix[80];
			}

			// Token: 0x06003151 RID: 12625 RVA: 0x00196318 File Offset: 0x00194518
			public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				skinnedModelDeferredEffect.OverrideColor = new Vector4(1f, 1f, 1f, this.Flash);
				skinnedModelDeferredEffect.Bones = this.mBones;
				this.mMaterial.Damage = this.Damage;
				base.Draw(iEffect, iViewFrustum);
				skinnedModelDeferredEffect.OverrideColor = new Vector4(1f, 1f, 1f, 0f);
			}

			// Token: 0x06003152 RID: 12626 RVA: 0x00196390 File Offset: 0x00194590
			public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				skinnedModelDeferredEffect.Bones = this.mBones;
				base.DrawShadow(iEffect, iViewFrustum);
			}

			// Token: 0x0400359D RID: 13725
			public float Flash;

			// Token: 0x0400359E RID: 13726
			public Matrix[] mBones;

			// Token: 0x0400359F RID: 13727
			public float Damage;
		}

		// Token: 0x02000650 RID: 1616
		public interface AssaturState : IBossState<Assatur>
		{
			// Token: 0x06003153 RID: 12627
			float GetWeight(Assatur iOwner);
		}

		// Token: 0x02000651 RID: 1617
		public class IntroState : Assatur.AssaturState, IBossState<Assatur>
		{
			// Token: 0x06003154 RID: 12628 RVA: 0x001963B8 File Offset: 0x001945B8
			public void OnEnter(Assatur iOwner)
			{
				iOwner.mController.StartClip(iOwner.mClips[15], true);
				iOwner.mTentacleController.StartClip(iOwner.mClips[15], true);
				this.mTimer = 3f;
			}

			// Token: 0x06003155 RID: 12629 RVA: 0x001963EF File Offset: 0x001945EF
			public void OnUpdate(float iDeltaTime, Assatur iOwner)
			{
				if (this.mTimer <= 0f)
				{
					iOwner.ChangeState(Assatur.States.Battle);
				}
				this.mTimer -= iDeltaTime;
			}

			// Token: 0x06003156 RID: 12630 RVA: 0x00196413 File Offset: 0x00194613
			public void OnExit(Assatur iOwner)
			{
			}

			// Token: 0x06003157 RID: 12631 RVA: 0x00196415 File Offset: 0x00194615
			public float GetWeight(Assatur iOwner)
			{
				throw new NotImplementedException();
			}

			// Token: 0x040035A0 RID: 13728
			private float mTimer;
		}

		// Token: 0x02000652 RID: 1618
		public class BattleState : Assatur.AssaturState, IBossState<Assatur>
		{
			// Token: 0x06003159 RID: 12633 RVA: 0x00196424 File Offset: 0x00194624
			public void OnEnter(Assatur iOwner)
			{
				iOwner.mController.CrossFade(iOwner.mClips[15], 0.4f, true);
				iOwner.SelectTarget();
			}

			// Token: 0x0600315A RID: 12634 RVA: 0x00196448 File Offset: 0x00194648
			public void OnUpdate(float iDeltaTime, Assatur iOwner)
			{
				if (NetworkManager.Instance.State != NetworkState.Client)
				{
					EntityManager entityManager = iOwner.mPlayState.EntityManager;
					float num = 0f;
					Assatur.States states = Assatur.States.None;
					for (int i = 0; i < 16; i++)
					{
						float num2 = (float)MagickaMath.Random.NextDouble();
						if (!(iOwner.mLastState == iOwner.mStates[i] | iOwner.mStates[i] is Assatur.DeathState | iOwner.mStates[i] is Assatur.BattleState))
						{
							switch (i)
							{
							case 1:
								num2 += ((iOwner.mProtectiveShield != null && !iOwner.mProtectiveShield.Dead) ? 0.5f : 1f);
								break;
							case 2:
								num2 += (SpellManager.Instance.IsEffectActive(typeof(Vortex)) ? -1f : 0.5f);
								break;
							case 3:
								num2 += (SpellManager.Instance.IsEffectActive(typeof(MeteorShower)) ? -1f : 0.5f);
								break;
							case 4:
								num2 += 0.5f;
								break;
							case 5:
								num2 += Math.Max(0f, (1f - iOwner.HitPoints / iOwner.MaxHitPoints) * 2f);
								break;
							case 6:
								num2 += (SpellManager.Instance.IsEffectActive(typeof(Blizzard)) ? -1f : 0.5f);
								break;
							case 7:
								num2 -= 10f;
								break;
							case 8:
								num2 += ((iOwner.mPlayState.TimeModifier < 1f) ? -1f : 0.25f);
								break;
							case 9:
							{
								List<Entity> entities = entityManager.GetEntities(iOwner.mOrientation.Translation, 10f, true);
								int num3 = entities.Count - 2;
								if (num3 > 0)
								{
									num2 += 1f / (float)num3;
								}
								entityManager.ReturnEntityList(entities);
								break;
							}
							case 10:
								num2 += (1f - iOwner.HitPoints / iOwner.MaxHitPoints) * 0.75f + 0.25f;
								break;
							case 11:
							{
								List<Entity> entities2 = entityManager.GetEntities(iOwner.mOrientation.Translation, 50f, true);
								List<Entity> entities3 = entityManager.GetEntities(iOwner.mOrientation.Translation, 50f, false);
								num2 += 1f - (float)entities2.Count / (float)entities3.Count;
								entityManager.ReturnEntityList(entities2);
								entityManager.ReturnEntityList(entities3);
								break;
							}
							case 12:
							{
								int num4 = entityManager.Shields.Count;
								List<Entity> entities3 = entityManager.GetEntities(iOwner.mOrientation.Translation, 50f, false);
								for (int j = 0; j < entities3.Count; j++)
								{
									Character character = entities3[j] as Character;
									if (character != null && (character.mSelfShield.Active & character.mSelfShield.mShieldType == Character.SelfShieldType.Shield))
									{
										num4++;
									}
								}
								num2 += (float)num4 - 1f;
								num2 -= ((iOwner.mProtectiveShield != null && !iOwner.mProtectiveShield.Dead) ? 1f : 0f);
								entityManager.ReturnEntityList(entities3);
								break;
							}
							}
							if (num2 > num)
							{
								num = num2;
								states = (Assatur.States)i;
							}
						}
					}
					if (iOwner.mStates[(int)states] != null)
					{
						iOwner.ChangeState(states);
					}
				}
			}

			// Token: 0x0600315B RID: 12635 RVA: 0x001967E1 File Offset: 0x001949E1
			public void OnExit(Assatur iOwner)
			{
			}

			// Token: 0x0600315C RID: 12636 RVA: 0x001967E3 File Offset: 0x001949E3
			public float GetWeight(Assatur iOwner)
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x02000653 RID: 1619
		public class CastNullifyState : Assatur.AssaturState, IBossState<Assatur>
		{
			// Token: 0x0600315E RID: 12638 RVA: 0x001967F2 File Offset: 0x001949F2
			public void OnEnter(Assatur iOwner)
			{
				iOwner.mController.CrossFade(iOwner.mClips[14], 0.3f, false);
				this.mCastedMagick = false;
			}

			// Token: 0x0600315F RID: 12639 RVA: 0x00196818 File Offset: 0x00194A18
			public unsafe void OnUpdate(float iDeltaTime, Assatur iOwner)
			{
				if (NetworkManager.Instance.State != NetworkState.Client && !iOwner.mController.CrossFadeEnabled)
				{
					Matrix matrix = iOwner.StaffOrientation();
					EffectManager.Instance.UpdateOrientation(ref iOwner.mCurrentEffect, ref matrix);
					if (iOwner.mController.Time > 0.5f && !this.mCastedMagick)
					{
						EffectManager.Instance.StartEffect(Assatur.NULLIFY_EFFECT, ref matrix, out iOwner.mCurrentEffect);
						iOwner.mPlayState.Camera.CameraShake(0.3f, 1.5f);
						this.mCastedMagick = true;
						Magick magick = default(Magick);
						magick.MagickType = MagickType.Nullify;
						magick.Effect.Execute(iOwner.mBodyZone, iOwner.mPlayState);
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							Assatur.CastMagickMessage castMagickMessage;
							castMagickMessage.Magick = MagickType.Nullify;
							castMagickMessage.Effect = Assatur.NULLIFY_EFFECT;
							castMagickMessage.Position = matrix.Translation;
							castMagickMessage.Direction = matrix.Forward;
							BossFight.Instance.SendMessage<Assatur.CastMagickMessage>(iOwner, 3, (void*)(&castMagickMessage), true);
						}
					}
					if (iOwner.mController.HasFinished)
					{
						iOwner.ChangeState(Assatur.States.Battle);
					}
				}
			}

			// Token: 0x06003160 RID: 12640 RVA: 0x00196944 File Offset: 0x00194B44
			public void OnExit(Assatur iOwner)
			{
				EffectManager.Instance.Stop(ref iOwner.mCurrentEffect);
			}

			// Token: 0x06003161 RID: 12641 RVA: 0x00196956 File Offset: 0x00194B56
			public float GetWeight(Assatur iOwner)
			{
				if (iOwner.mLastState is Assatur.CastNullifyState)
				{
					return 0f;
				}
				return (float)Assatur.RANDOM.NextDouble();
			}

			// Token: 0x040035A1 RID: 13729
			private bool mCastedMagick;
		}

		// Token: 0x02000654 RID: 1620
		public class CastBlizzard : Assatur.AssaturState, IBossState<Assatur>
		{
			// Token: 0x06003163 RID: 12643 RVA: 0x0019697E File Offset: 0x00194B7E
			public void OnEnter(Assatur iOwner)
			{
				iOwner.mController.CrossFade(iOwner.mClips[8], 0.4f, false);
				this.mCastedMagick = false;
				EffectManager.Instance.StartEffect(Assatur.GENERIC_MAGICK, ref iOwner.mMagickEffect, out this.mMagickEffect);
			}

			// Token: 0x06003164 RID: 12644 RVA: 0x001969BC File Offset: 0x00194BBC
			public unsafe void OnUpdate(float iDeltaTime, Assatur iOwner)
			{
				EffectManager.Instance.UpdateOrientation(ref this.mMagickEffect, ref iOwner.mMagickEffect);
				if (NetworkManager.Instance.State != NetworkState.Client && !iOwner.mController.CrossFadeEnabled)
				{
					if (iOwner.mController.Time > 2f & !this.mCastedMagick)
					{
						this.mCastedMagick = true;
						Blizzard.Instance.Execute(iOwner.mBodyZone, iOwner.mPlayState);
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							Assatur.CastMagickMessage castMagickMessage;
							castMagickMessage.Magick = MagickType.Blizzard;
							BossFight.Instance.SendMessage<Assatur.CastMagickMessage>(iOwner, 3, (void*)(&castMagickMessage), true);
						}
					}
					if (iOwner.mController.HasFinished)
					{
						iOwner.ChangeState(Assatur.States.Battle);
					}
				}
			}

			// Token: 0x06003165 RID: 12645 RVA: 0x00196A73 File Offset: 0x00194C73
			public void OnExit(Assatur iOwner)
			{
			}

			// Token: 0x06003166 RID: 12646 RVA: 0x00196A75 File Offset: 0x00194C75
			public float GetWeight(Assatur iOwner)
			{
				if (iOwner.mLastState is Assatur.CastBlizzard)
				{
					return 0f;
				}
				if (!SpellManager.Instance.IsEffectActive(typeof(Blizzard)))
				{
					return (float)Assatur.RANDOM.NextDouble();
				}
				return 0f;
			}

			// Token: 0x040035A2 RID: 13730
			private bool mCastedMagick;

			// Token: 0x040035A3 RID: 13731
			private VisualEffectReference mMagickEffect;
		}

		// Token: 0x02000655 RID: 1621
		public class CastMeteor : Assatur.AssaturState, IBossState<Assatur>
		{
			// Token: 0x06003168 RID: 12648 RVA: 0x00196ABC File Offset: 0x00194CBC
			public unsafe void OnEnter(Assatur iOwner)
			{
				iOwner.mController.CrossFade(iOwner.mClips[5], 0.4f, false);
				EffectManager.Instance.StartEffect(Assatur.GENERIC_MAGICK, ref iOwner.mMagickEffect, out this.mMagickEffect);
				if (NetworkManager.Instance.State != NetworkState.Client)
				{
					if (NetworkManager.Instance.State == NetworkState.Server)
					{
						Assatur.CastMagickMessage castMagickMessage;
						castMagickMessage.Magick = MagickType.MeteorS;
						BossFight.Instance.SendMessage<Assatur.CastMagickMessage>(iOwner, 3, (void*)(&castMagickMessage), true);
					}
					MeteorShower.Instance.Execute(iOwner.mBodyZone, iOwner.mPlayState);
				}
			}

			// Token: 0x06003169 RID: 12649 RVA: 0x00196B48 File Offset: 0x00194D48
			public void OnUpdate(float iDeltaTime, Assatur iOwner)
			{
				EffectManager.Instance.UpdateOrientation(ref this.mMagickEffect, ref iOwner.mMagickEffect);
				if (iOwner.mController.HasFinished && !iOwner.mController.CrossFadeEnabled)
				{
					iOwner.ChangeState(Assatur.States.Battle);
				}
			}

			// Token: 0x0600316A RID: 12650 RVA: 0x00196B82 File Offset: 0x00194D82
			public void OnExit(Assatur iOwner)
			{
			}

			// Token: 0x0600316B RID: 12651 RVA: 0x00196B84 File Offset: 0x00194D84
			public float GetWeight(Assatur iOwner)
			{
				if (iOwner.mLastState is Assatur.CastMeteor)
				{
					return 0f;
				}
				if (!SpellManager.Instance.IsEffectActive(typeof(MeteorShower)))
				{
					return (float)Assatur.RANDOM.NextDouble();
				}
				return 0f;
			}

			// Token: 0x040035A4 RID: 13732
			private VisualEffectReference mMagickEffect;
		}

		// Token: 0x02000656 RID: 1622
		public class CastVortex : Assatur.AssaturState, IBossState<Assatur>
		{
			// Token: 0x0600316D RID: 12653 RVA: 0x00196BC8 File Offset: 0x00194DC8
			public void OnEnter(Assatur iOwner)
			{
				iOwner.mController.CrossFade(iOwner.mClips[4], 0.4f, false);
				EffectManager.Instance.StartEffect(Assatur.GENERIC_MAGICK, ref iOwner.mMagickEffect, out this.mMagickEffect);
				this.mCastedMagick = false;
			}

			// Token: 0x0600316E RID: 12654 RVA: 0x00196C08 File Offset: 0x00194E08
			public unsafe void OnUpdate(float iDeltaTime, Assatur iOwner)
			{
				EffectManager.Instance.UpdateOrientation(ref this.mMagickEffect, ref iOwner.mMagickEffect);
				if (NetworkManager.Instance.State != NetworkState.Client && !iOwner.mController.CrossFadeEnabled)
				{
					if (iOwner.mController.Time > 1f & !this.mCastedMagick)
					{
						Magick magick = default(Magick);
						magick.MagickType = MagickType.Vortex;
						magick.Effect.Execute(iOwner.mBodyZone, iOwner.mPlayState);
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							Assatur.CastMagickMessage castMagickMessage;
							castMagickMessage.Magick = MagickType.Vortex;
							BossFight.Instance.SendMessage<Assatur.CastMagickMessage>(iOwner, 3, (void*)(&castMagickMessage), true);
						}
						this.mCastedMagick = true;
					}
					if (iOwner.mController.HasFinished)
					{
						iOwner.ChangeState(Assatur.States.Battle);
					}
				}
			}

			// Token: 0x0600316F RID: 12655 RVA: 0x00196CD5 File Offset: 0x00194ED5
			public void OnExit(Assatur iOwner)
			{
			}

			// Token: 0x06003170 RID: 12656 RVA: 0x00196CD7 File Offset: 0x00194ED7
			public float GetWeight(Assatur iOwner)
			{
				if (iOwner.mLastState is Assatur.CastVortex)
				{
					return 0f;
				}
				if (!SpellManager.Instance.IsEffectActive(typeof(Vortex)))
				{
					return (float)Assatur.RANDOM.NextDouble();
				}
				return 0f;
			}

			// Token: 0x040035A5 RID: 13733
			private bool mCastedMagick;

			// Token: 0x040035A6 RID: 13734
			private VisualEffectReference mMagickEffect;
		}

		// Token: 0x02000657 RID: 1623
		public class CastTimeWarp : Assatur.AssaturState, IBossState<Assatur>
		{
			// Token: 0x06003172 RID: 12658 RVA: 0x00196D1C File Offset: 0x00194F1C
			public unsafe void OnEnter(Assatur iOwner)
			{
				iOwner.mController.CrossFade(iOwner.mClips[10], 0.4f, false);
				if (NetworkManager.Instance.State != NetworkState.Client)
				{
					if (NetworkManager.Instance.State == NetworkState.Server)
					{
						Assatur.CastMagickMessage castMagickMessage;
						castMagickMessage.Magick = MagickType.TimeWarp;
						BossFight.Instance.SendMessage<Assatur.CastMagickMessage>(iOwner, 3, (void*)(&castMagickMessage), true);
					}
					TimeWarp.Instance.Execute(iOwner.mBodyZone, iOwner.mPlayState);
				}
			}

			// Token: 0x06003173 RID: 12659 RVA: 0x00196D8D File Offset: 0x00194F8D
			public void OnUpdate(float iDeltaTime, Assatur iOwner)
			{
				if (iOwner.mController.HasFinished && !iOwner.mController.CrossFadeEnabled)
				{
					iOwner.ChangeState(Assatur.States.Battle);
				}
			}

			// Token: 0x06003174 RID: 12660 RVA: 0x00196DB0 File Offset: 0x00194FB0
			public void OnExit(Assatur iOwner)
			{
			}

			// Token: 0x06003175 RID: 12661 RVA: 0x00196DB2 File Offset: 0x00194FB2
			public float GetWeight(Assatur iOwner)
			{
				if (iOwner.mLastState is Assatur.CastTimeWarp)
				{
					return 0f;
				}
				if (!SpellManager.Instance.IsEffectActive(typeof(TimeWarp)))
				{
					return (float)Assatur.RANDOM.NextDouble();
				}
				return 0f;
			}
		}

		// Token: 0x02000658 RID: 1624
		public class CastBlade : Assatur.AssaturState, IBossState<Assatur>
		{
			// Token: 0x06003177 RID: 12663 RVA: 0x00196DF8 File Offset: 0x00194FF8
			public void OnEnter(Assatur iOwner)
			{
				iOwner.mController.CrossFade(iOwner.mClips[11], 0.4f, false);
				Spell spell = default(Spell);
				spell.Element = Elements.Arcane;
				spell.ArcaneMagnitude = 5f;
				spell.CalculateDamage(SpellType.Beam, Magicka.GameLogic.Spells.CastType.Weapon, out this.mDamage);
				this.mHitlist.Clear();
			}

			// Token: 0x06003178 RID: 12664 RVA: 0x00196E58 File Offset: 0x00195058
			public void OnUpdate(float iDeltaTime, Assatur iOwner)
			{
				if (!iOwner.mController.CrossFadeEnabled)
				{
					float num = iOwner.mController.Time / iOwner.mController.AnimationClip.Duration;
					if (num >= Assatur.ANIMATION_TIMERS[11][0] && num <= Assatur.ANIMATION_TIMERS[11][1])
					{
						if (this.mBlade == null || this.mBlade.IsDead)
						{
							this.mBlade = ArcaneBlade.GetInstance();
							this.mBlade.Initialize(iOwner.mPlayState, null, Elements.Arcane, 18f);
							this.mCue = AudioManager.Instance.PlayCue(Banks.Spells, "spell_arcane_ray_stage2".GetHashCodeCustom(), iOwner.mBodyZone.AudioEmitter);
						}
						iOwner.mCastTimeCooldown -= iDeltaTime;
						this.mHitlist.Update(iDeltaTime);
						Matrix castSource = iOwner.mBodyZone.CastSource;
						Matrix matrix;
						Matrix.CreateRotationX(-1.5707964f, out matrix);
						Matrix.Multiply(ref matrix, ref castSource, out castSource);
						this.mBlade.Orientation = castSource;
						if (num >= Assatur.ANIMATION_TIMERS[11][2] && num <= Assatur.ANIMATION_TIMERS[11][3])
						{
							Vector3 translation = this.mBlade.Orientation.Translation;
							Vector3 up = this.mBlade.Orientation.Up;
							if (NetworkManager.Instance.State != NetworkState.Client)
							{
								EntityManager entityManager = iOwner.mPlayState.EntityManager;
								List<Entity> entities = entityManager.GetEntities(translation, this.mBlade.Range, true);
								entities.Remove(iOwner.mBodyZone);
								for (int i = 0; i < entities.Count; i++)
								{
									IDamageable damageable = entities[i] as IDamageable;
									Vector3 iAttackPosition;
									if (damageable != null && !this.mHitlist.ContainsKey(damageable.Handle) && damageable.ArcIntersect(out iAttackPosition, translation, up, this.mBlade.Range, 1.4137167f, 2f))
									{
										damageable.Damage(this.mDamage, iOwner.mBodyZone, 0.0, iAttackPosition);
										this.mHitlist.Add(damageable.Handle, 0.333f);
									}
								}
								entityManager.ReturnEntityList(entities);
							}
							if (iOwner.mCastTimeCooldown <= 0f)
							{
								Segment iSeg = new Segment(this.mBlade.Orientation.Translation, this.mBlade.Orientation.Up * this.mBlade.Range);
								float num2;
								Vector3 vector;
								if (iOwner.mPlayState.Level.CurrentScene.SegmentIntersect(out num2, out translation, out vector, iSeg))
								{
									VisualEffectReference visualEffectReference;
									EffectManager.Instance.StartEffect("arcane_detonation".GetHashCodeCustom(), ref translation, ref up, out visualEffectReference);
								}
								iOwner.mCastTimeCooldown += 0.005f;
							}
						}
					}
					else if (num > Assatur.ANIMATION_TIMERS[11][1] && this.mBlade != null && !this.mBlade.IsDead)
					{
						this.mBlade.Kill();
					}
					if (iOwner.mController.HasFinished)
					{
						iOwner.ChangeState(Assatur.States.Battle);
					}
				}
			}

			// Token: 0x06003179 RID: 12665 RVA: 0x0019716B File Offset: 0x0019536B
			public void OnExit(Assatur iOwner)
			{
				if (this.mBlade != null && !this.mBlade.IsDead)
				{
					this.mBlade.Kill();
				}
				this.mCue.Stop(AudioStopOptions.AsAuthored);
			}

			// Token: 0x0600317A RID: 12666 RVA: 0x00197199 File Offset: 0x00195399
			public float GetWeight(Assatur iOwner)
			{
				if (iOwner.mLastState is Assatur.CastBlade)
				{
					return 0f;
				}
				return (float)Assatur.RANDOM.NextDouble();
			}

			// Token: 0x040035A7 RID: 13735
			private ArcaneBlade mBlade;

			// Token: 0x040035A8 RID: 13736
			private DamageCollection5 mDamage;

			// Token: 0x040035A9 RID: 13737
			private HitList mHitlist = new HitList(16);

			// Token: 0x040035AA RID: 13738
			private Cue mCue;
		}

		// Token: 0x02000659 RID: 1625
		public class CastLife : Assatur.AssaturState, IBossState<Assatur>
		{
			// Token: 0x0600317C RID: 12668 RVA: 0x001971CE File Offset: 0x001953CE
			public void OnEnter(Assatur iOwner)
			{
				iOwner.mController.CrossFade(iOwner.mClips[7], 0.4f, false);
				this.mCastEffect = false;
				this.mHealTimer = 0f;
			}

			// Token: 0x0600317D RID: 12669 RVA: 0x001971FC File Offset: 0x001953FC
			public void OnUpdate(float iDeltaTime, Assatur iOwner)
			{
				if (!iOwner.mController.CrossFadeEnabled)
				{
					float num = iOwner.mController.Time / iOwner.mController.AnimationClip.Duration;
					if (num >= Assatur.ANIMATION_TIMERS[7][0] & num <= Assatur.ANIMATION_TIMERS[7][1])
					{
						if (!this.mCastEffect)
						{
							Vector3 translation = iOwner.mSpineOrientation.Translation;
							Vector3 unitZ = Vector3.UnitZ;
							Vector3.Negate(ref unitZ, out unitZ);
							EffectManager.Instance.StartEffect(Assatur.LIFE_EFFECT, ref translation, ref unitZ, out iOwner.mCurrentEffect);
							this.mCastEffect = true;
						}
						if (NetworkManager.Instance.State != NetworkState.Client && this.mHealTimer <= 0f)
						{
							iOwner.Damage(1, -1000f, Elements.Life);
							this.mHealTimer += 0.25f;
						}
					}
					this.mHealTimer -= iDeltaTime;
					if (iOwner.mController.HasFinished)
					{
						iOwner.mController.CrossFade(iOwner.mClips[7], 0.4f, false);
						this.mCastEffect = false;
						this.mHealTimer = 0f;
						iOwner.ChangeState(Assatur.States.Battle);
					}
				}
			}

			// Token: 0x0600317E RID: 12670 RVA: 0x00197322 File Offset: 0x00195522
			public void OnExit(Assatur iOwner)
			{
			}

			// Token: 0x0600317F RID: 12671 RVA: 0x00197324 File Offset: 0x00195524
			public float GetWeight(Assatur iOwner)
			{
				if (iOwner.mLastState is Assatur.CastLife)
				{
					return 0f;
				}
				float num = 1f - iOwner.mHitPoints / iOwner.mMaxHitPoints;
				return Math.Max(num * 2f, 1f);
			}

			// Token: 0x040035AB RID: 13739
			private bool mCastEffect;

			// Token: 0x040035AC RID: 13740
			private float mHealTimer;
		}

		// Token: 0x0200065A RID: 1626
		public class CastMMState : Assatur.AssaturState, IBossState<Assatur>
		{
			// Token: 0x06003181 RID: 12673 RVA: 0x00197373 File Offset: 0x00195573
			public void OnEnter(Assatur iOwner)
			{
				this.mMissileDelay = 0.15f;
				this.mCastDir = Assatur.CastMMState.Castdir.None;
				iOwner.mController.CrossFade(iOwner.mClips[0], 0.5f, false);
			}

			// Token: 0x06003182 RID: 12674 RVA: 0x001973A0 File Offset: 0x001955A0
			public unsafe void OnUpdate(float iDeltaTime, Assatur iOwner)
			{
				if (!iOwner.mController.CrossFadeEnabled && iOwner.mTarget != null)
				{
					if (this.mCastDir == Assatur.CastMMState.Castdir.None)
					{
						if (iOwner.mController.HasFinished)
						{
							Vector3 position = iOwner.mTarget.Position;
							Vector3 position2 = iOwner.mBodyZone.Position;
							Vector3.Subtract(ref position, ref position2, out position);
							position.Y = 0f;
							position.Normalize();
							Vector3 forward = iOwner.mBodyZone.Body.Orientation.Forward;
							forward.Y = 0f;
							forward.Normalize();
							float num = MagickaMath.Angle(ref forward, ref position);
							Vector3 vector;
							Vector3.Cross(ref forward, ref position, out vector);
							if (vector.Y <= 0f)
							{
								num = -num;
							}
							if (num >= 0.3926991f)
							{
								iOwner.mController.CrossFade(iOwner.mClips[1], 0.5f, false);
								this.mCastDir = Assatur.CastMMState.Castdir.Left;
								return;
							}
							if (num <= -0.3926991f)
							{
								iOwner.mController.CrossFade(iOwner.mClips[2], 0.5f, false);
								this.mCastDir = Assatur.CastMMState.Castdir.Right;
								return;
							}
							iOwner.mController.CrossFade(iOwner.mClips[3], 0.5f, false);
							this.mCastDir = Assatur.CastMMState.Castdir.Both;
							return;
						}
					}
					else
					{
						if (iOwner.mController.HasFinished)
						{
							this.mCastDir = Assatur.CastMMState.Castdir.None;
							iOwner.ChangeState(Assatur.States.Battle);
							return;
						}
						if (NetworkManager.Instance.State != NetworkState.Client)
						{
							if (this.mMissileDelay <= 0f)
							{
								float num2 = iOwner.mController.Time / iOwner.mController.AnimationClip.Duration;
								if (num2 >= Assatur.ANIMATION_TIMERS[3][0] && this.mCue == null)
								{
									this.mCue = AudioManager.Instance.PlayCue(Banks.Spells, Assatur.CastMMState.MMSOUND, iOwner.mBodyZone.AudioEmitter);
								}
								switch (this.mCastDir)
								{
								case Assatur.CastMMState.Castdir.Both:
									if (num2 >= Assatur.ANIMATION_TIMERS[3][0] && num2 <= Assatur.ANIMATION_TIMERS[3][1])
									{
										Matrix matrix = iOwner.mBodyZone.CastSource;
										this.SpawnMissile(ref matrix, iOwner, -0.75f);
										if (NetworkManager.Instance.State == NetworkState.Server)
										{
											Assatur.SpawnMMMessage spawnMMMessage;
											spawnMMMessage.CastDir = 1;
											spawnMMMessage.Handle = iOwner.mMissiles[iOwner.mMissiles.Count - 1].Handle;
											BossFight.Instance.SendMessage<Assatur.SpawnMMMessage>(iOwner, 6, (void*)(&spawnMMMessage), true);
										}
										matrix = iOwner.mBodyZone.WeaponSource;
										this.SpawnMissile(ref matrix, iOwner, -0.75f);
										if (NetworkManager.Instance.State == NetworkState.Server)
										{
											Assatur.SpawnMMMessage spawnMMMessage2;
											spawnMMMessage2.CastDir = 1;
											spawnMMMessage2.Handle = iOwner.mMissiles[iOwner.mMissiles.Count - 1].Handle;
											BossFight.Instance.SendMessage<Assatur.SpawnMMMessage>(iOwner, 6, (void*)(&spawnMMMessage2), true);
										}
									}
									break;
								case Assatur.CastMMState.Castdir.Left:
									if (num2 >= Assatur.ANIMATION_TIMERS[1][0] && num2 <= Assatur.ANIMATION_TIMERS[1][1])
									{
										Matrix castSource = iOwner.mBodyZone.CastSource;
										this.SpawnMissile(ref castSource, iOwner, -0.25f);
										if (NetworkManager.Instance.State == NetworkState.Server)
										{
											Assatur.SpawnMMMessage spawnMMMessage3;
											spawnMMMessage3.CastDir = 1;
											spawnMMMessage3.Handle = iOwner.mMissiles[iOwner.mMissiles.Count - 1].Handle;
											BossFight.Instance.SendMessage<Assatur.SpawnMMMessage>(iOwner, 6, (void*)(&spawnMMMessage3), true);
										}
									}
									break;
								case Assatur.CastMMState.Castdir.Right:
									if (num2 >= Assatur.ANIMATION_TIMERS[2][0] && num2 <= Assatur.ANIMATION_TIMERS[2][1])
									{
										Matrix weaponSource = iOwner.mBodyZone.WeaponSource;
										this.SpawnMissile(ref weaponSource, iOwner, -0.25f);
										if (NetworkManager.Instance.State == NetworkState.Server)
										{
											Assatur.SpawnMMMessage spawnMMMessage4;
											spawnMMMessage4.CastDir = 1;
											spawnMMMessage4.Handle = iOwner.mMissiles[iOwner.mMissiles.Count - 1].Handle;
											BossFight.Instance.SendMessage<Assatur.SpawnMMMessage>(iOwner, 6, (void*)(&spawnMMMessage4), true);
										}
									}
									break;
								}
								this.mMissileDelay += 0.15f;
							}
							this.mMissileDelay -= iDeltaTime;
						}
					}
				}
			}

			// Token: 0x06003183 RID: 12675 RVA: 0x001977A9 File Offset: 0x001959A9
			public void OnExit(Assatur iOwner)
			{
				if (this.mCue != null && !this.mCue.IsStopping)
				{
					this.mCue.Stop(AudioStopOptions.AsAuthored);
				}
				this.mCue = null;
				iOwner.mMissiles.Clear();
			}

			// Token: 0x06003184 RID: 12676 RVA: 0x001977E0 File Offset: 0x001959E0
			public void SpawnMissile(ref Matrix iOrientation, Assatur iOwner, float iTolerance)
			{
				AudioManager.Instance.PlayCue(Banks.Characters, "boss_assatur_barrage".GetHashCodeCustom(), iOwner.mBodyZone.AudioEmitter);
				Matrix matrix = iOrientation;
				Vector3 translation = matrix.Translation;
				MissileEntity instance = MissileEntity.GetInstance(iOwner.mPlayState);
				Vector3 forward = matrix.Forward;
				Vector3.Multiply(ref forward, 30f, out forward);
				iOwner.mMissileConditions[0].Clear();
				iOwner.mMissileConditions[0].Add(new EventStorage(new PlayEffectEvent(Assatur.MISSILE_TRAIL, true)));
				instance.Initialize(iOwner.mBodyZone, iOwner.mTarget, 1f, 0.25f, ref translation, ref forward, null, iOwner.mMissileConditions, false);
				instance.HomingTolerance = iTolerance;
				iOwner.mMissiles.Add(instance);
				iOwner.mPlayState.EntityManager.AddEntity(instance);
			}

			// Token: 0x06003185 RID: 12677 RVA: 0x001978BE File Offset: 0x00195ABE
			public float GetWeight(Assatur iOwner)
			{
				if (iOwner.mLastState is Assatur.CastMMState)
				{
					return 0f;
				}
				return (float)Assatur.RANDOM.NextDouble();
			}

			// Token: 0x040035AD RID: 13741
			private Assatur.CastMMState.Castdir mCastDir;

			// Token: 0x040035AE RID: 13742
			private float mMissileDelay = 0.15f;

			// Token: 0x040035AF RID: 13743
			private Cue mCue;

			// Token: 0x040035B0 RID: 13744
			private static readonly int MMSOUND = "magick_vortex".GetHashCodeCustom();

			// Token: 0x0200065B RID: 1627
			public enum Castdir
			{
				// Token: 0x040035B2 RID: 13746
				None,
				// Token: 0x040035B3 RID: 13747
				Both,
				// Token: 0x040035B4 RID: 13748
				Left,
				// Token: 0x040035B5 RID: 13749
				Right
			}
		}

		// Token: 0x0200065C RID: 1628
		public class CastLightning : Assatur.AssaturState, IBossState<Assatur>
		{
			// Token: 0x06003188 RID: 12680 RVA: 0x00197902 File Offset: 0x00195B02
			public CastLightning()
			{
				this.mHitlist = new HitList(16);
			}

			// Token: 0x06003189 RID: 12681 RVA: 0x00197917 File Offset: 0x00195B17
			public void OnEnter(Assatur iOwner)
			{
				iOwner.mController.CrossFade(iOwner.mClips[6], 0.4f, false);
				this.mLightningTimer = 0f;
				this.mHitlist.Clear();
			}

			// Token: 0x0600318A RID: 12682 RVA: 0x00197948 File Offset: 0x00195B48
			public void OnUpdate(float iDeltaTime, Assatur iOwner)
			{
				if (!iOwner.mController.CrossFadeEnabled)
				{
					float num = iOwner.mController.Time / iOwner.mController.AnimationClip.Duration;
					this.mLightningTimer -= iDeltaTime;
					if (iOwner.mController.HasFinished)
					{
						iOwner.ChangeState(Assatur.States.Battle);
						return;
					}
					if (num >= Assatur.ANIMATION_TIMERS[6][0] && (this.mCue == null || this.mCue.IsStopped))
					{
						this.mCue = AudioManager.Instance.PlayCue(Banks.Spells, Assatur.CastLightning.LIGHTNING_SOUND, iOwner.mBodyZone.AudioEmitter);
					}
					if (num >= Assatur.ANIMATION_TIMERS[6][0] && num <= Assatur.ANIMATION_TIMERS[6][1] && this.mLightningTimer <= 0f)
					{
						Spell spell = default(Spell);
						spell.Element |= Elements.Lightning;
						spell.LightningMagnitude = 1f;
						DamageCollection5 damageCollection;
						spell.CalculateDamage(SpellType.Lightning, Magicka.GameLogic.Spells.CastType.Force, out damageCollection);
						float num2 = Assatur.ANIMATION_TIMERS[6][1] - Assatur.ANIMATION_TIMERS[6][0];
						damageCollection.MultiplyMagnitude(num2 * 0.15f * 0.5f);
						this.mHitlist.Clear();
						LightningBolt lightning = LightningBolt.GetLightning();
						Vector3 translation = iOwner.mBodyZone.CastSource.Translation;
						-Vector3.UnitZ;
						lightning.Cast(iOwner.mBodyZone, translation, iOwner.mTarget, this.mHitlist, Spell.LIGHTNINGCOLOR, 1f, 30f, ref damageCollection, iOwner.mPlayState);
						lightning = LightningBolt.GetLightning();
						translation = iOwner.mBodyZone.WeaponSource.Translation;
						-Vector3.UnitZ;
						lightning.Cast(iOwner.mBodyZone, translation, iOwner.mTarget, this.mHitlist, Spell.LIGHTNINGCOLOR, 1f, 30f, ref damageCollection, iOwner.mPlayState);
						this.mLightningTimer += 0.15f;
					}
				}
			}

			// Token: 0x0600318B RID: 12683 RVA: 0x00197B3E File Offset: 0x00195D3E
			public void OnExit(Assatur iOwner)
			{
				if (this.mCue != null && !this.mCue.IsStopping)
				{
					this.mCue.Stop(AudioStopOptions.AsAuthored);
				}
				this.mCue = null;
			}

			// Token: 0x0600318C RID: 12684 RVA: 0x00197B68 File Offset: 0x00195D68
			public float GetWeight(Assatur iOwner)
			{
				if (iOwner.mLastState is Assatur.CastLightning)
				{
					return 0f;
				}
				return (float)Assatur.RANDOM.NextDouble();
			}

			// Token: 0x040035B6 RID: 13750
			private float mLightningTimer;

			// Token: 0x040035B7 RID: 13751
			private HitList mHitlist;

			// Token: 0x040035B8 RID: 13752
			private static readonly int LIGHTNING_SOUND = "spell_lightning_spray".GetHashCodeCustom();

			// Token: 0x040035B9 RID: 13753
			private Cue mCue;
		}

		// Token: 0x0200065D RID: 1629
		public class CastShield : Assatur.AssaturState, IBossState<Assatur>
		{
			// Token: 0x17000B97 RID: 2967
			// (get) Token: 0x0600318E RID: 12686 RVA: 0x00197B99 File Offset: 0x00195D99
			// (set) Token: 0x0600318F RID: 12687 RVA: 0x00197BA1 File Offset: 0x00195DA1
			public bool Shielded
			{
				get
				{
					return this.mShielded;
				}
				set
				{
					this.mShielded = value;
				}
			}

			// Token: 0x06003190 RID: 12688 RVA: 0x00197BAA File Offset: 0x00195DAA
			public void OnEnter(Assatur iOwner)
			{
				iOwner.mController.CrossFade(iOwner.mClips[12], 0.4f, false);
				this.mShielded = false;
			}

			// Token: 0x06003191 RID: 12689 RVA: 0x00197BD0 File Offset: 0x00195DD0
			public unsafe void OnUpdate(float iDeltaTime, Assatur iOwner)
			{
				if (NetworkManager.Instance.State != NetworkState.Client && !iOwner.mController.CrossFadeEnabled)
				{
					float num = iOwner.mController.Time / iOwner.mController.AnimationClip.Duration;
					if (num >= Assatur.ANIMATION_TIMERS[12][0] && !this.mShielded && (iOwner.mProtectiveShield == null || iOwner.mProtectiveShield.Dead))
					{
						iOwner.mProtectiveShield = Shield.GetFromCache(iOwner.mPlayState);
						iOwner.mProtectiveShield.Initialize(iOwner.mBodyZone, iOwner.mOrientation.Translation, 5f, iOwner.mOrientation.Forward, ShieldType.DISC, 5000f, Spell.SHIELDCOLOR);
						iOwner.mProtectiveShield.HitPoints = 5000f;
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							Assatur.CastShieldMessage castShieldMessage;
							castShieldMessage.Direction = iOwner.mBodyZone.Direction;
							castShieldMessage.Position = iOwner.mOrientation.Translation;
							castShieldMessage.Handle = iOwner.mProtectiveShield.Handle;
							castShieldMessage.Radius = iOwner.mProtectiveShield.Radius;
							castShieldMessage.HitPoints = 5000f;
							castShieldMessage.ShieldType = ShieldType.SPHERE;
							BossFight.Instance.SendMessage<Assatur.CastShieldMessage>(iOwner, 5, (void*)(&castShieldMessage), true);
						}
						iOwner.mShieldTimer = 15f;
						this.mShielded = true;
						iOwner.mPlayState.EntityManager.AddEntity(iOwner.mProtectiveShield);
					}
					if (iOwner.mController.HasFinished)
					{
						iOwner.ChangeState(Assatur.States.Battle);
					}
				}
			}

			// Token: 0x06003192 RID: 12690 RVA: 0x00197D5A File Offset: 0x00195F5A
			public void OnExit(Assatur iOwner)
			{
			}

			// Token: 0x06003193 RID: 12691 RVA: 0x00197D5C File Offset: 0x00195F5C
			public float GetWeight(Assatur iOwner)
			{
				if (iOwner.mLastState is Assatur.CastNullifyState)
				{
					return 0f;
				}
				if (iOwner.mProtectiveShield != null || !iOwner.mProtectiveShield.Dead)
				{
					return 0f;
				}
				float num = Math.Min(2f - iOwner.mTimeSinceLastDamage, 0.5f);
				float num2 = iOwner.HitPoints / iOwner.MaxHitPoints;
				num += Math.Min((1f - num2) * 2f, 0.5f);
				return num * (0.75f + (float)Assatur.RANDOM.NextDouble() * 0.25f);
			}

			// Token: 0x040035BA RID: 13754
			private bool mShielded;
		}

		// Token: 0x0200065E RID: 1630
		public class CastStaffBeamState : Assatur.AssaturState, IBossState<Assatur>
		{
			// Token: 0x06003195 RID: 12693 RVA: 0x00197DFC File Offset: 0x00195FFC
			public CastStaffBeamState()
			{
				this.mSpell = default(Spell);
				this.mSpell.Element = Elements.Arcane;
				this.mSpell.ArcaneMagnitude = 5f;
				this.mSpell.CalculateDamage(SpellType.Beam, Magicka.GameLogic.Spells.CastType.Force, out this.mDamage);
			}

			// Token: 0x06003196 RID: 12694 RVA: 0x00197E4B File Offset: 0x0019604B
			public void OnEnter(Assatur iOwner)
			{
				iOwner.mController.CrossFade(iOwner.mClips[13], 0.4f, false);
			}

			// Token: 0x06003197 RID: 12695 RVA: 0x00197E68 File Offset: 0x00196068
			public void OnUpdate(float iDeltaTime, Assatur iOwner)
			{
				if (!iOwner.mController.CrossFadeEnabled)
				{
					this.mTimer -= iDeltaTime;
					float num = iOwner.mController.Time / iOwner.mController.AnimationClip.Duration;
					float num2 = Assatur.ANIMATION_TIMERS[13][0];
					float num3 = Assatur.ANIMATION_TIMERS[13][1];
					float num4 = num3 - num2;
					float num5 = Assatur.ANIMATION_TIMERS[13][2];
					float num6 = Assatur.ANIMATION_TIMERS[13][3];
					float num7 = num6 - num5;
					float num8 = Math.Max(0f, Math.Min(1f, (num - num2) / num4));
					float num9 = Math.Max(0f, Math.Min(1f, (num5 - num) / num7));
					float num10 = num8 * num9;
					iOwner.mTargettingPower = 2f;
					Matrix mBindPose = iOwner.mStaffJoint.mBindPose;
					Matrix.Multiply(ref mBindPose, ref iOwner.mController.SkinnedBoneTransforms[iOwner.mStaffJoint.mIndex], out mBindPose);
					Quaternion quaternion;
					Quaternion.CreateFromRotationMatrix(ref mBindPose, out quaternion);
					Vector3 translation = mBindPose.Translation;
					Vector3 mTargetPosition = iOwner.mTargetPosition;
					Vector3 vector;
					Vector3.Subtract(ref mTargetPosition, ref translation, out vector);
					vector.Normalize();
					Vector3 right = mBindPose.Right;
					Vector3 vector2;
					Vector3.Cross(ref vector, ref right, out vector2);
					Matrix matrix;
					Matrix.CreateWorld(ref translation, ref vector2, ref vector, out matrix);
					Quaternion quaternion2;
					Quaternion.CreateFromRotationMatrix(ref matrix, out quaternion2);
					Quaternion.Lerp(ref quaternion, ref quaternion2, num10, out quaternion2);
					Matrix.CreateFromQuaternion(ref quaternion2, out iOwner.mRailStaffOrientation);
					iOwner.mRailStaffOrientation.Translation = translation;
					iOwner.mUseRailStaffOrientation = true;
					if (this.mRailGun == null || this.mRailGun.IsDead)
					{
						this.mRailGun = Railgun.GetFromCache();
						this.mRailGun.Initialize(iOwner.mBodyZone, translation, vector, Spell.ARCANECOLOR, ref this.mDamage, ref this.mSpell);
					}
					Vector3.Multiply(ref vector, num10, out vector);
					Vector3.Add(ref vector, ref translation, out translation);
					Vector3.Add(ref vector, ref translation, out translation);
					this.mRailGun.Position = translation;
					this.mRailGun.Direction = vector;
					if (iOwner.mController.HasFinished)
					{
						iOwner.ChangeState(Assatur.States.Battle);
						if (this.mRailGun != null && !this.mRailGun.IsDead)
						{
							this.mRailGun.Kill();
						}
					}
				}
			}

			// Token: 0x06003198 RID: 12696 RVA: 0x001980A0 File Offset: 0x001962A0
			public void OnExit(Assatur iOwner)
			{
				if (this.mRailGun != null && !this.mRailGun.IsDead)
				{
					this.mRailGun.Kill();
				}
				if (iOwner.mSpellEffect != null)
				{
					iOwner.mSpellEffect.DeInitialize(iOwner.mBodyZone);
				}
				iOwner.mUseRailStaffOrientation = false;
			}

			// Token: 0x06003199 RID: 12697 RVA: 0x001980ED File Offset: 0x001962ED
			public float GetWeight(Assatur iOwner)
			{
				if (iOwner.mLastState is Assatur.CastStaffBeamState)
				{
					return 0f;
				}
				return (float)Assatur.RANDOM.NextDouble();
			}

			// Token: 0x040035BB RID: 13755
			private Railgun mRailGun;

			// Token: 0x040035BC RID: 13756
			private float mTimer;

			// Token: 0x040035BD RID: 13757
			private DamageCollection5 mDamage;

			// Token: 0x040035BE RID: 13758
			private Spell mSpell;
		}

		// Token: 0x0200065F RID: 1631
		public class CastWater : Assatur.AssaturState, IBossState<Assatur>
		{
			// Token: 0x0600319A RID: 12698 RVA: 0x0019810D File Offset: 0x0019630D
			public void OnEnter(Assatur iOwner)
			{
				iOwner.mController.CrossFade(iOwner.mClips[9], 0.4f, false);
				this.mSpellCasted = false;
			}

			// Token: 0x0600319B RID: 12699 RVA: 0x00198130 File Offset: 0x00196330
			public void OnUpdate(float iDeltaTime, Assatur iOwner)
			{
				if (!iOwner.mController.CrossFadeEnabled)
				{
					float num = iOwner.mController.Time / iOwner.mController.AnimationClip.Duration;
					if (num >= Assatur.ANIMATION_TIMERS[9][0] && !this.mSpellCasted)
					{
						this.mSpellCasted = true;
						Spell spell = default(Spell);
						spell.Element |= Elements.Water;
						spell.WaterMagnitude = 5f;
						spell.Cast(true, iOwner.mBodyZone, Magicka.GameLogic.Spells.CastType.Force);
					}
					else if (num >= Assatur.ANIMATION_TIMERS[9][1] && iOwner.mSpellEffect != null)
					{
						iOwner.mSpellEffect.DeInitialize(iOwner.mBodyZone);
						this.mSpellCasted = false;
					}
					else if (iOwner.mSpellEffect == null)
					{
						this.mSpellCasted = false;
					}
					if (iOwner.mController.HasFinished)
					{
						iOwner.ChangeState(Assatur.States.Battle);
					}
				}
			}

			// Token: 0x0600319C RID: 12700 RVA: 0x0019820C File Offset: 0x0019640C
			public void OnExit(Assatur iOwner)
			{
				if (iOwner.mSpellEffect != null)
				{
					iOwner.mSpellEffect.Stop(iOwner.mBodyZone);
				}
			}

			// Token: 0x0600319D RID: 12701 RVA: 0x00198227 File Offset: 0x00196427
			public float GetWeight(Assatur iOwner)
			{
				if (iOwner.mLastState is Assatur.CastWater)
				{
					return 0f;
				}
				return (float)Assatur.RANDOM.NextDouble();
			}

			// Token: 0x040035BF RID: 13759
			private bool mSpellCasted;
		}

		// Token: 0x02000660 RID: 1632
		public class CastBarrier : Assatur.AssaturState, IBossState<Assatur>
		{
			// Token: 0x0600319F RID: 12703 RVA: 0x00198250 File Offset: 0x00196450
			public CastBarrier()
			{
				this.mSpell.Element = (Elements.Earth | Elements.Fire | Elements.Shield);
				this.mSpell.EarthMagnitude = 1f;
				this.mSpell.ShieldMagnitude = 1f;
				this.mSpell.FireMagnitude = 3f;
				this.mSpell.CalculateDamage(SpellType.Shield, Magicka.GameLogic.Spells.CastType.Weapon, out this.mDamage);
			}

			// Token: 0x060031A0 RID: 12704 RVA: 0x001982B6 File Offset: 0x001964B6
			public void OnEnter(Assatur iOwner)
			{
				iOwner.mController.CrossFade(iOwner.mClips[9], 0.4f, false);
				this.mSpellCasted = false;
			}

			// Token: 0x060031A1 RID: 12705 RVA: 0x001982DC File Offset: 0x001964DC
			public void OnUpdate(float iDeltaTime, Assatur iOwner)
			{
				if (NetworkManager.Instance.State != NetworkState.Client && !iOwner.mController.CrossFadeEnabled)
				{
					float num = iOwner.mController.Time / iOwner.mController.AnimationClip.Duration;
					if (num >= Assatur.ANIMATION_TIMERS[9][0] && !this.mSpellCasted)
					{
						this.mSpellCasted = true;
						Segment iSeg = new Segment(iOwner.mChasmOrientation.Translation, Vector3.Down * 3f);
						float num2;
						Vector3 vector;
						Vector3 vector2;
						iOwner.mPlayState.Level.CurrentScene.SegmentIntersect(out num2, out vector, out vector2, iSeg);
					}
					if (iOwner.mController.HasFinished)
					{
						iOwner.ChangeState(Assatur.States.Battle);
					}
				}
			}

			// Token: 0x060031A2 RID: 12706 RVA: 0x00198394 File Offset: 0x00196594
			public void OnExit(Assatur iOwner)
			{
			}

			// Token: 0x060031A3 RID: 12707 RVA: 0x00198396 File Offset: 0x00196596
			public float GetWeight(Assatur iOwner)
			{
				if (iOwner.mLastState is Assatur.CastBarrier)
				{
					return 0f;
				}
				return (float)Assatur.RANDOM.NextDouble();
			}

			// Token: 0x040035C0 RID: 13760
			private bool mSpellCasted;

			// Token: 0x040035C1 RID: 13761
			public Spell mSpell;

			// Token: 0x040035C2 RID: 13762
			public DamageCollection5 mDamage;
		}

		// Token: 0x02000661 RID: 1633
		public class DeathState : Assatur.AssaturState, IBossState<Assatur>
		{
			// Token: 0x060031A4 RID: 12708 RVA: 0x001983B8 File Offset: 0x001965B8
			public void OnEnter(Assatur iOwner)
			{
				iOwner.mPlayState.Level.CurrentScene.ExecuteTrigger(Assatur.ASSATUR_DEFEATED_TRIGGER, null, false);
				if (iOwner.mSpellEffect != null)
				{
					iOwner.mSpellEffect.DeInitialize(iOwner.mBodyZone);
				}
				iOwner.mSpellEffect = null;
				this.mStartPosition = iOwner.mOrientation.Translation.Y;
				this.mTriggerExecuted = false;
				iOwner.mController.CrossFade(iOwner.mClips[17], 0.2f, false);
				this.mPreTimer = iOwner.mClips[17].Duration * 0.2f;
				Matrix orientation = iOwner.mBodyZone.Body.Orientation;
				orientation.Translation = iOwner.mBodyZone.Position;
				EffectManager.Instance.StartEffect("assatur_death".GetHashCodeCustom(), ref orientation, out this.mEffect);
			}

			// Token: 0x060031A5 RID: 12709 RVA: 0x00198494 File Offset: 0x00196694
			public void OnUpdate(float iDeltaTime, Assatur iOwner)
			{
				float num = iOwner.mController.Time / iOwner.mController.AnimationClip.Duration;
				Vector3 translation = iOwner.mOrientation.Translation;
				translation.Y = MathHelper.Lerp(this.mStartPosition, this.mStartPosition + 4f, num);
				iOwner.mTargetOrientation.Translation = translation;
				iOwner.mOrientation.Translation = translation;
				Matrix mOrientation = iOwner.mOrientation;
				mOrientation.Translation = new Vector3(mOrientation.Translation.X, mOrientation.Translation.Y - num * 4f, mOrientation.Translation.Z);
				EffectManager.Instance.UpdateOrientation(ref this.mEffect, ref mOrientation);
				if (iOwner.mController.HasFinished && !iOwner.mController.CrossFadeEnabled && !this.mTriggerExecuted)
				{
					this.mTriggerExecuted = true;
					iOwner.mPlayState.Level.CurrentScene.ExecuteTrigger(Assatur.ASSATUR_KILLED_TRIGGER, null, false);
				}
				iOwner.mDeathScale = 1.001f - iOwner.mController.Time / iOwner.mController.AnimationClip.Duration;
				this.mPreTimer -= iDeltaTime;
			}

			// Token: 0x060031A6 RID: 12710 RVA: 0x001985CE File Offset: 0x001967CE
			public void OnExit(Assatur iOwner)
			{
			}

			// Token: 0x060031A7 RID: 12711 RVA: 0x001985D0 File Offset: 0x001967D0
			public float GetWeight(Assatur iOwner)
			{
				throw new NotImplementedException();
			}

			// Token: 0x040035C3 RID: 13763
			private bool mTriggerExecuted;

			// Token: 0x040035C4 RID: 13764
			private float mPreTimer;

			// Token: 0x040035C5 RID: 13765
			private float mStartPosition;

			// Token: 0x040035C6 RID: 13766
			private VisualEffectReference mEffect;
		}

		// Token: 0x02000662 RID: 1634
		public enum MessageType : ushort
		{
			// Token: 0x040035C8 RID: 13768
			Update,
			// Token: 0x040035C9 RID: 13769
			ChangeState,
			// Token: 0x040035CA RID: 13770
			ChangeTarget,
			// Token: 0x040035CB RID: 13771
			CastMagick,
			// Token: 0x040035CC RID: 13772
			CastBarrier,
			// Token: 0x040035CD RID: 13773
			CastShield,
			// Token: 0x040035CE RID: 13774
			SpawnMM
		}

		// Token: 0x02000663 RID: 1635
		internal struct UpdateMessage
		{
			// Token: 0x040035CF RID: 13775
			public const ushort TYPE = 0;

			// Token: 0x040035D0 RID: 13776
			public byte Animation;

			// Token: 0x040035D1 RID: 13777
			public float AnimationTime;

			// Token: 0x040035D2 RID: 13778
			public float Hitpoints;
		}

		// Token: 0x02000664 RID: 1636
		internal struct SpawnMMMessage
		{
			// Token: 0x040035D3 RID: 13779
			public const ushort TYPE = 6;

			// Token: 0x040035D4 RID: 13780
			public ushort Handle;

			// Token: 0x040035D5 RID: 13781
			public int CastDir;
		}

		// Token: 0x02000665 RID: 1637
		internal struct CastMagickMessage
		{
			// Token: 0x040035D6 RID: 13782
			public const ushort TYPE = 3;

			// Token: 0x040035D7 RID: 13783
			public MagickType Magick;

			// Token: 0x040035D8 RID: 13784
			public Vector3 Position;

			// Token: 0x040035D9 RID: 13785
			public Vector3 Direction;

			// Token: 0x040035DA RID: 13786
			public int Effect;
		}

		// Token: 0x02000666 RID: 1638
		internal struct CastBarrierMessage
		{
			// Token: 0x040035DB RID: 13787
			public const ushort TYPE = 4;

			// Token: 0x040035DC RID: 13788
			public Vector3 Position;

			// Token: 0x040035DD RID: 13789
			public Vector3 Direction;

			// Token: 0x040035DE RID: 13790
			public ushort Handle;
		}

		// Token: 0x02000667 RID: 1639
		internal struct CastShieldMessage
		{
			// Token: 0x040035DF RID: 13791
			public const ushort TYPE = 5;

			// Token: 0x040035E0 RID: 13792
			public Vector3 Position;

			// Token: 0x040035E1 RID: 13793
			public Vector3 Direction;

			// Token: 0x040035E2 RID: 13794
			public ShieldType ShieldType;

			// Token: 0x040035E3 RID: 13795
			public float Radius;

			// Token: 0x040035E4 RID: 13796
			public float HitPoints;

			// Token: 0x040035E5 RID: 13797
			public ushort Handle;
		}

		// Token: 0x02000668 RID: 1640
		internal struct ChangeStateMessage
		{
			// Token: 0x040035E6 RID: 13798
			public const ushort TYPE = 1;

			// Token: 0x040035E7 RID: 13799
			public Assatur.States NewState;
		}

		// Token: 0x02000669 RID: 1641
		internal struct ChangeTargetMessage
		{
			// Token: 0x040035E8 RID: 13800
			public const ushort TYPE = 2;

			// Token: 0x040035E9 RID: 13801
			public ushort Target;
		}

		// Token: 0x0200066A RID: 1642
		public enum Animations
		{
			// Token: 0x040035EB RID: 13803
			MMPrepare,
			// Token: 0x040035EC RID: 13804
			MMLeft,
			// Token: 0x040035ED RID: 13805
			MMRight,
			// Token: 0x040035EE RID: 13806
			MMBoth,
			// Token: 0x040035EF RID: 13807
			Vortex,
			// Token: 0x040035F0 RID: 13808
			MeteorShower,
			// Token: 0x040035F1 RID: 13809
			Lightning,
			// Token: 0x040035F2 RID: 13810
			Life,
			// Token: 0x040035F3 RID: 13811
			Blizzard,
			// Token: 0x040035F4 RID: 13812
			Water,
			// Token: 0x040035F5 RID: 13813
			Eruption,
			// Token: 0x040035F6 RID: 13814
			ArcaneBlade,
			// Token: 0x040035F7 RID: 13815
			Shield,
			// Token: 0x040035F8 RID: 13816
			RailBeam,
			// Token: 0x040035F9 RID: 13817
			Nullify,
			// Token: 0x040035FA RID: 13818
			Idle,
			// Token: 0x040035FB RID: 13819
			Tentacles,
			// Token: 0x040035FC RID: 13820
			Defeat,
			// Token: 0x040035FD RID: 13821
			NrOfAnimations
		}

		// Token: 0x0200066B RID: 1643
		public enum States
		{
			// Token: 0x040035FF RID: 13823
			Battle,
			// Token: 0x04003600 RID: 13824
			MagickMissiles,
			// Token: 0x04003601 RID: 13825
			Vortex,
			// Token: 0x04003602 RID: 13826
			MeteorShower,
			// Token: 0x04003603 RID: 13827
			Lightning,
			// Token: 0x04003604 RID: 13828
			Life,
			// Token: 0x04003605 RID: 13829
			Blizzard,
			// Token: 0x04003606 RID: 13830
			Water,
			// Token: 0x04003607 RID: 13831
			TimeWarp,
			// Token: 0x04003608 RID: 13832
			ArcaneBlade,
			// Token: 0x04003609 RID: 13833
			Shield,
			// Token: 0x0400360A RID: 13834
			StaffBeam,
			// Token: 0x0400360B RID: 13835
			Nullify,
			// Token: 0x0400360C RID: 13836
			Barrier,
			// Token: 0x0400360D RID: 13837
			Intro,
			// Token: 0x0400360E RID: 13838
			Dead,
			// Token: 0x0400360F RID: 13839
			None,
			// Token: 0x04003610 RID: 13840
			NrOfStates
		}
	}
}
