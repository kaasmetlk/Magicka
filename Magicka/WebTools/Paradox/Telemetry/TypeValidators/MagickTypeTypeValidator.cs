using System;
using Magicka.GameLogic.Spells;

namespace Magicka.WebTools.Paradox.Telemetry.TypeValidators
{
	// Token: 0x020002BE RID: 702
	public class MagickTypeTypeValidator : BaseTypeValidator<MagickType>
	{
		// Token: 0x06001528 RID: 5416 RVA: 0x00086614 File Offset: 0x00084814
		protected override string ToString(MagickType iValue)
		{
			string empty = string.Empty;
			switch (iValue)
			{
			case MagickType.ThunderB:
				return "thunder_bolt";
			case MagickType.Rain:
			case MagickType.Tornado:
			case MagickType.Blizzard:
			case MagickType.Conflagration:
			case MagickType.TimeWarp:
			case MagickType.Vortex:
				break;
			case MagickType.MeteorS:
				return "meteor_shower";
			case MagickType.ThunderS:
				return "thunder_storm";
			case MagickType.SUndead:
				return "summon_undead";
			case MagickType.SElemental:
				return "summon_elemental";
			case MagickType.SDeath:
				return "summon_death";
			case MagickType.SPhoenix:
				return "summon_phoenix";
			default:
				if (iValue == MagickType.CTD)
				{
					return "crash_to_desktop";
				}
				break;
			}
			return base.ToString(iValue);
		}

		// Token: 0x040016CD RID: 5837
		private const string THUNDER_BOLT = "thunder_bolt";

		// Token: 0x040016CE RID: 5838
		private const string METEOR_SHOWER = "meteor_shower";

		// Token: 0x040016CF RID: 5839
		private const string THUNDER_STORM = "thunder_storm";

		// Token: 0x040016D0 RID: 5840
		private const string SUMMON_UNDEAD = "summon_undead";

		// Token: 0x040016D1 RID: 5841
		private const string SUMMON_ELEMENTAL = "summon_elemental";

		// Token: 0x040016D2 RID: 5842
		private const string SUMMON_DEATH = "summon_death";

		// Token: 0x040016D3 RID: 5843
		private const string SUMMON_PHOENIX = "summon_phoenix";

		// Token: 0x040016D4 RID: 5844
		private const string CRASH_TO_DESKTOP = "crash_to_desktop";
	}
}
