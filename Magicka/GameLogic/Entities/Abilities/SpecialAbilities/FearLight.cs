using System;
using System.Collections.Generic;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000131 RID: 305
	public class FearLight : SpecialAbility
	{
		// Token: 0x06000887 RID: 2183 RVA: 0x00037297 File Offset: 0x00035497
		public FearLight(Animations iAnimation) : base(iAnimation, "#specab_fear".GetHashCodeCustom())
		{
		}

		// Token: 0x06000888 RID: 2184 RVA: 0x000372B8 File Offset: 0x000354B8
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			Vector3 right = Vector3.Right;
			Vector3 vector = iPosition;
			VisualEffectReference visualEffectReference;
			EffectManager.Instance.StartEffect(FearLight.EFFECT, ref vector, ref right, out visualEffectReference);
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				List<Entity> entities = iPlayState.EntityManager.GetEntities(iPosition, 10f, false);
				for (int i = 0; i < entities.Count; i++)
				{
					Character character = entities[i] as Character;
					if (character != null)
					{
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
							characterActionMessage.Handle = character.Handle;
							characterActionMessage.TargetHandle = 0;
							characterActionMessage.Param0F = iPosition.X;
							characterActionMessage.Param1F = iPosition.Y;
							characterActionMessage.Param2F = iPosition.Z;
							characterActionMessage.Action = ActionType.Magick;
							characterActionMessage.Param3I = 6;
							NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
						}
						character.Fear(iPosition);
					}
				}
			}
			this.mAudioEmitter.Position = iPosition;
			this.mAudioEmitter.Up = Vector3.Up;
			this.mAudioEmitter.Forward = Vector3.Forward;
			AudioManager.Instance.PlayCue(Banks.Spells, FearLight.SOUND, this.mAudioEmitter);
			return true;
		}

		// Token: 0x06000889 RID: 2185 RVA: 0x000373F8 File Offset: 0x000355F8
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			if (!(iOwner is Character))
			{
				return false;
			}
			Vector3 right = Vector3.Right;
			Vector3 position = iOwner.Position;
			VisualEffectReference visualEffectReference;
			EffectManager.Instance.StartEffect(FearLight.EFFECT, ref position, ref right, out visualEffectReference);
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				List<Entity> entities = iPlayState.EntityManager.GetEntities(position, 10f, false);
				entities.Remove(iOwner as Entity);
				Character character = iOwner as Character;
				for (int i = 0; i < entities.Count; i++)
				{
					Character character2 = entities[i] as Character;
					if (character2 != null && (character2.Faction & character.Faction) == Factions.NONE)
					{
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
							characterActionMessage.Handle = character2.Handle;
							characterActionMessage.TargetHandle = character.Handle;
							characterActionMessage.Action = ActionType.Magick;
							characterActionMessage.Param3I = 6;
							NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
						}
						character2.Fear(character);
					}
				}
				iPlayState.EntityManager.ReturnEntityList(entities);
			}
			AudioManager.Instance.PlayCue(Banks.Spells, FearLight.SOUND, iOwner.AudioEmitter);
			return true;
		}

		// Token: 0x0400080B RID: 2059
		private const float RANGE = 10f;

		// Token: 0x0400080C RID: 2060
		public const float FEAR_TIME = 6f;

		// Token: 0x0400080D RID: 2061
		public static readonly int SOUND = "spell_arcane_ray_stage4".GetHashCodeCustom();

		// Token: 0x0400080E RID: 2062
		public static readonly int EFFECT = "gandalf_fear".GetHashCodeCustom();

		// Token: 0x0400080F RID: 2063
		public static readonly int FEARED_EFFECT = "magick_feared".GetHashCodeCustom();

		// Token: 0x04000810 RID: 2064
		private AudioEmitter mAudioEmitter = new AudioEmitter();
	}
}
