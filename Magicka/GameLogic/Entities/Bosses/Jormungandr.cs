using System;
using System.Collections.Generic;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using JigLibX.Physics;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
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
	// Token: 0x020004AD RID: 1197
	public class Jormungandr : BossStatusEffected, IBoss
	{
		// Token: 0x06002420 RID: 9248 RVA: 0x00103124 File Offset: 0x00101324
		public Jormungandr(PlayState iPlayState)
		{
			this.mPlayState = iPlayState;
			this.mGlobalState = Jormungandr.GlobalState.Instance;
			this.mCurrentState = Jormungandr.IntroState.Instance;
			lock (Game.Instance.GraphicsDevice)
			{
				this.mModel = iPlayState.Content.Load<SkinnedModel>("Models/Bosses/Jormungandr/Jormungandr");
				this.mIceCubeMap = this.mPlayState.Content.Load<TextureCube>("EffectTextures/iceCube");
				this.mIceCubeNormalMap = this.mPlayState.Content.Load<TextureCube>("EffectTextures/iceCube_NRM");
			}
			this.mAnimationController = new AnimationController();
			this.mAnimationController.Skeleton = this.mModel.SkeletonBones;
			this.mAnimationClips = new AnimationClip[13];
			this.mAnimationClips[1] = this.mModel.AnimationClips["intro"];
			this.mAnimationClips[2] = this.mModel.AnimationClips["dive_attack"];
			this.mAnimationClips[3] = this.mModel.AnimationClips["dive_recoil"];
			this.mAnimationClips[4] = this.mModel.AnimationClips["emerge"];
			this.mAnimationClips[5] = this.mModel.AnimationClips["submerge"];
			this.mAnimationClips[6] = this.mModel.AnimationClips["idle"];
			this.mAnimationClips[7] = this.mModel.AnimationClips["spit"];
			this.mAnimationClips[8] = this.mModel.AnimationClips["venom"];
			this.mAnimationClips[9] = this.mModel.AnimationClips["attack"];
			this.mAnimationClips[10] = this.mModel.AnimationClips["hit"];
			this.mAnimationClips[11] = this.mModel.AnimationClips["die"];
			this.mAnimationClips[12] = this.mModel.AnimationClips["die_above"];
			this.mSpineIndices = new int[26];
			this.mSpineBindPose = new Matrix[26];
			Matrix matrix;
			Matrix.CreateRotationY(3.1415927f, out matrix);
			for (int i = 0; i < this.mModel.SkeletonBones.Count; i++)
			{
				SkinnedModelBone skinnedModelBone = this.mModel.SkeletonBones[i];
				if (skinnedModelBone.Name.Equals("HeadBase", StringComparison.OrdinalIgnoreCase))
				{
					this.mHeadJointIndex = (int)skinnedModelBone.Index;
					Matrix inverseBindPoseTransform = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref inverseBindPoseTransform, ref matrix, out inverseBindPoseTransform);
					Matrix.Invert(ref inverseBindPoseTransform, out this.mHeadBindPose);
				}
				for (int j = 11; j > 0; j--)
				{
					string value = string.Format("back{0:00}", j);
					if (skinnedModelBone.Name.Equals(value, StringComparison.OrdinalIgnoreCase))
					{
						this.mSpineIndices[11 - j] = (int)skinnedModelBone.Index;
						Matrix inverseBindPoseTransform = skinnedModelBone.InverseBindPoseTransform;
						Matrix.Multiply(ref inverseBindPoseTransform, ref matrix, out inverseBindPoseTransform);
						Matrix.Invert(ref inverseBindPoseTransform, out this.mSpineBindPose[11 - j]);
					}
				}
				if (skinnedModelBone.Name.Equals("center", StringComparison.OrdinalIgnoreCase))
				{
					this.mSpineIndices[11] = (int)skinnedModelBone.Index;
					Matrix inverseBindPoseTransform = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref inverseBindPoseTransform, ref matrix, out inverseBindPoseTransform);
					Matrix.Invert(ref inverseBindPoseTransform, out this.mSpineBindPose[11]);
				}
				for (int k = 1; k <= 12; k++)
				{
					string value2 = string.Format("front{0:00}", k);
					if (skinnedModelBone.Name.Equals(value2, StringComparison.OrdinalIgnoreCase))
					{
						this.mSpineIndices[k + 11] = (int)skinnedModelBone.Index;
						Matrix inverseBindPoseTransform = skinnedModelBone.InverseBindPoseTransform;
						Matrix.Multiply(ref inverseBindPoseTransform, ref matrix, out inverseBindPoseTransform);
						Matrix.Invert(ref inverseBindPoseTransform, out this.mSpineBindPose[k + 11]);
					}
				}
				if (skinnedModelBone.Name.Equals("NeckBase", StringComparison.OrdinalIgnoreCase))
				{
					this.mSpineIndices[24] = (int)skinnedModelBone.Index;
					Matrix inverseBindPoseTransform = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref inverseBindPoseTransform, ref matrix, out inverseBindPoseTransform);
					Matrix.Invert(ref inverseBindPoseTransform, out this.mSpineBindPose[24]);
				}
				if (skinnedModelBone.Name.Equals("NeckMid", StringComparison.OrdinalIgnoreCase))
				{
					this.mSpineIndices[25] = (int)skinnedModelBone.Index;
					Matrix inverseBindPoseTransform = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref inverseBindPoseTransform, ref matrix, out inverseBindPoseTransform);
					Matrix.Invert(ref inverseBindPoseTransform, out this.mSpineBindPose[25]);
				}
				if (skinnedModelBone.Name.Equals("NeckEnd", StringComparison.OrdinalIgnoreCase))
				{
					this.mNeckJointIndex = (int)skinnedModelBone.Index;
					Matrix inverseBindPoseTransform = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref inverseBindPoseTransform, ref matrix, out inverseBindPoseTransform);
					Matrix.Invert(ref inverseBindPoseTransform, out this.mNeckBindPose);
				}
			}
			Primitive[] array = new Primitive[25];
			for (int l = 0; l < array.Length; l++)
			{
				float radius = Math.Min(0.333f + (float)l * 0.2f, 1f);
				float length = Vector3.Distance(this.mSpineBindPose[l].Translation, this.mSpineBindPose[l + 1].Translation);
				array[l] = new Capsule(default(Vector3), Matrix.Identity, radius, length);
			}
			this.mSpine = new JormungandrCollisionZone(iPlayState, this, array);
			this.mSpine.Body.CollisionSkin.callbackFn += this.OnSpineCollision;
			this.mDamageZone = new BossDamageZone(iPlayState, this, 1, 1.1f);
			this.mDamageZone.Body.CollisionSkin.callbackFn += this.OnCollision;
			Helper.SkinnedModelDeferredMaterialFromBasicEffect(this.mModel.Model.Meshes[0].MeshParts[0].Effect as SkinnedModelBasicEffect, out this.mMaterial);
			ModelMesh modelMesh = this.mModel.Model.Meshes[0];
			this.mRenderData = new Jormungandr.RenderData[3];
			for (int m = 0; m < 3; m++)
			{
				Jormungandr.RenderData renderData = new Jormungandr.RenderData();
				this.mRenderData[m] = renderData;
				renderData.SetMesh(this.mModel.Model.Meshes[0].VertexBuffer, this.mModel.Model.Meshes[0].IndexBuffer, this.mModel.Model.Meshes[0].MeshParts[0], 0, 3, 4);
				renderData.mMaterial = this.mMaterial;
			}
			this.mLeapDamage = default(DamageCollection5);
			this.mLeapDamage.A.Amount = 150f;
			this.mLeapDamage.A.Magnitude = 1f;
			this.mLeapDamage.A.AttackProperty = AttackProperties.Damage;
			this.mLeapDamage.A.Element = Elements.Earth;
			this.mLeapDamage.B.Amount = 30f;
			this.mLeapDamage.B.Magnitude = 1f;
			this.mLeapDamage.B.AttackProperty = AttackProperties.Status;
			this.mLeapDamage.C.Element = Elements.Poison;
			this.mLeapDamage.C.Amount = 70f;
			this.mLeapDamage.C.Magnitude = 2f;
			this.mLeapDamage.C.AttackProperty = AttackProperties.Pushed;
			this.mLeapDamage.C.Element = Elements.Earth;
			this.mEmergeDamage = default(DamageCollection5);
			this.mEmergeDamage.A.Amount = 150f;
			this.mEmergeDamage.A.Magnitude = 2f;
			this.mEmergeDamage.A.AttackProperty = AttackProperties.Knockback;
			this.mEmergeDamage.A.Element = Elements.Earth;
			this.mEmergeDamage.B.Amount = 300f;
			this.mEmergeDamage.B.Magnitude = 2f;
			this.mEmergeDamage.B.AttackProperty = AttackProperties.Damage;
			this.mEmergeDamage.B.Element = Elements.Earth;
			this.mBiteDamage = default(DamageCollection5);
			this.mBiteDamage.A.Amount = 325f;
			this.mBiteDamage.A.Magnitude = 1f;
			this.mBiteDamage.A.AttackProperty = AttackProperties.Damage;
			this.mBiteDamage.A.Element = Elements.Earth;
			this.mBiteDamage.B.Amount = 30f;
			this.mBiteDamage.B.Magnitude = 2f;
			this.mBiteDamage.B.AttackProperty = AttackProperties.Status;
			this.mBiteDamage.B.Element = Elements.Poison;
			this.mBiteDamage.C.Amount = 70f;
			this.mBiteDamage.C.Magnitude = 2f;
			this.mBiteDamage.C.AttackProperty = AttackProperties.Knockback;
			this.mBiteDamage.C.Element = Elements.Earth;
			this.mSpitDamage = default(DamageCollection5);
			this.mSpitDamage.A.Amount = 75f;
			this.mSpitDamage.A.Magnitude = 4f;
			this.mSpitDamage.A.AttackProperty = AttackProperties.Status;
			this.mSpitDamage.A.Element = Elements.Poison;
			this.mSpitDamage.B.Amount = 70f;
			this.mSpitDamage.B.Magnitude = 1f;
			this.mSpitDamage.B.AttackProperty = AttackProperties.Pushed;
			this.mSpitDamage.B.Element = Elements.Earth;
			this.mSpitConditions = new ConditionCollection();
			this.mSpitConditions[0].Condition.EventConditionType = EventConditionType.Default;
			this.mSpitConditions[0].Condition.Repeat = true;
			this.mSpitConditions[0].Add(new EventStorage(new PlayEffectEvent(Jormungandr.JORMUNGANDREFFECTS[5], true)));
			this.mSpitConditions[0].Add(new EventStorage(new PlaySoundEvent(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[6])));
			this.mSpitConditions[1].Condition.EventConditionType = EventConditionType.Hit;
			this.mSpitConditions[1].Add(new EventStorage(new PlayEffectEvent(Jormungandr.JORMUNGANDREFFECTS[4])));
			this.mSpitConditions[1].Add(new EventStorage(new PlaySoundEvent(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[6])));
			this.mSpitConditions[1].Add(new EventStorage(new DamageEvent(this.mSpitDamage.A)));
			this.mSpitConditions[1].Add(new EventStorage(default(RemoveEvent)));
			this.mSpitConditions[2].Condition.EventConditionType = EventConditionType.Collision;
			this.mSpitConditions[2].Add(new EventStorage(new PlaySoundEvent(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[6])));
			this.mSpitConditions[2].Add(new EventStorage(new PlayEffectEvent(Jormungandr.JORMUNGANDREFFECTS[4])));
			this.mSpitConditions[2].Add(new EventStorage(new SplashEvent(this.mSpitDamage.A, 2f)));
			this.mSpitConditions[2].Add(new EventStorage(new SplashEvent(this.mSpitDamage.B, 2f)));
			this.mSpitConditions[2].Add(new EventStorage(default(RemoveEvent)));
			this.mDirtSpawned = false;
			this.mNoEarthCollisionEffect = false;
			this.mResistances = new Resistance[11];
			for (int n = 0; n < 11; n++)
			{
				Elements elements = Defines.ElementFromIndex(n);
				this.mResistances[n].ResistanceAgainst = elements;
				if (elements == Elements.Ice | elements == Elements.Earth)
				{
					this.mResistances[n].Multiplier = 0.75f;
					this.mResistances[n].Modifier = 0f;
				}
				else
				{
					this.mResistances[n].Multiplier = 1f;
					this.mResistances[n].Modifier = 0f;
				}
			}
			this.mAudioEmitter = new AudioEmitter();
		}

		// Token: 0x06002421 RID: 9249 RVA: 0x00103DD4 File Offset: 0x00101FD4
		public void Initialize(ref Matrix iOrientation, int iUniqueID)
		{
			this.Initialize(ref iOrientation);
		}

		// Token: 0x06002422 RID: 9250 RVA: 0x00103DE0 File Offset: 0x00101FE0
		public void Initialize(ref Matrix iOrientation)
		{
			this.mMaxHitPoints = 10000f;
			this.mHitPoints = 10000f;
			this.mCurrentState = Jormungandr.SleepIntroState.Instance;
			this.mCurrentState.OnEnter(this);
			this.mOrientation = iOrientation;
			this.mPosition = iOrientation.Translation;
			this.mDirection = iOrientation.Forward;
			this.mDamageZone.Initialize();
			this.mDamageZone.Body.CollisionSkin.NonCollidables.Add(this.mSpine.Body.CollisionSkin);
			this.mPlayState.EntityManager.AddEntity(this.mDamageZone);
			this.mSpine.Initialize();
			this.mSpine.Body.CollisionSkin.NonCollidables.Add(this.mDamageZone.Body.CollisionSkin);
			this.mSpine.Body.CollisionSkin.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
			this.mPlayState.EntityManager.AddEntity(this.mSpine);
			for (int i = 0; i < this.mStatusEffects.Length; i++)
			{
				this.mStatusEffects[i].Stop();
				this.mStatusEffects[i] = default(StatusEffect);
			}
			this.mInitialized = true;
			this.mDead = false;
			this.mDamageFlashTimer = 0f;
			this.mDeathAnimation = Jormungandr.Animations.Die_Above;
			EffectManager.Instance.Stop(ref this.mEarthTremorReference);
			EffectManager.Instance.Stop(ref this.mEarthSplashReference);
		}

		// Token: 0x06002423 RID: 9251 RVA: 0x00103F7C File Offset: 0x0010217C
		public bool OnSpineCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
		{
			if (iSkin1.Owner == null)
			{
				return false;
			}
			if (this.Dead)
			{
				return true;
			}
			IDamageable damageable = iSkin1.Owner.Tag as IDamageable;
			if (damageable == null || damageable is MissileEntity || this.mSpineHitlist.ContainsKey(damageable.Handle))
			{
				return true;
			}
			this.mSpineHitlist.Add(damageable.Handle, 0.25f);
			Segment seg = default(Segment);
			seg.Origin = iSkin0.Owner.Position;
			seg.Delta = iSkin1.Owner.Position;
			Vector3.Subtract(ref seg.Delta, ref seg.Origin, out seg.Delta);
			float num;
			Vector3 iAttackPosition;
			Vector3 vector;
			iSkin1.SegmentIntersect(out num, out iAttackPosition, out vector, seg);
			if (damageable is Character)
			{
				Damage iDamage = new Damage(AttackProperties.Knockback, Elements.Earth, 60f, 1f);
				damageable.Damage(iDamage, this.mDamageZone, 0.0, iAttackPosition);
			}
			else if (damageable is Shield || damageable is Barrier)
			{
				Damage iDamage2 = new Damage(AttackProperties.Knockback, Elements.Earth, 750f, 1f);
				damageable.Damage(iDamage2, this.mDamageZone, 0.0, iAttackPosition);
			}
			return true;
		}

		// Token: 0x06002424 RID: 9252 RVA: 0x001040AC File Offset: 0x001022AC
		public bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
		{
			if (iSkin1 == this.mPlayState.Level.CurrentScene.CollisionSkin && !this.mNoEarthCollisionEffect && !this.mDirtSpawned)
			{
				if (this.mCurrentState is Jormungandr.BiteState)
				{
					Vector3 right = Vector3.Right;
					Vector3 vector;
					if (this.FrontBodyCollision(out vector) && !EffectManager.Instance.IsActive(ref this.mEarthSplashReference))
					{
						EffectManager.Instance.StartEffect(Jormungandr.JORMUNGANDREFFECTS[0], ref vector, ref right, out this.mEarthSplashReference);
					}
					return false;
				}
				if (this.mAnimationController.AnimationClip == this.mAnimationClips[2])
				{
					if (this.mMidLeap)
					{
						AudioManager.Instance.PlayCue(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[11]);
					}
					else
					{
						AudioManager.Instance.PlayCue(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[4]);
					}
				}
				EffectManager.Instance.Stop(ref this.mEarthTremorReference);
				this.StartEffect(out this.mEarthSplashReference, Jormungandr.JORMUNGANDREFFECTS[2]);
				this.mDirtSpawned = true;
				this.mPlayState.Camera.CameraShake(1f, 0.7f);
			}
			if (iSkin1.Owner == null || this.mDamageState == Jormungandr.DamageState.None)
			{
				return false;
			}
			IDamageable damageable = iSkin1.Owner.Tag as IDamageable;
			if (damageable == null)
			{
				return false;
			}
			if (this.mHitlist.ContainsKey(damageable.Handle))
			{
				return true;
			}
			this.mHitlist.Add(damageable.Handle, 0.25f);
			DamageCollection5 iDamage;
			switch (this.mDamageState)
			{
			case Jormungandr.DamageState.Leap:
				iDamage = this.mLeapDamage;
				break;
			case Jormungandr.DamageState.Emerge:
				iDamage = this.mEmergeDamage;
				break;
			case Jormungandr.DamageState.Bite:
				iDamage = this.mBiteDamage;
				break;
			default:
				return false;
			}
			Segment seg = default(Segment);
			seg.Origin = iSkin0.Owner.Position;
			seg.Delta = iSkin1.Owner.Position;
			Vector3.Subtract(ref seg.Delta, ref seg.Origin, out seg.Delta);
			float num;
			Vector3 iAttackPosition;
			Vector3 vector2;
			iSkin1.SegmentIntersect(out num, out iAttackPosition, out vector2, seg);
			if (!(damageable is Shield) && !(damageable is Barrier))
			{
				damageable.Damage(iDamage, this.mDamageZone, 0.0, iAttackPosition);
				if (damageable.Dead)
				{
					damageable.OverKill();
				}
				return false;
			}
			if (damageable is Barrier && !(damageable as Barrier).Solid)
			{
				return false;
			}
			AudioManager.Instance.PlayCue(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[12]);
			if (this.mAnimationController.AnimationClip != this.mAnimationClips[5] || this.mAnimationController.AnimationClip != this.mAnimationClips[4])
			{
				int num2 = (int)(damageable.MaxHitPoints * 0.75f);
				Damage iDamage2 = new Damage(AttackProperties.Damage, Elements.Earth, (float)num2, 1f);
				damageable.Damage(iDamage2, this.mDamageZone, 0.0, iAttackPosition);
				this.mIsHit = true;
				this.mHitPoints -= 750f;
			}
			else
			{
				damageable.Kill();
			}
			return false;
		}

		// Token: 0x06002425 RID: 9253 RVA: 0x00104398 File Offset: 0x00102598
		public void UpdateBoss(DataChannel iDataChannel, float iDeltaTime, bool iFightStarted)
		{
			if (!this.mInitialized)
			{
				return;
			}
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				this.mNetworkUpdateTimer -= iDeltaTime;
				if (this.mNetworkUpdateTimer <= 0f)
				{
					this.mNetworkUpdateTimer = 0.033333335f;
					this.NetworkUpdate();
				}
			}
			if (this.mCurrentState is Jormungandr.SleepIntroState && iFightStarted)
			{
				this.ChangeState(Jormungandr.IntroState.Instance);
			}
			Jormungandr.WARNINGTIME = MathHelper.Lerp(Jormungandr.MIN_WARNINGTIME, Jormungandr.MAX_WARNINGTIME, this.mHitPoints / 10000f);
			this.mGlobalState.OnUpdate(iDeltaTime, this);
			this.mCurrentState.OnUpdate(iDeltaTime, this);
			Matrix orientation = this.GetOrientation();
			this.mAudioEmitter.Position = orientation.Translation;
			this.mAudioEmitter.Forward = orientation.Forward;
			this.mAudioEmitter.Up = orientation.Up;
			this.mAnimationController.Speed = 1f;
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
				this.mAnimationController.Speed = 0f;
			}
			else
			{
				if (base.HasStatus(StatusEffects.Cold))
				{
					this.mAnimationController.Speed *= 0.5f;
				}
				this.mRenderData[(int)iDataChannel].mMaterial.Bloat = 0f;
				this.mRenderData[(int)iDataChannel].mMaterial.EmissiveAmount = this.mMaterial.EmissiveAmount;
				this.mRenderData[(int)iDataChannel].mMaterial.SpecularBias = this.mMaterial.SpecularBias;
				this.mRenderData[(int)iDataChannel].mMaterial.SpecularPower = this.mMaterial.SpecularPower;
				this.mRenderData[(int)iDataChannel].mMaterial.CubeMapEnabled = false;
				this.mRenderData[(int)iDataChannel].mMaterial.CubeNormalMapEnabled = false;
			}
			if (this.Dead)
			{
				this.mIdleTimer += iDeltaTime;
				if (this.mIdleTimer > 1f)
				{
					this.mSpineHitlist.Clear();
					this.mHitlist.Clear();
				}
			}
			else
			{
				this.mSpineHitlist.Update(iDeltaTime);
				this.mHitlist.Update(iDeltaTime);
				Jormungandr.WARNINGTIME = 1f * (this.mHitPoints / 10000f) + 0.4f;
			}
			Vector3 position;
			if (this.mUpdateAnimation)
			{
				this.mAnimationController.Update(iDeltaTime, ref this.mOrientation, true);
				this.mSpine.Body.Position = default(Vector3);
				Transform transform = default(Transform);
				transform.Position = this.mSpineBindPose[0].Translation;
				Vector3.Transform(ref transform.Position, ref this.mAnimationController.SkinnedBoneTransforms[this.mSpineIndices[0]], out transform.Position);
				for (int i = 0; i < 25; i++)
				{
					Vector3 translation = this.mSpineBindPose[i + 1].Translation;
					Vector3.Transform(ref translation, ref this.mAnimationController.SkinnedBoneTransforms[this.mSpineIndices[i + 1]], out translation);
					Vector3 vector;
					Vector3.Subtract(ref transform.Position, ref translation, out vector);
					vector.Normalize();
					Jormungandr.CreateVertebraeOrientation(ref vector, out transform.Orientation);
					transform.Position = translation;
					this.mSpine.Body.CollisionSkin.GetPrimitiveLocal(i).SetTransform(ref transform);
				}
				Transform identity = Transform.Identity;
				this.mSpine.Body.CollisionSkin.SetTransform(ref identity, ref identity);
				this.mSpine.Body.CollisionSkin.UpdateWorldBoundingBox();
				this.mSpine.Body.Transform = identity;
				position = this.mHeadBindPose.Translation;
				Vector3.Transform(ref position, ref this.mAnimationController.SkinnedBoneTransforms[this.mHeadJointIndex], out position);
				Vector3 forward = this.mOrientation.Forward;
				Vector3.Multiply(ref forward, 0.6f, out forward);
				Vector3.Add(ref position, ref forward, out position);
			}
			else
			{
				position = new Vector3(0f, -100f, 0f);
				this.mSpine.Body.Position = position;
			}
			this.mDamageZone.SetPosition(ref position);
			if (this.mCurrentState is Jormungandr.UndergroundState | this.mCurrentState is Jormungandr.SleepIntroState)
			{
				EffectManager.Instance.Stop(ref this.mBackTremor);
				EffectManager.Instance.Stop(ref this.mFrontTremor);
			}
			else
			{
				Vector3 right = Vector3.Right;
				Vector3 vector2;
				if (this.BackBodyCollision(out vector2))
				{
					if (!EffectManager.Instance.UpdatePositionDirection(ref this.mBackTremor, ref vector2, ref right))
					{
						EffectManager.Instance.StartEffect(Jormungandr.JORMUNGANDREFFECTS[1], ref vector2, ref right, out this.mBackTremor);
					}
				}
				else if (EffectManager.Instance.IsActive(ref this.mBackTremor))
				{
					EffectManager.Instance.Stop(ref this.mBackTremor);
				}
				if ((this.FrontBodyCollision(out vector2) && !(this.mCurrentState is Jormungandr.IntroState)) || (this.mCurrentState is Jormungandr.IntroState && !this.mAnimationController.CrossFadeEnabled && this.mAnimationController.Time > 2f))
				{
					if (!EffectManager.Instance.UpdatePositionDirection(ref this.mFrontTremor, ref vector2, ref right))
					{
						EffectManager.Instance.StartEffect(Jormungandr.JORMUNGANDREFFECTS[1], ref vector2, ref right, out this.mFrontTremor);
					}
				}
				else if (EffectManager.Instance.IsActive(ref this.mFrontTremor))
				{
					EffectManager.Instance.Stop(ref this.mFrontTremor);
				}
			}
			this.UpdateDamage(iDeltaTime);
			this.UpdateStatusEffects(iDeltaTime);
			this.mDamageFlashTimer = Math.Max(this.mDamageFlashTimer - iDeltaTime, 0f);
			if (this.mDraw)
			{
				Jormungandr.RenderData renderData = this.mRenderData[(int)iDataChannel];
				this.mAnimationController.SkinnedBoneTransforms.CopyTo(renderData.mSkeleton, 0);
				renderData.Damage = 1f - this.mHitPoints / 10000f;
				renderData.mBoundingSphere.Center = this.mPosition;
				renderData.mBoundingSphere.Radius = 20f;
				renderData.Flash = this.mDamageFlashTimer * 10f;
				this.mPlayState.Scene.AddRenderableObject(iDataChannel, renderData);
			}
		}

		// Token: 0x06002426 RID: 9254 RVA: 0x00104AC8 File Offset: 0x00102CC8
		protected bool FindGround(ref Vector3 iPosition, out Vector3 iDirection)
		{
			Segment iSeg = default(Segment);
			iSeg.Origin = iPosition;
			iSeg.Origin.Y = iSeg.Origin.Y + 10f;
			iSeg.Delta.Y = -30f;
			float num;
			return this.mPlayState.Level.CurrentScene.SegmentIntersect(out num, out iPosition, out iDirection, iSeg);
		}

		// Token: 0x06002427 RID: 9255 RVA: 0x00104B30 File Offset: 0x00102D30
		protected void StartEffect(out VisualEffectReference iRef, int iHash)
		{
			Vector3 vector = this.mDamageZone.Body.Position;
			if (vector.Y < -10f)
			{
				vector = this.mOrientation.Translation;
			}
			Vector3 right;
			if (this.FindGround(ref vector, out right))
			{
				right = Vector3.Right;
				EffectManager.Instance.StartEffect(iHash, ref vector, ref right, out iRef);
				return;
			}
			iRef = default(VisualEffectReference);
		}

		// Token: 0x06002428 RID: 9256 RVA: 0x00104B94 File Offset: 0x00102D94
		protected void Turn(ref Vector3 iNewDirection, float iTurnSpeed, float iDeltaTime)
		{
			if (base.HasStatus(StatusEffects.Frozen))
			{
				return;
			}
			if (base.HasStatus(StatusEffects.Cold))
			{
				iTurnSpeed *= 0.5f;
			}
			Matrix identity = Matrix.Identity;
			Vector3 up = Vector3.Up;
			Vector3 right;
			Vector3.Cross(ref iNewDirection, ref up, out right);
			identity.Forward = iNewDirection;
			identity.Up = up;
			identity.Right = right;
			Quaternion quaternion;
			Quaternion.CreateFromRotationMatrix(ref this.mOrientation, out quaternion);
			Quaternion quaternion2;
			Quaternion.CreateFromRotationMatrix(ref identity, out quaternion2);
			Quaternion.Lerp(ref quaternion, ref quaternion2, MathHelper.Clamp(iDeltaTime * iTurnSpeed, 0f, 1f), out quaternion2);
			Vector3 translation = this.mOrientation.Translation;
			Matrix.CreateFromQuaternion(ref quaternion2, out this.mOrientation);
			this.mOrientation.Translation = translation;
			this.mDirection = this.mOrientation.Forward;
		}

		// Token: 0x06002429 RID: 9257 RVA: 0x00104C5C File Offset: 0x00102E5C
		protected unsafe void SelectTarget(Jormungandr.TargettingType iFlags)
		{
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				return;
			}
			Player[] players = Game.Instance.Players;
			Vector3 vector = this.mPosition;
			switch (iFlags)
			{
			default:
			{
				int num = Jormungandr.mRandom.Next(4);
				for (int i = 0; i < players.Length; i++)
				{
					Player player = players[(i + num) % 4];
					if (player.Avatar != null && !player.Avatar.Dead)
					{
						this.mTarget = player.Avatar;
						break;
					}
				}
				break;
			}
			case Jormungandr.TargettingType.Distance:
			{
				float num2 = float.MaxValue;
				foreach (Player player2 in players)
				{
					if (player2.Avatar != null && !player2.Avatar.Dead)
					{
						Vector3 position = player2.Avatar.Position;
						float num3 = 0f;
						Vector3.DistanceSquared(ref vector, ref position, out num3);
						if (num3 < num2)
						{
							this.mTarget = player2.Avatar;
							num2 = num3;
						}
					}
				}
				break;
			}
			case Jormungandr.TargettingType.Angle:
			{
				float num4 = float.MaxValue;
				foreach (Player player3 in players)
				{
					if (player3.Avatar != null && !player3.Avatar.Dead)
					{
						Vector3 forward = this.mOrientation.Forward;
						Vector3 position2 = player3.Avatar.Position;
						Vector3 vector2;
						Vector3.Subtract(ref position2, ref this.mPosition, out vector2);
						vector2.Normalize();
						float num5 = MagickaMath.Angle(ref forward, ref vector2);
						if (num5 < num4)
						{
							this.mTarget = player3.Avatar;
							num4 = num5;
						}
					}
				}
				break;
			}
			case Jormungandr.TargettingType.PlayerStrain:
			{
				int num6 = 0;
				foreach (Player player4 in players)
				{
					if (player4.Avatar != null && !player4.Avatar.Dead)
					{
						int count = player4.Avatar.SpellQueue.Count;
						if (count > num6)
						{
							this.mTarget = player4.Avatar;
							num6 = count;
						}
					}
				}
				break;
			}
			}
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				Jormungandr.ChangeTargetMessage changeTargetMessage;
				if (this.mTarget != null)
				{
					changeTargetMessage.Target = this.mTarget.Handle;
				}
				else
				{
					changeTargetMessage.Target = ushort.MaxValue;
				}
				BossFight.Instance.SendMessage<Jormungandr.ChangeTargetMessage>(this, 2, (void*)(&changeTargetMessage), true);
			}
		}

		// Token: 0x0600242A RID: 9258 RVA: 0x00104E92 File Offset: 0x00103092
		protected void ChangeState(IBossState<Jormungandr> iState)
		{
			if (this.mCurrentState == iState)
			{
				return;
			}
			this.mLastState = this.mCurrentState;
			this.mCurrentState.OnExit(this);
			this.mCurrentState = iState;
			this.mCurrentState.OnEnter(this);
		}

		// Token: 0x0600242B RID: 9259 RVA: 0x00104ECC File Offset: 0x001030CC
		protected static void CreateTransform(ref Vector3 iTranslation, ref Vector3 iDirection, out Matrix oOrientation)
		{
			Vector3 up = default(Vector3);
			up.Y = 1f;
			Vector3 right;
			Vector3.Cross(ref iDirection, ref up, out right);
			right.Normalize();
			Vector3.Cross(ref right, ref iDirection, out up);
			oOrientation = default(Matrix);
			oOrientation.Forward = iDirection;
			oOrientation.Up = up;
			oOrientation.Right = right;
			oOrientation.Translation = iTranslation;
			oOrientation.M44 = 1f;
		}

		// Token: 0x0600242C RID: 9260 RVA: 0x00104F40 File Offset: 0x00103140
		protected static void CreateVertebraeOrientation(ref Vector3 iForward, out Matrix oOrientation)
		{
			Vector3 up = default(Vector3);
			if (iForward.X < 1E-06f && iForward.Z < 1E-06f)
			{
				up.X = 1f;
			}
			else
			{
				up.Y = 1f;
			}
			Vector3 right;
			Vector3.Cross(ref iForward, ref up, out right);
			right.Normalize();
			Vector3.Cross(ref right, ref iForward, out up);
			oOrientation = default(Matrix);
			oOrientation.Forward = iForward;
			oOrientation.Up = up;
			oOrientation.Right = right;
			oOrientation.M44 = 1f;
		}

		// Token: 0x0600242D RID: 9261 RVA: 0x00104FD0 File Offset: 0x001031D0
		protected void TransformBones(int iStartIndex, ref Matrix iTransform)
		{
			Matrix.Multiply(ref this.mAnimationController.SkinnedBoneTransforms[iStartIndex], ref iTransform, out this.mAnimationController.SkinnedBoneTransforms[iStartIndex]);
			SkinnedModelBoneCollection children = this.mModel.SkeletonBones[iStartIndex].Children;
			for (int i = 0; i < children.Count; i++)
			{
				this.TransformBones((int)children[i].Index, ref iTransform);
			}
		}

		// Token: 0x0600242E RID: 9262 RVA: 0x00105040 File Offset: 0x00103240
		private Matrix GetOrientation()
		{
			Matrix orientation = this.mDamageZone.Body.Orientation;
			orientation.Translation = this.mDamageZone.Body.Position;
			return orientation;
		}

		// Token: 0x0600242F RID: 9263 RVA: 0x00105076 File Offset: 0x00103276
		public void DeInitialize()
		{
		}

		// Token: 0x06002430 RID: 9264 RVA: 0x00105078 File Offset: 0x00103278
		public DamageResult Damage(int iPartIndex, Damage iDamage, Entity iAttacker, ref Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			if ((iDamage.Amount == 0f && iDamage.Magnitude == 0f) || (!this.mDraw | this.Dead | iPartIndex != 1) || this.mAnimationController.AnimationClip == this.mAnimationClips[5])
			{
				return DamageResult.None;
			}
			float mHitPoints = this.mHitPoints;
			DamageResult damageResult = this.Damage(iDamage, iAttacker, iAttackPosition, iFeatures);
			if (((damageResult & DamageResult.Hit) == DamageResult.Hit | (damageResult & DamageResult.Damaged) == DamageResult.Damaged | (damageResult & DamageResult.Killed) == DamageResult.Killed) & (damageResult & DamageResult.Healed) == DamageResult.None)
			{
				this.mDamageFlashTimer = 0.1f;
				if (mHitPoints - this.mHitPoints >= 250f)
				{
					this.mIsHit = true;
				}
			}
			return damageResult;
		}

		// Token: 0x06002431 RID: 9265 RVA: 0x00105136 File Offset: 0x00103336
		public void Damage(int iPartIndex, float iDamage, Elements iElement)
		{
			base.Damage(iDamage, iElement);
		}

		// Token: 0x06002432 RID: 9266 RVA: 0x00105140 File Offset: 0x00103340
		public void SetSlow(int iIndex)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06002433 RID: 9267 RVA: 0x00105147 File Offset: 0x00103347
		public bool AddImpulseVelocity(Vector3 iDirection, float iElevation, float iMassPower, float iDistance)
		{
			return false;
		}

		// Token: 0x17000884 RID: 2180
		// (get) Token: 0x06002434 RID: 9268 RVA: 0x0010514A File Offset: 0x0010334A
		public float MaxHitPoints
		{
			get
			{
				return 10000f;
			}
		}

		// Token: 0x17000885 RID: 2181
		// (get) Token: 0x06002435 RID: 9269 RVA: 0x00105151 File Offset: 0x00103351
		public float HitPoints
		{
			get
			{
				return this.mHitPoints;
			}
		}

		// Token: 0x06002436 RID: 9270 RVA: 0x0010515C File Offset: 0x0010335C
		private bool BackBodyCollision(out Vector3 oPosition)
		{
			Segment iSeg = default(Segment);
			oPosition = default(Vector3);
			iSeg.Origin = (this.mSpine.Body.CollisionSkin.GetPrimitiveLocal(11) as Capsule).Position;
			for (int i = 0; i < 11; i++)
			{
				Vector3 position = (this.mSpine.Body.CollisionSkin.GetPrimitiveLocal(10 - i) as Capsule).Position;
				Vector3.Subtract(ref position, ref iSeg.Origin, out iSeg.Delta);
				float num;
				Vector3 vector;
				if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out num, out oPosition, out vector, iSeg))
				{
					return true;
				}
				iSeg.Origin = position;
			}
			return false;
		}

		// Token: 0x06002437 RID: 9271 RVA: 0x00105210 File Offset: 0x00103410
		private bool FrontBodyCollision(out Vector3 oPosition)
		{
			Segment iSeg = default(Segment);
			oPosition = default(Vector3);
			iSeg.Origin = (this.mSpine.Body.CollisionSkin.GetPrimitiveLocal(11) as Capsule).Position;
			for (int i = 0; i < 11; i++)
			{
				Vector3 position = (this.mSpine.Body.CollisionSkin.GetPrimitiveLocal(12 + i) as Capsule).Position;
				Vector3.Subtract(ref position, ref iSeg.Origin, out iSeg.Delta);
				float num;
				Vector3 vector;
				if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out num, out oPosition, out vector, iSeg))
				{
					return true;
				}
				iSeg.Origin = position;
			}
			return false;
		}

		// Token: 0x06002438 RID: 9272 RVA: 0x001052C2 File Offset: 0x001034C2
		public bool HasStatus(int iIndex, StatusEffects iStatus)
		{
			return (this.mCurrentStatusEffects & iStatus) == iStatus;
		}

		// Token: 0x06002439 RID: 9273 RVA: 0x001052D0 File Offset: 0x001034D0
		public float StatusMagnitude(int iIndex, StatusEffects iStatus)
		{
			int num = StatusEffect.StatusIndex(iStatus);
			if (!this.mStatusEffects[num].Dead)
			{
				return this.mStatusEffects[num].Magnitude;
			}
			return 0f;
		}

		// Token: 0x0600243A RID: 9274 RVA: 0x0010530E File Offset: 0x0010350E
		public void ScriptMessage(BossMessages iMessage)
		{
		}

		// Token: 0x0600243B RID: 9275 RVA: 0x00105310 File Offset: 0x00103510
		private unsafe void NetworkUpdate()
		{
			NetworkServer networkServer = NetworkManager.Instance.Interface as NetworkServer;
			if (networkServer == null)
			{
				return;
			}
			Jormungandr.UpdateMessage updateMessage = default(Jormungandr.UpdateMessage);
			updateMessage.Animation = 0;
			while ((int)updateMessage.Animation < this.mAnimationClips.Length && this.mAnimationController.AnimationClip != this.mAnimationClips[(int)updateMessage.Animation])
			{
				updateMessage.Animation += 1;
			}
			updateMessage.AnimationTime = this.mAnimationController.Time;
			updateMessage.Hitpoints = this.mHitPoints;
			updateMessage.Position = this.mPosition;
			updateMessage.Direction = this.mDirection;
			updateMessage.Orientation = this.mOrientation;
			for (int i = 0; i < networkServer.Connections; i++)
			{
				float num = networkServer.GetLatency(i) * 0.5f;
				Jormungandr.UpdateMessage updateMessage2 = updateMessage;
				updateMessage2.AnimationTime += num;
				BossFight.Instance.SendMessage<Jormungandr.UpdateMessage>(this, 0, (void*)(&updateMessage), false, i);
			}
		}

		// Token: 0x0600243C RID: 9276 RVA: 0x00105408 File Offset: 0x00103608
		public unsafe void NetworkUpdate(ref BossUpdateMessage iMsg)
		{
			if (iMsg.Type == 0)
			{
				if (iMsg.TimeStamp < this.mLastNetworkUpdate)
				{
					return;
				}
				this.mLastNetworkUpdate = iMsg.TimeStamp;
				Jormungandr.UpdateMessage updateMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&updateMessage));
				if (this.mAnimationController.AnimationClip != this.mAnimationClips[(int)updateMessage.Animation])
				{
					this.mAnimationController.StartClip(this.mAnimationClips[(int)updateMessage.Animation], false);
				}
				this.mAnimationController.Time = updateMessage.AnimationTime;
				this.mHitPoints = updateMessage.Hitpoints;
				this.mPosition = updateMessage.Position;
				this.mDirection = updateMessage.Direction;
				this.mOrientation = updateMessage.Orientation;
				return;
			}
			else
			{
				if (iMsg.Type == 3)
				{
					Jormungandr.SpitMessage spitMessage;
					BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&spitMessage));
					MissileEntity missileEntity = Magicka.GameLogic.Entities.Entity.GetFromHandle((int)spitMessage.Handle) as MissileEntity;
					missileEntity.Initialize(this.mDamageZone, 0.25f, ref spitMessage.Position, ref spitMessage.Velocity, null, this.mSpitConditions, false);
					this.mPlayState.EntityManager.AddEntity(missileEntity);
					return;
				}
				if (iMsg.Type != 2)
				{
					if (iMsg.Type == 1)
					{
						Jormungandr.ChangeStateMessage changeStateMessage;
						BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&changeStateMessage));
						if (changeStateMessage.Animation != Jormungandr.Animations.Invalid)
						{
							if (changeStateMessage.AnimationBlendTime > 1E-45f)
							{
								this.mAnimationController.CrossFade(this.mAnimationClips[(int)changeStateMessage.Animation], changeStateMessage.AnimationBlendTime, changeStateMessage.AnimationLooped);
							}
							else
							{
								this.mAnimationController.StartClip(this.mAnimationClips[(int)changeStateMessage.Animation], changeStateMessage.AnimationLooped);
							}
						}
						switch (changeStateMessage.NewState)
						{
						case Jormungandr.State.Sleep:
							this.ChangeState(Jormungandr.SleepIntroState.Instance);
							return;
						case Jormungandr.State.Intro:
							this.ChangeState(Jormungandr.IntroState.Instance);
							return;
						case Jormungandr.State.Underground:
							this.ChangeState(Jormungandr.UndergroundState.Instance);
							return;
						case Jormungandr.State.Leap:
							this.ChangeState(Jormungandr.LeapState.Instance);
							return;
						case Jormungandr.State.Risen:
							this.ChangeState(Jormungandr.RisenState.Instance);
							return;
						case Jormungandr.State.Spit:
							this.ChangeState(Jormungandr.SpitState.Instance);
							return;
						case Jormungandr.State.Bite:
							this.ChangeState(Jormungandr.BiteState.Instance);
							return;
						case Jormungandr.State.HitRisen:
							this.ChangeState(Jormungandr.HitRisenState.Instance);
							return;
						case Jormungandr.State.HitLeap:
							this.ChangeState(Jormungandr.HitLeapState.Instance);
							return;
						case Jormungandr.State.Dead:
							if (this.mCurrentState is Jormungandr.UndergroundState)
							{
								this.mDeathAnimation = Jormungandr.Animations.Die_Below;
							}
							else
							{
								this.mDeathAnimation = Jormungandr.Animations.Die_Above;
							}
							this.ChangeState(Jormungandr.DeadState.Instance);
							break;
						default:
							return;
						}
					}
					return;
				}
				Jormungandr.ChangeTargetMessage changeTargetMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&changeTargetMessage));
				if (changeTargetMessage.Target == 65535)
				{
					this.mTarget = null;
					return;
				}
				this.mTarget = (Magicka.GameLogic.Entities.Entity.GetFromHandle((int)changeTargetMessage.Target) as Character);
				return;
			}
		}

		// Token: 0x0600243D RID: 9277 RVA: 0x001056A1 File Offset: 0x001038A1
		public void NetworkInitialize(ref BossInitializeMessage iMsg)
		{
			throw new NotImplementedException();
		}

		// Token: 0x17000886 RID: 2182
		// (get) Token: 0x0600243E RID: 9278 RVA: 0x001056A8 File Offset: 0x001038A8
		protected override BossDamageZone Entity
		{
			get
			{
				return this.mDamageZone;
			}
		}

		// Token: 0x17000887 RID: 2183
		// (get) Token: 0x0600243F RID: 9279 RVA: 0x001056B0 File Offset: 0x001038B0
		protected override float Radius
		{
			get
			{
				return 1.5f;
			}
		}

		// Token: 0x17000888 RID: 2184
		// (get) Token: 0x06002440 RID: 9280 RVA: 0x001056B7 File Offset: 0x001038B7
		protected override float Length
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x17000889 RID: 2185
		// (get) Token: 0x06002441 RID: 9281 RVA: 0x001056BE File Offset: 0x001038BE
		protected override int BloodEffect
		{
			get
			{
				return Jormungandr.BLOOD_EFFECT;
			}
		}

		// Token: 0x1700088A RID: 2186
		// (get) Token: 0x06002442 RID: 9282 RVA: 0x001056C8 File Offset: 0x001038C8
		protected override Vector3 NotifierTextPostion
		{
			get
			{
				Vector3 position = this.mDamageZone.Position;
				position.Y += 1.5f;
				return position;
			}
		}

		// Token: 0x1700088B RID: 2187
		// (get) Token: 0x06002443 RID: 9283 RVA: 0x001056F5 File Offset: 0x001038F5
		public override bool Dead
		{
			get
			{
				return this.mDead;
			}
		}

		// Token: 0x06002444 RID: 9284 RVA: 0x001056FD File Offset: 0x001038FD
		public BossEnum GetBossType()
		{
			return BossEnum.Jormungandr;
		}

		// Token: 0x1700088C RID: 2188
		// (get) Token: 0x06002445 RID: 9285 RVA: 0x00105700 File Offset: 0x00103900
		public bool NetworkInitialized
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06002446 RID: 9286 RVA: 0x00105704 File Offset: 0x00103904
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

		// Token: 0x0400271C RID: 10012
		private const float NETWORK_UPDATE_PERIOD = 0.033333335f;

		// Token: 0x0400271D RID: 10013
		private const float MAXHITPOINTS = 10000f;

		// Token: 0x0400271E RID: 10014
		private const float COLLISIONDAMAGE = 750f;

		// Token: 0x0400271F RID: 10015
		private const float HITTOLERANCE = 250f;

		// Token: 0x04002720 RID: 10016
		private static float MAX_WARNINGTIME = 2.5f;

		// Token: 0x04002721 RID: 10017
		private static float MIN_WARNINGTIME = 0.75f;

		// Token: 0x04002722 RID: 10018
		private static float WARNINGTIME;

		// Token: 0x04002723 RID: 10019
		private static readonly int[] JORMUNGANDREFFECTS = new int[]
		{
			"jormungandr_dirt_splash".GetHashCodeCustom(),
			"jormungandr_dirt_tremor".GetHashCodeCustom(),
			"jormungandr_dirt_splash_tremor".GetHashCodeCustom(),
			"jormungandr_spit_spray".GetHashCodeCustom(),
			"jormungandr_spit_splash".GetHashCodeCustom(),
			"jormungandr_spit_projectile".GetHashCodeCustom()
		};

		// Token: 0x04002724 RID: 10020
		private static readonly int BLOOD_EFFECT = Gib.GORE_SPLASH_EFFECTS[0];

		// Token: 0x04002725 RID: 10021
		private static readonly float TAIL_WHIP = 0.88709676f;

		// Token: 0x04002726 RID: 10022
		private static readonly int[] JORMUNGANDRSOUNDS = new int[]
		{
			"boss_jormungandr_death".GetHashCodeCustom(),
			"boss_jormungandr_wakeup".GetHashCodeCustom(),
			"boss_jormungandr_dive".GetHashCodeCustom(),
			"boss_jormungandr_unburrow".GetHashCodeCustom(),
			"boss_jormungandr_leap".GetHashCodeCustom(),
			"boss_jormungandr_rise".GetHashCodeCustom(),
			"boss_jormungandr_spit".GetHashCodeCustom(),
			"boss_jormungandr_bite".GetHashCodeCustom(),
			"boss_jormungandr_pain".GetHashCodeCustom(),
			"boss_jormungandr_prespit".GetHashCodeCustom(),
			"boss_jormungandr_prebite".GetHashCodeCustom(),
			"boss_jormungandr_leapdive".GetHashCodeCustom(),
			"boss_jormungandr_hitobject".GetHashCodeCustom(),
			"boss_jormungandr_whip".GetHashCodeCustom(),
			"boss_jormungandr_prowl".GetHashCodeCustom()
		};

		// Token: 0x04002727 RID: 10023
		protected long mLastNetworkUpdate;

		// Token: 0x04002728 RID: 10024
		protected float mNetworkUpdateTimer;

		// Token: 0x04002729 RID: 10025
		protected VisualEffectReference mEarthSplashReference;

		// Token: 0x0400272A RID: 10026
		protected VisualEffectReference mEarthTremorReference;

		// Token: 0x0400272B RID: 10027
		protected Cue mProwlCue;

		// Token: 0x0400272C RID: 10028
		protected static Random mRandom = new Random();

		// Token: 0x0400272D RID: 10029
		protected AnimationClip[] mAnimationClips;

		// Token: 0x0400272E RID: 10030
		protected AnimationController mAnimationController;

		// Token: 0x0400272F RID: 10031
		protected SkinnedModel mModel;

		// Token: 0x04002730 RID: 10032
		protected IBossState<Jormungandr> mGlobalState;

		// Token: 0x04002731 RID: 10033
		protected IBossState<Jormungandr> mCurrentState;

		// Token: 0x04002732 RID: 10034
		protected IBossState<Jormungandr> mLastState;

		// Token: 0x04002733 RID: 10035
		protected Jormungandr.RenderData[] mRenderData;

		// Token: 0x04002734 RID: 10036
		private bool mInitialized;

		// Token: 0x04002735 RID: 10037
		private PlayState mPlayState;

		// Token: 0x04002736 RID: 10038
		protected ConditionCollection mSpitConditions;

		// Token: 0x04002737 RID: 10039
		protected Jormungandr.DamageState mDamageState;

		// Token: 0x04002738 RID: 10040
		protected bool mMidLeap;

		// Token: 0x04002739 RID: 10041
		protected bool mDraw;

		// Token: 0x0400273A RID: 10042
		protected bool mUpdateAnimation;

		// Token: 0x0400273B RID: 10043
		protected bool mIsHit;

		// Token: 0x0400273C RID: 10044
		protected bool mDirtSpawned;

		// Token: 0x0400273D RID: 10045
		protected bool mNoEarthCollisionEffect;

		// Token: 0x0400273E RID: 10046
		protected Quaternion mSavedRotation;

		// Token: 0x0400273F RID: 10047
		protected Vector3 mSavedPosition;

		// Token: 0x04002740 RID: 10048
		protected float mIdleTimer;

		// Token: 0x04002741 RID: 10049
		protected int mAttackCount;

		// Token: 0x04002742 RID: 10050
		protected Vector3 mPosition;

		// Token: 0x04002743 RID: 10051
		protected Vector3 mDirection = Vector3.Right;

		// Token: 0x04002744 RID: 10052
		protected Matrix mOrientation;

		// Token: 0x04002745 RID: 10053
		protected int mHeadJointIndex;

		// Token: 0x04002746 RID: 10054
		protected Matrix mHeadBindPose;

		// Token: 0x04002747 RID: 10055
		protected int mNeckJointIndex;

		// Token: 0x04002748 RID: 10056
		protected Matrix mNeckBindPose;

		// Token: 0x04002749 RID: 10057
		protected int[] mSpineIndices;

		// Token: 0x0400274A RID: 10058
		protected Matrix[] mSpineBindPose;

		// Token: 0x0400274B RID: 10059
		protected JormungandrCollisionZone mSpine;

		// Token: 0x0400274C RID: 10060
		protected BossDamageZone mDamageZone;

		// Token: 0x0400274D RID: 10061
		protected Character mTarget;

		// Token: 0x0400274E RID: 10062
		protected DamageCollection5 mLeapDamage;

		// Token: 0x0400274F RID: 10063
		protected DamageCollection5 mEmergeDamage;

		// Token: 0x04002750 RID: 10064
		protected DamageCollection5 mBiteDamage;

		// Token: 0x04002751 RID: 10065
		protected DamageCollection5 mSpitDamage;

		// Token: 0x04002752 RID: 10066
		protected HitList mHitlist = new HitList(8);

		// Token: 0x04002753 RID: 10067
		protected HitList mSpineHitlist = new HitList(8);

		// Token: 0x04002754 RID: 10068
		protected AudioEmitter mAudioEmitter;

		// Token: 0x04002755 RID: 10069
		private bool mDead;

		// Token: 0x04002756 RID: 10070
		protected float mDamageFlashTimer;

		// Token: 0x04002757 RID: 10071
		protected Jormungandr.Animations mDeathAnimation;

		// Token: 0x04002758 RID: 10072
		protected VisualEffectReference mFrontTremor;

		// Token: 0x04002759 RID: 10073
		protected VisualEffectReference mBackTremor;

		// Token: 0x0400275A RID: 10074
		private SkinnedModelDeferredAdvancedMaterial mMaterial;

		// Token: 0x0400275B RID: 10075
		private TextureCube mIceCubeMap;

		// Token: 0x0400275C RID: 10076
		private TextureCube mIceCubeNormalMap;

		// Token: 0x020004AE RID: 1198
		protected class RenderData : RenderableObject<SkinnedModelDeferredEffect, SkinnedModelDeferredAdvancedMaterial>
		{
			// Token: 0x06002448 RID: 9288 RVA: 0x00105933 File Offset: 0x00103B33
			public RenderData()
			{
				this.mSkeleton = new Matrix[80];
			}

			// Token: 0x06002449 RID: 9289 RVA: 0x00105948 File Offset: 0x00103B48
			public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				skinnedModelDeferredEffect.OverrideColor = new Vector4(1f, 1f, 1f, this.Flash);
				skinnedModelDeferredEffect.Bones = this.mSkeleton;
				skinnedModelDeferredEffect.Damage = this.Damage;
				base.Draw(iEffect, iViewFrustum);
				skinnedModelDeferredEffect.OverrideColor = Vector4.Zero;
			}

			// Token: 0x0600244A RID: 9290 RVA: 0x001059A8 File Offset: 0x00103BA8
			public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				skinnedModelDeferredEffect.Bones = this.mSkeleton;
				base.DrawShadow(iEffect, iViewFrustum);
			}

			// Token: 0x0400275D RID: 10077
			public float Damage;

			// Token: 0x0400275E RID: 10078
			public float Flash;

			// Token: 0x0400275F RID: 10079
			public Matrix[] mSkeleton;
		}

		// Token: 0x020004AF RID: 1199
		public enum State : byte
		{
			// Token: 0x04002761 RID: 10081
			Sleep,
			// Token: 0x04002762 RID: 10082
			Intro,
			// Token: 0x04002763 RID: 10083
			Underground,
			// Token: 0x04002764 RID: 10084
			Leap,
			// Token: 0x04002765 RID: 10085
			Risen,
			// Token: 0x04002766 RID: 10086
			Spit,
			// Token: 0x04002767 RID: 10087
			Bite,
			// Token: 0x04002768 RID: 10088
			HitRisen,
			// Token: 0x04002769 RID: 10089
			HitLeap,
			// Token: 0x0400276A RID: 10090
			Dead
		}

		// Token: 0x020004B0 RID: 1200
		protected class GlobalState : IBossState<Jormungandr>
		{
			// Token: 0x1700088D RID: 2189
			// (get) Token: 0x0600244B RID: 9291 RVA: 0x001059D0 File Offset: 0x00103BD0
			public static Jormungandr.GlobalState Instance
			{
				get
				{
					if (Jormungandr.GlobalState.mSingelton == null)
					{
						lock (Jormungandr.GlobalState.mSingeltonLock)
						{
							if (Jormungandr.GlobalState.mSingelton == null)
							{
								Jormungandr.GlobalState.mSingelton = new Jormungandr.GlobalState();
							}
						}
					}
					return Jormungandr.GlobalState.mSingelton;
				}
			}

			// Token: 0x0600244C RID: 9292 RVA: 0x00105A24 File Offset: 0x00103C24
			private GlobalState()
			{
				Jormungandr.IntroState instance = Jormungandr.IntroState.Instance;
				Jormungandr.UndergroundState instance2 = Jormungandr.UndergroundState.Instance;
				Jormungandr.LeapState instance3 = Jormungandr.LeapState.Instance;
				Jormungandr.RisenState instance4 = Jormungandr.RisenState.Instance;
				Jormungandr.SpitState instance5 = Jormungandr.SpitState.Instance;
				Jormungandr.BiteState instance6 = Jormungandr.BiteState.Instance;
				Jormungandr.HitRisenState instance7 = Jormungandr.HitRisenState.Instance;
				Jormungandr.HitLeapState instance8 = Jormungandr.HitLeapState.Instance;
				Jormungandr.DeadState instance9 = Jormungandr.DeadState.Instance;
			}

			// Token: 0x0600244D RID: 9293 RVA: 0x00105A62 File Offset: 0x00103C62
			public void OnEnter(Jormungandr iOwner)
			{
				throw new NotImplementedException();
			}

			// Token: 0x0600244E RID: 9294 RVA: 0x00105A6C File Offset: 0x00103C6C
			public unsafe void OnUpdate(float iDeltaTime, Jormungandr iOwner)
			{
				if (iOwner.mCurrentState is Jormungandr.DeadState)
				{
					return;
				}
				if (iOwner.mIsHit)
				{
					if (iOwner.mCurrentState is Jormungandr.LeapState)
					{
						if (NetworkManager.Instance.State != NetworkState.Client)
						{
							if (NetworkManager.Instance.State == NetworkState.Server)
							{
								Jormungandr.ChangeStateMessage changeStateMessage;
								changeStateMessage.NewState = Jormungandr.State.HitLeap;
								changeStateMessage.Animation = Jormungandr.Animations.Invalid;
								BossFight.Instance.SendMessage<Jormungandr.ChangeStateMessage>(iOwner, 1, (void*)(&changeStateMessage), true);
							}
							iOwner.ChangeState(Jormungandr.HitLeapState.Instance);
							return;
						}
					}
					else if (iOwner.mCurrentState is Jormungandr.RisenState || iOwner.mCurrentState is Jormungandr.BiteState)
					{
						if (NetworkManager.Instance.State != NetworkState.Client)
						{
							if (NetworkManager.Instance.State == NetworkState.Server)
							{
								Jormungandr.ChangeStateMessage changeStateMessage2;
								changeStateMessage2.NewState = Jormungandr.State.HitRisen;
								changeStateMessage2.Animation = Jormungandr.Animations.Invalid;
								BossFight.Instance.SendMessage<Jormungandr.ChangeStateMessage>(iOwner, 1, (void*)(&changeStateMessage2), true);
							}
							iOwner.ChangeState(Jormungandr.HitRisenState.Instance);
							return;
						}
					}
					else
					{
						iOwner.mIsHit = false;
					}
				}
			}

			// Token: 0x0600244F RID: 9295 RVA: 0x00105B4F File Offset: 0x00103D4F
			public void OnExit(Jormungandr iOwner)
			{
				throw new NotImplementedException();
			}

			// Token: 0x0400276B RID: 10091
			private static Jormungandr.GlobalState mSingelton;

			// Token: 0x0400276C RID: 10092
			private static volatile object mSingeltonLock = new object();
		}

		// Token: 0x020004B1 RID: 1201
		protected class SleepIntroState : IBossState<Jormungandr>
		{
			// Token: 0x1700088E RID: 2190
			// (get) Token: 0x06002451 RID: 9297 RVA: 0x00105B64 File Offset: 0x00103D64
			public static Jormungandr.SleepIntroState Instance
			{
				get
				{
					if (Jormungandr.SleepIntroState.mSingelton == null)
					{
						lock (Jormungandr.SleepIntroState.mSingeltonLock)
						{
							if (Jormungandr.SleepIntroState.mSingelton == null)
							{
								Jormungandr.SleepIntroState.mSingelton = new Jormungandr.SleepIntroState();
							}
						}
					}
					return Jormungandr.SleepIntroState.mSingelton;
				}
			}

			// Token: 0x06002452 RID: 9298 RVA: 0x00105BB8 File Offset: 0x00103DB8
			private SleepIntroState()
			{
			}

			// Token: 0x06002453 RID: 9299 RVA: 0x00105BC0 File Offset: 0x00103DC0
			public void OnEnter(Jormungandr iOwner)
			{
				iOwner.mAnimationController.StartClip(iOwner.mAnimationClips[1], false);
				Matrix mOrientation = iOwner.mOrientation;
				mOrientation.Translation = iOwner.mPosition;
				iOwner.mAnimationController.Update(0f, ref mOrientation, true);
				iOwner.mAnimationController.Stop();
				iOwner.mDraw = true;
				iOwner.mUpdateAnimation = true;
				iOwner.mNoEarthCollisionEffect = true;
			}

			// Token: 0x06002454 RID: 9300 RVA: 0x00105C28 File Offset: 0x00103E28
			public void OnUpdate(float iDeltaTime, Jormungandr iOwner)
			{
			}

			// Token: 0x06002455 RID: 9301 RVA: 0x00105C2A File Offset: 0x00103E2A
			public void OnExit(Jormungandr iOwner)
			{
			}

			// Token: 0x0400276D RID: 10093
			private static Jormungandr.SleepIntroState mSingelton;

			// Token: 0x0400276E RID: 10094
			private static volatile object mSingeltonLock = new object();
		}

		// Token: 0x020004B2 RID: 1202
		protected class IntroState : IBossState<Jormungandr>
		{
			// Token: 0x1700088F RID: 2191
			// (get) Token: 0x06002457 RID: 9303 RVA: 0x00105C3C File Offset: 0x00103E3C
			public static Jormungandr.IntroState Instance
			{
				get
				{
					if (Jormungandr.IntroState.mSingelton == null)
					{
						lock (Jormungandr.IntroState.mSingeltonLock)
						{
							if (Jormungandr.IntroState.mSingelton == null)
							{
								Jormungandr.IntroState.mSingelton = new Jormungandr.IntroState();
							}
						}
					}
					return Jormungandr.IntroState.mSingelton;
				}
			}

			// Token: 0x06002458 RID: 9304 RVA: 0x00105C90 File Offset: 0x00103E90
			private IntroState()
			{
			}

			// Token: 0x06002459 RID: 9305 RVA: 0x00105C98 File Offset: 0x00103E98
			public void OnEnter(Jormungandr iOwner)
			{
				iOwner.mAnimationController.StartClip(iOwner.mAnimationClips[1], false);
				AudioManager.Instance.PlayCue(Banks.Characters, "boss_jormungandr_wakeup".GetHashCodeCustom(), iOwner.mDamageZone.AudioEmitter);
				iOwner.mDraw = true;
				iOwner.mUpdateAnimation = true;
				iOwner.mNoEarthCollisionEffect = true;
				this.mPlayedCue = false;
			}

			// Token: 0x0600245A RID: 9306 RVA: 0x00105CF8 File Offset: 0x00103EF8
			public unsafe void OnUpdate(float iDeltaTime, Jormungandr iOwner)
			{
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					if (iOwner.mAnimationController.HasFinished)
					{
						if (NetworkManager.Instance.State != NetworkState.Client)
						{
							if (NetworkManager.Instance.State == NetworkState.Server)
							{
								Jormungandr.ChangeStateMessage changeStateMessage;
								changeStateMessage.NewState = Jormungandr.State.Underground;
								changeStateMessage.Animation = Jormungandr.Animations.Invalid;
								BossFight.Instance.SendMessage<Jormungandr.ChangeStateMessage>(iOwner, 1, (void*)(&changeStateMessage), true);
							}
							iOwner.ChangeState(Jormungandr.UndergroundState.Instance);
							return;
						}
					}
					else if (iOwner.mAnimationController.Time > 2.2f && !this.mPlayedCue)
					{
						AudioManager.Instance.PlayCue(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[2], iOwner.mDamageZone.AudioEmitter);
						this.mPlayedCue = true;
					}
				}
			}

			// Token: 0x0600245B RID: 9307 RVA: 0x00105DA8 File Offset: 0x00103FA8
			public void OnExit(Jormungandr iOwner)
			{
				iOwner.mIdleTimer = 0f;
			}

			// Token: 0x0400276F RID: 10095
			private static Jormungandr.IntroState mSingelton;

			// Token: 0x04002770 RID: 10096
			private static volatile object mSingeltonLock = new object();

			// Token: 0x04002771 RID: 10097
			private bool mPlayedCue;
		}

		// Token: 0x020004B3 RID: 1203
		protected class UndergroundState : IBossState<Jormungandr>
		{
			// Token: 0x17000890 RID: 2192
			// (get) Token: 0x0600245D RID: 9309 RVA: 0x00105DC4 File Offset: 0x00103FC4
			public static Jormungandr.UndergroundState Instance
			{
				get
				{
					if (Jormungandr.UndergroundState.mSingelton == null)
					{
						lock (Jormungandr.UndergroundState.mSingeltonLock)
						{
							if (Jormungandr.UndergroundState.mSingelton == null)
							{
								Jormungandr.UndergroundState.mSingelton = new Jormungandr.UndergroundState();
							}
						}
					}
					return Jormungandr.UndergroundState.mSingelton;
				}
			}

			// Token: 0x0600245E RID: 9310 RVA: 0x00105E18 File Offset: 0x00104018
			private UndergroundState()
			{
			}

			// Token: 0x0600245F RID: 9311 RVA: 0x00105E20 File Offset: 0x00104020
			public void OnEnter(Jormungandr iOwner)
			{
				EffectManager.Instance.Stop(ref iOwner.mEarthSplashReference);
				EffectManager.Instance.Stop(ref iOwner.mEarthTremorReference);
				iOwner.mNoEarthCollisionEffect = false;
				iOwner.mDirtSpawned = false;
				iOwner.mDraw = false;
				iOwner.mUpdateAnimation = false;
				iOwner.mIsHit = false;
				iOwner.mIdleTimer = 0f;
				iOwner.mAttackCount = 0;
				iOwner.mSpineHitlist.Clear();
				iOwner.mHitlist.Clear();
				for (int i = 0; i < iOwner.mStatusEffects.Length; i++)
				{
					iOwner.mStatusEffects[i].Stop();
					iOwner.mStatusEffects[i] = default(StatusEffect);
				}
			}

			// Token: 0x06002460 RID: 9312 RVA: 0x00105ED0 File Offset: 0x001040D0
			public unsafe void OnUpdate(float iDeltaTime, Jormungandr iOwner)
			{
				if (NetworkManager.Instance.State == NetworkState.Client)
				{
					return;
				}
				if (iOwner.HitPoints <= 0f)
				{
					if (NetworkManager.Instance.State == NetworkState.Server)
					{
						Jormungandr.ChangeStateMessage changeStateMessage;
						changeStateMessage.NewState = Jormungandr.State.Dead;
						changeStateMessage.Animation = Jormungandr.Animations.Invalid;
						BossFight.Instance.SendMessage<Jormungandr.ChangeStateMessage>(iOwner, 1, (void*)(&changeStateMessage), true);
					}
					iOwner.mDeathAnimation = Jormungandr.Animations.Die_Below;
					iOwner.ChangeState(Jormungandr.DeadState.Instance);
					return;
				}
				iOwner.mIdleTimer += iDeltaTime;
				if (iOwner.mIdleTimer > Jormungandr.WARNINGTIME)
				{
					iOwner.SelectTarget(Jormungandr.TargettingType.Random);
					float iMagnitude = 0.5f + (float)MagickaMath.Random.NextDouble() * 0.5f;
					iOwner.mPlayState.Camera.CameraShake(iOwner.mDamageZone.Position, iMagnitude, Jormungandr.WARNINGTIME);
					float num = (float)Jormungandr.mRandom.NextDouble();
					float num2 = 0.4f * iOwner.mHitPoints / 10000f;
					iOwner.mAttackCount = Jormungandr.mRandom.Next(2) + 2;
					if (num <= 0.35f + num2)
					{
						iOwner.mPosition = iOwner.mTarget.Position;
						iOwner.mPosition.Y = iOwner.mPosition.Y - 1f;
						iOwner.mOrientation = Matrix.CreateRotationY(2.5f);
						iOwner.mOrientation.Translation = iOwner.mPosition;
						iOwner.mHitlist.Clear();
						Vector3 mPosition = iOwner.mPosition;
						iOwner.mDamageZone.SetPosition(ref mPosition);
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							Jormungandr.ChangeStateMessage changeStateMessage2;
							changeStateMessage2.NewState = Jormungandr.State.Risen;
							changeStateMessage2.Animation = Jormungandr.Animations.Rise;
							changeStateMessage2.AnimationBlendTime = 0f;
							changeStateMessage2.AnimationLooped = false;
							BossFight.Instance.SendMessage<Jormungandr.ChangeStateMessage>(iOwner, 1, (void*)(&changeStateMessage2), true);
						}
						iOwner.mAnimationController.StartClip(iOwner.mAnimationClips[4], false);
						iOwner.ChangeState(Jormungandr.RisenState.Instance);
						return;
					}
					bool flag = true;
					Vector3 mPosition2 = default(Vector3);
					Vector3 position = iOwner.mTarget.Position;
					Vector3 vector = default(Vector3);
					double num3 = Jormungandr.mRandom.NextDouble() * 3.141592653589793 * 2.0;
					for (int i = 0; i < 6; i++)
					{
						vector.X = (float)Math.Cos(num3);
						vector.Z = (float)Math.Sin(num3);
						num3 += 1.0471975511965976;
						Vector3 vector2 = vector;
						Vector3.Negate(ref vector, out vector2);
						Vector3 vector3;
						Vector3.Multiply(ref vector, 14f, out vector3);
						Vector3.Add(ref position, ref vector3, out mPosition2);
						Vector3 vector4 = default(Vector3);
						iOwner.mPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref mPosition2, out vector4, MovementProperties.Default);
						Vector3 vector5;
						Vector3.Subtract(ref vector4, ref mPosition2, out vector5);
						Vector3 vector6 = default(Vector3);
						Vector3.Subtract(ref position, ref vector4, out vector6);
						vector6.Y = 0f;
						vector6.Normalize();
						bool flag2 = false;
						Segment seg = default(Segment);
						seg.Origin = vector4;
						seg.Delta.Y = 10f;
						for (int j = 0; j < iOwner.mPlayState.Level.CurrentScene.Liquids.Length; j++)
						{
							float num4;
							Vector3 vector7;
							Vector3 vector8;
							if (iOwner.mPlayState.Level.CurrentScene.Liquids[j].CollisionSkin.SegmentIntersect(out num4, out vector7, out vector8, seg))
							{
								flag2 = true;
							}
						}
						if (vector5.Length() <= 2f && !flag2)
						{
							flag = false;
							mPosition2 = vector4;
							vector = vector6;
						}
					}
					if (!flag)
					{
						iOwner.mPosition = mPosition2;
						Segment seg2 = default(Segment);
						seg2.Delta.Y = -10f;
						Vector3.Negate(ref vector, out iOwner.mDirection);
						Vector3 vector9;
						Vector3.Multiply(ref vector, 14f, out vector9);
						Vector3.Add(ref position, ref vector9, out seg2.Origin);
						float num5;
						Vector3 vector10;
						iOwner.mPlayState.Level.CurrentScene.CollisionSkin.SegmentIntersect(out num5, out iOwner.mPosition, out vector10, seg2);
						Jormungandr.CreateTransform(ref iOwner.mPosition, ref iOwner.mDirection, out iOwner.mOrientation);
						iOwner.mOrientation.M41 = iOwner.mOrientation.M41 - vector.X * 8f;
						iOwner.mOrientation.M42 = iOwner.mOrientation.M42 - 9.5f;
						iOwner.mOrientation.M43 = iOwner.mOrientation.M43 - vector.Z * 8f;
						iOwner.mDamageZone.SetPosition(ref mPosition2);
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							Jormungandr.ChangeStateMessage changeStateMessage3;
							changeStateMessage3.NewState = Jormungandr.State.Leap;
							changeStateMessage3.Animation = Jormungandr.Animations.Invalid;
							BossFight.Instance.SendMessage<Jormungandr.ChangeStateMessage>(iOwner, 1, (void*)(&changeStateMessage3), true);
						}
						iOwner.ChangeState(Jormungandr.LeapState.Instance);
					}
				}
			}

			// Token: 0x06002461 RID: 9313 RVA: 0x00106367 File Offset: 0x00104567
			public void OnExit(Jormungandr iOwner)
			{
			}

			// Token: 0x04002772 RID: 10098
			private static Jormungandr.UndergroundState mSingelton;

			// Token: 0x04002773 RID: 10099
			private static volatile object mSingeltonLock = new object();
		}

		// Token: 0x020004B4 RID: 1204
		protected class LeapState : IBossState<Jormungandr>
		{
			// Token: 0x17000891 RID: 2193
			// (get) Token: 0x06002463 RID: 9315 RVA: 0x00106378 File Offset: 0x00104578
			public static Jormungandr.LeapState Instance
			{
				get
				{
					if (Jormungandr.LeapState.mSingelton == null)
					{
						lock (Jormungandr.LeapState.mSingeltonLock)
						{
							if (Jormungandr.LeapState.mSingelton == null)
							{
								Jormungandr.LeapState.mSingelton = new Jormungandr.LeapState();
							}
						}
					}
					return Jormungandr.LeapState.mSingelton;
				}
			}

			// Token: 0x06002464 RID: 9316 RVA: 0x001063CC File Offset: 0x001045CC
			private LeapState()
			{
				this.mWhipDamage = default(DamageCollection5);
				this.mWhipDamage.AddDamage(new Damage(AttackProperties.Knockback, Elements.Earth, 200f, 4f));
				this.mWhipDamage.AddDamage(new Damage(AttackProperties.Damage, Elements.Earth, 300f, 1f));
			}

			// Token: 0x06002465 RID: 9317 RVA: 0x00106424 File Offset: 0x00104624
			public void OnEnter(Jormungandr iOwner)
			{
				Matrix matrix;
				Matrix.CreateRotationX(MathHelper.ToRadians(125f), out matrix);
				Matrix.Multiply(ref matrix, ref iOwner.mOrientation, out iOwner.mOrientation);
				iOwner.mAnimationController.StartClip(iOwner.mAnimationClips[2], false);
				iOwner.mUpdateAnimation = false;
				iOwner.mDraw = false;
				iOwner.mDamageState = Jormungandr.DamageState.Leap;
				iOwner.mIdleTimer = 0f;
				iOwner.mHitlist.Clear();
				iOwner.mDirtSpawned = false;
				iOwner.mNoEarthCollisionEffect = false;
				iOwner.mMidLeap = false;
				this.mHasGrowled = false;
				this.mHasWhipped = false;
				this.mHasWhipDamaged = false;
				this.mTargetLock = false;
			}

			// Token: 0x06002466 RID: 9318 RVA: 0x001064C8 File Offset: 0x001046C8
			public unsafe void OnUpdate(float iDeltaTime, Jormungandr iOwner)
			{
				float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
				iOwner.mIdleTimer += iDeltaTime;
				if (!iOwner.mAnimationController.CrossFadeEnabled && num > Jormungandr.TAIL_WHIP && !this.mHasWhipDamaged)
				{
					Matrix matrix;
					Matrix.Multiply(ref iOwner.mSpineBindPose[1], ref iOwner.mAnimationController.SkinnedBoneTransforms[iOwner.mSpineIndices[1]], out matrix);
					Vector3 translation = matrix.Translation;
					Helper.CircleDamage(iOwner.mPlayState, iOwner.mDamageZone, iOwner.mPlayState.PlayTime, iOwner.mSpine, ref translation, 3f, ref this.mWhipDamage);
					this.mHasWhipDamaged = true;
				}
				if (!this.mTargetLock)
				{
					Vector3 position = iOwner.mTarget.Position;
					Vector3.Subtract(ref position, ref iOwner.mPosition, out iOwner.mDirection);
					iOwner.mDirection.Normalize();
					Jormungandr.CreateTransform(ref iOwner.mPosition, ref iOwner.mDirection, out iOwner.mOrientation);
					Matrix matrix2;
					Matrix.CreateRotationX(MathHelper.ToRadians(125f), out matrix2);
					Matrix.Multiply(ref matrix2, ref iOwner.mOrientation, out iOwner.mOrientation);
					iOwner.mOrientation.M41 = iOwner.mOrientation.M41 + iOwner.mDirection.X * 8f;
					iOwner.mOrientation.M42 = iOwner.mOrientation.M42 - 9.5f;
					iOwner.mOrientation.M43 = iOwner.mOrientation.M43 + iOwner.mDirection.Z * 8f;
					this.mTargetLock = true;
					AudioManager.Instance.PlayCue(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[3]);
				}
				if (iOwner.mIdleTimer > Jormungandr.WARNINGTIME)
				{
					if (iOwner.mIdleTimer > Jormungandr.WARNINGTIME + 0.9f && !iOwner.mMidLeap)
					{
						iOwner.mMidLeap = true;
						iOwner.mDirtSpawned = false;
						EffectManager.Instance.Stop(ref iOwner.mEarthSplashReference);
					}
					else if (iOwner.mIdleTimer > Jormungandr.WARNINGTIME + 0.6f && !this.mHasGrowled)
					{
						AudioManager.Instance.PlayCue(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[7]);
						this.mHasGrowled = true;
					}
					else if (iOwner.mIdleTimer > Jormungandr.WARNINGTIME + 1.9f && !this.mHasWhipped)
					{
						AudioManager.Instance.PlayCue(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[13]);
						this.mHasWhipped = true;
					}
					EffectManager.Instance.Stop(ref iOwner.mEarthTremorReference);
					iOwner.mUpdateAnimation = true;
					iOwner.mDraw = true;
					float num2 = 275f / iOwner.mAnimationController.AnimationClip.Duration * iDeltaTime;
					Matrix matrix3;
					Matrix.CreateRotationX(MathHelper.ToRadians(-num2), out matrix3);
					Matrix.Multiply(ref matrix3, ref iOwner.mOrientation, out iOwner.mOrientation);
					iOwner.mPosition = iOwner.mDamageZone.Position;
					if (iOwner.mAnimationController.HasFinished && NetworkManager.Instance.State != NetworkState.Client)
					{
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							Jormungandr.ChangeStateMessage changeStateMessage;
							changeStateMessage.NewState = Jormungandr.State.Underground;
							changeStateMessage.Animation = Jormungandr.Animations.Invalid;
							BossFight.Instance.SendMessage<Jormungandr.ChangeStateMessage>(iOwner, 1, (void*)(&changeStateMessage), true);
						}
						iOwner.ChangeState(Jormungandr.UndergroundState.Instance);
					}
				}
			}

			// Token: 0x06002467 RID: 9319 RVA: 0x001067E8 File Offset: 0x001049E8
			public void OnExit(Jormungandr iOwner)
			{
				iOwner.mDamageState = Jormungandr.DamageState.None;
				iOwner.mPosition = iOwner.mDamageZone.Position;
				EffectManager.Instance.Stop(ref iOwner.mEarthSplashReference);
				iOwner.mMidLeap = false;
				this.mTargetLock = false;
				iOwner.mAnimationController.Speed = 1f;
			}

			// Token: 0x04002774 RID: 10100
			private static Jormungandr.LeapState mSingelton;

			// Token: 0x04002775 RID: 10101
			private static volatile object mSingeltonLock = new object();

			// Token: 0x04002776 RID: 10102
			protected bool mTargetLock;

			// Token: 0x04002777 RID: 10103
			protected bool mHasGrowled;

			// Token: 0x04002778 RID: 10104
			protected bool mHasWhipped;

			// Token: 0x04002779 RID: 10105
			protected bool mHasWhipDamaged;

			// Token: 0x0400277A RID: 10106
			private DamageCollection5 mWhipDamage;
		}

		// Token: 0x020004B5 RID: 1205
		protected class RisenState : IBossState<Jormungandr>
		{
			// Token: 0x17000892 RID: 2194
			// (get) Token: 0x06002469 RID: 9321 RVA: 0x0010684C File Offset: 0x00104A4C
			public static Jormungandr.RisenState Instance
			{
				get
				{
					if (Jormungandr.RisenState.mSingelton == null)
					{
						lock (Jormungandr.RisenState.mSingeltonLock)
						{
							if (Jormungandr.RisenState.mSingelton == null)
							{
								Jormungandr.RisenState.mSingelton = new Jormungandr.RisenState();
							}
						}
					}
					return Jormungandr.RisenState.mSingelton;
				}
			}

			// Token: 0x0600246A RID: 9322 RVA: 0x001068A0 File Offset: 0x00104AA0
			private RisenState()
			{
			}

			// Token: 0x0600246B RID: 9323 RVA: 0x001068B8 File Offset: 0x00104AB8
			public void OnEnter(Jormungandr iOwner)
			{
				iOwner.mIdleTimer = 0f;
				iOwner.mNoEarthCollisionEffect = false;
				iOwner.mDirtSpawned = false;
				this.mBarriersCleared = false;
				this.mRoarPlayed = false;
				iOwner.mSpineHitlist.Clear();
				if (iOwner.mAnimationController.AnimationClip == iOwner.mAnimationClips[4])
				{
					iOwner.StartEffect(out iOwner.mEarthTremorReference, Jormungandr.JORMUNGANDREFFECTS[1]);
					AudioManager.Instance.PlayCue(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[3]);
				}
				iOwner.SelectTarget(Jormungandr.TargettingType.Angle);
			}

			// Token: 0x0600246C RID: 9324 RVA: 0x0010693C File Offset: 0x00104B3C
			public unsafe void OnUpdate(float iDeltaTime, Jormungandr iOwner)
			{
				iOwner.mIdleTimer += iDeltaTime;
				if (iOwner.mAnimationController.AnimationClip == iOwner.mAnimationClips[5])
				{
					if (!this.mBarriersCleared)
					{
						List<Entity> entities = iOwner.mPlayState.EntityManager.GetEntities(iOwner.mPosition, 3f, false);
						for (int i = 0; i < entities.Count; i++)
						{
							if (entities[i] is Shield | entities[i] is Barrier)
							{
								IDamageable damageable = entities[i] as IDamageable;
								Vector3 position = entities[i].Position;
								Vector3 right = Vector3.Right;
								EffectManager.Instance.StartEffect(this.mDeathEffectHash, ref position, ref right, out this.mDeathEffect);
								damageable.Kill();
							}
						}
						iOwner.mPlayState.EntityManager.ReturnEntityList(entities);
						this.mBarriersCleared = true;
					}
					if (!iOwner.mDirtSpawned)
					{
						AudioManager.Instance.PlayCue(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[2]);
						iOwner.mDirtSpawned = true;
						iOwner.mNoEarthCollisionEffect = true;
						float iMagnitude = 0.5f + (float)MagickaMath.Random.NextDouble() * 0.5f;
						iOwner.mPlayState.Camera.CameraShake(iMagnitude, 1f);
					}
					if (iOwner.mAnimationController.HasFinished && !iOwner.mAnimationController.CrossFadeEnabled && NetworkManager.Instance.State != NetworkState.Client)
					{
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							Jormungandr.ChangeStateMessage changeStateMessage;
							changeStateMessage.NewState = Jormungandr.State.Underground;
							changeStateMessage.Animation = Jormungandr.Animations.Invalid;
							BossFight.Instance.SendMessage<Jormungandr.ChangeStateMessage>(iOwner, 1, (void*)(&changeStateMessage), true);
						}
						iOwner.ChangeState(Jormungandr.UndergroundState.Instance);
						return;
					}
				}
				else if (iOwner.mAnimationController.AnimationClip == iOwner.mAnimationClips[4])
				{
					if (iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration >= 0.125f && !this.mRoarPlayed)
					{
						this.mRoarPlayed = true;
						AudioManager.Instance.PlayCue(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[5]);
					}
					if (iOwner.mAnimationController.HasFinished)
					{
						if (NetworkManager.Instance.State != NetworkState.Client)
						{
							if (NetworkManager.Instance.State == NetworkState.Server)
							{
								Jormungandr.ChangeStateMessage changeStateMessage2;
								changeStateMessage2.NewState = Jormungandr.State.Risen;
								changeStateMessage2.Animation = Jormungandr.Animations.Idle;
								changeStateMessage2.AnimationBlendTime = 0.5f;
								changeStateMessage2.AnimationLooped = true;
								BossFight.Instance.SendMessage<Jormungandr.ChangeStateMessage>(iOwner, 1, (void*)(&changeStateMessage2), true);
							}
							iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[6], 0.5f, true);
						}
						iOwner.mIdleTimer = 0f;
					}
					if (iOwner.mIdleTimer > Jormungandr.WARNINGTIME || iOwner.mDirtSpawned)
					{
						iOwner.mDraw = true;
						iOwner.mUpdateAnimation = true;
						iOwner.mDamageState = Jormungandr.DamageState.Emerge;
						if (!this.mBarriersCleared)
						{
							List<Entity> entities2 = iOwner.mPlayState.EntityManager.GetEntities(iOwner.mPosition, 2f, false);
							for (int j = 0; j < entities2.Count; j++)
							{
								if (entities2[j] is Shield | entities2[j] is Barrier)
								{
									IDamageable damageable2 = entities2[j] as IDamageable;
									Vector3 position2 = entities2[j].Position;
									Vector3 right2 = Vector3.Right;
									EffectManager.Instance.StartEffect(this.mDeathEffectHash, ref position2, ref right2, out this.mDeathEffect);
									damageable2.Kill();
								}
							}
							iOwner.mPlayState.EntityManager.ReturnEntityList(entities2);
							this.mBarriersCleared = true;
							return;
						}
					}
				}
				else
				{
					if (NetworkManager.Instance.State != NetworkState.Client && iOwner.mHitPoints <= 0f)
					{
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							Jormungandr.ChangeStateMessage changeStateMessage3;
							changeStateMessage3.NewState = Jormungandr.State.Dead;
							changeStateMessage3.Animation = Jormungandr.Animations.Invalid;
							BossFight.Instance.SendMessage<Jormungandr.ChangeStateMessage>(iOwner, 1, (void*)(&changeStateMessage3), true);
						}
						iOwner.mDeathAnimation = Jormungandr.Animations.Die_Above;
						iOwner.ChangeState(Jormungandr.DeadState.Instance);
						return;
					}
					if (!iOwner.mAnimationController.CrossFadeEnabled && iOwner.mAttackCount <= 0)
					{
						if (NetworkManager.Instance.State != NetworkState.Client)
						{
							if (NetworkManager.Instance.State == NetworkState.Server)
							{
								Jormungandr.ChangeStateMessage changeStateMessage4;
								changeStateMessage4.NewState = Jormungandr.State.Risen;
								changeStateMessage4.Animation = Jormungandr.Animations.Submerge;
								changeStateMessage4.AnimationBlendTime = 0.5f;
								changeStateMessage4.AnimationLooped = false;
								BossFight.Instance.SendMessage<Jormungandr.ChangeStateMessage>(iOwner, 1, (void*)(&changeStateMessage4), true);
							}
							iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[5], 0.5f, false);
							return;
						}
					}
					else
					{
						if (iOwner.mTarget != null)
						{
							Vector3 position3 = iOwner.mDamageZone.Position;
							Vector3 position4 = iOwner.mTarget.Position;
							position3.Y = (position4.Y = 0f);
							Vector3 backward;
							Vector3.Subtract(ref position4, ref position3, out backward);
							float num = backward.Length();
							if (num > 1E-45f)
							{
								Vector3.Divide(ref backward, num, out backward);
							}
							else
							{
								backward = Vector3.Backward;
							}
							iOwner.Turn(ref backward, 2f, iDeltaTime);
						}
						if (iOwner.mIdleTimer > Jormungandr.WARNINGTIME + 0.8f && iOwner.mTarget != null)
						{
							Vector3 position4 = iOwner.mTarget.Position;
							Vector3 backward;
							Vector3.Subtract(ref position4, ref iOwner.mPosition, out backward);
							float num = backward.Length();
							backward.Y = 0f;
							bool flag = false;
							Vector3 mPosition = iOwner.mPosition;
							Vector3 forward = iOwner.mOrientation.Forward;
							Vector3.Multiply(ref forward, 3f, out forward);
							Vector3.Add(ref mPosition, ref forward, out mPosition);
							List<Entity> entities3 = iOwner.mPlayState.EntityManager.GetEntities(mPosition, 2f, false);
							for (int k = 0; k < entities3.Count; k++)
							{
								if (entities3[k] is Barrier && (entities3[k] as Barrier).Solid)
								{
									flag = true;
									break;
								}
							}
							iOwner.mPlayState.EntityManager.ReturnEntityList(entities3);
							if ((num > 15f || flag) && iOwner.mLastState != Jormungandr.SpitState.Instance)
							{
								if (NetworkManager.Instance.State != NetworkState.Client)
								{
									if (NetworkManager.Instance.State == NetworkState.Server)
									{
										Jormungandr.ChangeStateMessage changeStateMessage5;
										changeStateMessage5.NewState = Jormungandr.State.Spit;
										changeStateMessage5.Animation = Jormungandr.Animations.Invalid;
										BossFight.Instance.SendMessage<Jormungandr.ChangeStateMessage>(iOwner, 1, (void*)(&changeStateMessage5), true);
									}
									iOwner.ChangeState(Jormungandr.SpitState.Instance);
									return;
								}
							}
							else if (NetworkManager.Instance.State != NetworkState.Client)
							{
								if (NetworkManager.Instance.State == NetworkState.Server)
								{
									Jormungandr.ChangeStateMessage changeStateMessage6;
									changeStateMessage6.NewState = Jormungandr.State.Bite;
									changeStateMessage6.Animation = Jormungandr.Animations.Invalid;
									BossFight.Instance.SendMessage<Jormungandr.ChangeStateMessage>(iOwner, 1, (void*)(&changeStateMessage6), true);
								}
								iOwner.ChangeState(Jormungandr.BiteState.Instance);
							}
						}
					}
				}
			}

			// Token: 0x0600246D RID: 9325 RVA: 0x00106FA1 File Offset: 0x001051A1
			public void OnExit(Jormungandr iOwner)
			{
				iOwner.mDamageState = Jormungandr.DamageState.None;
			}

			// Token: 0x0400277B RID: 10107
			private static Jormungandr.RisenState mSingelton;

			// Token: 0x0400277C RID: 10108
			private static volatile object mSingeltonLock = new object();

			// Token: 0x0400277D RID: 10109
			private VisualEffectReference mDeathEffect;

			// Token: 0x0400277E RID: 10110
			private int mDeathEffectHash = "barrier_earth_death".GetHashCodeCustom();

			// Token: 0x0400277F RID: 10111
			private bool mBarriersCleared;

			// Token: 0x04002780 RID: 10112
			private bool mRoarPlayed;
		}

		// Token: 0x020004B6 RID: 1206
		protected class SpitState : IBossState<Jormungandr>
		{
			// Token: 0x17000893 RID: 2195
			// (get) Token: 0x0600246F RID: 9327 RVA: 0x00106FB8 File Offset: 0x001051B8
			public static Jormungandr.SpitState Instance
			{
				get
				{
					if (Jormungandr.SpitState.mSingelton == null)
					{
						lock (Jormungandr.SpitState.mSingeltonLock)
						{
							if (Jormungandr.SpitState.mSingelton == null)
							{
								Jormungandr.SpitState.mSingelton = new Jormungandr.SpitState();
							}
						}
					}
					return Jormungandr.SpitState.mSingelton;
				}
			}

			// Token: 0x06002470 RID: 9328 RVA: 0x0010700C File Offset: 0x0010520C
			private SpitState()
			{
			}

			// Token: 0x06002471 RID: 9329 RVA: 0x00107014 File Offset: 0x00105214
			public void OnEnter(Jormungandr iOwner)
			{
				iOwner.mAnimationController.Speed = 1.333f;
				iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[8], 0.2f, false);
				iOwner.mDamageState = Jormungandr.DamageState.None;
				AudioManager.Instance.PlayCue(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[9]);
				iOwner.SelectTarget(Jormungandr.TargettingType.Random);
				this.mNumProjectiles = 0;
				this.mSpitTimer = 0f;
				this.mHasSprayed = false;
			}

			// Token: 0x06002472 RID: 9330 RVA: 0x00107088 File Offset: 0x00105288
			public unsafe void OnUpdate(float iDeltaTime, Jormungandr iOwner)
			{
				if (iOwner.mTarget != null)
				{
					float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
					Vector3 position = iOwner.mTarget.Position;
					Vector3 position2 = iOwner.mDamageZone.Position;
					Vector3 vector = position;
					position2.Y = (vector.Y = 0f);
					position = iOwner.mTarget.Position;
					Vector3 forward;
					Vector3.Subtract(ref vector, ref position2, out forward);
					float num2 = forward.Length();
					if (num2 > 1E-45f)
					{
						Vector3.Divide(ref forward, num2, out forward);
					}
					else
					{
						forward = iOwner.mOrientation.Forward;
					}
					iOwner.Turn(ref forward, 1f, iDeltaTime);
					if (!iOwner.mAnimationController.CrossFadeEnabled)
					{
						this.mSpitTimer -= iDeltaTime;
						Vector3 position3 = iOwner.mDamageZone.Position;
						if ((double)num >= 0.375 & !this.mHasSprayed)
						{
							EffectManager.Instance.StartEffect(Jormungandr.JORMUNGANDREFFECTS[3], ref position3, ref iOwner.mDirection, out this.mSprayRef);
							AudioManager.Instance.PlayCue(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[6]);
							this.mHasSprayed = true;
						}
						else
						{
							EffectManager.Instance.UpdatePositionDirection(ref this.mSprayRef, ref position3, ref iOwner.mDirection);
						}
						if (num >= 0.38f & this.mSpitTimer <= 0f & this.mNumProjectiles < 4)
						{
							this.mNumProjectiles++;
							this.mSpitTimer += 0.5f;
							if (NetworkManager.Instance.State != NetworkState.Client)
							{
								Vector3.Multiply(ref forward, 0.5f, out forward);
								Vector3.Add(ref position3, ref forward, out position3);
								MissileEntity instance = MissileEntity.GetInstance(iOwner.mPlayState);
								Vector3 gravity = PhysicsSystem.CurrentPhysicsSystem.Gravity;
								float num3 = position3.Y - position.Y;
								num3 /= gravity.Y * -0.5f;
								num3 = (float)Math.Sqrt((double)num3);
								Vector3 velocity = default(Vector3);
								Vector3.Subtract(ref position, ref position3, out velocity);
								Vector3 vector2;
								Vector3.Multiply(ref gravity, num3 * num3, out vector2);
								Vector3.Divide(ref vector2, 2f, out vector2);
								Vector3.Subtract(ref velocity, ref vector2, out velocity);
								Vector3.Divide(ref velocity, num3, out velocity);
								instance.Initialize(iOwner.mDamageZone, 0.25f, ref position3, ref velocity, null, iOwner.mSpitConditions, false);
								iOwner.mPlayState.EntityManager.AddEntity(instance);
								if (NetworkManager.Instance.State == NetworkState.Server)
								{
									Jormungandr.SpitMessage spitMessage;
									spitMessage.Handle = instance.Handle;
									spitMessage.Position = position3;
									spitMessage.Velocity = velocity;
									BossFight.Instance.SendMessage<Jormungandr.SpitMessage>(iOwner, 3, (void*)(&spitMessage), true);
								}
							}
						}
					}
				}
				if (iOwner.mAnimationController.HasFinished && !iOwner.mAnimationController.CrossFadeEnabled && NetworkManager.Instance.State != NetworkState.Client)
				{
					if (NetworkManager.Instance.State == NetworkState.Server)
					{
						Jormungandr.ChangeStateMessage changeStateMessage;
						changeStateMessage.NewState = Jormungandr.State.Risen;
						changeStateMessage.Animation = Jormungandr.Animations.Idle;
						changeStateMessage.AnimationBlendTime = 0.5f;
						changeStateMessage.AnimationLooped = true;
						BossFight.Instance.SendMessage<Jormungandr.ChangeStateMessage>(iOwner, 1, (void*)(&changeStateMessage), true);
					}
					iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[6], 0.5f, true);
					iOwner.ChangeState(Jormungandr.RisenState.Instance);
				}
			}

			// Token: 0x06002473 RID: 9331 RVA: 0x001073D6 File Offset: 0x001055D6
			public void OnExit(Jormungandr iOwner)
			{
				iOwner.SelectTarget(Jormungandr.TargettingType.Random);
				iOwner.mAnimationController.Speed = 1f;
			}

			// Token: 0x04002781 RID: 10113
			private static Jormungandr.SpitState mSingelton;

			// Token: 0x04002782 RID: 10114
			private static volatile object mSingeltonLock = new object();

			// Token: 0x04002783 RID: 10115
			private float mSpitTimer;

			// Token: 0x04002784 RID: 10116
			private int mNumProjectiles;

			// Token: 0x04002785 RID: 10117
			private bool mHasSprayed;

			// Token: 0x04002786 RID: 10118
			private VisualEffectReference mSprayRef;
		}

		// Token: 0x020004B7 RID: 1207
		protected class BiteState : IBossState<Jormungandr>
		{
			// Token: 0x17000894 RID: 2196
			// (get) Token: 0x06002475 RID: 9333 RVA: 0x00107400 File Offset: 0x00105600
			public static Jormungandr.BiteState Instance
			{
				get
				{
					if (Jormungandr.BiteState.mSingelton == null)
					{
						lock (Jormungandr.BiteState.mSingeltonLock)
						{
							if (Jormungandr.BiteState.mSingelton == null)
							{
								Jormungandr.BiteState.mSingelton = new Jormungandr.BiteState();
							}
						}
					}
					return Jormungandr.BiteState.mSingelton;
				}
			}

			// Token: 0x06002476 RID: 9334 RVA: 0x00107454 File Offset: 0x00105654
			private BiteState()
			{
			}

			// Token: 0x06002477 RID: 9335 RVA: 0x0010745C File Offset: 0x0010565C
			public void OnEnter(Jormungandr iOwner)
			{
				this.mHasPlayedSound = false;
				iOwner.mAttackCount--;
				iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[9], 0.15f, false);
				iOwner.mDamageState = Jormungandr.DamageState.Bite;
				iOwner.mHitlist.Clear();
				AudioManager.Instance.PlayCue(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[10]);
				if (iOwner.mTarget == null)
				{
					iOwner.SelectTarget(Jormungandr.TargettingType.PlayerStrain);
				}
			}

			// Token: 0x06002478 RID: 9336 RVA: 0x001074D0 File Offset: 0x001056D0
			public unsafe void OnUpdate(float iDeltaTime, Jormungandr iOwner)
			{
				if (iOwner.mTarget != null)
				{
					Vector3 position = iOwner.mTarget.Position;
					Vector3 backward;
					Vector3.Subtract(ref position, ref iOwner.mPosition, out backward);
					float num = backward.Length();
					backward.Y = 0f;
					if (num > 1E-45f)
					{
						Vector3.Divide(ref backward, num, out backward);
					}
					else
					{
						backward = Vector3.Backward;
					}
					iOwner.Turn(ref backward, 1.5f, iDeltaTime);
				}
				if (!iOwner.mAnimationController.CrossFadeEnabled && iOwner.mAnimationController.Time / iOwner.mAnimationClips[9].Duration >= 0.4f && !this.mHasPlayedSound)
				{
					float num2 = (float)Jormungandr.mRandom.NextDouble() * 0.6f * (1f - iOwner.mHitPoints / iOwner.MaxHitPoints);
					iOwner.mAnimationController.Speed = 1f + num2;
					AudioManager.Instance.PlayCue(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[7]);
					this.mHasPlayedSound = true;
				}
				if (iOwner.mAnimationController.HasFinished && !iOwner.mAnimationController.CrossFadeEnabled && NetworkManager.Instance.State != NetworkState.Client)
				{
					if (NetworkManager.Instance.State == NetworkState.Server)
					{
						Jormungandr.ChangeStateMessage changeStateMessage;
						changeStateMessage.NewState = Jormungandr.State.Risen;
						changeStateMessage.Animation = Jormungandr.Animations.Idle;
						changeStateMessage.AnimationBlendTime = 0.5f;
						changeStateMessage.AnimationLooped = true;
						BossFight.Instance.SendMessage<Jormungandr.ChangeStateMessage>(iOwner, 1, (void*)(&changeStateMessage), true);
					}
					iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[6], 0.5f, true);
					iOwner.ChangeState(Jormungandr.RisenState.Instance);
				}
			}

			// Token: 0x06002479 RID: 9337 RVA: 0x00107651 File Offset: 0x00105851
			public void OnExit(Jormungandr iOwner)
			{
				iOwner.mDamageState = Jormungandr.DamageState.None;
				iOwner.mAnimationController.Speed = 1f;
			}

			// Token: 0x04002787 RID: 10119
			private static Jormungandr.BiteState mSingelton;

			// Token: 0x04002788 RID: 10120
			private static volatile object mSingeltonLock = new object();

			// Token: 0x04002789 RID: 10121
			private bool mHasPlayedSound;
		}

		// Token: 0x020004B8 RID: 1208
		protected class HitRisenState : IBossState<Jormungandr>
		{
			// Token: 0x17000895 RID: 2197
			// (get) Token: 0x0600247B RID: 9339 RVA: 0x00107678 File Offset: 0x00105878
			public static Jormungandr.HitRisenState Instance
			{
				get
				{
					if (Jormungandr.HitRisenState.mSingelton == null)
					{
						lock (Jormungandr.HitRisenState.mSingeltonLock)
						{
							if (Jormungandr.HitRisenState.mSingelton == null)
							{
								Jormungandr.HitRisenState.mSingelton = new Jormungandr.HitRisenState();
							}
						}
					}
					return Jormungandr.HitRisenState.mSingelton;
				}
			}

			// Token: 0x0600247C RID: 9340 RVA: 0x001076CC File Offset: 0x001058CC
			private HitRisenState()
			{
			}

			// Token: 0x0600247D RID: 9341 RVA: 0x001076D4 File Offset: 0x001058D4
			public void OnEnter(Jormungandr iOwner)
			{
				iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[10], 0.5f, false);
				if (iOwner.mAnimationController.Time > 1.1f)
				{
					iOwner.mAnimationController.Stop();
				}
				AudioManager.Instance.PlayCue(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[8]);
			}

			// Token: 0x0600247E RID: 9342 RVA: 0x0010772C File Offset: 0x0010592C
			public unsafe void OnUpdate(float iDeltaTime, Jormungandr iOwner)
			{
				if (iOwner.mAnimationController.HasFinished && !iOwner.mAnimationController.CrossFadeEnabled && NetworkManager.Instance.State != NetworkState.Client)
				{
					if (NetworkManager.Instance.State == NetworkState.Server)
					{
						Jormungandr.ChangeStateMessage changeStateMessage;
						changeStateMessage.NewState = Jormungandr.State.Risen;
						changeStateMessage.AnimationBlendTime = 0.6f;
						changeStateMessage.Animation = Jormungandr.Animations.Submerge;
						changeStateMessage.AnimationLooped = false;
						BossFight.Instance.SendMessage<Jormungandr.ChangeStateMessage>(iOwner, 1, (void*)(&changeStateMessage), true);
					}
					iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[5], 0.6f, false);
					iOwner.ChangeState(Jormungandr.RisenState.Instance);
				}
			}

			// Token: 0x0600247F RID: 9343 RVA: 0x001077C5 File Offset: 0x001059C5
			public void OnExit(Jormungandr iOwner)
			{
				iOwner.mIsHit = false;
			}

			// Token: 0x0400278A RID: 10122
			private static Jormungandr.HitRisenState mSingelton;

			// Token: 0x0400278B RID: 10123
			private static volatile object mSingeltonLock = new object();
		}

		// Token: 0x020004B9 RID: 1209
		protected class HitLeapState : IBossState<Jormungandr>
		{
			// Token: 0x17000896 RID: 2198
			// (get) Token: 0x06002481 RID: 9345 RVA: 0x001077DC File Offset: 0x001059DC
			public static Jormungandr.HitLeapState Instance
			{
				get
				{
					if (Jormungandr.HitLeapState.mSingelton == null)
					{
						lock (Jormungandr.HitLeapState.mSingeltonLock)
						{
							if (Jormungandr.HitLeapState.mSingelton == null)
							{
								Jormungandr.HitLeapState.mSingelton = new Jormungandr.HitLeapState();
							}
						}
					}
					return Jormungandr.HitLeapState.mSingelton;
				}
			}

			// Token: 0x06002482 RID: 9346 RVA: 0x00107830 File Offset: 0x00105A30
			private HitLeapState()
			{
			}

			// Token: 0x06002483 RID: 9347 RVA: 0x00107838 File Offset: 0x00105A38
			public void OnEnter(Jormungandr iOwner)
			{
				iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[3], 0.2f, false);
				AudioManager.Instance.PlayCue(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[8]);
			}

			// Token: 0x06002484 RID: 9348 RVA: 0x00107868 File Offset: 0x00105A68
			public unsafe void OnUpdate(float iDeltaTime, Jormungandr iOwner)
			{
				if (!iOwner.mAnimationController.CrossFadeEnabled && iOwner.mAnimationController.Time / iOwner.mAnimationClips[3].Duration > 0.666f)
				{
					iOwner.mIdleTimer -= iDeltaTime;
					float degrees = 275f / iOwner.mAnimationController.AnimationClip.Duration * iDeltaTime;
					Matrix matrix;
					Matrix.CreateRotationX(MathHelper.ToRadians(degrees), out matrix);
					Matrix.Multiply(ref matrix, ref iOwner.mOrientation, out iOwner.mOrientation);
					if (iOwner.mIdleTimer <= Jormungandr.WARNINGTIME && NetworkManager.Instance.State != NetworkState.Client)
					{
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							Jormungandr.ChangeStateMessage changeStateMessage;
							changeStateMessage.NewState = Jormungandr.State.Underground;
							changeStateMessage.Animation = Jormungandr.Animations.Invalid;
							BossFight.Instance.SendMessage<Jormungandr.ChangeStateMessage>(iOwner, 1, (void*)(&changeStateMessage), true);
						}
						iOwner.ChangeState(Jormungandr.UndergroundState.Instance);
					}
				}
			}

			// Token: 0x06002485 RID: 9349 RVA: 0x00107941 File Offset: 0x00105B41
			public void OnExit(Jormungandr iOwner)
			{
				iOwner.mIsHit = false;
				iOwner.mAnimationController.Speed = 1f;
			}

			// Token: 0x0400278C RID: 10124
			private static Jormungandr.HitLeapState mSingelton;

			// Token: 0x0400278D RID: 10125
			private static volatile object mSingeltonLock = new object();
		}

		// Token: 0x020004BA RID: 1210
		protected class DeadState : IBossState<Jormungandr>
		{
			// Token: 0x17000897 RID: 2199
			// (get) Token: 0x06002487 RID: 9351 RVA: 0x00107968 File Offset: 0x00105B68
			public static Jormungandr.DeadState Instance
			{
				get
				{
					if (Jormungandr.DeadState.mSingelton == null)
					{
						lock (Jormungandr.DeadState.mSingeltonLock)
						{
							if (Jormungandr.DeadState.mSingelton == null)
							{
								Jormungandr.DeadState.mSingelton = new Jormungandr.DeadState();
							}
						}
					}
					return Jormungandr.DeadState.mSingelton;
				}
			}

			// Token: 0x06002488 RID: 9352 RVA: 0x001079BC File Offset: 0x00105BBC
			private DeadState()
			{
			}

			// Token: 0x06002489 RID: 9353 RVA: 0x001079C4 File Offset: 0x00105BC4
			public void OnEnter(Jormungandr iOwner)
			{
				iOwner.mDraw = true;
				iOwner.mUpdateAnimation = true;
				Vector3 vector;
				iOwner.mOrientation.Decompose(out vector, out iOwner.mSavedRotation, out iOwner.mSavedPosition);
				iOwner.mAnimationController.StartClip(iOwner.mAnimationClips[(int)iOwner.mDeathAnimation], false);
				iOwner.mDamageState = Jormungandr.DamageState.None;
				AudioManager.Instance.PlayCue(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[0]);
				EffectManager.Instance.Stop(ref iOwner.mEarthSplashReference);
				EffectManager.Instance.Stop(ref iOwner.mEarthTremorReference);
				iOwner.mIdleTimer = 0f;
			}

			// Token: 0x0600248A RID: 9354 RVA: 0x00107A58 File Offset: 0x00105C58
			public void OnUpdate(float iDeltaTime, Jormungandr iOwner)
			{
				Matrix mOrientation;
				Jormungandr.CreateTransform(ref iOwner.mPosition, ref iOwner.mDirection, out mOrientation);
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					iOwner.mOrientation = mOrientation;
				}
				else
				{
					Vector3 vector;
					Quaternion quaternion;
					Vector3 translation;
					mOrientation.Decompose(out vector, out quaternion, out translation);
					float amount = iOwner.mAnimationController.CrossFadeTime / iOwner.mAnimationController.CrossFadeDuration;
					Quaternion.Lerp(ref iOwner.mSavedRotation, ref quaternion, amount, out quaternion);
					Vector3.Lerp(ref iOwner.mSavedPosition, ref translation, amount, out translation);
					Matrix.CreateFromQuaternion(ref quaternion, out iOwner.mOrientation);
					iOwner.mOrientation.Translation = translation;
				}
				if (iOwner.mAnimationController.HasFinished && !iOwner.mAnimationController.CrossFadeEnabled)
				{
					iOwner.mDead = true;
				}
			}

			// Token: 0x0600248B RID: 9355 RVA: 0x00107B11 File Offset: 0x00105D11
			public void OnExit(Jormungandr iOwner)
			{
			}

			// Token: 0x0400278E RID: 10126
			private static Jormungandr.DeadState mSingelton;

			// Token: 0x0400278F RID: 10127
			private static volatile object mSingeltonLock = new object();
		}

		// Token: 0x020004BB RID: 1211
		private enum MessageType : ushort
		{
			// Token: 0x04002791 RID: 10129
			Update,
			// Token: 0x04002792 RID: 10130
			ChangeState,
			// Token: 0x04002793 RID: 10131
			ChangeTarget,
			// Token: 0x04002794 RID: 10132
			Spit
		}

		// Token: 0x020004BC RID: 1212
		internal struct UpdateMessage
		{
			// Token: 0x04002795 RID: 10133
			public const ushort TYPE = 0;

			// Token: 0x04002796 RID: 10134
			public Vector3 Position;

			// Token: 0x04002797 RID: 10135
			public Vector3 Direction;

			// Token: 0x04002798 RID: 10136
			public Matrix Orientation;

			// Token: 0x04002799 RID: 10137
			public byte Animation;

			// Token: 0x0400279A RID: 10138
			public float AnimationTime;

			// Token: 0x0400279B RID: 10139
			public float Hitpoints;
		}

		// Token: 0x020004BD RID: 1213
		internal struct ChangeStateMessage
		{
			// Token: 0x0400279C RID: 10140
			public const ushort TYPE = 1;

			// Token: 0x0400279D RID: 10141
			public float AnimationBlendTime;

			// Token: 0x0400279E RID: 10142
			public bool AnimationLooped;

			// Token: 0x0400279F RID: 10143
			public Jormungandr.State NewState;

			// Token: 0x040027A0 RID: 10144
			public Jormungandr.Animations Animation;
		}

		// Token: 0x020004BE RID: 1214
		internal struct ChangeTargetMessage
		{
			// Token: 0x040027A1 RID: 10145
			public const ushort TYPE = 2;

			// Token: 0x040027A2 RID: 10146
			public ushort Target;
		}

		// Token: 0x020004BF RID: 1215
		internal struct SpitMessage
		{
			// Token: 0x040027A3 RID: 10147
			public const ushort TYPE = 3;

			// Token: 0x040027A4 RID: 10148
			public ushort Handle;

			// Token: 0x040027A5 RID: 10149
			public Vector3 Velocity;

			// Token: 0x040027A6 RID: 10150
			public Vector3 Position;
		}

		// Token: 0x020004C0 RID: 1216
		public enum Animations : byte
		{
			// Token: 0x040027A8 RID: 10152
			Invalid,
			// Token: 0x040027A9 RID: 10153
			Intro,
			// Token: 0x040027AA RID: 10154
			Leap,
			// Token: 0x040027AB RID: 10155
			Leap_Hit,
			// Token: 0x040027AC RID: 10156
			Rise,
			// Token: 0x040027AD RID: 10157
			Submerge,
			// Token: 0x040027AE RID: 10158
			Idle,
			// Token: 0x040027AF RID: 10159
			Spit,
			// Token: 0x040027B0 RID: 10160
			Venom,
			// Token: 0x040027B1 RID: 10161
			Bite,
			// Token: 0x040027B2 RID: 10162
			Risen_Hit,
			// Token: 0x040027B3 RID: 10163
			Die_Below,
			// Token: 0x040027B4 RID: 10164
			Die_Above,
			// Token: 0x040027B5 RID: 10165
			NrOfAnimations
		}

		// Token: 0x020004C1 RID: 1217
		public enum DamageState
		{
			// Token: 0x040027B7 RID: 10167
			None,
			// Token: 0x040027B8 RID: 10168
			Leap,
			// Token: 0x040027B9 RID: 10169
			Emerge,
			// Token: 0x040027BA RID: 10170
			Bite
		}

		// Token: 0x020004C2 RID: 1218
		public enum TargettingType
		{
			// Token: 0x040027BC RID: 10172
			Random,
			// Token: 0x040027BD RID: 10173
			Distance,
			// Token: 0x040027BE RID: 10174
			Angle,
			// Token: 0x040027BF RID: 10175
			PlayerStrain
		}

		// Token: 0x020004C3 RID: 1219
		public enum Effects
		{
			// Token: 0x040027C1 RID: 10177
			Splash,
			// Token: 0x040027C2 RID: 10178
			Tremor,
			// Token: 0x040027C3 RID: 10179
			SplashTremor,
			// Token: 0x040027C4 RID: 10180
			SpitSpray,
			// Token: 0x040027C5 RID: 10181
			SpitSplash,
			// Token: 0x040027C6 RID: 10182
			SpitProjectile
		}

		// Token: 0x020004C4 RID: 1220
		private enum Sounds
		{
			// Token: 0x040027C8 RID: 10184
			Death,
			// Token: 0x040027C9 RID: 10185
			WakeUp,
			// Token: 0x040027CA RID: 10186
			Dive,
			// Token: 0x040027CB RID: 10187
			Unburrow,
			// Token: 0x040027CC RID: 10188
			Leap,
			// Token: 0x040027CD RID: 10189
			Rise,
			// Token: 0x040027CE RID: 10190
			Spit,
			// Token: 0x040027CF RID: 10191
			Bite,
			// Token: 0x040027D0 RID: 10192
			Pain,
			// Token: 0x040027D1 RID: 10193
			PreSpit,
			// Token: 0x040027D2 RID: 10194
			PreBite,
			// Token: 0x040027D3 RID: 10195
			LeapDive,
			// Token: 0x040027D4 RID: 10196
			HitObject,
			// Token: 0x040027D5 RID: 10197
			Whip,
			// Token: 0x040027D6 RID: 10198
			Prowl
		}
	}
}
