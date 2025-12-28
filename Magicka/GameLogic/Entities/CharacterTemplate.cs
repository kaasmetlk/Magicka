using System;
using System.Collections.Generic;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Abilities;
using Magicka.GameLogic.Entities.Buffs;
using Magicka.GameLogic.Entities.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead.Lights;
using XNAnimation;

namespace Magicka.GameLogic.Entities
{
	// Token: 0x02000166 RID: 358
	public class CharacterTemplate
	{
		// Token: 0x06000AAA RID: 2730 RVA: 0x00040BD0 File Offset: 0x0003EDD0
		public static CharacterTemplate GetCachedTemplate(int iID)
		{
			CharacterTemplate result;
			try
			{
				result = CharacterTemplate.mCachedTemplates[iID];
			}
			catch (Exception)
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000AAB RID: 2731 RVA: 0x00040C04 File Offset: 0x0003EE04
		public static void ClearCache()
		{
			foreach (CharacterTemplate characterTemplate in CharacterTemplate.mCachedTemplates.Values)
			{
				for (int i = 0; i < characterTemplate.Gibs.Length; i++)
				{
					Model mModel = characterTemplate.Gibs[i].mModel;
					for (int j = 0; j < mModel.Meshes.Count; j++)
					{
						ModelMesh modelMesh = mModel.Meshes[j];
						for (int k = 0; k < modelMesh.MeshParts.Count; k++)
						{
							if (!modelMesh.MeshParts[k].Effect.IsDisposed)
							{
								modelMesh.MeshParts[k].Effect.Dispose();
							}
							if (!modelMesh.MeshParts[k].VertexDeclaration.IsDisposed)
							{
								modelMesh.MeshParts[k].VertexDeclaration.Dispose();
							}
						}
						modelMesh.VertexBuffer.Dispose();
						modelMesh.IndexBuffer.Dispose();
					}
				}
			}
			CharacterTemplate.mCachedTemplates.Clear();
		}

		// Token: 0x06000AAC RID: 2732 RVA: 0x00040D60 File Offset: 0x0003EF60
		private CharacterTemplate()
		{
		}

		// Token: 0x06000AAD RID: 2733 RVA: 0x00040D80 File Offset: 0x0003EF80
		public static CharacterTemplate Read(ContentReader iInput)
		{
			if (string.Compare(iInput.AssetName, CharacterTemplate.endlessLoopFix) == 0)
			{
				return null;
			}
			CharacterTemplate.endlessLoopFix = iInput.AssetName;
			CharacterTemplate characterTemplate = new CharacterTemplate();
			characterTemplate.mID = iInput.ReadString().ToLowerInvariant();
			characterTemplate.mIDHash = characterTemplate.mID.GetHashCodeCustom();
			characterTemplate.mDisplayID = iInput.ReadString().ToLowerInvariant();
			characterTemplate.mDisplayIDHash = characterTemplate.mDisplayID.GetHashCodeCustom();
			characterTemplate.mFaction = (Factions)iInput.ReadInt32();
			characterTemplate.mBlood = (BloodType)iInput.ReadInt32();
			characterTemplate.mIsEthereal = iInput.ReadBoolean();
			characterTemplate.mLooksEthereal = iInput.ReadBoolean();
			characterTemplate.mFearless = iInput.ReadBoolean();
			characterTemplate.mUncharmable = iInput.ReadBoolean();
			characterTemplate.mNonslippery = iInput.ReadBoolean();
			characterTemplate.mHasFairy = iInput.ReadBoolean();
			characterTemplate.mCanSeeInvisible = iInput.ReadBoolean();
			int num = iInput.ReadInt32();
			characterTemplate.mAttachedSounds = new KeyValuePair<int, Banks>[4];
			for (int i = 0; i < num; i++)
			{
				if (i < 4)
				{
					string iString = iInput.ReadString().ToLowerInvariant();
					Banks value = (Banks)iInput.ReadInt32();
					int hashCodeCustom = iString.GetHashCodeCustom();
					characterTemplate.mAttachedSounds[i] = new KeyValuePair<int, Banks>(hashCodeCustom, value);
				}
			}
			int num2 = iInput.ReadInt32();
			characterTemplate.mGibs = new GibReference[num2];
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			for (int j = 0; j < num2; j++)
			{
				GibReference gibReference;
				lock (graphicsDevice)
				{
					gibReference.mModel = iInput.ReadExternalReference<Model>();
				}
				gibReference.mMass = iInput.ReadSingle();
				gibReference.mScale = iInput.ReadSingle();
				characterTemplate.mGibs[j] = gibReference;
			}
			int num3 = iInput.ReadInt32();
			characterTemplate.mPointLightHolder = new Character.PointLightHolder[4];
			for (int k = 0; k < num3; k++)
			{
				if (k >= 4)
				{
					throw new Exception("Character may Only have Four Lights!");
				}
				characterTemplate.mPointLightHolder[k] = default(Character.PointLightHolder);
				characterTemplate.PointLightHolder[k].ContainsLight = true;
				characterTemplate.PointLightHolder[k].JointName = iInput.ReadString();
				characterTemplate.mPointLightHolder[k].Radius = iInput.ReadSingle();
				characterTemplate.mPointLightHolder[k].DiffuseColor = iInput.ReadVector3();
				characterTemplate.mPointLightHolder[k].AmbientColor = iInput.ReadVector3();
				characterTemplate.mPointLightHolder[k].SpecularAmount = iInput.ReadSingle();
				characterTemplate.mPointLightHolder[k].VariationType = (LightVariationType)iInput.ReadByte();
				characterTemplate.mPointLightHolder[k].VariationAmount = iInput.ReadSingle();
				characterTemplate.mPointLightHolder[k].VariationSpeed = iInput.ReadSingle();
			}
			characterTemplate.mMaxHitpoints = iInput.ReadSingle();
			characterTemplate.mNumberOfHealthBars = iInput.ReadInt32();
			if (characterTemplate.mNumberOfHealthBars <= 0)
			{
				characterTemplate.mNumberOfHealthBars = 1;
			}
			characterTemplate.mUndying = iInput.ReadBoolean();
			characterTemplate.mUndieTime = iInput.ReadSingle();
			characterTemplate.mUndieHitPoints = iInput.ReadSingle();
			characterTemplate.mHitTolerance = iInput.ReadInt32();
			characterTemplate.mKnockdownTolerance = iInput.ReadSingle();
			characterTemplate.mScoreValue = iInput.ReadInt32();
			characterTemplate.mRegeneration = iInput.ReadInt32();
			characterTemplate.mMaxPanic = iInput.ReadSingle();
			characterTemplate.mZapModifier = iInput.ReadSingle();
			characterTemplate.mLength = Math.Max(iInput.ReadSingle(), 0.01f);
			characterTemplate.mRadius = iInput.ReadSingle();
			characterTemplate.mMass = iInput.ReadSingle();
			characterTemplate.mSpeed = iInput.ReadSingle();
			characterTemplate.mTurnSpeed = iInput.ReadSingle();
			characterTemplate.mBleedRate = iInput.ReadSingle();
			characterTemplate.mStunTime = iInput.ReadSingle();
			characterTemplate.mSummonElementBank = (Banks)iInput.ReadInt32();
			characterTemplate.mSummonElementCueString = iInput.ReadString();
			characterTemplate.mSummonElementCueID = characterTemplate.mSummonElementCueString.GetHashCodeCustom();
			int num4 = iInput.ReadInt32();
			characterTemplate.mResistances = new Resistance[11];
			for (int l = 0; l < characterTemplate.mResistances.Length; l++)
			{
				characterTemplate.mResistances[l].ResistanceAgainst = Defines.ElementFromIndex(l);
				characterTemplate.mResistances[l].Multiplier = 1f;
				characterTemplate.mResistances[l].Modifier = 0f;
				characterTemplate.mResistances[l].StatusResistance = false;
			}
			for (int m = 0; m < num4; m++)
			{
				Elements elements = (Elements)iInput.ReadInt32();
				int num5 = Defines.ElementIndex(elements);
				characterTemplate.mResistances[num5].ResistanceAgainst = elements;
				characterTemplate.mResistances[num5].Multiplier = iInput.ReadSingle();
				characterTemplate.mResistances[num5].Modifier = iInput.ReadSingle();
				characterTemplate.mResistances[num5].StatusResistance = iInput.ReadBoolean();
			}
			if ((characterTemplate.mFaction & Factions.FRIENDLY) == Factions.NONE)
			{
				float num6 = 1f - ((float)Game.Instance.PlayerCount - 1f) * 0.00666f;
				for (int n = 0; n < characterTemplate.mResistances.Length; n++)
				{
					if (characterTemplate.mResistances[n].Multiplier < 0f)
					{
						Resistance[] array = characterTemplate.mResistances;
						int num7 = n;
						array[num7].Multiplier = array[num7].Multiplier / num6;
					}
					else
					{
						Resistance[] array2 = characterTemplate.mResistances;
						int num8 = n;
						array2[num8].Multiplier = array2[num8].Multiplier * num6;
					}
					if (characterTemplate.mResistances[n].Modifier < 0f)
					{
						Resistance[] array3 = characterTemplate.mResistances;
						int num9 = n;
						array3[num9].Modifier = array3[num9].Modifier / num6;
					}
					else
					{
						Resistance[] array4 = characterTemplate.mResistances;
						int num10 = n;
						array4[num10].Modifier = array4[num10].Modifier * num6;
					}
				}
			}
			int num11 = iInput.ReadInt32();
			characterTemplate.mModels = new ModelProperties[num11];
			SkinnedModel skinnedModel;
			lock (graphicsDevice)
			{
				for (int num12 = 0; num12 < num11; num12++)
				{
					ModelProperties modelProperties;
					modelProperties.Model = iInput.ReadExternalReference<SkinnedModel>();
					modelProperties.Scale = iInput.ReadSingle();
					Matrix.CreateScale(modelProperties.Scale, out modelProperties.Transform);
					modelProperties.Tint = iInput.ReadVector3();
					characterTemplate.mModels[num12] = modelProperties;
				}
				skinnedModel = iInput.ReadExternalReference<SkinnedModel>();
			}
			characterTemplate.mSkeleton = skinnedModel.SkeletonBones;
			int num13 = iInput.ReadInt32();
			characterTemplate.mAttachedEffects = new KeyValuePair<int, int>[num13];
			for (int num14 = 0; num14 < num13; num14++)
			{
				string text = iInput.ReadString().ToLowerInvariant();
				string iString2 = iInput.ReadString().ToLowerInvariant();
				int hashCodeCustom2 = iString2.GetHashCodeCustom();
				int num15 = -1;
				for (int num16 = 0; num16 < skinnedModel.SkeletonBones.Count; num16++)
				{
					SkinnedModelBone skinnedModelBone = skinnedModel.SkeletonBones[num16];
					if (skinnedModelBone.Name.Equals(text, StringComparison.OrdinalIgnoreCase))
					{
						num15 = (int)skinnedModelBone.Index;
					}
				}
				if (num15 < 0)
				{
					throw new Exception(string.Format("Bone \"{0}\" not found!", text));
				}
				characterTemplate.mAttachedEffects[num14] = new KeyValuePair<int, int>(num15, hashCodeCustom2);
			}
			characterTemplate.mAnimationClips = new AnimationClipAction[27][];
			for (int num17 = 0; num17 < characterTemplate.mAnimationClips.Length; num17++)
			{
				int num18 = iInput.ReadInt32();
				if (num18 > 0)
				{
					characterTemplate.mAnimationClips[num17] = new AnimationClipAction[231];
					for (int num19 = 0; num19 < num18; num19++)
					{
						string value2 = iInput.ReadString();
						Animations animations = (Animations)Enum.Parse(typeof(Animations), value2, true);
						characterTemplate.mAnimationClips[num17][(int)animations] = new AnimationClipAction(animations, iInput, skinnedModel.AnimationClips, characterTemplate.mSkeleton);
					}
				}
			}
			characterTemplate.mEquipment = new Attachment[8];
			int num20 = iInput.ReadInt32();
			for (int num21 = 0; num21 < characterTemplate.mEquipment.Length; num21++)
			{
				characterTemplate.mEquipment[num21] = new Attachment(null, null);
			}
			for (int num22 = 0; num22 < num20; num22++)
			{
				int num23 = iInput.ReadInt32();
				characterTemplate.mEquipment[num23] = new Attachment(iInput, characterTemplate.mSkeleton);
			}
			for (int num24 = 0; num24 < skinnedModel.SkeletonBones.Count; num24++)
			{
				SkinnedModelBone skinnedModelBone2 = skinnedModel.SkeletonBones[num24];
				if (skinnedModelBone2.Name.Equals("Mouth", StringComparison.OrdinalIgnoreCase))
				{
					characterTemplate.mMouthJoint.mIndex = (int)skinnedModelBone2.Index;
					characterTemplate.mMouthJoint.mBindPose = Matrix.CreateRotationY(3.1415927f) * Matrix.Invert(skinnedModelBone2.InverseBindPoseTransform);
				}
				else if (skinnedModelBone2.Name.Equals("RightAttach", StringComparison.OrdinalIgnoreCase))
				{
					characterTemplate.mRightHandJoint.mIndex = (int)skinnedModelBone2.Index;
					characterTemplate.mRightHandJoint.mBindPose = Matrix.CreateRotationY(3.1415927f) * Matrix.Invert(skinnedModelBone2.InverseBindPoseTransform);
				}
				else if (skinnedModelBone2.Name.Equals("RightLeg", StringComparison.OrdinalIgnoreCase))
				{
					characterTemplate.mRightKneeJoint.mIndex = (int)skinnedModelBone2.Index;
					characterTemplate.mRightKneeJoint.mBindPose = Matrix.CreateRotationY(3.1415927f) * Matrix.Invert(skinnedModelBone2.InverseBindPoseTransform);
				}
				else if (skinnedModelBone2.Name.Equals("LeftAttach", StringComparison.OrdinalIgnoreCase))
				{
					characterTemplate.mLeftHandJoint.mIndex = (int)skinnedModelBone2.Index;
					characterTemplate.mLeftHandJoint.mBindPose = Matrix.CreateRotationY(3.1415927f) * Matrix.Invert(skinnedModelBone2.InverseBindPoseTransform);
				}
				else if (skinnedModelBone2.Name.Equals("LeftLeg", StringComparison.OrdinalIgnoreCase))
				{
					characterTemplate.mLeftKneeJoint.mIndex = (int)skinnedModelBone2.Index;
					characterTemplate.mLeftKneeJoint.mBindPose = Matrix.CreateRotationY(3.1415927f) * Matrix.Invert(skinnedModelBone2.InverseBindPoseTransform);
				}
				else if (skinnedModelBone2.Index == 1)
				{
					characterTemplate.mHipJoint.mIndex = (int)skinnedModelBone2.Index;
					characterTemplate.mHipJoint.mBindPose = Matrix.CreateRotationY(3.1415927f) * Matrix.Invert(skinnedModelBone2.InverseBindPoseTransform);
				}
				for (int num25 = 0; num25 < characterTemplate.PointLightHolder.Length; num25++)
				{
					if (characterTemplate.PointLightHolder != null && skinnedModelBone2.Name.Equals(characterTemplate.PointLightHolder[num25].JointName, StringComparison.OrdinalIgnoreCase))
					{
						characterTemplate.PointLightHolder[num25].Joint.mIndex = num24;
						characterTemplate.PointLightHolder[num25].Joint.mBindPose = Matrix.CreateRotationY(3.1415927f) * Matrix.Invert(skinnedModelBone2.InverseBindPoseTransform);
					}
				}
			}
			characterTemplate.mEventConditions = new ConditionCollection(iInput);
			characterTemplate.mAlertRadius = iInput.ReadSingle();
			characterTemplate.mGroupChase = iInput.ReadSingle();
			characterTemplate.mGroupSeparation = iInput.ReadSingle();
			characterTemplate.mGroupCohesion = iInput.ReadSingle();
			characterTemplate.mGroupAlignment = iInput.ReadSingle();
			characterTemplate.mGroupWander = iInput.ReadSingle();
			characterTemplate.mFriendlyAvoidance = iInput.ReadSingle();
			characterTemplate.mEnemyAvoidance = iInput.ReadSingle();
			characterTemplate.mSightAvoidance = iInput.ReadSingle();
			characterTemplate.mDangerAvoidance = iInput.ReadSingle();
			characterTemplate.mAngerWeight = iInput.ReadSingle();
			characterTemplate.mDistanceWeight = iInput.ReadSingle();
			characterTemplate.mHealthWeight = iInput.ReadSingle();
			characterTemplate.mFlocking = iInput.ReadBoolean();
			characterTemplate.mBreakFreeStrength = iInput.ReadSingle();
			characterTemplate.mAbilities = new Ability[iInput.ReadInt32()];
			for (int num26 = 0; num26 < characterTemplate.mAbilities.Length; num26++)
			{
				characterTemplate.mAbilities[num26] = Ability.Read(num26, iInput, characterTemplate.mAnimationClips);
			}
			int num27 = iInput.ReadInt32();
			characterTemplate.mMoveAnimations = new Dictionary<byte, Animations[]>(num27);
			characterTemplate.mMoveAbilities = MovementProperties.Default;
			for (int num28 = 0; num28 < num27; num28++)
			{
				MovementProperties movementProperties = (MovementProperties)iInput.ReadByte();
				Animations[] array5 = new Animations[iInput.ReadInt32()];
				for (int num29 = 0; num29 < array5.Length; num29++)
				{
					array5[num29] = (Animations)Enum.Parse(typeof(Animations), iInput.ReadString(), true);
				}
				characterTemplate.mMoveAbilities |= movementProperties;
				characterTemplate.mMoveAnimations.Add((byte)movementProperties, array5);
			}
			if (CharacterTemplate.sSkeletonVertexDeclaration == null)
			{
				lock (graphicsDevice)
				{
					CharacterTemplate.sSkeletonVertexDeclaration = new VertexDeclaration(graphicsDevice, CharacterTemplate.VertexPositionIndexWeight.VertexElements);
				}
			}
			CharacterTemplate.GenerateSkeletonModel(characterTemplate.mModels[0].Model.SkeletonBones, 0.125f * characterTemplate.mRadius / characterTemplate.mModels[0].Scale, out characterTemplate.mSkeletonVertices, out characterTemplate.mSkeletonPrimitiveCount);
			int num30 = iInput.ReadInt32();
			for (int num31 = 0; num31 < num30; num31++)
			{
				BuffStorage item = new BuffStorage(iInput);
				characterTemplate.mBuffs.Add(item);
			}
			num30 = iInput.ReadInt32();
			for (int num32 = 0; num32 < num30; num32++)
			{
				AuraStorage item2 = new AuraStorage(iInput);
				if (item2.TTL <= 0f)
				{
					item2.TTL = float.PositiveInfinity;
				}
				characterTemplate.mAuras.Add(item2);
			}
			CharacterTemplate.mCachedTemplates[characterTemplate.mIDHash] = characterTemplate;
			return characterTemplate;
		}

		// Token: 0x17000252 RID: 594
		// (get) Token: 0x06000AAE RID: 2734 RVA: 0x00041B20 File Offset: 0x0003FD20
		public int Regeneration
		{
			get
			{
				return this.mRegeneration;
			}
		}

		// Token: 0x17000253 RID: 595
		// (get) Token: 0x06000AAF RID: 2735 RVA: 0x00041B28 File Offset: 0x0003FD28
		public int DisplayName
		{
			get
			{
				return this.mDisplayIDHash;
			}
		}

		// Token: 0x17000254 RID: 596
		// (get) Token: 0x06000AB0 RID: 2736 RVA: 0x00041B30 File Offset: 0x0003FD30
		public int ID
		{
			get
			{
				return this.mIDHash;
			}
		}

		// Token: 0x17000255 RID: 597
		// (get) Token: 0x06000AB1 RID: 2737 RVA: 0x00041B38 File Offset: 0x0003FD38
		public string Name
		{
			get
			{
				return this.mID;
			}
		}

		// Token: 0x17000256 RID: 598
		// (get) Token: 0x06000AB2 RID: 2738 RVA: 0x00041B40 File Offset: 0x0003FD40
		public bool IsEthereal
		{
			get
			{
				return this.mIsEthereal;
			}
		}

		// Token: 0x17000257 RID: 599
		// (get) Token: 0x06000AB3 RID: 2739 RVA: 0x00041B48 File Offset: 0x0003FD48
		public bool LooksEthereal
		{
			get
			{
				return this.mLooksEthereal;
			}
		}

		// Token: 0x17000258 RID: 600
		// (get) Token: 0x06000AB4 RID: 2740 RVA: 0x00041B50 File Offset: 0x0003FD50
		public bool CanSeeInvisible
		{
			get
			{
				return this.mCanSeeInvisible;
			}
		}

		// Token: 0x17000259 RID: 601
		// (get) Token: 0x06000AB5 RID: 2741 RVA: 0x00041B58 File Offset: 0x0003FD58
		public float ZapModifier
		{
			get
			{
				return this.mZapModifier;
			}
		}

		// Token: 0x1700025A RID: 602
		// (get) Token: 0x06000AB6 RID: 2742 RVA: 0x00041B60 File Offset: 0x0003FD60
		public float TurnSpeed
		{
			get
			{
				return this.mTurnSpeed;
			}
		}

		// Token: 0x1700025B RID: 603
		// (get) Token: 0x06000AB7 RID: 2743 RVA: 0x00041B68 File Offset: 0x0003FD68
		public BloodType Blood
		{
			get
			{
				return this.mBlood;
			}
		}

		// Token: 0x1700025C RID: 604
		// (get) Token: 0x06000AB8 RID: 2744 RVA: 0x00041B70 File Offset: 0x0003FD70
		public GibReference[] Gibs
		{
			get
			{
				return this.mGibs;
			}
		}

		// Token: 0x1700025D RID: 605
		// (get) Token: 0x06000AB9 RID: 2745 RVA: 0x00041B78 File Offset: 0x0003FD78
		public Attachment[] Equipment
		{
			get
			{
				return this.mEquipment;
			}
		}

		// Token: 0x1700025E RID: 606
		// (get) Token: 0x06000ABA RID: 2746 RVA: 0x00041B80 File Offset: 0x0003FD80
		public float MaxPanic
		{
			get
			{
				return this.mMaxPanic;
			}
		}

		// Token: 0x1700025F RID: 607
		// (get) Token: 0x06000ABB RID: 2747 RVA: 0x00041B88 File Offset: 0x0003FD88
		public float MaxHitpoints
		{
			get
			{
				return this.mMaxHitpoints;
			}
		}

		// Token: 0x17000260 RID: 608
		// (get) Token: 0x06000ABC RID: 2748 RVA: 0x00041B90 File Offset: 0x0003FD90
		public int NumberOfHealthBars
		{
			get
			{
				return this.mNumberOfHealthBars;
			}
		}

		// Token: 0x17000261 RID: 609
		// (get) Token: 0x06000ABD RID: 2749 RVA: 0x00041B98 File Offset: 0x0003FD98
		public bool Undying
		{
			get
			{
				return this.mUndying;
			}
		}

		// Token: 0x17000262 RID: 610
		// (get) Token: 0x06000ABE RID: 2750 RVA: 0x00041BA0 File Offset: 0x0003FDA0
		public float UndieTime
		{
			get
			{
				return this.mUndieTime;
			}
		}

		// Token: 0x17000263 RID: 611
		// (get) Token: 0x06000ABF RID: 2751 RVA: 0x00041BA8 File Offset: 0x0003FDA8
		public float UndieHitPoints
		{
			get
			{
				return this.mUndieHitPoints;
			}
		}

		// Token: 0x17000264 RID: 612
		// (get) Token: 0x06000AC0 RID: 2752 RVA: 0x00041BB0 File Offset: 0x0003FDB0
		public int HitTolerance
		{
			get
			{
				return this.mHitTolerance;
			}
		}

		// Token: 0x17000265 RID: 613
		// (get) Token: 0x06000AC1 RID: 2753 RVA: 0x00041BB8 File Offset: 0x0003FDB8
		public float KnockdownTolerance
		{
			get
			{
				return this.mKnockdownTolerance;
			}
		}

		// Token: 0x17000266 RID: 614
		// (get) Token: 0x06000AC2 RID: 2754 RVA: 0x00041BC0 File Offset: 0x0003FDC0
		public int ScoreValue
		{
			get
			{
				return this.mScoreValue;
			}
		}

		// Token: 0x17000267 RID: 615
		// (get) Token: 0x06000AC3 RID: 2755 RVA: 0x00041BC8 File Offset: 0x0003FDC8
		public float Length
		{
			get
			{
				return this.mLength;
			}
		}

		// Token: 0x17000268 RID: 616
		// (get) Token: 0x06000AC4 RID: 2756 RVA: 0x00041BD0 File Offset: 0x0003FDD0
		public float Radius
		{
			get
			{
				return this.mRadius;
			}
		}

		// Token: 0x17000269 RID: 617
		// (get) Token: 0x06000AC5 RID: 2757 RVA: 0x00041BD8 File Offset: 0x0003FDD8
		public float Mass
		{
			get
			{
				return this.mMass;
			}
		}

		// Token: 0x1700026A RID: 618
		// (get) Token: 0x06000AC6 RID: 2758 RVA: 0x00041BE0 File Offset: 0x0003FDE0
		public float Speed
		{
			get
			{
				return this.mSpeed;
			}
		}

		// Token: 0x1700026B RID: 619
		// (get) Token: 0x06000AC7 RID: 2759 RVA: 0x00041BE8 File Offset: 0x0003FDE8
		public int SummonElementCue
		{
			get
			{
				return this.mSummonElementCueID;
			}
		}

		// Token: 0x1700026C RID: 620
		// (get) Token: 0x06000AC8 RID: 2760 RVA: 0x00041BF0 File Offset: 0x0003FDF0
		public Banks SummonElementBank
		{
			get
			{
				return this.mSummonElementBank;
			}
		}

		// Token: 0x1700026D RID: 621
		// (get) Token: 0x06000AC9 RID: 2761 RVA: 0x00041BF8 File Offset: 0x0003FDF8
		public float BleedRate
		{
			get
			{
				return this.mBleedRate;
			}
		}

		// Token: 0x1700026E RID: 622
		// (get) Token: 0x06000ACA RID: 2762 RVA: 0x00041C00 File Offset: 0x0003FE00
		public float StunTime
		{
			get
			{
				return this.mStunTime;
			}
		}

		// Token: 0x1700026F RID: 623
		// (get) Token: 0x06000ACB RID: 2763 RVA: 0x00041C08 File Offset: 0x0003FE08
		public ModelProperties[] Models
		{
			get
			{
				return this.mModels;
			}
		}

		// Token: 0x17000270 RID: 624
		// (get) Token: 0x06000ACC RID: 2764 RVA: 0x00041C10 File Offset: 0x0003FE10
		public SkinnedModelBoneCollection Skeleton
		{
			get
			{
				return this.mSkeleton;
			}
		}

		// Token: 0x17000271 RID: 625
		// (get) Token: 0x06000ACD RID: 2765 RVA: 0x00041C18 File Offset: 0x0003FE18
		public VertexDeclaration SkeletonVertexDeclaration
		{
			get
			{
				return CharacterTemplate.sSkeletonVertexDeclaration;
			}
		}

		// Token: 0x17000272 RID: 626
		// (get) Token: 0x06000ACE RID: 2766 RVA: 0x00041C1F File Offset: 0x0003FE1F
		public VertexBuffer SkeletonVertices
		{
			get
			{
				return this.mSkeletonVertices;
			}
		}

		// Token: 0x17000273 RID: 627
		// (get) Token: 0x06000ACF RID: 2767 RVA: 0x00041C27 File Offset: 0x0003FE27
		public int SkeletonPrimitiveCount
		{
			get
			{
				return this.mSkeletonPrimitiveCount;
			}
		}

		// Token: 0x17000274 RID: 628
		// (get) Token: 0x06000AD0 RID: 2768 RVA: 0x00041C2F File Offset: 0x0003FE2F
		public int SkeletonVertexStride
		{
			get
			{
				return 48;
			}
		}

		// Token: 0x17000275 RID: 629
		// (get) Token: 0x06000AD1 RID: 2769 RVA: 0x00041C33 File Offset: 0x0003FE33
		public AnimationClipAction[][] AnimationClips
		{
			get
			{
				return this.mAnimationClips;
			}
		}

		// Token: 0x17000276 RID: 630
		// (get) Token: 0x06000AD2 RID: 2770 RVA: 0x00041C3B File Offset: 0x0003FE3B
		public Resistance[] Resistances
		{
			get
			{
				return this.mResistances;
			}
		}

		// Token: 0x17000277 RID: 631
		// (get) Token: 0x06000AD3 RID: 2771 RVA: 0x00041C43 File Offset: 0x0003FE43
		public BindJoint HipJoint
		{
			get
			{
				return this.mHipJoint;
			}
		}

		// Token: 0x17000278 RID: 632
		// (get) Token: 0x06000AD4 RID: 2772 RVA: 0x00041C4B File Offset: 0x0003FE4B
		public BindJoint LeftHandJoint
		{
			get
			{
				return this.mLeftHandJoint;
			}
		}

		// Token: 0x17000279 RID: 633
		// (get) Token: 0x06000AD5 RID: 2773 RVA: 0x00041C53 File Offset: 0x0003FE53
		public BindJoint LeftKneeJoint
		{
			get
			{
				return this.mLeftKneeJoint;
			}
		}

		// Token: 0x1700027A RID: 634
		// (get) Token: 0x06000AD6 RID: 2774 RVA: 0x00041C5B File Offset: 0x0003FE5B
		public BindJoint RightHandJoint
		{
			get
			{
				return this.mRightHandJoint;
			}
		}

		// Token: 0x1700027B RID: 635
		// (get) Token: 0x06000AD7 RID: 2775 RVA: 0x00041C63 File Offset: 0x0003FE63
		public BindJoint RightKneeJoint
		{
			get
			{
				return this.mRightKneeJoint;
			}
		}

		// Token: 0x1700027C RID: 636
		// (get) Token: 0x06000AD8 RID: 2776 RVA: 0x00041C6B File Offset: 0x0003FE6B
		public BindJoint MouthJoint
		{
			get
			{
				return this.mMouthJoint;
			}
		}

		// Token: 0x1700027D RID: 637
		// (get) Token: 0x06000AD9 RID: 2777 RVA: 0x00041C73 File Offset: 0x0003FE73
		public KeyValuePair<int, Banks>[] AttachedSounds
		{
			get
			{
				return this.mAttachedSounds;
			}
		}

		// Token: 0x1700027E RID: 638
		// (get) Token: 0x06000ADA RID: 2778 RVA: 0x00041C7B File Offset: 0x0003FE7B
		public KeyValuePair<int, int>[] AttachedEffects
		{
			get
			{
				return this.mAttachedEffects;
			}
		}

		// Token: 0x1700027F RID: 639
		// (get) Token: 0x06000ADB RID: 2779 RVA: 0x00041C83 File Offset: 0x0003FE83
		public Character.PointLightHolder[] PointLightHolder
		{
			get
			{
				return this.mPointLightHolder;
			}
		}

		// Token: 0x17000280 RID: 640
		// (get) Token: 0x06000ADC RID: 2780 RVA: 0x00041C8B File Offset: 0x0003FE8B
		public ConditionCollection EventConditions
		{
			get
			{
				return this.mEventConditions;
			}
		}

		// Token: 0x17000281 RID: 641
		// (get) Token: 0x06000ADD RID: 2781 RVA: 0x00041C93 File Offset: 0x0003FE93
		public List<AuraStorage> Auras
		{
			get
			{
				return this.mAuras;
			}
		}

		// Token: 0x17000282 RID: 642
		// (get) Token: 0x06000ADE RID: 2782 RVA: 0x00041C9B File Offset: 0x0003FE9B
		public List<BuffStorage> Buffs
		{
			get
			{
				return this.mBuffs;
			}
		}

		// Token: 0x17000283 RID: 643
		// (get) Token: 0x06000ADF RID: 2783 RVA: 0x00041CA3 File Offset: 0x0003FEA3
		public bool IsFearless
		{
			get
			{
				return this.mFearless;
			}
		}

		// Token: 0x17000284 RID: 644
		// (get) Token: 0x06000AE0 RID: 2784 RVA: 0x00041CAB File Offset: 0x0003FEAB
		public bool IsUncharmable
		{
			get
			{
				return this.mUncharmable;
			}
		}

		// Token: 0x17000285 RID: 645
		// (get) Token: 0x06000AE1 RID: 2785 RVA: 0x00041CB3 File Offset: 0x0003FEB3
		public bool IsNonslippery
		{
			get
			{
				return this.mNonslippery;
			}
		}

		// Token: 0x17000286 RID: 646
		// (get) Token: 0x06000AE2 RID: 2786 RVA: 0x00041CBB File Offset: 0x0003FEBB
		public bool HasFairy
		{
			get
			{
				return this.mHasFairy;
			}
		}

		// Token: 0x17000287 RID: 647
		// (get) Token: 0x06000AE3 RID: 2787 RVA: 0x00041CC3 File Offset: 0x0003FEC3
		public float AlertRadius
		{
			get
			{
				return this.mAlertRadius;
			}
		}

		// Token: 0x17000288 RID: 648
		// (get) Token: 0x06000AE4 RID: 2788 RVA: 0x00041CCB File Offset: 0x0003FECB
		public float GroupChase
		{
			get
			{
				return this.mGroupChase;
			}
		}

		// Token: 0x17000289 RID: 649
		// (get) Token: 0x06000AE5 RID: 2789 RVA: 0x00041CD3 File Offset: 0x0003FED3
		public float GroupSeparation
		{
			get
			{
				return this.mGroupSeparation;
			}
		}

		// Token: 0x1700028A RID: 650
		// (get) Token: 0x06000AE6 RID: 2790 RVA: 0x00041CDB File Offset: 0x0003FEDB
		public float GroupCohesion
		{
			get
			{
				return this.mGroupCohesion;
			}
		}

		// Token: 0x1700028B RID: 651
		// (get) Token: 0x06000AE7 RID: 2791 RVA: 0x00041CE3 File Offset: 0x0003FEE3
		public float GroupAlignment
		{
			get
			{
				return this.mGroupAlignment;
			}
		}

		// Token: 0x1700028C RID: 652
		// (get) Token: 0x06000AE8 RID: 2792 RVA: 0x00041CEB File Offset: 0x0003FEEB
		public float GroupWander
		{
			get
			{
				return this.mGroupWander;
			}
		}

		// Token: 0x1700028D RID: 653
		// (get) Token: 0x06000AE9 RID: 2793 RVA: 0x00041CF3 File Offset: 0x0003FEF3
		public float FriendlyAvoidance
		{
			get
			{
				return this.mFriendlyAvoidance;
			}
		}

		// Token: 0x1700028E RID: 654
		// (get) Token: 0x06000AEA RID: 2794 RVA: 0x00041CFB File Offset: 0x0003FEFB
		public float EnemyAvoidance
		{
			get
			{
				return this.mEnemyAvoidance;
			}
		}

		// Token: 0x1700028F RID: 655
		// (get) Token: 0x06000AEB RID: 2795 RVA: 0x00041D03 File Offset: 0x0003FF03
		public float SightAvoidance
		{
			get
			{
				return this.mSightAvoidance;
			}
		}

		// Token: 0x17000290 RID: 656
		// (get) Token: 0x06000AEC RID: 2796 RVA: 0x00041D0B File Offset: 0x0003FF0B
		public float DangerAvoidance
		{
			get
			{
				return this.mDangerAvoidance;
			}
		}

		// Token: 0x17000291 RID: 657
		// (get) Token: 0x06000AED RID: 2797 RVA: 0x00041D13 File Offset: 0x0003FF13
		public float AngerWeight
		{
			get
			{
				return this.mAngerWeight;
			}
		}

		// Token: 0x17000292 RID: 658
		// (get) Token: 0x06000AEE RID: 2798 RVA: 0x00041D1B File Offset: 0x0003FF1B
		public float DistanceWeight
		{
			get
			{
				return this.mDistanceWeight;
			}
		}

		// Token: 0x17000293 RID: 659
		// (get) Token: 0x06000AEF RID: 2799 RVA: 0x00041D23 File Offset: 0x0003FF23
		public float HealthWeight
		{
			get
			{
				return this.mHealthWeight;
			}
		}

		// Token: 0x17000294 RID: 660
		// (get) Token: 0x06000AF0 RID: 2800 RVA: 0x00041D2B File Offset: 0x0003FF2B
		public bool Flocking
		{
			get
			{
				return this.mFlocking;
			}
		}

		// Token: 0x17000295 RID: 661
		// (get) Token: 0x06000AF1 RID: 2801 RVA: 0x00041D33 File Offset: 0x0003FF33
		public float BreakFreeStrength
		{
			get
			{
				return this.mBreakFreeStrength;
			}
		}

		// Token: 0x17000296 RID: 662
		// (get) Token: 0x06000AF2 RID: 2802 RVA: 0x00041D3B File Offset: 0x0003FF3B
		public Factions Faction
		{
			get
			{
				return this.mFaction;
			}
		}

		// Token: 0x17000297 RID: 663
		// (get) Token: 0x06000AF3 RID: 2803 RVA: 0x00041D43 File Offset: 0x0003FF43
		public Ability[] Abilities
		{
			get
			{
				return this.mAbilities;
			}
		}

		// Token: 0x17000298 RID: 664
		// (get) Token: 0x06000AF4 RID: 2804 RVA: 0x00041D4B File Offset: 0x0003FF4B
		public MovementProperties MoveAbilities
		{
			get
			{
				return this.mMoveAbilities;
			}
		}

		// Token: 0x17000299 RID: 665
		// (get) Token: 0x06000AF5 RID: 2805 RVA: 0x00041D53 File Offset: 0x0003FF53
		public Dictionary<byte, Animations[]> MoveAnimations
		{
			get
			{
				return this.mMoveAnimations;
			}
		}

		// Token: 0x06000AF6 RID: 2806 RVA: 0x00041D5C File Offset: 0x0003FF5C
		private static void GenerateSkeletonModel(SkinnedModelBoneCollection iSkeleton, float iScale, out VertexBuffer oVertexBuffer, out int oPrimitiveCount)
		{
			List<CharacterTemplate.VertexPositionIndexWeight> list = new List<CharacterTemplate.VertexPositionIndexWeight>();
			for (int i = 1; i < iSkeleton.Count; i++)
			{
				SkinnedModelBone skinnedModelBone = iSkeleton[i];
				Matrix inverseBindPoseTransform = skinnedModelBone.InverseBindPoseTransform;
				Matrix.Invert(ref inverseBindPoseTransform, out inverseBindPoseTransform);
				float num = 0f;
				while (skinnedModelBone.Parent != null)
				{
					skinnedModelBone = skinnedModelBone.Parent;
					num += 1f;
				}
				skinnedModelBone = iSkeleton[i];
				float iRadius = (float)Math.Pow(0.8, (double)(num - 1f)) * iScale;
				CharacterTemplate.GenerateBall(list, inverseBindPoseTransform, (float)i, iRadius);
				if (i > 1)
				{
					CharacterTemplate.GeneratePyramid(list, skinnedModelBone, iRadius);
				}
			}
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			if (list.Count == 0)
			{
				oVertexBuffer = null;
				oPrimitiveCount = 0;
				return;
			}
			lock (graphicsDevice)
			{
				oVertexBuffer = new VertexBuffer(graphicsDevice, list.Count * 48, BufferUsage.WriteOnly);
				oVertexBuffer.SetData<CharacterTemplate.VertexPositionIndexWeight>(list.ToArray());
			}
			oPrimitiveCount = list.Count - 1;
		}

		// Token: 0x06000AF7 RID: 2807 RVA: 0x00041E64 File Offset: 0x00040064
		private static void GenerateBall(List<CharacterTemplate.VertexPositionIndexWeight> iVertices, Matrix iTransform, float iBoneIndex, float iRadius)
		{
			Vector3 vector = default(Vector3);
			vector.Z = iRadius;
			CharacterTemplate.VertexPositionIndexWeight item = default(CharacterTemplate.VertexPositionIndexWeight);
			item.Color.PackedValue = 0U;
			item.Indices = iBoneIndex;
			Vector3.Transform(ref vector, ref iTransform, out item.Position);
			iVertices.Add(item);
			item.Color.PackedValue = uint.MaxValue;
			iVertices.Add(item);
			for (int i = 0; i < 12; i++)
			{
				float num = 1f;
				if (i >= 4 & i != 7 & i != 11)
				{
					num = -1f;
				}
				num /= 2f;
				Quaternion quaternion;
				if (i % 3 == 0)
				{
					Quaternion.CreateFromYawPitchRoll(0f, num * -1.5707964f, 0f, out quaternion);
				}
				else if (i % 3 == 1)
				{
					Quaternion.CreateFromYawPitchRoll(0f, 0f, num * -1.5707964f, out quaternion);
				}
				else
				{
					Quaternion.CreateFromYawPitchRoll(num * 1.5707964f, 0f, 0f, out quaternion);
				}
				for (int j = 0; j < 2; j++)
				{
					Vector3.Transform(ref vector, ref quaternion, out vector);
					Vector3.Transform(ref vector, ref iTransform, out item.Position);
					iVertices.Add(item);
				}
			}
			item.Color.PackedValue = 0U;
			iVertices.Add(item);
		}

		// Token: 0x06000AF8 RID: 2808 RVA: 0x00041FB0 File Offset: 0x000401B0
		private static void GeneratePyramid(List<CharacterTemplate.VertexPositionIndexWeight> iVertices, SkinnedModelBone iChild, float iRadius)
		{
			Matrix inverseBindPoseTransform = iChild.InverseBindPoseTransform;
			Matrix.Invert(ref inverseBindPoseTransform, out inverseBindPoseTransform);
			Matrix inverseBindPoseTransform2 = iChild.Parent.InverseBindPoseTransform;
			Matrix.Invert(ref inverseBindPoseTransform2, out inverseBindPoseTransform2);
			CharacterTemplate.VertexPositionIndexWeight item = default(CharacterTemplate.VertexPositionIndexWeight);
			item.Offset.Z = -iRadius;
			item.Position = inverseBindPoseTransform.Translation;
			item.Indices = (float)iChild.Index;
			item.NextPosition = inverseBindPoseTransform2.Translation;
			item.NextIndices = (float)iChild.Parent.Index;
			item.Color = Color.White;
			CharacterTemplate.VertexPositionIndexWeight item2 = default(CharacterTemplate.VertexPositionIndexWeight);
			item2.Position = inverseBindPoseTransform2.Translation;
			item2.Indices = (float)iChild.Parent.Index;
			item2.NextPosition = inverseBindPoseTransform.Translation;
			item2.NextIndices = (float)iChild.Index;
			item2.Offset.Z = -iRadius / 0.8f;
			item2.Offset.X = iRadius / 0.8f;
			Quaternion quaternion;
			Quaternion.CreateFromYawPitchRoll(0f, 0f, 1.5707964f, out quaternion);
			for (int i = 0; i < 4; i++)
			{
				item2.Color.PackedValue = 0U;
				iVertices.Add(item2);
				item2.Color.PackedValue = uint.MaxValue;
				iVertices.Add(item2);
				Vector3.Transform(ref item2.Offset, ref quaternion, out item2.Offset);
				iVertices.Add(item2);
				item.Color.PackedValue = uint.MaxValue;
				iVertices.Add(item);
				item.Color.PackedValue = 0U;
				iVertices.Add(item);
			}
		}

		// Token: 0x040009AE RID: 2478
		private static string endlessLoopFix = "";

		// Token: 0x040009AF RID: 2479
		private static Dictionary<int, CharacterTemplate> mCachedTemplates = new Dictionary<int, CharacterTemplate>();

		// Token: 0x040009B0 RID: 2480
		private string mDisplayID;

		// Token: 0x040009B1 RID: 2481
		private int mDisplayIDHash;

		// Token: 0x040009B2 RID: 2482
		private string mID;

		// Token: 0x040009B3 RID: 2483
		private int mIDHash;

		// Token: 0x040009B4 RID: 2484
		private BloodType mBlood;

		// Token: 0x040009B5 RID: 2485
		private bool mIsEthereal;

		// Token: 0x040009B6 RID: 2486
		private bool mLooksEthereal;

		// Token: 0x040009B7 RID: 2487
		private bool mCanSeeInvisible;

		// Token: 0x040009B8 RID: 2488
		private bool mFearless;

		// Token: 0x040009B9 RID: 2489
		private bool mUncharmable;

		// Token: 0x040009BA RID: 2490
		private bool mNonslippery;

		// Token: 0x040009BB RID: 2491
		private bool mHasFairy;

		// Token: 0x040009BC RID: 2492
		private KeyValuePair<int, Banks>[] mAttachedSounds;

		// Token: 0x040009BD RID: 2493
		private Character.PointLightHolder[] mPointLightHolder;

		// Token: 0x040009BE RID: 2494
		private GibReference[] mGibs;

		// Token: 0x040009BF RID: 2495
		private Factions mFaction;

		// Token: 0x040009C0 RID: 2496
		private Attachment[] mEquipment;

		// Token: 0x040009C1 RID: 2497
		private float mMaxHitpoints;

		// Token: 0x040009C2 RID: 2498
		private bool mUndying;

		// Token: 0x040009C3 RID: 2499
		private float mUndieTime;

		// Token: 0x040009C4 RID: 2500
		private float mUndieHitPoints;

		// Token: 0x040009C5 RID: 2501
		private int mHitTolerance;

		// Token: 0x040009C6 RID: 2502
		private float mKnockdownTolerance;

		// Token: 0x040009C7 RID: 2503
		private int mScoreValue;

		// Token: 0x040009C8 RID: 2504
		private int mRegeneration;

		// Token: 0x040009C9 RID: 2505
		private float mMaxPanic;

		// Token: 0x040009CA RID: 2506
		private float mZapModifier;

		// Token: 0x040009CB RID: 2507
		private float mTurnSpeed;

		// Token: 0x040009CC RID: 2508
		private float mLength;

		// Token: 0x040009CD RID: 2509
		private float mRadius;

		// Token: 0x040009CE RID: 2510
		private float mMass;

		// Token: 0x040009CF RID: 2511
		private float mSpeed;

		// Token: 0x040009D0 RID: 2512
		private int mNumberOfHealthBars;

		// Token: 0x040009D1 RID: 2513
		private float mBleedRate;

		// Token: 0x040009D2 RID: 2514
		private float mStunTime;

		// Token: 0x040009D3 RID: 2515
		private Resistance[] mResistances;

		// Token: 0x040009D4 RID: 2516
		private KeyValuePair<int, int>[] mAttachedEffects;

		// Token: 0x040009D5 RID: 2517
		private ModelProperties[] mModels;

		// Token: 0x040009D6 RID: 2518
		private SkinnedModelBoneCollection mSkeleton;

		// Token: 0x040009D7 RID: 2519
		private AnimationClipAction[][] mAnimationClips;

		// Token: 0x040009D8 RID: 2520
		private static VertexDeclaration sSkeletonVertexDeclaration;

		// Token: 0x040009D9 RID: 2521
		private VertexBuffer mSkeletonVertices;

		// Token: 0x040009DA RID: 2522
		private int mSkeletonPrimitiveCount;

		// Token: 0x040009DB RID: 2523
		private BindJoint mRightHandJoint;

		// Token: 0x040009DC RID: 2524
		private BindJoint mRightKneeJoint;

		// Token: 0x040009DD RID: 2525
		private BindJoint mLeftHandJoint;

		// Token: 0x040009DE RID: 2526
		private BindJoint mLeftKneeJoint;

		// Token: 0x040009DF RID: 2527
		private BindJoint mMouthJoint;

		// Token: 0x040009E0 RID: 2528
		private BindJoint mHipJoint;

		// Token: 0x040009E1 RID: 2529
		private ConditionCollection mEventConditions;

		// Token: 0x040009E2 RID: 2530
		private Banks mSummonElementBank;

		// Token: 0x040009E3 RID: 2531
		private int mSummonElementCueID;

		// Token: 0x040009E4 RID: 2532
		private string mSummonElementCueString;

		// Token: 0x040009E5 RID: 2533
		private float mAlertRadius;

		// Token: 0x040009E6 RID: 2534
		private float mGroupChase;

		// Token: 0x040009E7 RID: 2535
		private float mGroupSeparation;

		// Token: 0x040009E8 RID: 2536
		private float mGroupCohesion;

		// Token: 0x040009E9 RID: 2537
		private float mGroupAlignment;

		// Token: 0x040009EA RID: 2538
		private float mGroupWander;

		// Token: 0x040009EB RID: 2539
		private float mFriendlyAvoidance;

		// Token: 0x040009EC RID: 2540
		private float mEnemyAvoidance;

		// Token: 0x040009ED RID: 2541
		private float mSightAvoidance;

		// Token: 0x040009EE RID: 2542
		private float mDangerAvoidance;

		// Token: 0x040009EF RID: 2543
		private float mBreakFreeStrength;

		// Token: 0x040009F0 RID: 2544
		private float mAngerWeight;

		// Token: 0x040009F1 RID: 2545
		private float mDistanceWeight;

		// Token: 0x040009F2 RID: 2546
		private float mHealthWeight;

		// Token: 0x040009F3 RID: 2547
		private bool mFlocking;

		// Token: 0x040009F4 RID: 2548
		private Ability[] mAbilities;

		// Token: 0x040009F5 RID: 2549
		private MovementProperties mMoveAbilities;

		// Token: 0x040009F6 RID: 2550
		private Dictionary<byte, Animations[]> mMoveAnimations;

		// Token: 0x040009F7 RID: 2551
		private List<AuraStorage> mAuras = new List<AuraStorage>();

		// Token: 0x040009F8 RID: 2552
		private List<BuffStorage> mBuffs = new List<BuffStorage>();

		// Token: 0x02000167 RID: 359
		private struct VertexPositionIndexWeight
		{
			// Token: 0x040009F9 RID: 2553
			public const int SIZEINBYTES = 48;

			// Token: 0x040009FA RID: 2554
			public Vector3 Position;

			// Token: 0x040009FB RID: 2555
			public float Indices;

			// Token: 0x040009FC RID: 2556
			public Vector3 NextPosition;

			// Token: 0x040009FD RID: 2557
			public float NextIndices;

			// Token: 0x040009FE RID: 2558
			public Vector3 Offset;

			// Token: 0x040009FF RID: 2559
			public Color Color;

			// Token: 0x04000A00 RID: 2560
			public static readonly VertexElement[] VertexElements = new VertexElement[]
			{
				new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
				new VertexElement(0, 12, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.BlendIndices, 0),
				new VertexElement(0, 16, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 1),
				new VertexElement(0, 28, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.BlendIndices, 1),
				new VertexElement(0, 32, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Normal, 0),
				new VertexElement(0, 44, VertexElementFormat.Color, VertexElementMethod.Default, VertexElementUsage.Color, 0)
			};
		}
	}
}
