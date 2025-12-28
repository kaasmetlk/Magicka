using System;
using System.Collections.Generic;
using JigLibX.Collision;
using JigLibX.Physics;
using Magicka.Achievements;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Statistics;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Graphics.Effects;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000588 RID: 1416
	internal class VortexEntity : Entity
	{
		// Token: 0x06002A4C RID: 10828 RVA: 0x0014C0A4 File Offset: 0x0014A2A4
		public static VortexEntity GetInstance()
		{
			VortexEntity vortexEntity;
			lock (VortexEntity.sCache)
			{
				vortexEntity = VortexEntity.sCache[0];
				VortexEntity.sCache.RemoveAt(0);
				VortexEntity.sCache.Add(vortexEntity);
			}
			return vortexEntity;
		}

		// Token: 0x06002A4D RID: 10829 RVA: 0x0014C0FC File Offset: 0x0014A2FC
		public static VortexEntity GetSpecificInstance(ushort iHandle)
		{
			VortexEntity vortexEntity;
			lock (VortexEntity.sCache)
			{
				vortexEntity = (Entity.GetFromHandle((int)iHandle) as VortexEntity);
				VortexEntity.sCache.Remove(vortexEntity);
				VortexEntity.sCache.Add(vortexEntity);
			}
			return vortexEntity;
		}

		// Token: 0x06002A4E RID: 10830 RVA: 0x0014C154 File Offset: 0x0014A354
		public static void InitializeCache(int iNr, PlayState iPlayState)
		{
			VortexEntity.sCache = new List<VortexEntity>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				VortexEntity.sCache.Add(new VortexEntity(iPlayState));
			}
		}

		// Token: 0x06002A4F RID: 10831 RVA: 0x0014C188 File Offset: 0x0014A388
		public VortexEntity(PlayState iPlayState) : base(iPlayState)
		{
			this.mHawkingHoleCount = 0;
			this.mAudioEmitter = new AudioEmitter();
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			if (VortexEntity.sVertices == null)
			{
				Vector3[] array = new Vector3[42];
				int num = 0;
				array[num++] = Vector3.Up;
				double num2 = -0.3141592653589793;
				double num3 = 0.5235987755982988;
				for (int i = 0; i < 5; i++)
				{
					double num4 = (double)i * 1.2566370614359172 + num2;
					array[num].Y = (float)Math.Cos(num3);
					double num5 = Math.Sin(num3);
					array[num].X = (float)(Math.Cos(num4) * num5);
					array[num].Z = (float)(Math.Sin(num4) * num5);
					num++;
				}
				num2 = -0.3141592653589793;
				num3 = 1.0471975511965976;
				for (int j = 0; j < 3; j++)
				{
					for (int k = 0; k < 10; k++)
					{
						double num6 = (double)k * 0.6283185307179586 + num2;
						array[num].Y = (float)Math.Cos(num3);
						double num7 = Math.Sin(num3);
						array[num].X = (float)(Math.Cos(num6) * num7);
						array[num].Z = (float)(Math.Sin(num6) * num7);
						num++;
					}
					num2 += 0.3141592653589793;
					num3 += 0.5235987755982988;
				}
				num2 -= 0.3141592653589793;
				for (int l = 0; l < 5; l++)
				{
					double num8 = (double)l * 1.2566370614359172 + num2;
					array[num].Y = (float)Math.Cos(num3);
					double num9 = Math.Sin(num3);
					array[num].X = (float)(Math.Cos(num8) * num9);
					array[num].Z = (float)(Math.Sin(num8) * num9);
					num++;
				}
				array[num++] = Vector3.Down;
				VortexEntity.sNumVertices = array.Length;
				lock (graphicsDevice)
				{
					VortexEntity.sVertices = new VertexBuffer(graphicsDevice, array.Length * 12, BufferUsage.WriteOnly);
					VortexEntity.sVertices.SetData<Vector3>(array);
				}
				ushort[] array2 = new ushort[240];
				num = 0;
				for (int m = 0; m < 5; m++)
				{
					array2[num++] = 0;
					array2[num++] = (ushort)(m + 1);
					array2[num++] = (ushort)((m + 1) % 5 + 1);
				}
				for (int n = 0; n < 5; n++)
				{
					array2[num++] = (ushort)(n + 1);
					array2[num++] = (ushort)(n * 2 + 6);
					array2[num++] = (ushort)(n * 2 + 7);
					array2[num++] = (ushort)(n + 1);
					array2[num++] = (ushort)(n * 2 + 7);
					array2[num++] = (ushort)((n + 1) % 5 + 1);
					array2[num++] = (ushort)((n + 1) % 5 + 1);
					array2[num++] = (ushort)(n * 2 + 7);
					array2[num++] = (ushort)((n * 2 + 2) % 10 + 6);
				}
				for (int num10 = 0; num10 < 2; num10++)
				{
					for (int num11 = 0; num11 < 5; num11++)
					{
						array2[num++] = (ushort)(num11 * 2 + 6 + num10 * 10);
						array2[num++] = (ushort)(num11 * 2 + 16 + num10 * 10);
						array2[num++] = (ushort)(num11 * 2 + 7 + num10 * 10);
						array2[num++] = (ushort)(num11 * 2 + 7 + num10 * 10);
						array2[num++] = (ushort)(num11 * 2 + 16 + num10 * 10);
						array2[num++] = (ushort)(num11 * 2 + 17 + num10 * 10);
						array2[num++] = (ushort)(num11 * 2 + 7 + num10 * 10);
						array2[num++] = (ushort)(num11 * 2 + 17 + num10 * 10);
						array2[num++] = (ushort)((num11 * 2 + 2) % 10 + 6 + num10 * 10);
						array2[num++] = (ushort)((num11 * 2 + 2) % 10 + 6 + num10 * 10);
						array2[num++] = (ushort)(num11 * 2 + 17 + num10 * 10);
						array2[num++] = (ushort)((num11 * 2 + 2) % 10 + 16 + num10 * 10);
					}
				}
				for (int num12 = 0; num12 < 5; num12++)
				{
					array2[num++] = (ushort)(num12 * 2 + 26);
					array2[num++] = (ushort)(num12 + 36);
					array2[num++] = (ushort)(num12 * 2 + 27);
					array2[num++] = (ushort)(num12 * 2 + 27);
					array2[num++] = (ushort)(num12 + 36);
					array2[num++] = (ushort)((num12 + 1) % 5 + 36);
					array2[num++] = (ushort)(num12 * 2 + 27);
					array2[num++] = (ushort)((num12 + 1) % 5 + 36);
					array2[num++] = (ushort)((num12 * 2 + 2) % 10 + 26);
				}
				for (int num13 = 0; num13 < 5; num13++)
				{
					array2[num++] = (ushort)(num13 + 36);
					array2[num++] = 41;
					array2[num++] = (ushort)((num13 + 1) % 5 + 36);
				}
				VortexEntity.sPrimitiveCount = array2.Length / 3;
				lock (graphicsDevice)
				{
					VortexEntity.sIndices = new IndexBuffer(graphicsDevice, 480, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
					VortexEntity.sIndices.SetData<ushort>(array2);
					VortexEntity.sVertexDeclaration = new VertexDeclaration(graphicsDevice, new VertexElement[]
					{
						new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0)
					});
				}
			}
			VortexEntity.sEffect = new VortexEffect();
			this.mBody = new Body();
			this.mCollision = new CollisionSkin();
			this.mBody.CollisionSkin = this.mCollision;
			this.mRenderData = new VortexEntity.RenderData[3];
			for (int num14 = 0; num14 < 3; num14++)
			{
				this.mRenderData[num14] = new VortexEntity.RenderData();
			}
		}

		// Token: 0x06002A50 RID: 10832 RVA: 0x0014C7D0 File Offset: 0x0014A9D0
		public void Initialize(ISpellCaster iOwner, Vector3 iPosition)
		{
			if (this.mVortexCue != null && !this.mVortexCue.IsStopping)
			{
				this.mVortexCue.Stop(AudioStopOptions.AsAuthored);
			}
			if (this.mOwner is Avatar && !((this.mOwner as Avatar).Player.Gamer is NetworkGamer) && this.mHawkingHoleCount >= 50)
			{
				AchievementsManager.Instance.AwardAchievement(iOwner.PlayState, "icallitahawkinghole");
			}
			EffectManager.Instance.Stop(ref this.mParticleEffect);
			this.mSphere = new BoundingSphere(iPosition, 0.5f);
			this.mRadius = 0f;
			this.mMass = 0.25f;
			this.mTargetMass = 0.25f;
			this.mHawkingHoleCount = 0;
			this.mOwner = iOwner;
			Vector3 vector = default(Vector3);
			vector.Z = -1f;
			this.mAudioEmitter.Position = iPosition;
			this.mAudioEmitter.Forward = Vector3.Right;
			this.mAudioEmitter.Up = Vector3.Up;
			if (this.mVortexCue == null || !this.mVortexCue.IsPlaying)
			{
				this.mVortexCue = AudioManager.Instance.PlayCue(Banks.Spells, VortexEntity.SOUNDHASH, this.mAudioEmitter);
			}
			EffectManager.Instance.StartEffect(VortexEntity.EFFECTHASH, ref this.mSphere.Center, ref vector, out this.mParticleEffect);
		}

		// Token: 0x06002A51 RID: 10833 RVA: 0x0014C92C File Offset: 0x0014AB2C
		public override void Deinitialize()
		{
			base.Deinitialize();
			if (this.mVortexCue != null && !this.mVortexCue.IsStopping)
			{
				this.mVortexCue.Stop(AudioStopOptions.AsAuthored);
			}
			if (this.mOwner is Avatar && !((this.mOwner as Avatar).Player.Gamer is NetworkGamer) && this.mHawkingHoleCount >= 50)
			{
				AchievementsManager.Instance.AwardAchievement(this.mOwner.PlayState, "icallitahawkinghole");
			}
			EffectManager.Instance.Stop(ref this.mParticleEffect);
		}

		// Token: 0x06002A52 RID: 10834 RVA: 0x0014C9C0 File Offset: 0x0014ABC0
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			base.Update(iDataChannel, iDeltaTime);
			this.mMass = MathHelper.Lerp(this.mTargetMass, this.mMass, (float)Math.Pow(0.1, (double)iDeltaTime));
			if (this.mMass < 1E-06f)
			{
				return;
			}
			this.mRadius = this.mMass * 10f;
			List<Entity> entities = this.mPlayState.EntityManager.GetEntities(this.mSphere.Center, this.mRadius, true);
			for (int i = 0; i < entities.Count; i++)
			{
				Entity entity = entities[i];
				if (entity is VortexEntity)
				{
					entities.RemoveAt(i--);
				}
				else if (!entity.Dead && (!(entity is Character) || !(entity as Character).IsEthereal) && !(entity is Portal.PortalEntity))
				{
					IDamageable damageable = entity as IDamageable;
					float mass = entity.Body.Mass;
					if (this.mSphere.Intersects(entity.Body.CollisionSkin.WorldBoundingBox))
					{
						if (damageable != null)
						{
							if (damageable is Character && !(damageable as Character).HasGibs())
							{
								(damageable as Character).Terminate(true, false);
							}
							else
							{
								damageable.OverKill();
							}
							if (this.mOwner is Avatar && !((this.mOwner as Avatar).Player.Gamer is NetworkGamer))
							{
								Damage iDamage = default(Damage);
								iDamage.Amount = damageable.HitPoints;
								StatisticsManager.Instance.AddDamageEvent(this.mOwner.PlayState, this.mOwner, damageable, this.mOwner.PlayState.PlayTime, iDamage, DamageResult.Killed | DamageResult.OverKilled);
								if (damageable is Character && ((damageable as Character).Faction & (this.mOwner as Avatar).Faction) != (this.mOwner as Avatar).Faction)
								{
									this.mHawkingHoleCount++;
								}
							}
						}
						else
						{
							entity.Kill();
						}
						if (entity.Dead)
						{
							float num = (float)(Math.Log((double)(1f - this.mTargetMass)) / VortexEntity.sLogD);
							this.mTargetMass = 1f - (float)Math.Pow(0.75, (double)(num + mass));
						}
					}
					else
					{
						Vector3 position = entity.Position;
						float num2;
						Vector3.Distance(ref this.mSphere.Center, ref position, out num2);
						if (num2 > 1E-06f)
						{
							float num3 = 15f * (1f - num2 / this.mRadius);
							if (num3 > 0f)
							{
								Vector3 additionalForce;
								Vector3.Subtract(ref this.mSphere.Center, ref position, out additionalForce);
								Vector3.Divide(ref additionalForce, num2, out additionalForce);
								Vector3.Multiply(ref additionalForce, num3, out additionalForce);
								Character character = entity as Character;
								if (character != null)
								{
									character.CharacterBody.AdditionalForce = additionalForce;
								}
								else
								{
									Vector3 velocity = entity.Body.Velocity;
									Vector3.Add(ref velocity, ref additionalForce, out velocity);
									entity.Body.Velocity = velocity;
								}
							}
						}
					}
				}
			}
			this.mPlayState.EntityManager.ReturnEntityList(entities);
			Matrix identity = Matrix.Identity;
			identity.Translation = this.mSphere.Center;
			float num4 = this.mRadius / 10f * 2f;
			if (num4 < 0.25f)
			{
				num4 = 0.25f;
			}
			MagickaMath.UniformMatrixScale(ref identity, num4);
			EffectManager.Instance.UpdateOrientation(ref this.mParticleEffect, ref identity);
			VortexEntity.RenderData renderData = this.mRenderData[(int)iDataChannel];
			Matrix transform;
			Matrix.CreateScale(this.mRadius, out transform);
			transform.Translation = this.mSphere.Center;
			renderData.Transform = transform;
			renderData.Time = this.mRadius;
			renderData.Radius = this.mRadius;
			this.mPlayState.Scene.AddPostEffect(iDataChannel, renderData);
			this.mTargetMass -= iDeltaTime * 0.05f;
			this.mAudioEmitter.Position = this.mSphere.Center;
			this.mAudioEmitter.Forward = Vector3.Right;
			this.mAudioEmitter.Up = Vector3.Up;
		}

		// Token: 0x170009F2 RID: 2546
		// (get) Token: 0x06002A53 RID: 10835 RVA: 0x0014CDDE File Offset: 0x0014AFDE
		public override bool Dead
		{
			get
			{
				return this.mMass < float.Epsilon;
			}
		}

		// Token: 0x170009F3 RID: 2547
		// (get) Token: 0x06002A54 RID: 10836 RVA: 0x0014CDED File Offset: 0x0014AFED
		public override bool Removable
		{
			get
			{
				return this.Dead;
			}
		}

		// Token: 0x06002A55 RID: 10837 RVA: 0x0014CDF5 File Offset: 0x0014AFF5
		public override void Kill()
		{
			this.mTargetMass = 0f;
		}

		// Token: 0x06002A56 RID: 10838 RVA: 0x0014CE02 File Offset: 0x0014B002
		protected override void INetworkUpdate(ref EntityUpdateMessage iMsg)
		{
			base.INetworkUpdate(ref iMsg);
			this.mMass = iMsg.GenericFloat;
			this.mTargetMass = iMsg.WanderAngle;
		}

		// Token: 0x06002A57 RID: 10839 RVA: 0x0014CE24 File Offset: 0x0014B024
		protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
		{
			oMsg = default(EntityUpdateMessage);
			oMsg.Features |= EntityFeatures.GenericFloat;
			oMsg.GenericFloat = this.mMass;
			oMsg.Features |= EntityFeatures.WanderAngle;
			oMsg.WanderAngle = this.mTargetMass;
		}

		// Token: 0x06002A58 RID: 10840 RVA: 0x0014CE76 File Offset: 0x0014B076
		internal override float GetDanger()
		{
			return this.mMass * 20f;
		}

		// Token: 0x04002DA9 RID: 11689
		private const float MAX_RADIUS = 10f;

		// Token: 0x04002DAA RID: 11690
		private const float START_MASS = 0.25f;

		// Token: 0x04002DAB RID: 11691
		private const float D = 0.75f;

		// Token: 0x04002DAC RID: 11692
		private static List<VortexEntity> sCache = null;

		// Token: 0x04002DAD RID: 11693
		public static readonly int EFFECTHASH = "magick_vortex".GetHashCodeCustom();

		// Token: 0x04002DAE RID: 11694
		public static readonly int SOUNDHASH = "magick_vortex".GetHashCodeCustom();

		// Token: 0x04002DAF RID: 11695
		private static readonly double sLogD = Math.Log(0.75);

		// Token: 0x04002DB0 RID: 11696
		private float mMass;

		// Token: 0x04002DB1 RID: 11697
		private float mTargetMass;

		// Token: 0x04002DB2 RID: 11698
		private new float mRadius;

		// Token: 0x04002DB3 RID: 11699
		private VortexEntity.RenderData[] mRenderData;

		// Token: 0x04002DB4 RID: 11700
		private static VortexEffect sEffect;

		// Token: 0x04002DB5 RID: 11701
		private static VertexBuffer sVertices;

		// Token: 0x04002DB6 RID: 11702
		private static IndexBuffer sIndices;

		// Token: 0x04002DB7 RID: 11703
		private static VertexDeclaration sVertexDeclaration;

		// Token: 0x04002DB8 RID: 11704
		private static int sNumVertices;

		// Token: 0x04002DB9 RID: 11705
		private static int sPrimitiveCount;

		// Token: 0x04002DBA RID: 11706
		private VisualEffectReference mParticleEffect;

		// Token: 0x04002DBB RID: 11707
		private ISpellCaster mOwner;

		// Token: 0x04002DBC RID: 11708
		private BoundingSphere mSphere;

		// Token: 0x04002DBD RID: 11709
		private Cue mVortexCue;

		// Token: 0x04002DBE RID: 11710
		private int mHawkingHoleCount;

		// Token: 0x02000589 RID: 1417
		private class RenderData : IPostEffect
		{
			// Token: 0x170009F4 RID: 2548
			// (get) Token: 0x06002A5A RID: 10842 RVA: 0x0014CEBD File Offset: 0x0014B0BD
			public int ZIndex
			{
				get
				{
					return 100;
				}
			}

			// Token: 0x06002A5B RID: 10843 RVA: 0x0014CEC4 File Offset: 0x0014B0C4
			public void Draw(float iDeltaTime, ref Vector2 iPixelSize, ref Matrix iViewMatrix, ref Matrix iProjectionMatrix, Texture2D iCandidate, Texture2D iDepthMap, Texture2D iNormalMap)
			{
				VortexEntity.sEffect.PixelSize = iPixelSize;
				VortexEntity.sEffect.World = this.Transform;
				VortexEntity.sEffect.View = iViewMatrix;
				VortexEntity.sEffect.Projection = iProjectionMatrix;
				VortexEntity.sEffect.SourceTexture = iCandidate;
				VortexEntity.sEffect.DepthTexture = iDepthMap;
				VortexEntity.sEffect.Distortion = 0.025f;
				VortexEntity.sEffect.DistortionPower = 2f;
				VortexEntity.sEffect.GraphicsDevice.Vertices[0].SetSource(VortexEntity.sVertices, 0, 12);
				VortexEntity.sEffect.GraphicsDevice.VertexDeclaration = VortexEntity.sVertexDeclaration;
				VortexEntity.sEffect.GraphicsDevice.Indices = VortexEntity.sIndices;
				VortexEntity.sEffect.Begin();
				VortexEntity.sEffect.CurrentTechnique.Passes[0].Begin();
				VortexEntity.sEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, VortexEntity.sNumVertices, 0, VortexEntity.sPrimitiveCount);
				VortexEntity.sEffect.CurrentTechnique.Passes[0].End();
				VortexEntity.sEffect.End();
			}

			// Token: 0x04002DBF RID: 11711
			public float Alpha = 1f;

			// Token: 0x04002DC0 RID: 11712
			public Matrix Transform;

			// Token: 0x04002DC1 RID: 11713
			public float Time;

			// Token: 0x04002DC2 RID: 11714
			public float Radius;
		}
	}
}
