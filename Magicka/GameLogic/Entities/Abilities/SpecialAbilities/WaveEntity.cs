using System;
using System.Collections.Generic;
using JigLibX.Collision;
using JigLibX.Geometry;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using XNAnimation;
using XNAnimation.Controllers;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000543 RID: 1347
	public class WaveEntity : Barrier
	{
		// Token: 0x06002802 RID: 10242 RVA: 0x00124554 File Offset: 0x00122754
		public new static void InitializeCache(int iNrOfWaves, PlayState iPlayState)
		{
			WaveEntity.mWaveCache = new List<WaveEntity>(iNrOfWaves);
			for (int i = 0; i < iNrOfWaves; i++)
			{
				WaveEntity.mWaveCache.Add(new WaveEntity(iPlayState));
			}
		}

		// Token: 0x06002803 RID: 10243 RVA: 0x00124588 File Offset: 0x00122788
		public new static WaveEntity GetFromCache(PlayState iPlayState)
		{
			if (WaveEntity.mWaveCache.Count > 0)
			{
				WaveEntity result = WaveEntity.mWaveCache[WaveEntity.mWaveCache.Count - 1];
				WaveEntity.mWaveCache.RemoveAt(WaveEntity.mWaveCache.Count - 1);
				return result;
			}
			return new WaveEntity(iPlayState);
		}

		// Token: 0x06002804 RID: 10244 RVA: 0x001245D7 File Offset: 0x001227D7
		public static void ReturnToCache(WaveEntity iWave)
		{
			if (!WaveEntity.mWaveCache.Contains(iWave))
			{
				WaveEntity.mWaveCache.Add(iWave);
			}
		}

		// Token: 0x06002805 RID: 10245 RVA: 0x001245F4 File Offset: 0x001227F4
		public WaveEntity(PlayState iPlayState) : base(iPlayState)
		{
			SkinnedModel skinnedModel;
			lock (Game.Instance.GraphicsDevice)
			{
				WaveEntity.sWaveBarrierModels = new SkinnedModel[1];
				WaveEntity.sWaveBarrierModels[0] = Game.Instance.Content.Load<SkinnedModel>("Models/Effects/rockpillar0");
				skinnedModel = Game.Instance.Content.Load<SkinnedModel>("Models/Effects/rockpillar_animation");
			}
			WaveEntity.sWaveAppearClips = new AnimationClip[skinnedModel.AnimationClips.Count];
			int num = 0;
			foreach (AnimationClip animationClip in skinnedModel.AnimationClips.Values)
			{
				WaveEntity.sWaveAppearClips[num++] = animationClip;
			}
		}

		// Token: 0x06002806 RID: 10246 RVA: 0x001246D0 File Offset: 0x001228D0
		public void Initialize(ISpellCaster iOwner, Vector3 iPosition, Vector3 iDirection, float iScale, float iRange, Vector3 iNextDirection, Quaternion iNextRotation, float iDistanceBetweenBarriers, ref Spell iSpell, ref DamageCollection5 iDamage, ref Barrier.HitListWithBarriers iHitList, AnimatedLevelPart iAnimation, ref Wave iWave)
		{
			if (this.mSoundCue != null && !this.mSoundCue.IsStopping)
			{
				this.mSoundCue.Stop(AudioStopOptions.AsAuthored);
			}
			this.mEarthAnimationController.Stop();
			EffectManager.Instance.Stop(ref this.mEffect);
			if ((this.mDrawMethod & Barrier.DrawMethod.PARTICLEWALL) == Barrier.DrawMethod.PARTICLEWALL)
			{
				this.mDrawMethod &= ~Barrier.DrawMethod.PARTICLEWALL;
			}
			if (this.mHitList != null)
			{
				this.mHitList.Owners.Remove(this);
				if (this.mHitList.Owners.Count == 0)
				{
					this.mHitList.Destroy();
				}
			}
			this.mHitList = null;
			this.mParent = iWave;
			this.mAnimationTimer = WaveEntity.sWaveAppearClips[0].Duration;
			this.mInitilizeDamage = false;
			if (iAnimation != null)
			{
				iAnimation.AddEntity(this);
			}
			this.mSpell = iSpell;
			this.mHitList = iHitList;
			this.mHitList.Owners.Add(this);
			this.mDirection = iDirection;
			this.mEarthAnimationController.PlaybackMode = PlaybackMode.Forward;
			this.mNextBarrierTTL = 0.075f;
			Vector3.Transform(ref iNextDirection, ref iNextRotation, out this.mNextBarrierDir);
			this.mNextBarrierRotation = iNextRotation;
			this.mNextBarrierRange = iRange - iDistanceBetweenBarriers;
			this.mDistanceBetweenBarriers = iDistanceBetweenBarriers;
			this.mScale = iScale;
			this.mRuneRotation = 0f;
			this.mNormalizedDamage = 0f;
			this.mNormalizedDamageTarget = 0f;
			this.mOwner = iOwner;
			if (iOwner != null)
			{
				this.mTimeStamp = iOwner.PlayState.PlayTime;
			}
			else
			{
				this.mTimeStamp = base.PlayState.PlayTime;
			}
			this.mDamageTimer = 0f;
			this.mDrawMethod = Barrier.DrawMethod.NONE;
			this.mDamageSelf = true;
			Elements elements = this.mSpell.Element & ~Elements.Shield;
			if ((elements & Elements.PhysicalElements) != Elements.None)
			{
				this.mBarrierType = Barrier.BarrierType.SOLID;
			}
			else
			{
				this.mBarrierType = Barrier.BarrierType.ELEMENTAL;
			}
			DamageCollection5 iDamage2 = default(DamageCollection5);
			if (this.mBarrierType == Barrier.BarrierType.SOLID)
			{
				this.mRadius = Barrier.GetRadius(true) * this.mScale;
				if ((iSpell.Element & Elements.PhysicalElements) == Elements.PhysicalElements)
				{
					this.mSoundCue = AudioManager.Instance.GetCue(Banks.Spells, Barrier.Ice_Earth_Barrier_Sound_Hash);
				}
				else if ((iSpell.Element & Elements.Ice) == Elements.Ice)
				{
					this.mSoundCue = AudioManager.Instance.GetCue(Banks.Spells, Barrier.Ice_Barrier_Sound_Hash);
				}
				else if ((iSpell.Element & Elements.Earth) == Elements.Earth)
				{
					this.mSoundCue = AudioManager.Instance.GetCue(Banks.Spells, Barrier.Earth_Barrier_Sound_Hash);
				}
				this.mAddedEffect = false;
				this.mSpell.CalculateDamage(SpellType.Projectile, CastType.Area, out iDamage2);
				if ((iSpell.Element & Elements.Earth) == Elements.Earth)
				{
					this.mSpell.CalculateDamage(SpellType.Projectile, CastType.Area, out iDamage2);
					Vector3 forward = this.Body.Orientation.Forward;
					SkinnedModel skinnedModel = Barrier.sEarthBarrierModels[Barrier.mRandom.Next(WaveEntity.sWaveBarrierModels.Length)];
					AnimationClip[] array = WaveEntity.sWaveAppearClips;
					if ((iSpell.Element & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Steam | Elements.Poison)) != Elements.None && (iSpell.Element & Elements.Beams) == Elements.None)
					{
						this.mAddedEffect = true;
						this.mEffectAttach = Barrier.sVulcanoAttach;
						Elements elements2 = Elements.None;
						float num = -1f;
						for (int i = 0; i < 11; i++)
						{
							Elements elements3 = Defines.ElementFromIndex(i);
							if ((elements3 & Elements.Lightning) == Elements.Lightning && (this.mSpell.Element & Elements.Lightning) == Elements.Lightning)
							{
								elements2 = elements3;
								num = this.mSpell[elements2];
								i = 11;
							}
							else if ((elements3 & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Lightning | Elements.Steam)) != Elements.None && this.mSpell[elements3] > num)
							{
								elements2 = elements3;
								num = this.mSpell[elements2];
							}
						}
						if ((elements2 & Elements.Steam) == Elements.Steam)
						{
							this.mEffectTTL = this.mSpell.SteamMagnitude * 5f;
						}
						if ((elements2 & Elements.Water) == Elements.Water)
						{
							this.mEffectTTL = this.mSpell.WaterMagnitude * 5f;
						}
						Matrix.Identity.Translation = iPosition;
					}
					ModelMesh modelMesh = skinnedModel.Model.Meshes[0];
					ModelMeshPart iMeshPart = modelMesh.MeshParts[0];
					this.mEarthAnimationController.Skeleton = skinnedModel.SkeletonBones;
					float overlayAlpha = MathHelper.Clamp((iSpell.EarthMagnitude - 1f) * 0.333333f, 0f, 1f);
					for (int j = 0; j < 3; j++)
					{
						Barrier.RenderData renderData = this.mEarthRenderData[j];
						renderData.SetMesh(modelMesh.VertexBuffer, modelMesh.IndexBuffer, iMeshPart);
						renderData.mSkinnedModelMaterial.OverlayAlpha = overlayAlpha;
					}
					this.mEarthAnimationController.StartClip(array[Barrier.mRandom.Next(array.Length)], false);
					this.mDrawMethod |= Barrier.DrawMethod.NORMAL;
				}
			}
			Matrix matrix;
			MagickaMath.MakeOrientationMatrix(ref iDirection, out matrix);
			Vector3 pos = iPosition;
			pos.Y += Barrier.GetRadius(base.Solid) * this.mScale + 0.05f;
			Matrix matrix2 = Matrix.CreateScale(this.mScale);
			Matrix orientation;
			Matrix.Multiply(ref matrix2, ref matrix, out orientation);
			this.mBody.MoveTo(pos, orientation);
			this.mBody.CollisionSkin.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
			(this.mCollision.GetPrimitiveLocal(0) as Capsule).Radius = this.mRadius;
			(this.mCollision.GetPrimitiveNewWorld(0) as Capsule).Radius = this.mRadius;
			(this.mCollision.GetPrimitiveOldWorld(0) as Capsule).Radius = this.mRadius;
			float radius = base.Solid ? (this.mRadius * 2f) : this.mRadius;
			(this.mCollision.GetPrimitiveLocal(1) as Sphere).Radius = radius;
			(this.mCollision.GetPrimitiveLocal(1) as Sphere).Position = new Vector3(0f, -this.mRadius, 0f);
			(this.mCollision.GetPrimitiveNewWorld(1) as Sphere).Radius = radius;
			(this.mCollision.GetPrimitiveNewWorld(1) as Sphere).Position = new Vector3(0f, -this.mRadius, 0f);
			(this.mCollision.GetPrimitiveOldWorld(1) as Sphere).Radius = radius;
			(this.mCollision.GetPrimitiveOldWorld(1) as Sphere).Position = new Vector3(0f, -this.mRadius, 0f);
			this.mVolume = (this.mCollision.GetPrimitiveLocal(0) as Capsule).GetVolume();
			List<Entity> entities = this.mPlayState.EntityManager.GetEntities(iPosition, iScale * Barrier.GetRadius(base.Solid) * 0.8f, false);
			for (int k = 0; k < entities.Count; k++)
			{
				Barrier barrier = entities[k] as Barrier;
				SpellMine spellMine = entities[k] as SpellMine;
				if (barrier != null && this.mHitList != barrier.HitList)
				{
					barrier.Kill();
				}
				else if (spellMine != null)
				{
					spellMine.Detonate();
				}
			}
			this.mPlayState.EntityManager.ReturnEntityList(entities);
			if (this.mInitilizeDamage)
			{
				entities = this.mPlayState.EntityManager.GetEntities(iPosition, iScale * Barrier.GetRadius(base.Solid) * 1.25f, false);
				for (int l = 0; l < entities.Count; l++)
				{
					IDamageable damageable = entities[l] as IDamageable;
					if (damageable != this && damageable != null && damageable != this.mOwner && !base.HitList.HitList.ContainsKey(damageable.Handle) && (!(damageable is Barrier) || (damageable as Barrier).HitList != base.HitList))
					{
						damageable.Damage(iDamage2, iOwner as Entity, this.mTimeStamp, iPosition);
						this.mHitList.HitList.Add(damageable.Handle, 0.25f);
					}
				}
				this.mPlayState.EntityManager.ReturnEntityList(entities);
			}
			this.mDamage = iDamage;
			if (base.Solid)
			{
				this.mHitPoints = (1f + iSpell[Elements.Earth]) * 500f;
			}
			else
			{
				this.mHitPoints = iSpell.TotalMagnitude() * 100f;
			}
			this.mArmour = (int)(iSpell[Elements.Ice] * 50f);
			this.mMaxHitPoints = this.mHitPoints;
			(this.mBody.CollisionSkin.GetPrimitiveLocal(0) as Capsule).Orientation = Matrix.CreateRotationX(-1.5707964f);
			base.Initialize();
			if (this.mSoundCue != null)
			{
				this.mSoundCue.Apply3D(this.mPlayState.Camera.Listener, base.AudioEmitter);
				this.mSoundCue.Play();
			}
			this.mDamage.A.Element = Elements.Earth;
			this.mDamage.B.Element = Elements.Earth;
			this.mDamage.C.Element = Elements.Earth;
			this.mDamage.D.Element = Elements.Earth;
			this.mDamage.E.Element = Elements.Earth;
			this.mDamage.A.AttackProperty = (AttackProperties.Damage | AttackProperties.Knockdown | AttackProperties.Pushed);
			this.mDamage.B.AttackProperty = AttackProperties.Knockback;
			this.mDamage.C.AttackProperty = AttackProperties.Knockback;
			this.mDamage.D.AttackProperty = AttackProperties.Knockback;
			this.mDamage.E.AttackProperty = AttackProperties.Knockback;
			this.mDamage.A.Amount = 0f;
			this.mDamage.B.Amount = 0f;
			this.mDamage.C.Amount = 0f;
			this.mDamage.D.Amount = 0f;
			this.mDamage.E.Amount = 0f;
			this.mDamage.A.Magnitude = 1f;
			this.mNextBarrierTTL = 0.1f;
		}

		// Token: 0x06002807 RID: 10247 RVA: 0x001250DC File Offset: 0x001232DC
		public override void Deinitialize()
		{
			if (this.mSoundCue != null && !this.mSoundCue.IsStopping)
			{
				this.mSoundCue.Stop(AudioStopOptions.AsAuthored);
			}
			this.mEarthAnimationController.Stop();
			EffectManager.Instance.Stop(ref this.mEffect);
			if (this.mHitList != null)
			{
				this.mHitList.Owners.Remove(this);
				if (this.mHitList.Owners.Count == 0)
				{
					this.mHitList.Destroy();
				}
			}
			this.mHitList = null;
			Entity entity;
			if (Entity.mUniqueEntities.TryGetValue(this.mUniqueID, out entity) && entity == this)
			{
				Entity.mUniqueEntities.Remove(this.mUniqueID);
			}
			this.mBody.DisableBody();
			WaveEntity.ReturnToCache(this);
		}

		// Token: 0x06002808 RID: 10248 RVA: 0x001251A0 File Offset: 0x001233A0
		private new void SpawnNextBarrier()
		{
			Vector3 origin = this.Position + this.mNextBarrierDir;
			Segment iSeg = default(Segment);
			iSeg.Delta.Y = -1.5f;
			iSeg.Origin = origin;
			iSeg.Origin.Y = iSeg.Origin.Y + (0.75f - (0.05f + this.Radius));
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
			if (!flag)
			{
				Vector3 vector2 = default(Vector3);
				float num;
				Vector3 vector3;
				AnimatedLevelPart animatedLevelPart;
				if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out num, out vector2, out vector3, out animatedLevelPart, iSeg))
				{
					Vector3 forward = this.mBody.Orientation.Forward;
					Vector3.Transform(ref forward, ref this.mNextBarrierRotation, out forward);
					List<Entity> entities = this.mPlayState.EntityManager.GetEntities(vector2, 1.5f * this.mRadius, false);
					for (int j = 0; j < entities.Count; j++)
					{
						if (entities[j] is Barrier && (entities[j] as Barrier).HitList != this.mHitList)
						{
							entities[j].Kill();
						}
						else if (entities[j] is SpellMine)
						{
							(entities[j] as SpellMine).Detonate();
						}
					}
					this.mPlayState.EntityManager.ReturnEntityList(entities);
					WaveEntity fromCache = WaveEntity.GetFromCache(this.mPlayState);
					if (NetworkManager.Instance.State == NetworkState.Server)
					{
						SpawnWaveMessage spawnWaveMessage;
						spawnWaveMessage.Handle = fromCache.Handle;
						spawnWaveMessage.OwnerHandle = base.Owner.Handle;
						spawnWaveMessage.AnimationHandle = ((animatedLevelPart == null) ? ushort.MaxValue : animatedLevelPart.Handle);
						spawnWaveMessage.Position = vector2;
						spawnWaveMessage.Direction = forward;
						spawnWaveMessage.Scale = this.mScale;
						spawnWaveMessage.Spell = this.mSpell;
						spawnWaveMessage.Damage = this.mDamage;
						spawnWaveMessage.HitlistHandle = this.mHitList.Handle;
						spawnWaveMessage.ParentHandle = 0;
						NetworkManager.Instance.Interface.SendMessage<SpawnWaveMessage>(ref spawnWaveMessage);
					}
					fromCache.Initialize(base.Owner, vector2, forward, this.mScale, this.mNextBarrierRange, this.mNextBarrierDir, this.mNextBarrierRotation, this.mDistanceBetweenBarriers, ref this.mSpell, ref this.mDamage, ref this.mHitList, animatedLevelPart, ref this.mParent);
					this.mParent.AddEntity(fromCache);
					this.mPlayState.EntityManager.AddEntity(fromCache);
				}
			}
		}

		// Token: 0x06002809 RID: 10249 RVA: 0x00125497 File Offset: 0x00123697
		public Wave Parent()
		{
			return this.mParent;
		}

		// Token: 0x0600280A RID: 10250 RVA: 0x001254A0 File Offset: 0x001236A0
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			Matrix orientation = this.GetOrientation();
			this.mAudioEmitter.Position = orientation.Translation;
			this.mAudioEmitter.Forward = orientation.Forward;
			this.mAudioEmitter.Up = orientation.Up;
			if (this.mSoundCue != null)
			{
				this.mSoundCue.Apply3D(this.mPlayState.Camera.Listener, base.AudioEmitter);
			}
			this.mEffectTTL -= iDeltaTime;
			this.mRuneRotation -= iDeltaTime * 0.25f;
			if (this.mNextBarrierRange > 1E-45f)
			{
				this.mNextBarrierTTL -= iDeltaTime;
				if (this.mNextBarrierTTL < 0f)
				{
					this.SpawnNextBarrier();
					this.mNextBarrierRange = 0f;
				}
			}
			if (this.mHitList.Owners[0] == this)
			{
				this.mHitList.HitList.Update(iDeltaTime);
			}
			this.mAnimationTimer -= iDeltaTime;
			if (this.mAnimationTimer < 0f)
			{
				this.mHitPoints = 0f;
			}
			if (this.mBody.Velocity.LengthSquared() > 1E-06f)
			{
				this.mRestingMovementTimer = 1f;
			}
			else
			{
				this.mRestingMovementTimer -= iDeltaTime;
			}
			this.mNormalizedDamageTarget = 1f - this.mHitPoints / this.mMaxHitPoints;
			this.mNormalizedDamage = this.mNormalizedDamageTarget + (this.mNormalizedDamage - this.mNormalizedDamageTarget) * (float)Math.Pow(0.15, (double)iDeltaTime);
			if ((this.mSpell.Element & Elements.Earth) != Elements.None)
			{
				this.mEarthAnimationController.Update(iDeltaTime, ref orientation, true);
			}
			if (this.mAddedEffect)
			{
				if (this.mEffectTTL > 0f)
				{
					Matrix matrix;
					Matrix.Multiply(ref this.mEffectAttach.mBindPose, ref this.mEarthAnimationController.SkinnedBoneTransforms[this.mEffectAttach.mIndex], out matrix);
					EffectManager.Instance.UpdateOrientation(ref this.mEffect, ref matrix);
				}
				else
				{
					EffectManager.Instance.Stop(ref this.mEffect);
					this.mAddedEffect = false;
				}
			}
			Barrier.RenderData renderData = this.mEarthRenderData[(int)iDataChannel];
			renderData.mBoundingSphere.Center = this.Position;
			renderData.mBoundingSphere.Radius = this.mRadius * 3f;
			renderData.mDamage = this.mNormalizedDamage;
			Array.Copy(this.mEarthAnimationController.SkinnedBoneTransforms, renderData.mBones, this.mEarthAnimationController.Skeleton.Count);
			this.mPlayState.Scene.AddRenderableObject(iDataChannel, renderData);
		}

		// Token: 0x0600280B RID: 10251 RVA: 0x00125734 File Offset: 0x00123934
		protected override bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
		{
			if (iSkin1.Owner == null)
			{
				return false;
			}
			if (!(iSkin1.Owner.Tag is Barrier))
			{
				IDamageable damageable = iSkin1.Owner.Tag as IDamageable;
				if (damageable != null && !damageable.Dead)
				{
					Character character = damageable as Character;
					if (character != null && character.IsEthereal)
					{
						return this.mBarrierType == Barrier.BarrierType.SOLID & iPrim0 == 0;
					}
					if (damageable is Character && character.CharacterBody.IsTouchingGround && !this.mParent.InHitlist(damageable))
					{
						damageable.Damage(this.mDamage, base.Owner as Entity, this.mTimeStamp, this.Position);
						this.mParent.AddToHitlist(damageable.Handle);
						if (!character.IsLevitating && !character.IsImmortal && !character.IsInAEvent && !character.IsEthereal && character.CharacterBody.Mass < 3000f)
						{
							Vector3 vector = new Vector3(0f, 10f, 0f);
							character.CharacterBody.AddImpulseVelocity(ref vector);
						}
					}
				}
				return this.mBarrierType == Barrier.BarrierType.SOLID & iPrim0 == 0;
			}
			return false;
		}

		// Token: 0x17000959 RID: 2393
		// (get) Token: 0x0600280C RID: 10252 RVA: 0x00125871 File Offset: 0x00123A71
		public override bool Removable
		{
			get
			{
				return this.Dead;
			}
		}

		// Token: 0x0600280D RID: 10253 RVA: 0x00125879 File Offset: 0x00123A79
		public override void Kill()
		{
		}

		// Token: 0x04002B96 RID: 11158
		private static List<WaveEntity> mWaveCache;

		// Token: 0x04002B97 RID: 11159
		private Wave mParent;

		// Token: 0x04002B98 RID: 11160
		private float mAnimationTimer;

		// Token: 0x04002B99 RID: 11161
		private static SkinnedModel[] sWaveBarrierModels;

		// Token: 0x04002B9A RID: 11162
		private static AnimationClip[] sWaveAppearClips;
	}
}
