using System;
using System.Collections.Generic;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Gamers;
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
	// Token: 0x020000E8 RID: 232
	public class Fafnir : BossStatusEffected, IBoss
	{
		// Token: 0x0600072E RID: 1838 RVA: 0x0002A800 File Offset: 0x00028A00
		public Fafnir(PlayState iPlayState)
		{
			this.mPlayState = iPlayState;
			this.mHitList = new HitList(16);
			SkinnedModel skinnedModel;
			SkinnedModel skinnedModel2;
			lock (Game.Instance.GraphicsDevice)
			{
				skinnedModel = this.mPlayState.Content.Load<SkinnedModel>("Models/Bosses/Fafnir/Fafnir_mesh");
				skinnedModel2 = this.mPlayState.Content.Load<SkinnedModel>("Models/Bosses/Fafnir/Fafnir_animation");
				this.mDebrisModel = this.mPlayState.Content.Load<Model>("Models/Bosses/Fafnir/Ceiling_Debri");
			}
			this.mTailIndices = new int[this.mNrOfTailBones];
			this.mTailBindPoses = new Matrix[this.mNrOfTailBones];
			this.mNeckIndices = new int[this.mNrOfNeckBones + 1];
			this.mNeckBindPoses = new Matrix[this.mNrOfNeckBones + 1];
			Matrix orient;
			Matrix.CreateRotationY(3.1415927f, out orient);
			for (int i = 0; i < skinnedModel.SkeletonBones.Count; i++)
			{
				SkinnedModelBone skinnedModelBone = skinnedModel.SkeletonBones[i];
				if (skinnedModelBone.Name.Equals("Head", StringComparison.OrdinalIgnoreCase))
				{
					this.mHeadIndex = (int)skinnedModelBone.Index;
					this.mHeadBindPose = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mHeadBindPose, ref orient, out this.mHeadBindPose);
					Matrix.Invert(ref this.mHeadBindPose, out this.mHeadBindPose);
					this.mNeckIndices[this.mNrOfNeckBones] = (int)skinnedModelBone.Index;
					this.mNeckBindPoses[this.mNrOfNeckBones] = this.mHeadBindPose;
				}
				else if (skinnedModelBone.Name.Equals("Mouth", StringComparison.OrdinalIgnoreCase))
				{
					this.mMouthIndex = (int)skinnedModelBone.Index;
					this.mMouthBindPose = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mMouthBindPose, ref orient, out this.mMouthBindPose);
					Matrix.Invert(ref this.mMouthBindPose, out this.mMouthBindPose);
				}
				else if (skinnedModelBone.Name.Equals("RightEye", StringComparison.OrdinalIgnoreCase))
				{
					this.mRightEyeIndex = (int)skinnedModelBone.Index;
					this.mRightEyeBindPose = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mRightEyeBindPose, ref orient, out this.mRightEyeBindPose);
					Matrix.Invert(ref this.mRightEyeBindPose, out this.mRightEyeBindPose);
				}
				else if (skinnedModelBone.Name.Equals("LeftEye", StringComparison.OrdinalIgnoreCase))
				{
					this.mLeftEyeIndex = (int)skinnedModelBone.Index;
					this.mLeftEyeBindPose = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mLeftEyeBindPose, ref orient, out this.mLeftEyeBindPose);
					Matrix.Invert(ref this.mLeftEyeBindPose, out this.mLeftEyeBindPose);
				}
				else if (skinnedModelBone.Name.Equals("SpineUpper", StringComparison.OrdinalIgnoreCase))
				{
					this.mSpineUpperIndex = (int)skinnedModelBone.Index;
					this.mSpineUpperBindPose = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mSpineUpperBindPose, ref orient, out this.mSpineUpperBindPose);
					Matrix.Invert(ref this.mSpineUpperBindPose, out this.mSpineUpperBindPose);
				}
				else if (skinnedModelBone.Name.Equals("SpineMid", StringComparison.OrdinalIgnoreCase))
				{
					this.mSpineMidIndex = (int)skinnedModelBone.Index;
					this.mSpineMidBindPose = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mSpineMidBindPose, ref orient, out this.mSpineMidBindPose);
					Matrix.Invert(ref this.mSpineMidBindPose, out this.mSpineMidBindPose);
				}
				else if (skinnedModelBone.Name.Equals("SpineBase", StringComparison.OrdinalIgnoreCase))
				{
					this.mSpineBaseIndex = (int)skinnedModelBone.Index;
					this.mSpineBaseBindPose = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mSpineBaseBindPose, ref orient, out this.mSpineBaseBindPose);
					Matrix.Invert(ref this.mSpineBaseBindPose, out this.mSpineBaseBindPose);
				}
				else if (skinnedModelBone.Name.Equals("RightHip", StringComparison.OrdinalIgnoreCase))
				{
					this.mRightHipIndex = (int)skinnedModelBone.Index;
					this.mRightHipBindPose = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mRightHipBindPose, ref orient, out this.mRightHipBindPose);
					Matrix.Invert(ref this.mRightHipBindPose, out this.mRightHipBindPose);
				}
				else if (skinnedModelBone.Name.Equals("RightHeel", StringComparison.OrdinalIgnoreCase))
				{
					this.mRightHeelIndex = (int)skinnedModelBone.Index;
					this.mRightHeelBindPose = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mRightHeelBindPose, ref orient, out this.mRightHeelBindPose);
					Matrix.Invert(ref this.mRightHeelBindPose, out this.mRightHeelBindPose);
				}
				else if (skinnedModelBone.Name.Equals("RightIndextoeEnd", StringComparison.OrdinalIgnoreCase))
				{
					this.mRightToeIndex = (int)skinnedModelBone.Index;
					this.mRightToeBindPose = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mRightToeBindPose, ref orient, out this.mRightToeBindPose);
					Matrix.Invert(ref this.mRightToeBindPose, out this.mRightToeBindPose);
				}
				else if (skinnedModelBone.Name.Equals("RightShoulder", StringComparison.OrdinalIgnoreCase))
				{
					this.mRightShoulderIndex = (int)skinnedModelBone.Index;
					this.mRightShoulderBindPose = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mRightShoulderBindPose, ref orient, out this.mRightShoulderBindPose);
					Matrix.Invert(ref this.mRightShoulderBindPose, out this.mRightShoulderBindPose);
				}
				else if (skinnedModelBone.Name.Equals("RightWrist", StringComparison.OrdinalIgnoreCase))
				{
					this.mRightWristIndex = (int)skinnedModelBone.Index;
					this.mRightWristBindPose = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mRightWristBindPose, ref orient, out this.mRightWristBindPose);
					Matrix.Invert(ref this.mRightWristBindPose, out this.mRightWristBindPose);
				}
				else
				{
					for (int j = 0; j < this.mTailIndices.Length; j++)
					{
						string value = string.Format("Tail{0}", j + 1);
						if (skinnedModelBone.Name.Equals(value, StringComparison.OrdinalIgnoreCase))
						{
							this.mTailIndices[j] = (int)skinnedModelBone.Index;
							this.mTailBindPoses[j] = skinnedModelBone.InverseBindPoseTransform;
							Matrix.Multiply(ref this.mTailBindPoses[j], ref orient, out this.mTailBindPoses[j]);
							Matrix.Invert(ref this.mTailBindPoses[j], out this.mTailBindPoses[j]);
							break;
						}
					}
					for (int k = 0; k < this.mNrOfNeckBones; k++)
					{
						string value2 = string.Format("Neck{0}", k + 1);
						if (skinnedModelBone.Name.Equals(value2, StringComparison.OrdinalIgnoreCase))
						{
							this.mNeckIndices[k] = (int)skinnedModelBone.Index;
							this.mNeckBindPoses[k] = skinnedModelBone.InverseBindPoseTransform;
							Matrix.Multiply(ref this.mNeckBindPoses[k], ref orient, out this.mNeckBindPoses[k]);
							Matrix.Invert(ref this.mNeckBindPoses[k], out this.mNeckBindPoses[k]);
							break;
						}
					}
				}
			}
			this.mAnimationController = new AnimationController();
			this.mAnimationController.Skeleton = skinnedModel.SkeletonBones;
			this.mAnimationClips = new AnimationClip[12];
			this.mAnimationClips[8] = skinnedModel2.AnimationClips["intro"];
			this.mAnimationClips[7] = skinnedModel2.AnimationClips["idle"];
			this.mAnimationClips[1] = skinnedModel2.AnimationClips["ceiling"];
			this.mAnimationClips[2] = skinnedModel2.AnimationClips["charm"];
			this.mAnimationClips[3] = skinnedModel2.AnimationClips["defeated"];
			this.mAnimationClips[5] = skinnedModel2.AnimationClips["fire_high"];
			this.mAnimationClips[6] = skinnedModel2.AnimationClips["fire_low"];
			this.mAnimationClips[4] = skinnedModel2.AnimationClips["fireball"];
			this.mAnimationClips[9] = skinnedModel2.AnimationClips["sleep"];
			this.mAnimationClips[10] = skinnedModel2.AnimationClips["tailwhip"];
			this.mAnimationClips[11] = skinnedModel2.AnimationClips["wings"];
			Matrix orient2;
			Matrix.CreateRotationZ(-3.1415927f, out orient2);
			this.mHeadZone = new BossDamageZone(this.mPlayState, this, 0, 3f, new Capsule(Vector3.Forward, orient2, 1f, 3f));
			float length = Vector3.Distance(this.mSpineBaseBindPose.Translation, this.mSpineUpperBindPose.Translation);
			float radius = 3.2f;
			this.mBodyZone = new BossDamageZone(this.mPlayState, this, 1, 3.2f, new Capsule(Vector3.Zero, Matrix.Identity, radius, length));
			this.mBodyZone.Body.CollisionSkin.callbackFn += this.OnCollision;
			this.mRightHeelZone = new BossDamageZone(this.mPlayState, this, 2, 2f, new Capsule(Vector3.Zero, orient, 1f, 2f));
			this.mRightHeelZone.Body.CollisionSkin.callbackFn += this.OnCollision;
			this.mRightWristZone = new BossDamageZone(this.mPlayState, this, 2, 2f, new Capsule(Vector3.Zero, orient, 1f, 2f));
			this.mRightWristZone.Body.CollisionSkin.callbackFn += this.OnCollision;
			Primitive[] array = new Primitive[this.mNrOfTailBones];
			for (int l = 0; l < array.Length - 1; l++)
			{
				float num = Math.Max((float)(array.Length - l) * 0.15f, 0.333f);
				float num2 = Vector3.Distance(this.mTailBindPoses[l].Translation, this.mTailBindPoses[l + 1].Translation);
				num2 = Math.Max(num2 - num, 0f);
				array[l] = new Capsule(Vector3.Zero, Matrix.Identity, num, num2);
			}
			array[this.mNrOfTailBones - 1] = new Capsule(Vector3.Zero, Matrix.Identity, 0.333f, 0.333f);
			this.mTailZone = new BossDamageZone(this.mPlayState, this, 3, 15f, array);
			this.mTailZone.Body.CollisionSkin.callbackFn += this.OnTailCollision;
			ModelMesh modelMesh = skinnedModel.Model.Meshes[0];
			ModelMeshPart modelMeshPart = modelMesh.MeshParts[0];
			this.mBoundingSphere = modelMesh.BoundingSphere;
			SkinnedModelDeferredBasicMaterial mMaterial;
			Helper.SkinnedModelDeferredMaterialFromBasicEffect(modelMeshPart.Effect as SkinnedModelBasicEffect, out mMaterial);
			this.mRenderData = new Fafnir.RenderData[3];
			for (int m = 0; m < 3; m++)
			{
				this.mRenderData[m] = new Fafnir.RenderData();
				this.mRenderData[m].SetMesh(modelMesh.VertexBuffer, modelMesh.IndexBuffer, modelMeshPart, 0, 3, 4);
				this.mRenderData[m].mMaterial = mMaterial;
			}
			this.mSleepState = new Fafnir.SleepState();
			this.mDecisionState = new Fafnir.DecisionState();
			this.mIntroState = new Fafnir.IntroState();
			this.mIdleState = new Fafnir.IdleState();
			this.mDefeatedState = new Fafnir.DefeatedState();
			this.mConfuseState = new Fafnir.ConfuseState();
			this.mWingState = new Fafnir.WingState();
			this.mTailState = new Fafnir.TailState();
			this.mFireballState = new Fafnir.FireballState();
			this.mFirelanceHighState = new Fafnir.FirelanceHighState();
			this.mFirelanceLowState = new Fafnir.FirelanceLowState();
			this.mCeilingState = new Fafnir.CeilingState();
			this.mEarthQuakeState = new Fafnir.EarthQuakeState();
			Texture2D texture = this.mPlayState.Content.Load<Texture2D>("EffectTextures/FireLance02");
			VertexPositionColorTexture[] array2 = new VertexPositionColorTexture[6];
			float x = 0.5f;
			float y = 1f;
			array2[0].TextureCoordinate = new Vector2(0f, y);
			array2[0].Position = new Vector3(-0.5f, 0f, 1f);
			array2[0].Color = Color.White;
			array2[1].TextureCoordinate = new Vector2(0f, 0f);
			array2[1].Position = new Vector3(-0.5f, 0f, 0f);
			array2[1].Color = Color.White;
			array2[2].TextureCoordinate = new Vector2(x, 0f);
			array2[2].Position = new Vector3(0.5f, 0f, 0f);
			array2[2].Color = Color.White;
			array2[3].TextureCoordinate = new Vector2(x, 0f);
			array2[3].Position = new Vector3(0.5f, 0f, 0f);
			array2[3].Color = Color.White;
			array2[4].TextureCoordinate = new Vector2(x, y);
			array2[4].Position = new Vector3(0.5f, 0f, 1f);
			array2[4].Color = Color.White;
			array2[5].TextureCoordinate = new Vector2(0f, y);
			array2[5].Position = new Vector3(-0.5f, 0f, 1f);
			array2[5].Color = Color.White;
			VertexBuffer vertexBuffer;
			VertexDeclaration iDeclaration;
			lock (Game.Instance.GraphicsDevice)
			{
				vertexBuffer = new VertexBuffer(Game.Instance.GraphicsDevice, array2.Length * VertexPositionColorTexture.SizeInBytes, BufferUsage.WriteOnly);
				vertexBuffer.SetData<VertexPositionColorTexture>(array2);
				iDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, VertexPositionColorTexture.VertexElements);
			}
			vertexBuffer.Name = "FireLanceBuffer";
			AdditiveMaterial mMaterial2 = default(AdditiveMaterial);
			lock (Game.Instance.GraphicsDevice)
			{
				mMaterial2.Texture = texture;
			}
			mMaterial2.TextureEnabled = true;
			mMaterial2.VertexColorEnabled = false;
			mMaterial2.ColorTint = new Vector4(1f, 1f, 1f, 1f);
			this.mFireRenderData = new Fafnir.FireRenderData[3];
			for (int n = 0; n < 3; n++)
			{
				this.mFireRenderData[n] = new Fafnir.FireRenderData(vertexBuffer, iDeclaration);
				this.mFireRenderData[n].mMaterial = mMaterial2;
			}
		}

		// Token: 0x0600072F RID: 1839 RVA: 0x0002B684 File Offset: 0x00029884
		protected bool OnTailCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
		{
			if (this.Dead && iSkin1.Owner != null)
			{
				return false;
			}
			if (iSkin1.Owner != null && this.mCurrentState is Fafnir.TailState)
			{
				float num = this.mAnimationController.Time / this.mAnimationController.AnimationClip.Duration;
				if (num >= Fafnir.ANIMATION_TIMES[10][0] && num <= Fafnir.ANIMATION_TIMES[10][1] && !this.mAnimationController.CrossFadeEnabled)
				{
					IDamageable damageable = iSkin1.Owner.Tag as IDamageable;
					if (damageable != null && !this.mHitList.ContainsKey(damageable.Handle))
					{
						Vector3 vector;
						damageable.Position.Z = vector.Z - 4f;
						Vector3 position = (iSkin0.GetPrimitiveNewWorld(iPrim0) as Capsule).Position;
						this.mHitList.Add(damageable.Handle, 1f);
						damageable.Damage(Fafnir.sTailDamage, this.mTailZone, 0.0, position);
					}
				}
			}
			return true;
		}

		// Token: 0x06000730 RID: 1840 RVA: 0x0002B791 File Offset: 0x00029991
		protected bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
		{
			return true;
		}

		// Token: 0x06000731 RID: 1841 RVA: 0x0002B794 File Offset: 0x00029994
		public void Initialize(ref Matrix iOrientation, int iUniqueID)
		{
			this.Initialize(ref iOrientation);
		}

		// Token: 0x06000732 RID: 1842 RVA: 0x0002B7A0 File Offset: 0x000299A0
		public void Initialize(ref Matrix iOrientation)
		{
			this.mOrientation = iOrientation;
			this.mBoundingSphere.Center = this.mOrientation.Translation;
			Segment iSeg = default(Segment);
			iSeg.Origin = iOrientation.Translation;
			iSeg.Delta.Y = -10f;
			float num;
			Vector3 translation;
			Vector3 vector;
			if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out num, out translation, out vector, iSeg))
			{
				translation.Y -= 0.333f;
				this.mOrientation.Translation = translation;
			}
			this.mHeadZone.Initialize("#boss_n14".GetHashCodeCustom());
			this.mHeadZone.Body.CollisionSkin.NonCollidables.Add(this.mBodyZone.Body.CollisionSkin);
			this.mHeadZone.Body.CollisionSkin.NonCollidables.Add(this.mRightHeelZone.Body.CollisionSkin);
			this.mHeadZone.Body.CollisionSkin.NonCollidables.Add(this.mRightWristZone.Body.CollisionSkin);
			this.mHeadZone.Body.CollisionSkin.NonCollidables.Add(this.mTailZone.Body.CollisionSkin);
			this.mHeadZone.Body.CollisionSkin.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
			this.mPlayState.EntityManager.AddEntity(this.mHeadZone);
			this.mBodyZone.Initialize();
			this.mBodyZone.Body.CollisionSkin.NonCollidables.Add(this.mRightHeelZone.Body.CollisionSkin);
			this.mBodyZone.Body.CollisionSkin.NonCollidables.Add(this.mRightWristZone.Body.CollisionSkin);
			this.mBodyZone.Body.CollisionSkin.NonCollidables.Add(this.mTailZone.Body.CollisionSkin);
			this.mBodyZone.Body.CollisionSkin.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
			this.mPlayState.EntityManager.AddEntity(this.mBodyZone);
			this.mRightWristZone.Initialize();
			this.mRightWristZone.Body.CollisionSkin.NonCollidables.Add(this.mRightHeelZone.Body.CollisionSkin);
			this.mRightWristZone.Body.CollisionSkin.NonCollidables.Add(this.mTailZone.Body.CollisionSkin);
			this.mRightWristZone.Body.CollisionSkin.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
			this.mPlayState.EntityManager.AddEntity(this.mRightWristZone);
			this.mRightHeelZone.Initialize();
			this.mRightHeelZone.Body.CollisionSkin.NonCollidables.Add(this.mTailZone.Body.CollisionSkin);
			this.mRightHeelZone.Body.CollisionSkin.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
			this.mPlayState.EntityManager.AddEntity(this.mRightHeelZone);
			this.mTailZone.Initialize();
			this.mTailZone.Body.CollisionSkin.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
			this.mPlayState.EntityManager.AddEntity(this.mTailZone);
			this.mCurrentState = this.mSleepState;
			this.mCurrentState.OnEnter(this);
			this.mMaxHitPoints = 35000f;
			this.mHitPoints = this.mMaxHitPoints;
			this.mDead = false;
			this.mAimForTarget = false;
			this.mLanceTime = 0f;
			this.mDrawFireLance = false;
			this.mPlayers = Game.Instance.Players;
			this.mHitList.Clear();
			for (int i = 0; i < this.mStatusEffects.Length; i++)
			{
				this.mStatusEffects[i].Stop();
				this.mStatusEffects[i] = default(StatusEffect);
			}
			this.mCurrentStatusEffects = StatusEffects.None;
			int j = 0;
			while (j < this.mResistances.Length)
			{
				this.mResistances[j].ResistanceAgainst = Spell.ElementFromIndex(j);
				Elements resistanceAgainst = this.mResistances[j].ResistanceAgainst;
				switch (resistanceAgainst)
				{
				case Elements.Water:
					this.mResistances[j].Modifier = 0f;
					this.mResistances[j].Multiplier = 0f;
					break;
				case Elements.Earth | Elements.Water:
					goto IL_557;
				case Elements.Cold:
					this.mResistances[j].Modifier = 10f;
					this.mResistances[j].Multiplier = 2f;
					break;
				default:
					if (resistanceAgainst != Elements.Fire)
					{
						goto IL_557;
					}
					this.mResistances[j].Modifier = 0f;
					this.mResistances[j].Multiplier = -3f;
					break;
				}
				IL_585:
				j++;
				continue;
				IL_557:
				this.mResistances[j].Modifier = 0f;
				this.mResistances[j].Multiplier = 1f;
				goto IL_585;
			}
			for (int k = 0; k < Fafnir.LEVEL_PARTS.Length; k++)
			{
				int iId = Fafnir.LEVEL_PARTS[k];
				AnimatedLevelPart animatedLevelPart = this.mPlayState.Level.CurrentScene.LevelModel.GetAnimatedLevelPart(iId);
				animatedLevelPart.Play(true, -1f, -1f, 1f, false, false);
				animatedLevelPart.Stop(true);
			}
			this.mNrOfEarthquakes = 0;
			this.SetEarthQuakeThreshold();
			this.mDamageFlashTimer = 0f;
		}

		// Token: 0x06000733 RID: 1843 RVA: 0x0002BDC0 File Offset: 0x00029FC0
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
			if (iFightStarted && this.mCurrentState is Fafnir.SleepState)
			{
				this.ChangeState(Fafnir.State.Intro);
			}
			else if (this.mHitPoints <= 1f && this.mCurrentState != this.mDefeatedState)
			{
				this.ChangeState(Fafnir.State.Defeated);
			}
			if (this.mConfuseState.mInverted)
			{
				this.mConfusedTimer -= iDeltaTime;
				if (this.mConfusedTimer <= 0f)
				{
					Player[] players = Game.Instance.Players;
					for (int i = 0; i < players.Length; i++)
					{
						if (players[i].Playing && !(players[i].Gamer is NetworkGamer))
						{
							players[i].Controller.Invert(false);
						}
					}
					this.mConfuseState.mInverted = false;
				}
			}
			Matrix identity = Matrix.Identity;
			Matrix.Multiply(ref identity, ref this.mRenderRotationOffset, out identity);
			Matrix.Multiply(ref identity, ref this.mOrientation, out identity);
			this.mAnimationController.Update(iDeltaTime, ref identity, true);
			if (this.mAimForTarget)
			{
				this.mAimForTargetWeight = Math.Min(this.mAimForTargetWeight + iDeltaTime, 1f);
			}
			else
			{
				this.mAimForTargetWeight = Math.Max(this.mAimForTargetWeight - iDeltaTime, 0f);
			}
			if (this.mTarget != null)
			{
				Vector3 position = this.mTarget.Position;
				this.mAimTargetPosition.X = this.mAimTargetPosition.X + (position.X - this.mAimTargetPosition.X) * iDeltaTime;
				this.mAimTargetPosition.Y = this.mAimTargetPosition.Y + (position.Y - this.mAimTargetPosition.Y) * iDeltaTime;
				this.mAimTargetPosition.Z = this.mAimTargetPosition.Z + (position.Z - this.mAimTargetPosition.Z) * iDeltaTime;
			}
			Matrix matrix2;
			if (this.mAimForTargetWeight > 1E-45f)
			{
				Matrix matrix;
				Matrix.CreateRotationY(3.1415927f, out matrix);
				Vector3 vector = this.mAimTargetPosition;
				for (int j = 0; j < this.mNeckIndices.Length; j++)
				{
					matrix2 = this.mMouthBindPose;
					Matrix.Multiply(ref matrix2, ref this.mAnimationController.SkinnedBoneTransforms[this.mMouthIndex], out matrix2);
					Vector3 translation = matrix2.Translation;
					Vector3 forward = matrix2.Forward;
					Vector3 vector2;
					Vector3.Subtract(ref vector, ref translation, out vector2);
					Vector3 vector3 = vector2;
					vector3.Y = 0f;
					vector3.Normalize();
					Vector3 vector4 = forward;
					vector4.Y = 0f;
					vector4.Normalize();
					float num;
					Vector3.Dot(ref vector4, ref vector3, out num);
					num = MathHelper.Clamp(num, -1f, 1f);
					Vector3 vector5;
					Vector3.Cross(ref vector4, ref vector3, out vector5);
					int num2 = this.mNeckIndices[j];
					if (vector5.LengthSquared() > 1E-06f)
					{
						float num3 = (float)Math.Acos((double)num) * this.mAimForTargetWeight;
						float angle = num3 / (float)(this.mNeckIndices.Length - j);
						Matrix inverseBindPoseTransform = this.mAnimationController.Skeleton[num2].InverseBindPoseTransform;
						Matrix.Invert(ref inverseBindPoseTransform, out inverseBindPoseTransform);
						Matrix.Multiply(ref inverseBindPoseTransform, ref this.mAnimationController.SkinnedBoneTransforms[num2], out inverseBindPoseTransform);
						Matrix.Invert(ref inverseBindPoseTransform, out inverseBindPoseTransform);
						Vector3.TransformNormal(ref vector5, ref inverseBindPoseTransform, out vector5);
						vector5.Normalize();
						Quaternion quaternion;
						Quaternion.CreateFromAxisAngle(ref vector5, angle, out quaternion);
						Quaternion.Concatenate(ref quaternion, ref this.mAnimationController.LocalBonePoses[num2].Orientation, out this.mAnimationController.LocalBonePoses[num2].Orientation);
						this.mAnimationController.UpdateAbsoluteBoneTransformsFrom(num2);
					}
					if (j == this.mNeckIndices.Length - 1)
					{
						matrix2 = this.mMouthBindPose;
						Matrix.Multiply(ref matrix2, ref this.mAnimationController.SkinnedBoneTransforms[this.mMouthIndex], out matrix2);
						translation = matrix2.Translation;
						forward = matrix2.Forward;
						Vector3.Subtract(ref vector, ref translation, out vector2);
						vector2.Normalize();
						Vector3.Cross(ref forward, ref vector2, out vector5);
						if (vector5.LengthSquared() > 1E-06f)
						{
							Vector3.Dot(ref forward, ref vector2, out num);
							num = MathHelper.Clamp(num, -1f, 1f);
							float num3 = (float)Math.Acos((double)num) * this.mAimForTargetWeight;
							Matrix inverseBindPoseTransform = this.mAnimationController.Skeleton[num2].InverseBindPoseTransform;
							Matrix.Invert(ref inverseBindPoseTransform, out inverseBindPoseTransform);
							Matrix.Multiply(ref inverseBindPoseTransform, ref this.mAnimationController.SkinnedBoneTransforms[num2], out inverseBindPoseTransform);
							Matrix.Invert(ref inverseBindPoseTransform, out inverseBindPoseTransform);
							Vector3.TransformNormal(ref vector5, ref inverseBindPoseTransform, out vector5);
							vector5.Normalize();
							Quaternion quaternion;
							Quaternion.CreateFromAxisAngle(ref vector5, num3, out quaternion);
							Quaternion.Concatenate(ref quaternion, ref this.mAnimationController.LocalBonePoses[num2].Orientation, out this.mAnimationController.LocalBonePoses[num2].Orientation);
							this.mAnimationController.UpdateAbsoluteBoneTransformsFrom(num2);
						}
					}
				}
			}
			matrix2 = this.mRightEyeBindPose;
			Matrix.Multiply(ref matrix2, ref this.mAnimationController.SkinnedBoneTransforms[this.mRightEyeIndex], out this.mRightEyeOrientation);
			matrix2 = this.mLeftEyeBindPose;
			Matrix.Multiply(ref matrix2, ref this.mAnimationController.SkinnedBoneTransforms[this.mLeftEyeIndex], out this.mLeftEyeOrientation);
			matrix2 = this.mMouthBindPose;
			Matrix.Multiply(ref matrix2, ref this.mAnimationController.SkinnedBoneTransforms[this.mMouthIndex], out this.mMouthOrientation);
			Array.Copy(this.mAnimationController.SkinnedBoneTransforms, 0, this.mRenderData[(int)iDataChannel].mSkeleton, 0, this.mAnimationController.Skeleton.Count);
			this.mDamageFlashTimer = Math.Max(this.mDamageFlashTimer - iDeltaTime, 0f);
			this.mRenderData[(int)iDataChannel].mBoundingSphere = this.mBoundingSphere;
			this.mRenderData[(int)iDataChannel].mDamage = 1f - this.mHitPoints * 2.8571429E-05f;
			this.mRenderData[(int)iDataChannel].Flash = this.mDamageFlashTimer * 10f;
			this.mPlayState.Scene.AddRenderableObject(iDataChannel, this.mRenderData[(int)iDataChannel]);
			this.mHitList.Update(iDeltaTime);
			Transform identity2 = Transform.Identity;
			Vector3 up = Vector3.Up;
			Vector3 zero = Vector3.Zero;
			Vector3 translation2;
			Vector3 translation3;
			Vector3 forward2;
			for (int k = 0; k < this.mNrOfTailBones - 1; k++)
			{
				translation2 = this.mTailBindPoses[k].Translation;
				Vector3.Transform(ref translation2, ref this.mAnimationController.SkinnedBoneTransforms[this.mTailIndices[k]], out translation2);
				if (k == 0)
				{
					this.mSpineBasePosition = translation2;
				}
				translation3 = this.mTailBindPoses[k + 1].Translation;
				Vector3.Transform(ref translation3, ref this.mAnimationController.SkinnedBoneTransforms[this.mTailIndices[k + 1]], out translation3);
				Vector3.Subtract(ref translation2, ref translation3, out forward2);
				forward2.Normalize();
				Matrix.CreateWorld(ref zero, ref forward2, ref up, out identity2.Orientation);
				identity2.Position = translation2;
				this.mTailZone.Body.CollisionSkin.GetPrimitiveLocal(k).SetTransform(ref identity2);
				this.mTailZone.Body.CollisionSkin.GetPrimitiveNewWorld(k).SetTransform(ref identity2);
				this.mTailZone.Body.CollisionSkin.GetPrimitiveOldWorld(k).SetTransform(ref identity2);
			}
			translation2 = this.mTailBindPoses[this.mNrOfTailBones - 1].Translation;
			Vector3.Transform(ref translation2, ref this.mAnimationController.SkinnedBoneTransforms[this.mTailIndices[this.mNrOfTailBones - 2]], out translation2);
			translation3 = this.mTailBindPoses[this.mNrOfTailBones - 2].Translation;
			Vector3.Transform(ref translation3, ref this.mAnimationController.SkinnedBoneTransforms[this.mTailIndices[this.mNrOfTailBones - 1]], out translation3);
			Vector3.Subtract(ref translation2, ref translation3, out forward2);
			forward2.Normalize();
			Matrix.CreateWorld(ref zero, ref forward2, ref up, out identity2.Orientation);
			identity2.Position = translation2;
			this.mTailZone.Body.CollisionSkin.GetPrimitiveLocal(this.mNrOfTailBones - 1).SetTransform(ref identity2);
			this.mTailZone.Body.CollisionSkin.GetPrimitiveOldWorld(this.mNrOfTailBones - 1).SetTransform(ref identity2);
			this.mTailZone.Body.CollisionSkin.GetPrimitiveNewWorld(this.mNrOfTailBones - 1).SetTransform(ref identity2);
			this.mTailZone.Body.CollisionSkin.UpdateWorldBoundingBox();
			matrix2 = this.mMouthOrientation;
			Vector3 translation4 = this.mMouthOrientation.Translation;
			matrix2.Translation = default(Vector3);
			this.mHeadZone.SetOrientation(ref translation4, ref matrix2);
			matrix2 = this.mRightHeelBindPose;
			Matrix.Multiply(ref matrix2, ref this.mAnimationController.SkinnedBoneTransforms[this.mRightHeelIndex], out matrix2);
			translation2 = matrix2.Translation;
			matrix2 = this.mRightToeBindPose;
			Matrix.Multiply(ref matrix2, ref this.mAnimationController.SkinnedBoneTransforms[this.mRightToeIndex], out matrix2);
			translation3 = matrix2.Translation;
			Vector3.Subtract(ref translation3, ref translation2, out forward2);
			forward2.Normalize();
			Matrix.CreateWorld(ref zero, ref forward2, ref up, out matrix2);
			this.mRightHeelZone.SetOrientation(ref translation2, ref matrix2);
			matrix2 = this.mRightWristBindPose;
			Matrix.Multiply(ref matrix2, ref this.mAnimationController.SkinnedBoneTransforms[this.mRightWristIndex], out matrix2);
			translation4 = matrix2.Translation;
			matrix2.Translation = default(Vector3);
			this.mRightWristZone.SetOrientation(ref translation4, ref matrix2);
			matrix2 = this.mSpineBaseBindPose;
			translation4 = matrix2.Translation;
			translation4.Y -= 1f;
			matrix2.Translation = translation4;
			Matrix.Multiply(ref matrix2, ref this.mAnimationController.SkinnedBoneTransforms[this.mSpineBaseIndex], out matrix2);
			translation4 = matrix2.Translation;
			matrix2.Translation = default(Vector3);
			this.mBodyZone.SetOrientation(ref translation4, ref matrix2);
			this.mCurrentState.OnUpdate(iDeltaTime, this);
			if (this.mDrawFireLance)
			{
				this.mLanceTime -= iDeltaTime;
				Vector3 translation5 = this.mMouthOrientation.Translation;
				this.mLanceSegment.Origin = translation5;
				float num4 = 30f * this.mLanceScale;
				forward2 = this.mHeadZone.Body.Orientation.Forward;
				Vector3.Multiply(ref forward2, num4, out this.mLanceSegment.Delta);
				Vector3 position2 = this.mAimTargetPosition;
				bool flag = false;
				List<Shield> shields = this.mPlayState.EntityManager.Shields;
				for (int l = 0; l < shields.Count; l++)
				{
					if (shields[l].SegmentIntersect(out position2, this.mLanceSegment, 0.5f))
					{
						flag = true;
						Vector3.Subtract(ref position2, ref this.mLanceSegment.Origin, out this.mLanceSegment.Delta);
						num4 = this.mLanceSegment.Delta.Length();
						break;
					}
				}
				if (!flag)
				{
					float num5;
					Vector3 vector6;
					Vector3 vector7;
					if (this.mPlayState.Level.CurrentScene.CollisionSkin.SegmentIntersect(out num5, out vector6, out vector7, this.mLanceSegment))
					{
						position2 = vector6;
						Vector3.Subtract(ref position2, ref this.mLanceSegment.Origin, out this.mLanceSegment.Delta);
						Vector3.Distance(ref translation5, ref position2, out num4);
					}
					else
					{
						foreach (AnimatedLevelPart animatedLevelPart in this.mPlayState.Level.CurrentScene.LevelModel.AnimatedLevelParts.Values)
						{
							if (animatedLevelPart.CollisionSkin.SegmentIntersect(out num5, out vector6, out vector7, this.mLanceSegment))
							{
								position2 = vector6;
								Vector3.Subtract(ref position2, ref this.mLanceSegment.Origin, out this.mLanceSegment.Delta);
								Vector3.Distance(ref translation5, ref position2, out num4);
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							Vector3.Add(ref this.mLanceSegment.Delta, ref translation5, out position2);
						}
					}
				}
				this.mLanceAudioEmitter.Position = position2;
				this.mLanceAudioEmitter.Up = Vector3.Up;
				this.mLanceAudioEmitter.Forward = Vector3.UnitZ;
				Vector3 position3 = this.mPlayState.Camera.Position;
				Vector3 vector8;
				Vector3.Subtract(ref position3, ref translation5, out vector8);
				Vector3.Negate(ref forward2, out forward2);
				Vector3 right;
				Vector3.Cross(ref forward2, ref vector8, out right);
				Vector3.Normalize(ref right, out right);
				Vector3.Cross(ref right, ref forward2, out up);
				Vector3.Normalize(ref up, out up);
				Matrix matrix3;
				Matrix.CreateScale(1f, 1f, num4, out matrix3);
				identity.Forward = forward2;
				identity.Up = up;
				identity.Right = right;
				Matrix.Multiply(ref matrix3, ref identity, out identity);
				identity.Translation = translation5;
				this.mLanceScale = Math.Max(Math.Min(this.mLanceScale, 1f), 0f);
				this.mFireRenderData[(int)iDataChannel].mTransform = identity;
				this.mFireRenderData[(int)iDataChannel].mScroll = this.mLanceTime;
				this.mFireRenderData[(int)iDataChannel].mSize = num4;
				this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, this.mFireRenderData[(int)iDataChannel]);
			}
			this.UpdateStatusEffects(iDeltaTime);
			this.UpdateDamage(iDeltaTime);
		}

		// Token: 0x06000734 RID: 1844 RVA: 0x0002CB30 File Offset: 0x0002AD30
		private IBossState<Fafnir> GetState(Fafnir.State iState)
		{
			switch (iState)
			{
			case Fafnir.State.Sleep:
				return this.mSleepState;
			case Fafnir.State.Intro:
				return this.mIntroState;
			case Fafnir.State.Idle:
				return this.mIdleState;
			case Fafnir.State.Decision:
				return this.mDecisionState;
			case Fafnir.State.Tail:
				return this.mTailState;
			case Fafnir.State.Wing:
				return this.mWingState;
			case Fafnir.State.Confuse:
				return this.mConfuseState;
			case Fafnir.State.Fireball:
				return this.mFireballState;
			case Fafnir.State.LanceHigh:
				return this.mFirelanceHighState;
			case Fafnir.State.LanceLow:
				return this.mFirelanceLowState;
			case Fafnir.State.Ceiling:
				return this.mCeilingState;
			case Fafnir.State.EarthQuake:
				return this.mEarthQuakeState;
			case Fafnir.State.Defeated:
				return this.mDefeatedState;
			default:
				return null;
			}
		}

		// Token: 0x06000735 RID: 1845 RVA: 0x0002CBD8 File Offset: 0x0002ADD8
		public unsafe void ChangeState(Fafnir.State iState)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				IBossState<Fafnir> state = this.GetState(iState);
				if (state != null)
				{
					if (NetworkManager.Instance.State == NetworkState.Server)
					{
						Fafnir.ChangeStateMessage changeStateMessage;
						changeStateMessage.NewState = iState;
						BossFight.Instance.SendMessage<Fafnir.ChangeStateMessage>(this, 1, (void*)(&changeStateMessage), true);
					}
					this.mCurrentState.OnExit(this);
					this.mPreviousState = this.mCurrentState;
					this.mCurrentState = state;
					this.mCurrentState.OnEnter(this);
				}
			}
		}

		// Token: 0x06000736 RID: 1846 RVA: 0x0002CC50 File Offset: 0x0002AE50
		private unsafe void SelectTarget()
		{
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				return;
			}
			int num = Fafnir.sRandom.Next(4);
			for (int i = 0; i < this.mPlayers.Length; i++)
			{
				if (this.mPlayers[(i + num) % 4].Playing)
				{
					Player player = this.mPlayers[(i + num) % 4];
					if (player.Avatar != null && !player.Avatar.Dead)
					{
						this.mTarget = player.Avatar;
						break;
					}
				}
			}
			if (this.mTarget != null)
			{
				this.mAimTargetPosition = this.mTarget.Position;
			}
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				Fafnir.ChangeTargetMessage changeTargetMessage;
				if (this.mTarget != null)
				{
					changeTargetMessage.Target = this.mTarget.Handle;
				}
				else
				{
					changeTargetMessage.Target = ushort.MaxValue;
				}
				BossFight.Instance.SendMessage<Fafnir.ChangeTargetMessage>(this, 2, (void*)(&changeTargetMessage), true);
			}
		}

		// Token: 0x06000737 RID: 1847 RVA: 0x0002CD2B File Offset: 0x0002AF2B
		protected void SetEarthQuakeThreshold()
		{
			this.mEarthQuakeThreshold = this.MaxHitPoints * (float)(5 - (1 + this.mNrOfEarthquakes)) * 0.2f;
			this.mEarthQuakeThreshold += 2f;
		}

		// Token: 0x06000738 RID: 1848 RVA: 0x0002CD5D File Offset: 0x0002AF5D
		public void DeInitialize()
		{
		}

		// Token: 0x06000739 RID: 1849 RVA: 0x0002CD5F File Offset: 0x0002AF5F
		public bool AddImpulseVelocity(Vector3 iDirection, float iElevation, float iMassPower, float iDistance)
		{
			return false;
		}

		// Token: 0x1700017A RID: 378
		// (get) Token: 0x0600073A RID: 1850 RVA: 0x0002CD62 File Offset: 0x0002AF62
		public override bool Dead
		{
			get
			{
				return this.mDead;
			}
		}

		// Token: 0x1700017B RID: 379
		// (get) Token: 0x0600073B RID: 1851 RVA: 0x0002CD6A File Offset: 0x0002AF6A
		public float MaxHitPoints
		{
			get
			{
				return 35000f;
			}
		}

		// Token: 0x1700017C RID: 380
		// (get) Token: 0x0600073C RID: 1852 RVA: 0x0002CD71 File Offset: 0x0002AF71
		public float HitPoints
		{
			get
			{
				return this.mHitPoints;
			}
		}

		// Token: 0x0600073D RID: 1853 RVA: 0x0002CD7C File Offset: 0x0002AF7C
		public DamageResult Damage(int iPartIndex, Damage iDamage, Entity iAttacker, ref Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			if (this.mCurrentState is Fafnir.SleepState || this.mCurrentState is Fafnir.IntroState || iAttacker is BossDamageZone)
			{
				return DamageResult.Deflected;
			}
			if (this.Dead | iPartIndex != 1)
			{
				return DamageResult.Deflected;
			}
			DamageResult damageResult = this.Damage(iDamage, iAttacker, iAttackPosition, iFeatures);
			if ((damageResult & DamageResult.Hit) == DamageResult.Hit | (damageResult & DamageResult.Damaged) == DamageResult.Damaged)
			{
				this.mDamageFlashTimer = 0.1f;
			}
			if (this.mHitPoints < 0f)
			{
				this.mHitPoints = 1f;
			}
			return damageResult;
		}

		// Token: 0x0600073E RID: 1854 RVA: 0x0002CE0C File Offset: 0x0002B00C
		public void Damage(int iPartIndex, float iDamage, Elements iElement)
		{
			base.Damage(iDamage, iElement);
		}

		// Token: 0x0600073F RID: 1855 RVA: 0x0002CE16 File Offset: 0x0002B016
		public void SetSlow(int iIndex)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000740 RID: 1856 RVA: 0x0002CE1D File Offset: 0x0002B01D
		public void GetRandomPositionOnCollisionSkin(int iIndex, out Vector3 oPosition)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000741 RID: 1857 RVA: 0x0002CE24 File Offset: 0x0002B024
		public bool HasStatus(int iIndex, StatusEffects iStatus)
		{
			return base.HasStatus(iStatus);
		}

		// Token: 0x06000742 RID: 1858 RVA: 0x0002CE2D File Offset: 0x0002B02D
		public float StatusMagnitude(int iIndex, StatusEffects iStatus)
		{
			return base.StatusMagnitude(iStatus);
		}

		// Token: 0x1700017D RID: 381
		// (get) Token: 0x06000743 RID: 1859 RVA: 0x0002CE36 File Offset: 0x0002B036
		protected override int BloodEffect
		{
			get
			{
				return Fafnir.BLOOD_BLACK_EFFECT;
			}
		}

		// Token: 0x1700017E RID: 382
		// (get) Token: 0x06000744 RID: 1860 RVA: 0x0002CE3D File Offset: 0x0002B03D
		protected override BossDamageZone Entity
		{
			get
			{
				return this.mHeadZone;
			}
		}

		// Token: 0x1700017F RID: 383
		// (get) Token: 0x06000745 RID: 1861 RVA: 0x0002CE45 File Offset: 0x0002B045
		protected override float Radius
		{
			get
			{
				return (this.mBodyZone.Body.CollisionSkin.GetPrimitiveLocal(0) as Capsule).Radius;
			}
		}

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x06000746 RID: 1862 RVA: 0x0002CE67 File Offset: 0x0002B067
		protected override float Length
		{
			get
			{
				return (this.mBodyZone.Body.CollisionSkin.GetPrimitiveLocal(0) as Capsule).Length;
			}
		}

		// Token: 0x17000181 RID: 385
		// (get) Token: 0x06000747 RID: 1863 RVA: 0x0002CE8C File Offset: 0x0002B08C
		protected override Vector3 NotifierTextPostion
		{
			get
			{
				Vector3 position = this.mHeadZone.Position;
				Vector3 vector = new Vector3(0f, 3f, 0f);
				Vector3.Add(ref vector, ref position, out position);
				return position;
			}
		}

		// Token: 0x06000748 RID: 1864 RVA: 0x0002CECC File Offset: 0x0002B0CC
		public void ScriptMessage(BossMessages iMessage)
		{
			if (iMessage != BossMessages.FafnirFight)
			{
				return;
			}
			this.ChangeState(Fafnir.State.Decision);
		}

		// Token: 0x06000749 RID: 1865 RVA: 0x0002CEE8 File Offset: 0x0002B0E8
		private unsafe void NetworkUpdate()
		{
			NetworkServer networkServer = NetworkManager.Instance.Interface as NetworkServer;
			if (networkServer == null)
			{
				return;
			}
			Fafnir.UpdateMessage updateMessage = default(Fafnir.UpdateMessage);
			updateMessage.Animation = 0;
			while ((int)updateMessage.Animation < this.mAnimationClips.Length && this.mAnimationController.AnimationClip != this.mAnimationClips[(int)updateMessage.Animation])
			{
				updateMessage.Animation += 1;
			}
			updateMessage.AnimationTime = this.mAnimationController.Time;
			updateMessage.Hitpoints = this.mHitPoints;
			updateMessage.AimForTarget = this.mAimForTarget;
			updateMessage.AimForTargetWeight = this.mAimForTargetWeight;
			updateMessage.AimTargetPosition = this.mAimTargetPosition;
			for (int i = 0; i < networkServer.Connections; i++)
			{
				float num = networkServer.GetLatency(i) * 0.5f;
				Fafnir.UpdateMessage updateMessage2 = updateMessage;
				updateMessage2.AnimationTime += num;
				BossFight.Instance.SendMessage<Fafnir.UpdateMessage>(this, 0, (void*)(&updateMessage), false, i);
			}
		}

		// Token: 0x0600074A RID: 1866 RVA: 0x0002CFE0 File Offset: 0x0002B1E0
		public unsafe void NetworkUpdate(ref BossUpdateMessage iMsg)
		{
			if (iMsg.Type == 0)
			{
				if ((float)iMsg.TimeStamp < this.mLastNetworkUpdate)
				{
					return;
				}
				this.mLastNetworkUpdate = (float)iMsg.TimeStamp;
				Fafnir.UpdateMessage updateMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&updateMessage));
				if (this.mAnimationController.AnimationClip != this.mAnimationClips[(int)updateMessage.Animation])
				{
					this.mAnimationController.StartClip(this.mAnimationClips[(int)updateMessage.Animation], false);
				}
				this.mAnimationController.Time = updateMessage.AnimationTime;
				this.mHitPoints = updateMessage.Hitpoints;
				this.mAimForTarget = updateMessage.AimForTarget;
				this.mAimForTargetWeight = updateMessage.AimForTargetWeight;
				this.mAimTargetPosition = updateMessage.AimTargetPosition;
				return;
			}
			else
			{
				if (iMsg.Type != 2)
				{
					if (iMsg.Type == 1)
					{
						Fafnir.ChangeStateMessage changeStateMessage;
						BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&changeStateMessage));
						IBossState<Fafnir> state = this.GetState(changeStateMessage.NewState);
						if (state != null)
						{
							this.mCurrentState.OnExit(this);
							this.mPreviousState = this.mCurrentState;
							this.mCurrentState = state;
							this.mCurrentState.OnEnter(this);
						}
					}
					return;
				}
				Fafnir.ChangeTargetMessage changeTargetMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&changeTargetMessage));
				if (changeTargetMessage.Target == 65535)
				{
					this.mTarget = null;
					return;
				}
				this.mTarget = (Magicka.GameLogic.Entities.Entity.GetFromHandle((int)changeTargetMessage.Target) as Character);
				this.mAimTargetPosition = this.mTarget.Position;
				return;
			}
		}

		// Token: 0x0600074B RID: 1867 RVA: 0x0002D139 File Offset: 0x0002B339
		public void NetworkInitialize(ref BossInitializeMessage iMsg)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600074C RID: 1868 RVA: 0x0002D140 File Offset: 0x0002B340
		BossEnum IBoss.GetBossType()
		{
			return BossEnum.Fafnir;
		}

		// Token: 0x17000182 RID: 386
		// (get) Token: 0x0600074D RID: 1869 RVA: 0x0002D144 File Offset: 0x0002B344
		public bool NetworkInitialized
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600074E RID: 1870 RVA: 0x0002D148 File Offset: 0x0002B348
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

		// Token: 0x0600074F RID: 1871 RVA: 0x0002D218 File Offset: 0x0002B418
		// Note: this type is marked as 'beforefieldinit'.
		static Fafnir()
		{
			float[][] array = new float[12][];
			float[][] array2 = array;
			int num = 0;
			float[] array3 = new float[2];
			array2[num] = array3;
			array[1] = new float[]
			{
				0.06666667f,
				0.93333334f
			};
			array[2] = new float[]
			{
				0.14444445f,
				0.8888889f
			};
			array[3] = new float[]
			{
				0.51428574f
			};
			array[4] = new float[]
			{
				0.6944444f,
				1f
			};
			array[5] = new float[]
			{
				0.42222223f,
				0.82222223f
			};
			array[6] = new float[]
			{
				0.3125f,
				0.75f
			};
			array[7] = new float[0];
			array[8] = new float[]
			{
				0.46666667f
			};
			array[9] = new float[0];
			array[10] = new float[]
			{
				0.46875f,
				1f
			};
			array[11] = new float[]
			{
				0.47f,
				0.51f,
				0.67f,
				0.67f
			};
			Fafnir.ANIMATION_TIMES = array;
			Fafnir.WAKE_TRIGGER = "fafnir_awake".GetHashCodeCustom();
			Fafnir.DEFEATED_TRIGGER = "fafnir_defeated".GetHashCodeCustom();
			Fafnir.LAVAQUAKE_SOUND = "magick_meteor_rumble".GetHashCodeCustom();
			Fafnir.DIALOG_INTRO = "fafnirintro".GetHashCodeCustom();
			Fafnir.DIALOG_OUTRO = "fafnirdefeat".GetHashCodeCustom();
			Fafnir.BLOOD_BLACK_EFFECT = "gore_splash_black".GetHashCodeCustom();
			Fafnir.FIRE_SPRAY_EFFECT = "fafnir_fire_spray".GetHashCodeCustom();
			Fafnir.FIRE_BREATH_EFFECT = "fafnir_fire_breath".GetHashCodeCustom();
			Fafnir.SOUNDS = new int[]
			{
				"boss_fafnir_attack".GetHashCodeCustom(),
				"boss_fafnir_breakfloor".GetHashCodeCustom(),
				"boss_fafnir_confuse".GetHashCodeCustom(),
				"boss_fafnir_death".GetHashCodeCustom(),
				"boss_fafnir_deathray".GetHashCodeCustom(),
				"boss_fafnir_pain".GetHashCodeCustom()
			};
			Fafnir.sRandom = new Random();
			Fafnir.sTailDamage = new Damage(AttackProperties.Knockback, Elements.Earth, 200f, 5f);
		}

		// Token: 0x040005CE RID: 1486
		private const int BEGIN_IDX = 0;

		// Token: 0x040005CF RID: 1487
		private const int END_IDX = 1;

		// Token: 0x040005D0 RID: 1488
		private const int WINGS_IDX_OFFSET = 2;

		// Token: 0x040005D1 RID: 1489
		private const float MAXHITPOINTS = 35000f;

		// Token: 0x040005D2 RID: 1490
		private const float MAXHITPOINTS_DIVISOR = 2.8571429E-05f;

		// Token: 0x040005D3 RID: 1491
		private const float WAKEUP_PROXIMITY_SQR = 400f;

		// Token: 0x040005D4 RID: 1492
		private const float NETWORK_UPDATE_PERIOD = 0.033333335f;

		// Token: 0x040005D5 RID: 1493
		private const float MIN_NECK_ANGLE = 0f;

		// Token: 0x040005D6 RID: 1494
		private const float MAX_NECK_ANGLE = 3.1415927f;

		// Token: 0x040005D7 RID: 1495
		private static readonly int[] LEVEL_PARTS = new int[]
		{
			"hole_debris0".GetHashCodeCustom(),
			"hole_debris1".GetHashCodeCustom(),
			"hole_debris2".GetHashCodeCustom(),
			"hole_debris3".GetHashCodeCustom(),
			"hole_debris4".GetHashCodeCustom(),
			"hole_debris5".GetHashCodeCustom(),
			"hole_debris6".GetHashCodeCustom(),
			"hole_debris7".GetHashCodeCustom(),
			"hole_debris8".GetHashCodeCustom(),
			"hole_debris9".GetHashCodeCustom(),
			"hole_debris10".GetHashCodeCustom(),
			"hole_debris11".GetHashCodeCustom(),
			"hole_debris12".GetHashCodeCustom(),
			"hole_debris14".GetHashCodeCustom(),
			"hole_debris15".GetHashCodeCustom(),
			"hole_debris16".GetHashCodeCustom(),
			"hole_debris17".GetHashCodeCustom(),
			"hole_debris18".GetHashCodeCustom(),
			"hole_debris19".GetHashCodeCustom(),
			"hole_debris20".GetHashCodeCustom(),
			"hole_debris21".GetHashCodeCustom(),
			"hole_debris22".GetHashCodeCustom(),
			"hole_debris23".GetHashCodeCustom(),
			"hole_debris24".GetHashCodeCustom(),
			"hole_debris25".GetHashCodeCustom()
		};

		// Token: 0x040005D8 RID: 1496
		private static readonly int[] LAVA_EFFECTS = new int[]
		{
			"effect_lava0".GetHashCodeCustom(),
			"effect_lava1".GetHashCodeCustom(),
			"effect_lava2".GetHashCodeCustom(),
			"effect_lava3".GetHashCodeCustom(),
			"effect_lava4".GetHashCodeCustom(),
			"effect_lava5".GetHashCodeCustom(),
			"effect_lava6".GetHashCodeCustom(),
			"effect_lava7".GetHashCodeCustom(),
			"effect_lava8".GetHashCodeCustom(),
			"effect_lava9".GetHashCodeCustom(),
			"effect_lava10".GetHashCodeCustom(),
			"effect_lava11".GetHashCodeCustom(),
			"effect_lava12".GetHashCodeCustom(),
			"effect_lava14".GetHashCodeCustom(),
			"effect_lava15".GetHashCodeCustom(),
			"effect_lava16".GetHashCodeCustom(),
			"effect_lava17".GetHashCodeCustom(),
			"effect_lava18".GetHashCodeCustom(),
			"effect_lava19".GetHashCodeCustom(),
			"effect_lava20".GetHashCodeCustom(),
			"effect_lava21".GetHashCodeCustom(),
			"effect_lava22".GetHashCodeCustom(),
			"effect_lava23".GetHashCodeCustom(),
			"effect_lava24".GetHashCodeCustom(),
			"effect_lava25".GetHashCodeCustom()
		};

		// Token: 0x040005D9 RID: 1497
		private static readonly float[][] ANIMATION_TIMES;

		// Token: 0x040005DA RID: 1498
		private static readonly int WAKE_TRIGGER;

		// Token: 0x040005DB RID: 1499
		private static readonly int DEFEATED_TRIGGER;

		// Token: 0x040005DC RID: 1500
		private static readonly int LAVAQUAKE_SOUND;

		// Token: 0x040005DD RID: 1501
		private static readonly int DIALOG_INTRO;

		// Token: 0x040005DE RID: 1502
		private static readonly int DIALOG_OUTRO;

		// Token: 0x040005DF RID: 1503
		private static readonly int BLOOD_BLACK_EFFECT;

		// Token: 0x040005E0 RID: 1504
		private static readonly int FIRE_SPRAY_EFFECT;

		// Token: 0x040005E1 RID: 1505
		private static readonly int FIRE_BREATH_EFFECT;

		// Token: 0x040005E2 RID: 1506
		private float mLastNetworkUpdate;

		// Token: 0x040005E3 RID: 1507
		protected float mNetworkUpdateTimer;

		// Token: 0x040005E4 RID: 1508
		private AudioEmitter mLanceAudioEmitter = new AudioEmitter();

		// Token: 0x040005E5 RID: 1509
		private static readonly int[] SOUNDS;

		// Token: 0x040005E6 RID: 1510
		private Fafnir.SleepState mSleepState;

		// Token: 0x040005E7 RID: 1511
		private Fafnir.IntroState mIntroState;

		// Token: 0x040005E8 RID: 1512
		private Fafnir.IdleState mIdleState;

		// Token: 0x040005E9 RID: 1513
		private Fafnir.DecisionState mDecisionState;

		// Token: 0x040005EA RID: 1514
		private Fafnir.DefeatedState mDefeatedState;

		// Token: 0x040005EB RID: 1515
		private Fafnir.ConfuseState mConfuseState;

		// Token: 0x040005EC RID: 1516
		private Fafnir.WingState mWingState;

		// Token: 0x040005ED RID: 1517
		private Fafnir.TailState mTailState;

		// Token: 0x040005EE RID: 1518
		private Fafnir.FireballState mFireballState;

		// Token: 0x040005EF RID: 1519
		private Fafnir.FirelanceHighState mFirelanceHighState;

		// Token: 0x040005F0 RID: 1520
		private Fafnir.FirelanceLowState mFirelanceLowState;

		// Token: 0x040005F1 RID: 1521
		private Fafnir.CeilingState mCeilingState;

		// Token: 0x040005F2 RID: 1522
		private Fafnir.EarthQuakeState mEarthQuakeState;

		// Token: 0x040005F3 RID: 1523
		private float mEarthQuakeThreshold;

		// Token: 0x040005F4 RID: 1524
		private int mNrOfEarthquakes;

		// Token: 0x040005F5 RID: 1525
		private float mConfusedTimer;

		// Token: 0x040005F6 RID: 1526
		private HitList mHitList;

		// Token: 0x040005F7 RID: 1527
		private Fafnir.RenderData[] mRenderData;

		// Token: 0x040005F8 RID: 1528
		private Fafnir.FireRenderData[] mFireRenderData;

		// Token: 0x040005F9 RID: 1529
		private AnimationController mAnimationController;

		// Token: 0x040005FA RID: 1530
		private AnimationClip[] mAnimationClips;

		// Token: 0x040005FB RID: 1531
		private Matrix mOrientation;

		// Token: 0x040005FC RID: 1532
		private IBossState<Fafnir> mCurrentState;

		// Token: 0x040005FD RID: 1533
		private IBossState<Fafnir> mPreviousState;

		// Token: 0x040005FE RID: 1534
		private PlayState mPlayState;

		// Token: 0x040005FF RID: 1535
		private bool mDead;

		// Token: 0x04000600 RID: 1536
		private float mAimForTargetWeight;

		// Token: 0x04000601 RID: 1537
		private Vector3 mAimTargetPosition;

		// Token: 0x04000602 RID: 1538
		private bool mAimForTarget;

		// Token: 0x04000603 RID: 1539
		private Cue mLavaCue;

		// Token: 0x04000604 RID: 1540
		private int mNrOfNeckBones = 6;

		// Token: 0x04000605 RID: 1541
		private int mNrOfTailBones = 11;

		// Token: 0x04000606 RID: 1542
		private int[] mNeckIndices;

		// Token: 0x04000607 RID: 1543
		private Matrix[] mNeckBindPoses;

		// Token: 0x04000608 RID: 1544
		private int mHeadIndex;

		// Token: 0x04000609 RID: 1545
		private Matrix mHeadBindPose;

		// Token: 0x0400060A RID: 1546
		private int mMouthIndex;

		// Token: 0x0400060B RID: 1547
		private Matrix mMouthBindPose;

		// Token: 0x0400060C RID: 1548
		private int mRightEyeIndex;

		// Token: 0x0400060D RID: 1549
		private Matrix mRightEyeBindPose;

		// Token: 0x0400060E RID: 1550
		private int mLeftEyeIndex;

		// Token: 0x0400060F RID: 1551
		private Matrix mLeftEyeBindPose;

		// Token: 0x04000610 RID: 1552
		private int mSpineUpperIndex;

		// Token: 0x04000611 RID: 1553
		private Matrix mSpineUpperBindPose;

		// Token: 0x04000612 RID: 1554
		private int mSpineMidIndex;

		// Token: 0x04000613 RID: 1555
		private Matrix mSpineMidBindPose;

		// Token: 0x04000614 RID: 1556
		private int mSpineBaseIndex;

		// Token: 0x04000615 RID: 1557
		private Matrix mSpineBaseBindPose;

		// Token: 0x04000616 RID: 1558
		private int mRightHipIndex;

		// Token: 0x04000617 RID: 1559
		private Matrix mRightHipBindPose;

		// Token: 0x04000618 RID: 1560
		private int mRightHeelIndex;

		// Token: 0x04000619 RID: 1561
		private Matrix mRightHeelBindPose;

		// Token: 0x0400061A RID: 1562
		private int mRightToeIndex;

		// Token: 0x0400061B RID: 1563
		private Matrix mRightToeBindPose;

		// Token: 0x0400061C RID: 1564
		private int mRightShoulderIndex;

		// Token: 0x0400061D RID: 1565
		private Matrix mRightShoulderBindPose;

		// Token: 0x0400061E RID: 1566
		private int mRightWristIndex;

		// Token: 0x0400061F RID: 1567
		private Matrix mRightWristBindPose;

		// Token: 0x04000620 RID: 1568
		private int[] mTailIndices;

		// Token: 0x04000621 RID: 1569
		private Matrix[] mTailBindPoses;

		// Token: 0x04000622 RID: 1570
		private BoundingSphere mBoundingSphere;

		// Token: 0x04000623 RID: 1571
		private bool mDrawFireLance;

		// Token: 0x04000624 RID: 1572
		private Matrix mRightEyeOrientation;

		// Token: 0x04000625 RID: 1573
		private Matrix mLeftEyeOrientation;

		// Token: 0x04000626 RID: 1574
		private Matrix mMouthOrientation;

		// Token: 0x04000627 RID: 1575
		private Vector3 mSpineBasePosition;

		// Token: 0x04000628 RID: 1576
		private BossDamageZone mHeadZone;

		// Token: 0x04000629 RID: 1577
		private BossDamageZone mBodyZone;

		// Token: 0x0400062A RID: 1578
		private BossDamageZone mRightWristZone;

		// Token: 0x0400062B RID: 1579
		private BossDamageZone mRightHeelZone;

		// Token: 0x0400062C RID: 1580
		private BossDamageZone mTailZone;

		// Token: 0x0400062D RID: 1581
		private Player[] mPlayers;

		// Token: 0x0400062E RID: 1582
		private Character mTarget;

		// Token: 0x0400062F RID: 1583
		private float mLanceTime;

		// Token: 0x04000630 RID: 1584
		private Model mDebrisModel;

		// Token: 0x04000631 RID: 1585
		private float mLanceScale;

		// Token: 0x04000632 RID: 1586
		private Segment mLanceSegment;

		// Token: 0x04000633 RID: 1587
		private static readonly Random sRandom;

		// Token: 0x04000634 RID: 1588
		private static readonly Damage sTailDamage;

		// Token: 0x04000635 RID: 1589
		private float mDamageFlashTimer;

		// Token: 0x04000636 RID: 1590
		private Matrix mRenderRotationOffset = Matrix.CreateRotationY(1.5707964f);

		// Token: 0x020000E9 RID: 233
		private enum MessageType : ushort
		{
			// Token: 0x04000638 RID: 1592
			Update,
			// Token: 0x04000639 RID: 1593
			ChangeState,
			// Token: 0x0400063A RID: 1594
			ChangeTarget
		}

		// Token: 0x020000EA RID: 234
		internal struct UpdateMessage
		{
			// Token: 0x0400063B RID: 1595
			public const ushort TYPE = 0;

			// Token: 0x0400063C RID: 1596
			public float AimForTargetWeight;

			// Token: 0x0400063D RID: 1597
			public Vector3 AimTargetPosition;

			// Token: 0x0400063E RID: 1598
			public bool AimForTarget;

			// Token: 0x0400063F RID: 1599
			public byte Animation;

			// Token: 0x04000640 RID: 1600
			public float AnimationTime;

			// Token: 0x04000641 RID: 1601
			public float Hitpoints;
		}

		// Token: 0x020000EB RID: 235
		internal struct ChangeStateMessage
		{
			// Token: 0x04000642 RID: 1602
			public const ushort TYPE = 1;

			// Token: 0x04000643 RID: 1603
			public Fafnir.State NewState;
		}

		// Token: 0x020000EC RID: 236
		internal struct ChangeTargetMessage
		{
			// Token: 0x04000644 RID: 1604
			public const ushort TYPE = 2;

			// Token: 0x04000645 RID: 1605
			public ushort Target;
		}

		// Token: 0x020000ED RID: 237
		private enum Sounds : byte
		{
			// Token: 0x04000647 RID: 1607
			Attack,
			// Token: 0x04000648 RID: 1608
			Breakfloor,
			// Token: 0x04000649 RID: 1609
			Confuse,
			// Token: 0x0400064A RID: 1610
			Death,
			// Token: 0x0400064B RID: 1611
			Deathray,
			// Token: 0x0400064C RID: 1612
			Pain
		}

		// Token: 0x020000EE RID: 238
		private enum Animations
		{
			// Token: 0x0400064E RID: 1614
			Invalid,
			// Token: 0x0400064F RID: 1615
			ceiling,
			// Token: 0x04000650 RID: 1616
			charm,
			// Token: 0x04000651 RID: 1617
			defeated,
			// Token: 0x04000652 RID: 1618
			fireball,
			// Token: 0x04000653 RID: 1619
			fire_high,
			// Token: 0x04000654 RID: 1620
			fire_low,
			// Token: 0x04000655 RID: 1621
			idle,
			// Token: 0x04000656 RID: 1622
			intro,
			// Token: 0x04000657 RID: 1623
			sleep,
			// Token: 0x04000658 RID: 1624
			tailwhip,
			// Token: 0x04000659 RID: 1625
			wings,
			// Token: 0x0400065A RID: 1626
			NrOfAnimations
		}

		// Token: 0x020000EF RID: 239
		protected class RenderData : RenderableObject<SkinnedModelDeferredEffect, SkinnedModelDeferredBasicMaterial>
		{
			// Token: 0x06000750 RID: 1872 RVA: 0x0002D70E File Offset: 0x0002B90E
			public RenderData()
			{
				this.mSkeleton = new Matrix[80];
			}

			// Token: 0x06000751 RID: 1873 RVA: 0x0002D724 File Offset: 0x0002B924
			public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				skinnedModelDeferredEffect.OverrideColor = new Vector4(1f, 1f, 1f, this.Flash);
				skinnedModelDeferredEffect.Damage = this.mDamage;
				skinnedModelDeferredEffect.ProjectionMapEnabled = false;
				this.mMaterial.Damage = this.mDamage;
				skinnedModelDeferredEffect.Bones = this.mSkeleton;
				base.Draw(iEffect, iViewFrustum);
				skinnedModelDeferredEffect.OverrideColor = Vector4.Zero;
			}

			// Token: 0x06000752 RID: 1874 RVA: 0x0002D79C File Offset: 0x0002B99C
			public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				skinnedModelDeferredEffect.Bones = this.mSkeleton;
				base.DrawShadow(iEffect, iViewFrustum);
			}

			// Token: 0x0400065B RID: 1627
			public float Flash;

			// Token: 0x0400065C RID: 1628
			public float mDamage;

			// Token: 0x0400065D RID: 1629
			public Matrix[] mSkeleton;
		}

		// Token: 0x020000F0 RID: 240
		protected class FireRenderData : IRenderableAdditiveObject
		{
			// Token: 0x06000753 RID: 1875 RVA: 0x0002D7C4 File Offset: 0x0002B9C4
			public FireRenderData(VertexBuffer iVertexBuffer, VertexDeclaration iDeclaration)
			{
				this.mRotation = Matrix.CreateRotationY(3.1415927f);
				this.mVerticesHash = iVertexBuffer.GetHashCode();
				this.mVertexBuffer = iVertexBuffer;
				this.mVertexDeclaration = iDeclaration;
			}

			// Token: 0x17000183 RID: 387
			// (get) Token: 0x06000754 RID: 1876 RVA: 0x0002D7F6 File Offset: 0x0002B9F6
			public int Effect
			{
				get
				{
					return AdditiveEffect.TYPEHASH;
				}
			}

			// Token: 0x17000184 RID: 388
			// (get) Token: 0x06000755 RID: 1877 RVA: 0x0002D7FD File Offset: 0x0002B9FD
			public int Technique
			{
				get
				{
					return 0;
				}
			}

			// Token: 0x17000185 RID: 389
			// (get) Token: 0x06000756 RID: 1878 RVA: 0x0002D800 File Offset: 0x0002BA00
			public VertexBuffer Vertices
			{
				get
				{
					return this.mVertexBuffer;
				}
			}

			// Token: 0x17000186 RID: 390
			// (get) Token: 0x06000757 RID: 1879 RVA: 0x0002D808 File Offset: 0x0002BA08
			public int VerticesHashCode
			{
				get
				{
					return this.mVerticesHash;
				}
			}

			// Token: 0x17000187 RID: 391
			// (get) Token: 0x06000758 RID: 1880 RVA: 0x0002D810 File Offset: 0x0002BA10
			public int VertexStride
			{
				get
				{
					return VertexPositionColorTexture.SizeInBytes;
				}
			}

			// Token: 0x17000188 RID: 392
			// (get) Token: 0x06000759 RID: 1881 RVA: 0x0002D817 File Offset: 0x0002BA17
			public IndexBuffer Indices
			{
				get
				{
					return null;
				}
			}

			// Token: 0x17000189 RID: 393
			// (get) Token: 0x0600075A RID: 1882 RVA: 0x0002D81A File Offset: 0x0002BA1A
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return this.mVertexDeclaration;
				}
			}

			// Token: 0x0600075B RID: 1883 RVA: 0x0002D822 File Offset: 0x0002BA22
			public bool Cull(BoundingFrustum iViewFrustum)
			{
				return false;
			}

			// Token: 0x0600075C RID: 1884 RVA: 0x0002D828 File Offset: 0x0002BA28
			public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				AdditiveEffect additiveEffect = iEffect as AdditiveEffect;
				this.mMaterial.AssignToEffect(additiveEffect);
				additiveEffect.TextureEnabled = true;
				Matrix world = this.mTransform;
				world.M11 *= 2f;
				world.M12 *= 2f;
				world.M13 *= 2f;
				world.M31 *= 16f;
				world.M32 *= 16f;
				world.M33 *= 16f;
				additiveEffect.World = world;
				additiveEffect.TextureScale = new Vector2(1f, this.mSize);
				additiveEffect.TextureOffset = new Vector2(0f, this.mScroll * 10f);
				additiveEffect.CommitChanges();
				additiveEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 2);
				world = this.mTransform;
				world.M11 *= 2f;
				world.M12 *= 2f;
				world.M13 *= 2f;
				world.M31 *= 16f;
				world.M32 *= 16f;
				world.M33 *= 16f;
				additiveEffect.World = world;
				additiveEffect.TextureScale = new Vector2(1f, this.mSize);
				additiveEffect.TextureOffset = new Vector2(0.5f, this.mScroll * 4f);
				additiveEffect.CommitChanges();
				additiveEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 2);
			}

			// Token: 0x0400065E RID: 1630
			private int mVerticesHash;

			// Token: 0x0400065F RID: 1631
			private VertexBuffer mVertexBuffer;

			// Token: 0x04000660 RID: 1632
			private VertexDeclaration mVertexDeclaration;

			// Token: 0x04000661 RID: 1633
			public Matrix mTransform;

			// Token: 0x04000662 RID: 1634
			public AdditiveMaterial mMaterial;

			// Token: 0x04000663 RID: 1635
			public float mScroll;

			// Token: 0x04000664 RID: 1636
			public float mSize;

			// Token: 0x04000665 RID: 1637
			private Matrix mRotation;
		}

		// Token: 0x020000F1 RID: 241
		public enum State : byte
		{
			// Token: 0x04000667 RID: 1639
			Sleep,
			// Token: 0x04000668 RID: 1640
			Intro,
			// Token: 0x04000669 RID: 1641
			Idle,
			// Token: 0x0400066A RID: 1642
			Decision,
			// Token: 0x0400066B RID: 1643
			Tail,
			// Token: 0x0400066C RID: 1644
			Wing,
			// Token: 0x0400066D RID: 1645
			Confuse,
			// Token: 0x0400066E RID: 1646
			Fireball,
			// Token: 0x0400066F RID: 1647
			LanceHigh,
			// Token: 0x04000670 RID: 1648
			LanceLow,
			// Token: 0x04000671 RID: 1649
			Ceiling,
			// Token: 0x04000672 RID: 1650
			EarthQuake,
			// Token: 0x04000673 RID: 1651
			Defeated
		}

		// Token: 0x020000F3 RID: 243
		public interface IFafnirState : IBossState<Fafnir>
		{
			// Token: 0x06000760 RID: 1888
			float GetWeight(Fafnir iOwner, float iHealth, float iPlayerWeight);
		}

		// Token: 0x020000F4 RID: 244
		public class SleepState : IBossState<Fafnir>
		{
			// Token: 0x06000761 RID: 1889 RVA: 0x0002D9DB File Offset: 0x0002BBDB
			public void OnEnter(Fafnir iOwner)
			{
				iOwner.mAnimationController.StartClip(iOwner.mAnimationClips[9], true);
			}

			// Token: 0x06000762 RID: 1890 RVA: 0x0002D9F2 File Offset: 0x0002BBF2
			public void OnUpdate(float iDeltaTime, Fafnir iOwner)
			{
			}

			// Token: 0x06000763 RID: 1891 RVA: 0x0002D9F4 File Offset: 0x0002BBF4
			public void OnExit(Fafnir iOwner)
			{
			}
		}

		// Token: 0x020000F5 RID: 245
		public class IntroState : IBossState<Fafnir>
		{
			// Token: 0x06000765 RID: 1893 RVA: 0x0002D9FE File Offset: 0x0002BBFE
			public void OnEnter(Fafnir iOwner)
			{
				iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[8], 0.5f, false);
			}

			// Token: 0x06000766 RID: 1894 RVA: 0x0002DA1C File Offset: 0x0002BC1C
			public void OnUpdate(float iDeltaTime, Fafnir iOwner)
			{
				if (!iOwner.mAnimationController.IsLooping && iOwner.mAnimationController.HasFinished && !iOwner.mAnimationController.CrossFadeEnabled)
				{
					iOwner.mPlayState.Level.CurrentScene.ExecuteTrigger(Fafnir.WAKE_TRIGGER, null, false);
					iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[7], 0.5f, true);
				}
			}

			// Token: 0x06000767 RID: 1895 RVA: 0x0002DA85 File Offset: 0x0002BC85
			public void OnExit(Fafnir iOwner)
			{
				iOwner.mAimForTarget = false;
			}
		}

		// Token: 0x020000F6 RID: 246
		public class IdleState : IBossState<Fafnir>
		{
			// Token: 0x06000769 RID: 1897 RVA: 0x0002DA96 File Offset: 0x0002BC96
			public void OnEnter(Fafnir iOwner)
			{
				iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[7], 2f, true);
				iOwner.mAimForTarget = false;
			}

			// Token: 0x0600076A RID: 1898 RVA: 0x0002DAB8 File Offset: 0x0002BCB8
			public void OnUpdate(float iDeltaTime, Fafnir iOwner)
			{
				for (int i = 0; i < iOwner.mPlayers.Length; i++)
				{
					if (iOwner.mPlayers[i].Playing && iOwner.mPlayers[i].Avatar != null && !iOwner.mPlayers[i].Avatar.Dead)
					{
						iOwner.ChangeState(Fafnir.State.Decision);
						return;
					}
				}
			}

			// Token: 0x0600076B RID: 1899 RVA: 0x0002DB12 File Offset: 0x0002BD12
			public void OnExit(Fafnir iOwner)
			{
			}
		}

		// Token: 0x020000F7 RID: 247
		public class DecisionState : IBossState<Fafnir>
		{
			// Token: 0x0600076D RID: 1901 RVA: 0x0002DB1C File Offset: 0x0002BD1C
			public void OnEnter(Fafnir iOwner)
			{
				iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[7], 2f, false);
				this.mTimer = (float)Fafnir.sRandom.NextDouble() * (1f - iOwner.mHitPoints * 2.8571429E-05f);
				iOwner.SelectTarget();
			}

			// Token: 0x0600076E RID: 1902 RVA: 0x0002DB6C File Offset: 0x0002BD6C
			public void OnUpdate(float iDeltaTime, Fafnir iOwner)
			{
				this.mTimer -= iDeltaTime;
				if (this.mTimer <= 0f || (!iOwner.mAnimationController.CrossFadeEnabled && iOwner.mAnimationController.HasFinished))
				{
					Player[] mPlayers = iOwner.mPlayers;
					bool flag = true;
					for (int i = 0; i < mPlayers.Length; i++)
					{
						if (mPlayers[i].Playing && mPlayers[i].Avatar != null && !mPlayers[i].Avatar.Dead)
						{
							flag = false;
						}
					}
					if (flag)
					{
						iOwner.ChangeState(Fafnir.State.Idle);
						return;
					}
					if (iOwner.mHitPoints <= iOwner.mEarthQuakeThreshold)
					{
						iOwner.ChangeState(Fafnir.State.EarthQuake);
						return;
					}
					float iHealth = iOwner.mHitPoints * 2.8571429E-05f;
					float num = 0f;
					for (int j = 0; j < mPlayers.Length; j++)
					{
						if (mPlayers[j].Playing && mPlayers[j].Avatar != null && !mPlayers[j].Avatar.Dead)
						{
							num += 1f;
						}
					}
					float iPlayerWeight = 1f / num;
					float num2 = iOwner.mWingState.GetWeight(iOwner, iHealth, iPlayerWeight);
					float num3 = iOwner.mConfuseState.GetWeight(iOwner, iHealth, iPlayerWeight);
					float num4 = iOwner.mCeilingState.GetWeight(iOwner, iHealth, iPlayerWeight);
					float num5 = iOwner.mTailState.GetWeight(iOwner, iHealth, iPlayerWeight);
					float num6 = iOwner.mFireballState.GetWeight(iOwner, iHealth, iPlayerWeight);
					float num7 = iOwner.mFirelanceHighState.GetWeight(iOwner, iHealth, iPlayerWeight);
					float num8 = iOwner.mFirelanceLowState.GetWeight(iOwner, iHealth, iPlayerWeight);
					float num9 = num2 + num3 + num4 + num5 + num6 + num7 + num8;
					num2 /= num9;
					num3 /= num9;
					num4 /= num9;
					num5 /= num9;
					num6 /= num9;
					num7 /= num9;
					num8 /= num9;
					if (num2 >= num3 && num2 >= num4 && num2 >= num6 && num2 >= num7 && num2 >= num5 && num2 >= num8)
					{
						iOwner.ChangeState(Fafnir.State.Wing);
						return;
					}
					if (num3 >= num4 && num3 >= num6 && num3 >= num7 && num3 >= num5 && num3 >= num8)
					{
						iOwner.ChangeState(Fafnir.State.Confuse);
						return;
					}
					if (num5 >= num4 && num5 >= num6 && num5 >= num7 && num5 >= num8)
					{
						iOwner.ChangeState(Fafnir.State.Tail);
						return;
					}
					if (num4 >= num6 && num4 >= num7 && num4 >= num8)
					{
						iOwner.ChangeState(Fafnir.State.Ceiling);
						return;
					}
					if (num7 >= num6 && num7 >= num8)
					{
						iOwner.ChangeState(Fafnir.State.LanceHigh);
						return;
					}
					if (num8 >= num6)
					{
						iOwner.ChangeState(Fafnir.State.LanceLow);
						return;
					}
					if (iOwner.mPreviousState != iOwner.mFireballState)
					{
						iOwner.ChangeState(Fafnir.State.Fireball);
						return;
					}
					Fafnir.State iState;
					IBossState<Fafnir> bossState;
					do
					{
						switch (Fafnir.sRandom.Next(6))
						{
						default:
							iState = Fafnir.State.Wing;
							bossState = iOwner.mWingState;
							break;
						case 1:
							iState = Fafnir.State.Ceiling;
							bossState = iOwner.mCeilingState;
							break;
						case 2:
							iState = Fafnir.State.Tail;
							bossState = iOwner.mTailState;
							break;
						case 3:
							iState = Fafnir.State.LanceLow;
							bossState = iOwner.mFirelanceLowState;
							break;
						case 4:
							iState = Fafnir.State.LanceHigh;
							bossState = iOwner.mFirelanceHighState;
							break;
						case 5:
							iState = Fafnir.State.Confuse;
							bossState = iOwner.mConfuseState;
							break;
						}
					}
					while (bossState == iOwner.mPreviousState);
					iOwner.ChangeState(iState);
				}
			}

			// Token: 0x0600076F RID: 1903 RVA: 0x0002DE8C File Offset: 0x0002C08C
			public void OnExit(Fafnir iOwner)
			{
				iOwner.mAimForTarget = false;
			}

			// Token: 0x04000674 RID: 1652
			private float mTimer;
		}

		// Token: 0x020000F8 RID: 248
		public class TailState : Fafnir.IFafnirState, IBossState<Fafnir>
		{
			// Token: 0x06000771 RID: 1905 RVA: 0x0002DE9D File Offset: 0x0002C09D
			public void OnEnter(Fafnir iOwner)
			{
				iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[10], 0.5f, false);
				iOwner.mAimForTarget = false;
				this.mPlayedSound = false;
			}

			// Token: 0x06000772 RID: 1906 RVA: 0x0002DEC8 File Offset: 0x0002C0C8
			public void OnUpdate(float iDeltaTime, Fafnir iOwner)
			{
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
					if (iOwner.mAnimationController.HasFinished)
					{
						iOwner.ChangeState(Fafnir.State.Decision);
					}
					if (!this.mPlayedSound && num > 0.5f)
					{
						this.mPlayedSound = true;
						AudioManager.Instance.PlayCue(Banks.Characters, Fafnir.SOUNDS[0], iOwner.mHeadZone.AudioEmitter);
					}
				}
			}

			// Token: 0x06000773 RID: 1907 RVA: 0x0002DF49 File Offset: 0x0002C149
			public void OnExit(Fafnir iOwner)
			{
			}

			// Token: 0x06000774 RID: 1908 RVA: 0x0002DF4C File Offset: 0x0002C14C
			public float GetWeight(Fafnir iOwner, float iHealth, float iPlayerWeight)
			{
				float num = 0f;
				if (iOwner.mPreviousState != this)
				{
					Vector3 translation = iOwner.mTailBindPoses[6].Translation;
					Matrix matrix = iOwner.mAnimationController.SkinnedBoneTransforms[iOwner.mTailIndices[6]];
					Vector3.Transform(ref translation, ref matrix, out translation);
					translation.Y = 0f;
					Player[] mPlayers = iOwner.mPlayers;
					for (int i = 0; i < mPlayers.Length; i++)
					{
						if (mPlayers[i].Playing && mPlayers[i].Avatar != null && !mPlayers[i].Avatar.Dead)
						{
							Vector3 position = mPlayers[i].Avatar.Position;
							position.Y = 0f;
							float num2;
							Vector3.DistanceSquared(ref translation, ref position, out num2);
							if (num2 <= Fafnir.TailState.WEIGHT_RANGE_SQR)
							{
								num += iPlayerWeight;
							}
						}
					}
				}
				return num;
			}

			// Token: 0x04000675 RID: 1653
			private static float WEIGHT_RANGE_SQR = 25f;

			// Token: 0x04000676 RID: 1654
			private bool mPlayedSound;
		}

		// Token: 0x020000F9 RID: 249
		public class WingState : Fafnir.IFafnirState, IBossState<Fafnir>
		{
			// Token: 0x06000777 RID: 1911 RVA: 0x0002E03A File Offset: 0x0002C23A
			public void OnEnter(Fafnir iOwner)
			{
				iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[11], 0.5f, false);
				iOwner.mAimForTarget = false;
				this.mPlayedFirstSound = false;
				this.mPlayedSecondSound = false;
			}

			// Token: 0x06000778 RID: 1912 RVA: 0x0002E06C File Offset: 0x0002C26C
			public void OnUpdate(float iDeltaTime, Fafnir iOwner)
			{
				float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
				if (iOwner.mAnimationController.HasFinished && !iOwner.mAnimationController.CrossFadeEnabled)
				{
					iOwner.ChangeState(Fafnir.State.Decision);
					return;
				}
				if ((!iOwner.mAnimationController.CrossFadeEnabled && num >= Fafnir.ANIMATION_TIMES[11][0] && num <= Fafnir.ANIMATION_TIMES[11][1]) || (num >= Fafnir.ANIMATION_TIMES[11][2] && num <= Fafnir.ANIMATION_TIMES[11][3]))
				{
					if (num <= Fafnir.ANIMATION_TIMES[11][1] && !this.mPlayedFirstSound)
					{
						AudioManager.Instance.PlayCue(Banks.Characters, Fafnir.SOUNDS[0], iOwner.mHeadZone.AudioEmitter);
						this.mPlayedFirstSound = true;
					}
					else if (num >= Fafnir.ANIMATION_TIMES[11][2] && !this.mPlayedSecondSound)
					{
						AudioManager.Instance.PlayCue(Banks.Characters, Fafnir.SOUNDS[0], iOwner.mHeadZone.AudioEmitter);
						this.mPlayedSecondSound = true;
					}
					Vector3 mSpineBasePosition = iOwner.mSpineBasePosition;
					List<Entity> entities = iOwner.mPlayState.EntityManager.GetEntities(mSpineBasePosition, 30f, true);
					entities.Remove(iOwner.mHeadZone);
					entities.Remove(iOwner.mBodyZone);
					entities.Remove(iOwner.mRightHeelZone);
					entities.Remove(iOwner.mRightWristZone);
					entities.Remove(iOwner.mTailZone);
					Vector3 backward = Vector3.Backward;
					for (int i = 0; i < entities.Count; i++)
					{
						if (entities[i] is IDamageable && !iOwner.mHitList.ContainsKey(entities[i].Handle))
						{
							Vector3 position = entities[i].Position;
							Vector3 vector;
							Vector3.Subtract(ref position, ref mSpineBasePosition, out vector);
							vector.Y = 0f;
							vector.Normalize();
							float num2;
							Vector3.Dot(ref vector, ref backward, out num2);
							if (num2 > 0.5f)
							{
								Vector3 position2 = entities[i].Position;
								position2.Z -= 2f;
								(entities[i] as IDamageable).Damage(Fafnir.WingState.sPush, iOwner.mHeadZone, 0.0, position2);
								iOwner.mHitList.Add(entities[i].Handle, 0.06125f);
							}
						}
					}
					iOwner.mPlayState.EntityManager.ReturnEntityList(entities);
				}
			}

			// Token: 0x06000779 RID: 1913 RVA: 0x0002E2E1 File Offset: 0x0002C4E1
			public void OnExit(Fafnir iOwner)
			{
			}

			// Token: 0x0600077A RID: 1914 RVA: 0x0002E2E4 File Offset: 0x0002C4E4
			public float GetWeight(Fafnir iOwner, float iHealth, float iPlayerWeight)
			{
				float num = 0f;
				if (iOwner.mPreviousState != this)
				{
					Vector3 mSpineBasePosition = iOwner.mSpineBasePosition;
					Vector3 backward = Vector3.Backward;
					Player[] mPlayers = iOwner.mPlayers;
					for (int i = 0; i < mPlayers.Length; i++)
					{
						if (mPlayers[i].Playing && mPlayers[i].Avatar != null && !mPlayers[i].Avatar.Dead)
						{
							Vector3 position = mPlayers[i].Avatar.Position;
							Vector3 vector;
							Vector3.Subtract(ref position, ref mSpineBasePosition, out vector);
							vector.Y = 0f;
							vector.Normalize();
							float num2;
							Vector3.Distance(ref position, ref mSpineBasePosition, out num2);
							float num3;
							Vector3.Dot(ref vector, ref backward, out num3);
							if (num3 > 0.5f && num2 < 10f)
							{
								num += 0.5f * iPlayerWeight + 0.75f * iPlayerWeight * num3;
							}
						}
					}
				}
				return num;
			}

			// Token: 0x04000677 RID: 1655
			private static readonly Damage sPush = new Damage(AttackProperties.Pushed, Elements.Earth, 150f, 1f);

			// Token: 0x04000678 RID: 1656
			private bool mPlayedFirstSound;

			// Token: 0x04000679 RID: 1657
			private bool mPlayedSecondSound;
		}

		// Token: 0x020000FA RID: 250
		public class ConfuseState : Fafnir.IFafnirState, IBossState<Fafnir>
		{
			// Token: 0x0600077D RID: 1917 RVA: 0x0002E3E4 File Offset: 0x0002C5E4
			public void OnEnter(Fafnir iOwner)
			{
				iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[2], 0.5f, false);
				AudioManager.Instance.PlayCue(Banks.Characters, Fafnir.SOUNDS[2], iOwner.mHeadZone.AudioEmitter);
				iOwner.mAimForTarget = true;
				EffectManager.Instance.StartEffect(Fafnir.ConfuseState.CHARM_EFFECT, ref iOwner.mRightEyeOrientation, out this.mRightCharmEffect);
				EffectManager.Instance.StartEffect(Fafnir.ConfuseState.CHARM_EFFECT, ref iOwner.mLeftEyeOrientation, out this.mLeftCharmEffect);
			}

			// Token: 0x0600077E RID: 1918 RVA: 0x0002E468 File Offset: 0x0002C668
			public void OnUpdate(float iDeltaTime, Fafnir iOwner)
			{
				EffectManager.Instance.UpdateOrientation(ref this.mLeftCharmEffect, ref iOwner.mLeftEyeOrientation);
				EffectManager.Instance.UpdateOrientation(ref this.mRightCharmEffect, ref iOwner.mRightEyeOrientation);
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
					if (!this.mInverted && num >= Fafnir.ANIMATION_TIMES[2][0] && num <= Fafnir.ANIMATION_TIMES[2][1])
					{
						Player[] players = Game.Instance.Players;
						for (int i = 0; i < players.Length; i++)
						{
							if (players[i].Playing && players[i].Avatar != null && !players[i].Avatar.Dead && !(players[i].Gamer is NetworkGamer))
							{
								players[i].Controller.Invert(true);
								this.mInverted = true;
							}
						}
						iOwner.mConfusedTimer = 20f;
						return;
					}
					if (iOwner.mAnimationController.HasFinished)
					{
						iOwner.ChangeState(Fafnir.State.Decision);
					}
				}
			}

			// Token: 0x0600077F RID: 1919 RVA: 0x0002E573 File Offset: 0x0002C773
			public void OnExit(Fafnir iOwner)
			{
				iOwner.mAimForTarget = false;
				EffectManager.Instance.Stop(ref this.mLeftCharmEffect);
				EffectManager.Instance.Stop(ref this.mRightCharmEffect);
			}

			// Token: 0x06000780 RID: 1920 RVA: 0x0002E59C File Offset: 0x0002C79C
			public float GetWeight(Fafnir iOwner, float iHealth, float iPlayerWeight)
			{
				float result = 0f;
				if (iHealth <= 0.6f && iOwner.mPreviousState != this && !this.mInverted)
				{
					result = (float)Fafnir.sRandom.NextDouble();
				}
				return result;
			}

			// Token: 0x0400067A RID: 1658
			private VisualEffectReference mLeftCharmEffect;

			// Token: 0x0400067B RID: 1659
			private VisualEffectReference mRightCharmEffect;

			// Token: 0x0400067C RID: 1660
			private static readonly int CHARM_EFFECT = "fafnir_charm".GetHashCodeCustom();

			// Token: 0x0400067D RID: 1661
			public bool mInverted;
		}

		// Token: 0x020000FB RID: 251
		public class FireballState : Fafnir.IFafnirState, IBossState<Fafnir>
		{
			// Token: 0x06000783 RID: 1923 RVA: 0x0002E5F0 File Offset: 0x0002C7F0
			public FireballState()
			{
				float iRadius = 6f;
				DamageCollection5 damageCollection = default(DamageCollection5);
				damageCollection.AddDamage(new Damage(AttackProperties.Damage, Elements.Fire, 300f, 1.5f));
				damageCollection.AddDamage(new Damage(AttackProperties.Knockback, Elements.Fire, 300f, 5f));
				damageCollection.AddDamage(new Damage(AttackProperties.Status, Elements.Fire, 200f, 3f));
				this.mFireballConditions = new ConditionCollection();
				this.mFireballConditions[0].Condition.EventConditionType = EventConditionType.Default;
				this.mFireballConditions[0].Condition.Repeat = true;
				this.mFireballConditions[0].Add(new EventStorage(new PlayEffectEvent(Fafnir.FireballState.FIREBALL_TRAIL_EFFECT, true)));
				this.mFireballConditions[0].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Fafnir.FireballState.FIREBALL_TRAIL_SOUND, true)));
				this.mFireballConditions[1].Condition.EventConditionType = EventConditionType.Hit;
				this.mFireballConditions[1].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Fafnir.FireballState.FIREBALL_HIT_SOUND)));
				this.mFireballConditions[1].Add(new EventStorage(new PlayEffectEvent(Fafnir.FireballState.FIREBALL_HIT_EFFECT)));
				this.mFireballConditions[1].Add(new EventStorage(new PlayEffectEvent(Fafnir.FireballState.FIREBALL_SPLASH_EFFECT)));
				this.mFireballConditions[1].Add(new EventStorage(default(RemoveEvent)));
				this.mFireballConditions[1].Add(new EventStorage(new SplashEvent(damageCollection.A, iRadius)));
				this.mFireballConditions[1].Add(new EventStorage(new SplashEvent(damageCollection.B, iRadius)));
				this.mFireballConditions[1].Add(new EventStorage(new SplashEvent(damageCollection.C, iRadius)));
				this.mFireballConditions[2].Condition.EventConditionType = EventConditionType.Collision;
				this.mFireballConditions[2].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Fafnir.FireballState.FIREBALL_HIT_SOUND)));
				this.mFireballConditions[2].Add(new EventStorage(new PlayEffectEvent(Fafnir.FireballState.FIREBALL_HIT_EFFECT)));
				this.mFireballConditions[2].Add(new EventStorage(new PlayEffectEvent(Fafnir.FireballState.FIREBALL_SPLASH_EFFECT)));
				this.mFireballConditions[2].Add(new EventStorage(default(RemoveEvent)));
				this.mFireballConditions[2].Add(new EventStorage(new SplashEvent(damageCollection.A, iRadius)));
				this.mFireballConditions[2].Add(new EventStorage(new SplashEvent(damageCollection.B, iRadius)));
				this.mFireballConditions[2].Add(new EventStorage(new SplashEvent(damageCollection.C, iRadius)));
				this.mFireballConditions[3].Condition.EventConditionType = EventConditionType.Timer;
				this.mFireballConditions[3].Add(new EventStorage(default(RemoveEvent)));
				this.mFireballConditions[3].Condition.Time = 8f;
			}

			// Token: 0x06000784 RID: 1924 RVA: 0x0002E92C File Offset: 0x0002CB2C
			public void OnEnter(Fafnir iOwner)
			{
				iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[4], 0.4f, false);
				this.mFireballShot = false;
				iOwner.mAimForTarget = true;
				iOwner.SelectTarget();
			}

			// Token: 0x06000785 RID: 1925 RVA: 0x0002E95C File Offset: 0x0002CB5C
			public void OnUpdate(float iDeltaTime, Fafnir iOwner)
			{
				if (iOwner.mTarget == null || iOwner.mTarget.Dead)
				{
					iOwner.ChangeState(Fafnir.State.Decision);
					return;
				}
				float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
				if (num >= Fafnir.ANIMATION_TIMES[4][0] && num <= Fafnir.ANIMATION_TIMES[4][1] && !iOwner.mAnimationController.CrossFadeEnabled && !this.mFireballShot)
				{
					AudioManager.Instance.PlayCue(Banks.Characters, Fafnir.SOUNDS[0], iOwner.mHeadZone.AudioEmitter);
					MissileEntity instance = MissileEntity.GetInstance(iOwner.mPlayState);
					Vector3 position = iOwner.mHeadZone.Position;
					Vector3 unitZ = Vector3.UnitZ;
					Matrix orientation = iOwner.mHeadZone.Body.Orientation;
					Vector3.Transform(ref unitZ, ref orientation, out unitZ);
					Vector3 forward = iOwner.mHeadZone.Body.Orientation.Forward;
					Vector3.Add(ref position, ref forward, out position);
					Vector3.Add(ref position, ref forward, out position);
					Vector3.Multiply(ref forward, 30f, out forward);
					instance.Initialize(iOwner.mHeadZone, 0.25f, ref position, ref forward, null, this.mFireballConditions, false);
					instance.Body.ApplyGravity = false;
					iOwner.mPlayState.EntityManager.AddEntity(instance);
					this.mFireballShot = true;
					return;
				}
				if (iOwner.mAnimationController.HasFinished && !iOwner.mAnimationController.CrossFadeEnabled)
				{
					iOwner.ChangeState(Fafnir.State.Decision);
				}
			}

			// Token: 0x06000786 RID: 1926 RVA: 0x0002EAD8 File Offset: 0x0002CCD8
			public void OnExit(Fafnir iOwner)
			{
				iOwner.mAimForTarget = false;
			}

			// Token: 0x06000787 RID: 1927 RVA: 0x0002EAE4 File Offset: 0x0002CCE4
			public float GetWeight(Fafnir iOwner, float iHealth, float iPlayerWeight)
			{
				float result = 0f;
				if (iOwner.mPreviousState != this)
				{
					result = (float)Fafnir.sRandom.NextDouble();
				}
				return result;
			}

			// Token: 0x0400067E RID: 1662
			public static readonly int FIREBALL_TRAIL_EFFECT = "fafnir_fireball_trail".GetHashCodeCustom();

			// Token: 0x0400067F RID: 1663
			public static readonly int FIREBALL_HIT_EFFECT = "magick_meteor_explosion".GetHashCodeCustom();

			// Token: 0x04000680 RID: 1664
			public static readonly int FIREBALL_SPLASH_EFFECT = "fafnir_fireball_splash".GetHashCodeCustom();

			// Token: 0x04000681 RID: 1665
			public static readonly int FIREBALL_TRAIL_SOUND = "spell_fire_projectile".GetHashCodeCustom();

			// Token: 0x04000682 RID: 1666
			public static readonly int FIREBALL_HIT_SOUND = "magick_meteor_blast".GetHashCodeCustom();

			// Token: 0x04000683 RID: 1667
			private bool mFireballShot;

			// Token: 0x04000684 RID: 1668
			private ConditionCollection mFireballConditions;
		}

		// Token: 0x020000FC RID: 252
		public class FirelanceHighState : Fafnir.IFafnirState, IBossState<Fafnir>
		{
			// Token: 0x06000789 RID: 1929 RVA: 0x0002EB68 File Offset: 0x0002CD68
			public FirelanceHighState()
			{
				this.mDamage = default(DamageCollection5);
				this.mDamage.AddDamage(new Damage(AttackProperties.Damage, Elements.Fire, 200f, 1f));
				this.mDamage.AddDamage(new Damage(AttackProperties.Status, Elements.Fire, 200f, 3f));
				this.mDamage.AddDamage(new Damage(AttackProperties.Pushed, Elements.Fire, 300f, 3f));
			}

			// Token: 0x0600078A RID: 1930 RVA: 0x0002EBDC File Offset: 0x0002CDDC
			public void OnEnter(Fafnir iOwner)
			{
				iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[5], 0.5f, false);
				iOwner.mAimForTarget = false;
				iOwner.mDrawFireLance = false;
				iOwner.mLanceScale = 0f;
				this.mLanceTime = 0f;
				this.mDamageTimer = 0.5f;
				float duration = iOwner.mAnimationClips[5].Duration;
				this.mTotalLanceTime = (Fafnir.ANIMATION_TIMES[5][1] - Fafnir.ANIMATION_TIMES[5][0]) * duration;
			}

			// Token: 0x0600078B RID: 1931 RVA: 0x0002EC5C File Offset: 0x0002CE5C
			public void OnUpdate(float iDeltaTime, Fafnir iOwner)
			{
				if (iOwner.mTarget == null || iOwner.mTarget.Dead)
				{
					iOwner.ChangeState(Fafnir.State.Decision);
					return;
				}
				float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
				if (num >= Fafnir.ANIMATION_TIMES[5][0] && num <= Fafnir.ANIMATION_TIMES[5][1] && !iOwner.mAnimationController.CrossFadeEnabled)
				{
					if (this.mLanceCue == null)
					{
						this.mLanceCue = AudioManager.Instance.PlayCue(Banks.Characters, Fafnir.SOUNDS[4], iOwner.mLanceAudioEmitter);
					}
					this.mLanceTime += iDeltaTime;
					this.mDamageTimer -= iDeltaTime;
					iOwner.mAimForTarget = true;
					float num2 = this.mLanceTime / this.mTotalLanceTime;
					float val = 2f - 32f * (float)Math.Pow((double)(num2 - 0.5f), 4.0);
					iOwner.mLanceScale = Math.Min(val, 1f);
					iOwner.mDrawFireLance = true;
					Vector3 delta = iOwner.mLanceSegment.Delta;
					delta.Normalize();
					if (!EffectManager.Instance.UpdatePositionDirection(ref this.mFireBreath, ref iOwner.mLanceSegment.Origin, ref delta))
					{
						EffectManager.Instance.StartEffect(Fafnir.FIRE_SPRAY_EFFECT, ref iOwner.mLanceSegment.Origin, ref delta, out this.mFireBreath);
					}
					Vector3 right = Vector3.Right;
					Vector3 vector;
					Vector3.Add(ref iOwner.mLanceSegment.Origin, ref iOwner.mLanceSegment.Delta, out vector);
					if (!EffectManager.Instance.UpdatePositionDirection(ref this.mWeldEffect, ref vector, ref right))
					{
						EffectManager.Instance.StartEffect(Fafnir.FirelanceHighState.LANCE_WELD, ref vector, ref right, out this.mWeldEffect);
					}
					if (this.mDamageTimer <= 0f)
					{
						this.mDamageTimer = 0.25f;
						Helper.CircleDamage(iOwner.mPlayState, iOwner.mHeadZone, 0.0, iOwner.mTailZone, ref vector, 2f, ref this.mDamage);
					}
				}
				else if (num > Fafnir.ANIMATION_TIMES[5][1])
				{
					if (this.mLanceCue != null && !this.mLanceCue.IsStopping)
					{
						this.mLanceCue.Stop(AudioStopOptions.AsAuthored);
						this.mLanceCue = null;
					}
					iOwner.mDrawFireLance = false;
					EffectManager.Instance.Stop(ref this.mFireBreath);
					EffectManager.Instance.Stop(ref this.mWeldEffect);
				}
				if (iOwner.mAnimationController.HasFinished && !iOwner.mAnimationController.CrossFadeEnabled)
				{
					iOwner.ChangeState(Fafnir.State.Decision);
				}
			}

			// Token: 0x0600078C RID: 1932 RVA: 0x0002EED8 File Offset: 0x0002D0D8
			public void OnExit(Fafnir iOwner)
			{
				if (this.mLanceCue != null && !this.mLanceCue.IsStopping)
				{
					this.mLanceCue.Stop(AudioStopOptions.AsAuthored);
					this.mLanceCue = null;
				}
				iOwner.mDrawFireLance = false;
				EffectManager.Instance.Stop(ref this.mFireBreath);
			}

			// Token: 0x0600078D RID: 1933 RVA: 0x0002EF24 File Offset: 0x0002D124
			public float GetWeight(Fafnir iOwner, float iHealth, float iPlayerWeight)
			{
				float result = 0f;
				if (iOwner.mPreviousState != this)
				{
					result = (float)Fafnir.sRandom.NextDouble();
				}
				return result;
			}

			// Token: 0x04000685 RID: 1669
			private static readonly int LANCE_GROUND_SPLASH = "fafnir_lance_ground_splash".GetHashCodeCustom();

			// Token: 0x04000686 RID: 1670
			private static readonly int LANCE_LAVA_SPLASH = "fafnir_lance_lava_splash".GetHashCodeCustom();

			// Token: 0x04000687 RID: 1671
			private static readonly int LANCE_HIT = "fafnir_lance_hit".GetHashCodeCustom();

			// Token: 0x04000688 RID: 1672
			private static readonly int LANCE_WELD = "fafnir_lance_weld".GetHashCodeCustom();

			// Token: 0x04000689 RID: 1673
			private Cue mLanceCue;

			// Token: 0x0400068A RID: 1674
			private float mDamageTimer;

			// Token: 0x0400068B RID: 1675
			private float mLanceTime;

			// Token: 0x0400068C RID: 1676
			private float mTotalLanceTime;

			// Token: 0x0400068D RID: 1677
			private DamageCollection5 mDamage;

			// Token: 0x0400068E RID: 1678
			private VisualEffectReference mFireBreath;

			// Token: 0x0400068F RID: 1679
			private VisualEffectReference mWeldEffect;
		}

		// Token: 0x020000FD RID: 253
		public class FirelanceLowState : Fafnir.IFafnirState, IBossState<Fafnir>
		{
			// Token: 0x0600078F RID: 1935 RVA: 0x0002EF8C File Offset: 0x0002D18C
			public FirelanceLowState()
			{
				this.mDamage = default(DamageCollection5);
				this.mDamage.AddDamage(new Damage(AttackProperties.Damage, Elements.Fire, 200f, 1f));
				this.mDamage.AddDamage(new Damage(AttackProperties.Status, Elements.Fire, 200f, 3f));
				this.mDamage.AddDamage(new Damage(AttackProperties.Pushed, Elements.Fire, 300f, 3f));
			}

			// Token: 0x06000790 RID: 1936 RVA: 0x0002F000 File Offset: 0x0002D200
			public void OnEnter(Fafnir iOwner)
			{
				iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[6], 0.5f, false);
				iOwner.mDrawFireLance = false;
				iOwner.mAimForTarget = false;
				this.mDamageTimer = 0.5f;
				this.mFireBreath.ID = -1;
				float duration = iOwner.mAnimationClips[6].Duration;
				this.mTotalLanceTime = (Fafnir.ANIMATION_TIMES[6][1] - Fafnir.ANIMATION_TIMES[6][0]) * duration;
			}

			// Token: 0x06000791 RID: 1937 RVA: 0x0002F074 File Offset: 0x0002D274
			public void OnUpdate(float iDeltaTime, Fafnir iOwner)
			{
				if (iOwner.mTarget == null || iOwner.mTarget.Dead)
				{
					iOwner.ChangeState(Fafnir.State.Decision);
					return;
				}
				float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					if (num >= Fafnir.ANIMATION_TIMES[6][0] && num <= Fafnir.ANIMATION_TIMES[6][1] && !iOwner.mAnimationController.CrossFadeEnabled)
					{
						this.mDamageTimer -= iDeltaTime;
						Vector3 forward = iOwner.mHeadZone.Body.Orientation.Forward;
						forward.Normalize();
						Vector3 translation = iOwner.mMouthOrientation.Translation;
						Vector3.Add(ref translation, ref forward, out translation);
						if (!EffectManager.Instance.UpdatePositionDirection(ref this.mFireBreath, ref translation, ref forward))
						{
							EffectManager.Instance.StartEffect(Fafnir.FIRE_BREATH_EFFECT, ref translation, ref forward, out this.mFireBreath);
						}
						if (this.mDamageTimer <= 0f)
						{
							Helper.ArcDamage(iOwner.mPlayState, iOwner.mHeadZone, 0.0, iOwner.mHeadZone, ref translation, ref forward, 20f, 0.7853982f, ref this.mDamage);
							this.mDamageTimer += 0.25f;
						}
					}
					else if (this.mFireBreath.ID != -1 && num > Fafnir.ANIMATION_TIMES[6][1])
					{
						iOwner.mDrawFireLance = false;
						EffectManager.Instance.Stop(ref this.mFireBreath);
					}
					if (iOwner.mAnimationController.HasFinished)
					{
						iOwner.ChangeState(Fafnir.State.Decision);
					}
				}
			}

			// Token: 0x06000792 RID: 1938 RVA: 0x0002F209 File Offset: 0x0002D409
			public void OnExit(Fafnir iOwner)
			{
				iOwner.mDrawFireLance = false;
				EffectManager.Instance.Stop(ref this.mFireBreath);
			}

			// Token: 0x06000793 RID: 1939 RVA: 0x0002F224 File Offset: 0x0002D424
			public float GetWeight(Fafnir iOwner, float iHealth, float iPlayerWeight)
			{
				float num = 0f;
				if (iOwner.mPreviousState != this)
				{
					Vector2 vector = new Vector2(iOwner.mHeadZone.Position.X, iOwner.mHeadZone.Position.Z);
					Player[] mPlayers = iOwner.mPlayers;
					for (int i = 0; i < mPlayers.Length; i++)
					{
						if (mPlayers[i].Playing && mPlayers[i].Avatar != null && !mPlayers[i].Avatar.Dead)
						{
							Vector2 vector2 = new Vector2(mPlayers[i].Avatar.Position.X, mPlayers[i].Avatar.Position.Z);
							float num2;
							Vector2.DistanceSquared(ref vector, ref vector2, out num2);
							num2 = Math.Max(9f, num2);
							if (num2 <= 73f)
							{
								num2 -= 9f;
								float val = (64f - num2) / 64f;
								num = Math.Max(val, num) + 0.2f;
							}
						}
					}
				}
				return num;
			}

			// Token: 0x04000690 RID: 1680
			private const float MIN_HEAD_RANGE_SQR = 9f;

			// Token: 0x04000691 RID: 1681
			private const float MAX_HEAD_RANGE_SQR = 73f;

			// Token: 0x04000692 RID: 1682
			private const float HEAD_RANGE_SQR = 64f;

			// Token: 0x04000693 RID: 1683
			private float mDamageTimer;

			// Token: 0x04000694 RID: 1684
			private float mTotalLanceTime;

			// Token: 0x04000695 RID: 1685
			private DamageCollection5 mDamage;

			// Token: 0x04000696 RID: 1686
			private VisualEffectReference mFireBreath;
		}

		// Token: 0x020000FE RID: 254
		public class CeilingState : Fafnir.IFafnirState, IBossState<Fafnir>
		{
			// Token: 0x06000794 RID: 1940 RVA: 0x0002F330 File Offset: 0x0002D530
			public CeilingState()
			{
				float iRadius = 3f;
				Damage iDamage = new Damage(AttackProperties.Pushed, Elements.Earth, 250f, 5f);
				Damage iDamage2 = new Damage(AttackProperties.Damage, Elements.Earth, 300f, 1f);
				this.mDebrisConditionCollection = new ConditionCollection();
				this.mDebrisConditionCollection[0].Condition.EventConditionType = EventConditionType.Default;
				this.mDebrisConditionCollection[0].Condition.Repeat = true;
				this.mDebrisConditionCollection[0].Add(new EventStorage(new PlayEffectEvent(Fafnir.CeilingState.DEBRI_TRAIL_EFFECT, true)));
				this.mDebrisConditionCollection[1].Condition.EventConditionType = EventConditionType.Hit;
				this.mDebrisConditionCollection[1].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Fafnir.CeilingState.DEBRIS_SOUND)));
				this.mDebrisConditionCollection[1].Add(new EventStorage(new PlayEffectEvent(Fafnir.CeilingState.DEBRIS_EFFECT)));
				this.mDebrisConditionCollection[1].Add(new EventStorage(default(RemoveEvent)));
				this.mDebrisConditionCollection[1].Add(new EventStorage(new SplashEvent(iDamage, iRadius)));
				this.mDebrisConditionCollection[1].Add(new EventStorage(new SplashEvent(iDamage2, iRadius)));
				this.mDebrisConditionCollection[1].Add(new EventStorage(new CameraShakeEvent(0.5f, 1f, true)));
				this.mDebrisConditionCollection[2].Condition.EventConditionType = EventConditionType.Collision;
				this.mDebrisConditionCollection[2].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Fafnir.CeilingState.DEBRIS_SOUND)));
				this.mDebrisConditionCollection[2].Add(new EventStorage(new PlayEffectEvent(Fafnir.CeilingState.DEBRIS_EFFECT)));
				this.mDebrisConditionCollection[2].Add(new EventStorage(default(RemoveEvent)));
				this.mDebrisConditionCollection[2].Add(new EventStorage(new SplashEvent(iDamage, iRadius)));
				this.mDebrisConditionCollection[2].Add(new EventStorage(new SplashEvent(iDamage2, iRadius)));
				this.mDebrisConditionCollection[2].Add(new EventStorage(new CameraShakeEvent(0.5f, 1f)));
				this.mDebrisConditionCollection[3].Condition.EventConditionType = EventConditionType.Timer;
				this.mDebrisConditionCollection[3].Add(new EventStorage(default(RemoveEvent)));
				this.mDebrisConditionCollection[3].Condition.Time = 8f;
			}

			// Token: 0x06000795 RID: 1941 RVA: 0x0002F5CC File Offset: 0x0002D7CC
			public void OnEnter(Fafnir iOwner)
			{
				iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[1], 0.25f, false);
				this.mTimer = 0f;
				iOwner.mAimForTarget = false;
				iOwner.mDrawFireLance = false;
				iOwner.mLanceScale = 0f;
				float duration = iOwner.mAnimationClips[1].Duration;
				this.mTotalLanceTime = (Fafnir.ANIMATION_TIMES[1][1] - Fafnir.ANIMATION_TIMES[1][0]) * duration;
			}

			// Token: 0x06000796 RID: 1942 RVA: 0x0002F640 File Offset: 0x0002D840
			public void OnUpdate(float iDeltaTime, Fafnir iOwner)
			{
				if (iOwner.mTarget == null || iOwner.mTarget.Dead)
				{
					iOwner.ChangeState(Fafnir.State.Decision);
					return;
				}
				float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
				if (num >= Fafnir.ANIMATION_TIMES[1][0] && num <= Fafnir.ANIMATION_TIMES[1][1] && !iOwner.mAnimationController.CrossFadeEnabled)
				{
					iOwner.mPlayState.Camera.CameraShake(0.5f, iDeltaTime);
					this.mLanceTime += iDeltaTime;
					float num2 = this.mLanceTime / this.mTotalLanceTime;
					float val = 2f - 32f * (float)Math.Pow((double)(num2 - 0.5f), 4.0);
					iOwner.mLanceScale = Math.Min(val, 1f);
					iOwner.mDrawFireLance = true;
					Vector3 delta = iOwner.mLanceSegment.Delta;
					delta.Normalize();
					if (!EffectManager.Instance.UpdatePositionDirection(ref this.mFireBreath, ref iOwner.mLanceSegment.Origin, ref delta))
					{
						EffectManager.Instance.StartEffect(Fafnir.FIRE_SPRAY_EFFECT, ref iOwner.mLanceSegment.Origin, ref delta, out this.mFireBreath);
					}
					this.mTimer += iDeltaTime;
					if (this.mTimer > 0.5f)
					{
						this.mTimer -= 0.5f;
						Vector3 position = iOwner.mTarget.Position;
						position.Y += 50f;
						position.X += ((float)Fafnir.sRandom.NextDouble() - 0.5f) * 6f;
						position.Z += ((float)Fafnir.sRandom.NextDouble() - 0.5f) * 6f;
						Vector3 vector = new Vector3(0.1f, -22f, 0.1f);
						MissileEntity instance = MissileEntity.GetInstance(iOwner.mPlayState);
						instance.Initialize(iOwner.mHeadZone, 1f, ref position, ref vector, iOwner.mDebrisModel, this.mDebrisConditionCollection, false);
						instance.FacingVelocity = false;
						Vector3 angImpulse = new Vector3((float)(Fafnir.sRandom.NextDouble() - 0.5) * instance.Body.Mass * 0.5f, (float)(Fafnir.sRandom.NextDouble() - 0.5) * instance.Body.Mass * 0.5f, (float)(Fafnir.sRandom.NextDouble() - 0.5) * instance.Body.Mass * 0.5f);
						instance.Body.ApplyBodyAngImpulse(angImpulse);
						iOwner.mPlayState.EntityManager.AddEntity(instance);
					}
				}
				else if (num > Fafnir.ANIMATION_TIMES[1][1])
				{
					iOwner.mDrawFireLance = false;
					EffectManager.Instance.Stop(ref this.mFireBreath);
				}
				if (iOwner.mAnimationController.HasFinished && !iOwner.mAnimationController.CrossFadeEnabled)
				{
					iOwner.ChangeState(Fafnir.State.Decision);
				}
			}

			// Token: 0x06000797 RID: 1943 RVA: 0x0002F94A File Offset: 0x0002DB4A
			public void OnExit(Fafnir iOwner)
			{
				iOwner.mDrawFireLance = false;
				EffectManager.Instance.Stop(ref this.mFireBreath);
			}

			// Token: 0x06000798 RID: 1944 RVA: 0x0002F964 File Offset: 0x0002DB64
			public float GetWeight(Fafnir iOwner, float iHealth, float iPlayerWeight)
			{
				float result = 0f;
				if (iHealth <= 0.4f && iOwner.mPreviousState != this)
				{
					result = (float)Fafnir.sRandom.NextDouble();
				}
				return result;
			}

			// Token: 0x04000697 RID: 1687
			private static readonly int CEILING_AREA = "trigger_area_ceiling_debri".GetHashCodeCustom();

			// Token: 0x04000698 RID: 1688
			private static readonly int DEBRIS_EFFECT = "fafnir_debri_hit".GetHashCodeCustom();

			// Token: 0x04000699 RID: 1689
			private static readonly int DEBRIS_SOUND = "spell_earth_hit".GetHashCodeCustom();

			// Token: 0x0400069A RID: 1690
			private static readonly int DEBRI_TRAIL_EFFECT = "fafnir_debri_trail".GetHashCodeCustom();

			// Token: 0x0400069B RID: 1691
			private VisualEffectReference mFireBreath;

			// Token: 0x0400069C RID: 1692
			private ConditionCollection mDebrisConditionCollection;

			// Token: 0x0400069D RID: 1693
			private float mTimer;

			// Token: 0x0400069E RID: 1694
			private float mLanceTime;

			// Token: 0x0400069F RID: 1695
			private float mTotalLanceTime;
		}

		// Token: 0x020000FF RID: 255
		public class EarthQuakeState : IBossState<Fafnir>
		{
			// Token: 0x0600079A RID: 1946 RVA: 0x0002F9D3 File Offset: 0x0002DBD3
			public void OnEnter(Fafnir iOwner)
			{
				iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[2], 0.4f, false);
				this.mStartedAnimations = false;
			}

			// Token: 0x0600079B RID: 1947 RVA: 0x0002F9F8 File Offset: 0x0002DBF8
			public void OnUpdate(float iDeltaTime, Fafnir iOwner)
			{
				float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
				Matrix orientation = iOwner.mHeadZone.Body.Orientation;
				orientation.Translation = iOwner.mHeadZone.Body.Position;
				EffectManager.Instance.UpdateOrientation(ref this.mEffect, ref orientation);
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					if (num >= 0.5f && !this.mStartedAnimations)
					{
						do
						{
							int i = 0;
							int num2 = 0;
							while (i < Fafnir.EarthQuakeState.FLOOR_GROUPS[iOwner.mNrOfEarthquakes].Length)
							{
								int iId = Fafnir.LEVEL_PARTS[Fafnir.EarthQuakeState.FLOOR_GROUPS[iOwner.mNrOfEarthquakes][i]];
								AnimatedLevelPart animatedLevelPart = iOwner.mPlayState.Level.CurrentScene.LevelModel.GetAnimatedLevelPart(iId);
								animatedLevelPart.Play(true, -1f, -1f, 1f, false, false);
								iId = Fafnir.LAVA_EFFECTS[Fafnir.EarthQuakeState.FLOOR_GROUPS[iOwner.mNrOfEarthquakes][i]];
								iOwner.mPlayState.Level.CurrentScene.StartEffect(iId);
								i++;
								num2++;
							}
							iOwner.mNrOfEarthquakes++;
							iOwner.SetEarthQuakeThreshold();
						}
						while (iOwner.mHitPoints <= iOwner.mEarthQuakeThreshold);
						iOwner.mLavaCue = AudioManager.Instance.PlayCue(Banks.Spells, Fafnir.LAVAQUAKE_SOUND);
						AudioManager.Instance.PlayCue(Banks.Characters, Fafnir.SOUNDS[1]);
						this.mStartedAnimations = true;
						EffectManager.Instance.StartEffect(Fafnir.EarthQuakeState.EFFECT, ref orientation, out this.mEffect);
						iOwner.mPlayState.Camera.CameraShake(1.5f, 2.75f);
						return;
					}
					if (iOwner.mAnimationController.HasFinished)
					{
						iOwner.ChangeState(Fafnir.State.Decision);
					}
				}
			}

			// Token: 0x0600079C RID: 1948 RVA: 0x0002FBC0 File Offset: 0x0002DDC0
			public void OnExit(Fafnir iOwner)
			{
				if (iOwner.mLavaCue != null && !iOwner.mLavaCue.IsStopping)
				{
					iOwner.mLavaCue.Stop(AudioStopOptions.AsAuthored);
				}
				EffectManager.Instance.Stop(ref this.mEffect);
			}

			// Token: 0x040006A0 RID: 1696
			private static readonly int[][] FLOOR_GROUPS = new int[][]
			{
				new int[]
				{
					2,
					6,
					10,
					14,
					19,
					23
				},
				new int[]
				{
					0,
					5,
					9,
					13,
					18,
					20,
					15
				},
				new int[]
				{
					3,
					4,
					7,
					12,
					16,
					22
				},
				new int[]
				{
					1,
					11,
					8,
					24,
					17,
					21
				}
			};

			// Token: 0x040006A1 RID: 1697
			private static int EFFECT = "fafnir_earthquake".GetHashCodeCustom();

			// Token: 0x040006A2 RID: 1698
			private VisualEffectReference mEffect;

			// Token: 0x040006A3 RID: 1699
			private bool mStartedAnimations;
		}

		// Token: 0x02000100 RID: 256
		public class DefeatedState : IBossState<Fafnir>
		{
			// Token: 0x0600079F RID: 1951 RVA: 0x0002FCE4 File Offset: 0x0002DEE4
			public void OnEnter(Fafnir iOwner)
			{
				iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[3], 0.35f, false);
				AudioManager.Instance.PlayCue(Banks.Characters, Fafnir.SOUNDS[3], iOwner.mHeadZone.AudioEmitter);
				iOwner.mAimForTarget = false;
				iOwner.mAimForTargetWeight = 0f;
			}

			// Token: 0x060007A0 RID: 1952 RVA: 0x0002FD3C File Offset: 0x0002DF3C
			public void OnUpdate(float iDeltaTime, Fafnir iOwner)
			{
				if (!iOwner.mAnimationController.IsLooping && iOwner.mAnimationController.HasFinished && !iOwner.mAnimationController.CrossFadeEnabled)
				{
					iOwner.mPlayState.Level.CurrentScene.ExecuteTrigger(Fafnir.DEFEATED_TRIGGER, null, false);
					iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[7], 1f, true);
				}
			}

			// Token: 0x060007A1 RID: 1953 RVA: 0x0002FDA5 File Offset: 0x0002DFA5
			public void OnExit(Fafnir iOwner)
			{
			}
		}
	}
}
