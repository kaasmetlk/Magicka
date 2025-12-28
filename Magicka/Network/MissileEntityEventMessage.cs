using System;
using System.IO;
using Magicka.GameLogic.Entities.Items;

namespace Magicka.Network
{
	// Token: 0x02000230 RID: 560
	internal struct MissileEntityEventMessage : ISendable
	{
		// Token: 0x1700046A RID: 1130
		// (get) Token: 0x06001151 RID: 4433 RVA: 0x0006B82D File Offset: 0x00069A2D
		public PacketType PacketType
		{
			get
			{
				return PacketType.MissileEntity;
			}
		}

		// Token: 0x06001152 RID: 4434 RVA: 0x0006B834 File Offset: 0x00069A34
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.OnCollision);
			iWriter.Write(this.Handle);
			iWriter.Write(this.HitPoints);
			iWriter.Write((ushort)this.Elements);
			iWriter.Write(this.TimeAlive);
			iWriter.Write(this.Threshold);
			iWriter.Write(this.TargetHandle);
			iWriter.Write((byte)this.EventConditionType);
		}

		// Token: 0x06001153 RID: 4435 RVA: 0x0006B8A4 File Offset: 0x00069AA4
		public void Read(BinaryReader iReader)
		{
			this.OnCollision = iReader.ReadBoolean();
			this.Handle = iReader.ReadUInt16();
			this.HitPoints = iReader.ReadSingle();
			this.Elements = (Elements)iReader.ReadUInt16();
			this.TimeAlive = iReader.ReadSingle();
			this.Threshold = iReader.ReadSingle();
			this.TargetHandle = iReader.ReadUInt16();
			this.EventConditionType = (EventConditionType)iReader.ReadByte();
		}

		// Token: 0x04001048 RID: 4168
		public bool OnCollision;

		// Token: 0x04001049 RID: 4169
		public float TimeAlive;

		// Token: 0x0400104A RID: 4170
		public float Threshold;

		// Token: 0x0400104B RID: 4171
		public float HitPoints;

		// Token: 0x0400104C RID: 4172
		public Elements Elements;

		// Token: 0x0400104D RID: 4173
		public ushort Handle;

		// Token: 0x0400104E RID: 4174
		public ushort TargetHandle;

		// Token: 0x0400104F RID: 4175
		public EventConditionType EventConditionType;
	}
}
