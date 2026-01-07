// Decompiled with JetBrains decompiler
// Type: PolygonHead.PackedContent.PackedItem
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using System.Collections.Generic;
using System.IO;

#nullable disable
namespace PolygonHead.PackedContent;

public abstract class PackedItem
{
  public static PackedDirectory ReadDirectory(BinaryReader iReader)
  {
    PackedDirectory packedDirectory = new PackedDirectory();
    int num = iReader.ReadInt32();
    for (int index = 0; index < num; ++index)
    {
      bool flag = iReader.ReadBoolean();
      string key = iReader.ReadString();
      if (flag)
        packedDirectory.mChildren.Add(key, (PackedItem) new PackedFile(iReader.ReadInt64()));
      else
        packedDirectory.mChildren.Add(key, (PackedItem) PackedItem.ReadDirectory(iReader));
    }
    return packedDirectory;
  }

  public static void WriteDirectory(BinaryWriter iWriter, PackedDirectory iDirectory)
  {
    iWriter.Write(iDirectory.mChildren.Count);
    foreach (KeyValuePair<string, PackedItem> mChild in iDirectory.mChildren)
    {
      iWriter.Write(mChild.Value is PackedFile);
      iWriter.Write(mChild.Key);
      if (mChild.Value is PackedFile)
        iWriter.Write((mChild.Value as PackedFile).mOffset);
      else
        PackedItem.WriteDirectory(iWriter, mChild.Value as PackedDirectory);
    }
  }
}
