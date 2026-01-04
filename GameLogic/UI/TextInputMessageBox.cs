// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.TextInputMessageBox
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using System;
using System.Text;

#nullable disable
namespace Magicka.GameLogic.UI;

internal class TextInputMessageBox : MessageBox
{
  private const string DEFAULT_DATE_FORMAT = "MM/DD/YYYY";
  private const int DATE_LENGTH = 10;
  private const float BUTTON_SPREAD = 50f;
  private const float BUTTON_OFFSET = 128f;
  private const int MIN_LINES_TXT = 1;
  private const int MIN_LINES_BTN = 3;
  private const float MAX_WIDTH_PERCENT = 0.8f;
  private MenuTextItem mOkButton;
  private MenuTextItem mCancelButton;
  private PolygonHead.Text mText;
  private PolygonHead.Text mBlinkerText;
  private StringBuilder mString;
  private float mTimer;
  private bool mLine;
  private Action<string> mComplete;
  private string mDaisyInputDescr = "";
  private bool mDaisyInputIsPsw;
  private bool mSecureMode;
  private TextInputMessageBox.MessageBoxInputType mInputType;
  private int mCharacterIndex;
  private int mMaxLength;
  private int mMessageHash;

  public bool SecureMode
  {
    set => this.mSecureMode = value;
  }

  public TextInputMessageBox.MessageBoxInputType InputType
  {
    set => this.mInputType = value;
  }

  public TextInputMessageBox(string iMessage, int iMaxLength)
    : base(iMessage)
  {
    this.mMessageHash = 0;
    this.mString = new StringBuilder(iMaxLength, iMaxLength);
    this.mText = new PolygonHead.Text(iMaxLength + 2, this.mFont, TextAlign.Left, true, false);
    this.mBlinkerText = new PolygonHead.Text(iMaxLength + 2, this.mFont, TextAlign.Left, true, false);
    string iText = LanguageManager.Instance.GetString(Defines.LOC_GEN_OK);
    this.mMaxLength = iMaxLength;
    Vector2 vector2 = this.mFont.MeasureText(iText, true);
    Vector2 iPosition;
    iPosition.X = this.mCenter.X - 50f;
    iPosition.Y = this.mCenter.Y + 128f - vector2.Y;
    this.mOkButton = new MenuTextItem(Defines.LOC_GEN_OK, iPosition, this.mFont, TextAlign.Right);
    this.mOkButton.ColorSelected = Vector4.One;
    this.mOkButton.Color = MenuItem.COLOR;
    LanguageManager.Instance.GetString(Defines.LOC_GEN_CANCEL);
    iPosition.X = this.mCenter.X + 50f;
    this.mCancelButton = new MenuTextItem(Defines.LOC_GEN_CANCEL, iPosition, this.mFont, TextAlign.Left);
    this.mCancelButton.ColorSelected = Vector4.One;
    this.mCancelButton.Color = MenuItem.COLOR;
    LanguageManager.Instance.LanguageChanged += new Action(((MessageBox) this).LanguageChanged);
    ResolutionMessageBox.Instance.Complete += new Action<ResolutionData>(this.ChangeResolution);
  }

  public TextInputMessageBox(int iMessage, int iMaxLength)
    : this(LanguageManager.Instance.GetString(iMessage), iMaxLength)
  {
    this.mMessageHash = iMessage;
    LanguageManager.Instance.LanguageChanged += new Action(((MessageBox) this).LanguageChanged);
  }

  private void ChangeResolution(ResolutionData iData)
  {
    Vector2 vector2 = this.mFont.MeasureText(LanguageManager.Instance.GetString(Defines.LOC_GEN_OK), true);
    this.mCenter = new Vector2((float) iData.Width, (float) iData.Height) * 0.5f;
    this.mOkButton.Position = this.mCenter + new Vector2(-50f, 128f - vector2.Y);
    this.mCancelButton.Position = this.mCenter + new Vector2(50f, 128f - vector2.Y);
  }

  public override void LanguageChanged()
  {
    base.LanguageChanged();
    this.mOkButton.LanguageChanged();
    this.mCancelButton.LanguageChanged();
    if (this.mMessageHash == 0)
      return;
    this.mMessage.SetText(LanguageManager.Instance.GetString(this.mMessageHash));
  }

  public void Show(Action<string> iCallback) => this.Show(iCallback, (Controller) null);

  public void Show(Action<string> iCallback, Controller iSender)
  {
    this.Show(iCallback, iSender, "");
  }

  public void Show(Action<string> iCallback, Controller iSender, string iDescr)
  {
    this.Show(iCallback, iSender, iDescr, false);
  }

  public void Show(Action<string> iCallback, Controller iSender, string iDescr, bool isPassword)
  {
    this.mComplete = iCallback;
    base.Show();
    this.mString.Length = 0;
    if (this.mInputType == TextInputMessageBox.MessageBoxInputType.Date)
    {
      this.mCharacterIndex = 0;
      this.mString.Append("MM/DD/YYYY");
      this.mText.SetText("MM/DD/YYYY");
    }
    else
      this.mText.SetText((string) null);
    if (!(iSender is XInputController))
      return;
    DaisyWheel.SetActionToCallWhenComplete(new Action<string>(this._DaisyWheelTextRecived));
    if (DaisyWheel.TryShow(iSender, iDescr, isPassword))
      return;
    DaisyWheel.SetActionToCallWhenComplete((Action<string>) null);
  }

  private void _DaisyWheelTextRecived(string enteredText)
  {
    DaisyWheel.SetActionToCallWhenComplete((Action<string>) null);
    if (enteredText == null)
      return;
    this.mString = new StringBuilder(enteredText);
    this.mText.SetText(enteredText);
    if (this.mComplete != null)
      this.mComplete(enteredText);
    this.Kill();
  }

  public override void Show() => throw new InvalidOperationException();

  public override void OnMove(Controller iSender, ControllerDirection iDirection)
  {
    if (DaisyWheel.IsDisplaying)
      return;
    if (iDirection == ControllerDirection.Left)
      this.HandleBackspace();
    this.mText.SetText(this.mSecureMode ? this.GetSecureStr(this.mString.ToString()) : this.mString.ToString());
  }

  public override void OnMouseMove(MouseState iNewState, MouseState iOldState)
  {
    if (DaisyWheel.IsDisplaying)
      return;
    this.mCancelButton.Selected = false;
    this.mOkButton.Selected = false;
    if (this.mOkButton.InsideBounds((float) iNewState.X, (float) iNewState.Y))
    {
      this.mOkButton.Selected = true;
    }
    else
    {
      if (!this.mCancelButton.InsideBounds((float) iNewState.X, (float) iNewState.Y))
        return;
      this.mCancelButton.Selected = true;
    }
  }

  public override void OnMouseClick(MouseState iNewState, MouseState iOldState)
  {
    if (DaisyWheel.IsDisplaying || iNewState.LeftButton != ButtonState.Released)
      return;
    if (this.mOkButton.InsideBounds((float) iNewState.X, (float) iNewState.Y))
    {
      this.OnSelect((Controller) ControlManager.Instance.MenuController);
    }
    else
    {
      if (!this.mCancelButton.InsideBounds((float) iNewState.X, (float) iNewState.Y))
        return;
      if (this.mComplete != null)
        this.mComplete("");
      this.Kill();
    }
  }

  public override void OnTextInput(char iChar)
  {
    if (DaisyWheel.IsDisplaying)
      return;
    bool flag = this.mInputType == TextInputMessageBox.MessageBoxInputType.Date;
    if (iChar == '\b')
    {
      this.HandleBackspace();
    }
    else
    {
      if ((!flag || this.mCharacterIndex >= "MM/DD/YYYY".Length) && (flag || this.mString.Length >= this.mString.Capacity))
        return;
      this.HandleNewCharacter(iChar);
    }
  }

  private string WrapText()
  {
    int num1 = (int) ((double) this.mSize.X * 0.800000011920929);
    string iStr = this.mString.ToString();
    string iText = this.mSecureMode ? this.GetSecureStr(iStr) : iStr;
    if ((double) this.mFont.MeasureText(iText, true).X > (double) num1)
    {
      string empty = string.Empty;
      float num2 = 0.0f;
      int index = 0;
      do
      {
        char ch = iText[index];
        float x = this.mFont.MeasureText(iText, true, 1).X;
        if ((double) num2 + (double) x >= (double) num1)
        {
          empty += "\n";
          num2 = 0.0f;
        }
        empty += (string) (object) ch;
        num2 += x;
        ++index;
      }
      while (index != iText.Length);
      iText = empty;
    }
    return iText;
  }

  private void HandleNewCharacter(char iChar)
  {
    string empty = string.Empty;
    if (this.mInputType == TextInputMessageBox.MessageBoxInputType.Standard || this.mInputType == TextInputMessageBox.MessageBoxInputType.NumericOnly && char.IsNumber(iChar))
      this.mString.Append(iChar);
    else if (this.mInputType == TextInputMessageBox.MessageBoxInputType.Date && char.IsNumber(iChar))
    {
      this.mCharacterIndex = Math.Max(0, this.mCharacterIndex);
      this.mString[this.mCharacterIndex] = iChar;
      ++this.mCharacterIndex;
      if (this.mCharacterIndex < "MM/DD/YYYY".Length - 1 && "MM/DD/YYYY"[this.mCharacterIndex] == '/')
        ++this.mCharacterIndex;
    }
    this.mText.SetText(this.mSecureMode ? this.GetSecureStr(this.mString.ToString()) : this.mString.ToString());
  }

  private void HandleBackspace()
  {
    bool flag = this.mInputType == TextInputMessageBox.MessageBoxInputType.Date;
    if (!flag && this.mString.Length > 0)
      --this.mString.Length;
    else if (flag && this.mCharacterIndex >= 0)
    {
      if (this.mCharacterIndex == 10)
      {
        this.mCharacterIndex = 9;
        this.mString[this.mCharacterIndex] = "MM/DD/YYYY"[this.mCharacterIndex];
      }
      else
      {
        --this.mCharacterIndex;
        if (this.mCharacterIndex > 0 && "MM/DD/YYYY"[this.mCharacterIndex] == '/')
          --this.mCharacterIndex;
        if (this.mCharacterIndex >= 0)
          this.mString[this.mCharacterIndex] = "MM/DD/YYYY"[this.mCharacterIndex];
      }
    }
    this.mText.SetText(this.mSecureMode ? this.GetSecureStr(this.mString.ToString()) : this.mString.ToString());
  }

  private string GetSecureStr(string iStr) => new string('*', iStr.Length);

  public override void OnSelect(Controller iSender)
  {
    if ((double) this.mAlpha < 1.0 | this.mDead || DaisyWheel.IsDisplaying)
      return;
    if (this.mComplete != null)
      this.mComplete(this.mString.ToString());
    this.Kill();
  }

  public override void Draw(float iDeltaTime)
  {
    if (DaisyWheel.IsDisplaying)
      return;
    Matrix iTransform = new Matrix();
    Vector4 vector4 = new Vector4();
    Point screenSize = RenderManager.Instance.ScreenSize;
    this.mCenter.X = (float) screenSize.X * 0.5f;
    this.mCenter.Y = (float) screenSize.Y * 0.5f;
    MessageBox.sGUIBasicEffect.SetScreenSize(screenSize.X, screenSize.Y);
    if (this.mDead)
      this.mAlpha = Math.Max(this.mAlpha - iDeltaTime * 4f, 0.0f);
    else
      this.mAlpha = Math.Min(this.mAlpha + iDeltaTime * 4f, 1f);
    iTransform.M12 = 1f;
    iTransform.M21 = -1f;
    iTransform.M33 = 1f;
    iTransform.M44 = 1f;
    iTransform.M41 = this.mCenter.X;
    iTransform.M42 = this.mCenter.Y;
    MessageBox.sGUIBasicEffect.Transform = iTransform;
    vector4.X = vector4.Y = vector4.Z = 1f;
    vector4.W = this.mAlpha;
    MessageBox.sGUIBasicEffect.Color = vector4;
    MessageBox.sGUIBasicEffect.VertexColorEnabled = false;
    MessageBox.sGUIBasicEffect.Texture = (Texture) MessageBox.sTexture;
    MessageBox.sGUIBasicEffect.TextureEnabled = true;
    MessageBox.sGUIBasicEffect.GraphicsDevice.Vertices[0].SetSource(MessageBox.sVertexBuffer, 0, 16 /*0x10*/);
    MessageBox.sGUIBasicEffect.GraphicsDevice.VertexDeclaration = MessageBox.sVertexDeclaration;
    MessageBox.sGUIBasicEffect.Begin();
    MessageBox.sGUIBasicEffect.CurrentTechnique.Passes[0].Begin();
    MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
    Vector2 mCenter = this.mCenter;
    this.mTimer -= iDeltaTime;
    while ((double) this.mTimer < 0.0)
    {
      this.mTimer += 0.5f;
      this.mLine = !this.mLine;
      if (this.mLine)
        this.mBlinkerText.SetText("_\0");
      else
        this.mBlinkerText.SetText("\0");
      this.mText.MarkAsDirty();
    }
    vector4.X = vector4.Y = vector4.Z = 1f;
    vector4.W = this.mAlpha;
    MessageBox.sGUIBasicEffect.Color = vector4;
    string str = this.WrapText();
    string[] strArray = str.Split('\n');
    int length = strArray.Length;
    float num1 = (float) this.mFont.LineHeight * 0.5f;
    MessageBox.sGUIBasicEffect.VertexColorEnabled = true;
    float num2 = this.mMessageHeight + (length > 1 ? (float) length * num1 : 0.0f);
    mCenter.Y -= num2;
    this.mMessage.Draw(MessageBox.sGUIBasicEffect, mCenter.X, mCenter.Y);
    mCenter.Y += num2;
    MessageBox.sGUIBasicEffect.VertexColorEnabled = false;
    this.mText.Font.MeasureText(this.mText.Characters, true, this.mString.Length);
    iTransform.M11 = 1f;
    iTransform.M12 = 0.0f;
    iTransform.M21 = 0.0f;
    iTransform.M22 = 1f;
    iTransform.M44 = 1f;
    float num3 = (mCenter.Y - (length > 1 ? (float) length * num1 : 0.0f)) * iTransform.M11;
    for (int index = 0; index < length; ++index)
    {
      this.mText.SetText(strArray[index]);
      float x = this.mText.Font.MeasureText(strArray[index], true, strArray[index].Length).X;
      iTransform.M41 = (float) Math.Floor((double) mCenter.X - 0.5 * (double) x * (double) iTransform.M11);
      iTransform.M42 = num3;
      this.mText.Draw(MessageBox.sGUIBasicEffect, ref iTransform);
      num3 += (float) this.mFont.LineHeight * iTransform.M11;
    }
    if (this.mInputType == TextInputMessageBox.MessageBoxInputType.Date)
    {
      if (this.mCharacterIndex < "MM/DD/YYYY".Length)
      {
        iTransform.M41 += this.mText.Font.MeasureText(this.mText.Characters, true, this.mCharacterIndex).X;
        this.mBlinkerText.Draw(MessageBox.sGUIBasicEffect, ref iTransform);
      }
    }
    else if (this.mString.Length < this.mString.Capacity)
    {
      if (length == 1)
      {
        iTransform.M41 += this.mText.Font.MeasureText(this.mText.Characters, true, this.mString.Length).X;
      }
      else
      {
        int num4 = str.LastIndexOf('\n');
        string iText = str.Substring(num4 + 1);
        float x = this.mText.Font.MeasureText(iText, true, iText.Length).X;
        iTransform.M41 += x;
      }
      this.mBlinkerText.Draw(MessageBox.sGUIBasicEffect, ref iTransform);
    }
    float num5 = length > 3 ? (float) (length - 3) * num1 : 0.0f;
    Vector2 vector2 = this.mFont.MeasureText(LanguageManager.Instance.GetString(Defines.LOC_GEN_OK), true);
    this.mOkButton.Position = this.mCenter + new Vector2(-50f, 128f - vector2.Y + num5);
    this.mCancelButton.Position = this.mCenter + new Vector2(50f, 128f - vector2.Y + num5);
    this.mOkButton.Alpha = this.mAlpha;
    this.mOkButton.Draw(MessageBox.sGUIBasicEffect);
    this.mCancelButton.Alpha = this.mAlpha;
    this.mCancelButton.Draw(MessageBox.sGUIBasicEffect);
    MessageBox.sGUIBasicEffect.CurrentTechnique.Passes[0].End();
    MessageBox.sGUIBasicEffect.End();
  }

  public override int ZIndex => 2002;

  public enum MessageBoxInputType
  {
    Standard,
    Date,
    NumericOnly,
  }
}
