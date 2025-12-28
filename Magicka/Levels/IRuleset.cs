using System;
using Magicka.Network;
using PolygonHead;

namespace Magicka.Levels
{
	// Token: 0x020000AE RID: 174
	public interface IRuleset
	{
		// Token: 0x06000512 RID: 1298
		int GetAnyArea();

		// Token: 0x06000513 RID: 1299
		void Update(float iDeltaTime, DataChannel iDataChannel);

		// Token: 0x06000514 RID: 1300
		void LocalUpdate(float iDeltaTime, DataChannel iDataChannel);

		// Token: 0x06000515 RID: 1301
		void NetworkUpdate(ref RulesetMessage iMsg);

		// Token: 0x06000516 RID: 1302
		void Initialize();

		// Token: 0x06000517 RID: 1303
		void DeInitialize();

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x06000518 RID: 1304
		Rulesets RulesetType { get; }
	}
}
