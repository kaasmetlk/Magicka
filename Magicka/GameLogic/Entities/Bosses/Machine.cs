using System;
using System.Collections.Generic;
using JigLibX.Geometry;
using Magicka.AI;
using Magicka.AI.AgentStates;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.UI;
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
	// Token: 0x0200051A RID: 1306
	public class Machine : BossStatusEffected, IBoss
	{
		// Token: 0x06002758 RID: 10072 RVA: 0x0011EB18 File Offset: 0x0011CD18
		public unsafe Machine(PlayState iPlayState)
		{
			this.mAudioEmitter = new AudioEmitter();
			this.mGibAudioEmitter = new AudioEmitter();
			this.mRandom = new Random();
			this.mShrapnelHitList = new HitList(16);
			this.mPlayState = iPlayState;
			lock (Game.Instance.GraphicsDevice)
			{
				this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/boss_machine_warlock");
				this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/warlock");
				this.mMachineModel = this.mPlayState.Content.Load<SkinnedModel>("Models/Bosses/theMachine/themachine");
				this.mKingModel = this.mPlayState.Content.Load<SkinnedModel>("Models/Characters/Human/King_animation");
				this.mIceCubeMap = this.mPlayState.Content.Load<TextureCube>("EffectTextures/iceCube");
				this.mIceCubeNormalMap = this.mPlayState.Content.Load<TextureCube>("EffectTextures/iceCube_NRM");
			}
			Matrix matrix;
			Matrix.CreateRotationY(3.1415927f, out matrix);
			for (int i = 0; i < this.mMachineModel.SkeletonBones.Count; i++)
			{
				SkinnedModelBone skinnedModelBone = this.mMachineModel.SkeletonBones[i];
				if (skinnedModelBone.Name.Equals("Platform", StringComparison.OrdinalIgnoreCase))
				{
					this.mDrillIndex = (int)skinnedModelBone.Index;
				}
				else if (skinnedModelBone.Name.Equals("BicycleRoot", StringComparison.OrdinalIgnoreCase))
				{
					Quaternion orientation = skinnedModelBone.BindPose.Orientation;
					Quaternion quaternion;
					Quaternion.CreateFromRotationMatrix(ref matrix, out quaternion);
					Quaternion.Multiply(ref orientation, ref quaternion, out orientation);
					this.mBicyclePosition = skinnedModelBone.BindPose.Translation;
					Vector3.Transform(ref this.mBicyclePosition, ref orientation, out this.mBicyclePosition);
				}
				else if (skinnedModelBone.Name.Equals("Drilly", StringComparison.OrdinalIgnoreCase))
				{
					this.mDrillyIndex = (int)skinnedModelBone.Index;
					Matrix inverseBindPoseTransform = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref inverseBindPoseTransform, ref matrix, out inverseBindPoseTransform);
					Matrix.Invert(ref inverseBindPoseTransform, out this.mDrillyBindPose);
				}
			}
			this.mMachineZone = new BossDamageZone(this.mPlayState, this, 0, 4f, new Box(new Vector3(-4.25f, 0f, -3.75f), Matrix.Identity, new Vector3(8.5f, 3f, 8f)));
			this.mOrcZone = new BossDamageZone(this.mPlayState, this, 1, 3f, new Box(new Vector3(-1.5f, -1f, -0.75f), Matrix.Identity, new Vector3(2.5f, 3.75f, 2.5f)));
			this.mOrcController = new AnimationController();
			this.mOrcController.Skeleton = this.mMachineModel.SkeletonBones;
			this.mOrcClips = new AnimationClip[4];
			this.mOrcClips[0] = this.mMachineModel.AnimationClips["pedal"];
			this.mOrcClips[1] = this.mMachineModel.AnimationClips["breakfree"];
			this.mOrcClips[2] = this.mMachineModel.AnimationClips["nod"];
			this.mOrcClips[3] = this.mMachineModel.AnimationClips["idle"];
			this.mMachineController = new AnimationController();
			this.mMachineController.Skeleton = this.mMachineModel.SkeletonBones;
			this.mMachineClips = new AnimationClip[3];
			this.mMachineClips[1] = this.mMachineModel.AnimationClips["idle"];
			this.mMachineClips[2] = this.mMachineModel.AnimationClips["breakfree"];
			this.mMachineClips[0] = this.mMachineModel.AnimationClips["pedal"];
			this.mKingController = new AnimationController();
			this.mKingController.Skeleton = this.mKingModel.SkeletonBones;
			this.mKingClips = new AnimationClip[5];
			this.mKingClips[0] = this.mKingModel.AnimationClips["sit_bound"];
			this.mKingClips[3] = this.mKingModel.AnimationClips["sit_bound"];
			this.mKingClips[4] = this.mKingModel.AnimationClips["king_vader"];
			this.mKingClips[1] = this.mKingModel.AnimationClips["talk_oldandweak"];
			this.mKingClips[2] = this.mKingModel.AnimationClips["talk_strikeatdawn"];
			ModelMesh modelMesh = this.mMachineModel.Model.Meshes[1];
			ModelMeshPart modelMeshPart = modelMesh.MeshParts[0];
			ModelMesh modelMesh2 = this.mMachineModel.Model.Meshes[0];
			ModelMeshPart modelMeshPart2 = modelMesh2.MeshParts[1];
			ModelMeshPart modelMeshPart3 = modelMesh2.MeshParts[0];
			ModelMesh modelMesh3 = this.mKingModel.Model.Meshes[0];
			ModelMeshPart modelMeshPart4 = modelMesh3.MeshParts[0];
			SkinnedModelBasicEffect iEffect = modelMeshPart.Effect as SkinnedModelBasicEffect;
			SkinnedModelDeferredAdvancedMaterial skinnedModelDeferredAdvancedMaterial;
			Helper.SkinnedModelDeferredMaterialFromBasicEffect(iEffect, out skinnedModelDeferredAdvancedMaterial);
			iEffect = (modelMeshPart4.Effect as SkinnedModelBasicEffect);
			SkinnedModelDeferredAdvancedMaterial skinnedModelDeferredAdvancedMaterial2;
			Helper.SkinnedModelDeferredMaterialFromBasicEffect(iEffect, out skinnedModelDeferredAdvancedMaterial2);
			iEffect = (modelMeshPart2.Effect as SkinnedModelBasicEffect);
			Helper.SkinnedModelDeferredMaterialFromBasicEffect(iEffect, out this.mMaterial);
			iEffect = (modelMeshPart3.Effect as SkinnedModelBasicEffect);
			SkinnedModelDeferredAdvancedMaterial mMaterial;
			Helper.SkinnedModelDeferredMaterialFromBasicEffect(iEffect, out mMaterial);
			this.mKingRenderData = new Machine.RenderData[3];
			this.mMachineRenderData = new Machine.RenderData[3];
			this.mOrcRenderData = new Machine.OrcRenderData[3];
			for (int j = 0; j < 3; j++)
			{
				this.mKingRenderData[j] = new Machine.RenderData();
				this.mKingRenderData[j].SetMesh(modelMesh3.VertexBuffer, modelMesh3.IndexBuffer, modelMeshPart4, 0, 3, 4);
				this.mKingRenderData[j].mMaterial = skinnedModelDeferredAdvancedMaterial2;
				this.mMachineRenderData[j] = new Machine.RenderData();
				this.mMachineRenderData[j].SetMesh(modelMesh.VertexBuffer, modelMesh.IndexBuffer, modelMeshPart, 0, 3, 4);
				this.mMachineRenderData[j].mMaterial = skinnedModelDeferredAdvancedMaterial;
				this.mOrcRenderData[j] = new Machine.OrcRenderData();
				this.mOrcRenderData[j].SetMesh(modelMesh2.VertexBuffer, modelMesh2.IndexBuffer, modelMeshPart2, modelMeshPart3, 0, 3, 4);
				this.mOrcRenderData[j].mMaterial = this.mMaterial;
				this.mOrcRenderData[j].mMaterial2 = mMaterial;
			}
			this.mNPCs = new List<NonPlayerCharacter>(16);
			this.mOriginEmitter = new AudioEmitter();
			this.mDestinationEmitter = new AudioEmitter();
			for (int k = 0; k < this.mResistances.Length; k++)
			{
				this.mResistances[k].ResistanceAgainst = Spell.ElementFromIndex(k);
				this.mResistances[k].Modifier = 0f;
				this.mResistances[k].Multiplier = 1f;
			}
			int num = Spell.ElementIndex(Elements.Earth);
			this.mResistances[num].Modifier = -200f;
			this.mResistances[num].Multiplier = 0.75f;
			num = Spell.ElementIndex(Elements.Arcane);
			this.mResistances[num].Modifier = 0f;
			this.mResistances[num].Multiplier = 0.5f;
			num = Spell.ElementIndex(Elements.Lightning);
			this.mResistances[num].Modifier = 0f;
			this.mResistances[num].Multiplier = 0.5f;
			num = Spell.ElementIndex(Elements.Fire);
			this.mResistances[num].Modifier = 0f;
			this.mResistances[num].Multiplier = 0.5f;
			Machine.DIALOG_MACHINE = new List<int>(3);
			Machine.DIALOG_MACHINE.Add("warlocktaunt1".GetHashCodeCustom());
			Machine.DIALOG_MACHINE.Add("warlocktaunt2".GetHashCodeCustom());
			Machine.DIALOG_MACHINE.Add("warlocktaunt3".GetHashCodeCustom());
			Machine.IdleState instance = Machine.IdleState.Instance;
			Machine.DrillState instance2 = Machine.DrillState.Instance;
			Machine.KillKingState instance3 = Machine.KillKingState.Instance;
			Machine.BrokenState instance4 = Machine.BrokenState.Instance;
			Machine.IntroStage instance5 = Machine.IntroStage.Instance;
			Machine.PreGunterStage instance6 = Machine.PreGunterStage.Instance;
			Machine.WarlockStage instance7 = Machine.WarlockStage.Instance;
			Machine.FinalStage instance8 = Machine.FinalStage.Instance;
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				this.mWarlock = NonPlayerCharacter.GetInstance(this.mPlayState);
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					Machine.InitializeMessage initializeMessage;
					initializeMessage.Handle = this.mWarlock.Handle;
					BossFight.Instance.SendInitializeMessage<Machine.InitializeMessage>(this, 0, (void*)(&initializeMessage));
				}
			}
			this.mNetworkInitialized = (NetworkManager.Instance.State != NetworkState.Client);
			Texture2D texture = this.mPlayState.Content.Load<Texture2D>("UI/Boss/MachineRuler");
			VertexPositionTexture[] array = new VertexPositionTexture[8];
			array[0].TextureCoordinate = new Vector2(0f, 1f);
			array[1].TextureCoordinate = new Vector2(0f, 0f);
			array[2].TextureCoordinate = new Vector2(0.33333334f, 1f);
			array[3].TextureCoordinate = new Vector2(0.33333334f, 0f);
			array[4].TextureCoordinate = new Vector2(0.6666667f, 1f);
			array[5].TextureCoordinate = new Vector2(0.6666667f, 0f);
			array[6].TextureCoordinate = new Vector2(1f, 1f);
			array[7].TextureCoordinate = new Vector2(1f, 0f);
			VertexDeclaration vertexDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
			BasicEffect basicEffect = new BasicEffect(Game.Instance.GraphicsDevice, null);
			basicEffect.Alpha = 1f;
			basicEffect.DiffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
			basicEffect.VertexColorEnabled = false;
			basicEffect.Texture = texture;
			GUIBasicEffect guibasicEffect = new GUIBasicEffect(Game.Instance.GraphicsDevice, null);
			guibasicEffect.TextureEnabled = false;
			guibasicEffect.Color = new Vector4(1f, 1f, 1f, 1f);
			guibasicEffect.ScaleToHDR = true;
			guibasicEffect.VertexColorEnabled = true;
			this.mRulerRenderData = new Machine.RulerRenderData[3];
			for (int l = 0; l < 3; l++)
			{
				this.mRulerRenderData[l] = new Machine.RulerRenderData();
				this.mRulerRenderData[l].Effect = basicEffect;
				this.mRulerRenderData[l].TextEffect = guibasicEffect;
				this.mRulerRenderData[l].Vertices = array;
				this.mRulerRenderData[l].Alpha = 0f;
				this.mRulerRenderData[l].VertexDeclaration = vertexDeclaration;
			}
		}

		// Token: 0x06002759 RID: 10073 RVA: 0x0011F5F8 File Offset: 0x0011D7F8
		public void Initialize(ref Matrix iOrientation, int iUniqueID)
		{
			this.Initialize(ref iOrientation);
		}

		// Token: 0x0600275A RID: 10074 RVA: 0x0011F604 File Offset: 0x0011D804
		public void Initialize(ref Matrix iOrientation)
		{
			this.mRenderKing = true;
			this.mDead = false;
			this.mMaxHitPoints = 5000f;
			this.mHitPoints = 5000f;
			this.mIdleTimer = 0f;
			this.mDrillTime = 0f;
			this.mPeddleSpeed = 1f;
			this.mPeddleTargetSpeed = 1f;
			this.mMachineOrientation = iOrientation;
			this.mOrcStageShield = null;
			Matrix matrix;
			this.mPlayState.Level.CurrentScene.GetLocator("spawn_warlock".GetHashCodeCustom(), out matrix);
			CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate("boss_machine_warlock".GetHashCodeCustom());
			Vector3 vector = matrix.Translation;
			Segment seg = default(Segment);
			seg.Origin = vector;
			seg.Origin.Y = seg.Origin.Y + 1f;
			seg.Delta.Y = seg.Delta.Y - 5f;
			float num;
			Vector3 vector2;
			Vector3 vector3;
			if (this.mPlayState.Level.CurrentScene.CollisionSkin.SegmentIntersect(out num, out vector2, out vector3, seg))
			{
				vector = vector2;
			}
			vector.Y += cachedTemplate.Radius + cachedTemplate.Length * 0.5f + 0.1f;
			this.mWarlock.Initialize(cachedTemplate, vector, "boss_machine_warlock".GetHashCodeCustom());
			Agent ai = this.mWarlock.AI;
			ai.SetOrder(Order.Idle, ReactTo.None, Order.Flee, 0, 0, 0, null);
			this.mWarlock.CharacterBody.DesiredDirection = matrix.Forward;
			this.mPlayState.EntityManager.AddEntity(this.mWarlock);
			this.mWarlock.CannotDieWithoutExplicitKill = true;
			this.mWarlock.SetImmortalTime(float.PositiveInfinity);
			this.mWarlock.AI.SetOrder(Order.Idle, ReactTo.None, Order.Idle, 0, 0, 0, null);
			this.mMachineZone.Initialize();
			this.mMachineZone.Body.Immovable = true;
			this.mMachineZone.Body.MoveTo(this.mMachineOrientation.Translation, Machine.INV_SCENE_ROTATION);
			this.mMachineZone.Body.CollisionSkin.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
			this.mPlayState.EntityManager.AddEntity(this.mMachineZone);
			this.mOrcZone.Initialize();
			this.mOrcZone.Body.Immovable = true;
			this.mOrcZone.Body.MoveTo(this.mMachineOrientation.Translation + this.mBicyclePosition, Machine.INV_SCENE_ROTATION);
			this.mOrcZone.Body.CollisionSkin.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
			this.mOrcZone.Body.CollisionSkin.NonCollidables.Add(this.mMachineZone.Body.CollisionSkin);
			this.mPlayState.EntityManager.AddEntity(this.mOrcZone);
			this.mAudioEmitter.Position = this.mOrcZone.Position;
			this.mAudioEmitter.Forward = Vector3.Right;
			this.mAudioEmitter.Up = Vector3.Up;
			this.mOrcController.Speed = 1f;
			this.mMachineController.Speed = 1f;
			this.mPeddleSpeed = 1f;
			this.mPeddleTargetSpeed = 1f;
			this.mCurrentDialog = 0;
			MoveEvent moveEvent = default(MoveEvent);
			Locator locator;
			this.mPlayState.Level.CurrentScene.GetLocator("spawn_king".GetHashCodeCustom(), out locator);
			this.mKingSpawnOrientation = locator.Transform;
			MagickaMath.UniformMatrixScale(ref this.mKingSpawnOrientation, 1.3f);
			this.mPlayState.Level.CurrentScene.GetLocator("spawn_warlock".GetHashCodeCustom(), out locator);
			this.mWarlockSpawnOrientation = locator.Transform;
			this.mPlayState.Level.CurrentScene.GetLocator("teleport_warlock".GetHashCodeCustom(), out locator);
			this.mWarlockEmperorOrientation = locator.Transform;
			this.mNPCs.Clear();
			this.mWarlockIntroEvent = new AIEvent[1];
			moveEvent = default(MoveEvent);
			moveEvent.Direction = this.mKingSpawnOrientation.Translation - this.mWarlockSpawnOrientation.Translation;
			moveEvent.Direction.Y = 0f;
			moveEvent.Direction.Normalize();
			moveEvent.Delay = 0f;
			moveEvent.FixedDirection = true;
			moveEvent.Waypoint = this.mWarlockSpawnOrientation.Translation;
			this.mWarlockIntroEvent[0].EventType = AIEventType.Move;
			this.mWarlockIntroEvent[0].MoveEvent = moveEvent;
			this.mWarlockFinalEvent = new AIEvent[1];
			moveEvent = default(MoveEvent);
			moveEvent.Waypoint = this.mWarlockSpawnOrientation.Translation;
			Vector3 direction = this.mKingSpawnOrientation.Translation - this.mWarlockSpawnOrientation.Translation;
			direction.Y = 0f;
			direction.Normalize();
			moveEvent.Direction = direction;
			moveEvent.FixedDirection = true;
			this.mWarlockFinalEvent[0].EventType = AIEventType.Move;
			this.mWarlockFinalEvent[0].MoveEvent = moveEvent;
			this.mWarlockEmperorEvent = new AIEvent[2];
			this.mWarlockEmperorEvent[0].AnimationEvent.Animation = Animations.spec_action0;
			this.mWarlockEmperorEvent[0].AnimationEvent.BlendTime = 0.01f;
			this.mWarlockEmperorEvent[0].AnimationEvent.Delay = 0.15f;
			this.mWarlockEmperorEvent[0].EventType = AIEventType.Animation;
			this.mWarlockEmperorEvent[1].AnimationEvent.Animation = Animations.spec_action0;
			this.mWarlockEmperorEvent[1].AnimationEvent.BlendTime = 0.01f;
			this.mWarlockEmperorEvent[1].AnimationEvent.Delay = 0.15f;
			this.mWarlockEmperorEvent[1].EventType = AIEventType.Animation;
			this.mAvatarMoveEvent = new AIEvent[4][];
			this.mAvatarMoveEvent[0] = new AIEvent[1];
			moveEvent.Speed = 1f;
			moveEvent.FixedDirection = true;
			moveEvent.Waypoint = this.mKingSpawnOrientation.Translation + Vector3.Transform(new Vector3(-3f, 1f, 8f), Machine.INV_SCENE_ROTATION);
			moveEvent.Direction = Vector3.Subtract(this.mWarlockSpawnOrientation.Translation, moveEvent.Waypoint);
			moveEvent.Direction.Normalize();
			this.mAvatarMoveEvent[0][0].EventType = AIEventType.Move;
			this.mAvatarMoveEvent[0][0].MoveEvent = moveEvent;
			this.mAvatarMoveEvent[1] = new AIEvent[1];
			moveEvent.Speed = 1f;
			moveEvent.FixedDirection = true;
			moveEvent.Waypoint = this.mKingSpawnOrientation.Translation + Vector3.Transform(new Vector3(-1f, 1f, 8f), Machine.INV_SCENE_ROTATION);
			moveEvent.Direction = Vector3.Subtract(this.mWarlockSpawnOrientation.Translation, moveEvent.Waypoint);
			moveEvent.Direction.Normalize();
			this.mAvatarMoveEvent[1][0].EventType = AIEventType.Move;
			this.mAvatarMoveEvent[1][0].MoveEvent = moveEvent;
			this.mAvatarMoveEvent[2] = new AIEvent[1];
			moveEvent.Speed = 1f;
			moveEvent.FixedDirection = true;
			moveEvent.Waypoint = this.mKingSpawnOrientation.Translation + Vector3.Transform(new Vector3(1f, 1f, 8f), Machine.INV_SCENE_ROTATION);
			moveEvent.Direction = Vector3.Subtract(this.mWarlockSpawnOrientation.Translation, moveEvent.Waypoint);
			moveEvent.Direction.Normalize();
			this.mAvatarMoveEvent[2][0].EventType = AIEventType.Move;
			this.mAvatarMoveEvent[2][0].MoveEvent = moveEvent;
			this.mAvatarMoveEvent[3] = new AIEvent[1];
			moveEvent.Speed = 1f;
			moveEvent.FixedDirection = true;
			moveEvent.Waypoint = this.mKingSpawnOrientation.Translation + Vector3.Transform(new Vector3(3f, 1f, 8f), Machine.INV_SCENE_ROTATION);
			moveEvent.Direction = Vector3.Subtract(this.mWarlockSpawnOrientation.Translation, moveEvent.Waypoint);
			moveEvent.Direction.Normalize();
			this.mAvatarMoveEvent[3][0].EventType = AIEventType.Move;
			this.mAvatarMoveEvent[3][0].MoveEvent = moveEvent;
			for (int i = 0; i < this.mStatusEffects.Length; i++)
			{
				this.mStatusEffects[i].Stop();
			}
			this.mCurrentStage = Machine.IntroStage.Instance;
			this.mCurrentStage.OnEnter(this);
			this.mMachineState = Machine.IdleState.Instance;
			this.mMachineState.OnEnter(this);
			for (int j = 0; j < 3; j++)
			{
				this.mKingRenderData[j].mBoundingSphere = new BoundingSphere(this.mKingSpawnOrientation.Translation, 10f);
				this.mMachineRenderData[j].mBoundingSphere = new BoundingSphere(this.mMachineOrientation.Translation, 12f);
				this.mOrcRenderData[j].mBoundingSphere = new BoundingSphere(this.mOrcZone.Position, 5f);
				this.mRulerRenderData[j].Alpha = 0f;
			}
			this.mRulerAlpha = 0f;
			this.mDamageFlashTimer = 0f;
		}

		// Token: 0x0600275B RID: 10075 RVA: 0x0011FFD2 File Offset: 0x0011E1D2
		public void DeInitialize()
		{
			this.mOrcStageShield.Kill();
			this.mOrcStageShield = null;
			this.mNPCs.Clear();
		}

		// Token: 0x0600275C RID: 10076 RVA: 0x0011FFF4 File Offset: 0x0011E1F4
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
			if (iFightStarted && this.mCurrentStage is Machine.IntroStage)
			{
				this.ChangeMachineState(Machine.States.Drill);
				this.ChangeStage(Machine.States.PreGunter);
			}
			this.mCurrentStage.OnUpdate(iDeltaTime, this);
			this.mMachineState.OnUpdate(iDeltaTime, this);
			this.mDamageFlashTimer = Math.Max(this.mDamageFlashTimer - iDeltaTime, 0f);
			this.mPeddleSpeed += (this.mPeddleTargetSpeed - this.mPeddleSpeed) * iDeltaTime * 4f;
			if (this.HasStatus(1, StatusEffects.Burning))
			{
				this.mPeddleTargetSpeed += (3f - this.mPeddleTargetSpeed) * iDeltaTime * 4f;
			}
			else if (this.HasStatus(1, StatusEffects.Frozen))
			{
				this.mPeddleSpeed = 0f;
				this.mPeddleTargetSpeed = 0f;
			}
			else if (this.HasStatus(1, StatusEffects.Cold))
			{
				this.mPeddleTargetSpeed += (1f - this.StatusMagnitude(1, StatusEffects.Cold) - this.mPeddleTargetSpeed) * iDeltaTime;
			}
			else
			{
				this.mPeddleTargetSpeed += (1f - this.mPeddleTargetSpeed) * iDeltaTime * 0.25f;
			}
			this.mShrapnelHitList.Update(iDeltaTime);
			this.UpdateStatusEffects(iDeltaTime);
			this.UpdateDamage(iDeltaTime);
			this.mMachineRenderData[(int)iDataChannel].Flash = this.mDamageFlashTimer * 10f;
			this.mMachineController.PreUpdate(iDeltaTime, ref this.mMachineOrientation);
			Pose[] localBonePoses = this.mMachineController.LocalBonePoses;
			Vector3 vector = new Vector3(0f, 0f, this.mDrillTime * 0.008333334f * Machine.DISTANCE);
			Quaternion orientation = localBonePoses[this.mDrillIndex].Orientation;
			Vector3.Transform(ref vector, ref orientation, out vector);
			Vector3.Add(ref vector, ref localBonePoses[this.mDrillIndex].Translation, out localBonePoses[this.mDrillIndex].Translation);
			this.mMachineController.UpdateAbsoluteBoneTransforms(ref this.mMachineOrientation);
			this.mMachineController.SkinnedBoneTransforms.CopyTo(this.mMachineRenderData[(int)iDataChannel].mSkeleton, 0);
			this.mPlayState.Scene.AddRenderableObject(iDataChannel, this.mMachineRenderData[(int)iDataChannel]);
			if (this.HasStatus(1, StatusEffects.Frozen))
			{
				this.mOrcRenderData[(int)iDataChannel].mMaterial.CubeMapRotation = Matrix.Identity;
				this.mOrcRenderData[(int)iDataChannel].mMaterial.Bloat = 0.1f;
				this.mOrcRenderData[(int)iDataChannel].mMaterial.EmissiveAmount = 2f;
				this.mOrcRenderData[(int)iDataChannel].mMaterial.SpecularBias = 0.8f;
				this.mOrcRenderData[(int)iDataChannel].mMaterial.SpecularPower = 20f;
				this.mOrcRenderData[(int)iDataChannel].mMaterial.CubeMapEnabled = true;
				this.mOrcRenderData[(int)iDataChannel].mMaterial.CubeNormalMapEnabled = true;
				this.mOrcRenderData[(int)iDataChannel].mMaterial.CubeMap = this.mIceCubeMap;
				this.mOrcRenderData[(int)iDataChannel].mMaterial.CubeNormalMap = this.mIceCubeNormalMap;
				this.mOrcRenderData[(int)iDataChannel].mMaterial.CubeMapColor.X = (this.mOrcRenderData[(int)iDataChannel].mMaterial.CubeMapColor.Y = (this.mOrcRenderData[(int)iDataChannel].mMaterial.CubeMapColor.Z = 1f));
				this.mOrcRenderData[(int)iDataChannel].mMaterial.CubeMapColor.W = 1f - (float)Math.Pow(0.20000000298023224, (double)this.StatusMagnitude(1, StatusEffects.Frozen));
			}
			else
			{
				this.mOrcRenderData[(int)iDataChannel].mMaterial.Bloat = 0f;
				this.mOrcRenderData[(int)iDataChannel].mMaterial.EmissiveAmount = this.mMaterial.EmissiveAmount;
				this.mOrcRenderData[(int)iDataChannel].mMaterial.SpecularBias = this.mMaterial.SpecularBias;
				this.mOrcRenderData[(int)iDataChannel].mMaterial.SpecularPower = this.mMaterial.SpecularPower;
				this.mOrcRenderData[(int)iDataChannel].mMaterial.CubeMapEnabled = false;
				this.mOrcRenderData[(int)iDataChannel].mMaterial.CubeNormalMapEnabled = false;
			}
			this.mOrcRenderData[(int)iDataChannel].Flash = this.mDamageFlashTimer * 10f;
			this.mOrcController.Update(iDeltaTime, ref this.mMachineOrientation, true);
			this.mOrcController.SkinnedBoneTransforms.CopyTo(this.mOrcRenderData[(int)iDataChannel].mSkeleton, 0);
			this.mPlayState.Scene.AddRenderableObject(iDataChannel, this.mOrcRenderData[(int)iDataChannel]);
			if (this.mRenderKing)
			{
				this.mKingController.Update(iDeltaTime, ref this.mKingSpawnOrientation, true);
				this.mKingController.SkinnedBoneTransforms.CopyTo(this.mKingRenderData[(int)iDataChannel].mSkeleton, 0);
				this.mPlayState.Scene.AddRenderableObject(iDataChannel, this.mKingRenderData[(int)iDataChannel]);
			}
			for (int i = 0; i < this.mNPCs.Count; i++)
			{
				if (this.mNPCs[i].Dead)
				{
					this.mNPCs.RemoveAt(i--);
				}
			}
			Matrix transform = this.mDrillyBindPose;
			Matrix.Multiply(ref transform, ref this.mMachineController.SkinnedBoneTransforms[this.mDrillyIndex], out transform);
			Vector3 translation = transform.Translation;
			Vector3 right = transform.Right;
			Vector3.Multiply(ref right, 6.2f, out right);
			Vector3.Add(ref right, ref translation, out translation);
			transform.Translation = translation;
			this.mRulerRenderData[(int)iDataChannel].Transform = transform;
			this.mRulerRenderData[(int)iDataChannel].Time = this.mDrillTime;
			this.mRulerRenderData[(int)iDataChannel].Alpha = Math.Min(Math.Max(this.mRulerAlpha, 0f), 1f);
			this.mPlayState.Scene.AddRenderableGUIObject(iDataChannel, this.mRulerRenderData[(int)iDataChannel]);
		}

		// Token: 0x0600275D RID: 10077 RVA: 0x001205F8 File Offset: 0x0011E7F8
		private IBossState<Machine> GetState(Machine.States iState)
		{
			switch (iState)
			{
			case Machine.States.Idle:
				return Machine.IdleState.Instance;
			case Machine.States.Drill:
				return Machine.DrillState.Instance;
			case Machine.States.Broken:
				return Machine.BrokenState.Instance;
			case Machine.States.KillKing:
				return Machine.KillKingState.Instance;
			case Machine.States.Intro:
				return Machine.IntroStage.Instance;
			case Machine.States.PreGunter:
				return Machine.PreGunterStage.Instance;
			case Machine.States.Warlock:
				return Machine.WarlockStage.Instance;
			case Machine.States.Final:
				return Machine.FinalStage.Instance;
			case Machine.States.PostFinal:
				return Machine.PostFinalStage.Instance;
			case Machine.States.KingKilled:
				return Machine.KingKilledStage.Instance;
			default:
				return null;
			}
		}

		// Token: 0x0600275E RID: 10078 RVA: 0x00120674 File Offset: 0x0011E874
		protected unsafe void ChangeMachineState(Machine.States iState)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					Machine.ChangeMachineStateMessage changeMachineStateMessage;
					changeMachineStateMessage.NewState = iState;
					BossFight.Instance.SendMessage<Machine.ChangeMachineStateMessage>(this, 2, (void*)(&changeMachineStateMessage), true);
				}
				this.mMachineState.OnExit(this);
				this.mMachineState = this.GetState(iState);
				this.mMachineState.OnEnter(this);
			}
		}

		// Token: 0x0600275F RID: 10079 RVA: 0x001206D8 File Offset: 0x0011E8D8
		protected unsafe void ChangeStage(Machine.States iState)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					Machine.ChangeFightStateMessage changeFightStateMessage;
					changeFightStateMessage.NewState = iState;
					BossFight.Instance.SendMessage<Machine.ChangeFightStateMessage>(this, 3, (void*)(&changeFightStateMessage), true);
				}
				this.mCurrentStage.OnExit(this);
				this.mCurrentStage = this.GetState(iState);
				this.mCurrentStage.OnEnter(this);
			}
		}

		// Token: 0x06002760 RID: 10080 RVA: 0x0012073C File Offset: 0x0011E93C
		public unsafe void TeleportWarlock(Matrix iOrientation)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				Vector3 vector = this.mWarlock.Position;
				Vector3 vector2 = this.mWarlock.Direction;
				VisualEffectReference visualEffectReference;
				EffectManager.Instance.StartEffect(Teleport.TELEPORT_EFFECT_DISAPPEAR, ref vector, ref vector2, out visualEffectReference);
				Cue cue = AudioManager.Instance.GetCue(Banks.Spells, Teleport.TELEPORT_SOUND_ORIGIN);
				this.mOriginEmitter.Up = Vector3.Up;
				this.mOriginEmitter.Position = vector;
				this.mOriginEmitter.Forward = vector2;
				cue.Apply3D(this.mPlayState.Camera.Listener, this.mOriginEmitter);
				cue.Play();
				Matrix orientation = iOrientation;
				vector = orientation.Translation;
				vector2 = orientation.Forward;
				Segment seg = default(Segment);
				seg.Origin = vector + Vector3.Up;
				seg.Delta.Y = -3f;
				float num;
				Vector3 vector3;
				Vector3 vector4;
				if (this.mPlayState.Level.CurrentScene.CollisionSkin.SegmentIntersect(out num, out vector3, out vector4, seg))
				{
					vector = vector3;
					vector.Y += 0.1f;
				}
				vector.Y += this.mWarlock.Capsule.Length * 0.5f + this.mWarlock.Capsule.Radius;
				EffectManager.Instance.StartEffect(Teleport.TELEPORT_EFFECT_APPEAR, ref vector, ref vector2, out visualEffectReference);
				cue = AudioManager.Instance.GetCue(Banks.Spells, Teleport.TELEPORT_SOUND_DESTINATION);
				this.mDestinationEmitter.Up = Vector3.Up;
				this.mDestinationEmitter.Position = vector;
				this.mDestinationEmitter.Forward = vector2;
				cue.Apply3D(this.mPlayState.Camera.Listener, this.mDestinationEmitter);
				cue.Play();
				orientation.Translation = default(Vector3);
				this.mWarlock.CharacterBody.DesiredDirection = iOrientation.Forward;
				this.mWarlock.StopStatusEffects(StatusEffects.Burning);
				this.mWarlock.StopStatusEffects(StatusEffects.Cold);
				this.mWarlock.CharacterBody.Force = default(Vector3);
				this.mWarlock.CharacterBody.Velocity = default(Vector3);
				this.mWarlock.CharacterBody.Movement = default(Vector3);
				this.mWarlock.Body.MoveTo(vector, orientation);
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					Machine.TeleportWarlockMessage teleportWarlockMessage;
					teleportWarlockMessage.Position = vector;
					teleportWarlockMessage.Direction = vector2;
					BossFight.Instance.SendMessage<Machine.TeleportWarlockMessage>(this, 5, (void*)(&teleportWarlockMessage), true);
				}
			}
		}

		// Token: 0x06002761 RID: 10081 RVA: 0x001209D0 File Offset: 0x0011EBD0
		public void OverKillKing()
		{
			Matrix matrix = this.mDrillyBindPose;
			Matrix.Multiply(ref matrix, ref this.mMachineController.SkinnedBoneTransforms[this.mDrillyIndex], out matrix);
			Vector3 translation = matrix.Translation;
			Vector3 right = matrix.Right;
			Vector3.Multiply(ref right, 6.2f, out right);
			Vector3.Add(ref right, ref translation, out translation);
			matrix.Translation = translation;
			BloodType bloodType = BloodType.wood;
			Vector3 translation2;
			for (int i = 0; i < this.mWarlock.Gibs.Count; i++)
			{
				Gib fromCache = Gib.GetFromCache();
				translation2.X = matrix.Translation.X + (float)this.mRandom.NextDouble();
				translation2.Y = matrix.Translation.Y + (float)this.mRandom.NextDouble();
				translation2.Z = matrix.Translation.Z + (float)this.mRandom.NextDouble();
				fromCache.Initialize(this.mWarlock.Gibs[i].mModel, this.mWarlock.Gibs[i].mMass, this.mWarlock.Gibs[i].mScale, translation2, this.mWarlock.Body.Velocity * 0.1f + new Vector3((float)this.mRandom.NextDouble() * 3f, (float)this.mRandom.NextDouble() * 10f, (float)MagickaMath.Random.NextDouble() * 3f), 15f, null, bloodType, Gib.GORE_GIB_TRAIL_EFFECTS[(int)bloodType], this.HasStatus(1, StatusEffects.Frozen));
				fromCache.Body.ApplyBodyAngImpulse(new Vector3((float)this.mRandom.NextDouble() * 25f, (float)this.mRandom.NextDouble() * 25f, (float)this.mRandom.NextDouble() * 25f));
				this.mPlayState.EntityManager.AddEntity(fromCache);
			}
			Vector3 forward = matrix.Forward;
			translation2 = matrix.Translation;
			VisualEffectReference visualEffectReference;
			EffectManager.Instance.StartEffect(Gib.GORE_GIB_MEDIUM_EFFECTS[(int)bloodType], ref translation2, ref forward, out visualEffectReference);
			Segment iSeg = default(Segment);
			iSeg.Origin = translation2 + new Vector3((float)this.mRandom.NextDouble(), 0f, (float)MagickaMath.Random.NextDouble());
			iSeg.Delta.Y = -2f;
			float num;
			Vector3 vector;
			AnimatedLevelPart iAnimation;
			if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out num, out translation, out vector, out iAnimation, iSeg))
			{
				DecalManager.Instance.AddAlphaBlendedDecal((Decal)(bloodType * BloodType.insect), iAnimation, 4f, ref translation, ref vector, 60f);
			}
			this.mGibAudioEmitter.Position = matrix.Translation;
			this.mGibAudioEmitter.Forward = matrix.Forward;
			this.mGibAudioEmitter.Up = Vector3.Up;
			AudioManager.Instance.PlayCue(Banks.Misc, "misc_gib".GetHashCodeCustom(), this.mGibAudioEmitter);
			this.mPlayState.Camera.CameraShake(1f, 1f);
			this.mRenderKing = false;
		}

		// Token: 0x06002762 RID: 10082 RVA: 0x00120D00 File Offset: 0x0011EF00
		public DamageResult Damage(int iPartIndex, Damage iDamage, Entity iAttacker, ref Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			if (this.mMachineState is Machine.BrokenState)
			{
				return DamageResult.None;
			}
			if (iPartIndex != 1 && (short)(iDamage.AttackProperty & AttackProperties.Status) == 32)
			{
				iDamage.AttackProperty &= ~AttackProperties.Status;
				if (iDamage.AttackProperty == (AttackProperties)0)
				{
					return DamageResult.None;
				}
			}
			DamageResult result = this.Damage(iDamage, iAttacker, iAttackPosition, iFeatures);
			if (this.mHitPoints <= 0f)
			{
				this.ChangeMachineState(Machine.States.Broken);
			}
			return result;
		}

		// Token: 0x06002763 RID: 10083 RVA: 0x00120D72 File Offset: 0x0011EF72
		public void Damage(int iPartIndex, float iDamage, Elements iElement)
		{
			if (this.mCurrentStage is Machine.IntroStage || this.mCurrentStage is Machine.PreGunterStage)
			{
				base.Damage(iDamage, iElement);
			}
		}

		// Token: 0x06002764 RID: 10084 RVA: 0x00120D98 File Offset: 0x0011EF98
		public unsafe void ScriptMessage(BossMessages iMessage)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					Machine.ScriptMessageMessage scriptMessageMessage;
					scriptMessageMessage.Message = iMessage;
					BossFight.Instance.SendMessage<Machine.ScriptMessageMessage>(this, 4, (void*)(&scriptMessageMessage), true);
				}
				switch (iMessage)
				{
				case BossMessages.OrcBipedal:
					this.mOrcController.Speed = 1f;
					this.mOrcController.CrossFade(this.mOrcClips[1], 0.25f, false);
					return;
				case BossMessages.GunterFight:
					break;
				case BossMessages.KingTalk1:
					this.mKingController.CrossFade(this.mKingClips[1], 0.25f, false);
					return;
				case BossMessages.KingTalk2:
					this.mKingController.CrossFade(this.mKingClips[2], 0.25f, false);
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x06002765 RID: 10085 RVA: 0x00120E50 File Offset: 0x0011F050
		public void SetSlow(int iIndex)
		{
		}

		// Token: 0x06002766 RID: 10086 RVA: 0x00120E52 File Offset: 0x0011F052
		public void GetRandomPositionOnCollisionSkin(int iIndex, out Vector3 oPosition)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06002767 RID: 10087 RVA: 0x00120E59 File Offset: 0x0011F059
		public bool HasStatus(int iIndex, StatusEffects iStatus)
		{
			return base.HasStatus(iStatus);
		}

		// Token: 0x06002768 RID: 10088 RVA: 0x00120E62 File Offset: 0x0011F062
		public float StatusMagnitude(int iIndex, StatusEffects iStatus)
		{
			return base.StatusMagnitude(iStatus);
		}

		// Token: 0x06002769 RID: 10089 RVA: 0x00120E6B File Offset: 0x0011F06B
		public bool AddImpulseVelocity(Vector3 iDirection, float iElevation, float iMassPower, float iDistance)
		{
			return false;
		}

		// Token: 0x1700093C RID: 2364
		// (get) Token: 0x0600276A RID: 10090 RVA: 0x00120E6E File Offset: 0x0011F06E
		public override bool Dead
		{
			get
			{
				return this.mDead;
			}
		}

		// Token: 0x1700093D RID: 2365
		// (get) Token: 0x0600276B RID: 10091 RVA: 0x00120E76 File Offset: 0x0011F076
		public float MaxHitPoints
		{
			get
			{
				if (this.mCurrentStage is Machine.IntroStage || this.mCurrentStage is Machine.PreGunterStage)
				{
					return 5000f;
				}
				if (!(this.mCurrentStage is Machine.PostFinalStage))
				{
					return this.mWarlock.MaxHitPoints;
				}
				return 1f;
			}
		}

		// Token: 0x1700093E RID: 2366
		// (get) Token: 0x0600276C RID: 10092 RVA: 0x00120EB8 File Offset: 0x0011F0B8
		public float HitPoints
		{
			get
			{
				if (this.mCurrentStage is Machine.IntroStage || this.mCurrentStage is Machine.PreGunterStage)
				{
					return Math.Max(this.mHitPoints, 0f) + 1f;
				}
				if (!(this.mCurrentStage is Machine.PostFinalStage))
				{
					return Math.Max(this.mWarlock.HitPoints, 0f) + 1f;
				}
				return 0f;
			}
		}

		// Token: 0x1700093F RID: 2367
		// (get) Token: 0x0600276D RID: 10093 RVA: 0x00120F24 File Offset: 0x0011F124
		protected override BossDamageZone Entity
		{
			get
			{
				return this.mOrcZone;
			}
		}

		// Token: 0x17000940 RID: 2368
		// (get) Token: 0x0600276E RID: 10094 RVA: 0x00120F2C File Offset: 0x0011F12C
		protected override float Radius
		{
			get
			{
				return 3f;
			}
		}

		// Token: 0x17000941 RID: 2369
		// (get) Token: 0x0600276F RID: 10095 RVA: 0x00120F33 File Offset: 0x0011F133
		protected override float Length
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x17000942 RID: 2370
		// (get) Token: 0x06002770 RID: 10096 RVA: 0x00120F3A File Offset: 0x0011F13A
		protected override int BloodEffect
		{
			get
			{
				return Machine.BLOOD_EFFECT;
			}
		}

		// Token: 0x17000943 RID: 2371
		// (get) Token: 0x06002771 RID: 10097 RVA: 0x00120F44 File Offset: 0x0011F144
		protected override Vector3 NotifierTextPostion
		{
			get
			{
				Vector3 position = this.mOrcZone.Position;
				position.Y += 2f;
				return position;
			}
		}

		// Token: 0x06002772 RID: 10098 RVA: 0x00120F74 File Offset: 0x0011F174
		private unsafe void NetworkUpdate()
		{
			NetworkServer networkServer = NetworkManager.Instance.Interface as NetworkServer;
			if (networkServer == null)
			{
				return;
			}
			Machine.UpdateMessage updateMessage = default(Machine.UpdateMessage);
			updateMessage.Animation = 0;
			while ((int)updateMessage.Animation < this.mMachineClips.Length && this.mMachineController.AnimationClip != this.mMachineClips[(int)updateMessage.Animation])
			{
				updateMessage.Animation += 1;
			}
			updateMessage.AnimationTime = this.mMachineController.Time;
			updateMessage.Hitpoints = this.mHitPoints;
			for (int i = 0; i < networkServer.Connections; i++)
			{
				float num = networkServer.GetLatency(i) * 0.5f;
				Machine.UpdateMessage updateMessage2 = updateMessage;
				updateMessage2.AnimationTime += num;
				BossFight.Instance.SendMessage<Machine.UpdateMessage>(this, 1, (void*)(&updateMessage), false, i);
			}
		}

		// Token: 0x06002773 RID: 10099 RVA: 0x00121044 File Offset: 0x0011F244
		public unsafe void NetworkUpdate(ref BossUpdateMessage iMsg)
		{
			switch (iMsg.Type)
			{
			case 1:
			{
				Machine.UpdateMessage updateMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&updateMessage));
				if (this.mMachineController.AnimationClip != this.mMachineClips[(int)updateMessage.Animation])
				{
					this.mMachineController.StartClip(this.mMachineClips[(int)updateMessage.Animation], false);
				}
				this.mMachineController.Time = updateMessage.AnimationTime;
				this.mHitPoints = updateMessage.Hitpoints;
				return;
			}
			case 2:
			{
				Machine.ChangeMachineStateMessage changeMachineStateMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&changeMachineStateMessage));
				this.mMachineState.OnExit(this);
				this.mMachineState = this.GetState(changeMachineStateMessage.NewState);
				this.mMachineState.OnEnter(this);
				return;
			}
			case 3:
			{
				Machine.ChangeFightStateMessage changeFightStateMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&changeFightStateMessage));
				this.mCurrentStage.OnExit(this);
				this.mCurrentStage = this.GetState(changeFightStateMessage.NewState);
				this.mCurrentStage.OnEnter(this);
				return;
			}
			case 4:
			{
				Machine.ScriptMessageMessage scriptMessageMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&scriptMessageMessage));
				switch (scriptMessageMessage.Message)
				{
				case BossMessages.OrcBipedal:
					this.mOrcController.Speed = 1f;
					this.mOrcController.CrossFade(this.mOrcClips[1], 0.25f, false);
					return;
				case BossMessages.GunterFight:
					break;
				case BossMessages.KingTalk1:
					this.mKingController.CrossFade(this.mKingClips[1], 0.25f, false);
					return;
				case BossMessages.KingTalk2:
					this.mKingController.CrossFade(this.mKingClips[2], 0.25f, false);
					return;
				default:
					throw new Exception(string.Concat(new object[]
					{
						"Incorrect message, ",
						scriptMessageMessage.Message,
						" passed to ",
						base.GetType().Name
					}));
				}
				break;
			}
			case 5:
			{
				Machine.TeleportWarlockMessage teleportWarlockMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&teleportWarlockMessage));
				Vector3 position = this.mWarlock.Position;
				Vector3 direction = this.mWarlock.Direction;
				VisualEffectReference visualEffectReference;
				EffectManager.Instance.StartEffect(Teleport.TELEPORT_EFFECT_DISAPPEAR, ref position, ref direction, out visualEffectReference);
				Cue cue = AudioManager.Instance.GetCue(Banks.Spells, Teleport.TELEPORT_SOUND_ORIGIN);
				this.mOriginEmitter.Up = Vector3.Up;
				this.mOriginEmitter.Position = position;
				this.mOriginEmitter.Forward = direction;
				cue.Apply3D(this.mPlayState.Camera.Listener, this.mOriginEmitter);
				cue.Play();
				EffectManager.Instance.StartEffect(Teleport.TELEPORT_EFFECT_APPEAR, ref teleportWarlockMessage.Position, ref teleportWarlockMessage.Direction, out visualEffectReference);
				cue = AudioManager.Instance.GetCue(Banks.Spells, Teleport.TELEPORT_SOUND_DESTINATION);
				this.mDestinationEmitter.Up = Vector3.Up;
				this.mDestinationEmitter.Position = position;
				this.mDestinationEmitter.Forward = direction;
				cue.Apply3D(this.mPlayState.Camera.Listener, this.mDestinationEmitter);
				cue.Play();
				this.mWarlock.CharacterBody.DesiredDirection = teleportWarlockMessage.Direction;
				this.mWarlock.StopStatusEffects(StatusEffects.Burning);
				this.mWarlock.StopStatusEffects(StatusEffects.Cold);
				this.mWarlock.CharacterBody.Force = default(Vector3);
				this.mWarlock.CharacterBody.Velocity = default(Vector3);
				this.mWarlock.CharacterBody.Movement = default(Vector3);
				this.mWarlock.Body.MoveTo(position, Matrix.CreateWorld(Vector3.Zero, teleportWarlockMessage.Direction, Vector3.Up));
				break;
			}
			default:
				return;
			}
		}

		// Token: 0x06002774 RID: 10100 RVA: 0x001213D0 File Offset: 0x0011F5D0
		public unsafe void NetworkInitialize(ref BossInitializeMessage iMsg)
		{
			if (iMsg.Type == 0)
			{
				Machine.InitializeMessage initializeMessage;
				BossInitializeMessage.ConvertTo(ref iMsg, (void*)(&initializeMessage));
				this.mWarlock = (Magicka.GameLogic.Entities.Entity.GetFromHandle((int)initializeMessage.Handle) as NonPlayerCharacter);
				this.mNetworkInitialized = true;
			}
		}

		// Token: 0x06002775 RID: 10101 RVA: 0x0012140C File Offset: 0x0011F60C
		public BossEnum GetBossType()
		{
			return BossEnum.Machine;
		}

		// Token: 0x17000944 RID: 2372
		// (get) Token: 0x06002776 RID: 10102 RVA: 0x0012140F File Offset: 0x0011F60F
		public bool NetworkInitialized
		{
			get
			{
				return this.mNetworkInitialized;
			}
		}

		// Token: 0x06002777 RID: 10103 RVA: 0x00121418 File Offset: 0x0011F618
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

		// Token: 0x04002A8C RID: 10892
		private const float NETWORK_UPDATE_PERIOD = 0.033333335f;

		// Token: 0x04002A8D RID: 10893
		private const int NPC_SPAWN_CAP = 2;

		// Token: 0x04002A8E RID: 10894
		private const float TOTAL_DURATION = 120f;

		// Token: 0x04002A8F RID: 10895
		private const float TOTAL_DIVISOR = 0.008333334f;

		// Token: 0x04002A90 RID: 10896
		private const float TIME_BETWEEN_SPAWNS = 1f;

		// Token: 0x04002A91 RID: 10897
		private const float TIME_BETWEEN_TAUNTS = 9f;

		// Token: 0x04002A92 RID: 10898
		private const float MAXHITPOINTS = 5000f;

		// Token: 0x04002A93 RID: 10899
		private const float DRILL_LENGTH = 6.2f;

		// Token: 0x04002A94 RID: 10900
		protected float mNetworkUpdateTimer;

		// Token: 0x04002A95 RID: 10901
		private float mDrillTime;

		// Token: 0x04002A96 RID: 10902
		private float mIdleTimer;

		// Token: 0x04002A97 RID: 10903
		private static readonly int MACHINE_BREAK_EFFECT = "machine_break".GetHashCodeCustom();

		// Token: 0x04002A98 RID: 10904
		private static readonly int[] MACHINE_SHRAPNEL_EFFECT = new int[]
		{
			"machine_shrapnel_cog".GetHashCodeCustom(),
			"machine_shrapnel_plank".GetHashCodeCustom(),
			"machine_shrapnel_misc".GetHashCodeCustom()
		};

		// Token: 0x04002A99 RID: 10905
		private int mCurrentDialog;

		// Token: 0x04002A9A RID: 10906
		private static readonly int DIALOG_MACHINE_DEAD = "machinedead".GetHashCodeCustom();

		// Token: 0x04002A9B RID: 10907
		private static readonly int DIALOG_KING1 = "kingsaved1".GetHashCodeCustom();

		// Token: 0x04002A9C RID: 10908
		private static readonly int DIALOG_KING2 = "kingsaved2".GetHashCodeCustom();

		// Token: 0x04002A9D RID: 10909
		private static List<int> DIALOG_MACHINE;

		// Token: 0x04002A9E RID: 10910
		private static readonly int MACHINE_DEATH_TRIGGER_ID = "machine_destroyed".GetHashCodeCustom();

		// Token: 0x04002A9F RID: 10911
		private static readonly int BIPEDAL_DONE_TRIGGER_ID = "bipedal_done".GetHashCodeCustom();

		// Token: 0x04002AA0 RID: 10912
		private static readonly int GUNTER_DEFEATED_TRIGGER_ID = "gunther_pissed".GetHashCodeCustom();

		// Token: 0x04002AA1 RID: 10913
		private static readonly int GUNTER_DEAD_TRIGGER_ID = "gunther_dead".GetHashCodeCustom();

		// Token: 0x04002AA2 RID: 10914
		private static readonly int BLOOD_EFFECT = "gore_splash_black".GetHashCodeCustom();

		// Token: 0x04002AA3 RID: 10915
		private float mPeddleSpeed = 1f;

		// Token: 0x04002AA4 RID: 10916
		private float mPeddleTargetSpeed = 1f;

		// Token: 0x04002AA5 RID: 10917
		private float mEmperorLightningTimer;

		// Token: 0x04002AA6 RID: 10918
		protected IBossState<Machine> mMachineState;

		// Token: 0x04002AA7 RID: 10919
		protected IBossState<Machine> mCurrentStage;

		// Token: 0x04002AA8 RID: 10920
		private Machine.RenderData[] mKingRenderData;

		// Token: 0x04002AA9 RID: 10921
		private Machine.RenderData[] mMachineRenderData;

		// Token: 0x04002AAA RID: 10922
		private Machine.OrcRenderData[] mOrcRenderData;

		// Token: 0x04002AAB RID: 10923
		private Random mRandom;

		// Token: 0x04002AAC RID: 10924
		private List<NonPlayerCharacter> mNPCs;

		// Token: 0x04002AAD RID: 10925
		private NonPlayerCharacter mWarlock;

		// Token: 0x04002AAE RID: 10926
		private bool mDead;

		// Token: 0x04002AAF RID: 10927
		private BossDamageZone mMachineZone;

		// Token: 0x04002AB0 RID: 10928
		private BossDamageZone mOrcZone;

		// Token: 0x04002AB1 RID: 10929
		private PlayState mPlayState;

		// Token: 0x04002AB2 RID: 10930
		private SkinnedModel mKingModel;

		// Token: 0x04002AB3 RID: 10931
		private AnimationClip[] mKingClips;

		// Token: 0x04002AB4 RID: 10932
		private AnimationController mKingController;

		// Token: 0x04002AB5 RID: 10933
		private SkinnedModel mMachineModel;

		// Token: 0x04002AB6 RID: 10934
		private AnimationClip[] mMachineClips;

		// Token: 0x04002AB7 RID: 10935
		private AnimationController mMachineController;

		// Token: 0x04002AB8 RID: 10936
		private AnimationClip[] mOrcClips;

		// Token: 0x04002AB9 RID: 10937
		private AnimationController mOrcController;

		// Token: 0x04002ABA RID: 10938
		private int mDrillIndex;

		// Token: 0x04002ABB RID: 10939
		private Matrix mMachineOrientation;

		// Token: 0x04002ABC RID: 10940
		private Vector3 mBicyclePosition;

		// Token: 0x04002ABD RID: 10941
		private bool mRenderKing;

		// Token: 0x04002ABE RID: 10942
		private Matrix mKingSpawnOrientation;

		// Token: 0x04002ABF RID: 10943
		private Matrix mWarlockSpawnOrientation;

		// Token: 0x04002AC0 RID: 10944
		private Matrix mWarlockEmperorOrientation;

		// Token: 0x04002AC1 RID: 10945
		private AIEvent[] mWarlockIntroEvent;

		// Token: 0x04002AC2 RID: 10946
		private AIEvent[] mWarlockFinalEvent;

		// Token: 0x04002AC3 RID: 10947
		private AIEvent[] mWarlockEmperorEvent;

		// Token: 0x04002AC4 RID: 10948
		private AIEvent[][] mAvatarMoveEvent;

		// Token: 0x04002AC5 RID: 10949
		private AudioEmitter mOriginEmitter;

		// Token: 0x04002AC6 RID: 10950
		private AudioEmitter mDestinationEmitter;

		// Token: 0x04002AC7 RID: 10951
		private AudioEmitter mAudioEmitter;

		// Token: 0x04002AC8 RID: 10952
		private AudioEmitter mGibAudioEmitter;

		// Token: 0x04002AC9 RID: 10953
		private static readonly Random RANDOM = new Random();

		// Token: 0x04002ACA RID: 10954
		private HitList mShrapnelHitList;

		// Token: 0x04002ACB RID: 10955
		private Machine.RulerRenderData[] mRulerRenderData;

		// Token: 0x04002ACC RID: 10956
		private Shield mOrcStageShield;

		// Token: 0x04002ACD RID: 10957
		private int mDrillyIndex;

		// Token: 0x04002ACE RID: 10958
		private Matrix mDrillyBindPose;

		// Token: 0x04002ACF RID: 10959
		private float mRulerAlpha;

		// Token: 0x04002AD0 RID: 10960
		private float mDamageFlashTimer;

		// Token: 0x04002AD1 RID: 10961
		private static readonly Vector3 DRILLY_START = new Vector3(19.44f, 2.57f, -57.97f);

		// Token: 0x04002AD2 RID: 10962
		private static readonly Vector3 DRILLY_END = new Vector3(21.36f, 1.7f, -58.5f);

		// Token: 0x04002AD3 RID: 10963
		private static readonly float DISTANCE = Vector3.Distance(Machine.DRILLY_END, Machine.DRILLY_START);

		// Token: 0x04002AD4 RID: 10964
		private static readonly Matrix INV_SCENE_ROTATION = Matrix.CreateRotationY(0.3455752f);

		// Token: 0x04002AD5 RID: 10965
		private SkinnedModelDeferredAdvancedMaterial mMaterial;

		// Token: 0x04002AD6 RID: 10966
		private TextureCube mIceCubeMap;

		// Token: 0x04002AD7 RID: 10967
		private TextureCube mIceCubeNormalMap;

		// Token: 0x04002AD8 RID: 10968
		private bool mNetworkInitialized;

		// Token: 0x0200051B RID: 1307
		protected class RenderData : RenderableObject<SkinnedModelDeferredEffect, SkinnedModelDeferredAdvancedMaterial>
		{
			// Token: 0x06002779 RID: 10105 RVA: 0x001215FB File Offset: 0x0011F7FB
			public RenderData()
			{
				this.mSkeleton = new Matrix[80];
			}

			// Token: 0x0600277A RID: 10106 RVA: 0x00121610 File Offset: 0x0011F810
			public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				skinnedModelDeferredEffect.Bones = this.mSkeleton;
				skinnedModelDeferredEffect.OverrideColor = new Vector4(1f, 1f, 1f, this.Flash);
				base.Draw(iEffect, iViewFrustum);
				skinnedModelDeferredEffect.OverrideColor = Vector4.Zero;
			}

			// Token: 0x0600277B RID: 10107 RVA: 0x00121664 File Offset: 0x0011F864
			public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				skinnedModelDeferredEffect.Bones = this.mSkeleton;
				skinnedModelDeferredEffect.OverrideColor = new Vector4(1f, 1f, 1f, this.Flash);
				base.DrawShadow(iEffect, iViewFrustum);
				skinnedModelDeferredEffect.OverrideColor = Vector4.Zero;
			}

			// Token: 0x04002AD9 RID: 10969
			public Matrix[] mSkeleton;

			// Token: 0x04002ADA RID: 10970
			public float Flash;
		}

		// Token: 0x0200051C RID: 1308
		protected class OrcRenderData : RenderableObject<SkinnedModelDeferredEffect, SkinnedModelDeferredAdvancedMaterial>
		{
			// Token: 0x0600277C RID: 10108 RVA: 0x001216B7 File Offset: 0x0011F8B7
			public OrcRenderData()
			{
				this.mSkeleton = new Matrix[80];
			}

			// Token: 0x0600277D RID: 10109 RVA: 0x001216CC File Offset: 0x0011F8CC
			public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				skinnedModelDeferredEffect.Bones = this.mSkeleton;
				skinnedModelDeferredEffect.OverrideColor = new Vector4(1f, 1f, 1f, this.Flash);
				base.Draw(iEffect, iViewFrustum);
				this.mMaterial2.AssignToEffect(skinnedModelDeferredEffect);
				skinnedModelDeferredEffect.CommitChanges();
				skinnedModelDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex2, 0, this.mNumVertices2, this.mStartIndex2, this.mPrimitiveCount2);
				skinnedModelDeferredEffect.OverrideColor = Vector4.Zero;
			}

			// Token: 0x0600277E RID: 10110 RVA: 0x00121758 File Offset: 0x0011F958
			public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				skinnedModelDeferredEffect.Bones = this.mSkeleton;
				base.Draw(iEffect, iViewFrustum);
				this.mMaterial2.AssignToEffect(skinnedModelDeferredEffect);
				skinnedModelDeferredEffect.CommitChanges();
				skinnedModelDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex2, 0, this.mNumVertices2, this.mStartIndex2, this.mPrimitiveCount2);
			}

			// Token: 0x0600277F RID: 10111 RVA: 0x001217B8 File Offset: 0x0011F9B8
			public void SetMesh(VertexBuffer iVertices, IndexBuffer iIndices, ModelMeshPart iMeshPart, ModelMeshPart iMeshPart2, int iTechnique, int iDepthTechnique, int iShadowTechnique)
			{
				this.mVertexDeclaration2 = iMeshPart2.VertexDeclaration;
				this.mBaseVertex2 = iMeshPart2.BaseVertex;
				this.mNumVertices2 = iMeshPart2.NumVertices;
				this.mStartIndex2 = iMeshPart2.StartIndex;
				this.mPrimitiveCount2 = iMeshPart2.PrimitiveCount;
				this.mVertexStride2 = iMeshPart2.VertexStride;
				base.SetMesh(iVertices, iIndices, iMeshPart, iTechnique, iDepthTechnique, iShadowTechnique);
			}

			// Token: 0x04002ADB RID: 10971
			public SkinnedModelDeferredAdvancedMaterial mMaterial2;

			// Token: 0x04002ADC RID: 10972
			protected VertexDeclaration mVertexDeclaration2;

			// Token: 0x04002ADD RID: 10973
			protected int mBaseVertex2;

			// Token: 0x04002ADE RID: 10974
			protected int mNumVertices2;

			// Token: 0x04002ADF RID: 10975
			protected int mStartIndex2;

			// Token: 0x04002AE0 RID: 10976
			protected int mPrimitiveCount2;

			// Token: 0x04002AE1 RID: 10977
			protected int mVertexStride2;

			// Token: 0x04002AE2 RID: 10978
			public Matrix[] mSkeleton;

			// Token: 0x04002AE3 RID: 10979
			public float Flash;
		}

		// Token: 0x0200051D RID: 1309
		protected class RulerRenderData : IRenderableGUIObject, IPreRenderRenderer
		{
			// Token: 0x06002780 RID: 10112 RVA: 0x00121822 File Offset: 0x0011FA22
			public RulerRenderData()
			{
				this.mText = new Text(20, FontManager.Instance.GetFont(MagickaFont.Maiandra14), TextAlign.Center, true);
			}

			// Token: 0x06002781 RID: 10113 RVA: 0x00121844 File Offset: 0x0011FA44
			public void Draw(float iDeltaTime)
			{
				Point screenSize = RenderManager.Instance.ScreenSize;
				this.Effect.VertexColorEnabled = false;
				this.Effect.TextureEnabled = true;
				this.Effect.Alpha = this.Alpha;
				this.Transform.Up = Vector3.Up;
				this.Effect.World = this.Transform;
				this.Effect.View = Matrix.Identity;
				this.Effect.Projection = this.mViewProjection;
				this.Effect.GraphicsDevice.VertexDeclaration = this.VertexDeclaration;
				this.Effect.Begin();
				this.Effect.CurrentTechnique.Passes[0].Begin();
				this.Effect.GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleStrip, this.Vertices, 0, 6);
				this.Effect.CurrentTechnique.Passes[0].End();
				this.Effect.End();
				this.TextEffect.VertexColorEnabled = true;
				this.TextEffect.SetScreenSize(screenSize.X, screenSize.Y);
				this.TextEffect.Color = new Vector4(1f, 1f, 1f, this.Alpha);
				this.TextEffect.Begin();
				this.TextEffect.CurrentTechnique.Passes[0].Begin();
				this.mText.Draw(this.TextEffect, (float)Math.Floor((double)this.mTextPosition.X), (float)Math.Floor((double)this.mTextPosition.Y));
				this.TextEffect.CurrentTechnique.Passes[0].End();
				this.TextEffect.End();
			}

			// Token: 0x17000945 RID: 2373
			// (get) Token: 0x06002782 RID: 10114 RVA: 0x00121A0D File Offset: 0x0011FC0D
			public int ZIndex
			{
				get
				{
					return 100;
				}
			}

			// Token: 0x06002783 RID: 10115 RVA: 0x00121A14 File Offset: 0x0011FC14
			public void PreRenderUpdate(DataChannel iDataChannel, float iDeltaTime, ref Matrix iViewProjectionMatrix, ref Vector3 iCameraPosition, ref Vector3 iCameraDirection)
			{
				this.mViewProjection = iViewProjectionMatrix;
				float num = (120f - this.Time) * 0.008333334f * Machine.DISTANCE;
				int num2 = (int)num;
				int num3 = (int)(num * 10f % 10f);
				int num4 = (int)(num * 100f % 10f);
				int num5 = (int)(num * 1000f % 10f);
				this.mText.Characters[0] = (char)(48 + num2);
				this.mText.Characters[1] = '.';
				this.mText.Characters[2] = (char)(48 + num3);
				this.mText.Characters[3] = (char)(48 + num4);
				this.mText.Characters[4] = (char)(48 + num5);
				this.mText.Characters[5] = 'm';
				this.mText.Characters[6] = '\0';
				this.mText.MarkAsDirty();
				Vector3 vector = this.Transform.Translation + new Vector3(0.5f, 1.3f, 0f);
				this.mTextPosition = MagickaMath.WorldToScreenPosition(ref vector, ref iViewProjectionMatrix);
				this.Vertices[0].Position = new Vector3(-0.25f, -0.1f, 0f);
				this.Vertices[1].Position = new Vector3(-0.25f, 0.9f, 0f);
				this.Vertices[2].Position = new Vector3(0f, -0.1f, 0f);
				this.Vertices[3].Position = new Vector3(0f, 0.9f, 0f);
				this.Vertices[4].Position = new Vector3(num, -0.1f, 0f);
				this.Vertices[5].Position = new Vector3(num, 0.9f, 0f);
				this.Vertices[6].Position = new Vector3(0.25f + num, -0.1f, 0f);
				this.Vertices[7].Position = new Vector3(0.25f + num, 0.9f, 0f);
			}

			// Token: 0x04002AE4 RID: 10980
			public BasicEffect Effect;

			// Token: 0x04002AE5 RID: 10981
			public VertexPositionTexture[] Vertices;

			// Token: 0x04002AE6 RID: 10982
			public VertexDeclaration VertexDeclaration;

			// Token: 0x04002AE7 RID: 10983
			public Matrix Transform;

			// Token: 0x04002AE8 RID: 10984
			public float Time;

			// Token: 0x04002AE9 RID: 10985
			public float Alpha;

			// Token: 0x04002AEA RID: 10986
			private Matrix mViewProjection;

			// Token: 0x04002AEB RID: 10987
			private Text mText;

			// Token: 0x04002AEC RID: 10988
			public GUIBasicEffect TextEffect;

			// Token: 0x04002AED RID: 10989
			private Vector2 mTextPosition;
		}

		// Token: 0x0200051E RID: 1310
		protected class IdleState : IBossState<Machine>
		{
			// Token: 0x17000946 RID: 2374
			// (get) Token: 0x06002784 RID: 10116 RVA: 0x00121C50 File Offset: 0x0011FE50
			public static Machine.IdleState Instance
			{
				get
				{
					if (Machine.IdleState.mSingelton == null)
					{
						lock (Machine.IdleState.mSingeltonLock)
						{
							if (Machine.IdleState.mSingelton == null)
							{
								Machine.IdleState.mSingelton = new Machine.IdleState();
							}
						}
					}
					return Machine.IdleState.mSingelton;
				}
			}

			// Token: 0x06002785 RID: 10117 RVA: 0x00121CA4 File Offset: 0x0011FEA4
			private IdleState()
			{
			}

			// Token: 0x06002786 RID: 10118 RVA: 0x00121CAC File Offset: 0x0011FEAC
			public void OnEnter(Machine iOwner)
			{
				iOwner.mMachineController.StartClip(iOwner.mMachineClips[1], true);
				iOwner.mOrcController.StartClip(iOwner.mOrcClips[3], true);
			}

			// Token: 0x06002787 RID: 10119 RVA: 0x00121CD6 File Offset: 0x0011FED6
			public void OnUpdate(float iDeltaTime, Machine iOwner)
			{
			}

			// Token: 0x06002788 RID: 10120 RVA: 0x00121CD8 File Offset: 0x0011FED8
			public void OnExit(Machine iOwner)
			{
			}

			// Token: 0x04002AEE RID: 10990
			private static Machine.IdleState mSingelton;

			// Token: 0x04002AEF RID: 10991
			private static volatile object mSingeltonLock = new object();
		}

		// Token: 0x0200051F RID: 1311
		protected class DrillState : IBossState<Machine>
		{
			// Token: 0x17000947 RID: 2375
			// (get) Token: 0x0600278A RID: 10122 RVA: 0x00121CE8 File Offset: 0x0011FEE8
			public static Machine.DrillState Instance
			{
				get
				{
					if (Machine.DrillState.mSingelton == null)
					{
						lock (Machine.DrillState.mSingeltonLock)
						{
							if (Machine.DrillState.mSingelton == null)
							{
								Machine.DrillState.mSingelton = new Machine.DrillState();
							}
						}
					}
					return Machine.DrillState.mSingelton;
				}
			}

			// Token: 0x0600278B RID: 10123 RVA: 0x00121D3C File Offset: 0x0011FF3C
			private DrillState()
			{
			}

			// Token: 0x0600278C RID: 10124 RVA: 0x00121D44 File Offset: 0x0011FF44
			public void OnEnter(Machine iOwner)
			{
				iOwner.mOrcController.CrossFade(iOwner.mOrcClips[2], 0.2f, false);
				iOwner.mOrcController.Speed = 1f;
			}

			// Token: 0x0600278D RID: 10125 RVA: 0x00121D70 File Offset: 0x0011FF70
			public void OnUpdate(float iDeltaTime, Machine iOwner)
			{
				iOwner.mRulerAlpha += iDeltaTime;
				if (iOwner.mOrcClips[2] == iOwner.mOrcController.AnimationClip)
				{
					if (iOwner.mOrcController.HasFinished && !iOwner.mOrcController.CrossFadeEnabled)
					{
						iOwner.mOrcController.CrossFade(iOwner.mOrcClips[0], 0.2f, true);
						iOwner.mOrcController.Speed = 0.1f;
						iOwner.mMachineController.CrossFade(iOwner.mMachineClips[0], 0.2f, true);
						iOwner.mMachineController.Speed = 0.1f;
						return;
					}
				}
				else
				{
					if (iOwner.mDrillTime >= 120f)
					{
						iOwner.ChangeMachineState(Machine.States.KillKing);
					}
					iOwner.mDrillTime += iDeltaTime * iOwner.mPeddleSpeed;
					iOwner.mMachineController.Speed = iOwner.mPeddleSpeed;
					iOwner.mOrcController.Speed = iOwner.mPeddleSpeed;
				}
			}

			// Token: 0x0600278E RID: 10126 RVA: 0x00121E5E File Offset: 0x0012005E
			public void OnExit(Machine iOwner)
			{
			}

			// Token: 0x04002AF0 RID: 10992
			private static Machine.DrillState mSingelton;

			// Token: 0x04002AF1 RID: 10993
			private static volatile object mSingeltonLock = new object();
		}

		// Token: 0x02000520 RID: 1312
		protected class BrokenState : IBossState<Machine>
		{
			// Token: 0x17000948 RID: 2376
			// (get) Token: 0x06002790 RID: 10128 RVA: 0x00121E70 File Offset: 0x00120070
			public static Machine.BrokenState Instance
			{
				get
				{
					if (Machine.BrokenState.mSingelton == null)
					{
						lock (Machine.BrokenState.mSingeltonLock)
						{
							if (Machine.BrokenState.mSingelton == null)
							{
								Machine.BrokenState.mSingelton = new Machine.BrokenState();
							}
						}
					}
					return Machine.BrokenState.mSingelton;
				}
			}

			// Token: 0x06002791 RID: 10129 RVA: 0x00121EC4 File Offset: 0x001200C4
			private BrokenState()
			{
			}

			// Token: 0x06002792 RID: 10130 RVA: 0x00121ECC File Offset: 0x001200CC
			public void OnEnter(Machine iOwner)
			{
				Matrix mMachineOrientation = iOwner.mMachineOrientation;
				Vector3 translation = mMachineOrientation.Translation;
				translation.Y = 0f;
				mMachineOrientation.Translation = translation;
				VisualEffectReference visualEffectReference;
				EffectManager.Instance.StartEffect(Machine.MACHINE_BREAK_EFFECT, ref mMachineOrientation, out visualEffectReference);
				iOwner.mMachineController.CrossFade(iOwner.mMachineClips[2], 0.2f, false);
				iOwner.mMachineController.Speed = 1f;
				StaticList<Entity> entities = iOwner.mPlayState.EntityManager.Entities;
				foreach (Entity entity in entities)
				{
					if (entity is Barrier | entity is MissileEntity | entity is SpellMine | entity is Grease.GreaseField)
					{
						entity.Kill();
					}
				}
				iOwner.mPlayState.Level.CurrentScene.ExecuteTrigger(Machine.MACHINE_DEATH_TRIGGER_ID, null, false);
				AudioManager.Instance.PlayCue(Banks.Misc, "misc_machine_break".GetHashCodeCustom(), iOwner.mAudioEmitter);
				iOwner.mRulerAlpha = 0f;
			}

			// Token: 0x06002793 RID: 10131 RVA: 0x00122000 File Offset: 0x00120200
			public void OnUpdate(float iDeltaTime, Machine iOwner)
			{
				iOwner.mPeddleTargetSpeed = 1f;
				iOwner.mRulerAlpha -= iDeltaTime;
				if (iOwner.mOrcController.HasFinished && !iOwner.mOrcController.CrossFadeEnabled && iOwner.mOrcController.AnimationClip == iOwner.mOrcClips[1] && !iOwner.mPlayState.Level.CurrentScene.Triggers[Machine.BIPEDAL_DONE_TRIGGER_ID].HasTriggered)
				{
					iOwner.mPlayState.Level.CurrentScene.Triggers[Machine.BIPEDAL_DONE_TRIGGER_ID].Execute(null, false);
					iOwner.ChangeStage(Machine.States.Warlock);
				}
			}

			// Token: 0x06002794 RID: 10132 RVA: 0x001220A8 File Offset: 0x001202A8
			public void OnExit(Machine iOwner)
			{
			}

			// Token: 0x04002AF2 RID: 10994
			private static Machine.BrokenState mSingelton;

			// Token: 0x04002AF3 RID: 10995
			private static volatile object mSingeltonLock = new object();
		}

		// Token: 0x02000521 RID: 1313
		protected class KillKingState : IBossState<Machine>
		{
			// Token: 0x17000949 RID: 2377
			// (get) Token: 0x06002796 RID: 10134 RVA: 0x001220B8 File Offset: 0x001202B8
			public static Machine.KillKingState Instance
			{
				get
				{
					if (Machine.KillKingState.mSingelton == null)
					{
						lock (Machine.KillKingState.mSingeltonLock)
						{
							if (Machine.KillKingState.mSingelton == null)
							{
								Machine.KillKingState.mSingelton = new Machine.KillKingState();
							}
						}
					}
					return Machine.KillKingState.mSingelton;
				}
			}

			// Token: 0x06002797 RID: 10135 RVA: 0x0012210C File Offset: 0x0012030C
			private KillKingState()
			{
			}

			// Token: 0x06002798 RID: 10136 RVA: 0x00122114 File Offset: 0x00120314
			public void OnEnter(Machine iOwner)
			{
				iOwner.mOrcController.CrossFade(iOwner.mOrcClips[3], 0.2f, true);
				iOwner.mOrcController.Speed = 1f;
				iOwner.mMachineController.CrossFade(iOwner.mMachineClips[1], 0.2f, true);
				iOwner.mOrcController.Speed = 1f;
				iOwner.ChangeStage(Machine.States.KingKilled);
			}

			// Token: 0x06002799 RID: 10137 RVA: 0x0012217B File Offset: 0x0012037B
			public void OnUpdate(float iDeltaTime, Machine iOwner)
			{
			}

			// Token: 0x0600279A RID: 10138 RVA: 0x0012217D File Offset: 0x0012037D
			public void OnExit(Machine iOwner)
			{
			}

			// Token: 0x04002AF4 RID: 10996
			private static Machine.KillKingState mSingelton;

			// Token: 0x04002AF5 RID: 10997
			private static volatile object mSingeltonLock = new object();
		}

		// Token: 0x02000522 RID: 1314
		protected class IntroStage : IBossState<Machine>
		{
			// Token: 0x1700094A RID: 2378
			// (get) Token: 0x0600279C RID: 10140 RVA: 0x00122190 File Offset: 0x00120390
			public static Machine.IntroStage Instance
			{
				get
				{
					if (Machine.IntroStage.mSingelton == null)
					{
						lock (Machine.IntroStage.mSingeltonLock)
						{
							if (Machine.IntroStage.mSingelton == null)
							{
								Machine.IntroStage.mSingelton = new Machine.IntroStage();
							}
						}
					}
					return Machine.IntroStage.mSingelton;
				}
			}

			// Token: 0x0600279D RID: 10141 RVA: 0x001221E4 File Offset: 0x001203E4
			private IntroStage()
			{
			}

			// Token: 0x0600279E RID: 10142 RVA: 0x001221EC File Offset: 0x001203EC
			public void OnEnter(Machine iOwner)
			{
				iOwner.mKingController.StartClip(iOwner.mKingClips[0], true);
			}

			// Token: 0x0600279F RID: 10143 RVA: 0x00122202 File Offset: 0x00120402
			public void OnUpdate(float iDeltaTime, Machine iOwner)
			{
			}

			// Token: 0x060027A0 RID: 10144 RVA: 0x00122204 File Offset: 0x00120404
			public void OnExit(Machine iOwner)
			{
			}

			// Token: 0x04002AF6 RID: 10998
			private static Machine.IntroStage mSingelton;

			// Token: 0x04002AF7 RID: 10999
			private static volatile object mSingeltonLock = new object();
		}

		// Token: 0x02000523 RID: 1315
		protected class PreGunterStage : IBossState<Machine>
		{
			// Token: 0x1700094B RID: 2379
			// (get) Token: 0x060027A2 RID: 10146 RVA: 0x00122214 File Offset: 0x00120414
			public static Machine.PreGunterStage Instance
			{
				get
				{
					if (Machine.PreGunterStage.mSingelton == null)
					{
						lock (Machine.PreGunterStage.mSingeltonLock)
						{
							if (Machine.PreGunterStage.mSingelton == null)
							{
								Machine.PreGunterStage.mSingelton = new Machine.PreGunterStage();
							}
						}
					}
					return Machine.PreGunterStage.mSingelton;
				}
			}

			// Token: 0x060027A3 RID: 10147 RVA: 0x00122268 File Offset: 0x00120468
			private PreGunterStage()
			{
			}

			// Token: 0x060027A4 RID: 10148 RVA: 0x00122270 File Offset: 0x00120470
			public void OnEnter(Machine iOwner)
			{
				iOwner.mIdleTimer = 0f;
				iOwner.mKingController.CrossFade(iOwner.mKingClips[3], 0.1f, true);
				iOwner.mWarlock.AI.SetOrder(Order.Idle, ReactTo.None, Order.Idle, 0, 0, 0, null);
				iOwner.mWarlock.SetImmortalTime(float.PositiveInfinity);
			}

			// Token: 0x060027A5 RID: 10149 RVA: 0x001222C8 File Offset: 0x001204C8
			public void OnUpdate(float iDeltaTime, Machine iOwner)
			{
				if (iOwner.mIdleTimer > 9f)
				{
					iOwner.mIdleTimer -= 9f;
					iOwner.mCurrentDialog = Machine.DIALOG_MACHINE[iOwner.mRandom.Next(Machine.DIALOG_MACHINE.Count)];
					DialogManager.Instance.StartDialog(iOwner.mCurrentDialog, iOwner.mWarlock, null);
				}
				iOwner.mIdleTimer += iDeltaTime;
			}

			// Token: 0x060027A6 RID: 10150 RVA: 0x0012233E File Offset: 0x0012053E
			public void OnExit(Machine iOwner)
			{
			}

			// Token: 0x04002AF8 RID: 11000
			private static Machine.PreGunterStage mSingelton;

			// Token: 0x04002AF9 RID: 11001
			private static volatile object mSingeltonLock = new object();
		}

		// Token: 0x02000524 RID: 1316
		protected class WarlockStage : IBossState<Machine>
		{
			// Token: 0x1700094C RID: 2380
			// (get) Token: 0x060027A8 RID: 10152 RVA: 0x00122350 File Offset: 0x00120550
			public static Machine.WarlockStage Instance
			{
				get
				{
					if (Machine.WarlockStage.mSingelton == null)
					{
						lock (Machine.WarlockStage.mSingeltonLock)
						{
							if (Machine.WarlockStage.mSingelton == null)
							{
								Machine.WarlockStage.mSingelton = new Machine.WarlockStage();
							}
						}
					}
					return Machine.WarlockStage.mSingelton;
				}
			}

			// Token: 0x060027A9 RID: 10153 RVA: 0x001223A4 File Offset: 0x001205A4
			private WarlockStage()
			{
			}

			// Token: 0x060027AA RID: 10154 RVA: 0x001223AC File Offset: 0x001205AC
			public void OnEnter(Machine iOwner)
			{
				iOwner.mWarlock.AI.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, 0, 0, null);
				iOwner.mWarlock.SetImmortalTime(0f);
				iOwner.mWarlock.HitPoints = iOwner.mWarlock.MaxHitPoints;
			}

			// Token: 0x060027AB RID: 10155 RVA: 0x001223EB File Offset: 0x001205EB
			public void OnUpdate(float iDeltaTime, Machine iOwner)
			{
				if (iOwner.mWarlock.HitPoints <= 0f)
				{
					iOwner.ChangeStage(Machine.States.Final);
				}
			}

			// Token: 0x060027AC RID: 10156 RVA: 0x00122406 File Offset: 0x00120606
			public void OnExit(Machine iOwner)
			{
			}

			// Token: 0x04002AFA RID: 11002
			private static Machine.WarlockStage mSingelton;

			// Token: 0x04002AFB RID: 11003
			private static volatile object mSingeltonLock = new object();
		}

		// Token: 0x02000525 RID: 1317
		protected class FinalStage : IBossState<Machine>
		{
			// Token: 0x1700094D RID: 2381
			// (get) Token: 0x060027AE RID: 10158 RVA: 0x00122418 File Offset: 0x00120618
			public static Machine.FinalStage Instance
			{
				get
				{
					if (Machine.FinalStage.mSingelton == null)
					{
						lock (Machine.FinalStage.mSingeltonLock)
						{
							if (Machine.FinalStage.mSingelton == null)
							{
								Machine.FinalStage.mSingelton = new Machine.FinalStage();
							}
						}
					}
					return Machine.FinalStage.mSingelton;
				}
			}

			// Token: 0x060027AF RID: 10159 RVA: 0x0012246C File Offset: 0x0012066C
			private FinalStage()
			{
				this.mSSV.mMagnitude = 2f;
			}

			// Token: 0x060027B0 RID: 10160 RVA: 0x00122484 File Offset: 0x00120684
			public void OnEnter(Machine iOwner)
			{
				iOwner.mPlayState.Level.CurrentScene.ExecuteTrigger(Machine.GUNTER_DEFEATED_TRIGGER_ID, null, false);
				iOwner.mWarlock.StopStatusEffects(StatusEffects.Wet);
				iOwner.mWarlock.StopStatusEffects(StatusEffects.Burning);
				iOwner.mWarlock.StopStatusEffects(StatusEffects.Cold);
				iOwner.mWarlock.StopStatusEffects(StatusEffects.Steamed);
				iOwner.TeleportWarlock(iOwner.mWarlockEmperorOrientation);
				while (iOwner.mWarlock.AI.CurrentState != AIStateIdle.Instance)
				{
					if (iOwner.mWarlock.AI.CurrentState is AIStateAttack)
					{
						iOwner.mWarlock.AI.ReleaseTarget();
					}
					iOwner.mWarlock.AI.PopState();
				}
				iOwner.mWarlock.AI.SetOrder(Order.Idle, ReactTo.None, Order.Idle, 0, 0, 0, iOwner.mWarlockEmperorEvent);
				AudioManager.Instance.PlayCue(Banks.Misc, "misc_trapdoor".GetHashCodeCustom(), iOwner.mAudioEmitter);
				iOwner.mKingController.CrossFade(iOwner.mKingClips[4], 0.25f, false);
				iOwner.mEmperorLightningTimer = 0f;
				iOwner.mIdleTimer = 0f;
				this.mLevelAnimation = false;
			}

			// Token: 0x060027B1 RID: 10161 RVA: 0x001225B0 File Offset: 0x001207B0
			public void OnUpdate(float iDeltaTime, Machine iOwner)
			{
				iOwner.mIdleTimer += iDeltaTime;
				iOwner.mEmperorLightningTimer += iDeltaTime;
				Matrix mWarlockEmperorOrientation = iOwner.mWarlockEmperorOrientation;
				mWarlockEmperorOrientation.Translation = default(Vector3);
				Vector3 translation = iOwner.mWarlockEmperorOrientation.Translation;
				Segment iSeg = default(Segment);
				iSeg.Origin = translation;
				iSeg.Delta.Y = iSeg.Delta.Y - 2f;
				float num;
				Vector3 vector;
				iOwner.mPlayState.Level.CurrentScene.SegmentIntersect(out num, out translation, out vector, iSeg);
				translation.Y -= iOwner.mWarlock.HeightOffset;
				iOwner.mWarlock.Body.MoveTo(translation, mWarlockEmperorOrientation);
				if (iOwner.mEmperorLightningTimer > 5.5f)
				{
					if (!this.mLevelAnimation && iOwner.mEmperorLightningTimer > 9f)
					{
						AnimatedLevelPart animatedLevelPart = iOwner.mPlayState.Level.CurrentScene.LevelModel.GetAnimatedLevelPart("hatch".GetHashCodeCustom());
						animatedLevelPart.Play(true, -1f, -1f, 1f, false, false);
						this.mLevelAnimation = true;
					}
					if (iOwner.mIdleTimer > 0.25f)
					{
						iOwner.mIdleTimer -= 0.25f;
						LightningBolt lightning = LightningBolt.GetLightning();
						translation = iOwner.mWarlock.GetRightAttachOrientation().Translation;
						Vector3 translation2 = iOwner.mWarlock.GetHipAttachOrientation().Translation;
						Vector3 iDirection;
						Vector3.Subtract(ref translation, ref translation2, out iDirection);
						iDirection.Normalize();
						iDirection.X *= (float)Machine.RANDOM.NextDouble() * 5f;
						iDirection.Y *= (float)Machine.RANDOM.NextDouble() * 5f;
						iDirection.Z *= (float)Machine.RANDOM.NextDouble() * 5f;
						iDirection.Normalize();
						lightning.InitializeEffect(ref translation, iDirection, Spell.LIGHTNINGCOLOR, false, 1f, 10f, iOwner.mPlayState);
						lightning = LightningBolt.GetLightning();
						translation = iOwner.mWarlock.GetLeftAttachOrientation().Translation;
						Vector3.Subtract(ref translation, ref translation2, out iDirection);
						iDirection.Normalize();
						iDirection.X *= (float)Machine.RANDOM.NextDouble() * 5f;
						iDirection.Y *= (float)Machine.RANDOM.NextDouble() * 5f;
						iDirection.Z *= (float)Machine.RANDOM.NextDouble() * 5f;
						iDirection.Normalize();
						lightning.InitializeEffect(ref translation, iDirection, Spell.LIGHTNINGCOLOR, false, 1f, 10f, iOwner.mPlayState);
					}
					iOwner.mIdleTimer += iDeltaTime;
				}
				if (!iOwner.mKingController.CrossFadeEnabled && iOwner.mKingController.HasFinished)
				{
					iOwner.ChangeStage(Machine.States.PostFinal);
				}
			}

			// Token: 0x060027B2 RID: 10162 RVA: 0x001228A5 File Offset: 0x00120AA5
			public void OnExit(Machine iOwner)
			{
			}

			// Token: 0x04002AFC RID: 11004
			private static Machine.FinalStage mSingelton;

			// Token: 0x04002AFD RID: 11005
			private static volatile object mSingeltonLock = new object();

			// Token: 0x04002AFE RID: 11006
			private SpellSoundVariables mSSV;

			// Token: 0x04002AFF RID: 11007
			private bool mLevelAnimation;
		}

		// Token: 0x02000526 RID: 1318
		protected class PostFinalStage : IBossState<Machine>
		{
			// Token: 0x1700094E RID: 2382
			// (get) Token: 0x060027B4 RID: 10164 RVA: 0x001228B8 File Offset: 0x00120AB8
			public static Machine.PostFinalStage Instance
			{
				get
				{
					if (Machine.PostFinalStage.mSingelton == null)
					{
						lock (Machine.PostFinalStage.mSingeltonLock)
						{
							if (Machine.PostFinalStage.mSingelton == null)
							{
								Machine.PostFinalStage.mSingelton = new Machine.PostFinalStage();
							}
						}
					}
					return Machine.PostFinalStage.mSingelton;
				}
			}

			// Token: 0x060027B5 RID: 10165 RVA: 0x0012290C File Offset: 0x00120B0C
			private PostFinalStage()
			{
			}

			// Token: 0x060027B6 RID: 10166 RVA: 0x00122914 File Offset: 0x00120B14
			public void OnEnter(Machine iOwner)
			{
				iOwner.mKingController.CrossFade(iOwner.mKingClips[1], 0.1f, false);
				iOwner.mWarlock.Body.MoveTo(new Vector3(0f, -101f, 0f), Matrix.Identity);
				iOwner.mWarlock.CannotDieWithoutExplicitKill = false;
				iOwner.mWarlock.SetImmortalTime(0f);
				iOwner.mWarlock.Terminate(true, false);
				iOwner.mPlayState.Level.CurrentScene.ExecuteTrigger(Machine.GUNTER_DEAD_TRIGGER_ID, null, false);
			}

			// Token: 0x060027B7 RID: 10167 RVA: 0x001229A8 File Offset: 0x00120BA8
			public void OnUpdate(float iDeltaTime, Machine iOwner)
			{
			}

			// Token: 0x060027B8 RID: 10168 RVA: 0x001229AA File Offset: 0x00120BAA
			public void OnExit(Machine iOwner)
			{
			}

			// Token: 0x04002B00 RID: 11008
			private static Machine.PostFinalStage mSingelton;

			// Token: 0x04002B01 RID: 11009
			private static volatile object mSingeltonLock = new object();
		}

		// Token: 0x02000527 RID: 1319
		protected class KingKilledStage : IBossState<Machine>
		{
			// Token: 0x1700094F RID: 2383
			// (get) Token: 0x060027BA RID: 10170 RVA: 0x001229BC File Offset: 0x00120BBC
			public static Machine.KingKilledStage Instance
			{
				get
				{
					if (Machine.KingKilledStage.mSingelton == null)
					{
						lock (Machine.KingKilledStage.mSingeltonLock)
						{
							if (Machine.KingKilledStage.mSingelton == null)
							{
								Machine.KingKilledStage.mSingelton = new Machine.KingKilledStage();
							}
						}
					}
					return Machine.KingKilledStage.mSingelton;
				}
			}

			// Token: 0x060027BB RID: 10171 RVA: 0x00122A10 File Offset: 0x00120C10
			private KingKilledStage()
			{
			}

			// Token: 0x060027BC RID: 10172 RVA: 0x00122A18 File Offset: 0x00120C18
			public void OnEnter(Machine iOwner)
			{
				iOwner.mIdleTimer = 0f;
				for (int i = 0; i < iOwner.mNPCs.Count; i++)
				{
					while (iOwner.mNPCs[i].AI.CurrentState != AIStateIdle.Instance)
					{
						if (iOwner.mNPCs[i].AI.CurrentState is AIStateAttack)
						{
							iOwner.mNPCs[i].AI.ReleaseTarget();
						}
						iOwner.mNPCs[i].AI.PopState();
					}
					iOwner.mNPCs[i].AI.SetOrder(Order.Idle, ReactTo.None, Order.Idle, 0, 0, 0, null);
				}
				iOwner.mPlayState.Endgame(EndGameCondition.Defeat, true, false, 0f);
				iOwner.OverKillKing();
			}

			// Token: 0x060027BD RID: 10173 RVA: 0x00122AE5 File Offset: 0x00120CE5
			public void OnUpdate(float iDeltaTime, Machine iOwner)
			{
			}

			// Token: 0x060027BE RID: 10174 RVA: 0x00122AE7 File Offset: 0x00120CE7
			public void OnExit(Machine iOwner)
			{
			}

			// Token: 0x04002B02 RID: 11010
			private static Machine.KingKilledStage mSingelton;

			// Token: 0x04002B03 RID: 11011
			private static volatile object mSingeltonLock = new object();
		}

		// Token: 0x02000528 RID: 1320
		private enum MessageType : ushort
		{
			// Token: 0x04002B05 RID: 11013
			Initialize,
			// Token: 0x04002B06 RID: 11014
			Update,
			// Token: 0x04002B07 RID: 11015
			ChangeMachineState,
			// Token: 0x04002B08 RID: 11016
			ChangeFightState,
			// Token: 0x04002B09 RID: 11017
			ScriptMessage,
			// Token: 0x04002B0A RID: 11018
			Teleport
		}

		// Token: 0x02000529 RID: 1321
		internal struct InitializeMessage
		{
			// Token: 0x04002B0B RID: 11019
			public const ushort TYPE = 0;

			// Token: 0x04002B0C RID: 11020
			public ushort Handle;
		}

		// Token: 0x0200052A RID: 1322
		internal struct UpdateMessage
		{
			// Token: 0x04002B0D RID: 11021
			public const ushort TYPE = 1;

			// Token: 0x04002B0E RID: 11022
			public byte Animation;

			// Token: 0x04002B0F RID: 11023
			public float AnimationTime;

			// Token: 0x04002B10 RID: 11024
			public float Hitpoints;
		}

		// Token: 0x0200052B RID: 1323
		internal struct ScriptMessageMessage
		{
			// Token: 0x04002B11 RID: 11025
			public const ushort TYPE = 4;

			// Token: 0x04002B12 RID: 11026
			public BossMessages Message;
		}

		// Token: 0x0200052C RID: 1324
		internal struct ChangeMachineStateMessage
		{
			// Token: 0x04002B13 RID: 11027
			public const ushort TYPE = 2;

			// Token: 0x04002B14 RID: 11028
			public Machine.States NewState;
		}

		// Token: 0x0200052D RID: 1325
		internal struct ChangeFightStateMessage
		{
			// Token: 0x04002B15 RID: 11029
			public const ushort TYPE = 3;

			// Token: 0x04002B16 RID: 11030
			public Machine.States NewState;
		}

		// Token: 0x0200052E RID: 1326
		internal struct TeleportWarlockMessage
		{
			// Token: 0x04002B17 RID: 11031
			public const ushort TYPE = 5;

			// Token: 0x04002B18 RID: 11032
			public Vector3 Position;

			// Token: 0x04002B19 RID: 11033
			public Vector3 Direction;
		}

		// Token: 0x0200052F RID: 1327
		public enum States
		{
			// Token: 0x04002B1B RID: 11035
			Idle,
			// Token: 0x04002B1C RID: 11036
			Drill,
			// Token: 0x04002B1D RID: 11037
			Broken,
			// Token: 0x04002B1E RID: 11038
			KillKing,
			// Token: 0x04002B1F RID: 11039
			Intro,
			// Token: 0x04002B20 RID: 11040
			PreGunter,
			// Token: 0x04002B21 RID: 11041
			Warlock,
			// Token: 0x04002B22 RID: 11042
			Final,
			// Token: 0x04002B23 RID: 11043
			PostFinal,
			// Token: 0x04002B24 RID: 11044
			KingKilled
		}

		// Token: 0x02000530 RID: 1328
		private enum KingAnimations
		{
			// Token: 0x04002B26 RID: 11046
			Idle,
			// Token: 0x04002B27 RID: 11047
			Dialog,
			// Token: 0x04002B28 RID: 11048
			DialogDawn,
			// Token: 0x04002B29 RID: 11049
			Struggle,
			// Token: 0x04002B2A RID: 11050
			Vader,
			// Token: 0x04002B2B RID: 11051
			NrOfAnimations
		}

		// Token: 0x02000531 RID: 1329
		private enum MachineAnimations
		{
			// Token: 0x04002B2D RID: 11053
			Drill,
			// Token: 0x04002B2E RID: 11054
			Idle,
			// Token: 0x04002B2F RID: 11055
			Break,
			// Token: 0x04002B30 RID: 11056
			NrOfAnimations
		}

		// Token: 0x02000532 RID: 1330
		private enum OrcAnimations
		{
			// Token: 0x04002B32 RID: 11058
			Peddle,
			// Token: 0x04002B33 RID: 11059
			BreakFree,
			// Token: 0x04002B34 RID: 11060
			Nod,
			// Token: 0x04002B35 RID: 11061
			Idle,
			// Token: 0x04002B36 RID: 11062
			NrOfAnimations
		}

		// Token: 0x02000533 RID: 1331
		public enum Sharpnel
		{
			// Token: 0x04002B38 RID: 11064
			Cog,
			// Token: 0x04002B39 RID: 11065
			Plank,
			// Token: 0x04002B3A RID: 11066
			Misc,
			// Token: 0x04002B3B RID: 11067
			NrOfSharpnel
		}
	}
}
