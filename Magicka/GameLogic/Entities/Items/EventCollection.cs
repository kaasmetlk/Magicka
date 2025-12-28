using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Items
{
	// Token: 0x0200024F RID: 591
	public class EventCollection : List<EventStorage>
	{
		// Token: 0x06001243 RID: 4675 RVA: 0x000700EB File Offset: 0x0006E2EB
		public EventCollection(int iCapacity) : base(iCapacity)
		{
			this.Condition.EventConditionType = EventConditionType.Default;
			EventCollection.Repeat = false;
		}

		// Token: 0x06001244 RID: 4676 RVA: 0x00070108 File Offset: 0x0006E308
		public static EventCollection Read(ContentReader iInput)
		{
			EventCondition condition = new EventCondition(iInput);
			EventCollection.Repeat = iInput.ReadBoolean();
			condition.Repeat = EventCollection.Repeat;
			int num = iInput.ReadInt32();
			EventCollection eventCollection = new EventCollection(num);
			eventCollection.Condition = condition;
			for (int i = 0; i < num; i++)
			{
				EventStorage item = new EventStorage(iInput);
				eventCollection.Add(item);
			}
			return eventCollection;
		}

		// Token: 0x06001245 RID: 4677 RVA: 0x00070168 File Offset: 0x0006E368
		public void ExecuteAll(Entity iOwner, Entity iTarget, Vector3? iPosition)
		{
			for (int i = 0; i < base.Count; i++)
			{
				base[i].Execute(iOwner, iTarget, ref iPosition);
			}
		}

		// Token: 0x06001246 RID: 4678 RVA: 0x0007019C File Offset: 0x0006E39C
		public bool ExecuteAll(Entity iOwner, Entity iTarget, ref EventCondition iArgs, out DamageResult oDamageResult)
		{
			oDamageResult = DamageResult.None;
			if (this.Condition.IsMet(ref iArgs))
			{
				for (int i = 0; i < base.Count; i++)
				{
					EventStorage value = base[i];
					oDamageResult |= value.Execute(iOwner, iTarget, ref iArgs.Position);
					base[i] = value;
				}
				if (base.Count > 0)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001247 RID: 4679 RVA: 0x000701FD File Offset: 0x0006E3FD
		public virtual void CopyTo(EventCollection iCollection)
		{
			iCollection.Clear();
			iCollection.AddRange(this);
			iCollection.Condition = this.Condition;
		}

		// Token: 0x04001104 RID: 4356
		public EventCondition Condition;

		// Token: 0x04001105 RID: 4357
		public static bool Repeat = true;
	}
}
