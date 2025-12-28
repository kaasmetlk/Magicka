using System;
using System.Collections.Generic;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using Magicka.AI;
using Magicka.Audio;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Levels.Triggers;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using PolygonHead;
using PolygonHead.Effects;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

namespace Magicka.GameLogic.Entities.Bosses
{
	// Token: 0x020001D2 RID: 466
	public class Parker : BossStatusEffected, IBoss
	{
		// Token: 0x06000FDB RID: 4059 RVA: 0x0006232C File Offset: 0x0006052C
		public Parker(PlayState iPlayState)
		{
			this.mPlayState = iPlayState;
			this.mHitList = new HitList(16);
			this.mStates = new Parker.IParkerState[18];
			this.mStates[0] = new Parker.IntroState();
			this.mStates[3] = new Parker.EnterCaveState();
			this.mStates[2] = new Parker.LeaveCaveState();
			this.mStates[12] = new Parker.DefendState();
			this.mStates[1] = new Parker.BattleState();
			this.mStates[10] = new Parker.SpawnState();
			this.mStates[4] = new Parker.SwipeState();
			this.mStates[8] = new Parker.StunState();
			this.mStates[11] = new Parker.CallMinionState();
			this.mStates[5] = new Parker.BiteState();
			this.mStates[6] = new Parker.RushState();
			this.mStates[7] = new Parker.BackupState();
			this.mStates[9] = new Parker.WebState();
			this.mStates[16] = new Parker.CaveState();
			this.mStates[17] = new Parker.DeadState();
			this.mStates[13] = new Parker.RotateState();
			this.mStates[15] = new Parker.MoveToCaveState();
			this.mStates[14] = new Parker.MoveToBattleState();
			this.mStageStates = new IBossState<Parker>[4];
			this.mStageStates[0] = new Parker.IntroStage();
			this.mStageStates[1] = new Parker.BattleStage();
			this.mStageStates[2] = new Parker.CriticalStage();
			this.mStageStates[3] = new Parker.FinalStage();
			this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/spider_baby");
			this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/spider_forest");
			this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/spider_poison");
			SkinnedModel skinnedModel;
			SkinnedModel skinnedModel2;
			lock (Game.Instance.GraphicsDevice)
			{
				skinnedModel = iPlayState.Content.Load<SkinnedModel>("Models/Bosses/Parker/Parker");
				skinnedModel2 = iPlayState.Content.Load<SkinnedModel>("Models/Bosses/Parker/Parker_animation");
			}
			this.mPlayersDead = false;
			this.mAnimationController = new AnimationController();
			this.mAnimationController.AnimationLooped += this.AnimationLooped;
			this.mAnimationController.CrossfadeFinished += this.AnimationLooped;
			this.mAnimationController.Skeleton = skinnedModel2.SkeletonBones;
			this.mAnimationClips = new AnimationClip[23];
			this.mAnimationClips[0] = skinnedModel2.AnimationClips["walk_backward"];
			this.mAnimationClips[1] = skinnedModel2.AnimationClips["walk_forward"];
			this.mAnimationClips[2] = skinnedModel2.AnimationClips["walk_left"];
			this.mAnimationClips[3] = skinnedModel2.AnimationClips["walk_right"];
			this.mAnimationClips[4] = skinnedModel2.AnimationClips["idle"];
			this.mAnimationClips[5] = skinnedModel2.AnimationClips["rotate_left"];
			this.mAnimationClips[6] = skinnedModel2.AnimationClips["rotate_right"];
			this.mAnimationClips[7] = skinnedModel2.AnimationClips["attack_left"];
			this.mAnimationClips[8] = skinnedModel2.AnimationClips["attack_right"];
			this.mAnimationClips[9] = skinnedModel2.AnimationClips["attack_front"];
			this.mAnimationClips[10] = skinnedModel2.AnimationClips["bite"];
			this.mAnimationClips[11] = skinnedModel2.AnimationClips["web"];
			this.mAnimationClips[12] = skinnedModel2.AnimationClips["spawn"];
			this.mAnimationClips[13] = skinnedModel2.AnimationClips["defend"];
			this.mAnimationClips[14] = skinnedModel2.AnimationClips["stun"];
			this.mAnimationClips[15] = skinnedModel2.AnimationClips["cave_enter"];
			this.mAnimationClips[16] = skinnedModel2.AnimationClips["cave_help"];
			this.mAnimationClips[17] = skinnedModel2.AnimationClips["cave_idle"];
			this.mAnimationClips[18] = skinnedModel2.AnimationClips["cave_taunt"];
			this.mAnimationClips[19] = skinnedModel2.AnimationClips["cave_exit"];
			this.mAnimationClips[20] = skinnedModel2.AnimationClips["intro"];
			this.mAnimationClips[21] = skinnedModel2.AnimationClips["taunt"];
			this.mAnimationClips[22] = skinnedModel2.AnimationClips["die"];
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			Matrix matrix;
			Matrix.CreateRotationY(3.1415927f, out matrix);
			Matrix matrix2 = default(Matrix);
			for (int i = 0; i < skinnedModel.SkeletonBones.Count; i++)
			{
				SkinnedModelBone skinnedModelBone = skinnedModel2.SkeletonBones[i];
				for (int j = 0; j < 16; j++)
				{
					string text = string.Format("LeftLeg{0}", j / 4);
					switch (j % 4)
					{
					case 0:
						text += "0";
						break;
					case 1:
						text += "1";
						break;
					case 2:
						text += "3";
						break;
					case 3:
						text += "5";
						break;
					}
					if (skinnedModelBone.Name.Equals(text, StringComparison.OrdinalIgnoreCase))
					{
						this.mLeftIndices[j] = (int)skinnedModelBone.Index;
					}
					else
					{
						text = string.Format("RightLeg{0}", j / 4);
						switch (j % 4)
						{
						case 0:
							text += "0";
							break;
						case 1:
							text += "1";
							break;
						case 2:
							text += "3";
							break;
						case 3:
							text += "5";
							break;
						}
						if (skinnedModelBone.Name.Equals(text, StringComparison.OrdinalIgnoreCase))
						{
							this.mRightIndices[j] = (int)skinnedModelBone.Index;
						}
					}
				}
				if (skinnedModelBone.Name.Equals("Mouth", StringComparison.OrdinalIgnoreCase))
				{
					this.mMouthJointIndex = (int)skinnedModelBone.Index;
					matrix2 = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref matrix2, ref matrix, out matrix2);
					Matrix.Invert(ref matrix2, out this.mMouthBindPose);
				}
				else if (skinnedModelBone.Name.Equals("Spine1", StringComparison.OrdinalIgnoreCase))
				{
					this.mAbdomJointIndex = (int)skinnedModelBone.Index;
					matrix2 = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref matrix2, ref matrix, out matrix2);
					Matrix.Invert(ref matrix2, out this.mAbdomBindPose);
				}
				else if (skinnedModelBone.Name.Equals("LeftLeg05", StringComparison.OrdinalIgnoreCase) || skinnedModelBone.Name.Equals("LeftLeg15", StringComparison.OrdinalIgnoreCase) || skinnedModelBone.Name.Equals("LeftLeg25", StringComparison.OrdinalIgnoreCase) || skinnedModelBone.Name.Equals("LeftLeg35", StringComparison.OrdinalIgnoreCase) || skinnedModelBone.Name.Equals("RightLeg05", StringComparison.OrdinalIgnoreCase) || skinnedModelBone.Name.Equals("RightLeg15", StringComparison.OrdinalIgnoreCase) || skinnedModelBone.Name.Equals("RightLeg25", StringComparison.OrdinalIgnoreCase) || skinnedModelBone.Name.Equals("RightLeg35", StringComparison.OrdinalIgnoreCase))
				{
					this.mFootJointIndex[num] = (int)skinnedModelBone.Index;
					matrix2 = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref matrix2, ref matrix, out matrix2);
					Matrix.Invert(ref matrix2, out this.mFootBindPose[num]);
					num++;
				}
				else if (skinnedModelBone.Name.Equals("LeftLeg01", StringComparison.OrdinalIgnoreCase) || skinnedModelBone.Name.Equals("LeftLeg11", StringComparison.OrdinalIgnoreCase) || skinnedModelBone.Name.Equals("LeftLeg21", StringComparison.OrdinalIgnoreCase) || skinnedModelBone.Name.Equals("LeftLeg31", StringComparison.OrdinalIgnoreCase) || skinnedModelBone.Name.Equals("RightLeg01", StringComparison.OrdinalIgnoreCase) || skinnedModelBone.Name.Equals("RightLeg11", StringComparison.OrdinalIgnoreCase) || skinnedModelBone.Name.Equals("RightLeg21", StringComparison.OrdinalIgnoreCase) || skinnedModelBone.Name.Equals("RightLeg31", StringComparison.OrdinalIgnoreCase))
				{
					this.mKneeJointIndex[num2] = (int)skinnedModelBone.Index;
					matrix2 = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref matrix2, ref matrix, out matrix2);
					Matrix.Invert(ref matrix2, out this.mKneeBindPose[num2]);
					num2++;
				}
				else if (skinnedModelBone.Name.Equals("LeftLeg00", StringComparison.OrdinalIgnoreCase) || skinnedModelBone.Name.Equals("LeftLeg10", StringComparison.OrdinalIgnoreCase) || skinnedModelBone.Name.Equals("LeftLeg20", StringComparison.OrdinalIgnoreCase) || skinnedModelBone.Name.Equals("LeftLeg30", StringComparison.OrdinalIgnoreCase) || skinnedModelBone.Name.Equals("RightLeg00", StringComparison.OrdinalIgnoreCase) || skinnedModelBone.Name.Equals("RightLeg10", StringComparison.OrdinalIgnoreCase) || skinnedModelBone.Name.Equals("RightLeg20", StringComparison.OrdinalIgnoreCase) || skinnedModelBone.Name.Equals("RightLeg30", StringComparison.OrdinalIgnoreCase))
				{
					this.mBodyJointIndex[num3] = (int)skinnedModelBone.Index;
					matrix2 = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref matrix2, ref matrix, out matrix2);
					Matrix.Invert(ref matrix2, out this.mBodyBindPose[num3]);
					num3++;
				}
				else if (skinnedModelBone.Name.Equals("Ass", StringComparison.OrdinalIgnoreCase))
				{
					this.mBackAbdomJointIndex = (int)skinnedModelBone.Index;
					matrix2 = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref matrix2, ref matrix, out matrix2);
					Matrix.Invert(ref matrix2, out this.mBackAbdomBindPose);
				}
			}
			Primitive[] array = new Primitive[16];
			for (int k = 0; k < 8; k++)
			{
				float length = Vector3.Distance(this.mKneeBindPose[k].Translation, this.mFootBindPose[k].Translation);
				array[k] = new Capsule(default(Vector3), Matrix.Identity, 0.55f, length);
			}
			for (int l = 0; l < 8; l++)
			{
				float length2 = Vector3.Distance(this.mBodyBindPose[l].Translation, this.mKneeBindPose[l].Translation);
				array[8 + l] = new Capsule(default(Vector3), Matrix.Identity, 0.8f, length2);
			}
			this.mLimbZone = new BossCollisionZone(iPlayState, this, array);
			this.mLimbZone.Body.CollisionSkin.callbackFn += this.OnCollision;
			this.mAbdomenZone = new BossDamageZone(iPlayState, this, 0, 2.5f, new Sphere(Vector3.Up, 2.5f));
			this.mAbdomenZone.Body.CollisionSkin.callbackFn += this.OnCollision;
			this.mMouthZone = new BossDamageZone(iPlayState, this, 1, 0.9f, new Sphere(Vector3.Zero, 0.8f));
			this.mMouthZone.Body.CollisionSkin.callbackFn += this.OnMouthCollision;
			this.mHeadRenderData = new Parker.RenderData[3];
			this.mBodyRenderData = new Parker.RenderData[3];
			this.mAssRenderData = new Parker.RenderData[3];
			for (int m = 0; m < 3; m++)
			{
				this.mHeadRenderData[m] = new Parker.RenderData(skinnedModel.Model.Meshes[0]);
				this.mHeadRenderData[m].SetMesh(skinnedModel.Model.Meshes[0].VertexBuffer, skinnedModel.Model.Meshes[0].IndexBuffer, skinnedModel.Model.Meshes[0].MeshParts[0], 0, 3, 4);
				this.mHeadRenderData[m].mBoundingSphere.Radius = 15f;
				this.mBodyRenderData[m] = new Parker.RenderData(skinnedModel.Model.Meshes[1]);
				this.mBodyRenderData[m].SetMesh(skinnedModel.Model.Meshes[1].VertexBuffer, skinnedModel.Model.Meshes[1].IndexBuffer, skinnedModel.Model.Meshes[1].MeshParts[0], 0, 3, 4);
				this.mBodyRenderData[m].mBoundingSphere.Radius = 15f;
				this.mAssRenderData[m] = new Parker.RenderData(skinnedModel.Model.Meshes[2]);
				this.mAssRenderData[m].SetMesh(skinnedModel.Model.Meshes[2].VertexBuffer, skinnedModel.Model.Meshes[2].IndexBuffer, skinnedModel.Model.Meshes[2].MeshParts[0], 0, 3, 4);
				this.mAssRenderData[m].mBoundingSphere.Radius = 15f;
			}
			this.mBodyResistances = new Resistance[this.mResistances.Length];
			int n = 0;
			while (n < this.mBodyResistances.Length)
			{
				this.mBodyResistances[n].ResistanceAgainst = Defines.ElementFromIndex(n);
				this.mBodyResistances[n].Modifier = 0f;
				Elements resistanceAgainst = this.mBodyResistances[n].ResistanceAgainst;
				if (resistanceAgainst <= Elements.Arcane)
				{
					if (resistanceAgainst <= Elements.Fire)
					{
						switch (resistanceAgainst)
						{
						case Elements.Earth:
							break;
						case Elements.Water:
						case Elements.Earth | Elements.Water:
							goto IL_EB4;
						case Elements.Cold:
							this.mBodyResistances[n].Multiplier = 0.5f;
							goto IL_ECB;
						default:
							if (resistanceAgainst != Elements.Fire)
							{
								goto IL_EB4;
							}
							this.mBodyResistances[n].Multiplier = 1.5f;
							goto IL_ECB;
						}
					}
					else
					{
						if (resistanceAgainst == Elements.Lightning)
						{
							goto IL_EB4;
						}
						if (resistanceAgainst != Elements.Arcane)
						{
							goto IL_EB4;
						}
					}
					this.mBodyResistances[n].Multiplier = 1.25f;
				}
				else if (resistanceAgainst <= Elements.Shield)
				{
					if (resistanceAgainst != Elements.Life && resistanceAgainst != Elements.Shield)
					{
						goto IL_EB4;
					}
					goto IL_EB4;
				}
				else
				{
					if (resistanceAgainst == Elements.Ice || resistanceAgainst == Elements.Steam)
					{
						goto IL_EB4;
					}
					if (resistanceAgainst != Elements.Poison)
					{
						goto IL_EB4;
					}
					this.mBodyResistances[n].Multiplier = 0f;
				}
				IL_ECB:
				n++;
				continue;
				IL_EB4:
				this.mBodyResistances[n].Multiplier = 1f;
				goto IL_ECB;
			}
			this.mHeadResistances = new Resistance[this.mResistances.Length];
			int num4 = 0;
			while (num4 < this.mHeadResistances.Length)
			{
				this.mHeadResistances[num4].ResistanceAgainst = Defines.ElementFromIndex(num4);
				this.mHeadResistances[num4].Modifier = 0f;
				Elements resistanceAgainst2 = this.mHeadResistances[num4].ResistanceAgainst;
				if (resistanceAgainst2 <= Elements.Arcane)
				{
					if (resistanceAgainst2 <= Elements.Fire)
					{
						switch (resistanceAgainst2)
						{
						case Elements.Earth:
							this.mHeadResistances[num4].Multiplier = 0.5f;
							this.mHeadResistances[num4].Modifier = 40f;
							break;
						case Elements.Water:
							goto IL_104B;
						case Elements.Earth | Elements.Water:
							break;
						case Elements.Cold:
							this.mHeadResistances[num4].Multiplier = 0.5f;
							break;
						default:
							if (resistanceAgainst2 == Elements.Fire)
							{
								this.mHeadResistances[num4].Multiplier = 1.5f;
							}
							break;
						}
					}
					else if (resistanceAgainst2 == Elements.Lightning || resistanceAgainst2 == Elements.Arcane)
					{
						goto IL_104B;
					}
				}
				else if (resistanceAgainst2 <= Elements.Shield)
				{
					if (resistanceAgainst2 == Elements.Life || resistanceAgainst2 == Elements.Shield)
					{
						goto IL_104B;
					}
				}
				else
				{
					if (resistanceAgainst2 == Elements.Ice || resistanceAgainst2 == Elements.Steam)
					{
						goto IL_104B;
					}
					if (resistanceAgainst2 == Elements.Poison)
					{
						this.mHeadResistances[num4].Multiplier = 0f;
					}
				}
				IL_1062:
				num4++;
				continue;
				IL_104B:
				this.mHeadResistances[num4].Multiplier = 1f;
				goto IL_1062;
			}
		}

		// Token: 0x06000FDC RID: 4060 RVA: 0x000633C0 File Offset: 0x000615C0
		private void AnimationLooped()
		{
			this.mCurrentFootstepIndex = -1;
		}

		// Token: 0x06000FDD RID: 4061 RVA: 0x000633C9 File Offset: 0x000615C9
		public void Initialize(ref Matrix iOrientation, int iUniqueID)
		{
			this.Initialize(ref iOrientation);
		}

		// Token: 0x06000FDE RID: 4062 RVA: 0x000633D4 File Offset: 0x000615D4
		public void Initialize(ref Matrix iOrientation)
		{
			this.mDead = false;
			this.mMaxHitPoints = 55000f;
			this.mHitPoints = 55000f;
			this.mTransform = iOrientation;
			this.mPlayState.Level.CurrentScene.GetLocator(Parker.BATTLE_LOCATOR, out this.mBattleTransform);
			this.mPlayState.Level.CurrentScene.GetLocator(Parker.SPAWN_LOCATOR, out this.mSpawnTransform);
			this.mPlayState.Level.CurrentScene.GetLocator(Parker.DEFEND_LOCATOR, out this.mDefendTransform);
			this.mPivotDistance = (this.mDefendTransform.Translation - this.mBattleTransform.Translation).Length();
			this.ResetBattleTransform();
			this.mTransform = this.BattleStateTransform;
			this.mMovement = default(Vector3);
			this.mIsHit = false;
			this.mHitTimer = 0f;
			this.mPlayersDead = false;
			this.mMouthZone.Initialize();
			this.mMouthZone.Body.CollisionSkin.NonCollidables.Add(this.mAbdomenZone.Body.CollisionSkin);
			this.mMouthZone.Body.CollisionSkin.NonCollidables.Add(this.mLimbZone.Body.CollisionSkin);
			this.mMouthZone.Body.CollisionSkin.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
			this.mPlayState.EntityManager.AddEntity(this.mMouthZone);
			this.mAbdomenZone.Initialize();
			this.mAbdomenZone.Body.CollisionSkin.NonCollidables.Add(this.mLimbZone.Body.CollisionSkin);
			this.mAbdomenZone.Body.CollisionSkin.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
			this.mPlayState.EntityManager.AddEntity(this.mAbdomenZone);
			this.mLimbZone.Initialize();
			this.mLimbZone.Body.CollisionSkin.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
			this.mPlayState.EntityManager.AddEntity(this.mLimbZone);
			Matrix matrix = this.mTransform;
			Vector3 translation = matrix.Translation;
			matrix.Translation = default(Vector3);
			this.mAbdomenZone.Body.MoveTo(ref translation, ref matrix);
			this.mAbdomenZone.Update(DataChannel.None, 0f);
			this.mPlayers = Game.Instance.Players;
			this.mHitList.Clear();
			this.mStageSpeedMod = 1f;
			this.mStateSpeedMod = 1f;
			this.mAnimationSpeedMod = 1f;
			this.mMovementSpeedMod = 1f;
			this.mCurrentStage = Parker.Stages.Intro;
			this.mStageStates[(int)this.mCurrentStage].OnEnter(this);
			this.mCurrentState = Parker.States.Intro;
			this.mStates[(int)this.mCurrentState].OnEnter(this);
			for (int i = 0; i < this.mStatusEffects.Length; i++)
			{
				this.mStatusEffects[i].Stop();
				this.mStatusEffects[i] = default(StatusEffect);
			}
			this.mCurrentStatusEffects = StatusEffects.None;
			this.mAnimationController.PlaybackMode = PlaybackMode.Forward;
			this.mAnimationController.Speed = this.mStageSpeedMod;
			this.mTargetWebbed = false;
			this.mCurrentFootstepIndex = -1;
		}

		// Token: 0x06000FDF RID: 4063 RVA: 0x00063760 File Offset: 0x00061960
		protected bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
		{
			if (this.mDead || iSkin1.Owner == null)
			{
				return false;
			}
			if (iSkin0.Owner.Tag is BossCollisionZone)
			{
				return false;
			}
			if (!(iSkin1.Owner.Tag is IDamageable))
			{
				return this.mCurrentState != Parker.States.Bite && this.mCurrentState != Parker.States.Swipe;
			}
			if (iSkin1.Owner.Tag is SprayEntity)
			{
				return false;
			}
			IDamageable damageable = iSkin1.Owner.Tag as IDamageable;
			if (!this.mHitList.Contains(damageable))
			{
				if (damageable is Character)
				{
					if (this.mCurrentState == Parker.States.MoveToCave || this.mCurrentState == Parker.States.Backup || this.mCurrentState == Parker.States.Rotate || this.mCurrentState == Parker.States.EnterCave)
					{
						DamageCollection5 iDamage = default(DamageCollection5);
						iDamage.AddDamage(new Damage(AttackProperties.Damage, Elements.Earth, 100f, 1f));
						iDamage.AddDamage(new Damage(AttackProperties.Knockback, Elements.Earth, damageable.Body.Mass, 1f));
						damageable.Damage(iDamage, this.mAbdomenZone, this.mPlayState.PlayTime, this.mAbdomPosition);
					}
				}
				else
				{
					DamageCollection5 iDamage2 = default(DamageCollection5);
					iDamage2.AddDamage(new Damage(AttackProperties.Damage, Elements.Earth, 100f, 1f));
					damageable.Damage(iDamage2, this.mAbdomenZone, this.mPlayState.PlayTime, this.mAbdomPosition);
				}
				this.mHitList.Add(damageable.Handle, 1f);
			}
			return true;
		}

		// Token: 0x06000FE0 RID: 4064 RVA: 0x000638E0 File Offset: 0x00061AE0
		protected bool OnMouthCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
		{
			if (this.mDead || iSkin1.Owner == null)
			{
				return false;
			}
			if (iSkin1.Owner.Tag is IDamageable && !this.mAnimationController.CrossFadeEnabled)
			{
				if (this.mCurrentState == Parker.States.Bite)
				{
					float num = this.mAnimationController.Time / this.mAnimationController.AnimationClip.Duration;
					if (num > 0.333f && (iSkin1.Owner.Tag is Shield || iSkin1.Owner.Tag is Barrier))
					{
						this.mAnimationController.Time = this.mAnimationController.AnimationClip.Duration - 0.3f;
						(iSkin1.Owner.Tag as IDamageable).Kill();
						return false;
					}
					if (num >= Parker.BITE_TIME[0] && num <= Parker.BITE_TIME[1])
					{
						IDamageable damageable = iSkin1.Owner.Tag as IDamageable;
						if (!this.mHitList.Contains(damageable))
						{
							DamageCollection5 iDamage = default(DamageCollection5);
							iDamage.AddDamage(new Damage(AttackProperties.Damage, Elements.Earth, 2500f, 1f));
							iDamage.AddDamage(new Damage(AttackProperties.Status, Elements.Poison, 100f, 1f));
							damageable.Damage(iDamage, this.mMouthZone, this.mPlayState.PlayTime, this.mTransform.Translation);
							this.mHitList.Add(damageable.Handle);
						}
					}
				}
				else if (this.mCurrentState == Parker.States.Swipe && this.GetSwipState.mSwipeType == Parker.SwipeState.Directions.Front)
				{
					if (iSkin1.Owner.Tag is Shield | iSkin1.Owner.Tag is Barrier)
					{
						(iSkin1.Owner.Tag as IDamageable).Kill();
						return false;
					}
					IDamageable damageable2 = iSkin1.Owner.Tag as IDamageable;
					if (!this.mHitList.Contains(damageable2))
					{
						DamageCollection5 iDamage2 = default(DamageCollection5);
						iDamage2.AddDamage(new Damage(AttackProperties.Knockback, Elements.Earth, damageable2.Body.Mass, 3f));
						iDamage2.AddDamage(new Damage(AttackProperties.Status, Elements.Poison, 100f, 1f));
						damageable2.Damage(iDamage2, this.mMouthZone, this.mPlayState.PlayTime, this.mTransform.Translation);
						this.mHitList.Add(damageable2.Handle);
					}
				}
			}
			return this.mCurrentState != Parker.States.Bite;
		}

		// Token: 0x06000FE1 RID: 4065 RVA: 0x00063B68 File Offset: 0x00061D68
		public void UpdateBoss(DataChannel iDataChannel, float iDeltaTime, bool iFightStarted)
		{
			bool flag = NetworkManager.Instance.State == NetworkState.Client;
			if (this.mHitPoints <= 0f && !flag && this.mCurrentState != Parker.States.Dead)
			{
				this.ChangeState(Parker.States.Dead);
			}
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				this.mNetworkUpdateTimer -= iDeltaTime;
				if (this.mNetworkUpdateTimer <= 0f)
				{
					this.mNetworkUpdateTimer = 0.06666667f;
					this.NetworkUpdate();
				}
			}
			this.UpdateDamage(iDeltaTime);
			this.UpdateStatusEffects(iDeltaTime);
			if (this.mHitTimer > 0f)
			{
				this.mIsHit = false;
			}
			this.mHitTimer -= iDeltaTime;
			this.mHitList.Update(iDeltaTime);
			bool flag2 = true;
			for (int i = 0; i < this.mPlayers.Length; i++)
			{
				if (this.mPlayers[i].Playing && this.mPlayers[i].Avatar != null && !this.mPlayers[i].Avatar.Dead)
				{
					flag2 = false;
				}
			}
			this.mPlayersDead = flag2;
			if (this.mTarget != null)
			{
				this.mPlayerTargetPosition = this.mTarget.Position;
				this.mTargetWebbed = this.mTarget.IsEntangled;
			}
			if (!flag && this.mCurrentState != Parker.States.Dead)
			{
				float num = this.HitPoints / 55000f;
				if (num > 0.95f)
				{
					if (this.mCurrentStage != Parker.Stages.Intro)
					{
						this.ChangeStage(Parker.Stages.Intro);
					}
				}
				else if (num >= 0.5f)
				{
					if (this.mCurrentStage != Parker.Stages.Battle)
					{
						this.ChangeStage(Parker.Stages.Battle);
					}
				}
				else if (num > 0.15f)
				{
					if (this.mCurrentStage != Parker.Stages.Critical)
					{
						this.ChangeStage(Parker.Stages.Critical);
					}
				}
				else if (num > 0f && this.mCurrentStage != Parker.Stages.Final)
				{
					this.ChangeStage(Parker.Stages.Final);
				}
				this.mStageStates[(int)this.mCurrentStage].OnUpdate(iDeltaTime, this);
			}
			this.mStates[(int)this.mCurrentState].OnUpdate(iDeltaTime, this);
			float num2 = 1f;
			if (base.HasStatus(StatusEffects.Cold))
			{
				num2 = this.GetColdSpeed();
			}
			Vector3 vector = this.mTransform.Translation;
			if (flag)
			{
				vector.X += this.mMovement.X * this.mMovementSpeed * iDeltaTime;
				vector.Z += this.mMovement.Z * this.mMovementSpeed * iDeltaTime;
			}
			else
			{
				vector.X += this.mMovement.X * this.mMovementSpeed * this.mStageSpeedMod * this.mMovementSpeedMod * iDeltaTime * this.mStateSpeedMod * num2;
				vector.Z += this.mMovement.Z * this.mMovementSpeed * this.mStageSpeedMod * this.mMovementSpeedMod * iDeltaTime * this.mStateSpeedMod * num2;
			}
			Segment seg = default(Segment);
			seg.Origin = vector;
			seg.Origin.Y = seg.Origin.Y + 1f;
			seg.Delta.Y = seg.Delta.Y - 3f;
			float num3;
			Vector3 vector2;
			Vector3 vector3;
			if (this.mPlayState.Level.CurrentScene.CollisionSkin.SegmentIntersect(out num3, out vector2, out vector3, seg))
			{
				vector = vector2;
			}
			vector.Y += 0.2f;
			this.mTransform.Translation = vector;
			if (this.mCurrentState == Parker.States.Dead)
			{
				this.mAnimationController.Speed = 1f;
			}
			else if (!flag)
			{
				this.mAnimationController.Speed = this.mStateSpeedMod * this.mStageSpeedMod * this.mAnimationSpeedMod * num2;
			}
			this.mAnimationController.Update(iDeltaTime, ref this.mTransform, true);
			Transform transform = default(Transform);
			Vector3 up = Vector3.Up;
			Vector3 zero = Vector3.Zero;
			Vector3 translation;
			Vector3 translation2;
			for (int j = 0; j < 8; j++)
			{
				translation = this.mKneeBindPose[j].Translation;
				Vector3.Transform(ref translation, ref this.mAnimationController.SkinnedBoneTransforms[this.mKneeJointIndex[j]], out translation);
				translation2 = this.mFootBindPose[j].Translation;
				Vector3.Transform(ref translation2, ref this.mAnimationController.SkinnedBoneTransforms[this.mFootJointIndex[j]], out translation2);
				Vector3 translation3 = this.mBodyBindPose[j].Translation;
				Vector3.Transform(ref translation3, ref this.mAnimationController.SkinnedBoneTransforms[this.mBodyJointIndex[j]], out translation3);
				Vector3 vector4;
				Vector3.Subtract(ref translation, ref translation2, out vector4);
				vector4.Normalize();
				Matrix.CreateWorld(ref zero, ref vector4, ref up, out transform.Orientation);
				transform.Position = translation;
				this.mFootTransform[j] = transform.Orientation;
				this.mFootTransform[j].Translation = translation2;
				this.mLimbZone.Body.CollisionSkin.GetPrimitiveLocal(j).SetTransform(ref transform);
				this.mLimbZone.Body.CollisionSkin.GetPrimitiveNewWorld(j).SetTransform(ref transform);
				this.mLimbZone.Body.CollisionSkin.GetPrimitiveOldWorld(j).SetTransform(ref transform);
				Vector3.Subtract(ref translation3, ref translation, out vector4);
				vector4.Normalize();
				Matrix.CreateWorld(ref zero, ref vector4, ref up, out transform.Orientation);
				transform.Position = translation3;
				this.mLimbZone.Body.CollisionSkin.GetPrimitiveLocal(8 + j).SetTransform(ref transform);
				this.mLimbZone.Body.CollisionSkin.GetPrimitiveNewWorld(8 + j).SetTransform(ref transform);
				this.mLimbZone.Body.CollisionSkin.GetPrimitiveOldWorld(8 + j).SetTransform(ref transform);
			}
			this.mLimbZone.Body.CollisionSkin.UpdateWorldBoundingBox();
			translation = this.mAbdomBindPose.Translation;
			Vector3.Transform(ref translation, ref this.mAnimationController.SkinnedBoneTransforms[this.mAbdomJointIndex], out translation);
			translation2 = this.mBackAbdomBindPose.Translation;
			Vector3.Transform(ref translation2, ref this.mAnimationController.SkinnedBoneTransforms[this.mBackAbdomJointIndex], out translation2);
			float y = translation.Y;
			Vector3.Lerp(ref translation, ref translation2, -1f, out translation);
			translation.Y = y;
			this.mAbdomPosition = translation;
			this.mAbdomenZone.SetPosition(ref translation);
			this.mMouthOrientation = this.mMouthBindPose;
			Matrix.Multiply(ref this.mMouthOrientation, ref this.mAnimationController.SkinnedBoneTransforms[this.mMouthJointIndex], out this.mMouthOrientation);
			translation = this.mMouthOrientation.Translation;
			this.mMouthZone.SetPosition(ref translation);
			if (this.mAnimationController.AnimationClip == this.mAnimationClips[1] || this.mAnimationController.AnimationClip == this.mAnimationClips[0] || this.mAnimationController.AnimationClip == this.mAnimationClips[2] || this.mAnimationController.AnimationClip == this.mAnimationClips[3] || this.mAnimationController.CrossFadeAnimationClip == this.mAnimationClips[1] || this.mAnimationController.CrossFadeAnimationClip == this.mAnimationClips[0] || this.mAnimationController.CrossFadeAnimationClip == this.mAnimationClips[2] || this.mAnimationController.CrossFadeAnimationClip == this.mAnimationClips[3])
			{
				float num4 = this.mAnimationController.Time / this.mAnimationController.AnimationClip.Duration;
				for (int k = Parker.FOOTSTEP_TIMES.Length - 1; k >= 0; k--)
				{
					if (num4 >= Parker.FOOTSTEP_TIMES[k] && this.mCurrentFootstepIndex < k)
					{
						this.mCurrentFootstepIndex = k;
						AudioManager.Instance.PlayCue(Banks.Additional, Parker.FOOTSTEP_SOUND, this.mAbdomenZone.AudioEmitter);
					}
				}
			}
			Parker.RenderData renderData = this.mHeadRenderData[(int)iDataChannel];
			Parker.RenderData renderData2 = this.mBodyRenderData[(int)iDataChannel];
			Parker.RenderData renderData3 = this.mAssRenderData[(int)iDataChannel];
			float num5 = this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].Magnitude;
			num5 *= 10f;
			num5 = Math.Min(num5, 1f);
			renderData.mMaterial.Colorize.X = (renderData2.mMaterial.Colorize.X = (renderData3.mMaterial.Colorize.X = Parker.ColdColor.X));
			renderData.mMaterial.Colorize.Y = (renderData2.mMaterial.Colorize.Y = (renderData3.mMaterial.Colorize.Y = Parker.ColdColor.Y));
			renderData.mMaterial.Colorize.Z = (renderData2.mMaterial.Colorize.Z = (renderData3.mMaterial.Colorize.Z = Parker.ColdColor.Z));
			renderData.mMaterial.Colorize.W = (renderData2.mMaterial.Colorize.W = (renderData3.mMaterial.Colorize.W = num5));
			this.mHeadHitFlashTimer = Math.Max(this.mHeadHitFlashTimer - iDeltaTime * 10f, 0f);
			this.mAssHitFlashTimer = Math.Max(this.mAssHitFlashTimer - iDeltaTime * 10f, 0f);
			renderData.mBoundingSphere.Center = this.mTransform.Translation;
			renderData2.mBoundingSphere.Center = this.mTransform.Translation;
			renderData3.mBoundingSphere.Center = this.mTransform.Translation;
			renderData.mDamage = 1f - this.mHitPoints / 55000f;
			renderData2.mDamage = 1f - this.mHitPoints / 55000f;
			renderData3.mDamage = 1f - this.mHitPoints / 55000f;
			renderData.mFlash = this.mHeadHitFlashTimer;
			renderData3.mFlash = this.mAssHitFlashTimer;
			Array.Copy(this.mAnimationController.SkinnedBoneTransforms, renderData.mSkeleton, renderData.mSkeleton.Length);
			this.mPlayState.Scene.AddRenderableObject(iDataChannel, renderData);
			Array.Copy(this.mAnimationController.SkinnedBoneTransforms, renderData2.mSkeleton, renderData2.mSkeleton.Length);
			this.mPlayState.Scene.AddRenderableObject(iDataChannel, renderData2);
			Array.Copy(this.mAnimationController.SkinnedBoneTransforms, renderData3.mSkeleton, renderData3.mSkeleton.Length);
			this.mPlayState.Scene.AddRenderableObject(iDataChannel, renderData3);
		}

		// Token: 0x06000FE2 RID: 4066 RVA: 0x00064600 File Offset: 0x00062800
		protected void ResetBattleTransform()
		{
			this.mPivotTransform = this.mSpawnTransform;
			this.mPivotTransform.Translation = this.mDefendTransform.Translation;
		}

		// Token: 0x17000416 RID: 1046
		// (get) Token: 0x06000FE3 RID: 4067 RVA: 0x00064624 File Offset: 0x00062824
		protected Matrix BattleStateTransform
		{
			get
			{
				Matrix result = this.mPivotTransform;
				Vector3 translation = this.mPivotTransform.Translation;
				Vector3 forward = this.mPivotTransform.Forward;
				Vector3.Multiply(ref forward, this.mPivotDistance, out forward);
				Vector3.Add(ref forward, ref translation, out translation);
				result.Translation = translation;
				return result;
			}
		}

		// Token: 0x06000FE4 RID: 4068 RVA: 0x00064673 File Offset: 0x00062873
		protected float GetColdSpeed()
		{
			return Math.Max(this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].GetSlowdown(), 0.666f);
		}

		// Token: 0x17000417 RID: 1047
		// (get) Token: 0x06000FE5 RID: 4069 RVA: 0x00064695 File Offset: 0x00062895
		protected float ColdMagnitude
		{
			get
			{
				return this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].Magnitude;
			}
		}

		// Token: 0x17000418 RID: 1048
		// (get) Token: 0x06000FE6 RID: 4070 RVA: 0x000646AD File Offset: 0x000628AD
		protected float BurningMagnitude
		{
			get
			{
				return this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)].Magnitude;
			}
		}

		// Token: 0x17000419 RID: 1049
		// (get) Token: 0x06000FE7 RID: 4071 RVA: 0x000646C5 File Offset: 0x000628C5
		protected float BurningDPS
		{
			get
			{
				return this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)].DPS;
			}
		}

		// Token: 0x06000FE8 RID: 4072 RVA: 0x000646DD File Offset: 0x000628DD
		public void DeInitialize()
		{
		}

		// Token: 0x1700041A RID: 1050
		// (get) Token: 0x06000FE9 RID: 4073 RVA: 0x000646DF File Offset: 0x000628DF
		private Parker.SwipeState GetSwipState
		{
			get
			{
				return this.mStates[4] as Parker.SwipeState;
			}
		}

		// Token: 0x1700041B RID: 1051
		// (get) Token: 0x06000FEA RID: 4074 RVA: 0x000646EE File Offset: 0x000628EE
		private Parker.BiteState GetBiteState
		{
			get
			{
				return this.mStates[5] as Parker.BiteState;
			}
		}

		// Token: 0x1700041C RID: 1052
		// (get) Token: 0x06000FEB RID: 4075 RVA: 0x000646FD File Offset: 0x000628FD
		private Parker.RushState GetRushState
		{
			get
			{
				return this.mStates[6] as Parker.RushState;
			}
		}

		// Token: 0x1700041D RID: 1053
		// (get) Token: 0x06000FEC RID: 4076 RVA: 0x0006470C File Offset: 0x0006290C
		private Parker.WebState GetWebState
		{
			get
			{
				return this.mStates[9] as Parker.WebState;
			}
		}

		// Token: 0x1700041E RID: 1054
		// (get) Token: 0x06000FED RID: 4077 RVA: 0x0006471C File Offset: 0x0006291C
		private Parker.SpawnState GetSpawnState
		{
			get
			{
				return this.mStates[10] as Parker.SpawnState;
			}
		}

		// Token: 0x1700041F RID: 1055
		// (get) Token: 0x06000FEE RID: 4078 RVA: 0x0006472C File Offset: 0x0006292C
		private Parker.CallMinionState GetCallMinionState
		{
			get
			{
				return this.mStates[11] as Parker.CallMinionState;
			}
		}

		// Token: 0x17000420 RID: 1056
		// (get) Token: 0x06000FEF RID: 4079 RVA: 0x0006473C File Offset: 0x0006293C
		private Parker.CaveState GetCaveState
		{
			get
			{
				return this.mStates[16] as Parker.CaveState;
			}
		}

		// Token: 0x17000421 RID: 1057
		// (get) Token: 0x06000FF0 RID: 4080 RVA: 0x0006474C File Offset: 0x0006294C
		private Parker.DefendState GetDefendState
		{
			get
			{
				return this.mStates[12] as Parker.DefendState;
			}
		}

		// Token: 0x17000422 RID: 1058
		// (get) Token: 0x06000FF1 RID: 4081 RVA: 0x0006475C File Offset: 0x0006295C
		// (set) Token: 0x06000FF2 RID: 4082 RVA: 0x00064764 File Offset: 0x00062964
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
				if (num > 1E-06f)
				{
					num = (float)Math.Sqrt((double)num);
					float num2 = 1f / num;
					this.mDesiredDirection.X = value.X * num2;
					this.mDesiredDirection.Y = value.Y * num2;
					this.mDesiredDirection.Z = value.Z * num2;
					if (num > 1f)
					{
						this.mMovement.X = value.X * num2;
						this.mMovement.Y = value.Y * num2;
						this.mMovement.Z = value.Z * num2;
					}
				}
			}
		}

		// Token: 0x06000FF3 RID: 4083 RVA: 0x0006482C File Offset: 0x00062A2C
		protected unsafe void ChangeState(Parker.States iState)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					Parker.ChangeStateMessage changeStateMessage;
					changeStateMessage.NewState = iState;
					BossFight.Instance.SendMessage<Parker.ChangeStateMessage>(this, 2, (void*)(&changeStateMessage), true);
				}
				this.mStates[(int)this.mCurrentState].OnExit(this);
				this.mPreviousState = this.mCurrentState;
				this.mCurrentState = iState;
				this.mStates[(int)this.mCurrentState].OnEnter(this);
			}
		}

		// Token: 0x06000FF4 RID: 4084 RVA: 0x000648A4 File Offset: 0x00062AA4
		protected unsafe void ChangeStage(Parker.Stages iStage)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				this.mStageStates[(int)this.mCurrentStage].OnExit(this);
				this.mCurrentStage = iStage;
				this.mStageStates[(int)this.mCurrentStage].OnEnter(this);
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					Parker.ChangeStageMessage changeStageMessage;
					changeStageMessage.NewStage = iStage;
					BossFight.Instance.SendMessage<Parker.ChangeStageMessage>(this, 1, (void*)(&changeStageMessage), true);
				}
			}
		}

		// Token: 0x06000FF5 RID: 4085 RVA: 0x00064910 File Offset: 0x00062B10
		protected bool GetRandomTarget(out Avatar oAvatar)
		{
			oAvatar = null;
			bool flag = false;
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				int num = 0;
				int num2 = 0;
				for (int i = 0; i < this.mPlayers.Length; i++)
				{
					num2 |= 1 << i;
				}
				while (!flag)
				{
					int num3 = Cthulhu.RANDOM.Next(this.mPlayers.Length);
					int num4 = 1 << num3;
					if ((num & num2) == num2)
					{
						return false;
					}
					if ((num & num4) != num4)
					{
						num |= num4;
						if (this.mPlayers[num3].Playing && this.mPlayers[num3].Avatar != null && !this.mPlayers[num3].Avatar.Dead)
						{
							oAvatar = this.mPlayers[num3].Avatar;
							flag = true;
						}
					}
				}
			}
			return flag;
		}

		// Token: 0x17000423 RID: 1059
		// (get) Token: 0x06000FF6 RID: 4086 RVA: 0x000649D1 File Offset: 0x00062BD1
		public override bool Dead
		{
			get
			{
				return this.mDead;
			}
		}

		// Token: 0x17000424 RID: 1060
		// (get) Token: 0x06000FF7 RID: 4087 RVA: 0x000649D9 File Offset: 0x00062BD9
		public float MaxHitPoints
		{
			get
			{
				return this.mMaxHitPoints;
			}
		}

		// Token: 0x17000425 RID: 1061
		// (get) Token: 0x06000FF8 RID: 4088 RVA: 0x000649E1 File Offset: 0x00062BE1
		public float HitPoints
		{
			get
			{
				return this.mHitPoints;
			}
		}

		// Token: 0x06000FF9 RID: 4089 RVA: 0x000649EC File Offset: 0x00062BEC
		internal unsafe void SetTarget(Avatar iAvatar)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					Parker.ChangeTargetMessage changeTargetMessage = default(Parker.ChangeTargetMessage);
					changeTargetMessage.Handle = (int)iAvatar.Handle;
					BossFight.Instance.SendMessage<Parker.ChangeTargetMessage>(this, 3, (void*)(&changeTargetMessage), true);
				}
				if (this.mTarget != null && (this.mTarget.IsEntangled || this.mTarget.HasStatus(StatusEffects.Poisoned)))
				{
					return;
				}
				this.mTarget = iAvatar;
			}
		}

		// Token: 0x06000FFA RID: 4090 RVA: 0x00064A64 File Offset: 0x00062C64
		public bool AddImpulseVelocity(Vector3 iDirection, float iElevation, float iMassPower, float iDistance)
		{
			return false;
		}

		// Token: 0x06000FFB RID: 4091 RVA: 0x00064A67 File Offset: 0x00062C67
		public void SetSlow(int iIndex)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000FFC RID: 4092 RVA: 0x00064A6E File Offset: 0x00062C6E
		public void GetRandomPositionOnCollisionSkin(int iIndex, out Vector3 oPosition)
		{
			oPosition = default(Vector3);
		}

		// Token: 0x06000FFD RID: 4093 RVA: 0x00064A77 File Offset: 0x00062C77
		public bool HasStatus(int iIndex, StatusEffects iStatus)
		{
			return base.HasStatus(iStatus);
		}

		// Token: 0x06000FFE RID: 4094 RVA: 0x00064A80 File Offset: 0x00062C80
		public float StatusMagnitude(int iIndex, StatusEffects iStatus)
		{
			return base.StatusMagnitude(iStatus);
		}

		// Token: 0x06000FFF RID: 4095 RVA: 0x00064A8C File Offset: 0x00062C8C
		public DamageResult Damage(int iPartIndex, Damage iDamage, Entity iAttacker, ref Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			DamageResult result = DamageResult.None;
			if (this.mCurrentState != Parker.States.Defend && this.mCurrentState != Parker.States.CallMinions && this.mCurrentState != Parker.States.Cave)
			{
				if (iPartIndex == 0)
				{
					this.mResistances = this.mBodyResistances;
				}
				else if (iPartIndex == 1)
				{
					this.mResistances = this.mHeadResistances;
				}
				result = this.Damage(iDamage, iAttacker, iAttackPosition, iFeatures);
			}
			return result;
		}

		// Token: 0x06001000 RID: 4096 RVA: 0x00064AF0 File Offset: 0x00062CF0
		public void Damage(int iPartIndex, float iDamage, Elements iElement)
		{
			if (iPartIndex == 0)
			{
				this.mResistances = this.mBodyResistances;
				base.Damage(iDamage, iElement);
				return;
			}
			if (iPartIndex != 1)
			{
				throw new Exception("Parker damaged on index:" + iPartIndex + "!");
			}
			if (this.mCurrentState != Parker.States.Defend)
			{
				this.mResistances = this.mHeadResistances;
				base.Damage(iDamage, iElement);
				return;
			}
		}

		// Token: 0x17000426 RID: 1062
		// (get) Token: 0x06001001 RID: 4097 RVA: 0x00064B53 File Offset: 0x00062D53
		protected override int BloodEffect
		{
			get
			{
				return Gib.GORE_GIB_MEDIUM_EFFECTS[4];
			}
		}

		// Token: 0x17000427 RID: 1063
		// (get) Token: 0x06001002 RID: 4098 RVA: 0x00064B5C File Offset: 0x00062D5C
		protected override BossDamageZone Entity
		{
			get
			{
				return this.mAbdomenZone;
			}
		}

		// Token: 0x17000428 RID: 1064
		// (get) Token: 0x06001003 RID: 4099 RVA: 0x00064B64 File Offset: 0x00062D64
		protected override float Radius
		{
			get
			{
				return (this.mAbdomenZone.Body.CollisionSkin.GetPrimitiveLocal(0) as Sphere).Radius;
			}
		}

		// Token: 0x17000429 RID: 1065
		// (get) Token: 0x06001004 RID: 4100 RVA: 0x00064B86 File Offset: 0x00062D86
		protected override float Length
		{
			get
			{
				return (this.mAbdomenZone.Body.CollisionSkin.GetPrimitiveLocal(0) as Sphere).Radius;
			}
		}

		// Token: 0x1700042A RID: 1066
		// (get) Token: 0x06001005 RID: 4101 RVA: 0x00064BA8 File Offset: 0x00062DA8
		protected override Vector3 NotifierTextPostion
		{
			get
			{
				Vector3 translation = this.mMouthOrientation.Translation;
				translation.Y += 3f;
				return translation;
			}
		}

		// Token: 0x06001006 RID: 4102 RVA: 0x00064BD5 File Offset: 0x00062DD5
		public void ScriptMessage(BossMessages iMessage)
		{
		}

		// Token: 0x06001007 RID: 4103 RVA: 0x00064BD8 File Offset: 0x00062DD8
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

		// Token: 0x06001008 RID: 4104 RVA: 0x00064C94 File Offset: 0x00062E94
		private unsafe void NetworkUpdate()
		{
			NetworkServer networkServer = NetworkManager.Instance.Interface as NetworkServer;
			Parker.UpdateMessage updateMessage = default(Parker.UpdateMessage);
			updateMessage.Animation = 0;
			while ((int)updateMessage.Animation < this.mAnimationClips.Length && this.mAnimationController.AnimationClip != this.mAnimationClips[(int)updateMessage.Animation])
			{
				updateMessage.Animation += 1;
			}
			updateMessage.Hitpoints = (ushort)Math.Max(Math.Min(this.mHitPoints, 65535f), 0f);
			updateMessage.AnimationTime = new HalfSingle(this.mAnimationController.Time);
			updateMessage.AnimationSpeed = new HalfSingle(this.mAnimationController.Speed);
			float num = 1f;
			if (base.HasStatus(StatusEffects.Cold))
			{
				num = this.GetColdSpeed();
			}
			updateMessage.Speed = new HalfSingle(this.mMovementSpeed * this.mStageSpeedMod * this.mMovementSpeedMod * this.mStateSpeedMod * num);
			updateMessage.PositionUpdate = (this.mCurrentState == Parker.States.Rush || this.mCurrentState == Parker.States.Backup || this.mCurrentState == Parker.States.MoveToBattle || this.mCurrentState == Parker.States.MoveToCave);
			updateMessage.Position = this.mTransform.Translation;
			for (int i = 0; i < networkServer.Connections; i++)
			{
				float num2 = networkServer.GetLatency(i) * 0.5f;
				Parker.UpdateMessage updateMessage2 = updateMessage;
				updateMessage2.AnimationTime = new HalfSingle(this.mAnimationController.Time + num2);
				updateMessage2.Position.X = updateMessage2.Position.X + this.mMovement.X * this.mMovementSpeed * this.mStageSpeedMod * this.mMovementSpeedMod * num2 * this.mStateSpeedMod * num;
				updateMessage2.Position.Z = updateMessage2.Position.Z + this.mMovement.Z * this.mMovementSpeed * this.mStageSpeedMod * this.mMovementSpeedMod * num2 * this.mStateSpeedMod * num;
				BossFight.Instance.SendMessage<Parker.UpdateMessage>(this, 0, (void*)(&updateMessage2), false, i);
			}
		}

		// Token: 0x06001009 RID: 4105 RVA: 0x00064EA4 File Offset: 0x000630A4
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
				Parker.UpdateMessage updateMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&updateMessage));
				if (this.mAnimationController.AnimationClip != this.mAnimationClips[(int)updateMessage.Animation])
				{
					this.mAnimationController.StartClip(this.mAnimationClips[(int)updateMessage.Animation], false);
				}
				this.mAnimationController.Time = updateMessage.AnimationTime.ToSingle();
				this.mHitPoints = (float)updateMessage.Hitpoints;
				this.mAnimationController.Speed = updateMessage.AnimationSpeed.ToSingle();
				this.mMovementSpeed = updateMessage.Speed.ToSingle();
				if (updateMessage.PositionUpdate)
				{
					this.mTransform.Translation = updateMessage.Position;
					return;
				}
				break;
			}
			case 1:
			{
				Parker.ChangeStageMessage changeStageMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&changeStageMessage));
				if (changeStageMessage.NewStage != Parker.Stages.NrOfStages && changeStageMessage.NewStage != this.mCurrentStage)
				{
					this.mStageStates[(int)this.mCurrentStage].OnExit(this);
					this.mCurrentStage = changeStageMessage.NewStage;
					this.mStageStates[(int)this.mCurrentStage].OnEnter(this);
					return;
				}
				break;
			}
			case 2:
			{
				Parker.ChangeStateMessage changeStateMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&changeStateMessage));
				this.mStates[(int)this.mCurrentState].OnExit(this);
				this.mPreviousState = this.mCurrentState;
				this.mCurrentState = changeStateMessage.NewState;
				this.mStates[(int)this.mCurrentState].OnEnter(this);
				return;
			}
			case 3:
			{
				Parker.ChangeTargetMessage changeTargetMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&changeTargetMessage));
				this.mTarget = (Magicka.GameLogic.Entities.Entity.GetFromHandle(changeTargetMessage.Handle) as Avatar);
				return;
			}
			case 4:
			{
				Parker.WebMessage webMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&webMessage));
				SprayEntity specificInstance = SprayEntity.GetSpecificInstance(webMessage.Handle);
				specificInstance.Initialize(webMessage.MouthIsOwner ? this.mMouthZone : null, webMessage.Position, webMessage.Direction.ToVector3(), webMessage.Velocity.ToSingle());
				AudioManager.Instance.PlayCue(Banks.Characters, Parker.WebState.WEB_SOUND, this.mMouthZone.AudioEmitter);
				if (webMessage.Parent != 0)
				{
					SprayEntity.GetSpecificInstance(webMessage.Parent).Child = specificInstance;
				}
				this.mPlayState.EntityManager.AddEntity(specificInstance);
				return;
			}
			case 5:
			{
				Parker.RotationTransitionMessage rotationTransitionMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&rotationTransitionMessage));
				this.mRotationTransitionState = rotationTransitionMessage.State;
				this.mTargetRotateDirection = rotationTransitionMessage.Direction.ToVector3();
				break;
			}
			default:
				return;
			}
		}

		// Token: 0x0600100A RID: 4106 RVA: 0x0006512A File Offset: 0x0006332A
		public BossEnum GetBossType()
		{
			return BossEnum.Parker;
		}

		// Token: 0x0600100B RID: 4107 RVA: 0x0006512E File Offset: 0x0006332E
		public void NetworkInitialize(ref BossInitializeMessage iMsg)
		{
			throw new NotImplementedException();
		}

		// Token: 0x1700042B RID: 1067
		// (get) Token: 0x0600100C RID: 4108 RVA: 0x00065135 File Offset: 0x00063335
		public bool NetworkInitialized
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600100D RID: 4109 RVA: 0x00065138 File Offset: 0x00063338
		private static bool CheckBehind(Parker iOwner)
		{
			Vector3 translation = iOwner.mTransform.Translation;
			Vector3 forward = iOwner.mTransform.Forward;
			for (int i = 0; i < iOwner.mPlayers.Length; i++)
			{
				if (iOwner.mPlayers[i].Playing && iOwner.mPlayers[i].Avatar != null && !iOwner.mPlayers[i].Avatar.Dead)
				{
					Vector3 position = iOwner.mPlayers[i].Avatar.Position;
					Vector3 vector;
					Vector3.Subtract(ref position, ref translation, out vector);
					vector.Y = 0f;
					vector.Normalize();
					float num;
					Vector3.Dot(ref vector, ref forward, out num);
					if (num < -0.85f)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600100E RID: 4110 RVA: 0x000651F0 File Offset: 0x000633F0
		private static float GetDistanceSquared(Parker iOwner, Character iTarget)
		{
			Vector2 vector = new Vector2(iOwner.mTransform.Translation.X, iOwner.mTransform.Translation.Z);
			Vector2 vector2 = new Vector2(iTarget.Position.X, iTarget.Position.Z);
			float result;
			Vector2.DistanceSquared(ref vector, ref vector2, out result);
			return result;
		}

		// Token: 0x0600100F RID: 4111 RVA: 0x00065258 File Offset: 0x00063458
		private static void GetAngle(ref Vector3 iPosition, ref Vector3 iForward, ref Vector3 iTarget, out float oAngle)
		{
			Vector3 vector;
			Vector3.Subtract(ref iTarget, ref iPosition, out vector);
			vector.Y = 0f;
			vector.Normalize();
			Parker.GetAngle(ref iForward, ref vector, out oAngle);
		}

		// Token: 0x06001010 RID: 4112 RVA: 0x0006528C File Offset: 0x0006348C
		private static void GetAngle(ref Vector3 iForward, ref Vector3 iTargetDirection, out float oAngle)
		{
			Vector3 vector;
			Vector3.Cross(ref iTargetDirection, ref iForward, out vector);
			float num = (float)Math.Acos((double)vector.Y);
			num -= 1.5707964f;
			oAngle = num;
		}

		// Token: 0x06001011 RID: 4113 RVA: 0x000652BC File Offset: 0x000634BC
		private static void GetConstrainedAngle(ref Vector3 iPosition, ref Vector3 iForward, ref Vector3 iTarget, out float oAngle, float iMinAngle, float iMaxAngle)
		{
			Vector3 vector;
			Vector3.Subtract(ref iTarget, ref iPosition, out vector);
			vector.Y = 0f;
			vector.Normalize();
			Parker.GetConstrainedAngle(ref iForward, ref vector, out oAngle, iMinAngle, iMaxAngle);
		}

		// Token: 0x06001012 RID: 4114 RVA: 0x000652F4 File Offset: 0x000634F4
		private static void GetConstrainedAngle(ref Vector3 iForward, ref Vector3 iTargetDirection, out float oAngle, float iMinAngle, float iMaxAngle)
		{
			Vector3 vector = new Vector3(0f, 0f, 1f);
			MagickaMath.ConstrainVector(ref iTargetDirection, ref vector, iMinAngle, iMaxAngle);
			Parker.GetAngle(ref iForward, ref iTargetDirection, out oAngle);
		}

		// Token: 0x06001013 RID: 4115 RVA: 0x00065330 File Offset: 0x00063530
		private unsafe static void TransitionToCave(Parker iOwner)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				Vector3 translation = iOwner.mTransform.Translation;
				Vector3 translation2 = iOwner.mSpawnTransform.Translation;
				Vector3.Subtract(ref translation, ref translation2, out iOwner.mTargetRotateDirection);
				iOwner.mTargetRotateDirection.Y = 0f;
				iOwner.mTargetRotateDirection.Normalize();
				iOwner.mRotationTransitionState = Parker.States.MoveToCave;
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					Parker.RotationTransitionMessage rotationTransitionMessage = default(Parker.RotationTransitionMessage);
					rotationTransitionMessage.Direction = new Normalized101010(iOwner.mTargetRotateDirection);
					rotationTransitionMessage.State = iOwner.mRotationTransitionState;
					BossFight.Instance.SendMessage<Parker.RotationTransitionMessage>(iOwner, 5, (void*)(&rotationTransitionMessage), true);
				}
				iOwner.ChangeState(Parker.States.Rotate);
			}
		}

		// Token: 0x04000E48 RID: 3656
		private const float NETWORK_UPDATE_PERIOD = 0.06666667f;

		// Token: 0x04000E49 RID: 3657
		private const float MAXHITPOINTS = 55000f;

		// Token: 0x04000E4A RID: 3658
		private const float HIT_COOLDOWN = 8f;

		// Token: 0x04000E4B RID: 3659
		private static readonly Vector3 ColdColor = new Vector3(1f, 1.6f, 2f);

		// Token: 0x04000E4C RID: 3660
		private float mLastNetworkUpdate;

		// Token: 0x04000E4D RID: 3661
		protected float mNetworkUpdateTimer;

		// Token: 0x04000E4E RID: 3662
		private Parker.IParkerState[] mStates;

		// Token: 0x04000E4F RID: 3663
		private Parker.States mCurrentState;

		// Token: 0x04000E50 RID: 3664
		private Parker.States mPreviousState;

		// Token: 0x04000E51 RID: 3665
		private IBossState<Parker>[] mStageStates;

		// Token: 0x04000E52 RID: 3666
		private Parker.Stages mCurrentStage;

		// Token: 0x04000E53 RID: 3667
		private static readonly float[] WEBBING_TIME = new float[]
		{
			0.2888889f,
			0.8f
		};

		// Token: 0x04000E54 RID: 3668
		private static readonly float[] BITE_TIME = new float[]
		{
			0.5483871f,
			0.9354839f
		};

		// Token: 0x04000E55 RID: 3669
		private static readonly float[] ATTACK_FRONT_TIME = new float[]
		{
			0.2f,
			0.6666667f
		};

		// Token: 0x04000E56 RID: 3670
		private static readonly float[] ATTACK_LEFT_RIGHT_TIME = new float[]
		{
			0.28f,
			0.56f
		};

		// Token: 0x04000E57 RID: 3671
		private static readonly float[] SPAWN_TIME = new float[]
		{
			0.28947368f,
			0.7894737f
		};

		// Token: 0x04000E58 RID: 3672
		private static readonly float[] MINION_TIME = new float[]
		{
			0.28947368f,
			0.7894737f
		};

		// Token: 0x04000E59 RID: 3673
		private static readonly int SPAWN_LOCATOR = "parker_spawn".GetHashCodeCustom();

		// Token: 0x04000E5A RID: 3674
		private static readonly int BATTLE_LOCATOR = "parker_battle".GetHashCodeCustom();

		// Token: 0x04000E5B RID: 3675
		private static readonly int DEFEND_LOCATOR = "parker_defensive".GetHashCodeCustom();

		// Token: 0x04000E5C RID: 3676
		private static Random RANDOM = new Random();

		// Token: 0x04000E5D RID: 3677
		private AnimationClip[] mAnimationClips;

		// Token: 0x04000E5E RID: 3678
		private AnimationController mAnimationController;

		// Token: 0x04000E5F RID: 3679
		private Parker.RenderData[] mHeadRenderData;

		// Token: 0x04000E60 RID: 3680
		private Parker.RenderData[] mBodyRenderData;

		// Token: 0x04000E61 RID: 3681
		private Parker.RenderData[] mAssRenderData;

		// Token: 0x04000E62 RID: 3682
		private bool mDead;

		// Token: 0x04000E63 RID: 3683
		private float mMovementSpeed = 8f;

		// Token: 0x04000E64 RID: 3684
		private Matrix mMouthOrientation;

		// Token: 0x04000E65 RID: 3685
		private Vector3 mAbdomPosition = Vector3.Zero;

		// Token: 0x04000E66 RID: 3686
		private Vector3 mMovement = Vector3.Zero;

		// Token: 0x04000E67 RID: 3687
		private Vector3 mDesiredDirection = Vector3.Forward;

		// Token: 0x04000E68 RID: 3688
		private BossDamageZone mMouthZone;

		// Token: 0x04000E69 RID: 3689
		private BossDamageZone mAbdomenZone;

		// Token: 0x04000E6A RID: 3690
		private BossCollisionZone mLimbZone;

		// Token: 0x04000E6B RID: 3691
		private Vector3 mPlayerTargetPosition;

		// Token: 0x04000E6C RID: 3692
		private Vector3 mTargetRotateDirection;

		// Token: 0x04000E6D RID: 3693
		private Parker.States mRotationTransitionState;

		// Token: 0x04000E6E RID: 3694
		private Matrix mTransform;

		// Token: 0x04000E6F RID: 3695
		private Matrix mDefendTransform;

		// Token: 0x04000E70 RID: 3696
		private Matrix mBattleTransform;

		// Token: 0x04000E71 RID: 3697
		private Matrix mSpawnTransform;

		// Token: 0x04000E72 RID: 3698
		private Matrix mPivotTransform;

		// Token: 0x04000E73 RID: 3699
		private float mPivotDistance;

		// Token: 0x04000E74 RID: 3700
		private Vector3 mPivotTargetDirection;

		// Token: 0x04000E75 RID: 3701
		private PlayState mPlayState;

		// Token: 0x04000E76 RID: 3702
		private bool mPlayersDead;

		// Token: 0x04000E77 RID: 3703
		private HitList mHitList;

		// Token: 0x04000E78 RID: 3704
		private bool mIsHit;

		// Token: 0x04000E79 RID: 3705
		private float mHitTimer;

		// Token: 0x04000E7A RID: 3706
		private Avatar mAggroTarget;

		// Token: 0x04000E7B RID: 3707
		private float mHeadHitFlashTimer;

		// Token: 0x04000E7C RID: 3708
		private float mAssHitFlashTimer;

		// Token: 0x04000E7D RID: 3709
		private float mStageSpeedMod;

		// Token: 0x04000E7E RID: 3710
		private float mTimeBetweenActions;

		// Token: 0x04000E7F RID: 3711
		private float mStateSpeedMod;

		// Token: 0x04000E80 RID: 3712
		private float mAnimationSpeedMod;

		// Token: 0x04000E81 RID: 3713
		private float mMovementSpeedMod;

		// Token: 0x04000E82 RID: 3714
		private bool mTargetWebbed;

		// Token: 0x04000E83 RID: 3715
		private Avatar mTarget;

		// Token: 0x04000E84 RID: 3716
		private Player[] mPlayers;

		// Token: 0x04000E85 RID: 3717
		private Resistance[] mBodyResistances;

		// Token: 0x04000E86 RID: 3718
		private Resistance[] mHeadResistances;

		// Token: 0x04000E87 RID: 3719
		private Matrix[] mFootTransform = new Matrix[8];

		// Token: 0x04000E88 RID: 3720
		private int mMouthJointIndex;

		// Token: 0x04000E89 RID: 3721
		private Matrix mMouthBindPose;

		// Token: 0x04000E8A RID: 3722
		private int mAbdomJointIndex;

		// Token: 0x04000E8B RID: 3723
		private Matrix mAbdomBindPose;

		// Token: 0x04000E8C RID: 3724
		private Matrix mBackAbdomBindPose;

		// Token: 0x04000E8D RID: 3725
		private int mBackAbdomJointIndex;

		// Token: 0x04000E8E RID: 3726
		private int[] mFootJointIndex = new int[8];

		// Token: 0x04000E8F RID: 3727
		private Matrix[] mFootBindPose = new Matrix[8];

		// Token: 0x04000E90 RID: 3728
		private int[] mKneeJointIndex = new int[8];

		// Token: 0x04000E91 RID: 3729
		private Matrix[] mKneeBindPose = new Matrix[8];

		// Token: 0x04000E92 RID: 3730
		private int[] mBodyJointIndex = new int[8];

		// Token: 0x04000E93 RID: 3731
		private Matrix[] mBodyBindPose = new Matrix[8];

		// Token: 0x04000E94 RID: 3732
		private int[] mLeftIndices = new int[16];

		// Token: 0x04000E95 RID: 3733
		private int[] mRightIndices = new int[16];

		// Token: 0x04000E96 RID: 3734
		private static readonly int FOOTSTEP_EFFECT = "footstep_mud".GetHashCodeCustom();

		// Token: 0x04000E97 RID: 3735
		private static readonly int FOOTSTEP_SOUND = "parker_footstep".GetHashCodeCustom();

		// Token: 0x04000E98 RID: 3736
		private static readonly int INTRO_SOUND = "parker_intro".GetHashCodeCustom();

		// Token: 0x04000E99 RID: 3737
		private static readonly int ENTER_CAVE_SOUND = "parker_enter_cave".GetHashCodeCustom();

		// Token: 0x04000E9A RID: 3738
		private static readonly int EXIT_CAVE_SOUND = "parker_exit_cave".GetHashCodeCustom();

		// Token: 0x04000E9B RID: 3739
		private static readonly int CALL_MINION_SOUND = "parker_call_minion".GetHashCodeCustom();

		// Token: 0x04000E9C RID: 3740
		private static readonly int SPAWN_SOUND = "parker_spawn".GetHashCodeCustom();

		// Token: 0x04000E9D RID: 3741
		private static readonly int BITE_SOUND = "parker_bite".GetHashCodeCustom();

		// Token: 0x04000E9E RID: 3742
		private static readonly int SWIPE_SIDES_SOUND = "parker_swipe_sides".GetHashCodeCustom();

		// Token: 0x04000E9F RID: 3743
		private static readonly int SWIPE_FORWARD_SOUND = "parker_swipe_forward".GetHashCodeCustom();

		// Token: 0x04000EA0 RID: 3744
		private static readonly int DEATH_SOUND = "parker_death".GetHashCodeCustom();

		// Token: 0x04000EA1 RID: 3745
		private static readonly int DEFEND_SOUND = "parker_defend".GetHashCodeCustom();

		// Token: 0x04000EA2 RID: 3746
		private static readonly float[] FOOTSTEP_TIMES = new float[]
		{
			0f,
			0.21875f,
			0.46875f,
			0.71875f
		};

		// Token: 0x04000EA3 RID: 3747
		private int mCurrentFootstepIndex;

		// Token: 0x020001D3 RID: 467
		protected class IntroStage : IBossState<Parker>
		{
			// Token: 0x06001015 RID: 4117 RVA: 0x000655D5 File Offset: 0x000637D5
			public void OnEnter(Parker iOwner)
			{
				iOwner.mStageSpeedMod = 1f;
				iOwner.mTimeBetweenActions = 3f;
			}

			// Token: 0x06001016 RID: 4118 RVA: 0x000655F0 File Offset: 0x000637F0
			public void OnUpdate(float iDeltaTime, Parker iOwner)
			{
				if (iOwner.mCurrentState == Parker.States.Intro || iOwner.mCurrentState == Parker.States.Rotate || iOwner.mCurrentState == Parker.States.MoveToCave || iOwner.mCurrentState == Parker.States.MoveToBattle || iOwner.mCurrentState == Parker.States.EnterCave || iOwner.mCurrentState == Parker.States.LeaveCave)
				{
					return;
				}
				if (iOwner.mPlayersDead)
				{
					if (iOwner.mCurrentState != Parker.States.Cave)
					{
						Parker.TransitionToCave(iOwner);
					}
					return;
				}
				if ((iOwner.mCurrentState == Parker.States.Battle || iOwner.mCurrentState == Parker.States.Swipe || iOwner.mCurrentState == Parker.States.Defend) && Parker.CheckBehind(iOwner))
				{
					Parker.TransitionToCave(iOwner);
					return;
				}
				if (iOwner.GetCallMinionState.GetWeight(iOwner, iDeltaTime) > 0f)
				{
					iOwner.ChangeState(Parker.States.CallMinions);
					return;
				}
				if (iOwner.GetSwipState.GetWeight(iOwner, iDeltaTime) > 0f)
				{
					iOwner.ChangeState(Parker.States.Swipe);
					return;
				}
				if (iOwner.GetDefendState.GetWeight(iOwner, iDeltaTime) > 0f)
				{
					iOwner.ChangeState(Parker.States.Defend);
					return;
				}
				if (iOwner.GetWebState.GetWeight(iOwner, iDeltaTime) > 0f)
				{
					iOwner.ChangeState(Parker.States.Web);
					return;
				}
				if (iOwner.GetRushState.GetWeight(iOwner, iDeltaTime) > 0f)
				{
					iOwner.ChangeState(Parker.States.Rush);
				}
			}

			// Token: 0x06001017 RID: 4119 RVA: 0x00065708 File Offset: 0x00063908
			public void OnExit(Parker iOwner)
			{
			}
		}

		// Token: 0x020001D4 RID: 468
		protected class BattleStage : IBossState<Parker>
		{
			// Token: 0x06001019 RID: 4121 RVA: 0x00065712 File Offset: 0x00063912
			public void OnEnter(Parker iOwner)
			{
				iOwner.mStageSpeedMod = 1f;
				iOwner.mTimeBetweenActions = 2f;
			}

			// Token: 0x0600101A RID: 4122 RVA: 0x0006572C File Offset: 0x0006392C
			public void OnUpdate(float iDeltaTime, Parker iOwner)
			{
				if (iOwner.mCurrentState == Parker.States.Rotate || iOwner.mCurrentState == Parker.States.CallMinions || iOwner.mCurrentState == Parker.States.MoveToCave || iOwner.mCurrentState == Parker.States.MoveToBattle || iOwner.mCurrentState == Parker.States.EnterCave || iOwner.mCurrentState == Parker.States.LeaveCave)
				{
					return;
				}
				if (iOwner.mPlayersDead)
				{
					if (iOwner.mCurrentState != Parker.States.Cave)
					{
						Parker.TransitionToCave(iOwner);
					}
					return;
				}
				if ((iOwner.mCurrentState == Parker.States.Battle || iOwner.mCurrentState == Parker.States.Swipe || iOwner.mCurrentState == Parker.States.Defend) && Parker.CheckBehind(iOwner))
				{
					Parker.TransitionToCave(iOwner);
					return;
				}
				if (iOwner.HasStatus(StatusEffects.Burning) && iOwner.BurningDPS > 250f)
				{
					Parker.TransitionToCave(iOwner);
					return;
				}
				if (iOwner.GetCallMinionState.GetWeight(iOwner, iDeltaTime) > 0f)
				{
					iOwner.ChangeState(Parker.States.CallMinions);
					return;
				}
				if (iOwner.GetRushState.GetWeight(iOwner, iDeltaTime) > 0f)
				{
					iOwner.ChangeState(Parker.States.Rush);
					return;
				}
				if (iOwner.GetSwipState.GetWeight(iOwner, iDeltaTime) > 0f)
				{
					iOwner.ChangeState(Parker.States.Swipe);
					return;
				}
				if (iOwner.GetWebState.GetWeight(iOwner, iDeltaTime) > 0f)
				{
					iOwner.ChangeState(Parker.States.Web);
					return;
				}
				if (iOwner.GetSpawnState.GetWeight(iOwner, iDeltaTime) > 0f)
				{
					iOwner.ChangeState(Parker.States.Spawn);
					return;
				}
				if (iOwner.GetDefendState.GetWeight(iOwner, iDeltaTime) > 0f)
				{
					iOwner.ChangeState(Parker.States.Defend);
				}
			}

			// Token: 0x0600101B RID: 4123 RVA: 0x00065880 File Offset: 0x00063A80
			public void OnExit(Parker iOwner)
			{
			}
		}

		// Token: 0x020001D5 RID: 469
		protected class CriticalStage : IBossState<Parker>
		{
			// Token: 0x0600101D RID: 4125 RVA: 0x0006588A File Offset: 0x00063A8A
			public void OnEnter(Parker iOwner)
			{
				iOwner.mStageSpeedMod = 1.5f;
				iOwner.mTimeBetweenActions = 1f;
			}

			// Token: 0x0600101E RID: 4126 RVA: 0x000658A4 File Offset: 0x00063AA4
			public void OnUpdate(float iDeltaTime, Parker iOwner)
			{
				if (iOwner.mCurrentState == Parker.States.Rotate || iOwner.mCurrentState == Parker.States.CallMinions || iOwner.mCurrentState == Parker.States.MoveToCave || iOwner.mCurrentState == Parker.States.MoveToBattle || iOwner.mCurrentState == Parker.States.EnterCave || iOwner.mCurrentState == Parker.States.LeaveCave)
				{
					return;
				}
				if (iOwner.mPlayersDead)
				{
					if (iOwner.mCurrentState != Parker.States.Cave)
					{
						Parker.TransitionToCave(iOwner);
					}
					return;
				}
				if ((iOwner.mCurrentState == Parker.States.Battle || iOwner.mCurrentState == Parker.States.Swipe || iOwner.mCurrentState == Parker.States.Defend) && Parker.CheckBehind(iOwner))
				{
					Parker.TransitionToCave(iOwner);
					return;
				}
				if (iOwner.HasStatus(StatusEffects.Burning) && iOwner.BurningDPS > 250f)
				{
					Parker.TransitionToCave(iOwner);
					return;
				}
				if (iOwner.GetCallMinionState.GetWeight(iOwner, iDeltaTime) > 0f)
				{
					iOwner.ChangeState(Parker.States.CallMinions);
					return;
				}
				if (iOwner.GetSwipState.GetWeight(iOwner, iDeltaTime) > 0f)
				{
					iOwner.ChangeState(Parker.States.Swipe);
					return;
				}
				if (iOwner.GetSpawnState.GetWeight(iOwner, iDeltaTime) > 0f)
				{
					iOwner.ChangeState(Parker.States.Spawn);
					return;
				}
				if (iOwner.GetRushState.GetWeight(iOwner, iDeltaTime) > 0f)
				{
					iOwner.ChangeState(Parker.States.Rush);
					return;
				}
				if (iOwner.GetWebState.GetWeight(iOwner, iDeltaTime) > 0f)
				{
					iOwner.ChangeState(Parker.States.Web);
					return;
				}
				if (iOwner.GetDefendState.GetWeight(iOwner, iDeltaTime) > 0f)
				{
					iOwner.ChangeState(Parker.States.Defend);
				}
			}

			// Token: 0x0600101F RID: 4127 RVA: 0x000659F8 File Offset: 0x00063BF8
			public void OnExit(Parker iOwner)
			{
			}
		}

		// Token: 0x020001D6 RID: 470
		protected class FinalStage : IBossState<Parker>
		{
			// Token: 0x06001021 RID: 4129 RVA: 0x00065A02 File Offset: 0x00063C02
			public void OnEnter(Parker iOwner)
			{
				iOwner.mStageSpeedMod = 2f;
				iOwner.mTimeBetweenActions = 0f;
			}

			// Token: 0x06001022 RID: 4130 RVA: 0x00065A1C File Offset: 0x00063C1C
			public void OnUpdate(float iDeltaTime, Parker iOwner)
			{
				if (iOwner.mCurrentState == Parker.States.Rotate || iOwner.mCurrentState == Parker.States.CallMinions || iOwner.mCurrentState == Parker.States.MoveToCave || iOwner.mCurrentState == Parker.States.MoveToBattle || iOwner.mCurrentState == Parker.States.EnterCave || iOwner.mCurrentState == Parker.States.LeaveCave)
				{
					return;
				}
				if (iOwner.mPlayersDead)
				{
					if (iOwner.mCurrentState != Parker.States.Cave)
					{
						Parker.TransitionToCave(iOwner);
					}
					return;
				}
				if ((iOwner.mCurrentState == Parker.States.Battle || iOwner.mCurrentState == Parker.States.Swipe || iOwner.mCurrentState == Parker.States.Defend) && Parker.CheckBehind(iOwner))
				{
					Parker.TransitionToCave(iOwner);
					return;
				}
				if (iOwner.mIsHit && iOwner.mCurrentState == Parker.States.Battle)
				{
					Parker.TransitionToCave(iOwner);
					return;
				}
				if (iOwner.GetCallMinionState.GetWeight(iOwner, iDeltaTime) > 0f)
				{
					iOwner.ChangeState(Parker.States.CallMinions);
					return;
				}
				if (iOwner.GetRushState.GetWeight(iOwner, iDeltaTime) > 0f)
				{
					iOwner.ChangeState(Parker.States.Rush);
					return;
				}
				if (iOwner.GetSwipState.GetWeight(iOwner, iDeltaTime) > 0f)
				{
					iOwner.ChangeState(Parker.States.Swipe);
					return;
				}
				if (iOwner.GetWebState.GetWeight(iOwner, iDeltaTime) > 0f)
				{
					iOwner.ChangeState(Parker.States.Web);
					return;
				}
				if (iOwner.GetSpawnState.GetWeight(iOwner, iDeltaTime) > 0f)
				{
					iOwner.ChangeState(Parker.States.Spawn);
					return;
				}
				if (iOwner.GetDefendState.GetWeight(iOwner, iDeltaTime) > 0f)
				{
					iOwner.ChangeState(Parker.States.Defend);
				}
			}

			// Token: 0x06001023 RID: 4131 RVA: 0x00065B6B File Offset: 0x00063D6B
			public void OnExit(Parker iOwner)
			{
			}
		}

		// Token: 0x020001D7 RID: 471
		protected class RenderData : RenderableObject<SkinnedModelDeferredEffect, SkinnedModelDeferredAdvancedMaterial>
		{
			// Token: 0x06001025 RID: 4133 RVA: 0x00065B75 File Offset: 0x00063D75
			public RenderData(ModelMesh iMesh)
			{
				this.mSkeleton = new Matrix[80];
				Helper.SkinnedModelDeferredMaterialFromBasicEffect(iMesh.MeshParts[0].Effect as SkinnedModelBasicEffect, out this.mMaterial);
			}

			// Token: 0x06001026 RID: 4134 RVA: 0x00065BAC File Offset: 0x00063DAC
			public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				this.mMaterial.Damage = this.mDamage;
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				skinnedModelDeferredEffect.Bones = this.mSkeleton;
				skinnedModelDeferredEffect.OverrideColor = new Vector4(1f, 1f, 1f, this.mFlash);
				base.Draw(iEffect, iViewFrustum);
				skinnedModelDeferredEffect.OverrideColor = default(Vector4);
				skinnedModelDeferredEffect.Colorize = default(Vector4);
			}

			// Token: 0x06001027 RID: 4135 RVA: 0x00065C24 File Offset: 0x00063E24
			public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				skinnedModelDeferredEffect.Bones = this.mSkeleton;
				base.DrawShadow(iEffect, iViewFrustum);
			}

			// Token: 0x04000EA4 RID: 3748
			public float mFlash;

			// Token: 0x04000EA5 RID: 3749
			public float mDamage;

			// Token: 0x04000EA6 RID: 3750
			public Matrix[] mSkeleton;
		}

		// Token: 0x020001D8 RID: 472
		public enum MessageType : ushort
		{
			// Token: 0x04000EA8 RID: 3752
			Update,
			// Token: 0x04000EA9 RID: 3753
			ChangeStage,
			// Token: 0x04000EAA RID: 3754
			ChangeState,
			// Token: 0x04000EAB RID: 3755
			ChangeTarget,
			// Token: 0x04000EAC RID: 3756
			Web,
			// Token: 0x04000EAD RID: 3757
			RotationTransition
		}

		// Token: 0x020001D9 RID: 473
		internal struct UpdateMessage
		{
			// Token: 0x04000EAE RID: 3758
			public const ushort TYPE = 0;

			// Token: 0x04000EAF RID: 3759
			public byte Animation;

			// Token: 0x04000EB0 RID: 3760
			public ushort Hitpoints;

			// Token: 0x04000EB1 RID: 3761
			public HalfSingle AnimationSpeed;

			// Token: 0x04000EB2 RID: 3762
			public HalfSingle AnimationTime;

			// Token: 0x04000EB3 RID: 3763
			public HalfSingle Speed;

			// Token: 0x04000EB4 RID: 3764
			public bool PositionUpdate;

			// Token: 0x04000EB5 RID: 3765
			public Vector3 Position;
		}

		// Token: 0x020001DA RID: 474
		internal struct ChangeStageMessage
		{
			// Token: 0x04000EB6 RID: 3766
			public const ushort TYPE = 1;

			// Token: 0x04000EB7 RID: 3767
			public Parker.Stages NewStage;
		}

		// Token: 0x020001DB RID: 475
		internal struct ChangeStateMessage
		{
			// Token: 0x04000EB8 RID: 3768
			public const ushort TYPE = 2;

			// Token: 0x04000EB9 RID: 3769
			public Parker.States NewState;
		}

		// Token: 0x020001DC RID: 476
		internal struct ChangeTargetMessage
		{
			// Token: 0x04000EBA RID: 3770
			public const ushort TYPE = 3;

			// Token: 0x04000EBB RID: 3771
			public int Handle;
		}

		// Token: 0x020001DD RID: 477
		internal struct WebMessage
		{
			// Token: 0x04000EBC RID: 3772
			public const ushort TYPE = 4;

			// Token: 0x04000EBD RID: 3773
			public ushort Handle;

			// Token: 0x04000EBE RID: 3774
			public ushort Parent;

			// Token: 0x04000EBF RID: 3775
			public Normalized101010 Direction;

			// Token: 0x04000EC0 RID: 3776
			public Vector3 Position;

			// Token: 0x04000EC1 RID: 3777
			public bool MouthIsOwner;

			// Token: 0x04000EC2 RID: 3778
			public HalfSingle Velocity;
		}

		// Token: 0x020001DE RID: 478
		internal struct RotationTransitionMessage
		{
			// Token: 0x04000EC3 RID: 3779
			public const ushort TYPE = 5;

			// Token: 0x04000EC4 RID: 3780
			public Parker.States State;

			// Token: 0x04000EC5 RID: 3781
			public Normalized101010 Direction;
		}

		// Token: 0x020001DF RID: 479
		public enum States
		{
			// Token: 0x04000EC7 RID: 3783
			Intro,
			// Token: 0x04000EC8 RID: 3784
			Battle,
			// Token: 0x04000EC9 RID: 3785
			LeaveCave,
			// Token: 0x04000ECA RID: 3786
			EnterCave,
			// Token: 0x04000ECB RID: 3787
			Swipe,
			// Token: 0x04000ECC RID: 3788
			Bite,
			// Token: 0x04000ECD RID: 3789
			Rush,
			// Token: 0x04000ECE RID: 3790
			Backup,
			// Token: 0x04000ECF RID: 3791
			Stun,
			// Token: 0x04000ED0 RID: 3792
			Web,
			// Token: 0x04000ED1 RID: 3793
			Spawn,
			// Token: 0x04000ED2 RID: 3794
			CallMinions,
			// Token: 0x04000ED3 RID: 3795
			Defend,
			// Token: 0x04000ED4 RID: 3796
			Rotate,
			// Token: 0x04000ED5 RID: 3797
			MoveToBattle,
			// Token: 0x04000ED6 RID: 3798
			MoveToCave,
			// Token: 0x04000ED7 RID: 3799
			Cave,
			// Token: 0x04000ED8 RID: 3800
			Dead,
			// Token: 0x04000ED9 RID: 3801
			NrOfStates
		}

		// Token: 0x020001E0 RID: 480
		public enum Stages
		{
			// Token: 0x04000EDB RID: 3803
			Intro,
			// Token: 0x04000EDC RID: 3804
			Battle,
			// Token: 0x04000EDD RID: 3805
			Critical,
			// Token: 0x04000EDE RID: 3806
			Final,
			// Token: 0x04000EDF RID: 3807
			NrOfStages
		}

		// Token: 0x020001E1 RID: 481
		public enum Animations
		{
			// Token: 0x04000EE1 RID: 3809
			walk_backward,
			// Token: 0x04000EE2 RID: 3810
			walk_forward,
			// Token: 0x04000EE3 RID: 3811
			walk_left,
			// Token: 0x04000EE4 RID: 3812
			walk_right,
			// Token: 0x04000EE5 RID: 3813
			idle,
			// Token: 0x04000EE6 RID: 3814
			rotate_left,
			// Token: 0x04000EE7 RID: 3815
			rotate_right,
			// Token: 0x04000EE8 RID: 3816
			attack_left,
			// Token: 0x04000EE9 RID: 3817
			attack_right,
			// Token: 0x04000EEA RID: 3818
			attack_front,
			// Token: 0x04000EEB RID: 3819
			bite,
			// Token: 0x04000EEC RID: 3820
			web,
			// Token: 0x04000EED RID: 3821
			spawn,
			// Token: 0x04000EEE RID: 3822
			defend,
			// Token: 0x04000EEF RID: 3823
			stun,
			// Token: 0x04000EF0 RID: 3824
			cave_enter,
			// Token: 0x04000EF1 RID: 3825
			cave_help,
			// Token: 0x04000EF2 RID: 3826
			cave_idle,
			// Token: 0x04000EF3 RID: 3827
			cave_taunt,
			// Token: 0x04000EF4 RID: 3828
			cave_exit,
			// Token: 0x04000EF5 RID: 3829
			intro,
			// Token: 0x04000EF6 RID: 3830
			taunt,
			// Token: 0x04000EF7 RID: 3831
			die,
			// Token: 0x04000EF8 RID: 3832
			NrOfAnimations
		}

		// Token: 0x020001E2 RID: 482
		private interface IParkerState : IBossState<Parker>
		{
			// Token: 0x06001028 RID: 4136
			float GetWeight(Parker iOwner, float iDeltaTime);
		}

		// Token: 0x020001E3 RID: 483
		protected class IntroState : Parker.IParkerState, IBossState<Parker>
		{
			// Token: 0x06001029 RID: 4137 RVA: 0x00065C4C File Offset: 0x00063E4C
			public void OnEnter(Parker iOwner)
			{
				AudioManager.Instance.PlayCue(Banks.Additional, Parker.INTRO_SOUND, iOwner.mAbdomenZone.AudioEmitter);
				iOwner.mAnimationController.StartClip(iOwner.mAnimationClips[20], false);
				iOwner.mStateSpeedMod = 1f;
			}

			// Token: 0x0600102A RID: 4138 RVA: 0x00065C9C File Offset: 0x00063E9C
			public void OnUpdate(float iDeltaTime, Parker iOwner)
			{
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					if (iOwner.mAnimationController.AnimationClip == iOwner.mAnimationClips[20])
					{
						if (iOwner.mAnimationController.HasFinished)
						{
							iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[4], 0.4f, false);
							return;
						}
					}
					else
					{
						iOwner.ChangeState(Parker.States.Battle);
					}
				}
			}

			// Token: 0x0600102B RID: 4139 RVA: 0x00065CFA File Offset: 0x00063EFA
			public void OnExit(Parker iOwner)
			{
			}

			// Token: 0x0600102C RID: 4140 RVA: 0x00065CFC File Offset: 0x00063EFC
			public float GetWeight(Parker iOwner, float iDeltaTime)
			{
				return 0f;
			}
		}

		// Token: 0x020001E4 RID: 484
		protected class BattleState : Parker.IParkerState, IBossState<Parker>
		{
			// Token: 0x0600102E RID: 4142 RVA: 0x00065D0C File Offset: 0x00063F0C
			public void OnEnter(Parker iOwner)
			{
				if (iOwner.mAnimationController.AnimationClip == iOwner.mAnimationClips[2])
				{
					this.mCurrentAnimation = Parker.Animations.walk_left;
				}
				else if (iOwner.mAnimationController.AnimationClip == iOwner.mAnimationClips[3])
				{
					this.mCurrentAnimation = Parker.Animations.walk_right;
				}
				else if (iOwner.mAnimationController.AnimationClip == iOwner.mAnimationClips[4])
				{
					this.mCurrentAnimation = Parker.Animations.idle;
				}
				else
				{
					this.mCurrentAnimation = Parker.Animations.intro;
				}
				iOwner.mStateSpeedMod = 1f;
				iOwner.mAnimationSpeedMod = 1f;
				if (iOwner.mTarget != null && iOwner.mTarget.Dead)
				{
					iOwner.mTarget = null;
				}
				Avatar target;
				if (iOwner.mTarget == null && iOwner.GetRandomTarget(out target))
				{
					iOwner.SetTarget(target);
				}
				if (iOwner.GetRushState.GetWeight(iOwner, 0f) > 0f)
				{
					iOwner.ChangeState(Parker.States.Rush);
					return;
				}
				if (iOwner.GetWebState.GetWeight(iOwner, 0f) > 0f)
				{
					iOwner.ChangeState(Parker.States.Web);
					return;
				}
				Avatar target2;
				if (iOwner.GetRandomTarget(out target2) && iOwner.mTarget != null && !iOwner.mTargetWebbed && !iOwner.mTarget.HasStatus(StatusEffects.Poisoned))
				{
					iOwner.SetTarget(target2);
				}
			}

			// Token: 0x0600102F RID: 4143 RVA: 0x00065E38 File Offset: 0x00064038
			public void OnUpdate(float iDeltaTime, Parker iOwner)
			{
				if (iOwner.mTarget != null && (iOwner.mTargetWebbed || iOwner.mTarget.HasStatus(StatusEffects.Poisoned)))
				{
					iOwner.mStateSpeedMod = 1.5f;
				}
				else
				{
					iOwner.mStateSpeedMod = 1f;
				}
				Vector3 translation = iOwner.mPivotTransform.Translation;
				Vector3 forward = iOwner.mPivotTransform.Forward;
				Avatar avatar;
				if ((iOwner.mTarget == null || iOwner.mTarget.Dead) && iOwner.GetRandomTarget(out avatar) && avatar != null)
				{
					iOwner.SetTarget(avatar);
				}
				Vector3 mPlayerTargetPosition = iOwner.mPlayerTargetPosition;
				Vector3 mPivotTargetDirection;
				Vector3.Subtract(ref mPlayerTargetPosition, ref translation, out mPivotTargetDirection);
				mPivotTargetDirection.Y = 0f;
				mPivotTargetDirection.Normalize();
				float num;
				Parker.GetConstrainedAngle(ref forward, ref mPivotTargetDirection, out num, -0.7f, 0.7f);
				iOwner.mPivotTargetDirection = mPivotTargetDirection;
				if (Math.Abs(num) > 0.005f && !iOwner.mAnimationController.CrossFadeEnabled)
				{
					float num2 = MathHelper.ToRadians(iOwner.mMovementSpeed);
					float num3;
					if (num < 0f)
					{
						num3 = Math.Max(num, -num2);
					}
					else
					{
						num3 = Math.Min(num, num2);
					}
					num3 = 2f * num3 * iOwner.mStateSpeedMod * iOwner.mStageSpeedMod * iOwner.GetColdSpeed();
					Quaternion quaternion;
					Quaternion.CreateFromYawPitchRoll(num3 * iDeltaTime, 0f, 0f, out quaternion);
					Vector3 forward2 = iOwner.mPivotTransform.Forward;
					Vector3 vector;
					Vector3.Transform(ref forward2, ref quaternion, out vector);
					Vector3 translation2 = iOwner.mPivotTransform.Translation;
					Matrix.Transform(ref iOwner.mPivotTransform, ref quaternion, out iOwner.mPivotTransform);
					iOwner.mPivotTransform.Translation = translation2;
				}
				iOwner.mTransform = iOwner.BattleStateTransform;
				float val = 0.25f + Math.Abs(num) * 5f;
				iOwner.mAnimationSpeedMod = Math.Min(Math.Max(val, 0f), 1f);
				if (num > 0.01f)
				{
					if (this.mCurrentAnimation != Parker.Animations.walk_left)
					{
						iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[2], 0.2f, true);
						this.mCurrentAnimation = Parker.Animations.walk_left;
						return;
					}
				}
				else if (num < -0.01f)
				{
					if (this.mCurrentAnimation != Parker.Animations.walk_right)
					{
						iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[3], 0.2f, true);
						this.mCurrentAnimation = Parker.Animations.walk_right;
						return;
					}
				}
				else
				{
					iOwner.mAnimationSpeedMod = 1f;
					if (this.mCurrentAnimation != Parker.Animations.idle)
					{
						iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[4], 0.2f, true);
						this.mCurrentAnimation = Parker.Animations.idle;
					}
				}
			}

			// Token: 0x06001030 RID: 4144 RVA: 0x0006609C File Offset: 0x0006429C
			public void OnExit(Parker iOwner)
			{
				iOwner.mStateSpeedMod = 1f;
				iOwner.mAnimationSpeedMod = 1f;
			}

			// Token: 0x06001031 RID: 4145 RVA: 0x000660B4 File Offset: 0x000642B4
			public float GetWeight(Parker iOwner, float iDeltaTime)
			{
				return 0f;
			}

			// Token: 0x04000EF9 RID: 3833
			public const float ANGLE = 0.7f;

			// Token: 0x04000EFA RID: 3834
			public const float THRESHOLD_ANGLE = 0.01f;

			// Token: 0x04000EFB RID: 3835
			private Parker.Animations mCurrentAnimation;
		}

		// Token: 0x020001E5 RID: 485
		protected class DefendState : Parker.IParkerState, IBossState<Parker>
		{
			// Token: 0x06001033 RID: 4147 RVA: 0x000660C3 File Offset: 0x000642C3
			public void OnEnter(Parker iOwner)
			{
				this.mPlayedSound = false;
				iOwner.mStateSpeedMod = 1.5f;
				iOwner.Movement = Vector3.Zero;
				iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[13], 0.15f, false);
			}

			// Token: 0x06001034 RID: 4148 RVA: 0x000660FC File Offset: 0x000642FC
			public void OnUpdate(float iDeltaTime, Parker iOwner)
			{
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					if (iOwner.mAnimationController.HasFinished)
					{
						if (iOwner.mPreviousState == Parker.States.Spawn)
						{
							iOwner.ChangeState(Parker.States.Battle);
							return;
						}
						iOwner.ChangeState(iOwner.mPreviousState);
						return;
					}
					else if (!this.mPlayedSound)
					{
						this.mPlayedSound = true;
						AudioManager.Instance.PlayCue(Banks.Additional, Parker.DEFEND_SOUND, iOwner.mMouthZone.AudioEmitter);
					}
				}
			}

			// Token: 0x06001035 RID: 4149 RVA: 0x00066171 File Offset: 0x00064371
			public void OnExit(Parker iOwner)
			{
				iOwner.mStateSpeedMod = 1f;
				iOwner.mIsHit = false;
				iOwner.mHitTimer = 8f;
			}

			// Token: 0x06001036 RID: 4150 RVA: 0x00066190 File Offset: 0x00064390
			public float GetWeight(Parker iOwner, float iDeltaTime)
			{
				if (iOwner.mCurrentState == Parker.States.Defend)
				{
					return 0f;
				}
				if (iOwner.mCurrentState == Parker.States.Spawn)
				{
					return 0f;
				}
				if (iOwner.mCurrentState == Parker.States.CallMinions)
				{
					return 0f;
				}
				if (iOwner.mCurrentState != Parker.States.Battle || !iOwner.mIsHit)
				{
					iOwner.mIsHit = false;
					return 0f;
				}
				if (iOwner.mCurrentStage == Parker.Stages.Intro)
				{
					return 1f;
				}
				if (iOwner.mCurrentStage != Parker.Stages.Intro && !iOwner.mTargetWebbed)
				{
					return 1f;
				}
				iOwner.mIsHit = false;
				return 0f;
			}

			// Token: 0x04000EFC RID: 3836
			private const float DEFEND_SOUND_TIME = 0.15384616f;

			// Token: 0x04000EFD RID: 3837
			private bool mPlayedSound;
		}

		// Token: 0x020001E6 RID: 486
		protected class StunState : Parker.IParkerState, IBossState<Parker>
		{
			// Token: 0x06001038 RID: 4152 RVA: 0x00066224 File Offset: 0x00064424
			public void OnEnter(Parker iOwner)
			{
				iOwner.Movement = default(Vector3);
				iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[14], 0.25f, false);
			}

			// Token: 0x06001039 RID: 4153 RVA: 0x0006625A File Offset: 0x0006445A
			public void OnUpdate(float iDeltaTime, Parker iOwner)
			{
				if (!iOwner.mAnimationController.CrossFadeEnabled && iOwner.mAnimationController.HasFinished)
				{
					iOwner.ChangeState(iOwner.mPreviousState);
				}
			}

			// Token: 0x0600103A RID: 4154 RVA: 0x00066282 File Offset: 0x00064482
			public void OnExit(Parker iOwner)
			{
				iOwner.mStateSpeedMod = 1f;
			}

			// Token: 0x0600103B RID: 4155 RVA: 0x0006628F File Offset: 0x0006448F
			public float GetWeight(Parker iOwner, float iDeltaTime)
			{
				return 0f;
			}
		}

		// Token: 0x020001E7 RID: 487
		protected class SwipeState : Parker.IParkerState, IBossState<Parker>
		{
			// Token: 0x0600103D RID: 4157 RVA: 0x000662A0 File Offset: 0x000644A0
			public SwipeState()
			{
				this.mSwipeDamage = default(DamageCollection5);
				this.mSwipeDamage.AddDamage(new Damage(AttackProperties.Knockback, Elements.Earth, 100f, 3f));
				this.mSwipeDamage.AddDamage(new Damage(AttackProperties.Damage, Elements.Earth, 50f, 1f));
			}

			// Token: 0x0600103E RID: 4158 RVA: 0x00066300 File Offset: 0x00064500
			public void OnEnter(Parker iOwner)
			{
				iOwner.Movement = Vector3.Zero;
				switch (this.mSwipeType)
				{
				case Parker.SwipeState.Directions.Left:
					iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[7], 0.25f, false);
					break;
				case Parker.SwipeState.Directions.Right:
					iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[8], 0.25f, false);
					break;
				case Parker.SwipeState.Directions.Front:
					iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[9], 0.25f, false);
					break;
				}
				this.mPlayedSound = false;
			}

			// Token: 0x0600103F RID: 4159 RVA: 0x0006638C File Offset: 0x0006458C
			public void OnUpdate(float iDeltaTime, Parker iOwner)
			{
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					if (iOwner.mAnimationController.HasFinished)
					{
						iOwner.ChangeState(iOwner.mPreviousState);
						return;
					}
					float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
					if (this.mSwipeType == Parker.SwipeState.Directions.Front)
					{
						if (!this.mPlayedSound && num >= 0.61290324f)
						{
							AudioManager.Instance.PlayCue(Banks.Additional, Parker.BITE_SOUND, iOwner.mAbdomenZone.AudioEmitter);
							this.mPlayedSound = true;
						}
						return;
					}
					if (num >= Parker.ATTACK_LEFT_RIGHT_TIME[0] && num <= Parker.ATTACK_LEFT_RIGHT_TIME[1])
					{
						Segment segment = default(Segment);
						Vector3 iCenter;
						float num2;
						if (this.mSwipeType == Parker.SwipeState.Directions.Left)
						{
							Vector3 translation = iOwner.mFootTransform[2].Translation;
							Vector3 translation2 = iOwner.mFootTransform[1].Translation;
							Vector3.Subtract(ref translation2, ref translation, out segment.Delta);
							segment.Origin = translation;
							Vector3.Multiply(ref segment.Delta, 0.5f, out iCenter);
							num2 = iCenter.Length();
							Vector3.Add(ref iCenter, ref translation, out iCenter);
						}
						else
						{
							Vector3 translation3 = iOwner.mFootTransform[6].Translation;
							Vector3 translation4 = iOwner.mFootTransform[5].Translation;
							Vector3.Subtract(ref translation4, ref translation3, out segment.Delta);
							segment.Origin = translation3;
							Vector3.Multiply(ref segment.Delta, 0.5f, out iCenter);
							num2 = iCenter.Length();
							Vector3.Add(ref iCenter, ref translation3, out iCenter);
						}
						List<Entity> entities = iOwner.mPlayState.EntityManager.GetEntities(iCenter, num2 * 1.25f, true, false);
						entities.Remove(iOwner.mAbdomenZone);
						for (int i = 0; i < entities.Count; i++)
						{
							IDamageable damageable = entities[i] as IDamageable;
							if (damageable != null && !iOwner.mHitList.Contains(damageable))
							{
								this.mSwipeDamage.A.Amount = damageable.Body.Mass;
								damageable.Damage(this.mSwipeDamage, iOwner.mAbdomenZone, iOwner.mPlayState.PlayTime, iOwner.mTransform.Translation);
								iOwner.mHitList.Add(damageable);
								if (!this.mPlayedSound)
								{
									AudioManager.Instance.PlayCue(Banks.Additional, Parker.SWIPE_SIDES_SOUND, iOwner.mAbdomenZone.AudioEmitter);
									this.mPlayedSound = true;
								}
							}
						}
						iOwner.mPlayState.EntityManager.ReturnEntityList(entities);
					}
				}
			}

			// Token: 0x06001040 RID: 4160 RVA: 0x0006661A File Offset: 0x0006481A
			public void OnExit(Parker iOwner)
			{
				iOwner.mStateSpeedMod = 1f;
			}

			// Token: 0x06001041 RID: 4161 RVA: 0x00066628 File Offset: 0x00064828
			public float GetWeight(Parker iOwner, float iDeltaTime)
			{
				if (iOwner.mCurrentState == Parker.States.Swipe)
				{
					return 0f;
				}
				if (iOwner.mCurrentState != Parker.States.Battle)
				{
					return 0f;
				}
				this.mWeightTimer -= iDeltaTime;
				if (this.mWeightTimer > 0f)
				{
					return 0f;
				}
				this.mWeightTimer = 0.5f;
				Vector3 translation = iOwner.mTransform.Translation;
				Vector3 forward = iOwner.mTransform.Forward;
				for (int i = 0; i < iOwner.mPlayers.Length; i++)
				{
					if (iOwner.mPlayers[i].Playing && iOwner.mPlayers[i].Avatar != null && !iOwner.mPlayers[i].Avatar.Dead && Parker.GetDistanceSquared(iOwner, iOwner.mPlayers[i].Avatar) < 49f)
					{
						Vector3 position = iOwner.mPlayers[i].Avatar.Position;
						float num;
						Parker.GetAngle(ref translation, ref forward, ref position, out num);
						if (Math.Abs(num) <= 2.5132742f)
						{
							iOwner.SetTarget(iOwner.mPlayers[i].Avatar);
							if (Math.Abs(num) <= 0.47123894f)
							{
								this.mSwipeType = Parker.SwipeState.Directions.Front;
								return 1f;
							}
							if (num > -0.47123894f)
							{
								this.mSwipeType = Parker.SwipeState.Directions.Left;
								return 1f;
							}
							if (num < 0.47123894f)
							{
								this.mSwipeType = Parker.SwipeState.Directions.Right;
								return 1f;
							}
						}
					}
				}
				return 0f;
			}

			// Token: 0x04000EFE RID: 3838
			internal bool mPlayedSound;

			// Token: 0x04000EFF RID: 3839
			internal Parker.SwipeState.Directions mSwipeType = Parker.SwipeState.Directions.Front;

			// Token: 0x04000F00 RID: 3840
			private DamageCollection5 mSwipeDamage;

			// Token: 0x04000F01 RID: 3841
			private float mWeightTimer;

			// Token: 0x020001E8 RID: 488
			public enum Directions
			{
				// Token: 0x04000F03 RID: 3843
				Left,
				// Token: 0x04000F04 RID: 3844
				Right,
				// Token: 0x04000F05 RID: 3845
				Front
			}
		}

		// Token: 0x020001E9 RID: 489
		protected class RushState : Parker.IParkerState, IBossState<Parker>
		{
			// Token: 0x06001042 RID: 4162 RVA: 0x00066798 File Offset: 0x00064998
			public void OnEnter(Parker iOwner)
			{
				iOwner.mStateSpeedMod = 1f;
				iOwner.Movement = Vector3.Zero;
				iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[1], 0.5f, true);
				if (iOwner.mTarget != null)
				{
					this.mTargetPosition = iOwner.mTarget.Position;
				}
				else
				{
					this.mTargetPosition = iOwner.mTransform.Translation;
					Vector3 forward = iOwner.mTransform.Forward;
					Vector3.Multiply(ref forward, 13f, out forward);
					Vector3.Add(ref this.mTargetPosition, ref forward, out this.mTargetPosition);
				}
				this.mBlendTime = 0f;
			}

			// Token: 0x06001043 RID: 4163 RVA: 0x00066838 File Offset: 0x00064A38
			public void OnUpdate(float iDeltaTime, Parker iOwner)
			{
				Vector3 vector = this.mTargetPosition;
				Vector3 translation = iOwner.mMouthOrientation.Translation;
				translation.Y = (vector.Y = 0f);
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					if (iOwner.mTarget != null && iOwner.mTarget.HasStatus(StatusEffects.Poisoned))
					{
						iOwner.mMovementSpeedMod = 1.5f;
					}
					else
					{
						iOwner.mMovementSpeedMod = 1f;
					}
				}
				else
				{
					iOwner.mMovementSpeedMod = 0f;
				}
				this.mBlendTime -= iDeltaTime;
				if (MagickaMath.IsApproximately(ref translation, ref vector, 1f))
				{
					iOwner.Movement = Vector3.Zero;
					iOwner.ChangeState(Parker.States.Bite);
					return;
				}
				Vector3 movement;
				Vector3.Subtract(ref vector, ref translation, out movement);
				float val = movement.LengthSquared();
				movement.Normalize();
				iOwner.mStateSpeedMod = 0.5f + Math.Min(val, 1f) * 0.5f;
				iOwner.Movement = movement;
			}

			// Token: 0x06001044 RID: 4164 RVA: 0x00066928 File Offset: 0x00064B28
			public void OnExit(Parker iOwner)
			{
				iOwner.mMovementSpeedMod = 1f;
				iOwner.mStateSpeedMod = 1f;
				iOwner.Movement = Vector3.Zero;
			}

			// Token: 0x06001045 RID: 4165 RVA: 0x0006694C File Offset: 0x00064B4C
			public float GetWeight(Parker iOwner, float iDeltaTime)
			{
				if (iOwner.mTarget != null && iOwner.mCurrentState == Parker.States.Battle)
				{
					Vector3 translation = iOwner.mTransform.Translation;
					Vector3 forward = iOwner.mTransform.Forward;
					Vector3 position = iOwner.mTarget.Position;
					if ((iOwner.mCurrentStage == Parker.Stages.Intro || iOwner.mCurrentStage == Parker.Stages.Battle) && iOwner.mTargetWebbed)
					{
						float value;
						Parker.GetAngle(ref translation, ref forward, ref position, out value);
						if (Math.Abs(value) > 0.06283186f)
						{
							return 0f;
						}
					}
					else if (iOwner.mCurrentStage == Parker.Stages.Final)
					{
						float value2;
						Parker.GetAngle(ref translation, ref forward, ref position, out value2);
						if (Math.Abs(value2) > 0.04712389f)
						{
							return 0f;
						}
					}
					else
					{
						if (!iOwner.mTarget.HasStatus(StatusEffects.Poisoned) && !iOwner.mTargetWebbed)
						{
							return 0f;
						}
						float value3;
						Parker.GetAngle(ref translation, ref forward, ref position, out value3);
						if (Math.Abs(value3) > 0.04712389f)
						{
							return 0f;
						}
					}
					Segment iSeg = default(Segment);
					iSeg.Origin = translation;
					Vector3.Subtract(ref position, ref iSeg.Origin, out iSeg.Delta);
					List<Shield> shields = iOwner.mPlayState.EntityManager.Shields;
					for (int i = 0; i < shields.Count; i++)
					{
						Vector3 vector;
						if (shields[i].SegmentIntersect(out vector, iSeg, 0f))
						{
							return 0f;
						}
					}
					return 1f;
				}
				return 0f;
			}

			// Token: 0x04000F06 RID: 3846
			private Vector3 mTargetPosition;

			// Token: 0x04000F07 RID: 3847
			private float mBlendTime;
		}

		// Token: 0x020001EA RID: 490
		protected class BackupState : Parker.IParkerState, IBossState<Parker>
		{
			// Token: 0x06001047 RID: 4167 RVA: 0x00066AB4 File Offset: 0x00064CB4
			public void OnEnter(Parker iOwner)
			{
				iOwner.mStateSpeedMod = 1f;
				this.mTargetPosition = iOwner.BattleStateTransform.Translation;
				iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[0], 0.2f, true);
			}

			// Token: 0x06001048 RID: 4168 RVA: 0x00066AFC File Offset: 0x00064CFC
			public void OnUpdate(float iDeltaTime, Parker iOwner)
			{
				Vector3 vector = this.mTargetPosition;
				Vector3 translation = iOwner.mTransform.Translation;
				translation.Y = (vector.Y = 0f);
				if (!MagickaMath.IsApproximately(ref translation, ref vector, 0.01f))
				{
					Vector3 movement;
					Vector3.Subtract(ref vector, ref translation, out movement);
					float val = movement.LengthSquared();
					movement.Normalize();
					iOwner.mStateSpeedMod = 0.5f + Math.Min(val, 1f) * 0.5f;
					iOwner.Movement = movement;
					return;
				}
				iOwner.Movement = Vector3.Zero;
				if (NetworkManager.Instance.State == NetworkState.Client)
				{
					return;
				}
				iOwner.ChangeState(Parker.States.Battle);
			}

			// Token: 0x06001049 RID: 4169 RVA: 0x00066BA3 File Offset: 0x00064DA3
			public void OnExit(Parker iOwner)
			{
				iOwner.mStateSpeedMod = 1f;
				iOwner.Movement = Vector3.Zero;
			}

			// Token: 0x0600104A RID: 4170 RVA: 0x00066BBB File Offset: 0x00064DBB
			public float GetWeight(Parker iOwner, float iDeltaTime)
			{
				return 0f;
			}

			// Token: 0x04000F08 RID: 3848
			private Vector3 mTargetPosition;
		}

		// Token: 0x020001EB RID: 491
		protected class BiteState : Parker.IParkerState, IBossState<Parker>
		{
			// Token: 0x0600104C RID: 4172 RVA: 0x00066BCA File Offset: 0x00064DCA
			public void OnEnter(Parker iOwner)
			{
				this.mPlayedSound = false;
				iOwner.mStateSpeedMod = 1.5f;
				iOwner.Movement = Vector3.Zero;
				iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[10], 0.25f, false);
			}

			// Token: 0x0600104D RID: 4173 RVA: 0x00066C04 File Offset: 0x00064E04
			public void OnUpdate(float iDeltaTime, Parker iOwner)
			{
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
					if (iOwner.mAnimationController.HasFinished)
					{
						if (iOwner.GetRushState.GetWeight(iOwner, iDeltaTime) > 0f)
						{
							iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[10], 0.25f, false);
							return;
						}
						iOwner.ChangeState(Parker.States.Backup);
						return;
					}
					else if (!this.mPlayedSound && num >= 0.61290324f)
					{
						AudioManager.Instance.PlayCue(Banks.Additional, Parker.BITE_SOUND, iOwner.mAbdomenZone.AudioEmitter);
						this.mPlayedSound = true;
					}
				}
			}

			// Token: 0x0600104E RID: 4174 RVA: 0x00066CB9 File Offset: 0x00064EB9
			public void OnExit(Parker iOwner)
			{
				iOwner.mStateSpeedMod = 1f;
			}

			// Token: 0x0600104F RID: 4175 RVA: 0x00066CC6 File Offset: 0x00064EC6
			public float GetWeight(Parker iOwner, float iDeltaTime)
			{
				return 0f;
			}

			// Token: 0x04000F09 RID: 3849
			internal const float BITE_SOUND_TIME = 0.61290324f;

			// Token: 0x04000F0A RID: 3850
			private bool mPlayedSound;
		}

		// Token: 0x020001EC RID: 492
		protected class WebState : Parker.IParkerState, IBossState<Parker>
		{
			// Token: 0x06001051 RID: 4177 RVA: 0x00066CD8 File Offset: 0x00064ED8
			public void OnEnter(Parker iOwner)
			{
				iOwner.mStateSpeedMod = 1.75f;
				iOwner.Movement = Vector3.Zero;
				iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[11], 0.75f, false);
				this.mEntity = null;
				this.mTimer = 0f;
				this.mCount = 0;
				this.mSprayTime = iOwner.mAnimationClips[11].Duration * Parker.WebState.WEB_TIME * 1f / (iOwner.mStateSpeedMod * iOwner.mStageSpeedMod * iOwner.mAnimationSpeedMod);
				AudioManager.Instance.PlayCue(Banks.Characters, Parker.WebState.PRE_SOUND, iOwner.mMouthZone.AudioEmitter);
			}

			// Token: 0x06001052 RID: 4178 RVA: 0x00066D80 File Offset: 0x00064F80
			public unsafe void OnUpdate(float iDeltaTime, Parker iOwner)
			{
				if (NetworkManager.Instance.State == NetworkState.Client)
				{
					return;
				}
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
					this.mTimer -= iDeltaTime;
					if (iOwner.mAnimationController.HasFinished)
					{
						if (iOwner.GetRushState.GetWeight(iOwner, 0f) > 0f)
						{
							iOwner.ChangeState(Parker.States.Rush);
							return;
						}
						iOwner.ChangeState(Parker.States.Battle);
						return;
					}
					else if (this.mTimer <= 0f & this.mCount < 10 & num >= Parker.WEBBING_TIME[0] & num <= Parker.WEBBING_TIME[1])
					{
						Vector3 translation = iOwner.mMouthOrientation.Translation;
						Vector3 mPlayerTargetPosition = iOwner.mPlayerTargetPosition;
						Vector3 backward;
						Vector3.Subtract(ref mPlayerTargetPosition, ref translation, out backward);
						float num2 = backward.Length();
						backward = iOwner.mMouthOrientation.Backward;
						backward.Y = 0f;
						backward.Normalize();
						Vector3 forward = iOwner.mTransform.Forward;
						MagickaMath.ConstrainVector(ref backward, ref forward, -0.65f, 0.65f);
						SprayEntity instance = SprayEntity.GetInstance();
						Vector3 translation2 = iOwner.mMouthOrientation.Translation;
						this.mCount++;
						instance.Initialize((this.mCount == 10) ? null : iOwner.mMouthZone, translation2, backward, num2 * 3f);
						if (this.mEntity != null)
						{
							this.mEntity.Child = instance;
						}
						AudioManager.Instance.PlayCue(Banks.Characters, Parker.WebState.WEB_SOUND, iOwner.mMouthZone.AudioEmitter);
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							Parker.WebMessage webMessage = default(Parker.WebMessage);
							webMessage.Handle = instance.Handle;
							if (this.mEntity != null)
							{
								webMessage.Parent = this.mEntity.Handle;
							}
							else
							{
								webMessage.Parent = 0;
							}
							webMessage.Position = translation2;
							webMessage.Direction = new Normalized101010(backward);
							webMessage.Velocity = new HalfSingle(num2 * 3f);
							webMessage.MouthIsOwner = (this.mCount != 10);
							BossFight.Instance.SendMessage<Parker.WebMessage>(iOwner, 4, (void*)(&webMessage), true);
						}
						this.mEntity = instance;
						iOwner.mPlayState.EntityManager.AddEntity(instance);
						this.mTimer = 0.1f * this.mSprayTime;
					}
				}
			}

			// Token: 0x06001053 RID: 4179 RVA: 0x00066FEB File Offset: 0x000651EB
			public void OnExit(Parker iOwner)
			{
				this.mWeightTimer = iOwner.mTimeBetweenActions;
				iOwner.mStateSpeedMod = 1f;
			}

			// Token: 0x06001054 RID: 4180 RVA: 0x00067004 File Offset: 0x00065204
			public float GetWeight(Parker iOwner, float iDeltaTime)
			{
				if (iOwner.mCurrentState == Parker.States.Web)
				{
					return 0f;
				}
				this.mWeightTimer -= iDeltaTime;
				if (this.mWeightTimer > 0f || iOwner.mCurrentState != Parker.States.Battle || iOwner.mTarget == null || iOwner.mTarget.Dead || iOwner.mTarget.IsEntangled)
				{
					return 0f;
				}
				Vector3 translation = iOwner.mTransform.Translation;
				Vector3 forward = iOwner.mTransform.Forward;
				Vector3 position = iOwner.mTarget.Position;
				float value;
				Parker.GetAngle(ref translation, ref forward, ref position, out value);
				if (Math.Abs(value) < 0.15707964f)
				{
					return 1f;
				}
				return 0f;
			}

			// Token: 0x04000F0B RID: 3851
			public const int SEGMENTS = 10;

			// Token: 0x04000F0C RID: 3852
			private const float SEGMENTS_DIVISOR = 0.1f;

			// Token: 0x04000F0D RID: 3853
			internal static readonly int WEB_SOUND = "chr_spider_web".GetHashCodeCustom();

			// Token: 0x04000F0E RID: 3854
			internal static readonly int PRE_SOUND = "chr_spider_web_pre".GetHashCodeCustom();

			// Token: 0x04000F0F RID: 3855
			private static readonly float WEB_TIME = Parker.WEBBING_TIME[1] - Parker.WEBBING_TIME[0];

			// Token: 0x04000F10 RID: 3856
			private SprayEntity mEntity;

			// Token: 0x04000F11 RID: 3857
			private float mTimer;

			// Token: 0x04000F12 RID: 3858
			private float mSprayTime;

			// Token: 0x04000F13 RID: 3859
			private int mCount;

			// Token: 0x04000F14 RID: 3860
			private float mWeightTimer;
		}

		// Token: 0x020001ED RID: 493
		protected class SpawnState : Parker.IParkerState, IBossState<Parker>
		{
			// Token: 0x06001057 RID: 4183 RVA: 0x000670F4 File Offset: 0x000652F4
			public void OnEnter(Parker iOwner)
			{
				iOwner.mStateSpeedMod = 1f;
				iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[12], 0.25f, false);
				this.mTimer = 0f;
				this.mCount = 0;
				this.mSpawnTime = iOwner.mAnimationClips[11].Duration * Parker.SpawnState.SPAWNING_TIME * 1f / (6f * iOwner.mStageSpeedMod * iOwner.mStateSpeedMod * iOwner.GetColdSpeed());
				iOwner.Movement = Vector3.Zero;
				this.mPlayedSound = false;
			}

			// Token: 0x06001058 RID: 4184 RVA: 0x00067188 File Offset: 0x00065388
			public void OnUpdate(float iDeltaTime, Parker iOwner)
			{
				if (NetworkManager.Instance.State == NetworkState.Client)
				{
					return;
				}
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					if (iOwner.mAnimationController.HasFinished)
					{
						iOwner.ChangeState(Parker.States.Battle);
						return;
					}
					if (!this.mPlayedSound)
					{
						AudioManager.Instance.PlayCue(Banks.Additional, Parker.SPAWN_SOUND, iOwner.mAbdomenZone.AudioEmitter);
						this.mPlayedSound = true;
					}
					float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
					this.mTimer -= iDeltaTime;
					if (num >= Parker.SPAWN_TIME[0] & num <= Parker.SPAWN_TIME[1] & this.mTimer <= 0f & this.mCount < 6)
					{
						Vector3 mAbdomPosition = iOwner.mAbdomPosition;
						mAbdomPosition.Z -= 4f;
						float num2 = (float)Parker.RANDOM.NextDouble() * 2f;
						float num3 = (float)Parker.RANDOM.NextDouble() * 6.2831855f;
						float num4 = (float)((double)num2 * Math.Cos((double)num3));
						float num5 = (float)((double)num2 * Math.Sin((double)num3));
						mAbdomPosition.X += num4;
						mAbdomPosition.Z += num5;
						CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(Parker.SpawnState.SPIDERLING_HASH);
						NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(iOwner.mPlayState);
						instance.Initialize(cachedTemplate, mAbdomPosition, 0, 2f);
						Agent ai = instance.AI;
						ai.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, -1, 0, null);
						instance.CharacterBody.Orientation = Matrix.Identity;
						instance.CharacterBody.DesiredDirection = iOwner.mTransform.Forward;
						instance.SpawnAnimation = Magicka.Animations.spawn;
						instance.ChangeState(RessurectionState.Instance);
						iOwner.mPlayState.EntityManager.AddEntity(instance);
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
							triggerActionMessage.ActionType = TriggerActionType.SpawnNPC;
							triggerActionMessage.Handle = instance.Handle;
							triggerActionMessage.Template = instance.Type;
							triggerActionMessage.Id = instance.UniqueID;
							triggerActionMessage.Position = instance.Position;
							triggerActionMessage.Direction = instance.CharacterBody.Direction;
							triggerActionMessage.Bool0 = false;
							triggerActionMessage.Point1 = 170;
							triggerActionMessage.Point2 = 170;
							NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
						}
						this.mCount++;
					}
				}
			}

			// Token: 0x06001059 RID: 4185 RVA: 0x00067412 File Offset: 0x00065612
			public void OnExit(Parker iOwner)
			{
				this.mWeightTimer = 20f;
			}

			// Token: 0x0600105A RID: 4186 RVA: 0x00067420 File Offset: 0x00065620
			public float GetWeight(Parker iOwner, float iDeltaTime)
			{
				if (iOwner.mCurrentState == Parker.States.Spawn)
				{
					return 0f;
				}
				this.mWeightTimer -= iDeltaTime;
				if (this.mWeightTimer > 0f)
				{
					return 0f;
				}
				if (iOwner.mCurrentState != Parker.States.Battle)
				{
					return 0f;
				}
				TriggerArea triggerArea = iOwner.mPlayState.Level.CurrentScene.GetTriggerArea(Parker.SpawnState.ANY);
				int count = triggerArea.GetCount(Parker.SpawnState.SPIDERLING_HASH);
				if (count >= 6)
				{
					return 0f;
				}
				for (int i = 0; i < iOwner.mPlayers.Length; i++)
				{
					if (iOwner.mPlayers[i].Playing && iOwner.mPlayers[i].Avatar != null && !iOwner.mPlayers[i].Avatar.Dead && Parker.GetDistanceSquared(iOwner, iOwner.mPlayers[i].Avatar) < 64f)
					{
						return 0f;
					}
				}
				this.mWeightTimer = 20f;
				return 1f;
			}

			// Token: 0x04000F15 RID: 3861
			public const int COUNT = 6;

			// Token: 0x04000F16 RID: 3862
			private static readonly float SPAWNING_TIME = Parker.SPAWN_TIME[1] - Parker.SPAWN_TIME[0];

			// Token: 0x04000F17 RID: 3863
			private static readonly int SPIDERLING_HASH = "spider_baby".GetHashCode();

			// Token: 0x04000F18 RID: 3864
			private float mTimer;

			// Token: 0x04000F19 RID: 3865
			private float mSpawnTime;

			// Token: 0x04000F1A RID: 3866
			private int mCount;

			// Token: 0x04000F1B RID: 3867
			private bool mPlayedSound;

			// Token: 0x04000F1C RID: 3868
			private float mWeightTimer;

			// Token: 0x04000F1D RID: 3869
			private static readonly int ANY = "any".GetHashCodeCustom();
		}

		// Token: 0x020001EE RID: 494
		protected class RotateState : Parker.IParkerState, IBossState<Parker>
		{
			// Token: 0x0600105D RID: 4189 RVA: 0x00067550 File Offset: 0x00065750
			public void OnEnter(Parker iOwner)
			{
				Vector3 forward = iOwner.mTransform.Forward;
				Vector3 mTargetRotateDirection = iOwner.mTargetRotateDirection;
				float num;
				Parker.GetAngle(ref forward, ref mTargetRotateDirection, out num);
				if (Math.Abs(num) < 0.05f)
				{
					iOwner.ChangeState(iOwner.mRotationTransitionState);
					return;
				}
				if (num > 0f)
				{
					iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[5], 0.25f, true);
					return;
				}
				iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[6], 0.25f, true);
			}

			// Token: 0x0600105E RID: 4190 RVA: 0x000675D0 File Offset: 0x000657D0
			public void OnUpdate(float iDeltaTime, Parker iOwner)
			{
				iOwner.Movement = default(Vector3);
				if (iOwner.mAnimationController.CrossFadeEnabled)
				{
					return;
				}
				Vector3 forward = iOwner.mTransform.Forward;
				Vector3 mTargetRotateDirection = iOwner.mTargetRotateDirection;
				float num;
				Parker.GetAngle(ref forward, ref mTargetRotateDirection, out num);
				if (Math.Abs(num) < 0.05f)
				{
					iOwner.ChangeState(iOwner.mRotationTransitionState);
				}
				float num2 = MathHelper.ToRadians(60f * iOwner.mStageSpeedMod * iOwner.mStateSpeedMod);
				float num3;
				if (num < 0f)
				{
					num3 = -num2;
				}
				else
				{
					num3 = num2;
				}
				Quaternion quaternion;
				Quaternion.CreateFromYawPitchRoll(num3 * iDeltaTime, 0f, 0f, out quaternion);
				Vector3 vector;
				Vector3.Transform(ref forward, ref quaternion, out vector);
				Vector3 translation = iOwner.mTransform.Translation;
				Matrix.Transform(ref iOwner.mTransform, ref quaternion, out iOwner.mTransform);
				iOwner.mTransform.Translation = translation;
			}

			// Token: 0x0600105F RID: 4191 RVA: 0x000676A9 File Offset: 0x000658A9
			public void OnExit(Parker iOwner)
			{
				iOwner.mStateSpeedMod = 1f;
			}

			// Token: 0x06001060 RID: 4192 RVA: 0x000676B6 File Offset: 0x000658B6
			public float GetWeight(Parker iOwner, float iDeltaTime)
			{
				return 0f;
			}

			// Token: 0x04000F1E RID: 3870
			private const float THRESHOLD = 0.001f;
		}

		// Token: 0x020001EF RID: 495
		protected class MoveToCaveState : Parker.IParkerState, IBossState<Parker>
		{
			// Token: 0x06001062 RID: 4194 RVA: 0x000676C5 File Offset: 0x000658C5
			public void OnEnter(Parker iOwner)
			{
				iOwner.mStateSpeedMod = 1f;
				iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[0], 0.25f, true);
			}

			// Token: 0x06001063 RID: 4195 RVA: 0x000676EC File Offset: 0x000658EC
			public unsafe void OnUpdate(float iDeltaTime, Parker iOwner)
			{
				Vector3 translation = iOwner.mTransform.Translation;
				Vector3 translation2 = iOwner.mSpawnTransform.Translation;
				translation.Y = (translation2.Y = 0f);
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					iOwner.mMovementSpeedMod = 1f;
				}
				else
				{
					iOwner.mMovementSpeedMod = 0f;
				}
				if (!MagickaMath.IsApproximately(ref translation, ref translation2, 0.01f))
				{
					Vector3 movement;
					Vector3.Subtract(ref translation2, ref translation, out movement);
					float mStateSpeedMod = Math.Min(Math.Max(movement.Length(), 0.25f), 1f);
					iOwner.mStateSpeedMod = mStateSpeedMod;
					movement.Normalize();
					iOwner.Movement = movement;
					return;
				}
				if (NetworkManager.Instance.State != NetworkState.Client)
				{
					iOwner.mTargetRotateDirection = iOwner.mSpawnTransform.Forward;
					iOwner.mRotationTransitionState = Parker.States.EnterCave;
					if (NetworkManager.Instance.State == NetworkState.Server)
					{
						Parker.RotationTransitionMessage rotationTransitionMessage = default(Parker.RotationTransitionMessage);
						rotationTransitionMessage.Direction = new Normalized101010(iOwner.mTargetRotateDirection);
						rotationTransitionMessage.State = iOwner.mRotationTransitionState;
						BossFight.Instance.SendMessage<Parker.RotationTransitionMessage>(iOwner, 5, (void*)(&rotationTransitionMessage), true);
					}
					iOwner.ChangeState(Parker.States.Rotate);
				}
			}

			// Token: 0x06001064 RID: 4196 RVA: 0x0006780C File Offset: 0x00065A0C
			public void OnExit(Parker iOwner)
			{
				iOwner.mMovementSpeedMod = 1f;
				iOwner.mAnimationSpeedMod = 1f;
				iOwner.mStateSpeedMod = 1f;
				iOwner.Movement = Vector3.Zero;
			}

			// Token: 0x06001065 RID: 4197 RVA: 0x0006783A File Offset: 0x00065A3A
			public float GetWeight(Parker iOwner, float iDeltaTime)
			{
				return 0f;
			}
		}

		// Token: 0x020001F0 RID: 496
		protected class LeaveCaveState : Parker.IParkerState, IBossState<Parker>
		{
			// Token: 0x06001067 RID: 4199 RVA: 0x00067849 File Offset: 0x00065A49
			public void OnEnter(Parker iOwner)
			{
				this.mPlaySound = false;
				iOwner.mStateSpeedMod = 1f;
				iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[19], 0.5f, false);
			}

			// Token: 0x06001068 RID: 4200 RVA: 0x00067878 File Offset: 0x00065A78
			public void OnUpdate(float iDeltaTime, Parker iOwner)
			{
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					if (!this.mPlaySound)
					{
						AudioManager.Instance.PlayCue(Banks.Additional, Parker.EXIT_CAVE_SOUND, iOwner.mAbdomenZone.AudioEmitter);
						this.mPlaySound = true;
						return;
					}
					if (iOwner.mAnimationController.HasFinished)
					{
						iOwner.ChangeState(Parker.States.MoveToBattle);
					}
				}
			}

			// Token: 0x06001069 RID: 4201 RVA: 0x000678D7 File Offset: 0x00065AD7
			public void OnExit(Parker iOwner)
			{
			}

			// Token: 0x0600106A RID: 4202 RVA: 0x000678D9 File Offset: 0x00065AD9
			public float GetWeight(Parker iOwner, float iDeltaTime)
			{
				return 0f;
			}

			// Token: 0x04000F1F RID: 3871
			private bool mPlaySound;
		}

		// Token: 0x020001F1 RID: 497
		protected class MoveToBattleState : Parker.IParkerState, IBossState<Parker>
		{
			// Token: 0x0600106C RID: 4204 RVA: 0x000678E8 File Offset: 0x00065AE8
			public void OnEnter(Parker iOwner)
			{
				iOwner.ResetBattleTransform();
				iOwner.mStateSpeedMod = 1f;
				iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[1], 0.2f, true);
				iOwner.mMovementSpeedMod = 0f;
			}

			// Token: 0x0600106D RID: 4205 RVA: 0x00067920 File Offset: 0x00065B20
			public void OnUpdate(float iDeltaTime, Parker iOwner)
			{
				if (iOwner.mAnimationController.CrossFadeEnabled)
				{
					iOwner.mStateSpeedMod = 1f;
					iOwner.Movement = Vector3.Zero;
					return;
				}
				if (iOwner.mAnimationController.AnimationClip != iOwner.mAnimationClips[1])
				{
					iOwner.ChangeState(Parker.States.Battle);
					return;
				}
				Vector3 translation = iOwner.mTransform.Translation;
				Vector3 translation2 = iOwner.mBattleTransform.Translation;
				translation.Y = 0f;
				translation2.Y = 0f;
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					iOwner.mMovementSpeedMod = 1f;
				}
				else
				{
					iOwner.mMovementSpeedMod = 0f;
				}
				if (!MagickaMath.IsApproximately(ref translation, ref translation2, 0.01f))
				{
					Vector3 movement;
					Vector3.Subtract(ref translation2, ref translation, out movement);
					float mStateSpeedMod = Math.Min(Math.Max(movement.Length(), 0.25f), 1f);
					iOwner.mStateSpeedMod = mStateSpeedMod;
					movement.Normalize();
					iOwner.Movement = movement;
					return;
				}
				iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[4], 0.4f, false);
			}

			// Token: 0x0600106E RID: 4206 RVA: 0x00067A2C File Offset: 0x00065C2C
			public void OnExit(Parker iOwner)
			{
				iOwner.mMovementSpeedMod = 1f;
				iOwner.mAnimationSpeedMod = 1f;
				iOwner.mStateSpeedMod = 1f;
				iOwner.Movement = Vector3.Zero;
			}

			// Token: 0x0600106F RID: 4207 RVA: 0x00067A5A File Offset: 0x00065C5A
			public float GetWeight(Parker iOwner, float iDeltaTime)
			{
				return 0f;
			}
		}

		// Token: 0x020001F2 RID: 498
		protected class EnterCaveState : Parker.IParkerState, IBossState<Parker>
		{
			// Token: 0x06001071 RID: 4209 RVA: 0x00067A6C File Offset: 0x00065C6C
			public void OnEnter(Parker iOwner)
			{
				iOwner.mStateSpeedMod = 1f;
				iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[15], 0.5f, false);
				this.mPlayedSound = false;
				for (int i = 0; i < iOwner.GetStatusEffects().Length; i++)
				{
					iOwner.GetStatusEffects()[i].Stop();
				}
			}

			// Token: 0x06001072 RID: 4210 RVA: 0x00067ACC File Offset: 0x00065CCC
			public void OnUpdate(float iDeltaTime, Parker iOwner)
			{
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					if (!this.mPlayedSound)
					{
						AudioManager.Instance.PlayCue(Banks.Additional, Parker.ENTER_CAVE_SOUND, iOwner.mAbdomenZone.AudioEmitter);
						this.mPlayedSound = true;
						return;
					}
					if (iOwner.mAnimationController.HasFinished)
					{
						iOwner.ChangeState(Parker.States.Cave);
					}
				}
			}

			// Token: 0x06001073 RID: 4211 RVA: 0x00067B2B File Offset: 0x00065D2B
			public void OnExit(Parker iOwner)
			{
				iOwner.mStateSpeedMod = 1f;
			}

			// Token: 0x06001074 RID: 4212 RVA: 0x00067B38 File Offset: 0x00065D38
			public float GetWeight(Parker iOwner, float iDeltaTime)
			{
				return 0f;
			}

			// Token: 0x04000F20 RID: 3872
			private bool mPlayedSound;
		}

		// Token: 0x020001F3 RID: 499
		protected class CaveState : Parker.IParkerState, IBossState<Parker>
		{
			// Token: 0x1700042C RID: 1068
			// (get) Token: 0x06001076 RID: 4214 RVA: 0x00067B47 File Offset: 0x00065D47
			public float Timer
			{
				get
				{
					return this.mTimer;
				}
			}

			// Token: 0x06001077 RID: 4215 RVA: 0x00067B50 File Offset: 0x00065D50
			public void OnEnter(Parker iOwner)
			{
				iOwner.mStateSpeedMod = 1f;
				iOwner.Movement = Vector3.Zero;
				iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[17], 0.25f, true);
				this.mModifier = 1f + (3f - iOwner.mTimeBetweenActions);
				this.mTimer = this.mModifier;
				for (int i = 0; i < iOwner.GetStatusEffects().Length; i++)
				{
					iOwner.GetStatusEffects()[i].Stop();
				}
			}

			// Token: 0x06001078 RID: 4216 RVA: 0x00067BD8 File Offset: 0x00065DD8
			public void OnUpdate(float iDeltaTime, Parker iOwner)
			{
				if (!iOwner.mPlayersDead)
				{
					this.mTimer -= iDeltaTime;
				}
				iOwner.Damage(iDeltaTime * -425f * this.mModifier, Elements.Life);
				if (this.mTimer <= 0f)
				{
					if (iOwner.GetCallMinionState.GetWeight(iOwner, iDeltaTime) > 0f)
					{
						iOwner.ChangeState(Parker.States.CallMinions);
						return;
					}
					iOwner.ChangeState(Parker.States.LeaveCave);
				}
			}

			// Token: 0x06001079 RID: 4217 RVA: 0x00067C42 File Offset: 0x00065E42
			public void OnExit(Parker iOwner)
			{
			}

			// Token: 0x0600107A RID: 4218 RVA: 0x00067C44 File Offset: 0x00065E44
			public float GetWeight(Parker iOwner, float iDeltaTime)
			{
				return 0f;
			}

			// Token: 0x04000F21 RID: 3873
			private float mModifier;

			// Token: 0x04000F22 RID: 3874
			private float mTimer;
		}

		// Token: 0x020001F4 RID: 500
		protected class CallMinionState : Parker.IParkerState, IBossState<Parker>
		{
			// Token: 0x1700042D RID: 1069
			// (get) Token: 0x0600107C RID: 4220 RVA: 0x00067C53 File Offset: 0x00065E53
			public int Count
			{
				get
				{
					if (this.mNrOfSpawns == 0)
					{
						this.mNrOfSpawns = 3 + (Game.Instance.PlayerCount - 1);
					}
					return this.mNrOfSpawns;
				}
			}

			// Token: 0x0600107D RID: 4221 RVA: 0x00067C78 File Offset: 0x00065E78
			public void OnEnter(Parker iOwner)
			{
				iOwner.Movement = default(Vector3);
				iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[16], 0.25f, false);
				this.mSpawnTime = iOwner.mAnimationClips[15].Duration * Parker.CallMinionState.CALL_TIME * 1f / ((float)this.mNrOfSpawns * iOwner.mStageSpeedMod * iOwner.mStateSpeedMod * iOwner.GetColdSpeed());
				this.mCount = 0;
				this.mTimer = 0f;
				this.mPlayedSound = false;
			}

			// Token: 0x0600107E RID: 4222 RVA: 0x00067D04 File Offset: 0x00065F04
			public void OnUpdate(float iDeltaTime, Parker iOwner)
			{
				if (NetworkManager.Instance.State == NetworkState.Client)
				{
					return;
				}
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					if (iOwner.mAnimationController.HasFinished)
					{
						iOwner.ChangeState(Parker.States.LeaveCave);
						return;
					}
					if (!this.mPlayedSound)
					{
						AudioManager.Instance.PlayCue(Banks.Additional, Parker.CALL_MINION_SOUND, iOwner.mAbdomenZone.AudioEmitter);
						this.mPlayedSound = true;
					}
					float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
					this.mTimer -= iDeltaTime;
					if (num >= Parker.SPAWN_TIME[0] & num <= Parker.SPAWN_TIME[1] & this.mTimer <= 0f & this.mCount < this.mNrOfSpawns)
					{
						NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(iOwner.mPlayState);
						Vector3 iPosition = iOwner.mTransform.Translation;
						Avatar avatar;
						if (iOwner.GetRandomTarget(out avatar))
						{
							iPosition = avatar.Position;
						}
						else
						{
							Matrix matrix;
							iOwner.mPlayState.Level.CurrentScene.GetLocator(Parker.CallMinionState.SPAWNS[Parker.RANDOM.Next(Parker.CallMinionState.SPAWNS.Length)], out matrix);
							iPosition = matrix.Translation;
						}
						float num2 = (float)Parker.RANDOM.NextDouble() * 6.2831855f;
						float num3 = (float)Math.Sqrt(Parker.RANDOM.NextDouble());
						CharacterTemplate cachedTemplate;
						if (this.mCount < 2)
						{
							cachedTemplate = CharacterTemplate.GetCachedTemplate(Parker.CallMinionState.POISON_HASH);
							num3 = 4f + num3 * 3f;
							float num4 = (float)((double)num3 * Math.Cos((double)num2));
							float num5 = (float)((double)num3 * Math.Sin((double)num2));
							iPosition.X += num4;
							iPosition.Z += num5;
						}
						else
						{
							cachedTemplate = CharacterTemplate.GetCachedTemplate(Parker.CallMinionState.FOREST_HASH);
							num3 = 2f + num3 * 2f;
							float num6 = (float)((double)num3 * Math.Cos((double)num2));
							float num7 = (float)((double)num3 * Math.Sin((double)num2));
							iPosition.X += num6;
							iPosition.Z += num7;
						}
						Vector3 vector;
						iOwner.mPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref iPosition, out vector, MovementProperties.Default);
						iPosition = vector;
						instance.Initialize(cachedTemplate, iPosition, 0, 5f);
						Agent ai = instance.AI;
						ai.SetOrder(Order.Idle, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, -1, 0, null);
						instance.CharacterBody.Orientation = Matrix.Identity;
						instance.CharacterBody.DesiredDirection = iOwner.mTransform.Forward;
						instance.SpawnAnimation = Magicka.Animations.spawn;
						instance.ChangeState(RessurectionState.Instance);
						iOwner.mPlayState.EntityManager.AddEntity(instance);
						this.mTimer += this.mSpawnTime;
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
							triggerActionMessage.ActionType = TriggerActionType.SpawnNPC;
							triggerActionMessage.Handle = instance.Handle;
							triggerActionMessage.Template = instance.Type;
							triggerActionMessage.Id = instance.UniqueID;
							triggerActionMessage.Position = instance.Position;
							triggerActionMessage.Direction = instance.CharacterBody.Direction;
							triggerActionMessage.Bool0 = false;
							triggerActionMessage.Point2 = 170;
							NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
						}
						this.mCount++;
					}
				}
			}

			// Token: 0x0600107F RID: 4223 RVA: 0x00068065 File Offset: 0x00066265
			public void OnExit(Parker iOwner)
			{
				this.mWeightTimer = 0f;
			}

			// Token: 0x06001080 RID: 4224 RVA: 0x00068074 File Offset: 0x00066274
			public float GetWeight(Parker iOwner, float iDeltaTime)
			{
				if (iOwner.mCurrentState != Parker.States.Cave || iOwner.mCurrentState == Parker.States.CallMinions || iOwner.mPreviousState == Parker.States.CallMinions)
				{
					return 0f;
				}
				if (iOwner.mPlayersDead)
				{
					return 0f;
				}
				if (iOwner.GetCaveState.Timer < 0.5f)
				{
					return 0f;
				}
				this.mWeightTimer -= iDeltaTime;
				if (this.mWeightTimer <= 0f)
				{
					TriggerArea triggerArea = iOwner.mPlayState.Level.CurrentScene.GetTriggerArea(Parker.CallMinionState.ANY);
					int count = triggerArea.GetCount(Parker.CallMinionState.FOREST_HASH);
					int count2 = triggerArea.GetCount(Parker.CallMinionState.POISON_HASH);
					this.mWeightTimer = 0.25f;
					if ((double)(count + count2) < (double)this.Count * 1.5)
					{
						return 1f;
					}
				}
				return 0f;
			}

			// Token: 0x04000F23 RID: 3875
			private static readonly float CALL_TIME = Parker.MINION_TIME[1] - Parker.MINION_TIME[0];

			// Token: 0x04000F24 RID: 3876
			private static readonly int FOREST_HASH = "spider_forest".GetHashCodeCustom();

			// Token: 0x04000F25 RID: 3877
			private static readonly int POISON_HASH = "spider_poison".GetHashCodeCustom();

			// Token: 0x04000F26 RID: 3878
			private float mTimer;

			// Token: 0x04000F27 RID: 3879
			private float mSpawnTime;

			// Token: 0x04000F28 RID: 3880
			private int mCount;

			// Token: 0x04000F29 RID: 3881
			private int mNrOfSpawns;

			// Token: 0x04000F2A RID: 3882
			private static readonly int[] SPAWNS = new int[]
			{
				"boss_start0".GetHashCodeCustom(),
				"boss_start1".GetHashCodeCustom(),
				"boss_start2".GetHashCodeCustom(),
				"boss_start3".GetHashCodeCustom()
			};

			// Token: 0x04000F2B RID: 3883
			private bool mPlayedSound;

			// Token: 0x04000F2C RID: 3884
			private float mWeightTimer;

			// Token: 0x04000F2D RID: 3885
			private static readonly int ANY = "any".GetHashCodeCustom();
		}

		// Token: 0x020001F5 RID: 501
		protected class DeadState : Parker.IParkerState, IBossState<Parker>
		{
			// Token: 0x06001083 RID: 4227 RVA: 0x000681DF File Offset: 0x000663DF
			public void OnEnter(Parker iOwner)
			{
				this.mPlayedSound = false;
				iOwner.Movement = Vector3.Zero;
				iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[22], 0.125f, false);
			}

			// Token: 0x06001084 RID: 4228 RVA: 0x00068210 File Offset: 0x00066410
			public void OnUpdate(float iDeltaTime, Parker iOwner)
			{
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					if (!this.mPlayedSound)
					{
						AudioManager.Instance.PlayCue(Banks.Additional, Parker.DEATH_SOUND, iOwner.mAbdomenZone.AudioEmitter);
						this.mPlayedSound = true;
					}
					if (iOwner.mAnimationController.HasFinished)
					{
						iOwner.mDead = true;
					}
				}
			}

			// Token: 0x06001085 RID: 4229 RVA: 0x0006826D File Offset: 0x0006646D
			public void OnExit(Parker iOwner)
			{
			}

			// Token: 0x06001086 RID: 4230 RVA: 0x0006826F File Offset: 0x0006646F
			public float GetWeight(Parker iOwner, float iDeltaTime)
			{
				return 0f;
			}

			// Token: 0x04000F2E RID: 3886
			private bool mPlayedSound;
		}
	}
}
