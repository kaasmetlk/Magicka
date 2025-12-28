using System;
using System.Collections.Generic;
using JigLibX.Geometry;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Magicka.Graphics.Lights;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using PolygonHead.Lights;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000316 RID: 790
	internal class Napalm : SpecialAbility, IAbilityEffect
	{
		// Token: 0x1700061A RID: 1562
		// (get) Token: 0x06001848 RID: 6216 RVA: 0x000A0CB4 File Offset: 0x0009EEB4
		public static Napalm Instance
		{
			get
			{
				if (Napalm.sSingelton == null)
				{
					lock (Napalm.sSingeltonLock)
					{
						if (Napalm.sSingelton == null)
						{
							Napalm.sSingelton = new Napalm();
						}
					}
				}
				return Napalm.sSingelton;
			}
		}

		// Token: 0x06001849 RID: 6217 RVA: 0x000A0D08 File Offset: 0x0009EF08
		static Napalm()
		{
			Napalm.sNapalmDamage.AddDamage(new Damage(AttackProperties.Damage, Elements.Fire, 2000f, 1f));
			Napalm.sNapalmDamage.AddDamage(new Damage(AttackProperties.Knockback, Elements.Earth, 500f, 2f));
			Napalm.sNapalmDamage.AddDamage(new Damage(AttackProperties.Status, Elements.Fire, 300f, 3f));
		}

		// Token: 0x0600184A RID: 6218 RVA: 0x000A0DBC File Offset: 0x0009EFBC
		private Napalm() : base(Animations.cast_magick_sweep, "#magick_napalm".GetHashCodeCustom())
		{
			try
			{
				if (Napalm.sModel == null)
				{
					lock (Game.Instance.GraphicsDevice)
					{
						Napalm.sModel = Game.Instance.Content.Load<Model>("Models/Magicks/f4");
					}
				}
				this.mRenderData = new Napalm.RenderData[3];
				for (int i = 0; i < 3; i++)
				{
					this.mRenderData[i] = new Napalm.RenderData();
					this.mRenderData[i].SetMesh(Napalm.sModel.Meshes[0], Napalm.sModel.Meshes[0].MeshParts[0], 4, 0, 5);
				}
			}
			catch
			{
			}
			this.mDead = true;
		}

		// Token: 0x0600184B RID: 6219 RVA: 0x000A0EA0 File Offset: 0x0009F0A0
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			if (Napalm.sModel == null)
			{
				return false;
			}
			if (iPlayState.Level.CurrentScene.Indoors)
			{
				AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL, iOwner.AudioEmitter);
				return false;
			}
			if (this.mDead)
			{
				this.mNoLight = (iPlayState.Level.CurrentScene.DirectionalLightSettings.Length == 0);
				EffectManager.Instance.Stop(ref this.mSmokeEffect);
				this.mDead = false;
				this.mOwner = iOwner;
				Vector3 position = iOwner.Position;
				Vector3 direction = iOwner.Direction;
				Vector3.Multiply(ref direction, 15f, out direction);
				Vector3.Add(ref position, ref direction, out position);
				direction = iOwner.Direction;
				Matrix matrix;
				Matrix.CreateRotationY(1.5707964f, out matrix);
				Vector3.TransformNormal(ref direction, ref matrix, out direction);
				Vector3 up = Vector3.Up;
				Matrix.CreateWorld(ref position, ref direction, ref up, out this.mFocalPoint);
				this.mPlayState = iPlayState;
				EffectManager.Instance.StartEffect(Napalm.TARGET_EFFECT, ref this.mFocalPoint, out this.mSmokeEffect);
				this.mTimer = 5f;
				this.mAirstrike = false;
				this.mNapalmHits = 0;
				SpellManager.Instance.AddSpellEffect(this);
				return true;
			}
			if (this.mAirstrike)
			{
				AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL, iOwner.AudioEmitter);
				return false;
			}
			this.mNoLight = (iPlayState.Level.CurrentScene.DirectionalLightSettings.Length == 0);
			this.mOwner = iOwner;
			Vector3 position2 = iOwner.Position;
			Vector3 direction2 = iOwner.Direction;
			Vector3.Multiply(ref direction2, 15f, out direction2);
			Vector3.Add(ref position2, ref direction2, out position2);
			this.mFocalPoint.Translation = position2;
			EffectManager.Instance.Stop(ref this.mSmokeEffect);
			EffectManager.Instance.StartEffect(Napalm.TARGET_EFFECT, ref this.mFocalPoint, out this.mSmokeEffect);
			return true;
		}

		// Token: 0x1700061B RID: 1563
		// (get) Token: 0x0600184C RID: 6220 RVA: 0x000A1070 File Offset: 0x0009F270
		public bool IsDead
		{
			get
			{
				return this.mDead;
			}
		}

		// Token: 0x0600184D RID: 6221 RVA: 0x000A1078 File Offset: 0x0009F278
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mTimer -= iDeltaTime;
			if (this.mAirstrike)
			{
				Vector3 forward = this.mFocalPoint.Forward;
				Vector3 translation = this.mFocalPoint.Translation;
				Vector3 vector = translation;
				Vector3 vector2;
				Vector3.Multiply(ref forward, -120f, out vector2);
				Vector3.Add(ref vector2, ref translation, out translation);
				Vector3.Multiply(ref forward, 120f, out vector2);
				Vector3.Add(ref vector2, ref vector, out vector);
				float num = 1f - this.mTimer / 2f;
				float num2 = 0.25f;
				float num3 = 0.5f + num2 - 0.044999998f;
				float num4 = num3 + (float)this.mNapalmHits * 0.015f;
				Vector3 translation2;
				Vector3.Lerp(ref vector, ref translation, num, out translation2);
				Matrix transform = this.mFocalPoint;
				transform.Translation = translation2;
				if (NetworkManager.Instance.State != NetworkState.Client && num >= num3 && num >= num4 && this.mNapalmHits < 6)
				{
					Segment iSeg = default(Segment);
					Vector3.Lerp(ref vector, ref translation, num - num2, out iSeg.Origin);
					iSeg.Origin.Y = iSeg.Origin.Y + 10f;
					iSeg.Delta.Y = -20f;
					float num5;
					Vector3 vector3;
					Vector3 up;
					AnimatedLevelPart animatedLevelPart;
					bool flag = this.mPlayState.Level.CurrentScene.SegmentIntersect(out num5, out vector3, out up, out animatedLevelPart, iSeg);
					bool flag2 = false;
					if (!flag)
					{
						for (int i = 0; i < this.mPlayState.Level.CurrentScene.Liquids.Length; i++)
						{
							if (this.mPlayState.Level.CurrentScene.Liquids[i].SegmentIntersect(out num5, out vector3, out up, ref iSeg, true, true, false))
							{
								flag2 = true;
								flag = true;
								break;
							}
						}
					}
					if (flag)
					{
						up = Vector3.Up;
						VisualEffectReference visualEffectReference;
						EffectManager.Instance.StartEffect(Napalm.NAPALM_EFFECT, ref vector3, ref up, out visualEffectReference);
						AudioManager.Instance.PlayCue(Banks.Additional, Napalm.NAPALM_SOUND);
						this.mPlayState.Camera.CameraShake(vector3, 0.25f, 0.5f);
						Grease.GreaseField greaseField = null;
						if (!flag2)
						{
							greaseField = Grease.GreaseField.GetInstance(this.mPlayState);
							greaseField.Initialize(this.mOwner, animatedLevelPart, ref vector3, ref up);
							greaseField.Burn(3f);
							this.mPlayState.EntityManager.AddEntity(greaseField);
						}
						Napalm.NapalmBlast(this.mPlayState, ref vector3, this.mOwner as Entity, this.mTimeStamp);
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
							triggerActionMessage.Handle = this.mOwner.Handle;
							triggerActionMessage.Position = vector3;
							triggerActionMessage.Direction = up;
							if (animatedLevelPart != null)
							{
								triggerActionMessage.Arg = (int)animatedLevelPart.Handle;
							}
							else
							{
								triggerActionMessage.Arg = 65535;
							}
							if (greaseField != null)
							{
								triggerActionMessage.Id = (int)greaseField.Handle;
							}
							else
							{
								triggerActionMessage.Id = int.MinValue;
							}
							triggerActionMessage.ActionType = TriggerActionType.NapalmStrike;
							triggerActionMessage.TimeStamp = this.mTimeStamp;
							NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
						}
					}
					this.mNapalmHits++;
				}
				if (!this.mNoLight)
				{
					Matrix matrix;
					Matrix.CreateScale(2.5f, out matrix);
					Matrix.Multiply(ref matrix, ref transform, out transform);
					Vector3 lightDirection = (this.mPlayState.Level.CurrentScene.DirectionalLightSettings[0].Light as DirectionalLight).LightDirection;
					Vector3.Negate(ref lightDirection, out lightDirection);
					Vector3.Multiply(ref lightDirection, 20f, out lightDirection);
					Vector3.Add(ref lightDirection, ref translation2, out translation2);
					transform.Translation = translation2;
					this.mRenderData[(int)iDataChannel].Transform = transform;
					this.mRenderData[(int)iDataChannel].mBoundingSphere.Center = this.mFocalPoint.Translation;
					this.mRenderData[(int)iDataChannel].mBoundingSphere.Radius = 30f;
					this.mPlayState.Scene.AddRenderableObject(iDataChannel, this.mRenderData[(int)iDataChannel]);
				}
				if (this.mTimer <= 0f)
				{
					this.mDead = true;
					return;
				}
			}
			else if (this.mTimer <= 0f)
			{
				this.mAirstrike = true;
				this.mTimer = 2f;
				this.mPlayState.Camera.CameraShake(this.mFocalPoint.Translation, 0.5f, 2f);
				AudioManager.Instance.PlayCue(Banks.Additional, Napalm.PLANE_PASSBY_SOUND);
			}
		}

		// Token: 0x0600184E RID: 6222 RVA: 0x000A14E6 File Offset: 0x0009F6E6
		public void OnRemove()
		{
			EffectManager.Instance.Stop(ref this.mSmokeEffect);
			this.mDead = true;
			this.mAirstrike = false;
		}

		// Token: 0x0600184F RID: 6223 RVA: 0x000A1508 File Offset: 0x0009F708
		internal static DamageResult NapalmBlast(PlayState iPlayState, ref Vector3 iPosition, Entity iOwner, double iTimeStamp)
		{
			List<Entity> entities = iPlayState.EntityManager.GetEntities(iPosition, 5f, true, false);
			for (int i = 0; i < entities.Count; i++)
			{
				IDamageable damageable = entities[i] as IDamageable;
				if (damageable != null && (!(damageable is Character) || !(damageable as Character).IsEthereal))
				{
					damageable.Damage(Napalm.sNapalmDamage, iOwner, iTimeStamp, iPosition);
				}
			}
			iPlayState.EntityManager.ReturnEntityList(entities);
			Vector3 vector = new Vector3(3.75f, 0f, 0f);
			Damage c = Napalm.sNapalmDamage.C;
			Liquid.Freeze(iPlayState.Level.CurrentScene, ref iPosition, ref vector, 6.2831855f, 10f, ref c);
			DynamicLight cachedLight = DynamicLight.GetCachedLight();
			Vector3 iPosition2 = iPosition;
			iPosition2.Y += 0.5f;
			Vector3 iColor = new Vector3(1f, 1f, 0f);
			cachedLight.Initialize(iPosition2, iColor, 3f, 16f, 5f, 1.5f, 0.5f);
			cachedLight.Enable(iPlayState.Scene);
			cachedLight.Disable(LightTransitionType.Linear, 2f);
			return DamageResult.None;
		}

		// Token: 0x06001850 RID: 6224 RVA: 0x000A1641 File Offset: 0x0009F841
		private static void LightCallback(DynamicLight iLight)
		{
		}

		// Token: 0x04001A06 RID: 6662
		internal const int NR_OF_FIELDS = 6;

		// Token: 0x04001A07 RID: 6663
		private const int NAPALM_HITS = 6;

		// Token: 0x04001A08 RID: 6664
		private const float NAPALM_TIME = 2f;

		// Token: 0x04001A09 RID: 6665
		private const float NAPALM_WAIT_TIME = 5f;

		// Token: 0x04001A0A RID: 6666
		private static Napalm sSingelton;

		// Token: 0x04001A0B RID: 6667
		private static volatile object sSingeltonLock = new object();

		// Token: 0x04001A0C RID: 6668
		private static Model sModel;

		// Token: 0x04001A0D RID: 6669
		private static readonly int TARGET_EFFECT = "magick_smoke_target".GetHashCodeCustom();

		// Token: 0x04001A0E RID: 6670
		public static readonly int NAPALM_EFFECT = "magick_napalm".GetHashCodeCustom();

		// Token: 0x04001A0F RID: 6671
		public static readonly int NAPALM_SOUND = "magick_napalm".GetHashCodeCustom();

		// Token: 0x04001A10 RID: 6672
		private static readonly int PLANE_PASSBY_SOUND = "magick_plane_passby".GetHashCodeCustom();

		// Token: 0x04001A11 RID: 6673
		public static readonly DamageCollection5 sNapalmDamage = default(DamageCollection5);

		// Token: 0x04001A12 RID: 6674
		private bool mDead;

		// Token: 0x04001A13 RID: 6675
		private Napalm.RenderData[] mRenderData;

		// Token: 0x04001A14 RID: 6676
		private int mNapalmHits;

		// Token: 0x04001A15 RID: 6677
		private float mTimer;

		// Token: 0x04001A16 RID: 6678
		private PlayState mPlayState;

		// Token: 0x04001A17 RID: 6679
		private ISpellCaster mOwner;

		// Token: 0x04001A18 RID: 6680
		private VisualEffectReference mSmokeEffect;

		// Token: 0x04001A19 RID: 6681
		private bool mAirstrike;

		// Token: 0x04001A1A RID: 6682
		private Matrix mFocalPoint;

		// Token: 0x04001A1B RID: 6683
		private bool mNoLight;

		// Token: 0x02000317 RID: 791
		private class RenderData : RenderableObject<RenderDeferredEffect, RenderDeferredMaterial>
		{
			// Token: 0x1700061C RID: 1564
			// (get) Token: 0x06001851 RID: 6225 RVA: 0x000A1643 File Offset: 0x0009F843
			public override int DepthTechnique
			{
				get
				{
					return -1;
				}
			}

			// Token: 0x06001852 RID: 6226 RVA: 0x000A1646 File Offset: 0x0009F846
			public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
			}

			// Token: 0x06001853 RID: 6227 RVA: 0x000A1648 File Offset: 0x0009F848
			public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				this.mMaterial.WorldTransform = this.Transform;
				base.DrawShadow(iEffect, iViewFrustum);
			}

			// Token: 0x04001A1C RID: 6684
			public Matrix Transform;
		}
	}
}
