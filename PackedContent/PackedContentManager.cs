// Decompiled with JetBrains decompiler
// Type: PolygonHead.PackedContent.PackedContentManager
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework.Content;
using System;
using System.IO;
using System.IO.Compression;

#nullable disable
namespace PolygonHead.PackedContent;

public class PackedContentManager : ContentManager
{
  private PackedDirectory mRoot;
  private Stream mPackegeStream;

  public PackedContentManager(IServiceProvider iServiceProvider, string iPackage)
    : base(iServiceProvider)
  {
    this.mPackegeStream = (Stream) File.OpenRead(iPackage);
    BinaryReader iReader = new BinaryReader(this.mPackegeStream);
    iReader.BaseStream.Position = iReader.ReadInt64();
    this.mRoot = PackedItem.ReadDirectory(iReader);
  }

  protected override Stream OpenStream(string iAssetName)
  {
    this.mPackegeStream.Position = (this.GetPackedItem(iAssetName + ".xnb") as PackedFile).mOffset;
    return (Stream) new DeflateStream(this.mPackegeStream, CompressionMode.Decompress, true);
  }

  public Stream GetStream(string iAssetName)
  {
    return this.GetStream(this.GetPackedItem(iAssetName) as PackedFile);
  }

  public Stream GetStream(PackedFile iFile)
  {
    this.mPackegeStream.Position = iFile.mOffset;
    return (Stream) new DeflateStream(this.mPackegeStream, CompressionMode.Decompress, true);
  }

  public PackedItem GetPackedItem(string iPath)
  {
    iPath = iPath.ToLower();
    return this.GetPackedItem(iPath.Split('\\', '/'));
  }

  private PackedItem GetPackedItem(string[] iPath)
  {
    PackedItem packedItem = (PackedItem) this.mRoot;
    for (int index = 0; index < iPath.Length; ++index)
      packedItem = (packedItem as PackedDirectory).mChildren[iPath[index]];
    return packedItem;
  }

  protected override void Dispose(bool disposing)
  {
    this.mPackegeStream.Close();
    base.Dispose(disposing);
  }
}
