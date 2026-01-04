// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.Popup.PopupAlign
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System;

#nullable disable
namespace Magicka.GameLogic.UI.Popup;

[Flags]
public enum PopupAlign
{
  None = 0,
  Top = 1,
  Middle = 2,
  Bottom = 4,
  Left = 8,
  Center = 16, // 0x00000010
  Right = 32, // 0x00000020
}
