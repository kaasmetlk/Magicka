using System;
using System.Collections.Generic;
using System.IO;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using JigLibX.Physics;
using Magicka.GameLogic.Entities;
using Magicka.Graphics;
using Magicka.Network;
using Magicka.PathFinding;
using Magicka.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using PolygonHead.Lights;
using XNAnimation;
using XNAnimation.Controllers;

namespace Magicka.Levels
{
	// Token: 0x020002D0 RID: 720
	public class AnimatedLevelPart : IDisposable
	{
		// Token: 0x060015FF RID: 5631 RVA: 0x0008B7F4 File Offset: 0x000899F4
		public static AnimatedLevelPart GetFromHandle(int iHandle)
		{
			if (iHandle >= AnimatedLevelPart.mInstances.Count)
			{
				return null;
			}
			return AnimatedLevelPart.mInstances[iHandle];
		}

		// Token: 0x06001600 RID: 5632 RVA: 0x0008B810 File Offset: 0x00089A10
		public static void ClearHandles()
		{
			AnimatedLevelPart.mInstances.Clear();
		}

		// Token: 0x06001601 RID: 5633 RVA: 0x0008B81C File Offset: 0x00089A1C
		public AnimatedLevelPart(ContentReader iInput, LevelModel iLevel)
		{
			lock (AnimatedLevelPart.mInstances)
			{
				this.mHandle = (ushort)AnimatedLevelPart.mInstances.Count;
				AnimatedLevelPart.mInstances.Add(this);
			}
			this.mLevel = iLevel;
			this.mName = iInput.ReadString().ToLowerInvariant();
			this.mID = this.mName.GetHashCodeCustom();
			this.mAffectShields = iInput.ReadBoolean();
			this.mModel = iInput.ReadObject<Model>();
			int num = iInput.ReadInt32();
			this.mMeshSettings = new Dictionary<string, KeyValuePair<bool, bool>>(num);
			for (int i = 0; i < num; i++)
			{
				string key = iInput.ReadString();
				bool key2 = iInput.ReadBoolean();
				bool value = iInput.ReadBoolean();
				this.mMeshSettings.Add(key, new KeyValuePair<bool, bool>(key2, value));
			}
			num = iInput.ReadInt32();
			this.mLiquids = new Liquid[num];
			for (int j = 0; j < num; j++)
			{
				this.mLiquids[j] = Liquid.Read(iInput, iLevel, this);
				if (this.mLiquids[j].CollisionSkin != null)
				{
					this.mLiquids[j].CollisionSkin.postCollisionCallbackFn += this.PostCollision;
				}
			}
			num = iInput.ReadInt32();
			for (int k = 0; k < num; k++)
			{
				string text = iInput.ReadString();
				int hashCodeCustom = text.GetHashCodeCustom();
				Locator value2 = new Locator(text, iInput);
				this.mLocators.Add(hashCodeCustom, value2);
			}
			this.mAnimationDuration = iInput.ReadSingle();
			this.mStart = 0f;
			this.mEnd = this.mAnimationDuration;
			this.mAnimation = AnimationChannel.Read(iInput);
			num = iInput.ReadInt32();
			this.mEffects = new LevelModel.VisualEffectStorage[num];
			Vector3 up = Vector3.Up;
			for (int l = 0; l < num; l++)
			{
				string iString = iInput.ReadString().ToLowerInvariant();
				LevelModel.VisualEffectStorage visualEffectStorage;
				visualEffectStorage.ID = iString.GetHashCodeCustom();
				Vector3 vector = iInput.ReadVector3();
				Vector3 vector2 = iInput.ReadVector3();
				visualEffectStorage.Range = iInput.ReadSingle();
				string iString2 = iInput.ReadString().ToLowerInvariant();
				visualEffectStorage.Effect = iString2.GetHashCodeCustom();
				Matrix.CreateWorld(ref vector, ref vector2, ref up, out visualEffectStorage.Transform);
				this.mEffects[l] = visualEffectStorage;
			}
			num = iInput.ReadInt32();
			this.mLights = new int[num];
			this.mLightPositions = new Matrix[num];
			for (int m = 0; m < num; m++)
			{
				this.mLights[m] = iInput.ReadString().GetHashCodeCustom();
				this.mLightPositions[m] = iInput.ReadMatrix();
			}
			if (iInput.ReadBoolean())
			{
				this.mCollisionMaterial = (CollisionMaterials)iInput.ReadByte();
				List<Vector3> vertices = iInput.ReadObject<List<Vector3>>();
				num = iInput.ReadInt32();
				List<TriangleVertexIndices> list = new List<TriangleVertexIndices>(num);
				for (int n = 0; n < num; n++)
				{
					TriangleVertexIndices item;
					item.I0 = iInput.ReadInt32();
					item.I1 = iInput.ReadInt32();
					item.I2 = iInput.ReadInt32();
					list.Add(item);
				}
				TriangleMesh triangleMesh = new TriangleMesh();
				triangleMesh.CreateMesh(vertices, list, 16, 2f);
				this.mCollisionSkin = new CollisionSkin(null);
				this.mCollisionSkin.AddPrimitive(triangleMesh, 1, new MaterialProperties(0.1f, 1f, 1f));
				this.mCollisionSkin.ApplyLocalTransform(Transform.Identity);
				this.mCollisionSkin.Tag = iLevel;
				this.mCollisionSkin.postCollisionCallbackFn += this.PostCollision;
			}
			if (iInput.ReadBoolean())
			{
				this.mNavMesh = new AnimatedNavMesh(iInput);
			}
			num = iInput.ReadInt32();
			this.mChildren = new Dictionary<int, AnimatedLevelPart>(num);
			for (int num2 = 0; num2 < num; num2++)
			{
				AnimatedLevelPart animatedLevelPart = new AnimatedLevelPart(iInput, iLevel);
				this.mChildren.Add(animatedLevelPart.mName.GetHashCodeCustom(), animatedLevelPart);
			}
		}

		// Token: 0x17000598 RID: 1432
		// (get) Token: 0x06001602 RID: 5634 RVA: 0x0008BC60 File Offset: 0x00089E60
		public CollisionMaterials CollisionMaterial
		{
			get
			{
				return this.mCollisionMaterial;
			}
		}

		// Token: 0x17000599 RID: 1433
		// (get) Token: 0x06001603 RID: 5635 RVA: 0x0008BC68 File Offset: 0x00089E68
		public ushort Handle
		{
			get
			{
				return this.mHandle;
			}
		}

		// Token: 0x06001604 RID: 5636 RVA: 0x0008BC70 File Offset: 0x00089E70
		private void PostCollision(ref CollisionInfo iInfo)
		{
			if (!this.mPlaying)
			{
				return;
			}
			CollisionSkin collisionSkin = iInfo.SkinInfo.Skin0;
			float num = iInfo.DirToBody0.Y;
			if (collisionSkin == this.mCollisionSkin)
			{
				collisionSkin = iInfo.SkinInfo.Skin1;
				num = -num;
			}
			if (collisionSkin.Owner != null)
			{
				Entity entity = collisionSkin.Owner.Tag as Entity;
				if ((entity != null && entity is Shield) || num > 0.7f)
				{
					float val;
					if (!this.mCollidingEntities.TryGetValue(entity.Handle, out val))
					{
						val = 0f;
					}
					this.mCollidingEntities[entity.Handle] = Math.Max(0.25f, val);
					collisionSkin.Owner.SetActive();
				}
			}
		}

		// Token: 0x06001605 RID: 5637 RVA: 0x0008BD28 File Offset: 0x00089F28
		public void Initialize(ref Matrix iParent)
		{
			this.GetTransform(out this.mOldTransform);
			Matrix.Multiply(ref iParent, ref this.mOldTransform, out this.mOldTransform);
			if (this.mCollisionSkin != null)
			{
				Transform transform;
				transform.Orientation = this.mOldTransform;
				transform.Orientation.M41 = 0f;
				transform.Orientation.M42 = 0f;
				transform.Orientation.M43 = 0f;
				transform.Position = this.mOldTransform.Translation;
				this.mCollisionSkin.SetTransform(ref transform, ref transform);
			}
			this.mAdditiveRenderData = new RenderableAdditiveObject<AdditiveEffect, AdditiveMaterial>[3][];
			this.mDefaultRenderData = new AnimatedLevelPart.DeferredRenderData[3][];
			this.mHighlightRenderData = new AnimatedLevelPart.HighlightRenderData[3][];
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			this.mHighlighted = -1f;
			if (this.mNavMesh != null)
			{
				this.mNavMesh.UpdateTransform(ref this.mOldTransform);
				this.mLevel.NavMesh.AnimatedParts.Add(this.mNavMesh);
			}
			for (int i = 0; i < this.mModel.Meshes.Count; i++)
			{
				ModelMesh modelMesh = this.mModel.Meshes[i];
				for (int j = 0; j < modelMesh.MeshParts.Count; j++)
				{
					ModelMeshPart modelMeshPart = modelMesh.MeshParts[j];
					if (modelMeshPart.Effect is AdditiveEffect)
					{
						num++;
					}
					else if (modelMeshPart.Effect is RenderDeferredEffect)
					{
						num2++;
					}
					else
					{
						if (!(modelMeshPart.Effect is RenderDeferredLiquidEffect))
						{
							throw new Exception("Invalid effect type!");
						}
						num3++;
					}
				}
				if (i == 0)
				{
					this.mBoundingSphere = modelMesh.BoundingSphere;
				}
				else
				{
					this.mBoundingSphere = BoundingSphere.CreateMerged(this.mBoundingSphere, modelMesh.BoundingSphere);
				}
			}
			for (int k = 0; k < 3; k++)
			{
				RenderableAdditiveObject<AdditiveEffect, AdditiveMaterial>[] array = new RenderableAdditiveObject<AdditiveEffect, AdditiveMaterial>[num];
				AnimatedLevelPart.DeferredRenderData[] array2 = new AnimatedLevelPart.DeferredRenderData[num2];
				AnimatedLevelPart.HighlightRenderData[] array3 = new AnimatedLevelPart.HighlightRenderData[num2];
				this.mAdditiveRenderData[k] = array;
				this.mDefaultRenderData[k] = array2;
				this.mHighlightRenderData[k] = array3;
				int num4 = 0;
				int num5 = 0;
				for (int l = 0; l < this.mModel.Meshes.Count; l++)
				{
					ModelMesh modelMesh2 = this.mModel.Meshes[l];
					KeyValuePair<bool, bool> keyValuePair = this.mMeshSettings[modelMesh2.Name];
					for (int m = 0; m < modelMesh2.MeshParts.Count; m++)
					{
						ModelMeshPart modelMeshPart2 = modelMesh2.MeshParts[m];
						AdditiveEffect additiveEffect = modelMeshPart2.Effect as AdditiveEffect;
						RenderDeferredEffect renderDeferredEffect = modelMeshPart2.Effect as RenderDeferredEffect;
						if (additiveEffect != null)
						{
							array[num4] = new RenderableAdditiveObject<AdditiveEffect, AdditiveMaterial>();
							array[num4++].SetMesh(modelMesh2, modelMeshPart2, 0);
						}
						else if (renderDeferredEffect != null)
						{
							array2[num5] = new AnimatedLevelPart.DeferredRenderData(keyValuePair.Key, keyValuePair.Value);
							array3[num5] = new AnimatedLevelPart.HighlightRenderData(keyValuePair.Key);
							array3[num5].SetMesh(modelMesh2.VertexBuffer, modelMesh2.IndexBuffer, modelMeshPart2, RenderDeferredEffect.TYPEHASH);
							VertexDeclaration vertexDeclaration = modelMeshPart2.VertexDeclaration;
							RenderDeferredEffect.Technique iTechnique;
							if (renderDeferredEffect.ReflectionMap != null)
							{
								if (renderDeferredEffect.DiffuseTexture1 != null)
								{
									iTechnique = RenderDeferredEffect.Technique.DualLayerReflection;
								}
								else
								{
									iTechnique = RenderDeferredEffect.Technique.SingleLayerReflection;
								}
							}
							else if (renderDeferredEffect.DiffuseTexture1 != null)
							{
								iTechnique = RenderDeferredEffect.Technique.DualLayer;
							}
							else
							{
								iTechnique = RenderDeferredEffect.Technique.SingleLayer;
							}
							array2[num5++].SetMesh(modelMesh2, modelMeshPart2, vertexDeclaration, 4, (int)iTechnique, 5);
						}
					}
				}
			}
			foreach (AnimatedLevelPart animatedLevelPart in this.mChildren.Values)
			{
				animatedLevelPart.Initialize(ref this.mOldTransform);
			}
		}

		// Token: 0x06001606 RID: 5638 RVA: 0x0008C104 File Offset: 0x0008A304
		public void Update(DataChannel iDataChannel, float iDeltaTime, ref Matrix iParent, GameScene iScene)
		{
			if (this.mPlaying)
			{
				this.mRestingTimer = 1f;
				float num = this.mTime;
				this.mTime += iDeltaTime * this.mSpeed;
				if (this.mSpeed > 0f)
				{
					if (num <= this.mEnd && this.mTime > this.mEnd)
					{
						if (this.mLooping)
						{
							this.mTime -= this.mEnd - this.mStart;
						}
						else
						{
							this.mTime = this.mEnd;
							this.mPlaying = false;
						}
					}
				}
				else if (num >= this.mEnd && this.mTime < this.mEnd)
				{
					if (this.mLooping)
					{
						this.mTime += this.mEnd - this.mStart;
					}
					else
					{
						this.mTime = this.mEnd;
						this.mPlaying = false;
					}
				}
			}
			Matrix matrix;
			this.GetTransform(out matrix);
			Matrix.Multiply(ref matrix, ref iParent, out matrix);
			for (int i = 0; i < this.mLights.Length; i++)
			{
				Matrix matrix2;
				Matrix.Multiply(ref this.mLightPositions[i], ref matrix, out matrix2);
				Light light = this.mLevel.Lights[this.mLights[i]];
				PointLight pointLight = light as PointLight;
				SpotLight spotLight = light as SpotLight;
				DirectionalLight directionalLight = light as DirectionalLight;
				if (pointLight != null)
				{
					pointLight.Position = matrix2.Translation;
				}
				else if (spotLight != null)
				{
					spotLight.Position = matrix2.Translation;
					spotLight.Direction = matrix2.Forward;
				}
				else if (directionalLight != null)
				{
					directionalLight.LightDirection = matrix2.Forward;
				}
			}
			if (this.mCollisionSkin != null)
			{
				Transform transform = this.mCollisionSkin.NewTransform;
				this.mCollisionSkin.SetOldTransform(ref transform);
				transform.Orientation = matrix;
				transform.Orientation.M41 = 0f;
				transform.Orientation.M42 = 0f;
				transform.Orientation.M43 = 0f;
				transform.Position = matrix.Translation;
				this.mCollisionSkin.SetNewTransform(ref transform);
				this.mCollisionSkin.UpdateWorldBoundingBox();
			}
			Matrix matrix3;
			Matrix.Invert(ref this.mOldTransform, out matrix3);
			Matrix.Multiply(ref matrix3, ref matrix, out matrix3);
			for (int j = 0; j < this.mCollidingEntities.Count; j++)
			{
				ushort num2 = this.mCollidingEntities.Keys[j];
				Entity fromHandle = Entity.GetFromHandle((int)num2);
				float num3 = this.mCollidingEntities[num2] - iDeltaTime;
				this.mCollidingEntities[num2] = num3;
				if (num3 <= 0f || fromHandle.Removable || (!this.mAffectShields && fromHandle is Shield))
				{
					this.mCollidingEntities.RemoveAt(j);
					j--;
				}
				Body body = fromHandle.Body;
				Transform transform = body.Transform;
				transform.Orientation.Translation = transform.Position;
				Matrix.Multiply(ref transform.Orientation, ref matrix3, out transform.Orientation);
				transform.Position = transform.Orientation.Translation;
				transform.Orientation.Translation = default(Vector3);
				CharacterBody characterBody = body as CharacterBody;
				if (characterBody != null)
				{
					Vector3 desiredDirection = characterBody.DesiredDirection;
					Vector3.TransformNormal(ref desiredDirection, ref matrix3, out desiredDirection);
					characterBody.DesiredDirection = desiredDirection;
				}
				body.Transform = transform;
			}
			for (int k = 0; k < this.mDecals.Count; k++)
			{
				DecalManager.DecalReference value = this.mDecals[k];
				if (!DecalManager.Instance.TransformDecal(ref value, ref matrix3))
				{
					this.mDecals.RemoveAt(k--);
				}
				else
				{
					this.mDecals[k] = value;
				}
			}
			if (this.mNavMesh != null & this.mOldTransform != matrix)
			{
				this.mNavMesh.UpdateTransform(ref matrix);
			}
			this.mOldTransform = matrix;
			if (iDataChannel != DataChannel.None)
			{
				RenderableAdditiveObject<AdditiveEffect, AdditiveMaterial>[] array = this.mAdditiveRenderData[(int)iDataChannel];
				RenderableObject<RenderDeferredEffect, RenderDeferredMaterial>[] array2 = this.mDefaultRenderData[(int)iDataChannel];
				AnimatedLevelPart.HighlightRenderData[] array3 = this.mHighlightRenderData[(int)iDataChannel];
				foreach (RenderableAdditiveObject<AdditiveEffect, AdditiveMaterial> renderableAdditiveObject in array)
				{
					renderableAdditiveObject.mBoundingSphere.Radius = this.mBoundingSphere.Radius;
					Vector3.Transform(ref this.mBoundingSphere.Center, ref matrix, out renderableAdditiveObject.mBoundingSphere.Center);
					renderableAdditiveObject.mMaterial.WorldTransform = matrix;
					iScene.PlayState.Scene.AddRenderableAdditiveObject(iDataChannel, renderableAdditiveObject);
				}
				foreach (RenderableObject<RenderDeferredEffect, RenderDeferredMaterial> renderableObject in array2)
				{
					renderableObject.mBoundingSphere.Radius = this.mBoundingSphere.Radius;
					Vector3.Transform(ref this.mBoundingSphere.Center, ref matrix, out renderableObject.mBoundingSphere.Center);
					renderableObject.mMaterial.WorldTransform = matrix;
					iScene.PlayState.Scene.AddRenderableObject(iDataChannel, renderableObject);
				}
				if (this.mHighlighted >= 0f)
				{
					foreach (AnimatedLevelPart.HighlightRenderData highlightRenderData in array3)
					{
						highlightRenderData.mBoundingSphere.Radius = this.mBoundingSphere.Radius;
						Vector3.Transform(ref this.mBoundingSphere.Center, ref matrix, out highlightRenderData.mBoundingSphere.Center);
						highlightRenderData.mTransform = matrix;
						iScene.PlayState.Scene.AddRenderableAdditiveObject(iDataChannel, highlightRenderData);
					}
				}
				if (this.mLiquids.Length > 0)
				{
					Matrix matrix4;
					Matrix.Invert(ref matrix, out matrix4);
					for (int num4 = 0; num4 < this.mLiquids.Length; num4++)
					{
						this.mLiquids[num4].Update(iDataChannel, iDeltaTime, iScene.PlayState.Scene, ref matrix, ref matrix4);
					}
				}
			}
			foreach (AnimatedLevelPart animatedLevelPart in this.mChildren.Values)
			{
				animatedLevelPart.Update(iDataChannel, iDeltaTime, ref matrix, iScene);
			}
			this.mHighlighted -= iDeltaTime;
		}

		// Token: 0x06001607 RID: 5639 RVA: 0x0008C72C File Offset: 0x0008A92C
		public bool IsTouchingEntity(ushort iHandle, bool iCheckChildren)
		{
			float num;
			if (this.mCollidingEntities.TryGetValue(iHandle, out num))
			{
				return num > 0f;
			}
			if (iCheckChildren)
			{
				foreach (AnimatedLevelPart animatedLevelPart in this.mChildren.Values)
				{
					if (animatedLevelPart.IsTouchingEntity(iHandle, iCheckChildren))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x06001608 RID: 5640 RVA: 0x0008C7AC File Offset: 0x0008A9AC
		private void GetTransform(out Matrix oTransform)
		{
			int keyframeIndexByTime = this.mAnimation.GetKeyframeIndexByTime(this.mTime);
			int num = (keyframeIndexByTime + 1) % this.mAnimation.Count;
			AnimationChannelKeyframe animationChannelKeyframe = this.mAnimation[keyframeIndexByTime];
			AnimationChannelKeyframe animationChannelKeyframe2 = this.mAnimation[num];
			float num2;
			if (num == 0)
			{
				num2 = 0f;
			}
			else
			{
				num2 = animationChannelKeyframe2.Time - animationChannelKeyframe.Time;
			}
			if (num2 > 0f)
			{
				float num3 = this.mTime - animationChannelKeyframe.Time;
				float amount = num3 / num2;
				Pose pose;
				Pose.Interpolate(ref animationChannelKeyframe.Pose, ref animationChannelKeyframe2.Pose, amount, InterpolationMode.Linear, InterpolationMode.Linear, InterpolationMode.Linear, out pose);
				pose.GetMatrix(out oTransform);
				return;
			}
			animationChannelKeyframe.Pose.GetMatrix(out oTransform);
		}

		// Token: 0x06001609 RID: 5641 RVA: 0x0008C85C File Offset: 0x0008AA5C
		public void Highlight(float iTTL)
		{
			this.mHighlighted = iTTL;
		}

		// Token: 0x0600160A RID: 5642 RVA: 0x0008C868 File Offset: 0x0008AA68
		public void RegisterCollisionSkin()
		{
			if (this.mCollisionSkin != null && !PhysicsManager.Instance.Simulator.CollisionSystem.CollisionSkins.Contains(this.mCollisionSkin))
			{
				PhysicsManager.Instance.Simulator.CollisionSystem.AddCollisionSkin(this.mCollisionSkin);
			}
			for (int i = 0; i < this.mLiquids.Length; i++)
			{
				this.mLiquids[i].Initialize();
			}
			foreach (AnimatedLevelPart animatedLevelPart in this.mChildren.Values)
			{
				animatedLevelPart.RegisterCollisionSkin();
			}
		}

		// Token: 0x1700059A RID: 1434
		// (get) Token: 0x0600160B RID: 5643 RVA: 0x0008C924 File Offset: 0x0008AB24
		public Matrix AbsoluteTransform
		{
			get
			{
				return this.mOldTransform;
			}
		}

		// Token: 0x1700059B RID: 1435
		// (get) Token: 0x0600160C RID: 5644 RVA: 0x0008C92C File Offset: 0x0008AB2C
		public CollisionSkin CollisionSkin
		{
			get
			{
				return this.mCollisionSkin;
			}
		}

		// Token: 0x0600160D RID: 5645 RVA: 0x0008C934 File Offset: 0x0008AB34
		public AnimatedLevelPart GetChild(int iId)
		{
			return this.mChildren[iId];
		}

		// Token: 0x1700059C RID: 1436
		// (get) Token: 0x0600160E RID: 5646 RVA: 0x0008C942 File Offset: 0x0008AB42
		public string Name
		{
			get
			{
				return this.mName;
			}
		}

		// Token: 0x1700059D RID: 1437
		// (get) Token: 0x0600160F RID: 5647 RVA: 0x0008C94A File Offset: 0x0008AB4A
		public int ID
		{
			get
			{
				return this.mID;
			}
		}

		// Token: 0x1700059E RID: 1438
		// (get) Token: 0x06001610 RID: 5648 RVA: 0x0008C952 File Offset: 0x0008AB52
		public float AnimationDuration
		{
			get
			{
				return this.mAnimationDuration;
			}
		}

		// Token: 0x1700059F RID: 1439
		// (get) Token: 0x06001611 RID: 5649 RVA: 0x0008C95A File Offset: 0x0008AB5A
		public float Time
		{
			get
			{
				return this.mTime;
			}
		}

		// Token: 0x06001612 RID: 5650 RVA: 0x0008C964 File Offset: 0x0008AB64
		public void Play(bool iAllChildren, float iStart, float iEnd, float iSpeed, bool iLoop, bool iResume)
		{
			if (iAllChildren)
			{
				foreach (AnimatedLevelPart animatedLevelPart in this.mChildren.Values)
				{
					animatedLevelPart.Play(true, iStart, iEnd, iSpeed, iLoop, false);
				}
			}
			this.mSpeed = iSpeed;
			if (iStart < 0f)
			{
				iStart = 0f;
			}
			if (iEnd < 0f)
			{
				iEnd = this.mAnimationDuration;
			}
			if (iSpeed < 0f)
			{
				this.mStart = Math.Max(iStart, iEnd);
				this.mEnd = Math.Min(iStart, iEnd);
			}
			else
			{
				this.mStart = Math.Min(iStart, iEnd);
				this.mEnd = Math.Max(iStart, iEnd);
			}
			if (iResume)
			{
				if (iSpeed >= 0f)
				{
					if (this.mTime < this.mStart)
					{
						this.mTime = this.mStart;
					}
					else if (this.mTime > this.mEnd)
					{
						this.mTime = this.mEnd;
					}
				}
				else if (this.mTime > this.mStart)
				{
					this.mTime = this.mStart;
				}
				else if (this.mTime < this.mEnd)
				{
					this.mTime = this.mEnd;
				}
			}
			else
			{
				this.mTime = this.mStart;
			}
			this.mLooping = iLoop;
			this.mPlaying = true;
		}

		// Token: 0x06001613 RID: 5651 RVA: 0x0008CAC4 File Offset: 0x0008ACC4
		public void Stop(bool iAllChildren)
		{
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				NetworkServer networkServer = NetworkManager.Instance.Interface as NetworkServer;
				if (networkServer != null)
				{
					AnimatedLevelPartUpdateMessage animatedLevelPartUpdateMessage = default(AnimatedLevelPartUpdateMessage);
					animatedLevelPartUpdateMessage.Playing = false;
					animatedLevelPartUpdateMessage.AnimationTime = this.mTime;
					networkServer.SendUdpMessage<AnimatedLevelPartUpdateMessage>(ref animatedLevelPartUpdateMessage);
				}
			}
			if (iAllChildren)
			{
				foreach (AnimatedLevelPart animatedLevelPart in this.mChildren.Values)
				{
					animatedLevelPart.Stop(true);
				}
			}
			this.mPlaying = false;
		}

		// Token: 0x06001614 RID: 5652 RVA: 0x0008CB6C File Offset: 0x0008AD6C
		internal void Resume(bool iChildren, float iLength, float iSpeed, bool? iLooping)
		{
			if (iChildren)
			{
				foreach (AnimatedLevelPart animatedLevelPart in this.mChildren.Values)
				{
					animatedLevelPart.Resume(true, iLength, iSpeed, iLooping);
				}
			}
			this.mSpeed = iSpeed;
			if (iLooping != null)
			{
				this.mLooping = iLooping.Value;
			}
			if (iLength > 1E-45f)
			{
				if (iSpeed < 0f)
				{
					this.mEnd = this.mTime - iLength;
				}
				else
				{
					this.mEnd = this.mTime + iLength;
				}
			}
			else
			{
				if (iLength < -1E-45f)
				{
					throw new Exception("Negative length is not an accepted value");
				}
				this.mEnd = this.mAnimationDuration;
			}
			this.mPlaying = true;
		}

		// Token: 0x06001615 RID: 5653 RVA: 0x0008CC40 File Offset: 0x0008AE40
		internal void AddStateTo(Dictionary<int, AnimatedLevelPart.AnimationState> iAnimationStates)
		{
			AnimatedLevelPart.AnimationState value;
			value.Start = this.mStart;
			value.End = this.mEnd;
			value.Loop = this.mLooping;
			value.Playing = this.mPlaying;
			value.Time = this.mTime;
			iAnimationStates.Add(this.mID, value);
			foreach (AnimatedLevelPart animatedLevelPart in this.mChildren.Values)
			{
				animatedLevelPart.AddStateTo(iAnimationStates);
			}
		}

		// Token: 0x06001616 RID: 5654 RVA: 0x0008CCE8 File Offset: 0x0008AEE8
		internal void RestoreStateFrom(Dictionary<int, AnimatedLevelPart.AnimationState> iAnimationStates)
		{
			AnimatedLevelPart.AnimationState animationState;
			if (iAnimationStates.TryGetValue(this.mID, out animationState))
			{
				this.mStart = animationState.Start;
				this.mEnd = animationState.End;
				this.mLooping = animationState.Loop;
				this.mPlaying = animationState.Playing;
				this.mTime = animationState.Time;
			}
			foreach (AnimatedLevelPart animatedLevelPart in this.mChildren.Values)
			{
				animatedLevelPart.RestoreStateFrom(iAnimationStates);
			}
		}

		// Token: 0x06001617 RID: 5655 RVA: 0x0008CD94 File Offset: 0x0008AF94
		internal void NetworkUpdate(NetworkServer iServer)
		{
			if (this.mRestingTimer > 0f)
			{
				AnimatedLevelPartUpdateMessage animatedLevelPartUpdateMessage = default(AnimatedLevelPartUpdateMessage);
				animatedLevelPartUpdateMessage.Handle = this.mHandle;
				animatedLevelPartUpdateMessage.Playing = this.mPlaying;
				for (int i = 0; i < iServer.Connections; i++)
				{
					float num = this.mPlaying ? (iServer.GetLatency(i) * 0.5f) : 0f;
					animatedLevelPartUpdateMessage.AnimationTime = this.mTime + num;
					iServer.SendUdpMessage<AnimatedLevelPartUpdateMessage>(ref animatedLevelPartUpdateMessage, i);
				}
			}
			foreach (AnimatedLevelPart animatedLevelPart in this.mChildren.Values)
			{
				animatedLevelPart.NetworkUpdate(iServer);
			}
		}

		// Token: 0x06001618 RID: 5656 RVA: 0x0008CE64 File Offset: 0x0008B064
		internal void NetworkUpdate(ref AnimatedLevelPartUpdateMessage iMsg)
		{
			this.mTime = iMsg.AnimationTime;
			this.mPlaying = iMsg.Playing;
		}

		// Token: 0x06001619 RID: 5657 RVA: 0x0008CE7E File Offset: 0x0008B07E
		internal void AddDecal(ref DecalManager.DecalReference iDecal)
		{
			this.mDecals.Add(iDecal);
		}

		// Token: 0x0600161A RID: 5658 RVA: 0x0008CE91 File Offset: 0x0008B091
		internal void AddEntity(Entity iEntity)
		{
			this.mCollidingEntities[iEntity.Handle] = float.MaxValue;
		}

		// Token: 0x0600161B RID: 5659 RVA: 0x0008CEA9 File Offset: 0x0008B0A9
		internal void RemoveEntity(Entity iEntity)
		{
			this.mCollidingEntities.Remove(iEntity.Handle);
		}

		// Token: 0x0600161C RID: 5660 RVA: 0x0008CEC0 File Offset: 0x0008B0C0
		internal bool SegmentIntersect(out float oFrac, out Vector3 oPos, out Vector3 oNrm, out AnimatedLevelPart oAnim, Segment iSeg)
		{
			oFrac = float.MaxValue;
			oPos = default(Vector3);
			oNrm = default(Vector3);
			oAnim = null;
			float num;
			Vector3 vector;
			Vector3 vector2;
			if (this.mCollisionSkin != null && this.mCollisionSkin.SegmentIntersect(out num, out vector, out vector2, iSeg))
			{
				oFrac = num;
				oPos = vector;
				oNrm = vector2;
				oAnim = this;
			}
			foreach (AnimatedLevelPart animatedLevelPart in this.mChildren.Values)
			{
				AnimatedLevelPart animatedLevelPart2;
				if (animatedLevelPart.SegmentIntersect(out num, out vector, out vector2, out animatedLevelPart2, iSeg) && num < oFrac)
				{
					oFrac = num;
					oPos = vector;
					oNrm = vector2;
					oAnim = animatedLevelPart2;
				}
			}
			return oFrac <= 1f;
		}

		// Token: 0x0600161D RID: 5661 RVA: 0x0008CF98 File Offset: 0x0008B198
		public void Dispose()
		{
			foreach (ModelMesh modelMesh in this.mModel.Meshes)
			{
				modelMesh.VertexBuffer.Dispose();
				modelMesh.IndexBuffer.Dispose();
				foreach (ModelMeshPart modelMeshPart in modelMesh.MeshParts)
				{
					modelMeshPart.Effect.Dispose();
					modelMeshPart.VertexDeclaration.Dispose();
				}
			}
			foreach (AnimatedLevelPart animatedLevelPart in this.mChildren.Values)
			{
				animatedLevelPart.Dispose();
			}
			this.mLevel = null;
		}

		// Token: 0x0600161E RID: 5662 RVA: 0x0008D0A0 File Offset: 0x0008B2A0
		internal bool TryGetLocator(int iId, ref Matrix iParentTransform, out Locator oLocator)
		{
			Matrix matrix = this.mOldTransform;
			if (this.mLocators.TryGetValue(iId, out oLocator))
			{
				Matrix.Multiply(ref oLocator.Transform, ref matrix, out oLocator.Transform);
				return true;
			}
			foreach (AnimatedLevelPart animatedLevelPart in this.mChildren.Values)
			{
				if (animatedLevelPart.TryGetLocator(iId, ref matrix, out oLocator))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600161F RID: 5663 RVA: 0x0008D130 File Offset: 0x0008B330
		internal void GetLiquids(List<Liquid> iLiquids)
		{
			iLiquids.AddRange(this.mLiquids);
			foreach (AnimatedLevelPart animatedLevelPart in this.mChildren.Values)
			{
				animatedLevelPart.GetLiquids(iLiquids);
			}
		}

		// Token: 0x06001620 RID: 5664 RVA: 0x0008D194 File Offset: 0x0008B394
		internal void GetAllEffects(SortedList<int, GameScene.EffectStorage> iEffects)
		{
			for (int i = 0; i < this.mEffects.Length; i++)
			{
				GameScene.EffectStorage value;
				value.Effect = EffectManager.Instance.GetEffect(this.mEffects[i].Effect);
				value.Transform = this.mEffects[i].Transform;
				value.Animation = this;
				value.Range = this.mEffects[i].Range;
				Matrix matrix;
				Matrix.Multiply(ref value.Transform, ref this.mOldTransform, out matrix);
				value.Effect.Start(ref matrix);
				iEffects.Add(this.mEffects[i].ID, value);
			}
			foreach (AnimatedLevelPart animatedLevelPart in this.mChildren.Values)
			{
				animatedLevelPart.GetAllEffects(iEffects);
			}
		}

		// Token: 0x04001736 RID: 5942
		private static List<AnimatedLevelPart> mInstances = new List<AnimatedLevelPart>();

		// Token: 0x04001737 RID: 5943
		private RenderableAdditiveObject<AdditiveEffect, AdditiveMaterial>[][] mAdditiveRenderData;

		// Token: 0x04001738 RID: 5944
		private AnimatedLevelPart.DeferredRenderData[][] mDefaultRenderData;

		// Token: 0x04001739 RID: 5945
		private AnimatedLevelPart.HighlightRenderData[][] mHighlightRenderData;

		// Token: 0x0400173A RID: 5946
		private Dictionary<int, Locator> mLocators = new Dictionary<int, Locator>();

		// Token: 0x0400173B RID: 5947
		private Liquid[] mLiquids;

		// Token: 0x0400173C RID: 5948
		private BoundingSphere mBoundingSphere;

		// Token: 0x0400173D RID: 5949
		private string mName;

		// Token: 0x0400173E RID: 5950
		private int mID;

		// Token: 0x0400173F RID: 5951
		private ushort mHandle;

		// Token: 0x04001740 RID: 5952
		private Model mModel;

		// Token: 0x04001741 RID: 5953
		private AnimationChannel mAnimation;

		// Token: 0x04001742 RID: 5954
		private float mAnimationDuration;

		// Token: 0x04001743 RID: 5955
		private float mTime;

		// Token: 0x04001744 RID: 5956
		private CollisionMaterials mCollisionMaterial;

		// Token: 0x04001745 RID: 5957
		private int[] mLights;

		// Token: 0x04001746 RID: 5958
		private Matrix[] mLightPositions;

		// Token: 0x04001747 RID: 5959
		private LevelModel.VisualEffectStorage[] mEffects;

		// Token: 0x04001748 RID: 5960
		private CollisionSkin mCollisionSkin;

		// Token: 0x04001749 RID: 5961
		private AnimatedNavMesh mNavMesh;

		// Token: 0x0400174A RID: 5962
		private Dictionary<int, AnimatedLevelPart> mChildren;

		// Token: 0x0400174B RID: 5963
		private LevelModel mLevel;

		// Token: 0x0400174C RID: 5964
		private float mSpeed = 1f;

		// Token: 0x0400174D RID: 5965
		private float mStart;

		// Token: 0x0400174E RID: 5966
		private float mEnd;

		// Token: 0x0400174F RID: 5967
		private bool mLooping;

		// Token: 0x04001750 RID: 5968
		private bool mPlaying;

		// Token: 0x04001751 RID: 5969
		private float mRestingTimer = -1f;

		// Token: 0x04001752 RID: 5970
		private bool mAffectShields = true;

		// Token: 0x04001753 RID: 5971
		private float mHighlighted = -1f;

		// Token: 0x04001754 RID: 5972
		private Dictionary<string, KeyValuePair<bool, bool>> mMeshSettings;

		// Token: 0x04001755 RID: 5973
		private Matrix mOldTransform;

		// Token: 0x04001756 RID: 5974
		private SortedList<ushort, float> mCollidingEntities = new SortedList<ushort, float>(32);

		// Token: 0x04001757 RID: 5975
		private List<DecalManager.DecalReference> mDecals = new List<DecalManager.DecalReference>(64);

		// Token: 0x020002D1 RID: 721
		public struct AnimationState
		{
			// Token: 0x06001622 RID: 5666 RVA: 0x0008D2A8 File Offset: 0x0008B4A8
			public AnimationState(BinaryReader iReader)
			{
				this.Start = iReader.ReadSingle();
				this.End = iReader.ReadSingle();
				this.Loop = iReader.ReadBoolean();
				this.Playing = iReader.ReadBoolean();
				this.Time = iReader.ReadSingle();
			}

			// Token: 0x06001623 RID: 5667 RVA: 0x0008D2E6 File Offset: 0x0008B4E6
			public void Write(BinaryWriter iWriter)
			{
				iWriter.Write(this.Start);
				iWriter.Write(this.End);
				iWriter.Write(this.Loop);
				iWriter.Write(this.Playing);
				iWriter.Write(this.Time);
			}

			// Token: 0x04001758 RID: 5976
			public float Start;

			// Token: 0x04001759 RID: 5977
			public float End;

			// Token: 0x0400175A RID: 5978
			public bool Loop;

			// Token: 0x0400175B RID: 5979
			public bool Playing;

			// Token: 0x0400175C RID: 5980
			public float Time;
		}

		// Token: 0x020002D2 RID: 722
		protected class DeferredRenderData : RenderableObject<RenderDeferredEffect, RenderDeferredMaterial>
		{
			// Token: 0x06001624 RID: 5668 RVA: 0x0008D324 File Offset: 0x0008B524
			public DeferredRenderData(bool iVisible, bool iCastShadows)
			{
				this.mVisible = iVisible;
				this.mCastShadows = iCastShadows;
			}

			// Token: 0x06001625 RID: 5669 RVA: 0x0008D33A File Offset: 0x0008B53A
			public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				if (!this.mVisible)
				{
					return;
				}
				iEffect.GraphicsDevice.RenderState.ReferenceStencil = 1;
				base.Draw(iEffect, iViewFrustum);
				iEffect.GraphicsDevice.RenderState.ReferenceStencil = 0;
			}

			// Token: 0x06001626 RID: 5670 RVA: 0x0008D370 File Offset: 0x0008B570
			public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				if (iEffect.CurrentTechnique == iEffect.Techniques[this.ShadowTechnique] && !this.mCastShadows)
				{
					return;
				}
				if (iEffect.CurrentTechnique == iEffect.Techniques[this.DepthTechnique] && !this.mVisible)
				{
					return;
				}
				base.DrawShadow(iEffect, iViewFrustum);
			}

			// Token: 0x06001627 RID: 5671 RVA: 0x0008D3C9 File Offset: 0x0008B5C9
			public void SetMesh(ModelMesh iMesh, ModelMeshPart iPart, VertexDeclaration iVertexDeclaration, int iDepthTechnique, int iTechnique, int iShadowTechnique)
			{
				base.SetMesh(iMesh, iPart, iDepthTechnique, iTechnique, iShadowTechnique);
				this.mVertexDeclaration = iVertexDeclaration;
			}

			// Token: 0x0400175D RID: 5981
			private bool mVisible;

			// Token: 0x0400175E RID: 5982
			private bool mCastShadows;
		}

		// Token: 0x020002D3 RID: 723
		protected class HighlightRenderData : IRenderableAdditiveObject
		{
			// Token: 0x06001628 RID: 5672 RVA: 0x0008D3E0 File Offset: 0x0008B5E0
			public HighlightRenderData(bool iVisible)
			{
				this.mVisible = iVisible;
			}

			// Token: 0x170005A0 RID: 1440
			// (get) Token: 0x06001629 RID: 5673 RVA: 0x0008D3F6 File Offset: 0x0008B5F6
			public int Effect
			{
				get
				{
					return this.mEffect;
				}
			}

			// Token: 0x170005A1 RID: 1441
			// (get) Token: 0x0600162A RID: 5674 RVA: 0x0008D3FE File Offset: 0x0008B5FE
			public int Technique
			{
				get
				{
					return 6;
				}
			}

			// Token: 0x170005A2 RID: 1442
			// (get) Token: 0x0600162B RID: 5675 RVA: 0x0008D401 File Offset: 0x0008B601
			public VertexBuffer Vertices
			{
				get
				{
					return this.mVertexBuffer;
				}
			}

			// Token: 0x170005A3 RID: 1443
			// (get) Token: 0x0600162C RID: 5676 RVA: 0x0008D409 File Offset: 0x0008B609
			public int VerticesHashCode
			{
				get
				{
					return this.mVerticesHash;
				}
			}

			// Token: 0x170005A4 RID: 1444
			// (get) Token: 0x0600162D RID: 5677 RVA: 0x0008D411 File Offset: 0x0008B611
			public int VertexStride
			{
				get
				{
					return this.mVertexStride;
				}
			}

			// Token: 0x170005A5 RID: 1445
			// (get) Token: 0x0600162E RID: 5678 RVA: 0x0008D419 File Offset: 0x0008B619
			public IndexBuffer Indices
			{
				get
				{
					return this.mIndexBuffer;
				}
			}

			// Token: 0x170005A6 RID: 1446
			// (get) Token: 0x0600162F RID: 5679 RVA: 0x0008D421 File Offset: 0x0008B621
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return this.mVertexDeclaration;
				}
			}

			// Token: 0x06001630 RID: 5680 RVA: 0x0008D42C File Offset: 0x0008B62C
			public bool Cull(BoundingFrustum iViewFrustum)
			{
				BoundingSphere boundingSphere = this.mBoundingSphere;
				return boundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
			}

			// Token: 0x06001631 RID: 5681 RVA: 0x0008D44C File Offset: 0x0008B64C
			public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				if (!this.mVisible)
				{
					return;
				}
				RenderDeferredEffect renderDeferredEffect = iEffect as RenderDeferredEffect;
				this.mMaterial.AssignToEffect(renderDeferredEffect);
				renderDeferredEffect.DiffuseColor0 = new Vector3(1f);
				renderDeferredEffect.FresnelPower = 1f;
				renderDeferredEffect.World = this.mTransform;
				renderDeferredEffect.CommitChanges();
				renderDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
				renderDeferredEffect.DiffuseColor0 = Vector3.One;
			}

			// Token: 0x170005A7 RID: 1447
			// (get) Token: 0x06001632 RID: 5682 RVA: 0x0008D4D2 File Offset: 0x0008B6D2
			public bool MeshDirty
			{
				get
				{
					return this.mMeshDirty;
				}
			}

			// Token: 0x06001633 RID: 5683 RVA: 0x0008D4DA File Offset: 0x0008B6DA
			public void SetMeshDirty()
			{
				this.mMeshDirty = true;
			}

			// Token: 0x06001634 RID: 5684 RVA: 0x0008D4E4 File Offset: 0x0008B6E4
			public void SetMesh(VertexBuffer iVertices, IndexBuffer iIndices, ModelMeshPart iMeshPart, int iEffectHash)
			{
				this.mMeshDirty = false;
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
				this.mMaterial.FetchFromEffect(iMeshPart.Effect as RenderDeferredEffect);
			}

			// Token: 0x0400175F RID: 5983
			protected VertexDeclaration mVertexDeclaration;

			// Token: 0x04001760 RID: 5984
			protected int mBaseVertex;

			// Token: 0x04001761 RID: 5985
			protected int mNumVertices;

			// Token: 0x04001762 RID: 5986
			protected int mPrimitiveCount;

			// Token: 0x04001763 RID: 5987
			protected int mStartIndex;

			// Token: 0x04001764 RID: 5988
			protected int mStreamOffset;

			// Token: 0x04001765 RID: 5989
			protected int mVertexStride;

			// Token: 0x04001766 RID: 5990
			protected VertexBuffer mVertexBuffer;

			// Token: 0x04001767 RID: 5991
			protected int mVerticesHash;

			// Token: 0x04001768 RID: 5992
			protected IndexBuffer mIndexBuffer;

			// Token: 0x04001769 RID: 5993
			protected int mEffect;

			// Token: 0x0400176A RID: 5994
			protected RenderDeferredMaterial mMaterial;

			// Token: 0x0400176B RID: 5995
			public BoundingSphere mBoundingSphere;

			// Token: 0x0400176C RID: 5996
			public Matrix mTransform;

			// Token: 0x0400176D RID: 5997
			protected bool mMeshDirty = true;

			// Token: 0x0400176E RID: 5998
			protected bool mVisible;
		}
	}
}
