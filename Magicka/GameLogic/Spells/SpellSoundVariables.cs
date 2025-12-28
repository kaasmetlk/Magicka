using System;
using Magicka.Audio;
using Microsoft.Xna.Framework.Audio;

namespace Magicka.GameLogic.Spells
{
	// Token: 0x02000630 RID: 1584
	public struct SpellSoundVariables : IAudioVariables
	{
		// Token: 0x06002FDC RID: 12252 RVA: 0x0018462F File Offset: 0x0018282F
		public void AssignToCue(Cue iCue)
		{
			iCue.SetVariable(SpellSoundVariables.VARIABLE_MAGNITUDE, this.mMagnitude);
		}

		// Token: 0x040033D6 RID: 13270
		public static readonly string VARIABLE_MAGNITUDE = "Magnitude";

		// Token: 0x040033D7 RID: 13271
		public float mMagnitude;
	}
}
