using System;
using System.Collections.Generic;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Magicka.AI;
using Magicka.GameLogic.GameStates;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

namespace Magicka.GameLogic.Entities
{
	// Token: 0x020000E0 RID: 224
	internal class Dispenser : Entity, IDamageable
	{
		// Token: 0x060006D0 RID: 1744 RVA: 0x00028410 File Offset: 0x00026610
		public static void InitializeCache(int iNrOfShields, PlayState iPlayState)
		{
			Dispenser.mCache = new List<Dispenser>(iNrOfShields);
			for (int i = 0; i < iNrOfShields; i++)
			{
				Dispenser.mCache.Add(new Dispenser(iPlayState));
			}
		}

		// Token: 0x060006D1 RID: 1745 RVA: 0x00028444 File Offset: 0x00026644
		public static Dispenser GetFromCache(PlayState iPlayState)
		{
			if (Dispenser.mCache.Count > 0)
			{
				Dispenser result = Dispenser.mCache[Dispenser.mCache.Count - 1];
				Dispenser.mCache.RemoveAt(Dispenser.mCache.Count - 1);
				return result;
			}
			return new Dispenser(iPlayState);
		}

		// Token: 0x060006D2 RID: 1746 RVA: 0x00028494 File Offset: 0x00026694
		public Dispenser(PlayState iPlayState) : base(iPlayState)
		{
			if (Dispenser.sDispenserModels == null)
			{
				int num = 3;
				Dispenser.sAnimationClips = new AnimationClip[num][];
				Dispenser.sDispenserModels = new SkinnedModel[num];
				lock (Game.Instance.GraphicsDevice)
				{
					for (int i = 0; i < num; i++)
					{
						Dispenser.sDispenserModels[i] = Game.Instance.Content.Load<SkinnedModel>("Models/Effects/" + ((Dispensers)i).ToString());
						Dispenser.sAnimationClips[i] = new AnimationClip[4];
						Dispenser.sDispenserModels[i].AnimationClips.TryGetValue("destroyed", out Dispenser.sAnimationClips[i][3]);
						Dispenser.sDispenserModels[i].AnimationClips.TryGetValue("spawn", out Dispenser.sAnimationClips[i][0]);
						Dispenser.sDispenserModels[i].AnimationClips.TryGetValue("idle", out Dispenser.sAnimationClips[i][2]);
						Dispenser.sDispenserModels[i].AnimationClips.TryGetValue("execute", out Dispenser.sAnimationClips[i][1]);
					}
				}
			}
			this.mAnimationController = new AnimationController();
			this.mAnimationController.ClipSpeed = 1f;
			this.mBody = new Body();
			this.mCollision = new CollisionSkin(this.mBody);
			this.mCollision.AddPrimitive(new Capsule(Vector3.Down, Matrix.CreateRotationX(1.5707964f), 1.25f, 1f), 1, new MaterialProperties(0f, 0.8f, 0.8f));
			this.mCollision.callbackFn += this.OnCollision;
			this.mBody.CollisionSkin = this.mCollision;
			this.mBody.Immovable = true;
			this.mBody.Tag = this;
			this.mRenderData = new Dispenser.RenderData[3];
			for (int j = 0; j < 3; j++)
			{
				Dispenser.RenderData renderData = new Dispenser.RenderData();
				this.mRenderData[j] = renderData;
			}
			this.mTypeID = new List<int>(32);
			this.mTypeAmount = new List<int>(32);
			this.mResistances = new Resistance[11];
			for (int k = 0; k < this.mResistances.Length; k++)
			{
				this.mResistances[k].ResistanceAgainst = Defines.ElementFromIndex(k);
				this.mResistances[k].Multiplier = 1f;
				this.mResistances[k].Modifier = 0f;
			}
			this.mType = Dispensers.NrOfModels;
		}

		// Token: 0x060006D3 RID: 1747 RVA: 0x0002874C File Offset: 0x0002694C
		protected bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
		{
			return false;
		}

		// Token: 0x060006D4 RID: 1748 RVA: 0x00028750 File Offset: 0x00026950
		public override Matrix GetOrientation()
		{
			Vector3 position = this.mBody.Position;
			position.Y -= 1.75f;
			Matrix orientation = this.mBody.Orientation;
			orientation.Translation = position;
			return orientation;
		}

		// Token: 0x060006D5 RID: 1749 RVA: 0x00028794 File Offset: 0x00026994
		public void Initialize(Matrix iTransform, Dispensers iModel, int[] iTypeID, int[] iAmount, float iTimeBetween, bool iActive)
		{
			this.mActive = iActive;
			this.mType = iModel;
			for (int i = 0; i < iTypeID.Length; i++)
			{
				this.mTypeID.Add(iTypeID[i]);
			}
			for (int j = 0; j < iAmount.Length; j++)
			{
				this.mTypeAmount.Add(iAmount[j]);
			}
			if (this.mTypeID.Count < this.mTypeAmount.Count)
			{
				this.mTypeAmount.RemoveRange(this.mTypeID.Count, this.mTypeAmount.Count - this.mTypeID.Count);
			}
			this.mTimeBetween = iTimeBetween;
			this.mTimer = 0f;
			this.mModel = Dispenser.sDispenserModels[(int)iModel];
			this.mRadius = 1.25f;
			(this.mCollision.GetPrimitiveLocal(0) as Capsule).Radius = this.mRadius;
			(this.mCollision.GetPrimitiveNewWorld(0) as Capsule).Radius = this.mRadius;
			(this.mCollision.GetPrimitiveOldWorld(0) as Capsule).Radius = this.mRadius;
			this.mVolume = (this.mCollision.GetPrimitiveLocal(0) as Capsule).GetVolume();
			Segment iSeg = new Segment(iTransform.Translation + Vector3.Up, Vector3.Down * 4f);
			float num;
			Vector3 translation;
			Vector3 vector;
			if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out num, out translation, out vector, iSeg))
			{
				iTransform.Translation = translation;
			}
			iTransform.Translation += new Vector3(0f, 1.75f, 0f);
			Matrix orientation = iTransform;
			orientation.Translation = default(Vector3);
			this.mBody.MoveTo(iTransform.Translation, orientation);
			this.mHitPoints = 500f;
			this.mMaxHitPoints = this.mHitPoints;
			this.mPlayState.EntityManager.AddEntity(this);
			this.mAnimationController.Skeleton = this.mModel.SkeletonBones;
			for (int k = 0; k < 3; k++)
			{
				Dispenser.RenderData renderData = this.mRenderData[k];
				renderData.mBoundingSphere.Center = this.Position;
				renderData.mBoundingSphere.Radius = this.mRadius * 1.333f;
				renderData.SetMesh(this.mModel.Model.Meshes[0].VertexBuffer, this.mModel.Model.Meshes[0].IndexBuffer, this.mModel.Model.Meshes[0].MeshParts[0], SkinnedModelBasicEffect.TYPEHASH);
			}
			if (Dispenser.sAnimationClips[(int)this.mType][0] != null)
			{
				this.mAnimationController.StartClip(Dispenser.sAnimationClips[(int)this.mType][0], false);
			}
			base.Initialize();
		}

		// Token: 0x060006D6 RID: 1750 RVA: 0x00028A83 File Offset: 0x00026C83
		public override void Deinitialize()
		{
			this.mAnimationController.StartClip(Dispenser.sAnimationClips[(int)this.mType][3], false);
			this.mDead = true;
			this.mHitPoints = 0f;
			base.Deinitialize();
			Dispenser.mCache.Add(this);
		}

		// Token: 0x060006D7 RID: 1751 RVA: 0x00028AC2 File Offset: 0x00026CC2
		public float ResistanceAgainst(Elements iElement)
		{
			return 0f;
		}

		// Token: 0x060006D8 RID: 1752 RVA: 0x00028ACC File Offset: 0x00026CCC
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			Matrix orientation = this.GetOrientation();
			this.mAnimationController.Update(iDeltaTime, ref orientation, true);
			base.Update(iDataChannel, iDeltaTime);
			if (this.mAnimationController.HasFinished && this.mActive)
			{
				this.mTimer -= iDeltaTime;
				if (this.mTimer <= 0f)
				{
					this.mTimer = this.mTimeBetween;
					if (!this.Spawn())
					{
						this.mHitPoints = 0f;
						if (Dispenser.sAnimationClips[(int)this.mType][3] != null)
						{
							this.mAnimationController.StartClip(Dispenser.sAnimationClips[(int)this.mType][3], false);
						}
					}
				}
			}
			Dispenser.RenderData renderData = this.mRenderData[(int)iDataChannel];
			this.mAnimationController.SkinnedBoneTransforms.CopyTo(renderData.mBones, 0);
			this.mPlayState.Scene.AddRenderableObject(iDataChannel, renderData);
		}

		// Token: 0x060006D9 RID: 1753 RVA: 0x00028BA4 File Offset: 0x00026DA4
		private bool Spawn()
		{
			if (this.mTypeID.Count == 0)
			{
				return false;
			}
			if (this.mTypeAmount.Count != 1)
			{
				for (int i = 0; i < this.mTypeAmount.Count; i++)
				{
					if (this.mTypeAmount[i] == 0)
					{
						this.mTypeAmount.RemoveAt(i);
						this.mTypeID.RemoveAt(i);
						i--;
					}
				}
			}
			int num = MagickaMath.Random.Next(0, this.mTypeID.Count);
			if ((this.mTypeAmount.Count >= num && this.mTypeAmount[num] > 0) || (this.mTypeAmount.Count == 1 && this.mTypeAmount[0] > 0))
			{
				if (Dispenser.sAnimationClips[(int)this.mType][1] != null)
				{
					this.mAnimationController.StartClip(Dispenser.sAnimationClips[(int)this.mType][1], false);
				}
				NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(this.mPlayState);
				CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(this.mTypeID[num]);
				instance.Initialize(cachedTemplate, this.Position, 0);
				Agent ai = instance.AI;
				ai.SetOrder(Order.Attack, ReactTo.None, Order.None, 0, -1, 0, null);
				instance.GoToAnimation(Animations.spawn, 0.001f);
				instance.Body.Orientation = this.mBody.Orientation;
				instance.CharacterBody.DesiredDirection = this.mBody.Orientation.Forward;
				this.mPlayState.EntityManager.AddEntity(instance);
				if (this.mPlayState.Level.CurrentScene.RuleSet is SurvivalRuleset)
				{
					(this.mPlayState.Level.CurrentScene.RuleSet as SurvivalRuleset).AddedCharacter(instance, false);
				}
				if (this.mTypeAmount[num] != -1)
				{
					List<int> list;
					int index;
					(list = this.mTypeAmount)[index = num] = list[index] - 1;
				}
				else
				{
					List<int> list2;
					(list2 = this.mTypeAmount)[0] = list2[0] - 1;
				}
				return true;
			}
			return false;
		}

		// Token: 0x060006DA RID: 1754 RVA: 0x00028DB0 File Offset: 0x00026FB0
		public override Vector3 CalcImpulseVelocity(Vector3 iDirection, float iElevation, float iMassPower, float iDistance)
		{
			return default(Vector3);
		}

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x060006DB RID: 1755 RVA: 0x00028DC6 File Offset: 0x00026FC6
		public override bool Dead
		{
			get
			{
				return this.mHitPoints <= 0f;
			}
		}

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x060006DC RID: 1756 RVA: 0x00028DD8 File Offset: 0x00026FD8
		public override bool Removable
		{
			get
			{
				return this.Dead && this.mAnimationController.HasFinished;
			}
		}

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x060006DD RID: 1757 RVA: 0x00028DEF File Offset: 0x00026FEF
		public override Vector3 Position
		{
			get
			{
				return this.mBody.Position;
			}
		}

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x060006DE RID: 1758 RVA: 0x00028DFC File Offset: 0x00026FFC
		public virtual float Volume
		{
			get
			{
				return this.mVolume;
			}
		}

		// Token: 0x060006DF RID: 1759 RVA: 0x00028E04 File Offset: 0x00027004
		public void SetSlow()
		{
		}

		// Token: 0x060006E0 RID: 1760 RVA: 0x00028E06 File Offset: 0x00027006
		public void ActiveToggle()
		{
			this.mActive = !this.mActive;
		}

		// Token: 0x060006E1 RID: 1761 RVA: 0x00028E17 File Offset: 0x00027017
		internal void Activate()
		{
			this.mActive = true;
		}

		// Token: 0x060006E2 RID: 1762 RVA: 0x00028E20 File Offset: 0x00027020
		public void Damage(float iDamage)
		{
			this.mHitPoints -= iDamage;
		}

		// Token: 0x060006E3 RID: 1763 RVA: 0x00028E30 File Offset: 0x00027030
		public virtual DamageResult InternalDamage(DamageCollection5 iDamages, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			DamageResult damageResult = DamageResult.None;
			damageResult |= this.InternalDamage(iDamages.A, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			damageResult |= this.InternalDamage(iDamages.B, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			damageResult |= this.InternalDamage(iDamages.C, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			damageResult |= this.InternalDamage(iDamages.D, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			return damageResult | this.InternalDamage(iDamages.E, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
		}

		// Token: 0x060006E4 RID: 1764 RVA: 0x00028EB0 File Offset: 0x000270B0
		public DamageResult InternalDamage(Damage iDamage, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			DamageResult damageResult = DamageResult.None;
			float num = 0f;
			float num2 = 0f;
			for (int i = 0; i < this.mResistances.Length; i++)
			{
				Elements elements = Defines.ElementFromIndex(i);
				if ((iDamage.Element & elements) == elements)
				{
					if (iDamage.Element == Elements.Earth && this.mResistances[i].Modifier != 0f)
					{
						iDamage.Amount = Math.Max(iDamage.Amount + (float)((int)this.mResistances[i].Modifier), 0f);
					}
					else
					{
						iDamage.Amount += (float)((int)this.mResistances[i].Modifier);
					}
					num += this.mResistances[i].Multiplier;
					num2 += 1f;
				}
			}
			if (num2 != 0f)
			{
				iDamage.Magnitude *= num / num2;
			}
			iDamage.Amount = (float)((int)(iDamage.Amount * iDamage.Magnitude));
			if (iDamage.Amount <= 0f)
			{
				damageResult |= DamageResult.Deflected;
			}
			else
			{
				damageResult |= DamageResult.Damaged;
			}
			if (Defines.FeatureDamage(iFeatures))
			{
				this.mHitPoints -= iDamage.Amount;
			}
			return damageResult;
		}

		// Token: 0x060006E5 RID: 1765 RVA: 0x00028FF5 File Offset: 0x000271F5
		public void Electrocute(IDamageable iTarget, float iMultiplyer)
		{
		}

		// Token: 0x060006E6 RID: 1766 RVA: 0x00028FF7 File Offset: 0x000271F7
		public override void Kill()
		{
			this.mHitPoints = 0f;
		}

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x060006E7 RID: 1767 RVA: 0x00029004 File Offset: 0x00027204
		public Capsule Capsule
		{
			get
			{
				return this.mCollision.GetPrimitiveNewWorld(0) as Capsule;
			}
		}

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x060006E8 RID: 1768 RVA: 0x00029017 File Offset: 0x00027217
		public float HitPoints
		{
			get
			{
				return this.mHitPoints;
			}
		}

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x060006E9 RID: 1769 RVA: 0x0002901F File Offset: 0x0002721F
		public float MaxHitPoints
		{
			get
			{
				return this.mMaxHitPoints;
			}
		}

		// Token: 0x060006EA RID: 1770 RVA: 0x00029028 File Offset: 0x00027228
		public bool SegmentIntersect(out Vector3 oPosition, Segment iSeg, float iSegmentRadius)
		{
			Segment seg = default(Segment);
			seg.Origin = this.Capsule.Position;
			seg.Delta = this.Capsule.Orientation.Forward;
			Vector3.Multiply(ref seg.Delta, this.Capsule.Length, out seg.Delta);
			float scaleFactor;
			float scaleFactor2;
			float num = Distance.SegmentSegmentDistanceSq(out scaleFactor, out scaleFactor2, seg, iSeg);
			float num2 = iSegmentRadius + this.Capsule.Radius;
			num2 *= num2;
			if (num > num2)
			{
				oPosition = default(Vector3);
				return false;
			}
			Vector3 vector;
			Vector3.Multiply(ref seg.Delta, scaleFactor, out vector);
			Vector3.Add(ref seg.Origin, ref vector, out vector);
			Vector3 vector2;
			Vector3.Multiply(ref iSeg.Delta, scaleFactor2, out vector2);
			Vector3.Add(ref iSeg.Origin, ref vector2, out vector2);
			vector2.Normalize();
			Vector3.Multiply(ref vector2, this.Capsule.Radius, out vector2);
			Vector3.Subtract(ref vector2, ref vector, out oPosition);
			Vector3.Add(ref seg.Origin, ref vector, out oPosition);
			return true;
		}

		// Token: 0x060006EB RID: 1771 RVA: 0x0002912C File Offset: 0x0002732C
		public bool ArcIntersect(out Vector3 oPosition, ref Vector3 iOrigin, ref Vector3 iDirection, float iRange, float iAngle, float iHeightDifference)
		{
			iOrigin.Y = 0f;
			iDirection.Y = 0f;
			Vector3 position = this.Position;
			position.Y = 0f;
			Vector3 vector;
			Vector3.Subtract(ref iOrigin, ref position, out vector);
			float num = vector.Length();
			float radius = this.Capsule.Radius;
			if (num - radius > iRange)
			{
				oPosition = default(Vector3);
				return false;
			}
			Vector3.Divide(ref vector, num, out vector);
			float num2;
			Vector3.Dot(ref vector, ref iDirection, out num2);
			num2 = -num2;
			float num3 = (float)Math.Acos((double)num2);
			float num4 = -2f * num * num;
			float num5 = (float)Math.Acos((double)((radius * radius + num4) / num4));
			if (num3 - num5 < iAngle)
			{
				Vector3.Multiply(ref vector, radius, out vector);
				position = this.Position;
				Vector3.Add(ref position, ref vector, out oPosition);
				return true;
			}
			oPosition = default(Vector3);
			return false;
		}

		// Token: 0x060006EC RID: 1772 RVA: 0x000291FF File Offset: 0x000273FF
		public void OverKill()
		{
			this.mHitPoints = 0f;
		}

		// Token: 0x060006ED RID: 1773 RVA: 0x0002920C File Offset: 0x0002740C
		protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
		{
			oMsg = default(EntityUpdateMessage);
		}

		// Token: 0x04000596 RID: 1430
		public const float RADIUS = 1.25f;

		// Token: 0x04000597 RID: 1431
		public const float LENGTH = 1f;

		// Token: 0x04000598 RID: 1432
		private static List<Dispenser> mCache;

		// Token: 0x04000599 RID: 1433
		private static SkinnedModel[] sDispenserModels;

		// Token: 0x0400059A RID: 1434
		private static AnimationClip[][] sAnimationClips;

		// Token: 0x0400059B RID: 1435
		private Dispenser.RenderData[] mRenderData;

		// Token: 0x0400059C RID: 1436
		private SkinnedModel mModel;

		// Token: 0x0400059D RID: 1437
		private AnimationController mAnimationController;

		// Token: 0x0400059E RID: 1438
		public Dispensers mType;

		// Token: 0x0400059F RID: 1439
		private float mHitPoints;

		// Token: 0x040005A0 RID: 1440
		private float mMaxHitPoints;

		// Token: 0x040005A1 RID: 1441
		private float mTimeBetween = 2f;

		// Token: 0x040005A2 RID: 1442
		private float mTimer;

		// Token: 0x040005A3 RID: 1443
		private List<int> mTypeID;

		// Token: 0x040005A4 RID: 1444
		private List<int> mTypeAmount;

		// Token: 0x040005A5 RID: 1445
		private bool mActive = true;

		// Token: 0x040005A6 RID: 1446
		protected float mVolume;

		// Token: 0x040005A7 RID: 1447
		protected Resistance[] mResistances;

		// Token: 0x020000E1 RID: 225
		protected class RenderData : IRenderableObject
		{
			// Token: 0x060006EE RID: 1774 RVA: 0x00029215 File Offset: 0x00027415
			public RenderData()
			{
				this.mBones = new Matrix[80];
			}

			// Token: 0x17000169 RID: 361
			// (get) Token: 0x060006EF RID: 1775 RVA: 0x0002922A File Offset: 0x0002742A
			public int Effect
			{
				get
				{
					return this.mEffect;
				}
			}

			// Token: 0x1700016A RID: 362
			// (get) Token: 0x060006F0 RID: 1776 RVA: 0x00029232 File Offset: 0x00027432
			public int DepthTechnique
			{
				get
				{
					return 3;
				}
			}

			// Token: 0x1700016B RID: 363
			// (get) Token: 0x060006F1 RID: 1777 RVA: 0x00029235 File Offset: 0x00027435
			public int Technique
			{
				get
				{
					return 0;
				}
			}

			// Token: 0x1700016C RID: 364
			// (get) Token: 0x060006F2 RID: 1778 RVA: 0x00029238 File Offset: 0x00027438
			public int ShadowTechnique
			{
				get
				{
					return 4;
				}
			}

			// Token: 0x1700016D RID: 365
			// (get) Token: 0x060006F3 RID: 1779 RVA: 0x0002923B File Offset: 0x0002743B
			public VertexBuffer Vertices
			{
				get
				{
					return this.mVertexBuffer;
				}
			}

			// Token: 0x1700016E RID: 366
			// (get) Token: 0x060006F4 RID: 1780 RVA: 0x00029243 File Offset: 0x00027443
			public IndexBuffer Indices
			{
				get
				{
					return this.mIndexBuffer;
				}
			}

			// Token: 0x1700016F RID: 367
			// (get) Token: 0x060006F5 RID: 1781 RVA: 0x0002924B File Offset: 0x0002744B
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return this.mVertexDeclaration;
				}
			}

			// Token: 0x17000170 RID: 368
			// (get) Token: 0x060006F6 RID: 1782 RVA: 0x00029253 File Offset: 0x00027453
			public int VertexStride
			{
				get
				{
					return this.mVertexStride;
				}
			}

			// Token: 0x17000171 RID: 369
			// (get) Token: 0x060006F7 RID: 1783 RVA: 0x0002925B File Offset: 0x0002745B
			public int VerticesHashCode
			{
				get
				{
					return this.mVerticesHash;
				}
			}

			// Token: 0x060006F8 RID: 1784 RVA: 0x00029264 File Offset: 0x00027464
			public bool Cull(BoundingFrustum iViewFrustum)
			{
				BoundingSphere boundingSphere = this.mBoundingSphere;
				return boundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
			}

			// Token: 0x060006F9 RID: 1785 RVA: 0x00029284 File Offset: 0x00027484
			public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelBasicEffect skinnedModelBasicEffect = iEffect as SkinnedModelBasicEffect;
				this.mSkinnedModelMaterial.AssignToEffect(skinnedModelBasicEffect);
				skinnedModelBasicEffect.Bones = this.mBones;
				skinnedModelBasicEffect.CommitChanges();
				skinnedModelBasicEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
			}

			// Token: 0x060006FA RID: 1786 RVA: 0x000292DC File Offset: 0x000274DC
			public void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelBasicEffect skinnedModelBasicEffect = iEffect as SkinnedModelBasicEffect;
				this.mSkinnedModelMaterial.AssignOpacityToEffect(skinnedModelBasicEffect);
				skinnedModelBasicEffect.Bones = this.mBones;
				skinnedModelBasicEffect.CommitChanges();
				skinnedModelBasicEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
			}

			// Token: 0x060006FB RID: 1787 RVA: 0x00029334 File Offset: 0x00027534
			public void SetMesh(VertexBuffer iVertices, IndexBuffer iIndices, ModelMeshPart iMeshPart, int iEffectHash)
			{
				this.mVertexBuffer = iVertices;
				this.mVertexStride = iVertices.GetHashCode();
				this.mIndexBuffer = iIndices;
				this.mEffect = iEffectHash;
				this.mVertexDeclaration = iMeshPart.VertexDeclaration;
				this.mBaseVertex = iMeshPart.BaseVertex;
				this.mNumVertices = iMeshPart.NumVertices;
				this.mPrimitiveCount = iMeshPart.PrimitiveCount;
				this.mStartIndex = iMeshPart.StartIndex;
				this.mStreamOffset = iMeshPart.StreamOffset;
				this.mVertexStride = iMeshPart.VertexStride;
				SkinnedModelMaterial.CreateFromEffect(iMeshPart.Effect as SkinnedModelBasicEffect, out this.mSkinnedModelMaterial);
			}

			// Token: 0x040005A8 RID: 1448
			protected int mEffect;

			// Token: 0x040005A9 RID: 1449
			protected VertexDeclaration mVertexDeclaration;

			// Token: 0x040005AA RID: 1450
			protected int mBaseVertex;

			// Token: 0x040005AB RID: 1451
			protected int mNumVertices;

			// Token: 0x040005AC RID: 1452
			protected int mPrimitiveCount;

			// Token: 0x040005AD RID: 1453
			protected int mStartIndex;

			// Token: 0x040005AE RID: 1454
			protected int mStreamOffset;

			// Token: 0x040005AF RID: 1455
			protected int mVertexStride;

			// Token: 0x040005B0 RID: 1456
			protected VertexBuffer mVertexBuffer;

			// Token: 0x040005B1 RID: 1457
			protected IndexBuffer mIndexBuffer;

			// Token: 0x040005B2 RID: 1458
			public Matrix[] mBones;

			// Token: 0x040005B3 RID: 1459
			public BoundingSphere mBoundingSphere;

			// Token: 0x040005B4 RID: 1460
			private SkinnedModelMaterial mSkinnedModelMaterial;

			// Token: 0x040005B5 RID: 1461
			public int mVerticesHash;
		}
	}
}
