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
	// Token: 0x020005B2 RID: 1458
	internal class TractorPull : SpecialAbility
	{
		// Token: 0x17000A37 RID: 2615
		// (get) Token: 0x06002B9F RID: 11167 RVA: 0x001587BC File Offset: 0x001569BC
		public static TractorPull Instance
		{
			get
			{
				if (TractorPull.sSingelton == null)
				{
					lock (TractorPull.sSingeltonLock)
					{
						if (TractorPull.sSingelton == null)
						{
							TractorPull.sSingelton = new TractorPull();
						}
					}
				}
				return TractorPull.sSingelton;
			}
		}

		// Token: 0x06002BA0 RID: 11168 RVA: 0x00158810 File Offset: 0x00156A10
		private TractorPull() : base(Animations.cast_area_ground, "#magick_tractorpull".GetHashCodeCustom())
		{
			TractorPull.sAudioEmitter = new AudioEmitter();
			TractorPull.sAudioEmitter.Forward = Vector3.Forward;
			TractorPull.sAudioEmitter.Up = Vector3.Up;
			TractorPull.sAudioEmitter.Position = Vector3.Zero;
		}

		// Token: 0x06002BA1 RID: 11169 RVA: 0x00158868 File Offset: 0x00156A68
		public TractorPull(Animations iAnimation) : base(iAnimation, "#magick_tractorpull".GetHashCodeCustom())
		{
			TractorPull.sAudioEmitter = new AudioEmitter();
			TractorPull.sAudioEmitter.Forward = Vector3.Forward;
			TractorPull.sAudioEmitter.Up = Vector3.Up;
			TractorPull.sAudioEmitter.Position = Vector3.Zero;
		}

		// Token: 0x06002BA2 RID: 11170 RVA: 0x001588C0 File Offset: 0x00156AC0
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
				IL_1C1:
				i++;
				continue;
				IL_5C:
				Vector3 position = entities[i].Position;
				Vector3 iDirection;
				Vector3.Subtract(ref iPosition, ref position, out iDirection);
				float num = iDirection.Length();
				iDirection.Y = 0f;
				if (num <= 1E-45f)
				{
					goto IL_1C1;
				}
				float num2 = 2.5f * (float)Math.Pow((double)(num / 15f), 0.5);
				float mass = entities[i].Body.Mass;
				float num3 = 0.17453292f;
				Damage iDamage = new Damage(AttackProperties.Pushed, Elements.Earth, mass, num2);
				if (entities[i] is IDamageable && !(entities[i] is MissileEntity))
				{
					Vector3.Negate(ref iDirection, out iDirection);
					Vector3.Add(ref position, ref iDirection, out position);
					(entities[i] as IDamageable).Damage(iDamage, this.mSpellCaster as Entity, this.mTimeStamp, position);
					goto IL_1C1;
				}
				if (entities[i] is Item)
				{
					if ((entities[i] as Item).IgnoreTractorPull)
					{
						goto IL_1C1;
					}
					if ((entities[i] as Item).Body.Immovable)
					{
						(entities[i] as Item).Body.Immovable = false;
						(entities[i] as Item).Body.SetActive();
					}
				}
				entities[i].AddImpulseVelocity(iDirection, num3 + num3 * 0.7853982f * 0.5f, mass, num2);
				goto IL_1C1;
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
				TractorPull.sAudioEmitter.Position = iPosition;
				audioEmitter = TractorPull.sAudioEmitter;
			}
			else
			{
				audioEmitter = this.mSpellCaster.AudioEmitter;
			}
			AudioManager.Instance.PlayCue<PushSpell.PushMagnitudeVolumAdjustAndOthers>(Banks.Spells, TractorPull.SOUND_EFFECT, iVariables, audioEmitter);
			return true;
		}

		// Token: 0x06002BA3 RID: 11171 RVA: 0x00158B34 File Offset: 0x00156D34
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			this.mSpellCaster = iOwner;
			bool result = this.Execute(iOwner.Position, iPlayState);
			this.mSpellCaster = null;
			return result;
		}

		// Token: 0x04002F53 RID: 12115
		private const float RADIUS = 15f;

		// Token: 0x04002F54 RID: 12116
		private static TractorPull sSingelton;

		// Token: 0x04002F55 RID: 12117
		private static volatile object sSingeltonLock = new object();

		// Token: 0x04002F56 RID: 12118
		private static readonly int SOUND_EFFECT = "spell_push_area".GetHashCodeCustom();

		// Token: 0x04002F57 RID: 12119
		private static readonly int MAGICK_EFFECT = "magick_tractorpull".GetHashCodeCustom();

		// Token: 0x04002F58 RID: 12120
		private ISpellCaster mSpellCaster;

		// Token: 0x04002F59 RID: 12121
		private static AudioEmitter sAudioEmitter;
	}
}
