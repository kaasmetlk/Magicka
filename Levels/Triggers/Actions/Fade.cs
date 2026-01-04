// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.Fade
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class Fade : Action
{
  private bool mFadeIn;
  private Color mColor;
  private float mTime;
  private bool mOverride;
  private new GameScene mScene;

  public Fade(Trigger iTrigger, GameScene iScene)
    : base(iTrigger, iScene)
  {
    this.mScene = iScene;
  }

  protected override void Execute()
  {
    if (this.mFadeIn)
      RenderManager.Instance.BeginTransition(Transitions.Fade, this.mColor, this.mTime);
    else
      RenderManager.Instance.EndTransition(Transitions.Fade, this.mColor, this.mTime);
    if (!this.mOverride)
      return;
    this.GameScene.Level.ClearTransition();
  }

  public Color Color
  {
    get => this.mColor;
    set => this.mColor = value;
  }

  public bool FadeIn
  {
    get => this.mFadeIn;
    set => this.mFadeIn = value;
  }

  public bool Override
  {
    get => this.mOverride;
    set => this.mOverride = value;
  }

  public float Time
  {
    get => this.mTime;
    set => this.mTime = value;
  }

  public override void QuickExecute()
  {
  }
}
