using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Magicka.Graphics.Effects
{
	// Token: 0x02000292 RID: 658
	public class SkinnedModelSkeletonEffect : Effect
	{
		// Token: 0x06001377 RID: 4983 RVA: 0x000782FC File Offset: 0x000764FC
		public SkinnedModelSkeletonEffect(GraphicsDevice iDevice, ContentManager iContentManager) : base(iDevice, iContentManager.Load<Effect>("Shaders/SkinnedModelSkeletonEffect"))
		{
			this.mViewParameter = base.Parameters["View"];
			this.mProjectionParameter = base.Parameters["Projection"];
			this.mBonesParameter = base.Parameters["Bones"];
			this.mDiffuseMap0EnabledParameter = base.Parameters["DiffuseMap0Enabled"];
			this.mDiffuseMap1EnabledParameter = base.Parameters["DiffuseMap1Enabled"];
			this.mDiffuseMap0Parameter = base.Parameters["DiffuseMap0"];
			this.mDiffuseMap1Parameter = base.Parameters["DiffuseMap1"];
			this.mDepthMapParameter = base.Parameters["DepthMap"];
		}

		// Token: 0x06001378 RID: 4984 RVA: 0x000783CB File Offset: 0x000765CB
		public void SetTechnique(SkinnedModelSkeletonEffect.Technique iTechnique)
		{
			base.CurrentTechnique = base.Techniques[(int)iTechnique];
		}

		// Token: 0x17000501 RID: 1281
		// (get) Token: 0x06001379 RID: 4985 RVA: 0x000783DF File Offset: 0x000765DF
		// (set) Token: 0x0600137A RID: 4986 RVA: 0x000783EC File Offset: 0x000765EC
		public Matrix View
		{
			get
			{
				return this.mViewParameter.GetValueMatrix();
			}
			set
			{
				this.mViewParameter.SetValue(value);
			}
		}

		// Token: 0x17000502 RID: 1282
		// (get) Token: 0x0600137B RID: 4987 RVA: 0x000783FA File Offset: 0x000765FA
		// (set) Token: 0x0600137C RID: 4988 RVA: 0x00078407 File Offset: 0x00076607
		public Matrix Projection
		{
			get
			{
				return this.mProjectionParameter.GetValueMatrix();
			}
			set
			{
				this.mProjectionParameter.SetValue(value);
			}
		}

		// Token: 0x17000503 RID: 1283
		// (get) Token: 0x0600137D RID: 4989 RVA: 0x00078415 File Offset: 0x00076615
		// (set) Token: 0x0600137E RID: 4990 RVA: 0x00078424 File Offset: 0x00076624
		public Matrix[] Bones
		{
			get
			{
				return this.mBonesParameter.GetValueMatrixArray(80);
			}
			set
			{
				this.mBonesParameter.SetValue(value);
			}
		}

		// Token: 0x17000504 RID: 1284
		// (get) Token: 0x0600137F RID: 4991 RVA: 0x00078432 File Offset: 0x00076632
		// (set) Token: 0x06001380 RID: 4992 RVA: 0x0007843F File Offset: 0x0007663F
		public bool DiffuseMap0Enabled
		{
			get
			{
				return this.mDiffuseMap0EnabledParameter.GetValueBoolean();
			}
			set
			{
				this.mDiffuseMap0EnabledParameter.SetValue(value);
			}
		}

		// Token: 0x17000505 RID: 1285
		// (get) Token: 0x06001381 RID: 4993 RVA: 0x0007844D File Offset: 0x0007664D
		// (set) Token: 0x06001382 RID: 4994 RVA: 0x0007845A File Offset: 0x0007665A
		public bool DiffuseMap1Enabled
		{
			get
			{
				return this.mDiffuseMap1EnabledParameter.GetValueBoolean();
			}
			set
			{
				this.mDiffuseMap1EnabledParameter.SetValue(value);
			}
		}

		// Token: 0x17000506 RID: 1286
		// (get) Token: 0x06001383 RID: 4995 RVA: 0x00078468 File Offset: 0x00076668
		// (set) Token: 0x06001384 RID: 4996 RVA: 0x00078475 File Offset: 0x00076675
		public Texture2D DiffuseMap0
		{
			get
			{
				return this.mDiffuseMap0Parameter.GetValueTexture2D();
			}
			set
			{
				this.mDiffuseMap0Parameter.SetValue(value);
			}
		}

		// Token: 0x17000507 RID: 1287
		// (get) Token: 0x06001385 RID: 4997 RVA: 0x00078483 File Offset: 0x00076683
		// (set) Token: 0x06001386 RID: 4998 RVA: 0x00078490 File Offset: 0x00076690
		public Texture2D DiffuseMap1
		{
			get
			{
				return this.mDiffuseMap1Parameter.GetValueTexture2D();
			}
			set
			{
				this.mDiffuseMap1Parameter.SetValue(value);
			}
		}

		// Token: 0x17000508 RID: 1288
		// (get) Token: 0x06001387 RID: 4999 RVA: 0x0007849E File Offset: 0x0007669E
		// (set) Token: 0x06001388 RID: 5000 RVA: 0x000784AB File Offset: 0x000766AB
		public Texture2D DepthMap
		{
			get
			{
				return this.mDepthMapParameter.GetValueTexture2D();
			}
			set
			{
				this.mDepthMapParameter.SetValue(value);
			}
		}

		// Token: 0x0400150A RID: 5386
		public static readonly int TYPEHASH = typeof(SkinnedModelSkeletonEffect).GetHashCode();

		// Token: 0x0400150B RID: 5387
		private EffectParameter mViewParameter;

		// Token: 0x0400150C RID: 5388
		private EffectParameter mProjectionParameter;

		// Token: 0x0400150D RID: 5389
		private EffectParameter mBonesParameter;

		// Token: 0x0400150E RID: 5390
		private EffectParameter mDiffuseMap0EnabledParameter;

		// Token: 0x0400150F RID: 5391
		private EffectParameter mDiffuseMap1EnabledParameter;

		// Token: 0x04001510 RID: 5392
		private EffectParameter mDiffuseMap0Parameter;

		// Token: 0x04001511 RID: 5393
		private EffectParameter mDiffuseMap1Parameter;

		// Token: 0x04001512 RID: 5394
		private EffectParameter mDepthMapParameter;

		// Token: 0x02000293 RID: 659
		public enum Technique
		{
			// Token: 0x04001514 RID: 5396
			Body,
			// Token: 0x04001515 RID: 5397
			Skeleton
		}
	}
}
