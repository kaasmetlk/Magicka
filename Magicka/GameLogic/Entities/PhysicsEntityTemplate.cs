using System;
using System.Collections.Generic;
using JigLibX.Geometry;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead.Effects;
using XNAnimation;

namespace Magicka.GameLogic.Entities
{
	// Token: 0x020003AE RID: 942
	public class PhysicsEntityTemplate
	{
		// Token: 0x17000712 RID: 1810
		// (get) Token: 0x06001CEE RID: 7406 RVA: 0x000CD983 File Offset: 0x000CBB83
		public int ID
		{
			get
			{
				return this.mIDHash;
			}
		}

		// Token: 0x06001CEF RID: 7407 RVA: 0x000CD98B File Offset: 0x000CBB8B
		private PhysicsEntityTemplate()
		{
		}

		// Token: 0x06001CF0 RID: 7408 RVA: 0x000CD9A0 File Offset: 0x000CBBA0
		public static PhysicsEntityTemplate Read(ContentReader iInput)
		{
			PhysicsEntityTemplate physicsEntityTemplate = new PhysicsEntityTemplate();
			string text = iInput.AssetName;
			int num = text.IndexOf("content", StringComparison.OrdinalIgnoreCase);
			text = text.Substring(num + "content".Length + 1);
			physicsEntityTemplate.mPath = text.ToLowerInvariant();
			physicsEntityTemplate.mMovable = iInput.ReadBoolean();
			physicsEntityTemplate.mPushable = iInput.ReadBoolean();
			physicsEntityTemplate.mSolid = iInput.ReadBoolean();
			physicsEntityTemplate.mMass = iInput.ReadSingle();
			physicsEntityTemplate.mMaxHitPoints = iInput.ReadInt32();
			physicsEntityTemplate.mCanHaveStatus = iInput.ReadBoolean();
			GraphicsDevice graphicsDevice = (iInput.ContentManager.ServiceProvider.GetService(typeof(IGraphicsDeviceManager)) as GraphicsDeviceManager).GraphicsDevice;
			int num2 = iInput.ReadInt32();
			physicsEntityTemplate.mResistances = new Resistance[11];
			for (int i = 0; i < physicsEntityTemplate.mResistances.Length; i++)
			{
				physicsEntityTemplate.mResistances[i].ResistanceAgainst = Defines.ElementFromIndex(i);
				physicsEntityTemplate.mResistances[i].Multiplier = 1f;
				physicsEntityTemplate.mResistances[i].Modifier = 0f;
			}
			for (int j = 0; j < num2; j++)
			{
				Elements elements = (Elements)iInput.ReadInt32();
				int num3 = Defines.ElementIndex(elements);
				physicsEntityTemplate.mResistances[num3].ResistanceAgainst = elements;
				physicsEntityTemplate.mResistances[num3].Multiplier = iInput.ReadSingle();
				physicsEntityTemplate.mResistances[num3].Modifier = iInput.ReadSingle();
			}
			num2 = iInput.ReadInt32();
			physicsEntityTemplate.mGibs = new GibReference[num2];
			for (int k = 0; k < num2; k++)
			{
				GibReference gibReference;
				lock (graphicsDevice)
				{
					gibReference.mModel = iInput.ReadExternalReference<Model>();
				}
				gibReference.mMass = iInput.ReadSingle();
				gibReference.mScale = iInput.ReadSingle();
				physicsEntityTemplate.mGibs[k] = gibReference;
			}
			string text2 = iInput.ReadString();
			if (!string.IsNullOrEmpty(text2))
			{
				physicsEntityTemplate.mHitEffect = text2.ToLowerInvariant().GetHashCodeCustom();
			}
			else
			{
				physicsEntityTemplate.mHitEffect = 0;
			}
			text2 = iInput.ReadString();
			if (string.IsNullOrEmpty(text2))
			{
				physicsEntityTemplate.mHitSoundBank = Banks.Weapons;
				physicsEntityTemplate.mHitSound = 0;
			}
			else
			{
				string[] array = text2.Split(new char[]
				{
					'/'
				});
				if (array != null && array.Length > 1)
				{
					physicsEntityTemplate.mHitSoundBank = (Banks)Enum.Parse(typeof(Banks), array[0], true);
					physicsEntityTemplate.mHitSound = array[1].ToLowerInvariant().GetHashCodeCustom();
				}
				else
				{
					physicsEntityTemplate.mHitSoundBank = Banks.Weapons;
					physicsEntityTemplate.mHitSound = text2.ToLowerInvariant().GetHashCodeCustom();
				}
			}
			string text3 = iInput.ReadString();
			if (!string.IsNullOrEmpty(text3))
			{
				physicsEntityTemplate.mGibTrailEffect = text3.ToLowerInvariant().GetHashCodeCustom();
			}
			else
			{
				physicsEntityTemplate.mGibTrailEffect = 0;
			}
			lock (graphicsDevice)
			{
				physicsEntityTemplate.mModel = iInput.ReadObject<Model>();
			}
			if (iInput.ReadBoolean())
			{
				physicsEntityTemplate.mMeshVertices = iInput.ReadObject<List<Vector3>>();
				num2 = iInput.ReadInt32();
				physicsEntityTemplate.mMeshIndices = new List<TriangleVertexIndices>(num2);
				for (int l = 0; l < num2; l++)
				{
					TriangleVertexIndices item;
					item.I0 = iInput.ReadInt32();
					item.I1 = iInput.ReadInt32();
					item.I2 = iInput.ReadInt32();
					physicsEntityTemplate.mMeshIndices.Add(item);
				}
			}
			num2 = iInput.ReadInt32();
			if (num2 > 0)
			{
				iInput.ReadString();
				physicsEntityTemplate.mBox.Positon = iInput.ReadVector3();
				physicsEntityTemplate.mBox.Sides = iInput.ReadVector3();
				physicsEntityTemplate.mBox.Orientation = iInput.ReadQuaternion();
				if (num2 > 1)
				{
					for (int m = 1; m < num2; m++)
					{
						iInput.ReadString();
						iInput.ReadVector3();
						iInput.ReadVector3();
						iInput.ReadQuaternion();
					}
				}
			}
			int num4 = iInput.ReadInt32();
			physicsEntityTemplate.mEffects = new PhysicsEntity.VisualEffectStorage[num4];
			for (int n = 0; n < num4; n++)
			{
				physicsEntityTemplate.mEffects[n].EffectHash = iInput.ReadString().GetHashCodeCustom();
				physicsEntityTemplate.mEffects[n].Transform = iInput.ReadMatrix();
			}
			num4 = iInput.ReadInt32();
			if (num4 > 0)
			{
				throw new NotImplementedException("Lights");
			}
			physicsEntityTemplate.Conditions = new ConditionCollection(iInput);
			for (int num5 = 0; num5 < physicsEntityTemplate.Conditions.Count; num5++)
			{
				EventCollection eventCollection = physicsEntityTemplate.Conditions[num5];
				if (eventCollection != null && (byte)(eventCollection.Condition.EventConditionType & (EventConditionType.Hit | EventConditionType.Collision)) != 0)
				{
					for (int num6 = 0; num6 < eventCollection.Count; num6++)
					{
						EventStorage value = eventCollection[num5];
						if (value.EventType == EventType.Damage)
						{
							value.DamageEvent.Damage.Magnitude = value.DamageEvent.Damage.Magnitude * 0.25f;
							eventCollection[num5] = value;
						}
					}
				}
			}
			physicsEntityTemplate.mID = iInput.ReadString().ToLowerInvariant();
			physicsEntityTemplate.mIDHash = physicsEntityTemplate.mID.GetHashCodeCustom();
			bool flag = false;
			try
			{
				flag = iInput.ReadBoolean();
			}
			catch
			{
				flag = false;
			}
			if (flag)
			{
				physicsEntityTemplate.mRadius = iInput.ReadSingle();
				SkinnedModel skinnedModel = null;
				int num7 = iInput.ReadInt32();
				if (num7 == 0)
				{
					physicsEntityTemplate.mModels = null;
				}
				else
				{
					physicsEntityTemplate.mModels = new ModelProperties[num7];
					lock (graphicsDevice)
					{
						for (int num8 = 0; num8 < num7; num8++)
						{
							ModelProperties modelProperties;
							modelProperties.Model = iInput.ReadExternalReference<SkinnedModel>();
							modelProperties.Scale = iInput.ReadSingle();
							Matrix.CreateScale(modelProperties.Scale, out modelProperties.Transform);
							modelProperties.Tint = iInput.ReadVector3();
							physicsEntityTemplate.mModels[num8] = modelProperties;
						}
					}
				}
				bool flag2 = iInput.ReadBoolean();
				if (flag2)
				{
					lock (graphicsDevice)
					{
						skinnedModel = iInput.ReadExternalReference<SkinnedModel>();
					}
				}
				if (skinnedModel == null || skinnedModel.SkeletonBones == null)
				{
					physicsEntityTemplate.mSkeleton = null;
				}
				else
				{
					physicsEntityTemplate.mSkeleton = skinnedModel.SkeletonBones;
				}
				int num9 = iInput.ReadInt32();
				if (num9 == 0)
				{
					physicsEntityTemplate.mAttachedEffects = null;
				}
				else
				{
					physicsEntityTemplate.mAttachedEffects = new KeyValuePair<int, int>[num9];
					for (int num10 = 0; num10 < num9; num10++)
					{
						string text4 = iInput.ReadString().ToLowerInvariant();
						string iString = iInput.ReadString().ToLowerInvariant();
						int hashCodeCustom = iString.GetHashCodeCustom();
						int num11 = -1;
						if (skinnedModel != null)
						{
							for (int num12 = 0; num12 < skinnedModel.SkeletonBones.Count; num12++)
							{
								SkinnedModelBone skinnedModelBone = skinnedModel.SkeletonBones[num12];
								if (skinnedModelBone.Name.Equals(text4, StringComparison.OrdinalIgnoreCase))
								{
									num11 = (int)skinnedModelBone.Index;
								}
							}
						}
						if (num11 < 0)
						{
							throw new Exception(string.Format("Bone \"{0}\" not found!", text4));
						}
						physicsEntityTemplate.mAttachedEffects[num10] = new KeyValuePair<int, int>(num11, hashCodeCustom);
					}
				}
				physicsEntityTemplate.mAnimationClips = new AnimationClipAction[27][];
				for (int num13 = 0; num13 < physicsEntityTemplate.mAnimationClips.Length; num13++)
				{
					int num14 = iInput.ReadInt32();
					if (num14 > 0)
					{
						physicsEntityTemplate.mAnimationClips[num13] = new AnimationClipAction[231];
						for (int num15 = 0; num15 < num14; num15++)
						{
							string value2 = iInput.ReadString();
							Animations animations = (Animations)Enum.Parse(typeof(Animations), value2, true);
							physicsEntityTemplate.mAnimationClips[num13][(int)animations] = new AnimationClipAction(animations, iInput, skinnedModel.AnimationClips, physicsEntityTemplate.mSkeleton);
						}
					}
				}
				if (PhysicsEntityTemplate.sSkeletonVertexDeclaration == null)
				{
					lock (graphicsDevice)
					{
						PhysicsEntityTemplate.sSkeletonVertexDeclaration = new VertexDeclaration(graphicsDevice, PhysicsEntityTemplate.VertexPositionIndexWeight.VertexElements);
					}
				}
				if (physicsEntityTemplate.mModels != null && physicsEntityTemplate.mModels.Length > 0)
				{
					PhysicsEntityTemplate.GenerateSkeletonModel(physicsEntityTemplate.mModels[0].Model.SkeletonBones, 0.125f * physicsEntityTemplate.mRadius / physicsEntityTemplate.mModels[0].Scale, out physicsEntityTemplate.mSkeletonVertices, out physicsEntityTemplate.mSkeletonPrimitiveCount);
				}
			}
			return physicsEntityTemplate;
		}

		// Token: 0x17000713 RID: 1811
		// (get) Token: 0x06001CF1 RID: 7409 RVA: 0x000CE21C File Offset: 0x000CC41C
		public string Path
		{
			get
			{
				return this.mPath;
			}
		}

		// Token: 0x17000714 RID: 1812
		// (get) Token: 0x06001CF2 RID: 7410 RVA: 0x000CE224 File Offset: 0x000CC424
		public bool Movable
		{
			get
			{
				return this.mMovable;
			}
		}

		// Token: 0x17000715 RID: 1813
		// (get) Token: 0x06001CF3 RID: 7411 RVA: 0x000CE22C File Offset: 0x000CC42C
		public bool Pushable
		{
			get
			{
				return this.mPushable;
			}
		}

		// Token: 0x17000716 RID: 1814
		// (get) Token: 0x06001CF4 RID: 7412 RVA: 0x000CE234 File Offset: 0x000CC434
		public bool Solid
		{
			get
			{
				return this.mSolid;
			}
		}

		// Token: 0x17000717 RID: 1815
		// (get) Token: 0x06001CF5 RID: 7413 RVA: 0x000CE23C File Offset: 0x000CC43C
		public float Mass
		{
			get
			{
				return this.mMass;
			}
		}

		// Token: 0x17000718 RID: 1816
		// (get) Token: 0x06001CF6 RID: 7414 RVA: 0x000CE244 File Offset: 0x000CC444
		public bool CanHaveStatus
		{
			get
			{
				return this.mCanHaveStatus;
			}
		}

		// Token: 0x17000719 RID: 1817
		// (get) Token: 0x06001CF7 RID: 7415 RVA: 0x000CE24C File Offset: 0x000CC44C
		public int MaxHitpoints
		{
			get
			{
				return this.mMaxHitPoints;
			}
		}

		// Token: 0x1700071A RID: 1818
		// (get) Token: 0x06001CF8 RID: 7416 RVA: 0x000CE254 File Offset: 0x000CC454
		public GibReference[] Gibs
		{
			get
			{
				return this.mGibs;
			}
		}

		// Token: 0x1700071B RID: 1819
		// (get) Token: 0x06001CF9 RID: 7417 RVA: 0x000CE25C File Offset: 0x000CC45C
		public PhysicsEntity.VisualEffectStorage[] Effects
		{
			get
			{
				return this.mEffects;
			}
		}

		// Token: 0x1700071C RID: 1820
		// (get) Token: 0x06001CFA RID: 7418 RVA: 0x000CE264 File Offset: 0x000CC464
		public Resistance[] Resistances
		{
			get
			{
				return this.mResistances;
			}
		}

		// Token: 0x1700071D RID: 1821
		// (get) Token: 0x06001CFB RID: 7419 RVA: 0x000CE26C File Offset: 0x000CC46C
		public PhysicsEntityTemplate.BoxInfo Box
		{
			get
			{
				return this.mBox;
			}
		}

		// Token: 0x1700071E RID: 1822
		// (get) Token: 0x06001CFC RID: 7420 RVA: 0x000CE274 File Offset: 0x000CC474
		public int HitSound
		{
			get
			{
				return this.mHitSound;
			}
		}

		// Token: 0x1700071F RID: 1823
		// (get) Token: 0x06001CFD RID: 7421 RVA: 0x000CE27C File Offset: 0x000CC47C
		public int HitEffect
		{
			get
			{
				return this.mHitEffect;
			}
		}

		// Token: 0x17000720 RID: 1824
		// (get) Token: 0x06001CFE RID: 7422 RVA: 0x000CE284 File Offset: 0x000CC484
		public int GibTrailEffect
		{
			get
			{
				return this.mGibTrailEffect;
			}
		}

		// Token: 0x17000721 RID: 1825
		// (get) Token: 0x06001CFF RID: 7423 RVA: 0x000CE28C File Offset: 0x000CC48C
		public int VertexCount
		{
			get
			{
				if (this.mModel.Meshes.Count > 0)
				{
					return this.mModel.Meshes[0].MeshParts[0].NumVertices;
				}
				return 0;
			}
		}

		// Token: 0x17000722 RID: 1826
		// (get) Token: 0x06001D00 RID: 7424 RVA: 0x000CE2C4 File Offset: 0x000CC4C4
		public int VertexStride
		{
			get
			{
				if (this.mModel.Meshes.Count > 0)
				{
					return this.mModel.Meshes[0].MeshParts[0].VertexStride;
				}
				return 0;
			}
		}

		// Token: 0x17000723 RID: 1827
		// (get) Token: 0x06001D01 RID: 7425 RVA: 0x000CE2FC File Offset: 0x000CC4FC
		public int PrimitiveCount
		{
			get
			{
				if (this.mModel.Meshes.Count > 0)
				{
					return this.mModel.Meshes[0].MeshParts[0].PrimitiveCount;
				}
				return 0;
			}
		}

		// Token: 0x17000724 RID: 1828
		// (get) Token: 0x06001D02 RID: 7426 RVA: 0x000CE334 File Offset: 0x000CC534
		public VertexBuffer Vertices
		{
			get
			{
				if (this.mModel.Meshes.Count > 0)
				{
					return this.mModel.Meshes[0].VertexBuffer;
				}
				return null;
			}
		}

		// Token: 0x17000725 RID: 1829
		// (get) Token: 0x06001D03 RID: 7427 RVA: 0x000CE361 File Offset: 0x000CC561
		public VertexDeclaration VertexDeclaration
		{
			get
			{
				if (this.mModel.Meshes.Count > 0)
				{
					return this.mModel.Meshes[0].MeshParts[0].VertexDeclaration;
				}
				return null;
			}
		}

		// Token: 0x17000726 RID: 1830
		// (get) Token: 0x06001D04 RID: 7428 RVA: 0x000CE399 File Offset: 0x000CC599
		public IndexBuffer Indices
		{
			get
			{
				if (this.mModel.Meshes.Count > 0)
				{
					return this.mModel.Meshes[0].IndexBuffer;
				}
				return null;
			}
		}

		// Token: 0x17000727 RID: 1831
		// (get) Token: 0x06001D05 RID: 7429 RVA: 0x000CE3C6 File Offset: 0x000CC5C6
		// (set) Token: 0x06001D06 RID: 7430 RVA: 0x000CE3CE File Offset: 0x000CC5CE
		public ConditionCollection Conditions
		{
			get
			{
				return this.mConditions;
			}
			set
			{
				this.mConditions = value;
			}
		}

		// Token: 0x17000728 RID: 1832
		// (get) Token: 0x06001D07 RID: 7431 RVA: 0x000CE3D8 File Offset: 0x000CC5D8
		public RenderDeferredMaterial Material
		{
			get
			{
				RenderDeferredMaterial result = default(RenderDeferredMaterial);
				if (this.mModel.Meshes.Count > 0)
				{
					result.FetchFromEffect(this.mModel.Meshes[0].MeshParts[0].Effect as RenderDeferredEffect);
				}
				return result;
			}
		}

		// Token: 0x17000729 RID: 1833
		// (get) Token: 0x06001D08 RID: 7432 RVA: 0x000CE42E File Offset: 0x000CC62E
		public ModelProperties[] Models
		{
			get
			{
				return this.mModels;
			}
		}

		// Token: 0x1700072A RID: 1834
		// (get) Token: 0x06001D09 RID: 7433 RVA: 0x000CE436 File Offset: 0x000CC636
		public SkinnedModelBoneCollection Skeleton
		{
			get
			{
				return this.mSkeleton;
			}
		}

		// Token: 0x1700072B RID: 1835
		// (get) Token: 0x06001D0A RID: 7434 RVA: 0x000CE43E File Offset: 0x000CC63E
		public VertexDeclaration SkeletonVertexDeclaration
		{
			get
			{
				return PhysicsEntityTemplate.sSkeletonVertexDeclaration;
			}
		}

		// Token: 0x1700072C RID: 1836
		// (get) Token: 0x06001D0B RID: 7435 RVA: 0x000CE445 File Offset: 0x000CC645
		public VertexBuffer SkeletonVertices
		{
			get
			{
				return this.mSkeletonVertices;
			}
		}

		// Token: 0x1700072D RID: 1837
		// (get) Token: 0x06001D0C RID: 7436 RVA: 0x000CE44D File Offset: 0x000CC64D
		public int SkeletonPrimitiveCount
		{
			get
			{
				return this.mSkeletonPrimitiveCount;
			}
		}

		// Token: 0x1700072E RID: 1838
		// (get) Token: 0x06001D0D RID: 7437 RVA: 0x000CE455 File Offset: 0x000CC655
		public int SkeletonVertexStride
		{
			get
			{
				return 48;
			}
		}

		// Token: 0x1700072F RID: 1839
		// (get) Token: 0x06001D0E RID: 7438 RVA: 0x000CE459 File Offset: 0x000CC659
		public AnimationClipAction[][] AnimationClips
		{
			get
			{
				return this.mAnimationClips;
			}
		}

		// Token: 0x06001D0F RID: 7439 RVA: 0x000CE464 File Offset: 0x000CC664
		private static void GenerateSkeletonModel(SkinnedModelBoneCollection iSkeleton, float iScale, out VertexBuffer oVertexBuffer, out int oPrimitiveCount)
		{
			List<PhysicsEntityTemplate.VertexPositionIndexWeight> list = new List<PhysicsEntityTemplate.VertexPositionIndexWeight>();
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
				PhysicsEntityTemplate.GenerateBall(list, inverseBindPoseTransform, (float)i, iRadius);
				if (i > 1)
				{
					PhysicsEntityTemplate.GeneratePyramid(list, skinnedModelBone, iRadius);
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
				oVertexBuffer.SetData<PhysicsEntityTemplate.VertexPositionIndexWeight>(list.ToArray());
			}
			oPrimitiveCount = list.Count - 1;
		}

		// Token: 0x06001D10 RID: 7440 RVA: 0x000CE56C File Offset: 0x000CC76C
		private static void GenerateBall(List<PhysicsEntityTemplate.VertexPositionIndexWeight> iVertices, Matrix iTransform, float iBoneIndex, float iRadius)
		{
			Vector3 vector = default(Vector3);
			vector.Z = iRadius;
			PhysicsEntityTemplate.VertexPositionIndexWeight item = default(PhysicsEntityTemplate.VertexPositionIndexWeight);
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

		// Token: 0x06001D11 RID: 7441 RVA: 0x000CE6B8 File Offset: 0x000CC8B8
		private static void GeneratePyramid(List<PhysicsEntityTemplate.VertexPositionIndexWeight> iVertices, SkinnedModelBone iChild, float iRadius)
		{
			Matrix inverseBindPoseTransform = iChild.InverseBindPoseTransform;
			Matrix.Invert(ref inverseBindPoseTransform, out inverseBindPoseTransform);
			Matrix inverseBindPoseTransform2 = iChild.Parent.InverseBindPoseTransform;
			Matrix.Invert(ref inverseBindPoseTransform2, out inverseBindPoseTransform2);
			PhysicsEntityTemplate.VertexPositionIndexWeight item = default(PhysicsEntityTemplate.VertexPositionIndexWeight);
			item.Offset.Z = -iRadius;
			item.Position = inverseBindPoseTransform.Translation;
			item.Indices = (float)iChild.Index;
			item.NextPosition = inverseBindPoseTransform2.Translation;
			item.NextIndices = (float)iChild.Parent.Index;
			item.Color = Color.White;
			PhysicsEntityTemplate.VertexPositionIndexWeight item2 = default(PhysicsEntityTemplate.VertexPositionIndexWeight);
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

		// Token: 0x04001F8F RID: 8079
		private string mPath;

		// Token: 0x04001F90 RID: 8080
		private bool mMovable;

		// Token: 0x04001F91 RID: 8081
		private bool mPushable;

		// Token: 0x04001F92 RID: 8082
		private bool mSolid;

		// Token: 0x04001F93 RID: 8083
		private float mMass;

		// Token: 0x04001F94 RID: 8084
		private bool mCanHaveStatus;

		// Token: 0x04001F95 RID: 8085
		private int mMaxHitPoints;

		// Token: 0x04001F96 RID: 8086
		private int mHitSound;

		// Token: 0x04001F97 RID: 8087
		private Banks mHitSoundBank;

		// Token: 0x04001F98 RID: 8088
		private int mHitEffect;

		// Token: 0x04001F99 RID: 8089
		private int mGibTrailEffect;

		// Token: 0x04001F9A RID: 8090
		private ConditionCollection mConditions;

		// Token: 0x04001F9B RID: 8091
		private List<Vector3> mMeshVertices;

		// Token: 0x04001F9C RID: 8092
		private List<TriangleVertexIndices> mMeshIndices;

		// Token: 0x04001F9D RID: 8093
		private PhysicsEntityTemplate.BoxInfo mBox;

		// Token: 0x04001F9E RID: 8094
		private PhysicsEntity.VisualEffectStorage[] mEffects;

		// Token: 0x04001F9F RID: 8095
		private Model mModel;

		// Token: 0x04001FA0 RID: 8096
		private Resistance[] mResistances;

		// Token: 0x04001FA1 RID: 8097
		private GibReference[] mGibs;

		// Token: 0x04001FA2 RID: 8098
		private ModelProperties[] mModels;

		// Token: 0x04001FA3 RID: 8099
		private SkinnedModelBoneCollection mSkeleton;

		// Token: 0x04001FA4 RID: 8100
		private AnimationClipAction[][] mAnimationClips;

		// Token: 0x04001FA5 RID: 8101
		private KeyValuePair<int, int>[] mAttachedEffects;

		// Token: 0x04001FA6 RID: 8102
		private static VertexDeclaration sSkeletonVertexDeclaration;

		// Token: 0x04001FA7 RID: 8103
		private VertexBuffer mSkeletonVertices;

		// Token: 0x04001FA8 RID: 8104
		private int mSkeletonPrimitiveCount;

		// Token: 0x04001FA9 RID: 8105
		public float mRadius = 1f;

		// Token: 0x04001FAA RID: 8106
		private string mID;

		// Token: 0x04001FAB RID: 8107
		private int mIDHash;

		// Token: 0x020003AF RID: 943
		public struct BoxInfo
		{
			// Token: 0x04001FAC RID: 8108
			public Vector3 Sides;

			// Token: 0x04001FAD RID: 8109
			public Vector3 Positon;

			// Token: 0x04001FAE RID: 8110
			public Quaternion Orientation;
		}

		// Token: 0x020003B0 RID: 944
		private struct VertexPositionIndexWeight
		{
			// Token: 0x04001FAF RID: 8111
			public const int SIZEINBYTES = 48;

			// Token: 0x04001FB0 RID: 8112
			public Vector3 Position;

			// Token: 0x04001FB1 RID: 8113
			public float Indices;

			// Token: 0x04001FB2 RID: 8114
			public Vector3 NextPosition;

			// Token: 0x04001FB3 RID: 8115
			public float NextIndices;

			// Token: 0x04001FB4 RID: 8116
			public Vector3 Offset;

			// Token: 0x04001FB5 RID: 8117
			public Color Color;

			// Token: 0x04001FB6 RID: 8118
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
