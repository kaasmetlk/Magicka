// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.InGameMenus.InGameMenuOptionsResolution
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace Magicka.GameLogic.GameStates.InGameMenus;

internal class InGameMenuOptionsResolution : InGameMenu
{
  private const string OPTION_BACK = "back";
  private const int VISIBLE_ITEMS = 12;
  private static InGameMenuOptionsResolution sSingelton;
  private static volatile object sSingeltonLock = new object();
  private MenuScrollBar mScrollBar;
  private BitmapFont mFont;
  private SortedList<uint, KeyValuePair<MenuTextItem, List<int>>> mResolutions;

  public static InGameMenuOptionsResolution Instance
  {
    get
    {
      if (InGameMenuOptionsResolution.sSingelton == null)
      {
        lock (InGameMenuOptionsResolution.sSingeltonLock)
        {
          if (InGameMenuOptionsResolution.sSingelton == null)
            InGameMenuOptionsResolution.sSingelton = new InGameMenuOptionsResolution();
        }
      }
      return InGameMenuOptionsResolution.sSingelton;
    }
  }

  private InGameMenuOptionsResolution()
  {
    this.mFont = FontManager.Instance.GetFont(MagickaFont.Maiandra16);
    this.mResolutions = new SortedList<uint, KeyValuePair<MenuTextItem, List<int>>>();
    this.mScrollBar = new MenuScrollBar(new Vector2(), (float) (this.mFont.LineHeight * 12), 0);
    this.mScrollBar.TextureOffset = new Vector2(-384f, 224f);
    this.mBackgroundSize = new Vector2(400f, 500f);
    Vector2 iPosition = new Vector2(InGameMenu.sScreenSize.X * 0.5f, (float) ((double) InGameMenu.sScreenSize.Y * 0.5 + (double) this.mBackgroundSize.Y * 0.5 - 64.0));
    this.mMenuItems.Add((MenuItem) new MenuTextItem("#menu_back".GetHashCodeCustom(), iPosition, this.mFont, TextAlign.Center));
  }

  public override void LanguageChanged()
  {
    base.LanguageChanged();
    if (this.mResolutions == null || this.mResolutions.Count <= 0)
      return;
    this.mResolutions.Last<KeyValuePair<uint, KeyValuePair<MenuTextItem, List<int>>>>().Value.Key.LanguageChanged();
  }

  public override void UpdatePositions()
  {
    for (int index = 0; index < this.mResolutions.Count; ++index)
      this.mResolutions.Values[index].Key.Scale = InGameMenu.sScale;
    this.mMenuItems[0].Position = new Vector2(InGameMenu.sScreenSize.X * 0.5f, (float) ((double) InGameMenu.sScreenSize.Y * 0.5 + ((double) this.mBackgroundSize.Y * 0.5 - 64.0) * (double) InGameMenu.sScale));
    this.mMenuItems[0].Scale = InGameMenu.sScale;
    this.mScrollBar.Position = new Vector2((float) ((double) InGameMenu.sScreenSize.X * 0.5 + 120.0 * (double) InGameMenu.sScale), InGameMenu.sScreenSize.Y * 0.5f);
    this.mScrollBar.Scale = InGameMenu.sScale;
  }

  protected override string IGetHighlightedButtonName()
  {
    return this.mSelectedItem != this.mResolutions.Count ? this.mResolutions.Values[this.mSelectedItem].Key.Name : "back";
  }

  protected override void IControllerSelect(Controller iSender)
  {
    if (this.mSelectedItem == this.mResolutions.Count)
    {
      AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_DECREASE);
      InGameMenu.PopMenu();
    }
    else
    {
      if (this.mSelectedItem < 0)
        return;
      AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
      ResolutionData iData = new ResolutionData();
      uint key = this.mResolutions.Keys[this.mSelectedItem];
      iData.Width = (int) ushort.MaxValue & (int) (key >> 16 /*0x10*/);
      iData.Height = (int) ushort.MaxValue & (int) key;
      GlobalSettings.Instance.Resolution = iData;
      ResolutionMessageBox.Instance.NotifyResolutionChanged(iData);
      InGameMenu.PopMenu();
    }
  }

  protected override void IControllerBack(Controller iSender)
  {
    AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_DECREASE);
    InGameMenu.PopMenu();
  }

  protected override void IControllerMove(Controller iSender, ControllerDirection iDirection)
  {
    switch (iDirection)
    {
      case ControllerDirection.Right:
        if (this.mSelectedItem < this.mResolutions.Count)
        {
          this.mSelectedItem = this.mResolutions.Count;
          break;
        }
        this.mSelectedItem = 0;
        break;
      case ControllerDirection.Up:
        int mSelectedItem1 = this.mSelectedItem;
        do
        {
          --mSelectedItem1;
          if (mSelectedItem1 < 0)
            mSelectedItem1 += this.mResolutions.Count;
        }
        while (!this.mResolutions.Values[mSelectedItem1].Key.Enabled);
        this.mSelectedItem = mSelectedItem1;
        AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_MOVE);
        break;
      case ControllerDirection.Left:
        if (this.mSelectedItem < this.mResolutions.Count)
        {
          this.mSelectedItem = this.mResolutions.Count;
          break;
        }
        this.mSelectedItem = 0;
        break;
      case ControllerDirection.Down:
        int mSelectedItem2 = this.mSelectedItem;
        do
        {
          ++mSelectedItem2;
          if (mSelectedItem2 >= this.mResolutions.Count)
            mSelectedItem2 -= this.mResolutions.Count;
        }
        while (!this.mResolutions.Values[mSelectedItem2].Key.Enabled);
        this.mSelectedItem = mSelectedItem2;
        AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_MOVE);
        break;
    }
    while (this.mSelectedItem < this.mScrollBar.Value)
      --this.mScrollBar.Value;
    while (this.mSelectedItem >= this.mScrollBar.Value + 12 && this.mSelectedItem < this.mResolutions.Count)
      ++this.mScrollBar.Value;
  }

  protected override void IMouseMove(Controller iSender, ref Vector2 iMousePosition)
  {
    if (this.mScrollBar.Grabbed)
    {
      this.mSelectedItem = -1;
      if (this.mScrollBar.InsideDragUpBounds(iMousePosition))
      {
        --this.mScrollBar.Value;
      }
      else
      {
        if (!this.mScrollBar.InsideDragDownBounds(iMousePosition))
          return;
        ++this.mScrollBar.Value;
      }
    }
    else
    {
      int num = -1;
      for (int index = 0; index < this.mResolutions.Count; ++index)
      {
        MenuTextItem key = this.mResolutions.Values[index].Key;
        if (key.Enabled && key.InsideBounds(ref iMousePosition))
        {
          num = index;
          break;
        }
      }
      if (num == -1 && this.mMenuItems[0].InsideBounds(ref iMousePosition))
        num = this.mResolutions.Count;
      if (this.mSelectedItem != num & num >= 0)
        AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_MOVE);
      this.mSelectedItem = num;
    }
  }

  protected override void IMouseScroll(Controller iSender, ref Vector2 iMousePos, int iValue)
  {
    if (this.mScrollBar.InsideBounds(ref iMousePos))
    {
      if (iValue > 0)
      {
        --this.mScrollBar.Value;
      }
      else
      {
        if (iValue >= 0)
          return;
        ++this.mScrollBar.Value;
      }
    }
    else
    {
      for (int index = 0; index < this.mResolutions.Count; ++index)
      {
        MenuTextItem key = this.mResolutions.Values[index].Key;
        if (key.Enabled && key.InsideBounds(ref iMousePos))
        {
          if (iValue > 0)
          {
            --this.mScrollBar.Value;
            break;
          }
          if (iValue >= 0)
            break;
          ++this.mScrollBar.Value;
          break;
        }
      }
    }
  }

  protected override void IMouseDown(Controller iSender, ref Vector2 iMousePosition)
  {
    base.IMouseDown(iSender, ref iMousePosition);
    if (this.mScrollBar.InsideDragBounds(iMousePosition))
    {
      this.mScrollBar.Grabbed = true;
    }
    else
    {
      if (this.mScrollBar.InsideUpBounds(iMousePosition) || this.mScrollBar.InsideDownBounds(iMousePosition) || !this.mScrollBar.InsideBounds(ref iMousePosition))
        return;
      this.mScrollBar.ScrollTo(iMousePosition.Y);
    }
  }

  protected override void IMouseUp(Controller iSender, ref Vector2 iMousePosition)
  {
    base.IMouseUp(iSender, ref iMousePosition);
    if (!this.mScrollBar.Grabbed)
    {
      if (this.mScrollBar.InsideUpBounds(iMousePosition))
        --this.mScrollBar.Value;
      else if (this.mScrollBar.InsideDownBounds(iMousePosition))
        ++this.mScrollBar.Value;
    }
    this.mScrollBar.Grabbed = false;
  }

  protected override void OnEnter()
  {
    if (InGameMenu.sController is KeyboardMouseController)
      this.mSelectedItem = -1;
    else
      this.mSelectedItem = 0;
    this.mScrollBar.Value = 0;
    this.mResolutions.Clear();
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
    this.mScrollBar.SetMaxValue(this.mResolutions.Count - 12);
    this.UpdatePositions();
  }

  protected override void IDraw(float iDeltaTime, ref Vector2 iBackgroundSize)
  {
    Vector4 vector4_1 = new Vector4();
    vector4_1.X = vector4_1.Y = vector4_1.Z = 1f;
    vector4_1.W = this.mAlpha;
    Vector4 vector4_2 = new Vector4();
    vector4_2.X = vector4_2.Y = vector4_2.Z = 0.0f;
    vector4_2.W = this.mAlpha;
    Vector4 vector4_3 = new Vector4();
    vector4_3.X = vector4_3.Y = vector4_3.Z = 0.4f;
    vector4_3.W = this.mAlpha;
    float lineHeight = (float) this.mFont.LineHeight;
    Vector2 vector2 = new Vector2();
    vector2.X = InGameMenu.sScreenSize.X * 0.5f;
    vector2.Y = (float) ((double) InGameMenu.sScreenSize.Y * 0.5 - (double) lineHeight * 11.0 * 0.5 * (double) InGameMenu.sScale);
    for (int index = this.mScrollBar.Value; index < Math.Min(this.mScrollBar.Value + 12, this.mResolutions.Count); ++index)
    {
      MenuItem key = (MenuItem) this.mResolutions.Values[index].Key;
      key.Position = vector2;
      key.Color = vector4_1;
      key.ColorSelected = vector4_2;
      key.ColorDisabled = vector4_3;
      key.Selected = key.Enabled & this.mSelectedItem == index;
      if (key.Selected)
      {
        InGameMenu.sEffect.Transform = new Matrix()
        {
          M44 = 1f,
          M11 = iBackgroundSize.X * InGameMenu.sScale,
          M22 = key.BottomRight.Y - key.TopLeft.Y,
          M41 = key.Position.X - iBackgroundSize.X * 0.5f * InGameMenu.sScale,
          M42 = key.TopLeft.Y
        };
        Vector4 vector4_4 = new Vector4();
        vector4_4.X = vector4_4.Y = vector4_4.Z = 1f;
        vector4_4.W = 0.8f * this.mAlpha;
        InGameMenu.sEffect.Color = vector4_4;
        InGameMenu.sEffect.GraphicsDevice.Vertices[0].SetSource(InGameMenu.sBackground, 0, 8);
        InGameMenu.sEffect.GraphicsDevice.VertexDeclaration = InGameMenu.sBackgroundDeclaration;
        InGameMenu.sEffect.CommitChanges();
        InGameMenu.sEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
      }
      vector2.Y += lineHeight * InGameMenu.sScale;
    }
    this.mMenuItems[0].Selected = this.mSelectedItem == this.mResolutions.Count;
    this.mMenuItems[0].Color = vector4_1;
    this.mMenuItems[0].ColorSelected = vector4_2;
    if (this.mMenuItems[0].Selected)
    {
      InGameMenu.sEffect.Transform = new Matrix()
      {
        M44 = 1f,
        M11 = iBackgroundSize.X * InGameMenu.sScale,
        M22 = this.mMenuItems[0].BottomRight.Y - this.mMenuItems[0].TopLeft.Y,
        M41 = this.mMenuItems[0].Position.X - iBackgroundSize.X * 0.5f * InGameMenu.sScale,
        M42 = this.mMenuItems[0].TopLeft.Y
      };
      Vector4 vector4_5 = new Vector4();
      vector4_5.X = vector4_5.Y = vector4_5.Z = 1f;
      vector4_5.W = 0.8f * this.mAlpha;
      InGameMenu.sEffect.Color = vector4_5;
      InGameMenu.sEffect.GraphicsDevice.Vertices[0].SetSource(InGameMenu.sBackground, 0, 8);
      InGameMenu.sEffect.GraphicsDevice.VertexDeclaration = InGameMenu.sBackgroundDeclaration;
      InGameMenu.sEffect.CommitChanges();
      InGameMenu.sEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
    }
    this.mScrollBar.Color = vector4_1;
    this.mScrollBar.Draw(InGameMenu.sEffect);
    for (int index = this.mScrollBar.Value; index < Math.Min(this.mScrollBar.Value + 12, this.mResolutions.Count); ++index)
      this.mResolutions.Values[index].Key.Draw(InGameMenu.sEffect);
    this.mMenuItems[0].Draw(InGameMenu.sEffect);
  }

  protected override void OnExit()
  {
    Point screenSize = RenderManager.Instance.ScreenSize;
    InGameMenu.sPlayState.Camera.AspectRation = (float) screenSize.X / (float) screenSize.Y;
    TutorialManager.Instance.UpdateResolution();
    if (BossFight.Instance.IsSetup && !BossFight.Instance.Dead)
      BossFight.Instance.UpdateResolution();
    DamageNotifyer.Instance.UpdateResolution();
    InGameMenu.UpdateAllPositions();
    SaveManager.Instance.SaveSettings();
  }
}
