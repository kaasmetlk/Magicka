using System;
using JigLibX.Geometry;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000254 RID: 596
	public class SummonPhoenix : SpecialAbility, IAbilityEffect
	{
		// Token: 0x170004B8 RID: 1208
		// (get) Token: 0x0600125A RID: 4698 RVA: 0x00070A50 File Offset: 0x0006EC50
		public static SummonPhoenix Instance
		{
			get
			{
				if (SummonPhoenix.mSingelton == null)
				{
					lock (SummonPhoenix.mSingeltonLock)
					{
						if (SummonPhoenix.mSingelton == null)
						{
							SummonPhoenix.mSingelton = new SummonPhoenix();
						}
					}
				}
				return SummonPhoenix.mSingelton;
			}
		}

		// Token: 0x0600125B RID: 4699 RVA: 0x00070AA4 File Offset: 0x0006ECA4
		static SummonPhoenix()
		{
			lock (Game.Instance.GraphicsDevice)
			{
				SummonPhoenix.sModel = Game.Instance.Content.Load<SkinnedModel>("Models/Magicks/Phoenix");
			}
			SummonPhoenix.mAudioEmitter = new AudioEmitter();
			SummonPhoenix.sController = new AnimationController();
			SummonPhoenix.sController.Skeleton = SummonPhoenix.sModel.SkeletonBones;
			SummonPhoenix.sClip = SummonPhoenix.sModel.AnimationClips["descend"];
			ModelMesh modelMesh = SummonPhoenix.sModel.Model.Meshes[0];
			ModelMeshPart iPart = modelMesh.MeshParts[0];
			SummonPhoenix.sBoundingSphere = modelMesh.BoundingSphere;
			SummonPhoenix.mRenderData = new SummonPhoenix.RenderData[3];
			SummonPhoenix.mRenderData[0] = new SummonPhoenix.RenderData();
			SummonPhoenix.mRenderData[0].mBones = new Matrix[80];
			SummonPhoenix.mRenderData[0].SetMesh(modelMesh, iPart, 3, 0, 4);
			SummonPhoenix.mRenderData[1] = new SummonPhoenix.RenderData();
			SummonPhoenix.mRenderData[1].mBones = new Matrix[80];
			SummonPhoenix.mRenderData[1].SetMesh(modelMesh, iPart, 3, 0, 4);
			SummonPhoenix.mRenderData[2] = new SummonPhoenix.RenderData();
			SummonPhoenix.mRenderData[2].mBones = new Matrix[80];
			SummonPhoenix.mRenderData[2].SetMesh(modelMesh, iPart, 3, 0, 4);
			SummonPhoenix.sDamage = default(DamageCollection5);
			SummonPhoenix.sDamage.A = new Damage(AttackProperties.Damage, Elements.Fire, 400f, 1f);
			SummonPhoenix.sDamage.B = new Damage(AttackProperties.Status, Elements.Fire, 200f, 5f);
			SummonPhoenix.sDamage.C = new Damage(AttackProperties.Knockdown, Elements.Fire, 400f, 2f);
		}

		// Token: 0x0600125C RID: 4700 RVA: 0x00070CAC File Offset: 0x0006EEAC
		private SummonPhoenix() : base(Animations.cast_magick_direct, "#magick_sphoenix".GetHashCodeCustom())
		{
		}

		// Token: 0x0600125D RID: 4701 RVA: 0x00070CC0 File Offset: 0x0006EEC0
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			SummonPhoenix.sPlayState = iPlayState;
			if (SummonPhoenix.sPlayState.Level.CurrentScene.Indoors)
			{
				AudioManager.Instance.PlayCue(Banks.Spells, SummonPhoenix.SOUND_FAIL_HASH, SummonPhoenix.mAudioEmitter);
				SummonPhoenix.sCastFail = true;
				SummonPhoenix.sFinished = false;
				SummonPhoenix.sFailTTL = 1.5f;
				return true;
			}
			if (SummonPhoenix.sFinished)
			{
				SummonPhoenix.sCastFail = false;
				this.mOwner = null;
				SummonPhoenix.sFinished = false;
				SummonPhoenix.sController.StartClip(SummonPhoenix.sClip, false);
				Vector3 vector = iPosition;
				Vector3 vector2 = new Vector3((float)(SpecialAbility.RANDOM.NextDouble() - 0.5) * 3f, 0f, (float)(SpecialAbility.RANDOM.NextDouble() - 0.5) * 3f);
				Vector3.Add(ref vector, ref vector2, out vector);
				Vector3 vector3;
				SummonPhoenix.sPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref vector, out vector3, MovementProperties.Default);
				vector = vector3;
				Segment iSeg = default(Segment);
				iSeg.Origin = vector;
				iSeg.Origin.Y = iSeg.Origin.Y + 1f;
				iSeg.Delta.Y = -3f;
				float num;
				Vector3 vector4;
				if (SummonPhoenix.sPlayState.Level.CurrentScene.SegmentIntersect(out num, out vector3, out vector4, iSeg))
				{
					vector = vector3;
					vector.Y += 1f;
				}
				Matrix.CreateRotationY(4.712389f, out SummonPhoenix.sMatrix);
				SummonPhoenix.sMatrix.Translation = vector;
				SummonPhoenix.mAudioEmitter.Position = vector;
				SummonPhoenix.mAudioEmitter.Up = Vector3.Up;
				SummonPhoenix.mAudioEmitter.Forward = Vector3.Right;
				AudioManager.Instance.PlayCue(Banks.Spells, SummonPhoenix.SOUND_HASH, SummonPhoenix.mAudioEmitter);
				SpellManager.Instance.AddSpellEffect(this);
				return true;
			}
			return false;
		}

		// Token: 0x0600125E RID: 4702 RVA: 0x00070E90 File Offset: 0x0006F090
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			SummonPhoenix.sPlayState = iPlayState;
			Vector3 position = iOwner.Position;
			if (SummonPhoenix.sPlayState.Level.CurrentScene.Indoors)
			{
				AudioManager.Instance.PlayCue(Banks.Spells, SummonPhoenix.SOUND_FAIL_HASH, SummonPhoenix.mAudioEmitter);
				SummonPhoenix.sCastFail = true;
				SummonPhoenix.sFinished = false;
				SummonPhoenix.sFailTTL = 1.5f;
				return true;
			}
			if (SummonPhoenix.sFinished)
			{
				SummonPhoenix.sCastFail = false;
				this.mOwner = iOwner;
				SummonPhoenix.sFinished = false;
				SummonPhoenix.sController.StartClip(SummonPhoenix.sClip, false);
				Vector3 vector = position;
				Vector3 vector2 = new Vector3((float)(SpecialAbility.RANDOM.NextDouble() - 0.5) * 4f, 0f, (float)(SpecialAbility.RANDOM.NextDouble() - 0.5) * 4f);
				Vector3.Add(ref vector, ref vector2, out vector);
				Vector3 vector3;
				SummonPhoenix.sPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref vector, out vector3, MovementProperties.Default);
				vector = vector3;
				Segment iSeg = default(Segment);
				iSeg.Origin = vector;
				iSeg.Origin.Y = iSeg.Origin.Y + 1f;
				iSeg.Delta.Y = -3f;
				float num;
				Vector3 vector4;
				if (SummonPhoenix.sPlayState.Level.CurrentScene.SegmentIntersect(out num, out vector3, out vector4, iSeg))
				{
					vector = vector3;
				}
				Matrix.CreateRotationY(4.712389f, out SummonPhoenix.sMatrix);
				SummonPhoenix.sMatrix.Translation = vector;
				SummonPhoenix.mAudioEmitter.Position = vector;
				SummonPhoenix.mAudioEmitter.Up = Vector3.Up;
				SummonPhoenix.mAudioEmitter.Forward = Vector3.Right;
				AudioManager.Instance.PlayCue(Banks.Spells, SummonPhoenix.SOUND_HASH, SummonPhoenix.mAudioEmitter);
				SpellManager.Instance.AddSpellEffect(this);
			}
			return true;
		}

		// Token: 0x170004B9 RID: 1209
		// (get) Token: 0x0600125F RID: 4703 RVA: 0x00071059 File Offset: 0x0006F259
		public bool IsDead
		{
			get
			{
				return SummonPhoenix.sFinished;
			}
		}

		// Token: 0x06001260 RID: 4704 RVA: 0x00071060 File Offset: 0x0006F260
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (SummonPhoenix.sCastFail)
			{
				SummonPhoenix.sFailTTL -= iDeltaTime;
				if (SummonPhoenix.sFailTTL <= 0f)
				{
					SummonPhoenix.sPlayState.Camera.CameraShake(4f, 0.2f);
					SummonPhoenix.sFinished = true;
					return;
				}
			}
			else
			{
				SummonPhoenix.mAudioEmitter.Position = SummonPhoenix.sMatrix.Translation;
				SummonPhoenix.mAudioEmitter.Up = Vector3.Up;
				SummonPhoenix.mAudioEmitter.Forward = Vector3.Right;
				SummonPhoenix.sBoundingSphere.Center = SummonPhoenix.mAudioEmitter.Position;
				SummonPhoenix.mRenderData[(int)iDataChannel].mBoundingSphere = SummonPhoenix.sBoundingSphere;
				SummonPhoenix.sController.Update(iDeltaTime, ref SummonPhoenix.sMatrix, true);
				SummonPhoenix.sController.SkinnedBoneTransforms.CopyTo(SummonPhoenix.mRenderData[(int)iDataChannel].mBones, 0);
				SummonPhoenix.sPlayState.Scene.AddRenderableObject(iDataChannel, SummonPhoenix.mRenderData[(int)iDataChannel]);
				if (SummonPhoenix.sController.HasFinished && !SummonPhoenix.sFinished)
				{
					Vector3 vector = SummonPhoenix.sMatrix.Translation;
					vector.Y += 0.5f;
					Vector3 vector2 = Vector3.Right;
					Helper.CircleDamage(SummonPhoenix.sPlayState, this.mOwner as Entity, this.mTimeStamp, null, ref vector, 6f, ref SummonPhoenix.sDamage);
					SummonPhoenix.sPlayState.Camera.CameraShake(1f, 1f);
					VisualEffectReference visualEffectReference;
					EffectManager.Instance.StartEffect(SummonPhoenix.BLAST_EFFECT, ref vector, ref vector2, out visualEffectReference);
					AudioManager.Instance.PlayCue(Banks.Spells, "spell_fire_area".GetHashCodeCustom(), SummonPhoenix.mAudioEmitter);
					Player[] players = Game.Instance.Players;
					for (int i = 0; i < players.Length; i++)
					{
						if (NetworkManager.Instance.State != NetworkState.Client)
						{
							Matrix matrix;
							Matrix.CreateRotationY((float)i * 6.2831855f * 0.25f, out matrix);
							if (players[i].Playing && players[i].Avatar.Dead)
							{
								vector = SummonPhoenix.sMatrix.Translation;
								vector2 = matrix.Forward;
								Vector3.Add(ref vector2, ref vector, out vector);
								Vector3 vector3;
								SummonPhoenix.sPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref vector, out vector3, MovementProperties.Default);
								vector = vector3;
								Segment iSeg = default(Segment);
								iSeg.Origin = vector;
								iSeg.Origin.Y = iSeg.Origin.Y + 1f;
								iSeg.Delta.Y = -3f;
								float num;
								Vector3 vector4;
								if (SummonPhoenix.sPlayState.Level.CurrentScene.SegmentIntersect(out num, out vector3, out vector4, iSeg))
								{
									vector = vector3;
								}
								Revive instance = Revive.GetInstance();
								instance.SetSpecificPlayer(players[i].ID);
								instance.Execute(vector, SummonPhoenix.sPlayState);
							}
						}
					}
					SummonPhoenix.sFinished = true;
				}
			}
		}

		// Token: 0x06001261 RID: 4705 RVA: 0x0007132A File Offset: 0x0006F52A
		public void OnRemove()
		{
			SummonPhoenix.sFinished = true;
		}

		// Token: 0x04001115 RID: 4373
		private static SummonPhoenix mSingelton;

		// Token: 0x04001116 RID: 4374
		private static volatile object mSingeltonLock = new object();

		// Token: 0x04001117 RID: 4375
		private static SummonPhoenix.RenderData[] mRenderData;

		// Token: 0x04001118 RID: 4376
		private static SkinnedModel sModel;

		// Token: 0x04001119 RID: 4377
		private static AnimationController sController;

		// Token: 0x0400111A RID: 4378
		private static AnimationClip sClip;

		// Token: 0x0400111B RID: 4379
		private static AudioEmitter mAudioEmitter;

		// Token: 0x0400111C RID: 4380
		private static bool sFinished = true;

		// Token: 0x0400111D RID: 4381
		private static bool sCastFail;

		// Token: 0x0400111E RID: 4382
		private static float sFailTTL;

		// Token: 0x0400111F RID: 4383
		private static Matrix sMatrix;

		// Token: 0x04001120 RID: 4384
		private static PlayState sPlayState;

		// Token: 0x04001121 RID: 4385
		public static readonly int SOUND_FAIL_HASH = "magick_summon_phoenix_indoors".GetHashCodeCustom();

		// Token: 0x04001122 RID: 4386
		public static readonly int SOUND_HASH = "magick_summon_phoenix".GetHashCodeCustom();

		// Token: 0x04001123 RID: 4387
		public static readonly int REVIVE_EFFECT = "magick_revive".GetHashCodeCustom();

		// Token: 0x04001124 RID: 4388
		public static readonly int BLAST_EFFECT = "magick_phoenix_blast".GetHashCodeCustom();

		// Token: 0x04001125 RID: 4389
		private static DamageCollection5 sDamage;

		// Token: 0x04001126 RID: 4390
		private static BoundingSphere sBoundingSphere;

		// Token: 0x04001127 RID: 4391
		private ISpellCaster mOwner;

		// Token: 0x02000255 RID: 597
		public class RenderData : RenderableObject<SkinnedModelBasicEffect, MagickaSkinnedModelMaterial>
		{
			// Token: 0x06001262 RID: 4706 RVA: 0x00071334 File Offset: 0x0006F534
			public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelBasicEffect skinnedModelBasicEffect = iEffect as SkinnedModelBasicEffect;
				skinnedModelBasicEffect.Bones = this.mBones;
				base.Draw(iEffect, iViewFrustum);
			}

			// Token: 0x06001263 RID: 4707 RVA: 0x0007135C File Offset: 0x0006F55C
			public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelBasicEffect skinnedModelBasicEffect = iEffect as SkinnedModelBasicEffect;
				skinnedModelBasicEffect.Bones = this.mBones;
				base.DrawShadow(iEffect, iViewFrustum);
			}

			// Token: 0x04001128 RID: 4392
			public Matrix[] mBones;
		}
	}
}
