// Decompiled with JetBrains decompiler
// Type: Magicka.ContentReaders.CharacterTemplateReader
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using Microsoft.Xna.Framework.Content;

#nullable disable
namespace Magicka.ContentReaders;

public class CharacterTemplateReader : ContentTypeReader<CharacterTemplate>
{
  protected override CharacterTemplate Read(ContentReader input, CharacterTemplate existingInstance)
  {
    return CharacterTemplate.Read(input);
  }
}
