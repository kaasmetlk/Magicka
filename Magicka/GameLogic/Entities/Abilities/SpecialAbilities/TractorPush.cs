using System;
using System.Collections.Generic;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.Graphics.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x020003CE RID: 974
	internal class TractorPush : SpecialAbility
	{
		// Token: 0x17000757 RID: 1879
		// (get) Token: 0x06001DD2 RID: 7634 RVA: 0x000D2134 File Offset: 0x000D0334
		public static TractorPush Instance
		{
			get
			{
				if (TractorPush.sSingelton == null)
				{
					lock (TractorPush.sSingeltonLock)
					{
						if (TractorPush.sSingelton == null)
						{
							TractorPush.sSingelton = new TractorPush();
						}
					}
				}
				return TractorPush.sSingelton;
			}
		}

		// Token: 0x06001DD3 RID: 7635 RVA: 0x000D2188 File Offset: 0x000D0388
		private TractorPush() : base(Animations.cast_area_ground, "#item_musician_force_push".GetHashCodeCustom())
		{
			TractorPush.sAudioEmitter = new AudioEmitter();
			TractorPush.sAudioEmitter.Forward = Vector3.Forward;
			TractorPush.sAudioEmitter.Up = Vector3.Up;
			TractorPush.sAudioEmitter.Position = Vector3.Zero;
		}

		// Token: 0x06001DD4 RID: 7636 RVA: 0x000D21E0 File Offset: 0x000D03E0
		public TractorPush(Animations iAnimation) : base(iAnimation, "#item_musician_force_push".GetHashCodeCustom())
		{
			TractorPush.sAudioEmitter = new AudioEmitter();
			TractorPush.sAudioEmitter.Forward = Vector3.Forward;
			TractorPush.sAudioEmitter.Up = Vector3.Up;
			TractorPush.sAudioEmitter.Position = Vector3.Zero;
		}

		// Token: 0x06001DD5 RID: 7637 RVA: 0x000D2238 File Offset: 0x000D0438
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			List<Entity> entities = iPlayState.EntityManager.GetEntities(iPosition, 15f, true);
			int i = 0;
			while (i < entities.Count)
			{
				Character character = entities[i] as Character;
				if (character == null)
				{
					goto IL_5C;
				}
				if (character != this.mSpellCaster && !character.IsEthereal)
				{
					if (character.IsGripped && character.Gripper != null)
					{
						character.Gripper.ReleaseAttachedCharacter();
						goto IL_5C;
					}
					goto IL_5C;
				}
				IL_1CF:
				i++;
				continue;
				IL_5C:
				Vector3 position = entities[i].Position;
				Vector3 vector;
				Vector3.Subtract(ref iPosition, ref position, out vector);
				float num = vector.Length();
				vector.Y = 0f;
				vector *= -1f;
				if (num <= 1E-45f)
				{
					goto IL_1CF;
				}
				float num2 = 1.5f * (float)Math.Pow((double)(15f / num), 0.5);
				float mass = entities[i].Body.Mass;
				float num3 = 0.17453292f;
				Damage iDamage = new Damage(AttackProperties.Pushed, Elements.Earth, mass, num2);
				if (entities[i] is IDamageable && !(entities[i] is MissileEntity))
				{
					Vector3.Negate(ref vector, out vector);
					Vector3.Add(ref position, ref vector, out position);
					(entities[i] as IDamageable).Damage(iDamage, this.mSpellCaster as Entity, this.mTimeStamp, position);
					goto IL_1CF;
				}
				if (entities[i] is Item)
				{
					if ((entities[i] as Item).IgnoreTractorPull)
					{
						goto IL_1CF;
					}
					if ((entities[i] as Item).Body.Immovable)
					{
						(entities[i] as Item).Body.Immovable = false;
						(entities[i] as Item).Body.SetActive();
					}
				}
				entities[i].AddImpulseVelocity(vector, num3 + num3 * 0.7853982f * 0.5f, mass, num2);
				goto IL_1CF;
			}
			iPlayState.EntityManager.ReturnEntityList(entities);
			iPlayState.Camera.CameraShake(1f, 0.5f);
			RadialBlur radialBlur = RadialBlur.GetRadialBlur();
			radialBlur.Initialize(ref iPosition, 15f, 1f, iPlayState.Scene);
			PushSpell.PushMagnitudeVolumAdjustAndOthers iVariables = default(PushSpell.PushMagnitudeVolumAdjustAndOthers);
			iVariables.magnitude = 1f;
			AudioEmitter audioEmitter;
			if (this.mSpellCaster == null)
			{
				TractorPush.sAudioEmitter.Position = iPosition;
				audioEmitter = TractorPush.sAudioEmitter;
			}
			else
			{
				audioEmitter = this.mSpellCaster.AudioEmitter;
			}
			AudioManager.Instance.PlayCue<PushSpell.PushMagnitudeVolumAdjustAndOthers>(Banks.Spells, TractorPush.SOUND_EFFECT, iVariables, audioEmitter);
			return true;
		}

		// Token: 0x06001DD6 RID: 7638 RVA: 0x000D24BC File Offset: 0x000D06BC
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			this.mSpellCaster = iOwner;
			bool result = this.Execute(iOwner.Position, iPlayState);
			this.mSpellCaster = null;
			return result;
		}

		// Token: 0x04002055 RID: 8277
		private const float RADIUS = 15f;

		// Token: 0x04002056 RID: 8278
		private static TractorPush sSingelton;

		// Token: 0x04002057 RID: 8279
		private static volatile object sSingeltonLock = new object();

		// Token: 0x04002058 RID: 8280
		private static readonly int SOUND_EFFECT = "spell_push_area".GetHashCodeCustom();

		// Token: 0x04002059 RID: 8281
		private static readonly int MAGICK_EFFECT = "magick_tractorpull".GetHashCodeCustom();

		// Token: 0x0400205A RID: 8282
		private ISpellCaster mSpellCaster;

		// Token: 0x0400205B RID: 8283
		private static AudioEmitter sAudioEmitter;
	}
}
