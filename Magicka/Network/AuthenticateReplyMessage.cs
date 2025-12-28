using System;
using System.IO;
using Magicka.GameLogic.GameStates;
using Magicka.Levels.Packs;
using Magicka.Levels.Versus;

namespace Magicka.Network
{
	// Token: 0x0200020E RID: 526
	internal struct AuthenticateReplyMessage : ISendable
	{
		// Token: 0x1700044F RID: 1103
		// (get) Token: 0x060010FE RID: 4350 RVA: 0x0006A00B File Offset: 0x0006820B
		public PacketType PacketType
		{
			get
			{
				return PacketType.Authenticate;
			}
		}

		// Token: 0x060010FF RID: 4351 RVA: 0x0006A010 File Offset: 0x00068210
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write((int)this.Response);
			if (this.GameInfo.GameName != null)
			{
				iWriter.Write(true);
				this.GameInfo.Write(iWriter);
				if (this.GameInfo.GameType == GameType.Versus)
				{
					this.VersusSettings.Write(iWriter);
					this.PackOptions.Write(iWriter);
					return;
				}
				if (this.GameInfo.GameType == GameType.Challenge)
				{
					this.PackOptions.Write(iWriter);
					return;
				}
			}
			else
			{
				iWriter.Write(false);
			}
		}

		// Token: 0x06001100 RID: 4352 RVA: 0x0006A094 File Offset: 0x00068294
		public void Read(BinaryReader iReader)
		{
			this.Response = (AuthenticateReplyMessage.Reply)iReader.ReadInt32();
			if (iReader.ReadBoolean())
			{
				this.GameInfo.Read(iReader);
				if (this.GameInfo.GameType == GameType.Versus)
				{
					this.VersusSettings.Read(iReader);
					this.PackOptions.Read(iReader);
					return;
				}
				if (this.GameInfo.GameType == GameType.Challenge)
				{
					this.PackOptions.Read(iReader);
				}
			}
		}

		// Token: 0x04000FBB RID: 4027
		public AuthenticateReplyMessage.Reply Response;

		// Token: 0x04000FBC RID: 4028
		public GameInfoMessage GameInfo;

		// Token: 0x04000FBD RID: 4029
		public VersusRuleset.Settings.OptionsMessage VersusSettings;

		// Token: 0x04000FBE RID: 4030
		public PackOptionsMessage PackOptions;

		// Token: 0x0200020F RID: 527
		public enum Reply
		{
			// Token: 0x04000FC0 RID: 4032
			Invalid,
			// Token: 0x04000FC1 RID: 4033
			Ok,
			// Token: 0x04000FC2 RID: 4034
			Error_ServerFull,
			// Token: 0x04000FC3 RID: 4035
			Error_AuthFailed,
			// Token: 0x04000FC4 RID: 4036
			Error_GamePlaying,
			// Token: 0x04000FC5 RID: 4037
			Error_Version,
			// Token: 0x04000FC6 RID: 4038
			Error_Password
		}
	}
}
