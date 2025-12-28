using System;
using Magicka.DRM;
using Microsoft.Xna.Framework;

namespace Magicka.Levels.Packs
{
	// Token: 0x020004FC RID: 1276
	internal interface IPack
	{
		// Token: 0x170008C7 RID: 2247
		// (get) Token: 0x060025B6 RID: 9654
		// (set) Token: 0x060025B7 RID: 9655
		bool Enabled { get; set; }

		// Token: 0x170008C8 RID: 2248
		// (get) Token: 0x060025B8 RID: 9656
		int Name { get; }

		// Token: 0x170008C9 RID: 2249
		// (get) Token: 0x060025B9 RID: 9657
		int Descritpion { get; }

		// Token: 0x170008CA RID: 2250
		// (get) Token: 0x060025BA RID: 9658
		Vector2 ThumbOffset { get; }

		// Token: 0x170008CB RID: 2251
		// (get) Token: 0x060025BB RID: 9659
		uint StoreURL { get; }

		// Token: 0x170008CC RID: 2252
		// (get) Token: 0x060025BC RID: 9660
		HackHelper.License License { get; }

		// Token: 0x170008CD RID: 2253
		// (get) Token: 0x060025BD RID: 9661
		bool IsUsed { get; }

		// Token: 0x060025BE RID: 9662
		void SetUsed(bool forceSave);
	}
}
