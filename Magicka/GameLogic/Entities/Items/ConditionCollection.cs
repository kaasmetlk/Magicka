using System;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Items
{
	// Token: 0x020003FC RID: 1020
	public class ConditionCollection
	{
		// Token: 0x06001F6E RID: 8046 RVA: 0x000DD5A0 File Offset: 0x000DB7A0
		public ConditionCollection()
		{
			for (int i = 0; i < this.mEventCollection.Length; i++)
			{
				this.mEventCollection[i] = new EventCollection(8);
			}
		}

		// Token: 0x06001F6F RID: 8047 RVA: 0x000DD5E0 File Offset: 0x000DB7E0
		public ConditionCollection(ContentReader iInput)
		{
			int num = iInput.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				this.mEventCollection[i] = EventCollection.Read(iInput);
			}
		}

		// Token: 0x170007B7 RID: 1975
		public EventCollection this[int iIndex]
		{
			get
			{
				return this.mEventCollection[iIndex];
			}
		}

		// Token: 0x170007B8 RID: 1976
		// (get) Token: 0x06001F71 RID: 8049 RVA: 0x000DD62A File Offset: 0x000DB82A
		public int Count
		{
			get
			{
				return this.mEventCollection.Length;
			}
		}

		// Token: 0x06001F72 RID: 8050 RVA: 0x000DD634 File Offset: 0x000DB834
		public void CopyTo(ConditionCollection iCollection)
		{
			for (int i = 0; i < this.mEventCollection.Length; i++)
			{
				if (this.mEventCollection[i] != null)
				{
					this.mEventCollection[i].CopyTo(iCollection[i]);
				}
			}
		}

		// Token: 0x06001F73 RID: 8051 RVA: 0x000DD674 File Offset: 0x000DB874
		public void Clear()
		{
			for (int i = 0; i < this.mEventCollection.Length; i++)
			{
				this.mEventCollection[i].Condition = default(EventCondition);
				this.mEventCollection[i].Condition.Repeat = true;
				this.mEventCollection[i].Clear();
			}
		}

		// Token: 0x06001F74 RID: 8052 RVA: 0x000DD6C8 File Offset: 0x000DB8C8
		public bool ExecuteAll(Entity iOwner, Entity iTarget, ref EventCondition iArgs)
		{
			bool flag = false;
			for (int i = 0; i < this.mEventCollection.Length; i++)
			{
				if (this.mEventCollection[i] != null)
				{
					DamageResult damageResult;
					flag |= this.mEventCollection[i].ExecuteAll(iOwner, iTarget, ref iArgs, out damageResult);
				}
			}
			return flag;
		}

		// Token: 0x06001F75 RID: 8053 RVA: 0x000DD70C File Offset: 0x000DB90C
		public bool ExecuteAll(Entity iOwner, Entity iTarget, ref EventCondition iArgs, out DamageResult oDamageResult)
		{
			bool flag = false;
			oDamageResult = DamageResult.None;
			for (int i = 0; i < this.mEventCollection.Length; i++)
			{
				if (this.mEventCollection[i] != null)
				{
					DamageResult damageResult;
					flag |= this.mEventCollection[i].ExecuteAll(iOwner, iTarget, ref iArgs, out damageResult);
					oDamageResult |= damageResult;
				}
			}
			return flag;
		}

		// Token: 0x040021E2 RID: 8674
		private EventCollection[] mEventCollection = new EventCollection[5];
	}
}
