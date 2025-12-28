using System;
using System.Collections.Generic;
using JigLibX.Geometry;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Magicka.Graphics.Effects;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x020002AA RID: 682
	public class Wave : SpecialAbility, IAbilityEffect
	{
		// Token: 0x0600149C RID: 5276 RVA: 0x0007FBC8 File Offset: 0x0007DDC8
		public static Wave GetInstance()
		{
			if (Wave.sCache.Count > 0)
			{
				Wave result = Wave.sCache[Wave.sCache.Count - 1];
				Wave.sCache.RemoveAt(Wave.sCache.Count - 1);
				return result;
			}
			return new Wave();
		}

		// Token: 0x0600149D RID: 5277 RVA: 0x0007FC18 File Offset: 0x0007DE18
		public static void InitializeCache(int iNr)
		{
			Wave.sCache = new List<Wave>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				Wave.sCache.Add(new Wave());
			}
		}

		// Token: 0x1700054C RID: 1356
		// (get) Token: 0x0600149E RID: 5278 RVA: 0x0007FC4B File Offset: 0x0007DE4B
		public ushort Handle
		{
			get
			{
				return this.mHandle;
			}
		}

		// Token: 0x0600149F RID: 5279 RVA: 0x0007FC54 File Offset: 0x0007DE54
		public Wave() : base(Animations.cast_magick_sweep, "#magick_wave".GetHashCodeCustom())
		{
			lock (Wave.sCache)
			{
				this.mHandle = (ushort)(Wave.sCache.Count - 1);
			}
			this.mAudioEmitter = new AudioEmitter();
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			this.EFFECT_RANGE = 20f;
			this.WEIGHT_THRESHOLD = 3000f;
			this.TTL = 1f;
			this.INV_TTL = 1f / this.TTL;
			this.RADIUS = 2.5f;
			this.RANGE = 6f;
			this.RANGE_INBETWEEN = 1.5f;
			if (Wave.sVertices == null)
			{
				Wave.Vertex[] array = new Wave.Vertex[32];
				int num = 0;
				float num2 = -2.3561945f;
				for (int i = 0; i < 8; i++)
				{
					float num3 = (float)Math.Cos((double)num2);
					float num4 = (float)Math.Sin((double)num2);
					array[num].Position.X = (array[num].TexCoord.X = num3 * 4f);
					array[num].Position.Z = (array[num].TexCoord.Y = num4 * 2f + 3.5f);
					array[num++].Alpha = 0f;
					array[num].Position.X = (array[num].TexCoord.X = num3 * 6f);
					array[num].Position.Z = (array[num].TexCoord.Y = num4 * 3f + 3.5f);
					array[num++].Alpha = ((i == 0 | i == 7) ? 0f : ((i == 1 | i == 6) ? 0.5f : 1f));
					array[num].Position.X = (array[num].TexCoord.X = num3 * 8f);
					array[num].Position.Z = (array[num].TexCoord.Y = num4 * 4f + 3.5f);
					array[num++].Alpha = ((i == 0 | i == 7) ? 0f : 1f);
					array[num].Position.X = (array[num].TexCoord.X = num3 * 10f);
					array[num].Position.Z = (array[num].TexCoord.Y = num4 * 5f + 3.5f);
					array[num++].Alpha = 0f;
					num2 += 0.22439948f;
				}
				Wave.sBoundingBox.Max.X = float.MinValue;
				Wave.sBoundingBox.Max.Y = 1f;
				Wave.sBoundingBox.Max.Z = float.MinValue;
				Wave.sBoundingBox.Min.X = float.MaxValue;
				Wave.sBoundingBox.Min.Y = -1f;
				Wave.sBoundingBox.Min.Z = float.MaxValue;
				Wave.sBoundingBox.Max = new Vector3(5.5f, 1f, 1f);
				Wave.sBoundingBox.Min = new Vector3(-5.5f, -1f, -1f);
				ushort[] array2 = new ushort[126];
				num = 0;
				for (int j = 0; j < 7; j++)
				{
					array2[num++] = (ushort)(j * 4);
					array2[num++] = (ushort)(j * 4 + 1);
					array2[num++] = (ushort)(j * 4 + 4);
					array2[num++] = (ushort)(j * 4 + 1);
					array2[num++] = (ushort)(j * 4 + 5);
					array2[num++] = (ushort)(j * 4 + 4);
					array2[num++] = (ushort)(j * 4 + 1);
					array2[num++] = (ushort)(j * 4 + 2);
					array2[num++] = (ushort)(j * 4 + 5);
					array2[num++] = (ushort)(j * 4 + 2);
					array2[num++] = (ushort)(j * 4 + 6);
					array2[num++] = (ushort)(j * 4 + 5);
					array2[num++] = (ushort)(j * 4 + 2);
					array2[num++] = (ushort)(j * 4 + 3);
					array2[num++] = (ushort)(j * 4 + 6);
					array2[num++] = (ushort)(j * 4 + 3);
					array2[num++] = (ushort)(j * 4 + 7);
					array2[num++] = (ushort)(j * 4 + 6);
				}
				Wave.sVertices = new VertexBuffer(graphicsDevice, 24 * array.Length, BufferUsage.WriteOnly);
				Wave.sVertices.SetData<Wave.Vertex>(array);
				Wave.sIndices = new IndexBuffer(graphicsDevice, 2 * array2.Length, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
				Wave.sIndices.SetData<ushort>(array2);
				Wave.sPrimitiveCount = array2.Length / 3;
				Wave.sVertexDeclaration = new VertexDeclaration(graphicsDevice, Wave.Vertex.VertexElements);
			}
			NormalDistortionEffect normalDistortionEffect = new NormalDistortionEffect(graphicsDevice, Game.Instance.Content);
			normalDistortionEffect.NormalTexture = Game.Instance.Content.Load<Texture2D>("EffectTextures/NormalDistortion");
			this.mRenderData = new Wave.RenderData[3];
			for (int k = 0; k < 3; k++)
			{
				Wave.RenderData renderData = new Wave.RenderData(normalDistortionEffect);
				this.mRenderData[k] = renderData;
			}
		}

		// Token: 0x060014A0 RID: 5280 RVA: 0x00080240 File Offset: 0x0007E440
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			Console.WriteLine("CLEAR Hitlist");
			this.mHitList.Clear();
			this.mDamageEntities.Clear();
			this.mWaveEntities.Clear();
			this.mTimeStamp = iPlayState.PlayTime;
			this.mTTL = this.TTL;
			this.mTime = 0f;
			this.mPlayState = iPlayState;
			this.mOwner = null;
			this.mPosition = iPosition;
			this.mStartPosition = iPosition;
			this.mDirection = new Vector3((float)SpecialAbility.RANDOM.NextDouble(), 0f, (float)SpecialAbility.RANDOM.NextDouble());
			this.mDirection.Normalize();
			Vector3.Add(ref this.mPosition, ref this.mDirection, out this.mPosition);
			Vector3.Add(ref this.mPosition, ref this.mDirection, out this.mPosition);
			Vector3.Add(ref this.mPosition, ref this.mDirection, out this.mPosition);
			this.mAudioEmitter.Position = this.mPosition;
			this.mAudioEmitter.Up = Vector3.Up;
			this.mAudioEmitter.Forward = Vector3.Right;
			SpellManager.Instance.AddSpellEffect(this);
			return true;
		}

		// Token: 0x060014A1 RID: 5281 RVA: 0x0008036C File Offset: 0x0007E56C
		public bool Execute(Vector3 iPosition, Vector3 iDirection, PlayState iPlayState)
		{
			this.mHitList.Clear();
			this.mDamageEntities.Clear();
			this.mWaveEntities.Clear();
			this.mTimeStamp = iPlayState.PlayTime;
			this.mTTL = this.TTL;
			this.mTime = 0f;
			this.mPlayState = iPlayState;
			this.mOwner = null;
			this.mPosition = iPosition;
			this.mStartPosition = iPosition;
			this.mDirection = iDirection;
			this.mAudioEmitter.Position = this.mPosition;
			this.mAudioEmitter.Up = Vector3.Up;
			this.mAudioEmitter.Forward = Vector3.Right;
			SpellManager.Instance.AddSpellEffect(this);
			return true;
		}

		// Token: 0x060014A2 RID: 5282 RVA: 0x00080420 File Offset: 0x0007E620
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			this.mHitList.Clear();
			this.mDamageEntities.Clear();
			this.mWaveEntities.Clear();
			this.mTTL = this.TTL;
			this.mTime = 0f;
			this.mPlayState = iPlayState;
			this.mOwner = iOwner;
			this.mTimeStamp = iOwner.PlayState.PlayTime;
			this.mPosition = iOwner.Position;
			this.mStartPosition = this.mPosition;
			this.mDirection = iOwner.Direction;
			Vector3.Add(ref this.mPosition, ref this.mDirection, out this.mPosition);
			Vector3.Add(ref this.mPosition, ref this.mDirection, out this.mPosition);
			Vector3.Add(ref this.mPosition, ref this.mDirection, out this.mPosition);
			this.mAudioEmitter.Position = this.mPosition;
			this.mAudioEmitter.Up = Vector3.Up;
			this.mAudioEmitter.Forward = Vector3.Right;
			SpellManager.Instance.AddSpellEffect(this);
			this.CreateEntity();
			return true;
		}

		// Token: 0x060014A3 RID: 5283 RVA: 0x0008053C File Offset: 0x0007E73C
		private void CreateEntity()
		{
			Vector3 position = this.mOwner.Position;
			Vector3 direction = this.mOwner.Direction;
			float radius = this.mOwner.Radius;
			Vector3.Multiply(ref direction, radius, out direction);
			Vector3.Add(ref direction, ref position, out position);
			direction = this.mOwner.Direction;
			Segment iSeg = default(Segment);
			iSeg.Delta.Y = -4f;
			iSeg.Origin = position;
			iSeg.Origin.Y = iSeg.Origin.Y + 2f;
			float radius2 = this.RADIUS;
			float num = radius2 * this.RANGE;
			Spell spell = default(Spell);
			spell.Element = (Elements.Earth | Elements.Steam);
			spell.EarthMagnitude = 3f;
			spell.SteamMagnitude = 2f;
			DamageCollection5 damage;
			spell.CalculateDamage(SpellType.Shield, CastType.Weapon, out damage);
			float num2;
			Vector3 value;
			Vector3 vector;
			AnimatedLevelPart animatedLevelPart;
			if (this.mOwner.PlayState.Level.CurrentScene.SegmentIntersect(out num2, out value, out vector, out animatedLevelPart, iSeg))
			{
				if (NetworkManager.Instance.State == NetworkState.Client)
				{
					SpawnWaveRequestMessage spawnWaveRequestMessage;
					spawnWaveRequestMessage.OwnerHandle = this.mOwner.Handle;
					spawnWaveRequestMessage.AnimationHandle = ((animatedLevelPart == null) ? ushort.MaxValue : animatedLevelPart.Handle);
					spawnWaveRequestMessage.Position = value + direction * this.RANGE_INBETWEEN * 2f;
					spawnWaveRequestMessage.Direction = new Vector3(direction.Z, 0f, -direction.X);
					spawnWaveRequestMessage.Scale = 1f;
					spawnWaveRequestMessage.Spell = spell;
					spawnWaveRequestMessage.Damage = damage;
					spawnWaveRequestMessage.Range = num;
					spawnWaveRequestMessage.Distance = num;
					spawnWaveRequestMessage.NextDir = direction * this.RANGE_INBETWEEN;
					spawnWaveRequestMessage.NextRotation = Quaternion.Identity;
					spawnWaveRequestMessage.ParentHandle = 0;
					NetworkManager.Instance.Interface.SendMessage<SpawnWaveRequestMessage>(ref spawnWaveRequestMessage, 0);
					return;
				}
				WaveEntity fromCache = WaveEntity.GetFromCache(this.mOwner.PlayState);
				Barrier.HitListWithBarriers fromCache2 = Barrier.HitListWithBarriers.GetFromCache();
				Wave wave = this;
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					SpawnWaveMessage spawnWaveMessage;
					spawnWaveMessage.Handle = fromCache.Handle;
					spawnWaveMessage.OwnerHandle = this.mOwner.Handle;
					spawnWaveMessage.AnimationHandle = ((animatedLevelPart == null) ? ushort.MaxValue : animatedLevelPart.Handle);
					spawnWaveMessage.Position = value + direction * this.RANGE_INBETWEEN * 2f;
					spawnWaveMessage.Direction = new Vector3(direction.Z, 0f, -direction.X);
					spawnWaveMessage.Scale = 1f;
					spawnWaveMessage.Spell = spell;
					spawnWaveMessage.Damage = damage;
					spawnWaveMessage.HitlistHandle = fromCache2.Handle;
					spawnWaveMessage.ParentHandle = 0;
					NetworkManager.Instance.Interface.SendMessage<SpawnWaveMessage>(ref spawnWaveMessage);
				}
				fromCache.Initialize(this.mOwner, value + direction * this.RANGE_INBETWEEN * 2f, new Vector3(direction.Z, 0f, -direction.X), 1f, num, direction * this.RANGE_INBETWEEN, Quaternion.Identity, radius2, ref spell, ref damage, ref fromCache2, animatedLevelPart, ref wave);
				this.AddEntity(fromCache);
				this.mOwner.PlayState.EntityManager.AddEntity(fromCache);
			}
		}

		// Token: 0x1700054D RID: 1357
		// (get) Token: 0x060014A4 RID: 5284 RVA: 0x00080896 File Offset: 0x0007EA96
		public bool IsDead
		{
			get
			{
				return this.mTTL <= 0f;
			}
		}

		// Token: 0x060014A5 RID: 5285 RVA: 0x000808A8 File Offset: 0x0007EAA8
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mTTL -= iDeltaTime;
			this.mTime += iDeltaTime * this.INV_TTL;
			Wave.RenderData renderData = this.mRenderData[(int)iDataChannel];
			Vector3 vector;
			Vector3.Multiply(ref this.mDirection, this.mTime * this.mTime * this.EFFECT_RANGE, out vector);
			Vector3.Add(ref this.mPosition, ref vector, out vector);
			NetworkState state = NetworkManager.Instance.State;
			Vector3 vector2 = default(Vector3);
			vector2.Y = 1f;
			Matrix.CreateWorld(ref vector, ref this.mDirection, ref vector2, out renderData.Transform);
			renderData.Time = this.mTime;
			Wave.RenderData renderData2 = renderData;
			renderData2.Transform.M31 = renderData2.Transform.M31 * (1f + this.mTime);
			Wave.RenderData renderData3 = renderData;
			renderData3.Transform.M32 = renderData3.Transform.M32 * (1f + this.mTime);
			Wave.RenderData renderData4 = renderData;
			renderData4.Transform.M33 = renderData4.Transform.M33 * (1f + this.mTime);
			this.mAudioEmitter.Position = this.mPosition;
			this.mAudioEmitter.Up = Vector3.Up;
			this.mAudioEmitter.Forward = Vector3.Right;
			EffectManager.Instance.UpdatePositionDirection(ref this.mEffect, ref vector, ref this.mDirection);
			this.mPlayState.Scene.AddPostEffect(iDataChannel, renderData);
		}

		// Token: 0x060014A6 RID: 5286 RVA: 0x00080A07 File Offset: 0x0007EC07
		public void AddEntity(WaveEntity ent)
		{
			this.mWaveEntities.Add(ent);
		}

		// Token: 0x060014A7 RID: 5287 RVA: 0x00080A15 File Offset: 0x0007EC15
		public void AddToHitlist(ushort handle)
		{
			this.mHitList.Add(handle);
		}

		// Token: 0x060014A8 RID: 5288 RVA: 0x00080A23 File Offset: 0x0007EC23
		public bool InHitlist(IDamageable target)
		{
			return this.mHitList.Contains(target.Handle);
		}

		// Token: 0x060014A9 RID: 5289 RVA: 0x00080A3B File Offset: 0x0007EC3B
		public void OnRemove()
		{
			EffectManager.Instance.Stop(ref this.mEffect);
			Wave.sCache.Add(this);
		}

		// Token: 0x040015E5 RID: 5605
		private static List<Wave> sCache;

		// Token: 0x040015E6 RID: 5606
		private ushort mHandle;

		// Token: 0x040015E7 RID: 5607
		private static readonly int SOUND;

		// Token: 0x040015E8 RID: 5608
		private static readonly int EFFECT;

		// Token: 0x040015E9 RID: 5609
		private readonly float EFFECT_RANGE;

		// Token: 0x040015EA RID: 5610
		public readonly float WEIGHT_THRESHOLD;

		// Token: 0x040015EB RID: 5611
		private readonly float TTL;

		// Token: 0x040015EC RID: 5612
		private readonly float INV_TTL;

		// Token: 0x040015ED RID: 5613
		private readonly float RADIUS;

		// Token: 0x040015EE RID: 5614
		private readonly float RANGE;

		// Token: 0x040015EF RID: 5615
		private readonly float RANGE_INBETWEEN;

		// Token: 0x040015F0 RID: 5616
		private static IndexBuffer sIndices;

		// Token: 0x040015F1 RID: 5617
		private static VertexBuffer sVertices;

		// Token: 0x040015F2 RID: 5618
		private static VertexDeclaration sVertexDeclaration;

		// Token: 0x040015F3 RID: 5619
		private static int sPrimitiveCount;

		// Token: 0x040015F4 RID: 5620
		private static BoundingBox sBoundingBox;

		// Token: 0x040015F5 RID: 5621
		private static readonly Damage mDamage = new Damage(AttackProperties.Damage | AttackProperties.Knockdown | AttackProperties.Pushed, Elements.Earth, 400f, 1f);

		// Token: 0x040015F6 RID: 5622
		private float mTTL;

		// Token: 0x040015F7 RID: 5623
		private float mTime;

		// Token: 0x040015F8 RID: 5624
		private ISpellCaster mOwner;

		// Token: 0x040015F9 RID: 5625
		private PlayState mPlayState;

		// Token: 0x040015FA RID: 5626
		private Wave.RenderData[] mRenderData;

		// Token: 0x040015FB RID: 5627
		private AudioEmitter mAudioEmitter;

		// Token: 0x040015FC RID: 5628
		private Vector3 mPosition;

		// Token: 0x040015FD RID: 5629
		private Vector3 mDirection;

		// Token: 0x040015FE RID: 5630
		private Vector3 mStartPosition;

		// Token: 0x040015FF RID: 5631
		private readonly List<WaveEntity> mWaveEntities = new List<WaveEntity>();

		// Token: 0x04001600 RID: 5632
		private readonly List<Entity> mDamageEntities = new List<Entity>(128);

		// Token: 0x04001601 RID: 5633
		private readonly List<ushort> mHitList = new List<ushort>();

		// Token: 0x04001602 RID: 5634
		private VisualEffectReference mEffect;

		// Token: 0x04001603 RID: 5635
		private new double mTimeStamp;

		// Token: 0x020002AB RID: 683
		private class RenderData : IPostEffect
		{
			// Token: 0x060014AB RID: 5291 RVA: 0x00080A70 File Offset: 0x0007EC70
			public RenderData(NormalDistortionEffect iEffect)
			{
				this.mEffect = iEffect;
			}

			// Token: 0x1700054E RID: 1358
			// (get) Token: 0x060014AC RID: 5292 RVA: 0x00080A7F File Offset: 0x0007EC7F
			public int ZIndex
			{
				get
				{
					return 89;
				}
			}

			// Token: 0x060014AD RID: 5293 RVA: 0x00080A84 File Offset: 0x0007EC84
			public void Draw(float iDeltaTime, ref Vector2 iPixelSize, ref Matrix iViewMatrix, ref Matrix iProjectionMatrix, Texture2D iCandidate, Texture2D iDepthMap, Texture2D iNormalMap)
			{
				this.mEffect.PixelSize = iPixelSize;
				this.mEffect.World = this.Transform;
				this.mEffect.View = iViewMatrix;
				this.mEffect.Projection = iProjectionMatrix;
				this.mEffect.SourceTexture = iCandidate;
				this.mEffect.DepthTexture = iDepthMap;
				this.mEffect.Time = this.Time;
				float distortion = this.Time * this.Time * (1f - this.Time) * 6.75f;
				this.mEffect.Distortion = distortion;
				this.mEffect.NormalTexture = Game.Instance.Content.Load<Texture2D>("EffectTextures/NormalDistortion");
				this.mEffect.GraphicsDevice.Vertices[0].SetSource(Wave.sVertices, 0, 24);
				this.mEffect.GraphicsDevice.VertexDeclaration = Wave.sVertexDeclaration;
				this.mEffect.GraphicsDevice.Indices = Wave.sIndices;
				this.mEffect.TextureScale = 4f;
				this.mEffect.Begin();
				this.mEffect.CurrentTechnique.Passes[0].Begin();
				this.mEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 0, 32, 0, Wave.sPrimitiveCount);
				this.mEffect.CurrentTechnique.Passes[0].End();
				this.mEffect.End();
			}

			// Token: 0x04001604 RID: 5636
			public Matrix Transform;

			// Token: 0x04001605 RID: 5637
			public float Time;

			// Token: 0x04001606 RID: 5638
			private readonly NormalDistortionEffect mEffect;
		}

		// Token: 0x020002AC RID: 684
		private struct Vertex
		{
			// Token: 0x04001607 RID: 5639
			public const int SIZEINBYTES = 24;

			// Token: 0x04001608 RID: 5640
			public Vector3 Position;

			// Token: 0x04001609 RID: 5641
			public Vector2 TexCoord;

			// Token: 0x0400160A RID: 5642
			public float Alpha;

			// Token: 0x0400160B RID: 5643
			public static readonly VertexElement[] VertexElements = new VertexElement[]
			{
				new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
				new VertexElement(0, 12, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
				new VertexElement(0, 20, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.Color, 0)
			};
		}
	}
}
