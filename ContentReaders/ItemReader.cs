// Decompiled with JetBrains decompiler
// Type: Magicka.ContentReaders.ItemReader
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities.Items;
using Microsoft.Xna.Framework.Content;

#nullable disable
namespace Magicka.ContentReaders;

public class ItemReader : ContentTypeReader<Item>
{
  protected override Item Read(ContentReader iInput, Item existingInstance) => Item.Read(iInput);
}
