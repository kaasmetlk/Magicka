// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Controls.KeyData
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework.Input;

#nullable disable
namespace Magicka.GameLogic.Controls;

internal struct KeyData(Keys iKey, KeyModifiers iModifier)
{
  public Keys Key = iKey;
  public KeyModifiers Modifier = iModifier;
}
