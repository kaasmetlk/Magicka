// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.OptionsMessageBox
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using System;

#nullable disable
namespace Magicka.GameLogic.UI;

internal class OptionsMessageBox : MessageBox
{
  private bool mKeyboardSelection;
  private int mSelectedIndex;
  private MenuTextItem[] mOptions;
  private int mMessageHash;

  public event Action<OptionsMessageBox, int> Select;

  public OptionsMessageBox(string iMessage, params string[] iOptions)
    : base(iMessage)
  {
    this.mMessageHash = 0;
    this.mOptions = new MenuTextItem[iOptions.Length];
    for (int index = 0; index < iOptions.Length; ++index)
    {
      MenuTextItem menuTextItem = new MenuTextItem(iOptions[index], new Vector2(), this.mFont, TextAlign.Center);
      this.mOptions[index] = menuTextItem;
    }
    LanguageManager.Instance.LanguageChanged += new Action(((MessageBox) this).LanguageChanged);
  }

  public OptionsMessageBox(int iMessage, params int[] iOptions)
    : base(iMessage)
  {
    this.mMessageHash = iMessage;
    this.mOptions = new MenuTextItem[iOptions.Length];
    for (int index = 0; index < iOptions.Length; ++index)
    {
      MenuTextItem menuTextItem = new MenuTextItem(iOptions[index], new Vector2(), this.mFont, TextAlign.Center);
      this.mOptions[index] = menuTextItem;
    }
    LanguageManager.Instance.LanguageChanged += new Action(((MessageBox) this).LanguageChanged);
  }

  public override void LanguageChanged()
  {
    base.LanguageChanged();
    for (int index = 0; index < this.mOptions.Length; ++index)
      this.mOptions[index].LanguageChanged();
    if (this.mMessageHash == 0)
      return;
    this.mMessage.SetText(this.mFont.Wrap(LanguageManager.Instance.GetString(this.mMessageHash), (int) ((double) this.mSize.X * 0.89999997615814209), true));
  }

  public override void OnMove(Controller iSender, ControllerDirection iDirection)
  {
    if (!this.mKeyboardSelection)
    {
      this.SelectedIndex = 0;
      this.mKeyboardSelection = true;
    }
    else
    {
      switch (iDirection)
      {
        case ControllerDirection.Up:
          --this.SelectedIndex;
          break;
        case ControllerDirection.Down:
          ++this.SelectedIndex;
          break;
      }
    }
  }

  public override void OnMouseMove(MouseState iNewState, MouseState iOldState)
  {
    bool flag = false;
    for (int index1 = 0; index1 < this.mOptions.Length; ++index1)
    {
      MenuTextItem mOption = this.mOptions[index1];
      if (mOption.InsideBounds((float) iNewState.X, (float) iNewState.Y))
      {
        this.mKeyboardSelection = false;
        this.mSelectedIndex = index1;
        for (int index2 = 0; index2 < this.mOptions.Length; ++index2)
          this.mOptions[index2].Selected = false;
        mOption.Selected = true;
        flag = true;
        break;
      }
    }
    if (flag || this.mKeyboardSelection)
      return;
    for (int index = 0; index < this.mOptions.Length; ++index)
      this.mOptions[index].Selected = false;
  }

  public override void OnMouseClick(MouseState iNewState, MouseState iOldState)
  {
    if (iNewState.LeftButton == ButtonState.Pressed)
      return;
    bool flag = false;
    for (int index1 = 0; index1 < this.mOptions.Length; ++index1)
    {
      MenuTextItem mOption = this.mOptions[index1];
      if (mOption.InsideBounds((float) iNewState.X, (float) iNewState.Y))
      {
        this.mKeyboardSelection = false;
        this.mSelectedIndex = index1;
        for (int index2 = 0; index2 < this.mOptions.Length; ++index2)
          this.mOptions[index2].Selected = false;
        mOption.Selected = true;
        flag = true;
        this.OnSelect((Controller) ControlManager.Instance.MenuController);
        break;
      }
    }
    if (flag || this.mKeyboardSelection)
      return;
    for (int index = 0; index < this.mOptions.Length; ++index)
      this.mOptions[index].Selected = false;
  }

  public override void OnSelect(Controller iSender)
  {
    if (this.Select != null)
      this.Select(this, this.mSelectedIndex);
    this.Kill();
  }

  public override void OnTextInput(char iChar)
  {
  }

  public override void Draw(float iDeltaTime)
  {
    base.Draw(iDeltaTime);
    int mSelectedIndex = this.mSelectedIndex;
    Vector2 mCenter = this.mCenter;
    MessageBox.sGUIBasicEffect.Color = new Vector4(1f, 1f, 1f, this.mAlpha);
    MessageBox.sGUIBasicEffect.SetScreenSize(RenderManager.Instance.ScreenSize.X, RenderManager.Instance.ScreenSize.Y);
    float num = Math.Min(this.mMessageHeight, 460f);
    mCenter.Y -= (float) (0.5 * ((double) num + (double) (this.mFont.LineHeight * this.mOptions.Length)));
    Matrix iTransform = new Matrix();
    iTransform.M11 = iTransform.M33 = 1f;
    iTransform.M22 = num / this.mMessageHeight;
    iTransform.M44 = 1f;
    iTransform.M41 = mCenter.X;
    iTransform.M42 = mCenter.Y;
    MessageBox.sGUIBasicEffect.VertexColorEnabled = true;
    this.mMessage.Draw(MessageBox.sGUIBasicEffect, ref iTransform);
    MessageBox.sGUIBasicEffect.VertexColorEnabled = false;
    mCenter.Y += num + 0.5f * (float) this.mFont.LineHeight;
    if (this.mKeyboardSelection)
    {
      for (int index = 0; index < this.mOptions.Length; ++index)
      {
        this.mOptions[index].Selected = index == mSelectedIndex;
        this.mOptions[index].Position = mCenter;
        this.mOptions[index].Draw(MessageBox.sGUIBasicEffect);
        mCenter.Y += (float) this.mFont.LineHeight;
      }
    }
    else
    {
      for (int index = 0; index < this.mOptions.Length; ++index)
      {
        this.mOptions[index].Position = mCenter;
        this.mOptions[index].Alpha = this.mAlpha;
        this.mOptions[index].Draw(MessageBox.sGUIBasicEffect);
        mCenter.Y += (float) this.mFont.LineHeight;
      }
    }
    MessageBox.sGUIBasicEffect.CurrentTechnique.Passes[0].End();
    MessageBox.sGUIBasicEffect.End();
  }

  public int SelectedIndex
  {
    get => this.mSelectedIndex;
    set
    {
      if (value >= this.mOptions.Length)
        value -= this.mOptions.Length;
      if (value < 0)
        value += this.mOptions.Length;
      this.mSelectedIndex = value;
    }
  }

  internal void SetMessage(string p)
  {
    p = this.mFont.Wrap(p, 320, true);
    this.mMessage.SetText(p);
    this.mMessageHeight = this.mFont.MeasureText(p, true).Y;
  }

  public override int ZIndex => 2002;

  public override void Show() => base.Show();

  public void Show(int iMessage)
  {
    this.mMessage.SetText(LanguageManager.Instance.GetString(iMessage));
  }
}
