using System;
using System.Collections.Generic;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using JigLibX.Physics;
using Magicka.Achievements;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Graphics.Lights;
using Magicka.Levels;
using Magicka.Network;
using Magicka.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.Entities
{
	// Token: 0x02000505 RID: 1285
	public class MissileEntity : Entity, IDamageable
	{
		// Token: 0x06002626 RID: 9766 RVA: 0x00114010 File Offset: 0x00112210
		public static MissileEntity GetInstance(PlayState iPlayState)
		{
			MissileEntity missileEntity;
			lock (MissileEntity.sCache)
			{
				missileEntity = MissileEntity.sCache[0];
				MissileEntity.sCache.RemoveAt(0);
				MissileEntity.sCache.Add(missileEntity);
			}
			return missileEntity;
		}

		// Token: 0x06002627 RID: 9767 RVA: 0x00114068 File Offset: 0x00112268
		public static MissileEntity GetSpecificInstance(ushort iHandle)
		{
			MissileEntity missileEntity;
			lock (MissileEntity.sCache)
			{
				missileEntity = (Entity.GetFromHandle((int)iHandle) as MissileEntity);
				MissileEntity.sCache.Remove(missileEntity);
				MissileEntity.sCache.Add(missileEntity);
			}
			return missileEntity;
		}

		// Token: 0x06002628 RID: 9768 RVA: 0x001140C0 File Offset: 0x001122C0
		public static void InitializeCache(int iNr, PlayState iPlayState)
		{
			MissileEntity.sCache = new List<MissileEntity>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				MissileEntity.sCache.Add(new MissileEntity(iPlayState));
			}
		}

		// Token: 0x170008F7 RID: 2295
		// (get) Token: 0x06002629 RID: 9769 RVA: 0x001140F4 File Offset: 0x001122F4
		public float TimeAlive
		{
			get
			{
				return this.mTimeAlive;
			}
		}

		// Token: 0x0600262A RID: 9770 RVA: 0x001140FC File Offset: 0x001122FC
		public MissileEntity(PlayState iPlayState) : base(iPlayState)
		{
			this.mBody = new Body();
			this.mCollision = new CollisionSkin(this.mBody);
			this.mCollision.AddPrimitive(new Box(Vector3.Zero, Matrix.Identity, new Vector3(1f)), 1, new MaterialProperties(0.1f, 0.8f, 0.8f));
			this.mBody.CollisionSkin = this.mCollision;
			Vector3 vector = base.SetMass(4f);
			Transform transform = default(Transform);
			Vector3.Negate(ref vector, out transform.Position);
			transform.Orientation = Matrix.Identity;
			this.mCollision.ApplyLocalTransform(transform);
			this.mCollision.callbackFn += this.OnCollision;
			this.mBody.Tag = this;
			this.mBody.Immovable = false;
			this.mBody.AllowFreezing = true;
			this.mOwner = null;
			this.mHitList = new HitList(32);
			this.mGibbedHitList = new HitList(16);
			this.mConditionCollection = new ConditionCollection();
			this.mRenderData = new MissileEntity.RenderData[3][];
			this.mRenderData[0] = new MissileEntity.RenderData[3];
			for (int i = 0; i < 3; i++)
			{
				MissileEntity.RenderData renderData = new MissileEntity.RenderData();
				this.mRenderData[0][i] = renderData;
			}
			this.mRenderData[1] = new MissileEntity.RenderData[3];
			for (int j = 0; j < 3; j++)
			{
				MissileEntity.RenderData renderData2 = new MissileEntity.RenderData();
				this.mRenderData[1][j] = renderData2;
			}
			this.mRenderData[2] = new MissileEntity.RenderData[3];
			for (int k = 0; k < 3; k++)
			{
				MissileEntity.RenderData renderData3 = new MissileEntity.RenderData();
				this.mRenderData[2][k] = renderData3;
			}
			this.mEffectReferences = new Dictionary<int, VisualEffectReference>(20);
		}

		// Token: 0x0600262B RID: 9771 RVA: 0x001142D3 File Offset: 0x001124D3
		public void Initialize(Entity iOwner, Entity iTarget, float iHoming, float iRadius, ref Vector3 iPosition, ref Vector3 iVelocity, Model iModel, ConditionCollection iConditions, bool iCanHitOwner)
		{
			this.Initialize(iOwner, iRadius, ref iPosition, ref iVelocity, iModel, iConditions, iCanHitOwner);
			this.mTarget = iTarget;
			this.mHomingPower = iHoming;
		}

		// Token: 0x0600262C RID: 9772 RVA: 0x001142F8 File Offset: 0x001124F8
		public void Initialize(Entity iOwner, float iRadius, ref Vector3 iPosition, ref Vector3 iVelocity, Model iModel, ConditionCollection iConditions, bool iCanHitOwner, Spell iSpell)
		{
			this.Initialize(iOwner, iRadius, ref iPosition, ref iVelocity, iModel, iConditions, iCanHitOwner);
			if (iSpell.Element == Elements.Earth && iOwner is Avatar && (iOwner as Avatar).Player != null && !((iOwner as Avatar).Player.Gamer is NetworkGamer))
			{
				this.mDeepImpactPossible = true;
			}
		}

		// Token: 0x0600262D RID: 9773 RVA: 0x00114354 File Offset: 0x00112554
		public void Initialize(Entity iOwner, float iRadius, ref Vector3 iPosition, ref Vector3 iVelocity, Model iModel, ConditionCollection iConditions, bool iCanHitOwner)
		{
			foreach (KeyValuePair<int, VisualEffectReference> keyValuePair in this.mEffectReferences)
			{
				VisualEffectReference value = keyValuePair.Value;
				EffectManager.Instance.Stop(ref value);
			}
			this.mEffectReferences.Clear();
			this.mDeepImpactPossible = false;
			this.mDeepImpactCount = 0;
			this.mTarget = null;
			this.mLastKnownTargetPosition = Vector3.Zero;
			this.mModel = iModel;
			this.MarkRenderDataDirty();
			if (iConditions != null)
			{
				iConditions.CopyTo(this.mConditionCollection);
			}
			else
			{
				this.mConditionCollection.Clear();
			}
			this.mCombinedElements = Elements.None;
			this.mPiercing = false;
			for (int i = 0; i < this.mConditionCollection.Count; i++)
			{
				EventCollection eventCollection = this.mConditionCollection[i];
				for (int j = 0; j < eventCollection.Count; j++)
				{
					EventStorage eventStorage = eventCollection[j];
					if (eventStorage.EventType == EventType.Damage)
					{
						this.mCombinedElements |= eventStorage.DamageEvent.Damage.Element;
						if ((short)(eventStorage.DamageEvent.Damage.AttackProperty & AttackProperties.Piercing) != 0)
						{
							this.mPiercing = true;
						}
					}
					else if (eventStorage.EventType == EventType.Blast)
					{
						this.mCombinedElements |= eventStorage.BlastEvent.Damage.A.Element;
						this.mCombinedElements |= eventStorage.BlastEvent.Damage.B.Element;
						this.mCombinedElements |= eventStorage.BlastEvent.Damage.C.Element;
						this.mCombinedElements |= eventStorage.BlastEvent.Damage.D.Element;
						this.mCombinedElements |= eventStorage.BlastEvent.Damage.E.Element;
					}
					else if (eventStorage.EventType == EventType.Splash)
					{
						this.mCombinedElements |= eventStorage.SplashEvent.Damage.Element;
					}
				}
			}
			this.mDead = false;
			this.mHasCollided = false;
			this.mIsDamaged = false;
			this.mLevelCollisionTimer = 0f;
			this.mDanger = 2f;
			this.mHitList.Clear();
			this.mGibbedHitList.Clear();
			this.mRadius = iRadius;
			this.mSoundList = new List<Cue>(4);
			base.Initialize();
			this.mBody.ApplyGravity = true;
			this.mOwner = iOwner;
			this.mSpawnVelocity = iVelocity.Length();
			this.mHomingPower = 0f;
			this.mHomingTolerance = -0.75f;
			Vector3 sideLengths = new Vector3(iRadius * 2f);
			Vector3 value2 = new Vector3(iRadius);
			(this.mCollision.GetPrimitiveLocal(0) as Box).SideLengths = sideLengths;
			(this.mCollision.GetPrimitiveLocal(0) as Box).Position = -value2;
			(this.mCollision.GetPrimitiveNewWorld(0) as Box).SideLengths = sideLengths;
			(this.mCollision.GetPrimitiveNewWorld(0) as Box).Position = -value2;
			(this.mCollision.GetPrimitiveOldWorld(0) as Box).SideLengths = sideLengths;
			(this.mCollision.GetPrimitiveOldWorld(0) as Box).Position = -value2;
			this.mBody.MoveTo(iPosition, Matrix.Identity);
			this.mAudioEmitter.Position = iPosition;
			this.mAudioEmitter.Forward = Vector3.Forward;
			this.mAudioEmitter.Up = Vector3.Up;
			this.mCollision.NonCollidables.Clear();
			if (iOwner != null)
			{
				if (iCanHitOwner)
				{
					this.mHitList.Add(iOwner.Handle);
				}
				else
				{
					this.mCollision.NonCollidables.Add(iOwner.Body.CollisionSkin);
				}
			}
			this.mBody.Velocity = iVelocity;
			this.mBody.EnableBody();
			EventCondition eventCondition = default(EventCondition);
			eventCondition.EventConditionType = EventConditionType.Default;
			iConditions.ExecuteAll(this, null, ref eventCondition);
			this.mTimeAlive = 0f;
			this.mCollisionTarget = null;
		}

		// Token: 0x0600262E RID: 9774 RVA: 0x001147B8 File Offset: 0x001129B8
		public override void Deinitialize()
		{
			foreach (KeyValuePair<int, VisualEffectReference> keyValuePair in this.mEffectReferences)
			{
				VisualEffectReference value = keyValuePair.Value;
				EffectManager.Instance.Stop(ref value);
			}
			this.mEffectReferences.Clear();
			for (int i = 0; i < this.mSoundList.Count; i++)
			{
				if (!this.mSoundList[i].IsStopped && !this.mSoundList[i].IsStopping)
				{
					this.mSoundList[i].Stop(AudioStopOptions.AsAuthored);
				}
			}
			this.mSoundList.Clear();
			for (int j = 0; j < this.mLights.Count; j++)
			{
				this.mLights[j].Stop(true);
			}
			this.mLights.Clear();
			this.mProppMagickEffectActive = false;
			EffectManager.Instance.Stop(ref this.mProppMagickEffect);
			this.mEffectReferences.Clear();
			this.mConditionCollection.Clear();
			this.mCurrentSpell = null;
			base.Deinitialize();
		}

		// Token: 0x0600262F RID: 9775 RVA: 0x001148EC File Offset: 0x00112AEC
		public float ResistanceAgainst(Elements iElement)
		{
			return 0f;
		}

		// Token: 0x06002630 RID: 9776 RVA: 0x001148F4 File Offset: 0x00112AF4
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (this.mEffectReferences != null)
			{
				foreach (KeyValuePair<int, VisualEffectReference> keyValuePair in this.mEffectReferences)
				{
					VisualEffectReference value = keyValuePair.Value;
					Vector3 position = this.Position;
					Vector3 forward = this.GetOrientation().Forward;
					EffectManager.Instance.UpdatePositionDirection(ref value, ref position, ref forward);
				}
			}
			this.mHitList.Update(iDeltaTime);
			this.mGibbedHitList.Update(iDeltaTime);
			this.mTimeAlive += iDeltaTime;
			if (this.mVelocityChange)
			{
				this.mBody.Velocity = this.mNewVelocity;
				this.mVelocityChange = false;
			}
			Vector3 position2 = this.mBody.Position;
			if (this.mHomingPower > 0f)
			{
				if (this.mTarget != null && this.mTarget.Body != null)
				{
					this.mLastKnownTargetPosition = this.mTarget.Position;
					this.mLastKnownTargetPosition.Y = this.mLastKnownTargetPosition.Y + this.mTarget.Radius;
				}
				Vector3 velocity = this.mBody.Velocity;
				if (velocity.LengthSquared() > 1E-06f)
				{
					Vector3 vector;
					Vector3.Normalize(ref velocity, out vector);
					Vector3 vector2;
					Vector3.Subtract(ref this.mLastKnownTargetPosition, ref position2, out vector2);
					vector2.Normalize();
					Vector3 vector3;
					Vector3.Cross(ref vector, ref vector2, out vector3);
					float num;
					Vector3.Dot(ref vector, ref vector2, out num);
					if (num <= this.mHomingTolerance)
					{
						this.mHomingPower = 0f;
					}
					Quaternion quaternion;
					Quaternion.CreateFromAxisAngle(ref vector3, iDeltaTime * 10f * this.mHomingPower, out quaternion);
					Vector3.Transform(ref velocity, ref quaternion, out velocity);
					this.mBody.Velocity = velocity;
				}
			}
			EventCondition eventCondition = default(EventCondition);
			eventCondition.EventConditionType = EventConditionType.Timer;
			eventCondition.Time = this.mTimeAlive;
			if (this.mHasCollided)
			{
				if (this.mLevelCollisionTimer <= 1E-45f)
				{
					eventCondition.EventConditionType |= EventConditionType.Collision;
					eventCondition.Threshold = this.mCollisionVelocity;
				}
				this.mLevelCollisionTimer += iDeltaTime;
				if (this.mLevelCollisionTimer >= 0.15f)
				{
					this.mHasCollided = false;
					this.mLevelCollisionTimer = 0f;
				}
				this.mCollisionTarget = null;
			}
			this.mCollisionVelocity = 0f;
			if (this.mIsDamaged)
			{
				eventCondition.EventConditionType |= EventConditionType.Damaged;
				eventCondition.Hitpoints = this.mDamageAmount;
				eventCondition.Elements = this.mDamageElements;
			}
			NetworkState state = NetworkManager.Instance.State;
			if (this.SendsNetworkUpdate(state))
			{
				DamageResult damageResult;
				bool flag = this.mConditionCollection.ExecuteAll(this, this.mCollisionTarget, ref eventCondition, out damageResult);
				if (flag && state != NetworkState.Offline)
				{
					MissileEntityEventMessage missileEntityEventMessage = default(MissileEntityEventMessage);
					missileEntityEventMessage.Handle = base.Handle;
					missileEntityEventMessage.EventConditionType = eventCondition.EventConditionType;
					if (this.mCollisionTarget != null)
					{
						missileEntityEventMessage.TargetHandle = this.mCollisionTarget.Handle;
					}
					missileEntityEventMessage.TimeAlive = eventCondition.Time;
					missileEntityEventMessage.Threshold = eventCondition.Threshold;
					missileEntityEventMessage.HitPoints = eventCondition.Hitpoints;
					missileEntityEventMessage.Elements = eventCondition.Elements;
					missileEntityEventMessage.OnCollision = false;
					NetworkManager.Instance.Interface.SendMessage<MissileEntityEventMessage>(ref missileEntityEventMessage);
				}
			}
			this.mDamageElements = Elements.None;
			this.mDamageAmount = 0f;
			if (position2.Y <= -50f)
			{
				this.Kill();
			}
			BoundingSphere mBoundingSphere = default(BoundingSphere);
			mBoundingSphere.Center = this.Position;
			mBoundingSphere.Radius = this.mRadius;
			for (int i = 0; i < 3; i++)
			{
				Model model = this.mModel;
				MissileEntity.RenderData renderData = this.mRenderData[i][(int)iDataChannel];
				if (model != null)
				{
					renderData.mTransform = this.GetOrientation();
					renderData.mBoundingSphere = mBoundingSphere;
					if (renderData.IsDirty)
					{
						ModelMesh modelMesh = model.Meshes[0];
						ModelMeshPart iMeshPart = modelMesh.MeshParts[0];
						renderData.SetMesh(modelMesh.VertexBuffer, modelMesh.IndexBuffer, iMeshPart);
					}
					if (renderData.Effect == AdditiveEffect.TYPEHASH)
					{
						this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, renderData);
					}
					else
					{
						this.mPlayState.Scene.AddRenderableObject(iDataChannel, renderData);
					}
				}
				else if (renderData.IsDirty)
				{
					renderData.SetMesh(null, null, null);
				}
			}
			if (this.mProppMagickEffectActive)
			{
				Matrix orientation = this.mBody.Orientation;
				Vector3 position3 = this.mBody.Position;
				Vector3 vector4;
				Vector3.TransformNormal(ref this.mProppMagickLever, ref orientation, out vector4);
				Vector3.Add(ref position3, ref vector4, out position3);
				orientation.Translation = position3;
				if (!EffectManager.Instance.UpdateOrientation(ref this.mProppMagickEffect, ref orientation))
				{
					this.mProppMagickEffectActive = false;
				}
			}
			base.Update(iDataChannel, iDeltaTime);
			for (int j = 0; j < this.mSoundList.Count; j++)
			{
				this.mSoundList[j].Apply3D(this.mPlayState.Camera.Listener, base.AudioEmitter);
			}
			for (int k = 0; k < this.mLights.Count; k++)
			{
				this.mLights[k].Position = this.Position;
			}
		}

		// Token: 0x06002631 RID: 9777 RVA: 0x00114E38 File Offset: 0x00113038
		protected void MarkRenderDataDirty()
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					this.mRenderData[i][j].SetDirty();
				}
			}
		}

		// Token: 0x170008F8 RID: 2296
		// (get) Token: 0x06002632 RID: 9778 RVA: 0x00114E6C File Offset: 0x0011306C
		// (set) Token: 0x06002633 RID: 9779 RVA: 0x00114E74 File Offset: 0x00113074
		public bool FacingVelocity
		{
			get
			{
				return this.mFacingVelocity;
			}
			set
			{
				this.mFacingVelocity = value;
			}
		}

		// Token: 0x170008F9 RID: 2297
		// (get) Token: 0x06002634 RID: 9780 RVA: 0x00114E80 File Offset: 0x00113080
		public float NormalizedVelocity
		{
			get
			{
				return this.mBody.Velocity.Length() / 50f;
			}
		}

		// Token: 0x06002635 RID: 9781 RVA: 0x00114EA8 File Offset: 0x001130A8
		public override Matrix GetOrientation()
		{
			if (this.mFacingVelocity)
			{
				Matrix orientation = this.mBody.Orientation;
				Vector3 velocity = this.mBody.Velocity;
				velocity.Normalize();
				orientation.Forward = velocity;
				orientation.Right = Vector3.Normalize(Vector3.Cross(orientation.Forward, Vector3.Up));
				orientation.Up = Vector3.Normalize(Vector3.Cross(orientation.Right, orientation.Forward));
				orientation.Translation += this.mBody.Position;
				return orientation;
			}
			return base.GetOrientation();
		}

		// Token: 0x170008FA RID: 2298
		// (get) Token: 0x06002636 RID: 9782 RVA: 0x00114F48 File Offset: 0x00113148
		public Elements CombinedDamageElements
		{
			get
			{
				return this.mCombinedElements;
			}
		}

		// Token: 0x06002637 RID: 9783 RVA: 0x00114F50 File Offset: 0x00113150
		protected bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
		{
			if (iSkin1.Owner != null)
			{
				if (iSkin1.Owner is PhysicsObjectBody)
				{
					this.mHasCollided = true;
					this.mCollisionVelocity = Math.Max(this.mCollisionVelocity, this.mBody.Velocity.LengthSquared());
					if (this.mCollisionTarget == null)
					{
						this.mCollisionTarget = (iSkin1.Owner.Tag as Entity);
					}
				}
				if (iSkin1.Owner != null)
				{
					if (iSkin1.Owner.Tag is Pickable)
					{
						return false;
					}
					if (iSkin1.Owner.Tag is Character && (iSkin1.Owner.Tag as Character).IsEthereal)
					{
						return false;
					}
					if (iSkin1.Owner.Tag is CthulhuMist)
					{
						return false;
					}
					if (iSkin1.Owner.Tag is BossDamageZone)
					{
						if ((iSkin1.Owner.Tag as BossDamageZone).IsEthereal)
						{
							return false;
						}
					}
					else if (iSkin1.Owner.Tag is Grease.GreaseField)
					{
						return false;
					}
					Entity entity = iSkin1.Owner.Tag as Entity;
					if (entity == null)
					{
						return true;
					}
					if (entity is BossCollisionZone && !(entity is BossDamageZone))
					{
						return true;
					}
					if (entity.Dead | !(entity is IDamageable))
					{
						return false;
					}
					if ((entity is Barrier && !(entity as Barrier).Solid) | entity is SpellMine | entity is ElementalEgg | entity is TornadoEntity | entity is Grease.GreaseField)
					{
						return false;
					}
					if (entity is MissileEntity && (entity as MissileEntity).Owner == this.mOwner)
					{
						return false;
					}
					if (entity != null)
					{
						if (this.mGibbedHitList.ContainsKey(entity.Handle))
						{
							return false;
						}
						if (this.mHitList.ContainsKey(entity.Handle))
						{
							return (iSkin1.Owner != null && (iSkin1.Owner.Tag is Shield || iSkin1.Owner.Tag is Barrier)) || !this.mPiercing;
						}
						EventCondition eventCondition = default(EventCondition);
						eventCondition.EventConditionType = EventConditionType.Hit;
						NetworkState state = NetworkManager.Instance.State;
						if (!this.SendsNetworkUpdate(state))
						{
							return true;
						}
						DamageResult damageResult = DamageResult.None;
						bool flag = this.mConditionCollection.ExecuteAll(this, entity, ref eventCondition, out damageResult);
						if (flag && state != NetworkState.Offline)
						{
							MissileEntityEventMessage missileEntityEventMessage = default(MissileEntityEventMessage);
							missileEntityEventMessage.Handle = base.Handle;
							missileEntityEventMessage.EventConditionType = eventCondition.EventConditionType;
							if (entity != null)
							{
								missileEntityEventMessage.TargetHandle = entity.Handle;
							}
							missileEntityEventMessage.TimeAlive = eventCondition.Time;
							missileEntityEventMessage.Threshold = eventCondition.Threshold;
							missileEntityEventMessage.HitPoints = eventCondition.Hitpoints;
							missileEntityEventMessage.Elements = eventCondition.Elements;
							missileEntityEventMessage.OnCollision = true;
							NetworkManager.Instance.Interface.SendMessage<MissileEntityEventMessage>(ref missileEntityEventMessage);
						}
						if (this.mDeepImpactPossible && entity is Character && (damageResult & (DamageResult.Killed | DamageResult.OverKilled)) != DamageResult.None)
						{
							this.mDeepImpactCount++;
							if (this.mDeepImpactCount >= 5)
							{
								AchievementsManager.Instance.AwardAchievement(base.PlayState, "deepimpact");
							}
						}
						if ((damageResult & DamageResult.OverKilled) == DamageResult.OverKilled || (damageResult & DamageResult.Pierced) == DamageResult.Pierced || ((damageResult & DamageResult.Killed) == DamageResult.Killed && entity is Barrier))
						{
							if ((damageResult & DamageResult.Pierced) != DamageResult.Pierced)
							{
								Vector3 position = entity.Position;
								Vector3 position2 = this.Position;
								Vector3 velocity = this.mBody.Velocity;
								float num = velocity.Length();
								Vector3 vector;
								Vector3.Subtract(ref position2, ref position, out vector);
								Vector3.Normalize(ref vector, out vector);
								Vector3.Multiply(ref vector, num * 0.3f, out vector);
								Vector3.Normalize(ref velocity, out velocity);
								Vector3.Multiply(ref velocity, num * 0.9f, out velocity);
								Vector3 vector2;
								Vector3.Add(ref vector, ref velocity, out vector2);
								this.mVelocityChange = true;
								this.mNewVelocity = vector2;
							}
							this.mGibbedHitList.Add(entity.Handle, 5f);
							return false;
						}
						this.mHitList.Add(entity.Handle);
						if (entity is Shield | entity is Barrier)
						{
							return true;
						}
						if ((damageResult & DamageResult.Deflected) != DamageResult.None | (damageResult & DamageResult.Hit) != DamageResult.None)
						{
							return true;
						}
					}
				}
				return false;
			}
			if (!(iSkin1.Tag is LevelModel | iSkin1.Tag is Water | iSkin1.Tag is Lava))
			{
				return false;
			}
			this.mCollisionTarget = null;
			if (this.mHasCollided)
			{
				return true;
			}
			if (this.mLevelCollisionTimer > 0f)
			{
				return false;
			}
			this.mHasCollided = true;
			this.mCollisionVelocity = Math.Max(this.mCollisionVelocity, this.mBody.Velocity.LengthSquared());
			return true;
		}

		// Token: 0x06002638 RID: 9784 RVA: 0x00115418 File Offset: 0x00113618
		internal void SetProppMagickEffect(int iEffect, ref Vector3 iLeverFromMissile)
		{
			Matrix orientation = this.mBody.Orientation;
			Vector3 position = this.mBody.Position;
			this.mProppMagickEffectActive = true;
			this.mProppMagickLever = iLeverFromMissile;
			orientation.Translation = position;
			EffectManager.Instance.StartEffect(iEffect, ref orientation, out this.mProppMagickEffect);
		}

		// Token: 0x06002639 RID: 9785 RVA: 0x0011546C File Offset: 0x0011366C
		internal override bool SendsNetworkUpdate(NetworkState iState)
		{
			Avatar avatar = this.mOwner as Avatar;
			return (avatar != null && !(avatar.Player.Gamer is NetworkGamer)) || (avatar == null && iState != NetworkState.Client);
		}

		// Token: 0x0600263A RID: 9786 RVA: 0x001154A8 File Offset: 0x001136A8
		internal void NetworkEventMessage(ref MissileEntityEventMessage iMsg)
		{
			EventCondition eventCondition = default(EventCondition);
			eventCondition.EventConditionType = iMsg.EventConditionType;
			eventCondition.Time = iMsg.TimeAlive;
			eventCondition.Threshold = iMsg.Threshold;
			eventCondition.Hitpoints = iMsg.HitPoints;
			eventCondition.Elements = iMsg.Elements;
			Entity fromHandle = Entity.GetFromHandle((int)iMsg.TargetHandle);
			DamageResult damageResult;
			this.mConditionCollection.ExecuteAll(this, fromHandle, ref eventCondition, out damageResult);
			if (iMsg.OnCollision)
			{
				if (this.mDeepImpactPossible && fromHandle is Character && (damageResult & (DamageResult.Killed | DamageResult.OverKilled)) != DamageResult.None)
				{
					this.mDeepImpactCount++;
					if (this.mDeepImpactCount >= 5)
					{
						AchievementsManager.Instance.AwardAchievement(base.PlayState, "deepimpact");
					}
				}
				if (!this.mGibbedHitList.Contains(fromHandle as IDamageable))
				{
					if ((damageResult & DamageResult.OverKilled) == DamageResult.OverKilled || (damageResult & DamageResult.Pierced) == DamageResult.Pierced || ((damageResult & DamageResult.Killed) == DamageResult.Killed && fromHandle is Barrier))
					{
						this.mGibbedHitList.Add(fromHandle.Handle, 5f);
						return;
					}
					this.mHitList.Add(fromHandle.Handle);
				}
			}
		}

		// Token: 0x0600263B RID: 9787 RVA: 0x001155DC File Offset: 0x001137DC
		public bool SegmentIntersect(out Vector3 oPosition, Segment iSeg, float iSegmentRadius)
		{
			Vector3 position = this.mBody.Position;
			float t;
			float num;
			Distance.PointSegmentDistanceSq(out t, out num, ref position, ref iSeg);
			float num2 = iSegmentRadius + this.mRadius;
			num2 *= num2;
			if (num > num2)
			{
				oPosition = default(Vector3);
				return false;
			}
			float num3 = (float)Math.Sqrt((double)num);
			Vector3 vector;
			iSeg.GetPoint(t, out vector);
			Vector3.Lerp(ref position, ref vector, Math.Min(this.mRadius / num3, 1f), out oPosition);
			return true;
		}

		// Token: 0x0600263C RID: 9788 RVA: 0x00115650 File Offset: 0x00113850
		public bool ArcIntersect(out Vector3 oPosition, ref Vector3 iOrigin, ref Vector3 iDirection, float iRange, float iAngle, float iHeightDifference)
		{
			iOrigin.Y = 0f;
			iDirection.Y = 0f;
			Vector3 position = this.Position;
			position.Y = 0f;
			Vector3 vector;
			Vector3.Subtract(ref iOrigin, ref position, out vector);
			float num = vector.Length();
			if (num - this.mRadius > iRange)
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
			float num5 = (float)Math.Acos((double)((this.mRadius * this.mRadius + num4) / num4));
			if (num3 - num5 < iAngle)
			{
				Vector3.Multiply(ref vector, this.mRadius, out vector);
				position = this.Position;
				Vector3.Add(ref position, ref vector, out oPosition);
				return true;
			}
			oPosition = default(Vector3);
			return false;
		}

		// Token: 0x0600263D RID: 9789 RVA: 0x00115728 File Offset: 0x00113928
		public override Vector3 CalcImpulseVelocity(Vector3 iDirection, float iElevation, float iMassPower, float iDistance)
		{
			return base.CalcImpulseVelocity(iDirection, iElevation, iMassPower, iDistance);
		}

		// Token: 0x0600263E RID: 9790 RVA: 0x00115738 File Offset: 0x00113938
		public virtual DamageResult InternalDamage(DamageCollection5 iDamages, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			DamageResult damageResult = this.InternalDamage(iDamages.A, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			DamageResult damageResult2 = this.InternalDamage(iDamages.B, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			DamageResult damageResult3 = this.InternalDamage(iDamages.C, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			DamageResult damageResult4 = this.InternalDamage(iDamages.D, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			this.InternalDamage(iDamages.E, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			return damageResult | damageResult2 | damageResult3 | damageResult4;
		}

		// Token: 0x0600263F RID: 9791 RVA: 0x001157B0 File Offset: 0x001139B0
		public DamageResult InternalDamage(Damage iDamage, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			if (iDamage.Amount != 0f && !(iAttacker is Grease.GreaseField))
			{
				this.mIsDamaged = true;
				this.mDamageAmount += iDamage.Amount;
				this.mDamageElements |= iDamage.Element;
			}
			return DamageResult.None;
		}

		// Token: 0x06002640 RID: 9792 RVA: 0x00115803 File Offset: 0x00113A03
		public void Electrocute(IDamageable iTarget, float iMultiplyer)
		{
		}

		// Token: 0x170008FB RID: 2299
		// (get) Token: 0x06002641 RID: 9793 RVA: 0x00115805 File Offset: 0x00113A05
		// (set) Token: 0x06002642 RID: 9794 RVA: 0x0011580D File Offset: 0x00113A0D
		public float HomingTolerance
		{
			get
			{
				return this.mHomingTolerance;
			}
			set
			{
				this.mHomingTolerance = value;
			}
		}

		// Token: 0x170008FC RID: 2300
		// (get) Token: 0x06002643 RID: 9795 RVA: 0x00115816 File Offset: 0x00113A16
		// (set) Token: 0x06002644 RID: 9796 RVA: 0x0011581E File Offset: 0x00113A1E
		public float Homing
		{
			get
			{
				return this.mHomingPower;
			}
			set
			{
				this.mHomingPower = value;
			}
		}

		// Token: 0x06002645 RID: 9797 RVA: 0x00115827 File Offset: 0x00113A27
		public override void Kill()
		{
			this.mDead = true;
		}

		// Token: 0x06002646 RID: 9798 RVA: 0x00115830 File Offset: 0x00113A30
		public void OverKill()
		{
			this.mDead = true;
		}

		// Token: 0x170008FD RID: 2301
		// (get) Token: 0x06002647 RID: 9799 RVA: 0x00115839 File Offset: 0x00113A39
		public float HitPoints
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x170008FE RID: 2302
		// (get) Token: 0x06002648 RID: 9800 RVA: 0x00115840 File Offset: 0x00113A40
		public float MaxHitPoints
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x170008FF RID: 2303
		// (get) Token: 0x06002649 RID: 9801 RVA: 0x00115847 File Offset: 0x00113A47
		public override bool Dead
		{
			get
			{
				return this.mDead;
			}
		}

		// Token: 0x17000900 RID: 2304
		// (get) Token: 0x0600264A RID: 9802 RVA: 0x0011584F File Offset: 0x00113A4F
		public override bool Removable
		{
			get
			{
				return this.mDead;
			}
		}

		// Token: 0x17000901 RID: 2305
		// (get) Token: 0x0600264B RID: 9803 RVA: 0x00115857 File Offset: 0x00113A57
		public Entity Owner
		{
			get
			{
				return this.mOwner;
			}
		}

		// Token: 0x17000902 RID: 2306
		// (get) Token: 0x0600264C RID: 9804 RVA: 0x0011585F File Offset: 0x00113A5F
		// (set) Token: 0x0600264D RID: 9805 RVA: 0x00115867 File Offset: 0x00113A67
		public SpellEffect CurrentSpell
		{
			get
			{
				return this.mCurrentSpell;
			}
			set
			{
				this.mCurrentSpell = value;
			}
		}

		// Token: 0x0600264E RID: 9806 RVA: 0x00115870 File Offset: 0x00113A70
		public void AddEffectReference(int iEffectHash, VisualEffectReference iEffect)
		{
			if (!this.mEffectReferences.ContainsKey(iEffectHash))
			{
				this.mEffectReferences.Add(iEffectHash, iEffect);
				return;
			}
			throw new Exception();
		}

		// Token: 0x0600264F RID: 9807 RVA: 0x00115893 File Offset: 0x00113A93
		public void AddCue(Cue iCue)
		{
			this.mSoundList.Add(iCue);
		}

		// Token: 0x06002650 RID: 9808 RVA: 0x001158A1 File Offset: 0x00113AA1
		public void AddLight(DynamicLight iLight)
		{
			this.mLights.Add(iLight);
		}

		// Token: 0x17000903 RID: 2307
		// (get) Token: 0x06002651 RID: 9809 RVA: 0x001158AF File Offset: 0x00113AAF
		// (set) Token: 0x06002652 RID: 9810 RVA: 0x001158B7 File Offset: 0x00113AB7
		public float Danger
		{
			get
			{
				return this.Danger;
			}
			set
			{
				this.mDanger = value;
			}
		}

		// Token: 0x06002653 RID: 9811 RVA: 0x001158C0 File Offset: 0x00113AC0
		protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
		{
			oMsg = default(EntityUpdateMessage);
			oMsg.Features |= EntityFeatures.Position;
			oMsg.Position = this.Position;
			Vector3 velocity = this.mBody.Velocity;
			Vector3.Multiply(ref velocity, iPrediction, out velocity);
			Vector3.Add(ref velocity, ref oMsg.Position, out oMsg.Position);
			oMsg.Features |= EntityFeatures.Velocity;
			oMsg.Velocity = this.mBody.Velocity;
		}

		// Token: 0x06002654 RID: 9812 RVA: 0x00115938 File Offset: 0x00113B38
		internal override float GetDanger()
		{
			return this.mDanger;
		}

		// Token: 0x04002966 RID: 10598
		private static List<MissileEntity> sCache;

		// Token: 0x04002967 RID: 10599
		private Model mModel;

		// Token: 0x04002968 RID: 10600
		private bool mFacingVelocity = true;

		// Token: 0x04002969 RID: 10601
		private bool mHasCollided;

		// Token: 0x0400296A RID: 10602
		private float mCollisionVelocity;

		// Token: 0x0400296B RID: 10603
		private bool mIsDamaged;

		// Token: 0x0400296C RID: 10604
		private float mDamageAmount;

		// Token: 0x0400296D RID: 10605
		private Elements mDamageElements;

		// Token: 0x0400296E RID: 10606
		private float mLevelCollisionTimer;

		// Token: 0x0400296F RID: 10607
		private Entity mCollisionTarget;

		// Token: 0x04002970 RID: 10608
		private float mHomingTolerance;

		// Token: 0x04002971 RID: 10609
		private float mHomingPower;

		// Token: 0x04002972 RID: 10610
		private float mTimeAlive;

		// Token: 0x04002973 RID: 10611
		private ConditionCollection mConditionCollection;

		// Token: 0x04002974 RID: 10612
		private SpellEffect mCurrentSpell;

		// Token: 0x04002975 RID: 10613
		private List<DynamicLight> mLights = new List<DynamicLight>(2);

		// Token: 0x04002976 RID: 10614
		private float mSpawnVelocity;

		// Token: 0x04002977 RID: 10615
		private Entity mOwner;

		// Token: 0x04002978 RID: 10616
		private Entity mTarget;

		// Token: 0x04002979 RID: 10617
		private Vector3 mLastKnownTargetPosition;

		// Token: 0x0400297A RID: 10618
		private HitList mHitList;

		// Token: 0x0400297B RID: 10619
		private HitList mGibbedHitList;

		// Token: 0x0400297C RID: 10620
		private MissileEntity.RenderData[][] mRenderData;

		// Token: 0x0400297D RID: 10621
		private List<Cue> mSoundList;

		// Token: 0x0400297E RID: 10622
		private Dictionary<int, VisualEffectReference> mEffectReferences;

		// Token: 0x0400297F RID: 10623
		private float mDanger;

		// Token: 0x04002980 RID: 10624
		private bool mPiercing;

		// Token: 0x04002981 RID: 10625
		private int mDeepImpactCount;

		// Token: 0x04002982 RID: 10626
		private bool mDeepImpactPossible;

		// Token: 0x04002983 RID: 10627
		private Vector3 mProppMagickLever;

		// Token: 0x04002984 RID: 10628
		private VisualEffectReference mProppMagickEffect;

		// Token: 0x04002985 RID: 10629
		private bool mProppMagickEffectActive;

		// Token: 0x04002986 RID: 10630
		private bool mVelocityChange;

		// Token: 0x04002987 RID: 10631
		private Vector3 mNewVelocity;

		// Token: 0x04002988 RID: 10632
		private Elements mCombinedElements;

		// Token: 0x02000506 RID: 1286
		protected class RenderData : IRenderableObject, IRenderableAdditiveObject
		{
			// Token: 0x17000904 RID: 2308
			// (get) Token: 0x06002655 RID: 9813 RVA: 0x00115940 File Offset: 0x00113B40
			public bool IsDirty
			{
				get
				{
					return this.mDirty;
				}
			}

			// Token: 0x06002656 RID: 9814 RVA: 0x00115948 File Offset: 0x00113B48
			public void SetDirty()
			{
				this.mDirty = true;
			}

			// Token: 0x17000905 RID: 2309
			// (get) Token: 0x06002657 RID: 9815 RVA: 0x00115951 File Offset: 0x00113B51
			public int Effect
			{
				get
				{
					return this.mEffect;
				}
			}

			// Token: 0x17000906 RID: 2310
			// (get) Token: 0x06002658 RID: 9816 RVA: 0x00115959 File Offset: 0x00113B59
			public int DepthTechnique
			{
				get
				{
					return 4;
				}
			}

			// Token: 0x17000907 RID: 2311
			// (get) Token: 0x06002659 RID: 9817 RVA: 0x0011595C File Offset: 0x00113B5C
			public int Technique
			{
				get
				{
					if (this.mEffect != AdditiveEffect.TYPEHASH)
					{
						return 0;
					}
					return 0;
				}
			}

			// Token: 0x17000908 RID: 2312
			// (get) Token: 0x0600265A RID: 9818 RVA: 0x0011596E File Offset: 0x00113B6E
			public int ShadowTechnique
			{
				get
				{
					return 5;
				}
			}

			// Token: 0x17000909 RID: 2313
			// (get) Token: 0x0600265B RID: 9819 RVA: 0x00115971 File Offset: 0x00113B71
			public VertexBuffer Vertices
			{
				get
				{
					return this.mVertexBuffer;
				}
			}

			// Token: 0x1700090A RID: 2314
			// (get) Token: 0x0600265C RID: 9820 RVA: 0x00115979 File Offset: 0x00113B79
			public IndexBuffer Indices
			{
				get
				{
					return this.mIndexBuffer;
				}
			}

			// Token: 0x1700090B RID: 2315
			// (get) Token: 0x0600265D RID: 9821 RVA: 0x00115981 File Offset: 0x00113B81
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return this.mVertexDeclaration;
				}
			}

			// Token: 0x1700090C RID: 2316
			// (get) Token: 0x0600265E RID: 9822 RVA: 0x00115989 File Offset: 0x00113B89
			public int VertexStride
			{
				get
				{
					return this.mVertexStride;
				}
			}

			// Token: 0x1700090D RID: 2317
			// (get) Token: 0x0600265F RID: 9823 RVA: 0x00115991 File Offset: 0x00113B91
			public int VerticesHashCode
			{
				get
				{
					return this.mVerticesHash;
				}
			}

			// Token: 0x06002660 RID: 9824 RVA: 0x0011599C File Offset: 0x00113B9C
			public bool Cull(BoundingFrustum iViewFrustum)
			{
				BoundingSphere boundingSphere = this.mBoundingSphere;
				return boundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
			}

			// Token: 0x06002661 RID: 9825 RVA: 0x001159BC File Offset: 0x00113BBC
			public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				AdditiveEffect additiveEffect = iEffect as AdditiveEffect;
				RenderDeferredEffect renderDeferredEffect = iEffect as RenderDeferredEffect;
				if (additiveEffect != null)
				{
					this.mAdditiveMaterial.AssignToEffect(additiveEffect);
					additiveEffect.World = this.mTransform;
				}
				if (renderDeferredEffect != null)
				{
					this.mRenderDeferredMaterial.AssignToEffect(renderDeferredEffect);
					renderDeferredEffect.World = this.mTransform;
				}
				iEffect.CommitChanges();
				iEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
			}

			// Token: 0x06002662 RID: 9826 RVA: 0x00115A38 File Offset: 0x00113C38
			public void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				RenderDeferredEffect renderDeferredEffect = iEffect as RenderDeferredEffect;
				this.mRenderDeferredMaterial.AssignToEffect(renderDeferredEffect);
				renderDeferredEffect.World = this.mTransform;
				iEffect.CommitChanges();
				iEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
			}

			// Token: 0x06002663 RID: 9827 RVA: 0x00115A90 File Offset: 0x00113C90
			public void SetMesh(VertexBuffer iVertices, IndexBuffer iIndices, ModelMeshPart iMeshPart)
			{
				this.mDirty = false;
				if (iMeshPart == null)
				{
					this.mAdditiveMaterial = default(AdditiveMaterial);
					this.mRenderDeferredMaterial = default(RenderDeferredMaterial);
					this.mVertexBuffer = null;
					this.mVerticesHash = 0;
					this.mIndexBuffer = null;
					this.mVertexDeclaration = null;
					this.mBaseVertex = 0;
					this.mNumVertices = 0;
					this.mPrimitiveCount = 0;
					this.mStartIndex = 0;
					this.mStreamOffset = 0;
					this.mVertexStride = 0;
					this.mEffect = 0;
					return;
				}
				AdditiveEffect additiveEffect = iMeshPart.Effect as AdditiveEffect;
				RenderDeferredEffect renderDeferredEffect = iMeshPart.Effect as RenderDeferredEffect;
				if (additiveEffect != null)
				{
					this.mAdditiveMaterial.FetchFromEffect(additiveEffect);
					this.mEffect = AdditiveEffect.TYPEHASH;
				}
				if (renderDeferredEffect != null)
				{
					this.mRenderDeferredMaterial.FetchFromEffect(renderDeferredEffect);
					this.mEffect = RenderDeferredEffect.TYPEHASH;
				}
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
			}

			// Token: 0x04002989 RID: 10633
			protected AdditiveMaterial mAdditiveMaterial;

			// Token: 0x0400298A RID: 10634
			protected RenderDeferredMaterial mRenderDeferredMaterial;

			// Token: 0x0400298B RID: 10635
			protected int mEffect;

			// Token: 0x0400298C RID: 10636
			protected VertexDeclaration mVertexDeclaration;

			// Token: 0x0400298D RID: 10637
			protected int mBaseVertex;

			// Token: 0x0400298E RID: 10638
			protected int mNumVertices;

			// Token: 0x0400298F RID: 10639
			protected int mPrimitiveCount;

			// Token: 0x04002990 RID: 10640
			protected int mStartIndex;

			// Token: 0x04002991 RID: 10641
			protected int mStreamOffset;

			// Token: 0x04002992 RID: 10642
			protected int mVertexStride;

			// Token: 0x04002993 RID: 10643
			protected VertexBuffer mVertexBuffer;

			// Token: 0x04002994 RID: 10644
			protected int mVerticesHash;

			// Token: 0x04002995 RID: 10645
			protected IndexBuffer mIndexBuffer;

			// Token: 0x04002996 RID: 10646
			public Matrix mTransform;

			// Token: 0x04002997 RID: 10647
			public BoundingSphere mBoundingSphere;

			// Token: 0x04002998 RID: 10648
			protected bool mDirty;
		}
	}
}
