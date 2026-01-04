// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Items.RemoveEvent
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework.Content;

#nullable disable
namespace Magicka.GameLogic.Entities.Items;

public struct RemoveEvent
{
  public int Bounce;

  public RemoveEvent(int iNrOfBounces) => this.Bounce = iNrOfBounces;

  public RemoveEvent(ContentReader iInput) => this.Bounce = iInput.ReadInt32();

  public void Execute(Entity iItem, Entity iTarget)
  {
    if (iItem is Character)
      (iItem as Character).Terminate(true, false);
    else if (this.Bounce <= 0)
      iItem.Kill();
    else
      --this.Bounce;
  }
}
