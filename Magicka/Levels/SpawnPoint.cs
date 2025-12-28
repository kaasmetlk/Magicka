using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Magicka.Levels
{
	// Token: 0x02000009 RID: 9
	public struct SpawnPoint
	{
		// Token: 0x0600000D RID: 13 RVA: 0x00002284 File Offset: 0x00000484
		public unsafe SpawnPoint(string iScene, string iLocation, bool iSpawnPlayers)
		{
			this.Scene = iScene.GetHashCodeCustom();
			fixed (int* ptr = &this.Locations.FixedElementField)
			{
				for (int i = 0; i < 4; i++)
				{
					ptr[i] = (iLocation + i).GetHashCodeCustom();
				}
			}
			this.SpawnPlayers = iSpawnPlayers;
		}

		// Token: 0x0400002C RID: 44
		public int Scene;

		// Token: 0x0400002D RID: 45
		[FixedBuffer(typeof(int), 4)]
		public SpawnPoint.<Locations>e__FixedBuffer0 Locations;

		// Token: 0x0400002E RID: 46
		public bool SpawnPlayers;

		// Token: 0x0200000A RID: 10
		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 16)]
		public struct <Locations>e__FixedBuffer0
		{
			// Token: 0x0400002F RID: 47
			public int FixedElementField;
		}
	}
}
