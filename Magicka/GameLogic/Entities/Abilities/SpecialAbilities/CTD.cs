using System;
using System.Collections.Generic;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Localization;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x020004DA RID: 1242
	internal class CTD : SpecialAbility
	{
		// Token: 0x170008A2 RID: 2210
		// (get) Token: 0x060024EF RID: 9455 RVA: 0x0010A938 File Offset: 0x00108B38
		public static CTD Instance
		{
			get
			{
				if (CTD.sSingelton == null)
				{
					lock (CTD.sSingeltonLock)
					{
						if (CTD.sSingelton == null)
						{
							CTD.sSingelton = new CTD();
						}
					}
				}
				return CTD.sSingelton;
			}
		}

		// Token: 0x060024F0 RID: 9456 RVA: 0x0010A98C File Offset: 0x00108B8C
		private CTD() : base(Animations.cast_magick_sweep, "#magick_ctd".GetHashCodeCustom())
		{
		}

		// Token: 0x060024F1 RID: 9457 RVA: 0x0010A9AC File Offset: 0x00108BAC
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			NetworkState state = NetworkManager.Instance.State;
			if ((state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer))) || (state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer)))
			{
				Vector3 position = iOwner.Position;
				Vector3 direction = iOwner.Direction;
				new Vector2(direction.X, direction.Z);
				Character character = null;
				List<Entity> entities = iPlayState.EntityManager.GetEntities(position, 20f, true, true);
				for (int i = 0; i < entities.Count; i++)
				{
					if (!(entities[i] is Character))
					{
						entities.RemoveAt(i--);
					}
					else if (entities[i] is Character && ((entities[i] as Character).CannotDieWithoutExplicitKill || (entities[i] as Character).MaxHitPoints > 100000f))
					{
						entities.RemoveAt(i--);
					}
				}
				if (entities.Count > 0)
				{
					character = (entities[SpecialAbility.RANDOM.Next(entities.Count)] as Character);
				}
				iPlayState.EntityManager.ReturnEntityList(entities);
				if (character != null)
				{
					Matrix matrix;
					Matrix.CreateScale(character.Radius, character.Capsule.Length + character.Radius * 2f, character.Radius, out matrix);
					matrix.Translation = character.Position;
					VisualEffectReference visualEffectReference;
					EffectManager.Instance.StartEffect(CTD.EFFECT, ref matrix, out visualEffectReference);
					AudioManager.Instance.PlayCue(Banks.Additional, CTD.SOUND, character.AudioEmitter);
					if (state != NetworkState.Client)
					{
						character.Terminate(true, false);
					}
					if (state != NetworkState.Offline)
					{
						if (iPlayState.Level.CurrentScene.RuleSet is SurvivalRuleset && character is NonPlayerCharacter && character.DisplayName != 0)
						{
							NetworkChat.Instance.AddMessage(LanguageManager.Instance.GetString(CTD.MESSAGE).Replace("#1;", LanguageManager.Instance.GetString(character.DisplayName)));
						}
						CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
						characterActionMessage.Action = ActionType.Magick;
						characterActionMessage.Handle = iOwner.Handle;
						characterActionMessage.TargetHandle = character.Handle;
						characterActionMessage.Param3I = 23;
						NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
					}
					return true;
				}
				AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL, iOwner.AudioEmitter);
			}
			return false;
		}

		// Token: 0x04002847 RID: 10311
		private static CTD sSingelton;

		// Token: 0x04002848 RID: 10312
		private static volatile object sSingeltonLock = new object();

		// Token: 0x04002849 RID: 10313
		internal static readonly int EFFECT = "magick_ctd".GetHashCodeCustom();

		// Token: 0x0400284A RID: 10314
		internal static readonly int SOUND = "magick_ctd".GetHashCodeCustom();

		// Token: 0x0400284B RID: 10315
		internal static readonly int MESSAGE = "#add_connectiontolost".GetHashCodeCustom();

		// Token: 0x0400284C RID: 10316
		private AudioEmitter mAudioEmitter = new AudioEmitter();
	}
}
