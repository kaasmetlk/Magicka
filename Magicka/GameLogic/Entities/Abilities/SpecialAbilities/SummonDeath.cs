using System;
using System.Collections.Generic;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Magicka.Audio;
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

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000178 RID: 376
	public class SummonDeath : SpecialAbility, IAbilityEffect
	{
		// Token: 0x170002BA RID: 698
		// (get) Token: 0x06000B71 RID: 2929 RVA: 0x00044520 File Offset: 0x00042720
		public static SummonDeath Instance
		{
			get
			{
				if (SummonDeath.mSingelton == null)
				{
					lock (SummonDeath.mSingeltonLock)
					{
						if (SummonDeath.mSingelton == null)
						{
							SummonDeath.mSingelton = new SummonDeath();
						}
					}
				}
				return SummonDeath.mSingelton;
			}
		}

		// Token: 0x06000B72 RID: 2930 RVA: 0x00044574 File Offset: 0x00042774
		private SummonDeath() : base(Animations.cast_magick_direct, "#magick_sdeath".GetHashCodeCustom())
		{
		}

		// Token: 0x06000B73 RID: 2931 RVA: 0x00044588 File Offset: 0x00042788
		public void Initialize(PlayState iPlayState)
		{
			this.mDeath = new SummonDeath.MagickDeath(iPlayState);
		}

		// Token: 0x06000B74 RID: 2932 RVA: 0x00044598 File Offset: 0x00042798
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			if (this.IsDead)
			{
				this.mOwner = iOwner;
				this.mPlayState = iPlayState;
				return this.Execute(this.mOwner.Position);
			}
			AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL);
			return false;
		}

		// Token: 0x06000B75 RID: 2933 RVA: 0x000445E9 File Offset: 0x000427E9
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			if (this.IsDead)
			{
				this.mPlayState = iPlayState;
				this.mOwner = null;
				return this.Execute(iPosition);
			}
			AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL);
			return false;
		}

		// Token: 0x06000B76 RID: 2934 RVA: 0x0004461C File Offset: 0x0004281C
		private bool Execute(Vector3 iPosition)
		{
			AudioManager.Instance.PlayCue(Banks.Spells, SummonDeath.SOUND_HASH);
			SummonDeath.sCue = AudioManager.Instance.GetCue(Banks.Spells, SummonDeath.SOUND_SLOWDOWN_HASH);
			SummonDeath.sCue.Play();
			Vector3 vector = this.mPlayState.Camera.Position;
			Vector3 cameraoffset = MagickCamera.CAMERAOFFSET;
			Vector3.Negate(ref cameraoffset, out cameraoffset);
			Vector3.Add(ref vector, ref cameraoffset, out vector);
			Character character = null;
			List<Entity> entities = this.mPlayState.EntityManager.GetEntities(vector, 40f, false);
			float num = 1f;
			int num2 = SpecialAbility.RANDOM.Next(entities.Count);
			for (int i = 0; i < entities.Count; i++)
			{
				int index = (i + num2) % entities.Count;
				Character character2 = entities[index] as Character;
				if (character2 != null && !character2.Dead && (character2.Faction & Factions.UNDEAD) == Factions.NONE)
				{
					float num3 = character2.HitPoints / character2.MaxHitPoints;
					if (num3 > 0f && num3 <= num)
					{
						num = num3;
						character = character2;
					}
				}
			}
			this.mPlayState.EntityManager.ReturnEntityList(entities);
			if (character == null)
			{
				if (!(this.mOwner is Character))
				{
					AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL);
					return false;
				}
				character = (this.mOwner as Character);
			}
			vector = character.Position;
			Vector3 vector2 = vector;
			int num4 = 0;
			float num6;
			do
			{
				float num5 = (float)(SpecialAbility.RANDOM.NextDouble() - 0.5) * 2f;
				float z;
				float x;
				MathApproximation.FastSinCos(num5 * 3.1415927f, out z, out x);
				Vector3 vector3 = new Vector3(x, 0f, z);
				Vector3.Multiply(ref vector3, 7f, out vector3);
				Vector3.Add(ref vector, ref vector3, out vector);
				Vector3 vector4;
				this.mPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref vector, out vector4, MovementProperties.Default);
				vector = vector4;
				Vector3.DistanceSquared(ref vector, ref vector2, out num6);
			}
			while (num6 < 4f && num4 < 10);
			if (num4 > 9)
			{
				return false;
			}
			Vector3 up = Vector3.Up;
			Vector3 vector5 = vector;
			Vector3 position = character.Position;
			vector5.Y = (position.Y = 0f);
			Matrix matrix;
			Matrix.CreateLookAt(ref vector5, ref position, ref up, out matrix);
			matrix.Translation = vector;
			this.mDeath.Initialize(ref matrix, this.mPlayState, character);
			this.mPlayState.EntityManager.AddEntity(this.mDeath);
			SpellManager.Instance.AddSpellEffect(this);
			return true;
		}

		// Token: 0x170002BB RID: 699
		// (get) Token: 0x06000B77 RID: 2935 RVA: 0x00044894 File Offset: 0x00042A94
		public bool IsDead
		{
			get
			{
				return this.mDeath.Dead;
			}
		}

		// Token: 0x06000B78 RID: 2936 RVA: 0x000448A1 File Offset: 0x00042AA1
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			RenderManager.Instance.Saturation = 0.5f;
		}

		// Token: 0x06000B79 RID: 2937 RVA: 0x000448B2 File Offset: 0x00042AB2
		public void OnRemove()
		{
			RenderManager.Instance.Saturation = 1f;
			if (SummonDeath.sCue != null)
			{
				SummonDeath.sCue.Stop(AudioStopOptions.AsAuthored);
			}
			this.mDeath.Kill();
		}

		// Token: 0x04000A61 RID: 2657
		private static SummonDeath mSingelton;

		// Token: 0x04000A62 RID: 2658
		private static volatile object mSingeltonLock = new object();

		// Token: 0x04000A63 RID: 2659
		private SummonDeath.MagickDeath mDeath;

		// Token: 0x04000A64 RID: 2660
		private PlayState mPlayState;

		// Token: 0x04000A65 RID: 2661
		private ISpellCaster mOwner;

		// Token: 0x04000A66 RID: 2662
		private static readonly int SOUND_HASH = "magick_summon_death".GetHashCodeCustom();

		// Token: 0x04000A67 RID: 2663
		private static readonly int SOUND_SLOWDOWN_HASH = "magick_timewarp".GetHashCodeCustom();

		// Token: 0x04000A68 RID: 2664
		private static Cue sCue;

		// Token: 0x02000179 RID: 377
		public class MagickDeath : Entity, IDamageable
		{
			// Token: 0x06000B7B RID: 2939 RVA: 0x0004490C File Offset: 0x00042B0C
			public MagickDeath(PlayState iPlayState) : base(iPlayState)
			{
				this.mRandom = new Random();
				this.mPlayState = iPlayState;
				this.mDead = true;
				Model model;
				lock (Game.Instance.GraphicsDevice)
				{
					this.mModel = Game.Instance.Content.Load<SkinnedModel>("Models/Bosses/Death/Death");
					model = Game.Instance.Content.Load<Model>("Models/Bosses/Death/death_Scythe");
				}
				Matrix matrix = Matrix.CreateRotationY(3.1415927f);
				foreach (SkinnedModelBone skinnedModelBone in this.mModel.SkeletonBones)
				{
					if (skinnedModelBone.Name.Equals("RightAttach", StringComparison.OrdinalIgnoreCase))
					{
						this.mHandJoint.mIndex = (int)skinnedModelBone.Index;
						this.mHandJoint.mBindPose = skinnedModelBone.InverseBindPoseTransform;
						Matrix.Multiply(ref this.mHandJoint.mBindPose, ref matrix, out this.mHandJoint.mBindPose);
						Matrix.Invert(ref this.mHandJoint.mBindPose, out this.mHandJoint.mBindPose);
					}
				}
				this.mController = new AnimationController();
				this.mController.Skeleton = this.mModel.SkeletonBones;
				this.mClips = new AnimationClip[4];
				this.mClips[0] = this.mModel.AnimationClips["move_glide"];
				this.mClips[1] = this.mModel.AnimationClips["attack_scythe_rise"];
				this.mClips[2] = this.mModel.AnimationClips["attack_scythe_fall"];
				this.mClips[3] = this.mModel.AnimationClips["hit"];
				SkinnedModelBasicEffect iEffect = this.mModel.Model.Meshes[0].MeshParts[0].Effect as SkinnedModelBasicEffect;
				SkinnedModelDeferredBasicMaterial mMaterial;
				Helper.SkinnedModelDeferredMaterialFromBasicEffect(iEffect, out mMaterial);
				this.mBoundingSphere = this.mModel.Model.Meshes[0].BoundingSphere;
				this.mRenderData = new SummonDeath.MagickDeath.DeferredRenderData[3];
				this.mScytheRenderData = new SummonDeath.MagickDeath.ItemRenderData[3];
				this.mAfterImageRenderData = new SummonDeath.MagickDeath.AfterImageRenderData[3];
				Matrix[][] array = new Matrix[5][];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = new Matrix[80];
				}
				for (int j = 0; j < 3; j++)
				{
					this.mRenderData[j] = new SummonDeath.MagickDeath.DeferredRenderData();
					this.mRenderData[j].SetMesh(this.mModel.Model.Meshes[0].VertexBuffer, this.mModel.Model.Meshes[0].IndexBuffer, this.mModel.Model.Meshes[0].MeshParts[0], 0, 3, 4);
					this.mRenderData[j].mMaterial = mMaterial;
					this.mScytheRenderData[j] = new SummonDeath.MagickDeath.ItemRenderData();
					this.mScytheRenderData[j].SetMesh(model.Meshes[0], model.Meshes[0].MeshParts[0], 4, 0, 5);
					this.mAfterImageRenderData[j] = new SummonDeath.MagickDeath.AfterImageRenderData(array);
				}
				this.mBody = new Body();
				this.mCollision = new CollisionSkin(this.mBody);
				this.mCollision.AddPrimitive(new Capsule(Vector3.Zero, Matrix.CreateRotationX(-1.5707964f), 0.6f, 1.2f), 1, new MaterialProperties(0f, 0f, 0f));
				this.mCollision.callbackFn += this.OnCollision;
				this.mBody.CollisionSkin = this.mCollision;
				this.mBody.SetBodyInvInertia(0f, 0f, 0f);
				this.mBody.Mass = 500f;
				this.mBody.Immovable = false;
				this.mBody.Tag = this;
			}

			// Token: 0x06000B7C RID: 2940 RVA: 0x00044D54 File Offset: 0x00042F54
			public bool OnCollision(CollisionSkin skin0, int prim0, CollisionSkin skin1, int prim1)
			{
				return false;
			}

			// Token: 0x06000B7D RID: 2941 RVA: 0x00044D58 File Offset: 0x00042F58
			public void Initialize(ref Matrix iOrientation, PlayState iPlayState, Character iTarget)
			{
				this.mPlayState = iPlayState;
				this.mTarget = iTarget;
				Vector3 position = this.mTarget.Position;
				Vector3 vector = iOrientation.Translation;
				iOrientation.Translation = default(Vector3);
				Vector3 vector2;
				Vector3.Subtract(ref position, ref vector, out vector2);
				vector2.Normalize();
				Segment iSeg = default(Segment);
				iSeg.Origin = vector + Vector3.Up;
				iSeg.Delta.Y = iSeg.Delta.Y - 6f;
				float num;
				Vector3 vector3;
				Vector3 vector4;
				if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out num, out vector3, out vector4, iSeg))
				{
					vector = vector3;
				}
				this.mOrientation = iOrientation;
				this.mOrientation.Translation = vector;
				this.Turn(ref vector2, 1f, 1f);
				vector.Y += 1.3000001f;
				this.mOrientation.Translation = vector;
				this.mBody.MoveTo(vector, iOrientation);
				this.mDead = false;
				this.mIsHit = false;
				this.mSpeed = 8f;
				base.Initialize();
				Matrix[] array = new Matrix[5];
				for (int i = 0; i < 3; i++)
				{
					for (int j = 0; j < this.mAfterImageRenderData[i].mSkeleton.Length; j++)
					{
						array.CopyTo(this.mAfterImageRenderData[i].mSkeleton[j], 0);
					}
				}
				this.mHitlist.Clear();
				this.mRangeSqr = 0.39600003f + this.mTarget.Radius * this.mTarget.Radius;
				this.mRangeSqr *= 1.2f;
				this.mController.StartClip(this.mClips[0], true);
				this.mCurrentAnimation = SummonDeath.MagickDeath.Animations.Move;
				this.mTimeTargetModifier = 0.25f;
				this.mPursuitCue = AudioManager.Instance.PlayCue(Banks.Characters, SummonDeath.MagickDeath.DEATH_PURSUIT_SOUND, this.mAudioEmitter);
				this.mScytheCue = null;
				this.mDeathCue = null;
				VisualEffectReference visualEffectReference;
				EffectManager.Instance.StartEffect(SummonDeath.MagickDeath.SPAWN_EFFECT, ref this.mOrientation, out visualEffectReference);
				this.mSpawnDelayTimer = 1f / this.mTimeTargetModifier * 0.5f;
				this.mMovement = Vector3.Zero;
				this.mAlphaTimer = 0f;
				this.mOscilationTimer = 0f;
				this.mIsEthereal = true;
			}

			// Token: 0x06000B7E RID: 2942 RVA: 0x00044FB0 File Offset: 0x000431B0
			public float ResistanceAgainst(Elements iElement)
			{
				float num = 0f;
				float num2 = 0f;
				float num3 = MathHelper.Clamp(num / 300f + num2, -1f, 1f);
				return 1f - num3;
			}

			// Token: 0x06000B7F RID: 2943 RVA: 0x00044FEC File Offset: 0x000431EC
			public override void Update(DataChannel iDataChannel, float iDeltaTime)
			{
				float num = this.mPlayState.TimeModifier;
				num += (this.mTimeTargetModifier - num) * (iDeltaTime / this.mTimeTargetModifier);
				this.mPlayState.TimeModifier = num;
				iDeltaTime /= this.mPlayState.TimeModifier * this.mPlayState.TimeMultiplier;
				base.Update(iDataChannel, iDeltaTime);
				this.mHitlist.Update(iDeltaTime);
				Vector3 vector = this.Position;
				vector = this.mOrientation.Translation;
				vector.X += this.mMovement.X * this.mSpeed * iDeltaTime;
				vector.Z += this.mMovement.Z * this.mSpeed * iDeltaTime;
				Segment iSeg = default(Segment);
				iSeg.Origin = vector;
				iSeg.Origin.Y = iSeg.Origin.Y + 1f;
				iSeg.Delta.Y = iSeg.Delta.Y - 3f;
				float num2;
				Vector3 vector2;
				Vector3 vector3;
				if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out num2, out vector2, out vector3, iSeg))
				{
					vector2.Y += 1.2f;
				}
				vector.Y += (vector2.Y - vector.Y) * iDeltaTime;
				this.mBody.Velocity = Vector3.Zero;
				this.mBody.AngularVelocity = Vector3.Zero;
				this.mBody.MoveTo(vector, this.mOrientation);
				this.mOrientation.Translation = vector;
				Matrix matrix = SummonDeath.MagickDeath.sScale;
				Matrix matrix2;
				Matrix.Multiply(ref matrix, ref this.mOrientation, out matrix2);
				vector.Y -= 1.2f;
				this.mOscilationTimer += iDeltaTime;
				vector.Y += (float)Math.Cos((double)this.mOscilationTimer) * 0.2f;
				matrix2.Translation = vector;
				this.mBoundingSphere.Center = vector;
				this.mController.Update(iDeltaTime, ref matrix2, true);
				this.mController.SkinnedBoneTransforms.CopyTo(this.mRenderData[(int)iDataChannel].mBones, 0);
				this.mRenderData[(int)iDataChannel].mBoundingSphere = this.mBoundingSphere;
				this.mRenderData[(int)iDataChannel].RenderAdditive = this.IsEthereal;
				if (!this.IsEthereal)
				{
					this.mPlayState.Scene.AddRenderableObject(iDataChannel, this.mRenderData[(int)iDataChannel]);
				}
				else
				{
					this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, this.mRenderData[(int)iDataChannel]);
				}
				Matrix mBindPose = this.mHandJoint.mBindPose;
				Matrix.Multiply(ref mBindPose, ref this.mController.SkinnedBoneTransforms[this.mHandJoint.mIndex], out mBindPose);
				Vector3 translation = mBindPose.Translation;
				Vector3 up = mBindPose.Up;
				Vector3.Add(ref up, ref translation, out translation);
				Segment seg = default(Segment);
				seg.Origin = translation;
				Vector3.Subtract(ref seg.Origin, ref this.mLastScythePosition, out seg.Delta);
				this.mLastScythePosition = seg.Origin;
				translation = mBindPose.Translation;
				matrix = SummonDeath.MagickDeath.sInvScale;
				Matrix.Multiply(ref mBindPose, ref matrix, out mBindPose);
				mBindPose.Translation = translation;
				this.mScytheRenderData[(int)iDataChannel].mBoundingSphere = this.mBoundingSphere;
				this.mScytheRenderData[(int)iDataChannel].WorldOrientation = mBindPose;
				this.mPlayState.Scene.AddRenderableObject(iDataChannel, this.mScytheRenderData[(int)iDataChannel]);
				if (this.mDead || this.mTarget == null || this.mTarget.Dead)
				{
					this.mDead = true;
					return;
				}
				if (this.mCurrentAnimation == SummonDeath.MagickDeath.Animations.Move)
				{
					this.mAlphaTimer += iDeltaTime;
					this.mRenderData[(int)iDataChannel].mMaterial.Alpha = Math.Min(this.mAlphaTimer, 1f);
					Vector3 position = this.mTarget.Position;
					Vector3 movement;
					Vector3.Subtract(ref position, ref vector, out movement);
					movement.Y = 0f;
					float num3 = movement.LengthSquared();
					movement.Normalize();
					this.Turn(ref movement, 8f, iDeltaTime);
					this.mSpawnDelayTimer -= iDeltaTime;
					if (this.mSpawnDelayTimer <= 0f)
					{
						this.Movement = movement;
					}
					if (num3 <= this.mRangeSqr)
					{
						this.mSpeed = 0f;
						this.mController.CrossFade(this.mClips[1], 0.33f, false);
						this.mCurrentAnimation = SummonDeath.MagickDeath.Animations.Scythe_Rise;
						this.IsEthereal = false;
						AudioManager.Instance.PlayCue(Banks.Weapons, SummonDeath.MagickDeath.SCYTHE_SWING_SOUND, this.mAudioEmitter);
					}
				}
				else if (this.mCurrentAnimation == SummonDeath.MagickDeath.Animations.Scythe_Rise)
				{
					this.mAlphaTimer += iDeltaTime;
					this.mRenderData[(int)iDataChannel].mMaterial.Alpha = Math.Min(this.mAlphaTimer, 1f);
					if (this.mController.HasFinished && !this.mController.CrossFadeEnabled)
					{
						this.mCurrentAnimation = SummonDeath.MagickDeath.Animations.Scythe_Fall;
						this.mController.CrossFade(this.mClips[2], 0.1f, false);
					}
				}
				else if (this.mCurrentAnimation == SummonDeath.MagickDeath.Animations.Scythe_Fall)
				{
					this.mAlphaTimer += iDeltaTime;
					this.mRenderData[(int)iDataChannel].mMaterial.Alpha = Math.Min(this.mAlphaTimer, 1f);
					Vector3 position2 = this.mTarget.Position;
					Vector3 movement2;
					Vector3.Subtract(ref position2, ref vector, out movement2);
					movement2.Y = 0f;
					float num4 = movement2.LengthSquared();
					movement2.Normalize();
					float num5 = Math.Max(0f, Math.Min(1f, num4 - this.mRangeSqr * 1.1f));
					Vector3.Multiply(ref movement2, num5, out movement2);
					this.Movement = movement2;
					this.Turn(ref movement2, num5 * 4f, iDeltaTime);
					float num6 = this.mController.Time / this.mController.AnimationClip.Duration;
					if (!this.mController.CrossFadeEnabled && num6 >= 0.1f && num6 <= 0.8f)
					{
						if (this.mScytheCue == null)
						{
							this.mScytheCue = AudioManager.Instance.PlayCue(Banks.Weapons, SummonDeath.MagickDeath.SCYTHE_SWING_SOUND, this.mAudioEmitter);
						}
						List<Entity> entities = this.mPlayState.EntityManager.GetEntities(seg.Origin, 3f, false, true);
						entities.Remove(this);
						for (int i = 0; i < entities.Count; i++)
						{
							IStatusEffected statusEffected = entities[i] as IStatusEffected;
							float num7;
							Vector3 vector4;
							Vector3 vector5;
							if (statusEffected != null && !this.mHitlist.ContainsKey(statusEffected.Handle) && entities[i].Body.CollisionSkin.SegmentIntersect(out num7, out vector4, out vector5, seg))
							{
								AudioManager.Instance.PlayCue(Banks.Weapons, SummonDeath.MagickDeath.SCYTHE_HIT_SOUND, this.mAudioEmitter);
								statusEffected.Damage(statusEffected.HitPoints, Elements.Arcane);
								this.mHitlist.Add(statusEffected.Handle, 1.5f);
							}
						}
						this.mPlayState.EntityManager.ReturnEntityList(entities);
					}
					if (this.mIsHit)
					{
						this.mController.CrossFade(this.mClips[3], 0.33f, false);
						this.mCurrentAnimation = SummonDeath.MagickDeath.Animations.Disappear;
						this.mAlphaTimer = 1f;
					}
					else if (!this.mController.CrossFadeEnabled && this.mController.HasFinished)
					{
						this.mController.CrossFade(this.mClips[3], 0.33f, false);
						this.mCurrentAnimation = SummonDeath.MagickDeath.Animations.Disappear;
						this.mAlphaTimer = 1f;
					}
				}
				else if (this.mCurrentAnimation == SummonDeath.MagickDeath.Animations.Disappear)
				{
					this.mAlphaTimer -= iDeltaTime;
					this.mRenderData[(int)iDataChannel].mMaterial.Alpha = Math.Max(this.mAlphaTimer, 0f);
					if (this.mDeathCue == null)
					{
						this.mDeathCue = AudioManager.Instance.PlayCue(Banks.Characters, SummonDeath.MagickDeath.DEATH_SOUND, base.AudioEmitter);
						VisualEffectReference visualEffectReference;
						EffectManager.Instance.StartEffect(SummonDeath.MagickDeath.DESPAWN_EFFECT, ref this.mOrientation, out visualEffectReference);
					}
					this.mTimeTargetModifier = 1f;
					if (!this.mController.CrossFadeEnabled && this.mController.HasFinished)
					{
						this.mDead = true;
					}
				}
				if (this.mMovement.LengthSquared() > 0.5f)
				{
					SummonDeath.MagickDeath.AfterImageRenderData afterImageRenderData = this.mAfterImageRenderData[(int)iDataChannel];
					if (afterImageRenderData.MeshDirty)
					{
						ModelMesh modelMesh = this.mModel.Model.Meshes[0];
						ModelMeshPart iMeshPart = modelMesh.MeshParts[0];
						afterImageRenderData.SetMesh(modelMesh.VertexBuffer, modelMesh.IndexBuffer, iMeshPart, SkinnedModelBasicEffect.TYPEHASH);
					}
					int count = this.mModel.SkeletonBones.Count;
					this.mAfterImageTimer -= iDeltaTime;
					this.mAfterImageIntensity -= iDeltaTime;
					while (this.mAfterImageTimer <= 0f)
					{
						this.mAfterImageTimer += 0.0375f;
						if (this.mSpeed > 0f)
						{
							while (this.mAfterImageIntensity <= 0f)
							{
								this.mAfterImageIntensity += 0.05f;
							}
						}
						for (int j = afterImageRenderData.mSkeleton.Length - 1; j > 0; j--)
						{
							Array.Copy(afterImageRenderData.mSkeleton[j - 1], afterImageRenderData.mSkeleton[j], count);
						}
						Array.Copy(this.mController.SkinnedBoneTransforms, afterImageRenderData.mSkeleton[0], count);
					}
					afterImageRenderData.mIntensity = this.mAfterImageIntensity * 20f;
					afterImageRenderData.mBoundingSphere = this.mRenderData[(int)iDataChannel].mBoundingSphere;
					if (!this.IsEthereal)
					{
						this.mPlayState.Scene.AddRenderableObject(iDataChannel, afterImageRenderData);
						return;
					}
					this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, afterImageRenderData);
				}
			}

			// Token: 0x06000B80 RID: 2944 RVA: 0x000459C7 File Offset: 0x00043BC7
			public override void Deinitialize()
			{
				if (this.mPursuitCue != null && !this.mPursuitCue.IsStopping)
				{
					this.mPursuitCue.Stop(AudioStopOptions.AsAuthored);
				}
				this.mPlayState.TimeModifier = 1f;
				base.Deinitialize();
				this.mDead = true;
			}

			// Token: 0x170002BC RID: 700
			// (get) Token: 0x06000B81 RID: 2945 RVA: 0x00045A07 File Offset: 0x00043C07
			public override bool Dead
			{
				get
				{
					return this.mDead;
				}
			}

			// Token: 0x170002BD RID: 701
			// (get) Token: 0x06000B82 RID: 2946 RVA: 0x00045A0F File Offset: 0x00043C0F
			public override bool Removable
			{
				get
				{
					return this.mDead;
				}
			}

			// Token: 0x06000B83 RID: 2947 RVA: 0x00045A17 File Offset: 0x00043C17
			public override void Kill()
			{
				this.Deinitialize();
			}

			// Token: 0x170002BE RID: 702
			// (get) Token: 0x06000B84 RID: 2948 RVA: 0x00045A1F File Offset: 0x00043C1F
			public float HitPoints
			{
				get
				{
					return 1f;
				}
			}

			// Token: 0x170002BF RID: 703
			// (get) Token: 0x06000B85 RID: 2949 RVA: 0x00045A26 File Offset: 0x00043C26
			public float MaxHitPoints
			{
				get
				{
					return 1f;
				}
			}

			// Token: 0x06000B86 RID: 2950 RVA: 0x00045A2D File Offset: 0x00043C2D
			internal override float GetDanger()
			{
				return 1000f;
			}

			// Token: 0x06000B87 RID: 2951 RVA: 0x00045A34 File Offset: 0x00043C34
			public bool SegmentIntersect(out Vector3 oPosition, Segment iSeg, float iSegmentRadius)
			{
				float num;
				Vector3 vector;
				return this.mBody.CollisionSkin.SegmentIntersect(out num, out oPosition, out vector, iSeg);
			}

			// Token: 0x06000B88 RID: 2952 RVA: 0x00045A58 File Offset: 0x00043C58
			public DamageResult InternalDamage(DamageCollection5 iDamages, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
			{
				return this.InternalDamage(iDamages.A, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.B, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.C, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.D, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.E, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			}

			// Token: 0x06000B89 RID: 2953 RVA: 0x00045AC8 File Offset: 0x00043CC8
			public DamageResult InternalDamage(Damage iDamage, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
			{
				if ((iDamage.Element & Elements.Life) == Elements.Life && !this.IsEthereal)
				{
					if (Defines.FeatureDamage(iFeatures))
					{
						this.mIsHit = true;
					}
					return DamageResult.Hit;
				}
				return DamageResult.None;
			}

			// Token: 0x06000B8A RID: 2954 RVA: 0x00045AF3 File Offset: 0x00043CF3
			public void Electrocute(IDamageable iTarget, float iMultiplyer)
			{
			}

			// Token: 0x06000B8B RID: 2955 RVA: 0x00045AF5 File Offset: 0x00043CF5
			public void OverKill()
			{
				this.Kill();
			}

			// Token: 0x06000B8C RID: 2956 RVA: 0x00045AFD File Offset: 0x00043CFD
			protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
			{
				oMsg = default(EntityUpdateMessage);
				oMsg.Features |= EntityFeatures.Position;
				oMsg.Position = this.Position;
			}

			// Token: 0x06000B8D RID: 2957 RVA: 0x00045B24 File Offset: 0x00043D24
			protected void Turn(ref Vector3 iNewDirection, float iTurnSpeed, float iDeltaTime)
			{
				Vector3 up = Vector3.Up;
				Vector3 right;
				Vector3.Cross(ref iNewDirection, ref up, out right);
				Matrix identity = Matrix.Identity;
				identity.Forward = iNewDirection;
				identity.Up = up;
				identity.Right = right;
				Quaternion quaternion;
				Quaternion.CreateFromRotationMatrix(ref this.mOrientation, out quaternion);
				Quaternion quaternion2;
				Quaternion.CreateFromRotationMatrix(ref identity, out quaternion2);
				Quaternion.Lerp(ref quaternion, ref quaternion2, MathHelper.Clamp(iDeltaTime * iTurnSpeed, 0f, 1f), out quaternion2);
				Matrix matrix;
				Matrix.CreateFromQuaternion(ref quaternion2, out matrix);
				matrix.Translation = this.mOrientation.Translation;
				this.mOrientation = matrix;
			}

			// Token: 0x170002C0 RID: 704
			// (get) Token: 0x06000B8E RID: 2958 RVA: 0x00045BBA File Offset: 0x00043DBA
			// (set) Token: 0x06000B8F RID: 2959 RVA: 0x00045BC4 File Offset: 0x00043DC4
			protected Vector3 Movement
			{
				get
				{
					return this.mMovement;
				}
				set
				{
					value.Y = 0f;
					this.mMovement = value;
					float num = this.mMovement.LengthSquared();
					if (num > 1E-45f)
					{
						num = (float)Math.Sqrt((double)num);
						float num2 = 1f / num;
						if (num > 1f)
						{
							this.mMovement.X = value.X * num2;
							this.mMovement.Y = value.Y * num2;
							this.mMovement.Z = value.Z * num2;
						}
					}
				}
			}

			// Token: 0x170002C1 RID: 705
			// (get) Token: 0x06000B90 RID: 2960 RVA: 0x00045C4D File Offset: 0x00043E4D
			// (set) Token: 0x06000B91 RID: 2961 RVA: 0x00045C55 File Offset: 0x00043E55
			public bool IsEthereal
			{
				get
				{
					return this.mIsEthereal;
				}
				set
				{
					this.mIsEthereal = value;
				}
			}

			// Token: 0x04000A69 RID: 2665
			private const float CAPSULE_RADIUS = 0.6f;

			// Token: 0x04000A6A RID: 2666
			private const float CAPSULE_LENGTH = 1.2f;

			// Token: 0x04000A6B RID: 2667
			private const float SCYTHE_ATTACK_START = 0.40625f;

			// Token: 0x04000A6C RID: 2668
			private const float SCYTHE_ATTACK_END = 0.46875f;

			// Token: 0x04000A6D RID: 2669
			private const float SCALE = 1.2f;

			// Token: 0x04000A6E RID: 2670
			private const float HEIGHT_OFFSET = 1.2f;

			// Token: 0x04000A6F RID: 2671
			private static readonly int SPAWN_EFFECT = "death_ethereal_spawn".GetHashCodeCustom();

			// Token: 0x04000A70 RID: 2672
			private static readonly int DESPAWN_EFFECT = "death_ethereal_despawn".GetHashCodeCustom();

			// Token: 0x04000A71 RID: 2673
			private static readonly int SCYTHE_HIT_SOUND = "wep_death_scythe".GetHashCodeCustom();

			// Token: 0x04000A72 RID: 2674
			private static readonly int SCYTHE_SWING_SOUND = "wep_death_scythe_swing".GetHashCodeCustom();

			// Token: 0x04000A73 RID: 2675
			private static readonly int DEATH_PURSUIT_SOUND = "boss_death_proximity".GetHashCodeCustom();

			// Token: 0x04000A74 RID: 2676
			private static readonly int DEATH_SOUND = "boss_death_death".GetHashCodeCustom();

			// Token: 0x04000A75 RID: 2677
			private Cue mPursuitCue;

			// Token: 0x04000A76 RID: 2678
			private Cue mScytheCue;

			// Token: 0x04000A77 RID: 2679
			private Cue mDeathCue;

			// Token: 0x04000A78 RID: 2680
			private HitList mHitlist = new HitList(32);

			// Token: 0x04000A79 RID: 2681
			private SummonDeath.MagickDeath.DeferredRenderData[] mRenderData;

			// Token: 0x04000A7A RID: 2682
			private SummonDeath.MagickDeath.ItemRenderData[] mScytheRenderData;

			// Token: 0x04000A7B RID: 2683
			private SummonDeath.MagickDeath.AfterImageRenderData[] mAfterImageRenderData;

			// Token: 0x04000A7C RID: 2684
			private float mAfterImageTimer;

			// Token: 0x04000A7D RID: 2685
			private float mAfterImageIntensity;

			// Token: 0x04000A7E RID: 2686
			private AnimationController mController;

			// Token: 0x04000A7F RID: 2687
			private AnimationClip[] mClips;

			// Token: 0x04000A80 RID: 2688
			private SummonDeath.MagickDeath.Animations mCurrentAnimation;

			// Token: 0x04000A81 RID: 2689
			private Vector3 mLastScythePosition;

			// Token: 0x04000A82 RID: 2690
			private SkinnedModel mModel;

			// Token: 0x04000A83 RID: 2691
			private Random mRandom;

			// Token: 0x04000A84 RID: 2692
			private Character mTarget;

			// Token: 0x04000A85 RID: 2693
			private bool mIsHit;

			// Token: 0x04000A86 RID: 2694
			private float mOscilationTimer;

			// Token: 0x04000A87 RID: 2695
			private float mAlphaTimer;

			// Token: 0x04000A88 RID: 2696
			private float mSpawnDelayTimer;

			// Token: 0x04000A89 RID: 2697
			private float mRangeSqr;

			// Token: 0x04000A8A RID: 2698
			private float mTimeTargetModifier;

			// Token: 0x04000A8B RID: 2699
			private float mSpeed;

			// Token: 0x04000A8C RID: 2700
			private Matrix mOrientation;

			// Token: 0x04000A8D RID: 2701
			private Vector3 mMovement;

			// Token: 0x04000A8E RID: 2702
			private BoundingSphere mBoundingSphere;

			// Token: 0x04000A8F RID: 2703
			private BindJoint mHandJoint;

			// Token: 0x04000A90 RID: 2704
			private bool mIsEthereal = true;

			// Token: 0x04000A91 RID: 2705
			private static readonly Matrix sScale = Matrix.CreateScale(1.2f);

			// Token: 0x04000A92 RID: 2706
			private static readonly Matrix sInvScale = Matrix.Invert(SummonDeath.MagickDeath.sScale);

			// Token: 0x0200017A RID: 378
			public class ItemRenderData : RenderableObject<RenderDeferredEffect, RenderDeferredMaterial>, IRenderableAdditiveObject
			{
				// Token: 0x06000B93 RID: 2963 RVA: 0x00045CE5 File Offset: 0x00043EE5
				public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
				{
					this.mMaterial.WorldTransform = this.WorldOrientation;
					base.Draw(iEffect, iViewFrustum);
				}

				// Token: 0x06000B94 RID: 2964 RVA: 0x00045D00 File Offset: 0x00043F00
				public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
				{
					this.mMaterial.WorldTransform = this.WorldOrientation;
					base.DrawShadow(iEffect, iViewFrustum);
				}

				// Token: 0x04000A93 RID: 2707
				public Matrix WorldOrientation;
			}

			// Token: 0x0200017B RID: 379
			public class DeferredRenderData : RenderableObject<SkinnedModelDeferredEffect, SkinnedModelDeferredBasicMaterial>, IRenderableAdditiveObject
			{
				// Token: 0x06000B96 RID: 2966 RVA: 0x00045D23 File Offset: 0x00043F23
				public DeferredRenderData()
				{
					this.mBones = new Matrix[80];
				}

				// Token: 0x170002C2 RID: 706
				// (get) Token: 0x06000B97 RID: 2967 RVA: 0x00045D38 File Offset: 0x00043F38
				public override int Technique
				{
					get
					{
						if (this.RenderAdditive)
						{
							return 2;
						}
						return base.Technique;
					}
				}

				// Token: 0x06000B98 RID: 2968 RVA: 0x00045D4C File Offset: 0x00043F4C
				public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
				{
					SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
					skinnedModelDeferredEffect.Bones = this.mBones;
					base.Draw(iEffect, iViewFrustum);
				}

				// Token: 0x06000B99 RID: 2969 RVA: 0x00045D74 File Offset: 0x00043F74
				public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
				{
					SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
					skinnedModelDeferredEffect.Bones = this.mBones;
					base.DrawShadow(iEffect, iViewFrustum);
				}

				// Token: 0x04000A94 RID: 2708
				public bool RenderAdditive;

				// Token: 0x04000A95 RID: 2709
				public Matrix[] mBones;
			}

			// Token: 0x0200017C RID: 380
			protected class AfterImageRenderData : IRenderableObject, IRenderableAdditiveObject
			{
				// Token: 0x06000B9A RID: 2970 RVA: 0x00045D9C File Offset: 0x00043F9C
				public AfterImageRenderData(Matrix[][] iSkeleton)
				{
					this.mSkeleton = iSkeleton;
				}

				// Token: 0x170002C3 RID: 707
				// (get) Token: 0x06000B9B RID: 2971 RVA: 0x00045DB2 File Offset: 0x00043FB2
				public bool MeshDirty
				{
					get
					{
						return this.mMeshDirty;
					}
				}

				// Token: 0x170002C4 RID: 708
				// (get) Token: 0x06000B9C RID: 2972 RVA: 0x00045DBA File Offset: 0x00043FBA
				public int Effect
				{
					get
					{
						return this.mEffect;
					}
				}

				// Token: 0x170002C5 RID: 709
				// (get) Token: 0x06000B9D RID: 2973 RVA: 0x00045DC2 File Offset: 0x00043FC2
				public int DepthTechnique
				{
					get
					{
						return 3;
					}
				}

				// Token: 0x170002C6 RID: 710
				// (get) Token: 0x06000B9E RID: 2974 RVA: 0x00045DC5 File Offset: 0x00043FC5
				public int Technique
				{
					get
					{
						return 1;
					}
				}

				// Token: 0x170002C7 RID: 711
				// (get) Token: 0x06000B9F RID: 2975 RVA: 0x00045DC8 File Offset: 0x00043FC8
				public int ShadowTechnique
				{
					get
					{
						return 4;
					}
				}

				// Token: 0x170002C8 RID: 712
				// (get) Token: 0x06000BA0 RID: 2976 RVA: 0x00045DCB File Offset: 0x00043FCB
				public VertexBuffer Vertices
				{
					get
					{
						return this.mVertexBuffer;
					}
				}

				// Token: 0x170002C9 RID: 713
				// (get) Token: 0x06000BA1 RID: 2977 RVA: 0x00045DD3 File Offset: 0x00043FD3
				public IndexBuffer Indices
				{
					get
					{
						return this.mIndexBuffer;
					}
				}

				// Token: 0x170002CA RID: 714
				// (get) Token: 0x06000BA2 RID: 2978 RVA: 0x00045DDB File Offset: 0x00043FDB
				public VertexDeclaration VertexDeclaration
				{
					get
					{
						return this.mVertexDeclaration;
					}
				}

				// Token: 0x170002CB RID: 715
				// (get) Token: 0x06000BA3 RID: 2979 RVA: 0x00045DE3 File Offset: 0x00043FE3
				public int VertexStride
				{
					get
					{
						return this.mVertexStride;
					}
				}

				// Token: 0x170002CC RID: 716
				// (get) Token: 0x06000BA4 RID: 2980 RVA: 0x00045DEB File Offset: 0x00043FEB
				public int VerticesHashCode
				{
					get
					{
						return this.mVerticesHash;
					}
				}

				// Token: 0x06000BA5 RID: 2981 RVA: 0x00045DF4 File Offset: 0x00043FF4
				public bool Cull(BoundingFrustum iViewFrustum)
				{
					BoundingSphere boundingSphere = this.mBoundingSphere;
					return boundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
				}

				// Token: 0x06000BA6 RID: 2982 RVA: 0x00045E14 File Offset: 0x00044014
				public virtual void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
				{
					SkinnedModelBasicEffect skinnedModelBasicEffect = iEffect as SkinnedModelBasicEffect;
					this.mMaterial.AssignToEffect(skinnedModelBasicEffect);
					skinnedModelBasicEffect.Colorize = new Vector4(SummonDeath.MagickDeath.AfterImageRenderData.ColdColor, 1f);
					float num = 0.333f;
					float num2 = 0.333f / ((float)this.mSkeleton.Length + 1f);
					num += this.mIntensity * num2;
					for (int i = 0; i < this.mSkeleton.Length; i++)
					{
						if (num != 0f)
						{
							skinnedModelBasicEffect.Alpha = num;
							skinnedModelBasicEffect.Bones = this.mSkeleton[i];
							skinnedModelBasicEffect.CommitChanges();
							skinnedModelBasicEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
							num -= num2;
						}
					}
					skinnedModelBasicEffect.Colorize = default(Vector4);
				}

				// Token: 0x06000BA7 RID: 2983 RVA: 0x00045EDF File Offset: 0x000440DF
				public virtual void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
				{
				}

				// Token: 0x06000BA8 RID: 2984 RVA: 0x00045EE1 File Offset: 0x000440E1
				public void SetMeshDirty()
				{
					this.mMeshDirty = true;
				}

				// Token: 0x06000BA9 RID: 2985 RVA: 0x00045EEC File Offset: 0x000440EC
				public void SetMesh(VertexBuffer iVertices, IndexBuffer iIndices, ModelMeshPart iMeshPart, int iEffectHash)
				{
					this.mMeshDirty = false;
					SkinnedModelMaterial.CreateFromEffect(iMeshPart.Effect as SkinnedModelBasicEffect, out this.mMaterial);
					this.mVertexBuffer = iVertices;
					this.mVerticesHash = iVertices.GetHashCode();
					this.mIndexBuffer = iIndices;
					this.mEffect = iEffectHash;
					this.mVertexDeclaration = iMeshPart.VertexDeclaration;
					this.mBaseVertex = iMeshPart.BaseVertex;
					this.mNumVertices = iMeshPart.NumVertices;
					this.mPrimitiveCount = iMeshPart.PrimitiveCount;
					this.mStartIndex = iMeshPart.StartIndex;
					this.mStreamOffset = iMeshPart.StreamOffset;
					this.mVertexStride = iMeshPart.VertexStride;
					for (int i = 0; i < this.mSkeleton.Length; i++)
					{
						Matrix[] array = this.mSkeleton[i];
						for (int j = 0; j < array.Length; j++)
						{
							array[j].M11 = (array[j].M22 = (array[j].M33 = (array[j].M44 = float.NaN)));
						}
					}
				}

				// Token: 0x04000A96 RID: 2710
				protected static readonly Vector3 ColdColor = new Vector3(1f, 1.6f, 2f);

				// Token: 0x04000A97 RID: 2711
				public BoundingSphere mBoundingSphere;

				// Token: 0x04000A98 RID: 2712
				protected int mEffect;

				// Token: 0x04000A99 RID: 2713
				protected VertexDeclaration mVertexDeclaration;

				// Token: 0x04000A9A RID: 2714
				protected int mBaseVertex;

				// Token: 0x04000A9B RID: 2715
				protected int mNumVertices;

				// Token: 0x04000A9C RID: 2716
				protected int mPrimitiveCount;

				// Token: 0x04000A9D RID: 2717
				protected int mStartIndex;

				// Token: 0x04000A9E RID: 2718
				protected int mStreamOffset;

				// Token: 0x04000A9F RID: 2719
				protected int mVertexStride;

				// Token: 0x04000AA0 RID: 2720
				protected VertexBuffer mVertexBuffer;

				// Token: 0x04000AA1 RID: 2721
				protected IndexBuffer mIndexBuffer;

				// Token: 0x04000AA2 RID: 2722
				public float mIntensity;

				// Token: 0x04000AA3 RID: 2723
				public Vector3 Color;

				// Token: 0x04000AA4 RID: 2724
				public Matrix[][] mSkeleton;

				// Token: 0x04000AA5 RID: 2725
				private SkinnedModelMaterial mMaterial;

				// Token: 0x04000AA6 RID: 2726
				protected int mVerticesHash;

				// Token: 0x04000AA7 RID: 2727
				protected bool mMeshDirty = true;
			}

			// Token: 0x0200017D RID: 381
			public enum Animations
			{
				// Token: 0x04000AA9 RID: 2729
				Move,
				// Token: 0x04000AAA RID: 2730
				Scythe_Rise,
				// Token: 0x04000AAB RID: 2731
				Scythe_Fall,
				// Token: 0x04000AAC RID: 2732
				Disappear,
				// Token: 0x04000AAD RID: 2733
				NrOfAnimations
			}
		}
	}
}
