using System;
using System.Collections.Generic;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
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
	// Token: 0x020002AE RID: 686
	internal class Portal : SpecialAbility
	{
		// Token: 0x17000550 RID: 1360
		// (get) Token: 0x060014B4 RID: 5300 RVA: 0x00080E30 File Offset: 0x0007F030
		public static Portal Instance
		{
			get
			{
				if (Portal.sSingelton == null)
				{
					lock (Portal.sSingeltonLock)
					{
						if (Portal.sSingelton == null)
						{
							Portal.sSingelton = new Portal();
						}
					}
				}
				return Portal.sSingelton;
			}
		}

		// Token: 0x060014B5 RID: 5301 RVA: 0x00080E84 File Offset: 0x0007F084
		private Portal() : base(Animations.cast_area_fireworks, 0)
		{
		}

		// Token: 0x17000551 RID: 1361
		// (get) Token: 0x060014B6 RID: 5302 RVA: 0x00080E8F File Offset: 0x0007F08F
		public bool Connected
		{
			get
			{
				return Portal.sConnected;
			}
		}

		// Token: 0x060014B7 RID: 5303 RVA: 0x00080E96 File Offset: 0x0007F096
		public void Initialize(PlayState iPlayState)
		{
			Portal.sPortalA = new Portal.PortalEntity(iPlayState, Portal.PortalEntity.PortalType.Blue);
			Portal.sPortalB = new Portal.PortalEntity(iPlayState, Portal.PortalEntity.PortalType.Orange);
		}

		// Token: 0x060014B8 RID: 5304 RVA: 0x00080EB0 File Offset: 0x0007F0B0
		internal static Portal.PortalEntity OtherPortal(Portal.PortalEntity iPortalEntity)
		{
			if (iPortalEntity == Portal.sPortalA)
			{
				return Portal.sPortalB;
			}
			return Portal.sPortalA;
		}

		// Token: 0x060014B9 RID: 5305 RVA: 0x00080EC5 File Offset: 0x0007F0C5
		public void Kill()
		{
			Portal.sPortalA.Kill();
			Portal.sPortalB.Kill();
		}

		// Token: 0x060014BA RID: 5306 RVA: 0x00080EDC File Offset: 0x0007F0DC
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			this.mPlayState = iPlayState;
			Vector3 position = iOwner.Position;
			Vector3 direction = iOwner.Direction;
			Vector3.Multiply(ref direction, 2f, out direction);
			Vector3.Add(ref position, ref direction, out position);
			Vector3 vector;
			iOwner.PlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref position, out vector, MovementProperties.Water | MovementProperties.Dynamic);
			float num;
			Vector3.DistanceSquared(ref vector, ref position, out num);
			if (num > 25f)
			{
				AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL, iOwner.AudioEmitter);
				return false;
			}
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				Helper.Swap<Portal.PortalEntity>(ref Portal.sPortalA, ref Portal.sPortalB);
				Portal.sPortalA.Initialize(ref vector);
			}
			return true;
		}

		// Token: 0x060014BB RID: 5307 RVA: 0x00080F94 File Offset: 0x0007F194
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				Vector3 vector = iPosition;
				Vector3 vector2;
				iPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref vector, out vector2, MovementProperties.Water | MovementProperties.Dynamic);
				float num;
				Vector3.DistanceSquared(ref vector2, ref vector, out num);
				if (num > 25f)
				{
					return false;
				}
				Helper.Swap<Portal.PortalEntity>(ref Portal.sPortalA, ref Portal.sPortalB);
				Portal.sPortalA.Initialize(ref vector2);
			}
			return true;
		}

		// Token: 0x060014BC RID: 5308 RVA: 0x00081000 File Offset: 0x0007F200
		internal void SpawnPortal(ref SpawnPortalMessage iMsg)
		{
			Helper.Swap<Portal.PortalEntity>(ref Portal.sPortalA, ref Portal.sPortalB);
			Portal.sPortalA.Initialize(ref iMsg);
		}

		// Token: 0x060014BD RID: 5309 RVA: 0x0008101C File Offset: 0x0007F21C
		public static void Update(PlayState iPlayState)
		{
			Vector3 position = Portal.sPortalA.Position;
			Vector3 position2 = Portal.sPortalB.Position;
			float num;
			Vector3.DistanceSquared(ref position, ref position2, out num);
			Portal.sConnected = (!Portal.sPortalA.Dead && !Portal.sPortalB.Dead && num <= 900f / iPlayState.Camera.Magnification);
			if (Portal.sConnected)
			{
				num = (float)Math.Sqrt((double)num);
				if (Portal.sSoundA == null)
				{
					Portal.sSoundA = AudioManager.Instance.GetCue(Banks.Spells, Portal.SOUND_CONNECTED);
				}
				if (Portal.sSoundB == null)
				{
					Portal.sSoundB = AudioManager.Instance.GetCue(Banks.Spells, Portal.SOUND_CONNECTED);
				}
				Portal.sSoundA.Apply3D(Portal.sPortalA.PlayState.Camera.Listener, Portal.sPortalA.AudioEmitter);
				Portal.sSoundA.SetVariable("Volume", -8f);
				Portal.sSoundA.SetVariable("Health", 1f - num / 30f);
				Portal.sSoundB.Apply3D(Portal.sPortalA.PlayState.Camera.Listener, Portal.sPortalB.AudioEmitter);
				Portal.sSoundB.SetVariable("Volume", -8f);
				Portal.sSoundB.SetVariable("Health", 1f - num / 30f);
				if (Portal.sSoundA.IsPrepared)
				{
					Portal.sSoundA.Play();
				}
				if (Portal.sSoundB.IsPrepared)
				{
					Portal.sSoundB.Play();
					return;
				}
			}
			else
			{
				if (Portal.sSoundA != null)
				{
					Portal.sSoundA.Stop(AudioStopOptions.AsAuthored);
					Portal.sSoundA = null;
				}
				if (Portal.sSoundB != null)
				{
					Portal.sSoundB.Stop(AudioStopOptions.AsAuthored);
					Portal.sSoundB = null;
				}
			}
		}

		// Token: 0x04001610 RID: 5648
		private const string SOUND_VARIABLE = "Health";

		// Token: 0x04001611 RID: 5649
		private const float MAX_DISTANCE = 30f;

		// Token: 0x04001612 RID: 5650
		private static readonly int EFFECT_BLUE = "magick_portal_blue".GetHashCodeCustom();

		// Token: 0x04001613 RID: 5651
		private static readonly int EFFECT_ORANGE = "magick_portal_orange".GetHashCodeCustom();

		// Token: 0x04001614 RID: 5652
		private static readonly int EFFECT_INACTIVE = "magick_portal_inactive".GetHashCodeCustom();

		// Token: 0x04001615 RID: 5653
		private static readonly int SOUND_CONNECTED = "spell_shield".GetHashCodeCustom();

		// Token: 0x04001616 RID: 5654
		private static readonly int SOUND_SPAWN = "magick_teleportb".GetHashCodeCustom();

		// Token: 0x04001617 RID: 5655
		private static readonly int SOUND_TELEPORT = "magick_teleporta".GetHashCodeCustom();

		// Token: 0x04001618 RID: 5656
		private static Portal sSingelton;

		// Token: 0x04001619 RID: 5657
		private static volatile object sSingeltonLock = new object();

		// Token: 0x0400161A RID: 5658
		private static Portal.PortalEntity sPortalA;

		// Token: 0x0400161B RID: 5659
		private static Portal.PortalEntity sPortalB;

		// Token: 0x0400161C RID: 5660
		private static Cue sSoundA;

		// Token: 0x0400161D RID: 5661
		private static Cue sSoundB;

		// Token: 0x0400161E RID: 5662
		private static bool sConnected;

		// Token: 0x0400161F RID: 5663
		private PlayState mPlayState;

		// Token: 0x020002AF RID: 687
		public sealed class PortalEntity : Entity
		{
			// Token: 0x060014BF RID: 5311 RVA: 0x00081250 File Offset: 0x0007F450
			internal PortalEntity(PlayState iPlayState, Portal.PortalEntity.PortalType iType) : base(iPlayState)
			{
				this.mPortalType = iType;
				this.mDead = true;
				this.mBody = new Body();
				this.mCollision = new CollisionSkin(this.mBody);
				this.mCollision.AddPrimitive(new Capsule(new Vector3(0f, -1f, 0f), Portal.PortalEntity.sRotateX90, 0.5f, 2f), 1, new MaterialProperties(0f, 0f, 0f));
				this.mCollision.callbackFn += this.OnCollision;
				this.mBody.CollisionSkin = this.mCollision;
				this.mBody.Immovable = false;
				this.mBody.ApplyGravity = false;
				this.mBody.Tag = this;
				this.mRenderData = new Portal.PortalEntity.RenderData[3];
				for (int i = 0; i < this.mRenderData.Length; i++)
				{
					this.mRenderData[i] = new Portal.PortalEntity.RenderData();
					if (this.mPortalType == Portal.PortalEntity.PortalType.Blue)
					{
						this.mRenderData[i].TextureOffset = new Vector2(0f, 0f);
					}
					else
					{
						this.mRenderData[i].TextureOffset = new Vector2(0.5f, 0f);
					}
					this.mRenderData[i].Texture = Game.Instance.Content.Load<Texture2D>("EffectTextures/portal");
				}
			}

			// Token: 0x060014C0 RID: 5312 RVA: 0x000813C0 File Offset: 0x0007F5C0
			private bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
			{
				if (!Portal.Instance.Connected || iSkin1.Owner == null)
				{
					return false;
				}
				Entity entity = iSkin1.Owner.Tag as Entity;
				if (entity != null && !(entity is Barrier) && !(entity is Shield) && !(entity is Grease.GreaseField))
				{
					if (!Portal.PortalEntity.sIgnoredEntities.ContainsKey(entity.Handle))
					{
						this.mTeleportQueue.Enqueue(entity);
					}
					Portal.PortalEntity.sIgnoredEntities[entity.Handle] = 0.1f;
				}
				return false;
			}

			// Token: 0x060014C1 RID: 5313 RVA: 0x00081444 File Offset: 0x0007F644
			public override Vector3 CalcImpulseVelocity(Vector3 iDirection, float iElevation, float iMassPower, float iDistance)
			{
				return default(Vector3);
			}

			// Token: 0x060014C2 RID: 5314 RVA: 0x0008145A File Offset: 0x0007F65A
			protected override void AddImpulseVelocity(ref Vector3 iVelocity)
			{
			}

			// Token: 0x060014C3 RID: 5315 RVA: 0x0008145C File Offset: 0x0007F65C
			public void Initialize(ref Vector3 iPosition)
			{
				this.Deinitialize();
				this.mDead = false;
				this.mEffectAlpha = 0f;
				Segment iSeg = default(Segment);
				iSeg.Origin = iPosition;
				iSeg.Origin.Y = iSeg.Origin.Y + 2f;
				iSeg.Delta.Y = -4f;
				float num;
				Vector3 vector;
				Vector3 vector2;
				if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out num, out vector, out vector2, out this.mAnimatedPart, iSeg))
				{
					iPosition = vector;
					iPosition.Y += 1.5f;
				}
				if (this.mAnimatedPart != null)
				{
					this.mAnimatedPart.AddEntity(this);
				}
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					SpawnPortalMessage spawnPortalMessage;
					spawnPortalMessage.Position = iPosition;
					spawnPortalMessage.AnimationHandle = ((this.mAnimatedPart == null) ? ushort.MaxValue : this.mAnimatedPart.Handle);
					NetworkManager.Instance.Interface.SendMessage<SpawnPortalMessage>(ref spawnPortalMessage);
				}
				this.mBody.MoveTo(iPosition, Matrix.Identity);
				this.mConnected = Portal.Instance.Connected;
				Vector3 forward = Vector3.Forward;
				if (this.mConnected)
				{
					if (this.mPortalType == Portal.PortalEntity.PortalType.Blue)
					{
						EffectManager.Instance.StartEffect(Portal.EFFECT_BLUE, ref iPosition, ref forward, out this.mEffect);
					}
					else
					{
						EffectManager.Instance.StartEffect(Portal.EFFECT_ORANGE, ref iPosition, ref forward, out this.mEffect);
					}
				}
				this.mPlayState.EntityManager.AddEntity(this);
				base.Initialize();
				AudioManager.Instance.PlayCue(Banks.Spells, Portal.SOUND_SPAWN, base.AudioEmitter);
			}

			// Token: 0x060014C4 RID: 5316 RVA: 0x000815FC File Offset: 0x0007F7FC
			internal void Initialize(ref SpawnPortalMessage iMsg)
			{
				this.Deinitialize();
				this.mDead = false;
				this.mEffectAlpha = 0f;
				if (iMsg.AnimationHandle < 65535)
				{
					this.mAnimatedPart = AnimatedLevelPart.GetFromHandle((int)iMsg.AnimationHandle);
					if (this.mAnimatedPart != null)
					{
						this.mAnimatedPart.AddEntity(this);
					}
				}
				this.mBody.MoveTo(iMsg.Position, Matrix.Identity);
				this.mConnected = Portal.Instance.Connected;
				Vector3 forward = Vector3.Forward;
				if (this.mConnected)
				{
					if (this.mPortalType == Portal.PortalEntity.PortalType.Blue)
					{
						EffectManager.Instance.StartEffect(Portal.EFFECT_BLUE, ref iMsg.Position, ref forward, out this.mEffect);
					}
					else
					{
						EffectManager.Instance.StartEffect(Portal.EFFECT_ORANGE, ref iMsg.Position, ref forward, out this.mEffect);
					}
				}
				this.mPlayState.EntityManager.AddEntity(this);
				base.Initialize();
				AudioManager.Instance.PlayCue(Banks.Spells, Portal.SOUND_SPAWN, base.AudioEmitter);
			}

			// Token: 0x17000552 RID: 1362
			// (get) Token: 0x060014C5 RID: 5317 RVA: 0x000816FC File Offset: 0x0007F8FC
			public override bool Dead
			{
				get
				{
					return this.mDead;
				}
			}

			// Token: 0x17000553 RID: 1363
			// (get) Token: 0x060014C6 RID: 5318 RVA: 0x00081704 File Offset: 0x0007F904
			public override bool Removable
			{
				get
				{
					return this.mDead;
				}
			}

			// Token: 0x060014C7 RID: 5319 RVA: 0x0008170C File Offset: 0x0007F90C
			public override void Kill()
			{
				this.mDead = true;
			}

			// Token: 0x060014C8 RID: 5320 RVA: 0x00081715 File Offset: 0x0007F915
			public override void Deinitialize()
			{
				this.mDead = true;
				EffectManager.Instance.Stop(ref this.mEffect);
				if (this.mAnimatedPart != null)
				{
					this.mAnimatedPart.RemoveEntity(this);
				}
				this.mAnimatedPart = null;
				base.Deinitialize();
			}

			// Token: 0x060014C9 RID: 5321 RVA: 0x00081750 File Offset: 0x0007F950
			public override void Update(DataChannel iDataChannel, float iDeltaTime)
			{
				this.mBody.AllowFreezing = (this.mAnimatedPart == null);
				if (this == Portal.sPortalA)
				{
					Portal.PortalEntity.sIgnoredEntities.Update(iDeltaTime);
				}
				while (this.mTeleportQueue.Count > 0)
				{
					Entity entity = this.mTeleportQueue.Dequeue();
					Vector3 position = entity.Body.Position;
					Vector3 position2;
					this.GetOutPos(ref position, out position2);
					entity.Body.Position = position2;
					AudioManager.Instance.PlayCue(Banks.Spells, Portal.SOUND_TELEPORT, base.AudioEmitter);
				}
				Matrix orientation = this.mBody.Orientation;
				orientation.Translation = this.mBody.Position;
				if (this.mConnected != Portal.Instance.Connected)
				{
					EffectManager.Instance.Stop(ref this.mEffect);
					this.mConnected = Portal.Instance.Connected;
					if (this.mConnected)
					{
						if (this.mPortalType == Portal.PortalEntity.PortalType.Blue)
						{
							EffectManager.Instance.StartEffect(Portal.EFFECT_BLUE, ref orientation, out this.mEffect);
						}
						else
						{
							EffectManager.Instance.StartEffect(Portal.EFFECT_ORANGE, ref orientation, out this.mEffect);
						}
					}
				}
				EffectManager.Instance.UpdateOrientation(ref this.mEffect, ref orientation);
				this.mEffectAlpha = Math.Min(this.mEffectAlpha + iDeltaTime, 1f);
				base.Update(iDataChannel, iDeltaTime);
				this.mRenderData[(int)iDataChannel].Position = this.mBody.Position;
				this.mRenderData[(int)iDataChannel].Alpha = this.mEffectAlpha;
				this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, this.mRenderData[(int)iDataChannel]);
			}

			// Token: 0x060014CA RID: 5322 RVA: 0x000818E3 File Offset: 0x0007FAE3
			internal override bool SendsNetworkUpdate(NetworkState iState)
			{
				return false;
			}

			// Token: 0x060014CB RID: 5323 RVA: 0x000818E6 File Offset: 0x0007FAE6
			protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
			{
				oMsg = default(EntityUpdateMessage);
			}

			// Token: 0x060014CC RID: 5324 RVA: 0x000818F0 File Offset: 0x0007FAF0
			internal void GetOutPos(ref Vector3 iPos, out Vector3 oPos)
			{
				Vector3 position = this.mBody.Position;
				Vector3.Subtract(ref iPos, ref position, out position);
				Vector3.Multiply(ref position, 0.8f, out position);
				Portal.PortalEntity portalEntity = Portal.OtherPortal(this);
				oPos = portalEntity.Position;
				Vector3.Add(ref oPos, ref position, out oPos);
			}

			// Token: 0x04001620 RID: 5664
			private const float HEIGHT = 2f;

			// Token: 0x04001621 RID: 5665
			private const float RADIUS = 0.5f;

			// Token: 0x04001622 RID: 5666
			public static int VERTEXSTRIDE = VertexPositionTexture.SizeInBytes;

			// Token: 0x04001623 RID: 5667
			public static readonly VertexPositionTexture[] QUAD = new VertexPositionTexture[]
			{
				new VertexPositionTexture(new Vector3(-0.5f, -0.5f, 0f), new Vector2(0f, 1f)),
				new VertexPositionTexture(new Vector3(-0.5f, 0.5f, 0f), new Vector2(0f, 0f)),
				new VertexPositionTexture(new Vector3(0.5f, 0.5f, 0f), new Vector2(1f, 0f)),
				new VertexPositionTexture(new Vector3(0.5f, -0.5f, 0f), new Vector2(1f, 1f))
			};

			// Token: 0x04001624 RID: 5668
			public static readonly ushort[] INDICES = new ushort[]
			{
				0,
				1,
				2,
				0,
				2,
				3
			};

			// Token: 0x04001625 RID: 5669
			private static Matrix sRotateX90 = Matrix.CreateRotationX(-1.5707964f);

			// Token: 0x04001626 RID: 5670
			private bool mConnected;

			// Token: 0x04001627 RID: 5671
			private VisualEffectReference mEffect;

			// Token: 0x04001628 RID: 5672
			private new bool mDead;

			// Token: 0x04001629 RID: 5673
			private AnimatedLevelPart mAnimatedPart;

			// Token: 0x0400162A RID: 5674
			private Queue<Entity> mTeleportQueue = new Queue<Entity>(16);

			// Token: 0x0400162B RID: 5675
			private static HitList sIgnoredEntities = new HitList(32);

			// Token: 0x0400162C RID: 5676
			private float mEffectAlpha;

			// Token: 0x0400162D RID: 5677
			private Portal.PortalEntity.PortalType mPortalType;

			// Token: 0x0400162E RID: 5678
			private Portal.PortalEntity.RenderData[] mRenderData;

			// Token: 0x020002B0 RID: 688
			private class RenderData : IRenderableAdditiveObject, IPreRenderRenderer
			{
				// Token: 0x060014CE RID: 5326 RVA: 0x00081A74 File Offset: 0x0007FC74
				public RenderData()
				{
					if (Portal.PortalEntity.RenderData.sVertexBuffer == null)
					{
						lock (Game.Instance.GraphicsDevice)
						{
							Portal.PortalEntity.RenderData.sIndexBuffer = new IndexBuffer(Game.Instance.GraphicsDevice, 2 * Portal.PortalEntity.INDICES.Length, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
							Portal.PortalEntity.RenderData.sIndexBuffer.SetData<ushort>(Portal.PortalEntity.INDICES);
							Portal.PortalEntity.RenderData.sVertexBuffer = new VertexBuffer(Game.Instance.GraphicsDevice, Portal.PortalEntity.VERTEXSTRIDE * Portal.PortalEntity.QUAD.Length, BufferUsage.WriteOnly);
							Portal.PortalEntity.RenderData.sVertexBuffer.SetData<VertexPositionTexture>(Portal.PortalEntity.QUAD);
							Portal.PortalEntity.RenderData.sVertexDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
						}
						Portal.PortalEntity.RenderData.VERTICEHASH = Portal.PortalEntity.RenderData.sVertexBuffer.GetHashCode();
					}
					this.mBoundingSphere = default(BoundingSphere);
					this.mBoundingSphere.Radius = 8f;
				}

				// Token: 0x17000554 RID: 1364
				// (get) Token: 0x060014CF RID: 5327 RVA: 0x00081B6C File Offset: 0x0007FD6C
				public int Effect
				{
					get
					{
						return AdditiveEffect.TYPEHASH;
					}
				}

				// Token: 0x17000555 RID: 1365
				// (get) Token: 0x060014D0 RID: 5328 RVA: 0x00081B73 File Offset: 0x0007FD73
				public int Technique
				{
					get
					{
						return 0;
					}
				}

				// Token: 0x17000556 RID: 1366
				// (get) Token: 0x060014D1 RID: 5329 RVA: 0x00081B76 File Offset: 0x0007FD76
				public VertexBuffer Vertices
				{
					get
					{
						return Portal.PortalEntity.RenderData.sVertexBuffer;
					}
				}

				// Token: 0x17000557 RID: 1367
				// (get) Token: 0x060014D2 RID: 5330 RVA: 0x00081B7D File Offset: 0x0007FD7D
				public int VerticesHashCode
				{
					get
					{
						return Portal.PortalEntity.RenderData.VERTICEHASH;
					}
				}

				// Token: 0x17000558 RID: 1368
				// (get) Token: 0x060014D3 RID: 5331 RVA: 0x00081B84 File Offset: 0x0007FD84
				public int VertexStride
				{
					get
					{
						return Portal.PortalEntity.VERTEXSTRIDE;
					}
				}

				// Token: 0x17000559 RID: 1369
				// (get) Token: 0x060014D4 RID: 5332 RVA: 0x00081B8B File Offset: 0x0007FD8B
				public IndexBuffer Indices
				{
					get
					{
						return Portal.PortalEntity.RenderData.sIndexBuffer;
					}
				}

				// Token: 0x1700055A RID: 1370
				// (get) Token: 0x060014D5 RID: 5333 RVA: 0x00081B92 File Offset: 0x0007FD92
				public VertexDeclaration VertexDeclaration
				{
					get
					{
						return Portal.PortalEntity.RenderData.sVertexDeclaration;
					}
				}

				// Token: 0x060014D6 RID: 5334 RVA: 0x00081B9C File Offset: 0x0007FD9C
				public bool Cull(BoundingFrustum iViewFrustum)
				{
					this.mBoundingSphere.Center = this.Position;
					BoundingSphere boundingSphere = this.mBoundingSphere;
					return boundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
				}

				// Token: 0x060014D7 RID: 5335 RVA: 0x00081BCC File Offset: 0x0007FDCC
				public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
				{
					AdditiveEffect additiveEffect = iEffect as AdditiveEffect;
					this.mLookAt.Translation = this.Position;
					additiveEffect.World = this.mLookAt;
					additiveEffect.ColorTint = new Vector4(1f, 1f, 1f, this.Alpha);
					additiveEffect.TextureOffset = this.TextureOffset;
					additiveEffect.TextureScale = new Vector2(0.5f, 1f);
					additiveEffect.Texture = this.Texture;
					additiveEffect.TextureEnabled = true;
					additiveEffect.VertexColorEnabled = false;
					additiveEffect.CommitChanges();
					additiveEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
				}

				// Token: 0x060014D8 RID: 5336 RVA: 0x00081C70 File Offset: 0x0007FE70
				public void PreRenderUpdate(DataChannel iDataChannel, float iDeltaTime, ref Matrix iViewProjectionMatrix, ref Vector3 iCameraPosition, ref Vector3 iCameraDirection)
				{
					Vector3 up = Vector3.Up;
					Vector3 position = this.Position;
					Vector3 forward;
					Vector3.Subtract(ref iCameraPosition, ref position, out forward);
					forward.Normalize();
					Vector3 right;
					Vector3.Cross(ref up, ref forward, out right);
					Vector3.Cross(ref forward, ref right, out up);
					this.mLookAt.Forward = forward;
					this.mLookAt.Right = right;
					this.mLookAt.Up = up;
					this.mLookAt.M11 = this.mLookAt.M11 * 2f;
					this.mLookAt.M12 = this.mLookAt.M12 * 2f;
					this.mLookAt.M13 = this.mLookAt.M13 * 2f;
					this.mLookAt.M21 = this.mLookAt.M21 * 4f;
					this.mLookAt.M22 = this.mLookAt.M22 * 4f;
					this.mLookAt.M23 = this.mLookAt.M23 * 4f;
				}

				// Token: 0x0400162F RID: 5679
				private static IndexBuffer sIndexBuffer;

				// Token: 0x04001630 RID: 5680
				private static VertexBuffer sVertexBuffer;

				// Token: 0x04001631 RID: 5681
				private static VertexDeclaration sVertexDeclaration;

				// Token: 0x04001632 RID: 5682
				private static int VERTICEHASH;

				// Token: 0x04001633 RID: 5683
				public Texture2D Texture;

				// Token: 0x04001634 RID: 5684
				public Vector3 Position;

				// Token: 0x04001635 RID: 5685
				private Matrix mLookAt = Matrix.Identity;

				// Token: 0x04001636 RID: 5686
				private BoundingSphere mBoundingSphere;

				// Token: 0x04001637 RID: 5687
				public float Alpha;

				// Token: 0x04001638 RID: 5688
				public Vector2 TextureOffset;
			}

			// Token: 0x020002B1 RID: 689
			public enum PortalType
			{
				// Token: 0x0400163A RID: 5690
				Blue,
				// Token: 0x0400163B RID: 5691
				Orange
			}
		}
	}
}
