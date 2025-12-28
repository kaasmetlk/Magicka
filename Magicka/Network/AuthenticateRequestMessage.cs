using System;
using System.IO;

namespace Magicka.Network
{
	// Token: 0x0200020D RID: 525
	internal struct AuthenticateRequestMessage : ISendable
	{
		// Token: 0x1700044E RID: 1102
		// (get) Token: 0x060010FB RID: 4347 RVA: 0x00069EFA File Offset: 0x000680FA
		public PacketType PacketType
		{
			get
			{
				return (PacketType)130;
			}
		}

		// Token: 0x060010FC RID: 4348 RVA: 0x00069F04 File Offset: 0x00068104
		public unsafe void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.NrOfPlayers);
			iWriter.Write(this.Token.Length);
			fixed (byte* ptr = &this.Token.Data.FixedElementField)
			{
				for (int i = 0; i < this.Token.Length; i++)
				{
					iWriter.Write(ptr[i]);
				}
			}
			iWriter.Write(this.Version);
			if (string.IsNullOrEmpty(this.Password))
			{
				iWriter.Write("");
				return;
			}
			iWriter.Write(this.Password);
		}

		// Token: 0x060010FD RID: 4349 RVA: 0x00069F94 File Offset: 0x00068194
		public unsafe void Read(BinaryReader iReader)
		{
			this.NrOfPlayers = iReader.ReadByte();
			this.Token.Length = iReader.ReadInt32();
			fixed (byte* ptr = &this.Token.Data.FixedElementField)
			{
				for (int i = 0; i < this.Token.Length; i++)
				{
					ptr[i] = iReader.ReadByte();
				}
			}
			this.Version = iReader.ReadUInt64();
			this.Password = iReader.ReadString();
		}

		// Token: 0x04000FB7 RID: 4023
		public AuthenticationToken Token;

		// Token: 0x04000FB8 RID: 4024
		public byte NrOfPlayers;

		// Token: 0x04000FB9 RID: 4025
		public ulong Version;

		// Token: 0x04000FBA RID: 4026
		public string Password;
	}
}
