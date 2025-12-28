using System;
using System.Collections.Generic;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.Entities.Bosses
{
	// Token: 0x02000038 RID: 56
	internal class CountryDefense : IBoss
	{
		// Token: 0x06000242 RID: 578 RVA: 0x0000E910 File Offset: 0x0000CB10
		public CountryDefense(PlayState iPlayState)
		{
			this.mPlayState = iPlayState;
			Texture3D texture3D = null;
			VertexPositionTexture[] array = new VertexPositionTexture[]
			{
				new VertexPositionTexture(new Vector3(0f, 1f, 0f), new Vector2(0f, 1f)),
				new VertexPositionTexture(new Vector3(0f, 0f, 0f), new Vector2(0f, 0f)),
				new VertexPositionTexture(new Vector3(1f, 0f, 0f), new Vector2(1f, 0f)),
				new VertexPositionTexture(new Vector3(1f, 1f, 0f), new Vector2(1f, 1f))
			};
			VertexBuffer vertexBuffer;
			VertexDeclaration vertexDeclaration;
			GUIBasicEffect guibasicEffect;
			lock (this.mPlayState.Scene.GraphicsDevice)
			{
				vertexBuffer = new VertexBuffer(this.mPlayState.Scene.GraphicsDevice, VertexPositionTexture.SizeInBytes * array.Length, BufferUsage.WriteOnly);
				vertexBuffer.SetData<VertexPositionTexture>(array);
				vertexDeclaration = new VertexDeclaration(this.mPlayState.Scene.GraphicsDevice, VertexPositionTexture.VertexElements);
				guibasicEffect = new GUIBasicEffect(this.mPlayState.Scene.GraphicsDevice, RenderManager.Instance.GlobalDummyEffect.EffectPool);
				guibasicEffect.SetTechnique(GUIBasicEffect.Technique.Texture3D);
				texture3D = this.mPlayState.Content.Load<Texture3D>("UI/Boss/CountryDefense");
			}
			guibasicEffect.Texture = texture3D;
			this.mDepth = (float)texture3D.Depth;
			this.mDepthDivisor = 1f / this.mDepth;
			this.mDepthOffset = this.mDepthDivisor * 0.5f;
			this.mHouseAnimations = new float[5];
			this.mRenderData = new CountryDefense.RenderData[3];
			for (int i = 0; i < 3; i++)
			{
				this.mRenderData[i] = new CountryDefense.RenderData();
				this.mRenderData[i].mHouseAnimations = new float[5];
				this.mRenderData[i].Effect = guibasicEffect;
				this.mRenderData[i].Size = new Vector2((float)texture3D.Width, (float)texture3D.Height);
				this.mRenderData[i].DivSize = new Vector2(1f / (float)texture3D.Width, 1f / (float)texture3D.Height);
				this.mRenderData[i].VertexBuffer = vertexBuffer;
				this.mRenderData[i].VertexDeclaration = vertexDeclaration;
			}
		}

		// Token: 0x06000243 RID: 579 RVA: 0x0000EBD4 File Offset: 0x0000CDD4
		public void Initialize(ref Matrix iOrientation, int iUniqueID)
		{
			this.Initialize(ref iOrientation);
		}

		// Token: 0x06000244 RID: 580 RVA: 0x0000EBE0 File Offset: 0x0000CDE0
		public void Initialize(ref Matrix iOrientation)
		{
			this.mHouseAnimations = new float[5];
			this.mHouseEntities.Clear();
			this.mHouseEntities.Add((DamageablePhysicsEntity)Entity.GetByID("house0".GetHashCodeCustom()));
			this.mHouseEntities.Add((DamageablePhysicsEntity)Entity.GetByID("house1".GetHashCodeCustom()));
			this.mHouseEntities.Add((DamageablePhysicsEntity)Entity.GetByID("house2".GetHashCodeCustom()));
			this.mHouseEntities.Add((DamageablePhysicsEntity)Entity.GetByID("house3".GetHashCodeCustom()));
			this.mHouseEntities.Add((DamageablePhysicsEntity)Entity.GetByID("house4".GetHashCodeCustom()));
			this.mChieftain = (NonPlayerCharacter)Entity.GetByID("#ent_beast_chieftain".GetHashCodeCustom());
			this.mTimer = 0f;
		}

		// Token: 0x06000245 RID: 581 RVA: 0x0000ECC4 File Offset: 0x0000CEC4
		public void UpdateBoss(DataChannel iDataChannel, float iDeltaTime, bool iFightStarted)
		{
			if (!this.mPlayState.IsInCutscene)
			{
				this.mTimer += iDeltaTime;
			}
			if (this.mChieftain == null)
			{
				this.mChieftain = (NonPlayerCharacter)Entity.GetByID("#ent_beast_chieftain".GetHashCodeCustom());
			}
			for (int i = 0; i < this.mHouseEntities.Count; i++)
			{
				if (this.mHouseEntities[i] == null)
				{
					this.mHouseAnimations[i] = 1f - this.mDepthOffset;
					this.mHouseEntities.RemoveAt(i--);
				}
				else
				{
					float num = this.mHouseEntities[i].HitPoints / this.mHouseEntities[i].MaxHitPoints;
					num = Math.Max(num, 0f);
					this.mHouseAnimations[i] = (float)Math.Ceiling((double)(num * (this.mDepth - 1f))) * this.mDepthDivisor + this.mDepthDivisor;
					this.mHouseAnimations[i] = 1f - this.mHouseAnimations[i];
					this.mHouseAnimations[i] += this.mDepthOffset;
				}
			}
			CountryDefense.RenderData renderData = this.mRenderData[(int)iDataChannel];
			this.mHouseAnimations.CopyTo(renderData.mHouseAnimations, 0);
			renderData.Alpha = Math.Min(this.mTimer, 1f);
			this.mPlayState.Scene.AddRenderableGUIObject(iDataChannel, renderData);
		}

		// Token: 0x06000246 RID: 582 RVA: 0x0000EE33 File Offset: 0x0000D033
		public void DeInitialize()
		{
		}

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x06000247 RID: 583 RVA: 0x0000EE35 File Offset: 0x0000D035
		public bool Dead
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x06000248 RID: 584 RVA: 0x0000EE38 File Offset: 0x0000D038
		public float MaxHitPoints
		{
			get
			{
				if (this.mChieftain == null)
				{
					return 1f;
				}
				return this.mChieftain.MaxHitPoints;
			}
		}

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000249 RID: 585 RVA: 0x0000EE53 File Offset: 0x0000D053
		public float HitPoints
		{
			get
			{
				if (this.mChieftain == null)
				{
					return 1f;
				}
				return this.mChieftain.HitPoints;
			}
		}

		// Token: 0x0600024A RID: 586 RVA: 0x0000EE6E File Offset: 0x0000D06E
		public DamageResult Damage(int iPartIndex, Damage iDamage, Entity iAttacker, ref Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			return DamageResult.None;
		}

		// Token: 0x0600024B RID: 587 RVA: 0x0000EE71 File Offset: 0x0000D071
		public void Damage(int iPartIndex, float iDamage, Elements iElement)
		{
		}

		// Token: 0x0600024C RID: 588 RVA: 0x0000EE73 File Offset: 0x0000D073
		public void SetSlow(int iIndex)
		{
		}

		// Token: 0x0600024D RID: 589 RVA: 0x0000EE75 File Offset: 0x0000D075
		public void GetRandomPositionOnCollisionSkin(int iIndex, out Vector3 oPosition)
		{
			oPosition = default(Vector3);
		}

		// Token: 0x0600024E RID: 590 RVA: 0x0000EE7E File Offset: 0x0000D07E
		public bool HasStatus(int iIndex, StatusEffects iStatus)
		{
			return false;
		}

		// Token: 0x0600024F RID: 591 RVA: 0x0000EE81 File Offset: 0x0000D081
		public float StatusMagnitude(int iIndex, StatusEffects iStatus)
		{
			return 0f;
		}

		// Token: 0x06000250 RID: 592 RVA: 0x0000EE88 File Offset: 0x0000D088
		public StatusEffect[] GetStatusEffects()
		{
			return null;
		}

		// Token: 0x06000251 RID: 593 RVA: 0x0000EE8B File Offset: 0x0000D08B
		public bool AddImpulseVelocity(Vector3 iDirection, float iElevation, float iMassPower, float iDistance)
		{
			return false;
		}

		// Token: 0x06000252 RID: 594 RVA: 0x0000EE8E File Offset: 0x0000D08E
		public void ScriptMessage(BossMessages iMessage)
		{
		}

		// Token: 0x06000253 RID: 595 RVA: 0x0000EE90 File Offset: 0x0000D090
		public void NetworkUpdate(ref BossUpdateMessage iMsg)
		{
		}

		// Token: 0x06000254 RID: 596 RVA: 0x0000EE92 File Offset: 0x0000D092
		public void NetworkInitialize(ref BossInitializeMessage iMsg)
		{
		}

		// Token: 0x06000255 RID: 597 RVA: 0x0000EE94 File Offset: 0x0000D094
		public BossEnum GetBossType()
		{
			return BossEnum.CountryDefense;
		}

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000256 RID: 598 RVA: 0x0000EE97 File Offset: 0x0000D097
		public bool NetworkInitialized
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06000257 RID: 599 RVA: 0x0000EE9A File Offset: 0x0000D09A
		public float ResistanceAgainst(Elements iElement)
		{
			return 1f;
		}

		// Token: 0x040001C1 RID: 449
		private List<DamageablePhysicsEntity> mHouseEntities = new List<DamageablePhysicsEntity>(5);

		// Token: 0x040001C2 RID: 450
		private float[] mHouseAnimations;

		// Token: 0x040001C3 RID: 451
		private float mDepth;

		// Token: 0x040001C4 RID: 452
		private float mDepthOffset;

		// Token: 0x040001C5 RID: 453
		private float mDepthDivisor;

		// Token: 0x040001C6 RID: 454
		private float mTimer;

		// Token: 0x040001C7 RID: 455
		private CountryDefense.RenderData[] mRenderData;

		// Token: 0x040001C8 RID: 456
		private PlayState mPlayState;

		// Token: 0x040001C9 RID: 457
		private NonPlayerCharacter mChieftain;

		// Token: 0x02000039 RID: 57
		protected class RenderData : IRenderableGUIObject
		{
			// Token: 0x06000258 RID: 600 RVA: 0x0000EEA4 File Offset: 0x0000D0A4
			public void Draw(float iDeltaTime)
			{
				Point screenSize = RenderManager.Instance.ScreenSize;
				float num = Math.Min(RenderManager.Instance.GUIScale, 1f);
				this.Effect.SetScreenSize(screenSize.X, screenSize.Y);
				this.Effect.GraphicsDevice.Vertices[0].SetSource(this.VertexBuffer, 0, VertexPositionTexture.SizeInBytes);
				this.Effect.GraphicsDevice.VertexDeclaration = this.VertexDeclaration;
				Vector2 vector = new Vector2(this.Size.X * num, this.Size.Y * num);
				int num2 = this.mHouseAnimations.Length;
				float m = (float)screenSize.X * 0.1f;
				Matrix identity = Matrix.Identity;
				identity.M11 = vector.X;
				identity.M22 = vector.Y;
				identity.M41 = m;
				identity.M42 = 32f;
				this.Effect.Transform = identity;
				this.Effect.TextureEnabled = true;
				this.Effect.VertexColorEnabled = false;
				this.Effect.Color = new Vector4(1f, 1f, 1f, this.Alpha);
				this.Effect.Begin();
				this.Effect.CurrentTechnique.Passes[0].Begin();
				for (int i = 0; i < this.mHouseAnimations.Length; i++)
				{
					this.Effect.Transform = identity;
					this.Effect.CommitChanges();
					this.Effect.W = this.mHouseAnimations[i];
					this.Effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
					identity.M41 += 8f + vector.X;
				}
				this.Effect.CurrentTechnique.Passes[0].End();
				this.Effect.End();
			}

			// Token: 0x1700007C RID: 124
			// (get) Token: 0x06000259 RID: 601 RVA: 0x0000F0A1 File Offset: 0x0000D2A1
			public int ZIndex
			{
				get
				{
					return 100;
				}
			}

			// Token: 0x040001CA RID: 458
			private const float PADDING = 8f;

			// Token: 0x040001CB RID: 459
			public VertexBuffer VertexBuffer;

			// Token: 0x040001CC RID: 460
			public VertexDeclaration VertexDeclaration;

			// Token: 0x040001CD RID: 461
			public GUIBasicEffect Effect;

			// Token: 0x040001CE RID: 462
			public float Alpha;

			// Token: 0x040001CF RID: 463
			public Vector2 Size;

			// Token: 0x040001D0 RID: 464
			public Vector2 DivSize;

			// Token: 0x040001D1 RID: 465
			public float[] mHouseAnimations;
		}
	}
}
