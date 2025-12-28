using System;
using Magicka.GameLogic.GameStates;
using SteamWrapper;

namespace Magicka.Network
{
	// Token: 0x020002C3 RID: 707
	internal abstract class NetworkInterface
	{
		// Token: 0x1700056D RID: 1389
		// (get) Token: 0x06001570 RID: 5488 RVA: 0x0008A3A6 File Offset: 0x000885A6
		public GameType GameType
		{
			get
			{
				return this.mGameType;
			}
		}

		// Token: 0x06001571 RID: 5489
		public abstract void Dispose();

		// Token: 0x1700056E RID: 1390
		// (get) Token: 0x06001572 RID: 5490
		public abstract int Connections { get; }

		// Token: 0x1700056F RID: 1391
		// (get) Token: 0x06001573 RID: 5491
		public abstract bool IsVACSecure { get; }

		// Token: 0x06001574 RID: 5492
		public abstract float GetLatency(int iConnection);

		// Token: 0x06001575 RID: 5493
		public abstract int GetLatencyMS(int iConnection);

		// Token: 0x17000570 RID: 1392
		// (get) Token: 0x06001576 RID: 5494
		public abstract SteamID ServerID { get; }

		// Token: 0x06001577 RID: 5495
		public abstract float GetLatency(SteamID iConnection);

		// Token: 0x06001578 RID: 5496
		public abstract int GetLatencyMS(SteamID iConnection);

		// Token: 0x06001579 RID: 5497 RVA: 0x0008A3AE File Offset: 0x000885AE
		public virtual void SendMessage<T>(ref T iMessage) where T : ISendable
		{
			this.SendMessage<T>(ref iMessage, P2PSend.ReliableWithBuffering);
		}

		// Token: 0x0600157A RID: 5498
		public abstract void SendMessage<T>(ref T iMessage, P2PSend sendType) where T : ISendable;

		// Token: 0x0600157B RID: 5499 RVA: 0x0008A3B8 File Offset: 0x000885B8
		public virtual void SendMessage<T>(ref T iMessage, int iPeer) where T : ISendable
		{
			this.SendMessage<T>(ref iMessage, iPeer, P2PSend.ReliableWithBuffering);
		}

		// Token: 0x0600157C RID: 5500
		public abstract void SendMessage<T>(ref T iMessage, int iPeer, P2PSend sendType) where T : ISendable;

		// Token: 0x0600157D RID: 5501 RVA: 0x0008A3C3 File Offset: 0x000885C3
		public virtual void SendMessage<T>(ref T iMessage, SteamID iPeer) where T : ISendable
		{
			this.SendMessage<T>(ref iMessage, iPeer, P2PSend.ReliableWithBuffering);
		}

		// Token: 0x0600157E RID: 5502
		public abstract void SendMessage<T>(ref T iMessage, SteamID iPeer, P2PSend sendType) where T : ISendable;

		// Token: 0x0600157F RID: 5503
		public abstract void SendUdpMessage<T>(ref T iMessage) where T : ISendable;

		// Token: 0x06001580 RID: 5504
		public abstract void SendUdpMessage<T>(ref T iMessage, int iPeer) where T : ISendable;

		// Token: 0x06001581 RID: 5505
		public abstract void SendUdpMessage<T>(ref T iMessage, SteamID iPeer) where T : ISendable;

		// Token: 0x06001582 RID: 5506
		public unsafe abstract void SendRaw(PacketType iType, void* iPtr, int iLength);

		// Token: 0x06001583 RID: 5507
		public unsafe abstract void SendRaw(PacketType iType, void* iPtr, int iLength, int iPeer);

		// Token: 0x06001584 RID: 5508
		public unsafe abstract void SendRaw(PacketType iType, void* iPtr, int iLength, SteamID iPeer);

		// Token: 0x06001585 RID: 5509
		public abstract void Sync();

		// Token: 0x06001586 RID: 5510
		public abstract void Update();

		// Token: 0x06001587 RID: 5511
		public abstract void CloseConnection();

		// Token: 0x06001588 RID: 5512
		public abstract void FlushMessageBuffers();

		// Token: 0x040016ED RID: 5869
		protected GameType mGameType;
	}
}
