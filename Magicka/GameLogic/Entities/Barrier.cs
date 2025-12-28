using System;
using System.Collections.Generic;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
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
using PolygonHead.Effects;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

namespace Magicka.GameLogic.Entities
{
	// Token: 0x020002F4 RID: 756
	public class Barrier : Entity, IDamageable
	{
		// Token: 0x0600174D RID: 5965 RVA: 0x00097318 File Offset: 0x00095518
		public static void InitializeCache(int iNrOfBarriers, PlayState iPlayState)
		{
			Barrier.mCache = new List<Barrier>(iNrOfBarriers);
			for (int i = 0; i < iNrOfBarriers; i++)
			{
				Barrier.mCache.Add(new Barrier(iPlayState));
			}
		}

		// Token: 0x0600174E RID: 5966 RVA: 0x0009734C File Offset: 0x0009554C
		public static Barrier GetFromCache(PlayState iPlayState)
		{
			if (Barrier.mCache.Count > 0)
			{
				Barrier result = Barrier.mCache[Barrier.mCache.Count - 1];
				Barrier.mCache.RemoveAt(Barrier.mCache.Count - 1);
				return result;
			}
			return new Barrier(iPlayState);
		}

		// Token: 0x0600174F RID: 5967 RVA: 0x0009739B File Offset: 0x0009559B
		public static void ReturnToCache(Barrier iBarrier)
		{
			if (!Barrier.mCache.Contains(iBarrier))
			{
				Barrier.mCache.Add(iBarrier);
			}
		}

		// Token: 0x06001750 RID: 5968 RVA: 0x000973B8 File Offset: 0x000955B8
		static Barrier()
		{
			Barrier.BARRIER_EFFECTS = new int[10];
			for (int i = 0; i < Barrier.BARRIER_EFFECTS.Length; i++)
			{
				Elements elements = Spell.ElementFromIndex(i);
				if (elements <= Elements.Arcane)
				{
					if (elements <= Elements.Fire)
					{
						switch (elements)
						{
						case Elements.Earth:
							Barrier.BARRIER_EFFECTS[i] = "barrier_earth1_b".GetHashCodeCustom();
							break;
						case Elements.Water:
							Barrier.BARRIER_EFFECTS[i] = "barrier_water1_b".GetHashCodeCustom();
							break;
						case Elements.Earth | Elements.Water:
							break;
						case Elements.Cold:
							Barrier.BARRIER_EFFECTS[i] = "barrier_cold1_b".GetHashCodeCustom();
							break;
						default:
							if (elements == Elements.Fire)
							{
								Barrier.BARRIER_EFFECTS[i] = "barrier_fire1_b".GetHashCodeCustom();
							}
							break;
						}
					}
					else if (elements != Elements.Lightning)
					{
						if (elements == Elements.Arcane)
						{
							Barrier.BARRIER_EFFECTS[i] = "barrier_arcane1_b".GetHashCodeCustom();
						}
					}
					else
					{
						Barrier.BARRIER_EFFECTS[i] = "barrier_lightning1_b".GetHashCodeCustom();
					}
				}
				else if (elements <= Elements.Shield)
				{
					if (elements != Elements.Life)
					{
						if (elements == Elements.Shield)
						{
							Barrier.BARRIER_EFFECTS[i] = 0;
						}
					}
					else
					{
						Barrier.BARRIER_EFFECTS[i] = "barrier_life1_b".GetHashCodeCustom();
					}
				}
				else if (elements != Elements.Ice)
				{
					if (elements == Elements.Steam)
					{
						Barrier.BARRIER_EFFECTS[i] = "barrier_steam1_b".GetHashCodeCustom();
					}
				}
				else
				{
					Barrier.BARRIER_EFFECTS[i] = "barrier_ice1_b".GetHashCodeCustom();
				}
			}
		}

		// Token: 0x06001751 RID: 5969 RVA: 0x000976E2 File Offset: 0x000958E2
		public static float GetRadius(bool iPhysical)
		{
			if (iPhysical)
			{
				return 1.25f;
			}
			return 0.75f;
		}

		// Token: 0x06001752 RID: 5970 RVA: 0x000976F4 File Offset: 0x000958F4
		protected Barrier(PlayState iPlayState) : base(iPlayState)
		{
			if (Barrier.sIceBarrierModels == null)
			{
				Barrier.sIceBarrierModels = new SkinnedModel[1];
				SkinnedModel skinnedModel;
				lock (Game.Instance.GraphicsDevice)
				{
					Barrier.sIceBarrierModels[0] = Game.Instance.Content.Load<SkinnedModel>("Models/Effects/iceBarrier0_mesh");
					skinnedModel = Game.Instance.Content.Load<SkinnedModel>("Models/Effects/iceBarrier_animation");
				}
				Barrier.sIceAppearClips = new AnimationClip[skinnedModel.AnimationClips.Count];
				int num = 0;
				foreach (AnimationClip animationClip in skinnedModel.AnimationClips.Values)
				{
					Barrier.sIceAppearClips[num++] = animationClip;
				}
				Barrier.sIceBarrierGeyserModels = new SkinnedModel[1];
				SkinnedModel skinnedModel2;
				lock (Game.Instance.GraphicsDevice)
				{
					Barrier.sIceBarrierGeyserModels[0] = Game.Instance.Content.Load<SkinnedModel>("Models/Effects/icebarrier_geyser");
					foreach (SkinnedModelBone skinnedModelBone in Barrier.sIceBarrierGeyserModels[0].SkeletonBones)
					{
						if (skinnedModelBone.Name.Equals("effect0", StringComparison.OrdinalIgnoreCase))
						{
							Barrier.sGeyserAttach.mIndex = (int)skinnedModelBone.Index;
							Barrier.sGeyserAttach.mBindPose = Matrix.CreateRotationY(3.1415927f) * Matrix.Invert(skinnedModelBone.InverseBindPoseTransform);
						}
					}
					skinnedModel2 = Game.Instance.Content.Load<SkinnedModel>("Models/Effects/iceBarrier_geyser");
				}
				Barrier.sIceGeyserAppearClips = new AnimationClip[skinnedModel2.AnimationClips.Count];
				num = 0;
				foreach (AnimationClip animationClip2 in skinnedModel2.AnimationClips.Values)
				{
					Barrier.sIceGeyserAppearClips[num++] = animationClip2;
				}
				Barrier.sIceBarrierTeslaModels = new SkinnedModel[1];
				SkinnedModel skinnedModel3;
				lock (Game.Instance.GraphicsDevice)
				{
					Barrier.sIceBarrierTeslaModels[0] = Game.Instance.Content.Load<SkinnedModel>("Models/Effects/icebarrier_tesla");
					foreach (SkinnedModelBone skinnedModelBone2 in Barrier.sIceBarrierTeslaModels[0].SkeletonBones)
					{
						if (skinnedModelBone2.Name.Equals("effect0", StringComparison.OrdinalIgnoreCase))
						{
							Barrier.sTeslaAttach.mIndex = (int)skinnedModelBone2.Index;
							Barrier.sTeslaAttach.mBindPose = Matrix.CreateRotationY(3.1415927f) * Matrix.Invert(skinnedModelBone2.InverseBindPoseTransform);
						}
					}
					skinnedModel3 = Game.Instance.Content.Load<SkinnedModel>("Models/Effects/iceBarrier_tesla");
				}
				Barrier.sIceTeslaAppearClips = new AnimationClip[skinnedModel3.AnimationClips.Count];
				num = 0;
				foreach (AnimationClip animationClip3 in skinnedModel3.AnimationClips.Values)
				{
					Barrier.sIceTeslaAppearClips[num++] = animationClip3;
				}
				Barrier.sIceBarrierEarthModels = new SkinnedModel[1];
				SkinnedModel skinnedModel4;
				lock (Game.Instance.GraphicsDevice)
				{
					Barrier.sIceBarrierEarthModels[0] = Game.Instance.Content.Load<SkinnedModel>("Models/Effects/iceBarrier_Earth");
					skinnedModel4 = Game.Instance.Content.Load<SkinnedModel>("Models/Effects/iceBarrier_Earth");
				}
				Barrier.sIceEarthAppearClips = new AnimationClip[skinnedModel4.AnimationClips.Count];
				num = 0;
				foreach (AnimationClip animationClip4 in skinnedModel4.AnimationClips.Values)
				{
					Barrier.sIceEarthAppearClips[num++] = animationClip4;
				}
				SkinnedModel skinnedModel5;
				lock (Game.Instance.GraphicsDevice)
				{
					Barrier.sEarthBarrierModels = new SkinnedModel[5];
					Barrier.sEarthBarrierModels[0] = Game.Instance.Content.Load<SkinnedModel>("Models/Effects/earthBarrier0_mesh");
					Barrier.sEarthBarrierModels[1] = Game.Instance.Content.Load<SkinnedModel>("Models/Effects/earthBarrier1_mesh");
					Barrier.sEarthBarrierModels[2] = Game.Instance.Content.Load<SkinnedModel>("Models/Effects/earthBarrier2_mesh");
					Barrier.sEarthBarrierModels[3] = Game.Instance.Content.Load<SkinnedModel>("Models/Effects/earthBarrier3_mesh");
					Barrier.sEarthBarrierModels[4] = Game.Instance.Content.Load<SkinnedModel>("Models/Effects/earthBarrier4_mesh");
					skinnedModel5 = Game.Instance.Content.Load<SkinnedModel>("Models/Effects/earthBarrier_animation");
				}
				Barrier.sEarthAppearClips = new AnimationClip[skinnedModel5.AnimationClips.Count];
				num = 0;
				foreach (AnimationClip animationClip5 in skinnedModel5.AnimationClips.Values)
				{
					Barrier.sEarthAppearClips[num++] = animationClip5;
				}
				SkinnedModel skinnedModel6;
				lock (Game.Instance.GraphicsDevice)
				{
					Barrier.sEarthVulcanoBarrierModels = new SkinnedModel[1];
					Barrier.sEarthVulcanoBarrierModels[0] = Game.Instance.Content.Load<SkinnedModel>("Models/Effects/earthBarrier_volcano");
					foreach (SkinnedModelBone skinnedModelBone3 in Barrier.sEarthVulcanoBarrierModels[0].SkeletonBones)
					{
						if (skinnedModelBone3.Name.Equals("effect0", StringComparison.OrdinalIgnoreCase))
						{
							Barrier.sVulcanoAttach.mIndex = (int)skinnedModelBone3.Index;
							Barrier.sVulcanoAttach.mBindPose = Matrix.CreateRotationY(3.1415927f) * Matrix.Invert(skinnedModelBone3.InverseBindPoseTransform);
						}
					}
					skinnedModel6 = Game.Instance.Content.Load<SkinnedModel>("Models/Effects/earthBarrier_volcano");
				}
				Barrier.sEarthVulcanoAppearClips = new AnimationClip[skinnedModel6.AnimationClips.Count];
				num = 0;
				foreach (AnimationClip animationClip6 in skinnedModel6.AnimationClips.Values)
				{
					Barrier.sEarthVulcanoAppearClips[num++] = animationClip6;
				}
				lock (Game.Instance.GraphicsDevice)
				{
					Barrier.sRunesArcane = Game.Instance.Content.Load<Model>("Models/Effects/runesArcane");
				}
				lock (Game.Instance.GraphicsDevice)
				{
					Barrier.sRunesLife = Game.Instance.Content.Load<Model>("Models/Effects/runesLife");
				}
			}
			this.mIceAnimationController = new AnimationController();
			this.mIceAnimationController.ClipSpeed = 2f;
			this.mEarthAnimationController = new AnimationController();
			this.mEarthAnimationController.ClipSpeed = 2f;
			this.mBody = new Body();
			this.mCollision = new CollisionSkin(this.mBody);
			this.mCollision.AddPrimitive(new Capsule(Vector3.Down, Matrix.CreateRotationX(1.5707964f), Barrier.GetRadius(true), 0.1f), 1, new MaterialProperties(0.2f, 0.8f, 0.8f));
			this.mCollision.AddPrimitive(new Sphere(default(Vector3), Barrier.GetRadius(true)), 1, new MaterialProperties(0.2f, 0.8f, 0.8f));
			this.mCollision.callbackFn += this.OnCollision;
			this.mCollision.postCollisionCallbackFn += this.PostCollision;
			this.mBody.CollisionSkin = this.mCollision;
			this.mBody.ApplyGravity = false;
			this.mBody.AllowFreezing = false;
			this.mBody.Tag = this;
			this.mInitilizeDamage = true;
			this.mResistances = new Resistance[Defines.DamageTypeIndex(AttackProperties.NumberOfTypes)];
			for (int i = 0; i < this.mResistances.Length; i++)
			{
				this.mResistances[i].Multiplier = 1f;
				this.mResistances[i].Modifier = 0f;
				this.mResistances[i].ResistanceAgainst = Defines.ElementFromIndex(i);
			}
			this.mIceRenderData = new Barrier.RenderData[3];
			this.mEarthRenderData = new Barrier.RenderData[3];
			this.mRuneRenderData = new RenderableAdditiveObject<AdditiveEffect, AdditiveMaterial>[3];
			for (int j = 0; j < 3; j++)
			{
				this.mIceRenderData[j] = new Barrier.RenderData();
				this.mEarthRenderData[j] = new Barrier.RenderData();
				this.mRuneRenderData[j] = new RenderableAdditiveObject<AdditiveEffect, AdditiveMaterial>();
			}
		}

		// Token: 0x06001753 RID: 5971 RVA: 0x00098078 File Offset: 0x00096278
		protected virtual bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
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
					if ((this.mEffectTTL > 0f | !this.Solid) && !this.mHitList.HitList.Contains(damageable))
					{
						if ((this.mSpell.Element & Elements.Lightning) != Elements.None)
						{
							Vector3 iSource = this.mEffectAttach.mBindPose.Translation;
							if (!this.Solid)
							{
								iSource = this.Position;
								iSource.Y += 1f;
							}
							else
							{
								Vector3.Transform(ref iSource, ref this.mIceAnimationController.SkinnedBoneTransforms[this.mEffectAttach.mIndex], out iSource);
							}
							LightningBolt lightning = LightningBolt.GetLightning();
							lightning.Cast(this.mOwner, iSource, damageable as Entity, this.mHitList.HitList, Vector3.One, 1f, 1f, ref this.mDamage, this.mPlayState);
							if (!this.mHitList.HitList.ContainsKey(damageable.Handle))
							{
								this.mHitList.HitList.Add(damageable.Handle, 0.25f);
							}
						}
						else
						{
							damageable.Damage(this.mDamage, this.Owner as Entity, this.mTimeStamp, this.Position);
							this.mHitList.HitList.Add(damageable.Handle, 0.25f);
						}
					}
				}
				return this.mBarrierType == Barrier.BarrierType.SOLID & iPrim0 == 0;
			}
			return false;
		}

		// Token: 0x06001754 RID: 5972 RVA: 0x0009824B File Offset: 0x0009644B
		private void PostCollision(ref CollisionInfo iInfo)
		{
			if (iInfo.SkinInfo.Skin0 == this.mCollision)
			{
				iInfo.SkinInfo.IgnoreSkin0 = true;
				return;
			}
			iInfo.SkinInfo.IgnoreSkin1 = true;
		}

		// Token: 0x06001755 RID: 5973 RVA: 0x0009827C File Offset: 0x0009647C
		public override Matrix GetOrientation()
		{
			Vector3 position = this.mBody.Position;
			Matrix orientation = this.mBody.Orientation;
			Vector3 down = orientation.Down;
			Vector3.Multiply(ref down, 0.05f + Barrier.GetRadius(this.Solid), out down);
			Vector3.Add(ref position, ref down, out position);
			orientation.Translation = position;
			return orientation;
		}

		// Token: 0x06001756 RID: 5974 RVA: 0x000982D8 File Offset: 0x000964D8
		public void Initialize(ISpellCaster iOwner, Vector3 iPosition, Vector3 iDirection, float iScale, float iRange, Vector3 iNextDirection, Quaternion iNextRotation, float iDistanceBetweenBarriers, ref Spell iSpell, ref DamageCollection5 iDamage, Barrier.HitListWithBarriers iHitList, AnimatedLevelPart iAnimation)
		{
			if (this.mSoundCue != null && !this.mSoundCue.IsStopping)
			{
				this.mSoundCue.Stop(AudioStopOptions.AsAuthored);
			}
			this.mEarthAnimationController.Stop();
			this.mIceAnimationController.Stop();
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
			if (iAnimation != null)
			{
				iAnimation.AddEntity(this);
			}
			this.mSpell = iSpell;
			this.mHitList = iHitList;
			this.mHitList.Owners.Add(this);
			this.mDirection = iDirection;
			this.mIceAnimationController.PlaybackMode = PlaybackMode.Forward;
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
				if ((iSpell.Element & Elements.Life) == Elements.Life)
				{
					this.mRuneModel = Barrier.sRunesLife;
				}
				else if ((iSpell.Element & Elements.Arcane) == Elements.Arcane)
				{
					this.mRuneModel = Barrier.sRunesArcane;
				}
				else
				{
					this.mRuneModel = null;
				}
				if (this.mRuneModel != null)
				{
					Spell spell = iSpell;
					spell.EarthMagnitude = 0f;
					spell.IceMagnitude = 0f;
					spell.ShieldMagnitude = 0f;
					spell.Element &= ~(Elements.Earth | Elements.Shield | Elements.Ice);
					Vector3 color = spell.GetColor();
					Vector4 colorTint = default(Vector4);
					colorTint.X = color.X;
					colorTint.Y = color.Y;
					colorTint.Z = color.Z;
					colorTint.W = 1f;
					for (int i = 0; i < this.mRuneRenderData.Length; i++)
					{
						this.mRuneRenderData[i].SetMesh(this.mRuneModel.Meshes[0], this.mRuneModel.Meshes[0].MeshParts[0], 0);
						this.mRuneRenderData[i].mMaterial.ColorTint = colorTint;
					}
				}
				this.mSpell.CalculateDamage(SpellType.Projectile, CastType.Area, out iDamage2);
				if ((iSpell.Element & Elements.Ice) == Elements.Ice)
				{
					this.mDrawIce = true;
					this.mKillIce = false;
					Vector3 forward = this.Body.Orientation.Forward;
					EffectManager.Instance.StartEffect(Barrier.Ice_Barrier_Spawn_Effect_Hash, ref iPosition, ref forward, out this.mSpawnDeathEffectReference);
					SkinnedModel skinnedModel = Barrier.sIceBarrierModels[Barrier.mRandom.Next(Barrier.sIceBarrierModels.Length)];
					AnimationClip[] array = Barrier.sIceAppearClips;
					this.mEffectTTL = iSpell.IceMagnitude * 1f;
					if ((iSpell.Element & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Lightning | Elements.Steam | Elements.Poison)) != Elements.None && (iSpell.Element & Elements.Beams) == Elements.None)
					{
						this.mAddedEffect = true;
						skinnedModel = Barrier.sIceBarrierGeyserModels[Barrier.mRandom.Next(Barrier.sIceBarrierGeyserModels.Length)];
						array = Barrier.sIceGeyserAppearClips;
						this.mEffectAttach = Barrier.sGeyserAttach;
						if ((iSpell.Element & Elements.Lightning) == Elements.Lightning)
						{
							this.mEffectAttach = Barrier.sTeslaAttach;
							skinnedModel = Barrier.sIceBarrierTeslaModels[Barrier.mRandom.Next(Barrier.sIceBarrierTeslaModels.Length)];
							array = Barrier.sIceTeslaAppearClips;
							this.mEffectTTL = iSpell.LightningMagnitude * 1f;
						}
					}
					if ((iSpell.Element & Elements.Earth) != Elements.None)
					{
						skinnedModel = Barrier.sIceBarrierEarthModels[Barrier.mRandom.Next(Barrier.sIceBarrierEarthModels.Length)];
						array = Barrier.sIceEarthAppearClips;
						this.mAddedEffect = false;
						this.mIceEarthTTL = iSpell.IceMagnitude * 1f;
					}
					this.mIceBarrierTTL = this.mEffectTTL;
					ModelMesh modelMesh = skinnedModel.Model.Meshes[0];
					ModelMeshPart iMeshPart = modelMesh.MeshParts[0];
					this.mIceAnimationController.Skeleton = skinnedModel.SkeletonBones;
					if (this.mAddedEffect)
					{
						Elements elements2 = Elements.None;
						float num = -1f;
						for (int j = 0; j < 11; j++)
						{
							Elements elements3 = Defines.ElementFromIndex(j);
							if ((elements3 & Elements.Lightning) == Elements.Lightning && (this.mSpell.Element & Elements.Lightning) == Elements.Lightning)
							{
								elements2 = elements3;
								num = this.mSpell[elements2];
								j = 11;
							}
							if ((elements3 & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Lightning | Elements.Steam)) != Elements.None && this.mSpell[elements3] > num)
							{
								elements2 = elements3;
								num = this.mSpell[elements2];
							}
						}
						Matrix identity = Matrix.Identity;
						identity.Translation = iPosition;
						EffectManager.Instance.StartEffect(Barrier.BARRIER_GEYSER_EFFECTS[Defines.ElementIndex(elements2)], ref identity, out this.mEffect);
						if ((elements2 & Elements.Cold) == Elements.Cold)
						{
							this.mEffectTTL = this.mSpell.ColdMagnitude * 1f;
						}
						if ((elements2 & Elements.Fire) == Elements.Fire)
						{
							this.mEffectTTL = this.mSpell.FireMagnitude * 1f;
						}
						if ((elements2 & Elements.Poison) == Elements.Poison)
						{
							this.mEffectTTL = this.mSpell.PoisonMagnitude * 1f;
						}
						if ((elements2 & Elements.Steam) == Elements.Steam)
						{
							this.mEffectTTL = this.mSpell.SteamMagnitude * 1f;
						}
						if ((elements2 & Elements.Water) == Elements.Water)
						{
							this.mEffectTTL = this.mSpell.WaterMagnitude * 1f;
						}
					}
					float overlayAlpha = MathHelper.Clamp((iSpell.IceMagnitude - 1f) * 0.333333f, 0f, 1f);
					for (int k = 0; k < 3; k++)
					{
						Barrier.RenderData renderData = this.mIceRenderData[k];
						renderData.SetMesh(modelMesh.VertexBuffer, modelMesh.IndexBuffer, iMeshPart);
						renderData.mSkinnedModelMaterial.OverlayAlpha = overlayAlpha;
					}
					this.mIceAnimationController.StartClip(array[Barrier.mRandom.Next(array.Length)], false);
					this.mDrawMethod |= Barrier.DrawMethod.NORMAL;
				}
				if ((iSpell.Element & Elements.Earth) == Elements.Earth)
				{
					this.mSpell.CalculateDamage(SpellType.Projectile, CastType.Area, out iDamage2);
					Vector3 forward2 = this.Body.Orientation.Forward;
					EffectManager.Instance.StartEffect(Barrier.Earth_Barrier_Spawn_Effect_Hash, ref iPosition, ref forward2, out this.mSpawnDeathEffectReference);
					SkinnedModel skinnedModel2 = Barrier.sEarthBarrierModels[Barrier.mRandom.Next(Barrier.sIceBarrierModels.Length)];
					AnimationClip[] array2 = Barrier.sEarthAppearClips;
					if ((iSpell.Element & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Steam | Elements.Poison)) != Elements.None && (iSpell.Element & Elements.Beams) == Elements.None)
					{
						this.mAddedEffect = true;
						this.mEffectAttach = Barrier.sVulcanoAttach;
						skinnedModel2 = Barrier.sEarthVulcanoBarrierModels[Barrier.mRandom.Next(Barrier.sEarthVulcanoBarrierModels.Length)];
						array2 = Barrier.sEarthVulcanoAppearClips;
						Elements elements4 = Elements.None;
						float num2 = -1f;
						for (int l = 0; l < 11; l++)
						{
							Elements elements5 = Defines.ElementFromIndex(l);
							if ((elements5 & Elements.Lightning) == Elements.Lightning && (this.mSpell.Element & Elements.Lightning) == Elements.Lightning)
							{
								elements4 = elements5;
								num2 = this.mSpell[elements4];
								l = 11;
							}
							else if ((elements5 & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Lightning | Elements.Steam)) != Elements.None && this.mSpell[elements5] > num2)
							{
								elements4 = elements5;
								num2 = this.mSpell[elements4];
							}
						}
						if ((elements4 & Elements.Cold) == Elements.Cold)
						{
							this.mEffectTTL = this.mSpell.ColdMagnitude * 5f;
						}
						if ((elements4 & Elements.Fire) == Elements.Fire)
						{
							this.mEffectTTL = this.mSpell.FireMagnitude * 5f;
						}
						if ((elements4 & Elements.Poison) == Elements.Poison)
						{
							this.mEffectTTL = this.mSpell.PoisonMagnitude * 5f;
						}
						if ((elements4 & Elements.Steam) == Elements.Steam)
						{
							this.mEffectTTL = this.mSpell.SteamMagnitude * 5f;
						}
						if ((elements4 & Elements.Water) == Elements.Water)
						{
							this.mEffectTTL = this.mSpell.WaterMagnitude * 5f;
						}
						Matrix identity2 = Matrix.Identity;
						identity2.Translation = iPosition;
						EffectManager.Instance.StartEffect(Barrier.BARRIER_GEYSER_EFFECTS[Defines.ElementIndex(elements4)], ref identity2, out this.mEffect);
					}
					ModelMesh modelMesh2 = skinnedModel2.Model.Meshes[0];
					ModelMeshPart iMeshPart2 = modelMesh2.MeshParts[0];
					this.mEarthAnimationController.Skeleton = skinnedModel2.SkeletonBones;
					float overlayAlpha2 = MathHelper.Clamp((iSpell.EarthMagnitude - 1f) * 0.333333f, 0f, 1f);
					for (int m = 0; m < 3; m++)
					{
						Barrier.RenderData renderData2 = this.mEarthRenderData[m];
						renderData2.SetMesh(modelMesh2.VertexBuffer, modelMesh2.IndexBuffer, iMeshPart2);
						renderData2.mSkinnedModelMaterial.OverlayAlpha = overlayAlpha2;
					}
					this.mEarthAnimationController.StartClip(array2[Barrier.mRandom.Next(array2.Length)], false);
					this.mDrawMethod |= Barrier.DrawMethod.NORMAL;
				}
			}
			else
			{
				Elements iElement = Elements.None;
				float num3 = -1f;
				for (int n = 0; n < 11; n++)
				{
					Elements elements6 = Defines.ElementFromIndex(n);
					if ((elements6 & Elements.Steam) == Elements.Steam && (this.mSpell.Element & Elements.Lightning) == Elements.Lightning && (this.mSpell.Element & Elements.Steam) == Elements.Steam)
					{
						iElement = elements6;
						num3 = this.mSpell[iElement];
						n = 11;
					}
					if ((elements6 & Elements.Lightning) == Elements.Lightning && (this.mSpell.Element & Elements.Steam) == Elements.None && (this.mSpell.Element & Elements.Lightning) == Elements.Lightning)
					{
						iElement = elements6;
						num3 = this.mSpell[iElement];
						n = 11;
					}
					if ((elements6 & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Lightning | Elements.Steam)) != Elements.None && this.mSpell[elements6] > num3)
					{
						iElement = elements6;
						num3 = this.mSpell[iElement];
					}
				}
				Vector3 up = Vector3.Up;
				Matrix identity3 = Matrix.Identity;
				identity3.Forward = iDirection;
				identity3.Up = up;
				Vector3 right;
				Vector3.Cross(ref iDirection, ref up, out right);
				identity3.Right = right;
				MagickaMath.UniformMatrixScale(ref identity3, this.mScale);
				identity3.Translation = iPosition;
				this.mDrawMethod |= Barrier.DrawMethod.PARTICLEWALL;
				this.mSoundCue = AudioManager.Instance.GetCue(Banks.Spells, Barrier.BARRIER_SOUND_HASH[Spell.ElementIndex(iElement)]);
				if (this.mBarrierType == Barrier.BarrierType.ELEMENTAL)
				{
					this.mRadius = Barrier.GetRadius(false) * this.mScale;
					EffectManager.Instance.StartEffect(Barrier.BARRIER_EFFECTS[Defines.ElementIndex(iElement)], ref identity3, out this.mEffect);
				}
			}
			Matrix matrix;
			MagickaMath.MakeOrientationMatrix(ref iDirection, out matrix);
			Vector3 pos = iPosition;
			pos.Y += Barrier.GetRadius(this.Solid) * this.mScale + 0.05f;
			Matrix matrix2 = Matrix.CreateScale(this.mScale);
			Matrix orientation;
			Matrix.Multiply(ref matrix2, ref matrix, out orientation);
			this.mBody.MoveTo(pos, orientation);
			this.mBody.CollisionSkin.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
			(this.mCollision.GetPrimitiveLocal(0) as Capsule).Radius = this.mRadius;
			(this.mCollision.GetPrimitiveNewWorld(0) as Capsule).Radius = this.mRadius;
			(this.mCollision.GetPrimitiveOldWorld(0) as Capsule).Radius = this.mRadius;
			float radius = this.Solid ? (this.mRadius * 2f) : this.mRadius;
			(this.mCollision.GetPrimitiveLocal(1) as Sphere).Radius = radius;
			(this.mCollision.GetPrimitiveLocal(1) as Sphere).Position = new Vector3(0f, -this.mRadius, 0f);
			(this.mCollision.GetPrimitiveNewWorld(1) as Sphere).Radius = radius;
			(this.mCollision.GetPrimitiveNewWorld(1) as Sphere).Position = new Vector3(0f, -this.mRadius, 0f);
			(this.mCollision.GetPrimitiveOldWorld(1) as Sphere).Radius = radius;
			(this.mCollision.GetPrimitiveOldWorld(1) as Sphere).Position = new Vector3(0f, -this.mRadius, 0f);
			this.mVolume = (this.mCollision.GetPrimitiveLocal(0) as Capsule).GetVolume();
			List<Entity> entities = this.mPlayState.EntityManager.GetEntities(iPosition, iScale * Barrier.GetRadius(this.Solid) * 0.8f, false);
			for (int num4 = 0; num4 < entities.Count; num4++)
			{
				Barrier barrier = entities[num4] as Barrier;
				SpellMine spellMine = entities[num4] as SpellMine;
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
				entities = this.mPlayState.EntityManager.GetEntities(iPosition, iScale * Barrier.GetRadius(this.Solid) * 1.25f, false);
				for (int num5 = 0; num5 < entities.Count; num5++)
				{
					IDamageable damageable = entities[num5] as IDamageable;
					if (damageable != this && damageable != null && damageable != this.mOwner && !this.HitList.HitList.ContainsKey(damageable.Handle) && (!(damageable is Barrier) || (damageable as Barrier).HitList != this.HitList))
					{
						damageable.Damage(iDamage2, iOwner as Entity, this.mTimeStamp, iPosition);
						this.mHitList.HitList.Add(damageable.Handle, 0.25f);
					}
				}
				this.mPlayState.EntityManager.ReturnEntityList(entities);
			}
			this.mDamage = iDamage;
			if (this.Solid)
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
		}

		// Token: 0x06001757 RID: 5975 RVA: 0x000992DC File Offset: 0x000974DC
		public override void Deinitialize()
		{
			if (this.mSoundCue != null && !this.mSoundCue.IsStopping)
			{
				this.mSoundCue.Stop(AudioStopOptions.AsAuthored);
			}
			this.mEarthAnimationController.Stop();
			this.mIceAnimationController.Stop();
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
			base.Deinitialize();
			Barrier.ReturnToCache(this);
		}

		// Token: 0x170005E8 RID: 1512
		// (get) Token: 0x06001758 RID: 5976 RVA: 0x00099394 File Offset: 0x00097594
		protected bool RestingMovement
		{
			get
			{
				return this.mRestingMovementTimer < 0f;
			}
		}

		// Token: 0x06001759 RID: 5977 RVA: 0x000993A4 File Offset: 0x000975A4
		protected void SpawnNextBarrier()
		{
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				return;
			}
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
			float num;
			Vector3 vector2;
			Vector3 vector3;
			AnimatedLevelPart animatedLevelPart;
			if (!flag && this.mPlayState.Level.CurrentScene.SegmentIntersect(out num, out vector2, out vector3, out animatedLevelPart, iSeg))
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
				Barrier fromCache = Barrier.GetFromCache(this.mPlayState);
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					SpawnBarrierMessage spawnBarrierMessage;
					spawnBarrierMessage.Handle = fromCache.Handle;
					spawnBarrierMessage.OwnerHandle = this.Owner.Handle;
					spawnBarrierMessage.AnimationHandle = ((animatedLevelPart == null) ? ushort.MaxValue : animatedLevelPart.Handle);
					spawnBarrierMessage.Position = vector2;
					spawnBarrierMessage.Direction = forward;
					spawnBarrierMessage.Scale = this.mScale;
					spawnBarrierMessage.Spell = this.mSpell;
					spawnBarrierMessage.Damage = this.mDamage;
					spawnBarrierMessage.HitlistHandle = this.mHitList.Handle;
					NetworkManager.Instance.Interface.SendMessage<SpawnBarrierMessage>(ref spawnBarrierMessage);
				}
				fromCache.Initialize(this.Owner, vector2, forward, this.mScale, this.mNextBarrierRange, this.mNextBarrierDir, this.mNextBarrierRotation, this.mDistanceBetweenBarriers, ref this.mSpell, ref this.mDamage, this.mHitList, animatedLevelPart);
				this.mPlayState.EntityManager.AddEntity(fromCache);
			}
		}

		// Token: 0x0600175A RID: 5978 RVA: 0x00099688 File Offset: 0x00097888
		public float ResistanceAgainst(Elements iElement)
		{
			float num = 0f;
			float num2 = 0f;
			for (int i = 0; i < this.mResistances.Length; i++)
			{
				Elements elements = Defines.ElementFromIndex(i);
				if ((iElement & elements) != Elements.None)
				{
					float num3 = this.mResistances[i].Multiplier;
					float num4 = this.mResistances[i].Modifier;
					if (this.HasStatus(StatusEffects.Frozen) && (iElement & Elements.Earth) != Elements.None)
					{
						num4 -= 350f;
					}
					if (this.HasStatus(StatusEffects.Greased) && (iElement & Elements.Fire) != Elements.None)
					{
						num3 *= 2f;
					}
					num += num4;
					num2 += num3;
				}
			}
			float num5 = MathHelper.Clamp(num / 300f + num2, -1f, 1f);
			return 1f - num5;
		}

		// Token: 0x0600175B RID: 5979 RVA: 0x00099744 File Offset: 0x00097944
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			base.Update(iDataChannel, iDeltaTime);
			if (this.mSoundCue != null)
			{
				this.mSoundCue.Apply3D(this.mPlayState.Camera.Listener, base.AudioEmitter);
			}
			this.mEffectTTL -= iDeltaTime;
			this.mIceBarrierTTL -= iDeltaTime;
			this.mIceEarthTTL -= iDeltaTime;
			this.mRuneRotation -= iDeltaTime * 0.25f;
			if (this.mIceBarrierTTL < 0f && (this.mSpell.Element & Elements.PhysicalElements) == Elements.Ice)
			{
				this.mHitPoints = 0f;
			}
			if (this.mIceEarthTTL < 0f && !this.mKillIce && (this.mSpell.Element & Elements.Ice) == Elements.Ice && (this.mSpell.Element & Elements.Earth) == Elements.Earth)
			{
				this.mKillIce = true;
				this.RunIceEarthDeathAnimation();
			}
			if (this.mIceEarthTTL < 0f && !this.mIceAnimationController.IsPlaying && this.mKillIce && this.mDrawIce)
			{
				this.mDrawIce = false;
			}
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
			if (this.mDamageSelf)
			{
				this.mDamageTimer -= iDeltaTime;
				if (this.mDamageTimer < 0f)
				{
					this.mHitPoints -= 10f;
					this.mDamageTimer += 0.2f;
					if (this.Dead && this.mBarrierType == Barrier.BarrierType.SOLID)
					{
						this.RunDeathAnimation();
					}
				}
			}
			if (this.mBody.Velocity.LengthSquared() > 1E-06f)
			{
				this.mRestingMovementTimer = 1f;
			}
			else
			{
				this.mRestingMovementTimer -= iDeltaTime;
			}
			if ((this.mDrawMethod & Barrier.DrawMethod.PARTICLEWALL) == Barrier.DrawMethod.PARTICLEWALL)
			{
				Vector3 up = Vector3.Up;
				Matrix identity = Matrix.Identity;
				identity.Forward = this.mDirection;
				identity.Up = up;
				Vector3 right;
				Vector3.Cross(ref this.mDirection, ref up, out right);
				identity.Right = right;
				MagickaMath.UniformMatrixScale(ref identity, this.mScale);
				identity.Translation = this.Position;
				EffectManager.Instance.UpdateOrientation(ref this.mEffect, ref identity);
			}
			this.mNormalizedDamageTarget = 1f - this.mHitPoints / this.mMaxHitPoints;
			this.mNormalizedDamage = this.mNormalizedDamageTarget + (this.mNormalizedDamage - this.mNormalizedDamageTarget) * (float)Math.Pow(0.15, (double)iDeltaTime);
			if ((this.mDrawMethod & Barrier.DrawMethod.NORMAL) != Barrier.DrawMethod.NONE)
			{
				Matrix orientation = this.GetOrientation();
				if ((this.mSpell.Element & Elements.Ice) != Elements.None)
				{
					this.mIceAnimationController.Update(iDeltaTime, ref orientation, true);
				}
				if ((this.mSpell.Element & Elements.Earth) != Elements.None)
				{
					this.mEarthAnimationController.Update(iDeltaTime, ref orientation, true);
				}
			}
			if (this.mAddedEffect)
			{
				if (this.mEffectTTL > 0f)
				{
					Matrix matrix;
					if ((this.mSpell.Element & Elements.Ice) == Elements.Ice)
					{
						Matrix.Multiply(ref this.mEffectAttach.mBindPose, ref this.mIceAnimationController.SkinnedBoneTransforms[this.mEffectAttach.mIndex], out matrix);
					}
					else
					{
						Matrix.Multiply(ref this.mEffectAttach.mBindPose, ref this.mEarthAnimationController.SkinnedBoneTransforms[this.mEffectAttach.mIndex], out matrix);
					}
					EffectManager.Instance.UpdateOrientation(ref this.mEffect, ref matrix);
				}
				else
				{
					EffectManager.Instance.Stop(ref this.mEffect);
					this.mAddedEffect = false;
				}
			}
			if ((this.mDrawMethod & Barrier.DrawMethod.NORMAL) == Barrier.DrawMethod.NORMAL)
			{
				if ((this.mSpell.Element & Elements.Ice) == Elements.Ice && this.mDrawIce)
				{
					Barrier.RenderData renderData = this.mIceRenderData[(int)iDataChannel];
					renderData.mBoundingSphere.Center = this.Position;
					renderData.mBoundingSphere.Radius = this.mRadius * 3f;
					renderData.mDamage = this.mNormalizedDamage;
					Array.Copy(this.mIceAnimationController.SkinnedBoneTransforms, renderData.mBones, this.mIceAnimationController.Skeleton.Count);
					this.mPlayState.Scene.AddRenderableObject(iDataChannel, renderData);
				}
				if ((this.mSpell.Element & Elements.Earth) == Elements.Earth)
				{
					Barrier.RenderData renderData2 = this.mEarthRenderData[(int)iDataChannel];
					renderData2.mBoundingSphere.Center = this.Position;
					renderData2.mBoundingSphere.Radius = this.mRadius * 3f;
					renderData2.mDamage = this.mNormalizedDamage;
					Array.Copy(this.mEarthAnimationController.SkinnedBoneTransforms, renderData2.mBones, this.mEarthAnimationController.Skeleton.Count);
					this.mPlayState.Scene.AddRenderableObject(iDataChannel, renderData2);
				}
				if (this.mRuneModel != null)
				{
					RenderableAdditiveObject<AdditiveEffect, AdditiveMaterial> renderableAdditiveObject = this.mRuneRenderData[(int)iDataChannel];
					renderableAdditiveObject.mBoundingSphere.Center = this.Position;
					renderableAdditiveObject.mBoundingSphere.Radius = this.mRadius * 3f;
					Matrix.CreateRotationY(this.mRuneRotation, out renderableAdditiveObject.mMaterial.WorldTransform);
					renderableAdditiveObject.mMaterial.WorldTransform.Translation = this.Position;
					RenderableAdditiveObject<AdditiveEffect, AdditiveMaterial> renderableAdditiveObject2 = renderableAdditiveObject;
					renderableAdditiveObject2.mMaterial.WorldTransform.M42 = renderableAdditiveObject2.mMaterial.WorldTransform.M42 - (Barrier.GetRadius(this.Solid) * this.mScale + 0.05f);
					this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, renderableAdditiveObject);
				}
			}
		}

		// Token: 0x0600175C RID: 5980 RVA: 0x00099D04 File Offset: 0x00097F04
		public override Vector3 CalcImpulseVelocity(Vector3 iDirection, float iElevation, float iMassPower, float iDistance)
		{
			return default(Vector3);
		}

		// Token: 0x170005E9 RID: 1513
		// (get) Token: 0x0600175D RID: 5981 RVA: 0x00099D1A File Offset: 0x00097F1A
		public override bool Dead
		{
			get
			{
				return this.mHitPoints <= 0f;
			}
		}

		// Token: 0x170005EA RID: 1514
		// (get) Token: 0x0600175E RID: 5982 RVA: 0x00099D2C File Offset: 0x00097F2C
		public override bool Removable
		{
			get
			{
				return this.Dead && !this.mIceAnimationController.IsPlaying && !this.mEarthAnimationController.IsPlaying;
			}
		}

		// Token: 0x170005EB RID: 1515
		// (get) Token: 0x0600175F RID: 5983 RVA: 0x00099D53 File Offset: 0x00097F53
		public ISpellCaster Owner
		{
			get
			{
				return this.mOwner;
			}
		}

		// Token: 0x06001760 RID: 5984 RVA: 0x00099D5B File Offset: 0x00097F5B
		public virtual bool HasStatus(StatusEffects iStatus)
		{
			return (this.mCurrentStatusEffects & iStatus) == iStatus;
		}

		// Token: 0x06001761 RID: 5985 RVA: 0x00099D6C File Offset: 0x00097F6C
		public virtual float StatusMagnitude(StatusEffects iStatus)
		{
			int num = StatusEffect.StatusIndex(iStatus);
			if (!this.mStatusEffects[num].Dead)
			{
				return this.mStatusEffects[num].Magnitude;
			}
			return 0f;
		}

		// Token: 0x170005EC RID: 1516
		// (get) Token: 0x06001762 RID: 5986 RVA: 0x00099DAA File Offset: 0x00097FAA
		public override Vector3 Position
		{
			get
			{
				return this.mBody.Position;
			}
		}

		// Token: 0x170005ED RID: 1517
		// (get) Token: 0x06001763 RID: 5987 RVA: 0x00099DB7 File Offset: 0x00097FB7
		public float HitPoints
		{
			get
			{
				return this.mHitPoints;
			}
		}

		// Token: 0x170005EE RID: 1518
		// (get) Token: 0x06001764 RID: 5988 RVA: 0x00099DBF File Offset: 0x00097FBF
		public float MaxHitPoints
		{
			get
			{
				return this.mMaxHitPoints;
			}
		}

		// Token: 0x170005EF RID: 1519
		// (get) Token: 0x06001765 RID: 5989 RVA: 0x00099DC7 File Offset: 0x00097FC7
		public bool Solid
		{
			get
			{
				return this.mBarrierType == Barrier.BarrierType.SOLID;
			}
		}

		// Token: 0x170005F0 RID: 1520
		// (get) Token: 0x06001766 RID: 5990 RVA: 0x00099DD2 File Offset: 0x00097FD2
		public Barrier.HitListWithBarriers HitList
		{
			get
			{
				return this.mHitList;
			}
		}

		// Token: 0x06001767 RID: 5991 RVA: 0x00099DDC File Offset: 0x00097FDC
		public void GetRandomPositionOnCollisionSkin(out Vector3 oPosition)
		{
			Vector3 position = this.Position;
			if (this.mCollision == null)
			{
				oPosition = position;
			}
			Vector3 vector = default(Vector3);
			float iAngle = MagickaMath.RandomBetween(-3.1415927f, 3.1415927f);
			float iAngle2 = MagickaMath.RandomBetween(-1.5707964f, 1.5707964f);
			float num;
			float num2;
			MathApproximation.FastSinCos(iAngle, out num, out num2);
			float num3;
			float num4;
			MathApproximation.FastSinCos(iAngle2, out num3, out num4);
			num2 *= num4;
			num *= num4;
			float radius = (this.mCollision.GetPrimitiveLocal(0) as Capsule).Radius;
			float num5 = (this.mCollision.GetPrimitiveLocal(0) as Capsule).Radius * 2f + (this.mCollision.GetPrimitiveLocal(0) as Capsule).Length;
			vector.X = num2 * radius;
			vector.Z = num * radius;
			vector.Y = num3 * num5 * 0.5f;
			vector.X += position.X;
			vector.Y += position.Y;
			vector.Z += position.Z;
			oPosition = vector;
		}

		// Token: 0x170005F1 RID: 1521
		// (get) Token: 0x06001768 RID: 5992 RVA: 0x00099F06 File Offset: 0x00098106
		public virtual float Volume
		{
			get
			{
				return this.mVolume;
			}
		}

		// Token: 0x170005F2 RID: 1522
		// (get) Token: 0x06001769 RID: 5993 RVA: 0x00099F0E File Offset: 0x0009810E
		public Resistance[] Resistance
		{
			get
			{
				return this.mResistances;
			}
		}

		// Token: 0x0600176A RID: 5994 RVA: 0x00099F16 File Offset: 0x00098116
		public void SetSlow()
		{
		}

		// Token: 0x0600176B RID: 5995 RVA: 0x00099F18 File Offset: 0x00098118
		public void Damage(float iDamage, Elements iElement)
		{
			this.mHitPoints -= iDamage;
			if (this.Dead && this.mBarrierType == Barrier.BarrierType.SOLID)
			{
				this.RunDeathAnimation();
			}
		}

		// Token: 0x0600176C RID: 5996 RVA: 0x00099F3E File Offset: 0x0009813E
		public StatusEffect[] GetStatusEffects()
		{
			return this.mStatusEffects;
		}

		// Token: 0x0600176D RID: 5997 RVA: 0x00099F48 File Offset: 0x00098148
		public virtual DamageResult InternalDamage(DamageCollection5 iDamages, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			DamageResult damageResult = DamageResult.None;
			damageResult |= this.InternalDamage(iDamages.A, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			damageResult |= this.InternalDamage(iDamages.B, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			damageResult |= this.InternalDamage(iDamages.C, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			damageResult |= this.InternalDamage(iDamages.D, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			return damageResult | this.InternalDamage(iDamages.E, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
		}

		// Token: 0x0600176E RID: 5998 RVA: 0x00099FC8 File Offset: 0x000981C8
		public DamageResult InternalDamage(Damage iDamage, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			DamageResult damageResult = DamageResult.None;
			Elements elements = iDamage.Element;
			if ((elements & Elements.Arcane) == Elements.Arcane)
			{
				elements ^= Elements.Arcane;
			}
			switch (this.mBarrierType)
			{
			case Barrier.BarrierType.SOLID:
				if (iDamage.Amount > 0f)
				{
					if ((this.mSpell.Element & Elements.Cold) == Elements.Cold && ((elements & Elements.Fire) == Elements.Fire || (elements & Elements.Steam) == Elements.Steam))
					{
						this.mEffectTTL = 0f;
						break;
					}
					if ((this.mSpell.Element & Elements.Steam) == Elements.Steam && (elements & Elements.Cold) == Elements.Cold)
					{
						this.mEffectTTL = 0f;
						break;
					}
					if ((this.mSpell.Element & Elements.Water) == Elements.Water && (elements & Elements.Fire) == Elements.Fire)
					{
						this.mEffectTTL = 0f;
						break;
					}
					if ((this.mSpell.Element & Elements.Fire) == Elements.Fire && (elements & Elements.Water) == Elements.Water)
					{
						this.mEffectTTL = 0f;
						break;
					}
					if (this.mSpell[Elements.Ice] > 0f && (elements & Elements.Fire) == Elements.Fire)
					{
						iDamage.Amount = Math.Max(iDamage.Amount, 0f);
					}
					else
					{
						iDamage.Amount = Math.Max(iDamage.Amount - (float)this.mArmour, 0f);
					}
					if (iDamage.Amount == 0f)
					{
						damageResult |= DamageResult.Deflected;
					}
					if (iDamage.Amount > 0f)
					{
						if ((iDamage.Element & Elements.Earth) == Elements.Earth && (short)(iDamage.AttackProperty & AttackProperties.Damage) == 1)
						{
							if (this.mSpell.IceMagnitude > 0f)
							{
								Vector3 position = this.Position;
								Vector3 vector;
								Vector3.Subtract(ref iAttackPosition, ref position, out vector);
								vector.Normalize();
								VisualEffectReference visualEffectReference;
								EffectManager.Instance.StartEffect(Barrier.ICE_HIT_EFFECT, ref position, ref vector, out visualEffectReference);
							}
							else
							{
								Vector3 position2 = this.Position;
								Vector3 vector2;
								Vector3.Subtract(ref iAttackPosition, ref position2, out vector2);
								vector2.Normalize();
								VisualEffectReference visualEffectReference;
								EffectManager.Instance.StartEffect(Barrier.EARTH_HIT_EFFECT, ref position2, ref vector2, out visualEffectReference);
							}
						}
						damageResult |= DamageResult.Damaged;
					}
				}
				if (Defines.FeatureDamage(iFeatures) && iDamage.Amount * iDamage.Magnitude > 0f)
				{
					this.mHitPoints -= iDamage.Amount;
				}
				break;
			case Barrier.BarrierType.ELEMENTAL:
			{
				float num = 0f;
				float num2 = 0f;
				for (int i = 0; i < 11; i++)
				{
					Elements elements2 = Defines.ElementFromIndex(i);
					if ((this.mSpell.Element & elements2) == elements2 && (elements2 & Elements.Shield) == Elements.None)
					{
						num2 += 1f;
						if (SpellManager.InclusiveOpposites(elements, elements2))
						{
							num += 1f;
						}
					}
				}
				if (Defines.FeatureDamage(iFeatures) && iDamage.Amount * iDamage.Magnitude > 0f)
				{
					this.mHitPoints -= (float)((int)(num / num2 * iDamage.Amount * 5f));
				}
				break;
			}
			}
			if (this.mBarrierType == Barrier.BarrierType.SOLID)
			{
				if (this.Dead)
				{
					this.RunDeathAnimation();
				}
				if (this.mHitPoints - iDamage.Amount < 0f)
				{
					damageResult |= DamageResult.Killed;
					this.DeathVisuals();
				}
			}
			return damageResult;
		}

		// Token: 0x0600176F RID: 5999 RVA: 0x0009A2F4 File Offset: 0x000984F4
		public void Electrocute(IDamageable iTarget, float iMultiplyer)
		{
			if (!this.mHitList.HitList.ContainsKey(iTarget.Handle))
			{
				this.mHitList.HitList.Add(iTarget.Handle);
				DamageCollection5 damageCollection = this.mDamage;
				damageCollection.MultiplyMagnitude(iMultiplyer);
				if ((damageCollection.A.Element & Elements.Lightning) != Elements.None)
				{
					iTarget.Damage(damageCollection.A, this.Owner as Entity, this.mTimeStamp, this.Position);
				}
				if ((damageCollection.B.Element & Elements.Lightning) != Elements.None)
				{
					iTarget.Damage(damageCollection.B, this.Owner as Entity, this.mTimeStamp, this.Position);
				}
				if ((damageCollection.C.Element & Elements.Lightning) != Elements.None)
				{
					iTarget.Damage(damageCollection.C, this.Owner as Entity, this.mTimeStamp, this.Position);
				}
				if ((damageCollection.D.Element & Elements.Lightning) != Elements.None)
				{
					iTarget.Damage(damageCollection.D, this.Owner as Entity, this.mTimeStamp, this.Position);
				}
				if ((damageCollection.E.Element & Elements.Lightning) != Elements.None)
				{
					iTarget.Damage(damageCollection.E, this.Owner as Entity, this.mTimeStamp, this.Position);
				}
			}
		}

		// Token: 0x06001770 RID: 6000 RVA: 0x0009A44F File Offset: 0x0009864F
		public override void Kill()
		{
			this.mHitPoints = 0f;
			if (this.mBarrierType == Barrier.BarrierType.SOLID)
			{
				this.RunDeathAnimation();
			}
		}

		// Token: 0x06001771 RID: 6001 RVA: 0x0009A46C File Offset: 0x0009866C
		public void Detonate()
		{
			this.mIceAnimationController.Stop();
			this.mEarthAnimationController.Stop();
			Vector3 position = this.Position;
			Vector3 forward = this.mBody.Orientation.Forward;
			Blast.FullBlast(this.mPlayState, this.mOwner as Entity, this.mTimeStamp, this, 5f, position, this.mDamage);
			VisualEffectReference visualEffectReference;
			EffectManager.Instance.StartEffect(Barrier.EXPLOSION_EFFECT, ref position, ref forward, out visualEffectReference);
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

		// Token: 0x06001772 RID: 6002 RVA: 0x0009A544 File Offset: 0x00098744
		private void DeathVisuals()
		{
			if ((this.mSpell.Element & Elements.Ice) == Elements.Ice && (this.mSpell.Element & Elements.Earth) == Elements.Earth && !this.mKillIce)
			{
				AudioManager.Instance.PlayCue(Banks.Spells, Barrier.Ice_Earth_Barrier_Break_Sound_Hash, base.AudioEmitter);
			}
			else if ((this.mSpell.Element & Elements.Ice) == Elements.Ice && !this.mKillIce)
			{
				AudioManager.Instance.PlayCue(Banks.Spells, Barrier.Ice_Barrier_Break_Sound_Hash, base.AudioEmitter);
			}
			else if ((this.mSpell.Element & Elements.Earth) == Elements.Earth)
			{
				AudioManager.Instance.PlayCue(Banks.Spells, Barrier.Earth_Barrier_Break_Sound_Hash, base.AudioEmitter);
			}
			if ((this.mSpell.Element & Elements.Ice) == Elements.Ice && !this.mKillIce)
			{
				EffectManager.Instance.Stop(ref this.mSpawnDeathEffectReference);
				Vector3 position = this.Body.Position;
				position.Y -= 0.5f * this.Capsule.Length + this.Capsule.Radius;
				Vector3 forward = this.Body.Orientation.Forward;
				EffectManager.Instance.StartEffect(Barrier.Ice_Barrier_Death_Effect_Hash, ref position, ref forward, out this.mSpawnDeathEffectReference);
			}
			if ((this.mSpell.Element & Elements.Earth) == Elements.Earth)
			{
				EffectManager.Instance.Stop(ref this.mSpawnDeathEffectReference);
				Vector3 position2 = this.Body.Position;
				position2.Y -= 0.5f * this.Capsule.Length + this.Capsule.Radius;
				Vector3 forward2 = this.Body.Orientation.Forward;
				EffectManager.Instance.StartEffect(Barrier.Earth_Barrier_Death_Effect_Hash, ref position2, ref forward2, out this.mSpawnDeathEffectReference);
			}
		}

		// Token: 0x06001773 RID: 6003 RVA: 0x0009A71C File Offset: 0x0009891C
		protected void RunDeathAnimation()
		{
			if (this.mDead)
			{
				return;
			}
			this.mDead = true;
			if ((this.mSpell.Element & Elements.Beams) != Elements.None)
			{
				this.Detonate();
				return;
			}
			if (!this.mIceAnimationController.IsPlaying && !this.mEarthAnimationController.IsPlaying)
			{
				this.mIceAnimationController.PlaybackMode = PlaybackMode.Backward;
				this.mEarthAnimationController.PlaybackMode = PlaybackMode.Backward;
				if ((this.mSpell.Element & Elements.Ice) == Elements.Ice && (this.mSpell.Element & Elements.Earth) == Elements.Earth && !this.mKillIce)
				{
					AudioManager.Instance.PlayCue(Banks.Spells, Barrier.Ice_Earth_Barrier_Break_Sound_Hash, base.AudioEmitter);
				}
				else if ((this.mSpell.Element & Elements.Ice) == Elements.Ice && !this.mKillIce)
				{
					AudioManager.Instance.PlayCue(Banks.Spells, Barrier.Ice_Barrier_Break_Sound_Hash, base.AudioEmitter);
				}
				else if ((this.mSpell.Element & Elements.Earth) == Elements.Earth)
				{
					AudioManager.Instance.PlayCue(Banks.Spells, Barrier.Earth_Barrier_Break_Sound_Hash, base.AudioEmitter);
				}
				if ((this.mSpell.Element & Elements.Ice) == Elements.Ice && !this.mKillIce)
				{
					EffectManager.Instance.Stop(ref this.mSpawnDeathEffectReference);
					Vector3 position = this.Body.Position;
					position.Y -= 0.5f * this.Capsule.Length + this.Capsule.Radius;
					Vector3 forward = this.Body.Orientation.Forward;
					EffectManager.Instance.StartEffect(Barrier.Ice_Barrier_Death_Effect_Hash, ref position, ref forward, out this.mSpawnDeathEffectReference);
					this.mIceAnimationController.StartClip(Barrier.sIceAppearClips[0], false);
				}
				if ((this.mSpell.Element & Elements.Earth) == Elements.Earth)
				{
					EffectManager.Instance.Stop(ref this.mSpawnDeathEffectReference);
					Vector3 position2 = this.Body.Position;
					position2.Y -= 0.5f * this.Capsule.Length + this.Capsule.Radius;
					Vector3 forward2 = this.Body.Orientation.Forward;
					EffectManager.Instance.StartEffect(Barrier.Earth_Barrier_Death_Effect_Hash, ref position2, ref forward2, out this.mSpawnDeathEffectReference);
					this.mEarthAnimationController.StartClip(Barrier.sEarthAppearClips[0], false);
				}
			}
		}

		// Token: 0x06001774 RID: 6004 RVA: 0x0009A980 File Offset: 0x00098B80
		public void RunIceEarthDeathAnimation()
		{
			if (!this.mIceAnimationController.IsPlaying)
			{
				this.mIceAnimationController.PlaybackMode = PlaybackMode.Backward;
				AudioManager.Instance.PlayCue(Banks.Spells, Barrier.Ice_Barrier_Break_Sound_Hash, base.AudioEmitter);
				EffectManager.Instance.Stop(ref this.mSpawnDeathEffectReference);
				Vector3 position = this.Body.Position;
				position.Y -= 0.5f * this.Capsule.Length + this.Capsule.Radius;
				Vector3 forward = this.Body.Orientation.Forward;
				EffectManager.Instance.StartEffect(Barrier.Ice_Barrier_Death_Effect_Hash, ref position, ref forward, out this.mSpawnDeathEffectReference);
				this.mIceAnimationController.StartClip(Barrier.sIceAppearClips[0], false);
			}
		}

		// Token: 0x170005F3 RID: 1523
		// (get) Token: 0x06001775 RID: 6005 RVA: 0x0009AA49 File Offset: 0x00098C49
		public Capsule Capsule
		{
			get
			{
				return this.mCollision.GetPrimitiveNewWorld(0) as Capsule;
			}
		}

		// Token: 0x06001776 RID: 6006 RVA: 0x0009AA5C File Offset: 0x00098C5C
		public bool SegmentIntersect(out Vector3 oPosition, Segment iSeg, float iSegmentRadius)
		{
			Segment seg = default(Segment);
			seg.Origin = this.Position;
			seg.Origin.Y = seg.Origin.Y - this.Capsule.Length * 0.5f;
			seg.Delta = this.Capsule.Orientation.Backward;
			Vector3.Multiply(ref seg.Delta, this.Capsule.Length, out seg.Delta);
			float t;
			float t2;
			float num = Distance.SegmentSegmentDistanceSq(out t, out t2, seg, iSeg);
			float num2 = iSegmentRadius + this.Capsule.Radius;
			num2 *= num2;
			if (num > num2)
			{
				oPosition = default(Vector3);
				return false;
			}
			Vector3 vector;
			seg.GetPoint(t, out vector);
			Vector3 vector2;
			iSeg.GetPoint(t2, out vector2);
			Vector3.Subtract(ref vector2, ref vector, out vector2);
			vector2.Normalize();
			Vector3.Multiply(ref vector2, this.Capsule.Radius, out vector2);
			Vector3.Add(ref vector, ref vector2, out oPosition);
			return true;
		}

		// Token: 0x06001777 RID: 6007 RVA: 0x0009AB50 File Offset: 0x00098D50
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

		// Token: 0x06001778 RID: 6008 RVA: 0x0009AC23 File Offset: 0x00098E23
		public void OverKill()
		{
			this.mHitPoints = -this.mMaxHitPoints;
		}

		// Token: 0x06001779 RID: 6009 RVA: 0x0009AC34 File Offset: 0x00098E34
		protected unsafe override void INetworkUpdate(ref EntityUpdateMessage iMsg)
		{
			base.INetworkUpdate(ref iMsg);
			this.mHitPoints = iMsg.HitPoints;
			fixed (float* ptr = &iMsg.StatusEffectMagnitude.FixedElementField)
			{
				for (int i = 0; i < 9; i++)
				{
					this.mStatusEffects[i].Magnitude = ptr[i];
				}
			}
		}

		// Token: 0x0600177A RID: 6010 RVA: 0x0009AC8C File Offset: 0x00098E8C
		protected unsafe override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
		{
			oMsg = default(EntityUpdateMessage);
			if (!this.RestingMovement)
			{
				oMsg.Features |= EntityFeatures.Position;
				oMsg.Position = this.Position;
			}
			oMsg.Features |= EntityFeatures.Damageable;
			oMsg.HitPoints = this.mHitPoints;
			oMsg.Features |= EntityFeatures.StatusEffected;
			oMsg.StatusEffects = this.mCurrentStatusEffects;
			fixed (float* ptr = &oMsg.StatusEffectMagnitude.FixedElementField)
			{
				for (int i = 0; i < 9; i++)
				{
					ptr[i] = this.mStatusEffects[i].Magnitude;
				}
			}
		}

		// Token: 0x0600177B RID: 6011 RVA: 0x0009AD30 File Offset: 0x00098F30
		internal override float GetDanger()
		{
			return this.mDamage.GetTotalMagnitude();
		}

		// Token: 0x040018CB RID: 6347
		private const float ELEMENTALRADIUS = 0.75f;

		// Token: 0x040018CC RID: 6348
		private const float RADIUS = 1.25f;

		// Token: 0x040018CD RID: 6349
		public const float LENGTH = 0.1f;

		// Token: 0x040018CE RID: 6350
		protected static List<Barrier> mCache;

		// Token: 0x040018CF RID: 6351
		protected static readonly int[] BARRIER_EFFECTS;

		// Token: 0x040018D0 RID: 6352
		public static readonly int EXPLOSION_EFFECT = "mine_explosion".GetHashCodeCustom();

		// Token: 0x040018D1 RID: 6353
		protected static readonly int[] BARRIER_GEYSER_EFFECTS = new int[]
		{
			0,
			"barrier_geyser_water".GetHashCodeCustom(),
			"barrier_geyser_cold".GetHashCodeCustom(),
			"barrier_geyser_fire".GetHashCodeCustom(),
			"barrier_ice_tesla".GetHashCodeCustom(),
			0,
			0,
			0,
			0,
			"barrier_geyser_steam".GetHashCodeCustom(),
			"barrier_geyser_poison".GetHashCodeCustom()
		};

		// Token: 0x040018D2 RID: 6354
		public static readonly int[] BARRIER_SOUND_HASH = new int[]
		{
			"spell_earth_barrier".GetHashCodeCustom(),
			"spell_water_barrier".GetHashCodeCustom(),
			"spell_cold_barrier".GetHashCodeCustom(),
			"spell_fire_barrier".GetHashCodeCustom(),
			"spell_lightning_barrier".GetHashCodeCustom(),
			"spell_arcane_barrier".GetHashCodeCustom(),
			"spell_life_barrier".GetHashCodeCustom(),
			"spell_shield_barrier".GetHashCodeCustom(),
			"spell_ice_barrier".GetHashCodeCustom(),
			"spell_steam_barrier".GetHashCodeCustom()
		};

		// Token: 0x040018D3 RID: 6355
		public static readonly int Earth_Barrier_Spawn_Effect_Hash = "barrier_earth_spawn".GetHashCodeCustom();

		// Token: 0x040018D4 RID: 6356
		public static readonly int Earth_Barrier_Death_Effect_Hash = "barrier_earth_death".GetHashCodeCustom();

		// Token: 0x040018D5 RID: 6357
		public static readonly int Ice_Barrier_Spawn_Effect_Hash = "barrier_ice_spawn".GetHashCodeCustom();

		// Token: 0x040018D6 RID: 6358
		public static readonly int Ice_Barrier_Death_Effect_Hash = "barrier_ice_death".GetHashCodeCustom();

		// Token: 0x040018D7 RID: 6359
		public static readonly int Earth_Barrier_Sound_Hash = "spell_earth_barrier".GetHashCodeCustom();

		// Token: 0x040018D8 RID: 6360
		public static readonly int Earth_Barrier_Break_Sound_Hash = "spell_earth_barrier_break".GetHashCodeCustom();

		// Token: 0x040018D9 RID: 6361
		public static readonly int Ice_Barrier_Sound_Hash = "spell_ice_barrier".GetHashCodeCustom();

		// Token: 0x040018DA RID: 6362
		public static readonly int Ice_Barrier_Break_Sound_Hash = "spell_ice_barrier_break".GetHashCodeCustom();

		// Token: 0x040018DB RID: 6363
		public static readonly int Ice_Earth_Barrier_Sound_Hash = "spell_ice_earth_barrier".GetHashCodeCustom();

		// Token: 0x040018DC RID: 6364
		public static readonly int Ice_Earth_Barrier_Break_Sound_Hash = "spell_ice_earth_barrier_break".GetHashCodeCustom();

		// Token: 0x040018DD RID: 6365
		public static readonly int ICE_HIT_EFFECT = "barrier_ice_deflect".GetHashCodeCustom();

		// Token: 0x040018DE RID: 6366
		public static readonly int EARTH_HIT_EFFECT = "barrier_earth_deflect".GetHashCodeCustom();

		// Token: 0x040018DF RID: 6367
		protected static SkinnedModel[] sIceBarrierModels;

		// Token: 0x040018E0 RID: 6368
		protected static AnimationClip[] sIceAppearClips;

		// Token: 0x040018E1 RID: 6369
		protected static SkinnedModel[] sIceBarrierGeyserModels;

		// Token: 0x040018E2 RID: 6370
		protected static AnimationClip[] sIceGeyserAppearClips;

		// Token: 0x040018E3 RID: 6371
		protected static SkinnedModel[] sIceBarrierTeslaModels;

		// Token: 0x040018E4 RID: 6372
		protected static AnimationClip[] sIceTeslaAppearClips;

		// Token: 0x040018E5 RID: 6373
		protected static SkinnedModel[] sIceBarrierEarthModels;

		// Token: 0x040018E6 RID: 6374
		protected static AnimationClip[] sIceEarthAppearClips;

		// Token: 0x040018E7 RID: 6375
		protected static SkinnedModel[] sEarthBarrierModels;

		// Token: 0x040018E8 RID: 6376
		protected static AnimationClip[] sEarthAppearClips;

		// Token: 0x040018E9 RID: 6377
		protected static SkinnedModel[] sEarthVulcanoBarrierModels;

		// Token: 0x040018EA RID: 6378
		protected static AnimationClip[] sEarthVulcanoAppearClips;

		// Token: 0x040018EB RID: 6379
		protected static Model sRunesArcane;

		// Token: 0x040018EC RID: 6380
		protected static Model sRunesLife;

		// Token: 0x040018ED RID: 6381
		private static BindJoint sGeyserAttach;

		// Token: 0x040018EE RID: 6382
		private static BindJoint sTeslaAttach;

		// Token: 0x040018EF RID: 6383
		protected static BindJoint sVulcanoAttach;

		// Token: 0x040018F0 RID: 6384
		protected BindJoint mEffectAttach;

		// Token: 0x040018F1 RID: 6385
		protected bool mAddedEffect;

		// Token: 0x040018F2 RID: 6386
		private Model mRuneModel;

		// Token: 0x040018F3 RID: 6387
		protected Barrier.HitListWithBarriers mHitList;

		// Token: 0x040018F4 RID: 6388
		private Barrier.RenderData[] mIceRenderData;

		// Token: 0x040018F5 RID: 6389
		protected Barrier.RenderData[] mEarthRenderData;

		// Token: 0x040018F6 RID: 6390
		private RenderableAdditiveObject<AdditiveEffect, AdditiveMaterial>[] mRuneRenderData;

		// Token: 0x040018F7 RID: 6391
		protected Barrier.DrawMethod mDrawMethod;

		// Token: 0x040018F8 RID: 6392
		protected Spell mSpell;

		// Token: 0x040018F9 RID: 6393
		protected DamageCollection5 mDamage;

		// Token: 0x040018FA RID: 6394
		protected int mArmour;

		// Token: 0x040018FB RID: 6395
		protected float mScale;

		// Token: 0x040018FC RID: 6396
		protected float mHitPoints;

		// Token: 0x040018FD RID: 6397
		protected float mMaxHitPoints;

		// Token: 0x040018FE RID: 6398
		protected float mDamageTimer;

		// Token: 0x040018FF RID: 6399
		protected float mNextBarrierTTL;

		// Token: 0x04001900 RID: 6400
		protected Vector3 mNextBarrierDir;

		// Token: 0x04001901 RID: 6401
		protected Quaternion mNextBarrierRotation;

		// Token: 0x04001902 RID: 6402
		protected float mNextBarrierRange;

		// Token: 0x04001903 RID: 6403
		protected float mDistanceBetweenBarriers;

		// Token: 0x04001904 RID: 6404
		protected float mNormalizedDamage;

		// Token: 0x04001905 RID: 6405
		protected float mNormalizedDamageTarget;

		// Token: 0x04001906 RID: 6406
		protected bool mDamageSelf;

		// Token: 0x04001907 RID: 6407
		protected Barrier.BarrierType mBarrierType;

		// Token: 0x04001908 RID: 6408
		protected static Random mRandom = new Random();

		// Token: 0x04001909 RID: 6409
		private AnimationController mIceAnimationController;

		// Token: 0x0400190A RID: 6410
		protected AnimationController mEarthAnimationController;

		// Token: 0x0400190B RID: 6411
		protected float mVolume;

		// Token: 0x0400190C RID: 6412
		protected Resistance[] mResistances;

		// Token: 0x0400190D RID: 6413
		protected Vector3 mDirection;

		// Token: 0x0400190E RID: 6414
		protected ISpellCaster mOwner;

		// Token: 0x0400190F RID: 6415
		protected Cue mSoundCue;

		// Token: 0x04001910 RID: 6416
		protected double mTimeStamp;

		// Token: 0x04001911 RID: 6417
		protected float mEffectTTL;

		// Token: 0x04001912 RID: 6418
		private float mIceBarrierTTL;

		// Token: 0x04001913 RID: 6419
		private float mIceEarthTTL;

		// Token: 0x04001914 RID: 6420
		private bool mDrawIce;

		// Token: 0x04001915 RID: 6421
		private bool mKillIce;

		// Token: 0x04001916 RID: 6422
		protected float mRuneRotation;

		// Token: 0x04001917 RID: 6423
		protected bool mInitilizeDamage;

		// Token: 0x04001918 RID: 6424
		protected float mRestingMovementTimer = 1f;

		// Token: 0x04001919 RID: 6425
		protected VisualEffectReference mEffect;

		// Token: 0x0400191A RID: 6426
		private VisualEffectReference mSpawnDeathEffectReference;

		// Token: 0x0400191B RID: 6427
		protected StatusEffect[] mStatusEffects = new StatusEffect[9];

		// Token: 0x0400191C RID: 6428
		protected StatusEffects mCurrentStatusEffects;

		// Token: 0x020002F5 RID: 757
		protected class RenderData : IRenderableObject
		{
			// Token: 0x0600177C RID: 6012 RVA: 0x0009AD3D File Offset: 0x00098F3D
			public RenderData()
			{
				this.mBones = new Matrix[80];
				this.mDamage = 0f;
			}

			// Token: 0x170005F4 RID: 1524
			// (get) Token: 0x0600177D RID: 6013 RVA: 0x0009AD5D File Offset: 0x00098F5D
			public int Effect
			{
				get
				{
					return SkinnedModelDeferredEffect.TYPEHASH;
				}
			}

			// Token: 0x170005F5 RID: 1525
			// (get) Token: 0x0600177E RID: 6014 RVA: 0x0009AD64 File Offset: 0x00098F64
			public int DepthTechnique
			{
				get
				{
					return 3;
				}
			}

			// Token: 0x170005F6 RID: 1526
			// (get) Token: 0x0600177F RID: 6015 RVA: 0x0009AD67 File Offset: 0x00098F67
			public int Technique
			{
				get
				{
					return 0;
				}
			}

			// Token: 0x170005F7 RID: 1527
			// (get) Token: 0x06001780 RID: 6016 RVA: 0x0009AD6A File Offset: 0x00098F6A
			public int ShadowTechnique
			{
				get
				{
					return 4;
				}
			}

			// Token: 0x170005F8 RID: 1528
			// (get) Token: 0x06001781 RID: 6017 RVA: 0x0009AD6D File Offset: 0x00098F6D
			public VertexBuffer Vertices
			{
				get
				{
					return this.mVertexBuffer;
				}
			}

			// Token: 0x170005F9 RID: 1529
			// (get) Token: 0x06001782 RID: 6018 RVA: 0x0009AD75 File Offset: 0x00098F75
			public IndexBuffer Indices
			{
				get
				{
					return this.mIndexBuffer;
				}
			}

			// Token: 0x170005FA RID: 1530
			// (get) Token: 0x06001783 RID: 6019 RVA: 0x0009AD7D File Offset: 0x00098F7D
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return this.mVertexDeclaration;
				}
			}

			// Token: 0x170005FB RID: 1531
			// (get) Token: 0x06001784 RID: 6020 RVA: 0x0009AD85 File Offset: 0x00098F85
			public int VertexStride
			{
				get
				{
					return this.mVertexStride;
				}
			}

			// Token: 0x170005FC RID: 1532
			// (get) Token: 0x06001785 RID: 6021 RVA: 0x0009AD8D File Offset: 0x00098F8D
			public Texture2D Texture
			{
				get
				{
					return this.mSkinnedModelMaterial.DiffuseMap0;
				}
			}

			// Token: 0x170005FD RID: 1533
			// (get) Token: 0x06001786 RID: 6022 RVA: 0x0009AD9A File Offset: 0x00098F9A
			public int VerticesHashCode
			{
				get
				{
					return this.mVerticesHash;
				}
			}

			// Token: 0x06001787 RID: 6023 RVA: 0x0009ADA4 File Offset: 0x00098FA4
			public bool Cull(BoundingFrustum iViewFrustum)
			{
				BoundingSphere boundingSphere = this.mBoundingSphere;
				return boundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
			}

			// Token: 0x06001788 RID: 6024 RVA: 0x0009ADC4 File Offset: 0x00098FC4
			public virtual void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				this.mSkinnedModelMaterial.AssignToEffect(skinnedModelDeferredEffect);
				skinnedModelDeferredEffect.Damage = this.mDamage;
				skinnedModelDeferredEffect.Bones = this.mBones;
				skinnedModelDeferredEffect.CubeMapEnabled = false;
				skinnedModelDeferredEffect.CubeNormalMapEnabled = false;
				skinnedModelDeferredEffect.ProjectionMapEnabled = false;
				skinnedModelDeferredEffect.SpecularBias = 0f;
				skinnedModelDeferredEffect.CommitChanges();
				skinnedModelDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
			}

			// Token: 0x06001789 RID: 6025 RVA: 0x0009AE48 File Offset: 0x00099048
			public virtual void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				this.mSkinnedModelMaterial.AssignOpacityToEffect(skinnedModelDeferredEffect);
				skinnedModelDeferredEffect.Bones = this.mBones;
				skinnedModelDeferredEffect.CommitChanges();
				skinnedModelDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
			}

			// Token: 0x0600178A RID: 6026 RVA: 0x0009AEA0 File Offset: 0x000990A0
			public virtual void SetMesh(VertexBuffer iVertices, IndexBuffer iIndices, ModelMeshPart iMeshPart)
			{
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
				Helper.SkinnedModelDeferredMaterialFromBasicEffect(iMeshPart.Effect as SkinnedModelBasicEffect, out this.mSkinnedModelMaterial);
			}

			// Token: 0x0400191D RID: 6429
			protected VertexDeclaration mVertexDeclaration;

			// Token: 0x0400191E RID: 6430
			protected int mBaseVertex;

			// Token: 0x0400191F RID: 6431
			protected int mNumVertices;

			// Token: 0x04001920 RID: 6432
			protected int mPrimitiveCount;

			// Token: 0x04001921 RID: 6433
			protected int mStartIndex;

			// Token: 0x04001922 RID: 6434
			protected int mStreamOffset;

			// Token: 0x04001923 RID: 6435
			protected int mVertexStride;

			// Token: 0x04001924 RID: 6436
			public float mDamage;

			// Token: 0x04001925 RID: 6437
			protected VertexBuffer mVertexBuffer;

			// Token: 0x04001926 RID: 6438
			protected IndexBuffer mIndexBuffer;

			// Token: 0x04001927 RID: 6439
			public Matrix[] mBones;

			// Token: 0x04001928 RID: 6440
			public BoundingSphere mBoundingSphere;

			// Token: 0x04001929 RID: 6441
			public SkinnedModelDeferredBasicMaterial mSkinnedModelMaterial;

			// Token: 0x0400192A RID: 6442
			public int mVerticesHash;
		}

		// Token: 0x020002F6 RID: 758
		public class HitListWithBarriers
		{
			// Token: 0x0600178B RID: 6027 RVA: 0x0009AF34 File Offset: 0x00099134
			public static void InitializeCache(int iNrOfBarriers)
			{
				Barrier.HitListWithBarriers.sInstances = new List<Barrier.HitListWithBarriers>(64);
				Barrier.HitListWithBarriers.sHitListCache = new List<Barrier.HitListWithBarriers>(iNrOfBarriers);
				for (int i = 0; i < iNrOfBarriers; i++)
				{
					Barrier.HitListWithBarriers.sHitListCache.Add(new Barrier.HitListWithBarriers());
				}
			}

			// Token: 0x0600178C RID: 6028 RVA: 0x0009AF73 File Offset: 0x00099173
			private HitListWithBarriers()
			{
				this.mHandle = (ushort)Barrier.HitListWithBarriers.sInstances.Count;
				Barrier.HitListWithBarriers.sInstances.Add(this);
			}

			// Token: 0x0600178D RID: 6029 RVA: 0x0009AFB4 File Offset: 0x000991B4
			public static Barrier.HitListWithBarriers GetFromCache()
			{
				lock (Barrier.HitListWithBarriers.sHitListCache)
				{
					if (Barrier.HitListWithBarriers.sHitListCache.Count == 0)
					{
						return new Barrier.HitListWithBarriers();
					}
				}
				int index = Barrier.HitListWithBarriers.sHitListCache.Count - 1;
				Barrier.HitListWithBarriers result = Barrier.HitListWithBarriers.sHitListCache[index];
				Barrier.HitListWithBarriers.sHitListCache.RemoveAt(index);
				return result;
			}

			// Token: 0x0600178E RID: 6030 RVA: 0x0009B024 File Offset: 0x00099224
			public static Barrier.HitListWithBarriers GetByHandle(ushort iHandle)
			{
				Barrier.HitListWithBarriers hitListWithBarriers = Barrier.HitListWithBarriers.sInstances[(int)iHandle];
				Barrier.HitListWithBarriers.sHitListCache.Remove(hitListWithBarriers);
				return hitListWithBarriers;
			}

			// Token: 0x0600178F RID: 6031 RVA: 0x0009B04A File Offset: 0x0009924A
			public void Destroy()
			{
				if (!Barrier.HitListWithBarriers.sHitListCache.Contains(this))
				{
					Barrier.HitListWithBarriers.sHitListCache.Add(this);
				}
			}

			// Token: 0x170005FE RID: 1534
			// (get) Token: 0x06001790 RID: 6032 RVA: 0x0009B064 File Offset: 0x00099264
			public ushort Handle
			{
				get
				{
					return this.mHandle;
				}
			}

			// Token: 0x170005FF RID: 1535
			// (get) Token: 0x06001791 RID: 6033 RVA: 0x0009B06C File Offset: 0x0009926C
			public List<Barrier> Owners
			{
				get
				{
					return this.mOwners;
				}
			}

			// Token: 0x17000600 RID: 1536
			// (get) Token: 0x06001792 RID: 6034 RVA: 0x0009B074 File Offset: 0x00099274
			public HitList HitList
			{
				get
				{
					return this.mHitList;
				}
			}

			// Token: 0x0400192B RID: 6443
			private static List<Barrier.HitListWithBarriers> sInstances;

			// Token: 0x0400192C RID: 6444
			private static List<Barrier.HitListWithBarriers> sHitListCache;

			// Token: 0x0400192D RID: 6445
			private ushort mHandle;

			// Token: 0x0400192E RID: 6446
			private HitList mHitList = new HitList(64);

			// Token: 0x0400192F RID: 6447
			private List<Barrier> mOwners = new List<Barrier>(32);
		}

		// Token: 0x020002F7 RID: 759
		[Flags]
		protected enum DrawMethod
		{
			// Token: 0x04001931 RID: 6449
			NONE = 0,
			// Token: 0x04001932 RID: 6450
			NORMAL = 1,
			// Token: 0x04001933 RID: 6451
			PARTICLEWALL = 2,
			// Token: 0x04001934 RID: 6452
			PARTICLEFOUNTAIN = 4
		}

		// Token: 0x020002F8 RID: 760
		protected enum BarrierType
		{
			// Token: 0x04001936 RID: 6454
			SOLID,
			// Token: 0x04001937 RID: 6455
			ELEMENTAL
		}
	}
}
