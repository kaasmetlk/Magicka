using System;
using Magicka.GameLogic.GameStates;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000542 RID: 1346
	internal class Zoom : SpecialAbility
	{
		// Token: 0x060027FF RID: 10239 RVA: 0x00124502 File Offset: 0x00122702
		public Zoom(Animations iAnimation) : base(iAnimation, "#specab_tsal_magnify".GetHashCodeCustom())
		{
		}

		// Token: 0x06002800 RID: 10240 RVA: 0x00124515 File Offset: 0x00122715
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			iPlayState.Camera.SetPlayerMagnification(2f, 4f);
			return base.Execute(iOwner, iPlayState);
		}

		// Token: 0x06002801 RID: 10241 RVA: 0x00124534 File Offset: 0x00122734
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			iPlayState.Camera.SetPlayerMagnification(2f, 4f);
			return base.Execute(iPosition, iPlayState);
		}

		// Token: 0x04002B94 RID: 11156
		private const float TTL = 4f;

		// Token: 0x04002B95 RID: 11157
		private const float MAGNIFICATION = 2f;
	}
}
