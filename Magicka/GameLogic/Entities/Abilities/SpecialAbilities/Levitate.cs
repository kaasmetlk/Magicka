using System;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.Gamers;
using Magicka.Network;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000545 RID: 1349
	internal class Levitate : SpecialAbility
	{
		// Token: 0x1700095B RID: 2395
		// (get) Token: 0x06002815 RID: 10261 RVA: 0x00125CAC File Offset: 0x00123EAC
		public static Levitate Instance
		{
			get
			{
				if (Levitate.sSingelton == null)
				{
					lock (Levitate.sSingeltonLock)
					{
						if (Levitate.sSingelton == null)
						{
							Levitate.sSingelton = new Levitate();
						}
					}
				}
				return Levitate.sSingelton;
			}
		}

		// Token: 0x06002816 RID: 10262 RVA: 0x00125D00 File Offset: 0x00123F00
		private Levitate() : base(Animations.cast_magick_self, "#magick_levitate".GetHashCodeCustom())
		{
		}

		// Token: 0x06002817 RID: 10263 RVA: 0x00125D14 File Offset: 0x00123F14
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			NetworkState state = NetworkManager.Instance.State;
			if ((state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer))) || (state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer)))
			{
				Character character = iOwner as Character;
				if (character == null)
				{
					return false;
				}
				Levitate.CastLevitate(character);
				if (state != NetworkState.Offline)
				{
					CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
					characterActionMessage.Action = ActionType.Magick;
					characterActionMessage.Handle = iOwner.Handle;
					characterActionMessage.Param3I = 28;
					NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
				}
			}
			return true;
		}

		// Token: 0x06002818 RID: 10264 RVA: 0x00125DC9 File Offset: 0x00123FC9
		internal static void CastLevitate(Character iOwner)
		{
			AudioManager.Instance.PlayCue(Banks.Spells, Levitate.MAGICK_SOUND, iOwner.AudioEmitter);
			iOwner.SetLevitate(30f, Levitate.MAGICK_EFFECT);
		}

		// Token: 0x04002B9F RID: 11167
		private static Levitate sSingelton;

		// Token: 0x04002BA0 RID: 11168
		private static volatile object sSingeltonLock = new object();

		// Token: 0x04002BA1 RID: 11169
		public static readonly int MAGICK_EFFECT = "magick_levitate".GetHashCodeCustom();

		// Token: 0x04002BA2 RID: 11170
		private static readonly int MAGICK_SOUND = Haste.SOUNDHASH;
	}
}
