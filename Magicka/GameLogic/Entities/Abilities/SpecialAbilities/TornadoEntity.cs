using System;
using System.Collections.Generic;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Magicka.Network;
using Magicka.PathFinding;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x020005B4 RID: 1460
	internal class TornadoEntity : Entity
	{
		// Token: 0x06002BAB RID: 11179 RVA: 0x00158D3C File Offset: 0x00156F3C
		public static TornadoEntity GetInstance()
		{
			TornadoEntity tornadoEntity;
			lock (TornadoEntity.sCache)
			{
				tornadoEntity = TornadoEntity.sCache[0];
				TornadoEntity.sCache.RemoveAt(0);
				TornadoEntity.sCache.Add(tornadoEntity);
			}
			return tornadoEntity;
		}

		// Token: 0x06002BAC RID: 11180 RVA: 0x00158D94 File Offset: 0x00156F94
		public static TornadoEntity GetSpecificInstance(ushort iHandle)
		{
			TornadoEntity tornadoEntity;
			lock (TornadoEntity.sCache)
			{
				tornadoEntity = (Entity.GetFromHandle((int)iHandle) as TornadoEntity);
				TornadoEntity.sCache.Remove(tornadoEntity);
				TornadoEntity.sCache.Add(tornadoEntity);
			}
			return tornadoEntity;
		}

		// Token: 0x06002BAD RID: 11181 RVA: 0x00158DEC File Offset: 0x00156FEC
		public static void InitializeCache(int iNr)
		{
			TornadoEntity.sCache = new List<TornadoEntity>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				TornadoEntity.sCache.Add(new TornadoEntity(null));
			}
		}

		// Token: 0x06002BAE RID: 11182 RVA: 0x00158E20 File Offset: 0x00157020
		public TornadoEntity(PlayState iPlayState) : base(iPlayState)
		{
			this.mHitList = new HitList(32);
			SkinnedModel skinnedModel;
			SkinnedModel skinnedModel2;
			lock (Game.Instance.GraphicsDevice)
			{
				skinnedModel = Game.Instance.Content.Load<SkinnedModel>("Models/Magicks/Tornado");
				skinnedModel2 = Game.Instance.Content.Load<SkinnedModel>("Models/Magicks/Tornado_debri");
			}
			this.mController = new AnimationController();
			this.mController.Skeleton = skinnedModel.SkeletonBones;
			this.mClip = skinnedModel.AnimationClips["tornado"];
			this.mPath = new List<PathNode>(8);
			this.mBody = new Body();
			this.mCollision = new CollisionSkin(this.mBody);
			this.mCollision.AddPrimitive(new Capsule(default(Vector3), Matrix.CreateRotationX(-1.5707964f), TornadoEntity.RADIUS, TornadoEntity.LENGTH), 1, default(MaterialProperties));
			this.mCollision.callbackFn += this.OnCollision;
			this.mBody.CollisionSkin = this.mCollision;
			this.mBody.Immovable = true;
			this.mBody.Tag = this;
			this.mDamage = default(DamageCollection5);
			this.mDamage.AddDamage(new Damage(AttackProperties.Knockback, Elements.Earth, 800f, 2f));
			SkinnedModelBasicEffect iEffect = skinnedModel.Model.Meshes[0].MeshParts[0].Effect as SkinnedModelBasicEffect;
			SkinnedModelDeferredBasicMaterial mMaterial;
			Helper.SkinnedModelDeferredMaterialFromBasicEffect(iEffect, out mMaterial);
			SkinnedModelBasicEffect iEffect2 = skinnedModel.Model.Meshes[0].MeshParts[0].Effect as SkinnedModelBasicEffect;
			SkinnedModelDeferredBasicMaterial mMaterial2;
			Helper.SkinnedModelDeferredMaterialFromBasicEffect(iEffect2, out mMaterial2);
			this.mRenderData = new TornadoEntity.DeferredRenderData[3];
			this.mDebriRenderData = new TornadoEntity.DeferredRenderData[3];
			for (int i = 0; i < 3; i++)
			{
				this.mRenderData[i] = new TornadoEntity.DeferredRenderData();
				this.mRenderData[i].SetMesh(skinnedModel.Model.Meshes[0].VertexBuffer, skinnedModel.Model.Meshes[0].IndexBuffer, skinnedModel.Model.Meshes[0].MeshParts[0], 2);
				this.mRenderData[i].mMaterial = mMaterial;
				this.mDebriRenderData[i] = new TornadoEntity.DeferredRenderData();
				this.mDebriRenderData[i].SetMesh(skinnedModel2.Model.Meshes[0].VertexBuffer, skinnedModel2.Model.Meshes[0].IndexBuffer, skinnedModel2.Model.Meshes[0].MeshParts[0], 2);
				this.mDebriRenderData[i].mMaterial = mMaterial2;
			}
		}

		// Token: 0x06002BAF RID: 11183 RVA: 0x0015910C File Offset: 0x0015730C
		public void Initialize(PlayState iPlayState, Matrix iOrientation, ISpellCaster iOwner)
		{
			if (this.mAmbience != null && !this.mAmbience.IsStopping)
			{
				this.mAmbience.Stop(AudioStopOptions.AsAuthored);
			}
			EffectManager.Instance.Stop(ref this.mDustEffect);
			this.mOwner = iOwner;
			this.mPlayState = iPlayState;
			this.mTimeStamp = this.mPlayState.PlayTime;
			this.mPath.Clear();
			this.mHeading = iOrientation.Forward;
			this.mController.StartClip(this.mClip, true);
			this.mController.Speed = 4f;
			this.mTTL = 15f;
			this.mAmbience = AudioManager.Instance.PlayCue(Banks.Spells, Tornado.AMBIENCE, base.AudioEmitter);
			base.Initialize();
			this.mBody.MoveTo(iOrientation.Translation, iOrientation);
			this.mAlpha = 0f;
		}

		// Token: 0x06002BB0 RID: 11184 RVA: 0x001591EF File Offset: 0x001573EF
		public override void Deinitialize()
		{
			base.Deinitialize();
			if (this.mAmbience != null && !this.mAmbience.IsStopping)
			{
				this.mAmbience.Stop(AudioStopOptions.AsAuthored);
			}
			EffectManager.Instance.Stop(ref this.mDustEffect);
		}

		// Token: 0x06002BB1 RID: 11185 RVA: 0x00159228 File Offset: 0x00157428
		public bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
		{
			if (iSkin1.Owner != null)
			{
				if (iSkin1.Owner.Tag is MissileEntity)
				{
					return false;
				}
				IDamageable damageable = iSkin1.Owner.Tag as IDamageable;
				if (damageable != null && !this.mHitList.ContainsKey(damageable.Handle))
				{
					Vector3 position = damageable.Position;
					Vector3 position2 = this.Position;
					Vector3 vector;
					Vector3.Subtract(ref position, ref position2, out vector);
					vector.Normalize();
					Vector3.Multiply(ref vector, TornadoEntity.RADIUS, out vector);
					Vector3.Add(ref position2, ref vector, out position2);
					Matrix identity = Matrix.Identity;
					identity.Translation = position2;
					VisualEffectReference visualEffectReference;
					EffectManager.Instance.StartEffect(Tornado.HIT_EFFECT, ref identity, out visualEffectReference);
					damageable.Damage(this.mDamage, this.mOwner as Entity, this.mTimeStamp, position2);
					this.mHitList.Add(damageable.Handle, 1f);
				}
			}
			return false;
		}

		// Token: 0x17000A39 RID: 2617
		// (get) Token: 0x06002BB2 RID: 11186 RVA: 0x00159313 File Offset: 0x00157513
		public override bool Dead
		{
			get
			{
				return this.mTTL < 0f;
			}
		}

		// Token: 0x06002BB3 RID: 11187 RVA: 0x00159324 File Offset: 0x00157524
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			base.Update(iDataChannel, iDeltaTime);
			this.mAlpha = Math.Min(15f - this.mTTL, 0.5f) * Math.Min(this.mTTL, 1f) * 2f;
			this.mTTL -= iDeltaTime;
			this.mHitList.Update(iDeltaTime);
			Vector3 position = this.Position;
			if (this.mPath.Count > 0)
			{
				this.mTargetPosition = this.mPath[0].Position;
				float num;
				Vector3.DistanceSquared(ref position, ref this.mTargetPosition, out num);
				while (this.mPath.Count > 0)
				{
					if (num > 1f)
					{
						break;
					}
					this.mTargetPosition = this.mPath[0].Position;
					this.mPath.RemoveAt(0);
					Vector3.DistanceSquared(ref position, ref this.mTargetPosition, out num);
				}
			}
			else
			{
				float epsilon = float.Epsilon;
				NavMesh navMesh = this.mPlayState.Level.CurrentScene.NavMesh;
				this.mPath.Clear();
				Vector2 vector = new Vector2(this.mHeading.X, this.mHeading.Z);
				vector.Normalize();
				float num2 = MagickaMath.Angle(vector);
				num2 += ((float)TornadoEntity.RANDOM.NextDouble() - 0.5f) * 1.5707964f;
				float num3 = 5f + (float)TornadoEntity.RANDOM.NextDouble() * 10f;
				this.mTargetPosition.Y = position.Y;
				this.mTargetPosition.X = position.X + (float)Math.Cos((double)num2) * num3;
				this.mTargetPosition.Z = position.Z + (float)Math.Sin((double)num2) * num3;
				if (navMesh.FindShortestPath(ref position, ref this.mTargetPosition, this.mPath, MovementProperties.Default))
				{
					this.mPath.RemoveAt(0);
					this.mTargetPosition = this.mPath[0].Position;
					this.mPath.RemoveAt(0);
					Vector3.DistanceSquared(ref position, ref this.mTargetPosition, out epsilon);
				}
			}
			Vector3 vector2;
			Vector3.Subtract(ref this.mTargetPosition, ref position, out vector2);
			float num4 = (float)Math.Abs(Math.Sin((double)this.mTTL) * 4.0);
			vector2.Normalize();
			this.mHeading = vector2;
			position.X += vector2.X * num4 * iDeltaTime;
			position.Z += vector2.Z * num4 * iDeltaTime;
			Segment iSeg = default(Segment);
			iSeg.Origin = position;
			iSeg.Delta.Y = iSeg.Delta.Y - 3f;
			float num5;
			Vector3 vector3;
			Vector3 vector4;
			if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out num5, out vector3, out vector4, iSeg))
			{
				position.Y += (vector3.Y - position.Y) * iDeltaTime;
			}
			Matrix orientation = this.mBody.Orientation;
			orientation.Translation = position;
			MagickaMath.UniformMatrixScale(ref orientation, 0.75f);
			this.mBody.MoveTo(position, Matrix.Identity);
			if (!EffectManager.Instance.UpdateOrientation(ref this.mDustEffect, ref orientation))
			{
				EffectManager.Instance.StartEffect(Tornado.EFFECT, ref orientation, out this.mDustEffect);
			}
			this.mController.Update(iDeltaTime, ref orientation, true);
			TornadoEntity.DeferredRenderData deferredRenderData = this.mRenderData[(int)iDataChannel];
			TornadoEntity.DeferredRenderData deferredRenderData2 = this.mDebriRenderData[(int)iDataChannel];
			deferredRenderData2.mMaterial.Alpha = (deferredRenderData.mMaterial.Alpha = this.mAlpha);
			deferredRenderData2.mBoundingSphere.Center = (deferredRenderData.mBoundingSphere.Center = position);
			deferredRenderData2.mBoundingSphere.Radius = (deferredRenderData.mBoundingSphere.Radius = 2f);
			this.mController.SkinnedBoneTransforms.CopyTo(deferredRenderData2.mBones, 0);
			this.mController.SkinnedBoneTransforms.CopyTo(deferredRenderData.mBones, 0);
		}

		// Token: 0x06002BB4 RID: 11188 RVA: 0x0015972C File Offset: 0x0015792C
		protected override void AddImpulseVelocity(ref Vector3 iVelocity)
		{
			base.AddImpulseVelocity(ref iVelocity);
			Vector3 position = this.Position;
			float epsilon = float.Epsilon;
			NavMesh navMesh = this.mPlayState.Level.CurrentScene.NavMesh;
			this.mPath.Clear();
			Vector2 vector = new Vector2(iVelocity.X, iVelocity.Z);
			vector.Normalize();
			float num = 10f;
			iVelocity.Normalize();
			this.mTargetPosition.Y = position.Y;
			this.mTargetPosition.X = position.X + iVelocity.X * num;
			this.mTargetPosition.Z = position.Z + iVelocity.Z * num;
			if (navMesh.FindShortestPath(ref position, ref this.mTargetPosition, this.mPath, MovementProperties.Default))
			{
				this.mPath.RemoveAt(0);
				this.mTargetPosition = this.mPath[0].Position;
				this.mPath.RemoveAt(0);
				Vector3.DistanceSquared(ref position, ref this.mTargetPosition, out epsilon);
			}
		}

		// Token: 0x17000A3A RID: 2618
		// (get) Token: 0x06002BB5 RID: 11189 RVA: 0x00159834 File Offset: 0x00157A34
		public override bool Removable
		{
			get
			{
				return this.Dead;
			}
		}

		// Token: 0x06002BB6 RID: 11190 RVA: 0x0015983C File Offset: 0x00157A3C
		public override void Kill()
		{
			this.mTTL = 1f;
		}

		// Token: 0x06002BB7 RID: 11191 RVA: 0x00159849 File Offset: 0x00157A49
		protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
		{
			oMsg = default(EntityUpdateMessage);
			oMsg.Features |= EntityFeatures.Position;
			oMsg.Position = this.Position;
		}

		// Token: 0x06002BB8 RID: 11192 RVA: 0x0015986D File Offset: 0x00157A6D
		internal override float GetDanger()
		{
			return 10f * this.mAlpha;
		}

		// Token: 0x04002F5D RID: 12125
		private static List<TornadoEntity> sCache;

		// Token: 0x04002F5E RID: 12126
		private static readonly float RADIUS = 2.5f;

		// Token: 0x04002F5F RID: 12127
		private static readonly float LENGTH = 6f;

		// Token: 0x04002F60 RID: 12128
		private static readonly Random RANDOM = new Random();

		// Token: 0x04002F61 RID: 12129
		private float mTTL;

		// Token: 0x04002F62 RID: 12130
		private TornadoEntity.DeferredRenderData[] mRenderData;

		// Token: 0x04002F63 RID: 12131
		private TornadoEntity.DeferredRenderData[] mDebriRenderData;

		// Token: 0x04002F64 RID: 12132
		private AnimationController mController;

		// Token: 0x04002F65 RID: 12133
		private AnimationClip mClip;

		// Token: 0x04002F66 RID: 12134
		private ISpellCaster mOwner;

		// Token: 0x04002F67 RID: 12135
		private Vector3 mHeading;

		// Token: 0x04002F68 RID: 12136
		private VisualEffectReference mDustEffect;

		// Token: 0x04002F69 RID: 12137
		private HitList mHitList;

		// Token: 0x04002F6A RID: 12138
		private List<PathNode> mPath;

		// Token: 0x04002F6B RID: 12139
		private Vector3 mTargetPosition;

		// Token: 0x04002F6C RID: 12140
		private DamageCollection5 mDamage;

		// Token: 0x04002F6D RID: 12141
		private Cue mAmbience;

		// Token: 0x04002F6E RID: 12142
		private float mAlpha;

		// Token: 0x04002F6F RID: 12143
		private double mTimeStamp;

		// Token: 0x020005B5 RID: 1461
		protected class DeferredRenderData : RenderableAdditiveObject<SkinnedModelDeferredEffect, SkinnedModelDeferredBasicMaterial>
		{
			// Token: 0x06002BBA RID: 11194 RVA: 0x0015989B File Offset: 0x00157A9B
			public DeferredRenderData()
			{
				this.mBones = new Matrix[80];
			}

			// Token: 0x06002BBB RID: 11195 RVA: 0x001598B0 File Offset: 0x00157AB0
			public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				skinnedModelDeferredEffect.Bones = this.mBones;
				base.Draw(iEffect, iViewFrustum);
			}

			// Token: 0x04002F70 RID: 12144
			public Matrix[] mBones;
		}
	}
}
