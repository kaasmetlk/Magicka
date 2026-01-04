// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.WaveIndicator
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Graphics;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

#nullable disable
namespace Magicka.Levels;

public class WaveIndicator
{
  private static readonly int LOC_WAVE = "#challenge_wave".GetHashCodeCustom();
  private WaveIndicator.RenderData[] mRenderData;
  private Text mText;
  private int mWaveNum;

  public WaveIndicator()
  {
    this.mRenderData = new WaveIndicator.RenderData[3];
    GUIBasicEffect guiBasicEffect = new GUIBasicEffect(Magicka.Game.Instance.GraphicsDevice, (EffectPool) null);
    guiBasicEffect.SetScreenSize(RenderManager.Instance.ScreenSize.X, RenderManager.Instance.ScreenSize.Y);
    guiBasicEffect.Color = new Vector4(1f, 1f, 1f, 1f);
    this.mText = new Text(48 /*0x30*/, FontManager.Instance.GetFont(MagickaFont.Maiandra16), TextAlign.Center, false);
    this.mText.SetText(LanguageManager.Instance.GetString(WaveIndicator.LOC_WAVE));
    Vector2 vector2 = new Vector2((float) ((double) RenderManager.Instance.ScreenSize.X * 0.5 - (double) this.mText.Font.MeasureText(this.mText.Characters, true).X * 0.5), 8f);
    for (int index = 0; index < this.mRenderData.Length; ++index)
    {
      this.mRenderData[index] = new WaveIndicator.RenderData();
      WaveIndicator.RenderData renderData = this.mRenderData[index];
      renderData.mGuiEffect = guiBasicEffect;
      renderData.mRenderableText = this.mText;
      renderData.mPosition = vector2;
    }
    this.mWaveNum = 0;
  }

  public void Update(float iDeltaTime, DataChannel iDataChan, Scene iScene)
  {
    if (iDataChan == DataChannel.None)
      return;
    iScene.AddRenderableGUIObject(iDataChan, (IRenderableGUIObject) this.mRenderData[(int) iDataChan]);
  }

  public int WaveNum => this.mWaveNum;

  public void SetWave(int iWaveNum)
  {
    this.mWaveNum = iWaveNum;
    this.mText.SetText(LanguageManager.Instance.GetString(WaveIndicator.LOC_WAVE).Replace("#1;", iWaveNum.ToString()));
  }

  protected class RenderData : IRenderableGUIObject
  {
    public Text mRenderableText;
    public GUIBasicEffect mGuiEffect;
    public Vector2 mPosition;

    public void Draw(float iDeltaTime)
    {
      this.mGuiEffect.SetScreenSize(RenderManager.Instance.ScreenSize.X, RenderManager.Instance.ScreenSize.Y);
      this.mGuiEffect.Begin();
      this.mGuiEffect.CurrentTechnique.Passes[0].Begin();
      this.mRenderableText.Draw(this.mGuiEffect, this.mPosition.X, this.mPosition.Y);
      this.mGuiEffect.CurrentTechnique.Passes[0].End();
      this.mGuiEffect.End();
    }

    public int ZIndex => 1226;
  }
}
