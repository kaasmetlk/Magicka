using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Lights;

namespace Magicka.Graphics.Lights
{
	// Token: 0x02000560 RID: 1376
	public class DynamicLight : PointLight
	{
		// Token: 0x06002903 RID: 10499 RVA: 0x00142244 File Offset: 0x00140444
		private DynamicLight() : base(Game.Instance.GraphicsDevice)
		{
		}

		// Token: 0x06002904 RID: 10500 RVA: 0x0014226C File Offset: 0x0014046C
		public static void StopAndClearAll()
		{
			if (DynamicLight.sLightCache == null)
			{
				return;
			}
			foreach (DynamicLight dynamicLight in DynamicLight.sLightCache)
			{
				dynamicLight.Stop(true);
			}
			Thread.Sleep(0);
			DynamicLight.sLightCache.Clear();
		}

		// Token: 0x06002905 RID: 10501 RVA: 0x001422D8 File Offset: 0x001404D8
		public static void Initialize(Scene iScene)
		{
			DynamicLight.sLightCache = new Queue<DynamicLight>(128);
			for (int i = 0; i < 128; i++)
			{
				DynamicLight.sLightCache.Enqueue(new DynamicLight());
			}
			DynamicLight.sScene = new WeakReference(iScene);
		}

		// Token: 0x06002906 RID: 10502 RVA: 0x00142320 File Offset: 0x00140520
		public static void DisposeCache()
		{
			lock (DynamicLight.sLightCache)
			{
				for (int i = 0; i < DynamicLight.sLightCache.Count; i++)
				{
					DynamicLight dynamicLight = DynamicLight.sLightCache.Dequeue();
					dynamicLight.DisposeShadowMap();
					DynamicLight.sLightCache.Enqueue(dynamicLight);
				}
			}
		}

		// Token: 0x06002907 RID: 10503 RVA: 0x00142384 File Offset: 0x00140584
		public static DynamicLight GetCachedLight()
		{
			DynamicLight result;
			lock (DynamicLight.sLightCache)
			{
				if (DynamicLight.sLightCache.Count > 0)
				{
					result = DynamicLight.sLightCache.Dequeue();
				}
				else
				{
					result = new DynamicLight();
				}
			}
			return result;
		}

		// Token: 0x06002908 RID: 10504 RVA: 0x001423D8 File Offset: 0x001405D8
		public void Initialize()
		{
			this.Initialize(Vector3.Zero, Vector3.One, 1f, 1f, 1f, 1f);
		}

		// Token: 0x06002909 RID: 10505 RVA: 0x001423FE File Offset: 0x001405FE
		public void Initialize(Vector3 iPosition)
		{
			this.Initialize(iPosition, Vector3.One, 1f, 1f, 1f, 1f);
		}

		// Token: 0x0600290A RID: 10506 RVA: 0x00142420 File Offset: 0x00140620
		public void Initialize(Vector3 iPosition, Vector3 iColor, float iIntensity, float iRadius, float iSpeed, float iSpecularAmount)
		{
			this.Initialize(iPosition, iColor, iIntensity, iRadius, iSpeed, iSpecularAmount, Vector3.Zero, 1f, -1f);
		}

		// Token: 0x0600290B RID: 10507 RVA: 0x0014244C File Offset: 0x0014064C
		public void Initialize(Vector3 iPosition, Vector3 iColor, float iIntensity, float iRadius, float iSpeed, float iSpecularAmount, float iTTL)
		{
			this.Initialize(iPosition, iColor, iIntensity, iRadius, iSpeed, iSpecularAmount, Vector3.Zero, 1f, iTTL);
		}

		// Token: 0x0600290C RID: 10508 RVA: 0x00142474 File Offset: 0x00140674
		public void Initialize(Vector3 iPosition, Vector3 iColor, float iIntensity, float iRadius, float iSpeed, float iSpecularAmount, Vector3 iVelocity, float iFriction)
		{
			this.Initialize(iPosition, iColor, iIntensity, iRadius, iSpeed, iSpecularAmount, iVelocity, iFriction, -1f);
		}

		// Token: 0x0600290D RID: 10509 RVA: 0x0014249C File Offset: 0x0014069C
		public void Initialize(Vector3 iPosition, Vector3 iColor, float iIntensity, float iRadius, float iSpeed, float iSpecularAmount, Vector3 iVelocity, float iFriction, float iTTL)
		{
			if (iSpeed <= 1E-45f)
			{
				throw new ArgumentException("iSpeed must be greater than zero!", "iSpeed");
			}
			base.Position = iPosition;
			this.mDynamicIntensity = iIntensity;
			this.mSpeed = iSpeed;
			this.DiffuseColor = iColor;
			this.SpecularAmount = iSpecularAmount;
			this.mVelocity = iVelocity;
			this.mFriction = iFriction;
			this.mTTL = iTTL;
			base.Radius = iRadius;
			this.CastShadows = false;
			this.ShadowMapSize = GlobalSettings.Instance.ModShadowResolution(256);
			base.VariationAmount = 0f;
			base.VariationSpeed = 0f;
			base.VariationType = LightVariationType.None;
		}

		// Token: 0x0600290E RID: 10510 RVA: 0x00142540 File Offset: 0x00140740
		public void Enable()
		{
			base.Enable(this.Scene, LightTransitionType.Linear, 1f / this.mSpeed);
		}

		// Token: 0x0600290F RID: 10511 RVA: 0x0014255B File Offset: 0x0014075B
		public void Stop(bool Immediate)
		{
			if (Immediate)
			{
				base.Disable();
				return;
			}
			base.Disable(LightTransitionType.Linear, 1f / this.mSpeed);
		}

		// Token: 0x06002910 RID: 10512 RVA: 0x0014257C File Offset: 0x0014077C
		protected override void Update(DataChannel iDataChannel, float iDeltaTime, ref Vector3 iCameraPosition, ref Vector3 iCameraDirection)
		{
			base.Update(iDataChannel, iDeltaTime, ref iCameraPosition, ref iCameraDirection);
			if (this.mTTL != -1f)
			{
				this.mTTL -= iDeltaTime;
				if (this.mTTL <= 0f)
				{
					this.Stop(false);
				}
			}
			this.mVelocity *= this.mFriction;
			base.Position += this.mVelocity;
		}

		// Token: 0x06002911 RID: 10513 RVA: 0x001425F4 File Offset: 0x001407F4
		protected override void OnRemove()
		{
			base.OnRemove();
			lock (DynamicLight.sLightCache)
			{
				DynamicLight.sLightCache.Enqueue(this);
			}
		}

		// Token: 0x06002912 RID: 10514 RVA: 0x00142638 File Offset: 0x00140838
		public override void Draw(Effect iEffect, DataChannel iDataChannel, float iDeltaTime, Texture2D iNormalMap, Texture2D iDepthMap)
		{
			float mIntensity = this.mIntensity;
			this.mIntensity *= this.mDynamicIntensity;
			base.Draw(iEffect, iDataChannel, iDeltaTime, iNormalMap, iDepthMap);
			this.mIntensity = mIntensity;
		}

		// Token: 0x170009A7 RID: 2471
		// (get) Token: 0x06002913 RID: 10515 RVA: 0x00142673 File Offset: 0x00140873
		public new Scene Scene
		{
			get
			{
				return DynamicLight.sScene.Target as Scene;
			}
		}

		// Token: 0x170009A8 RID: 2472
		// (get) Token: 0x06002914 RID: 10516 RVA: 0x00142684 File Offset: 0x00140884
		// (set) Token: 0x06002915 RID: 10517 RVA: 0x0014268C File Offset: 0x0014088C
		public float Friction
		{
			get
			{
				return this.mFriction;
			}
			set
			{
				this.mFriction = value;
			}
		}

		// Token: 0x170009A9 RID: 2473
		// (get) Token: 0x06002916 RID: 10518 RVA: 0x00142695 File Offset: 0x00140895
		// (set) Token: 0x06002917 RID: 10519 RVA: 0x0014269D File Offset: 0x0014089D
		public Vector3 Velocity
		{
			get
			{
				return this.mVelocity;
			}
			set
			{
				this.mVelocity = value;
			}
		}

		// Token: 0x170009AA RID: 2474
		// (get) Token: 0x06002918 RID: 10520 RVA: 0x001426A6 File Offset: 0x001408A6
		// (set) Token: 0x06002919 RID: 10521 RVA: 0x001426AE File Offset: 0x001408AE
		public float Intensity
		{
			get
			{
				return this.mDynamicIntensity;
			}
			set
			{
				this.mDynamicIntensity = value;
			}
		}

		// Token: 0x170009AB RID: 2475
		// (get) Token: 0x0600291A RID: 10522 RVA: 0x001426B7 File Offset: 0x001408B7
		// (set) Token: 0x0600291B RID: 10523 RVA: 0x001426BF File Offset: 0x001408BF
		public float Speed
		{
			get
			{
				return this.mSpeed;
			}
			set
			{
				if (value <= 1E-45f)
				{
					throw new ArgumentException("iSpeed must be greater than zero!", "iSpeed");
				}
				this.mSpeed = value;
			}
		}

		// Token: 0x04002C67 RID: 11367
		private float mDynamicIntensity;

		// Token: 0x04002C68 RID: 11368
		private float mSpeed = 1f;

		// Token: 0x04002C69 RID: 11369
		private Vector3 mVelocity;

		// Token: 0x04002C6A RID: 11370
		private float mFriction;

		// Token: 0x04002C6B RID: 11371
		private float mTTL = -1f;

		// Token: 0x04002C6C RID: 11372
		private static WeakReference sScene;

		// Token: 0x04002C6D RID: 11373
		private static Queue<DynamicLight> sLightCache;
	}
}
