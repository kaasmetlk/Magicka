using System;
using System.Runtime.InteropServices;

namespace Magicka.GameLogic.Spells
{
	// Token: 0x020002EA RID: 746
	[StructLayout(LayoutKind.Explicit)]
	public struct SpellMagickConverter
	{
		// Token: 0x170005C9 RID: 1481
		// (get) Token: 0x060016E8 RID: 5864 RVA: 0x0009379E File Offset: 0x0009199E
		public bool IsMagick
		{
			get
			{
				return this.Magick.Element == Elements.All;
			}
		}

		// Token: 0x04001850 RID: 6224
		[FieldOffset(0)]
		public Spell Spell;

		// Token: 0x04001851 RID: 6225
		[FieldOffset(0)]
		public Magick Magick;
	}
}
