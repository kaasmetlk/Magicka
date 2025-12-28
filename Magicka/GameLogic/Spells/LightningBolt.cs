using System;
using System.Collections.Generic;
using JigLibX.Geometry;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Magicka.Graphics.Lights;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.Spells
{
	// Token: 0x020003F5 RID: 1013
	public class LightningBolt : IAbilityEffect
	{
		// Token: 0x17000791 RID: 1937
		// (get) Token: 0x06001EF2 RID: 7922 RVA: 0x000D857E File Offset: 0x000D677E
		// (set) Token: 0x06001EF3 RID: 7923 RVA: 0x000D8586 File Offset: 0x000D6786
		public bool AirToSurface
		{
			get
			{
				return this.mAirToSurface;
			}
			set
			{
				this.mAirToSurface = value;
			}
		}

		// Token: 0x06001EF4 RID: 7924 RVA: 0x000D8590 File Offset: 0x000D6790
		private LightningBolt(ContentManager iContent)
		{
			this.mLight = new CapsuleLight(iContent);
			VertexPositionColorTexture[] array = new VertexPositionColorTexture[34];
			VertexPositionColorTexture vertexPositionColorTexture = default(VertexPositionColorTexture);
			vertexPositionColorTexture.Color = Color.White;
			float num = (float)MagickaMath.Random.Next(8) * 0.125f;
			float num2 = 0f;
			for (int i = 0; i < array.Length; i++)
			{
				if (i > 0 & i % 2 == 0)
				{
					num2 = MagickaMath.RandomBetween(-0.75f, 0.75f);
				}
				vertexPositionColorTexture.Position.X = ((float)(i % 2) - 0.5f + num2) * -1f;
				vertexPositionColorTexture.Position.Y = 0f;
				vertexPositionColorTexture.Position.Z = (float)(-(float)(i / 2)) * 2f;
				vertexPositionColorTexture.TextureCoordinate.X = (float)(i / 2) * 0.125f + num;
				vertexPositionColorTexture.TextureCoordinate.Y = 0.375f + (float)(i % 2) / 8f;
				array[i] = vertexPositionColorTexture;
			}
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			lock (graphicsDevice)
			{
				this.mVertices = new VertexBuffer(graphicsDevice, array.Length * VertexPositionColorTexture.SizeInBytes, BufferUsage.WriteOnly);
				this.mVertices.SetData<VertexPositionColorTexture>(array);
				this.mVertexDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, VertexPositionColorTexture.VertexElements);
			}
			AdditiveMaterial mMaterial = default(AdditiveMaterial);
			lock (graphicsDevice)
			{
				mMaterial.Texture = iContent.Load<Texture2D>("EffectTextures/Beams");
			}
			mMaterial.TextureEnabled = true;
			mMaterial.VertexColorEnabled = false;
			this.mRenderData = new LightningBolt.RenderData[3];
			for (int j = 0; j < 3; j++)
			{
				LightningBolt.RenderData renderData = new LightningBolt.RenderData();
				this.mRenderData[j] = renderData;
				renderData.mVertices = this.mVertices;
				renderData.mVerticesHash = this.mVertices.GetHashCode();
				renderData.mVertexDeclaration = this.mVertexDeclaration;
				renderData.mMaterial = mMaterial;
			}
		}

		// Token: 0x06001EF5 RID: 7925 RVA: 0x000D87C4 File Offset: 0x000D69C4
		public void Dispose()
		{
			this.mVertices.Dispose();
			this.mVertexDeclaration.Dispose();
		}

		// Token: 0x06001EF6 RID: 7926 RVA: 0x000D87DC File Offset: 0x000D69DC
		public static void InitializeCache(ContentManager iContent, int iSize)
		{
			LightningBolt.sContent = iContent;
			LightningBolt.sCache = new List<LightningBolt>(iSize);
			for (int i = 0; i < iSize; i++)
			{
				LightningBolt.sCache.Add(new LightningBolt(LightningBolt.sContent));
			}
		}

		// Token: 0x06001EF7 RID: 7927 RVA: 0x000D881C File Offset: 0x000D6A1C
		public static LightningBolt GetLightning()
		{
			if (LightningBolt.sCache.Count == 0)
			{
				return new LightningBolt(LightningBolt.sContent);
			}
			int index = MagickaMath.Random.Next(LightningBolt.sCache.Count);
			LightningBolt result = LightningBolt.sCache[index];
			LightningBolt.sCache.RemoveAt(index);
			return result;
		}

		// Token: 0x06001EF8 RID: 7928 RVA: 0x000D8870 File Offset: 0x000D6A70
		public void Cast(ISpellCaster iCaster, Vector3 iSource, Entity iTarget, HitList iHitList, Vector3 iColor, float iScale, float iRange, ref DamageCollection5 iDamages, PlayState iState)
		{
			if (iCaster is Magicka.GameLogic.Entities.Character)
			{
				(iCaster as Magicka.GameLogic.Entities.Character).GetSpellRangeModifier(ref iRange);
			}
			iHitList.Add(iTarget);
			DamageResult damageResult = DamageResult.None;
			this.mTimeStamp = iCaster.PlayState.PlayTime;
			if (iTarget is Magicka.GameLogic.Entities.Character && (iTarget as Magicka.GameLogic.Entities.Character).IsBlocking)
			{
				Spell spell;
				Spell.DefaultSpell(Elements.Lightning, out spell);
				(iTarget as Magicka.GameLogic.Entities.Character).Equipment[0].Item.TryAddToQueue(ref spell, true);
				damageResult |= DamageResult.Deflected;
			}
			else
			{
				damageResult = ((IDamageable)iTarget).Damage(iDamages, iCaster as Entity, this.mTimeStamp, iSource);
			}
			if (damageResult == DamageResult.Deflected)
			{
				Vector3 position = iTarget.Position;
				Vector3 position2 = iState.Scene.Camera.Position;
				Vector3 vector;
				Vector3.Subtract(ref position, ref iSource, out vector);
				vector.Normalize();
				this.InitializeEffect(ref iSource, ref vector, ref position, ref position2, ref iColor, false, iScale, 0.4f, iState);
				return;
			}
			Vector3 vector2 = iTarget.Position;
			Shield shield = iTarget as Shield;
			if (shield != null)
			{
				vector2 = shield.GetNearestPosition(iSource);
			}
			Vector3 position3 = iState.Scene.Camera.Position;
			Vector3 vector3;
			Vector3.Subtract(ref vector2, ref iSource, out vector3);
			vector3.Normalize();
			this.InitializeEffect(ref iSource, ref vector3, ref vector2, ref position3, ref iColor, false, iScale, 0.4f, iState);
			if (iRange > 0.05f && !(iTarget is Barrier | iTarget is Shield | iTarget is BossDamageZone))
			{
				List<Entity> entities;
				if (this.AirToSurface)
				{
					float y = iSource.Y;
					iSource.Y = 0f;
					entities = iState.EntityManager.GetEntities(vector2, iRange, true);
					iSource.Y = y;
				}
				else
				{
					entities = iState.EntityManager.GetEntities(vector2, iRange, true);
				}
				IDamageable damageable = null;
				float num = float.MaxValue;
				default(Segment).Origin = vector2;
				for (int i = 0; i < entities.Count; i++)
				{
					IDamageable damageable2 = entities[i] as IDamageable;
					if (damageable2 != null && !damageable2.Dead && !iHitList.Contains(damageable2) && (damageable2 is ElementalEgg | damageable2 is DamageablePhysicsEntity | (damageable2 is Magicka.GameLogic.Entities.Character && !(damageable2 as Magicka.GameLogic.Entities.Character).IsEthereal) | (damageable2 is Barrier && (damageable2 as Barrier).Solid) | damageable2 is MissileEntity | damageable2 is BossDamageZone))
					{
						IStatusEffected statusEffected = damageable2 as IStatusEffected;
						if (statusEffected == null || !statusEffected.HasStatus(StatusEffects.Frozen))
						{
							float num2 = Vector3.DistanceSquared(vector2, damageable2.Position);
							Vector3 position4 = damageable2.Position;
							Vector3 value = vector3;
							Vector3 vector4;
							Vector3.Subtract(ref position4, ref vector2, out vector4);
							vector4.Y = 0f;
							vector4.Normalize();
							value.Y = 0f;
							if (value != Vector3.Zero)
							{
								value.Normalize();
							}
							float num3 = 0.7853982f;
							if (num2 < num && MagickaMath.Angle(ref value, ref vector4) <= num3)
							{
								num = num2;
								damageable = damageable2;
							}
						}
					}
				}
				iCaster.PlayState.EntityManager.ReturnEntityList(entities);
				if (damageable != null)
				{
					num = (float)Math.Sqrt((double)num);
					LightningBolt lightning = LightningBolt.GetLightning();
					lightning.Cast(iCaster, iTarget.Position, damageable as Entity, iHitList, iColor, iScale, iRange - num, ref iDamages, iState);
					return;
				}
				LightningBolt lightning2 = LightningBolt.GetLightning();
				lightning2.InitializeEffect(ref vector2, vector3, iColor, true, iScale, iRange, iState);
			}
		}

		// Token: 0x06001EF9 RID: 7929 RVA: 0x000D8C04 File Offset: 0x000D6E04
		public void Cast(ISpellCaster iCaster, Vector3 iSource, Vector3 iDirection, HitList iHitList, Vector3 iColor, float iRange, ref DamageCollection5 iDamages, Spell? iSpell, PlayState iState)
		{
			this.Cast(iCaster, iSource, iDirection, iHitList, iColor, 1f, iRange, ref iDamages, iSpell, iState);
		}

		// Token: 0x06001EFA RID: 7930 RVA: 0x000D8C2C File Offset: 0x000D6E2C
		public void Cast(ISpellCaster iCaster, Vector3 iSource, Vector3 iDirection, HitList iHitList, Vector3 iColor, float iSize, float iRange, ref DamageCollection5 iDamages, Spell? iSpell, PlayState iState)
		{
			if (iRange > 0.05f)
			{
				this.mTimeStamp = iCaster.PlayState.PlayTime;
				List<Entity> entities;
				if (this.AirToSurface)
				{
					float y = iSource.Y;
					iSource.Y = 0f;
					entities = iState.EntityManager.GetEntities(iSource, iRange, true);
					iSource.Y = y;
				}
				else
				{
					entities = iState.EntityManager.GetEntities(iSource, iRange, true);
				}
				IDamageable damageable = null;
				float num = float.MaxValue;
				float num2 = float.MaxValue;
				Segment seg = default(Segment);
				seg.Origin = iSource;
				List<Shield> shields = iCaster.PlayState.EntityManager.Shields;
				for (int i = 0; i < entities.Count; i++)
				{
					IDamageable damageable2 = entities[i] as IDamageable;
					if (damageable2 != null && !damageable2.Dead && !iHitList.Contains(damageable2) && (damageable2 is ElementalEgg | damageable2 is DamageablePhysicsEntity | (damageable2 is Magicka.GameLogic.Entities.Character && !(damageable2 as Magicka.GameLogic.Entities.Character).IsEthereal) | (damageable2 is Barrier && (damageable2 as Barrier).Solid) | damageable2 is MissileEntity | damageable2 is Shield | damageable2 is BossDamageZone | damageable2 is Tentacle))
					{
						if (!(damageable2 is Shield))
						{
							bool flag = false;
							for (int j = 0; j < shields.Count; j++)
							{
								seg.Delta = damageable2.Position - seg.Origin;
								Shield shield = shields[j];
								float num3;
								Vector3 vector;
								Vector3 vector2;
								if (shield != null && shield.Body.CollisionSkin.SegmentIntersect(out num3, out vector, out vector2, seg))
								{
									flag = true;
									break;
								}
							}
							if (flag)
							{
								goto IL_273;
							}
						}
						IStatusEffected statusEffected = damageable2 as IStatusEffected;
						if (statusEffected == null || !statusEffected.HasStatus(StatusEffects.Frozen))
						{
							Vector3 value = damageable2.Position;
							Shield shield2 = damageable2 as Shield;
							if (shield2 != null)
							{
								value = shield2.GetNearestPosition(iSource);
							}
							float num4 = Vector3.DistanceSquared(iSource, value);
							Vector3 value2 = iDirection;
							Vector3 vector3;
							Vector3.Subtract(ref value, ref iSource, out vector3);
							vector3.Y = 0f;
							vector3.Normalize();
							value2.Y = 0f;
							if (value2 != Vector3.Zero)
							{
								value2.Normalize();
							}
							float num5 = MagickaMath.Angle(ref value2, ref vector3);
							if ((num5 <= 1.5707964f && num5 < num2 && num4 < num) || num4 < 1E-45f)
							{
								num2 = num5;
								num = num4;
								damageable = damageable2;
							}
						}
					}
					IL_273:;
				}
				iCaster.PlayState.EntityManager.ReturnEntityList(entities);
				if (damageable == null)
				{
					Quaternion quaternion = Quaternion.CreateFromYawPitchRoll(((float)MagickaMath.Random.NextDouble() - 0.5f) * 0.5f * 0.7853982f, 0f, 0f);
					Vector3.Transform(ref iDirection, ref quaternion, out iDirection);
					this.InitializeEffect(ref iSource, iDirection, iColor, true, iSize, iRange, iState);
					iHitList.Clear();
					return;
				}
				DamageResult damageResult = DamageResult.None;
				iHitList.Add(damageable);
				Vector3 iSource2 = damageable.Position;
				if (damageable is Magicka.GameLogic.Entities.Character && (damageable as Magicka.GameLogic.Entities.Character).IsBlocking && (damageable as Magicka.GameLogic.Entities.Character).BlockItem >= 0)
				{
					Spell value3;
					if (iSpell != null)
					{
						value3 = iSpell.Value;
					}
					else
					{
						Spell.DefaultSpell(Elements.Lightning, out value3);
					}
					(damageable as Magicka.GameLogic.Entities.Character).Equipment[0].Item.TryAddToQueue(ref value3, true);
					iSource2 = (damageable as Magicka.GameLogic.Entities.Character).Equipment[0].Item.Position;
					damageResult |= DamageResult.Deflected;
				}
				else
				{
					damageResult = damageable.Damage(iDamages, iCaster as Entity, this.mTimeStamp, iSource);
				}
				Shield shield3 = damageable as Shield;
				if (shield3 != null)
				{
					iSource2 = shield3.GetNearestPosition(iSource);
				}
				Vector3 position = iState.Scene.Camera.Position;
				Vector3 iDirection2;
				Vector3.Subtract(ref iSource2, ref iSource, out iDirection2);
				iDirection2.Normalize();
				this.InitializeEffect(ref iSource, ref iDirection2, ref iSource2, ref position, ref iColor, false, iSize, 0.25f, iState);
				if (damageResult != DamageResult.Deflected && damageable != null && !(damageable is Barrier | damageable is Shield | damageable is BossDamageZone))
				{
					LightningBolt lightning = LightningBolt.GetLightning();
					DamageCollection5 damageCollection = iDamages;
					lightning.Cast(iCaster, iSource2, iDirection2, iHitList, iColor, iSize, iRange, ref damageCollection, iSpell, iState);
				}
			}
		}

		// Token: 0x06001EFB RID: 7931 RVA: 0x000D9080 File Offset: 0x000D7280
		public void InitializeEffect(ref Vector3 iCastFrom, Vector3 iDirection, Vector3 iColor, bool iCut, float iScale, float iRange, PlayState iState)
		{
			Vector3 vector = iCastFrom;
			Vector3 vector2 = iCastFrom + iDirection * iRange;
			Vector3 position = iState.Scene.Camera.Position;
			this.InitializeEffect(ref vector, ref iDirection, ref vector2, ref position, ref iColor, iCut, iScale, 0.25f, iState);
		}

		// Token: 0x06001EFC RID: 7932 RVA: 0x000D90D8 File Offset: 0x000D72D8
		public void InitializeEffect(ref Vector3 iPosition, ref Vector3 iDirection, ref Vector3 iTarget, ref Vector3 iEyePosition, ref Vector3 iColor, bool iCut, float iScale, float iTTL, PlayState iState)
		{
			if (iCut)
			{
				Segment iSeg;
				iSeg.Origin = iPosition;
				Vector3.Subtract(ref iTarget, ref iPosition, out iSeg.Delta);
				float num;
				Vector3 vector;
				Vector3 vector2;
				if (iState.Level.CurrentScene.SegmentIntersect(out num, out vector, out vector2, iSeg))
				{
					iTarget = vector;
				}
			}
			EffectManager instance = EffectManager.Instance;
			VisualEffectReference visualEffectReference;
			instance.StartEffect(LightningBolt.SOURCEEFFECTHASH, ref iPosition, ref iDirection, out visualEffectReference);
			instance.StartEffect(LightningBolt.HITEFFECTHASH, ref iTarget, ref iDirection, out visualEffectReference);
			this.mTTL = iTTL;
			Vector3 vector3;
			Vector3.Subtract(ref iEyePosition, ref iPosition, out vector3);
			float num2;
			Vector3.Distance(ref iPosition, ref iTarget, out num2);
			num2 /= iScale;
			this.mDrawCount = Math.Min((int)Math.Floor((double)(num2 / 2f + 0.5f)) * 2, 32);
			for (int i = 0; i < 3; i++)
			{
				this.mRenderData[i].mDrawCount = this.mDrawCount;
				this.mRenderData[i].mMaterial.ColorTint = new Vector4(iColor * 2f, 1f);
			}
			Matrix matrix = Matrix.CreateScale(iScale, iScale, iScale * (num2 / (float)this.mDrawCount));
			Matrix identity = Matrix.Identity;
			Vector3 forward;
			Vector3.Subtract(ref iTarget, ref iPosition, out forward);
			Vector3.Normalize(ref forward, out forward);
			Vector3 right;
			Vector3.Cross(ref forward, ref vector3, out right);
			Vector3.Normalize(ref right, out right);
			Vector3 up;
			Vector3.Cross(ref right, ref forward, out up);
			Vector3.Normalize(ref up, out up);
			identity.Forward = forward;
			identity.Right = right;
			identity.Up = up;
			Matrix.Multiply(ref matrix, ref identity, out this.mOrientation);
			this.mOrientation.Translation = iPosition;
			this.mScene = iState.Scene;
			Vector3 diffuseColor = default(Vector3);
			diffuseColor.X = iColor.X;
			diffuseColor.Y = iColor.Y;
			diffuseColor.Z = iColor.Z;
			this.mLight.DiffuseColor = diffuseColor;
			this.mLight.End = iTarget;
			this.mLight.Start = iPosition;
			this.mLight.Radius = 3f + 2f * iScale;
			this.mLight.Enable(this.mScene);
			SpellManager.Instance.AddSpellEffect(this);
		}

		// Token: 0x06001EFD RID: 7933 RVA: 0x000D9320 File Offset: 0x000D7520
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mTTL -= iDeltaTime;
			if (this.mTTL <= 0f)
			{
				return;
			}
			if (this.mDrawCount <= 0)
			{
				return;
			}
			LightningBolt.RenderData renderData = this.mRenderData[(int)iDataChannel];
			renderData.mMaterial.ColorTint.W = (float)Math.Sqrt((double)(this.TTL * 10f));
			float intensity = (float)Math.Sqrt((double)(this.TTL * 1f)) * 1.5f;
			this.mLight.Intensity = intensity;
			renderData.mTransform = this.mOrientation;
			this.mScene.AddRenderableAdditiveObject(iDataChannel, renderData);
		}

		// Token: 0x06001EFE RID: 7934 RVA: 0x000D93BE File Offset: 0x000D75BE
		public void OnRemove()
		{
			this.mLight.Disable();
			this.mScene = null;
			LightningBolt.sCache.Add(this);
		}

		// Token: 0x17000792 RID: 1938
		// (get) Token: 0x06001EFF RID: 7935 RVA: 0x000D93DD File Offset: 0x000D75DD
		public bool IsDead
		{
			get
			{
				return this.mTTL <= 0f;
			}
		}

		// Token: 0x17000793 RID: 1939
		// (get) Token: 0x06001F00 RID: 7936 RVA: 0x000D93EF File Offset: 0x000D75EF
		// (set) Token: 0x06001F01 RID: 7937 RVA: 0x000D93F7 File Offset: 0x000D75F7
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

		// Token: 0x0400215B RID: 8539
		private static readonly int HITEFFECTHASH = "lightning_hit".GetHashCodeCustom();

		// Token: 0x0400215C RID: 8540
		private static readonly int SOURCEEFFECTHASH = "lightning_source".GetHashCodeCustom();

		// Token: 0x0400215D RID: 8541
		private static List<LightningBolt> sCache;

		// Token: 0x0400215E RID: 8542
		private static ContentManager sContent;

		// Token: 0x0400215F RID: 8543
		private Matrix mOrientation;

		// Token: 0x04002160 RID: 8544
		private VertexBuffer mVertices;

		// Token: 0x04002161 RID: 8545
		private VertexDeclaration mVertexDeclaration;

		// Token: 0x04002162 RID: 8546
		private int mDrawCount;

		// Token: 0x04002163 RID: 8547
		private float mTTL;

		// Token: 0x04002164 RID: 8548
		private Scene mScene;

		// Token: 0x04002165 RID: 8549
		private CapsuleLight mLight;

		// Token: 0x04002166 RID: 8550
		private double mTimeStamp;

		// Token: 0x04002167 RID: 8551
		private LightningBolt.RenderData[] mRenderData;

		// Token: 0x04002168 RID: 8552
		private bool mAirToSurface;

		// Token: 0x020003F6 RID: 1014
		protected class RenderData : IRenderableAdditiveObject
		{
			// Token: 0x17000794 RID: 1940
			// (get) Token: 0x06001F03 RID: 7939 RVA: 0x000D9420 File Offset: 0x000D7620
			public int Effect
			{
				get
				{
					return AdditiveEffect.TYPEHASH;
				}
			}

			// Token: 0x17000795 RID: 1941
			// (get) Token: 0x06001F04 RID: 7940 RVA: 0x000D9427 File Offset: 0x000D7627
			public int Technique
			{
				get
				{
					return 0;
				}
			}

			// Token: 0x17000796 RID: 1942
			// (get) Token: 0x06001F05 RID: 7941 RVA: 0x000D942A File Offset: 0x000D762A
			public VertexBuffer Vertices
			{
				get
				{
					return this.mVertices;
				}
			}

			// Token: 0x17000797 RID: 1943
			// (get) Token: 0x06001F06 RID: 7942 RVA: 0x000D9432 File Offset: 0x000D7632
			public IndexBuffer Indices
			{
				get
				{
					return null;
				}
			}

			// Token: 0x17000798 RID: 1944
			// (get) Token: 0x06001F07 RID: 7943 RVA: 0x000D9435 File Offset: 0x000D7635
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return this.mVertexDeclaration;
				}
			}

			// Token: 0x17000799 RID: 1945
			// (get) Token: 0x06001F08 RID: 7944 RVA: 0x000D943D File Offset: 0x000D763D
			public int VertexStride
			{
				get
				{
					return VertexPositionColorTexture.SizeInBytes;
				}
			}

			// Token: 0x1700079A RID: 1946
			// (get) Token: 0x06001F09 RID: 7945 RVA: 0x000D9444 File Offset: 0x000D7644
			public int VerticesHashCode
			{
				get
				{
					return this.mVerticesHash;
				}
			}

			// Token: 0x06001F0A RID: 7946 RVA: 0x000D944C File Offset: 0x000D764C
			public bool Cull(BoundingFrustum iViewFrustum)
			{
				return false;
			}

			// Token: 0x06001F0B RID: 7947 RVA: 0x000D9450 File Offset: 0x000D7650
			public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				int num = this.mDrawCount;
				if (num <= 0)
				{
					return;
				}
				AdditiveEffect additiveEffect = iEffect as AdditiveEffect;
				this.mMaterial.AssignToEffect(additiveEffect);
				additiveEffect.World = this.mTransform;
				additiveEffect.TextureOffset = default(Vector2);
				additiveEffect.TextureScale = new Vector2(1f);
				additiveEffect.CommitChanges();
				additiveEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleStrip, 0, num);
			}

			// Token: 0x04002169 RID: 8553
			public Matrix mTransform;

			// Token: 0x0400216A RID: 8554
			public int mDrawCount;

			// Token: 0x0400216B RID: 8555
			public VertexBuffer mVertices;

			// Token: 0x0400216C RID: 8556
			public int mVerticesHash;

			// Token: 0x0400216D RID: 8557
			public VertexDeclaration mVertexDeclaration;

			// Token: 0x0400216E RID: 8558
			public AdditiveMaterial mMaterial;

			// Token: 0x0400216F RID: 8559
			public BoundingSphere mBoundingSphere;
		}
	}
}
