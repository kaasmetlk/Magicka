using System;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000611 RID: 1553
	public sealed class Blizzard : SpecialAbility, IAbilityEffect
	{
		// Token: 0x17000AF4 RID: 2804
		// (get) Token: 0x06002E93 RID: 11923 RVA: 0x0017A21C File Offset: 0x0017841C
		public static Blizzard Instance
		{
			get
			{
				if (Blizzard.mSingelton == null)
				{
					lock (Blizzard.mSingeltonLock)
					{
						if (Blizzard.mSingelton == null)
						{
							Blizzard.mSingelton = new Blizzard();
						}
					}
				}
				return Blizzard.mSingelton;
			}
		}

		// Token: 0x06002E94 RID: 11924 RVA: 0x0017A270 File Offset: 0x00178470
		private Blizzard() : base(Animations.cast_magick_global, "#magick_blizzard".GetHashCodeCustom())
		{
			this.mDamage.AttackProperty = AttackProperties.Status;
			this.mDamage.Element = Elements.Cold;
			this.mDamage.Magnitude = 1f;
			this.mDamage.Amount = 0f;
			FogEffect iEffect = new FogEffect(Game.Instance.GraphicsDevice, null);
			Texture2D iTexture = Game.Instance.Content.Load<Texture2D>("EffectTextures/Blizzard");
			this.mRenderData = new Blizzard.RenderData[3];
			for (int i = 0; i < 3; i++)
			{
				this.mRenderData[i] = new Blizzard.RenderData(iEffect, iTexture);
			}
		}

		// Token: 0x06002E95 RID: 11925 RVA: 0x0017A315 File Offset: 0x00178515
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			this.mCaster = null;
			this.mTimeStamp = 0.0;
			this.mPlayState = iPlayState;
			this.mDoDamage = (NetworkManager.Instance.State != NetworkState.Client);
			return this.Execute();
		}

		// Token: 0x06002E96 RID: 11926 RVA: 0x0017A350 File Offset: 0x00178550
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			this.mCaster = iOwner;
			this.mTimeStamp = iOwner.PlayState.PlayTime;
			this.mPlayState = iPlayState;
			Avatar avatar = iOwner as Avatar;
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				if (avatar != null && !(avatar.Player.Gamer is NetworkGamer))
				{
					this.mDoDamage = true;
				}
				else
				{
					this.mDoDamage = false;
				}
			}
			else if (NetworkManager.Instance.State == NetworkState.Server)
			{
				if (avatar == null)
				{
					this.mDoDamage = true;
				}
				else if (avatar.Player.Gamer is NetworkGamer)
				{
					this.mDoDamage = false;
				}
				else
				{
					this.mDoDamage = true;
				}
			}
			else
			{
				this.mDoDamage = true;
			}
			return this.Execute();
		}

		// Token: 0x06002E97 RID: 11927 RVA: 0x0017A40C File Offset: 0x0017860C
		private bool Execute()
		{
			if (this.mPlayState.Level.CurrentScene.Indoors)
			{
				return false;
			}
			this.mScene = this.mPlayState.Level.CurrentScene;
			if (this.IsDead)
			{
				this.mTextureOffset0 = new Vector2(0f, 0.5f);
				this.mTextureOffset1 = new Vector2(0.5f, 0f);
				this.mAlpha = 0f;
			}
			if (this.mAmbience == null || !this.mAmbience.IsPlaying)
			{
				this.mAmbience = AudioManager.Instance.PlayCue(Banks.Spells, Blizzard.AMBIENCE);
			}
			this.mTTL = 10f;
			SpellManager.Instance.AddSpellEffect(this);
			if (this.mCaster is Avatar)
			{
				this.mPlayState.IncrementBlizzardRainCount();
			}
			return true;
		}

		// Token: 0x17000AF5 RID: 2805
		// (get) Token: 0x06002E98 RID: 11928 RVA: 0x0017A4E0 File Offset: 0x001786E0
		public bool IsDead
		{
			get
			{
				return this.mTTL <= 0f & this.mAlpha <= 0f;
			}
		}

		// Token: 0x06002E99 RID: 11929 RVA: 0x0017A504 File Offset: 0x00178704
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mTTL -= iDeltaTime;
			this.mColdTimer -= iDeltaTime;
			if (this.mTTL > 0f)
			{
				this.mAlpha = Math.Min(this.mAlpha + iDeltaTime, 1f);
			}
			else
			{
				this.mAlpha = Math.Max(this.mAlpha - iDeltaTime, 0f);
			}
			this.mTextureOffset0.X = this.mTextureOffset0.X - iDeltaTime * 0.738724f;
			this.mTextureOffset0.Y = this.mTextureOffset0.Y - iDeltaTime * 1.234238f;
			this.mTextureOffset1.X = this.mTextureOffset1.X - iDeltaTime * 1.324678f;
			this.mTextureOffset1.Y = this.mTextureOffset1.Y - iDeltaTime * 0.647534f;
			Vector3 position = this.mPlayState.Camera.Position;
			Vector3 cameraoffset = MagickCamera.CAMERAOFFSET;
			default(Vector3).Z = -1f;
			Vector3.Subtract(ref position, ref cameraoffset, out position);
			this.mDamage.Magnitude = 0.25f;
			if (this.mColdTimer <= 0f)
			{
				if (this.mDoDamage)
				{
					EntityManager entityManager = this.mPlayState.EntityManager;
					StaticList<Entity> entities = entityManager.Entities;
					foreach (Entity entity in entities)
					{
						IDamageable damageable = entity as IDamageable;
						Shield shield = null;
						if (!(damageable == null | entity is MissileEntity) && damageable.ResistanceAgainst(Elements.Cold) != 1f && !entityManager.IsProtectedByShield(entity, out shield))
						{
							damageable.Damage(this.mDamage, this.mCaster as Entity, this.mTimeStamp, default(Vector3));
						}
					}
				}
				Liquid[] liquids = this.mPlayState.Level.CurrentScene.Liquids;
				for (int i = 0; i < liquids.Length; i++)
				{
					liquids[i].FreezeAll(0.25f);
				}
				this.mColdTimer = 0.25f;
			}
			Blizzard.RenderData renderData = this.mRenderData[(int)iDataChannel];
			renderData.TextureOffset0 = this.mTextureOffset0;
			renderData.TextureOffset1 = this.mTextureOffset1;
			renderData.Alpha = this.mAlpha;
			this.mPlayState.Scene.AddPostEffect(iDataChannel, renderData);
		}

		// Token: 0x06002E9A RID: 11930 RVA: 0x0017A76C File Offset: 0x0017896C
		public void OnRemove()
		{
			this.mTTL = 0f;
			if (this.mAmbience != null)
			{
				this.mAmbience.Stop(AudioStopOptions.AsAuthored);
			}
		}

		// Token: 0x040032A8 RID: 12968
		private const float MAGICK_TTL = 10f;

		// Token: 0x040032A9 RID: 12969
		private static Blizzard mSingelton;

		// Token: 0x040032AA RID: 12970
		private static volatile object mSingeltonLock = new object();

		// Token: 0x040032AB RID: 12971
		private static readonly int AMBIENCE = "magick_blizzard".GetHashCodeCustom();

		// Token: 0x040032AC RID: 12972
		private float mTTL;

		// Token: 0x040032AD RID: 12973
		private float mColdTimer;

		// Token: 0x040032AE RID: 12974
		private Vector2 mTextureOffset0;

		// Token: 0x040032AF RID: 12975
		private Vector2 mTextureOffset1;

		// Token: 0x040032B0 RID: 12976
		private float mAlpha;

		// Token: 0x040032B1 RID: 12977
		private PlayState mPlayState;

		// Token: 0x040032B2 RID: 12978
		private GameScene mScene;

		// Token: 0x040032B3 RID: 12979
		private Cue mAmbience;

		// Token: 0x040032B4 RID: 12980
		private Damage mDamage;

		// Token: 0x040032B5 RID: 12981
		private ISpellCaster mCaster;

		// Token: 0x040032B6 RID: 12982
		private Blizzard.RenderData[] mRenderData;

		// Token: 0x040032B7 RID: 12983
		private bool mDoDamage;

		// Token: 0x02000612 RID: 1554
		private class RenderData : IPostEffect
		{
			// Token: 0x06002E9C RID: 11932 RVA: 0x0017A7AA File Offset: 0x001789AA
			public RenderData(FogEffect iEffect, Texture2D iTexture)
			{
				this.mEffect = iEffect;
				this.mTexture = iTexture;
			}

			// Token: 0x17000AF6 RID: 2806
			// (get) Token: 0x06002E9D RID: 11933 RVA: 0x0017A7C0 File Offset: 0x001789C0
			public int ZIndex
			{
				get
				{
					return 10000;
				}
			}

			// Token: 0x06002E9E RID: 11934 RVA: 0x0017A7C8 File Offset: 0x001789C8
			public void Draw(float iDeltaTime, ref Vector2 iPixelSize, ref Matrix iViewMatrix, ref Matrix iProjectionMatrix, Texture2D iCandidate, Texture2D iDepthMap, Texture2D iNormalMap)
			{
				this.mEffect.SourceTexture0 = this.mTexture;
				this.mEffect.SourceTexture1 = this.mTexture;
				this.mEffect.TextureOffset0 = this.TextureOffset0;
				this.mEffect.TextureOffset1 = this.TextureOffset1;
				this.mEffect.SourceTexture2 = iDepthMap;
				this.mEffect.FogStart = 180f;
				this.mEffect.FogEnd = 240f;
				this.mEffect.Color = new Vector3(1.4f, 1.8f, 2f);
				this.mEffect.Alpha = this.Alpha;
				this.mEffect.SetTechnique(FogEffect.Technique.Linear);
				Vector2 destinationDimensions = default(Vector2);
				destinationDimensions.X = (float)iDepthMap.Width;
				destinationDimensions.Y = (float)iDepthMap.Height;
				this.mEffect.DestinationDimensions = destinationDimensions;
				this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mEffect.VertexBuffer, 0, VertexPositionTexture.SizeInBytes);
				this.mEffect.GraphicsDevice.VertexDeclaration = this.mEffect.VertexDeclaration;
				this.mEffect.Begin();
				this.mEffect.CurrentTechnique.Passes[0].Begin();
				this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
				this.mEffect.CurrentTechnique.Passes[0].End();
				this.mEffect.End();
			}

			// Token: 0x040032B8 RID: 12984
			private FogEffect mEffect;

			// Token: 0x040032B9 RID: 12985
			private Texture2D mTexture;

			// Token: 0x040032BA RID: 12986
			public float Alpha;

			// Token: 0x040032BB RID: 12987
			public Vector2 TextureOffset0;

			// Token: 0x040032BC RID: 12988
			public Vector2 TextureOffset1;
		}
	}
}
