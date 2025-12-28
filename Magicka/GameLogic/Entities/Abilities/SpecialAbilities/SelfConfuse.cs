using System;
using Magicka.GameLogic.GameStates;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000367 RID: 871
	internal class SelfConfuse : SpecialAbility
	{
		// Token: 0x06001A93 RID: 6803 RVA: 0x000B4F52 File Offset: 0x000B3152
		public SelfConfuse(Animations iAnimation) : base(Animations.cast_magick_direct, SelfConfuse.DISPLAY_NAME)
		{
		}

		// Token: 0x06001A94 RID: 6804 RVA: 0x000B4F61 File Offset: 0x000B3161
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new Exception("SelfConfuse have to be cast by a character!");
		}

		// Token: 0x06001A95 RID: 6805 RVA: 0x000B4F70 File Offset: 0x000B3170
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			Confuse instance = Confuse.GetInstance();
			return instance.Execute(iOwner, iOwner as Entity, iPlayState);
		}

		// Token: 0x04001CE6 RID: 7398
		private static readonly int DISPLAY_NAME = "#magick_confuse".GetHashCodeCustom();
	}
}
