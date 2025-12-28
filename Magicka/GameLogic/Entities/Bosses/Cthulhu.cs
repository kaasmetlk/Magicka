using System;
using System.Collections.Generic;
using System.Linq;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using Magicka.AI;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.Entities.Buffs;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Graphics.Effects;
using Magicka.Levels;
using Magicka.Levels.Triggers;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using PolygonHead;
using PolygonHead.Effects;
using XNAnimation;
using XNAnimation.Controllers;

namespace Magicka.GameLogic.Entities.Bosses
{
	// Token: 0x02000101 RID: 257
	public class Cthulhu : BossStatusEffected, IBossSpellCaster, IBoss
	{
		// Token: 0x1700018A RID: 394
		// (get) Token: 0x060007A3 RID: 1955 RVA: 0x0002FDAF File Offset: 0x0002DFAF
		private Cthulhu.IdleState GetIdleState
		{
			get
			{
				return this.mStates[0] as Cthulhu.IdleState;
			}
		}

		// Token: 0x1700018B RID: 395
		// (get) Token: 0x060007A4 RID: 1956 RVA: 0x0002FDBE File Offset: 0x0002DFBE
		private Cthulhu.EmergeState GetEmergeState
		{
			get
			{
				return this.mStates[1] as Cthulhu.EmergeState;
			}
		}

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x060007A5 RID: 1957 RVA: 0x0002FDCD File Offset: 0x0002DFCD
		private Cthulhu.SubmergeState GetSubmergeState
		{
			get
			{
				return this.mStates[2] as Cthulhu.SubmergeState;
			}
		}

		// Token: 0x1700018D RID: 397
		// (get) Token: 0x060007A6 RID: 1958 RVA: 0x0002FDDC File Offset: 0x0002DFDC
		private Cthulhu.DevourState GetDevourState
		{
			get
			{
				return this.mStates[3] as Cthulhu.DevourState;
			}
		}

		// Token: 0x1700018E RID: 398
		// (get) Token: 0x060007A7 RID: 1959 RVA: 0x0002FDEB File Offset: 0x0002DFEB
		private Cthulhu.DevourHitState GetDevourHitState
		{
			get
			{
				return this.mStates[4] as Cthulhu.DevourHitState;
			}
		}

		// Token: 0x1700018F RID: 399
		// (get) Token: 0x060007A8 RID: 1960 RVA: 0x0002FDFA File Offset: 0x0002DFFA
		private Cthulhu.LightningState GetLightningState
		{
			get
			{
				return this.mStates[5] as Cthulhu.LightningState;
			}
		}

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x060007A9 RID: 1961 RVA: 0x0002FE09 File Offset: 0x0002E009
		private Cthulhu.MistState GetMistState
		{
			get
			{
				return this.mStates[6] as Cthulhu.MistState;
			}
		}

		// Token: 0x17000191 RID: 401
		// (get) Token: 0x060007AA RID: 1962 RVA: 0x0002FE18 File Offset: 0x0002E018
		private Cthulhu.CallState GetCallofCthulhuState
		{
			get
			{
				return this.mStates[7] as Cthulhu.CallState;
			}
		}

		// Token: 0x17000192 RID: 402
		// (get) Token: 0x060007AB RID: 1963 RVA: 0x0002FE27 File Offset: 0x0002E027
		private Cthulhu.LesserCallState GetLesserCallofCthulhuState
		{
			get
			{
				return this.mStates[8] as Cthulhu.LesserCallState;
			}
		}

		// Token: 0x17000193 RID: 403
		// (get) Token: 0x060007AC RID: 1964 RVA: 0x0002FE36 File Offset: 0x0002E036
		private Cthulhu.TimewarpState GetTimewarpState
		{
			get
			{
				return this.mStates[9] as Cthulhu.TimewarpState;
			}
		}

		// Token: 0x17000194 RID: 404
		// (get) Token: 0x060007AD RID: 1965 RVA: 0x0002FE46 File Offset: 0x0002E046
		private Cthulhu.HypnotizeState GetHypnotizeState
		{
			get
			{
				return this.mStates[10] as Cthulhu.HypnotizeState;
			}
		}

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x060007AE RID: 1966 RVA: 0x0002FE56 File Offset: 0x0002E056
		private Cthulhu.RageState GetRageState
		{
			get
			{
				return this.mStates[11] as Cthulhu.RageState;
			}
		}

		// Token: 0x17000196 RID: 406
		// (get) Token: 0x060007AF RID: 1967 RVA: 0x0002FE66 File Offset: 0x0002E066
		private Cthulhu.OtherworldlyBoltState GetOtherworldlyBoltState
		{
			get
			{
				return this.mStates[12] as Cthulhu.OtherworldlyBoltState;
			}
		}

		// Token: 0x060007B0 RID: 1968 RVA: 0x0002FE78 File Offset: 0x0002E078
		public Cthulhu(PlayState iPlayState)
		{
			this.mPlayState = iPlayState;
			if (!(RenderManager.Instance.GetEffect(SkinnedModelDeferredNormalMappedEffect.TYPEHASH) is SkinnedModelDeferredNormalMappedEffect))
			{
				SkinnedModelDeferredNormalMappedEffect.DefaultEffectPool = RenderManager.Instance.GlobalDummyEffect.EffectPool;
				SkinnedModelDeferredNormalMappedEffect iEffect = new SkinnedModelDeferredNormalMappedEffect(Game.Instance.GraphicsDevice, SkinnedModelDeferredNormalMappedEffect.DefaultEffectPool);
				RenderManager.Instance.RegisterEffect(iEffect);
			}
			SkinnedModel skinnedModel = null;
			SkinnedModel skinnedModel2 = null;
			this.mOccupiedSpawnPoints = new int[Cthulhu.SPAWN_LOCATORS.Length];
			for (int i = 0; i < this.mOccupiedSpawnPoints.Length; i++)
			{
				this.mOccupiedSpawnPoints[i] = -1;
			}
			lock (Game.Instance.GraphicsDevice)
			{
				skinnedModel = iPlayState.Content.Load<SkinnedModel>("Models/Bosses/Cthulhu/Cthulhu_mesh");
				skinnedModel2 = iPlayState.Content.Load<SkinnedModel>("Models/Bosses/Cthulhu/Cthulhu_animations");
			}
			this.mRadius = skinnedModel.Model.Meshes[0].BoundingSphere.Radius;
			this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/deep_one");
			this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/starspawn");
			this.mAnimationController = new AnimationController();
			this.mAnimationController.Skeleton = skinnedModel2.SkeletonBones;
			this.mAnimations = new AnimationClip[16];
			this.mAnimations[0] = skinnedModel2.AnimationClips["cast_bolt_begin"];
			this.mAnimations[1] = skinnedModel2.AnimationClips["cast_bolt_mid"];
			this.mAnimations[2] = skinnedModel2.AnimationClips["cast_bolt_end"];
			this.mAnimations[3] = skinnedModel2.AnimationClips["submerge"];
			this.mAnimations[4] = skinnedModel2.AnimationClips["mists"];
			this.mAnimations[5] = skinnedModel2.AnimationClips["timewarp"];
			this.mAnimations[6] = skinnedModel2.AnimationClips["mesmerize"];
			this.mAnimations[7] = skinnedModel2.AnimationClips["madness"];
			this.mAnimations[8] = skinnedModel2.AnimationClips["emerge"];
			this.mAnimations[9] = skinnedModel2.AnimationClips["idle"];
			this.mAnimations[10] = skinnedModel2.AnimationClips["devour"];
			this.mAnimations[11] = skinnedModel2.AnimationClips["devour_hit"];
			this.mAnimations[15] = skinnedModel2.AnimationClips["rage"];
			this.mAnimations[12] = skinnedModel2.AnimationClips["cast_lightning"];
			this.mAnimations[13] = skinnedModel2.AnimationClips["call_of_cthulhu"];
			this.mAnimations[14] = skinnedModel2.AnimationClips["die"];
			this.mCustomColdEffectRef = new VisualEffectReference[5];
			this.mRenderData = new Cthulhu.RenderData[3];
			for (int j = 0; j < 3; j++)
			{
				this.mRenderData[j] = new Cthulhu.RenderData(skinnedModel2.SkeletonBones.Count, skinnedModel.Model.Meshes[0], skinnedModel.Model.Meshes[0].MeshParts[0]);
			}
			int num = 0;
			Matrix matrix;
			Matrix.CreateRotationY(3.1415927f, out matrix);
			for (int k = 0; k < skinnedModel2.SkeletonBones.Count; k++)
			{
				SkinnedModelBone skinnedModelBone = skinnedModel2.SkeletonBones[k];
				if (skinnedModelBone.Name.Equals("head", StringComparison.OrdinalIgnoreCase))
				{
					this.mHeadIndex = (int)skinnedModelBone.Index;
					this.mHeadBindPose = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mHeadBindPose, ref matrix, out this.mHeadBindPose);
					Matrix.Invert(ref this.mHeadBindPose, out this.mHeadBindPose);
				}
				else if (skinnedModelBone.Name.Equals("rightthumb2", StringComparison.OrdinalIgnoreCase))
				{
					this.mRightFingerIndex[0] = (int)skinnedModelBone.Index;
					Matrix inverseBindPoseTransform = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref inverseBindPoseTransform, ref matrix, out inverseBindPoseTransform);
					Matrix.Invert(ref inverseBindPoseTransform, out inverseBindPoseTransform);
					this.mRightFingerBindPose[0] = inverseBindPoseTransform;
				}
				else if (skinnedModelBone.Name.Equals("rightfinger1_3", StringComparison.OrdinalIgnoreCase))
				{
					this.mRightFingerIndex[1] = (int)skinnedModelBone.Index;
					Matrix inverseBindPoseTransform2 = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref inverseBindPoseTransform2, ref matrix, out inverseBindPoseTransform2);
					Matrix.Invert(ref inverseBindPoseTransform2, out inverseBindPoseTransform2);
					this.mRightFingerBindPose[1] = inverseBindPoseTransform2;
				}
				else if (skinnedModelBone.Name.Equals("rightfinger2_3", StringComparison.OrdinalIgnoreCase))
				{
					this.mRightFingerIndex[2] = (int)skinnedModelBone.Index;
					Matrix inverseBindPoseTransform3 = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref inverseBindPoseTransform3, ref matrix, out inverseBindPoseTransform3);
					Matrix.Invert(ref inverseBindPoseTransform3, out inverseBindPoseTransform3);
					this.mRightFingerBindPose[2] = inverseBindPoseTransform3;
				}
				else if (skinnedModelBone.Name.Equals("rightfinger3_3", StringComparison.OrdinalIgnoreCase))
				{
					this.mRightFingerIndex[3] = (int)skinnedModelBone.Index;
					Matrix inverseBindPoseTransform4 = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref inverseBindPoseTransform4, ref matrix, out inverseBindPoseTransform4);
					Matrix.Invert(ref inverseBindPoseTransform4, out inverseBindPoseTransform4);
					this.mRightFingerBindPose[3] = inverseBindPoseTransform4;
					this.mRightHandIndex = (int)skinnedModelBone.Index;
					this.mRightHandBindPose = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mRightHandBindPose, ref matrix, out this.mRightHandBindPose);
					Matrix.Invert(ref this.mRightHandBindPose, out this.mRightHandBindPose);
				}
				else if (skinnedModelBone.Name.Equals("rightfinger4_3", StringComparison.OrdinalIgnoreCase))
				{
					this.mRightFingerIndex[4] = (int)skinnedModelBone.Index;
					Matrix inverseBindPoseTransform5 = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref inverseBindPoseTransform5, ref matrix, out inverseBindPoseTransform5);
					Matrix.Invert(ref inverseBindPoseTransform5, out inverseBindPoseTransform5);
					this.mRightFingerBindPose[4] = inverseBindPoseTransform5;
				}
				else if (skinnedModelBone.Name.Equals("rightfinger5_3", StringComparison.OrdinalIgnoreCase))
				{
					this.mRightFingerIndex[5] = (int)skinnedModelBone.Index;
					Matrix inverseBindPoseTransform6 = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref inverseBindPoseTransform6, ref matrix, out inverseBindPoseTransform6);
					Matrix.Invert(ref inverseBindPoseTransform6, out inverseBindPoseTransform6);
					this.mRightFingerBindPose[5] = inverseBindPoseTransform6;
				}
				else if (skinnedModelBone.Name.Equals("leftthumb2", StringComparison.OrdinalIgnoreCase))
				{
					this.mLeftFingerIndex[0] = (int)skinnedModelBone.Index;
					Matrix inverseBindPoseTransform7 = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref inverseBindPoseTransform7, ref matrix, out inverseBindPoseTransform7);
					Matrix.Invert(ref inverseBindPoseTransform7, out inverseBindPoseTransform7);
					this.mLeftFingerBindPose[0] = inverseBindPoseTransform7;
				}
				else if (skinnedModelBone.Name.Equals("leftfinger1_3", StringComparison.OrdinalIgnoreCase))
				{
					this.mLeftFingerIndex[1] = (int)skinnedModelBone.Index;
					Matrix inverseBindPoseTransform8 = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref inverseBindPoseTransform8, ref matrix, out inverseBindPoseTransform8);
					Matrix.Invert(ref inverseBindPoseTransform8, out inverseBindPoseTransform8);
					this.mLeftFingerBindPose[1] = inverseBindPoseTransform8;
				}
				else if (skinnedModelBone.Name.Equals("leftfinger2_3", StringComparison.OrdinalIgnoreCase))
				{
					this.mLeftFingerIndex[2] = (int)skinnedModelBone.Index;
					Matrix inverseBindPoseTransform9 = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref inverseBindPoseTransform9, ref matrix, out inverseBindPoseTransform9);
					Matrix.Invert(ref inverseBindPoseTransform9, out inverseBindPoseTransform9);
					this.mLeftFingerBindPose[2] = inverseBindPoseTransform9;
				}
				else if (skinnedModelBone.Name.Equals("leftfinger3_3", StringComparison.OrdinalIgnoreCase))
				{
					this.mLeftFingerIndex[3] = (int)skinnedModelBone.Index;
					Matrix inverseBindPoseTransform10 = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref inverseBindPoseTransform10, ref matrix, out inverseBindPoseTransform10);
					Matrix.Invert(ref inverseBindPoseTransform10, out inverseBindPoseTransform10);
					this.mLeftFingerBindPose[3] = inverseBindPoseTransform10;
					this.mLeftHandIndex = (int)skinnedModelBone.Index;
					this.mLeftHandBindPose = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mLeftHandBindPose, ref matrix, out this.mLeftHandBindPose);
					Matrix.Invert(ref this.mLeftHandBindPose, out this.mLeftHandBindPose);
				}
				else if (skinnedModelBone.Name.Equals("leftfinger4_3", StringComparison.OrdinalIgnoreCase))
				{
					this.mLeftFingerIndex[4] = (int)skinnedModelBone.Index;
					Matrix inverseBindPoseTransform11 = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref inverseBindPoseTransform11, ref matrix, out inverseBindPoseTransform11);
					Matrix.Invert(ref inverseBindPoseTransform11, out inverseBindPoseTransform11);
					this.mLeftFingerBindPose[4] = inverseBindPoseTransform11;
				}
				else if (skinnedModelBone.Name.Equals("leftfinger5_3", StringComparison.OrdinalIgnoreCase))
				{
					this.mLeftFingerIndex[5] = (int)skinnedModelBone.Index;
					Matrix inverseBindPoseTransform12 = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref inverseBindPoseTransform12, ref matrix, out inverseBindPoseTransform12);
					Matrix.Invert(ref inverseBindPoseTransform12, out inverseBindPoseTransform12);
					this.mLeftFingerBindPose[5] = inverseBindPoseTransform12;
				}
				else if (skinnedModelBone.Name.Equals("attach", StringComparison.OrdinalIgnoreCase))
				{
					this.mMouthAttachIndex = (int)skinnedModelBone.Index;
					this.mMouthAttachBindPose = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mMouthAttachBindPose, ref matrix, out this.mMouthAttachBindPose);
					Matrix.Invert(ref this.mMouthAttachBindPose, out this.mMouthAttachBindPose);
				}
				else if (skinnedModelBone.Name.Equals("leftshoulder", StringComparison.OrdinalIgnoreCase))
				{
					this.mLeftArmIndex[0] = (int)skinnedModelBone.Index;
					this.mLeftArmBindPose[0] = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mLeftArmBindPose[0], ref matrix, out this.mLeftArmBindPose[0]);
					Matrix.Invert(ref this.mLeftArmBindPose[0], out this.mLeftArmBindPose[0]);
				}
				else if (skinnedModelBone.Name.Equals("leftelbow", StringComparison.OrdinalIgnoreCase))
				{
					this.mLeftArmIndex[1] = (int)skinnedModelBone.Index;
					this.mLeftArmBindPose[1] = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mLeftArmBindPose[1], ref matrix, out this.mLeftArmBindPose[1]);
					Matrix.Invert(ref this.mLeftArmBindPose[1], out this.mLeftArmBindPose[1]);
				}
				else if (skinnedModelBone.Name.Equals("lefthand", StringComparison.OrdinalIgnoreCase))
				{
					this.mLeftArmIndex[2] = (int)skinnedModelBone.Index;
					this.mLeftArmBindPose[2] = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mLeftArmBindPose[2], ref matrix, out this.mLeftArmBindPose[2]);
					Matrix.Invert(ref this.mLeftArmBindPose[2], out this.mLeftArmBindPose[2]);
				}
				else if (skinnedModelBone.Name.Equals("rightshoulder", StringComparison.OrdinalIgnoreCase))
				{
					this.mRightArmIndex[0] = (int)skinnedModelBone.Index;
					this.mRightArmBindPose[0] = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mRightArmBindPose[0], ref matrix, out this.mRightArmBindPose[0]);
					Matrix.Invert(ref this.mRightArmBindPose[0], out this.mRightArmBindPose[0]);
				}
				else if (skinnedModelBone.Name.Equals("rightelbow", StringComparison.OrdinalIgnoreCase))
				{
					this.mRightArmIndex[1] = (int)skinnedModelBone.Index;
					this.mRightArmBindPose[1] = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mRightArmBindPose[1], ref matrix, out this.mRightArmBindPose[1]);
					Matrix.Invert(ref this.mRightArmBindPose[1], out this.mRightArmBindPose[1]);
				}
				else if (skinnedModelBone.Name.Equals("righthand", StringComparison.OrdinalIgnoreCase))
				{
					this.mRightArmIndex[2] = (int)skinnedModelBone.Index;
					this.mRightArmBindPose[2] = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mRightArmBindPose[2], ref matrix, out this.mRightArmBindPose[2]);
					Matrix.Invert(ref this.mRightArmBindPose[2], out this.mRightArmBindPose[2]);
				}
				else
				{
					for (int l = num; l < 5; l++)
					{
						string value = string.Format("spine{0}", l + 1);
						if (skinnedModelBone.Name.Equals(value, StringComparison.OrdinalIgnoreCase))
						{
							this.mSpineIndex[l] = (int)skinnedModelBone.Index;
							this.mSpineBindPose[l] = skinnedModelBone.InverseBindPoseTransform;
							Matrix.Multiply(ref this.mSpineBindPose[l], ref matrix, out this.mSpineBindPose[l]);
							Matrix.Invert(ref this.mSpineBindPose[l], out this.mSpineBindPose[l]);
							num++;
							break;
						}
					}
				}
			}
			this.mDamageZone = new BossSpellCasterZone(this.mPlayState, this, this.mAnimationController, this.mRightHandIndex, this.mLeftHandIndex, 0, 1.75f, new Primitive[]
			{
				new Capsule(Vector3.Zero, Matrix.Identity, 1.2f, 4f)
			});
			Capsule capsule = new Capsule(Vector3.Zero, Matrix.Identity, 0.5f, 1.6f);
			Capsule capsule2 = new Capsule(Vector3.Zero, Matrix.Identity, 0.4f, 4.7f);
			Capsule capsule3 = new Capsule(Vector3.Zero, Matrix.Identity, 0.5f, 1.6f);
			Capsule capsule4 = new Capsule(Vector3.Zero, Matrix.Identity, 0.4f, 4.7f);
			this.mArmZone = new BossCollisionZone(this.mPlayState, this, new Primitive[]
			{
				capsule,
				capsule2,
				capsule3,
				capsule4
			});
			this.mDamageZone.Body.CollisionSkin.callbackFn += this.OnCollision;
			this.mArmZone.Body.CollisionSkin.callbackFn += this.OnCollision;
			this.mActiveTentacles = new List<Tentacle>(4);
			this.mInactiveTentacles = new List<Tentacle>(4);
			this.mTentacles = new Tentacle[4];
			for (int m = 0; m < 4; m++)
			{
				this.mTentacles[m] = new Tentacle(this, (byte)m, m + 1, iPlayState);
			}
			this.mMistCloud = new CthulhuMist(this, (byte)this.mTentacles.Length, this.mTentacles.Length + 1, iPlayState);
			this.mResistances = new Resistance[11];
			for (int n = 0; n < 11; n++)
			{
				this.mResistances[n].Multiplier = 1f;
				this.mResistances[n].Modifier = 0f;
				Elements elements = Spell.ElementFromIndex(n);
				this.mResistances[n].ResistanceAgainst = elements;
				Elements elements2 = elements;
				switch (elements2)
				{
				case Elements.Earth:
					this.mResistances[n].Multiplier = 0.25f;
					this.mResistances[n].Modifier = -500f;
					break;
				case Elements.Water:
					this.mResistances[n].Multiplier = 0f;
					break;
				case Elements.Earth | Elements.Water:
					break;
				case Elements.Cold:
					this.mResistances[n].Multiplier = 0.25f;
					break;
				default:
					if (elements2 == Elements.Poison)
					{
						this.mResistances[n].Multiplier = 0f;
					}
					break;
				}
			}
			this.mMaxHitPoints = 100000f;
			this.mStateSpeedModifier = 1f;
			this.mCultistMissileCache = new List<OtherworldlyBolt>();
			for (int num2 = 0; num2 < 8; num2++)
			{
				this.mCultistMissileCache.Add(new OtherworldlyBolt(this.mPlayState));
			}
		}

		// Token: 0x17000197 RID: 407
		// (get) Token: 0x060007B1 RID: 1969 RVA: 0x00030F9C File Offset: 0x0002F19C
		public float WaterYpos
		{
			get
			{
				return this.mWaterYpos;
			}
		}

		// Token: 0x17000198 RID: 408
		// (get) Token: 0x060007B2 RID: 1970 RVA: 0x00030FA4 File Offset: 0x0002F1A4
		public bool InitialEmerge
		{
			get
			{
				return this.mInitialEmerge;
			}
		}

		// Token: 0x17000199 RID: 409
		// (get) Token: 0x060007B3 RID: 1971 RVA: 0x00030FAC File Offset: 0x0002F1AC
		internal bool OkToFight
		{
			get
			{
				return this.mOkToFight;
			}
		}

		// Token: 0x060007B4 RID: 1972 RVA: 0x00030FB4 File Offset: 0x0002F1B4
		private unsafe bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
		{
			if (iSkin1.Owner == null)
			{
				return false;
			}
			if (iSkin1.Owner.Tag is Shield)
			{
				(iSkin1.Owner.Tag as Shield).Kill();
			}
			if (iSkin1.Owner.Tag is MissileEntity)
			{
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					Cthulhu.ActivateDeflectionMessage activateDeflectionMessage = default(Cthulhu.ActivateDeflectionMessage);
					BossFight.Instance.SendMessage<Cthulhu.ActivateDeflectionMessage>(this, 10, (void*)(&activateDeflectionMessage), true);
				}
				if (NetworkManager.Instance.State != NetworkState.Client)
				{
					this.mDeflectionTimer = 8f;
				}
			}
			return true;
		}

		// Token: 0x060007B5 RID: 1973 RVA: 0x00031043 File Offset: 0x0002F243
		public void Initialize(ref Matrix iOrientation, int iUniqueID)
		{
			this.Initialize(ref iOrientation);
		}

		// Token: 0x060007B6 RID: 1974 RVA: 0x0003104C File Offset: 0x0002F24C
		public void Initialize(ref Matrix iOrientation)
		{
			this.mHitPoints = this.mMaxHitPoints;
			this.mTransform = iOrientation;
			this.mInitialEmerge = true;
			this.mTheKingHasFallen = false;
			for (int i = 0; i < this.mSpawnTransforms.Length; i++)
			{
				this.mPlayState.Level.CurrentScene.GetLocator(Cthulhu.SPAWN_LOCATORS[i], out this.mSpawnTransforms[i]);
			}
			this.mPlayState.Level.CurrentScene.GetLocator(Cthulhu.MIST_LOCATOR, out this.mMistSpawnTransform);
			for (int j = 0; j < this.mDeepOnesSpawnTransforms.Length; j++)
			{
				this.mPlayState.Level.CurrentScene.GetLocator(Cthulhu.DEEP_ONES_SPAWN_LOCATORS[j], out this.mDeepOnesSpawnTransforms[j]);
			}
			this.ClearAllStatusEffects();
			this.mDead = false;
			this.mStarSpawnSpawned = false;
			this.mHitFlashTimer = 0f;
			this.mDamageZone.Initialize();
			this.mDamageZone.Body.CollisionSkin.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
			this.mDamageZone.Body.CollisionSkin.NonCollidables.Add(this.mArmZone.Body.CollisionSkin);
			this.mPlayState.EntityManager.AddEntity(this.mDamageZone);
			this.mArmZone.Initialize();
			this.mArmZone.Body.CollisionSkin.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
			this.mArmZone.Body.CollisionSkin.NonCollidables.Add(this.mDamageZone.Body.CollisionSkin);
			this.mPlayState.EntityManager.AddEntity(this.mArmZone);
			for (int k = 0; k < this.mOccupiedSpawnPoints.Length; k++)
			{
				this.mOccupiedSpawnPoints[k] = -1;
			}
			this.OccupySpawnTransform(Cthulhu.BossSpawnPoints.NORTH, (int)this.mDamageZone.Handle);
			this.mDesiredSpawnPoint = -1;
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				this.mDesiredSpawnPoint = 0;
			}
			this.mActiveTentacles.Clear();
			this.mInactiveTentacles.Clear();
			for (int l = 0; l < this.mTentacles.Length; l++)
			{
				this.mInactiveTentacles.Add(this.mTentacles[l]);
			}
			this.mDeflectionTimer = 0f;
			this.mMistCloud.Initialize();
			this.mHitPoints = this.mMaxHitPoints;
			this.mCurrentStage = Cthulhu.Stages.Intro;
			this.mStages[(int)this.mCurrentStage].OnEnter(this);
			this.mCurrentState = (this.mPreviousState = Cthulhu.States.Emerge);
			this.GetEmergeState.OnEnter(this);
			this.mOkToFight = false;
			Vector3 translation = this.mSpawnTransforms[0].Translation;
			translation.Y = -10f;
			Segment segment = new Segment(translation, new Vector3(0f, 20f, 0f));
			this.mWaterYpos = -2f;
			if (this.mPlayState.Level.CurrentScene.Liquids.Length > 0)
			{
				Liquid liquid = this.mPlayState.Level.CurrentScene.Liquids[0];
				float scaleFactor;
				Vector3 vector;
				Vector3 vector2;
				if (liquid.SegmentIntersect(out scaleFactor, out vector, out vector2, ref segment, false, false, false))
				{
					Vector3 delta = segment.Delta;
					Vector3.Multiply(ref delta, scaleFactor, out delta);
					Vector3.Add(ref translation, ref delta, out translation);
					this.mWaterYpos = translation.Y;
				}
			}
			for (int m = 0; m < this.mCultistMissileCache.Count; m++)
			{
				this.mCultistMissileCache[m].Reset();
			}
		}

		// Token: 0x060007B7 RID: 1975 RVA: 0x000313FC File Offset: 0x0002F5FC
		public void UpdateBoss(DataChannel iDataChannel, float iDeltaTime, bool iFightStarted)
		{
			iDeltaTime /= this.mPlayState.TimeModifier * this.mPlayState.TimeMultiplier;
			if (this.mHitPoints <= 0f && !this.mTheKingHasFallen)
			{
				this.mTheKingHasFallen = true;
				this.ChangeState(Cthulhu.States.Death);
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
			this.UpdateDamage(iDeltaTime);
			this.UpdateStatusEffects(iDeltaTime);
			this.mTimeUntilSubmerge -= iDeltaTime;
			this.mTimeSinceLastEmerge += iDeltaTime;
			this.mTimeSinceLastDamageTimewarp += iDeltaTime;
			this.CheckIfCharactersAreClose(iDeltaTime);
			this.CheckMistDesirability(iDeltaTime);
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				if (this.mCurrentState != Cthulhu.States.Death && this.mCurrentState == Cthulhu.States.Idle)
				{
					float num = this.HitPoints / this.MaxHitPoints;
					if (num > 0.95f)
					{
						if (this.mCurrentStage < Cthulhu.Stages.Intro)
						{
							this.ChangeStage(Cthulhu.Stages.Intro);
						}
					}
					else if (num >= 0.65f)
					{
						if (this.mCurrentStage < Cthulhu.Stages.Battle)
						{
							this.ChangeStage(Cthulhu.Stages.Battle);
						}
					}
					else if (num >= 0.35f)
					{
						if (this.mCurrentStage < Cthulhu.Stages.LateBattle)
						{
							this.ChangeStage(Cthulhu.Stages.LateBattle);
						}
					}
					else if (num > 0.15f)
					{
						if (this.mCurrentStage < Cthulhu.Stages.Critical)
						{
							this.ChangeStage(Cthulhu.Stages.Critical);
						}
					}
					else if (num > 0f && this.mCurrentStage < Cthulhu.Stages.Final)
					{
						this.ChangeStage(Cthulhu.Stages.Final);
					}
				}
				this.mStages[(int)this.mCurrentStage].OnUpdate(iDeltaTime, this);
			}
			this.mStates[(int)this.mCurrentState].OnUpdate(iDeltaTime, this);
			Matrix matrix = Matrix.CreateScale(1.25f) * this.mTransform;
			float slowdown = this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].GetSlowdown();
			float elapsedTime = this.mStageSpeedModifier * this.mStateSpeedModifier * slowdown * iDeltaTime;
			this.mAnimationController.Update(elapsedTime, ref matrix, true);
			float num2;
			if (this.mSpellEffect != null && !this.mSpellEffect.CastUpdate(iDeltaTime, this.mDamageZone, out num2))
			{
				this.mSpellEffect.DeInitialize(this.mDamageZone);
				this.mSpellEffect = null;
			}
			Matrix.Multiply(ref this.mMouthAttachBindPose, ref this.mAnimationController.SkinnedBoneTransforms[this.mMouthAttachIndex], out this.mMouthAttachOrientation);
			Matrix.Multiply(ref this.mHeadBindPose, ref this.mAnimationController.SkinnedBoneTransforms[this.mHeadIndex], out this.mHeadOrientation);
			Matrix.Multiply(ref this.mLeftHandBindPose, ref this.mAnimationController.SkinnedBoneTransforms[this.mLeftHandIndex], out this.mLeftHandOrientation);
			Matrix.Multiply(ref this.mRightHandBindPose, ref this.mAnimationController.SkinnedBoneTransforms[this.mRightHandIndex], out this.mRightHandOrientation);
			for (int i = 0; i < this.mRightFingerBindPose.Length; i++)
			{
				Matrix.Multiply(ref this.mRightFingerBindPose[i], ref this.mAnimationController.SkinnedBoneTransforms[this.mRightFingerIndex[i]], out this.mRightFingerOrientation[i]);
			}
			for (int j = 0; j < this.mLeftFingerBindPose.Length; j++)
			{
				Matrix.Multiply(ref this.mLeftFingerBindPose[j], ref this.mAnimationController.SkinnedBoneTransforms[this.mLeftFingerIndex[j]], out this.mLeftFingerOrientation[j]);
			}
			Vector3 up = Vector3.Up;
			Vector3 zero = Vector3.Zero;
			Vector3 translation = this.mSpineBindPose[0].Translation;
			Vector3.Transform(ref translation, ref this.mAnimationController.SkinnedBoneTransforms[this.mSpineIndex[0]], out translation);
			Vector3 translation2 = this.mSpineBindPose[4].Translation;
			Vector3.Transform(ref translation2, ref this.mAnimationController.SkinnedBoneTransforms[this.mSpineIndex[4]], out translation2);
			Vector3 vector;
			Vector3.Subtract(ref translation, ref translation2, out vector);
			vector.Normalize();
			Transform transform = default(Transform);
			Matrix.CreateWorld(ref zero, ref vector, ref up, out transform.Orientation);
			transform.Position = translation;
			this.mDamageZone.SetOrientation(ref transform.Position, ref transform.Orientation);
			this.mDamageZone.Update(iDataChannel, iDeltaTime);
			for (int k = 0; k < 2; k++)
			{
				translation = this.mLeftArmBindPose[k].Translation;
				Vector3.Transform(ref translation, ref this.mAnimationController.SkinnedBoneTransforms[this.mLeftArmIndex[k]], out translation);
				translation2 = this.mLeftArmBindPose[k + 1].Translation;
				Vector3.Transform(ref translation2, ref this.mAnimationController.SkinnedBoneTransforms[this.mLeftArmIndex[k + 1]], out translation2);
				Vector3.Subtract(ref translation, ref translation2, out vector);
				vector.Normalize();
				Matrix.CreateWorld(ref zero, ref vector, ref up, out transform.Orientation);
				transform.Position = translation;
				this.mArmZone.Body.CollisionSkin.GetPrimitiveLocal(k).SetTransform(ref transform);
				this.mArmZone.Body.CollisionSkin.GetPrimitiveNewWorld(k).SetTransform(ref transform);
				this.mArmZone.Body.CollisionSkin.GetPrimitiveOldWorld(k).SetTransform(ref transform);
				translation = this.mRightArmBindPose[k].Translation;
				Vector3.Transform(ref translation, ref this.mAnimationController.SkinnedBoneTransforms[this.mRightArmIndex[k]], out translation);
				translation2 = this.mRightArmBindPose[k + 1].Translation;
				Vector3.Transform(ref translation2, ref this.mAnimationController.SkinnedBoneTransforms[this.mRightArmIndex[k + 1]], out translation2);
				Vector3.Subtract(ref translation, ref translation2, out vector);
				vector.Normalize();
				Matrix.CreateWorld(ref zero, ref vector, ref up, out transform.Orientation);
				transform.Position = translation;
				this.mArmZone.Body.CollisionSkin.GetPrimitiveLocal(2 + k).SetTransform(ref transform);
				this.mArmZone.Body.CollisionSkin.GetPrimitiveNewWorld(2 + k).SetTransform(ref transform);
				this.mArmZone.Body.CollisionSkin.GetPrimitiveOldWorld(2 + k).SetTransform(ref transform);
			}
			this.mArmZone.Body.CollisionSkin.UpdateWorldBoundingBox();
			this.mArmZone.Update(iDataChannel, iDeltaTime);
			Cthulhu.RenderData renderData = this.mRenderData[(int)iDataChannel];
			float num3 = this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].Magnitude;
			num3 *= 10f;
			num3 = Math.Min(num3, 1f);
			renderData.Colorize.X = Cthulhu.ColdColor.X;
			renderData.Colorize.Y = Cthulhu.ColdColor.Y;
			renderData.Colorize.Z = Cthulhu.ColdColor.Z;
			renderData.Colorize.W = num3;
			this.mHitFlashTimer = Math.Max(this.mHitFlashTimer - iDeltaTime * 5f, 0f);
			renderData.BoundingSphere.Center = this.mTransform.Translation;
			renderData.Damage = 1f - this.mHitPoints / this.mMaxHitPoints;
			renderData.Flash = this.mHitFlashTimer;
			this.ApplyColdEffect(iDeltaTime);
			this.ApplyWetSpecularEffect(renderData, iDeltaTime);
			Vector3 vector2 = new Vector3(0.75f, 0.8f, 1f);
			Vector3 diffuseColor = renderData.Material.DiffuseColor;
			Vector3.Multiply(ref renderData.Material.DiffuseColor, ref vector2, out renderData.Material.DiffuseColor);
			Array.Copy(this.mAnimationController.SkinnedBoneTransforms, renderData.Skeleton, renderData.Skeleton.Length);
			this.mPlayState.Scene.AddRenderableObject(iDataChannel, renderData);
			renderData.Material.DiffuseColor = diffuseColor;
			if ((this.mCurrentState == Cthulhu.States.Submerge || this.mCurrentState == Cthulhu.States.Death) && EffectManager.Instance.IsActive(ref this.mDeflectionEffect))
			{
				this.mDeflectionTimer = Math.Min(this.mDeflectionTimer, 0.2f);
			}
			if (this.mDeflectionTimer > 0f)
			{
				this.mDeflectionTimer -= iDeltaTime;
				if (!EffectManager.Instance.IsActive(ref this.mDeflectionEffect))
				{
					EffectManager.Instance.StartEffect(Cthulhu.DEFLECTION_EFFECT, ref this.mTransform, out this.mDeflectionEffect);
				}
				EffectManager.Instance.UpdateOrientation(ref this.mDeflectionEffect, ref this.mTransform);
				this.mDeflectionAura.Execute(this.mDamageZone, iDeltaTime, AuraTarget.Self, 0, 5f);
				return;
			}
			EffectManager.Instance.Stop(ref this.mDeflectionEffect);
		}

		// Token: 0x060007B8 RID: 1976 RVA: 0x00031CA8 File Offset: 0x0002FEA8
		internal void TurnOnIdleEffect()
		{
			Vector3 translation = this.mTransform.Translation;
			translation.Y = this.WaterYpos;
			Matrix matrix = Matrix.CreateTranslation(translation);
			EffectManager.Instance.StartEffect(this.mIdleEffect, ref matrix, out this.mIdleEffectRef);
		}

		// Token: 0x060007B9 RID: 1977 RVA: 0x00031CEE File Offset: 0x0002FEEE
		internal void KillIdleEffect()
		{
			if (EffectManager.Instance.IsActive(ref this.mIdleEffectRef))
			{
				EffectManager.Instance.Stop(ref this.mIdleEffectRef);
			}
		}

		// Token: 0x060007BA RID: 1978 RVA: 0x00031D14 File Offset: 0x0002FF14
		private void ApplyColdEffect(float iDeltaTime)
		{
			if (!base.HasStatus(StatusEffects.Cold))
			{
				if (EffectManager.Instance.IsActive(ref this.mCustomColdEffectRef[0]))
				{
					EffectManager.Instance.Stop(ref this.mCustomColdEffectRef[0]);
					EffectManager.Instance.Stop(ref this.mCustomColdEffectRef[1]);
					EffectManager.Instance.Stop(ref this.mCustomColdEffectRef[2]);
					EffectManager.Instance.Stop(ref this.mCustomColdEffectRef[3]);
					EffectManager.Instance.Stop(ref this.mCustomColdEffectRef[4]);
				}
				return;
			}
			if (!EffectManager.Instance.IsActive(ref this.mCustomColdEffectRef[0]))
			{
				EffectManager.Instance.StartEffect("cthulhu_cold".GetHashCodeCustom(), ref this.mTransform, out this.mCustomColdEffectRef[0]);
				EffectManager.Instance.StartEffect("cthulhu_cold".GetHashCodeCustom(), ref this.mHeadOrientation, out this.mCustomColdEffectRef[1]);
				EffectManager.Instance.StartEffect("cthulhu_cold".GetHashCodeCustom(), ref this.mMouthAttachOrientation, out this.mCustomColdEffectRef[2]);
				EffectManager.Instance.StartEffect("cthulhu_cold".GetHashCodeCustom(), ref this.mRightHandOrientation, out this.mCustomColdEffectRef[3]);
				EffectManager.Instance.StartEffect("cthulhu_cold".GetHashCodeCustom(), ref this.mLeftHandOrientation, out this.mCustomColdEffectRef[4]);
				return;
			}
			EffectManager.Instance.UpdateOrientation(ref this.mCustomColdEffectRef[0], ref this.mTransform);
			EffectManager.Instance.UpdateOrientation(ref this.mCustomColdEffectRef[1], ref this.mHeadOrientation);
			EffectManager.Instance.UpdateOrientation(ref this.mCustomColdEffectRef[2], ref this.mMouthAttachOrientation);
			EffectManager.Instance.UpdateOrientation(ref this.mCustomColdEffectRef[3], ref this.mRightHandOrientation);
			EffectManager.Instance.UpdateOrientation(ref this.mCustomColdEffectRef[4], ref this.mLeftHandOrientation);
		}

		// Token: 0x060007BB RID: 1979 RVA: 0x00031F24 File Offset: 0x00030124
		private void ApplyWetSpecularEffect(Cthulhu.RenderData iRenderData, float iDeltaTime)
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

		// Token: 0x060007BC RID: 1980 RVA: 0x00032070 File Offset: 0x00030270
		internal OtherworldlyBolt SpawnCultistMissile(ref Vector3 iPosition, float iSpeed)
		{
			OtherworldlyBolt otherworldlyBolt = this.mCultistMissileCache[0];
			this.mCultistMissileCache.RemoveAt(0);
			Vector3 forward = this.mTransform.Forward;
			otherworldlyBolt.Spawn(this.mPlayState, ref iPosition, ref forward, iSpeed);
			this.mCultistMissileCache.Add(otherworldlyBolt);
			return otherworldlyBolt;
		}

		// Token: 0x060007BD RID: 1981 RVA: 0x000320C0 File Offset: 0x000302C0
		internal void KillBolts()
		{
			for (int i = 0; i < this.mCultistMissileCache.Count; i++)
			{
				this.mCultistMissileCache[i].DestroyOnNetwork(false, false, null, true);
			}
		}

		// Token: 0x060007BE RID: 1982 RVA: 0x000320F8 File Offset: 0x000302F8
		private void CheckIfCharactersAreClose(float iDeltaTime)
		{
			this.mCheckEnemiesInRangeTimer -= iDeltaTime;
			if (this.mCheckEnemiesInRangeTimer < 0f)
			{
				EntityManager entityManager = this.mPlayState.EntityManager;
				Vector3 translation = this.mTransform.Translation;
				List<Entity> entities = entityManager.GetEntities(translation, 21f, true);
				bool flag = false;
				int num = 0;
				while (num < entities.Count && !flag)
				{
					Avatar avatar = entities[num] as Avatar;
					flag = (avatar != null && !avatar.IsEthereal && !avatar.IsInvisibile);
					num++;
				}
				entityManager.ReturnEntityList(entities);
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

		// Token: 0x060007BF RID: 1983 RVA: 0x000321BC File Offset: 0x000303BC
		private void CheckMistDesirability(float iDeltaTime)
		{
			this.mCheckMistDesirabilityTimer -= iDeltaTime;
			if (this.mCheckMistDesirabilityTimer <= 0f)
			{
				EntityManager entityManager = this.mPlayState.EntityManager;
				Vector3 translation = this.mMistSpawnTransform.Translation;
				List<Entity> entities = entityManager.GetEntities(translation, this.mMistCloud.Radius, true, true);
				float num = 2.5f;
				float num2 = 0.5f;
				float num3 = num2 / num;
				float num4 = 0f;
				for (int i = 0; i < entities.Count; i++)
				{
					Avatar avatar = entities[i] as Avatar;
					num4 += ((avatar != null && !avatar.IsInvisibile) ? num3 : 0f);
				}
				entityManager.ReturnEntityList(entities);
				this.mMistDesirability += num4;
				this.mMistDesirability -= (num3 - num4) / 2f;
				this.mMistDesirability = MathHelper.Clamp(this.mMistDesirability, 0f, 1f);
				if (this.mMistDesirability == 1f)
				{
					this.mActivateMist = true;
				}
				else if (this.mMistDesirability == 0f)
				{
					this.mActivateMist = false;
				}
				this.mCheckMistDesirabilityTimer += num2;
			}
		}

		// Token: 0x060007C0 RID: 1984 RVA: 0x000322F0 File Offset: 0x000304F0
		private unsafe void ChangeState(Cthulhu.States iNewState)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				this.mStates[(int)this.mCurrentState].OnExit(this);
				this.mPreviousState = this.mCurrentState;
				this.mCurrentState = iNewState;
				this.mStates[(int)this.mCurrentState].OnEnter(this);
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					Cthulhu.ChangeStateMessage changeStateMessage;
					changeStateMessage.NewState = iNewState;
					BossFight.Instance.SendMessage<Cthulhu.ChangeStateMessage>(this, 1, (void*)(&changeStateMessage), true);
				}
			}
		}

		// Token: 0x060007C1 RID: 1985 RVA: 0x00032368 File Offset: 0x00030568
		private unsafe void ChangeStage(Cthulhu.Stages iNewStage)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				this.mStages[(int)this.mCurrentStage].OnExit(this);
				this.mCurrentStage = iNewStage;
				this.mStages[(int)this.mCurrentStage].OnEnter(this);
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					Cthulhu.ChangeStageMessage changeStageMessage;
					changeStageMessage.NewStage = iNewStage;
					BossFight.Instance.SendMessage<Cthulhu.ChangeStageMessage>(this, 2, (void*)(&changeStageMessage), true);
				}
			}
		}

		// Token: 0x060007C2 RID: 1986 RVA: 0x000323D4 File Offset: 0x000305D4
		private int FindGoodSpot(float iRadius)
		{
			int result = -1;
			float num = float.MaxValue;
			for (int i = 0; i < this.mSpawnTransforms.Length; i++)
			{
				if (!this.IsSpawnPointOccupied((Cthulhu.BossSpawnPoints)i))
				{
					Matrix matrix = this.mSpawnTransforms[i];
					EntityManager entityManager = this.mPlayState.EntityManager;
					Vector3 translation = matrix.Translation;
					List<Entity> entities = entityManager.GetEntities(translation, iRadius, true);
					float num2 = 0f;
					bool flag = false;
					for (int j = 0; j < entities.Count; j++)
					{
						Avatar avatar = entities[j] as Avatar;
						if (avatar != null && !avatar.IsEthereal && !avatar.IsInvisibile)
						{
							flag = true;
							num2 += (avatar.Position - translation).LengthSquared();
						}
					}
					entityManager.ReturnEntityList(entities);
					if (flag && num2 < num)
					{
						result = i;
						num = num2;
					}
				}
			}
			return result;
		}

		// Token: 0x060007C3 RID: 1987 RVA: 0x000324C4 File Offset: 0x000306C4
		internal Matrix ChangeSpawnPoint(int iNewSpawnPoint, int iHandle)
		{
			this.LeaveSpawnTransform(iHandle);
			Cthulhu.BossSpawnPoints iSpawn = (Cthulhu.BossSpawnPoints)iNewSpawnPoint;
			Matrix spawnTransform = this.GetSpawnTransform(iSpawn);
			this.OccupySpawnTransform(iSpawn, iHandle);
			return spawnTransform;
		}

		// Token: 0x060007C4 RID: 1988 RVA: 0x000324EC File Offset: 0x000306EC
		private Matrix GetSpawnTransform(Cthulhu.BossSpawnPoints iSpawn)
		{
			return this.mSpawnTransforms[(int)iSpawn];
		}

		// Token: 0x060007C5 RID: 1989 RVA: 0x000324FF File Offset: 0x000306FF
		private void OccupySpawnTransform(Cthulhu.BossSpawnPoints iSpawn, int iHandle)
		{
			this.mOccupiedSpawnPoints[(int)iSpawn] = iHandle;
		}

		// Token: 0x060007C6 RID: 1990 RVA: 0x0003250C File Offset: 0x0003070C
		private void LeaveSpawnTransform(int iHandle)
		{
			for (int i = 0; i < this.mOccupiedSpawnPoints.Length; i++)
			{
				if (this.mOccupiedSpawnPoints[i] == iHandle)
				{
					this.mOccupiedSpawnPoints[i] = -1;
					return;
				}
			}
		}

		// Token: 0x060007C7 RID: 1991 RVA: 0x00032541 File Offset: 0x00030741
		internal void LeaveSpawnTransform(Tentacle iTentacle)
		{
			this.LeaveSpawnTransform((int)iTentacle.Handle);
		}

		// Token: 0x060007C8 RID: 1992 RVA: 0x0003254F File Offset: 0x0003074F
		private bool IsAtPoint(Cthulhu.BossSpawnPoints iPoint)
		{
			return this.mOccupiedSpawnPoints[(int)iPoint] == (int)this.mDamageZone.Handle;
		}

		// Token: 0x060007C9 RID: 1993 RVA: 0x00032566 File Offset: 0x00030766
		private bool IsSpawnPointOccupied(Cthulhu.BossSpawnPoints iSpawn)
		{
			return this.mOccupiedSpawnPoints[(int)iSpawn] != -1;
		}

		// Token: 0x060007CA RID: 1994 RVA: 0x00032576 File Offset: 0x00030776
		internal void SpawnNewTentacleAtGoodPoint()
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				this.SpawnTentacleAtGoodPoint(null);
			}
		}

		// Token: 0x060007CB RID: 1995 RVA: 0x00032590 File Offset: 0x00030790
		private int FindRandomSpot()
		{
			bool flag = false;
			int num = -1;
			int num2 = 0;
			int num3 = 200;
			while (!flag && num2 < num3)
			{
				num = Cthulhu.RANDOM.Next(this.mSpawnTransforms.Length);
				flag = !this.IsSpawnPointOccupied((Cthulhu.BossSpawnPoints)num);
				num2++;
			}
			return num;
		}

		// Token: 0x060007CC RID: 1996 RVA: 0x000325D8 File Offset: 0x000307D8
		internal unsafe bool SpawnTentacleAtGoodPoint(Tentacle iTentacle)
		{
			if (iTentacle == null)
			{
				if (this.mInactiveTentacles.Count <= 0)
				{
					return false;
				}
				iTentacle = this.mInactiveTentacles[0];
				this.AddTentacleToActiveList(0);
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					Cthulhu.TentacleSpawnMessage tentacleSpawnMessage = default(Cthulhu.TentacleSpawnMessage);
					tentacleSpawnMessage.TentacleIndex = (byte)iTentacle.ID;
					BossFight.Instance.SendMessage<Cthulhu.TentacleSpawnMessage>(this, 13, (void*)(&tentacleSpawnMessage), true);
				}
			}
			int num = this.FindGoodSpot(18f);
			if (num == -1)
			{
				int num2 = this.FindRandomSpot();
				if (num2 == -1)
				{
					return false;
				}
				num = num2;
			}
			Matrix iTransform = this.ChangeSpawnPoint(num, (int)iTentacle.Handle);
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				Cthulhu.SpawnPointMessage spawnPointMessage = default(Cthulhu.SpawnPointMessage);
				spawnPointMessage.Index = (sbyte)iTentacle.ID;
				spawnPointMessage.SpawnPoint = num;
				BossFight.Instance.SendMessage<Cthulhu.SpawnPointMessage>(this, 4, (void*)(&spawnPointMessage), true);
			}
			iTentacle.Start(iTransform);
			return true;
		}

		// Token: 0x060007CD RID: 1997 RVA: 0x000326B3 File Offset: 0x000308B3
		internal void ActivateMist()
		{
			if (!this.mMistCloud.Active)
			{
				this.mMistCloud.Initialize(this.mMistSpawnTransform.Translation);
				this.mPlayState.EntityManager.AddEntity(this.mMistCloud);
			}
		}

		// Token: 0x060007CE RID: 1998 RVA: 0x000326F0 File Offset: 0x000308F0
		protected bool GetRandomTarget(out Avatar oAvatar, ref Vector3 iSource, bool iIgnoreProtected)
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
							Avatar avatar = this.mPlayers[num3].Avatar;
							if (!iIgnoreProtected)
							{
								List<Shield> shields = this.mPlayState.EntityManager.Shields;
								bool flag2 = false;
								int num5 = 0;
								while (num5 < shields.Count && !flag2)
								{
									Vector3 position = avatar.Position;
									Vector3.Subtract(ref position, ref iSource, out position);
									Segment iSeg = new Segment(iSource, position);
									Shield shield = shields[num5];
									flag2 = shield.SegmentIntersect(out position, iSeg, 0f);
									num5++;
								}
								if (flag2)
								{
									continue;
								}
							}
							oAvatar = avatar;
							flag = true;
						}
					}
				}
			}
			return flag;
		}

		// Token: 0x1700019A RID: 410
		// (get) Token: 0x060007CF RID: 1999 RVA: 0x0003283C File Offset: 0x00030A3C
		internal float TimeBetweenAttacks
		{
			get
			{
				return this.mTimeBetweenActions;
			}
		}

		// Token: 0x1700019B RID: 411
		// (get) Token: 0x060007D0 RID: 2000 RVA: 0x00032844 File Offset: 0x00030A44
		internal float StageSpeedModifier
		{
			get
			{
				return this.mStageSpeedModifier;
			}
		}

		// Token: 0x1700019C RID: 412
		// (get) Token: 0x060007D1 RID: 2001 RVA: 0x0003284C File Offset: 0x00030A4C
		internal float StateSpeedModifier
		{
			get
			{
				return this.mStateSpeedModifier;
			}
		}

		// Token: 0x060007D2 RID: 2002 RVA: 0x00032854 File Offset: 0x00030A54
		private void StartClip(Cthulhu.Animations anim, bool loop)
		{
			this.mAnimationController.StartClip(this.mAnimations[(int)anim], loop);
		}

		// Token: 0x060007D3 RID: 2003 RVA: 0x0003286A File Offset: 0x00030A6A
		private void StopClip()
		{
			this.mAnimationController.Stop();
		}

		// Token: 0x060007D4 RID: 2004 RVA: 0x00032877 File Offset: 0x00030A77
		private void CrossFade(Cthulhu.Animations anim, float time, bool loop)
		{
			this.mAnimationController.CrossFade(this.mAnimations[(int)anim], time, loop);
		}

		// Token: 0x060007D5 RID: 2005 RVA: 0x00032890 File Offset: 0x00030A90
		public DamageResult Damage(int iPartIndex, Damage iDamage, Entity iAttacker, ref Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			DamageResult result = DamageResult.None;
			if (iPartIndex != 0)
			{
				if (iPartIndex == 5)
				{
					result = this.mMistCloud.Damage(iDamage, iAttacker, ref iAttackPosition);
				}
			}
			else
			{
				result = this.Damage(iDamage, iAttacker, iAttackPosition, iFeatures);
			}
			return result;
		}

		// Token: 0x060007D6 RID: 2006 RVA: 0x000328D0 File Offset: 0x00030AD0
		public void Damage(int iPartIndex, float iDamage, Elements iElement)
		{
			if (iPartIndex == 0)
			{
				base.Damage(iDamage, iElement);
				return;
			}
			if (iPartIndex != 5)
			{
				return;
			}
			this.mMistCloud.Damage(iDamage, iElement);
		}

		// Token: 0x060007D7 RID: 2007 RVA: 0x000328FE File Offset: 0x00030AFE
		public void SetSlow(int iIndex)
		{
		}

		// Token: 0x060007D8 RID: 2008 RVA: 0x00032900 File Offset: 0x00030B00
		public void DeInitialize()
		{
		}

		// Token: 0x060007D9 RID: 2009 RVA: 0x00032902 File Offset: 0x00030B02
		public bool AddImpulseVelocity(Vector3 iDirection, float iElevation, float iMassPower, float iDistance)
		{
			return false;
		}

		// Token: 0x1700019D RID: 413
		// (get) Token: 0x060007DA RID: 2010 RVA: 0x00032905 File Offset: 0x00030B05
		public override bool Dead
		{
			get
			{
				return this.mDead;
			}
		}

		// Token: 0x1700019E RID: 414
		// (get) Token: 0x060007DB RID: 2011 RVA: 0x0003290D File Offset: 0x00030B0D
		public float MaxHitPoints
		{
			get
			{
				return this.mMaxHitPoints;
			}
		}

		// Token: 0x1700019F RID: 415
		// (get) Token: 0x060007DC RID: 2012 RVA: 0x00032915 File Offset: 0x00030B15
		public float HitPoints
		{
			get
			{
				return this.mHitPoints;
			}
		}

		// Token: 0x060007DD RID: 2013 RVA: 0x0003291D File Offset: 0x00030B1D
		public bool HasStatus(int iIndex, StatusEffects iStatus)
		{
			return base.HasStatus(iStatus);
		}

		// Token: 0x060007DE RID: 2014 RVA: 0x00032926 File Offset: 0x00030B26
		public float StatusMagnitude(int iIndex, StatusEffects iStatus)
		{
			return base.StatusMagnitude(iStatus);
		}

		// Token: 0x060007DF RID: 2015 RVA: 0x00032930 File Offset: 0x00030B30
		public void ScriptMessage(BossMessages iMessage)
		{
			if (iMessage != BossMessages.StartFight)
			{
				return;
			}
			this.mOkToFight = true;
		}

		// Token: 0x060007E0 RID: 2016 RVA: 0x0003294C File Offset: 0x00030B4C
		private unsafe void NetworkUpdate()
		{
			NetworkServer networkServer = NetworkManager.Instance.Interface as NetworkServer;
			Cthulhu.UpdateMessage updateMessage = default(Cthulhu.UpdateMessage);
			updateMessage.Animation = 0;
			while ((int)updateMessage.Animation < this.mAnimations.Length && this.mAnimationController.AnimationClip != this.mAnimations[(int)updateMessage.Animation])
			{
				updateMessage.Animation += 1;
			}
			updateMessage.Hitpoints = this.mHitPoints;
			for (int i = 0; i < networkServer.Connections; i++)
			{
				float num = networkServer.GetLatency(i) * 0.5f;
				Cthulhu.UpdateMessage updateMessage2 = updateMessage;
				updateMessage2.AnimationTime = new HalfSingle(this.mAnimationController.Time + num);
				BossFight.Instance.SendMessage<Cthulhu.UpdateMessage>(this, 0, (void*)(&updateMessage2), false, i);
			}
		}

		// Token: 0x060007E1 RID: 2017 RVA: 0x00032A10 File Offset: 0x00030C10
		private void AddTentacleToActiveList(int iIndex)
		{
			Tentacle tentacle = this.mInactiveTentacles[iIndex];
			this.mInactiveTentacles.RemoveAt(iIndex);
			this.mActiveTentacles.Add(tentacle);
			if (!this.mPlayState.EntityManager.Entities.Contains(tentacle))
			{
				tentacle.Initialize();
				this.mPlayState.EntityManager.AddEntity(tentacle);
			}
		}

		// Token: 0x060007E2 RID: 2018 RVA: 0x00032A74 File Offset: 0x00030C74
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
				Cthulhu.UpdateMessage updateMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&updateMessage));
				if (this.mAnimationController.AnimationClip != this.mAnimations[(int)updateMessage.Animation])
				{
					this.mAnimationController.StartClip(this.mAnimations[(int)updateMessage.Animation], false);
				}
				this.mAnimationController.Time = updateMessage.AnimationTime.ToSingle();
				this.mHitPoints = updateMessage.Hitpoints;
				return;
			}
			case 1:
			{
				Cthulhu.ChangeStateMessage changeStateMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&changeStateMessage));
				if (changeStateMessage.NewState != Cthulhu.States.NR_OF_STATES)
				{
					this.mStates[(int)this.mCurrentState].OnExit(this);
					this.mPreviousState = this.mCurrentState;
					this.mCurrentState = changeStateMessage.NewState;
					this.mStates[(int)this.mCurrentState].OnEnter(this);
					return;
				}
				break;
			}
			case 2:
			{
				Cthulhu.ChangeStageMessage changeStageMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&changeStageMessage));
				if (changeStageMessage.NewStage != Cthulhu.Stages.NR_OF_STAGES)
				{
					this.mStages[(int)this.mCurrentStage].OnExit(this);
					this.mCurrentStage = changeStageMessage.NewStage;
					this.mStages[(int)this.mCurrentStage].OnEnter(this);
					return;
				}
				break;
			}
			case 3:
			{
				Cthulhu.ChangeTargetMessage changeTargetMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&changeTargetMessage));
				Magicka.GameLogic.Entities.Entity.GetFromHandle(changeTargetMessage.Handle);
				break;
			}
			case 4:
			{
				Cthulhu.SpawnPointMessage spawnPointMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&spawnPointMessage));
				if (spawnPointMessage.Index == -1)
				{
					if (spawnPointMessage.SpawnPoint != -1)
					{
						this.mDesiredSpawnPoint = spawnPointMessage.SpawnPoint;
						Matrix matrix = this.ChangeSpawnPoint(this.mDesiredSpawnPoint, (int)this.mDamageZone.Handle);
						this.mTransform = matrix;
						return;
					}
				}
				else if (spawnPointMessage.SpawnPoint != -1)
				{
					this.mTentacles[(int)spawnPointMessage.Index].NetworkSpawnPoint(ref spawnPointMessage);
					return;
				}
				break;
			}
			case 5:
			{
				Cthulhu.HypnotizeMessage hypnotizeMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&hypnotizeMessage));
				Entity fromHandle = Magicka.GameLogic.Entities.Entity.GetFromHandle((int)hypnotizeMessage.Handle);
				Vector3 vector = hypnotizeMessage.Direction.ToVector3();
				Avatar avatar = fromHandle as Avatar;
				if (avatar == null)
				{
					return;
				}
				avatar.Hypnotize(ref vector, this.mPlayerHypnotizeEffect);
				return;
			}
			case 6:
				TimeWarp.Instance.Execute(this.mDamageZone, this.mPlayState);
				return;
			case 7:
				this.ActivateMist();
				return;
			case 8:
			{
				Cthulhu.SetCharacterToEatMessage setCharacterToEatMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&setCharacterToEatMessage));
				Entity fromHandle2 = Magicka.GameLogic.Entities.Entity.GetFromHandle((int)setCharacterToEatMessage.Handle);
				Character character = fromHandle2 as Character;
				if (character == null)
				{
					return;
				}
				this.mCharacterToEat = character;
				return;
			}
			case 9:
			{
				Cthulhu.CharmAndConfuseMessage charmAndConfuseMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&charmAndConfuseMessage));
				Entity fromHandle3 = Magicka.GameLogic.Entities.Entity.GetFromHandle((int)charmAndConfuseMessage.Handle);
				Avatar avatar2 = fromHandle3 as Avatar;
				if (avatar2 == null)
				{
					return;
				}
				this.mMistCloud.CharmAndConfuse(avatar2);
				return;
			}
			case 10:
				this.mDeflectionTimer = 8f;
				return;
			case 11:
			{
				Cthulhu.TentacleUpdateMessage tentacleUpdateMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&tentacleUpdateMessage));
				this.mTentacles[(int)tentacleUpdateMessage.TentacleIndex].NetworkUpdate(ref tentacleUpdateMessage, (float)iMsg.TimeStamp);
				return;
			}
			case 12:
			{
				Cthulhu.TentacleChangeStateMessage tentacleChangeStateMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&tentacleChangeStateMessage));
				this.mTentacles[(int)tentacleChangeStateMessage.TentacleIndex].NetworkChangeState(ref tentacleChangeStateMessage);
				return;
			}
			case 13:
			{
				Cthulhu.TentacleSpawnMessage tentacleSpawnMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&tentacleSpawnMessage));
				for (int i = 0; i < this.mInactiveTentacles.Count; i++)
				{
					if (this.mInactiveTentacles[i].ID == (int)tentacleSpawnMessage.TentacleIndex)
					{
						this.AddTentacleToActiveList(i);
						return;
					}
				}
				return;
			}
			case 14:
			{
				Cthulhu.TentacleGrabMessage tentacleGrabMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&tentacleGrabMessage));
				Entity fromHandle4 = Magicka.GameLogic.Entities.Entity.GetFromHandle((int)tentacleGrabMessage.Handle);
				IDamageable damageable = fromHandle4 as IDamageable;
				if (damageable == null)
				{
					return;
				}
				this.mTentacles[(int)tentacleGrabMessage.TentacleIndex].GrabDamageable(damageable);
				return;
			}
			case 15:
			{
				Cthulhu.TentacleAimTargetMessage tentacleAimTargetMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&tentacleAimTargetMessage));
				Entity fromHandle5 = Magicka.GameLogic.Entities.Entity.GetFromHandle((int)tentacleAimTargetMessage.Handle);
				Avatar avatar3 = fromHandle5 as Avatar;
				if (avatar3 == null)
				{
					return;
				}
				this.mTentacles[(int)tentacleAimTargetMessage.TentacleIndex].SetAimTarget(avatar3);
				return;
			}
			case 16:
			{
				Cthulhu.TentacleReleaseAimTargetMessage tentacleReleaseAimTargetMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&tentacleReleaseAimTargetMessage));
				this.mTentacles[(int)tentacleReleaseAimTargetMessage.TentacleIndex].SetAimTarget(null);
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x060007E3 RID: 2019 RVA: 0x00032E65 File Offset: 0x00031065
		public void NetworkInitialize(ref BossInitializeMessage iMsg)
		{
		}

		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x060007E4 RID: 2020 RVA: 0x00032E67 File Offset: 0x00031067
		public bool NetworkInitialized
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060007E5 RID: 2021 RVA: 0x00032E6A File Offset: 0x0003106A
		public BossEnum GetBossType()
		{
			return BossEnum.Cthulhu;
		}

		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x060007E6 RID: 2022 RVA: 0x00032E6E File Offset: 0x0003106E
		protected override BossDamageZone Entity
		{
			get
			{
				return this.mDamageZone;
			}
		}

		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x060007E7 RID: 2023 RVA: 0x00032E76 File Offset: 0x00031076
		protected override float Radius
		{
			get
			{
				return 1.2f;
			}
		}

		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x060007E8 RID: 2024 RVA: 0x00032E7D File Offset: 0x0003107D
		protected override float Length
		{
			get
			{
				return 4f;
			}
		}

		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x060007E9 RID: 2025 RVA: 0x00032E84 File Offset: 0x00031084
		protected override int BloodEffect
		{
			get
			{
				return Cthulhu.BLOOD_BLACK_EFFECT;
			}
		}

		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x060007EA RID: 2026 RVA: 0x00032E8C File Offset: 0x0003108C
		protected override Vector3 NotifierTextPostion
		{
			get
			{
				Vector3 translation = this.mHeadOrientation.Translation;
				translation.Y += 3f;
				return translation;
			}
		}

		// Token: 0x060007EB RID: 2027 RVA: 0x00032EBC File Offset: 0x000310BC
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

		// Token: 0x060007EC RID: 2028 RVA: 0x00032F77 File Offset: 0x00031177
		public void AddSelfShield(int iIndex, Spell iSpell)
		{
		}

		// Token: 0x060007ED RID: 2029 RVA: 0x00032F79 File Offset: 0x00031179
		public void RemoveSelfShield(int iIndex, Character.SelfShieldType iType)
		{
		}

		// Token: 0x060007EE RID: 2030 RVA: 0x00032F7B File Offset: 0x0003117B
		public CastType CastType(int iIndex)
		{
			return Magicka.GameLogic.Spells.CastType.None;
		}

		// Token: 0x060007EF RID: 2031 RVA: 0x00032F7E File Offset: 0x0003117E
		public float SpellPower(int iIndex)
		{
			return 1f;
		}

		// Token: 0x060007F0 RID: 2032 RVA: 0x00032F85 File Offset: 0x00031185
		public void SpellPower(int iIndex, float iSpellPower)
		{
		}

		// Token: 0x060007F1 RID: 2033 RVA: 0x00032F87 File Offset: 0x00031187
		public SpellEffect CurrentSpell(int iIndex)
		{
			return this.mSpellEffect;
		}

		// Token: 0x060007F2 RID: 2034 RVA: 0x00032F8F File Offset: 0x0003118F
		public void CurrentSpell(int iIndex, SpellEffect iEffect)
		{
			this.mSpellEffect = iEffect;
		}

		// Token: 0x060007F3 RID: 2035 RVA: 0x00032F98 File Offset: 0x00031198
		internal void ClearAllStatusEffects()
		{
			for (int i = 0; i < this.mStatusEffects.Length; i++)
			{
				this.mStatusEffects[i].Stop();
				this.mStatusEffects[i] = default(StatusEffect);
			}
		}

		// Token: 0x060007F4 RID: 2036 RVA: 0x00032FDC File Offset: 0x000311DC
		protected override DamageResult Damage(Damage iDamage, Entity iAttacker, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			if (this.Dead)
			{
				return DamageResult.Deflected;
			}
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
				if ((damage.Element & Elements.Fire) == Elements.Fire && this.mResistances[Spell.ElementIndex(Elements.Fire)].Multiplier > 1E-45f && (base.HasStatus(StatusEffects.Wet) || base.HasStatus(StatusEffects.Cold)))
				{
					damageResult |= base.AddStatusEffect(new StatusEffect(StatusEffects.Burning, damage.Amount, damage.Magnitude, this.Length, this.Radius));
				}
				if ((damage.Element & Elements.Cold) == Elements.Cold && this.mResistances[Spell.ElementIndex(Elements.Cold)].Multiplier > 1E-45f)
				{
					damageResult |= base.AddStatusEffect(new StatusEffect(StatusEffects.Cold, damage.Amount, damage.Magnitude, this.Length, this.Radius * 4f));
				}
				if ((damage.Element & Elements.Water) == Elements.Water && this.mResistances[Spell.ElementIndex(Elements.Water)].Multiplier > 1E-45f)
				{
					damageResult |= base.AddStatusEffect(new StatusEffect(StatusEffects.Wet, damage.Amount, damage.Magnitude, this.Length, this.Radius));
				}
				if ((damage.Element & Elements.Steam) == Elements.Steam && this.mResistances[Spell.ElementIndex(Elements.Steam)].Multiplier > 1E-45f)
				{
					damageResult |= base.AddStatusEffect(new StatusEffect(StatusEffects.Wet, damage.Amount, damage.Magnitude, this.Length, this.Radius));
				}
			}
			if ((short)(damage.AttackProperty & AttackProperties.Damage) == 1)
			{
				if ((damage.Element & Elements.Lightning) == Elements.Lightning && base.HasStatus(StatusEffects.Wet))
				{
					damage.Amount *= 2f;
				}
				if ((damage.Element & Elements.PhysicalElements) != Elements.None)
				{
					if (base.HasStatus(StatusEffects.Frozen))
					{
						damage.Amount = Math.Max(damage.Amount - 200f, 0f);
						damage.Magnitude = Math.Max(1f, damage.Magnitude);
						damage.Amount *= 3f;
					}
					else if (GlobalSettings.Instance.BloodAndGore == SettingOptions.On)
					{
						Vector3 vector = iAttackPosition;
						Vector3 right = Vector3.Right;
						VisualEffectReference visualEffectReference;
						EffectManager.Instance.StartEffect(this.BloodEffect, ref vector, ref right, out visualEffectReference);
					}
				}
				damage.Amount *= damage.Magnitude;
				this.mHitPoints -= damage.Amount;
				if (damage.Amount > 0f)
				{
					this.mHitFlashTimer = 0.5f;
					this.mTimeSinceLastDamageTimewarp = 0f;
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
				if (Defines.FeatureNotify(iFeatures))
				{
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

		// Token: 0x060007F5 RID: 2037 RVA: 0x00033518 File Offset: 0x00031718
		protected override void UpdateStatusEffects(float iDeltaTime)
		{
			this.mDryTimer -= iDeltaTime;
			StatusEffects statusEffects = StatusEffects.None;
			if (this.Dead)
			{
				for (int i = 0; i < this.mStatusEffects.Length; i++)
				{
					this.mStatusEffects[i].Stop();
					this.mStatusEffects[i] = default(StatusEffect);
				}
			}
			else
			{
				for (int j = 0; j < this.mStatusEffects.Length; j++)
				{
					if (this.mStatusEffects[j].DamageType == StatusEffects.Wet)
					{
						this.mStatusEffects[j].StopEffect();
					}
					else
					{
						this.mStatusEffects[j].Update(iDeltaTime, this.Entity);
					}
					if (this.mStatusEffects[j].Dead)
					{
						this.mStatusEffects[j].Stop();
						this.mStatusEffects[j] = default(StatusEffect);
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

		// Token: 0x040006A4 RID: 1700
		private const float NETWORK_UPDATE_PERIOD = 0.033333335f;

		// Token: 0x040006A5 RID: 1701
		private const int NR_OF_ARM_JOINTS = 3;

		// Token: 0x040006A6 RID: 1702
		private const int NR_OF_FINGERS = 6;

		// Token: 0x040006A7 RID: 1703
		private const int NR_OF_SPINE_PARTS = 5;

		// Token: 0x040006A8 RID: 1704
		private const int DEFLECTION_TIME = 8;

		// Token: 0x040006A9 RID: 1705
		private const int MAX_LIGHTING_RANGE_REL_BODY_CHECK = 19;

		// Token: 0x040006AA RID: 1706
		private const int MIN_LIGHTING_RANGE_REL_BODY_CHECK = 5;

		// Token: 0x040006AB RID: 1707
		private const int MAX_VORTEX_RANGE_REL_BODY_CHECK = 23;

		// Token: 0x040006AC RID: 1708
		private const int MIN_VORTEX_RANGE_REL_BODY_CHECK = 2;

		// Token: 0x040006AD RID: 1709
		private const int SUBMERGE_PUSH_RANGE = 6;

		// Token: 0x040006AE RID: 1710
		private const int SUBMERGE_SOAK_RANGE = 6;

		// Token: 0x040006AF RID: 1711
		private const int EMERGE_KNOCKBACK_RANGE = 6;

		// Token: 0x040006B0 RID: 1712
		private const int EMERGE_SOAK_RANGE = 10;

		// Token: 0x040006B1 RID: 1713
		private const int MAX_DIST_TO_AVATARS = 21;

		// Token: 0x040006B2 RID: 1714
		private const int MAX_RANGE_FOR_GOOD_SPOT = 18;

		// Token: 0x040006B3 RID: 1715
		private const int DEVOUR_SUCK_RADIUS = 30;

		// Token: 0x040006B4 RID: 1716
		private IBossState<Cthulhu>[] mStages = new IBossState<Cthulhu>[]
		{
			new Cthulhu.IntroStage(),
			new Cthulhu.BattleStage(),
			new Cthulhu.LateBattleStage(),
			new Cthulhu.CriticalStage(),
			new Cthulhu.FinalStage()
		};

		// Token: 0x040006B5 RID: 1717
		private Cthulhu.CthulhuState[] mStates = new Cthulhu.CthulhuState[]
		{
			new Cthulhu.IdleState(),
			new Cthulhu.EmergeState(),
			new Cthulhu.SubmergeState(),
			new Cthulhu.DevourState(),
			new Cthulhu.DevourHitState(),
			new Cthulhu.LightningState(),
			new Cthulhu.MistState(),
			new Cthulhu.CallState(),
			new Cthulhu.LesserCallState(),
			new Cthulhu.TimewarpState(),
			new Cthulhu.HypnotizeState(),
			new Cthulhu.RageState(),
			new Cthulhu.OtherworldlyBoltState(),
			new Cthulhu.DeathState()
		};

		// Token: 0x040006B6 RID: 1718
		private static readonly int SOUND_CALL_OF_CTHULHU = "cthulhu_call_of_cthulhu".GetHashCodeCustom();

		// Token: 0x040006B7 RID: 1719
		private static readonly int SOUND_DEATH = "cthulhu_death".GetHashCodeCustom();

		// Token: 0x040006B8 RID: 1720
		private static readonly int SOUND_DEVOUR = "cthulhu_devour".GetHashCodeCustom();

		// Token: 0x040006B9 RID: 1721
		private static readonly int SOUND_DEVOUR_SUCK = "cthulhu_devour_suck".GetHashCodeCustom();

		// Token: 0x040006BA RID: 1722
		private static readonly int SOUND_DEVOUR_HIT = "cthulhu_devour_hit".GetHashCodeCustom();

		// Token: 0x040006BB RID: 1723
		private static readonly int SOUND_OTHERWORLDLY_BOLT = "cthulhu_howl".GetHashCodeCustom();

		// Token: 0x040006BC RID: 1724
		private static readonly int SOUND_HOWL = "cthulhu_howl".GetHashCodeCustom();

		// Token: 0x040006BD RID: 1725
		private static readonly int SOUND_EMERGE = "cthulhu_emerge".GetHashCodeCustom();

		// Token: 0x040006BE RID: 1726
		private static readonly int SOUND_HYPNOTIZE = "cthulhu_hypnotize".GetHashCodeCustom();

		// Token: 0x040006BF RID: 1727
		private static readonly int SOUND_LESSER_CALL_OF_CTHULHU = "cthulhu_lesser_call_of_cthulhu".GetHashCodeCustom();

		// Token: 0x040006C0 RID: 1728
		private static readonly int SOUND_LIGHTNING = "spell_lightning_spray".GetHashCodeCustom();

		// Token: 0x040006C1 RID: 1729
		private static readonly int SOUND_MIST = "cthulhu_mist".GetHashCodeCustom();

		// Token: 0x040006C2 RID: 1730
		private static readonly int SOUND_SUBMERGE = "cthulhu_submerge".GetHashCodeCustom();

		// Token: 0x040006C3 RID: 1731
		private static readonly int SOUND_TIMEWARP = "magick_timewarp".GetHashCodeCustom();

		// Token: 0x040006C4 RID: 1732
		private static readonly int SOUND_RAGE = "cthulhu_rage".GetHashCodeCustom();

		// Token: 0x040006C5 RID: 1733
		private float mLastNetworkUpdate;

		// Token: 0x040006C6 RID: 1734
		protected float mNetworkUpdateTimer;

		// Token: 0x040006C7 RID: 1735
		private static readonly int[] SPAWN_LOCATORS = new int[]
		{
			"boss_spawn0".GetHashCodeCustom(),
			"boss_spawn6".GetHashCodeCustom(),
			"boss_spawn4".GetHashCodeCustom(),
			"boss_spawn5".GetHashCodeCustom(),
			"boss_spawn2".GetHashCodeCustom(),
			"boss_spawn1".GetHashCodeCustom(),
			"boss_spawn3".GetHashCodeCustom(),
			"boss_spawn7".GetHashCodeCustom()
		};

		// Token: 0x040006C8 RID: 1736
		private static readonly int[] DEEP_ONES_SPAWN_LOCATORS = new int[]
		{
			"spawn_deepone0".GetHashCodeCustom(),
			"spawn_deepone1".GetHashCodeCustom(),
			"spawn_deepone2".GetHashCodeCustom(),
			"spawn_deepone3".GetHashCodeCustom(),
			"spawn_deepone4".GetHashCodeCustom(),
			"spawn_deepone5".GetHashCodeCustom(),
			"spawn_deepone6".GetHashCodeCustom(),
			"spawn_deepone7".GetHashCodeCustom()
		};

		// Token: 0x040006C9 RID: 1737
		private static readonly int MIST_LOCATOR = "boss_spawn_middle".GetHashCodeCustom();

		// Token: 0x040006CA RID: 1738
		internal static readonly int BLOOD_BLACK_EFFECT = "gore_splash_black".GetHashCodeCustom();

		// Token: 0x040006CB RID: 1739
		private static readonly int DEFLECTION_EFFECT = "cthulhu_deflect".GetHashCodeCustom();

		// Token: 0x040006CC RID: 1740
		private static readonly Vector3 ColdColor = new Vector3(1f, 1.6f, 2f);

		// Token: 0x040006CD RID: 1741
		private Cthulhu.RenderData[] mRenderData;

		// Token: 0x040006CE RID: 1742
		private Matrix mTransform;

		// Token: 0x040006CF RID: 1743
		private PlayState mPlayState;

		// Token: 0x040006D0 RID: 1744
		private AnimationClip[] mAnimations;

		// Token: 0x040006D1 RID: 1745
		private AnimationController mAnimationController;

		// Token: 0x040006D2 RID: 1746
		internal static Random RANDOM = new Random();

		// Token: 0x040006D3 RID: 1747
		private Cthulhu.States mCurrentState;

		// Token: 0x040006D4 RID: 1748
		private Cthulhu.States mPreviousState;

		// Token: 0x040006D5 RID: 1749
		private Cthulhu.Stages mCurrentStage;

		// Token: 0x040006D6 RID: 1750
		private float mStageSpeedModifier;

		// Token: 0x040006D7 RID: 1751
		private float mStateSpeedModifier;

		// Token: 0x040006D8 RID: 1752
		private float mTimeBetweenActions;

		// Token: 0x040006D9 RID: 1753
		private int mNumberOfTentacles;

		// Token: 0x040006DA RID: 1754
		private float mRadius;

		// Token: 0x040006DB RID: 1755
		private bool mDead;

		// Token: 0x040006DC RID: 1756
		private BossSpellCasterZone mDamageZone;

		// Token: 0x040006DD RID: 1757
		private BossCollisionZone mArmZone;

		// Token: 0x040006DE RID: 1758
		private SpellEffect mSpellEffect;

		// Token: 0x040006DF RID: 1759
		private int[] mRightArmIndex = new int[3];

		// Token: 0x040006E0 RID: 1760
		private Matrix[] mRightArmBindPose = new Matrix[3];

		// Token: 0x040006E1 RID: 1761
		private int[] mLeftArmIndex = new int[3];

		// Token: 0x040006E2 RID: 1762
		private Matrix[] mLeftArmBindPose = new Matrix[3];

		// Token: 0x040006E3 RID: 1763
		private int mRightHandIndex;

		// Token: 0x040006E4 RID: 1764
		private Matrix mRightHandBindPose;

		// Token: 0x040006E5 RID: 1765
		private int mLeftHandIndex;

		// Token: 0x040006E6 RID: 1766
		private Matrix mLeftHandBindPose;

		// Token: 0x040006E7 RID: 1767
		private int[] mRightFingerIndex = new int[6];

		// Token: 0x040006E8 RID: 1768
		private Matrix[] mRightFingerBindPose = new Matrix[6];

		// Token: 0x040006E9 RID: 1769
		private int[] mLeftFingerIndex = new int[6];

		// Token: 0x040006EA RID: 1770
		private Matrix[] mLeftFingerBindPose = new Matrix[6];

		// Token: 0x040006EB RID: 1771
		private int mHeadIndex;

		// Token: 0x040006EC RID: 1772
		private Matrix mHeadBindPose;

		// Token: 0x040006ED RID: 1773
		private int mMouthAttachIndex;

		// Token: 0x040006EE RID: 1774
		private Matrix mMouthAttachBindPose;

		// Token: 0x040006EF RID: 1775
		private int[] mSpineIndex = new int[5];

		// Token: 0x040006F0 RID: 1776
		private Matrix[] mSpineBindPose = new Matrix[5];

		// Token: 0x040006F1 RID: 1777
		private Matrix mMouthAttachOrientation;

		// Token: 0x040006F2 RID: 1778
		private Matrix mHeadOrientation;

		// Token: 0x040006F3 RID: 1779
		private Matrix mLeftHandOrientation;

		// Token: 0x040006F4 RID: 1780
		private Matrix mRightHandOrientation;

		// Token: 0x040006F5 RID: 1781
		private Matrix[] mRightFingerOrientation = new Matrix[6];

		// Token: 0x040006F6 RID: 1782
		private Matrix[] mLeftFingerOrientation = new Matrix[6];

		// Token: 0x040006F7 RID: 1783
		private CthulhuMist mMistCloud;

		// Token: 0x040006F8 RID: 1784
		private Tentacle[] mTentacles;

		// Token: 0x040006F9 RID: 1785
		private List<Tentacle> mActiveTentacles;

		// Token: 0x040006FA RID: 1786
		private List<Tentacle> mInactiveTentacles;

		// Token: 0x040006FB RID: 1787
		private Matrix mMistSpawnTransform;

		// Token: 0x040006FC RID: 1788
		private Matrix[] mDeepOnesSpawnTransforms = new Matrix[8];

		// Token: 0x040006FD RID: 1789
		private Matrix[] mSpawnTransforms = new Matrix[8];

		// Token: 0x040006FE RID: 1790
		private int[] mOccupiedSpawnPoints;

		// Token: 0x040006FF RID: 1791
		private float mDeflectionTimer;

		// Token: 0x04000700 RID: 1792
		private AuraDeflect mDeflectionAura = new AuraDeflect(5f);

		// Token: 0x04000701 RID: 1793
		private VisualEffectReference mDeflectionEffect;

		// Token: 0x04000702 RID: 1794
		private VisualEffectReference[] mCustomColdEffectRef;

		// Token: 0x04000703 RID: 1795
		private Player[] mPlayers = Game.Instance.Players;

		// Token: 0x04000704 RID: 1796
		private Character mCharacterToEat;

		// Token: 0x04000705 RID: 1797
		private float mHPOnLastEmerge;

		// Token: 0x04000706 RID: 1798
		private float mTimeUntilSubmerge;

		// Token: 0x04000707 RID: 1799
		private float mTimeSinceLastEmerge;

		// Token: 0x04000708 RID: 1800
		private float mTimeSinceLastDamageTimewarp;

		// Token: 0x04000709 RID: 1801
		private float mHitFlashTimer;

		// Token: 0x0400070A RID: 1802
		private int mSecondsWhileOutOfRange;

		// Token: 0x0400070B RID: 1803
		private float mCheckEnemiesInRangeTimer;

		// Token: 0x0400070C RID: 1804
		private float mWetnessCounter;

		// Token: 0x0400070D RID: 1805
		private int mDesiredSpawnPoint;

		// Token: 0x0400070E RID: 1806
		private bool mActivateMist;

		// Token: 0x0400070F RID: 1807
		private float mMistDesirability;

		// Token: 0x04000710 RID: 1808
		private float mCheckMistDesirabilityTimer;

		// Token: 0x04000711 RID: 1809
		private float mWaterYpos;

		// Token: 0x04000712 RID: 1810
		private VisualEffectReference mIdleEffectRef;

		// Token: 0x04000713 RID: 1811
		private int mIdleEffect = "cthulhu_water_surface".GetHashCodeCustom();

		// Token: 0x04000714 RID: 1812
		private bool mInitialEmerge;

		// Token: 0x04000715 RID: 1813
		private bool mOkToFight;

		// Token: 0x04000716 RID: 1814
		private bool mTheKingHasFallen;

		// Token: 0x04000717 RID: 1815
		private List<OtherworldlyBolt> mCultistMissileCache;

		// Token: 0x04000718 RID: 1816
		private bool mStarSpawnSpawned;

		// Token: 0x04000719 RID: 1817
		private int mPlayerHypnotizeEffect = "cthulhu_player_hypnotize".GetHashCodeCustom();

		// Token: 0x02000102 RID: 258
		public enum Stages
		{
			// Token: 0x0400071B RID: 1819
			Intro,
			// Token: 0x0400071C RID: 1820
			Battle,
			// Token: 0x0400071D RID: 1821
			LateBattle,
			// Token: 0x0400071E RID: 1822
			Critical,
			// Token: 0x0400071F RID: 1823
			Final,
			// Token: 0x04000720 RID: 1824
			NR_OF_STAGES
		}

		// Token: 0x02000103 RID: 259
		private class IntroStage : IBossState<Cthulhu>
		{
			// Token: 0x060007F7 RID: 2039 RVA: 0x00033890 File Offset: 0x00031A90
			public void OnEnter(Cthulhu iOwner)
			{
				iOwner.mStageSpeedModifier = 1f;
				iOwner.mTimeBetweenActions = 1.5f;
				iOwner.mResistances[Spell.ElementIndex(Elements.Cold)].Multiplier = 0.9f;
				iOwner.mNumberOfTentacles = 2;
				for (int i = 0; i < iOwner.mNumberOfTentacles; i++)
				{
					iOwner.SpawnNewTentacleAtGoodPoint();
				}
				iOwner.GetRageState.Callback = null;
			}

			// Token: 0x060007F8 RID: 2040 RVA: 0x000338F8 File Offset: 0x00031AF8
			public void OnUpdate(float iDeltaTime, Cthulhu iOwner)
			{
				for (int i = 0; i < iOwner.mStates.Length; i++)
				{
					iOwner.mStates[i].NonActiveUpdate(iOwner, iDeltaTime);
				}
				if (iOwner.mCurrentState == Cthulhu.States.Idle && iOwner.GetIdleState.Done)
				{
					if (iOwner.GetEmergeState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Emerge);
						return;
					}
					if (iOwner.GetSubmergeState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Submerge);
						return;
					}
					if (iOwner.GetLightningState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Lightning);
						return;
					}
					if (iOwner.GetDevourState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Devour);
					}
				}
			}

			// Token: 0x060007F9 RID: 2041 RVA: 0x00033997 File Offset: 0x00031B97
			public void OnExit(Cthulhu iOwner)
			{
			}
		}

		// Token: 0x02000104 RID: 260
		private class BattleStage : IBossState<Cthulhu>
		{
			// Token: 0x060007FB RID: 2043 RVA: 0x000339A4 File Offset: 0x00031BA4
			public void OnEnter(Cthulhu iOwner)
			{
				iOwner.mStageSpeedModifier = 1f;
				iOwner.mTimeBetweenActions = 1f;
				iOwner.mNumberOfTentacles = 2;
				iOwner.mResistances[Spell.ElementIndex(Elements.Cold)].Multiplier = 0.7f;
				iOwner.GetRageState.Callback = null;
				iOwner.ChangeState(Cthulhu.States.NewStage);
			}

			// Token: 0x060007FC RID: 2044 RVA: 0x00033A00 File Offset: 0x00031C00
			public void OnUpdate(float iDeltaTime, Cthulhu iOwner)
			{
				for (int i = 0; i < iOwner.mStates.Length; i++)
				{
					iOwner.mStates[i].NonActiveUpdate(iOwner, iDeltaTime);
				}
				if (iOwner.mCurrentState == Cthulhu.States.Idle && iOwner.GetIdleState.Done)
				{
					if (iOwner.GetEmergeState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Emerge);
						return;
					}
					if (iOwner.GetMistState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Mist);
						return;
					}
					if (iOwner.GetSubmergeState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Submerge);
						return;
					}
					if (iOwner.GetLesserCallofCthulhuState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Lesser_Call_of_Cthulhu);
						return;
					}
					if (iOwner.GetLightningState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Lightning);
						return;
					}
					if (iOwner.GetHypnotizeState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Hypnotize);
						return;
					}
					if (iOwner.GetOtherworldlyBoltState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.OtherworldlyBolt);
						return;
					}
					if (iOwner.GetDevourState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Devour);
					}
				}
			}

			// Token: 0x060007FD RID: 2045 RVA: 0x00033B03 File Offset: 0x00031D03
			public void OnExit(Cthulhu iOwner)
			{
			}
		}

		// Token: 0x02000105 RID: 261
		private class LateBattleStage : IBossState<Cthulhu>
		{
			// Token: 0x060007FF RID: 2047 RVA: 0x00033B10 File Offset: 0x00031D10
			public void OnEnter(Cthulhu iOwner)
			{
				iOwner.mStageSpeedModifier = 1f;
				iOwner.mTimeBetweenActions = 1f;
				iOwner.mNumberOfTentacles = 4;
				iOwner.mResistances[Spell.ElementIndex(Elements.Cold)].Multiplier = 0.5f;
				Cthulhu.RageState getRageState = iOwner.GetRageState;
				getRageState.Callback = (Action<Cthulhu>)Delegate.Combine(getRageState.Callback, new Action<Cthulhu>(this.SpawnTentacles));
				iOwner.ChangeState(Cthulhu.States.NewStage);
			}

			// Token: 0x06000800 RID: 2048 RVA: 0x00033B84 File Offset: 0x00031D84
			private void SpawnTentacles(Cthulhu iOwner)
			{
				int num = iOwner.mNumberOfTentacles - iOwner.mActiveTentacles.Count;
				for (int i = 0; i < num; i++)
				{
					iOwner.SpawnNewTentacleAtGoodPoint();
				}
				iOwner.GetRageState.Callback = null;
			}

			// Token: 0x06000801 RID: 2049 RVA: 0x00033BC4 File Offset: 0x00031DC4
			public void OnUpdate(float iDeltaTime, Cthulhu iOwner)
			{
				for (int i = 0; i < iOwner.mStates.Length; i++)
				{
					iOwner.mStates[i].NonActiveUpdate(iOwner, iDeltaTime);
				}
				if (iOwner.mCurrentState == Cthulhu.States.Idle && iOwner.GetIdleState.Done)
				{
					if (iOwner.GetEmergeState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Emerge);
						return;
					}
					if (iOwner.GetMistState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Mist);
						return;
					}
					if (iOwner.GetSubmergeState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Submerge);
						return;
					}
					if (iOwner.GetLesserCallofCthulhuState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Lesser_Call_of_Cthulhu);
						return;
					}
					if (iOwner.GetLightningState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Lightning);
						return;
					}
					if (iOwner.GetHypnotizeState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Hypnotize);
						return;
					}
					if (iOwner.GetOtherworldlyBoltState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.OtherworldlyBolt);
						return;
					}
					if (iOwner.GetDevourState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Devour);
					}
				}
			}

			// Token: 0x06000802 RID: 2050 RVA: 0x00033CC7 File Offset: 0x00031EC7
			public void OnExit(Cthulhu iOwner)
			{
			}
		}

		// Token: 0x02000106 RID: 262
		private class CriticalStage : IBossState<Cthulhu>
		{
			// Token: 0x06000804 RID: 2052 RVA: 0x00033CD4 File Offset: 0x00031ED4
			public void OnEnter(Cthulhu iOwner)
			{
				iOwner.mStageSpeedModifier = 1.2f;
				iOwner.mTimeBetweenActions = 0.75f;
				iOwner.mNumberOfTentacles = 4;
				iOwner.mResistances[Spell.ElementIndex(Elements.Cold)].Multiplier = 0.25f;
				Cthulhu.RageState getRageState = iOwner.GetRageState;
				getRageState.Callback = (Action<Cthulhu>)Delegate.Combine(getRageState.Callback, new Action<Cthulhu>(this.SpawnTentacles));
				iOwner.ChangeState(Cthulhu.States.NewStage);
			}

			// Token: 0x06000805 RID: 2053 RVA: 0x00033D48 File Offset: 0x00031F48
			private void SpawnTentacles(Cthulhu iOwner)
			{
				int num = iOwner.mNumberOfTentacles - iOwner.mActiveTentacles.Count;
				for (int i = 0; i < num; i++)
				{
					iOwner.SpawnNewTentacleAtGoodPoint();
				}
				iOwner.GetRageState.Callback = null;
			}

			// Token: 0x06000806 RID: 2054 RVA: 0x00033D88 File Offset: 0x00031F88
			public void OnUpdate(float iDeltaTime, Cthulhu iOwner)
			{
				for (int i = 0; i < iOwner.mStates.Length; i++)
				{
					iOwner.mStates[i].NonActiveUpdate(iOwner, iDeltaTime);
				}
				if (iOwner.mCurrentState == Cthulhu.States.Idle && iOwner.GetIdleState.Done)
				{
					if (!iOwner.mStarSpawnSpawned && iOwner.GetCallofCthulhuState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Call_of_Cthulhu);
						iOwner.mStarSpawnSpawned = true;
						return;
					}
					if (iOwner.GetEmergeState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Emerge);
						return;
					}
					if (iOwner.GetTimewarpState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Timewarp);
						return;
					}
					if (iOwner.GetMistState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Mist);
						return;
					}
					if (iOwner.GetSubmergeState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Submerge);
						return;
					}
					if (iOwner.GetLesserCallofCthulhuState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Lesser_Call_of_Cthulhu);
						return;
					}
					if (iOwner.GetLightningState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Lightning);
						return;
					}
					if (iOwner.GetHypnotizeState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Hypnotize);
						return;
					}
					if (iOwner.GetOtherworldlyBoltState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.OtherworldlyBolt);
						return;
					}
					if (iOwner.GetDevourState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Devour);
					}
				}
			}

			// Token: 0x06000807 RID: 2055 RVA: 0x00033EC9 File Offset: 0x000320C9
			public void OnExit(Cthulhu iOwner)
			{
			}
		}

		// Token: 0x02000107 RID: 263
		private class FinalStage : IBossState<Cthulhu>
		{
			// Token: 0x06000809 RID: 2057 RVA: 0x00033ED4 File Offset: 0x000320D4
			public void OnEnter(Cthulhu iOwner)
			{
				iOwner.mStageSpeedModifier = 1.3f;
				iOwner.mTimeBetweenActions = 0.5f;
				iOwner.mNumberOfTentacles = 4;
				iOwner.mResistances[Spell.ElementIndex(Elements.Cold)].Multiplier = 0.25f;
				Cthulhu.RageState getRageState = iOwner.GetRageState;
				getRageState.Callback = (Action<Cthulhu>)Delegate.Combine(getRageState.Callback, new Action<Cthulhu>(this.SpawnTentacles));
				iOwner.ChangeState(Cthulhu.States.NewStage);
			}

			// Token: 0x0600080A RID: 2058 RVA: 0x00033F48 File Offset: 0x00032148
			private void SpawnTentacles(Cthulhu iOwner)
			{
				int num = iOwner.mNumberOfTentacles - iOwner.mActiveTentacles.Count;
				for (int i = 0; i < num; i++)
				{
					iOwner.SpawnNewTentacleAtGoodPoint();
				}
				iOwner.GetRageState.Callback = null;
			}

			// Token: 0x0600080B RID: 2059 RVA: 0x00033F88 File Offset: 0x00032188
			public void OnUpdate(float iDeltaTime, Cthulhu iOwner)
			{
				for (int i = 0; i < iOwner.mStates.Length; i++)
				{
					iOwner.mStates[i].NonActiveUpdate(iOwner, iDeltaTime);
				}
				if (iOwner.mCurrentState == Cthulhu.States.Idle && iOwner.GetIdleState.Done)
				{
					if (!iOwner.mStarSpawnSpawned && iOwner.GetCallofCthulhuState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Call_of_Cthulhu);
						iOwner.mStarSpawnSpawned = true;
						return;
					}
					if (iOwner.GetMistState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Mist);
						return;
					}
					if (iOwner.GetTimewarpState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Timewarp);
						return;
					}
					if (iOwner.GetLesserCallofCthulhuState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Lesser_Call_of_Cthulhu);
						return;
					}
					if (iOwner.GetEmergeState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Emerge);
						return;
					}
					if (iOwner.GetSubmergeState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Submerge);
						return;
					}
					if (iOwner.GetLightningState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Lightning);
						return;
					}
					if (iOwner.GetHypnotizeState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Hypnotize);
						return;
					}
					if (iOwner.GetOtherworldlyBoltState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.OtherworldlyBolt);
						return;
					}
					if (iOwner.GetDevourState.Active(iOwner, iDeltaTime))
					{
						iOwner.ChangeState(Cthulhu.States.Devour);
					}
				}
			}

			// Token: 0x0600080C RID: 2060 RVA: 0x000340C9 File Offset: 0x000322C9
			public void OnExit(Cthulhu iOwner)
			{
			}
		}

		// Token: 0x02000108 RID: 264
		public enum States
		{
			// Token: 0x04000722 RID: 1826
			Idle,
			// Token: 0x04000723 RID: 1827
			Emerge,
			// Token: 0x04000724 RID: 1828
			Submerge,
			// Token: 0x04000725 RID: 1829
			Devour,
			// Token: 0x04000726 RID: 1830
			DevourHit,
			// Token: 0x04000727 RID: 1831
			Lightning,
			// Token: 0x04000728 RID: 1832
			Mist,
			// Token: 0x04000729 RID: 1833
			Call_of_Cthulhu,
			// Token: 0x0400072A RID: 1834
			Lesser_Call_of_Cthulhu,
			// Token: 0x0400072B RID: 1835
			Timewarp,
			// Token: 0x0400072C RID: 1836
			Hypnotize,
			// Token: 0x0400072D RID: 1837
			NewStage,
			// Token: 0x0400072E RID: 1838
			OtherworldlyBolt,
			// Token: 0x0400072F RID: 1839
			Death,
			// Token: 0x04000730 RID: 1840
			NR_OF_STATES
		}

		// Token: 0x02000109 RID: 265
		private interface CthulhuState : IBossState<Cthulhu>
		{
			// Token: 0x0600080E RID: 2062
			bool Active(Cthulhu iOwner, float iDeltaTime);

			// Token: 0x0600080F RID: 2063
			void NonActiveUpdate(Cthulhu iOwner, float iDeltaTime);
		}

		// Token: 0x0200010A RID: 266
		private class IdleState : Cthulhu.CthulhuState, IBossState<Cthulhu>
		{
			// Token: 0x06000810 RID: 2064 RVA: 0x000340D4 File Offset: 0x000322D4
			public void OnEnter(Cthulhu iOwner)
			{
				Cthulhu.States mPreviousState = iOwner.mPreviousState;
				float time;
				if (mPreviousState == Cthulhu.States.Devour)
				{
					time = 0.4f;
				}
				else
				{
					time = 0.05f;
				}
				iOwner.CrossFade(Cthulhu.Animations.Idle, time, true);
				this.mTimer = iOwner.mTimeBetweenActions;
				this.Done = false;
			}

			// Token: 0x06000811 RID: 2065 RVA: 0x0003411D File Offset: 0x0003231D
			public void OnUpdate(float iDeltaTime, Cthulhu iOwner)
			{
				this.mTimer -= iDeltaTime;
				if (this.mTimer < 0f && iOwner.mOkToFight)
				{
					this.Done = true;
				}
			}

			// Token: 0x06000812 RID: 2066 RVA: 0x00034149 File Offset: 0x00032349
			public void OnExit(Cthulhu iOwner)
			{
			}

			// Token: 0x06000813 RID: 2067 RVA: 0x0003414B File Offset: 0x0003234B
			public bool Active(Cthulhu iOwner, float iDeltaTime)
			{
				return false;
			}

			// Token: 0x06000814 RID: 2068 RVA: 0x0003414E File Offset: 0x0003234E
			public void NonActiveUpdate(Cthulhu iOwner, float iDeltaTime)
			{
			}

			// Token: 0x04000731 RID: 1841
			private float mTimer;

			// Token: 0x04000732 RID: 1842
			public bool Done;
		}

		// Token: 0x0200010B RID: 267
		private class MistState : Cthulhu.CthulhuState, IBossState<Cthulhu>
		{
			// Token: 0x06000816 RID: 2070 RVA: 0x00034158 File Offset: 0x00032358
			public void OnEnter(Cthulhu iOwner)
			{
				iOwner.CrossFade(Cthulhu.Animations.Mists, 0.4f, false);
				this.mCasted = false;
				AudioManager.Instance.PlayCue(Banks.Additional, Cthulhu.SOUND_MIST, iOwner.mDamageZone.AudioEmitter);
			}

			// Token: 0x06000817 RID: 2071 RVA: 0x00034190 File Offset: 0x00032390
			public unsafe void OnUpdate(float iDeltaTime, Cthulhu iOwner)
			{
				if (NetworkManager.Instance.State == NetworkState.Client)
				{
					return;
				}
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
					if (iOwner.mAnimationController.HasFinished)
					{
						iOwner.ChangeState(Cthulhu.States.Idle);
						return;
					}
					if (!this.mCasted && num >= 0.2631579f)
					{
						iOwner.ActivateMist();
						this.mCasted = true;
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							Cthulhu.ActivateMistMessage activateMistMessage = default(Cthulhu.ActivateMistMessage);
							BossFight.Instance.SendMessage<Cthulhu.ActivateMistMessage>(iOwner, 7, (void*)(&activateMistMessage), true);
						}
					}
				}
			}

			// Token: 0x06000818 RID: 2072 RVA: 0x0003422C File Offset: 0x0003242C
			public void OnExit(Cthulhu iOwner)
			{
				this.mCooldown = 10f;
			}

			// Token: 0x06000819 RID: 2073 RVA: 0x00034239 File Offset: 0x00032439
			public bool Active(Cthulhu iOwner, float iDeltaTime)
			{
				return this.mCooldown <= 0f && iOwner.mActivateMist && !iOwner.mMistCloud.Active;
			}

			// Token: 0x0600081A RID: 2074 RVA: 0x00034262 File Offset: 0x00032462
			public void NonActiveUpdate(Cthulhu iOwner, float iDeltaTime)
			{
				if (this.mCooldown >= 0f)
				{
					this.mCooldown -= iDeltaTime;
				}
			}

			// Token: 0x04000733 RID: 1843
			private const float TIME = 0.2631579f;

			// Token: 0x04000734 RID: 1844
			private bool mCasted;

			// Token: 0x04000735 RID: 1845
			private float mCooldown;
		}

		// Token: 0x0200010C RID: 268
		private class EmergeState : Cthulhu.CthulhuState, IBossState<Cthulhu>
		{
			// Token: 0x0600081C RID: 2076 RVA: 0x00034288 File Offset: 0x00032488
			public void OnEnter(Cthulhu iOwner)
			{
				this.mEffectDone = (this.mLogicDone = false);
				iOwner.mHPOnLastEmerge = iOwner.HitPoints;
				iOwner.mTimeSinceLastEmerge = 0f;
				iOwner.mTimeUntilSubmerge = 30f + (float)Cthulhu.RANDOM.NextDouble() * 5f;
				if (iOwner.mDesiredSpawnPoint != -1)
				{
					Matrix mTransform = iOwner.ChangeSpawnPoint(iOwner.mDesiredSpawnPoint, (int)iOwner.mDamageZone.Handle);
					iOwner.mTransform = mTransform;
				}
				StatusEffect iStatusEffect = new StatusEffect(StatusEffects.Wet, 0f, 1f, 1f, 1f);
				iOwner.AddStatusEffect(iStatusEffect);
				this.mUnfreezeDone = false;
				if (iOwner.InitialEmerge || iOwner.mTheKingHasFallen)
				{
					this.mBubblesDone = true;
					this.mBubbleTimer = -1f;
					iOwner.StartClip(Cthulhu.Animations.Emerge, false);
				}
				else
				{
					this.mBubblesDone = false;
					this.mBubbleTimer = 1.5f;
					iOwner.StartClip(Cthulhu.Animations.Emerge, false);
					iOwner.StopClip();
				}
				AudioManager.Instance.PlayCue(Banks.Additional, Cthulhu.SOUND_EMERGE, iOwner.mDamageZone.AudioEmitter);
			}

			// Token: 0x0600081D RID: 2077 RVA: 0x00034398 File Offset: 0x00032598
			public void OnUpdate(float iDeltaTime, Cthulhu iOwner)
			{
				if (this.mBubbleTimer >= 0f)
				{
					this.mBubbleTimer -= iDeltaTime;
					if (this.mBubbleTimer <= 0f)
					{
						iOwner.StartClip(Cthulhu.Animations.Emerge, false);
						if (EffectManager.Instance.IsActive(ref this.mBubbleEffectRef))
						{
							EffectManager.Instance.Stop(ref this.mBubbleEffectRef);
							return;
						}
					}
					else if (!this.mBubblesDone && !EffectManager.Instance.IsActive(ref this.mBubbleEffectRef))
					{
						Vector3 translation = iOwner.mTransform.Translation;
						translation.Y = iOwner.WaterYpos;
						Matrix matrix = Matrix.CreateTranslation(translation);
						EffectManager.Instance.StartEffect(this.BubbleEffect, ref matrix, out this.mBubbleEffectRef);
						this.mBubblesDone = true;
						return;
					}
				}
				else
				{
					float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
					if (iOwner.mTheKingHasFallen && num >= 0.42f)
					{
						iOwner.ChangeState(Cthulhu.States.Death);
						return;
					}
					if (iOwner.mAnimationController.HasFinished)
					{
						iOwner.ChangeState(Cthulhu.States.Idle);
						return;
					}
					if (!this.mUnfreezeDone && num >= 0.07272727f)
					{
						Vector3 translation2 = iOwner.mTransform.Translation;
						Vector3 right = Vector3.Right;
						Vector3.Multiply(ref right, 4f, out right);
						Damage damage = new Damage(AttackProperties.Status, Elements.Fire, 100f, 4f);
						Liquid.Freeze(iOwner.mPlayState.Level.CurrentScene, ref translation2, ref right, 6.2831855f, 2f, ref damage);
						this.mUnfreezeDone = true;
					}
					if (!this.mLogicDone && num >= 0.21818182f)
					{
						Entity entity = iOwner.Entity;
						Vector3 position = entity.Position;
						Damage damage2 = new Damage(AttackProperties.Knockback, Elements.Earth, 100f, 4f);
						Helper.CircleDamage(iOwner.mPlayState, entity, iOwner.mPlayState.PlayTime, entity, ref position, 6f, ref damage2);
						Damage damage3 = new Damage(AttackProperties.Status, Elements.Water, 100f, 1f);
						Helper.CircleDamage(iOwner.mPlayState, entity, iOwner.mPlayState.PlayTime, entity, ref position, 10f, ref damage3);
						this.mLogicDone = true;
						return;
					}
					if (!this.mEffectDone && num >= 0f)
					{
						if (!EffectManager.Instance.IsActive(ref this.mEffectRef))
						{
							Vector3 translation3 = iOwner.mTransform.Translation;
							translation3.Y = 0f;
							Matrix matrix2 = Matrix.CreateTranslation(translation3);
							EffectManager.Instance.StartEffect(this.WaterSplashEffect, ref matrix2, out this.mEffectRef);
						}
						this.mEffectDone = true;
						iOwner.TurnOnIdleEffect();
					}
				}
			}

			// Token: 0x0600081E RID: 2078 RVA: 0x00034632 File Offset: 0x00032832
			public void OnExit(Cthulhu iOwner)
			{
				iOwner.mInitialEmerge = false;
				if (EffectManager.Instance.IsActive(ref this.mBubbleEffectRef))
				{
					EffectManager.Instance.Stop(ref this.mBubbleEffectRef);
				}
			}

			// Token: 0x0600081F RID: 2079 RVA: 0x0003465D File Offset: 0x0003285D
			public bool Active(Cthulhu iOwner, float iDeltaTime)
			{
				return false;
			}

			// Token: 0x06000820 RID: 2080 RVA: 0x00034660 File Offset: 0x00032860
			public void NonActiveUpdate(Cthulhu iOwner, float iDeltaTime)
			{
			}

			// Token: 0x04000736 RID: 1846
			private const float LOGIC_EXECUTE_TIME = 0.21818182f;

			// Token: 0x04000737 RID: 1847
			private const float PFX_EXECUTE_TIME = 0f;

			// Token: 0x04000738 RID: 1848
			private const float UNFREEZE_TIME = 0.07272727f;

			// Token: 0x04000739 RID: 1849
			private VisualEffectReference mEffectRef;

			// Token: 0x0400073A RID: 1850
			private int WaterSplashEffect = "cthulhu_emerge_water_splash".GetHashCodeCustom();

			// Token: 0x0400073B RID: 1851
			private VisualEffectReference mBubbleEffectRef;

			// Token: 0x0400073C RID: 1852
			private int BubbleEffect = "cthulhu_intro_bubbles".GetHashCodeCustom();

			// Token: 0x0400073D RID: 1853
			private bool mLogicDone;

			// Token: 0x0400073E RID: 1854
			private bool mEffectDone;

			// Token: 0x0400073F RID: 1855
			private bool mUnfreezeDone;

			// Token: 0x04000740 RID: 1856
			private bool mBubblesDone;

			// Token: 0x04000741 RID: 1857
			private float mBubbleTimer;
		}

		// Token: 0x0200010D RID: 269
		private class SubmergeState : Cthulhu.CthulhuState, IBossState<Cthulhu>
		{
			// Token: 0x06000822 RID: 2082 RVA: 0x0003468C File Offset: 0x0003288C
			public void OnEnter(Cthulhu iOwner)
			{
				iOwner.CrossFade(Cthulhu.Animations.Submerge, 0.4f, false);
				this.mEffectDone = (this.mLogicDone = false);
				this.mTimer = 2f;
				iOwner.mDeflectionTimer = 0f;
				AudioManager.Instance.PlayCue(Banks.Additional, Cthulhu.SOUND_SUBMERGE, iOwner.mDamageZone.AudioEmitter);
			}

			// Token: 0x06000823 RID: 2083 RVA: 0x000346EC File Offset: 0x000328EC
			public unsafe void OnUpdate(float iDeltaTime, Cthulhu iOwner)
			{
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
					if (!iOwner.mAnimationController.CrossFadeEnabled && iOwner.mAnimationController.HasFinished)
					{
						if (this.mTimer <= 0f)
						{
							if (NetworkManager.Instance.State != NetworkState.Client)
							{
								int num2 = iOwner.mDesiredSpawnPoint;
								iOwner.mDesiredSpawnPoint = -1;
								if (num2 == -1)
								{
									num2 = iOwner.FindGoodSpot(18f);
								}
								if (num2 == -1)
								{
									num2 = iOwner.FindRandomSpot();
								}
								if (num2 != -1)
								{
									Matrix mTransform = iOwner.ChangeSpawnPoint(num2, (int)iOwner.mDamageZone.Handle);
									iOwner.mTransform = mTransform;
								}
								if (NetworkManager.Instance.State == NetworkState.Server)
								{
									Cthulhu.SpawnPointMessage spawnPointMessage = default(Cthulhu.SpawnPointMessage);
									spawnPointMessage.Index = -1;
									spawnPointMessage.SpawnPoint = num2;
									BossFight.Instance.SendMessage<Cthulhu.SpawnPointMessage>(iOwner, 4, (void*)(&spawnPointMessage), true);
								}
							}
							iOwner.ChangeState(Cthulhu.States.Emerge);
						}
						this.mTimer -= iDeltaTime;
						return;
					}
					if (!this.mLogicDone && num >= 0.6976744f)
					{
						Entity entity = iOwner.Entity;
						Vector3 translation = iOwner.mTransform.Translation;
						Damage damage = new Damage(AttackProperties.Pushed, Elements.Earth, 100f, 4f);
						Helper.CircleDamage(iOwner.mPlayState, entity, iOwner.mPlayState.PlayTime, entity, ref translation, 6f, ref damage);
						Damage damage2 = new Damage(AttackProperties.Status, Elements.Water, 100f, 1f);
						Helper.CircleDamage(iOwner.mPlayState, entity, iOwner.mPlayState.PlayTime, entity, ref translation, 6f, ref damage2);
						this.mLogicDone = true;
						return;
					}
					if (!this.mEffectDone && num >= 0.5813953f)
					{
						if (!EffectManager.Instance.IsActive(ref this.mEffectRef))
						{
							Vector3 translation2 = iOwner.mTransform.Translation;
							translation2.Y = 0f;
							Matrix matrix = Matrix.CreateTranslation(translation2);
							EffectManager.Instance.StartEffect(this.WaterSplashEffect, ref matrix, out this.mEffectRef);
						}
						iOwner.KillIdleEffect();
						this.mEffectDone = true;
					}
				}
			}

			// Token: 0x06000824 RID: 2084 RVA: 0x0003490B File Offset: 0x00032B0B
			public void OnExit(Cthulhu iOwner)
			{
				iOwner.ClearAllStatusEffects();
			}

			// Token: 0x06000825 RID: 2085 RVA: 0x00034914 File Offset: 0x00032B14
			public bool Active(Cthulhu iOwner, float iDeltaTime)
			{
				return iOwner.mHPOnLastEmerge - iOwner.HitPoints > 0.1f * iOwner.MaxHitPoints || iOwner.mTimeUntilSubmerge < 0f || (iOwner.mTimeSinceLastEmerge > 5f && iOwner.mSecondsWhileOutOfRange > 1);
			}

			// Token: 0x06000826 RID: 2086 RVA: 0x00034966 File Offset: 0x00032B66
			public void NonActiveUpdate(Cthulhu iOwner, float iDeltaTime)
			{
			}

			// Token: 0x04000742 RID: 1858
			private const float LOGIC_EXECUTE_TIME = 0.6976744f;

			// Token: 0x04000743 RID: 1859
			private const float PFX_EXECUTE_TIME = 0.5813953f;

			// Token: 0x04000744 RID: 1860
			private float mTimer;

			// Token: 0x04000745 RID: 1861
			private VisualEffectReference mEffectRef;

			// Token: 0x04000746 RID: 1862
			private int WaterSplashEffect = "cthulhu_emerge_water_splash".GetHashCodeCustom();

			// Token: 0x04000747 RID: 1863
			private bool mLogicDone;

			// Token: 0x04000748 RID: 1864
			private bool mEffectDone;
		}

		// Token: 0x0200010E RID: 270
		private class TimewarpState : Cthulhu.CthulhuState, IBossState<Cthulhu>
		{
			// Token: 0x06000828 RID: 2088 RVA: 0x00034980 File Offset: 0x00032B80
			public void OnEnter(Cthulhu iOwner)
			{
				AudioManager.Instance.PlayCue(Banks.Additional, Cthulhu.SOUND_HOWL, iOwner.mDamageZone.AudioEmitter);
				iOwner.CrossFade(Cthulhu.Animations.Timewarp, 0.4f, false);
				this.mCasted = false;
				this.mSoundPlayed = false;
			}

			// Token: 0x06000829 RID: 2089 RVA: 0x000349C0 File Offset: 0x00032BC0
			public unsafe void OnUpdate(float iDeltaTime, Cthulhu iOwner)
			{
				if (NetworkManager.Instance.State == NetworkState.Client)
				{
					return;
				}
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
					if (iOwner.mAnimationController.HasFinished)
					{
						iOwner.ChangeState(Cthulhu.States.Idle);
						return;
					}
					if (!this.mCasted && num >= 0.3939394f)
					{
						if (!this.mSoundPlayed)
						{
							AudioManager.Instance.PlayCue(Banks.Spells, Cthulhu.SOUND_TIMEWARP, iOwner.mDamageZone.AudioEmitter);
							this.mSoundPlayed = true;
						}
						TimeWarp.Instance.Execute(iOwner.mDamageZone, iOwner.mPlayState);
						this.mCasted = true;
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							Cthulhu.TimewarpMessage timewarpMessage = default(Cthulhu.TimewarpMessage);
							BossFight.Instance.SendMessage<Cthulhu.TimewarpMessage>(iOwner, 6, (void*)(&timewarpMessage), true);
						}
					}
				}
			}

			// Token: 0x0600082A RID: 2090 RVA: 0x00034A9C File Offset: 0x00032C9C
			public void OnExit(Cthulhu iOwner)
			{
				this.mCooldown = 20f;
			}

			// Token: 0x0600082B RID: 2091 RVA: 0x00034AAC File Offset: 0x00032CAC
			public bool Active(Cthulhu iOwner, float iDeltaTime)
			{
				return iOwner.mCurrentState != Cthulhu.States.Timewarp && this.mCooldown <= 0f && !SpellManager.Instance.IsEffectActive(typeof(TimeWarp)) && iOwner.mTimeSinceLastDamage > 2f;
			}

			// Token: 0x0600082C RID: 2092 RVA: 0x00034AFC File Offset: 0x00032CFC
			public void NonActiveUpdate(Cthulhu iOwner, float iDeltaTime)
			{
				if (SpellManager.Instance.IsEffectActive(typeof(TimeWarp)) && this.mCooldown >= 0f)
				{
					this.mCooldown -= iDeltaTime;
				}
			}

			// Token: 0x04000749 RID: 1865
			private const float TIME = 0.3939394f;

			// Token: 0x0400074A RID: 1866
			private float mCooldown;

			// Token: 0x0400074B RID: 1867
			private bool mCasted;

			// Token: 0x0400074C RID: 1868
			private bool mSoundPlayed;
		}

		// Token: 0x0200010F RID: 271
		private class LightningState : Cthulhu.CthulhuState, IBossState<Cthulhu>
		{
			// Token: 0x0600082E RID: 2094 RVA: 0x00034B38 File Offset: 0x00032D38
			public LightningState()
			{
				this.mHitList = new HitList(16);
				this.mTimers = new SortedList<float, int>();
				this.mDamageCollection = default(DamageCollection5);
				Damage iDamage = new Damage(AttackProperties.Damage, Elements.Lightning, 8f, 1f);
				this.mDamageCollection.AddDamage(iDamage);
			}

			// Token: 0x0600082F RID: 2095 RVA: 0x00034B90 File Offset: 0x00032D90
			public void OnEnter(Cthulhu iOwner)
			{
				this.mTimers.Clear();
				iOwner.CrossFade(Cthulhu.Animations.Cast_Lightning, 0.4f, false);
				this.mHitList.Clear();
				for (int i = 0; i < 12; i++)
				{
					float key;
					do
					{
						key = 0f + (float)i / 1000f;
					}
					while (this.mTimers.ContainsKey(key));
					this.mTimers.Add(key, i);
				}
				this.mTimer = 0f;
				this.mSoundCue = null;
			}

			// Token: 0x06000830 RID: 2096 RVA: 0x00034C0C File Offset: 0x00032E0C
			public void OnUpdate(float iDeltaTime, Cthulhu iOwner)
			{
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					this.mHitList.Update(iDeltaTime);
					float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
					if (iOwner.mAnimationController.HasFinished)
					{
						iOwner.ChangeState(Cthulhu.States.Idle);
						return;
					}
					if (num >= 0.26744187f && num <= 0.8372093f)
					{
						if (this.mSoundCue == null)
						{
							this.mSoundCue = AudioManager.Instance.PlayCue(Banks.Spells, Cthulhu.SOUND_LIGHTNING, iOwner.mDamageZone.AudioEmitter);
						}
						this.mTimer -= iDeltaTime;
						while (this.mTimers.Count > 0)
						{
							if (this.mTimers.First<KeyValuePair<float, int>>().Key + this.mTimer >= 0f)
							{
								return;
							}
							float num2 = 6f;
							int value = this.mTimers.First<KeyValuePair<float, int>>().Value;
							this.mTimers.RemoveAt(0);
							LightningBolt lightning = LightningBolt.GetLightning();
							Vector3 vector;
							if (value >= 6)
							{
								vector = iOwner.mRightFingerOrientation[value - 6].Translation + iOwner.mTransform.Forward;
							}
							else
							{
								vector = iOwner.mLeftFingerOrientation[value].Translation + iOwner.mTransform.Forward;
							}
							float num3 = (float)Cthulhu.RANDOM.NextDouble() / 4f + 0.15f;
							this.mHitList.Add(iOwner.mDamageZone);
							foreach (Tentacle iEntity in iOwner.mActiveTentacles)
							{
								this.mHitList.Add(iEntity);
							}
							Character iTarget;
							if (!this.FindTarget(iOwner, num2, 0f, vector, out iTarget))
							{
								Vector3 vector2 = iOwner.mTransform.Forward;
								float scaleFactor = 0.7f;
								vector2 += iOwner.mTransform.Right * scaleFactor;
								vector2 += iOwner.mTransform.Left * 2f * (float)Cthulhu.RANDOM.NextDouble() * scaleFactor;
								vector2.Normalize();
								lightning.Cast(iOwner.mDamageZone, vector, vector2, this.mHitList, new Vector3(0.2f, 0.2f, 0.2f), 1f, num2 + (float)(Cthulhu.RANDOM.NextDouble() * 6.0), ref this.mDamageCollection, null, iOwner.mPlayState);
							}
							else
							{
								lightning.Cast(iOwner.mDamageZone, vector, iTarget, this.mHitList, new Vector3(0.2f, 0.2f, 0.2f), 1f, num2 + (float)(Cthulhu.RANDOM.NextDouble() * 6.0), ref this.mDamageCollection, iOwner.mPlayState);
							}
							lightning.TTL = num3;
							float num4 = num3 - this.mTimer;
							while (this.mTimers.ContainsKey(num4))
							{
								num4 += 1E-06f;
							}
							this.mTimers.Add(num4, value);
						}
					}
					else if (num >= 0.8372093f)
					{
						if (this.mSoundCue != null && !this.mSoundCue.IsStopping)
						{
							this.mSoundCue.Stop(AudioStopOptions.AsAuthored);
						}
						this.mSoundCue = null;
					}
				}
			}

			// Token: 0x06000831 RID: 2097 RVA: 0x00034F84 File Offset: 0x00033184
			private bool FindTarget(Cthulhu iOwner, float radius, float tooCloseDistance, Vector3 pos, out Character target)
			{
				EntityManager entityManager = iOwner.mPlayState.EntityManager;
				List<Entity> entities = entityManager.GetEntities(pos, radius, true);
				bool flag = false;
				target = null;
				int num = 0;
				while (num < entities.Count && !flag)
				{
					Avatar avatar = entities[num] as Avatar;
					if (avatar != null && !avatar.IsEthereal && !this.mHitList.Contains(avatar))
					{
						target = avatar;
						Vector3 vector = avatar.Position - pos;
						vector.Y = 0f;
						float num2 = vector.LengthSquared();
						if (num2 > tooCloseDistance * tooCloseDistance)
						{
							vector.Normalize();
							Vector3 forward = iOwner.mTransform.Forward;
							float num3 = Vector3.Dot(vector, forward);
							flag = (num3 > 0.7f);
						}
					}
					num++;
				}
				entityManager.ReturnEntityList(entities);
				return flag;
			}

			// Token: 0x06000832 RID: 2098 RVA: 0x00035054 File Offset: 0x00033254
			private bool ShouldDoLightning(Cthulhu iOwner, float radius, float tooCloseDistance, Vector3 pos, out Character target)
			{
				EntityManager entityManager = iOwner.mPlayState.EntityManager;
				List<Entity> entities = entityManager.GetEntities(pos, radius, true);
				target = null;
				entities.Remove(iOwner.mDamageZone);
				foreach (Tentacle item in iOwner.mActiveTentacles)
				{
					entities.Remove(item);
				}
				float num = 0f;
				for (int i = 0; i < entities.Count; i++)
				{
					Character character = entities[i] as Character;
					if (character != null && !character.IsEthereal && !this.mHitList.Contains(character))
					{
						Vector3 vector = character.Position - pos;
						vector.Y = 0f;
						float num2 = vector.LengthSquared();
						bool flag = false;
						if (num2 > tooCloseDistance * tooCloseDistance)
						{
							vector.Normalize();
							Vector3 forward = iOwner.mTransform.Forward;
							float num3 = Vector3.Dot(vector, forward);
							flag = (num3 > 0.7f);
						}
						if (flag)
						{
							if ((character.Faction & Factions.FRIENDLY) != Factions.NONE)
							{
								num += character.IsResistantAgainst(Elements.Lightning);
							}
							else
							{
								num -= character.IsResistantAgainst(Elements.Lightning);
							}
						}
					}
				}
				entityManager.ReturnEntityList(entities);
				return num > 0.5f;
			}

			// Token: 0x06000833 RID: 2099 RVA: 0x000351B8 File Offset: 0x000333B8
			public void OnExit(Cthulhu iOwner)
			{
				if (this.mSoundCue != null && !this.mSoundCue.IsStopping)
				{
					this.mSoundCue.Stop(AudioStopOptions.AsAuthored);
				}
				this.mSoundCue = null;
				this.mCooldown = 10f;
			}

			// Token: 0x06000834 RID: 2100 RVA: 0x000351F0 File Offset: 0x000333F0
			public bool Active(Cthulhu iOwner, float iDeltaTime)
			{
				Character character;
				return this.mCooldown < 0f && this.ShouldDoLightning(iOwner, 19f, 5f, iOwner.mTransform.Translation, out character);
			}

			// Token: 0x06000835 RID: 2101 RVA: 0x0003522A File Offset: 0x0003342A
			public void NonActiveUpdate(Cthulhu iOwner, float iDeltaTime)
			{
				if (this.mCooldown >= 0f)
				{
					this.mCooldown -= iDeltaTime;
				}
			}

			// Token: 0x0400074D RID: 1869
			private const float START_TIME = 0.26744187f;

			// Token: 0x0400074E RID: 1870
			private const float END_TIME = 0.8372093f;

			// Token: 0x0400074F RID: 1871
			private HitList mHitList;

			// Token: 0x04000750 RID: 1872
			private DamageCollection5 mDamageCollection;

			// Token: 0x04000751 RID: 1873
			private float mTimer;

			// Token: 0x04000752 RID: 1874
			private SortedList<float, int> mTimers;

			// Token: 0x04000753 RID: 1875
			private Cue mSoundCue;

			// Token: 0x04000754 RID: 1876
			private float mCooldown;
		}

		// Token: 0x02000110 RID: 272
		private class CallState : Cthulhu.CthulhuState, IBossState<Cthulhu>
		{
			// Token: 0x06000836 RID: 2102 RVA: 0x00035247 File Offset: 0x00033447
			public CallState()
			{
				this.mRandomClosedList = new int[4];
				this.mNrSpawns = 1;
			}

			// Token: 0x06000837 RID: 2103 RVA: 0x00035262 File Offset: 0x00033462
			public void OnEnter(Cthulhu iOwner)
			{
				iOwner.CrossFade(Cthulhu.Animations.Call_of_Cthulhu, 0.4f, false);
				this.mCasted = false;
				AudioManager.Instance.PlayCue(Banks.Additional, Cthulhu.SOUND_CALL_OF_CTHULHU, iOwner.mDamageZone.AudioEmitter);
			}

			// Token: 0x06000838 RID: 2104 RVA: 0x0003529C File Offset: 0x0003349C
			public void OnUpdate(float iDeltaTime, Cthulhu iOwner)
			{
				if (NetworkManager.Instance.State == NetworkState.Client)
				{
					return;
				}
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
					if (iOwner.mAnimationController.HasFinished)
					{
						iOwner.ChangeState(Cthulhu.States.Idle);
						return;
					}
					if (!this.mCasted && num >= 0.4864865f)
					{
						for (int i = 0; i < 4; i++)
						{
							this.mRandomClosedList[i] = -1;
						}
						for (int j = 0; j < this.mNrSpawns; j++)
						{
							bool flag = false;
							int num2 = -1;
							while (!flag)
							{
								num2 = Cthulhu.RANDOM.Next(Cthulhu.DEEP_ONES_SPAWN_LOCATORS.Length);
								bool flag2 = false;
								int num3 = 0;
								while (num3 < 4 && !flag2)
								{
									flag2 = (this.mRandomClosedList[num3] == num2);
									num3++;
								}
								flag = !flag2;
							}
							this.mRandomClosedList[j] = num2;
							Matrix matrix = iOwner.mDeepOnesSpawnTransforms[num2];
							this.mRandomClosedList[j] = num2;
							Vector3 iPosition = matrix.Translation;
							Vector3 vector;
							iOwner.mPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref iPosition, out vector, MovementProperties.Default);
							iPosition = vector;
							CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(Cthulhu.CallState.STARSPAWN_HASH);
							NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(iOwner.mPlayState);
							instance.Initialize(cachedTemplate, iPosition, 0);
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
								triggerActionMessage.Point2 = (int)instance.SpawnAnimation;
								NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
							}
						}
						this.mCasted = true;
					}
				}
			}

			// Token: 0x06000839 RID: 2105 RVA: 0x00035501 File Offset: 0x00033701
			public void OnExit(Cthulhu iOwner)
			{
			}

			// Token: 0x0600083A RID: 2106 RVA: 0x00035504 File Offset: 0x00033704
			public bool Active(Cthulhu iOwner, float iDeltaTime)
			{
				TriggerArea triggerArea = iOwner.mPlayState.Level.CurrentScene.GetTriggerArea(Cthulhu.CallState.ANY);
				int num = triggerArea.GetCount(Cthulhu.CallState.STARSPAWN_HASH);
				num += triggerArea.GetCount(Cthulhu.CallState.DEEP_ONE_HASH);
				num += triggerArea.GetCount(Cthulhu.CallState.CULTIST_HASH);
				return num == 0;
			}

			// Token: 0x0600083B RID: 2107 RVA: 0x00035558 File Offset: 0x00033758
			public void NonActiveUpdate(Cthulhu iOwner, float iDeltaTime)
			{
			}

			// Token: 0x04000755 RID: 1877
			private const float LOGIC_EXECUTE_TIME = 0.4864865f;

			// Token: 0x04000756 RID: 1878
			private const float PFX_EXECUTE_TIME = 0f;

			// Token: 0x04000757 RID: 1879
			private const int MAX_NR_SPAWNS = 4;

			// Token: 0x04000758 RID: 1880
			private static readonly int ANY = "any".GetHashCodeCustom();

			// Token: 0x04000759 RID: 1881
			private static readonly int STARSPAWN_HASH = "starspawn".GetHashCodeCustom();

			// Token: 0x0400075A RID: 1882
			private static readonly int DEEP_ONE_HASH = "deep_one".GetHashCodeCustom();

			// Token: 0x0400075B RID: 1883
			private static readonly int CULTIST_HASH = "cultist".GetHashCodeCustom();

			// Token: 0x0400075C RID: 1884
			private int[] mRandomClosedList;

			// Token: 0x0400075D RID: 1885
			private bool mCasted;

			// Token: 0x0400075E RID: 1886
			private int mNrSpawns;
		}

		// Token: 0x02000111 RID: 273
		private class LesserCallState : Cthulhu.CthulhuState, IBossState<Cthulhu>
		{
			// Token: 0x0600083D RID: 2109 RVA: 0x00035598 File Offset: 0x00033798
			public LesserCallState()
			{
				switch (Game.Instance.PlayerCount)
				{
				case 1:
					this.mNrSpawns = 2;
					break;
				case 2:
					this.mNrSpawns = 4;
					break;
				case 3:
					this.mNrSpawns = 5;
					break;
				case 4:
					this.mNrSpawns = 6;
					break;
				default:
					this.mNrSpawns = 1;
					break;
				}
				this.mRandomClosedList = new int[this.mNrSpawns];
				this.mTimers = new SortedList<float, int>(this.mNrSpawns);
			}

			// Token: 0x0600083E RID: 2110 RVA: 0x00035620 File Offset: 0x00033820
			public void OnEnter(Cthulhu iOwner)
			{
				iOwner.CrossFade(Cthulhu.Animations.Call_of_Cthulhu, 0.4f, false);
				AudioManager.Instance.PlayCue(Banks.Additional, Cthulhu.SOUND_LESSER_CALL_OF_CTHULHU, iOwner.mDamageZone.AudioEmitter);
				for (int i = 0; i < this.mNrSpawns; i++)
				{
					this.mRandomClosedList[i] = -1;
				}
				for (int j = 0; j < this.mNrSpawns; j++)
				{
					bool flag = false;
					int num = -1;
					while (!flag)
					{
						num = Cthulhu.RANDOM.Next(Cthulhu.DEEP_ONES_SPAWN_LOCATORS.Length);
						bool flag2 = false;
						int num2 = 0;
						while (num2 < this.mNrSpawns && !flag2)
						{
							flag2 = (this.mRandomClosedList[num2] == num);
							num2++;
						}
						flag = !flag2;
					}
					this.mRandomClosedList[j] = num;
				}
				this.mTimers.Clear();
				for (int k = 0; k < this.mNrSpawns; k++)
				{
					float key;
					do
					{
						key = (float)Cthulhu.RANDOM.NextDouble() + 0.15f;
					}
					while (this.mTimers.ContainsKey(key));
					this.mTimers.Add(key, k);
				}
				this.mTimer = 0f;
			}

			// Token: 0x0600083F RID: 2111 RVA: 0x00035738 File Offset: 0x00033938
			public void OnUpdate(float iDeltaTime, Cthulhu iOwner)
			{
				if (NetworkManager.Instance.State == NetworkState.Client)
				{
					return;
				}
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
					if (iOwner.mAnimationController.HasFinished)
					{
						iOwner.ChangeState(Cthulhu.States.Idle);
						return;
					}
					if (num >= 0.4864865f)
					{
						this.mTimer -= iDeltaTime;
						while (this.mTimers.Count > 0 && this.mTimers.First<KeyValuePair<float, int>>().Key + this.mTimer < 0f)
						{
							int value = this.mTimers.First<KeyValuePair<float, int>>().Value;
							this.mTimers.RemoveAt(0);
							int num2 = this.mRandomClosedList[value];
							Matrix matrix = iOwner.mDeepOnesSpawnTransforms[num2];
							Vector3 iPosition = matrix.Translation;
							Vector3 vector;
							iOwner.mPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref iPosition, out vector, MovementProperties.Default);
							iPosition = vector;
							CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(Cthulhu.LesserCallState.DEEP_ONE_HASH);
							NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(iOwner.mPlayState);
							instance.Initialize(cachedTemplate, iPosition, 0);
							Agent ai = instance.AI;
							ai.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, -1, 0, null);
							Matrix orientation = matrix;
							orientation.Translation = Vector3.Zero;
							instance.CharacterBody.Orientation = orientation;
							instance.CharacterBody.DesiredDirection = iOwner.mTransform.Forward;
							instance.SpawnAnimation = Magicka.Animations.special0;
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
								triggerActionMessage.Point2 = (int)instance.SpawnAnimation;
								NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
							}
						}
					}
				}
			}

			// Token: 0x06000840 RID: 2112 RVA: 0x0003597C File Offset: 0x00033B7C
			public void OnExit(Cthulhu iOwner)
			{
				this.mCooldown = 25f;
			}

			// Token: 0x06000841 RID: 2113 RVA: 0x0003598C File Offset: 0x00033B8C
			public bool Active(Cthulhu iOwner, float iDeltaTime)
			{
				if (this.mCooldown >= 0f)
				{
					return false;
				}
				TriggerArea triggerArea = iOwner.mPlayState.Level.CurrentScene.GetTriggerArea(Cthulhu.LesserCallState.ANY);
				int count = triggerArea.GetCount(Cthulhu.LesserCallState.DEEP_ONE_HASH);
				return count + triggerArea.GetCount(Cthulhu.LesserCallState.STARSPAWN_HASH) == 0 && Cthulhu.RANDOM.NextDouble() < 0.800000011920929;
			}

			// Token: 0x06000842 RID: 2114 RVA: 0x000359F7 File Offset: 0x00033BF7
			public void NonActiveUpdate(Cthulhu iOwner, float iDeltaTime)
			{
				if (this.mCooldown >= 0f)
				{
					this.mCooldown -= iDeltaTime;
				}
			}

			// Token: 0x0400075F RID: 1887
			private const float LOGIC_EXECUTE_TIME = 0.4864865f;

			// Token: 0x04000760 RID: 1888
			private const float PFX_EXECUTE_TIME = 0f;

			// Token: 0x04000761 RID: 1889
			private static readonly int ANY = "any".GetHashCodeCustom();

			// Token: 0x04000762 RID: 1890
			private static readonly int DEEP_ONE_HASH = "deep_one".GetHashCodeCustom();

			// Token: 0x04000763 RID: 1891
			private static readonly int STARSPAWN_HASH = "starspawn".GetHashCodeCustom();

			// Token: 0x04000764 RID: 1892
			private float mTimer;

			// Token: 0x04000765 RID: 1893
			private SortedList<float, int> mTimers;

			// Token: 0x04000766 RID: 1894
			private int[] mRandomClosedList;

			// Token: 0x04000767 RID: 1895
			private int mNrSpawns;

			// Token: 0x04000768 RID: 1896
			private float mCooldown;
		}

		// Token: 0x02000112 RID: 274
		private class DevourState : Cthulhu.CthulhuState, IBossState<Cthulhu>
		{
			// Token: 0x06000844 RID: 2116 RVA: 0x00035A43 File Offset: 0x00033C43
			public void OnEnter(Cthulhu iOwner)
			{
				iOwner.CrossFade(Cthulhu.Animations.Devour, 0.4f, false);
				this.mVortexTimer = 0f;
				this.mSoundPlayed = false;
				this.mEffectStarted = false;
			}

			// Token: 0x06000845 RID: 2117 RVA: 0x00035A6C File Offset: 0x00033C6C
			public void OnUpdate(float iDeltaTime, Cthulhu iOwner)
			{
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
					if (iOwner.mAnimationController.HasFinished)
					{
						iOwner.ChangeState(Cthulhu.States.Idle);
					}
					else if (num >= 0.08f && num <= 0.9f)
					{
						if (!this.mSoundPlayed)
						{
							AudioManager.Instance.PlayCue(Banks.Additional, Cthulhu.SOUND_DEVOUR, iOwner.mDamageZone.AudioEmitter);
							this.mSoundCueSuck = AudioManager.Instance.PlayCue(Banks.Additional, Cthulhu.SOUND_DEVOUR_SUCK, iOwner.mDamageZone.AudioEmitter);
							this.mSoundPlayed = true;
						}
						bool flag = this.CustomVortex(iOwner, iDeltaTime);
						if (flag)
						{
							return;
						}
					}
					if (num >= 0.08f && !this.mEffectStarted)
					{
						if (!EffectManager.Instance.IsActive(ref this.mEffectRef))
						{
							Matrix mMouthAttachOrientation = iOwner.mMouthAttachOrientation;
							EffectManager.Instance.StartEffect(this.ParticleEffect, ref mMouthAttachOrientation, out this.mEffectRef);
						}
						this.mEffectStarted = true;
					}
					if (num > 0.9f)
					{
						if (this.mEffectStarted && EffectManager.Instance.IsActive(ref this.mEffectRef))
						{
							EffectManager.Instance.Stop(ref this.mEffectRef);
							return;
						}
					}
					else if (this.mEffectStarted && EffectManager.Instance.IsActive(ref this.mEffectRef))
					{
						Matrix mMouthAttachOrientation2 = iOwner.mMouthAttachOrientation;
						EffectManager.Instance.UpdateOrientation(ref this.mEffectRef, ref mMouthAttachOrientation2);
					}
				}
			}

			// Token: 0x06000846 RID: 2118 RVA: 0x00035BE0 File Offset: 0x00033DE0
			private bool CustomVortex(Cthulhu iOwner, float iDeltaTime)
			{
				bool result = false;
				Vector3 vector = iOwner.mMouthAttachOrientation.Translation;
				Vector3 forward = iOwner.mTransform.Forward;
				Vector3.Multiply(ref forward, -0.8f, out forward);
				Vector3.Add(ref vector, ref forward, out vector);
				EntityManager entityManager = iOwner.mPlayState.EntityManager;
				List<Entity> entities = entityManager.GetEntities(vector, 30f, true);
				entities.Remove(iOwner.mDamageZone);
				int i = 0;
				while (i < entities.Count)
				{
					Character character = entities[i] as Character;
					if (character != null)
					{
						if (!character.IsEthereal)
						{
							if (character.IsGripped && character.Gripper != null)
							{
								character.Gripper.ReleaseAttachedCharacter();
							}
							if (character.Type != Cthulhu.DevourState.STARSPAWN_HASH)
							{
								goto IL_14F;
							}
						}
					}
					else if (entities[i] is Shield)
					{
						Vector3 nearestPosition = (entities[i] as Shield).GetNearestPosition(vector);
						Vector3 vector2;
						Vector3.Subtract(ref vector, ref nearestPosition, out vector2);
						vector2.Y = 0f;
						if (vector2.LengthSquared() > 1E-06f)
						{
							float num = vector2.Length();
							if (num < 1.8f)
							{
								if (this.blur != null)
								{
									this.blur.Kill();
								}
								iOwner.ChangeState(Cthulhu.States.Idle);
								result = true;
								break;
							}
						}
					}
					else if (entities[i] is MissileEntity)
					{
						goto IL_14F;
					}
					IL_2C2:
					i++;
					continue;
					IL_14F:
					Vector3 position = entities[i].Position;
					Vector3 vector3;
					Vector3.Subtract(ref vector, ref position, out vector3);
					vector3.Y = 0f;
					float num2 = vector3.LengthSquared();
					if (num2 <= 1E-06f)
					{
						goto IL_2C2;
					}
					float num3 = vector3.Length();
					bool flag;
					if (entities[i] is MissileEntity && num3 < 1.25f)
					{
						flag = true;
						entities[i].Kill();
					}
					else
					{
						flag = (num3 < 0.4f);
					}
					if (flag)
					{
						this.SetCharacterToEat(iOwner, character);
						if (this.blur != null)
						{
							this.blur.Kill();
						}
						iOwner.ChangeState(Cthulhu.States.DevourHit);
						result = true;
						break;
					}
					float num4 = 7.5f * (1f - num3 / 30f);
					if (num4 <= 0f)
					{
						goto IL_2C2;
					}
					vector3.Normalize();
					float num5 = Vector3.Dot(vector3, iOwner.mMouthAttachOrientation.Forward);
					if ((double)num5 < 0.7)
					{
						goto IL_2C2;
					}
					Vector3.Multiply(ref vector3, num4, out vector3);
					if (entities[i] is MissileEntity)
					{
						vector3 *= 100f;
						entities[i].Body.Velocity += vector3 * entities[i].Body.InverseMass * iDeltaTime;
						goto IL_2C2;
					}
					if (character != null)
					{
						character.CharacterBody.AdditionalForce = vector3;
						goto IL_2C2;
					}
					goto IL_2C2;
				}
				entityManager.ReturnEntityList(entities);
				vector -= 1.5f * iOwner.mTransform.Forward;
				this.mVortexTimer -= iDeltaTime;
				if (this.mVortexTimer <= 0f)
				{
					RadialBlur.GetRadialBlur();
					vector.Y = 0f;
					Vector3 forward2 = iOwner.mTransform.Forward;
					float iTTL = iOwner.mAnimationController.AnimationClip.Duration * 0.82f;
					this.blur = RadialBlur.GetRadialBlur();
					this.blur.Initialize(ref vector, ref forward2, 0.5235988f, 15f, iTTL, iOwner.mPlayState.Scene);
					this.mVortexTimer = iTTL;
				}
				return result;
			}

			// Token: 0x06000847 RID: 2119 RVA: 0x00035F74 File Offset: 0x00034174
			private unsafe void SetCharacterToEat(Cthulhu iOwner, Character iCharacter)
			{
				if (iCharacter == null)
				{
					return;
				}
				if (NetworkManager.Instance.State != NetworkState.Client)
				{
					iOwner.mCharacterToEat = iCharacter;
					if (NetworkManager.Instance.State == NetworkState.Server)
					{
						Cthulhu.SetCharacterToEatMessage setCharacterToEatMessage = default(Cthulhu.SetCharacterToEatMessage);
						setCharacterToEatMessage.Handle = iCharacter.Handle;
						BossFight.Instance.SendMessage<Cthulhu.SetCharacterToEatMessage>(iOwner, 8, (void*)(&setCharacterToEatMessage), true);
					}
				}
			}

			// Token: 0x06000848 RID: 2120 RVA: 0x00035FCC File Offset: 0x000341CC
			private bool FindTarget(Cthulhu iOwner, float radius, float tooCloseDistance, Vector3 pos, out Character target)
			{
				EntityManager entityManager = iOwner.mPlayState.EntityManager;
				List<Entity> entities = entityManager.GetEntities(pos, radius, true);
				bool flag = false;
				target = null;
				int num = 0;
				while (num < entities.Count && !flag)
				{
					Avatar avatar = entities[num] as Avatar;
					if (avatar != null && !avatar.IsEthereal)
					{
						target = avatar;
						Vector3 vector = avatar.Position - pos;
						vector.Y = 0f;
						float num2 = vector.LengthSquared();
						if (num2 > tooCloseDistance * tooCloseDistance)
						{
							vector.Normalize();
							Vector3 forward = iOwner.mTransform.Forward;
							float num3 = Vector3.Dot(vector, forward);
							flag = (num3 > 0.7f);
						}
					}
					num++;
				}
				entityManager.ReturnEntityList(entities);
				return flag;
			}

			// Token: 0x06000849 RID: 2121 RVA: 0x0003608A File Offset: 0x0003428A
			public void OnExit(Cthulhu iOwner)
			{
				this.StopAllEffectAndSounds();
			}

			// Token: 0x0600084A RID: 2122 RVA: 0x00036094 File Offset: 0x00034294
			private void StopAllEffectAndSounds()
			{
				if (EffectManager.Instance.IsActive(ref this.mEffectRef))
				{
					EffectManager.Instance.Stop(ref this.mEffectRef);
				}
				if (this.blur != null)
				{
					this.blur.Kill();
				}
				if (this.mSoundCueSuck != null)
				{
					this.mSoundCueSuck.Stop(AudioStopOptions.AsAuthored);
				}
			}

			// Token: 0x0600084B RID: 2123 RVA: 0x000360EC File Offset: 0x000342EC
			public bool Active(Cthulhu iOwner, float iDeltaTime)
			{
				Character character;
				return this.FindTarget(iOwner, 23f, 2f, iOwner.mTransform.Translation, out character);
			}

			// Token: 0x0600084C RID: 2124 RVA: 0x00036117 File Offset: 0x00034317
			public void NonActiveUpdate(Cthulhu iOwner, float iDeltaTime)
			{
			}

			// Token: 0x04000769 RID: 1897
			private const float START_TIME = 0.08f;

			// Token: 0x0400076A RID: 1898
			private const float END_TIME = 0.9f;

			// Token: 0x0400076B RID: 1899
			private const float PFX_START_TIME = 0.08f;

			// Token: 0x0400076C RID: 1900
			private VisualEffectReference mEffectRef;

			// Token: 0x0400076D RID: 1901
			private int ParticleEffect = "cthulhu_pull".GetHashCodeCustom();

			// Token: 0x0400076E RID: 1902
			private static readonly int STARSPAWN_HASH = "starspawn".GetHashCodeCustom();

			// Token: 0x0400076F RID: 1903
			private float mVortexTimer;

			// Token: 0x04000770 RID: 1904
			private bool mSoundPlayed;

			// Token: 0x04000771 RID: 1905
			private bool mEffectStarted;

			// Token: 0x04000772 RID: 1906
			private RadialBlur blur;

			// Token: 0x04000773 RID: 1907
			private Cue mSoundCueSuck;
		}

		// Token: 0x02000113 RID: 275
		private class DevourHitState : Cthulhu.CthulhuState, IBossState<Cthulhu>
		{
			// Token: 0x0600084F RID: 2127 RVA: 0x00036144 File Offset: 0x00034344
			public void OnEnter(Cthulhu iOwner)
			{
				iOwner.CrossFade(Cthulhu.Animations.DevourHit, 0.2f, false);
				this.mSoundPlayed = false;
				this.mEffectStarted = false;
				this.mCharacterTerminated = (iOwner.mCharacterToEat == null);
				this.mCharacterScale = 1f;
				if (iOwner.mCharacterToEat != null)
				{
					Vector3 up = iOwner.mCharacterToEat.Capsule.Orientation.Up;
					Vector3.Multiply(ref up, iOwner.mCharacterToEat.Capsule.Length, out this.mCharacterOffsetUp);
					Vector3 position = iOwner.mCharacterToEat.Position;
					Vector3 translation = iOwner.mMouthAttachOrientation.Translation;
					Vector3.Subtract(ref position, ref translation, out this.mCharacterOffset);
				}
			}

			// Token: 0x06000850 RID: 2128 RVA: 0x000361EC File Offset: 0x000343EC
			public void OnUpdate(float iDeltaTime, Cthulhu iOwner)
			{
				if (!this.mCharacterTerminated)
				{
					this.mCharacterScale -= iDeltaTime;
					Character mCharacterToEat = iOwner.mCharacterToEat;
					float scaleFactor = (this.mCharacterScale - 0.6666667f) * 3f;
					Vector3 vector;
					Vector3.Multiply(ref this.mCharacterOffset, scaleFactor, out vector);
					Vector3 vector2;
					Vector3.Multiply(ref this.mCharacterOffsetUp, (1f - this.mCharacterScale) * 2f, out vector2);
					Vector3 translation = iOwner.mMouthAttachOrientation.Translation;
					Vector3.Add(ref translation, ref vector, out translation);
					Vector3.Add(ref translation, ref vector2, out translation);
					Matrix orientation = mCharacterToEat.Body.Orientation;
					mCharacterToEat.Body.MoveTo(ref translation, ref orientation);
					mCharacterToEat.ScaleGraphicModel(this.mCharacterScale);
					if (this.mCharacterScale <= 0.1f)
					{
						this.mCharacterTerminated = true;
						mCharacterToEat.Terminate(true, false);
						mCharacterToEat.ScaleGraphicModel(1f);
					}
				}
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
					if (iOwner.mAnimationController.HasFinished)
					{
						iOwner.ChangeState(Cthulhu.States.Idle);
					}
					else if (num >= 0.1f && !this.mEffectStarted)
					{
						this.mEffectStarted = true;
						Matrix mMouthAttachOrientation = iOwner.mMouthAttachOrientation;
						EffectManager.Instance.StartEffect(this.ParticleEffect, ref mMouthAttachOrientation, out this.mEffectRef);
					}
					else if (num >= 0.08f && !this.mSoundPlayed)
					{
						AudioManager.Instance.PlayCue(Banks.Additional, Cthulhu.SOUND_DEVOUR_HIT, iOwner.mDamageZone.AudioEmitter);
						this.mSoundPlayed = true;
					}
					if (this.mEffectStarted && EffectManager.Instance.IsActive(ref this.mEffectRef))
					{
						Matrix mMouthAttachOrientation2 = iOwner.mMouthAttachOrientation;
						EffectManager.Instance.UpdateOrientation(ref this.mEffectRef, ref mMouthAttachOrientation2);
					}
				}
			}

			// Token: 0x06000851 RID: 2129 RVA: 0x000363B7 File Offset: 0x000345B7
			public void OnExit(Cthulhu iOwner)
			{
				if (EffectManager.Instance.IsActive(ref this.mEffectRef))
				{
					EffectManager.Instance.Stop(ref this.mEffectRef);
				}
			}

			// Token: 0x06000852 RID: 2130 RVA: 0x000363DB File Offset: 0x000345DB
			public bool Active(Cthulhu iOwner, float iDeltaTime)
			{
				return false;
			}

			// Token: 0x06000853 RID: 2131 RVA: 0x000363DE File Offset: 0x000345DE
			public void NonActiveUpdate(Cthulhu iOwner, float iDeltaTime)
			{
			}

			// Token: 0x04000774 RID: 1908
			private const float PFX_START_TIME = 0.1f;

			// Token: 0x04000775 RID: 1909
			private const float SOUND_START_TIME = 0.08f;

			// Token: 0x04000776 RID: 1910
			private VisualEffectReference mEffectRef;

			// Token: 0x04000777 RID: 1911
			private int ParticleEffect = "cthulhu_is_eating".GetHashCodeCustom();

			// Token: 0x04000778 RID: 1912
			private bool mSoundPlayed;

			// Token: 0x04000779 RID: 1913
			private bool mEffectStarted;

			// Token: 0x0400077A RID: 1914
			private bool mCharacterTerminated;

			// Token: 0x0400077B RID: 1915
			private float mCharacterScale;

			// Token: 0x0400077C RID: 1916
			private Vector3 mCharacterOffset;

			// Token: 0x0400077D RID: 1917
			private Vector3 mCharacterOffsetUp;
		}

		// Token: 0x02000114 RID: 276
		private class HypnotizeState : Cthulhu.CthulhuState, IBossState<Cthulhu>
		{
			// Token: 0x06000855 RID: 2133 RVA: 0x000363F8 File Offset: 0x000345F8
			public void OnEnter(Cthulhu iOwner)
			{
				iOwner.CrossFade(Cthulhu.Animations.Mesmerize, 0.4f, false);
				this.mCasted = false;
				this.mEffectDone = false;
				AudioManager.Instance.PlayCue(Banks.Additional, Cthulhu.SOUND_HYPNOTIZE, iOwner.mDamageZone.AudioEmitter);
			}

			// Token: 0x06000856 RID: 2134 RVA: 0x00036438 File Offset: 0x00034638
			public unsafe void OnUpdate(float iDeltaTime, Cthulhu iOwner)
			{
				if (NetworkManager.Instance.State == NetworkState.Client)
				{
					return;
				}
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					if (iOwner.mAnimationController.HasFinished)
					{
						iOwner.ChangeState(Cthulhu.States.Idle);
						return;
					}
					float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
					if (num >= this.LOGIC_EXECUTE_TIME && !this.mCasted)
					{
						Vector3 translation = iOwner.mTransform.Translation;
						Avatar avatar;
						if (iOwner.GetRandomTarget(out avatar, ref translation, false))
						{
							Vector3 position = avatar.Position;
							Vector3 translation2 = iOwner.mMistSpawnTransform.Translation;
							Vector3 vector;
							Vector3.Subtract(ref position, ref translation2, out vector);
							vector.Y = 0f;
							if (vector.LengthSquared() < 1E-06f)
							{
								translation2 = iOwner.mTransform.Translation;
								Vector3.Subtract(ref translation2, ref position, out vector);
								vector.Y = 0f;
							}
							if (vector.LengthSquared() < 1E-06f)
							{
								return;
							}
							vector.Normalize();
							avatar.Hypnotize(ref vector, iOwner.mPlayerHypnotizeEffect);
							if (NetworkManager.Instance.State == NetworkState.Server)
							{
								Cthulhu.HypnotizeMessage hypnotizeMessage = default(Cthulhu.HypnotizeMessage);
								hypnotizeMessage.Direction = new Normalized101010(vector);
								hypnotizeMessage.Handle = avatar.Handle;
								BossFight.Instance.SendMessage<Cthulhu.HypnotizeMessage>(iOwner, 5, (void*)(&hypnotizeMessage), true);
							}
						}
						this.mCasted = true;
					}
					else if (!this.mEffectDone && num >= this.PFX_EXECUTE_TIME)
					{
						if (!EffectManager.Instance.IsActive(ref this.mEffectRef))
						{
							Matrix mMouthAttachOrientation = iOwner.mMouthAttachOrientation;
							EffectManager.Instance.StartEffect(this.ParticleEffect, ref mMouthAttachOrientation, out this.mEffectRef);
						}
						this.mEffectDone = true;
					}
					if (this.mEffectDone && EffectManager.Instance.IsActive(ref this.mEffectRef))
					{
						Matrix mMouthAttachOrientation2 = iOwner.mMouthAttachOrientation;
						EffectManager.Instance.UpdateOrientation(ref this.mEffectRef, ref mMouthAttachOrientation2);
					}
				}
			}

			// Token: 0x06000857 RID: 2135 RVA: 0x00036612 File Offset: 0x00034812
			public void OnExit(Cthulhu iOwner)
			{
				if (EffectManager.Instance.IsActive(ref this.mEffectRef))
				{
					EffectManager.Instance.Stop(ref this.mEffectRef);
				}
				this.mCoolDown = 5f;
			}

			// Token: 0x06000858 RID: 2136 RVA: 0x00036644 File Offset: 0x00034844
			public bool Active(Cthulhu iOwner, float iDeltaTime)
			{
				if (Game.Instance.PlayerCount <= 1 || this.mCoolDown >= 0f)
				{
					return false;
				}
				int num = 0;
				for (int i = 0; i < iOwner.mPlayers.Length; i++)
				{
					if (iOwner.mPlayers[i].Playing && iOwner.mPlayers[i].Avatar != null)
					{
						if (iOwner.mPlayers[i].Avatar.IsHypnotized)
						{
							return false;
						}
						if (!iOwner.mPlayers[i].Avatar.Dead)
						{
							num++;
						}
					}
				}
				return num >= 2;
			}

			// Token: 0x06000859 RID: 2137 RVA: 0x000366D5 File Offset: 0x000348D5
			public void NonActiveUpdate(Cthulhu iOwner, float iDeltaTime)
			{
				if (this.mCoolDown >= 0f)
				{
					this.mCoolDown -= iDeltaTime;
				}
			}

			// Token: 0x0400077E RID: 1918
			private float LOGIC_EXECUTE_TIME = 0.6041667f;

			// Token: 0x0400077F RID: 1919
			private float PFX_EXECUTE_TIME;

			// Token: 0x04000780 RID: 1920
			private bool mCasted;

			// Token: 0x04000781 RID: 1921
			private bool mEffectDone;

			// Token: 0x04000782 RID: 1922
			private float mCoolDown;

			// Token: 0x04000783 RID: 1923
			private VisualEffectReference mEffectRef;

			// Token: 0x04000784 RID: 1924
			private int ParticleEffect = "cthulhu_hypnotize".GetHashCodeCustom();
		}

		// Token: 0x02000115 RID: 277
		private class RageState : Cthulhu.CthulhuState, IBossState<Cthulhu>
		{
			// Token: 0x0600085B RID: 2139 RVA: 0x00036715 File Offset: 0x00034915
			public void OnEnter(Cthulhu iOwner)
			{
				iOwner.CrossFade(Cthulhu.Animations.Rage, 0.5f, false);
				this.mCalled = false;
				this.mRumble = false;
				AudioManager.Instance.PlayCue(Banks.Additional, Cthulhu.SOUND_RAGE, iOwner.mDamageZone.AudioEmitter);
			}

			// Token: 0x0600085C RID: 2140 RVA: 0x00036754 File Offset: 0x00034954
			public void OnUpdate(float iDeltaTime, Cthulhu iOwner)
			{
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					if (iOwner.mAnimationController.HasFinished)
					{
						iOwner.ChangeState(Cthulhu.States.Idle);
					}
					float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
					if (num >= this.CAST_TIME && !this.mCalled)
					{
						this.mCalled = true;
						if (this.Callback != null)
						{
							this.Callback(iOwner);
						}
					}
					if (num >= this.RUMBLE_TIME && !this.mRumble)
					{
						this.mRumble = true;
						iOwner.mPlayState.Camera.CameraShake(iOwner.mTransform.Translation, 0.5f, 2.5f);
					}
				}
			}

			// Token: 0x0600085D RID: 2141 RVA: 0x0003680B File Offset: 0x00034A0B
			public void OnExit(Cthulhu iOwner)
			{
			}

			// Token: 0x0600085E RID: 2142 RVA: 0x0003680D File Offset: 0x00034A0D
			public bool Active(Cthulhu iOwner, float iDeltaTime)
			{
				return false;
			}

			// Token: 0x0600085F RID: 2143 RVA: 0x00036810 File Offset: 0x00034A10
			public void NonActiveUpdate(Cthulhu iOwner, float iDeltaTime)
			{
			}

			// Token: 0x04000785 RID: 1925
			public Action<Cthulhu> Callback;

			// Token: 0x04000786 RID: 1926
			private float RUMBLE_TIME = 0.25f;

			// Token: 0x04000787 RID: 1927
			private float CAST_TIME = 0.5f;

			// Token: 0x04000788 RID: 1928
			private bool mCalled;

			// Token: 0x04000789 RID: 1929
			private bool mRumble;
		}

		// Token: 0x02000116 RID: 278
		private class OtherworldlyBoltState : Cthulhu.CthulhuState, IBossState<Cthulhu>
		{
			// Token: 0x06000861 RID: 2145 RVA: 0x00036830 File Offset: 0x00034A30
			public void OnEnter(Cthulhu iOwner)
			{
				this.mNrToCast = Game.Instance.PlayerCount;
				AudioManager.Instance.PlayCue(Banks.Additional, Cthulhu.SOUND_OTHERWORLDLY_BOLT, iOwner.mDamageZone.AudioEmitter);
				this.mNrCasted = 0;
				this.mCasted = false;
				iOwner.CrossFade(Cthulhu.Animations.OtherwordlyBoltStart, 0.2f, false);
				this.mState = Cthulhu.OtherworldlyBoltState.BoltState.Start;
			}

			// Token: 0x06000862 RID: 2146 RVA: 0x00036890 File Offset: 0x00034A90
			public void OnUpdate(float iDeltaTime, Cthulhu iOwner)
			{
				switch (this.mState)
				{
				case Cthulhu.OtherworldlyBoltState.BoltState.Start:
					if (iOwner.mAnimationController.CrossFadeEnabled)
					{
						return;
					}
					if (iOwner.mAnimationController.HasFinished)
					{
						this.mState = Cthulhu.OtherworldlyBoltState.BoltState.Mid;
						iOwner.CrossFade(Cthulhu.Animations.OtherwordlyBoltMid, 0f, false);
						return;
					}
					break;
				case Cthulhu.OtherworldlyBoltState.BoltState.Mid:
					if (iOwner.mAnimationController.CrossFadeEnabled)
					{
						return;
					}
					if (iOwner.mAnimationController.HasFinished)
					{
						if (this.mNrCasted >= this.mNrToCast)
						{
							this.mState = Cthulhu.OtherworldlyBoltState.BoltState.End;
							iOwner.CrossFade(Cthulhu.Animations.OtherwordlyBoltEnd, 0f, false);
							return;
						}
						this.mState = Cthulhu.OtherworldlyBoltState.BoltState.Mid;
						this.mCasted = false;
						iOwner.CrossFade(Cthulhu.Animations.OtherwordlyBoltMid, 0f, false);
						return;
					}
					else if (!this.mCasted)
					{
						float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
						if (num >= 0f && this.mCurrentBolt == null)
						{
							Vector3 translation = iOwner.mLeftHandOrientation.Translation;
							Vector3 translation2 = iOwner.mRightHandOrientation.Translation;
							Vector3 vector;
							Vector3.Subtract(ref translation2, ref translation, out vector);
							Vector3.Multiply(ref vector, 0.5f, out vector);
							Vector3 vector2;
							Vector3.Add(ref translation, ref vector, out vector2);
							this.mCurrentBolt = iOwner.SpawnCultistMissile(ref vector2, 4f * iOwner.mStageSpeedModifier);
							return;
						}
						if (num >= 0.27f)
						{
							this.mNrCasted++;
							this.mCurrentBolt.GoHunt();
							this.mCurrentBolt = null;
							this.mCasted = true;
							return;
						}
					}
					break;
				case Cthulhu.OtherworldlyBoltState.BoltState.End:
					if (iOwner.mAnimationController.CrossFadeEnabled)
					{
						return;
					}
					if (iOwner.mAnimationController.HasFinished)
					{
						iOwner.ChangeState(Cthulhu.States.Idle);
					}
					break;
				default:
					return;
				}
			}

			// Token: 0x06000863 RID: 2147 RVA: 0x00036A28 File Offset: 0x00034C28
			private void FindTarget(Cthulhu iOwner, Vector3 pos, out Avatar target)
			{
				EntityManager entityManager = iOwner.mPlayState.EntityManager;
				List<Entity> entities = entityManager.GetEntities(pos, 25f, true);
				bool flag = false;
				target = null;
				float num = float.MaxValue;
				int num2 = -1;
				int num3 = 0;
				while (num3 < entities.Count && !flag)
				{
					Avatar avatar = entities[num3] as Avatar;
					if (avatar != null && !avatar.IsEthereal)
					{
						Vector3 vector = avatar.Position - pos;
						vector.Y = 0f;
						float num4 = vector.LengthSquared();
						if (num4 < num)
						{
							num = num4;
							num2 = num3;
						}
					}
					num3++;
				}
				if (num2 != -1)
				{
					target = (entities[num2] as Avatar);
				}
				else
				{
					target = null;
				}
				entityManager.ReturnEntityList(entities);
			}

			// Token: 0x06000864 RID: 2148 RVA: 0x00036AE1 File Offset: 0x00034CE1
			public void OnExit(Cthulhu iOwner)
			{
				this.mCoolDown = 5f;
			}

			// Token: 0x06000865 RID: 2149 RVA: 0x00036AF0 File Offset: 0x00034CF0
			public bool Active(Cthulhu iOwner, float iDeltaTime)
			{
				if (this.mCoolDown > 0f)
				{
					return false;
				}
				TriggerArea triggerArea = iOwner.mPlayState.Level.CurrentScene.GetTriggerArea(Cthulhu.OtherworldlyBoltState.ANY);
				int num = triggerArea.GetCount(Cthulhu.OtherworldlyBoltState.CULTIST_HASH);
				num += triggerArea.GetCount(Cthulhu.OtherworldlyBoltState.DEEP_ONE_HASH);
				return num <= Game.Instance.PlayerCount;
			}

			// Token: 0x06000866 RID: 2150 RVA: 0x00036B51 File Offset: 0x00034D51
			public void NonActiveUpdate(Cthulhu iOwner, float iDeltaTime)
			{
				if (this.mCoolDown >= 0f)
				{
					this.mCoolDown -= iDeltaTime;
				}
			}

			// Token: 0x0400078A RID: 1930
			private int mNrCasted;

			// Token: 0x0400078B RID: 1931
			private int mNrToCast;

			// Token: 0x0400078C RID: 1932
			private Cthulhu.OtherworldlyBoltState.BoltState mState;

			// Token: 0x0400078D RID: 1933
			private float mCoolDown;

			// Token: 0x0400078E RID: 1934
			private bool mCasted;

			// Token: 0x0400078F RID: 1935
			private OtherworldlyBolt mCurrentBolt;

			// Token: 0x04000790 RID: 1936
			private static readonly int ANY = "any".GetHashCodeCustom();

			// Token: 0x04000791 RID: 1937
			private static readonly int CULTIST_HASH = "cultist".GetHashCodeCustom();

			// Token: 0x04000792 RID: 1938
			private static readonly int DEEP_ONE_HASH = "deep_one".GetHashCodeCustom();

			// Token: 0x02000117 RID: 279
			private enum BoltState
			{
				// Token: 0x04000794 RID: 1940
				Start,
				// Token: 0x04000795 RID: 1941
				Mid,
				// Token: 0x04000796 RID: 1942
				End
			}
		}

		// Token: 0x02000118 RID: 280
		private class DeathState : Cthulhu.CthulhuState, IBossState<Cthulhu>
		{
			// Token: 0x06000869 RID: 2153 RVA: 0x00036BA8 File Offset: 0x00034DA8
			public void OnEnter(Cthulhu iOwner)
			{
				for (int i = 0; i < iOwner.mActiveTentacles.Count; i++)
				{
					iOwner.mActiveTentacles[i].KillTentacle();
				}
				iOwner.KillBolts();
				if (!iOwner.IsAtPoint(Cthulhu.BossSpawnPoints.NORTH))
				{
					iOwner.mDesiredSpawnPoint = 0;
					iOwner.ChangeState(Cthulhu.States.Submerge);
					return;
				}
				iOwner.CrossFade(Cthulhu.Animations.Die, 0.5f, false);
				AudioManager.Instance.PlayCue(Banks.Additional, Cthulhu.SOUND_DEATH, iOwner.mDamageZone.AudioEmitter);
				this.mEffectStarted = false;
				GameScene currentScene = iOwner.mPlayState.Level.CurrentScene;
				currentScene.ExecuteTrigger("bossdead".GetHashCodeCustom(), null, false);
			}

			// Token: 0x0600086A RID: 2154 RVA: 0x00036C54 File Offset: 0x00034E54
			public void OnUpdate(float iDeltaTime, Cthulhu iOwner)
			{
				if (!iOwner.mAnimationController.CrossFadeEnabled)
				{
					float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
					if (iOwner.mAnimationController.HasFinished)
					{
						iOwner.KillIdleEffect();
						iOwner.mDead = true;
					}
					else if (!this.mEffectStarted && num >= 0f)
					{
						this.mEffectStarted = true;
						Matrix mTransform = iOwner.mTransform;
						EffectManager.Instance.StartEffect(this.ParticleEffect, ref mTransform, out this.mEffectRef);
					}
					if (this.mEffectStarted && EffectManager.Instance.IsActive(ref this.mEffectRef))
					{
						Matrix mTransform2 = iOwner.mTransform;
						EffectManager.Instance.UpdateOrientation(ref this.mEffectRef, ref mTransform2);
					}
				}
			}

			// Token: 0x0600086B RID: 2155 RVA: 0x00036D15 File Offset: 0x00034F15
			public void OnExit(Cthulhu iOwner)
			{
				iOwner.KillIdleEffect();
				iOwner.mDead = true;
			}

			// Token: 0x0600086C RID: 2156 RVA: 0x00036D24 File Offset: 0x00034F24
			public bool Active(Cthulhu iOwner, float iDeltaTime)
			{
				return false;
			}

			// Token: 0x0600086D RID: 2157 RVA: 0x00036D27 File Offset: 0x00034F27
			public void NonActiveUpdate(Cthulhu iOwner, float iDeltaTime)
			{
			}

			// Token: 0x04000797 RID: 1943
			private VisualEffectReference mEffectRef;

			// Token: 0x04000798 RID: 1944
			private int ParticleEffect = "cthulhu_death".GetHashCodeCustom();

			// Token: 0x04000799 RID: 1945
			private bool mEffectStarted;
		}

		// Token: 0x02000119 RID: 281
		public enum MessageType : ushort
		{
			// Token: 0x0400079B RID: 1947
			Update,
			// Token: 0x0400079C RID: 1948
			ChangeState,
			// Token: 0x0400079D RID: 1949
			ChangeStage,
			// Token: 0x0400079E RID: 1950
			ChangeTarget,
			// Token: 0x0400079F RID: 1951
			SpawnPoint,
			// Token: 0x040007A0 RID: 1952
			Hypnotize,
			// Token: 0x040007A1 RID: 1953
			Timewarp,
			// Token: 0x040007A2 RID: 1954
			ActivateMist,
			// Token: 0x040007A3 RID: 1955
			SetCharacterToEat,
			// Token: 0x040007A4 RID: 1956
			CharmAndConfuse,
			// Token: 0x040007A5 RID: 1957
			ActivateDeflection,
			// Token: 0x040007A6 RID: 1958
			TentacleUpdate,
			// Token: 0x040007A7 RID: 1959
			TentacleChangeState,
			// Token: 0x040007A8 RID: 1960
			TentacleSpawn,
			// Token: 0x040007A9 RID: 1961
			TentacleGrab,
			// Token: 0x040007AA RID: 1962
			TentacleAimTarget,
			// Token: 0x040007AB RID: 1963
			TentacleReleaseAimTarget
		}

		// Token: 0x0200011A RID: 282
		internal struct ActivateDeflectionMessage
		{
			// Token: 0x040007AC RID: 1964
			public const ushort TYPE = 10;
		}

		// Token: 0x0200011B RID: 283
		internal struct CharmAndConfuseMessage
		{
			// Token: 0x040007AD RID: 1965
			public const ushort TYPE = 9;

			// Token: 0x040007AE RID: 1966
			public ushort Handle;
		}

		// Token: 0x0200011C RID: 284
		internal struct SetCharacterToEatMessage
		{
			// Token: 0x040007AF RID: 1967
			public const ushort TYPE = 8;

			// Token: 0x040007B0 RID: 1968
			public ushort Handle;
		}

		// Token: 0x0200011D RID: 285
		internal struct ActivateMistMessage
		{
			// Token: 0x040007B1 RID: 1969
			public const ushort TYPE = 7;
		}

		// Token: 0x0200011E RID: 286
		internal struct TimewarpMessage
		{
			// Token: 0x040007B2 RID: 1970
			public const ushort TYPE = 6;
		}

		// Token: 0x0200011F RID: 287
		internal struct HypnotizeMessage
		{
			// Token: 0x040007B3 RID: 1971
			public const ushort TYPE = 5;

			// Token: 0x040007B4 RID: 1972
			public ushort Handle;

			// Token: 0x040007B5 RID: 1973
			public Normalized101010 Direction;
		}

		// Token: 0x02000120 RID: 288
		internal struct TentacleGrabMessage
		{
			// Token: 0x040007B6 RID: 1974
			public const ushort TYPE = 14;

			// Token: 0x040007B7 RID: 1975
			public ushort Handle;

			// Token: 0x040007B8 RID: 1976
			public byte TentacleIndex;
		}

		// Token: 0x02000121 RID: 289
		internal struct TentacleAimTargetMessage
		{
			// Token: 0x040007B9 RID: 1977
			public const ushort TYPE = 15;

			// Token: 0x040007BA RID: 1978
			public ushort Handle;

			// Token: 0x040007BB RID: 1979
			public byte TentacleIndex;
		}

		// Token: 0x02000122 RID: 290
		internal struct TentacleReleaseAimTargetMessage
		{
			// Token: 0x040007BC RID: 1980
			public const ushort TYPE = 16;

			// Token: 0x040007BD RID: 1981
			public byte TentacleIndex;
		}

		// Token: 0x02000123 RID: 291
		internal struct TentacleUpdateMessage
		{
			// Token: 0x040007BE RID: 1982
			public const ushort TYPE = 11;

			// Token: 0x040007BF RID: 1983
			public byte Animation;

			// Token: 0x040007C0 RID: 1984
			public HalfSingle AnimationTime;

			// Token: 0x040007C1 RID: 1985
			public byte TentacleIndex;
		}

		// Token: 0x02000124 RID: 292
		internal struct TentacleChangeStateMessage
		{
			// Token: 0x040007C2 RID: 1986
			public const ushort TYPE = 12;

			// Token: 0x040007C3 RID: 1987
			public Tentacle.States NewState;

			// Token: 0x040007C4 RID: 1988
			public byte TentacleIndex;
		}

		// Token: 0x02000125 RID: 293
		internal struct TentacleSpawnMessage
		{
			// Token: 0x040007C5 RID: 1989
			public const ushort TYPE = 13;

			// Token: 0x040007C6 RID: 1990
			public byte TentacleIndex;
		}

		// Token: 0x02000126 RID: 294
		internal struct UpdateMessage
		{
			// Token: 0x040007C7 RID: 1991
			public const ushort TYPE = 0;

			// Token: 0x040007C8 RID: 1992
			public byte Animation;

			// Token: 0x040007C9 RID: 1993
			public HalfSingle AnimationTime;

			// Token: 0x040007CA RID: 1994
			public float Hitpoints;
		}

		// Token: 0x02000127 RID: 295
		internal struct ChangeStateMessage
		{
			// Token: 0x040007CB RID: 1995
			public const ushort TYPE = 1;

			// Token: 0x040007CC RID: 1996
			public Cthulhu.States NewState;
		}

		// Token: 0x02000128 RID: 296
		internal struct ChangeStageMessage
		{
			// Token: 0x040007CD RID: 1997
			public const ushort TYPE = 2;

			// Token: 0x040007CE RID: 1998
			public Cthulhu.Stages NewStage;
		}

		// Token: 0x02000129 RID: 297
		internal struct ChangeTargetMessage
		{
			// Token: 0x040007CF RID: 1999
			public const ushort TYPE = 3;

			// Token: 0x040007D0 RID: 2000
			public int Handle;
		}

		// Token: 0x0200012A RID: 298
		internal struct SpawnPointMessage
		{
			// Token: 0x040007D1 RID: 2001
			public const ushort TYPE = 4;

			// Token: 0x040007D2 RID: 2002
			public sbyte Index;

			// Token: 0x040007D3 RID: 2003
			public int SpawnPoint;
		}

		// Token: 0x0200012B RID: 299
		private enum BossSpawnPoints : byte
		{
			// Token: 0x040007D5 RID: 2005
			NORTH,
			// Token: 0x040007D6 RID: 2006
			NORTHWEST,
			// Token: 0x040007D7 RID: 2007
			WEST,
			// Token: 0x040007D8 RID: 2008
			SOUTHWEST,
			// Token: 0x040007D9 RID: 2009
			SOUTH,
			// Token: 0x040007DA RID: 2010
			SOUTHEAST,
			// Token: 0x040007DB RID: 2011
			EAST,
			// Token: 0x040007DC RID: 2012
			NORTHEAST
		}

		// Token: 0x0200012C RID: 300
		private enum Animations
		{
			// Token: 0x040007DE RID: 2014
			OtherwordlyBoltStart,
			// Token: 0x040007DF RID: 2015
			OtherwordlyBoltMid,
			// Token: 0x040007E0 RID: 2016
			OtherwordlyBoltEnd,
			// Token: 0x040007E1 RID: 2017
			Submerge,
			// Token: 0x040007E2 RID: 2018
			Mists,
			// Token: 0x040007E3 RID: 2019
			Timewarp,
			// Token: 0x040007E4 RID: 2020
			Mesmerize,
			// Token: 0x040007E5 RID: 2021
			Madness,
			// Token: 0x040007E6 RID: 2022
			Emerge,
			// Token: 0x040007E7 RID: 2023
			Idle,
			// Token: 0x040007E8 RID: 2024
			Devour,
			// Token: 0x040007E9 RID: 2025
			DevourHit,
			// Token: 0x040007EA RID: 2026
			Cast_Lightning,
			// Token: 0x040007EB RID: 2027
			Call_of_Cthulhu,
			// Token: 0x040007EC RID: 2028
			Die,
			// Token: 0x040007ED RID: 2029
			Rage,
			// Token: 0x040007EE RID: 2030
			NR_OF_ANIMATIONS
		}

		// Token: 0x0200012D RID: 301
		private enum Fingers
		{
			// Token: 0x040007F0 RID: 2032
			Thumb,
			// Token: 0x040007F1 RID: 2033
			Finger1,
			// Token: 0x040007F2 RID: 2034
			Finger2,
			// Token: 0x040007F3 RID: 2035
			Finger3,
			// Token: 0x040007F4 RID: 2036
			Finger4,
			// Token: 0x040007F5 RID: 2037
			Finger5
		}

		// Token: 0x0200012E RID: 302
		private class RenderData : IRenderableObject
		{
			// Token: 0x0600086F RID: 2159 RVA: 0x00036D44 File Offset: 0x00034F44
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
				this.BoundingSphere = iMesh.BoundingSphere;
				SkinnedModelDeferredNormalMappedEffect iEffect = iPart.Effect as SkinnedModelDeferredNormalMappedEffect;
				this.Material.FetchFromEffect(iEffect);
			}

			// Token: 0x170001A6 RID: 422
			// (get) Token: 0x06000870 RID: 2160 RVA: 0x00036DE7 File Offset: 0x00034FE7
			public int Effect
			{
				get
				{
					return SkinnedModelDeferredNormalMappedEffect.TYPEHASH;
				}
			}

			// Token: 0x170001A7 RID: 423
			// (get) Token: 0x06000871 RID: 2161 RVA: 0x00036DEE File Offset: 0x00034FEE
			public int DepthTechnique
			{
				get
				{
					return 1;
				}
			}

			// Token: 0x170001A8 RID: 424
			// (get) Token: 0x06000872 RID: 2162 RVA: 0x00036DF1 File Offset: 0x00034FF1
			public int Technique
			{
				get
				{
					return 0;
				}
			}

			// Token: 0x170001A9 RID: 425
			// (get) Token: 0x06000873 RID: 2163 RVA: 0x00036DF4 File Offset: 0x00034FF4
			public int ShadowTechnique
			{
				get
				{
					return 2;
				}
			}

			// Token: 0x170001AA RID: 426
			// (get) Token: 0x06000874 RID: 2164 RVA: 0x00036DF7 File Offset: 0x00034FF7
			public VertexBuffer Vertices
			{
				get
				{
					return this.mVertices;
				}
			}

			// Token: 0x170001AB RID: 427
			// (get) Token: 0x06000875 RID: 2165 RVA: 0x00036DFF File Offset: 0x00034FFF
			public int VerticesHashCode
			{
				get
				{
					return this.mVerticesHash;
				}
			}

			// Token: 0x170001AC RID: 428
			// (get) Token: 0x06000876 RID: 2166 RVA: 0x00036E07 File Offset: 0x00035007
			public int VertexStride
			{
				get
				{
					return this.mVertexStride;
				}
			}

			// Token: 0x170001AD RID: 429
			// (get) Token: 0x06000877 RID: 2167 RVA: 0x00036E0F File Offset: 0x0003500F
			public IndexBuffer Indices
			{
				get
				{
					return this.mIndices;
				}
			}

			// Token: 0x170001AE RID: 430
			// (get) Token: 0x06000878 RID: 2168 RVA: 0x00036E17 File Offset: 0x00035017
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return this.mDeclaration;
				}
			}

			// Token: 0x06000879 RID: 2169 RVA: 0x00036E1F File Offset: 0x0003501F
			public bool Cull(BoundingFrustum iViewFrustum)
			{
				return !iViewFrustum.Intersects(this.BoundingSphere);
			}

			// Token: 0x0600087A RID: 2170 RVA: 0x00036E30 File Offset: 0x00035030
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

			// Token: 0x0600087B RID: 2171 RVA: 0x00036EE8 File Offset: 0x000350E8
			public void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelDeferredNormalMappedEffect skinnedModelDeferredNormalMappedEffect = iEffect as SkinnedModelDeferredNormalMappedEffect;
				this.Material.AssignOpacityToEffect(skinnedModelDeferredNormalMappedEffect);
				skinnedModelDeferredNormalMappedEffect.Bones = this.Skeleton;
				skinnedModelDeferredNormalMappedEffect.CommitChanges();
				skinnedModelDeferredNormalMappedEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimCount);
			}

			// Token: 0x040007F6 RID: 2038
			public Matrix[] Skeleton;

			// Token: 0x040007F7 RID: 2039
			public SkinnedModelDeferredNormalMappedMaterial Material;

			// Token: 0x040007F8 RID: 2040
			private VertexBuffer mVertices;

			// Token: 0x040007F9 RID: 2041
			private int mVerticesHash;

			// Token: 0x040007FA RID: 2042
			private VertexDeclaration mDeclaration;

			// Token: 0x040007FB RID: 2043
			private IndexBuffer mIndices;

			// Token: 0x040007FC RID: 2044
			private int mBaseVertex;

			// Token: 0x040007FD RID: 2045
			private int mNumVertices;

			// Token: 0x040007FE RID: 2046
			private int mPrimCount;

			// Token: 0x040007FF RID: 2047
			private int mStartIndex;

			// Token: 0x04000800 RID: 2048
			private int mVertexStride;

			// Token: 0x04000801 RID: 2049
			public float Damage;

			// Token: 0x04000802 RID: 2050
			public float Flash;

			// Token: 0x04000803 RID: 2051
			public Vector4 Colorize;

			// Token: 0x04000804 RID: 2052
			public BoundingSphere BoundingSphere;
		}
	}
}
