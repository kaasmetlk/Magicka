using System;
using System.Collections.Generic;
using JigLibX.Geometry;
using Magicka.Audio;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.Graphics;
using Magicka.Graphics.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Magicka.GameLogic.Spells.SpellEffects
{
	// Token: 0x0200042B RID: 1067
	public class PushSpell : SpellEffect
	{
		// Token: 0x06002113 RID: 8467 RVA: 0x000EB56C File Offset: 0x000E976C
		public static void IntializeCache(int iNum)
		{
			PushSpell.mCache = new List<PushSpell>(iNum);
			for (int i = 0; i < iNum; i++)
			{
				PushSpell.mCache.Add(new PushSpell());
			}
		}

		// Token: 0x06002114 RID: 8468 RVA: 0x000EB5A0 File Offset: 0x000E97A0
		public static SpellEffect GetFromCache()
		{
			if (PushSpell.mCache.Count <= 0)
			{
				return new PushSpell();
			}
			PushSpell pushSpell = PushSpell.mCache[PushSpell.mCache.Count - 1];
			PushSpell.mCache.Remove(pushSpell);
			SpellEffect.mPlayState.SpellEffects.Add(pushSpell);
			return pushSpell;
		}

		// Token: 0x06002115 RID: 8469 RVA: 0x000EB5F4 File Offset: 0x000E97F4
		public static void ReturnToCache(PushSpell iEffect)
		{
			iEffect.mHitList.Clear();
			SpellEffect.mPlayState.SpellEffects.Remove(iEffect);
			PushSpell.mCache.Add(iEffect);
		}

		// Token: 0x06002116 RID: 8470 RVA: 0x000EB61D File Offset: 0x000E981D
		public PushSpell()
		{
			this.mHitList = new List<Entity>(256);
		}

		// Token: 0x06002117 RID: 8471 RVA: 0x000EB638 File Offset: 0x000E9838
		public override void CastArea(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
		{
			base.CastArea(iSpell, iOwner, iFromStaff);
			this.mOwner = iOwner;
			this.mTimeStamp = iOwner.PlayState.PlayTime;
			this.mMinTTL = 0.4f;
			this.mForce = 50f + 500f * iOwner.SpellPower;
			this.mElevation = iOwner.SpellPower * 0.1f;
			this.mDamage = new Damage(AttackProperties.Pushed, Elements.Earth, this.mForce, 1.5f);
			base.Active = true;
			this.mAngle = 3.1415927f;
			this.mRange = 3.5f;
			this.mTime = 0.25f;
			RadialBlur radialBlur = RadialBlur.GetRadialBlur();
			this.mInitialDirection = iOwner.Direction;
			Vector3 position = iOwner.Position;
			radialBlur.Initialize(ref position, ref this.mInitialDirection, this.mAngle, this.mRange, 0.5f, iOwner.PlayState.Scene);
			Vector3 position2 = iOwner.Position;
			Vector3 direction = iOwner.Direction;
			VisualEffectReference visualEffectReference;
			EffectManager.Instance.StartEffect(PushSpell.PUSH_AREA, ref position2, ref direction, out visualEffectReference);
			PushSpell.PushMagnitudeVolumAdjustAndOthers iVariables = default(PushSpell.PushMagnitudeVolumAdjustAndOthers);
			iVariables.magnitude = iOwner.SpellPower;
			AudioManager.Instance.PlayCue<PushSpell.PushMagnitudeVolumAdjustAndOthers>(Banks.Spells, PushSpell.PUSH_STR_AREA, iVariables, iOwner.AudioEmitter);
			iOwner.SpellPower = 0f;
		}

		// Token: 0x06002118 RID: 8472 RVA: 0x000EB780 File Offset: 0x000E9980
		public override void CastForce(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
		{
			base.CastForce(iSpell, iOwner, iFromStaff);
			this.mOwner = iOwner;
			this.mTimeStamp = iOwner.PlayState.PlayTime;
			if (!((iOwner as Character).CurrentState is PanicCastState) && iOwner is Character && this.mFromStaff)
			{
				this.mFromStaff = false;
			}
			this.mMinTTL = 0.4f;
			this.mForce = 50f + 500f * iOwner.SpellPower;
			this.mElevation = iOwner.SpellPower * 0.1f;
			this.mDamage = new Damage(AttackProperties.Pushed, Elements.Earth, this.mForce, 1.5f);
			base.Active = true;
			this.mAngle = 0.5235988f;
			this.mRange = 10f;
			this.mTime = 0.25f;
			RadialBlur radialBlur = RadialBlur.GetRadialBlur();
			this.mInitialDirection = iOwner.Direction;
			Vector3 position = iOwner.Position;
			radialBlur.Initialize(ref position, ref this.mInitialDirection, this.mAngle + this.mAngle * 0.2f, this.mRange, 0.5f, iOwner.PlayState.Scene);
			Vector3 translation = iOwner.CastSource.Translation;
			Vector3 direction = iOwner.Direction;
			VisualEffectReference visualEffectReference;
			if (EffectManager.Instance.StartEffect(PushSpell.PUSH_FORCE, ref translation, ref direction, out visualEffectReference))
			{
				PushSpell.PushMagnitudeVolumAdjustAndOthers iVariables = default(PushSpell.PushMagnitudeVolumAdjustAndOthers);
				iVariables.magnitude = iOwner.SpellPower;
				AudioManager.Instance.PlayCue<PushSpell.PushMagnitudeVolumAdjustAndOthers>(Banks.Spells, PushSpell.PUSH_STR_FORCE, iVariables, iOwner.AudioEmitter);
			}
			iOwner.SpellPower = 0f;
		}

		// Token: 0x06002119 RID: 8473 RVA: 0x000EB905 File Offset: 0x000E9B05
		public override void CastSelf(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
		{
			base.CastSelf(iSpell, iOwner, iFromStaff);
			this.mOwner = iOwner;
			this.mTimeStamp = iOwner.PlayState.PlayTime;
			this.mMinTTL = 0.4f;
		}

		// Token: 0x0600211A RID: 8474 RVA: 0x000EB933 File Offset: 0x000E9B33
		public override void CastWeapon(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
		{
			base.CastWeapon(iSpell, iOwner, iFromStaff);
			this.mOwner = iOwner;
			this.mTimeStamp = iOwner.PlayState.PlayTime;
			this.mMinTTL = 0.4f;
		}

		// Token: 0x0600211B RID: 8475 RVA: 0x000EB964 File Offset: 0x000E9B64
		public override bool CastUpdate(float iDeltaTime, ISpellCaster iOwner, out float oTurnSpeed)
		{
			oTurnSpeed = 0f;
			this.mTime -= iDeltaTime;
			this.mMinTTL -= iDeltaTime;
			if (this.mTime > 0f)
			{
				float num = (1f - MathHelper.Clamp(this.mTime, 0f, 0.25f) / 0.25f) * this.mRange * 0.75f + this.mRange * 0.25f;
				List<Entity> entities = iOwner.PlayState.EntityManager.GetEntities(iOwner.Position, num, true);
				entities.Remove(iOwner as Entity);
				Vector3 position = iOwner.Position;
				Segment iSeg;
				iSeg.Origin = position;
				for (int i = 0; i < entities.Count; i++)
				{
					Entity entity = entities[i];
					Vector3 vector;
					if (!this.mHitList.Contains(entity) && !entity.Dead && (this.mAngle == 3.1415927f || entity.ArcIntersect(out vector, position, this.mInitialDirection, num, this.mAngle, 4f)))
					{
						iSeg.Delta = entity.Position;
						Vector3.Subtract(ref iSeg.Delta, ref iSeg.Origin, out iSeg.Delta);
						float num2;
						Vector3 vector2;
						Vector3 vector3;
						if (!iOwner.PlayState.Level.CurrentScene.SegmentIntersect(out num2, out vector2, out vector3, iSeg))
						{
							Vector3 iDirection = entity.Position - iOwner.Position;
							float num3 = iDirection.Length();
							if (num3 > 1E-45f)
							{
								iDirection.Y = 0f;
								Vector3.Divide(ref iDirection, num3, out iDirection);
								float iDistance = 1.5f;
								this.mHitList.Add(entity);
								if (entity is IDamageable && !(entity is MissileEntity))
								{
									(entity as IDamageable).Damage(this.mDamage, this.mOwner as Entity, this.mTimeStamp, this.mOwner.Position);
								}
								else
								{
									entity.AddImpulseVelocity(iDirection, 0.17453292f + this.mElevation * 0.7853982f * 0.5f, this.mForce, iDistance);
								}
							}
						}
					}
				}
				iOwner.PlayState.EntityManager.ReturnEntityList(entities);
			}
			else if (this.mMinTTL < 0f)
			{
				this.DeInitialize(iOwner);
				return false;
			}
			return true;
		}

		// Token: 0x0600211C RID: 8476 RVA: 0x000EBBBB File Offset: 0x000E9DBB
		public override void DeInitialize(ISpellCaster iOwner)
		{
			if (!base.Active)
			{
				return;
			}
			base.Active = false;
			PushSpell.ReturnToCache(this);
		}

		// Token: 0x040023A8 RID: 9128
		private static List<PushSpell> mCache;

		// Token: 0x040023A9 RID: 9129
		private static int PUSH_FORCE = "push_force".GetHashCodeCustom();

		// Token: 0x040023AA RID: 9130
		private static int PUSH_AREA = "push_area".GetHashCodeCustom();

		// Token: 0x040023AB RID: 9131
		private static string PUSH_VARIABLE = "Magnitude";

		// Token: 0x040023AC RID: 9132
		private static int PUSH_STR_FORCE = "spell_push_force".GetHashCodeCustom();

		// Token: 0x040023AD RID: 9133
		private static int PUSH_STR_AREA = "spell_push_area".GetHashCodeCustom();

		// Token: 0x040023AE RID: 9134
		private List<Entity> mHitList;

		// Token: 0x040023AF RID: 9135
		private float mRange;

		// Token: 0x040023B0 RID: 9136
		private float mTime;

		// Token: 0x040023B1 RID: 9137
		private float mAngle;

		// Token: 0x040023B2 RID: 9138
		private float mElevation;

		// Token: 0x040023B3 RID: 9139
		private float mForce;

		// Token: 0x040023B4 RID: 9140
		private Vector3 mInitialDirection;

		// Token: 0x040023B5 RID: 9141
		private ISpellCaster mOwner;

		// Token: 0x040023B6 RID: 9142
		private Damage mDamage;

		// Token: 0x040023B7 RID: 9143
		private new double mTimeStamp;

		// Token: 0x0200042C RID: 1068
		public struct PushMagnitudeVolumAdjustAndOthers : IAudioVariables
		{
			// Token: 0x0600211E RID: 8478 RVA: 0x000EBC27 File Offset: 0x000E9E27
			public void AssignToCue(Cue iCue)
			{
				iCue.SetVariable(PushSpell.PUSH_VARIABLE, this.magnitude * 3f);
			}

			// Token: 0x040023B8 RID: 9144
			public float magnitude;
		}
	}
}
