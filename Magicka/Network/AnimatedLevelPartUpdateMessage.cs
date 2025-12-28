using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace Magicka.Network
{
	// Token: 0x02000224 RID: 548
	internal struct AnimatedLevelPartUpdateMessage : ISendable
	{
		// Token: 0x17000462 RID: 1122
		// (get) Token: 0x06001139 RID: 4409 RVA: 0x0006AB48 File Offset: 0x00068D48
		public PacketType PacketType
		{
			get
			{
				return PacketType.AnimatedLevelPartUpdate;
			}
		}

		// Token: 0x0600113A RID: 4410 RVA: 0x0006AB4C File Offset: 0x00068D4C
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.Handle);
			iWriter.Write(this.Playing);
			iWriter.Write(new HalfSingle(this.AnimationTime).PackedValue);
		}

		// Token: 0x0600113B RID: 4411 RVA: 0x0006AB8C File Offset: 0x00068D8C
		public void Read(BinaryReader iReader)
		{
			this.Handle = iReader.ReadUInt16();
			this.Playing = iReader.ReadBoolean();
			HalfSingle halfSingle = default(HalfSingle);
			halfSingle.PackedValue = iReader.ReadUInt16();
			this.AnimationTime = halfSingle.ToSingle();
		}

		// Token: 0x0400100F RID: 4111
		public ushort Handle;

		// Token: 0x04001010 RID: 4112
		public bool Playing;

		// Token: 0x04001011 RID: 4113
		public float AnimationTime;
	}
}
