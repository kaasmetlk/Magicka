using System;
using System.Collections.Generic;
using JigLibX.Collision;
using JigLibX.Geometry;
using Magicka.AI;
using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Levels.Triggers;
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
	// Token: 0x0200043A RID: 1082
	public class Death : IBoss
	{
		// Token: 0x06002183 RID: 8579 RVA: 0x000EF2F0 File Offset: 0x000ED4F0
		protected unsafe void RaiseDead(int iAmount, Death iOwner)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				if (iAmount <= 0)
				{
					return;
				}
				for (int i = 0; i < iAmount; i++)
				{
					CharacterTemplate cachedTemplate;
					switch (Death.sRandom.Next(4))
					{
					case 0:
						cachedTemplate = CharacterTemplate.GetCachedTemplate(Death.SKELETON1);
						break;
					case 1:
						cachedTemplate = CharacterTemplate.GetCachedTemplate(Death.SKELETON2);
						break;
					case 2:
						cachedTemplate = CharacterTemplate.GetCachedTemplate(Death.SKELETON3);
						break;
					default:
						cachedTemplate = CharacterTemplate.GetCachedTemplate(Death.SKELETON4);
						break;
					}
					NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(this.mPlayState);
					Matrix matrix = this.mOrientation;
					StaticList<Entity> entities = this.mPlayState.EntityManager.Entities;
					int num = 0;
					float num2;
					Vector3 randomLocation;
					do
					{
						num2 = float.MaxValue;
						randomLocation = iOwner.mCheckboxDeathArea.GetRandomLocation();
						for (int j = 0; j < entities.Count; j++)
						{
							Vector3 position = entities[j].Position;
							float val;
							Vector3.DistanceSquared(ref randomLocation, ref position, out val);
							num2 = Math.Min(val, num2);
						}
						num++;
					}
					while (num2 < 3f && num < 10);
					if (num >= 10)
					{
						return;
					}
					instance.Initialize(cachedTemplate, randomLocation, 0, 0.5f);
					instance.IsSummoned = true;
					Agent ai = instance.AI;
					ai.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, -1, 0, null);
					instance.CharacterBody.Orientation = Matrix.Identity;
					instance.CharacterBody.DesiredDirection = matrix.Forward;
					instance.SpawnAnimation = Magicka.Animations.spawn;
					this.mPlayState.EntityManager.AddEntity(instance);
					this.mSkeletonList.Add(instance);
					if (NetworkManager.Instance.State == NetworkState.Server)
					{
						Death.SpawnMessage spawnMessage;
						spawnMessage.Handle = instance.Handle;
						spawnMessage.Position = instance.Position;
						spawnMessage.Direction = matrix.Forward;
						spawnMessage.TypeID = cachedTemplate.ID;
						BossFight.Instance.SendMessage<Death.SpawnMessage>(this, 3, (void*)(&spawnMessage), true);
					}
				}
			}
		}

		// Token: 0x06002184 RID: 8580 RVA: 0x000EF4E0 File Offset: 0x000ED6E0
		public Death(PlayState iPlayState)
		{
			this.mPlayState = iPlayState;
			this.mAudioEmitter = new AudioEmitter();
			Model model;
			lock (Game.Instance.GraphicsDevice)
			{
				this.mModel = iPlayState.Content.Load<SkinnedModel>("Models/Bosses/Death/Death");
				model = iPlayState.Content.Load<Model>("Models/Bosses/Death/Death_Scythe");
				iPlayState.Content.Load<CharacterTemplate>("data/characters/skeleton_darksoul_arcane");
				iPlayState.Content.Load<CharacterTemplate>("data/characters/skeleton_darksoul_frost");
				iPlayState.Content.Load<CharacterTemplate>("data/characters/skeleton_darksoul_lightning");
				iPlayState.Content.Load<CharacterTemplate>("data/characters/skeleton_darksoul_poison");
			}
			Matrix matrix = Matrix.CreateRotationY(3.1415927f);
			foreach (SkinnedModelBone skinnedModelBone in this.mModel.SkeletonBones)
			{
				if (skinnedModelBone.Name.Equals("RightAttach", StringComparison.OrdinalIgnoreCase))
				{
					this.mHandIndex = (int)skinnedModelBone.Index;
					this.mHandBindPose = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mHandBindPose, ref matrix, out this.mHandBindPose);
					Matrix.Invert(ref this.mHandBindPose, out this.mHandBindPose);
				}
			}
			this.mEmptyMatrix = new Matrix[80];
			this.mController = new AnimationController();
			this.mController.Skeleton = this.mModel.SkeletonBones;
			this.mCloneController = new AnimationController();
			this.mCloneController.Skeleton = this.mModel.SkeletonBones;
			this.mClips = new AnimationClip[12];
			this.mClips[0] = this.mModel.AnimationClips["appear"];
			this.mClips[1] = this.mModel.AnimationClips["attack_scythe_fall"];
			this.mClips[2] = this.mModel.AnimationClips["attack_scythe_rise"];
			this.mClips[3] = this.mModel.AnimationClips["defeated"];
			this.mClips[4] = this.mModel.AnimationClips["hit"];
			this.mClips[5] = this.mModel.AnimationClips["idle"];
			this.mClips[6] = this.mModel.AnimationClips["move_glide"];
			this.mClips[7] = this.mModel.AnimationClips["sit"];
			this.mClips[8] = this.mModel.AnimationClips["sit_talk"];
			this.mClips[9] = this.mModel.AnimationClips["summon_scythe"];
			this.mClips[10] = this.mModel.AnimationClips["summon_skeleton"];
			this.mClips[11] = this.mModel.AnimationClips["timewarp"];
			SkinnedModelBasicEffect iEffect = this.mModel.Model.Meshes[0].MeshParts[0].Effect as SkinnedModelBasicEffect;
			SkinnedModelDeferredBasicMaterial mMaterial;
			Helper.SkinnedModelDeferredMaterialFromBasicEffect(iEffect, out mMaterial);
			this.mBoundingSphere = this.mModel.Model.Meshes[0].BoundingSphere;
			this.mRenderData = new Death.RenderData[3];
			this.mScytheRenderData = new Death.ItemRenderData[3];
			this.mAfterImageRenderData = new Death.AfterImageRenderData[3];
			Matrix[][] array = new Matrix[5][];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new Matrix[80];
			}
			for (int j = 0; j < 3; j++)
			{
				this.mRenderData[j] = new Death.RenderData();
				this.mRenderData[j].SetMesh(this.mModel.Model.Meshes[0].VertexBuffer, this.mModel.Model.Meshes[0].IndexBuffer, this.mModel.Model.Meshes[0].MeshParts[0], 0, 3, 4);
				this.mRenderData[j].mMaterial = mMaterial;
				this.mScytheRenderData[j] = new Death.ItemRenderData();
				this.mScytheRenderData[j].SetMesh(model.Meshes[0], model.Meshes[0].MeshParts[0], 4, 0, 5);
				this.mAfterImageRenderData[j] = new Death.AfterImageRenderData(array);
				this.mAfterImageRenderData[j].SetMesh(this.mModel.Model.Meshes[0].VertexBuffer, this.mModel.Model.Meshes[0].IndexBuffer, this.mModel.Model.Meshes[0].MeshParts[0], 0, 3, 4);
				this.mAfterImageRenderData[j].mMaterial = mMaterial;
			}
			this.mBody = new BossDamageZone(iPlayState, this, 0, 0.75f, new Capsule(Vector3.Zero, Matrix.CreateRotationX(-1.5707964f), 0.6f, 1.2f));
			this.mBody.Body.CollisionSkin.callbackFn += this.OnBodyCollision;
			VertexElement[] vertexElements;
			lock (Game.Instance.GraphicsDevice)
			{
				vertexElements = model.Meshes[0].MeshParts[0].VertexDeclaration.GetVertexElements();
			}
			int num = -1;
			for (int k = 0; k < vertexElements.Length; k++)
			{
				if (vertexElements[k].VertexElementUsage == VertexElementUsage.Position)
				{
					num = (int)vertexElements[k].Offset;
					break;
				}
			}
			if (num < 0)
			{
				throw new Exception("No positions found");
			}
			Vector3[] array2 = new Vector3[model.Meshes[0].MeshParts[0].NumVertices];
			model.Meshes[0].VertexBuffer.GetData<Vector3>(num, array2, model.Meshes[0].MeshParts[0].StartIndex, array2.Length, model.Meshes[0].MeshParts[0].VertexStride);
			BoundingBox boundingBox = BoundingBox.CreateFromPoints(array2);
			Vector3 vector;
			Vector3.Subtract(ref boundingBox.Max, ref boundingBox.Min, out vector);
			this.mScytheBody = new BossCollisionZone(iPlayState, this, new Primitive[]
			{
				new Box(new Vector3(boundingBox.Min.X, boundingBox.Max.Y * 0.5f, boundingBox.Min.Z), Matrix.Identity, new Vector3(vector.X, boundingBox.Max.Y * 0.5f, vector.Z))
			});
			this.mScytheBody.Body.CollisionSkin.callbackFn += this.OnScytheCollision;
			this.mSitIntroState = new Death.SitIntroState();
			this.mRiseIntroState = new Death.RiseIntroState();
			this.mPrepState = new Death.PrepState();
			this.mAttackState = new Death.AttackState();
			this.mMoveState = new Death.MoveState();
			this.mHitState = new Death.HitState();
			this.mIdleState = new Death.IdleState();
			this.mDefeatState = new Death.DefeatState();
			this.mDefeatPreSitState = new Death.DefeatPreSitState();
			this.mRaiseDeadState = new Death.RaiseDeadState();
			this.mWarpTimeState = new Death.WarpTimeState();
			this.mMultiDeathState = new Death.MultiDeathState();
			this.mPreMultiDeathState = new Death.PreMultiDeathState();
			this.mAppearState = new Death.AppearState();
			this.mSkeletonList = new List<NonPlayerCharacter>();
		}

		// Token: 0x06002185 RID: 8581 RVA: 0x000EFCC0 File Offset: 0x000EDEC0
		protected bool OnBodyCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
		{
			return false;
		}

		// Token: 0x06002186 RID: 8582 RVA: 0x000EFCC4 File Offset: 0x000EDEC4
		protected bool OnScytheCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
		{
			if (iSkin1.Owner != null && (this.mCurrentState is Death.AttackState | this.mCurrentState is Death.MultiDeathState))
			{
				if (iSkin1.Owner.Tag is Character && (iSkin1.Owner.Tag as Character).HitPoints > 0f)
				{
					if (!this.mIsHit && this.mController.AnimationClip == this.mClips[1] && !(iSkin1.Owner.Tag as Character).IsImmortal)
					{
						Vector3 position = (iSkin1.Owner.Tag as Character).Position;
						DamageNotifyer.Instance.AddNumber((iSkin1.Owner.Tag as Character).HitPoints, ref position, 1f, false);
						(iSkin1.Owner.Tag as Character).Damage((iSkin1.Owner.Tag as Character).HitPoints, Elements.Arcane);
					}
					return true;
				}
				if (iSkin1.Owner.Tag is Shield && !(iSkin1.Owner.Tag as Shield).Dead)
				{
					(iSkin1.Owner.Tag as Shield).Kill();
				}
			}
			return false;
		}

		// Token: 0x06002187 RID: 8583 RVA: 0x000EFE17 File Offset: 0x000EE017
		public void Initialize(ref Matrix iOrientation, int iUniqueID)
		{
			this.Initialize(ref iOrientation);
		}

		// Token: 0x06002188 RID: 8584 RVA: 0x000EFE20 File Offset: 0x000EE020
		public void Initialize(ref Matrix iOrientation)
		{
			this.mOrientation = iOrientation;
			this.mStartOrientation = iOrientation;
			this.mHitPoints = 10000f;
			this.mDead = false;
			this.mDefeated = false;
			this.mBody.IsEthereal = false;
			this.mCurrentState = this.mSitIntroState;
			this.mCurrentState.OnEnter(this);
			this.mBody.Initialize();
			this.mBody.Body.CollisionSkin.NonCollidables.Add(this.mScytheBody.Body.CollisionSkin);
			this.mPlayState.EntityManager.AddEntity(this.mBody);
			this.mScytheBody.Initialize();
			this.mScytheBody.Body.CollisionSkin.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
			this.mPlayState.EntityManager.AddEntity(this.mScytheBody);
			this.mPlayers = Game.Instance.Players;
			this.mIsHit = false;
			this.mDraw = true;
			this.mUpdate = true;
			this.mSpeed = 5f * (2f - this.mHitPoints / 10000f);
			this.mControllerSkeletonIndex = 0;
			this.mWarpTimeTTL = -99f;
			this.mAnyArea = this.mPlayState.Level.CurrentScene.GetTriggerArea(TriggerArea.ANYID);
			this.mCheckboxDeathArea = this.mPlayState.Level.CurrentScene.GetTriggerArea("checkbox_death".GetHashCodeCustom());
			this.mIsVisibleMultiDeath[0] = true;
			for (int i = 1; i < this.mIsVisibleMultiDeath.Length; i++)
			{
				this.mIsVisibleMultiDeath[i] = false;
			}
			this.mDamageFlashTimer = 0f;
		}

		// Token: 0x06002189 RID: 8585 RVA: 0x000EFFE8 File Offset: 0x000EE1E8
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
			this.mDamageFlashTimer = Math.Max(this.mDamageFlashTimer - iDeltaTime, 0f);
			this.mCurrentChannel = iDataChannel;
			iDeltaTime /= this.mPlayState.TimeModifier * this.mPlayState.TimeMultiplier;
			if (iFightStarted && this.mCurrentState is Death.SitIntroState && !this.mDead)
			{
				this.ChangeState(Death.States.RiseIntro);
			}
			if (this.mCurrentState == this.mAppearState || this.mCurrentState == this.mPreMultiDeathState)
			{
				this.mAlphaFadeSteps += iDeltaTime / this.mAppearDuration;
			}
			this.mRenderData[(int)iDataChannel].Flash = this.mDamageFlashTimer * 10f;
			this.mRenderData[(int)iDataChannel].mMaterial.Alpha = Math.Min(this.mAlphaFadeSteps, 1f);
			Vector3 origin;
			if (this.mHitPoints <= 1f && !this.mDefeated)
			{
				this.mDefeated = true;
				this.KillSkeletons();
				origin = this.mOrientation.Translation;
				Vector3 forward = this.mOrientation.Forward;
				VisualEffectReference visualEffectReference;
				EffectManager.Instance.StartEffect(Death.HIT_EFFECT, ref origin, ref forward, out visualEffectReference);
				AudioManager.Instance.PlayCue(Banks.Characters, Death.DEATH_SOUND, this.mAudioEmitter);
				this.ChangeState(Death.States.DefeatPreSit);
			}
			this.mIsVisibleMultiDeath.CopyTo(this.mRenderData[(int)iDataChannel].mDeathsIsVisible, 0);
			this.mIsVisibleMultiDeath.CopyTo(this.mScytheRenderData[(int)iDataChannel].mItemsIsVisible, 0);
			if (this.mCurrentState == this.mSitIntroState || this.mCurrentState == this.mDefeatState)
			{
				this.mScytheRenderData[(int)iDataChannel].mItemsIsVisible[0] = false;
			}
			this.mAudioEmitter.Position = this.mOrientation.Translation;
			this.mAudioEmitter.Forward = this.mOrientation.Forward;
			this.mAudioEmitter.Up = this.mOrientation.Up;
			if (!this.mDead && !this.mDefeated)
			{
				bool flag = false;
				for (int i = 0; i < this.mPlayers.Length; i++)
				{
					if (this.mPlayers[i].Playing && this.mPlayers[i].Avatar != null && !this.mPlayers[i].Avatar.Dead)
					{
						flag = true;
					}
				}
				if (!flag && this.mCurrentState != this.mIdleState && this.mController.HasFinished && !this.mController.CrossFadeEnabled)
				{
					this.ChangeState(Death.States.Idle);
				}
				else if (this.mIsHit && this.mCurrentState != this.mHitState)
				{
					this.ChangeState(Death.States.Hit);
				}
			}
			this.mCurrentState.OnUpdate(iDeltaTime, this);
			origin = this.mOrientation.Translation;
			origin.X += this.mMovement.X * this.mSpeed * iDeltaTime;
			origin.Z += this.mMovement.Z * this.mSpeed * iDeltaTime;
			Segment iSeg = default(Segment);
			iSeg.Origin = origin;
			iSeg.Origin.Y = iSeg.Origin.Y + 1f;
			iSeg.Delta.Y = iSeg.Delta.Y - 4f;
			float num;
			Vector3 translation;
			Vector3 vector;
			if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out num, out translation, out vector, iSeg))
			{
				if (this.mCurrentState is Death.SitIntroState || this.mCurrentState is Death.DefeatState)
				{
					translation.Y += 1f;
				}
				this.mOrientation.Translation = translation;
			}
			if (this.mUpdate)
			{
				Matrix matrix = this.mOrientation;
				if (this.mCurrentState != this.mSitIntroState && this.mCurrentState != this.mDefeatState)
				{
					this.mDeathFlux += iDeltaTime;
				}
				else
				{
					this.mDeathFlux = 0f;
				}
				Vector3 translation2 = matrix.Translation;
				translation2.Y += (float)Math.Cos((double)this.mDeathFlux) * 0.2f;
				matrix.Translation = translation2;
				MagickaMath.UniformMatrixScale(ref matrix, 1.2f);
				this.mController.Update(iDeltaTime, ref matrix, true);
				this.mController.SkinnedBoneTransforms.CopyTo(this.mRenderData[(int)iDataChannel].mBones[this.mControllerSkeletonIndex], 0);
				origin.Y += (this.mBody.Body.CollisionSkin.GetPrimitiveLocal(0) as Capsule).Length * 0.5f;
				origin.Y += (translation.Y - origin.Y) * iDeltaTime;
				Matrix matrix2 = this.mOrientation;
				matrix2.Translation = default(Vector3);
				this.mBody.SetOrientation(ref origin, ref matrix2);
				Matrix matrix3 = this.mHandBindPose;
				Matrix.Multiply(ref matrix3, ref this.mController.SkinnedBoneTransforms[this.mHandIndex], out matrix3);
				this.mScytheRenderData[(int)iDataChannel].WorldOrientation[this.mControllerSkeletonIndex] = matrix3;
				origin = matrix3.Translation;
				matrix3.Translation = default(Vector3);
				this.mScytheBody.SetOrientation(ref origin, ref matrix3);
			}
			else
			{
				origin = new Vector3(0f, 50f, 0f);
				Matrix identity = Matrix.Identity;
				this.mBody.SetOrientation(ref origin, ref identity);
				this.mScytheBody.SetOrientation(ref origin, ref identity);
			}
			if (this.mDraw)
			{
				this.mBoundingSphere.Center = this.mOrientation.Translation;
				this.mScytheRenderData[(int)iDataChannel].mBoundingSphere = this.mBoundingSphere;
				this.mRenderData[(int)iDataChannel].mBoundingSphere = this.mBoundingSphere;
				this.mRenderData[(int)iDataChannel].RenderAdditive = this.mBody.IsEthereal;
				if (this.mBody.IsEthereal)
				{
					this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, this.mRenderData[(int)iDataChannel]);
					this.mPlayState.Scene.AddRenderableObject(iDataChannel, this.mScytheRenderData[(int)iDataChannel]);
				}
				else
				{
					this.mPlayState.Scene.AddRenderableObject(iDataChannel, this.mRenderData[(int)iDataChannel]);
					this.mPlayState.Scene.AddRenderableObject(iDataChannel, this.mScytheRenderData[(int)iDataChannel]);
				}
				Vector3 vector2 = this.mMovement;
				vector2.Y = 0f;
				float num2 = vector2.LengthSquared();
				if (num2 > 0.1f)
				{
					Death.AfterImageRenderData afterImageRenderData = this.mAfterImageRenderData[(int)iDataChannel];
					int count = this.mModel.SkeletonBones.Count;
					this.mAfterImageTimer -= iDeltaTime;
					this.mAfterImageIntensity -= iDeltaTime;
					while (this.mAfterImageTimer <= 0f)
					{
						this.mAfterImageTimer += 0.05f;
						if (num2 > 0.001f)
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
					afterImageRenderData.RenderAdditive = this.mBody.IsEthereal;
					if (this.mBody.IsEthereal)
					{
						this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, afterImageRenderData);
						return;
					}
					this.mPlayState.Scene.AddRenderableObject(iDataChannel, afterImageRenderData);
				}
			}
		}

		// Token: 0x0600218A RID: 8586 RVA: 0x000F07DC File Offset: 0x000EE9DC
		protected unsafe void ChangeState(Death.States iState)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				this.mCurrentState.OnExit(this);
				this.mPreviousState = this.mCurrentState;
				this.mCurrentState = this.GetState(iState);
				this.mCurrentState.OnEnter(this);
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					Death.ChangeStateMessage changeStateMessage;
					changeStateMessage.NewState = iState;
					BossFight.Instance.SendMessage<Death.ChangeStateMessage>(this, 1, (void*)(&changeStateMessage), true);
				}
			}
		}

		// Token: 0x0600218B RID: 8587 RVA: 0x000F084C File Offset: 0x000EEA4C
		private IBossState<Death> GetState(Death.States iState)
		{
			switch (iState)
			{
			case Death.States.Idle:
				return this.mIdleState;
			case Death.States.SitIntro:
				return this.mSitIntroState;
			case Death.States.RiseIntro:
				return this.mRiseIntroState;
			case Death.States.Prep:
				return this.mPrepState;
			case Death.States.Attack:
				return this.mAttackState;
			case Death.States.Move:
				return this.mMoveState;
			case Death.States.Hit:
				return this.mHitState;
			case Death.States.Defeat:
				return this.mDefeatState;
			case Death.States.DefeatPreSit:
				return this.mDefeatPreSitState;
			case Death.States.WarpTime:
				return this.mWarpTimeState;
			case Death.States.RaiseDead:
				return this.mRaiseDeadState;
			case Death.States.MultiDeath:
				return this.mMultiDeathState;
			case Death.States.PreMultiDeath:
				return this.mPreMultiDeathState;
			case Death.States.Appear:
				return this.mAppearState;
			default:
				return null;
			}
		}

		// Token: 0x0600218C RID: 8588 RVA: 0x000F08FC File Offset: 0x000EEAFC
		public void DeInitialize()
		{
		}

		// Token: 0x0600218D RID: 8589 RVA: 0x000F0900 File Offset: 0x000EEB00
		private unsafe void SelectTarget()
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				int num = 4;
				int num2 = Death.sRandom.Next(Game.Instance.PlayerCount);
				for (int i = 0; i < num; i++)
				{
					if (this.mPlayers[(i + num2) % num].Playing)
					{
						Player player = this.mPlayers[(i + num2) % num];
						if (player.Avatar != null && !player.Avatar.Dead)
						{
							this.mTarget = player.Avatar;
							if (NetworkManager.Instance.State == NetworkState.Server)
							{
								Death.ChangeTargetMessage changeTargetMessage;
								changeTargetMessage.Target = this.mTarget.Handle;
								BossFight.Instance.SendMessage<Death.ChangeTargetMessage>(this, 2, (void*)(&changeTargetMessage), true);
								return;
							}
							break;
						}
					}
				}
			}
		}

		// Token: 0x0600218E RID: 8590 RVA: 0x000F09B4 File Offset: 0x000EEBB4
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

		// Token: 0x17000822 RID: 2082
		// (get) Token: 0x0600218F RID: 8591 RVA: 0x000F0A4A File Offset: 0x000EEC4A
		// (set) Token: 0x06002190 RID: 8592 RVA: 0x000F0A54 File Offset: 0x000EEC54
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

		// Token: 0x06002191 RID: 8593 RVA: 0x000F0AE0 File Offset: 0x000EECE0
		public DamageResult Damage(int iPartIndex, Damage iDamage, Entity iAttacker, ref Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			if (!this.mDraw || (this.mCurrentState is Death.MultiDeathState && this.mMultiDeathState.mIsFakeAttack))
			{
				return DamageResult.Deflected;
			}
			if (this.mDraw && !this.mBody.IsEthereal && (short)(iDamage.AttackProperty & AttackProperties.Damage) != 0 && (iDamage.Element & Elements.Life) == Elements.Life)
			{
				if (Math.Abs(iDamage.Amount) >= 0f)
				{
					if (!this.mIsHit)
					{
						if (this.mSwingCue != null)
						{
							this.mSwingCue.Stop(AudioStopOptions.AsAuthored);
						}
						AudioManager.Instance.PlayCue(Banks.Weapons, Death.SCYTHE_HIT_SOUND, this.mAudioEmitter);
						this.mIsHit = true;
						Vector3 translation = this.mOrientation.Translation;
						Vector3 right = Vector3.Right;
						VisualEffectReference visualEffectReference;
						EffectManager.Instance.StartEffect(Death.HIT_EFFECT, ref translation, ref right, out visualEffectReference);
						translation.Y += 2.2f;
						this.mDamageNotifyerNumber = DamageNotifyer.Instance.AddNumber(Math.Abs(iDamage.Amount), ref translation, 1f, false);
					}
					this.mHitPoints -= Math.Abs(iDamage.Amount);
					this.mDamageFlashTimer = 0.1f;
					if (this.mDamageNotifyerNumber >= 0)
					{
						DamageNotifyer.Instance.AddToNumber(this.mDamageNotifyerNumber, Math.Abs(iDamage.Amount));
					}
				}
				if (this.mHitPoints < 1f)
				{
					this.mHitPoints = 1f;
				}
			}
			return DamageResult.Deflected;
		}

		// Token: 0x06002192 RID: 8594 RVA: 0x000F0C6C File Offset: 0x000EEE6C
		public void Damage(int iPartIndex, float iDamage, Elements iElement)
		{
		}

		// Token: 0x06002193 RID: 8595 RVA: 0x000F0C6E File Offset: 0x000EEE6E
		public bool AddImpulseVelocity(Vector3 iDirection, float iElevation, float iMassPower, float iDistance)
		{
			return false;
		}

		// Token: 0x17000823 RID: 2083
		// (get) Token: 0x06002194 RID: 8596 RVA: 0x000F0C71 File Offset: 0x000EEE71
		public bool Dead
		{
			get
			{
				return this.mDead;
			}
		}

		// Token: 0x17000824 RID: 2084
		// (get) Token: 0x06002195 RID: 8597 RVA: 0x000F0C79 File Offset: 0x000EEE79
		public float MaxHitPoints
		{
			get
			{
				return 10000f;
			}
		}

		// Token: 0x17000825 RID: 2085
		// (get) Token: 0x06002196 RID: 8598 RVA: 0x000F0C80 File Offset: 0x000EEE80
		public float HitPoints
		{
			get
			{
				return this.mHitPoints;
			}
		}

		// Token: 0x06002197 RID: 8599 RVA: 0x000F0C88 File Offset: 0x000EEE88
		public void SetSlow(int iIndex)
		{
		}

		// Token: 0x06002198 RID: 8600 RVA: 0x000F0C8A File Offset: 0x000EEE8A
		public void GetRandomPositionOnCollisionSkin(int iIndex, out Vector3 oPosition)
		{
			oPosition = default(Vector3);
		}

		// Token: 0x06002199 RID: 8601 RVA: 0x000F0C93 File Offset: 0x000EEE93
		public bool HasStatus(int iIndex, StatusEffects iStatus)
		{
			return false;
		}

		// Token: 0x0600219A RID: 8602 RVA: 0x000F0C96 File Offset: 0x000EEE96
		public float StatusMagnitude(int iIndex, StatusEffects iStatus)
		{
			return 0f;
		}

		// Token: 0x0600219B RID: 8603 RVA: 0x000F0C9D File Offset: 0x000EEE9D
		public StatusEffect[] GetStatusEffects()
		{
			return null;
		}

		// Token: 0x0600219C RID: 8604 RVA: 0x000F0CA0 File Offset: 0x000EEEA0
		public unsafe void KillSkeletons()
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					Death.ClearMessage clearMessage;
					BossFight.Instance.SendMessage<Death.ClearMessage>(this, 4, (void*)(&clearMessage), true);
				}
				foreach (NonPlayerCharacter nonPlayerCharacter in this.mSkeletonList)
				{
					nonPlayerCharacter.OverKill();
				}
				this.mSkeletonList.Clear();
			}
		}

		// Token: 0x17000826 RID: 2086
		// (get) Token: 0x0600219D RID: 8605 RVA: 0x000F0D28 File Offset: 0x000EEF28
		// (set) Token: 0x0600219E RID: 8606 RVA: 0x000F0D35 File Offset: 0x000EEF35
		public bool IsEthereal
		{
			get
			{
				return this.mBody.IsEthereal;
			}
			set
			{
				this.mBody.IsEthereal = value;
			}
		}

		// Token: 0x0600219F RID: 8607 RVA: 0x000F0D44 File Offset: 0x000EEF44
		public void Corporealize()
		{
			for (int i = 0; i < this.mIsVisibleMultiDeath.Length; i++)
			{
				if (i != this.mIsRealDeath)
				{
					this.mIsVisibleMultiDeath[i] = false;
				}
			}
		}

		// Token: 0x060021A0 RID: 8608 RVA: 0x000F0D76 File Offset: 0x000EEF76
		public void ScriptMessage(BossMessages iMessage)
		{
		}

		// Token: 0x060021A1 RID: 8609 RVA: 0x000F0D78 File Offset: 0x000EEF78
		private unsafe void NetworkUpdate()
		{
			NetworkServer networkServer = NetworkManager.Instance.Interface as NetworkServer;
			if (networkServer == null)
			{
				return;
			}
			Death.UpdateMessage updateMessage = default(Death.UpdateMessage);
			updateMessage.Animation = 0;
			while ((int)updateMessage.Animation < this.mClips.Length && this.mController.AnimationClip != this.mClips[(int)updateMessage.Animation])
			{
				updateMessage.Animation += 1;
			}
			updateMessage.AnimationTime = this.mController.Time;
			updateMessage.Hitpoints = this.mHitPoints;
			updateMessage.Orientation = this.mOrientation;
			for (int i = 0; i < networkServer.Connections; i++)
			{
				float num = networkServer.GetLatency(i) * 0.5f;
				Death.UpdateMessage updateMessage2 = updateMessage;
				updateMessage2.AnimationTime += num;
				BossFight.Instance.SendMessage<Death.UpdateMessage>(this, 0, (void*)(&updateMessage), false, i);
			}
		}

		// Token: 0x060021A2 RID: 8610 RVA: 0x000F0E58 File Offset: 0x000EF058
		public unsafe void NetworkUpdate(ref BossUpdateMessage iMsg)
		{
			switch (iMsg.Type)
			{
			case 0:
			{
				if ((float)iMsg.TimeStamp < this.mLastNetworkUpdate)
				{
					return;
				}
				this.mLastNetworkUpdate = (float)iMsg.TimeStamp;
				Death.UpdateMessage updateMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&updateMessage));
				if (this.mController.AnimationClip != this.mClips[(int)updateMessage.Animation])
				{
					this.mController.StartClip(this.mClips[(int)updateMessage.Animation], false);
				}
				this.mController.Time = updateMessage.AnimationTime;
				this.mHitPoints = updateMessage.Hitpoints;
				this.mOrientation = updateMessage.Orientation;
				return;
			}
			case 1:
			{
				Death.ChangeStateMessage changeStateMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&changeStateMessage));
				IBossState<Death> state = this.GetState(changeStateMessage.NewState);
				if (state != null)
				{
					this.mCurrentState.OnExit(this);
					this.mPreviousState = this.mCurrentState;
					this.mCurrentState = state;
					this.mCurrentState.OnEnter(this);
				}
				return;
			}
			case 2:
			{
				Death.ChangeTargetMessage changeTargetMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&changeTargetMessage));
				if (changeTargetMessage.Target == 65535)
				{
					this.mTarget = null;
					return;
				}
				this.mTarget = (Entity.GetFromHandle((int)changeTargetMessage.Target) as Character);
				return;
			}
			case 3:
			{
				Death.SpawnMessage spawnMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&spawnMessage));
				CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(spawnMessage.TypeID);
				NonPlayerCharacter nonPlayerCharacter = Entity.GetFromHandle((int)spawnMessage.Handle) as NonPlayerCharacter;
				nonPlayerCharacter.Initialize(cachedTemplate, spawnMessage.Position, 0, 0.5f);
				nonPlayerCharacter.IsSummoned = true;
				Agent ai = nonPlayerCharacter.AI;
				ai.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, -1, 0, null);
				nonPlayerCharacter.CharacterBody.Orientation = Matrix.Identity;
				nonPlayerCharacter.CharacterBody.DesiredDirection = spawnMessage.Direction;
				nonPlayerCharacter.SpawnAnimation = Magicka.Animations.spawn;
				this.mPlayState.EntityManager.AddEntity(nonPlayerCharacter);
				this.mSkeletonList.Add(nonPlayerCharacter);
				return;
			}
			case 4:
				this.mSkeletonList.Clear();
				return;
			case 5:
			{
				Death.MultiDeathMessage multiDeathMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&multiDeathMessage));
				this.mIsRealDeath = (int)multiDeathMessage.RealDeath;
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x060021A3 RID: 8611 RVA: 0x000F1064 File Offset: 0x000EF264
		public void NetworkInitialize(ref BossInitializeMessage iMsg)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060021A4 RID: 8612 RVA: 0x000F106B File Offset: 0x000EF26B
		public BossEnum GetBossType()
		{
			return BossEnum.Death;
		}

		// Token: 0x17000827 RID: 2087
		// (get) Token: 0x060021A5 RID: 8613 RVA: 0x000F106F File Offset: 0x000EF26F
		public bool NetworkInitialized
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060021A6 RID: 8614 RVA: 0x000F1072 File Offset: 0x000EF272
		public float ResistanceAgainst(Elements iElement)
		{
			return 1f;
		}

		// Token: 0x04002445 RID: 9285
		private const float NETWORK_UPDATE_PERIOD = 0.033333335f;

		// Token: 0x04002446 RID: 9286
		private const float MAXHITPOINTS = 10000f;

		// Token: 0x04002447 RID: 9287
		private const float MOVESPEED = 5f;

		// Token: 0x04002448 RID: 9288
		private const float HITTOLERANCE = 0f;

		// Token: 0x04002449 RID: 9289
		private const int MAX_NUMBER_OF_DEATHS = 16;

		// Token: 0x0400244A RID: 9290
		private const float MIN_ATTACK_ANIMATION_SPEED = 0.65f;

		// Token: 0x0400244B RID: 9291
		private const float MAX_ATTACK_ANIMATION_SPEED = 2f;

		// Token: 0x0400244C RID: 9292
		private const float MAX_MOVE_DISTANCE = 10f;

		// Token: 0x0400244D RID: 9293
		private const float MIN_MOVE_DISTANCE = 5f;

		// Token: 0x0400244E RID: 9294
		private const float SCYTHE_ATTACK_START = 0.40625f;

		// Token: 0x0400244F RID: 9295
		private const float SCYTHE_ATTACK_END = 0.46875f;

		// Token: 0x04002450 RID: 9296
		private const float SCALE = 1.2f;

		// Token: 0x04002451 RID: 9297
		private float mLastNetworkUpdate;

		// Token: 0x04002452 RID: 9298
		protected float mNetworkUpdateTimer;

		// Token: 0x04002453 RID: 9299
		private Death.IdleState mIdleState;

		// Token: 0x04002454 RID: 9300
		private Death.SitIntroState mSitIntroState;

		// Token: 0x04002455 RID: 9301
		private Death.RiseIntroState mRiseIntroState;

		// Token: 0x04002456 RID: 9302
		private Death.PrepState mPrepState;

		// Token: 0x04002457 RID: 9303
		private Death.AttackState mAttackState;

		// Token: 0x04002458 RID: 9304
		private Death.MoveState mMoveState;

		// Token: 0x04002459 RID: 9305
		private Death.HitState mHitState;

		// Token: 0x0400245A RID: 9306
		private Death.DefeatState mDefeatState;

		// Token: 0x0400245B RID: 9307
		private Death.DefeatPreSitState mDefeatPreSitState;

		// Token: 0x0400245C RID: 9308
		private Death.WarpTimeState mWarpTimeState;

		// Token: 0x0400245D RID: 9309
		private Death.RaiseDeadState mRaiseDeadState;

		// Token: 0x0400245E RID: 9310
		private Death.MultiDeathState mMultiDeathState;

		// Token: 0x0400245F RID: 9311
		private Death.PreMultiDeathState mPreMultiDeathState;

		// Token: 0x04002460 RID: 9312
		private IBossState<Death> mPreviousState;

		// Token: 0x04002461 RID: 9313
		private Death.States mNextState;

		// Token: 0x04002462 RID: 9314
		private Death.AppearState mAppearState;

		// Token: 0x04002463 RID: 9315
		private TriggerArea mAnyArea;

		// Token: 0x04002464 RID: 9316
		private TriggerArea mCheckboxDeathArea;

		// Token: 0x04002465 RID: 9317
		private float mWarpTimeTTL;

		// Token: 0x04002466 RID: 9318
		private float mAlphaFadeSteps;

		// Token: 0x04002467 RID: 9319
		private float mAppearDuration;

		// Token: 0x04002468 RID: 9320
		private Matrix[] mEmptyMatrix;

		// Token: 0x04002469 RID: 9321
		private List<NonPlayerCharacter> mSkeletonList;

		// Token: 0x0400246A RID: 9322
		private static readonly int MAX_SKELETONS = 6;

		// Token: 0x0400246B RID: 9323
		private static readonly int HIT_EFFECT = "death_hit".GetHashCodeCustom();

		// Token: 0x0400246C RID: 9324
		private static readonly int ETHEREAL_SPAWN_EFFECT = "death_ethereal_spawn".GetHashCodeCustom();

		// Token: 0x0400246D RID: 9325
		private static readonly int ETHEREAL_DESPAWN_EFFECT = "death_ethereal_despawn".GetHashCodeCustom();

		// Token: 0x0400246E RID: 9326
		private static readonly int DEFEATED_EFFECT = "death_defeated".GetHashCodeCustom();

		// Token: 0x0400246F RID: 9327
		private static readonly int CORPOREAL_SPAWN_EFFECT = "death_corporeal_spawn".GetHashCodeCustom();

		// Token: 0x04002470 RID: 9328
		private static readonly int CORPOREAL_DESPAWN_EFFECT = "death_corporeal_despawn".GetHashCodeCustom();

		// Token: 0x04002471 RID: 9329
		private static readonly int STUNNED_EFFECT = "death_stunned".GetHashCodeCustom();

		// Token: 0x04002472 RID: 9330
		private static readonly int SCYTHE_HIT_SOUND = "wep_death_scythe".GetHashCodeCustom();

		// Token: 0x04002473 RID: 9331
		private static readonly int SCYTHE_SWING_SOUND = "wep_death_scythe_swing".GetHashCodeCustom();

		// Token: 0x04002474 RID: 9332
		private static readonly int DEATH_SOUND = "boss_death_death".GetHashCodeCustom();

		// Token: 0x04002475 RID: 9333
		private static readonly int SKELETON1 = "skeleton_darksoul_arcane".GetHashCodeCustom();

		// Token: 0x04002476 RID: 9334
		private static readonly int SKELETON2 = "skeleton_darksoul_frost".GetHashCodeCustom();

		// Token: 0x04002477 RID: 9335
		private static readonly int SKELETON3 = "skeleton_darksoul_lightning".GetHashCodeCustom();

		// Token: 0x04002478 RID: 9336
		private static readonly int SKELETON4 = "skeleton_darksoul_poison".GetHashCodeCustom();

		// Token: 0x04002479 RID: 9337
		private static readonly float TIME_SLOW_DOWN = 0.5f;

		// Token: 0x0400247A RID: 9338
		private float mHitPoints;

		// Token: 0x0400247B RID: 9339
		private int mDamageNotifyerNumber;

		// Token: 0x0400247C RID: 9340
		private bool mDead;

		// Token: 0x0400247D RID: 9341
		private bool mDefeated;

		// Token: 0x0400247E RID: 9342
		private bool mDraw;

		// Token: 0x0400247F RID: 9343
		private bool mUpdate;

		// Token: 0x04002480 RID: 9344
		private int mControllerSkeletonIndex;

		// Token: 0x04002481 RID: 9345
		private static Random sRandom = new Random();

		// Token: 0x04002482 RID: 9346
		private bool[] mIsVisibleMultiDeath = new bool[16];

		// Token: 0x04002483 RID: 9347
		private int mIsRealDeath;

		// Token: 0x04002484 RID: 9348
		private int mCurrentMaxDeaths;

		// Token: 0x04002485 RID: 9349
		private Vector3 mMultiDeathOffset;

		// Token: 0x04002486 RID: 9350
		private Matrix mMultiDeathOrientation;

		// Token: 0x04002487 RID: 9351
		private BossDamageZone mBody;

		// Token: 0x04002488 RID: 9352
		private BossCollisionZone mScytheBody;

		// Token: 0x04002489 RID: 9353
		private PlayState mPlayState;

		// Token: 0x0400248A RID: 9354
		private AudioEmitter mAudioEmitter;

		// Token: 0x0400248B RID: 9355
		private Cue mSwingCue;

		// Token: 0x0400248C RID: 9356
		private float mAfterImageTimer;

		// Token: 0x0400248D RID: 9357
		private float mAfterImageIntensity;

		// Token: 0x0400248E RID: 9358
		private float mDeathFlux;

		// Token: 0x0400248F RID: 9359
		private Death.AfterImageRenderData[] mAfterImageRenderData;

		// Token: 0x04002490 RID: 9360
		private Death.RenderData[] mRenderData;

		// Token: 0x04002491 RID: 9361
		private Death.ItemRenderData[] mScytheRenderData;

		// Token: 0x04002492 RID: 9362
		private AnimationController mController;

		// Token: 0x04002493 RID: 9363
		private AnimationController mCloneController;

		// Token: 0x04002494 RID: 9364
		private AnimationClip[] mClips;

		// Token: 0x04002495 RID: 9365
		private SkinnedModel mModel;

		// Token: 0x04002496 RID: 9366
		private Character mTarget;

		// Token: 0x04002497 RID: 9367
		private int mHandIndex;

		// Token: 0x04002498 RID: 9368
		private Matrix mHandBindPose;

		// Token: 0x04002499 RID: 9369
		private float mSpeed;

		// Token: 0x0400249A RID: 9370
		private Vector3 mMovement;

		// Token: 0x0400249B RID: 9371
		private Matrix mStartOrientation;

		// Token: 0x0400249C RID: 9372
		private Matrix mOrientation;

		// Token: 0x0400249D RID: 9373
		private bool mIsHit;

		// Token: 0x0400249E RID: 9374
		private IBossState<Death> mCurrentState;

		// Token: 0x0400249F RID: 9375
		private DataChannel mCurrentChannel;

		// Token: 0x040024A0 RID: 9376
		private BoundingSphere mBoundingSphere;

		// Token: 0x040024A1 RID: 9377
		private float mDamageFlashTimer;

		// Token: 0x040024A2 RID: 9378
		private Player[] mPlayers;

		// Token: 0x040024A3 RID: 9379
		public static readonly int SOUND_HASH = "misc_flash".GetHashCodeCustom();

		// Token: 0x0200043B RID: 1083
		public class ItemRenderData : RenderableObject<RenderDeferredEffect, RenderDeferredMaterial>, IRenderableAdditiveObject
		{
			// Token: 0x060021A8 RID: 8616 RVA: 0x000F1184 File Offset: 0x000EF384
			public ItemRenderData()
			{
				this.WorldOrientation = new Matrix[16];
				this.mItemsIsVisible = new bool[16];
				for (int i = 0; i < 16; i++)
				{
					this.mItemsIsVisible[i] = false;
				}
			}

			// Token: 0x060021A9 RID: 8617 RVA: 0x000F11C8 File Offset: 0x000EF3C8
			public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				RenderDeferredEffect renderDeferredEffect = iEffect as RenderDeferredEffect;
				this.mMaterial.AssignToEffect(renderDeferredEffect);
				for (int i = 0; i < this.mItemsIsVisible.Length; i++)
				{
					renderDeferredEffect.World = this.WorldOrientation[i];
					this.mMaterial.WorldTransform = this.WorldOrientation[i];
					renderDeferredEffect.CommitChanges();
					if (this.mItemsIsVisible[i])
					{
						renderDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
					}
				}
			}

			// Token: 0x060021AA RID: 8618 RVA: 0x000F1264 File Offset: 0x000EF464
			public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				RenderDeferredEffect renderDeferredEffect = iEffect as RenderDeferredEffect;
				this.mMaterial.AssignOpacityToEffect(renderDeferredEffect);
				for (int i = 0; i < this.mItemsIsVisible.Length; i++)
				{
					renderDeferredEffect.World = this.WorldOrientation[i];
					this.mMaterial.WorldTransform = this.WorldOrientation[i];
					renderDeferredEffect.CommitChanges();
					if (this.mItemsIsVisible[i])
					{
						renderDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
					}
				}
			}

			// Token: 0x040024A4 RID: 9380
			public Matrix[] WorldOrientation;

			// Token: 0x040024A5 RID: 9381
			public bool[] mItemsIsVisible;
		}

		// Token: 0x0200043C RID: 1084
		public class RenderData : RenderableObject<SkinnedModelDeferredEffect, SkinnedModelDeferredBasicMaterial>, IRenderableAdditiveObject
		{
			// Token: 0x060021AB RID: 8619 RVA: 0x000F1300 File Offset: 0x000EF500
			public RenderData()
			{
				this.mBones = new Matrix[16][];
				for (int i = 0; i < this.mBones.Length; i++)
				{
					this.mBones[i] = new Matrix[80];
				}
				this.mDeathsIsVisible = new bool[16];
				for (int j = 0; j < this.mDeathsIsVisible.Length; j++)
				{
					this.mDeathsIsVisible[j] = false;
				}
			}

			// Token: 0x17000828 RID: 2088
			// (get) Token: 0x060021AC RID: 8620 RVA: 0x000F136B File Offset: 0x000EF56B
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

			// Token: 0x060021AD RID: 8621 RVA: 0x000F1380 File Offset: 0x000EF580
			public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				this.mMaterial.AssignToEffect(skinnedModelDeferredEffect);
				skinnedModelDeferredEffect.ProjectionMapEnabled = false;
				for (int i = 0; i < this.mDeathsIsVisible.Length; i++)
				{
					skinnedModelDeferredEffect.Bones = this.mBones[i];
					skinnedModelDeferredEffect.CommitChanges();
					if (this.mDeathsIsVisible[i])
					{
						skinnedModelDeferredEffect.OverrideColor = new Vector4(1f, 1f, 1f, this.Flash);
						skinnedModelDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
					}
				}
				skinnedModelDeferredEffect.OverrideColor = Vector4.Zero;
				skinnedModelDeferredEffect.Colorize = default(Vector4);
			}

			// Token: 0x060021AE RID: 8622 RVA: 0x000F1438 File Offset: 0x000EF638
			public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				this.mMaterial.AssignOpacityToEffect(skinnedModelDeferredEffect);
				for (int i = 0; i < this.mDeathsIsVisible.Length; i++)
				{
					skinnedModelDeferredEffect.Bones = this.mBones[i];
					skinnedModelDeferredEffect.CommitChanges();
					if (this.mDeathsIsVisible[i])
					{
						skinnedModelDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
					}
				}
			}

			// Token: 0x040024A6 RID: 9382
			public bool RenderAdditive;

			// Token: 0x040024A7 RID: 9383
			public float Flash;

			// Token: 0x040024A8 RID: 9384
			public Matrix[][] mBones;

			// Token: 0x040024A9 RID: 9385
			public bool[] mDeathsIsVisible;
		}

		// Token: 0x0200043D RID: 1085
		protected class AfterImageRenderData : RenderableObject<SkinnedModelDeferredEffect, SkinnedModelDeferredBasicMaterial>, IRenderableAdditiveObject
		{
			// Token: 0x060021AF RID: 8623 RVA: 0x000F14AE File Offset: 0x000EF6AE
			public AfterImageRenderData(Matrix[][] iSkeleton)
			{
				this.mSkeleton = iSkeleton;
			}

			// Token: 0x17000829 RID: 2089
			// (get) Token: 0x060021B0 RID: 8624 RVA: 0x000F14BD File Offset: 0x000EF6BD
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

			// Token: 0x060021B1 RID: 8625 RVA: 0x000F14D0 File Offset: 0x000EF6D0
			public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				this.mMaterial.AssignToEffect(skinnedModelDeferredEffect);
				skinnedModelDeferredEffect.ProjectionMapEnabled = false;
				skinnedModelDeferredEffect.Colorize = new Vector4(Death.AfterImageRenderData.ColdColor, 1f);
				float num = 0.333f;
				float num2 = 0.333f / ((float)this.mSkeleton.Length + 1f);
				num += this.mIntensity * num2;
				for (int i = 0; i < this.mSkeleton.Length; i++)
				{
					if (num != 0f)
					{
						skinnedModelDeferredEffect.Alpha = num;
						skinnedModelDeferredEffect.Bones = this.mSkeleton[i];
						skinnedModelDeferredEffect.CommitChanges();
						skinnedModelDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
						num -= num2;
					}
				}
				skinnedModelDeferredEffect.Colorize = default(Vector4);
			}

			// Token: 0x060021B2 RID: 8626 RVA: 0x000F15A2 File Offset: 0x000EF7A2
			public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
			}

			// Token: 0x040024AA RID: 9386
			protected static readonly Vector3 ColdColor = new Vector3(1f, 1.6f, 2f);

			// Token: 0x040024AB RID: 9387
			public float mIntensity;

			// Token: 0x040024AC RID: 9388
			public Matrix[][] mSkeleton;

			// Token: 0x040024AD RID: 9389
			public bool RenderAdditive;
		}

		// Token: 0x0200043E RID: 1086
		public class SitIntroState : IBossState<Death>
		{
			// Token: 0x060021B4 RID: 8628 RVA: 0x000F15BF File Offset: 0x000EF7BF
			public void OnEnter(Death iOwner)
			{
				iOwner.mSpeed = 0f;
				iOwner.mOrientation = iOwner.mStartOrientation;
				iOwner.mDraw = true;
				iOwner.mController.StartClip(iOwner.mClips[8], true);
			}

			// Token: 0x060021B5 RID: 8629 RVA: 0x000F15F3 File Offset: 0x000EF7F3
			public void OnUpdate(float iDeltaTime, Death iOwner)
			{
			}

			// Token: 0x060021B6 RID: 8630 RVA: 0x000F15F5 File Offset: 0x000EF7F5
			public void OnExit(Death iOwner)
			{
				iOwner.mPreviousState = iOwner.mCurrentState;
			}
		}

		// Token: 0x0200043F RID: 1087
		public class RiseIntroState : IBossState<Death>
		{
			// Token: 0x060021B8 RID: 8632 RVA: 0x000F160B File Offset: 0x000EF80B
			public void OnEnter(Death iOwner)
			{
				iOwner.mDraw = true;
				iOwner.mController.CrossFade(iOwner.mClips[9], 0.15f, false);
				this.mIdleTimer = 0.15f;
				iOwner.IsEthereal = true;
			}

			// Token: 0x060021B9 RID: 8633 RVA: 0x000F1640 File Offset: 0x000EF840
			public void OnUpdate(float iDeltaTime, Death iOwner)
			{
				iOwner.Movement = iOwner.mOrientation.Forward;
				iOwner.mSpeed = 1f;
				if (NetworkManager.Instance.State != NetworkState.Client)
				{
					if (iOwner.mController.HasFinished && !iOwner.mController.CrossFadeEnabled)
					{
						this.mIdleTimer -= iDeltaTime;
					}
					if (this.mIdleTimer <= 0f)
					{
						iOwner.ChangeState(Death.States.Prep);
					}
				}
			}

			// Token: 0x060021BA RID: 8634 RVA: 0x000F16B2 File Offset: 0x000EF8B2
			public void OnExit(Death iOwner)
			{
				iOwner.mSpeed = 5f * (2f - iOwner.mHitPoints / 10000f);
				iOwner.mPreviousState = iOwner.mCurrentState;
			}

			// Token: 0x040024AE RID: 9390
			private float mIdleTimer;
		}

		// Token: 0x02000440 RID: 1088
		public class IdleState : IBossState<Death>
		{
			// Token: 0x060021BC RID: 8636 RVA: 0x000F16E6 File Offset: 0x000EF8E6
			public void OnEnter(Death iOwner)
			{
				iOwner.mDraw = true;
				iOwner.mController.CrossFade(iOwner.mClips[5], 0.2f, true);
			}

			// Token: 0x060021BD RID: 8637 RVA: 0x000F1708 File Offset: 0x000EF908
			public void OnUpdate(float iDeltaTime, Death iOwner)
			{
				for (int i = 0; i < iOwner.mPlayers.Length; i++)
				{
					if (iOwner.mPlayers[i].Playing && iOwner.mPlayers[i].Avatar != null && !iOwner.mPlayers[i].Avatar.Dead)
					{
						iOwner.ChangeState(Death.States.Prep);
						return;
					}
				}
			}

			// Token: 0x060021BE RID: 8638 RVA: 0x000F1762 File Offset: 0x000EF962
			public void OnExit(Death iOwner)
			{
			}
		}

		// Token: 0x02000441 RID: 1089
		public class PrepState : IBossState<Death>
		{
			// Token: 0x060021C0 RID: 8640 RVA: 0x000F176C File Offset: 0x000EF96C
			public void OnEnter(Death iOwner)
			{
				iOwner.mDraw = false;
				iOwner.mBody.IsEthereal = true;
				iOwner.mController.StartClip(iOwner.mClips[6], true);
				float num = iOwner.mHitPoints / 10000f;
				this.mIdleTimer = 1f + 3f * (float)(0.5 + Death.sRandom.NextDouble() * 0.5) * num;
				Vector3 translation = iOwner.mOrientation.Translation;
				Vector3 forward = iOwner.mOrientation.Forward;
				VisualEffectReference visualEffectReference;
				EffectManager.Instance.StartEffect(Death.ETHEREAL_DESPAWN_EFFECT, ref translation, ref forward, out visualEffectReference);
				iOwner.SelectTarget();
			}

			// Token: 0x060021C1 RID: 8641 RVA: 0x000F1814 File Offset: 0x000EFA14
			public void OnUpdate(float iDeltaTime, Death iOwner)
			{
				if (NetworkManager.Instance.State == NetworkState.Client)
				{
					return;
				}
				this.mIdleTimer -= iDeltaTime;
				if (this.mIdleTimer <= 0f)
				{
					float num = iOwner.mHitPoints / 10000f;
					if (iOwner.mTarget != null)
					{
						bool flag = false;
						while (!flag)
						{
							int num2 = Death.sRandom.Next(10);
							if (num2 < 3 & iOwner.mPreviousState != iOwner.mRaiseDeadState)
							{
								this.PositionDeath(iOwner, 1f);
								iOwner.mNextState = Death.States.RaiseDead;
								iOwner.ChangeState(Death.States.Appear);
								flag = true;
							}
							else if (num2 < 6 & iOwner.mHitPoints / 10000f < 0.8f & iOwner.mPreviousState != iOwner.mMultiDeathState)
							{
								this.PositionDeath(iOwner, num);
								iOwner.ChangeState(Death.States.PreMultiDeath);
								flag = true;
							}
							else if (num2 < 9 & iOwner.mHitPoints / 10000f < 0.5f & iOwner.mPreviousState != iOwner.mWarpTimeState & iOwner.mWarpTimeTTL <= -5f)
							{
								this.PositionDeath(iOwner, 1f);
								iOwner.mNextState = Death.States.WarpTime;
								iOwner.ChangeState(Death.States.Appear);
								flag = true;
							}
							else
							{
								this.PositionDeath(iOwner, num);
								iOwner.mNextState = Death.States.Move;
								iOwner.ChangeState(Death.States.Appear);
								flag = true;
							}
						}
						return;
					}
					this.mIdleTimer = 1f + 3f * (float)(0.5 + Death.sRandom.NextDouble() * 0.5) * num;
				}
			}

			// Token: 0x060021C2 RID: 8642 RVA: 0x000F19A5 File Offset: 0x000EFBA5
			public void OnExit(Death iOwner)
			{
			}

			// Token: 0x060021C3 RID: 8643 RVA: 0x000F19A8 File Offset: 0x000EFBA8
			internal void PositionDeath(Death iOwner, float iDifficulty)
			{
				Vector3 position = iOwner.mTarget.Position;
				Vector3 direction = iOwner.mTarget.Direction;
				Vector3 vector;
				Vector3.Negate(ref direction, out vector);
				float scaleFactor = MathHelper.Lerp(5f, 10f, iDifficulty);
				Vector3.Multiply(ref vector, scaleFactor, out vector);
				Vector3 translation;
				Vector3.Add(ref position, ref vector, out translation);
				Vector3 vector2;
				if (iOwner.mPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref translation, out vector2, MovementProperties.Default) < 7f)
				{
					translation = vector2;
				}
				Vector3 forward;
				Vector3.Subtract(ref position, ref translation, out forward);
				forward.Y = 0f;
				forward.Normalize();
				Vector3 up = Vector3.Up;
				Vector3 right;
				Vector3.Cross(ref forward, ref up, out right);
				iOwner.mOrientation.Forward = forward;
				iOwner.mOrientation.Up = up;
				iOwner.mOrientation.Right = right;
				iOwner.mOrientation.Translation = translation;
			}

			// Token: 0x040024AF RID: 9391
			private float mIdleTimer;
		}

		// Token: 0x02000442 RID: 1090
		public class MoveState : IBossState<Death>
		{
			// Token: 0x060021C5 RID: 8645 RVA: 0x000F1A94 File Offset: 0x000EFC94
			public void OnEnter(Death iOwner)
			{
				iOwner.mBody.IsEthereal = true;
				iOwner.mDraw = true;
				iOwner.mController.CrossFade(iOwner.mClips[6], 0.2f, true);
				iOwner.mController.Update(0.001f, ref iOwner.mOrientation, true);
				int count = iOwner.mController.Skeleton.Count;
				for (int i = 0; i < 3; i++)
				{
					Death.AfterImageRenderData afterImageRenderData = iOwner.mAfterImageRenderData[i];
					for (int j = 0; j < afterImageRenderData.mSkeleton.Length; j++)
					{
						Array.Copy(iOwner.mController.SkinnedBoneTransforms, afterImageRenderData.mSkeleton[j], count);
					}
				}
			}

			// Token: 0x060021C6 RID: 8646 RVA: 0x000F1B38 File Offset: 0x000EFD38
			public void OnUpdate(float iDeltaTime, Death iOwner)
			{
				if (NetworkManager.Instance.State == NetworkState.Client)
				{
					return;
				}
				if (iOwner.mTarget == null)
				{
					iOwner.ChangeState(Death.States.Prep);
					return;
				}
				if (iOwner.mSpeed <= 0.1f)
				{
					iOwner.mSpeed = 5f * (2f - iOwner.mHitPoints / 10000f);
				}
				Vector3 position = iOwner.mTarget.Position;
				Vector3 translation = iOwner.mOrientation.Translation;
				Vector3 movement;
				Vector3.Subtract(ref position, ref translation, out movement);
				movement.Y = 0f;
				float num = movement.Length();
				movement.Normalize();
				iOwner.Movement = movement;
				iOwner.Turn(ref movement, 8f, iDeltaTime);
				if (num < 2.5f)
				{
					iOwner.ChangeState(Death.States.Attack);
				}
			}

			// Token: 0x060021C7 RID: 8647 RVA: 0x000F1BF0 File Offset: 0x000EFDF0
			public void OnExit(Death iOwner)
			{
				iOwner.mBody.IsEthereal = false;
				iOwner.Movement = Vector3.Zero;
			}
		}

		// Token: 0x02000443 RID: 1091
		public class AttackState : IBossState<Death>
		{
			// Token: 0x060021C9 RID: 8649 RVA: 0x000F1C14 File Offset: 0x000EFE14
			public void OnEnter(Death iOwner)
			{
				this.mScytheLifted = false;
				iOwner.mController.CrossFade(iOwner.mClips[2], 0.2f, false);
				iOwner.mSwingCue = AudioManager.Instance.PlayCue(Banks.Weapons, Death.SCYTHE_SWING_SOUND, iOwner.mAudioEmitter);
			}

			// Token: 0x060021CA RID: 8650 RVA: 0x000F1C64 File Offset: 0x000EFE64
			public void OnUpdate(float iDeltaTime, Death iOwner)
			{
				if (iOwner.mSpeed <= 0.1f)
				{
					iOwner.mSpeed = 1f;
				}
				if (iOwner.mTarget == null)
				{
					iOwner.ChangeState(Death.States.Prep);
					return;
				}
				if (NetworkManager.Instance.State != NetworkState.Client)
				{
					Vector3 position = iOwner.mTarget.Position;
					Vector3 translation = iOwner.mOrientation.Translation;
					Vector3 movement;
					Vector3.Subtract(ref position, ref translation, out movement);
					movement.Y = 0f;
					float num = movement.Length();
					movement.Normalize();
					if (iOwner.mTarget.Radius + iOwner.mBody.Radius < num)
					{
						float num2 = num / 2.5f * 5f * (2f - iOwner.mHitPoints / 10000f);
						iOwner.mSpeed = num2;
						iOwner.Movement = movement;
						if (num2 < 8f)
						{
							num2 = 8f;
						}
						iOwner.Turn(ref movement, num2, iDeltaTime);
					}
					else
					{
						iOwner.mSpeed = 0.1f;
					}
				}
				if (iOwner.mController.HasFinished && !iOwner.mController.CrossFadeEnabled)
				{
					if (!this.mScytheLifted)
					{
						iOwner.mController.CrossFade(iOwner.mClips[1], 0.2f, false);
						this.mScytheLifted = true;
						return;
					}
					iOwner.ChangeState(Death.States.Prep);
				}
			}

			// Token: 0x060021CB RID: 8651 RVA: 0x000F1DA4 File Offset: 0x000EFFA4
			public void OnExit(Death iOwner)
			{
				iOwner.mSpeed = 5f * (2f - iOwner.mHitPoints / 10000f);
				iOwner.Movement = Vector3.Zero;
				iOwner.mController.Speed = 1f;
				iOwner.mPreviousState = iOwner.mCurrentState;
				for (int i = 0; i < 3; i++)
				{
					Death.AfterImageRenderData afterImageRenderData = iOwner.mAfterImageRenderData[i];
					for (int j = 0; j < afterImageRenderData.mSkeleton.Length; j++)
					{
						iOwner.mEmptyMatrix.CopyTo(afterImageRenderData.mSkeleton[j], 0);
					}
				}
			}

			// Token: 0x040024B0 RID: 9392
			private bool mScytheLifted;
		}

		// Token: 0x02000444 RID: 1092
		public class HitState : IBossState<Death>
		{
			// Token: 0x060021CD RID: 8653 RVA: 0x000F1E3C File Offset: 0x000F003C
			public void OnEnter(Death iOwner)
			{
				iOwner.mController.CrossFade(iOwner.mClips[4], 0.2f, false);
				Vector3 translation = iOwner.mOrientation.Translation;
				Vector3 forward = iOwner.mOrientation.Forward;
				EffectManager.Instance.StartEffect(Death.STUNNED_EFFECT, ref translation, ref forward, out this.e);
			}

			// Token: 0x060021CE RID: 8654 RVA: 0x000F1E94 File Offset: 0x000F0094
			public void OnUpdate(float iDeltaTime, Death iOwner)
			{
				if (iOwner.mController.HasFinished && !iOwner.mController.CrossFadeEnabled)
				{
					iOwner.ChangeState(Death.States.Prep);
				}
			}

			// Token: 0x060021CF RID: 8655 RVA: 0x000F1EB7 File Offset: 0x000F00B7
			public void OnExit(Death iOwner)
			{
				EffectManager.Instance.Stop(ref this.e);
				iOwner.mDraw = false;
				iOwner.mIsHit = false;
				iOwner.mDamageNotifyerNumber = -1;
				if (iOwner.mHitPoints < 1f)
				{
					iOwner.mHitPoints = 1f;
				}
			}

			// Token: 0x040024B1 RID: 9393
			private VisualEffectReference e;
		}

		// Token: 0x02000445 RID: 1093
		public class DefeatState : IBossState<Death>
		{
			// Token: 0x060021D1 RID: 8657 RVA: 0x000F1EFE File Offset: 0x000F00FE
			public void OnEnter(Death iOwner)
			{
				iOwner.IsEthereal = false;
				iOwner.mDraw = true;
				iOwner.mOrientation = iOwner.mStartOrientation;
				iOwner.mController.CrossFade(iOwner.mClips[8], 0.2f, false);
				iOwner.mHitPoints = 0f;
			}

			// Token: 0x060021D2 RID: 8658 RVA: 0x000F1F3E File Offset: 0x000F013E
			public void OnUpdate(float iDeltaTime, Death iOwner)
			{
			}

			// Token: 0x060021D3 RID: 8659 RVA: 0x000F1F40 File Offset: 0x000F0140
			public void OnExit(Death iOwner)
			{
			}
		}

		// Token: 0x02000446 RID: 1094
		public class DefeatPreSitState : IBossState<Death>
		{
			// Token: 0x060021D5 RID: 8661 RVA: 0x000F1F4C File Offset: 0x000F014C
			public void OnEnter(Death iOwner)
			{
				iOwner.IsEthereal = false;
				iOwner.mDraw = true;
				iOwner.mController.CrossFade(iOwner.mClips[3], 0.2f, false);
				Vector3 translation = iOwner.mOrientation.Translation;
				Vector3 forward = iOwner.mOrientation.Forward;
				EffectManager.Instance.StartEffect(Death.DEFEATED_EFFECT, ref translation, ref forward, out this.e);
			}

			// Token: 0x060021D6 RID: 8662 RVA: 0x000F1FB2 File Offset: 0x000F01B2
			public void OnUpdate(float iDeltaTime, Death iOwner)
			{
				if (iOwner.mController.HasFinished && !iOwner.mController.CrossFadeEnabled)
				{
					iOwner.ChangeState(Death.States.Defeat);
				}
			}

			// Token: 0x060021D7 RID: 8663 RVA: 0x000F1FD5 File Offset: 0x000F01D5
			public void OnExit(Death iOwner)
			{
				EffectManager.Instance.Stop(ref this.e);
			}

			// Token: 0x040024B2 RID: 9394
			private VisualEffectReference e;
		}

		// Token: 0x02000447 RID: 1095
		public class PreMultiDeathSweepingCircleState : IBossState<Death>
		{
			// Token: 0x060021D9 RID: 8665 RVA: 0x000F1FF0 File Offset: 0x000F01F0
			public void OnEnter(Death iOwner)
			{
				iOwner.mBody.IsEthereal = true;
				iOwner.mDraw = true;
				iOwner.KillSkeletons();
				for (int i = 0; i < 4; i++)
				{
					if ((iOwner.mTarget as Avatar).Player.ID != i)
					{
						if (iOwner.mPlayers[i].Playing)
						{
							iOwner.mPlayers[i].Avatar.IsEthereal = true;
						}
						ControlManager.Instance.LockPlayerInput(i);
					}
				}
				iOwner.mCloneController.StartClip(iOwner.mClips[0], false);
				iOwner.mUpdate = false;
				iOwner.Movement = Vector3.Zero;
				Vector3 translation = iOwner.mOrientation.Translation;
				Vector3 position = iOwner.mTarget.Position;
				Vector3 vector;
				Vector3.Subtract(ref position, ref translation, out vector);
				vector.Y = 0f;
				float z = vector.Length();
				iOwner.mMultiDeathOrientation = iOwner.mOrientation;
				iOwner.mMultiDeathOffset = new Vector3(0f, 0f, z);
				this.mSpawnTimer = 0f;
				if (iOwner.mHitPoints / iOwner.MaxHitPoints > 0.6666667f)
				{
					iOwner.mCurrentMaxDeaths = 8;
				}
				else if (iOwner.mHitPoints / iOwner.MaxHitPoints > 0.33333334f)
				{
					iOwner.mCurrentMaxDeaths = 12;
				}
				else
				{
					iOwner.mCurrentMaxDeaths = 16;
				}
				this.mCount = 1;
				iOwner.mIsVisibleMultiDeath[0] = true;
				AudioManager.Instance.PlayCue(Banks.Misc, Death.SOUND_HASH);
				Flash.Instance.Execute(iOwner.mPlayState.Scene, 0.2f);
				iOwner.mPlayState.Camera.Magnification = 1.2f;
				iOwner.mPlayState.Camera.AttachPlayers(iOwner.mTarget);
			}

			// Token: 0x060021DA RID: 8666 RVA: 0x000F21A0 File Offset: 0x000F03A0
			public void OnUpdate(float iDeltaTime, Death iOwner)
			{
				if (iOwner.mTarget == null | iOwner.mTarget.Dead)
				{
					iOwner.ChangeState(Death.States.Prep);
					return;
				}
				this.mSpawnTimer += iDeltaTime;
				bool flag = false;
				if (this.mSpawnTimer > 1f)
				{
					iOwner.ChangeState(Death.States.MultiDeath);
				}
				if (this.mSpawnTimer / 1f >= (float)this.mCount / (float)iOwner.mCurrentMaxDeaths & this.mCount < iOwner.mCurrentMaxDeaths)
				{
					flag = true;
					iOwner.mIsVisibleMultiDeath[this.mCount] = true;
					this.mCount++;
				}
				Matrix identity = Matrix.Identity;
				iOwner.mCloneController.Update(iDeltaTime, ref identity, true);
				Vector3 mMultiDeathOffset = iOwner.mMultiDeathOffset;
				Matrix mOrientation = iOwner.mOrientation;
				Vector3.TransformNormal(ref mMultiDeathOffset, ref mOrientation, out mMultiDeathOffset);
				float num = Math.Min(this.mSpawnTimer / 1f, 1f);
				Matrix matrix;
				Matrix.CreateRotationY(1f / (float)iOwner.mCurrentMaxDeaths * 6.2831855f * num, out matrix);
				for (int i = 0; i < iOwner.mCurrentMaxDeaths; i++)
				{
					Matrix mHandBindPose = iOwner.mHandBindPose;
					mOrientation.Translation = default(Vector3);
					Matrix.Multiply(ref matrix, ref mOrientation, out mOrientation);
					Vector3.TransformNormal(ref mMultiDeathOffset, ref matrix, out mMultiDeathOffset);
					mOrientation.Translation = mMultiDeathOffset + iOwner.mTarget.Position;
					if (flag)
					{
						Vector3 translation = iOwner.mMultiDeathOrientation.Translation;
						Vector3 right = Vector3.Right;
						VisualEffectReference visualEffectReference;
						EffectManager.Instance.StartEffect(Death.ETHEREAL_SPAWN_EFFECT, ref translation, ref right, out visualEffectReference);
						flag = false;
					}
					for (int j = 0; j < iOwner.mCloneController.Skeleton.Count; j++)
					{
						Matrix.Multiply(ref iOwner.mCloneController.SkinnedBoneTransforms[j], ref mOrientation, out iOwner.mRenderData[(int)iOwner.mCurrentChannel].mBones[i][j]);
					}
					Matrix.Multiply(ref mHandBindPose, ref iOwner.mRenderData[(int)iOwner.mCurrentChannel].mBones[i][iOwner.mHandIndex], out mHandBindPose);
					iOwner.mScytheRenderData[(int)iOwner.mCurrentChannel].WorldOrientation[i] = mHandBindPose;
				}
			}

			// Token: 0x060021DB RID: 8667 RVA: 0x000F23D1 File Offset: 0x000F05D1
			public void OnExit(Death iOwner)
			{
			}

			// Token: 0x040024B3 RID: 9395
			private float mSpawnTimer;

			// Token: 0x040024B4 RID: 9396
			private int mCount;
		}

		// Token: 0x02000448 RID: 1096
		public class PreMultiDeathState : IBossState<Death>
		{
			// Token: 0x060021DD RID: 8669 RVA: 0x000F23DC File Offset: 0x000F05DC
			public void OnEnter(Death iOwner)
			{
				iOwner.mBody.IsEthereal = true;
				iOwner.mDraw = true;
				iOwner.SelectTarget();
				iOwner.KillSkeletons();
				iOwner.mPlayState.TimeModifier = Death.TIME_SLOW_DOWN;
				iOwner.mTarget.TimeWarpModifier = 1f / Death.TIME_SLOW_DOWN;
				for (int i = 0; i < 4; i++)
				{
					if ((iOwner.mTarget as Avatar).Player.ID != i)
					{
						if (iOwner.mPlayers[i].Playing)
						{
							iOwner.mPlayers[i].Avatar.IsEthereal = true;
						}
						ControlManager.Instance.LockPlayerInput(i);
					}
				}
				iOwner.mCloneController.StartClip(iOwner.mClips[0], false);
				iOwner.mUpdate = false;
				iOwner.Movement = Vector3.Zero;
				Vector3 translation = iOwner.mOrientation.Translation;
				Vector3 position = iOwner.mTarget.Position;
				Vector3 vector;
				Vector3.Subtract(ref position, ref translation, out vector);
				vector.Y = 0f;
				float z = vector.Length();
				iOwner.mMultiDeathOrientation = iOwner.mOrientation;
				iOwner.mMultiDeathOffset = new Vector3(0f, 0f, z);
				this.mSpawnTimer = 0f;
				if (iOwner.mHitPoints / iOwner.MaxHitPoints > 0.6666667f)
				{
					iOwner.mCurrentMaxDeaths = 8;
				}
				else if (iOwner.mHitPoints / iOwner.MaxHitPoints > 0.33333334f)
				{
					iOwner.mCurrentMaxDeaths = 12;
				}
				else
				{
					iOwner.mCurrentMaxDeaths = 16;
				}
				for (int j = 0; j < iOwner.mCurrentMaxDeaths; j++)
				{
					iOwner.mIsVisibleMultiDeath[j] = true;
				}
				AudioManager.Instance.PlayCue(Banks.Misc, Death.SOUND_HASH);
				Flash.Instance.Execute(iOwner.mPlayState.Scene, 0.2f);
				iOwner.mPlayState.Camera.Time = 0f;
				iOwner.mPlayState.Camera.Magnification = 1.2f;
				iOwner.mPlayState.Camera.AttachPlayers(iOwner.mTarget);
				this.mSpawnTimer = 1f;
				iOwner.mAlphaFadeSteps = 0f;
			}

			// Token: 0x060021DE RID: 8670 RVA: 0x000F25F4 File Offset: 0x000F07F4
			public void OnUpdate(float iDeltaTime, Death iOwner)
			{
				bool isEthereal = iOwner.mBody.IsEthereal;
				if (iOwner.mTarget == null | iOwner.mTarget.Dead)
				{
					iOwner.ChangeState(Death.States.Prep);
					return;
				}
				if (iOwner.mCloneController.HasFinished && !iOwner.mCloneController.CrossFadeEnabled)
				{
					iOwner.ChangeState(Death.States.MultiDeath);
				}
				iOwner.mTarget.CharacterBody.SpeedMultiplier *= 1f / Death.TIME_SLOW_DOWN;
				Matrix identity = Matrix.Identity;
				MagickaMath.UniformMatrixScale(ref identity, 1.2f);
				iOwner.mCloneController.Update(iDeltaTime, ref identity, true);
				Vector3 mMultiDeathOffset = iOwner.mMultiDeathOffset;
				Matrix mOrientation = iOwner.mOrientation;
				Vector3.TransformNormal(ref mMultiDeathOffset, ref mOrientation, out mMultiDeathOffset);
				float num = Math.Min(this.mSpawnTimer / 1f, 1f);
				Matrix matrix;
				Matrix.CreateRotationY(1f / (float)iOwner.mCurrentMaxDeaths * 6.2831855f * num, out matrix);
				for (int i = 0; i < iOwner.mCurrentMaxDeaths; i++)
				{
					Matrix mHandBindPose = iOwner.mHandBindPose;
					mOrientation.Translation = default(Vector3);
					Matrix.Multiply(ref matrix, ref mOrientation, out mOrientation);
					Vector3.TransformNormal(ref mMultiDeathOffset, ref matrix, out mMultiDeathOffset);
					mOrientation.Translation = mMultiDeathOffset + iOwner.mTarget.Position;
					for (int j = 0; j < iOwner.mCloneController.Skeleton.Count; j++)
					{
						Matrix.Multiply(ref iOwner.mCloneController.SkinnedBoneTransforms[j], ref mOrientation, out iOwner.mRenderData[(int)iOwner.mCurrentChannel].mBones[i][j]);
					}
					Matrix.Multiply(ref mHandBindPose, ref iOwner.mRenderData[(int)iOwner.mCurrentChannel].mBones[i][iOwner.mHandIndex], out mHandBindPose);
					iOwner.mScytheRenderData[(int)iOwner.mCurrentChannel].WorldOrientation[i] = mHandBindPose;
				}
			}

			// Token: 0x060021DF RID: 8671 RVA: 0x000F27DC File Offset: 0x000F09DC
			public void OnExit(Death iOwner)
			{
			}

			// Token: 0x040024B5 RID: 9397
			private float mSpawnTimer;
		}

		// Token: 0x02000449 RID: 1097
		public class MultiDeathState : IBossState<Death>
		{
			// Token: 0x060021E1 RID: 8673 RVA: 0x000F27E8 File Offset: 0x000F09E8
			public unsafe void OnEnter(Death iOwner)
			{
				iOwner.mCloneController.CrossFade(iOwner.mClips[6], 0.2f, true);
				iOwner.Movement = Vector3.Zero;
				if (NetworkManager.Instance.State != NetworkState.Client)
				{
					iOwner.mIsRealDeath = Death.sRandom.Next(iOwner.mCurrentMaxDeaths);
					if (NetworkManager.Instance.State == NetworkState.Server)
					{
						Death.MultiDeathMessage multiDeathMessage = default(Death.MultiDeathMessage);
						multiDeathMessage.RealDeath = (byte)iOwner.mIsRealDeath;
						BossFight.Instance.SendMessage<Death.MultiDeathMessage>(iOwner, 5, (void*)(&multiDeathMessage), true);
					}
				}
				this.mNrOfAttacks = 0;
				this.mAttacker = -1;
				this.mAttackTime = 0f;
				this.mIsAttacking = false;
				this.mIsWithinReach = false;
				iOwner.mUpdate = false;
				this.mScytheRaised = false;
			}

			// Token: 0x060021E2 RID: 8674 RVA: 0x000F28A4 File Offset: 0x000F0AA4
			public void OnUpdate(float iDeltaTime, Death iOwner)
			{
				if (this.mNrOfAttacks >= iOwner.mCurrentMaxDeaths | iOwner.mTarget == null | iOwner.mTarget.Dead)
				{
					iOwner.ChangeState(Death.States.Prep);
					return;
				}
				if (iOwner.mSpeed <= 0.1f)
				{
					iOwner.mSpeed = 5f * (2f - iOwner.mHitPoints / 10000f);
				}
				this.mAttackTime += iDeltaTime;
				iOwner.mTarget.CharacterBody.SpeedMultiplier *= 1f / Death.TIME_SLOW_DOWN;
				if (!this.mIsAttacking && !this.mIsWithinReach)
				{
					this.mIsAttacking = true;
					while (this.mAttacker == -1 || !iOwner.mIsVisibleMultiDeath[this.mAttacker])
					{
						this.mAttacker = Death.sRandom.Next(iOwner.mCurrentMaxDeaths);
					}
					this.mIsFakeAttack = (this.mAttacker != iOwner.mIsRealDeath);
					iOwner.mController.StartClip(iOwner.mCloneController.AnimationClip, true);
					iOwner.mController.Time = iOwner.mCloneController.Time;
					iOwner.mCloneController.LocalBonePoses.CopyTo(iOwner.mController.LocalBonePoses, 0);
					iOwner.mController.CrossFade(iOwner.mClips[6], 0.2f, true);
					Vector3 mMultiDeathOffset = iOwner.mMultiDeathOffset;
					Matrix mMultiDeathOrientation = iOwner.mMultiDeathOrientation;
					Vector3.TransformNormal(ref mMultiDeathOffset, ref mMultiDeathOrientation, out mMultiDeathOffset);
					Matrix matrix;
					Matrix.CreateRotationY((float)(this.mAttacker + 1) * (1f / (float)iOwner.mCurrentMaxDeaths) * 6.2831855f, out matrix);
					mMultiDeathOrientation.Translation = default(Vector3);
					Matrix.Multiply(ref matrix, ref mMultiDeathOrientation, out mMultiDeathOrientation);
					Vector3.TransformNormal(ref mMultiDeathOffset, ref matrix, out mMultiDeathOffset);
					mMultiDeathOrientation.Translation = mMultiDeathOffset + iOwner.mTarget.Position;
					iOwner.mOrientation = mMultiDeathOrientation;
					iOwner.mControllerSkeletonIndex = this.mAttacker;
					iOwner.mUpdate = true;
					for (int i = 0; i < 3; i++)
					{
						Death.AfterImageRenderData afterImageRenderData = iOwner.mAfterImageRenderData[i];
						for (int j = 0; j < afterImageRenderData.mSkeleton.Length; j++)
						{
							iOwner.mRenderData[(int)iOwner.mCurrentChannel].mBones[this.mAttacker].CopyTo(afterImageRenderData.mSkeleton[j], 0);
						}
					}
				}
				Matrix identity = Matrix.Identity;
				MagickaMath.UniformMatrixScale(ref identity, 1.2f);
				iOwner.mCloneController.Update(iDeltaTime, ref identity, true);
				Vector3 mMultiDeathOffset2 = iOwner.mMultiDeathOffset;
				Matrix mMultiDeathOrientation2 = iOwner.mMultiDeathOrientation;
				Vector3.TransformNormal(ref mMultiDeathOffset2, ref mMultiDeathOrientation2, out mMultiDeathOffset2);
				Matrix matrix2;
				Matrix.CreateRotationY(1f / (float)iOwner.mCurrentMaxDeaths * 6.2831855f, out matrix2);
				for (int k = 0; k < iOwner.mCurrentMaxDeaths; k++)
				{
					Matrix mHandBindPose = iOwner.mHandBindPose;
					mMultiDeathOrientation2.Translation = default(Vector3);
					Matrix.Multiply(ref matrix2, ref mMultiDeathOrientation2, out mMultiDeathOrientation2);
					Vector3.TransformNormal(ref mMultiDeathOffset2, ref matrix2, out mMultiDeathOffset2);
					mMultiDeathOrientation2.Translation = mMultiDeathOffset2 + iOwner.mTarget.Position;
					if (this.mAttacker != k)
					{
						for (int l = 0; l < iOwner.mCloneController.Skeleton.Count; l++)
						{
							Matrix.Multiply(ref iOwner.mCloneController.SkinnedBoneTransforms[l], ref mMultiDeathOrientation2, out iOwner.mRenderData[(int)iOwner.mCurrentChannel].mBones[k][l]);
						}
						Matrix.Multiply(ref mHandBindPose, ref iOwner.mRenderData[(int)iOwner.mCurrentChannel].mBones[k][iOwner.mHandIndex], out mHandBindPose);
						iOwner.mScytheRenderData[(int)iOwner.mCurrentChannel].WorldOrientation[k] = mHandBindPose;
					}
					else if (this.mIsWithinReach)
					{
						if (!this.mIsFakeAttack)
						{
							iOwner.mBody.IsEthereal = false;
							for (int m = 0; m < 4; m++)
							{
								if (iOwner.mPlayers[m].Playing)
								{
									iOwner.mPlayers[m].Avatar.IsEthereal = false;
								}
							}
							for (int n = 0; n < iOwner.mCurrentMaxDeaths; n++)
							{
								if (n != this.mAttacker)
								{
									iOwner.mIsVisibleMultiDeath[n] = false;
								}
							}
						}
						Vector3 position = iOwner.mTarget.Position;
						Vector3 translation = iOwner.mOrientation.Translation;
						Vector3 movement;
						Vector3.Subtract(ref position, ref translation, out movement);
						movement.Y = 0f;
						float num = movement.Length();
						movement.Normalize();
						if (iOwner.mTarget.Radius + iOwner.mBody.Radius < num)
						{
							float num2 = num / 2.5f * 5f * (2f - iOwner.mHitPoints / 10000f);
							iOwner.mSpeed = num2;
							iOwner.Movement = movement;
							if (num2 < 8f)
							{
								num2 = 8f;
							}
							iOwner.Turn(ref movement, num2, iDeltaTime);
						}
						else
						{
							iOwner.mSpeed = 0.1f;
						}
						if (iOwner.mController.HasFinished && this.mIsFakeAttack)
						{
							translation = iOwner.mOrientation.Translation;
							Vector3 right = Vector3.Right;
							VisualEffectReference visualEffectReference;
							EffectManager.Instance.StartEffect(Death.ETHEREAL_SPAWN_EFFECT, ref translation, ref right, out visualEffectReference);
							iOwner.mIsVisibleMultiDeath[this.mAttacker] = false;
							this.mIsAttacking = false;
							this.mIsWithinReach = false;
							this.mAttackTime = 0f;
							iOwner.mUpdate = false;
							iOwner.Movement = Vector3.Zero;
							iOwner.mSpeed = 5f * (2f - iOwner.mHitPoints / 10000f);
							this.mNrOfAttacks++;
						}
						if (iOwner.mController.HasFinished && !iOwner.mController.CrossFadeEnabled && !this.mIsFakeAttack)
						{
							if (!this.mScytheRaised)
							{
								iOwner.mController.CrossFade(iOwner.mClips[1], 0.2f, false);
								this.mScytheRaised = true;
							}
							else
							{
								iOwner.ChangeState(Death.States.Prep);
							}
						}
					}
					if (this.mIsAttacking)
					{
						Vector3 position2 = iOwner.mTarget.Position;
						Vector3 translation2 = iOwner.mOrientation.Translation;
						Vector3 movement2;
						Vector3.Subtract(ref position2, ref translation2, out movement2);
						movement2.Y = 0f;
						float num3 = movement2.Length();
						movement2.Normalize();
						iOwner.Movement = movement2;
						iOwner.Turn(ref movement2, 8f, iDeltaTime);
						if (num3 < 2.5f)
						{
							this.mIsAttacking = false;
							this.mIsWithinReach = true;
							iOwner.mController.CrossFade(iOwner.mClips[2], 0.2f, false);
							iOwner.Movement = Vector3.Zero;
						}
					}
				}
			}

			// Token: 0x060021E3 RID: 8675 RVA: 0x000F2F28 File Offset: 0x000F1128
			public void OnExit(Death iOwner)
			{
				iOwner.Movement = Vector3.Zero;
				iOwner.mDraw = true;
				iOwner.mIsVisibleMultiDeath[0] = true;
				iOwner.mPlayState.TimeModifier = 1f;
				iOwner.mTarget.CharacterBody.SpeedMultiplier = 1f;
				ControlManager.Instance.UnlockPlayerInput();
				iOwner.mPlayState.Camera.Release(0.5f, false);
				for (int i = 0; i < 4; i++)
				{
					if (iOwner.mPlayers[i].Playing)
					{
						iOwner.mPlayers[i].Avatar.IsEthereal = false;
					}
				}
				for (int j = 1; j < iOwner.mIsVisibleMultiDeath.Length; j++)
				{
					bool flag = iOwner.mIsVisibleMultiDeath[j];
					iOwner.mIsVisibleMultiDeath[j] = false;
				}
				iOwner.mController.Speed = 1f;
				iOwner.mUpdate = true;
				iOwner.mPreviousState = iOwner.mCurrentState;
				iOwner.mControllerSkeletonIndex = 0;
				for (int k = 0; k < 3; k++)
				{
					Death.AfterImageRenderData afterImageRenderData = iOwner.mAfterImageRenderData[k];
					for (int l = 0; l < afterImageRenderData.mSkeleton.Length; l++)
					{
						iOwner.mEmptyMatrix.CopyTo(afterImageRenderData.mSkeleton[l], 0);
					}
				}
				iOwner.mIsRealDeath = 0;
			}

			// Token: 0x040024B6 RID: 9398
			private float mAttackTime;

			// Token: 0x040024B7 RID: 9399
			private int mAttacker;

			// Token: 0x040024B8 RID: 9400
			private bool mIsAttacking;

			// Token: 0x040024B9 RID: 9401
			private bool mIsWithinReach;

			// Token: 0x040024BA RID: 9402
			private bool mScytheRaised;

			// Token: 0x040024BB RID: 9403
			private int mNrOfAttacks;

			// Token: 0x040024BC RID: 9404
			public bool mIsFakeAttack;
		}

		// Token: 0x0200044A RID: 1098
		public class WarpTimeState : IBossState<Death>
		{
			// Token: 0x060021E5 RID: 8677 RVA: 0x000F3062 File Offset: 0x000F1262
			public void OnEnter(Death iOwner)
			{
				iOwner.mDraw = true;
				iOwner.mController.CrossFade(iOwner.mClips[11], 0.2f, false);
				iOwner.Movement = Vector3.Zero;
			}

			// Token: 0x060021E6 RID: 8678 RVA: 0x000F3090 File Offset: 0x000F1290
			public void OnUpdate(float iDeltaTime, Death iOwner)
			{
				if (NetworkManager.Instance.State == NetworkState.Client)
				{
					return;
				}
				if (iOwner.mController.HasFinished && !iOwner.mController.CrossFadeEnabled)
				{
					Magick magick = default(Magick);
					magick.MagickType = MagickType.TimeWarp;
					magick.Effect.Execute(iOwner.mOrientation.Translation, iOwner.mPlayState);
					iOwner.ChangeState(Death.States.Prep);
				}
			}

			// Token: 0x060021E7 RID: 8679 RVA: 0x000F30FB File Offset: 0x000F12FB
			public void OnExit(Death iOwner)
			{
				iOwner.mController.Speed = 1f;
				iOwner.mPreviousState = iOwner.mCurrentState;
			}
		}

		// Token: 0x0200044B RID: 1099
		public class AppearState : IBossState<Death>
		{
			// Token: 0x060021E9 RID: 8681 RVA: 0x000F3124 File Offset: 0x000F1324
			public void OnEnter(Death iOwner)
			{
				iOwner.mBody.IsEthereal = true;
				iOwner.mDraw = true;
				iOwner.mController.StartClip(iOwner.mClips[0], false);
				iOwner.Movement = Vector3.Zero;
				iOwner.mAppearDuration = iOwner.mClips[0].Duration;
				iOwner.mAlphaFadeSteps = 0f;
			}

			// Token: 0x060021EA RID: 8682 RVA: 0x000F3181 File Offset: 0x000F1381
			public void OnUpdate(float iDeltaTime, Death iOwner)
			{
				if (iOwner.mController.HasFinished && !iOwner.mController.CrossFadeEnabled)
				{
					iOwner.ChangeState(iOwner.mNextState);
				}
			}

			// Token: 0x060021EB RID: 8683 RVA: 0x000F31A9 File Offset: 0x000F13A9
			public void OnExit(Death iOwner)
			{
				iOwner.mBody.IsEthereal = false;
			}
		}

		// Token: 0x0200044C RID: 1100
		public class RaiseDeadState : IBossState<Death>
		{
			// Token: 0x060021ED RID: 8685 RVA: 0x000F31C0 File Offset: 0x000F13C0
			public void OnEnter(Death iOwner)
			{
				iOwner.mDraw = true;
				iOwner.mController.Speed = MathHelper.Lerp(2f, 0.65f, iOwner.mHitPoints / 10000f - 1f);
				iOwner.mController.CrossFade(iOwner.mClips[10], 0.2f, false);
				iOwner.Movement = Vector3.Zero;
				this.time = 0.2f;
			}

			// Token: 0x060021EE RID: 8686 RVA: 0x000F3230 File Offset: 0x000F1430
			public void OnUpdate(float iDeltaTime, Death iOwner)
			{
				int num = Death.MAX_SKELETONS - iOwner.mAnyArea.GetCount(Death.SKELETON1) - iOwner.mAnyArea.GetCount(Death.SKELETON2) - iOwner.mAnyArea.GetCount(Death.SKELETON3) - iOwner.mAnyArea.GetCount(Death.SKELETON4);
				this.time -= iDeltaTime;
				if (this.time <= 0f)
				{
					if (num > 0)
					{
						iOwner.RaiseDead(1, iOwner);
						num++;
					}
					this.time = 0.2f;
				}
				if (iOwner.mController.HasFinished && !iOwner.mController.CrossFadeEnabled)
				{
					iOwner.ChangeState(Death.States.Prep);
				}
			}

			// Token: 0x060021EF RID: 8687 RVA: 0x000F32DE File Offset: 0x000F14DE
			public void OnExit(Death iOwner)
			{
				iOwner.mController.Speed = 1f;
				iOwner.mPreviousState = iOwner.mCurrentState;
			}

			// Token: 0x040024BD RID: 9405
			private float time;
		}

		// Token: 0x0200044D RID: 1101
		private enum MessageType : ushort
		{
			// Token: 0x040024BF RID: 9407
			Update,
			// Token: 0x040024C0 RID: 9408
			ChangeState,
			// Token: 0x040024C1 RID: 9409
			ChangeTarget,
			// Token: 0x040024C2 RID: 9410
			Spawn,
			// Token: 0x040024C3 RID: 9411
			ClearSkeletons,
			// Token: 0x040024C4 RID: 9412
			MultiDeath,
			// Token: 0x040024C5 RID: 9413
			Ethereal
		}

		// Token: 0x0200044E RID: 1102
		internal struct UpdateMessage
		{
			// Token: 0x040024C6 RID: 9414
			public const ushort TYPE = 0;

			// Token: 0x040024C7 RID: 9415
			public Matrix Orientation;

			// Token: 0x040024C8 RID: 9416
			public byte Animation;

			// Token: 0x040024C9 RID: 9417
			public float AnimationTime;

			// Token: 0x040024CA RID: 9418
			public float Hitpoints;
		}

		// Token: 0x0200044F RID: 1103
		internal struct ClearMessage
		{
			// Token: 0x040024CB RID: 9419
			public const ushort TYPE = 4;
		}

		// Token: 0x02000450 RID: 1104
		internal struct SpawnMessage
		{
			// Token: 0x040024CC RID: 9420
			public const ushort TYPE = 3;

			// Token: 0x040024CD RID: 9421
			public ushort Handle;

			// Token: 0x040024CE RID: 9422
			public int TypeID;

			// Token: 0x040024CF RID: 9423
			public Vector3 Position;

			// Token: 0x040024D0 RID: 9424
			public Vector3 Direction;
		}

		// Token: 0x02000451 RID: 1105
		internal struct ChangeStateMessage
		{
			// Token: 0x040024D1 RID: 9425
			public const ushort TYPE = 1;

			// Token: 0x040024D2 RID: 9426
			public Death.States NewState;
		}

		// Token: 0x02000452 RID: 1106
		internal struct ChangeTargetMessage
		{
			// Token: 0x040024D3 RID: 9427
			public const ushort TYPE = 2;

			// Token: 0x040024D4 RID: 9428
			public ushort Target;
		}

		// Token: 0x02000453 RID: 1107
		internal struct MultiDeathMessage
		{
			// Token: 0x040024D5 RID: 9429
			public const ushort TYPE = 5;

			// Token: 0x040024D6 RID: 9430
			public byte RealDeath;
		}

		// Token: 0x02000454 RID: 1108
		public enum States
		{
			// Token: 0x040024D8 RID: 9432
			Idle,
			// Token: 0x040024D9 RID: 9433
			SitIntro,
			// Token: 0x040024DA RID: 9434
			RiseIntro,
			// Token: 0x040024DB RID: 9435
			Prep,
			// Token: 0x040024DC RID: 9436
			Attack,
			// Token: 0x040024DD RID: 9437
			Move,
			// Token: 0x040024DE RID: 9438
			Hit,
			// Token: 0x040024DF RID: 9439
			Defeat,
			// Token: 0x040024E0 RID: 9440
			DefeatPreSit,
			// Token: 0x040024E1 RID: 9441
			WarpTime,
			// Token: 0x040024E2 RID: 9442
			RaiseDead,
			// Token: 0x040024E3 RID: 9443
			MultiDeath,
			// Token: 0x040024E4 RID: 9444
			PreMultiDeath,
			// Token: 0x040024E5 RID: 9445
			Appear
		}

		// Token: 0x02000455 RID: 1109
		private enum Animations
		{
			// Token: 0x040024E7 RID: 9447
			appear,
			// Token: 0x040024E8 RID: 9448
			attack_scythe_fall,
			// Token: 0x040024E9 RID: 9449
			attack_scythe_rise,
			// Token: 0x040024EA RID: 9450
			defeated,
			// Token: 0x040024EB RID: 9451
			hit,
			// Token: 0x040024EC RID: 9452
			idle,
			// Token: 0x040024ED RID: 9453
			move_glide,
			// Token: 0x040024EE RID: 9454
			sit,
			// Token: 0x040024EF RID: 9455
			sit_talk,
			// Token: 0x040024F0 RID: 9456
			summon_scythe,
			// Token: 0x040024F1 RID: 9457
			summon_skeleton,
			// Token: 0x040024F2 RID: 9458
			timewarp,
			// Token: 0x040024F3 RID: 9459
			NrOfAnimations
		}
	}
}
