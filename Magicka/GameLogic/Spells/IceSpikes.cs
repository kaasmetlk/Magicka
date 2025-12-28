using System;
using System.Collections.Generic;
using JigLibX.Geometry;
using Magicka.GameLogic.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

namespace Magicka.GameLogic.Spells
{
	// Token: 0x02000152 RID: 338
	internal class IceSpikes : IAbilityEffect
	{
		// Token: 0x06000A10 RID: 2576 RVA: 0x0003C770 File Offset: 0x0003A970
		public static IceSpikes GetInstance()
		{
			if (IceSpikes.sCache.Count > 0)
			{
				IceSpikes result = IceSpikes.sCache[IceSpikes.sCache.Count - 1];
				IceSpikes.sCache.RemoveAt(IceSpikes.sCache.Count - 1);
				return result;
			}
			return new IceSpikes();
		}

		// Token: 0x06000A11 RID: 2577 RVA: 0x0003C7C0 File Offset: 0x0003A9C0
		public static void InitializeCache(int iNr)
		{
			IceSpikes.sCache = new List<IceSpikes>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				IceSpikes.sCache.Add(new IceSpikes());
			}
		}

		// Token: 0x06000A12 RID: 2578 RVA: 0x0003C7F4 File Offset: 0x0003A9F4
		private IceSpikes()
		{
			if (IceSpikes.sModels == null)
			{
				GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
				IceSpikes.sModels = new SkinnedModel[1];
				lock (graphicsDevice)
				{
					IceSpikes.sModels[0] = Game.Instance.Content.Load<SkinnedModel>("Models/Effects/IceBarrier0_mesh");
				}
				SkinnedModel skinnedModel = Game.Instance.Content.Load<SkinnedModel>("Models/Effects/IceBarrier_animation");
				IceSpikes.sAnimations = new AnimationClip[skinnedModel.AnimationClips.Count];
				int num = 0;
				foreach (AnimationClip animationClip in skinnedModel.AnimationClips.Values)
				{
					IceSpikes.sAnimations[num++] = animationClip;
				}
			}
			this.mDraw = new bool[18];
			this.mRoots = new Matrix[18];
			this.mControllers = new AnimationController[18];
			for (int i = 0; i < this.mControllers.Length; i++)
			{
				this.mControllers[i] = new AnimationController();
				this.mControllers[i].Skeleton = IceSpikes.sModels[0].SkeletonBones;
			}
			this.mBoundingSphere = IceSpikes.sModels[0].Model.Meshes[0].BoundingSphere;
			this.mRenderData = new IceSpikes.RenderData[3];
			for (int j = 0; j < 3; j++)
			{
				IceSpikes.RenderData renderData = new IceSpikes.RenderData(this.mControllers.Length);
				this.mRenderData[j] = renderData;
				renderData.DoDraw = this.mDraw;
			}
		}

		// Token: 0x06000A13 RID: 2579 RVA: 0x0003C9A8 File Offset: 0x0003ABA8
		public bool Initialize(PlayState iPlayState, ref Vector3 iCenter, float iRadius)
		{
			this.mDead = false;
			this.mScene = iPlayState.Scene;
			float num = iRadius / 4.332f;
			bool flag = false;
			Segment iSeg = default(Segment);
			iSeg.Delta.Y = -3f;
			int num2 = 0;
			float num3 = 2.6179938f;
			Vector3 vector = default(Vector3);
			for (int i = 0; i < 12; i++)
			{
				vector.X = (float)Math.Sin((double)num3) * 2f * 1.666f * num;
				vector.Z = (float)Math.Cos((double)num3) * 2f * 1.666f * num;
				Vector3.Add(ref vector, ref iCenter, out iSeg.Origin);
				iSeg.Origin.Y = iSeg.Origin.Y + 0.5f;
				float num4;
				Vector3 translation;
				Vector3 vector2;
				this.mDraw[num2] = iPlayState.Level.CurrentScene.SegmentIntersect(out num4, out translation, out vector2, iSeg);
				if (this.mDraw[num2])
				{
					flag = true;
					Matrix.CreateRotationY((float)IceSpikes.sRandom.NextDouble() * 6.2831855f, out this.mRoots[num2]);
					MagickaMath.UniformMatrixScale(ref this.mRoots[num2], num);
					this.mRoots[num2].Translation = translation;
					this.mControllers[num2].PlaybackMode = PlaybackMode.Forward;
					this.mControllers[num2].ClipSpeed = 2f;
					this.mControllers[num2].PlayClip(IceSpikes.sAnimations[IceSpikes.sRandom.Next(IceSpikes.sAnimations.Length)], false);
				}
				num2++;
				num3 += 0.5235988f;
			}
			for (int j = 0; j < 6; j++)
			{
				vector.X = (float)Math.Sin((double)num3) * 1f * 1.666f * num;
				vector.Z = (float)Math.Cos((double)num3) * 1f * 1.666f * num;
				Vector3.Add(ref vector, ref iCenter, out iSeg.Origin);
				iSeg.Origin.Y = iSeg.Origin.Y + 1.5f;
				float num4;
				Vector3 translation;
				Vector3 vector2;
				this.mDraw[num2] = iPlayState.Level.CurrentScene.SegmentIntersect(out num4, out translation, out vector2, iSeg);
				if (this.mDraw[num2])
				{
					flag = true;
					Matrix.CreateRotationY((float)IceSpikes.sRandom.NextDouble() * 6.2831855f, out this.mRoots[num2]);
					MagickaMath.UniformMatrixScale(ref this.mRoots[num2], num);
					this.mRoots[num2].Translation = translation;
					this.mControllers[num2].PlaybackMode = PlaybackMode.Forward;
					this.mControllers[num2].ClipSpeed = 2f;
					this.mControllers[num2].PlayClip(IceSpikes.sAnimations[IceSpikes.sRandom.Next(IceSpikes.sAnimations.Length)], false);
				}
				num2++;
				num3 += 1.0471976f;
			}
			ModelMesh modelMesh = IceSpikes.sModels[0].Model.Meshes[0];
			ModelMeshPart modelMeshPart = modelMesh.MeshParts[0];
			SkinnedModelDeferredBasicMaterial mMaterial;
			Helper.SkinnedModelDeferredMaterialFromBasicEffect(modelMeshPart.Effect as SkinnedModelBasicEffect, out mMaterial);
			for (int k = 0; k < 3; k++)
			{
				IceSpikes.RenderData renderData = this.mRenderData[k];
				renderData.mBoundingSphere.Center = iCenter;
				renderData.mBoundingSphere.Radius = 3.332f;
				renderData.SetMesh(modelMesh.VertexBuffer, modelMesh.IndexBuffer, modelMeshPart, 0, 3, 4);
				renderData.mMaterial = mMaterial;
			}
			if (!flag)
			{
				return false;
			}
			SpellManager.Instance.AddSpellEffect(this);
			return true;
		}

		// Token: 0x17000228 RID: 552
		// (get) Token: 0x06000A14 RID: 2580 RVA: 0x0003CD2C File Offset: 0x0003AF2C
		public bool IsDead
		{
			get
			{
				if (!this.mDead)
				{
					return false;
				}
				for (int i = 0; i < this.mControllers.Length; i++)
				{
					if (this.mDraw[i] & !this.mControllers[i].HasFinished)
					{
						return false;
					}
				}
				return true;
			}
		}

		// Token: 0x06000A15 RID: 2581 RVA: 0x0003CD74 File Offset: 0x0003AF74
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			IceSpikes.RenderData renderData = this.mRenderData[(int)iDataChannel];
			for (int i = 0; i < this.mControllers.Length; i++)
			{
				if (this.mDraw[i])
				{
					AnimationController animationController = this.mControllers[i];
					if (animationController.HasFinished && animationController.PlaybackMode == PlaybackMode.Forward)
					{
						animationController.PlaybackMode = PlaybackMode.Backward;
						animationController.StartClip(animationController.AnimationClip, false);
						this.mDead = true;
					}
					animationController.Update(iDeltaTime, ref this.mRoots[i], true);
					this.mBoundingSphere.Center = this.mRoots[i].Translation;
					renderData.mBoundingSphere = this.mBoundingSphere;
					Array.Copy(animationController.SkinnedBoneTransforms, renderData.Bones[i], animationController.Skeleton.Count);
				}
			}
			this.mScene.AddRenderableObject(iDataChannel, renderData);
		}

		// Token: 0x06000A16 RID: 2582 RVA: 0x0003CE4B File Offset: 0x0003B04B
		public void OnRemove()
		{
			this.mScene = null;
		}

		// Token: 0x04000919 RID: 2329
		private static List<IceSpikes> sCache;

		// Token: 0x0400091A RID: 2330
		private static Random sRandom = new Random();

		// Token: 0x0400091B RID: 2331
		private static SkinnedModel[] sModels;

		// Token: 0x0400091C RID: 2332
		private static AnimationClip[] sAnimations;

		// Token: 0x0400091D RID: 2333
		private IceSpikes.RenderData[] mRenderData;

		// Token: 0x0400091E RID: 2334
		private bool[] mDraw;

		// Token: 0x0400091F RID: 2335
		private Matrix[] mRoots;

		// Token: 0x04000920 RID: 2336
		private AnimationController[] mControllers;

		// Token: 0x04000921 RID: 2337
		private BoundingSphere mBoundingSphere;

		// Token: 0x04000922 RID: 2338
		private bool mDead;

		// Token: 0x04000923 RID: 2339
		private Scene mScene;

		// Token: 0x02000153 RID: 339
		private class RenderData : RenderableObject<SkinnedModelDeferredEffect, SkinnedModelDeferredBasicMaterial>
		{
			// Token: 0x06000A18 RID: 2584 RVA: 0x0003CE60 File Offset: 0x0003B060
			public RenderData(int iControllerCount)
			{
				this.Bones = new Matrix[iControllerCount][];
				for (int i = 0; i < iControllerCount; i++)
				{
					this.Bones[i] = new Matrix[80];
				}
			}

			// Token: 0x06000A19 RID: 2585 RVA: 0x0003CE9C File Offset: 0x0003B09C
			public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				for (int i = 0; i < this.DoDraw.Length; i++)
				{
					if (this.DoDraw[i])
					{
						(iEffect as SkinnedModelDeferredEffect).Bones = this.Bones[i];
						base.Draw(iEffect, iViewFrustum);
					}
				}
			}

			// Token: 0x06000A1A RID: 2586 RVA: 0x0003CEE4 File Offset: 0x0003B0E4
			public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				for (int i = 0; i < this.DoDraw.Length; i++)
				{
					if (this.DoDraw[i])
					{
						(iEffect as SkinnedModelDeferredEffect).Bones = this.Bones[i];
						base.DrawShadow(iEffect, iViewFrustum);
					}
				}
			}

			// Token: 0x04000924 RID: 2340
			public bool[] DoDraw;

			// Token: 0x04000925 RID: 2341
			public Matrix[][] Bones;
		}
	}
}
