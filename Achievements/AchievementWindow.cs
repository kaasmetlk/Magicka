// Decompiled with JetBrains decompiler
// Type: Magicka.Achievements.AchievementWindow
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using PolygonHead;
using System;

#nullable disable
namespace Magicka.Achievements;

internal abstract class AchievementWindow
{
  protected const string UI_PATH = "content/connectui/";
  protected float mAlpha;
  protected bool mVisible;

  public virtual void Show() => this.mVisible = true;

  public virtual void Hide() => this.mVisible = false;

  public virtual void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if (this.mVisible)
      this.mAlpha = Math.Min(this.mAlpha + iDeltaTime * 8f, 1f);
    else
      this.mAlpha = Math.Max(this.mAlpha - iDeltaTime * 8f, 0.0f);
  }

  public bool Visible => this.mVisible | (double) this.mAlpha > 1.4012984643248171E-45;

  public virtual void OnLanguageChanged()
  {
  }
}
