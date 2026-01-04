// Decompiled with JetBrains decompiler
// Type: Magicka.Storage.PlayerSaveData
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System.IO;

#nullable disable
namespace Magicka.Storage;

public class PlayerSaveData
{
  public string Staff;
  public string Weapon;

  public PlayerSaveData()
  {
    this.Staff = "";
    this.Weapon = "";
  }

  public void Write(BinaryWriter iWriter)
  {
    if (this.Staff == null)
      this.Staff = "";
    if (this.Weapon == null)
      this.Weapon = "";
    iWriter.Write(this.Staff);
    iWriter.Write(this.Weapon);
  }

  public static PlayerSaveData Read(BinaryReader iReader)
  {
    return new PlayerSaveData()
    {
      Staff = iReader.ReadString(),
      Weapon = iReader.ReadString()
    };
  }
}
