// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Locator
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

#nullable disable
namespace Magicka.Levels;

public struct Locator
{
  public static readonly string DISTANCE_VAR_NAME = "Distance";
  public string Name;
  public Matrix Transform;
  public float Radius;

  public Locator(string iName, ContentReader iInput)
  {
    this.Name = iName;
    this.Transform = iInput.ReadMatrix();
    this.Radius = iInput.ReadSingle();
  }

  public override string ToString() => this.Name;
}
