using System;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka.GameLogic.GameStates
{
	// Token: 0x02000157 RID: 343
	internal class CompanyState : GameState
	{
		// Token: 0x1700022D RID: 557
		// (get) Token: 0x06000A37 RID: 2615 RVA: 0x0003DFF0 File Offset: 0x0003C1F0
		public static CompanyState Instance
		{
			get
			{
				if (CompanyState.mSingelton == null)
				{
					lock (CompanyState.mSingeltonLock)
					{
						if (CompanyState.mSingelton == null)
						{
							CompanyState.mSingelton = new CompanyState();
						}
					}
				}
				return CompanyState.mSingelton;
			}
		}

		// Token: 0x06000A38 RID: 2616 RVA: 0x0003E044 File Offset: 0x0003C244
		public void Initialize()
		{
			Point screenSize = RenderManager.Instance.ScreenSize;
			screenSize.X /= 2;
			screenSize.Y /= 2;
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			this.mContentManager = new ContentManager(Game.Instance.Content.ServiceProvider, "content");
			this.mArrowhead = new RenderableQuad(this.mContentManager.Load<Texture2D>("UI/Company/arrowhead"), graphicsDevice, PivotPosition.CENTER, RenderType.GUI, this.mScene, 2);
			this.mArrowhead.Position = new Vector2((float)screenSize.X, (float)screenSize.Y);
			this.mParadox = new RenderableQuad(this.mContentManager.Load<Texture2D>("UI/Company/paradox"), graphicsDevice, PivotPosition.CENTER, RenderType.GUI, this.mScene, 2);
			this.mParadox.Position = new Vector2((float)screenSize.X, (float)screenSize.Y);
			this.mMagicka = new RenderableQuad(this.mContentManager.Load<Texture2D>("UI/Company/magicka"), graphicsDevice, PivotPosition.CENTER, RenderType.GUI, this.mScene, 2);
			this.mMagicka.Position = new Vector2((float)screenSize.X, (float)screenSize.Y);
			float num = (float)GlobalSettings.Instance.Resolution.Width / this.mMagicka.Width;
			this.mMagicka.Scale = ((num < 1f) ? num : 1f);
			this.mPieces = new RenderableQuad(this.mContentManager.Load<Texture2D>("UI/Company/pieces"), graphicsDevice, PivotPosition.CENTER, RenderType.GUI, this.mScene, 2);
			this.mPieces.Position = new Vector2((float)screenSize.X, (float)screenSize.Y);
			this.mMagicka.Visible = false;
			this.mArrowhead.Visible = false;
			this.mPieces.Visible = false;
			this.mCountDown = 3f;
			this.mCurrentUpdate = new CompanyState.UpdateState(this.UpdateParadox);
		}

		// Token: 0x06000A39 RID: 2617 RVA: 0x0003E234 File Offset: 0x0003C434
		public CompanyState() : base(new Camera(Vector3.Zero, Vector3.Forward, Vector3.Up, MagickCamera.DEFAULTFOV, 1.7777778f, 100f, 500f))
		{
		}

		// Token: 0x06000A3A RID: 2618 RVA: 0x0003E278 File Offset: 0x0003C478
		public override void OnEnter()
		{
			Game.Instance.AddLoadTask(delegate
			{
				Tome instance = Tome.Instance;
			});
			Point screenSize = RenderManager.Instance.ScreenSize;
			screenSize.X /= 2;
			screenSize.Y /= 2;
			this.mArrowhead.Position = new Vector2((float)screenSize.X, (float)screenSize.Y);
			this.mParadox.Position = new Vector2((float)screenSize.X, (float)screenSize.Y);
			this.mMagicka.Position = new Vector2((float)screenSize.X, (float)screenSize.Y);
			this.mPieces.Position = new Vector2((float)screenSize.X, (float)screenSize.Y);
		}

		// Token: 0x06000A3B RID: 2619 RVA: 0x0003E355 File Offset: 0x0003C555
		public override void OnExit()
		{
			this.mContentManager.Dispose();
			ControlManager.Instance.ClearControllers();
			Tome.Instance.SetCameraAnimation(Tome.CameraAnimation.Wake_Up);
			Tome.Instance.LightIntensity = 0f;
		}

		// Token: 0x06000A3C RID: 2620 RVA: 0x0003E386 File Offset: 0x0003C586
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			base.Update(iDataChannel, iDeltaTime);
			this.mCurrentUpdate(iDataChannel, iDeltaTime);
		}

		// Token: 0x06000A3D RID: 2621 RVA: 0x0003E3A0 File Offset: 0x0003C5A0
		private void UpdateParadox(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mParadox.Update(iDataChannel, iDeltaTime);
			this.mCountDown -= iDeltaTime;
			if (this.mCountDown <= 0f || this.mSkip)
			{
				if (this.mParadox.Visible && !this.mSkip)
				{
					this.mParadox.Fade(0.5f);
					return;
				}
				this.mParadox.Visible = false;
				this.mCurrentUpdate = (CompanyState.UpdateState)Delegate.Remove(this.mCurrentUpdate, new CompanyState.UpdateState(this.UpdateParadox));
				this.mCountDown = 3f;
				this.mCurrentUpdate = (CompanyState.UpdateState)Delegate.Combine(this.mCurrentUpdate, new CompanyState.UpdateState(this.UpdateArrowhead));
				this.mArrowhead.Fade(1f);
				this.mSkip = false;
			}
		}

		// Token: 0x06000A3E RID: 2622 RVA: 0x0003E478 File Offset: 0x0003C678
		private void UpdateArrowhead(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mArrowhead.Update(iDataChannel, iDeltaTime);
			this.mCountDown -= iDeltaTime;
			if (this.mCountDown <= 0f || this.mSkip)
			{
				if (this.mArrowhead.Visible && !this.mSkip)
				{
					this.mArrowhead.Fade(0.5f);
					return;
				}
				this.mArrowhead.Visible = false;
				this.mCurrentUpdate = (CompanyState.UpdateState)Delegate.Remove(this.mCurrentUpdate, new CompanyState.UpdateState(this.UpdateArrowhead));
				this.mCountDown = 3f;
				this.mCurrentUpdate = (CompanyState.UpdateState)Delegate.Combine(this.mCurrentUpdate, new CompanyState.UpdateState(this.UpdatePieces));
				this.mPieces.Fade(1f);
				this.mSkip = false;
			}
		}

		// Token: 0x06000A3F RID: 2623 RVA: 0x0003E550 File Offset: 0x0003C750
		private void UpdatePieces(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mPieces.Update(iDataChannel, iDeltaTime);
			this.mCountDown -= iDeltaTime;
			if (this.mCountDown <= 0f || this.mSkip)
			{
				if (this.mPieces.Visible && !this.mSkip)
				{
					this.mPieces.Fade(0.5f);
					return;
				}
				this.mPieces.Visible = false;
				this.mCurrentUpdate = (CompanyState.UpdateState)Delegate.Remove(this.mCurrentUpdate, new CompanyState.UpdateState(this.UpdatePieces));
				this.mCountDown = 3f;
				this.mCurrentUpdate = (CompanyState.UpdateState)Delegate.Combine(this.mCurrentUpdate, new CompanyState.UpdateState(this.UpdateMagicka));
				this.mMagicka.Fade(1f);
				this.mSkip = false;
			}
		}

		// Token: 0x06000A40 RID: 2624 RVA: 0x0003E628 File Offset: 0x0003C828
		private void UpdateMagicka(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mMagicka.Update(iDataChannel, iDeltaTime);
			this.mCountDown -= iDeltaTime;
			if (this.mCountDown <= 0f || this.mSkip)
			{
				if (this.mMagicka.Alpha == 1f)
				{
					if (this.mSkip)
					{
						this.mMagicka.Fade(0.2f);
					}
					else
					{
						this.mMagicka.Fade(1f);
					}
				}
				if (!this.mMagicka.Visible)
				{
					this.StartGame();
				}
			}
		}

		// Token: 0x06000A41 RID: 2625 RVA: 0x0003E6B4 File Offset: 0x0003C8B4
		private void StartGame()
		{
			GameStateManager.Instance.ChangeState(MenuState.Instance);
		}

		// Token: 0x06000A42 RID: 2626 RVA: 0x0003E6C5 File Offset: 0x0003C8C5
		public void SkipScreen()
		{
			this.mSkip = true;
		}

		// Token: 0x0400094B RID: 2379
		private static CompanyState mSingelton;

		// Token: 0x0400094C RID: 2380
		private static volatile object mSingeltonLock = new object();

		// Token: 0x0400094D RID: 2381
		private ContentManager mContentManager;

		// Token: 0x0400094E RID: 2382
		private RenderableQuad mParadox;

		// Token: 0x0400094F RID: 2383
		private RenderableQuad mArrowhead;

		// Token: 0x04000950 RID: 2384
		private RenderableQuad mPieces;

		// Token: 0x04000951 RID: 2385
		private RenderableQuad mMagicka;

		// Token: 0x04000952 RID: 2386
		private float mCountDown;

		// Token: 0x04000953 RID: 2387
		private bool mSkip;

		// Token: 0x04000954 RID: 2388
		private CompanyState.UpdateState mCurrentUpdate;

		// Token: 0x02000158 RID: 344
		// (Invoke) Token: 0x06000A46 RID: 2630
		public delegate void UpdateState(DataChannel iDataChannel, float iDeltaTime);
	}
}
