// Decompiled with JetBrains decompiler
// Type: PolygonHead.Pipeline.BiTreeModelReader
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework.Content;
using PolygonHead.Models;

#nullable disable
namespace PolygonHead.Pipeline;

public class BiTreeModelReader : ContentTypeReader<BiTreeModel>
{
  protected override BiTreeModel Read(ContentReader input, BiTreeModel existingInstance)
  {
    return BiTreeModel.Read(input);
  }
}
