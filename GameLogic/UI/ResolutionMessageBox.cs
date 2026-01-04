// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.ResolutionMessageBox
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.UI;

internal class ResolutionMessageBox : MessageBox
{
  private const float BACK_PADDING = 48f;
  private const int VISIBLE_RESOLUTIONS = 12;
  private static readonly int LOC_RESOLUTIONS = "#menu_opt_gfx_10".GetHashCodeCustom();
  private static ResolutionMessageBox sSingelton;
  private static volatile object sSingeltonLock = new object();
  private SortedList<uint, KeyValuePair<MenuTextItem, List<int>>> mResolutions;
  private MenuScrollBar mResolutionScrollBar;
  private MenuTextItem mBackItem;
  private int mCurrentResolution;

  public static ResolutionMessageBox Instance
  {
    get
    {
      if (ResolutionMessageBox.sSingelton == null)
      {
        lock (ResolutionMessageBox.sSingeltonLock)
        {
          if (ResolutionMessageBox.sSingelton == null)
            ResolutionMessageBox.sSingelton = new ResolutionMessageBox();
        }
      }
      return ResolutionMessageBox.sSingelton;
    }
  }

  public event Action<ResolutionData> Complete;

  private ResolutionMessageBox()
    : base(ResolutionMessageBox.LOC_RESOLUTIONS)
  {
    this.mResolutions = new SortedList<uint, KeyValuePair<MenuTextItem, List<int>>>();
    float lineHeight = (float) this.mFont.LineHeight;
    foreach (DisplayMode supportedDisplayMode in Magicka.Game.Instance.GraphicsDevice.CreationParameters.Adapter.SupportedDisplayModes)
    {
      if (supportedDisplayMode.Width >= 800 && supportedDisplayMode.Height >= 600)
      {
        uint key = (uint) (4294901760UL & (ulong) ((int) (ushort) supportedDisplayMode.Width << 16 /*0x10*/)) | (uint) ushort.MaxValue & (uint) (ushort) supportedDisplayMode.Height;
        KeyValuePair<MenuTextItem, List<int>> keyValuePair;
        if (!this.mResolutions.TryGetValue(key, out keyValuePair))
        {
          keyValuePair = new KeyValuePair<MenuTextItem, List<int>>(new MenuTextItem($"{supportedDisplayMode.Width} x {supportedDisplayMode.Height}", new Vector2(), this.mFont, TextAlign.Center), new List<int>());
          this.mResolutions.Add(key, keyValuePair);
        }
        if (!keyValuePair.Value.Contains(supportedDisplayMode.RefreshRate))
          keyValuePair.Value.Add(supportedDisplayMode.RefreshRate);
      }
    }
    Vector2 iPosition = new Vector2((float) ((double) this.mCenter.X + (double) this.mSize.X * 0.5 - 64.0), this.mCenter.Y);
    this.mResolutionScrollBar = new MenuScrollBar(iPosition, (float) ((double) lineHeight * 12.0 * 1.25), this.mResolutions.Count - 12);
    this.mResolutionScrollBar.Scale = 0.8f;
    iPosition = new Vector2(this.mCenter.X, (float) ((double) this.mCenter.Y + (double) this.mSize.Y * 0.5 - 48.0));
    this.mBackItem = new MenuTextItem(SubMenu.LOC_CANCEL, iPosition, this.mFont, TextAlign.Center);
    this.mBackItem.ColorDisabled = Defines.MESSAGEBOX_COLOR_DEFAULT * 0.5f;
    this.mBackItem.Color = Defines.MESSAGEBOX_COLOR_DEFAULT;
    this.mBackItem.ColorSelected = Vector4.One;
  }

  public override void LanguageChanged()
  {
    base.LanguageChanged();
    this.mBackItem.LanguageChanged();
  }

  public override void Show()
  {
    base.Show();
    this.mResolutionScrollBar.Position = new Vector2((float) ((double) this.mCenter.X + (double) this.mSize.X * 0.5 - 64.0), this.mCenter.Y);
    this.mBackItem.Position = new Vector2(this.mCenter.X, (float) ((double) this.mCenter.Y + (double) this.mSize.Y * 0.5 - 48.0));
  }

  public override void OnTextInput(char iChar)
  {
  }

  public override void OnMove(Controller iSender, ControllerDirection iDirection)
  {
    switch (iDirection)
    {
      case ControllerDirection.Right:
        if (this.mBackItem.Selected)
        {
          this.mBackItem.Selected = false;
          this.mCurrentResolution = 0;
          break;
        }
        this.mBackItem.Selected = true;
        this.mCurrentResolution = -1;
        break;
      case ControllerDirection.Up:
        if (this.mBackItem.Selected)
        {
          this.mBackItem.Selected = false;
          this.mCurrentResolution = this.mResolutions.Count - 1;
          break;
        }
        --this.mCurrentResolution;
        if (this.mCurrentResolution < 0)
        {
          this.mBackItem.Selected = true;
          return;
        }
        break;
      case ControllerDirection.Left:
        if (this.mBackItem.Selected)
        {
          this.mBackItem.Selected = false;
          this.mCurrentResolution = 0;
          break;
        }
        this.mBackItem.Selected = true;
        this.mCurrentResolution = -1;
        break;
      case ControllerDirection.Down:
        if (this.mBackItem.Selected)
        {
          this.mBackItem.Selected = false;
          this.mCurrentResolution = 0;
          break;
        }
        ++this.mCurrentResolution;
        if (this.mCurrentResolution >= this.mResolutions.Count)
        {
          this.mBackItem.Selected = true;
          return;
        }
        break;
    }
    if (this.mCurrentResolution < 0)
      return;
    while (this.mResolutionScrollBar.Value > this.mCurrentResolution)
      --this.mResolutionScrollBar.Value;
    while (this.mResolutionScrollBar.Value + 12 <= this.mCurrentResolution)
      ++this.mResolutionScrollBar.Value;
  }

  public override void OnSelect(Controller iSender)
  {
    if (this.mCurrentResolution < this.mResolutions.Count && this.mCurrentResolution >= 0)
    {
      ResolutionData resolutionData = new ResolutionData();
      uint key = this.mResolutions.Keys[this.mCurrentResolution];
      resolutionData.Width = (int) ushort.MaxValue & (int) (key >> 16 /*0x10*/);
      resolutionData.Height = (int) ushort.MaxValue & (int) key;
      if (this.Complete != null)
        this.Complete(resolutionData);
    }
    this.Kill();
  }

  public override void Kill()
  {
    base.Kill();
    SaveManager.Instance.SaveSettings();
  }

  public override void OnMouseMove(MouseState iNewState, MouseState iOldState)
  {
    if (this.mBackItem.Enabled && this.mBackItem.InsideBounds((float) iNewState.X, (float) iNewState.Y))
    {
      this.mBackItem.Selected = true;
    }
    else
    {
      this.mBackItem.Selected = false;
      this.mCurrentResolution = -1;
      if (this.mResolutionScrollBar.Grabbed)
      {
        this.mResolutionScrollBar.ScrollTo((float) iNewState.Y);
      }
      else
      {
        for (int index = this.mResolutionScrollBar.Value; index < Math.Min(this.mResolutionScrollBar.Value + 12, this.mResolutions.Count); ++index)
        {
          MenuItem key = (MenuItem) this.mResolutions.Values[index].Key;
          if (key.Enabled & key.InsideBounds(iNewState))
          {
            this.mCurrentResolution = index;
            break;
          }
        }
      }
    }
  }

  public override void OnMouseClick(MouseState iNewState, MouseState iOldState)
  {
    if (iNewState.LeftButton == ButtonState.Released)
      this.mResolutionScrollBar.Grabbed = false;
    if (iNewState.LeftButton == ButtonState.Released && iOldState.LeftButton == ButtonState.Pressed && this.mBackItem.Enabled && this.mBackItem.InsideBounds((float) iNewState.X, (float) iNewState.Y))
      this.Kill();
    else if (this.mResolutionScrollBar.InsideBounds(iNewState))
    {
      if (iNewState.LeftButton == ButtonState.Pressed && this.mResolutionScrollBar.InsideDragBounds(iNewState))
      {
        this.mResolutionScrollBar.Grabbed = true;
      }
      else
      {
        if (iNewState.LeftButton != ButtonState.Released || iOldState.LeftButton != ButtonState.Pressed)
          return;
        if (this.mResolutionScrollBar.InsideUpBounds(iNewState))
          --this.mResolutionScrollBar.Value;
        else if (this.mResolutionScrollBar.InsideDownBounds(iNewState))
          ++this.mResolutionScrollBar.Value;
        else
          this.mResolutionScrollBar.ScrollTo((float) iNewState.Y);
      }
    }
    else if (iNewState.ScrollWheelValue > iOldState.ScrollWheelValue)
      --this.mResolutionScrollBar.Value;
    else if (iNewState.ScrollWheelValue < iOldState.ScrollWheelValue)
    {
      ++this.mResolutionScrollBar.Value;
    }
    else
    {
      if (iNewState.LeftButton != ButtonState.Released || iOldState.LeftButton != ButtonState.Pressed)
        return;
      for (int index = this.mResolutionScrollBar.Value; index < Math.Min(this.mResolutionScrollBar.Value + 12, this.mResolutions.Count); ++index)
      {
        MenuItem key = (MenuItem) this.mResolutions.Values[index].Key;
        if (key.Enabled & key.InsideBounds(iNewState))
        {
          this.OnSelect((Controller) ControlManager.Instance.MenuController);
          break;
        }
      }
    }
  }

  public override void Draw(float iDeltaTime)
  {
    base.Draw(iDeltaTime);
    float lineHeight = (float) this.mFont.LineHeight;
    Vector2 vector2 = new Vector2(this.mCenter.X, this.mCenter.Y - (float) ((double) lineHeight * 12.0 * 0.5));
    vector2.Y += lineHeight * 0.5f;
    for (int index = this.mResolutionScrollBar.Value; index < Math.Min(this.mResolutionScrollBar.Value + 12, this.mResolutions.Count); ++index)
    {
      MenuTextItem key = this.mResolutions.Values[index].Key;
      key.Color = new Vector4(Defines.MESSAGEBOX_COLOR_DEFAULT.X, Defines.MESSAGEBOX_COLOR_DEFAULT.Y, Defines.MESSAGEBOX_COLOR_DEFAULT.Z, this.mAlpha);
      key.Position = vector2;
      key.Draw(MessageBox.sGUIBasicEffect);
      vector2.Y += lineHeight;
      key.Selected = this.mCurrentResolution == index;
    }
    this.mResolutionScrollBar.Color = new Vector4(1f, 1f, 1f, this.mAlpha);
    this.mResolutionScrollBar.Draw(MessageBox.sGUIBasicEffect);
    this.mBackItem.Alpha = this.mAlpha;
    this.mBackItem.Draw(MessageBox.sGUIBasicEffect);
    MessageBox.sGUIBasicEffect.CurrentTechnique.Passes[0].End();
    MessageBox.sGUIBasicEffect.End();
  }

  public void NotifyResolutionChanged(ResolutionData iData)
  {
    if (this.Complete == null)
      return;
    this.Complete(iData);
  }
}
