using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SteamWrapper;

namespace Magicka.WebTools
{
	// Token: 0x020005C2 RID: 1474
	public static class InternalSteamUtils
	{
		// Token: 0x06002C3C RID: 11324 RVA: 0x0015C1A9 File Offset: 0x0015A3A9
		public static uint GetSteamAppID()
		{
			return SteamUtils.GetAppID();
		}

		// Token: 0x06002C3D RID: 11325 RVA: 0x0015C1B0 File Offset: 0x0015A3B0
		public unsafe static string GetSteamAuthToken()
		{
			uint num = 0U;
			byte[] array;
			fixed (byte* ptr = &InternalSteamUtils.mBufferClass.mSteamBuffer.mBuffer.FixedElementField)
			{
				SteamUser.GetAuthSessionTicket((void*)ptr, 1024, &num);
				array = new byte[num];
				int num2 = 0;
				while ((long)num2 < (long)((ulong)num))
				{
					array[num2] = ptr[num2];
					num2++;
				}
			}
			string text = BitConverter.ToString(array);
			return text.Replace("-", string.Empty);
		}

		// Token: 0x04002FE0 RID: 12256
		public const int STEAM_AUTH_BUFFER_SIZE = 1024;

		// Token: 0x04002FE1 RID: 12257
		private static InternalSteamUtils.BufferClass mBufferClass = new InternalSteamUtils.BufferClass();

		// Token: 0x020005C3 RID: 1475
		private struct SteamAuthBuffer
		{
			// Token: 0x04002FE2 RID: 12258
			[FixedBuffer(typeof(byte), 1024)]
			public InternalSteamUtils.SteamAuthBuffer.<mBuffer>e__FixedBuffere mBuffer;

			// Token: 0x020005C4 RID: 1476
			[UnsafeValueType]
			[CompilerGenerated]
			[StructLayout(LayoutKind.Sequential, Size = 1024)]
			public struct <mBuffer>e__FixedBuffere
			{
				// Token: 0x04002FE3 RID: 12259
				public byte FixedElementField;
			}
		}

		// Token: 0x020005C5 RID: 1477
		private class BufferClass
		{
			// Token: 0x04002FE4 RID: 12260
			public InternalSteamUtils.SteamAuthBuffer mSteamBuffer = default(InternalSteamUtils.SteamAuthBuffer);
		}
	}
}
