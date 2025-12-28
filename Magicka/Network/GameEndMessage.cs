using System;
using System.IO;
using Magicka.Levels;

namespace Magicka.Network
{
	// Token: 0x02000222 RID: 546
	public struct GameEndMessage : ISendable
	{
		// Token: 0x17000460 RID: 1120
		// (get) Token: 0x06001133 RID: 4403 RVA: 0x0006A879 File Offset: 0x00068A79
		public PacketType PacketType
		{
			get
			{
				return PacketType.GameEnd;
			}
		}

		// Token: 0x06001134 RID: 4404 RVA: 0x0006A87D File Offset: 0x00068A7D
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write((byte)this.Condition);
			iWriter.Write(this.Argument);
			iWriter.Write(this.DelayTime);
			iWriter.Write(this.Phony);
		}

		// Token: 0x06001135 RID: 4405 RVA: 0x0006A8AF File Offset: 0x00068AAF
		public void Read(BinaryReader iReader)
		{
			this.Condition = (EndGameCondition)iReader.ReadByte();
			this.Argument = iReader.ReadInt32();
			this.DelayTime = iReader.ReadSingle();
			this.Phony = iReader.ReadBoolean();
		}

		// Token: 0x04001006 RID: 4102
		public EndGameCondition Condition;

		// Token: 0x04001007 RID: 4103
		public int Argument;

		// Token: 0x04001008 RID: 4104
		public float DelayTime;

		// Token: 0x04001009 RID: 4105
		public bool Phony;
	}
}
