using System;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Network;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x020002AD RID: 685
	internal class Etherealize : SpecialAbility
	{
		// Token: 0x1700054F RID: 1359
		// (get) Token: 0x060014AF RID: 5295 RVA: 0x00080C78 File Offset: 0x0007EE78
		public static Etherealize Instance
		{
			get
			{
				if (Etherealize.sSingelton == null)
				{
					lock (Etherealize.sSingeltonLock)
					{
						if (Etherealize.sSingelton == null)
						{
							Etherealize.sSingelton = new Etherealize();
						}
					}
				}
				return Etherealize.sSingelton;
			}
		}

		// Token: 0x060014B0 RID: 5296 RVA: 0x00080CCC File Offset: 0x0007EECC
		private Etherealize() : base(Animations.cast_magick_self, "#magick_etherealize".GetHashCodeCustom())
		{
		}

		// Token: 0x060014B1 RID: 5297 RVA: 0x00080CE0 File Offset: 0x0007EEE0
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			Character character = iOwner as Character;
			if (character != null)
			{
				NetworkState state = NetworkManager.Instance.State;
				if ((state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer))) || (state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer)))
				{
					character.Ethereal(true, 1f, 1f);
					Vector3 position = iOwner.Position;
					Vector3 direction = iOwner.Direction;
					VisualEffectReference visualEffectReference;
					EffectManager.Instance.StartEffect(Etherealize.MAGICK_EFFECT, ref position, ref direction, out visualEffectReference);
					AudioManager.Instance.PlayCue(Banks.Characters, Etherealize.SOUND_EFFECT, iOwner.AudioEmitter);
					if (state != NetworkState.Offline)
					{
						CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
						characterActionMessage.Action = ActionType.Magick;
						characterActionMessage.Handle = iOwner.Handle;
						characterActionMessage.Param3I = 38;
						NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
					}
				}
				return true;
			}
			AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL, iOwner.AudioEmitter);
			return false;
		}

		// Token: 0x060014B2 RID: 5298 RVA: 0x00080DF7 File Offset: 0x0007EFF7
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new Exception("Etherealize can only be called by a character!");
		}

		// Token: 0x0400160C RID: 5644
		private static Etherealize sSingelton;

		// Token: 0x0400160D RID: 5645
		private static volatile object sSingeltonLock = new object();

		// Token: 0x0400160E RID: 5646
		internal static readonly int SOUND_EFFECT = "chr_daemon_etherealize3".GetHashCodeCustom();

		// Token: 0x0400160F RID: 5647
		internal static readonly int MAGICK_EFFECT = "magick_etherealize".GetHashCodeCustom();
	}
}
