using System;
using System.Collections.Generic;
using Magicka.AI.AgentStates;
using Magicka.GameLogic.Entities;
using Magicka.PathFinding;
using Microsoft.Xna.Framework;

namespace Magicka.AI
{
	// Token: 0x020005C0 RID: 1472
	public interface IAI
	{
		// Token: 0x17000A40 RID: 2624
		// (get) Token: 0x06002BD8 RID: 11224
		Character Owner { get; }

		// Token: 0x17000A41 RID: 2625
		// (get) Token: 0x06002BD9 RID: 11225
		AIEvent[] Events { get; }

		// Token: 0x17000A42 RID: 2626
		// (get) Token: 0x06002BDA RID: 11226
		// (set) Token: 0x06002BDB RID: 11227
		int CurrentEvent { get; set; }

		// Token: 0x17000A43 RID: 2627
		// (get) Token: 0x06002BDC RID: 11228
		bool LoopEvents { get; }

		// Token: 0x17000A44 RID: 2628
		// (get) Token: 0x06002BDD RID: 11229
		// (set) Token: 0x06002BDE RID: 11230
		Vector3 WayPoint { get; set; }

		// Token: 0x17000A45 RID: 2629
		// (get) Token: 0x06002BDF RID: 11231
		// (set) Token: 0x06002BE0 RID: 11232
		float CurrentEventDelay { get; set; }

		// Token: 0x17000A46 RID: 2630
		// (get) Token: 0x06002BE1 RID: 11233
		List<PathNode> Path { get; }

		// Token: 0x17000A47 RID: 2631
		// (get) Token: 0x06002BE2 RID: 11234
		float CurrentStateAge { get; }

		// Token: 0x17000A48 RID: 2632
		// (get) Token: 0x06002BE3 RID: 11235
		// (set) Token: 0x06002BE4 RID: 11236
		float WanderAngle { get; set; }

		// Token: 0x17000A49 RID: 2633
		// (get) Token: 0x06002BE5 RID: 11237
		MovementProperties MoveAbilities { get; }

		// Token: 0x17000A4A RID: 2634
		// (get) Token: 0x06002BE6 RID: 11238
		Dictionary<byte, Animations[]> MoveAnimations { get; }

		// Token: 0x06002BE7 RID: 11239
		void PushState(IAIState iAIState);

		// Token: 0x06002BE8 RID: 11240
		void PopState();
	}
}
