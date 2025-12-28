using System;
using System.IO;
using Magicka.DRM;
using Magicka.Network;

namespace Magicka.Levels.Packs
{
	// Token: 0x020004FB RID: 1275
	internal struct PackOptionsMessage : ISendable
	{
		// Token: 0x170008C6 RID: 2246
		// (get) Token: 0x060025B3 RID: 9651 RVA: 0x0011171D File Offset: 0x0010F91D
		public PacketType PacketType
		{
			get
			{
				return PacketType.PackOptions;
			}
		}

		// Token: 0x060025B4 RID: 9652 RVA: 0x00111724 File Offset: 0x0010F924
		public void Write(BinaryWriter iWriter)
		{
			ItemPack[] itemPacks = PackMan.Instance.ItemPacks;
			for (int i = 0; i < itemPacks.Length; i++)
			{
				iWriter.Write(itemPacks[i].Enabled);
			}
			MagickPack[] magickPacks = PackMan.Instance.MagickPacks;
			for (int j = 0; j < magickPacks.Length; j++)
			{
				iWriter.Write(magickPacks[j].Enabled);
			}
		}

		// Token: 0x060025B5 RID: 9653 RVA: 0x00111780 File Offset: 0x0010F980
		public void Read(BinaryReader iReader)
		{
			ItemPack[] itemPacks = PackMan.Instance.ItemPacks;
			for (int i = 0; i < itemPacks.Length; i++)
			{
				itemPacks[i].License = HackHelper.License.Yes;
				itemPacks[i].Enabled = iReader.ReadBoolean();
			}
			MagickPack[] magickPacks = PackMan.Instance.MagickPacks;
			for (int j = 0; j < magickPacks.Length; j++)
			{
				magickPacks[j].License = HackHelper.License.Yes;
				magickPacks[j].Enabled = iReader.ReadBoolean();
			}
		}
	}
}
