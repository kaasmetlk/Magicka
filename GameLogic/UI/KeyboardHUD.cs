// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.KeyboardHUD
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;

#nullable disable
namespace Magicka.GameLogic.UI;

internal class KeyboardHUD
{
  private const float RESETTIME = 0.125f;
  private const float BLOOMTIME_DIVISOR = 8f;
  private const float COOLDOWN = 0.125f;
  private const float COOLDOWN_DIVISOR = 8f;
  private static KeyboardHUD mSingelton;
  private static volatile object mSingeltonLock = new object();
  private Text[] mKeyTexts;
  private KeyboardHUD.Icon[] mIcons;
  private KeyboardHUD.RenderData[] mRenderData;
  private bool mUIEnabled = true;

  public static KeyboardHUD Instance
  {
    get
    {
      if (KeyboardHUD.mSingelton == null)
      {
        lock (KeyboardHUD.mSingeltonLock)
        {
          if (KeyboardHUD.mSingelton == null)
            KeyboardHUD.mSingelton = new KeyboardHUD();
        }
      }
      return KeyboardHUD.mSingelton;
    }
  }

  private KeyboardHUD()
  {
    Texture2D iHudTexture = (Texture2D) null;
    lock (Magicka.Game.Instance.GraphicsDevice)
      iHudTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/HUD/hud");
    GUIBasicEffect iEffect = new GUIBasicEffect(Magicka.Game.Instance.GraphicsDevice, (EffectPool) null);
    iEffect.TextureEnabled = true;
    iEffect.Color = new Vector4(1f);
    float num1 = 1f / (float) iHudTexture.Width;
    float num2 = 1f / (float) iHudTexture.Height;
    VertexPositionTexture[] data = new VertexPositionTexture[66];
    float num3 = 50f * num1;
    float num4 = 50f * num2;
    float num5 = 50f;
    float num6 = 50f;
    float num7 = 156f * num2;
    VertexPositionTexture vertexPositionTexture = new VertexPositionTexture();
    for (int index1 = 0; index1 < 8; ++index1)
    {
      int index2 = index1 * 6;
      float num8 = (float) (index1 % 5);
      int num9 = index1 / 5;
      vertexPositionTexture.Position.X = 0.0f;
      vertexPositionTexture.Position.Y = num6;
      vertexPositionTexture.TextureCoordinate.X = num8 * num3;
      vertexPositionTexture.TextureCoordinate.Y = num7 + (float) num9 * num4 + num4;
      data[index2] = vertexPositionTexture;
      vertexPositionTexture.Position.X = 0.0f;
      vertexPositionTexture.Position.Y = 0.0f;
      vertexPositionTexture.TextureCoordinate.X = num8 * num3;
      vertexPositionTexture.TextureCoordinate.Y = num7 + (float) num9 * num4;
      data[index2 + 1] = vertexPositionTexture;
      vertexPositionTexture.Position.X = num5;
      vertexPositionTexture.Position.Y = 0.0f;
      vertexPositionTexture.TextureCoordinate.X = num8 * num3 + num3;
      vertexPositionTexture.TextureCoordinate.Y = num7 + (float) num9 * num4;
      data[index2 + 2] = vertexPositionTexture;
      vertexPositionTexture.Position.X = num5;
      vertexPositionTexture.Position.Y = 0.0f;
      vertexPositionTexture.TextureCoordinate.X = num8 * num3 + num3;
      vertexPositionTexture.TextureCoordinate.Y = num7 + (float) num9 * num4;
      data[index2 + 3] = vertexPositionTexture;
      vertexPositionTexture.Position.X = num5;
      vertexPositionTexture.Position.Y = num6;
      vertexPositionTexture.TextureCoordinate.X = num8 * num3 + num3;
      vertexPositionTexture.TextureCoordinate.Y = num7 + (float) num9 * num4 + num4;
      data[index2 + 4] = vertexPositionTexture;
      vertexPositionTexture.Position.X = 0.0f;
      vertexPositionTexture.Position.Y = num6;
      vertexPositionTexture.TextureCoordinate.X = num8 * num3;
      vertexPositionTexture.TextureCoordinate.Y = num7 + (float) num9 * num4 + num4;
      data[index2 + 5] = vertexPositionTexture;
    }
    float num10 = 25f;
    float num11 = 25f;
    float num12 = 25f * num1;
    float num13 = 25f * num2;
    vertexPositionTexture.Position.X = 0.0f;
    vertexPositionTexture.Position.Y = num11;
    vertexPositionTexture.TextureCoordinate.X = 0.0f;
    vertexPositionTexture.TextureCoordinate.Y = num13;
    data[48 /*0x30*/] = vertexPositionTexture;
    vertexPositionTexture.Position.X = 0.0f;
    vertexPositionTexture.Position.Y = 0.0f;
    vertexPositionTexture.TextureCoordinate.X = 0.0f;
    vertexPositionTexture.TextureCoordinate.Y = 0.0f;
    data[49] = vertexPositionTexture;
    vertexPositionTexture.Position.X = num10;
    vertexPositionTexture.Position.Y = 0.0f;
    vertexPositionTexture.TextureCoordinate.X = num12;
    vertexPositionTexture.TextureCoordinate.Y = 0.0f;
    data[50] = vertexPositionTexture;
    vertexPositionTexture.Position.X = num10;
    vertexPositionTexture.Position.Y = 0.0f;
    vertexPositionTexture.TextureCoordinate.X = num12;
    vertexPositionTexture.TextureCoordinate.Y = 0.0f;
    data[51] = vertexPositionTexture;
    vertexPositionTexture.Position.X = num10;
    vertexPositionTexture.Position.Y = num11;
    vertexPositionTexture.TextureCoordinate.X = num12;
    vertexPositionTexture.TextureCoordinate.Y = num13;
    data[52] = vertexPositionTexture;
    vertexPositionTexture.Position.X = 0.0f;
    vertexPositionTexture.Position.Y = num11;
    vertexPositionTexture.TextureCoordinate.X = 0.0f;
    vertexPositionTexture.TextureCoordinate.Y = num13;
    data[53] = vertexPositionTexture;
    float num14 = 25f * num1;
    float num15 = 50f;
    float num16 = 50f;
    vertexPositionTexture.Position.X = 0.0f;
    vertexPositionTexture.Position.Y = num16;
    vertexPositionTexture.TextureCoordinate.X = num14;
    vertexPositionTexture.TextureCoordinate.Y = num13;
    data[54] = vertexPositionTexture;
    vertexPositionTexture.Position.X = 0.0f;
    vertexPositionTexture.Position.Y = 0.0f;
    vertexPositionTexture.TextureCoordinate.X = num14;
    vertexPositionTexture.TextureCoordinate.Y = 0.0f;
    data[55] = vertexPositionTexture;
    vertexPositionTexture.Position.X = num15;
    vertexPositionTexture.Position.Y = 0.0f;
    vertexPositionTexture.TextureCoordinate.X = num14 + num12;
    vertexPositionTexture.TextureCoordinate.Y = 0.0f;
    data[56] = vertexPositionTexture;
    vertexPositionTexture.Position.X = num15;
    vertexPositionTexture.Position.Y = 0.0f;
    vertexPositionTexture.TextureCoordinate.X = num14 + num12;
    vertexPositionTexture.TextureCoordinate.Y = 0.0f;
    data[57] = vertexPositionTexture;
    vertexPositionTexture.Position.X = num15;
    vertexPositionTexture.Position.Y = num16;
    vertexPositionTexture.TextureCoordinate.X = num14 + num12;
    vertexPositionTexture.TextureCoordinate.Y = num13;
    data[58] = vertexPositionTexture;
    vertexPositionTexture.Position.X = 0.0f;
    vertexPositionTexture.Position.Y = num16;
    vertexPositionTexture.TextureCoordinate.X = num14;
    vertexPositionTexture.TextureCoordinate.Y = num13;
    data[59] = vertexPositionTexture;
    float num17 = 50f * num1;
    float num18 = 37f;
    float num19 = 25f;
    vertexPositionTexture.Position.X = 0.0f;
    vertexPositionTexture.Position.Y = num19;
    vertexPositionTexture.TextureCoordinate.X = num17;
    vertexPositionTexture.TextureCoordinate.Y = num13;
    data[60] = vertexPositionTexture;
    vertexPositionTexture.Position.X = 0.0f;
    vertexPositionTexture.Position.Y = 0.0f;
    vertexPositionTexture.TextureCoordinate.X = num17;
    vertexPositionTexture.TextureCoordinate.Y = 0.0f;
    data[61] = vertexPositionTexture;
    vertexPositionTexture.Position.X = num18;
    vertexPositionTexture.Position.Y = 0.0f;
    vertexPositionTexture.TextureCoordinate.X = num17 + num12;
    vertexPositionTexture.TextureCoordinate.Y = 0.0f;
    data[62] = vertexPositionTexture;
    vertexPositionTexture.Position.X = num18;
    vertexPositionTexture.Position.Y = 0.0f;
    vertexPositionTexture.TextureCoordinate.X = num17 + num12;
    vertexPositionTexture.TextureCoordinate.Y = 0.0f;
    data[63 /*0x3F*/] = vertexPositionTexture;
    vertexPositionTexture.Position.X = num18;
    vertexPositionTexture.Position.Y = num19;
    vertexPositionTexture.TextureCoordinate.X = num17 + num12;
    vertexPositionTexture.TextureCoordinate.Y = num13;
    data[64 /*0x40*/] = vertexPositionTexture;
    vertexPositionTexture.Position.X = 0.0f;
    vertexPositionTexture.Position.Y = num19;
    vertexPositionTexture.TextureCoordinate.X = num17;
    vertexPositionTexture.TextureCoordinate.Y = num13;
    data[65] = vertexPositionTexture;
    VertexBuffer iVertexBuffer;
    VertexDeclaration iVertexDeclaration;
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      iVertexBuffer = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, VertexPositionTexture.SizeInBytes * data.Length, BufferUsage.WriteOnly);
      iVertexBuffer.SetData<VertexPositionTexture>(data);
      iVertexDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
    }
    this.mIcons = new KeyboardHUD.Icon[8];
    for (int index = 0; index < this.mIcons.Length; ++index)
    {
      this.mIcons[index].Enabled = true;
      this.mIcons[index].Saturation = 1f;
      this.mIcons[index].Intensity = 1f;
      this.mIcons[index].Cooldown = 0.0f;
      this.mIcons[index].ResetTimer = 0.0f;
    }
    this.mKeyTexts = new Text[8];
    for (int index = 0; index < 8; ++index)
    {
      this.mKeyTexts[index] = new Text(32 /*0x20*/, FontManager.Instance.GetFont(MagickaFont.Maiandra14), TextAlign.Center, false);
      this.mKeyTexts[index].SetText("X");
    }
    this.mRenderData = new KeyboardHUD.RenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      this.mRenderData[index] = new KeyboardHUD.RenderData(iEffect, iHudTexture, iVertexBuffer, iVertexDeclaration, VertexPositionTexture.SizeInBytes, this.mKeyTexts);
      this.mRenderData[index].Icons = new KeyboardHUD.Icon[8];
    }
    this.UpdateControls();
  }

  public void Reset()
  {
    for (int index = 0; index < this.mIcons.Length; ++index)
      this.mIcons[index].Enabled = true;
  }

  public void CoolDown(Elements iElement)
  {
    this.mIcons[Spell.ElementIndex(iElement)].Cooldown = 0.125f;
  }

  public bool IconCoolDown(Elements iElement)
  {
    return (double) this.mIcons[Spell.ElementIndex(iElement)].Cooldown <= 0.0;
  }

  public void Enable(Elements iElement)
  {
    for (int iIndex = 0; iIndex < 11; ++iIndex)
    {
      Elements elements = Spell.ElementFromIndex(iIndex);
      if ((elements & iElement) == elements)
        this.mIcons[iIndex].Enabled = true;
    }
  }

  public void Disable(Elements iElement)
  {
    for (int iIndex = 0; iIndex < this.mIcons.Length; ++iIndex)
    {
      Elements elements = Spell.ElementFromIndex(iIndex);
      if ((elements & iElement) == elements)
      {
        this.mIcons[iIndex].Enabled = false;
        this.mIcons[iIndex].Intensity = 1f;
      }
    }
  }

  public void UpdateControls()
  {
    this.mKeyTexts[0].SetText(KeyboardMouseController.KeyToString(KeyboardBindings.Earth));
    this.mKeyTexts[1].SetText(KeyboardMouseController.KeyToString(KeyboardBindings.Water));
    this.mKeyTexts[2].SetText(KeyboardMouseController.KeyToString(KeyboardBindings.Cold));
    this.mKeyTexts[3].SetText(KeyboardMouseController.KeyToString(KeyboardBindings.Fire));
    this.mKeyTexts[4].SetText(KeyboardMouseController.KeyToString(KeyboardBindings.Lightning));
    this.mKeyTexts[5].SetText(KeyboardMouseController.KeyToString(KeyboardBindings.Arcane));
    this.mKeyTexts[6].SetText(KeyboardMouseController.KeyToString(KeyboardBindings.Life));
    this.mKeyTexts[7].SetText(KeyboardMouseController.KeyToString(KeyboardBindings.Shield));
  }

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    for (int index = 0; index < this.mIcons.Length; ++index)
    {
      if (this.mIcons[index].Enabled)
      {
        if ((double) this.mIcons[index].Cooldown > 0.0)
        {
          this.mIcons[index].Cooldown -= iDeltaTime;
          if ((double) this.mIcons[index].Cooldown <= 0.0)
            this.mIcons[index].ResetTimer = 0.125f;
          this.mIcons[index].Saturation = MathHelper.Min(this.mIcons[index].Saturation + iDeltaTime * 4f, 0.5f);
        }
        else if ((double) this.mIcons[index].ResetTimer > 0.0)
        {
          this.mIcons[index].ResetTimer -= iDeltaTime;
          this.mIcons[index].Intensity = MathHelper.Min(this.mIcons[index].Intensity + iDeltaTime * 20f, 20f);
          this.mIcons[index].Saturation = MathHelper.Min(this.mIcons[index].Saturation + iDeltaTime * 4f, 1f);
        }
        else
        {
          this.mIcons[index].Saturation = MathHelper.Min(this.mIcons[index].Saturation + iDeltaTime * 10f, 1f);
          this.mIcons[index].Intensity = MathHelper.Min(this.mIcons[index].Intensity + iDeltaTime * 4f, 1f);
        }
      }
      else
        this.mIcons[index].Saturation = MathHelper.Max(this.mIcons[index].Saturation - iDeltaTime * 4f, 0.0f);
    }
    this.mIcons.CopyTo((Array) this.mRenderData[(int) iDataChannel].Icons, 0);
    if (!this.mUIEnabled)
      return;
    GameStateManager.Instance.CurrentState.Scene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) this.mRenderData[(int) iDataChannel]);
  }

  public bool UIEnabled
  {
    get => this.mUIEnabled;
    set => this.mUIEnabled = value;
  }

  protected class RenderData : IRenderableGUIObject
  {
    private GUIBasicEffect mEffect;
    private Texture2D mHudTexture;
    private VertexBuffer mVertexBuffer;
    private VertexDeclaration mVertexDeclaration;
    private int mVertexStride;
    public KeyboardHUD.Icon[] Icons;
    private Text[] mKeyTexts;
    private Point mScreenSize;
    private Matrix mTransform;

    public RenderData(
      GUIBasicEffect iEffect,
      Texture2D iHudTexture,
      VertexBuffer iVertexBuffer,
      VertexDeclaration iVertexDeclaration,
      int iVertexStride,
      Text[] iKeyTexts)
    {
      this.mEffect = iEffect;
      this.mHudTexture = iHudTexture;
      this.mVertexBuffer = iVertexBuffer;
      this.mVertexDeclaration = iVertexDeclaration;
      this.mVertexStride = iVertexStride;
      this.mKeyTexts = iKeyTexts;
      this.mTransform = Matrix.Identity;
    }

    public void Draw(float iDeltaTime)
    {
      this.mScreenSize = RenderManager.Instance.ScreenSize;
      this.mEffect.Begin();
      this.mEffect.CurrentTechnique.Passes[0].Begin();
      float iXOffset = 25f;
      this.DrawIcon(0, 1, 0.0f, 0.0f);
      this.DrawIcon(0, 4, iXOffset, 60f);
      this.DrawIcon(1, 6, 0.0f, 0.0f);
      this.DrawIcon(1, 5, iXOffset, 60f);
      this.DrawIcon(2, 7, 0.0f, 0.0f);
      this.DrawIcon(2, 0, iXOffset, 60f);
      this.DrawIcon(3, 2, 0.0f, 0.0f);
      this.DrawIcon(3, 3, iXOffset, 60f);
      this.mEffect.CurrentTechnique.Passes[0].End();
      this.mEffect.End();
      this.mEffect.Saturation = 1f;
    }

    private void DrawIcon(int iPosition, int iElementIndex, float iXOffset, float iYOffset)
    {
      this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, this.mVertexStride);
      this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
      this.mEffect.SetScreenSize(this.mScreenSize.X, this.mScreenSize.Y);
      this.mEffect.ScaleToHDR = true;
      this.mEffect.Saturation = this.Icons[iElementIndex].Saturation;
      this.mEffect.Color = new Vector4(this.Icons[iElementIndex].Intensity, this.Icons[iElementIndex].Intensity, this.Icons[iElementIndex].Intensity, 1f);
      this.mEffect.TextureEnabled = true;
      this.mEffect.Texture = (Texture) this.mHudTexture;
      this.mTransform.M41 = 140f + (float) ((iPosition - 2) * 50) + iXOffset;
      this.mTransform.M42 = (float) this.mScreenSize.Y - 140f + iYOffset;
      this.mTransform.M41 -= 5f;
      this.mTransform.M42 += 5f;
      this.mEffect.Transform = this.mTransform;
      this.mEffect.CommitChanges();
      this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 54, 2);
      this.mTransform.M41 += 5f;
      this.mTransform.M42 -= 5f;
      this.mEffect.Transform = this.mTransform;
      this.mEffect.CommitChanges();
      this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, iElementIndex * 6, 2);
      this.mTransform.M41 -= 12f;
      this.mTransform.M42 -= 12f;
      this.mEffect.Transform = this.mTransform;
      this.mEffect.CommitChanges();
      this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 48 /*0x30*/, 2);
      this.mTransform.M41 += 14f;
      this.mKeyTexts[iElementIndex].Draw(this.mEffect, ref this.mTransform);
    }

    public int ZIndex => 600;
  }

  public struct Icon
  {
    public bool Enabled;
    public float Saturation;
    public float Cooldown;
    public float ResetTimer;
    public float Intensity;
  }
}
