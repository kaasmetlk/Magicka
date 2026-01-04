// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Packs.PackOptionsMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.DRM;
using Magicka.Network;
using System.IO;
using System.Runtime.InteropServices;

#nullable disable
namespace Magicka.Levels.Packs;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal struct PackOptionsMessage : ISendable
{
  public PacketType PacketType => PacketType.PackOptions;

  public void Write(BinaryWriter iWriter)
  {
    foreach (ItemPack itemPack in PackMan.Instance.ItemPacks)
      iWriter.Write(itemPack.Enabled);
    foreach (MagickPack magickPack in PackMan.Instance.MagickPacks)
      iWriter.Write(magickPack.Enabled);
  }

  public void Read(BinaryReader iReader)
  {
    ItemPack[] itemPacks = PackMan.Instance.ItemPacks;
    for (int index = 0; index < itemPacks.Length; ++index)
    {
      itemPacks[index].License = HackHelper.License.Yes;
      itemPacks[index].Enabled = iReader.ReadBoolean();
    }
    MagickPack[] magickPacks = PackMan.Instance.MagickPacks;
    for (int index = 0; index < magickPacks.Length; ++index)
    {
      magickPacks[index].License = HackHelper.License.Yes;
      magickPacks[index].Enabled = iReader.ReadBoolean();
    }
  }
}
