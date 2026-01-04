// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Controls.InputMessageFilter
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#nullable disable
namespace Magicka.GameLogic.Controls;

internal class InputMessageFilter : IMessageFilter
{
  private KeyModifiers mModifier;

  [DllImport("user32.dll", EntryPoint = "TranslateMessage")]
  protected static extern bool _TranslateMessage(ref System.Windows.Forms.Message m);

  [DllImport("user32.dll")]
  public static extern uint MapVirtualKey(uint code, uint mapType);

  public event Action<KeyData> KeyDown;

  public event Action<char, KeyModifiers> KeyPress;

  public event Action<KeyData> KeyUp;

  public event Action<System.Windows.Forms.MouseButtons, Microsoft.Xna.Framework.Point> MouseMove;

  public event Action<System.Windows.Forms.MouseButtons, Microsoft.Xna.Framework.Point> MouseLeave;

  public event Action<System.Windows.Forms.MouseButtons, Microsoft.Xna.Framework.Point> MouseDown;

  public event Action<System.Windows.Forms.MouseButtons, Microsoft.Xna.Framework.Point> MouseUp;

  public event Action<int, Microsoft.Xna.Framework.Point> MouseScroll;

  public bool Enabled { get; set; }

  public bool TranslateMessage { get; set; }

  public InputMessageFilter()
  {
    Magicka.Game.Instance.Form.BeginInvoke((Delegate) (() => Application.AddMessageFilter((IMessageFilter) this)));
  }

  bool IMessageFilter.PreFilterMessage(ref System.Windows.Forms.Message m)
  {
    if (!Magicka.Game.Instance.Form.Focused)
      return false;
    switch ((InputMessageFilter.Wm) m.Msg)
    {
      case InputMessageFilter.Wm.Active:
        if (((int) m.WParam & (int) ushort.MaxValue) == 0)
        {
          this.mModifier = KeyModifiers.None;
          break;
        }
        break;
      case InputMessageFilter.Wm.KeyDown:
      case InputMessageFilter.Wm.SysKeyDown:
        if ((m.LParam.ToInt32() & 1073741824 /*0x40000000*/) == 0)
        {
          KeyData keyData;
          switch ((int) m.WParam)
          {
            case 16 /*0x10*/:
              if ((m.LParam.ToInt32() & 16777216 /*0x01000000*/) == 0)
              {
                keyData = new KeyData(Microsoft.Xna.Framework.Input.Keys.LeftShift, this.mModifier);
                this.mModifier |= KeyModifiers.LeftShift;
                break;
              }
              keyData = new KeyData(Microsoft.Xna.Framework.Input.Keys.RightShift, this.mModifier);
              this.mModifier |= KeyModifiers.RightShift;
              break;
            case 17:
              if ((m.LParam.ToInt32() & 16777216 /*0x01000000*/) == 0)
              {
                keyData = new KeyData(Microsoft.Xna.Framework.Input.Keys.LeftControl, this.mModifier);
                this.mModifier |= KeyModifiers.LeftControl;
                break;
              }
              keyData = new KeyData(Microsoft.Xna.Framework.Input.Keys.RightControl, this.mModifier);
              this.mModifier |= KeyModifiers.RightControl;
              break;
            case 18:
              if ((m.LParam.ToInt32() & 16777216 /*0x01000000*/) == 0)
              {
                keyData = new KeyData(Microsoft.Xna.Framework.Input.Keys.LeftAlt, this.mModifier);
                this.mModifier |= KeyModifiers.LeftAlt;
                break;
              }
              keyData = new KeyData(Microsoft.Xna.Framework.Input.Keys.RightAlt, this.mModifier);
              this.mModifier |= KeyModifiers.RightAlt;
              break;
            default:
              keyData = new KeyData((Microsoft.Xna.Framework.Input.Keys) (int) m.WParam, this.mModifier);
              break;
          }
          if (this.KeyDown != null)
            this.KeyDown(keyData);
          if (keyData.Key == Microsoft.Xna.Framework.Input.Keys.F4 & (keyData.Modifier & KeyModifiers.Alt) != KeyModifiers.None)
            return false;
        }
        if (this.TranslateMessage)
          InputMessageFilter._TranslateMessage(ref m);
        return true;
      case InputMessageFilter.Wm.KeyUp:
      case InputMessageFilter.Wm.SysKeyUp:
        KeyData keyData1;
        switch ((int) m.WParam)
        {
          case 16 /*0x10*/:
            if ((m.LParam.ToInt32() & 16777216 /*0x01000000*/) == 0)
            {
              this.mModifier &= ~KeyModifiers.LeftShift;
              keyData1 = new KeyData(Microsoft.Xna.Framework.Input.Keys.LeftShift, this.mModifier);
              break;
            }
            this.mModifier &= ~KeyModifiers.RightShift;
            keyData1 = new KeyData(Microsoft.Xna.Framework.Input.Keys.RightShift, this.mModifier);
            break;
          case 17:
            if ((m.LParam.ToInt32() & 16777216 /*0x01000000*/) == 0)
            {
              this.mModifier &= ~KeyModifiers.LeftControl;
              keyData1 = new KeyData(Microsoft.Xna.Framework.Input.Keys.LeftControl, this.mModifier);
              break;
            }
            this.mModifier &= ~KeyModifiers.RightControl;
            keyData1 = new KeyData(Microsoft.Xna.Framework.Input.Keys.RightControl, this.mModifier);
            break;
          case 18:
            if ((m.LParam.ToInt32() & 16777216 /*0x01000000*/) == 0)
            {
              this.mModifier &= ~KeyModifiers.LeftAlt;
              keyData1 = new KeyData(Microsoft.Xna.Framework.Input.Keys.LeftAlt, this.mModifier);
              break;
            }
            this.mModifier &= ~KeyModifiers.RightAlt;
            keyData1 = new KeyData(Microsoft.Xna.Framework.Input.Keys.RightAlt, this.mModifier);
            break;
          default:
            keyData1 = new KeyData((Microsoft.Xna.Framework.Input.Keys) (int) m.WParam, this.mModifier);
            break;
        }
        if (this.KeyUp != null)
          this.KeyUp(keyData1);
        return true;
      case InputMessageFilter.Wm.Char:
        char ch = (char) (int) m.WParam;
        if (ch >= ' ' || ch == '\n' || ch == '\r' || ch == '\b')
        {
          if (ch == '\r')
            ch = '\n';
          if (this.KeyPress != null)
            this.KeyPress(ch, this.mModifier);
          return true;
        }
        break;
      case InputMessageFilter.Wm.MouseMove:
        if (this.MouseMove != null)
        {
          this.MouseMove(this.GetButtons(m.WParam.ToInt32()), this.GetMousePosition(m.LParam.ToInt32()));
          break;
        }
        break;
      case InputMessageFilter.Wm.LeftButtonDown:
        if (this.MouseDown != null)
        {
          this.MouseDown(System.Windows.Forms.MouseButtons.Left, this.GetMousePosition(m.LParam.ToInt32()));
          break;
        }
        break;
      case InputMessageFilter.Wm.LeftButtonUp:
        if (this.MouseUp != null)
        {
          this.MouseUp(System.Windows.Forms.MouseButtons.Left, this.GetMousePosition(m.LParam.ToInt32()));
          break;
        }
        break;
      case InputMessageFilter.Wm.RightButtonDown:
        if (this.MouseDown != null)
        {
          this.MouseDown(System.Windows.Forms.MouseButtons.Right, this.GetMousePosition(m.LParam.ToInt32()));
          break;
        }
        break;
      case InputMessageFilter.Wm.RightButtonUp:
        if (this.MouseUp != null)
        {
          this.MouseUp(System.Windows.Forms.MouseButtons.Right, this.GetMousePosition(m.LParam.ToInt32()));
          break;
        }
        break;
      case InputMessageFilter.Wm.MiddleButtonDown:
        if (this.MouseDown != null)
        {
          this.MouseDown(System.Windows.Forms.MouseButtons.Middle, this.GetMousePosition(m.LParam.ToInt32()));
          break;
        }
        break;
      case InputMessageFilter.Wm.MiddleButtonUp:
        if (this.MouseUp != null)
        {
          this.MouseUp(System.Windows.Forms.MouseButtons.Middle, this.GetMousePosition(m.LParam.ToInt32()));
          break;
        }
        break;
      case InputMessageFilter.Wm.MouseWheel:
        if (this.MouseScroll != null)
        {
          Microsoft.Xna.Framework.Point mousePosition = this.GetMousePosition(m.LParam.ToInt32());
          System.Drawing.Point client = Magicka.Game.Instance.Form.PointToClient(new System.Drawing.Point(mousePosition.X, mousePosition.Y));
          Microsoft.Xna.Framework.Point point = new Microsoft.Xna.Framework.Point(client.X, client.Y);
          this.MouseScroll((int) this.GetWheelDelta(m.WParam.ToInt32()), point);
          break;
        }
        break;
      case InputMessageFilter.Wm.MouseLeave:
        if (this.MouseLeave != null)
        {
          this.MouseLeave(this.GetButtons(m.WParam.ToInt32()), this.GetMousePosition(m.LParam.ToInt32()));
          break;
        }
        break;
    }
    return false;
  }

  private System.Windows.Forms.MouseButtons GetButtons(int iWParam)
  {
    System.Windows.Forms.MouseButtons buttons = System.Windows.Forms.MouseButtons.None;
    if ((iWParam & 1) != 0)
      buttons |= System.Windows.Forms.MouseButtons.Left;
    if ((iWParam & 16 /*0x10*/) != 0)
      buttons |= System.Windows.Forms.MouseButtons.Middle;
    if ((iWParam & 2) != 0)
      buttons |= System.Windows.Forms.MouseButtons.Right;
    if ((iWParam & 32 /*0x20*/) != 0)
      buttons |= System.Windows.Forms.MouseButtons.XButton1;
    if ((iWParam & 64 /*0x40*/) != 0)
      buttons |= System.Windows.Forms.MouseButtons.XButton2;
    return buttons;
  }

  private Microsoft.Xna.Framework.Point GetMousePosition(int iLParam)
  {
    Microsoft.Xna.Framework.Point mousePosition;
    mousePosition.X = (int) (ushort) iLParam;
    mousePosition.Y = (int) (ushort) (iLParam >> 16 /*0x10*/);
    return mousePosition;
  }

  private short GetWheelDelta(int iWParam) => (short) (iWParam >> 16 /*0x10*/);

  public class Stack
  {
    private KeyData[] mStack;

    public int Capacity => this.mStack.Length;

    public int Count { get; private set; }

    public Stack()
      : this(32 /*0x20*/)
    {
    }

    public Stack(int capacity)
    {
      if (capacity < 0)
        capacity = 0;
      this.mStack = new KeyData[capacity];
    }

    public void Push(ref KeyData item)
    {
      if (this.Count == this.mStack.Length)
      {
        KeyData[] destinationArray = new KeyData[this.mStack.Length << 1];
        Array.Copy((Array) this.mStack, 0, (Array) destinationArray, 0, this.mStack.Length);
        this.mStack = destinationArray;
      }
      this.mStack[this.Count] = item;
      ++this.Count;
    }

    public void Pop(out KeyData item)
    {
      item = this.Count > 0 ? this.mStack[this.Count] : throw new InvalidOperationException("The stack is empty!");
      --this.Count;
    }

    public void PopSegment(out ArraySegment<KeyData> segment)
    {
      segment = new ArraySegment<KeyData>(this.mStack, 0, this.Count);
      this.Count = 0;
    }
  }

  protected enum Wm
  {
    Active = 6,
    KeyDown = 256, // 0x00000100
    KeyUp = 257, // 0x00000101
    Char = 258, // 0x00000102
    SysKeyDown = 260, // 0x00000104
    SysKeyUp = 261, // 0x00000105
    MouseMove = 512, // 0x00000200
    LeftButtonDown = 513, // 0x00000201
    LeftButtonUp = 514, // 0x00000202
    RightButtonDown = 516, // 0x00000204
    RightButtonUp = 517, // 0x00000205
    MiddleButtonDown = 519, // 0x00000207
    MiddleButtonUp = 520, // 0x00000208
    MouseWheel = 522, // 0x0000020A
    MouseLeave = 675, // 0x000002A3
  }

  protected enum Wa
  {
    Inactive,
    Active,
    ClickActive,
  }

  protected enum Vk
  {
    Shift = 16, // 0x00000010
    Control = 17, // 0x00000011
    Alt = 18, // 0x00000012
  }
}
