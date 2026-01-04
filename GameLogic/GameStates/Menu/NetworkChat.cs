// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.NetworkChat
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using Magicka.Graphics;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using SteamWrapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu;

internal class NetworkChat
{
  private static NetworkChat sSingelton;
  private static volatile object sSingeltonLock = new object();
  private GUIBasicEffect mEffect;
  private StringBuilder mLog;
  private StringBuilder mMessage;
  private List<float> mMessageTTLs = new List<float>(64 /*0x40*/);
  private PolygonHead.Text mTitle;
  private bool mTitleActive;
  private PolygonHead.Text mLogText;
  private PolygonHead.Text mMessageText;
  private BitmapFont mFont;
  private float mTimer;
  private bool mInputLine;
  private int mMargin;
  private MenuScrollBar mScrollBar;
  private Point mSize;
  private Texture2D mBackgroundTexture;
  private bool mScrollBarVisible;
  private Vector2 mBackgroundTextureOffset;
  private Vector2 mBackgroundTextureScale;
  private VertexBuffer mBackgroundVertices;
  private VertexDeclaration mBackgroundDeclaration;
  private int mScrollValue;
  private int mVisibleLines;
  private float mMessageTTL;
  private int mLineCount;
  private bool mActive;
  private bool mDirty;

  public static NetworkChat Instance
  {
    get
    {
      if (NetworkChat.sSingelton == null)
      {
        lock (NetworkChat.sSingeltonLock)
        {
          if (NetworkChat.sSingelton == null)
            NetworkChat.sSingelton = new NetworkChat();
        }
      }
      return NetworkChat.sSingelton;
    }
  }

  private NetworkChat()
  {
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    lock (graphicsDevice)
      this.mEffect = new GUIBasicEffect(graphicsDevice, (EffectPool) null);
    this.mFont = FontManager.Instance.GetFont(MagickaFont.MenuDefault);
    this.mTitle = new PolygonHead.Text(256 /*0x0100*/, FontManager.Instance.GetFont(MagickaFont.MenuDefault), TextAlign.Center, false, true);
    this.mTitle.DefaultColor = Vector4.One;
    this.mTitle.ShadowAlpha = 0.75f;
    this.mTitle.ShadowsOffset = Vector2.One;
    this.mLog = new StringBuilder(2048 /*0x0800*/, 2048 /*0x0800*/);
    this.mLogText = new PolygonHead.Text(2048 /*0x0800*/, this.mFont, TextAlign.Left, true, true);
    this.mLogText.DefaultColor = Vector4.One;
    this.mLogText.ShadowAlpha = 0.75f;
    this.mLogText.ShadowsOffset = Vector2.One;
    this.mMessage = new StringBuilder(202, 202);
    this.mMessageText = new PolygonHead.Text(202, this.mFont, TextAlign.Left, true, false);
    this.mMessageText.DefaultColor = Vector4.One;
    this.mMessageText.ShadowAlpha = 0.75f;
    this.mMessageText.ShadowsOffset = Vector2.One;
    this.mScrollBar = new MenuScrollBar(new Vector2(), 1f, 0);
    this.mScrollBar.TextureOffset = new Vector2(-384f, 224f);
    this.mTitleActive = false;
    VertexPositionColor[] data = new VertexPositionColor[Defines.QUAD_COL_VERTS_TL.Length];
    Defines.QUAD_COL_VERTS_TL.CopyTo((Array) data, 0);
    for (int index = 0; index < data.Length; ++index)
      data[index].Color.A = (byte) 127 /*0x7F*/;
    lock (graphicsDevice)
    {
      this.mBackgroundVertices = new VertexBuffer(graphicsDevice, VertexPositionColor.SizeInBytes * data.Length, BufferUsage.WriteOnly);
      this.mBackgroundVertices.SetData<VertexPositionColor>(data);
      this.mBackgroundDeclaration = RenderManager.Instance.CreateVertexDeclaration(VertexPositionColor.VertexElements);
    }
  }

  public void Set(
    int iWidth,
    int iHeight,
    Texture2D iBackgroundTexture,
    Rectangle iBackgroundRect,
    BitmapFont iFont,
    bool iDropShadow,
    int iMargin,
    bool iScrollBarVisible,
    float iMessageTTL)
  {
    this.mMargin = iMargin;
    this.mSize.X = iWidth;
    this.mSize.Y = iHeight;
    this.mScrollBarVisible = iScrollBarVisible;
    this.mBackgroundTexture = iBackgroundTexture;
    if (this.mBackgroundTexture != null)
    {
      this.mBackgroundTextureOffset.X = (float) iBackgroundRect.X / (float) iBackgroundTexture.Width;
      this.mBackgroundTextureOffset.Y = (float) iBackgroundRect.Y / (float) iBackgroundTexture.Height;
      this.mBackgroundTextureScale.X = (float) iBackgroundRect.Width / (float) iBackgroundTexture.Width;
      this.mBackgroundTextureScale.Y = (float) iBackgroundRect.Height / (float) iBackgroundTexture.Height;
    }
    if (this.mTitleActive)
    {
      this.mScrollBar.Height = (float) this.mSize.Y - 64f;
      this.mVisibleLines = (this.mSize.Y - this.mFont.LineHeight * 2 - this.mMargin) / this.mFont.LineHeight;
    }
    else
    {
      this.mScrollBar.Height = (float) this.mSize.Y;
      this.mVisibleLines = (this.mSize.Y - this.mMargin) / this.mFont.LineHeight;
    }
    this.mMessageTTL = iMessageTTL;
    for (int index = 0; index < this.mMessageTTLs.Count; ++index)
      this.mMessageTTLs[index] = Math.Min(this.mMessageTTLs[index], iMessageTTL);
    this.mTitle.DrawShadows = iDropShadow;
    this.mLogText.DrawShadows = iDropShadow;
    this.mMessageText.DrawShadows = iDropShadow;
    this.mTitle.Font = iFont;
    this.mLogText.Font = iFont;
    this.mMessageText.Font = iFont;
    this.mScrollBar.SetMaxValue(0);
    this.mMessage.Length = 0;
    this.mMessageText.SetText((string) null);
  }

  public Point Size => this.mSize;

  public MenuScrollBar ScrollBar => this.mScrollBar;

  public void Draw(ref Vector2 iPos)
  {
    if (this.mScrollValue != this.mScrollBar.Value)
    {
      this.mScrollValue = this.mScrollBar.Value;
      this.mDirty = true;
    }
    if (this.mDirty)
      this.UpdateText();
    Viewport viewport = this.mEffect.GraphicsDevice.Viewport;
    this.mEffect.SetScreenSize(viewport.Width, viewport.Height);
    if (this.mBackgroundTexture != null)
    {
      this.mEffect.Color = new Vector4(1f);
      this.mEffect.VertexColorEnabled = true;
      this.mEffect.TextureEnabled = false;
      this.mEffect.Transform = new Matrix()
      {
        M11 = (float) this.mSize.X,
        M22 = (float) this.mSize.Y,
        M41 = iPos.X,
        M42 = iPos.Y,
        M44 = 1f
      };
      this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mBackgroundVertices, 0, VertexPositionColor.SizeInBytes);
      this.mEffect.GraphicsDevice.VertexDeclaration = this.mBackgroundDeclaration;
      this.mEffect.Begin();
      this.mEffect.CurrentTechnique.Passes[0].Begin();
      this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
    }
    else
    {
      this.mEffect.Begin();
      this.mEffect.CurrentTechnique.Passes[0].Begin();
    }
    this.mEffect.TextureOffset = new Vector2();
    this.mEffect.TextureScale = Vector2.One;
    this.mEffect.Color = Vector4.One;
    this.mEffect.VertexColorEnabled = true;
    if (this.mScrollBarVisible)
    {
      if (this.mTitleActive)
      {
        this.mTitle.Draw(this.mEffect, iPos.X + (float) this.mSize.X * 0.5f, iPos.Y + (float) this.mMargin);
        this.mLogText.Draw(this.mEffect, iPos.X + (float) this.mMargin, iPos.Y + (float) (this.mMargin * 2) + (float) this.mFont.LineHeight);
      }
      else
        this.mLogText.Draw(this.mEffect, iPos.X + (float) this.mMargin, iPos.Y + (float) this.mMargin);
    }
    else
      this.mLogText.Draw(this.mEffect, iPos.X + (float) this.mMargin, iPos.Y + (float) this.mSize.Y - (float) ((this.mLineCount + 1) * this.mMessageText.Font.LineHeight));
    if (this.mScrollBarVisible)
      this.mMessageText.Draw(this.mEffect, iPos.X + (float) this.mMargin, iPos.Y + (float) this.mSize.Y - (float) this.mMessageText.Font.LineHeight - (float) this.mMargin);
    else if (this.mActive)
      this.mMessageText.Draw(this.mEffect, iPos.X + (float) this.mMargin, iPos.Y + (float) this.mSize.Y);
    if (this.mScrollBarVisible)
    {
      this.mEffect.VertexColorEnabled = false;
      this.mScrollBar.Scale = 0.75f;
      this.mScrollBar.Height = (float) (((double) this.mSize.Y - 64.0) / 0.75);
      this.mScrollBar.Position = new Vector2((float) ((double) iPos.X + (double) this.mSize.X - 32.0), iPos.Y + (float) this.mSize.Y * 0.5f);
      this.mScrollBar.Draw(this.mEffect);
    }
    this.mEffect.CurrentTechnique.Passes[0].End();
    this.mEffect.End();
  }

  public void AddMessage(string iMessage)
  {
    iMessage = this.mFont.Wrap(iMessage, this.mSize.X - this.mMargin * 2, true);
    int num = Regex.Matches(iMessage, "\n").Count + 1;
    if (this.mLog.Length > 0)
      iMessage = '\n'.ToString() + iMessage;
    while (this.mLog.Length + iMessage.Length >= this.mLog.Capacity)
      this.RemoveLine();
    this.mLog.Append(iMessage);
    for (int index = 0; index < num; ++index)
      this.mMessageTTLs.Add(this.mMessageTTL);
    this.mLineCount = Regex.Matches(this.mLog.ToString(), "\n").Count;
    this.mScrollBar.SetMaxValue(this.mLineCount - this.mVisibleLines);
    this.mScrollBar.Value = this.mScrollBar.MaxValue;
    this.mScrollValue = -1;
    this.mDirty = true;
  }

  private void UpdateText()
  {
    this.mDirty = false;
    int startIndex = 0;
    int length = this.mLog.Length;
    if (this.mScrollBarVisible)
    {
      MatchCollection matchCollection = Regex.Matches(this.mLog.ToString(), "\n");
      this.mLineCount = matchCollection.Count;
      if (matchCollection.Count >= this.mVisibleLines)
      {
        startIndex = matchCollection[this.mScrollBar.Value].Index + 1;
        length = this.mScrollBar.Value >= this.mScrollBar.MaxValue ? this.mLog.Length - startIndex : matchCollection[this.mScrollBar.Value + this.mVisibleLines].Index - startIndex;
      }
    }
    this.mLogText.SetText(this.mLog.ToString(startIndex, length));
  }

  private void RemoveLine()
  {
    int num = this.mLog.ToString().IndexOf('\n');
    if (num < 0)
    {
      this.mLog.Length = 0;
    }
    else
    {
      this.mLog.Remove(0, num + 1);
      --this.mLineCount;
    }
    this.mMessageTTLs.RemoveAt(0);
    this.mDirty = true;
  }

  public void TakeInput(Controller iSender, char iInput)
  {
    if (!this.mActive)
      return;
    if (iInput == '\b')
    {
      if (this.mMessage.Length > 0)
        --this.mMessage.Length;
    }
    else if (this.mMessage.Length > 0 | iInput != ' ' && this.mMessage.Length < 50)
      this.mMessage.Append(iInput);
    this.mMessageText.SetText(this.mFont.Wrap(this.mMessage.ToString(), this.mSize.X - this.mMargin * 2, true));
  }

  public void Update(float iDeltaTime)
  {
    for (int index = 0; index < this.mMessageTTLs.Count; ++index)
    {
      float num = this.mMessageTTLs[index] - iDeltaTime;
      if ((double) num <= 0.0)
      {
        this.RemoveLine();
        --index;
      }
      else
        this.mMessageTTLs[index] = num;
    }
    this.mTimer -= iDeltaTime;
    while ((double) this.mTimer < 0.0)
    {
      this.mTimer += 0.5f;
      this.mInputLine = !this.mInputLine;
      if (this.mInputLine)
      {
        this.mMessageText.Characters[this.mMessage.Length] = '_';
        this.mMessageText.Characters[this.mMessage.Length + 1] = char.MinValue;
      }
      else
        this.mMessageText.Characters[this.mMessage.Length] = char.MinValue;
      this.mMessageText.MarkAsDirty();
    }
  }

  internal void SendMessage()
  {
    if (this.mMessage.Length <= 0)
      return;
    ChatMessage iMessage = new ChatMessage();
    iMessage.Sender = SteamFriends.GetPersonaName();
    iMessage.Message = this.mMessage.ToString();
    NetworkManager.Instance.Interface.SendMessage<ChatMessage>(ref iMessage);
    NetworkChat.Instance.AddMessage(iMessage.ToString());
    this.mMessage.Length = 0;
    this.mMessageText.SetText((string) null);
  }

  internal void Clear()
  {
    this.mLog.Length = 0;
    this.mLogText.SetText((string) null);
    this.mMessage.Length = 0;
    this.mMessageText.SetText((string) null);
  }

  internal void SetTitle(string iTitle)
  {
    this.mTitleActive = iTitle.Length > 0;
    this.mTitle.SetText(iTitle);
  }

  public bool Active
  {
    get => this.mActive;
    set => this.mActive = value;
  }
}
