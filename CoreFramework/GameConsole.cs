// Decompiled with JetBrains decompiler
// Type: Magicka.CoreFramework.GameConsole
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Misc;
using PolygonHead;
using PolygonHead.Effects;
using System.Collections.Generic;

#nullable disable
namespace Magicka.CoreFramework;

public class GameConsole : Singleton<GameConsole>
{
  private const int MAX_TEXT_SIZE = 5000;
  public const int DEFAULT_PRIORITY = 1000;
  private List<GameConsole.Input> mTextInputs = new List<GameConsole.Input>();
  private Text mText;
  private object mListLock = new object();
  private bool mEnabled = true;

  public bool Enabled
  {
    get => this.mEnabled;
    set => this.mEnabled = value;
  }

  public void Init(BitmapFont iFont)
  {
  }

  public void PushInput(GameConsole.DisplayTextDelegate iDelegate, int iPriority)
  {
  }

  public void PushInput(GameConsole.DisplayTextDelegate iDelegate)
  {
    this.PushInput(iDelegate, 1000);
  }

  public void Draw(GUIBasicEffect iEffect, float iPosX, float iPoxY)
  {
  }

  public delegate string DisplayTextDelegate();

  private class Input
  {
    public readonly int Priority;
    public readonly GameConsole.DisplayTextDelegate Delegate;

    public Input(int iPriority, GameConsole.DisplayTextDelegate iDelegate)
    {
      this.Priority = iPriority;
      this.Delegate = iDelegate;
    }
  }
}
