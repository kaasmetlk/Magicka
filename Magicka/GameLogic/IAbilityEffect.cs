using System;
using PolygonHead;

namespace Magicka.GameLogic
{
	// Token: 0x0200003B RID: 59
	public interface IAbilityEffect
	{
		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000264 RID: 612
		bool IsDead { get; }

		// Token: 0x06000265 RID: 613
		void Update(DataChannel iDataChannel, float iDeltaTime);

		// Token: 0x06000266 RID: 614
		void OnRemove();
	}
}
