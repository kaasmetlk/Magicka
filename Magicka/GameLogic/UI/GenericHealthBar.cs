using System;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.UI
{
	// Token: 0x020003F7 RID: 1015
	public class GenericHealthBar
	{
		// Token: 0x06001F0D RID: 7949 RVA: 0x000D94C4 File Offset: 0x000D76C4
		public GenericHealthBar(Scene iScene)
		{
			this.mScene = iScene;
			this.mOnEndTriggerd = true;
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			lock (graphicsDevice)
			{
				this.mEffect = new GUIBasicEffect(graphicsDevice, RenderManager.Instance.GlobalDummyEffect.EffectPool);
				this.mTexture = Game.Instance.Content.Load<Texture2D>("UI/HUD/hud");
				this.mEffect.Texture = this.mTexture;
				this.mAnimatedTexture = Game.Instance.Content.Load<Texture3D>("UI/HUD/counteranimation");
				this.mDisplayNameEffect = new GUIBasicEffect(Game.Instance.GraphicsDevice, null);
			}
			this.mDepth = (float)this.mAnimatedTexture.Depth;
			this.mDepthDivisor = 1f / this.mDepth;
			this.mDepth = this.mDepthDivisor * 0.5f;
			this.mEffect.SetScreenSize(RenderManager.Instance.ScreenSize.X, RenderManager.Instance.ScreenSize.Y);
			this.mEffect.TextureEnabled = true;
			this.mRenderData = new GenericHealthBar.RenderData[3];
			for (int i = 0; i < 3; i++)
			{
				GenericHealthBar.RenderData renderData = new GenericHealthBar.RenderData();
				this.mRenderData[i] = renderData;
				renderData.SetupDisplayName(MagickaFont.Maiandra14);
				renderData.mEffect = this.mEffect;
				renderData.SetText(this.mDisplayName);
				renderData.mDisplayNameEffect = this.mDisplayNameEffect;
			}
			this.mVertexBuffer = new VertexBuffer(Game.Instance.GraphicsDevice, VertexPositionTexture.SizeInBytes * this.mVertices.Length, BufferUsage.WriteOnly);
			this.mVertexDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
			this.mBarTextureOffsets = new Vector2[3];
			this.CreateVertices(this.mHealthBarWidth, (float)this.mTexture.Width, (float)this.mTexture.Height);
			this.CreateAnimatedVertices();
			this.Reset();
		}

		// Token: 0x1700079B RID: 1947
		// (get) Token: 0x06001F0E RID: 7950 RVA: 0x000D9700 File Offset: 0x000D7900
		// (set) Token: 0x06001F0F RID: 7951 RVA: 0x000D9708 File Offset: 0x000D7908
		public Scene Scene
		{
			get
			{
				return this.mScene;
			}
			set
			{
				this.mScene = value;
			}
		}

		// Token: 0x06001F10 RID: 7952 RVA: 0x000D9714 File Offset: 0x000D7914
		public void Reset()
		{
			this.mAlpha = 0f;
			this.mPower = 1f;
			this.mDisplayHealth = 0f;
			this.mNormalizedHealth = 1f;
			this.mActive = false;
			this.mRemove = false;
			this.mTimeTicking = false;
			this.mIsDone = false;
			this.mEndTime = 0f;
			this.mTTL = 0f;
			this.mInitialTimerDelay = 0f;
			if (this.mGraphicsType == GenericHealthBarGraphics.Dynamite)
			{
				for (int i = 0; i < this.mRenderData.Length; i++)
				{
					this.mRenderData[i].mHealthBarPosition -= this.mDynamiteBarOffset;
					this.mRenderData[i].mIsTicking = this.mTimeTicking;
					this.mRenderData[i].mAlpha = this.mAlpha;
					this.mRenderData[i].mNormalizedHealth = this.mNormalizedHealth;
					this.mRenderData[i].mIsDone = this.mIsDone;
				}
			}
		}

		// Token: 0x06001F11 RID: 7953 RVA: 0x000D980C File Offset: 0x000D7A0C
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (!this.mActive)
			{
				return;
			}
			if (this.mIsDone)
			{
				this.OnEndTrigger();
			}
			if (this.mInitialTimerDelay > 0f && !this.mIsPaused)
			{
				this.mInitialTimerDelay -= iDeltaTime;
				if (this.mInitialTimerDelay <= 0f)
				{
					this.TimeTicking = true;
				}
			}
			if (this.mInitialTimerDelay <= 0f && this.mCurrentFadeTime >= this.mFadeTime)
			{
				this.mTest += iDeltaTime;
				if (this.mTest >= 4f)
				{
					this.mTest = 0f;
					float num = (float)GenericHealthBar.sRandom.Next(15);
					if (this.mType == GenericHealthBarTypes.Counter)
					{
						this.mCounterCurrent += num;
					}
					if (this.mType == GenericHealthBarTypes.HP)
					{
						this.mCounterCurrent -= num;
					}
				}
			}
			if (this.mType == GenericHealthBarTypes.TimerDecreasing)
			{
				if (this.mTimeTicking && !this.mIsPaused)
				{
					this.mTTL -= iDeltaTime;
					if (this.mTTL <= this.mEndTime)
					{
						this.IsDone = true;
					}
				}
				this.mDisplayHealth = Math.Max(this.mTTL / this.mStartTime, 0f);
			}
			else if (this.mType == GenericHealthBarTypes.TimerIncreasing)
			{
				if (this.mTimeTicking && !this.mIsPaused)
				{
					this.mTTL += iDeltaTime;
					if (this.mTTL >= this.mEndTime)
					{
						this.IsDone = true;
					}
				}
				this.mDisplayHealth = Math.Min(this.mTTL / this.mEndTime, 1f);
			}
			else if (this.mType == GenericHealthBarTypes.Counter)
			{
				this.mNormalizedHealth = Math.Min(this.mCounterCurrent / this.mCounterEnd, 1f);
				if (this.mCounterCurrent >= this.mCounterEnd)
				{
					this.IsDone = true;
				}
			}
			else
			{
				this.mNormalizedHealth = Math.Max(this.mCounterCurrent / this.mCounterStart, 0f);
				if (this.mCounterCurrent <= this.mCounterEnd)
				{
					this.IsDone = true;
				}
			}
			GenericHealthBar.RenderData renderData = this.mRenderData[(int)iDataChannel];
			this.mCurrentFadeTime += iDeltaTime;
			if (this.mRemove)
			{
				this.mAlpha = Math.Max(1f - this.mCurrentFadeTime / this.mFadeTime, 0f);
			}
			else
			{
				this.mAlpha = Math.Min(this.mCurrentFadeTime / this.mFadeTime, 1f);
			}
			if ((this.mAlpha > 0.99f || this.mRemove) && (this.mType == GenericHealthBarTypes.Counter | this.mType == GenericHealthBarTypes.HP))
			{
				this.mDisplayHealth += (this.mNormalizedHealth - this.mDisplayHealth) * 10f * iDeltaTime;
			}
			renderData.mAlpha = this.mAlpha;
			renderData.mNormalizedHealth = this.mDisplayHealth;
			renderData.mHealthColor = new Vector3(this.mPower, 0f, 0f);
			if (this.mShowDisplayName)
			{
				renderData.mAlphaDisplayName = this.mAlpha;
			}
			if (this.mHasAnimatedSprite)
			{
				if (!this.mIsPaused)
				{
					this.mAnimationTimer += iDeltaTime;
					if (this.mAnimationTimer > this.mDepthDivisor / 2.25f)
					{
						this.mDepth += this.mDepthDivisor;
						this.mAnimationTimer -= this.mDepthDivisor / 2.25f;
					}
				}
				renderData.Saturate = !this.mActive;
				renderData.AnimationSpriteDepth = this.mDepth;
				renderData.AnimationSpritePosition.X = (float)(RenderManager.Instance.ScreenSize.X / 2) - this.mHealthBarWidth * (float)RenderManager.Instance.ScreenSize.X / 2f + this.mDisplayHealth * (this.mHealthBarWidth * (float)RenderManager.Instance.ScreenSize.X - 40f * this.mHealthBarWidth) + 40f * this.mHealthBarWidth + 32f;
				renderData.AnimationSpritePosition.Y = this.mAnimationSpriteOffsetY;
			}
			this.mScene.AddRenderableGUIObject(iDataChannel, renderData);
		}

		// Token: 0x06001F12 RID: 7954 RVA: 0x000D9C13 File Offset: 0x000D7E13
		public void SetWidth(float iHealthbarWidth)
		{
			this.CreateVertices(iHealthbarWidth, (float)this.mTexture.Width, (float)this.mTexture.Height);
		}

		// Token: 0x06001F13 RID: 7955 RVA: 0x000D9C34 File Offset: 0x000D7E34
		protected void CreateVertices(float iHealthbarWidth, float iTextureWidth, float iTextureHeight)
		{
			int width = GlobalSettings.Instance.Resolution.Width;
			int num = width / 2;
			int num2 = (int)(iHealthbarWidth * (float)width);
			int num3 = num2 / 2;
			this.mVertices[0].Position.X = (float)(num - num3);
			this.mVertices[0].Position.Y = 32f;
			this.mVertices[0].TextureCoordinate.X = 0f / iTextureWidth;
			this.mVertices[0].TextureCoordinate.Y = 24f / iTextureHeight;
			this.mVertices[1].Position.X = (float)(num - num3);
			this.mVertices[1].Position.Y = 8f;
			this.mVertices[1].TextureCoordinate.X = 0f / iTextureWidth;
			this.mVertices[1].TextureCoordinate.Y = 0f / iTextureHeight;
			this.mVertices[2].Position.X = (float)(num - num3 + 96);
			this.mVertices[2].Position.Y = 32f;
			this.mVertices[2].TextureCoordinate.X = 96f / iTextureWidth;
			this.mVertices[2].TextureCoordinate.Y = 24f / iTextureHeight;
			this.mVertices[3].Position.X = (float)(num - num3 + 96);
			this.mVertices[3].Position.Y = 8f;
			this.mVertices[3].TextureCoordinate.X = 96f / iTextureWidth;
			this.mVertices[3].TextureCoordinate.Y = 0f / iTextureHeight;
			this.mVertices[4].Position.X = (float)(num + num3 - 96);
			this.mVertices[4].Position.Y = 32f;
			this.mVertices[4].TextureCoordinate.X = 160f / iTextureWidth;
			this.mVertices[4].TextureCoordinate.Y = 24f / iTextureHeight;
			this.mVertices[5].Position.X = (float)(num + num3 - 96);
			this.mVertices[5].Position.Y = 8f;
			this.mVertices[5].TextureCoordinate.X = 160f / iTextureWidth;
			this.mVertices[5].TextureCoordinate.Y = 0f / iTextureHeight;
			this.mVertices[6].Position.X = (float)(num + num3);
			this.mVertices[6].Position.Y = 32f;
			this.mVertices[6].TextureCoordinate.X = 256f / iTextureWidth;
			this.mVertices[6].TextureCoordinate.Y = 24f / iTextureHeight;
			this.mVertices[7].Position.X = (float)(num + num3);
			this.mVertices[7].Position.Y = 8f;
			this.mVertices[7].TextureCoordinate.X = 256f / iTextureWidth;
			this.mVertices[7].TextureCoordinate.Y = 0f / iTextureHeight;
			this.mVertices[8].Position.X = 0f;
			this.mVertices[8].Position.Y = 32f;
			this.mVertices[8].TextureCoordinate.X = 0f / iTextureWidth;
			this.mVertices[8].TextureCoordinate.Y = 24f / iTextureHeight;
			this.mVertices[9].Position.X = 0f;
			this.mVertices[9].Position.Y = 8f;
			this.mVertices[9].TextureCoordinate.X = 0f / iTextureWidth;
			this.mVertices[9].TextureCoordinate.Y = 0f / iTextureHeight;
			this.mVertices[10].Position.X = (float)(num2 - 32);
			this.mVertices[10].Position.Y = 32f;
			this.mVertices[10].TextureCoordinate.X = 256f / iTextureWidth;
			this.mVertices[10].TextureCoordinate.Y = 24f / iTextureHeight;
			this.mVertices[11].Position.X = (float)(num2 - 32);
			this.mVertices[11].Position.Y = 8f;
			this.mVertices[11].TextureCoordinate.X = 256f / iTextureWidth;
			this.mVertices[11].TextureCoordinate.Y = 0f / iTextureHeight;
			this.mVertices[12].Position.X = (float)(num - num3);
			this.mVertices[12].Position.Y = 32f;
			this.mVertices[12].TextureCoordinate.X = 0f / iTextureWidth;
			this.mVertices[12].TextureCoordinate.Y = 24f / iTextureHeight;
			this.mVertices[13].Position.X = (float)(num - num3);
			this.mVertices[13].Position.Y = 8f;
			this.mVertices[13].TextureCoordinate.X = 0f / iTextureWidth;
			this.mVertices[13].TextureCoordinate.Y = 0f / iTextureHeight;
			this.mVertices[14].Position.X = (float)(num - num3 + 96);
			this.mVertices[14].Position.Y = 32f;
			this.mVertices[14].TextureCoordinate.X = 96f / iTextureWidth;
			this.mVertices[14].TextureCoordinate.Y = 24f / iTextureHeight;
			this.mVertices[15].Position.X = (float)(num - num3 + 96);
			this.mVertices[15].Position.Y = 8f;
			this.mVertices[15].TextureCoordinate.X = 96f / iTextureWidth;
			this.mVertices[15].TextureCoordinate.Y = 0f / iTextureHeight;
			this.mVertices[16].Position.X = (float)(num + num3 - 96);
			this.mVertices[16].Position.Y = 32f;
			this.mVertices[16].TextureCoordinate.X = 160f / iTextureWidth;
			this.mVertices[16].TextureCoordinate.Y = 24f / iTextureHeight;
			this.mVertices[17].Position.X = (float)(num + num3 - 96);
			this.mVertices[17].Position.Y = 8f;
			this.mVertices[17].TextureCoordinate.X = 160f / iTextureWidth;
			this.mVertices[17].TextureCoordinate.Y = 0f / iTextureHeight;
			this.mVertices[18].Position.X = (float)(num + num3);
			this.mVertices[18].Position.Y = 32f;
			this.mVertices[18].TextureCoordinate.X = 256f / iTextureWidth;
			this.mVertices[18].TextureCoordinate.Y = 24f / iTextureHeight;
			this.mVertices[19].Position.X = (float)(num + num3);
			this.mVertices[19].Position.Y = 8f;
			this.mVertices[19].TextureCoordinate.X = 256f / iTextureWidth;
			this.mVertices[19].TextureCoordinate.Y = 0f / iTextureHeight;
			this.mVertexBuffer.SetData<VertexPositionTexture>(this.mVertices);
			for (int i = 0; i < 3; i++)
			{
				this.mRenderData[i].mVertices = this.mVertexBuffer;
				this.mRenderData[i].mVertexDeclaration = this.mVertexDeclaration;
				this.mRenderData[i].mHealthBarPosition = (float)(num - num3 + 16);
			}
		}

		// Token: 0x06001F14 RID: 7956 RVA: 0x000DA5B4 File Offset: 0x000D87B4
		public void CreateAnimatedVertices()
		{
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			VertexPositionTexture[] array = new VertexPositionTexture[4];
			array[0].Position = new Vector3(-32f, 32f, 0f);
			array[0].TextureCoordinate = new Vector2(0f, 1f);
			array[1].Position = new Vector3(-32f, -32f, 0f);
			array[1].TextureCoordinate = new Vector2(0f, 0f);
			array[2].Position = new Vector3(32f, -32f, 0f);
			array[2].TextureCoordinate = new Vector2(1f, 0f);
			array[3].Position = new Vector3(32f, 32f, 0f);
			array[3].TextureCoordinate = new Vector2(1f, 1f);
			VertexBuffer vertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionTexture.SizeInBytes * 4, BufferUsage.WriteOnly);
			lock (graphicsDevice)
			{
				vertexBuffer.SetData<VertexPositionTexture>(array);
			}
			VertexDeclaration mAnimationSpriteVertexDeclaration = new VertexDeclaration(graphicsDevice, VertexPositionTexture.VertexElements);
			GUIBasicEffect mAnimationSpriteEffect = new GUIBasicEffect(graphicsDevice, null);
			for (int i = 0; i < 3; i++)
			{
				this.mRenderData[i].mAnimationSpriteEffect = mAnimationSpriteEffect;
				this.mRenderData[i].mAnimationSpriteVertexBuffer = vertexBuffer;
				this.mRenderData[i].mAnimationSpriteVertexDeclaration = mAnimationSpriteVertexDeclaration;
				this.mRenderData[i].mAnimationSpriteTexture = this.mAnimatedTexture;
			}
		}

		// Token: 0x1700079C RID: 1948
		// (get) Token: 0x06001F15 RID: 7957 RVA: 0x000DA760 File Offset: 0x000D8960
		// (set) Token: 0x06001F16 RID: 7958 RVA: 0x000DA768 File Offset: 0x000D8968
		public float Alpha
		{
			get
			{
				return this.mAlpha;
			}
			set
			{
				this.mAlpha = value;
			}
		}

		// Token: 0x1700079D RID: 1949
		// (get) Token: 0x06001F17 RID: 7959 RVA: 0x000DA771 File Offset: 0x000D8971
		// (set) Token: 0x06001F18 RID: 7960 RVA: 0x000DA779 File Offset: 0x000D8979
		public float Power
		{
			get
			{
				return this.mPower;
			}
			set
			{
				this.mPower = value;
			}
		}

		// Token: 0x06001F19 RID: 7961 RVA: 0x000DA782 File Offset: 0x000D8982
		public void SetNormalizedHealth(float iPercent)
		{
			this.mNormalizedHealth = iPercent;
		}

		// Token: 0x06001F1A RID: 7962 RVA: 0x000DA78B File Offset: 0x000D898B
		public void Remove()
		{
			this.mRemove = true;
			this.TimeTicking = false;
			this.mCurrentFadeTime = 0f;
			this.HasAnimatedSprite = false;
		}

		// Token: 0x06001F1B RID: 7963 RVA: 0x000DA7B0 File Offset: 0x000D89B0
		public void Activate()
		{
			this.mActive = true;
			if ((this.mType == GenericHealthBarTypes.TimerDecreasing | this.mType == GenericHealthBarTypes.TimerIncreasing) && this.mInitialTimerDelay <= 0f)
			{
				this.TimeTicking = true;
			}
			for (int i = 0; i < 3; i++)
			{
				this.mRenderData[i].mHealthBarBackgroundTextureOffset = this.mBarTextureOffsets[0];
				this.mRenderData[i].mHealthBarTextureOffset = this.mBarTextureOffsets[1];
				this.mRenderData[i].mHealthBarOverlayTextureOffset = this.mBarTextureOffsets[2];
				if (this.mGraphicsType == GenericHealthBarGraphics.Dynamite)
				{
					this.mRenderData[i].mHealthBarPosition += this.mDynamiteBarOffset;
				}
			}
			this.mIsPaused = false;
		}

		// Token: 0x1700079E RID: 1950
		// (get) Token: 0x06001F1C RID: 7964 RVA: 0x000DA883 File Offset: 0x000D8A83
		// (set) Token: 0x06001F1D RID: 7965 RVA: 0x000DA88B File Offset: 0x000D8A8B
		public GenericHealthBarTypes Type
		{
			get
			{
				return this.mType;
			}
			set
			{
				this.mType = value;
			}
		}

		// Token: 0x1700079F RID: 1951
		// (get) Token: 0x06001F1E RID: 7966 RVA: 0x000DA894 File Offset: 0x000D8A94
		// (set) Token: 0x06001F1F RID: 7967 RVA: 0x000DA89C File Offset: 0x000D8A9C
		public float NormalizedHealth
		{
			get
			{
				return this.mNormalizedHealth;
			}
			set
			{
				this.mNormalizedHealth = value;
			}
		}

		// Token: 0x170007A0 RID: 1952
		// (get) Token: 0x06001F20 RID: 7968 RVA: 0x000DA8A5 File Offset: 0x000D8AA5
		// (set) Token: 0x06001F21 RID: 7969 RVA: 0x000DA8AD File Offset: 0x000D8AAD
		public float DisplayHealth
		{
			get
			{
				return this.mDisplayHealth;
			}
			set
			{
				this.mDisplayHealth = value;
			}
		}

		// Token: 0x170007A1 RID: 1953
		// (get) Token: 0x06001F22 RID: 7970 RVA: 0x000DA8B6 File Offset: 0x000D8AB6
		// (set) Token: 0x06001F23 RID: 7971 RVA: 0x000DA8BE File Offset: 0x000D8ABE
		public float TTL
		{
			get
			{
				return this.mTTL;
			}
			set
			{
				this.mTTL = value;
			}
		}

		// Token: 0x170007A2 RID: 1954
		// (get) Token: 0x06001F24 RID: 7972 RVA: 0x000DA8C7 File Offset: 0x000D8AC7
		// (set) Token: 0x06001F25 RID: 7973 RVA: 0x000DA8CF File Offset: 0x000D8ACF
		public float StartTime
		{
			get
			{
				return this.mStartTime;
			}
			set
			{
				this.mStartTime = value;
			}
		}

		// Token: 0x170007A3 RID: 1955
		// (get) Token: 0x06001F26 RID: 7974 RVA: 0x000DA8D8 File Offset: 0x000D8AD8
		// (set) Token: 0x06001F27 RID: 7975 RVA: 0x000DA8E0 File Offset: 0x000D8AE0
		public float EndTime
		{
			get
			{
				return this.mEndTime;
			}
			set
			{
				this.mEndTime = value;
			}
		}

		// Token: 0x06001F28 RID: 7976 RVA: 0x000DA8EC File Offset: 0x000D8AEC
		public void SetupTimer(float iTime)
		{
			if (this.mType == GenericHealthBarTypes.TimerDecreasing)
			{
				this.mStartTime = iTime;
				this.mTTL = iTime;
				this.mEndTime = 0f;
				return;
			}
			this.mStartTime = 0f;
			this.mTTL = 0f;
			this.mEndTime = iTime;
		}

		// Token: 0x170007A4 RID: 1956
		// (get) Token: 0x06001F29 RID: 7977 RVA: 0x000DA939 File Offset: 0x000D8B39
		// (set) Token: 0x06001F2A RID: 7978 RVA: 0x000DA941 File Offset: 0x000D8B41
		public float CounterCurrent
		{
			get
			{
				return this.mCounterCurrent;
			}
			set
			{
				this.mCounterCurrent = value;
			}
		}

		// Token: 0x170007A5 RID: 1957
		// (get) Token: 0x06001F2B RID: 7979 RVA: 0x000DA94A File Offset: 0x000D8B4A
		// (set) Token: 0x06001F2C RID: 7980 RVA: 0x000DA952 File Offset: 0x000D8B52
		public float CounterStart
		{
			get
			{
				return this.mCounterStart;
			}
			set
			{
				this.mCounterStart = value;
			}
		}

		// Token: 0x170007A6 RID: 1958
		// (get) Token: 0x06001F2D RID: 7981 RVA: 0x000DA95B File Offset: 0x000D8B5B
		// (set) Token: 0x06001F2E RID: 7982 RVA: 0x000DA963 File Offset: 0x000D8B63
		public float CounterEnd
		{
			get
			{
				return this.mCounterEnd;
			}
			set
			{
				this.mCounterEnd = value;
			}
		}

		// Token: 0x06001F2F RID: 7983 RVA: 0x000DA96C File Offset: 0x000D8B6C
		public void SetupCounter(float iCounterCount)
		{
			if (this.mType == GenericHealthBarTypes.Counter)
			{
				this.mCounterStart = 0f;
				this.mCounterCurrent = 0f;
				this.mCounterEnd = iCounterCount;
				return;
			}
			this.mCounterStart = iCounterCount;
			this.mCounterCurrent = iCounterCount;
			this.mCounterEnd = 0f;
		}

		// Token: 0x170007A7 RID: 1959
		// (get) Token: 0x06001F30 RID: 7984 RVA: 0x000DA9B9 File Offset: 0x000D8BB9
		// (set) Token: 0x06001F31 RID: 7985 RVA: 0x000DA9C4 File Offset: 0x000D8BC4
		public bool IsDone
		{
			get
			{
				return this.mIsDone;
			}
			set
			{
				this.mIsDone = value;
				this.mRenderData[0].mIsDone = this.mIsDone;
				this.mRenderData[1].mIsDone = this.mIsDone;
				this.mRenderData[2].mIsDone = this.mIsDone;
			}
		}

		// Token: 0x170007A8 RID: 1960
		// (get) Token: 0x06001F32 RID: 7986 RVA: 0x000DAA11 File Offset: 0x000D8C11
		// (set) Token: 0x06001F33 RID: 7987 RVA: 0x000DAA19 File Offset: 0x000D8C19
		public GenericHealthBarPosition BarPosition
		{
			get
			{
				return this.mBarPosition;
			}
			set
			{
				this.mBarPosition = value;
			}
		}

		// Token: 0x170007A9 RID: 1961
		// (get) Token: 0x06001F34 RID: 7988 RVA: 0x000DAA22 File Offset: 0x000D8C22
		// (set) Token: 0x06001F35 RID: 7989 RVA: 0x000DAA2A File Offset: 0x000D8C2A
		public float InitialTimerDelay
		{
			get
			{
				return this.mInitialTimerDelay;
			}
			set
			{
				this.mInitialTimerDelay = value;
			}
		}

		// Token: 0x170007AA RID: 1962
		// (get) Token: 0x06001F36 RID: 7990 RVA: 0x000DAA33 File Offset: 0x000D8C33
		// (set) Token: 0x06001F37 RID: 7991 RVA: 0x000DAA3B File Offset: 0x000D8C3B
		public bool IsPaused
		{
			get
			{
				return this.mIsPaused;
			}
			set
			{
				this.mIsPaused = value;
			}
		}

		// Token: 0x170007AB RID: 1963
		// (get) Token: 0x06001F38 RID: 7992 RVA: 0x000DAA44 File Offset: 0x000D8C44
		// (set) Token: 0x06001F39 RID: 7993 RVA: 0x000DAA4C File Offset: 0x000D8C4C
		public string DisplayName
		{
			get
			{
				return this.mDisplayName;
			}
			set
			{
				this.mDisplayName = value;
				this.mShowDisplayName = true;
				for (int i = 0; i < 3; i++)
				{
					this.mRenderData[i].SetText(this.mDisplayName);
					this.mRenderData[i].mShowDisplayName = true;
				}
			}
		}

		// Token: 0x170007AC RID: 1964
		// (get) Token: 0x06001F3A RID: 7994 RVA: 0x000DAA94 File Offset: 0x000D8C94
		// (set) Token: 0x06001F3B RID: 7995 RVA: 0x000DAA9C File Offset: 0x000D8C9C
		public bool ShowDisplayName
		{
			get
			{
				return this.mShowDisplayName;
			}
			set
			{
				this.mShowDisplayName = value;
				for (int i = 0; i < 3; i++)
				{
					this.mRenderData[i].mShowDisplayName = this.mShowDisplayName;
				}
			}
		}

		// Token: 0x170007AD RID: 1965
		// (get) Token: 0x06001F3C RID: 7996 RVA: 0x000DAACF File Offset: 0x000D8CCF
		// (set) Token: 0x06001F3D RID: 7997 RVA: 0x000DAAD8 File Offset: 0x000D8CD8
		public bool IsScaled
		{
			get
			{
				return this.mIsScaled;
			}
			set
			{
				this.mIsScaled = value;
				this.mRenderData[0].mIsScaled = this.mIsScaled;
				this.mRenderData[1].mIsScaled = this.mIsScaled;
				this.mRenderData[2].mIsScaled = this.mIsScaled;
			}
		}

		// Token: 0x170007AE RID: 1966
		// (get) Token: 0x06001F3E RID: 7998 RVA: 0x000DAB25 File Offset: 0x000D8D25
		// (set) Token: 0x06001F3F RID: 7999 RVA: 0x000DAB30 File Offset: 0x000D8D30
		public bool IsColoredRed
		{
			get
			{
				return this.mIsColoredRed;
			}
			set
			{
				this.mIsColoredRed = value;
				this.mRenderData[0].mIsColored = this.mIsColoredRed;
				this.mRenderData[1].mIsColored = this.mIsColoredRed;
				this.mRenderData[2].mIsColored = this.mIsColoredRed;
			}
		}

		// Token: 0x170007AF RID: 1967
		// (get) Token: 0x06001F40 RID: 8000 RVA: 0x000DAB7D File Offset: 0x000D8D7D
		// (set) Token: 0x06001F41 RID: 8001 RVA: 0x000DAB88 File Offset: 0x000D8D88
		public GenericHealthBarGraphics GraphicsType
		{
			get
			{
				return this.mGraphicsType;
			}
			set
			{
				this.mGraphicsType = value;
				if (this.mGraphicsType == GenericHealthBarGraphics.Dynamite)
				{
					this.mBarTextureOffsets[0].X = 72f / (float)this.mTexture.Width;
					this.mBarTextureOffsets[0].Y = 392f / (float)this.mTexture.Height;
					this.mBarTextureOffsets[1].X = 128f / (float)this.mTexture.Width;
					this.mBarTextureOffsets[1].Y = 360f / (float)this.mTexture.Height;
					this.mBarTextureOffsets[2].X = 0f / (float)this.mTexture.Width;
					this.mBarTextureOffsets[2].Y = 450f / (float)this.mTexture.Height;
					return;
				}
				this.mBarTextureOffsets[0].X = 0f / (float)this.mTexture.Width;
				this.mBarTextureOffsets[0].Y = 48f / (float)this.mTexture.Height;
				this.mBarTextureOffsets[1].X = 128f / (float)this.mTexture.Width;
				this.mBarTextureOffsets[1].Y = 424f / (float)this.mTexture.Height;
				this.mBarTextureOffsets[2].X = 0f / (float)this.mTexture.Width;
				this.mBarTextureOffsets[2].Y = 24f / (float)this.mTexture.Height;
			}
		}

		// Token: 0x170007B0 RID: 1968
		// (get) Token: 0x06001F42 RID: 8002 RVA: 0x000DAD4D File Offset: 0x000D8F4D
		// (set) Token: 0x06001F43 RID: 8003 RVA: 0x000DAD58 File Offset: 0x000D8F58
		public bool HasAnimatedSprite
		{
			get
			{
				return this.mHasAnimatedSprite;
			}
			set
			{
				this.mHasAnimatedSprite = value;
				this.mRenderData[0].mHasAnimatedSprite = this.mHasAnimatedSprite;
				this.mRenderData[1].mHasAnimatedSprite = this.mHasAnimatedSprite;
				this.mRenderData[2].mHasAnimatedSprite = this.mHasAnimatedSprite;
			}
		}

		// Token: 0x170007B1 RID: 1969
		// (get) Token: 0x06001F44 RID: 8004 RVA: 0x000DADA5 File Offset: 0x000D8FA5
		// (set) Token: 0x06001F45 RID: 8005 RVA: 0x000DADB0 File Offset: 0x000D8FB0
		public bool TimeTicking
		{
			get
			{
				return this.mTimeTicking;
			}
			set
			{
				this.mTimeTicking = value;
				this.mRenderData[0].mIsTicking = this.mTimeTicking;
				this.mRenderData[1].mIsTicking = this.mTimeTicking;
				this.mRenderData[2].mIsTicking = this.mTimeTicking;
			}
		}

		// Token: 0x170007B2 RID: 1970
		// (get) Token: 0x06001F46 RID: 8006 RVA: 0x000DADFD File Offset: 0x000D8FFD
		// (set) Token: 0x06001F47 RID: 8007 RVA: 0x000DAE05 File Offset: 0x000D9005
		public float AnimationSpriteOffsetY
		{
			get
			{
				return this.mAnimationSpriteOffsetY;
			}
			set
			{
				this.mAnimationSpriteOffsetY = value;
			}
		}

		// Token: 0x170007B3 RID: 1971
		// (get) Token: 0x06001F48 RID: 8008 RVA: 0x000DAE0E File Offset: 0x000D900E
		// (set) Token: 0x06001F49 RID: 8009 RVA: 0x000DAE16 File Offset: 0x000D9016
		public float FadeTime
		{
			get
			{
				return this.mFadeTime;
			}
			set
			{
				this.mFadeTime = value;
			}
		}

		// Token: 0x170007B4 RID: 1972
		// (get) Token: 0x06001F4A RID: 8010 RVA: 0x000DAE1F File Offset: 0x000D901F
		// (set) Token: 0x06001F4B RID: 8011 RVA: 0x000DAE27 File Offset: 0x000D9027
		public int OnEndTriggerID
		{
			get
			{
				return this.mOnEndTriggerID;
			}
			set
			{
				this.mOnEndTriggerID = value;
				if (this.mOnEndTriggerID != 0)
				{
					this.mOnEndTriggerd = false;
				}
			}
		}

		// Token: 0x06001F4C RID: 8012 RVA: 0x000DAE3F File Offset: 0x000D903F
		public void OnEndTrigger()
		{
			if (this.mOnEndTriggerd)
			{
				return;
			}
			this.mOnEndTriggerd = true;
			(GameStateManager.Instance.CurrentState as PlayState).Level.CurrentScene.ExecuteTrigger(this.mOnEndTriggerID, null, false);
		}

		// Token: 0x06001F4D RID: 8013 RVA: 0x000DAE78 File Offset: 0x000D9078
		public void UpdateResolution()
		{
			this.CreateVertices(this.mHealthBarWidth, (float)this.mTexture.Width, (float)this.mTexture.Height);
			this.CreateAnimatedVertices();
			for (int i = 0; i < 3; i++)
			{
				this.mRenderData[i].mHealthBarBackgroundTextureOffset = this.mBarTextureOffsets[0];
				this.mRenderData[i].mHealthBarTextureOffset = this.mBarTextureOffsets[1];
				this.mRenderData[i].mHealthBarOverlayTextureOffset = this.mBarTextureOffsets[2];
				if (this.mGraphicsType == GenericHealthBarGraphics.Dynamite)
				{
					this.mRenderData[i].mHealthBarPosition += this.mDynamiteBarOffset;
				}
			}
		}

		// Token: 0x04002170 RID: 8560
		public const int HEALTHBARSIDESIZE = 96;

		// Token: 0x04002171 RID: 8561
		public const float COUNTER_MIN = 0f;

		// Token: 0x04002172 RID: 8562
		public const float COUNTER_MAX = 1f;

		// Token: 0x04002173 RID: 8563
		private GUIBasicEffect mEffect;

		// Token: 0x04002174 RID: 8564
		private GUIBasicEffect mDisplayNameEffect;

		// Token: 0x04002175 RID: 8565
		private VertexBuffer mVertexBuffer;

		// Token: 0x04002176 RID: 8566
		private VertexDeclaration mVertexDeclaration;

		// Token: 0x04002177 RID: 8567
		private VertexPositionTexture[] mVertices = new VertexPositionTexture[20];

		// Token: 0x04002178 RID: 8568
		private Scene mScene;

		// Token: 0x04002179 RID: 8569
		private GenericHealthBar.RenderData[] mRenderData;

		// Token: 0x0400217A RID: 8570
		private Vector2[] mBarTextureOffsets;

		// Token: 0x0400217B RID: 8571
		private bool mActive;

		// Token: 0x0400217C RID: 8572
		private bool mRemove;

		// Token: 0x0400217D RID: 8573
		private float mAlpha;

		// Token: 0x0400217E RID: 8574
		private float mPower;

		// Token: 0x0400217F RID: 8575
		private float mNormalizedHealth;

		// Token: 0x04002180 RID: 8576
		private float mDisplayHealth;

		// Token: 0x04002181 RID: 8577
		private Texture2D mTexture;

		// Token: 0x04002182 RID: 8578
		private Texture3D mAnimatedTexture;

		// Token: 0x04002183 RID: 8579
		private GenericHealthBarTypes mType;

		// Token: 0x04002184 RID: 8580
		private GenericHealthBarGraphics mGraphicsType;

		// Token: 0x04002185 RID: 8581
		private GenericHealthBarPosition mBarPosition;

		// Token: 0x04002186 RID: 8582
		private float mStartTime;

		// Token: 0x04002187 RID: 8583
		private float mEndTime;

		// Token: 0x04002188 RID: 8584
		private float mTTL;

		// Token: 0x04002189 RID: 8585
		private float mInitialTimerDelay;

		// Token: 0x0400218A RID: 8586
		private bool mTimeTicking;

		// Token: 0x0400218B RID: 8587
		private bool mIsDone;

		// Token: 0x0400218C RID: 8588
		private float mCounterStart;

		// Token: 0x0400218D RID: 8589
		private float mCounterEnd;

		// Token: 0x0400218E RID: 8590
		private float mCounterCurrent;

		// Token: 0x0400218F RID: 8591
		private float mAnimationTimer;

		// Token: 0x04002190 RID: 8592
		private float mDepthDivisor;

		// Token: 0x04002191 RID: 8593
		private float mDepth;

		// Token: 0x04002192 RID: 8594
		private float mHealthBarWidth = 0.8f;

		// Token: 0x04002193 RID: 8595
		private string mDisplayName = "";

		// Token: 0x04002194 RID: 8596
		private bool mShowDisplayName;

		// Token: 0x04002195 RID: 8597
		private bool mIsPaused = true;

		// Token: 0x04002196 RID: 8598
		private bool mIsScaled;

		// Token: 0x04002197 RID: 8599
		private bool mIsColoredRed;

		// Token: 0x04002198 RID: 8600
		private bool mHasAnimatedSprite;

		// Token: 0x04002199 RID: 8601
		private float mAnimationSpriteOffsetY;

		// Token: 0x0400219A RID: 8602
		private float mFadeTime;

		// Token: 0x0400219B RID: 8603
		private float mCurrentFadeTime = 0.01f;

		// Token: 0x0400219C RID: 8604
		private int mOnEndTriggerID;

		// Token: 0x0400219D RID: 8605
		private bool mOnEndTriggerd;

		// Token: 0x0400219E RID: 8606
		private float mDynamiteBarOffset = 40f;

		// Token: 0x0400219F RID: 8607
		private static Random sRandom = new Random();

		// Token: 0x040021A0 RID: 8608
		private float mTest;

		// Token: 0x020003F8 RID: 1016
		protected class RenderData : IRenderableGUIObject
		{
			// Token: 0x06001F4F RID: 8015 RVA: 0x000DAF4C File Offset: 0x000D914C
			public void SetText(string iDisplayName)
			{
				if (iDisplayName == "" || iDisplayName == null)
				{
					return;
				}
				this.mDisplayNameText.Font = this.mFont;
				this.mDisplayNameHeight = this.mFont.MeasureText(iDisplayName, true).Y;
				this.mDisplayName = iDisplayName;
				this.mDisplayNameIsDirty = true;
			}

			// Token: 0x06001F50 RID: 8016 RVA: 0x000DAFA4 File Offset: 0x000D91A4
			public void SetupDisplayName(MagickaFont iFont)
			{
				this.mFont = FontManager.Instance.GetFont(iFont);
				this.mDisplayNameText = new Text(100, this.mFont, TextAlign.Center, false);
				this.mDisplayNameText.DrawShadows = true;
				this.mDisplayNameText.ShadowsOffset = new Vector2(2f, 2f);
				this.mDisplayNameText.ShadowAlpha = 1f;
			}

			// Token: 0x06001F51 RID: 8017 RVA: 0x000DB010 File Offset: 0x000D9210
			public void Draw(float iDeltaTime)
			{
				this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertices, 0, VertexPositionTexture.SizeInBytes);
				this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
				this.mEffect.SetScreenSize(RenderManager.Instance.ScreenSize.X, RenderManager.Instance.ScreenSize.Y);
				this.mEffect.Begin();
				EffectPassCollection passes = this.mEffect.CurrentTechnique.Passes;
				for (int i = 0; i < passes.Count; i++)
				{
					passes[i].Begin();
					this.mEffect.Transform = Matrix.Identity;
					Vector4 color = default(Vector4);
					color.X = 1f;
					color.Y = 1f;
					color.Z = 1f;
					color.W = this.mAlpha;
					this.mEffect.Color = color;
					this.mEffect.TextureOffset = this.mHealthBarBackgroundTextureOffset;
					this.mEffect.CommitChanges();
					this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 6);
					if (this.mIsScaled)
					{
						this.mHealthBarScale.X = this.mNormalizedHealth;
					}
					Matrix transform = default(Matrix);
					transform.M11 = this.mNormalizedHealth;
					transform.M41 = this.mHealthBarPosition;
					transform.M22 = 1f;
					transform.M33 = 1f;
					transform.M44 = 1f;
					this.mEffect.Transform = transform;
					this.mEffect.TextureOffset = this.mHealthBarTextureOffset;
					this.mHealthBarStandardScale = this.mEffect.TextureScale;
					this.mEffect.TextureScale = this.mHealthBarScale * this.mHealthBarStandardScale;
					if (this.mIsColored)
					{
						color.X = this.mHealthColor.X;
						color.Y = this.mHealthColor.Y;
						color.Z = this.mHealthColor.Z;
					}
					color.W = this.mAlpha;
					this.mEffect.Color = color;
					this.mEffect.CommitChanges();
					this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 8, 2);
					this.mEffect.Transform = Matrix.Identity;
					color.X = 1f;
					color.Y = 1f;
					color.Z = 1f;
					color.W = this.mAlpha;
					this.mEffect.TextureOffset = this.mHealthBarOverlayTextureOffset;
					this.mEffect.TextureScale = this.mHealthBarStandardScale;
					this.mEffect.Color = color;
					this.mEffect.CommitChanges();
					this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 12, 6);
					passes[i].End();
				}
				this.mEffect.End();
				if (this.mShowDisplayName)
				{
					if (this.mDisplayNameIsDirty)
					{
						this.mDisplayNameText.SetText(this.mDisplayName);
						this.mDisplayNameIsDirty = false;
					}
					Point screenSize = RenderManager.Instance.ScreenSize;
					this.mDisplayNameEffect.Color = new Vector4(1f, 1f, 1f, this.mAlphaDisplayName);
					this.mDisplayNameEffect.SetScreenSize(screenSize.X, screenSize.Y);
					this.mDisplayNameEffect.TextureEnabled = true;
					this.mDisplayNameEffect.Begin();
					this.mDisplayNameEffect.CurrentTechnique.Passes[0].Begin();
					this.mDisplayNameText.Draw(this.mDisplayNameEffect, (float)screenSize.X * 0.5f + 0.5f, 8f);
					this.mDisplayNameEffect.CurrentTechnique.Passes[0].End();
					this.mDisplayNameEffect.End();
				}
				if (this.mHasAnimatedSprite && this.mIsTicking && !this.mIsDone)
				{
					Point screenSize2 = RenderManager.Instance.ScreenSize;
					this.mTransform.M41 = this.AnimationSpritePosition.X;
					this.mTransform.M42 = this.AnimationSpritePosition.Y;
					this.mAnimationSpriteEffect.Transform = this.mTransform;
					this.mAnimationSpriteEffect.SetScreenSize(screenSize2.X, screenSize2.Y);
					this.mAnimationSpriteEffect.SetTechnique(GUIBasicEffect.Technique.Texture3D);
					this.mAnimationSpriteEffect.W = this.AnimationSpriteDepth;
					this.mAnimationSpriteEffect.Color = (this.Saturate ? GenericHealthBar.RenderData.SATURATED_COLOR : GenericHealthBar.RenderData.COLOR);
					this.mAnimationSpriteEffect.VertexColorEnabled = false;
					this.mAnimationSpriteEffect.TextureEnabled = true;
					this.mAnimationSpriteEffect.Texture = this.mAnimationSpriteTexture;
					this.mAnimationSpriteEffect.TextureOffset = default(Vector2);
					this.mAnimationSpriteEffect.GraphicsDevice.Vertices[0].SetSource(this.mAnimationSpriteVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
					this.mAnimationSpriteEffect.GraphicsDevice.VertexDeclaration = this.mAnimationSpriteVertexDeclaration;
					this.mAnimationSpriteEffect.GraphicsDevice.RenderState.DestinationBlend = Blend.One;
					this.mAnimationSpriteEffect.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
					this.mAnimationSpriteEffect.Begin();
					this.mAnimationSpriteEffect.CurrentTechnique.Passes[0].Begin();
					this.mAnimationSpriteEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
					this.mAnimationSpriteEffect.CurrentTechnique.Passes[0].End();
					this.mAnimationSpriteEffect.End();
					this.mAnimationSpriteEffect.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
					this.mAnimationSpriteEffect.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
				}
			}

			// Token: 0x170007B5 RID: 1973
			// (get) Token: 0x06001F52 RID: 8018 RVA: 0x000DB5F1 File Offset: 0x000D97F1
			public int ZIndex
			{
				get
				{
					return 200;
				}
			}

			// Token: 0x040021A1 RID: 8609
			public GUIBasicEffect mEffect;

			// Token: 0x040021A2 RID: 8610
			public VertexBuffer mVertices;

			// Token: 0x040021A3 RID: 8611
			public VertexDeclaration mVertexDeclaration;

			// Token: 0x040021A4 RID: 8612
			public float mAlpha;

			// Token: 0x040021A5 RID: 8613
			public float mNormalizedHealth;

			// Token: 0x040021A6 RID: 8614
			public Vector3 mHealthColor;

			// Token: 0x040021A7 RID: 8615
			public float mHealthBarPosition;

			// Token: 0x040021A8 RID: 8616
			public Vector2 mHealthBarTextureOffset;

			// Token: 0x040021A9 RID: 8617
			public Vector2 mHealthBarBackgroundTextureOffset;

			// Token: 0x040021AA RID: 8618
			public Vector2 mHealthBarOverlayTextureOffset;

			// Token: 0x040021AB RID: 8619
			public Vector2 mHealthBarScale = new Vector2(1f, 1f);

			// Token: 0x040021AC RID: 8620
			private Vector2 mHealthBarStandardScale;

			// Token: 0x040021AD RID: 8621
			public bool mIsScaled;

			// Token: 0x040021AE RID: 8622
			public bool mIsColored;

			// Token: 0x040021AF RID: 8623
			public GUIBasicEffect mAnimationSpriteEffect;

			// Token: 0x040021B0 RID: 8624
			public VertexBuffer mAnimationSpriteVertexBuffer;

			// Token: 0x040021B1 RID: 8625
			public VertexDeclaration mAnimationSpriteVertexDeclaration;

			// Token: 0x040021B2 RID: 8626
			public Texture3D mAnimationSpriteTexture;

			// Token: 0x040021B3 RID: 8627
			public Vector2 AnimationSpritePosition;

			// Token: 0x040021B4 RID: 8628
			public float AnimationSpriteDepth;

			// Token: 0x040021B5 RID: 8629
			public bool Saturate;

			// Token: 0x040021B6 RID: 8630
			private Matrix mTransform = Matrix.Identity;

			// Token: 0x040021B7 RID: 8631
			private static readonly Vector4 COLOR = new Vector4(1f, 1f, 1f, 1f);

			// Token: 0x040021B8 RID: 8632
			private static readonly Vector4 SATURATED_COLOR = new Vector4(0.5f, 0.5f, 0.5f, 1f);

			// Token: 0x040021B9 RID: 8633
			public bool mHasAnimatedSprite;

			// Token: 0x040021BA RID: 8634
			public bool mIsTicking;

			// Token: 0x040021BB RID: 8635
			public bool mIsDone;

			// Token: 0x040021BC RID: 8636
			public GUIBasicEffect mDisplayNameEffect;

			// Token: 0x040021BD RID: 8637
			private string mDisplayName;

			// Token: 0x040021BE RID: 8638
			private Text mDisplayNameText;

			// Token: 0x040021BF RID: 8639
			public bool mShowDisplayName;

			// Token: 0x040021C0 RID: 8640
			public BitmapFont mFont;

			// Token: 0x040021C1 RID: 8641
			public bool mDisplayNameIsDirty;

			// Token: 0x040021C2 RID: 8642
			public float mDisplayNameHeight;

			// Token: 0x040021C3 RID: 8643
			public float mAlphaDisplayName;
		}
	}
}
