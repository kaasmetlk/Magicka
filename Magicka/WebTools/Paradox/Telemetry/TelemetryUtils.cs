using System;
using Magicka.GameLogic;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.UI;
using Magicka.Levels.Versus;
using Magicka.Misc;
using Magicka.Network;

namespace Magicka.WebTools.Paradox.Telemetry
{
	// Token: 0x020005C6 RID: 1478
	public static class TelemetryUtils
	{
		// Token: 0x06002C40 RID: 11328 RVA: 0x0015C244 File Offset: 0x0015A444
		public static object[] GetUnhandledExceptionParameters(string iVariant, string iErrorMessage)
		{
			return new object[]
			{
				iVariant,
				iErrorMessage,
				HardwareInfoManager.OSVersion,
				HardwareInfoManager.SystemMemory,
				HardwareInfoManager.GfxDevice,
				HardwareInfoManager.GfxMem,
				HardwareInfoManager.GfxDriver,
				HardwareInfoManager.CPUType,
				HardwareInfoManager.LogicalProcessors
			};
		}

		// Token: 0x06002C41 RID: 11329 RVA: 0x0015C2AC File Offset: 0x0015A4AC
		public static void SendHardwareReport()
		{
			Singleton<ParadoxServices>.Instance.TelemetryEvent("hardware_report", new object[]
			{
				Singleton<GameSparksAccount>.Instance.Variant,
				Singleton<PlayerSegmentManager>.Instance.CurrentSegment.ToSegmentString(),
				HardwareInfoManager.OSVersion,
				HardwareInfoManager.SystemMemory,
				HardwareInfoManager.GfxDevice,
				HardwareInfoManager.GfxMem,
				HardwareInfoManager.GfxDriver,
				HardwareInfoManager.CPUType,
				HardwareInfoManager.LogicalProcessors
			});
		}

		// Token: 0x06002C42 RID: 11330 RVA: 0x0015C338 File Offset: 0x0015A538
		public static void SendDLCPromotionClicked()
		{
			Singleton<ParadoxServices>.Instance.TelemetryEvent("dlc_ad_clicked", new object[]
			{
				Singleton<GameSparksAccount>.Instance.Variant,
				Singleton<PlayerSegmentManager>.Instance.CurrentSegment.ToSegmentString(),
				DLC_StatusHelper.Instance.CurrentPromotion_Name,
				DLC_StatusHelper.Instance.CurrentPromotion_Name,
				DLC_StatusHelper.Instance.CurrentPromotion_AppID
			});
		}

		// Token: 0x06002C43 RID: 11331 RVA: 0x0015C3A8 File Offset: 0x0015A5A8
		public static EventData GetGameplayStartedData(string iGameType, string iLevelName, int iPlayerCount)
		{
			return new EventData("gameplay_started", new object[]
			{
				Singleton<GameSparksAccount>.Instance.Variant,
				Singleton<PlayerSegmentManager>.Instance.CurrentSegment.ToSegmentString(),
				iGameType,
				iLevelName,
				iPlayerCount,
				NetworkManager.Instance.State
			});
		}

		// Token: 0x06002C44 RID: 11332 RVA: 0x0015C40C File Offset: 0x0015A60C
		public static EventData GetTutorialActionData(EventEnums.TutorialAction iAction, string iTutorialName, string iTutorialStep, int iTimeSpent)
		{
			return new EventData("tutorial_action", new object[]
			{
				Singleton<GameSparksAccount>.Instance.Variant,
				Singleton<PlayerSegmentManager>.Instance.CurrentSegment.ToSegmentString(),
				iAction,
				iTutorialName,
				iTutorialStep,
				iTimeSpent
			});
		}

		// Token: 0x06002C45 RID: 11333 RVA: 0x0015C464 File Offset: 0x0015A664
		public static void SendTutorialAction(EventEnums.TutorialAction iAction, string iTutorialName, string iTutorialStep, int iTimeSpent)
		{
			Singleton<ParadoxServices>.Instance.TelemetryEvent("tutorial_action", new object[]
			{
				Singleton<GameSparksAccount>.Instance.Variant,
				Singleton<PlayerSegmentManager>.Instance.CurrentSegment.ToSegmentString(),
				iAction,
				iTutorialName,
				iTutorialStep,
				iTimeSpent
			});
		}

		// Token: 0x06002C46 RID: 11334 RVA: 0x0015C4C4 File Offset: 0x0015A6C4
		public static void SendCollectSpellbook(MagickType iMagickType)
		{
			Singleton<ParadoxServices>.Instance.TelemetryEvent("collect_spellbook", new object[]
			{
				Singleton<GameSparksAccount>.Instance.Variant,
				Singleton<PlayerSegmentManager>.Instance.CurrentSegment.ToSegmentString(),
				iMagickType
			});
		}

		// Token: 0x06002C47 RID: 11335 RVA: 0x0015C510 File Offset: 0x0015A710
		public static void SendInGameMenuButtonPressTelemetry(string iGameTypeString, string iLevelNameString, string iButtonName)
		{
			Singleton<ParadoxServices>.Instance.TelemetryEvent("ingame_menu_clicked", new object[]
			{
				Singleton<GameSparksAccount>.Instance.Variant,
				Singleton<PlayerSegmentManager>.Instance.CurrentSegment.ToSegmentString(),
				iGameTypeString,
				iLevelNameString,
				Game.Instance.PlayerCount,
				NetworkManager.Instance.State,
				iButtonName
			});
		}

		// Token: 0x06002C48 RID: 11336 RVA: 0x0015C584 File Offset: 0x0015A784
		public static EventData GetControllerChangedData(Player[] iPlayers)
		{
			EventEnums.ControllerType[] array = new EventEnums.ControllerType[4];
			for (int i = 0; i < iPlayers.Length; i++)
			{
				Player player = iPlayers[i];
				if (player == null || player.Controller == null)
				{
					array[i] = EventEnums.ControllerType.NotApplicable;
				}
				else if (player.Controller is XInputController || player.Controller is DirectInputController)
				{
					array[i] = EventEnums.ControllerType.Gamepad;
				}
				else if (player.Controller is KeyboardMouseController)
				{
					array[i] = EventEnums.ControllerType.Keyboard;
				}
				else
				{
					array[i] = EventEnums.ControllerType.NotApplicable;
				}
			}
			return new EventData("controller_setup", new object[]
			{
				Singleton<GameSparksAccount>.Instance.Variant,
				Singleton<PlayerSegmentManager>.Instance.CurrentSegment.ToSegmentString(),
				array[0],
				array[1],
				array[2],
				array[3]
			});
		}

		// Token: 0x06002C49 RID: 11337 RVA: 0x0015C650 File Offset: 0x0015A850
		public static void SendSpellCast(Character iCharacter)
		{
			Spell spell = iCharacter.Spell;
			string text = string.Empty;
			text += TelemetryUtils.GetSpellCombo("earth", (int)spell.EarthMagnitude);
			text += TelemetryUtils.GetSpellCombo("water", (int)spell.WaterMagnitude);
			text += TelemetryUtils.GetSpellCombo("cold", (int)spell.ColdMagnitude);
			text += TelemetryUtils.GetSpellCombo("fire", (int)spell.FireMagnitude);
			text += TelemetryUtils.GetSpellCombo("lightning", (int)spell.LightningMagnitude);
			text += TelemetryUtils.GetSpellCombo("arcane", (int)spell.ArcaneMagnitude);
			text += TelemetryUtils.GetSpellCombo("life", (int)spell.LifeMagnitude);
			text += TelemetryUtils.GetSpellCombo("shield", (int)spell.ShieldMagnitude);
			text += TelemetryUtils.GetSpellCombo("ice", (int)spell.IceMagnitude);
			text += TelemetryUtils.GetSpellCombo("steam", (int)spell.SteamMagnitude);
			text += TelemetryUtils.GetSpellCombo("poison", (int)spell.PoisonMagnitude);
			if (text.EndsWith(", "))
			{
				text = text.Substring(0, text.Length - 2);
			}
			Singleton<ParadoxServices>.Instance.TelemetryEvent("spell_cast", new object[]
			{
				Singleton<GameSparksAccount>.Instance.Variant,
				Singleton<PlayerSegmentManager>.Instance.CurrentSegment.ToSegmentString(),
				iCharacter.PlayState.GameType.ToString(),
				iCharacter.PlayState.Level.Name,
				text
			});
		}

		// Token: 0x06002C4A RID: 11338 RVA: 0x0015C800 File Offset: 0x0015AA00
		private static string GetSpellCombo(string iElement, int iCount)
		{
			string text = string.Empty;
			if (iCount != 0)
			{
				for (int i = 0; i < iCount; i++)
				{
					text += iElement;
					text += ", ";
				}
			}
			return text;
		}

		// Token: 0x06002C4B RID: 11339 RVA: 0x0015C838 File Offset: 0x0015AA38
		public static void SendPlayerDeath(Character iCharacter)
		{
			string text = string.Empty;
			Avatar avatar = iCharacter.LastAttacker as Avatar;
			bool flag = avatar != null && avatar.Player != null;
			bool flag2 = iCharacter.LastAttacker is NonPlayerCharacter;
			bool flag3 = iCharacter.PlayState.Level.CurrentScene.RuleSet is VersusRuleset;
			EventEnums.DeathCategory deathCategory;
			if (iCharacter == iCharacter.LastAttacker)
			{
				deathCategory = EventEnums.DeathCategory.Suicide;
				if ((iCharacter.LastDamageElement & Elements.Lightning) == Elements.Lightning)
				{
					text = (iCharacter.HasStatus(StatusEffects.Wet) ? "electic_on_self_wet" : "electric_on_self");
				}
				else if (iCharacter.HasStatus(StatusEffects.Burning) && (iCharacter.LastDamageElement & Elements.Fire) == Elements.Fire)
				{
					text = "fire_on_self";
				}
				else if ((iCharacter.LastDamageElement & Elements.Earth) == Elements.Earth)
				{
					text = "rock_on_self";
				}
				else if ((iCharacter.LastDamageElement & Elements.Arcane) == Elements.Arcane)
				{
					text = "arcane_on_self";
				}
			}
			else if (flag)
			{
				deathCategory = (flag3 ? EventEnums.DeathCategory.PlayerVsPlayer : EventEnums.DeathCategory.FriendlyFire);
				text = "killed_by_player";
			}
			else
			{
				deathCategory = (flag2 ? EventEnums.DeathCategory.NPC : EventEnums.DeathCategory.Environment);
				if (iCharacter.Drowning)
				{
					text = "drowned";
				}
				else if (iCharacter.LastAttacker != null)
				{
					if (iCharacter.LastAttacker is BossDamageZone)
					{
						BossDamageZone bossDamageZone = iCharacter.LastAttacker as BossDamageZone;
						text = bossDamageZone.Owner.GetBossType().ToString();
					}
					else
					{
						text = (iCharacter.LastAttacker as NonPlayerCharacter).Name;
					}
				}
			}
			Singleton<ParadoxServices>.Instance.TelemetryEvent("player_death", new object[]
			{
				Singleton<GameSparksAccount>.Instance.Variant,
				Singleton<PlayerSegmentManager>.Instance.CurrentSegment.ToSegmentString(),
				iCharacter.PlayState.GameType.ToString(),
				iCharacter.PlayState.Level.Name,
				deathCategory,
				text
			});
		}

		// Token: 0x04002FE5 RID: 12261
		private const string SPELL_SEPERATOR = ", ";

		// Token: 0x04002FE6 RID: 12262
		private const string KILLED_BY_PLAYER = "killed_by_player";

		// Token: 0x04002FE7 RID: 12263
		private const string DROWNED = "drowned";

		// Token: 0x04002FE8 RID: 12264
		private const string ELECTROCUTED = "electric_on_self";

		// Token: 0x04002FE9 RID: 12265
		private const string ELECTROCUTED_WET = "electic_on_self_wet";

		// Token: 0x04002FEA RID: 12266
		private const string BURNT = "fire_on_self";

		// Token: 0x04002FEB RID: 12267
		private const string METEOR = "rock_on_self";

		// Token: 0x04002FEC RID: 12268
		private const string ARCANE = "arcane_on_self";
	}
}
