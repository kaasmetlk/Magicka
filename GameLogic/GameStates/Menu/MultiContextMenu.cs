// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.MultiContextMenu
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using PolygonHead;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu;

internal class MultiContextMenu(BitmapFont iFont, TextAlign iAlignment, int? iWidth) : ContextMenu(iFont, iAlignment, iWidth)
{
  public int AddOption(int[] iLocValues)
  {
    lock (this.mNames)
    {
      this.mTextScales.Add(1f);
      this.mTexts.Add(new Text(64 /*0x40*/, this.mFont, this.mAlignment, false));
      this.mNames.Add(iLocValues);
    }
    this.LanguageChanged();
    return this.mTexts.Count - 1;
  }
}
