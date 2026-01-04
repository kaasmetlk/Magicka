// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Items.CallbackEvent
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework.Content;
using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Magicka.GameLogic.Entities.Items;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct CallbackEvent
{
  private static CallbackEvent.ItemCallbackFn callbackFn;

  public static event CallbackEvent.ItemCallbackFn callbackFn
  {
    add => CallbackEvent.callbackFn += value;
    remove => CallbackEvent.callbackFn -= value;
  }

  public CallbackEvent(ContentReader iInput)
  {
    throw new Exception("Unhandled callback event! CallbackEvent cannot be called from script.");
  }

  public CallbackEvent(CallbackEvent.ItemCallbackFn func) => CallbackEvent.callbackFn = func;

  public void Execute(Entity iItem, Entity iTarget)
  {
    if (iItem is Character)
      (iItem as Character).Terminate(true, false);
    else
      CallbackEvent.callbackFn(iItem, iTarget);
  }

  public delegate void ItemCallbackFn(Entity iItem, Entity iTarget);
}
