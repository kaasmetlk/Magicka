using System;
using System.Collections.Generic;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using JigLibX.Physics;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using PolygonHead;
using PolygonHead.Effects;
using XNAnimation;
using XNAnimation.Controllers;

namespace Magicka.GameLogic.Entities.Bosses
{
	// Token: 0x02000304 RID: 772
	public class Tentacle : Entity, IStatusEffected, IDamageable
	{
		// Token: 0x17000602 RID: 1538
		// (get) Token: 0x060017AF RID: 6063 RVA: 0x0009BFAE File Offset: 0x0009A1AE
		private float WaterYPos
		{
			get
			{
				if (this.mOwner != null)
				{
					return this.mOwner.WaterYpos;
				}
				return -2f;
			}
		}

		// Token: 0x060017B0 RID: 6064 RVA: 0x0009BFCC File Offset: 0x0009A1CC
		public Tentacle(Cthulhu iOwner, byte iID, int iDamageZoneIndex, PlayState iPlayState) : base(iPlayState)
		{
			this.mOwner = iOwner;
			this.mID = iID;
			this.mPlayState = iPlayState;
			if (!(RenderManager.Instance.GetEffect(SkinnedModelDeferredNormalMappedEffect.TYPEHASH) is SkinnedModelDeferredNormalMappedEffect))
			{
				SkinnedModelDeferredNormalMappedEffect.DefaultEffectPool = RenderManager.Instance.GlobalDummyEffect.EffectPool;
				SkinnedModelDeferredNormalMappedEffect iEffect = new SkinnedModelDeferredNormalMappedEffect(Game.Instance.GraphicsDevice, SkinnedModelDeferredNormalMappedEffect.DefaultEffectPool);
				RenderManager.Instance.RegisterEffect(iEffect);
			}
			SkinnedModel skinnedModel = null;
			SkinnedModel skinnedModel2 = null;
			lock (Game.Instance.GraphicsDevice)
			{
				skinnedModel = iPlayState.Content.Load<SkinnedModel>("Models/Bosses/Cthulhu/Tentacle_mesh");
				skinnedModel2 = iPlayState.Content.Load<SkinnedModel>("Models/Bosses/Cthulhu/Tentacle_animations");
			}
			this.mAnimationController = new AnimationController();
			this.mAnimationController.Skeleton = skinnedModel2.SkeletonBones;
			this.mAnimations = new AnimationClip[9];
			this.mAnimations[0] = skinnedModel2.AnimationClips["idle"];
			this.mAnimations[6] = skinnedModel2.AnimationClips["grip"];
			this.mAnimations[7] = skinnedModel2.AnimationClips["grip_success"];
			this.mAnimations[1] = skinnedModel2.AnimationClips["squeeze"];
			this.mAnimations[2] = skinnedModel2.AnimationClips["crush"];
			this.mAnimations[8] = skinnedModel2.AnimationClips["let_go"];
			this.mAnimations[5] = skinnedModel2.AnimationClips["die"];
			this.mAnimations[3] = skinnedModel2.AnimationClips["emerge"];
			this.mAnimations[4] = skinnedModel2.AnimationClips["submerge0"];
			this.mRenderData = new Tentacle.RenderData[3];
			for (int i = 0; i < this.mRenderData.Length; i++)
			{
				this.mRenderData[i] = new Tentacle.RenderData(skinnedModel2.SkeletonBones.Count, skinnedModel.Model.Meshes[0], skinnedModel.Model.Meshes[0].MeshParts[0]);
				this.mRenderData[i].BoundingSphere.Radius = skinnedModel.Model.Meshes[0].BoundingSphere.Radius;
			}
			Matrix matrix;
			Matrix.CreateRotationY(3.1415927f, out matrix);
			for (int j = 0; j < skinnedModel2.SkeletonBones.Count; j++)
			{
				SkinnedModelBone skinnedModelBone = skinnedModel2.SkeletonBones[j];
				if (skinnedModelBone.Name.Equals("joint24", StringComparison.OrdinalIgnoreCase))
				{
					this.mGrabAttachIndex = (int)skinnedModelBone.Index;
					this.mGrabAttachBindPose = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mGrabAttachBindPose, ref matrix, out this.mGrabAttachBindPose);
					Matrix.Invert(ref this.mGrabAttachBindPose, out this.mGrabAttachBindPose);
				}
				else if (skinnedModelBone.Name.Equals("joint12", StringComparison.OrdinalIgnoreCase))
				{
					this.mSpineIndex[0] = (int)skinnedModelBone.Index;
					this.mSpineBindPose[0] = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mSpineBindPose[0], ref matrix, out this.mSpineBindPose[0]);
					Matrix.Invert(ref this.mSpineBindPose[0], out this.mSpineBindPose[0]);
				}
				else if (skinnedModelBone.Name.Equals("joint2", StringComparison.OrdinalIgnoreCase))
				{
					this.mSpineIndex[1] = (int)skinnedModelBone.Index;
					this.mSpineBindPose[1] = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mSpineBindPose[1], ref matrix, out this.mSpineBindPose[1]);
					Matrix.Invert(ref this.mSpineBindPose[1], out this.mSpineBindPose[1]);
				}
				else if (skinnedModelBone.Name.Equals("joint3", StringComparison.OrdinalIgnoreCase))
				{
					this.mSpineIndex[2] = (int)skinnedModelBone.Index;
					this.mSpineBindPose[2] = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mSpineBindPose[2], ref matrix, out this.mSpineBindPose[2]);
					Matrix.Invert(ref this.mSpineBindPose[2], out this.mSpineBindPose[2]);
				}
				else if (skinnedModelBone.Name.Equals("joint4", StringComparison.OrdinalIgnoreCase))
				{
					this.mSpineIndex[3] = (int)skinnedModelBone.Index;
					this.mSpineBindPose[3] = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mSpineBindPose[3], ref matrix, out this.mSpineBindPose[3]);
					Matrix.Invert(ref this.mSpineBindPose[3], out this.mSpineBindPose[3]);
				}
				else if (skinnedModelBone.Name.Equals("joint5", StringComparison.OrdinalIgnoreCase))
				{
					this.mSpineIndex[4] = (int)skinnedModelBone.Index;
					this.mSpineBindPose[4] = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mSpineBindPose[4], ref matrix, out this.mSpineBindPose[4]);
					Matrix.Invert(ref this.mSpineBindPose[4], out this.mSpineBindPose[4]);
				}
				else if (skinnedModelBone.Name.Equals("joint6", StringComparison.OrdinalIgnoreCase))
				{
					this.mSpineIndex[5] = (int)skinnedModelBone.Index;
					this.mSpineBindPose[5] = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mSpineBindPose[5], ref matrix, out this.mSpineBindPose[5]);
					Matrix.Invert(ref this.mSpineBindPose[5], out this.mSpineBindPose[5]);
				}
				else if (skinnedModelBone.Name.Equals("joint7", StringComparison.OrdinalIgnoreCase))
				{
					this.mSpineIndex[6] = (int)skinnedModelBone.Index;
					this.mSpineBindPose[6] = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mSpineBindPose[6], ref matrix, out this.mSpineBindPose[6]);
					Matrix.Invert(ref this.mSpineBindPose[6], out this.mSpineBindPose[6]);
				}
				else if (skinnedModelBone.Name.Equals("joint8", StringComparison.OrdinalIgnoreCase))
				{
					this.mSpineIndex[7] = (int)skinnedModelBone.Index;
					this.mSpineBindPose[7] = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mSpineBindPose[7], ref matrix, out this.mSpineBindPose[7]);
					Matrix.Invert(ref this.mSpineBindPose[7], out this.mSpineBindPose[7]);
				}
				else if (skinnedModelBone.Name.Equals("joint9", StringComparison.OrdinalIgnoreCase))
				{
					this.mSpineIndex[8] = (int)skinnedModelBone.Index;
					this.mSpineBindPose[8] = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mSpineBindPose[8], ref matrix, out this.mSpineBindPose[8]);
					Matrix.Invert(ref this.mSpineBindPose[8], out this.mSpineBindPose[8]);
				}
				else if (skinnedModelBone.Name.Equals("joint10", StringComparison.OrdinalIgnoreCase))
				{
					this.mSpineIndex[9] = (int)skinnedModelBone.Index;
					this.mSpineBindPose[9] = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mSpineBindPose[9], ref matrix, out this.mSpineBindPose[9]);
					Matrix.Invert(ref this.mSpineBindPose[9], out this.mSpineBindPose[9]);
				}
				else if (skinnedModelBone.Name.Equals("joint11", StringComparison.OrdinalIgnoreCase))
				{
					this.mSpineIndex[10] = (int)skinnedModelBone.Index;
					this.mSpineBindPose[10] = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mSpineBindPose[10], ref matrix, out this.mSpineBindPose[10]);
					Matrix.Invert(ref this.mSpineBindPose[10], out this.mSpineBindPose[10]);
				}
			}
			this.mRadius = 0.8f;
			Primitive[] array = new Primitive[this.mSpineBindPose.Length - 1];
			for (int k = 0; k < this.mSpineBindPose.Length - 1; k++)
			{
				Vector3 translation = this.mSpineBindPose[k].Translation;
				Vector3 translation2 = this.mSpineBindPose[k + 1].Translation;
				float length;
				Vector3.Distance(ref translation, ref translation2, out length);
				array[k] = new Capsule(Vector3.Zero, Matrix.Identity, this.mRadius, length);
			}
			this.mTentacleLength = Vector3.Distance(this.mSpineBindPose[0].Translation, this.mSpineBindPose[this.mSpineBindPose.Length - 1].Translation);
			this.mBody = new Body();
			this.mCollision = new CollisionSkin(this.mBody);
			for (int l = 0; l < array.Length; l++)
			{
				this.mCollision.AddPrimitive(array[l], 1, new MaterialProperties(0f, 0f, 0f));
			}
			this.mCollision.ApplyLocalTransform(Transform.Identity);
			this.mBody.CollisionSkin = this.mCollision;
			this.mBody.Immovable = true;
			this.mBody.CollisionSkin.callbackFn += this.OnCollision;
			this.mBody.Tag = this;
			this.mBody.MoveTo(default(Vector3), Matrix.Identity);
			this.mMaxHitPoints = 800f;
			for (int m = 0; m < 11; m++)
			{
				Elements elements = Spell.ElementFromIndex(m);
				this.mResistances[m].Modifier = 0f;
				this.mResistances[m].Multiplier = 1f;
				this.mResistances[m].ResistanceAgainst = elements;
				Elements elements2 = elements;
				if (elements2 != Elements.Water)
				{
					if (elements2 == Elements.Poison)
					{
						this.mResistances[m].Multiplier = 0f;
					}
				}
				else
				{
					this.mResistances[m].Multiplier = 0f;
				}
			}
			this.mHitList = new HitList(8);
			this.mSqueezeDamage = new Damage(AttackProperties.Damage | AttackProperties.Bleed, Elements.Earth, 150f, 1f);
			this.mCrushDamage = new Damage(AttackProperties.Damage, Elements.Earth, 500f, 1f);
			this.mSwipeDamage = new Damage(AttackProperties.Damage, Elements.Earth, 150f, 1f);
			Vector3 vector = new Vector3(0f, (float)((int)this.mID * 1000), 0f);
			Matrix identity = Matrix.Identity;
			this.mBody.MoveTo(ref vector, ref identity);
		}

		// Token: 0x060017B1 RID: 6065 RVA: 0x0009CB88 File Offset: 0x0009AD88
		private bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
		{
			return iSkin1.Owner == null || !(iSkin1.Owner.Tag is Character);
		}

		// Token: 0x17000603 RID: 1539
		// (get) Token: 0x060017B2 RID: 6066 RVA: 0x0009CBA8 File Offset: 0x0009ADA8
		private Vector3 NotifierTextPostion
		{
			get
			{
				Vector3 translation = this.mTransform.Translation;
				translation.Y += 3f;
				return translation;
			}
		}

		// Token: 0x17000604 RID: 1540
		// (get) Token: 0x060017B3 RID: 6067 RVA: 0x0009CBD5 File Offset: 0x0009ADD5
		internal int ID
		{
			get
			{
				return (int)this.mID;
			}
		}

		// Token: 0x060017B4 RID: 6068 RVA: 0x0009CBDD File Offset: 0x0009ADDD
		private void StartClip(Tentacle.Animations anim, bool loop)
		{
			this.mAnimationController.StartClip(this.mAnimations[(int)anim], loop);
		}

		// Token: 0x060017B5 RID: 6069 RVA: 0x0009CBF3 File Offset: 0x0009ADF3
		private void StopClip()
		{
			this.mAnimationController.Stop();
		}

		// Token: 0x060017B6 RID: 6070 RVA: 0x0009CC00 File Offset: 0x0009AE00
		private void CrossFade(Tentacle.Animations anim, float time, bool loop)
		{
			this.mAnimationController.CrossFade(this.mAnimations[(int)anim], time, loop);
		}

		// Token: 0x060017B7 RID: 6071 RVA: 0x0009CC17 File Offset: 0x0009AE17
		private bool IsPlaying(Tentacle.Animations anim)
		{
			return this.mAnimationController.AnimationClip == this.mAnimations[(int)anim];
		}

		// Token: 0x17000605 RID: 1541
		// (get) Token: 0x060017B8 RID: 6072 RVA: 0x0009CC2E File Offset: 0x0009AE2E
		private bool AnimationHasFinished
		{
			get
			{
				return !this.mAnimationController.CrossFadeEnabled && this.mAnimationController.HasFinished;
			}
		}

		// Token: 0x060017B9 RID: 6073 RVA: 0x0009CC4A File Offset: 0x0009AE4A
		public new void Initialize()
		{
			base.Initialize();
			this.mDead = true;
		}

		// Token: 0x060017BA RID: 6074 RVA: 0x0009CC5C File Offset: 0x0009AE5C
		internal void Start(Matrix iTransform)
		{
			this.mTransform = iTransform;
			this.mOriginalForwardDirection = this.mTransform.Forward;
			this.mHitList.Clear();
			this.mHitPoints = this.mMaxHitPoints;
			this.mDead = false;
			if (this.mCurrentState == Tentacle.States.None)
			{
				this.mPreviousState = Tentacle.States.Idle;
				this.mCurrentState = Tentacle.States.Emerge;
				this.mStates[(int)this.mCurrentState].OnEnter(this);
				return;
			}
			this.ChangeState(Tentacle.States.Emerge);
		}

		// Token: 0x060017BB RID: 6075 RVA: 0x0009CCD0 File Offset: 0x0009AED0
		internal unsafe void ChangeState(Tentacle.States iState)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				if (this.mCurrentState != Tentacle.States.None)
				{
					this.mStates[(int)this.mCurrentState].OnExit(this);
				}
				this.mPreviousState = this.mCurrentState;
				this.mCurrentState = iState;
				this.mStates[(int)this.mCurrentState].OnEnter(this);
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					Cthulhu.TentacleChangeStateMessage tentacleChangeStateMessage;
					tentacleChangeStateMessage.NewState = iState;
					tentacleChangeStateMessage.TentacleIndex = this.mID;
					BossFight.Instance.SendMessage<Cthulhu.TentacleChangeStateMessage>(this.mOwner, 12, (void*)(&tentacleChangeStateMessage), true);
				}
			}
		}

		// Token: 0x060017BC RID: 6076 RVA: 0x0009CD63 File Offset: 0x0009AF63
		internal void GrabDamageable(IDamageable iDamageable)
		{
			this.mGrabbed = iDamageable;
		}

		// Token: 0x060017BD RID: 6077 RVA: 0x0009CD6C File Offset: 0x0009AF6C
		internal void SetAimTarget(Avatar iAvatar)
		{
			this.mCrushTarget = iAvatar;
			if (this.mCrushTarget == null)
			{
				this.mSearchForTarget = false;
			}
		}

		// Token: 0x060017BE RID: 6078 RVA: 0x0009CD84 File Offset: 0x0009AF84
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			iDeltaTime /= this.mPlayState.TimeModifier * this.mPlayState.TimeMultiplier;
			base.Update(iDataChannel, iDeltaTime);
			if (this.mHitPoints <= 0f && !this.mCthulhuKilled && NetworkManager.Instance.State != NetworkState.Client && this.mCurrentState != Tentacle.States.Submerge && this.mCurrentState != Tentacle.States.Release)
			{
				if (this.mGrabbed == null)
				{
					this.ChangeState(Tentacle.States.Submerge);
				}
				else
				{
					this.ChangeState(Tentacle.States.Release);
				}
			}
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				this.mNetworkUpdateTimer -= iDeltaTime;
				if (this.mNetworkUpdateTimer <= 0f)
				{
					this.mNetworkUpdateTimer = 0.05f;
					this.NetworkUpdate();
				}
			}
			if (this.mCurrentState == Tentacle.States.None)
			{
				return;
			}
			this.UpdateDamage(iDeltaTime);
			this.UpdateStatusEffects(iDeltaTime);
			this.mTimeUntilSubmerge -= iDeltaTime;
			this.mTimeSinceLastEmerge += iDeltaTime;
			this.mHitList.Update(iDeltaTime);
			this.mStates[(int)this.mCurrentState].OnUpdate(this, iDeltaTime);
			Matrix matrix;
			Matrix.CreateScale(1f, out matrix);
			Matrix.Multiply(ref this.mTransform, ref matrix, out matrix);
			matrix.Translation = this.mTransform.Translation;
			float slowdown = this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].GetSlowdown();
			float elapsedTime = this.mOwner.StageSpeedModifier * slowdown * iDeltaTime;
			this.mAnimationController.Update(elapsedTime, ref matrix, true);
			Matrix identity = Matrix.Identity;
			Matrix.Multiply(ref this.mGrabAttachBindPose, ref this.mAnimationController.SkinnedBoneTransforms[this.mGrabAttachIndex], out this.mGrabTransform);
			Matrix.Multiply(ref this.mSpineBindPose[10], ref this.mAnimationController.SkinnedBoneTransforms[this.mSpineIndex[10]], out this.mTransformedTopJoint);
			Vector3 translation = this.mTransform.Translation;
			Vector3 zero = Vector3.Zero;
			Vector3 up = Vector3.Up;
			for (int i = 0; i < 10; i++)
			{
				Vector3 translation2 = this.mSpineBindPose[i].Translation;
				Vector3.Transform(ref translation2, ref this.mAnimationController.SkinnedBoneTransforms[this.mSpineIndex[i]], out translation2);
				Vector3 translation3 = this.mSpineBindPose[i + 1].Translation;
				Vector3.Transform(ref translation3, ref this.mAnimationController.SkinnedBoneTransforms[this.mSpineIndex[i + 1]], out translation3);
				Vector3 vector;
				Vector3.Subtract(ref translation3, ref translation2, out vector);
				vector.Normalize();
				Transform transform;
				if (Vector3.Cross(vector, up).LengthSquared() <= 1E-06f)
				{
					Vector3 right = Vector3.Right;
					Matrix.CreateWorld(ref zero, ref vector, ref right, out transform.Orientation);
				}
				else
				{
					Matrix.CreateWorld(ref zero, ref vector, ref up, out transform.Orientation);
				}
				Vector3.Subtract(ref translation3, ref translation, out transform.Position);
				this.mBody.CollisionSkin.GetPrimitiveLocal(i).SetTransform(ref transform);
			}
			this.mBody.MoveTo(ref translation, ref identity);
			if (this.mGrabbed != null)
			{
				if (this.mGrabbed.Dead)
				{
					this.mGrabbed.OverKill();
					this.mGrabbed = null;
				}
				else
				{
					Vector3 translation4 = this.mGrabTransform.Translation;
					Vector3 position = this.mGrabbed.Position;
					float num;
					Vector3.DistanceSquared(ref translation4, ref position, out num);
					Matrix identity2 = Matrix.Identity;
					identity2.Translation = default(Vector3);
					this.mGrabbed.Body.MoveTo(ref translation4, ref identity2);
				}
			}
			Tentacle.RenderData renderData = this.mRenderData[(int)iDataChannel];
			float num2 = this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].Magnitude;
			num2 *= 10f;
			num2 = Math.Min(num2, 1f);
			renderData.Colorize.X = Tentacle.ColdColor.X;
			renderData.Colorize.Y = Tentacle.ColdColor.Y;
			renderData.Colorize.Z = Tentacle.ColdColor.Z;
			renderData.Colorize.W = num2;
			this.mHitFlashTimer = Math.Max(this.mHitFlashTimer - iDeltaTime * 5f, 0f);
			renderData.BoundingSphere.Center = this.mTransform.Translation;
			renderData.Damage = 1f - this.mOwner.HitPoints / this.mOwner.MaxHitPoints;
			renderData.Flash = this.mHitFlashTimer;
			this.ApplyWetSpecularEffect(renderData, iDeltaTime);
			Vector3 vector2 = new Vector3(0.75f, 0.8f, 1f);
			Vector3 diffuseColor = renderData.Material.DiffuseColor;
			Vector3.Multiply(ref renderData.Material.DiffuseColor, ref vector2, out renderData.Material.DiffuseColor);
			Array.Copy(this.mAnimationController.SkinnedBoneTransforms, renderData.Skeleton, renderData.Skeleton.Length);
			this.mPlayState.Scene.AddRenderableObject(iDataChannel, renderData);
			renderData.Material.DiffuseColor = diffuseColor;
			this.mPlayState.Scene.AddRenderableObject(iDataChannel, this.mRenderData[(int)iDataChannel]);
			this.CheckIfCharactersAreClose(iDeltaTime);
			if (EffectManager.Instance.IsActive(ref this.mIdleEffectRef))
			{
				Vector3 translation5 = this.mTransform.Translation;
				translation5.Y = this.mOwner.WaterYpos;
				Matrix matrix2 = Matrix.CreateTranslation(translation5);
				EffectManager.Instance.UpdateOrientation(ref this.mIdleEffectRef, ref matrix2);
			}
		}

		// Token: 0x060017BF RID: 6079 RVA: 0x0009D2E8 File Offset: 0x0009B4E8
		internal void TurnOnIdleEffect()
		{
			Vector3 translation = this.mTransform.Translation;
			translation.Y = this.mOwner.WaterYpos;
			Matrix matrix = Matrix.CreateTranslation(translation);
			EffectManager.Instance.StartEffect(this.mIdleEffect, ref matrix, out this.mIdleEffectRef);
		}

		// Token: 0x060017C0 RID: 6080 RVA: 0x0009D333 File Offset: 0x0009B533
		internal void KillIdleEffect()
		{
			EffectManager.Instance.Stop(ref this.mIdleEffectRef);
		}

		// Token: 0x060017C1 RID: 6081 RVA: 0x0009D345 File Offset: 0x0009B545
		private float GetColdSlowdown()
		{
			return this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].GetSlowdown();
		}

		// Token: 0x060017C2 RID: 6082 RVA: 0x0009D360 File Offset: 0x0009B560
		private void ApplyWetSpecularEffect(Tentacle.RenderData iRenderData, float iDeltaTime)
		{
			float magnitude = this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)].Magnitude;
			if (magnitude > 0f)
			{
				if (this.mWetnessCounter < 1f)
				{
					iRenderData.Material.SpecularAmount = 8f - 6f * (1f - this.mWetnessCounter);
					iRenderData.Material.SpecularPower = 800f - 600f * (1f - this.mWetnessCounter);
					this.mWetnessCounter += iDeltaTime;
					return;
				}
				this.mWetnessCounter = 1f;
				iRenderData.Material.SpecularAmount = 8f;
				iRenderData.Material.SpecularPower = 600f;
				return;
			}
			else
			{
				if (this.mWetnessCounter > 0f)
				{
					iRenderData.Material.SpecularAmount = 8f - 6f * (1f - this.mWetnessCounter);
					iRenderData.Material.SpecularPower = 800f - 600f * (1f - this.mWetnessCounter);
					this.mWetnessCounter -= iDeltaTime;
					return;
				}
				iRenderData.Material.SpecularAmount = 2f;
				iRenderData.Material.SpecularPower = 200f;
				this.mWetnessCounter = 0f;
				return;
			}
		}

		// Token: 0x060017C3 RID: 6083 RVA: 0x0009D4AC File Offset: 0x0009B6AC
		private void CheckIfCharactersAreClose(float iDeltaTime)
		{
			this.mCheckEnemiesInRangeTimer -= iDeltaTime;
			if (this.mCheckEnemiesInRangeTimer < 0f)
			{
				bool flag = this.CheckIfCharactersAreWithinRadius(this.mTentacleLength);
				if (flag)
				{
					this.mSecondsWhileOutOfRange = 0;
				}
				else
				{
					this.mSecondsWhileOutOfRange++;
				}
				this.mCheckEnemiesInRangeTimer = 1f;
			}
		}

		// Token: 0x060017C4 RID: 6084 RVA: 0x0009D508 File Offset: 0x0009B708
		private bool CheckIfCharactersAreWithinRadius(float iRadius)
		{
			EntityManager entityManager = this.mPlayState.EntityManager;
			Vector3 translation = this.mTransform.Translation;
			List<Entity> entities = entityManager.GetEntities(translation, iRadius, true);
			bool flag = false;
			int num = 0;
			while (num < entities.Count && !flag)
			{
				Avatar avatar = entities[num] as Avatar;
				flag = (avatar != null && !avatar.IsEthereal && !avatar.IsInvisibile);
				num++;
			}
			entityManager.ReturnEntityList(entities);
			return flag;
		}

		// Token: 0x060017C5 RID: 6085 RVA: 0x0009D584 File Offset: 0x0009B784
		internal void NetworkUpdate(ref Cthulhu.TentacleUpdateMessage iMsg, float iTimeStamp)
		{
			if (iTimeStamp < this.mLastNetworkUpdate)
			{
				return;
			}
			this.mLastNetworkUpdate = iTimeStamp;
			if (this.mAnimationController.AnimationClip != this.mAnimations[(int)iMsg.Animation])
			{
				this.mAnimationController.StartClip(this.mAnimations[(int)iMsg.Animation], false);
			}
			this.mAnimationController.Time = iMsg.AnimationTime.ToSingle();
		}

		// Token: 0x060017C6 RID: 6086 RVA: 0x0009D5EB File Offset: 0x0009B7EB
		internal void NetworkChangeState(ref Cthulhu.TentacleChangeStateMessage iMsg)
		{
			if (iMsg.NewState != Tentacle.States.NR_OF_STATES)
			{
				this.mStates[(int)this.mCurrentState].OnExit(this);
				this.mCurrentState = iMsg.NewState;
				this.mStates[(int)this.mCurrentState].OnEnter(this);
			}
		}

		// Token: 0x060017C7 RID: 6087 RVA: 0x0009D62C File Offset: 0x0009B82C
		internal void NetworkSpawnPoint(ref Cthulhu.SpawnPointMessage iMsg)
		{
			if (this.mSpawnPoint != -1)
			{
				this.mSpawnPoint = iMsg.SpawnPoint;
			}
			Matrix iTransform = this.mOwner.ChangeSpawnPoint(this.mSpawnPoint, (int)base.Handle);
			this.Start(iTransform);
		}

		// Token: 0x060017C8 RID: 6088 RVA: 0x0009D670 File Offset: 0x0009B870
		private unsafe void NetworkUpdate()
		{
			NetworkServer networkServer = NetworkManager.Instance.Interface as NetworkServer;
			Cthulhu.TentacleUpdateMessage tentacleUpdateMessage = default(Cthulhu.TentacleUpdateMessage);
			tentacleUpdateMessage.Animation = 0;
			while ((int)tentacleUpdateMessage.Animation < this.mAnimations.Length && this.mAnimationController.AnimationClip != this.mAnimations[(int)tentacleUpdateMessage.Animation])
			{
				tentacleUpdateMessage.Animation += 1;
			}
			tentacleUpdateMessage.TentacleIndex = this.mID;
			for (int i = 0; i < networkServer.Connections; i++)
			{
				float num = networkServer.GetLatency(i) * 0.5f;
				Cthulhu.TentacleUpdateMessage tentacleUpdateMessage2 = tentacleUpdateMessage;
				tentacleUpdateMessage2.AnimationTime = new HalfSingle(this.mAnimationController.Time + num);
				BossFight.Instance.SendMessage<Cthulhu.TentacleUpdateMessage>(this.mOwner, 11, (void*)(&tentacleUpdateMessage2), false, i);
			}
		}

		// Token: 0x060017C9 RID: 6089 RVA: 0x0009D73A File Offset: 0x0009B93A
		internal void KillTentacle()
		{
			if (!this.mDead)
			{
				this.mCthulhuKilled = true;
				this.mDead = true;
				this.mHitPoints = 0f;
				this.ChangeState(Tentacle.States.Die);
			}
		}

		// Token: 0x17000606 RID: 1542
		// (get) Token: 0x060017CA RID: 6090 RVA: 0x0009D765 File Offset: 0x0009B965
		public override bool Dead
		{
			get
			{
				return this.mDead;
			}
		}

		// Token: 0x17000607 RID: 1543
		// (get) Token: 0x060017CB RID: 6091 RVA: 0x0009D76D File Offset: 0x0009B96D
		public override bool Removable
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060017CC RID: 6092 RVA: 0x0009D770 File Offset: 0x0009B970
		public override void Kill()
		{
			this.mHitPoints = 0f;
		}

		// Token: 0x060017CD RID: 6093 RVA: 0x0009D77D File Offset: 0x0009B97D
		internal override bool SendsNetworkUpdate(NetworkState iState)
		{
			return false;
		}

		// Token: 0x060017CE RID: 6094 RVA: 0x0009D780 File Offset: 0x0009B980
		protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
		{
			oMsg = default(EntityUpdateMessage);
		}

		// Token: 0x060017CF RID: 6095 RVA: 0x0009D78C File Offset: 0x0009B98C
		public bool SegmentIntersect(out Vector3 oPosition, Segment iSeg, float iSegmentRadius)
		{
			float num;
			Vector3 vector;
			return this.mCollision.SegmentIntersect(out num, out oPosition, out vector, iSeg);
		}

		// Token: 0x060017D0 RID: 6096 RVA: 0x0009D7AC File Offset: 0x0009B9AC
		public override bool ArcIntersect(out Vector3 oPosition, Vector3 iOrigin, Vector3 iDirection, float iRange, float iAngle, float iHeightDifference)
		{
			oPosition = default(Vector3);
			for (int i = 0; i < this.mCollision.NumPrimitives; i++)
			{
				Capsule capsule = this.mCollision.GetPrimitiveNewWorld(i) as Capsule;
				Vector3 value = Vector3.Subtract(capsule.Position, capsule.GetEnd());
				Vector3.Multiply(ref value, 0.5f, out value);
				Vector3 vector = Vector3.Add(capsule.Position, value);
				float value2 = vector.Y - iOrigin.Y;
				if (Math.Abs(value2) <= iHeightDifference)
				{
					iOrigin.Y = 0f;
					iDirection.Y = 0f;
					Vector3 vector2 = vector;
					vector2.Y = 0f;
					Vector3 vector3;
					Vector3.Subtract(ref iOrigin, ref vector2, out vector3);
					if (vector3.LengthSquared() > 1E-06f)
					{
						float num = vector3.Length();
						float length = capsule.Length;
						if (num - length <= iRange)
						{
							vector3.Normalize();
							float num2;
							Vector3.Dot(ref vector3, ref iDirection, out num2);
							num2 = -num2;
							float num3 = (float)Math.Acos((double)num2);
							float num4 = -2f * num * num;
							float num5 = (float)Math.Acos((double)((length * length + num4) / num4));
							if (num3 - num5 < iAngle)
							{
								Vector3.Multiply(ref vector3, length, out vector3);
								vector2 = vector;
								Vector3.Add(ref vector2, ref vector3, out oPosition);
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		// Token: 0x060017D1 RID: 6097 RVA: 0x0009D8FE File Offset: 0x0009BAFE
		internal void Revive()
		{
			if (this.mHitPoints <= 0f)
			{
				this.mHitPoints = this.mMaxHitPoints;
			}
		}

		// Token: 0x17000608 RID: 1544
		// (get) Token: 0x060017D2 RID: 6098 RVA: 0x0009D919 File Offset: 0x0009BB19
		public float HitPoints
		{
			get
			{
				return this.mHitPoints;
			}
		}

		// Token: 0x17000609 RID: 1545
		// (get) Token: 0x060017D3 RID: 6099 RVA: 0x0009D921 File Offset: 0x0009BB21
		public float MaxHitPoints
		{
			get
			{
				return this.mMaxHitPoints;
			}
		}

		// Token: 0x060017D4 RID: 6100 RVA: 0x0009D92C File Offset: 0x0009BB2C
		public DamageResult InternalDamage(DamageCollection5 iDamages, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			DamageResult damageResult = DamageResult.None;
			damageResult |= this.InternalDamage(iDamages.A, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			damageResult |= this.InternalDamage(iDamages.B, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			damageResult |= this.InternalDamage(iDamages.C, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			damageResult |= this.InternalDamage(iDamages.D, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			return damageResult | this.InternalDamage(iDamages.E, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
		}

		// Token: 0x060017D5 RID: 6101 RVA: 0x0009D9AC File Offset: 0x0009BBAC
		public DamageResult InternalDamage(Damage iDamage, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			Damage damage = iDamage;
			DamageResult damageResult = DamageResult.None;
			float num = 0f;
			float num2 = 0f;
			for (int i = 0; i < this.mResistances.Length; i++)
			{
				Elements elements = Defines.ElementFromIndex(i);
				if ((damage.Element & elements) == elements)
				{
					if (damage.Element == Elements.Earth && this.mResistances[i].Modifier != 0f)
					{
						damage.Amount = (float)((int)Math.Max(damage.Amount + this.mResistances[i].Modifier, 0f));
					}
					else
					{
						damage.Amount += (float)((int)this.mResistances[i].Modifier);
					}
					num += this.mResistances[i].Multiplier;
					num2 += 1f;
				}
			}
			if (num2 != 0f)
			{
				damage.Magnitude *= num / num2;
			}
			if (Math.Abs(damage.Magnitude) <= 1E-45f)
			{
				damageResult |= DamageResult.Deflected;
			}
			if ((damageResult & DamageResult.Deflected) == DamageResult.Deflected)
			{
				return damageResult;
			}
			if ((short)(damage.AttackProperty & AttackProperties.Status) == 32 && Math.Abs(num) > 1E-45f)
			{
				if ((damage.Element & Elements.Fire) == Elements.Fire && this.mResistances[Spell.ElementIndex(Elements.Fire)].Multiplier > 1E-45f)
				{
					damageResult |= this.AddStatusEffect(new TentacleStatusEffect(StatusEffects.Burning, damage.Amount, damage.Magnitude, 1f, 1f));
				}
				if ((damage.Element & Elements.Cold) == Elements.Cold && this.mResistances[Spell.ElementIndex(Elements.Cold)].Multiplier > 1E-45f)
				{
					damageResult |= this.AddStatusEffect(new TentacleStatusEffect(StatusEffects.Cold, damage.Amount, damage.Magnitude, 1f, 1f));
				}
				if ((damage.Element & Elements.Water) == Elements.Water && this.mResistances[Spell.ElementIndex(Elements.Water)].Multiplier > 1E-45f)
				{
					damageResult |= this.AddStatusEffect(new TentacleStatusEffect(StatusEffects.Wet, damage.Amount, damage.Magnitude, 1f, 1f));
				}
				if ((damage.Element & Elements.Steam) == Elements.Steam && this.mResistances[Spell.ElementIndex(Elements.Steam)].Multiplier > 1E-45f)
				{
					damageResult |= this.AddStatusEffect(new TentacleStatusEffect(StatusEffects.Steamed, damage.Amount, damage.Magnitude, 1f, 1f));
				}
			}
			if ((short)(damage.AttackProperty & AttackProperties.Damage) == 1)
			{
				if ((damage.Element & Elements.Lightning) == Elements.Lightning && this.HasStatus(StatusEffects.Wet))
				{
					damage.Amount *= 2f;
				}
				if ((damage.Element & Elements.Life) == Elements.Life)
				{
					TentacleStatusEffect[] array = this.mStatusEffects;
					int num3 = StatusEffect.StatusIndex(StatusEffects.Poisoned);
					array[num3].Magnitude = array[num3].Magnitude - damage.Magnitude;
					if (this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Poisoned)].Magnitude <= 0f)
					{
						this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Poisoned)].Stop();
					}
				}
				if ((damage.Element & Elements.PhysicalElements) != Elements.None && !this.HasStatus(StatusEffects.Frozen) && GlobalSettings.Instance.BloodAndGore == SettingOptions.On)
				{
					Vector3 vector = iAttackPosition;
					Vector3 right = Vector3.Right;
					VisualEffectReference visualEffectReference;
					EffectManager.Instance.StartEffect(Cthulhu.BLOOD_BLACK_EFFECT, ref vector, ref right, out visualEffectReference);
				}
				damage.Amount *= damage.Magnitude;
				this.mHitPoints -= damage.Amount;
				if (damage.Amount > 0f)
				{
					this.mHitFlashTimer = 0.5f;
				}
				if ((short)(damage.AttackProperty & AttackProperties.Piercing) != 0 && damage.Magnitude > 0f && damage.Amount > 0f)
				{
					damageResult |= DamageResult.Pierced;
				}
				if (damage.Amount > 0f)
				{
					damageResult |= DamageResult.Damaged;
				}
				if (damage.Amount == 0f)
				{
					damageResult |= DamageResult.Deflected;
				}
				if (damage.Amount < 0f)
				{
					damageResult |= DamageResult.Healed;
				}
				damageResult |= DamageResult.Hit;
				if (damage.Amount != 0f)
				{
					this.mTimeSinceLastDamage = 0f;
				}
				if (this.mLastDamageIndex >= 0)
				{
					DamageNotifyer.Instance.AddToNumber(this.mLastDamageIndex, damage.Amount);
				}
				else
				{
					if (this.mLastDamageIndex >= 0)
					{
						DamageNotifyer.Instance.ReleasNumber(this.mLastDamageIndex);
					}
					this.mLastDamageAmount = damage.Amount;
					this.mLastDamageElement = damage.Element;
					Vector3 notifierTextPostion = this.NotifierTextPostion;
					this.mLastDamageIndex = DamageNotifyer.Instance.AddNumber(damage.Amount, ref notifierTextPostion, 0.4f, true);
				}
			}
			if (this.mHitPoints > this.mMaxHitPoints)
			{
				this.mHitPoints = this.mMaxHitPoints;
			}
			if (damage.Amount == 0f)
			{
				damageResult |= DamageResult.Deflected;
			}
			if (this.mHitPoints <= 0f)
			{
				damageResult |= DamageResult.Killed;
			}
			return damageResult;
		}

		// Token: 0x060017D6 RID: 6102 RVA: 0x0009DEC0 File Offset: 0x0009C0C0
		public void Damage(float iDamage, Elements iElement)
		{
			if ((iElement & Elements.Fire) == Elements.Fire && this.HasStatus(StatusEffects.Greased))
			{
				iDamage *= 2f;
			}
			this.mHitPoints -= iDamage;
			if (this.mHitPoints > this.mMaxHitPoints)
			{
				this.mHitPoints = this.mMaxHitPoints;
			}
		}

		// Token: 0x060017D7 RID: 6103 RVA: 0x0009DF0E File Offset: 0x0009C10E
		public void OverKill()
		{
			this.Kill();
		}

		// Token: 0x060017D8 RID: 6104 RVA: 0x0009DF16 File Offset: 0x0009C116
		public void Electrocute(IDamageable iTarget, float iMultiplyer)
		{
		}

		// Token: 0x060017D9 RID: 6105 RVA: 0x0009DF18 File Offset: 0x0009C118
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
					if (this.HasStatus(StatusEffects.Frozen) && (iElement & Elements.Earth) != Elements.None)
					{
						num4 -= 350f;
					}
					if (this.HasStatus(StatusEffects.Greased) && (iElement & Elements.Fire) != Elements.None)
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

		// Token: 0x060017DA RID: 6106 RVA: 0x0009DFD4 File Offset: 0x0009C1D4
		internal void ClearAllStatusEffects()
		{
			for (int i = 0; i < this.mStatusEffects.Length; i++)
			{
				this.mStatusEffects[i].Stop();
				this.mStatusEffects[i] = default(TentacleStatusEffect);
			}
		}

		// Token: 0x060017DB RID: 6107 RVA: 0x0009E018 File Offset: 0x0009C218
		protected void UpdateDamage(float iDeltaTime)
		{
			Vector3 notifierTextPostion = this.NotifierTextPostion;
			this.mTimeSinceLastDamage += iDeltaTime;
			this.mTimeSinceLastStatusDamage += iDeltaTime;
			if (this.mLastDamageIndex >= 0)
			{
				if (this.mTimeSinceLastDamage > 0.333f || this.Dead)
				{
					DamageNotifyer.Instance.ReleasNumber(this.mLastDamageIndex);
					this.mLastDamageIndex = -1;
					return;
				}
				DamageNotifyer.Instance.UpdateNumberPosition(this.mLastDamageIndex, ref notifierTextPostion);
			}
		}

		// Token: 0x060017DC RID: 6108 RVA: 0x0009E090 File Offset: 0x0009C290
		protected void UpdateStatusEffects(float iDeltaTime)
		{
			this.mDryTimer -= iDeltaTime;
			StatusEffects statusEffects = StatusEffects.None;
			if (this.Dead)
			{
				for (int i = 0; i < this.mStatusEffects.Length; i++)
				{
					this.mStatusEffects[i].Stop();
					this.mStatusEffects[i] = default(TentacleStatusEffect);
				}
			}
			else
			{
				for (int j = 0; j < this.mStatusEffects.Length; j++)
				{
					this.mStatusEffects[j].Update(iDeltaTime, this, this.Body.CollisionSkin);
					if (this.mStatusEffects[j].Dead)
					{
						this.mStatusEffects[j].Stop();
						this.mStatusEffects[j] = default(TentacleStatusEffect);
					}
					else if (this.mStatusEffects[j].DamageType == StatusEffects.Wet)
					{
						if (this.mStatusEffects[j].Magnitude >= 1f)
						{
							statusEffects |= this.mStatusEffects[j].DamageType;
						}
					}
					else
					{
						statusEffects |= this.mStatusEffects[j].DamageType;
					}
				}
			}
			this.mCurrentStatusEffects = statusEffects;
		}

		// Token: 0x060017DD RID: 6109 RVA: 0x0009E1C0 File Offset: 0x0009C3C0
		public DamageResult AddStatusEffect(TentacleStatusEffect iStatusEffect)
		{
			DamageResult damageResult = DamageResult.None;
			if (!iStatusEffect.Dead)
			{
				bool flag = false;
				StatusEffects damageType = iStatusEffect.DamageType;
				if (damageType <= StatusEffects.Poisoned)
				{
					switch (damageType)
					{
					case StatusEffects.Burning:
						if (!this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].Dead || !this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)].Dead || !this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)].Dead || this.mDryTimer > 0f)
						{
							this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].Stop();
							this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)] = default(TentacleStatusEffect);
							this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)].Stop();
							this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)] = default(TentacleStatusEffect);
							this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)].Stop();
							this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)] = default(TentacleStatusEffect);
							flag = true;
						}
						break;
					case StatusEffects.Wet:
						if (this.HasStatus(StatusEffects.Burning) || this.mDryTimer > 0f)
						{
							int num = StatusEffect.StatusIndex(StatusEffects.Burning);
							this.mStatusEffects[num].Stop();
							this.mStatusEffects[num] = default(TentacleStatusEffect);
							flag = true;
						}
						if (this.HasStatus(StatusEffects.Greased))
						{
							int num2 = StatusEffect.StatusIndex(StatusEffects.Greased);
							this.mStatusEffects[num2].Stop();
							this.mStatusEffects[num2] = default(TentacleStatusEffect);
						}
						break;
					default:
						if (damageType != StatusEffects.Cold)
						{
							if (damageType == StatusEffects.Poisoned)
							{
								iStatusEffect.Magnitude *= this.mResistances[Defines.ElementIndex(Elements.Poison)].Multiplier;
							}
						}
						else
						{
							float num3 = iStatusEffect.Magnitude;
							if (!this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)].Dead || this.mDryTimer > 0f)
							{
								this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)].Stop();
								this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)] = default(TentacleStatusEffect);
								this.mDryTimer = 0.9f;
								flag = true;
							}
							num3 *= this.mResistances[Defines.ElementIndex(Elements.Cold)].Multiplier;
							if (this.HasStatus(StatusEffects.Wet))
							{
								this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)] = new TentacleStatusEffect(StatusEffects.Frozen, 0f, num3, 1f, 1f);
								this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)] = default(TentacleStatusEffect);
							}
							if (this.HasStatus(StatusEffects.Frozen))
							{
								TentacleStatusEffect[] array = this.mStatusEffects;
								int num4 = StatusEffect.StatusIndex(StatusEffects.Frozen);
								array[num4].Magnitude = array[num4].Magnitude + num3;
								num3 = 0f;
							}
							iStatusEffect.Magnitude = num3;
						}
						break;
					}
				}
				else if (damageType != StatusEffects.Healing)
				{
					if (damageType != StatusEffects.Greased)
					{
						if (damageType != StatusEffects.Steamed)
						{
						}
					}
					else if (this.HasStatus(StatusEffects.Wet))
					{
						int num5 = StatusEffect.StatusIndex(StatusEffects.Wet);
						this.mStatusEffects[num5].Stop();
						this.mStatusEffects[num5] = default(TentacleStatusEffect);
					}
				}
				else if (this.HasStatus(StatusEffects.Poisoned))
				{
					this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Poisoned)].Stop();
					this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Poisoned)] = default(TentacleStatusEffect);
				}
				if (!flag)
				{
					int num6 = StatusEffect.StatusIndex(iStatusEffect.DamageType);
					this.mStatusEffects[num6] = this.mStatusEffects[num6] + iStatusEffect;
					damageResult |= DamageResult.Statusadded;
				}
				else
				{
					damageResult |= DamageResult.Statusremoved;
				}
			}
			return damageResult;
		}

		// Token: 0x060017DE RID: 6110 RVA: 0x0009E58A File Offset: 0x0009C78A
		public bool HasStatus(StatusEffects iStatus)
		{
			return (this.mCurrentStatusEffects & iStatus) == iStatus;
		}

		// Token: 0x060017DF RID: 6111 RVA: 0x0009E598 File Offset: 0x0009C798
		public StatusEffect[] GetStatusEffects()
		{
			return null;
		}

		// Token: 0x060017E0 RID: 6112 RVA: 0x0009E59B File Offset: 0x0009C79B
		public float StatusMagnitude(StatusEffects iStatus)
		{
			return this.mStatusEffects[StatusEffect.StatusIndex(iStatus)].Magnitude;
		}

		// Token: 0x1700060A RID: 1546
		// (get) Token: 0x060017E1 RID: 6113 RVA: 0x0009E5B3 File Offset: 0x0009C7B3
		public float Volume
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x0400196A RID: 6506
		private const float NETWORK_UPDATE_PERIOD = 0.05f;

		// Token: 0x0400196B RID: 6507
		internal const int NR_OF_SPINE_PARTS = 11;

		// Token: 0x0400196C RID: 6508
		public const int MAX_RANGE_FOR_GOOD_SPOT = 18;

		// Token: 0x0400196D RID: 6509
		public const int CRUSH_RADIUS_RELATIVE_HEAD = 2;

		// Token: 0x0400196E RID: 6510
		private static readonly Vector3 ColdColor = new Vector3(1f, 1.6f, 2f);

		// Token: 0x0400196F RID: 6511
		private float mLastNetworkUpdate;

		// Token: 0x04001970 RID: 6512
		protected float mNetworkUpdateTimer;

		// Token: 0x04001971 RID: 6513
		private static readonly float[] SQUEEZE_DAMAGE_TIME = new float[]
		{
			0.4f,
			0.7f
		};

		// Token: 0x04001972 RID: 6514
		private static readonly float[] CRUSH_DAMAGE_TIME = new float[]
		{
			0.20833333f,
			0.5f
		};

		// Token: 0x04001973 RID: 6515
		private static readonly float[] GRAB_TIME = new float[]
		{
			0.17857143f,
			0.32142857f
		};

		// Token: 0x04001974 RID: 6516
		private Tentacle.States mPreviousState;

		// Token: 0x04001975 RID: 6517
		private Tentacle.States mCurrentState;

		// Token: 0x04001976 RID: 6518
		private Tentacle.RenderData[] mRenderData;

		// Token: 0x04001977 RID: 6519
		private Matrix mTransform;

		// Token: 0x04001978 RID: 6520
		private AnimationClip[] mAnimations;

		// Token: 0x04001979 RID: 6521
		private AnimationController mAnimationController;

		// Token: 0x0400197A RID: 6522
		private IDamageable mGrabbed;

		// Token: 0x0400197B RID: 6523
		private Damage mSqueezeDamage;

		// Token: 0x0400197C RID: 6524
		private Damage mCrushDamage;

		// Token: 0x0400197D RID: 6525
		private Damage mSwipeDamage;

		// Token: 0x0400197E RID: 6526
		private HitList mHitList;

		// Token: 0x0400197F RID: 6527
		private int mSpawnPoint;

		// Token: 0x04001980 RID: 6528
		private int mGrabAttachIndex;

		// Token: 0x04001981 RID: 6529
		private Matrix mGrabAttachBindPose;

		// Token: 0x04001982 RID: 6530
		private int[] mSpineIndex = new int[11];

		// Token: 0x04001983 RID: 6531
		private Matrix[] mSpineBindPose = new Matrix[11];

		// Token: 0x04001984 RID: 6532
		private Matrix mGrabTransform;

		// Token: 0x04001985 RID: 6533
		private Matrix mTransformedTopJoint;

		// Token: 0x04001986 RID: 6534
		private Cthulhu mOwner;

		// Token: 0x04001987 RID: 6535
		private byte mID;

		// Token: 0x04001988 RID: 6536
		private Avatar mCrushTarget;

		// Token: 0x04001989 RID: 6537
		private Vector3 mOriginalForwardDirection;

		// Token: 0x0400198A RID: 6538
		private bool mSearchForTarget;

		// Token: 0x0400198B RID: 6539
		private int mSecondsWhileOutOfRange;

		// Token: 0x0400198C RID: 6540
		private float mCheckEnemiesInRangeTimer;

		// Token: 0x0400198D RID: 6541
		private bool mCthulhuKilled;

		// Token: 0x0400198E RID: 6542
		private float mHitFlashTimer;

		// Token: 0x0400198F RID: 6543
		private float mHitPoints;

		// Token: 0x04001990 RID: 6544
		private float mMaxHitPoints;

		// Token: 0x04001991 RID: 6545
		private float mWetnessCounter;

		// Token: 0x04001992 RID: 6546
		private float mHPOnLastEmerge;

		// Token: 0x04001993 RID: 6547
		private float mTimeUntilSubmerge;

		// Token: 0x04001994 RID: 6548
		private float mTimeSinceLastEmerge;

		// Token: 0x04001995 RID: 6549
		private float mTentacleLength;

		// Token: 0x04001996 RID: 6550
		private VisualEffectReference mIdleEffectRef;

		// Token: 0x04001997 RID: 6551
		private int mIdleEffect = "cthulhu_tentacle_water_surface".GetHashCodeCustom();

		// Token: 0x04001998 RID: 6552
		private StatusEffects mCurrentStatusEffects;

		// Token: 0x04001999 RID: 6553
		private TentacleStatusEffect[] mStatusEffects = new TentacleStatusEffect[9];

		// Token: 0x0400199A RID: 6554
		private Resistance[] mResistances = new Resistance[11];

		// Token: 0x0400199B RID: 6555
		private int mLastDamageIndex;

		// Token: 0x0400199C RID: 6556
		private float mLastDamageAmount;

		// Token: 0x0400199D RID: 6557
		private Elements mLastDamageElement;

		// Token: 0x0400199E RID: 6558
		private float mTimeSinceLastDamage;

		// Token: 0x0400199F RID: 6559
		private float mTimeSinceLastStatusDamage;

		// Token: 0x040019A0 RID: 6560
		private float mDryTimer;

		// Token: 0x040019A1 RID: 6561
		private Tentacle.TentacleState[] mStates = new Tentacle.TentacleState[]
		{
			null,
			new Tentacle.IdleState(),
			new Tentacle.EmergeState(),
			new Tentacle.SubmergeState(),
			new Tentacle.SwipeGrabState(),
			new Tentacle.GrabSuccessState(),
			new Tentacle.SqueezeState(),
			new Tentacle.ReleaseState(),
			new Tentacle.CrushState(),
			new Tentacle.AimState(),
			new Tentacle.DieState()
		};

		// Token: 0x040019A2 RID: 6562
		private static readonly int SOUND_CRUSH = "cthulhu_tentacle_crush".GetHashCodeCustom();

		// Token: 0x040019A3 RID: 6563
		private static readonly int SOUND_EMERGE = "cthulhu_tentacle_emerge".GetHashCodeCustom();

		// Token: 0x040019A4 RID: 6564
		private static readonly int SOUND_GRAB_SUCCESS = "cthulhu_tentacle_grab_success".GetHashCodeCustom();

		// Token: 0x040019A5 RID: 6565
		private static readonly int SOUND_RELEASE = "cthulhu_tentacle_release".GetHashCodeCustom();

		// Token: 0x040019A6 RID: 6566
		private static readonly int SOUND_SQUEEZE = "cthulhu_tentacle_squeeze".GetHashCodeCustom();

		// Token: 0x040019A7 RID: 6567
		private static readonly int SOUND_SUBMERGE = "cthulhu_tentacle_submerge".GetHashCodeCustom();

		// Token: 0x040019A8 RID: 6568
		private static readonly int SOUND_SWIPE_GRAB = "cthulhu_tentacle_swipe_grab".GetHashCodeCustom();

		// Token: 0x02000305 RID: 773
		private class RenderData : IRenderableObject
		{
			// Token: 0x060017E3 RID: 6115 RVA: 0x0009E6A4 File Offset: 0x0009C8A4
			public RenderData(int iSkeletonLength, ModelMesh iMesh, ModelMeshPart iPart)
			{
				this.Skeleton = new Matrix[iSkeletonLength];
				this.mVertices = iMesh.VertexBuffer;
				this.mIndices = iMesh.IndexBuffer;
				this.mDeclaration = iPart.VertexDeclaration;
				this.mBaseVertex = iPart.BaseVertex;
				this.mNumVertices = iPart.NumVertices;
				this.mPrimCount = iPart.PrimitiveCount;
				this.mStartIndex = iPart.StartIndex;
				this.mVertexStride = iPart.VertexStride;
				SkinnedModelDeferredNormalMappedEffect iEffect = iPart.Effect as SkinnedModelDeferredNormalMappedEffect;
				this.Material.FetchFromEffect(iEffect);
			}

			// Token: 0x1700060B RID: 1547
			// (get) Token: 0x060017E4 RID: 6116 RVA: 0x0009E73B File Offset: 0x0009C93B
			public int Effect
			{
				get
				{
					return SkinnedModelDeferredNormalMappedEffect.TYPEHASH;
				}
			}

			// Token: 0x1700060C RID: 1548
			// (get) Token: 0x060017E5 RID: 6117 RVA: 0x0009E742 File Offset: 0x0009C942
			public int DepthTechnique
			{
				get
				{
					return 1;
				}
			}

			// Token: 0x1700060D RID: 1549
			// (get) Token: 0x060017E6 RID: 6118 RVA: 0x0009E745 File Offset: 0x0009C945
			public int Technique
			{
				get
				{
					return 0;
				}
			}

			// Token: 0x1700060E RID: 1550
			// (get) Token: 0x060017E7 RID: 6119 RVA: 0x0009E748 File Offset: 0x0009C948
			public int ShadowTechnique
			{
				get
				{
					return 2;
				}
			}

			// Token: 0x1700060F RID: 1551
			// (get) Token: 0x060017E8 RID: 6120 RVA: 0x0009E74B File Offset: 0x0009C94B
			public VertexBuffer Vertices
			{
				get
				{
					return this.mVertices;
				}
			}

			// Token: 0x17000610 RID: 1552
			// (get) Token: 0x060017E9 RID: 6121 RVA: 0x0009E753 File Offset: 0x0009C953
			public int VerticesHashCode
			{
				get
				{
					return this.mVerticesHash;
				}
			}

			// Token: 0x17000611 RID: 1553
			// (get) Token: 0x060017EA RID: 6122 RVA: 0x0009E75B File Offset: 0x0009C95B
			public int VertexStride
			{
				get
				{
					return this.mVertexStride;
				}
			}

			// Token: 0x17000612 RID: 1554
			// (get) Token: 0x060017EB RID: 6123 RVA: 0x0009E763 File Offset: 0x0009C963
			public IndexBuffer Indices
			{
				get
				{
					return this.mIndices;
				}
			}

			// Token: 0x17000613 RID: 1555
			// (get) Token: 0x060017EC RID: 6124 RVA: 0x0009E76B File Offset: 0x0009C96B
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return this.mDeclaration;
				}
			}

			// Token: 0x060017ED RID: 6125 RVA: 0x0009E773 File Offset: 0x0009C973
			public bool Cull(BoundingFrustum iViewFrustum)
			{
				return false;
			}

			// Token: 0x060017EE RID: 6126 RVA: 0x0009E778 File Offset: 0x0009C978
			public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelDeferredNormalMappedEffect skinnedModelDeferredNormalMappedEffect = iEffect as SkinnedModelDeferredNormalMappedEffect;
				this.Material.AssignToEffect(skinnedModelDeferredNormalMappedEffect);
				skinnedModelDeferredNormalMappedEffect.Bones = this.Skeleton;
				skinnedModelDeferredNormalMappedEffect.Damage = this.Damage;
				skinnedModelDeferredNormalMappedEffect.OverrideColor = new Vector4(1f, 1f, 1f, this.Flash);
				skinnedModelDeferredNormalMappedEffect.Colorize = this.Colorize;
				skinnedModelDeferredNormalMappedEffect.CommitChanges();
				skinnedModelDeferredNormalMappedEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimCount);
				skinnedModelDeferredNormalMappedEffect.OverrideColor = default(Vector4);
				skinnedModelDeferredNormalMappedEffect.Colorize = default(Vector4);
				skinnedModelDeferredNormalMappedEffect.Damage = 0f;
			}

			// Token: 0x060017EF RID: 6127 RVA: 0x0009E830 File Offset: 0x0009CA30
			public void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelDeferredNormalMappedEffect skinnedModelDeferredNormalMappedEffect = iEffect as SkinnedModelDeferredNormalMappedEffect;
				this.Material.AssignOpacityToEffect(skinnedModelDeferredNormalMappedEffect);
				skinnedModelDeferredNormalMappedEffect.Bones = this.Skeleton;
				skinnedModelDeferredNormalMappedEffect.CommitChanges();
				skinnedModelDeferredNormalMappedEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimCount);
			}

			// Token: 0x040019A9 RID: 6569
			public Matrix[] Skeleton;

			// Token: 0x040019AA RID: 6570
			public SkinnedModelDeferredNormalMappedMaterial Material;

			// Token: 0x040019AB RID: 6571
			public BoundingSphere BoundingSphere;

			// Token: 0x040019AC RID: 6572
			private VertexBuffer mVertices;

			// Token: 0x040019AD RID: 6573
			private int mVerticesHash;

			// Token: 0x040019AE RID: 6574
			private VertexDeclaration mDeclaration;

			// Token: 0x040019AF RID: 6575
			private IndexBuffer mIndices;

			// Token: 0x040019B0 RID: 6576
			private int mBaseVertex;

			// Token: 0x040019B1 RID: 6577
			private int mNumVertices;

			// Token: 0x040019B2 RID: 6578
			private int mPrimCount;

			// Token: 0x040019B3 RID: 6579
			private int mStartIndex;

			// Token: 0x040019B4 RID: 6580
			private int mVertexStride;

			// Token: 0x040019B5 RID: 6581
			public float Damage;

			// Token: 0x040019B6 RID: 6582
			public float Flash;

			// Token: 0x040019B7 RID: 6583
			public Vector4 Colorize;
		}

		// Token: 0x02000306 RID: 774
		private enum Animations
		{
			// Token: 0x040019B9 RID: 6585
			Idle,
			// Token: 0x040019BA RID: 6586
			Squeeze,
			// Token: 0x040019BB RID: 6587
			Crush,
			// Token: 0x040019BC RID: 6588
			Emerge,
			// Token: 0x040019BD RID: 6589
			Submerge,
			// Token: 0x040019BE RID: 6590
			Die,
			// Token: 0x040019BF RID: 6591
			Grip,
			// Token: 0x040019C0 RID: 6592
			Grip_Success,
			// Token: 0x040019C1 RID: 6593
			Let_Go,
			// Token: 0x040019C2 RID: 6594
			NR_OF_ANIMATIONS
		}

		// Token: 0x02000307 RID: 775
		internal enum States
		{
			// Token: 0x040019C4 RID: 6596
			None,
			// Token: 0x040019C5 RID: 6597
			Idle,
			// Token: 0x040019C6 RID: 6598
			Emerge,
			// Token: 0x040019C7 RID: 6599
			Submerge,
			// Token: 0x040019C8 RID: 6600
			SwipeGrab,
			// Token: 0x040019C9 RID: 6601
			GrabSuccess,
			// Token: 0x040019CA RID: 6602
			Squeeze,
			// Token: 0x040019CB RID: 6603
			Release,
			// Token: 0x040019CC RID: 6604
			Crush,
			// Token: 0x040019CD RID: 6605
			Aim,
			// Token: 0x040019CE RID: 6606
			Die,
			// Token: 0x040019CF RID: 6607
			NR_OF_STATES
		}

		// Token: 0x02000308 RID: 776
		private interface TentacleState
		{
			// Token: 0x060017F0 RID: 6128
			void OnEnter(Tentacle iOwner);

			// Token: 0x060017F1 RID: 6129
			void OnUpdate(Tentacle iOwner, float iDeltaTime);

			// Token: 0x060017F2 RID: 6130
			void OnExit(Tentacle iOwner);

			// Token: 0x060017F3 RID: 6131
			bool Active(Tentacle iOwner);
		}

		// Token: 0x02000309 RID: 777
		private class IdleState : Tentacle.TentacleState
		{
			// Token: 0x060017F4 RID: 6132 RVA: 0x0009E888 File Offset: 0x0009CA88
			public void OnEnter(Tentacle iOwner)
			{
				float num = 0.6f;
				switch (iOwner.mPreviousState)
				{
				case Tentacle.States.Submerge:
					num = 1f;
					break;
				case Tentacle.States.GrabSuccess:
				case Tentacle.States.Squeeze:
					num = 2f;
					break;
				case Tentacle.States.Aim:
					num = -1f;
					break;
				}
				if (num != -1f && !iOwner.IsPlaying(Tentacle.Animations.Idle))
				{
					iOwner.CrossFade(Tentacle.Animations.Idle, num, true);
				}
				this.mTimer = iOwner.mOwner.TimeBetweenAttacks;
			}

			// Token: 0x060017F5 RID: 6133 RVA: 0x0009E90C File Offset: 0x0009CB0C
			public void OnUpdate(Tentacle iOwner, float iDeltaTime)
			{
				if (this.mTimer <= 0f && iOwner.mOwner.OkToFight)
				{
					if (iOwner.mStates[3].Active(iOwner))
					{
						iOwner.ChangeState(Tentacle.States.Submerge);
					}
					else if (iOwner.mStates[4].Active(iOwner))
					{
						iOwner.ChangeState(Tentacle.States.SwipeGrab);
					}
					else if (iOwner.mStates[9].Active(iOwner))
					{
						iOwner.ChangeState(Tentacle.States.Aim);
					}
				}
				this.mTimer -= iDeltaTime;
			}

			// Token: 0x060017F6 RID: 6134 RVA: 0x0009E98C File Offset: 0x0009CB8C
			public void OnExit(Tentacle iOwner)
			{
			}

			// Token: 0x060017F7 RID: 6135 RVA: 0x0009E98E File Offset: 0x0009CB8E
			public bool Active(Tentacle iOwner)
			{
				return false;
			}

			// Token: 0x040019D0 RID: 6608
			private float mTimer;
		}

		// Token: 0x0200030A RID: 778
		private class EmergeState : Tentacle.TentacleState
		{
			// Token: 0x060017F9 RID: 6137 RVA: 0x0009E99C File Offset: 0x0009CB9C
			public void OnEnter(Tentacle iOwner)
			{
				iOwner.mDead = false;
				TentacleStatusEffect iStatusEffect = new TentacleStatusEffect(StatusEffects.Wet, 0f, 1f, 1f, 1f, 2f);
				iOwner.AddStatusEffect(iStatusEffect);
				iOwner.mHPOnLastEmerge = iOwner.HitPoints;
				iOwner.mTimeSinceLastEmerge = 0f;
				iOwner.mTimeUntilSubmerge = 30f + (float)Cthulhu.RANDOM.NextDouble() * 5f;
				Vector3 translation = iOwner.mTransform.Translation;
				Vector3 right = Vector3.Right;
				Vector3.Multiply(ref right, 2.5f, out right);
				Damage damage = new Damage(AttackProperties.Status, Elements.Fire, 100f, 4f);
				Liquid.Freeze(iOwner.mPlayState.Level.CurrentScene, ref translation, ref right, 6.2831855f, 2f, ref damage);
				this.mUnfreezeDone = false;
				this.mEffectDone = false;
				if (iOwner.mOwner.InitialEmerge)
				{
					this.mBubblesDone = true;
					this.mBubbleTimer = 0f;
					iOwner.StartClip(Tentacle.Animations.Emerge, false);
				}
				else
				{
					this.mBubblesDone = false;
					this.mBubbleTimer = 1.5f;
					iOwner.StartClip(Tentacle.Animations.Emerge, false);
					iOwner.StopClip();
				}
				AudioManager.Instance.PlayCue(Banks.Additional, Tentacle.SOUND_EMERGE, iOwner.AudioEmitter);
			}

			// Token: 0x060017FA RID: 6138 RVA: 0x0009EAE0 File Offset: 0x0009CCE0
			public void OnUpdate(Tentacle iOwner, float iDeltaTime)
			{
				if (this.mBubbleTimer >= 0f)
				{
					this.mBubbleTimer -= iDeltaTime;
					if (this.mBubbleTimer <= 0f)
					{
						iOwner.StartClip(Tentacle.Animations.Emerge, false);
						if (EffectManager.Instance.IsActive(ref this.mBubbleEffectRef))
						{
							EffectManager.Instance.Stop(ref this.mBubbleEffectRef);
							return;
						}
					}
					else if (!this.mBubblesDone && !EffectManager.Instance.IsActive(ref this.mBubbleEffectRef))
					{
						Vector3 translation = iOwner.mTransform.Translation;
						translation.Y = iOwner.WaterYPos;
						Matrix matrix = Matrix.CreateTranslation(translation);
						EffectManager.Instance.StartEffect(this.BubbleEffect, ref matrix, out this.mBubbleEffectRef);
						this.mBubblesDone = true;
						return;
					}
				}
				else if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					if (iOwner.AnimationHasFinished)
					{
						iOwner.ChangeState(Tentacle.States.Idle);
					}
					float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
					if (!this.mUnfreezeDone && num >= 0.09090909f)
					{
						Vector3 translation2 = iOwner.mTransform.Translation;
						Vector3 right = Vector3.Right;
						Vector3.Multiply(ref right, 4f, out right);
						Damage damage = new Damage(AttackProperties.Status, Elements.Fire, 100f, 4f);
						Liquid.Freeze(iOwner.mPlayState.Level.CurrentScene, ref translation2, ref right, 6.2831855f, 2f, ref damage);
						this.mUnfreezeDone = true;
					}
					if (!this.mEffectDone && num >= 0f)
					{
						if (!EffectManager.Instance.IsActive(ref this.mEffectRef))
						{
							Vector3 translation3 = iOwner.mTransform.Translation;
							translation3.Y = iOwner.WaterYPos;
							Matrix matrix2 = Matrix.CreateTranslation(translation3);
							EffectManager.Instance.StartEffect(this.WaterSplashEffect, ref matrix2, out this.mEffectRef);
						}
						this.mEffectDone = true;
						iOwner.TurnOnIdleEffect();
					}
				}
			}

			// Token: 0x060017FB RID: 6139 RVA: 0x0009ECC4 File Offset: 0x0009CEC4
			public void OnExit(Tentacle iOwner)
			{
			}

			// Token: 0x060017FC RID: 6140 RVA: 0x0009ECC6 File Offset: 0x0009CEC6
			public bool Active(Tentacle iOwner)
			{
				return false;
			}

			// Token: 0x040019D1 RID: 6609
			private const float LOGIC_EXECUTE_TIME = 0.21818182f;

			// Token: 0x040019D2 RID: 6610
			private const float PFX_EXECUTE_TIME = 0f;

			// Token: 0x040019D3 RID: 6611
			private const float UNFREEZE_TIME = 0.09090909f;

			// Token: 0x040019D4 RID: 6612
			private bool mEffectDone;

			// Token: 0x040019D5 RID: 6613
			private bool mUnfreezeDone;

			// Token: 0x040019D6 RID: 6614
			private bool mBubblesDone;

			// Token: 0x040019D7 RID: 6615
			private VisualEffectReference mEffectRef;

			// Token: 0x040019D8 RID: 6616
			private int WaterSplashEffect = "cthulhu_tentacle_emerge_water_splash".GetHashCodeCustom();

			// Token: 0x040019D9 RID: 6617
			private VisualEffectReference mBubbleEffectRef;

			// Token: 0x040019DA RID: 6618
			private int BubbleEffect = "cthulhu_tentacle_emerge_bubbles".GetHashCodeCustom();

			// Token: 0x040019DB RID: 6619
			private float mBubbleTimer;
		}

		// Token: 0x0200030B RID: 779
		private class SubmergeState : Tentacle.TentacleState
		{
			// Token: 0x060017FE RID: 6142 RVA: 0x0009ECF4 File Offset: 0x0009CEF4
			public void OnEnter(Tentacle iOwner)
			{
				iOwner.CrossFade(Tentacle.Animations.Submerge, 0.5f, false);
				this.mRegenerationTimer = (float)((iOwner.HitPoints <= 0f) ? (10 + MagickaMath.Random.Next(6)) : 0);
				this.mEffectDone = false;
				AudioManager.Instance.PlayCue(Banks.Additional, Tentacle.SOUND_SUBMERGE, iOwner.AudioEmitter);
			}

			// Token: 0x060017FF RID: 6143 RVA: 0x0009ED58 File Offset: 0x0009CF58
			public void OnUpdate(Tentacle iOwner, float iDeltaTime)
			{
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					if (NetworkManager.Instance.State != NetworkState.Client && iOwner.AnimationHasFinished)
					{
						this.mRegenerationTimer -= iDeltaTime;
						if (this.mRegenerationTimer <= 0f)
						{
							iOwner.Revive();
							iOwner.mOwner.SpawnTentacleAtGoodPoint(iOwner);
						}
					}
					float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
					if (!this.mEffectDone && num >= 0.4651163f)
					{
						if (!EffectManager.Instance.IsActive(ref this.mEffectRef))
						{
							Vector3 translation = iOwner.mTransform.Translation;
							translation.Y = 0f;
							Matrix matrix = Matrix.CreateTranslation(translation);
							EffectManager.Instance.StartEffect(this.WaterSplashEffect, ref matrix, out this.mEffectRef);
						}
						iOwner.KillIdleEffect();
						this.mEffectDone = true;
					}
				}
			}

			// Token: 0x06001800 RID: 6144 RVA: 0x0009EE3C File Offset: 0x0009D03C
			public void OnExit(Tentacle iOwner)
			{
				iOwner.mDead = true;
				iOwner.KillIdleEffect();
			}

			// Token: 0x06001801 RID: 6145 RVA: 0x0009EE4C File Offset: 0x0009D04C
			public bool Active(Tentacle iOwner)
			{
				return iOwner.mHPOnLastEmerge - iOwner.HitPoints > 0.5f * iOwner.MaxHitPoints || iOwner.mTimeUntilSubmerge < 0f || (iOwner.mTimeSinceLastEmerge > 5f && iOwner.mSecondsWhileOutOfRange > 0);
			}

			// Token: 0x040019DC RID: 6620
			private const float PFX_EXECUTE_TIME = 0f;

			// Token: 0x040019DD RID: 6621
			private float mRegenerationTimer;

			// Token: 0x040019DE RID: 6622
			private VisualEffectReference mEffectRef;

			// Token: 0x040019DF RID: 6623
			private int WaterSplashEffect = "cthulhu_tentacle_emerge_water_splash".GetHashCodeCustom();

			// Token: 0x040019E0 RID: 6624
			private bool mEffectDone;
		}

		// Token: 0x0200030C RID: 780
		private class DieState : Tentacle.TentacleState
		{
			// Token: 0x06001803 RID: 6147 RVA: 0x0009EEB8 File Offset: 0x0009D0B8
			public void OnEnter(Tentacle iOwner)
			{
				iOwner.CrossFade(Tentacle.Animations.Die, 0.5f, false);
				iOwner.mGrabbed = null;
				this.mEffectDone = false;
				AudioManager.Instance.PlayCue(Banks.Additional, Tentacle.SOUND_SUBMERGE, iOwner.AudioEmitter);
				iOwner.mOwner.LeaveSpawnTransform(iOwner);
			}

			// Token: 0x06001804 RID: 6148 RVA: 0x0009EF08 File Offset: 0x0009D108
			public void OnUpdate(Tentacle iOwner, float iDeltaTime)
			{
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
					if (!this.mEffectDone && num >= 0.4651163f)
					{
						if (!EffectManager.Instance.IsActive(ref this.mEffectRef))
						{
							Vector3 translation = iOwner.mTransform.Translation;
							translation.Y = 0f;
							Matrix matrix = Matrix.CreateTranslation(translation);
							EffectManager.Instance.StartEffect(this.WaterSplashEffect, ref matrix, out this.mEffectRef);
						}
						this.mEffectDone = true;
						iOwner.KillIdleEffect();
					}
				}
			}

			// Token: 0x06001805 RID: 6149 RVA: 0x0009EFA9 File Offset: 0x0009D1A9
			public void OnExit(Tentacle iOwner)
			{
				iOwner.mDead = true;
			}

			// Token: 0x06001806 RID: 6150 RVA: 0x0009EFB2 File Offset: 0x0009D1B2
			public bool Active(Tentacle iOwner)
			{
				return false;
			}

			// Token: 0x040019E1 RID: 6625
			private const float PFX_EXECUTE_TIME = 0f;

			// Token: 0x040019E2 RID: 6626
			private VisualEffectReference mEffectRef;

			// Token: 0x040019E3 RID: 6627
			private int WaterSplashEffect = "cthulhu_tentacle_emerge_water_splash".GetHashCodeCustom();

			// Token: 0x040019E4 RID: 6628
			private bool mEffectDone;
		}

		// Token: 0x0200030D RID: 781
		private class SwipeGrabState : Tentacle.TentacleState
		{
			// Token: 0x06001808 RID: 6152 RVA: 0x0009EFCD File Offset: 0x0009D1CD
			public void OnEnter(Tentacle iOwner)
			{
				iOwner.CrossFade(Tentacle.Animations.Grip, 0.2f, false);
				iOwner.mGrabbed = null;
				this.mDone = false;
				this.mDeflected = false;
				AudioManager.Instance.PlayCue(Banks.Additional, Tentacle.SOUND_GRAB_SUCCESS, iOwner.AudioEmitter);
			}

			// Token: 0x06001809 RID: 6153 RVA: 0x0009F00C File Offset: 0x0009D20C
			public unsafe void OnUpdate(Tentacle iOwner, float iDeltaTime)
			{
				if (iOwner.mGrabbed != null)
				{
					iOwner.ChangeState(Tentacle.States.GrabSuccess);
					return;
				}
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					if (iOwner.mAnimationController.HasFinished || this.mDeflected)
					{
						iOwner.ChangeState(Tentacle.States.Idle);
					}
					if (this.mDone)
					{
						return;
					}
					float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
					Segment iSeg = default(Segment);
					iSeg.Origin = iOwner.mTransform.Translation;
					iSeg.Delta = iOwner.mTransformedTopJoint.Translation;
					Vector3.Subtract(ref iSeg.Delta, ref iSeg.Origin, out iSeg.Delta);
					Vector3 vector;
					Vector3.Multiply(ref iSeg.Delta, 0.5f, out vector);
					Vector3.Add(ref iSeg.Origin, ref vector, out vector);
					float num2 = iSeg.Delta.Length() + 1.5f;
					List<Entity> entities = iOwner.mPlayState.EntityManager.GetEntities(vector, num2 / 2f, false);
					Vector3 translation = iOwner.mTransformedTopJoint.Translation;
					BoundingSphere boundingSphere = new BoundingSphere(translation, 1f);
					entities.Remove(iOwner);
					int num3 = 0;
					while (num3 < entities.Count && !this.mDone)
					{
						IDamageable damageable = entities[num3] as IDamageable;
						if (damageable != null)
						{
							if (damageable is Shield)
							{
								damageable.Damage(iOwner.mSwipeDamage, null, iOwner.mPlayState.PlayTime, vector);
								this.mDone = true;
								this.mDeflected = true;
								iOwner.CrossFade(Tentacle.Animations.Idle, 0.15f, true);
							}
							else if (!(damageable is SpellMine))
							{
								if (damageable is Barrier)
								{
									damageable.Damage(iOwner.mSwipeDamage, null, iOwner.mPlayState.PlayTime, vector);
									this.mDone = true;
									this.mDeflected = true;
									iOwner.CrossFade(Tentacle.Animations.Idle, 0.15f, true);
								}
								else if (num >= Tentacle.GRAB_TIME[0] && num <= Tentacle.GRAB_TIME[1])
								{
									BoundingSphere sphere = new BoundingSphere(damageable.Position, damageable.Radius);
									if (!this.mDone && boundingSphere.Intersects(sphere) && damageable is Avatar)
									{
										this.mDone = true;
										if (NetworkManager.Instance.State == NetworkState.Client)
										{
											break;
										}
										iOwner.mGrabbed = (entities[num3] as IDamageable);
										if (NetworkManager.Instance.State == NetworkState.Server)
										{
											Cthulhu.TentacleGrabMessage tentacleGrabMessage = default(Cthulhu.TentacleGrabMessage);
											tentacleGrabMessage.Handle = iOwner.mGrabbed.Handle;
											tentacleGrabMessage.TentacleIndex = iOwner.mID;
											BossFight.Instance.SendMessage<Cthulhu.TentacleGrabMessage>(iOwner.mOwner, 14, (void*)(&tentacleGrabMessage), true);
											break;
										}
										break;
									}
									else
									{
										Vector3 vector2;
										if (!this.mDone && damageable.SegmentIntersect(out vector2, iSeg, 0.6f) && !iOwner.mHitList.Contains(damageable))
										{
											vector2 += iOwner.mTransform.Right * 1f;
											damageable.Damage(iOwner.mSwipeDamage, null, iOwner.mPlayState.PlayTime, vector2);
											Damage iDamage = new Damage(AttackProperties.Knockback, Elements.Earth, damageable.Body.Mass, 2f);
											damageable.Damage(iDamage, iOwner, iOwner.mPlayState.PlayTime, vector2);
											iOwner.mHitList.Add(damageable);
											break;
										}
										break;
									}
								}
							}
						}
						num3++;
					}
					iOwner.mPlayState.EntityManager.ReturnEntityList(entities);
				}
			}

			// Token: 0x0600180A RID: 6154 RVA: 0x0009F391 File Offset: 0x0009D591
			public void OnExit(Tentacle iOwner)
			{
			}

			// Token: 0x0600180B RID: 6155 RVA: 0x0009F394 File Offset: 0x0009D594
			public bool Active(Tentacle iOwner)
			{
				if (Cthulhu.RANDOM.NextDouble() < 0.5)
				{
					EntityManager entityManager = iOwner.mPlayState.EntityManager;
					Vector3 translation = iOwner.mTransform.Translation;
					List<Entity> entities = entityManager.GetEntities(translation, iOwner.mTentacleLength - 1.2f, true);
					bool flag = false;
					for (int i = 0; i < entities.Count; i++)
					{
						IDamageable damageable = entities[i] as IDamageable;
						if (damageable != null)
						{
							if (damageable is Shield || damageable is Barrier)
							{
								flag = false;
								break;
							}
							if (!flag)
							{
								Avatar avatar = entities[i] as Avatar;
								flag = (avatar != null && !avatar.IsEthereal && !avatar.IsInvisibile);
							}
						}
					}
					entityManager.ReturnEntityList(entities);
					return flag;
				}
				return false;
			}

			// Token: 0x040019E5 RID: 6629
			private const float TOP_JOINT_TO_TOPMOST_POINT = 1.5f;

			// Token: 0x040019E6 RID: 6630
			private bool mDone;

			// Token: 0x040019E7 RID: 6631
			private bool mDeflected;
		}

		// Token: 0x0200030E RID: 782
		private class GrabSuccessState : Tentacle.TentacleState
		{
			// Token: 0x0600180D RID: 6157 RVA: 0x0009F465 File Offset: 0x0009D665
			public void OnEnter(Tentacle iOwner)
			{
				iOwner.CrossFade(Tentacle.Animations.Grip_Success, 0.1f, false);
				AudioManager.Instance.PlayCue(Banks.Additional, Tentacle.SOUND_GRAB_SUCCESS, iOwner.AudioEmitter);
			}

			// Token: 0x0600180E RID: 6158 RVA: 0x0009F48F File Offset: 0x0009D68F
			public void OnUpdate(Tentacle iOwner, float iDeltaTime)
			{
				if (iOwner.AnimationHasFinished)
				{
					iOwner.ChangeState(Tentacle.States.Squeeze);
				}
			}

			// Token: 0x0600180F RID: 6159 RVA: 0x0009F4A0 File Offset: 0x0009D6A0
			public void OnExit(Tentacle iOwner)
			{
			}

			// Token: 0x06001810 RID: 6160 RVA: 0x0009F4A2 File Offset: 0x0009D6A2
			public bool Active(Tentacle iOwner)
			{
				return false;
			}
		}

		// Token: 0x0200030F RID: 783
		private class SqueezeState : Tentacle.TentacleState
		{
			// Token: 0x06001812 RID: 6162 RVA: 0x0009F4B0 File Offset: 0x0009D6B0
			public void OnEnter(Tentacle iOwner)
			{
				iOwner.CrossFade(Tentacle.Animations.Squeeze, 0.15f, true);
				this.mTimer = 5f - iOwner.mOwner.TimeBetweenAttacks;
				AudioManager.Instance.PlayCue(Banks.Additional, Tentacle.SOUND_SQUEEZE, iOwner.AudioEmitter);
			}

			// Token: 0x06001813 RID: 6163 RVA: 0x0009F4FC File Offset: 0x0009D6FC
			public void OnUpdate(Tentacle iOwner, float iDeltaTime)
			{
				IDamageable mGrabbed = iOwner.mGrabbed;
				if (mGrabbed != null && mGrabbed is IStatusEffected)
				{
					IStatusEffected statusEffected = mGrabbed as IStatusEffected;
					if (statusEffected.HasStatus(StatusEffects.Burning))
					{
						this.mTimer = 0f;
					}
				}
				if (this.mTimer <= 0f)
				{
					if (mGrabbed != null)
					{
						iOwner.ChangeState(Tentacle.States.Release);
						return;
					}
					iOwner.mGrabbed = null;
				}
				this.mTimer -= iDeltaTime;
				if (iOwner.mGrabbed == null || iOwner.mGrabbed.Dead)
				{
					iOwner.ChangeState(Tentacle.States.Idle);
					return;
				}
				if (!iOwner.mAnimationController.CrossFadeEnabled && !iOwner.mHitList.Contains(iOwner.mGrabbed))
				{
					float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
					if (num >= Tentacle.SQUEEZE_DAMAGE_TIME[0] && num <= Tentacle.SQUEEZE_DAMAGE_TIME[1])
					{
						iOwner.mGrabbed.Damage(iOwner.mSqueezeDamage, null, iOwner.mPlayState.PlayTime, iOwner.mGrabTransform.Translation);
						iOwner.mHitList.Add(iOwner.mGrabbed);
					}
				}
			}

			// Token: 0x06001814 RID: 6164 RVA: 0x0009F60B File Offset: 0x0009D80B
			public void OnExit(Tentacle iOwner)
			{
			}

			// Token: 0x06001815 RID: 6165 RVA: 0x0009F60D File Offset: 0x0009D80D
			public bool Active(Tentacle iOwner)
			{
				return false;
			}

			// Token: 0x040019E8 RID: 6632
			private float mTimer;
		}

		// Token: 0x02000310 RID: 784
		private class ReleaseState : Tentacle.TentacleState
		{
			// Token: 0x06001817 RID: 6167 RVA: 0x0009F618 File Offset: 0x0009D818
			public void OnEnter(Tentacle iOwner)
			{
				iOwner.CrossFade(Tentacle.Animations.Let_Go, 0.2f, false);
				AudioManager.Instance.PlayCue(Banks.Additional, Tentacle.SOUND_RELEASE, iOwner.AudioEmitter);
			}

			// Token: 0x06001818 RID: 6168 RVA: 0x0009F644 File Offset: 0x0009D844
			public void OnUpdate(Tentacle iOwner, float iDeltaTime)
			{
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					if (iOwner.mAnimationController.HasFinished)
					{
						iOwner.ChangeState(Tentacle.States.Idle);
						return;
					}
					float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
					if (iOwner.mGrabbed != null && num >= 0.4054054f)
					{
						Vector3 forward = iOwner.mTransform.Forward;
						forward.Y = 0f;
						Vector3.Multiply(ref forward, 12f, out forward);
						iOwner.mGrabbed.Body.Velocity = forward;
						iOwner.mGrabbed = null;
					}
				}
			}

			// Token: 0x06001819 RID: 6169 RVA: 0x0009F6E1 File Offset: 0x0009D8E1
			public void OnExit(Tentacle iOwner)
			{
			}

			// Token: 0x0600181A RID: 6170 RVA: 0x0009F6E3 File Offset: 0x0009D8E3
			public bool Active(Tentacle iOwner)
			{
				return false;
			}

			// Token: 0x040019E9 RID: 6633
			private const float THROW_TIME = 0.4054054f;
		}

		// Token: 0x02000311 RID: 785
		private class AimState : Tentacle.TentacleState
		{
			// Token: 0x0600181C RID: 6172 RVA: 0x0009F6EE File Offset: 0x0009D8EE
			public void OnEnter(Tentacle iOwner)
			{
				this.mRadius = iOwner.mTentacleLength;
				iOwner.mSearchForTarget = true;
				iOwner.CrossFade(Tentacle.Animations.Idle, 0.6f, true);
			}

			// Token: 0x0600181D RID: 6173 RVA: 0x0009F710 File Offset: 0x0009D910
			public unsafe void OnUpdate(Tentacle iOwner, float iDeltaTime)
			{
				Vector3 translation = iOwner.mTransform.Translation;
				Vector3 forward = iOwner.mTransform.Forward;
				Vector3 vector = Vector3.Zero;
				Vector3 vector2 = Vector3.Zero;
				bool flag = false;
				if (iOwner.mSearchForTarget)
				{
					Avatar avatar = iOwner.mCrushTarget;
					if (NetworkManager.Instance.State != NetworkState.Client)
					{
						if (avatar != null && (avatar.Dead || avatar.Drowning))
						{
							avatar = (iOwner.mCrushTarget = null);
						}
						if (avatar != null && !avatar.Dead && !avatar.IsEthereal)
						{
							float num = translation.X - avatar.Position.X;
							float num2 = translation.Z - avatar.Position.Z;
							float num3 = num * num + num2 * num2;
							if (num3 > this.mRadius * this.mRadius)
							{
								avatar = null;
							}
						}
					}
					if (avatar == null)
					{
						if (NetworkManager.Instance.State != NetworkState.Client)
						{
							List<Entity> entities = iOwner.mPlayState.EntityManager.GetEntities(translation, this.mRadius, false, true);
							entities.Remove(iOwner);
							int num4 = 0;
							while (num4 < entities.Count && !flag)
							{
								avatar = (entities[num4] as Avatar);
								flag = (avatar != null && !avatar.IsEthereal);
								if (flag)
								{
									iOwner.mCrushTarget = avatar;
									vector = entities[num4].Position;
									Vector3.Subtract(ref vector, ref translation, out vector2);
									vector2.Y = 0f;
									if (vector2.LengthSquared() <= 1E-06f)
									{
										flag = false;
									}
									else
									{
										vector2.Normalize();
									}
								}
								num4++;
							}
							iOwner.mPlayState.EntityManager.ReturnEntityList(entities);
							if (flag && NetworkManager.Instance.State == NetworkState.Server)
							{
								Cthulhu.TentacleAimTargetMessage tentacleAimTargetMessage = default(Cthulhu.TentacleAimTargetMessage);
								tentacleAimTargetMessage.Handle = iOwner.mCrushTarget.Handle;
								tentacleAimTargetMessage.TentacleIndex = iOwner.mID;
								BossFight.Instance.SendMessage<Cthulhu.TentacleAimTargetMessage>(iOwner.mOwner, 15, (void*)(&tentacleAimTargetMessage), true);
							}
						}
					}
					else
					{
						flag = true;
						vector = avatar.Position;
						Vector3.Subtract(ref vector, ref translation, out vector2);
						vector2.Y = 0f;
						if (vector2.LengthSquared() <= 1E-06f)
						{
							flag = false;
						}
						else
						{
							vector2.Normalize();
						}
					}
				}
				if (!flag)
				{
					vector2 = iOwner.mOriginalForwardDirection;
				}
				float num5;
				Tentacle.AimState.GetAngle(ref forward, ref vector2, out num5);
				if ((double)Math.Abs(num5) < 0.01 && flag)
				{
					iOwner.ChangeState(Tentacle.States.Crush);
				}
				float num6 = MathHelper.ToRadians(30f);
				float num7;
				if (num5 < 0f)
				{
					num7 = Math.Max(num5, -num6);
				}
				else
				{
					num7 = Math.Min(num5, num6);
				}
				num7 = 2f * num7 * iOwner.mOwner.StageSpeedModifier * iOwner.GetColdSlowdown();
				Quaternion quaternion;
				Quaternion.CreateFromYawPitchRoll(num7 * iDeltaTime, 0f, 0f, out quaternion);
				Vector3 forward2 = iOwner.mTransform.Forward;
				Vector3 vector3;
				Vector3.Transform(ref forward2, ref quaternion, out vector3);
				Vector3 translation2 = iOwner.mTransform.Translation;
				Matrix.Transform(ref iOwner.mTransform, ref quaternion, out iOwner.mTransform);
				iOwner.mTransform.Translation = translation2;
				if ((double)Math.Abs(num5) < 0.01 && !flag)
				{
					iOwner.ChangeState(Tentacle.States.Idle);
				}
			}

			// Token: 0x0600181E RID: 6174 RVA: 0x0009FA48 File Offset: 0x0009DC48
			private static void GetConstrainedAngle(ref Vector3 iForward, ref Vector3 iTargetDirection, out float oAngle, float iMinAngle, float iMaxAngle)
			{
				Vector3 vector = new Vector3(0f, 0f, 1f);
				MagickaMath.ConstrainVector(ref iTargetDirection, ref vector, iMinAngle, iMaxAngle);
				Tentacle.AimState.GetAngle(ref iForward, ref iTargetDirection, out oAngle);
			}

			// Token: 0x0600181F RID: 6175 RVA: 0x0009FA84 File Offset: 0x0009DC84
			private static void GetAngle(ref Vector3 iForward, ref Vector3 iTargetDirection, out float oAngle)
			{
				Vector3 vector;
				Vector3.Cross(ref iTargetDirection, ref iForward, out vector);
				float num = (float)Math.Acos((double)MathHelper.Clamp(vector.Y, -1f, 1f));
				num -= 1.5707964f;
				oAngle = num;
			}

			// Token: 0x06001820 RID: 6176 RVA: 0x0009FAC3 File Offset: 0x0009DCC3
			public void OnExit(Tentacle iOwner)
			{
			}

			// Token: 0x06001821 RID: 6177 RVA: 0x0009FAC8 File Offset: 0x0009DCC8
			public bool Active(Tentacle iOwner)
			{
				return iOwner.CheckIfCharactersAreWithinRadius(iOwner.mTentacleLength);
			}

			// Token: 0x040019EA RID: 6634
			private float mRadius = 8f;
		}

		// Token: 0x02000312 RID: 786
		private class CrushState : Tentacle.TentacleState
		{
			// Token: 0x06001823 RID: 6179 RVA: 0x0009FAF6 File Offset: 0x0009DCF6
			public void OnEnter(Tentacle iOwner)
			{
				iOwner.CrossFade(Tentacle.Animations.Crush, 0.15f, false);
				this.mDone = false;
				this.mDeflected = false;
			}

			// Token: 0x06001824 RID: 6180 RVA: 0x0009FB14 File Offset: 0x0009DD14
			public unsafe void OnUpdate(Tentacle iOwner, float iDeltaTime)
			{
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					if (iOwner.mAnimationController.HasFinished || this.mDeflected)
					{
						iOwner.ChangeState(Tentacle.States.Aim);
						iOwner.mCrushTarget = null;
						iOwner.mSearchForTarget = false;
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							Cthulhu.TentacleReleaseAimTargetMessage tentacleReleaseAimTargetMessage = default(Cthulhu.TentacleReleaseAimTargetMessage);
							tentacleReleaseAimTargetMessage.TentacleIndex = iOwner.mID;
							BossFight.Instance.SendMessage<Cthulhu.TentacleReleaseAimTargetMessage>(iOwner.mOwner, 16, (void*)(&tentacleReleaseAimTargetMessage), true);
						}
					}
					if (this.mDone)
					{
						return;
					}
					float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
					Segment iSeg = default(Segment);
					iSeg.Origin = iOwner.mTransform.Translation;
					iSeg.Delta = iOwner.mTransformedTopJoint.Translation;
					Vector3.Subtract(ref iSeg.Delta, ref iSeg.Origin, out iSeg.Delta);
					Vector3 iCenter;
					Vector3.Multiply(ref iSeg.Delta, 0.5f, out iCenter);
					Vector3.Add(ref iSeg.Origin, ref iCenter, out iCenter);
					float num2 = iSeg.Delta.Length();
					List<Entity> entities = iOwner.mPlayState.EntityManager.GetEntities(iCenter, num2 / 2f, false);
					entities.Remove(iOwner);
					for (int i = 0; i < entities.Count; i++)
					{
						IDamageable damageable = entities[i] as IDamageable;
						Vector3 iAttackPosition;
						if (damageable != null && damageable.SegmentIntersect(out iAttackPosition, iSeg, 0.6f))
						{
							if (damageable is Shield)
							{
								this.mDone = true;
								damageable.Kill();
								this.mDeflected = true;
								iOwner.CrossFade(Tentacle.Animations.Idle, 0.15f, true);
							}
							else if (damageable is SpellMine || damageable is Barrier)
							{
								damageable.Kill();
							}
							else if (num >= 0.35416666f && !iOwner.mHitList.Contains(damageable))
							{
								damageable.Damage(iOwner.mCrushDamage, null, iOwner.mPlayState.PlayTime, iAttackPosition);
								Damage iDamage = new Damage(AttackProperties.Knockback, Elements.Earth, damageable.Body.Mass, 2f);
								damageable.Damage(iDamage, null, iOwner.mPlayState.PlayTime, iAttackPosition);
								iOwner.mHitList.Add(damageable);
							}
						}
					}
					if (num >= 0.35416666f && !this.mDone)
					{
						this.mDone = true;
						if (!EffectManager.Instance.IsActive(ref this.mEffectRef))
						{
							Vector3 translation = iOwner.mTransformedTopJoint.Translation;
							Matrix matrix = Matrix.CreateTranslation(translation);
							EffectManager.Instance.StartEffect(this.CrushDustEffect, ref matrix, out this.mEffectRef);
						}
						AudioManager.Instance.PlayCue(Banks.Additional, Tentacle.SOUND_CRUSH, iOwner.AudioEmitter);
					}
					iOwner.mPlayState.EntityManager.ReturnEntityList(entities);
				}
			}

			// Token: 0x06001825 RID: 6181 RVA: 0x0009FDDF File Offset: 0x0009DFDF
			public void OnExit(Tentacle iOwner)
			{
			}

			// Token: 0x06001826 RID: 6182 RVA: 0x0009FDE1 File Offset: 0x0009DFE1
			public bool Active(Tentacle iOwner)
			{
				return false;
			}

			// Token: 0x040019EB RID: 6635
			private VisualEffectReference mEffectRef;

			// Token: 0x040019EC RID: 6636
			private int CrushDustEffect = "cthulhu_tentacle_crush_dust".GetHashCodeCustom();

			// Token: 0x040019ED RID: 6637
			private bool mDone;

			// Token: 0x040019EE RID: 6638
			private bool mDeflected;
		}
	}
}
