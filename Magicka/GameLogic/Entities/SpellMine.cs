using System;
using System.Collections.Generic;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

namespace Magicka.GameLogic.Entities
{
	// Token: 0x020000DC RID: 220
	public class SpellMine : Entity, IDamageable
	{
		// Token: 0x060006A8 RID: 1704 RVA: 0x000272D8 File Offset: 0x000254D8
		public static SpellMine GetInstance()
		{
			SpellMine spellMine = SpellMine.sCache.Dequeue();
			SpellMine.sCache.Enqueue(spellMine);
			return spellMine;
		}

		// Token: 0x060006A9 RID: 1705 RVA: 0x000272FC File Offset: 0x000254FC
		public static void InitializeCache(int iNr, PlayState iPlayState)
		{
			SpellMine.sCache = new Queue<SpellMine>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				SpellMine.sCache.Enqueue(new SpellMine(iPlayState));
			}
		}

		// Token: 0x060006AA RID: 1706 RVA: 0x00027330 File Offset: 0x00025530
		public SpellMine(PlayState iPlayState) : base(iPlayState)
		{
			this.mBody = new Body();
			this.mBody.AllowFreezing = false;
			this.mBody.ApplyGravity = false;
			this.mBody.Tag = this;
			this.mCollision = new CollisionSkin(this.mBody);
			this.mBody.CollisionSkin = this.mCollision;
			lock (Game.Instance.GraphicsDevice)
			{
				this.mModel = iPlayState.Content.Load<SkinnedModel>("Models/Effects/mine");
			}
			VertexElement[] vertexElements;
			lock (Game.Instance.GraphicsDevice)
			{
				vertexElements = this.mModel.Model.Meshes[0].MeshParts[0].VertexDeclaration.GetVertexElements();
			}
			int num = -1;
			for (int i = 0; i < vertexElements.Length; i++)
			{
				if (vertexElements[i].VertexElementUsage == VertexElementUsage.Position)
				{
					num = (int)vertexElements[i].Offset;
					break;
				}
			}
			if (num < 0)
			{
				throw new Exception("No positions found");
			}
			Vector3[] array = new Vector3[this.mModel.Model.Meshes[0].MeshParts[0].NumVertices];
			this.mModel.Model.Meshes[0].VertexBuffer.GetData<Vector3>(num, array, this.mModel.Model.Meshes[0].MeshParts[0].StartIndex, array.Length, this.mModel.Model.Meshes[0].MeshParts[0].VertexStride);
			BoundingBox boundingBox = BoundingBox.CreateFromPoints(array);
			Vector3.Subtract(ref boundingBox.Max, ref boundingBox.Min, out this.mModelSize);
			this.mController = new AnimationController();
			this.mController.Skeleton = this.mModel.SkeletonBones;
			this.mClip = this.mModel.AnimationClips["Take 001"];
			Helper.SkinnedModelDeferredMaterialFromBasicEffect(this.mModel.Model.Meshes[0].MeshParts[0].Effect as SkinnedModelBasicEffect, out this.mMaterial);
			this.mRenderData = new SpellMine.RenderData[3];
			for (int j = 0; j < 3; j++)
			{
				this.mRenderData[j] = new SpellMine.RenderData();
				this.mRenderData[j].SetMesh(this.mModel.Model.Meshes[0].VertexBuffer, this.mModel.Model.Meshes[0].IndexBuffer, this.mModel.Model.Meshes[0].MeshParts[0], ref this.mMaterial, (SkinnedModelDeferredEffect.Technique)(this.mModel.Model.Meshes[0].MeshParts[0].Effect as SkinnedModelBasicEffect).ActiveTechnique);
			}
			this.mCollision.AddPrimitive(new Sphere(Vector3.Zero, 1f), 1, new MaterialProperties(0f, 0.8f, 0.8f));
			this.mCollision.callbackFn += this.OnCollision;
		}

		// Token: 0x060006AB RID: 1707 RVA: 0x000276B4 File Offset: 0x000258B4
		public void Initialize(ISpellCaster iOwner, Vector3 iPosition, Vector3 iDirection, float iScale, float iRange, Vector3 iNextDirection, Quaternion iNextRotation, float iDistanceBetweenMines, ref Spell iSpell, ref DamageCollection5 iDamage, AnimatedLevelPart iAnimation)
		{
			if (iAnimation != null)
			{
				iAnimation.AddEntity(this);
			}
			iPosition.Y += 0.1f;
			this.Deinitialize();
			this.mTimeStamp = iOwner.PlayState.PlayTime;
			this.mScale = iScale;
			this.mRadius = 1f;
			this.mSpell = iSpell;
			this.mDirection = iDirection;
			this.mNextMineTTL = 0.1f;
			Vector3.Transform(ref iNextDirection, ref iNextRotation, out this.mNextMineDir);
			this.mNextMineRotation = iNextRotation;
			this.mNextMineRange = iRange - iDistanceBetweenMines;
			this.mDistanceBetweenMines = iDistanceBetweenMines;
			this.mActivationEffect.Hash = 0;
			this.mDetonationTimer = 0f;
			this.mOwner = iOwner;
			this.mDead = false;
			this.mOwnerCollision = false;
			this.mModelMatrix = Matrix.CreateScale(2.1f);
			Matrix matrix = Matrix.CreateRotationY((float)SpellMine.RANDOM.NextDouble() * 6.2831855f);
			Matrix.Multiply(ref this.mModelMatrix, ref matrix, out this.mModelMatrix);
			this.mBody.MoveTo(iPosition, Matrix.Identity);
			this.mSpell = iSpell;
			iSpell.CalculateDamage(SpellType.Shield, CastType.Force, out this.mDamage);
			this.mCollision.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
			(this.mCollision.GetPrimitiveLocal(0) as Sphere).Radius = 0.9f;
			(this.mCollision.GetPrimitiveNewWorld(0) as Sphere).Radius = 0.9f;
			(this.mCollision.GetPrimitiveOldWorld(0) as Sphere).Radius = 0.9f;
			base.Initialize();
			this.mActivated = false;
			this.mMaterial.TintColor = iSpell.GetColor();
			for (int i = 0; i < 3; i++)
			{
				this.mRenderData[i].mMaterial.TintColor = iSpell.GetColor();
				this.mRenderData[i].mBoundingSphere.Center = iPosition;
				this.mRenderData[i].mBoundingSphere.Radius = 1f;
			}
			AudioManager.Instance.PlayCue(Banks.Spells, SpellMine.SOUND_CAST, base.AudioEmitter);
			this.mController.Speed = 3f;
			this.mController.PlaybackMode = PlaybackMode.Forward;
			this.mController.StartClip(this.mClip, false);
			List<Entity> entities = this.mPlayState.EntityManager.GetEntities(iPosition, 1f, false);
			for (int j = 0; j < entities.Count; j++)
			{
				Barrier barrier = entities[j] as Barrier;
				SpellMine spellMine = entities[j] as SpellMine;
				if (barrier != null)
				{
					barrier.Kill();
				}
				else if (spellMine != null)
				{
					spellMine.Detonate();
				}
			}
			this.mPlayState.EntityManager.ReturnEntityList(entities);
		}

		// Token: 0x17000153 RID: 339
		// (get) Token: 0x060006AC RID: 1708 RVA: 0x0002797E File Offset: 0x00025B7E
		public bool Resting
		{
			get
			{
				return this.mRestingTimer < 0f;
			}
		}

		// Token: 0x060006AD RID: 1709 RVA: 0x00027990 File Offset: 0x00025B90
		public void Detonate()
		{
			Vector3 position = this.Position;
			Vector3 forward = this.mBody.Orientation.Forward;
			Blast.FullBlast(this.mPlayState, this.mOwner as Entity, this.mTimeStamp, this, 3f, position, this.mDamage);
			VisualEffectReference visualEffectReference;
			EffectManager.Instance.StartEffect(SpellMine.EXPLOSION_EFFECT, ref position, ref forward, out visualEffectReference);
			Elements elements = this.mSpell.Element & ~Elements.Shield;
			for (int i = 0; i < 11; i++)
			{
				Elements elements2 = Defines.ElementFromIndex(i);
				if ((elements2 & elements) == elements2)
				{
					AudioManager.Instance.PlayCue(Banks.Spells, Defines.SOUNDS_AREA[i], base.AudioEmitter);
				}
			}
			this.mDead = true;
		}

		// Token: 0x060006AE RID: 1710 RVA: 0x00027A50 File Offset: 0x00025C50
		protected bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
		{
			if (iSkin1.Owner != null & !this.mDead & this.mDetonationTimer <= 0f & this.mController.HasFinished)
			{
				IDamageable damageable = iSkin1.Owner.Tag as IDamageable;
				if (damageable != null && !damageable.Dead && (damageable is Character || damageable is BossDamageZone || damageable is MissileEntity))
				{
					if (damageable is Character && ((damageable as Character).IsEthereal || (damageable as Character).IsLevitating))
					{
						return false;
					}
					if (damageable == this.mOwner && !this.mActivated)
					{
						this.mOwnerCollision = true;
					}
					else if (this.mController.HasFinished)
					{
						this.mDetonationTimer = 0.5f;
						if (this.mActivationEffect.Hash == 0)
						{
							Vector3 position = this.Position;
							Vector3 forward = Vector3.Forward;
							EffectManager.Instance.StartEffect(SpellMine.ACTIVATION_EFFECT, ref position, ref forward, out this.mActivationEffect);
							AudioManager.Instance.PlayCue(Banks.Spells, SpellMine.SOUND_ACTIVATE, base.AudioEmitter);
						}
					}
				}
			}
			return false;
		}

		// Token: 0x060006AF RID: 1711 RVA: 0x00027B75 File Offset: 0x00025D75
		public float ResistanceAgainst(Elements iElement)
		{
			return 0f;
		}

		// Token: 0x060006B0 RID: 1712 RVA: 0x00027B7C File Offset: 0x00025D7C
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			base.Update(iDataChannel, iDeltaTime);
			if (this.mNextMineRange > 1E-45f)
			{
				this.mNextMineTTL -= iDeltaTime;
				if (this.mNextMineTTL < 0f)
				{
					this.SpawnNextMine();
					this.mNextMineRange = 0f;
				}
			}
			Matrix orientation = this.mBody.Transform.Orientation;
			orientation.Translation = this.mBody.Transform.Position;
			if (!this.mOwnerCollision)
			{
				this.mActivated = true;
				if (this.mDetonationTimer > 0f)
				{
					this.mDetonationTimer -= iDeltaTime;
					if (this.mDetonationTimer <= 0f)
					{
						this.Detonate();
					}
				}
			}
			if (this.mOwner != null)
			{
				Vector3 position = this.mOwner.Position;
				Vector3 position2 = this.Position;
				float num;
				Vector3.DistanceSquared(ref position, ref position2, out num);
				if (num > 900f)
				{
					this.Detonate();
				}
			}
			if (this.mBody.Velocity.LengthSquared() > 1E-06f)
			{
				this.mRestingTimer = 1f;
			}
			else
			{
				this.mRestingTimer -= iDeltaTime;
			}
			SpellMine.RenderData renderData = this.mRenderData[(int)iDataChannel];
			Matrix.Multiply(ref this.mModelMatrix, ref orientation, out orientation);
			orientation.Translation -= new Vector3(0f, 0.1f, 0f);
			renderData.mBoundingSphere.Center = orientation.Translation;
			this.mController.Update(iDeltaTime, ref orientation, true);
			Array.Copy(this.mController.SkinnedBoneTransforms, 0, renderData.mBones, 0, 80);
			this.mPlayState.Scene.AddRenderableObject(iDataChannel, renderData);
			this.mOwnerCollision = false;
		}

		// Token: 0x060006B1 RID: 1713 RVA: 0x00027D30 File Offset: 0x00025F30
		protected void SpawnNextMine()
		{
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				return;
			}
			Vector3 origin = this.Position + this.mNextMineDir;
			Segment iSeg = default(Segment);
			iSeg.Delta.Y = -1.5f;
			iSeg.Origin = origin;
			iSeg.Origin.Y = iSeg.Origin.Y + 0.75f;
			List<Shield> shields = this.mPlayState.EntityManager.Shields;
			Segment iSeg2 = default(Segment);
			iSeg2.Origin = this.Position;
			Vector3.Subtract(ref iSeg.Origin, ref iSeg2.Origin, out iSeg2.Delta);
			bool flag = false;
			for (int i = 0; i < shields.Count; i++)
			{
				Vector3 vector;
				if (shields[i].SegmentIntersect(out vector, iSeg2, 1f))
				{
					flag = true;
				}
			}
			float num;
			Vector3 vector2;
			Vector3 vector3;
			AnimatedLevelPart animatedLevelPart;
			if (!flag && this.mPlayState.Level.CurrentScene.SegmentIntersect(out num, out vector2, out vector3, out animatedLevelPart, iSeg))
			{
				Vector3 forward = this.mBody.Orientation.Forward;
				Vector3.Transform(ref forward, ref this.mNextMineRotation, out forward);
				List<Entity> entities = this.mPlayState.EntityManager.GetEntities(vector2, 1f, false);
				for (int j = 0; j < entities.Count; j++)
				{
					if (entities[j] is Barrier)
					{
						entities[j].Kill();
					}
					else if (entities[j] is SpellMine)
					{
						(entities[j] as SpellMine).Detonate();
					}
				}
				this.mPlayState.EntityManager.ReturnEntityList(entities);
				SpellMine instance = SpellMine.GetInstance();
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					SpawnMineMessage spawnMineMessage;
					spawnMineMessage.Handle = instance.Handle;
					spawnMineMessage.OwnerHandle = this.mOwner.Handle;
					spawnMineMessage.AnimationHandle = ((animatedLevelPart == null) ? ushort.MaxValue : animatedLevelPart.Handle);
					spawnMineMessage.Position = vector2;
					spawnMineMessage.Direction = forward;
					spawnMineMessage.Scale = this.mScale;
					spawnMineMessage.Spell = this.mSpell;
					spawnMineMessage.Damage = this.mDamage;
					NetworkManager.Instance.Interface.SendMessage<SpawnMineMessage>(ref spawnMineMessage);
				}
				instance.Initialize(this.mOwner, vector2, forward, this.mScale, this.mNextMineRange, this.mNextMineDir, this.mNextMineRotation, this.mDistanceBetweenMines, ref this.mSpell, ref this.mDamage, animatedLevelPart);
				this.mPlayState.EntityManager.AddEntity(instance);
			}
		}

		// Token: 0x060006B2 RID: 1714 RVA: 0x00027FC5 File Offset: 0x000261C5
		public override void Deinitialize()
		{
			base.Deinitialize();
			this.mCollision.NonCollidables.Clear();
		}

		// Token: 0x17000154 RID: 340
		// (get) Token: 0x060006B3 RID: 1715 RVA: 0x00027FDD File Offset: 0x000261DD
		public override bool Dead
		{
			get
			{
				return this.mDead;
			}
		}

		// Token: 0x17000155 RID: 341
		// (get) Token: 0x060006B4 RID: 1716 RVA: 0x00027FE5 File Offset: 0x000261E5
		public override bool Removable
		{
			get
			{
				return this.mDead && this.mController.HasFinished;
			}
		}

		// Token: 0x060006B5 RID: 1717 RVA: 0x00027FFC File Offset: 0x000261FC
		public override void Kill()
		{
			this.mDead = true;
		}

		// Token: 0x060006B6 RID: 1718 RVA: 0x00028005 File Offset: 0x00026205
		protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
		{
			oMsg = default(EntityUpdateMessage);
			if (!this.Resting)
			{
				oMsg.Features |= EntityFeatures.Position;
				oMsg.Position = this.Position;
			}
		}

		// Token: 0x060006B7 RID: 1719 RVA: 0x00028031 File Offset: 0x00026231
		internal override float GetDanger()
		{
			if (this.mDetonationTimer <= 0f)
			{
				return 1f;
			}
			return 10000f;
		}

		// Token: 0x17000156 RID: 342
		// (get) Token: 0x060006B8 RID: 1720 RVA: 0x0002804B File Offset: 0x0002624B
		public float HitPoints
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x17000157 RID: 343
		// (get) Token: 0x060006B9 RID: 1721 RVA: 0x00028052 File Offset: 0x00026252
		public float MaxHitPoints
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x060006BA RID: 1722 RVA: 0x0002805C File Offset: 0x0002625C
		public bool SegmentIntersect(out Vector3 oPosition, Segment iSeg, float iSegmentRadius)
		{
			float num;
			Vector3 vector;
			return this.mBody.CollisionSkin.SegmentIntersect(out num, out oPosition, out vector, iSeg);
		}

		// Token: 0x060006BB RID: 1723 RVA: 0x00028080 File Offset: 0x00026280
		public DamageResult InternalDamage(DamageCollection5 iDamages, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			DamageResult damageResult = DamageResult.None;
			damageResult |= this.InternalDamage(iDamages.A, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			damageResult |= this.InternalDamage(iDamages.B, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			damageResult |= this.InternalDamage(iDamages.C, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			damageResult |= this.InternalDamage(iDamages.D, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			damageResult |= this.InternalDamage(iDamages.E, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			if ((damageResult & DamageResult.Damaged) == DamageResult.Damaged)
			{
				this.mDetonationTimer = 0.1f;
			}
			return damageResult;
		}

		// Token: 0x060006BC RID: 1724 RVA: 0x0002810F File Offset: 0x0002630F
		public DamageResult InternalDamage(Damage iDamage, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			if (Math.Abs(iDamage.Amount * iDamage.Magnitude) > 0f && ((short)(iDamage.AttackProperty & AttackProperties.Damage) == 1 || (short)(iDamage.AttackProperty & AttackProperties.Status) == 32))
			{
				return DamageResult.Damaged;
			}
			return DamageResult.None;
		}

		// Token: 0x060006BD RID: 1725 RVA: 0x0002814B File Offset: 0x0002634B
		public void OverKill()
		{
			this.Detonate();
		}

		// Token: 0x060006BE RID: 1726 RVA: 0x00028153 File Offset: 0x00026353
		public void Electrocute(IDamageable iTarget, float iMultiplyer)
		{
		}

		// Token: 0x060006BF RID: 1727 RVA: 0x00028158 File Offset: 0x00026358
		// Note: this type is marked as 'beforefieldinit'.
		static SpellMine()
		{
			int[] array = new int[11];
			array[1] = "water_detonation".GetHashCodeCustom();
			array[2] = "cold_detonation".GetHashCodeCustom();
			array[3] = "fire_detonation".GetHashCodeCustom();
			array[4] = "lightning_detonation".GetHashCodeCustom();
			array[5] = "arcane_detonation".GetHashCodeCustom();
			array[6] = "life_detonation".GetHashCodeCustom();
			array[9] = "steam_detonation".GetHashCodeCustom();
			SpellMine.DETONATION_EFFECTS = array;
			SpellMine.EXPLOSION_EFFECT = "mine_explosion".GetHashCodeCustom();
			SpellMine.ACTIVATION_EFFECT = "mine_activation".GetHashCodeCustom();
			SpellMine.SOUND_CAST = "spell_mine01".GetHashCodeCustom();
			SpellMine.SOUND_EXPLOSION = "spell_mine02".GetHashCodeCustom();
			SpellMine.SOUND_ACTIVATE = "spell_mine_activate".GetHashCodeCustom();
			SpellMine.RANDOM = new Random();
		}

		// Token: 0x0400055A RID: 1370
		public const float RADIUS = 1f;

		// Token: 0x0400055B RID: 1371
		public const float MAX_DISTANCE_SQ = 900f;

		// Token: 0x0400055C RID: 1372
		private const float DETONATION_TIMER = 0.5f;

		// Token: 0x0400055D RID: 1373
		private static Queue<SpellMine> sCache;

		// Token: 0x0400055E RID: 1374
		public static readonly int[] DETONATION_EFFECTS;

		// Token: 0x0400055F RID: 1375
		public static readonly int EXPLOSION_EFFECT;

		// Token: 0x04000560 RID: 1376
		public static readonly int ACTIVATION_EFFECT;

		// Token: 0x04000561 RID: 1377
		public static readonly int SOUND_CAST;

		// Token: 0x04000562 RID: 1378
		public static readonly int SOUND_EXPLOSION;

		// Token: 0x04000563 RID: 1379
		public static readonly int SOUND_ACTIVATE;

		// Token: 0x04000564 RID: 1380
		private ISpellCaster mOwner;

		// Token: 0x04000565 RID: 1381
		private DamageCollection5 mDamage;

		// Token: 0x04000566 RID: 1382
		private Spell mSpell;

		// Token: 0x04000567 RID: 1383
		private bool mActivated;

		// Token: 0x04000568 RID: 1384
		private bool mOwnerCollision;

		// Token: 0x04000569 RID: 1385
		private float mDetonationTimer;

		// Token: 0x0400056A RID: 1386
		private float mNextMineTTL;

		// Token: 0x0400056B RID: 1387
		private Vector3 mNextMineDir;

		// Token: 0x0400056C RID: 1388
		private Quaternion mNextMineRotation;

		// Token: 0x0400056D RID: 1389
		private float mNextMineRange;

		// Token: 0x0400056E RID: 1390
		private float mDistanceBetweenMines;

		// Token: 0x0400056F RID: 1391
		private float mScale;

		// Token: 0x04000570 RID: 1392
		private SkinnedModelDeferredBasicMaterial mMaterial;

		// Token: 0x04000571 RID: 1393
		private SkinnedModel mModel;

		// Token: 0x04000572 RID: 1394
		private AnimationController mController;

		// Token: 0x04000573 RID: 1395
		private AnimationClip mClip;

		// Token: 0x04000574 RID: 1396
		private SpellMine.RenderData[] mRenderData;

		// Token: 0x04000575 RID: 1397
		private Vector3 mDirection;

		// Token: 0x04000576 RID: 1398
		private Vector3 mModelSize;

		// Token: 0x04000577 RID: 1399
		private VisualEffectReference mActivationEffect;

		// Token: 0x04000578 RID: 1400
		private double mTimeStamp;

		// Token: 0x04000579 RID: 1401
		protected float mRestingTimer = 1f;

		// Token: 0x0400057A RID: 1402
		private Matrix mModelMatrix;

		// Token: 0x0400057B RID: 1403
		private static readonly Random RANDOM;

		// Token: 0x020000DD RID: 221
		protected class RenderData : IRenderableObject
		{
			// Token: 0x060006C0 RID: 1728 RVA: 0x00028224 File Offset: 0x00026424
			public RenderData()
			{
				this.mBones = new Matrix[80];
			}

			// Token: 0x17000158 RID: 344
			// (get) Token: 0x060006C1 RID: 1729 RVA: 0x00028240 File Offset: 0x00026440
			public bool MeshDirty
			{
				get
				{
					return this.mMeshDirty;
				}
			}

			// Token: 0x17000159 RID: 345
			// (get) Token: 0x060006C2 RID: 1730 RVA: 0x00028248 File Offset: 0x00026448
			public int Effect
			{
				get
				{
					return SkinnedModelDeferredEffect.TYPEHASH;
				}
			}

			// Token: 0x1700015A RID: 346
			// (get) Token: 0x060006C3 RID: 1731 RVA: 0x0002824F File Offset: 0x0002644F
			public int DepthTechnique
			{
				get
				{
					return 3;
				}
			}

			// Token: 0x1700015B RID: 347
			// (get) Token: 0x060006C4 RID: 1732 RVA: 0x00028252 File Offset: 0x00026452
			public int Technique
			{
				get
				{
					return (int)this.mTechnique;
				}
			}

			// Token: 0x1700015C RID: 348
			// (get) Token: 0x060006C5 RID: 1733 RVA: 0x0002825A File Offset: 0x0002645A
			public int ShadowTechnique
			{
				get
				{
					return 4;
				}
			}

			// Token: 0x1700015D RID: 349
			// (get) Token: 0x060006C6 RID: 1734 RVA: 0x0002825D File Offset: 0x0002645D
			public VertexBuffer Vertices
			{
				get
				{
					return this.mVertexBuffer;
				}
			}

			// Token: 0x1700015E RID: 350
			// (get) Token: 0x060006C7 RID: 1735 RVA: 0x00028265 File Offset: 0x00026465
			public IndexBuffer Indices
			{
				get
				{
					return this.mIndexBuffer;
				}
			}

			// Token: 0x1700015F RID: 351
			// (get) Token: 0x060006C8 RID: 1736 RVA: 0x0002826D File Offset: 0x0002646D
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return this.mVertexDeclaration;
				}
			}

			// Token: 0x17000160 RID: 352
			// (get) Token: 0x060006C9 RID: 1737 RVA: 0x00028275 File Offset: 0x00026475
			public int VertexStride
			{
				get
				{
					return this.mVertexStride;
				}
			}

			// Token: 0x17000161 RID: 353
			// (get) Token: 0x060006CA RID: 1738 RVA: 0x0002827D File Offset: 0x0002647D
			public int VerticesHashCode
			{
				get
				{
					return this.mVerticesHash;
				}
			}

			// Token: 0x060006CB RID: 1739 RVA: 0x00028288 File Offset: 0x00026488
			public bool Cull(BoundingFrustum iViewFrustum)
			{
				BoundingSphere boundingSphere = this.mBoundingSphere;
				return boundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
			}

			// Token: 0x060006CC RID: 1740 RVA: 0x000282A8 File Offset: 0x000264A8
			public virtual void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				this.mMaterial.AssignToEffect(skinnedModelDeferredEffect);
				skinnedModelDeferredEffect.EmissiveAmount = 2f;
				skinnedModelDeferredEffect.Bones = this.mBones;
				skinnedModelDeferredEffect.CommitChanges();
				skinnedModelDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
			}

			// Token: 0x060006CD RID: 1741 RVA: 0x0002830C File Offset: 0x0002650C
			public virtual void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				if (this.mTechnique == SkinnedModelDeferredEffect.Technique.Additive)
				{
					return;
				}
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				this.mMaterial.AssignOpacityToEffect(skinnedModelDeferredEffect);
				skinnedModelDeferredEffect.Bones = this.mBones;
				skinnedModelDeferredEffect.CommitChanges();
				skinnedModelDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
			}

			// Token: 0x060006CE RID: 1742 RVA: 0x0002836D File Offset: 0x0002656D
			public void SetMeshDirty()
			{
				this.mMeshDirty = true;
			}

			// Token: 0x060006CF RID: 1743 RVA: 0x00028378 File Offset: 0x00026578
			public void SetMesh(VertexBuffer iVertices, IndexBuffer iIndices, ModelMeshPart iMeshPart, ref SkinnedModelDeferredBasicMaterial iBasicMaterial, SkinnedModelDeferredEffect.Technique iTechnique)
			{
				this.mMeshDirty = false;
				this.mMaterial.CopyFrom(ref iBasicMaterial);
				this.mVertexBuffer = iVertices;
				this.mVerticesHash = iVertices.GetHashCode();
				this.mIndexBuffer = iIndices;
				this.mVertexDeclaration = iMeshPart.VertexDeclaration;
				this.mBaseVertex = iMeshPart.BaseVertex;
				this.mNumVertices = iMeshPart.NumVertices;
				this.mPrimitiveCount = iMeshPart.PrimitiveCount;
				this.mStartIndex = iMeshPart.StartIndex;
				this.mStreamOffset = iMeshPart.StreamOffset;
				this.mVertexStride = iMeshPart.VertexStride;
				this.mTechnique = iTechnique;
			}

			// Token: 0x0400057C RID: 1404
			public BoundingSphere mBoundingSphere;

			// Token: 0x0400057D RID: 1405
			protected VertexDeclaration mVertexDeclaration;

			// Token: 0x0400057E RID: 1406
			protected int mBaseVertex;

			// Token: 0x0400057F RID: 1407
			protected int mNumVertices;

			// Token: 0x04000580 RID: 1408
			protected int mPrimitiveCount;

			// Token: 0x04000581 RID: 1409
			protected int mStartIndex;

			// Token: 0x04000582 RID: 1410
			protected int mStreamOffset;

			// Token: 0x04000583 RID: 1411
			protected int mVertexStride;

			// Token: 0x04000584 RID: 1412
			protected VertexBuffer mVertexBuffer;

			// Token: 0x04000585 RID: 1413
			protected IndexBuffer mIndexBuffer;

			// Token: 0x04000586 RID: 1414
			public Matrix[] mBones;

			// Token: 0x04000587 RID: 1415
			public SkinnedModelDeferredAdvancedMaterial mMaterial;

			// Token: 0x04000588 RID: 1416
			public SkinnedModelDeferredEffect.Technique mTechnique;

			// Token: 0x04000589 RID: 1417
			protected int mVerticesHash;

			// Token: 0x0400058A RID: 1418
			protected bool mMeshDirty = true;
		}
	}
}
