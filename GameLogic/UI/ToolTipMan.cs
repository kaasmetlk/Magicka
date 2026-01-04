// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.ToolTipMan
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using PolygonHead.Effects;
using System;

#nullable disable
namespace Magicka.GameLogic.UI;

internal class ToolTipMan
{
  private const float SCALE = 0.8f;
  private const float MARGIN = 2f;
  public const float FADE_IN_SPEED = 4f;
  public const float FADE_OUT_SPEED = 4f;
  public const float SHOW_DELAY = 0.25f;
  private static ToolTipMan sSingelton;
  private static volatile object sSingeltonLock = new object();
  private int mMaxWidth;
  private ToolTipMan.Tip[] mTips;
  private ToolTipMan.RenderData[] mRenderData;
  private BitmapFont mFont;
  private GUIBasicEffect mEffect;
  private VertexBuffer mVertices;
  private VertexDeclaration mVertexDeclaration;

  public static ToolTipMan Instance
  {
    get
    {
      if (ToolTipMan.sSingelton == null)
      {
        lock (ToolTipMan.sSingeltonLock)
        {
          if (ToolTipMan.sSingelton == null)
            ToolTipMan.sSingelton = new ToolTipMan();
        }
      }
      return ToolTipMan.sSingelton;
    }
  }

  private ToolTipMan()
  {
    this.mRenderData = new ToolTipMan.RenderData[3];
    Vector2[] data = new Vector2[5];
    data[0].X = 0.0f;
    data[0].Y = 0.0f;
    data[1].X = 1f;
    data[1].Y = 0.0f;
    data[2].X = 1f;
    data[2].Y = 1f;
    data[3].X = 0.0f;
    data[3].Y = 1f;
    data[4] = data[0];
    this.mTips = new ToolTipMan.Tip[5];
    for (int index = 0; index < this.mTips.Length; ++index)
    {
      this.mTips[index].ShowDelay = 0.25f;
      this.mTips[index].String = "";
      this.mTips[index].OldString = "";
    }
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    lock (graphicsDevice)
    {
      this.mFont = FontManager.Instance.GetFont(MagickaFont.Maiandra14);
      this.mVertexDeclaration = new VertexDeclaration(graphicsDevice, new VertexElement[1]
      {
        new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0)
      });
      this.mVertices = new VertexBuffer(graphicsDevice, 8 * data.Length, BufferUsage.WriteOnly);
      this.mVertices.SetData<Vector2>(data);
      this.mEffect = new GUIBasicEffect(graphicsDevice, (EffectPool) null);
      for (int index = 0; index < 3; ++index)
        this.mRenderData[index] = new ToolTipMan.RenderData(this.mTips.Length, this.mFont, this.mEffect, this.mVertices, this.mVertexDeclaration);
    }
  }

  public void Initialize(int iMaxWidth, ref Vector4 iBackgroundColor, ref Vector4 iForegroundColor)
  {
    this.mMaxWidth = (int) Math.Ceiling((double) iMaxWidth / 0.800000011920929);
    this.KillAll(true);
    for (int index = 0; index < this.mRenderData.Length; ++index)
      this.mRenderData[index].Init(iBackgroundColor, iForegroundColor);
  }

  public void Kill(Player iPlayer, bool iInstant)
  {
    int index = this.mTips.Length - 1;
    if (iPlayer != null)
      index = iPlayer.ID;
    this.mTips[index].TTL = 0.0f;
    if (!iInstant)
      return;
    this.mTips[index].ShowDelay = 0.25f;
    this.mTips[index].TTL = this.mTips[index].Alpha = 0.0f;
  }

  public void KillAll(bool iInstant)
  {
    for (int index = 0; index < this.mTips.Length; ++index)
    {
      this.mTips[index].TTL = 0.0f;
      if (iInstant)
      {
        this.mTips[index].ShowDelay = 0.25f;
        this.mTips[index].TTL = this.mTips[index].Alpha = 0.0f;
      }
    }
  }

  public void Set(Player iPlayer, string iString, MouseState iMouseState)
  {
    this.Set(iPlayer, iString, ref new Vector2()
    {
      X = (float) iMouseState.X,
      Y = (float) iMouseState.Y + 32f
    }, float.PositiveInfinity, false);
  }

  public void Set(Player iPlayer, string iString, MouseState iMouseState, float iTTL)
  {
    this.Set(iPlayer, iString, ref new Vector2()
    {
      X = (float) iMouseState.X,
      Y = (float) iMouseState.Y + 32f
    }, iTTL, false);
  }

  public void Set(
    Player iPlayer,
    string iString,
    MouseState iMouseState,
    float iTTL,
    bool iSkipWait)
  {
    this.Set(iPlayer, iString, ref new Vector2()
    {
      X = (float) iMouseState.X,
      Y = (float) iMouseState.Y + 32f
    }, iTTL, iSkipWait);
  }

  public void Set(Player iPlayer, string iString, ref Vector2 iPosition)
  {
    this.Set(iPlayer, iString, ref iPosition, float.PositiveInfinity, false);
  }

  public void Set(Player iPlayer, string iString, ref Vector2 iPosition, float iTTL)
  {
    this.Set(iPlayer, iString, ref iPosition, iTTL, false);
  }

  public void Set(
    Player iPlayer,
    string iString,
    ref Vector2 iPosition,
    float iTTL,
    bool iSkipWait)
  {
    if (iString == null)
      throw new ArgumentNullException(nameof (iString));
    int index = this.mTips.Length - 1;
    if (iPlayer != null)
      index = iPlayer.ID;
    iString = this.mFont.Wrap(iString, this.mMaxWidth, true);
    if (iSkipWait)
      this.mTips[index].ShowDelay = 0.0f;
    else if (!iString.Equals(this.mTips[index].String) || (double) this.mTips[index].TTL <= 0.0 && (double) this.mTips[index].Alpha <= 0.0)
      this.mTips[index].ShowDelay = 0.25f;
    this.mTips[index].String = iString;
    this.mTips[index].Position = iPosition;
    this.mTips[index].TTL = iTTL;
  }

  public void Update(Scene iScene, DataChannel iDataChannel, float iDeltaTime)
  {
    bool flag = false;
    for (int index = 0; index < this.mTips.Length; ++index)
    {
      this.mTips[index].ShowDelay -= iDeltaTime;
      if ((double) this.mTips[index].ShowDelay <= 0.0)
      {
        this.mTips[index].OldString = this.mTips[index].String;
        this.mTips[index].OldPosition = this.mTips[index].Position;
        this.mTips[index].Alpha = (double) this.mTips[index].TTL <= 0.0 ? Math.Max(this.mTips[index].Alpha - iDeltaTime * 4f, 0.0f) : Math.Min(this.mTips[index].Alpha + iDeltaTime * 4f, 1f);
        this.mTips[index].TTL -= iDeltaTime;
      }
      else
        this.mTips[index].Alpha = Math.Max(this.mTips[index].Alpha - iDeltaTime * 4f, 0.0f);
      flag |= (double) this.mTips[index].Alpha > 0.0;
    }
    if (!flag)
      return;
    ToolTipMan.RenderData iObject = this.mRenderData[(int) iDataChannel];
    iObject.Update(this.mTips);
    iScene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) iObject);
  }

  private class RenderData : IRenderableGUIObject
  {
    private ToolTipMan.RenderData.Data[] mData;
    private Vector4 mBackgroundColor;
    private Vector4 mForegroundColor;
    private GUIBasicEffect mEffect;
    private VertexBuffer mVertices;
    private VertexDeclaration mVertexDeclaration;

    public RenderData(
      int iCount,
      BitmapFont iFont,
      GUIBasicEffect iEffect,
      VertexBuffer iBackgroundVertices,
      VertexDeclaration iDeclaration)
    {
      this.mEffect = iEffect;
      this.mVertices = iBackgroundVertices;
      this.mVertexDeclaration = iDeclaration;
      this.mData = new ToolTipMan.RenderData.Data[iCount];
      for (int index = 0; index < this.mData.Length; ++index)
        this.mData[index].Text = new Text(512 /*0x0200*/, iFont, TextAlign.Left, false);
    }

    public void Init(Vector4 iBackgroundColor, Vector4 iForegroundColor)
    {
      this.mBackgroundColor = iBackgroundColor;
      this.mForegroundColor = iForegroundColor;
      for (int index = 0; index < this.mData.Length; ++index)
        this.mData[index].Text.DefaultColor = iForegroundColor;
    }

    public void Update(ToolTipMan.Tip[] iTips)
    {
      Point screenSize = RenderManager.Instance.ScreenSize;
      Vector2 vector2_1 = new Vector2();
      vector2_1.X = (float) screenSize.X;
      vector2_1.Y = (float) screenSize.Y;
      for (int index = 0; index < this.mData.Length; ++index)
      {
        Vector2 vector2_2;
        string oldString;
        if ((double) iTips[index].ShowDelay <= 0.0)
        {
          vector2_2 = iTips[index].Position;
          oldString = iTips[index].String;
        }
        else
        {
          vector2_2 = iTips[index].OldPosition;
          oldString = iTips[index].OldString;
        }
        Vector2 vector2_3;
        if (!oldString.Equals(this.mData[index].String))
        {
          this.mData[index].String = oldString;
          this.mData[index].Text.SetText(oldString);
          vector2_3 = this.mData[index].Text.Font.MeasureText(oldString, true);
          vector2_3.X = (float) Math.Ceiling((double) vector2_3.X * 0.800000011920929);
          vector2_3.Y = (float) Math.Ceiling((double) vector2_3.Y * 0.800000011920929);
          this.mData[index].TextSize = vector2_3;
        }
        else
          vector2_3 = this.mData[index].TextSize;
        vector2_2.X = Math.Max(Math.Min(vector2_2.X, (float) ((double) vector2_1.X - (double) vector2_3.X - 2.0)), 3f);
        vector2_2.Y = Math.Max(Math.Min(vector2_2.Y, (float) ((double) vector2_1.Y - (double) vector2_3.Y - 2.0)), 3f);
        this.mData[index].Position = vector2_2;
        this.mData[index].Alpha = iTips[index].Alpha;
      }
    }

    public void Draw(float iDeltaTime)
    {
      Point screenSize = RenderManager.Instance.ScreenSize;
      Vector2 vector2 = new Vector2()
      {
        X = (float) screenSize.X,
        Y = (float) screenSize.Y
      };
      this.mEffect.SetScreenSize(screenSize.X, screenSize.Y);
      Matrix matrix = new Matrix();
      matrix.M44 = 1f;
      this.mEffect.Begin();
      this.mEffect.CurrentTechnique.Passes[0].Begin();
      for (int index = 0; index < this.mData.Length; ++index)
      {
        float alpha = this.mData[index].Alpha;
        if ((double) alpha > 0.0)
        {
          this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertices, 0, 8);
          this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
          this.mEffect.VertexColorEnabled = false;
          this.mEffect.TextureEnabled = false;
          Vector2 position = this.mData[index].Position;
          matrix.M41 = (float) ((double) position.X - 2.0 - 0.5);
          matrix.M42 = (float) ((double) position.Y - 2.0 - 0.5);
          matrix.M11 = this.mData[index].TextSize.X + 4f;
          matrix.M22 = this.mData[index].TextSize.Y + 4f;
          this.mEffect.Transform = matrix;
          Vector4 vector4 = this.mBackgroundColor;
          vector4.W *= alpha;
          this.mEffect.Color = vector4;
          this.mEffect.CommitChanges();
          this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
          vector4 = this.mForegroundColor;
          vector4.W *= alpha;
          this.mEffect.Color = vector4;
          this.mEffect.CommitChanges();
          this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.LineStrip, 0, 4);
          vector4.X = vector4.Y = vector4.Z = 1f;
          vector4.W = alpha * alpha;
          this.mEffect.Color = vector4;
          this.mEffect.VertexColorEnabled = true;
          this.mData[index].Text.Draw(this.mEffect, position.X, position.Y, 0.8f);
        }
      }
      this.mEffect.CurrentTechnique.Passes[0].End();
      this.mEffect.End();
    }

    public int ZIndex => int.MaxValue;

    private struct Data
    {
      public string String;
      public Text Text;
      public Vector2 TextSize;
      public float Alpha;
      public Vector2 Position;
    }
  }

  private struct Tip
  {
    public float ShowDelay;
    public float TTL;
    public float Alpha;
    public Vector2 Position;
    public string String;
    public Vector2 OldPosition;
    public string OldString;
  }
}
