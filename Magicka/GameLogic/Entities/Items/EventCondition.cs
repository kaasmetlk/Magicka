using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Items
{
	// Token: 0x0200024E RID: 590
	public struct EventCondition
	{
		// Token: 0x06001241 RID: 4673 RVA: 0x0006FF80 File Offset: 0x0006E180
		public EventCondition(ContentReader iInput)
		{
			this.EventConditionType = (EventConditionType)iInput.ReadByte();
			this.Hitpoints = (float)iInput.ReadInt32();
			this.Elements = (Elements)iInput.ReadInt32();
			this.Threshold = iInput.ReadSingle();
			this.Time = iInput.ReadSingle();
			this.Activated = false;
			this.Repeat = true;
			this.Count = 0;
			this.Position = null;
		}

		// Token: 0x06001242 RID: 4674 RVA: 0x0006FFEC File Offset: 0x0006E1EC
		public bool IsMet(ref EventCondition iArgs)
		{
			EventConditionType eventConditionType = iArgs.EventConditionType & this.EventConditionType;
			if (eventConditionType == (EventConditionType)0)
			{
				return false;
			}
			if (!this.Activated | this.Repeat)
			{
				int i = 0;
				while (i < 7)
				{
					EventConditionType eventConditionType2 = (EventConditionType)(1 << i);
					EventConditionType eventConditionType3 = eventConditionType2 & eventConditionType;
					if (eventConditionType3 <= EventConditionType.Damaged)
					{
						switch (eventConditionType3)
						{
						case EventConditionType.Default:
						case EventConditionType.Hit:
							goto IL_71;
						case EventConditionType.Default | EventConditionType.Hit:
							break;
						case EventConditionType.Collision:
							if (iArgs.Threshold >= this.Threshold)
							{
								this.Activated = true;
								return true;
							}
							break;
						default:
							if (eventConditionType3 == EventConditionType.Damaged)
							{
								if (iArgs.Hitpoints >= this.Hitpoints && (iArgs.Elements & this.Elements) != Elements.None)
								{
									this.Activated = true;
									return true;
								}
							}
							break;
						}
					}
					else if (eventConditionType3 != EventConditionType.Timer)
					{
						if (eventConditionType3 == EventConditionType.Death || eventConditionType3 == EventConditionType.OverKill)
						{
							goto IL_71;
						}
					}
					else if (iArgs.Time >= this.Time * (float)(this.Count + 1))
					{
						this.Count++;
						this.Activated = true;
						return true;
					}
					i++;
					continue;
					IL_71:
					this.Activated = true;
					return true;
				}
			}
			return false;
		}

		// Token: 0x040010FB RID: 4347
		public EventConditionType EventConditionType;

		// Token: 0x040010FC RID: 4348
		public float Hitpoints;

		// Token: 0x040010FD RID: 4349
		public Elements Elements;

		// Token: 0x040010FE RID: 4350
		public float Time;

		// Token: 0x040010FF RID: 4351
		public float Threshold;

		// Token: 0x04001100 RID: 4352
		public int Count;

		// Token: 0x04001101 RID: 4353
		public bool Activated;

		// Token: 0x04001102 RID: 4354
		public bool Repeat;

		// Token: 0x04001103 RID: 4355
		public Vector3? Position;
	}
}
