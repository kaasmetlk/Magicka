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
	// Token: 0x02000315 RID: 789
	public class Fear : SpecialAbility, ITargetAbility
	{
		// Token: 0x17000619 RID: 1561
		// (get) Token: 0x06001841 RID: 6209 RVA: 0x000A0884 File Offset: 0x0009EA84
		public static Fear Instance
		{
			get
			{
				if (Fear.mSingelton == null)
				{
					lock (Fear.mSingeltonLock)
					{
						if (Fear.mSingelton == null)
						{
							Fear.mSingelton = new Fear();
						}
					}
				}
				return Fear.mSingelton;
			}
		}

		// Token: 0x06001842 RID: 6210 RVA: 0x000A08D8 File Offset: 0x0009EAD8
		public Fear(Animations iAnimation) : base(iAnimation, "#magick_fear".GetHashCodeCustom())
		{
		}

		// Token: 0x06001843 RID: 6211 RVA: 0x000A08F6 File Offset: 0x0009EAF6
		private Fear() : base(Animations.cast_magick_self, "#magick_fear".GetHashCodeCustom())
		{
		}

		// Token: 0x06001844 RID: 6212 RVA: 0x000A0918 File Offset: 0x0009EB18
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			Vector3 right = Vector3.Right;
			Vector3 vector = iPosition;
			VisualEffectReference visualEffectReference;
			EffectManager.Instance.StartEffect(Fear.EFFECT, ref vector, ref right, out visualEffectReference);
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				List<Entity> entities = iPlayState.EntityManager.GetEntities(iPosition, 6f, true);
				for (int i = 0; i < entities.Count; i++)
				{
					Character character = entities[i] as Character;
					if (character != null && (!character.IsSelfShielded || character.IsSolidSelfShielded))
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
			AudioManager.Instance.PlayCue(Banks.Spells, Fear.SOUND, this.mAudioEmitter);
			return true;
		}

		// Token: 0x06001845 RID: 6213 RVA: 0x000A0A70 File Offset: 0x0009EC70
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			if (!(iOwner is Character))
			{
				return false;
			}
			Vector3 right = Vector3.Right;
			Vector3 position = iOwner.Position;
			VisualEffectReference visualEffectReference;
			EffectManager.Instance.StartEffect(Fear.EFFECT, ref position, ref right, out visualEffectReference);
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				List<Entity> entities = iPlayState.EntityManager.GetEntities(position, 6f, true);
				entities.Remove(iOwner as Entity);
				Character character = iOwner as Character;
				for (int i = 0; i < entities.Count; i++)
				{
					Character character2 = entities[i] as Character;
					if (character2 != null && (character2.Faction & character.Faction) == Factions.NONE && (!character2.IsSelfShielded || character2.IsSolidSelfShielded))
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
			AudioManager.Instance.PlayCue(Banks.Spells, Fear.SOUND, iOwner.AudioEmitter);
			return true;
		}

		// Token: 0x06001846 RID: 6214 RVA: 0x000A0BB4 File Offset: 0x0009EDB4
		public bool Execute(ISpellCaster iOwner, Entity iTarget, PlayState iPlayState)
		{
			if (!(iTarget is Character))
			{
				return false;
			}
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
					characterActionMessage.Handle = iTarget.Handle;
					characterActionMessage.TargetHandle = iOwner.Handle;
					characterActionMessage.Action = ActionType.Magick;
					characterActionMessage.Param3I = 6;
					NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
				}
				(iTarget as Character).Fear(iOwner as Character);
			}
			Vector3 right = Vector3.Right;
			Vector3 position = iOwner.Position;
			VisualEffectReference visualEffectReference;
			EffectManager.Instance.StartEffect(Fear.EFFECT, ref position, ref right, out visualEffectReference);
			AudioManager.Instance.PlayCue(Banks.Spells, Fear.SOUND, iOwner.AudioEmitter);
			return true;
		}

		// Token: 0x040019FE RID: 6654
		private const float RANGE = 6f;

		// Token: 0x040019FF RID: 6655
		public const float FEAR_TIME = 5f;

		// Token: 0x04001A00 RID: 6656
		private static Fear mSingelton;

		// Token: 0x04001A01 RID: 6657
		private static volatile object mSingeltonLock = new object();

		// Token: 0x04001A02 RID: 6658
		public static readonly int SOUND = "magick_fear".GetHashCodeCustom();

		// Token: 0x04001A03 RID: 6659
		public static readonly int EFFECT = "magick_fear".GetHashCodeCustom();

		// Token: 0x04001A04 RID: 6660
		public static readonly int FEARED_EFFECT = "magick_feared".GetHashCodeCustom();

		// Token: 0x04001A05 RID: 6661
		private AudioEmitter mAudioEmitter = new AudioEmitter();
	}
}
