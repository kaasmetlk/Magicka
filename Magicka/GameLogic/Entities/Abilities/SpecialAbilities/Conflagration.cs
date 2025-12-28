using System;
using System.Collections.Generic;
using JigLibX.Geometry;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
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
	// Token: 0x020005AD RID: 1453
	public class Conflagration : SpecialAbility, IAbilityEffect
	{
		// Token: 0x06002B82 RID: 11138 RVA: 0x00157494 File Offset: 0x00155694
		public static Conflagration GetInstance()
		{
			if (Conflagration.sCache.Count > 0)
			{
				Conflagration result = Conflagration.sCache[Conflagration.sCache.Count - 1];
				Conflagration.sCache.RemoveAt(Conflagration.sCache.Count - 1);
				return result;
			}
			return new Conflagration();
		}

		// Token: 0x06002B83 RID: 11139 RVA: 0x001574E4 File Offset: 0x001556E4
		public static void InitializeCache(int iNr)
		{
			Conflagration.sCache = new List<Conflagration>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				Conflagration.sCache.Add(new Conflagration());
			}
		}

		// Token: 0x06002B84 RID: 11140 RVA: 0x00157518 File Offset: 0x00155718
		private Conflagration() : base(Animations.cast_magick_sweep, "#magick_conflagration".GetHashCodeCustom())
		{
			this.mAudioEmitter = new AudioEmitter();
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			if (Conflagration.sVertices == null)
			{
				Conflagration.Vertex[] array = new Conflagration.Vertex[32];
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
				Conflagration.sBoundingBox.Max.X = float.MinValue;
				Conflagration.sBoundingBox.Max.Y = 1f;
				Conflagration.sBoundingBox.Max.Z = float.MinValue;
				Conflagration.sBoundingBox.Min.X = float.MaxValue;
				Conflagration.sBoundingBox.Min.Y = -1f;
				Conflagration.sBoundingBox.Min.Z = float.MaxValue;
				Conflagration.sBoundingBox.Max = new Vector3(5.5f, 1f, 1f);
				Conflagration.sBoundingBox.Min = new Vector3(-5.5f, -1f, -1f);
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
				Conflagration.sVertices = new VertexBuffer(graphicsDevice, 24 * array.Length, BufferUsage.WriteOnly);
				Conflagration.sVertices.SetData<Conflagration.Vertex>(array);
				Conflagration.sIndices = new IndexBuffer(graphicsDevice, 2 * array2.Length, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
				Conflagration.sIndices.SetData<ushort>(array2);
				Conflagration.sPrimitiveCount = array2.Length / 3;
				Conflagration.sVertexDeclaration = new VertexDeclaration(graphicsDevice, Conflagration.Vertex.VertexElements);
			}
			NormalDistortionEffect normalDistortionEffect = new NormalDistortionEffect(graphicsDevice, Game.Instance.Content);
			normalDistortionEffect.NormalTexture = Game.Instance.Content.Load<Texture2D>("EffectTextures/NormalDistortion");
			this.mRenderData = new Conflagration.RenderData[3];
			for (int k = 0; k < 3; k++)
			{
				Conflagration.RenderData renderData = new Conflagration.RenderData(normalDistortionEffect);
				this.mRenderData[k] = renderData;
			}
		}

		// Token: 0x06002B85 RID: 11141 RVA: 0x00157A5C File Offset: 0x00155C5C
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			this.mHitList.Clear();
			this.mTimeStamp = iPlayState.PlayTime;
			this.mTTL = 1f;
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
			AudioManager.Instance.PlayCue(Banks.Spells, Conflagration.SOUND, this.mAudioEmitter);
			SpellManager.Instance.AddSpellEffect(this);
			return true;
		}

		// Token: 0x06002B86 RID: 11142 RVA: 0x00157B80 File Offset: 0x00155D80
		public bool Execute(Vector3 iPosition, Vector3 iDirection, PlayState iPlayState)
		{
			this.mHitList.Clear();
			this.mTimeStamp = iPlayState.PlayTime;
			this.mTTL = 1f;
			this.mTime = 0f;
			this.mPlayState = iPlayState;
			this.mOwner = null;
			this.mPosition = iPosition;
			this.mStartPosition = iPosition;
			this.mDirection = iDirection;
			this.mAudioEmitter.Position = this.mPosition;
			this.mAudioEmitter.Up = Vector3.Up;
			this.mAudioEmitter.Forward = Vector3.Right;
			AudioManager.Instance.PlayCue(Banks.Spells, Conflagration.SOUND, this.mAudioEmitter);
			EffectManager.Instance.StartEffect(Conflagration.EFFECT, ref this.mPosition, ref this.mDirection, out this.mEffect);
			SpellManager.Instance.AddSpellEffect(this);
			return true;
		}

		// Token: 0x06002B87 RID: 11143 RVA: 0x00157C54 File Offset: 0x00155E54
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			this.mHitList.Clear();
			this.mTTL = 1f;
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
			AudioManager.Instance.PlayCue(Banks.Spells, Conflagration.SOUND, this.mAudioEmitter);
			EffectManager.Instance.StartEffect(Conflagration.EFFECT, ref this.mPosition, ref this.mDirection, out this.mEffect);
			SpellManager.Instance.AddSpellEffect(this);
			return true;
		}

		// Token: 0x17000A35 RID: 2613
		// (get) Token: 0x06002B88 RID: 11144 RVA: 0x00157D8A File Offset: 0x00155F8A
		public bool IsDead
		{
			get
			{
				return this.mTTL <= 0f;
			}
		}

		// Token: 0x06002B89 RID: 11145 RVA: 0x00157D9C File Offset: 0x00155F9C
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mTTL -= iDeltaTime;
			this.mTime += iDeltaTime * 1f;
			Conflagration.RenderData renderData = this.mRenderData[(int)iDataChannel];
			Vector3 iCenter;
			Vector3.Multiply(ref this.mDirection, this.mTime * this.mTime * 20f, out iCenter);
			Vector3.Add(ref this.mPosition, ref iCenter, out iCenter);
			NetworkState state = NetworkManager.Instance.State;
			Vector3 vector = default(Vector3);
			vector.Y = 1f;
			Matrix.CreateWorld(ref iCenter, ref this.mDirection, ref vector, out renderData.Transform);
			renderData.Time = this.mTime;
			if ((state != NetworkState.Client && (!(this.mOwner is Avatar) || !((this.mOwner as Avatar).Player.Gamer is NetworkGamer))) || (state == NetworkState.Client && this.mOwner is Avatar && !((this.mOwner as Avatar).Player.Gamer is NetworkGamer)))
			{
				BoundingSphere boundingSphere = default(BoundingSphere);
				Matrix matrix;
				Matrix.Invert(ref renderData.Transform, out matrix);
				List<Entity> entities = this.mPlayState.EntityManager.GetEntities(iCenter, 10f, false);
				List<Shield> shields = this.mPlayState.EntityManager.Shields;
				for (int i = 0; i < entities.Count; i++)
				{
					IDamageable damageable = entities[i] as IDamageable;
					if (!(damageable == null | (this.mOwner != null && damageable == this.mOwner) | this.mHitList.ContainsKey(entities[i].Handle)))
					{
						boundingSphere.Center = damageable.Position;
						Vector3.Transform(ref boundingSphere.Center, ref matrix, out boundingSphere.Center);
						boundingSphere.Radius = damageable.Radius;
						ContainmentType containmentType;
						boundingSphere.Contains(ref Conflagration.sBoundingBox, out containmentType);
						Segment iSeg = default(Segment);
						iSeg.Origin = this.mStartPosition;
						Vector3 position = damageable.Position;
						Vector3.Subtract(ref position, ref this.mStartPosition, out iSeg.Delta);
						bool flag = false;
						for (int j = 0; j < shields.Count; j++)
						{
							Vector3 vector2;
							if (shields[j].SegmentIntersect(out vector2, iSeg, 1f))
							{
								flag = true;
								break;
							}
						}
						if (containmentType != ContainmentType.Disjoint && !flag)
						{
							if (damageable is Character)
							{
								if (!(damageable as Character).IsEthereal)
								{
									damageable.Damage(Conflagration.sInstantDamage, this.mOwner as Entity, this.mTimeStamp, this.mPosition);
									damageable.Damage(Conflagration.sStatusDamage, this.mOwner as Entity, this.mTimeStamp, this.mPosition);
									this.mHitList.Add(damageable.Handle);
								}
							}
							else
							{
								damageable.Damage(Conflagration.sInstantDamage, this.mOwner as Entity, this.mTimeStamp, this.mPosition);
								damageable.Damage(Conflagration.sStatusDamage, this.mOwner as Entity, this.mTimeStamp, this.mPosition);
								this.mHitList.Add(damageable.Handle);
							}
						}
					}
				}
				this.mPlayState.EntityManager.ReturnEntityList(entities);
			}
			Conflagration.RenderData renderData2 = renderData;
			renderData2.Transform.M31 = renderData2.Transform.M31 * (1f + this.mTime);
			Conflagration.RenderData renderData3 = renderData;
			renderData3.Transform.M32 = renderData3.Transform.M32 * (1f + this.mTime);
			Conflagration.RenderData renderData4 = renderData;
			renderData4.Transform.M33 = renderData4.Transform.M33 * (1f + this.mTime);
			this.mAudioEmitter.Position = this.mPosition;
			this.mAudioEmitter.Up = Vector3.Up;
			this.mAudioEmitter.Forward = Vector3.Right;
			EffectManager.Instance.UpdatePositionDirection(ref this.mEffect, ref iCenter, ref this.mDirection);
			this.mPlayState.Scene.AddPostEffect(iDataChannel, renderData);
		}

		// Token: 0x06002B8A RID: 11146 RVA: 0x0015819A File Offset: 0x0015639A
		public void OnRemove()
		{
			EffectManager.Instance.Stop(ref this.mEffect);
			Conflagration.sCache.Add(this);
		}

		// Token: 0x04002F25 RID: 12069
		private const float RANGE = 20f;

		// Token: 0x04002F26 RID: 12070
		private const float TTL = 1f;

		// Token: 0x04002F27 RID: 12071
		private const float INV_TTL = 1f;

		// Token: 0x04002F28 RID: 12072
		private static List<Conflagration> sCache;

		// Token: 0x04002F29 RID: 12073
		public static readonly int SOUND = "magick_conflagration".GetHashCodeCustom();

		// Token: 0x04002F2A RID: 12074
		public static readonly int EFFECT = "magick_conflagration".GetHashCodeCustom();

		// Token: 0x04002F2B RID: 12075
		private static IndexBuffer sIndices;

		// Token: 0x04002F2C RID: 12076
		private static VertexBuffer sVertices;

		// Token: 0x04002F2D RID: 12077
		private static VertexDeclaration sVertexDeclaration;

		// Token: 0x04002F2E RID: 12078
		private static int sPrimitiveCount;

		// Token: 0x04002F2F RID: 12079
		private static BoundingBox sBoundingBox;

		// Token: 0x04002F30 RID: 12080
		private static Damage sInstantDamage = new Damage(AttackProperties.Damage, Elements.Fire, 600f, 1f);

		// Token: 0x04002F31 RID: 12081
		private static Damage sStatusDamage = new Damage(AttackProperties.Status, Elements.Fire, 75f, 2f);

		// Token: 0x04002F32 RID: 12082
		private float mTTL;

		// Token: 0x04002F33 RID: 12083
		private float mTime;

		// Token: 0x04002F34 RID: 12084
		private ISpellCaster mOwner;

		// Token: 0x04002F35 RID: 12085
		private PlayState mPlayState;

		// Token: 0x04002F36 RID: 12086
		private Conflagration.RenderData[] mRenderData;

		// Token: 0x04002F37 RID: 12087
		private AudioEmitter mAudioEmitter;

		// Token: 0x04002F38 RID: 12088
		private Vector3 mPosition;

		// Token: 0x04002F39 RID: 12089
		private Vector3 mDirection;

		// Token: 0x04002F3A RID: 12090
		private Vector3 mStartPosition;

		// Token: 0x04002F3B RID: 12091
		private HitList mHitList = new HitList(64);

		// Token: 0x04002F3C RID: 12092
		private VisualEffectReference mEffect;

		// Token: 0x04002F3D RID: 12093
		private new double mTimeStamp;

		// Token: 0x020005AE RID: 1454
		private class RenderData : IPostEffect
		{
			// Token: 0x06002B8C RID: 11148 RVA: 0x00158210 File Offset: 0x00156410
			public RenderData(NormalDistortionEffect iEffect)
			{
				this.mEffect = iEffect;
			}

			// Token: 0x17000A36 RID: 2614
			// (get) Token: 0x06002B8D RID: 11149 RVA: 0x0015822A File Offset: 0x0015642A
			public int ZIndex
			{
				get
				{
					return 89;
				}
			}

			// Token: 0x06002B8E RID: 11150 RVA: 0x00158230 File Offset: 0x00156430
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
				this.mEffect.GraphicsDevice.Vertices[0].SetSource(Conflagration.sVertices, 0, 24);
				this.mEffect.GraphicsDevice.VertexDeclaration = Conflagration.sVertexDeclaration;
				this.mEffect.GraphicsDevice.Indices = Conflagration.sIndices;
				this.mEffect.TextureScale = 4f;
				this.mEffect.Begin();
				this.mEffect.CurrentTechnique.Passes[0].Begin();
				this.mEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 0, 32, 0, Conflagration.sPrimitiveCount);
				this.mEffect.CurrentTechnique.Passes[0].End();
				this.mEffect.End();
			}

			// Token: 0x04002F3E RID: 12094
			public float Alpha = 1f;

			// Token: 0x04002F3F RID: 12095
			public Matrix Transform;

			// Token: 0x04002F40 RID: 12096
			public float Time;

			// Token: 0x04002F41 RID: 12097
			private NormalDistortionEffect mEffect;
		}

		// Token: 0x020005AF RID: 1455
		private struct Vertex
		{
			// Token: 0x04002F42 RID: 12098
			public const int SIZEINBYTES = 24;

			// Token: 0x04002F43 RID: 12099
			public Vector3 Position;

			// Token: 0x04002F44 RID: 12100
			public Vector2 TexCoord;

			// Token: 0x04002F45 RID: 12101
			public float Alpha;

			// Token: 0x04002F46 RID: 12102
			public static readonly VertexElement[] VertexElements = new VertexElement[]
			{
				new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
				new VertexElement(0, 12, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
				new VertexElement(0, 20, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.Color, 0)
			};
		}
	}
}
