// Decompiled with JetBrains decompiler
// Type: PolygonHead.PackedContent.PackedDirectory
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using System.Collections.Generic;

#nullable disable
namespace PolygonHead.PackedContent;

public class PackedDirectory : PackedItem
{
  internal Dictionary<string, PackedItem> mChildren;

  public PackedDirectory() => this.mChildren = new Dictionary<string, PackedItem>();

  public void AddItem(string iPath, PackedItem iItem)
  {
    string[] strArray = iPath.Split('\\', '/');
    PackedDirectory packedDirectory = this;
    for (int index = 0; index < strArray.Length - 1; ++index)
    {
      PackedItem packedItem;
      if (!packedDirectory.mChildren.TryGetValue(strArray[index], out packedItem))
      {
        packedItem = (PackedItem) new PackedDirectory();
        packedDirectory.mChildren.Add(strArray[index], packedItem);
      }
      packedDirectory = packedItem as PackedDirectory;
    }
    packedDirectory.mChildren.Add(strArray[strArray.Length - 1], iItem);
  }

  public Dictionary<string, PackedItem> Children => this.mChildren;
}
