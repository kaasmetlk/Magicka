using System;
using System.Collections.Generic;
using JigLibX.Collision;
using JigLibX.Geometry;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Levels.Triggers;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

namespace Magicka.GameLogic.Entities.Bosses
{
	// Token: 0x020005EF RID: 1519
	internal class Grimnir2 : BossStatusEffected, IBossSpellCaster, IBoss
	{
		// Token: 0x06002E01 RID: 11777 RVA: 0x001752F4 File Offset: 0x001734F4
		static Grimnir2()
		{
			Grimnir2.SpellData spellData;
			spellData.SPELL = default(Spell);
			spellData.SPELL.Element = (Elements.Lightning | Elements.Arcane);
			spellData.SPELL.ArcaneMagnitude = 1f;
			spellData.SPELL.LightningMagnitude = 3f;
			spellData.CASTTYPE = CastType.Force;
			spellData.SPELLPOWER = 1f;
			Grimnir2.SPIRITSPELLS[0] = spellData;
			spellData.SPELL = default(Spell);
			spellData.SPELL.Element = (Elements.Earth | Elements.Fire);
			spellData.SPELL.EarthMagnitude = 2f;
			spellData.SPELL.FireMagnitude = 2f;
			spellData.CASTTYPE = CastType.Force;
			spellData.SPELLPOWER = 0.1f;
			Grimnir2.SPIRITSPELLS[1] = spellData;
			spellData.SPELL = default(Spell);
			spellData.SPELL.Element = Elements.Ice;
			spellData.SPELL.IceMagnitude = 5f;
			spellData.CASTTYPE = CastType.Area;
			spellData.SPELLPOWER = 1f;
			Grimnir2.GRIMNIRSPELLS[0] = spellData;
			spellData.SPELL = default(Spell);
			spellData.SPELL.Element = (Elements.Earth | Elements.Shield);
			spellData.SPELL.ShieldMagnitude = 1f;
			spellData.SPELL.EarthMagnitude = 4f;
			spellData.CASTTYPE = CastType.Force;
			spellData.SPELLPOWER = 1f;
			Grimnir2.GRIMNIRSPELLS[1] = spellData;
			spellData.SPELL = default(Spell);
			spellData.SPELL.Element = Elements.Shield;
			spellData.SPELL.ShieldMagnitude = 1f;
			spellData.CASTTYPE = CastType.Force;
			spellData.SPELLPOWER = 1f;
			Grimnir2.GRIMNIRSPELLS[2] = spellData;
		}

		// Token: 0x06002E02 RID: 11778 RVA: 0x001755A0 File Offset: 0x001737A0
		public Grimnir2(PlayState iPlayState)
		{
			this.mPlayState = iPlayState;
			this.mGrimnirGibs = new GibReference[6];
			SkinnedModel skinnedModel;
			SkinnedModel skinnedModel2;
			SkinnedModel skinnedModel3;
			SkinnedModel skinnedModel4;
			lock (this.mPlayState.Scene.GraphicsDevice)
			{
				skinnedModel = this.mPlayState.Content.Load<SkinnedModel>("Models/Bosses/grimnir/grimnir_mesh");
				skinnedModel2 = this.mPlayState.Content.Load<SkinnedModel>("Models/Bosses/grimnir/grimnir_animation");
				skinnedModel3 = this.mPlayState.Content.Load<SkinnedModel>("Models/Bosses/grimnir/grimnir_mesh");
				skinnedModel4 = this.mPlayState.Content.Load<SkinnedModel>("Models/Bosses/assatur/assatur");
				this.mIceCubeMap = this.mPlayState.Content.Load<TextureCube>("EffectTextures/iceCube");
				this.mIceCubeNormalMap = this.mPlayState.Content.Load<TextureCube>("EffectTextures/iceCube_NRM");
			}
			for (int i = 0; i < skinnedModel2.SkeletonBones.Count; i++)
			{
				SkinnedModelBone skinnedModelBone = skinnedModel2.SkeletonBones[i];
				if (skinnedModelBone.Name.Equals("leftattach", StringComparison.OrdinalIgnoreCase))
				{
					this.mCastAttachIndex = (int)skinnedModelBone.Index;
				}
				else if (skinnedModelBone.Name.Equals("rightattach", StringComparison.OrdinalIgnoreCase))
				{
					this.mWeaponAttachIndex = (int)skinnedModelBone.Index;
				}
			}
			this.mAsaController = new AnimationController();
			this.mAsaController.Skeleton = skinnedModel4.SkeletonBones;
			this.mAsaClips = new AnimationClip[3];
			this.mAsaClips[0] = skinnedModel4.AnimationClips["idle"];
			this.mAsaClips[1] = skinnedModel4.AnimationClips["cast_self"];
			this.mAsaClips[2] = skinnedModel4.AnimationClips["cast_ground"];
			this.mController = new AnimationController();
			this.mController.Skeleton = skinnedModel2.SkeletonBones;
			this.mClips = new AnimationClip[15];
			this.mClips[0] = skinnedModel2.AnimationClips["die"];
			this.mClips[1] = skinnedModel2.AnimationClips["hanging"];
			this.mClips[2] = skinnedModel2.AnimationClips["taunt"];
			this.mClips[3] = skinnedModel2.AnimationClips["talk0"];
			this.mClips[4] = skinnedModel2.AnimationClips["talk1"];
			this.mClips[5] = skinnedModel2.AnimationClips["talk2"];
			this.mClips[6] = skinnedModel2.AnimationClips["idle"];
			this.mClips[8] = skinnedModel2.AnimationClips["spirit_cast_projectile"];
			this.mClips[9] = skinnedModel2.AnimationClips["spirit_cast_railgun"];
			this.mClips[7] = skinnedModel2.AnimationClips["spirit_idle"];
			this.mClips[10] = skinnedModel2.AnimationClips["spirit_die"];
			this.mClips[11] = skinnedModel2.AnimationClips["cast_magick_global"];
			this.mClips[12] = skinnedModel2.AnimationClips["cast_magick_direct"];
			this.mClips[13] = skinnedModel2.AnimationClips["summon_spirits"];
			this.mClips[14] = skinnedModel2.AnimationClips["cast_shield"];
			this.mLeftSpiritController = new AnimationController();
			this.mLeftSpiritController.Skeleton = skinnedModel3.SkeletonBones;
			this.mRightSpiritController = new AnimationController();
			this.mRightSpiritController.Skeleton = skinnedModel3.SkeletonBones;
			Capsule capsule = new Capsule(new Vector3(0f, -0.375f, 0f), Matrix.CreateRotationX(-1.5707964f), 0.75f, 1.5f);
			this.mGrimnirBody = new BossSpellCasterZone(this.mPlayState, this, this.mController, this.mCastAttachIndex, this.mWeaponAttachIndex, 0, 1f, new Primitive[]
			{
				capsule
			});
			this.mGrimnirBody.Body.CollisionSkin.callbackFn += this.OnCollision;
			this.mLeftSpiritBody = new BossSpellCasterZone(this.mPlayState, this, this.mLeftSpiritController, this.mCastAttachIndex, this.mWeaponAttachIndex, 1, 1.5f, new Primitive[]
			{
				capsule
			});
			this.mLeftSpiritBody.Body.CollisionSkin.callbackFn += this.OnCollision;
			this.mLeftSpiritBody.IsEthereal = true;
			this.mRightSpiritBody = new BossSpellCasterZone(this.mPlayState, this, this.mRightSpiritController, this.mCastAttachIndex, this.mWeaponAttachIndex, 2, 1.5f, new Primitive[]
			{
				capsule
			});
			this.mRightSpiritBody.Body.CollisionSkin.callbackFn += this.OnCollision;
			this.mRightSpiritBody.IsEthereal = true;
			SkinnedModelBasicEffect iEffect = skinnedModel.Model.Meshes[0].MeshParts[0].Effect as SkinnedModelBasicEffect;
			Helper.SkinnedModelDeferredMaterialFromBasicEffect(iEffect, out this.mGrimnirMaterial);
			iEffect = (skinnedModel3.Model.Meshes[0].MeshParts[0].Effect as SkinnedModelBasicEffect);
			SkinnedModelDeferredAdvancedMaterial mMaterial;
			Helper.SkinnedModelDeferredMaterialFromBasicEffect(iEffect, out mMaterial);
			iEffect = (skinnedModel4.Model.Meshes[0].MeshParts[0].Effect as SkinnedModelBasicEffect);
			SkinnedModelDeferredAdvancedMaterial mMaterial2;
			Helper.SkinnedModelDeferredMaterialFromBasicEffect(iEffect, out mMaterial2);
			this.mGrimnirBoundingSphere = skinnedModel.Model.Meshes[0].BoundingSphere;
			this.mSpiritBoundingSphere = new BoundingSphere(Vector3.Zero, 30f);
			this.mAsaBoundingSphere = skinnedModel4.Model.Meshes[0].BoundingSphere;
			this.mAsaRenderData = new Grimnir2.RenderData[3];
			this.mAdditiveAsaRenderData = new Grimnir2.AdditiveRenderData[3];
			this.mRenderData = new Grimnir2.RenderData[3];
			this.mAdditiveRenderData = new Grimnir2.AdditiveRenderData[3];
			for (int j = 0; j < 3; j++)
			{
				this.mRenderData[j] = new Grimnir2.RenderData();
				this.mRenderData[j].SetMesh(skinnedModel.Model.Meshes[0].VertexBuffer, skinnedModel.Model.Meshes[0].IndexBuffer, skinnedModel.Model.Meshes[0].MeshParts[0], 0, 3, 4);
				this.mRenderData[j].mMaterial = this.mGrimnirMaterial;
				this.mAdditiveRenderData[j] = new Grimnir2.AdditiveRenderData();
				this.mAdditiveRenderData[j].SetMesh(skinnedModel3.Model.Meshes[0].VertexBuffer, skinnedModel3.Model.Meshes[0].IndexBuffer, skinnedModel3.Model.Meshes[0].MeshParts[0], 2);
				this.mAdditiveRenderData[j].mMaterial = mMaterial;
				this.mAsaRenderData[j] = new Grimnir2.RenderData();
				this.mAsaRenderData[j].SetMesh(skinnedModel4.Model.Meshes[0].VertexBuffer, skinnedModel4.Model.Meshes[0].IndexBuffer, skinnedModel4.Model.Meshes[0].MeshParts[0], 0, 3, 4);
				this.mAsaRenderData[j].mMaterial = mMaterial2;
				this.mAdditiveAsaRenderData[j] = new Grimnir2.AdditiveRenderData();
				this.mAdditiveAsaRenderData[j].SetMesh(skinnedModel4.Model.Meshes[0].VertexBuffer, skinnedModel4.Model.Meshes[0].IndexBuffer, skinnedModel4.Model.Meshes[0].MeshParts[0], 2);
				this.mAdditiveAsaRenderData[j].mMaterial = mMaterial2;
			}
			for (int k = 0; k < this.mResistances.Length; k++)
			{
				Elements resistanceAgainst = Spell.ElementFromIndex(k);
				this.mResistances[k].Multiplier = 1f;
				this.mResistances[k].ResistanceAgainst = resistanceAgainst;
				this.mResistances[k].Modifier = 0f;
			}
			this.mLoopFight = false;
		}

		// Token: 0x06002E03 RID: 11779 RVA: 0x00175E14 File Offset: 0x00174014
		public bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
		{
			return false;
		}

		// Token: 0x06002E04 RID: 11780 RVA: 0x00175E17 File Offset: 0x00174017
		public void Initialize(ref Matrix iOrientation, int iUniqueID)
		{
			this.Initialize(ref iOrientation);
		}

		// Token: 0x06002E05 RID: 11781 RVA: 0x00175E20 File Offset: 0x00174020
		public void Initialize(ref Matrix iOrientation)
		{
			for (int i = 0; i < this.mStatusEffects.Length; i++)
			{
				this.mStatusEffects[i].Stop();
			}
			this.mOrientation = iOrientation;
			this.mGrimnirBoundingSphere.Center = this.mOrientation.Translation;
			this.mCalledAppearTrigger = false;
			this.mMaxHitPoints = 35000f;
			this.mHitPoints = 35000f;
			EffectManager.Instance.Stop(ref this.mCastEffect);
			this.mGrimnirBody.Initialize("#boss_n07".GetHashCodeCustom());
			this.mGrimnirBody.IsEthereal = false;
			this.mPlayState.EntityManager.AddEntity(this.mGrimnirBody);
			this.mLeftSpiritBody.Initialize();
			this.mPlayState.EntityManager.AddEntity(this.mLeftSpiritBody);
			this.mRightSpiritBody.Initialize();
			this.mPlayState.EntityManager.AddEntity(this.mRightSpiritBody);
			this.mAsaController.StartClip(this.mAsaClips[0], true);
			this.mAssaturAlpha = 0f;
			this.mAssaturOrientation = iOrientation;
			Vector3 backward = iOrientation.Backward;
			Vector3.Multiply(ref backward, 5f, out backward);
			backward.Y -= 6f;
			this.mAssaturOrientation.Translation = iOrientation.Translation + backward;
			this.mCurrentState = Grimnir2.IntroState.Instance;
			this.mCurrentState.OnEnter(this);
			this.mLeftSpiritState = Grimnir2.SpecIntroState.Instance;
			this.mLeftSpiritState.OnEnter(this.mLeftSpiritBody, this);
			this.mRightSpiritState = Grimnir2.SpecIntroState.Instance;
			this.mRightSpiritState.OnEnter(this.mRightSpiritBody, this);
			Player[] players = Game.Instance.Players;
			for (int j = 0; j < players.Length; j++)
			{
				if (players[j].Playing && players[j].Avatar != null)
				{
					this.mTarget = players[j].Avatar;
					break;
				}
			}
			this.mTargetPosition = this.mTarget.Position;
			this.mTargetSwapTimer = 30f;
			this.mLeftFloatCounter = 0f;
			this.mHitPoints = 35000f;
			this.mAssaturHeal = false;
			this.mCorporeal = false;
			this.mSpectralAlpha = 0f;
			this.mRightFloatCounter = 0f;
			this.mLeftFloatCounter = 0f;
			this.mLeftSpiritBody.IsEthereal = true;
			this.mRightSpiritBody.IsEthereal = true;
			this.mSpiritTimer = 25.1f;
			this.mLoopFight = true;
			foreach (AnimatedLevelPart animatedLevelPart in this.mPlayState.Level.CurrentScene.LevelModel.AnimatedLevelParts.Values)
			{
				if (animatedLevelPart.ID == "cliff".GetHashCodeCustom())
				{
					this.mCliffOrientation = animatedLevelPart.CollisionSkin.NewTransform.Orientation;
					this.mCliffOrientation.Translation = animatedLevelPart.CollisionSkin.NewPosition;
					break;
				}
			}
		}

		// Token: 0x06002E06 RID: 11782 RVA: 0x0017613C File Offset: 0x0017433C
		public unsafe void UpdateBoss(DataChannel iDataChannel, float iDeltaTime, bool iFightStarted)
		{
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				this.mNetworkUpdateTimer -= iDeltaTime;
				if (this.mNetworkUpdateTimer <= 0f)
				{
					this.mNetworkUpdateTimer = 0.033333335f;
					this.NetworkUpdate();
				}
			}
			this.mTargetSwapTimer -= iDeltaTime;
			this.mCurrentState.OnUpdate(iDeltaTime, this);
			this.mTimeSinceLastDamage += iDeltaTime;
			if (iFightStarted && this.mCurrentState is Grimnir2.IntroState)
			{
				this.ChangeState(Grimnir2.States.Idle);
				this.ChangeSpiritState(Grimnir2.SpiritStates.Idle, 1);
				this.ChangeSpiritState(Grimnir2.SpiritStates.Idle, 2);
			}
			Vector3 translation = this.mOrientation.Translation;
			translation.Y += 1.5f;
			Matrix orientation = this.mOrientation;
			orientation.Translation = default(Vector3);
			this.mGrimnirBody.Body.MoveTo(translation, orientation);
			orientation = this.mOrientation;
			foreach (AnimatedLevelPart animatedLevelPart in this.mPlayState.Level.CurrentScene.LevelModel.AnimatedLevelParts.Values)
			{
				if (animatedLevelPart.ID == "cliff".GetHashCodeCustom() && animatedLevelPart.Time > 0f)
				{
					Matrix orientation2 = animatedLevelPart.CollisionSkin.NewTransform.Orientation;
					orientation2.Translation = animatedLevelPart.CollisionSkin.NewPosition;
					Matrix matrix;
					Matrix.Invert(ref this.mCliffOrientation, out matrix);
					Matrix matrix2;
					Matrix.Multiply(ref orientation2, ref matrix, out matrix2);
					Matrix matrix3;
					Matrix.Multiply(ref matrix2, ref orientation, out matrix3);
					orientation = matrix3;
					break;
				}
			}
			this.UpdateDamage(iDeltaTime);
			this.UpdateStatusEffects(iDeltaTime);
			this.mController.Speed = 1f;
			if (base.HasStatus(StatusEffects.Frozen))
			{
				this.mRenderData[(int)iDataChannel].mMaterial.Bloat = 0.1f;
				this.mRenderData[(int)iDataChannel].mMaterial.EmissiveAmount = 3f;
				this.mRenderData[(int)iDataChannel].mMaterial.SpecularBias = 0.8f;
				this.mRenderData[(int)iDataChannel].mMaterial.SpecularPower = 20f;
				this.mRenderData[(int)iDataChannel].mMaterial.CubeMapRotation = Matrix.Identity;
				this.mRenderData[(int)iDataChannel].mMaterial.CubeMap = this.mIceCubeMap;
				this.mRenderData[(int)iDataChannel].mMaterial.CubeNormalMap = this.mIceCubeNormalMap;
				this.mRenderData[(int)iDataChannel].mMaterial.CubeMapColor = Vector4.One;
				this.mRenderData[(int)iDataChannel].mMaterial.CubeMapEnabled = true;
				this.mRenderData[(int)iDataChannel].mMaterial.CubeNormalMapEnabled = true;
				this.mRenderData[(int)iDataChannel].mMaterial.CubeMapColor.W = 1f - (float)Math.Pow(0.20000000298023224, (double)base.StatusMagnitude(StatusEffects.Frozen));
				this.mController.Speed = 0f;
			}
			else
			{
				if (base.HasStatus(StatusEffects.Cold))
				{
					this.mController.Speed *= 0.2f;
				}
				this.mRenderData[(int)iDataChannel].mMaterial.Bloat = 0f;
				this.mRenderData[(int)iDataChannel].mMaterial.EmissiveAmount = this.mGrimnirMaterial.EmissiveAmount;
				this.mRenderData[(int)iDataChannel].mMaterial.SpecularBias = this.mGrimnirMaterial.SpecularBias;
				this.mRenderData[(int)iDataChannel].mMaterial.SpecularPower = this.mGrimnirMaterial.SpecularPower;
				this.mRenderData[(int)iDataChannel].mMaterial.CubeMapEnabled = false;
				this.mRenderData[(int)iDataChannel].mMaterial.CubeNormalMapEnabled = false;
			}
			this.mController.Update(iDeltaTime, ref orientation, true);
			this.mController.SkinnedBoneTransforms.CopyTo(this.mRenderData[(int)iDataChannel].mBones, 0);
			this.mDamageFlashTimer = Math.Max(this.mDamageFlashTimer - iDeltaTime, 0f);
			this.mRenderData[(int)iDataChannel].Flash = this.mDamageFlashTimer * 10f;
			this.mRenderData[(int)iDataChannel].mBoundingSphere = this.mGrimnirBoundingSphere;
			this.mPlayState.Scene.AddRenderableObject(iDataChannel, this.mRenderData[(int)iDataChannel]);
			if (NetworkManager.Instance.State != NetworkState.Client && (this.mTargetSwapTimer <= 0f || this.mTarget == null || this.mTarget.Dead))
			{
				this.mTargetSwapTimer = 30f;
				int num = Grimnir2.RANDOM.Next(Game.Instance.PlayerCount);
				Player[] players = Game.Instance.Players;
				int i = 0;
				while (i < players.Length)
				{
					if (players[(i + num) % 4].Playing && players[(i + num) % 4].Avatar != null)
					{
						this.mTarget = players[(i + num) % 4].Avatar;
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							Grimnir2.ChangeTargetMessage changeTargetMessage;
							changeTargetMessage.Target = this.mTarget.Handle;
							BossFight.Instance.SendMessage<Grimnir2.ChangeTargetMessage>(this, 7, (void*)(&changeTargetMessage), true);
							break;
						}
						break;
					}
					else
					{
						i++;
					}
				}
			}
			if (this.mGrimnirSpellEffect != null)
			{
				if (this.mGrimnirSpellEffect.CastType == CastType.Weapon)
				{
					this.mGrimnirSpellEffect.AnimationEnd(this.mGrimnirBody);
				}
				float num2;
				if (!this.mGrimnirSpellEffect.CastUpdate(iDeltaTime, this.mGrimnirBody, out num2))
				{
					this.mGrimnirSpellEffect.DeInitialize(this.mGrimnirBody);
					this.mGrimnirSpellEffect = null;
				}
			}
			if (this.mLeftSpiritSpellEffect != null)
			{
				if (this.mLeftSpiritSpellEffect.CastType == CastType.Weapon)
				{
					this.mLeftSpiritSpellEffect.AnimationEnd(this.mLeftSpiritBody);
				}
				float num3;
				if (!this.mLeftSpiritSpellEffect.CastUpdate(iDeltaTime, this.mLeftSpiritBody, out num3))
				{
					this.mLeftSpiritSpellEffect.DeInitialize(this.mGrimnirBody);
					this.mLeftSpiritSpellEffect = null;
				}
			}
			if (this.mRightSpiritSpellEffect != null)
			{
				if (this.mRightSpiritSpellEffect.CastType == CastType.Weapon)
				{
					this.mRightSpiritSpellEffect.AnimationEnd(this.mRightSpiritBody);
				}
				float num4;
				if (!this.mRightSpiritSpellEffect.CastUpdate(iDeltaTime, this.mRightSpiritBody, out num4))
				{
					this.mRightSpiritSpellEffect.DeInitialize(this.mGrimnirBody);
					this.mRightSpiritSpellEffect = null;
				}
			}
			this.mTargetPosition.X = this.mTargetPosition.X + (this.mTarget.Position.X - this.mTargetPosition.X) * iDeltaTime * 2f;
			this.mTargetPosition.Y = this.mTargetPosition.Y + (this.mTarget.Position.Y - this.mTargetPosition.Y) * iDeltaTime * 2f;
			this.mTargetPosition.Z = this.mTargetPosition.Z + (this.mTarget.Position.Z - this.mTargetPosition.Z) * iDeltaTime * 2f;
			if (!this.mAssaturHeal)
			{
				this.mSpiritTimer += iDeltaTime;
				this.mRightFloatCounter += iDeltaTime * 0.05f;
				this.mLeftFloatCounter += iDeltaTime * 0.075f;
			}
			if (this.mSpiritTimer <= 25f)
			{
				this.mSpiritBoundingSphere.Radius = 40f;
				this.mSpiritBoundingSphere.Center = this.mOrientation.Translation;
				this.mAdditiveRenderData[(int)iDataChannel].mBoundingSphere = this.mSpiritBoundingSphere;
				if (this.mSpiritTimer > 25f)
				{
					this.ChangeSpiritState(Grimnir2.SpiritStates.Idle, 1);
					if (this.mUseBothSpirits)
					{
						this.ChangeSpiritState(Grimnir2.SpiritStates.Idle, 2);
					}
				}
				this.mLeftSpiritState.OnUpdate(iDeltaTime, this.mLeftSpiritBody, this);
				this.mRightSpiritState.OnUpdate(iDeltaTime, this.mRightSpiritBody, this);
				float num5 = (float)Math.Pow(Math.Sin((double)this.mLeftFloatCounter), 2.0);
				float num6 = (float)Math.Pow(Math.Sin((double)this.mRightFloatCounter), 2.0);
				float num7 = Math.Min(this.mSpiritTimer * 0.25f, 1f);
				Vector3 forward = this.mOrientation.Forward;
				Vector3.Multiply(ref forward, num7 * (10f + num6 * 0.25f), out forward);
				Matrix matrix4;
				Matrix.CreateRotationY(3.1415927f + Math.Abs(num6) * num7 * 1.5707964f + 0.62831855f * num7, out matrix4);
				Matrix.Multiply(ref matrix4, ref this.mOrientation, out this.mLeftSpecOrientation);
				translation = this.mLeftSpecOrientation.Translation;
				Vector3.Transform(ref forward, ref matrix4, out forward);
				Vector3.Add(ref translation, ref forward, out translation);
				translation.Y = this.mTargetPosition.Y - 1.5f * num7;
				Matrix.Multiply(ref this.YROT, ref this.mLeftSpecOrientation, out this.mLeftSpecOrientation);
				Vector3 backward = this.mLeftSpecOrientation.Backward;
				Vector3.Multiply(ref backward, num7 * 10f, out backward);
				Vector3 forward2 = this.mOrientation.Forward;
				Vector3.Multiply(ref forward2, num7 * 10f, out forward2);
				Vector3.Add(ref forward2, ref translation, out translation);
				this.mLeftSpecOrientation.Translation = translation;
				Vector3 up = Vector3.Up;
				Matrix.CreateConstrainedBillboard(ref translation, ref this.mTargetPosition, ref up, null, null, out orientation);
				orientation.Translation = default(Vector3);
				translation.Y += 0.75f;
				this.mLeftSpiritBody.Body.MoveTo(translation, orientation);
				translation.Y -= 0.75f;
				orientation.Translation = translation;
				this.mLeftSpiritBody.IsEthereal = true;
				this.mLeftSpiritController.Update(iDeltaTime, ref orientation, true);
				this.mLeftSpiritController.SkinnedBoneTransforms.CopyTo(this.mAdditiveRenderData[(int)iDataChannel].mLeftSpecBones, 0);
				this.mAdditiveRenderData[(int)iDataChannel].RenderLeftSpectral = true;
				if (!this.mUseBothSpirits)
				{
					this.mAdditiveRenderData[(int)iDataChannel].RenderRightSpectral = false;
				}
				else
				{
					forward = this.mOrientation.Forward;
					Vector3.Multiply(ref forward, num7 * (10f + num5 * 0.5f), out forward);
					Matrix.CreateRotationY(3.1415927f - Math.Abs(num5) * num7 * 1.5707964f - 0.62831855f * num7, out matrix4);
					Matrix.Multiply(ref matrix4, ref this.mOrientation, out this.mRightSpecOrientation);
					translation = this.mRightSpecOrientation.Translation;
					Vector3.Transform(ref forward, ref matrix4, out forward);
					Vector3.Add(ref translation, ref forward, out translation);
					translation.Y = this.mTargetPosition.Y - 1.5f * num7;
					Matrix.Multiply(ref this.YROT, ref this.mRightSpecOrientation, out this.mRightSpecOrientation);
					backward = this.mRightSpecOrientation.Backward;
					Vector3.Multiply(ref backward, num7 * 10f, out backward);
					forward2 = this.mOrientation.Forward;
					Vector3.Multiply(ref forward2, num7 * 10f, out forward2);
					Vector3.Add(ref forward2, ref translation, out translation);
					this.mRightSpecOrientation.Translation = translation;
					Matrix.CreateConstrainedBillboard(ref translation, ref this.mTargetPosition, ref up, null, null, out orientation);
					orientation.Translation = default(Vector3);
					translation.Y += 0.75f;
					this.mRightSpiritBody.Body.MoveTo(translation, orientation);
					translation.Y -= 0.75f;
					orientation.Translation = translation;
					this.mRightSpiritBody.IsEthereal = true;
					this.mRightSpiritController.Update(iDeltaTime, ref orientation, true);
					this.mRightSpiritController.SkinnedBoneTransforms.CopyTo(this.mAdditiveRenderData[(int)iDataChannel].mRightSpecBones, 0);
					this.mAdditiveRenderData[(int)iDataChannel].RenderRightSpectral = true;
				}
				this.mSpectralAlpha += (this.mSpectralTargetAlpha - this.mSpectralAlpha) * iDeltaTime;
				float num8 = Math.Min(Math.Min(this.mSpiritTimer, 1f) * Math.Max(25f - this.mSpiritTimer, 0f), 1f);
				num8 *= this.mSpectralAlpha;
				this.mAdditiveRenderData[(int)iDataChannel].mMaterial.Alpha = num8;
				this.mAdditiveRenderData[(int)iDataChannel].mMaterial.Colorize = Grimnir2.AdditiveRenderData.ColdColor;
				this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, this.mAdditiveRenderData[(int)iDataChannel]);
			}
			else
			{
				if (this.mLeftSpiritSpellEffect != null)
				{
					this.mLeftSpiritSpellEffect.Stop(this.mLeftSpiritBody);
					this.mLeftSpiritSpellEffect.DeInitialize(this.mLeftSpiritBody);
					this.mLeftSpiritSpellEffect = null;
				}
				if (this.mRightSpiritSpellEffect != null)
				{
					this.mRightSpiritSpellEffect.Stop(this.mRightSpiritBody);
					this.mRightSpiritSpellEffect.DeInitialize(this.mRightSpiritBody);
					this.mRightSpiritSpellEffect = null;
				}
			}
			if (this.mAssaturHeal)
			{
				if (this.mCurrentState != Grimnir2.HealState.Instance)
				{
					this.mCalledAppearTrigger = false;
					this.ChangeState(Grimnir2.States.Heal);
					this.ChangeSpiritState(Grimnir2.SpiritStates.Heal, 1);
					this.ChangeSpiritState(Grimnir2.SpiritStates.Heal, 2);
				}
				this.mAssaturAlpha = Math.Min(this.mAssaturAlpha + iDeltaTime * 0.25f, 1f);
				if (this.mAssaturAlpha >= 1f)
				{
					if (this.mLoopFight && !this.mCalledAppearTrigger)
					{
						this.mPlayState.Level.CurrentScene.ExecuteTrigger(Grimnir2.ASSATUR_APPEARS_TRIGGER, null, false);
						this.mCalledAppearTrigger = true;
					}
					if (this.mAsaController.AnimationClip == this.mAsaClips[0] && !this.mAsaController.CrossFadeEnabled)
					{
						this.mAsaController.CrossFade(this.mAsaClips[1], 0.5f, true);
					}
				}
			}
			else
			{
				this.mAssaturAlpha = Math.Max(this.mAssaturAlpha - iDeltaTime * 2f, 0f);
			}
			orientation = this.mAssaturOrientation;
			translation = this.mAssaturOrientation.Translation;
			translation.Y += (float)Math.Sin((double)this.mRightFloatCounter) * 0.5f;
			orientation.Translation = translation;
			this.mAsaBoundingSphere.Center = orientation.Translation;
			this.mAsaController.Update(iDeltaTime, ref orientation, true);
			if (this.mCorporeal)
			{
				this.mAsaController.SkinnedBoneTransforms.CopyTo(this.mAsaRenderData[(int)iDataChannel].mBones, 0);
				this.mAsaRenderData[(int)iDataChannel].mBoundingSphere = this.mAsaBoundingSphere;
				this.mPlayState.Scene.AddRenderableObject(iDataChannel, this.mAsaRenderData[(int)iDataChannel]);
				return;
			}
			this.mAdditiveAsaRenderData[(int)iDataChannel].mBoundingSphere = this.mAsaBoundingSphere;
			this.mAdditiveAsaRenderData[(int)iDataChannel].mMaterial.Alpha = this.mAssaturAlpha * 0.25f;
			this.mAdditiveAsaRenderData[(int)iDataChannel].RenderLeftSpectral = true;
			this.mAdditiveAsaRenderData[(int)iDataChannel].RenderRightSpectral = false;
			this.mAdditiveAsaRenderData[(int)iDataChannel].mMaterial.Colorize = Grimnir2.AdditiveRenderData.ColdColor;
			this.mAsaController.SkinnedBoneTransforms.CopyTo(this.mAdditiveAsaRenderData[(int)iDataChannel].mLeftSpecBones, 0);
			this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, this.mAdditiveAsaRenderData[(int)iDataChannel]);
		}

		// Token: 0x06002E07 RID: 11783 RVA: 0x00177004 File Offset: 0x00175204
		private IBossState<Grimnir2> GetState(Grimnir2.States iState)
		{
			switch (iState)
			{
			case Grimnir2.States.Idle:
				return Grimnir2.IdleState.Instance;
			case Grimnir2.States.Spirit:
				return Grimnir2.SpiritState.Instance;
			case Grimnir2.States.Magick:
				return Grimnir2.CastMagickState.Instance;
			case Grimnir2.States.Spell:
				return Grimnir2.CastSpellState.Instance;
			case Grimnir2.States.Heal:
				return Grimnir2.HealState.Instance;
			case Grimnir2.States.Die:
				return Grimnir2.DieState.Instance;
			default:
				return null;
			}
		}

		// Token: 0x06002E08 RID: 11784 RVA: 0x00177058 File Offset: 0x00175258
		protected unsafe void ChangeState(Grimnir2.States iState)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				IBossState<Grimnir2> state = this.GetState(iState);
				if (state == null)
				{
					return;
				}
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					Grimnir2.ChangeStateMessage changeStateMessage;
					changeStateMessage.NewState = iState;
					BossFight.Instance.SendMessage<Grimnir2.ChangeStateMessage>(this, 2, (void*)(&changeStateMessage), true);
				}
				this.mCurrentState.OnExit(this);
				this.mCurrentState = state;
				this.mCurrentState.OnEnter(this);
			}
		}

		// Token: 0x06002E09 RID: 11785 RVA: 0x001770C4 File Offset: 0x001752C4
		private Grimnir2.ISpiritState GetSpiritState(Grimnir2.SpiritStates iState)
		{
			switch (iState)
			{
			case Grimnir2.SpiritStates.Intro:
				return Grimnir2.SpecIntroState.Instance;
			case Grimnir2.SpiritStates.Idle:
				return Grimnir2.SpecIdleState.Instance;
			case Grimnir2.SpiritStates.Cast:
				return Grimnir2.SpecCastState.Instance;
			case Grimnir2.SpiritStates.Heal:
				return Grimnir2.SpecHealState.Instance;
			case Grimnir2.SpiritStates.Die:
				return Grimnir2.SpecDieState.Instance;
			default:
				return null;
			}
		}

		// Token: 0x06002E0A RID: 11786 RVA: 0x00177110 File Offset: 0x00175310
		protected unsafe void ChangeSpiritState(Grimnir2.SpiritStates iState, int iIndex)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				Grimnir2.ISpiritState spiritState = this.GetSpiritState(iState);
				if (spiritState == null)
				{
					return;
				}
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					Grimnir2.ChangeSpiritStateMessage changeSpiritStateMessage;
					changeSpiritStateMessage.NewState = iState;
					changeSpiritStateMessage.SpiritIndex = iIndex;
					BossFight.Instance.SendMessage<Grimnir2.ChangeSpiritStateMessage>(this, 3, (void*)(&changeSpiritStateMessage), true);
				}
				switch (iIndex)
				{
				default:
					this.mLeftSpiritState.OnExit(this.mLeftSpiritBody, this);
					this.mLeftSpiritState = spiritState;
					this.mLeftSpiritState.OnEnter(this.mLeftSpiritBody, this);
					return;
				case 2:
					this.mRightSpiritState.OnExit(this.mRightSpiritBody, this);
					this.mRightSpiritState = spiritState;
					this.mRightSpiritState.OnEnter(this.mRightSpiritBody, this);
					break;
				}
			}
		}

		// Token: 0x06002E0B RID: 11787 RVA: 0x001771CF File Offset: 0x001753CF
		public void Nullify()
		{
			if (this.mSpiritTimer <= 25f)
			{
				this.mSpiritTimer = 24f;
				this.ChangeSpiritState(Grimnir2.SpiritStates.Idle, 1);
				if (this.mUseBothSpirits)
				{
					this.ChangeSpiritState(Grimnir2.SpiritStates.Idle, 2);
				}
			}
		}

		// Token: 0x06002E0C RID: 11788 RVA: 0x00177204 File Offset: 0x00175404
		public unsafe void Corporealize()
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					Grimnir2.CorpMessage corpMessage = default(Grimnir2.CorpMessage);
					BossFight.Instance.SendMessage<Grimnir2.CorpMessage>(this, 8, (void*)(&corpMessage), true);
				}
				if (this.mSpiritTimer <= 25f)
				{
					this.mSpiritTimer = 24f;
				}
				if (this.mAssaturHeal)
				{
					Trigger trigger;
					if (this.mPlayState.Level.CurrentScene.Triggers.TryGetValue(Grimnir2.ASSATUR_TALKS_TRIGGER, out trigger))
					{
						this.mPlayState.Level.CurrentScene.ExecuteTrigger(Grimnir2.ASSATUR_TALKS_TRIGGER, null, false);
						this.ChangeState(Grimnir2.States.Die);
						this.ChangeSpiritState(Grimnir2.SpiritStates.Die, 1);
						this.ChangeSpiritState(Grimnir2.SpiritStates.Die, 2);
						this.mAssaturHeal = false;
						this.mCorporeal = true;
						this.mAsaController.CrossFade(this.mAsaClips[0], 1f, true);
						Magick magick = default(Magick);
						magick.MagickType = MagickType.Nullify;
						magick.Effect.Execute(this.mGrimnirBody, this.mPlayState);
						return;
					}
					this.mCorporeal = true;
				}
			}
		}

		// Token: 0x06002E0D RID: 11789 RVA: 0x0017731C File Offset: 0x0017551C
		protected override DamageResult Damage(Damage iDamage, Entity iAttacker, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			return base.Damage(iDamage, iAttacker, iAttackPosition, iFeatures);
		}

		// Token: 0x06002E0E RID: 11790 RVA: 0x00177338 File Offset: 0x00175538
		public DamageResult Damage(int iPartIndex, Damage iDamage, Entity iAttacker, ref Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			if (iPartIndex != 0 || this.mCurrentState is Grimnir2.DieState)
			{
				return DamageResult.None;
			}
			if (this.mAssaturHeal)
			{
				return DamageResult.None;
			}
			DamageResult damageResult = this.Damage(iDamage, iAttacker, iAttackPosition, iFeatures);
			if ((damageResult & DamageResult.Hit) == DamageResult.Hit | (damageResult & DamageResult.Damaged) == DamageResult.Damaged)
			{
				this.mTimeSinceLastDamage = 0f;
				this.mDamageFlashTimer = 0.1f;
			}
			if (this.mHitPoints < 3500f)
			{
				this.mAssaturHeal = true;
				this.mHitPoints = 3500f;
			}
			return damageResult;
		}

		// Token: 0x06002E0F RID: 11791 RVA: 0x001773B8 File Offset: 0x001755B8
		public void Damage(int iPartIndex, float iDamage, Elements iElement)
		{
			if (iPartIndex == 0)
			{
				base.Damage(iDamage, iElement);
				if (this.mHitPoints < 3500f)
				{
					this.mAssaturHeal = true;
					this.mHitPoints = 3500f;
				}
				this.mTimeSinceLastStatusDamage = 0f;
				if (this.mDamageFlashTimer <= 0f)
				{
					this.mDamageFlashTimer = 0.1f;
				}
			}
		}

		// Token: 0x06002E10 RID: 11792 RVA: 0x00177412 File Offset: 0x00175612
		public void AddSelfShield(int iIndex, Spell iSpell)
		{
		}

		// Token: 0x06002E11 RID: 11793 RVA: 0x00177414 File Offset: 0x00175614
		public void RemoveSelfShield(int iIndex, Character.SelfShieldType iType)
		{
		}

		// Token: 0x06002E12 RID: 11794 RVA: 0x00177418 File Offset: 0x00175618
		CastType IBossSpellCaster.CastType(int iIndex)
		{
			switch (iIndex)
			{
			default:
				return this.mGrimnirCastType;
			case 1:
				return this.mLeftSpiritCastType;
			case 2:
				return this.mRightSpiritCastType;
			}
		}

		// Token: 0x06002E13 RID: 11795 RVA: 0x00177450 File Offset: 0x00175650
		float IBossSpellCaster.SpellPower(int iIndex)
		{
			switch (iIndex)
			{
			default:
				return this.mGrimnirSpellPower;
			case 1:
				return this.mLeftSpiritSpellPower;
			case 2:
				return this.mRightSpiritSpellPower;
			}
		}

		// Token: 0x06002E14 RID: 11796 RVA: 0x00177488 File Offset: 0x00175688
		void IBossSpellCaster.SpellPower(int iIndex, float iSpellPower)
		{
			switch (iIndex)
			{
			default:
				this.mGrimnirSpellPower = iSpellPower;
				return;
			case 1:
				this.mLeftSpiritSpellPower = iSpellPower;
				return;
			case 2:
				this.mRightSpiritSpellPower = iSpellPower;
				return;
			}
		}

		// Token: 0x06002E15 RID: 11797 RVA: 0x001774C0 File Offset: 0x001756C0
		SpellEffect IBossSpellCaster.CurrentSpell(int iIndex)
		{
			switch (iIndex)
			{
			default:
				return this.mGrimnirSpellEffect;
			case 1:
				return this.mLeftSpiritSpellEffect;
			case 2:
				return this.mRightSpiritSpellEffect;
			}
		}

		// Token: 0x06002E16 RID: 11798 RVA: 0x001774F8 File Offset: 0x001756F8
		void IBossSpellCaster.CurrentSpell(int iIndex, SpellEffect iEffect)
		{
			switch (iIndex)
			{
			default:
				this.mGrimnirSpellEffect = iEffect;
				return;
			case 1:
				this.mLeftSpiritSpellEffect = iEffect;
				return;
			case 2:
				this.mRightSpiritSpellEffect = iEffect;
				return;
			}
		}

		// Token: 0x06002E17 RID: 11799 RVA: 0x00177530 File Offset: 0x00175730
		public void DeInitialize()
		{
		}

		// Token: 0x06002E18 RID: 11800 RVA: 0x00177532 File Offset: 0x00175732
		public bool AddImpulseVelocity(Vector3 iDirection, float iElevation, float iMassPower, float iDistance)
		{
			return false;
		}

		// Token: 0x17000ADB RID: 2779
		// (get) Token: 0x06002E19 RID: 11801 RVA: 0x00177535 File Offset: 0x00175735
		public override bool Dead
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000ADC RID: 2780
		// (get) Token: 0x06002E1A RID: 11802 RVA: 0x00177538 File Offset: 0x00175738
		public float MaxHitPoints
		{
			get
			{
				return 35000f;
			}
		}

		// Token: 0x17000ADD RID: 2781
		// (get) Token: 0x06002E1B RID: 11803 RVA: 0x0017753F File Offset: 0x0017573F
		public float HitPoints
		{
			get
			{
				return this.mHitPoints;
			}
		}

		// Token: 0x06002E1C RID: 11804 RVA: 0x00177547 File Offset: 0x00175747
		public void SetSlow(int iIndex)
		{
		}

		// Token: 0x06002E1D RID: 11805 RVA: 0x00177549 File Offset: 0x00175749
		public void GetRandomPositionOnCollisionSkin(int iIndex, out Vector3 oPosition)
		{
			oPosition = default(Vector3);
		}

		// Token: 0x06002E1E RID: 11806 RVA: 0x00177552 File Offset: 0x00175752
		public bool HasStatus(int iIndex, StatusEffects iStatus)
		{
			return false;
		}

		// Token: 0x06002E1F RID: 11807 RVA: 0x00177555 File Offset: 0x00175755
		public float StatusMagnitude(int iIndex, StatusEffects iStatus)
		{
			return 0f;
		}

		// Token: 0x06002E20 RID: 11808 RVA: 0x0017755C File Offset: 0x0017575C
		public unsafe void ScriptMessage(BossMessages iMessage)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					Grimnir2.ScriptMessageMessage scriptMessageMessage;
					scriptMessageMessage.Message = iMessage;
					BossFight.Instance.SendMessage<Grimnir2.ScriptMessageMessage>(this, 1, (void*)(&scriptMessageMessage), true);
				}
				switch (iMessage)
				{
				case BossMessages.CloneDone:
					this.mLoopFight = true;
					this.mCalledAppearTrigger = true;
					return;
				case BossMessages.GrimnirHurt:
					this.mAssaturHeal = true;
					this.mLoopFight = false;
					this.mAssaturAlpha = 1f;
					this.mHitPoints = 3500f;
					return;
				case BossMessages.FutureVladDone:
				{
					if (this.mCorporeal)
					{
						return;
					}
					this.mLoopFight = true;
					this.mCalledAppearTrigger = false;
					this.mUseBothSpirits = true;
					this.mHitPoints = 35000f;
					this.mAsaController.CrossFade(this.mAsaClips[0], 0.5f, true);
					this.mAssaturHeal = false;
					Matrix matrix = this.mOrientation;
					Vector3 translation = this.mOrientation.Translation;
					translation.Y += 1.5f;
					matrix.Translation = translation;
					VisualEffectReference visualEffectReference;
					EffectManager.Instance.StartEffect(Grimnir2.ASSATUR_HEAL_EFFECT, ref matrix, out visualEffectReference);
					AudioManager.Instance.PlayCue(Grimnir2.ASSATUR_HEAL_SOUND.Value, Grimnir2.ASSATUR_HEAL_SOUND.Key, this.mGrimnirBody.AudioEmitter);
					return;
				}
				case BossMessages.KillGrimnir:
					this.mController.CrossFade(this.mClips[0], 0.25f, false);
					return;
				case BossMessages.Assatur_Cut:
					this.mAsaController.CrossFade(this.mAsaClips[2], 0.25f, false);
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x17000ADE RID: 2782
		// (get) Token: 0x06002E21 RID: 11809 RVA: 0x001776E4 File Offset: 0x001758E4
		protected override BossDamageZone Entity
		{
			get
			{
				return this.mGrimnirBody;
			}
		}

		// Token: 0x17000ADF RID: 2783
		// (get) Token: 0x06002E22 RID: 11810 RVA: 0x001776EC File Offset: 0x001758EC
		protected override float Radius
		{
			get
			{
				return this.mGrimnirBody.Radius;
			}
		}

		// Token: 0x17000AE0 RID: 2784
		// (get) Token: 0x06002E23 RID: 11811 RVA: 0x001776F9 File Offset: 0x001758F9
		protected override float Length
		{
			get
			{
				return 2f;
			}
		}

		// Token: 0x17000AE1 RID: 2785
		// (get) Token: 0x06002E24 RID: 11812 RVA: 0x00177700 File Offset: 0x00175900
		protected override int BloodEffect
		{
			get
			{
				return Grimnir2.BLOOD_EFFECT;
			}
		}

		// Token: 0x17000AE2 RID: 2786
		// (get) Token: 0x06002E25 RID: 11813 RVA: 0x00177708 File Offset: 0x00175908
		protected override Vector3 NotifierTextPostion
		{
			get
			{
				Vector3 translation = this.mOrientation.Translation;
				Vector3 vector = new Vector3(0f, 4f, 0f);
				Vector3.Add(ref translation, ref vector, out translation);
				return translation;
			}
		}

		// Token: 0x06002E26 RID: 11814 RVA: 0x00177748 File Offset: 0x00175948
		private unsafe void NetworkUpdate()
		{
			NetworkServer networkServer = NetworkManager.Instance.Interface as NetworkServer;
			if (networkServer == null)
			{
				return;
			}
			Grimnir2.UpdateMessage updateMessage = default(Grimnir2.UpdateMessage);
			updateMessage.Animation = 0;
			while ((int)updateMessage.Animation < this.mClips.Length && this.mController.AnimationClip != this.mClips[(int)updateMessage.Animation])
			{
				updateMessage.Animation += 1;
			}
			updateMessage.AnimationTime = this.mController.Time;
			updateMessage.Hitpoints = this.mHitPoints;
			updateMessage.LeftOrientation = this.mLeftSpecOrientation;
			updateMessage.RightOrientation = this.mRightSpecOrientation;
			updateMessage.LeftFloatCounter = this.mLeftFloatCounter;
			updateMessage.RightFloatCounter = this.mRightFloatCounter;
			updateMessage.SpiritTimer = this.mSpiritTimer;
			for (int i = 0; i < networkServer.Connections; i++)
			{
				float num = networkServer.GetLatency(i) * 0.5f;
				Grimnir2.UpdateMessage updateMessage2 = updateMessage;
				updateMessage2.AnimationTime += num;
				updateMessage2.SpiritTimer += num;
				BossFight.Instance.SendMessage<Grimnir2.UpdateMessage>(this, 0, (void*)(&updateMessage), false, i);
			}
		}

		// Token: 0x06002E27 RID: 11815 RVA: 0x00177868 File Offset: 0x00175A68
		public unsafe void NetworkUpdate(ref BossUpdateMessage iMsg)
		{
			if (iMsg.Type == 0)
			{
				if ((float)iMsg.TimeStamp < this.mLastNetworkUpdate)
				{
					return;
				}
				this.mLastNetworkUpdate = (float)iMsg.TimeStamp;
				Grimnir2.UpdateMessage updateMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&updateMessage));
				if (this.mController.AnimationClip != this.mClips[(int)updateMessage.Animation])
				{
					this.mController.StartClip(this.mClips[(int)updateMessage.Animation], false);
				}
				this.mLeftFloatCounter = updateMessage.LeftFloatCounter;
				this.mRightFloatCounter = updateMessage.RightFloatCounter;
				this.mLeftSpecOrientation = updateMessage.LeftOrientation;
				this.mRightSpecOrientation = updateMessage.RightOrientation;
				this.mSpiritTimer = updateMessage.SpiritTimer;
				this.mController.Time = updateMessage.AnimationTime;
				this.mHitPoints = updateMessage.Hitpoints;
				return;
			}
			else
			{
				if (iMsg.Type == 6)
				{
					Grimnir2.CastMagickMessage castMagickMessage;
					BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&castMagickMessage));
					Magick magick = default(Magick);
					magick.MagickType = castMagickMessage.Magick;
					magick.Effect.Execute(this.mGrimnirBody, this.mPlayState);
					return;
				}
				if (iMsg.Type == 5)
				{
					Grimnir2.CastSpellMessage castSpellMessage;
					BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&castSpellMessage));
					if (!castSpellMessage.SpiritCast)
					{
						Grimnir2.SpellData spellData = Grimnir2.GRIMNIRSPELLS[castSpellMessage.SpellIndex];
						this.mGrimnirCastType = spellData.CASTTYPE;
						this.mGrimnirSpellPower = spellData.SPELLPOWER;
						spellData.SPELL.Cast(true, this.mGrimnirBody, this.mGrimnirCastType);
						return;
					}
					Grimnir2.SpellData spellData2 = Grimnir2.SPIRITSPELLS[castSpellMessage.SpellIndex];
					if (castSpellMessage.SpiritIndex == 1)
					{
						this.mLeftSpiritCastType = spellData2.CASTTYPE;
						this.mLeftSpiritSpellPower = spellData2.SPELLPOWER;
						spellData2.SPELL.Cast(true, this.mLeftSpiritBody, this.mLeftSpiritCastType);
						return;
					}
					if (castSpellMessage.SpiritIndex == 2)
					{
						this.mRightSpiritCastType = spellData2.CASTTYPE;
						this.mRightSpiritSpellPower = spellData2.SPELLPOWER;
						spellData2.SPELL.Cast(true, this.mRightSpiritBody, this.mRightSpiritCastType);
						return;
					}
				}
				else if (iMsg.Type == 8)
				{
					Grimnir2.CorpMessage corpMessage;
					BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&corpMessage));
					if (this.mSpiritTimer <= 25f)
					{
						this.mSpiritTimer = 24f;
					}
					if (this.mAssaturHeal)
					{
						this.mAssaturHeal = false;
						this.mCorporeal = true;
						this.mAsaController.CrossFade(this.mAsaClips[0], 1f, true);
						Magick magick2 = default(Magick);
						magick2.MagickType = MagickType.Nullify;
						magick2.Effect.Execute(this.mGrimnirBody, this.mPlayState);
						return;
					}
				}
				else if (iMsg.Type == 1)
				{
					Grimnir2.ScriptMessageMessage scriptMessageMessage;
					BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&scriptMessageMessage));
					switch (scriptMessageMessage.Message)
					{
					case BossMessages.CloneDone:
						this.mLoopFight = true;
						this.mCalledAppearTrigger = true;
						return;
					case BossMessages.GrimnirHurt:
						this.mAssaturHeal = true;
						this.mLoopFight = false;
						this.mAssaturAlpha = 1f;
						this.mHitPoints = 3500f;
						return;
					case BossMessages.FutureVladDone:
					{
						if (this.mCorporeal)
						{
							return;
						}
						this.mLoopFight = true;
						this.mCalledAppearTrigger = false;
						this.mUseBothSpirits = true;
						this.mHitPoints = 35000f;
						this.mAsaController.CrossFade(this.mAsaClips[0], 0.5f, true);
						this.mAssaturHeal = false;
						Matrix matrix = this.mOrientation;
						Vector3 translation = this.mOrientation.Translation;
						translation.Y += 1.5f;
						matrix.Translation = translation;
						VisualEffectReference visualEffectReference;
						EffectManager.Instance.StartEffect(Grimnir2.ASSATUR_HEAL_EFFECT, ref matrix, out visualEffectReference);
						AudioManager.Instance.PlayCue(Grimnir2.ASSATUR_HEAL_SOUND.Value, Grimnir2.ASSATUR_HEAL_SOUND.Key, this.mGrimnirBody.AudioEmitter);
						return;
					}
					case BossMessages.KillGrimnir:
						this.mController.CrossFade(this.mClips[0], 0.25f, false);
						return;
					case BossMessages.Assatur_Cut:
						this.mAsaController.CrossFade(this.mAsaClips[2], 0.25f, false);
						return;
					default:
						throw new Exception(string.Concat(new object[]
						{
							"Incorrect message, ",
							scriptMessageMessage.Message,
							" passed to ",
							base.GetType().Name
						}));
					}
				}
				else if (iMsg.Type == 7)
				{
					Grimnir2.ChangeTargetMessage changeTargetMessage;
					BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&changeTargetMessage));
					if (changeTargetMessage.Target == 65535)
					{
						this.mTarget = null;
						return;
					}
					this.mTarget = (Magicka.GameLogic.Entities.Entity.GetFromHandle((int)changeTargetMessage.Target) as Character);
					return;
				}
				else
				{
					if (iMsg.Type == 2)
					{
						Grimnir2.ChangeStateMessage changeStateMessage;
						BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&changeStateMessage));
						this.mCurrentState.OnExit(this);
						this.mCurrentState = this.GetState(changeStateMessage.NewState);
						this.mCurrentState.OnEnter(this);
						return;
					}
					if (iMsg.Type == 3)
					{
						Grimnir2.ChangeSpiritStateMessage changeSpiritStateMessage;
						BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&changeSpiritStateMessage));
						switch (changeSpiritStateMessage.SpiritIndex)
						{
						default:
							this.mLeftSpiritState.OnExit(this.mLeftSpiritBody, this);
							this.mLeftSpiritState = this.GetSpiritState(changeSpiritStateMessage.NewState);
							this.mLeftSpiritState.OnEnter(this.mLeftSpiritBody, this);
							return;
						case 2:
							this.mRightSpiritState.OnExit(this.mRightSpiritBody, this);
							this.mRightSpiritState = this.GetSpiritState(changeSpiritStateMessage.NewState);
							this.mRightSpiritState.OnEnter(this.mRightSpiritBody, this);
							break;
						}
					}
				}
				return;
			}
		}

		// Token: 0x06002E28 RID: 11816 RVA: 0x00177DD4 File Offset: 0x00175FD4
		public void NetworkInitialize(ref BossInitializeMessage iMsg)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06002E29 RID: 11817 RVA: 0x00177DDB File Offset: 0x00175FDB
		public BossEnum GetBossType()
		{
			return BossEnum.Grimnir2;
		}

		// Token: 0x17000AE3 RID: 2787
		// (get) Token: 0x06002E2A RID: 11818 RVA: 0x00177DDE File Offset: 0x00175FDE
		public bool NetworkInitialized
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06002E2B RID: 11819 RVA: 0x00177DE4 File Offset: 0x00175FE4
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
					if (base.HasStatus(StatusEffects.Frozen) && (iElement & Elements.Earth) != Elements.None)
					{
						num4 -= 350f;
					}
					if (base.HasStatus(StatusEffects.Greased) && (iElement & Elements.Fire) != Elements.None)
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

		// Token: 0x040031C0 RID: 12736
		private const float NETWORK_UPDATE_PERIOD = 0.033333335f;

		// Token: 0x040031C1 RID: 12737
		private const float MAXHITPOINTS = 35000f;

		// Token: 0x040031C2 RID: 12738
		private const float CAPSULE_RADIUS = 0.75f;

		// Token: 0x040031C3 RID: 12739
		private const float CAPSULE_LENGTH = 1.5f;

		// Token: 0x040031C4 RID: 12740
		private const int GRIMNIRID = 0;

		// Token: 0x040031C5 RID: 12741
		private const int LEFTSPIRITID = 1;

		// Token: 0x040031C6 RID: 12742
		private const int RIGHTSPIRITID = 2;

		// Token: 0x040031C7 RID: 12743
		private const float SPIRIT_TIME = 25f;

		// Token: 0x040031C8 RID: 12744
		private float mLastNetworkUpdate;

		// Token: 0x040031C9 RID: 12745
		protected float mNetworkUpdateTimer;

		// Token: 0x040031CA RID: 12746
		private static readonly int ASSATUR_HEAL_EFFECT = "assatur_life".GetHashCodeCustom();

		// Token: 0x040031CB RID: 12747
		private static readonly KeyValuePair<int, Banks> ASSATUR_HEAL_SOUND = new KeyValuePair<int, Banks>("spell_life_self".GetHashCodeCustom(), Banks.Spells);

		// Token: 0x040031CC RID: 12748
		private static readonly Random RANDOM = new Random();

		// Token: 0x040031CD RID: 12749
		private static readonly int ASSATUR_TALKS_TRIGGER = "assatur_talks".GetHashCodeCustom();

		// Token: 0x040031CE RID: 12750
		private static readonly int ASSATUR_APPEARS_TRIGGER = "assatur_appears".GetHashCodeCustom();

		// Token: 0x040031CF RID: 12751
		private static readonly int CAST_TORNADO_EFFECT = "grimnir_tornado".GetHashCodeCustom();

		// Token: 0x040031D0 RID: 12752
		private static readonly int CAST_THUNDER_EFFECT = "grimnir_thunder".GetHashCodeCustom();

		// Token: 0x040031D1 RID: 12753
		private static readonly int CAST_CONFLAG_EFFECT = "grimnir_conflagration".GetHashCodeCustom();

		// Token: 0x040031D2 RID: 12754
		private static readonly int CAST_RAIN_EFFECT = "grimnir_rain".GetHashCodeCustom();

		// Token: 0x040031D3 RID: 12755
		private static readonly int CAST_SPELL_EFFECT = "grimnir_spell".GetHashCodeCustom();

		// Token: 0x040031D4 RID: 12756
		private static readonly int BLOOD_EFFECT = "gore_splash_regular".GetHashCodeCustom();

		// Token: 0x040031D5 RID: 12757
		private static readonly int GENERIC_MAGICK = "magick_generic".GetHashCodeCustom();

		// Token: 0x040031D6 RID: 12758
		private VisualEffectReference mCastEffect;

		// Token: 0x040031D7 RID: 12759
		private Grimnir2.Animations mLeftCastAnimation;

		// Token: 0x040031D8 RID: 12760
		private Grimnir2.Animations mRightCastAnimation;

		// Token: 0x040031D9 RID: 12761
		private static readonly Grimnir2.SpellData[] SPIRITSPELLS = new Grimnir2.SpellData[2];

		// Token: 0x040031DA RID: 12762
		private static readonly Grimnir2.SpellData[] GRIMNIRSPELLS = new Grimnir2.SpellData[3];

		// Token: 0x040031DB RID: 12763
		private AnimationClip[] mAsaClips;

		// Token: 0x040031DC RID: 12764
		private AnimationController mAsaController;

		// Token: 0x040031DD RID: 12765
		private Grimnir2.RenderData[] mAsaRenderData;

		// Token: 0x040031DE RID: 12766
		private Grimnir2.AdditiveRenderData[] mAdditiveAsaRenderData;

		// Token: 0x040031DF RID: 12767
		private AnimationClip[] mClips;

		// Token: 0x040031E0 RID: 12768
		private AnimationController mController;

		// Token: 0x040031E1 RID: 12769
		private AnimationController mLeftSpiritController;

		// Token: 0x040031E2 RID: 12770
		private AnimationController mRightSpiritController;

		// Token: 0x040031E3 RID: 12771
		private Grimnir2.RenderData[] mRenderData;

		// Token: 0x040031E4 RID: 12772
		private Grimnir2.AdditiveRenderData[] mAdditiveRenderData;

		// Token: 0x040031E5 RID: 12773
		private Matrix mOrientation;

		// Token: 0x040031E6 RID: 12774
		private IBossState<Grimnir2> mCurrentState;

		// Token: 0x040031E7 RID: 12775
		private float mSpectralAlpha;

		// Token: 0x040031E8 RID: 12776
		private float mSpectralTargetAlpha;

		// Token: 0x040031E9 RID: 12777
		private Matrix mLeftSpecOrientation;

		// Token: 0x040031EA RID: 12778
		private Matrix mRightSpecOrientation;

		// Token: 0x040031EB RID: 12779
		private Grimnir2.ISpiritState mLeftSpiritState;

		// Token: 0x040031EC RID: 12780
		private Grimnir2.ISpiritState mRightSpiritState;

		// Token: 0x040031ED RID: 12781
		private int mCastAttachIndex;

		// Token: 0x040031EE RID: 12782
		private int mWeaponAttachIndex;

		// Token: 0x040031EF RID: 12783
		private bool mAssaturHeal;

		// Token: 0x040031F0 RID: 12784
		private MagickType mLastMagick = MagickType.MeteorS;

		// Token: 0x040031F1 RID: 12785
		private float mGrimnirIdleTimer;

		// Token: 0x040031F2 RID: 12786
		private float mGrimnirSpellPower;

		// Token: 0x040031F3 RID: 12787
		private CastType mGrimnirCastType;

		// Token: 0x040031F4 RID: 12788
		private SpellEffect mGrimnirSpellEffect;

		// Token: 0x040031F5 RID: 12789
		private int mGrimnirLastSpell;

		// Token: 0x040031F6 RID: 12790
		private float mLeftSpiritIdleTimer;

		// Token: 0x040031F7 RID: 12791
		private bool mLeftSpiritHasCastSpell;

		// Token: 0x040031F8 RID: 12792
		private float mLeftSpiritSpellPower;

		// Token: 0x040031F9 RID: 12793
		private CastType mLeftSpiritCastType;

		// Token: 0x040031FA RID: 12794
		private SpellEffect mLeftSpiritSpellEffect;

		// Token: 0x040031FB RID: 12795
		private int mLeftSpiritLastSpell;

		// Token: 0x040031FC RID: 12796
		private float mRightSpiritIdleTimer;

		// Token: 0x040031FD RID: 12797
		private bool mRightSpiritHasCastSpell;

		// Token: 0x040031FE RID: 12798
		private float mRightSpiritSpellPower;

		// Token: 0x040031FF RID: 12799
		private CastType mRightSpiritCastType;

		// Token: 0x04003200 RID: 12800
		private SpellEffect mRightSpiritSpellEffect;

		// Token: 0x04003201 RID: 12801
		private int mRightSpiritLastSpell;

		// Token: 0x04003202 RID: 12802
		private Character mTarget;

		// Token: 0x04003203 RID: 12803
		private Vector3 mTargetPosition;

		// Token: 0x04003204 RID: 12804
		private float mLeftFloatCounter;

		// Token: 0x04003205 RID: 12805
		private float mRightFloatCounter;

		// Token: 0x04003206 RID: 12806
		private float mSpiritTimer;

		// Token: 0x04003207 RID: 12807
		private float mTargetSwapTimer;

		// Token: 0x04003208 RID: 12808
		private PlayState mPlayState;

		// Token: 0x04003209 RID: 12809
		private BossSpellCasterZone mGrimnirBody;

		// Token: 0x0400320A RID: 12810
		private BossSpellCasterZone mLeftSpiritBody;

		// Token: 0x0400320B RID: 12811
		private BossSpellCasterZone mRightSpiritBody;

		// Token: 0x0400320C RID: 12812
		private TextureCube mIceCubeMap;

		// Token: 0x0400320D RID: 12813
		private TextureCube mIceCubeNormalMap;

		// Token: 0x0400320E RID: 12814
		private Matrix YROT = Matrix.CreateRotationY(3.1415927f);

		// Token: 0x0400320F RID: 12815
		private float mAssaturAlpha;

		// Token: 0x04003210 RID: 12816
		private Matrix mAssaturOrientation;

		// Token: 0x04003211 RID: 12817
		private bool mUseBothSpirits;

		// Token: 0x04003212 RID: 12818
		private float mDamageFlashTimer;

		// Token: 0x04003213 RID: 12819
		private BoundingSphere mGrimnirBoundingSphere;

		// Token: 0x04003214 RID: 12820
		private BoundingSphere mSpiritBoundingSphere;

		// Token: 0x04003215 RID: 12821
		private BoundingSphere mAsaBoundingSphere;

		// Token: 0x04003216 RID: 12822
		private SkinnedModelDeferredAdvancedMaterial mGrimnirMaterial;

		// Token: 0x04003217 RID: 12823
		private bool mCalledAppearTrigger;

		// Token: 0x04003218 RID: 12824
		private bool mLoopFight;

		// Token: 0x04003219 RID: 12825
		private bool mCorporeal;

		// Token: 0x0400321A RID: 12826
		private GibReference[] mGrimnirGibs;

		// Token: 0x0400321B RID: 12827
		private Matrix mCliffOrientation;

		// Token: 0x020005F0 RID: 1520
		protected class RenderData : RenderableObject<SkinnedModelDeferredEffect, SkinnedModelDeferredAdvancedMaterial>
		{
			// Token: 0x06002E2C RID: 11820 RVA: 0x00177E9F File Offset: 0x0017609F
			public RenderData()
			{
				this.mBones = new Matrix[80];
			}

			// Token: 0x06002E2D RID: 11821 RVA: 0x00177EB4 File Offset: 0x001760B4
			public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				skinnedModelDeferredEffect.OverrideColor = new Vector4(1f, 1f, 1f, this.Flash);
				skinnedModelDeferredEffect.Bones = this.mBones;
				base.Draw(iEffect, iViewFrustum);
				skinnedModelDeferredEffect.OverrideColor = Vector4.Zero;
			}

			// Token: 0x06002E2E RID: 11822 RVA: 0x00177F08 File Offset: 0x00176108
			public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				skinnedModelDeferredEffect.Bones = this.mBones;
				base.DrawShadow(iEffect, iViewFrustum);
			}

			// Token: 0x0400321C RID: 12828
			public float Flash;

			// Token: 0x0400321D RID: 12829
			public Matrix[] mBones;
		}

		// Token: 0x020005F1 RID: 1521
		protected class AdditiveRenderData : RenderableAdditiveObject<SkinnedModelDeferredEffect, SkinnedModelDeferredAdvancedMaterial>
		{
			// Token: 0x06002E2F RID: 11823 RVA: 0x00177F30 File Offset: 0x00176130
			public AdditiveRenderData()
			{
				this.mRightSpecBones = new Matrix[80];
				this.mLeftSpecBones = new Matrix[80];
			}

			// Token: 0x06002E30 RID: 11824 RVA: 0x00177F54 File Offset: 0x00176154
			public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				if (this.RenderLeftSpectral)
				{
					SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
					skinnedModelDeferredEffect.Bones = this.mLeftSpecBones;
					skinnedModelDeferredEffect.OverrideColor = new Vector4(1f, 1f, 1f, 0f);
					base.Draw(iEffect, iViewFrustum);
					skinnedModelDeferredEffect.Colorize = Vector4.Zero;
				}
				if (this.RenderRightSpectral)
				{
					SkinnedModelDeferredEffect skinnedModelDeferredEffect2 = iEffect as SkinnedModelDeferredEffect;
					skinnedModelDeferredEffect2.Bones = this.mRightSpecBones;
					skinnedModelDeferredEffect2.OverrideColor = new Vector4(1f, 1f, 1f, 0f);
					base.Draw(iEffect, iViewFrustum);
					skinnedModelDeferredEffect2.Colorize = Vector4.Zero;
				}
			}

			// Token: 0x0400321E RID: 12830
			public static readonly Vector4 ColdColor = new Vector4(1f, 1.6f, 2f, 1f);

			// Token: 0x0400321F RID: 12831
			public Matrix[] mLeftSpecBones;

			// Token: 0x04003220 RID: 12832
			public Matrix[] mRightSpecBones;

			// Token: 0x04003221 RID: 12833
			public bool RenderLeftSpectral;

			// Token: 0x04003222 RID: 12834
			public bool RenderRightSpectral;
		}

		// Token: 0x020005F2 RID: 1522
		protected class IntroState : IBossState<Grimnir2>
		{
			// Token: 0x17000AE4 RID: 2788
			// (get) Token: 0x06002E32 RID: 11826 RVA: 0x0017801C File Offset: 0x0017621C
			public static Grimnir2.IntroState Instance
			{
				get
				{
					if (Grimnir2.IntroState.sSingelton == null)
					{
						lock (Grimnir2.IntroState.sSingeltonLock)
						{
							if (Grimnir2.IntroState.sSingelton == null)
							{
								Grimnir2.IntroState.sSingelton = new Grimnir2.IntroState();
							}
						}
					}
					return Grimnir2.IntroState.sSingelton;
				}
			}

			// Token: 0x06002E33 RID: 11827 RVA: 0x00178070 File Offset: 0x00176270
			public void OnEnter(Grimnir2 iOwner)
			{
				iOwner.mController.StartClip(iOwner.mClips[6], false);
			}

			// Token: 0x06002E34 RID: 11828 RVA: 0x00178086 File Offset: 0x00176286
			public void OnUpdate(float iDeltaTime, Grimnir2 iOwner)
			{
			}

			// Token: 0x06002E35 RID: 11829 RVA: 0x00178088 File Offset: 0x00176288
			public void OnExit(Grimnir2 iOwner)
			{
			}

			// Token: 0x04003223 RID: 12835
			private static Grimnir2.IntroState sSingelton;

			// Token: 0x04003224 RID: 12836
			private static volatile object sSingeltonLock = new object();
		}

		// Token: 0x020005F3 RID: 1523
		protected class IdleState : IBossState<Grimnir2>
		{
			// Token: 0x17000AE5 RID: 2789
			// (get) Token: 0x06002E38 RID: 11832 RVA: 0x001780A0 File Offset: 0x001762A0
			public static Grimnir2.IdleState Instance
			{
				get
				{
					if (Grimnir2.IdleState.sSingelton == null)
					{
						lock (Grimnir2.IdleState.sSingeltonLock)
						{
							if (Grimnir2.IdleState.sSingelton == null)
							{
								Grimnir2.IdleState.sSingelton = new Grimnir2.IdleState();
							}
						}
					}
					return Grimnir2.IdleState.sSingelton;
				}
			}

			// Token: 0x06002E39 RID: 11833 RVA: 0x001780F4 File Offset: 0x001762F4
			private IdleState()
			{
			}

			// Token: 0x06002E3A RID: 11834 RVA: 0x001780FC File Offset: 0x001762FC
			public void OnEnter(Grimnir2 iOwner)
			{
				iOwner.mController.CrossFade(iOwner.mClips[6], 0.5f, true);
				iOwner.mGrimnirIdleTimer = 2f + (float)Grimnir2.RANDOM.NextDouble() * 2f;
			}

			// Token: 0x06002E3B RID: 11835 RVA: 0x00178134 File Offset: 0x00176334
			public void OnUpdate(float iDeltaTime, Grimnir2 iOwner)
			{
				if (iOwner.mTimeSinceLastDamage < 1f)
				{
					iOwner.mGrimnirIdleTimer = 0f;
				}
				if (iOwner.mGrimnirIdleTimer <= 0f)
				{
					float num = (iOwner.mSpiritTimer - 25f) / 25f;
					num = Math.Max(num, 0f);
					if (Grimnir2.RANDOM.NextDouble() <= (double)num)
					{
						iOwner.ChangeState(Grimnir2.States.Spirit);
					}
					else if (Grimnir2.RANDOM.NextDouble() < (double)Math.Min(iOwner.mTimeSinceLastDamage, 0.75f))
					{
						iOwner.ChangeState(Grimnir2.States.Spell);
					}
					else
					{
						iOwner.ChangeState(Grimnir2.States.Magick);
					}
				}
				iOwner.mGrimnirIdleTimer -= iDeltaTime;
			}

			// Token: 0x06002E3C RID: 11836 RVA: 0x001781D7 File Offset: 0x001763D7
			public void OnExit(Grimnir2 iOwner)
			{
			}

			// Token: 0x04003225 RID: 12837
			private static Grimnir2.IdleState sSingelton;

			// Token: 0x04003226 RID: 12838
			private static volatile object sSingeltonLock = new object();
		}

		// Token: 0x020005F4 RID: 1524
		protected class SpiritState : IBossState<Grimnir2>
		{
			// Token: 0x17000AE6 RID: 2790
			// (get) Token: 0x06002E3E RID: 11838 RVA: 0x001781E8 File Offset: 0x001763E8
			public static Grimnir2.SpiritState Instance
			{
				get
				{
					if (Grimnir2.SpiritState.sSingelton == null)
					{
						lock (Grimnir2.SpiritState.sSingeltonLock)
						{
							if (Grimnir2.SpiritState.sSingelton == null)
							{
								Grimnir2.SpiritState.sSingelton = new Grimnir2.SpiritState();
							}
						}
					}
					return Grimnir2.SpiritState.sSingelton;
				}
			}

			// Token: 0x06002E3F RID: 11839 RVA: 0x0017823C File Offset: 0x0017643C
			public void OnEnter(Grimnir2 iOwner)
			{
				iOwner.mController.CrossFade(iOwner.mClips[13], 0.25f, false);
			}

			// Token: 0x06002E40 RID: 11840 RVA: 0x00178258 File Offset: 0x00176458
			public void OnUpdate(float iDeltaTime, Grimnir2 iOwner)
			{
				if (iOwner.mController.HasFinished && !iOwner.mController.CrossFadeEnabled)
				{
					iOwner.mSpiritTimer = 0f;
					iOwner.mRightFloatCounter = 0f;
					iOwner.mLeftFloatCounter = 0f;
					iOwner.ChangeState(Grimnir2.States.Idle);
				}
			}

			// Token: 0x06002E41 RID: 11841 RVA: 0x001782A7 File Offset: 0x001764A7
			public void OnExit(Grimnir2 iOwner)
			{
			}

			// Token: 0x04003227 RID: 12839
			private static Grimnir2.SpiritState sSingelton;

			// Token: 0x04003228 RID: 12840
			private static volatile object sSingeltonLock = new object();
		}

		// Token: 0x020005F5 RID: 1525
		protected class CastMagickState : IBossState<Grimnir2>
		{
			// Token: 0x17000AE7 RID: 2791
			// (get) Token: 0x06002E44 RID: 11844 RVA: 0x001782C0 File Offset: 0x001764C0
			public static Grimnir2.CastMagickState Instance
			{
				get
				{
					if (Grimnir2.CastMagickState.sSingelton == null)
					{
						lock (Grimnir2.CastMagickState.sSingeltonLock)
						{
							if (Grimnir2.CastMagickState.sSingelton == null)
							{
								Grimnir2.CastMagickState.sSingelton = new Grimnir2.CastMagickState();
							}
						}
					}
					return Grimnir2.CastMagickState.sSingelton;
				}
			}

			// Token: 0x06002E45 RID: 11845 RVA: 0x00178314 File Offset: 0x00176514
			private CastMagickState()
			{
			}

			// Token: 0x06002E46 RID: 11846 RVA: 0x0017831C File Offset: 0x0017651C
			public void OnEnter(Grimnir2 iOwner)
			{
				iOwner.mGrimnirIdleTimer = 2f;
				if (NetworkManager.Instance.State != NetworkState.Client)
				{
					float num = this.GetTornadoWeight(iOwner.mLastMagick, iOwner.mPlayState);
					float num2 = this.GetRainWeight(iOwner.mLastMagick);
					float num3 = this.GetConflagrationWeight(iOwner.mLastMagick);
					float num4 = this.GetThunderBWeight(iOwner.mLastMagick);
					float num5 = num + num2 + num3 + num4;
					num /= num5;
					num2 /= num5;
					num3 /= num5;
					num4 /= num5;
					Magick magick = default(Magick);
					Matrix mOrientation = iOwner.mOrientation;
					Vector3 translation = mOrientation.Translation;
					translation.Y += iOwner.Radius * 2f;
					translation.Z += iOwner.Radius * 0.5f;
					mOrientation.Translation = translation;
					if (num > num2 && num > num3 && num > num4)
					{
						iOwner.mController.CrossFade(iOwner.mClips[11], 0.2f, true);
						magick.MagickType = MagickType.Tornado;
						EffectManager.Instance.StartEffect(Grimnir2.GENERIC_MAGICK, ref mOrientation, out iOwner.mCastEffect);
					}
					else if (num2 > num3 && num2 > num4)
					{
						iOwner.mController.CrossFade(iOwner.mClips[11], 0.2f, true);
						magick.MagickType = MagickType.Rain;
						EffectManager.Instance.StartEffect(Grimnir2.GENERIC_MAGICK, ref mOrientation, out iOwner.mCastEffect);
					}
					else if (num3 > num4)
					{
						iOwner.mController.CrossFade(iOwner.mClips[12], 0.25f, true);
						magick.MagickType = MagickType.Conflagration;
						EffectManager.Instance.StartEffect(Grimnir2.GENERIC_MAGICK, ref mOrientation, out iOwner.mCastEffect);
					}
					else
					{
						iOwner.mController.CrossFade(iOwner.mClips[12], 0.2f, true);
						magick.MagickType = MagickType.ThunderB;
						EffectManager.Instance.StartEffect(Grimnir2.GENERIC_MAGICK, ref mOrientation, out iOwner.mCastEffect);
					}
					iOwner.mLastMagick = magick.MagickType;
				}
			}

			// Token: 0x06002E47 RID: 11847 RVA: 0x0017850A File Offset: 0x0017670A
			public void OnUpdate(float iDeltaTime, Grimnir2 iOwner)
			{
				if (NetworkManager.Instance.State != NetworkState.Client)
				{
					if (iOwner.mGrimnirIdleTimer <= 0f)
					{
						iOwner.ChangeState(Grimnir2.States.Idle);
					}
					iOwner.mGrimnirIdleTimer -= iDeltaTime;
				}
			}

			// Token: 0x06002E48 RID: 11848 RVA: 0x0017853C File Offset: 0x0017673C
			public unsafe void OnExit(Grimnir2 iOwner)
			{
				EffectManager.Instance.Stop(ref iOwner.mCastEffect);
				if (NetworkManager.Instance.State != NetworkState.Client)
				{
					Magick magick = default(Magick);
					magick.MagickType = iOwner.mLastMagick;
					if (NetworkManager.Instance.State == NetworkState.Server)
					{
						Grimnir2.CastMagickMessage castMagickMessage;
						castMagickMessage.Magick = magick.MagickType;
						BossFight.Instance.SendMessage<Grimnir2.CastMagickMessage>(iOwner, 6, (void*)(&castMagickMessage), true);
					}
					magick.Effect.Execute(iOwner.mGrimnirBody, iOwner.mPlayState);
				}
			}

			// Token: 0x06002E49 RID: 11849 RVA: 0x001785C0 File Offset: 0x001767C0
			private float GetTornadoWeight(MagickType iLastMagick, PlayState iPlayState)
			{
				if (iLastMagick == MagickType.Blizzard)
				{
					return 0f;
				}
				float result = (float)Grimnir2.RANDOM.NextDouble();
				StaticList<Entity> entities = iPlayState.EntityManager.Entities;
				for (int i = 0; i < entities.Count; i++)
				{
					if (entities[i] is TornadoEntity)
					{
						return 0f;
					}
				}
				return result;
			}

			// Token: 0x06002E4A RID: 11850 RVA: 0x00178618 File Offset: 0x00176818
			private float GetRainWeight(MagickType iLastMagick)
			{
				if (iLastMagick == MagickType.Blizzard)
				{
					return 0f;
				}
				float result = Rain.Instance.IsDead ? ((float)Grimnir2.RANDOM.NextDouble()) : 0f;
				Player[] players = Game.Instance.Players;
				for (int i = 0; i < players.Length; i++)
				{
					if (players[i].Playing && players[i].Avatar != null && players[i].Avatar.HasStatus(StatusEffects.Wet))
					{
						return 0f;
					}
				}
				return result;
			}

			// Token: 0x06002E4B RID: 11851 RVA: 0x00178694 File Offset: 0x00176894
			private float GetConflagrationWeight(MagickType iLastMagick)
			{
				float result = 0f;
				if (iLastMagick == MagickType.Blizzard)
				{
					return 0f;
				}
				if (SpellManager.Instance.IsEffectActive(typeof(Conflagration)))
				{
					return 0f;
				}
				if (Blizzard.Instance.IsDead)
				{
					result = (float)Grimnir2.RANDOM.NextDouble();
				}
				return result;
			}

			// Token: 0x06002E4C RID: 11852 RVA: 0x001786E8 File Offset: 0x001768E8
			private float GetThunderBWeight(MagickType iLastMagick)
			{
				if (iLastMagick == MagickType.Blizzard)
				{
					return 0f;
				}
				float num = 0f;
				Player[] players = Game.Instance.Players;
				for (int i = 0; i < players.Length; i++)
				{
					if (players[i].Playing && players[i].Avatar != null && players[i].Avatar.HasStatus(StatusEffects.Wet))
					{
						num += 1f / (float)Game.Instance.PlayerCount;
					}
				}
				return num;
			}

			// Token: 0x04003229 RID: 12841
			private static Grimnir2.CastMagickState sSingelton;

			// Token: 0x0400322A RID: 12842
			private static volatile object sSingeltonLock = new object();
		}

		// Token: 0x020005F6 RID: 1526
		protected class CastSpellState : IBossState<Grimnir2>
		{
			// Token: 0x17000AE8 RID: 2792
			// (get) Token: 0x06002E4E RID: 11854 RVA: 0x00178768 File Offset: 0x00176968
			public static Grimnir2.CastSpellState Instance
			{
				get
				{
					if (Grimnir2.CastSpellState.sSingelton == null)
					{
						lock (Grimnir2.CastSpellState.sSingeltonLock)
						{
							if (Grimnir2.CastSpellState.sSingelton == null)
							{
								Grimnir2.CastSpellState.sSingelton = new Grimnir2.CastSpellState();
							}
						}
					}
					return Grimnir2.CastSpellState.sSingelton;
				}
			}

			// Token: 0x06002E4F RID: 11855 RVA: 0x001787BC File Offset: 0x001769BC
			private CastSpellState()
			{
			}

			// Token: 0x06002E50 RID: 11856 RVA: 0x001787C4 File Offset: 0x001769C4
			public void OnEnter(Grimnir2 iOwner)
			{
				iOwner.mController.CrossFade(iOwner.mClips[14], 0.25f, false);
				if (NetworkManager.Instance.State == NetworkState.Client)
				{
					return;
				}
				float num = float.MaxValue;
				Player[] players = Game.Instance.Players;
				for (int i = 0; i < players.Length; i++)
				{
					if (players[i].Playing && players[i].Avatar != null)
					{
						Vector3 translation = iOwner.mOrientation.Translation;
						Vector3 position = players[i].Avatar.Position;
						float num2;
						Vector3.DistanceSquared(ref translation, ref position, out num2);
						if (num2 < num)
						{
							num = num2;
						}
					}
				}
				if (iOwner.mTimeSinceLastDamage < 1f)
				{
					iOwner.mGrimnirLastSpell = ((Grimnir2.RANDOM.NextDouble() > 0.5) ? 1 : 2);
					return;
				}
				if (num < 25f)
				{
					iOwner.mGrimnirLastSpell = 0;
					return;
				}
				if (Grimnir2.RANDOM.NextDouble() > 0.5)
				{
					iOwner.mGrimnirLastSpell = 2;
					return;
				}
				iOwner.mGrimnirLastSpell = 1;
			}

			// Token: 0x06002E51 RID: 11857 RVA: 0x001788BC File Offset: 0x00176ABC
			public unsafe void OnUpdate(float iDeltaTime, Grimnir2 iOwner)
			{
				if (NetworkManager.Instance.State != NetworkState.Client && iOwner.mController.HasFinished && !iOwner.mController.CrossFadeEnabled)
				{
					Grimnir2.SpellData spellData = Grimnir2.GRIMNIRSPELLS[iOwner.mGrimnirLastSpell];
					if (NetworkManager.Instance.State == NetworkState.Server)
					{
						Grimnir2.CastSpellMessage castSpellMessage;
						castSpellMessage.SpiritCast = false;
						castSpellMessage.SpiritIndex = 0;
						castSpellMessage.SpellIndex = iOwner.mGrimnirLastSpell;
						BossFight.Instance.SendMessage<Grimnir2.CastSpellMessage>(iOwner, 5, (void*)(&castSpellMessage), true);
					}
					iOwner.mGrimnirCastType = spellData.CASTTYPE;
					iOwner.mGrimnirSpellPower = spellData.SPELLPOWER;
					spellData.SPELL.Cast(true, iOwner.mGrimnirBody, iOwner.mGrimnirCastType);
					iOwner.ChangeState(Grimnir2.States.Idle);
				}
			}

			// Token: 0x06002E52 RID: 11858 RVA: 0x00178983 File Offset: 0x00176B83
			public void OnExit(Grimnir2 iOwner)
			{
				if (iOwner.mGrimnirSpellEffect != null)
				{
					iOwner.mGrimnirSpellEffect.DeInitialize(iOwner.mGrimnirBody);
				}
			}

			// Token: 0x0400322B RID: 12843
			private static Grimnir2.CastSpellState sSingelton;

			// Token: 0x0400322C RID: 12844
			private static volatile object sSingeltonLock = new object();
		}

		// Token: 0x020005F7 RID: 1527
		protected class HealState : IBossState<Grimnir2>
		{
			// Token: 0x17000AE9 RID: 2793
			// (get) Token: 0x06002E54 RID: 11860 RVA: 0x001789AC File Offset: 0x00176BAC
			public static Grimnir2.HealState Instance
			{
				get
				{
					if (Grimnir2.HealState.sSingelton == null)
					{
						lock (Grimnir2.HealState.sSingeltonLock)
						{
							if (Grimnir2.HealState.sSingelton == null)
							{
								Grimnir2.HealState.sSingelton = new Grimnir2.HealState();
							}
						}
					}
					return Grimnir2.HealState.sSingelton;
				}
			}

			// Token: 0x06002E55 RID: 11861 RVA: 0x00178A00 File Offset: 0x00176C00
			private HealState()
			{
			}

			// Token: 0x06002E56 RID: 11862 RVA: 0x00178A08 File Offset: 0x00176C08
			public void OnEnter(Grimnir2 iOwner)
			{
				iOwner.mController.CrossFade(iOwner.mClips[6], 0.25f, true);
				if (NetworkManager.Instance.State == NetworkState.Client)
				{
					return;
				}
				Vector3 vector = new Vector3(0f, 4f, -7f);
				iOwner.mPlayState.Camera.SetBias(ref vector, 3f);
			}

			// Token: 0x06002E57 RID: 11863 RVA: 0x00178A6E File Offset: 0x00176C6E
			public void OnUpdate(float iDeltaTime, Grimnir2 iOwner)
			{
				if (!iOwner.mAssaturHeal)
				{
					iOwner.ChangeState(Grimnir2.States.Idle);
				}
			}

			// Token: 0x06002E58 RID: 11864 RVA: 0x00178A7F File Offset: 0x00176C7F
			public void OnExit(Grimnir2 iOwner)
			{
			}

			// Token: 0x0400322D RID: 12845
			private static Grimnir2.HealState sSingelton;

			// Token: 0x0400322E RID: 12846
			private static volatile object sSingeltonLock = new object();
		}

		// Token: 0x020005F8 RID: 1528
		protected class DieState : IBossState<Grimnir2>
		{
			// Token: 0x17000AEA RID: 2794
			// (get) Token: 0x06002E5A RID: 11866 RVA: 0x00178A90 File Offset: 0x00176C90
			public static Grimnir2.DieState Instance
			{
				get
				{
					if (Grimnir2.DieState.sSingelton == null)
					{
						lock (Grimnir2.DieState.sSingeltonLock)
						{
							if (Grimnir2.DieState.sSingelton == null)
							{
								Grimnir2.DieState.sSingelton = new Grimnir2.DieState();
							}
						}
					}
					return Grimnir2.DieState.sSingelton;
				}
			}

			// Token: 0x06002E5B RID: 11867 RVA: 0x00178AE4 File Offset: 0x00176CE4
			private DieState()
			{
			}

			// Token: 0x06002E5C RID: 11868 RVA: 0x00178AEC File Offset: 0x00176CEC
			public void OnEnter(Grimnir2 iOwner)
			{
				iOwner.mGrimnirBody.IsEthereal = true;
				iOwner.mHitPoints = 0f;
				iOwner.mController.CrossFade(iOwner.mClips[6], 0.25f, false);
			}

			// Token: 0x06002E5D RID: 11869 RVA: 0x00178B1E File Offset: 0x00176D1E
			public void OnUpdate(float iDeltaTime, Grimnir2 iOwner)
			{
			}

			// Token: 0x06002E5E RID: 11870 RVA: 0x00178B20 File Offset: 0x00176D20
			public void OnExit(Grimnir2 iOwner)
			{
			}

			// Token: 0x0400322F RID: 12847
			private static Grimnir2.DieState sSingelton;

			// Token: 0x04003230 RID: 12848
			private static volatile object sSingeltonLock = new object();
		}

		// Token: 0x020005F9 RID: 1529
		public interface ISpiritState
		{
			// Token: 0x06002E60 RID: 11872
			void OnEnter(BossSpellCasterZone iZone, Grimnir2 iOwner);

			// Token: 0x06002E61 RID: 11873
			void OnUpdate(float iDeltaTime, BossSpellCasterZone iZone, Grimnir2 iOwner);

			// Token: 0x06002E62 RID: 11874
			void OnExit(BossSpellCasterZone iZone, Grimnir2 iOwner);
		}

		// Token: 0x020005FA RID: 1530
		protected class SpecIntroState : Grimnir2.ISpiritState
		{
			// Token: 0x17000AEB RID: 2795
			// (get) Token: 0x06002E63 RID: 11875 RVA: 0x00178B30 File Offset: 0x00176D30
			public static Grimnir2.SpecIntroState Instance
			{
				get
				{
					if (Grimnir2.SpecIntroState.sSingelton == null)
					{
						lock (Grimnir2.SpecIntroState.sSingeltonLock)
						{
							if (Grimnir2.SpecIntroState.sSingelton == null)
							{
								Grimnir2.SpecIntroState.sSingelton = new Grimnir2.SpecIntroState();
							}
						}
					}
					return Grimnir2.SpecIntroState.sSingelton;
				}
			}

			// Token: 0x06002E64 RID: 11876 RVA: 0x00178B84 File Offset: 0x00176D84
			private SpecIntroState()
			{
			}

			// Token: 0x06002E65 RID: 11877 RVA: 0x00178B8C File Offset: 0x00176D8C
			public void OnEnter(BossSpellCasterZone iZone, Grimnir2 iOwner)
			{
				iOwner.mSpectralTargetAlpha = 0f;
				iOwner.mLeftSpiritController.StartClip(iOwner.mClips[7], true);
				iOwner.mRightSpiritController.StartClip(iOwner.mClips[7], true);
			}

			// Token: 0x06002E66 RID: 11878 RVA: 0x00178BC1 File Offset: 0x00176DC1
			public void OnUpdate(float iDeltaTime, BossSpellCasterZone iZone, Grimnir2 iOwner)
			{
			}

			// Token: 0x06002E67 RID: 11879 RVA: 0x00178BC3 File Offset: 0x00176DC3
			public void OnExit(BossSpellCasterZone iZone, Grimnir2 iOwner)
			{
			}

			// Token: 0x04003231 RID: 12849
			private static Grimnir2.SpecIntroState sSingelton;

			// Token: 0x04003232 RID: 12850
			private static volatile object sSingeltonLock = new object();
		}

		// Token: 0x020005FB RID: 1531
		protected class SpecIdleState : Grimnir2.ISpiritState
		{
			// Token: 0x17000AEC RID: 2796
			// (get) Token: 0x06002E69 RID: 11881 RVA: 0x00178BD4 File Offset: 0x00176DD4
			public static Grimnir2.SpecIdleState Instance
			{
				get
				{
					if (Grimnir2.SpecIdleState.sSingelton == null)
					{
						lock (Grimnir2.SpecIdleState.sSingeltonLock)
						{
							if (Grimnir2.SpecIdleState.sSingelton == null)
							{
								Grimnir2.SpecIdleState.sSingelton = new Grimnir2.SpecIdleState();
							}
						}
					}
					return Grimnir2.SpecIdleState.sSingelton;
				}
			}

			// Token: 0x06002E6A RID: 11882 RVA: 0x00178C28 File Offset: 0x00176E28
			private SpecIdleState()
			{
			}

			// Token: 0x06002E6B RID: 11883 RVA: 0x00178C30 File Offset: 0x00176E30
			public void OnEnter(BossSpellCasterZone iZone, Grimnir2 iOwner)
			{
				if (iZone.Index == 1)
				{
					iOwner.mLeftSpiritIdleTimer = (float)Grimnir2.RANDOM.NextDouble() * 2f + 2f;
				}
				else
				{
					iOwner.mRightSpiritIdleTimer = (float)Grimnir2.RANDOM.NextDouble() * 2f + 2f;
				}
				iOwner.mSpectralTargetAlpha = 1f;
				iZone.AnimationController.CrossFade(iOwner.mClips[7], 0.25f, true);
			}

			// Token: 0x06002E6C RID: 11884 RVA: 0x00178CA8 File Offset: 0x00176EA8
			public void OnUpdate(float iDeltaTime, BossSpellCasterZone iZone, Grimnir2 iOwner)
			{
				if (iZone.Index == 1)
				{
					iOwner.mLeftSpiritIdleTimer -= iDeltaTime;
					if (iOwner.mLeftSpiritIdleTimer <= 0f && iOwner.mLeftSpiritSpellEffect == null)
					{
						iOwner.ChangeSpiritState(Grimnir2.SpiritStates.Cast, 1);
						return;
					}
				}
				else
				{
					iOwner.mRightSpiritIdleTimer -= iDeltaTime;
					if (iOwner.mRightSpiritIdleTimer <= 0f && iOwner.mRightSpiritSpellEffect == null)
					{
						iOwner.ChangeSpiritState(Grimnir2.SpiritStates.Cast, 2);
					}
				}
			}

			// Token: 0x06002E6D RID: 11885 RVA: 0x00178D15 File Offset: 0x00176F15
			public void OnExit(BossSpellCasterZone iZone, Grimnir2 iOwner)
			{
			}

			// Token: 0x04003233 RID: 12851
			private static Grimnir2.SpecIdleState sSingelton;

			// Token: 0x04003234 RID: 12852
			private static volatile object sSingeltonLock = new object();
		}

		// Token: 0x020005FC RID: 1532
		protected class SpecCastState : Grimnir2.ISpiritState
		{
			// Token: 0x17000AED RID: 2797
			// (get) Token: 0x06002E6F RID: 11887 RVA: 0x00178D28 File Offset: 0x00176F28
			public static Grimnir2.SpecCastState Instance
			{
				get
				{
					if (Grimnir2.SpecCastState.sSingelton == null)
					{
						lock (Grimnir2.SpecCastState.sSingeltonLock)
						{
							if (Grimnir2.SpecCastState.sSingelton == null)
							{
								Grimnir2.SpecCastState.sSingelton = new Grimnir2.SpecCastState();
							}
						}
					}
					return Grimnir2.SpecCastState.sSingelton;
				}
			}

			// Token: 0x06002E70 RID: 11888 RVA: 0x00178D7C File Offset: 0x00176F7C
			private SpecCastState()
			{
			}

			// Token: 0x06002E71 RID: 11889 RVA: 0x00178D84 File Offset: 0x00176F84
			public void OnEnter(BossSpellCasterZone iZone, Grimnir2 iOwner)
			{
				if (iZone.Index == 1)
				{
					int num;
					do
					{
						num = Grimnir2.RANDOM.Next(Grimnir2.SPIRITSPELLS.Length);
					}
					while (num == iOwner.mLeftSpiritLastSpell);
					iOwner.mLeftSpiritHasCastSpell = false;
					iOwner.mLeftSpiritLastSpell = num;
					iOwner.mLeftCastAnimation = ((num < 1) ? Grimnir2.Animations.spirit_cast_railgrun : Grimnir2.Animations.spirit_cast_projectile);
					iZone.AnimationController.CrossFade(iOwner.mClips[(int)iOwner.mLeftCastAnimation], 0.15f, false);
					return;
				}
				int num2;
				do
				{
					num2 = Grimnir2.RANDOM.Next(Grimnir2.SPIRITSPELLS.Length);
				}
				while (num2 == iOwner.mRightSpiritLastSpell);
				iOwner.mRightSpiritHasCastSpell = false;
				iOwner.mRightSpiritLastSpell = num2;
				iOwner.mRightCastAnimation = ((num2 < 1) ? Grimnir2.Animations.spirit_cast_railgrun : Grimnir2.Animations.spirit_cast_projectile);
				iZone.AnimationController.CrossFade(iOwner.mClips[(int)iOwner.mRightCastAnimation], 0.15f, false);
			}

			// Token: 0x06002E72 RID: 11890 RVA: 0x00178E4C File Offset: 0x0017704C
			public unsafe void OnUpdate(float iDeltaTime, BossSpellCasterZone iZone, Grimnir2 iOwner)
			{
				if (NetworkManager.Instance.State != NetworkState.Client)
				{
					float num = iZone.AnimationController.Time / iZone.AnimationController.AnimationClip.Duration;
					if (!iZone.AnimationController.CrossFadeEnabled)
					{
						switch (iZone.Index)
						{
						default:
							if (!iOwner.mLeftSpiritHasCastSpell && ((iOwner.mLeftCastAnimation == Grimnir2.Animations.spirit_cast_projectile && num >= 0.6f) || (iOwner.mLeftCastAnimation == Grimnir2.Animations.spirit_cast_railgrun && num >= 0.15f)))
							{
								Grimnir2.SpellData spellData = Grimnir2.SPIRITSPELLS[iOwner.mLeftSpiritLastSpell];
								if (NetworkManager.Instance.State == NetworkState.Server)
								{
									Grimnir2.CastSpellMessage castSpellMessage;
									castSpellMessage.SpiritCast = true;
									castSpellMessage.SpiritIndex = 1;
									castSpellMessage.SpellIndex = iOwner.mLeftSpiritLastSpell;
									BossFight.Instance.SendMessage<Grimnir2.CastSpellMessage>(iOwner, 5, (void*)(&castSpellMessage), true);
								}
								iOwner.mLeftSpiritCastType = spellData.CASTTYPE;
								iOwner.mLeftSpiritSpellPower = spellData.SPELLPOWER;
								iOwner.mLeftSpiritHasCastSpell = true;
								spellData.SPELL.Cast(true, iZone, iOwner.mLeftSpiritCastType);
							}
							break;
						case 2:
							if (!iOwner.mRightSpiritHasCastSpell && ((iOwner.mRightCastAnimation == Grimnir2.Animations.spirit_cast_projectile && num >= 0.6f) || (iOwner.mRightCastAnimation == Grimnir2.Animations.spirit_cast_railgrun && num >= 0.15f)))
							{
								Grimnir2.SpellData spellData2 = Grimnir2.SPIRITSPELLS[iOwner.mRightSpiritLastSpell];
								if (NetworkManager.Instance.State == NetworkState.Server)
								{
									Grimnir2.CastSpellMessage castSpellMessage2;
									castSpellMessage2.SpiritCast = true;
									castSpellMessage2.SpiritIndex = 2;
									castSpellMessage2.SpellIndex = iOwner.mRightSpiritLastSpell;
									BossFight.Instance.SendMessage<Grimnir2.CastSpellMessage>(iOwner, 5, (void*)(&castSpellMessage2), true);
								}
								iOwner.mRightSpiritCastType = spellData2.CASTTYPE;
								iOwner.mRightSpiritSpellPower = spellData2.SPELLPOWER;
								iOwner.mRightSpiritHasCastSpell = true;
								spellData2.SPELL.Cast(true, iZone, iOwner.mRightSpiritCastType);
							}
							break;
						}
						if (iZone.AnimationController.HasFinished)
						{
							iOwner.ChangeSpiritState(Grimnir2.SpiritStates.Idle, iZone.Index);
						}
					}
				}
			}

			// Token: 0x06002E73 RID: 11891 RVA: 0x00179040 File Offset: 0x00177240
			public void OnExit(BossSpellCasterZone iZone, Grimnir2 iOwner)
			{
				switch (iZone.Index)
				{
				default:
					if (iOwner.mLeftSpiritSpellEffect != null)
					{
						iOwner.mLeftSpiritSpellEffect.Stop(iZone);
						return;
					}
					break;
				case 2:
					if (iOwner.mRightSpiritSpellEffect != null)
					{
						iOwner.mRightSpiritSpellEffect.Stop(iZone);
					}
					break;
				}
			}

			// Token: 0x04003235 RID: 12853
			private static Grimnir2.SpecCastState sSingelton;

			// Token: 0x04003236 RID: 12854
			private static volatile object sSingeltonLock = new object();
		}

		// Token: 0x020005FD RID: 1533
		protected class SpecHealState : Grimnir2.ISpiritState
		{
			// Token: 0x17000AEE RID: 2798
			// (get) Token: 0x06002E75 RID: 11893 RVA: 0x0017909C File Offset: 0x0017729C
			public static Grimnir2.SpecHealState Instance
			{
				get
				{
					if (Grimnir2.SpecHealState.sSingelton == null)
					{
						lock (Grimnir2.SpecHealState.sSingeltonLock)
						{
							if (Grimnir2.SpecHealState.sSingelton == null)
							{
								Grimnir2.SpecHealState.sSingelton = new Grimnir2.SpecHealState();
							}
						}
					}
					return Grimnir2.SpecHealState.sSingelton;
				}
			}

			// Token: 0x06002E76 RID: 11894 RVA: 0x001790F0 File Offset: 0x001772F0
			private SpecHealState()
			{
			}

			// Token: 0x06002E77 RID: 11895 RVA: 0x001790F8 File Offset: 0x001772F8
			public void OnEnter(BossSpellCasterZone iZone, Grimnir2 iOwner)
			{
				iZone.AnimationController.CrossFade(iOwner.mClips[11], 0.25f, true);
				if (iZone.Index == 1)
				{
					iOwner.mLeftSpiritIdleTimer = 0.25f;
					return;
				}
				iOwner.mRightSpiritIdleTimer = 0.25f;
			}

			// Token: 0x06002E78 RID: 11896 RVA: 0x00179134 File Offset: 0x00177334
			public void OnUpdate(float iDeltaTime, BossSpellCasterZone iZone, Grimnir2 iOwner)
			{
				float num;
				if (iZone.Index == 1)
				{
					num = (iOwner.mLeftSpiritIdleTimer -= iDeltaTime);
				}
				else
				{
					num = (iOwner.mRightSpiritIdleTimer -= iDeltaTime);
				}
				if (num <= 0f)
				{
					iOwner.mSpectralTargetAlpha = ((Grimnir2.RANDOM.NextDouble() > 0.0) ? 0f : 1f);
					if (iZone.Index == 1)
					{
						iOwner.mLeftSpiritIdleTimer = 0.25f;
					}
					else
					{
						iOwner.mRightSpiritIdleTimer = 0.25f;
					}
				}
				if (!iOwner.mAssaturHeal)
				{
					iOwner.ChangeSpiritState(Grimnir2.SpiritStates.Idle, iZone.Index);
				}
			}

			// Token: 0x06002E79 RID: 11897 RVA: 0x001791DB File Offset: 0x001773DB
			public void OnExit(BossSpellCasterZone iZone, Grimnir2 iOwner)
			{
				iOwner.mSpectralTargetAlpha = 1f;
			}

			// Token: 0x04003237 RID: 12855
			private static Grimnir2.SpecHealState sSingelton;

			// Token: 0x04003238 RID: 12856
			private static volatile object sSingeltonLock = new object();
		}

		// Token: 0x020005FE RID: 1534
		protected class SpecDieState : Grimnir2.ISpiritState
		{
			// Token: 0x17000AEF RID: 2799
			// (get) Token: 0x06002E7B RID: 11899 RVA: 0x001791F8 File Offset: 0x001773F8
			public static Grimnir2.SpecDieState Instance
			{
				get
				{
					if (Grimnir2.SpecDieState.sSingelton == null)
					{
						lock (Grimnir2.SpecDieState.sSingeltonLock)
						{
							if (Grimnir2.SpecDieState.sSingelton == null)
							{
								Grimnir2.SpecDieState.sSingelton = new Grimnir2.SpecDieState();
							}
						}
					}
					return Grimnir2.SpecDieState.sSingelton;
				}
			}

			// Token: 0x06002E7C RID: 11900 RVA: 0x0017924C File Offset: 0x0017744C
			private SpecDieState()
			{
			}

			// Token: 0x06002E7D RID: 11901 RVA: 0x00179254 File Offset: 0x00177454
			public void OnEnter(BossSpellCasterZone iZone, Grimnir2 iOwner)
			{
				iOwner.mSpectralTargetAlpha = 0f;
				iZone.AnimationController.CrossFade(iOwner.mClips[10], 0.15f, true);
			}

			// Token: 0x06002E7E RID: 11902 RVA: 0x0017927B File Offset: 0x0017747B
			public void OnUpdate(float iDeltaTime, BossSpellCasterZone iZone, Grimnir2 iOwner)
			{
			}

			// Token: 0x06002E7F RID: 11903 RVA: 0x0017927D File Offset: 0x0017747D
			public void OnExit(BossSpellCasterZone iZone, Grimnir2 iOwner)
			{
			}

			// Token: 0x04003239 RID: 12857
			private static Grimnir2.SpecDieState sSingelton;

			// Token: 0x0400323A RID: 12858
			private static volatile object sSingeltonLock = new object();
		}

		// Token: 0x020005FF RID: 1535
		private enum MessageType : ushort
		{
			// Token: 0x0400323C RID: 12860
			Update,
			// Token: 0x0400323D RID: 12861
			Script,
			// Token: 0x0400323E RID: 12862
			ChangeState,
			// Token: 0x0400323F RID: 12863
			ChangeSpiritState,
			// Token: 0x04003240 RID: 12864
			GrimnirSpell,
			// Token: 0x04003241 RID: 12865
			CastSpell,
			// Token: 0x04003242 RID: 12866
			CastMagick,
			// Token: 0x04003243 RID: 12867
			ChangeTarget,
			// Token: 0x04003244 RID: 12868
			Corporealize
		}

		// Token: 0x02000600 RID: 1536
		internal struct UpdateMessage
		{
			// Token: 0x04003245 RID: 12869
			public const ushort TYPE = 0;

			// Token: 0x04003246 RID: 12870
			public Matrix LeftOrientation;

			// Token: 0x04003247 RID: 12871
			public Matrix RightOrientation;

			// Token: 0x04003248 RID: 12872
			public float LeftFloatCounter;

			// Token: 0x04003249 RID: 12873
			public float RightFloatCounter;

			// Token: 0x0400324A RID: 12874
			public float SpiritTimer;

			// Token: 0x0400324B RID: 12875
			public byte Animation;

			// Token: 0x0400324C RID: 12876
			public float AnimationTime;

			// Token: 0x0400324D RID: 12877
			public float Hitpoints;
		}

		// Token: 0x02000601 RID: 1537
		internal struct ScriptMessageMessage
		{
			// Token: 0x0400324E RID: 12878
			public const ushort TYPE = 1;

			// Token: 0x0400324F RID: 12879
			public BossMessages Message;
		}

		// Token: 0x02000602 RID: 1538
		internal struct CorpMessage
		{
			// Token: 0x04003250 RID: 12880
			public const ushort TYPE = 8;

			// Token: 0x04003251 RID: 12881
			public bool Dummy;
		}

		// Token: 0x02000603 RID: 1539
		internal struct ChangeStateMessage
		{
			// Token: 0x04003252 RID: 12882
			public const ushort TYPE = 2;

			// Token: 0x04003253 RID: 12883
			public Grimnir2.States NewState;
		}

		// Token: 0x02000604 RID: 1540
		internal struct CastMagickMessage
		{
			// Token: 0x04003254 RID: 12884
			public const ushort TYPE = 6;

			// Token: 0x04003255 RID: 12885
			public MagickType Magick;
		}

		// Token: 0x02000605 RID: 1541
		internal struct CastSpellMessage
		{
			// Token: 0x04003256 RID: 12886
			public const ushort TYPE = 5;

			// Token: 0x04003257 RID: 12887
			public bool SpiritCast;

			// Token: 0x04003258 RID: 12888
			public int SpiritIndex;

			// Token: 0x04003259 RID: 12889
			public int SpellIndex;
		}

		// Token: 0x02000606 RID: 1542
		internal struct ChangeSpiritStateMessage
		{
			// Token: 0x0400325A RID: 12890
			public const ushort TYPE = 3;

			// Token: 0x0400325B RID: 12891
			public Grimnir2.SpiritStates NewState;

			// Token: 0x0400325C RID: 12892
			public int SpiritIndex;
		}

		// Token: 0x02000607 RID: 1543
		internal struct ChangeTargetMessage
		{
			// Token: 0x0400325D RID: 12893
			public const ushort TYPE = 7;

			// Token: 0x0400325E RID: 12894
			public ushort Target;
		}

		// Token: 0x02000608 RID: 1544
		public enum States
		{
			// Token: 0x04003260 RID: 12896
			Idle,
			// Token: 0x04003261 RID: 12897
			Spirit,
			// Token: 0x04003262 RID: 12898
			Magick,
			// Token: 0x04003263 RID: 12899
			Spell,
			// Token: 0x04003264 RID: 12900
			Heal,
			// Token: 0x04003265 RID: 12901
			Die
		}

		// Token: 0x02000609 RID: 1545
		public enum SpiritStates
		{
			// Token: 0x04003267 RID: 12903
			Intro,
			// Token: 0x04003268 RID: 12904
			Idle,
			// Token: 0x04003269 RID: 12905
			Cast,
			// Token: 0x0400326A RID: 12906
			Heal,
			// Token: 0x0400326B RID: 12907
			Die
		}

		// Token: 0x0200060A RID: 1546
		public struct SpellData
		{
			// Token: 0x0400326C RID: 12908
			public Spell SPELL;

			// Token: 0x0400326D RID: 12909
			public CastType CASTTYPE;

			// Token: 0x0400326E RID: 12910
			public float SPELLPOWER;
		}

		// Token: 0x0200060B RID: 1547
		private enum AsaAnimation
		{
			// Token: 0x04003270 RID: 12912
			idle,
			// Token: 0x04003271 RID: 12913
			heal,
			// Token: 0x04003272 RID: 12914
			cut,
			// Token: 0x04003273 RID: 12915
			NrOfAnimations
		}

		// Token: 0x0200060C RID: 1548
		private enum Animations
		{
			// Token: 0x04003275 RID: 12917
			die,
			// Token: 0x04003276 RID: 12918
			hanging,
			// Token: 0x04003277 RID: 12919
			taunt,
			// Token: 0x04003278 RID: 12920
			talk0,
			// Token: 0x04003279 RID: 12921
			talk1,
			// Token: 0x0400327A RID: 12922
			talk2,
			// Token: 0x0400327B RID: 12923
			idle,
			// Token: 0x0400327C RID: 12924
			spirit_idle,
			// Token: 0x0400327D RID: 12925
			spirit_cast_projectile,
			// Token: 0x0400327E RID: 12926
			spirit_cast_railgrun,
			// Token: 0x0400327F RID: 12927
			spirit_die,
			// Token: 0x04003280 RID: 12928
			cast_magick_global,
			// Token: 0x04003281 RID: 12929
			cast_magick_direct,
			// Token: 0x04003282 RID: 12930
			cast_spirit,
			// Token: 0x04003283 RID: 12931
			cast_shield,
			// Token: 0x04003284 RID: 12932
			NrOfAnimations
		}
	}
}
