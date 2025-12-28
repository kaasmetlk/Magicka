using System;
using System.Collections.Generic;
using Magicka.AI.Messaging;
using Magicka.Audio;
using Magicka.Network;

namespace Magicka.AI
{
	// Token: 0x0200027A RID: 634
	public class AIManager
	{
		// Token: 0x170004C1 RID: 1217
		// (get) Token: 0x060012C3 RID: 4803 RVA: 0x000749F0 File Offset: 0x00072BF0
		public static AIManager Instance
		{
			get
			{
				if (AIManager.mSingelton == null)
				{
					lock (AIManager.mSingeltonLock)
					{
						if (AIManager.mSingelton == null)
						{
							AIManager.mSingelton = new AIManager();
						}
					}
				}
				return AIManager.mSingelton;
			}
		}

		// Token: 0x060012C4 RID: 4804 RVA: 0x00074A44 File Offset: 0x00072C44
		private AIManager()
		{
		}

		// Token: 0x060012C5 RID: 4805 RVA: 0x00074A5C File Offset: 0x00072C5C
		public void Clear()
		{
			this.mAgents.Clear();
		}

		// Token: 0x060012C6 RID: 4806 RVA: 0x00074A6C File Offset: 0x00072C6C
		public void Update(float iDeltaTime)
		{
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				return;
			}
			MessageDispatcher.Instance.DischargeDelayedMessages(iDeltaTime);
			bool flag = false;
			if (this.mAgents.Count > 0)
			{
				this.mUpdateIndex = (this.mUpdateIndex + 20) % this.mAgents.Count;
				int num = Math.Min(20, this.mAgents.Count);
				for (int i = 0; i < num; i++)
				{
					int index = (i + this.mUpdateIndex) % this.mAgents.Count;
					this.mAgents[index].Update();
				}
				for (int j = 0; j < this.Agents.Count; j++)
				{
					Agent agent = this.mAgents[j];
					agent.UpdateTime(iDeltaTime);
					flag |= (agent.CurrentTarget != null);
				}
			}
			AudioManager.Instance.Threat = flag;
		}

		// Token: 0x170004C2 RID: 1218
		// (get) Token: 0x060012C7 RID: 4807 RVA: 0x00074B51 File Offset: 0x00072D51
		public List<Agent> Agents
		{
			get
			{
				return this.mAgents;
			}
		}

		// Token: 0x040014A4 RID: 5284
		private const int STEPS = 20;

		// Token: 0x040014A5 RID: 5285
		private static AIManager mSingelton;

		// Token: 0x040014A6 RID: 5286
		private static volatile object mSingeltonLock = new object();

		// Token: 0x040014A7 RID: 5287
		private List<Agent> mAgents = new List<Agent>(256);

		// Token: 0x040014A8 RID: 5288
		private int mUpdateIndex;
	}
}
