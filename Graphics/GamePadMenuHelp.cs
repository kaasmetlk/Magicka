// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.GamePadMenuHelp
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;

#nullable disable
namespace Magicka.Graphics;

internal sealed class GamePadMenuHelp
{
  private static GamePadMenuHelp mSingelton;
  private static volatile object mSingeltonLock = new object();
  public static readonly int LOC_BACK = "#menu_back".GetHashCodeCustom();
  public static readonly int LOC_SELECT = "#menu_select".GetHashCodeCustom();
  public static readonly int HOSTLOCHASH = "#menu_opt_online_01".GetHashCodeCustom();
  public static readonly int LOC_JOIN = "#menu_opt_online_03".GetHashCodeCustom();
  public static readonly int LOC_DISCONNECT = "#network_10".GetHashCodeCustom();
  public static readonly int LOC_KICK = "#network_06".GetHashCodeCustom();
  public static readonly int LOC_QUIT = "#menu_main_07".GetHashCodeCustom();
  public static readonly int LOC_OPEN = "#menu_open".GetHashCodeCustom();
  public static readonly int LOC_CLOSE = "#menu_close".GetHashCodeCustom();
  private GamePadMenuHelp.ButtonData[] mActionButtons = new GamePadMenuHelp.ButtonData[4];
  private GamePadMenuHelp.RenderData[] mRenderData = new GamePadMenuHelp.RenderData[3];
  private static readonly float FADE_SPEED = 3f;

  public static GamePadMenuHelp Instance
  {
    get
    {
      if (GamePadMenuHelp.mSingelton == null)
      {
        lock (GamePadMenuHelp.mSingeltonLock)
        {
          if (GamePadMenuHelp.mSingelton == null)
            GamePadMenuHelp.mSingelton = new GamePadMenuHelp();
        }
      }
      return GamePadMenuHelp.mSingelton;
    }
  }

  private GamePadMenuHelp()
  {
    for (int index = 0; index < this.mActionButtons.Length; ++index)
    {
      this.mActionButtons[index].String = " - ";
      this.mActionButtons[index].OldString = " - ";
    }
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    GUIBasicEffect iEffect;
    Texture2D iTexture;
    lock (graphicsDevice)
    {
      iEffect = new GUIBasicEffect(graphicsDevice, (EffectPool) null);
      iTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/Pages");
    }
    Vector4[] data = new Vector4[4];
    Vector2 vector2_1 = new Vector2();
    vector2_1.X = 1536f / (float) iTexture.Width;
    vector2_1.Y = 0.0f / (float) iTexture.Height;
    Vector2 vector2_2 = new Vector2();
    vector2_2.X = 80f / (float) iTexture.Width;
    vector2_2.Y = 96f / (float) iTexture.Height;
    data[0].X = -80f;
    data[0].Y = 0.0f;
    data[0].Z = vector2_1.X;
    data[0].W = vector2_1.Y;
    data[1].X = 0.0f;
    data[1].Y = 0.0f;
    data[1].Z = vector2_1.X + vector2_2.X;
    data[1].W = vector2_1.Y;
    data[2].X = 0.0f;
    data[2].Y = 96f;
    data[2].Z = vector2_1.X + vector2_2.X;
    data[2].W = vector2_1.Y + vector2_2.Y;
    data[3].X = -80f;
    data[3].Y = 96f;
    data[3].Z = vector2_1.X;
    data[3].W = vector2_1.Y + vector2_2.Y;
    VertexBuffer iVertexBuffer;
    VertexDeclaration vertexDeclaration;
    lock (graphicsDevice)
    {
      iVertexBuffer = new VertexBuffer(graphicsDevice, 16 /*0x10*/ * data.Length, BufferUsage.WriteOnly);
      iVertexBuffer.SetData<Vector4>(data);
      vertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(new VertexElement[2]
      {
        new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
        new VertexElement((short) 0, (short) 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 0)
      });
    }
    for (int index = 0; index < 3; ++index)
      this.mRenderData[index] = new GamePadMenuHelp.RenderData(iTexture, iEffect, iVertexBuffer, vertexDeclaration);
  }

  public void ActivateButton(GamePadMenuHelp.Button iButton, int iText)
  {
    string iText1 = LanguageManager.Instance.GetString(iText);
    this.ActivateButton(iButton, iText1);
  }

  public void ActivateButton(GamePadMenuHelp.Button iButton, string iText)
  {
    this.mActionButtons[(int) iButton].SetText(iText);
  }

  public void DeactivateButton(GamePadMenuHelp.Button iButton)
  {
    if (this.mActionButtons[(int) iButton].Active)
      this.mActionButtons[(int) iButton].SetText(" - ");
    this.mActionButtons[(int) iButton].Active = false;
  }

  public void DeactivateAll()
  {
    for (int index = 0; index < this.mActionButtons.Length; ++index)
    {
      if (this.mActionButtons[index].Active)
        this.mActionButtons[index].SetText(" - ");
      this.mActionButtons[index].Active = false;
    }
  }

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    bool flag1 = false;
    for (int index = 0; index < this.mActionButtons.Length; ++index)
      flag1 |= this.mActionButtons[index].Active;
    bool flag2 = flag1 & ControlManager.Instance.GamePadCount > 0;
    for (int index = 0; index < this.mActionButtons.Length; ++index)
    {
      this.mActionButtons[index].Alpha = !flag2 ? Math.Max(this.mActionButtons[index].Alpha - iDeltaTime * GamePadMenuHelp.FADE_SPEED, 0.0f) : Math.Min(this.mActionButtons[index].Alpha + iDeltaTime * GamePadMenuHelp.FADE_SPEED, 1f);
      this.mActionButtons[index].OldAlpha = Math.Max(this.mActionButtons[index].OldAlpha - iDeltaTime * GamePadMenuHelp.FADE_SPEED, 0.0f);
    }
    GamePadMenuHelp.RenderData iObject = this.mRenderData[(int) iDataChannel];
    iObject.Update(this.mActionButtons);
    GameStateManager.Instance.CurrentState.Scene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) iObject);
  }

  private class RenderData : IRenderableGUIObject
  {
    private GUIBasicEffect mEffect;
    private BitmapFont mFont;
    private GamePadMenuHelp.RenderData.Data[] mData;
    private Texture2D mTexture;
    private VertexBuffer mVertexBuffer;
    private VertexDeclaration mVertexDeclaration;

    public RenderData(
      Texture2D iTexture,
      GUIBasicEffect iEffect,
      VertexBuffer iVertexBuffer,
      VertexDeclaration iVertexDeclaration)
    {
      this.mEffect = iEffect;
      this.mTexture = iTexture;
      this.mVertexBuffer = iVertexBuffer;
      this.mVertexDeclaration = iVertexDeclaration;
      this.mFont = FontManager.Instance.GetFont(MagickaFont.Maiandra14);
      this.mData = new GamePadMenuHelp.RenderData.Data[4];
      for (int index = 0; index < this.mData.Length; ++index)
      {
        this.mData[index].Text = new Text(64 /*0x40*/, this.mFont, TextAlign.Right, false);
        this.mData[index].Text.DrawShadows = true;
        this.mData[index].Text.ShadowsOffset = Vector2.One;
        this.mData[index].OldText = new Text(64 /*0x40*/, this.mFont, TextAlign.Right, false);
        this.mData[index].OldText.DrawShadows = true;
        this.mData[index].OldText.ShadowsOffset = Vector2.One;
      }
    }

    internal void Update(GamePadMenuHelp.ButtonData[] iButtons)
    {
      for (int index = 0; index < iButtons.Length; ++index)
      {
        if (!iButtons[index].String.Equals(this.mData[index].String) && (double) iButtons[index].Alpha > 0.0)
        {
          this.mData[index].Text.SetText(iButtons[index].String);
          this.mData[index].String = iButtons[index].String;
        }
        this.mData[index].Alpha = iButtons[index].Alpha;
        if (!iButtons[index].OldString.Equals(this.mData[index].OldString) && (double) iButtons[index].OldAlpha > 0.0)
        {
          this.mData[index].OldText.SetText(iButtons[index].OldString);
          this.mData[index].OldString = iButtons[index].OldString;
        }
        this.mData[index].OldAlpha = iButtons[index].OldAlpha;
      }
    }

    public void Draw(float iDeltaTime)
    {
      Vector4 vector4 = new Vector4();
      vector4.X = vector4.Y = vector4.Z = 1f;
      Point screenSize = RenderManager.Instance.ScreenSize;
      this.mEffect.SetScreenSize(screenSize.X, screenSize.Y);
      Vector2 vector2 = new Vector2();
      vector2.X = (float) (screenSize.X - 8);
      vector2.Y = 8f;
      float val1 = 0.0f;
      this.mEffect.Begin();
      this.mEffect.CurrentTechnique.Passes[0].Begin();
      float num = 0.0f;
      for (int index = 0; index < this.mData.Length; ++index)
      {
        vector4.W = this.mData[index].Alpha;
        if ((double) vector4.W > 0.0)
        {
          val1 = Math.Max(val1, vector4.W);
          this.mEffect.Color = vector4;
          this.mData[index].Text.ShadowAlpha = vector4.W;
          this.mData[index].Text.Draw(this.mEffect, vector2.X - 84f, vector2.Y + num);
        }
        vector4.W = this.mData[index].OldAlpha;
        if ((double) vector4.W > 0.0)
        {
          val1 = Math.Max(val1, vector4.W);
          this.mEffect.Color = vector4;
          this.mData[index].OldText.ShadowAlpha = vector4.W;
          this.mData[index].OldText.Draw(this.mEffect, vector2.X - 84f, vector2.Y + num);
        }
        num += 24f;
      }
      this.mEffect.Texture = (Texture) this.mTexture;
      this.mEffect.TextureEnabled = true;
      vector4.W = val1;
      this.mEffect.Color = vector4;
      this.mEffect.Transform = new Matrix()
      {
        M11 = 1f,
        M22 = 1f,
        M41 = vector2.X,
        M42 = vector2.Y,
        M44 = 1f
      };
      this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, 16 /*0x10*/);
      this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
      this.mEffect.CommitChanges();
      this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
      this.mEffect.CurrentTechnique.Passes[0].End();
      this.mEffect.End();
    }

    public int ZIndex => 1001;

    private struct Data
    {
      public float Alpha;
      public string String;
      public Text Text;
      public float OldAlpha;
      public string OldString;
      public Text OldText;
    }
  }

  private struct ButtonData
  {
    public string String;
    public string OldString;
    public float Alpha;
    public float OldAlpha;
    public bool Active;

    public void SetText(string iText)
    {
      if (!iText.Equals(this.String))
      {
        this.OldAlpha = this.Alpha;
        this.Alpha = 0.0f;
        this.OldString = this.String;
        this.String = iText;
      }
      this.Active = true;
    }
  }

  public enum Button
  {
    Top = 0,
    Y = 0,
    Left = 1,
    X = 1,
    A = 2,
    Bottom = 2,
    B = 3,
    Right = 3,
  }
}
