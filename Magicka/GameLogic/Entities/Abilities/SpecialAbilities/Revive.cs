using System;
using System.Collections.Generic;
using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Levels.Versus;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using PolygonHead.Lights;
using SteamWrapper;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000570 RID: 1392
	public class Revive : SpecialAbility, IAbilityEffect
	{
		// Token: 0x0600297F RID: 10623 RVA: 0x00145310 File Offset: 0x00143510
		public static Revive GetInstance()
		{
			if (Revive.sCache.Count > 0)
			{
				Revive result = Revive.sCache[Revive.sCache.Count - 1];
				Revive.sCache.RemoveAt(Revive.sCache.Count - 1);
				return result;
			}
			return new Revive();
		}

		// Token: 0x06002980 RID: 10624 RVA: 0x00145360 File Offset: 0x00143560
		public static void InitializeCache(int iNr)
		{
			Revive.sCache = new List<Revive>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				Revive.sCache.Add(new Revive());
			}
		}

		// Token: 0x06002981 RID: 10625 RVA: 0x00145393 File Offset: 0x00143593
		public void SetSpecificPlayer(int iID)
		{
			this.mPlayerID = iID;
			this.mIDSpecific = true;
		}

		// Token: 0x06002982 RID: 10626 RVA: 0x001453A4 File Offset: 0x001435A4
		private Revive() : base(Animations.cast_magick_self, "#magick_revive".GetHashCodeCustom())
		{
			Texture2D texture = Game.Instance.Content.Load<Texture2D>("Levels/Textures/Surface/Nature/Atmosphere/light_ray01");
			this.mRenderData = new Revive.GodRayRenderData[3];
			for (int i = 0; i < 3; i++)
			{
				this.mRenderData[i] = new Revive.GodRayRenderData();
				this.mRenderData[i].Texture = texture;
			}
			lock (Game.Instance.GraphicsDevice)
			{
				this.mSpotLight = new SpotLight(Game.Instance.GraphicsDevice);
			}
		}

		// Token: 0x06002983 RID: 10627 RVA: 0x0014544C File Offset: 0x0014364C
		public Revive(Animations iAnimation) : base(iAnimation, "#magick_revive".GetHashCodeCustom())
		{
			Texture2D texture = Game.Instance.Content.Load<Texture2D>("Levels/Textures/Surface/Nature/Atmosphere/light_ray01");
			this.mRenderData = new Revive.GodRayRenderData[3];
			for (int i = 0; i < 3; i++)
			{
				this.mRenderData[i] = new Revive.GodRayRenderData();
				this.mRenderData[i].Texture = texture;
			}
			lock (Game.Instance.GraphicsDevice)
			{
				this.mSpotLight = new SpotLight(Game.Instance.GraphicsDevice);
			}
		}

		// Token: 0x06002984 RID: 10628 RVA: 0x001454F4 File Offset: 0x001436F4
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			return this.Execute(iPosition, iPlayState, 1f);
		}

		// Token: 0x06002985 RID: 10629 RVA: 0x00145504 File Offset: 0x00143704
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			this.mOwner = iOwner;
			Vector3 position = iOwner.Position;
			Vector3 direction = iOwner.Direction;
			Vector3.Multiply(ref direction, 4f, out direction);
			Vector3.Add(ref position, ref direction, out position);
			return this.Execute(position, iPlayState, 0.1f);
		}

		// Token: 0x06002986 RID: 10630 RVA: 0x00145558 File Offset: 0x00143758
		internal bool Execute(Vector3 iPosition, PlayState iPlayState, float iHealth)
		{
			VersusRuleset versusRuleset = iPlayState.Level.CurrentScene.RuleSet as VersusRuleset;
			Player player = null;
			if (this.mOwner is Avatar)
			{
				player = (this.mOwner as Avatar).Player;
			}
			this.mHealthPercentage = iHealth;
			this.mPlayState = iPlayState;
			float num = float.Epsilon;
			int num2 = -1;
			Player[] players = Game.Instance.Players;
			if (this.mIDSpecific)
			{
				Player player2 = players[this.mPlayerID];
				if (player2.Playing && !player2.Ressing && (player2.Avatar == null || player2.Avatar.Overkilled || (player2.Avatar.Dead && !player2.Avatar.Undying && float.IsNaN(player2.Avatar.UndyingTimer))) && player2.DeadAge > num)
				{
					if (player != null && versusRuleset != null && !versusRuleset.CanRevive(player, player2))
					{
						this.mIDSpecific = false;
						AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL);
						Revive.sCache.Add(this);
						return false;
					}
					num2 = this.mPlayerID;
					this.mRevivee = player2;
				}
			}
			else
			{
				for (int i = 0; i < players.Length; i++)
				{
					Player player3 = players[i];
					if (player3.Playing && !player3.Ressing && (player3.Avatar == null || player3.Avatar.Overkilled || (player3.Avatar.Dead && !player3.Avatar.Undying && float.IsNaN(player3.Avatar.UndyingTimer))) && player3.DeadAge > num && (player == null || versusRuleset == null || versusRuleset.CanRevive(player, player3)))
					{
						num2 = i;
						num = player3.DeadAge;
						this.mRevivee = player3;
					}
				}
			}
			if (num2 == -1)
			{
				this.mIDSpecific = false;
				AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL);
				Revive.sCache.Add(this);
				return false;
			}
			this.mRevivee.Ressing = true;
			this.mTypeID = this.mRevivee.Gamer.Avatar.Type;
			if (this.mOwner != null)
			{
				AudioManager.Instance.PlayCue(Banks.Spells, Revive.SOUND_REVIVE_HASH, this.mOwner.AudioEmitter);
			}
			else
			{
				AudioManager.Instance.PlayCue(Banks.Spells, Revive.SOUND_REVIVE_HASH);
			}
			Vector3 vector;
			this.mPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref iPosition, out vector, MovementProperties.Default);
			iPosition = vector;
			this.mPosition = iPosition;
			this.mDead = false;
			this.mRessed = false;
			this.mTimer = 0f;
			this.mSpotLight.AmbientColor = Vector3.Zero;
			this.mSpotLight.DiffuseColor = Vector3.Zero;
			this.mSpotLight.SpecularAmount = 0f;
			this.mSpotLight.Direction = Vector3.Down;
			this.mSpotLight.Sharpness = 1f;
			this.mSpotLight.UseAttenuation = true;
			this.mSpotLight.CutoffAngle = 0.4712389f;
			this.mSpotLight.Range = 13f;
			this.mSpotLight.Direction = Revive.GodRayRenderData.sRayRotation.Down;
			Vector3 position;
			Vector3.Add(ref this.mPosition, ref Revive.GodRayRenderData.sOffset1, out position);
			this.mSpotLight.Position = position;
			this.mSpotLight.Enable(this.mPlayState.Scene);
			for (int j = 0; j < 3; j++)
			{
				Vector3.Add(ref this.mPosition, ref Revive.GodRayRenderData.sOffset1, out this.mRenderData[j].Position1);
				Vector3.Add(ref this.mPosition, ref Revive.GodRayRenderData.sOffset2, out this.mRenderData[j].Position2);
			}
			SpellManager.Instance.AddSpellEffect(this);
			return true;
		}

		// Token: 0x170009C4 RID: 2500
		// (get) Token: 0x06002987 RID: 10631 RVA: 0x00145924 File Offset: 0x00143B24
		public bool IsDead
		{
			get
			{
				return this.mDead;
			}
		}

		// Token: 0x06002988 RID: 10632 RVA: 0x0014592C File Offset: 0x00143B2C
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			Revive.GodRayRenderData godRayRenderData = this.mRenderData[(int)iDataChannel];
			float num = Math.Min(this.mTimer, 1.3f);
			godRayRenderData.Alpha = num;
			Vector3 vector = new Vector3(num * 10f);
			this.mSpotLight.AmbientColor = vector;
			this.mSpotLight.DiffuseColor = vector;
			this.mSpotLight.SpecularAmount = num;
			if (!this.mRessed)
			{
				this.mTimer += iDeltaTime * 4f;
				if (this.mTimer > 1f)
				{
					if (NetworkManager.Instance.State != NetworkState.Client && this.mRevivee.Playing && this.mRevivee.Gamer != null)
					{
						this.mRevivee.Weapon = null;
						this.mRevivee.Staff = null;
						Avatar fromCache = Avatar.GetFromCache(this.mRevivee);
						CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(this.mTypeID);
						fromCache.Initialize(cachedTemplate, this.mPosition, Player.UNIQUE_ID[this.mRevivee.ID]);
						if (this.mPlayState.Level.CurrentScene.RuleSet is VersusRuleset)
						{
							fromCache.Faction &= ~Factions.FRIENDLY;
						}
						fromCache.HitPoints = fromCache.MaxHitPoints * this.mHealthPercentage;
						fromCache.SpawnAnimation = Animations.revive;
						fromCache.ChangeState(RessurectionState.Instance);
						this.mPlayState.EntityManager.AddEntity(fromCache);
						AudioManager.Instance.PlayCue(Banks.Spells, Revive.SOUNDHASH, fromCache.AudioEmitter);
						this.mRevivee.Avatar = fromCache;
						this.mRevivee.Ressing = false;
						if (fromCache.Player != null && fromCache.Player.Controller is XInputController)
						{
							(fromCache.Player.Controller as XInputController).Rumble(2f, 2f);
						}
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							SpawnPlayerMessage spawnPlayerMessage = default(SpawnPlayerMessage);
							spawnPlayerMessage.Handle = fromCache.Handle;
							spawnPlayerMessage.Id = (byte)this.mRevivee.ID;
							spawnPlayerMessage.MagickRevive = true;
							spawnPlayerMessage.Position = fromCache.Position;
							spawnPlayerMessage.Direction = fromCache.CharacterBody.DesiredDirection;
							NetworkManager.Instance.Interface.SendMessage<SpawnPlayerMessage>(ref spawnPlayerMessage, P2PSend.Reliable);
						}
					}
					else if (NetworkManager.Instance.State == NetworkState.Client && this.mOwner is Avatar)
					{
						(this.mOwner as Avatar).RequestForcedSyncingOfPlayers();
					}
					this.mTimer = 3f;
					this.mRessed = true;
				}
			}
			else
			{
				this.mTimer -= iDeltaTime;
				if (this.mTimer <= 0f)
				{
					this.mDead = true;
				}
			}
			this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, godRayRenderData);
		}

		// Token: 0x06002989 RID: 10633 RVA: 0x00145BF0 File Offset: 0x00143DF0
		public void OnRemove()
		{
			this.mIDSpecific = false;
			this.mTypeID = Avatar.WIZARDHASH;
			this.mSpotLight.AmbientColor = Vector3.Zero;
			this.mSpotLight.DiffuseColor = Vector3.Zero;
			this.mSpotLight.SpecularAmount = 0f;
			this.mSpotLight.Sharpness = 1f;
			this.mSpotLight.UseAttenuation = true;
			this.mSpotLight.Direction = Vector3.Down;
			if (this.mRevivee != null)
			{
				this.mRevivee.Ressing = false;
			}
			this.mSpotLight.Disable();
			Revive.sCache.Add(this);
		}

		// Token: 0x04002CDC RID: 11484
		private static List<Revive> sCache;

		// Token: 0x04002CDD RID: 11485
		public static int VERTEXSTRIDE = VertexPositionTexture.SizeInBytes;

		// Token: 0x04002CDE RID: 11486
		public static readonly VertexPositionTexture[] QUAD = new VertexPositionTexture[]
		{
			new VertexPositionTexture(new Vector3(-0.5f, -0.5f, 0f), new Vector2(0f, 1f)),
			new VertexPositionTexture(new Vector3(-0.5f, 0.5f, 0f), new Vector2(0f, 0f)),
			new VertexPositionTexture(new Vector3(0.5f, 0.5f, 0f), new Vector2(1f, 0f)),
			new VertexPositionTexture(new Vector3(0.5f, -0.5f, 0f), new Vector2(1f, 1f))
		};

		// Token: 0x04002CDF RID: 11487
		public static readonly ushort[] INDICES = new ushort[]
		{
			0,
			1,
			2,
			0,
			2,
			3
		};

		// Token: 0x04002CE0 RID: 11488
		public static readonly int SOUND_REVIVE_HASH = "magick_revive_cast".GetHashCodeCustom();

		// Token: 0x04002CE1 RID: 11489
		public static readonly int SOUNDHASH = "magick_revive".GetHashCodeCustom();

		// Token: 0x04002CE2 RID: 11490
		public static readonly int SOUND_2_HASH = "magick_revive2".GetHashCodeCustom();

		// Token: 0x04002CE3 RID: 11491
		public static readonly int EFFECT = "magick_revive".GetHashCodeCustom();

		// Token: 0x04002CE4 RID: 11492
		private bool mDead;

		// Token: 0x04002CE5 RID: 11493
		private bool mRessed;

		// Token: 0x04002CE6 RID: 11494
		private float mTimer;

		// Token: 0x04002CE7 RID: 11495
		private Vector3 mPosition;

		// Token: 0x04002CE8 RID: 11496
		private Player mRevivee;

		// Token: 0x04002CE9 RID: 11497
		private PlayState mPlayState;

		// Token: 0x04002CEA RID: 11498
		private Revive.GodRayRenderData[] mRenderData;

		// Token: 0x04002CEB RID: 11499
		private SpotLight mSpotLight;

		// Token: 0x04002CEC RID: 11500
		private ISpellCaster mOwner;

		// Token: 0x04002CED RID: 11501
		private float mHealthPercentage;

		// Token: 0x04002CEE RID: 11502
		private bool mIDSpecific;

		// Token: 0x04002CEF RID: 11503
		private int mPlayerID;

		// Token: 0x04002CF0 RID: 11504
		private int mTypeID;

		// Token: 0x02000571 RID: 1393
		protected class GodRayRenderData : IRenderableAdditiveObject, IPreRenderRenderer
		{
			// Token: 0x0600298B RID: 10635 RVA: 0x00145DEC File Offset: 0x00143FEC
			public GodRayRenderData()
			{
				if (Revive.GodRayRenderData.sVertexBuffer == null)
				{
					lock (Game.Instance.GraphicsDevice)
					{
						Revive.GodRayRenderData.sIndexBuffer = new IndexBuffer(Game.Instance.GraphicsDevice, 2 * Revive.INDICES.Length, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
						Revive.GodRayRenderData.sIndexBuffer.SetData<ushort>(Revive.INDICES);
						Revive.GodRayRenderData.sVertexBuffer = new VertexBuffer(Game.Instance.GraphicsDevice, Revive.VERTEXSTRIDE * Revive.QUAD.Length, BufferUsage.WriteOnly);
						Revive.GodRayRenderData.sVertexBuffer.SetData<VertexPositionTexture>(Revive.QUAD);
						Revive.GodRayRenderData.sVertexDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
					}
					Revive.GodRayRenderData.VERTICEHASH = Revive.GodRayRenderData.sVertexBuffer.GetHashCode();
				}
				this.mBoundingSphere = default(BoundingSphere);
				this.mBoundingSphere.Radius = 30f;
			}

			// Token: 0x170009C5 RID: 2501
			// (get) Token: 0x0600298C RID: 10636 RVA: 0x00145F00 File Offset: 0x00144100
			public int Effect
			{
				get
				{
					return AdditiveEffect.TYPEHASH;
				}
			}

			// Token: 0x170009C6 RID: 2502
			// (get) Token: 0x0600298D RID: 10637 RVA: 0x00145F07 File Offset: 0x00144107
			public int Technique
			{
				get
				{
					return 0;
				}
			}

			// Token: 0x170009C7 RID: 2503
			// (get) Token: 0x0600298E RID: 10638 RVA: 0x00145F0A File Offset: 0x0014410A
			public VertexBuffer Vertices
			{
				get
				{
					return Revive.GodRayRenderData.sVertexBuffer;
				}
			}

			// Token: 0x170009C8 RID: 2504
			// (get) Token: 0x0600298F RID: 10639 RVA: 0x00145F11 File Offset: 0x00144111
			public int VerticesHashCode
			{
				get
				{
					return Revive.GodRayRenderData.VERTICEHASH;
				}
			}

			// Token: 0x170009C9 RID: 2505
			// (get) Token: 0x06002990 RID: 10640 RVA: 0x00145F18 File Offset: 0x00144118
			public int VertexStride
			{
				get
				{
					return Revive.VERTEXSTRIDE;
				}
			}

			// Token: 0x170009CA RID: 2506
			// (get) Token: 0x06002991 RID: 10641 RVA: 0x00145F1F File Offset: 0x0014411F
			public IndexBuffer Indices
			{
				get
				{
					return Revive.GodRayRenderData.sIndexBuffer;
				}
			}

			// Token: 0x170009CB RID: 2507
			// (get) Token: 0x06002992 RID: 10642 RVA: 0x00145F26 File Offset: 0x00144126
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return Revive.GodRayRenderData.sVertexDeclaration;
				}
			}

			// Token: 0x06002993 RID: 10643 RVA: 0x00145F30 File Offset: 0x00144130
			public bool Cull(BoundingFrustum iViewFrustum)
			{
				this.mBoundingSphere.Center = this.Position1;
				BoundingSphere boundingSphere = this.mBoundingSphere;
				return boundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
			}

			// Token: 0x06002994 RID: 10644 RVA: 0x00145F60 File Offset: 0x00144160
			public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				AdditiveEffect additiveEffect = iEffect as AdditiveEffect;
				this.mLookAt.Translation = this.Position1;
				additiveEffect.World = this.mLookAt;
				this.mColorTint.W = this.Alpha * 0.5f;
				additiveEffect.ColorTint = this.mColorTint;
				additiveEffect.TextureOffset = Vector2.Zero;
				additiveEffect.TextureScale = Vector2.One;
				additiveEffect.Texture = this.Texture;
				additiveEffect.TextureEnabled = true;
				additiveEffect.VertexColorEnabled = false;
				additiveEffect.CommitChanges();
				additiveEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
				this.mColorTint.W = this.Alpha * 0.35f;
				additiveEffect.ColorTint = this.mColorTint;
				this.mLookAt.Translation = this.Position2;
				additiveEffect.World = this.mLookAt;
				additiveEffect.CommitChanges();
				additiveEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
			}

			// Token: 0x06002995 RID: 10645 RVA: 0x00146054 File Offset: 0x00144254
			public void PreRenderUpdate(DataChannel iDataChannel, float iDeltaTime, ref Matrix iViewProjectionMatrix, ref Vector3 iCameraPosition, ref Vector3 iCameraDirection)
			{
				Vector3 up = Vector3.Up;
				Vector3 position = this.Position1;
				Vector3 forward;
				Vector3.Subtract(ref iCameraPosition, ref position, out forward);
				forward.Normalize();
				Vector3 right;
				Vector3.Cross(ref up, ref forward, out right);
				Vector3.Cross(ref forward, ref right, out up);
				this.mLookAt.Forward = forward;
				this.mLookAt.Right = right;
				this.mLookAt.Up = up;
				Matrix.Multiply(ref Revive.GodRayRenderData.sRayRotation, ref this.mLookAt, out this.mLookAt);
				this.mLookAt.M11 = this.mLookAt.M11 * 6.5f;
				this.mLookAt.M12 = this.mLookAt.M12 * 6.5f;
				this.mLookAt.M13 = this.mLookAt.M13 * 6.5f;
				this.mLookAt.M21 = this.mLookAt.M21 * 17f;
				this.mLookAt.M22 = this.mLookAt.M22 * 17f;
				this.mLookAt.M23 = this.mLookAt.M23 * 17f;
			}

			// Token: 0x04002CF1 RID: 11505
			private static IndexBuffer sIndexBuffer;

			// Token: 0x04002CF2 RID: 11506
			private static VertexBuffer sVertexBuffer;

			// Token: 0x04002CF3 RID: 11507
			private static VertexDeclaration sVertexDeclaration;

			// Token: 0x04002CF4 RID: 11508
			private static int VERTICEHASH;

			// Token: 0x04002CF5 RID: 11509
			public static Vector3 sOffset1 = new Vector3(1.5f, 11f, 0f);

			// Token: 0x04002CF6 RID: 11510
			public static Vector3 sOffset2 = new Vector3(2.05f, 9f, 0.75f);

			// Token: 0x04002CF7 RID: 11511
			public static Matrix sRayRotation = Matrix.CreateFromYawPitchRoll(0f, 0f, -0.24f);

			// Token: 0x04002CF8 RID: 11512
			public float Alpha;

			// Token: 0x04002CF9 RID: 11513
			public Texture2D Texture;

			// Token: 0x04002CFA RID: 11514
			public Vector3 Position1;

			// Token: 0x04002CFB RID: 11515
			public Vector3 Position2;

			// Token: 0x04002CFC RID: 11516
			private BoundingSphere mBoundingSphere;

			// Token: 0x04002CFD RID: 11517
			private Vector4 mColorTint = new Vector4(1f, 1f, 1f, 0f);

			// Token: 0x04002CFE RID: 11518
			private Matrix mLookAt = Matrix.Identity;
		}
	}
}
